using System.Text.Json.Serialization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Multiplayer.Serialization;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace MegaCrit.Sts2.Core.Runs.History;

public struct CardTransformationHistoryEntry(CardModel originalCard, CardModel finalCard) : IPacketSerializable
{
	[JsonPropertyName("original_card")]
	public SerializableCard OriginalCard { get; set; } = originalCard.ToSerializable();

	[JsonPropertyName("final_card")]
	public SerializableCard FinalCard { get; set; } = finalCard.ToSerializable();

	public void Serialize(PacketWriter writer)
	{
		writer.Write(OriginalCard);
		writer.Write(FinalCard);
	}

	public void Deserialize(PacketReader reader)
	{
		OriginalCard = reader.Read<SerializableCard>();
		FinalCard = reader.Read<SerializableCard>();
	}
}
