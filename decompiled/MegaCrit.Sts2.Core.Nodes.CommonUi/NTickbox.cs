using System;
using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;

namespace MegaCrit.Sts2.Core.Nodes.CommonUi;

[ScriptPath("res://src/Core/Nodes/CommonUi/NTickbox.cs")]
public class NTickbox : NButton
{
	[Signal]
	public delegate void ToggledEventHandler(NTickbox tickbox);

	public new class MethodName : NButton.MethodName
	{
		public new static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public new static readonly StringName ConnectSignals = StringName.op_Implicit("ConnectSignals");

		public new static readonly StringName OnRelease = StringName.op_Implicit("OnRelease");

		public static readonly StringName OnUntick = StringName.op_Implicit("OnUntick");

		public static readonly StringName OnTick = StringName.op_Implicit("OnTick");

		public new static readonly StringName OnFocus = StringName.op_Implicit("OnFocus");

		public new static readonly StringName OnUnfocus = StringName.op_Implicit("OnUnfocus");

		public new static readonly StringName OnPress = StringName.op_Implicit("OnPress");

		public new static readonly StringName OnDisable = StringName.op_Implicit("OnDisable");

		public new static readonly StringName OnEnable = StringName.op_Implicit("OnEnable");

		public static readonly StringName UpdateShaderV = StringName.op_Implicit("UpdateShaderV");
	}

	public new class PropertyName : NButton.PropertyName
	{
		public static readonly StringName IsTicked = StringName.op_Implicit("IsTicked");

		public static readonly StringName _isTicked = StringName.op_Implicit("_isTicked");

		public static readonly StringName _imageContainer = StringName.op_Implicit("_imageContainer");

		public static readonly StringName _tickedImage = StringName.op_Implicit("_tickedImage");

		public static readonly StringName _notTickedImage = StringName.op_Implicit("_notTickedImage");

		public static readonly StringName _hsv = StringName.op_Implicit("_hsv");

		public static readonly StringName _tween = StringName.op_Implicit("_tween");

		public static readonly StringName _baseScale = StringName.op_Implicit("_baseScale");

		public static readonly StringName _hoverScale = StringName.op_Implicit("_hoverScale");

		public static readonly StringName _pressDownScale = StringName.op_Implicit("_pressDownScale");

		public static readonly StringName _hoverV = StringName.op_Implicit("_hoverV");
	}

	public new class SignalName : NButton.SignalName
	{
		public static readonly StringName Toggled = StringName.op_Implicit("Toggled");
	}

	private static readonly StringName _v = new StringName("v");

	private bool _isTicked = true;

	private Control _imageContainer;

	private Control _tickedImage;

	private Control _notTickedImage;

	private ShaderMaterial _hsv;

	private Tween? _tween;

	private Vector2 _baseScale;

	private float _hoverScale = 1.05f;

	private float _pressDownScale = 0.95f;

	private float _hoverV = 1.2f;

	private ToggledEventHandler backing_Toggled;

	public bool IsTicked
	{
		get
		{
			return _isTicked;
		}
		set
		{
			_isTicked = value;
			((CanvasItem)_tickedImage).Visible = _isTicked;
			((CanvasItem)_notTickedImage).Visible = !_isTicked;
		}
	}

	public event ToggledEventHandler Toggled
	{
		add
		{
			backing_Toggled = (ToggledEventHandler)Delegate.Combine(backing_Toggled, value);
		}
		remove
		{
			backing_Toggled = (ToggledEventHandler)Delegate.Remove(backing_Toggled, value);
		}
	}

	public override void _Ready()
	{
		if (((object)this).GetType() != typeof(NTickbox))
		{
			Log.Error($"{((object)this).GetType()}");
			throw new InvalidOperationException("Don't call base._Ready()! Call ConnectSignals() instead.");
		}
		ConnectSignals();
	}

