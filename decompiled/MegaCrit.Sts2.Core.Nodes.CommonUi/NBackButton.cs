using System;
using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.ControllerInput;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;

namespace MegaCrit.Sts2.Core.Nodes.CommonUi;

[ScriptPath("res://src/Core/Nodes/CommonUi/NBackButton.cs")]
public class NBackButton : NButton
{
	public new class MethodName : NButton.MethodName
	{
		public new static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName OnWindowChange = StringName.op_Implicit("OnWindowChange");

		public static readonly StringName MoveToHidePosition = StringName.op_Implicit("MoveToHidePosition");

		public new static readonly StringName OnEnable = StringName.op_Implicit("OnEnable");

		public new static readonly StringName OnDisable = StringName.op_Implicit("OnDisable");

		public new static readonly StringName OnFocus = StringName.op_Implicit("OnFocus");

		public new static readonly StringName OnUnfocus = StringName.op_Implicit("OnUnfocus");

		public new static readonly StringName OnPress = StringName.op_Implicit("OnPress");
	}

	public new class PropertyName : NButton.PropertyName
	{
		public new static readonly StringName ClickedSfx = StringName.op_Implicit("ClickedSfx");

		public new static readonly StringName Hotkeys = StringName.op_Implicit("Hotkeys");

		public new static readonly StringName ControllerIconHotkey = StringName.op_Implicit("ControllerIconHotkey");

		public static readonly StringName _outline = StringName.op_Implicit("_outline");

		public static readonly StringName _buttonImage = StringName.op_Implicit("_buttonImage");

		public static readonly StringName _defaultOutlineColor = StringName.op_Implicit("_defaultOutlineColor");

		public static readonly StringName _hoveredOutlineColor = StringName.op_Implicit("_hoveredOutlineColor");

		public static readonly StringName _downColor = StringName.op_Implicit("_downColor");

		public static readonly StringName _outlineColor = StringName.op_Implicit("_outlineColor");

		public static readonly StringName _outlineTransparentColor = StringName.op_Implicit("_outlineTransparentColor");

		public static readonly StringName _posOffset = StringName.op_Implicit("_posOffset");

		public static readonly StringName _showPos = StringName.op_Implicit("_showPos");

		public static readonly StringName _hidePos = StringName.op_Implicit("_hidePos");

		public static readonly StringName _hoverTween = StringName.op_Implicit("_hoverTween");

		public static readonly StringName _moveTween = StringName.op_Implicit("_moveTween");
	}

	public new class SignalName : NButton.SignalName
	{
	}

	private Control _outline;

	private Control _buttonImage;

	private Color _defaultOutlineColor = StsColors.cream;

	private Color _hoveredOutlineColor = StsColors.gold;

	private Color _downColor = Colors.Gray;

	private Color _outlineColor = new Color("F0B400");

	private Color _outlineTransparentColor = new Color("FF000000");

	private static readonly Vector2 _hoverScale = Vector2.One * 1.05f;

	private static readonly Vector2 _downScale = Vector2.One;

	private const double _animInOutDur = 0.35;

	private Vector2 _posOffset;

	private Vector2 _showPos;

	private Vector2 _hidePos;

	private static readonly Vector2 _hideOffset = new Vector2(-180f, 0f);

	private Tween? _hoverTween;

	private Tween? _moveTween;

	protected override string ClickedSfx => "event:/sfx/ui/clicks/ui_back";

	protected override string[] Hotkeys => new string[3]
	{
		StringName.op_Implicit(MegaInput.cancel),
		StringName.op_Implicit(MegaInput.pauseAndBack),
		StringName.op_Implicit(MegaInput.back)
	};

	protected override string ControllerIconHotkey => StringName.op_Implicit(MegaInput.cancel);

	public override void _Ready()
	{
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		ConnectSignals();
		_outline = ((Node)this).GetNode<Control>(NodePath.op_Implicit("Outline"));
		_buttonImage = ((Node)this).GetNode<Control>(NodePath.op_Implicit("Image"));
		_isEnabled = false;
		_posOffset = new Vector2(((Control)this).OffsetLeft + 80f, 0f - ((Control)this).OffsetBottom + 110f);
		((GodotObject)((Node)this).GetTree().Root).Connect(SignalName.SizeChanged, Callable.From((Action)OnWindowChange), 0u);
		OnWindowChange();
		OnDisable();
	}

