using Godot;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Multiplayer.Game.PeerInput;
using MegaCrit.Sts2.Core.Multiplayer.Serialization;
using MegaCrit.Sts2.Core.Multiplayer.Transport;

namespace MegaCrit.Sts2.Core.Multiplayer.Messages.Game.Sync;

public class PeerInputMessage : INetMessage, IPacketSerializable
{
	private static readonly QuantizeParams _quantizeParams = new QuantizeParams(-3f, 3f, 16);

	public bool mouseDown;

	public bool isTargeting;

	public Vector2? netMousePos;

	public NetScreenType screenType;

	public HoveredModelData hoveredModelData;

	public bool isUsingController;

	public Vector2? controllerFocusPosition;

	public bool ShouldBroadcast => true;

	public NetTransferMode Mode => NetTransferMode.Unreliable;

	public LogLevel LogLevel => LogLevel.VeryDebug;

	public void Serialize(PacketWriter writer)
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		writer.WriteBool(mouseDown);
		writer.WriteBool(isTargeting);
		writer.WriteBool(netMousePos.HasValue);
		if (netMousePos.HasValue)
		{
			writer.WriteVector2(netMousePos.Value, _quantizeParams, _quantizeParams);
		}
		writer.WriteEnum(screenType);
		writer.Write(hoveredModelData);
		writer.WriteBool(isUsingController);
		writer.WriteBool(controllerFocusPosition.HasValue);
		if (controllerFocusPosition.HasValue)
		{
			writer.WriteVector2(controllerFocusPosition.Value, _quantizeParams, _quantizeParams);
		}
	}

	public void Deserialize(PacketReader reader)
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		mouseDown = reader.ReadBool();
		isTargeting = reader.ReadBool();
		if (reader.ReadBool())
		{
			netMousePos = reader.ReadVector2(_quantizeParams, _quantizeParams);
		}
		screenType = reader.ReadEnum<NetScreenType>();
		hoveredModelData = reader.Read<HoveredModelData>();
		isUsingController = reader.ReadBool();
		if (reader.ReadBool())
		{
			controllerFocusPosition = reader.ReadVector2(_quantizeParams, _quantizeParams);
		}
	}
}
