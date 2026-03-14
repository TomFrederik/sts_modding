using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Screens.MainMenu;
using MegaCrit.Sts2.Core.Platform;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.TestSupport;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.Nodes.Screens.ModdingScreen;

[ScriptPath("res://src/Core/Nodes/Screens/ModdingScreen/NModdingScreen.cs")]
public class NModdingScreen : NSubmenu
{
	public new class MethodName : NSubmenu.MethodName
	{
		public static readonly StringName Create = StringName.op_Implicit("Create");

		public new static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public new static readonly StringName OnSubmenuOpened = StringName.op_Implicit("OnSubmenuOpened");

		public static readonly StringName OnGetModsPressed = StringName.op_Implicit("OnGetModsPressed");

		public static readonly StringName OnMakeModsPressed = StringName.op_Implicit("OnMakeModsPressed");

		public static readonly StringName OnRowSelected = StringName.op_Implicit("OnRowSelected");

		public static readonly StringName OnModEnabledOrDisabled = StringName.op_Implicit("OnModEnabledOrDisabled");

		public static readonly StringName _ExitTree = StringName.op_Implicit("_ExitTree");
	}

	public new class PropertyName : NSubmenu.PropertyName
	{
		public new static readonly StringName InitialFocusedControl = StringName.op_Implicit("InitialFocusedControl");

		public static readonly StringName _modInfoContainer = StringName.op_Implicit("_modInfoContainer");

		public static readonly StringName _modRowContainer = StringName.op_Implicit("_modRowContainer");

		public static readonly StringName _pendingChangesWarning = StringName.op_Implicit("_pendingChangesWarning");
	}

	public new class SignalName : NSubmenu.SignalName
	{
	}

	private static readonly string _scenePath = SceneHelper.GetScenePath("screens/modding/modding_screen");

	private NModInfoContainer _modInfoContainer;

	private Control _modRowContainer;

	private Control _pendingChangesWarning;

	protected override Control? InitialFocusedControl => null;

	public static string[] AssetPaths => new string[1] { _scenePath };

	public static NModdingScreen? Create()
	{
		if (TestMode.IsOn)
		{
			return null;
		}
		return PreloadManager.Cache.GetScene(_scenePath).Instantiate<NModdingScreen>((GenEditState)0);
	}

