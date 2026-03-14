using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.UI;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.Cards.Holders;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.Unlocks;

namespace MegaCrit.Sts2.Core.Nodes.Screens.CardLibrary;

[ScriptPath("res://src/Core/Nodes/Screens/CardLibrary/NCardLibraryGrid.cs")]
public class NCardLibraryGrid : NCardGrid
{
	private struct InitialSorter(List<CardPoolModel> cardPoolModels) : IComparer<CardModel>
	{
		private List<CardPoolModel> _cardPoolModels = cardPoolModels;

		public int Compare(CardModel? x, CardModel? y)
		{
			if (x == null)
			{
				if (y != null)
				{
					return -1;
				}
				return 0;
			}
			if (y == null)
			{
				return 1;
			}
			int num = _cardPoolModels.IndexOf(x.Pool).CompareTo(_cardPoolModels.IndexOf(y.Pool));
			if (num != 0)
			{
				return num;
			}
			int num2 = x.Rarity.CompareTo(y.Rarity);
			if (num2 != 0)
			{
				return num2;
			}
			return x.Id.CompareTo(y.Id);
		}
	}

	public new class MethodName : NCardGrid.MethodName
	{
		public new static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName RefreshVisibility = StringName.op_Implicit("RefreshVisibility");

		public new static readonly StringName InitGrid = StringName.op_Implicit("InitGrid");

		public new static readonly StringName UpdateGridNavigation = StringName.op_Implicit("UpdateGridNavigation");
	}

	public new class PropertyName : NCardGrid.PropertyName
	{
		public new static readonly StringName IsCardLibrary = StringName.op_Implicit("IsCardLibrary");

		public new static readonly StringName CenterGrid = StringName.op_Implicit("CenterGrid");

		public static readonly StringName ShowStats = StringName.op_Implicit("ShowStats");

		public static readonly StringName _showStats = StringName.op_Implicit("_showStats");
	}

	public new class SignalName : NCardGrid.SignalName
	{
	}

	private readonly List<CardModel> _allCards = new List<CardModel>();

	private HashSet<ModelId> _seenCards;

	private HashSet<CardModel> _unlockedCards;

	private bool _showStats;

	protected override bool IsCardLibrary => true;

	protected override bool CenterGrid => false;

	public bool ShowStats
	{
		get
		{
			return _showStats;
		}
		set
		{
			_showStats = value;
			foreach (NGridCardHolder item in _cardRows.SelectMany((List<NGridCardHolder> r) => r))
			{
				if (item.CardNode != null)
				{
					NCardLibraryStats node = ((Node)item).GetNode<NCardLibraryStats>(NodePath.op_Implicit("CardLibraryStats"));
					((CanvasItem)node).Visible = _showStats;
				}
			}
		}
	}

	public IEnumerable<CardModel> VisibleCards => _cards;

	public override void _Ready()
	{
		ConnectSignals();
		List<CardPoolModel> cardPoolModels = ModelDb.AllCardPools.ToList();
		foreach (CardModel allCard in ModelDb.AllCards)
		{
			if (allCard.ShouldShowInCardLibrary)
			{
				_allCards.Add(allCard);
			}
		}
		_allCards.Sort(new InitialSorter(cardPoolModels));
		RefreshVisibility();
	}

	public void RefreshVisibility()
	{
		_seenCards = SaveManager.Instance.Progress.DiscoveredCards.ToHashSet();
		UnlockState unlockState = SaveManager.Instance.GenerateUnlockStateFromProgress();
		_unlockedCards = ModelDb.AllCardPools.Select((CardPoolModel p) => p.GetUnlockedCards(unlockState, CardMultiplayerConstraint.None)).SelectMany((IEnumerable<CardModel> c) => c).ToHashSet();
	}

	public void FilterCards(Func<CardModel, bool> filter)
	{
		int num = 1;
		List<SortingOrders> list = new List<SortingOrders>(num);
		CollectionsMarshal.SetCount(list, num);
		Span<SortingOrders> span = CollectionsMarshal.AsSpan(list);
		int index = 0;
		span[index] = SortingOrders.AlphabetAscending;
		FilterCards(filter, list);
	}

	public void FilterCards(Func<CardModel, bool> filter, List<SortingOrders> sortingPriority)
	{
		List<CardModel> cards = _allCards.Where(filter).ToList();
		DisplayCards(cards, sortingPriority);
	}

	private void DisplayCards(List<CardModel> cards, List<SortingOrders> sortingPriority)
	{
		SetCards(cards, PileType.None, sortingPriority, Task.CompletedTask);
	}

