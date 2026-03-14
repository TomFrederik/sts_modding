using System;
using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;

namespace MegaCrit.Sts2.Core.Nodes.Vfx;

[GlobalClass]
[ScriptPath("res://src/Core/Nodes/Vfx/NTheInsatiableVfx.cs")]
public class NTheInsatiableVfx : Node
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName OnAnimationEvent = StringName.op_Implicit("OnAnimationEvent");

		public static readonly StringName TurnOnSaliva = StringName.op_Implicit("TurnOnSaliva");

		public static readonly StringName TurnOffSaliva = StringName.op_Implicit("TurnOffSaliva");

		public static readonly StringName TurnOnDrool = StringName.op_Implicit("TurnOnDrool");

		public static readonly StringName TurnOffDrool = StringName.op_Implicit("TurnOffDrool");

		public static readonly StringName TurnOnBaseBlast = StringName.op_Implicit("TurnOnBaseBlast");

		public static readonly StringName TurnOffBaseBlast = StringName.op_Implicit("TurnOffBaseBlast");

		public static readonly StringName TurnOffContinuousParticles = StringName.op_Implicit("TurnOffContinuousParticles");

		public static readonly StringName OnAnimationStart = StringName.op_Implicit("OnAnimationStart");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName _continuousParticles = StringName.op_Implicit("_continuousParticles");

		public static readonly StringName _salivaFountainParticles = StringName.op_Implicit("_salivaFountainParticles");

		public static readonly StringName _salivaDroolParticles = StringName.op_Implicit("_salivaDroolParticles");

		public static readonly StringName _salivaCloudParticles = StringName.op_Implicit("_salivaCloudParticles");

		public static readonly StringName _baseBlastParticles = StringName.op_Implicit("_baseBlastParticles");

		public static readonly StringName _parent = StringName.op_Implicit("_parent");
	}

	public class SignalName : SignalName
	{
	}

	[Export(/*Could not decode attribute arguments.*/)]
	private CpuParticles2D[] _continuousParticles;

	private CpuParticles2D _salivaFountainParticles;

	private CpuParticles2D _salivaDroolParticles;

	private CpuParticles2D _salivaCloudParticles;

	private GpuParticles2D _baseBlastParticles;

	private Node2D _parent;

	private MegaSprite _animController;

	public override void _Ready()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		_parent = ((Node)this).GetParent<Node2D>();
		_animController = new MegaSprite(Variant.op_Implicit((GodotObject)(object)_parent));
		_animController.ConnectAnimationEvent(Callable.From<GodotObject, GodotObject, GodotObject, GodotObject>((Action<GodotObject, GodotObject, GodotObject, GodotObject>)OnAnimationEvent));
		_animController.ConnectAnimationStarted(Callable.From<GodotObject, GodotObject, GodotObject>((Action<GodotObject, GodotObject, GodotObject>)OnAnimationStart));
		_salivaFountainParticles = ((Node)_parent).GetNode<CpuParticles2D>(NodePath.op_Implicit("SalivaSlotNode/SalivaFountainParticles"));
		_salivaDroolParticles = ((Node)_parent).GetNode<CpuParticles2D>(NodePath.op_Implicit("SalivaSlotNode/SalivaDroolParticles"));
		_salivaCloudParticles = ((Node)_parent).GetNode<CpuParticles2D>(NodePath.op_Implicit("SalivaSlotNode/SalivaCloudParticles"));
		_baseBlastParticles = ((Node)_parent).GetNode<GpuParticles2D>(NodePath.op_Implicit("BaseBlastSlot/BaseBlastParticles"));
		_salivaFountainParticles.Emitting = false;
		_salivaDroolParticles.Emitting = false;
		_salivaCloudParticles.Emitting = false;
		_baseBlastParticles.Emitting = false;
	}

	private void OnAnimationEvent(GodotObject _, GodotObject __, GodotObject ___, GodotObject spineEvent)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		string eventName = new MegaEvent(Variant.op_Implicit(spineEvent)).GetData().GetEventName();
		if (eventName == null)
		{
			return;
		}
		switch (eventName.Length)
		{
		case 9:
			switch (eventName[1])
			{
			case 'r':
				if (eventName == "drool_end")
				{
					TurnOffDrool();
				}
				break;
			case 'e':
				if (eventName == "death_end")
				{
					TurnOffContinuousParticles();
				}
				break;
			}
			break;
		case 12:
			if (eventName == "saliva_start")
			{
				TurnOnSaliva();
			}
			break;
		case 10:
			if (eventName == "saliva_end")
			{
				TurnOffSaliva();
			}
			break;
		case 11:
			if (eventName == "drool_start")
			{
				TurnOnDrool();
			}
			break;
		case 16:
			if (eventName == "base_blast_start")
			{
				TurnOnBaseBlast();
			}
			break;
		case 14:
			if (eventName == "base_blast_end")
			{
				TurnOffBaseBlast();
			}
			break;
		case 13:
		case 15:
			break;
		}
	}

	private void TurnOnSaliva()
	{
		_salivaFountainParticles.Restart();
		_salivaCloudParticles.Restart();
	}

	private void TurnOffSaliva()
	{
		_salivaFountainParticles.Emitting = false;
		_salivaCloudParticles.Emitting = false;
	}

	private void TurnOnDrool()
	{
		_salivaDroolParticles.Restart();
	}

	private void TurnOffDrool()
	{
		_salivaDroolParticles.Emitting = false;
	}

	private void TurnOnBaseBlast()
	{
		_baseBlastParticles.Emitting = true;
	}

	private void TurnOffBaseBlast()
	{
		_baseBlastParticles.Emitting = false;
	}

	private void TurnOffContinuousParticles()
	{
		CpuParticles2D[] continuousParticles = _continuousParticles;
		foreach (CpuParticles2D val in continuousParticles)
		{
			val.Emitting = false;
		}
	}

	private void OnAnimationStart(GodotObject spineSprite, GodotObject animationState, GodotObject trackEntry)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		if (new MegaAnimationState(Variant.op_Implicit(animationState)).GetCurrent(0).GetAnimation().GetName() != "attack_thrash")
		{
			TurnOffBaseBlast();
		}
		if (new MegaAnimationState(Variant.op_Implicit(animationState)).GetCurrent(0).GetAnimation().GetName() != "salivate")
		{
			TurnOffSaliva();
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Expected O, but got Unknown
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Expected O, but got Unknown
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Expected O, but got Unknown
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Expected O, but got Unknown
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b2: Expected O, but got Unknown
		//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02de: Expected O, but got Unknown
		//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_030a: Expected O, but got Unknown
		//IL_0305: Unknown result type (might be due to invalid IL or missing references)
		//IL_0310: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(10);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnAnimationEvent, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Object"), false),
			new PropertyInfo((Type)24, StringName.op_Implicit("__"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Object"), false),
			new PropertyInfo((Type)24, StringName.op_Implicit("___"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Object"), false),
			new PropertyInfo((Type)24, StringName.op_Implicit("spineEvent"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Object"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.TurnOnSaliva, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.TurnOffSaliva, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.TurnOnDrool, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.TurnOffDrool, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.TurnOnBaseBlast, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.TurnOffBaseBlast, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.TurnOffContinuousParticles, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnAnimationStart, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
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
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
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
		if ((ref method) == MethodName.TurnOnSaliva && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			TurnOnSaliva();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.TurnOffSaliva && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			TurnOffSaliva();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.TurnOnDrool && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			TurnOnDrool();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.TurnOffDrool && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			TurnOffDrool();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.TurnOnBaseBlast && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			TurnOnBaseBlast();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.TurnOffBaseBlast && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			TurnOffBaseBlast();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.TurnOffContinuousParticles && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			TurnOffContinuousParticles();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnAnimationStart && ((NativeVariantPtrArgs)(ref args)).Count == 3)
		{
			OnAnimationStart(VariantUtils.ConvertTo<GodotObject>(ref ((NativeVariantPtrArgs)(ref args))[0]), VariantUtils.ConvertTo<GodotObject>(ref ((NativeVariantPtrArgs)(ref args))[1]), VariantUtils.ConvertTo<GodotObject>(ref ((NativeVariantPtrArgs)(ref args))[2]));
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
		if ((ref method) == MethodName.TurnOnSaliva)
		{
			return true;
		}
		if ((ref method) == MethodName.TurnOffSaliva)
		{
			return true;
		}
		if ((ref method) == MethodName.TurnOnDrool)
		{
			return true;
		}
		if ((ref method) == MethodName.TurnOffDrool)
		{
			return true;
		}
		if ((ref method) == MethodName.TurnOnBaseBlast)
		{
			return true;
		}
		if ((ref method) == MethodName.TurnOffBaseBlast)
		{
			return true;
		}
		if ((ref method) == MethodName.TurnOffContinuousParticles)
		{
			return true;
		}
		if ((ref method) == MethodName.OnAnimationStart)
		{
			return true;
		}
		return ((Node)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._continuousParticles)
		{
			_continuousParticles = VariantUtils.ConvertToSystemArrayOfGodotObject<CpuParticles2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._salivaFountainParticles)
		{
			_salivaFountainParticles = VariantUtils.ConvertTo<CpuParticles2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._salivaDroolParticles)
		{
			_salivaDroolParticles = VariantUtils.ConvertTo<CpuParticles2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._salivaCloudParticles)
		{
			_salivaCloudParticles = VariantUtils.ConvertTo<CpuParticles2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._baseBlastParticles)
		{
			_baseBlastParticles = VariantUtils.ConvertTo<GpuParticles2D>(ref value);
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
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName._continuousParticles)
		{
			GodotObject[] continuousParticles = (GodotObject[])(object)_continuousParticles;
			value = VariantUtils.CreateFromSystemArrayOfGodotObject(continuousParticles);
			return true;
		}
		if ((ref name) == PropertyName._salivaFountainParticles)
		{
			value = VariantUtils.CreateFrom<CpuParticles2D>(ref _salivaFountainParticles);
			return true;
		}
		if ((ref name) == PropertyName._salivaDroolParticles)
		{
			value = VariantUtils.CreateFrom<CpuParticles2D>(ref _salivaDroolParticles);
			return true;
		}
		if ((ref name) == PropertyName._salivaCloudParticles)
		{
			value = VariantUtils.CreateFrom<CpuParticles2D>(ref _salivaCloudParticles);
			return true;
		}
		if ((ref name) == PropertyName._baseBlastParticles)
		{
			value = VariantUtils.CreateFrom<GpuParticles2D>(ref _baseBlastParticles);
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
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)28, PropertyName._continuousParticles, (PropertyHint)23, "24/34:CPUParticles2D", (PropertyUsageFlags)4102, true));
		list.Add(new PropertyInfo((Type)24, PropertyName._salivaFountainParticles, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._salivaDroolParticles, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._salivaCloudParticles, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._baseBlastParticles, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._parent, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		StringName continuousParticles = PropertyName._continuousParticles;
		GodotObject[] continuousParticles2 = (GodotObject[])(object)_continuousParticles;
		info.AddProperty(continuousParticles, Variant.CreateFrom(continuousParticles2));
		info.AddProperty(PropertyName._salivaFountainParticles, Variant.From<CpuParticles2D>(ref _salivaFountainParticles));
		info.AddProperty(PropertyName._salivaDroolParticles, Variant.From<CpuParticles2D>(ref _salivaDroolParticles));
		info.AddProperty(PropertyName._salivaCloudParticles, Variant.From<CpuParticles2D>(ref _salivaCloudParticles));
		info.AddProperty(PropertyName._baseBlastParticles, Variant.From<GpuParticles2D>(ref _baseBlastParticles));
		info.AddProperty(PropertyName._parent, Variant.From<Node2D>(ref _parent));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._continuousParticles, ref val))
		{
			_continuousParticles = ((Variant)(ref val)).AsGodotObjectArray<CpuParticles2D>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._salivaFountainParticles, ref val2))
		{
			_salivaFountainParticles = ((Variant)(ref val2)).As<CpuParticles2D>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._salivaDroolParticles, ref val3))
		{
			_salivaDroolParticles = ((Variant)(ref val3)).As<CpuParticles2D>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._salivaCloudParticles, ref val4))
		{
			_salivaCloudParticles = ((Variant)(ref val4)).As<CpuParticles2D>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._baseBlastParticles, ref val5))
		{
			_baseBlastParticles = ((Variant)(ref val5)).As<GpuParticles2D>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._parent, ref val6))
		{
			_parent = ((Variant)(ref val6)).As<Node2D>();
		}
	}
}
