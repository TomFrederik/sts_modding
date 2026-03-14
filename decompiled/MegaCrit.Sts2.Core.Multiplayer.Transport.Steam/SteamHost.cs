using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Platform.Steam;
using Steamworks;

namespace MegaCrit.Sts2.Core.Multiplayer.Transport.Steam;

public class SteamHost : NetHost
{
	private struct ClientConnection
	{
		public HSteamNetConnection conn;

		public SteamNetworkingIdentity netId;
	}

	private static readonly List<ClientConnection> _connectionsCache = new List<ClientConnection>();

	private readonly Logger _logger = new Logger("SteamHost", LogType.Network);

	private Callback<SteamNetConnectionStatusChangedCallback_t>? _netStatusChangedCallback;

	private CSteamID? _lobbyId;

	private HSteamListenSocket _socket;

	private readonly List<ClientConnection> _connections = new List<ClientConnection>();

	private bool _isConnected;

	public override bool IsConnected => _isConnected;

	public override ulong NetId => SteamUser.GetSteamID().m_SteamID;

	public override IEnumerable<ulong> ConnectedPeerIds => _connections.Select((ClientConnection c) => ((SteamNetworkingIdentity)(ref c.netId)).GetSteamID64());

	public CSteamID? LobbyId => _lobbyId;

	public SteamHost(INetHostHandler handler)
		: base(handler)
	{
	}

	public async Task<NetErrorInfo?> StartHost(int maxPlayers)
	{
		_logger.Info($"Initializing Steam host. Our player id: {NetId}");
		SteamAPICall_t call = SteamMatchmaking.CreateLobby((ELobbyType)1, maxPlayers);
		using SteamCallResult<LobbyCreated_t> callResult = new SteamCallResult<LobbyCreated_t>(call);
		LobbyCreated_t val = await callResult.Task;
		if ((int)val.m_eResult != 1)
		{
			_logger.Error($"Error creating steam lobby! {val.m_eResult}");
			return new NetErrorInfo(val.m_eResult);
		}
		_lobbyId = new CSteamID(val.m_ulSteamIDLobby);
		_socket = SteamNetworkingSockets.CreateListenSocketP2P(0, 0, (SteamNetworkingConfigValue_t[])(object)new SteamNetworkingConfigValue_t[2]
		{
			new SteamNetworkingConfigValue_t
			{
				m_eValue = (ESteamNetworkingConfigValue)24,
				m_eDataType = (ESteamNetworkingConfigDataType)1,
				m_val = new OptionValue
				{
					m_int32 = 20000
				}
			},
			new SteamNetworkingConfigValue_t
			{
				m_eValue = (ESteamNetworkingConfigValue)25,
				m_eDataType = (ESteamNetworkingConfigDataType)1,
				m_val = new OptionValue
				{
					m_int32 = 20000
				}
			}
		});
		_netStatusChangedCallback = new Callback<SteamNetConnectionStatusChangedCallback_t>((DispatchDelegate<SteamNetConnectionStatusChangedCallback_t>)OnNetStatusChanged, false);
		_isConnected = true;
		return null;
	}

