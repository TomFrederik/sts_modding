using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.Nodes.Screens.Timeline;

[ScriptPath("res://src/Core/Nodes/Screens/Timeline/NUnlockInfo.cs")]
public class NUnlockInfo : Control
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName HideImmediately = StringName.op_Implicit("HideImmediately");

		public static readonly StringName AnimIn = StringName.op_Implicit("AnimIn");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName _icon = StringName.op_Implicit("_icon");

		public static readonly StringName _label = StringName.op_Implicit("_label");

		public static readonly StringName _tween = StringName.op_Implicit("_tween");
	}

	public class SignalName : SignalName
	{
	}

	private TextureRect _icon;

	private MegaRichTextLabel _label;

	private Tween? _tween;

	public override void _Ready()
	{
		_icon = ((Node)this).GetNode<TextureRect>(NodePath.op_Implicit("%Icon"));
		_label = ((Node)this).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("%Text"));
	}

	public void HideImmediately()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		Color modulate = ((CanvasItem)this).Modulate;
		modulate.A = 0f;
		((CanvasItem)this).Modulate = modulate;
	}

	public void AnimIn(string text)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		Color modulate = ((CanvasItem)this).Modulate;
		modulate.A = 0f;
		((CanvasItem)this).Modulate = modulate;
		_label.Text = text;
		Tween? tween = _tween;
		if (tween != null)
		{
			tween.Kill();
		}
		_tween = ((Node)this).CreateTween();
		_tween.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0.8f), 1.0);
	}

	public async Task AnimInViaPaginator(string text)
	{
		Tween? tween = _tween;
		if (tween != null)
		{
			tween.Kill();
		}
		_tween = ((Node)this).CreateTween();
		_tween.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0f), 0.1);
		await ((GodotObject)this).ToSignal((GodotObject)(object)_tween, SignalName.Finished);
		_label.Text = text;
		Tween? tween2 = _tween;
		if (tween2 != null)
		{
			tween2.Kill();
		}
		_tween = ((Node)this).CreateTween();
		_tween.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0.8f), 1.0);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(3);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.HideImmediately, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AnimIn, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)4, StringName.op_Implicit("text"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.HideImmediately && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			HideImmediately();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.AnimIn && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			AnimIn(VariantUtils.ConvertTo<string>(ref ((NativeVariantPtrArgs)(ref args))[0]));
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
		if ((ref method) == MethodName.HideImmediately)
		{
			return true;
		}
		if ((ref method) == MethodName.AnimIn)
		{
			return true;
		}
		return ((Control)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._icon)
		{
			_icon = VariantUtils.ConvertTo<TextureRect>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._label)
		{
			_label = VariantUtils.ConvertTo<MegaRichTextLabel>(ref value);
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
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName._icon)
		{
			value = VariantUtils.CreateFrom<TextureRect>(ref _icon);
			return true;
		}
		if ((ref name) == PropertyName._label)
		{
			value = VariantUtils.CreateFrom<MegaRichTextLabel>(ref _label);
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
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._icon, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._label, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._tween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		info.AddProperty(PropertyName._icon, Variant.From<TextureRect>(ref _icon));
		info.AddProperty(PropertyName._label, Variant.From<MegaRichTextLabel>(ref _label));
		info.AddProperty(PropertyName._tween, Variant.From<Tween>(ref _tween));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._icon, ref val))
		{
			_icon = ((Variant)(ref val)).As<TextureRect>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._label, ref val2))
		{
			_label = ((Variant)(ref val2)).As<MegaRichTextLabel>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._tween, ref val3))
		{
			_tween = ((Variant)(ref val3)).As<Tween>();
		}
	}
}
