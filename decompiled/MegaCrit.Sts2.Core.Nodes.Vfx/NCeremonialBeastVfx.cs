using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Helpers;

namespace MegaCrit.Sts2.Core.Nodes.Vfx;

[GlobalClass]
[ScriptPath("res://src/Core/Nodes/Vfx/NCeremonialBeastVfx.cs")]
public class NCeremonialBeastVfx : Node, IDeathDelayer
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName OnAnimationEvent = StringName.op_Implicit("OnAnimationEvent");

		public static readonly StringName TurnOnDeathParticles = StringName.op_Implicit("TurnOnDeathParticles");

		public static readonly StringName TurnOnEnergyParticles = StringName.op_Implicit("TurnOnEnergyParticles");

		public static readonly StringName TurnOffEnergyParticles = StringName.op_Implicit("TurnOffEnergyParticles");

		public static readonly StringName OnPlowStart = StringName.op_Implicit("OnPlowStart");

		public static readonly StringName OnPlowEnd = StringName.op_Implicit("OnPlowEnd");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName _deathParticles = StringName.op_Implicit("_deathParticles");

		public static readonly StringName _energyParticlesFront = StringName.op_Implicit("_energyParticlesFront");

		public static readonly StringName _energyParticlesBack = StringName.op_Implicit("_energyParticlesBack");

		public static readonly StringName _plowStartTarget = StringName.op_Implicit("_plowStartTarget");

		public static readonly StringName _plowEndTarget = StringName.op_Implicit("_plowEndTarget");

		public static readonly StringName _parent = StringName.op_Implicit("_parent");

		public static readonly StringName _globalPlowTarget = StringName.op_Implicit("_globalPlowTarget");

		public static readonly StringName _globalPlowEndTarget = StringName.op_Implicit("_globalPlowEndTarget");
	}

	public class SignalName : SignalName
	{
	}

	[Export(/*Could not decode attribute arguments.*/)]
	private GpuParticles2D _deathParticles;

	[Export(/*Could not decode attribute arguments.*/)]
	private CpuParticles2D _energyParticlesFront;

	[Export(/*Could not decode attribute arguments.*/)]
	private CpuParticles2D _energyParticlesBack;

	[Export(/*Could not decode attribute arguments.*/)]
	private Node2D _plowStartTarget;

	[Export(/*Could not decode attribute arguments.*/)]
	private Node2D _plowEndTarget;

	private Node2D _parent;

	private MegaSprite _animController;

	private Vector2 _globalPlowTarget;

	private Vector2 _globalPlowEndTarget;

	private readonly TaskCompletionSource _deathTask = new TaskCompletionSource();

	public override void _Ready()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		_parent = ((Node)this).GetParent<Node2D>();
		_animController = new MegaSprite(Variant.op_Implicit((GodotObject)(object)_parent));
		_animController.ConnectAnimationEvent(Callable.From<GodotObject, GodotObject, GodotObject, GodotObject>((Action<GodotObject, GodotObject, GodotObject, GodotObject>)OnAnimationEvent));
		_deathParticles.OneShot = true;
		_deathParticles.Emitting = false;
		_energyParticlesBack.Emitting = true;
		_energyParticlesFront.Emitting = true;
		_globalPlowTarget = _plowStartTarget.GlobalPosition;
		_globalPlowEndTarget = _plowEndTarget.GlobalPosition;
	}

	private void OnAnimationEvent(GodotObject _, GodotObject __, GodotObject ___, GodotObject spineEvent)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		switch (new MegaEvent(Variant.op_Implicit(spineEvent)).GetData().GetEventName())
		{
		case "turnOffEnergy":
			TurnOffEnergyParticles();
			break;
		case "turnOnEnergy":
			TurnOnEnergyParticles();
			break;
		case "deathParticles":
			TurnOnDeathParticles();
			break;
		case "plowStart":
			OnPlowStart();
			break;
		case "plowEnd":
			OnPlowEnd();
			break;
		}
	}

	public Task GetDelayTask()
	{
		return _deathTask.Task;
	}

	private void TurnOnDeathParticles()
	{
		_deathParticles.Restart();
		TaskHelper.RunSafely(FinishTaskWhenDeathParticlesFinished());
	}

	private async Task FinishTaskWhenDeathParticlesFinished()
	{
		await ((GodotObject)this).ToSignal((GodotObject)(object)_deathParticles, SignalName.Finished);
		_deathTask.SetResult();
	}

	private void TurnOnEnergyParticles()
	{
		_energyParticlesFront.Emitting = true;
		_energyParticlesBack.Emitting = true;
	}

	private void TurnOffEnergyParticles()
	{
		_energyParticlesFront.Emitting = false;
		_energyParticlesBack.Emitting = false;
	}

	private void OnPlowStart()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_plowStartTarget.GlobalPosition = _globalPlowTarget;
	}

	private void OnPlowEnd()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_plowEndTarget.GlobalPosition = _globalPlowEndTarget;
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
		list.Add(new MethodInfo(MethodName.TurnOnDeathParticles, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.TurnOnEnergyParticles, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.TurnOffEnergyParticles, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnPlowStart, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnPlowEnd, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
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
		if ((ref method) == MethodName.TurnOnDeathParticles && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			TurnOnDeathParticles();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.TurnOnEnergyParticles && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			TurnOnEnergyParticles();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.TurnOffEnergyParticles && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			TurnOffEnergyParticles();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnPlowStart && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnPlowStart();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnPlowEnd && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnPlowEnd();
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
		if ((ref method) == MethodName.TurnOnDeathParticles)
		{
			return true;
		}
		if ((ref method) == MethodName.TurnOnEnergyParticles)
		{
			return true;
		}
		if ((ref method) == MethodName.TurnOffEnergyParticles)
		{
			return true;
		}
		if ((ref method) == MethodName.OnPlowStart)
		{
			return true;
		}
		if ((ref method) == MethodName.OnPlowEnd)
		{
			return true;
		}
		return ((Node)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName._deathParticles)
		{
			_deathParticles = VariantUtils.ConvertTo<GpuParticles2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._energyParticlesFront)
		{
			_energyParticlesFront = VariantUtils.ConvertTo<CpuParticles2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._energyParticlesBack)
		{
			_energyParticlesBack = VariantUtils.ConvertTo<CpuParticles2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._plowStartTarget)
		{
			_plowStartTarget = VariantUtils.ConvertTo<Node2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._plowEndTarget)
		{
			_plowEndTarget = VariantUtils.ConvertTo<Node2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._parent)
		{
			_parent = VariantUtils.ConvertTo<Node2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._globalPlowTarget)
		{
			_globalPlowTarget = VariantUtils.ConvertTo<Vector2>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._globalPlowEndTarget)
		{
			_globalPlowEndTarget = VariantUtils.ConvertTo<Vector2>(ref value);
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
		if ((ref name) == PropertyName._deathParticles)
		{
			value = VariantUtils.CreateFrom<GpuParticles2D>(ref _deathParticles);
			return true;
		}
		if ((ref name) == PropertyName._energyParticlesFront)
		{
			value = VariantUtils.CreateFrom<CpuParticles2D>(ref _energyParticlesFront);
			return true;
		}
		if ((ref name) == PropertyName._energyParticlesBack)
		{
			value = VariantUtils.CreateFrom<CpuParticles2D>(ref _energyParticlesBack);
			return true;
		}
		if ((ref name) == PropertyName._plowStartTarget)
		{
			value = VariantUtils.CreateFrom<Node2D>(ref _plowStartTarget);
			return true;
		}
		if ((ref name) == PropertyName._plowEndTarget)
		{
			value = VariantUtils.CreateFrom<Node2D>(ref _plowEndTarget);
			return true;
		}
		if ((ref name) == PropertyName._parent)
		{
			value = VariantUtils.CreateFrom<Node2D>(ref _parent);
			return true;
		}
		if ((ref name) == PropertyName._globalPlowTarget)
		{
			value = VariantUtils.CreateFrom<Vector2>(ref _globalPlowTarget);
			return true;
		}
		if ((ref name) == PropertyName._globalPlowEndTarget)
		{
			value = VariantUtils.CreateFrom<Vector2>(ref _globalPlowEndTarget);
			return true;
		}
		return ((GodotObject)this).GetGodotClassPropertyValue(ref name, ref value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._deathParticles, (PropertyHint)34, "GPUParticles2D", (PropertyUsageFlags)4102, true));
		list.Add(new PropertyInfo((Type)24, PropertyName._energyParticlesFront, (PropertyHint)34, "CPUParticles2D", (PropertyUsageFlags)4102, true));
		list.Add(new PropertyInfo((Type)24, PropertyName._energyParticlesBack, (PropertyHint)34, "CPUParticles2D", (PropertyUsageFlags)4102, true));
		list.Add(new PropertyInfo((Type)24, PropertyName._plowStartTarget, (PropertyHint)34, "Node2D", (PropertyUsageFlags)4102, true));
		list.Add(new PropertyInfo((Type)24, PropertyName._plowEndTarget, (PropertyHint)34, "Node2D", (PropertyUsageFlags)4102, true));
		list.Add(new PropertyInfo((Type)24, PropertyName._parent, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)5, PropertyName._globalPlowTarget, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)5, PropertyName._globalPlowEndTarget, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		((GodotObject)this).SaveGodotObjectData(info);
		info.AddProperty(PropertyName._deathParticles, Variant.From<GpuParticles2D>(ref _deathParticles));
		info.AddProperty(PropertyName._energyParticlesFront, Variant.From<CpuParticles2D>(ref _energyParticlesFront));
		info.AddProperty(PropertyName._energyParticlesBack, Variant.From<CpuParticles2D>(ref _energyParticlesBack));
		info.AddProperty(PropertyName._plowStartTarget, Variant.From<Node2D>(ref _plowStartTarget));
		info.AddProperty(PropertyName._plowEndTarget, Variant.From<Node2D>(ref _plowEndTarget));
		info.AddProperty(PropertyName._parent, Variant.From<Node2D>(ref _parent));
		info.AddProperty(PropertyName._globalPlowTarget, Variant.From<Vector2>(ref _globalPlowTarget));
		info.AddProperty(PropertyName._globalPlowEndTarget, Variant.From<Vector2>(ref _globalPlowEndTarget));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._deathParticles, ref val))
		{
			_deathParticles = ((Variant)(ref val)).As<GpuParticles2D>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._energyParticlesFront, ref val2))
		{
			_energyParticlesFront = ((Variant)(ref val2)).As<CpuParticles2D>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._energyParticlesBack, ref val3))
		{
			_energyParticlesBack = ((Variant)(ref val3)).As<CpuParticles2D>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._plowStartTarget, ref val4))
		{
			_plowStartTarget = ((Variant)(ref val4)).As<Node2D>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._plowEndTarget, ref val5))
		{
			_plowEndTarget = ((Variant)(ref val5)).As<Node2D>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._parent, ref val6))
		{
			_parent = ((Variant)(ref val6)).As<Node2D>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName._globalPlowTarget, ref val7))
		{
			_globalPlowTarget = ((Variant)(ref val7)).As<Vector2>();
		}
		Variant val8 = default(Variant);
		if (info.TryGetProperty(PropertyName._globalPlowEndTarget, ref val8))
		{
			_globalPlowEndTarget = ((Variant)(ref val8)).As<Vector2>();
		}
	}
}
