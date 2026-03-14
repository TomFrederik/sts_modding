using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Godot;
using MegaCrit.Sts2.Core.AutoSlay.Helpers;
using MegaCrit.Sts2.Core.Nodes.Cards.Holders;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.Screens.CardSelection;
using MegaCrit.Sts2.Core.Random;

namespace MegaCrit.Sts2.Core.AutoSlay.Handlers.Screens;

public class DeckUpgradeScreenHandler : IScreenHandler, IHandler
{
	public Type ScreenType => typeof(NDeckUpgradeSelectScreen);

	public TimeSpan Timeout => TimeSpan.FromSeconds(30L);

	public async Task HandleAsync(Rng random, CancellationToken ct)
	{
		AutoSlayLog.EnterScreen("NDeckUpgradeSelectScreen");
		NDeckUpgradeSelectScreen screen = AutoSlayer.GetCurrentScreen<NDeckUpgradeSelectScreen>();
		List<NGridCardHolder> cards = UiHelper.FindAll<NGridCardHolder>((Node)(object)screen);
		if (cards.Count == 0)
		{
			AutoSlayLog.Warn("No cards found in upgrade screen");
			return;
		}
		int maxSelections = Math.Min(cards.Count, 5);
		for (int i = 0; i < maxSelections; i++)
		{
			if (!GodotObject.IsInstanceValid((GodotObject)(object)screen))
			{
				break;
			}
			if (!((CanvasItem)screen).IsVisibleInTree())
			{
				break;
			}
			Control nodeOrNull = ((Node)screen).GetNodeOrNull<Control>(NodePath.op_Implicit("%UpgradeSinglePreviewContainer"));
			Control nodeOrNull2 = ((Node)screen).GetNodeOrNull<Control>(NodePath.op_Implicit("%UpgradeMultiPreviewContainer"));
			if ((nodeOrNull != null && ((CanvasItem)nodeOrNull).Visible) || (nodeOrNull2 != null && ((CanvasItem)nodeOrNull2).Visible))
			{
				break;
			}
			NGridCardHolder nGridCardHolder = random.NextItem(cards);
			AutoSlayLog.Action("Selecting card to upgrade");
			((GodotObject)nGridCardHolder).EmitSignal(NCardHolder.SignalName.Pressed, (Variant[])(object)new Variant[1] { Variant.op_Implicit((GodotObject)(object)nGridCardHolder) });
			cards.Remove(nGridCardHolder);
			await Task.Delay(300, ct);
		}
		Control visiblePreview = null;
		await WaitHelper.Until(delegate
		{
			Control nodeOrNull3 = ((Node)screen).GetNodeOrNull<Control>(NodePath.op_Implicit("%UpgradeSinglePreviewContainer"));
			Control nodeOrNull4 = ((Node)screen).GetNodeOrNull<Control>(NodePath.op_Implicit("%UpgradeMultiPreviewContainer"));
			if (nodeOrNull3 != null && ((CanvasItem)nodeOrNull3).Visible)
			{
				visiblePreview = nodeOrNull3;
				return true;
			}
			if (nodeOrNull4 != null && ((CanvasItem)nodeOrNull4).Visible)
			{
				visiblePreview = nodeOrNull4;
				return true;
			}
			return !GodotObject.IsInstanceValid((GodotObject)(object)screen) || !((CanvasItem)screen).IsVisibleInTree();
		}, ct, TimeSpan.FromSeconds(5L), "Upgrade preview did not appear");
		if (!GodotObject.IsInstanceValid((GodotObject)(object)screen) || !((CanvasItem)screen).IsVisibleInTree())
		{
			AutoSlayLog.ExitScreen("NDeckUpgradeSelectScreen");
			return;
		}
		Control obj = visiblePreview;
		NConfirmButton confirmButton = ((obj != null) ? ((Node)obj).GetNodeOrNull<NConfirmButton>(NodePath.op_Implicit("Confirm")) : null);
		if (confirmButton == null)
		{
			AutoSlayLog.Error("Preview confirm button not found");
			AutoSlayLog.ExitScreen("NDeckUpgradeSelectScreen");
			return;
		}
		await WaitHelper.Until(() => confirmButton.IsEnabled, ct, TimeSpan.FromSeconds(5L), "Upgrade confirm button did not become enabled");
		AutoSlayLog.Action("Confirming upgrade");
		await UiHelper.Click(confirmButton);
		await WaitHelper.Until(() => !GodotObject.IsInstanceValid((GodotObject)(object)screen) || !((CanvasItem)screen).IsVisibleInTree(), ct, TimeSpan.FromSeconds(10L), "Upgrade screen did not close after confirmation");
		AutoSlayLog.ExitScreen("NDeckUpgradeSelectScreen");
	}
}
