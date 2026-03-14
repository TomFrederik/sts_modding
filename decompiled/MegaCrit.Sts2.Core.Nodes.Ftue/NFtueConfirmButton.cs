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

namespace MegaCrit.Sts2.Core.Nodes.Ftue;

[ScriptPath("res://src/Core/Nodes/Ftue/NFtueConfirmButton.cs")]
public class NFtueConfirmButton : NButton
{
	public new class MethodName : NButton.MethodName
	{
		public new static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public new static readonly StringName OnFocus = StringName.op_Implicit("OnFocus");

		public new static readonly StringName OnUnfocus = StringName.op_Implicit("OnUnfocus");

		public static readonly StringName UpdateShaderV = StringName.op_Implicit("UpdateShaderV");
	}

	public new class PropertyName : NButton.PropertyName
	{
		public new static readonly StringName Hotkeys = StringName.op_Implicit("Hotkeys");

		public static readonly StringName _outline = StringName.op_Implicit("_outline");

		public static readonly StringName _outlineTween = StringName.op_Implicit("_outlineTween");

		public static readonly StringName _scaleTween = StringName.op_Implicit("_scaleTween");

		public static readonly StringName _hsv = StringName.op_Implicit("_hsv");

		public static readonly StringName _label = StringName.op_Implicit("_label");
	}

	public new class SignalName : NButton.SignalName
	{
	}

	private static readonly StringName _v = new StringName("v");

	private TextureRect _outline;

	private Tween? _outlineTween;

	private Tween? _scaleTween;

	private ShaderMaterial _hsv;

	private MegaLabel _label;

	protected override string[] Hotkeys => new string[1] { StringName.op_Implicit(MegaInput.select) };

	public override void _Ready()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Expected O, but got Unknown
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		ConnectSignals();
		_outline = ((Node)this).GetNode<TextureRect>(NodePath.op_Implicit("Outline"));
		_hsv = (ShaderMaterial)((CanvasItem)this).Material;
		_label = ((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%Label"));
		_label.SetTextAutoSize(new LocString("ftues", "CONFIRM_BUTTON").GetRawText());
		_outlineTween = ((Node)this).CreateTween().SetLoops(0);
		_outlineTween.TweenProperty((GodotObject)(object)_outline, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(1f), 0.6);
		_outlineTween.TweenProperty((GodotObject)(object)_outline, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0.25f), 0.6);
	}

	protected override void OnFocus()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		base.OnFocus();
		Tween? outlineTween = _outlineTween;
		if (outlineTween != null)
		{
			outlineTween.Kill();
		}
		((CanvasItem)_outline).Modulate = StsColors.gold;
		Tween? scaleTween = _scaleTween;
		if (scaleTween != null)
		{
			scaleTween.Kill();
		}
		_scaleTween = ((Node)this).CreateTween();
		_scaleTween.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("scale"), Variant.op_Implicit(Vector2.One * 1.05f), 0.05).SetEase((EaseType)1).SetTrans((TransitionType)5);
		_scaleTween.TweenMethod(Callable.From<float>((Action<float>)UpdateShaderV), _hsv.GetShaderParameter(_v), Variant.op_Implicit(1.4f), 0.05).SetEase((EaseType)1).SetTrans((TransitionType)5);
	}

	protected override void OnUnfocus()
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		base.OnUnfocus();
		Tween? scaleTween = _scaleTween;
		if (scaleTween != null)
		{
			scaleTween.Kill();
		}
		_scaleTween = ((Node)this).CreateTween();
		_scaleTween.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("scale"), Variant.op_Implicit(Vector2.One), 0.5).SetEase((EaseType)1).SetTrans((TransitionType)5);
		_scaleTween.TweenMethod(Callable.From<float>((Action<float>)UpdateShaderV), _hsv.GetShaderParameter(_v), Variant.op_Implicit(1f), 0.5).SetEase((EaseType)1).SetTrans((TransitionType)5);
		Tween? outlineTween = _outlineTween;
		if (outlineTween != null)
		{
			outlineTween.Kill();
		}
		_outlineTween = ((Node)this).CreateTween().SetLoops(0);
		_outlineTween.TweenProperty((GodotObject)(object)_outline, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0.25f), 0.6);
		_outlineTween.TweenProperty((GodotObject)(object)_outline, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(1f), 0.6).SetEase((EaseType)1).SetTrans((TransitionType)7);
	}

	private void UpdateShaderV(float value)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		_hsv.SetShaderParameter(_v, Variant.op_Implicit(value));
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
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(4);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnFocus, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnUnfocus, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
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
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
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
		if ((ref method) == MethodName.OnFocus)
		{
			return true;
		}
		if ((ref method) == MethodName.OnUnfocus)
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
		if ((ref name) == PropertyName._outline)
		{
			_outline = VariantUtils.ConvertTo<TextureRect>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._outlineTween)
		{
			_outlineTween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._scaleTween)
		{
			_scaleTween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._hsv)
		{
			_hsv = VariantUtils.ConvertTo<ShaderMaterial>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._label)
		{
			_label = VariantUtils.ConvertTo<MegaLabel>(ref value);
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
		if ((ref name) == PropertyName.Hotkeys)
		{
			string[] hotkeys = Hotkeys;
			value = VariantUtils.CreateFrom<string[]>(ref hotkeys);
			return true;
		}
		if ((ref name) == PropertyName._outline)
		{
			value = VariantUtils.CreateFrom<TextureRect>(ref _outline);
			return true;
		}
		if ((ref name) == PropertyName._outlineTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _outlineTween);
			return true;
		}
		if ((ref name) == PropertyName._scaleTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _scaleTween);
			return true;
		}
		if ((ref name) == PropertyName._hsv)
		{
			value = VariantUtils.CreateFrom<ShaderMaterial>(ref _hsv);
			return true;
		}
		if ((ref name) == PropertyName._label)
		{
			value = VariantUtils.CreateFrom<MegaLabel>(ref _label);
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
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)34, PropertyName.Hotkeys, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._outline, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._outlineTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._scaleTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._hsv, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._label, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		info.AddProperty(PropertyName._outline, Variant.From<TextureRect>(ref _outline));
		info.AddProperty(PropertyName._outlineTween, Variant.From<Tween>(ref _outlineTween));
		info.AddProperty(PropertyName._scaleTween, Variant.From<Tween>(ref _scaleTween));
		info.AddProperty(PropertyName._hsv, Variant.From<ShaderMaterial>(ref _hsv));
		info.AddProperty(PropertyName._label, Variant.From<MegaLabel>(ref _label));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._outline, ref val))
		{
			_outline = ((Variant)(ref val)).As<TextureRect>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._outlineTween, ref val2))
		{
			_outlineTween = ((Variant)(ref val2)).As<Tween>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._scaleTween, ref val3))
		{
			_scaleTween = ((Variant)(ref val3)).As<Tween>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._hsv, ref val4))
		{
			_hsv = ((Variant)(ref val4)).As<ShaderMaterial>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._label, ref val5))
		{
			_label = ((Variant)(ref val5)).As<MegaLabel>();
		}
	}
}
