using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Multiplayer.Game;
using MegaCrit.Sts2.Core.Multiplayer.Transport.Steam;
using Steamworks;

namespace MegaCrit.Sts2.Core.Platform.Steam;

public class SteamPlatformUtilStrategy : IPlatformUtilStrategy
{
	private const string _richPresenceDisplayKey = "steam_display";

	private const string _richPresenceGroupKey = "steam_player_group";

	private const string _richPresenceGroupSizeKey = "steam_player_group_size";

	private readonly Lazy<string?> _steamBranch = new Lazy<string>(() =>
	{
		string text = default(string);
		return (!SteamApps.GetCurrentBetaName(ref text, 128)) ? "public" : text;
	});

	public bool SupportsInviteDialog => SteamUtils.IsOverlayEnabled();

	public string GetPlayerName(ulong playerId)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		string text = ((playerId != SteamUser.GetSteamID().m_SteamID) ? SteamFriends.GetFriendPersonaName(new CSteamID(playerId)) : SteamFriends.GetPersonaName());
		if (!string.IsNullOrEmpty(text))
		{
			return text;
		}
		return playerId.ToString();
	}

	public ulong GetLocalPlayerId()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		return SteamUser.GetSteamID().m_SteamID;
	}

	public Task<IEnumerable<ulong>> GetFriendsWithOpenLobbies()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		List<ulong> list = new List<ulong>();
		int friendCount = SteamFriends.GetFriendCount((EFriendFlags)4);
		FriendGameInfo_t val = default(FriendGameInfo_t);
		for (int i = 0; i < friendCount; i++)
		{
			CSteamID friendByIndex = SteamFriends.GetFriendByIndex(i, (EFriendFlags)4);
			if (SteamFriends.GetFriendGamePlayed(friendByIndex, ref val) && val.m_gameID.m_GameID == 2868840 && ((CSteamID)(ref val.m_steamIDLobby)).IsValid())
			{
				list.Add(friendByIndex.m_SteamID);
			}
		}
		return Task.FromResult((IEnumerable<ulong>)list);
	}

	public void OpenInviteDialog(INetGameService netService)
	{
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		if (!SteamUtils.IsOverlayEnabled())
		{
			Log.Error("Tried to open invite dialog, but the player has disabled the steam overlay");
			return;
		}
		CSteamID value;
		if (netService is INetHostGameService { NetHost: SteamHost { LobbyId: var lobbyId } netHost })
		{
			if (!lobbyId.HasValue)
			{
				Log.Warn("Tried to open invite dialog but steam host is not yet in a lobby");
				return;
			}
			value = netHost.LobbyId.Value;
		}
		else
		{
			if (!(netService is INetClientGameService { NetClient: SteamClient { LobbyId: var lobbyId2 } netClient }))
			{
				throw new InvalidOperationException($"Tried to open invite dialog for non-steam net service {netService}");
			}
			if (!lobbyId2.HasValue)
			{
				Log.Warn("Tried to open invite dialog but steam host is not yet in a lobby");
				return;
			}
			value = netClient.LobbyId.Value;
		}
		SteamFriends.ActivateGameOverlayInviteDialog(value);
	}

	public void OpenUrl(string url)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		if (SteamUtils.IsOverlayEnabled())
		{
			SteamFriends.ActivateGameOverlayToWebPage(url, (EActivateGameOverlayToWebPageMode)0);
		}
		else
		{
			OS.ShellOpen(url);
		}
	}

	public void OpenVirtualKeyboard()
	{
		SteamUtils.ShowFloatingGamepadTextInput((EFloatingGamepadTextInputMode)0, 100, 200, 300, 50);
	}

	public void CloseVirtualKeyboard()
	{
		SteamUtils.DismissFloatingGamepadTextInput();
	}

	public void SetRichPresence(string token, string? playerGroup, int? groupSize)
	{
		SteamFriends.SetRichPresence("steam_display", "#" + token);
		SteamFriends.SetRichPresence("steam_player_group", playerGroup);
		SteamFriends.SetRichPresence("steam_player_group_size", groupSize?.ToString());
	}

	public void SetRichPresenceValue(string key, string? value)
	{
		SteamFriends.SetRichPresence(key, value);
	}

	public void ClearRichPresence()
	{
		SteamFriends.ClearRichPresence();
	}

	public string? GetThreeLetterLanguageCode()
	{
		return GetRawLanguage() switch
		{
			"arabic" => "ara", 
			"schinese" => "zhs", 
			"tchinese" => "zht", 
			"czech" => "cze", 
			"dutch" => "dut", 
			"english" => "eng", 
			"french" => "fra", 
			"german" => "deu", 
			"greek" => "gre", 
			"indonesian" => "ind", 
			"italian" => "ita", 
			"japanese" => "jpn", 
			"korean" => "kor", 
			"norwegian" => "nor", 
			"polish" => "pol", 
			"portuguese" => "por", 
			"brazilian" => "ptb", 
			"russian" => "rus", 
			"spanish" => "spa", 
			"latam" => "esp", 
			"swedish" => "swe", 
			"thai" => "tha", 
			"turkish" => "tur", 
			"ukrainian" => "ukr", 
			"vietnamese" => "vie", 
			_ => null, 
		};
	}

	public string? GetPlatformBranch()
	{
		return _steamBranch.Value;
	}

	public string GetRawLanguage()
	{
		return SteamUtils.GetSteamUILanguage();
	}

	public SupportedWindowMode GetSupportedWindowMode()
	{
		if (SteamUtils.IsSteamInBigPictureMode())
		{
			return SupportedWindowMode.FullscreenOnlyDisplayToggle;
		}
		return SupportedWindowMode.Any;
	}
}