	protected override void ConnectSignals()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Expected O, but got Unknown
		base.ConnectSignals();
		_imageContainer = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%TickboxVisuals"));
		_baseScale = _imageContainer.Scale;
		_hsv = (ShaderMaterial)((CanvasItem)_imageContainer).Material;
		_tickedImage = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%TickboxVisuals/Ticked"));
		_notTickedImage = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%TickboxVisuals/NotTicked"));
	}

	protected override void OnRelease()
	{
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		base.OnRelease();
		IsTicked = !IsTicked;
		if (IsTicked)
		{
			SfxCmd.Play("event:/sfx/ui/clicks/ui_checkbox_on");
			OnTick();
		}
		else
		{
			SfxCmd.Play("event:/sfx/ui/clicks/ui_checkbox_off");
			OnUntick();
		}
		((GodotObject)this).EmitSignal(SignalName.Toggled, (Variant[])(object)new Variant[1] { Variant.op_Implicit((GodotObject)(object)this) });
		Tween? tween = _tween;
		if (tween != null)
		{
			tween.Kill();
		}
		_tween = ((Node)this).CreateTween().SetParallel(true);
		_tween.TweenProperty((GodotObject)(object)_imageContainer, NodePath.op_Implicit("scale"), Variant.op_Implicit(_baseScale * _hoverScale), 0.05);
		_tween.TweenMethod(Callable.From<float>((Action<float>)UpdateShaderV), _hsv.GetShaderParameter(_v), Variant.op_Implicit(_hoverV), 0.05);
	}

	protected virtual void OnUntick()
	{
	}

	protected virtual void OnTick()
	{
	}

	protected override void OnFocus()
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		Tween? tween = _tween;
		if (tween != null)
		{
			tween.Kill();
		}
		_tween = ((Node)this).CreateTween().SetParallel(true);
		_tween.TweenProperty((GodotObject)(object)_imageContainer, NodePath.op_Implicit("scale"), Variant.op_Implicit(_baseScale * _hoverScale), 0.05);
		_tween.TweenMethod(Callable.From<float>((Action<float>)UpdateShaderV), _hsv.GetShaderParameter(_v), Variant.op_Implicit(_hoverV), 0.05);
	}

	protected override void OnUnfocus()
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		Tween? tween = _tween;
		if (tween != null)
		{
			tween.Kill();
		}
		_tween = ((Node)this).CreateTween().SetParallel(true);
		_tween.TweenProperty((GodotObject)(object)_imageContainer, NodePath.op_Implicit("scale"), Variant.op_Implicit(_baseScale), 0.5).SetEase((EaseType)1).SetTrans((TransitionType)5);
		_tween.TweenMethod(Callable.From<float>((Action<float>)UpdateShaderV), _hsv.GetShaderParameter(_v), Variant.op_Implicit(1f), 0.5).SetEase((EaseType)1).SetTrans((TransitionType)5);
	}

	protected override void OnPress()
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		Tween? tween = _tween;
		if (tween != null)
		{
			tween.Kill();
		}
		_tween = ((Node)this).CreateTween().SetParallel(true);
		_tween.TweenProperty((GodotObject)(object)_imageContainer, NodePath.op_Implicit("scale"), Variant.op_Implicit(_baseScale * _pressDownScale), 0.5).SetEase((EaseType)1).SetTrans((TransitionType)5);
		_tween.TweenMethod(Callable.From<float>((Action<float>)UpdateShaderV), _hsv.GetShaderParameter(_v), Variant.op_Implicit(0.8f), 0.5).SetEase((EaseType)1).SetTrans((TransitionType)5);
	}

	protected override void OnDisable()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		base.OnDisable();
		((CanvasItem)this).Modulate = StsColors.gray;
	}

	protected override void OnEnable()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		base.OnEnable();
		((CanvasItem)this).Modulate = Colors.White;
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
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(11);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ConnectSignals, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnRelease, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnUntick, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnTick, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnFocus, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnUnfocus, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnPress, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnDisable, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnEnable, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
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
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.ConnectSignals && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			ConnectSignals();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnRelease && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnRelease();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnUntick && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnUntick();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnTick && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnTick();
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
		if ((ref method) == MethodName.OnDisable && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnDisable();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnEnable && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnEnable();
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
		if ((ref method) == MethodName.ConnectSignals)
		{
			return true;
		}
		if ((ref method) == MethodName.OnRelease)
		{
			return true;
		}
		if ((ref method) == MethodName.OnUntick)
		{
			return true;
		}
		if ((ref method) == MethodName.OnTick)
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
		if ((ref method) == MethodName.OnDisable)
		{
			return true;
		}
		if ((ref method) == MethodName.OnEnable)
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
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName.IsTicked)
		{
			IsTicked = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._isTicked)
		{
			_isTicked = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._imageContainer)
		{
			_imageContainer = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._tickedImage)
		{
			_tickedImage = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._notTickedImage)
		{
			_notTickedImage = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._hsv)
		{
			_hsv = VariantUtils.ConvertTo<ShaderMaterial>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._tween)
		{
			_tween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._baseScale)
		{
			_baseScale = VariantUtils.ConvertTo<Vector2>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._hoverScale)
		{
			_hoverScale = VariantUtils.ConvertTo<float>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._pressDownScale)
		{
			_pressDownScale = VariantUtils.ConvertTo<float>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._hoverV)
		{
			_hoverV = VariantUtils.ConvertTo<float>(ref value);
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
		if ((ref name) == PropertyName.IsTicked)
		{
			bool isTicked = IsTicked;
			value = VariantUtils.CreateFrom<bool>(ref isTicked);
			return true;
		}
		if ((ref name) == PropertyName._isTicked)
		{
			value = VariantUtils.CreateFrom<bool>(ref _isTicked);
			return true;
		}
		if ((ref name) == PropertyName._imageContainer)
		{
			value = VariantUtils.CreateFrom<Control>(ref _imageContainer);
			return true;
		}
		if ((ref name) == PropertyName._tickedImage)
		{
			value = VariantUtils.CreateFrom<Control>(ref _tickedImage);
			return true;
		}
		if ((ref name) == PropertyName._notTickedImage)
		{
			value = VariantUtils.CreateFrom<Control>(ref _notTickedImage);
			return true;
		}
		if ((ref name) == PropertyName._hsv)
		{
			value = VariantUtils.CreateFrom<ShaderMaterial>(ref _hsv);
			return true;
		}
		if ((ref name) == PropertyName._tween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _tween);
			return true;
		}
		if ((ref name) == PropertyName._baseScale)
		{
			value = VariantUtils.CreateFrom<Vector2>(ref _baseScale);
			return true;
		}
		if ((ref name) == PropertyName._hoverScale)
		{
			value = VariantUtils.CreateFrom<float>(ref _hoverScale);
			return true;
		}
		if ((ref name) == PropertyName._pressDownScale)
		{
			value = VariantUtils.CreateFrom<float>(ref _pressDownScale);
			return true;
		}
		if ((ref name) == PropertyName._hoverV)
		{
			value = VariantUtils.CreateFrom<float>(ref _hoverV);
			return true;
		}
		return base.GetGodotClassPropertyValue(in name, out value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)1, PropertyName._isTicked, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName.IsTicked, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._imageContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._tickedImage, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._notTickedImage, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._hsv, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._tween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)5, PropertyName._baseScale, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)3, PropertyName._hoverScale, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)3, PropertyName._pressDownScale, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)3, PropertyName._hoverV, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		base.SaveGodotObjectData(info);
		StringName isTicked = PropertyName.IsTicked;
		bool isTicked2 = IsTicked;
		info.AddProperty(isTicked, Variant.From<bool>(ref isTicked2));
		info.AddProperty(PropertyName._isTicked, Variant.From<bool>(ref _isTicked));
		info.AddProperty(PropertyName._imageContainer, Variant.From<Control>(ref _imageContainer));
		info.AddProperty(PropertyName._tickedImage, Variant.From<Control>(ref _tickedImage));
		info.AddProperty(PropertyName._notTickedImage, Variant.From<Control>(ref _notTickedImage));
		info.AddProperty(PropertyName._hsv, Variant.From<ShaderMaterial>(ref _hsv));
		info.AddProperty(PropertyName._tween, Variant.From<Tween>(ref _tween));
		info.AddProperty(PropertyName._baseScale, Variant.From<Vector2>(ref _baseScale));
		info.AddProperty(PropertyName._hoverScale, Variant.From<float>(ref _hoverScale));
		info.AddProperty(PropertyName._pressDownScale, Variant.From<float>(ref _pressDownScale));
		info.AddProperty(PropertyName._hoverV, Variant.From<float>(ref _hoverV));
		info.AddSignalEventDelegate(SignalName.Toggled, (Delegate)backing_Toggled);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		base.RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName.IsTicked, ref val))
		{
			IsTicked = ((Variant)(ref val)).As<bool>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._isTicked, ref val2))
		{
			_isTicked = ((Variant)(ref val2)).As<bool>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._imageContainer, ref val3))
		{
			_imageContainer = ((Variant)(ref val3)).As<Control>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._tickedImage, ref val4))
		{
			_tickedImage = ((Variant)(ref val4)).As<Control>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._notTickedImage, ref val5))
		{
			_notTickedImage = ((Variant)(ref val5)).As<Control>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._hsv, ref val6))
		{
			_hsv = ((Variant)(ref val6)).As<ShaderMaterial>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName._tween, ref val7))
		{
			_tween = ((Variant)(ref val7)).As<Tween>();
		}
		Variant val8 = default(Variant);
		if (info.TryGetProperty(PropertyName._baseScale, ref val8))
		{
			_baseScale = ((Variant)(ref val8)).As<Vector2>();
		}
		Variant val9 = default(Variant);
		if (info.TryGetProperty(PropertyName._hoverScale, ref val9))
		{
			_hoverScale = ((Variant)(ref val9)).As<float>();
		}
		Variant val10 = default(Variant);
		if (info.TryGetProperty(PropertyName._pressDownScale, ref val10))
		{
			_pressDownScale = ((Variant)(ref val10)).As<float>();
		}
		Variant val11 = default(Variant);
		if (info.TryGetProperty(PropertyName._hoverV, ref val11))
		{
			_hoverV = ((Variant)(ref val11)).As<float>();
		}
		ToggledEventHandler toggledEventHandler = default(ToggledEventHandler);
		if (info.TryGetSignalEventDelegate<ToggledEventHandler>(SignalName.Toggled, ref toggledEventHandler))
		{
			backing_Toggled = toggledEventHandler;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<MethodInfo> GetGodotSignalList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Expected O, but got Unknown
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(1);
		list.Add(new MethodInfo(SignalName.Toggled, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("tickbox"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		return list;
	}

	protected void EmitSignalToggled(NTickbox tickbox)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).EmitSignal(SignalName.Toggled, (Variant[])(object)new Variant[1] { Variant.op_Implicit((GodotObject)(object)tickbox) });
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RaiseGodotClassSignalCallbacks(in godot_string_name signal, NativeVariantPtrArgs args)
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		if ((ref signal) == SignalName.Toggled && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			backing_Toggled?.Invoke(VariantUtils.ConvertTo<NTickbox>(ref ((NativeVariantPtrArgs)(ref args))[0]));
		}
		else
		{
			base.RaiseGodotClassSignalCallbacks(in signal, args);
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassSignal(in godot_string_name signal)
	{
		if ((ref signal) == SignalName.Toggled)
		{
			return true;
		}
		return base.HasGodotClassSignal(in signal);
	}
}
