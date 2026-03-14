using Godot;

namespace MegaCrit.Sts2.Core.RichTextTags;

public static class RichTextUtil
{
	public static readonly Variant colorKey;

	public static readonly Variant visibleKey;

	static RichTextUtil()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		string text = "color";
		colorKey = Variant.From<string>(ref text);
		text = "visible";
		visibleKey = Variant.From<string>(ref text);
	}
}
