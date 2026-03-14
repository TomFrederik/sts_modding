using System;
using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Screens.CharacterSelect;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.TestSupport;

namespace MegaCrit.Sts2.Core.Nodes.Ftue;

[ScriptPath("res://src/Core/Nodes/Ftue/NAcceptTutorialsFtue.cs")]
public class NAcceptTutorialsFtue : NFtue
{
	public new class MethodName : NFtue.MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName NoTutorials = StringName.op_Implicit("NoTutorials");

		public static readonly StringName YesTutorials = StringName.op_Implicit("YesTutorials");
	}

	public new class PropertyName : NFtue.PropertyName
	{
		public static readonly StringName _charSelectScreen = StringName.op_Implicit("_charSelectScreen");

		public static readonly StringName _verticalPopup = StringName.op_Implicit("_verticalPopup");
	}

	public new class SignalName : NFtue.SignalName
	{
	}

	public const string id = "accept_tutorials_ftue";

	private static readonly string _scenePath = SceneHelper.GetScenePath("ftue/accept_tutorials_ftue");

	private NCharacterSelectScreen _charSelectScreen;

	private NVerticalPopup _verticalPopup;

	private Action _onFinished;

	public override void _Ready()
	{
		_verticalPopup = ((Node)this).GetNode<NVerticalPopup>(NodePath.op_Implicit("VerticalPopup"));
		_verticalPopup.SetText(new LocString("main_menu_ui", "ENABLE_TUTORIALS.title"), new LocString("main_menu_ui", "ENABLE_TUTORIALS.description"));
		_verticalPopup.InitYesButton(new LocString("main_menu_ui", "GENERIC_POPUP.confirm"), YesTutorials);
		_verticalPopup.InitNoButton(new LocString("main_menu_ui", "GENERIC_POPUP.cancel"), NoTutorials);
	}

	public static NAcceptTutorialsFtue? Create(NCharacterSelectScreen charSelectScreen, Action onFinished)
	{
		if (TestMode.IsOn)
		{
			return null;
		}
		NAcceptTutorialsFtue nAcceptTutorialsFtue = PreloadManager.Cache.GetScene(_scenePath).Instantiate<NAcceptTutorialsFtue>((GenEditState)0);
		nAcceptTutorialsFtue._charSelectScreen = charSelectScreen;
		nAcceptTutorialsFtue._onFinished = onFinished;
		return nAcceptTutorialsFtue;
	}

	private void NoTutorials(NButton _)
	{
		SaveManager.Instance.MarkFtueAsComplete("accept_tutorials_ftue");
		SaveManager.Instance.SetFtuesEnabled(enabled: false);
		_onFinished();
		CloseFtue();
	}

	private void YesTutorials(NButton _)
	{
		SaveManager.Instance.MarkFtueAsComplete("accept_tutorials_ftue");
		_onFinished();
		CloseFtue();
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Expected O, but got Unknown
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Expected O, but got Unknown
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(3);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.NoTutorials, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.YesTutorials, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.NoTutorials && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			NoTutorials(VariantUtils.ConvertTo<NButton>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.YesTutorials && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			YesTutorials(VariantUtils.ConvertTo<NButton>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		return base.InvokeGodotClassMethod(in method, args, out ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if ((ref method) == MethodName._Ready)
		{
			return true;
		}
		if ((ref method) == MethodName.NoTutorials)
		{
			return true;
		}
		if ((ref method) == MethodName.YesTutorials)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._charSelectScreen)
		{
			_charSelectScreen = VariantUtils.ConvertTo<NCharacterSelectScreen>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._verticalPopup)
		{
			_verticalPopup = VariantUtils.ConvertTo<NVerticalPopup>(ref value);
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
		if ((ref name) == PropertyName._charSelectScreen)
		{
			value = VariantUtils.CreateFrom<NCharacterSelectScreen>(ref _charSelectScreen);
			return true;
		}
		if ((ref name) == PropertyName._verticalPopup)
		{
			value = VariantUtils.CreateFrom<NVerticalPopup>(ref _verticalPopup);
			return true;
		}
		return base.GetGodotClassPropertyValue(in name, out value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._charSelectScreen, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._verticalPopup, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName._charSelectScreen, Variant.From<NCharacterSelectScreen>(ref _charSelectScreen));
		info.AddProperty(PropertyName._verticalPopup, Variant.From<NVerticalPopup>(ref _verticalPopup));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._charSelectScreen, ref val))
		{
			_charSelectScreen = ((Variant)(ref val)).As<NCharacterSelectScreen>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._verticalPopup, ref val2))
		{
			_verticalPopup = ((Variant)(ref val2)).As<NVerticalPopup>();
		}
	}
}
