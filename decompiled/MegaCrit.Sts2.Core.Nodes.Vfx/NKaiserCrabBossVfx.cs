using System;
using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Commands;

namespace MegaCrit.Sts2.Core.Nodes.Vfx;

[ScriptPath("res://src/Core/Nodes/Vfx/NKaiserCrabBossVfx.cs")]
public class NKaiserCrabBossVfx : Node
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName OnAnimationEvent = StringName.op_Implicit("OnAnimationEvent");

		public static readonly StringName OnAnimationStart = StringName.op_Implicit("OnAnimationStart");

		public static readonly StringName OnChargeSteamStart = StringName.op_Implicit("OnChargeSteamStart");

		public static readonly StringName OnChargeSteamEnd = StringName.op_Implicit("OnChargeSteamEnd");

		public static readonly StringName OnDeathSpitStart = StringName.op_Implicit("OnDeathSpitStart");

		public static readonly StringName OnDeathSpitEnd = StringName.op_Implicit("OnDeathSpitEnd");

		public static readonly StringName OnLeftEmbersStart = StringName.op_Implicit("OnLeftEmbersStart");

		public static readonly StringName OnPlowChunksStart = StringName.op_Implicit("OnPlowChunksStart");

		public static readonly StringName OnPlowChunksEnd = StringName.op_Implicit("OnPlowChunksEnd");

		public static readonly StringName OnRegenSplatsStart = StringName.op_Implicit("OnRegenSplatsStart");

		public static readonly StringName OnRegenSplatsEnd = StringName.op_Implicit("OnRegenSplatsEnd");

		public static readonly StringName OnRocketThrustStart = StringName.op_Implicit("OnRocketThrustStart");

		public static readonly StringName OnRocketThrustEnd = StringName.op_Implicit("OnRocketThrustEnd");

		public static readonly StringName OnClawLExplode = StringName.op_Implicit("OnClawLExplode");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName _regenSplatParticles = StringName.op_Implicit("_regenSplatParticles");

		public static readonly StringName _plowChunkParticles = StringName.op_Implicit("_plowChunkParticles");

		public static readonly StringName _steamParticles1 = StringName.op_Implicit("_steamParticles1");

		public static readonly StringName _steamParticles2 = StringName.op_Implicit("_steamParticles2");

		public static readonly StringName _steamParticles3 = StringName.op_Implicit("_steamParticles3");

		public static readonly StringName _smokeParticles = StringName.op_Implicit("_smokeParticles");

		public static readonly StringName _sparkParticles = StringName.op_Implicit("_sparkParticles");

		public static readonly StringName _spittleParticles = StringName.op_Implicit("_spittleParticles");

		public static readonly StringName _leftArmExplosionPosition = StringName.op_Implicit("_leftArmExplosionPosition");

		public static readonly StringName _parent = StringName.op_Implicit("_parent");
	}

	public class SignalName : SignalName
	{
	}

	private MegaSprite _megaSprite;

	private GpuParticles2D _regenSplatParticles;

	private GpuParticles2D _plowChunkParticles;

	private GpuParticles2D _steamParticles1;

	private GpuParticles2D _steamParticles2;

	private GpuParticles2D _steamParticles3;

	private GpuParticles2D _smokeParticles;

	private GpuParticles2D _sparkParticles;

	private GpuParticles2D _spittleParticles;

	private Node2D _leftArmExplosionPosition;

	private Node2D _parent;

	public override void _Ready()
	{
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		_parent = ((Node)this).GetParent<Node2D>();
		_regenSplatParticles = ((Node)_parent).GetNode<GpuParticles2D>(NodePath.op_Implicit("RegenSplatSlot/RegenSplatParticles"));
		_plowChunkParticles = ((Node)_parent).GetNode<GpuParticles2D>(NodePath.op_Implicit("PlowChunkSlot/PlowChunkParticles"));
		_steamParticles1 = ((Node)_parent).GetNode<GpuParticles2D>(NodePath.op_Implicit("RocketSlot/SteamParticles1"));
		_steamParticles2 = ((Node)_parent).GetNode<GpuParticles2D>(NodePath.op_Implicit("RocketSlot/SteamParticles2"));
		_steamParticles3 = ((Node)_parent).GetNode<GpuParticles2D>(NodePath.op_Implicit("RocketSlot/SteamParticles3"));
		_sparkParticles = ((Node)_parent).GetNode<GpuParticles2D>(NodePath.op_Implicit("RocketSlot/SparkParticles"));
		_smokeParticles = ((Node)_parent).GetNode<GpuParticles2D>(NodePath.op_Implicit("RocketSlot/SmokeParticles"));
		_leftArmExplosionPosition = ((Node)_parent).GetNode<Node2D>(NodePath.op_Implicit("%LeftArmExplosionPosition"));
		_spittleParticles = ((Node)_parent).GetNode<GpuParticles2D>(NodePath.op_Implicit("SpittleSlot/SpittleParticles"));
		_megaSprite = new MegaSprite(Variant.op_Implicit((GodotObject)(object)((Node)this).GetParent<Node2D>()));
		_megaSprite.ConnectAnimationEvent(Callable.From<GodotObject, GodotObject, GodotObject, GodotObject>((Action<GodotObject, GodotObject, GodotObject, GodotObject>)OnAnimationEvent));
		_megaSprite.ConnectAnimationStarted(Callable.From<GodotObject, GodotObject, GodotObject>((Action<GodotObject, GodotObject, GodotObject>)OnAnimationStart));
		_regenSplatParticles.Emitting = false;
		_plowChunkParticles.Emitting = false;
		_steamParticles1.Emitting = false;
		_steamParticles2.Emitting = false;
		_steamParticles3.Emitting = false;
		_spittleParticles.Emitting = false;
		_sparkParticles.Emitting = false;
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
		case 18:
			switch (eventName[0])
			{
			case 'c':
				if (eventName == "charge_steam_start")
				{
					OnChargeSteamStart();
				}
				break;
			case 'r':
				if (eventName == "regen_splats_start")
				{
					OnRegenSplatsStart();
				}
				break;
			}
			break;
		case 16:
			switch (eventName[0])
			{
			case 'c':
				if (eventName == "charge_steam_end")
				{
					OnChargeSteamEnd();
				}
				break;
			case 'd':
				if (eventName == "death_spit_start")
				{
					OnDeathSpitStart();
				}
				break;
			case 'r':
				if (eventName == "regen_splats_end")
				{
					OnRegenSplatsEnd();
				}
				break;
			}
			break;
		case 14:
			switch (eventName[0])
			{
			case 'd':
				if (eventName == "death_spit_end")
				{
					OnDeathSpitEnd();
				}
				break;
			case 'c':
				if (eventName == "claw_explode_l")
				{
					OnClawLExplode();
				}
				break;
			}
			break;
		case 17:
			switch (eventName[0])
			{
			case 'p':
				if (eventName == "plow_chunks_start")
				{
					OnPlowChunksStart();
				}
				break;
			case 'r':
				if (eventName == "rocket_thrust_end")
				{
					OnRocketThrustEnd();
				}
				break;
			}
			break;
		case 15:
			if (eventName == "plow_chunks_end")
			{
				OnPlowChunksEnd();
			}
			break;
		case 19:
			if (eventName == "rocket_thrust_start")
			{
				OnRocketThrustStart();
			}
			break;
		}
	}

	private void OnAnimationStart(GodotObject spineSprite, GodotObject animationState, GodotObject trackEntry)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		string name = new MegaAnimationState(Variant.op_Implicit(animationState)).GetCurrent(2).GetAnimation().GetName();
		if (name != "right/charged_loop" && name != "right/charge_up")
		{
			OnChargeSteamEnd();
		}
		if (name != "right/attack_heavy")
		{
			OnRocketThrustEnd();
		}
	}

	private void OnChargeSteamStart()
	{
		_steamParticles1.Restart();
		_steamParticles2.Restart();
		_steamParticles3.Restart();
	}

	private void OnChargeSteamEnd()
	{
		_steamParticles1.Emitting = false;
		_steamParticles2.Emitting = false;
		_steamParticles3.Emitting = false;
	}

	private void OnDeathSpitStart()
	{
		_spittleParticles.Restart();
	}

	private void OnDeathSpitEnd()
	{
		_spittleParticles.Emitting = false;
	}

	private void OnLeftEmbersStart()
	{
	}

	private void OnPlowChunksStart()
	{
		_plowChunkParticles.Restart();
	}

	private void OnPlowChunksEnd()
	{
		_plowChunkParticles.Emitting = false;
	}

	private void OnRegenSplatsStart()
	{
		_regenSplatParticles.Restart();
	}

	private void OnRegenSplatsEnd()
	{
		_regenSplatParticles.Emitting = false;
	}

	private void OnRocketThrustStart()
	{
		_smokeParticles.Restart();
		_sparkParticles.Restart();
	}

	private void OnRocketThrustEnd()
	{
		_smokeParticles.Emitting = false;
		_sparkParticles.Emitting = false;
	}

	private void OnClawLExplode()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		VfxCmd.PlayVfx(_leftArmExplosionPosition.GlobalPosition, "vfx/monsters/kaiser_crab_boss_explosion");
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
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Expected O, but got Unknown
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Expected O, but got Unknown
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Expected O, but got Unknown
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0283: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0307: Unknown result type (might be due to invalid IL or missing references)
		//IL_0310: Unknown result type (might be due to invalid IL or missing references)
		//IL_0336: Unknown result type (might be due to invalid IL or missing references)
		//IL_033f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0365: Unknown result type (might be due to invalid IL or missing references)
		//IL_036e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0394: Unknown result type (might be due to invalid IL or missing references)
		//IL_039d: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fb: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(15);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnAnimationEvent, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Object"), false),
			new PropertyInfo((Type)24, StringName.op_Implicit("__"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Object"), false),
			new PropertyInfo((Type)24, StringName.op_Implicit("___"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Object"), false),
			new PropertyInfo((Type)24, StringName.op_Implicit("spineEvent"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Object"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnAnimationStart, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("spineSprite"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Object"), false),
			new PropertyInfo((Type)24, StringName.op_Implicit("animationState"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Object"), false),
			new PropertyInfo((Type)24, StringName.op_Implicit("trackEntry"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Object"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnChargeSteamStart, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnChargeSteamEnd, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnDeathSpitStart, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnDeathSpitEnd, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnLeftEmbersStart, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnPlowChunksStart, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnPlowChunksEnd, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnRegenSplatsStart, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnRegenSplatsEnd, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnRocketThrustStart, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnRocketThrustEnd, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnClawLExplode, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_028a: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0280: Unknown result type (might be due to invalid IL or missing references)
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
		if ((ref method) == MethodName.OnAnimationStart && ((NativeVariantPtrArgs)(ref args)).Count == 3)
		{
			OnAnimationStart(VariantUtils.ConvertTo<GodotObject>(ref ((NativeVariantPtrArgs)(ref args))[0]), VariantUtils.ConvertTo<GodotObject>(ref ((NativeVariantPtrArgs)(ref args))[1]), VariantUtils.ConvertTo<GodotObject>(ref ((NativeVariantPtrArgs)(ref args))[2]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnChargeSteamStart && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnChargeSteamStart();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnChargeSteamEnd && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnChargeSteamEnd();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnDeathSpitStart && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnDeathSpitStart();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnDeathSpitEnd && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnDeathSpitEnd();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnLeftEmbersStart && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnLeftEmbersStart();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnPlowChunksStart && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnPlowChunksStart();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnPlowChunksEnd && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnPlowChunksEnd();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnRegenSplatsStart && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnRegenSplatsStart();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnRegenSplatsEnd && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnRegenSplatsEnd();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnRocketThrustStart && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnRocketThrustStart();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnRocketThrustEnd && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnRocketThrustEnd();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnClawLExplode && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnClawLExplode();
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
		if ((ref method) == MethodName.OnAnimationStart)
		{
			return true;
		}
		if ((ref method) == MethodName.OnChargeSteamStart)
		{
			return true;
		}
		if ((ref method) == MethodName.OnChargeSteamEnd)
		{
			return true;
		}
		if ((ref method) == MethodName.OnDeathSpitStart)
		{
			return true;
		}
		if ((ref method) == MethodName.OnDeathSpitEnd)
		{
			return true;
		}
		if ((ref method) == MethodName.OnLeftEmbersStart)
		{
			return true;
		}
		if ((ref method) == MethodName.OnPlowChunksStart)
		{
			return true;
		}
		if ((ref method) == MethodName.OnPlowChunksEnd)
		{
			return true;
		}
		if ((ref method) == MethodName.OnRegenSplatsStart)
		{
			return true;
		}
		if ((ref method) == MethodName.OnRegenSplatsEnd)
		{
			return true;
		}
		if ((ref method) == MethodName.OnRocketThrustStart)
		{
			return true;
		}
		if ((ref method) == MethodName.OnRocketThrustEnd)
		{
			return true;
		}
		if ((ref method) == MethodName.OnClawLExplode)
		{
			return true;
		}
		return ((Node)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._regenSplatParticles)
		{
			_regenSplatParticles = VariantUtils.ConvertTo<GpuParticles2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._plowChunkParticles)
		{
			_plowChunkParticles = VariantUtils.ConvertTo<GpuParticles2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._steamParticles1)
		{
			_steamParticles1 = VariantUtils.ConvertTo<GpuParticles2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._steamParticles2)
		{
			_steamParticles2 = VariantUtils.ConvertTo<GpuParticles2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._steamParticles3)
		{
			_steamParticles3 = VariantUtils.ConvertTo<GpuParticles2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._smokeParticles)
		{
			_smokeParticles = VariantUtils.ConvertTo<GpuParticles2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._sparkParticles)
		{
			_sparkParticles = VariantUtils.ConvertTo<GpuParticles2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._spittleParticles)
		{
			_spittleParticles = VariantUtils.ConvertTo<GpuParticles2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._leftArmExplosionPosition)
		{
			_leftArmExplosionPosition = VariantUtils.ConvertTo<Node2D>(ref value);
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
		if ((ref name) == PropertyName._regenSplatParticles)
		{
			value = VariantUtils.CreateFrom<GpuParticles2D>(ref _regenSplatParticles);
			return true;
		}
		if ((ref name) == PropertyName._plowChunkParticles)
		{
			value = VariantUtils.CreateFrom<GpuParticles2D>(ref _plowChunkParticles);
			return true;
		}
		if ((ref name) == PropertyName._steamParticles1)
		{
			value = VariantUtils.CreateFrom<GpuParticles2D>(ref _steamParticles1);
			return true;
		}
		if ((ref name) == PropertyName._steamParticles2)
		{
			value = VariantUtils.CreateFrom<GpuParticles2D>(ref _steamParticles2);
			return true;
		}
		if ((ref name) == PropertyName._steamParticles3)
		{
			value = VariantUtils.CreateFrom<GpuParticles2D>(ref _steamParticles3);
			return true;
		}
		if ((ref name) == PropertyName._smokeParticles)
		{
			value = VariantUtils.CreateFrom<GpuParticles2D>(ref _smokeParticles);
			return true;
		}
		if ((ref name) == PropertyName._sparkParticles)
		{
			value = VariantUtils.CreateFrom<GpuParticles2D>(ref _sparkParticles);
			return true;
		}
		if ((ref name) == PropertyName._spittleParticles)
		{
			value = VariantUtils.CreateFrom<GpuParticles2D>(ref _spittleParticles);
			return true;
		}
		if ((ref name) == PropertyName._leftArmExplosionPosition)
		{
			value = VariantUtils.CreateFrom<Node2D>(ref _leftArmExplosionPosition);
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
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._regenSplatParticles, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._plowChunkParticles, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._steamParticles1, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._steamParticles2, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._steamParticles3, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._smokeParticles, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._sparkParticles, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._spittleParticles, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._leftArmExplosionPosition, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		((GodotObject)this).SaveGodotObjectData(info);
		info.AddProperty(PropertyName._regenSplatParticles, Variant.From<GpuParticles2D>(ref _regenSplatParticles));
		info.AddProperty(PropertyName._plowChunkParticles, Variant.From<GpuParticles2D>(ref _plowChunkParticles));
		info.AddProperty(PropertyName._steamParticles1, Variant.From<GpuParticles2D>(ref _steamParticles1));
		info.AddProperty(PropertyName._steamParticles2, Variant.From<GpuParticles2D>(ref _steamParticles2));
		info.AddProperty(PropertyName._steamParticles3, Variant.From<GpuParticles2D>(ref _steamParticles3));
		info.AddProperty(PropertyName._smokeParticles, Variant.From<GpuParticles2D>(ref _smokeParticles));
		info.AddProperty(PropertyName._sparkParticles, Variant.From<GpuParticles2D>(ref _sparkParticles));
		info.AddProperty(PropertyName._spittleParticles, Variant.From<GpuParticles2D>(ref _spittleParticles));
		info.AddProperty(PropertyName._leftArmExplosionPosition, Variant.From<Node2D>(ref _leftArmExplosionPosition));
		info.AddProperty(PropertyName._parent, Variant.From<Node2D>(ref _parent));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._regenSplatParticles, ref val))
		{
			_regenSplatParticles = ((Variant)(ref val)).As<GpuParticles2D>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._plowChunkParticles, ref val2))
		{
			_plowChunkParticles = ((Variant)(ref val2)).As<GpuParticles2D>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._steamParticles1, ref val3))
		{
			_steamParticles1 = ((Variant)(ref val3)).As<GpuParticles2D>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._steamParticles2, ref val4))
		{
			_steamParticles2 = ((Variant)(ref val4)).As<GpuParticles2D>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._steamParticles3, ref val5))
		{
			_steamParticles3 = ((Variant)(ref val5)).As<GpuParticles2D>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._smokeParticles, ref val6))
		{
			_smokeParticles = ((Variant)(ref val6)).As<GpuParticles2D>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName._sparkParticles, ref val7))
		{
			_sparkParticles = ((Variant)(ref val7)).As<GpuParticles2D>();
		}
		Variant val8 = default(Variant);
		if (info.TryGetProperty(PropertyName._spittleParticles, ref val8))
		{
			_spittleParticles = ((Variant)(ref val8)).As<GpuParticles2D>();
		}
		Variant val9 = default(Variant);
		if (info.TryGetProperty(PropertyName._leftArmExplosionPosition, ref val9))
		{
			_leftArmExplosionPosition = ((Variant)(ref val9)).As<Node2D>();
		}
		Variant val10 = default(Variant);
		if (info.TryGetProperty(PropertyName._parent, ref val10))
		{
			_parent = ((Variant)(ref val10)).As<Node2D>();
		}
	}
}
