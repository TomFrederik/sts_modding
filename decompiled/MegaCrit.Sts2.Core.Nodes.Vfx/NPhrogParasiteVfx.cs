using System;
using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;

namespace MegaCrit.Sts2.Core.Nodes.Vfx;

[GlobalClass]
[ScriptPath("res://src/Core/Nodes/Vfx/NPhrogParasiteVfx.cs")]
public class NPhrogParasiteVfx : Node
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName OnAnimationEvent = StringName.op_Implicit("OnAnimationEvent");

		public static readonly StringName TurnOnInfect = StringName.op_Implicit("TurnOnInfect");

		public static readonly StringName TurnOffInfect = StringName.op_Implicit("TurnOffInfect");

		public static readonly StringName StartExplode = StringName.op_Implicit("StartExplode");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName _bubbleParticlesA = StringName.op_Implicit("_bubbleParticlesA");

		public static readonly StringName _bubbleParticlesB = StringName.op_Implicit("_bubbleParticlesB");

		public static readonly StringName _bubbleParticlesC = StringName.op_Implicit("_bubbleParticlesC");

		public static readonly StringName _gooParticlesDeath = StringName.op_Implicit("_gooParticlesDeath");

		public static readonly StringName _wormParticlesDeath = StringName.op_Implicit("_wormParticlesDeath");

		public static readonly StringName _parent = StringName.op_Implicit("_parent");
	}

	public class SignalName : SignalName
	{
	}

	private GpuParticles2D _bubbleParticlesA;

	private GpuParticles2D _bubbleParticlesB;

	private GpuParticles2D _bubbleParticlesC;

	private GpuParticles2D _gooParticlesDeath;

	private GpuParticles2D _wormParticlesDeath;

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
		_bubbleParticlesA = ((Node)_parent).GetNode<GpuParticles2D>(NodePath.op_Implicit("BubbleABoneNode/WormParticlesA"));
		_bubbleParticlesB = ((Node)_parent).GetNode<GpuParticles2D>(NodePath.op_Implicit("BubbleBSlotNode/WormParticlesB"));
		_bubbleParticlesC = ((Node)_parent).GetNode<GpuParticles2D>(NodePath.op_Implicit("BubbleCBoneNode/WormParticlesC"));
		_gooParticlesDeath = ((Node)_parent).GetNode<GpuParticles2D>(NodePath.op_Implicit("DeathParticles"));
		_wormParticlesDeath = ((Node)_parent).GetNode<GpuParticles2D>(NodePath.op_Implicit("DeathWormParticles"));
		_bubbleParticlesA.Emitting = false;
		_bubbleParticlesB.Emitting = false;
		_bubbleParticlesC.Emitting = false;
		_gooParticlesDeath.Emitting = false;
		_wormParticlesDeath.Emitting = false;
		_gooParticlesDeath.OneShot = true;
		_wormParticlesDeath.OneShot = true;
		_animController.GetAnimationState().SetAnimation("die");
	}

	private void OnAnimationEvent(GodotObject _, GodotObject __, GodotObject ___, GodotObject spineEvent)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		switch (new MegaEvent(Variant.op_Implicit(spineEvent)).GetData().GetEventName())
		{
		case "infect":
			TurnOnInfect();
			break;
		case "stop_infect":
			TurnOffInfect();
			break;
		case "explode":
			StartExplode();
			break;
		}
	}

	private void TurnOnInfect()
	{
		_bubbleParticlesA.Emitting = true;
		_bubbleParticlesB.Emitting = true;
		_bubbleParticlesC.Emitting = true;
	}

	private void TurnOffInfect()
	{
		_bubbleParticlesA.Emitting = false;
		_bubbleParticlesB.Emitting = false;
		_bubbleParticlesC.Emitting = false;
	}

	private void StartExplode()
	{
		_gooParticlesDeath.Restart();
		_wormParticlesDeath.Restart();
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
		List<MethodInfo> list = new List<MethodInfo>(5);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnAnimationEvent, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Object"), false),
			new PropertyInfo((Type)24, StringName.op_Implicit("__"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Object"), false),
			new PropertyInfo((Type)24, StringName.op_Implicit("___"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Object"), false),
			new PropertyInfo((Type)24, StringName.op_Implicit("spineEvent"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Object"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.TurnOnInfect, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.TurnOffInfect, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.StartExplode, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
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
		if ((ref method) == MethodName.TurnOnInfect && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			TurnOnInfect();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.TurnOffInfect && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			TurnOffInfect();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.StartExplode && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			StartExplode();
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
		if ((ref method) == MethodName.TurnOnInfect)
		{
			return true;
		}
		if ((ref method) == MethodName.TurnOffInfect)
		{
			return true;
		}
		if ((ref method) == MethodName.StartExplode)
		{
			return true;
		}
		return ((Node)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._bubbleParticlesA)
		{
			_bubbleParticlesA = VariantUtils.ConvertTo<GpuParticles2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._bubbleParticlesB)
		{
			_bubbleParticlesB = VariantUtils.ConvertTo<GpuParticles2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._bubbleParticlesC)
		{
			_bubbleParticlesC = VariantUtils.ConvertTo<GpuParticles2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._gooParticlesDeath)
		{
			_gooParticlesDeath = VariantUtils.ConvertTo<GpuParticles2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._wormParticlesDeath)
		{
			_wormParticlesDeath = VariantUtils.ConvertTo<GpuParticles2D>(ref value);
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
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName._bubbleParticlesA)
		{
			value = VariantUtils.CreateFrom<GpuParticles2D>(ref _bubbleParticlesA);
			return true;
		}
		if ((ref name) == PropertyName._bubbleParticlesB)
		{
			value = VariantUtils.CreateFrom<GpuParticles2D>(ref _bubbleParticlesB);
			return true;
		}
		if ((ref name) == PropertyName._bubbleParticlesC)
		{
			value = VariantUtils.CreateFrom<GpuParticles2D>(ref _bubbleParticlesC);
			return true;
		}
		if ((ref name) == PropertyName._gooParticlesDeath)
		{
			value = VariantUtils.CreateFrom<GpuParticles2D>(ref _gooParticlesDeath);
			return true;
		}
		if ((ref name) == PropertyName._wormParticlesDeath)
		{
			value = VariantUtils.CreateFrom<GpuParticles2D>(ref _wormParticlesDeath);
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
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._bubbleParticlesA, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._bubbleParticlesB, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._bubbleParticlesC, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._gooParticlesDeath, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._wormParticlesDeath, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._parent, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		info.AddProperty(PropertyName._bubbleParticlesA, Variant.From<GpuParticles2D>(ref _bubbleParticlesA));
		info.AddProperty(PropertyName._bubbleParticlesB, Variant.From<GpuParticles2D>(ref _bubbleParticlesB));
		info.AddProperty(PropertyName._bubbleParticlesC, Variant.From<GpuParticles2D>(ref _bubbleParticlesC));
		info.AddProperty(PropertyName._gooParticlesDeath, Variant.From<GpuParticles2D>(ref _gooParticlesDeath));
		info.AddProperty(PropertyName._wormParticlesDeath, Variant.From<GpuParticles2D>(ref _wormParticlesDeath));
		info.AddProperty(PropertyName._parent, Variant.From<Node2D>(ref _parent));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._bubbleParticlesA, ref val))
		{
			_bubbleParticlesA = ((Variant)(ref val)).As<GpuParticles2D>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._bubbleParticlesB, ref val2))
		{
			_bubbleParticlesB = ((Variant)(ref val2)).As<GpuParticles2D>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._bubbleParticlesC, ref val3))
		{
			_bubbleParticlesC = ((Variant)(ref val3)).As<GpuParticles2D>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._gooParticlesDeath, ref val4))
		{
			_gooParticlesDeath = ((Variant)(ref val4)).As<GpuParticles2D>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._wormParticlesDeath, ref val5))
		{
			_wormParticlesDeath = ((Variant)(ref val5)).As<GpuParticles2D>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._parent, ref val6))
		{
			_parent = ((Variant)(ref val6)).As<Node2D>();
		}
	}
}
