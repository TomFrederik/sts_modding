using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.ControllerInput;
using MegaCrit.Sts2.Core.Debug;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Entities.UI;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Multiplayer;
using MegaCrit.Sts2.Core.Multiplayer.Game;
using MegaCrit.Sts2.Core.Multiplayer.Game.Lobby;
using MegaCrit.Sts2.Core.Multiplayer.Messages.Lobby;
using MegaCrit.Sts2.Core.Nodes.Audio;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Multiplayer;
using MegaCrit.Sts2.Core.Nodes.Screens.CharacterSelect;
using MegaCrit.Sts2.Core.Nodes.Screens.MainMenu;
using MegaCrit.Sts2.Core.Platform;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.TestSupport;
using MegaCrit.Sts2.Core.Unlocks;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.Nodes.Screens.CustomRun;

[ScriptPath("res://src/Core/Nodes/Screens/CustomRun/NCustomRunScreen.cs")]
public class NCustomRunScreen : NSubmenu, IStartRunLobbyListener, ICharacterSelectButtonDelegate
{
	public new class MethodName : NSubmenu.MethodName
	{
		public static readonly StringName Create = StringName.op_Implicit("Create");

		public new static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName InitializeSingleplayer = StringName.op_Implicit("InitializeSingleplayer");

		public static readonly StringName OnSeedInputSubmitted = StringName.op_Implicit("OnSeedInputSubmitted");

		public static readonly StringName InitCharacterButtons = StringName.op_Implicit("InitCharacterButtons");

		public static readonly StringName _Input = StringName.op_Implicit("_Input");

		public static readonly StringName DebugUnlockAllCharacters = StringName.op_Implicit("DebugUnlockAllCharacters");

		public new static readonly StringName OnSubmenuOpened = StringName.op_Implicit("OnSubmenuOpened");

		public new static readonly StringName OnSubmenuClosed = StringName.op_Implicit("OnSubmenuClosed");

		public static readonly StringName OnEmbarkPressed = StringName.op_Implicit("OnEmbarkPressed");

		public static readonly StringName OnUnreadyPressed = StringName.op_Implicit("OnUnreadyPressed");

		public static readonly StringName UpdateRichPresence = StringName.op_Implicit("UpdateRichPresence");

		public static readonly StringName _Process = StringName.op_Implicit("_Process");

		public static readonly StringName CleanUpLobby = StringName.op_Implicit("CleanUpLobby");

		public static readonly StringName GetModifiersString = StringName.op_Implicit("GetModifiersString");

		public static readonly StringName OnAscensionPanelLevelChanged = StringName.op_Implicit("OnAscensionPanelLevelChanged");

		public static readonly StringName OnModifiersListChanged = StringName.op_Implicit("OnModifiersListChanged");

		public static readonly StringName MaxAscensionChanged = StringName.op_Implicit("MaxAscensionChanged");

		public static readonly StringName AscensionChanged = StringName.op_Implicit("AscensionChanged");

		public static readonly StringName SeedChanged = StringName.op_Implicit("SeedChanged");

		public static readonly StringName ModifiersChanged = StringName.op_Implicit("ModifiersChanged");

		public static readonly StringName AfterInitialized = StringName.op_Implicit("AfterInitialized");

		public static readonly StringName UpdateControllerButton = StringName.op_Implicit("UpdateControllerButton");

		public static readonly StringName TryFocusOnModifiersList = StringName.op_Implicit("TryFocusOnModifiersList");
	}

	public new class PropertyName : NSubmenu.PropertyName
	{
		public static readonly StringName ModifiersHotkey = StringName.op_Implicit("ModifiersHotkey");

		public new static readonly StringName InitialFocusedControl = StringName.op_Implicit("InitialFocusedControl");

		public static readonly StringName _selectedButton = StringName.op_Implicit("_selectedButton");

		public static readonly StringName _charButtonContainer = StringName.op_Implicit("_charButtonContainer");

		public static readonly StringName _confirmButton = StringName.op_Implicit("_confirmButton");

		public new static readonly StringName _backButton = StringName.op_Implicit("_backButton");

		public static readonly StringName _unreadyButton = StringName.op_Implicit("_unreadyButton");

		public static readonly StringName _ascensionPanel = StringName.op_Implicit("_ascensionPanel");

		public static readonly StringName _readyAndWaitingContainer = StringName.op_Implicit("_readyAndWaitingContainer");

		public static readonly StringName _seedInput = StringName.op_Implicit("_seedInput");

		public static readonly StringName _remotePlayerContainer = StringName.op_Implicit("_remotePlayerContainer");

		public static readonly StringName _modifiersList = StringName.op_Implicit("_modifiersList");

		public static readonly StringName _modifiersHotkeyIcon = StringName.op_Implicit("_modifiersHotkeyIcon");

		public static readonly StringName _uiMode = StringName.op_Implicit("_uiMode");
	}

