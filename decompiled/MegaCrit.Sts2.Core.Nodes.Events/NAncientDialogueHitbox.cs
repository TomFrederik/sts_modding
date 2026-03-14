using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.ControllerInput;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.Nodes.Events;

[ScriptPath("res://src/Core/Nodes/Events/NAncientDialogueHitbox.cs")]
public class NAncientDialogueHitbox : NButton
{
	public new class MethodName : NButton.MethodName
	{
		public static readonly StringName GetHotkey = StringName.op_Implicit("GetHotkey");

		public new static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public new static readonly StringName OnRelease = StringName.op_Implicit("OnRelease");

		public new static readonly StringName OnPress = StringName.op_Implicit("OnPress");

		public new static readonly StringName OnFocus = StringName.op_Implicit("OnFocus");
	}

	public new class PropertyName : NButton.PropertyName
	{
		public new static readonly StringName Hotkeys = StringName.op_Implicit("Hotkeys");

		public static readonly StringName _label = StringName.op_Implicit("_label");

		public static readonly StringName _arrow = StringName.op_Implicit("_arrow");

		public static readonly StringName _loopTween = StringName.op_Implicit("_loopTween");

		public static readonly StringName _tween = StringName.op_Implicit("_tween");

		public static readonly StringName _isAnimating = StringName.op_Implicit("_isAnimating");
	}

	public new class SignalName : NButton.SignalName
	{
	}

	private MegaLabel _label;

	private TextureRect _arrow;

	private Tween? _loopTween;

	private Tween? _tween;

	private bool _isAnimating;

	protected override string[] Hotkeys => new string[1] { StringName.op_Implicit(MegaInput.accept) };

	public string? GetHotkey()
	{
		return Hotkeys.FirstOrDefault();
	}

	public override void _Ready()
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		ConnectSignals();
		_label = ((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%Label"));
		((CanvasItem)_label).SelfModulate = StsColors.transparentWhite;
		((Label)_label).Text = string.Empty;
		_arrow = ((Node)this).GetNode<TextureRect>(NodePath.op_Implicit("%Arrow"));
		((CanvasItem)_arrow).SelfModulate = StsColors.transparentWhite;
		_loopTween = ((Node)this).CreateTween().SetParallel(true).SetLoops(0);
		_loopTween.TweenProperty((GodotObject)(object)_arrow, NodePath.op_Implicit("position:x"), Variant.op_Implicit(((Control)_arrow).Position.X + 4f), 0.4).SetEase((EaseType)2).SetTrans((TransitionType)1);
		_loopTween.Chain().TweenProperty((GodotObject)(object)_arrow, NodePath.op_Implicit("position:x"), Variant.op_Implicit(((Control)_arrow).Position.X - 4f), 0.6).SetEase((EaseType)2)
			.SetTrans((TransitionType)1);
		Tween val = ((Node)this).CreateTween().SetParallel(true);
		val.TweenProperty((GodotObject)(object)_label, NodePath.op_Implicit("self_modulate:a"), Variant.op_Implicit(1f), 1.0).SetDelay(0.5);
		val.TweenProperty((GodotObject)(object)_arrow, NodePath.op_Implicit("self_modulate:a"), Variant.op_Implicit(1f), 1.0).SetDelay(0.5);
	}

	protected override void OnRelease()
	{
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		base.OnRelease();
		Tween? loopTween = _loopTween;
		if (loopTween != null)
		{
			loopTween.Play();
		}
		Tween? tween = _tween;
		if (tween != null)
		{
			tween.Kill();
		}
		_tween = ((Node)this).CreateTween().SetParallel(true);
		_tween.TweenProperty((GodotObject)(object)_label, NodePath.op_Implicit("scale"), Variant.op_Implicit(Vector2.One), 0.5).SetEase((EaseType)1).SetTrans((TransitionType)5);
		_tween.TweenProperty((GodotObject)(object)_arrow, NodePath.op_Implicit("scale"), Variant.op_Implicit(Vector2.One), 0.5).SetEase((EaseType)1).SetTrans((TransitionType)5);
	}

	protected override void OnPress()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		base.OnPress();
		Tween? loopTween = _loopTween;
		if (loopTween != null)
		{
			loopTween.Pause();
		}
		((Control)_label).PivotOffset = ((Control)_label).Size * 0.5f;
		Tween? tween = _tween;
		if (tween != null)
		{
			tween.Kill();
		}
		_tween = ((Node)this).CreateTween().SetParallel(true);
		_tween.TweenProperty((GodotObject)(object)_label, NodePath.op_Implicit("scale"), Variant.op_Implicit(Vector2.One * 0.9f), 0.5).SetEase((EaseType)1).SetTrans((TransitionType)5);
		_tween.TweenProperty((GodotObject)(object)_arrow, NodePath.op_Implicit("scale"), Variant.op_Implicit(Vector2.One * 0.9f), 0.5).SetEase((EaseType)1).SetTrans((TransitionType)5);
		_isAnimating = false;
	}

