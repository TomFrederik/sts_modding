using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Godot;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Logging;

namespace MegaCrit.Sts2.Core.Multiplayer.Transport.ENet;

public class ENetClient : NetClient
{
	private const int _handshakeTimeoutMsec = 10000;

	private const int _handshakeUpdateRateMsec = 100;

	private readonly Logger _logger = new Logger("ENetClient", LogType.Network);

	private ENetConnection? _connection;

	private ENetPacketPeer? _peer;

	private bool _isConnected;

	private ulong _netId;

	public override bool IsConnected => _isConnected;

	public override ulong NetId => _netId;

	public override ulong HostNetId => 1uL;

	public ENetClient(INetClientHandler handler)
		: base(handler)
	{
	}

	public async Task<NetErrorInfo?> ConnectToHost(ulong netId, string ip, ushort port, CancellationToken cancelToken = default(CancellationToken))
	{
		_connection = new ENetConnection();
		_connection.CreateHost(32, 0, 0, 0);
		_peer = _connection.ConnectToHost(ip, (int)port, 0, 0);
		int timeoutTimer = 0;
		ENetServiceData? output;
		while (!_connection.TryService(out output) || (long)output.Value.type != 1)
		{
			await Task.Delay(100, cancelToken);
			if (cancelToken.IsCancellationRequested)
			{
				DisconnectFromHost(NetError.CancelledJoin);
				_logger.Warn("User cancelled join flow");
				return null;
			}
			timeoutTimer += 100;
			if (timeoutTimer > 10000)
			{
				_peer.Reset();
				_logger.Error("Connection timed out!");
				return new NetErrorInfo(NetError.Timeout, selfInitiated: false);
			}
		}
		if ((long)_peer.GetState() != 5)
		{
			_logger.Error($"Connection to {ip}:{port} failed!");
			return new NetErrorInfo(NetError.UnknownNetworkError, selfInitiated: false);
		}
		List<ENetServiceData> bufferedPackets = new List<ENetServiceData>();
		NetErrorInfo? result = await SendAndWaitForNetIdAck(netId, bufferedPackets, cancelToken);
		if (result.HasValue)
		{
			_peer.PeerDisconnect(0);
			return result;
		}
		_netId = netId;
		_isConnected = true;
		_handler.OnConnectedToHost();
		foreach (ENetServiceData item in bufferedPackets)
		{
			HandleMessageReceived(item);
		}
		return null;
	}

	private async Task<NetErrorInfo?> SendAndWaitForNetIdAck(ulong netId, List<ENetServiceData> bufferedPackets, CancellationToken cancelToken = default(CancellationToken))
	{
		_logger.Info($"Sending handshake with net ID {netId}");
		ENetPacket eNetPacket = ENetPacket.FromHandshakeRequest(new ENetHandshakeRequest
		{
			netId = netId
		});
		_peer.Send(0, eNetPacket.AllBytes, 1);
		bool receivedAck = false;
		int timeoutTimer = 0;
		while (!receivedAck)
		{
			await Task.Delay(100, cancelToken);
			if (cancelToken.IsCancellationRequested)
			{
				_logger.Warn("User cancelled join flow");
				DisconnectFromHost(NetError.CancelledJoin);
				return null;
			}
			if (_connection.TryService(out var output) && (long)output.Value.type == 3)
			{
				byte[] packetData = output.Value.packetData;
				ENetPacket eNetPacket2 = new ENetPacket(packetData);
				if (eNetPacket2.PacketType == ENetPacketType.ApplicationMessage)
				{
					bufferedPackets.Add(output.Value);
					continue;
				}
				ENetHandshakeResponse eNetHandshakeResponse = eNetPacket2.AsHandshakeResponse();
				if (eNetHandshakeResponse.netId != netId)
				{
					_logger.Error($"Received net ID ({eNetHandshakeResponse.netId}) during handshake that did not match ours!");
					return new NetErrorInfo(NetError.InternalError, selfInitiated: false);
				}
				if (eNetHandshakeResponse.status != ENetHandshakeStatus.Success)
				{
					_logger.Error($"Received non-success code during handshake ({eNetHandshakeResponse.status})!");
					return new NetErrorInfo(NetError.Kicked, selfInitiated: false);
				}
				receivedAck = true;
			}
			timeoutTimer += 100;
			if (timeoutTimer > 10000)
			{
				_logger.Error("Timed out waiting for handshake ack!");
				DisconnectFromHost(NetError.Timeout);
				return new NetErrorInfo(NetError.Timeout, selfInitiated: false);
			}
		}
		return null;
	}

