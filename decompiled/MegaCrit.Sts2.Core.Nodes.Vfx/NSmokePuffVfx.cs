using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.TestSupport;

namespace MegaCrit.Sts2.Core.Nodes.Vfx;

[ScriptPath("res://src/Core/Nodes/Vfx/NSmokePuffVfx.cs")]
public class NSmokePuffVfx : Node2D
{
	public enum SmokePuffColor
	{
		Green,
		Purple
	}

	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName _clouds = StringName.op_Implicit("_clouds");

		public static readonly StringName _ember = StringName.op_Implicit("_ember");

		public static readonly StringName _color = StringName.op_Implicit("_color");
	}

	public class SignalName : SignalName
	{
	}

	private GpuParticles2D _clouds;

	private GpuParticles2D _ember;

	private SmokePuffColor _color;

	public static IEnumerable<string> AssetPaths => new _003C_003Ez__ReadOnlySingleElementList<string>(ScenePath);

	private static string ScenePath => SceneHelper.GetScenePath("vfx/vfx_smoke_puff");

	public static NSmokePuffVfx? Create(Creature target, SmokePuffColor puffColor)
	{
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		if (TestMode.IsOn)
		{
			return null;
		}
		NCreature creatureNode = NCombatRoom.Instance.GetCreatureNode(target);
		if (creatureNode == null)
		{
			Log.Warn($"Tried to spawn {"NSmokePuffVfx"} on creature {target} without node!");
			return null;
		}
		NSmokePuffVfx nSmokePuffVfx = PreloadManager.Cache.GetScene(ScenePath).Instantiate<NSmokePuffVfx>((GenEditState)0);
		nSmokePuffVfx._color = puffColor;
		((Node2D)nSmokePuffVfx).GlobalPosition = creatureNode.VfxSpawnPosition;
		return nSmokePuffVfx;
	}

	public override void _Ready()
	{
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Expected O, but got Unknown
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		_ember = ((Node)this).GetNode<GpuParticles2D>(NodePath.op_Implicit("Ember"));
		_clouds = ((Node)this).GetNode<GpuParticles2D>(NodePath.op_Implicit("Clouds"));
		_ember.Emitting = true;
		_clouds.Emitting = true;
		if (_color == SmokePuffColor.Purple)
		{
			ParticleProcessMaterial val = (ParticleProcessMaterial)_clouds.ProcessMaterial;
			val.HueVariationMin = -0.02f;
			val.HueVariationMax = 0.02f;
			val.Color = new Color("F6B1FF");
		}
		TaskHelper.RunSafely(DeleteAfterComplete());
	}

	private async Task DeleteAfterComplete()
	{
		await Task.Delay(2500);
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
		if ((ref name) == PropertyName._clouds)
		{
			_clouds = VariantUtils.ConvertTo<GpuParticles2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._ember)
		{
			_ember = VariantUtils.ConvertTo<GpuParticles2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._color)
		{
			_color = VariantUtils.ConvertTo<SmokePuffColor>(ref value);
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
		if ((ref name) == PropertyName._clouds)
		{
			value = VariantUtils.CreateFrom<GpuParticles2D>(ref _clouds);
			return true;
		}
		if ((ref name) == PropertyName._ember)
		{
			value = VariantUtils.CreateFrom<GpuParticles2D>(ref _ember);
			return true;
		}
		if ((ref name) == PropertyName._color)
		{
			value = VariantUtils.CreateFrom<SmokePuffColor>(ref _color);
			return true;
		}
		return ((GodotObject)this).GetGodotClassPropertyValue(ref name, ref value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._clouds, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._ember, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)2, PropertyName._color, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		info.AddProperty(PropertyName._clouds, Variant.From<GpuParticles2D>(ref _clouds));
		info.AddProperty(PropertyName._ember, Variant.From<GpuParticles2D>(ref _ember));
		info.AddProperty(PropertyName._color, Variant.From<SmokePuffColor>(ref _color));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._clouds, ref val))
		{
			_clouds = ((Variant)(ref val)).As<GpuParticles2D>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._ember, ref val2))
		{
			_ember = ((Variant)(ref val2)).As<GpuParticles2D>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._color, ref val3))
		{
			_color = ((Variant)(ref val3)).As<SmokePuffColor>();
		}
	}
}
