using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.Nodes.Screens.Timeline.UnlockScreens;

[ScriptPath("res://src/Core/Nodes/Screens/Timeline/UnlockScreens/NUnlockMiscScreen.cs")]
public class NUnlockMiscScreen : NUnlockScreen
{
	public new class MethodName : NUnlockScreen.MethodName
	{
		public static readonly StringName Create = StringName.op_Implicit("Create");

		public new static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public new static readonly StringName Open = StringName.op_Implicit("Open");

		public static readonly StringName SetUnlocks = StringName.op_Implicit("SetUnlocks");

		public new static readonly StringName OnScreenPreClose = StringName.op_Implicit("OnScreenPreClose");

		public new static readonly StringName OnScreenClose = StringName.op_Implicit("OnScreenClose");
	}

	public new class PropertyName : NUnlockScreen.PropertyName
	{
		public static readonly StringName _label = StringName.op_Implicit("_label");

		public static readonly StringName _textToSet = StringName.op_Implicit("_textToSet");

		public new static readonly StringName _tween = StringName.op_Implicit("_tween");
	}

	public new class SignalName : NUnlockScreen.SignalName
	{
	}

	private static readonly string _scenePath = SceneHelper.GetScenePath("timeline_screen/unlock_misc_screen");

	private MegaRichTextLabel _label;

	private string _textToSet;

	private Tween? _tween;

	public static IEnumerable<string> AssetPaths => new _003C_003Ez__ReadOnlySingleElementList<string>(_scenePath);

	public static NUnlockMiscScreen Create()
	{
		return PreloadManager.Cache.GetScene(_scenePath).Instantiate<NUnlockMiscScreen>((GenEditState)0);
	}

	public override void _Ready()
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		ConnectSignals();
		_label = ((Node)this).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("%Label"));
		_label.Text = _textToSet;
		((CanvasItem)_label).Modulate = StsColors.transparentBlack;
	}

	public override void Open()
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		base.Open();
		SfxCmd.Play("event:/sfx/ui/timeline/ui_timeline_unlock");
		_tween = ((Node)this).CreateTween().SetParallel(true);
		_tween.TweenProperty((GodotObject)(object)_label, NodePath.op_Implicit("modulate"), Variant.op_Implicit(Colors.White), 1.0).SetDelay(0.25);
	}

	public void SetUnlocks(string text)
	{
		_textToSet = text;
	}

	protected override void OnScreenPreClose()
	{
		Tween? tween = _tween;
		if (tween != null)
		{
			tween.Kill();
		}
	}

	protected override void OnScreenClose()
	{
		NTimelineScreen.Instance.EnableInput();
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Expected O, but got Unknown
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(6);
		list.Add(new MethodInfo(MethodName.Create, new PropertyInfo((Type)24, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false), (MethodFlags)33, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.Open, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetUnlocks, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)4, StringName.op_Implicit("text"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnScreenPreClose, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnScreenClose, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.Create && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			NUnlockMiscScreen nUnlockMiscScreen = Create();
			ret = VariantUtils.CreateFrom<NUnlockMiscScreen>(ref nUnlockMiscScreen);
			return true;
		}
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.Open && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			Open();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetUnlocks && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			SetUnlocks(VariantUtils.ConvertTo<string>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnScreenPreClose && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnScreenPreClose();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnScreenClose && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnScreenClose();
			ret = default(godot_variant);
			return true;
		}
		return base.InvokeGodotClassMethod(in method, args, out ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static bool InvokeGodotClassStaticMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.Create && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			NUnlockMiscScreen nUnlockMiscScreen = Create();
			ret = VariantUtils.CreateFrom<NUnlockMiscScreen>(ref nUnlockMiscScreen);
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
		if ((ref method) == MethodName.Open)
		{
			return true;
		}
		if ((ref method) == MethodName.SetUnlocks)
		{
			return true;
		}
		if ((ref method) == MethodName.OnScreenPreClose)
		{
			return true;
		}
		if ((ref method) == MethodName.OnScreenClose)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._label)
		{
			_label = VariantUtils.ConvertTo<MegaRichTextLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._textToSet)
		{
			_textToSet = VariantUtils.ConvertTo<string>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._tween)
		{
			_tween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		return base.SetGodotClassPropertyValue(in name, in value);
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
			value = VariantUtils.CreateFrom<MegaRichTextLabel>(ref _label);
			return true;
		}
		if ((ref name) == PropertyName._textToSet)
		{
			value = VariantUtils.CreateFrom<string>(ref _textToSet);
			return true;
		}
		if ((ref name) == PropertyName._tween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _tween);
			return true;
		}
		return base.GetGodotClassPropertyValue(in name, out value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._label, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)4, PropertyName._textToSet, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._tween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName._label, Variant.From<MegaRichTextLabel>(ref _label));
		info.AddProperty(PropertyName._textToSet, Variant.From<string>(ref _textToSet));
		info.AddProperty(PropertyName._tween, Variant.From<Tween>(ref _tween));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._label, ref val))
		{
			_label = ((Variant)(ref val)).As<MegaRichTextLabel>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._textToSet, ref val2))
		{
			_textToSet = ((Variant)(ref val2)).As<string>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._tween, ref val3))
		{
			_tween = ((Variant)(ref val3)).As<Tween>();
		}
	}
}
