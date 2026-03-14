using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Audio.Debug;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.TestSupport;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.Nodes.Combat;

[ScriptPath("res://src/Core/Nodes/Combat/NPlayerTurnBanner.cs")]
public class NPlayerTurnBanner : Control
{
	public class MethodName : MethodName
	{
		public static readonly StringName Create = StringName.op_Implicit("Create");

		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName _label = StringName.op_Implicit("_label");

		public static readonly StringName _turnLabel = StringName.op_Implicit("_turnLabel");

		public static readonly StringName _roundNumber = StringName.op_Implicit("_roundNumber");
	}

	public class SignalName : SignalName
	{
	}

	private MegaLabel _label;

	private MegaLabel _turnLabel;

	private int _roundNumber;

	private static readonly string _scenePath = SceneHelper.GetScenePath("combat/player_turn_banner");

	public static IEnumerable<string> AssetPaths => new _003C_003Ez__ReadOnlySingleElementList<string>(_scenePath);

	public static NPlayerTurnBanner? Create(int roundNumber)
	{
		if (TestMode.IsOn)
		{
			return null;
		}
		if (NCombatUi.IsDebugHideTextVfx)
		{
			return null;
		}
		NPlayerTurnBanner nPlayerTurnBanner = PreloadManager.Cache.GetScene(_scenePath).Instantiate<NPlayerTurnBanner>((GenEditState)0);
		nPlayerTurnBanner._roundNumber = roundNumber;
		return nPlayerTurnBanner;
	}

	public override void _Ready()
	{
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		_label = ((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("Label"));
		if (CombatManager.Instance.PlayersTakingExtraTurn.Count > 0)
		{
			_label.SetTextAutoSize(new LocString("gameplay_ui", "PLAYER_TURN_EXTRA").GetFormattedText());
		}
		else
		{
			_label.SetTextAutoSize(new LocString("gameplay_ui", "PLAYER_TURN").GetFormattedText());
		}
		_turnLabel = ((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("TurnNumber"));
		LocString locString = new LocString("gameplay_ui", "TURN_COUNT");
		locString.Add("turnNumber", _roundNumber);
		_turnLabel.SetTextAutoSize(locString.GetFormattedText());
		((CanvasItem)this).Modulate = Colors.Transparent;
		TaskHelper.RunSafely(Display());
	}

	private async Task Display()
	{
		NDebugAudioManager.Instance?.Play("player_turn.mp3");
		Tween val = ((Node)this).CreateTween();
		val.SetParallel(true);
		val.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(1f), 1.0).SetEase((EaseType)1).SetTrans((TransitionType)5);
		val.TweenProperty((GodotObject)(object)_label, NodePath.op_Implicit("position"), Variant.op_Implicit(((Control)_label).Position + new Vector2(0f, -50f)), 1.5).SetEase((EaseType)1).SetTrans((TransitionType)5);
		val.TweenProperty((GodotObject)(object)_turnLabel, NodePath.op_Implicit("position"), Variant.op_Implicit(((Control)_turnLabel).Position + new Vector2(0f, 50f)), 1.5).SetEase((EaseType)1).SetTrans((TransitionType)5);
		await ((GodotObject)this).ToSignal((GodotObject)(object)val, SignalName.Finished);
		val = ((Node)this).CreateTween();
		val.TweenInterval(0.4);
		val.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0f), 0.30000001192092896).SetEase((EaseType)1).SetTrans((TransitionType)1);
		await ((GodotObject)this).ToSignal((GodotObject)(object)val, SignalName.Finished);
		((Node)(object)this).QueueFreeSafely();
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Expected O, but got Unknown
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(2);
		list.Add(new MethodInfo(MethodName.Create, new PropertyInfo((Type)24, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false), (MethodFlags)33, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("roundNumber"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.Create && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			NPlayerTurnBanner nPlayerTurnBanner = Create(VariantUtils.ConvertTo<int>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<NPlayerTurnBanner>(ref nPlayerTurnBanner);
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
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.Create && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			NPlayerTurnBanner nPlayerTurnBanner = Create(VariantUtils.ConvertTo<int>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<NPlayerTurnBanner>(ref nPlayerTurnBanner);
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
		if ((ref name) == PropertyName._turnLabel)
		{
			_turnLabel = VariantUtils.ConvertTo<MegaLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._roundNumber)
		{
			_roundNumber = VariantUtils.ConvertTo<int>(ref value);
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
		if ((ref name) == PropertyName._label)
		{
			value = VariantUtils.CreateFrom<MegaLabel>(ref _label);
			return true;
		}
		if ((ref name) == PropertyName._turnLabel)
		{
			value = VariantUtils.CreateFrom<MegaLabel>(ref _turnLabel);
			return true;
		}
		if ((ref name) == PropertyName._roundNumber)
		{
			value = VariantUtils.CreateFrom<int>(ref _roundNumber);
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
		list.Add(new PropertyInfo((Type)24, PropertyName._label, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._turnLabel, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)2, PropertyName._roundNumber, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		info.AddProperty(PropertyName._label, Variant.From<MegaLabel>(ref _label));
		info.AddProperty(PropertyName._turnLabel, Variant.From<MegaLabel>(ref _turnLabel));
		info.AddProperty(PropertyName._roundNumber, Variant.From<int>(ref _roundNumber));
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
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._turnLabel, ref val2))
		{
			_turnLabel = ((Variant)(ref val2)).As<MegaLabel>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._roundNumber, ref val3))
		{
			_roundNumber = ((Variant)(ref val3)).As<int>();
		}
	}
}