	protected override void InitGrid()
	{
		base.InitGrid();
		foreach (NGridCardHolder item in _cardRows.SelectMany((List<NGridCardHolder> r) => r))
		{
			if (item.CardNode != null)
			{
				CardModel model = item.CardNode.Model;
				bool flag = _seenCards.Contains(model.Id);
				item.EnsureCardLibraryStatsExists();
				NCardLibraryStats cardLibraryStats = item.CardLibraryStats;
				cardLibraryStats.UpdateStats(item.CardNode.Model);
				((CanvasItem)cardLibraryStats).Visible = ShowStats;
				((Control)item.Hitbox).MouseDefaultCursorShape = (CursorShape)(flag ? 16 : 0);
			}
		}
	}

	protected override void AssignCardsToRow(List<NGridCardHolder> row, int startIndex)
	{
		base.AssignCardsToRow(row, startIndex);
		foreach (NGridCardHolder item in row)
		{
			if (item.CardNode != null)
			{
				CardModel model = item.CardNode.Model;
				bool flag = _seenCards.Contains(model.Id);
				item.CardLibraryStats.UpdateStats(model);
				((Control)item.Hitbox).MouseDefaultCursorShape = (CursorShape)(flag ? 16 : 0);
			}
		}
	}

	protected override ModelVisibility GetCardVisibility(CardModel card)
	{
		if (!_unlockedCards.Contains(card))
		{
			return ModelVisibility.Locked;
		}
		if (!_seenCards.Contains(card.Id))
		{
			return ModelVisibility.NotSeen;
		}
		return ModelVisibility.Visible;
	}

	protected override void UpdateGridNavigation()
	{
		for (int i = 0; i < _cardRows.Count; i++)
		{
			for (int j = 0; j < _cardRows[i].Count; j++)
			{
				NCardHolder nCardHolder = _cardRows[i][j];
				((Control)nCardHolder).FocusNeighborLeft = ((j > 0) ? ((Node)_cardRows[i][j - 1]).GetPath() : null);
				((Control)nCardHolder).FocusNeighborRight = ((j < _cardRows[i].Count - 1) ? ((Node)_cardRows[i][j + 1]).GetPath() : null);
				((Control)nCardHolder).FocusNeighborTop = ((i > 0) ? ((Node)_cardRows[i - 1][j]).GetPath() : null);
				((Control)nCardHolder).FocusNeighborBottom = ((i < _cardRows.Count - 1 && j < _cardRows[i + 1].Count) ? ((Node)_cardRows[i + 1][j]).GetPath() : null);
			}
		}
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
		List<MethodInfo> list = new List<MethodInfo>(4);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.RefreshVisibility, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.InitGrid, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UpdateGridNavigation, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.RefreshVisibility && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			RefreshVisibility();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.InitGrid && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			InitGrid();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.UpdateGridNavigation && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			UpdateGridNavigation();
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
		if ((ref method) == MethodName.RefreshVisibility)
		{
			return true;
		}
		if ((ref method) == MethodName.InitGrid)
		{
			return true;
		}
		if ((ref method) == MethodName.UpdateGridNavigation)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName.ShowStats)
		{
			ShowStats = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._showStats)
		{
			_showStats = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		return base.SetGodotClassPropertyValue(in name, in value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName.IsCardLibrary)
		{
			bool isCardLibrary = IsCardLibrary;
			value = VariantUtils.CreateFrom<bool>(ref isCardLibrary);
			return true;
		}
		if ((ref name) == PropertyName.CenterGrid)
		{
			bool isCardLibrary = CenterGrid;
			value = VariantUtils.CreateFrom<bool>(ref isCardLibrary);
			return true;
		}
		if ((ref name) == PropertyName.ShowStats)
		{
			bool isCardLibrary = ShowStats;
			value = VariantUtils.CreateFrom<bool>(ref isCardLibrary);
			return true;
		}
		if ((ref name) == PropertyName._showStats)
		{
			value = VariantUtils.CreateFrom<bool>(ref _showStats);
			return true;
		}
		return base.GetGodotClassPropertyValue(in name, out value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)1, PropertyName.IsCardLibrary, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName.CenterGrid, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._showStats, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName.ShowStats, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		base.SaveGodotObjectData(info);
		StringName showStats = PropertyName.ShowStats;
		bool showStats2 = ShowStats;
		info.AddProperty(showStats, Variant.From<bool>(ref showStats2));
		info.AddProperty(PropertyName._showStats, Variant.From<bool>(ref _showStats));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName.ShowStats, ref val))
		{
			ShowStats = ((Variant)(ref val)).As<bool>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._showStats, ref val2))
		{
			_showStats = ((Variant)(ref val2)).As<bool>();
		}
	}
}
