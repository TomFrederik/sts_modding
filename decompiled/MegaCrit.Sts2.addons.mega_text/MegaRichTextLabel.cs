using System;
using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.Collections;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Entities.Text;
using MegaCrit.Sts2.Core.Localization.Fonts;
using MegaCrit.Sts2.Core.RichTextTags;

namespace MegaCrit.Sts2.addons.mega_text;

[Tool]
[ScriptPath("res://addons/mega_text/MegaRichTextLabel.cs")]
public class MegaRichTextLabel : RichTextLabel
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName RefreshFont = StringName.op_Implicit("RefreshFont");

		public static readonly StringName _Notification = StringName.op_Implicit("_Notification");

		public static readonly StringName InstallEffectsIfNeeded = StringName.op_Implicit("InstallEffectsIfNeeded");

		public static readonly StringName HasEffect = StringName.op_Implicit("HasEffect");

		public static readonly StringName SetTextAutoSize = StringName.op_Implicit("SetTextAutoSize");

		public static readonly StringName AdjustFontSize = StringName.op_Implicit("AdjustFontSize");

		public static readonly StringName SetFontSize = StringName.op_Implicit("SetFontSize");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName AutoSizeEnabled = StringName.op_Implicit("AutoSizeEnabled");

		public static readonly StringName MinFontSize = StringName.op_Implicit("MinFontSize");

		public static readonly StringName MaxFontSize = StringName.op_Implicit("MaxFontSize");

		public static readonly StringName IsVerticallyBound = StringName.op_Implicit("IsVerticallyBound");

		public static readonly StringName IsHorizontallyBound = StringName.op_Implicit("IsHorizontallyBound");

		public static readonly StringName Text = StringName.op_Implicit("Text");

		public static readonly StringName _isAutoSizeEnabled = StringName.op_Implicit("_isAutoSizeEnabled");

		public static readonly StringName _minFontSize = StringName.op_Implicit("_minFontSize");

		public static readonly StringName _maxFontSize = StringName.op_Implicit("_maxFontSize");

		public static readonly StringName _lastSetSize = StringName.op_Implicit("_lastSetSize");

		public static readonly StringName _isVerticallyBound = StringName.op_Implicit("_isVerticallyBound");

		public static readonly StringName _isHorizontallyBound = StringName.op_Implicit("_isHorizontallyBound");

		public static readonly StringName _needsResize = StringName.op_Implicit("_needsResize");

		public static readonly StringName _effectsInstalled = StringName.op_Implicit("_effectsInstalled");

		public static readonly StringName _lastAdjustedSize = StringName.op_Implicit("_lastAdjustedSize");

		public static readonly StringName _isAutoSizing = StringName.op_Implicit("_isAutoSizing");
	}

	public class SignalName : SignalName
	{
	}

	private static readonly TextParagraph _cachedParagraph = new TextParagraph();

	private const float _sizeComparisonEpsilon = 0.01f;

	private bool _isAutoSizeEnabled = true;

	private int _minFontSize = 8;

	private int _maxFontSize = 100;

	private int _lastSetSize;

	private bool _isVerticallyBound = true;

	private bool _isHorizontallyBound;

	private bool _needsResize = true;

	private bool _effectsInstalled;

	private Vector2 _lastAdjustedSize;

	private static readonly AbstractMegaRichTextEffect[] _textEffects = new AbstractMegaRichTextEffect[13]
	{
		new RichTextAqua(),
		new RichTextBlue(),
		new RichTextFadeIn(),
		new RichTextFlyIn(),
		new RichTextGold(),
		new RichTextGreen(),
		new RichTextJitter(),
		new RichTextOrange(),
		new RichTextPink(),
		new RichTextPurple(),
		new RichTextRed(),
		new RichTextSine(),
		new RichTextThinkyDots()
	};

	private bool _isAutoSizing;

	[Export(/*Could not decode attribute arguments.*/)]
	public bool AutoSizeEnabled
	{
		get
		{
			return _isAutoSizeEnabled;
		}
		set
		{
			if (value && ((RichTextLabel)this).FitContent)
			{
				GD.PushWarning("Auto Size is not compatible with Fit Content, disabling Auto Size...");
				_isAutoSizeEnabled = false;
			}
			else if (AutoSizeEnabled != value)
			{
				_isAutoSizeEnabled = value;
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

	[Export(/*Could not decode attribute arguments.*/)]
	public bool IsVerticallyBound
	{
		get
		{
			return _isVerticallyBound;
		}
		set
		{
			_isVerticallyBound = value;
			if (Engine.IsEditorHint())
			{
				AdjustFontSize();
			}
		}
	}

	[Export(/*Could not decode attribute arguments.*/)]
	public bool IsHorizontallyBound
	{
		get
		{
			return _isHorizontallyBound;
		}
		set
		{
			_isHorizontallyBound = value;
			if (Engine.IsEditorHint())
			{
				AdjustFontSize();
			}
		}
	}

	public string Text
	{
		get
		{
			return ((RichTextLabel)this).Text;
		}
		set
		{
			SetTextAutoSize(value);
		}
	}

	public override void _Ready()
	{
		MegaLabelHelper.AssertThemeFontOverride((Control)(object)this, ThemeConstants.RichTextLabel.normalFont);
		RefreshFont();
		InstallEffectsIfNeeded();
		AdjustFontSize();
		((RichTextLabel)this).ParseBbcode(Text);
	}

	public void RefreshFont()
	{
		((Control)(object)this).ApplyLocaleFontSubstitution(FontType.Regular, ThemeConstants.RichTextLabel.normalFont);
		((Control)(object)this).ApplyLocaleFontSubstitution(FontType.Bold, ThemeConstants.RichTextLabel.boldFont);
		((Control)(object)this).ApplyLocaleFontSubstitution(FontType.Italic, ThemeConstants.RichTextLabel.italicsFont);
	}

	public override void _Notification(int what)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		switch (what)
		{
		case 40:
			if (!(((Vector2)(ref _lastAdjustedSize)).DistanceSquaredTo(((Control)this).Size) < 0.0001f) && AutoSizeEnabled)
			{
				_needsResize = true;
				AdjustFontSize();
			}
			break;
		case 9001:
			((RichTextLabel)this).CustomEffects.Clear();
			break;
		case 9002:
			InstallEffectsIfNeeded();
			break;
		}
	}

	private void InstallEffectsIfNeeded()
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Expected O, but got Unknown
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		if ((!_effectsInstalled || ((RichTextLabel)this).CustomEffects.Count <= 0) && ((RichTextLabel)this).BbcodeEnabled)
		{
			Array val = new Array();
			AbstractMegaRichTextEffect[] textEffects = _textEffects;
			foreach (AbstractMegaRichTextEffect abstractMegaRichTextEffect in textEffects)
			{
				val.Add(Variant.op_Implicit((GodotObject)(object)abstractMegaRichTextEffect));
			}
			((RichTextLabel)this).CustomEffects = val;
			_effectsInstalled = true;
		}
	}

	private bool HasEffect(AbstractMegaRichTextEffect effect)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return ((RichTextLabel)this).CustomEffects.Contains(Variant.op_Implicit((GodotObject)(object)effect));
	}

	public void SetTextAutoSize(string text)
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		if (!(((RichTextLabel)this).Text == text))
		{
			((RichTextLabel)this).Text = text;
			InstallEffectsIfNeeded();
			if (AutoSizeEnabled)
			{
				_needsResize = true;
				((GodotObject)this).CallDeferred(StringName.op_Implicit("AdjustFontSize"), Array.Empty<Variant>());
			}
		}
	}

	private void AdjustFontSize()
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		if (!AutoSizeEnabled || _isAutoSizing || !_needsResize)
		{
			return;
		}
		_isAutoSizing = true;
		try
		{
			_needsResize = true;
			_lastAdjustedSize = ((Control)this).Size;
			Font themeFont = ((Control)this).GetThemeFont(ThemeConstants.RichTextLabel.normalFont, StringName.op_Implicit("RichTextLabel"));
			float lineSpacing = ((Control)this).GetThemeConstant(ThemeConstants.RichTextLabel.lineSpacing, StringName.op_Implicit("RichTextLabel"));
			Rect2 rect = ((Control)this).GetRect();
			Vector2 size = ((Rect2)(ref rect)).Size;
			List<BbcodeObject> objs = MegaLabelHelper.ParseBbcode(Text);
			if (!MegaLabelHelper.IsTooBig(_cachedParagraph, objs, themeFont, MaxFontSize, lineSpacing, size, _isHorizontallyBound, _isVerticallyBound))
			{
				SetFontSize(MaxFontSize);
				return;
			}
			if (_lastSetSize >= MinFontSize && _lastSetSize < MaxFontSize && !MegaLabelHelper.IsTooBig(_cachedParagraph, objs, themeFont, _lastSetSize, lineSpacing, size, _isHorizontallyBound, _isVerticallyBound) && MegaLabelHelper.IsTooBig(_cachedParagraph, objs, themeFont, _lastSetSize + 1, lineSpacing, size, _isHorizontallyBound, _isVerticallyBound))
			{
				SetFontSize(_lastSetSize);
				return;
			}
			int num = MinFontSize;
			int num2 = MaxFontSize;
			while (num2 >= num)
			{
				int num3 = num + (num2 - num) / 2;
				if (MegaLabelHelper.IsTooBig(_cachedParagraph, objs, themeFont, num3, lineSpacing, size, _isHorizontallyBound, _isVerticallyBound))
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
		finally
		{
			_isAutoSizing = false;
		}
	}

	private void SetFontSize(int size)
	{
		if (_lastSetSize != size)
		{
			_lastSetSize = size;
			((Control)this).AddThemeFontSizeOverride(ThemeConstants.RichTextLabel.normalFontSize, size);
			if (((RichTextLabel)this).BbcodeEnabled)
			{
				((Control)this).AddThemeFontSizeOverride(ThemeConstants.RichTextLabel.boldFontSize, size);
				((Control)this).AddThemeFontSizeOverride(ThemeConstants.RichTextLabel.boldItalicsFontSize, size);
				((Control)this).AddThemeFontSizeOverride(ThemeConstants.RichTextLabel.italicsFontSize, size);
				((Control)this).AddThemeFontSizeOverride(ThemeConstants.RichTextLabel.monoFontSize, size);
				((RichTextLabel)this).ParseBbcode(Text);
			}
		}
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
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Expected O, but got Unknown
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(8);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.RefreshFont, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Notification, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("what"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.InstallEffectsIfNeeded, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.HasEffect, new PropertyInfo((Type)1, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("effect"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("RichTextEffect"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetTextAutoSize, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)4, StringName.op_Implicit("text"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AdjustFontSize, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetFontSize, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("size"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
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
		if ((ref method) == MethodName.InstallEffectsIfNeeded && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			InstallEffectsIfNeeded();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.HasEffect && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			bool flag = HasEffect(VariantUtils.ConvertTo<AbstractMegaRichTextEffect>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<bool>(ref flag);
			return true;
		}
		if ((ref method) == MethodName.SetTextAutoSize && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			SetTextAutoSize(VariantUtils.ConvertTo<string>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.AdjustFontSize && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			AdjustFontSize();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetFontSize && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			SetFontSize(VariantUtils.ConvertTo<int>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		return ((RichTextLabel)this).InvokeGodotClassMethod(ref method, args, ref ret);
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
		if ((ref method) == MethodName.InstallEffectsIfNeeded)
		{
			return true;
		}
		if ((ref method) == MethodName.HasEffect)
		{
			return true;
		}
		if ((ref method) == MethodName.SetTextAutoSize)
		{
			return true;
		}
		if ((ref method) == MethodName.AdjustFontSize)
		{
			return true;
		}
		if ((ref method) == MethodName.SetFontSize)
		{
			return true;
		}
		return ((RichTextLabel)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
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
		if ((ref name) == PropertyName.IsVerticallyBound)
		{
			IsVerticallyBound = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName.IsHorizontallyBound)
		{
			IsHorizontallyBound = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName.Text)
		{
			Text = VariantUtils.ConvertTo<string>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._isAutoSizeEnabled)
		{
			_isAutoSizeEnabled = VariantUtils.ConvertTo<bool>(ref value);
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
		if ((ref name) == PropertyName._isVerticallyBound)
		{
			_isVerticallyBound = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._isHorizontallyBound)
		{
			_isHorizontallyBound = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._needsResize)
		{
			_needsResize = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._effectsInstalled)
		{
			_effectsInstalled = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._lastAdjustedSize)
		{
			_lastAdjustedSize = VariantUtils.ConvertTo<Vector2>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._isAutoSizing)
		{
			_isAutoSizing = VariantUtils.ConvertTo<bool>(ref value);
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
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
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
		if ((ref name) == PropertyName.IsVerticallyBound)
		{
			bool autoSizeEnabled = IsVerticallyBound;
			value = VariantUtils.CreateFrom<bool>(ref autoSizeEnabled);
			return true;
		}
		if ((ref name) == PropertyName.IsHorizontallyBound)
		{
			bool autoSizeEnabled = IsHorizontallyBound;
			value = VariantUtils.CreateFrom<bool>(ref autoSizeEnabled);
			return true;
		}
		if ((ref name) == PropertyName.Text)
		{
			string text = Text;
			value = VariantUtils.CreateFrom<string>(ref text);
			return true;
		}
		if ((ref name) == PropertyName._isAutoSizeEnabled)
		{
			value = VariantUtils.CreateFrom<bool>(ref _isAutoSizeEnabled);
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
		if ((ref name) == PropertyName._isVerticallyBound)
		{
			value = VariantUtils.CreateFrom<bool>(ref _isVerticallyBound);
			return true;
		}
		if ((ref name) == PropertyName._isHorizontallyBound)
		{
			value = VariantUtils.CreateFrom<bool>(ref _isHorizontallyBound);
			return true;
		}
		if ((ref name) == PropertyName._needsResize)
		{
			value = VariantUtils.CreateFrom<bool>(ref _needsResize);
			return true;
		}
		if ((ref name) == PropertyName._effectsInstalled)
		{
			value = VariantUtils.CreateFrom<bool>(ref _effectsInstalled);
			return true;
		}
		if ((ref name) == PropertyName._lastAdjustedSize)
		{
			value = VariantUtils.CreateFrom<Vector2>(ref _lastAdjustedSize);
			return true;
		}
		if ((ref name) == PropertyName._isAutoSizing)
		{
			value = VariantUtils.CreateFrom<bool>(ref _isAutoSizing);
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
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)1, PropertyName._isAutoSizeEnabled, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)2, PropertyName._minFontSize, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)2, PropertyName._maxFontSize, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)2, PropertyName._lastSetSize, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._isVerticallyBound, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._isHorizontallyBound, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._needsResize, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._effectsInstalled, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)5, PropertyName._lastAdjustedSize, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName.AutoSizeEnabled, (PropertyHint)0, "", (PropertyUsageFlags)4102, true));
		list.Add(new PropertyInfo((Type)2, PropertyName.MinFontSize, (PropertyHint)0, "", (PropertyUsageFlags)4102, true));
		list.Add(new PropertyInfo((Type)2, PropertyName.MaxFontSize, (PropertyHint)0, "", (PropertyUsageFlags)4102, true));
		list.Add(new PropertyInfo((Type)1, PropertyName.IsVerticallyBound, (PropertyHint)0, "", (PropertyUsageFlags)4102, true));
		list.Add(new PropertyInfo((Type)1, PropertyName.IsHorizontallyBound, (PropertyHint)0, "", (PropertyUsageFlags)4102, true));
		list.Add(new PropertyInfo((Type)1, PropertyName._isAutoSizing, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)4, PropertyName.Text, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
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
		StringName isVerticallyBound = PropertyName.IsVerticallyBound;
		autoSizeEnabled2 = IsVerticallyBound;
		info.AddProperty(isVerticallyBound, Variant.From<bool>(ref autoSizeEnabled2));
		StringName isHorizontallyBound = PropertyName.IsHorizontallyBound;
		autoSizeEnabled2 = IsHorizontallyBound;
		info.AddProperty(isHorizontallyBound, Variant.From<bool>(ref autoSizeEnabled2));
		StringName text = PropertyName.Text;
		string text2 = Text;
		info.AddProperty(text, Variant.From<string>(ref text2));
		info.AddProperty(PropertyName._isAutoSizeEnabled, Variant.From<bool>(ref _isAutoSizeEnabled));
		info.AddProperty(PropertyName._minFontSize, Variant.From<int>(ref _minFontSize));
		info.AddProperty(PropertyName._maxFontSize, Variant.From<int>(ref _maxFontSize));
		info.AddProperty(PropertyName._lastSetSize, Variant.From<int>(ref _lastSetSize));
		info.AddProperty(PropertyName._isVerticallyBound, Variant.From<bool>(ref _isVerticallyBound));
		info.AddProperty(PropertyName._isHorizontallyBound, Variant.From<bool>(ref _isHorizontallyBound));
		info.AddProperty(PropertyName._needsResize, Variant.From<bool>(ref _needsResize));
		info.AddProperty(PropertyName._effectsInstalled, Variant.From<bool>(ref _effectsInstalled));
		info.AddProperty(PropertyName._lastAdjustedSize, Variant.From<Vector2>(ref _lastAdjustedSize));
		info.AddProperty(PropertyName._isAutoSizing, Variant.From<bool>(ref _isAutoSizing));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
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
		if (info.TryGetProperty(PropertyName.IsVerticallyBound, ref val4))
		{
			IsVerticallyBound = ((Variant)(ref val4)).As<bool>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName.IsHorizontallyBound, ref val5))
		{
			IsHorizontallyBound = ((Variant)(ref val5)).As<bool>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName.Text, ref val6))
		{
			Text = ((Variant)(ref val6)).As<string>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName._isAutoSizeEnabled, ref val7))
		{
			_isAutoSizeEnabled = ((Variant)(ref val7)).As<bool>();
		}
		Variant val8 = default(Variant);
		if (info.TryGetProperty(PropertyName._minFontSize, ref val8))
		{
			_minFontSize = ((Variant)(ref val8)).As<int>();
		}
		Variant val9 = default(Variant);
		if (info.TryGetProperty(PropertyName._maxFontSize, ref val9))
		{
			_maxFontSize = ((Variant)(ref val9)).As<int>();
		}
		Variant val10 = default(Variant);
		if (info.TryGetProperty(PropertyName._lastSetSize, ref val10))
		{
			_lastSetSize = ((Variant)(ref val10)).As<int>();
		}
		Variant val11 = default(Variant);
		if (info.TryGetProperty(PropertyName._isVerticallyBound, ref val11))
		{
			_isVerticallyBound = ((Variant)(ref val11)).As<bool>();
		}
		Variant val12 = default(Variant);
		if (info.TryGetProperty(PropertyName._isHorizontallyBound, ref val12))
		{
			_isHorizontallyBound = ((Variant)(ref val12)).As<bool>();
		}
		Variant val13 = default(Variant);
		if (info.TryGetProperty(PropertyName._needsResize, ref val13))
		{
			_needsResize = ((Variant)(ref val13)).As<bool>();
		}
		Variant val14 = default(Variant);
		if (info.TryGetProperty(PropertyName._effectsInstalled, ref val14))
		{
			_effectsInstalled = ((Variant)(ref val14)).As<bool>();
		}
		Variant val15 = default(Variant);
		if (info.TryGetProperty(PropertyName._lastAdjustedSize, ref val15))
		{
			_lastAdjustedSize = ((Variant)(ref val15)).As<Vector2>();
		}
		Variant val16 = default(Variant);
		if (info.TryGetProperty(PropertyName._isAutoSizing, ref val16))
		{
			_isAutoSizing = ((Variant)(ref val16)).As<bool>();
		}
	}
}