	public new class SignalName : NSubmenu.SignalName
	{
	}

	private static readonly string _scenePath = SceneHelper.GetScenePath("screens/custom_run/custom_run_screen");

	private const string _sceneCharSelectButtonPath = "res://scenes/screens/char_select/char_select_button.tscn";

	private NCharacterSelectButton? _selectedButton;

	private Control _charButtonContainer;

	private NConfirmButton _confirmButton;

	private NBackButton _backButton;

	private NBackButton _unreadyButton;

	private NAscensionPanel _ascensionPanel;

	private Control _readyAndWaitingContainer;

	private LineEdit _seedInput;

	private NRemoteLobbyPlayerContainer _remotePlayerContainer;

	private NCustomRunModifiersList _modifiersList;

	private TextureRect _modifiersHotkeyIcon;

	private StartRunLobby _lobby;

	private MultiplayerUiMode _uiMode;

	private string ModifiersHotkey => StringName.op_Implicit(MegaInput.topPanel);

	public StartRunLobby Lobby => _lobby;

	public static IEnumerable<string> AssetPaths => new global::_003C_003Ez__ReadOnlyArray<string>(new string[3] { _scenePath, "res://scenes/screens/char_select/char_select_button.tscn", "res://scenes/screens/custom_run/modifier_tickbox.tscn" });

	protected override Control InitialFocusedControl => ((Node)_charButtonContainer).GetChild<Control>(0, false);

	public static NCustomRunScreen? Create()
	{
		if (TestMode.IsOn)
		{
			return null;
		}
		return PreloadManager.Cache.GetScene(_scenePath).Instantiate<NCustomRunScreen>((GenEditState)0);
	}

