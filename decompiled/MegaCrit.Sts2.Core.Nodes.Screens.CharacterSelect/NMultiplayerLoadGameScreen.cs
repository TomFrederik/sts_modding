using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Entities.UI;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Multiplayer.Game;
using MegaCrit.Sts2.Core.Multiplayer.Game.Lobby;
using MegaCrit.Sts2.Core.Multiplayer.Messages.Lobby;
using MegaCrit.Sts2.Core.Nodes.Audio;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Multiplayer;
using MegaCrit.Sts2.Core.Nodes.Screens.MainMenu;
using MegaCrit.Sts2.Core.Platform;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.TestSupport;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.Nodes.Screens.CharacterSelect;

[ScriptPath("res://src/Core/Nodes/Screens/CharacterSelect/NMultiplayerLoadGameScreen.cs")]
public class NMultiplayerLoadGameScreen : NSubmenu, ILoadRunLobbyListener
{
	public new class MethodName : NSubmenu.MethodName
	{
		public static readonly StringName Create = StringName.op_Implicit("Create");

		public new static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public new static readonly StringName OnSubmenuOpened = StringName.op_Implicit("OnSubmenuOpened");

		public new static readonly StringName OnSubmenuShown = StringName.op_Implicit("OnSubmenuShown");

		public new static readonly StringName OnSubmenuClosed = StringName.op_Implicit("OnSubmenuClosed");

		public new static readonly StringName OnSubmenuHidden = StringName.op_Implicit("OnSubmenuHidden");

		public static readonly StringName OnEmbarkPressed = StringName.op_Implicit("OnEmbarkPressed");

		public static readonly StringName OnUnreadyPressed = StringName.op_Implicit("OnUnreadyPressed");

		public static readonly StringName UpdateRichPresence = StringName.op_Implicit("UpdateRichPresence");

		public static readonly StringName _Process = StringName.op_Implicit("_Process");

		public static readonly StringName CleanUpLobby = StringName.op_Implicit("CleanUpLobby");

		public static readonly StringName PlayerConnected = StringName.op_Implicit("PlayerConnected");

		public static readonly StringName PlayerReadyChanged = StringName.op_Implicit("PlayerReadyChanged");

		public static readonly StringName RemotePlayerDisconnected = StringName.op_Implicit("RemotePlayerDisconnected");

		public static readonly StringName BeginRun = StringName.op_Implicit("BeginRun");

		public static readonly StringName AfterMultiplayerStarted = StringName.op_Implicit("AfterMultiplayerStarted");
	}

	public new class PropertyName : NSubmenu.PropertyName
	{
		public new static readonly StringName InitialFocusedControl = StringName.op_Implicit("InitialFocusedControl");

		public static readonly StringName _name = StringName.op_Implicit("_name");

		public static readonly StringName _infoPanel = StringName.op_Implicit("_infoPanel");

		public static readonly StringName _hp = StringName.op_Implicit("_hp");

		public static readonly StringName _gold = StringName.op_Implicit("_gold");

		public static readonly StringName _selectedButton = StringName.op_Implicit("_selectedButton");

		public static readonly StringName _bgContainer = StringName.op_Implicit("_bgContainer");

		public static readonly StringName _confirmButton = StringName.op_Implicit("_confirmButton");

		public new static readonly StringName _backButton = StringName.op_Implicit("_backButton");

		public static readonly StringName _unreadyButton = StringName.op_Implicit("_unreadyButton");

		public static readonly StringName _ascensionPanel = StringName.op_Implicit("_ascensionPanel");

		public static readonly StringName _floorLabel = StringName.op_Implicit("_floorLabel");

		public static readonly StringName _actLabel = StringName.op_Implicit("_actLabel");

		public static readonly StringName _remotePlayerContainer = StringName.op_Implicit("_remotePlayerContainer");

		public static readonly StringName _infoPanelTween = StringName.op_Implicit("_infoPanelTween");

		public static readonly StringName _infoPanelPosFinalVal = StringName.op_Implicit("_infoPanelPosFinalVal");
	}

	public new class SignalName : NSubmenu.SignalName
	{
	}

