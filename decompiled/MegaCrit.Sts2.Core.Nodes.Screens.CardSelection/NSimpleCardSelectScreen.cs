using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards.Holders;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Screens.Overlays;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.Nodes.Screens.CardSelection;

[ScriptPath("res://src/Core/Nodes/Screens/CardSelection/NSimpleCardSelectScreen.cs")]
public sealed class NSimpleCardSelectScreen : NCardGridSelectionScreen
{
	public new class MethodName : NCardGridSelectionScreen.MethodName
	{
		public new static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public new static readonly StringName ConnectSignalsAndInitGrid = StringName.op_Implicit("ConnectSignalsAndInitGrid");

		public new static readonly StringName AfterOverlayOpened = StringName.op_Implicit("AfterOverlayOpened");

		public static readonly StringName CheckIfSelectionComplete = StringName.op_Implicit("CheckIfSelectionComplete");

		public static readonly StringName CompleteSelection = StringName.op_Implicit("CompleteSelection");
	}

	public new class PropertyName : NCardGridSelectionScreen.PropertyName
	{
		public static readonly StringName _bottomTextContainer = StringName.op_Implicit("_bottomTextContainer");

		public static readonly StringName _infoLabel = StringName.op_Implicit("_infoLabel");

		public static readonly StringName _confirmButton = StringName.op_Implicit("_confirmButton");

		public static readonly StringName _combatPiles = StringName.op_Implicit("_combatPiles");
	}

	public new class SignalName : NCardGridSelectionScreen.SignalName
	{
	}

	private Control _bottomTextContainer;

	private MegaRichTextLabel _infoLabel;

	private NConfirmButton _confirmButton;

	private NCombatPilesContainer _combatPiles;

	private readonly HashSet<CardModel> _selectedCards = new HashSet<CardModel>();

	private CardSelectorPrefs _prefs;

	private List<CardCreationResult>? _cardResults;

	private static string ScenePath => SceneHelper.GetScenePath("screens/card_selection/simple_card_select_screen");

	public static IEnumerable<string> AssetPaths => new _003C_003Ez__ReadOnlySingleElementList<string>(ScenePath);

	protected override IEnumerable<Control> PeekButtonTargets => new _003C_003Ez__ReadOnlySingleElementList<Control>(_bottomTextContainer);

	public static NSimpleCardSelectScreen Create(IReadOnlyList<CardModel> cards, CardSelectorPrefs prefs)
	{
		NSimpleCardSelectScreen nSimpleCardSelectScreen = PreloadManager.Cache.GetScene(ScenePath).Instantiate<NSimpleCardSelectScreen>((GenEditState)0);
		((Node)nSimpleCardSelectScreen).Name = StringName.op_Implicit("NSimpleCardSelectScreen");
		nSimpleCardSelectScreen._cards = cards.ToList();
		nSimpleCardSelectScreen._cardResults = null;
		nSimpleCardSelectScreen._prefs = prefs;
		return nSimpleCardSelectScreen;
	}

	public static NSimpleCardSelectScreen Create(IReadOnlyList<CardCreationResult> cards, CardSelectorPrefs prefs)
	{
		NSimpleCardSelectScreen nSimpleCardSelectScreen = PreloadManager.Cache.GetScene(ScenePath).Instantiate<NSimpleCardSelectScreen>((GenEditState)0);
		((Node)nSimpleCardSelectScreen).Name = StringName.op_Implicit("NSimpleCardSelectScreen");
		nSimpleCardSelectScreen._cards = cards.Select((CardCreationResult r) => r.Card).ToList();
		nSimpleCardSelectScreen._cardResults = cards.ToList();
		nSimpleCardSelectScreen._prefs = prefs;
		return nSimpleCardSelectScreen;
	}