	public override void _Ready()
	{
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Unknown result type (might be due to invalid IL or missing references)
		//IL_0271: Unknown result type (might be due to invalid IL or missing references)
		//IL_0277: Unknown result type (might be due to invalid IL or missing references)
		//IL_0293: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Unknown result type (might be due to invalid IL or missing references)
		ConnectSignals();
		_charButtonContainer = ((Node)this).GetNode<Control>(NodePath.op_Implicit("LeftContainer/CharSelectButtons/ButtonContainer"));
		_ascensionPanel = ((Node)this).GetNode<NAscensionPanel>(NodePath.op_Implicit("%AscensionPanel"));
		_remotePlayerContainer = ((Node)this).GetNode<NRemoteLobbyPlayerContainer>(NodePath.op_Implicit("%RemotePlayerContainer"));
		_readyAndWaitingContainer = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%ReadyAndWaitingPanel"));
		_modifiersList = ((Node)this).GetNode<NCustomRunModifiersList>(NodePath.op_Implicit("%ModifiersList"));
		_seedInput = ((Node)this).GetNode<LineEdit>(NodePath.op_Implicit("%SeedInput"));
		_confirmButton = ((Node)this).GetNode<NConfirmButton>(NodePath.op_Implicit("ConfirmButton"));
		_backButton = ((Node)this).GetNode<NBackButton>(NodePath.op_Implicit("BackButton"));
		_unreadyButton = ((Node)this).GetNode<NBackButton>(NodePath.op_Implicit("UnreadyButton"));
		_modifiersHotkeyIcon = ((Node)this).GetNode<TextureRect>(NodePath.op_Implicit("%ModifiersHotkeyIcon"));
		((GodotObject)_confirmButton).Connect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)OnEmbarkPressed), 0u);
		((GodotObject)_unreadyButton).Connect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)OnUnreadyPressed), 0u);
		((GodotObject)_ascensionPanel).Connect(NAscensionPanel.SignalName.AscensionLevelChanged, Callable.From((Action)OnAscensionPanelLevelChanged), 0u);
		((GodotObject)_modifiersList).Connect(NCustomRunModifiersList.SignalName.ModifiersChanged, Callable.From((Action)OnModifiersListChanged), 0u);
		((Node)this).ProcessMode = (ProcessModeEnum)4;
		((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%CustomModeTitle")).SetTextAutoSize(new LocString("main_menu_ui", "CUSTOM_RUN_SCREEN.CUSTOM_MODE_TITLE").GetFormattedText());
		((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%ModifiersTitle")).SetTextAutoSize(new LocString("main_menu_ui", "CUSTOM_RUN_SCREEN.MODIFIERS_TITLE").GetFormattedText());
		((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%SeedLabel")).SetTextAutoSize(new LocString("main_menu_ui", "CUSTOM_RUN_SCREEN.SEED_LABEL").GetFormattedText());
		_seedInput.PlaceholderText = new LocString("main_menu_ui", "CUSTOM_RUN_SCREEN.SEED_RANDOM_PLACEHOLDER").GetFormattedText();
		((GodotObject)_seedInput).Connect(SignalName.TextChanged, Callable.From<string>((Action<string>)OnSeedInputSubmitted), 0u);
		InitCharacterButtons();
		((GodotObject)NControllerManager.Instance).Connect(NControllerManager.SignalName.MouseDetected, Callable.From((Action)UpdateControllerButton), 0u);
		((GodotObject)NControllerManager.Instance).Connect(NControllerManager.SignalName.ControllerDetected, Callable.From((Action)UpdateControllerButton), 0u);
		((GodotObject)NInputManager.Instance).Connect(NInputManager.SignalName.InputRebound, Callable.From((Action)UpdateControllerButton), 0u);
	}

	public void InitializeMultiplayerAsHost(INetGameService gameService, int maxPlayers)
	{
		if (gameService.Type != NetGameType.Host)
		{
			throw new InvalidOperationException($"Initialized character select screen with GameService of type {gameService.Type} when hosting!");
		}
		_lobby = new StartRunLobby(GameMode.Custom, gameService, this, maxPlayers);
		_ascensionPanel.Initialize(MultiplayerUiMode.Host);
		_modifiersList.Initialize(MultiplayerUiMode.Host);
		_lobby.AddLocalHostPlayer(new UnlockState(SaveManager.Instance.Progress), SaveManager.Instance.Progress.MaxMultiplayerAscension);
		_uiMode = MultiplayerUiMode.Host;
		((CanvasItem)_remotePlayerContainer).Visible = true;
		UpdateControllerButton();
		AfterInitialized();
	}

	public void InitializeMultiplayerAsClient(INetGameService gameService, ClientLobbyJoinResponseMessage message)
	{
		if (gameService.Type != NetGameType.Client)
		{
			throw new InvalidOperationException($"Initialized character select screen with GameService of type {gameService.Type} when joining!");
		}
		_lobby = new StartRunLobby(GameMode.Custom, gameService, this, -1);
		_ascensionPanel.Initialize(MultiplayerUiMode.Client);
		_modifiersList.Initialize(MultiplayerUiMode.Client);
		_lobby.InitializeFromMessage(message);
		_seedInput.Editable = false;
		_uiMode = MultiplayerUiMode.Client;
		UpdateControllerButton();
		AfterInitialized();
	}

	public void InitializeSingleplayer()
	{
		_lobby = new StartRunLobby(GameMode.Custom, new NetSingleplayerGameService(), this, 1);
		((CanvasItem)_remotePlayerContainer).Visible = false;
		_ascensionPanel.Initialize(MultiplayerUiMode.Singleplayer);
		_modifiersList.Initialize(MultiplayerUiMode.Singleplayer);
		_lobby.AddLocalHostPlayer(new UnlockState(SaveManager.Instance.Progress), 0);
		_uiMode = MultiplayerUiMode.Singleplayer;
		UpdateControllerButton();
		AfterInitialized();
	}

	private void OnSeedInputSubmitted(string newText)
	{
		if (newText != string.Empty)
		{
			Lobby.SetSeed(newText);
		}
		else
		{
			Lobby.SetSeed(null);
		}
	}

	private void InitCharacterButtons()
	{
		foreach (CharacterModel allCharacter in ModelDb.AllCharacters)
		{
			NCharacterSelectButton nCharacterSelectButton = PreloadManager.Cache.GetScene("res://scenes/screens/char_select/char_select_button.tscn").Instantiate<NCharacterSelectButton>((GenEditState)0);
			((Node)nCharacterSelectButton).Name = StringName.op_Implicit(allCharacter.Id.Entry + "_button");
			((Node)(object)_charButtonContainer).AddChildSafely((Node?)(object)nCharacterSelectButton);
			nCharacterSelectButton.Init(allCharacter, this);
		}
		for (int i = 0; i < ((Node)_charButtonContainer).GetChildCount(false); i++)
		{
			Control child = ((Node)_charButtonContainer).GetChild<Control>(i, false);
			child.FocusNeighborLeft = ((i > 0) ? ((Node)((Node)_charButtonContainer).GetChild<Control>(i - 1, false)).GetPath() : ((Node)child).GetPath());
			child.FocusNeighborRight = ((i < ((Node)_charButtonContainer).GetChildCount(false) - 1) ? ((Node)((Node)_charButtonContainer).GetChild<Control>(i + 1, false)).GetPath() : ((Node)child).GetPath());
			child.FocusNeighborTop = ((Node)_seedInput).GetPath();
			child.FocusNeighborBottom = ((Node)child).GetPath();
		}
	}

	public override void _Input(InputEvent inputEvent)
	{
		if (inputEvent.IsActionReleased(DebugHotkey.unlockCharacters, false))
		{
			DebugUnlockAllCharacters();
		}
	}

	private void DebugUnlockAllCharacters()
	{
		foreach (NCharacterSelectButton item in ((IEnumerable)((Node)_charButtonContainer).GetChildren(false)).OfType<NCharacterSelectButton>())
		{
			item.DebugUnlock();
		}
	}

	public override void OnSubmenuOpened()
	{
		base.OnSubmenuOpened();
		foreach (NCharacterSelectButton item in ((IEnumerable)((Node)_charButtonContainer).GetChildren(false)).OfType<NCharacterSelectButton>())
		{
			if (!item.IsLocked)
			{
				item.Enable();
				item.Reset();
			}
			else
			{
				item.UnlockIfPossible();
			}
		}
		_confirmButton.Enable();
		((Node)_charButtonContainer).GetChild<NCharacterSelectButton>(0, false).Select();
		_remotePlayerContainer.Initialize(_lobby, displayLocalPlayer: true);
		if (_lobby.NetService.Type == NetGameType.Client)
		{
			_ascensionPanel.SetAscensionLevel(_lobby.Ascension);
			_seedInput.Text = _lobby.Seed ?? "";
		}
		((CanvasItem)_readyAndWaitingContainer).Visible = false;
		foreach (LobbyPlayer player in _lobby.Players)
		{
			RefreshButtonSelectionForPlayer(player);
		}
		((Node)this).ProcessMode = (ProcessModeEnum)0;
		NHotkeyManager.Instance.PushHotkeyPressedBinding(ModifiersHotkey, TryFocusOnModifiersList);
	}

	public override void OnSubmenuClosed()
	{
		base.OnSubmenuClosed();
		_confirmButton.Disable();
		_remotePlayerContainer.Cleanup();
		if (_lobby.NetService.Type.IsMultiplayer())
		{
			PlatformUtil.SetRichPresence("MAIN_MENU", null, null);
		}
		CleanUpLobby(disconnectSession: true);
		NHotkeyManager.Instance.RemoveHotkeyPressedBinding(ModifiersHotkey, TryFocusOnModifiersList);
	}

	private void OnEmbarkPressed(NButton _)
	{
		_confirmButton.Disable();
		_backButton.Disable();
		_lobby.SetReady(ready: true);
		foreach (NCharacterSelectButton item in ((IEnumerable)((Node)_charButtonContainer).GetChildren(false)).OfType<NCharacterSelectButton>())
		{
			item.Disable();
		}
		if (_lobby.NetService.Type.IsMultiplayer() && !_lobby.IsAboutToBeginGame())
		{
			_unreadyButton.Enable();
			((CanvasItem)_readyAndWaitingContainer).Visible = true;
		}
	}

	private void OnUnreadyPressed(NButton _)
	{
		_confirmButton.Enable();
		_backButton.Enable();
		_unreadyButton.Disable();
		_lobby.SetReady(ready: false);
		((CanvasItem)_readyAndWaitingContainer).Visible = false;
		foreach (NCharacterSelectButton item in ((IEnumerable)((Node)_charButtonContainer).GetChildren(false)).OfType<NCharacterSelectButton>())
		{
			item.Enable();
		}
	}

	private void UpdateRichPresence()
	{
		if (_lobby.NetService.Type.IsMultiplayer())
		{
			PlatformUtil.SetRichPresence("CUSTOM_MP_LOBBY", _lobby.NetService.GetRawLobbyIdentifier(), _lobby.Players.Count);
		}
	}

	public override void _Process(double delta)
	{
		if (_lobby.NetService.IsConnected)
		{
			_lobby.NetService.Update();
		}
	}

	private void CleanUpLobby(bool disconnectSession)
	{
		_lobby.CleanUp(disconnectSession);
		_lobby = null;
		if (GodotObject.IsInstanceValid((GodotObject)(object)this))
		{
			((Node)this).ProcessMode = (ProcessModeEnum)4;
		}
	}

	private async Task StartNewSingleplayerRun(string seed, List<ActModel> acts, IReadOnlyList<ModifierModel> modifiers)
	{
		Log.Info($"Embarking on a CUSTOM {_lobby.LocalPlayer.character.Id.Entry} run. Ascension: {_lobby.Ascension} Seed: {_lobby.Seed} Modifiers: {GetModifiersString()}");
		SfxCmd.Play(_lobby.LocalPlayer.character.CharacterTransitionSfx);
		await NGame.Instance.Transition.FadeOut(0.8f, _lobby.LocalPlayer.character.CharacterSelectTransitionPath);
		await NGame.Instance.StartNewSingleplayerRun(_lobby.LocalPlayer.character, shouldSave: true, acts, modifiers, seed, _lobby.Ascension);
		CleanUpLobby(disconnectSession: false);
	}

	private async Task StartNewMultiplayerRun(string seed, List<ActModel> acts, IReadOnlyList<ModifierModel> modifiers)
	{
		Log.Info($"Embarking on a CUSTOM multiplayer run. Players: {string.Join(",", _lobby.Players)}. Ascension: {_lobby.Ascension} Seed: {_lobby.Seed} Modifiers: {GetModifiersString()}");
		SfxCmd.Play(_lobby.LocalPlayer.character.CharacterTransitionSfx);
		await NGame.Instance.Transition.FadeOut(0.8f, _lobby.LocalPlayer.character.CharacterSelectTransitionPath);
		await NGame.Instance.StartNewMultiplayerRun(_lobby, shouldSave: true, acts, modifiers, seed, _lobby.Ascension);
		CleanUpLobby(disconnectSession: false);
	}

	private string GetModifiersString()
	{
		return string.Join(",", _lobby.Modifiers.Select((ModifierModel m) => m.Id));
	}

	public void SelectCharacter(NCharacterSelectButton charSelectButton, CharacterModel characterModel)
	{
		if (_lobby == null)
		{
			throw new InvalidOperationException("Cannot select character while loading!");
		}
		SfxCmd.Play(characterModel.CharacterSelectSfx);
		_selectedButton = charSelectButton;
		foreach (NCharacterSelectButton item in ((IEnumerable)((Node)_charButtonContainer).GetChildren(false)).OfType<NCharacterSelectButton>())
		{
			if (item != _selectedButton)
			{
				item.Deselect();
			}
		}
		_lobby.SetLocalCharacter(characterModel);
	}

	private void OnAscensionPanelLevelChanged()
	{
		if (_lobby.NetService.Type != NetGameType.Client && _lobby.Ascension != _ascensionPanel.Ascension)
		{
			_lobby.SyncAscensionChange(_ascensionPanel.Ascension);
		}
	}

	private void OnModifiersListChanged()
	{
		if (_lobby.NetService.Type != NetGameType.Client)
		{
			Lobby.SetModifiers(_modifiersList.GetModifiersTickedOn());
		}
	}

	public void MaxAscensionChanged()
	{
		_ascensionPanel.SetMaxAscension(_lobby.MaxAscension);
	}

	public void PlayerConnected(LobbyPlayer player)
	{
		_remotePlayerContainer.OnPlayerConnected(player);
		RefreshButtonSelectionForPlayer(player);
		UpdateRichPresence();
	}

	public void PlayerChanged(LobbyPlayer player)
	{
		_remotePlayerContainer.OnPlayerChanged(player);
		RefreshButtonSelectionForPlayer(player);
	}

	private void RefreshButtonSelectionForPlayer(LobbyPlayer player)
	{
		if (player.id == _lobby.LocalPlayer.id)
		{
			return;
		}
		foreach (NCharacterSelectButton item in ((IEnumerable)((Node)_charButtonContainer).GetChildren(false)).OfType<NCharacterSelectButton>())
		{
			if (item.RemoteSelectedPlayers.Contains(player.id) && player.character != item.Character)
			{
				item.OnRemotePlayerDeselected(player.id);
			}
			else if (player.character == item.Character)
			{
				item.OnRemotePlayerSelected(player.id);
			}
		}
	}

	public void AscensionChanged()
	{
		if (_lobby.NetService.Type == NetGameType.Client)
		{
			((CanvasItem)_ascensionPanel).Visible = _lobby.Ascension > 0;
		}
		_ascensionPanel.SetAscensionLevel(_lobby.Ascension);
	}

	public void SeedChanged()
	{
		NetGameType type = _lobby.NetService.Type;
		if ((uint)(type - 1) > 1u)
		{
			_seedInput.Text = Lobby.Seed;
		}
	}

	public void ModifiersChanged()
	{
		NetGameType type = _lobby.NetService.Type;
		if ((uint)(type - 1) > 1u)
		{
			_modifiersList.SyncModifierList(Lobby.Modifiers);
		}
	}

	public void RemotePlayerDisconnected(LobbyPlayer player)
	{
		_remotePlayerContainer.OnPlayerDisconnected(player);
		foreach (NCharacterSelectButton item in ((IEnumerable)((Node)_charButtonContainer).GetChildren(false)).OfType<NCharacterSelectButton>())
		{
			if (item.RemoteSelectedPlayers.Contains(player.id) && player.character == item.Character)
			{
				item.OnRemotePlayerDeselected(player.id);
			}
		}
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

	private void AfterInitialized()
	{
		NGame.Instance.RemoteCursorContainer.Initialize(_lobby.InputSynchronizer, _lobby.Players.Select((LobbyPlayer p) => p.id));
		NGame.Instance.ReactionContainer.InitializeNetworking(_lobby.NetService);
		NGame.Instance.TimeoutOverlay.Initialize(_lobby.NetService, isGameLevel: true);
		UpdateRichPresence();
		if (!string.IsNullOrEmpty(_seedInput.Text))
		{
			_lobby.SetSeed(_seedInput.Text);
		}
		Logger.logLevelTypeMap[LogType.Network] = ((_lobby.NetService.Type == NetGameType.Singleplayer) ? LogLevel.Info : LogLevel.Debug);
		Logger.logLevelTypeMap[LogType.Actions] = ((_lobby.NetService.Type == NetGameType.Singleplayer) ? LogLevel.Info : LogLevel.VeryDebug);
		Logger.logLevelTypeMap[LogType.GameSync] = ((_lobby.NetService.Type == NetGameType.Singleplayer) ? LogLevel.Info : LogLevel.VeryDebug);
		NGame.Instance.DebugSeedOverride = null;
	}

	private void UpdateControllerButton()
	{
		MultiplayerUiMode uiMode = _uiMode;
		if ((uint)(uiMode - 1) <= 1u)
		{
			((CanvasItem)_modifiersHotkeyIcon).Visible = NControllerManager.Instance.IsUsingController;
			_modifiersHotkeyIcon.Texture = NInputManager.Instance.GetHotkeyIcon(ModifiersHotkey);
		}
		else
		{
			((CanvasItem)_modifiersHotkeyIcon).Visible = false;
		}
	}

	private void TryFocusOnModifiersList()
	{
		Control val = ((Node)this).GetViewport().GuiGetFocusOwner();
		if (val == null || !((Node)_modifiersList).IsAncestorOf((Node)(object)val))
		{
			MultiplayerUiMode uiMode = _uiMode;
			if ((uint)(uiMode - 1) <= 1u)
			{
				_modifiersList.DefaultFocusedControl?.TryGrabFocus();
			}
		}
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
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Expected O, but got Unknown
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Expected O, but got Unknown
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_028b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02be: Expected O, but got Unknown
		//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0319: Unknown result type (might be due to invalid IL or missing references)
		//IL_033c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0347: Unknown result type (might be due to invalid IL or missing references)
		//IL_036d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0390: Unknown result type (might be due to invalid IL or missing references)
		//IL_039b: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_041f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0428: Unknown result type (might be due to invalid IL or missing references)
		//IL_044e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0457: Unknown result type (might be due to invalid IL or missing references)
		//IL_047d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0486: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04db: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_050a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0513: Unknown result type (might be due to invalid IL or missing references)
		//IL_0539: Unknown result type (might be due to invalid IL or missing references)
		//IL_0542: Unknown result type (might be due to invalid IL or missing references)
		//IL_0568: Unknown result type (might be due to invalid IL or missing references)
		//IL_0571: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(24);
		list.Add(new MethodInfo(MethodName.Create, new PropertyInfo((Type)24, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false), (MethodFlags)33, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.InitializeSingleplayer, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnSeedInputSubmitted, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)4, StringName.op_Implicit("newText"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.InitCharacterButtons, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Input, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("inputEvent"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("InputEvent"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.DebugUnlockAllCharacters, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnSubmenuOpened, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnSubmenuClosed, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnEmbarkPressed, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnUnreadyPressed, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UpdateRichPresence, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Process, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)3, StringName.op_Implicit("delta"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.CleanUpLobby, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)1, StringName.op_Implicit("disconnectSession"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.GetModifiersString, new PropertyInfo((Type)4, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnAscensionPanelLevelChanged, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnModifiersListChanged, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.MaxAscensionChanged, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AscensionChanged, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SeedChanged, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ModifiersChanged, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AfterInitialized, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UpdateControllerButton, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.TryFocusOnModifiersList, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_0280: Unknown result type (might be due to invalid IL or missing references)
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0318: Unknown result type (might be due to invalid IL or missing references)
		//IL_033d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0362: Unknown result type (might be due to invalid IL or missing references)
		//IL_0387: Unknown result type (might be due to invalid IL or missing references)
		//IL_03db: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d1: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.Create && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			NCustomRunScreen nCustomRunScreen = Create();
			ret = VariantUtils.CreateFrom<NCustomRunScreen>(ref nCustomRunScreen);
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
		if ((ref method) == MethodName.OnSeedInputSubmitted && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OnSeedInputSubmitted(VariantUtils.ConvertTo<string>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.InitCharacterButtons && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			InitCharacterButtons();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._Input && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			((Node)this)._Input(VariantUtils.ConvertTo<InputEvent>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.DebugUnlockAllCharacters && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			DebugUnlockAllCharacters();
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
		if ((ref method) == MethodName._Process && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			((Node)this)._Process(VariantUtils.ConvertTo<double>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.CleanUpLobby && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			CleanUpLobby(VariantUtils.ConvertTo<bool>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.GetModifiersString && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			string modifiersString = GetModifiersString();
			ret = VariantUtils.CreateFrom<string>(ref modifiersString);
			return true;
		}
		if ((ref method) == MethodName.OnAscensionPanelLevelChanged && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnAscensionPanelLevelChanged();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnModifiersListChanged && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnModifiersListChanged();
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
		if ((ref method) == MethodName.AfterInitialized && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			AfterInitialized();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.UpdateControllerButton && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			UpdateControllerButton();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.TryFocusOnModifiersList && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			TryFocusOnModifiersList();
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
			NCustomRunScreen nCustomRunScreen = Create();
			ret = VariantUtils.CreateFrom<NCustomRunScreen>(ref nCustomRunScreen);
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
		if ((ref method) == MethodName.OnSeedInputSubmitted)
		{
			return true;
		}
		if ((ref method) == MethodName.InitCharacterButtons)
		{
			return true;
		}
		if ((ref method) == MethodName._Input)
		{
			return true;
		}
		if ((ref method) == MethodName.DebugUnlockAllCharacters)
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
		if ((ref method) == MethodName._Process)
		{
			return true;
		}
		if ((ref method) == MethodName.CleanUpLobby)
		{
			return true;
		}
		if ((ref method) == MethodName.GetModifiersString)
		{
			return true;
		}
		if ((ref method) == MethodName.OnAscensionPanelLevelChanged)
		{
			return true;
		}
		if ((ref method) == MethodName.OnModifiersListChanged)
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
		if ((ref method) == MethodName.AfterInitialized)
		{
			return true;
		}
		if ((ref method) == MethodName.UpdateControllerButton)
		{
			return true;
		}
		if ((ref method) == MethodName.TryFocusOnModifiersList)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._selectedButton)
		{
			_selectedButton = VariantUtils.ConvertTo<NCharacterSelectButton>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._charButtonContainer)
		{
			_charButtonContainer = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._confirmButton)
		{
			_confirmButton = VariantUtils.ConvertTo<NConfirmButton>(ref value);
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
		if ((ref name) == PropertyName._ascensionPanel)
		{
			_ascensionPanel = VariantUtils.ConvertTo<NAscensionPanel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._readyAndWaitingContainer)
		{
			_readyAndWaitingContainer = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._seedInput)
		{
			_seedInput = VariantUtils.ConvertTo<LineEdit>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._remotePlayerContainer)
		{
			_remotePlayerContainer = VariantUtils.ConvertTo<NRemoteLobbyPlayerContainer>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._modifiersList)
		{
			_modifiersList = VariantUtils.ConvertTo<NCustomRunModifiersList>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._modifiersHotkeyIcon)
		{
			_modifiersHotkeyIcon = VariantUtils.ConvertTo<TextureRect>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._uiMode)
		{
			_uiMode = VariantUtils.ConvertTo<MultiplayerUiMode>(ref value);
			return true;
		}
		return base.SetGodotClassPropertyValue(in name, in value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName.ModifiersHotkey)
		{
			string modifiersHotkey = ModifiersHotkey;
			value = VariantUtils.CreateFrom<string>(ref modifiersHotkey);
			return true;
		}
		if ((ref name) == PropertyName.InitialFocusedControl)
		{
			Control initialFocusedControl = InitialFocusedControl;
			value = VariantUtils.CreateFrom<Control>(ref initialFocusedControl);
			return true;
		}
		if ((ref name) == PropertyName._selectedButton)
		{
			value = VariantUtils.CreateFrom<NCharacterSelectButton>(ref _selectedButton);
			return true;
		}
		if ((ref name) == PropertyName._charButtonContainer)
		{
			value = VariantUtils.CreateFrom<Control>(ref _charButtonContainer);
			return true;
		}
		if ((ref name) == PropertyName._confirmButton)
		{
			value = VariantUtils.CreateFrom<NConfirmButton>(ref _confirmButton);
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
		if ((ref name) == PropertyName._ascensionPanel)
		{
			value = VariantUtils.CreateFrom<NAscensionPanel>(ref _ascensionPanel);
			return true;
		}
		if ((ref name) == PropertyName._readyAndWaitingContainer)
		{
			value = VariantUtils.CreateFrom<Control>(ref _readyAndWaitingContainer);
			return true;
		}
		if ((ref name) == PropertyName._seedInput)
		{
			value = VariantUtils.CreateFrom<LineEdit>(ref _seedInput);
			return true;
		}
		if ((ref name) == PropertyName._remotePlayerContainer)
		{
			value = VariantUtils.CreateFrom<NRemoteLobbyPlayerContainer>(ref _remotePlayerContainer);
			return true;
		}
		if ((ref name) == PropertyName._modifiersList)
		{
			value = VariantUtils.CreateFrom<NCustomRunModifiersList>(ref _modifiersList);
			return true;
		}
		if ((ref name) == PropertyName._modifiersHotkeyIcon)
		{
			value = VariantUtils.CreateFrom<TextureRect>(ref _modifiersHotkeyIcon);
			return true;
		}
		if ((ref name) == PropertyName._uiMode)
		{
			value = VariantUtils.CreateFrom<MultiplayerUiMode>(ref _uiMode);
			return true;
		}
		return base.GetGodotClassPropertyValue(in name, out value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)4, PropertyName.ModifiersHotkey, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._selectedButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._charButtonContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._confirmButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._backButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._unreadyButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._ascensionPanel, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._readyAndWaitingContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._seedInput, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._remotePlayerContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._modifiersList, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._modifiersHotkeyIcon, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.InitialFocusedControl, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)2, PropertyName._uiMode, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		info.AddProperty(PropertyName._selectedButton, Variant.From<NCharacterSelectButton>(ref _selectedButton));
		info.AddProperty(PropertyName._charButtonContainer, Variant.From<Control>(ref _charButtonContainer));
		info.AddProperty(PropertyName._confirmButton, Variant.From<NConfirmButton>(ref _confirmButton));
		info.AddProperty(PropertyName._backButton, Variant.From<NBackButton>(ref _backButton));
		info.AddProperty(PropertyName._unreadyButton, Variant.From<NBackButton>(ref _unreadyButton));
		info.AddProperty(PropertyName._ascensionPanel, Variant.From<NAscensionPanel>(ref _ascensionPanel));
		info.AddProperty(PropertyName._readyAndWaitingContainer, Variant.From<Control>(ref _readyAndWaitingContainer));
		info.AddProperty(PropertyName._seedInput, Variant.From<LineEdit>(ref _seedInput));
		info.AddProperty(PropertyName._remotePlayerContainer, Variant.From<NRemoteLobbyPlayerContainer>(ref _remotePlayerContainer));
		info.AddProperty(PropertyName._modifiersList, Variant.From<NCustomRunModifiersList>(ref _modifiersList));
		info.AddProperty(PropertyName._modifiersHotkeyIcon, Variant.From<TextureRect>(ref _modifiersHotkeyIcon));
		info.AddProperty(PropertyName._uiMode, Variant.From<MultiplayerUiMode>(ref _uiMode));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._selectedButton, ref val))
		{
			_selectedButton = ((Variant)(ref val)).As<NCharacterSelectButton>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._charButtonContainer, ref val2))
		{
			_charButtonContainer = ((Variant)(ref val2)).As<Control>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._confirmButton, ref val3))
		{
			_confirmButton = ((Variant)(ref val3)).As<NConfirmButton>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._backButton, ref val4))
		{
			_backButton = ((Variant)(ref val4)).As<NBackButton>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._unreadyButton, ref val5))
		{
			_unreadyButton = ((Variant)(ref val5)).As<NBackButton>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._ascensionPanel, ref val6))
		{
			_ascensionPanel = ((Variant)(ref val6)).As<NAscensionPanel>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName._readyAndWaitingContainer, ref val7))
		{
			_readyAndWaitingContainer = ((Variant)(ref val7)).As<Control>();
		}
		Variant val8 = default(Variant);
		if (info.TryGetProperty(PropertyName._seedInput, ref val8))
		{
			_seedInput = ((Variant)(ref val8)).As<LineEdit>();
		}
		Variant val9 = default(Variant);
		if (info.TryGetProperty(PropertyName._remotePlayerContainer, ref val9))
		{
			_remotePlayerContainer = ((Variant)(ref val9)).As<NRemoteLobbyPlayerContainer>();
		}
		Variant val10 = default(Variant);
		if (info.TryGetProperty(PropertyName._modifiersList, ref val10))
		{
			_modifiersList = ((Variant)(ref val10)).As<NCustomRunModifiersList>();
		}
		Variant val11 = default(Variant);
		if (info.TryGetProperty(PropertyName._modifiersHotkeyIcon, ref val11))
		{
			_modifiersHotkeyIcon = ((Variant)(ref val11)).As<TextureRect>();
		}
		Variant val12 = default(Variant);
		if (info.TryGetProperty(PropertyName._uiMode, ref val12))
		{
			_uiMode = ((Variant)(ref val12)).As<MultiplayerUiMode>();
		}
	}
}
