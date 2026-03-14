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
using MegaCrit.Sts2.Core.Daily;
using MegaCrit.Sts2.Core.Debug;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Multiplayer;
using MegaCrit.Sts2.Core.Multiplayer.Connection;
using MegaCrit.Sts2.Core.Nodes.Audio;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Multiplayer;
using MegaCrit.Sts2.Core.Nodes.Screens.CharacterSelect;
using MegaCrit.Sts2.Core.Nodes.Screens.ModdingScreen;
using MegaCrit.Sts2.Core.Nodes.Screens.ProfileScreen;
using MegaCrit.Sts2.Core.Nodes.Screens.ScreenContext;
using MegaCrit.Sts2.Core.Nodes.Screens.Settings;
using MegaCrit.Sts2.Core.Nodes.Screens.Timeline;
using MegaCrit.Sts2.Core.Platform;
using MegaCrit.Sts2.Core.Platform.Steam;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.Settings;
using MegaCrit.Sts2.Core.Timeline.Epochs;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.Nodes.Screens.MainMenu;

[ScriptPath("res://src/Core/Nodes/Screens/MainMenu/NMainMenu.cs")]
public class NMainMenu : Control, IScreenContext
{
	public class MethodName : MethodName
	{
		public static readonly StringName Create = StringName.op_Implicit("Create");

		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName ConnectMainMenuTextButtonFocusLogic = StringName.op_Implicit("ConnectMainMenuTextButtonFocusLogic");

		public static readonly StringName MainMenuButtonFocused = StringName.op_Implicit("MainMenuButtonFocused");

		public static readonly StringName MainMenuButtonUnfocused = StringName.op_Implicit("MainMenuButtonUnfocused");

		public static readonly StringName EnableBackstop = StringName.op_Implicit("EnableBackstop");

		public static readonly StringName DisableBackstop = StringName.op_Implicit("DisableBackstop");

		public static readonly StringName DisableBackstopInstantly = StringName.op_Implicit("DisableBackstopInstantly");

		public static readonly StringName EnableBackstopInstantly = StringName.op_Implicit("EnableBackstopInstantly");

		public static readonly StringName UpdateShaderMix = StringName.op_Implicit("UpdateShaderMix");

		public static readonly StringName UpdateShaderLod = StringName.op_Implicit("UpdateShaderLod");

		public static readonly StringName RefreshButtons = StringName.op_Implicit("RefreshButtons");

		public static readonly StringName UpdateTimelineButtonBehavior = StringName.op_Implicit("UpdateTimelineButtonBehavior");

		public static readonly StringName OnSubmenuStackChanged = StringName.op_Implicit("OnSubmenuStackChanged");

		public static readonly StringName OnContinueButtonPressed = StringName.op_Implicit("OnContinueButtonPressed");

		public static readonly StringName DisplayLoadSaveError = StringName.op_Implicit("DisplayLoadSaveError");

		public static readonly StringName OnAbandonRunButtonPressed = StringName.op_Implicit("OnAbandonRunButtonPressed");

		public static readonly StringName AbandonRun = StringName.op_Implicit("AbandonRun");

		public static readonly StringName SingleplayerButtonPressed = StringName.op_Implicit("SingleplayerButtonPressed");

		public static readonly StringName OpenSingleplayerSubmenu = StringName.op_Implicit("OpenSingleplayerSubmenu");

		public static readonly StringName OpenMultiplayerSubmenu = StringName.op_Implicit("OpenMultiplayerSubmenu");

		public static readonly StringName OpenCompendiumSubmenu = StringName.op_Implicit("OpenCompendiumSubmenu");

		public static readonly StringName OpenTimelineScreen = StringName.op_Implicit("OpenTimelineScreen");

		public static readonly StringName OpenSettingsMenu = StringName.op_Implicit("OpenSettingsMenu");

		public static readonly StringName OpenProfileScreen = StringName.op_Implicit("OpenProfileScreen");

		public static readonly StringName OpenPatchNotes = StringName.op_Implicit("OpenPatchNotes");

		public static readonly StringName Quit = StringName.op_Implicit("Quit");

		public static readonly StringName OnWindowChange = StringName.op_Implicit("OnWindowChange");

		public static readonly StringName CheckCommandLineArgs = StringName.op_Implicit("CheckCommandLineArgs");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName PatchNotesScreen = StringName.op_Implicit("PatchNotesScreen");

		public static readonly StringName BlurBackstop = StringName.op_Implicit("BlurBackstop");

		public static readonly StringName MainMenuButtons = StringName.op_Implicit("MainMenuButtons");

		public static readonly StringName SubmenuStack = StringName.op_Implicit("SubmenuStack");

		public static readonly StringName ContinueRunInfo = StringName.op_Implicit("ContinueRunInfo");

		public static readonly StringName DefaultFocusedControl = StringName.op_Implicit("DefaultFocusedControl");

		public static readonly StringName _window = StringName.op_Implicit("_window");

		public static readonly StringName _continueButton = StringName.op_Implicit("_continueButton");

		public static readonly StringName _abandonRunButton = StringName.op_Implicit("_abandonRunButton");

		public static readonly StringName _singleplayerButton = StringName.op_Implicit("_singleplayerButton");

		public static readonly StringName _compendiumButton = StringName.op_Implicit("_compendiumButton");

		public static readonly StringName _timelineButton = StringName.op_Implicit("_timelineButton");

		public static readonly StringName _settingsButton = StringName.op_Implicit("_settingsButton");

		public static readonly StringName _quitButton = StringName.op_Implicit("_quitButton");

		public static readonly StringName _multiplayerButton = StringName.op_Implicit("_multiplayerButton");

		public static readonly StringName _buttonReticleLeft = StringName.op_Implicit("_buttonReticleLeft");

		public static readonly StringName _buttonReticleRight = StringName.op_Implicit("_buttonReticleRight");

		public static readonly StringName _reticleTween = StringName.op_Implicit("_reticleTween");

		public static readonly StringName _patchNotesButtonNode = StringName.op_Implicit("_patchNotesButtonNode");

		public static readonly StringName _openProfileScreenButton = StringName.op_Implicit("_openProfileScreenButton");

		public static readonly StringName _lastHitButton = StringName.op_Implicit("_lastHitButton");

		public static readonly StringName _runInfo = StringName.op_Implicit("_runInfo");

		public static readonly StringName _timelineNotificationDot = StringName.op_Implicit("_timelineNotificationDot");

		public static readonly StringName _backstopTween = StringName.op_Implicit("_backstopTween");

		public static readonly StringName _bg = StringName.op_Implicit("_bg");

		public static readonly StringName _blur = StringName.op_Implicit("_blur");

		public static readonly StringName _openTimeline = StringName.op_Implicit("_openTimeline");
	}

	public class SignalName : SignalName
	{
	}

	private static readonly StringName _lod = new StringName("lod");

	private static readonly StringName _mixPercentage = new StringName("mix_percentage");

	private const string _scenePath = "res://scenes/screens/main_menu.tscn";

	private const string _menuMusicParam = "menu_progress";

	private Window _window;

	private NMainMenuTextButton _continueButton;

	private NMainMenuTextButton _abandonRunButton;

	private NMainMenuTextButton _singleplayerButton;

	private NMainMenuTextButton _compendiumButton;

	private NMainMenuTextButton _timelineButton;

	private NMainMenuTextButton _settingsButton;

	private NMainMenuTextButton _quitButton;

	private NMainMenuTextButton _multiplayerButton;

	private const float _reticleYOffset = 5f;

	private const float _reticlePadding = 28f;

	private Control _buttonReticleLeft;

	private Control _buttonReticleRight;

	private Tween? _reticleTween;

	private NPatchNotesButton _patchNotesButtonNode;

