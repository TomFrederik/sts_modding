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

[ScriptPath("res://src/Core/Nodes/Vfx/NHyperbeamVfx.cs")]
public class NHyperbeamVfx : Node2D
{
	public class MethodName : MethodName
	{
		public static readonly StringName Create = StringName.op_Implicit("Create");

		public static readonly StringName ApplyRotation = StringName.op_Implicit("ApplyRotation");

		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName ShowLaser = StringName.op_Implicit("ShowLaser");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName _anticipationParticles = StringName.op_Implicit("_anticipationParticles");

		public static readonly StringName _laserParticles = StringName.op_Implicit("_laserParticles");

		public static readonly StringName _laserEndParticles = StringName.op_Implicit("_laserEndParticles");

		public static readonly StringName _laserLine = StringName.op_Implicit("_laserLine");

		public static readonly StringName _laserContainer = StringName.op_Implicit("_laserContainer");
	}

	public class SignalName : SignalName
	{
	}

	public static readonly string scenePath = SceneHelper.GetScenePath("vfx/vfx_hyperbeam");

	[Export(/*Could not decode attribute arguments.*/)]
	private Array<GpuParticles2D> _anticipationParticles = new Array<GpuParticles2D>();

	[Export(/*Could not decode attribute arguments.*/)]
	private Array<GpuParticles2D> _laserParticles = new Array<GpuParticles2D>();

	[Export(/*Could not decode attribute arguments.*/)]
	private Array<GpuParticles2D> _laserEndParticles = new Array<GpuParticles2D>();

	[Export(/*Could not decode attribute arguments.*/)]
	private Line2D? _laserLine;

	[Export(/*Could not decode attribute arguments.*/)]
	private Node2D? _laserContainer;

	public static readonly float hyperbeamAnticipationDuration = 0.525f;

	public static readonly float hyperbeamLaserDuration = 0.5f;

