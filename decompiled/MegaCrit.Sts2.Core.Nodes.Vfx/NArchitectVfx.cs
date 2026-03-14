using System;
using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;

namespace MegaCrit.Sts2.Core.Nodes.Vfx;

[GlobalClass]
[ScriptPath("res://src/Core/Nodes/Vfx/NArchitectVfx.cs")]
public class NArchitectVfx : Node
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName OnAnimationEvent = StringName.op_Implicit("OnAnimationEvent");

		public static readonly StringName StartTrail = StringName.op_Implicit("StartTrail");

		public static readonly StringName EndTrail = StringName.op_Implicit("EndTrail");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName _parent = StringName.op_Implicit("_parent");

		public static readonly StringName _innerTrail = StringName.op_Implicit("_innerTrail");

		public static readonly StringName _outerTrail = StringName.op_Implicit("_outerTrail");
	}

	public class SignalName : SignalName
	{
	}

	private Node2D _parent;

	private MegaSprite _animController;

	private NBasicTrail _innerTrail;

	private NBasicTrail _outerTrail;

	public override void _Ready()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		_parent = ((Node)this).GetParent<Node2D>();
		_animController = new MegaSprite(Variant.op_Implicit((GodotObject)(object)_parent));
		_animController.ConnectAnimationEvent(Callable.From<GodotObject, GodotObject, GodotObject, GodotObject>((Action<GodotObject, GodotObject, GodotObject, GodotObject>)OnAnimationEvent));
		_innerTrail = ((Node)_parent).GetNode<NBasicTrail>(NodePath.op_Implicit("TrailSlot/TrailInner"));
		_outerTrail = ((Node)_parent).GetNode<NBasicTrail>(NodePath.op_Implicit("TrailSlot/TrailOuter"));
		_animController.GetAnimationState().SetAnimation("idle_loop");
		_animController.GetAnimationState().SetAnimation("_tracks/head_normal", loop: true, 1);
	}

	private void OnAnimationEvent(GodotObject _, GodotObject __, GodotObject ___, GodotObject spineEvent)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		string eventName = new MegaEvent(Variant.op_Implicit(spineEvent)).GetData().GetEventName();
		if (!(eventName == "trail_start"))
		{
			if (eventName == "trail_end")
			{
				EndTrail();
			}
		}
		else
		{
			StartTrail();
		}
	}

	private void StartTrail()
	{
		((CanvasItem)_innerTrail).Visible = true;
		((CanvasItem)_outerTrail).Visible = true;
		((Line2D)_innerTrail).ClearPoints();
		((Line2D)_outerTrail).ClearPoints();
	}

	private void EndTrail()
	{
		((CanvasItem)_innerTrail).Visible = false;
		((CanvasItem)_outerTrail).Visible = false;
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
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Expected O, but got Unknown
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(4);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnAnimationEvent, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Object"), false),
			new PropertyInfo((Type)24, StringName.op_Implicit("__"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Object"), false),
			new PropertyInfo((Type)24, StringName.op_Implicit("___"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Object"), false),
			new PropertyInfo((Type)24, StringName.op_Implicit("spineEvent"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Object"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.StartTrail, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.EndTrail, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnAnimationEvent && ((NativeVariantPtrArgs)(ref args)).Count == 4)
		{
			OnAnimationEvent(VariantUtils.ConvertTo<GodotObject>(ref ((NativeVariantPtrArgs)(ref args))[0]), VariantUtils.ConvertTo<GodotObject>(ref ((NativeVariantPtrArgs)(ref args))[1]), VariantUtils.ConvertTo<GodotObject>(ref ((NativeVariantPtrArgs)(ref args))[2]), VariantUtils.ConvertTo<GodotObject>(ref ((NativeVariantPtrArgs)(ref args))[3]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.StartTrail && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			StartTrail();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.EndTrail && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			EndTrail();
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
		if ((ref method) == MethodName.OnAnimationEvent)
		{
			return true;
		}
		if ((ref method) == MethodName.StartTrail)
		{
			return true;
		}
		if ((ref method) == MethodName.EndTrail)
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
		if ((ref name) == PropertyName._innerTrail)
		{
			_innerTrail = VariantUtils.ConvertTo<NBasicTrail>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._outerTrail)
		{
			_outerTrail = VariantUtils.ConvertTo<NBasicTrail>(ref value);
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
		if ((ref name) == PropertyName._parent)
		{
			value = VariantUtils.CreateFrom<Node2D>(ref _parent);
			return true;
		}
		if ((ref name) == PropertyName._innerTrail)
		{
			value = VariantUtils.CreateFrom<NBasicTrail>(ref _innerTrail);
			return true;
		}
		if ((ref name) == PropertyName._outerTrail)
		{
			value = VariantUtils.CreateFrom<NBasicTrail>(ref _outerTrail);
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
		list.Add(new PropertyInfo((Type)24, PropertyName._parent, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._innerTrail, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._outerTrail, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		info.AddProperty(PropertyName._parent, Variant.From<Node2D>(ref _parent));
		info.AddProperty(PropertyName._innerTrail, Variant.From<NBasicTrail>(ref _innerTrail));
		info.AddProperty(PropertyName._outerTrail, Variant.From<NBasicTrail>(ref _outerTrail));
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
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._innerTrail, ref val2))
		{
			_innerTrail = ((Variant)(ref val2)).As<NBasicTrail>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._outerTrail, ref val3))
		{
			_outerTrail = ((Variant)(ref val3)).As<NBasicTrail>();
		}
	}
}
