using Godot;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Multiplayer.Serialization;
using MegaCrit.Sts2.Core.Multiplayer.Transport;

namespace MegaCrit.Sts2.Core.Multiplayer.Messages.Game.Flavor;

public struct ReactionMessage : INetMessage, IPacketSerializable
{
	private static readonly QuantizeParams _quantizeParams = new QuantizeParams(-3f, 3f, 16);

	public ReactionType type;

	public Vector2 normalizedPosition;

	public bool ShouldBroadcast => true;

	public NetTransferMode Mode => NetTransferMode.Unreliable;

	public LogLevel LogLevel => LogLevel.VeryDebug;

	public void Serialize(PacketWriter writer)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		writer.WriteEnum(type);
		writer.WriteVector2(normalizedPosition, _quantizeParams, _quantizeParams);
	}

	public void Deserialize(PacketReader reader)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		type = reader.ReadEnum<ReactionType>();
		normalizedPosition = reader.ReadVector2(_quantizeParams, _quantizeParams);
	}
}
