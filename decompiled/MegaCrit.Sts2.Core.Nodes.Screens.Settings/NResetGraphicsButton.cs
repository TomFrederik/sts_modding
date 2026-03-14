using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Multiplayer;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.Settings;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.Nodes.Screens.Settings;

[ScriptPath("res://src/Core/Nodes/Screens/Settings/NResetGraphicsButton.cs")]
public class NResetGraphicsButton : NSettingsButton
{
	public new class MethodName : NSettingsButton.MethodName
	{
		public new static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public new static readonly StringName OnRelease = StringName.op_Implicit("OnRelease");
	}

	public new class PropertyName : NSettingsButton.PropertyName
	{
	}

	public new class SignalName : NSettingsButton.SignalName
	{
	}

	public override void _Ready()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		ConnectSignals();
		((Control)this).PivotOffset = ((Control)this).Size * 0.5f;
		((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("Label")).SetTextAutoSize(new LocString("settings_ui", "RESET_SETTINGS_BUTTON").GetFormattedText());
	}

	private async Task ResetSettingsAfterConfirmation()
	{
		NGenericPopup nGenericPopup = NGenericPopup.Create();
		NModalContainer.Instance.Add((Node)(object)nGenericPopup);
		if (!(await nGenericPopup.WaitForConfirmation(new LocString("settings_ui", "RESET_GRAPHICS_CONFIRMATION.body"), new LocString("settings_ui", "RESET_CONFIRMATION.header"), new LocString("main_menu_ui", "GENERIC_POPUP.cancel"), new LocString("main_menu_ui", "GENERIC_POPUP.confirm"))))
		{
			return;
		}
		Log.Info("Player reset graphics settings");
		SettingsSave settingsSave = SaveManager.Instance.SettingsSave;
		settingsSave.FpsLimit = 60;
		settingsSave.WindowPosition = new Vector2I(-1, -1);
		settingsSave.WindowSize = new Vector2I(1920, 1080);
		settingsSave.Fullscreen = true;
		settingsSave.AspectRatioSetting = AspectRatioSetting.SixteenByNine;
		settingsSave.TargetDisplay = -1;
		settingsSave.ResizeWindows = true;
		settingsSave.VSync = VSyncType.Adaptive;
		settingsSave.Msaa = 2;
		NGame.Instance.ApplyDisplaySettings();
		NSettingsPanel ancestorOfType = ((Node)(object)this).GetAncestorOfType<NSettingsPanel>();
		foreach (IResettableSettingNode item in ((Node)(object)ancestorOfType).GetChildrenRecursive<IResettableSettingNode>())
		{
			item.SetFromSettings();
		}
	}

	protected override void OnRelease()
	{
		base.OnRelease();
		TaskHelper.RunSafely(ResetSettingsAfterConfirmation());
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(2);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnRelease, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
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
		if ((ref method) == MethodName.OnRelease && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnRelease();
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
		if ((ref method) == MethodName.OnRelease)
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
