using System;
using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;

namespace MegaCrit.Sts2.Core.Nodes.Vfx;

[GlobalClass]
[ScriptPath("res://src/Core/Nodes/Vfx/NAxebotVfx.cs")]
public class NAxebotVfx : Node
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName OnAnimationEvent = StringName.op_Implicit("OnAnimationEvent");

		public static readonly StringName TurnOnDeath1 = StringName.op_Implicit("TurnOnDeath1");

		public static readonly StringName TurnOnDeath2 = StringName.op_Implicit("TurnOnDeath2");

		public static readonly StringName TurnOnHurt = StringName.op_Implicit("TurnOnHurt");

		public static readonly StringName TurnOnLandingSmoke = StringName.op_Implicit("TurnOnLandingSmoke");

		public static readonly StringName TurnOffLandingSmoke = StringName.op_Implicit("TurnOffLandingSmoke");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName _hurtParticles1 = StringName.op_Implicit("_hurtParticles1");

		public static readonly StringName _hurtParticles2 = StringName.op_Implicit("_hurtParticles2");

		public static readonly StringName _smokeParticlesLeft = StringName.op_Implicit("_smokeParticlesLeft");

		public static readonly StringName _smokeParticlesRight = StringName.op_Implicit("_smokeParticlesRight");

		public static readonly StringName _parent = StringName.op_Implicit("_parent");

		public static readonly StringName _currentWeapon = StringName.op_Implicit("_currentWeapon");
	}

	public class SignalName : SignalName
	{
	}

	private GpuParticles2D _hurtParticles1;

	private GpuParticles2D _hurtParticles2;

	private GpuParticles2D _smokeParticlesLeft;

	private GpuParticles2D _smokeParticlesRight;

	private Node2D _parent;

	private MegaSprite _animController;

	private int _currentWeapon = 1;

	public override void _Ready()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		_parent = ((Node)this).GetParent<Node2D>();
		_animController = new MegaSprite(Variant.op_Implicit((GodotObject)(object)_parent));
		_animController.ConnectAnimationEvent(Callable.From<GodotObject, GodotObject, GodotObject, GodotObject>((Action<GodotObject, GodotObject, GodotObject, GodotObject>)OnAnimationEvent));
		_hurtParticles1 = ((Node)_parent).GetNode<GpuParticles2D>(NodePath.op_Implicit("SparksBoneNode/HurtParticles1"));
		_hurtParticles2 = ((Node)_parent).GetNode<GpuParticles2D>(NodePath.op_Implicit("SparksBoneNode/HurtParticles2"));
		_smokeParticlesLeft = ((Node)_parent).GetNode<GpuParticles2D>(NodePath.op_Implicit("SmokeNodeLeft/SmokeParticles"));
		_smokeParticlesRight = ((Node)_parent).GetNode<GpuParticles2D>(NodePath.op_Implicit("SmokeNodeRight/SmokeParticles"));
		_hurtParticles1.OneShot = true;
		_hurtParticles2.OneShot = true;
		_hurtParticles1.Emitting = false;
		_hurtParticles2.Emitting = false;
		_smokeParticlesLeft.Emitting = false;
		_smokeParticlesRight.Emitting = false;
	}

	private void OnAnimationEvent(GodotObject _, GodotObject __, GodotObject ___, GodotObject spineEvent)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		switch (new MegaEvent(Variant.op_Implicit(spineEvent)).GetData().GetEventName())
		{
		case "start_hurt_sparks":
			TurnOnHurt();
			break;
		case "start_death_sparks1":
			TurnOnDeath1();
			break;
		case "start_death_sparks2":
			TurnOnDeath2();
			break;
		case "landing_smoke_start":
			TurnOnLandingSmoke();
			break;
		case "landing_smoke_end":
			TurnOffLandingSmoke();
			break;
		}
	}

	private void TurnOnDeath1()
	{
		_hurtParticles1.Restart();
	}

	private void TurnOnDeath2()
	{
		_hurtParticles2.Restart();
	}

	private void TurnOnHurt()
	{
		if (_currentWeapon == 1)
		{
			_hurtParticles1.Restart();
			_currentWeapon = 2;
		}
		else
		{
			_hurtParticles2.Restart();
			_currentWeapon = 1;
		}
	}

	private void TurnOnLandingSmoke()
	{
		_smokeParticlesLeft.Restart();
		_smokeParticlesRight.Restart();
	}

	private void TurnOffLandingSmoke()
	{
		_smokeParticlesLeft.Emitting = false;
		_smokeParticlesRight.Emitting = false;
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
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(7);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnAnimationEvent, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Object"), false),
			new PropertyInfo((Type)24, StringName.op_Implicit("__"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Object"), false),
			new PropertyInfo((Type)24, StringName.op_Implicit("___"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Object"), false),
			new PropertyInfo((Type)24, StringName.op_Implicit("spineEvent"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Object"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.TurnOnDeath1, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.TurnOnDeath2, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.TurnOnHurt, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.TurnOnLandingSmoke, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.TurnOffLandingSmoke, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
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
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
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
		if ((ref method) == MethodName.TurnOnDeath1 && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			TurnOnDeath1();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.TurnOnDeath2 && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			TurnOnDeath2();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.TurnOnHurt && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			TurnOnHurt();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.TurnOnLandingSmoke && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			TurnOnLandingSmoke();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.TurnOffLandingSmoke && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			TurnOffLandingSmoke();
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
		if ((ref method) == MethodName.TurnOnDeath1)
		{
			return true;
		}
		if ((ref method) == MethodName.TurnOnDeath2)
		{
			return true;
		}
		if ((ref method) == MethodName.TurnOnHurt)
		{
			return true;
		}
		if ((ref method) == MethodName.TurnOnLandingSmoke)
		{
			return true;
		}
		if ((ref method) == MethodName.TurnOffLandingSmoke)
		{
			return true;
		}
		return ((Node)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._hurtParticles1)
		{
			_hurtParticles1 = VariantUtils.ConvertTo<GpuParticles2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._hurtParticles2)
		{
			_hurtParticles2 = VariantUtils.ConvertTo<GpuParticles2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._smokeParticlesLeft)
		{
			_smokeParticlesLeft = VariantUtils.ConvertTo<GpuParticles2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._smokeParticlesRight)
		{
			_smokeParticlesRight = VariantUtils.ConvertTo<GpuParticles2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._parent)
		{
			_parent = VariantUtils.ConvertTo<Node2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._currentWeapon)
		{
			_currentWeapon = VariantUtils.ConvertTo<int>(ref value);
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
		if ((ref name) == PropertyName._hurtParticles1)
		{
			value = VariantUtils.CreateFrom<GpuParticles2D>(ref _hurtParticles1);
			return true;
		}
		if ((ref name) == PropertyName._hurtParticles2)
		{
			value = VariantUtils.CreateFrom<GpuParticles2D>(ref _hurtParticles2);
			return true;
		}
		if ((ref name) == PropertyName._smokeParticlesLeft)
		{
			value = VariantUtils.CreateFrom<GpuParticles2D>(ref _smokeParticlesLeft);
			return true;
		}
		if ((ref name) == PropertyName._smokeParticlesRight)
		{
			value = VariantUtils.CreateFrom<GpuParticles2D>(ref _smokeParticlesRight);
			return true;
		}
		if ((ref name) == PropertyName._parent)
		{
			value = VariantUtils.CreateFrom<Node2D>(ref _parent);
			return true;
		}
		if ((ref name) == PropertyName._currentWeapon)
		{
			value = VariantUtils.CreateFrom<int>(ref _currentWeapon);
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
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._hurtParticles1, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._hurtParticles2, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._smokeParticlesLeft, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._smokeParticlesRight, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._parent, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)2, PropertyName._currentWeapon, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		info.AddProperty(PropertyName._hurtParticles1, Variant.From<GpuParticles2D>(ref _hurtParticles1));
		info.AddProperty(PropertyName._hurtParticles2, Variant.From<GpuParticles2D>(ref _hurtParticles2));
		info.AddProperty(PropertyName._smokeParticlesLeft, Variant.From<GpuParticles2D>(ref _smokeParticlesLeft));
		info.AddProperty(PropertyName._smokeParticlesRight, Variant.From<GpuParticles2D>(ref _smokeParticlesRight));
		info.AddProperty(PropertyName._parent, Variant.From<Node2D>(ref _parent));
		info.AddProperty(PropertyName._currentWeapon, Variant.From<int>(ref _currentWeapon));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._hurtParticles1, ref val))
		{
			_hurtParticles1 = ((Variant)(ref val)).As<GpuParticles2D>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._hurtParticles2, ref val2))
		{
			_hurtParticles2 = ((Variant)(ref val2)).As<GpuParticles2D>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._smokeParticlesLeft, ref val3))
		{
			_smokeParticlesLeft = ((Variant)(ref val3)).As<GpuParticles2D>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._smokeParticlesRight, ref val4))
		{
			_smokeParticlesRight = ((Variant)(ref val4)).As<GpuParticles2D>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._parent, ref val5))
		{
			_parent = ((Variant)(ref val5)).As<Node2D>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._currentWeapon, ref val6))
		{
			_currentWeapon = ((Variant)(ref val6)).As<int>();
		}
	}
}