	private NOpenProfileScreenButton _openProfileScreenButton;

	private NMainMenuTextButton? _lastHitButton;

	private NContinueRunInfo _runInfo;

	private Control _timelineNotificationDot;

	private Tween? _backstopTween;

	private NMainMenuBg _bg;

	private ShaderMaterial _blur;

	private bool _openTimeline;

	private ReadSaveResult<SerializableRun>? _readRunSaveResult;

	public static IEnumerable<string> AssetPaths => new _003C_003Ez__ReadOnlySingleElementList<string>("res://scenes/screens/main_menu.tscn");

	public NPatchNotesScreen PatchNotesScreen { get; private set; }

	public Control BlurBackstop { get; private set; }

	private NButton[] MainMenuButtons => new NButton[8] { _continueButton, _abandonRunButton, _singleplayerButton, _multiplayerButton, _timelineButton, _settingsButton, _compendiumButton, _quitButton };

	public NMainMenuSubmenuStack SubmenuStack { get; private set; }

	public NContinueRunInfo ContinueRunInfo => _runInfo;

	public Control DefaultFocusedControl
	{
		get
		{
			if (_lastHitButton == null || !((CanvasItem)_lastHitButton).IsVisible())
			{
				return (Control)(object)MainMenuButtons.First((NButton b) => b.IsEnabled && ((CanvasItem)b).IsVisible());
			}
			return (Control)(object)_lastHitButton;
		}
	}

	public static NMainMenu Create(bool openTimeline)
	{
		NMainMenu nMainMenu = PreloadManager.Cache.GetScene("res://scenes/screens/main_menu.tscn").Instantiate<NMainMenu>((GenEditState)0);
		nMainMenu._openTimeline = openTimeline;
		return nMainMenu;
	}

