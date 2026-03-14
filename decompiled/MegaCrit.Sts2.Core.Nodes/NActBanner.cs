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
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.Settings;
using MegaCrit.Sts2.Core.TestSupport;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.Nodes;

[ScriptPath("res://src/Core/Nodes/NActBanner.cs")]
public class NActBanner : Control
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName _actNumber = StringName.op_Implicit("_actNumber");

		public static readonly StringName _actName = StringName.op_Implicit("_actName");

		public static readonly StringName _banner = StringName.op_Implicit("_banner");

		public static readonly StringName _actIndex = StringName.op_Implicit("_actIndex");
	}

	public class SignalName : SignalName
	{
	}

	private MegaLabel _actNumber;

	private MegaLabel _actName;

	private ColorRect _banner;

	private static readonly string _path = SceneHelper.GetScenePath("ui/act_banner");

	private ActModel _act;

	private int _actIndex;

	public static IEnumerable<string> AssetPaths => new _003C_003Ez__ReadOnlySingleElementList<string>(_path);

	public static NActBanner? Create(ActModel act, int actIndex)
	{
		if (TestMode.IsOn)
		{
			return null;
		}
		NActBanner nActBanner = PreloadManager.Cache.GetScene(_path).Instantiate<NActBanner>((GenEditState)0);
		nActBanner._act = act;
		nActBanner._actIndex = actIndex;
		return nActBanner;
	}

	public override void _Ready()
	{
		_actNumber = ((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("ActNumber"));
		_actName = ((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("ActName"));
		_banner = ((Node)this).GetNode<ColorRect>(NodePath.op_Implicit("%Banner"));
		LocString locString = new LocString("gameplay_ui", "ACT_NUMBER");
		locString.Add("actNumber", _actIndex + 1);
		_actNumber.SetTextAutoSize(locString.GetFormattedText());
		_actName.SetTextAutoSize(_act.Title.GetFormattedText());
		TaskHelper.RunSafely(AnimateVfx());
	}

	private async Task AnimateVfx()
	{
		((CanvasItem)_banner).Modulate = StsColors.transparentBlack;
		((CanvasItem)_actName).Modulate = StsColors.transparentWhite;
		((CanvasItem)_actNumber).Modulate = StsColors.transparentWhite;
		Tween val = ((Node)this).CreateTween().SetParallel(true);
		val.TweenProperty((GodotObject)(object)_banner, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0.25f), 0.5).SetDelay(0.5);
		val.TweenProperty((GodotObject)(object)_actName, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(1f), 1.0).SetDelay(0.25);
		val.TweenProperty((GodotObject)(object)_actNumber, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(1f), 1.0).SetDelay(0.5);
		val.TweenProperty((GodotObject)(object)_actNumber, NodePath.op_Implicit("position:y"), Variant.op_Implicit(440f), 1.25).SetDelay(0.5).SetEase((EaseType)1)
			.SetTrans((TransitionType)4)
			.From(Variant.op_Implicit(450f));
		val.Chain();
		val.TweenInterval((SaveManager.Instance.PrefsSave.FastMode == FastModeType.Fast) ? 0.5 : 2.0);
		val.Chain();
		val.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0f), 1.0).SetEase((EaseType)1).SetTrans((TransitionType)4);
		await ((GodotObject)this).ToSignal((GodotObject)(object)val, SignalName.Finished);
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
		return ((Control)this).InvokeGodotClassMethod(ref method, args, ref ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if ((ref method) == MethodName._Ready)
		{
			return true;
		}
		return ((Control)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._actNumber)
		{
			_actNumber = VariantUtils.ConvertTo<MegaLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._actName)
		{
			_actName = VariantUtils.ConvertTo<MegaLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._banner)
		{
			_banner = VariantUtils.ConvertTo<ColorRect>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._actIndex)
		{
			_actIndex = VariantUtils.ConvertTo<int>(ref value);
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
		if ((ref name) == PropertyName._actNumber)
		{
			value = VariantUtils.CreateFrom<MegaLabel>(ref _actNumber);
			return true;
		}
		if ((ref name) == PropertyName._actName)
		{
			value = VariantUtils.CreateFrom<MegaLabel>(ref _actName);
			return true;
		}
		if ((ref name) == PropertyName._banner)
		{
			value = VariantUtils.CreateFrom<ColorRect>(ref _banner);
			return true;
		}
		if ((ref name) == PropertyName._actIndex)
		{
			value = VariantUtils.CreateFrom<int>(ref _actIndex);
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
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._actNumber, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._actName, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._banner, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)2, PropertyName._actIndex, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		info.AddProperty(PropertyName._actNumber, Variant.From<MegaLabel>(ref _actNumber));
		info.AddProperty(PropertyName._actName, Variant.From<MegaLabel>(ref _actName));
		info.AddProperty(PropertyName._banner, Variant.From<ColorRect>(ref _banner));
		info.AddProperty(PropertyName._actIndex, Variant.From<int>(ref _actIndex));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._actNumber, ref val))
		{
			_actNumber = ((Variant)(ref val)).As<MegaLabel>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._actName, ref val2))
		{
			_actName = ((Variant)(ref val2)).As<MegaLabel>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._banner, ref val3))
		{
			_banner = ((Variant)(ref val3)).As<ColorRect>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._actIndex, ref val4))
		{
			_actIndex = ((Variant)(ref val4)).As<int>();
		}
	}
}
