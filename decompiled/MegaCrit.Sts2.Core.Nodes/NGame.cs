using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Audio.Debug;
using MegaCrit.Sts2.Core.AutoSlay;
using MegaCrit.Sts2.Core.ControllerInput;
using MegaCrit.Sts2.Core.Debug;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Leaderboard;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Multiplayer.Game;
using MegaCrit.Sts2.Core.Multiplayer.Game.Lobby;
using MegaCrit.Sts2.Core.Nodes.Audio;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.Cards.Holders;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.Debug;
using MegaCrit.Sts2.Core.Nodes.Multiplayer;
using MegaCrit.Sts2.Core.Nodes.Reaction;
using MegaCrit.Sts2.Core.Nodes.Screens;
using MegaCrit.Sts2.Core.Nodes.Screens.FeedbackScreen;
using MegaCrit.Sts2.Core.Nodes.Screens.InspectScreens;
using MegaCrit.Sts2.Core.Nodes.Screens.MainMenu;
using MegaCrit.Sts2.Core.Nodes.Screens.Settings;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Nodes.Vfx.Utilities;
using MegaCrit.Sts2.Core.Platform;
using MegaCrit.Sts2.Core.Platform.Steam;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.Settings;
using MegaCrit.Sts2.Core.Unlocks;

namespace MegaCrit.Sts2.Core.Nodes;

[ScriptPath("res://src/Core/Nodes/NGame.cs")]
public class NGame : Control
{
	[Signal]
	public delegate void WindowChangeEventHandler();

	public class MethodName : MethodName
	{
		public static readonly StringName _EnterTree = StringName.op_Implicit("_EnterTree");

		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName OnWindowChange = StringName.op_Implicit("OnWindowChange");

		public static readonly StringName IsMainThread = StringName.op_Implicit("IsMainThread");

		public static readonly StringName _ExitTree = StringName.op_Implicit("_ExitTree");

		public static readonly StringName IsReleaseGame = StringName.op_Implicit("IsReleaseGame");

		public static readonly StringName InitializeGraphicsPreferences = StringName.op_Implicit("InitializeGraphicsPreferences");

		public static readonly StringName ApplyDisplaySettings = StringName.op_Implicit("ApplyDisplaySettings");

		public static readonly StringName GetInspectRelicScreen = StringName.op_Implicit("GetInspectRelicScreen");

		public static readonly StringName GetInspectCardScreen = StringName.op_Implicit("GetInspectCardScreen");

		public static readonly StringName ApplySyncSetting = StringName.op_Implicit("ApplySyncSetting");

		public static readonly StringName Reset = StringName.op_Implicit("Reset");

		public static readonly StringName _Notification = StringName.op_Implicit("_Notification");

		public static readonly StringName Quit = StringName.op_Implicit("Quit");

		public static readonly StringName Relocalize = StringName.op_Implicit("Relocalize");

		public static readonly StringName ReloadMainMenu = StringName.op_Implicit("ReloadMainMenu");

		public static readonly StringName _Input = StringName.op_Implicit("_Input");

		public static readonly StringName ToggleFullscreen = StringName.op_Implicit("ToggleFullscreen");

		public static readonly StringName DebugModifyTimescale = StringName.op_Implicit("DebugModifyTimescale");

		public static readonly StringName ActivateWorldEnvironment = StringName.op_Implicit("ActivateWorldEnvironment");

		public static readonly StringName DeactivateWorldEnvironment = StringName.op_Implicit("DeactivateWorldEnvironment");

		public static readonly StringName SetScreenShakeTarget = StringName.op_Implicit("SetScreenShakeTarget");

		public static readonly StringName ClearScreenShakeTarget = StringName.op_Implicit("ClearScreenShakeTarget");

		public static readonly StringName ScreenShake = StringName.op_Implicit("ScreenShake");

		public static readonly StringName ScreenRumble = StringName.op_Implicit("ScreenRumble");

		public static readonly StringName ScreenShakeTrauma = StringName.op_Implicit("ScreenShakeTrauma");

		public static readonly StringName DoHitStop = StringName.op_Implicit("DoHitStop");

		public static readonly StringName ToggleTrailerMode = StringName.op_Implicit("ToggleTrailerMode");

		public static readonly StringName SetScreenshakeMultiplier = StringName.op_Implicit("SetScreenshakeMultiplier");

		public static readonly StringName InitPools = StringName.op_Implicit("InitPools");

		public static readonly StringName CheckShowLocalizationOverrideErrors = StringName.op_Implicit("CheckShowLocalizationOverrideErrors");

		public static readonly StringName LogResourceStats = StringName.op_Implicit("LogResourceStats");

		public static readonly StringName FormatBytes = StringName.op_Implicit("FormatBytes");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName RootSceneContainer = StringName.op_Implicit("RootSceneContainer");

		public static readonly StringName HoverTipsContainer = StringName.op_Implicit("HoverTipsContainer");

		public static readonly StringName MainMenu = StringName.op_Implicit("MainMenu");

		public static readonly StringName CurrentRunNode = StringName.op_Implicit("CurrentRunNode");

		public static readonly StringName LogoAnimation = StringName.op_Implicit("LogoAnimation");

		public static readonly StringName Transition = StringName.op_Implicit("Transition");

		public static readonly StringName TimeoutOverlay = StringName.op_Implicit("TimeoutOverlay");

		public static readonly StringName AudioManager = StringName.op_Implicit("AudioManager");

		public static readonly StringName RemoteCursorContainer = StringName.op_Implicit("RemoteCursorContainer");

		public static readonly StringName InputManager = StringName.op_Implicit("InputManager");

		public static readonly StringName HotkeyManager = StringName.op_Implicit("HotkeyManager");

		public static readonly StringName ReactionWheel = StringName.op_Implicit("ReactionWheel");

		public static readonly StringName ReactionContainer = StringName.op_Implicit("ReactionContainer");

		public static readonly StringName CursorManager = StringName.op_Implicit("CursorManager");

		public static readonly StringName DebugAudio = StringName.op_Implicit("DebugAudio");

		public static readonly StringName DebugSeedOverride = StringName.op_Implicit("DebugSeedOverride");

		public static readonly StringName StartOnMainMenu = StringName.op_Implicit("StartOnMainMenu");

		public static readonly StringName InspectRelicScreen = StringName.op_Implicit("InspectRelicScreen");

		public static readonly StringName InspectCardScreen = StringName.op_Implicit("InspectCardScreen");

		public static readonly StringName FeedbackScreen = StringName.op_Implicit("FeedbackScreen");

		public static readonly StringName WorldEnvironment = StringName.op_Implicit("WorldEnvironment");

		public static readonly StringName HitStop = StringName.op_Implicit("HitStop");

		public static readonly StringName _inspectionContainer = StringName.op_Implicit("_inspectionContainer");

		public static readonly StringName _screenShake = StringName.op_Implicit("_screenShake");
	}

	public class SignalName : SignalName
	{
		public static readonly StringName WindowChange = StringName.op_Implicit("WindowChange");
	}

	public static readonly Vector2 devResolution = new Vector2(1920f, 1080f);

	private Control _inspectionContainer;

	private NScreenShake _screenShake;

	private static int? _mainThreadId;

	private static Window _window = null;

	private CancellationTokenSource? _logoCancelToken;

	private SteamJoinCallbackHandler? _joinCallbackHandler;

	private WindowChangeEventHandler backing_WindowChange;

	public static NGame? Instance { get; private set; }

	public NSceneContainer RootSceneContainer { get; private set; }

	public Node? HoverTipsContainer { get; private set; }

	public NMainMenu? MainMenu => RootSceneContainer.CurrentScene as NMainMenu;

	public NRun? CurrentRunNode => RootSceneContainer.CurrentScene as NRun;

	public NLogoAnimation? LogoAnimation => RootSceneContainer.CurrentScene as NLogoAnimation;

	public NTransition Transition { get; private set; }

	public NMultiplayerTimeoutOverlay TimeoutOverlay { get; private set; }

	public NAudioManager AudioManager { get; private set; }

	public NRemoteMouseCursorContainer RemoteCursorContainer { get; private set; }

	public NInputManager InputManager { get; private set; }

	public NHotkeyManager HotkeyManager { get; private set; }

	public NReactionWheel ReactionWheel { get; private set; }

	public NReactionContainer ReactionContainer { get; private set; }

	public NCursorManager CursorManager { get; private set; }

	public NDebugAudioManager DebugAudio { get; private set; }

	public string? DebugSeedOverride { get; set; }

	public bool StartOnMainMenu { get; set; } = true;

	public static bool IsTrailerMode { get; private set; }

	public static bool IsDebugHidingHoverTips { get; private set; }

	public static bool IsDebugHidingProceedButton { get; private set; }

	public NInspectRelicScreen? InspectRelicScreen { get; set; }

	public NInspectCardScreen? InspectCardScreen { get; set; }

	public NSendFeedbackScreen FeedbackScreen { get; set; }

	private WorldEnvironment WorldEnvironment { get; set; }

	private NHitStop HitStop { get; set; }

	public event Action? DebugToggleProceedButton;

	public event WindowChangeEventHandler WindowChange
	{
		add
		{
			backing_WindowChange = (WindowChangeEventHandler)Delegate.Combine(backing_WindowChange, value);
		}
		remove
		{
			backing_WindowChange = (WindowChangeEventHandler)Delegate.Remove(backing_WindowChange, value);
		}
	}

