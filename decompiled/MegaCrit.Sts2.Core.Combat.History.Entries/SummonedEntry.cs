using MegaCrit.Sts2.Core.Entities.Players;

namespace MegaCrit.Sts2.Core.Combat.History.Entries;

public class SummonedEntry(int amount, Player player, int roundNumber, CombatSide currentSide, CombatHistory history) : CombatHistoryEntry(player.Creature, roundNumber, currentSide, history)
{
	public int Amount { get; } = amount;

	public override string Description => $"{base.Actor.Player.Character.Id.Entry} summoned {Amount}";
}
