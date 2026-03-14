using System;
using System.Text.Json.Serialization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Multiplayer.Serialization;

namespace MegaCrit.Sts2.Core.Runs.History;

[Serializable]
public struct ModelChoiceHistoryEntry(ModelId choice, bool wasPicked)
{
	[JsonPropertyName("choice")]
	public ModelId choice = choice;

	[JsonPropertyName("was_picked")]
	public bool wasPicked = wasPicked;

	public void Serialize<T>(PacketWriter writer) where T : AbstractModel
	{
		writer.WriteModelEntry(choice);
		writer.WriteBool(wasPicked);
	}

	public void Deserialize<T>(PacketReader reader) where T : AbstractModel
	{
		choice = reader.ReadModelIdAssumingType<T>();
		wasPicked = reader.ReadBool();
	}
}
