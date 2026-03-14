using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.Collections;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models.Characters;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx.Utilities;
using MegaCrit.Sts2.Core.TestSupport;

namespace MegaCrit.Sts2.Core.Nodes.Vfx;

[ScriptPath("res://src/Core/Nodes/Vfx/NSweepingBeamVfx.cs")]
public class NSweepingBeamVfx : Node2D
{
	public class MethodName : MethodName
	{
		public static readonly StringName Create = StringName.op_Implicit("Create");

		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName _emittingParticles = StringName.op_Implicit("_emittingParticles");

		public static readonly StringName _startParticles = StringName.op_Implicit("_startParticles");

		public static readonly StringName _endParticles = StringName.op_Implicit("_endParticles");

		public static readonly StringName _sweepingParticles = StringName.op_Implicit("_sweepingParticles");

		public static readonly StringName _sweepingIndexCurve = StringName.op_Implicit("_sweepingIndexCurve");

		public static readonly StringName _sweepDuration = StringName.op_Implicit("_sweepDuration");

		public static readonly StringName _targetCenterPositions = StringName.op_Implicit("_targetCenterPositions");
	}

	public class SignalName : SignalName
	{
	}

	public static readonly string scenePath = SceneHelper.GetScenePath("vfx/vfx_sweeping_beam");

	[Export(/*Could not decode attribute arguments.*/)]
	private Array<GpuParticles2D> _emittingParticles = new Array<GpuParticles2D>();

	[Export(/*Could not decode attribute arguments.*/)]
	private Array<GpuParticles2D> _startParticles = new Array<GpuParticles2D>();

	[Export(/*Could not decode attribute arguments.*/)]
	private Array<GpuParticles2D> _endParticles = new Array<GpuParticles2D>();

	[Export(/*Could not decode attribute arguments.*/)]
	private Array<GpuParticles2D> _sweepingParticles = new Array<GpuParticles2D>();

	[Export(/*Could not decode attribute arguments.*/)]
	private Curve? _sweepingIndexCurve;

	[Export(/*Could not decode attribute arguments.*/)]
	private float _sweepDuration = 0.65f;

	private Array<Vector2> _targetCenterPositions = new Array<Vector2>();