	private static readonly string _scenePath = SceneHelper.GetScenePath("screens/multiplayer_load_game_screen");

	private MegaLabel _name;

	private Control _infoPanel;

	private MegaLabel _hp;

	private MegaLabel _gold;

	private NCharacterSelectButton? _selectedButton;

	private Control _bgContainer;

	private NConfirmButton _confirmButton;

	private NBackButton _backButton;

	private NBackButton _unreadyButton;

	private NAscensionPanel _ascensionPanel;

	private MegaRichTextLabel _floorLabel;

	private MegaRichTextLabel _actLabel;

	private NRemoteLoadLobbyPlayerContainer _remotePlayerContainer;

	private Tween? _infoPanelTween;

	private Vector2 _infoPanelPosFinalVal;

	private const string _sceneCharSelectButtonPath = "res://scenes/screens/char_select/char_select_button.tscn";

	private LoadRunLobby _runLobby;

	public static IEnumerable<string> AssetPaths => new global::_003C_003Ez__ReadOnlyArray<string>(new string[2] { _scenePath, "res://scenes/screens/char_select/char_select_button.tscn" });

	protected override Control? InitialFocusedControl => null;

	public static NMultiplayerLoadGameScreen? Create()
	{
		if (TestMode.IsOn)
		{
			return null;
		}
		return PreloadManager.Cache.GetScene(_scenePath).Instantiate<NMultiplayerLoadGameScreen>((GenEditState)0);
	}

