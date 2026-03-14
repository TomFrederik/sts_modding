using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.UI;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards.Holders;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Screens.CardLibrary;

namespace MegaCrit.Sts2.Core.Nodes.Cards;

[ScriptPath("res://src/Core/Nodes/Cards/NCardGrid.cs")]
public class NCardGrid : Control
{
	[Signal]
	public delegate void HolderPressedEventHandler(NCardHolder card);

	[Signal]
	public delegate void HolderAltPressedEventHandler(NCardHolder card);

	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName ConnectSignals = StringName.op_Implicit("ConnectSignals");

		public static readonly StringName _EnterTree = StringName.op_Implicit("_EnterTree");

		public static readonly StringName _ExitTree = StringName.op_Implicit("_ExitTree");

		public static readonly StringName UpdateScrollLimitBottom = StringName.op_Implicit("UpdateScrollLimitBottom");

		public static readonly StringName _GuiInput = StringName.op_Implicit("_GuiInput");

		public static readonly StringName _Process = StringName.op_Implicit("_Process");

		public static readonly StringName _Notification = StringName.op_Implicit("_Notification");

		public static readonly StringName SetScrollPosition = StringName.op_Implicit("SetScrollPosition");

		public static readonly StringName SetCanScroll = StringName.op_Implicit("SetCanScroll");

		public static readonly StringName InsetForTopBar = StringName.op_Implicit("InsetForTopBar");

		public static readonly StringName ProcessMouseEvent = StringName.op_Implicit("ProcessMouseEvent");

		public static readonly StringName ProcessScrollEvent = StringName.op_Implicit("ProcessScrollEvent");

		public static readonly StringName ProcessGuiFocus = StringName.op_Implicit("ProcessGuiFocus");

		public static readonly StringName UpdateScrollPosition = StringName.op_Implicit("UpdateScrollPosition");

		public static readonly StringName ClearGrid = StringName.op_Implicit("ClearGrid");

		public static readonly StringName CalculateRowsNeeded = StringName.op_Implicit("CalculateRowsNeeded");

		public static readonly StringName InitGrid = StringName.op_Implicit("InitGrid");

		public static readonly StringName GetContainedCardsSize = StringName.op_Implicit("GetContainedCardsSize");

		public static readonly StringName ReflowColumns = StringName.op_Implicit("ReflowColumns");

		public static readonly StringName UpdateGridPositions = StringName.op_Implicit("UpdateGridPositions");

		public static readonly StringName OnHolderPressed = StringName.op_Implicit("OnHolderPressed");

		public static readonly StringName OnHolderAltPressed = StringName.op_Implicit("OnHolderAltPressed");

		public static readonly StringName GetTotalRowCount = StringName.op_Implicit("GetTotalRowCount");

		public static readonly StringName AllocateCardHolders = StringName.op_Implicit("AllocateCardHolders");

		public static readonly StringName ReallocateAll = StringName.op_Implicit("ReallocateAll");

		public static readonly StringName UpdateGridNavigation = StringName.op_Implicit("UpdateGridNavigation");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName CanScroll = StringName.op_Implicit("CanScroll");

		public static readonly StringName DisplayedRows = StringName.op_Implicit("DisplayedRows");

		public static readonly StringName Columns = StringName.op_Implicit("Columns");

		public static readonly StringName CardPadding = StringName.op_Implicit("CardPadding");

		public static readonly StringName IsCardLibrary = StringName.op_Implicit("IsCardLibrary");

		public static readonly StringName ScrollLimitBottom = StringName.op_Implicit("ScrollLimitBottom");

		public static readonly StringName ScrollLimitTop = StringName.op_Implicit("ScrollLimitTop");

		public static readonly StringName IsAnimatingOut = StringName.op_Implicit("IsAnimatingOut");

		public static readonly StringName IsShowingUpgrades = StringName.op_Implicit("IsShowingUpgrades");

		public static readonly StringName YOffset = StringName.op_Implicit("YOffset");

		public static readonly StringName CenterGrid = StringName.op_Implicit("CenterGrid");

		public static readonly StringName DefaultFocusedControl = StringName.op_Implicit("DefaultFocusedControl");

		public static readonly StringName FocusedControlFromTopBar = StringName.op_Implicit("FocusedControlFromTopBar");

		public static readonly StringName _startDrag = StringName.op_Implicit("_startDrag");

		public static readonly StringName _targetDrag = StringName.op_Implicit("_targetDrag");

		public static readonly StringName _isDragging = StringName.op_Implicit("_isDragging");

		public static readonly StringName _scrollingEnabled = StringName.op_Implicit("_scrollingEnabled");

		public static readonly StringName _scrollContainer = StringName.op_Implicit("_scrollContainer");

		public static readonly StringName _scrollbarPressed = StringName.op_Implicit("_scrollbarPressed");

		public static readonly StringName _scrollbar = StringName.op_Implicit("_scrollbar");

		public static readonly StringName _slidingWindowCardIndex = StringName.op_Implicit("_slidingWindowCardIndex");

		public static readonly StringName _pileType = StringName.op_Implicit("_pileType");

		public static readonly StringName _cardSize = StringName.op_Implicit("_cardSize");

		public static readonly StringName _cardsAnimatingOutForSetCards = StringName.op_Implicit("_cardsAnimatingOutForSetCards");

		public static readonly StringName _isShowingUpgrades = StringName.op_Implicit("_isShowingUpgrades");

		public static readonly StringName _lastFocusedHolder = StringName.op_Implicit("_lastFocusedHolder");

