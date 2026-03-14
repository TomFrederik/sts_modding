using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Audio.Debug;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.TestSupport;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.Nodes.Combat;

[ScriptPath("res://src/Core/Nodes/Combat/NEnemyTurnBanner.cs")]
public class NEnemyTurnBanner : Control
{
	public class MethodName : MethodName
	{
		public static readonly StringName Create = StringName.op_Implicit("Create");

		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName _label = StringName.op_Implicit("_label");
	}

	public class SignalName : SignalName
	{
	}

	private MegaLabel _label;

	private static readonly string _scenePath = SceneHelper.GetScenePath("combat/enemy_turn_banner");

	public static IEnumerable<string> AssetPaths => new _003C_003Ez__ReadOnlySingleElementList<string>(_scenePath);

	public static NEnemyTurnBanner? Create()
	{
		if (TestMode.IsOn)
		{
			return null;
		}
		if (NCombatUi.IsDebugHideTextVfx)
		{
			return null;
		}
		return PreloadManager.Cache.GetScene(_scenePath).Instantiate<NEnemyTurnBanner>((GenEditState)0);
	}

	public override void _Ready()
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		_label = ((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("Label"));
		_label.SetTextAutoSize(new LocString("gameplay_ui", "ENEMY_TURN").GetFormattedText());
		((CanvasItem)this).Modulate = Colors.Transparent;
		TaskHelper.RunSafely(Display());
	}

	private async Task Display()
	{
		NDebugAudioManager.Instance?.Play("enemy_turn.mp3");
		((Control)_label).Scale = Vector2.One * 2f;
		Tween val = ((Node)this).CreateTween();
		val.SetParallel(true);
		val.TweenProperty((GodotObject)(object)_label, NodePath.op_Implicit("scale"), Variant.op_Implicit(Vector2.One), 0.75).SetEase((EaseType)1).SetTrans((TransitionType)5);
		val.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(1f), 1.2999999523162842).SetEase((EaseType)1).SetTrans((TransitionType)5);
		val.Chain();
		val.TweenProperty((GodotObject)(object)_label, NodePath.op_Implicit("modulate"), Variant.op_Implicit(Colors.Red), 1.0).SetEase((EaseType)1).SetTrans((TransitionType)5)
			.FromCurrent();
		val.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0f), 1.0).SetEase((EaseType)1).SetTrans((TransitionType)7);
		await ((GodotObject)this).ToSignal((GodotObject)(object)val, SignalName.Finished);
		((Node)(object)this).QueueFreeSafely();
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Expected O, but got Unknown
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(2);
		list.Add(new MethodInfo(MethodName.Create, new PropertyInfo((Type)24, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false), (MethodFlags)33, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.Create && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			NEnemyTurnBanner nEnemyTurnBanner = Create();
			ret = VariantUtils.CreateFrom<NEnemyTurnBanner>(ref nEnemyTurnBanner);
			return true;
		}
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		return ((Control)this).InvokeGodotClassMethod(ref method, args, ref ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static bool InvokeGodotClassStaticMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.Create && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			NEnemyTurnBanner nEnemyTurnBanner = Create();
			ret = VariantUtils.CreateFrom<NEnemyTurnBanner>(ref nEnemyTurnBanner);
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
		return ((Control)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._label)
		{
			_label = VariantUtils.ConvertTo<MegaLabel>(ref value);
			return true;
		}
		return ((GodotObject)this).SetGodotClassPropertyValue(ref name, ref value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName._label)
		{
			value = VariantUtils.CreateFrom<MegaLabel>(ref _label);
			return true;
		}
		return ((GodotObject)this).GetGodotClassPropertyValue(ref name, ref value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._label, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		info.AddProperty(PropertyName._label, Variant.From<MegaLabel>(ref _label));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._label, ref val))
		{
			_label = ((Variant)(ref val)).As<MegaLabel>();
		}
	}
}
