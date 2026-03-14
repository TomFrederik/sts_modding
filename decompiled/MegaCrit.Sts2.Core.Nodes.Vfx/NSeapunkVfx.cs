using System;
using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;

namespace MegaCrit.Sts2.Core.Nodes.Vfx;

[GlobalClass]
[ScriptPath("res://src/Core/Nodes/Vfx/NSeapunkVfx.cs")]
public class NSeapunkVfx : Node
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName OnAnimationEvent = StringName.op_Implicit("OnAnimationEvent");

		public static readonly StringName StartBubbles = StringName.op_Implicit("StartBubbles");

		public static readonly StringName StartWeeds = StringName.op_Implicit("StartWeeds");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName _bubbleParticles = StringName.op_Implicit("_bubbleParticles");

		public static readonly StringName _weedParticles = StringName.op_Implicit("_weedParticles");

		public static readonly StringName _parent = StringName.op_Implicit("_parent");
	}

	public class SignalName : SignalName
	{
	}

	private GpuParticles2D _bubbleParticles;

	private GpuParticles2D _weedParticles;

	private Node2D _parent;

	private MegaSprite _animController;

	public override void _Ready()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		_parent = ((Node)this).GetParent<Node2D>();
		_animController = new MegaSprite(Variant.op_Implicit((GodotObject)(object)_parent));
		_animController.ConnectAnimationEvent(Callable.From<GodotObject, GodotObject, GodotObject, GodotObject>((Action<GodotObject, GodotObject, GodotObject, GodotObject>)OnAnimationEvent));
		_bubbleParticles = ((Node)_parent).GetNode<GpuParticles2D>(NodePath.op_Implicit("BubbleParticles"));
		_weedParticles = ((Node)_parent).GetNode<GpuParticles2D>(NodePath.op_Implicit("WeedParticles"));
		_bubbleParticles.OneShot = true;
		_bubbleParticles.Emitting = false;
		_weedParticles.OneShot = true;
		_weedParticles.Emitting = false;
		_animController.GetAnimationState().SetAnimation("cast");
	}

	private void OnAnimationEvent(GodotObject _, GodotObject __, GodotObject ___, GodotObject spineEvent)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		string eventName = new MegaEvent(Variant.op_Implicit(spineEvent)).GetData().GetEventName();
		if (!(eventName == "bubbles_start"))
		{
			if (eventName == "weeds_start")
			{
				StartWeeds();
			}
		}
		else
		{
			StartBubbles();
		}
	}

	private void StartBubbles()
	{
		_bubbleParticles.Restart();
	}

	private void StartWeeds()
	{
		_weedParticles.Restart();
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
		list.Add(new MethodInfo(MethodName.StartBubbles, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.StartWeeds, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
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
		if ((ref method) == MethodName.StartBubbles && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			StartBubbles();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.StartWeeds && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			StartWeeds();
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
		if ((ref method) == MethodName.StartBubbles)
		{
			return true;
		}
		if ((ref method) == MethodName.StartWeeds)
		{
			return true;
		}
		return ((Node)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._bubbleParticles)
		{
			_bubbleParticles = VariantUtils.ConvertTo<GpuParticles2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._weedParticles)
		{
			_weedParticles = VariantUtils.ConvertTo<GpuParticles2D>(ref value);
			return true;
		}
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
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName._bubbleParticles)
		{
			value = VariantUtils.CreateFrom<GpuParticles2D>(ref _bubbleParticles);
			return true;
		}
		if ((ref name) == PropertyName._weedParticles)
		{
			value = VariantUtils.CreateFrom<GpuParticles2D>(ref _weedParticles);
			return true;
		}
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
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._bubbleParticles, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._weedParticles, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._parent, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		info.AddProperty(PropertyName._bubbleParticles, Variant.From<GpuParticles2D>(ref _bubbleParticles));
		info.AddProperty(PropertyName._weedParticles, Variant.From<GpuParticles2D>(ref _weedParticles));
		info.AddProperty(PropertyName._parent, Variant.From<Node2D>(ref _parent));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._bubbleParticles, ref val))
		{
			_bubbleParticles = ((Variant)(ref val)).As<GpuParticles2D>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._weedParticles, ref val2))
		{
			_weedParticles = ((Variant)(ref val2)).As<GpuParticles2D>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._parent, ref val3))
		{
			_parent = ((Variant)(ref val3)).As<Node2D>();
		}
	}
}
