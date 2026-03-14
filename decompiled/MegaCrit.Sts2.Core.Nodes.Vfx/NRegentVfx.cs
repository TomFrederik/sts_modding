using System;
using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;

namespace MegaCrit.Sts2.Core.Nodes.Vfx;

[GlobalClass]
[ScriptPath("res://src/Core/Nodes/Vfx/NRegentVfx.cs")]
public class NRegentVfx : Node
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName OnAnimationEvent = StringName.op_Implicit("OnAnimationEvent");

		public static readonly StringName TurnOnDying = StringName.op_Implicit("TurnOnDying");

		public static readonly StringName TurnOnDying2 = StringName.op_Implicit("TurnOnDying2");

		public static readonly StringName TurnOffDying = StringName.op_Implicit("TurnOffDying");

		public static readonly StringName Explode = StringName.op_Implicit("Explode");

		public static readonly StringName DisableExplode = StringName.op_Implicit("DisableExplode");

		public static readonly StringName Attack = StringName.op_Implicit("Attack");

		public static readonly StringName OnAnimationStart = StringName.op_Implicit("OnAnimationStart");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName _deathParticlesArm = StringName.op_Implicit("_deathParticlesArm");

		public static readonly StringName _deathParticlesChest = StringName.op_Implicit("_deathParticlesChest");

		public static readonly StringName _deathParticlesBack = StringName.op_Implicit("_deathParticlesBack");

		public static readonly StringName _deathParticlesLeg = StringName.op_Implicit("_deathParticlesLeg");

		public static readonly StringName _deathParticlesLegL = StringName.op_Implicit("_deathParticlesLegL");

		public static readonly StringName _explosionParticles = StringName.op_Implicit("_explosionParticles");

		public static readonly StringName _attackParticlesSmall = StringName.op_Implicit("_attackParticlesSmall");

		public static readonly StringName _attackParticlesSmall2 = StringName.op_Implicit("_attackParticlesSmall2");

		public static readonly StringName _attackParticlesLarge = StringName.op_Implicit("_attackParticlesLarge");

		public static readonly StringName _curWeapon = StringName.op_Implicit("_curWeapon");

		public static readonly StringName _parent = StringName.op_Implicit("_parent");
	}

	public class SignalName : SignalName
	{
	}

	private GpuParticles2D _deathParticlesArm;

	private GpuParticles2D _deathParticlesChest;

	private GpuParticles2D _deathParticlesBack;

	private GpuParticles2D _deathParticlesLeg;

	private GpuParticles2D _deathParticlesLegL;

	private GpuParticles2D _explosionParticles;

	private MegaSprite _weapon;

	private MegaSprite _weapon2;

	private GpuParticles2D _attackParticlesSmall;

	private GpuParticles2D _attackParticlesSmall2;

	private GpuParticles2D _attackParticlesLarge;

	private MegaAnimationState _weaponAnimState;

	private MegaAnimationState _weaponAnimState2;

	private int _curWeapon = 1;

	private Node2D _parent;

	private MegaSprite _animController;

	public override void _Ready()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		_parent = ((Node)this).GetParent<Node2D>();
		_animController = new MegaSprite(Variant.op_Implicit((GodotObject)(object)_parent));
		_animController.ConnectAnimationEvent(Callable.From<GodotObject, GodotObject, GodotObject, GodotObject>((Action<GodotObject, GodotObject, GodotObject, GodotObject>)OnAnimationEvent));
		_animController.ConnectAnimationStarted(Callable.From<GodotObject, GodotObject, GodotObject>((Action<GodotObject, GodotObject, GodotObject>)OnAnimationStart));
		_deathParticlesArm = ((Node)_parent).GetNode<GpuParticles2D>(NodePath.op_Implicit("SpineArmBone/Particles"));
		_deathParticlesChest = ((Node)_parent).GetNode<GpuParticles2D>(NodePath.op_Implicit("SpineChestBone/Particles"));
		_deathParticlesBack = ((Node)_parent).GetNode<GpuParticles2D>(NodePath.op_Implicit("SpineChestBone/ParticlesBack"));
		_deathParticlesLeg = ((Node)_parent).GetNode<GpuParticles2D>(NodePath.op_Implicit("SpineLegBone/Particles"));
		_deathParticlesLegL = ((Node)_parent).GetNode<GpuParticles2D>(NodePath.op_Implicit("SpineLegBoneL/Particles"));
		_explosionParticles = ((Node)_parent).GetNode<GpuParticles2D>(NodePath.op_Implicit("Explosion"));
		_weapon = new MegaSprite(Variant.op_Implicit((GodotObject)(object)((Node)_parent).GetNode(NodePath.op_Implicit("Weapons/WeaponAnim1"))));
		_weapon2 = new MegaSprite(Variant.op_Implicit((GodotObject)(object)((Node)_parent).GetNode(NodePath.op_Implicit("Weapons/WeaponAnim2"))));
		_weaponAnimState = _weapon.GetAnimationState();
		_weaponAnimState2 = _weapon2.GetAnimationState();
		_deathParticlesArm.Emitting = false;
		_deathParticlesChest.Emitting = false;
		_deathParticlesBack.Emitting = false;
		_deathParticlesLeg.Emitting = false;
		_deathParticlesLegL.Emitting = false;
		_explosionParticles.Emitting = false;
	}

	private void OnAnimationEvent(GodotObject _, GodotObject __, GodotObject ___, GodotObject spineEvent)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		switch (new MegaEvent(Variant.op_Implicit(spineEvent)).GetData().GetEventName())
		{
		case "death_particles_start":
			TurnOnDying();
			break;
		case "death_particles_start2":
			TurnOnDying2();
			break;
		case "death_particles_end":
			TurnOffDying();
			break;
		case "explode_dead":
			Explode();
			break;
		case "explode_end":
			DisableExplode();
			break;
		case "attack1":
			Attack();
			break;
		}
	}

	private void TurnOnDying()
	{
		_deathParticlesArm.Restart();
		_deathParticlesLeg.Restart();
		_deathParticlesLegL.Restart();
	}

	private void TurnOnDying2()
	{
		_deathParticlesChest.Restart();
		_deathParticlesBack.Restart();
	}

	private void TurnOffDying()
	{
		_deathParticlesArm.Emitting = false;
		_deathParticlesChest.Emitting = false;
		_deathParticlesBack.Emitting = false;
		_deathParticlesLeg.Emitting = false;
		_deathParticlesLegL.Emitting = false;
	}

	private void Explode()
	{
		_explosionParticles.Restart();
	}

	private void DisableExplode()
	{
		_explosionParticles.Emitting = false;
	}

	private void Attack()
	{
		if (_curWeapon == 1)
		{
			_weaponAnimState.SetAnimation("attack", loop: false);
			_curWeapon = 2;
		}
		else
		{
			_weaponAnimState2.SetAnimation("attack2", loop: false);
			_curWeapon = 1;
		}
	}

	private void OnAnimationStart(GodotObject spineSprite, GodotObject animationState, GodotObject trackEntry)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		if (new MegaAnimationState(Variant.op_Implicit(animationState)).GetCurrent(0).GetAnimation().GetName() != "die")
		{
			DisableExplode();
			TurnOffDying();
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
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		//IL_0283: Expected O, but got Unknown
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02af: Expected O, but got Unknown
		//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02db: Expected O, but got Unknown
		//IL_02d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(9);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnAnimationEvent, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Object"), false),
			new PropertyInfo((Type)24, StringName.op_Implicit("__"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Object"), false),
			new PropertyInfo((Type)24, StringName.op_Implicit("___"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Object"), false),
			new PropertyInfo((Type)24, StringName.op_Implicit("spineEvent"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Object"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.TurnOnDying, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.TurnOnDying2, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.TurnOffDying, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.Explode, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.DisableExplode, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.Attack, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
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
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
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
		if ((ref method) == MethodName.TurnOnDying && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			TurnOnDying();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.TurnOnDying2 && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			TurnOnDying2();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.TurnOffDying && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			TurnOffDying();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.Explode && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			Explode();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.DisableExplode && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			DisableExplode();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.Attack && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			Attack();
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
		if ((ref method) == MethodName.TurnOnDying)
		{
			return true;
		}
		if ((ref method) == MethodName.TurnOnDying2)
		{
			return true;
		}
		if ((ref method) == MethodName.TurnOffDying)
		{
			return true;
		}
		if ((ref method) == MethodName.Explode)
		{
			return true;
		}
		if ((ref method) == MethodName.DisableExplode)
		{
			return true;
		}
		if ((ref method) == MethodName.Attack)
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
		if ((ref name) == PropertyName._deathParticlesArm)
		{
			_deathParticlesArm = VariantUtils.ConvertTo<GpuParticles2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._deathParticlesChest)
		{
			_deathParticlesChest = VariantUtils.ConvertTo<GpuParticles2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._deathParticlesBack)
		{
			_deathParticlesBack = VariantUtils.ConvertTo<GpuParticles2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._deathParticlesLeg)
		{
			_deathParticlesLeg = VariantUtils.ConvertTo<GpuParticles2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._deathParticlesLegL)
		{
			_deathParticlesLegL = VariantUtils.ConvertTo<GpuParticles2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._explosionParticles)
		{
			_explosionParticles = VariantUtils.ConvertTo<GpuParticles2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._attackParticlesSmall)
		{
			_attackParticlesSmall = VariantUtils.ConvertTo<GpuParticles2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._attackParticlesSmall2)
		{
			_attackParticlesSmall2 = VariantUtils.ConvertTo<GpuParticles2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._attackParticlesLarge)
		{
			_attackParticlesLarge = VariantUtils.ConvertTo<GpuParticles2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._curWeapon)
		{
			_curWeapon = VariantUtils.ConvertTo<int>(ref value);
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
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName._deathParticlesArm)
		{
			value = VariantUtils.CreateFrom<GpuParticles2D>(ref _deathParticlesArm);
			return true;
		}
		if ((ref name) == PropertyName._deathParticlesChest)
		{
			value = VariantUtils.CreateFrom<GpuParticles2D>(ref _deathParticlesChest);
			return true;
		}
		if ((ref name) == PropertyName._deathParticlesBack)
		{
			value = VariantUtils.CreateFrom<GpuParticles2D>(ref _deathParticlesBack);
			return true;
		}
		if ((ref name) == PropertyName._deathParticlesLeg)
		{
			value = VariantUtils.CreateFrom<GpuParticles2D>(ref _deathParticlesLeg);
			return true;
		}
		if ((ref name) == PropertyName._deathParticlesLegL)
		{
			value = VariantUtils.CreateFrom<GpuParticles2D>(ref _deathParticlesLegL);
			return true;
		}
		if ((ref name) == PropertyName._explosionParticles)
		{
			value = VariantUtils.CreateFrom<GpuParticles2D>(ref _explosionParticles);
			return true;
		}
		if ((ref name) == PropertyName._attackParticlesSmall)
		{
			value = VariantUtils.CreateFrom<GpuParticles2D>(ref _attackParticlesSmall);
			return true;
		}
		if ((ref name) == PropertyName._attackParticlesSmall2)
		{
			value = VariantUtils.CreateFrom<GpuParticles2D>(ref _attackParticlesSmall2);
			return true;
		}
		if ((ref name) == PropertyName._attackParticlesLarge)
		{
			value = VariantUtils.CreateFrom<GpuParticles2D>(ref _attackParticlesLarge);
			return true;
		}
		if ((ref name) == PropertyName._curWeapon)
		{
			value = VariantUtils.CreateFrom<int>(ref _curWeapon);
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
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._deathParticlesArm, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._deathParticlesChest, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._deathParticlesBack, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._deathParticlesLeg, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._deathParticlesLegL, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._explosionParticles, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._attackParticlesSmall, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._attackParticlesSmall2, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._attackParticlesLarge, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)2, PropertyName._curWeapon, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		info.AddProperty(PropertyName._deathParticlesArm, Variant.From<GpuParticles2D>(ref _deathParticlesArm));
		info.AddProperty(PropertyName._deathParticlesChest, Variant.From<GpuParticles2D>(ref _deathParticlesChest));
		info.AddProperty(PropertyName._deathParticlesBack, Variant.From<GpuParticles2D>(ref _deathParticlesBack));
		info.AddProperty(PropertyName._deathParticlesLeg, Variant.From<GpuParticles2D>(ref _deathParticlesLeg));
		info.AddProperty(PropertyName._deathParticlesLegL, Variant.From<GpuParticles2D>(ref _deathParticlesLegL));
		info.AddProperty(PropertyName._explosionParticles, Variant.From<GpuParticles2D>(ref _explosionParticles));
		info.AddProperty(PropertyName._attackParticlesSmall, Variant.From<GpuParticles2D>(ref _attackParticlesSmall));
		info.AddProperty(PropertyName._attackParticlesSmall2, Variant.From<GpuParticles2D>(ref _attackParticlesSmall2));
		info.AddProperty(PropertyName._attackParticlesLarge, Variant.From<GpuParticles2D>(ref _attackParticlesLarge));
		info.AddProperty(PropertyName._curWeapon, Variant.From<int>(ref _curWeapon));
		info.AddProperty(PropertyName._parent, Variant.From<Node2D>(ref _parent));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._deathParticlesArm, ref val))
		{
			_deathParticlesArm = ((Variant)(ref val)).As<GpuParticles2D>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._deathParticlesChest, ref val2))
		{
			_deathParticlesChest = ((Variant)(ref val2)).As<GpuParticles2D>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._deathParticlesBack, ref val3))
		{
			_deathParticlesBack = ((Variant)(ref val3)).As<GpuParticles2D>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._deathParticlesLeg, ref val4))
		{
			_deathParticlesLeg = ((Variant)(ref val4)).As<GpuParticles2D>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._deathParticlesLegL, ref val5))
		{
			_deathParticlesLegL = ((Variant)(ref val5)).As<GpuParticles2D>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._explosionParticles, ref val6))
		{
			_explosionParticles = ((Variant)(ref val6)).As<GpuParticles2D>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName._attackParticlesSmall, ref val7))
		{
			_attackParticlesSmall = ((Variant)(ref val7)).As<GpuParticles2D>();
		}
		Variant val8 = default(Variant);
		if (info.TryGetProperty(PropertyName._attackParticlesSmall2, ref val8))
		{
			_attackParticlesSmall2 = ((Variant)(ref val8)).As<GpuParticles2D>();
		}
		Variant val9 = default(Variant);
		if (info.TryGetProperty(PropertyName._attackParticlesLarge, ref val9))
		{
			_attackParticlesLarge = ((Variant)(ref val9)).As<GpuParticles2D>();
		}
		Variant val10 = default(Variant);
		if (info.TryGetProperty(PropertyName._curWeapon, ref val10))
		{
			_curWeapon = ((Variant)(ref val10)).As<int>();
		}
		Variant val11 = default(Variant);
		if (info.TryGetProperty(PropertyName._parent, ref val11))
		{
			_parent = ((Variant)(ref val11)).As<Node2D>();
		}
	}
}
