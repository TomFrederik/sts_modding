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
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.ControllerInput;
using MegaCrit.Sts2.Core.Debug;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.UI;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Map;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Characters;
using MegaCrit.Sts2.Core.Multiplayer;
using MegaCrit.Sts2.Core.Multiplayer.Game;
using MegaCrit.Sts2.Core.Multiplayer.Game.Lobby;
using MegaCrit.Sts2.Core.Multiplayer.Messages.Lobby;
using MegaCrit.Sts2.Core.Nodes.Audio;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.Debug;
using MegaCrit.Sts2.Core.Nodes.Ftue;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Multiplayer;
using MegaCrit.Sts2.Core.Nodes.Screens.MainMenu;
using MegaCrit.Sts2.Core.Nodes.Vfx.Utilities;
using MegaCrit.Sts2.Core.Platform;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.TestSupport;
using MegaCrit.Sts2.Core.Unlocks;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.Nodes.Screens.CharacterSelect;

[ScriptPath("res://src/Core/Nodes/Screens/CharacterSelect/NCharacterSelectScreen.cs")]
public class NCharacterSelectScreen : NSubmenu, IStartRunLobbyListener, ICharacterSelectButtonDelegate
{
	public new class MethodName : NSubmenu.MethodName
	{
		public static readonly StringName Create = StringName.op_Implicit("Create");

		public new static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName InitializeSingleplayer = StringName.op_Implicit("InitializeSingleplayer");

		public static readonly StringName InitCharacterButtons = StringName.op_Implicit("InitCharacterButtons");

		public static readonly StringName UpdateRandomCharacterVisibility = StringName.op_Implicit("UpdateRandomCharacterVisibility");

		public static readonly StringName _Input = StringName.op_Implicit("_Input");

		public static readonly StringName DebugUnlockAllCharacters = StringName.op_Implicit("DebugUnlockAllCharacters");

		public new static readonly StringName OnSubmenuOpened = StringName.op_Implicit("OnSubmenuOpened");

		public new static readonly StringName OnSubmenuClosed = StringName.op_Implicit("OnSubmenuClosed");

		public static readonly StringName OnEmbarkPressed = StringName.op_Implicit("OnEmbarkPressed");

		public static readonly StringName _Process = StringName.op_Implicit("_Process");

		public static readonly StringName CleanUpLobby = StringName.op_Implicit("CleanUpLobby");

		public static readonly StringName RollRandomCharacter = StringName.op_Implicit("RollRandomCharacter");

		public static readonly StringName OnAscensionPanelLevelChanged = StringName.op_Implicit("OnAscensionPanelLevelChanged");

		public static readonly StringName OnUnreadyPressed = StringName.op_Implicit("OnUnreadyPressed");

		public static readonly StringName UpdateRichPresence = StringName.op_Implicit("UpdateRichPresence");

		public static readonly StringName MaxAscensionChanged = StringName.op_Implicit("MaxAscensionChanged");

		public static readonly StringName AscensionChanged = StringName.op_Implicit("AscensionChanged");

		public static readonly StringName SeedChanged = StringName.op_Implicit("SeedChanged");

		public static readonly StringName ModifiersChanged = StringName.op_Implicit("ModifiersChanged");

		public static readonly StringName AfterInitialized = StringName.op_Implicit("AfterInitialized");
	}

	public new class PropertyName : NSubmenu.PropertyName
	{
		public new static readonly StringName InitialFocusedControl = StringName.op_Implicit("InitialFocusedControl");

		public static readonly StringName ShouldShowActDropdown = StringName.op_Implicit("ShouldShowActDropdown");

		public static readonly StringName _name = StringName.op_Implicit("_name");

		public static readonly StringName _infoPanel = StringName.op_Implicit("_infoPanel");

		public static readonly StringName _description = StringName.op_Implicit("_description");

		public static readonly StringName _hp = StringName.op_Implicit("_hp");

		public static readonly StringName _gold = StringName.op_Implicit("_gold");

		public static readonly StringName _relicTitle = StringName.op_Implicit("_relicTitle");

		public static readonly StringName _relicDescription = StringName.op_Implicit("_relicDescription");

		public static readonly StringName _relicIcon = StringName.op_Implicit("_relicIcon");

		public static readonly StringName _relicIconOutline = StringName.op_Implicit("_relicIconOutline");

		public static readonly StringName _selectedButton = StringName.op_Implicit("_selectedButton");

		public static readonly StringName _charButtonContainer = StringName.op_Implicit("_charButtonContainer");

		public static readonly StringName _bgContainer = StringName.op_Implicit("_bgContainer");

		public static readonly StringName _readyAndWaitingContainer = StringName.op_Implicit("_readyAndWaitingContainer");

		public new static readonly StringName _backButton = StringName.op_Implicit("_backButton");

		public static readonly StringName _unreadyButton = StringName.op_Implicit("_unreadyButton");

		public static readonly StringName _embarkButton = StringName.op_Implicit("_embarkButton");

		public static readonly StringName _ascensionPanel = StringName.op_Implicit("_ascensionPanel");

		public static readonly StringName _actDropdown = StringName.op_Implicit("_actDropdown");

		public static readonly StringName _actDropdownLabel = StringName.op_Implicit("_actDropdownLabel");

		public static readonly StringName _remotePlayerContainer = StringName.op_Implicit("_remotePlayerContainer");

		public static readonly StringName _characterUnlockAnimationBackstop = StringName.op_Implicit("_characterUnlockAnimationBackstop");

		public static readonly StringName _randomCharacterButton = StringName.op_Implicit("_randomCharacterButton");

		public static readonly StringName _infoPanelTween = StringName.op_Implicit("_infoPanelTween");

		public static readonly StringName _infoPanelPosFinalVal = StringName.op_Implicit("_infoPanelPosFinalVal");

		public static readonly StringName _charSelectButtonScene = StringName.op_Implicit("_charSelectButtonScene");
	}

	public new class SignalName : NSubmenu.SignalName
	{
	}

	private static readonly string _scenePath = SceneHelper.GetScenePath("screens/character_select_screen");

	private MegaLabel _name;

	private Control _infoPanel;

	private MegaRichTextLabel _description;

	private MegaLabel _hp;

	private MegaLabel _gold;

	private MegaRichTextLabel _relicTitle;

	private MegaRichTextLabel _relicDescription;

	private TextureRect _relicIcon;