	public override void _Ready()
	{
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_0298: Unknown result type (might be due to invalid IL or missing references)
		//IL_029e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0407: Unknown result type (might be due to invalid IL or missing references)
		//IL_0411: Expected O, but got Unknown
		//IL_0476: Unknown result type (might be due to invalid IL or missing references)
		//IL_047c: Unknown result type (might be due to invalid IL or missing references)
		Log.Info($"[Startup] Time to main menu (Godot ticks): {Time.GetTicksMsec()}ms");
		_window = ((Node)this).GetTree().Root;
		((GodotObject)NGame.Instance).Connect(NGame.SignalName.WindowChange, Callable.From<bool>((Action<bool>)OnWindowChange), 0u);
		if (SaveManager.Instance.SettingsSave.AspectRatioSetting == AspectRatioSetting.Auto)
		{
			OnWindowChange(isAspectRatioAuto: true);
		}
		_continueButton = ((Node)this).GetNode<NMainMenuTextButton>(NodePath.op_Implicit("MainMenuTextButtons/ContinueButton"));
		((GodotObject)_continueButton).Connect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)OnContinueButtonPressed), 0u);
		_continueButton.SetLocalization("CONTINUE");
		_abandonRunButton = ((Node)this).GetNode<NMainMenuTextButton>(NodePath.op_Implicit("MainMenuTextButtons/AbandonRunButton"));
		((GodotObject)_abandonRunButton).Connect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)OnAbandonRunButtonPressed), 0u);
		_abandonRunButton.SetLocalization("ABANDON_RUN");
		_singleplayerButton = ((Node)this).GetNode<NMainMenuTextButton>(NodePath.op_Implicit("MainMenuTextButtons/SingleplayerButton"));
		((GodotObject)_singleplayerButton).Connect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)SingleplayerButtonPressed), 0u);
		_singleplayerButton.SetLocalization("SINGLE_PLAYER");
		_multiplayerButton = ((Node)this).GetNode<NMainMenuTextButton>(NodePath.op_Implicit("MainMenuTextButtons/MultiplayerButton"));
		((GodotObject)_multiplayerButton).Connect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)OpenMultiplayerSubmenu), 0u);
		_multiplayerButton.SetLocalization("MULTIPLAYER");
		_compendiumButton = ((Node)this).GetNode<NMainMenuTextButton>(NodePath.op_Implicit("MainMenuTextButtons/CompendiumButton"));
		((GodotObject)_compendiumButton).Connect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)OpenCompendiumSubmenu), 0u);
		_compendiumButton.SetLocalization("COMPENDIUM");
		_timelineButton = ((Node)this).GetNode<NMainMenuTextButton>(NodePath.op_Implicit("MainMenuTextButtons/TimelineButton"));
		((GodotObject)_timelineButton).Connect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)OpenTimelineScreen), 0u);
		_timelineButton.SetLocalization("TIMELINE");
		_timelineNotificationDot = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%TimelineNotificationDot"));
		((CanvasItem)_timelineNotificationDot).Visible = SaveManager.Instance.GetDiscoveredEpochCount() > 0;
		_settingsButton = ((Node)this).GetNode<NMainMenuTextButton>(NodePath.op_Implicit("MainMenuTextButtons/SettingsButton"));
		((GodotObject)_settingsButton).Connect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)OpenSettingsMenu), 0u);
		_settingsButton.SetLocalization("SETTINGS");
		_quitButton = ((Node)this).GetNode<NMainMenuTextButton>(NodePath.op_Implicit("MainMenuTextButtons/QuitButton"));
		((GodotObject)_quitButton).Connect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)Quit), 0u);
		_quitButton.SetLocalization("QUIT");
		_buttonReticleLeft = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%ButtonReticleLeft"));
		_buttonReticleRight = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%ButtonReticleRight"));
		ConnectMainMenuTextButtonFocusLogic();
		PatchNotesScreen = ((Node)this).GetNode<NPatchNotesScreen>(NodePath.op_Implicit("%PatchNotesScreen"));
		SubmenuStack = ((Node)this).GetNode<NMainMenuSubmenuStack>(NodePath.op_Implicit("%Submenus"));
		_runInfo = ((Node)this).GetNode<NContinueRunInfo>(NodePath.op_Implicit("%ContinueRunInfo"));
		_patchNotesButtonNode = ((Node)this).GetNode<NPatchNotesButton>(NodePath.op_Implicit("%PatchNotesButton"));
		((GodotObject)_patchNotesButtonNode).Connect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)OpenPatchNotes), 0u);
		_openProfileScreenButton = ((Node)this).GetNode<NOpenProfileScreenButton>(NodePath.op_Implicit("%ChangeProfileButton"));
		_bg = ((Node)this).GetNode<NMainMenuBg>(NodePath.op_Implicit("%MainMenuBg"));
		BlurBackstop = ((Node)this).GetNode<Control>(NodePath.op_Implicit("BlurBackstop"));
		_blur = (ShaderMaterial)((CanvasItem)BlurBackstop).Material;
		((CanvasItem)_timelineButton).Visible = SaveManager.Instance.Progress.Epochs.Count > 0;
		NGame.Instance.SetScreenShakeTarget((Control)(object)this);
		NAudioManager.Instance?.PlayMusic("event:/music/menu_update");
		SubmenuStack.InitializeForMainMenu(this);
		((GodotObject)SubmenuStack).Connect(NSubmenuStack.SignalName.StackModified, Callable.From((Action)OnSubmenuStackChanged), 0u);
		OnSubmenuStackChanged();
		ActiveScreenContext.Instance.Update();
		RefreshButtons();
		CheckCommandLineArgs();
		if (SaveManager.Instance.SettingsSave.ModSettings == null && ModManager.AllMods.Count > 0)
		{
			NModalContainer.Instance.Add((Node)(object)NConfirmModLoadingPopup.Create());
		}
		TaskHelper.RunSafely(NGame.Instance.Transition.FadeIn(3f));
		PlatformUtil.SetRichPresence("MAIN_MENU", null, null);
		if (_openTimeline)
		{
			TaskHelper.RunSafely(OpenTimelineFromGameOverScreen());
		}
		if (NGame.IsReleaseGame() && !SaveManager.Instance.SettingsSave.SeenEaDisclaimer)
		{
			NModalContainer.Instance.Add((Node)(object)NEarlyAccessDisclaimer.Create());
		}
	}

	private void ConnectMainMenuTextButtonFocusLogic()
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		foreach (NMainMenuTextButton item in ((IEnumerable)((Node)((Node)this).GetNode<Control>(NodePath.op_Implicit("%MainMenuTextButtons"))).GetChildren(false)).OfType<NMainMenuTextButton>())
		{
			((GodotObject)item).Connect(NClickableControl.SignalName.Unfocused, Callable.From<NMainMenuTextButton>((Action<NMainMenuTextButton>)MainMenuButtonUnfocused), 0u);
			((GodotObject)item).Connect(NClickableControl.SignalName.Focused, Callable.From<NMainMenuTextButton>((Action<NMainMenuTextButton>)delegate(NMainMenuTextButton b)
			{
				//IL_0020: Unknown result type (might be due to invalid IL or missing references)
				//IL_0025: Unknown result type (might be due to invalid IL or missing references)
				Callable val = Callable.From((Action)delegate
				{
					MainMenuButtonFocused(b);
				});
				((Callable)(ref val)).CallDeferred(Array.Empty<Variant>());
			}), 0u);
		}
	}

	private void MainMenuButtonFocused(NMainMenuTextButton button)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		Tween? reticleTween = _reticleTween;
		if (reticleTween != null)
		{
			reticleTween.Kill();
		}
		_reticleTween = ((Node)this).CreateTween().SetParallel(true);
		_buttonReticleLeft.GlobalPosition = new Vector2(0f, ((Control)button).GlobalPosition.Y + 5f);
		_buttonReticleRight.GlobalPosition = new Vector2(0f, ((Control)button).GlobalPosition.Y + 5f);
		MegaLabel? label = button.label;
		float num = ((label != null) ? ((Control)label).GlobalPosition.X : 0f);
		MegaLabel? label2 = button.label;
		float num2 = ((label2 != null) ? ((Control)label2).Size.X : 0f);
		float num3 = num - 20f - 6f;
		float num4 = num + num2 - 20f + 6f;
		_reticleTween.TweenProperty((GodotObject)(object)_buttonReticleLeft, NodePath.op_Implicit("global_position:x"), Variant.op_Implicit(num3 - 28f), 0.25).SetEase((EaseType)1).SetTrans((TransitionType)10)
			.From(Variant.op_Implicit(num3));
		_reticleTween.TweenProperty((GodotObject)(object)_buttonReticleLeft, NodePath.op_Implicit("modulate"), Variant.op_Implicit(StsColors.gold), 0.05).From(Variant.op_Implicit(StsColors.transparentWhite));
		_reticleTween.TweenProperty((GodotObject)(object)_buttonReticleRight, NodePath.op_Implicit("global_position:x"), Variant.op_Implicit(num4 + 28f), 0.25).SetEase((EaseType)1).SetTrans((TransitionType)10)
			.From(Variant.op_Implicit(num4));
		_reticleTween.TweenProperty((GodotObject)(object)_buttonReticleRight, NodePath.op_Implicit("modulate"), Variant.op_Implicit(StsColors.gold), 0.05).From(Variant.op_Implicit(StsColors.transparentWhite));
	}

	private void MainMenuButtonUnfocused(NMainMenuTextButton obj)
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		Tween? reticleTween = _reticleTween;
		if (reticleTween != null)
		{
			reticleTween.Kill();
		}
		_reticleTween = ((Node)this).CreateTween().SetParallel(true);
		_reticleTween.TweenProperty((GodotObject)(object)_buttonReticleLeft, NodePath.op_Implicit("modulate"), Variant.op_Implicit(StsColors.transparentWhite), 0.25);
		_reticleTween.TweenProperty((GodotObject)(object)_buttonReticleRight, NodePath.op_Implicit("modulate"), Variant.op_Implicit(StsColors.transparentWhite), 0.25);
	}

	private async Task OpenTimelineFromGameOverScreen()
	{
		if (SaveManager.Instance.PrefsSave.FastMode != FastModeType.Instant)
		{
			await Task.Delay(500);
		}
		SubmenuStack.PushSubmenuType<NTimelineScreen>();
	}

	public void EnableBackstop()
	{
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		_bg.HideLogo();
		BlurBackstop.MouseFilter = (MouseFilterEnum)0;
		Tween? backstopTween = _backstopTween;
		if (backstopTween != null)
		{
			backstopTween.Kill();
		}
		_backstopTween = ((Node)this).CreateTween().SetParallel(true);
		_backstopTween.TweenMethod(Callable.From<float>((Action<float>)UpdateShaderLod), _blur.GetShaderParameter(_lod), Variant.op_Implicit(3f), 0.25);
		_backstopTween.TweenMethod(Callable.From<float>((Action<float>)UpdateShaderMix), _blur.GetShaderParameter(_mixPercentage), Variant.op_Implicit(0.7f), 0.25);
	}

	public void DisableBackstop()
	{
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		_bg.ShowLogo();
		BlurBackstop.MouseFilter = (MouseFilterEnum)2;
		Tween? backstopTween = _backstopTween;
		if (backstopTween != null)
		{
			backstopTween.Kill();
		}
		_backstopTween = ((Node)this).CreateTween().SetParallel(true);
		_backstopTween.TweenMethod(Callable.From<float>((Action<float>)UpdateShaderLod), _blur.GetShaderParameter(_lod), Variant.op_Implicit(0f), 0.15);
		_backstopTween.TweenMethod(Callable.From<float>((Action<float>)UpdateShaderMix), _blur.GetShaderParameter(_mixPercentage), Variant.op_Implicit(0f), 0.15);
	}

	public void DisableBackstopInstantly()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		Tween? backstopTween = _backstopTween;
		if (backstopTween != null)
		{
			backstopTween.Kill();
		}
		_blur.SetShaderParameter(_lod, Variant.op_Implicit(0f));
		_blur.SetShaderParameter(_mixPercentage, Variant.op_Implicit(0f));
	}

	public void EnableBackstopInstantly()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		Tween? backstopTween = _backstopTween;
		if (backstopTween != null)
		{
			backstopTween.Kill();
		}
		_blur.SetShaderParameter(_lod, Variant.op_Implicit(3f));
		_blur.SetShaderParameter(_mixPercentage, Variant.op_Implicit(0.7f));
	}

	private void UpdateShaderMix(float obj)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		_blur.SetShaderParameter(_mixPercentage, Variant.op_Implicit(obj));
	}

	private void UpdateShaderLod(float obj)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		_blur.SetShaderParameter(_lod, Variant.op_Implicit(obj));
	}

	public void RefreshButtons()
	{
		if (SaveManager.Instance.HasRunSave)
		{
			_readRunSaveResult = SaveManager.Instance.LoadRunSave();
			((CanvasItem)_singleplayerButton).Visible = false;
			((CanvasItem)_abandonRunButton).Visible = true;
			((CanvasItem)_continueButton).Visible = true;
			_continueButton.SetEnabled(enabled: true);
			_runInfo.SetResult(_readRunSaveResult);
		}
		else
		{
			_readRunSaveResult = null;
			((CanvasItem)_singleplayerButton).Visible = true;
			((CanvasItem)_abandonRunButton).Visible = false;
			((CanvasItem)_continueButton).Visible = false;
			_continueButton.SetEnabled(enabled: false);
			_runInfo.SetResult(null);
		}
		UpdateTimelineButtonBehavior();
		((CanvasItem)_compendiumButton).Visible = SaveManager.Instance.IsCompendiumAvailable();
		ActiveScreenContext.Instance.Update();
	}

	private void UpdateTimelineButtonBehavior()
	{
		if (!DebugSettings.DevSkip && SaveManager.Instance.GetDiscoveredEpochCount() > 0 && !SaveManager.Instance.HasRunSave)
		{
			_timelineButton.Enable();
			_singleplayerButton.Disable();
			_multiplayerButton.Disable();
			_compendiumButton.Disable();
			((CanvasItem)_timelineButton).Visible = true;
			((CanvasItem)_timelineNotificationDot).Visible = true;
		}
		else if (SaveManager.Instance.Progress.Epochs.Count > 1 && SaveManager.Instance.IsEpochRevealed<NeowEpoch>())
		{
			((CanvasItem)_timelineButton).Visible = true;
			if (SaveManager.Instance.GetDiscoveredEpochCount() == 0)
			{
				_timelineButton.Enable();
			}
			else
			{
				_timelineButton.Disable();
			}
			((CanvasItem)_timelineNotificationDot).Visible = false;
			_singleplayerButton.Enable();
			_multiplayerButton.Enable();
			_compendiumButton.Enable();
		}
		else if (SaveManager.Instance.Progress.Epochs.Count > 1)
		{
			_timelineButton.Disable();
		}
		else
		{
			((CanvasItem)_timelineButton).Visible = false;
		}
	}

	private void OnSubmenuStackChanged()
	{
		((CanvasItem)((Node)this).GetNode<Control>(NodePath.op_Implicit("MainMenuTextButtons"))).Visible = !SubmenuStack.SubmenusOpen;
		((CanvasItem)_patchNotesButtonNode).Visible = !SubmenuStack.SubmenusOpen;
		if (SubmenuStack.SubmenusOpen)
		{
			((CanvasItem)_openProfileScreenButton).Visible = false;
			((CanvasItem)_patchNotesButtonNode).Visible = false;
			_openProfileScreenButton.Disable();
			_patchNotesButtonNode.Disable();
		}
		else
		{
			((CanvasItem)_openProfileScreenButton).Visible = true;
			((CanvasItem)_patchNotesButtonNode).Visible = true;
			_openProfileScreenButton.Enable();
			_patchNotesButtonNode.Enable();
			NAudioManager.Instance?.UpdateMusicParameter("menu_progress", "main");
		}
	}

	private void OnContinueButtonPressed(NButton _)
	{
		if (_readRunSaveResult == null || !_readRunSaveResult.Success || _readRunSaveResult.SaveData == null)
		{
			DisplayLoadSaveError();
		}
		else
		{
			TaskHelper.RunSafely(OnContinueButtonPressedAsync());
		}
	}

	private async Task OnContinueButtonPressedAsync()
	{
		_ = 2;
		try
		{
			_continueButton.Disable();
			NAudioManager.Instance?.StopMusic();
			SerializableRun serializableRun = _readRunSaveResult.SaveData;
			RunState runState = RunState.FromSerializable(serializableRun);
			RunManager.Instance.SetUpSavedSinglePlayer(runState, serializableRun);
			Log.Info($"Continuing run with character: {serializableRun.Players[0].CharacterId}");
			SfxCmd.Play(runState.Players[0].Character.CharacterTransitionSfx);
			await NGame.Instance.Transition.FadeOut(0.8f, runState.Players[0].Character.CharacterSelectTransitionPath);
			NGame.Instance.ReactionContainer.InitializeNetworking(new NetSingleplayerGameService());
			await NGame.Instance.LoadRun(runState, serializableRun.PreFinishedRoom);
			await NGame.Instance.Transition.FadeIn();
		}
		catch (Exception)
		{
			DisplayLoadSaveError();
			throw;
		}
	}

	private void DisplayLoadSaveError()
	{
		NErrorPopup modalToCreate = NErrorPopup.Create(new LocString("main_menu_ui", "INVALID_SAVE_POPUP.title"), new LocString("main_menu_ui", "INVALID_SAVE_POPUP.description_run"), new LocString("main_menu_ui", "INVALID_SAVE_POPUP.dismiss"), showReportBugButton: true);
		NModalContainer.Instance.Add((Node)(object)modalToCreate);
		NModalContainer.Instance.ShowBackstop();
		_continueButton.Disable();
	}

	private void OnAbandonRunButtonPressed(NButton _)
	{
		NModalContainer.Instance.Add((Node)(object)NAbandonRunConfirmPopup.Create(this));
		_lastHitButton = _abandonRunButton;
	}

	public void AbandonRun()
	{
		if (_readRunSaveResult == null)
		{
			return;
		}
		if (_readRunSaveResult.Success && _readRunSaveResult.SaveData != null)
		{
			try
			{
				Log.Info("Abandoning run from main menu");
				SerializableRun saveData = _readRunSaveResult.SaveData;
				SaveManager.Instance.UpdateProgressWithRunData(saveData, victory: false);
				RunHistoryUtilities.CreateRunHistoryEntry(saveData, victory: false, isAbandoned: true, saveData.PlatformType);
				if (saveData.DailyTime.HasValue)
				{
					int score = ScoreUtility.CalculateScore(saveData, won: false);
					TaskHelper.RunSafely(DailyRunUtility.UploadScore(saveData.DailyTime.Value, score, saveData.Players));
				}
			}
			catch (Exception value)
			{
				Log.Error($"ERROR: Failed to upload run history/metrics: {value}");
			}
		}
		else
		{
			Log.Info($"Abandoning run with invalid save (status={_readRunSaveResult.Status})");
		}
		SaveManager.Instance.DeleteCurrentRun();
		RefreshButtons();
		GC.Collect();
	}

	private void SingleplayerButtonPressed(NButton _)
	{
		if (SaveManager.Instance.Progress.NumberOfRuns > 0)
		{
			OpenSingleplayerSubmenu();
			return;
		}
		NCharacterSelectScreen submenuType = SubmenuStack.GetSubmenuType<NCharacterSelectScreen>();
		submenuType.InitializeSingleplayer();
		SubmenuStack.Push(submenuType);
		_lastHitButton = _singleplayerButton;
	}

	public NSingleplayerSubmenu OpenSingleplayerSubmenu()
	{
		_lastHitButton = _singleplayerButton;
		return SubmenuStack.PushSubmenuType<NSingleplayerSubmenu>();
	}

	public void OpenMultiplayerSubmenu(NButton _)
	{
		OpenMultiplayerSubmenu();
	}

	public NMultiplayerSubmenu OpenMultiplayerSubmenu()
	{
		_lastHitButton = _multiplayerButton;
		return SubmenuStack.PushSubmenuType<NMultiplayerSubmenu>();
	}

	private void OpenCompendiumSubmenu(NButton _)
	{
		_lastHitButton = _compendiumButton;
		SubmenuStack.PushSubmenuType<NCompendiumSubmenu>();
	}

	private void OpenTimelineScreen(NButton obj)
	{
		_lastHitButton = _timelineButton;
		NAudioManager.Instance?.UpdateMusicParameter("menu_progress", "timeline");
		SubmenuStack.PushSubmenuType<NTimelineScreen>();
	}

	private void OpenSettingsMenu(NButton _)
	{
		OpenSettingsMenu();
	}

	public void OpenProfileScreen()
	{
		SubmenuStack.PushSubmenuType<NProfileScreen>();
	}

	public void OpenSettingsMenu()
	{
		_lastHitButton = _settingsButton;
		SubmenuStack.PushSubmenuType<NSettingsScreen>();
	}

	private void OpenPatchNotes(NButton _)
	{
		PatchNotesScreen.Open();
	}

	public async Task JoinGame(IClientConnectionInitializer connInitializer)
	{
		NMultiplayerSubmenu nMultiplayerSubmenu = OpenMultiplayerSubmenu();
		NJoinFriendScreen nJoinFriendScreen = nMultiplayerSubmenu.OnJoinFriendsPressed();
		await nJoinFriendScreen.JoinGameAsync(connInitializer);
	}

	private static void Quit(NButton _)
	{
		Log.Info("Quit button pressed");
		TaskHelper.RunSafely(ConfirmAndQuit());
	}

	private static async Task ConfirmAndQuit()
	{
		NGenericPopup nGenericPopup = NGenericPopup.Create();
		NModalContainer.Instance.Add((Node)(object)nGenericPopup);
		if (await nGenericPopup.WaitForConfirmation(new LocString("main_menu_ui", "QUIT_CONFIRM_POPUP.body"), new LocString("main_menu_ui", "QUIT_CONFIRM_POPUP.header"), new LocString("main_menu_ui", "GENERIC_POPUP.cancel"), new LocString("main_menu_ui", "GENERIC_POPUP.confirm")))
		{
			Log.Info("Quit confirmed");
			NGame.Instance?.Quit();
		}
		else
		{
			Log.Info("Quit cancelled");
		}
	}

	private void OnWindowChange(bool isAspectRatioAuto)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		if (isAspectRatioAuto)
		{
			float num = (float)_window.Size.X / (float)_window.Size.Y;
			if (num > 2.3888888f)
			{
				_window.ContentScaleAspect = (ContentScaleAspectEnum)2;
				_window.ContentScaleSize = new Vector2I(2580, 1080);
			}
			else if (num < 1.3333334f)
			{
				_window.ContentScaleAspect = (ContentScaleAspectEnum)3;
				_window.ContentScaleSize = new Vector2I(1680, 1260);
			}
			else
			{
				_window.ContentScaleAspect = (ContentScaleAspectEnum)4;
				_window.ContentScaleSize = new Vector2I(1680, 1080);
			}
		}
	}

	private void CheckCommandLineArgs()
	{
		if (!CommandLineHelper.TryGetValue("fastmp", out string value))
		{
			return;
		}
		NMultiplayerSubmenu nMultiplayerSubmenu = OpenMultiplayerSubmenu();
		switch (value)
		{
		case "host":
		case "host_standard":
		case "host_daily":
		case "host_custom":
		{
			GameMode gameMode = value switch
			{
				"host_standard" => GameMode.Standard, 
				"host_daily" => GameMode.Daily, 
				"host_custom" => GameMode.Custom, 
				_ => GameMode.None, 
			};
			if (gameMode != GameMode.None)
			{
				nMultiplayerSubmenu.FastHost(gameMode);
			}
			break;
		}
		case "load":
		{
			PlatformType platformType = (SteamInitializer.Initialized ? PlatformType.Steam : PlatformType.None);
			ulong localPlayerId = PlatformUtil.GetLocalPlayerId(platformType);
			ReadSaveResult<SerializableRun> readSaveResult = SaveManager.Instance.LoadAndCanonicalizeMultiplayerRunSave(localPlayerId);
			if (readSaveResult.SaveData != null)
			{
				nMultiplayerSubmenu.StartHost(readSaveResult.SaveData);
			}
			else
			{
				Log.Error("Failed to load multiplayer save");
			}
			break;
		}
		case "join":
			nMultiplayerSubmenu.OnJoinFriendsPressed();
			break;
		default:
			Log.Error("fastmp command line argument passed with invalid value: " + value + ". Expected host, load, or join");
			break;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Expected O, but got Unknown
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Expected O, but got Unknown
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Expected O, but got Unknown
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		//IL_028a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02de: Unknown result type (might be due to invalid IL or missing references)
		//IL_0304: Unknown result type (might be due to invalid IL or missing references)
		//IL_030d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0333: Unknown result type (might be due to invalid IL or missing references)
		//IL_033c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0362: Unknown result type (might be due to invalid IL or missing references)
		//IL_036b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0391: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c4: Expected O, but got Unknown
		//IL_03bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_041f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0447: Unknown result type (might be due to invalid IL or missing references)
		//IL_0452: Expected O, but got Unknown
		//IL_044d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0458: Unknown result type (might be due to invalid IL or missing references)
		//IL_047e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0487: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e0: Expected O, but got Unknown
		//IL_04db: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0511: Unknown result type (might be due to invalid IL or missing references)
		//IL_051c: Expected O, but got Unknown
		//IL_0517: Unknown result type (might be due to invalid IL or missing references)
		//IL_0520: Unknown result type (might be due to invalid IL or missing references)
		//IL_0546: Unknown result type (might be due to invalid IL or missing references)
		//IL_056e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0579: Expected O, but got Unknown
		//IL_0574: Unknown result type (might be due to invalid IL or missing references)
		//IL_057f: Unknown result type (might be due to invalid IL or missing references)
		//IL_05aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b5: Expected O, but got Unknown
		//IL_05b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_05df: Unknown result type (might be due to invalid IL or missing references)
		//IL_0607: Unknown result type (might be due to invalid IL or missing references)
		//IL_0612: Expected O, but got Unknown
		//IL_060d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0618: Unknown result type (might be due to invalid IL or missing references)
		//IL_063e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0666: Unknown result type (might be due to invalid IL or missing references)
		//IL_0671: Expected O, but got Unknown
		//IL_066c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0677: Unknown result type (might be due to invalid IL or missing references)
		//IL_069d: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d0: Expected O, but got Unknown
		//IL_06cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_06fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0705: Unknown result type (might be due to invalid IL or missing references)
		//IL_072b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0734: Unknown result type (might be due to invalid IL or missing references)
		//IL_075a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0782: Unknown result type (might be due to invalid IL or missing references)
		//IL_078d: Expected O, but got Unknown
		//IL_0788: Unknown result type (might be due to invalid IL or missing references)
		//IL_0793: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_07e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ed: Expected O, but got Unknown
		//IL_07e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0819: Unknown result type (might be due to invalid IL or missing references)
		//IL_083c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0847: Unknown result type (might be due to invalid IL or missing references)
		//IL_086d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0876: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(31);
		list.Add(new MethodInfo(MethodName.Create, new PropertyInfo((Type)24, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false), (MethodFlags)33, new List<PropertyInfo>
		{
			new PropertyInfo((Type)1, StringName.op_Implicit("openTimeline"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ConnectMainMenuTextButtonFocusLogic, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.MainMenuButtonFocused, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("button"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.MainMenuButtonUnfocused, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("obj"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.EnableBackstop, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.DisableBackstop, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.DisableBackstopInstantly, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.EnableBackstopInstantly, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UpdateShaderMix, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)3, StringName.op_Implicit("obj"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UpdateShaderLod, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)3, StringName.op_Implicit("obj"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.RefreshButtons, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UpdateTimelineButtonBehavior, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnSubmenuStackChanged, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnContinueButtonPressed, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.DisplayLoadSaveError, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnAbandonRunButtonPressed, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AbandonRun, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SingleplayerButtonPressed, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OpenSingleplayerSubmenu, new PropertyInfo((Type)24, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OpenMultiplayerSubmenu, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OpenMultiplayerSubmenu, new PropertyInfo((Type)24, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OpenCompendiumSubmenu, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OpenTimelineScreen, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("obj"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OpenSettingsMenu, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OpenProfileScreen, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OpenSettingsMenu, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OpenPatchNotes, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.Quit, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)33, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnWindowChange, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)1, StringName.op_Implicit("isAspectRatioAuto"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.CheckCommandLineArgs, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_032d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0355: Unknown result type (might be due to invalid IL or missing references)
		//IL_035a: Unknown result type (might be due to invalid IL or missing references)
		//IL_038c: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_041e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0451: Unknown result type (might be due to invalid IL or missing references)
		//IL_0476: Unknown result type (might be due to invalid IL or missing references)
		//IL_049b: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_0500: Unknown result type (might be due to invalid IL or missing references)
		//IL_0562: Unknown result type (might be due to invalid IL or missing references)
		//IL_0533: Unknown result type (might be due to invalid IL or missing references)
		//IL_0558: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.Create && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			NMainMenu nMainMenu = Create(VariantUtils.ConvertTo<bool>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<NMainMenu>(ref nMainMenu);
			return true;
		}
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.ConnectMainMenuTextButtonFocusLogic && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			ConnectMainMenuTextButtonFocusLogic();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.MainMenuButtonFocused && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			MainMenuButtonFocused(VariantUtils.ConvertTo<NMainMenuTextButton>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.MainMenuButtonUnfocused && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			MainMenuButtonUnfocused(VariantUtils.ConvertTo<NMainMenuTextButton>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.EnableBackstop && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			EnableBackstop();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.DisableBackstop && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			DisableBackstop();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.DisableBackstopInstantly && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			DisableBackstopInstantly();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.EnableBackstopInstantly && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			EnableBackstopInstantly();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.UpdateShaderMix && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			UpdateShaderMix(VariantUtils.ConvertTo<float>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.UpdateShaderLod && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			UpdateShaderLod(VariantUtils.ConvertTo<float>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.RefreshButtons && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			RefreshButtons();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.UpdateTimelineButtonBehavior && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			UpdateTimelineButtonBehavior();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnSubmenuStackChanged && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnSubmenuStackChanged();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnContinueButtonPressed && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OnContinueButtonPressed(VariantUtils.ConvertTo<NButton>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.DisplayLoadSaveError && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			DisplayLoadSaveError();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnAbandonRunButtonPressed && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OnAbandonRunButtonPressed(VariantUtils.ConvertTo<NButton>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.AbandonRun && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			AbandonRun();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SingleplayerButtonPressed && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			SingleplayerButtonPressed(VariantUtils.ConvertTo<NButton>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OpenSingleplayerSubmenu && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			NSingleplayerSubmenu nSingleplayerSubmenu = OpenSingleplayerSubmenu();
			ret = VariantUtils.CreateFrom<NSingleplayerSubmenu>(ref nSingleplayerSubmenu);
			return true;
		}
		if ((ref method) == MethodName.OpenMultiplayerSubmenu && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OpenMultiplayerSubmenu(VariantUtils.ConvertTo<NButton>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OpenMultiplayerSubmenu && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			NMultiplayerSubmenu nMultiplayerSubmenu = OpenMultiplayerSubmenu();
			ret = VariantUtils.CreateFrom<NMultiplayerSubmenu>(ref nMultiplayerSubmenu);
			return true;
		}
		if ((ref method) == MethodName.OpenCompendiumSubmenu && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OpenCompendiumSubmenu(VariantUtils.ConvertTo<NButton>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OpenTimelineScreen && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OpenTimelineScreen(VariantUtils.ConvertTo<NButton>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OpenSettingsMenu && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OpenSettingsMenu(VariantUtils.ConvertTo<NButton>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OpenProfileScreen && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OpenProfileScreen();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OpenSettingsMenu && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OpenSettingsMenu();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OpenPatchNotes && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OpenPatchNotes(VariantUtils.ConvertTo<NButton>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.Quit && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			Quit(VariantUtils.ConvertTo<NButton>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnWindowChange && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OnWindowChange(VariantUtils.ConvertTo<bool>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.CheckCommandLineArgs && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			CheckCommandLineArgs();
			ret = default(godot_variant);
			return true;
		}
		return ((Control)this).InvokeGodotClassMethod(ref method, args, ref ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static bool InvokeGodotClassStaticMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.Create && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			NMainMenu nMainMenu = Create(VariantUtils.ConvertTo<bool>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<NMainMenu>(ref nMainMenu);
			return true;
		}
		if ((ref method) == MethodName.Quit && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			Quit(VariantUtils.ConvertTo<NButton>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
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
		if ((ref method) == MethodName.ConnectMainMenuTextButtonFocusLogic)
		{
			return true;
		}
		if ((ref method) == MethodName.MainMenuButtonFocused)
		{
			return true;
		}
		if ((ref method) == MethodName.MainMenuButtonUnfocused)
		{
			return true;
		}
		if ((ref method) == MethodName.EnableBackstop)
		{
			return true;
		}
		if ((ref method) == MethodName.DisableBackstop)
		{
			return true;
		}
		if ((ref method) == MethodName.DisableBackstopInstantly)
		{
			return true;
		}
		if ((ref method) == MethodName.EnableBackstopInstantly)
		{
			return true;
		}
		if ((ref method) == MethodName.UpdateShaderMix)
		{
			return true;
		}
		if ((ref method) == MethodName.UpdateShaderLod)
		{
			return true;
		}
		if ((ref method) == MethodName.RefreshButtons)
		{
			return true;
		}
		if ((ref method) == MethodName.UpdateTimelineButtonBehavior)
		{
			return true;
		}
		if ((ref method) == MethodName.OnSubmenuStackChanged)
		{
			return true;
		}
		if ((ref method) == MethodName.OnContinueButtonPressed)
		{
			return true;
		}
		if ((ref method) == MethodName.DisplayLoadSaveError)
		{
			return true;
		}
		if ((ref method) == MethodName.OnAbandonRunButtonPressed)
		{
			return true;
		}
		if ((ref method) == MethodName.AbandonRun)
		{
			return true;
		}
		if ((ref method) == MethodName.SingleplayerButtonPressed)
		{
			return true;
		}
		if ((ref method) == MethodName.OpenSingleplayerSubmenu)
		{
			return true;
		}
		if ((ref method) == MethodName.OpenMultiplayerSubmenu)
		{
			return true;
		}
		if ((ref method) == MethodName.OpenCompendiumSubmenu)
		{
			return true;
		}
		if ((ref method) == MethodName.OpenTimelineScreen)
		{
			return true;
		}
		if ((ref method) == MethodName.OpenSettingsMenu)
		{
			return true;
		}
		if ((ref method) == MethodName.OpenProfileScreen)
		{
			return true;
		}
		if ((ref method) == MethodName.OpenPatchNotes)
		{
			return true;
		}
		if ((ref method) == MethodName.Quit)
		{
			return true;
		}
		if ((ref method) == MethodName.OnWindowChange)
		{
			return true;
		}
		if ((ref method) == MethodName.CheckCommandLineArgs)
		{
			return true;
		}
		return ((Control)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName.PatchNotesScreen)
		{
			PatchNotesScreen = VariantUtils.ConvertTo<NPatchNotesScreen>(ref value);
			return true;
		}
		if ((ref name) == PropertyName.BlurBackstop)
		{
			BlurBackstop = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName.SubmenuStack)
		{
			SubmenuStack = VariantUtils.ConvertTo<NMainMenuSubmenuStack>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._window)
		{
			_window = VariantUtils.ConvertTo<Window>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._continueButton)
		{
			_continueButton = VariantUtils.ConvertTo<NMainMenuTextButton>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._abandonRunButton)
		{
			_abandonRunButton = VariantUtils.ConvertTo<NMainMenuTextButton>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._singleplayerButton)
		{
			_singleplayerButton = VariantUtils.ConvertTo<NMainMenuTextButton>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._compendiumButton)
		{
			_compendiumButton = VariantUtils.ConvertTo<NMainMenuTextButton>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._timelineButton)
		{
			_timelineButton = VariantUtils.ConvertTo<NMainMenuTextButton>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._settingsButton)
		{
			_settingsButton = VariantUtils.ConvertTo<NMainMenuTextButton>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._quitButton)
		{
			_quitButton = VariantUtils.ConvertTo<NMainMenuTextButton>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._multiplayerButton)
		{
			_multiplayerButton = VariantUtils.ConvertTo<NMainMenuTextButton>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._buttonReticleLeft)
		{
			_buttonReticleLeft = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._buttonReticleRight)
		{
			_buttonReticleRight = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._reticleTween)
		{
			_reticleTween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._patchNotesButtonNode)
		{
			_patchNotesButtonNode = VariantUtils.ConvertTo<NPatchNotesButton>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._openProfileScreenButton)
		{
			_openProfileScreenButton = VariantUtils.ConvertTo<NOpenProfileScreenButton>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._lastHitButton)
		{
			_lastHitButton = VariantUtils.ConvertTo<NMainMenuTextButton>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._runInfo)
		{
			_runInfo = VariantUtils.ConvertTo<NContinueRunInfo>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._timelineNotificationDot)
		{
			_timelineNotificationDot = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._backstopTween)
		{
			_backstopTween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._bg)
		{
			_bg = VariantUtils.ConvertTo<NMainMenuBg>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._blur)
		{
			_blur = VariantUtils.ConvertTo<ShaderMaterial>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._openTimeline)
		{
			_openTimeline = VariantUtils.ConvertTo<bool>(ref value);
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
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_026b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0286: Unknown result type (might be due to invalid IL or missing references)
		//IL_028b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0306: Unknown result type (might be due to invalid IL or missing references)
		//IL_030b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0326: Unknown result type (might be due to invalid IL or missing references)
		//IL_032b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0346: Unknown result type (might be due to invalid IL or missing references)
		//IL_034b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0366: Unknown result type (might be due to invalid IL or missing references)
		//IL_036b: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName.PatchNotesScreen)
		{
			NPatchNotesScreen patchNotesScreen = PatchNotesScreen;
			value = VariantUtils.CreateFrom<NPatchNotesScreen>(ref patchNotesScreen);
			return true;
		}
		if ((ref name) == PropertyName.BlurBackstop)
		{
			Control blurBackstop = BlurBackstop;
			value = VariantUtils.CreateFrom<Control>(ref blurBackstop);
			return true;
		}
		if ((ref name) == PropertyName.MainMenuButtons)
		{
			GodotObject[] mainMenuButtons = (GodotObject[])(object)MainMenuButtons;
			value = VariantUtils.CreateFromSystemArrayOfGodotObject(mainMenuButtons);
			return true;
		}
		if ((ref name) == PropertyName.SubmenuStack)
		{
			NMainMenuSubmenuStack submenuStack = SubmenuStack;
			value = VariantUtils.CreateFrom<NMainMenuSubmenuStack>(ref submenuStack);
			return true;
		}
		if ((ref name) == PropertyName.ContinueRunInfo)
		{
			NContinueRunInfo continueRunInfo = ContinueRunInfo;
			value = VariantUtils.CreateFrom<NContinueRunInfo>(ref continueRunInfo);
			return true;
		}
		if ((ref name) == PropertyName.DefaultFocusedControl)
		{
			Control blurBackstop = DefaultFocusedControl;
			value = VariantUtils.CreateFrom<Control>(ref blurBackstop);
			return true;
		}
		if ((ref name) == PropertyName._window)
		{
			value = VariantUtils.CreateFrom<Window>(ref _window);
			return true;
		}
		if ((ref name) == PropertyName._continueButton)
		{
			value = VariantUtils.CreateFrom<NMainMenuTextButton>(ref _continueButton);
			return true;
		}
		if ((ref name) == PropertyName._abandonRunButton)
		{
			value = VariantUtils.CreateFrom<NMainMenuTextButton>(ref _abandonRunButton);
			return true;
		}
		if ((ref name) == PropertyName._singleplayerButton)
		{
			value = VariantUtils.CreateFrom<NMainMenuTextButton>(ref _singleplayerButton);
			return true;
		}
		if ((ref name) == PropertyName._compendiumButton)
		{
			value = VariantUtils.CreateFrom<NMainMenuTextButton>(ref _compendiumButton);
			return true;
		}
		if ((ref name) == PropertyName._timelineButton)
		{
			value = VariantUtils.CreateFrom<NMainMenuTextButton>(ref _timelineButton);
			return true;
		}
		if ((ref name) == PropertyName._settingsButton)
		{
			value = VariantUtils.CreateFrom<NMainMenuTextButton>(ref _settingsButton);
			return true;
		}
		if ((ref name) == PropertyName._quitButton)
		{
			value = VariantUtils.CreateFrom<NMainMenuTextButton>(ref _quitButton);
			return true;
		}
		if ((ref name) == PropertyName._multiplayerButton)
		{
			value = VariantUtils.CreateFrom<NMainMenuTextButton>(ref _multiplayerButton);
			return true;
		}
		if ((ref name) == PropertyName._buttonReticleLeft)
		{
			value = VariantUtils.CreateFrom<Control>(ref _buttonReticleLeft);
			return true;
		}
		if ((ref name) == PropertyName._buttonReticleRight)
		{
			value = VariantUtils.CreateFrom<Control>(ref _buttonReticleRight);
			return true;
		}
		if ((ref name) == PropertyName._reticleTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _reticleTween);
			return true;
		}
		if ((ref name) == PropertyName._patchNotesButtonNode)
		{
			value = VariantUtils.CreateFrom<NPatchNotesButton>(ref _patchNotesButtonNode);
			return true;
		}
		if ((ref name) == PropertyName._openProfileScreenButton)
		{
			value = VariantUtils.CreateFrom<NOpenProfileScreenButton>(ref _openProfileScreenButton);
			return true;
		}
		if ((ref name) == PropertyName._lastHitButton)
		{
			value = VariantUtils.CreateFrom<NMainMenuTextButton>(ref _lastHitButton);
			return true;
		}
		if ((ref name) == PropertyName._runInfo)
		{
			value = VariantUtils.CreateFrom<NContinueRunInfo>(ref _runInfo);
			return true;
		}
		if ((ref name) == PropertyName._timelineNotificationDot)
		{
			value = VariantUtils.CreateFrom<Control>(ref _timelineNotificationDot);
			return true;
		}
		if ((ref name) == PropertyName._backstopTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _backstopTween);
			return true;
		}
		if ((ref name) == PropertyName._bg)
		{
			value = VariantUtils.CreateFrom<NMainMenuBg>(ref _bg);
			return true;
		}
		if ((ref name) == PropertyName._blur)
		{
			value = VariantUtils.CreateFrom<ShaderMaterial>(ref _blur);
			return true;
		}
		if ((ref name) == PropertyName._openTimeline)
		{
			value = VariantUtils.CreateFrom<bool>(ref _openTimeline);
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
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0290: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0313: Unknown result type (might be due to invalid IL or missing references)
		//IL_0334: Unknown result type (might be due to invalid IL or missing references)
		//IL_0355: Unknown result type (might be due to invalid IL or missing references)
		//IL_0376: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName.PatchNotesScreen, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._window, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._continueButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._abandonRunButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._singleplayerButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._compendiumButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._timelineButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._settingsButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._quitButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._multiplayerButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._buttonReticleLeft, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._buttonReticleRight, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._reticleTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._patchNotesButtonNode, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.BlurBackstop, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._openProfileScreenButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)28, PropertyName.MainMenuButtons, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._lastHitButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._runInfo, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._timelineNotificationDot, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._backstopTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._bg, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._blur, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._openTimeline, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.SubmenuStack, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.ContinueRunInfo, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.DefaultFocusedControl, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		StringName patchNotesScreen = PropertyName.PatchNotesScreen;
		NPatchNotesScreen patchNotesScreen2 = PatchNotesScreen;
		info.AddProperty(patchNotesScreen, Variant.From<NPatchNotesScreen>(ref patchNotesScreen2));
		StringName blurBackstop = PropertyName.BlurBackstop;
		Control blurBackstop2 = BlurBackstop;
		info.AddProperty(blurBackstop, Variant.From<Control>(ref blurBackstop2));
		StringName submenuStack = PropertyName.SubmenuStack;
		NMainMenuSubmenuStack submenuStack2 = SubmenuStack;
		info.AddProperty(submenuStack, Variant.From<NMainMenuSubmenuStack>(ref submenuStack2));
		info.AddProperty(PropertyName._window, Variant.From<Window>(ref _window));
		info.AddProperty(PropertyName._continueButton, Variant.From<NMainMenuTextButton>(ref _continueButton));
		info.AddProperty(PropertyName._abandonRunButton, Variant.From<NMainMenuTextButton>(ref _abandonRunButton));
		info.AddProperty(PropertyName._singleplayerButton, Variant.From<NMainMenuTextButton>(ref _singleplayerButton));
		info.AddProperty(PropertyName._compendiumButton, Variant.From<NMainMenuTextButton>(ref _compendiumButton));
		info.AddProperty(PropertyName._timelineButton, Variant.From<NMainMenuTextButton>(ref _timelineButton));
		info.AddProperty(PropertyName._settingsButton, Variant.From<NMainMenuTextButton>(ref _settingsButton));
		info.AddProperty(PropertyName._quitButton, Variant.From<NMainMenuTextButton>(ref _quitButton));
		info.AddProperty(PropertyName._multiplayerButton, Variant.From<NMainMenuTextButton>(ref _multiplayerButton));
		info.AddProperty(PropertyName._buttonReticleLeft, Variant.From<Control>(ref _buttonReticleLeft));
		info.AddProperty(PropertyName._buttonReticleRight, Variant.From<Control>(ref _buttonReticleRight));
		info.AddProperty(PropertyName._reticleTween, Variant.From<Tween>(ref _reticleTween));
		info.AddProperty(PropertyName._patchNotesButtonNode, Variant.From<NPatchNotesButton>(ref _patchNotesButtonNode));
		info.AddProperty(PropertyName._openProfileScreenButton, Variant.From<NOpenProfileScreenButton>(ref _openProfileScreenButton));
		info.AddProperty(PropertyName._lastHitButton, Variant.From<NMainMenuTextButton>(ref _lastHitButton));
		info.AddProperty(PropertyName._runInfo, Variant.From<NContinueRunInfo>(ref _runInfo));
		info.AddProperty(PropertyName._timelineNotificationDot, Variant.From<Control>(ref _timelineNotificationDot));
		info.AddProperty(PropertyName._backstopTween, Variant.From<Tween>(ref _backstopTween));
		info.AddProperty(PropertyName._bg, Variant.From<NMainMenuBg>(ref _bg));
		info.AddProperty(PropertyName._blur, Variant.From<ShaderMaterial>(ref _blur));
		info.AddProperty(PropertyName._openTimeline, Variant.From<bool>(ref _openTimeline));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName.PatchNotesScreen, ref val))
		{
			PatchNotesScreen = ((Variant)(ref val)).As<NPatchNotesScreen>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName.BlurBackstop, ref val2))
		{
			BlurBackstop = ((Variant)(ref val2)).As<Control>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName.SubmenuStack, ref val3))
		{
			SubmenuStack = ((Variant)(ref val3)).As<NMainMenuSubmenuStack>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._window, ref val4))
		{
			_window = ((Variant)(ref val4)).As<Window>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._continueButton, ref val5))
		{
			_continueButton = ((Variant)(ref val5)).As<NMainMenuTextButton>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._abandonRunButton, ref val6))
		{
			_abandonRunButton = ((Variant)(ref val6)).As<NMainMenuTextButton>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName._singleplayerButton, ref val7))
		{
			_singleplayerButton = ((Variant)(ref val7)).As<NMainMenuTextButton>();
		}
		Variant val8 = default(Variant);
		if (info.TryGetProperty(PropertyName._compendiumButton, ref val8))
		{
			_compendiumButton = ((Variant)(ref val8)).As<NMainMenuTextButton>();
		}
		Variant val9 = default(Variant);
		if (info.TryGetProperty(PropertyName._timelineButton, ref val9))
		{
			_timelineButton = ((Variant)(ref val9)).As<NMainMenuTextButton>();
		}
		Variant val10 = default(Variant);
		if (info.TryGetProperty(PropertyName._settingsButton, ref val10))
		{
			_settingsButton = ((Variant)(ref val10)).As<NMainMenuTextButton>();
		}
		Variant val11 = default(Variant);
		if (info.TryGetProperty(PropertyName._quitButton, ref val11))
		{
			_quitButton = ((Variant)(ref val11)).As<NMainMenuTextButton>();
		}
		Variant val12 = default(Variant);
		if (info.TryGetProperty(PropertyName._multiplayerButton, ref val12))
		{
			_multiplayerButton = ((Variant)(ref val12)).As<NMainMenuTextButton>();
		}
		Variant val13 = default(Variant);
		if (info.TryGetProperty(PropertyName._buttonReticleLeft, ref val13))
		{
			_buttonReticleLeft = ((Variant)(ref val13)).As<Control>();
		}
		Variant val14 = default(Variant);
		if (info.TryGetProperty(PropertyName._buttonReticleRight, ref val14))
		{
			_buttonReticleRight = ((Variant)(ref val14)).As<Control>();
		}
		Variant val15 = default(Variant);
		if (info.TryGetProperty(PropertyName._reticleTween, ref val15))
		{
			_reticleTween = ((Variant)(ref val15)).As<Tween>();
		}
		Variant val16 = default(Variant);
		if (info.TryGetProperty(PropertyName._patchNotesButtonNode, ref val16))
		{
			_patchNotesButtonNode = ((Variant)(ref val16)).As<NPatchNotesButton>();
		}
		Variant val17 = default(Variant);
		if (info.TryGetProperty(PropertyName._openProfileScreenButton, ref val17))
		{
			_openProfileScreenButton = ((Variant)(ref val17)).As<NOpenProfileScreenButton>();
		}
		Variant val18 = default(Variant);
		if (info.TryGetProperty(PropertyName._lastHitButton, ref val18))
		{
			_lastHitButton = ((Variant)(ref val18)).As<NMainMenuTextButton>();
		}
		Variant val19 = default(Variant);
		if (info.TryGetProperty(PropertyName._runInfo, ref val19))
		{
			_runInfo = ((Variant)(ref val19)).As<NContinueRunInfo>();
		}
		Variant val20 = default(Variant);
		if (info.TryGetProperty(PropertyName._timelineNotificationDot, ref val20))
		{
			_timelineNotificationDot = ((Variant)(ref val20)).As<Control>();
		}
		Variant val21 = default(Variant);
		if (info.TryGetProperty(PropertyName._backstopTween, ref val21))
		{
			_backstopTween = ((Variant)(ref val21)).As<Tween>();
		}
		Variant val22 = default(Variant);
		if (info.TryGetProperty(PropertyName._bg, ref val22))
		{
			_bg = ((Variant)(ref val22)).As<NMainMenuBg>();
		}
		Variant val23 = default(Variant);
		if (info.TryGetProperty(PropertyName._blur, ref val23))
		{
			_blur = ((Variant)(ref val23)).As<ShaderMaterial>();
		}
		Variant val24 = default(Variant);
		if (info.TryGetProperty(PropertyName._openTimeline, ref val24))
		{
			_openTimeline = ((Variant)(ref val24)).As<bool>();
		}
	}
}
