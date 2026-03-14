using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Debug;
using MegaCrit.Sts2.Core.DevConsole.ConsoleCommands;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Map;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Multiplayer;
using MegaCrit.Sts2.Core.Multiplayer.Connection;
using MegaCrit.Sts2.Core.Multiplayer.Game;
using MegaCrit.Sts2.Core.Multiplayer.Game.Lobby;
using MegaCrit.Sts2.Core.Multiplayer.Replay;
using MegaCrit.Sts2.Core.Multiplayer.Serialization;
using MegaCrit.Sts2.Core.Nodes.Audio;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Platform;
using MegaCrit.Sts2.Core.Platform.Steam;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.Unlocks;

namespace MegaCrit.Sts2.Core.Nodes.Debug.Multiplayer;

[ScriptPath("res://src/Core/Nodes/Debug/Multiplayer/NMultiplayerTest.cs")]
public class NMultiplayerTest : Control, IStartRunLobbyListener
{
	private struct CharacterContainer
	{
		public TextureRect characterImage;

		public Label playerName;
	}

	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName _ExitTree = StringName.op_Implicit("_ExitTree");

		public static readonly StringName AddGame = StringName.op_Implicit("AddGame");

		public static readonly StringName HostButtonPressed = StringName.op_Implicit("HostButtonPressed");

		public static readonly StringName SteamHostButtonPressed = StringName.op_Implicit("SteamHostButtonPressed");

		public static readonly StringName JoinButtonPressed = StringName.op_Implicit("JoinButtonPressed");

		public static readonly StringName ReadyButtonPressed = StringName.op_Implicit("ReadyButtonPressed");

		public static readonly StringName Disconnect = StringName.op_Implicit("Disconnect");

		public static readonly StringName AfterMultiplayerStarted = StringName.op_Implicit("AfterMultiplayerStarted");

		public static readonly StringName ChooseReplayToLoad = StringName.op_Implicit("ChooseReplayToLoad");

		public static readonly StringName LoadReplay = StringName.op_Implicit("LoadReplay");

		public static readonly StringName _Process = StringName.op_Implicit("_Process");

		public static readonly StringName AscensionChanged = StringName.op_Implicit("AscensionChanged");

		public static readonly StringName SeedChanged = StringName.op_Implicit("SeedChanged");

		public static readonly StringName ModifiersChanged = StringName.op_Implicit("ModifiersChanged");

		public static readonly StringName MaxAscensionChanged = StringName.op_Implicit("MaxAscensionChanged");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName _ipField = StringName.op_Implicit("_ipField");

		public static readonly StringName _idField = StringName.op_Implicit("_idField");

		public static readonly StringName _readyButton = StringName.op_Implicit("_readyButton");

		public static readonly StringName _readyIndicator = StringName.op_Implicit("_readyIndicator");

		public static readonly StringName _loadingPanel = StringName.op_Implicit("_loadingPanel");

		public static readonly StringName _characterPaginator = StringName.op_Implicit("_characterPaginator");

		public static readonly StringName _game = StringName.op_Implicit("_game");

		public static readonly StringName _ignoreReplayModelIdHash = StringName.op_Implicit("_ignoreReplayModelIdHash");

