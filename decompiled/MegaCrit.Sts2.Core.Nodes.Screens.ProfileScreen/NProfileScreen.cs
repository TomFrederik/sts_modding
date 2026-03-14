using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Nodes.Screens.MainMenu;
using MegaCrit.Sts2.Core.Nodes.Screens.ScreenContext;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.TestSupport;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.Nodes.Screens.ProfileScreen;

[ScriptPath("res://src/Core/Nodes/Screens/ProfileScreen/NProfileScreen.cs")]
public class NProfileScreen : NSubmenu
{
	public new class MethodName : NSubmenu.MethodName
	{
		public static readonly StringName Create = StringName.op_Implicit("Create");

		public new static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public new static readonly StringName OnSubmenuOpened = StringName.op_Implicit("OnSubmenuOpened");

		public static readonly StringName ShowLoading = StringName.op_Implicit("ShowLoading");

		public static readonly StringName Refresh = StringName.op_Implicit("Refresh");
	}

	public new class PropertyName : NSubmenu.PropertyName
	{
		public new static readonly StringName InitialFocusedControl = StringName.op_Implicit("InitialFocusedControl");

		public static readonly StringName _loadingOverlay = StringName.op_Implicit("_loadingOverlay");
	}

	public new class SignalName : NSubmenu.SignalName
	{
	}

	public static int? forceShowProfileAsDeleted;

	private static readonly string _scenePath = SceneHelper.GetScenePath("screens/profiles/profile_screen");

	private Control _loadingOverlay;

	private readonly List<NProfileButton> _profileButtons = new List<NProfileButton>();

	private readonly List<NDeleteProfileButton> _deleteButtons = new List<NDeleteProfileButton>();

	public static string[] AssetPaths
	{
		get
		{
			List<string> list = new List<string>();
			list.Add(_scenePath);
			list.AddRange(NProfileButton.AssetPaths);
			return list.ToArray();
		}
	}

	protected override Control InitialFocusedControl => (Control)(object)_profileButtons[SaveManager.Instance.CurrentProfileId - 1];

	public static NProfileScreen? Create()
	{
		if (TestMode.IsOn)
		{
			return null;
		}
		return PreloadManager.Cache.GetScene(_scenePath).Instantiate<NProfileScreen>((GenEditState)0);
	}

