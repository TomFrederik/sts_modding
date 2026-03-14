using System;
using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.TestSupport;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.Nodes.Screens.ModdingScreen;

[ScriptPath("res://src/Core/Nodes/Screens/ModdingScreen/NModMenuRow.cs")]
public class NModMenuRow : NClickableControl
{
	public new class MethodName : NClickableControl.MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public new static readonly StringName OnFocus = StringName.op_Implicit("OnFocus");

		public new static readonly StringName OnUnfocus = StringName.op_Implicit("OnUnfocus");

		public new static readonly StringName OnRelease = StringName.op_Implicit("OnRelease");

		public static readonly StringName SetSelected = StringName.op_Implicit("SetSelected");

		public static readonly StringName OnTickboxToggled = StringName.op_Implicit("OnTickboxToggled");

		public static readonly StringName GetPlatformIcon = StringName.op_Implicit("GetPlatformIcon");
	}

	public new class PropertyName : NClickableControl.PropertyName
	{
		public static readonly StringName _selectionHighlight = StringName.op_Implicit("_selectionHighlight");

		public static readonly StringName _tickbox = StringName.op_Implicit("_tickbox");

		public static readonly StringName _screen = StringName.op_Implicit("_screen");

		public static readonly StringName _isSelected = StringName.op_Implicit("_isSelected");
	}

	public new class SignalName : NClickableControl.SignalName
	{
	}

	private static readonly string _scenePath = SceneHelper.GetScenePath("screens/modding/modding_screen_row");

	private const float _selectedAlpha = 0.25f;

	private Panel _selectionHighlight;

	private NTickbox _tickbox;

	private NModdingScreen _screen;

	private bool _isSelected;

	public Mod? Mod { get; private set; }

	public static NModMenuRow? Create(NModdingScreen screen, Mod mod)
	{
		if (TestMode.IsOn)
		{
			return null;
		}
		NModMenuRow nModMenuRow = PreloadManager.Cache.GetScene(_scenePath).Instantiate<NModMenuRow>((GenEditState)0);
		nModMenuRow.Mod = mod;
		nModMenuRow._screen = screen;
		return nModMenuRow;
	}

	public override void _Ready()
	{
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		if (Mod != null)
		{
			_selectionHighlight = ((Node)this).GetNode<Panel>(NodePath.op_Implicit("SelectionHighlight"));
			NTickbox node = ((Node)this).GetNode<NTickbox>(NodePath.op_Implicit("Tickbox"));
			MegaRichTextLabel node2 = ((Node)this).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("Title"));
			TextureRect node3 = ((Node)this).GetNode<TextureRect>(NodePath.op_Implicit("PlatformIcon"));
			Panel selectionHighlight = _selectionHighlight;
			Color modulate = ((CanvasItem)_selectionHighlight).Modulate;
			modulate.A = 0f;
			((CanvasItem)selectionHighlight).Modulate = modulate;
			node.IsTicked = !(SaveManager.Instance.SettingsSave.ModSettings?.IsModDisabled(Mod) ?? false);
			((GodotObject)node).Connect(NTickbox.SignalName.Toggled, Callable.From<NTickbox>((Action<NTickbox>)OnTickboxToggled), 0u);
			node2.Text = Mod.manifest?.name ?? Mod.pckName;
			node3.Texture = GetPlatformIcon(Mod.modSource);
			((CanvasItem)node2).Modulate = (Mod.wasLoaded ? Colors.White : StsColors.gray);
			((CanvasItem)node3).Modulate = (Mod.wasLoaded ? Colors.White : StsColors.gray);
			ConnectSignals();
		}
	}

	protected override void OnFocus()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		if (!_isSelected)
		{
			Panel selectionHighlight = _selectionHighlight;
			Color darkBlue = StsColors.darkBlue;
			darkBlue.A = 0.25f;
			((CanvasItem)selectionHighlight).Modulate = darkBlue;
		}
	}

	protected override void OnUnfocus()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		if (!_isSelected)
		{
			((CanvasItem)_selectionHighlight).Modulate = Colors.Transparent;
		}
	}

	protected override void OnRelease()
	{
		_screen.OnRowSelected(this);
	}

	public void SetSelected(bool isSelected)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		if (_isSelected != isSelected)
		{
			_isSelected = isSelected;
			if (_isSelected)
			{
				Panel selectionHighlight = _selectionHighlight;
				Color blue = StsColors.blue;
				blue.A = 0.25f;
				((CanvasItem)selectionHighlight).Modulate = blue;
			}
			else if (base.IsFocused)
			{
				Panel selectionHighlight2 = _selectionHighlight;
				Color blue = StsColors.darkBlue;
				blue.A = 0.25f;
				((CanvasItem)selectionHighlight2).Modulate = blue;
			}
			else
			{
				((CanvasItem)_selectionHighlight).Modulate = Colors.Transparent;
			}
		}
	}

	private void OnTickboxToggled(NTickbox tickbox)
	{
		SettingsSave settingsSave = SaveManager.Instance.SettingsSave;
		if (settingsSave.ModSettings == null)
		{
			ModSettings modSettings = (settingsSave.ModSettings = new ModSettings());
		}
		if (tickbox.IsTicked)
		{
			SaveManager.Instance.SettingsSave.ModSettings.DisabledMods.RemoveAll((DisabledMod m) => m.Name == Mod.pckName && m.Source == Mod.modSource);
		}
		else
		{
			SaveManager.Instance.SettingsSave.ModSettings.DisabledMods.Add(new DisabledMod(Mod));
		}
		_screen.OnModEnabledOrDisabled();
	}

	public static Texture2D GetPlatformIcon(ModSource modSource)
	{
		AssetCache cache = PreloadManager.Cache;
		return cache.GetTexture2D(modSource switch
		{
			ModSource.ModsDirectory => ImageHelper.GetImagePath("ui/mods/folder.png"), 
			ModSource.SteamWorkshop => ImageHelper.GetImagePath("ui/mods/steam_logo.png"), 
			_ => throw new ArgumentOutOfRangeException("modSource", modSource, null), 
		});
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
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Expected O, but got Unknown
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Expected O, but got Unknown
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(7);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnFocus, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnUnfocus, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnRelease, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetSelected, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)1, StringName.op_Implicit("isSelected"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnTickboxToggled, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("tickbox"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.GetPlatformIcon, new PropertyInfo((Type)24, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Texture2D"), false), (MethodFlags)33, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("modSource"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnFocus && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnFocus();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnUnfocus && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnUnfocus();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnRelease && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnRelease();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetSelected && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			SetSelected(VariantUtils.ConvertTo<bool>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnTickboxToggled && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OnTickboxToggled(VariantUtils.ConvertTo<NTickbox>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.GetPlatformIcon && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			Texture2D platformIcon = GetPlatformIcon(VariantUtils.ConvertTo<ModSource>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<Texture2D>(ref platformIcon);
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
		if ((ref method) == MethodName.GetPlatformIcon && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			Texture2D platformIcon = GetPlatformIcon(VariantUtils.ConvertTo<ModSource>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<Texture2D>(ref platformIcon);
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
		if ((ref method) == MethodName.OnFocus)
		{
			return true;
		}
		if ((ref method) == MethodName.OnUnfocus)
		{
			return true;
		}
		if ((ref method) == MethodName.OnRelease)
		{
			return true;
		}
		if ((ref method) == MethodName.SetSelected)
		{
			return true;
		}
		if ((ref method) == MethodName.OnTickboxToggled)
		{
			return true;
		}
		if ((ref method) == MethodName.GetPlatformIcon)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._selectionHighlight)
		{
			_selectionHighlight = VariantUtils.ConvertTo<Panel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._tickbox)
		{
			_tickbox = VariantUtils.ConvertTo<NTickbox>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._screen)
		{
			_screen = VariantUtils.ConvertTo<NModdingScreen>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._isSelected)
		{
			_isSelected = VariantUtils.ConvertTo<bool>(ref value);
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
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName._selectionHighlight)
		{
			value = VariantUtils.CreateFrom<Panel>(ref _selectionHighlight);
			return true;
		}
		if ((ref name) == PropertyName._tickbox)
		{
			value = VariantUtils.CreateFrom<NTickbox>(ref _tickbox);
			return true;
		}
		if ((ref name) == PropertyName._screen)
		{
			value = VariantUtils.CreateFrom<NModdingScreen>(ref _screen);
			return true;
		}
		if ((ref name) == PropertyName._isSelected)
		{
			value = VariantUtils.CreateFrom<bool>(ref _isSelected);
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
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._selectionHighlight, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._tickbox, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._screen, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._isSelected, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName._selectionHighlight, Variant.From<Panel>(ref _selectionHighlight));
		info.AddProperty(PropertyName._tickbox, Variant.From<NTickbox>(ref _tickbox));
		info.AddProperty(PropertyName._screen, Variant.From<NModdingScreen>(ref _screen));
		info.AddProperty(PropertyName._isSelected, Variant.From<bool>(ref _isSelected));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._selectionHighlight, ref val))
		{
			_selectionHighlight = ((Variant)(ref val)).As<Panel>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._tickbox, ref val2))
		{
			_tickbox = ((Variant)(ref val2)).As<NTickbox>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._screen, ref val3))
		{
			_screen = ((Variant)(ref val3)).As<NModdingScreen>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._isSelected, ref val4))
		{
			_isSelected = ((Variant)(ref val4)).As<bool>();
		}
	}
}
