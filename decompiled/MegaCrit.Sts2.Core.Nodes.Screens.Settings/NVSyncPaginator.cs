using System;
using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.Settings;

namespace MegaCrit.Sts2.Core.Nodes.Screens.Settings;

[ScriptPath("res://src/Core/Nodes/Screens/Settings/NVSyncPaginator.cs")]
public class NVSyncPaginator : NPaginator, IResettableSettingNode
{
	public new class MethodName : NPaginator.MethodName
	{
		public new static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName SetFromSettings = StringName.op_Implicit("SetFromSettings");

		public static readonly StringName GetVSyncString = StringName.op_Implicit("GetVSyncString");

		public new static readonly StringName OnIndexChanged = StringName.op_Implicit("OnIndexChanged");
	}

	public new class PropertyName : NPaginator.PropertyName
	{
	}

	public new class SignalName : NPaginator.SignalName
	{
	}

	public override void _Ready()
	{
		ConnectSignals();
		_options.Add(new LocString("settings_ui", "VSYNC_OFF").GetFormattedText());
		_options.Add(new LocString("settings_ui", "VSYNC_ON").GetFormattedText());
		_options.Add(new LocString("settings_ui", "VSYNC_ADAPTIVE").GetFormattedText());
		SetFromSettings();
	}

	public void SetFromSettings()
	{
		int num = _options.IndexOf(GetVSyncString(SaveManager.Instance.SettingsSave.VSync));
		if (num != -1)
		{
			_currentIndex = num;
		}
		else
		{
			_currentIndex = 2;
		}
		_label.SetTextAutoSize(_options[_currentIndex]);
	}

	private static string GetVSyncString(VSyncType vsyncType)
	{
		switch (vsyncType)
		{
		case VSyncType.Off:
			return new LocString("settings_ui", "VSYNC_ON").GetFormattedText();
		case VSyncType.On:
			return new LocString("settings_ui", "VSYNC_OFF").GetFormattedText();
		case VSyncType.Adaptive:
			return new LocString("settings_ui", "VSYNC_ADAPTIVE").GetFormattedText();
		default:
			Log.Error("Invalid VSync type: " + vsyncType);
			throw new ArgumentOutOfRangeException("vsyncType", vsyncType, null);
		}
	}

	protected override void OnIndexChanged(int index)
	{
		_currentIndex = index;
		_label.SetTextAutoSize(_options[index]);
		switch (index)
		{
		case 0:
			SaveManager.Instance.SettingsSave.VSync = VSyncType.Off;
			break;
		case 1:
			SaveManager.Instance.SettingsSave.VSync = VSyncType.On;
			break;
		case 2:
			SaveManager.Instance.SettingsSave.VSync = VSyncType.Adaptive;
			break;
		default:
			Log.Error($"Invalid VSync index: {index}");
			break;
		}
		NGame.ApplySyncSetting();
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(4);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetFromSettings, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.GetVSyncString, new PropertyInfo((Type)4, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)33, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("vsyncType"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnIndexChanged, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("index"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
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
		if ((ref method) == MethodName.GetVSyncString && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			string vSyncString = GetVSyncString(VariantUtils.ConvertTo<VSyncType>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<string>(ref vSyncString);
			return true;
		}
		if ((ref method) == MethodName.OnIndexChanged && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OnIndexChanged(VariantUtils.ConvertTo<int>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
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
		if ((ref method) == MethodName.GetVSyncString && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			string vSyncString = GetVSyncString(VariantUtils.ConvertTo<VSyncType>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<string>(ref vSyncString);
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
		if ((ref method) == MethodName.GetVSyncString)
		{
			return true;
		}
		if ((ref method) == MethodName.OnIndexChanged)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		base.SaveGodotObjectData(info);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
	}
}