	public override void _Ready()
	{
		ConnectSignals();
		_loadingOverlay = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%LoadingOverlay"));
		((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%ChooseProfileMessage")).SetTextAutoSize(new LocString("main_menu_ui", "PROFILE_SCREEN.BUTTON.chooseProfileMessage").GetFormattedText());
		_profileButtons.Add(((Node)this).GetNode<NProfileButton>(NodePath.op_Implicit("%ProfileButton1")));
		_profileButtons.Add(((Node)this).GetNode<NProfileButton>(NodePath.op_Implicit("%ProfileButton2")));
		_profileButtons.Add(((Node)this).GetNode<NProfileButton>(NodePath.op_Implicit("%ProfileButton3")));
		_deleteButtons.Add(((Node)this).GetNode<NDeleteProfileButton>(NodePath.op_Implicit("%DeleteProfileButton1")));
		_deleteButtons.Add(((Node)this).GetNode<NDeleteProfileButton>(NodePath.op_Implicit("%DeleteProfileButton2")));
		_deleteButtons.Add(((Node)this).GetNode<NDeleteProfileButton>(NodePath.op_Implicit("%DeleteProfileButton3")));
		if (_profileButtons.Count != 3)
		{
			Log.Error($"There are {_profileButtons.Count} profile buttons, but max profile count in ProfileSaveManager is {3}! This might result in subtle bugs");
		}
		for (int i = 0; i < _profileButtons.Count; i++)
		{
			((Control)_profileButtons[i]).FocusNeighborTop = ((Node)_profileButtons[i]).GetPath();
			((Control)_profileButtons[i]).FocusNeighborBottom = ((Node)_deleteButtons[i]).GetPath();
			NProfileButton nProfileButton = _profileButtons[i];
			NodePath path;
			if (i <= 0)
			{
				List<NProfileButton> profileButtons = _profileButtons;
				path = ((Node)profileButtons[profileButtons.Count - 1]).GetPath();
			}
			else
			{
				path = ((Node)_profileButtons[i - 1]).GetPath();
			}
			((Control)nProfileButton).FocusNeighborLeft = path;
			((Control)_profileButtons[i]).FocusNeighborRight = ((i < _profileButtons.Count - 1) ? ((Node)_profileButtons[i + 1]).GetPath() : ((Node)_profileButtons[0]).GetPath());
			((Control)_deleteButtons[i]).FocusNeighborTop = ((Node)_profileButtons[i]).GetPath();
			((Control)_deleteButtons[i]).FocusNeighborBottom = ((Node)_deleteButtons[i]).GetPath();
			NDeleteProfileButton nDeleteProfileButton = _deleteButtons[i];
			NodePath path2;
			if (i <= 0)
			{
				List<NDeleteProfileButton> deleteButtons = _deleteButtons;
				path2 = ((Node)deleteButtons[deleteButtons.Count - 1]).GetPath();
			}
			else
			{
				path2 = ((Node)_deleteButtons[i - 1]).GetPath();
			}
			((Control)nDeleteProfileButton).FocusNeighborLeft = path2;
			((Control)_deleteButtons[i]).FocusNeighborRight = ((i < _deleteButtons.Count - 1) ? ((Node)_deleteButtons[i + 1]).GetPath() : ((Node)_deleteButtons[0]).GetPath());
		}
	}

	public override void OnSubmenuOpened()
	{
		Refresh();
	}

	public void ShowLoading()
	{
		((CanvasItem)_loadingOverlay).Visible = true;
	}

	public void Refresh()
	{
		for (int i = 0; i < _profileButtons.Count; i++)
		{
			_profileButtons[i].Initialize(this, i + 1);
		}
		for (int j = 0; j < _deleteButtons.Count; j++)
		{
			_deleteButtons[j].Initialize(this, j + 1);
		}
		forceShowProfileAsDeleted = null;
		ActiveScreenContext.Instance.Update();
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
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(5);
		list.Add(new MethodInfo(MethodName.Create, new PropertyInfo((Type)24, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false), (MethodFlags)33, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnSubmenuOpened, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ShowLoading, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.Refresh, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.Create && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			NProfileScreen nProfileScreen = Create();
			ret = VariantUtils.CreateFrom<NProfileScreen>(ref nProfileScreen);
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
		if ((ref method) == MethodName.ShowLoading && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			ShowLoading();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.Refresh && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			Refresh();
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
			NProfileScreen nProfileScreen = Create();
			ret = VariantUtils.CreateFrom<NProfileScreen>(ref nProfileScreen);
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
		if ((ref method) == MethodName.ShowLoading)
		{
			return true;
		}
		if ((ref method) == MethodName.Refresh)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._loadingOverlay)
		{
			_loadingOverlay = VariantUtils.ConvertTo<Control>(ref value);
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
		if ((ref name) == PropertyName.InitialFocusedControl)
		{
			Control initialFocusedControl = InitialFocusedControl;
			value = VariantUtils.CreateFrom<Control>(ref initialFocusedControl);
			return true;
		}
		if ((ref name) == PropertyName._loadingOverlay)
		{
			value = VariantUtils.CreateFrom<Control>(ref _loadingOverlay);
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
		list.Add(new PropertyInfo((Type)24, PropertyName._loadingOverlay, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.InitialFocusedControl, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName._loadingOverlay, Variant.From<Control>(ref _loadingOverlay));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._loadingOverlay, ref val))
		{
			_loadingOverlay = ((Variant)(ref val)).As<Control>();
		}
	}
}