		public static readonly StringName _needsReinit = StringName.op_Implicit("_needsReinit");
	}

	public class SignalName : SignalName
	{
		public static readonly StringName HolderPressed = StringName.op_Implicit("HolderPressed");

		public static readonly StringName HolderAltPressed = StringName.op_Implicit("HolderAltPressed");
	}

	private Dictionary<SortingOrders, Func<CardModel, CardModel, int>>? _sortingAlgorithms;

	private float _startDrag;

	private float _targetDrag;

	private bool _isDragging;

	private bool _scrollingEnabled = true;

	private const float _topMargin = 80f;

	private const float _bottomMargin = 320f;

	protected Control _scrollContainer;

	private bool _scrollbarPressed;

	private NScrollbar _scrollbar;

	private int _slidingWindowCardIndex;

	private PileType _pileType;

	protected Vector2 _cardSize;

	protected readonly List<CardModel> _cards = new List<CardModel>();

	protected readonly List<List<NGridCardHolder>> _cardRows = new List<List<NGridCardHolder>>();

	private readonly List<CardModel> _highlightedCards = new List<CardModel>();

	private Task? _animatingOutTask;

	private bool _cardsAnimatingOutForSetCards;

	private CancellationTokenSource? _setCardsCancellation;

	private bool _isShowingUpgrades;

	private NCardHolder? _lastFocusedHolder;

	private readonly List<CardModel> _cardsCache = new List<CardModel>();

	private readonly List<CardModel> _sortedCardsCache = new List<CardModel>();

	private bool _needsReinit;

	private HolderPressedEventHandler backing_HolderPressed;

	private HolderAltPressedEventHandler backing_HolderAltPressed;

	private Dictionary<SortingOrders, Func<CardModel, CardModel, int>> SortingAlgorithms
	{
		get
		{
			Dictionary<SortingOrders, Func<CardModel, CardModel, int>> dictionary = _sortingAlgorithms;
			if (dictionary == null)
			{
				Dictionary<SortingOrders, Func<CardModel, CardModel, int>> obj = new Dictionary<SortingOrders, Func<CardModel, CardModel, int>>
				{
					{
						SortingOrders.RarityAscending,
						(CardModel a, CardModel b) => GetCardRarityComparisonValue(a).CompareTo(GetCardRarityComparisonValue(b))
					},
					{
						SortingOrders.CostAscending,
						(CardModel a, CardModel b) => a.EnergyCost.Canonical.CompareTo(b.EnergyCost.Canonical)
					},
					{
						SortingOrders.TypeAscending,
						(CardModel a, CardModel b) => a.Type.CompareTo(b.Type)
					},
					{
						SortingOrders.AlphabetAscending,
						(CardModel a, CardModel b) => string.Compare(a.Title, b.Title, LocManager.Instance.CultureInfo, CompareOptions.None)
					},
					{
						SortingOrders.RarityDescending,
						(CardModel a, CardModel b) => -GetCardRarityComparisonValue(a).CompareTo(GetCardRarityComparisonValue(b))
					},
					{
						SortingOrders.CostDescending,
						(CardModel a, CardModel b) => -a.EnergyCost.Canonical.CompareTo(b.EnergyCost.Canonical)
					},
					{
						SortingOrders.TypeDescending,
						(CardModel a, CardModel b) => -a.Type.CompareTo(b.Type)
					},
					{
						SortingOrders.AlphabetDescending,
						(CardModel a, CardModel b) => -string.Compare(a.Title, b.Title, LocManager.Instance.CultureInfo, CompareOptions.None)
					},
					{
						SortingOrders.Ascending,
						(CardModel a, CardModel b) => _cards.IndexOf(a).CompareTo(_cards.IndexOf(b))
					},
					{
						SortingOrders.Descending,
						(CardModel a, CardModel b) => -_cards.IndexOf(a).CompareTo(_cards.IndexOf(b))
					}
				};
				Dictionary<SortingOrders, Func<CardModel, CardModel, int>> dictionary2 = obj;
				_sortingAlgorithms = obj;
				dictionary = dictionary2;
			}
			return dictionary;
		}
	}

	private bool CanScroll
	{
		get
		{
			if (_scrollingEnabled)
			{
				return ((CanvasItem)this).Visible;
			}
			return false;
		}
	}

	private int DisplayedRows { get; set; }

	protected int Columns => (int)((_scrollContainer.Size.X + CardPadding) / (_cardSize.X + CardPadding));

	protected float CardPadding => 40f;

	protected virtual bool IsCardLibrary => false;

	private float ScrollLimitBottom
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			if (!(((Control)this).Size.Y > _scrollContainer.Size.Y))
			{
				return ((Control)this).Size.Y - _scrollContainer.Size.Y;
			}
			return (((Control)this).Size.Y - _scrollContainer.Size.Y) * 0.5f;
		}
	}

	protected float ScrollLimitTop
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			if (!(((Control)this).Size.Y > _scrollContainer.Size.Y) || !CenterGrid)
			{
				return 0f;
			}
			return (((Control)this).Size.Y - _scrollContainer.Size.Y) * 0.5f;
		}
	}

	public IEnumerable<NGridCardHolder> CurrentlyDisplayedCardHolders => _cardRows.SelectMany((List<NGridCardHolder> r) => r);

	public IEnumerable<CardModel> CurrentlyDisplayedCards => CurrentlyDisplayedCardHolders.Select((NGridCardHolder h) => h.CardModel);

	public bool IsAnimatingOut
	{
		get
		{
			Task animatingOutTask = _animatingOutTask;
			if (animatingOutTask != null)
			{
				return !animatingOutTask.IsCompleted;
			}
			return false;
		}
	}

	public bool IsShowingUpgrades
	{
		get
		{
			return _isShowingUpgrades;
		}
		set
		{
			_isShowingUpgrades = value;
			foreach (List<NGridCardHolder> cardRow in _cardRows)
			{
				foreach (NGridCardHolder item in cardRow)
				{
					if ((_isShowingUpgrades || item.CardModel.CanonicalInstance.IsUpgradable) && (!_isShowingUpgrades || item.CardModel.IsUpgradable))
					{
						item.SetIsPreviewingUpgrade(_isShowingUpgrades);
					}
				}
			}
		}
	}

	public int YOffset { get; set; }

	protected virtual bool CenterGrid => true;

	public Control? DefaultFocusedControl
	{
		get
		{
			if (_lastFocusedHolder != null)
			{
				return (Control?)(object)_lastFocusedHolder;
			}
			if (_cards.Count == 0)
			{
				return null;
			}
			return (Control?)(object)_cardRows[0][0];
		}
	}

	public Control? FocusedControlFromTopBar
	{
		get
		{
			if (_cards.Count != 0)
			{
				return (Control?)(object)_cardRows[0][0];
			}
			return null;
		}
	}

	public event HolderPressedEventHandler HolderPressed
	{
		add
		{
			backing_HolderPressed = (HolderPressedEventHandler)Delegate.Combine(backing_HolderPressed, value);
		}
		remove
		{
			backing_HolderPressed = (HolderPressedEventHandler)Delegate.Remove(backing_HolderPressed, value);
		}
	}

	public event HolderAltPressedEventHandler HolderAltPressed
	{
		add
		{
			backing_HolderAltPressed = (HolderAltPressedEventHandler)Delegate.Combine(backing_HolderAltPressed, value);
		}
		remove
		{
			backing_HolderAltPressed = (HolderAltPressedEventHandler)Delegate.Remove(backing_HolderAltPressed, value);
		}
	}

	private int CompareCardVisibility(CardModel a, CardModel b)
	{
		bool flag = GetCardVisibility(a) == ModelVisibility.Locked;
		bool value = GetCardVisibility(b) == ModelVisibility.Locked;
		return flag.CompareTo(value);
	}

	private int GetCardRarityComparisonValue(CardModel a)
	{
		if (a.Rarity <= CardRarity.Ancient)
		{
			return (int)a.Rarity;
		}
		return a.Rarity switch
		{
			CardRarity.Status => 6, 
			CardRarity.Curse => 7, 
			CardRarity.Event => 8, 
			CardRarity.Quest => 9, 
			CardRarity.Token => 10, 
			_ => throw new ArgumentOutOfRangeException("a", a, null), 
		};
	}

	public override void _Ready()
	{
		if (((object)this).GetType() != typeof(NCardGrid))
		{
			Log.Error($"{((object)this).GetType()}");
			throw new InvalidOperationException("Don't call base._Ready()! Call ConnectSignals() instead.");
		}
		ConnectSignals();
	}

	protected virtual void ConnectSignals()
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		_scrollContainer = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%ScrollContainer"));
		_scrollbar = ((Node)this).GetNode<NScrollbar>(NodePath.op_Implicit("Scrollbar"));
		_cardSize = NCard.defaultSize * NCardHolder.smallScale;
		((GodotObject)_scrollContainer).Connect(SignalName.ItemRectChanged, Callable.From((Action)UpdateScrollLimitBottom), 0u);
		((CanvasItem)_scrollbar).Visible = false;
		((GodotObject)_scrollbar).Connect(NScrollbar.SignalName.MousePressed, Callable.From<InputEvent>((Action<InputEvent>)delegate
		{
			_scrollbarPressed = true;
		}), 0u);
		((GodotObject)_scrollbar).Connect(NScrollbar.SignalName.MouseReleased, Callable.From<InputEvent>((Action<InputEvent>)delegate
		{
			_scrollbarPressed = false;
		}), 0u);
	}

	public override void _EnterTree()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		((Node)this)._EnterTree();
		((GodotObject)((Node)this).GetViewport()).Connect(SignalName.GuiFocusChanged, Callable.From<Control>((Action<Control>)ProcessGuiFocus), 0u);
	}

	public override void _ExitTree()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		((Node)this)._ExitTree();
		((GodotObject)((Node)this).GetViewport()).Disconnect(SignalName.GuiFocusChanged, Callable.From<Control>((Action<Control>)ProcessGuiFocus));
		foreach (List<NGridCardHolder> cardRow in _cardRows)
		{
			foreach (NGridCardHolder item in cardRow)
			{
				((Node)(object)item).QueueFreeSafely();
			}
		}
		_cardRows.Clear();
	}

	private void UpdateScrollLimitBottom()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		float num = ((Control)this).Size.Y + 320f;
		((CanvasItem)_scrollbar).Visible = _scrollContainer.Size.Y > num && CanScroll;
		((Control)_scrollbar).MouseFilter = (MouseFilterEnum)((_scrollContainer.Size.Y > num && CanScroll) ? 0 : 2);
	}

	public override void _GuiInput(InputEvent inputEvent)
	{
		if (((CanvasItem)this).IsVisibleInTree())
		{
			ProcessMouseEvent(inputEvent);
			ProcessScrollEvent(inputEvent);
		}
	}

	public override void _Process(double delta)
	{
		if (((CanvasItem)this).IsVisibleInTree() && CanScroll)
		{
			UpdateScrollPosition(delta);
			if (_needsReinit)
			{
				InitGrid();
			}
		}
	}

	public override void _Notification(int what)
	{
		((GodotObject)this)._Notification(what);
		if ((long)what == 40 && ((Node)this).IsNodeReady())
		{
			_needsReinit = true;
		}
	}

	public void SetScrollPosition(float scrollY)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		_targetDrag = scrollY;
		_scrollContainer.Position = new Vector2(_scrollContainer.Position.X, scrollY);
	}

	public void SetCanScroll(bool canScroll)
	{
		_scrollingEnabled = canScroll;
		if (!CanScroll)
		{
			_isDragging = false;
		}
	}

	public void InsetForTopBar()
	{
		((Control)this).SetAnchorAndOffset((Side)1, 0f, 80f, false);
	}

	private void ProcessMouseEvent(InputEvent inputEvent)
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Invalid comparison between Unknown and I8
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		if (_isDragging)
		{
			InputEventMouseMotion val = (InputEventMouseMotion)(object)((inputEvent is InputEventMouseMotion) ? inputEvent : null);
			if (val != null)
			{
				_targetDrag += val.Relative.Y;
				return;
			}
		}
		InputEventMouseButton val2 = (InputEventMouseButton)(object)((inputEvent is InputEventMouseButton) ? inputEvent : null);
		if (val2 == null)
		{
			return;
		}
		if ((long)val2.ButtonIndex == 1)
		{
			if (val2.Pressed)
			{
				_isDragging = true;
				_startDrag = _scrollContainer.Position.Y;
				_targetDrag = _startDrag;
			}
			else
			{
				_isDragging = false;
			}
		}
		else if (!val2.Pressed)
		{
			_isDragging = false;
		}
	}

	private void ProcessScrollEvent(InputEvent inputEvent)
	{
		_targetDrag += ScrollHelper.GetDragForScrollEvent(inputEvent);
	}

	private void ProcessGuiFocus(Control focusedControl)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		if (((CanvasItem)this).IsVisibleInTree() && CanScroll && NControllerManager.Instance.IsUsingController && (object)((Node)focusedControl).GetParent() == _scrollContainer)
		{
			float value = 0f - focusedControl.Position.Y + ((Control)this).Size.Y * 0.5f;
			float min = Math.Min(Math.Min(ScrollLimitTop, ScrollLimitBottom), 0f);
			float max = Math.Max(Math.Min(ScrollLimitTop, ScrollLimitBottom), 0f);
			value = Math.Clamp(value, min, max);
			_targetDrag = value;
		}
	}

	private void UpdateScrollPosition(double delta)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		float num = _scrollContainer.Position.Y;
		if (Math.Abs(num - _targetDrag) > 0.1f)
		{
			num = Mathf.Lerp(num, _targetDrag, Mathf.Clamp((float)delta * 15f, 0f, 1f));
			if (Mathf.Abs(num - _targetDrag) < 0.5f)
			{
				num = _targetDrag;
			}
			AllocateCardHolders();
			if (!_scrollbarPressed && CanScroll)
			{
				_scrollbar.SetValueWithoutAnimation(Mathf.Clamp(_scrollContainer.Position.Y / ScrollLimitBottom, 0f, 1f) * 100f);
			}
		}
		if (_scrollbarPressed)
		{
			_targetDrag = Mathf.Lerp(0f, ScrollLimitBottom, (float)((Range)_scrollbar).Value / 100f);
			AllocateCardHolders();
		}
		if (!_isDragging)
		{
			if (_targetDrag < Mathf.Min(ScrollLimitBottom, ScrollLimitTop))
			{
				_targetDrag = Mathf.Lerp(_targetDrag, Mathf.Min(ScrollLimitBottom, ScrollLimitTop), (float)delta * 12f);
			}
			else if (_targetDrag > Mathf.Max(ScrollLimitTop, ScrollLimitBottom))
			{
				_targetDrag = Mathf.Lerp(_targetDrag, Mathf.Max(ScrollLimitTop, ScrollLimitBottom), (float)delta * 12f);
			}
		}
		_scrollContainer.Position = new Vector2(_scrollContainer.Position.X, num);
	}

	public void ClearGrid()
	{
		_cardsCache.Clear();
		_cards.Clear();
		TaskHelper.RunSafely(InitGrid(null));
	}

	public void SetCards(IReadOnlyList<CardModel> cardsToDisplay, PileType pileType, List<SortingOrders> sortingPriority, Task? taskToWaitOn = null)
	{
		_cardsCache.Clear();
		_cardsCache.AddRange(cardsToDisplay);
		if (sortingPriority[0] == SortingOrders.Descending)
		{
			_cardsCache.Reverse();
		}
		else if (sortingPriority[0] != SortingOrders.Ascending)
		{
			_cardsCache.Sort(delegate(CardModel x, CardModel y)
			{
				foreach (SortingOrders item in sortingPriority)
				{
					int num = SortingAlgorithms[item](x, y);
					if (num != 0)
					{
						return num;
					}
				}
				return x.Id.CompareTo(y.Id);
			});
		}
		if (IsCardLibrary)
		{
			_sortedCardsCache.Clear();
			_sortedCardsCache.AddRange(_cardsCache.OrderBy((CardModel c) => c, Comparer<CardModel>.Create(CompareCardVisibility)));
			_cardsCache.Clear();
			_cardsCache.AddRange(_sortedCardsCache);
		}
		_cards.Clear();
		_cards.AddRange(_cardsCache);
		_pileType = pileType;
		if (!_cardsAnimatingOutForSetCards)
		{
			TaskHelper.RunSafely(InitGrid(taskToWaitOn));
		}
	}

	public Task AnimateOut()
	{
		_animatingOutTask = AnimateOutInternal();
		return _animatingOutTask;
	}

	private async Task AnimateOutInternal()
	{
		if (!IsCardLibrary)
		{
			return;
		}
		List<NGridCardHolder> list = _cardRows.SelectMany((List<NGridCardHolder> c) => c).ToList();
		if (list.Count <= 0)
		{
			return;
		}
		Tween val = ((Node)this).CreateTween().SetParallel(true);
		foreach (NGridCardHolder item in list)
		{
			val.TweenProperty((GodotObject)(object)item, NodePath.op_Implicit("position:y"), Variant.op_Implicit(((Control)item).Position.Y + 40f), 0.2).SetEase((EaseType)1).SetTrans((TransitionType)5);
			val.TweenProperty((GodotObject)(object)item, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0f), 0.2);
		}
		await ((GodotObject)this).ToSignal((GodotObject)(object)val, SignalName.Finished);
	}

	private async Task AnimateIn()
	{
		if (!IsCardLibrary)
		{
			return;
		}
		List<NGridCardHolder> list = _cardRows.SelectMany((List<NGridCardHolder> c) => c).ToList();
		if (list.Count <= 0)
		{
			return;
		}
		Tween tween = ((Node)this).CreateTween().SetParallel(true);
		for (int num = 0; num < list.Count; num++)
		{
			NGridCardHolder nGridCardHolder = list[num];
			float num2 = (float)num / (float)list.Count * 0.2f;
			float y = ((Control)nGridCardHolder).Position.Y;
			Vector2 position = ((Control)nGridCardHolder).Position;
			position.Y = ((Control)nGridCardHolder).Position.Y + 40f;
			((Control)nGridCardHolder).Position = position;
			Color modulate = ((CanvasItem)nGridCardHolder).Modulate;
			modulate.A = 0f;
			((CanvasItem)nGridCardHolder).Modulate = modulate;
			tween.TweenProperty((GodotObject)(object)nGridCardHolder, NodePath.op_Implicit("position:y"), Variant.op_Implicit(y), 0.4).SetEase((EaseType)1).SetTrans((TransitionType)10)
				.SetDelay((double)num2);
			tween.TweenProperty((GodotObject)(object)nGridCardHolder, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(1f), 0.4).SetEase((EaseType)1).SetTrans((TransitionType)5)
				.SetDelay((double)num2);
		}
		_setCardsCancellation = new CancellationTokenSource();
		while (tween.IsRunning())
		{
			if (_setCardsCancellation.IsCancellationRequested)
			{
				tween.Kill();
			}
			await ((GodotObject)this).ToSignal((GodotObject)(object)((Node)this).GetTree(), SignalName.ProcessFrame);
		}
	}

	private async Task InitGrid(Task? taskToWaitOn)
	{
		if (_setCardsCancellation != null)
		{
			await _setCardsCancellation.CancelAsync();
		}
		_cardsAnimatingOutForSetCards = true;
		Task animatingOutTask = _animatingOutTask;
		if (animatingOutTask == null || animatingOutTask.IsCompleted)
		{
			await AnimateOut();
		}
		else
		{
			await _animatingOutTask;
		}
		if (taskToWaitOn != null)
		{
			await taskToWaitOn;
		}
		_cardsAnimatingOutForSetCards = false;
		InitGrid();
		SetScrollPosition(ScrollLimitTop);
		await AnimateIn();
	}

	private int CalculateRowsNeeded()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		int num = Mathf.CeilToInt((((Control)this).Size.Y + CardPadding) / (_cardSize.Y + CardPadding)) + 2;
		return Mathf.Min(num, GetTotalRowCount());
	}

	protected virtual void InitGrid()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		_scrollContainer.Position = new Vector2(_scrollContainer.Position.X, ScrollLimitTop);
		_slidingWindowCardIndex = 0;
		((Range)_scrollbar).Value = 0.0;
		DisplayedRows = CalculateRowsNeeded();
		foreach (List<NGridCardHolder> cardRow in _cardRows)
		{
			foreach (NGridCardHolder item in cardRow)
			{
				((Node)item).Name = StringName.op_Implicit(StringName.op_Implicit(((Node)item).Name) + "-OLD");
				((Node)(object)item).QueueFreeSafely();
			}
		}
		_cardRows.Clear();
		if (_cards.Count != 0)
		{
			int num = 0;
			for (int i = 0; i < DisplayedRows; i++)
			{
				List<NGridCardHolder> list = new List<NGridCardHolder>();
				for (int j = 0; j < Columns; j++)
				{
					if (num >= _cards.Count)
					{
						break;
					}
					CardModel card = _cards[num];
					NCard nCard = NCard.Create(card, GetCardVisibility(card));
					NGridCardHolder nGridCardHolder = NGridCardHolder.Create(nCard);
					list.Add(nGridCardHolder);
					((GodotObject)nGridCardHolder).Connect(NCardHolder.SignalName.Pressed, Callable.From<NCardHolder>((Action<NCardHolder>)OnHolderPressed), 0u);
					((GodotObject)nGridCardHolder).Connect(NCardHolder.SignalName.AltPressed, Callable.From<NCardHolder>((Action<NCardHolder>)OnHolderAltPressed), 0u);
					((CanvasItem)nGridCardHolder).Visible = true;
					((Control)nGridCardHolder).MouseFilter = (MouseFilterEnum)1;
					((Control)nGridCardHolder).Scale = nGridCardHolder.SmallScale;
					((Node)(object)_scrollContainer).AddChildSafely((Node?)(object)nGridCardHolder);
					nCard.UpdateVisuals(_pileType, CardPreviewMode.Normal);
					if (nGridCardHolder.CardModel.CanonicalInstance.IsUpgradable)
					{
						nGridCardHolder.SetIsPreviewingUpgrade(IsShowingUpgrades);
					}
					num++;
				}
				_cardRows.Add(list);
			}
		}
		((GodotObject)_scrollContainer).SetDeferred(PropertyName.Size, Variant.op_Implicit(new Vector2(_scrollContainer.Size.X, GetContainedCardsSize().Y + 80f + 320f + (float)YOffset)));
		UpdateGridPositions(0);
		UpdateGridNavigation();
		_needsReinit = false;
	}

	private Vector2 GetContainedCardsSize()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		int totalRowCount = GetTotalRowCount();
		return new Vector2((float)Columns, (float)totalRowCount) * _cardSize + new Vector2((float)(Columns - 1), (float)(totalRowCount - 1)) * CardPadding;
	}

	private void ReflowColumns()
	{
		if (_cards.Count != 0)
		{
			InitGrid();
		}
	}

	private void UpdateGridPositions(int index)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		Vector2 val = new Vector2((_scrollContainer.Size.X - GetContainedCardsSize().X) * 0.5f, (float)YOffset + 80f) + _cardSize * 0.5f;
		foreach (List<NGridCardHolder> cardRow in _cardRows)
		{
			foreach (NGridCardHolder item in cardRow)
			{
				int num = index / Columns;
				int num2 = index % Columns;
				((Control)item).Position = val + new Vector2((float)num2 * (_cardSize.X + CardPadding), (float)num * (_cardSize.Y + CardPadding));
				index++;
			}
		}
	}

	public NGridCardHolder? GetCardHolder(CardModel model)
	{
		return _cardRows.SelectMany((List<NGridCardHolder> row) => row).FirstOrDefault((NGridCardHolder h) => h.CardModel == model);
	}

	public NCard? GetCardNode(CardModel model)
	{
		return GetCardHolder(model)?.CardNode;
	}

	public IEnumerable<NGridCardHolder>? GetTopRowOfCardNodes()
	{
		return _cardRows.FirstOrDefault();
	}

	private void OnHolderPressed(NCardHolder holder)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		_lastFocusedHolder = holder;
		((GodotObject)this).EmitSignal(SignalName.HolderPressed, (Variant[])(object)new Variant[1] { Variant.op_Implicit((GodotObject)(object)holder) });
	}

	private void OnHolderAltPressed(NCardHolder holder)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		_lastFocusedHolder = holder;
		((GodotObject)this).EmitSignal(SignalName.HolderAltPressed, (Variant[])(object)new Variant[1] { Variant.op_Implicit((GodotObject)(object)holder) });
	}

	private int GetTotalRowCount()
	{
		return Mathf.CeilToInt((float)_cards.Count / (float)Columns);
	}

	private void AllocateCardHolders()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		if (_cardRows.Count != 0)
		{
			Rect2 viewportRect = ((CanvasItem)this).GetViewportRect();
			float y = ((Rect2)(ref viewportRect)).Size.Y;
			float num = y;
			List<NGridCardHolder> list = _cardRows[0];
			float y2 = ((Control)list[0]).GlobalPosition.Y;
			List<List<NGridCardHolder>> cardRows = _cardRows;
			List<NGridCardHolder> list2 = cardRows[cardRows.Count - 1];
			float y3 = ((Control)list2[0]).GlobalPosition.Y;
			if (Mathf.Abs(y2 - 0f) > ((Control)this).Size.Y * 2f)
			{
				ReallocateAll();
			}
			else if (y2 > 0f)
			{
				List<List<NGridCardHolder>> cardRows2 = _cardRows;
				ReallocateAbove(cardRows2[cardRows2.Count - 1]);
			}
			else if (y3 < num)
			{
				ReallocateBelow(_cardRows[0]);
			}
		}
	}

	private void ReallocateAll()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		List<NGridCardHolder> list = _cardRows[0];
		float y = ((Control)list[0]).GlobalPosition.Y;
		float num = y - 0f;
		int num2 = Mathf.RoundToInt(num / (_cardSize.Y + CardPadding));
		int slidingWindowCardIndex = Mathf.Max(0, _slidingWindowCardIndex - Columns * num2);
		_slidingWindowCardIndex = slidingWindowCardIndex;
		int count = _cardRows.Count;
		for (int i = 0; i < count; i++)
		{
			AssignCardsToRow(_cardRows[i], _slidingWindowCardIndex + i * Columns);
		}
		UpdateGridPositions(_slidingWindowCardIndex);
		UpdateGridNavigation();
	}

	private void ReallocateAbove(List<NGridCardHolder> row)
	{
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		int num = _slidingWindowCardIndex - Columns;
		if (num < 0)
		{
			return;
		}
		_slidingWindowCardIndex = num;
		_cardRows.RemoveAt(_cardRows.Count - 1);
		AssignCardsToRow(row, _slidingWindowCardIndex);
		_cardRows.Insert(0, row);
		List<NGridCardHolder> list = _cardRows[1];
		float y = ((Control)list[0]).Position.Y;
		foreach (NGridCardHolder item in row)
		{
			Vector2 position = ((Control)item).Position;
			position.Y = y - _cardSize.Y - CardPadding;
			((Control)item).Position = position;
		}
		UpdateGridNavigation();
	}

	private void ReallocateBelow(List<NGridCardHolder> row)
	{
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		int num = Columns * DisplayedRows;
		int num2 = _slidingWindowCardIndex + num;
		if (num2 >= _cards.Count)
		{
			return;
		}
		_slidingWindowCardIndex += Columns;
		_cardRows.RemoveAt(0);
		AssignCardsToRow(row, num2);
		_cardRows.Add(row);
		List<List<NGridCardHolder>> cardRows = _cardRows;
		List<NGridCardHolder> list = cardRows[cardRows.Count - 2];
		float y = ((Control)list[0]).Position.Y;
		foreach (NGridCardHolder item in row)
		{
			Vector2 position = ((Control)item).Position;
			position.Y = y + _cardSize.Y + CardPadding;
			((Control)item).Position = position;
		}
		UpdateGridNavigation();
	}

	public void HighlightCard(CardModel card)
	{
		_highlightedCards.Add(card);
		GetCardNode(card)?.CardHighlight.AnimShow();
	}

	public void UnhighlightCard(CardModel card)
	{
		_highlightedCards.Remove(card);
		GetCardNode(card)?.CardHighlight.AnimHide();
	}

	protected virtual void AssignCardsToRow(List<NGridCardHolder> row, int startIndex)
	{
		for (int i = 0; i < row.Count; i++)
		{
			NGridCardHolder nGridCardHolder = row[i];
			if (startIndex + i >= _cards.Count)
			{
				((CanvasItem)nGridCardHolder).Visible = false;
				continue;
			}
			CardModel cardModel = _cards[startIndex + i];
			nGridCardHolder.ReassignToCard(cardModel, PileType.None, null, GetCardVisibility(cardModel));
			((CanvasItem)nGridCardHolder).Visible = true;
			if (_highlightedCards.Contains(cardModel))
			{
				nGridCardHolder.CardNode.CardHighlight.AnimShow();
			}
			else
			{
				nGridCardHolder.CardNode.CardHighlight.AnimHide();
			}
			if (_isShowingUpgrades && cardModel.IsUpgradable)
			{
				nGridCardHolder.SetIsPreviewingUpgrade(showUpgradePreview: true);
			}
		}
	}

	protected virtual ModelVisibility GetCardVisibility(CardModel card)
	{
		return ModelVisibility.Visible;
	}

	protected virtual void UpdateGridNavigation()
	{
		for (int i = 0; i < _cardRows.Count; i++)
		{
			for (int j = 0; j < _cardRows[i].Count; j++)
			{
				NCardHolder nCardHolder = _cardRows[i][j];
				((Control)nCardHolder).FocusNeighborLeft = ((j > 0) ? ((Node)_cardRows[i][j - 1]).GetPath() : ((Node)_cardRows[i][_cardRows[i].Count - 1]).GetPath());
				((Control)nCardHolder).FocusNeighborRight = ((j < _cardRows[i].Count - 1) ? ((Node)_cardRows[i][j + 1]).GetPath() : ((Node)_cardRows[i][0]).GetPath());
				((Control)nCardHolder).FocusNeighborTop = ((i > 0) ? ((Node)_cardRows[i - 1][j]).GetPath() : ((Node)_cardRows[i][j]).GetPath());
				((Control)nCardHolder).FocusNeighborBottom = ((i < _cardRows.Count - 1 && j < _cardRows[i + 1].Count) ? ((Node)_cardRows[i + 1][j]).GetPath() : ((Node)_cardRows[i][j]).GetPath());
			}
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Expected O, but got Unknown
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_028d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0298: Unknown result type (might be due to invalid IL or missing references)
		//IL_02be: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0315: Unknown result type (might be due to invalid IL or missing references)
		//IL_0320: Expected O, but got Unknown
		//IL_031b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0326: Unknown result type (might be due to invalid IL or missing references)
		//IL_034c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0374: Unknown result type (might be due to invalid IL or missing references)
		//IL_037f: Expected O, but got Unknown
		//IL_037a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0385: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03de: Expected O, but got Unknown
		//IL_03d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_040a: Unknown result type (might be due to invalid IL or missing references)
		//IL_042d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0438: Unknown result type (might be due to invalid IL or missing references)
		//IL_045e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0467: Unknown result type (might be due to invalid IL or missing references)
		//IL_048d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0496: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_051a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0523: Unknown result type (might be due to invalid IL or missing references)
		//IL_0549: Unknown result type (might be due to invalid IL or missing references)
		//IL_056c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0577: Unknown result type (might be due to invalid IL or missing references)
		//IL_059d: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d0: Expected O, but got Unknown
		//IL_05cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_05fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0624: Unknown result type (might be due to invalid IL or missing references)
		//IL_062f: Expected O, but got Unknown
		//IL_062a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0635: Unknown result type (might be due to invalid IL or missing references)
		//IL_065b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0664: Unknown result type (might be due to invalid IL or missing references)
		//IL_068a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0693: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f1: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(27);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ConnectSignals, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._EnterTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._ExitTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UpdateScrollLimitBottom, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._GuiInput, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("inputEvent"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("InputEvent"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Process, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)3, StringName.op_Implicit("delta"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Notification, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("what"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetScrollPosition, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)3, StringName.op_Implicit("scrollY"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetCanScroll, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)1, StringName.op_Implicit("canScroll"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.InsetForTopBar, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ProcessMouseEvent, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("inputEvent"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("InputEvent"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ProcessScrollEvent, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("inputEvent"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("InputEvent"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ProcessGuiFocus, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("focusedControl"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UpdateScrollPosition, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)3, StringName.op_Implicit("delta"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ClearGrid, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.CalculateRowsNeeded, new PropertyInfo((Type)2, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.InitGrid, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.GetContainedCardsSize, new PropertyInfo((Type)5, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ReflowColumns, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UpdateGridPositions, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("index"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnHolderPressed, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("holder"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnHolderAltPressed, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("holder"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.GetTotalRowCount, new PropertyInfo((Type)2, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AllocateCardHolders, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ReallocateAll, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UpdateGridNavigation, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0317: Unknown result type (might be due to invalid IL or missing references)
		//IL_0336: Unknown result type (might be due to invalid IL or missing references)
		//IL_033b: Unknown result type (might be due to invalid IL or missing references)
		//IL_033f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0344: Unknown result type (might be due to invalid IL or missing references)
		//IL_0368: Unknown result type (might be due to invalid IL or missing references)
		//IL_039b: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_0401: Unknown result type (might be due to invalid IL or missing references)
		//IL_0429: Unknown result type (might be due to invalid IL or missing references)
		//IL_042e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0452: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0477: Unknown result type (might be due to invalid IL or missing references)
		//IL_049c: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.ConnectSignals && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			ConnectSignals();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._EnterTree && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._EnterTree();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._ExitTree && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._ExitTree();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.UpdateScrollLimitBottom && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			UpdateScrollLimitBottom();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._GuiInput && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			((Control)this)._GuiInput(VariantUtils.ConvertTo<InputEvent>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._Process && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			((Node)this)._Process(VariantUtils.ConvertTo<double>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._Notification && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			((GodotObject)this)._Notification(VariantUtils.ConvertTo<int>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetScrollPosition && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			SetScrollPosition(VariantUtils.ConvertTo<float>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetCanScroll && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			SetCanScroll(VariantUtils.ConvertTo<bool>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.InsetForTopBar && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			InsetForTopBar();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.ProcessMouseEvent && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			ProcessMouseEvent(VariantUtils.ConvertTo<InputEvent>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.ProcessScrollEvent && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			ProcessScrollEvent(VariantUtils.ConvertTo<InputEvent>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.ProcessGuiFocus && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			ProcessGuiFocus(VariantUtils.ConvertTo<Control>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.UpdateScrollPosition && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			UpdateScrollPosition(VariantUtils.ConvertTo<double>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.ClearGrid && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			ClearGrid();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.CalculateRowsNeeded && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			int num = CalculateRowsNeeded();
			ret = VariantUtils.CreateFrom<int>(ref num);
			return true;
		}
		if ((ref method) == MethodName.InitGrid && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			InitGrid();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.GetContainedCardsSize && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			Vector2 containedCardsSize = GetContainedCardsSize();
			ret = VariantUtils.CreateFrom<Vector2>(ref containedCardsSize);
			return true;
		}
		if ((ref method) == MethodName.ReflowColumns && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			ReflowColumns();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.UpdateGridPositions && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			UpdateGridPositions(VariantUtils.ConvertTo<int>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnHolderPressed && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OnHolderPressed(VariantUtils.ConvertTo<NCardHolder>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnHolderAltPressed && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OnHolderAltPressed(VariantUtils.ConvertTo<NCardHolder>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.GetTotalRowCount && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			int totalRowCount = GetTotalRowCount();
			ret = VariantUtils.CreateFrom<int>(ref totalRowCount);
			return true;
		}
		if ((ref method) == MethodName.AllocateCardHolders && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			AllocateCardHolders();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.ReallocateAll && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			ReallocateAll();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.UpdateGridNavigation && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			UpdateGridNavigation();
			ret = default(godot_variant);
			return true;
		}
		return ((Control)this).InvokeGodotClassMethod(ref method, args, ref ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if ((ref method) == MethodName._Ready)
		{
			return true;
		}
		if ((ref method) == MethodName.ConnectSignals)
		{
			return true;
		}
		if ((ref method) == MethodName._EnterTree)
		{
			return true;
		}
		if ((ref method) == MethodName._ExitTree)
		{
			return true;
		}
		if ((ref method) == MethodName.UpdateScrollLimitBottom)
		{
			return true;
		}
		if ((ref method) == MethodName._GuiInput)
		{
			return true;
		}
		if ((ref method) == MethodName._Process)
		{
			return true;
		}
		if ((ref method) == MethodName._Notification)
		{
			return true;
		}
		if ((ref method) == MethodName.SetScrollPosition)
		{
			return true;
		}
		if ((ref method) == MethodName.SetCanScroll)
		{
			return true;
		}
		if ((ref method) == MethodName.InsetForTopBar)
		{
			return true;
		}
		if ((ref method) == MethodName.ProcessMouseEvent)
		{
			return true;
		}
		if ((ref method) == MethodName.ProcessScrollEvent)
		{
			return true;
		}
		if ((ref method) == MethodName.ProcessGuiFocus)
		{
			return true;
		}
		if ((ref method) == MethodName.UpdateScrollPosition)
		{
			return true;
		}
		if ((ref method) == MethodName.ClearGrid)
		{
			return true;
		}
		if ((ref method) == MethodName.CalculateRowsNeeded)
		{
			return true;
		}
		if ((ref method) == MethodName.InitGrid)
		{
			return true;
		}
		if ((ref method) == MethodName.GetContainedCardsSize)
		{
			return true;
		}
		if ((ref method) == MethodName.ReflowColumns)
		{
			return true;
		}
		if ((ref method) == MethodName.UpdateGridPositions)
		{
			return true;
		}
		if ((ref method) == MethodName.OnHolderPressed)
		{
			return true;
		}
		if ((ref method) == MethodName.OnHolderAltPressed)
		{
			return true;
		}
		if ((ref method) == MethodName.GetTotalRowCount)
		{
			return true;
		}
		if ((ref method) == MethodName.AllocateCardHolders)
		{
			return true;
		}
		if ((ref method) == MethodName.ReallocateAll)
		{
			return true;
		}
		if ((ref method) == MethodName.UpdateGridNavigation)
		{
			return true;
		}
		return ((Control)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName.DisplayedRows)
		{
			DisplayedRows = VariantUtils.ConvertTo<int>(ref value);
			return true;
		}
		if ((ref name) == PropertyName.IsShowingUpgrades)
		{
			IsShowingUpgrades = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName.YOffset)
		{
			YOffset = VariantUtils.ConvertTo<int>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._startDrag)
		{
			_startDrag = VariantUtils.ConvertTo<float>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._targetDrag)
		{
			_targetDrag = VariantUtils.ConvertTo<float>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._isDragging)
		{
			_isDragging = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._scrollingEnabled)
		{
			_scrollingEnabled = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._scrollContainer)
		{
			_scrollContainer = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._scrollbarPressed)
		{
			_scrollbarPressed = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._scrollbar)
		{
			_scrollbar = VariantUtils.ConvertTo<NScrollbar>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._slidingWindowCardIndex)
		{
			_slidingWindowCardIndex = VariantUtils.ConvertTo<int>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._pileType)
		{
			_pileType = VariantUtils.ConvertTo<PileType>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._cardSize)
		{
			_cardSize = VariantUtils.ConvertTo<Vector2>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._cardsAnimatingOutForSetCards)
		{
			_cardsAnimatingOutForSetCards = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._isShowingUpgrades)
		{
			_isShowingUpgrades = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._lastFocusedHolder)
		{
			_lastFocusedHolder = VariantUtils.ConvertTo<NCardHolder>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._needsReinit)
		{
			_needsReinit = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		return ((GodotObject)this).SetGodotClassPropertyValue(ref name, ref value);
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
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0280: Unknown result type (might be due to invalid IL or missing references)
		//IL_029b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02db: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0300: Unknown result type (might be due to invalid IL or missing references)
		//IL_031b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0320: Unknown result type (might be due to invalid IL or missing references)
		//IL_033b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0340: Unknown result type (might be due to invalid IL or missing references)
		//IL_035b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0360: Unknown result type (might be due to invalid IL or missing references)
		//IL_037b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0380: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName.CanScroll)
		{
			bool canScroll = CanScroll;
			value = VariantUtils.CreateFrom<bool>(ref canScroll);
			return true;
		}
		if ((ref name) == PropertyName.DisplayedRows)
		{
			int displayedRows = DisplayedRows;
			value = VariantUtils.CreateFrom<int>(ref displayedRows);
			return true;
		}
		if ((ref name) == PropertyName.Columns)
		{
			int displayedRows = Columns;
			value = VariantUtils.CreateFrom<int>(ref displayedRows);
			return true;
		}
		if ((ref name) == PropertyName.CardPadding)
		{
			float cardPadding = CardPadding;
			value = VariantUtils.CreateFrom<float>(ref cardPadding);
			return true;
		}
		if ((ref name) == PropertyName.IsCardLibrary)
		{
			bool canScroll = IsCardLibrary;
			value = VariantUtils.CreateFrom<bool>(ref canScroll);
			return true;
		}
		if ((ref name) == PropertyName.ScrollLimitBottom)
		{
			float cardPadding = ScrollLimitBottom;
			value = VariantUtils.CreateFrom<float>(ref cardPadding);
			return true;
		}
		if ((ref name) == PropertyName.ScrollLimitTop)
		{
			float cardPadding = ScrollLimitTop;
			value = VariantUtils.CreateFrom<float>(ref cardPadding);
			return true;
		}
		if ((ref name) == PropertyName.IsAnimatingOut)
		{
			bool canScroll = IsAnimatingOut;
			value = VariantUtils.CreateFrom<bool>(ref canScroll);
			return true;
		}
		if ((ref name) == PropertyName.IsShowingUpgrades)
		{
			bool canScroll = IsShowingUpgrades;
			value = VariantUtils.CreateFrom<bool>(ref canScroll);
			return true;
		}
		if ((ref name) == PropertyName.YOffset)
		{
			int displayedRows = YOffset;
			value = VariantUtils.CreateFrom<int>(ref displayedRows);
			return true;
		}
		if ((ref name) == PropertyName.CenterGrid)
		{
			bool canScroll = CenterGrid;
			value = VariantUtils.CreateFrom<bool>(ref canScroll);
			return true;
		}
		if ((ref name) == PropertyName.DefaultFocusedControl)
		{
			Control defaultFocusedControl = DefaultFocusedControl;
			value = VariantUtils.CreateFrom<Control>(ref defaultFocusedControl);
			return true;
		}
		if ((ref name) == PropertyName.FocusedControlFromTopBar)
		{
			Control defaultFocusedControl = FocusedControlFromTopBar;
			value = VariantUtils.CreateFrom<Control>(ref defaultFocusedControl);
			return true;
		}
		if ((ref name) == PropertyName._startDrag)
		{
			value = VariantUtils.CreateFrom<float>(ref _startDrag);
			return true;
		}
		if ((ref name) == PropertyName._targetDrag)
		{
			value = VariantUtils.CreateFrom<float>(ref _targetDrag);
			return true;
		}
		if ((ref name) == PropertyName._isDragging)
		{
			value = VariantUtils.CreateFrom<bool>(ref _isDragging);
			return true;
		}
		if ((ref name) == PropertyName._scrollingEnabled)
		{
			value = VariantUtils.CreateFrom<bool>(ref _scrollingEnabled);
			return true;
		}
		if ((ref name) == PropertyName._scrollContainer)
		{
			value = VariantUtils.CreateFrom<Control>(ref _scrollContainer);
			return true;
		}
		if ((ref name) == PropertyName._scrollbarPressed)
		{
			value = VariantUtils.CreateFrom<bool>(ref _scrollbarPressed);
			return true;
		}
		if ((ref name) == PropertyName._scrollbar)
		{
			value = VariantUtils.CreateFrom<NScrollbar>(ref _scrollbar);
			return true;
		}
		if ((ref name) == PropertyName._slidingWindowCardIndex)
		{
			value = VariantUtils.CreateFrom<int>(ref _slidingWindowCardIndex);
			return true;
		}
		if ((ref name) == PropertyName._pileType)
		{
			value = VariantUtils.CreateFrom<PileType>(ref _pileType);
			return true;
		}
		if ((ref name) == PropertyName._cardSize)
		{
			value = VariantUtils.CreateFrom<Vector2>(ref _cardSize);
			return true;
		}
		if ((ref name) == PropertyName._cardsAnimatingOutForSetCards)
		{
			value = VariantUtils.CreateFrom<bool>(ref _cardsAnimatingOutForSetCards);
			return true;
		}
		if ((ref name) == PropertyName._isShowingUpgrades)
		{
			value = VariantUtils.CreateFrom<bool>(ref _isShowingUpgrades);
			return true;
		}
		if ((ref name) == PropertyName._lastFocusedHolder)
		{
			value = VariantUtils.CreateFrom<NCardHolder>(ref _lastFocusedHolder);
			return true;
		}
		if ((ref name) == PropertyName._needsReinit)
		{
			value = VariantUtils.CreateFrom<bool>(ref _needsReinit);
			return true;
		}
		return ((GodotObject)this).GetGodotClassPropertyValue(ref name, ref value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		//IL_029f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02df: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_031f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0340: Unknown result type (might be due to invalid IL or missing references)
		//IL_0361: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)3, PropertyName._startDrag, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)3, PropertyName._targetDrag, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._isDragging, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._scrollingEnabled, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName.CanScroll, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)2, PropertyName.DisplayedRows, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)2, PropertyName.Columns, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)3, PropertyName.CardPadding, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName.IsCardLibrary, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._scrollContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)3, PropertyName.ScrollLimitBottom, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)3, PropertyName.ScrollLimitTop, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._scrollbarPressed, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._scrollbar, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)2, PropertyName._slidingWindowCardIndex, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)2, PropertyName._pileType, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)5, PropertyName._cardSize, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._cardsAnimatingOutForSetCards, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._isShowingUpgrades, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._lastFocusedHolder, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName.IsAnimatingOut, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName.IsShowingUpgrades, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._needsReinit, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)2, PropertyName.YOffset, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName.CenterGrid, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.DefaultFocusedControl, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.FocusedControlFromTopBar, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		StringName displayedRows = PropertyName.DisplayedRows;
		int displayedRows2 = DisplayedRows;
		info.AddProperty(displayedRows, Variant.From<int>(ref displayedRows2));
		StringName isShowingUpgrades = PropertyName.IsShowingUpgrades;
		bool isShowingUpgrades2 = IsShowingUpgrades;
		info.AddProperty(isShowingUpgrades, Variant.From<bool>(ref isShowingUpgrades2));
		StringName yOffset = PropertyName.YOffset;
		displayedRows2 = YOffset;
		info.AddProperty(yOffset, Variant.From<int>(ref displayedRows2));
		info.AddProperty(PropertyName._startDrag, Variant.From<float>(ref _startDrag));
		info.AddProperty(PropertyName._targetDrag, Variant.From<float>(ref _targetDrag));
		info.AddProperty(PropertyName._isDragging, Variant.From<bool>(ref _isDragging));
		info.AddProperty(PropertyName._scrollingEnabled, Variant.From<bool>(ref _scrollingEnabled));
		info.AddProperty(PropertyName._scrollContainer, Variant.From<Control>(ref _scrollContainer));
		info.AddProperty(PropertyName._scrollbarPressed, Variant.From<bool>(ref _scrollbarPressed));
		info.AddProperty(PropertyName._scrollbar, Variant.From<NScrollbar>(ref _scrollbar));
		info.AddProperty(PropertyName._slidingWindowCardIndex, Variant.From<int>(ref _slidingWindowCardIndex));
		info.AddProperty(PropertyName._pileType, Variant.From<PileType>(ref _pileType));
		info.AddProperty(PropertyName._cardSize, Variant.From<Vector2>(ref _cardSize));
		info.AddProperty(PropertyName._cardsAnimatingOutForSetCards, Variant.From<bool>(ref _cardsAnimatingOutForSetCards));
		info.AddProperty(PropertyName._isShowingUpgrades, Variant.From<bool>(ref _isShowingUpgrades));
		info.AddProperty(PropertyName._lastFocusedHolder, Variant.From<NCardHolder>(ref _lastFocusedHolder));
		info.AddProperty(PropertyName._needsReinit, Variant.From<bool>(ref _needsReinit));
		info.AddSignalEventDelegate(SignalName.HolderPressed, (Delegate)backing_HolderPressed);
		info.AddSignalEventDelegate(SignalName.HolderAltPressed, (Delegate)backing_HolderAltPressed);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName.DisplayedRows, ref val))
		{
			DisplayedRows = ((Variant)(ref val)).As<int>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName.IsShowingUpgrades, ref val2))
		{
			IsShowingUpgrades = ((Variant)(ref val2)).As<bool>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName.YOffset, ref val3))
		{
			YOffset = ((Variant)(ref val3)).As<int>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._startDrag, ref val4))
		{
			_startDrag = ((Variant)(ref val4)).As<float>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._targetDrag, ref val5))
		{
			_targetDrag = ((Variant)(ref val5)).As<float>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._isDragging, ref val6))
		{
			_isDragging = ((Variant)(ref val6)).As<bool>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName._scrollingEnabled, ref val7))
		{
			_scrollingEnabled = ((Variant)(ref val7)).As<bool>();
		}
		Variant val8 = default(Variant);
		if (info.TryGetProperty(PropertyName._scrollContainer, ref val8))
		{
			_scrollContainer = ((Variant)(ref val8)).As<Control>();
		}
		Variant val9 = default(Variant);
		if (info.TryGetProperty(PropertyName._scrollbarPressed, ref val9))
		{
			_scrollbarPressed = ((Variant)(ref val9)).As<bool>();
		}
		Variant val10 = default(Variant);
		if (info.TryGetProperty(PropertyName._scrollbar, ref val10))
		{
			_scrollbar = ((Variant)(ref val10)).As<NScrollbar>();
		}
		Variant val11 = default(Variant);
		if (info.TryGetProperty(PropertyName._slidingWindowCardIndex, ref val11))
		{
			_slidingWindowCardIndex = ((Variant)(ref val11)).As<int>();
		}
		Variant val12 = default(Variant);
		if (info.TryGetProperty(PropertyName._pileType, ref val12))
		{
			_pileType = ((Variant)(ref val12)).As<PileType>();
		}
		Variant val13 = default(Variant);
		if (info.TryGetProperty(PropertyName._cardSize, ref val13))
		{
			_cardSize = ((Variant)(ref val13)).As<Vector2>();
		}
		Variant val14 = default(Variant);
		if (info.TryGetProperty(PropertyName._cardsAnimatingOutForSetCards, ref val14))
		{
			_cardsAnimatingOutForSetCards = ((Variant)(ref val14)).As<bool>();
		}
		Variant val15 = default(Variant);
		if (info.TryGetProperty(PropertyName._isShowingUpgrades, ref val15))
		{
			_isShowingUpgrades = ((Variant)(ref val15)).As<bool>();
		}
		Variant val16 = default(Variant);
		if (info.TryGetProperty(PropertyName._lastFocusedHolder, ref val16))
		{
			_lastFocusedHolder = ((Variant)(ref val16)).As<NCardHolder>();
		}
		Variant val17 = default(Variant);
		if (info.TryGetProperty(PropertyName._needsReinit, ref val17))
		{
			_needsReinit = ((Variant)(ref val17)).As<bool>();
		}
		HolderPressedEventHandler holderPressedEventHandler = default(HolderPressedEventHandler);
		if (info.TryGetSignalEventDelegate<HolderPressedEventHandler>(SignalName.HolderPressed, ref holderPressedEventHandler))
		{
			backing_HolderPressed = holderPressedEventHandler;
		}
		HolderAltPressedEventHandler holderAltPressedEventHandler = default(HolderAltPressedEventHandler);
		if (info.TryGetSignalEventDelegate<HolderAltPressedEventHandler>(SignalName.HolderAltPressed, ref holderAltPressedEventHandler))
		{
			backing_HolderAltPressed = holderAltPressedEventHandler;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotSignalList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Expected O, but got Unknown
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Expected O, but got Unknown
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(2);
		list.Add(new MethodInfo(SignalName.HolderPressed, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("card"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(SignalName.HolderAltPressed, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("card"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		return list;
	}

	protected void EmitSignalHolderPressed(NCardHolder card)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).EmitSignal(SignalName.HolderPressed, (Variant[])(object)new Variant[1] { Variant.op_Implicit((GodotObject)(object)card) });
	}

	protected void EmitSignalHolderAltPressed(NCardHolder card)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).EmitSignal(SignalName.HolderAltPressed, (Variant[])(object)new Variant[1] { Variant.op_Implicit((GodotObject)(object)card) });
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RaiseGodotClassSignalCallbacks(in godot_string_name signal, NativeVariantPtrArgs args)
	{
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		if ((ref signal) == SignalName.HolderPressed && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			backing_HolderPressed?.Invoke(VariantUtils.ConvertTo<NCardHolder>(ref ((NativeVariantPtrArgs)(ref args))[0]));
		}
		else if ((ref signal) == SignalName.HolderAltPressed && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			backing_HolderAltPressed?.Invoke(VariantUtils.ConvertTo<NCardHolder>(ref ((NativeVariantPtrArgs)(ref args))[0]));
		}
		else
		{
			((GodotObject)this).RaiseGodotClassSignalCallbacks(ref signal, args);
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassSignal(in godot_string_name signal)
	{
		if ((ref signal) == SignalName.HolderPressed)
		{
			return true;
		}
		if ((ref signal) == SignalName.HolderAltPressed)
		{
			return true;
		}
		return ((Control)this).HasGodotClassSignal(ref signal);
	}
}