	private void OnWindowChange()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		_showPos = new Vector2(0f, (float)((Node)this).GetWindow().ContentScaleSize.Y) - _posOffset;
		_hidePos = _showPos + _hideOffset;
		((Control)this).Position = (_isEnabled ? _showPos : _hidePos);
	}

	public void MoveToHidePosition()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		((Control)this).GlobalPosition = _hidePos;
	}

	protected override void OnEnable()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		base.OnEnable();
		_isEnabled = true;
		((CanvasItem)_outline).Modulate = Colors.Transparent;
		((CanvasItem)_buttonImage).Modulate = Colors.White;
		((Control)this).Scale = Vector2.One;
		Tween? moveTween = _moveTween;
		if (moveTween != null)
		{
			moveTween.Kill();
		}
		_moveTween = ((Node)this).CreateTween();
		_moveTween.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("global_position"), Variant.op_Implicit(_showPos), 0.35).SetEase((EaseType)1).SetTrans((TransitionType)10);
	}

	protected override void OnDisable()
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		base.OnDisable();
		_isEnabled = false;
		Tween? moveTween = _moveTween;
		if (moveTween != null)
		{
			moveTween.Kill();
		}
		_moveTween = ((Node)this).CreateTween();
		_moveTween.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("global_position"), Variant.op_Implicit(_hidePos), 0.35).SetEase((EaseType)1).SetTrans((TransitionType)5);
	}

	protected override void OnFocus()
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		base.OnFocus();
		Tween? hoverTween = _hoverTween;
		if (hoverTween != null)
		{
			hoverTween.Kill();
		}
		_hoverTween = ((Node)this).CreateTween().SetParallel(true);
		_hoverTween.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("scale"), Variant.op_Implicit(_hoverScale), 0.05);
		_hoverTween.TweenProperty((GodotObject)(object)_outline, NodePath.op_Implicit("modulate"), Variant.op_Implicit(_outlineColor), 0.05);
	}

	protected override void OnUnfocus()
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		Tween? hoverTween = _hoverTween;
		if (hoverTween != null)
		{
			hoverTween.Kill();
		}
		_hoverTween = ((Node)this).CreateTween().SetParallel(true);
		_hoverTween.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("scale"), Variant.op_Implicit(_hoverScale), 0.5).SetTrans((TransitionType)5).SetEase((EaseType)1);
		_hoverTween.TweenProperty((GodotObject)(object)_outline, NodePath.op_Implicit("modulate"), Variant.op_Implicit(_outlineTransparentColor), 0.5);
	}

	protected override void OnPress()
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		base.OnPress();
		Tween? hoverTween = _hoverTween;
		if (hoverTween != null)
		{
			hoverTween.Kill();
		}
		_hoverTween = ((Node)this).CreateTween().SetParallel(true);
		_hoverTween.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("scale"), Variant.op_Implicit(_downScale), 0.25).SetTrans((TransitionType)5).SetEase((EaseType)1);
		_hoverTween.TweenProperty((GodotObject)(object)_buttonImage, NodePath.op_Implicit("modulate"), Variant.op_Implicit(_downColor), 0.25).SetTrans((TransitionType)7).SetEase((EaseType)1);
		_hoverTween.TweenProperty((GodotObject)(object)_outline, NodePath.op_Implicit("modulate"), Variant.op_Implicit(_outlineTransparentColor), 0.25).SetTrans((TransitionType)7).SetEase((EaseType)1);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(8);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnWindowChange, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.MoveToHidePosition, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnEnable, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnDisable, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnFocus, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnUnfocus, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnPress, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
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
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
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
		if ((ref method) == MethodName.MoveToHidePosition && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			MoveToHidePosition();
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
		return base.InvokeGodotClassMethod(in method, args, out ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if ((ref method) == MethodName._Ready)
		{
			return true;
		}
		if ((ref method) == MethodName.OnWindowChange)
		{
			return true;
		}
		if ((ref method) == MethodName.MoveToHidePosition)
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
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
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
		if ((ref name) == PropertyName._posOffset)
		{
			_posOffset = VariantUtils.ConvertTo<Vector2>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._showPos)
		{
			_showPos = VariantUtils.ConvertTo<Vector2>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._hidePos)
		{
			_hidePos = VariantUtils.ConvertTo<Vector2>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._hoverTween)
		{
			_hoverTween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._moveTween)
		{
			_moveTween = VariantUtils.ConvertTo<Tween>(ref value);
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
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName.ClickedSfx)
		{
			string clickedSfx = ClickedSfx;
			value = VariantUtils.CreateFrom<string>(ref clickedSfx);
			return true;
		}
		if ((ref name) == PropertyName.Hotkeys)
		{
			string[] hotkeys = Hotkeys;
			value = VariantUtils.CreateFrom<string[]>(ref hotkeys);
			return true;
		}
		if ((ref name) == PropertyName.ControllerIconHotkey)
		{
			string clickedSfx = ControllerIconHotkey;
			value = VariantUtils.CreateFrom<string>(ref clickedSfx);
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
		if ((ref name) == PropertyName._posOffset)
		{
			value = VariantUtils.CreateFrom<Vector2>(ref _posOffset);
			return true;
		}
		if ((ref name) == PropertyName._showPos)
		{
			value = VariantUtils.CreateFrom<Vector2>(ref _showPos);
			return true;
		}
		if ((ref name) == PropertyName._hidePos)
		{
			value = VariantUtils.CreateFrom<Vector2>(ref _hidePos);
			return true;
		}
		if ((ref name) == PropertyName._hoverTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _hoverTween);
			return true;
		}
		if ((ref name) == PropertyName._moveTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _moveTween);
			return true;
		}
		return base.GetGodotClassPropertyValue(in name, out value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)4, PropertyName.ClickedSfx, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)34, PropertyName.Hotkeys, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)4, PropertyName.ControllerIconHotkey, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._outline, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._buttonImage, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)20, PropertyName._defaultOutlineColor, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)20, PropertyName._hoveredOutlineColor, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)20, PropertyName._downColor, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)20, PropertyName._outlineColor, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)20, PropertyName._outlineTransparentColor, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)5, PropertyName._posOffset, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)5, PropertyName._showPos, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)5, PropertyName._hidePos, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._hoverTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._moveTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		info.AddProperty(PropertyName._outline, Variant.From<Control>(ref _outline));
		info.AddProperty(PropertyName._buttonImage, Variant.From<Control>(ref _buttonImage));
		info.AddProperty(PropertyName._defaultOutlineColor, Variant.From<Color>(ref _defaultOutlineColor));
		info.AddProperty(PropertyName._hoveredOutlineColor, Variant.From<Color>(ref _hoveredOutlineColor));
		info.AddProperty(PropertyName._downColor, Variant.From<Color>(ref _downColor));
		info.AddProperty(PropertyName._outlineColor, Variant.From<Color>(ref _outlineColor));
		info.AddProperty(PropertyName._outlineTransparentColor, Variant.From<Color>(ref _outlineTransparentColor));
		info.AddProperty(PropertyName._posOffset, Variant.From<Vector2>(ref _posOffset));
		info.AddProperty(PropertyName._showPos, Variant.From<Vector2>(ref _showPos));
		info.AddProperty(PropertyName._hidePos, Variant.From<Vector2>(ref _hidePos));
		info.AddProperty(PropertyName._hoverTween, Variant.From<Tween>(ref _hoverTween));
		info.AddProperty(PropertyName._moveTween, Variant.From<Tween>(ref _moveTween));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		base.RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._outline, ref val))
		{
			_outline = ((Variant)(ref val)).As<Control>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._buttonImage, ref val2))
		{
			_buttonImage = ((Variant)(ref val2)).As<Control>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._defaultOutlineColor, ref val3))
		{
			_defaultOutlineColor = ((Variant)(ref val3)).As<Color>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._hoveredOutlineColor, ref val4))
		{
			_hoveredOutlineColor = ((Variant)(ref val4)).As<Color>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._downColor, ref val5))
		{
			_downColor = ((Variant)(ref val5)).As<Color>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._outlineColor, ref val6))
		{
			_outlineColor = ((Variant)(ref val6)).As<Color>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName._outlineTransparentColor, ref val7))
		{
			_outlineTransparentColor = ((Variant)(ref val7)).As<Color>();
		}
		Variant val8 = default(Variant);
		if (info.TryGetProperty(PropertyName._posOffset, ref val8))
		{
			_posOffset = ((Variant)(ref val8)).As<Vector2>();
		}
		Variant val9 = default(Variant);
		if (info.TryGetProperty(PropertyName._showPos, ref val9))
		{
			_showPos = ((Variant)(ref val9)).As<Vector2>();
		}
		Variant val10 = default(Variant);
		if (info.TryGetProperty(PropertyName._hidePos, ref val10))
		{
			_hidePos = ((Variant)(ref val10)).As<Vector2>();
		}
		Variant val11 = default(Variant);
		if (info.TryGetProperty(PropertyName._hoverTween, ref val11))
		{
			_hoverTween = ((Variant)(ref val11)).As<Tween>();
		}
		Variant val12 = default(Variant);
		if (info.TryGetProperty(PropertyName._moveTween, ref val12))
		{
			_moveTween = ((Variant)(ref val12)).As<Tween>();
		}
	}
}
