using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.Collections;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace MegaCrit.Sts2.Core.Nodes.Vfx;

[ScriptPath("res://src/Core/Nodes/Vfx/NBlockSparkVfx.cs")]
public class NBlockSparkVfx : Node2D
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName _particles = StringName.op_Implicit("_particles");

		public static readonly StringName _specks = StringName.op_Implicit("_specks");

		public static readonly StringName _creatureNode = StringName.op_Implicit("_creatureNode");
	}

	public class SignalName : SignalName
	{
	}

	[Export(/*Could not decode attribute arguments.*/)]
	private Array<GpuParticles2D> _particles = new Array<GpuParticles2D>();

	[Export(/*Could not decode attribute arguments.*/)]
	private GpuParticles2D _specks;

	private NCreature _creatureNode;

	private static string ScenePath => SceneHelper.GetScenePath("vfx/block_spark_vfx");

	public static IEnumerable<string> AssetPaths => new _003C_003Ez__ReadOnlySingleElementList<string>(ScenePath);

	public static NBlockSparkVfx? Create(Creature target)
	{
		NCreature nCreature = NCombatRoom.Instance?.GetCreatureNode(target);
		if (nCreature == null || !nCreature.IsInteractable)
		{
			return null;
		}
		NBlockSparkVfx nBlockSparkVfx = PreloadManager.Cache.GetScene(ScenePath).Instantiate<NBlockSparkVfx>((GenEditState)0);
		nBlockSparkVfx._creatureNode = nCreature;
		return nBlockSparkVfx;
	}

	public override void _Ready()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		((Node2D)this).GlobalPosition = _creatureNode.VfxSpawnPosition;
		for (int i = 0; i < _particles.Count; i++)
		{
			_particles[i].Restart();
		}
		TaskHelper.RunSafely(FlashAndFree());
	}

	private async Task FlashAndFree()
	{
		await ((GodotObject)this).ToSignal((GodotObject)(object)_specks, SignalName.Finished);
		((Node)(object)this).QueueFreeSafely();
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(1);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
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
		return ((Node2D)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._particles)
		{
			_particles = VariantUtils.ConvertToArray<GpuParticles2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._specks)
		{
			_specks = VariantUtils.ConvertTo<GpuParticles2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._creatureNode)
		{
			_creatureNode = VariantUtils.ConvertTo<NCreature>(ref value);
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
		if ((ref name) == PropertyName._particles)
		{
			value = VariantUtils.CreateFromArray<GpuParticles2D>(_particles);
			return true;
		}
		if ((ref name) == PropertyName._specks)
		{
			value = VariantUtils.CreateFrom<GpuParticles2D>(ref _specks);
			return true;
		}
		if ((ref name) == PropertyName._creatureNode)
		{
			value = VariantUtils.CreateFrom<NCreature>(ref _creatureNode);
			return true;
		}
		return ((GodotObject)this).GetGodotClassPropertyValue(ref name, ref value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)28, PropertyName._particles, (PropertyHint)23, "24/34:GPUParticles2D", (PropertyUsageFlags)4102, true));
		list.Add(new PropertyInfo((Type)24, PropertyName._specks, (PropertyHint)34, "GPUParticles2D", (PropertyUsageFlags)4102, true));
		list.Add(new PropertyInfo((Type)24, PropertyName._creatureNode, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		info.AddProperty(PropertyName._particles, Variant.CreateFrom<GpuParticles2D>(_particles));
		info.AddProperty(PropertyName._specks, Variant.From<GpuParticles2D>(ref _specks));
		info.AddProperty(PropertyName._creatureNode, Variant.From<NCreature>(ref _creatureNode));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._particles, ref val))
		{
			_particles = ((Variant)(ref val)).AsGodotArray<GpuParticles2D>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._specks, ref val2))
		{
			_specks = ((Variant)(ref val2)).As<GpuParticles2D>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._creatureNode, ref val3))
		{
			_creatureNode = ((Variant)(ref val3)).As<NCreature>();
		}
	}
}
