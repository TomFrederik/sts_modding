using Godot;
using Godot.Collections;

namespace MegaCrit.Sts2.Core.Multiplayer.Transport.ENet;

public static class ENetConnectionExtension
{
	public static bool TryService(this ENetConnection connection, out ENetServiceData? output)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Invalid comparison between Unknown and I8
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		Array val = connection.Service(0);
		output = null;
		if (val == null)
		{
			return false;
		}
		Variant val2 = val[0];
		EventType val3 = ((Variant)(ref val2)).As<EventType>();
		if ((int)val3 == 0)
		{
			return false;
		}
		ENetServiceData eNetServiceData = new ENetServiceData
		{
			type = val3
		};
		val2 = val[1];
		eNetServiceData.peer = ((Variant)(ref val2)).As<ENetPacketPeer>();
		eNetServiceData.originalData = val;
		ENetServiceData value = eNetServiceData;
		if ((long)val3 == 3)
		{
			val2 = val[3];
			value.channel = ((Variant)(ref val2)).As<int>();
			value.packetData = ((PacketPeer)value.peer).GetPacket();
			value.error = ((PacketPeer)value.peer).GetPacketError();
			value.mode = NetTransferMode.None;
		}
		output = value;
		return true;
	}
}
