using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Daily;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Modifiers;
using MegaCrit.Sts2.Core.Multiplayer;
using MegaCrit.Sts2.Core.Multiplayer.Game;
using MegaCrit.Sts2.Core.Multiplayer.Game.Lobby;
using MegaCrit.Sts2.Core.Multiplayer.Messages.Lobby;
using MegaCrit.Sts2.Core.Nodes.Audio;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Multiplayer;
using MegaCrit.Sts2.Core.Nodes.Screens.MainMenu;
using MegaCrit.Sts2.Core.Platform;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.TestSupport;
using MegaCrit.Sts2.Core.Unlocks;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.Nodes.Screens.DailyRun;

[ScriptPath("res://src/Core/Nodes/Screens/DailyRun/NDailyRunScreen.cs")]
public class NDailyRunScreen : NSubmenu, IStartRunLobbyListener
{
	public new class MethodName : NSubmenu.MethodName
	{
		public static readonly StringName Create = StringName.op_Implicit("Create");

		public new static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName InitializeSingleplayer = StringName.op_Implicit("InitializeSingleplayer");

		public new static readonly StringName OnSubmenuOpened = StringName.op_Implicit("OnSubmenuOpened");

		public new static readonly StringName OnSubmenuClosed = StringName.op_Implicit("OnSubmenuClosed");

		public static readonly StringName InitializeLeaderboard = StringName.op_Implicit("InitializeLeaderboard");

		public static readonly StringName InitializeDisplay = StringName.op_Implicit("InitializeDisplay");

		public static readonly StringName SetIsLoading = StringName.op_Implicit("SetIsLoading");

		public static readonly StringName _Process = StringName.op_Implicit("_Process");

		public static readonly StringName MaxAscensionChanged = StringName.op_Implicit("MaxAscensionChanged");

		public static readonly StringName AscensionChanged = StringName.op_Implicit("AscensionChanged");

		public static readonly StringName SeedChanged = StringName.op_Implicit("SeedChanged");

		public static readonly StringName ModifiersChanged = StringName.op_Implicit("ModifiersChanged");

		public static readonly StringName OnEmbarkPressed = StringName.op_Implicit("OnEmbarkPressed");

		public static readonly StringName OnUnreadyPressed = StringName.op_Implicit("OnUnreadyPressed");

		public static readonly StringName UpdateRichPresence = StringName.op_Implicit("UpdateRichPresence");

		public static readonly StringName CleanUpLobby = StringName.op_Implicit("CleanUpLobby");

		public static readonly StringName AfterLobbyInitialized = StringName.op_Implicit("AfterLobbyInitialized");
	}

	public new class PropertyName : NSubmenu.PropertyName
	{
		public new static readonly StringName InitialFocusedControl = StringName.op_Implicit("InitialFocusedControl");

		public static readonly StringName _titleLabel = StringName.op_Implicit("_titleLabel");

		public static readonly StringName _dateLabel = StringName.op_Implicit("_dateLabel");

		public static readonly StringName _timeLeftLabel = StringName.op_Implicit("_timeLeftLabel");

		public static readonly StringName _characterContainer = StringName.op_Implicit("_characterContainer");

		public static readonly StringName _embarkButton = StringName.op_Implicit("_embarkButton");

		public new static readonly StringName _backButton = StringName.op_Implicit("_backButton");

		public static readonly StringName _unreadyButton = StringName.op_Implicit("_unreadyButton");

		public static readonly StringName _leaderboard = StringName.op_Implicit("_leaderboard");

		public static readonly StringName _modifiersTitleLabel = StringName.op_Implicit("_modifiersTitleLabel");

		public static readonly StringName _modifiersContainer = StringName.op_Implicit("_modifiersContainer");

		public static readonly StringName _remotePlayerContainer = StringName.op_Implicit("_remotePlayerContainer");

		public static readonly StringName _readyAndWaitingContainer = StringName.op_Implicit("_readyAndWaitingContainer");
	}

	public new class SignalName : NSubmenu.SignalName
	{
	}

	private static readonly string _scenePath = SceneHelper.GetScenePath("screens/daily_run/daily_run_screen");

	private static readonly LocString _timeLeftLoc = new LocString("main_menu_ui", "DAILY_RUN_MENU.TIME_LEFT");

	public static readonly string dateFormat = LocManager.Instance.GetTable("main_menu_ui").GetRawText("DAILY_RUN_MENU.DATE_FORMAT");

	private static readonly string _timeLeftFormat = LocManager.Instance.GetTable("main_menu_ui").GetRawText("DAILY_RUN_MENU.TIME_FORMAT");