	public override void _Ready()
	{
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		ConnectSignalsAndInitGrid();
		_confirmButton = ((Node)this).GetNode<NConfirmButton>(NodePath.op_Implicit("%Confirm"));
		_bottomTextContainer = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%BottomText"));
		_infoLabel = ((Node)_bottomTextContainer).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("%BottomLabel"));
		_infoLabel.Text = _prefs.Prompt.GetFormattedText();
		if (_prefs.MinSelect == 0)
		{
			_confirmButton.Enable();
		}
		else
		{
			_confirmButton.Disable();
		}
		((GodotObject)_confirmButton).Connect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)delegate
		{
			CompleteSelection();
		}), 0u);
	}

	protected override void ConnectSignalsAndInitGrid()
	{
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		base.ConnectSignalsAndInitGrid();
		_combatPiles = ((Node)this).GetNode<NCombatPilesContainer>(NodePath.op_Implicit("%CombatPiles"));
		if (CombatManager.Instance.IsInProgress)
		{
			_combatPiles.Initialize(_cards.First().Owner);
		}
		_combatPiles.Disable();
		((CanvasItem)_combatPiles).SetVisible(false);
		((GodotObject)_peekButton).Connect(NPeekButton.SignalName.Toggled, Callable.From<NPeekButton>((Action<NPeekButton>)delegate
		{
			if (_peekButton.IsPeeking)
			{
				_combatPiles.Enable();
				((CanvasItem)_combatPiles).SetVisible(true);
			}
			else
			{
				_combatPiles.Disable();
				((CanvasItem)_combatPiles).SetVisible(false);
			}
		}), 0u);
	}

	public override void AfterOverlayOpened()
	{
		base.AfterOverlayOpened();
		TaskHelper.RunSafely(FlashRelicsOnModifiedCards());
	}

	private async Task FlashRelicsOnModifiedCards()
	{
		if (_cardResults == null)
		{
			return;
		}
		await ((GodotObject)this).ToSignal((GodotObject)(object)((Node)this).GetTree(), SignalName.ProcessFrame);
		foreach (CardCreationResult result in _cardResults)
		{
			NGridCardHolder nGridCardHolder = _grid.CurrentlyDisplayedCardHolders.FirstOrDefault((NGridCardHolder h) => h.CardModel == result.Card);
			if (nGridCardHolder == null || !result.HasBeenModified)
			{
				continue;
			}
			foreach (RelicModel modifyingRelic in result.ModifyingRelics)
			{
				modifyingRelic.Flash();
				nGridCardHolder.CardNode?.FlashRelicOnCard(modifyingRelic);
			}
		}
	}

	protected override void OnCardClicked(CardModel card)
	{
		if (_selectedCards.Contains(card))
		{
			_grid.UnhighlightCard(card);
			_selectedCards.Remove(card);
		}
		else
		{
			if (_selectedCards.Count < _prefs.MaxSelect)
			{
				_grid.HighlightCard(card);
				_selectedCards.Add(card);
			}
			if (!_prefs.RequireManualConfirmation)
			{
				CheckIfSelectionComplete();
			}
		}
		if (_selectedCards.Count >= _prefs.MinSelect && _prefs.RequireManualConfirmation)
		{
			_confirmButton.Enable();
		}
		else
		{
			_confirmButton.Disable();
		}
	}

	private void CheckIfSelectionComplete()
	{
		if (_selectedCards.Count >= _prefs.MaxSelect)
		{
			CompleteSelection();
		}
	}

	private void CompleteSelection()
	{
		_completionSource.SetResult(_selectedCards);
		NOverlayStack.Instance.Remove(this);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(5);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ConnectSignalsAndInitGrid, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AfterOverlayOpened, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.CheckIfSelectionComplete, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.CompleteSelection, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.ConnectSignalsAndInitGrid && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			ConnectSignalsAndInitGrid();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.AfterOverlayOpened && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			AfterOverlayOpened();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.CheckIfSelectionComplete && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			CheckIfSelectionComplete();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.CompleteSelection && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			CompleteSelection();
			ret = default(godot_variant);
			return true;
		}
		return base.InvokeGodotClassMethod(in method, args, out ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if ((ref method) == MethodName._Ready)
		{
			return true;
		}
		if ((ref method) == MethodName.ConnectSignalsAndInitGrid)
		{
			return true;
		}
		if ((ref method) == MethodName.AfterOverlayOpened)
		{
			return true;
		}
		if ((ref method) == MethodName.CheckIfSelectionComplete)
		{
			return true;
		}
		if ((ref method) == MethodName.CompleteSelection)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._bottomTextContainer)
		{
			_bottomTextContainer = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._infoLabel)
		{
			_infoLabel = VariantUtils.ConvertTo<MegaRichTextLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._confirmButton)
		{
			_confirmButton = VariantUtils.ConvertTo<NConfirmButton>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._combatPiles)
		{
			_combatPiles = VariantUtils.ConvertTo<NCombatPilesContainer>(ref value);
			return true;
		}
		return base.SetGodotClassPropertyValue(in name, in value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName._bottomTextContainer)
		{
			value = VariantUtils.CreateFrom<Control>(ref _bottomTextContainer);
			return true;
		}
		if ((ref name) == PropertyName._infoLabel)
		{
			value = VariantUtils.CreateFrom<MegaRichTextLabel>(ref _infoLabel);
			return true;
		}
		if ((ref name) == PropertyName._confirmButton)
		{
			value = VariantUtils.CreateFrom<NConfirmButton>(ref _confirmButton);
			return true;
		}
		if ((ref name) == PropertyName._combatPiles)
		{
			value = VariantUtils.CreateFrom<NCombatPilesContainer>(ref _combatPiles);
			return true;
		}
		return base.GetGodotClassPropertyValue(in name, out value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._bottomTextContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._infoLabel, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._confirmButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._combatPiles, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName._bottomTextContainer, Variant.From<Control>(ref _bottomTextContainer));
		info.AddProperty(PropertyName._infoLabel, Variant.From<MegaRichTextLabel>(ref _infoLabel));
		info.AddProperty(PropertyName._confirmButton, Variant.From<NConfirmButton>(ref _confirmButton));
		info.AddProperty(PropertyName._combatPiles, Variant.From<NCombatPilesContainer>(ref _combatPiles));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._bottomTextContainer, ref val))
		{
			_bottomTextContainer = ((Variant)(ref val)).As<Control>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._infoLabel, ref val2))
		{
			_infoLabel = ((Variant)(ref val2)).As<MegaRichTextLabel>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._confirmButton, ref val3))
		{
			_confirmButton = ((Variant)(ref val3)).As<NConfirmButton>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._combatPiles, ref val4))
		{
			_combatPiles = ((Variant)(ref val4)).As<NCombatPilesContainer>();
		}
	}
}
