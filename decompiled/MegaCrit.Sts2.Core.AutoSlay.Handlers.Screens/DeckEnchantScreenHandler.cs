using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Godot;
using MegaCrit.Sts2.Core.AutoSlay.Helpers;
using MegaCrit.Sts2.Core.Nodes.Cards.Holders;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.Screens.CardSelection;
using MegaCrit.Sts2.Core.Random;

namespace MegaCrit.Sts2.Core.AutoSlay.Handlers.Screens;

public class DeckEnchantScreenHandler : IScreenHandler, IHandler
{
	public Type ScreenType => typeof(NDeckEnchantSelectScreen);

	public TimeSpan Timeout => TimeSpan.FromSeconds(30L);

	public async Task HandleAsync(Rng random, CancellationToken ct)
	{
		AutoSlayLog.EnterScreen("NDeckEnchantSelectScreen");
		NDeckEnchantSelectScreen screen = AutoSlayer.GetCurrentScreen<NDeckEnchantSelectScreen>();
		List<NGridCardHolder> cards = new List<NGridCardHolder>();
		await WaitHelper.Until(delegate
		{
			cards = UiHelper.FindAll<NGridCardHolder>((Node)(object)screen);
			return cards.Count > 0;
		}, ct, TimeSpan.FromSeconds(5L), "No cards found in enchant screen");
		await Task.Delay(300, ct);
		int maxSelections = Math.Min(cards.Count, 5);
		List<NGridCardHolder> selectedCards = new List<NGridCardHolder>();
		Control singlePreviewContainer;
		Control multiPreviewContainer;
		NConfirmButton nodeOrNull;
		for (int i = 0; i < maxSelections; i++)
		{
			singlePreviewContainer = ((Node)screen).GetNodeOrNull<Control>(NodePath.op_Implicit("%EnchantSinglePreviewContainer"));
			multiPreviewContainer = ((Node)screen).GetNodeOrNull<Control>(NodePath.op_Implicit("%EnchantMultiPreviewContainer"));
			nodeOrNull = ((Node)screen).GetNodeOrNull<NConfirmButton>(NodePath.op_Implicit("Confirm"));
			Control obj = singlePreviewContainer;
			if (obj != null && ((CanvasItem)obj).Visible)
			{
				break;
			}
			Control obj2 = multiPreviewContainer;
			if ((obj2 != null && ((CanvasItem)obj2).Visible) || (nodeOrNull != null && nodeOrNull.IsEnabled))
			{
				break;
			}
			List<NGridCardHolder> list = cards.Where((NGridCardHolder c) => !selectedCards.Contains(c)).ToList();
			if (list.Count == 0)
			{
				break;
			}
			NGridCardHolder nGridCardHolder = random.NextItem(list);
			selectedCards.Add(nGridCardHolder);
			AutoSlayLog.Action($"Selecting card to enchant ({cards.Count} cards available)");
			((GodotObject)nGridCardHolder).EmitSignal(NCardHolder.SignalName.Pressed, (Variant[])(object)new Variant[1] { Variant.op_Implicit((GodotObject)(object)nGridCardHolder) });
			await Task.Delay(200, ct);
		}
		singlePreviewContainer = ((Node)screen).GetNodeOrNull<Control>(NodePath.op_Implicit("%EnchantSinglePreviewContainer"));
		multiPreviewContainer = ((Node)screen).GetNodeOrNull<Control>(NodePath.op_Implicit("%EnchantMultiPreviewContainer"));
		nodeOrNull = ((Node)screen).GetNodeOrNull<NConfirmButton>(NodePath.op_Implicit("Confirm"));
		Control obj3 = singlePreviewContainer;
		if (obj3 == null || !((CanvasItem)obj3).Visible)
		{
			Control obj4 = multiPreviewContainer;
			if ((obj4 == null || !((CanvasItem)obj4).Visible) && nodeOrNull != null && nodeOrNull.IsEnabled)
			{
				AutoSlayLog.Action("Clicking main confirm button to show preview");
				await UiHelper.Click(nodeOrNull);
				await Task.Delay(200, ct);
			}
		}
		Control visiblePreviewContainer = null;
		await WaitHelper.Until(delegate
		{
			singlePreviewContainer = ((Node)screen).GetNodeOrNull<Control>(NodePath.op_Implicit("%EnchantSinglePreviewContainer"));
			multiPreviewContainer = ((Node)screen).GetNodeOrNull<Control>(NodePath.op_Implicit("%EnchantMultiPreviewContainer"));
			Control obj6 = singlePreviewContainer;
			if (obj6 != null && ((CanvasItem)obj6).Visible)
			{
				visiblePreviewContainer = singlePreviewContainer;
				return true;
			}
			Control obj7 = multiPreviewContainer;
			if (obj7 != null && ((CanvasItem)obj7).Visible)
			{
				visiblePreviewContainer = multiPreviewContainer;
				return true;
			}
			return false;
		}, ct, TimeSpan.FromSeconds(5L), "Enchant preview container did not appear");
		await Task.Delay(500, ct);
		Control obj5 = visiblePreviewContainer;
		NConfirmButton confirmButton = ((obj5 != null) ? ((Node)obj5).GetNodeOrNull<NConfirmButton>(NodePath.op_Implicit("Confirm")) : null);
		if (confirmButton == null)
		{
			List<NConfirmButton> source = UiHelper.FindAll<NConfirmButton>((Node)(object)screen);
			confirmButton = source.FirstOrDefault((NConfirmButton b) => b.IsEnabled);
		}
		if (confirmButton != null)
		{
			await WaitHelper.Until(() => confirmButton.IsEnabled, ct, TimeSpan.FromSeconds(5L), "Enchant confirm button did not become enabled");
			AutoSlayLog.Action("Confirming enchant");
			await UiHelper.Click(confirmButton);
			await WaitHelper.Until(() => !GodotObject.IsInstanceValid((GodotObject)(object)screen) || !((CanvasItem)screen).IsVisibleInTree(), ct, TimeSpan.FromSeconds(10L), "Enchant screen did not close after confirmation");
		}
		else
		{
			AutoSlayLog.Error("No confirm button found on enchant screen");
		}
		AutoSlayLog.ExitScreen("NDeckEnchantSelectScreen");
	}
}