	public override void _Ready()
	{
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		ConnectSignals();
		_infoPanel = ((Node)this).GetNode<Control>(NodePath.op_Implicit("InfoPanel"));
		_name = ((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("InfoPanel/VBoxContainer/Name"));
		_hp = ((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("InfoPanel/VBoxContainer/HpGoldSpacer/HpGold/Hp/Label"));
		_gold = ((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("InfoPanel/VBoxContainer/HpGoldSpacer/HpGold/Gold/Label"));
		_actLabel = ((Node)this).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("InfoPanel/VBoxContainer/RunLocation/ActLabel"));
		_floorLabel = ((Node)this).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("InfoPanel/VBoxContainer/RunLocation/FloorLabel"));
		_bgContainer = ((Node)this).GetNode<Control>(NodePath.op_Implicit("AnimatedBg"));
		_ascensionPanel = ((Node)this).GetNode<NAscensionPanel>(NodePath.op_Implicit("%AscensionPanel"));
		_remotePlayerContainer = ((Node)this).GetNode<NRemoteLoadLobbyPlayerContainer>(NodePath.op_Implicit("RemotePlayerLoadContainer"));
		_confirmButton = ((Node)this).GetNode<NConfirmButton>(NodePath.op_Implicit("ConfirmButton"));
		_backButton = ((Node)this).GetNode<NBackButton>(NodePath.op_Implicit("BackButton"));
		_unreadyButton = ((Node)this).GetNode<NBackButton>(NodePath.op_Implicit("UnreadyButton"));
		((GodotObject)_confirmButton).Connect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)OnEmbarkPressed), 0u);
		((GodotObject)_unreadyButton).Connect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)OnUnreadyPressed), 0u);
		_unreadyButton.Disable();
		((Node)this).ProcessMode = (ProcessModeEnum)4;
	}

	public void InitializeAsHost(INetGameService gameService, SerializableRun run)
	{
		if (gameService.Type != NetGameType.Host)
		{
			throw new InvalidOperationException($"Initialized character select screen with GameService of type {gameService.Type} when hosting!");
		}
		_runLobby = new LoadRunLobby(gameService, this, run);
		try
		{
			_runLobby.AddLocalHostPlayer();
			AfterMultiplayerStarted();
		}
		catch
		{
			CleanUpLobby(disconnectSession: true);
			throw;
		}
	}

	public void InitializeAsClient(INetGameService gameService, ClientLoadJoinResponseMessage message)
	{
		if (gameService.Type != NetGameType.Client)
		{
			throw new InvalidOperationException($"Initialized character select screen with GameService of type {gameService.Type} when joining!");
		}
		_runLobby = new LoadRunLobby(gameService, this, message);
		AfterMultiplayerStarted();
	}

	public override void OnSubmenuOpened()
	{
		base.OnSubmenuOpened();
		_confirmButton.Enable();
		_remotePlayerContainer.Initialize(_runLobby, displayLocalPlayer: false);
		_ascensionPanel.Initialize(MultiplayerUiMode.Load);
		_ascensionPanel.SetAscensionLevel(_runLobby.Run.Ascension);
	}

	protected override void OnSubmenuShown()
	{
		((Node)this).ProcessMode = (ProcessModeEnum)0;
	}

	public override void OnSubmenuClosed()
	{
		base.OnSubmenuClosed();
		_confirmButton.Disable();
		_remotePlayerContainer.Cleanup();
		if (_runLobby.NetService.Type.IsMultiplayer())
		{
			PlatformUtil.SetRichPresence("MAIN_MENU", null, null);
		}
		CleanUpLobby(disconnectSession: true);
	}

	protected override void OnSubmenuHidden()
	{
		((Node)this).ProcessMode = (ProcessModeEnum)4;
	}

	private void OnEmbarkPressed(NButton _)
	{
		_confirmButton.Disable();
		_backButton.Disable();
		_unreadyButton.Enable();
		_runLobby.SetReady(ready: true);
	}

	private void OnUnreadyPressed(NButton _)
	{
		_confirmButton.Enable();
		_backButton.Enable();
		_unreadyButton.Disable();
		_runLobby.SetReady(ready: false);
	}

	private void UpdateRichPresence()
	{
		if (_runLobby.NetService.Type.IsMultiplayer())
		{
			PlatformUtil.SetRichPresence("LOADING_MP_LOBBY", _runLobby.NetService.GetRawLobbyIdentifier(), _runLobby.ConnectedPlayerIds.Count);
		}
	}

	public override void _Process(double delta)
	{
		if (_runLobby.NetService.IsConnected)
		{
			_runLobby.NetService.Update();
		}
	}

	private void CleanUpLobby(bool disconnectSession)
	{
		_runLobby.CleanUp(disconnectSession);
		_runLobby = null;
	}

	public async Task<bool> ShouldAllowRunToBegin()
	{
		if (_runLobby.ConnectedPlayerIds.Count >= _runLobby.Run.Players.Count)
		{
			return true;
		}
		LocString locString = new LocString("gameplay_ui", "CONFIRM_LOAD_SAVE.body");
		locString.Add("MissingCount", _runLobby.Run.Players.Count - _runLobby.ConnectedPlayerIds.Count);
		NGenericPopup nGenericPopup = NGenericPopup.Create();
		NModalContainer.Instance.Add((Node)(object)nGenericPopup);
		return await nGenericPopup.WaitForConfirmation(locString, new LocString("gameplay_ui", "CONFIRM_LOAD_SAVE.header"), new LocString("gameplay_ui", "CONFIRM_LOAD_SAVE.cancel"), new LocString("gameplay_ui", "CONFIRM_LOAD_SAVE.confirm"));
	}

	private async Task StartRun()
	{
		Log.Info("Loading a multiplayer run. Players: " + string.Join(",", _runLobby.ConnectedPlayerIds) + ".");
		SerializablePlayer serializablePlayer = _runLobby.Run.Players.First((SerializablePlayer p) => p.NetId == _runLobby.NetService.NetId);
		SfxCmd.Play(ModelDb.GetById<CharacterModel>(serializablePlayer.CharacterId).CharacterTransitionSfx);
		await NGame.Instance.Transition.FadeOut(0.8f, ModelDb.GetById<CharacterModel>(serializablePlayer.CharacterId).CharacterSelectTransitionPath);
		RunState runState = RunState.FromSerializable(_runLobby.Run);
		RunManager.Instance.SetUpSavedMultiPlayer(runState, _runLobby);
		await NGame.Instance.LoadRun(runState, _runLobby.Run.PreFinishedRoom);
		CleanUpLobby(disconnectSession: false);
		await NGame.Instance.Transition.FadeIn();
	}

	public void PlayerConnected(ulong playerId)
	{
		Log.Info($"Player connected: {playerId}");
		_remotePlayerContainer.OnPlayerConnected(playerId);
		UpdateRichPresence();
	}

	public void PlayerReadyChanged(ulong playerId)
	{
		Log.Info($"Player ready changed: {playerId}");
		_remotePlayerContainer.OnPlayerChanged(playerId);
		if (playerId == _runLobby.NetService.NetId && !_runLobby.IsPlayerReady(playerId))
		{
			_confirmButton.Enable();
		}
	}

	public void RemotePlayerDisconnected(ulong playerId)
	{
		Log.Info($"Player disconnected: {playerId}");
		_remotePlayerContainer.OnPlayerDisconnected(playerId);
		UpdateRichPresence();
	}

	public void BeginRun()
	{
		NAudioManager.Instance?.StopMusic();
		TaskHelper.RunSafely(StartRun());
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

	private void AfterMultiplayerStarted()
	{
		NGame.Instance.RemoteCursorContainer.Initialize(_runLobby.InputSynchronizer, _runLobby.ConnectedPlayerIds);
		NGame.Instance.ReactionContainer.InitializeNetworking(_runLobby.NetService);
		SerializablePlayer serializablePlayer = _runLobby.Run.Players.First((SerializablePlayer p) => p.NetId == _runLobby.NetService.NetId);
		CharacterModel byId = ModelDb.GetById<CharacterModel>(serializablePlayer.CharacterId);
		SfxCmd.Play(byId.CharacterSelectSfx);
		foreach (Node child in ((Node)_bgContainer).GetChildren(false))
		{
			((Node)(object)_bgContainer).RemoveChildSafely(child);
			child.QueueFreeSafely();
		}
		Control val = PreloadManager.Cache.GetScene(byId.CharacterSelectBg).Instantiate<Control>((GenEditState)0);
		((Node)val).Name = StringName.op_Implicit(byId.Id.Entry + "_bg");
		((Node)(object)_bgContainer).AddChildSafely((Node?)(object)val);
		_name.SetTextAutoSize(byId.Title.GetFormattedText());
		_hp.SetTextAutoSize($"{serializablePlayer.CurrentHp}/{serializablePlayer.MaxHp}");
		_gold.SetTextAutoSize($"{serializablePlayer.Gold}");
		LocString locString = new LocString("main_menu_ui", "MULTIPLAYER_LOAD_MENU.FLOOR");
		locString.Add("floor", _runLobby.Run.VisitedMapCoords.Count);
		_floorLabel.Text = locString.GetFormattedText();
		LocString locString2 = new LocString("main_menu_ui", "MULTIPLAYER_LOAD_MENU.ACT");
		locString2.Add("act", _runLobby.Run.CurrentActIndex + 1);
		_actLabel.Text = locString2.GetFormattedText();
		UpdateRichPresence();
		Logger.logLevelTypeMap[LogType.Network] = LogLevel.Debug;
		Logger.logLevelTypeMap[LogType.Actions] = LogLevel.VeryDebug;
		Logger.logLevelTypeMap[LogType.GameSync] = LogLevel.VeryDebug;
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
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Expected O, but got Unknown
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Expected O, but got Unknown
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_028b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02df: Unknown result type (might be due to invalid IL or missing references)
		//IL_0302: Unknown result type (might be due to invalid IL or missing references)
		//IL_030d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0333: Unknown result type (might be due to invalid IL or missing references)
		//IL_0356: Unknown result type (might be due to invalid IL or missing references)
		//IL_0361: Unknown result type (might be due to invalid IL or missing references)
		//IL_0387: Unknown result type (might be due to invalid IL or missing references)
		//IL_03aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03db: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_040a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0413: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(16);
		list.Add(new MethodInfo(MethodName.Create, new PropertyInfo((Type)24, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false), (MethodFlags)33, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnSubmenuOpened, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnSubmenuShown, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnSubmenuClosed, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnSubmenuHidden, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
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
		list.Add(new MethodInfo(MethodName.PlayerConnected, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("playerId"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.PlayerReadyChanged, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("playerId"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.RemotePlayerDisconnected, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("playerId"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.BeginRun, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AfterMultiplayerStarted, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
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
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_028b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.Create && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			NMultiplayerLoadGameScreen nMultiplayerLoadGameScreen = Create();
			ret = VariantUtils.CreateFrom<NMultiplayerLoadGameScreen>(ref nMultiplayerLoadGameScreen);
			return true;
		}
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnSubmenuOpened && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnSubmenuOpened();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnSubmenuShown && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnSubmenuShown();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnSubmenuClosed && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnSubmenuClosed();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnSubmenuHidden && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnSubmenuHidden();
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
		if ((ref method) == MethodName.PlayerConnected && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			PlayerConnected(VariantUtils.ConvertTo<ulong>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.PlayerReadyChanged && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			PlayerReadyChanged(VariantUtils.ConvertTo<ulong>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.RemotePlayerDisconnected && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			RemotePlayerDisconnected(VariantUtils.ConvertTo<ulong>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.BeginRun && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			BeginRun();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.AfterMultiplayerStarted && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			AfterMultiplayerStarted();
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
			NMultiplayerLoadGameScreen nMultiplayerLoadGameScreen = Create();
			ret = VariantUtils.CreateFrom<NMultiplayerLoadGameScreen>(ref nMultiplayerLoadGameScreen);
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
		if ((ref method) == MethodName.OnSubmenuOpened)
		{
			return true;
		}
		if ((ref method) == MethodName.OnSubmenuShown)
		{
			return true;
		}
		if ((ref method) == MethodName.OnSubmenuClosed)
		{
			return true;
		}
		if ((ref method) == MethodName.OnSubmenuHidden)
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
		if ((ref method) == MethodName.PlayerConnected)
		{
			return true;
		}
		if ((ref method) == MethodName.PlayerReadyChanged)
		{
			return true;
		}
		if ((ref method) == MethodName.RemotePlayerDisconnected)
		{
			return true;
		}
		if ((ref method) == MethodName.BeginRun)
		{
			return true;
		}
		if ((ref method) == MethodName.AfterMultiplayerStarted)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
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
		if ((ref name) == PropertyName._selectedButton)
		{
			_selectedButton = VariantUtils.ConvertTo<NCharacterSelectButton>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._bgContainer)
		{
			_bgContainer = VariantUtils.ConvertTo<Control>(ref value);
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
		if ((ref name) == PropertyName._floorLabel)
		{
			_floorLabel = VariantUtils.ConvertTo<MegaRichTextLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._actLabel)
		{
			_actLabel = VariantUtils.ConvertTo<MegaRichTextLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._remotePlayerContainer)
		{
			_remotePlayerContainer = VariantUtils.ConvertTo<NRemoteLoadLobbyPlayerContainer>(ref value);
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
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName.InitialFocusedControl)
		{
			Control initialFocusedControl = InitialFocusedControl;
			value = VariantUtils.CreateFrom<Control>(ref initialFocusedControl);
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
		if ((ref name) == PropertyName._selectedButton)
		{
			value = VariantUtils.CreateFrom<NCharacterSelectButton>(ref _selectedButton);
			return true;
		}
		if ((ref name) == PropertyName._bgContainer)
		{
			value = VariantUtils.CreateFrom<Control>(ref _bgContainer);
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
		if ((ref name) == PropertyName._floorLabel)
		{
			value = VariantUtils.CreateFrom<MegaRichTextLabel>(ref _floorLabel);
			return true;
		}
		if ((ref name) == PropertyName._actLabel)
		{
			value = VariantUtils.CreateFrom<MegaRichTextLabel>(ref _actLabel);
			return true;
		}
		if ((ref name) == PropertyName._remotePlayerContainer)
		{
			value = VariantUtils.CreateFrom<NRemoteLoadLobbyPlayerContainer>(ref _remotePlayerContainer);
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
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._name, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._infoPanel, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._hp, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._gold, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._selectedButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._bgContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._confirmButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._backButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._unreadyButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._ascensionPanel, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._floorLabel, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._actLabel, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._remotePlayerContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._infoPanelTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)5, PropertyName._infoPanelPosFinalVal, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.InitialFocusedControl, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName._name, Variant.From<MegaLabel>(ref _name));
		info.AddProperty(PropertyName._infoPanel, Variant.From<Control>(ref _infoPanel));
		info.AddProperty(PropertyName._hp, Variant.From<MegaLabel>(ref _hp));
		info.AddProperty(PropertyName._gold, Variant.From<MegaLabel>(ref _gold));
		info.AddProperty(PropertyName._selectedButton, Variant.From<NCharacterSelectButton>(ref _selectedButton));
		info.AddProperty(PropertyName._bgContainer, Variant.From<Control>(ref _bgContainer));
		info.AddProperty(PropertyName._confirmButton, Variant.From<NConfirmButton>(ref _confirmButton));
		info.AddProperty(PropertyName._backButton, Variant.From<NBackButton>(ref _backButton));
		info.AddProperty(PropertyName._unreadyButton, Variant.From<NBackButton>(ref _unreadyButton));
		info.AddProperty(PropertyName._ascensionPanel, Variant.From<NAscensionPanel>(ref _ascensionPanel));
		info.AddProperty(PropertyName._floorLabel, Variant.From<MegaRichTextLabel>(ref _floorLabel));
		info.AddProperty(PropertyName._actLabel, Variant.From<MegaRichTextLabel>(ref _actLabel));
		info.AddProperty(PropertyName._remotePlayerContainer, Variant.From<NRemoteLoadLobbyPlayerContainer>(ref _remotePlayerContainer));
		info.AddProperty(PropertyName._infoPanelTween, Variant.From<Tween>(ref _infoPanelTween));
		info.AddProperty(PropertyName._infoPanelPosFinalVal, Variant.From<Vector2>(ref _infoPanelPosFinalVal));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
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
		if (info.TryGetProperty(PropertyName._hp, ref val3))
		{
			_hp = ((Variant)(ref val3)).As<MegaLabel>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._gold, ref val4))
		{
			_gold = ((Variant)(ref val4)).As<MegaLabel>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._selectedButton, ref val5))
		{
			_selectedButton = ((Variant)(ref val5)).As<NCharacterSelectButton>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._bgContainer, ref val6))
		{
			_bgContainer = ((Variant)(ref val6)).As<Control>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName._confirmButton, ref val7))
		{
			_confirmButton = ((Variant)(ref val7)).As<NConfirmButton>();
		}
		Variant val8 = default(Variant);
		if (info.TryGetProperty(PropertyName._backButton, ref val8))
		{
			_backButton = ((Variant)(ref val8)).As<NBackButton>();
		}
		Variant val9 = default(Variant);
		if (info.TryGetProperty(PropertyName._unreadyButton, ref val9))
		{
			_unreadyButton = ((Variant)(ref val9)).As<NBackButton>();
		}
		Variant val10 = default(Variant);
		if (info.TryGetProperty(PropertyName._ascensionPanel, ref val10))
		{
			_ascensionPanel = ((Variant)(ref val10)).As<NAscensionPanel>();
		}
		Variant val11 = default(Variant);
		if (info.TryGetProperty(PropertyName._floorLabel, ref val11))
		{
			_floorLabel = ((Variant)(ref val11)).As<MegaRichTextLabel>();
		}
		Variant val12 = default(Variant);
		if (info.TryGetProperty(PropertyName._actLabel, ref val12))
		{
			_actLabel = ((Variant)(ref val12)).As<MegaRichTextLabel>();
		}
		Variant val13 = default(Variant);
		if (info.TryGetProperty(PropertyName._remotePlayerContainer, ref val13))
		{
			_remotePlayerContainer = ((Variant)(ref val13)).As<NRemoteLoadLobbyPlayerContainer>();
		}
		Variant val14 = default(Variant);
		if (info.TryGetProperty(PropertyName._infoPanelTween, ref val14))
		{
			_infoPanelTween = ((Variant)(ref val14)).As<Tween>();
		}
		Variant val15 = default(Variant);
		if (info.TryGetProperty(PropertyName._infoPanelPosFinalVal, ref val15))
		{
			_infoPanelPosFinalVal = ((Variant)(ref val15)).As<Vector2>();
		}
	}
}
