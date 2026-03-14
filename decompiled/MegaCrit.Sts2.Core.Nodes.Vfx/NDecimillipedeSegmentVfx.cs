using System;
using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Random;

namespace MegaCrit.Sts2.Core.Nodes.Vfx;

[GlobalClass]
[ScriptPath("res://src/Core/Nodes/Vfx/NDecimillipedeSegmentVfx.cs")]
public class NDecimillipedeSegmentVfx : Node
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName Regenerate = StringName.op_Implicit("Regenerate");

		public static readonly StringName EndRegenerate = StringName.op_Implicit("EndRegenerate");

		public static readonly StringName Wither = StringName.op_Implicit("Wither");

		public static readonly StringName OnAnimationEvent = StringName.op_Implicit("OnAnimationEvent");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName _damageParticleNodes = StringName.op_Implicit("_damageParticleNodes");

		public static readonly StringName _particleGravity = StringName.op_Implicit("_particleGravity");

		public static readonly StringName _particleSpeedScale = StringName.op_Implicit("_particleSpeedScale");

		public static readonly StringName _particleVelocityMinMax = StringName.op_Implicit("_particleVelocityMinMax");

		public static readonly StringName _sprayNodes = StringName.op_Implicit("_sprayNodes");

		public static readonly StringName _parent = StringName.op_Implicit("_parent");
	}

	public class SignalName : SignalName
	{
	}

	private static readonly StringName _opacity = new StringName("opacity");

	private static readonly StringName _direction = new StringName("direction");

	[Export(/*Could not decode attribute arguments.*/)]
	private CpuParticles2D[] _damageParticleNodes;

	private readonly Vector2 _particleGravity = new Vector2(0f, 300f);

	private float _particleSpeedScale = 2f;

	private Vector2 _particleVelocityMinMax = new Vector2(400f, 600f);

	[Export(/*Could not decode attribute arguments.*/)]
	private Node2D[] _sprayNodes;

	private Node2D _parent;

	private MegaSprite _animController;

	private readonly List<Vector2> _sprayNodeScales = new List<Vector2>();

	public override void _Ready()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		_parent = ((Node)this).GetParent<Node2D>();
		_animController = new MegaSprite(Variant.op_Implicit((GodotObject)(object)_parent));
		_animController.ConnectAnimationEvent(Callable.From<GodotObject, GodotObject, GodotObject, GodotObject>((Action<GodotObject, GodotObject, GodotObject, GodotObject>)OnAnimationEvent));
		Node2D[] sprayNodes = _sprayNodes;
		foreach (Node2D val in sprayNodes)
		{
			((CanvasItem)val).Visible = false;
			_sprayNodeScales.Add(val.Scale);
		}
	}

	public void Regenerate()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Expected O, but got Unknown
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < _sprayNodes.Length; i++)
		{
			Node2D val = _sprayNodes[i];
			((CanvasItem)val).Visible = true;
			ShaderMaterial val2 = (ShaderMaterial)((CanvasItem)val).Material;
			val2.SetShaderParameter(_direction, Variant.op_Implicit(-1));
			Tween val3 = ((Node)this).CreateTween().SetParallel(true);
			float num = Rng.Chaotic.NextFloat(0.5f);
			val2.SetShaderParameter(_opacity, Variant.op_Implicit(0.5f));
			val3.TweenProperty((GodotObject)(object)val, NodePath.op_Implicit("scale"), Variant.op_Implicit(_sprayNodeScales[i]), (double)(num + 0.5f)).SetTrans((TransitionType)5).SetEase((EaseType)1);
			val3.TweenProperty((GodotObject)(object)((CanvasItem)val).Material, NodePath.op_Implicit("shader_parameter/opacity"), Variant.op_Implicit(0.9f), (double)num).SetTrans((TransitionType)5).SetEase((EaseType)1);
		}
	}

	private void EndRegenerate()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		Node2D[] sprayNodes = _sprayNodes;
		foreach (Node2D val in sprayNodes)
		{
			Vector2 scale = val.Scale;
			Tween val2 = ((Node)this).CreateTween();
			float num = Rng.Chaotic.NextFloat(0.9f, 1.25f);
			val2.TweenProperty((GodotObject)(object)val, NodePath.op_Implicit("scale"), Variant.op_Implicit(Vector2.Zero), (double)num).SetTrans((TransitionType)4).SetEase((EaseType)0);
			val2.TweenProperty((GodotObject)(object)val, NodePath.op_Implicit("visible"), Variant.op_Implicit(false), 0.0);
			val2.TweenProperty((GodotObject)(object)val, NodePath.op_Implicit("scale"), Variant.op_Implicit(scale), 0.0);
		}
	}

	private void Wither()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		CpuParticles2D[] damageParticleNodes = _damageParticleNodes;
		foreach (CpuParticles2D val in damageParticleNodes)
		{
			val.Gravity = _particleGravity;
			val.SpeedScale = _particleSpeedScale;
			val.InitialVelocityMin = _particleVelocityMinMax.X;
			val.InitialVelocityMax = _particleVelocityMinMax.Y;
			val.Restart();
		}
	}

	private void OnAnimationEvent(GodotObject _, GodotObject __, GodotObject ___, GodotObject animEvent)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		string eventName = new MegaEvent(Variant.op_Implicit(animEvent)).GetData().GetEventName();
		if (!(eventName == "suck_complete"))
		{
			if (eventName == "explode")
			{
				Wither();
			}
		}
		else
		{
			EndRegenerate();
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Expected O, but got Unknown
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Expected O, but got Unknown
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Expected O, but got Unknown
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Expected O, but got Unknown
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(5);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.Regenerate, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.EndRegenerate, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.Wither, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnAnimationEvent, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Object"), false),
			new PropertyInfo((Type)24, StringName.op_Implicit("__"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Object"), false),
			new PropertyInfo((Type)24, StringName.op_Implicit("___"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Object"), false),
			new PropertyInfo((Type)24, StringName.op_Implicit("animEvent"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Object"), false)
		}, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.Regenerate && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			Regenerate();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.EndRegenerate && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			EndRegenerate();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.Wither && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			Wither();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnAnimationEvent && ((NativeVariantPtrArgs)(ref args)).Count == 4)
		{
			OnAnimationEvent(VariantUtils.ConvertTo<GodotObject>(ref ((NativeVariantPtrArgs)(ref args))[0]), VariantUtils.ConvertTo<GodotObject>(ref ((NativeVariantPtrArgs)(ref args))[1]), VariantUtils.ConvertTo<GodotObject>(ref ((NativeVariantPtrArgs)(ref args))[2]), VariantUtils.ConvertTo<GodotObject>(ref ((NativeVariantPtrArgs)(ref args))[3]));
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
		if ((ref method) == MethodName.Regenerate)
		{
			return true;
		}
		if ((ref method) == MethodName.EndRegenerate)
		{
			return true;
		}
		if ((ref method) == MethodName.Wither)
		{
			return true;
		}
		if ((ref method) == MethodName.OnAnimationEvent)
		{
			return true;
		}
		return ((Node)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName._damageParticleNodes)
		{
			_damageParticleNodes = VariantUtils.ConvertToSystemArrayOfGodotObject<CpuParticles2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._particleSpeedScale)
		{
			_particleSpeedScale = VariantUtils.ConvertTo<float>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._particleVelocityMinMax)
		{
			_particleVelocityMinMax = VariantUtils.ConvertTo<Vector2>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._sprayNodes)
		{
			_sprayNodes = VariantUtils.ConvertToSystemArrayOfGodotObject<Node2D>(ref value);
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
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName._damageParticleNodes)
		{
			GodotObject[] damageParticleNodes = (GodotObject[])(object)_damageParticleNodes;
			value = VariantUtils.CreateFromSystemArrayOfGodotObject(damageParticleNodes);
			return true;
		}
		if ((ref name) == PropertyName._particleGravity)
		{
			value = VariantUtils.CreateFrom<Vector2>(ref _particleGravity);
			return true;
		}
		if ((ref name) == PropertyName._particleSpeedScale)
		{
			value = VariantUtils.CreateFrom<float>(ref _particleSpeedScale);
			return true;
		}
		if ((ref name) == PropertyName._particleVelocityMinMax)
		{
			value = VariantUtils.CreateFrom<Vector2>(ref _particleVelocityMinMax);
			return true;
		}
		if ((ref name) == PropertyName._sprayNodes)
		{
			GodotObject[] damageParticleNodes = (GodotObject[])(object)_sprayNodes;
			value = VariantUtils.CreateFromSystemArrayOfGodotObject(damageParticleNodes);
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
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)28, PropertyName._damageParticleNodes, (PropertyHint)23, "24/34:CPUParticles2D", (PropertyUsageFlags)4102, true));
		list.Add(new PropertyInfo((Type)5, PropertyName._particleGravity, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)3, PropertyName._particleSpeedScale, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)5, PropertyName._particleVelocityMinMax, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)28, PropertyName._sprayNodes, (PropertyHint)23, "24/34:Node2D", (PropertyUsageFlags)4102, true));
		list.Add(new PropertyInfo((Type)24, PropertyName._parent, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		StringName damageParticleNodes = PropertyName._damageParticleNodes;
		GodotObject[] damageParticleNodes2 = (GodotObject[])(object)_damageParticleNodes;
		info.AddProperty(damageParticleNodes, Variant.CreateFrom(damageParticleNodes2));
		info.AddProperty(PropertyName._particleSpeedScale, Variant.From<float>(ref _particleSpeedScale));
		info.AddProperty(PropertyName._particleVelocityMinMax, Variant.From<Vector2>(ref _particleVelocityMinMax));
		StringName sprayNodes = PropertyName._sprayNodes;
		damageParticleNodes2 = (GodotObject[])(object)_sprayNodes;
		info.AddProperty(sprayNodes, Variant.CreateFrom(damageParticleNodes2));
		info.AddProperty(PropertyName._parent, Variant.From<Node2D>(ref _parent));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._damageParticleNodes, ref val))
		{
			_damageParticleNodes = ((Variant)(ref val)).AsGodotObjectArray<CpuParticles2D>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._particleSpeedScale, ref val2))
		{
			_particleSpeedScale = ((Variant)(ref val2)).As<float>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._particleVelocityMinMax, ref val3))
		{
			_particleVelocityMinMax = ((Variant)(ref val3)).As<Vector2>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._sprayNodes, ref val4))
		{
			_sprayNodes = ((Variant)(ref val4)).AsGodotObjectArray<Node2D>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._parent, ref val5))
		{
			_parent = ((Variant)(ref val5)).As<Node2D>();
		}
	}
}
