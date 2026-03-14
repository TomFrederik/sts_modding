using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Godot;
using MegaCrit.Sts2.Core.AutoSlay.Helpers;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.Cards.Holders;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.Screens.CardSelection;
using MegaCrit.Sts2.Core.Random;

namespace MegaCrit.Sts2.Core.AutoSlay.Handlers.Screens;

public class DeckTransformScreenHandler : IScreenHandler, IHandler
{
	public Type ScreenType => typeof(NDeckTransformSelectScreen);

	public TimeSpan Timeout => TimeSpan.FromSeconds(30L);

	public async Task HandleAsync(Rng random, CancellationToken ct)
	{
		AutoSlayLog.EnterScreen("NDeckTransformSelectScreen");
		NDeckTransformSelectScreen screen = AutoSlayer.GetCurrentScreen<NDeckTransformSelectScreen>();
		NCardGrid grid = UiHelper.FindFirst<NCardGrid>((Node)(object)screen);
		if (grid == null)
		{
			AutoSlayLog.Error("Card grid not found in transform screen");
			return;
		}
		Control previewContainer = ((Node)screen).GetNodeOrNull<Control>(NodePath.op_Implicit("%PreviewContainer"));
		NConfirmButton mainConfirmButton = ((Node)screen).GetNodeOrNull<NConfirmButton>(NodePath.op_Implicit("Confirm"));
		HashSet<NGridCardHolder> selectedCards = new HashSet<NGridCardHolder>();
		for (int i = 0; i < 10; i++)
		{
			ct.ThrowIfCancellationRequested();
			Control obj = previewContainer;
			if (obj != null && ((CanvasItem)obj).Visible)
			{
				AutoSlayLog.Info("Preview container appeared after selecting cards");
				break;
			}
			if (mainConfirmButton != null && mainConfirmButton.IsEnabled)
			{
				AutoSlayLog.Action("Clicking main confirm button");
				await UiHelper.Click(mainConfirmButton);
				await Task.Delay(300, ct);
				await WaitHelper.Until(delegate
				{
					Control obj3 = previewContainer;
					return obj3 != null && ((CanvasItem)obj3).Visible;
				}, ct, TimeSpan.FromSeconds(5L), "Preview container did not appear after confirm");
				break;
			}
			List<NGridCardHolder> list = (from c in UiHelper.FindAll<NGridCardHolder>((Node)(object)screen)
				where !selectedCards.Contains(c)
				select c).ToList();
			if (list.Count == 0)
			{
				AutoSlayLog.Warn("No more cards available to select");
				break;
			}
			NGridCardHolder nGridCardHolder = random.NextItem(list);
			selectedCards.Add(nGridCardHolder);
			AutoSlayLog.Action($"Selecting card to transform ({selectedCards.Count})");
			((GodotObject)grid).EmitSignal(NCardGrid.SignalName.HolderPressed, (Variant[])(object)new Variant[1] { Variant.op_Implicit((GodotObject)(object)nGridCardHolder) });
			await Task.Delay(300, ct);
		}
		await WaitHelper.Until(delegate
		{
			Control obj3 = previewContainer;
			return obj3 != null && ((CanvasItem)obj3).Visible;
		}, ct, TimeSpan.FromSeconds(5L), "Preview container did not appear");
		await Task.Delay(500, ct);
		Control obj2 = previewContainer;
		NConfirmButton previewConfirmButton = ((obj2 != null) ? ((Node)obj2).GetNodeOrNull<NConfirmButton>(NodePath.op_Implicit("Confirm")) : null);
		if (previewConfirmButton == null)
		{
			AutoSlayLog.Error("Preview confirm button not found");
			return;
		}
		await WaitHelper.Until(() => previewConfirmButton.IsEnabled, ct, TimeSpan.FromSeconds(5L), "Preview confirm button did not become enabled");
		AutoSlayLog.Action("Confirming transform");
		await UiHelper.Click(previewConfirmButton);
		await WaitHelper.Until(() => !GodotObject.IsInstanceValid((GodotObject)(object)screen) || !((CanvasItem)screen).IsVisibleInTree(), ct, TimeSpan.FromSeconds(10L), "Transform screen did not close after confirmation");
		AutoSlayLog.ExitScreen("NDeckTransformSelectScreen");
	}
}
