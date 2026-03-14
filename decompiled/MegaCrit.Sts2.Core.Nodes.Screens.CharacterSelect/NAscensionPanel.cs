using System;
using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.ControllerInput;
using MegaCrit.Sts2.Core.Entities.UI;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.Nodes.Screens.CharacterSelect;

[ScriptPath("res://src/Core/Nodes/Screens/CharacterSelect/NAscensionPanel.cs")]
public class NAscensionPanel : Control
{
	[Signal]
	public delegate void AscensionLevelChangedEventHandler();

	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName Initialize = StringName.op_Implicit("Initialize");

		public static readonly StringName SetFireBlue = StringName.op_Implicit("SetFireBlue");

		public static readonly StringName SetFireRed = StringName.op_Implicit("SetFireRed");

		public static readonly StringName Cleanup = StringName.op_Implicit("Cleanup");

		public static readonly StringName SetAscensionLevel = StringName.op_Implicit("SetAscensionLevel");

		public static readonly StringName IncrementAscension = StringName.op_Implicit("IncrementAscension");

		public static readonly StringName DecrementAscension = StringName.op_Implicit("DecrementAscension");

		public static readonly StringName RefreshArrowVisibility = StringName.op_Implicit("RefreshArrowVisibility");

		public static readonly StringName SetMaxAscension = StringName.op_Implicit("SetMaxAscension");

		public static readonly StringName RefreshAscensionText = StringName.op_Implicit("RefreshAscensionText");

		public static readonly StringName AnimIn = StringName.op_Implicit("AnimIn");

		public static readonly StringName UpdateControllerButton = StringName.op_Implicit("UpdateControllerButton");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName Ascension = StringName.op_Implicit("Ascension");

		public static readonly StringName _maxAscension = StringName.op_Implicit("_maxAscension");

		public static readonly StringName _leftArrow = StringName.op_Implicit("_leftArrow");

		public static readonly StringName _rightArrow = StringName.op_Implicit("_rightArrow");

		public static readonly StringName _ascensionLevel = StringName.op_Implicit("_ascensionLevel");

		public static readonly StringName _info = StringName.op_Implicit("_info");

		public static readonly StringName _leftTriggerIcon = StringName.op_Implicit("_leftTriggerIcon");

		public static readonly StringName _rightTriggerIcon = StringName.op_Implicit("_rightTriggerIcon");

		public static readonly StringName _iconHsv = StringName.op_Implicit("_iconHsv");

		public static readonly StringName _arrowsVisible = StringName.op_Implicit("_arrowsVisible");

		public static readonly StringName _mode = StringName.op_Implicit("_mode");

