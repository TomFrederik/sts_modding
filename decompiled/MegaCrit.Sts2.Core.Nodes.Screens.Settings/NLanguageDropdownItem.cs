using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.CommonUi;

namespace MegaCrit.Sts2.Core.Nodes.Screens.Settings;

[ScriptPath("res://src/Core/Nodes/Screens/Settings/NLanguageDropdownItem.cs")]
public class NLanguageDropdownItem : NDropdownItem
{
	public new class MethodName : NDropdownItem.MethodName
	{
		public static readonly StringName Init = StringName.op_Implicit("Init");
	}

	public new class PropertyName : NDropdownItem.PropertyName
	{
		public static readonly StringName LanguageCode = StringName.op_Implicit("LanguageCode");
	}

	public new class SignalName : NDropdownItem.SignalName
	{
	}

	public const string languageWarningIconPath = "res://images/ui/language_warning.png";

	private const string _warnImageTag = "[img]res://images/ui/language_warning.png[/img]";

	public string LanguageCode { get; private set; }

	public void Init(string languageCode)
	{
		LanguageCode = languageCode;
		string text = NLanguageDropdown.GetLanguageNameForCode(languageCode);
		float languageCompletion = LocManager.Instance.GetLanguageCompletion(languageCode);
		if (languageCompletion < 0.9f)
		{
			text = "[img]res://images/ui/language_warning.png[/img]" + text;
		}
		_richLabel.SetTextAutoSize(text);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(1);
		list.Add(new MethodInfo(MethodName.Init, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)4, StringName.op_Implicit("languageCode"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.Init && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			Init(VariantUtils.ConvertTo<string>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		return base.InvokeGodotClassMethod(in method, args, out ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if ((ref method) == MethodName.Init)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName.LanguageCode)
		{
			LanguageCode = VariantUtils.ConvertTo<string>(ref value);
			return true;
		}
		return base.SetGodotClassPropertyValue(in name, in value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName.LanguageCode)
		{
			string languageCode = LanguageCode;
			value = VariantUtils.CreateFrom<string>(ref languageCode);
			return true;
		}
		return base.GetGodotClassPropertyValue(in name, out value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)4, PropertyName.LanguageCode, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		base.SaveGodotObjectData(info);
		StringName languageCode = PropertyName.LanguageCode;
		string languageCode2 = LanguageCode;
		info.AddProperty(languageCode, Variant.From<string>(ref languageCode2));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName.LanguageCode, ref val))
		{
			LanguageCode = ((Variant)(ref val)).As<string>();
		}
	}
}