	private MegaLabel _titleLabel;

	private MegaRichTextLabel _dateLabel;

	private MegaRichTextLabel _timeLeftLabel;

	private NDailyRunCharacterContainer _characterContainer;

	private NConfirmButton _embarkButton;

	private NBackButton _backButton;

	private NBackButton _unreadyButton;

	private NDailyRunLeaderboard _leaderboard;

	private MegaLabel _modifiersTitleLabel;

	private Control _modifiersContainer;

	private readonly List<NDailyRunScreenModifier> _modifierContainers = new List<NDailyRunScreenModifier>();

	private NRemoteLobbyPlayerContainer _remotePlayerContainer;

	private Control _readyAndWaitingContainer;

	private DateTimeOffset _endOfDay;

	private INetGameService _netService;

	private StartRunLobby? _lobby;

	private int? _lastSetTimeLeftSecond;

	public static string[] AssetPaths => new string[1] { _scenePath };

	protected override Control? InitialFocusedControl => null;

	public static NDailyRunScreen? Create()
	{
		if (TestMode.IsOn)
		{
			return null;
		}
		return PreloadManager.Cache.GetScene(_scenePath).Instantiate<NDailyRunScreen>((GenEditState)0);
	}

	public override void _Ready()
	{
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		ConnectSignals();
		_titleLabel = ((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%Title"));
		_dateLabel = ((Node)this).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("%Date"));
		_embarkButton = ((Node)this).GetNode<NConfirmButton>(NodePath.op_Implicit("%ConfirmButton"));
		_backButton = ((Node)this).GetNode<NBackButton>(NodePath.op_Implicit("%BackButton"));
		_unreadyButton = ((Node)this).GetNode<NBackButton>(NodePath.op_Implicit("%UnreadyButton"));
		_timeLeftLabel = ((Node)this).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("%TimeLeft"));
		_leaderboard = ((Node)this).GetNode<NDailyRunLeaderboard>(NodePath.op_Implicit("%Leaderboards"));
		_modifiersTitleLabel = ((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%ModifiersLabel"));
		_modifiersContainer = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%ModifiersContainer"));
		_characterContainer = ((Node)this).GetNode<NDailyRunCharacterContainer>(NodePath.op_Implicit("ChallengeContainer/CenterContainer/HBoxContainer/CharacterContainer"));
		_remotePlayerContainer = ((Node)this).GetNode<NRemoteLobbyPlayerContainer>(NodePath.op_Implicit("%RemotePlayerContainer"));
		_readyAndWaitingContainer = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%ReadyAndWaitingPanel"));
		_titleLabel.SetTextAutoSize(new LocString("main_menu_ui", "DAILY_RUN_MENU.DAILY_TITLE").GetFormattedText());
		_modifiersTitleLabel.SetTextAutoSize(new LocString("main_menu_ui", "DAILY_RUN_MENU.MODIFIERS").GetFormattedText());
		_dateLabel.SetTextAutoSize(new LocString("main_menu_ui", "DAILY_RUN_MENU.FETCHING_TIME").GetFormattedText());
		foreach (NDailyRunScreenModifier item in ((IEnumerable)((Node)_modifiersContainer).GetChildren(false)).OfType<NDailyRunScreenModifier>())
		{
			_modifierContainers.Add(item);
		}
		((GodotObject)_embarkButton).Connect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)OnEmbarkPressed), 0u);
		_embarkButton.Disable();
		((GodotObject)_unreadyButton).Connect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)OnUnreadyPressed), 0u);
		_unreadyButton.Disable();
		((CanvasItem)_remotePlayerContainer).Visible = false;
		((CanvasItem)_readyAndWaitingContainer).Visible = false;
		_leaderboard.Cleanup();
	}

	public void InitializeMultiplayerAsHost(INetGameService gameService)
	{
		if (gameService.Type != NetGameType.Host)
		{
			throw new InvalidOperationException($"Initialized character select screen with GameService of type {gameService.Type} when hosting!");
		}
		_netService = gameService;
	}

	public void InitializeMultiplayerAsClient(INetGameService gameService, ClientLobbyJoinResponseMessage message)
	{
		if (gameService.Type != NetGameType.Client)
		{
			throw new InvalidOperationException($"Initialized character select screen with GameService of type {gameService.Type} when joining!");
		}
		_netService = gameService;
		_lobby = new StartRunLobby(GameMode.Daily, gameService, this, message.dailyTime.Value, -1);
		_lobby.InitializeFromMessage(message);
		SetupLobbyParams(_lobby);
		AfterLobbyInitialized();
	}

	public void InitializeSingleplayer()
	{
		_netService = new NetSingleplayerGameService();
	}

	public override void OnSubmenuOpened()
	{
		base.OnSubmenuOpened();
		NetGameType type = _netService.Type;
		if ((uint)(type - 1) <= 1u)
		{
			TaskHelper.RunSafely(SetupLobbyForHostOrSingleplayer());
		}
		else
		{
			SetIsLoading(isLoading: false);
		}
	}

	public override void OnSubmenuClosed()
	{
		_embarkButton.Disable();
		_remotePlayerContainer.Cleanup();
		_leaderboard.Cleanup();
		StartRunLobby? lobby = _lobby;
		if (lobby != null && lobby.NetService.Type.IsMultiplayer())
		{
			PlatformUtil.SetRichPresence("MAIN_MENU", null, null);
		}
		CleanUpLobby(disconnectSession: true);
	}

	private void InitializeLeaderboard()
	{
		_leaderboard.Initialize(_lobby.DailyTime.Value.serverTime, _lobby.Players.Select((LobbyPlayer p) => p.id), allowPagination: false);
	}

	private async Task SetupLobbyForHostOrSingleplayer()
	{
		if (_netService.Type != NetGameType.Host && _netService.Type != NetGameType.Singleplayer)
		{
			throw new InvalidOperationException("Should only be called as host or singleplayer!");
		}
		SetIsLoading(isLoading: true);
		TimeServerResult timeServerResult = await GetTimeServerTime();
		_lobby = new StartRunLobby(GameMode.Daily, _netService, this, timeServerResult, 4);
		_lobby.AddLocalHostPlayer(new UnlockState(SaveManager.Instance.Progress), SaveManager.Instance.Progress.MaxMultiplayerAscension);
		SetupLobbyParams(_lobby);
		AfterLobbyInitialized();
		SetIsLoading(isLoading: false);
		Log.Info($"Daily initialized with seed: {_lobby.Seed} time: {GetServerRelativeTime()}");
	}

	private async Task<TimeServerResult> GetTimeServerTime()
	{
		TimeServerResult? result = null;
		if (TimeServer.RequestTimeTask?.IsCompleted ?? false)
		{
			if (!TimeServer.RequestTimeTask.IsFaulted)
			{
				result = await TimeServer.RequestTimeTask;
			}
			if (!result.HasValue)
			{
				try
				{
					result = await TimeServer.FetchDailyTime();
				}
				catch (HttpRequestException ex)
				{
					Log.Error(ex.ToString());
				}
			}
		}
		else
		{
			try
			{
				result = await TimeServer.FetchDailyTime();
			}
			catch (HttpRequestException ex2)
			{
				Log.Error(ex2.ToString());
			}
		}
		if (!result.HasValue)
		{
			Log.Info("Couldn't retrieve time from time server, using local time");
			result = new TimeServerResult
			{
				serverTime = DateTimeOffset.UtcNow,
				localReceivedTime = DateTimeOffset.UtcNow
			};
		}
		return result.Value;
	}

	private DateTimeOffset GetServerRelativeTime()
	{
		return _lobby.DailyTime.Value.serverTime + (DateTimeOffset.UtcNow - _lobby.DailyTime.Value.localReceivedTime);
	}

	private void SetupLobbyParams(StartRunLobby lobby)
	{
		DateTimeOffset serverRelativeTime = GetServerRelativeTime();
		string str = SeedHelper.CanonicalizeSeed(serverRelativeTime.ToString("dd_MM_yyyy"));
		string text = SeedHelper.CanonicalizeSeed(serverRelativeTime.ToString($"dd_MM_yyyy_{lobby.Players.Count}p"));
		Rng rng = new Rng((uint)StringHelper.GetDeterministicHashCode(str));
		Rng rng2 = new Rng(rng.NextUnsignedInt());
		Rng rng3 = new Rng(rng.NextUnsignedInt());
		Rng rng4 = new Rng(rng.NextUnsignedInt());
		CharacterModel characterModel = null;
		foreach (LobbyPlayer player in lobby.Players)
		{
			CharacterModel characterModel2 = rng2.NextItem(ModelDb.AllCharacters);
			if (player.id == lobby.LocalPlayer.id)
			{
				characterModel = characterModel2;
			}
		}
		int num = rng3.NextInt(0, 11);
		List<ModifierModel> list = RollModifiers(rng4);
		NetGameType type = lobby.NetService.Type;
		if ((uint)(type - 1) <= 1u)
		{
			if (lobby.Seed != text)
			{
				lobby.SetSeed(text);
			}
			if (lobby.Ascension != num)
			{
				lobby.SyncAscensionChange(num);
			}
			if (list.Any((ModifierModel m) => lobby.Modifiers.FirstOrDefault(m.IsEquivalent) == null))
			{
				lobby.SetModifiers(list);
			}
		}
		if (lobby.LocalPlayer.character != characterModel)
		{
			lobby.SetLocalCharacter(characterModel);
		}
		InitializeDisplay();
	}

	private void InitializeDisplay()
	{
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		if (_lobby == null)
		{
			throw new InvalidOperationException("Tried to initialize daily run display before lobby was initialized!");
		}
		DateTimeOffset serverRelativeTime = GetServerRelativeTime();
		_endOfDay = new DateTimeOffset(serverRelativeTime.Year, serverRelativeTime.Month, serverRelativeTime.Day, 0, 0, 0, TimeSpan.Zero) + TimeSpan.FromDays(1);
		((CanvasItem)_remotePlayerContainer).Visible = _lobby.NetService.Type.IsMultiplayer();
		CharacterModel character = _lobby.LocalPlayer.character;
		_characterContainer.Fill(character, _lobby.LocalPlayer.id, _lobby.Ascension, _lobby.NetService);
		((CanvasItem)_dateLabel).Modulate = StsColors.blue;
		_dateLabel.Text = serverRelativeTime.ToString(dateFormat);
		for (int i = 0; i < _lobby.Modifiers.Count; i++)
		{
			_modifierContainers[i].Fill(_lobby.Modifiers[i]);
		}
	}

	private List<ModifierModel> RollModifiers(Rng rng)
	{
		List<ModifierModel> list = new List<ModifierModel>();
		List<ModifierModel> list2 = ModelDb.GoodModifiers.ToList().StableShuffle(rng);
		for (int i = 0; i < 2; i++)
		{
			ModifierModel canonicalModifier = rng.NextItem(list2);
			if (canonicalModifier == null)
			{
				throw new InvalidOperationException("There were not enough good modifiers to fill the daily!");
			}
			ModifierModel modifierModel = canonicalModifier.ToMutable();
			if (modifierModel is CharacterCards characterCards)
			{
				IEnumerable<CharacterModel> second = _lobby.Players.Select((LobbyPlayer p) => p.character);
				characterCards.CharacterModel = rng.NextItem(ModelDb.AllCharacters.Except(second)).Id;
			}
			list.Add(modifierModel);
			list2.Remove(canonicalModifier);
			IReadOnlySet<ModifierModel> readOnlySet = ModelDb.MutuallyExclusiveModifiers.FirstOrDefault((IReadOnlySet<ModifierModel> s) => s.Contains(canonicalModifier));
			if (readOnlySet == null)
			{
				continue;
			}
			foreach (ModifierModel item in readOnlySet)
			{
				list2.Remove(item);
			}
		}
		list.Add(rng.NextItem(ModelDb.BadModifiers).ToMutable());
		return list;
	}

	private void SetIsLoading(bool isLoading)
	{
		if (isLoading)
		{
			((CanvasItem)_remotePlayerContainer).Visible = false;
			((CanvasItem)_readyAndWaitingContainer).Visible = false;
		}
		((CanvasItem)_timeLeftLabel).Visible = !isLoading;
		((CanvasItem)_characterContainer).Visible = !isLoading;
		((CanvasItem)_modifiersTitleLabel).Visible = !isLoading;
		((CanvasItem)_modifiersContainer).Visible = !isLoading;
		if (isLoading)
		{
			_embarkButton.Disable();
		}
		else
		{
			_embarkButton.Enable();
		}
	}

	public override void _Process(double delta)
	{
		if (_lobby != null)
		{
			DateTimeOffset serverRelativeTime = GetServerRelativeTime();
			if (serverRelativeTime > _endOfDay)
			{
				SetupLobbyParams(_lobby);
			}
			TimeSpan timeSpan = _endOfDay - serverRelativeTime;
			if (_lastSetTimeLeftSecond != timeSpan.Seconds)
			{
				string variable = timeSpan.ToString(_timeLeftFormat);
				_timeLeftLoc.Add("time", variable);
				_timeLeftLabel.Text = _timeLeftLoc.GetFormattedText();
				_lastSetTimeLeftSecond = timeSpan.Seconds;
			}
			if (_lobby.NetService.IsConnected)
			{
				_lobby.NetService.Update();
			}
		}
	}

	public void PlayerConnected(LobbyPlayer player)
	{
		_remotePlayerContainer.OnPlayerConnected(player);
		SetupLobbyParams(_lobby);
		InitializeLeaderboard();
		UpdateRichPresence();
	}

	public void PlayerChanged(LobbyPlayer player)
	{
		_remotePlayerContainer.OnPlayerChanged(player);
		if (player.id == _netService.NetId && _netService.Type.IsMultiplayer())
		{
			_characterContainer.SetIsReady(player.isReady);
		}
	}

	public void MaxAscensionChanged()
	{
	}

	public void AscensionChanged()
	{
		InitializeDisplay();
	}

	public void SeedChanged()
	{
	}

	public void ModifiersChanged()
	{
		InitializeDisplay();
	}

	public void RemotePlayerDisconnected(LobbyPlayer player)
	{
		_remotePlayerContainer.OnPlayerDisconnected(player);
		SetupLobbyParams(_lobby);
		InitializeLeaderboard();
		UpdateRichPresence();
	}

	public void BeginRun(string seed, List<ActModel> acts, IReadOnlyList<ModifierModel> modifiers)
	{
		NAudioManager.Instance?.StopMusic();
		if (_lobby.NetService.Type == NetGameType.Singleplayer)
		{
			TaskHelper.RunSafely(StartNewSingleplayerRun(seed, acts, modifiers));
		}
		else
		{
			TaskHelper.RunSafely(StartNewMultiplayerRun(seed, acts, modifiers));
		}
	}

	public void LocalPlayerDisconnected(NetErrorInfo info)
	{
		if (info.SelfInitiated && info.GetReason() == NetError.Quit)
		{
			return;
		}
		_stack.Pop();
		if (TestMode.IsOff)
		{
			NErrorPopup nErrorPopup = NErrorPopup.Create(info);
			if (nErrorPopup != null)
			{
				NModalContainer.Instance.Add((Node)(object)nErrorPopup);
			}
		}
	}

	private void OnEmbarkPressed(NButton _)
	{
		_embarkButton.Disable();
		_backButton.Disable();
		_lobby.SetReady(ready: true);
		if (_lobby.NetService.Type != NetGameType.Singleplayer && !_lobby.IsAboutToBeginGame())
		{
			((CanvasItem)_readyAndWaitingContainer).Visible = true;
			_unreadyButton.Enable();
		}
	}

	private void OnUnreadyPressed(NButton _)
	{
		_lobby.SetReady(ready: false);
		((CanvasItem)_readyAndWaitingContainer).Visible = false;
		_embarkButton.Enable();
		_backButton.Enable();
		_unreadyButton.Disable();
	}

	private void UpdateRichPresence()
	{
		StartRunLobby? lobby = _lobby;
		if (lobby != null && lobby.NetService.Type.IsMultiplayer())
		{
			PlatformUtil.SetRichPresence("DAILY_MP_LOBBY", _lobby.NetService.GetRawLobbyIdentifier(), _lobby.Players.Count);
		}
	}

	public async Task StartNewSingleplayerRun(string seed, List<ActModel> acts, IReadOnlyList<ModifierModel> modifiers)
	{
		Log.Info($"Embarking on a DAILY {_lobby.LocalPlayer.character.Id.Entry} run with {_lobby.Players.Count} players. Ascension: {_lobby.Ascension} Seed: {seed}");
		SfxCmd.Play(_lobby.LocalPlayer.character.CharacterTransitionSfx);
		await NGame.Instance.Transition.FadeOut(0.8f, _lobby.LocalPlayer.character.CharacterSelectTransitionPath);
		await NGame.Instance.StartNewSingleplayerRun(_lobby.LocalPlayer.character, shouldSave: true, acts, modifiers, seed, _lobby.Ascension, _lobby.DailyTime.Value.serverTime);
		CleanUpLobby(disconnectSession: false);
	}

	public async Task StartNewMultiplayerRun(string seed, List<ActModel> acts, IReadOnlyList<ModifierModel> modifiers)
	{
		Log.Info($"Embarking on a DAILY multiplayer run. Players: {string.Join(",", _lobby.Players)}. Ascension: {_lobby.Ascension} Seed: {seed}");
		SfxCmd.Play(_lobby.LocalPlayer.character.CharacterTransitionSfx);
		await NGame.Instance.Transition.FadeOut(0.8f, _lobby.LocalPlayer.character.CharacterSelectTransitionPath);
		await NGame.Instance.StartNewMultiplayerRun(_lobby, shouldSave: true, acts, modifiers, seed, _lobby.Ascension, _lobby.DailyTime.Value.serverTime);
		CleanUpLobby(disconnectSession: false);
	}

	private void CleanUpLobby(bool disconnectSession)
	{
		_lobby?.CleanUp(disconnectSession);
		_lobby = null;
	}

	private void AfterLobbyInitialized()
	{
		NGame.Instance.RemoteCursorContainer.Initialize(_lobby.InputSynchronizer, _lobby.Players.Select((LobbyPlayer p) => p.id));
		NGame.Instance.ReactionContainer.InitializeNetworking(_lobby.NetService);
		NGame.Instance.TimeoutOverlay.Initialize(_lobby.NetService, isGameLevel: true);
		_remotePlayerContainer.Initialize(_lobby, displayLocalPlayer: false);
		UpdateRichPresence();
		Logger.logLevelTypeMap[LogType.Network] = ((_lobby.NetService.Type == NetGameType.Singleplayer) ? LogLevel.Info : LogLevel.Debug);
		Logger.logLevelTypeMap[LogType.Actions] = ((_lobby.NetService.Type == NetGameType.Singleplayer) ? LogLevel.Info : LogLevel.VeryDebug);
		Logger.logLevelTypeMap[LogType.GameSync] = ((_lobby.NetService.Type == NetGameType.Singleplayer) ? LogLevel.Info : LogLevel.VeryDebug);
		NGame.Instance.DebugSeedOverride = null;
		_embarkButton.Enable();
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Expected O, but got Unknown
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0288: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0305: Unknown result type (might be due to invalid IL or missing references)
		//IL_0310: Expected O, but got Unknown
		//IL_030b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0316: Unknown result type (might be due to invalid IL or missing references)
		//IL_033c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0364: Unknown result type (might be due to invalid IL or missing references)
		//IL_036f: Expected O, but got Unknown
		//IL_036a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0375: Unknown result type (might be due to invalid IL or missing references)
		//IL_039b: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_041e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0427: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(18);
		list.Add(new MethodInfo(MethodName.Create, new PropertyInfo((Type)24, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false), (MethodFlags)33, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.InitializeSingleplayer, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnSubmenuOpened, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnSubmenuClosed, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.InitializeLeaderboard, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.InitializeDisplay, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetIsLoading, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)1, StringName.op_Implicit("isLoading"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Process, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)3, StringName.op_Implicit("delta"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.MaxAscensionChanged, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AscensionChanged, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SeedChanged, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ModifiersChanged, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnEmbarkPressed, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnUnreadyPressed, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UpdateRichPresence, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.CleanUpLobby, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)1, StringName.op_Implicit("disconnectSession"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AfterLobbyInitialized, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_0286: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02de: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.Create && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			NDailyRunScreen nDailyRunScreen = Create();
			ret = VariantUtils.CreateFrom<NDailyRunScreen>(ref nDailyRunScreen);
			return true;
		}
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.InitializeSingleplayer && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			InitializeSingleplayer();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnSubmenuOpened && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnSubmenuOpened();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnSubmenuClosed && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnSubmenuClosed();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.InitializeLeaderboard && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			InitializeLeaderboard();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.InitializeDisplay && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			InitializeDisplay();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetIsLoading && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			SetIsLoading(VariantUtils.ConvertTo<bool>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._Process && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			((Node)this)._Process(VariantUtils.ConvertTo<double>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.MaxAscensionChanged && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			MaxAscensionChanged();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.AscensionChanged && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			AscensionChanged();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SeedChanged && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			SeedChanged();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.ModifiersChanged && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			ModifiersChanged();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnEmbarkPressed && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OnEmbarkPressed(VariantUtils.ConvertTo<NButton>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnUnreadyPressed && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OnUnreadyPressed(VariantUtils.ConvertTo<NButton>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.UpdateRichPresence && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			UpdateRichPresence();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.CleanUpLobby && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			CleanUpLobby(VariantUtils.ConvertTo<bool>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.AfterLobbyInitialized && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			AfterLobbyInitialized();
			ret = default(godot_variant);
			return true;
		}
		return base.InvokeGodotClassMethod(in method, args, out ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static bool InvokeGodotClassStaticMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.Create && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			NDailyRunScreen nDailyRunScreen = Create();
			ret = VariantUtils.CreateFrom<NDailyRunScreen>(ref nDailyRunScreen);
			return true;
		}
		ret = default(godot_variant);
		return false;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if ((ref method) == MethodName.Create)
		{
			return true;
		}
		if ((ref method) == MethodName._Ready)
		{
			return true;
		}
		if ((ref method) == MethodName.InitializeSingleplayer)
		{
			return true;
		}
		if ((ref method) == MethodName.OnSubmenuOpened)
		{
			return true;
		}
		if ((ref method) == MethodName.OnSubmenuClosed)
		{
			return true;
		}
		if ((ref method) == MethodName.InitializeLeaderboard)
		{
			return true;
		}
		if ((ref method) == MethodName.InitializeDisplay)
		{
			return true;
		}
		if ((ref method) == MethodName.SetIsLoading)
		{
			return true;
		}
		if ((ref method) == MethodName._Process)
		{
			return true;
		}
		if ((ref method) == MethodName.MaxAscensionChanged)
		{
			return true;
		}
		if ((ref method) == MethodName.AscensionChanged)
		{
			return true;
		}
		if ((ref method) == MethodName.SeedChanged)
		{
			return true;
		}
		if ((ref method) == MethodName.ModifiersChanged)
		{
			return true;
		}
		if ((ref method) == MethodName.OnEmbarkPressed)
		{
			return true;
		}
		if ((ref method) == MethodName.OnUnreadyPressed)
		{
			return true;
		}
		if ((ref method) == MethodName.UpdateRichPresence)
		{
			return true;
		}
		if ((ref method) == MethodName.CleanUpLobby)
		{
			return true;
		}
		if ((ref method) == MethodName.AfterLobbyInitialized)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._titleLabel)
		{
			_titleLabel = VariantUtils.ConvertTo<MegaLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._dateLabel)
		{
			_dateLabel = VariantUtils.ConvertTo<MegaRichTextLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._timeLeftLabel)
		{
			_timeLeftLabel = VariantUtils.ConvertTo<MegaRichTextLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._characterContainer)
		{
			_characterContainer = VariantUtils.ConvertTo<NDailyRunCharacterContainer>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._embarkButton)
		{
			_embarkButton = VariantUtils.ConvertTo<NConfirmButton>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._backButton)
		{
			_backButton = VariantUtils.ConvertTo<NBackButton>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._unreadyButton)
		{
			_unreadyButton = VariantUtils.ConvertTo<NBackButton>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._leaderboard)
		{
			_leaderboard = VariantUtils.ConvertTo<NDailyRunLeaderboard>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._modifiersTitleLabel)
		{
			_modifiersTitleLabel = VariantUtils.ConvertTo<MegaLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._modifiersContainer)
		{
			_modifiersContainer = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._remotePlayerContainer)
		{
			_remotePlayerContainer = VariantUtils.ConvertTo<NRemoteLobbyPlayerContainer>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._readyAndWaitingContainer)
		{
			_readyAndWaitingContainer = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		return base.SetGodotClassPropertyValue(in name, in value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName.InitialFocusedControl)
		{
			Control initialFocusedControl = InitialFocusedControl;
			value = VariantUtils.CreateFrom<Control>(ref initialFocusedControl);
			return true;
		}
		if ((ref name) == PropertyName._titleLabel)
		{
			value = VariantUtils.CreateFrom<MegaLabel>(ref _titleLabel);
			return true;
		}
		if ((ref name) == PropertyName._dateLabel)
		{
			value = VariantUtils.CreateFrom<MegaRichTextLabel>(ref _dateLabel);
			return true;
		}
		if ((ref name) == PropertyName._timeLeftLabel)
		{
			value = VariantUtils.CreateFrom<MegaRichTextLabel>(ref _timeLeftLabel);
			return true;
		}
		if ((ref name) == PropertyName._characterContainer)
		{
			value = VariantUtils.CreateFrom<NDailyRunCharacterContainer>(ref _characterContainer);
			return true;
		}
		if ((ref name) == PropertyName._embarkButton)
		{
			value = VariantUtils.CreateFrom<NConfirmButton>(ref _embarkButton);
			return true;
		}
		if ((ref name) == PropertyName._backButton)
		{
			value = VariantUtils.CreateFrom<NBackButton>(ref _backButton);
			return true;
		}
		if ((ref name) == PropertyName._unreadyButton)
		{
			value = VariantUtils.CreateFrom<NBackButton>(ref _unreadyButton);
			return true;
		}
		if ((ref name) == PropertyName._leaderboard)
		{
			value = VariantUtils.CreateFrom<NDailyRunLeaderboard>(ref _leaderboard);
			return true;
		}
		if ((ref name) == PropertyName._modifiersTitleLabel)
		{
			value = VariantUtils.CreateFrom<MegaLabel>(ref _modifiersTitleLabel);
			return true;
		}
		if ((ref name) == PropertyName._modifiersContainer)
		{
			value = VariantUtils.CreateFrom<Control>(ref _modifiersContainer);
			return true;
		}
		if ((ref name) == PropertyName._remotePlayerContainer)
		{
			value = VariantUtils.CreateFrom<NRemoteLobbyPlayerContainer>(ref _remotePlayerContainer);
			return true;
		}
		if ((ref name) == PropertyName._readyAndWaitingContainer)
		{
			value = VariantUtils.CreateFrom<Control>(ref _readyAndWaitingContainer);
			return true;
		}
		return base.GetGodotClassPropertyValue(in name, out value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName.InitialFocusedControl, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._titleLabel, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._dateLabel, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._timeLeftLabel, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._characterContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._embarkButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._backButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._unreadyButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._leaderboard, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._modifiersTitleLabel, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._modifiersContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._remotePlayerContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._readyAndWaitingContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName._titleLabel, Variant.From<MegaLabel>(ref _titleLabel));
		info.AddProperty(PropertyName._dateLabel, Variant.From<MegaRichTextLabel>(ref _dateLabel));
		info.AddProperty(PropertyName._timeLeftLabel, Variant.From<MegaRichTextLabel>(ref _timeLeftLabel));
		info.AddProperty(PropertyName._characterContainer, Variant.From<NDailyRunCharacterContainer>(ref _characterContainer));
		info.AddProperty(PropertyName._embarkButton, Variant.From<NConfirmButton>(ref _embarkButton));
		info.AddProperty(PropertyName._backButton, Variant.From<NBackButton>(ref _backButton));
		info.AddProperty(PropertyName._unreadyButton, Variant.From<NBackButton>(ref _unreadyButton));
		info.AddProperty(PropertyName._leaderboard, Variant.From<NDailyRunLeaderboard>(ref _leaderboard));
		info.AddProperty(PropertyName._modifiersTitleLabel, Variant.From<MegaLabel>(ref _modifiersTitleLabel));
		info.AddProperty(PropertyName._modifiersContainer, Variant.From<Control>(ref _modifiersContainer));
		info.AddProperty(PropertyName._remotePlayerContainer, Variant.From<NRemoteLobbyPlayerContainer>(ref _remotePlayerContainer));
		info.AddProperty(PropertyName._readyAndWaitingContainer, Variant.From<Control>(ref _readyAndWaitingContainer));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._titleLabel, ref val))
		{
			_titleLabel = ((Variant)(ref val)).As<MegaLabel>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._dateLabel, ref val2))
		{
			_dateLabel = ((Variant)(ref val2)).As<MegaRichTextLabel>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._timeLeftLabel, ref val3))
		{
			_timeLeftLabel = ((Variant)(ref val3)).As<MegaRichTextLabel>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._characterContainer, ref val4))
		{
			_characterContainer = ((Variant)(ref val4)).As<NDailyRunCharacterContainer>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._embarkButton, ref val5))
		{
			_embarkButton = ((Variant)(ref val5)).As<NConfirmButton>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._backButton, ref val6))
		{
			_backButton = ((Variant)(ref val6)).As<NBackButton>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName._unreadyButton, ref val7))
		{
			_unreadyButton = ((Variant)(ref val7)).As<NBackButton>();
		}
		Variant val8 = default(Variant);
		if (info.TryGetProperty(PropertyName._leaderboard, ref val8))
		{
			_leaderboard = ((Variant)(ref val8)).As<NDailyRunLeaderboard>();
		}
		Variant val9 = default(Variant);
		if (info.TryGetProperty(PropertyName._modifiersTitleLabel, ref val9))
		{
			_modifiersTitleLabel = ((Variant)(ref val9)).As<MegaLabel>();
		}
		Variant val10 = default(Variant);
		if (info.TryGetProperty(PropertyName._modifiersContainer, ref val10))
		{
			_modifiersContainer = ((Variant)(ref val10)).As<Control>();
		}
		Variant val11 = default(Variant);
		if (info.TryGetProperty(PropertyName._remotePlayerContainer, ref val11))
		{
			_remotePlayerContainer = ((Variant)(ref val11)).As<NRemoteLobbyPlayerContainer>();
		}
		Variant val12 = default(Variant);
		if (info.TryGetProperty(PropertyName._readyAndWaitingContainer, ref val12))
		{
			_readyAndWaitingContainer = ((Variant)(ref val12)).As<Control>();
		}
	}
}