	public override void Update()
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		_connectionsCache.Clear();
		_connectionsCache.AddRange(_connections);
		foreach (ClientConnection item in _connectionsCache)
		{
			SteamUtil.ProcessMessages(item.conn, _handler, _logger);
		}
	}

	public override void SetHostIsClosed(bool isClosed)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		SteamMatchmaking.SetLobbyType(_lobbyId.Value, (ELobbyType)(!isClosed));
	}

	private void OnNetStatusChanged(SteamNetConnectionStatusChangedCallback_t data)
	{
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Invalid comparison between Unknown and I4
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Invalid comparison between Unknown and I4
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_026d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Invalid comparison between Unknown and I4
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Invalid comparison between Unknown and I4
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0304: Invalid comparison between Unknown and I4
		//IL_0296: Unknown result type (might be due to invalid IL or missing references)
		//IL_0297: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0324: Unknown result type (might be due to invalid IL or missing references)
		//IL_0325: Unknown result type (might be due to invalid IL or missing references)
		//IL_0361: Unknown result type (might be due to invalid IL or missing references)
		//IL_0362: Unknown result type (might be due to invalid IL or missing references)
		//IL_0367: Unknown result type (might be due to invalid IL or missing references)
		//IL_0368: Unknown result type (might be due to invalid IL or missing references)
		_logger.Debug($"Connection status changed: ({((SteamNetworkingIdentity)(ref data.m_info.m_identityRemote)).GetSteamID64()}: {data.m_eOldState} -> {data.m_info.m_eState}");
		if ((int)data.m_info.m_eState == 1)
		{
			if (!IsInLobby(data.m_info.m_identityRemote))
			{
				_logger.Warn($"Player with steam id {((SteamNetworkingIdentity)(ref data.m_info.m_identityRemote)).GetSteamID64()} attempted to join the game, but they are not in the lobby (id {_lobbyId.Value})");
				SteamNetworkingSockets.CloseConnection(data.m_hConn, 0, "Player is not in the lobby!", false);
				return;
			}
			_logger.Info($"Accepting new connection with user {((SteamNetworkingIdentity)(ref data.m_info.m_identityRemote)).GetSteamID64()}");
			EResult val = SteamNetworkingSockets.AcceptConnection(data.m_hConn);
			if ((int)val != 1)
			{
				_logger.Error($"Tried to accept connection with user {((SteamNetworkingIdentity)(ref data.m_info.m_identityRemote)).GetSteamID64()} but it returned result {val}!");
				CloseConnectionAndRemove(data.m_hConn, SteamDisconnectionReason.AppInternalError, $"Connection accept failure: {val}", now: true, selfInitiated: false);
			}
		}
		else if ((int)data.m_info.m_eState == 3)
		{
			_connections.Add(new ClientConnection
			{
				conn = data.m_hConn,
				netId = data.m_info.m_identityRemote
			});
			_handler.OnPeerConnected(((SteamNetworkingIdentity)(ref data.m_info.m_identityRemote)).GetSteamID64());
		}
		else if ((int)data.m_info.m_eState == 5)
		{
			_logger.Info($"Steam connection closed because of problem. Reason: {data.m_info.m_eEndReason}, {((SteamNetConnectionInfo_t)(ref data.m_info)).m_szEndDebug}");
			CloseConnectionAndRemove(data.m_hConn, (SteamDisconnectionReason)data.m_info.m_eEndReason, ((SteamNetConnectionInfo_t)(ref data.m_info)).m_szEndDebug, now: true, selfInitiated: false);
		}
		else if ((int)data.m_info.m_eState == 4)
		{
			_logger.Info($"Steam connection closed by peer. Reason: {data.m_info.m_eEndReason}, {((SteamNetConnectionInfo_t)(ref data.m_info)).m_szEndDebug}");
			CloseConnectionAndRemove(data.m_hConn, (SteamDisconnectionReason)data.m_info.m_eEndReason, ((SteamNetConnectionInfo_t)(ref data.m_info)).m_szEndDebug, now: true, selfInitiated: false);
		}
	}

	public override void SendMessageToClient(ulong peerId, byte[] bytes, int length, NetTransferMode mode, int channel = 0)
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Invalid comparison between Unknown and I4
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		_logger.VeryDebug($"Sending {length} bytes to client {peerId}");
		SteamNetworkingIdentity val = default(SteamNetworkingIdentity);
		((SteamNetworkingIdentity)(ref val)).SetSteamID64(peerId);
		ClientConnection? connectionForNetId = GetConnectionForNetId(peerId);
		if (!connectionForNetId.HasValue)
		{
			throw new InvalidOperationException($"Could not find connection for peer {peerId}!");
		}
		GCHandle gCHandle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
		try
		{
			long num = default(long);
			EResult val2 = SteamNetworkingSockets.SendMessageToConnection(connectionForNetId.Value.conn, (IntPtr)gCHandle.AddrOfPinnedObject(), (uint)length, SteamUtil.FlagsFromMode(mode), ref num);
			if ((int)val2 != 1)
			{
				_logger.Warn($"Failed to send message length {length} to peer {peerId}: {val2}");
			}
		}
		finally
		{
			gCHandle.Free();
		}
	}

	public override void SendMessageToAll(byte[] bytes, int length, NetTransferMode mode, int channel = 0)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		foreach (ClientConnection connection in _connections)
		{
			SteamNetworkingIdentity netId = connection.netId;
			SendMessageToClient(((SteamNetworkingIdentity)(ref netId)).GetSteamID64(), bytes, length, mode, channel);
		}
	}

	private ClientConnection? GetConnectionForNetId(ulong peerId)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		foreach (ClientConnection connection in _connections)
		{
			SteamNetworkingIdentity netId = connection.netId;
			if (((SteamNetworkingIdentity)(ref netId)).GetSteamID64() == peerId)
			{
				return connection;
			}
		}
		return null;
	}

	public override void DisconnectClient(ulong peerId, NetError reason, bool now = false)
	{
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		HSteamNetConnection? val = null;
		_logger.Debug($"Disconnecting peer {peerId}, reason: {reason}");
		foreach (ClientConnection connection in _connections)
		{
			SteamNetworkingIdentity netId = connection.netId;
			if (((SteamNetworkingIdentity)(ref netId)).GetSteamID64() == peerId)
			{
				val = connection.conn;
				break;
			}
		}
		if (val.HasValue)
		{
			CloseConnectionAndRemove(val.Value, reason.ToSteam(), null, now, selfInitiated: true);
		}
	}

	private void CloseConnectionAndRemove(HSteamNetConnection conn, SteamDisconnectionReason reason, string? debugReason, bool now, bool selfInitiated)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		SteamNetworkingSockets.CloseConnection(conn, (int)reason, debugReason, !now);
		int num = _connections.FindIndex((ClientConnection c) => c.conn == conn);
		if (num >= 0)
		{
			ClientConnection clientConnection = _connections[num];
			_connections.RemoveAt(num);
			_handler.OnPeerDisconnected(((SteamNetworkingIdentity)(ref clientConnection.netId)).GetSteamID64(), new NetErrorInfo(reason, debugReason, selfInitiated));
		}
	}

	public override void StopHost(NetError reason, bool now = false)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		_logger.Debug("Stopping host");
		foreach (ClientConnection connection in _connections)
		{
			SteamNetworkingSockets.CloseConnection(connection.conn, (int)reason.ToSteam(), (string)null, !now);
		}
		_connections.Clear();
		SteamNetworkingSockets.CloseListenSocket(_socket);
		SteamMatchmaking.LeaveLobby(_lobbyId.Value);
		_lobbyId = null;
		_isConnected = false;
		_netStatusChangedCallback?.Dispose();
		_netStatusChangedCallback = null;
		_handler.OnDisconnected(new NetErrorInfo(reason, selfInitiated: true));
	}

	private bool IsInLobby(SteamNetworkingIdentity id)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		CSteamID steamID = ((SteamNetworkingIdentity)(ref id)).GetSteamID();
		if (steamID == CSteamID.Nil)
		{
			return false;
		}
		int numLobbyMembers = SteamMatchmaking.GetNumLobbyMembers(_lobbyId.Value);
		for (int i = 0; i < numLobbyMembers; i++)
		{
			CSteamID lobbyMemberByIndex = SteamMatchmaking.GetLobbyMemberByIndex(_lobbyId.Value, i);
			if (steamID == lobbyMemberByIndex)
			{
				return true;
			}
		}
		return false;
	}

	public override string? GetRawLobbyIdentifier()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		ref CSteamID? lobbyId = ref _lobbyId;
		if (!lobbyId.HasValue)
		{
			return null;
		}
		return lobbyId.GetValueOrDefault().m_SteamID.ToString();
	}
}
