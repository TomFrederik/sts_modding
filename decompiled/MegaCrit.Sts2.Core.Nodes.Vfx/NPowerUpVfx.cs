using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace MegaCrit.Sts2.Core.Nodes.Vfx;

[ScriptPath("res://src/Core/Nodes/Vfx/NPowerUpVfx.cs")]
public class NPowerUpVfx : Node2D
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName _Process = StringName.op_Implicit("_Process");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName _timer = StringName.op_Implicit("_timer");

		public static readonly StringName _creatureVisuals = StringName.op_Implicit("_creatureVisuals");

		public static readonly StringName _backVfx = StringName.op_Implicit("_backVfx");
	}

	public class SignalName : SignalName
	{
	}

	private float _timer;

	private const float _vfxDuration = 1f;

	private Control _creatureVisuals;

	private Sprite2D _backVfx;

	private static string NormalScenePath => SceneHelper.GetScenePath("/vfx/vfx_power_up/vfx_power_up");

	private static string GhostlyScenePath => SceneHelper.GetScenePath("/vfx/vfx_ghostly_power_up/vfx_ghostly_power_up");

	public static IEnumerable<string> AssetPaths => new global::_003C_003Ez__ReadOnlyArray<string>(new string[2] { NormalScenePath, GhostlyScenePath });

	public static NPowerUpVfx? CreateNormal(Creature target)
	{
		return CreatePowerUpVfx(target, NormalScenePath);
	}

	public static NPowerUpVfx? CreateGhostly(Creature target)
	{
		return CreatePowerUpVfx(target, GhostlyScenePath);
	}

	private static NPowerUpVfx? CreatePowerUpVfx(Creature target, string scenePath)
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		NCreature nCreature = NCombatRoom.Instance?.GetCreatureNode(target);
		if (nCreature == null || !nCreature.IsInteractable)
		{
			return null;
		}
		NPowerUpVfx nPowerUpVfx = PreloadManager.Cache.GetScene(scenePath).Instantiate<NPowerUpVfx>((GenEditState)0);
		((Node2D)nPowerUpVfx).GlobalPosition = nCreature.VfxSpawnPosition;
		((Node)(object)NCombatRoom.Instance.CombatVfxContainer).AddChildSafely((Node?)(object)nPowerUpVfx);
		return nPowerUpVfx;
	}

	public override void _Ready()
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		_timer = 1f;
		_backVfx = ((Node)this).GetNode<Sprite2D>(NodePath.op_Implicit("%BackVfx"));
		Vector2 globalPosition = ((Node2D)_backVfx).GlobalPosition;
		((Node)_backVfx).Reparent(NCombatRoom.Instance.BackCombatVfxContainer, true);
		((Node2D)_backVfx).GlobalPosition = globalPosition;
	}

	public override void _Process(double delta)
	{
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		_timer -= (float)delta;
		float num = 1f;
		if (Mathf.Abs(_timer / 1f - 0.5f) > 0.4f)
		{
			num = Mathf.Max(0f, 1f - (Mathf.Abs(_timer / 1f - 0.5f) - 0.4f) / 0.1f);
		}
		((CanvasItem)this).Modulate = new Color(1f, 1f, 1f, num);
		((CanvasItem)_backVfx).Modulate = new Color(1f, 1f, 1f, num);
		if (_timer < 0f)
		{
			((Node)(object)this).QueueFreeSafely();
			((Node)(object)_backVfx).QueueFreeSafely();
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(2);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Process, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)3, StringName.op_Implicit("delta"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._Process && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			((Node)this)._Process(VariantUtils.ConvertTo<double>(ref ((NativeVariantPtrArgs)(ref args))[0]));
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
		if ((ref method) == MethodName._Process)
		{
			return true;
		}
		return ((Node2D)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._timer)
		{
			_timer = VariantUtils.ConvertTo<float>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._creatureVisuals)
		{
			_creatureVisuals = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._backVfx)
		{
			_backVfx = VariantUtils.ConvertTo<Sprite2D>(ref value);
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
		if ((ref name) == PropertyName._timer)
		{
			value = VariantUtils.CreateFrom<float>(ref _timer);
			return true;
		}
		if ((ref name) == PropertyName._creatureVisuals)
		{
			value = VariantUtils.CreateFrom<Control>(ref _creatureVisuals);
			return true;
		}
		if ((ref name) == PropertyName._backVfx)
		{
			value = VariantUtils.CreateFrom<Sprite2D>(ref _backVfx);
			return true;
		}
		return ((GodotObject)this).GetGodotClassPropertyValue(ref name, ref value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)3, PropertyName._timer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._creatureVisuals, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._backVfx, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		info.AddProperty(PropertyName._timer, Variant.From<float>(ref _timer));
		info.AddProperty(PropertyName._creatureVisuals, Variant.From<Control>(ref _creatureVisuals));
		info.AddProperty(PropertyName._backVfx, Variant.From<Sprite2D>(ref _backVfx));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._timer, ref val))
		{
			_timer = ((Variant)(ref val)).As<float>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._creatureVisuals, ref val2))
		{
			_creatureVisuals = ((Variant)(ref val2)).As<Control>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._backVfx, ref val3))
		{
			_backVfx = ((Variant)(ref val3)).As<Sprite2D>();
		}
	}
}
