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
using MegaCrit.Sts2.Core.Entities.Multiplayer;
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

namespace MegaCrit.Sts2.Core.Nodes.Screens.DailyRun;

[ScriptPath("res://src/Core/Nodes/Screens/DailyRun/NDailyRunLoadScreen.cs")]
public class NDailyRunLoadScreen : NSubmenu, ILoadRunLobbyListener
{
	public new class MethodName : NSubmenu.MethodName
	{
		public static readonly StringName Create = StringName.op_Implicit("Create");

		public new static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public new static readonly StringName OnSubmenuOpened = StringName.op_Implicit("OnSubmenuOpened");

		public new static readonly StringName OnSubmenuClosed = StringName.op_Implicit("OnSubmenuClosed");

		public new static readonly StringName OnSubmenuShown = StringName.op_Implicit("OnSubmenuShown");

		public new static readonly StringName OnSubmenuHidden = StringName.op_Implicit("OnSubmenuHidden");

		public static readonly StringName InitializeDisplay = StringName.op_Implicit("InitializeDisplay");

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

		public static readonly StringName _dateLabel = StringName.op_Implicit("_dateLabel");

		public static readonly StringName _embarkButton = StringName.op_Implicit("_embarkButton");

		public new static readonly StringName _backButton = StringName.op_Implicit("_backButton");

		public static readonly StringName _unreadyButton = StringName.op_Implicit("_unreadyButton");

		public static readonly StringName _characterContainer = StringName.op_Implicit("_characterContainer");

		public static readonly StringName _leaderboard = StringName.op_Implicit("_leaderboard");

		public static readonly StringName _modifiersTitleLabel = StringName.op_Implicit("_modifiersTitleLabel");

		public static readonly StringName _modifiersContainer = StringName.op_Implicit("_modifiersContainer");

		public static readonly StringName _remotePlayerContainer = StringName.op_Implicit("_remotePlayerContainer");

		public static readonly StringName _readyAndWaitingContainer = StringName.op_Implicit("_readyAndWaitingContainer");
	}

	public new class SignalName : NSubmenu.SignalName
	{
	}

	private static readonly string _scenePath = SceneHelper.GetScenePath("screens/daily_run/daily_run_load_screen");

	private static readonly LocString _ascensionLoc = new LocString("main_menu_ui", "DAILY_RUN_MENU.ASCENSION");

	public static readonly string dateFormat = LocManager.Instance.GetTable("main_menu_ui").GetRawText("DAILY_RUN_MENU.DATE_FORMAT");

	private MegaLabel _dateLabel;

	private NConfirmButton _embarkButton;

	private NBackButton _backButton;

	private NBackButton _unreadyButton;

	private NDailyRunCharacterContainer _characterContainer;

	private NDailyRunLeaderboard _leaderboard;

	private MegaLabel _modifiersTitleLabel;

	private Control _modifiersContainer;

	private readonly List<NDailyRunScreenModifier> _modifierContainers = new List<NDailyRunScreenModifier>();

	private NRemoteLoadLobbyPlayerContainer _remotePlayerContainer;

	private Control _readyAndWaitingContainer;

	private LoadRunLobby? _lobby;

	public static string[] AssetPaths => new string[1] { _scenePath };

	protected override Control? InitialFocusedControl => null;

	public static NDailyRunLoadScreen? Create()
	{
		if (TestMode.IsOn)
		{
			return null;
		}
		return PreloadManager.Cache.GetScene(_scenePath).Instantiate<NDailyRunLoadScreen>((GenEditState)0);
	}

