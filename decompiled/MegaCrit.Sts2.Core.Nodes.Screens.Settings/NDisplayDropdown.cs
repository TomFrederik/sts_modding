using System;
using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Platform;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.Settings;

namespace MegaCrit.Sts2.Core.Nodes.Screens.Settings;

[ScriptPath("res://src/Core/Nodes/Screens/Settings/NDisplayDropdown.cs")]
public class NDisplayDropdown : NSettingsDropdown
{
	public new class MethodName : NSettingsDropdown.MethodName
	{
		public new static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName _Notification = StringName.op_Implicit("_Notification");

		public static readonly StringName OnWindowChange = StringName.op_Implicit("OnWindowChange");

		public static readonly StringName OnDropdownItemSelected = StringName.op_Implicit("OnDropdownItemSelected");
	}

	public new class PropertyName : NSettingsDropdown.PropertyName
	{
		public static readonly StringName _dropdownItemScene = StringName.op_Implicit("_dropdownItemScene");

		public static readonly StringName _currentDisplayIndex = StringName.op_Implicit("_currentDisplayIndex");
	}

	public new class SignalName : NSettingsDropdown.SignalName
	{
	}

	[Export(/*Could not decode attribute arguments.*/)]
	private PackedScene _dropdownItemScene;

	private static readonly LocString _optionString = new LocString("settings_ui", "DISPLAY_DROPDOWN_OPTION");

	private int _currentDisplayIndex = -1;

	public override void _Ready()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		ConnectSignals();
		((GodotObject)NGame.Instance).Connect(NGame.SignalName.WindowChange, Callable.From<bool>((Action<bool>)OnWindowChange), 0u);
		OnWindowChange(SaveManager.Instance.SettingsSave.AspectRatioSetting == AspectRatioSetting.Auto);
		ClearDropdownItems();
		for (int i = 0; i < DisplayServer.GetScreenCount(); i++)
		{
			NDisplayDropdownItem nDisplayDropdownItem = _dropdownItemScene.Instantiate<NDisplayDropdownItem>((GenEditState)0);
			((Node)(object)_dropdownItems).AddChildSafely((Node?)(object)nDisplayDropdownItem);
			((GodotObject)nDisplayDropdownItem).Connect(NDropdownItem.SignalName.Selected, Callable.From<NDropdownItem>((Action<NDropdownItem>)OnDropdownItemSelected), 0u);
			nDisplayDropdownItem.Init(i);
		}
		((Node)_dropdownItems).GetParent<NDropdownContainer>().RefreshLayout();
	}

	public override void _Notification(int what)
	{
		if ((long)what == 1012 && ((Node)this).IsNodeReady())
		{
			OnWindowChange(SaveManager.Instance.SettingsSave.AspectRatioSetting == AspectRatioSetting.Auto);
		}
	}

	private void OnWindowChange(bool _)
	{
		long num = ((Node)this).GetWindow().CurrentScreen;
		if (num != _currentDisplayIndex)
		{
			_currentDisplayIndex = (int)num;
			_optionString.Add("MonitorIndex", _currentDisplayIndex);
			_currentOptionLabel.SetTextAutoSize(_optionString.GetFormattedText());
			SaveManager.Instance.SettingsSave.TargetDisplay = _currentDisplayIndex;
			NResolutionDropdown? instance = NResolutionDropdown.Instance;
			if (instance != null && ((Node)instance).IsNodeReady())
			{
				NResolutionDropdown.Instance.RefreshCurrentlySelectedResolution();
				NResolutionDropdown.Instance.PopulateDropdownItems();
			}
		}
	}

	private void OnDropdownItemSelected(NDropdownItem nDropdownItem)
	{
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		SettingsSave settingsSave = SaveManager.Instance.SettingsSave;
		NDisplayDropdownItem nDisplayDropdownItem = (NDisplayDropdownItem)nDropdownItem;
		if (nDisplayDropdownItem.displayIndex != _currentDisplayIndex)
		{
			CloseDropdown();
			settingsSave.TargetDisplay = nDisplayDropdownItem.displayIndex;
			if (!settingsSave.Fullscreen && !PlatformUtil.GetSupportedWindowMode().ShouldForceFullscreen())
			{
				Vector2I val = DisplayServer.ScreenGetSize(nDisplayDropdownItem.displayIndex);
				settingsSave.WindowPosition = val / 8;
				settingsSave.WindowSize = new Vector2I((int)((float)val.X * 0.75f), (int)((float)val.Y * 0.75f));
			}
			NResolutionDropdown.Instance.RefreshCurrentlySelectedResolution();
			NResolutionDropdown.Instance.PopulateDropdownItems();
			NGame.Instance.ApplyDisplaySettings();
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Expected O, but got Unknown
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(4);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Notification, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("what"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnWindowChange, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)1, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnDropdownItemSelected, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("nDropdownItem"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._Notification && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			((GodotObject)this)._Notification(VariantUtils.ConvertTo<int>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnWindowChange && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OnWindowChange(VariantUtils.ConvertTo<bool>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnDropdownItemSelected && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OnDropdownItemSelected(VariantUtils.ConvertTo<NDropdownItem>(ref ((NativeVariantPtrArgs)(ref args))[0]));
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
		if ((ref method) == MethodName._Notification)
		{
			return true;
		}
		if ((ref method) == MethodName.OnWindowChange)
		{
			return true;
		}
		if ((ref method) == MethodName.OnDropdownItemSelected)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._dropdownItemScene)
		{
			_dropdownItemScene = VariantUtils.ConvertTo<PackedScene>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._currentDisplayIndex)
		{
			_currentDisplayIndex = VariantUtils.ConvertTo<int>(ref value);
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
		if ((ref name) == PropertyName._dropdownItemScene)
		{
			value = VariantUtils.CreateFrom<PackedScene>(ref _dropdownItemScene);
			return true;
		}
		if ((ref name) == PropertyName._currentDisplayIndex)
		{
			value = VariantUtils.CreateFrom<int>(ref _currentDisplayIndex);
			return true;
		}
		return base.GetGodotClassPropertyValue(in name, out value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._dropdownItemScene, (PropertyHint)17, "PackedScene", (PropertyUsageFlags)4102, true));
		list.Add(new PropertyInfo((Type)2, PropertyName._currentDisplayIndex, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName._dropdownItemScene, Variant.From<PackedScene>(ref _dropdownItemScene));
		info.AddProperty(PropertyName._currentDisplayIndex, Variant.From<int>(ref _currentDisplayIndex));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._dropdownItemScene, ref val))
		{
			_dropdownItemScene = ((Variant)(ref val)).As<PackedScene>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._currentDisplayIndex, ref val2))
		{
			_currentDisplayIndex = ((Variant)(ref val2)).As<int>();
		}
	}
}
