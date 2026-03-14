using System;
using System.Collections.Generic;
using System.Text;
using Godot;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Entities.Text;

namespace MegaCrit.Sts2.addons.mega_text;

public static class MegaLabelHelper
{
	private enum BbcodeParsingState
	{
		NotInTag,
		InTag,
		InEndTag,
		InTagEnvironment
	}

	private const float _defaultLineSpacing = -3f;

	private static readonly List<BbcodeObject> _cachedBbcodeObjects = new List<BbcodeObject>();

	public static void AssertThemeFontOverride(Control control, StringName fontOverrideName)
	{
		if (control.HasThemeFontOverride(fontOverrideName))
		{
			return;
		}
		throw new InvalidOperationException($"{((object)control).GetType().Name} '{((Node)control).GetPath()}' has no theme font override. Please set one to avoid a Godot engine bug.");
	}

	public static List<BbcodeObject> ParseBbcode(string bbcode)
	{
		_cachedBbcodeObjects.Clear();
		BbcodeParsingState bbcodeParsingState = BbcodeParsingState.NotInTag;
		Stack<string> stack = new Stack<string>();
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < bbcode.Length; i++)
		{
			if (bbcode[i] == '[' && bbcodeParsingState == BbcodeParsingState.NotInTag)
			{
				if (stringBuilder.Length > 0)
				{
					_cachedBbcodeObjects.Add(new BbcodeObject
					{
						text = stringBuilder.ToString(),
						type = BbcodeObjectType.Text
					});
					stringBuilder.Clear();
				}
				bbcodeParsingState = BbcodeParsingState.InTag;
			}
			else if ((bbcode[i] == ' ' || bbcode[i] == '=') && bbcodeParsingState == BbcodeParsingState.InTag)
			{
				string text = stringBuilder.ToString();
				_cachedBbcodeObjects.Add(new BbcodeObject
				{
					tag = text,
					type = BbcodeObjectType.BeginTag
				});
				stack.Push(text);
				bbcodeParsingState = BbcodeParsingState.InTagEnvironment;
			}
			else if (bbcode[i] == '/' && bbcodeParsingState == BbcodeParsingState.InTag)
			{
				bbcodeParsingState = BbcodeParsingState.InEndTag;
			}
			else if (bbcode[i] == ']' && (bbcodeParsingState == BbcodeParsingState.InTag || bbcodeParsingState == BbcodeParsingState.InEndTag || bbcodeParsingState == BbcodeParsingState.InTagEnvironment))
			{
				if (bbcodeParsingState != BbcodeParsingState.InTagEnvironment)
				{
					string text2 = stringBuilder.ToString();
					if (bbcodeParsingState == BbcodeParsingState.InEndTag)
					{
						if (stack.Count == 0)
						{
							throw new InvalidOperationException($"Found end tag {text2} with no tag on the stack. ({bbcode})");
						}
						if (stack.Peek() != text2)
						{
							throw new InvalidOperationException($"Found end tag {text2}, expected {stack.Peek()}. ({bbcode})");
						}
						stack.Pop();
					}
					else
					{
						stack.Push(text2);
					}
					_cachedBbcodeObjects.Add(new BbcodeObject
					{
						tag = text2,
						type = ((bbcodeParsingState == BbcodeParsingState.InTag) ? BbcodeObjectType.BeginTag : BbcodeObjectType.EndTag)
					});
				}
				bbcodeParsingState = BbcodeParsingState.NotInTag;
				stringBuilder.Clear();
			}
			else if (bbcodeParsingState != BbcodeParsingState.InTagEnvironment)
			{
				stringBuilder.Append(bbcode[i]);
			}
		}
		if (bbcodeParsingState != BbcodeParsingState.NotInTag)
		{
			throw new InvalidOperationException("In tag at end of string");
		}
		if (stringBuilder.Length > 0)
		{
			_cachedBbcodeObjects.Add(new BbcodeObject
			{
				text = stringBuilder.ToString(),
				type = BbcodeObjectType.Text
			});
		}
		return _cachedBbcodeObjects;
	}

	public static Vector2 EstimateTextSize(TextParagraph paragraph, List<BbcodeObject> objs, Font font, int fontSize, float maxWidth, float lineSpacing)
	{
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		paragraph.Clear();
		paragraph.Direction = (Direction)0;
		paragraph.Orientation = (Orientation)0;
		Stack<string> stack = new Stack<string>();
		int num = 0;
		foreach (BbcodeObject obj in objs)
		{
			if (obj.type == BbcodeObjectType.BeginTag)
			{
				stack.Push(obj.tag);
			}
			else if (obj.type == BbcodeObjectType.EndTag)
			{
				stack.Pop();
			}
			else if (obj.type == BbcodeObjectType.Text)
			{
				if (stack.TryPeek(out var result) && result == "img")
				{
					string text = obj.text;
					Texture2D texture2D = PreloadManager.Cache.GetTexture2D(text);
					paragraph.AddObject(Variant.op_Implicit(num), texture2D.GetSize(), (InlineAlignment)5, 1, 0f);
					num++;
				}
				else
				{
					paragraph.AddString(obj.text, font, fontSize, "", default(Variant));
				}
			}
		}
		paragraph.Width = maxWidth;
		paragraph.BreakFlags = (LineBreakFlag)3;
		paragraph.JustificationFlags = (JustificationFlag)3;
		paragraph.TextOverrunBehavior = (OverrunBehavior)1;
		paragraph.Alignment = (HorizontalAlignment)1;
		paragraph.MaxLinesVisible = -1;
		int lineCount = paragraph.GetLineCount();
		return paragraph.GetSize() + Vector2.Down * (lineSpacing - -3f) * (float)(lineCount - 1);
	}

	public static bool IsTooBig(TextParagraph paragraph, List<BbcodeObject> objs, Font font, int fontSize, float lineSpacing, Vector2 rectSize, bool horizontallyBound, bool verticallyBound)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		Vector2 val = EstimateTextSize(paragraph, objs, font, fontSize, rectSize.X, lineSpacing);
		float x = rectSize.X;
		float y = rectSize.Y;
		bool flag = val.X > x;
		bool flag2 = val.Y > y;
		if (!(flag && horizontallyBound))
		{
			return flag2 && verticallyBound;
		}
		return true;
	}

	public static Vector2 EstimateTextSize(TextParagraph paragraph, string text, Font font, int fontSize, float maxWidth, float lineSpacing, bool wrap = true)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		paragraph.Clear();
		paragraph.Direction = (Direction)0;
		paragraph.Orientation = (Orientation)0;
		paragraph.AddString(text, font, fontSize, "", default(Variant));
		paragraph.Width = maxWidth;
		paragraph.BreakFlags = (LineBreakFlag)(wrap ? 3 : 0);
		paragraph.JustificationFlags = (JustificationFlag)3;
		paragraph.TextOverrunBehavior = (OverrunBehavior)0;
		paragraph.Alignment = (HorizontalAlignment)1;
		paragraph.MaxLinesVisible = -1;
		int lineCount = paragraph.GetLineCount();
		return paragraph.GetSize() + Vector2.Down * (lineSpacing - -3f) * (float)(lineCount - 1);
	}

	public static bool IsTooBig(TextParagraph paragraph, string text, Font font, int fontSize, float lineSpacing, bool wrap, Vector2 rectSize)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		Vector2 val = EstimateTextSize(paragraph, text, font, fontSize, rectSize.X, lineSpacing, wrap);
		float x = rectSize.X;
		float y = rectSize.Y;
		bool flag = val.X > x;
		bool flag2 = val.Y > y;
		return flag || flag2;
	}
}
