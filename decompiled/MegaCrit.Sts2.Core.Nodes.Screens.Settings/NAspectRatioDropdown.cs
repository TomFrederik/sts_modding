using System;
using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.Settings;

namespace MegaCrit.Sts2.Core.Nodes.Screens.Settings;

[ScriptPath("res://src/Core/Nodes/Screens/Settings/NAspectRatioDropdown.cs")]
public class NAspectRatioDropdown : NSettingsDropdown, IResettableSettingNode
{
	public new class MethodName : NSettingsDropdown.MethodName
	{
		public new static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName SetFromSettings = StringName.op_Implicit("SetFromSettings");

		public static readonly StringName AddDropdownItem = StringName.op_Implicit("AddDropdownItem");

		public static readonly StringName OnDropdownItemSelected = StringName.op_Implicit("OnDropdownItemSelected");

		public static readonly StringName GetAspectRatioSettingString = StringName.op_Implicit("GetAspectRatioSettingString");
	}

	public new class PropertyName : NSettingsDropdown.PropertyName
	{
		public static readonly StringName _currentAspectRatioSetting = StringName.op_Implicit("_currentAspectRatioSetting");
	}

	public new class SignalName : NSettingsDropdown.SignalName
	{
	}

	private AspectRatioSetting _currentAspectRatioSetting;

	private static string AspectRatioDropdownItemScenePath => SceneHelper.GetScenePath("ui/aspect_ratio_dropdown_item");

	public static IEnumerable<string> AssetPaths => new _003C_003Ez__ReadOnlySingleElementList<string>(AspectRatioDropdownItemScenePath);

	public override void _Ready()
	{
		ConnectSignals();
		ClearDropdownItems();
		AddDropdownItem(AspectRatioSetting.Auto);
		AddDropdownItem(AspectRatioSetting.FourByThree);
		AddDropdownItem(AspectRatioSetting.SixteenByTen);
		AddDropdownItem(AspectRatioSetting.SixteenByNine);
		AddDropdownItem(AspectRatioSetting.TwentyOneByNine);
		((Node)_dropdownItems).GetParent<NDropdownContainer>().RefreshLayout();
		SetFromSettings();
	}

	public void SetFromSettings()
	{
		_currentAspectRatioSetting = SaveManager.Instance.SettingsSave.AspectRatioSetting;
		_currentOptionLabel.SetTextAutoSize(GetAspectRatioSettingString(_currentAspectRatioSetting));
	}

	private void AddDropdownItem(AspectRatioSetting aspectRatioSetting)
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		NAspectRatioDropdownItem nAspectRatioDropdownItem = ResourceLoader.Load<PackedScene>(AspectRatioDropdownItemScenePath, (string)null, (CacheMode)1).Instantiate<NAspectRatioDropdownItem>((GenEditState)0);
		((Node)(object)_dropdownItems).AddChildSafely((Node?)(object)nAspectRatioDropdownItem);
		((GodotObject)nAspectRatioDropdownItem).Connect(NDropdownItem.SignalName.Selected, Callable.From<NDropdownItem>((Action<NDropdownItem>)OnDropdownItemSelected), 0u);
		nAspectRatioDropdownItem.Init(aspectRatioSetting);
	}

	private void OnDropdownItemSelected(NDropdownItem nDropdownItem)
	{
		NAspectRatioDropdownItem nAspectRatioDropdownItem = (NAspectRatioDropdownItem)nDropdownItem;
		if (nAspectRatioDropdownItem.aspectRatioSetting != _currentAspectRatioSetting)
		{
			CloseDropdown();
			SaveManager.Instance.SettingsSave.AspectRatioSetting = nAspectRatioDropdownItem.aspectRatioSetting;
			SetFromSettings();
			NGame.Instance.ApplyDisplaySettings();
		}
	}

	private static string GetAspectRatioSettingString(AspectRatioSetting aspectRatioSettingString)
	{
		return aspectRatioSettingString switch
		{
			AspectRatioSetting.Auto => new LocString("settings_ui", "ASPECT_RATIO_AUTO").GetFormattedText(), 
			AspectRatioSetting.FourByThree => new LocString("settings_ui", "ASPECT_RATIO_FOUR_BY_THREE").GetFormattedText(), 
			AspectRatioSetting.SixteenByTen => new LocString("settings_ui", "ASPECT_RATIO_SIXTEEN_BY_TEN").GetFormattedText(), 
			AspectRatioSetting.SixteenByNine => new LocString("settings_ui", "ASPECT_RATIO_SIXTEEN_BY_NINE").GetFormattedText(), 
			AspectRatioSetting.TwentyOneByNine => new LocString("settings_ui", "ASPECT_RATIO_TWENTY_ONE_BY_NINE").GetFormattedText(), 
			_ => throw new ArgumentOutOfRangeException($"Invalid Aspect Ratio: {aspectRatioSettingString}"), 
		};
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Expected O, but got Unknown
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(5);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetFromSettings, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AddDropdownItem, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("aspectRatioSetting"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnDropdownItemSelected, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("nDropdownItem"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.GetAspectRatioSettingString, new PropertyInfo((Type)4, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)33, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("aspectRatioSettingString"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetFromSettings && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			SetFromSettings();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.AddDropdownItem && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			AddDropdownItem(VariantUtils.ConvertTo<AspectRatioSetting>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnDropdownItemSelected && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OnDropdownItemSelected(VariantUtils.ConvertTo<NDropdownItem>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.GetAspectRatioSettingString && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			string aspectRatioSettingString = GetAspectRatioSettingString(VariantUtils.ConvertTo<AspectRatioSetting>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<string>(ref aspectRatioSettingString);
			return true;
		}
		return base.InvokeGodotClassMethod(in method, args, out ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static bool InvokeGodotClassStaticMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.GetAspectRatioSettingString && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			string aspectRatioSettingString = GetAspectRatioSettingString(VariantUtils.ConvertTo<AspectRatioSetting>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<string>(ref aspectRatioSettingString);
			return true;
		}
		ret = default(godot_variant);
		return false;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if ((ref method) == MethodName._Ready)
		{
			return true;
		}
		if ((ref method) == MethodName.SetFromSettings)
		{
			return true;
		}
		if ((ref method) == MethodName.AddDropdownItem)
		{
			return true;
		}
		if ((ref method) == MethodName.OnDropdownItemSelected)
		{
			return true;
		}
		if ((ref method) == MethodName.GetAspectRatioSettingString)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._currentAspectRatioSetting)
		{
			_currentAspectRatioSetting = VariantUtils.ConvertTo<AspectRatioSetting>(ref value);
			return true;
		}
		return base.SetGodotClassPropertyValue(in name, in value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName._currentAspectRatioSetting)
		{
			value = VariantUtils.CreateFrom<AspectRatioSetting>(ref _currentAspectRatioSetting);
			return true;
		}
		return base.GetGodotClassPropertyValue(in name, out value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)2, PropertyName._currentAspectRatioSetting, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName._currentAspectRatioSetting, Variant.From<AspectRatioSetting>(ref _currentAspectRatioSetting));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._currentAspectRatioSetting, ref val))
		{
			_currentAspectRatioSetting = ((Variant)(ref val)).As<AspectRatioSetting>();
		}
	}
}
