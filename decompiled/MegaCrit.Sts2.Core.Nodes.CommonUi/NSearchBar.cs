using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Text.RegularExpressions.Generated;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;

namespace MegaCrit.Sts2.Core.Nodes.CommonUi;

[ScriptPath("res://src/Core/Nodes/CommonUi/NSearchBar.cs")]
public class NSearchBar : Control
{
	[Signal]
	public delegate void QueryChangedEventHandler(string query);

	[Signal]
	public delegate void QuerySubmittedEventHandler(string query);

	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName TextUpdated = StringName.op_Implicit("TextUpdated");

		public static readonly StringName TextSubmitted = StringName.op_Implicit("TextSubmitted");

		public static readonly StringName ClearText = StringName.op_Implicit("ClearText");

		public static readonly StringName Normalize = StringName.op_Implicit("Normalize");

		public static readonly StringName RemoveHtmlTags = StringName.op_Implicit("RemoveHtmlTags");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName Text = StringName.op_Implicit("Text");

		public static readonly StringName TextArea = StringName.op_Implicit("TextArea");

		public static readonly StringName _textArea = StringName.op_Implicit("_textArea");

		public static readonly StringName _clearButton = StringName.op_Implicit("_clearButton");
	}

	public class SignalName : SignalName
	{
		public static readonly StringName QueryChanged = StringName.op_Implicit("QueryChanged");

		public static readonly StringName QuerySubmitted = StringName.op_Implicit("QuerySubmitted");
	}

	private LineEdit _textArea;

	private NButton _clearButton;

	private QueryChangedEventHandler backing_QueryChanged;

	private QuerySubmittedEventHandler backing_QuerySubmitted;

	public string Text => _textArea.Text;

	public LineEdit TextArea => _textArea;

	public event QueryChangedEventHandler QueryChanged
	{
		add
		{
			backing_QueryChanged = (QueryChangedEventHandler)Delegate.Combine(backing_QueryChanged, value);
		}
		remove
		{
			backing_QueryChanged = (QueryChangedEventHandler)Delegate.Remove(backing_QueryChanged, value);
		}
	}

	public event QuerySubmittedEventHandler QuerySubmitted
	{
		add
		{
			backing_QuerySubmitted = (QuerySubmittedEventHandler)Delegate.Combine(backing_QuerySubmitted, value);
		}
		remove
		{
			backing_QuerySubmitted = (QuerySubmittedEventHandler)Delegate.Remove(backing_QuerySubmitted, value);
		}
	}

	[GeneratedRegex("[\\t\\r\\n]")]
	[GeneratedCode("System.Text.RegularExpressions.Generator", "9.0.12.31616")]
	private static Regex NonSpaceWhitespaceCharacters()
	{
		return _003CRegexGenerator_g_003EFDF0533EE0711AEDB383B6CD4472EB7858DDC4C8D92DA17B11D90839AAF25C6DD__NonSpaceWhitespaceCharacters_5.Instance;
	}

	[GeneratedRegex("\\s{2,}")]
	[GeneratedCode("System.Text.RegularExpressions.Generator", "9.0.12.31616")]
	private static Regex ConsecutiveSpaces()
	{
		return _003CRegexGenerator_g_003EFDF0533EE0711AEDB383B6CD4472EB7858DDC4C8D92DA17B11D90839AAF25C6DD__ConsecutiveSpaces_6.Instance;
	}

	[GeneratedRegex("<.*?>")]
	[GeneratedCode("System.Text.RegularExpressions.Generator", "9.0.12.31616")]
	private static Regex HtmlTags()
	{
		return _003CRegexGenerator_g_003EFDF0533EE0711AEDB383B6CD4472EB7858DDC4C8D92DA17B11D90839AAF25C6DD__HtmlTags_7.Instance;
	}

	public override void _Ready()
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		_textArea = ((Node)this).GetNode<LineEdit>(NodePath.op_Implicit("TextArea"));
		((GodotObject)_textArea).Connect(SignalName.TextChanged, Callable.From<string>((Action<string>)TextUpdated), 0u);
		((GodotObject)_textArea).Connect(SignalName.TextSubmitted, Callable.From<string>((Action<string>)TextSubmitted), 0u);
		_textArea.SetPlaceholder(new LocString("card_library", "SEARCH_PLACEHOLDER").GetRawText());
		_clearButton = ((Node)this).GetNode<NButton>(NodePath.op_Implicit("ClearButton"));
		((GodotObject)_clearButton).Connect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)ClearText), 0u);
	}

	private void TextUpdated(string _)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).EmitSignal(SignalName.QueryChanged, (Variant[])(object)new Variant[1] { Variant.op_Implicit(_textArea.Text) });
	}

	private void TextSubmitted(string _)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).EmitSignal(SignalName.QuerySubmitted, (Variant[])(object)new Variant[1] { Variant.op_Implicit(_textArea.Text) });
	}

	private void ClearText(NButton _)
	{
		ClearText();
	}

	public void ClearText()
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		((Control)(object)_textArea).TryGrabFocus();
		if (!string.IsNullOrWhiteSpace(_textArea.Text))
		{
			_textArea.Text = "";
			((GodotObject)this).EmitSignal(SignalName.QueryChanged, (Variant[])(object)new Variant[1] { Variant.op_Implicit(_textArea.Text) });
		}
	}

	public static string Normalize(string text)
	{
		string input = NonSpaceWhitespaceCharacters().Replace(text.Trim(), " ");
		return ConsecutiveSpaces().Replace(input, " ").ToLowerInvariant();
	}

	public static string RemoveHtmlTags(string text)
	{
		return HtmlTags().Replace(text, string.Empty);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Expected O, but got Unknown
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(7);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.TextUpdated, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)4, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.TextSubmitted, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)4, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ClearText, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ClearText, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.Normalize, new PropertyInfo((Type)4, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)33, new List<PropertyInfo>
		{
			new PropertyInfo((Type)4, StringName.op_Implicit("text"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.RemoveHtmlTags, new PropertyInfo((Type)4, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)33, new List<PropertyInfo>
		{
			new PropertyInfo((Type)4, StringName.op_Implicit("text"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.TextUpdated && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			TextUpdated(VariantUtils.ConvertTo<string>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.TextSubmitted && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			TextSubmitted(VariantUtils.ConvertTo<string>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.ClearText && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			ClearText(VariantUtils.ConvertTo<NButton>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.ClearText && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			ClearText();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.Normalize && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			string text = Normalize(VariantUtils.ConvertTo<string>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<string>(ref text);
			return true;
		}
		if ((ref method) == MethodName.RemoveHtmlTags && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			string text2 = RemoveHtmlTags(VariantUtils.ConvertTo<string>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<string>(ref text2);
			return true;
		}
		return ((Control)this).InvokeGodotClassMethod(ref method, args, ref ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static bool InvokeGodotClassStaticMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.Normalize && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			string text = Normalize(VariantUtils.ConvertTo<string>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<string>(ref text);
			return true;
		}
		if ((ref method) == MethodName.RemoveHtmlTags && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			string text2 = RemoveHtmlTags(VariantUtils.ConvertTo<string>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<string>(ref text2);
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
		if ((ref method) == MethodName.TextUpdated)
		{
			return true;
		}
		if ((ref method) == MethodName.TextSubmitted)
		{
			return true;
		}
		if ((ref method) == MethodName.ClearText)
		{
			return true;
		}
		if ((ref method) == MethodName.Normalize)
		{
			return true;
		}
		if ((ref method) == MethodName.RemoveHtmlTags)
		{
			return true;
		}
		return ((Control)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._textArea)
		{
			_textArea = VariantUtils.ConvertTo<LineEdit>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._clearButton)
		{
			_clearButton = VariantUtils.ConvertTo<NButton>(ref value);
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
		if ((ref name) == PropertyName.Text)
		{
			string text = Text;
			value = VariantUtils.CreateFrom<string>(ref text);
			return true;
		}
		if ((ref name) == PropertyName.TextArea)
		{
			LineEdit textArea = TextArea;
			value = VariantUtils.CreateFrom<LineEdit>(ref textArea);
			return true;
		}
		if ((ref name) == PropertyName._textArea)
		{
			value = VariantUtils.CreateFrom<LineEdit>(ref _textArea);
			return true;
		}
		if ((ref name) == PropertyName._clearButton)
		{
			value = VariantUtils.CreateFrom<NButton>(ref _clearButton);
			return true;
		}
		return ((GodotObject)this).GetGodotClassPropertyValue(ref name, ref value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._textArea, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._clearButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)4, PropertyName.Text, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.TextArea, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		info.AddProperty(PropertyName._textArea, Variant.From<LineEdit>(ref _textArea));
		info.AddProperty(PropertyName._clearButton, Variant.From<NButton>(ref _clearButton));
		info.AddSignalEventDelegate(SignalName.QueryChanged, (Delegate)backing_QueryChanged);
		info.AddSignalEventDelegate(SignalName.QuerySubmitted, (Delegate)backing_QuerySubmitted);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._textArea, ref val))
		{
			_textArea = ((Variant)(ref val)).As<LineEdit>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._clearButton, ref val2))
		{
			_clearButton = ((Variant)(ref val2)).As<NButton>();
		}
		QueryChangedEventHandler queryChangedEventHandler = default(QueryChangedEventHandler);
		if (info.TryGetSignalEventDelegate<QueryChangedEventHandler>(SignalName.QueryChanged, ref queryChangedEventHandler))
		{
			backing_QueryChanged = queryChangedEventHandler;
		}
		QuerySubmittedEventHandler querySubmittedEventHandler = default(QuerySubmittedEventHandler);
		if (info.TryGetSignalEventDelegate<QuerySubmittedEventHandler>(SignalName.QuerySubmitted, ref querySubmittedEventHandler))
		{
			backing_QuerySubmitted = querySubmittedEventHandler;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotSignalList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(2);
		list.Add(new MethodInfo(SignalName.QueryChanged, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)4, StringName.op_Implicit("query"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(SignalName.QuerySubmitted, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)4, StringName.op_Implicit("query"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		return list;
	}

	protected void EmitSignalQueryChanged(string query)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).EmitSignal(SignalName.QueryChanged, (Variant[])(object)new Variant[1] { Variant.op_Implicit(query) });
	}

	protected void EmitSignalQuerySubmitted(string query)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).EmitSignal(SignalName.QuerySubmitted, (Variant[])(object)new Variant[1] { Variant.op_Implicit(query) });
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RaiseGodotClassSignalCallbacks(in godot_string_name signal, NativeVariantPtrArgs args)
	{
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		if ((ref signal) == SignalName.QueryChanged && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			backing_QueryChanged?.Invoke(VariantUtils.ConvertTo<string>(ref ((NativeVariantPtrArgs)(ref args))[0]));
		}
		else if ((ref signal) == SignalName.QuerySubmitted && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			backing_QuerySubmitted?.Invoke(VariantUtils.ConvertTo<string>(ref ((NativeVariantPtrArgs)(ref args))[0]));
		}
		else
		{
			((GodotObject)this).RaiseGodotClassSignalCallbacks(ref signal, args);
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassSignal(in godot_string_name signal)
	{
		if ((ref signal) == SignalName.QueryChanged)
		{
			return true;
		}
		if ((ref signal) == SignalName.QuerySubmitted)
		{
			return true;
		}
		return ((Control)this).HasGodotClassSignal(ref signal);
	}
}