	public override void _EnterTree()
	{
		if (Instance != null)
		{
			Log.Error("NGame already exists.");
			((Node)(object)this).QueueFreeSafely();
			return;
		}
		Instance = this;
		SentryService.Initialize();
		RootSceneContainer = ((Node)this).GetNode<NSceneContainer>(NodePath.op_Implicit("%RootSceneContainer"));
		HoverTipsContainer = ((Node)this).GetNode<Node>(NodePath.op_Implicit("%HoverTipsContainer"));
		DebugAudio = ((Node)this).GetNode<NDebugAudioManager>(NodePath.op_Implicit("%DebugAudioManager"));
		AudioManager = ((Node)this).GetNode<NAudioManager>(NodePath.op_Implicit("%AudioManager"));
		RemoteCursorContainer = ((Node)this).GetNode<NRemoteMouseCursorContainer>(NodePath.op_Implicit("%RemoteCursorContainer"));
		InputManager = ((Node)this).GetNode<NInputManager>(NodePath.op_Implicit("%InputManager"));
		CursorManager = ((Node)this).GetNode<NCursorManager>(NodePath.op_Implicit("%CursorManager"));
		ReactionWheel = ((Node)this).GetNode<NReactionWheel>(NodePath.op_Implicit("%ReactionWheel"));
		ReactionContainer = ((Node)this).GetNode<NReactionContainer>(NodePath.op_Implicit("%ReactionContainer"));
		TimeoutOverlay = ((Node)this).GetNode<NMultiplayerTimeoutOverlay>(NodePath.op_Implicit("%MultiplayerTimeoutOverlay"));
		WorldEnvironment = ((Node)this).GetNode<WorldEnvironment>(NodePath.op_Implicit("%WorldEnvironment"));
		HotkeyManager = ((Node)this).GetNode<NHotkeyManager>(NodePath.op_Implicit("%HotkeyManager"));
		_inspectionContainer = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%InspectionContainer"));
		_screenShake = ((Node)this).GetNode<NScreenShake>(NodePath.op_Implicit("ScreenShake"));
		HitStop = ((Node)this).GetNode<NHitStop>(NodePath.op_Implicit("HitStop"));
		Transition = ((Node)this).GetNode<NTransition>(NodePath.op_Implicit("%GameTransitionRect"));
		FeedbackScreen = ((Node)this).GetNode<NSendFeedbackScreen>(NodePath.op_Implicit("%FeedbackScreen"));
		_mainThreadId = Environment.CurrentManagedThreadId;
		TaskHelper.RunSafely(GameStartupWrapper());
	}

