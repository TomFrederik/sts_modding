using Godot;

namespace MegaCrit.Sts2.addons.mega_text;

public static class ThemeConstants
{
	public static class Label
	{
		public static readonly StringName fontSize = StringName.op_Implicit("font_size");

		public static readonly StringName font = StringName.op_Implicit("font");

		public static readonly StringName lineSpacing = StringName.op_Implicit("line_spacing");

		public static readonly StringName outlineSize = StringName.op_Implicit("outline_size");

		public static readonly StringName fontColor = StringName.op_Implicit("font_color");

		public static readonly StringName fontOutlineColor = StringName.op_Implicit("font_outline_color");

		public static readonly StringName fontShadowColor = StringName.op_Implicit("font_shadow_color");
	}

	public static class RichTextLabel
	{
		public static readonly StringName normalFont = StringName.op_Implicit("normal_font");

		public static readonly StringName boldFont = StringName.op_Implicit("bold_font");

		public static readonly StringName italicsFont = StringName.op_Implicit("italics_font");

		public static readonly StringName lineSpacing = StringName.op_Implicit("line_separation");

		public static readonly StringName normalFontSize = StringName.op_Implicit("normal_font_size");

		public static readonly StringName boldFontSize = StringName.op_Implicit("bold_font_size");

		public static readonly StringName boldItalicsFontSize = StringName.op_Implicit("bold_italics_font_size");

		public static readonly StringName italicsFontSize = StringName.op_Implicit("italics_font_size");

		public static readonly StringName monoFontSize = StringName.op_Implicit("mono_font_size");

		public static readonly StringName[] allFontSizes = (StringName[])(object)new StringName[5] { normalFontSize, boldFontSize, boldItalicsFontSize, italicsFontSize, monoFontSize };

		public static readonly StringName defaultColor = StringName.op_Implicit("default_color");

		public static readonly StringName fontOutlineColor = StringName.op_Implicit("font_outline_color");

		public static readonly StringName fontShadowColor = StringName.op_Implicit("font_shadow_color");
	}

	public static class Control
	{
		public static readonly StringName focus = StringName.op_Implicit("focus");
	}

	public static class MarginContainer
	{
		public static readonly StringName marginLeft = StringName.op_Implicit("margin_left");

		public static readonly StringName marginRight = StringName.op_Implicit("margin_right");

		public static readonly StringName marginTop = StringName.op_Implicit("margin_top");

		public static readonly StringName marginBottom = StringName.op_Implicit("margin_bottom");
	}

	public static class BoxContainer
	{
		public static readonly StringName separation = StringName.op_Implicit("separation");
	}

	public static class FlowContainer
	{
		public static readonly StringName hSeparation = StringName.op_Implicit("h_separation");

		public static readonly StringName vSeparation = StringName.op_Implicit("v_separation");
	}

	public static class TextEdit
	{
		public static readonly StringName font = StringName.op_Implicit("font");
	}

	public static class LineEdit
	{
		public static readonly StringName font = StringName.op_Implicit("font");
	}
}
