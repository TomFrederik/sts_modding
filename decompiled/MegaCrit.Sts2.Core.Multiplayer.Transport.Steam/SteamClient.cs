using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Platform.Steam;
using Steamworks;

namespace MegaCrit.Sts2.Core.Multiplayer.Transport.Steam;

public class SteamClient : NetClient
{
	private struct ConnectionResult
	{
		public HSteamNetConnection? connection;

		public SteamDisconnectionReason? disconnectionReason;

		public string? debugReason;
	}

	private CSteamID? _lobbyId;

	private CSteamID _hostNetId;

	private HSteamNetConnection? _conn;

	private bool _isConnected;

	private TaskCompletionSource<ConnectionResult>? _connectingTaskCompletionSource;

	private Callback<SteamNetConnectionStatusChangedCallback_t>? _netStatusChangedCallback;

	private readonly Logger _logger = new Logger("SteamClient", LogType.Network);

	public override bool IsConnected => _isConnected;

	public override ulong NetId => SteamUser.GetSteamID().m_SteamID;

	public override ulong HostNetId => _hostNetId.m_SteamID;

	public CSteamID? LobbyId => _lobbyId;

	public SteamClient(INetClientHandler handler)
		: base(handler)
	{
	}

	public Task<NetErrorInfo?> ConnectToLobbyOwnedByFriend(ulong steamPlayerId, CancellationToken cancelToken = default(CancellationToken))
	{
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		_logger.Info($"Initializing Steam client. Our player id: {NetId}");
		_logger.Debug($"Attempting to connect to lobby of player {steamPlayerId}");
		FriendGameInfo_t val = default(FriendGameInfo_t);
		if (!SteamFriends.GetFriendGamePlayed(new CSteamID(steamPlayerId), ref val))
		{
			throw new InvalidOperationException($"Tried to join game of {steamPlayerId}, but they are not playing a game!");
		}
		if (val.m_gameID != new CGameID(2868840uL) || val.m_steamIDLobby.m_SteamID == 0L)
		{
			return Task.FromResult((NetErrorInfo?)new NetErrorInfo(NetError.InvalidJoin, selfInitiated: false));
		}
		return ConnectToLobby(val.m_steamIDLobby.m_SteamID, cancelToken);
	}

	public async Task<NetErrorInfo?> ConnectToLobby(ulong lobbyId, CancellationToken cancelToken = default(CancellationToken))
	{
		NetErrorInfo? result;
		await using (cancelToken.Register(CancelConnection))
		{
			SteamAPICall_t call = SteamMatchmaking.JoinLobby(new CSteamID(lobbyId));
			using SteamCallResult<LobbyEnter_t> callResult = new SteamCallResult<LobbyEnter_t>(call, cancelToken);
			_logger.Debug($"Attempting to enter lobby {lobbyId}");
			LobbyEnter_t val = await callResult.Task;
			if (val.m_ulSteamIDLobby != lobbyId)
			{
				_logger.Error("Joined incorrect lobby?");
				result = new NetErrorInfo(NetError.InternalError, selfInitiated: false);
			}
			else
			{
				EChatRoomEnterResponse val2 = (EChatRoomEnterResponse)val.m_EChatRoomEnterResponse;
				if ((int)val2 != 1)
				{
					_logger.Error($"Failed to enter lobby, response: {val2}");
					result = new NetErrorInfo(val2);
				}
				else
				{
					_lobbyId = new CSteamID(val.m_ulSteamIDLobby);
					CSteamID lobbyOwnerId = SteamMatchmaking.GetLobbyOwner(_lobbyId.Value);
					SteamNetworkingIdentity val3 = lobbyOwnerId.ToNetId();
					_netStatusChangedCallback = new Callback<SteamNetConnectionStatusChangedCallback_t>((DispatchDelegate<SteamNetConnectionStatusChangedCallback_t>)OnNetStatusChanged, false);
					_connectingTaskCompletionSource = new TaskCompletionSource<ConnectionResult>();
					_conn = SteamNetworkingSockets.ConnectP2P(ref val3, 0, 0, (SteamNetworkingConfigValue_t[])null);
					_logger.Debug($"Connecting to user {lobbyOwnerId.m_SteamID}");
					ConnectionResult connectionResult = await _connectingTaskCompletionSource.Task;
					if (connectionResult.disconnectionReason.HasValue)
					{
						SteamNetworkingSockets.CloseConnection(_conn.Value, (int)connectionResult.disconnectionReason.Value, connectionResult.debugReason, false);
						_conn = null;
						_connectingTaskCompletionSource = null;
						SteamMatchmaking.LeaveLobby(_lobbyId.Value);
						_lobbyId = null;
						_netStatusChangedCallback.Dispose();
						_netStatusChangedCallback = null;
						result = new NetErrorInfo(connectionResult.disconnectionReason.Value, connectionResult.debugReason, selfInitiated: false);
					}
					else if (_conn.Value.m_HSteamNetConnection != connectionResult.connection.Value.m_HSteamNetConnection)
					{
						_logger.Error("Got different connection back from OnNetStatusChanged than we expected!");
						DisconnectFromHostInternal(SteamDisconnectionReason.AppInternalError, "Invalid OnNetStatusChanged hConn", now: true, selfInitiated: false);
						result = new NetErrorInfo(NetError.InternalError, selfInitiated: false);
					}
					else
					{
						_connectingTaskCompletionSource = null;
						_isConnected = true;
						_hostNetId = lobbyOwnerId;
						_handler.OnConnectedToHost();
						_logger.Debug($"Successfully connected to host {lobbyOwnerId.m_SteamID}");
						result = null;
					}
				}
			}
		}
		return result;
	}

