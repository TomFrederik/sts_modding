using System;
using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.Collections;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Random;

namespace MegaCrit.Sts2.Core.Nodes.Vfx;

[ScriptPath("res://src/Core/Nodes/Vfx/NRestSiteFireVfx.cs")]
public class NRestSiteFireVfx : Node2D
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName Flicker = StringName.op_Implicit("Flicker");

		public static readonly StringName Sway = StringName.op_Implicit("Sway");

		public static readonly StringName Extinguish = StringName.op_Implicit("Extinguish");

		public static readonly StringName _ExitTree = StringName.op_Implicit("_ExitTree");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName _minFlickerScale = StringName.op_Implicit("_minFlickerScale");

		public static readonly StringName _maxFlickerScale = StringName.op_Implicit("_maxFlickerScale");

		public static readonly StringName _minFlickerTime = StringName.op_Implicit("_minFlickerTime");

		public static readonly StringName _maxFlickerTime = StringName.op_Implicit("_maxFlickerTime");

		public static readonly StringName _minSkew = StringName.op_Implicit("_minSkew");

		public static readonly StringName _maxSkew = StringName.op_Implicit("_maxSkew");

		public static readonly StringName _minSkewTime = StringName.op_Implicit("_minSkewTime");

		public static readonly StringName _maxSkewTime = StringName.op_Implicit("_maxSkewTime");

		public static readonly StringName _extinguishTime = StringName.op_Implicit("_extinguishTime");

		public static readonly StringName _enabled = StringName.op_Implicit("_enabled");

		public static readonly StringName _cpuGlowParticles = StringName.op_Implicit("_cpuGlowParticles");

		public static readonly StringName _gpuSparkParticles = StringName.op_Implicit("_gpuSparkParticles");

		public static readonly StringName _baseScale = StringName.op_Implicit("_baseScale");

		public static readonly StringName _baseSkew = StringName.op_Implicit("_baseSkew");

		public static readonly StringName _scaleTweenRef = StringName.op_Implicit("_scaleTweenRef");
	}

	public class SignalName : SignalName
	{
	}

	[Export(/*Could not decode attribute arguments.*/)]
	private float _minFlickerScale = 0.85f;

	[Export(/*Could not decode attribute arguments.*/)]
	private float _maxFlickerScale = 1.05f;

	[Export(/*Could not decode attribute arguments.*/)]
	private float _minFlickerTime = 0.3f;

	[Export(/*Could not decode attribute arguments.*/)]
	private float _maxFlickerTime = 0.5f;

	[Export(/*Could not decode attribute arguments.*/)]
	private float _minSkew = -0.1f;

	[Export(/*Could not decode attribute arguments.*/)]
	private float _maxSkew = 0.1f;

	[Export(/*Could not decode attribute arguments.*/)]
	private float _minSkewTime = 0.8f;

	[Export(/*Could not decode attribute arguments.*/)]
	private float _maxSkewTime = 1.5f;

	[Export(/*Could not decode attribute arguments.*/)]
	private float _extinguishTime = 0.2f;

	[Export(/*Could not decode attribute arguments.*/)]
	private bool _enabled = true;

	[Export(/*Could not decode attribute arguments.*/)]
	private Array<CpuParticles2D> _cpuGlowParticles = new Array<CpuParticles2D>();

	[Export(/*Could not decode attribute arguments.*/)]
	private Array<GpuParticles2D> _gpuSparkParticles = new Array<GpuParticles2D>();

	private Vector2 _baseScale;

	private float _baseSkew;

	private Tween? _scaleTweenRef;

	public override void _Ready()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		if (_enabled)
		{
			_baseScale = ((Node2D)this).Scale;
			_baseSkew = ((Node2D)this).Skew;
			Flicker();
			Sway();
		}
	}

	private void Flicker()
	{
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		Vector2 val = default(Vector2);
		((Vector2)(ref val))._002Ector(_baseScale.X, Rng.Chaotic.NextFloat(_baseScale.Y * _minFlickerScale, _baseScale.Y));
		Vector2 val2 = default(Vector2);
		((Vector2)(ref val2))._002Ector(_baseScale.X, Rng.Chaotic.NextFloat(_baseScale.Y, _baseScale.Y * _maxFlickerScale));
		Tween val3 = ((Node)this).CreateTween();
		val3.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("scale"), Variant.op_Implicit(val), (double)Rng.Chaotic.NextFloat(_minFlickerTime, _maxFlickerTime)).SetTrans((TransitionType)4).SetEase((EaseType)2);
		val3.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("scale"), Variant.op_Implicit(val2), (double)Rng.Chaotic.NextFloat(_minFlickerTime, _maxFlickerTime)).SetTrans((TransitionType)4).SetEase((EaseType)2);
		val3.TweenCallback(Callable.From((Action)Flicker));
		_scaleTweenRef = val3;
	}

	private void Sway()
	{
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		float num = Rng.Chaotic.NextFloat(_baseSkew + _minSkew, _baseSkew);
		float num2 = Rng.Chaotic.NextFloat(_baseSkew, _baseSkew + _maxSkew);
		Tween val = ((Node)this).CreateTween();
		val.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("skew"), Variant.op_Implicit(num), (double)Rng.Chaotic.NextFloat(_minSkewTime, _maxSkewTime)).SetTrans((TransitionType)1).SetEase((EaseType)2);
		val.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("skew"), Variant.op_Implicit(num2), (double)Rng.Chaotic.NextFloat(_minSkewTime, _maxSkewTime)).SetTrans((TransitionType)1).SetEase((EaseType)2);
		val.TweenCallback(Callable.From((Action)Sway));
	}

	public void Extinguish()
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		Tween val = ((Node)this).CreateTween().SetParallel(true);
		foreach (CpuParticles2D cpuGlowParticle in _cpuGlowParticles)
		{
			cpuGlowParticle.Emitting = false;
			val.TweenProperty((GodotObject)(object)cpuGlowParticle, NodePath.op_Implicit("scale"), Variant.op_Implicit(Vector2.Zero), (double)_extinguishTime).SetTrans((TransitionType)10).SetEase((EaseType)0);
		}
		foreach (GpuParticles2D gpuSparkParticle in _gpuSparkParticles)
		{
			gpuSparkParticle.Emitting = false;
		}
		Tween? scaleTweenRef = _scaleTweenRef;
		if (scaleTweenRef != null)
		{
			scaleTweenRef.Kill();
		}
		Tween val2 = ((Node)this).CreateTween();
		val2.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("scale"), Variant.op_Implicit(Vector2.Zero), (double)_extinguishTime).SetTrans((TransitionType)10).SetEase((EaseType)0);
	}

	public override void _ExitTree()
	{
		Tween? scaleTweenRef = _scaleTweenRef;
		if (scaleTweenRef != null)
		{
			scaleTweenRef.Kill();
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
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(5);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.Flicker, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.Sway, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.Extinguish, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._ExitTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.Flicker && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			Flicker();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.Sway && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			Sway();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.Extinguish && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			Extinguish();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._ExitTree && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._ExitTree();
			ret = default(godot_variant);
			return true;
		}
		return ((Node2D)this).InvokeGodotClassMethod(ref method, args, ref ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if ((ref method) == MethodName._Ready)
		{
			return true;
		}
		if ((ref method) == MethodName.Flicker)
		{
			return true;
		}
		if ((ref method) == MethodName.Sway)
		{
			return true;
		}
		if ((ref method) == MethodName.Extinguish)
		{
			return true;
		}
		if ((ref method) == MethodName._ExitTree)
		{
			return true;
		}
		return ((Node2D)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName._minFlickerScale)
		{
			_minFlickerScale = VariantUtils.ConvertTo<float>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._maxFlickerScale)
		{
			_maxFlickerScale = VariantUtils.ConvertTo<float>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._minFlickerTime)
		{
			_minFlickerTime = VariantUtils.ConvertTo<float>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._maxFlickerTime)
		{
			_maxFlickerTime = VariantUtils.ConvertTo<float>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._minSkew)
		{
			_minSkew = VariantUtils.ConvertTo<float>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._maxSkew)
		{
			_maxSkew = VariantUtils.ConvertTo<float>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._minSkewTime)
		{
			_minSkewTime = VariantUtils.ConvertTo<float>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._maxSkewTime)
		{
			_maxSkewTime = VariantUtils.ConvertTo<float>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._extinguishTime)
		{
			_extinguishTime = VariantUtils.ConvertTo<float>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._enabled)
		{
			_enabled = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._cpuGlowParticles)
		{
			_cpuGlowParticles = VariantUtils.ConvertToArray<CpuParticles2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._gpuSparkParticles)
		{
			_gpuSparkParticles = VariantUtils.ConvertToArray<GpuParticles2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._baseScale)
		{
			_baseScale = VariantUtils.ConvertTo<Vector2>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._baseSkew)
		{
			_baseSkew = VariantUtils.ConvertTo<float>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._scaleTweenRef)
		{
			_scaleTweenRef = VariantUtils.ConvertTo<Tween>(ref value);
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
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName._minFlickerScale)
		{
			value = VariantUtils.CreateFrom<float>(ref _minFlickerScale);
			return true;
		}
		if ((ref name) == PropertyName._maxFlickerScale)
		{
			value = VariantUtils.CreateFrom<float>(ref _maxFlickerScale);
			return true;
		}
		if ((ref name) == PropertyName._minFlickerTime)
		{
			value = VariantUtils.CreateFrom<float>(ref _minFlickerTime);
			return true;
		}
		if ((ref name) == PropertyName._maxFlickerTime)
		{
			value = VariantUtils.CreateFrom<float>(ref _maxFlickerTime);
			return true;
		}
		if ((ref name) == PropertyName._minSkew)
		{
			value = VariantUtils.CreateFrom<float>(ref _minSkew);
			return true;
		}
		if ((ref name) == PropertyName._maxSkew)
		{
			value = VariantUtils.CreateFrom<float>(ref _maxSkew);
			return true;
		}
		if ((ref name) == PropertyName._minSkewTime)
		{
			value = VariantUtils.CreateFrom<float>(ref _minSkewTime);
			return true;
		}
		if ((ref name) == PropertyName._maxSkewTime)
		{
			value = VariantUtils.CreateFrom<float>(ref _maxSkewTime);
			return true;
		}
		if ((ref name) == PropertyName._extinguishTime)
		{
			value = VariantUtils.CreateFrom<float>(ref _extinguishTime);
			return true;
		}
		if ((ref name) == PropertyName._enabled)
		{
			value = VariantUtils.CreateFrom<bool>(ref _enabled);
			return true;
		}
		if ((ref name) == PropertyName._cpuGlowParticles)
		{
			value = VariantUtils.CreateFromArray<CpuParticles2D>(_cpuGlowParticles);
			return true;
		}
		if ((ref name) == PropertyName._gpuSparkParticles)
		{
			value = VariantUtils.CreateFromArray<GpuParticles2D>(_gpuSparkParticles);
			return true;
		}
		if ((ref name) == PropertyName._baseScale)
		{
			value = VariantUtils.CreateFrom<Vector2>(ref _baseScale);
			return true;
		}
		if ((ref name) == PropertyName._baseSkew)
		{
			value = VariantUtils.CreateFrom<float>(ref _baseSkew);
			return true;
		}
		if ((ref name) == PropertyName._scaleTweenRef)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _scaleTweenRef);
			return true;
		}
		return ((GodotObject)this).GetGodotClassPropertyValue(ref name, ref value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)3, PropertyName._minFlickerScale, (PropertyHint)0, "", (PropertyUsageFlags)4102, true));
		list.Add(new PropertyInfo((Type)3, PropertyName._maxFlickerScale, (PropertyHint)0, "", (PropertyUsageFlags)4102, true));
		list.Add(new PropertyInfo((Type)3, PropertyName._minFlickerTime, (PropertyHint)0, "", (PropertyUsageFlags)4102, true));
		list.Add(new PropertyInfo((Type)3, PropertyName._maxFlickerTime, (PropertyHint)0, "", (PropertyUsageFlags)4102, true));
		list.Add(new PropertyInfo((Type)3, PropertyName._minSkew, (PropertyHint)0, "", (PropertyUsageFlags)4102, true));
		list.Add(new PropertyInfo((Type)3, PropertyName._maxSkew, (PropertyHint)0, "", (PropertyUsageFlags)4102, true));
		list.Add(new PropertyInfo((Type)3, PropertyName._minSkewTime, (PropertyHint)0, "", (PropertyUsageFlags)4102, true));
		list.Add(new PropertyInfo((Type)3, PropertyName._maxSkewTime, (PropertyHint)0, "", (PropertyUsageFlags)4102, true));
		list.Add(new PropertyInfo((Type)3, PropertyName._extinguishTime, (PropertyHint)0, "", (PropertyUsageFlags)4102, true));
		list.Add(new PropertyInfo((Type)1, PropertyName._enabled, (PropertyHint)0, "", (PropertyUsageFlags)4102, true));
		list.Add(new PropertyInfo((Type)28, PropertyName._cpuGlowParticles, (PropertyHint)23, "24/34:CPUParticles2D", (PropertyUsageFlags)4102, true));
		list.Add(new PropertyInfo((Type)28, PropertyName._gpuSparkParticles, (PropertyHint)23, "24/34:GPUParticles2D", (PropertyUsageFlags)4102, true));
		list.Add(new PropertyInfo((Type)5, PropertyName._baseScale, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)3, PropertyName._baseSkew, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._scaleTweenRef, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		info.AddProperty(PropertyName._minFlickerScale, Variant.From<float>(ref _minFlickerScale));
		info.AddProperty(PropertyName._maxFlickerScale, Variant.From<float>(ref _maxFlickerScale));
		info.AddProperty(PropertyName._minFlickerTime, Variant.From<float>(ref _minFlickerTime));
		info.AddProperty(PropertyName._maxFlickerTime, Variant.From<float>(ref _maxFlickerTime));
		info.AddProperty(PropertyName._minSkew, Variant.From<float>(ref _minSkew));
		info.AddProperty(PropertyName._maxSkew, Variant.From<float>(ref _maxSkew));
		info.AddProperty(PropertyName._minSkewTime, Variant.From<float>(ref _minSkewTime));
		info.AddProperty(PropertyName._maxSkewTime, Variant.From<float>(ref _maxSkewTime));
		info.AddProperty(PropertyName._extinguishTime, Variant.From<float>(ref _extinguishTime));
		info.AddProperty(PropertyName._enabled, Variant.From<bool>(ref _enabled));
		info.AddProperty(PropertyName._cpuGlowParticles, Variant.CreateFrom<CpuParticles2D>(_cpuGlowParticles));
		info.AddProperty(PropertyName._gpuSparkParticles, Variant.CreateFrom<GpuParticles2D>(_gpuSparkParticles));
		info.AddProperty(PropertyName._baseScale, Variant.From<Vector2>(ref _baseScale));
		info.AddProperty(PropertyName._baseSkew, Variant.From<float>(ref _baseSkew));
		info.AddProperty(PropertyName._scaleTweenRef, Variant.From<Tween>(ref _scaleTweenRef));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._minFlickerScale, ref val))
		{
			_minFlickerScale = ((Variant)(ref val)).As<float>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._maxFlickerScale, ref val2))
		{
			_maxFlickerScale = ((Variant)(ref val2)).As<float>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._minFlickerTime, ref val3))
		{
			_minFlickerTime = ((Variant)(ref val3)).As<float>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._maxFlickerTime, ref val4))
		{
			_maxFlickerTime = ((Variant)(ref val4)).As<float>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._minSkew, ref val5))
		{
			_minSkew = ((Variant)(ref val5)).As<float>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._maxSkew, ref val6))
		{
			_maxSkew = ((Variant)(ref val6)).As<float>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName._minSkewTime, ref val7))
		{
			_minSkewTime = ((Variant)(ref val7)).As<float>();
		}
		Variant val8 = default(Variant);
		if (info.TryGetProperty(PropertyName._maxSkewTime, ref val8))
		{
			_maxSkewTime = ((Variant)(ref val8)).As<float>();
		}
		Variant val9 = default(Variant);
		if (info.TryGetProperty(PropertyName._extinguishTime, ref val9))
		{
			_extinguishTime = ((Variant)(ref val9)).As<float>();
		}
		Variant val10 = default(Variant);
		if (info.TryGetProperty(PropertyName._enabled, ref val10))
		{
			_enabled = ((Variant)(ref val10)).As<bool>();
		}
		Variant val11 = default(Variant);
		if (info.TryGetProperty(PropertyName._cpuGlowParticles, ref val11))
		{
			_cpuGlowParticles = ((Variant)(ref val11)).AsGodotArray<CpuParticles2D>();
		}
		Variant val12 = default(Variant);
		if (info.TryGetProperty(PropertyName._gpuSparkParticles, ref val12))
		{
			_gpuSparkParticles = ((Variant)(ref val12)).AsGodotArray<GpuParticles2D>();
		}
		Variant val13 = default(Variant);
		if (info.TryGetProperty(PropertyName._baseScale, ref val13))
		{
			_baseScale = ((Variant)(ref val13)).As<Vector2>();
		}
		Variant val14 = default(Variant);
		if (info.TryGetProperty(PropertyName._baseSkew, ref val14))
		{
			_baseSkew = ((Variant)(ref val14)).As<float>();
		}
		Variant val15 = default(Variant);
		if (info.TryGetProperty(PropertyName._scaleTweenRef, ref val15))
		{
			_scaleTweenRef = ((Variant)(ref val15)).As<Tween>();
		}
	}
}
