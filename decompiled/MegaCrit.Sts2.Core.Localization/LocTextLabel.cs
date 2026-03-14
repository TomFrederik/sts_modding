using System;
using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;

namespace MegaCrit.Sts2.Core.Localization;

[ScriptPath("res://src/Core/Localization/LocTextLabel.cs")]
public class LocTextLabel : RichTextLabel
{
	public class MethodName : MethodName
	{
		public static readonly StringName UpdateLocalization = StringName.op_Implicit("UpdateLocalization");

		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName LocalizationTable = StringName.op_Implicit("LocalizationTable");

		public static readonly StringName LocalizationKey = StringName.op_Implicit("LocalizationKey");

		public static readonly StringName _localizationTable = StringName.op_Implicit("_localizationTable");

		public static readonly StringName _localizationKey = StringName.op_Implicit("_localizationKey");
	}

	public class SignalName : SignalName
	{
	}

	[Export(/*Could not decode attribute arguments.*/)]
	private string? _localizationTable;

	[Export(/*Could not decode attribute arguments.*/)]
	private string? _localizationKey;

	private LocString? _locString;

	public string? LocalizationTable
	{
		get
		{
			return _localizationTable;
		}
		set
		{
			if (!(_localizationTable == value))
			{
				_localizationTable = value;
				_locString = null;
				UpdateLocalization();
			}
		}
	}

	public string? LocalizationKey
	{
		get
		{
			return _localizationKey;
		}
		set
		{
			if (!(_localizationKey == value))
			{
				_localizationKey = value;
				_locString = null;
				UpdateLocalization();
			}
		}
	}

	private void UpdateLocalization()
	{
		if (_localizationTable == null)
		{
			throw new InvalidOperationException("_localizationTable is null.");
		}
		if (_localizationKey == null)
		{
			throw new InvalidOperationException("_localizationKey is null.");
		}
		if (_locString == null)
		{
			_locString = new LocString(_localizationTable, _localizationKey);
		}
		((RichTextLabel)this).Text = _locString.GetFormattedText();
	}

	public override void _Ready()
	{
		UpdateLocalization();
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(2);
		list.Add(new MethodInfo(MethodName.UpdateLocalization, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.UpdateLocalization && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			UpdateLocalization();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		return ((RichTextLabel)this).InvokeGodotClassMethod(ref method, args, ref ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if ((ref method) == MethodName.UpdateLocalization)
		{
			return true;
		}
		if ((ref method) == MethodName._Ready)
		{
			return true;
		}
		return ((RichTextLabel)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName.LocalizationTable)
		{
			LocalizationTable = VariantUtils.ConvertTo<string>(ref value);
			return true;
		}
		if ((ref name) == PropertyName.LocalizationKey)
		{
			LocalizationKey = VariantUtils.ConvertTo<string>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._localizationTable)
		{
			_localizationTable = VariantUtils.ConvertTo<string>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._localizationKey)
		{
			_localizationKey = VariantUtils.ConvertTo<string>(ref value);
			return true;
		}
		return ((GodotObject)this).SetGodotClassPropertyValue(ref name, ref value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName.LocalizationTable)
		{
			string localizationTable = LocalizationTable;
			value = VariantUtils.CreateFrom<string>(ref localizationTable);
			return true;
		}
		if ((ref name) == PropertyName.LocalizationKey)
		{
			string localizationTable = LocalizationKey;
			value = VariantUtils.CreateFrom<string>(ref localizationTable);
			return true;
		}
		if ((ref name) == PropertyName._localizationTable)
		{
			value = VariantUtils.CreateFrom<string>(ref _localizationTable);
			return true;
		}
		if ((ref name) == PropertyName._localizationKey)
		{
			value = VariantUtils.CreateFrom<string>(ref _localizationKey);
			return true;
		}
		return ((GodotObject)this).GetGodotClassPropertyValue(ref name, ref value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)4, PropertyName._localizationTable, (PropertyHint)0, "", (PropertyUsageFlags)4102, true));
		list.Add(new PropertyInfo((Type)4, PropertyName._localizationKey, (PropertyHint)0, "", (PropertyUsageFlags)4102, true));
		list.Add(new PropertyInfo((Type)4, PropertyName.LocalizationTable, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)4, PropertyName.LocalizationKey, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		StringName localizationTable = PropertyName.LocalizationTable;
		string localizationTable2 = LocalizationTable;
		info.AddProperty(localizationTable, Variant.From<string>(ref localizationTable2));
		StringName localizationKey = PropertyName.LocalizationKey;
		localizationTable2 = LocalizationKey;
		info.AddProperty(localizationKey, Variant.From<string>(ref localizationTable2));
		info.AddProperty(PropertyName._localizationTable, Variant.From<string>(ref _localizationTable));
		info.AddProperty(PropertyName._localizationKey, Variant.From<string>(ref _localizationKey));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName.LocalizationTable, ref val))
		{
			LocalizationTable = ((Variant)(ref val)).As<string>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName.LocalizationKey, ref val2))
		{
			LocalizationKey = ((Variant)(ref val2)).As<string>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._localizationTable, ref val3))
		{
			_localizationTable = ((Variant)(ref val3)).As<string>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._localizationKey, ref val4))
		{
			_localizationKey = ((Variant)(ref val4)).As<string>();
		}
	}
}