	protected override void OnFocus()
	{
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
		List<MethodInfo> list = new List<MethodInfo>(5);
		list.Add(new MethodInfo(MethodName.GetHotkey, new PropertyInfo((Type)4, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnRelease, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnPress, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnFocus, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.GetHotkey && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			string hotkey = GetHotkey();
			ret = VariantUtils.CreateFrom<string>(ref hotkey);
			return true;
		}
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnRelease && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnRelease();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnPress && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnPress();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnFocus && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnFocus();
			ret = default(godot_variant);
			return true;
		}
		return base.InvokeGodotClassMethod(in method, args, out ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if ((ref method) == MethodName.GetHotkey)
		{
			return true;
		}
		if ((ref method) == MethodName._Ready)
		{
			return true;
		}
		if ((ref method) == MethodName.OnRelease)
		{
			return true;
		}
		if ((ref method) == MethodName.OnPress)
		{
			return true;
		}
		if ((ref method) == MethodName.OnFocus)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._label)
		{
			_label = VariantUtils.ConvertTo<MegaLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._arrow)
		{
			_arrow = VariantUtils.ConvertTo<TextureRect>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._loopTween)
		{
			_loopTween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._tween)
		{
			_tween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._isAnimating)
		{
			_isAnimating = VariantUtils.ConvertTo<bool>(ref value);
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
		if ((ref name) == PropertyName._label)
		{
			value = VariantUtils.CreateFrom<MegaLabel>(ref _label);
			return true;
		}
		if ((ref name) == PropertyName._arrow)
		{
			value = VariantUtils.CreateFrom<TextureRect>(ref _arrow);
			return true;
		}
		if ((ref name) == PropertyName._loopTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _loopTween);
			return true;
		}
		if ((ref name) == PropertyName._tween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _tween);
			return true;
		}
		if ((ref name) == PropertyName._isAnimating)
		{
			value = VariantUtils.CreateFrom<bool>(ref _isAnimating);
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
		list.Add(new PropertyInfo((Type)34, PropertyName.Hotkeys, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._label, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._arrow, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._loopTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._tween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._isAnimating, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		info.AddProperty(PropertyName._label, Variant.From<MegaLabel>(ref _label));
		info.AddProperty(PropertyName._arrow, Variant.From<TextureRect>(ref _arrow));
		info.AddProperty(PropertyName._loopTween, Variant.From<Tween>(ref _loopTween));
		info.AddProperty(PropertyName._tween, Variant.From<Tween>(ref _tween));
		info.AddProperty(PropertyName._isAnimating, Variant.From<bool>(ref _isAnimating));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._label, ref val))
		{
			_label = ((Variant)(ref val)).As<MegaLabel>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._arrow, ref val2))
		{
			_arrow = ((Variant)(ref val2)).As<TextureRect>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._loopTween, ref val3))
		{
			_loopTween = ((Variant)(ref val3)).As<Tween>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._tween, ref val4))
		{
			_tween = ((Variant)(ref val4)).As<Tween>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._isAnimating, ref val5))
		{
			_isAnimating = ((Variant)(ref val5)).As<bool>();
		}
	}
}