	public static NSweepingBeamVfx? Create(Creature owner, List<Creature> targets)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		if (TestMode.IsOn)
		{
			return null;
		}
		NCreature nCreature = NCombatRoom.Instance?.GetCreatureNode(owner);
		if (nCreature != null)
		{
			Vector2 val = nCreature.VfxSpawnPosition;
			Player player = owner.Player;
			if (player != null && player.Character is Defect defect)
			{
				val += defect.EyelineOffset;
			}
			Array<Vector2> val2 = new Array<Vector2>();
			foreach (Creature target in targets)
			{
				NCreature nCreature2 = NCombatRoom.Instance?.GetCreatureNode(target);
				if (nCreature2 != null)
				{
					val2.Add(nCreature2.VfxSpawnPosition);
				}
			}
			return Create(val, val2);
		}
		return null;
	}

	public static NSweepingBeamVfx? Create(Vector2 defectEyeCenter, Array<Vector2> targetCenterPositions)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		if (TestMode.IsOn)
		{
			return null;
		}
		NSweepingBeamVfx nSweepingBeamVfx = PreloadManager.Cache.GetScene(scenePath).Instantiate<NSweepingBeamVfx>((GenEditState)0);
		((Node2D)nSweepingBeamVfx).GlobalPosition = defectEyeCenter;
		nSweepingBeamVfx._targetCenterPositions = targetCenterPositions;
		return nSweepingBeamVfx;
	}

	public override void _Ready()
	{
		for (int i = 0; i < _emittingParticles.Count; i++)
		{
			_emittingParticles[i].Emitting = false;
		}
		TaskHelper.RunSafely(PlaySequence());
	}

	private async Task PlaySequence()
	{
		double timer = 0.0;
		bool playedImpactParticles = false;
		for (int i = 0; i < _startParticles.Count; i++)
		{
			_startParticles[i].Restart();
		}
		for (int j = 0; j < _emittingParticles.Count; j++)
		{
			_emittingParticles[j].Restart();
			_emittingParticles[j].Emitting = true;
		}
		int previousSweepIndex = -1;
		while (timer < (double)_sweepDuration)
		{
			double processDeltaTime = ((Node)this).GetProcessDeltaTime();
			float num = (float)(timer / (double)_sweepDuration);
			int num2 = Mathf.FloorToInt(_sweepingIndexCurve.Sample(num));
			if (previousSweepIndex != num2 && num2 >= 0 && num2 < _sweepingParticles.Count)
			{
				_sweepingParticles[num2].Restart();
				previousSweepIndex = num2;
			}
			if (num >= 0.5f && !playedImpactParticles)
			{
				playedImpactParticles = true;
				NGame.Instance?.ScreenShake(ShakeStrength.Medium, ShakeDuration.Normal);
				for (int k = 0; k < _targetCenterPositions.Count; k++)
				{
					NSweepingBeamImpactVfx child = NSweepingBeamImpactVfx.Create(_targetCenterPositions[k]);
					((Node)(object)((Node)this).GetTree().Root).AddChildSafely((Node?)(object)child);
				}
			}
			timer += processDeltaTime;
			await ((GodotObject)this).ToSignal((GodotObject)(object)((Node)this).GetTree(), SignalName.ProcessFrame);
		}
		for (int l = 0; l < _endParticles.Count; l++)
		{
			_endParticles[l].Restart();
		}
		for (int m = 0; m < _emittingParticles.Count; m++)
		{
			_emittingParticles[m].Emitting = false;
		}
		await Cmd.Wait(2f);
		((Node)(object)this).QueueFreeSafely();
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Expected O, but got Unknown
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(2);
		list.Add(new MethodInfo(MethodName.Create, new PropertyInfo((Type)24, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Node2D"), false), (MethodFlags)33, new List<PropertyInfo>
		{
			new PropertyInfo((Type)5, StringName.op_Implicit("defectEyeCenter"), (PropertyHint)0, "", (PropertyUsageFlags)6, false),
			new PropertyInfo((Type)28, StringName.op_Implicit("targetCenterPositions"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.Create && ((NativeVariantPtrArgs)(ref args)).Count == 2)
		{
			NSweepingBeamVfx nSweepingBeamVfx = Create(VariantUtils.ConvertTo<Vector2>(ref ((NativeVariantPtrArgs)(ref args))[0]), VariantUtils.ConvertToArray<Vector2>(ref ((NativeVariantPtrArgs)(ref args))[1]));
			ret = VariantUtils.CreateFrom<NSweepingBeamVfx>(ref nSweepingBeamVfx);
			return true;
		}
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		return ((Node2D)this).InvokeGodotClassMethod(ref method, args, ref ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static bool InvokeGodotClassStaticMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.Create && ((NativeVariantPtrArgs)(ref args)).Count == 2)
		{
			NSweepingBeamVfx nSweepingBeamVfx = Create(VariantUtils.ConvertTo<Vector2>(ref ((NativeVariantPtrArgs)(ref args))[0]), VariantUtils.ConvertToArray<Vector2>(ref ((NativeVariantPtrArgs)(ref args))[1]));
			ret = VariantUtils.CreateFrom<NSweepingBeamVfx>(ref nSweepingBeamVfx);
			return true;
		}
		ret = default(godot_variant);
		return false;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if ((ref method) == MethodName.Create)
		{
			return true;
		}
		if ((ref method) == MethodName._Ready)
		{
			return true;
		}
		return ((Node2D)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._emittingParticles)
		{
			_emittingParticles = VariantUtils.ConvertToArray<GpuParticles2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._startParticles)
		{
			_startParticles = VariantUtils.ConvertToArray<GpuParticles2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._endParticles)
		{
			_endParticles = VariantUtils.ConvertToArray<GpuParticles2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._sweepingParticles)
		{
			_sweepingParticles = VariantUtils.ConvertToArray<GpuParticles2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._sweepingIndexCurve)
		{
			_sweepingIndexCurve = VariantUtils.ConvertTo<Curve>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._sweepDuration)
		{
			_sweepDuration = VariantUtils.ConvertTo<float>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._targetCenterPositions)
		{
			_targetCenterPositions = VariantUtils.ConvertToArray<Vector2>(ref value);
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
		if ((ref name) == PropertyName._emittingParticles)
		{
			value = VariantUtils.CreateFromArray<GpuParticles2D>(_emittingParticles);
			return true;
		}
		if ((ref name) == PropertyName._startParticles)
		{
			value = VariantUtils.CreateFromArray<GpuParticles2D>(_startParticles);
			return true;
		}
		if ((ref name) == PropertyName._endParticles)
		{
			value = VariantUtils.CreateFromArray<GpuParticles2D>(_endParticles);
			return true;
		}
		if ((ref name) == PropertyName._sweepingParticles)
		{
			value = VariantUtils.CreateFromArray<GpuParticles2D>(_sweepingParticles);
			return true;
		}
		if ((ref name) == PropertyName._sweepingIndexCurve)
		{
			value = VariantUtils.CreateFrom<Curve>(ref _sweepingIndexCurve);
			return true;
		}
		if ((ref name) == PropertyName._sweepDuration)
		{
			value = VariantUtils.CreateFrom<float>(ref _sweepDuration);
			return true;
		}
		if ((ref name) == PropertyName._targetCenterPositions)
		{
			value = VariantUtils.CreateFromArray<Vector2>(_targetCenterPositions);
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
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)28, PropertyName._emittingParticles, (PropertyHint)23, "24/34:GPUParticles2D", (PropertyUsageFlags)4102, true));
		list.Add(new PropertyInfo((Type)28, PropertyName._startParticles, (PropertyHint)23, "24/34:GPUParticles2D", (PropertyUsageFlags)4102, true));
		list.Add(new PropertyInfo((Type)28, PropertyName._endParticles, (PropertyHint)23, "24/34:GPUParticles2D", (PropertyUsageFlags)4102, true));
		list.Add(new PropertyInfo((Type)28, PropertyName._sweepingParticles, (PropertyHint)23, "24/34:GPUParticles2D", (PropertyUsageFlags)4102, true));
		list.Add(new PropertyInfo((Type)24, PropertyName._sweepingIndexCurve, (PropertyHint)17, "Curve", (PropertyUsageFlags)4102, true));
		list.Add(new PropertyInfo((Type)3, PropertyName._sweepDuration, (PropertyHint)0, "", (PropertyUsageFlags)4102, true));
		list.Add(new PropertyInfo((Type)28, PropertyName._targetCenterPositions, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		((GodotObject)this).SaveGodotObjectData(info);
		info.AddProperty(PropertyName._emittingParticles, Variant.CreateFrom<GpuParticles2D>(_emittingParticles));
		info.AddProperty(PropertyName._startParticles, Variant.CreateFrom<GpuParticles2D>(_startParticles));
		info.AddProperty(PropertyName._endParticles, Variant.CreateFrom<GpuParticles2D>(_endParticles));
		info.AddProperty(PropertyName._sweepingParticles, Variant.CreateFrom<GpuParticles2D>(_sweepingParticles));
		info.AddProperty(PropertyName._sweepingIndexCurve, Variant.From<Curve>(ref _sweepingIndexCurve));
		info.AddProperty(PropertyName._sweepDuration, Variant.From<float>(ref _sweepDuration));
		info.AddProperty(PropertyName._targetCenterPositions, Variant.CreateFrom<Vector2>(_targetCenterPositions));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._emittingParticles, ref val))
		{
			_emittingParticles = ((Variant)(ref val)).AsGodotArray<GpuParticles2D>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._startParticles, ref val2))
		{
			_startParticles = ((Variant)(ref val2)).AsGodotArray<GpuParticles2D>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._endParticles, ref val3))
		{
			_endParticles = ((Variant)(ref val3)).AsGodotArray<GpuParticles2D>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._sweepingParticles, ref val4))
		{
			_sweepingParticles = ((Variant)(ref val4)).AsGodotArray<GpuParticles2D>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._sweepingIndexCurve, ref val5))
		{
			_sweepingIndexCurve = ((Variant)(ref val5)).As<Curve>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._sweepDuration, ref val6))
		{
			_sweepDuration = ((Variant)(ref val6)).As<float>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName._targetCenterPositions, ref val7))
		{
			_targetCenterPositions = ((Variant)(ref val7)).AsGodotArray<Vector2>();
		}
	}
}
