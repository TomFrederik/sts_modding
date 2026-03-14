using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;

namespace MegaCrit.Sts2.Core.Nodes.CommonUi;

[ScriptPath("res://src/Core/Nodes/CommonUi/NMiscConfirmButton.cs")]
public class NMiscConfirmButton : NButton
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
	}

	public new class PropertyName : NButton.PropertyName
	{
		public static readonly StringName _buttonImage = StringName.op_Implicit("_buttonImage");

		public static readonly StringName _downColor = StringName.op_Implicit("_downColor");

		public static readonly StringName _showPos = StringName.op_Implicit("_showPos");

		public static readonly StringName _hidePos = StringName.op_Implicit("_hidePos");

		public static readonly StringName _moveTween = StringName.op_Implicit("_moveTween");
	}

	public new class SignalName : NButton.SignalName
	{
	}

	private Control _buttonImage;

	private Color _downColor = Colors.Gray;

	private static readonly Vector2 _hoverScale = new Vector2(1.05f, 1.05f);

	private static readonly Vector2 _downScale = new Vector2(0.95f, 0.95f);

	private const float _pressDownDur = 0.25f;

	private const float _unhoverAnimDur = 0.5f;

	private const float _animInOutDur = 0.35f;

	private Vector2 _showPos;

	private Vector2 _hidePos;

	private Tween? _moveTween;

	private CancellationTokenSource? _pressDownCancelToken;

	private CancellationTokenSource? _unhoverAnimCancelToken;

	public override void _Ready()
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		ConnectSignals();
		_isEnabled = false;
		_buttonImage = ((Node)this).GetNode<Control>(NodePath.op_Implicit("Image"));
		((GodotObject)((Node)this).GetTree().Root).Connect(SignalName.SizeChanged, Callable.From((Action)OnWindowChange), 0u);
		OnWindowChange();
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		_pressDownCancelToken?.Cancel();
		_unhoverAnimCancelToken?.Cancel();
	}

	private void OnWindowChange()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		_showPos = ((Control)this).Position;
		_hidePos = ((Control)this).Position + new Vector2(0f, 64f);
	}

	protected override void OnEnable()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		((CanvasItem)_buttonImage).Modulate = Colors.White;
		Tween? moveTween = _moveTween;
		if (moveTween != null)
		{
			moveTween.Kill();
		}
		_moveTween = ((Node)this).CreateTween();
		_moveTween.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("position"), Variant.op_Implicit(_showPos), 0.3499999940395355).SetEase((EaseType)1).SetTrans((TransitionType)10)
			.From(Variant.op_Implicit(_hidePos));
	}

	protected override void OnDisable()
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		Tween? moveTween = _moveTween;
		if (moveTween != null)
		{
			moveTween.Kill();
		}
		_moveTween = ((Node)this).CreateTween();
		_moveTween.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("position"), Variant.op_Implicit(_hidePos), 0.3499999940395355).SetEase((EaseType)1).SetTrans((TransitionType)5)
			.From(Variant.op_Implicit(_showPos));
	}

	protected override void OnFocus()
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		base.OnFocus();
		_unhoverAnimCancelToken?.Cancel();
		((Control)this).Scale = _hoverScale;
		((CanvasItem)_buttonImage).Modulate = Colors.White;
	}

	protected override void OnUnfocus()
	{
		_pressDownCancelToken?.Cancel();
		_unhoverAnimCancelToken = new CancellationTokenSource();
		TaskHelper.RunSafely(AnimUnhover(_unhoverAnimCancelToken));
	}

	private async Task AnimUnhover(CancellationTokenSource cancelToken)
	{
		float timer = 0f;
		Vector2 startScale = ((Control)this).Scale;
		Color startButtonColor = ((CanvasItem)_buttonImage).Modulate;
		for (; timer < 0.5f; timer += (float)((Node)this).GetProcessDeltaTime())
		{
			if (cancelToken.IsCancellationRequested)
			{
				return;
			}
			((Control)this).Scale = ((Vector2)(ref startScale)).Lerp(Vector2.One, Ease.ExpoOut(timer / 0.5f));
			((CanvasItem)_buttonImage).Modulate = ((Color)(ref startButtonColor)).Lerp(Colors.White, Ease.ExpoOut(timer / 0.5f));
			await ((GodotObject)this).ToSignal((GodotObject)(object)((Node)this).GetTree(), SignalName.ProcessFrame);
		}
		((Control)this).Scale = Vector2.One;
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
			await ((GodotObject)this).ToSignal((GodotObject)(object)((Node)this).GetTree(), SignalName.ProcessFrame);
		}
		((Control)this).Scale = _downScale;
		((CanvasItem)_buttonImage).Modulate = _downColor;
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
		list.Add(new MethodInfo(MethodName._ExitTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnWindowChange, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
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
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName._buttonImage)
		{
			_buttonImage = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._downColor)
		{
			_downColor = VariantUtils.ConvertTo<Color>(ref value);
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
		if ((ref name) == PropertyName._buttonImage)
		{
			value = VariantUtils.CreateFrom<Control>(ref _buttonImage);
			return true;
		}
		if ((ref name) == PropertyName._downColor)
		{
			value = VariantUtils.CreateFrom<Color>(ref _downColor);
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
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._buttonImage, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)20, PropertyName._downColor, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName._buttonImage, Variant.From<Control>(ref _buttonImage));
		info.AddProperty(PropertyName._downColor, Variant.From<Color>(ref _downColor));
		info.AddProperty(PropertyName._showPos, Variant.From<Vector2>(ref _showPos));
		info.AddProperty(PropertyName._hidePos, Variant.From<Vector2>(ref _hidePos));
		info.AddProperty(PropertyName._moveTween, Variant.From<Tween>(ref _moveTween));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		base.RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._buttonImage, ref val))
		{
			_buttonImage = ((Variant)(ref val)).As<Control>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._downColor, ref val2))
		{
			_downColor = ((Variant)(ref val2)).As<Color>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._showPos, ref val3))
		{
			_showPos = ((Variant)(ref val3)).As<Vector2>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._hidePos, ref val4))
		{
			_hidePos = ((Variant)(ref val4)).As<Vector2>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._moveTween, ref val5))
		{
			_moveTween = ((Variant)(ref val5)).As<Tween>();
		}
	}
}