	public override void _Ready()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		_window = ((Node)this).GetTree().Root;
		((GodotObject)_window).Connect(SignalName.SizeChanged, Callable.From((Action)OnWindowChange), 0u);
		((Node)(object)this).RemoveChildSafely((Node?)(object)WorldEnvironment);
	}

	private async Task GameStartupWrapper()
	{
		if (!(await InitializePlatform()))
		{
			return;
		}
		TaskHelper.RunSafely(OsDebugInfo.LogSystemInfo());
		TaskHelper.RunSafely(GitHelper.Initialize());
		try
		{
			await GameStartup();
		}
		catch
		{
			TaskHelper.RunSafely(GameStartupError());
			throw;
		}
	}

	private async Task TryErrorInit()
	{
		try
		{
			if (SaveManager.Instance.SettingsSave == null)
			{
				SaveManager.Instance.InitSettingsData();
			}
			if (LocManager.Instance == null)
			{
				LocManager.Initialize();
			}
		}
		catch (Exception value)
		{
			Log.Error($"Failed to show error dialog! Exception: {value}");
			((Node)this).GetTree().Quit(0);
			throw;
		}
		if (!((Node)this).IsNodeReady())
		{
			await ((GodotObject)this).ToSignal((GodotObject)(object)this, SignalName.Ready);
		}
		((CanvasItem)Transition).Visible = false;
	}

	private async Task GameStartupError()
	{
		Log.Error("Encountered error on game startup! Attempting to show error dialog");
		await TryErrorInit();
		NGenericPopup nGenericPopup = NGenericPopup.Create();
		NModalContainer.Instance.Add((Node)(object)nGenericPopup);
		await nGenericPopup.WaitForConfirmation(new LocString("main_menu_ui", "STARTUP_ERROR.description"), new LocString("main_menu_ui", "STARTUP_ERROR.title"), null, new LocString("main_menu_ui", "QUIT"));
		((Node)this).GetTree().Quit(0);
	}

	private async Task GameStartup()
	{
		AccountScopeUserDataMigrator.MigrateToUserScopedDirectories();
		AccountScopeUserDataMigrator.ArchiveLegacyData();
		ProfileAccountScopeMigrator.MigrateToProfileScopedDirectories();
		ProfileAccountScopeMigrator.ArchiveLegacyData();
		bool flag = await SaveManager.Instance.TryFirstTimeCloudSync();
		Task cloudSavesTask = null;
		if (!flag)
		{
			cloudSavesTask = Task.Run((Func<Task?>)SaveManager.Instance.SyncCloudToLocal);
		}
		InitPools();
		OneTimeInitialization.ExecuteEssential();
		if (!((Node)this).IsNodeReady())
		{
			await ((GodotObject)this).ToSignal((GodotObject)(object)this, SignalName.Ready);
		}
		Callable val = Callable.From((Action)InitializeGraphicsPreferences);
		((Callable)(ref val)).CallDeferred(Array.Empty<Variant>());
		AudioManager.SetMasterVol(SaveManager.Instance.SettingsSave.VolumeMaster);
		AudioManager.SetSfxVol(SaveManager.Instance.SettingsSave.VolumeSfx);
		AudioManager.SetAmbienceVol(SaveManager.Instance.SettingsSave.VolumeAmbience);
		AudioManager.SetBgmVol(SaveManager.Instance.SettingsSave.VolumeBgm);
		DebugAudio.SetMasterAudioVolume(SaveManager.Instance.SettingsSave.VolumeMaster);
		DebugAudio.SetSfxAudioVolume(SaveManager.Instance.SettingsSave.VolumeSfx);
		LeaderboardManager.Initialize();
		SteamStatsManager.Initialize();
		if (cloudSavesTask != null)
		{
			await cloudSavesTask;
		}
		SaveManager.Instance.InitProfileId();
		ReadSaveResult<SerializableProgress> progressReadResult = SaveManager.Instance.InitProgressData();
		ReadSaveResult<PrefsSave> prefsReadResult = SaveManager.Instance.InitPrefsData();
		SentryService.SetUserContext(SaveManager.Instance.Progress.UniqueId);
		string platformBranch = PlatformUtil.GetPlatformBranch();
		if (platformBranch != null)
		{
			SentryService.SetTag("platform.branch", platformBranch);
		}
		_screenShake.SetMultiplier(NScreenshakePaginator.GetShakeMultiplier(SaveManager.Instance.PrefsSave.ScreenShakeOptionIndex));
		if (!OS.HasFeature("editor") && SaveManager.Instance.PrefsSave.FastMode == FastModeType.Instant)
		{
			SaveManager.Instance.PrefsSave.FastMode = FastModeType.Fast;
		}
		if (!IsReleaseGame() && CommandLineHelper.HasArg("autoslay"))
		{
			await LaunchMainMenu(skipLogo: true);
			string seed = CommandLineHelper.GetValue("seed") ?? SeedHelper.GetRandomSeed();
			string value = CommandLineHelper.GetValue("log-file");
			AutoSlayer autoSlayer = new AutoSlayer();
			autoSlayer.Start(seed, value);
		}
		else if (CommandLineHelper.HasArg("bootstrap"))
		{
			NSceneBootstrapper child = SceneHelper.Instantiate<NSceneBootstrapper>("debug/scene_bootstrapper");
			((Node)(object)this).AddChildSafely((Node?)(object)child);
		}
		else if (StartOnMainMenu)
		{
			bool skipLogo = DebugSettings.DevSkip || SaveManager.Instance.SettingsSave.SkipIntroLogo || CommandLineHelper.HasArg("fastmp");
			await LaunchMainMenu(skipLogo);
			CheckShowSaveFileError(progressReadResult, prefsReadResult, OneTimeInitialization.SettingsReadResult);
			CheckShowLocalizationOverrideErrors();
		}
		ModManager.OnModDetected += OnNewModDetected;
	}

	private void OnWindowChange()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		Log.Info($"Window changed! New size: {DisplayServer.WindowGetSize(0)}");
		((GodotObject)this).EmitSignal(SignalName.WindowChange, (Variant[])(object)new Variant[1] { Variant.op_Implicit(SaveManager.Instance.SettingsSave.AspectRatioSetting == AspectRatioSetting.Auto) });
	}

	public static bool IsMainThread()
	{
		if (!_mainThreadId.HasValue)
		{
			_mainThreadId = Environment.CurrentManagedThreadId;
			return true;
		}
		return _mainThreadId == Environment.CurrentManagedThreadId;
	}

	public override void _ExitTree()
	{
		ModManager.OnModDetected -= OnNewModDetected;
		ModManager.Dispose();
		_joinCallbackHandler?.Dispose();
		SteamInitializer.Uninitialize();
		SentryService.Shutdown();
	}

	public static bool IsReleaseGame()
	{
		return true;
	}

	private void InitializeGraphicsPreferences()
	{
		if (!DisplayServer.GetName().Equals("headless", StringComparison.OrdinalIgnoreCase))
		{
			ApplyDisplaySettings();
			ApplySyncSetting();
		}
		Engine.MaxFps = SaveManager.Instance.SettingsSave.FpsLimit;
	}

	public void ApplyDisplaySettings()
	{
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_0397: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0420: Unknown result type (might be due to invalid IL or missing references)
		//IL_0425: Unknown result type (might be due to invalid IL or missing references)
		//IL_043f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0476: Unknown result type (might be due to invalid IL or missing references)
		//IL_0496: Unknown result type (might be due to invalid IL or missing references)
		//IL_0498: Unknown result type (might be due to invalid IL or missing references)
		//IL_049a: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_052b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0530: Unknown result type (might be due to invalid IL or missing references)
		//IL_0532: Unknown result type (might be due to invalid IL or missing references)
		//IL_0510: Unknown result type (might be due to invalid IL or missing references)
		//IL_0513: Unknown result type (might be due to invalid IL or missing references)
		//IL_0518: Unknown result type (might be due to invalid IL or missing references)
		//IL_051b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0520: Unknown result type (might be due to invalid IL or missing references)
		//IL_053c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0764: Unknown result type (might be due to invalid IL or missing references)
		//IL_0769: Unknown result type (might be due to invalid IL or missing references)
		//IL_076e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0546: Unknown result type (might be due to invalid IL or missing references)
		//IL_054d: Unknown result type (might be due to invalid IL or missing references)
		//IL_061a: Unknown result type (might be due to invalid IL or missing references)
		//IL_061f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0621: Unknown result type (might be due to invalid IL or missing references)
		//IL_0628: Unknown result type (might be due to invalid IL or missing references)
		//IL_05cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_05dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0604: Unknown result type (might be due to invalid IL or missing references)
		//IL_0556: Unknown result type (might be due to invalid IL or missing references)
		//IL_055d: Unknown result type (might be due to invalid IL or missing references)
		//IL_063f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0646: Unknown result type (might be due to invalid IL or missing references)
		//IL_0633: Unknown result type (might be due to invalid IL or missing references)
		//IL_0675: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_06bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_06df: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_06eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_070f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0728: Unknown result type (might be due to invalid IL or missing references)
		//IL_0651: Unknown result type (might be due to invalid IL or missing references)
		bool flag = false;
		SettingsSave settingsSave = SaveManager.Instance.SettingsSave;
		if (settingsSave.TargetDisplay == -1)
		{
			Log.Info("First time setup for display settings...");
			settingsSave.TargetDisplay = DisplayServer.GetPrimaryScreen();
		}
		bool flag2 = settingsSave.Fullscreen;
		if (PlatformUtil.GetSupportedWindowMode().ShouldForceFullscreen())
		{
			if (!flag2)
			{
				Log.Warn($"Settings has fullscreen set to false, but we're forcing fullscreen because the platform reports our supported window mode as {PlatformUtil.GetSupportedWindowMode()}");
			}
			flag2 = true;
		}
		Log.Info($"Applying display settings...\n  FULLSCREEN: {flag2}\n  ASPECT_RATIO: ({settingsSave.AspectRatioSetting})\n  TARGET_DISPLAY: ({settingsSave.TargetDisplay})\n  WINDOW_SIZE: {settingsSave.WindowSize}\n  POSITION: {settingsSave.WindowPosition}");
		Log.Info($"[Display] Min size: {DisplayServer.WindowGetMinSize(0)} Max size: {DisplayServer.WindowGetMaxSize(0)}");
		if (settingsSave.AspectRatioSetting != AspectRatioSetting.Auto)
		{
			_window.ContentScaleAspect = (ContentScaleAspectEnum)1;
		}
		switch (settingsSave.AspectRatioSetting)
		{
		case AspectRatioSetting.Auto:
			flag = true;
			break;
		case AspectRatioSetting.FourByThree:
			_window.ContentScaleSize = new Vector2I(1680, 1260);
			break;
		case AspectRatioSetting.SixteenByTen:
			_window.ContentScaleSize = new Vector2I(1920, 1200);
			break;
		case AspectRatioSetting.SixteenByNine:
			_window.ContentScaleSize = new Vector2I(1920, 1080);
			break;
		case AspectRatioSetting.TwentyOneByNine:
			_window.ContentScaleSize = new Vector2I(2580, 1080);
			break;
		default:
			throw new ArgumentOutOfRangeException($"Invalid Aspect Ratio: {settingsSave.AspectRatioSetting}");
		}
		int num = ListExtensions.IndexOf(Environment.GetCommandLineArgs(), "-wpos");
		if (flag2 && num < 0)
		{
			if (_window.Unresizable)
			{
				_window.Unresizable = false;
			}
			Log.Info($"[Display] Setting FULLSCREEN on Display: {settingsSave.TargetDisplay + 1} of {DisplayServer.GetScreenCount()}");
			if (settingsSave.TargetDisplay >= DisplayServer.GetScreenCount())
			{
				Log.Warn($"[Display] FAILED: Display {settingsSave.TargetDisplay} is missing. Fallback to primary.");
				DisplayServer.WindowSetCurrentScreen(DisplayServer.GetPrimaryScreen(), 0);
				settingsSave.TargetDisplay = DisplayServer.GetPrimaryScreen();
			}
			else
			{
				DisplayServer.WindowSetCurrentScreen(settingsSave.TargetDisplay, 0);
			}
			DisplayServer.WindowSetMode((WindowMode)3, 0);
		}
		else
		{
			Log.Info($"[Display] Attempting WINDOWED mode on Display {settingsSave.TargetDisplay + 1} of {DisplayServer.GetScreenCount()} at position {settingsSave.WindowPosition}");
			DisplayServer.WindowSetMode((WindowMode)0, 0);
			if (_window.Unresizable != !settingsSave.ResizeWindows)
			{
				_window.Unresizable = !settingsSave.ResizeWindows;
			}
			if (num >= 0)
			{
				Log.Info("[Display] -wpos called. Applying special logic.");
				settingsSave.Fullscreen = false;
				Vector2I val = default(Vector2I);
				((Vector2I)(ref val))._002Ector(int.Parse(Environment.GetCommandLineArgs()[num + 1]), int.Parse(Environment.GetCommandLineArgs()[num + 2]));
				Vector2I val2 = DisplayServer.ScreenGetPosition(DisplayServer.WindowGetCurrentScreen(0));
				Log.Info($"Applying window position from command line arg: {val} ({string.Join(",", Environment.GetCommandLineArgs())} {val2})");
				DisplayServer.WindowSetPosition(val2 + val, 0);
				DisplayServer.WindowSetSize(settingsSave.WindowSize, 0);
			}
			else
			{
				Vector2I val3 = DisplayServer.ScreenGetSize(settingsSave.TargetDisplay);
				Vector2I windowSize = settingsSave.WindowSize;
				if (settingsSave.WindowPosition == new Vector2I(-1, -1))
				{
					Log.Info($"[Display] Going from fullscreen to windowed. Attempting to center window on screen {settingsSave.TargetDisplay}");
					settingsSave.WindowPosition = val3 / 2 - windowSize / 2;
				}
				Vector2I windowPosition = settingsSave.WindowPosition;
				if (windowPosition.X < 0 || windowPosition.Y < 0 || windowPosition.X > val3.X || windowPosition.Y > val3.Y)
				{
					Log.Warn("[Display] WARN: Game Window was offscreen. Resetting to top left corner.");
					((Vector2I)(ref windowPosition))._002Ector(8, 48);
				}
				if (settingsSave.TargetDisplay >= DisplayServer.GetScreenCount())
				{
					Log.Info($"[Display] FAILED: Display {settingsSave.TargetDisplay + 1} is missing. Fallback to primary.");
					settingsSave.WindowPosition = new Vector2I(8, 48);
					DisplayServer.WindowSetSize(DisplayServer.ScreenGetSize(DisplayServer.GetPrimaryScreen()) - new Vector2I(8, 48), 0);
					DisplayServer.WindowSetPosition(DisplayServer.ScreenGetPosition(DisplayServer.GetPrimaryScreen()) + settingsSave.WindowPosition, 0);
				}
				else
				{
					Vector2I val4 = DisplayServer.ScreenGetPosition(settingsSave.TargetDisplay);
					if (windowSize.X > val3.X)
					{
						windowSize.X = val3.X;
					}
					if (windowSize.Y > val3.Y)
					{
						windowSize.Y = val3.Y;
					}
					Log.Info($"[Display] SUCCESS: {windowSize} Windowed mode in Display {settingsSave.TargetDisplay}: Position {val4 + windowPosition} ({windowPosition})");
					DisplayServer.WindowSetSize(windowSize, 0);
					DisplayServer.WindowSetPosition(val4 + windowPosition, 0);
					Log.Info($"[Display] New size: {DisplayServer.WindowGetSize(0)} position: {DisplayServer.WindowGetPosition(0)}");
				}
			}
		}
		if (flag)
		{
			Log.Info("Manual window change signal because of auto scaling");
			((GodotObject)this).EmitSignal(SignalName.WindowChange, (Variant[])(object)new Variant[1] { Variant.op_Implicit(settingsSave.AspectRatioSetting == AspectRatioSetting.Auto) });
		}
	}

	public NInspectRelicScreen GetInspectRelicScreen()
	{
		if (InspectRelicScreen == null)
		{
			InspectRelicScreen = NInspectRelicScreen.Create();
			((Node)(object)_inspectionContainer).AddChildSafely((Node?)(object)InspectRelicScreen);
		}
		return InspectRelicScreen;
	}

	public NInspectCardScreen GetInspectCardScreen()
	{
		if (InspectCardScreen == null)
		{
			InspectCardScreen = NInspectCardScreen.Create();
			((Node)(object)_inspectionContainer).AddChildSafely((Node?)(object)InspectCardScreen);
		}
		return InspectCardScreen;
	}

	public static void ApplySyncSetting()
	{
		switch (SaveManager.Instance.SettingsSave.VSync)
		{
		case VSyncType.Off:
			Log.Info("VSync: Off");
			DisplayServer.WindowSetVsyncMode((VSyncMode)0, 0);
			break;
		case VSyncType.On:
			Log.Info("VSync: On");
			DisplayServer.WindowSetVsyncMode((VSyncMode)1, 0);
			break;
		case VSyncType.Adaptive:
			Log.Info("VSync: Adaptive");
			DisplayServer.WindowSetVsyncMode((VSyncMode)2, 0);
			break;
		default:
			Log.Error($"Invalid VSync type: {SaveManager.Instance.SettingsSave.VSync}");
			break;
		}
	}

	public static void Reset()
	{
		((Node)(object)Instance)?.QueueFreeSafely();
		Instance = null;
	}

	public override void _Notification(int what)
	{
		if ((long)what == 1006)
		{
			Quit();
		}
	}

	public void Quit()
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		Log.Info("NGame.Quit called");
		if (!PlatformUtil.GetSupportedWindowMode().ShouldForceFullscreen() && !SaveManager.Instance.SettingsSave.Fullscreen)
		{
			SettingsSave settingsSave = SaveManager.Instance.SettingsSave;
			settingsSave.WindowSize = DisplayServer.WindowGetSize(0);
			settingsSave.TargetDisplay = DisplayServer.WindowGetCurrentScreen(0);
			settingsSave.WindowPosition = DisplayServer.WindowGetPosition(0) - DisplayServer.ScreenGetPosition(SaveManager.Instance.SettingsSave.TargetDisplay);
			Log.Info($"[Display] On exit, saving window size: {settingsSave.WindowSize} display: {settingsSave.TargetDisplay} position: {settingsSave.WindowPosition}");
		}
		SaveManager.Instance.SaveSettings();
		SaveManager.Instance.SavePrefsFile();
		SaveManager.Instance.SaveProgressFile();
		SaveManager.Instance.SaveProfile();
		((Node)this).GetTree().Quit(0);
	}

	private async Task LaunchMainMenu(bool skipLogo)
	{
		NLogoAnimation logoAnimation = null;
		if (skipLogo)
		{
			await PreloadManager.LoadMainMenuEssentials();
		}
		else
		{
			await PreloadManager.LoadLogoAnimation();
			logoAnimation = NLogoAnimation.Create();
			RootSceneContainer.SetCurrentScene((Control)(object)logoAnimation);
			await PreloadManager.LoadMainMenuEssentials();
		}
		if (logoAnimation != null)
		{
			_logoCancelToken = new CancellationTokenSource();
			await Transition.FadeIn(0.8f, "res://materials/transitions/fade_transition_mat.tres", _logoCancelToken.Token);
			await logoAnimation.PlayAnimation(_logoCancelToken.Token);
			await Transition.FadeOut();
		}
		await LoadMainMenu();
		Log.Info($"[Startup] Time to main menu: {Time.GetTicksMsec():N0}ms");
		LogResourceStats("main menu loaded (essential)");
		TaskHelper.RunSafely(LoadDeferredStartupAssetsAsync());
		_joinCallbackHandler?.CheckForCommandLineJoin();
	}

	private async Task LoadDeferredStartupAssetsAsync()
	{
		OneTimeInitialization.ExecuteDeferred();
		await PreloadManager.LoadCommonAndMainMenuAssets();
		LogResourceStats("main menu loaded (complete)");
	}

	public async Task GoToTimelineAfterRun()
	{
		await GoToTimeline();
	}

	public async Task ReturnToMainMenuAfterRun()
	{
		await ReturnToMainMenu();
	}

	public async Task GoToTimeline()
	{
		await Transition.FadeOut();
		await PreloadManager.LoadCommonAndMainMenuAssets();
		RunManager.Instance.CleanUp();
		await LoadMainMenu(openTimeline: true);
	}

	public async Task ReturnToMainMenu()
	{
		await Transition.FadeOut();
		await PreloadManager.LoadCommonAndMainMenuAssets();
		RunManager.Instance.CleanUp();
		await LoadMainMenu();
	}

	public void Relocalize()
	{
		ReloadMainMenu();
		FeedbackScreen.Relocalize();
		TimeoutOverlay.Relocalize();
	}

	public void ReloadMainMenu()
	{
		if (MainMenu == null)
		{
			throw new InvalidOperationException("Tried to reload main menu when not already on the main menu!");
		}
		TaskHelper.RunSafely(LoadMainMenu());
	}

	private async Task LoadMainMenu(bool openTimeline = false)
	{
		Task currentRunSaveTask = SaveManager.Instance.CurrentRunSaveTask;
		if (currentRunSaveTask != null)
		{
			Log.Info("Saving in progress, waiting for it to be finished before loading the main menu");
			try
			{
				await currentRunSaveTask;
			}
			catch (Exception value)
			{
				Log.Error($"Save task failed while waiting to load main menu: {value}");
			}
		}
		NMainMenu currentScene = NMainMenu.Create(openTimeline);
		RootSceneContainer.SetCurrentScene((Control)(object)currentScene);
	}

	public async Task<RunState> StartNewSingleplayerRun(CharacterModel character, bool shouldSave, IReadOnlyList<ActModel> acts, IReadOnlyList<ModifierModel> modifiers, string seed, int ascensionLevel = 0, DateTimeOffset? dailyTime = null)
	{
		UnlockState unlockState = SaveManager.Instance.GenerateUnlockStateFromProgress();
		RunState runState = RunState.CreateForNewRun(new _003C_003Ez__ReadOnlySingleElementList<Player>(Player.CreateForNewRun(character, unlockState, 1uL)), acts.Select((ActModel a) => a.ToMutable()).ToList(), modifiers, ascensionLevel, seed);
		RunManager.Instance.SetUpNewSinglePlayer(runState, shouldSave, dailyTime);
		await StartRun(runState);
		return runState;
	}

	public async Task<RunState> StartNewMultiplayerRun(StartRunLobby lobby, bool shouldSave, IReadOnlyList<ActModel> acts, IReadOnlyList<ModifierModel> modifiers, string seed, int ascensionLevel, DateTimeOffset? dailyTime = null)
	{
		RunState runState = RunState.CreateForNewRun(lobby.Players.Select((LobbyPlayer p) => Player.CreateForNewRun(p.character, UnlockState.FromSerializable(p.unlockState), p.id)).ToList(), acts.Select((ActModel a) => a.ToMutable()).ToList(), modifiers, ascensionLevel, seed);
		RunManager.Instance.SetUpNewMultiPlayer(runState, lobby, shouldSave, dailyTime);
		await StartRun(runState);
		return runState;
	}

	public async Task LoadRun(RunState runState, SerializableRoom? preFinishedRoom)
	{
		await PreloadManager.LoadRunAssets(runState.Players.Select((Player p) => p.Character));
		await PreloadManager.LoadActAssets(runState.Act);
		RunManager.Instance.Launch();
		RootSceneContainer.SetCurrentScene((Control)(object)NRun.Create(runState));
		await RunManager.Instance.GenerateMap();
		await RunManager.Instance.LoadIntoLatestMapCoord(AbstractRoom.FromSerializable(preFinishedRoom, runState));
		if (RunManager.Instance.MapDrawingsToLoad != null)
		{
			NRun.Instance.GlobalUi.MapScreen.Drawings.LoadDrawings(RunManager.Instance.MapDrawingsToLoad);
			RunManager.Instance.MapDrawingsToLoad = null;
		}
	}

	private async Task StartRun(RunState runState)
	{
		using (new NetLoadingHandle(RunManager.Instance.NetService))
		{
			await PreloadManager.LoadRunAssets(runState.Players.Select((Player p) => p.Character));
			await PreloadManager.LoadActAssets(runState.Acts[0]);
			await RunManager.Instance.FinalizeStartingRelics();
			RunManager.Instance.Launch();
			RootSceneContainer.SetCurrentScene((Control)(object)NRun.Create(runState));
			await RunManager.Instance.EnterAct(0, doTransition: false);
		}
	}

	public override void _Input(InputEvent inputEvent)
	{
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Invalid comparison between Unknown and I8
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Invalid comparison between Unknown and I8
		if (inputEvent.IsActionReleased(DebugHotkey.speedUp, false))
		{
			DebugModifyTimescale(0.1);
		}
		else if (inputEvent.IsActionReleased(DebugHotkey.speedDown, false))
		{
			DebugModifyTimescale(-0.1);
		}
		else if (inputEvent.IsActionReleased(DebugHotkey.hideProceedButton, false))
		{
			IsDebugHidingProceedButton = !IsDebugHidingProceedButton;
			((Node)(object)Instance).AddChildSafely((Node?)(object)NFullscreenTextVfx.Create(IsDebugHidingProceedButton ? "Hide Proceed Button" : "Show Proceed Button"));
			this.DebugToggleProceedButton?.Invoke();
		}
		else if (inputEvent.IsActionReleased(DebugHotkey.hideHoverTips, false))
		{
			IsDebugHidingHoverTips = !IsDebugHidingHoverTips;
			((Node)(object)Instance).AddChildSafely((Node?)(object)NFullscreenTextVfx.Create(IsDebugHidingHoverTips ? "Hide HoverTips" : "Show HoverTips"));
		}
		InputEventMouseButton val = (InputEventMouseButton)(object)((inputEvent is InputEventMouseButton) ? inputEvent : null);
		if ((val != null && val.Pressed) || inputEvent.IsActionPressed(MegaInput.select, false, false) || inputEvent.IsActionPressed(MegaInput.cancel, false, false))
		{
			_logoCancelToken?.Cancel();
		}
		InputEventKey val2 = (InputEventKey)(object)((inputEvent is InputEventKey) ? inputEvent : null);
		if (val2 == null)
		{
			return;
		}
		if (OS.GetName().Contains("Windows"))
		{
			if (val2.Pressed && ((InputEventWithModifiers)val2).AltPressed && (long)val2.Keycode == 4194309)
			{
				ToggleFullscreen();
			}
		}
		else if (OS.GetName().Contains("macOS") && val2.Pressed && ((InputEventWithModifiers)val2).CtrlPressed && ((InputEventWithModifiers)val2).MetaPressed && (long)val2.Keycode == 70)
		{
			ToggleFullscreen();
		}
	}

	private void ToggleFullscreen()
	{
		Log.Info("Used FULLSCREEN shortcut");
		NFullscreenTickbox.SetFullscreen(!SaveManager.Instance.SettingsSave.Fullscreen);
	}

	private void DebugModifyTimescale(double offset)
	{
		double value = Math.Round(Engine.TimeScale + offset, 1);
		value = Math.Clamp(value, 0.1, 4.0);
		Engine.TimeScale = value;
		((Node)(object)this).AddChildSafely((Node?)(object)NFullscreenTextVfx.Create($"TimeScale:{Engine.TimeScale}"));
	}

	public WorldEnvironment ActivateWorldEnvironment()
	{
		((Node)(object)this).AddChildSafely((Node?)(object)WorldEnvironment);
		return WorldEnvironment;
	}

	public void DeactivateWorldEnvironment()
	{
		((Node)(object)this).RemoveChildSafely((Node?)(object)WorldEnvironment);
	}

	public void SetScreenShakeTarget(Control target)
	{
		_screenShake.SetTarget(target);
	}

	public void ClearScreenShakeTarget()
	{
		_screenShake.ClearTarget();
	}

	public void ScreenShake(ShakeStrength strength, ShakeDuration duration, float degAngle = -1f)
	{
		if (degAngle < 0f)
		{
			degAngle = Rng.Chaotic.NextFloat(360f);
		}
		_screenShake.Shake(strength, duration, degAngle);
	}

	public void ScreenRumble(ShakeStrength strength, ShakeDuration duration, RumbleStyle style)
	{
		_screenShake.Rumble(strength, duration, style);
	}

	public void ScreenShakeTrauma(ShakeStrength strength)
	{
		_screenShake.AddTrauma(strength);
	}

	public void DoHitStop(ShakeStrength strength, ShakeDuration duration)
	{
		HitStop.DoHitStop(strength, duration);
	}

	public static void ToggleTrailerMode()
	{
		IsTrailerMode = !IsTrailerMode;
	}

	public void SetScreenshakeMultiplier(float multiplier)
	{
		_screenShake.SetMultiplier(multiplier);
	}

	private void InitPools()
	{
		NCard.InitPool();
		NGridCardHolder.InitPool();
	}

	private void OnNewModDetected(Mod mod)
	{
		if (!((IEnumerable)((Node)NModalContainer.Instance).GetChildren(false)).OfType<NErrorPopup>().Any())
		{
			NErrorPopup modalToCreate = NErrorPopup.Create(new LocString("main_menu_ui", "MOD_NOT_LOADED_POPUP.title"), new LocString("main_menu_ui", "MOD_NOT_LOADED_POPUP.description"), null, showReportBugButton: false);
			NModalContainer.Instance.Add((Node)(object)modalToCreate);
		}
	}

	public void CheckShowSaveFileError(ReadSaveResult<SerializableProgress> progressReadResult, ReadSaveResult<PrefsSave> prefsReadResult, ReadSaveResult<SettingsSave>? settingsReadResult)
	{
		LocString locString = null;
		if (!progressReadResult.Success && progressReadResult.Status != ReadSaveStatus.FileNotFound)
		{
			locString = new LocString("main_menu_ui", "INVALID_SAVE_POPUP.description_progress");
		}
		else if (settingsReadResult != null && !settingsReadResult.Success && settingsReadResult.Status != ReadSaveStatus.FileNotFound)
		{
			locString = new LocString("main_menu_ui", "INVALID_SAVE_POPUP.description_settings");
		}
		else if (!prefsReadResult.Success && prefsReadResult.Status != ReadSaveStatus.FileNotFound)
		{
			locString = new LocString("main_menu_ui", "INVALID_SAVE_POPUP.description_settings");
		}
		if (locString != null)
		{
			NErrorPopup modalToCreate = NErrorPopup.Create(new LocString("main_menu_ui", "INVALID_SAVE_POPUP.title"), locString, new LocString("main_menu_ui", "INVALID_SAVE_POPUP.dismiss"), showReportBugButton: true);
			NModalContainer.Instance.Add((Node)(object)modalToCreate);
		}
	}

	private void CheckShowLocalizationOverrideErrors()
	{
		if (LocManager.Instance.ValidationErrors.Count != 0)
		{
			List<IGrouping<string, LocValidationError>> list = (from e in LocManager.Instance.ValidationErrors
				group e by e.FilePath).ToList();
			string text = string.Join("\n", from g in list.Take(5)
				select $"{Path.GetFileName(g.Key)} ({g.Count()} errors)");
			if (list.Count > 5)
			{
				text += $"\n... and {list.Count - 5} more files";
			}
			string body = "Errors found in the following localization override files:\n\n" + text + "\n\n[gold]Check the console logs for detailed error messages.[/gold]\n\nTo fix: Remove or correct invalid override files in your localization_override folder.";
			NErrorPopup modalToCreate = NErrorPopup.Create("Localization Override Errors", body, showReportBugButton: false);
			NModalContainer.Instance.Add((Node)(object)modalToCreate);
		}
	}

	private async Task<bool> InitializePlatform()
	{
		bool flag = CommandLineHelper.HasArg("force-steam");
		string text = CommandLineHelper.GetValue("force-steam") ?? "";
		if (!text.Equals("on", StringComparison.OrdinalIgnoreCase) && (!flag || !(text == string.Empty)) && (text.Equals("off", StringComparison.OrdinalIgnoreCase) || OS.HasFeature("editor")))
		{
			Log.Info("Steam initialization skipped (editor mode). Use --force-steam to enable.");
			return true;
		}
		bool steamInitialized = SteamInitializer.Initialize((Node)(object)this);
		if (!steamInitialized)
		{
			Log.Error("Failed to initialize Steam! Attempting to show error popup");
			await TryErrorInit();
			NGenericPopup nGenericPopup = NGenericPopup.Create();
			NModalContainer.Instance.Add((Node)(object)nGenericPopup);
			LocString locString = new LocString("main_menu_ui", "STEAM_INIT_ERROR.description");
			locString.Add("details", $"{SteamInitializer.InitResult}: {SteamInitializer.InitErrorMessage}");
			await nGenericPopup.WaitForConfirmation(locString, new LocString("main_menu_ui", "STEAM_INIT_ERROR.title"), null, new LocString("main_menu_ui", "QUIT"));
			((Node)this).GetTree().Quit(0);
		}
		else
		{
			_joinCallbackHandler = new SteamJoinCallbackHandler();
		}
		return steamInitialized;
	}

	public static void LogResourceStats(string context)
	{
		ulong staticMemoryUsage = OS.GetStaticMemoryUsage();
		ulong renderingInfo = RenderingServer.GetRenderingInfo((RenderingInfo)5);
		int value = (int)Performance.GetMonitor((Monitor)7);
		int value2 = (int)Performance.GetMonitor((Monitor)8);
		int value3 = (int)Performance.GetMonitor((Monitor)9);
		int value4 = PreloadManager.Cache.GetCacheKeys().Count();
		Log.Info($"[Startup] Resource stats ({context}): StaticMem={FormatBytes(staticMemoryUsage)}, VRAM={FormatBytes(renderingInfo)}, Objects={value:N0}, Resources={value2:N0}, Nodes={value3:N0}, CachedAssets={value4:N0}");
	}

	private static string FormatBytes(ulong bytes)
	{
		string[] array = new string[4] { "B", "KB", "MB", "GB" };
		int num = 0;
		double num2 = bytes;
		while (num2 >= 1024.0 && num < array.Length - 1)
		{
			num2 /= 1024.0;
			num++;
		}
		return $"{num2:0.#}{array[num]}";
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
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Expected O, but got Unknown
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Expected O, but got Unknown
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0272: Unknown result type (might be due to invalid IL or missing references)
		//IL_0295: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0324: Unknown result type (might be due to invalid IL or missing references)
		//IL_032d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0353: Unknown result type (might be due to invalid IL or missing references)
		//IL_037b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0386: Expected O, but got Unknown
		//IL_0381: Unknown result type (might be due to invalid IL or missing references)
		//IL_038c: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0404: Unknown result type (might be due to invalid IL or missing references)
		//IL_040f: Unknown result type (might be due to invalid IL or missing references)
		//IL_043a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0445: Expected O, but got Unknown
		//IL_0440: Unknown result type (might be due to invalid IL or missing references)
		//IL_0449: Unknown result type (might be due to invalid IL or missing references)
		//IL_046f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0478: Unknown result type (might be due to invalid IL or missing references)
		//IL_049e: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d1: Expected O, but got Unknown
		//IL_04cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0506: Unknown result type (might be due to invalid IL or missing references)
		//IL_052c: Unknown result type (might be due to invalid IL or missing references)
		//IL_054f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0570: Unknown result type (might be due to invalid IL or missing references)
		//IL_0591: Unknown result type (might be due to invalid IL or missing references)
		//IL_059c: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0606: Unknown result type (might be due to invalid IL or missing references)
		//IL_0627: Unknown result type (might be due to invalid IL or missing references)
		//IL_0632: Unknown result type (might be due to invalid IL or missing references)
		//IL_0658: Unknown result type (might be due to invalid IL or missing references)
		//IL_067b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0686: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_06cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_06fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0721: Unknown result type (might be due to invalid IL or missing references)
		//IL_072b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0751: Unknown result type (might be due to invalid IL or missing references)
		//IL_0774: Unknown result type (might be due to invalid IL or missing references)
		//IL_077f: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_07dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0803: Unknown result type (might be due to invalid IL or missing references)
		//IL_0827: Unknown result type (might be due to invalid IL or missing references)
		//IL_0832: Unknown result type (might be due to invalid IL or missing references)
		//IL_0858: Unknown result type (might be due to invalid IL or missing references)
		//IL_087c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0887: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(33);
		list.Add(new MethodInfo(MethodName._EnterTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnWindowChange, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.IsMainThread, new PropertyInfo((Type)1, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)33, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._ExitTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.IsReleaseGame, new PropertyInfo((Type)1, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)33, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.InitializeGraphicsPreferences, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ApplyDisplaySettings, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.GetInspectRelicScreen, new PropertyInfo((Type)24, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.GetInspectCardScreen, new PropertyInfo((Type)24, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ApplySyncSetting, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)33, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.Reset, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)33, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Notification, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("what"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.Quit, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.Relocalize, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ReloadMainMenu, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Input, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("inputEvent"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("InputEvent"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ToggleFullscreen, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.DebugModifyTimescale, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)3, StringName.op_Implicit("offset"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ActivateWorldEnvironment, new PropertyInfo((Type)24, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("WorldEnvironment"), false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.DeactivateWorldEnvironment, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetScreenShakeTarget, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("target"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ClearScreenShakeTarget, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ScreenShake, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("strength"), (PropertyHint)0, "", (PropertyUsageFlags)6, false),
			new PropertyInfo((Type)2, StringName.op_Implicit("duration"), (PropertyHint)0, "", (PropertyUsageFlags)6, false),
			new PropertyInfo((Type)3, StringName.op_Implicit("degAngle"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ScreenRumble, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("strength"), (PropertyHint)0, "", (PropertyUsageFlags)6, false),
			new PropertyInfo((Type)2, StringName.op_Implicit("duration"), (PropertyHint)0, "", (PropertyUsageFlags)6, false),
			new PropertyInfo((Type)2, StringName.op_Implicit("style"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ScreenShakeTrauma, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("strength"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.DoHitStop, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("strength"), (PropertyHint)0, "", (PropertyUsageFlags)6, false),
			new PropertyInfo((Type)2, StringName.op_Implicit("duration"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ToggleTrailerMode, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)33, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetScreenshakeMultiplier, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)3, StringName.op_Implicit("multiplier"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.InitPools, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.CheckShowLocalizationOverrideErrors, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.LogResourceStats, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)33, new List<PropertyInfo>
		{
			new PropertyInfo((Type)4, StringName.op_Implicit("context"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.FormatBytes, new PropertyInfo((Type)4, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)33, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("bytes"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0322: Unknown result type (might be due to invalid IL or missing references)
		//IL_0327: Unknown result type (might be due to invalid IL or missing references)
		//IL_034b: Unknown result type (might be due to invalid IL or missing references)
		//IL_037e: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_043d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0470: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0507: Unknown result type (might be due to invalid IL or missing references)
		//IL_052c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0551: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0583: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_05be: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._EnterTree && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._EnterTree();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnWindowChange && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnWindowChange();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.IsMainThread && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			bool flag = IsMainThread();
			ret = VariantUtils.CreateFrom<bool>(ref flag);
			return true;
		}
		if ((ref method) == MethodName._ExitTree && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._ExitTree();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.IsReleaseGame && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			bool flag2 = IsReleaseGame();
			ret = VariantUtils.CreateFrom<bool>(ref flag2);
			return true;
		}
		if ((ref method) == MethodName.InitializeGraphicsPreferences && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			InitializeGraphicsPreferences();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.ApplyDisplaySettings && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			ApplyDisplaySettings();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.GetInspectRelicScreen && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			NInspectRelicScreen inspectRelicScreen = GetInspectRelicScreen();
			ret = VariantUtils.CreateFrom<NInspectRelicScreen>(ref inspectRelicScreen);
			return true;
		}
		if ((ref method) == MethodName.GetInspectCardScreen && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			NInspectCardScreen inspectCardScreen = GetInspectCardScreen();
			ret = VariantUtils.CreateFrom<NInspectCardScreen>(ref inspectCardScreen);
			return true;
		}
		if ((ref method) == MethodName.ApplySyncSetting && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			ApplySyncSetting();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.Reset && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			Reset();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._Notification && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			((GodotObject)this)._Notification(VariantUtils.ConvertTo<int>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.Quit && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			Quit();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.Relocalize && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			Relocalize();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.ReloadMainMenu && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			ReloadMainMenu();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._Input && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			((Node)this)._Input(VariantUtils.ConvertTo<InputEvent>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.ToggleFullscreen && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			ToggleFullscreen();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.DebugModifyTimescale && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			DebugModifyTimescale(VariantUtils.ConvertTo<double>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.ActivateWorldEnvironment && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			WorldEnvironment val = ActivateWorldEnvironment();
			ret = VariantUtils.CreateFrom<WorldEnvironment>(ref val);
			return true;
		}
		if ((ref method) == MethodName.DeactivateWorldEnvironment && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			DeactivateWorldEnvironment();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetScreenShakeTarget && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			SetScreenShakeTarget(VariantUtils.ConvertTo<Control>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.ClearScreenShakeTarget && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			ClearScreenShakeTarget();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.ScreenShake && ((NativeVariantPtrArgs)(ref args)).Count == 3)
		{
			ScreenShake(VariantUtils.ConvertTo<ShakeStrength>(ref ((NativeVariantPtrArgs)(ref args))[0]), VariantUtils.ConvertTo<ShakeDuration>(ref ((NativeVariantPtrArgs)(ref args))[1]), VariantUtils.ConvertTo<float>(ref ((NativeVariantPtrArgs)(ref args))[2]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.ScreenRumble && ((NativeVariantPtrArgs)(ref args)).Count == 3)
		{
			ScreenRumble(VariantUtils.ConvertTo<ShakeStrength>(ref ((NativeVariantPtrArgs)(ref args))[0]), VariantUtils.ConvertTo<ShakeDuration>(ref ((NativeVariantPtrArgs)(ref args))[1]), VariantUtils.ConvertTo<RumbleStyle>(ref ((NativeVariantPtrArgs)(ref args))[2]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.ScreenShakeTrauma && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			ScreenShakeTrauma(VariantUtils.ConvertTo<ShakeStrength>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.DoHitStop && ((NativeVariantPtrArgs)(ref args)).Count == 2)
		{
			DoHitStop(VariantUtils.ConvertTo<ShakeStrength>(ref ((NativeVariantPtrArgs)(ref args))[0]), VariantUtils.ConvertTo<ShakeDuration>(ref ((NativeVariantPtrArgs)(ref args))[1]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.ToggleTrailerMode && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			ToggleTrailerMode();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetScreenshakeMultiplier && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			SetScreenshakeMultiplier(VariantUtils.ConvertTo<float>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.InitPools && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			InitPools();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.CheckShowLocalizationOverrideErrors && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			CheckShowLocalizationOverrideErrors();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.LogResourceStats && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			LogResourceStats(VariantUtils.ConvertTo<string>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.FormatBytes && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			string text = FormatBytes(VariantUtils.ConvertTo<ulong>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<string>(ref text);
			return true;
		}
		return ((Control)this).InvokeGodotClassMethod(ref method, args, ref ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static bool InvokeGodotClassStaticMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.IsMainThread && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			bool flag = IsMainThread();
			ret = VariantUtils.CreateFrom<bool>(ref flag);
			return true;
		}
		if ((ref method) == MethodName.IsReleaseGame && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			bool flag2 = IsReleaseGame();
			ret = VariantUtils.CreateFrom<bool>(ref flag2);
			return true;
		}
		if ((ref method) == MethodName.ApplySyncSetting && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			ApplySyncSetting();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.Reset && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			Reset();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.ToggleTrailerMode && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			ToggleTrailerMode();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.LogResourceStats && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			LogResourceStats(VariantUtils.ConvertTo<string>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.FormatBytes && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			string text = FormatBytes(VariantUtils.ConvertTo<ulong>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<string>(ref text);
			return true;
		}
		ret = default(godot_variant);
		return false;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if ((ref method) == MethodName._EnterTree)
		{
			return true;
		}
		if ((ref method) == MethodName._Ready)
		{
			return true;
		}
		if ((ref method) == MethodName.OnWindowChange)
		{
			return true;
		}
		if ((ref method) == MethodName.IsMainThread)
		{
			return true;
		}
		if ((ref method) == MethodName._ExitTree)
		{
			return true;
		}
		if ((ref method) == MethodName.IsReleaseGame)
		{
			return true;
		}
		if ((ref method) == MethodName.InitializeGraphicsPreferences)
		{
			return true;
		}
		if ((ref method) == MethodName.ApplyDisplaySettings)
		{
			return true;
		}
		if ((ref method) == MethodName.GetInspectRelicScreen)
		{
			return true;
		}
		if ((ref method) == MethodName.GetInspectCardScreen)
		{
			return true;
		}
		if ((ref method) == MethodName.ApplySyncSetting)
		{
			return true;
		}
		if ((ref method) == MethodName.Reset)
		{
			return true;
		}
		if ((ref method) == MethodName._Notification)
		{
			return true;
		}
		if ((ref method) == MethodName.Quit)
		{
			return true;
		}
		if ((ref method) == MethodName.Relocalize)
		{
			return true;
		}
		if ((ref method) == MethodName.ReloadMainMenu)
		{
			return true;
		}
		if ((ref method) == MethodName._Input)
		{
			return true;
		}
		if ((ref method) == MethodName.ToggleFullscreen)
		{
			return true;
		}
		if ((ref method) == MethodName.DebugModifyTimescale)
		{
			return true;
		}
		if ((ref method) == MethodName.ActivateWorldEnvironment)
		{
			return true;
		}
		if ((ref method) == MethodName.DeactivateWorldEnvironment)
		{
			return true;
		}
		if ((ref method) == MethodName.SetScreenShakeTarget)
		{
			return true;
		}
		if ((ref method) == MethodName.ClearScreenShakeTarget)
		{
			return true;
		}
		if ((ref method) == MethodName.ScreenShake)
		{
			return true;
		}
		if ((ref method) == MethodName.ScreenRumble)
		{
			return true;
		}
		if ((ref method) == MethodName.ScreenShakeTrauma)
		{
			return true;
		}
		if ((ref method) == MethodName.DoHitStop)
		{
			return true;
		}
		if ((ref method) == MethodName.ToggleTrailerMode)
		{
			return true;
		}
		if ((ref method) == MethodName.SetScreenshakeMultiplier)
		{
			return true;
		}
		if ((ref method) == MethodName.InitPools)
		{
			return true;
		}
		if ((ref method) == MethodName.CheckShowLocalizationOverrideErrors)
		{
			return true;
		}
		if ((ref method) == MethodName.LogResourceStats)
		{
			return true;
		}
		if ((ref method) == MethodName.FormatBytes)
		{
			return true;
		}
		return ((Control)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName.RootSceneContainer)
		{
			RootSceneContainer = VariantUtils.ConvertTo<NSceneContainer>(ref value);
			return true;
		}
		if ((ref name) == PropertyName.HoverTipsContainer)
		{
			HoverTipsContainer = VariantUtils.ConvertTo<Node>(ref value);
			return true;
		}
		if ((ref name) == PropertyName.Transition)
		{
			Transition = VariantUtils.ConvertTo<NTransition>(ref value);
			return true;
		}
		if ((ref name) == PropertyName.TimeoutOverlay)
		{
			TimeoutOverlay = VariantUtils.ConvertTo<NMultiplayerTimeoutOverlay>(ref value);
			return true;
		}
		if ((ref name) == PropertyName.AudioManager)
		{
			AudioManager = VariantUtils.ConvertTo<NAudioManager>(ref value);
			return true;
		}
		if ((ref name) == PropertyName.RemoteCursorContainer)
		{
			RemoteCursorContainer = VariantUtils.ConvertTo<NRemoteMouseCursorContainer>(ref value);
			return true;
		}
		if ((ref name) == PropertyName.InputManager)
		{
			InputManager = VariantUtils.ConvertTo<NInputManager>(ref value);
			return true;
		}
		if ((ref name) == PropertyName.HotkeyManager)
		{
			HotkeyManager = VariantUtils.ConvertTo<NHotkeyManager>(ref value);
			return true;
		}
		if ((ref name) == PropertyName.ReactionWheel)
		{
			ReactionWheel = VariantUtils.ConvertTo<NReactionWheel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName.ReactionContainer)
		{
			ReactionContainer = VariantUtils.ConvertTo<NReactionContainer>(ref value);
			return true;
		}
		if ((ref name) == PropertyName.CursorManager)
		{
			CursorManager = VariantUtils.ConvertTo<NCursorManager>(ref value);
			return true;
		}
		if ((ref name) == PropertyName.DebugAudio)
		{
			DebugAudio = VariantUtils.ConvertTo<NDebugAudioManager>(ref value);
			return true;
		}
		if ((ref name) == PropertyName.DebugSeedOverride)
		{
			DebugSeedOverride = VariantUtils.ConvertTo<string>(ref value);
			return true;
		}
		if ((ref name) == PropertyName.StartOnMainMenu)
		{
			StartOnMainMenu = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName.InspectRelicScreen)
		{
			InspectRelicScreen = VariantUtils.ConvertTo<NInspectRelicScreen>(ref value);
			return true;
		}
		if ((ref name) == PropertyName.InspectCardScreen)
		{
			InspectCardScreen = VariantUtils.ConvertTo<NInspectCardScreen>(ref value);
			return true;
		}
		if ((ref name) == PropertyName.FeedbackScreen)
		{
			FeedbackScreen = VariantUtils.ConvertTo<NSendFeedbackScreen>(ref value);
			return true;
		}
		if ((ref name) == PropertyName.WorldEnvironment)
		{
			WorldEnvironment = VariantUtils.ConvertTo<WorldEnvironment>(ref value);
			return true;
		}
		if ((ref name) == PropertyName.HitStop)
		{
			HitStop = VariantUtils.ConvertTo<NHitStop>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._inspectionContainer)
		{
			_inspectionContainer = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._screenShake)
		{
			_screenShake = VariantUtils.ConvertTo<NScreenShake>(ref value);
			return true;
		}
		return ((GodotObject)this).SetGodotClassPropertyValue(ref name, ref value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_029c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0308: Unknown result type (might be due to invalid IL or missing references)
		//IL_030d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0328: Unknown result type (might be due to invalid IL or missing references)
		//IL_032d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0348: Unknown result type (might be due to invalid IL or missing references)
		//IL_034d: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName.RootSceneContainer)
		{
			NSceneContainer rootSceneContainer = RootSceneContainer;
			value = VariantUtils.CreateFrom<NSceneContainer>(ref rootSceneContainer);
			return true;
		}
		if ((ref name) == PropertyName.HoverTipsContainer)
		{
			Node hoverTipsContainer = HoverTipsContainer;
			value = VariantUtils.CreateFrom<Node>(ref hoverTipsContainer);
			return true;
		}
		if ((ref name) == PropertyName.MainMenu)
		{
			NMainMenu mainMenu = MainMenu;
			value = VariantUtils.CreateFrom<NMainMenu>(ref mainMenu);
			return true;
		}
		if ((ref name) == PropertyName.CurrentRunNode)
		{
			NRun currentRunNode = CurrentRunNode;
			value = VariantUtils.CreateFrom<NRun>(ref currentRunNode);
			return true;
		}
		if ((ref name) == PropertyName.LogoAnimation)
		{
			NLogoAnimation logoAnimation = LogoAnimation;
			value = VariantUtils.CreateFrom<NLogoAnimation>(ref logoAnimation);
			return true;
		}
		if ((ref name) == PropertyName.Transition)
		{
			NTransition transition = Transition;
			value = VariantUtils.CreateFrom<NTransition>(ref transition);
			return true;
		}
		if ((ref name) == PropertyName.TimeoutOverlay)
		{
			NMultiplayerTimeoutOverlay timeoutOverlay = TimeoutOverlay;
			value = VariantUtils.CreateFrom<NMultiplayerTimeoutOverlay>(ref timeoutOverlay);
			return true;
		}
		if ((ref name) == PropertyName.AudioManager)
		{
			NAudioManager audioManager = AudioManager;
			value = VariantUtils.CreateFrom<NAudioManager>(ref audioManager);
			return true;
		}
		if ((ref name) == PropertyName.RemoteCursorContainer)
		{
			NRemoteMouseCursorContainer remoteCursorContainer = RemoteCursorContainer;
			value = VariantUtils.CreateFrom<NRemoteMouseCursorContainer>(ref remoteCursorContainer);
			return true;
		}
		if ((ref name) == PropertyName.InputManager)
		{
			NInputManager inputManager = InputManager;
			value = VariantUtils.CreateFrom<NInputManager>(ref inputManager);
			return true;
		}
		if ((ref name) == PropertyName.HotkeyManager)
		{
			NHotkeyManager hotkeyManager = HotkeyManager;
			value = VariantUtils.CreateFrom<NHotkeyManager>(ref hotkeyManager);
			return true;
		}
		if ((ref name) == PropertyName.ReactionWheel)
		{
			NReactionWheel reactionWheel = ReactionWheel;
			value = VariantUtils.CreateFrom<NReactionWheel>(ref reactionWheel);
			return true;
		}
		if ((ref name) == PropertyName.ReactionContainer)
		{
			NReactionContainer reactionContainer = ReactionContainer;
			value = VariantUtils.CreateFrom<NReactionContainer>(ref reactionContainer);
			return true;
		}
		if ((ref name) == PropertyName.CursorManager)
		{
			NCursorManager cursorManager = CursorManager;
			value = VariantUtils.CreateFrom<NCursorManager>(ref cursorManager);
			return true;
		}
		if ((ref name) == PropertyName.DebugAudio)
		{
			NDebugAudioManager debugAudio = DebugAudio;
			value = VariantUtils.CreateFrom<NDebugAudioManager>(ref debugAudio);
			return true;
		}
		if ((ref name) == PropertyName.DebugSeedOverride)
		{
			string debugSeedOverride = DebugSeedOverride;
			value = VariantUtils.CreateFrom<string>(ref debugSeedOverride);
			return true;
		}
		if ((ref name) == PropertyName.StartOnMainMenu)
		{
			bool startOnMainMenu = StartOnMainMenu;
			value = VariantUtils.CreateFrom<bool>(ref startOnMainMenu);
			return true;
		}
		if ((ref name) == PropertyName.InspectRelicScreen)
		{
			NInspectRelicScreen inspectRelicScreen = InspectRelicScreen;
			value = VariantUtils.CreateFrom<NInspectRelicScreen>(ref inspectRelicScreen);
			return true;
		}
		if ((ref name) == PropertyName.InspectCardScreen)
		{
			NInspectCardScreen inspectCardScreen = InspectCardScreen;
			value = VariantUtils.CreateFrom<NInspectCardScreen>(ref inspectCardScreen);
			return true;
		}
		if ((ref name) == PropertyName.FeedbackScreen)
		{
			NSendFeedbackScreen feedbackScreen = FeedbackScreen;
			value = VariantUtils.CreateFrom<NSendFeedbackScreen>(ref feedbackScreen);
			return true;
		}
		if ((ref name) == PropertyName.WorldEnvironment)
		{
			WorldEnvironment worldEnvironment = WorldEnvironment;
			value = VariantUtils.CreateFrom<WorldEnvironment>(ref worldEnvironment);
			return true;
		}
		if ((ref name) == PropertyName.HitStop)
		{
			NHitStop hitStop = HitStop;
			value = VariantUtils.CreateFrom<NHitStop>(ref hitStop);
			return true;
		}
		if ((ref name) == PropertyName._inspectionContainer)
		{
			value = VariantUtils.CreateFrom<Control>(ref _inspectionContainer);
			return true;
		}
		if ((ref name) == PropertyName._screenShake)
		{
			value = VariantUtils.CreateFrom<NScreenShake>(ref _screenShake);
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
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_026d: Unknown result type (might be due to invalid IL or missing references)
		//IL_028e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02af: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0312: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName.RootSceneContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.HoverTipsContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.MainMenu, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.CurrentRunNode, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.LogoAnimation, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.Transition, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.TimeoutOverlay, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.AudioManager, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.RemoteCursorContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.InputManager, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.HotkeyManager, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.ReactionWheel, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.ReactionContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.CursorManager, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.DebugAudio, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)4, PropertyName.DebugSeedOverride, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName.StartOnMainMenu, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.InspectRelicScreen, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.InspectCardScreen, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.FeedbackScreen, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.WorldEnvironment, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.HitStop, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._inspectionContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._screenShake, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		StringName rootSceneContainer = PropertyName.RootSceneContainer;
		NSceneContainer rootSceneContainer2 = RootSceneContainer;
		info.AddProperty(rootSceneContainer, Variant.From<NSceneContainer>(ref rootSceneContainer2));
		StringName hoverTipsContainer = PropertyName.HoverTipsContainer;
		Node hoverTipsContainer2 = HoverTipsContainer;
		info.AddProperty(hoverTipsContainer, Variant.From<Node>(ref hoverTipsContainer2));
		StringName transition = PropertyName.Transition;
		NTransition transition2 = Transition;
		info.AddProperty(transition, Variant.From<NTransition>(ref transition2));
		StringName timeoutOverlay = PropertyName.TimeoutOverlay;
		NMultiplayerTimeoutOverlay timeoutOverlay2 = TimeoutOverlay;
		info.AddProperty(timeoutOverlay, Variant.From<NMultiplayerTimeoutOverlay>(ref timeoutOverlay2));
		StringName audioManager = PropertyName.AudioManager;
		NAudioManager audioManager2 = AudioManager;
		info.AddProperty(audioManager, Variant.From<NAudioManager>(ref audioManager2));
		StringName remoteCursorContainer = PropertyName.RemoteCursorContainer;
		NRemoteMouseCursorContainer remoteCursorContainer2 = RemoteCursorContainer;
		info.AddProperty(remoteCursorContainer, Variant.From<NRemoteMouseCursorContainer>(ref remoteCursorContainer2));
		StringName inputManager = PropertyName.InputManager;
		NInputManager inputManager2 = InputManager;
		info.AddProperty(inputManager, Variant.From<NInputManager>(ref inputManager2));
		StringName hotkeyManager = PropertyName.HotkeyManager;
		NHotkeyManager hotkeyManager2 = HotkeyManager;
		info.AddProperty(hotkeyManager, Variant.From<NHotkeyManager>(ref hotkeyManager2));
		StringName reactionWheel = PropertyName.ReactionWheel;
		NReactionWheel reactionWheel2 = ReactionWheel;
		info.AddProperty(reactionWheel, Variant.From<NReactionWheel>(ref reactionWheel2));
		StringName reactionContainer = PropertyName.ReactionContainer;
		NReactionContainer reactionContainer2 = ReactionContainer;
		info.AddProperty(reactionContainer, Variant.From<NReactionContainer>(ref reactionContainer2));
		StringName cursorManager = PropertyName.CursorManager;
		NCursorManager cursorManager2 = CursorManager;
		info.AddProperty(cursorManager, Variant.From<NCursorManager>(ref cursorManager2));
		StringName debugAudio = PropertyName.DebugAudio;
		NDebugAudioManager debugAudio2 = DebugAudio;
		info.AddProperty(debugAudio, Variant.From<NDebugAudioManager>(ref debugAudio2));
		StringName debugSeedOverride = PropertyName.DebugSeedOverride;
		string debugSeedOverride2 = DebugSeedOverride;
		info.AddProperty(debugSeedOverride, Variant.From<string>(ref debugSeedOverride2));
		StringName startOnMainMenu = PropertyName.StartOnMainMenu;
		bool startOnMainMenu2 = StartOnMainMenu;
		info.AddProperty(startOnMainMenu, Variant.From<bool>(ref startOnMainMenu2));
		StringName inspectRelicScreen = PropertyName.InspectRelicScreen;
		NInspectRelicScreen inspectRelicScreen2 = InspectRelicScreen;
		info.AddProperty(inspectRelicScreen, Variant.From<NInspectRelicScreen>(ref inspectRelicScreen2));
		StringName inspectCardScreen = PropertyName.InspectCardScreen;
		NInspectCardScreen inspectCardScreen2 = InspectCardScreen;
		info.AddProperty(inspectCardScreen, Variant.From<NInspectCardScreen>(ref inspectCardScreen2));
		StringName feedbackScreen = PropertyName.FeedbackScreen;
		NSendFeedbackScreen feedbackScreen2 = FeedbackScreen;
		info.AddProperty(feedbackScreen, Variant.From<NSendFeedbackScreen>(ref feedbackScreen2));
		StringName worldEnvironment = PropertyName.WorldEnvironment;
		WorldEnvironment worldEnvironment2 = WorldEnvironment;
		info.AddProperty(worldEnvironment, Variant.From<WorldEnvironment>(ref worldEnvironment2));
		StringName hitStop = PropertyName.HitStop;
		NHitStop hitStop2 = HitStop;
		info.AddProperty(hitStop, Variant.From<NHitStop>(ref hitStop2));
		info.AddProperty(PropertyName._inspectionContainer, Variant.From<Control>(ref _inspectionContainer));
		info.AddProperty(PropertyName._screenShake, Variant.From<NScreenShake>(ref _screenShake));
		info.AddSignalEventDelegate(SignalName.WindowChange, (Delegate)backing_WindowChange);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName.RootSceneContainer, ref val))
		{
			RootSceneContainer = ((Variant)(ref val)).As<NSceneContainer>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName.HoverTipsContainer, ref val2))
		{
			HoverTipsContainer = ((Variant)(ref val2)).As<Node>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName.Transition, ref val3))
		{
			Transition = ((Variant)(ref val3)).As<NTransition>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName.TimeoutOverlay, ref val4))
		{
			TimeoutOverlay = ((Variant)(ref val4)).As<NMultiplayerTimeoutOverlay>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName.AudioManager, ref val5))
		{
			AudioManager = ((Variant)(ref val5)).As<NAudioManager>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName.RemoteCursorContainer, ref val6))
		{
			RemoteCursorContainer = ((Variant)(ref val6)).As<NRemoteMouseCursorContainer>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName.InputManager, ref val7))
		{
			InputManager = ((Variant)(ref val7)).As<NInputManager>();
		}
		Variant val8 = default(Variant);
		if (info.TryGetProperty(PropertyName.HotkeyManager, ref val8))
		{
			HotkeyManager = ((Variant)(ref val8)).As<NHotkeyManager>();
		}
		Variant val9 = default(Variant);
		if (info.TryGetProperty(PropertyName.ReactionWheel, ref val9))
		{
			ReactionWheel = ((Variant)(ref val9)).As<NReactionWheel>();
		}
		Variant val10 = default(Variant);
		if (info.TryGetProperty(PropertyName.ReactionContainer, ref val10))
		{
			ReactionContainer = ((Variant)(ref val10)).As<NReactionContainer>();
		}
		Variant val11 = default(Variant);
		if (info.TryGetProperty(PropertyName.CursorManager, ref val11))
		{
			CursorManager = ((Variant)(ref val11)).As<NCursorManager>();
		}
		Variant val12 = default(Variant);
		if (info.TryGetProperty(PropertyName.DebugAudio, ref val12))
		{
			DebugAudio = ((Variant)(ref val12)).As<NDebugAudioManager>();
		}
		Variant val13 = default(Variant);
		if (info.TryGetProperty(PropertyName.DebugSeedOverride, ref val13))
		{
			DebugSeedOverride = ((Variant)(ref val13)).As<string>();
		}
		Variant val14 = default(Variant);
		if (info.TryGetProperty(PropertyName.StartOnMainMenu, ref val14))
		{
			StartOnMainMenu = ((Variant)(ref val14)).As<bool>();
		}
		Variant val15 = default(Variant);
		if (info.TryGetProperty(PropertyName.InspectRelicScreen, ref val15))
		{
			InspectRelicScreen = ((Variant)(ref val15)).As<NInspectRelicScreen>();
		}
		Variant val16 = default(Variant);
		if (info.TryGetProperty(PropertyName.InspectCardScreen, ref val16))
		{
			InspectCardScreen = ((Variant)(ref val16)).As<NInspectCardScreen>();
		}
		Variant val17 = default(Variant);
		if (info.TryGetProperty(PropertyName.FeedbackScreen, ref val17))
		{
			FeedbackScreen = ((Variant)(ref val17)).As<NSendFeedbackScreen>();
		}
		Variant val18 = default(Variant);
		if (info.TryGetProperty(PropertyName.WorldEnvironment, ref val18))
		{
			WorldEnvironment = ((Variant)(ref val18)).As<WorldEnvironment>();
		}
		Variant val19 = default(Variant);
		if (info.TryGetProperty(PropertyName.HitStop, ref val19))
		{
			HitStop = ((Variant)(ref val19)).As<NHitStop>();
		}
		Variant val20 = default(Variant);
		if (info.TryGetProperty(PropertyName._inspectionContainer, ref val20))
		{
			_inspectionContainer = ((Variant)(ref val20)).As<Control>();
		}
		Variant val21 = default(Variant);
		if (info.TryGetProperty(PropertyName._screenShake, ref val21))
		{
			_screenShake = ((Variant)(ref val21)).As<NScreenShake>();
		}
		WindowChangeEventHandler windowChangeEventHandler = default(WindowChangeEventHandler);
		if (info.TryGetSignalEventDelegate<WindowChangeEventHandler>(SignalName.WindowChange, ref windowChangeEventHandler))
		{
			backing_WindowChange = windowChangeEventHandler;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotSignalList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(1);
		list.Add(new MethodInfo(SignalName.WindowChange, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	protected void EmitSignalWindowChange()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).EmitSignal(SignalName.WindowChange, Array.Empty<Variant>());
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RaiseGodotClassSignalCallbacks(in godot_string_name signal, NativeVariantPtrArgs args)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		if ((ref signal) == SignalName.WindowChange && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			backing_WindowChange?.Invoke();
		}
		else
		{
			((GodotObject)this).RaiseGodotClassSignalCallbacks(ref signal, args);
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassSignal(in godot_string_name signal)
	{
		if ((ref signal) == SignalName.WindowChange)
		{
			return true;
		}
		return ((Control)this).HasGodotClassSignal(ref signal);
	}
}