	private void OnNetStatusChanged(SteamNetConnectionStatusChangedCallback_t data)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Invalid comparison between Unknown and I4
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Invalid comparison between Unknown and I4
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Invalid comparison between Unknown and I4
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		_logger.Debug($"Connection status changed: {data.m_eOldState} -> {data.m_info.m_eState}");
		if ((int)data.m_info.m_eState == 3)
		{
			_logger.Debug("Steam connection accepted.");
			if (_connectingTaskCompletionSource == null)
			{
				_logger.Error("Connection was accepted while we were not waiting for it!");
				DisconnectFromHostInternal(SteamDisconnectionReason.InternalError, "Not Connecting", now: true, selfInitiated: false);
			}
			else
			{
				_connectingTaskCompletionSource.SetResult(new ConnectionResult
				{
					connection = data.m_hConn
				});
			}
		}
		else if ((int)data.m_info.m_eState == 5)
		{
			_logger.Info($"Steam connection closed because of problem. Reason: {data.m_info.m_eEndReason}, {((SteamNetConnectionInfo_t)(ref data.m_info)).m_szEndDebug}");
			HandleDisconnection((SteamDisconnectionReason)data.m_info.m_eEndReason, ((SteamNetConnectionInfo_t)(ref data.m_info)).m_szEndDebug);
		}
		else if ((int)data.m_info.m_eState == 4)
		{
			_logger.Info($"Steam connection closed by host. Reason: {data.m_info.m_eEndReason}, {((SteamNetConnectionInfo_t)(ref data.m_info)).m_szEndDebug}");
			HandleDisconnection((SteamDisconnectionReason)data.m_info.m_eEndReason, ((SteamNetConnectionInfo_t)(ref data.m_info)).m_szEndDebug);
		}
	}

	private void HandleDisconnection(SteamDisconnectionReason reason, string debugReason)
	{
		if (_connectingTaskCompletionSource != null)
		{
			_connectingTaskCompletionSource.SetResult(new ConnectionResult
			{
				disconnectionReason = reason,
				debugReason = debugReason
			});
		}
		else
		{
			DisconnectFromHostInternal(reason, debugReason, now: true, selfInitiated: false);
		}
	}

	public override void Update()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		SteamUtil.ProcessMessages(_conn.Value, _handler, _logger);
	}

	public override void SendMessageToHost(byte[] bytes, int length, NetTransferMode mode, int channel = 0)
	{
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Invalid comparison between Unknown and I4
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		_logger.VeryDebug($"Sending {length} bytes to host");
		GCHandle gCHandle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
		try
		{
			long num = default(long);
			EResult val = SteamNetworkingSockets.SendMessageToConnection(_conn.Value, (IntPtr)gCHandle.AddrOfPinnedObject(), (uint)length, SteamUtil.FlagsFromMode(mode), ref num);
			if ((int)val != 1)
			{
				_logger.Warn($"Failed to send message length {length}: {val}");
			}
		}
		finally
		{
			gCHandle.Free();
		}
	}

	public override void DisconnectFromHost(NetError reason, bool now = false)
	{
		DisconnectFromHostInternal(reason.ToSteam(), string.Empty, now, selfInitiated: true);
	}

	private void DisconnectFromHostInternal(SteamDisconnectionReason reason, string? debugReason, bool now, bool selfInitiated)
	{
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		_logger.Debug($"Disconnecting from host (now: {now} reason: {reason} debug: {debugReason})");
		SteamNetworkingSockets.CloseConnection(_conn.Value, (int)reason, debugReason, !now);
		SteamMatchmaking.LeaveLobby(_lobbyId.Value);
		ulong steamID = _hostNetId.m_SteamID;
		_conn = null;
		_connectingTaskCompletionSource = null;
		_netStatusChangedCallback?.Dispose();
		_netStatusChangedCallback = null;
		_isConnected = false;
		_hostNetId = CSteamID.Nil;
		_handler.OnDisconnectedFromHost(steamID, new NetErrorInfo(reason, debugReason, selfInitiated));
	}

	private void CancelConnection()
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		_connectingTaskCompletionSource?.SetCanceled();
		if (_conn.HasValue)
		{
			DisconnectFromHost(NetError.Quit);
		}
		else if (_lobbyId.HasValue)
		{
			SteamMatchmaking.LeaveLobby(_lobbyId.Value);
			_lobbyId = null;
			_netStatusChangedCallback?.Dispose();
			_netStatusChangedCallback = null;
		}
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