	public override void _Ready()
	{
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		ConnectSignals();
		_embarkButton = ((Node)this).GetNode<NConfirmButton>(NodePath.op_Implicit("%ConfirmButton"));
		_backButton = ((Node)this).GetNode<NBackButton>(NodePath.op_Implicit("%BackButton"));
		_unreadyButton = ((Node)this).GetNode<NBackButton>(NodePath.op_Implicit("%UnreadyButton"));
		_dateLabel = ((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%Date"));
		_leaderboard = ((Node)this).GetNode<NDailyRunLeaderboard>(NodePath.op_Implicit("%Leaderboards"));
		_modifiersTitleLabel = ((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%ModifiersLabel"));
		_modifiersContainer = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%ModifiersContainer"));
		_characterContainer = ((Node)this).GetNode<NDailyRunCharacterContainer>(NodePath.op_Implicit("%CharacterContainer"));
		_remotePlayerContainer = ((Node)this).GetNode<NRemoteLoadLobbyPlayerContainer>(NodePath.op_Implicit("%RemotePlayerLoadContainer"));
		_readyAndWaitingContainer = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%ReadyAndWaitingPanel"));
		foreach (NDailyRunScreenModifier item in ((IEnumerable)((Node)_modifiersContainer).GetChildren(false)).OfType<NDailyRunScreenModifier>())
		{
			_modifierContainers.Add(item);
		}
		((CanvasItem)_readyAndWaitingContainer).Visible = false;
		((GodotObject)_embarkButton).Connect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)OnEmbarkPressed), 0u);
		((GodotObject)_unreadyButton).Connect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)OnUnreadyPressed), 0u);
		_unreadyButton.Disable();
		_leaderboard.Cleanup();
		((Node)this).ProcessMode = (ProcessModeEnum)4;
	}

	public void InitializeAsHost(INetGameService gameService, SerializableRun run)
	{
		if (gameService.Type != NetGameType.Host)
		{
			throw new InvalidOperationException($"Initialized daily run load screen with net service of type {gameService.Type} when hosting!");
		}
		_lobby = new LoadRunLobby(gameService, this, run);
		try
		{
			_lobby.AddLocalHostPlayer();
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
			throw new InvalidOperationException($"Initialized daily run load screen with net service of type {gameService.Type} when joining!");
		}
		_lobby = new LoadRunLobby(gameService, this, message);
		AfterMultiplayerStarted();
	}

	public override void OnSubmenuOpened()
	{
		base.OnSubmenuOpened();
		_leaderboard.Initialize(_lobby.Run.DailyTime.Value, _lobby.Run.Players.Select((SerializablePlayer p) => p.NetId), allowPagination: true);
		_embarkButton.Enable();
		_remotePlayerContainer.Initialize(_lobby, displayLocalPlayer: false);
	}

	public override void OnSubmenuClosed()
	{
		_embarkButton.Disable();
		_remotePlayerContainer.Cleanup();
		_leaderboard.Cleanup();
		LoadRunLobby? lobby = _lobby;
		if (lobby != null && lobby.NetService.Type.IsMultiplayer())
		{
			PlatformUtil.SetRichPresence("MAIN_MENU", null, null);
		}
		CleanUpLobby(disconnectSession: true);
	}

	protected override void OnSubmenuShown()
	{
		((Node)this).ProcessMode = (ProcessModeEnum)0;
	}

	protected override void OnSubmenuHidden()
	{
		((Node)this).ProcessMode = (ProcessModeEnum)4;
	}

	private void InitializeDisplay()
	{
		if (_lobby == null)
		{
			throw new InvalidOperationException("Tried to initialize daily run display before lobby was initialized!");
		}
		_ascensionLoc.Add("ascension", _lobby.Run.Ascension);
		DateTimeOffset value = _lobby.Run.DailyTime.Value;
		SerializablePlayer serializablePlayer = _lobby.Run.Players.First((SerializablePlayer p) => p.NetId == _lobby.NetService.NetId);
		CharacterModel byId = ModelDb.GetById<CharacterModel>(serializablePlayer.CharacterId);
		_characterContainer.Fill(byId, serializablePlayer.NetId, _lobby.Run.Ascension, _lobby.NetService);
		_dateLabel.SetTextAutoSize(value.ToString(dateFormat));
		_embarkButton.Enable();
		for (int num = 0; num < _lobby.Run.Modifiers.Count; num++)
		{
			ModifierModel modifier = ModifierModel.FromSerializable(_lobby.Run.Modifiers[num]);
			_modifierContainers[num].Fill(modifier);
		}
	}

	private void OnEmbarkPressed(NButton _)
	{
		_embarkButton.Disable();
		_backButton.Disable();
		_lobby.SetReady(ready: true);
		if (_lobby.NetService.Type.IsMultiplayer() && _lobby.Run.Players.Any((SerializablePlayer p) => !_lobby.IsPlayerReady(p.NetId)))
		{
			((CanvasItem)_readyAndWaitingContainer).Visible = true;
			_unreadyButton.Enable();
		}
	}

	private void OnUnreadyPressed(NButton _)
	{
		_embarkButton.Enable();
		_unreadyButton.Disable();
		_backButton.Enable();
		_lobby.SetReady(ready: true);
		((CanvasItem)_readyAndWaitingContainer).Visible = false;
	}

	private void UpdateRichPresence()
	{
		LoadRunLobby? lobby = _lobby;
		if (lobby != null && lobby.NetService.Type.IsMultiplayer())
		{
			PlatformUtil.SetRichPresence("LOADING_MP_LOBBY", _lobby.NetService.GetRawLobbyIdentifier(), _lobby.ConnectedPlayerIds.Count);
		}
	}

	public override void _Process(double delta)
	{
		LoadRunLobby? lobby = _lobby;
		if (lobby != null && lobby.NetService.IsConnected)
		{
			_lobby.NetService.Update();
		}
	}

	private void CleanUpLobby(bool disconnectSession)
	{
		_lobby.CleanUp(disconnectSession);
		_lobby = null;
	}

	public async Task<bool> ShouldAllowRunToBegin()
	{
		if (_lobby.ConnectedPlayerIds.Count >= _lobby.Run.Players.Count)
		{
			return true;
		}
		LocString locString = new LocString("gameplay_ui", "CONFIRM_LOAD_SAVE.body");
		locString.Add("MissingCount", _lobby.Run.Players.Count - _lobby.ConnectedPlayerIds.Count);
		NGenericPopup nGenericPopup = NGenericPopup.Create();
		NModalContainer.Instance.Add((Node)(object)nGenericPopup);
		return await nGenericPopup.WaitForConfirmation(locString, new LocString("gameplay_ui", "CONFIRM_LOAD_SAVE.header"), new LocString("gameplay_ui", "CONFIRM_LOAD_SAVE.cancel"), new LocString("gameplay_ui", "CONFIRM_LOAD_SAVE.confirm"));
	}

	private async Task StartRun()
	{
		Log.Info("Loading a multiplayer run. Players: " + string.Join(",", _lobby.ConnectedPlayerIds) + ".");
		SerializablePlayer serializablePlayer = _lobby.Run.Players.First((SerializablePlayer p) => p.NetId == _lobby.NetService.NetId);
		SfxCmd.Play(ModelDb.GetById<CharacterModel>(serializablePlayer.CharacterId).CharacterTransitionSfx);
		await NGame.Instance.Transition.FadeOut(0.8f, ModelDb.GetById<CharacterModel>(serializablePlayer.CharacterId).CharacterSelectTransitionPath);
		RunState runState = RunState.FromSerializable(_lobby.Run);
		RunManager.Instance.SetUpSavedMultiPlayer(runState, _lobby);
		await NGame.Instance.LoadRun(runState, _lobby.Run.PreFinishedRoom);
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
		if (playerId == _lobby.NetService.NetId && !_lobby.IsPlayerReady(playerId))
		{
			_embarkButton.Enable();
		}
		if (playerId == _lobby.NetService.NetId && _lobby.NetService.Type.IsMultiplayer())
		{
			_characterContainer.SetIsReady(_lobby.IsPlayerReady(playerId));
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
		NGame.Instance.RemoteCursorContainer.Initialize(_lobby.InputSynchronizer, _lobby.ConnectedPlayerIds);
		NGame.Instance.ReactionContainer.InitializeNetworking(_lobby.NetService);
		InitializeDisplay();
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
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Expected O, but got Unknown
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Expected O, but got Unknown
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_0289: Unknown result type (might be due to invalid IL or missing references)
		//IL_0294: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_030e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0331: Unknown result type (might be due to invalid IL or missing references)
		//IL_033c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0362: Unknown result type (might be due to invalid IL or missing references)
		//IL_0385: Unknown result type (might be due to invalid IL or missing references)
		//IL_0390: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_040a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0413: Unknown result type (might be due to invalid IL or missing references)
		//IL_0439: Unknown result type (might be due to invalid IL or missing references)
		//IL_0442: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(17);
		list.Add(new MethodInfo(MethodName.Create, new PropertyInfo((Type)24, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false), (MethodFlags)33, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnSubmenuOpened, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnSubmenuClosed, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnSubmenuShown, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnSubmenuHidden, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.InitializeDisplay, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
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
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_028b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02df: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.Create && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			NDailyRunLoadScreen nDailyRunLoadScreen = Create();
			ret = VariantUtils.CreateFrom<NDailyRunLoadScreen>(ref nDailyRunLoadScreen);
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
		if ((ref method) == MethodName.OnSubmenuClosed && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnSubmenuClosed();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnSubmenuShown && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnSubmenuShown();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnSubmenuHidden && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnSubmenuHidden();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.InitializeDisplay && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			InitializeDisplay();
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
			NDailyRunLoadScreen nDailyRunLoadScreen = Create();
			ret = VariantUtils.CreateFrom<NDailyRunLoadScreen>(ref nDailyRunLoadScreen);
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
		if ((ref method) == MethodName.OnSubmenuClosed)
		{
			return true;
		}
		if ((ref method) == MethodName.OnSubmenuShown)
		{
			return true;
		}
		if ((ref method) == MethodName.OnSubmenuHidden)
		{
			return true;
		}
		if ((ref method) == MethodName.InitializeDisplay)
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
		if ((ref name) == PropertyName._dateLabel)
		{
			_dateLabel = VariantUtils.ConvertTo<MegaLabel>(ref value);
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
		if ((ref name) == PropertyName._characterContainer)
		{
			_characterContainer = VariantUtils.ConvertTo<NDailyRunCharacterContainer>(ref value);
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
			_remotePlayerContainer = VariantUtils.ConvertTo<NRemoteLoadLobbyPlayerContainer>(ref value);
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
		if ((ref name) == PropertyName.InitialFocusedControl)
		{
			Control initialFocusedControl = InitialFocusedControl;
			value = VariantUtils.CreateFrom<Control>(ref initialFocusedControl);
			return true;
		}
		if ((ref name) == PropertyName._dateLabel)
		{
			value = VariantUtils.CreateFrom<MegaLabel>(ref _dateLabel);
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
		if ((ref name) == PropertyName._characterContainer)
		{
			value = VariantUtils.CreateFrom<NDailyRunCharacterContainer>(ref _characterContainer);
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
			value = VariantUtils.CreateFrom<NRemoteLoadLobbyPlayerContainer>(ref _remotePlayerContainer);
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
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName.InitialFocusedControl, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._dateLabel, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._embarkButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._backButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._unreadyButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._characterContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName._dateLabel, Variant.From<MegaLabel>(ref _dateLabel));
		info.AddProperty(PropertyName._embarkButton, Variant.From<NConfirmButton>(ref _embarkButton));
		info.AddProperty(PropertyName._backButton, Variant.From<NBackButton>(ref _backButton));
		info.AddProperty(PropertyName._unreadyButton, Variant.From<NBackButton>(ref _unreadyButton));
		info.AddProperty(PropertyName._characterContainer, Variant.From<NDailyRunCharacterContainer>(ref _characterContainer));
		info.AddProperty(PropertyName._leaderboard, Variant.From<NDailyRunLeaderboard>(ref _leaderboard));
		info.AddProperty(PropertyName._modifiersTitleLabel, Variant.From<MegaLabel>(ref _modifiersTitleLabel));
		info.AddProperty(PropertyName._modifiersContainer, Variant.From<Control>(ref _modifiersContainer));
		info.AddProperty(PropertyName._remotePlayerContainer, Variant.From<NRemoteLoadLobbyPlayerContainer>(ref _remotePlayerContainer));
		info.AddProperty(PropertyName._readyAndWaitingContainer, Variant.From<Control>(ref _readyAndWaitingContainer));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._dateLabel, ref val))
		{
			_dateLabel = ((Variant)(ref val)).As<MegaLabel>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._embarkButton, ref val2))
		{
			_embarkButton = ((Variant)(ref val2)).As<NConfirmButton>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._backButton, ref val3))
		{
			_backButton = ((Variant)(ref val3)).As<NBackButton>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._unreadyButton, ref val4))
		{
			_unreadyButton = ((Variant)(ref val4)).As<NBackButton>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._characterContainer, ref val5))
		{
			_characterContainer = ((Variant)(ref val5)).As<NDailyRunCharacterContainer>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._leaderboard, ref val6))
		{
			_leaderboard = ((Variant)(ref val6)).As<NDailyRunLeaderboard>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName._modifiersTitleLabel, ref val7))
		{
			_modifiersTitleLabel = ((Variant)(ref val7)).As<MegaLabel>();
		}
		Variant val8 = default(Variant);
		if (info.TryGetProperty(PropertyName._modifiersContainer, ref val8))
		{
			_modifiersContainer = ((Variant)(ref val8)).As<Control>();
		}
		Variant val9 = default(Variant);
		if (info.TryGetProperty(PropertyName._remotePlayerContainer, ref val9))
		{
			_remotePlayerContainer = ((Variant)(ref val9)).As<NRemoteLoadLobbyPlayerContainer>();
		}
		Variant val10 = default(Variant);
		if (info.TryGetProperty(PropertyName._readyAndWaitingContainer, ref val10))
		{
			_readyAndWaitingContainer = ((Variant)(ref val10)).As<Control>();
		}
	}
}
