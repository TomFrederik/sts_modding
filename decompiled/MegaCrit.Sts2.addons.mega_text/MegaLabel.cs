using System;
using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Localization.Fonts;

namespace MegaCrit.Sts2.addons.mega_text;

[Tool]
[ScriptPath("res://addons/mega_text/MegaLabel.cs")]
public class MegaLabel : Label
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName RefreshFont = StringName.op_Implicit("RefreshFont");

		public static readonly StringName _Notification = StringName.op_Implicit("_Notification");

		public static readonly StringName SetTextAutoSize = StringName.op_Implicit("SetTextAutoSize");

		public static readonly StringName SetFontSize = StringName.op_Implicit("SetFontSize");

		public static readonly StringName AdjustFontSize = StringName.op_Implicit("AdjustFontSize");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName AutoSizeEnabled = StringName.op_Implicit("AutoSizeEnabled");

		public static readonly StringName MinFontSize = StringName.op_Implicit("MinFontSize");

		public static readonly StringName MaxFontSize = StringName.op_Implicit("MaxFontSize");

		public static readonly StringName _autoSizeEnabled = StringName.op_Implicit("_autoSizeEnabled");

		public static readonly StringName _minFontSize = StringName.op_Implicit("_minFontSize");

		public static readonly StringName _maxFontSize = StringName.op_Implicit("_maxFontSize");

		public static readonly StringName _lastSetSize = StringName.op_Implicit("_lastSetSize");

		public static readonly StringName _lastAdjustedSize = StringName.op_Implicit("_lastAdjustedSize");
	}

	public class SignalName : SignalName
	{
	}

	private static readonly TextParagraph _cachedParagraph = new TextParagraph();

	private const float _sizeComparisonEpsilon = 0.01f;

	private bool _autoSizeEnabled = true;

	private int _minFontSize = 8;

	private int _maxFontSize = 100;

	private int _lastSetSize;

	private Vector2 _lastAdjustedSize;

	[Export(/*Could not decode attribute arguments.*/)]
	public bool AutoSizeEnabled
	{
		get
		{
			return _autoSizeEnabled;
		}
		set
		{
			if (_autoSizeEnabled != value)
			{
				_autoSizeEnabled = value;
				if (Engine.IsEditorHint())
				{
					AdjustFontSize();
				}
			}
		}
	}

	[Export(/*Could not decode attribute arguments.*/)]
	public int MinFontSize
	{
		get
		{
			return _minFontSize;
		}
		set
		{
			if (_minFontSize != value)
			{
				_minFontSize = value;
				if (Engine.IsEditorHint())
				{
					AdjustFontSize();
				}
			}
		}
	}

	[Export(/*Could not decode attribute arguments.*/)]
	public int MaxFontSize
	{
		get
		{
			return _maxFontSize;
		}
		set
		{
			if (_maxFontSize != value)
			{
				_maxFontSize = value;
				if (Engine.IsEditorHint())
				{
					AdjustFontSize();
				}
			}
		}
	}

	public override void _Ready()
	{
		MegaLabelHelper.AssertThemeFontOverride((Control)(object)this, ThemeConstants.Label.font);
		RefreshFont();
		AdjustFontSize();
	}

	public void RefreshFont()
	{
		((Control)(object)this).ApplyLocaleFontSubstitution(FontType.Regular, ThemeConstants.Label.font);
	}

	public override void _Notification(int what)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		if ((long)what == 40 && !(((Vector2)(ref _lastAdjustedSize)).DistanceSquaredTo(((Control)this).Size) < 0.0001f))
		{
			AdjustFontSize();
		}
	}

	public void SetTextAutoSize(string text)
	{
		if (!(((Label)this).Text == text))
		{
			((Label)this).Text = text;
			AdjustFontSize();
		}
	}

	private void SetFontSize(int size)
	{
		if (_lastSetSize != size)
		{
			_lastSetSize = size;
			if (((Control)this).HasThemeFont(ThemeConstants.Label.font, (StringName)null))
			{
				((Control)this).AddThemeFontSizeOverride(ThemeConstants.Label.fontSize, size);
			}
		}
	}

	private void AdjustFontSize()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Invalid comparison between Unknown and I8
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		if (!AutoSizeEnabled)
		{
			return;
		}
		_lastAdjustedSize = ((Control)this).Size;
		Font themeFont = ((Control)this).GetThemeFont(ThemeConstants.Label.font, StringName.op_Implicit("Label"));
		float lineSpacing = ((Control)this).GetThemeConstant(ThemeConstants.Label.lineSpacing, StringName.op_Implicit("Label"));
		Rect2 rect = ((Control)this).GetRect();
		Vector2 size = ((Rect2)(ref rect)).Size;
		bool wrap = (long)((Label)this).AutowrapMode > 0L;
		if (!MegaLabelHelper.IsTooBig(_cachedParagraph, ((Label)this).Text, themeFont, MaxFontSize, lineSpacing, wrap, size))
		{
			SetFontSize(MaxFontSize);
			return;
		}
		if (_lastSetSize >= MinFontSize && _lastSetSize < MaxFontSize && !MegaLabelHelper.IsTooBig(_cachedParagraph, ((Label)this).Text, themeFont, _lastSetSize, lineSpacing, wrap, size) && MegaLabelHelper.IsTooBig(_cachedParagraph, ((Label)this).Text, themeFont, _lastSetSize + 1, lineSpacing, wrap, size))
		{
			SetFontSize(_lastSetSize);
			return;
		}
		int num = MinFontSize;
		int num2 = MaxFontSize;
		while (num2 >= num)
		{
			int num3 = num + (num2 - num) / 2;
			if (num3 == MaxFontSize || MegaLabelHelper.IsTooBig(_cachedParagraph, ((Label)this).Text, themeFont, num3, lineSpacing, wrap, size))
			{
				num2 = num3 - 1;
			}
			else
			{
				num = num3 + 1;
			}
		}
		SetFontSize(Math.Min(num, num2));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(6);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.RefreshFont, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Notification, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("what"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetTextAutoSize, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)4, StringName.op_Implicit("text"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetFontSize, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("size"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AdjustFontSize, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.RefreshFont && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			RefreshFont();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._Notification && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			((GodotObject)this)._Notification(VariantUtils.ConvertTo<int>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetTextAutoSize && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			SetTextAutoSize(VariantUtils.ConvertTo<string>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetFontSize && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			SetFontSize(VariantUtils.ConvertTo<int>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.AdjustFontSize && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			AdjustFontSize();
			ret = default(godot_variant);
			return true;
		}
		return ((Label)this).InvokeGodotClassMethod(ref method, args, ref ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if ((ref method) == MethodName._Ready)
		{
			return true;
		}
		if ((ref method) == MethodName.RefreshFont)
		{
			return true;
		}
		if ((ref method) == MethodName._Notification)
		{
			return true;
		}
		if ((ref method) == MethodName.SetTextAutoSize)
		{
			return true;
		}
		if ((ref method) == MethodName.SetFontSize)
		{
			return true;
		}
		if ((ref method) == MethodName.AdjustFontSize)
		{
			return true;
		}
		return ((Label)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName.AutoSizeEnabled)
		{
			AutoSizeEnabled = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName.MinFontSize)
		{
			MinFontSize = VariantUtils.ConvertTo<int>(ref value);
			return true;
		}
		if ((ref name) == PropertyName.MaxFontSize)
		{
			MaxFontSize = VariantUtils.ConvertTo<int>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._autoSizeEnabled)
		{
			_autoSizeEnabled = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._minFontSize)
		{
			_minFontSize = VariantUtils.ConvertTo<int>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._maxFontSize)
		{
			_maxFontSize = VariantUtils.ConvertTo<int>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._lastSetSize)
		{
			_lastSetSize = VariantUtils.ConvertTo<int>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._lastAdjustedSize)
		{
			_lastAdjustedSize = VariantUtils.ConvertTo<Vector2>(ref value);
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
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName.AutoSizeEnabled)
		{
			bool autoSizeEnabled = AutoSizeEnabled;
			value = VariantUtils.CreateFrom<bool>(ref autoSizeEnabled);
			return true;
		}
		if ((ref name) == PropertyName.MinFontSize)
		{
			int minFontSize = MinFontSize;
			value = VariantUtils.CreateFrom<int>(ref minFontSize);
			return true;
		}
		if ((ref name) == PropertyName.MaxFontSize)
		{
			int minFontSize = MaxFontSize;
			value = VariantUtils.CreateFrom<int>(ref minFontSize);
			return true;
		}
		if ((ref name) == PropertyName._autoSizeEnabled)
		{
			value = VariantUtils.CreateFrom<bool>(ref _autoSizeEnabled);
			return true;
		}
		if ((ref name) == PropertyName._minFontSize)
		{
			value = VariantUtils.CreateFrom<int>(ref _minFontSize);
			return true;
		}
		if ((ref name) == PropertyName._maxFontSize)
		{
			value = VariantUtils.CreateFrom<int>(ref _maxFontSize);
			return true;
		}
		if ((ref name) == PropertyName._lastSetSize)
		{
			value = VariantUtils.CreateFrom<int>(ref _lastSetSize);
			return true;
		}
		if ((ref name) == PropertyName._lastAdjustedSize)
		{
			value = VariantUtils.CreateFrom<Vector2>(ref _lastAdjustedSize);
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
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)1, PropertyName._autoSizeEnabled, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)2, PropertyName._minFontSize, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)2, PropertyName._maxFontSize, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)2, PropertyName._lastSetSize, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)5, PropertyName._lastAdjustedSize, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName.AutoSizeEnabled, (PropertyHint)0, "", (PropertyUsageFlags)4102, true));
		list.Add(new PropertyInfo((Type)2, PropertyName.MinFontSize, (PropertyHint)0, "", (PropertyUsageFlags)4102, true));
		list.Add(new PropertyInfo((Type)2, PropertyName.MaxFontSize, (PropertyHint)0, "", (PropertyUsageFlags)4102, true));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		StringName autoSizeEnabled = PropertyName.AutoSizeEnabled;
		bool autoSizeEnabled2 = AutoSizeEnabled;
		info.AddProperty(autoSizeEnabled, Variant.From<bool>(ref autoSizeEnabled2));
		StringName minFontSize = PropertyName.MinFontSize;
		int minFontSize2 = MinFontSize;
		info.AddProperty(minFontSize, Variant.From<int>(ref minFontSize2));
		StringName maxFontSize = PropertyName.MaxFontSize;
		minFontSize2 = MaxFontSize;
		info.AddProperty(maxFontSize, Variant.From<int>(ref minFontSize2));
		info.AddProperty(PropertyName._autoSizeEnabled, Variant.From<bool>(ref _autoSizeEnabled));
		info.AddProperty(PropertyName._minFontSize, Variant.From<int>(ref _minFontSize));
		info.AddProperty(PropertyName._maxFontSize, Variant.From<int>(ref _maxFontSize));
		info.AddProperty(PropertyName._lastSetSize, Variant.From<int>(ref _lastSetSize));
		info.AddProperty(PropertyName._lastAdjustedSize, Variant.From<Vector2>(ref _lastAdjustedSize));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName.AutoSizeEnabled, ref val))
		{
			AutoSizeEnabled = ((Variant)(ref val)).As<bool>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName.MinFontSize, ref val2))
		{
			MinFontSize = ((Variant)(ref val2)).As<int>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName.MaxFontSize, ref val3))
		{
			MaxFontSize = ((Variant)(ref val3)).As<int>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._autoSizeEnabled, ref val4))
		{
			_autoSizeEnabled = ((Variant)(ref val4)).As<bool>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._minFontSize, ref val5))
		{
			_minFontSize = ((Variant)(ref val5)).As<int>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._maxFontSize, ref val6))
		{
			_maxFontSize = ((Variant)(ref val6)).As<int>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName._lastSetSize, ref val7))
		{
			_lastSetSize = ((Variant)(ref val7)).As<int>();
		}
		Variant val8 = default(Variant);
		if (info.TryGetProperty(PropertyName._lastAdjustedSize, ref val8))
		{
			_lastAdjustedSize = ((Variant)(ref val8)).As<Vector2>();
		}
	}
}
