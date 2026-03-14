using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Screens.Capstones;
using MegaCrit.Sts2.Core.Nodes.Screens.FeedbackScreen;
using MegaCrit.Sts2.Core.Nodes.Screens.MainMenu;
using MegaCrit.Sts2.Core.Nodes.Screens.ModdingScreen;
using MegaCrit.Sts2.Core.Platform;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.Nodes.Screens.Settings;

[ScriptPath("res://src/Core/Nodes/Screens/Settings/NSettingsScreen.cs")]
public class NSettingsScreen : NSubmenu
{
	[Signal]
	public delegate void SettingsClosedEventHandler();

	[Signal]
	public delegate void SettingsOpenedEventHandler();

	public new class MethodName : NSubmenu.MethodName
	{
		public static readonly StringName SetIsInRun = StringName.op_Implicit("SetIsInRun");

		public new static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName OnSettingsTabChanged = StringName.op_Implicit("OnSettingsTabChanged");

		public static readonly StringName LocalizeLabels = StringName.op_Implicit("LocalizeLabels");

		public static readonly StringName OpenModdingScreen = StringName.op_Implicit("OpenModdingScreen");

		public static readonly StringName OpenFeedbackScreen = StringName.op_Implicit("OpenFeedbackScreen");

		public new static readonly StringName OnSubmenuOpened = StringName.op_Implicit("OnSubmenuOpened");

		public new static readonly StringName OnSubmenuClosed = StringName.op_Implicit("OnSubmenuClosed");

		public new static readonly StringName OnSubmenuHidden = StringName.op_Implicit("OnSubmenuHidden");

		public new static readonly StringName OnSubmenuShown = StringName.op_Implicit("OnSubmenuShown");
	}

	public new class PropertyName : NSubmenu.PropertyName
	{
		public new static readonly StringName InitialFocusedControl = StringName.op_Implicit("InitialFocusedControl");

		public static readonly StringName _settingsTabManager = StringName.op_Implicit("_settingsTabManager");

		public static readonly StringName _feedbackScreenButton = StringName.op_Implicit("_feedbackScreenButton");

		public static readonly StringName _moddingScreenButton = StringName.op_Implicit("_moddingScreenButton");

		public static readonly StringName _toast = StringName.op_Implicit("_toast");

		public static readonly StringName _isInRun = StringName.op_Implicit("_isInRun");
	}

	public new class SignalName : NSubmenu.SignalName
	{
		public static readonly StringName SettingsClosed = StringName.op_Implicit("SettingsClosed");

		public static readonly StringName SettingsOpened = StringName.op_Implicit("SettingsOpened");
	}

	private static readonly string _scenePath = SceneHelper.GetScenePath("screens/settings_screen");

	private NSettingsTabManager _settingsTabManager;

	private NOpenFeedbackScreenButton _feedbackScreenButton;

	private NOpenModdingScreenButton _moddingScreenButton;

	private NSettingsToast _toast;

	private bool _isInRun;

	public static readonly Vector2 settingTipsOffset = new Vector2(1012f, -60f);

	private SettingsClosedEventHandler backing_SettingsClosed;

	private SettingsOpenedEventHandler backing_SettingsOpened;

	public static string[] AssetPaths => new string[2] { _scenePath, "res://images/ui/language_warning.png" };

	protected override Control? InitialFocusedControl => _settingsTabManager.DefaultFocusedControl;

	public event SettingsClosedEventHandler SettingsClosed
	{
		add
		{
			backing_SettingsClosed = (SettingsClosedEventHandler)Delegate.Combine(backing_SettingsClosed, value);
		}
		remove
		{
			backing_SettingsClosed = (SettingsClosedEventHandler)Delegate.Remove(backing_SettingsClosed, value);
		}
	}

	public event SettingsOpenedEventHandler SettingsOpened
	{
		add
		{
			backing_SettingsOpened = (SettingsOpenedEventHandler)Delegate.Combine(backing_SettingsOpened, value);
		}
		remove
		{
			backing_SettingsOpened = (SettingsOpenedEventHandler)Delegate.Remove(backing_SettingsOpened, value);
		}
	}

	public void SetIsInRun(bool isInRun)
	{
		_isInRun = isInRun;
	}