		public static readonly StringName _beginningRun = StringName.op_Implicit("_beginningRun");
	}

	public class SignalName : SignalName
	{
	}

	private const ushort _port = 33771;

	private TextEdit _ipField;

	private TextEdit _idField;

	private Button _readyButton;

	private Control _readyIndicator;

	private Control _loadingPanel;

	private NMultiplayerTestCharacterPaginator _characterPaginator;

	private readonly List<CharacterContainer> _characterContainers = new List<CharacterContainer>();

	private NGame _game;

	private StartRunLobby? _lobby;

	private readonly SerializablePlayer _localPlayerData = new SerializablePlayer();

	private IBootstrapSettings? _settings;

	private bool _ignoreReplayModelIdHash;

	private bool _beginningRun;

	public override void _Ready()
	{
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02af: Unknown result type (might be due to invalid IL or missing references)
		_ipField = ((Node)this).GetNode<TextEdit>(NodePath.op_Implicit("IpField"));
		_idField = ((Node)this).GetNode<TextEdit>(NodePath.op_Implicit("NameField"));
		_characterPaginator = ((Node)this).GetNode<NMultiplayerTestCharacterPaginator>(NodePath.op_Implicit("CharacterChooser"));
		Button node = ((Node)this).GetNode<Button>(NodePath.op_Implicit("HostButton"));
		Button node2 = ((Node)this).GetNode<Button>(NodePath.op_Implicit("SteamHostButton"));
		Button node3 = ((Node)this).GetNode<Button>(NodePath.op_Implicit("JoinButton"));
		_readyButton = ((Node)this).GetNode<Button>(NodePath.op_Implicit("ReadyButton"));
		_readyIndicator = ((Node)this).GetNode<Control>(NodePath.op_Implicit("ReadyButton/ReadyIndicator"));
		Button node4 = ((Node)this).GetNode<Button>(NodePath.op_Implicit("ReplayButton"));
		Button node5 = ((Node)this).GetNode<Button>(NodePath.op_Implicit("DeleteCloudSavesButton"));
		_loadingPanel = ((Node)this).GetNode<Control>(NodePath.op_Implicit("LoadingPanel"));
		Control node6 = ((Node)this).GetNode<Control>(NodePath.op_Implicit("Characters"));
		foreach (Node child in ((Node)node6).GetChildren(false))
		{
			_characterContainers.Add(new CharacterContainer
			{
				characterImage = child.GetNode<TextureRect>(NodePath.op_Implicit("Image")),
				playerName = child.GetNode<Label>(NodePath.op_Implicit("Name"))
			});
		}
		((GodotObject)node).Connect(SignalName.ButtonUp, Callable.From((Action)HostButtonPressed), 0u);
		((GodotObject)node2).Connect(SignalName.ButtonUp, Callable.From((Action)SteamHostButtonPressed), 0u);
		((GodotObject)node3).Connect(SignalName.ButtonUp, Callable.From((Action)JoinButtonPressed), 0u);
		((GodotObject)_readyButton).Connect(SignalName.ButtonUp, Callable.From((Action)ReadyButtonPressed), 0u);
		((GodotObject)node4).Connect(SignalName.ButtonUp, Callable.From((Action)ChooseReplayToLoad), 0u);
		((GodotObject)node5).Connect(SignalName.ButtonUp, Callable.From((Action)CloudConsoleCmd.DeleteCloudSaves), 0u);
		((CanvasItem)_characterPaginator).Visible = false;
		_characterPaginator.CharacterChanged += OnCharacterChanged;
		((CanvasItem)_readyButton).Visible = false;
		_game = ((Node)((Node)this).GetTree().Root).GetNodeOrNull<NGame>(NodePath.op_Implicit("Game"));
		if (_game == null)
		{
			_game = SceneHelper.Instantiate<NGame>("game");
			_game.StartOnMainMenu = false;
			Callable val = Callable.From((Action)AddGame);
			((Callable)(ref val)).CallDeferred(Array.Empty<Variant>());
		}
		Type type = BootstrapSettingsUtil.Get();
		if (type != null)
		{
			_settings = (IBootstrapSettings)Activator.CreateInstance(type);
			if (!_settings.BootstrapInMultiplayer)
			{
				_settings = null;
			}
			else
			{
				PreloadManager.Enabled = _settings.DoPreloading;
				_game.DebugSeedOverride = _settings.Seed;
			}
		}
		if (!SteamInitializer.Initialized)
		{
			SteamInitializer.Initialize((Node)(object)_game);
		}
		Logger.logLevelTypeMap[LogType.Network] = LogLevel.Debug;
		Logger.logLevelTypeMap[LogType.Actions] = LogLevel.VeryDebug;
		Logger.logLevelTypeMap[LogType.GameSync] = LogLevel.VeryDebug;
	}

	public override void _ExitTree()
	{
		_characterPaginator.CharacterChanged -= OnCharacterChanged;
		if (!_beginningRun)
		{
			_lobby?.CleanUp(disconnectSession: true);
		}
	}

	private void AddGame()
	{
		Node root = (Node)(object)((Node)this).GetTree().Root;
		root.AddChildSafely((Node?)(object)_game);
		_game.RootSceneContainer.SetCurrentScene((Control)(object)this);
		TaskHelper.RunSafely(_game.Transition.FadeIn());
	}

	private void HostButtonPressed()
	{
		TaskHelper.RunSafely(StartHost(steam: false));
	}

	private void SteamHostButtonPressed()
	{
		TaskHelper.RunSafely(StartHost(steam: true));
	}

	private void JoinButtonPressed()
	{
		ulong netId = ((_idField.Text != string.Empty) ? ulong.Parse(_idField.Text) : 1000);
		string ip = ((_ipField.Text != string.Empty) ? _ipField.Text : "127.0.0.1");
		ENetClientConnectionInitializer initializer = new ENetClientConnectionInitializer(netId, ip, 33771);
		TaskHelper.RunSafely(JoinToHost(initializer));
	}

	private void ReadyButtonPressed()
	{
		_localPlayerData.Deck = _characterPaginator.Character.StartingDeck.Select((CardModel c) => c.ToMutable().ToSerializable()).ToList();
		_localPlayerData.Relics = _characterPaginator.Character.StartingRelics.Select((RelicModel r) => r.ToMutable().ToSerializable()).ToList();
		_localPlayerData.Potions = new List<SerializablePotion>();
		_localPlayerData.Rng = new SerializablePlayerRngSet();
		_localPlayerData.Odds = new SerializablePlayerOddsSet();
		_localPlayerData.RelicGrabBag = new SerializableRelicGrabBag();
		_localPlayerData.ExtraFields = new SerializableExtraPlayerFields();
		_localPlayerData.UnlockState = new SerializableUnlockState();
		_lobby.SetReady(ready: true);
		((CanvasItem)_readyIndicator).Visible = true;
	}

	public void BeginRun(string seed, List<ActModel> acts, IReadOnlyList<ModifierModel> __)
	{
		((CanvasItem)_loadingPanel).Visible = true;
		TaskHelper.RunSafely(BeginRunAsyncWrapper(seed, acts));
	}

	private async Task BeginRunAsyncWrapper(string seed, List<ActModel> acts)
	{
		try
		{
			await BeginRunAsync(seed, acts);
		}
		finally
		{
			if (((Node?)(object)_loadingPanel).IsValid())
			{
				((CanvasItem)_loadingPanel).Visible = false;
			}
		}
	}

	private async Task BeginRunAsync(string seed, List<ActModel> acts)
	{
		_beginningRun = true;
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
				_game.RootSceneContainer.SetCurrentScene((Control)(object)NRun.Create(runState));
				await RunManager.Instance.SetActInternal(0);
				await SaveManager.Instance.SaveRun(null);
				_lobby.CleanUp(disconnectSession: false);
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
			await _game.StartNewMultiplayerRun(_lobby, shouldSave: true, acts, Array.Empty<ModifierModel>(), seed, _lobby.Ascension);
			_lobby.CleanUp(disconnectSession: false);
		}
	}

	private void Disconnect(NetError reason)
	{
		_lobby?.NetService.Disconnect(reason);
		_lobby?.CleanUp(disconnectSession: true);
		_lobby = null;
	}

	private async Task<bool> StartHost(bool steam)
	{
		Disconnect(NetError.Quit);
		NetHostGameService netService = new NetHostGameService();
		NetErrorInfo? value = ((!steam) ? netService.StartENetHost(33771, 4) : (await netService.StartSteamHost(4)));
		if (!value.HasValue)
		{
			_lobby = new StartRunLobby(GameMode.Standard, netService, this, 4);
			_lobby.AddLocalHostPlayer(new UnlockState(SaveManager.Instance.Progress), SaveManager.Instance.Progress.MaxMultiplayerAscension);
			AfterMultiplayerStarted();
			Log.Info("Successful host");
		}
		else
		{
			_lobby = null;
			Log.Info($"Failed host: {value}");
		}
		return !value.HasValue;
	}

	public async Task JoinToHost(IClientConnectionInitializer initializer)
	{
		Disconnect(NetError.Quit);
		JoinFlow joinFlow = new JoinFlow();
		try
		{
			JoinResult joinResult = await joinFlow.Begin(initializer, ((Node)this).GetTree());
			if (joinResult.sessionState == RunSessionState.InLobby)
			{
				Log.Info("Successfully joined lobby");
				_lobby = new StartRunLobby(joinResult.gameMode, joinFlow.NetService, this, -1);
				_lobby.InitializeFromMessage(joinResult.joinResponse.Value);
				AfterMultiplayerStarted();
			}
			else if (joinResult.sessionState == RunSessionState.Running)
			{
				Log.Info("Successfully joined run in-progress. Initializing run");
				throw new NotImplementedException("Run re-joining has yet to be implemented!");
			}
		}
		catch (Exception value)
		{
			joinFlow.NetService.Disconnect(NetError.RunInProgress);
			_lobby = null;
			Log.Info($"Failed join: {value}");
		}
	}

	private void AfterMultiplayerStarted()
	{
		foreach (LobbyPlayer player in _lobby.Players)
		{
			_characterContainers[player.slotId].characterImage.Texture = player.character.IconTexture;
			_characterContainers[player.slotId].playerName.Text = PlatformUtil.GetPlayerName(_lobby.NetService.Platform, player.id);
		}
		((CanvasItem)_readyButton).Visible = true;
		((CanvasItem)_characterPaginator).Visible = true;
		_game.RemoteCursorContainer.Initialize(_lobby.InputSynchronizer, _lobby.Players.Select((LobbyPlayer p) => p.id));
		_game.ReactionContainer.InitializeNetworking(_lobby.NetService);
		OnCharacterChanged(_lobby.LocalPlayer.character);
	}

	private void ChooseReplayToLoad()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		FileDialog val = new FileDialog();
		val.Filters = new string[1] { "*.mcr" };
		val.UseNativeDialog = true;
		((Window)val).Title = "Choose Replay";
		val.Access = (AccessEnum)2;
		val.FileMode = (FileModeEnum)0;
		((GodotObject)val).Connect(SignalName.FileSelected, Callable.From<string>((Action<string>)LoadReplay), 0u);
		((Window)val).Show();
	}

	private void LoadReplay(string path)
	{
		using MemoryStream memoryStream = new MemoryStream();
		using (FileAccessStream fileAccessStream = new FileAccessStream(path, (ModeFlags)1))
		{
			fileAccessStream.CopyTo(memoryStream);
		}
		PacketReader packetReader = new PacketReader();
		packetReader.Reset(memoryStream.ToArray());
		CombatReplay replay = packetReader.Read<CombatReplay>();
		TaskHelper.RunSafely(RunReplay(replay));
	}

	private async Task RunReplay(CombatReplay replay)
	{
		Log.Info($"Loaded replay. Game version: {replay.version} Commit: {replay.gitCommit} Model ID hash: {replay.modelIdHash}");
		if (replay.modelIdHash != ModelIdSerializationCache.Hash)
		{
			if (!_ignoreReplayModelIdHash)
			{
				Log.Error($"Attempting to load replay with Model ID hash {replay.modelIdHash} that does not match ours ({ModelIdSerializationCache.Hash})! The replay will mismatch. If you want to continue anyway, try running the replay again.");
				_ignoreReplayModelIdHash = true;
				return;
			}
			Log.Warn("Ignoring model ID hash mismatch in replay.");
		}
		string text = ReleaseInfoManager.Instance.ReleaseInfo?.Commit ?? GitHelper.ShortCommitId;
		if (replay.gitCommit != text)
		{
			Log.Warn($"Git commit in replay {replay.gitCommit} does not match ours ({text}). The replay has a chance of mismatching!");
		}
		RunState runState = RunState.FromSerializable(replay.serializableRun);
		RunManager.Instance.SetUpReplay(runState, replay);
		RunManager.Instance.CombatStateSynchronizer.IsDisabled = true;
		await PreloadManager.LoadRunAssets(runState.Players.Select((Player p) => p.Character));
		await PreloadManager.LoadActAssets(runState.Act);
		RunManager.Instance.Launch();
		NAudioManager.Instance?.StopMusic();
		NGame.Instance.RootSceneContainer.SetCurrentScene((Control)(object)NRun.Create(runState));
		await RunManager.Instance.GenerateMap();
		RunManager.Instance.ActionQueueSet.FastForwardNextActionId(replay.nextActionId);
		RunManager.Instance.ActionQueueSynchronizer.FastForwardHookId(replay.nextHookId);
		RunManager.Instance.ChecksumTracker.LoadReplayChecksums(replay.checksumData, replay.nextChecksumId);
		RunManager.Instance.PlayerChoiceSynchronizer.FastForwardChoiceIds(replay.choiceIds);
		await RunManager.Instance.LoadIntoLatestMapCoord(AbstractRoom.FromSerializable(replay.serializableRun.PreFinishedRoom, runState));
		while (RunManager.Instance.ActionExecutor.IsPaused)
		{
			await ((GodotObject)Engine.GetMainLoop()).ToSignal((GodotObject)(object)Engine.GetMainLoop(), SignalName.ProcessFrame);
		}
		foreach (CombatReplayEvent replayEvent in replay.events)
		{
			switch (replayEvent.eventType)
			{
			case CombatReplayEventType.GameAction:
			{
				while (CombatManager.Instance.EndingPlayerTurnPhaseOne || CombatManager.Instance.EndingPlayerTurnPhaseTwo)
				{
					await new SignalAwaiter((GodotObject)(object)Engine.GetMainLoop(), SignalName.ProcessFrame, (GodotObject)(object)Engine.GetMainLoop());
				}
				Player player2 = runState.GetPlayer(replayEvent.playerId.Value);
				GameAction action = replayEvent.action.ToGameAction(player2);
				if (action.ActionType == GameActionType.CombatPlayPhaseOnly)
				{
					while (CombatManager.Instance.DebugOnlyGetState().CurrentSide == CombatSide.Enemy)
					{
						await new SignalAwaiter((GodotObject)(object)Engine.GetMainLoop(), SignalName.ProcessFrame, (GodotObject)(object)Engine.GetMainLoop());
					}
				}
				RunManager.Instance.ActionQueueSet.EnqueueWithoutSynchronizing(action);
				if ((action is EndPlayerTurnAction || action is ReadyToBeginEnemyTurnAction) ? true : false)
				{
					await RunManager.Instance.ActionExecutor.FinishedExecutingActions();
				}
				break;
			}
			case CombatReplayEventType.HookAction:
			{
				uint value = replayEvent.hookId.Value;
				GameActionType value2 = replayEvent.gameActionType.Value;
				RunManager.Instance.ActionQueueSet.EnqueueWithoutSynchronizing(RunManager.Instance.ActionQueueSynchronizer.GetHookActionForId(value, replayEvent.playerId.Value, value2));
				break;
			}
			case CombatReplayEventType.ResumeAction:
				RunManager.Instance.ActionQueueSet.ResumeActionWithoutSynchronizing(replayEvent.actionId.Value);
				break;
			case CombatReplayEventType.PlayerChoice:
			{
				Player player = runState.GetPlayer(replayEvent.playerId.Value);
				RunManager.Instance.PlayerChoiceSynchronizer.ReceiveReplayChoice(player, replayEvent.choiceId.Value, replayEvent.playerChoiceResult.Value);
				break;
			}
			default:
				throw new InvalidEnumArgumentException();
			}
		}
	}

	private void OnCharacterChanged(CharacterModel model)
	{
		_lobby.SetLocalCharacter(model);
		_localPlayerData.CharacterId = model.Id;
		_localPlayerData.CurrentHp = model.StartingHp;
		_localPlayerData.MaxHp = model.StartingHp;
		_localPlayerData.MaxEnergy = model.MaxEnergy;
		_localPlayerData.MaxPotionSlotCount = 3;
		_localPlayerData.Gold = model.StartingGold;
	}

	public override void _Process(double delta)
	{
		_lobby?.NetService.Update();
	}

	public void PlayerConnected(LobbyPlayer player)
	{
		_characterContainers[player.slotId].characterImage.Texture = player.character.IconTexture;
		_characterContainers[player.slotId].playerName.Text = PlatformUtil.GetPlayerName(_lobby.NetService.Platform, player.id);
	}

	public void PlayerChanged(LobbyPlayer player)
	{
		_characterContainers[player.slotId].characterImage.Texture = player.character.IconTexture;
		_characterContainers[player.slotId].playerName.Text = PlatformUtil.GetPlayerName(_lobby.NetService.Platform, player.id);
	}

	public void AscensionChanged()
	{
	}

	public void SeedChanged()
	{
	}

	public void ModifiersChanged()
	{
	}

	public void MaxAscensionChanged()
	{
	}

	public void RemotePlayerDisconnected(LobbyPlayer player)
	{
		_characterContainers[player.slotId].characterImage.Texture = null;
		_characterContainers[player.slotId].playerName.Text = "?";
	}

	public void LocalPlayerDisconnected(NetErrorInfo info)
	{
		_lobby = null;
		((CanvasItem)_characterPaginator).Visible = false;
		((CanvasItem)_readyButton).Visible = false;
		_characterPaginator.SetIndex(0);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_0296: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0325: Unknown result type (might be due to invalid IL or missing references)
		//IL_032e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0354: Unknown result type (might be due to invalid IL or missing references)
		//IL_035d: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(16);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._ExitTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AddGame, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.HostButtonPressed, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SteamHostButtonPressed, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.JoinButtonPressed, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ReadyButtonPressed, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.Disconnect, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("reason"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AfterMultiplayerStarted, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ChooseReplayToLoad, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.LoadReplay, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)4, StringName.op_Implicit("path"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Process, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)3, StringName.op_Implicit("delta"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AscensionChanged, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SeedChanged, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ModifiersChanged, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.MaxAscensionChanged, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0272: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._ExitTree && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._ExitTree();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.AddGame && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			AddGame();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.HostButtonPressed && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			HostButtonPressed();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SteamHostButtonPressed && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			SteamHostButtonPressed();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.JoinButtonPressed && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			JoinButtonPressed();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.ReadyButtonPressed && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			ReadyButtonPressed();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.Disconnect && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			Disconnect(VariantUtils.ConvertTo<NetError>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.AfterMultiplayerStarted && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			AfterMultiplayerStarted();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.ChooseReplayToLoad && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			ChooseReplayToLoad();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.LoadReplay && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			LoadReplay(VariantUtils.ConvertTo<string>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._Process && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			((Node)this)._Process(VariantUtils.ConvertTo<double>(ref ((NativeVariantPtrArgs)(ref args))[0]));
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
		if ((ref method) == MethodName.MaxAscensionChanged && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			MaxAscensionChanged();
			ret = default(godot_variant);
			return true;
		}
		return ((Control)this).InvokeGodotClassMethod(ref method, args, ref ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if ((ref method) == MethodName._Ready)
		{
			return true;
		}
		if ((ref method) == MethodName._ExitTree)
		{
			return true;
		}
		if ((ref method) == MethodName.AddGame)
		{
			return true;
		}
		if ((ref method) == MethodName.HostButtonPressed)
		{
			return true;
		}
		if ((ref method) == MethodName.SteamHostButtonPressed)
		{
			return true;
		}
		if ((ref method) == MethodName.JoinButtonPressed)
		{
			return true;
		}
		if ((ref method) == MethodName.ReadyButtonPressed)
		{
			return true;
		}
		if ((ref method) == MethodName.Disconnect)
		{
			return true;
		}
		if ((ref method) == MethodName.AfterMultiplayerStarted)
		{
			return true;
		}
		if ((ref method) == MethodName.ChooseReplayToLoad)
		{
			return true;
		}
		if ((ref method) == MethodName.LoadReplay)
		{
			return true;
		}
		if ((ref method) == MethodName._Process)
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
		if ((ref method) == MethodName.MaxAscensionChanged)
		{
			return true;
		}
		return ((Control)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._ipField)
		{
			_ipField = VariantUtils.ConvertTo<TextEdit>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._idField)
		{
			_idField = VariantUtils.ConvertTo<TextEdit>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._readyButton)
		{
			_readyButton = VariantUtils.ConvertTo<Button>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._readyIndicator)
		{
			_readyIndicator = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._loadingPanel)
		{
			_loadingPanel = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._characterPaginator)
		{
			_characterPaginator = VariantUtils.ConvertTo<NMultiplayerTestCharacterPaginator>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._game)
		{
			_game = VariantUtils.ConvertTo<NGame>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._ignoreReplayModelIdHash)
		{
			_ignoreReplayModelIdHash = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._beginningRun)
		{
			_beginningRun = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		return ((GodotObject)this).SetGodotClassPropertyValue(ref name, ref value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName._ipField)
		{
			value = VariantUtils.CreateFrom<TextEdit>(ref _ipField);
			return true;
		}
		if ((ref name) == PropertyName._idField)
		{
			value = VariantUtils.CreateFrom<TextEdit>(ref _idField);
			return true;
		}
		if ((ref name) == PropertyName._readyButton)
		{
			value = VariantUtils.CreateFrom<Button>(ref _readyButton);
			return true;
		}
		if ((ref name) == PropertyName._readyIndicator)
		{
			value = VariantUtils.CreateFrom<Control>(ref _readyIndicator);
			return true;
		}
		if ((ref name) == PropertyName._loadingPanel)
		{
			value = VariantUtils.CreateFrom<Control>(ref _loadingPanel);
			return true;
		}
		if ((ref name) == PropertyName._characterPaginator)
		{
			value = VariantUtils.CreateFrom<NMultiplayerTestCharacterPaginator>(ref _characterPaginator);
			return true;
		}
		if ((ref name) == PropertyName._game)
		{
			value = VariantUtils.CreateFrom<NGame>(ref _game);
			return true;
		}
		if ((ref name) == PropertyName._ignoreReplayModelIdHash)
		{
			value = VariantUtils.CreateFrom<bool>(ref _ignoreReplayModelIdHash);
			return true;
		}
		if ((ref name) == PropertyName._beginningRun)
		{
			value = VariantUtils.CreateFrom<bool>(ref _beginningRun);
			return true;
		}
		return ((GodotObject)this).GetGodotClassPropertyValue(ref name, ref value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._ipField, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._idField, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._readyButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._readyIndicator, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._loadingPanel, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._characterPaginator, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._game, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._ignoreReplayModelIdHash, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._beginningRun, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		((GodotObject)this).SaveGodotObjectData(info);
		info.AddProperty(PropertyName._ipField, Variant.From<TextEdit>(ref _ipField));
		info.AddProperty(PropertyName._idField, Variant.From<TextEdit>(ref _idField));
		info.AddProperty(PropertyName._readyButton, Variant.From<Button>(ref _readyButton));
		info.AddProperty(PropertyName._readyIndicator, Variant.From<Control>(ref _readyIndicator));
		info.AddProperty(PropertyName._loadingPanel, Variant.From<Control>(ref _loadingPanel));
		info.AddProperty(PropertyName._characterPaginator, Variant.From<NMultiplayerTestCharacterPaginator>(ref _characterPaginator));
		info.AddProperty(PropertyName._game, Variant.From<NGame>(ref _game));
		info.AddProperty(PropertyName._ignoreReplayModelIdHash, Variant.From<bool>(ref _ignoreReplayModelIdHash));
		info.AddProperty(PropertyName._beginningRun, Variant.From<bool>(ref _beginningRun));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._ipField, ref val))
		{
			_ipField = ((Variant)(ref val)).As<TextEdit>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._idField, ref val2))
		{
			_idField = ((Variant)(ref val2)).As<TextEdit>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._readyButton, ref val3))
		{
			_readyButton = ((Variant)(ref val3)).As<Button>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._readyIndicator, ref val4))
		{
			_readyIndicator = ((Variant)(ref val4)).As<Control>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._loadingPanel, ref val5))
		{
			_loadingPanel = ((Variant)(ref val5)).As<Control>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._characterPaginator, ref val6))
		{
			_characterPaginator = ((Variant)(ref val6)).As<NMultiplayerTestCharacterPaginator>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName._game, ref val7))
		{
			_game = ((Variant)(ref val7)).As<NGame>();
		}
		Variant val8 = default(Variant);
		if (info.TryGetProperty(PropertyName._ignoreReplayModelIdHash, ref val8))
		{
			_ignoreReplayModelIdHash = ((Variant)(ref val8)).As<bool>();
		}
		Variant val9 = default(Variant);
		if (info.TryGetProperty(PropertyName._beginningRun, ref val9))
		{
			_beginningRun = ((Variant)(ref val9)).As<bool>();
		}
	}
}
