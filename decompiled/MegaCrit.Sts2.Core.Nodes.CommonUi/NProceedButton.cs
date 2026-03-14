using System;
using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.ControllerInput;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.Nodes.CommonUi;

[ScriptPath("res://src/Core/Nodes/CommonUi/NProceedButton.cs")]
public class NProceedButton : NButton
{
	public new class MethodName : NButton.MethodName
	{
		public new static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public new static readonly StringName _ExitTree = StringName.op_Implicit("_ExitTree");

		public new static readonly StringName OnEnable = StringName.op_Implicit("OnEnable");

		public new static readonly StringName OnDisable = StringName.op_Implicit("OnDisable");

		public new static readonly StringName OnRelease = StringName.op_Implicit("OnRelease");

		public new static readonly StringName OnFocus = StringName.op_Implicit("OnFocus");

		public new static readonly StringName OnUnfocus = StringName.op_Implicit("OnUnfocus");

		public new static readonly StringName OnPress = StringName.op_Implicit("OnPress");

		public static readonly StringName DebugToggleVisibility = StringName.op_Implicit("DebugToggleVisibility");

		public static readonly StringName SetPulseState = StringName.op_Implicit("SetPulseState");

		public static readonly StringName StartGlowTween = StringName.op_Implicit("StartGlowTween");

		public static readonly StringName StopGlowTween = StringName.op_Implicit("StopGlowTween");

		public static readonly StringName UpdateShaderS = StringName.op_Implicit("UpdateShaderS");

		public static readonly StringName UpdateShaderV = StringName.op_Implicit("UpdateShaderV");
	}

	public new class PropertyName : NButton.PropertyName
	{
		public static readonly StringName IsSkip = StringName.op_Implicit("IsSkip");

		public new static readonly StringName Hotkeys = StringName.op_Implicit("Hotkeys");

		public static readonly StringName ShowPos = StringName.op_Implicit("ShowPos");

		public static readonly StringName HidePos = StringName.op_Implicit("HidePos");

		public static readonly StringName _outline = StringName.op_Implicit("_outline");

		public static readonly StringName _buttonImage = StringName.op_Implicit("_buttonImage");

		public static readonly StringName _label = StringName.op_Implicit("_label");

		public static readonly StringName _hsv = StringName.op_Implicit("_hsv");

		public static readonly StringName _viewport = StringName.op_Implicit("_viewport");

		public static readonly StringName _defaultOutlineColor = StringName.op_Implicit("_defaultOutlineColor");

		public static readonly StringName _hoveredOutlineColor = StringName.op_Implicit("_hoveredOutlineColor");

		public static readonly StringName _downColor = StringName.op_Implicit("_downColor");

		public static readonly StringName _outlineColor = StringName.op_Implicit("_outlineColor");

		public static readonly StringName _outlineTransparentColor = StringName.op_Implicit("_outlineTransparentColor");

		public static readonly StringName _animTween = StringName.op_Implicit("_animTween");

		public static readonly StringName _glowTween = StringName.op_Implicit("_glowTween");

		public static readonly StringName _hoverTween = StringName.op_Implicit("_hoverTween");

		public static readonly StringName _elapsedTime = StringName.op_Implicit("_elapsedTime");

		public static readonly StringName _shouldPulse = StringName.op_Implicit("_shouldPulse");
	}

	public new class SignalName : NButton.SignalName
	{
	}

	private static readonly StringName _v = new StringName("v");

	private static readonly StringName _s = new StringName("s");

	private Control _outline;

	private Control _buttonImage;

	private MegaLabel _label;

	private ShaderMaterial _hsv;

	private Viewport _viewport;

	private Color _defaultOutlineColor = StsColors.cream;

	private Color _hoveredOutlineColor = StsColors.gold;

	private Color _downColor = Colors.Gray;

	private Color _outlineColor = new Color("FFCC00C0");

	private Color _outlineTransparentColor = new Color("FF000000");

	private Tween? _animTween;

	private Tween? _glowTween;

	private Tween? _hoverTween;

	private static readonly Vector2 _hoverScale = Vector2.One * 1.05f;

	private float _elapsedTime;

	private bool _shouldPulse = true;

	private static readonly Vector2 _showPosRatio = new Vector2(1583f, 764f) / NGame.devResolution;

