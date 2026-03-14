using System;
using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;

namespace MegaCrit.Sts2.Core.Nodes.Vfx;

[ScriptPath("res://src/Core/Nodes/Vfx/NLivingGasVfx.cs")]
public class NLivingGasVfx : Node
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName OnAnimationEvent = StringName.op_Implicit("OnAnimationEvent");

		public static readonly StringName OnAttackStart = StringName.op_Implicit("OnAttackStart");

		public static readonly StringName OnAttackEnd = StringName.op_Implicit("OnAttackEnd");

		public static readonly StringName OnHurtStart = StringName.op_Implicit("OnHurtStart");

		public static readonly StringName OnHurtEnd = StringName.op_Implicit("OnHurtEnd");

		public static readonly StringName OnDebuffStart = StringName.op_Implicit("OnDebuffStart");

		public static readonly StringName OnDebuffEnd = StringName.op_Implicit("OnDebuffEnd");

		public static readonly StringName OnDissipate = StringName.op_Implicit("OnDissipate");

		public static readonly StringName DissipateFunction = StringName.op_Implicit("DissipateFunction");

		public static readonly StringName OnDeathBreathStart = StringName.op_Implicit("OnDeathBreathStart");

		public static readonly StringName OnDeathBreathEnd = StringName.op_Implicit("OnDeathBreathEnd");

		public static readonly StringName OnReconstitute = StringName.op_Implicit("OnReconstitute");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName _attackPuffParticles = StringName.op_Implicit("_attackPuffParticles");

		public static readonly StringName _attackSparkParticles = StringName.op_Implicit("_attackSparkParticles");

		public static readonly StringName _debuffPuffParticles = StringName.op_Implicit("_debuffPuffParticles");

		public static readonly StringName _gasPuffParticles = StringName.op_Implicit("_gasPuffParticles");

		public static readonly StringName _parent = StringName.op_Implicit("_parent");
	}

	public class SignalName : SignalName
	{
	}

	private static readonly StringName _alphaStep = new StringName("AlphaStep");

	private GpuParticles2D _attackPuffParticles;

	private GpuParticles2D _attackSparkParticles;

	private GpuParticles2D _debuffPuffParticles;

	private GpuParticles2D _gasPuffParticles;

	private MegaSlotNode _smokeSlot1;

	private MegaSlotNode _smokeSlot2;

	private MegaSlotNode _smokeSlot3;

	private List<ShaderMaterial?> _smokeMaterials;

	private List<Vector2> _smokeSteps;

	private Node2D _parent;

	private MegaSprite _megaSprite;

	public override void _Ready()
	{
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		_parent = ((Node)this).GetParent<Node2D>();
		_attackPuffParticles = ((Node)this).GetNode<GpuParticles2D>(NodePath.op_Implicit("../AttackFXNode/AttackPuffParticles"));
		_attackSparkParticles = ((Node)this).GetNode<GpuParticles2D>(NodePath.op_Implicit("../AttackFXNode/AttackSparkParticles"));
		_debuffPuffParticles = ((Node)this).GetNode<GpuParticles2D>(NodePath.op_Implicit("../AttackFXNode/DebuffPuffParticles"));
		_gasPuffParticles = ((Node)this).GetNode<GpuParticles2D>(NodePath.op_Implicit("../AttackFXNode/GasPuffParticles"));
		_smokeSlot1 = new MegaSlotNode(Variant.op_Implicit((GodotObject)(object)((Node)this).GetNode<Node2D>(NodePath.op_Implicit("../SmokeSlot1"))));
		_smokeSlot2 = new MegaSlotNode(Variant.op_Implicit((GodotObject)(object)((Node)this).GetNode<Node2D>(NodePath.op_Implicit("../SmokeSlot2"))));
		_smokeSlot3 = new MegaSlotNode(Variant.op_Implicit((GodotObject)(object)((Node)this).GetNode<Node2D>(NodePath.op_Implicit("../SmokeSlot3"))));
		_smokeMaterials = new List<ShaderMaterial>();
		List<ShaderMaterial?> smokeMaterials = _smokeMaterials;
		Material? normalMaterial = _smokeSlot1.GetNormalMaterial();
		smokeMaterials.Add((ShaderMaterial)(object)((normalMaterial is ShaderMaterial) ? normalMaterial : null));
		List<ShaderMaterial?> smokeMaterials2 = _smokeMaterials;
		Material? normalMaterial2 = _smokeSlot2.GetNormalMaterial();
		smokeMaterials2.Add((ShaderMaterial)(object)((normalMaterial2 is ShaderMaterial) ? normalMaterial2 : null));
		List<ShaderMaterial?> smokeMaterials3 = _smokeMaterials;
		Material? normalMaterial3 = _smokeSlot3.GetNormalMaterial();
		smokeMaterials3.Add((ShaderMaterial)(object)((normalMaterial3 is ShaderMaterial) ? normalMaterial3 : null));
		_smokeSteps = new List<Vector2>();
		_smokeSteps.Add((Vector2)_smokeMaterials[0].GetShaderParameter(_alphaStep));
		_smokeSteps.Add((Vector2)_smokeMaterials[1].GetShaderParameter(_alphaStep));
		_smokeSteps.Add((Vector2)_smokeMaterials[2].GetShaderParameter(_alphaStep));
		_attackPuffParticles.Emitting = false;
		_attackSparkParticles.Emitting = false;
		_debuffPuffParticles.Emitting = false;
		_gasPuffParticles.Emitting = false;
		_megaSprite = new MegaSprite(Variant.op_Implicit((GodotObject)(object)((Node)this).GetParent<Node2D>()));
		_megaSprite.ConnectAnimationEvent(Callable.From<GodotObject, GodotObject, GodotObject, GodotObject>((Action<GodotObject, GodotObject, GodotObject, GodotObject>)OnAnimationEvent));
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
		case 12:
			switch (eventName[0])
			{
			case 'a':
				if (eventName == "attack_start")
				{
					OnAttackStart();
				}
				break;
			case 'd':
				if (eventName == "debuff_start")
				{
					OnDebuffStart();
				}
				break;
			case 'r':
				if (eventName == "reconstitute")
				{
					OnReconstitute();
				}
				break;
			}
			break;
		case 10:
			switch (eventName[0])
			{
			case 'a':
				if (eventName == "attack_end")
				{
					OnAttackEnd();
				}
				break;
			case 'h':
				if (eventName == "hurt_start")
				{
					OnHurtStart();
				}
				break;
			case 'd':
				if (eventName == "debuff_end")
				{
					OnDebuffEnd();
				}
				break;
			}
			break;
		case 8:
			if (eventName == "hurt_end")
			{
				OnHurtEnd();
			}
			break;
		case 9:
			if (eventName == "dissipate")
			{
				OnDissipate();
			}
			break;
		case 16:
			if (eventName == "gas_breath_start")
			{
				OnDeathBreathStart();
			}
			break;
		case 14:
			if (eventName == "gas_breath_end")
			{
				OnDeathBreathEnd();
			}
			break;
		case 11:
		case 13:
		case 15:
			break;
		}
	}

	private void OnAttackStart()
	{
		_attackPuffParticles.Emitting = true;
		_attackSparkParticles.Amount = 24;
		_attackSparkParticles.Restart();
	}

	private void OnAttackEnd()
	{
		_attackPuffParticles.Emitting = false;
		_attackSparkParticles.Emitting = false;
	}

	private void OnHurtStart()
	{
		_attackSparkParticles.Amount = 100;
		_attackSparkParticles.Emitting = true;
	}

	private void OnHurtEnd()
	{
		_attackSparkParticles.Emitting = false;
	}

	private void OnDebuffStart()
	{
		_debuffPuffParticles.Emitting = true;
	}

	private void OnDebuffEnd()
	{
		_debuffPuffParticles.Emitting = false;
	}

	private void OnDissipate()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		Tween val = ((Node)this).CreateTween();
		val.TweenMethod(Callable.From<float>((Action<float>)DissipateFunction), Variant.op_Implicit(0f), Variant.op_Implicit(1f), 1.399999976158142);
		val.SetEase((EaseType)0).SetTrans((TransitionType)3);
	}

	private void DissipateFunction(float t)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < _smokeMaterials.Count; i++)
		{
			_smokeMaterials[i].SetShaderParameter(_alphaStep, Variant.op_Implicit(_smokeSteps[i] + (Vector2.One - _smokeSteps[i]) * t));
		}
	}

	private void OnDeathBreathStart()
	{
		_gasPuffParticles.Emitting = true;
	}

	private void OnDeathBreathEnd()
	{
		_gasPuffParticles.Emitting = false;
	}

	private void OnReconstitute()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < _smokeMaterials.Count; i++)
		{
			_smokeMaterials[i].SetShaderParameter(_alphaStep, Variant.op_Implicit(_smokeSteps[i]));
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
		//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0302: Unknown result type (might be due to invalid IL or missing references)
		//IL_030b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0331: Unknown result type (might be due to invalid IL or missing references)
		//IL_033a: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(13);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnAnimationEvent, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Object"), false),
			new PropertyInfo((Type)24, StringName.op_Implicit("__"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Object"), false),
			new PropertyInfo((Type)24, StringName.op_Implicit("___"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Object"), false),
			new PropertyInfo((Type)24, StringName.op_Implicit("spineEvent"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Object"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnAttackStart, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnAttackEnd, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnHurtStart, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnHurtEnd, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnDebuffStart, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnDebuffEnd, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnDissipate, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.DissipateFunction, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)3, StringName.op_Implicit("t"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnDeathBreathStart, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnDeathBreathEnd, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnReconstitute, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
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
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
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
		if ((ref method) == MethodName.OnAttackStart && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnAttackStart();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnAttackEnd && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnAttackEnd();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnHurtStart && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnHurtStart();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnHurtEnd && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnHurtEnd();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnDebuffStart && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnDebuffStart();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnDebuffEnd && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnDebuffEnd();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnDissipate && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnDissipate();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.DissipateFunction && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			DissipateFunction(VariantUtils.ConvertTo<float>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnDeathBreathStart && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnDeathBreathStart();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnDeathBreathEnd && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnDeathBreathEnd();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnReconstitute && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnReconstitute();
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
		if ((ref method) == MethodName.OnAttackStart)
		{
			return true;
		}
		if ((ref method) == MethodName.OnAttackEnd)
		{
			return true;
		}
		if ((ref method) == MethodName.OnHurtStart)
		{
			return true;
		}
		if ((ref method) == MethodName.OnHurtEnd)
		{
			return true;
		}
		if ((ref method) == MethodName.OnDebuffStart)
		{
			return true;
		}
		if ((ref method) == MethodName.OnDebuffEnd)
		{
			return true;
		}
		if ((ref method) == MethodName.OnDissipate)
		{
			return true;
		}
		if ((ref method) == MethodName.DissipateFunction)
		{
			return true;
		}
		if ((ref method) == MethodName.OnDeathBreathStart)
		{
			return true;
		}
		if ((ref method) == MethodName.OnDeathBreathEnd)
		{
			return true;
		}
		if ((ref method) == MethodName.OnReconstitute)
		{
			return true;
		}
		return ((Node)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._attackPuffParticles)
		{
			_attackPuffParticles = VariantUtils.ConvertTo<GpuParticles2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._attackSparkParticles)
		{
			_attackSparkParticles = VariantUtils.ConvertTo<GpuParticles2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._debuffPuffParticles)
		{
			_debuffPuffParticles = VariantUtils.ConvertTo<GpuParticles2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._gasPuffParticles)
		{
			_gasPuffParticles = VariantUtils.ConvertTo<GpuParticles2D>(ref value);
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
		if ((ref name) == PropertyName._attackPuffParticles)
		{
			value = VariantUtils.CreateFrom<GpuParticles2D>(ref _attackPuffParticles);
			return true;
		}
		if ((ref name) == PropertyName._attackSparkParticles)
		{
			value = VariantUtils.CreateFrom<GpuParticles2D>(ref _attackSparkParticles);
			return true;
		}
		if ((ref name) == PropertyName._debuffPuffParticles)
		{
			value = VariantUtils.CreateFrom<GpuParticles2D>(ref _debuffPuffParticles);
			return true;
		}
		if ((ref name) == PropertyName._gasPuffParticles)
		{
			value = VariantUtils.CreateFrom<GpuParticles2D>(ref _gasPuffParticles);
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
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._attackPuffParticles, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._attackSparkParticles, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._debuffPuffParticles, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._gasPuffParticles, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		((GodotObject)this).SaveGodotObjectData(info);
		info.AddProperty(PropertyName._attackPuffParticles, Variant.From<GpuParticles2D>(ref _attackPuffParticles));
		info.AddProperty(PropertyName._attackSparkParticles, Variant.From<GpuParticles2D>(ref _attackSparkParticles));
		info.AddProperty(PropertyName._debuffPuffParticles, Variant.From<GpuParticles2D>(ref _debuffPuffParticles));
		info.AddProperty(PropertyName._gasPuffParticles, Variant.From<GpuParticles2D>(ref _gasPuffParticles));
		info.AddProperty(PropertyName._parent, Variant.From<Node2D>(ref _parent));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._attackPuffParticles, ref val))
		{
			_attackPuffParticles = ((Variant)(ref val)).As<GpuParticles2D>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._attackSparkParticles, ref val2))
		{
			_attackSparkParticles = ((Variant)(ref val2)).As<GpuParticles2D>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._debuffPuffParticles, ref val3))
		{
			_debuffPuffParticles = ((Variant)(ref val3)).As<GpuParticles2D>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._gasPuffParticles, ref val4))
		{
			_gasPuffParticles = ((Variant)(ref val4)).As<GpuParticles2D>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._parent, ref val5))
		{
			_parent = ((Variant)(ref val5)).As<Node2D>();
		}
	}
}