	public static NHyperbeamVfx? Create(Creature owner, Creature target)
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		if (TestMode.IsOn)
		{
			return null;
		}
		NCreature nCreature = NCombatRoom.Instance?.GetCreatureNode(owner);
		NCreature nCreature2 = NCombatRoom.Instance?.GetCreatureNode(target);
		if (nCreature2 != null && nCreature != null)
		{
			Vector2 val = nCreature.VfxSpawnPosition;
			Player player = owner.Player;
			if (player != null && player.Character is Defect defect)
			{
				val += defect.EyelineOffset;
			}
			return Create(val, nCreature2.VfxSpawnPosition);
		}
		return null;
	}

	public static NHyperbeamVfx? Create(Vector2 defectEyePosition, Vector2 mainTargetCenterPosition)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		if (TestMode.IsOn)
		{
			return null;
		}
		NHyperbeamVfx nHyperbeamVfx = PreloadManager.Cache.GetScene(scenePath).Instantiate<NHyperbeamVfx>((GenEditState)0);
		((Node2D)nHyperbeamVfx).GlobalPosition = defectEyePosition;
		nHyperbeamVfx.ApplyRotation(defectEyePosition, mainTargetCenterPosition);
		return nHyperbeamVfx;
	}

	public void ApplyRotation(Vector2 sourcePosition, Vector2 targetPosition)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		Vector2 val = targetPosition - sourcePosition;
		float rotationDegrees = Mathf.RadToDeg(Mathf.Atan2(val.Y, val.X));
		((Node2D)this).RotationDegrees = rotationDegrees;
	}

	public override void _Ready()
	{
		TaskHelper.RunSafely(PlaySequence());
	}

	private void ShowLaser(bool showing)
	{
		for (int i = 0; i < _laserParticles.Count; i++)
		{
			((CanvasItem)_laserParticles[i]).Visible = showing;
			if (showing)
			{
				_laserParticles[i].Restart();
			}
		}
		((CanvasItem)_laserLine).Visible = showing;
		((CanvasItem)_laserContainer).Visible = showing;
	}

	private async Task PlaySequence()
	{
		ShowLaser(showing: false);
		for (int i = 0; i < _anticipationParticles.Count; i++)
		{
			_anticipationParticles[i].Restart();
		}
		await Cmd.Wait(hyperbeamAnticipationDuration);
		ShowLaser(showing: true);
		NGame.Instance?.ScreenShake(ShakeStrength.Medium, ShakeDuration.Normal);
		await Cmd.Wait(hyperbeamLaserDuration);
		ShowLaser(showing: false);
		for (int j = 0; j < _laserEndParticles.Count; j++)
		{
			_laserEndParticles[j].Restart();
		}
		NGame.Instance?.ScreenShake(ShakeStrength.Strong, ShakeDuration.Short);
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
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(4);
		list.Add(new MethodInfo(MethodName.Create, new PropertyInfo((Type)24, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Node2D"), false), (MethodFlags)33, new List<PropertyInfo>
		{
			new PropertyInfo((Type)5, StringName.op_Implicit("defectEyePosition"), (PropertyHint)0, "", (PropertyUsageFlags)6, false),
			new PropertyInfo((Type)5, StringName.op_Implicit("mainTargetCenterPosition"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ApplyRotation, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)5, StringName.op_Implicit("sourcePosition"), (PropertyHint)0, "", (PropertyUsageFlags)6, false),
			new PropertyInfo((Type)5, StringName.op_Implicit("targetPosition"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ShowLaser, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)1, StringName.op_Implicit("showing"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.Create && ((NativeVariantPtrArgs)(ref args)).Count == 2)
		{
			NHyperbeamVfx nHyperbeamVfx = Create(VariantUtils.ConvertTo<Vector2>(ref ((NativeVariantPtrArgs)(ref args))[0]), VariantUtils.ConvertTo<Vector2>(ref ((NativeVariantPtrArgs)(ref args))[1]));
			ret = VariantUtils.CreateFrom<NHyperbeamVfx>(ref nHyperbeamVfx);
			return true;
		}
		if ((ref method) == MethodName.ApplyRotation && ((NativeVariantPtrArgs)(ref args)).Count == 2)
		{
			ApplyRotation(VariantUtils.ConvertTo<Vector2>(ref ((NativeVariantPtrArgs)(ref args))[0]), VariantUtils.ConvertTo<Vector2>(ref ((NativeVariantPtrArgs)(ref args))[1]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.ShowLaser && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			ShowLaser(VariantUtils.ConvertTo<bool>(ref ((NativeVariantPtrArgs)(ref args))[0]));
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
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.Create && ((NativeVariantPtrArgs)(ref args)).Count == 2)
		{
			NHyperbeamVfx nHyperbeamVfx = Create(VariantUtils.ConvertTo<Vector2>(ref ((NativeVariantPtrArgs)(ref args))[0]), VariantUtils.ConvertTo<Vector2>(ref ((NativeVariantPtrArgs)(ref args))[1]));
			ret = VariantUtils.CreateFrom<NHyperbeamVfx>(ref nHyperbeamVfx);
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
		if ((ref method) == MethodName.ApplyRotation)
		{
			return true;
		}
		if ((ref method) == MethodName._Ready)
		{
			return true;
		}
		if ((ref method) == MethodName.ShowLaser)
		{
			return true;
		}
		return ((Node2D)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._anticipationParticles)
		{
			_anticipationParticles = VariantUtils.ConvertToArray<GpuParticles2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._laserParticles)
		{
			_laserParticles = VariantUtils.ConvertToArray<GpuParticles2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._laserEndParticles)
		{
			_laserEndParticles = VariantUtils.ConvertToArray<GpuParticles2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._laserLine)
		{
			_laserLine = VariantUtils.ConvertTo<Line2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._laserContainer)
		{
			_laserContainer = VariantUtils.ConvertTo<Node2D>(ref value);
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
		if ((ref name) == PropertyName._anticipationParticles)
		{
			value = VariantUtils.CreateFromArray<GpuParticles2D>(_anticipationParticles);
			return true;
		}
		if ((ref name) == PropertyName._laserParticles)
		{
			value = VariantUtils.CreateFromArray<GpuParticles2D>(_laserParticles);
			return true;
		}
		if ((ref name) == PropertyName._laserEndParticles)
		{
			value = VariantUtils.CreateFromArray<GpuParticles2D>(_laserEndParticles);
			return true;
		}
		if ((ref name) == PropertyName._laserLine)
		{
			value = VariantUtils.CreateFrom<Line2D>(ref _laserLine);
			return true;
		}
		if ((ref name) == PropertyName._laserContainer)
		{
			value = VariantUtils.CreateFrom<Node2D>(ref _laserContainer);
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
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)28, PropertyName._anticipationParticles, (PropertyHint)23, "24/34:GPUParticles2D", (PropertyUsageFlags)4102, true));
		list.Add(new PropertyInfo((Type)28, PropertyName._laserParticles, (PropertyHint)23, "24/34:GPUParticles2D", (PropertyUsageFlags)4102, true));
		list.Add(new PropertyInfo((Type)28, PropertyName._laserEndParticles, (PropertyHint)23, "24/34:GPUParticles2D", (PropertyUsageFlags)4102, true));
		list.Add(new PropertyInfo((Type)24, PropertyName._laserLine, (PropertyHint)34, "Line2D", (PropertyUsageFlags)4102, true));
		list.Add(new PropertyInfo((Type)24, PropertyName._laserContainer, (PropertyHint)34, "Node2D", (PropertyUsageFlags)4102, true));
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
		info.AddProperty(PropertyName._anticipationParticles, Variant.CreateFrom<GpuParticles2D>(_anticipationParticles));
		info.AddProperty(PropertyName._laserParticles, Variant.CreateFrom<GpuParticles2D>(_laserParticles));
		info.AddProperty(PropertyName._laserEndParticles, Variant.CreateFrom<GpuParticles2D>(_laserEndParticles));
		info.AddProperty(PropertyName._laserLine, Variant.From<Line2D>(ref _laserLine));
		info.AddProperty(PropertyName._laserContainer, Variant.From<Node2D>(ref _laserContainer));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._anticipationParticles, ref val))
		{
			_anticipationParticles = ((Variant)(ref val)).AsGodotArray<GpuParticles2D>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._laserParticles, ref val2))
		{
			_laserParticles = ((Variant)(ref val2)).AsGodotArray<GpuParticles2D>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._laserEndParticles, ref val3))
		{
			_laserEndParticles = ((Variant)(ref val3)).AsGodotArray<GpuParticles2D>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._laserLine, ref val4))
		{
			_laserLine = ((Variant)(ref val4)).As<Line2D>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._laserContainer, ref val5))
		{
			_laserContainer = ((Variant)(ref val5)).As<Node2D>();
		}
	}
}
