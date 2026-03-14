using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Saves;

namespace MegaCrit.Sts2.Core.Nodes.Screens.Settings;

[ScriptPath("res://src/Core/Nodes/Screens/Settings/NHandCardCountTickbox.cs")]
public class NHandCardCountTickbox : NSettingsTickbox, IResettableSettingNode
{
	public new class MethodName : NSettingsTickbox.MethodName
	{
		public new static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName SetFromSettings = StringName.op_Implicit("SetFromSettings");

		public new static readonly StringName OnTick = StringName.op_Implicit("OnTick");

		public new static readonly StringName OnUntick = StringName.op_Implicit("OnUntick");

		public static readonly StringName TryRefreshHandIndices = StringName.op_Implicit("TryRefreshHandIndices");
	}

	public new class PropertyName : NSettingsTickbox.PropertyName
	{
		public new static readonly StringName _settingsScreen = StringName.op_Implicit("_settingsScreen");
	}

	public new class SignalName : NSettingsTickbox.SignalName
	{
	}

	private NSettingsScreen _settingsScreen;

	public override void _Ready()
	{
		ConnectSignals();
		_settingsScreen = ((Node)(object)this).GetAncestorOfType<NSettingsScreen>();
		SetFromSettings();
	}

	public void SetFromSettings()
	{
		base.IsTicked = SaveManager.Instance.PrefsSave.ShowCardIndices;
	}

	protected override void OnTick()
	{
		_settingsScreen.ShowToast(new LocString("settings_ui", "TOAST_HAND_CARD_COUNT_ON"));
		SaveManager.Instance.PrefsSave.ShowCardIndices = true;
		TryRefreshHandIndices();
	}

	protected override void OnUntick()
	{
		_settingsScreen.ShowToast(new LocString("settings_ui", "TOAST_HAND_CARD_COUNT_OFF"));
		SaveManager.Instance.PrefsSave.ShowCardIndices = false;
		TryRefreshHandIndices();
	}

	private static void TryRefreshHandIndices()
	{
		if (NPlayerHand.Instance != null)
		{
			NPlayerHand.Instance.ForceRefreshCardIndices();
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(5);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetFromSettings, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnTick, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnUntick, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.TryRefreshHandIndices, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)33, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
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
		if ((ref method) == MethodName.OnTick && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnTick();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnUntick && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnUntick();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.TryRefreshHandIndices && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			TryRefreshHandIndices();
			ret = default(godot_variant);
			return true;
		}
		return base.InvokeGodotClassMethod(in method, args, out ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static bool InvokeGodotClassStaticMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.TryRefreshHandIndices && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			TryRefreshHandIndices();
			ret = default(godot_variant);
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
		if ((ref method) == MethodName.OnTick)
		{
			return true;
		}
		if ((ref method) == MethodName.OnUntick)
		{
			return true;
		}
		if ((ref method) == MethodName.TryRefreshHandIndices)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._settingsScreen)
		{
			_settingsScreen = VariantUtils.ConvertTo<NSettingsScreen>(ref value);
			return true;
		}
		return base.SetGodotClassPropertyValue(in name, in value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName._settingsScreen)
		{
			value = VariantUtils.CreateFrom<NSettingsScreen>(ref _settingsScreen);
			return true;
		}
		return base.GetGodotClassPropertyValue(in name, out value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._settingsScreen, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName._settingsScreen, Variant.From<NSettingsScreen>(ref _settingsScreen));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._settingsScreen, ref val))
		{
			_settingsScreen = ((Variant)(ref val)).As<NSettingsScreen>();
		}
	}
}