	private TextureRect _relicIconOutline;

	private NCharacterSelectButton? _selectedButton;

	private Control _charButtonContainer;

	private Control _bgContainer;

	private Control _readyAndWaitingContainer;

	private NBackButton _backButton;

	private NBackButton _unreadyButton;

	private NConfirmButton _embarkButton;

	private NAscensionPanel _ascensionPanel;

	private NActDropdown _actDropdown;

	private MegaRichTextLabel _actDropdownLabel;

	private NRemoteLobbyPlayerContainer _remotePlayerContainer;

	private Control _characterUnlockAnimationBackstop;

	private NCharacterSelectButton _randomCharacterButton;

	private Tween? _infoPanelTween;

	private Vector2 _infoPanelPosFinalVal;

	private const string _sceneCharSelectButtonPath = "res://scenes/screens/char_select/char_select_button.tscn";

	[Export(/*Could not decode attribute arguments.*/)]
	private PackedScene _charSelectButtonScene;

	private IBootstrapSettings? _settings;

	private StartRunLobby _lobby;

	public StartRunLobby Lobby => _lobby;

	public static IEnumerable<string> AssetPaths
	{
		get
		{
			List<string> list = new List<string>();
			list.Add(_scenePath);
			list.Add("res://scenes/screens/char_select/char_select_button.tscn");
			list.AddRange(NCharacterSelectButton.AssetPaths);
			return new _003C_003Ez__ReadOnlyList<string>(list);
		}
	}

	protected override Control InitialFocusedControl => ((Node)_charButtonContainer).GetChild<Control>(0, false);

	private bool ShouldShowActDropdown => false;

	public static NCharacterSelectScreen? Create()
	{
		if (TestMode.IsOn)
		{
			return null;
		}
		return ResourceLoader.Load<PackedScene>(_scenePath, (string)null, (CacheMode)1).Instantiate<NCharacterSelectScreen>((GenEditState)0);
	}

