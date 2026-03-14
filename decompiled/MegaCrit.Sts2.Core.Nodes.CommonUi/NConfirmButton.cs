using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.ControllerInput;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;

namespace MegaCrit.Sts2.Core.Nodes.CommonUi;

[ScriptPath("res://src/Core/Nodes/CommonUi/NConfirmButton.cs")]
public class NConfirmButton : NButton
{
	public new class MethodName : NButton.MethodName
	{
		public new static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public new static readonly StringName _ExitTree = StringName.op_Implicit("_ExitTree");

		public static readonly StringName OnWindowChange = StringName.op_Implicit("OnWindowChange");

		public new static readonly StringName OnEnable = StringName.op_Implicit("OnEnable");

		public new static readonly StringName OnDisable = StringName.op_Implicit("OnDisable");

		public new static readonly StringName OnFocus = StringName.op_Implicit("OnFocus");

		public new static readonly StringName OnUnfocus = StringName.op_Implicit("OnUnfocus");

		public new static readonly StringName OnPress = StringName.op_Implicit("OnPress");

		public static readonly StringName OverrideHotkeys = StringName.op_Implicit("OverrideHotkeys");
	}

	public new class PropertyName : NButton.PropertyName
	{
		public new static readonly StringName Hotkeys = StringName.op_Implicit("Hotkeys");

		public static readonly StringName _outline = StringName.op_Implicit("_outline");

		public static readonly StringName _buttonImage = StringName.op_Implicit("_buttonImage");

		public static readonly StringName _defaultOutlineColor = StringName.op_Implicit("_defaultOutlineColor");

		public static readonly StringName _hoveredOutlineColor = StringName.op_Implicit("_hoveredOutlineColor");

		public static readonly StringName _downColor = StringName.op_Implicit("_downColor");

		public static readonly StringName _outlineColor = StringName.op_Implicit("_outlineColor");

		public static readonly StringName _outlineTransparentColor = StringName.op_Implicit("_outlineTransparentColor");

		public static readonly StringName _viewport = StringName.op_Implicit("_viewport");

		public static readonly StringName _hotkeys = StringName.op_Implicit("_hotkeys");

		public static readonly StringName _posOffset = StringName.op_Implicit("_posOffset");

		public static readonly StringName _showPos = StringName.op_Implicit("_showPos");

		public static readonly StringName _hidePos = StringName.op_Implicit("_hidePos");

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

	private Color _outlineTransparentColor = new Color("00FFFF00");

	private Viewport _viewport;

	private string[] _hotkeys = new string[1] { StringName.op_Implicit(MegaInput.accept) };

	private static readonly Vector2 _hoverScale = new Vector2(1.05f, 1.05f);

	private static readonly Vector2 _downScale = new Vector2(0.95f, 0.95f);

	private const float _pressDownDur = 0.25f;

	private const float _unhoverAnimDur = 0.5f;

	private const float _animInOutDur = 0.35f;

	private Vector2 _posOffset;

	private Vector2 _showPos;

	private Vector2 _hidePos;

	private static readonly Vector2 _hideOffset = new Vector2(180f, 0f);

	private Tween? _moveTween;

	private CancellationTokenSource? _pressDownCancelToken;

	private CancellationTokenSource? _unhoverAnimCancelToken;

	protected override string[] Hotkeys => _hotkeys;

	public override void _Ready()
	{
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		ConnectSignals();
		_isEnabled = false;
		_outline = ((Node)this).GetNode<Control>(NodePath.op_Implicit("Outline"));
		_buttonImage = ((Node)this).GetNode<Control>(NodePath.op_Implicit("Image"));
		_viewport = ((Node)this).GetViewport();
		_posOffset = new Vector2(((Control)this).OffsetRight + 120f, 0f - ((Control)this).OffsetBottom + 110f);
		((GodotObject)((Node)this).GetTree().Root).Connect(SignalName.SizeChanged, Callable.From((Action)OnWindowChange), 0u);
		OnWindowChange();
		OnDisable();
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		_pressDownCancelToken?.Cancel();
		_unhoverAnimCancelToken?.Cancel();
	}

	private void OnWindowChange()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		_showPos = ((Control)NGame.Instance).Size - _posOffset;
		_hidePos = _showPos + _hideOffset;
		((Control)this).Position = (_isEnabled ? _showPos : _hidePos);
	}

