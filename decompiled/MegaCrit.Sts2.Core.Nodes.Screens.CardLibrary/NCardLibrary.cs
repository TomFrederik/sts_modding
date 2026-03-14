using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Characters;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.Cards.Holders;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Screens.MainMenu;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.TestSupport;
using MegaCrit.Sts2.Core.Unlocks;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.Nodes.Screens.CardLibrary;

[ScriptPath("res://src/Core/Nodes/Screens/CardLibrary/NCardLibrary.cs")]
public sealed class NCardLibrary : NSubmenu
{
	public new class MethodName : NSubmenu.MethodName
	{
		public static readonly StringName Create = StringName.op_Implicit("Create");

		public new static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName OnCardTypeSort = StringName.op_Implicit("OnCardTypeSort");

		public static readonly StringName OnRaritySort = StringName.op_Implicit("OnRaritySort");

		public static readonly StringName OnCostSort = StringName.op_Implicit("OnCostSort");

		public static readonly StringName OnAlphabetSort = StringName.op_Implicit("OnAlphabetSort");

		public new static readonly StringName OnSubmenuOpened = StringName.op_Implicit("OnSubmenuOpened");

		public new static readonly StringName OnSubmenuClosed = StringName.op_Implicit("OnSubmenuClosed");

		public static readonly StringName ToggleShowStats = StringName.op_Implicit("ToggleShowStats");

		public static readonly StringName ToggleShowUpgrades = StringName.op_Implicit("ToggleShowUpgrades");

		public static readonly StringName ToggleFilterMultiplayerCards = StringName.op_Implicit("ToggleFilterMultiplayerCards");

		public static readonly StringName UpdateCardPoolFilter = StringName.op_Implicit("UpdateCardPoolFilter");

		public static readonly StringName UpdateTypeFilter = StringName.op_Implicit("UpdateTypeFilter");

		public static readonly StringName UpdateRarityFilter = StringName.op_Implicit("UpdateRarityFilter");

		public static readonly StringName UpdateCostFilter = StringName.op_Implicit("UpdateCostFilter");

		public static readonly StringName SearchBarQueryChanged = StringName.op_Implicit("SearchBarQueryChanged");

		public static readonly StringName SearchBarQuerySubmitted = StringName.op_Implicit("SearchBarQuerySubmitted");

		public static readonly StringName UpdateFilter = StringName.op_Implicit("UpdateFilter");

		public static readonly StringName ShowCardDetail = StringName.op_Implicit("ShowCardDetail");
	}

	public new class PropertyName : NSubmenu.PropertyName
	{
		public new static readonly StringName InitialFocusedControl = StringName.op_Implicit("InitialFocusedControl");

		public static readonly StringName _grid = StringName.op_Implicit("_grid");

		public static readonly StringName _searchBar = StringName.op_Implicit("_searchBar");

		public static readonly StringName _ironcladFilter = StringName.op_Implicit("_ironcladFilter");

		public static readonly StringName _silentFilter = StringName.op_Implicit("_silentFilter");

		public static readonly StringName _defectFilter = StringName.op_Implicit("_defectFilter");

		public static readonly StringName _regentFilter = StringName.op_Implicit("_regentFilter");

		public static readonly StringName _necrobinderFilter = StringName.op_Implicit("_necrobinderFilter");

		public static readonly StringName _colorlessFilter = StringName.op_Implicit("_colorlessFilter");

		public static readonly StringName _ancientsFilter = StringName.op_Implicit("_ancientsFilter");

		public static readonly StringName _miscPoolFilter = StringName.op_Implicit("_miscPoolFilter");

		public static readonly StringName _typeSorter = StringName.op_Implicit("_typeSorter");

		public static readonly StringName _attackFilter = StringName.op_Implicit("_attackFilter");

		public static readonly StringName _skillFilter = StringName.op_Implicit("_skillFilter");

		public static readonly StringName _powerFilter = StringName.op_Implicit("_powerFilter");

		public static readonly StringName _otherTypeFilter = StringName.op_Implicit("_otherTypeFilter");

		public static readonly StringName _raritySorter = StringName.op_Implicit("_raritySorter");

		public static readonly StringName _commonFilter = StringName.op_Implicit("_commonFilter");

		public static readonly StringName _uncommonFilter = StringName.op_Implicit("_uncommonFilter");

		public static readonly StringName _rareFilter = StringName.op_Implicit("_rareFilter");

		public static readonly StringName _otherFilter = StringName.op_Implicit("_otherFilter");

		public static readonly StringName _costSorter = StringName.op_Implicit("_costSorter");

		public static readonly StringName _zeroFilter = StringName.op_Implicit("_zeroFilter");

		public static readonly StringName _oneFilter = StringName.op_Implicit("_oneFilter");

		public static readonly StringName _twoFilter = StringName.op_Implicit("_twoFilter");

		public static readonly StringName _threePlusFilter = StringName.op_Implicit("_threePlusFilter");

		public static readonly StringName _xFilter = StringName.op_Implicit("_xFilter");

		public static readonly StringName _alphabetSorter = StringName.op_Implicit("_alphabetSorter");

		public static readonly StringName _viewMultiplayerCards = StringName.op_Implicit("_viewMultiplayerCards");

		public static readonly StringName _viewStats = StringName.op_Implicit("_viewStats");

		public static readonly StringName _viewUpgrades = StringName.op_Implicit("_viewUpgrades");

		public static readonly StringName _cardCountLabel = StringName.op_Implicit("_cardCountLabel");

		public static readonly StringName _noResultsLabel = StringName.op_Implicit("_noResultsLabel");

		public static readonly StringName _lastHoveredControl = StringName.op_Implicit("_lastHoveredControl");
	}

	public new class SignalName : NSubmenu.SignalName
	{
	}

	private const int _delayAfterTextFilterChangedMsec = 250;

	private static readonly string _scenePath = SceneHelper.GetScenePath("screens/card_library/card_library");

	private readonly LocString _cardCountLocString = new LocString("card_library", "CARD_COUNT");

	private readonly LocString _noResultsLocString = new LocString("card_library", "NO_RESULTS");

	private IRunState? _runState;

	private NCardLibraryGrid _grid;

	private NSearchBar _searchBar;

	private readonly Dictionary<string, Func<CardModel, bool>> _specialSearchbarKeywords = new Dictionary<string, Func<CardModel, bool>>();

	private readonly Dictionary<CharacterModel, NCardPoolFilter> _cardPoolFilters = new Dictionary<CharacterModel, NCardPoolFilter>();

	private NCardPoolFilter _ironcladFilter;

	private NCardPoolFilter _silentFilter;

	private NCardPoolFilter _defectFilter;

	private NCardPoolFilter _regentFilter;

	private NCardPoolFilter _necrobinderFilter;

	private NCardPoolFilter _colorlessFilter;

	private NCardPoolFilter _ancientsFilter;

	private NCardPoolFilter _miscPoolFilter;

	private readonly Dictionary<NCardPoolFilter, Func<CardModel, bool>> _poolFilters = new Dictionary<NCardPoolFilter, Func<CardModel, bool>>();

	private NCardViewSortButton _typeSorter;

	private NCardTypeTickbox _attackFilter;

	private NCardTypeTickbox _skillFilter;

	private NCardTypeTickbox _powerFilter;

	private NCardTypeTickbox _otherTypeFilter;

	private readonly Dictionary<NCardTypeTickbox, Func<CardModel, bool>> _cardTypeFilters = new Dictionary<NCardTypeTickbox, Func<CardModel, bool>>();

	private NCardViewSortButton _raritySorter;

	private NCardRarityTickbox _commonFilter;

	private NCardRarityTickbox _uncommonFilter;

	private NCardRarityTickbox _rareFilter;

	private NCardRarityTickbox _otherFilter;

	private readonly Dictionary<NCardRarityTickbox, Func<CardModel, bool>> _rarityFilters = new Dictionary<NCardRarityTickbox, Func<CardModel, bool>>();

	private NCardViewSortButton _costSorter;

	private NCardCostTickbox _zeroFilter;

	private NCardCostTickbox _oneFilter;

	private NCardCostTickbox _twoFilter;

	private NCardCostTickbox _threePlusFilter;

	private NCardCostTickbox _xFilter;

	private readonly Dictionary<NCardCostTickbox, Func<CardModel, bool>> _costFilters = new Dictionary<NCardCostTickbox, Func<CardModel, bool>>();

	private NCardViewSortButton _alphabetSorter;

	private NLibraryStatTickbox _viewMultiplayerCards;

	private NLibraryStatTickbox _viewStats;

	private NLibraryStatTickbox _viewUpgrades;

	private MegaRichTextLabel _cardCountLabel;

	private MegaRichTextLabel _noResultsLabel;

	private CancellationTokenSource? _displayCardsShortDelayCancelToken;

	private readonly List<SortingOrders> _sortingPriority;

	private Func<CardModel, bool> _filter;

	private Control? _lastHoveredControl;

	public static string[] AssetPaths => new string[1] { _scenePath };

	protected override Control InitialFocusedControl => (Control)(((object)_lastHoveredControl) ?? ((object)_ironcladFilter));

	public static NCardLibrary? Create()
	{
		if (TestMode.IsOn)
		{
			return null;
		}
		return PreloadManager.Cache.GetScene(_scenePath).Instantiate<NCardLibrary>((GenEditState)0);
	}

