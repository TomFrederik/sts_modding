using Godot;
using MegaCrit.Sts2.Core.Multiplayer.Serialization;

namespace MegaCrit.Sts2.Core.Multiplayer.Game.PeerInput;

public static class NetCursorHelper
{
	public static readonly QuantizeParams quantizeParams = new QuantizeParams(-3f, 3f, 16);

	public static Vector2 GetNormalizedPosition(Vector2 mouseScreenPos, Control rootNode)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		Vector2 val = ((CanvasItem)rootNode).GetGlobalTransformWithCanvas() * mouseScreenPos;
		Vector2 val2 = default(Vector2);
		((Vector2)(ref val2))._002Ector(960f, 540f);
		return (val - rootNode.Size / 2f) / val2;
	}

	public static Vector2 GetControlSpacePosition(Vector2 normalizedCursorPosition, Control rootNode)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		Vector2 val = default(Vector2);
		((Vector2)(ref val))._002Ector(960f, 540f);
		return normalizedCursorPosition * val + rootNode.Size / 2f;
	}
}
