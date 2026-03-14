using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.TestSupport;

namespace MegaCrit.Sts2.Core.Nodes.Vfx;

[ScriptPath("res://src/Core/Nodes/Vfx/NRelicFlashVfx.cs")]
public class NRelicFlashVfx : Control
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName _ExitTree = StringName.op_Implicit("_ExitTree");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName _sprite = StringName.op_Implicit("_sprite");

		public static readonly StringName _sprite2 = StringName.op_Implicit("_sprite2");

		public static readonly StringName _sprite3 = StringName.op_Implicit("_sprite3");

		public static readonly StringName _tween = StringName.op_Implicit("_tween");
	}

	public class SignalName : SignalName
	{
	}

	public const float activationDuration = 1f;

	private const string _scenePath = "res://scenes/vfx/relic_flash_vfx.tscn";

	private TextureRect _sprite;

	private TextureRect _sprite2;

	private TextureRect _sprite3;

	private static readonly Vector2 _targetScale = Vector2.One * 1.25f;

	private RelicModel? _relic;

	private Creature? _target;

	private Tween? _tween;

	public static IEnumerable<string> AssetPaths => new _003C_003Ez__ReadOnlySingleElementList<string>("res://scenes/vfx/relic_flash_vfx.tscn");

	public static NRelicFlashVfx? Create(RelicModel relic)
	{
		if (TestMode.IsOn)
		{
			return null;
		}
		NRelicFlashVfx nRelicFlashVfx = PreloadManager.Cache.GetScene("res://scenes/vfx/relic_flash_vfx.tscn").Instantiate<NRelicFlashVfx>((GenEditState)0);
		nRelicFlashVfx._relic = relic;
		return nRelicFlashVfx;
	}

	public static NRelicFlashVfx? Create(RelicModel relic, Creature target)
	{
		NRelicFlashVfx nRelicFlashVfx = Create(relic);
		if (nRelicFlashVfx == null)
		{
			return null;
		}
		nRelicFlashVfx._target = target;
		return nRelicFlashVfx;
	}

	public override void _Ready()
	{
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		_sprite = ((Node)this).GetNode<TextureRect>(NodePath.op_Implicit("Image1"));
		_sprite2 = ((Node)this).GetNode<TextureRect>(NodePath.op_Implicit("Image2"));
		_sprite3 = ((Node)this).GetNode<TextureRect>(NodePath.op_Implicit("Image3"));
		if (_target != null)
		{
			((Control)this).GlobalPosition = NCombatRoom.Instance.GetCreatureNode(_target).GetTopOfHitbox();
		}
		TaskHelper.RunSafely(StartVfx());
	}

	public override void _ExitTree()
	{
		Tween? tween = _tween;
		if (tween != null)
		{
			tween.Kill();
		}
	}

	private async Task StartVfx()
	{
		_sprite.Texture = _relic.Icon;
		_sprite2.Texture = _relic.Icon;
		_sprite3.Texture = _relic.Icon;
		_tween = ((Node)this).CreateTween().SetParallel(true);
		if (_target != null)
		{
			((Control)this).Position = ((Control)this).Position + new Vector2(0f, 64f);
			_tween.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("position:y"), Variant.op_Implicit(((Control)this).Position.Y - 64f), 1.0).SetEase((EaseType)1).SetTrans((TransitionType)5);
		}
		_tween.TweenProperty((GodotObject)(object)_sprite, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(1f), 0.01);
		_tween.TweenProperty((GodotObject)(object)_sprite, NodePath.op_Implicit("scale"), Variant.op_Implicit(_targetScale), 1.0).SetEase((EaseType)1).SetTrans((TransitionType)7);
		_tween.TweenProperty((GodotObject)(object)_sprite, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0f), 1.5).SetDelay(0.01);
		_tween.TweenProperty((GodotObject)(object)_sprite2, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(1f), 0.01).SetDelay(0.2);
		_tween.TweenProperty((GodotObject)(object)_sprite2, NodePath.op_Implicit("scale"), Variant.op_Implicit(_targetScale), 1.0).SetEase((EaseType)1).SetTrans((TransitionType)7)
			.SetDelay(0.2);
		_tween.TweenProperty((GodotObject)(object)_sprite2, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0f), 1.5).SetDelay(0.21);
		_tween.TweenProperty((GodotObject)(object)_sprite3, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(1f), 0.01).SetDelay(0.4);
		_tween.TweenProperty((GodotObject)(object)_sprite3, NodePath.op_Implicit("scale"), Variant.op_Implicit(_targetScale), 1.0).SetEase((EaseType)1).SetTrans((TransitionType)7)
			.SetDelay(0.4);
		_tween.TweenProperty((GodotObject)(object)_sprite3, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0f), 1.5).SetDelay(0.41);
		await ((GodotObject)this).ToSignal((GodotObject)(object)_tween, SignalName.Finished);
		((Node)(object)this).QueueFreeSafely();
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
		return ((Control)this).InvokeGodotClassMethod(ref method, args, ref ret);
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
		return ((Control)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._sprite)
		{
			_sprite = VariantUtils.ConvertTo<TextureRect>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._sprite2)
		{
			_sprite2 = VariantUtils.ConvertTo<TextureRect>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._sprite3)
		{
			_sprite3 = VariantUtils.ConvertTo<TextureRect>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._tween)
		{
			_tween = VariantUtils.ConvertTo<Tween>(ref value);
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
		if ((ref name) == PropertyName._sprite)
		{
			value = VariantUtils.CreateFrom<TextureRect>(ref _sprite);
			return true;
		}
		if ((ref name) == PropertyName._sprite2)
		{
			value = VariantUtils.CreateFrom<TextureRect>(ref _sprite2);
			return true;
		}
		if ((ref name) == PropertyName._sprite3)
		{
			value = VariantUtils.CreateFrom<TextureRect>(ref _sprite3);
			return true;
		}
		if ((ref name) == PropertyName._tween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _tween);
			return true;
		}
		return ((GodotObject)this).GetGodotClassPropertyValue(ref name, ref value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._sprite, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._sprite2, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._sprite3, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._tween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		info.AddProperty(PropertyName._sprite, Variant.From<TextureRect>(ref _sprite));
		info.AddProperty(PropertyName._sprite2, Variant.From<TextureRect>(ref _sprite2));
		info.AddProperty(PropertyName._sprite3, Variant.From<TextureRect>(ref _sprite3));
		info.AddProperty(PropertyName._tween, Variant.From<Tween>(ref _tween));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._sprite, ref val))
		{
			_sprite = ((Variant)(ref val)).As<TextureRect>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._sprite2, ref val2))
		{
			_sprite2 = ((Variant)(ref val2)).As<TextureRect>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._sprite3, ref val3))
		{
			_sprite3 = ((Variant)(ref val3)).As<TextureRect>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._tween, ref val4))
		{
			_tween = ((Variant)(ref val4)).As<Tween>();
		}
	}
}
