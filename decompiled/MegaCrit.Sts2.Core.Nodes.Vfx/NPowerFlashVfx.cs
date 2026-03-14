using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.TestSupport;

namespace MegaCrit.Sts2.Core.Nodes.Vfx;

[ScriptPath("res://src/Core/Nodes/Vfx/NPowerFlashVfx.cs")]
public class NPowerFlashVfx : Node2D
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName _ExitTree = StringName.op_Implicit("_ExitTree");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName _sprite = StringName.op_Implicit("_sprite");

		public static readonly StringName _spriteTween = StringName.op_Implicit("_spriteTween");
	}

	public class SignalName : SignalName
	{
	}

	private const string _scenePath = "res://scenes/vfx/power_flash_vfx.tscn";

	private Sprite2D _sprite;

	private PowerModel _power;

	private Tween? _spriteTween;

	public static IEnumerable<string> AssetPaths => new _003C_003Ez__ReadOnlySingleElementList<string>("res://scenes/vfx/power_flash_vfx.tscn");

	public override void _Ready()
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		NCreature nCreature = NCombatRoom.Instance?.GetCreatureNode(_power.Owner);
		if (nCreature == null)
		{
			((Node)(object)this).QueueFreeSafely();
			return;
		}
		_sprite = ((Node)this).GetNode<Sprite2D>(NodePath.op_Implicit("Sprite2D"));
		((Node2D)this).GlobalPosition = nCreature.VfxSpawnPosition;
		TaskHelper.RunSafely(StartVfx());
	}

	public override void _ExitTree()
	{
		Tween? spriteTween = _spriteTween;
		if (spriteTween != null)
		{
			spriteTween.Kill();
		}
	}

	private async Task StartVfx()
	{
		_sprite.Texture = _power.BigIcon;
		((CanvasItem)_sprite).Modulate = Colors.White;
		_spriteTween = ((Node)this).CreateTween();
		_spriteTween.SetParallel(true);
		_spriteTween.TweenProperty((GodotObject)(object)_sprite, NodePath.op_Implicit("scale"), Variant.op_Implicit(Vector2.One * 0.4f), 0.4);
		_spriteTween.TweenProperty((GodotObject)(object)_sprite, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(1), 0.25).SetEase((EaseType)1).SetTrans((TransitionType)1);
		_spriteTween.SetParallel(false);
		_spriteTween.TweenProperty((GodotObject)(object)_sprite, NodePath.op_Implicit("scale"), Variant.op_Implicit(Vector2.One * 0.45f), 0.4);
		_spriteTween.TweenProperty((GodotObject)(object)_sprite, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0), 0.25).SetEase((EaseType)0).SetTrans((TransitionType)1);
		await ((GodotObject)this).ToSignal((GodotObject)(object)_spriteTween, SignalName.Finished);
		((Node)(object)this).QueueFreeSafely();
	}

	public static NPowerFlashVfx? Create(PowerModel power)
	{
		if (TestMode.IsOn)
		{
			return null;
		}
		if (!power.ShouldPlayVfx)
		{
			return null;
		}
		NPowerFlashVfx nPowerFlashVfx = (NPowerFlashVfx)(object)PreloadManager.Cache.GetScene("res://scenes/vfx/power_flash_vfx.tscn").Instantiate((GenEditState)0);
		nPowerFlashVfx._power = power;
		return nPowerFlashVfx;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(2);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._ExitTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
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
		if ((ref method) == MethodName._ExitTree)
		{
			return true;
		}
		return ((Node2D)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._sprite)
		{
			_sprite = VariantUtils.ConvertTo<Sprite2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._spriteTween)
		{
			_spriteTween = VariantUtils.ConvertTo<Tween>(ref value);
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
		if ((ref name) == PropertyName._sprite)
		{
			value = VariantUtils.CreateFrom<Sprite2D>(ref _sprite);
			return true;
		}
		if ((ref name) == PropertyName._spriteTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _spriteTween);
			return true;
		}
		return ((GodotObject)this).GetGodotClassPropertyValue(ref name, ref value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._sprite, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._spriteTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		info.AddProperty(PropertyName._sprite, Variant.From<Sprite2D>(ref _sprite));
		info.AddProperty(PropertyName._spriteTween, Variant.From<Tween>(ref _spriteTween));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._sprite, ref val))
		{
			_sprite = ((Variant)(ref val)).As<Sprite2D>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._spriteTween, ref val2))
		{
			_spriteTween = ((Variant)(ref val2)).As<Tween>();
		}
	}
}
