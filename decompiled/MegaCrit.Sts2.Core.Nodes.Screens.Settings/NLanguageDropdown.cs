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

namespace MegaCrit.Sts2.Core.Nodes.Screens.Settings;

[ScriptPath("res://src/Core/Nodes/Screens/Settings/NLanguageDropdown.cs")]
public class NLanguageDropdown : NSettingsDropdown
{
	public new class MethodName : NSettingsDropdown.MethodName
	{
		public new static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName PopulateOptions = StringName.op_Implicit("PopulateOptions");

		public static readonly StringName OnDropdownItemSelected = StringName.op_Implicit("OnDropdownItemSelected");

		public static readonly StringName GetLanguageNameForCode = StringName.op_Implicit("GetLanguageNameForCode");
	}

	public new class PropertyName : NSettingsDropdown.PropertyName
	{
		public static readonly StringName CurrentLanguage = StringName.op_Implicit("CurrentLanguage");

		public static readonly StringName _dropdownItemScene = StringName.op_Implicit("_dropdownItemScene");
	}

	public new class SignalName : NSettingsDropdown.SignalName
	{
	}

	[Export(/*Could not decode attribute arguments.*/)]
	private PackedScene _dropdownItemScene;

	private static readonly Dictionary<string, string> _languageCodeToName = new Dictionary<string, string>
	{
		{ "ARA", "العربية" },
		{ "BEN", "ব\u09be\u0982ল\u09be" },
		{ "CZE", "Čeština" },
		{ "DEU", "Deutsch" },
		{ "DUT", "Nederlands" },
		{ "ENG", "English" },
		{ "ESP", "Español (Latinoamérica)" },
		{ "FIL", "Filipino" },
		{ "FIN", "Suomi" },
		{ "FRA", "Français" },
		{ "GRE", "Ελληνικά" },
		{ "HIN", "ह\u093fन\u094dद\u0940" },
		{ "IND", "Bahasa Indonesia" },
		{ "ITA", "Italiano" },
		{ "JPN", "日本語" },
		{ "KOR", "한국어" },
		{ "MAL", "Bahasa Melayu" },
		{ "NOR", "Norsk" },
		{ "POL", "Polski" },
		{ "POR", "Português" },
		{ "PTB", "Português Brasileiro" },
		{ "RUS", "Русский" },
		{ "SPA", "Español (Castellano)" },
		{ "SWE", "Svenska" },
		{ "THA", "ไทย" },
		{ "TUR", "Türkçe" },
		{ "UKR", "Українська" },
		{ "VIE", "Tiếng Việt" },
		{ "ZHS", "中文" },
		{ "ZHT", "繁體中文" }
	};

	private string CurrentLanguage => LocManager.Instance.Language;

	public override void _Ready()
	{
		ConnectSignals();
		PopulateOptions();
		_currentOptionLabel.SetTextAutoSize(GetLanguageNameForCode(CurrentLanguage));
	}

	private void PopulateOptions()
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		ClearDropdownItems();
		foreach (string language in LocManager.Languages)
		{
			NLanguageDropdownItem nLanguageDropdownItem = _dropdownItemScene.Instantiate<NLanguageDropdownItem>((GenEditState)0);
			((Node)(object)_dropdownItems).AddChildSafely((Node?)(object)nLanguageDropdownItem);
			((GodotObject)nLanguageDropdownItem).Connect(NDropdownItem.SignalName.Selected, Callable.From<NDropdownItem>((Action<NDropdownItem>)OnDropdownItemSelected), 0u);
			nLanguageDropdownItem.Init(language);
		}
		((Node)_dropdownItems).GetParent<NDropdownContainer>().RefreshLayout();
	}

	private void OnDropdownItemSelected(NDropdownItem nDropdownItem)
	{
		NLanguageDropdownItem nLanguageDropdownItem = (NLanguageDropdownItem)nDropdownItem;
		if (!(nLanguageDropdownItem.LanguageCode == CurrentLanguage))
		{
			CloseDropdown();
			_currentOptionLabel.SetTextAutoSize(GetLanguageNameForCode(nLanguageDropdownItem.LanguageCode));
			SaveManager.Instance.SettingsSave.Language = nLanguageDropdownItem.LanguageCode;
			LocManager.Instance.SetLanguage(nLanguageDropdownItem.LanguageCode);
			NGame.Instance.Relocalize();
			NGame.Instance.MainMenu.OpenSettingsMenu();
		}
	}

	public static string GetLanguageNameForCode(string languageCode)
	{
		if (!_languageCodeToName.TryGetValue(languageCode.ToUpperInvariant(), out string value))
		{
			throw new InvalidOperationException("Tried to get language name for code " + languageCode + " but it doesn't exist!");
		}
		return value;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Expected O, but got Unknown
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(4);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.PopulateOptions, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnDropdownItemSelected, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("nDropdownItem"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.GetLanguageNameForCode, new PropertyInfo((Type)4, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)33, new List<PropertyInfo>
		{
			new PropertyInfo((Type)4, StringName.op_Implicit("languageCode"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.PopulateOptions && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			PopulateOptions();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnDropdownItemSelected && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OnDropdownItemSelected(VariantUtils.ConvertTo<NDropdownItem>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.GetLanguageNameForCode && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			string languageNameForCode = GetLanguageNameForCode(VariantUtils.ConvertTo<string>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<string>(ref languageNameForCode);
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
		if ((ref method) == MethodName.GetLanguageNameForCode && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			string languageNameForCode = GetLanguageNameForCode(VariantUtils.ConvertTo<string>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<string>(ref languageNameForCode);
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
		if ((ref method) == MethodName.PopulateOptions)
		{
			return true;
		}
		if ((ref method) == MethodName.OnDropdownItemSelected)
		{
			return true;
		}
		if ((ref method) == MethodName.GetLanguageNameForCode)
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
		return base.SetGodotClassPropertyValue(in name, in value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName.CurrentLanguage)
		{
			string currentLanguage = CurrentLanguage;
			value = VariantUtils.CreateFrom<string>(ref currentLanguage);
			return true;
		}
		if ((ref name) == PropertyName._dropdownItemScene)
		{
			value = VariantUtils.CreateFrom<PackedScene>(ref _dropdownItemScene);
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
		list.Add(new PropertyInfo((Type)4, PropertyName.CurrentLanguage, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName._dropdownItemScene, Variant.From<PackedScene>(ref _dropdownItemScene));
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
	}
}