	public override void Update()
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Invalid comparison between Unknown and I8
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		AssertClientStarted();
		while (true)
		{
			ENetPacketPeer? peer = _peer;
			if (peer == null || !peer.IsActive())
			{
				break;
			}
			ENetConnection? connection = _connection;
			if (connection == null || !connection.TryService(out var output))
			{
				break;
			}
			EventType type = output.Value.type;
			_003F val = type - -1;
			if ((long)val <= 4L)
			{
				switch ((uint)val)
				{
				case 0u:
					_logger.Error($"Got error from ENetConnection! Error: {output.Value.error} TODO: Expand me");
					continue;
				case 2u:
					_logger.Debug("Received connect on client");
					continue;
				case 3u:
					_logger.Debug($"Received disconnect on client. Already disconnected: {!_isConnected}");
					if (_isConnected)
					{
						_isConnected = false;
						_handler.OnDisconnectedFromHost(HostNetId, new NetErrorInfo(NetError.UnknownNetworkError, selfInitiated: false));
					}
					continue;
				case 4u:
					HandleMessageReceived(output.Value);
					continue;
				}
			}
			throw new ArgumentOutOfRangeException();
		}
	}

	private void HandleMessageReceived(ENetServiceData data)
	{
		if (_isConnected)
		{
			_logger.VeryDebug($"Received packet of length {data.packetData.Length}");
			ENetPacket eNetPacket = new ENetPacket(data.packetData);
			if (eNetPacket.PacketType == ENetPacketType.ApplicationMessage)
			{
				_handler.OnPacketReceived(1uL, eNetPacket.AsAppMessage(), data.mode, data.channel);
			}
			else if (eNetPacket.PacketType == ENetPacketType.Disconnection)
			{
				ENetDisconnection eNetDisconnection = eNetPacket.AsDisconnection();
				_logger.Debug($"Received disconnection packet with reason: {eNetDisconnection.reason}");
				_handler.OnDisconnectedFromHost(HostNetId, new NetErrorInfo(eNetDisconnection.reason, selfInitiated: false));
				_isConnected = false;
			}
			else
			{
				_logger.Error($"Got unexpected packet of type {eNetPacket.PacketType} while we were connected to the host!");
			}
		}
	}

	public override void DisconnectFromHost(NetError reason, bool now = false)
	{
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		if (now)
		{
			ENetPacketPeer? peer = _peer;
			if (peer != null)
			{
				peer.PeerDisconnectNow(0);
			}
		}
		else
		{
			ENetPacket eNetPacket = ENetPacket.FromDisconnection(new ENetDisconnection
			{
				reason = reason
			});
			ENetPacketPeer? peer2 = _peer;
			if (peer2 != null)
			{
				peer2.Send(0, eNetPacket.AllBytes, 8);
			}
			ENetPacketPeer? peer3 = _peer;
			if (peer3 != null)
			{
				peer3.PeerDisconnectLater(0);
			}
		}
		ENetConnection? connection = _connection;
		if (connection != null)
		{
			connection.Flush();
		}
		if (_isConnected)
		{
			_isConnected = false;
			_handler.OnDisconnectedFromHost(HostNetId, new NetErrorInfo(reason, selfInitiated: true));
			ENetConnection? connection2 = _connection;
			if (connection2 != null)
			{
				connection2.Destroy();
			}
		}
	}

	public override void SendMessageToHost(byte[] bytes, int length, NetTransferMode mode, int channel = 0)
	{
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		if (!_isConnected)
		{
			throw new InvalidOperationException("Tried to send message to host while disconnected!");
		}
		AssertClientStarted();
		ENetPacket eNetPacket = ENetPacket.FromAppMessage(bytes, length);
		_logger.VeryDebug($"Sending packet of length {eNetPacket.AllBytes.Length}");
		ENetPacketPeer? peer = _peer;
		if (peer != null)
		{
			peer.Send(channel, eNetPacket.AllBytes, ENetUtil.FlagsFromMode(mode));
		}
	}

	private void AssertClientStarted()
	{
		if (_connection == null)
		{
			throw new InvalidOperationException("Must call ConnectToHost first and wait for connection!");
		}
	}

	public override string? GetRawLobbyIdentifier()
	{
		return null;
	}
}