	public override void _Ready()
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0241: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		//IL_0407: Unknown result type (might be due to invalid IL or missing references)
		//IL_040d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0477: Unknown result type (might be due to invalid IL or missing references)
		//IL_047c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0488: Unknown result type (might be due to invalid IL or missing references)
		//IL_048a: Unknown result type (might be due to invalid IL or missing references)
		//IL_049b: Unknown result type (might be due to invalid IL or missing references)
		//IL_049d: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_05bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0626: Unknown result type (might be due to invalid IL or missing references)
		//IL_062b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0637: Unknown result type (might be due to invalid IL or missing references)
		//IL_0639: Unknown result type (might be due to invalid IL or missing references)
		//IL_064a: Unknown result type (might be due to invalid IL or missing references)
		//IL_064c: Unknown result type (might be due to invalid IL or missing references)
		//IL_065d: Unknown result type (might be due to invalid IL or missing references)
		//IL_065f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0670: Unknown result type (might be due to invalid IL or missing references)
		//IL_0672: Unknown result type (might be due to invalid IL or missing references)
		//IL_0765: Unknown result type (might be due to invalid IL or missing references)
		//IL_076b: Unknown result type (might be due to invalid IL or missing references)
		//IL_07eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_07fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_07fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_080f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0811: Unknown result type (might be due to invalid IL or missing references)
		//IL_0822: Unknown result type (might be due to invalid IL or missing references)
		//IL_0824: Unknown result type (might be due to invalid IL or missing references)
		//IL_0835: Unknown result type (might be due to invalid IL or missing references)
		//IL_0837: Unknown result type (might be due to invalid IL or missing references)
		//IL_0848: Unknown result type (might be due to invalid IL or missing references)
		//IL_084a: Unknown result type (might be due to invalid IL or missing references)
		//IL_096d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0973: Unknown result type (might be due to invalid IL or missing references)
		//IL_09d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_09d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_09f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_09fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a18: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a1e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e56: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e5c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e79: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e7f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e9c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ea2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ebf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ec5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ee2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ee8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f05: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f0b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f28: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f2e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f4b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f51: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f6e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f74: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f91: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f97: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fb4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fba: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fd7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fdd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ffa: Unknown result type (might be due to invalid IL or missing references)
		//IL_1000: Unknown result type (might be due to invalid IL or missing references)
		//IL_101d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1023: Unknown result type (might be due to invalid IL or missing references)
		//IL_1040: Unknown result type (might be due to invalid IL or missing references)
		//IL_1046: Unknown result type (might be due to invalid IL or missing references)
		//IL_1063: Unknown result type (might be due to invalid IL or missing references)
		//IL_1069: Unknown result type (might be due to invalid IL or missing references)
		//IL_1086: Unknown result type (might be due to invalid IL or missing references)
		//IL_108c: Unknown result type (might be due to invalid IL or missing references)
		//IL_10a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_10af: Unknown result type (might be due to invalid IL or missing references)
		//IL_10cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_10d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_10ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_10f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_1112: Unknown result type (might be due to invalid IL or missing references)
		//IL_1118: Unknown result type (might be due to invalid IL or missing references)
		//IL_1135: Unknown result type (might be due to invalid IL or missing references)
		//IL_113b: Unknown result type (might be due to invalid IL or missing references)
		ConnectSignals();
		_grid = ((Node)this).GetNode<NCardLibraryGrid>(NodePath.op_Implicit("%CardGrid"));
		((GodotObject)_grid).Connect(NCardGrid.SignalName.HolderPressed, Callable.From<NCardHolder>((Action<NCardHolder>)ShowCardDetail), 0u);
		((GodotObject)_grid).Connect(NCardGrid.SignalName.HolderAltPressed, Callable.From<NCardHolder>((Action<NCardHolder>)ShowCardDetail), 0u);
		_cardCountLabel = ((Node)this).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("%CardCountLabel"));
		_noResultsLabel = ((Node)this).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("%NoResultsLabel"));
		_noResultsLabel.Text = _noResultsLocString.GetFormattedText();
		_searchBar = ((Node)this).GetNode<NSearchBar>(NodePath.op_Implicit("%SearchBar"));
		((GodotObject)_searchBar).Connect(NSearchBar.SignalName.QueryChanged, Callable.From<string>((Action<string>)SearchBarQueryChanged), 0u);
		((GodotObject)_searchBar).Connect(NSearchBar.SignalName.QuerySubmitted, Callable.From<string>((Action<string>)SearchBarQuerySubmitted), 0u);
		_ironcladFilter = ((Node)this).GetNode<NCardPoolFilter>(NodePath.op_Implicit("%IroncladPool"));
		_silentFilter = ((Node)this).GetNode<NCardPoolFilter>(NodePath.op_Implicit("%SilentPool"));
		_defectFilter = ((Node)this).GetNode<NCardPoolFilter>(NodePath.op_Implicit("%DefectPool"));
		_regentFilter = ((Node)this).GetNode<NCardPoolFilter>(NodePath.op_Implicit("%RegentPool"));
		_necrobinderFilter = ((Node)this).GetNode<NCardPoolFilter>(NodePath.op_Implicit("%NecrobinderPool"));
		_colorlessFilter = ((Node)this).GetNode<NCardPoolFilter>(NodePath.op_Implicit("%ColorlessPool"));
		_ancientsFilter = ((Node)this).GetNode<NCardPoolFilter>(NodePath.op_Implicit("%AncientsPool"));
		_miscPoolFilter = ((Node)this).GetNode<NCardPoolFilter>(NodePath.op_Implicit("%MiscPool"));
		Callable val = Callable.From<NCardPoolFilter>((Action<NCardPoolFilter>)UpdateCardPoolFilter);
		((GodotObject)_ironcladFilter).Connect(NCardPoolFilter.SignalName.Toggled, val, 0u);
		((GodotObject)_silentFilter).Connect(NCardPoolFilter.SignalName.Toggled, val, 0u);
		((GodotObject)_defectFilter).Connect(NCardPoolFilter.SignalName.Toggled, val, 0u);
		((GodotObject)_regentFilter).Connect(NCardPoolFilter.SignalName.Toggled, val, 0u);
		((GodotObject)_necrobinderFilter).Connect(NCardPoolFilter.SignalName.Toggled, val, 0u);
		((GodotObject)_colorlessFilter).Connect(NCardPoolFilter.SignalName.Toggled, val, 0u);
		((GodotObject)_ancientsFilter).Connect(NCardPoolFilter.SignalName.Toggled, val, 0u);
		((GodotObject)_miscPoolFilter).Connect(NCardPoolFilter.SignalName.Toggled, val, 0u);
		_poolFilters.Add(_ironcladFilter, (CardModel c) => c.Pool is IroncladCardPool);
		_poolFilters.Add(_silentFilter, (CardModel c) => c.Pool is SilentCardPool);
		_poolFilters.Add(_defectFilter, (CardModel c) => c.Pool is DefectCardPool);
		_poolFilters.Add(_regentFilter, (CardModel c) => c.Pool is RegentCardPool);
		_poolFilters.Add(_necrobinderFilter, (CardModel c) => c.Pool is NecrobinderCardPool);
		_poolFilters.Add(_colorlessFilter, (CardModel c) => c.Pool is ColorlessCardPool);
		_poolFilters.Add(_ancientsFilter, (CardModel c) => c.Rarity == CardRarity.Ancient);
		_poolFilters.Add(_miscPoolFilter, delegate(CardModel c)
		{
			CardRarity rarity = c.Rarity;
			return (uint)(rarity - 6) <= 4u;
		});
		_typeSorter = ((Node)this).GetNode<NCardViewSortButton>(NodePath.op_Implicit("%CardTypeSorter"));
		((GodotObject)_typeSorter).Connect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)OnCardTypeSort), 0u);
		_attackFilter = ((Node)this).GetNode<NCardTypeTickbox>(NodePath.op_Implicit("%AttackType"));
		_skillFilter = ((Node)this).GetNode<NCardTypeTickbox>(NodePath.op_Implicit("%SkillType"));
		_powerFilter = ((Node)this).GetNode<NCardTypeTickbox>(NodePath.op_Implicit("%PowerType"));
		_otherTypeFilter = ((Node)this).GetNode<NCardTypeTickbox>(NodePath.op_Implicit("%OtherType"));
		Callable val2 = Callable.From<NCardTypeTickbox>((Action<NCardTypeTickbox>)UpdateTypeFilter);
		((GodotObject)_attackFilter).Connect(NCardTypeTickbox.SignalName.Toggled, val2, 0u);
		((GodotObject)_skillFilter).Connect(NCardTypeTickbox.SignalName.Toggled, val2, 0u);
		((GodotObject)_powerFilter).Connect(NCardTypeTickbox.SignalName.Toggled, val2, 0u);
		((GodotObject)_otherTypeFilter).Connect(NCardTypeTickbox.SignalName.Toggled, val2, 0u);
		_cardTypeFilters.Add(_attackFilter, (CardModel c) => c.Type == CardType.Attack);
		_cardTypeFilters.Add(_skillFilter, (CardModel c) => c.Type == CardType.Skill);
		_cardTypeFilters.Add(_powerFilter, (CardModel c) => c.Type == CardType.Power);
		_cardTypeFilters.Add(_otherTypeFilter, delegate(CardModel c)
		{
			CardType type = c.Type;
			bool flag = (uint)(type - 1) <= 2u;
			return !flag;
		});
		_raritySorter = ((Node)this).GetNode<NCardViewSortButton>(NodePath.op_Implicit("%RaritySorter"));
		((GodotObject)_raritySorter).Connect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)OnRaritySort), 0u);
		_commonFilter = ((Node)this).GetNode<NCardRarityTickbox>(NodePath.op_Implicit("%CommonRarity"));
		_uncommonFilter = ((Node)this).GetNode<NCardRarityTickbox>(NodePath.op_Implicit("%UncommonRarity"));
		_rareFilter = ((Node)this).GetNode<NCardRarityTickbox>(NodePath.op_Implicit("%RareRarity"));
		_otherFilter = ((Node)this).GetNode<NCardRarityTickbox>(NodePath.op_Implicit("%OtherRarity"));
		Callable val3 = Callable.From<NTickbox>((Action<NTickbox>)UpdateRarityFilter);
		((GodotObject)_commonFilter).Connect(NTickbox.SignalName.Toggled, val3, 0u);
		((GodotObject)_uncommonFilter).Connect(NTickbox.SignalName.Toggled, val3, 0u);
		((GodotObject)_rareFilter).Connect(NTickbox.SignalName.Toggled, val3, 0u);
		((GodotObject)_otherFilter).Connect(NTickbox.SignalName.Toggled, val3, 0u);
		_rarityFilters.Add(_commonFilter, (CardModel c) => c.Rarity == CardRarity.Common);
		_rarityFilters.Add(_uncommonFilter, (CardModel c) => c.Rarity == CardRarity.Uncommon);
		_rarityFilters.Add(_rareFilter, (CardModel c) => c.Rarity == CardRarity.Rare);
		_rarityFilters.Add(_otherFilter, delegate(CardModel c)
		{
			CardRarity rarity = c.Rarity;
			bool flag = (uint)(rarity - 2) <= 2u;
			return !flag;
		});
		_costSorter = ((Node)this).GetNode<NCardViewSortButton>(NodePath.op_Implicit("%CostSorter"));
		((GodotObject)_costSorter).Connect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)OnCostSort), 0u);
		_zeroFilter = ((Node)this).GetNode<NCardCostTickbox>(NodePath.op_Implicit("%Cost0"));
		_oneFilter = ((Node)this).GetNode<NCardCostTickbox>(NodePath.op_Implicit("%Cost1"));
		_twoFilter = ((Node)this).GetNode<NCardCostTickbox>(NodePath.op_Implicit("%Cost2"));
		_threePlusFilter = ((Node)this).GetNode<NCardCostTickbox>(NodePath.op_Implicit("%Cost3+"));
		_xFilter = ((Node)this).GetNode<NCardCostTickbox>(NodePath.op_Implicit("%CostX"));
		Callable val4 = Callable.From<NCardCostTickbox>((Action<NCardCostTickbox>)UpdateCostFilter);
		((GodotObject)_zeroFilter).Connect(NClickableControl.SignalName.Released, val4, 0u);
		((GodotObject)_oneFilter).Connect(NClickableControl.SignalName.Released, val4, 0u);
		((GodotObject)_twoFilter).Connect(NClickableControl.SignalName.Released, val4, 0u);
		((GodotObject)_threePlusFilter).Connect(NClickableControl.SignalName.Released, val4, 0u);
		((GodotObject)_xFilter).Connect(NClickableControl.SignalName.Released, val4, 0u);
		_costFilters.Add(_zeroFilter, delegate(CardModel c)
		{
			CardEnergyCost energyCost = c.EnergyCost;
			return energyCost != null && energyCost.Canonical <= 0 && !energyCost.CostsX;
		});
		_costFilters.Add(_oneFilter, (CardModel c) => c.EnergyCost.Canonical == 1);
		_costFilters.Add(_twoFilter, (CardModel c) => c.EnergyCost.Canonical == 2);
		_costFilters.Add(_threePlusFilter, (CardModel c) => c.EnergyCost.Canonical >= 3);
		_costFilters.Add(_xFilter, (CardModel c) => c.EnergyCost.CostsX || c.HasStarCostX);
		_alphabetSorter = ((Node)this).GetNode<NCardViewSortButton>(NodePath.op_Implicit("%AlphabetSorter"));
		((GodotObject)_alphabetSorter).Connect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)OnAlphabetSort), 0u);
		_viewStats = ((Node)this).GetNode<NLibraryStatTickbox>(NodePath.op_Implicit("%Stats"));
		_viewUpgrades = ((Node)this).GetNode<NLibraryStatTickbox>(NodePath.op_Implicit("%Upgrades"));
		_viewMultiplayerCards = ((Node)this).GetNode<NLibraryStatTickbox>(NodePath.op_Implicit("%MultiplayerCards"));
		((GodotObject)_viewStats).Connect(NTickbox.SignalName.Toggled, Callable.From<NTickbox>((Action<NTickbox>)ToggleShowStats), 0u);
		((GodotObject)_viewUpgrades).Connect(NTickbox.SignalName.Toggled, Callable.From<NTickbox>((Action<NTickbox>)ToggleShowUpgrades), 0u);
		((GodotObject)_viewMultiplayerCards).Connect(NTickbox.SignalName.Toggled, Callable.From<NTickbox>((Action<NTickbox>)ToggleFilterMultiplayerCards), 0u);
		_typeSorter.SetLabel(new LocString("gameplay_ui", "SORT_TYPE").GetRawText());
		_raritySorter.SetLabel(new LocString("gameplay_ui", "SORT_RARITY").GetRawText());
		_costSorter.SetLabel(new LocString("gameplay_ui", "SORT_COST").GetRawText());
		_alphabetSorter.SetLabel(new LocString("gameplay_ui", "SORT_ALPHABET").GetRawText());
		_commonFilter.SetLabel(new LocString("card_library", "RARITY_COMMON").GetRawText());
		_uncommonFilter.SetLabel(new LocString("card_library", "RARITY_UNCOMMON").GetRawText());
		_rareFilter.SetLabel(new LocString("card_library", "RARITY_RARE").GetRawText());
		_otherFilter.SetLabel(new LocString("card_library", "RARITY_OTHER").GetRawText());
		_viewStats.SetLabel(new LocString("card_library", "VIEW_STATS").GetRawText());
		_viewUpgrades.SetLabel(new LocString("card_library", "VIEW_UPGRADES").GetRawText());
		_viewMultiplayerCards.SetLabel(new LocString("card_library", "VIEW_MULTIPLAYER_CARDS").GetRawText());
		_colorlessFilter.Loc = new LocString("card_library", "POOL_COLORLESS_TIP");
		_ancientsFilter.Loc = new LocString("card_library", "POOL_ANCIENT_TIP");
		_miscPoolFilter.Loc = new LocString("card_library", "POOL_MISC_TIP");
		_attackFilter.Loc = new LocString("card_library", "TYPE_ATTACK_TIP");
		_skillFilter.Loc = new LocString("card_library", "TYPE_SKILL_TIP");
		_powerFilter.Loc = new LocString("card_library", "TYPE_POWER_TIP");
		_otherTypeFilter.Loc = new LocString("card_library", "TYPE_OTHER_TIP");
		_commonFilter.Loc = new LocString("card_library", "RARITY_COMMON_TIP");
		_uncommonFilter.Loc = new LocString("card_library", "RARITY_UNCOMMON_TIP");
		_rareFilter.Loc = new LocString("card_library", "RARITY_RARE_TIP");
		_otherFilter.Loc = new LocString("card_library", "RARITY_OTHER_TIP");
		_zeroFilter.Loc = new LocString("card_library", "COST_ZERO_TIP");
		_oneFilter.Loc = new LocString("card_library", "COST_ONE_TIP");
		_twoFilter.Loc = new LocString("card_library", "COST_TWO_TIP");
		_threePlusFilter.Loc = new LocString("card_library", "COST_THREE_TIP");
		_xFilter.Loc = new LocString("card_library", "COST_X_TIP");
		_cardPoolFilters.Add(ModelDb.Character<Ironclad>(), _ironcladFilter);
		_cardPoolFilters.Add(ModelDb.Character<Silent>(), _silentFilter);
		_cardPoolFilters.Add(ModelDb.Character<Defect>(), _defectFilter);
		_cardPoolFilters.Add(ModelDb.Character<Necrobinder>(), _necrobinderFilter);
		_cardPoolFilters.Add(ModelDb.Character<Regent>(), _regentFilter);
		UnlockState unlockState = SaveManager.Instance.GenerateUnlockStateFromProgress();
		foreach (KeyValuePair<CharacterModel, NCardPoolFilter> cardPoolFilter in _cardPoolFilters)
		{
			((CanvasItem)cardPoolFilter.Value).Visible = unlockState.Characters.Contains(cardPoolFilter.Key);
		}
		CardRarity[] values = Enum.GetValues<CardRarity>();
		for (int num = 0; num < values.Length; num++)
		{
			CardRarity keyword = values[num];
			_specialSearchbarKeywords.Add(keyword.ToString().ToLowerInvariant(), (CardModel c) => c.Rarity == keyword);
		}
		((GodotObject)_ironcladFilter).Connect(SignalName.FocusEntered, Callable.From((Action)delegate
		{
			_lastHoveredControl = (Control?)(object)_ironcladFilter;
		}), 0u);
		((GodotObject)_silentFilter).Connect(SignalName.FocusEntered, Callable.From((Action)delegate
		{
			_lastHoveredControl = (Control?)(object)_silentFilter;
		}), 0u);
		((GodotObject)_defectFilter).Connect(SignalName.FocusEntered, Callable.From((Action)delegate
		{
			_lastHoveredControl = (Control?)(object)_defectFilter;
		}), 0u);
		((GodotObject)_regentFilter).Connect(SignalName.FocusEntered, Callable.From((Action)delegate
		{
			_lastHoveredControl = (Control?)(object)_regentFilter;
		}), 0u);
		((GodotObject)_necrobinderFilter).Connect(SignalName.FocusEntered, Callable.From((Action)delegate
		{
			_lastHoveredControl = (Control?)(object)_necrobinderFilter;
		}), 0u);
		((GodotObject)_colorlessFilter).Connect(SignalName.FocusEntered, Callable.From((Action)delegate
		{
			_lastHoveredControl = (Control?)(object)_colorlessFilter;
		}), 0u);
		((GodotObject)_ancientsFilter).Connect(SignalName.FocusEntered, Callable.From((Action)delegate
		{
			_lastHoveredControl = (Control?)(object)_ancientsFilter;
		}), 0u);
		((GodotObject)_miscPoolFilter).Connect(SignalName.FocusEntered, Callable.From((Action)delegate
		{
			_lastHoveredControl = (Control?)(object)_miscPoolFilter;
		}), 0u);
		((GodotObject)_attackFilter).Connect(SignalName.FocusEntered, Callable.From((Action)delegate
		{
			_lastHoveredControl = (Control?)(object)_attackFilter;
		}), 0u);
		((GodotObject)_skillFilter).Connect(SignalName.FocusEntered, Callable.From((Action)delegate
		{
			_lastHoveredControl = (Control?)(object)_skillFilter;
		}), 0u);
		((GodotObject)_powerFilter).Connect(SignalName.FocusEntered, Callable.From((Action)delegate
		{
			_lastHoveredControl = (Control?)(object)_powerFilter;
		}), 0u);
		((GodotObject)_otherTypeFilter).Connect(SignalName.FocusEntered, Callable.From((Action)delegate
		{
			_lastHoveredControl = (Control?)(object)_otherTypeFilter;
		}), 0u);
		((GodotObject)_commonFilter).Connect(SignalName.FocusEntered, Callable.From((Action)delegate
		{
			_lastHoveredControl = (Control?)(object)_commonFilter;
		}), 0u);
		((GodotObject)_uncommonFilter).Connect(SignalName.FocusEntered, Callable.From((Action)delegate
		{
			_lastHoveredControl = (Control?)(object)_uncommonFilter;
		}), 0u);
		((GodotObject)_rareFilter).Connect(SignalName.FocusEntered, Callable.From((Action)delegate
		{
			_lastHoveredControl = (Control?)(object)_rareFilter;
		}), 0u);
		((GodotObject)_otherFilter).Connect(SignalName.FocusEntered, Callable.From((Action)delegate
		{
			_lastHoveredControl = (Control?)(object)_otherFilter;
		}), 0u);
		((GodotObject)_zeroFilter).Connect(SignalName.FocusEntered, Callable.From((Action)delegate
		{
			_lastHoveredControl = (Control?)(object)_zeroFilter;
		}), 0u);
		((GodotObject)_oneFilter).Connect(SignalName.FocusEntered, Callable.From((Action)delegate
		{
			_lastHoveredControl = (Control?)(object)_oneFilter;
		}), 0u);
		((GodotObject)_twoFilter).Connect(SignalName.FocusEntered, Callable.From((Action)delegate
		{
			_lastHoveredControl = (Control?)(object)_twoFilter;
		}), 0u);
		((GodotObject)_threePlusFilter).Connect(SignalName.FocusEntered, Callable.From((Action)delegate
		{
			_lastHoveredControl = (Control?)(object)_threePlusFilter;
		}), 0u);
		((GodotObject)_alphabetSorter).Connect(SignalName.FocusEntered, Callable.From((Action)delegate
		{
			_lastHoveredControl = (Control?)(object)_alphabetSorter;
		}), 0u);
		((GodotObject)_viewUpgrades).Connect(SignalName.FocusEntered, Callable.From((Action)delegate
		{
			_lastHoveredControl = (Control?)(object)_viewUpgrades;
		}), 0u);
	}

	public void Initialize(IRunState runState)
	{
		_runState = runState;
	}

	private void OnCardTypeSort(NButton button)
	{
		_sortingPriority.Remove(SortingOrders.TypeAscending);
		_sortingPriority.Remove(SortingOrders.TypeDescending);
		if (_typeSorter.IsDescending)
		{
			_sortingPriority.Insert(0, SortingOrders.TypeDescending);
		}
		else
		{
			_sortingPriority.Insert(0, SortingOrders.TypeAscending);
		}
		TaskHelper.RunSafely(DisplayCards());
	}

	private void OnRaritySort(NButton button)
	{
		_sortingPriority.Remove(SortingOrders.RarityAscending);
		_sortingPriority.Remove(SortingOrders.RarityDescending);
		if (_raritySorter.IsDescending)
		{
			_sortingPriority.Insert(0, SortingOrders.RarityAscending);
		}
		else
		{
			_sortingPriority.Insert(0, SortingOrders.RarityDescending);
		}
		TaskHelper.RunSafely(DisplayCards());
	}

	private void OnCostSort(NButton button)
	{
		_sortingPriority.Remove(SortingOrders.CostAscending);
		_sortingPriority.Remove(SortingOrders.CostDescending);
		if (_costSorter.IsDescending)
		{
			_sortingPriority.Insert(0, SortingOrders.CostDescending);
		}
		else
		{
			_sortingPriority.Insert(0, SortingOrders.CostAscending);
		}
		TaskHelper.RunSafely(DisplayCards());
	}

	private void OnAlphabetSort(NButton button)
	{
		_sortingPriority.Remove(SortingOrders.AlphabetAscending);
		_sortingPriority.Remove(SortingOrders.AlphabetDescending);
		if (_alphabetSorter.IsDescending)
		{
			_sortingPriority.Insert(0, SortingOrders.AlphabetDescending);
		}
		else
		{
			_sortingPriority.Insert(0, SortingOrders.AlphabetAscending);
		}
		TaskHelper.RunSafely(DisplayCards());
	}

	public override void OnSubmenuOpened()
	{
		_grid.RefreshVisibility();
		CharacterModel characterModel = LocalContext.GetMe(_runState)?.Character;
		_searchBar.ClearText();
		if (characterModel != null)
		{
			foreach (NCardPoolFilter key in _poolFilters.Keys)
			{
				key.IsSelected = _cardPoolFilters[characterModel] == key;
			}
		}
		else
		{
			foreach (NCardPoolFilter key2 in _poolFilters.Keys)
			{
				key2.IsSelected = key2 == _ironcladFilter;
			}
		}
		foreach (NCardTypeTickbox key3 in _cardTypeFilters.Keys)
		{
			key3.IsTicked = false;
		}
		foreach (NCardRarityTickbox key4 in _rarityFilters.Keys)
		{
			key4.IsTicked = false;
		}
		foreach (NCardCostTickbox key5 in _costFilters.Keys)
		{
			key5.IsTicked = false;
		}
		_typeSorter.IsDescending = true;
		_raritySorter.IsDescending = true;
		_costSorter.IsDescending = true;
		_alphabetSorter.IsDescending = true;
		_viewUpgrades.IsTicked = false;
		_viewStats.IsTicked = false;
		_viewMultiplayerCards.IsTicked = true;
		ToggleShowStats(_viewStats);
		ToggleShowUpgrades(_viewUpgrades);
		UpdateFilter();
	}

	public override void OnSubmenuClosed()
	{
		_grid.ClearGrid();
	}

	private async Task DisplayCardsAfterShortDelay()
	{
		if (_displayCardsShortDelayCancelToken != null)
		{
			await _displayCardsShortDelayCancelToken.CancelAsync();
		}
		if (!_grid.IsAnimatingOut)
		{
			TaskHelper.RunSafely(_grid.AnimateOut());
		}
		CancellationTokenSource cancelToken = (_displayCardsShortDelayCancelToken = new CancellationTokenSource());
		await Task.Delay(250, cancelToken.Token);
		if (!cancelToken.IsCancellationRequested)
		{
			await DisplayCards();
		}
	}

	private async Task DisplayCards()
	{
		if (_displayCardsShortDelayCancelToken != null)
		{
			await _displayCardsShortDelayCancelToken.CancelAsync();
		}
		await Task.Yield();
		_grid.FilterCards(_filter, _sortingPriority);
		_cardCountLocString.Add("Amount", _grid.VisibleCards.Count());
		_cardCountLabel.Text = "[center]" + _cardCountLocString.GetFormattedText() + "[/center]";
		((CanvasItem)_noResultsLabel).Visible = !_grid.VisibleCards.Any();
	}

	private void ToggleShowStats(NTickbox tickbox)
	{
		_grid.ShowStats = tickbox.IsTicked;
	}

	private void ToggleShowUpgrades(NTickbox tickbox)
	{
		_grid.IsShowingUpgrades = tickbox.IsTicked;
		if (!string.IsNullOrWhiteSpace(_searchBar.Text))
		{
			UpdateFilter();
		}
	}

	private void ToggleFilterMultiplayerCards(NTickbox tickbox)
	{
		UpdateFilter();
	}

	private void UpdateCardPoolFilter(NCardPoolFilter filter)
	{
		if (filter.IsSelected)
		{
			foreach (NCardPoolFilter key2 in _poolFilters.Keys)
			{
				if (key2 != filter)
				{
					key2.IsSelected = false;
				}
			}
		}
		bool flag = true;
		foreach (KeyValuePair<NCardPoolFilter, Func<CardModel, bool>> poolFilter in _poolFilters)
		{
			NCardPoolFilter key = poolFilter.Key;
			if (key.IsSelected && key != _miscPoolFilter && key != _ancientsFilter)
			{
				flag = false;
				break;
			}
		}
		foreach (NCardRarityTickbox key3 in _rarityFilters.Keys)
		{
			if (flag)
			{
				key3.Disable();
			}
			else
			{
				key3.Enable();
			}
		}
		UpdateFilter();
	}

	private void UpdateTypeFilter(NCardTypeTickbox tickbox)
	{
		UpdateFilter();
	}

	private void UpdateRarityFilter(NTickbox tickbox)
	{
		UpdateFilter();
	}

	private void UpdateCostFilter(NCardCostTickbox tickbox)
	{
		UpdateFilter();
	}

	private void SearchBarQueryChanged(string _ = "")
	{
		UpdateFilter(isTextInput: true);
	}

	private void SearchBarQuerySubmitted(string _ = "")
	{
		UpdateFilter();
	}

	private void UpdateFilter(bool isTextInput = false)
	{
		List<Func<CardModel, bool>> activeRarityFilters = new List<Func<CardModel, bool>>();
		bool flag = true;
		foreach (KeyValuePair<NCardPoolFilter, Func<CardModel, bool>> poolFilter2 in _poolFilters)
		{
			if (poolFilter2.Key.IsSelected && poolFilter2.Key != _miscPoolFilter && poolFilter2.Key != _ancientsFilter)
			{
				flag = false;
				break;
			}
		}
		Func<CardModel, bool> value;
		if (!flag)
		{
			foreach (KeyValuePair<NCardRarityTickbox, Func<CardModel, bool>> rarityFilter in _rarityFilters)
			{
				rarityFilter.Deconstruct(out var key, out value);
				NTickbox nTickbox = key;
				Func<CardModel, bool> item = value;
				if (nTickbox.IsTicked)
				{
					activeRarityFilters.Add(item);
				}
			}
		}
		if (activeRarityFilters.Count == 0)
		{
			activeRarityFilters.Add((CardModel _) => true);
		}
		List<Func<CardModel, bool>> activeCardTypeFilter = new List<Func<CardModel, bool>>();
		foreach (KeyValuePair<NCardTypeTickbox, Func<CardModel, bool>> cardTypeFilter in _cardTypeFilters)
		{
			cardTypeFilter.Deconstruct(out var key2, out value);
			NCardTypeTickbox nCardTypeTickbox = key2;
			Func<CardModel, bool> item2 = value;
			if (nCardTypeTickbox.IsTicked)
			{
				activeCardTypeFilter.Add(item2);
			}
		}
		if (activeCardTypeFilter.Count == 0)
		{
			activeCardTypeFilter.Add((CardModel _) => true);
		}
		List<Func<CardModel, bool>> poolFilter = new List<Func<CardModel, bool>>();
		foreach (KeyValuePair<NCardPoolFilter, Func<CardModel, bool>> poolFilter3 in _poolFilters)
		{
			if (poolFilter3.Key.IsSelected)
			{
				poolFilter.Add(poolFilter3.Value);
			}
		}
		List<Func<CardModel, bool>> activeCostFilter = new List<Func<CardModel, bool>>();
		foreach (KeyValuePair<NCardCostTickbox, Func<CardModel, bool>> costFilter in _costFilters)
		{
			costFilter.Deconstruct(out var key3, out value);
			NCardCostTickbox nCardCostTickbox = key3;
			Func<CardModel, bool> item3 = value;
			if (nCardCostTickbox.IsTicked)
			{
				activeCostFilter.Add(item3);
			}
		}
		if (activeCostFilter.Count == 0)
		{
			activeCostFilter.Add((CardModel _) => true);
		}
		Func<CardModel, bool> multiplayerCardFilter = (CardModel c) => true;
		if (!_viewMultiplayerCards.IsTicked)
		{
			multiplayerCardFilter = (CardModel c) => c.MultiplayerConstraint != CardMultiplayerConstraint.MultiplayerOnly;
		}
		_filter = (CardModel c) => activeCostFilter.Any((Func<CardModel, bool> filter) => filter(c)) && activeRarityFilters.Any((Func<CardModel, bool> filter) => filter(c)) && activeCardTypeFilter.Any((Func<CardModel, bool> filter) => filter(c)) && poolFilter.Any((Func<CardModel, bool> filter) => filter(c)) && TextFilter(c) && multiplayerCardFilter(c);
		Task task = ((!isTextInput) ? DisplayCards() : DisplayCardsAfterShortDelay());
		TaskHelper.RunSafely(task);
		bool TextFilter(CardModel card)
		{
			if (string.IsNullOrWhiteSpace(_searchBar.Text))
			{
				return true;
			}
			if (!SaveManager.Instance.Progress.DiscoveredCards.Contains(card.Id))
			{
				return false;
			}
			string title = card.Title;
			string text;
			if (_viewUpgrades.IsTicked && card.IsUpgradable)
			{
				CardModel cardModel = (CardModel)card.MutableClone();
				cardModel.UpgradeInternal();
				cardModel.UpdateDynamicVarPreview(CardPreviewMode.Upgrade, null, card.DynamicVars);
				text = cardModel.GetDescriptionForUpgradePreview().StripBbCode();
			}
			else
			{
				text = card.GetDescriptionForPile(PileType.None).StripBbCode();
			}
			global::_003C_003Ey__InlineArray2<string> buffer = default(global::_003C_003Ey__InlineArray2<string>);
			buffer[0] = title;
			buffer[1] = NSearchBar.RemoveHtmlTags(text);
			string text2 = string.Join(" ", (ReadOnlySpan<string?>)buffer);
			string text3 = NSearchBar.Normalize(text2);
			string text4 = _searchBar.Text.ToLowerInvariant();
			if (_specialSearchbarKeywords.TryGetValue(text4, out Func<CardModel, bool> value2))
			{
				if (!value2(card))
				{
					return text3.Contains(text4);
				}
				return true;
			}
			return text3.Contains(text4);
		}
	}

	private void ShowCardDetail(NCardHolder holder)
	{
		if (SaveManager.Instance.Progress.DiscoveredCards.Contains(holder.CardModel.Id))
		{
			_lastHoveredControl = (Control?)(object)holder;
			List<CardModel> list = _grid.VisibleCards.Where((CardModel c) => SaveManager.Instance.Progress.DiscoveredCards.Contains(c.Id)).ToList();
			NGame.Instance.GetInspectCardScreen().Open(list, list.IndexOf(holder.CardModel), _viewUpgrades.IsTicked);
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Expected O, but got Unknown
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Expected O, but got Unknown
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Expected O, but got Unknown
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Expected O, but got Unknown
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Expected O, but got Unknown
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_0290: Unknown result type (might be due to invalid IL or missing references)
		//IL_029b: Expected O, but got Unknown
		//IL_0296: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fa: Expected O, but got Unknown
		//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0300: Unknown result type (might be due to invalid IL or missing references)
		//IL_0326: Unknown result type (might be due to invalid IL or missing references)
		//IL_034e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0359: Expected O, but got Unknown
		//IL_0354: Unknown result type (might be due to invalid IL or missing references)
		//IL_035f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0385: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b8: Expected O, but got Unknown
		//IL_03b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03be: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_040c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0417: Expected O, but got Unknown
		//IL_0412: Unknown result type (might be due to invalid IL or missing references)
		//IL_041d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0443: Unknown result type (might be due to invalid IL or missing references)
		//IL_046b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0476: Expected O, but got Unknown
		//IL_0471: Unknown result type (might be due to invalid IL or missing references)
		//IL_047c: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d5: Expected O, but got Unknown
		//IL_04d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04db: Unknown result type (might be due to invalid IL or missing references)
		//IL_0501: Unknown result type (might be due to invalid IL or missing references)
		//IL_0524: Unknown result type (might be due to invalid IL or missing references)
		//IL_052f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0555: Unknown result type (might be due to invalid IL or missing references)
		//IL_0578: Unknown result type (might be due to invalid IL or missing references)
		//IL_0583: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_05cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0625: Unknown result type (might be due to invalid IL or missing references)
		//IL_0630: Expected O, but got Unknown
		//IL_062b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0636: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(19);
		list.Add(new MethodInfo(MethodName.Create, new PropertyInfo((Type)24, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false), (MethodFlags)33, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnCardTypeSort, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("button"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnRaritySort, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("button"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnCostSort, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("button"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnAlphabetSort, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("button"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnSubmenuOpened, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnSubmenuClosed, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ToggleShowStats, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("tickbox"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ToggleShowUpgrades, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("tickbox"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ToggleFilterMultiplayerCards, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("tickbox"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UpdateCardPoolFilter, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("filter"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UpdateTypeFilter, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("tickbox"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UpdateRarityFilter, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("tickbox"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UpdateCostFilter, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("tickbox"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SearchBarQueryChanged, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)4, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SearchBarQuerySubmitted, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)4, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UpdateFilter, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)1, StringName.op_Implicit("isTextInput"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ShowCardDetail, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("holder"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0290: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0329: Unknown result type (might be due to invalid IL or missing references)
		//IL_0399: Unknown result type (might be due to invalid IL or missing references)
		//IL_035c: Unknown result type (might be due to invalid IL or missing references)
		//IL_038f: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.Create && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			NCardLibrary nCardLibrary = Create();
			ret = VariantUtils.CreateFrom<NCardLibrary>(ref nCardLibrary);
			return true;
		}
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnCardTypeSort && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OnCardTypeSort(VariantUtils.ConvertTo<NButton>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnRaritySort && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OnRaritySort(VariantUtils.ConvertTo<NButton>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnCostSort && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OnCostSort(VariantUtils.ConvertTo<NButton>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnAlphabetSort && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OnAlphabetSort(VariantUtils.ConvertTo<NButton>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnSubmenuOpened && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnSubmenuOpened();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnSubmenuClosed && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnSubmenuClosed();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.ToggleShowStats && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			ToggleShowStats(VariantUtils.ConvertTo<NTickbox>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.ToggleShowUpgrades && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			ToggleShowUpgrades(VariantUtils.ConvertTo<NTickbox>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.ToggleFilterMultiplayerCards && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			ToggleFilterMultiplayerCards(VariantUtils.ConvertTo<NTickbox>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.UpdateCardPoolFilter && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			UpdateCardPoolFilter(VariantUtils.ConvertTo<NCardPoolFilter>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.UpdateTypeFilter && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			UpdateTypeFilter(VariantUtils.ConvertTo<NCardTypeTickbox>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.UpdateRarityFilter && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			UpdateRarityFilter(VariantUtils.ConvertTo<NTickbox>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.UpdateCostFilter && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			UpdateCostFilter(VariantUtils.ConvertTo<NCardCostTickbox>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SearchBarQueryChanged && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			SearchBarQueryChanged(VariantUtils.ConvertTo<string>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SearchBarQuerySubmitted && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			SearchBarQuerySubmitted(VariantUtils.ConvertTo<string>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.UpdateFilter && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			UpdateFilter(VariantUtils.ConvertTo<bool>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.ShowCardDetail && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			ShowCardDetail(VariantUtils.ConvertTo<NCardHolder>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		return base.InvokeGodotClassMethod(in method, args, out ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static bool InvokeGodotClassStaticMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.Create && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			NCardLibrary nCardLibrary = Create();
			ret = VariantUtils.CreateFrom<NCardLibrary>(ref nCardLibrary);
			return true;
		}
		ret = default(godot_variant);
		return false;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if ((ref method) == MethodName.Create)
		{
			return true;
		}
		if ((ref method) == MethodName._Ready)
		{
			return true;
		}
		if ((ref method) == MethodName.OnCardTypeSort)
		{
			return true;
		}
		if ((ref method) == MethodName.OnRaritySort)
		{
			return true;
		}
		if ((ref method) == MethodName.OnCostSort)
		{
			return true;
		}
		if ((ref method) == MethodName.OnAlphabetSort)
		{
			return true;
		}
		if ((ref method) == MethodName.OnSubmenuOpened)
		{
			return true;
		}
		if ((ref method) == MethodName.OnSubmenuClosed)
		{
			return true;
		}
		if ((ref method) == MethodName.ToggleShowStats)
		{
			return true;
		}
		if ((ref method) == MethodName.ToggleShowUpgrades)
		{
			return true;
		}
		if ((ref method) == MethodName.ToggleFilterMultiplayerCards)
		{
			return true;
		}
		if ((ref method) == MethodName.UpdateCardPoolFilter)
		{
			return true;
		}
		if ((ref method) == MethodName.UpdateTypeFilter)
		{
			return true;
		}
		if ((ref method) == MethodName.UpdateRarityFilter)
		{
			return true;
		}
		if ((ref method) == MethodName.UpdateCostFilter)
		{
			return true;
		}
		if ((ref method) == MethodName.SearchBarQueryChanged)
		{
			return true;
		}
		if ((ref method) == MethodName.SearchBarQuerySubmitted)
		{
			return true;
		}
		if ((ref method) == MethodName.UpdateFilter)
		{
			return true;
		}
		if ((ref method) == MethodName.ShowCardDetail)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._grid)
		{
			_grid = VariantUtils.ConvertTo<NCardLibraryGrid>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._searchBar)
		{
			_searchBar = VariantUtils.ConvertTo<NSearchBar>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._ironcladFilter)
		{
			_ironcladFilter = VariantUtils.ConvertTo<NCardPoolFilter>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._silentFilter)
		{
			_silentFilter = VariantUtils.ConvertTo<NCardPoolFilter>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._defectFilter)
		{
			_defectFilter = VariantUtils.ConvertTo<NCardPoolFilter>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._regentFilter)
		{
			_regentFilter = VariantUtils.ConvertTo<NCardPoolFilter>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._necrobinderFilter)
		{
			_necrobinderFilter = VariantUtils.ConvertTo<NCardPoolFilter>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._colorlessFilter)
		{
			_colorlessFilter = VariantUtils.ConvertTo<NCardPoolFilter>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._ancientsFilter)
		{
			_ancientsFilter = VariantUtils.ConvertTo<NCardPoolFilter>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._miscPoolFilter)
		{
			_miscPoolFilter = VariantUtils.ConvertTo<NCardPoolFilter>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._typeSorter)
		{
			_typeSorter = VariantUtils.ConvertTo<NCardViewSortButton>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._attackFilter)
		{
			_attackFilter = VariantUtils.ConvertTo<NCardTypeTickbox>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._skillFilter)
		{
			_skillFilter = VariantUtils.ConvertTo<NCardTypeTickbox>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._powerFilter)
		{
			_powerFilter = VariantUtils.ConvertTo<NCardTypeTickbox>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._otherTypeFilter)
		{
			_otherTypeFilter = VariantUtils.ConvertTo<NCardTypeTickbox>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._raritySorter)
		{
			_raritySorter = VariantUtils.ConvertTo<NCardViewSortButton>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._commonFilter)
		{
			_commonFilter = VariantUtils.ConvertTo<NCardRarityTickbox>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._uncommonFilter)
		{
			_uncommonFilter = VariantUtils.ConvertTo<NCardRarityTickbox>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._rareFilter)
		{
			_rareFilter = VariantUtils.ConvertTo<NCardRarityTickbox>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._otherFilter)
		{
			_otherFilter = VariantUtils.ConvertTo<NCardRarityTickbox>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._costSorter)
		{
			_costSorter = VariantUtils.ConvertTo<NCardViewSortButton>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._zeroFilter)
		{
			_zeroFilter = VariantUtils.ConvertTo<NCardCostTickbox>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._oneFilter)
		{
			_oneFilter = VariantUtils.ConvertTo<NCardCostTickbox>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._twoFilter)
		{
			_twoFilter = VariantUtils.ConvertTo<NCardCostTickbox>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._threePlusFilter)
		{
			_threePlusFilter = VariantUtils.ConvertTo<NCardCostTickbox>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._xFilter)
		{
			_xFilter = VariantUtils.ConvertTo<NCardCostTickbox>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._alphabetSorter)
		{
			_alphabetSorter = VariantUtils.ConvertTo<NCardViewSortButton>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._viewMultiplayerCards)
		{
			_viewMultiplayerCards = VariantUtils.ConvertTo<NLibraryStatTickbox>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._viewStats)
		{
			_viewStats = VariantUtils.ConvertTo<NLibraryStatTickbox>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._viewUpgrades)
		{
			_viewUpgrades = VariantUtils.ConvertTo<NLibraryStatTickbox>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._cardCountLabel)
		{
			_cardCountLabel = VariantUtils.ConvertTo<MegaRichTextLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._noResultsLabel)
		{
			_noResultsLabel = VariantUtils.ConvertTo<MegaRichTextLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._lastHoveredControl)
		{
			_lastHoveredControl = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		return base.SetGodotClassPropertyValue(in name, in value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0277: Unknown result type (might be due to invalid IL or missing references)
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0297: Unknown result type (might be due to invalid IL or missing references)
		//IL_029c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0317: Unknown result type (might be due to invalid IL or missing references)
		//IL_031c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0337: Unknown result type (might be due to invalid IL or missing references)
		//IL_033c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0357: Unknown result type (might be due to invalid IL or missing references)
		//IL_035c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0377: Unknown result type (might be due to invalid IL or missing references)
		//IL_037c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0397: Unknown result type (might be due to invalid IL or missing references)
		//IL_039c: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0417: Unknown result type (might be due to invalid IL or missing references)
		//IL_041c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0437: Unknown result type (might be due to invalid IL or missing references)
		//IL_043c: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName.InitialFocusedControl)
		{
			Control initialFocusedControl = InitialFocusedControl;
			value = VariantUtils.CreateFrom<Control>(ref initialFocusedControl);
			return true;
		}
		if ((ref name) == PropertyName._grid)
		{
			value = VariantUtils.CreateFrom<NCardLibraryGrid>(ref _grid);
			return true;
		}
		if ((ref name) == PropertyName._searchBar)
		{
			value = VariantUtils.CreateFrom<NSearchBar>(ref _searchBar);
			return true;
		}
		if ((ref name) == PropertyName._ironcladFilter)
		{
			value = VariantUtils.CreateFrom<NCardPoolFilter>(ref _ironcladFilter);
			return true;
		}
		if ((ref name) == PropertyName._silentFilter)
		{
			value = VariantUtils.CreateFrom<NCardPoolFilter>(ref _silentFilter);
			return true;
		}
		if ((ref name) == PropertyName._defectFilter)
		{
			value = VariantUtils.CreateFrom<NCardPoolFilter>(ref _defectFilter);
			return true;
		}
		if ((ref name) == PropertyName._regentFilter)
		{
			value = VariantUtils.CreateFrom<NCardPoolFilter>(ref _regentFilter);
			return true;
		}
		if ((ref name) == PropertyName._necrobinderFilter)
		{
			value = VariantUtils.CreateFrom<NCardPoolFilter>(ref _necrobinderFilter);
			return true;
		}
		if ((ref name) == PropertyName._colorlessFilter)
		{
			value = VariantUtils.CreateFrom<NCardPoolFilter>(ref _colorlessFilter);
			return true;
		}
		if ((ref name) == PropertyName._ancientsFilter)
		{
			value = VariantUtils.CreateFrom<NCardPoolFilter>(ref _ancientsFilter);
			return true;
		}
		if ((ref name) == PropertyName._miscPoolFilter)
		{
			value = VariantUtils.CreateFrom<NCardPoolFilter>(ref _miscPoolFilter);
			return true;
		}
		if ((ref name) == PropertyName._typeSorter)
		{
			value = VariantUtils.CreateFrom<NCardViewSortButton>(ref _typeSorter);
			return true;
		}
		if ((ref name) == PropertyName._attackFilter)
		{
			value = VariantUtils.CreateFrom<NCardTypeTickbox>(ref _attackFilter);
			return true;
		}
		if ((ref name) == PropertyName._skillFilter)
		{
			value = VariantUtils.CreateFrom<NCardTypeTickbox>(ref _skillFilter);
			return true;
		}
		if ((ref name) == PropertyName._powerFilter)
		{
			value = VariantUtils.CreateFrom<NCardTypeTickbox>(ref _powerFilter);
			return true;
		}
		if ((ref name) == PropertyName._otherTypeFilter)
		{
			value = VariantUtils.CreateFrom<NCardTypeTickbox>(ref _otherTypeFilter);
			return true;
		}
		if ((ref name) == PropertyName._raritySorter)
		{
			value = VariantUtils.CreateFrom<NCardViewSortButton>(ref _raritySorter);
			return true;
		}
		if ((ref name) == PropertyName._commonFilter)
		{
			value = VariantUtils.CreateFrom<NCardRarityTickbox>(ref _commonFilter);
			return true;
		}
		if ((ref name) == PropertyName._uncommonFilter)
		{
			value = VariantUtils.CreateFrom<NCardRarityTickbox>(ref _uncommonFilter);
			return true;
		}
		if ((ref name) == PropertyName._rareFilter)
		{
			value = VariantUtils.CreateFrom<NCardRarityTickbox>(ref _rareFilter);
			return true;
		}
		if ((ref name) == PropertyName._otherFilter)
		{
			value = VariantUtils.CreateFrom<NCardRarityTickbox>(ref _otherFilter);
			return true;
		}
		if ((ref name) == PropertyName._costSorter)
		{
			value = VariantUtils.CreateFrom<NCardViewSortButton>(ref _costSorter);
			return true;
		}
		if ((ref name) == PropertyName._zeroFilter)
		{
			value = VariantUtils.CreateFrom<NCardCostTickbox>(ref _zeroFilter);
			return true;
		}
		if ((ref name) == PropertyName._oneFilter)
		{
			value = VariantUtils.CreateFrom<NCardCostTickbox>(ref _oneFilter);
			return true;
		}
		if ((ref name) == PropertyName._twoFilter)
		{
			value = VariantUtils.CreateFrom<NCardCostTickbox>(ref _twoFilter);
			return true;
		}
		if ((ref name) == PropertyName._threePlusFilter)
		{
			value = VariantUtils.CreateFrom<NCardCostTickbox>(ref _threePlusFilter);
			return true;
		}
		if ((ref name) == PropertyName._xFilter)
		{
			value = VariantUtils.CreateFrom<NCardCostTickbox>(ref _xFilter);
			return true;
		}
		if ((ref name) == PropertyName._alphabetSorter)
		{
			value = VariantUtils.CreateFrom<NCardViewSortButton>(ref _alphabetSorter);
			return true;
		}
		if ((ref name) == PropertyName._viewMultiplayerCards)
		{
			value = VariantUtils.CreateFrom<NLibraryStatTickbox>(ref _viewMultiplayerCards);
			return true;
		}
		if ((ref name) == PropertyName._viewStats)
		{
			value = VariantUtils.CreateFrom<NLibraryStatTickbox>(ref _viewStats);
			return true;
		}
		if ((ref name) == PropertyName._viewUpgrades)
		{
			value = VariantUtils.CreateFrom<NLibraryStatTickbox>(ref _viewUpgrades);
			return true;
		}
		if ((ref name) == PropertyName._cardCountLabel)
		{
			value = VariantUtils.CreateFrom<MegaRichTextLabel>(ref _cardCountLabel);
			return true;
		}
		if ((ref name) == PropertyName._noResultsLabel)
		{
			value = VariantUtils.CreateFrom<MegaRichTextLabel>(ref _noResultsLabel);
			return true;
		}
		if ((ref name) == PropertyName._lastHoveredControl)
		{
			value = VariantUtils.CreateFrom<Control>(ref _lastHoveredControl);
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
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0290: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0314: Unknown result type (might be due to invalid IL or missing references)
		//IL_0335: Unknown result type (might be due to invalid IL or missing references)
		//IL_0356: Unknown result type (might be due to invalid IL or missing references)
		//IL_0377: Unknown result type (might be due to invalid IL or missing references)
		//IL_0398: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03da: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_041c: Unknown result type (might be due to invalid IL or missing references)
		//IL_043d: Unknown result type (might be due to invalid IL or missing references)
		//IL_045e: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._grid, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._searchBar, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._ironcladFilter, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._silentFilter, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._defectFilter, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._regentFilter, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._necrobinderFilter, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._colorlessFilter, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._ancientsFilter, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._miscPoolFilter, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._typeSorter, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._attackFilter, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._skillFilter, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._powerFilter, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._otherTypeFilter, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._raritySorter, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._commonFilter, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._uncommonFilter, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._rareFilter, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._otherFilter, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._costSorter, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._zeroFilter, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._oneFilter, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._twoFilter, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._threePlusFilter, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._xFilter, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._alphabetSorter, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._viewMultiplayerCards, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._viewStats, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._viewUpgrades, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._cardCountLabel, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._noResultsLabel, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._lastHoveredControl, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.InitialFocusedControl, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName._grid, Variant.From<NCardLibraryGrid>(ref _grid));
		info.AddProperty(PropertyName._searchBar, Variant.From<NSearchBar>(ref _searchBar));
		info.AddProperty(PropertyName._ironcladFilter, Variant.From<NCardPoolFilter>(ref _ironcladFilter));
		info.AddProperty(PropertyName._silentFilter, Variant.From<NCardPoolFilter>(ref _silentFilter));
		info.AddProperty(PropertyName._defectFilter, Variant.From<NCardPoolFilter>(ref _defectFilter));
		info.AddProperty(PropertyName._regentFilter, Variant.From<NCardPoolFilter>(ref _regentFilter));
		info.AddProperty(PropertyName._necrobinderFilter, Variant.From<NCardPoolFilter>(ref _necrobinderFilter));
		info.AddProperty(PropertyName._colorlessFilter, Variant.From<NCardPoolFilter>(ref _colorlessFilter));
		info.AddProperty(PropertyName._ancientsFilter, Variant.From<NCardPoolFilter>(ref _ancientsFilter));
		info.AddProperty(PropertyName._miscPoolFilter, Variant.From<NCardPoolFilter>(ref _miscPoolFilter));
		info.AddProperty(PropertyName._typeSorter, Variant.From<NCardViewSortButton>(ref _typeSorter));
		info.AddProperty(PropertyName._attackFilter, Variant.From<NCardTypeTickbox>(ref _attackFilter));
		info.AddProperty(PropertyName._skillFilter, Variant.From<NCardTypeTickbox>(ref _skillFilter));
		info.AddProperty(PropertyName._powerFilter, Variant.From<NCardTypeTickbox>(ref _powerFilter));
		info.AddProperty(PropertyName._otherTypeFilter, Variant.From<NCardTypeTickbox>(ref _otherTypeFilter));
		info.AddProperty(PropertyName._raritySorter, Variant.From<NCardViewSortButton>(ref _raritySorter));
		info.AddProperty(PropertyName._commonFilter, Variant.From<NCardRarityTickbox>(ref _commonFilter));
		info.AddProperty(PropertyName._uncommonFilter, Variant.From<NCardRarityTickbox>(ref _uncommonFilter));
		info.AddProperty(PropertyName._rareFilter, Variant.From<NCardRarityTickbox>(ref _rareFilter));
		info.AddProperty(PropertyName._otherFilter, Variant.From<NCardRarityTickbox>(ref _otherFilter));
		info.AddProperty(PropertyName._costSorter, Variant.From<NCardViewSortButton>(ref _costSorter));
		info.AddProperty(PropertyName._zeroFilter, Variant.From<NCardCostTickbox>(ref _zeroFilter));
		info.AddProperty(PropertyName._oneFilter, Variant.From<NCardCostTickbox>(ref _oneFilter));
		info.AddProperty(PropertyName._twoFilter, Variant.From<NCardCostTickbox>(ref _twoFilter));
		info.AddProperty(PropertyName._threePlusFilter, Variant.From<NCardCostTickbox>(ref _threePlusFilter));
		info.AddProperty(PropertyName._xFilter, Variant.From<NCardCostTickbox>(ref _xFilter));
		info.AddProperty(PropertyName._alphabetSorter, Variant.From<NCardViewSortButton>(ref _alphabetSorter));
		info.AddProperty(PropertyName._viewMultiplayerCards, Variant.From<NLibraryStatTickbox>(ref _viewMultiplayerCards));
		info.AddProperty(PropertyName._viewStats, Variant.From<NLibraryStatTickbox>(ref _viewStats));
		info.AddProperty(PropertyName._viewUpgrades, Variant.From<NLibraryStatTickbox>(ref _viewUpgrades));
		info.AddProperty(PropertyName._cardCountLabel, Variant.From<MegaRichTextLabel>(ref _cardCountLabel));
		info.AddProperty(PropertyName._noResultsLabel, Variant.From<MegaRichTextLabel>(ref _noResultsLabel));
		info.AddProperty(PropertyName._lastHoveredControl, Variant.From<Control>(ref _lastHoveredControl));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._grid, ref val))
		{
			_grid = ((Variant)(ref val)).As<NCardLibraryGrid>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._searchBar, ref val2))
		{
			_searchBar = ((Variant)(ref val2)).As<NSearchBar>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._ironcladFilter, ref val3))
		{
			_ironcladFilter = ((Variant)(ref val3)).As<NCardPoolFilter>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._silentFilter, ref val4))
		{
			_silentFilter = ((Variant)(ref val4)).As<NCardPoolFilter>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._defectFilter, ref val5))
		{
			_defectFilter = ((Variant)(ref val5)).As<NCardPoolFilter>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._regentFilter, ref val6))
		{
			_regentFilter = ((Variant)(ref val6)).As<NCardPoolFilter>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName._necrobinderFilter, ref val7))
		{
			_necrobinderFilter = ((Variant)(ref val7)).As<NCardPoolFilter>();
		}
		Variant val8 = default(Variant);
		if (info.TryGetProperty(PropertyName._colorlessFilter, ref val8))
		{
			_colorlessFilter = ((Variant)(ref val8)).As<NCardPoolFilter>();
		}
		Variant val9 = default(Variant);
		if (info.TryGetProperty(PropertyName._ancientsFilter, ref val9))
		{
			_ancientsFilter = ((Variant)(ref val9)).As<NCardPoolFilter>();
		}
		Variant val10 = default(Variant);
		if (info.TryGetProperty(PropertyName._miscPoolFilter, ref val10))
		{
			_miscPoolFilter = ((Variant)(ref val10)).As<NCardPoolFilter>();
		}
		Variant val11 = default(Variant);
		if (info.TryGetProperty(PropertyName._typeSorter, ref val11))
		{
			_typeSorter = ((Variant)(ref val11)).As<NCardViewSortButton>();
		}
		Variant val12 = default(Variant);
		if (info.TryGetProperty(PropertyName._attackFilter, ref val12))
		{
			_attackFilter = ((Variant)(ref val12)).As<NCardTypeTickbox>();
		}
		Variant val13 = default(Variant);
		if (info.TryGetProperty(PropertyName._skillFilter, ref val13))
		{
			_skillFilter = ((Variant)(ref val13)).As<NCardTypeTickbox>();
		}
		Variant val14 = default(Variant);
		if (info.TryGetProperty(PropertyName._powerFilter, ref val14))
		{
			_powerFilter = ((Variant)(ref val14)).As<NCardTypeTickbox>();
		}
		Variant val15 = default(Variant);
		if (info.TryGetProperty(PropertyName._otherTypeFilter, ref val15))
		{
			_otherTypeFilter = ((Variant)(ref val15)).As<NCardTypeTickbox>();
		}
		Variant val16 = default(Variant);
		if (info.TryGetProperty(PropertyName._raritySorter, ref val16))
		{
			_raritySorter = ((Variant)(ref val16)).As<NCardViewSortButton>();
		}
		Variant val17 = default(Variant);
		if (info.TryGetProperty(PropertyName._commonFilter, ref val17))
		{
			_commonFilter = ((Variant)(ref val17)).As<NCardRarityTickbox>();
		}
		Variant val18 = default(Variant);
		if (info.TryGetProperty(PropertyName._uncommonFilter, ref val18))
		{
			_uncommonFilter = ((Variant)(ref val18)).As<NCardRarityTickbox>();
		}
		Variant val19 = default(Variant);
		if (info.TryGetProperty(PropertyName._rareFilter, ref val19))
		{
			_rareFilter = ((Variant)(ref val19)).As<NCardRarityTickbox>();
		}
		Variant val20 = default(Variant);
		if (info.TryGetProperty(PropertyName._otherFilter, ref val20))
		{
			_otherFilter = ((Variant)(ref val20)).As<NCardRarityTickbox>();
		}
		Variant val21 = default(Variant);
		if (info.TryGetProperty(PropertyName._costSorter, ref val21))
		{
			_costSorter = ((Variant)(ref val21)).As<NCardViewSortButton>();
		}
		Variant val22 = default(Variant);
		if (info.TryGetProperty(PropertyName._zeroFilter, ref val22))
		{
			_zeroFilter = ((Variant)(ref val22)).As<NCardCostTickbox>();
		}
		Variant val23 = default(Variant);
		if (info.TryGetProperty(PropertyName._oneFilter, ref val23))
		{
			_oneFilter = ((Variant)(ref val23)).As<NCardCostTickbox>();
		}
		Variant val24 = default(Variant);
		if (info.TryGetProperty(PropertyName._twoFilter, ref val24))
		{
			_twoFilter = ((Variant)(ref val24)).As<NCardCostTickbox>();
		}
		Variant val25 = default(Variant);
		if (info.TryGetProperty(PropertyName._threePlusFilter, ref val25))
		{
			_threePlusFilter = ((Variant)(ref val25)).As<NCardCostTickbox>();
		}
		Variant val26 = default(Variant);
		if (info.TryGetProperty(PropertyName._xFilter, ref val26))
		{
			_xFilter = ((Variant)(ref val26)).As<NCardCostTickbox>();
		}
		Variant val27 = default(Variant);
		if (info.TryGetProperty(PropertyName._alphabetSorter, ref val27))
		{
			_alphabetSorter = ((Variant)(ref val27)).As<NCardViewSortButton>();
		}
		Variant val28 = default(Variant);
		if (info.TryGetProperty(PropertyName._viewMultiplayerCards, ref val28))
		{
			_viewMultiplayerCards = ((Variant)(ref val28)).As<NLibraryStatTickbox>();
		}
		Variant val29 = default(Variant);
		if (info.TryGetProperty(PropertyName._viewStats, ref val29))
		{
			_viewStats = ((Variant)(ref val29)).As<NLibraryStatTickbox>();
		}
		Variant val30 = default(Variant);
		if (info.TryGetProperty(PropertyName._viewUpgrades, ref val30))
		{
			_viewUpgrades = ((Variant)(ref val30)).As<NLibraryStatTickbox>();
		}
		Variant val31 = default(Variant);
		if (info.TryGetProperty(PropertyName._cardCountLabel, ref val31))
		{
			_cardCountLabel = ((Variant)(ref val31)).As<MegaRichTextLabel>();
		}
		Variant val32 = default(Variant);
		if (info.TryGetProperty(PropertyName._noResultsLabel, ref val32))
		{
			_noResultsLabel = ((Variant)(ref val32)).As<MegaRichTextLabel>();
		}
		Variant val33 = default(Variant);
		if (info.TryGetProperty(PropertyName._lastHoveredControl, ref val33))
		{
			_lastHoveredControl = ((Variant)(ref val33)).As<Control>();
		}
	}

	public NCardLibrary()
	{
		int num = 4;
		List<SortingOrders> list = new List<SortingOrders>(num);
		CollectionsMarshal.SetCount(list, num);
		Span<SortingOrders> span = CollectionsMarshal.AsSpan(list);
		int num2 = 0;
		span[num2] = SortingOrders.RarityAscending;
		num2++;
		span[num2] = SortingOrders.TypeAscending;
		num2++;
		span[num2] = SortingOrders.CostAscending;
		span[num2 + 1] = SortingOrders.AlphabetAscending;
		_sortingPriority = list;
		_filter = (CardModel _) => true;
		base._002Ector();
	}
}
