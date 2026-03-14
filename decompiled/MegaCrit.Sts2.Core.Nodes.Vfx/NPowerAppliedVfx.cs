using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.TestSupport;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.Nodes.Vfx;

[ScriptPath("res://src/Core/Nodes/Vfx/NPowerAppliedVfx.cs")]
public class NPowerAppliedVfx : Control
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName _ExitTree = StringName.op_Implicit("_ExitTree");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName _icon = StringName.op_Implicit("_icon");

		public static readonly StringName _iconEcho = StringName.op_Implicit("_iconEcho");

		public static readonly StringName _powerField = StringName.op_Implicit("_powerField");

		public static readonly StringName _amount = StringName.op_Implicit("_amount");

		public static readonly StringName _textTween = StringName.op_Implicit("_textTween");

		public static readonly StringName _spriteTween = StringName.op_Implicit("_spriteTween");
	}

	public class SignalName : SignalName
	{
	}

	private const string _scenePath = "res://scenes/vfx/power_applied_vfx.tscn";

	private TextureRect _icon;

	private TextureRect _iconEcho;

	private MegaLabel _powerField;

	private PowerModel _power;

	private int _amount;

	private Tween? _textTween;

	private Tween? _spriteTween;

	public static IEnumerable<string> AssetPaths => new _003C_003Ez__ReadOnlySingleElementList<string>("res://scenes/vfx/power_applied_vfx.tscn");

	public override void _Ready()
	{
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		_icon = ((Node)this).GetNode<TextureRect>(NodePath.op_Implicit("Icon"));
		_iconEcho = ((Node)this).GetNode<TextureRect>(NodePath.op_Implicit("Icon/IconEcho"));
		_powerField = ((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("Label"));
		NCreature nCreature = NCombatRoom.Instance?.GetCreatureNode(_power.Owner);
		if (nCreature == null)
		{
			((Node)(object)this).QueueFreeSafely();
			return;
		}
		((Control)this).GlobalPosition = nCreature.VfxSpawnPosition;
		TaskHelper.RunSafely(StartVfx());
	}

	public static NPowerAppliedVfx? Create(PowerModel power, int amount)
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
		NPowerAppliedVfx nPowerAppliedVfx = PreloadManager.Cache.GetScene("res://scenes/vfx/power_applied_vfx.tscn").Instantiate<NPowerAppliedVfx>((GenEditState)0);
		nPowerAppliedVfx._power = power;
		nPowerAppliedVfx._amount = amount;
		return nPowerAppliedVfx;
	}

	public override void _ExitTree()
	{
		Tween? spriteTween = _spriteTween;
		if (spriteTween != null)
		{
			spriteTween.Kill();
		}
		Tween? textTween = _textTween;
		if (textTween != null)
		{
			textTween.Kill();
		}
	}

	private async Task StartVfx()
	{
		_powerField.SetTextAutoSize(_power.Title.GetFormattedText());
		_icon.Texture = _power.BigIcon;
		_iconEcho.Texture = _power.BigIcon;
		((CanvasItem)_powerField).Modulate = ((_power.GetTypeForAmount(_amount) == PowerType.Buff) ? StsColors.green : StsColors.red);
		((Control)_powerField).Position = new Vector2(((Control)_powerField).Position.X, ((Control)_powerField).Position.Y - 200f);
		_spriteTween = ((Node)this).CreateTween().SetParallel(true);
		_spriteTween.TweenProperty((GodotObject)(object)_icon, NodePath.op_Implicit("scale"), Variant.op_Implicit(Vector2.One * 0.8f), 1.25).SetEase((EaseType)1).SetTrans((TransitionType)5)
			.From(Variant.op_Implicit(Vector2.One * 0.4f));
		_spriteTween.TweenProperty((GodotObject)(object)_icon, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0.5f), 0.25).SetEase((EaseType)1).SetTrans((TransitionType)1);
		_spriteTween.TweenProperty((GodotObject)(object)_icon, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0f), 1.0).SetEase((EaseType)0).SetTrans((TransitionType)1)
			.SetDelay(0.25);
		_textTween = ((Node)this).CreateTween().SetParallel(true);
		((Node)this).CreateTween().TweenProperty((GodotObject)(object)_powerField, NodePath.op_Implicit("position:y"), Variant.op_Implicit(((Control)_powerField).Position.Y + 50f), 1.25).SetEase((EaseType)1)
			.SetTrans((TransitionType)7);
		_textTween.TweenProperty((GodotObject)(object)_powerField, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(1f), 0.25).SetEase((EaseType)1).SetTrans((TransitionType)5);
		_textTween.TweenProperty((GodotObject)(object)_powerField, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0f), 0.75).SetEase((EaseType)0).SetTrans((TransitionType)5)
			.From(Variant.op_Implicit(1f))
			.SetDelay(0.25);
		await ((GodotObject)this).ToSignal((GodotObject)(object)_spriteTween, SignalName.Finished);
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
		if ((ref name) == PropertyName._icon)
		{
			_icon = VariantUtils.ConvertTo<TextureRect>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._iconEcho)
		{
			_iconEcho = VariantUtils.ConvertTo<TextureRect>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._powerField)
		{
			_powerField = VariantUtils.ConvertTo<MegaLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._amount)
		{
			_amount = VariantUtils.ConvertTo<int>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._textTween)
		{
			_textTween = VariantUtils.ConvertTo<Tween>(ref value);
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
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName._icon)
		{
			value = VariantUtils.CreateFrom<TextureRect>(ref _icon);
			return true;
		}
		if ((ref name) == PropertyName._iconEcho)
		{
			value = VariantUtils.CreateFrom<TextureRect>(ref _iconEcho);
			return true;
		}
		if ((ref name) == PropertyName._powerField)
		{
			value = VariantUtils.CreateFrom<MegaLabel>(ref _powerField);
			return true;
		}
		if ((ref name) == PropertyName._amount)
		{
			value = VariantUtils.CreateFrom<int>(ref _amount);
			return true;
		}
		if ((ref name) == PropertyName._textTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _textTween);
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
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._icon, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._iconEcho, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._powerField, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)2, PropertyName._amount, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._textTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._spriteTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		((GodotObject)this).SaveGodotObjectData(info);
		info.AddProperty(PropertyName._icon, Variant.From<TextureRect>(ref _icon));
		info.AddProperty(PropertyName._iconEcho, Variant.From<TextureRect>(ref _iconEcho));
		info.AddProperty(PropertyName._powerField, Variant.From<MegaLabel>(ref _powerField));
		info.AddProperty(PropertyName._amount, Variant.From<int>(ref _amount));
		info.AddProperty(PropertyName._textTween, Variant.From<Tween>(ref _textTween));
		info.AddProperty(PropertyName._spriteTween, Variant.From<Tween>(ref _spriteTween));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._icon, ref val))
		{
			_icon = ((Variant)(ref val)).As<TextureRect>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._iconEcho, ref val2))
		{
			_iconEcho = ((Variant)(ref val2)).As<TextureRect>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._powerField, ref val3))
		{
			_powerField = ((Variant)(ref val3)).As<MegaLabel>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._amount, ref val4))
		{
			_amount = ((Variant)(ref val4)).As<int>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._textTween, ref val5))
		{
			_textTween = ((Variant)(ref val5)).As<Tween>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._spriteTween, ref val6))
		{
			_spriteTween = ((Variant)(ref val6)).As<Tween>();
		}
	}
}
