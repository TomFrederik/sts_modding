using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.Collections;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.Vfx.Utilities;

[ScriptPath("res://src/Core/Nodes/Vfx/Ui/NEpochChains.cs")]
public class NEpochChains : TextureRect
{
	[Signal]
	public delegate void OnAnimationFinishedEventHandler();

	public class MethodName : MethodName
	{
		public static readonly StringName UpdateParticles = StringName.op_Implicit("UpdateParticles");

		public static readonly StringName SetProperties = StringName.op_Implicit("SetProperties");

		public static readonly StringName Unlock = StringName.op_Implicit("Unlock");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName _duration = StringName.op_Implicit("_duration");

		public static readonly StringName _particles = StringName.op_Implicit("_particles");

		public static readonly StringName _endParticles = StringName.op_Implicit("_endParticles");

		public static readonly StringName _particlesCurve = StringName.op_Implicit("_particlesCurve");

		public static readonly StringName _brightEnabledCurve = StringName.op_Implicit("_brightEnabledCurve");

		public static readonly StringName _erosionEnabledCurve = StringName.op_Implicit("_erosionEnabledCurve");

		public static readonly StringName _erosionBaseCurve = StringName.op_Implicit("_erosionBaseCurve");

		public static readonly StringName _previousParticleIndex = StringName.op_Implicit("_previousParticleIndex");

		public static readonly StringName _asShaderMaterial = StringName.op_Implicit("_asShaderMaterial");
	}

	public class SignalName : SignalName
	{
		public static readonly StringName OnAnimationFinished = StringName.op_Implicit("OnAnimationFinished");
	}

	[Export(/*Could not decode attribute arguments.*/)]
	private float _duration = 0.5f;

	[Export(/*Could not decode attribute arguments.*/)]
	private Array<NParticlesContainer>? _particles;

	[Export(/*Could not decode attribute arguments.*/)]
	private NParticlesContainer? _endParticles;

	[Export(/*Could not decode attribute arguments.*/)]
	private Curve? _particlesCurve;

	[Export(/*Could not decode attribute arguments.*/)]
	private Curve _brightEnabledCurve;

	[Export(/*Could not decode attribute arguments.*/)]
	private Curve _erosionEnabledCurve;

	[Export(/*Could not decode attribute arguments.*/)]
	private Curve _erosionBaseCurve;

	private static readonly StringName _brightEnabledString = new StringName("bright_enabled");

	private static readonly StringName _erosionEnabledString = new StringName("erosion_enabled");

	private static readonly StringName _erosionBaseString = new StringName("erosion_base");

	private int _previousParticleIndex = -1;

	private ShaderMaterial? _asShaderMaterial;

	private OnAnimationFinishedEventHandler backing_OnAnimationFinished;

	public event OnAnimationFinishedEventHandler OnAnimationFinished
	{
		add
		{
			backing_OnAnimationFinished = (OnAnimationFinishedEventHandler)Delegate.Combine(backing_OnAnimationFinished, value);
		}
		remove
		{
			backing_OnAnimationFinished = (OnAnimationFinishedEventHandler)Delegate.Remove(backing_OnAnimationFinished, value);
		}
	}

	private void UpdateParticles(int index)
	{
		if (_previousParticleIndex == index)
		{
			return;
		}
		_previousParticleIndex = index;
		for (int i = 0; i < _particles.Count; i++)
		{
			if (i == index)
			{
				_particles[i].Restart();
			}
		}
	}