	protected override void OnEnable()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		base.OnEnable();
		_isEnabled = true;
		((CanvasItem)_outline).Modulate = Colors.Transparent;
		((CanvasItem)_buttonImage).Modulate = Colors.White;
		Tween? moveTween = _moveTween;
		if (moveTween != null)
		{
			moveTween.Kill();
		}
		_moveTween = ((Node)this).CreateTween();
		_moveTween.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("position"), Variant.op_Implicit(_showPos), 0.3499999940395355).SetEase((EaseType)1).SetTrans((TransitionType)10)
			.FromCurrent();
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
		_moveTween.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("position"), Variant.op_Implicit(_hidePos), 0.3499999940395355).SetEase((EaseType)1).SetTrans((TransitionType)5)
			.FromCurrent();
	}

	protected override void OnFocus()
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		base.OnFocus();
		_unhoverAnimCancelToken?.Cancel();
		((Control)this).Scale = _hoverScale;
		((CanvasItem)_outline).Modulate = _outlineColor;
		((CanvasItem)_buttonImage).Modulate = Colors.White;
	}

	protected override void OnUnfocus()
	{
		base.OnUnfocus();
		_pressDownCancelToken?.Cancel();
		_unhoverAnimCancelToken = new CancellationTokenSource();
		TaskHelper.RunSafely(AnimUnhover(_unhoverAnimCancelToken));
	}

	private async Task AnimUnhover(CancellationTokenSource cancelToken)
	{
		float timer = 0f;
		Vector2 startScale = ((Control)this).Scale;
		Color startButtonColor = ((CanvasItem)_buttonImage).Modulate;
		Color startColor = ((CanvasItem)_outline).Modulate;
		for (; timer < 0.5f; timer += (float)((Node)this).GetProcessDeltaTime())
		{
			if (cancelToken.IsCancellationRequested)
			{
				return;
			}
			((Control)this).Scale = ((Vector2)(ref startScale)).Lerp(Vector2.One, Ease.ExpoOut(timer / 0.5f));
			((CanvasItem)_outline).Modulate = ((Color)(ref startColor)).Lerp(_outlineTransparentColor, Ease.ExpoOut(timer / 0.5f));
			((CanvasItem)_buttonImage).Modulate = ((Color)(ref startButtonColor)).Lerp(Colors.White, Ease.ExpoOut(timer / 0.5f));
			await ((GodotObject)this).ToSignal((GodotObject)(object)((Node)this).GetTree(), SignalName.ProcessFrame);
		}
		((Control)this).Scale = Vector2.One;
		((CanvasItem)_outline).Modulate = _outlineTransparentColor;
		((CanvasItem)_buttonImage).Modulate = Colors.White;
	}

	protected override void OnPress()
	{
		base.OnPress();
		_pressDownCancelToken = new CancellationTokenSource();
		TaskHelper.RunSafely(AnimPressDown(_pressDownCancelToken));
	}

	private async Task AnimPressDown(CancellationTokenSource cancelToken)
	{
		float timer = 0f;
		((CanvasItem)_buttonImage).Modulate = Colors.White;
		((CanvasItem)_outline).Modulate = _outlineColor;
		((Control)this).Scale = _hoverScale;
		for (; timer < 0.25f; timer += (float)((Node)this).GetProcessDeltaTime())
		{
			if (cancelToken.IsCancellationRequested)
			{
				return;
			}
			((Control)this).Scale = ((Vector2)(ref _hoverScale)).Lerp(_downScale, Ease.CubicOut(timer / 0.25f));
			Control buttonImage = _buttonImage;
			Color white = Colors.White;
			((CanvasItem)buttonImage).Modulate = ((Color)(ref white)).Lerp(_downColor, Ease.CubicOut(timer / 0.25f));
			((CanvasItem)_outline).Modulate = ((Color)(ref _outlineColor)).Lerp(_outlineTransparentColor, Ease.CubicOut(timer / 0.25f));
			await ((GodotObject)this).ToSignal((GodotObject)(object)((Node)this).GetTree(), SignalName.ProcessFrame);
		}
		((Control)this).Scale = _downScale;
		((CanvasItem)_buttonImage).Modulate = _downColor;
		((CanvasItem)_outline).Modulate = _outlineTransparentColor;
	}

	public void OverrideHotkeys(string[] hotkeys)
	{
		_hotkeys = hotkeys;
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
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(9);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._ExitTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnWindowChange, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnEnable, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnDisable, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnFocus, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnUnfocus, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnPress, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OverrideHotkeys, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)34, StringName.op_Implicit("hotkeys"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
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
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
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
		if ((ref method) == MethodName.OnWindowChange && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnWindowChange();
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
		if ((ref method) == MethodName.OverrideHotkeys && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OverrideHotkeys(VariantUtils.ConvertTo<string[]>(ref ((NativeVariantPtrArgs)(ref args))[0]));
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
		if ((ref method) == MethodName.OnWindowChange)
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
		if ((ref method) == MethodName.OverrideHotkeys)
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
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
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
		if ((ref name) == PropertyName._viewport)
		{
			_viewport = VariantUtils.ConvertTo<Viewport>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._hotkeys)
		{
			_hotkeys = VariantUtils.ConvertTo<string[]>(ref value);
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
		if ((ref name) == PropertyName.Hotkeys)
		{
			string[] hotkeys = Hotkeys;
			value = VariantUtils.CreateFrom<string[]>(ref hotkeys);
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
		if ((ref name) == PropertyName._viewport)
		{
			value = VariantUtils.CreateFrom<Viewport>(ref _viewport);
			return true;
		}
		if ((ref name) == PropertyName._hotkeys)
		{
			value = VariantUtils.CreateFrom<string[]>(ref _hotkeys);
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
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._outline, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._buttonImage, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)20, PropertyName._defaultOutlineColor, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)20, PropertyName._hoveredOutlineColor, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)20, PropertyName._downColor, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)20, PropertyName._outlineColor, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)20, PropertyName._outlineTransparentColor, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._viewport, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)34, PropertyName._hotkeys, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)34, PropertyName.Hotkeys, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)5, PropertyName._posOffset, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)5, PropertyName._showPos, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)5, PropertyName._hidePos, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName._outline, Variant.From<Control>(ref _outline));
		info.AddProperty(PropertyName._buttonImage, Variant.From<Control>(ref _buttonImage));
		info.AddProperty(PropertyName._defaultOutlineColor, Variant.From<Color>(ref _defaultOutlineColor));
		info.AddProperty(PropertyName._hoveredOutlineColor, Variant.From<Color>(ref _hoveredOutlineColor));
		info.AddProperty(PropertyName._downColor, Variant.From<Color>(ref _downColor));
		info.AddProperty(PropertyName._outlineColor, Variant.From<Color>(ref _outlineColor));
		info.AddProperty(PropertyName._outlineTransparentColor, Variant.From<Color>(ref _outlineTransparentColor));
		info.AddProperty(PropertyName._viewport, Variant.From<Viewport>(ref _viewport));
		info.AddProperty(PropertyName._hotkeys, Variant.From<string[]>(ref _hotkeys));
		info.AddProperty(PropertyName._posOffset, Variant.From<Vector2>(ref _posOffset));
		info.AddProperty(PropertyName._showPos, Variant.From<Vector2>(ref _showPos));
		info.AddProperty(PropertyName._hidePos, Variant.From<Vector2>(ref _hidePos));
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
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
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
		if (info.TryGetProperty(PropertyName._viewport, ref val8))
		{
			_viewport = ((Variant)(ref val8)).As<Viewport>();
		}
		Variant val9 = default(Variant);
		if (info.TryGetProperty(PropertyName._hotkeys, ref val9))
		{
			_hotkeys = ((Variant)(ref val9)).As<string[]>();
		}
		Variant val10 = default(Variant);
		if (info.TryGetProperty(PropertyName._posOffset, ref val10))
		{
			_posOffset = ((Variant)(ref val10)).As<Vector2>();
		}
		Variant val11 = default(Variant);
		if (info.TryGetProperty(PropertyName._showPos, ref val11))
		{
			_showPos = ((Variant)(ref val11)).As<Vector2>();
		}
		Variant val12 = default(Variant);
		if (info.TryGetProperty(PropertyName._hidePos, ref val12))
		{
			_hidePos = ((Variant)(ref val12)).As<Vector2>();
		}
		Variant val13 = default(Variant);
		if (info.TryGetProperty(PropertyName._moveTween, ref val13))
		{
			_moveTween = ((Variant)(ref val13)).As<Tween>();
		}
	}
}