	public override void _Ready()
	{
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		_modInfoContainer = ((Node)this).GetNode<NModInfoContainer>(NodePath.op_Implicit("%ModInfoContainer"));
		_modRowContainer = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%ModsScrollContainer/Mask/Content"));
		_pendingChangesWarning = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%PendingChangesLabel"));
		NButton node = ((Node)this).GetNode<NButton>(NodePath.op_Implicit("%GetModsButton"));
		NButton node2 = ((Node)this).GetNode<NButton>(NodePath.op_Implicit("%MakeModsButton"));
		foreach (Node child in ((Node)_modRowContainer).GetChildren(false))
		{
			child.QueueFreeSafely();
		}
		foreach (Mod allMod in ModManager.AllMods)
		{
			OnNewModDetected(allMod);
		}
		((GodotObject)node).Connect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)OnGetModsPressed), 0u);
		((GodotObject)node2).Connect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)OnMakeModsPressed), 0u);
		((Node)node).GetNode<MegaLabel>(NodePath.op_Implicit("Visuals/Label")).SetTextAutoSize(new LocString("settings_ui", "MODDING_SCREEN.GET_MODS_BUTTON").GetFormattedText());
		((Node)node2).GetNode<MegaLabel>(NodePath.op_Implicit("Visuals/Label")).SetTextAutoSize(new LocString("settings_ui", "MODDING_SCREEN.MAKE_MODS_BUTTON").GetFormattedText());
		((Node)this).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("%InstalledModsTitle")).SetTextAutoSize(new LocString("settings_ui", "MODDING_SCREEN.INSTALLED_MODS_TITLE").GetFormattedText());
		((Node)this).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("%PendingChangesLabel")).SetTextAutoSize(new LocString("settings_ui", "MODDING_SCREEN.PENDING_CHANGES_WARNING").GetFormattedText());
		((CanvasItem)_pendingChangesWarning).Visible = false;
		ModManager.OnModDetected += OnNewModDetected;
		ConnectSignals();
	}

	public override void OnSubmenuOpened()
	{
		if (!ModManager.PlayerAgreedToModLoading && ModManager.AllMods.Count > 0)
		{
			NModalContainer.Instance.Add((Node)(object)NConfirmModLoadingPopup.Create());
		}
	}

	private void OnGetModsPressed(NButton _)
	{
		PlatformUtil.OpenUrl("https://steamcommunity.com/app/2868840/workshop/");
	}

	private void OnMakeModsPressed(NButton _)
	{
		PlatformUtil.OpenUrl("https://gitlab.com/megacrit/sts2/example-mod/-/wikis/home");
	}

	public void OnRowSelected(NModMenuRow row)
	{
		row.SetSelected(isSelected: true);
		_modInfoContainer.Fill(row.Mod);
		foreach (NModMenuRow item in ((IEnumerable)((Node)_modRowContainer).GetChildren(false)).OfType<NModMenuRow>())
		{
			if (item != row)
			{
				item.SetSelected(isSelected: false);
			}
		}
	}

	private void OnNewModDetected(Mod mod)
	{
		NModMenuRow child = NModMenuRow.Create(this, mod);
		((Node)(object)_modRowContainer).AddChildSafely((Node?)(object)child);
		OnModEnabledOrDisabled();
	}

	public void OnModEnabledOrDisabled()
	{
		foreach (Mod allMod in ModManager.AllMods)
		{
			bool flag = SaveManager.Instance.SettingsSave.ModSettings?.IsModDisabled(allMod) ?? false;
			if (allMod.wasLoaded == flag)
			{
				((CanvasItem)_pendingChangesWarning).Visible = true;
				return;
			}
		}
		((CanvasItem)_pendingChangesWarning).Visible = false;
	}

	public override void _ExitTree()
	{
		ModManager.OnModDetected -= OnNewModDetected;
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
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Expected O, but got Unknown
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Expected O, but got Unknown
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Expected O, but got Unknown
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(8);
		list.Add(new MethodInfo(MethodName.Create, new PropertyInfo((Type)24, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false), (MethodFlags)33, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnSubmenuOpened, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnGetModsPressed, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnMakeModsPressed, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnRowSelected, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("row"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnModEnabledOrDisabled, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._ExitTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
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
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.Create && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			NModdingScreen nModdingScreen = Create();
			ret = VariantUtils.CreateFrom<NModdingScreen>(ref nModdingScreen);
			return true;
		}
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnSubmenuOpened && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnSubmenuOpened();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnGetModsPressed && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OnGetModsPressed(VariantUtils.ConvertTo<NButton>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnMakeModsPressed && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OnMakeModsPressed(VariantUtils.ConvertTo<NButton>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnRowSelected && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OnRowSelected(VariantUtils.ConvertTo<NModMenuRow>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnModEnabledOrDisabled && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnModEnabledOrDisabled();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._ExitTree && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._ExitTree();
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
			NModdingScreen nModdingScreen = Create();
			ret = VariantUtils.CreateFrom<NModdingScreen>(ref nModdingScreen);
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
		if ((ref method) == MethodName.OnSubmenuOpened)
		{
			return true;
		}
		if ((ref method) == MethodName.OnGetModsPressed)
		{
			return true;
		}
		if ((ref method) == MethodName.OnMakeModsPressed)
		{
			return true;
		}
		if ((ref method) == MethodName.OnRowSelected)
		{
			return true;
		}
		if ((ref method) == MethodName.OnModEnabledOrDisabled)
		{
			return true;
		}
		if ((ref method) == MethodName._ExitTree)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._modInfoContainer)
		{
			_modInfoContainer = VariantUtils.ConvertTo<NModInfoContainer>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._modRowContainer)
		{
			_modRowContainer = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._pendingChangesWarning)
		{
			_pendingChangesWarning = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		return base.SetGodotClassPropertyValue(in name, in value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName.InitialFocusedControl)
		{
			Control initialFocusedControl = InitialFocusedControl;
			value = VariantUtils.CreateFrom<Control>(ref initialFocusedControl);
			return true;
		}
		if ((ref name) == PropertyName._modInfoContainer)
		{
			value = VariantUtils.CreateFrom<NModInfoContainer>(ref _modInfoContainer);
			return true;
		}
		if ((ref name) == PropertyName._modRowContainer)
		{
			value = VariantUtils.CreateFrom<Control>(ref _modRowContainer);
			return true;
		}
		if ((ref name) == PropertyName._pendingChangesWarning)
		{
			value = VariantUtils.CreateFrom<Control>(ref _pendingChangesWarning);
			return true;
		}
		return base.GetGodotClassPropertyValue(in name, out value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName.InitialFocusedControl, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._modInfoContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._modRowContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._pendingChangesWarning, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName._modInfoContainer, Variant.From<NModInfoContainer>(ref _modInfoContainer));
		info.AddProperty(PropertyName._modRowContainer, Variant.From<Control>(ref _modRowContainer));
		info.AddProperty(PropertyName._pendingChangesWarning, Variant.From<Control>(ref _pendingChangesWarning));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._modInfoContainer, ref val))
		{
			_modInfoContainer = ((Variant)(ref val)).As<NModInfoContainer>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._modRowContainer, ref val2))
		{
			_modRowContainer = ((Variant)(ref val2)).As<Control>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._pendingChangesWarning, ref val3))
		{
			_pendingChangesWarning = ((Variant)(ref val3)).As<Control>();
		}
	}
}
