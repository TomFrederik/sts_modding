using System;
using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;

namespace MegaCrit.Sts2.Core.Nodes.Vfx;

[ScriptPath("res://src/Core/Nodes/Vfx/NHauntedShipVfx.cs")]
public class NHauntedShipVfx : Node
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName OnAnimationEvent = StringName.op_Implicit("OnAnimationEvent");

		public static readonly StringName OnEyeBubblesStart = StringName.op_Implicit("OnEyeBubblesStart");

		public static readonly StringName OnEyeBubblesEnd = StringName.op_Implicit("OnEyeBubblesEnd");

		public static readonly StringName OnHeadBubblesStart = StringName.op_Implicit("OnHeadBubblesStart");

		public static readonly StringName OnHeadBubblesEnd = StringName.op_Implicit("OnHeadBubblesEnd");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName _eyeParticles1 = StringName.op_Implicit("_eyeParticles1");

		public static readonly StringName _eyeParticles2 = StringName.op_Implicit("_eyeParticles2");

		public static readonly StringName _eyeParticles3 = StringName.op_Implicit("_eyeParticles3");

		public static readonly StringName _headParticles1 = StringName.op_Implicit("_headParticles1");

		public static readonly StringName _headParticles2 = StringName.op_Implicit("_headParticles2");
	}

	public class SignalName : SignalName
	{
	}

	private MegaSprite _megaSprite;

	private GpuParticles2D _eyeParticles1;

	private GpuParticles2D _eyeParticles2;

	private GpuParticles2D _eyeParticles3;

	private GpuParticles2D _headParticles1;

	private GpuParticles2D _headParticles2;

	public override void _Ready()
	{
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		_eyeParticles1 = ((Node)this).GetNode<GpuParticles2D>(NodePath.op_Implicit("../EyeBone1/BubbleParticles"));
		_eyeParticles2 = ((Node)this).GetNode<GpuParticles2D>(NodePath.op_Implicit("../EyeBone2/BubbleParticles"));
		_eyeParticles3 = ((Node)this).GetNode<GpuParticles2D>(NodePath.op_Implicit("../EyeBone3/BubbleParticles"));
		_headParticles1 = ((Node)this).GetNode<GpuParticles2D>(NodePath.op_Implicit("../HeadSlot/BubbleParticles"));
		_headParticles2 = ((Node)this).GetNode<GpuParticles2D>(NodePath.op_Implicit("../HeadSlot/BubbleParticles2"));
		_eyeParticles1.Emitting = false;
		_eyeParticles2.Emitting = false;
		_eyeParticles3.Emitting = false;
		_megaSprite = new MegaSprite(Variant.op_Implicit((GodotObject)(object)((Node)this).GetParent<Node2D>()));
		_megaSprite.ConnectAnimationEvent(Callable.From<GodotObject, GodotObject, GodotObject, GodotObject>((Action<GodotObject, GodotObject, GodotObject, GodotObject>)OnAnimationEvent));
	}

	private void OnAnimationEvent(GodotObject _, GodotObject __, GodotObject ___, GodotObject spineEvent)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		switch (new MegaEvent(Variant.op_Implicit(spineEvent)).GetData().GetEventName())
		{
		case "eye_bubbles_start":
			OnEyeBubblesStart();
			break;
		case "eye_bubbles_end":
			OnEyeBubblesEnd();
			break;
		case "head_bubbles_start":
			OnHeadBubblesStart();
			break;
		case "head_bubbles_end":
			OnHeadBubblesEnd();
			break;
		}
	}

	private void OnEyeBubblesStart()
	{
		_eyeParticles1.Emitting = true;
		_eyeParticles2.Emitting = true;
		_eyeParticles3.Emitting = true;
	}

	private void OnEyeBubblesEnd()
	{
		_eyeParticles1.Emitting = false;
		_eyeParticles2.Emitting = false;
		_eyeParticles3.Emitting = false;
	}

	private void OnHeadBubblesStart()
	{
		_headParticles1.Emitting = true;
		_headParticles2.Emitting = true;
	}

	private void OnHeadBubblesEnd()
	{
		_headParticles1.Emitting = false;
		_headParticles2.Emitting = false;
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
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(6);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnAnimationEvent, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Object"), false),
			new PropertyInfo((Type)24, StringName.op_Implicit("__"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Object"), false),
			new PropertyInfo((Type)24, StringName.op_Implicit("___"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Object"), false),
			new PropertyInfo((Type)24, StringName.op_Implicit("spineEvent"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Object"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnEyeBubblesStart, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnEyeBubblesEnd, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnHeadBubblesStart, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnHeadBubblesEnd, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
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
		if ((ref method) == MethodName.OnEyeBubblesStart && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnEyeBubblesStart();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnEyeBubblesEnd && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnEyeBubblesEnd();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnHeadBubblesStart && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnHeadBubblesStart();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnHeadBubblesEnd && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnHeadBubblesEnd();
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
		if ((ref method) == MethodName.OnEyeBubblesStart)
		{
			return true;
		}
		if ((ref method) == MethodName.OnEyeBubblesEnd)
		{
			return true;
		}
		if ((ref method) == MethodName.OnHeadBubblesStart)
		{
			return true;
		}
		if ((ref method) == MethodName.OnHeadBubblesEnd)
		{
			return true;
		}
		return ((Node)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._eyeParticles1)
		{
			_eyeParticles1 = VariantUtils.ConvertTo<GpuParticles2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._eyeParticles2)
		{
			_eyeParticles2 = VariantUtils.ConvertTo<GpuParticles2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._eyeParticles3)
		{
			_eyeParticles3 = VariantUtils.ConvertTo<GpuParticles2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._headParticles1)
		{
			_headParticles1 = VariantUtils.ConvertTo<GpuParticles2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._headParticles2)
		{
			_headParticles2 = VariantUtils.ConvertTo<GpuParticles2D>(ref value);
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
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName._eyeParticles1)
		{
			value = VariantUtils.CreateFrom<GpuParticles2D>(ref _eyeParticles1);
			return true;
		}
		if ((ref name) == PropertyName._eyeParticles2)
		{
			value = VariantUtils.CreateFrom<GpuParticles2D>(ref _eyeParticles2);
			return true;
		}
		if ((ref name) == PropertyName._eyeParticles3)
		{
			value = VariantUtils.CreateFrom<GpuParticles2D>(ref _eyeParticles3);
			return true;
		}
		if ((ref name) == PropertyName._headParticles1)
		{
			value = VariantUtils.CreateFrom<GpuParticles2D>(ref _headParticles1);
			return true;
		}
		if ((ref name) == PropertyName._headParticles2)
		{
			value = VariantUtils.CreateFrom<GpuParticles2D>(ref _headParticles2);
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
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._eyeParticles1, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._eyeParticles2, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._eyeParticles3, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._headParticles1, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._headParticles2, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		((GodotObject)this).SaveGodotObjectData(info);
		info.AddProperty(PropertyName._eyeParticles1, Variant.From<GpuParticles2D>(ref _eyeParticles1));
		info.AddProperty(PropertyName._eyeParticles2, Variant.From<GpuParticles2D>(ref _eyeParticles2));
		info.AddProperty(PropertyName._eyeParticles3, Variant.From<GpuParticles2D>(ref _eyeParticles3));
		info.AddProperty(PropertyName._headParticles1, Variant.From<GpuParticles2D>(ref _headParticles1));
		info.AddProperty(PropertyName._headParticles2, Variant.From<GpuParticles2D>(ref _headParticles2));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._eyeParticles1, ref val))
		{
			_eyeParticles1 = ((Variant)(ref val)).As<GpuParticles2D>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._eyeParticles2, ref val2))
		{
			_eyeParticles2 = ((Variant)(ref val2)).As<GpuParticles2D>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._eyeParticles3, ref val3))
		{
			_eyeParticles3 = ((Variant)(ref val3)).As<GpuParticles2D>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._headParticles1, ref val4))
		{
			_headParticles1 = ((Variant)(ref val4)).As<GpuParticles2D>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._headParticles2, ref val5))
		{
			_headParticles2 = ((Variant)(ref val5)).As<GpuParticles2D>();
		}
	}
}
