using System;
using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;

namespace MegaCrit.Sts2.Core.Nodes.Vfx;

[GlobalClass]
[ScriptPath("res://src/Core/Nodes/Vfx/NNecrobinderFlameVfx.cs")]
public class NNecrobinderFlameVfx : Node
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName UpdateFlameVisibility = StringName.op_Implicit("UpdateFlameVisibility");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName _parent = StringName.op_Implicit("_parent");
	}

	public class SignalName : SignalName
	{
	}

	private Node2D _parent;

	private MegaSprite _animController;

	public override void _Ready()
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		_parent = ((Node)this).GetParent<Node2D>();
		_animController = new MegaSprite(Variant.op_Implicit((GodotObject)(object)((Node)_parent).GetParent()));
		_animController.ConnectAnimationStarted(Callable.From<GodotObject, GodotObject, GodotObject>((Action<GodotObject, GodotObject, GodotObject>)UpdateFlameVisibility));
	}

	private void UpdateFlameVisibility(GodotObject spineSprite, GodotObject animationState, GodotObject trackEntry)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		((CanvasItem)_parent).Visible = new MegaAnimationState(Variant.op_Implicit(animationState)).GetCurrent(0).GetAnimation().GetName() != "die";
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Expected O, but got Unknown
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Expected O, but got Unknown
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Expected O, but got Unknown
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(2);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UpdateFlameVisibility, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("spineSprite"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Object"), false),
			new PropertyInfo((Type)24, StringName.op_Implicit("animationState"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Object"), false),
			new PropertyInfo((Type)24, StringName.op_Implicit("trackEntry"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Object"), false)
		}, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.UpdateFlameVisibility && ((NativeVariantPtrArgs)(ref args)).Count == 3)
		{
			UpdateFlameVisibility(VariantUtils.ConvertTo<GodotObject>(ref ((NativeVariantPtrArgs)(ref args))[0]), VariantUtils.ConvertTo<GodotObject>(ref ((NativeVariantPtrArgs)(ref args))[1]), VariantUtils.ConvertTo<GodotObject>(ref ((NativeVariantPtrArgs)(ref args))[2]));
			ret = default(godot_variant);
			return true;
		}
		return ((Node)this).InvokeGodotClassMethod(ref method, args, ref ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if ((ref method) == MethodName._Ready)
		{
			return true;
		}
		if ((ref method) == MethodName.UpdateFlameVisibility)
		{
			return true;
		}
		return ((Node)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._parent)
		{
			_parent = VariantUtils.ConvertTo<Node2D>(ref value);
			return true;
		}
		return ((GodotObject)this).SetGodotClassPropertyValue(ref name, ref value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName._parent)
		{
			value = VariantUtils.CreateFrom<Node2D>(ref _parent);
			return true;
		}
		return ((GodotObject)this).GetGodotClassPropertyValue(ref name, ref value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._parent, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		info.AddProperty(PropertyName._parent, Variant.From<Node2D>(ref _parent));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._parent, ref val))
		{
			_parent = ((Variant)(ref val)).As<Node2D>();
		}
	}
}
