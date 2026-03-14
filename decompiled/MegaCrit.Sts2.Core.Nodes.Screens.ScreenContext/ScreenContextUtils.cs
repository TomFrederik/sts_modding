using Godot;

namespace MegaCrit.Sts2.Core.Nodes.Screens.ScreenContext;

public static class ScreenContextUtils
{
	public static void UpdateControllerNavEnabled<T>(this T screenContext) where T : Control, IScreenContext
	{
		if (ActiveScreenContext.Instance.IsCurrent(screenContext))
		{
			((Control)screenContext).FocusBehaviorRecursive = (FocusBehaviorRecursiveEnum)2;
		}
		else
		{
			((Control)screenContext).FocusBehaviorRecursive = (FocusBehaviorRecursiveEnum)1;
		}
	}
}
