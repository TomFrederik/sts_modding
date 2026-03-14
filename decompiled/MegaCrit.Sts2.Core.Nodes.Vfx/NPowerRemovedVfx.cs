using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.TestSupport;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.Nodes.Vfx;

[ScriptPath("res://src/Core/Nodes/Vfx/NPowerRemovedVfx.cs")]
public class NPowerRemovedVfx : Node2D
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName _ExitTree = StringName.op_Implicit("_ExitTree");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName _sprite = StringName.op_Implicit("_sprite");

		public static readonly StringName _powerField = StringName.op_Implicit("_powerField");

		public static readonly StringName _vfxContainer = StringName.op_Implicit("_vfxContainer");

		public static readonly StringName _textTween = StringName.op_Implicit("_textTween");
	}

	public class SignalName : SignalName
	{
	}

	private const string _scenePath = "res://scenes/vfx/power_removed_vfx.tscn";

	private static LocString _wearsOffLoc = new LocString("vfx", "POWER_WEARS_OFF");

	private TextureRect _sprite;

	private MegaLabel _powerField;

	private Control _vfxContainer;

	private PowerModel _power;

	private Tween? _textTween;

	public static IEnumerable<string> AssetPaths => new _003C_003Ez__ReadOnlySingleElementList<string>("res://scenes/vfx/power_removed_vfx.tscn");

	public override void _Ready()
	{
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		NCreature nCreature = NCombatRoom.Instance?.GetCreatureNode(_power.Owner);
		if (nCreature == null)
		{
			((Node)(object)this).QueueFreeSafely();
			return;
		}
		_sprite = ((Node)this).GetNode<TextureRect>(NodePath.op_Implicit("%TextureRect"));
		_powerField = ((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%PowerField"));
		_vfxContainer = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%Container"));
		((Node2D)this).GlobalPosition = nCreature.GetTopOfHitbox();
		TaskHelper.RunSafely(StartVfx());
	}

	public override void _ExitTree()
	{
		Tween? textTween = _textTween;
		if (textTween != null)
		{
			textTween.Kill();
		}
	}

	private async Task StartVfx()
	{
		((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%WearsOff")).SetTextAutoSize(_wearsOffLoc.GetRawText());
		_powerField.SetTextAutoSize(_power.Title.GetFormattedText());
		_sprite.Texture = _power.BigIcon;
		Control vfxContainer = _vfxContainer;
		vfxContainer.Position -= _vfxContainer.Size * 0.5f;
		_textTween = ((Node)this).CreateTween();
		_textTween.TweenProperty((GodotObject)(object)_vfxContainer, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(1f), 0.25).SetEase((EaseType)1).SetTrans((TransitionType)5);
		_textTween.TweenInterval(0.5);
		_textTween.TweenProperty((GodotObject)(object)_vfxContainer, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0f), 1.0).SetEase((EaseType)1).SetTrans((TransitionType)0);
		((Node)this).CreateTween().TweenProperty((GodotObject)(object)_vfxContainer, NodePath.op_Implicit("position:y"), Variant.op_Implicit(((Control)_powerField).Position.Y - 160f), 2.0).SetEase((EaseType)1)
			.SetTrans((TransitionType)3);
		await ((GodotObject)this).ToSignal((GodotObject)(object)_textTween, SignalName.Finished);
		((Node)(object)this).QueueFreeSafely();
	}

	public static NPowerRemovedVfx? Create(PowerModel power)
	{
		if (TestMode.IsOn)
		{
			return null;
		}
		if (NCombatUi.IsDebugHideTextVfx)
		{
			return null;
		}
		if (!power.ShouldPlayVfx)
		{
			return null;
		}
		NPowerRemovedVfx nPowerRemovedVfx = PreloadManager.Cache.GetScene("res://scenes/vfx/power_removed_vfx.tscn").Instantiate<NPowerRemovedVfx>((GenEditState)0);
		nPowerRemovedVfx._power = power;
		return nPowerRemovedVfx;
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
			_sprite = VariantUtils.ConvertTo<TextureRect>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._powerField)
		{
			_powerField = VariantUtils.ConvertTo<MegaLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._vfxContainer)
		{
			_vfxContainer = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._textTween)
		{
			_textTween = VariantUtils.ConvertTo<Tween>(ref value);
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
		if ((ref name) == PropertyName._powerField)
		{
			value = VariantUtils.CreateFrom<MegaLabel>(ref _powerField);
			return true;
		}
		if ((ref name) == PropertyName._vfxContainer)
		{
			value = VariantUtils.CreateFrom<Control>(ref _vfxContainer);
			return true;
		}
		if ((ref name) == PropertyName._textTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _textTween);
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
		list.Add(new PropertyInfo((Type)24, PropertyName._powerField, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._vfxContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._textTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		info.AddProperty(PropertyName._powerField, Variant.From<MegaLabel>(ref _powerField));
		info.AddProperty(PropertyName._vfxContainer, Variant.From<Control>(ref _vfxContainer));
		info.AddProperty(PropertyName._textTween, Variant.From<Tween>(ref _textTween));
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
		if (info.TryGetProperty(PropertyName._powerField, ref val2))
		{
			_powerField = ((Variant)(ref val2)).As<MegaLabel>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._vfxContainer, ref val3))
		{
			_vfxContainer = ((Variant)(ref val3)).As<Control>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._textTween, ref val4))
		{
			_textTween = ((Variant)(ref val4)).As<Tween>();
		}
	}
}
