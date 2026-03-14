using System;
using System.Collections.Generic;
using System.Text;
using Godot;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Platform.Steam;
using Steamworks;

namespace MegaCrit.Sts2.Core.Entities.Multiplayer;

public readonly struct NetErrorInfo
{
	private readonly NetError? _reason;

	private readonly ConnectionFailureReason? _connectionReason;

	private readonly ConnectionFailureExtraInfo? _connectionExtraInfo;

	private readonly SteamDisconnectionReason? _steamReason;

	private readonly EResult? _lobbyCreationResult;

	private readonly EChatRoomEnterResponse? _lobbyEnterResponse;

	private readonly string? _debugReason;

	private readonly Error? _godotError;

	public bool SelfInitiated { get; }

	public NetErrorInfo(NetError reason, bool selfInitiated)
	{
		_connectionReason = null;
		_connectionExtraInfo = null;
		_steamReason = null;
		_lobbyCreationResult = null;
		_lobbyEnterResponse = null;
		_debugReason = null;
		_godotError = null;
		_reason = reason;
		SelfInitiated = selfInitiated;
	}

	public NetErrorInfo(ConnectionFailureReason reason, ConnectionFailureExtraInfo? extraInfo = null)
	{
		_reason = null;
		_steamReason = null;
		_lobbyCreationResult = null;
		_lobbyEnterResponse = null;
		_debugReason = null;
		_godotError = null;
		_connectionReason = reason;
		_connectionExtraInfo = extraInfo;
		SelfInitiated = false;
	}

	public NetErrorInfo(SteamDisconnectionReason steamReason, string? debugReason, bool selfInitiated)
	{
		_reason = null;
		_connectionReason = null;
		_connectionExtraInfo = null;
		_lobbyCreationResult = null;
		_lobbyEnterResponse = null;
		_godotError = null;
		_steamReason = steamReason;
		_debugReason = debugReason;
		SelfInitiated = selfInitiated;
	}

	public NetErrorInfo(EChatRoomEnterResponse lobbyEnterResponse)
	{
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		_reason = null;
		_connectionReason = null;
		_connectionExtraInfo = null;
		_steamReason = null;
		_lobbyCreationResult = null;
		_debugReason = null;
		_godotError = null;
		_lobbyEnterResponse = lobbyEnterResponse;
		SelfInitiated = true;
	}

	public NetErrorInfo(EResult lobbyCreationResult)
	{
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		_reason = null;
		_connectionReason = null;
		_connectionExtraInfo = null;
		_steamReason = null;
		_lobbyEnterResponse = null;
		_debugReason = null;
		_godotError = null;
		_lobbyCreationResult = lobbyCreationResult;
		SelfInitiated = true;
	}

	public NetErrorInfo(Error error)
	{
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		_reason = null;
		_connectionReason = null;
		_connectionExtraInfo = null;
		_steamReason = null;
		_lobbyCreationResult = null;
		_lobbyEnterResponse = null;
		_debugReason = null;
		_godotError = error;
		SelfInitiated = true;
	}

	public NetError GetReason()
	{
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Expected I4, but got Unknown
		if (_reason.HasValue)
		{
			return _reason.Value;
		}
		if (_connectionReason.HasValue)
		{
			ConnectionFailureReason value = _connectionReason.Value;
			switch (value)
			{
			case ConnectionFailureReason.None:
				return NetError.None;
			case ConnectionFailureReason.LobbyFull:
				return NetError.LobbyFull;
			case ConnectionFailureReason.RunInProgress:
				return NetError.RunInProgress;
			case ConnectionFailureReason.NotInSaveGame:
				return NetError.NotInSaveGame;
			case ConnectionFailureReason.VersionMismatch:
				return NetError.VersionMismatch;
			case ConnectionFailureReason.ModMismatch:
				return NetError.ModMismatch;
			default:
			{
				global::_003CPrivateImplementationDetails_003E.ThrowSwitchExpressionException(value);
				NetError result = default(NetError);
				return result;
			}
			}
		}
		if (_steamReason.HasValue)
		{
			return _steamReason.Value.ToApp();
		}
		if (_lobbyCreationResult.HasValue)
		{
			return NetError.FailedToHost;
		}
		if (_lobbyEnterResponse.HasValue)
		{
			EChatRoomEnterResponse value2 = _lobbyEnterResponse.Value;
			return (value2 - 2) switch
			{
				0 => NetError.InvalidJoin, 
				1 => NetError.InternalError, 
				2 => NetError.LobbyFull, 
				3 => NetError.UnknownNetworkError, 
				4 => NetError.JoinBlockedByUser, 
				5 => NetError.UnknownNetworkError, 
				6 => NetError.JoinBlockedByUser, 
				7 => NetError.JoinBlockedByUser, 
				8 => NetError.JoinBlockedByUser, 
				9 => NetError.JoinBlockedByUser, 
				13 => NetError.TryAgainLater, 
				_ => throw new ArgumentOutOfRangeException(), 
			};
		}
		if (_godotError.HasValue)
		{
			return NetError.FailedToHost;
		}
		throw new InvalidOperationException("Tried to get DisconnectionReason from DisconnectionInfo without any assigned errors");
	}

	public string GetErrorString()
	{
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		if (_reason.HasValue)
		{
			return _reason.Value.ToString();
		}
		if (_connectionReason.HasValue)
		{
			if (_connectionReason == ConnectionFailureReason.ModMismatch)
			{
				StringBuilder stringBuilder = new StringBuilder();
				List<string> list = _connectionExtraInfo?.missingModsOnHost;
				if (list != null && list.Count > 0)
				{
					LocString locString = new LocString("main_menu_ui", "NETWORK_ERROR.MOD_MISMATCH.description.missingOnHost");
					locString.Add("mods", string.Join(", ", _connectionExtraInfo.missingModsOnHost));
					stringBuilder.AppendLine(locString.GetFormattedText());
				}
				list = _connectionExtraInfo?.missingModsOnLocal;
				if (list != null && list.Count > 0)
				{
					LocString locString2 = new LocString("main_menu_ui", "NETWORK_ERROR.MOD_MISMATCH.description.missingOnLocal");
					locString2.Add("mods", string.Join(", ", _connectionExtraInfo.missingModsOnLocal));
					stringBuilder.AppendLine(locString2.GetFormattedText());
				}
				return stringBuilder.ToString();
			}
			return _connectionReason.Value.ToString();
		}
		if (_steamReason.HasValue)
		{
			return $"{_steamReason} - {_debugReason}";
		}
		if (_lobbyCreationResult.HasValue)
		{
			return $"Lobby creation failed: {_lobbyCreationResult.Value}";
		}
		if (_lobbyEnterResponse.HasValue)
		{
			return $"Lobby join failed: {_lobbyEnterResponse.Value}";
		}
		if (_godotError.HasValue)
		{
			return ((object)_godotError.Value/*cast due to .constrained prefix*/).ToString();
		}
		return "<null>";
	}

	public override string ToString()
	{
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		if (_reason.HasValue)
		{
			return $"DisconnectionReason {_reason.Value} {SelfInitiated}";
		}
		if (_connectionReason.HasValue)
		{
			return $"ConnectionFailureReason {_connectionReason.Value} {SelfInitiated}";
		}
		if (_steamReason.HasValue)
		{
			return $"SteamDisconnectionReason {_steamReason.Value} {_debugReason} {SelfInitiated}";
		}
		if (_lobbyCreationResult.HasValue)
		{
			return $"EResult {_lobbyCreationResult.Value} {SelfInitiated}";
		}
		if (_lobbyEnterResponse.HasValue)
		{
			return $"EChatRoomEnterResponse {_lobbyEnterResponse.Value} {SelfInitiated}";
		}
		if (_godotError.HasValue)
		{
			return $"Godot.Error {_godotError.Value} {SelfInitiated}";
		}
		return "<null>";
	}
}
