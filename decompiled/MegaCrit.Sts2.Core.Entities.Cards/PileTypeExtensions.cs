using System;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace MegaCrit.Sts2.Core.Entities.Cards;

public static class PileTypeExtensions
{
	public static CardPile GetPile(this PileType pileType, Player player)
	{
		ArgumentNullException.ThrowIfNull(player, "player");
		CardPile cardPile = CardPile.Get(pileType, player);
		if (cardPile == null)
		{
			throw new InvalidOperationException($"Tried to get {pileType} pile while out of combat.");
		}
		return cardPile;
	}

	public static bool IsCombatPile(this PileType pileType)
	{
		if ((uint)(pileType - 1) <= 4u)
		{
			return true;
		}
		return false;
	}

	public static Vector2 GetTargetPosition(this PileType pileType, NCard? node)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		if (pileType.IsCombatPile() && !CombatManager.Instance.IsInProgress)
		{
			return Vector2.Zero;
		}
		Rect2 viewportRect = ((CanvasItem)NGame.Instance).GetViewportRect();
		Vector2 size = ((Rect2)(ref viewportRect)).Size;
		return (Vector2)(pileType switch
		{
			PileType.None => size, 
			PileType.Draw => ((Control)NCombatRoom.Instance.Ui.DrawPile).GlobalPosition + ((Control)NCombatRoom.Instance.Ui.DrawPile).Size * 0.5f, 
			PileType.Hand => new Vector2(size.X * 0.5f - ((Control)node).Size.X * 0.5f, size.Y - ((Control)node).Size.Y * 0.5f), 
			PileType.Discard => ((Control)NCombatRoom.Instance.Ui.DiscardPile).GlobalPosition + ((Control)NCombatRoom.Instance.Ui.DiscardPile).Size * 0.5f, 
			PileType.Play => NCombatRoom.Instance.Ui.PlayContainer.Size * 0.5f - ((Control)node).Size * 0.5f + Vector2.Up * 100f, 
			PileType.Deck => ((Control)NRun.Instance.GlobalUi.TopBar.Deck).GlobalPosition + ((Control)NRun.Instance.GlobalUi.TopBar.Deck).Size * 0.5f, 
			PileType.Exhaust => ((Control)NCombatRoom.Instance.Ui.ExhaustPile).GlobalPosition + ((Control)NCombatRoom.Instance.Ui.ExhaustPile).Size * 0.5f, 
			_ => throw new ArgumentOutOfRangeException("pileType", pileType, "Unknown pile type"), 
		});
	}
}