	private void SetProperties(float interpolation)
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		if (_asShaderMaterial != null)
		{
			float num = _brightEnabledCurve.Sample(interpolation);
			float num2 = _erosionEnabledCurve.Sample(interpolation);
			float num3 = _erosionBaseCurve.Sample(interpolation);
			_asShaderMaterial.SetShaderParameter(_brightEnabledString, Variant.op_Implicit(num));
			_asShaderMaterial.SetShaderParameter(_erosionEnabledString, Variant.op_Implicit(num2));
			_asShaderMaterial.SetShaderParameter(_erosionBaseString, Variant.op_Implicit(num3));
		}
	}

	public void Unlock()
	{
		TaskHelper.RunSafely(Unlocking());
	}

	public async Task Unlocking()
	{
		_previousParticleIndex = -1;
		((CanvasItem)this).SelfModulate = Colors.White;
		double timer = 0.0;
		Material originalMaterial = ((CanvasItem)this).Material;
		_asShaderMaterial = (ShaderMaterial)((Resource)originalMaterial).Duplicate(true);
		((CanvasItem)this).Material = (Material)(object)_asShaderMaterial;
		SetProperties(0f);
		while (timer < (double)_duration)
		{
			float num = (float)timer / _duration;
			float num2 = _particlesCurve.Sample(num);
			SetProperties(num);
			UpdateParticles(Mathf.FloorToInt(num2));
			timer += ((Node)this).GetProcessDeltaTime();
			await ((GodotObject)this).ToSignal((GodotObject)(object)((Node)this).GetTree(), SignalName.ProcessFrame);
		}
		SetProperties(1f);
		((CanvasItem)this).Material = originalMaterial;
		((GodotObject)_asShaderMaterial).Dispose();
		((CanvasItem)this).SelfModulate = new Color(1f, 1f, 1f, 0f);
		_endParticles.Restart();
		((GodotObject)this).EmitSignal(SignalName.OnAnimationFinished, Array.Empty<Variant>());
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(3);
		list.Add(new MethodInfo(MethodName.UpdateParticles, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("index"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetProperties, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)3, StringName.op_Implicit("interpolation"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.Unlock, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.UpdateParticles && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			UpdateParticles(VariantUtils.ConvertTo<int>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetProperties && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			SetProperties(VariantUtils.ConvertTo<float>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.Unlock && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			Unlock();
			ret = default(godot_variant);
			return true;
		}
		return ((TextureRect)this).InvokeGodotClassMethod(ref method, args, ref ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if ((ref method) == MethodName.UpdateParticles)
		{
			return true;
		}
		if ((ref method) == MethodName.SetProperties)
		{
			return true;
		}
		if ((ref method) == MethodName.Unlock)
		{
			return true;
		}
		return ((TextureRect)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._duration)
		{
			_duration = VariantUtils.ConvertTo<float>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._particles)
		{
			_particles = VariantUtils.ConvertToArray<NParticlesContainer>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._endParticles)
		{
			_endParticles = VariantUtils.ConvertTo<NParticlesContainer>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._particlesCurve)
		{
			_particlesCurve = VariantUtils.ConvertTo<Curve>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._brightEnabledCurve)
		{
			_brightEnabledCurve = VariantUtils.ConvertTo<Curve>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._erosionEnabledCurve)
		{
			_erosionEnabledCurve = VariantUtils.ConvertTo<Curve>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._erosionBaseCurve)
		{
			_erosionBaseCurve = VariantUtils.ConvertTo<Curve>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._previousParticleIndex)
		{
			_previousParticleIndex = VariantUtils.ConvertTo<int>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._asShaderMaterial)
		{
			_asShaderMaterial = VariantUtils.ConvertTo<ShaderMaterial>(ref value);
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
		if ((ref name) == PropertyName._duration)
		{
			value = VariantUtils.CreateFrom<float>(ref _duration);
			return true;
		}
		if ((ref name) == PropertyName._particles)
		{
			value = VariantUtils.CreateFromArray<NParticlesContainer>(_particles);
			return true;
		}
		if ((ref name) == PropertyName._endParticles)
		{
			value = VariantUtils.CreateFrom<NParticlesContainer>(ref _endParticles);
			return true;
		}
		if ((ref name) == PropertyName._particlesCurve)
		{
			value = VariantUtils.CreateFrom<Curve>(ref _particlesCurve);
			return true;
		}
		if ((ref name) == PropertyName._brightEnabledCurve)
		{
			value = VariantUtils.CreateFrom<Curve>(ref _brightEnabledCurve);
			return true;
		}
		if ((ref name) == PropertyName._erosionEnabledCurve)
		{
			value = VariantUtils.CreateFrom<Curve>(ref _erosionEnabledCurve);
			return true;
		}
		if ((ref name) == PropertyName._erosionBaseCurve)
		{
			value = VariantUtils.CreateFrom<Curve>(ref _erosionBaseCurve);
			return true;
		}
		if ((ref name) == PropertyName._previousParticleIndex)
		{
			value = VariantUtils.CreateFrom<int>(ref _previousParticleIndex);
			return true;
		}
		if ((ref name) == PropertyName._asShaderMaterial)
		{
			value = VariantUtils.CreateFrom<ShaderMaterial>(ref _asShaderMaterial);
			return true;
		}
		return ((GodotObject)this).GetGodotClassPropertyValue(ref name, ref value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)3, PropertyName._duration, (PropertyHint)0, "", (PropertyUsageFlags)4102, true));
		list.Add(new PropertyInfo((Type)28, PropertyName._particles, (PropertyHint)23, "24/34:Node2D", (PropertyUsageFlags)4102, true));
		list.Add(new PropertyInfo((Type)24, PropertyName._endParticles, (PropertyHint)34, "Node2D", (PropertyUsageFlags)4102, true));
		list.Add(new PropertyInfo((Type)24, PropertyName._particlesCurve, (PropertyHint)17, "Curve", (PropertyUsageFlags)4102, true));
		list.Add(new PropertyInfo((Type)24, PropertyName._brightEnabledCurve, (PropertyHint)17, "Curve", (PropertyUsageFlags)4102, true));
		list.Add(new PropertyInfo((Type)24, PropertyName._erosionEnabledCurve, (PropertyHint)17, "Curve", (PropertyUsageFlags)4102, true));
		list.Add(new PropertyInfo((Type)24, PropertyName._erosionBaseCurve, (PropertyHint)17, "Curve", (PropertyUsageFlags)4102, true));
		list.Add(new PropertyInfo((Type)2, PropertyName._previousParticleIndex, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._asShaderMaterial, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		((GodotObject)this).SaveGodotObjectData(info);
		info.AddProperty(PropertyName._duration, Variant.From<float>(ref _duration));
		info.AddProperty(PropertyName._particles, Variant.CreateFrom<NParticlesContainer>(_particles));
		info.AddProperty(PropertyName._endParticles, Variant.From<NParticlesContainer>(ref _endParticles));
		info.AddProperty(PropertyName._particlesCurve, Variant.From<Curve>(ref _particlesCurve));
		info.AddProperty(PropertyName._brightEnabledCurve, Variant.From<Curve>(ref _brightEnabledCurve));
		info.AddProperty(PropertyName._erosionEnabledCurve, Variant.From<Curve>(ref _erosionEnabledCurve));
		info.AddProperty(PropertyName._erosionBaseCurve, Variant.From<Curve>(ref _erosionBaseCurve));
		info.AddProperty(PropertyName._previousParticleIndex, Variant.From<int>(ref _previousParticleIndex));
		info.AddProperty(PropertyName._asShaderMaterial, Variant.From<ShaderMaterial>(ref _asShaderMaterial));
		info.AddSignalEventDelegate(SignalName.OnAnimationFinished, (Delegate)backing_OnAnimationFinished);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._duration, ref val))
		{
			_duration = ((Variant)(ref val)).As<float>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._particles, ref val2))
		{
			_particles = ((Variant)(ref val2)).AsGodotArray<NParticlesContainer>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._endParticles, ref val3))
		{
			_endParticles = ((Variant)(ref val3)).As<NParticlesContainer>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._particlesCurve, ref val4))
		{
			_particlesCurve = ((Variant)(ref val4)).As<Curve>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._brightEnabledCurve, ref val5))
		{
			_brightEnabledCurve = ((Variant)(ref val5)).As<Curve>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._erosionEnabledCurve, ref val6))
		{
			_erosionEnabledCurve = ((Variant)(ref val6)).As<Curve>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName._erosionBaseCurve, ref val7))
		{
			_erosionBaseCurve = ((Variant)(ref val7)).As<Curve>();
		}
		Variant val8 = default(Variant);
		if (info.TryGetProperty(PropertyName._previousParticleIndex, ref val8))
		{
			_previousParticleIndex = ((Variant)(ref val8)).As<int>();
		}
		Variant val9 = default(Variant);
		if (info.TryGetProperty(PropertyName._asShaderMaterial, ref val9))
		{
			_asShaderMaterial = ((Variant)(ref val9)).As<ShaderMaterial>();
		}
		OnAnimationFinishedEventHandler onAnimationFinishedEventHandler = default(OnAnimationFinishedEventHandler);
		if (info.TryGetSignalEventDelegate<OnAnimationFinishedEventHandler>(SignalName.OnAnimationFinished, ref onAnimationFinishedEventHandler))
		{
			backing_OnAnimationFinished = onAnimationFinishedEventHandler;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotSignalList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(1);
		list.Add(new MethodInfo(SignalName.OnAnimationFinished, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	protected void EmitSignalOnAnimationFinished()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).EmitSignal(SignalName.OnAnimationFinished, Array.Empty<Variant>());
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RaiseGodotClassSignalCallbacks(in godot_string_name signal, NativeVariantPtrArgs args)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		if ((ref signal) == SignalName.OnAnimationFinished && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			backing_OnAnimationFinished?.Invoke();
		}
		else
		{
			((GodotObject)this).RaiseGodotClassSignalCallbacks(ref signal, args);
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassSignal(in godot_string_name signal)
	{
		if ((ref signal) == SignalName.OnAnimationFinished)
		{
			return true;
		}
		return ((TextureRect)this).HasGodotClassSignal(ref signal);
	}
}
