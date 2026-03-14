using Godot;

namespace MegaCrit.Sts2.Core.Helpers;

public static class ScrollHelper
{
	private const float _scrollAmount = 40f;

	private const float _panScrollSpeed = 50f;

	public const float dragLerpSpeed = 15f;

	public const float snapThreshold = 0.5f;

	public const float bounceBackStrength = 12f;

	public static float GetDragForScrollEvent(InputEvent inputEvent)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Invalid comparison between Unknown and I8
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Invalid comparison between Unknown and I8
		InputEventMouseButton val = (InputEventMouseButton)(object)((inputEvent is InputEventMouseButton) ? inputEvent : null);
		if (val != null)
		{
			MouseButton buttonIndex = val.ButtonIndex;
			if ((long)buttonIndex == 4)
			{
				return 40f;
			}
			if ((long)buttonIndex == 5)
			{
				return -40f;
			}
		}
		else
		{
			InputEventPanGesture val2 = (InputEventPanGesture)(object)((inputEvent is InputEventPanGesture) ? inputEvent : null);
			if (val2 != null)
			{
				return (0f - val2.Delta.Y) * 50f;
			}
		}
		return 0f;
	}
}