	public override void _Ready()
	{
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		ConnectSignals();
		_infoPanel = ((Node)this).GetNode<Control>(NodePath.op_Implicit("InfoPanel"));
		_name = ((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("InfoPanel/VBoxContainer/Name"));
		_description = ((Node)this).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("InfoPanel/VBoxContainer/DescriptionLabel"));
		_hp = ((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("InfoPanel/VBoxContainer/HpGoldSpacer/HpGold/Hp/Label"));
		_gold = ((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("InfoPanel/VBoxContainer/HpGoldSpacer/HpGold/Gold/Label"));
		_relicTitle = ((Node)this).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("InfoPanel/VBoxContainer/Relic/Name/RichTextLabel"));
		_relicDescription = ((Node)this).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("InfoPanel/VBoxContainer/Relic/Description"));
		_relicIcon = ((Node)this).GetNode<TextureRect>(NodePath.op_Implicit("InfoPanel/VBoxContainer/Relic/Icon"));
		_relicIconOutline = ((Node)this).GetNode<TextureRect>(NodePath.op_Implicit("InfoPanel/VBoxContainer/Relic/Icon/Outline"));
		_bgContainer = ((Node)this).GetNode<Control>(NodePath.op_Implicit("AnimatedBg"));
		_charButtonContainer = ((Node)this).GetNode<Control>(NodePath.op_Implicit("CharSelectButtons/ButtonContainer"));
		_ascensionPanel = ((Node)this).GetNode<NAscensionPanel>(NodePath.op_Implicit("%AscensionPanel"));
		_actDropdown = ((Node)this).GetNode<NActDropdown>(NodePath.op_Implicit("%ActDropdown"));
		_actDropdownLabel = ((Node)this).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("ActLabel"));
		_remotePlayerContainer = ((Node)this).GetNode<NRemoteLobbyPlayerContainer>(NodePath.op_Implicit("RemotePlayerContainer"));
		_readyAndWaitingContainer = ((Node)this).GetNode<Control>(NodePath.op_Implicit("ReadyAndWaitingPanel"));
		((Node)this).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("%WaitingForPlayers")).Text = new LocString("main_menu_ui", "CHARACTER_SELECT.waitingForPlayers").GetFormattedText();
		_characterUnlockAnimationBackstop = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%CharacterUnlockAnimationBackstop"));
		_backButton = ((Node)this).GetNode<NBackButton>(NodePath.op_Implicit("BackButton"));
		_unreadyButton = ((Node)this).GetNode<NBackButton>(NodePath.op_Implicit("UnreadyButton"));
		_embarkButton = ((Node)this).GetNode<NConfirmButton>(NodePath.op_Implicit("ConfirmButton"));
		_embarkButton.OverrideHotkeys(new string[1] { StringName.op_Implicit(MegaInput.select) });
		((GodotObject)_embarkButton).Connect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)OnEmbarkPressed), 0u);
		((GodotObject)_ascensionPanel).Connect(NAscensionPanel.SignalName.AscensionLevelChanged, Callable.From((Action)OnAscensionPanelLevelChanged), 0u);
		((GodotObject)_unreadyButton).Connect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)OnUnreadyPressed), 0u);
		_unreadyButton.Disable();
		((Node)this).ProcessMode = (ProcessModeEnum)4;
		InitCharacterButtons();
		Type type = BootstrapSettingsUtil.Get();
		if (type != null)
		{
			_settings = (IBootstrapSettings)Activator.CreateInstance(type);
			PreloadManager.Enabled = _settings.DoPreloading;
		}
	}

	public void InitializeMultiplayerAsHost(INetGameService gameService, int maxPlayers)
	{
		if (gameService.Type != NetGameType.Host)
		{
			throw new InvalidOperationException($"Initialized character select screen with GameService of type {gameService.Type} when hosting!");
		}
		_lobby = new StartRunLobby(GameMode.Standard, gameService, this, maxPlayers);
		_ascensionPanel.Initialize(MultiplayerUiMode.Host);
		_lobby.AddLocalHostPlayer(new UnlockState(SaveManager.Instance.Progress), SaveManager.Instance.Progress.MaxMultiplayerAscension);
		AfterInitialized();
	}

	public void InitializeMultiplayerAsClient(INetGameService gameService, ClientLobbyJoinResponseMessage message)
	{
		if (gameService.Type != NetGameType.Client)
		{
			throw new InvalidOperationException($"Initialized character select screen with GameService of type {gameService.Type} when joining!");
		}
		_lobby = new StartRunLobby(GameMode.Standard, gameService, this, -1);
		_ascensionPanel.Initialize(MultiplayerUiMode.Client);
		_lobby.InitializeFromMessage(message);
		AfterInitialized();
	}

	public void InitializeSingleplayer()
	{
		_lobby = new StartRunLobby(GameMode.Standard, new NetSingleplayerGameService(), this, 1);
		_ascensionPanel.Initialize(MultiplayerUiMode.Singleplayer);
		_lobby.AddLocalHostPlayer(new UnlockState(SaveManager.Instance.Progress), 0);
		AfterInitialized();
	}

	private void InitCharacterButtons()
	{
		foreach (CharacterModel allCharacter in ModelDb.AllCharacters)
		{
			NCharacterSelectButton nCharacterSelectButton = _charSelectButtonScene.Instantiate<NCharacterSelectButton>((GenEditState)0);
			((Node)nCharacterSelectButton).Name = StringName.op_Implicit(allCharacter.Id.Entry + "_button");
			((Node)(object)_charButtonContainer).AddChildSafely((Node?)(object)nCharacterSelectButton);
			nCharacterSelectButton.Init(allCharacter, this);
		}
		_randomCharacterButton = _charSelectButtonScene.Instantiate<NCharacterSelectButton>((GenEditState)0);
		((Node)(object)_charButtonContainer).AddChildSafely((Node?)(object)_randomCharacterButton);
		_randomCharacterButton.Init(ModelDb.Character<RandomCharacter>(), this);
		UpdateRandomCharacterVisibility();
	}

	private void UpdateRandomCharacterVisibility()
	{
		if (_lobby == null)
		{
			return;
		}
		bool visible = false;
		foreach (LobbyPlayer player in _lobby.Players)
		{
			UnlockState unlockState = UnlockState.FromSerializable(player.unlockState);
			bool flag = true;
			foreach (CharacterModel allCharacter in ModelDb.AllCharacters)
			{
				if (!unlockState.Characters.Contains(allCharacter))
				{
					flag = false;
					break;
				}
			}
			if (flag)
			{
				visible = true;
				break;
			}
		}
		((CanvasItem)_randomCharacterButton).Visible = visible;
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
			}
			else
			{
				item.UnlockIfPossible();
			}
			item.Reset();
		}
		_embarkButton.Enable();
		if (SaveManager.Instance.Progress.PendingCharacterUnlock == ModelId.none)
		{
			((Node)_charButtonContainer).GetChild<NCharacterSelectButton>(0, false).Select();
		}
		else
		{
			TaskHelper.RunSafely(PlayUnlockCharacterAnimation(SaveManager.Instance.Progress.PendingCharacterUnlock));
		}
		((CanvasItem)_remotePlayerContainer).Visible = _lobby.NetService.Type != NetGameType.Singleplayer;
		_remotePlayerContainer.Initialize(_lobby, displayLocalPlayer: true);
		if (_lobby.NetService.Type == NetGameType.Client)
		{
			_ascensionPanel.SetAscensionLevel(_lobby.Ascension);
		}
		((CanvasItem)_actDropdown).Visible = ShouldShowActDropdown;
		((CanvasItem)_actDropdownLabel).Visible = ((CanvasItem)_actDropdown).Visible;
		((CanvasItem)_readyAndWaitingContainer).Visible = false;
		foreach (LobbyPlayer player in _lobby.Players)
		{
			RefreshButtonSelectionForPlayer(player);
		}
		((Node)this).ProcessMode = (ProcessModeEnum)0;
	}

	private async Task PlayUnlockCharacterAnimation(ModelId character)
	{
		await ((GodotObject)this).ToSignal((GodotObject)(object)((Node)this).GetTree(), SignalName.ProcessFrame);
		_backButton.Disable();
		_embarkButton.Disable();
		((CanvasItem)_infoPanel).Visible = false;
		((CanvasItem)_characterUnlockAnimationBackstop).Visible = true;
		foreach (NCharacterSelectButton button in ((IEnumerable)((Node)_charButtonContainer).GetChildren(false)).OfType<NCharacterSelectButton>())
		{
			if (button.Character.Id == character)
			{
				button.LockForAnimation();
				await Cmd.Wait(0.3f);
				await button.AnimateUnlock();
				button.Select();
			}
		}
		((CanvasItem)_infoPanel).Visible = true;
		((CanvasItem)_characterUnlockAnimationBackstop).Visible = false;
		_backButton.Enable();
		_embarkButton.Enable();
		SaveManager.Instance.Progress.PendingCharacterUnlock = ModelId.none;
	}

	public override void OnSubmenuClosed()
	{
		base.OnSubmenuClosed();
		_embarkButton.Disable();
		_remotePlayerContainer.Cleanup();
		_ascensionPanel.Cleanup();
		if (_lobby.NetService.Type.IsMultiplayer())
		{
			PlatformUtil.SetRichPresence("MAIN_MENU", null, null);
		}
		CleanUpLobby(disconnectSession: true);
	}

	private void OnEmbarkPressed(NButton _)
	{
		_embarkButton.Disable();
		if (!SaveManager.Instance.SeenFtue("accept_tutorials_ftue"))
		{
			NModalContainer.Instance.Add((Node)(object)NAcceptTutorialsFtue.Create(this, delegate
			{
				OnEmbarkPressed(null);
			}));
			return;
		}
		NetGameType type = _lobby.NetService.Type;
		if ((uint)(type - 1) <= 1u)
		{
			_lobby.Act1 = _actDropdown.CurrentOption;
		}
		_lobby.SetReady(ready: true);
		foreach (NCharacterSelectButton item in ((IEnumerable)((Node)_charButtonContainer).GetChildren(false)).OfType<NCharacterSelectButton>())
		{
			item.Disable();
		}
		_backButton.Disable();
		if (_lobby.NetService.Type.IsMultiplayer() && !_lobby.IsAboutToBeginGame())
		{
			((CanvasItem)_readyAndWaitingContainer).Visible = true;
			_unreadyButton.Enable();
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

	private async Task StartNewSingleplayerRun(string seed, List<ActModel> acts)
	{
		Log.Info($"Embarking on a singleplayer {_lobby.LocalPlayer.character.Id.Entry} run. Ascension: {_lobby.Ascension} Seed: {seed}");
		int ascensionToEmbark = _lobby.Ascension;
		if (_lobby.LocalPlayer.character is RandomCharacter)
		{
			RollRandomCharacter();
			CharacterModel character = _lobby.LocalPlayer.character;
			int maxAscension = SaveManager.Instance.Progress.GetOrCreateCharacterStats(_lobby.LocalPlayer.character.Id).MaxAscension;
			ascensionToEmbark = Math.Min(maxAscension, ascensionToEmbark);
			NGame.Instance?.ScreenShake(ShakeStrength.Weak, ShakeDuration.Short, 90f);
			SfxCmd.Play(character.CharacterSelectSfx);
			Control val = PreloadManager.Cache.GetScene(character.CharacterSelectBg).Instantiate<Control>((GenEditState)0);
			((Node)val).Name = StringName.op_Implicit(character.Id.Entry + "_bg");
			((Node)(object)_bgContainer).AddChildSafely((Node?)(object)val);
			if (ascensionToEmbark < maxAscension)
			{
				_ascensionPanel.SetAscensionLevel(ascensionToEmbark);
			}
			await Task.Delay(1000);
		}
		SfxCmd.Play(_lobby.LocalPlayer.character.CharacterTransitionSfx);
		await NGame.Instance.Transition.FadeOut(0.8f, _lobby.LocalPlayer.character.CharacterSelectTransitionPath);
		await NGame.Instance.StartNewSingleplayerRun(_lobby.LocalPlayer.character, shouldSave: true, acts, Array.Empty<ModifierModel>(), seed, ascensionToEmbark);
		CleanUpLobby(disconnectSession: false);
	}

	private async Task StartNewMultiplayerRun(string seed, List<ActModel> acts)
	{
		Log.Info($"Embarking on a multiplayer run. Players: {string.Join(",", _lobby.Players)}. Ascension: {_lobby.Ascension} Seed: {seed}");
		if (_lobby.LocalPlayer.character is RandomCharacter)
		{
			RollRandomCharacter();
		}
		SfxCmd.Play(_lobby.LocalPlayer.character.CharacterTransitionSfx);
		await NGame.Instance.Transition.FadeOut(0.8f, _lobby.LocalPlayer.character.CharacterSelectTransitionPath);
		IBootstrapSettings settings = _settings;
		if (settings != null && settings.BootstrapInMultiplayer)
		{
			using (new NetLoadingHandle(_lobby.NetService))
			{
				acts[0] = _settings.Act;
				RunState runState = RunState.CreateForNewRun(_lobby.Players.Select((LobbyPlayer p) => Player.CreateForNewRun(p.character, UnlockState.FromSerializable(p.unlockState), p.id)).ToList(), acts.Select((ActModel a) => a.ToMutable()).ToList(), _settings.Modifiers, _lobby.Ascension, seed);
				RunManager.Instance.SetUpNewMultiPlayer(runState, _lobby, _settings.SaveRunHistory);
				await PreloadManager.LoadRunAssets(runState.Players.Select((Player p) => p.Character));
				await RunManager.Instance.FinalizeStartingRelics();
				RunManager.Instance.Launch();
				NGame.Instance.RootSceneContainer.SetCurrentScene((Control)(object)NRun.Create(runState));
				await RunManager.Instance.SetActInternal(0);
				await SaveManager.Instance.SaveRun(null);
				CleanUpLobby(disconnectSession: false);
				await _settings.Setup(LocalContext.GetMe(runState));
				switch (_settings.RoomType)
				{
				case RoomType.Unassigned:
					await RunManager.Instance.EnterAct(0);
					break;
				case RoomType.Treasure:
				case RoomType.Shop:
				case RoomType.RestSite:
					await RunManager.Instance.EnterRoomDebug(_settings.RoomType, MapPointType.Unassigned, null, showTransition: false);
					RunManager.Instance.ActionExecutor.Unpause();
					break;
				case RoomType.Event:
					await RunManager.Instance.EnterRoomDebug(_settings.RoomType, MapPointType.Unassigned, _settings.Event, showTransition: false);
					break;
				default:
					await RunManager.Instance.EnterRoomDebug(_settings.RoomType, MapPointType.Unassigned, _settings.RoomType.IsCombatRoom() ? _settings.Encounter.ToMutable() : null, showTransition: false);
					break;
				}
			}
		}
		else
		{
			await NGame.Instance.StartNewMultiplayerRun(_lobby, shouldSave: true, acts, Array.Empty<ModifierModel>(), seed, _lobby.Ascension);
			CleanUpLobby(disconnectSession: false);
		}
	}

	private void RollRandomCharacter()
	{
		CharacterModel[] items = ModelDb.AllCharacters.ToArray();
		_lobby.SetLocalCharacter(Rng.Chaotic.NextItem(items));
	}

	public void SelectCharacter(NCharacterSelectButton charSelectButton, CharacterModel characterModel)
	{
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0469: Unknown result type (might be due to invalid IL or missing references)
		//IL_0479: Unknown result type (might be due to invalid IL or missing references)
		//IL_0447: Unknown result type (might be due to invalid IL or missing references)
		//IL_0457: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
		if (!charSelectButton.IsRandom)
		{
			SfxCmd.Play(characterModel.CharacterSelectSfx);
		}
		NGame.Instance?.ScreenShake(ShakeStrength.Weak, ShakeDuration.Short, 90f);
		if (_infoPanelTween != null)
		{
			_infoPanel.Position = _infoPanelPosFinalVal;
		}
		_infoPanelPosFinalVal = _infoPanel.Position;
		Tween? infoPanelTween = _infoPanelTween;
		if (infoPanelTween != null)
		{
			infoPanelTween.Kill();
		}
		_infoPanelTween = ((Node)this).CreateTween().SetParallel(true);
		_infoPanelTween.TweenProperty((GodotObject)(object)_infoPanel, NodePath.op_Implicit("position"), Variant.op_Implicit(_infoPanel.Position), 0.5).SetEase((EaseType)1).SetTrans((TransitionType)5)
			.From(Variant.op_Implicit(_infoPanel.Position - new Vector2(300f, 0f)));
		foreach (Node child in ((Node)_bgContainer).GetChildren(false))
		{
			((Node)(object)_bgContainer).RemoveChildSafely(child);
			child.QueueFreeSafely();
		}
		_selectedButton = charSelectButton;
		if (!charSelectButton.IsLocked)
		{
			_embarkButton.Enable();
			Control val = PreloadManager.Cache.GetScene(characterModel.CharacterSelectBg).Instantiate<Control>((GenEditState)0);
			((Node)val).Name = StringName.op_Implicit(characterModel.Id.Entry + "_bg");
			((Node)(object)_bgContainer).AddChildSafely((Node?)(object)val);
			string formattedText = new LocString("characters", characterModel.CharacterSelectTitle).GetFormattedText();
			_name.SetTextAutoSize(formattedText);
			_description.Text = new LocString("characters", characterModel.CharacterSelectDesc).GetFormattedText();
			if (!_selectedButton.IsRandom)
			{
				_hp.SetTextAutoSize($"{characterModel.StartingHp}/{characterModel.StartingHp}");
				_gold.SetTextAutoSize($"{characterModel.StartingGold}");
				RelicModel relicModel = characterModel.StartingRelics[0];
				_relicTitle.Text = relicModel.Title.GetFormattedText();
				_relicDescription.Text = relicModel.DynamicDescription.GetFormattedText();
				_relicIcon.Texture = relicModel.Icon;
				_relicIconOutline.Texture = relicModel.IconOutline;
				((CanvasItem)_relicIcon).SelfModulate = Colors.White;
				((CanvasItem)_relicIconOutline).SelfModulate = StsColors.halfTransparentBlack;
			}
			else
			{
				_hp.SetTextAutoSize("??/??");
				_gold.SetTextAutoSize("???");
				((CanvasItem)_relicIcon).SelfModulate = StsColors.transparentBlack;
				((CanvasItem)_relicIconOutline).SelfModulate = StsColors.transparentBlack;
				_relicTitle.Text = string.Empty;
				_relicDescription.Text = string.Empty;
			}
			_lobby.SetLocalCharacter(characterModel);
			if (!_lobby.NetService.Type.IsMultiplayer())
			{
				_ascensionPanel.AnimIn();
			}
		}
		else
		{
			_embarkButton.Disable();
			string formattedText2 = new LocString("main_menu_ui", "CHARACTER_SELECT.locked.title").GetFormattedText();
			_name.SetTextAutoSize(formattedText2);
			_description.Text = characterModel.GetUnlockText().GetFormattedText();
			_hp.SetTextAutoSize("??/??");
			_gold.SetTextAutoSize("???");
			if (!_selectedButton.IsRandom)
			{
				RelicModel relicModel2 = characterModel.StartingRelics[0];
				_relicTitle.Text = new LocString("main_menu_ui", "CHARACTER_SELECT.lockedRelic.title").GetFormattedText();
				_relicDescription.Text = new LocString("main_menu_ui", "CHARACTER_SELECT.lockedRelic.description").GetFormattedText();
				_relicIcon.Texture = relicModel2.Icon;
				_relicIconOutline.Texture = relicModel2.IconOutline;
				((CanvasItem)_relicIcon).SelfModulate = StsColors.ninetyPercentBlack;
				((CanvasItem)_relicIconOutline).SelfModulate = StsColors.halfTransparentWhite;
			}
			else
			{
				((CanvasItem)_relicIcon).SelfModulate = StsColors.transparentBlack;
				((CanvasItem)_relicIconOutline).SelfModulate = StsColors.transparentBlack;
				_relicTitle.Text = string.Empty;
				_relicDescription.Text = string.Empty;
			}
			((CanvasItem)_ascensionPanel).Visible = false;
		}
		foreach (NCharacterSelectButton item in ((IEnumerable)((Node)_charButtonContainer).GetChildren(false)).OfType<NCharacterSelectButton>())
		{
			if (item != _selectedButton)
			{
				item.Deselect();
			}
		}
	}

	private void OnAscensionPanelLevelChanged()
	{
		if (_lobby.NetService.Type != NetGameType.Client && _lobby.Ascension != _ascensionPanel.Ascension)
		{
			_lobby.SyncAscensionChange(_ascensionPanel.Ascension);
		}
	}

	private void OnUnreadyPressed(NButton _)
	{
		_lobby.SetReady(ready: false);
		foreach (NCharacterSelectButton item in ((IEnumerable)((Node)_charButtonContainer).GetChildren(false)).OfType<NCharacterSelectButton>())
		{
			item.Enable();
		}
		((CanvasItem)_readyAndWaitingContainer).Visible = false;
		_embarkButton.Enable();
		_backButton.Enable();
		_unreadyButton.Disable();
	}

	private void UpdateRichPresence()
	{
		if (_lobby.NetService.Type.IsMultiplayer())
		{
			PlatformUtil.SetRichPresence("STANDARD_MP_LOBBY", _lobby.NetService.GetRawLobbyIdentifier(), _lobby.Players.Count);
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
		UpdateRandomCharacterVisibility();
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
		throw new NotImplementedException("Seed should not be changed in standard mode!");
	}

	public void ModifiersChanged()
	{
		throw new NotImplementedException("Modifiers should not be changed in standard mode!");
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
		UpdateRandomCharacterVisibility();
	}

	public void BeginRun(string seed, List<ActModel> acts, IReadOnlyList<ModifierModel> modifiers)
	{
		if (modifiers.Count > 0)
		{
			Log.Error("Modifiers list is not empty while starting a standard run, ignoring!");
		}
		NAudioManager.Instance?.StopMusic();
		_ascensionPanel.Cleanup();
		if (_lobby.NetService.Type == NetGameType.Singleplayer)
		{
			TaskHelper.RunSafely(StartNewSingleplayerRun(seed, acts));
		}
		else
		{
			TaskHelper.RunSafely(StartNewMultiplayerRun(seed, acts));
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
		UpdateRandomCharacterVisibility();
		Logger.logLevelTypeMap[LogType.Network] = ((_lobby.NetService.Type == NetGameType.Singleplayer) ? LogLevel.Info : LogLevel.Debug);
		Logger.logLevelTypeMap[LogType.Actions] = ((_lobby.NetService.Type == NetGameType.Singleplayer) ? LogLevel.Info : LogLevel.VeryDebug);
		Logger.logLevelTypeMap[LogType.GameSync] = ((_lobby.NetService.Type == NetGameType.Singleplayer) ? LogLevel.Info : LogLevel.VeryDebug);
		if (_lobby.NetService.Type != NetGameType.Singleplayer)
		{
			IBootstrapSettings? settings = _settings;
			if (settings != null && settings.BootstrapInMultiplayer)
			{
				NGame.Instance.DebugSeedOverride = _settings.Seed;
				return;
			}
		}
		NGame.Instance.DebugSeedOverride = null;
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
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Expected O, but got Unknown
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Expected O, but got Unknown
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_0289: Unknown result type (might be due to invalid IL or missing references)
		//IL_0294: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_030e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0317: Unknown result type (might be due to invalid IL or missing references)
		//IL_033d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0346: Unknown result type (might be due to invalid IL or missing references)
		//IL_036c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0394: Unknown result type (might be due to invalid IL or missing references)
		//IL_039f: Expected O, but got Unknown
		//IL_039a: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0403: Unknown result type (might be due to invalid IL or missing references)
		//IL_0429: Unknown result type (might be due to invalid IL or missing references)
		//IL_0432: Unknown result type (might be due to invalid IL or missing references)
		//IL_0458: Unknown result type (might be due to invalid IL or missing references)
		//IL_0461: Unknown result type (might be due to invalid IL or missing references)
		//IL_0487: Unknown result type (might be due to invalid IL or missing references)
		//IL_0490: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bf: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(21);
		list.Add(new MethodInfo(MethodName.Create, new PropertyInfo((Type)24, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false), (MethodFlags)33, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.InitializeSingleplayer, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.InitCharacterButtons, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UpdateRandomCharacterVisibility, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
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
		list.Add(new MethodInfo(MethodName._Process, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)3, StringName.op_Implicit("delta"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.CleanUpLobby, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)1, StringName.op_Implicit("disconnectSession"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.RollRandomCharacter, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnAscensionPanelLevelChanged, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnUnreadyPressed, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UpdateRichPresence, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.MaxAscensionChanged, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AscensionChanged, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SeedChanged, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ModifiersChanged, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AfterInitialized, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
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
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0294: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02de: Unknown result type (might be due to invalid IL or missing references)
		//IL_0303: Unknown result type (might be due to invalid IL or missing references)
		//IL_0357: Unknown result type (might be due to invalid IL or missing references)
		//IL_0328: Unknown result type (might be due to invalid IL or missing references)
		//IL_034d: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.Create && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			NCharacterSelectScreen nCharacterSelectScreen = Create();
			ret = VariantUtils.CreateFrom<NCharacterSelectScreen>(ref nCharacterSelectScreen);
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
		if ((ref method) == MethodName.InitCharacterButtons && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			InitCharacterButtons();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.UpdateRandomCharacterVisibility && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			UpdateRandomCharacterVisibility();
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
		if ((ref method) == MethodName.RollRandomCharacter && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			RollRandomCharacter();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnAscensionPanelLevelChanged && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnAscensionPanelLevelChanged();
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
			NCharacterSelectScreen nCharacterSelectScreen = Create();
			ret = VariantUtils.CreateFrom<NCharacterSelectScreen>(ref nCharacterSelectScreen);
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
		if ((ref method) == MethodName.InitCharacterButtons)
		{
			return true;
		}
		if ((ref method) == MethodName.UpdateRandomCharacterVisibility)
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
		if ((ref method) == MethodName._Process)
		{
			return true;
		}
		if ((ref method) == MethodName.CleanUpLobby)
		{
			return true;
		}
		if ((ref method) == MethodName.RollRandomCharacter)
		{
			return true;
		}
		if ((ref method) == MethodName.OnAscensionPanelLevelChanged)
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
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0281: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName._name)
		{
			_name = VariantUtils.ConvertTo<MegaLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._infoPanel)
		{
			_infoPanel = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._description)
		{
			_description = VariantUtils.ConvertTo<MegaRichTextLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._hp)
		{
			_hp = VariantUtils.ConvertTo<MegaLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._gold)
		{
			_gold = VariantUtils.ConvertTo<MegaLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._relicTitle)
		{
			_relicTitle = VariantUtils.ConvertTo<MegaRichTextLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._relicDescription)
		{
			_relicDescription = VariantUtils.ConvertTo<MegaRichTextLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._relicIcon)
		{
			_relicIcon = VariantUtils.ConvertTo<TextureRect>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._relicIconOutline)
		{
			_relicIconOutline = VariantUtils.ConvertTo<TextureRect>(ref value);
			return true;
		}
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
		if ((ref name) == PropertyName._bgContainer)
		{
			_bgContainer = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._readyAndWaitingContainer)
		{
			_readyAndWaitingContainer = VariantUtils.ConvertTo<Control>(ref value);
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
		if ((ref name) == PropertyName._embarkButton)
		{
			_embarkButton = VariantUtils.ConvertTo<NConfirmButton>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._ascensionPanel)
		{
			_ascensionPanel = VariantUtils.ConvertTo<NAscensionPanel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._actDropdown)
		{
			_actDropdown = VariantUtils.ConvertTo<NActDropdown>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._actDropdownLabel)
		{
			_actDropdownLabel = VariantUtils.ConvertTo<MegaRichTextLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._remotePlayerContainer)
		{
			_remotePlayerContainer = VariantUtils.ConvertTo<NRemoteLobbyPlayerContainer>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._characterUnlockAnimationBackstop)
		{
			_characterUnlockAnimationBackstop = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._randomCharacterButton)
		{
			_randomCharacterButton = VariantUtils.ConvertTo<NCharacterSelectButton>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._infoPanelTween)
		{
			_infoPanelTween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._infoPanelPosFinalVal)
		{
			_infoPanelPosFinalVal = VariantUtils.ConvertTo<Vector2>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._charSelectButtonScene)
		{
			_charSelectButtonScene = VariantUtils.ConvertTo<PackedScene>(ref value);
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
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		//IL_029a: Unknown result type (might be due to invalid IL or missing references)
		//IL_029f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02da: Unknown result type (might be due to invalid IL or missing references)
		//IL_02df: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_031a: Unknown result type (might be due to invalid IL or missing references)
		//IL_031f: Unknown result type (might be due to invalid IL or missing references)
		//IL_033a: Unknown result type (might be due to invalid IL or missing references)
		//IL_033f: Unknown result type (might be due to invalid IL or missing references)
		//IL_035a: Unknown result type (might be due to invalid IL or missing references)
		//IL_035f: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName.InitialFocusedControl)
		{
			Control initialFocusedControl = InitialFocusedControl;
			value = VariantUtils.CreateFrom<Control>(ref initialFocusedControl);
			return true;
		}
		if ((ref name) == PropertyName.ShouldShowActDropdown)
		{
			bool shouldShowActDropdown = ShouldShowActDropdown;
			value = VariantUtils.CreateFrom<bool>(ref shouldShowActDropdown);
			return true;
		}
		if ((ref name) == PropertyName._name)
		{
			value = VariantUtils.CreateFrom<MegaLabel>(ref _name);
			return true;
		}
		if ((ref name) == PropertyName._infoPanel)
		{
			value = VariantUtils.CreateFrom<Control>(ref _infoPanel);
			return true;
		}
		if ((ref name) == PropertyName._description)
		{
			value = VariantUtils.CreateFrom<MegaRichTextLabel>(ref _description);
			return true;
		}
		if ((ref name) == PropertyName._hp)
		{
			value = VariantUtils.CreateFrom<MegaLabel>(ref _hp);
			return true;
		}
		if ((ref name) == PropertyName._gold)
		{
			value = VariantUtils.CreateFrom<MegaLabel>(ref _gold);
			return true;
		}
		if ((ref name) == PropertyName._relicTitle)
		{
			value = VariantUtils.CreateFrom<MegaRichTextLabel>(ref _relicTitle);
			return true;
		}
		if ((ref name) == PropertyName._relicDescription)
		{
			value = VariantUtils.CreateFrom<MegaRichTextLabel>(ref _relicDescription);
			return true;
		}
		if ((ref name) == PropertyName._relicIcon)
		{
			value = VariantUtils.CreateFrom<TextureRect>(ref _relicIcon);
			return true;
		}
		if ((ref name) == PropertyName._relicIconOutline)
		{
			value = VariantUtils.CreateFrom<TextureRect>(ref _relicIconOutline);
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
		if ((ref name) == PropertyName._bgContainer)
		{
			value = VariantUtils.CreateFrom<Control>(ref _bgContainer);
			return true;
		}
		if ((ref name) == PropertyName._readyAndWaitingContainer)
		{
			value = VariantUtils.CreateFrom<Control>(ref _readyAndWaitingContainer);
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
		if ((ref name) == PropertyName._embarkButton)
		{
			value = VariantUtils.CreateFrom<NConfirmButton>(ref _embarkButton);
			return true;
		}
		if ((ref name) == PropertyName._ascensionPanel)
		{
			value = VariantUtils.CreateFrom<NAscensionPanel>(ref _ascensionPanel);
			return true;
		}
		if ((ref name) == PropertyName._actDropdown)
		{
			value = VariantUtils.CreateFrom<NActDropdown>(ref _actDropdown);
			return true;
		}
		if ((ref name) == PropertyName._actDropdownLabel)
		{
			value = VariantUtils.CreateFrom<MegaRichTextLabel>(ref _actDropdownLabel);
			return true;
		}
		if ((ref name) == PropertyName._remotePlayerContainer)
		{
			value = VariantUtils.CreateFrom<NRemoteLobbyPlayerContainer>(ref _remotePlayerContainer);
			return true;
		}
		if ((ref name) == PropertyName._characterUnlockAnimationBackstop)
		{
			value = VariantUtils.CreateFrom<Control>(ref _characterUnlockAnimationBackstop);
			return true;
		}
		if ((ref name) == PropertyName._randomCharacterButton)
		{
			value = VariantUtils.CreateFrom<NCharacterSelectButton>(ref _randomCharacterButton);
			return true;
		}
		if ((ref name) == PropertyName._infoPanelTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _infoPanelTween);
			return true;
		}
		if ((ref name) == PropertyName._infoPanelPosFinalVal)
		{
			value = VariantUtils.CreateFrom<Vector2>(ref _infoPanelPosFinalVal);
			return true;
		}
		if ((ref name) == PropertyName._charSelectButtonScene)
		{
			value = VariantUtils.CreateFrom<PackedScene>(ref _charSelectButtonScene);
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
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0290: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0313: Unknown result type (might be due to invalid IL or missing references)
		//IL_0335: Unknown result type (might be due to invalid IL or missing references)
		//IL_0356: Unknown result type (might be due to invalid IL or missing references)
		//IL_0376: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._name, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._infoPanel, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._description, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._hp, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._gold, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._relicTitle, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._relicDescription, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._relicIcon, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._relicIconOutline, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._selectedButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._charButtonContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._bgContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._readyAndWaitingContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._backButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._unreadyButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._embarkButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._ascensionPanel, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._actDropdown, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._actDropdownLabel, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._remotePlayerContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._characterUnlockAnimationBackstop, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._randomCharacterButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._infoPanelTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)5, PropertyName._infoPanelPosFinalVal, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._charSelectButtonScene, (PropertyHint)17, "PackedScene", (PropertyUsageFlags)4102, true));
		list.Add(new PropertyInfo((Type)24, PropertyName.InitialFocusedControl, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName.ShouldShowActDropdown, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName._name, Variant.From<MegaLabel>(ref _name));
		info.AddProperty(PropertyName._infoPanel, Variant.From<Control>(ref _infoPanel));
		info.AddProperty(PropertyName._description, Variant.From<MegaRichTextLabel>(ref _description));
		info.AddProperty(PropertyName._hp, Variant.From<MegaLabel>(ref _hp));
		info.AddProperty(PropertyName._gold, Variant.From<MegaLabel>(ref _gold));
		info.AddProperty(PropertyName._relicTitle, Variant.From<MegaRichTextLabel>(ref _relicTitle));
		info.AddProperty(PropertyName._relicDescription, Variant.From<MegaRichTextLabel>(ref _relicDescription));
		info.AddProperty(PropertyName._relicIcon, Variant.From<TextureRect>(ref _relicIcon));
		info.AddProperty(PropertyName._relicIconOutline, Variant.From<TextureRect>(ref _relicIconOutline));
		info.AddProperty(PropertyName._selectedButton, Variant.From<NCharacterSelectButton>(ref _selectedButton));
		info.AddProperty(PropertyName._charButtonContainer, Variant.From<Control>(ref _charButtonContainer));
		info.AddProperty(PropertyName._bgContainer, Variant.From<Control>(ref _bgContainer));
		info.AddProperty(PropertyName._readyAndWaitingContainer, Variant.From<Control>(ref _readyAndWaitingContainer));
		info.AddProperty(PropertyName._backButton, Variant.From<NBackButton>(ref _backButton));
		info.AddProperty(PropertyName._unreadyButton, Variant.From<NBackButton>(ref _unreadyButton));
		info.AddProperty(PropertyName._embarkButton, Variant.From<NConfirmButton>(ref _embarkButton));
		info.AddProperty(PropertyName._ascensionPanel, Variant.From<NAscensionPanel>(ref _ascensionPanel));
		info.AddProperty(PropertyName._actDropdown, Variant.From<NActDropdown>(ref _actDropdown));
		info.AddProperty(PropertyName._actDropdownLabel, Variant.From<MegaRichTextLabel>(ref _actDropdownLabel));
		info.AddProperty(PropertyName._remotePlayerContainer, Variant.From<NRemoteLobbyPlayerContainer>(ref _remotePlayerContainer));
		info.AddProperty(PropertyName._characterUnlockAnimationBackstop, Variant.From<Control>(ref _characterUnlockAnimationBackstop));
		info.AddProperty(PropertyName._randomCharacterButton, Variant.From<NCharacterSelectButton>(ref _randomCharacterButton));
		info.AddProperty(PropertyName._infoPanelTween, Variant.From<Tween>(ref _infoPanelTween));
		info.AddProperty(PropertyName._infoPanelPosFinalVal, Variant.From<Vector2>(ref _infoPanelPosFinalVal));
		info.AddProperty(PropertyName._charSelectButtonScene, Variant.From<PackedScene>(ref _charSelectButtonScene));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		//IL_029d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
		base.RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._name, ref val))
		{
			_name = ((Variant)(ref val)).As<MegaLabel>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._infoPanel, ref val2))
		{
			_infoPanel = ((Variant)(ref val2)).As<Control>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._description, ref val3))
		{
			_description = ((Variant)(ref val3)).As<MegaRichTextLabel>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._hp, ref val4))
		{
			_hp = ((Variant)(ref val4)).As<MegaLabel>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._gold, ref val5))
		{
			_gold = ((Variant)(ref val5)).As<MegaLabel>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._relicTitle, ref val6))
		{
			_relicTitle = ((Variant)(ref val6)).As<MegaRichTextLabel>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName._relicDescription, ref val7))
		{
			_relicDescription = ((Variant)(ref val7)).As<MegaRichTextLabel>();
		}
		Variant val8 = default(Variant);
		if (info.TryGetProperty(PropertyName._relicIcon, ref val8))
		{
			_relicIcon = ((Variant)(ref val8)).As<TextureRect>();
		}
		Variant val9 = default(Variant);
		if (info.TryGetProperty(PropertyName._relicIconOutline, ref val9))
		{
			_relicIconOutline = ((Variant)(ref val9)).As<TextureRect>();
		}
		Variant val10 = default(Variant);
		if (info.TryGetProperty(PropertyName._selectedButton, ref val10))
		{
			_selectedButton = ((Variant)(ref val10)).As<NCharacterSelectButton>();
		}
		Variant val11 = default(Variant);
		if (info.TryGetProperty(PropertyName._charButtonContainer, ref val11))
		{
			_charButtonContainer = ((Variant)(ref val11)).As<Control>();
		}
		Variant val12 = default(Variant);
		if (info.TryGetProperty(PropertyName._bgContainer, ref val12))
		{
			_bgContainer = ((Variant)(ref val12)).As<Control>();
		}
		Variant val13 = default(Variant);
		if (info.TryGetProperty(PropertyName._readyAndWaitingContainer, ref val13))
		{
			_readyAndWaitingContainer = ((Variant)(ref val13)).As<Control>();
		}
		Variant val14 = default(Variant);
		if (info.TryGetProperty(PropertyName._backButton, ref val14))
		{
			_backButton = ((Variant)(ref val14)).As<NBackButton>();
		}
		Variant val15 = default(Variant);
		if (info.TryGetProperty(PropertyName._unreadyButton, ref val15))
		{
			_unreadyButton = ((Variant)(ref val15)).As<NBackButton>();
		}
		Variant val16 = default(Variant);
		if (info.TryGetProperty(PropertyName._embarkButton, ref val16))
		{
			_embarkButton = ((Variant)(ref val16)).As<NConfirmButton>();
		}
		Variant val17 = default(Variant);
		if (info.TryGetProperty(PropertyName._ascensionPanel, ref val17))
		{
			_ascensionPanel = ((Variant)(ref val17)).As<NAscensionPanel>();
		}
		Variant val18 = default(Variant);
		if (info.TryGetProperty(PropertyName._actDropdown, ref val18))
		{
			_actDropdown = ((Variant)(ref val18)).As<NActDropdown>();
		}
		Variant val19 = default(Variant);
		if (info.TryGetProperty(PropertyName._actDropdownLabel, ref val19))
		{
			_actDropdownLabel = ((Variant)(ref val19)).As<MegaRichTextLabel>();
		}
		Variant val20 = default(Variant);
		if (info.TryGetProperty(PropertyName._remotePlayerContainer, ref val20))
		{
			_remotePlayerContainer = ((Variant)(ref val20)).As<NRemoteLobbyPlayerContainer>();
		}
		Variant val21 = default(Variant);
		if (info.TryGetProperty(PropertyName._characterUnlockAnimationBackstop, ref val21))
		{
			_characterUnlockAnimationBackstop = ((Variant)(ref val21)).As<Control>();
		}
		Variant val22 = default(Variant);
		if (info.TryGetProperty(PropertyName._randomCharacterButton, ref val22))
		{
			_randomCharacterButton = ((Variant)(ref val22)).As<NCharacterSelectButton>();
		}
		Variant val23 = default(Variant);
		if (info.TryGetProperty(PropertyName._infoPanelTween, ref val23))
		{
			_infoPanelTween = ((Variant)(ref val23)).As<Tween>();
		}
		Variant val24 = default(Variant);
		if (info.TryGetProperty(PropertyName._infoPanelPosFinalVal, ref val24))
		{
			_infoPanelPosFinalVal = ((Variant)(ref val24)).As<Vector2>();
		}
		Variant val25 = default(Variant);
		if (info.TryGetProperty(PropertyName._charSelectButtonScene, ref val25))
		{
			_charSelectButtonScene = ((Variant)(ref val25)).As<PackedScene>();
		}
	}
}
