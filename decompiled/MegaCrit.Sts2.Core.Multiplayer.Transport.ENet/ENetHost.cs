using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Logging;

namespace MegaCrit.Sts2.Core.Multiplayer.Transport.ENet;

public class ENetHost : NetHost
{
	private record struct ClientConnection
	{
		public ulong netId;

		public ENetPacketPeer peer;
	}

	private struct HandshakeAwaitingResponse
	{
		public ulong receivedMsec;

		public ClientConnection conn;
	}

	private const int _handshakeTimeoutMsec = 10000;

	private const int _handshakeUpdateRateMsec = 100;

	private readonly List<ClientConnection> _connectedPeers = new List<ClientConnection>();

	private ENetConnection? _connection;

	private bool _isConnected;

	private readonly List<HandshakeAwaitingResponse> _receivedHandshakes = new List<HandshakeAwaitingResponse>();

	private readonly Logger _logger = new Logger("ENetHost", LogType.Network);

	public override bool IsConnected => _isConnected;

	public override IEnumerable<ulong> ConnectedPeerIds => _connectedPeers.Select((ClientConnection c) => c.netId);

	public override ulong NetId => 1uL;

	public ENetHost(INetHostHandler handler)
		: base(handler)
	{
	}

	public NetErrorInfo? StartHost(ushort port, int maxClients)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		_connection = new ENetConnection();
		Error val = _connection.CreateHostBound("0.0.0.0", (int)port, maxClients, 0, 0, 0);
		if ((int)val != 0)
		{
			_logger.Error($"Failed to create host! {val}");
			return new NetErrorInfo(val);
		}
		_isConnected = true;
		return null;
	}

	private async Task DoClientHandshake(ENetPacketPeer peer)
	{
		peer.SetTimeout(24, 20000, 20000);
		int timeoutTimer = 0;
		HandshakeAwaitingResponse? handshake = null;
		while (!handshake.HasValue)
		{
			foreach (HandshakeAwaitingResponse receivedHandshake in _receivedHandshakes)
			{
				if (receivedHandshake.conn.peer == peer)
				{
					handshake = receivedHandshake;
					break;
				}
			}
			if (!handshake.HasValue)
			{
				await Task.Delay(100);
				timeoutTimer += 100;
				if (timeoutTimer >= 10000)
				{
					_logger.Error("Timed out waiting for handshake!");
					peer.Reset();
					return;
				}
			}
		}
		if (GetConnectionById(handshake.Value.conn.netId).HasValue)
		{
			_logger.Info($"Second client attempted to connect with peer ID {handshake.Value.conn.netId}, disconnecting them");
			ENetPacket eNetPacket = ENetPacket.FromHandshakeResponse(new ENetHandshakeResponse
			{
				netId = handshake.Value.conn.netId,
				status = ENetHandshakeStatus.IdCollision
			});
			handshake.Value.conn.peer.Send(0, eNetPacket.AllBytes, 1);
			handshake.Value.conn.peer.PeerDisconnect(0);
		}
		else
		{
			_logger.Debug($"Acknowledging handshake for peer with ID {handshake.Value.conn.netId}");
			ENetPacket eNetPacket2 = ENetPacket.FromHandshakeResponse(new ENetHandshakeResponse
			{
				netId = handshake.Value.conn.netId,
				status = ENetHandshakeStatus.Success
			});
			handshake.Value.conn.peer.Send(0, eNetPacket2.AllBytes, 1);
			_connectedPeers.Add(handshake.Value.conn);
			_handler.OnPeerConnected(handshake.Value.conn.netId);
		}
	}

	private void HandleClientDisconnection(ClientConnection conn, NetError reason, bool notifyHandler = true)
	{
		_logger.Debug($"Peer ID {conn.netId} disconnected, reason: {reason}");
		_connectedPeers.Remove(conn);
		if (notifyHandler)
		{
			_handler.OnPeerDisconnected(conn.netId, new NetErrorInfo(reason, selfInitiated: false));
		}
	}

	private ClientConnection? GetConnectionByPeer(ENetPacketPeer peer)
	{
		foreach (ClientConnection connectedPeer in _connectedPeers)
		{
			if (connectedPeer.peer == peer)
			{
				return connectedPeer;
			}
		}
		return null;
	}

	private ClientConnection? GetConnectionById(ulong id)
	{
		foreach (ClientConnection connectedPeer in _connectedPeers)
		{
			if (connectedPeer.netId == id)
			{
				return connectedPeer;
			}
		}
		return null;
	}

	public override void Update()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Invalid comparison between Unknown and I8
		AssertHostStarted();
		ENetServiceData? output;
		while (_connection.TryService(out output))
		{
			EventType type = output.Value.type;
			_003F val = type - -1;
			if ((long)val <= 4L)
			{
				switch ((uint)val)
				{
				case 3u:
					continue;
				case 0u:
					_logger.Error("Got error from ENetConnection! TODO: Expand me");
					continue;
				case 2u:
					TaskHelper.RunSafely(DoClientHandshake(output.Value.peer));
					continue;
				case 4u:
					HandlePacketReceived(output.Value);
					continue;
				}
			}
			throw new ArgumentOutOfRangeException();
		}
	}

	private void HandlePacketReceived(ENetServiceData data)
	{
		byte[] packetData = data.packetData;
		_logger.VeryDebug($"Received packet of length {packetData.Length}");
		ENetPacketPeer peer = data.peer;
		ENetPacket eNetPacket = new ENetPacket(packetData);
		if (eNetPacket.PacketType == ENetPacketType.HandshakeRequest)
		{
			ENetHandshakeRequest eNetHandshakeRequest = eNetPacket.AsHandshakeRequest();
			ClientConnection conn = new ClientConnection
			{
				peer = peer,
				netId = eNetHandshakeRequest.netId
			};
			HandshakeAwaitingResponse item = new HandshakeAwaitingResponse
			{
				conn = conn,
				receivedMsec = Time.GetTicksMsec()
			};
			_logger.Debug($"Received handshake packet containing peer ID {conn.netId}");
			_receivedHandshakes.Add(item);
		}
		else if (eNetPacket.PacketType == ENetPacketType.Disconnection)
		{
			ENetDisconnection eNetDisconnection = eNetPacket.AsDisconnection();
			ClientConnection? connectionByPeer = GetConnectionByPeer(peer);
			if (connectionByPeer.HasValue)
			{
				HandleClientDisconnection(connectionByPeer.Value, eNetDisconnection.reason);
			}
		}
		else
		{
			ClientConnection? connectionByPeer2 = GetConnectionByPeer(peer);
			if (!connectionByPeer2.HasValue)
			{
				_logger.Error($"Received a non-handshake packet length {packetData.Length} for a peer, but no connection exists!");
			}
			else
			{
				_handler.OnPacketReceived(connectionByPeer2.Value.netId, eNetPacket.AsAppMessage(), data.mode, data.channel);
			}
		}
	}

	public override void SendMessageToClient(ulong peerId, byte[] bytes, int length, NetTransferMode mode, int channel = 0)
	{
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		AssertHostStarted();
		ClientConnection? connectionById = GetConnectionById(peerId);
		if (!connectionById.HasValue)
		{
			_logger.Error($"Tried to send message to client with ID {peerId}, but no client with that ID is connected!");
		}
		else
		{
			ENetPacket eNetPacket = ENetPacket.FromAppMessage(bytes, length);
			_logger.VeryDebug($"Sending packet of length {eNetPacket.AllBytes.Length}");
			connectionById.Value.peer.Send(channel, eNetPacket.AllBytes, ENetUtil.FlagsFromMode(mode));
		}
	}

	public override void DisconnectClient(ulong peerId, NetError reason, bool now = false)
	{
		DisconnectClientInternal(peerId, reason, now);
	}

	private void DisconnectClientInternal(ulong peerId, NetError reason, bool now = false, bool notifyHandler = true)
	{
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		_logger.Info($"Disconnecting client {peerId}, reason: {reason}. Now: {now}");
		AssertHostStarted();
		ClientConnection? connectionById = GetConnectionById(peerId);
		if (connectionById.HasValue)
		{
			if (now)
			{
				connectionById.Value.peer.PeerDisconnectNow(0);
			}
			else
			{
				ENetPacket eNetPacket = ENetPacket.FromDisconnection(new ENetDisconnection
				{
					reason = reason
				});
				connectionById.Value.peer.Send(0, eNetPacket.AllBytes, 1);
				connectionById.Value.peer.PeerDisconnectLater(0);
			}
			HandleClientDisconnection(connectionById.Value, reason, notifyHandler);
		}
	}

	public override void SendMessageToAll(byte[] bytes, int length, NetTransferMode mode, int channel = 0)
	{
		AssertHostStarted();
		foreach (ClientConnection connectedPeer in _connectedPeers)
		{
			SendMessageToClient(connectedPeer.netId, bytes, length, mode, channel);
		}
	}

	public override void SetHostIsClosed(bool isClosed)
	{
	}

	public override void StopHost(NetError reason, bool now = false)
	{
		Log.Info($"Stopping host. Reason: {reason}. Now: {now}");
		foreach (ClientConnection item in _connectedPeers.ToList())
		{
			DisconnectClientInternal(item.netId, reason, now, notifyHandler: false);
		}
		_connectedPeers.Clear();
		_connection.Flush();
		_connection.Destroy();
		_isConnected = false;
		_handler.OnDisconnected(new NetErrorInfo(reason, selfInitiated: true));
	}

	private void AssertHostStarted()
	{
		if (_connection == null)
		{
			throw new InvalidOperationException("Must call StartHost first!");
		}
	}

	public override string? GetRawLobbyIdentifier()
	{
		return null;
	}
}