	private static readonly Vector2 _hidePosRatio = _showPosRatio + new Vector2(400f, 0f) / NGame.devResolution;

	public static LocString ProceedLoc => new LocString("gameplay_ui", "PROCEED_BUTTON");

	public static LocString SkipLoc => new LocString("gameplay_ui", "CHOOSE_CARD_SKIP_BUTTON");

	public bool IsSkip { get; private set; }

	protected override string[] Hotkeys => new string[1] { StringName.op_Implicit(MegaInput.accept) };

	private Vector2 ShowPos
	{
		get
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			Vector2 showPosRatio = _showPosRatio;
			Rect2 visibleRect = _viewport.GetVisibleRect();
			return showPosRatio * ((Rect2)(ref visibleRect)).Size;
		}
	}

	private Vector2 HidePos
	{
		get
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			Vector2 hidePosRatio = _hidePosRatio;
			Rect2 visibleRect = _viewport.GetVisibleRect();
			return hidePosRatio * ((Rect2)(ref visibleRect)).Size;
		}
	}

	public override void _Ready()
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Expected O, but got Unknown
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		ConnectSignals();
		_outline = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%Outline"));
		_buttonImage = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%Image"));
		_hsv = (ShaderMaterial)((CanvasItem)_buttonImage).GetMaterial();
		_label = ((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%Label"));
		_viewport = ((Node)this).GetViewport();
		((Control)this).Position = HidePos;
		Disable();
		NGame.Instance.DebugToggleProceedButton += DebugToggleVisibility;
	}

	public override void _ExitTree()
	{
		NGame.Instance.DebugToggleProceedButton -= DebugToggleVisibility;
	}

	protected override void OnEnable()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		base.OnEnable();
		if (NGame.IsDebugHidingProceedButton)
		{
			((CanvasItem)this).Modulate = Colors.Transparent;
		}
		((Control)this).Scale = Vector2.One;
		((CanvasItem)_buttonImage).SelfModulate = StsColors.gray;
		UpdateShaderS(1f);
		UpdateShaderV(1f);
		Tween? animTween = _animTween;
		if (animTween != null)
		{
			animTween.Kill();
		}
		_animTween = ((Node)this).CreateTween().SetParallel(true);
		_animTween.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("position"), Variant.op_Implicit(ShowPos), 0.5).SetEase((EaseType)1).SetTrans((TransitionType)5);
		_animTween.TweenProperty((GodotObject)(object)_buttonImage, NodePath.op_Implicit("self_modulate"), Variant.op_Implicit(Colors.White), 0.5);
		Tween? hoverTween = _hoverTween;
		if (hoverTween != null)
		{
			hoverTween.Kill();
		}
		_hoverTween = ((Node)this).CreateTween().SetParallel(true);
		_hoverTween.TweenProperty((GodotObject)(object)_outline, NodePath.op_Implicit("modulate"), Variant.op_Implicit(Colors.White), 0.5);
		if (_shouldPulse)
		{
			StartGlowTween();
		}
	}

	protected override void OnDisable()
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		base.OnDisable();
		Tween? animTween = _animTween;
		if (animTween != null)
		{
			animTween.Kill();
		}
		_animTween = ((Node)this).CreateTween().SetParallel(true);
		_animTween.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("position"), Variant.op_Implicit(HidePos), 0.8).SetEase((EaseType)1).SetTrans((TransitionType)5);
		_animTween.TweenProperty((GodotObject)(object)_buttonImage, NodePath.op_Implicit("self_modulate"), Variant.op_Implicit(StsColors.gray), 0.8);
		_animTween.TweenProperty((GodotObject)(object)_outline, NodePath.op_Implicit("modulate"), Variant.op_Implicit(StsColors.transparentWhite), 0.8);
		Tween? glowTween = _glowTween;
		if (glowTween != null)
		{
			glowTween.Kill();
		}
	}

	protected override void OnRelease()
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		if (_isEnabled)
		{
			Tween? hoverTween = _hoverTween;
			if (hoverTween != null)
			{
				hoverTween.Kill();
			}
			_hoverTween = ((Node)this).CreateTween().SetParallel(true);
			_hoverTween.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("scale"), Variant.op_Implicit(Vector2.One), 0.5).SetEase((EaseType)1).SetTrans((TransitionType)5);
			_hoverTween.TweenProperty((GodotObject)(object)_buttonImage, NodePath.op_Implicit("modulate"), Variant.op_Implicit(Colors.White), 0.5);
			_hoverTween.TweenMethod(Callable.From<float>((Action<float>)UpdateShaderV), _hsv.GetShaderParameter(_v), Variant.op_Implicit(1f), 0.05);
			_hoverTween.TweenMethod(Callable.From<float>((Action<float>)UpdateShaderS), _hsv.GetShaderParameter(_s), Variant.op_Implicit(1f), 0.05);
		}
	}

	protected override void OnFocus()
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		Tween? hoverTween = _hoverTween;
		if (hoverTween != null)
		{
			hoverTween.Kill();
		}
		_hoverTween = ((Node)this).CreateTween().SetParallel(true);
		_hoverTween.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("scale"), Variant.op_Implicit(_hoverScale), 0.05);
		_hoverTween.TweenMethod(Callable.From<float>((Action<float>)UpdateShaderV), _hsv.GetShaderParameter(_v), Variant.op_Implicit(1.4f), 0.05);
		Tween? glowTween = _glowTween;
		if (glowTween != null)
		{
			glowTween.Kill();
		}
		_glowTween = ((Node)this).CreateTween();
		_glowTween.TweenProperty((GodotObject)(object)_outline, NodePath.op_Implicit("self_modulate:a"), Variant.op_Implicit(1f), 0.05);
	}

	protected override void OnUnfocus()
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		Tween? hoverTween = _hoverTween;
		if (hoverTween != null)
		{
			hoverTween.Kill();
		}
		_hoverTween = ((Node)this).CreateTween().SetParallel(true);
		_hoverTween.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("scale"), Variant.op_Implicit(Vector2.One), 0.5).SetEase((EaseType)1).SetTrans((TransitionType)5);
		_hoverTween.TweenProperty((GodotObject)(object)_buttonImage, NodePath.op_Implicit("modulate"), Variant.op_Implicit(Colors.White), 0.5);
		_hoverTween.TweenMethod(Callable.From<float>((Action<float>)UpdateShaderV), _hsv.GetShaderParameter(_v), Variant.op_Implicit(1f), 0.05);
		_hoverTween.TweenMethod(Callable.From<float>((Action<float>)UpdateShaderS), _hsv.GetShaderParameter(_s), Variant.op_Implicit(1f), 0.05);
		if (_shouldPulse)
		{
			StartGlowTween();
		}
		else
		{
			StopGlowTween();
		}
	}

	protected override void OnPress()
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		Tween? hoverTween = _hoverTween;
		if (hoverTween != null)
		{
			hoverTween.Kill();
		}
		_hoverTween = ((Node)this).CreateTween().SetParallel(true);
		_hoverTween.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("scale"), Variant.op_Implicit(Vector2.One * 0.95f), 0.5).SetEase((EaseType)1).SetTrans((TransitionType)5);
		_hoverTween.TweenProperty((GodotObject)(object)_buttonImage, NodePath.op_Implicit("modulate"), Variant.op_Implicit(StsColors.gray), 0.5);
		_hoverTween.TweenMethod(Callable.From<float>((Action<float>)UpdateShaderV), _hsv.GetShaderParameter(_v), Variant.op_Implicit(1f), 0.25);
		_hoverTween.TweenMethod(Callable.From<float>((Action<float>)UpdateShaderS), _hsv.GetShaderParameter(_s), Variant.op_Implicit(0.8f), 0.25);
		StopGlowTween();
	}

	public void UpdateText(LocString loc)
	{
		_label.SetTextAutoSize(loc.GetFormattedText());
		IsSkip = loc.LocEntryKey == SkipLoc.LocEntryKey;
	}

	private void DebugToggleVisibility()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		((CanvasItem)this).Modulate = (NGame.IsDebugHidingProceedButton ? Colors.Transparent : Colors.White);
	}

	public void SetPulseState(bool isPulsing)
	{
		_shouldPulse = isPulsing;
		if (isPulsing)
		{
			StartGlowTween();
		}
		else
		{
			StopGlowTween();
		}
	}

	private void StartGlowTween()
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		Tween? glowTween = _glowTween;
		if (glowTween != null)
		{
			glowTween.Kill();
		}
		_glowTween = ((Node)this).CreateTween().SetLoops(0);
		_glowTween.TweenProperty((GodotObject)(object)_outline, NodePath.op_Implicit("self_modulate:a"), Variant.op_Implicit(0.25f), 0.5);
		_glowTween.TweenProperty((GodotObject)(object)_outline, NodePath.op_Implicit("self_modulate:a"), Variant.op_Implicit(0.75f), 0.5);
	}

	private void StopGlowTween()
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		Tween? glowTween = _glowTween;
		if (glowTween != null)
		{
			glowTween.Kill();
		}
		_glowTween = ((Node)this).CreateTween();
		_glowTween.TweenProperty((GodotObject)(object)_outline, NodePath.op_Implicit("self_modulate:a"), Variant.op_Implicit(0f), 0.5);
	}

	private void UpdateShaderS(float value)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		_hsv.SetShaderParameter(_s, Variant.op_Implicit(value));
	}

	private void UpdateShaderV(float value)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		_hsv.SetShaderParameter(_v, Variant.op_Implicit(value));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<MethodInfo> GetGodotMethodList()
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
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(14);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._ExitTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnEnable, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnDisable, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnRelease, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnFocus, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnUnfocus, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnPress, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.DebugToggleVisibility, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetPulseState, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)1, StringName.op_Implicit("isPulsing"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.StartGlowTween, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.StopGlowTween, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UpdateShaderS, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)3, StringName.op_Implicit("value"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UpdateShaderV, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)3, StringName.op_Implicit("value"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
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
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
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
		if ((ref method) == MethodName.OnEnable && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnEnable();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnDisable && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnDisable();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnRelease && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnRelease();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnFocus && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnFocus();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnUnfocus && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnUnfocus();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnPress && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnPress();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.DebugToggleVisibility && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			DebugToggleVisibility();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetPulseState && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			SetPulseState(VariantUtils.ConvertTo<bool>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.StartGlowTween && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			StartGlowTween();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.StopGlowTween && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			StopGlowTween();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.UpdateShaderS && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			UpdateShaderS(VariantUtils.ConvertTo<float>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.UpdateShaderV && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			UpdateShaderV(VariantUtils.ConvertTo<float>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		return base.InvokeGodotClassMethod(in method, args, out ret);
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
		if ((ref method) == MethodName.OnEnable)
		{
			return true;
		}
		if ((ref method) == MethodName.OnDisable)
		{
			return true;
		}
		if ((ref method) == MethodName.OnRelease)
		{
			return true;
		}
		if ((ref method) == MethodName.OnFocus)
		{
			return true;
		}
		if ((ref method) == MethodName.OnUnfocus)
		{
			return true;
		}
		if ((ref method) == MethodName.OnPress)
		{
			return true;
		}
		if ((ref method) == MethodName.DebugToggleVisibility)
		{
			return true;
		}
		if ((ref method) == MethodName.SetPulseState)
		{
			return true;
		}
		if ((ref method) == MethodName.StartGlowTween)
		{
			return true;
		}
		if ((ref method) == MethodName.StopGlowTween)
		{
			return true;
		}
		if ((ref method) == MethodName.UpdateShaderS)
		{
			return true;
		}
		if ((ref method) == MethodName.UpdateShaderV)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName.IsSkip)
		{
			IsSkip = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._outline)
		{
			_outline = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._buttonImage)
		{
			_buttonImage = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._label)
		{
			_label = VariantUtils.ConvertTo<MegaLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._hsv)
		{
			_hsv = VariantUtils.ConvertTo<ShaderMaterial>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._viewport)
		{
			_viewport = VariantUtils.ConvertTo<Viewport>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._defaultOutlineColor)
		{
			_defaultOutlineColor = VariantUtils.ConvertTo<Color>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._hoveredOutlineColor)
		{
			_hoveredOutlineColor = VariantUtils.ConvertTo<Color>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._downColor)
		{
			_downColor = VariantUtils.ConvertTo<Color>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._outlineColor)
		{
			_outlineColor = VariantUtils.ConvertTo<Color>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._outlineTransparentColor)
		{
			_outlineTransparentColor = VariantUtils.ConvertTo<Color>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._animTween)
		{
			_animTween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._glowTween)
		{
			_glowTween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._hoverTween)
		{
			_hoverTween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._elapsedTime)
		{
			_elapsedTime = VariantUtils.ConvertTo<float>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._shouldPulse)
		{
			_shouldPulse = VariantUtils.ConvertTo<bool>(ref value);
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
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName.IsSkip)
		{
			bool isSkip = IsSkip;
			value = VariantUtils.CreateFrom<bool>(ref isSkip);
			return true;
		}
		if ((ref name) == PropertyName.Hotkeys)
		{
			string[] hotkeys = Hotkeys;
			value = VariantUtils.CreateFrom<string[]>(ref hotkeys);
			return true;
		}
		if ((ref name) == PropertyName.ShowPos)
		{
			Vector2 showPos = ShowPos;
			value = VariantUtils.CreateFrom<Vector2>(ref showPos);
			return true;
		}
		if ((ref name) == PropertyName.HidePos)
		{
			Vector2 showPos = HidePos;
			value = VariantUtils.CreateFrom<Vector2>(ref showPos);
			return true;
		}
		if ((ref name) == PropertyName._outline)
		{
			value = VariantUtils.CreateFrom<Control>(ref _outline);
			return true;
		}
		if ((ref name) == PropertyName._buttonImage)
		{
			value = VariantUtils.CreateFrom<Control>(ref _buttonImage);
			return true;
		}
		if ((ref name) == PropertyName._label)
		{
			value = VariantUtils.CreateFrom<MegaLabel>(ref _label);
			return true;
		}
		if ((ref name) == PropertyName._hsv)
		{
			value = VariantUtils.CreateFrom<ShaderMaterial>(ref _hsv);
			return true;
		}
		if ((ref name) == PropertyName._viewport)
		{
			value = VariantUtils.CreateFrom<Viewport>(ref _viewport);
			return true;
		}
		if ((ref name) == PropertyName._defaultOutlineColor)
		{
			value = VariantUtils.CreateFrom<Color>(ref _defaultOutlineColor);
			return true;
		}
		if ((ref name) == PropertyName._hoveredOutlineColor)
		{
			value = VariantUtils.CreateFrom<Color>(ref _hoveredOutlineColor);
			return true;
		}
		if ((ref name) == PropertyName._downColor)
		{
			value = VariantUtils.CreateFrom<Color>(ref _downColor);
			return true;
		}
		if ((ref name) == PropertyName._outlineColor)
		{
			value = VariantUtils.CreateFrom<Color>(ref _outlineColor);
			return true;
		}
		if ((ref name) == PropertyName._outlineTransparentColor)
		{
			value = VariantUtils.CreateFrom<Color>(ref _outlineTransparentColor);
			return true;
		}
		if ((ref name) == PropertyName._animTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _animTween);
			return true;
		}
		if ((ref name) == PropertyName._glowTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _glowTween);
			return true;
		}
		if ((ref name) == PropertyName._hoverTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _hoverTween);
			return true;
		}
		if ((ref name) == PropertyName._elapsedTime)
		{
			value = VariantUtils.CreateFrom<float>(ref _elapsedTime);
			return true;
		}
		if ((ref name) == PropertyName._shouldPulse)
		{
			value = VariantUtils.CreateFrom<bool>(ref _shouldPulse);
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
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)1, PropertyName.IsSkip, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)34, PropertyName.Hotkeys, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._outline, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._buttonImage, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._label, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._hsv, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._viewport, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)20, PropertyName._defaultOutlineColor, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)20, PropertyName._hoveredOutlineColor, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)20, PropertyName._downColor, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)20, PropertyName._outlineColor, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)20, PropertyName._outlineTransparentColor, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._animTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._glowTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._hoverTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)3, PropertyName._elapsedTime, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._shouldPulse, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)5, PropertyName.ShowPos, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)5, PropertyName.HidePos, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		base.SaveGodotObjectData(info);
		StringName isSkip = PropertyName.IsSkip;
		bool isSkip2 = IsSkip;
		info.AddProperty(isSkip, Variant.From<bool>(ref isSkip2));
		info.AddProperty(PropertyName._outline, Variant.From<Control>(ref _outline));
		info.AddProperty(PropertyName._buttonImage, Variant.From<Control>(ref _buttonImage));
		info.AddProperty(PropertyName._label, Variant.From<MegaLabel>(ref _label));
		info.AddProperty(PropertyName._hsv, Variant.From<ShaderMaterial>(ref _hsv));
		info.AddProperty(PropertyName._viewport, Variant.From<Viewport>(ref _viewport));
		info.AddProperty(PropertyName._defaultOutlineColor, Variant.From<Color>(ref _defaultOutlineColor));
		info.AddProperty(PropertyName._hoveredOutlineColor, Variant.From<Color>(ref _hoveredOutlineColor));
		info.AddProperty(PropertyName._downColor, Variant.From<Color>(ref _downColor));
		info.AddProperty(PropertyName._outlineColor, Variant.From<Color>(ref _outlineColor));
		info.AddProperty(PropertyName._outlineTransparentColor, Variant.From<Color>(ref _outlineTransparentColor));
		info.AddProperty(PropertyName._animTween, Variant.From<Tween>(ref _animTween));
		info.AddProperty(PropertyName._glowTween, Variant.From<Tween>(ref _glowTween));
		info.AddProperty(PropertyName._hoverTween, Variant.From<Tween>(ref _hoverTween));
		info.AddProperty(PropertyName._elapsedTime, Variant.From<float>(ref _elapsedTime));
		info.AddProperty(PropertyName._shouldPulse, Variant.From<bool>(ref _shouldPulse));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		base.RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName.IsSkip, ref val))
		{
			IsSkip = ((Variant)(ref val)).As<bool>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._outline, ref val2))
		{
			_outline = ((Variant)(ref val2)).As<Control>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._buttonImage, ref val3))
		{
			_buttonImage = ((Variant)(ref val3)).As<Control>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._label, ref val4))
		{
			_label = ((Variant)(ref val4)).As<MegaLabel>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._hsv, ref val5))
		{
			_hsv = ((Variant)(ref val5)).As<ShaderMaterial>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._viewport, ref val6))
		{
			_viewport = ((Variant)(ref val6)).As<Viewport>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName._defaultOutlineColor, ref val7))
		{
			_defaultOutlineColor = ((Variant)(ref val7)).As<Color>();
		}
		Variant val8 = default(Variant);
		if (info.TryGetProperty(PropertyName._hoveredOutlineColor, ref val8))
		{
			_hoveredOutlineColor = ((Variant)(ref val8)).As<Color>();
		}
		Variant val9 = default(Variant);
		if (info.TryGetProperty(PropertyName._downColor, ref val9))
		{
			_downColor = ((Variant)(ref val9)).As<Color>();
		}
		Variant val10 = default(Variant);
		if (info.TryGetProperty(PropertyName._outlineColor, ref val10))
		{
			_outlineColor = ((Variant)(ref val10)).As<Color>();
		}
		Variant val11 = default(Variant);
		if (info.TryGetProperty(PropertyName._outlineTransparentColor, ref val11))
		{
			_outlineTransparentColor = ((Variant)(ref val11)).As<Color>();
		}
		Variant val12 = default(Variant);
		if (info.TryGetProperty(PropertyName._animTween, ref val12))
		{
			_animTween = ((Variant)(ref val12)).As<Tween>();
		}
		Variant val13 = default(Variant);
		if (info.TryGetProperty(PropertyName._glowTween, ref val13))
		{
			_glowTween = ((Variant)(ref val13)).As<Tween>();
		}
		Variant val14 = default(Variant);
		if (info.TryGetProperty(PropertyName._hoverTween, ref val14))
		{
			_hoverTween = ((Variant)(ref val14)).As<Tween>();
		}
		Variant val15 = default(Variant);
		if (info.TryGetProperty(PropertyName._elapsedTime, ref val15))
		{
			_elapsedTime = ((Variant)(ref val15)).As<float>();
		}
		Variant val16 = default(Variant);
		if (info.TryGetProperty(PropertyName._shouldPulse, ref val16))
		{
			_shouldPulse = ((Variant)(ref val16)).As<bool>();
		}
	}
}