	public override void _Ready()
	{
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		ConnectSignals();
		_settingsTabManager = ((Node)this).GetNode<NSettingsTabManager>(NodePath.op_Implicit("%SettingsTabManager"));
		_feedbackScreenButton = ((Node)this).GetNode<NOpenFeedbackScreenButton>(NodePath.op_Implicit("%FeedbackButton"));
		_moddingScreenButton = ((Node)this).GetNode<NOpenModdingScreenButton>(NodePath.op_Implicit("%ModdingButton"));
		_toast = ((Node)this).GetNode<NSettingsToast>(NodePath.op_Implicit("%Toast"));
		LocalizeLabels();
		((Node)this).ProcessMode = (ProcessModeEnum)(((CanvasItem)this).Visible ? 0 : 4);
		((GodotObject)_settingsTabManager).Connect(NSettingsTabManager.SignalName.TabChanged, Callable.From((Action)OnSettingsTabChanged), 0u);
		((GodotObject)_moddingScreenButton).Connect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)OpenModdingScreen), 0u);
		((GodotObject)_feedbackScreenButton).Connect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)OpenFeedbackScreen), 0u);
		if (SaveManager.Instance.SettingsSave.ModSettings != null && ModManager.AllMods.Count > 0)
		{
			((CanvasItem)((Node)this).GetNode<Control>(NodePath.op_Implicit("%Modding"))).Visible = true;
			((CanvasItem)((Node)this).GetNode<Control>(NodePath.op_Implicit("%ModdingDivider"))).Visible = true;
		}
		if (PlatformUtil.GetSupportedWindowMode() == SupportedWindowMode.FullscreenOnly)
		{
			((CanvasItem)((Node)this).GetNode<Control>(NodePath.op_Implicit("%Fullscreen"))).Visible = false;
			((CanvasItem)((Node)this).GetNode<Control>(NodePath.op_Implicit("%FullscreenDivider"))).Visible = false;
		}
		if (RunManager.Instance.IsInProgress)
		{
			((CanvasItem)((Node)this).GetNode<Node>(NodePath.op_Implicit("%LanguageLine")).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("Label"))).Modulate = StsColors.gray;
			NLanguageDropdown node = ((Node)this).GetNode<NLanguageDropdown>(NodePath.op_Implicit("%LanguageDropdown"));
			((CanvasItem)node).Modulate = StsColors.gray;
			node.Disable();
			_moddingScreenButton.Disable();
		}
	}

	public void ShowToast(LocString locString)
	{
		_toast.Show(locString);
	}

	private void OnSettingsTabChanged()
	{
	}

	private void LocalizeLabels()
	{
		Node content = (Node)(object)((Node)this).GetNode<NSettingsPanel>(NodePath.op_Implicit("%GeneralSettings")).Content;
		LocHelper(content.GetNode<Node>(NodePath.op_Implicit("LanguageLine")), new LocString("settings_ui", _isInRun ? "LANGUAGE_IN_RUN" : "LANGUAGE"));
		LocHelper(content.GetNode<Node>(NodePath.op_Implicit("FastMode")), new LocString("settings_ui", "FASTMODE"));
		LocHelper(content.GetNode<Node>(NodePath.op_Implicit("Screenshake")), new LocString("settings_ui", "SCREENSHAKE"));
		LocHelper(content.GetNode<Node>(NodePath.op_Implicit("CommonTooltips")), new LocString("settings_ui", "COMMON_TOOLTIPS"));
		LocHelper(content.GetNode<Node>(NodePath.op_Implicit("ShowRunTimer")), new LocString("settings_ui", "SHOW_RUN_TIMER_HEADER"));
		LocHelper(content.GetNode<Node>(NodePath.op_Implicit("ShowHandCardCount")), new LocString("settings_ui", "SHOW_HAND_CARD_COUNT_HEADER"));
		LocHelper(content.GetNode<Node>(NodePath.op_Implicit("LongPressConfirmations")), new LocString("settings_ui", "LONG_PRESS_CONFIRMATION_HEADER"));
		LocHelper(content.GetNode<Node>(NodePath.op_Implicit("SkipIntroLogo")), new LocString("settings_ui", "SKIP_INTRO_LOGO_HEADER"));
		LocHelper(content.GetNode<Node>(NodePath.op_Implicit("LimitFpsInBackground")), new LocString("settings_ui", "LIMIT_FPS_IN_BACKGROUND_HEADER"));
		LocHelper(content.GetNode<Node>(NodePath.op_Implicit("UploadGameplayData")), new LocString("settings_ui", "UPLOAD_GAMEPLAY_DATA"));
		LocHelper(content.GetNode<Node>(NodePath.op_Implicit("TextEffects")), new LocString("settings_ui", "TEXT_EFFECTS"));
		LocHelper(content.GetNode<Node>(NodePath.op_Implicit("SendFeedback")), new LocString("settings_ui", "SEND_FEEDBACK"));
		LocHelper(content.GetNode<Node>(NodePath.op_Implicit("ResetTutorials")), new LocString("settings_ui", "TUTORIAL_RESET"));
		LocHelper(content.GetNode<Node>(NodePath.op_Implicit("Credits")), new LocString("settings_ui", "CREDITS"));
		LocHelper(content.GetNode<Node>(NodePath.op_Implicit("ResetGameplay")), new LocString("settings_ui", "RESET_DEFAULT"));
		Node content2 = (Node)(object)((Node)this).GetNode<NSettingsPanel>(NodePath.op_Implicit("%GraphicsSettings")).Content;
		LocHelper(content2.GetNode<Node>(NodePath.op_Implicit("Fullscreen")), new LocString("settings_ui", "FULLSCREEN"));
		LocHelper(content2.GetNode<Node>(NodePath.op_Implicit("DisplaySelection")), new LocString("settings_ui", "DISPLAY_SELECTION"));
		LocHelper(content2.GetNode<Node>(NodePath.op_Implicit("WindowedResolution")), new LocString("settings_ui", "RESOLUTION"));
		LocHelper(content2.GetNode<Node>(NodePath.op_Implicit("AspectRatio")), new LocString("settings_ui", "ASPECT_RATIO"));
		LocHelper(content2.GetNode<Node>(NodePath.op_Implicit("WindowResizing")), new LocString("settings_ui", "WINDOW_RESIZING"));
		LocHelper(content2.GetNode<Node>(NodePath.op_Implicit("VSync")), new LocString("settings_ui", "VSYNC"));
		LocHelper(content2.GetNode<Node>(NodePath.op_Implicit("MaxFps")), new LocString("settings_ui", "FPS_CAP"));
		LocHelper(content2.GetNode<Node>(NodePath.op_Implicit("Msaa")), new LocString("settings_ui", "MSAA"));
		LocHelper(content2.GetNode<Node>(NodePath.op_Implicit("ResetGraphics")), new LocString("settings_ui", "RESET_DEFAULT"));
		Node content3 = (Node)(object)((Node)this).GetNode<NSettingsPanel>(NodePath.op_Implicit("%SoundSettings")).Content;
		LocHelper(content3.GetNode<Node>(NodePath.op_Implicit("MasterVolume")), new LocString("settings_ui", "MASTER_VOLUME"));
		LocHelper(content3.GetNode<Node>(NodePath.op_Implicit("BgmVolume")), new LocString("settings_ui", "MUSIC_VOLUME"));
		LocHelper(content3.GetNode<Node>(NodePath.op_Implicit("SfxVolume")), new LocString("settings_ui", "SFX_VOLUME"));
		LocHelper(content3.GetNode<Node>(NodePath.op_Implicit("AmbienceVolume")), new LocString("settings_ui", "AMBIENCE_VOLUME"));
		LocHelper(content3.GetNode<Node>(NodePath.op_Implicit("MuteIfBackground")), new LocString("settings_ui", "BACKGROUND_MUTE"));
	}

	private static void LocHelper(Node settingsLineNode, LocString locString)
	{
		settingsLineNode.GetNode<MegaRichTextLabel>(NodePath.op_Implicit("Label")).Text = locString.GetFormattedText();
	}

	private void OpenModdingScreen(NButton _)
	{
		_stack.PushSubmenuType<NModdingScreen>();
	}

	private void OpenFeedbackScreen(NButton _)
	{
		_lastFocusedControl = (Control?)(object)_feedbackScreenButton;
		TaskHelper.RunSafely(OpenFeedbackScreen());
	}

	public async Task OpenFeedbackScreen()
	{
		Log.Info("Opening feedback screen");
		((CanvasItem)this).Visible = false;
		NGame.Instance.MainMenu?.DisableBackstopInstantly();
		NCapstoneContainer.Instance?.DisableBackstopInstantly();
		NRun.Instance?.GlobalUi.RelicInventory.ShowImmediately();
		NRun.Instance?.GlobalUi.MultiplayerPlayerContainer.ShowImmediately();
		await ((GodotObject)this).ToSignal((GodotObject)(object)((Node)this).GetTree(), SignalName.ProcessFrame);
		await ((GodotObject)this).ToSignal((GodotObject)(object)((Node)this).GetTree(), SignalName.ProcessFrame);
		await ((GodotObject)this).ToSignal((GodotObject)(object)((Node)this).GetTree(), SignalName.ProcessFrame);
		Image image = ((Texture2D)((Node)this).GetViewport().GetTexture()).GetImage();
		((CanvasItem)this).Visible = true;
		NRun.Instance?.GlobalUi.RelicInventory.HideImmediately();
		NRun.Instance?.GlobalUi.MultiplayerPlayerContainer.HideImmediately();
		NGame.Instance.MainMenu?.EnableBackstopInstantly();
		NCapstoneContainer.Instance?.EnableBackstopInstantly();
		NSendFeedbackScreen feedbackScreen = NGame.Instance.FeedbackScreen;
		feedbackScreen.SetScreenshot(image);
		NGame.Instance.FeedbackScreen.Open();
	}

	public override void OnSubmenuOpened()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		base.OnSubmenuOpened();
		((Node)this).ProcessMode = (ProcessModeEnum)0;
		((GodotObject)this).EmitSignal(SignalName.SettingsOpened, Array.Empty<Variant>());
		_settingsTabManager.ResetTabs();
		_settingsTabManager.Enable();
	}

	public override void OnSubmenuClosed()
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		base.OnSubmenuClosed();
		((Node)this).ProcessMode = (ProcessModeEnum)4;
		SaveManager.Instance.SaveSettings();
		SaveManager.Instance.SavePrefsFile();
		((GodotObject)this).EmitSignal(SignalName.SettingsClosed, Array.Empty<Variant>());
		_settingsTabManager.Disable();
	}

	protected override void OnSubmenuHidden()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		base.OnSubmenuClosed();
		((Node)this).ProcessMode = (ProcessModeEnum)4;
		((GodotObject)this).EmitSignal(SignalName.SettingsClosed, Array.Empty<Variant>());
	}

	protected override void OnSubmenuShown()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		base.OnSubmenuShown();
		((Node)this).ProcessMode = (ProcessModeEnum)0;
		((GodotObject)this).EmitSignal(SignalName.SettingsOpened, Array.Empty<Variant>());
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Expected O, but got Unknown
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Expected O, but got Unknown
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(10);
		list.Add(new MethodInfo(MethodName.SetIsInRun, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)1, StringName.op_Implicit("isInRun"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnSettingsTabChanged, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.LocalizeLabels, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OpenModdingScreen, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OpenFeedbackScreen, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnSubmenuOpened, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnSubmenuClosed, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnSubmenuHidden, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnSubmenuShown, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.SetIsInRun && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			SetIsInRun(VariantUtils.ConvertTo<bool>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnSettingsTabChanged && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnSettingsTabChanged();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.LocalizeLabels && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			LocalizeLabels();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OpenModdingScreen && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OpenModdingScreen(VariantUtils.ConvertTo<NButton>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OpenFeedbackScreen && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OpenFeedbackScreen(VariantUtils.ConvertTo<NButton>(ref ((NativeVariantPtrArgs)(ref args))[0]));
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
		if ((ref method) == MethodName.OnSubmenuHidden && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnSubmenuHidden();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnSubmenuShown && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnSubmenuShown();
			ret = default(godot_variant);
			return true;
		}
		return base.InvokeGodotClassMethod(in method, args, out ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if ((ref method) == MethodName.SetIsInRun)
		{
			return true;
		}
		if ((ref method) == MethodName._Ready)
		{
			return true;
		}
		if ((ref method) == MethodName.OnSettingsTabChanged)
		{
			return true;
		}
		if ((ref method) == MethodName.LocalizeLabels)
		{
			return true;
		}
		if ((ref method) == MethodName.OpenModdingScreen)
		{
			return true;
		}
		if ((ref method) == MethodName.OpenFeedbackScreen)
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
		if ((ref method) == MethodName.OnSubmenuHidden)
		{
			return true;
		}
		if ((ref method) == MethodName.OnSubmenuShown)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._settingsTabManager)
		{
			_settingsTabManager = VariantUtils.ConvertTo<NSettingsTabManager>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._feedbackScreenButton)
		{
			_feedbackScreenButton = VariantUtils.ConvertTo<NOpenFeedbackScreenButton>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._moddingScreenButton)
		{
			_moddingScreenButton = VariantUtils.ConvertTo<NOpenModdingScreenButton>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._toast)
		{
			_toast = VariantUtils.ConvertTo<NSettingsToast>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._isInRun)
		{
			_isInRun = VariantUtils.ConvertTo<bool>(ref value);
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
		if ((ref name) == PropertyName.InitialFocusedControl)
		{
			Control initialFocusedControl = InitialFocusedControl;
			value = VariantUtils.CreateFrom<Control>(ref initialFocusedControl);
			return true;
		}
		if ((ref name) == PropertyName._settingsTabManager)
		{
			value = VariantUtils.CreateFrom<NSettingsTabManager>(ref _settingsTabManager);
			return true;
		}
		if ((ref name) == PropertyName._feedbackScreenButton)
		{
			value = VariantUtils.CreateFrom<NOpenFeedbackScreenButton>(ref _feedbackScreenButton);
			return true;
		}
		if ((ref name) == PropertyName._moddingScreenButton)
		{
			value = VariantUtils.CreateFrom<NOpenModdingScreenButton>(ref _moddingScreenButton);
			return true;
		}
		if ((ref name) == PropertyName._toast)
		{
			value = VariantUtils.CreateFrom<NSettingsToast>(ref _toast);
			return true;
		}
		if ((ref name) == PropertyName._isInRun)
		{
			value = VariantUtils.CreateFrom<bool>(ref _isInRun);
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
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName.InitialFocusedControl, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._settingsTabManager, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._feedbackScreenButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._moddingScreenButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._toast, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._isInRun, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName._settingsTabManager, Variant.From<NSettingsTabManager>(ref _settingsTabManager));
		info.AddProperty(PropertyName._feedbackScreenButton, Variant.From<NOpenFeedbackScreenButton>(ref _feedbackScreenButton));
		info.AddProperty(PropertyName._moddingScreenButton, Variant.From<NOpenModdingScreenButton>(ref _moddingScreenButton));
		info.AddProperty(PropertyName._toast, Variant.From<NSettingsToast>(ref _toast));
		info.AddProperty(PropertyName._isInRun, Variant.From<bool>(ref _isInRun));
		info.AddSignalEventDelegate(SignalName.SettingsClosed, (Delegate)backing_SettingsClosed);
		info.AddSignalEventDelegate(SignalName.SettingsOpened, (Delegate)backing_SettingsOpened);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._settingsTabManager, ref val))
		{
			_settingsTabManager = ((Variant)(ref val)).As<NSettingsTabManager>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._feedbackScreenButton, ref val2))
		{
			_feedbackScreenButton = ((Variant)(ref val2)).As<NOpenFeedbackScreenButton>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._moddingScreenButton, ref val3))
		{
			_moddingScreenButton = ((Variant)(ref val3)).As<NOpenModdingScreenButton>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._toast, ref val4))
		{
			_toast = ((Variant)(ref val4)).As<NSettingsToast>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._isInRun, ref val5))
		{
			_isInRun = ((Variant)(ref val5)).As<bool>();
		}
		SettingsClosedEventHandler settingsClosedEventHandler = default(SettingsClosedEventHandler);
		if (info.TryGetSignalEventDelegate<SettingsClosedEventHandler>(SignalName.SettingsClosed, ref settingsClosedEventHandler))
		{
			backing_SettingsClosed = settingsClosedEventHandler;
		}
		SettingsOpenedEventHandler settingsOpenedEventHandler = default(SettingsOpenedEventHandler);
		if (info.TryGetSignalEventDelegate<SettingsOpenedEventHandler>(SignalName.SettingsOpened, ref settingsOpenedEventHandler))
		{
			backing_SettingsOpened = settingsOpenedEventHandler;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotSignalList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(2);
		list.Add(new MethodInfo(SignalName.SettingsClosed, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(SignalName.SettingsOpened, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	protected void EmitSignalSettingsClosed()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).EmitSignal(SignalName.SettingsClosed, Array.Empty<Variant>());
	}

	protected void EmitSignalSettingsOpened()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).EmitSignal(SignalName.SettingsOpened, Array.Empty<Variant>());
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RaiseGodotClassSignalCallbacks(in godot_string_name signal, NativeVariantPtrArgs args)
	{
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		if ((ref signal) == SignalName.SettingsClosed && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			backing_SettingsClosed?.Invoke();
		}
		else if ((ref signal) == SignalName.SettingsOpened && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			backing_SettingsOpened?.Invoke();
		}
		else
		{
			((GodotObject)this).RaiseGodotClassSignalCallbacks(ref signal, args);
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassSignal(in godot_string_name signal)
	{
		if ((ref signal) == SignalName.SettingsClosed)
		{
			return true;
		}
		if ((ref signal) == SignalName.SettingsOpened)
		{
			return true;
		}
		return ((Control)this).HasGodotClassSignal(ref signal);
	}
}