		public static readonly StringName _tween = StringName.op_Implicit("_tween");
	}

	public class SignalName : SignalName
	{
		public static readonly StringName AscensionLevelChanged = StringName.op_Implicit("AscensionLevelChanged");
	}

	private static readonly StringName _tabLeftHotkey = MegaInput.viewDeckAndTabLeft;

	private static readonly StringName _tabRightHotkey = MegaInput.viewExhaustPileAndTabRight;

	private static readonly StringName _fontOutlineTheme = StringName.op_Implicit("font_outline_color");

	private static readonly StringName _h = new StringName("h");

	private static readonly StringName _v = new StringName("v");

	private static readonly Color _redLabelOutline = new Color("593400");

	private static readonly Color _blueLabelOutline = new Color("004759");

	private int _maxAscension;

	private NButton _leftArrow;

	private NButton _rightArrow;

	private MegaLabel _ascensionLevel;

	private MegaRichTextLabel _info;

	private TextureRect _leftTriggerIcon;

	private TextureRect _rightTriggerIcon;

	private ShaderMaterial _iconHsv;

	private bool _arrowsVisible = true;

	private MultiplayerUiMode _mode = MultiplayerUiMode.Singleplayer;

	private Tween? _tween;

	private AscensionLevelChangedEventHandler backing_AscensionLevelChanged;

	public int Ascension { get; private set; }

	public event AscensionLevelChangedEventHandler AscensionLevelChanged
	{
		add
		{
			backing_AscensionLevelChanged = (AscensionLevelChangedEventHandler)Delegate.Combine(backing_AscensionLevelChanged, value);
		}
		remove
		{
			backing_AscensionLevelChanged = (AscensionLevelChangedEventHandler)Delegate.Remove(backing_AscensionLevelChanged, value);
		}
	}

	public override void _Ready()
	{
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Expected O, but got Unknown
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		_leftTriggerIcon = ((Node)this).GetNode<TextureRect>(NodePath.op_Implicit("%LeftTriggerIcon"));
		_rightTriggerIcon = ((Node)this).GetNode<TextureRect>(NodePath.op_Implicit("%RightTriggerIcon"));
		_leftArrow = ((Node)this).GetNode<NButton>(NodePath.op_Implicit("HBoxContainer/LeftArrowContainer/LeftArrow"));
		_rightArrow = ((Node)this).GetNode<NButton>(NodePath.op_Implicit("HBoxContainer/RightArrowContainer/RightArrow"));
		_ascensionLevel = ((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("HBoxContainer/AscensionIconContainer/AscensionIcon/AscensionLevel"));
		_info = ((Node)this).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("HBoxContainer/AscensionDescription/Description"));
		_iconHsv = (ShaderMaterial)((CanvasItem)((Node)this).GetNode<Control>(NodePath.op_Implicit("%AscensionIcon"))).Material;
		((GodotObject)_leftArrow).Connect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)delegate
		{
			DecrementAscension();
		}), 0u);
		((GodotObject)_rightArrow).Connect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)delegate
		{
			IncrementAscension();
		}), 0u);
		((GodotObject)NControllerManager.Instance).Connect(NControllerManager.SignalName.MouseDetected, Callable.From((Action)UpdateControllerButton), 0u);
		((GodotObject)NControllerManager.Instance).Connect(NControllerManager.SignalName.ControllerDetected, Callable.From((Action)UpdateControllerButton), 0u);
		((GodotObject)NInputManager.Instance).Connect(NInputManager.SignalName.InputRebound, Callable.From((Action)UpdateControllerButton), 0u);
		UpdateControllerButton();
	}

	public void Initialize(MultiplayerUiMode mode)
	{
		_mode = mode;
		if (_mode == MultiplayerUiMode.Host)
		{
			SetFireBlue();
			_arrowsVisible = true;
			SetMaxAscension(SaveManager.Instance.Progress.MaxMultiplayerAscension);
			SetAscensionLevel(Math.Min(_maxAscension, SaveManager.Instance.Progress.PreferredMultiplayerAscension));
			NHotkeyManager.Instance.PushHotkeyPressedBinding(StringName.op_Implicit(_tabLeftHotkey), DecrementAscension);
			NHotkeyManager.Instance.PushHotkeyPressedBinding(StringName.op_Implicit(_tabRightHotkey), IncrementAscension);
		}
		else if (_mode == MultiplayerUiMode.Singleplayer)
		{
			SetFireRed();
			_arrowsVisible = true;
			SetMaxAscension(0);
			SetAscensionLevel(0);
			NHotkeyManager.Instance.PushHotkeyPressedBinding(StringName.op_Implicit(_tabLeftHotkey), DecrementAscension);
			NHotkeyManager.Instance.PushHotkeyPressedBinding(StringName.op_Implicit(_tabRightHotkey), IncrementAscension);
		}
		else
		{
			MultiplayerUiMode mode2 = _mode;
			if ((uint)(mode2 - 3) <= 1u)
			{
				SetFireBlue();
				_arrowsVisible = false;
				SetMaxAscension(0);
			}
		}
	}

	private void SetFireBlue()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		_iconHsv.SetShaderParameter(_h, Variant.op_Implicit(0.52f));
		_iconHsv.SetShaderParameter(_v, Variant.op_Implicit(1.2f));
		((Control)_ascensionLevel).AddThemeColorOverride(_fontOutlineTheme, _blueLabelOutline);
	}

	private void SetFireRed()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		_iconHsv.SetShaderParameter(_h, Variant.op_Implicit(1f));
		_iconHsv.SetShaderParameter(_v, Variant.op_Implicit(1f));
		((Control)_ascensionLevel).AddThemeColorOverride(_fontOutlineTheme, _redLabelOutline);
	}

	public void Cleanup()
	{
		MultiplayerUiMode mode = _mode;
		if ((uint)(mode - 1) <= 1u)
		{
			NHotkeyManager.Instance.RemoveHotkeyPressedBinding(StringName.op_Implicit(_tabLeftHotkey), DecrementAscension);
			NHotkeyManager.Instance.RemoveHotkeyPressedBinding(StringName.op_Implicit(_tabRightHotkey), IncrementAscension);
		}
	}

	public void SetAscensionLevel(int ascension)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		if (Ascension != ascension)
		{
			Ascension = ascension;
			((GodotObject)this).EmitSignal(SignalName.AscensionLevelChanged, Array.Empty<Variant>());
		}
		RefreshAscensionText();
		RefreshArrowVisibility();
	}

	private void IncrementAscension()
	{
		if (Ascension < _maxAscension)
		{
			SetAscensionLevel(Ascension + 1);
		}
	}

	private void DecrementAscension()
	{
		if (Ascension > 0)
		{
			SetAscensionLevel(Ascension - 1);
		}
	}

	private void RefreshArrowVisibility()
	{
		((CanvasItem)_leftArrow).Visible = _arrowsVisible && Ascension != 0;
		((CanvasItem)_rightArrow).Visible = _arrowsVisible && Ascension != _maxAscension;
	}

	public void SetMaxAscension(int maxAscension)
	{
		Log.Info($"Max ascension changed to {maxAscension}");
		_maxAscension = maxAscension;
		if (Ascension >= _maxAscension)
		{
			SetAscensionLevel(_maxAscension);
		}
		((CanvasItem)this).Visible = _maxAscension > 0;
		RefreshArrowVisibility();
	}

	private void RefreshAscensionText()
	{
		_ascensionLevel.SetTextAutoSize(Ascension.ToString());
		string formattedText = AscensionHelper.GetTitle(Ascension).GetFormattedText();
		string formattedText2 = AscensionHelper.GetDescription(Ascension).GetFormattedText();
		_info.Text = "[b][gold]" + formattedText + "[/gold][/b]\n" + formattedText2;
	}

	public void AnimIn()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		if (((CanvasItem)this).Visible)
		{
			Color modulate = ((CanvasItem)this).Modulate;
			modulate.A = 0f;
			((CanvasItem)this).Modulate = modulate;
			_tween?.FastForwardToCompletion();
			_tween = ((Node)this).CreateTween().SetParallel(true);
			_tween.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(1f), 0.2);
			_tween.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("position:y"), Variant.op_Implicit(((Control)this).Position.Y), 0.3).From(Variant.op_Implicit(((Control)this).Position.Y + 30f)).SetEase((EaseType)1)
				.SetTrans((TransitionType)10);
		}
	}

	private void UpdateControllerButton()
	{
		MultiplayerUiMode mode = _mode;
		if ((uint)(mode - 1) <= 1u)
		{
			((CanvasItem)_leftTriggerIcon).Visible = NControllerManager.Instance.IsUsingController;
			((CanvasItem)_rightTriggerIcon).Visible = NControllerManager.Instance.IsUsingController;
			_leftTriggerIcon.Texture = NInputManager.Instance.GetHotkeyIcon(StringName.op_Implicit(MegaInput.viewDeckAndTabLeft));
			_rightTriggerIcon.Texture = NInputManager.Instance.GetHotkeyIcon(StringName.op_Implicit(MegaInput.viewExhaustPileAndTabRight));
		}
		else
		{
			((CanvasItem)_leftTriggerIcon).Visible = false;
			((CanvasItem)_rightTriggerIcon).Visible = false;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_0269: Unknown result type (might be due to invalid IL or missing references)
		//IL_0272: Unknown result type (might be due to invalid IL or missing references)
		//IL_0298: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(13);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.Initialize, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("mode"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetFireBlue, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetFireRed, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.Cleanup, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetAscensionLevel, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("ascension"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.IncrementAscension, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.DecrementAscension, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.RefreshArrowVisibility, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetMaxAscension, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("maxAscension"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.RefreshAscensionText, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AnimIn, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UpdateControllerButton, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.Initialize && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			Initialize(VariantUtils.ConvertTo<MultiplayerUiMode>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetFireBlue && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			SetFireBlue();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetFireRed && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			SetFireRed();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.Cleanup && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			Cleanup();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetAscensionLevel && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			SetAscensionLevel(VariantUtils.ConvertTo<int>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.IncrementAscension && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			IncrementAscension();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.DecrementAscension && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			DecrementAscension();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.RefreshArrowVisibility && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			RefreshArrowVisibility();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetMaxAscension && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			SetMaxAscension(VariantUtils.ConvertTo<int>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.RefreshAscensionText && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			RefreshAscensionText();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.AnimIn && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			AnimIn();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.UpdateControllerButton && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			UpdateControllerButton();
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
		if ((ref method) == MethodName.Initialize)
		{
			return true;
		}
		if ((ref method) == MethodName.SetFireBlue)
		{
			return true;
		}
		if ((ref method) == MethodName.SetFireRed)
		{
			return true;
		}
		if ((ref method) == MethodName.Cleanup)
		{
			return true;
		}
		if ((ref method) == MethodName.SetAscensionLevel)
		{
			return true;
		}
		if ((ref method) == MethodName.IncrementAscension)
		{
			return true;
		}
		if ((ref method) == MethodName.DecrementAscension)
		{
			return true;
		}
		if ((ref method) == MethodName.RefreshArrowVisibility)
		{
			return true;
		}
		if ((ref method) == MethodName.SetMaxAscension)
		{
			return true;
		}
		if ((ref method) == MethodName.RefreshAscensionText)
		{
			return true;
		}
		if ((ref method) == MethodName.AnimIn)
		{
			return true;
		}
		if ((ref method) == MethodName.UpdateControllerButton)
		{
			return true;
		}
		return ((Control)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName.Ascension)
		{
			Ascension = VariantUtils.ConvertTo<int>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._maxAscension)
		{
			_maxAscension = VariantUtils.ConvertTo<int>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._leftArrow)
		{
			_leftArrow = VariantUtils.ConvertTo<NButton>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._rightArrow)
		{
			_rightArrow = VariantUtils.ConvertTo<NButton>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._ascensionLevel)
		{
			_ascensionLevel = VariantUtils.ConvertTo<MegaLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._info)
		{
			_info = VariantUtils.ConvertTo<MegaRichTextLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._leftTriggerIcon)
		{
			_leftTriggerIcon = VariantUtils.ConvertTo<TextureRect>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._rightTriggerIcon)
		{
			_rightTriggerIcon = VariantUtils.ConvertTo<TextureRect>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._iconHsv)
		{
			_iconHsv = VariantUtils.ConvertTo<ShaderMaterial>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._arrowsVisible)
		{
			_arrowsVisible = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._mode)
		{
			_mode = VariantUtils.ConvertTo<MultiplayerUiMode>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._tween)
		{
			_tween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		return ((GodotObject)this).SetGodotClassPropertyValue(ref name, ref value);
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
		if ((ref name) == PropertyName.Ascension)
		{
			int ascension = Ascension;
			value = VariantUtils.CreateFrom<int>(ref ascension);
			return true;
		}
		if ((ref name) == PropertyName._maxAscension)
		{
			value = VariantUtils.CreateFrom<int>(ref _maxAscension);
			return true;
		}
		if ((ref name) == PropertyName._leftArrow)
		{
			value = VariantUtils.CreateFrom<NButton>(ref _leftArrow);
			return true;
		}
		if ((ref name) == PropertyName._rightArrow)
		{
			value = VariantUtils.CreateFrom<NButton>(ref _rightArrow);
			return true;
		}
		if ((ref name) == PropertyName._ascensionLevel)
		{
			value = VariantUtils.CreateFrom<MegaLabel>(ref _ascensionLevel);
			return true;
		}
		if ((ref name) == PropertyName._info)
		{
			value = VariantUtils.CreateFrom<MegaRichTextLabel>(ref _info);
			return true;
		}
		if ((ref name) == PropertyName._leftTriggerIcon)
		{
			value = VariantUtils.CreateFrom<TextureRect>(ref _leftTriggerIcon);
			return true;
		}
		if ((ref name) == PropertyName._rightTriggerIcon)
		{
			value = VariantUtils.CreateFrom<TextureRect>(ref _rightTriggerIcon);
			return true;
		}
		if ((ref name) == PropertyName._iconHsv)
		{
			value = VariantUtils.CreateFrom<ShaderMaterial>(ref _iconHsv);
			return true;
		}
		if ((ref name) == PropertyName._arrowsVisible)
		{
			value = VariantUtils.CreateFrom<bool>(ref _arrowsVisible);
			return true;
		}
		if ((ref name) == PropertyName._mode)
		{
			value = VariantUtils.CreateFrom<MultiplayerUiMode>(ref _mode);
			return true;
		}
		if ((ref name) == PropertyName._tween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _tween);
			return true;
		}
		return ((GodotObject)this).GetGodotClassPropertyValue(ref name, ref value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)2, PropertyName.Ascension, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)2, PropertyName._maxAscension, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._leftArrow, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._rightArrow, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._ascensionLevel, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._info, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._leftTriggerIcon, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._rightTriggerIcon, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._iconHsv, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._arrowsVisible, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)2, PropertyName._mode, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._tween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		StringName ascension = PropertyName.Ascension;
		int ascension2 = Ascension;
		info.AddProperty(ascension, Variant.From<int>(ref ascension2));
		info.AddProperty(PropertyName._maxAscension, Variant.From<int>(ref _maxAscension));
		info.AddProperty(PropertyName._leftArrow, Variant.From<NButton>(ref _leftArrow));
		info.AddProperty(PropertyName._rightArrow, Variant.From<NButton>(ref _rightArrow));
		info.AddProperty(PropertyName._ascensionLevel, Variant.From<MegaLabel>(ref _ascensionLevel));
		info.AddProperty(PropertyName._info, Variant.From<MegaRichTextLabel>(ref _info));
		info.AddProperty(PropertyName._leftTriggerIcon, Variant.From<TextureRect>(ref _leftTriggerIcon));
		info.AddProperty(PropertyName._rightTriggerIcon, Variant.From<TextureRect>(ref _rightTriggerIcon));
		info.AddProperty(PropertyName._iconHsv, Variant.From<ShaderMaterial>(ref _iconHsv));
		info.AddProperty(PropertyName._arrowsVisible, Variant.From<bool>(ref _arrowsVisible));
		info.AddProperty(PropertyName._mode, Variant.From<MultiplayerUiMode>(ref _mode));
		info.AddProperty(PropertyName._tween, Variant.From<Tween>(ref _tween));
		info.AddSignalEventDelegate(SignalName.AscensionLevelChanged, (Delegate)backing_AscensionLevelChanged);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName.Ascension, ref val))
		{
			Ascension = ((Variant)(ref val)).As<int>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._maxAscension, ref val2))
		{
			_maxAscension = ((Variant)(ref val2)).As<int>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._leftArrow, ref val3))
		{
			_leftArrow = ((Variant)(ref val3)).As<NButton>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._rightArrow, ref val4))
		{
			_rightArrow = ((Variant)(ref val4)).As<NButton>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._ascensionLevel, ref val5))
		{
			_ascensionLevel = ((Variant)(ref val5)).As<MegaLabel>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._info, ref val6))
		{
			_info = ((Variant)(ref val6)).As<MegaRichTextLabel>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName._leftTriggerIcon, ref val7))
		{
			_leftTriggerIcon = ((Variant)(ref val7)).As<TextureRect>();
		}
		Variant val8 = default(Variant);
		if (info.TryGetProperty(PropertyName._rightTriggerIcon, ref val8))
		{
			_rightTriggerIcon = ((Variant)(ref val8)).As<TextureRect>();
		}
		Variant val9 = default(Variant);
		if (info.TryGetProperty(PropertyName._iconHsv, ref val9))
		{
			_iconHsv = ((Variant)(ref val9)).As<ShaderMaterial>();
		}
		Variant val10 = default(Variant);
		if (info.TryGetProperty(PropertyName._arrowsVisible, ref val10))
		{
			_arrowsVisible = ((Variant)(ref val10)).As<bool>();
		}
		Variant val11 = default(Variant);
		if (info.TryGetProperty(PropertyName._mode, ref val11))
		{
			_mode = ((Variant)(ref val11)).As<MultiplayerUiMode>();
		}
		Variant val12 = default(Variant);
		if (info.TryGetProperty(PropertyName._tween, ref val12))
		{
			_tween = ((Variant)(ref val12)).As<Tween>();
		}
		AscensionLevelChangedEventHandler ascensionLevelChangedEventHandler = default(AscensionLevelChangedEventHandler);
		if (info.TryGetSignalEventDelegate<AscensionLevelChangedEventHandler>(SignalName.AscensionLevelChanged, ref ascensionLevelChangedEventHandler))
		{
			backing_AscensionLevelChanged = ascensionLevelChangedEventHandler;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotSignalList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(1);
		list.Add(new MethodInfo(SignalName.AscensionLevelChanged, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	protected void EmitSignalAscensionLevelChanged()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).EmitSignal(SignalName.AscensionLevelChanged, Array.Empty<Variant>());
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RaiseGodotClassSignalCallbacks(in godot_string_name signal, NativeVariantPtrArgs args)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		if ((ref signal) == SignalName.AscensionLevelChanged && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			backing_AscensionLevelChanged?.Invoke();
		}
		else
		{
			((GodotObject)this).RaiseGodotClassSignalCallbacks(ref signal, args);
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassSignal(in godot_string_name signal)
	{
		if ((ref signal) == SignalName.AscensionLevelChanged)
		{
			return true;
		}
		return ((Control)this).HasGodotClassSignal(ref signal);
	}
}
