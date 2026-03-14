using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.ControllerInput;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.Cards.Holders;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Screens.Overlays;
using MegaCrit.Sts2.Core.Nodes.Screens.ScreenContext;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.Nodes.Combat;

[ScriptPath("res://src/Core/Nodes/Combat/NPlayerHand.cs")]
public class NPlayerHand : Control
{
	[Signal]
	public delegate void ModeChangedEventHandler();

	public enum Mode
	{
		None,
		Play,
		SimpleSelect,
		UpgradeSelect
	}

	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName _EnterTree = StringName.op_Implicit("_EnterTree");

		public static readonly StringName _ExitTree = StringName.op_Implicit("_ExitTree");

		public static readonly StringName IsAwaitingPlay = StringName.op_Implicit("IsAwaitingPlay");

		public static readonly StringName Add = StringName.op_Implicit("Add");

		public static readonly StringName AddCardHolder = StringName.op_Implicit("AddCardHolder");

		public static readonly StringName RemoveCardHolder = StringName.op_Implicit("RemoveCardHolder");

		public static readonly StringName OnHolderFocused = StringName.op_Implicit("OnHolderFocused");

		public static readonly StringName OnHolderUnfocused = StringName.op_Implicit("OnHolderUnfocused");

		public static readonly StringName CancelAllCardPlay = StringName.op_Implicit("CancelAllCardPlay");

		public static readonly StringName ReturnHolderToHand = StringName.op_Implicit("ReturnHolderToHand");

		public static readonly StringName ForceRefreshCardIndices = StringName.op_Implicit("ForceRefreshCardIndices");

		public static readonly StringName RefreshLayout = StringName.op_Implicit("RefreshLayout");

		public static readonly StringName OnPeekButtonToggled = StringName.op_Implicit("OnPeekButtonToggled");

		public static readonly StringName UpdateSelectModeCardVisibility = StringName.op_Implicit("UpdateSelectModeCardVisibility");

		public static readonly StringName CancelHandSelectionIfNecessary = StringName.op_Implicit("CancelHandSelectionIfNecessary");

		public static readonly StringName OnHolderPressed = StringName.op_Implicit("OnHolderPressed");

		public static readonly StringName CanPlayCards = StringName.op_Implicit("CanPlayCards");

		public static readonly StringName AreCardActionsAllowed = StringName.op_Implicit("AreCardActionsAllowed");

		public static readonly StringName StartCardPlay = StringName.op_Implicit("StartCardPlay");

		public static readonly StringName SelectCardInSimpleMode = StringName.op_Implicit("SelectCardInSimpleMode");

		public static readonly StringName SelectCardInUpgradeMode = StringName.op_Implicit("SelectCardInUpgradeMode");

		public static readonly StringName DeselectCard = StringName.op_Implicit("DeselectCard");

		public static readonly StringName OnSelectModeConfirmButtonPressed = StringName.op_Implicit("OnSelectModeConfirmButtonPressed");

		public static readonly StringName CheckIfSelectionComplete = StringName.op_Implicit("CheckIfSelectionComplete");

		public static readonly StringName RefreshSelectModeConfirmButton = StringName.op_Implicit("RefreshSelectModeConfirmButton");

		public static readonly StringName AnimIn = StringName.op_Implicit("AnimIn");

		public static readonly StringName AnimOut = StringName.op_Implicit("AnimOut");

		public static readonly StringName AnimDisable = StringName.op_Implicit("AnimDisable");

		public static readonly StringName AnimEnable = StringName.op_Implicit("AnimEnable");

		public static readonly StringName FlashPlayableHolders = StringName.op_Implicit("FlashPlayableHolders");

		public static readonly StringName OnCardSelected = StringName.op_Implicit("OnCardSelected");

		public static readonly StringName OnCardDeselected = StringName.op_Implicit("OnCardDeselected");

		public static readonly StringName UpdateSelectedCardContainer = StringName.op_Implicit("UpdateSelectedCardContainer");

		public static readonly StringName EnableControllerNavigation = StringName.op_Implicit("EnableControllerNavigation");

		public static readonly StringName DisableControllerNavigation = StringName.op_Implicit("DisableControllerNavigation");

		public static readonly StringName _UnhandledInput = StringName.op_Implicit("_UnhandledInput");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName CardHolderContainer = StringName.op_Implicit("CardHolderContainer");

		public static readonly StringName PeekButton = StringName.op_Implicit("PeekButton");

		public static readonly StringName InCardPlay = StringName.op_Implicit("InCardPlay");

		public static readonly StringName IsInCardSelection = StringName.op_Implicit("IsInCardSelection");

		public static readonly StringName CurrentMode = StringName.op_Implicit("CurrentMode");

		public static readonly StringName HasDraggedHolder = StringName.op_Implicit("HasDraggedHolder");

		public static readonly StringName FocusedHolder = StringName.op_Implicit("FocusedHolder");

		public static readonly StringName DefaultFocusedControl = StringName.op_Implicit("DefaultFocusedControl");

		public static readonly StringName _selectCardShortcuts = StringName.op_Implicit("_selectCardShortcuts");

		public static readonly StringName _selectModeBackstop = StringName.op_Implicit("_selectModeBackstop");

		public static readonly StringName _upgradePreviewContainer = StringName.op_Implicit("_upgradePreviewContainer");

		public static readonly StringName _selectedHandCardContainer = StringName.op_Implicit("_selectedHandCardContainer");

		public static readonly StringName _upgradePreview = StringName.op_Implicit("_upgradePreview");

		public static readonly StringName _selectModeConfirmButton = StringName.op_Implicit("_selectModeConfirmButton");

		public static readonly StringName _selectionHeader = StringName.op_Implicit("_selectionHeader");

		public static readonly StringName _currentCardPlay = StringName.op_Implicit("_currentCardPlay");

		public static readonly StringName _currentMode = StringName.op_Implicit("_currentMode");

		public static readonly StringName _draggedHolderIndex = StringName.op_Implicit("_draggedHolderIndex");

		public static readonly StringName _lastFocusedHolderIdx = StringName.op_Implicit("_lastFocusedHolderIdx");

		public static readonly StringName _animEnableTween = StringName.op_Implicit("_animEnableTween");

		public static readonly StringName _isDisabled = StringName.op_Implicit("_isDisabled");

		public static readonly StringName _animInTween = StringName.op_Implicit("_animInTween");

		public static readonly StringName _animOutTween = StringName.op_Implicit("_animOutTween");

		public static readonly StringName _selectedCardScaleTween = StringName.op_Implicit("_selectedCardScaleTween");
	}

	public class SignalName : SignalName
	{
		public static readonly StringName ModeChanged = StringName.op_Implicit("ModeChanged");
	}

	private StringName[] _selectCardShortcuts = (StringName[])(object)new StringName[10]
	{
		MegaInput.selectCard1,
		MegaInput.selectCard2,
		MegaInput.selectCard3,
		MegaInput.selectCard4,
		MegaInput.selectCard5,
		MegaInput.selectCard6,
		MegaInput.selectCard7,
		MegaInput.selectCard8,
		MegaInput.selectCard9,
		MegaInput.selectCard10
	};

	private Control _selectModeBackstop;

	private readonly List<CardModel> _selectedCards = new List<CardModel>();

	private CardSelectorPrefs _prefs;

	private TaskCompletionSource<IEnumerable<CardModel>>? _selectionCompletionSource;

	private Control _upgradePreviewContainer;

	private NSelectedHandCardContainer _selectedHandCardContainer;

	private NUpgradePreview _upgradePreview;

	private NConfirmButton _selectModeConfirmButton;

	private MegaRichTextLabel _selectionHeader;

	private NCardPlay? _currentCardPlay;

	private CombatState? _combatState;

	private Mode _currentMode = Mode.Play;

	private Func<CardModel, bool>? _currentSelectionFilter;

	private int _draggedHolderIndex = -1;

	private int _lastFocusedHolderIdx = -1;

	private readonly Dictionary<NHandCardHolder, int> _holdersAwaitingQueue = new Dictionary<NHandCardHolder, int>();

	private Tween? _animEnableTween;

	private bool _isDisabled;

	private const double _enableDisableDuration = 0.2;

	private static readonly Vector2 _disablePosition = new Vector2(0f, 100f);

	private static readonly Color _disableModulate = StsColors.gray;

	private Tween? _animInTween;

	private Tween? _animOutTween;

	private Tween? _selectedCardScaleTween;

	private const float _showHideAnimDuration = 0.8f;

	private static readonly Vector2 _showPosition = Vector2.Zero;

	private static readonly Vector2 _hidePosition = new Vector2(0f, 500f);

	private ModeChangedEventHandler backing_ModeChanged;

	public static NPlayerHand? Instance => NCombatRoom.Instance?.Ui.Hand;

	public Control CardHolderContainer { get; private set; }

	public NPeekButton PeekButton { get; private set; }

	public bool InCardPlay
	{
		get
		{
			if (_currentCardPlay != null)
			{
				return GodotObject.IsInstanceValid((GodotObject)(object)_currentCardPlay);
			}
			return false;
		}
	}

	public bool IsInCardSelection
	{
		get
		{
			Mode currentMode = CurrentMode;
			if ((uint)(currentMode - 2) <= 1u)
			{
				return true;
			}
			return false;
		}
	}

	public Mode CurrentMode
	{
		get
		{
			return _currentMode;
		}
		private set
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			_currentMode = value;
			((GodotObject)this).EmitSignal(SignalName.ModeChanged, Array.Empty<Variant>());
		}
	}

	private bool HasDraggedHolder => _draggedHolderIndex >= 0;

	public Func<CardModel, bool>? SelectModeGoldGlowOverride => _prefs.ShouldGlowGold;

	public NHandCardHolder? FocusedHolder { get; private set; }

	public IReadOnlyList<NHandCardHolder> ActiveHolders => Holders.Where((NHandCardHolder child) => ((CanvasItem)child).Visible).ToList();

	private IReadOnlyList<NHandCardHolder> Holders => ((IEnumerable)((Node)CardHolderContainer).GetChildren(false)).OfType<NHandCardHolder>().ToList();

	public Control DefaultFocusedControl
	{
		get
		{
			if (ActiveHolders.Count > 0)
			{
				if (_lastFocusedHolderIdx >= 0)
				{
					return (Control)(object)ActiveHolders[Mathf.Clamp(_lastFocusedHolderIdx, 0, ActiveHolders.Count - 1)];
				}
				return (Control)(object)ActiveHolders[ActiveHolders.Count / 2];
			}
			return CardHolderContainer;
		}
	}

	public event ModeChangedEventHandler ModeChanged
	{
		add
		{
			backing_ModeChanged = (ModeChangedEventHandler)Delegate.Combine(backing_ModeChanged, value);
		}
		remove
		{
			backing_ModeChanged = (ModeChangedEventHandler)Delegate.Remove(backing_ModeChanged, value);
		}
	}

	public override void _Ready()
	{
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		_selectModeBackstop = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%SelectModeBackstop"));
		CardHolderContainer = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%CardHolderContainer"));
		_upgradePreviewContainer = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%UpgradePreviewContainer"));
		_selectModeConfirmButton = ((Node)this).GetNode<NConfirmButton>(NodePath.op_Implicit("%SelectModeConfirmButton"));
		_upgradePreview = ((Node)this).GetNode<NUpgradePreview>(NodePath.op_Implicit("%UpgradePreview"));
		_selectionHeader = ((Node)this).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("%SelectionHeader"));
		((CanvasItem)_selectionHeader).Visible = false;
		_selectedHandCardContainer = ((Node)this).GetNode<NSelectedHandCardContainer>(NodePath.op_Implicit("%SelectedHandCardContainer"));
		_selectedHandCardContainer.Hand = this;
		((GodotObject)_selectModeConfirmButton).Connect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)OnSelectModeConfirmButtonPressed), 0u);
		_selectModeConfirmButton.Disable();
		((GodotObject)_selectedHandCardContainer).Connect(SignalName.ChildExitingTree, Callable.From<Node>((Action<Node>)OnCardDeselected), 0u);
		((GodotObject)_selectedHandCardContainer).Connect(SignalName.ChildEnteredTree, Callable.From<Node>((Action<Node>)OnCardSelected), 0u);
		PeekButton = ((Node)this).GetNode<NPeekButton>(NodePath.op_Implicit("%PeekButton"));
		PeekButton.Disable();
		PeekButton.AddTargets(_selectModeBackstop, _upgradePreviewContainer, _selectModeConfirmButton, (Control)_selectionHeader, _selectedHandCardContainer);
		((GodotObject)PeekButton).Connect(NPeekButton.SignalName.Toggled, Callable.From<NPeekButton>((Action<NPeekButton>)OnPeekButtonToggled), 0u);
		((GodotObject)CardHolderContainer).Connect(SignalName.FocusEntered, Callable.From((Action)delegate
		{
			if (ActiveHolders.Count > 0)
			{
				DefaultFocusedControl.TryGrabFocus();
			}
		}), 0u);
		CardHolderContainer.FocusNeighborBottom = ((Node)CardHolderContainer).GetPath();
		CardHolderContainer.FocusNeighborLeft = ((Node)CardHolderContainer).GetPath();
		CardHolderContainer.FocusNeighborRight = ((Node)CardHolderContainer).GetPath();
	}

	public override void _EnterTree()
	{
		((Node)this)._EnterTree();
		CombatManager.Instance.PlayerActionsDisabledChanged += OnPlayerActionsDisabledChanged;
		CombatManager.Instance.PlayerUnendedTurn += OnPlayerUnendedTurn;
		CombatManager.Instance.StateTracker.CombatStateChanged += OnCombatStateChanged;
		CombatManager.Instance.CombatEnded += OnCombatEnded;
	}

	public override void _ExitTree()
	{
		((Node)this)._ExitTree();
		TaskCompletionSource<IEnumerable<CardModel>> selectionCompletionSource = _selectionCompletionSource;
		if (selectionCompletionSource != null)
		{
			Task<IEnumerable<CardModel>> task = selectionCompletionSource.Task;
			if (task != null && !task.IsCompleted)
			{
				_selectionCompletionSource.SetResult(Array.Empty<CardModel>());
			}
		}
		CombatManager.Instance.PlayerActionsDisabledChanged -= OnPlayerActionsDisabledChanged;
		CombatManager.Instance.PlayerUnendedTurn -= OnPlayerUnendedTurn;
		CombatManager.Instance.StateTracker.CombatStateChanged -= OnCombatStateChanged;
		CombatManager.Instance.CombatEnded -= OnCombatEnded;
	}

	public NCard? GetCard(CardModel card)
	{
		return GetCardHolder(card)?.CardNode;
	}

	public bool IsAwaitingPlay(NHandCardHolder? holder)
	{
		if (holder != null)
		{
			return _holdersAwaitingQueue.ContainsKey(holder);
		}
		return false;
	}

	public NCardHolder? GetCardHolder(CardModel card)
	{
		return ((IEnumerable<NCardHolder>)Holders).Concat((IEnumerable<NCardHolder>)_selectedHandCardContainer.Holders).Concat(_holdersAwaitingQueue.Keys).FirstOrDefault((NCardHolder h) => h.CardNode != null && h.CardNode.Model == card);
	}

	public NHandCardHolder Add(NCard card, int index = -1)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		Vector2 globalPosition = ((Control)card).GlobalPosition;
		NHandCardHolder nHandCardHolder = NHandCardHolder.Create(card, this);
		AddCardHolder(nHandCardHolder, index);
		((Control)nHandCardHolder).GlobalPosition = globalPosition;
		RefreshLayout();
		return nHandCardHolder;
	}

	public void Remove(CardModel card)
	{
		NCardHolder cardHolder = GetCardHolder(card);
		if (cardHolder == null)
		{
			throw new InvalidOperationException($"No holder for card {card.Id}");
		}
		if (InCardPlay && card == _currentCardPlay.Holder.CardModel)
		{
			_currentCardPlay.CancelPlayCard();
		}
		RemoveCardHolder(cardHolder);
	}

	private void AddCardHolder(NHandCardHolder holder, int index)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		((Node)(object)CardHolderContainer).AddChildSafely((Node?)(object)holder);
		if (index >= 0)
		{
			((Node)CardHolderContainer).MoveChild((Node)(object)holder, index);
		}
		((GodotObject)holder).Connect(NCardHolder.SignalName.Pressed, Callable.From<NCardHolder>((Action<NCardHolder>)OnHolderPressed), 0u);
		((GodotObject)holder).Connect(NHandCardHolder.SignalName.HolderMouseClicked, Callable.From<NCardHolder>((Action<NCardHolder>)OnHolderPressed), 0u);
		((GodotObject)holder).Connect(NHandCardHolder.SignalName.HolderFocused, Callable.From<NHandCardHolder>((Action<NHandCardHolder>)OnHolderFocused), 0u);
		((GodotObject)holder).Connect(NHandCardHolder.SignalName.HolderUnfocused, Callable.From<NHandCardHolder>((Action<NHandCardHolder>)OnHolderUnfocused), 0u);
		RefreshLayout();
		if (CardHolderContainer.HasFocus())
		{
			((Control)(object)holder).TryGrabFocus();
		}
	}

	public void RemoveCardHolder(NCardHolder holder)
	{
		if (holder is NHandCardHolder key)
		{
			_holdersAwaitingQueue.Remove(key);
		}
		if (InCardPlay && _currentCardPlay.Holder == holder)
		{
			_currentCardPlay.CancelPlayCard();
		}
		bool flag = ((Control)holder).HasFocus();
		holder.Clear();
		((Node)holder).GetParent().RemoveChildSafely((Node?)(object)holder);
		((Node)(object)holder).QueueFreeSafely();
		RefreshLayout();
		if (flag)
		{
			DefaultFocusedControl.TryGrabFocus();
		}
	}

	private void OnHolderFocused(NHandCardHolder holder)
	{
		FocusedHolder = holder;
		_lastFocusedHolderIdx = ((Node)holder).GetIndex(false);
		RunManager.Instance.HoveredModelTracker.OnLocalCardHovered(FocusedHolder.CardModel);
		RefreshLayout();
	}

	private void OnHolderUnfocused(NHandCardHolder holder)
	{
		FocusedHolder = null;
		RunManager.Instance.HoveredModelTracker.OnLocalCardUnhovered();
		RefreshLayout();
	}

	public void TryCancelCardPlay(CardModel card)
	{
		NCardHolder cardHolder = GetCardHolder(card);
		if (cardHolder is NHandCardHolder nHandCardHolder && IsAwaitingPlay(nHandCardHolder))
		{
			ReturnHolderToHand(nHandCardHolder);
			nHandCardHolder.UpdateCard();
			if (InCardPlay && _currentCardPlay.Holder == nHandCardHolder)
			{
				_currentCardPlay.CancelPlayCard();
			}
			else
			{
				RefreshLayout();
			}
		}
	}

	public void CancelAllCardPlay()
	{
		if (InCardPlay)
		{
			_currentCardPlay.CancelPlayCard();
		}
		foreach (NHandCardHolder item in _holdersAwaitingQueue.Keys.ToList())
		{
			ReturnHolderToHand(item);
		}
	}

	private void ReturnHolderToHand(NHandCardHolder holder)
	{
		if (IsAwaitingPlay(holder))
		{
			int num = _holdersAwaitingQueue[holder];
			_holdersAwaitingQueue.Remove(holder);
			((Node)holder).Reparent((Node)(object)CardHolderContainer, true);
			if (num >= 0)
			{
				((Node)CardHolderContainer).MoveChild((Node)(object)holder, num);
			}
			holder.SetDefaultTargets();
		}
	}

	public void ForceRefreshCardIndices()
	{
		RefreshLayout();
	}

	private void RefreshLayout()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		int count = ActiveHolders.Count;
		if (count <= 0)
		{
			return;
		}
		int handSize = count;
		Vector2 scale = HandPosHelper.GetScale(count);
		int num = -1;
		if (FocusedHolder != null)
		{
			num = ActiveHolders.IndexOf<NHandCardHolder>(FocusedHolder);
		}
		for (int i = 0; i < count; i++)
		{
			int cardIndex = i;
			Vector2 val = HandPosHelper.GetPosition(handSize, cardIndex);
			if (num > -1)
			{
				float num2 = Mathf.Lerp(100f, 0f, Mathf.Min(1f, (float)Mathf.Abs(num - i) / 4f));
				val += Vector2.Left * (float)Mathf.Sign(num - i) * num2;
			}
			NHandCardHolder nHandCardHolder = ActiveHolders[i];
			if (num == i)
			{
				nHandCardHolder.SetAngleInstantly(0f);
				nHandCardHolder.SetScaleInstantly(Vector2.One);
				val.Y = (0f - ((Control)nHandCardHolder.Hitbox).Size.Y) * 0.5f + 2f;
				if (_isDisabled)
				{
					val -= _disablePosition;
				}
				((Control)nHandCardHolder).Position = new Vector2(((Control)nHandCardHolder).Position.X, val.Y);
				nHandCardHolder.SetTargetPosition(val);
			}
			else
			{
				nHandCardHolder.SetTargetPosition(val);
				nHandCardHolder.SetTargetScale(scale);
				nHandCardHolder.SetTargetAngle(HandPosHelper.GetAngle(handSize, cardIndex));
			}
			((Control)nHandCardHolder.Hitbox).MouseFilter = (MouseFilterEnum)(HasDraggedHolder ? 2 : 0);
			NodePath path;
			if (i <= 0)
			{
				IReadOnlyList<NHandCardHolder> activeHolders = ActiveHolders;
				path = ((Node)activeHolders[activeHolders.Count - 1]).GetPath();
			}
			else
			{
				path = ((Node)ActiveHolders[i - 1]).GetPath();
			}
			((Control)nHandCardHolder).FocusNeighborLeft = path;
			((Control)nHandCardHolder).FocusNeighborRight = ((i < ActiveHolders.Count - 1) ? ((Node)ActiveHolders[i + 1]).GetPath() : ((Node)ActiveHolders[0]).GetPath());
			((Control)nHandCardHolder).FocusNeighborBottom = ((Node)nHandCardHolder).GetPath();
			if (HasDraggedHolder && i >= _draggedHolderIndex)
			{
				nHandCardHolder.SetIndexLabel(i + 2);
			}
			else
			{
				nHandCardHolder.SetIndexLabel(i + 1);
			}
		}
	}

	private void OnPlayerUnendedTurn(Player player)
	{
		UpdateHandDisabledState(player.Creature.CombatState);
	}

	private void OnPlayerActionsDisabledChanged(CombatState state)
	{
		UpdateHandDisabledState(state);
	}

	private void UpdateHandDisabledState(CombatState state)
	{
		Player me = LocalContext.GetMe(state);
		bool flag = CombatManager.Instance.PlayerActionsDisabled;
		if (!flag && CombatManager.Instance.PlayersTakingExtraTurn.Count > 0 && me != null && !CombatManager.Instance.PlayersTakingExtraTurn.Contains(me))
		{
			flag = true;
		}
		if (flag)
		{
			if (me == null || !state.Players.Except(new _003C_003Ez__ReadOnlySingleElementList<Player>(me)).All(CombatManager.Instance.IsPlayerReadyToEndTurn))
			{
				AnimDisable();
			}
		}
		else
		{
			AnimEnable();
		}
	}

	private void OnCombatStateChanged(CombatState state)
	{
		_combatState = state;
		foreach (NHandCardHolder holder in Holders)
		{
			holder.UpdateCard();
		}
		foreach (NHandCardHolder key in _holdersAwaitingQueue.Keys)
		{
			key.UpdateCard();
		}
		foreach (NSelectedHandCardHolder holder2 in _selectedHandCardContainer.Holders)
		{
			holder2.CardNode?.UpdateVisuals(PileType.Hand, CardPreviewMode.Normal);
		}
		UpdateHandDisabledState(state);
	}

	private void OnCombatEnded(CombatRoom _)
	{
		CancelAllCardPlay();
		CardHolderContainer.FocusMode = (FocusModeEnum)0;
	}

	private void OnPeekButtonToggled(NPeekButton button)
	{
		if (button.IsPeeking)
		{
			NCombatRoom.Instance.EnableControllerNavigation();
		}
		else
		{
			NCombatRoom.Instance.RestrictControllerNavigation(Array.Empty<Control>());
			EnableControllerNavigation();
		}
		UpdateSelectModeCardVisibility();
		ActiveScreenContext.Instance.Update();
	}

	public async Task<IEnumerable<CardModel>> SelectCards(CardSelectorPrefs prefs, Func<CardModel, bool>? filter, AbstractModel? source, Mode mode = Mode.SimpleSelect)
	{
		CancelAllCardPlay();
		((CanvasItem)_selectModeBackstop).Visible = true;
		_selectModeBackstop.MouseFilter = (MouseFilterEnum)0;
		Control selectModeBackstop = _selectModeBackstop;
		Color selfModulate = ((CanvasItem)_selectModeBackstop).SelfModulate;
		selfModulate.A = 0f;
		((CanvasItem)selectModeBackstop).SelfModulate = selfModulate;
		Tween tween = ((Node)this).CreateTween();
		tween.TweenProperty((GodotObject)(object)_selectModeBackstop, NodePath.op_Implicit("self_modulate:a"), Variant.op_Implicit(1f), 0.20000000298023224);
		bool wasDisabled = _isDisabled;
		if (_isDisabled)
		{
			AnimEnable();
		}
		CurrentMode = mode;
		_currentSelectionFilter = filter;
		NCombatRoom.Instance.RestrictControllerNavigation(Array.Empty<Control>());
		NCombatUi ui = NCombatRoom.Instance.Ui;
		ui.OnHandSelectModeEntered();
		EnableControllerNavigation();
		_prefs = prefs;
		_selectionCompletionSource = new TaskCompletionSource<IEnumerable<CardModel>>();
		((CanvasItem)_selectionHeader).Visible = true;
		_selectionHeader.Text = "[center]" + prefs.Prompt.GetFormattedText() + "[/center]";
		PeekButton.Enable();
		UpdateSelectModeCardVisibility();
		RefreshSelectModeConfirmButton();
		IEnumerable<CardModel> result = await _selectionCompletionSource.Task;
		tween.Kill();
		AfterCardsSelected(source);
		if (wasDisabled)
		{
			AnimDisable();
		}
		return result;
	}

	private void UpdateSelectModeCardVisibility()
	{
		if (CurrentMode != Mode.SimpleSelect && CurrentMode != Mode.UpgradeSelect)
		{
			throw new InvalidOperationException("Can only be used when we are selecting a card");
		}
		foreach (NHandCardHolder holder in Holders)
		{
			if (holder.CardNode != null)
			{
				if (PeekButton.IsPeeking)
				{
					((CanvasItem)holder).Visible = true;
					holder.CardNode.SetPretendCardCanBePlayed(pretendCardCanBePlayed: false);
					holder.CardNode.SetForceUnpoweredPreview(forceUnpoweredPreview: false);
				}
				else
				{
					((CanvasItem)holder).Visible = _currentSelectionFilter?.Invoke(holder.CardNode.Model) ?? true;
					holder.CardNode.SetPretendCardCanBePlayed(_prefs.PretendCardsCanBePlayed);
					holder.CardNode.SetForceUnpoweredPreview(_prefs.UnpoweredPreviews);
				}
				holder.UpdateCard();
			}
		}
		RefreshLayout();
	}

	private void AfterCardsSelected(AbstractModel? source)
	{
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		_selectedCards.Clear();
		foreach (NHandCardHolder holder in Holders)
		{
			holder.InSelectMode = false;
			((CanvasItem)holder).Visible = true;
			holder.CardNode?.SetPretendCardCanBePlayed(pretendCardCanBePlayed: false);
			holder.CardNode?.SetForceUnpoweredPreview(forceUnpoweredPreview: false);
			holder.UpdateCard();
		}
		RefreshLayout();
		((CanvasItem)_selectModeBackstop).Visible = false;
		_selectModeBackstop.MouseFilter = (MouseFilterEnum)2;
		Tween val = ((Node)this).CreateTween();
		val.TweenProperty((GodotObject)(object)_selectModeBackstop, NodePath.op_Implicit("self_modulate:a"), Variant.op_Implicit(0f), 0.20000000298023224);
		_selectModeConfirmButton.Disable();
		((CanvasItem)_upgradePreviewContainer).Visible = false;
		((CanvasItem)_selectionHeader).Visible = false;
		PeekButton.Disable();
		_prefs = default(CardSelectorPrefs);
		CurrentMode = Mode.Play;
		_currentSelectionFilter = null;
		NCombatRoom.Instance.Ui.OnHandSelectModeExited();
		if (source != null)
		{
			source.ExecutionFinished += OnSelectModeSourceFinished;
		}
		else
		{
			OnSelectModeSourceFinished(null);
		}
	}

	private void CancelHandSelectionIfNecessary()
	{
		if (IsInCardSelection && _selectionCompletionSource != null)
		{
			_selectionCompletionSource.SetCanceled();
			AfterCardsSelected(null);
		}
	}

	private void OnHolderPressed(NCardHolder holder)
	{
		if (PeekButton.IsPeeking)
		{
			PeekButton.Wiggle();
			return;
		}
		NHandCardHolder nHandCardHolder = (NHandCardHolder)holder;
		if (nHandCardHolder.CardNode == null || !CombatManager.Instance.IsInProgress || NOverlayStack.Instance.ScreenCount > 0)
		{
			return;
		}
		switch (CurrentMode)
		{
		case Mode.Play:
			if (CanPlayCards())
			{
				StartCardPlay(nHandCardHolder, startedViaShortcut: false);
			}
			break;
		case Mode.SimpleSelect:
			SelectCardInSimpleMode(nHandCardHolder);
			break;
		case Mode.UpgradeSelect:
			SelectCardInUpgradeMode(nHandCardHolder);
			break;
		default:
			throw new ArgumentOutOfRangeException("CurrentMode");
		case Mode.None:
			break;
		}
	}

	private bool CanPlayCards()
	{
		if (!InCardPlay)
		{
			return AreCardActionsAllowed();
		}
		return false;
	}

	private bool AreCardActionsAllowed()
	{
		if (CombatManager.Instance.PlayersTakingExtraTurn.Count > 0 && _combatState != null)
		{
			Player me = LocalContext.GetMe(_combatState);
			if (me == null || !CombatManager.Instance.PlayersTakingExtraTurn.Contains(me))
			{
				return false;
			}
		}
		if (!CombatManager.Instance.PlayerActionsDisabled)
		{
			return !PeekButton.IsPeeking;
		}
		return false;
	}

	private void StartCardPlay(NHandCardHolder holder, bool startedViaShortcut)
	{
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		_draggedHolderIndex = ((Node)holder).GetIndex(false);
		_holdersAwaitingQueue.Add(holder, _draggedHolderIndex);
		((Node)holder).Reparent((Node)(object)this, true);
		holder.BeginDrag();
		_currentCardPlay = (NControllerManager.Instance.IsUsingController ? ((NCardPlay)NControllerCardPlay.Create(holder)) : ((NCardPlay)NMouseCardPlay.Create(holder, _selectCardShortcuts[_draggedHolderIndex], startedViaShortcut)));
		((Node)(object)this).AddChildSafely((Node?)(object)_currentCardPlay);
		((GodotObject)_currentCardPlay).Connect(NCardPlay.SignalName.Finished, Callable.From<bool>((Action<bool>)delegate(bool success)
		{
			RunManager.Instance.HoveredModelTracker.OnLocalCardDeselected();
			if (!success)
			{
				ReturnHolderToHand(holder);
			}
			_draggedHolderIndex = -1;
			RefreshLayout();
		}), 0u);
		RunManager.Instance.HoveredModelTracker.OnLocalCardSelected(holder.CardNode.Model);
		_currentCardPlay.Start();
		RefreshLayout();
		holder.SetIndexLabel(_draggedHolderIndex + 1);
	}

	private void SelectCardInSimpleMode(NHandCardHolder holder)
	{
		if (_selectedCards.Count >= _prefs.MaxSelect)
		{
			_selectedHandCardContainer.DeselectCard(_selectedCards.Last());
		}
		_selectedCards.Add(holder.CardNode.Model);
		_selectedHandCardContainer.Add(holder);
		RemoveCardHolder(holder);
		RefreshSelectModeConfirmButton();
	}

	private void SelectCardInUpgradeMode(NHandCardHolder holder)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		CardModel model = holder.CardNode.Model;
		if (_selectedCards.Count != 0)
		{
			NCard nCard = NCard.Create(_selectedCards.Last());
			((Control)nCard).GlobalPosition = _upgradePreview.SelectedCardPosition;
			DeselectCard(nCard);
		}
		_selectedCards.Add(model);
		((CanvasItem)_upgradePreviewContainer).Visible = true;
		_upgradePreview.Card = model;
		RemoveCardHolder(holder);
		RefreshSelectModeConfirmButton();
	}

	public void DeselectCard(NCard card)
	{
		if (!IsInCardSelection)
		{
			throw new InvalidOperationException("Only valid when in Select Mode.");
		}
		NHandCardHolder nHandCardHolder = Add(card, PileType.Hand.GetPile(card.Model.Owner).Cards.IndexOf<CardModel>(card.Model));
		nHandCardHolder.InSelectMode = true;
		((CanvasItem)nHandCardHolder).Visible = true;
		_selectedCards.Remove(card.Model);
		RefreshSelectModeConfirmButton();
		((Control)(object)nHandCardHolder).TryGrabFocus();
	}

	private void OnSelectModeConfirmButtonPressed(NButton _)
	{
		_selectionCompletionSource.SetResult(_selectedCards.ToList());
	}

	private void CheckIfSelectionComplete()
	{
		if (_selectedCards.Count >= _prefs.MaxSelect)
		{
			_selectionCompletionSource.SetResult(_selectedCards.ToList());
		}
	}

	private void RefreshSelectModeConfirmButton()
	{
		int count = _selectedCards.Count;
		if (count >= _prefs.MinSelect && count <= _prefs.MaxSelect)
		{
			_selectModeConfirmButton.Enable();
		}
		else
		{
			_selectModeConfirmButton.Disable();
		}
	}

	private void OnSelectModeSourceFinished(AbstractModel? source)
	{
		foreach (NSelectedHandCardHolder item in _selectedHandCardContainer.Holders.ToList())
		{
			NCard cardNode = item.CardNode;
			((Node)(object)item).QueueFreeSafely();
			Add(cardNode);
		}
		if (_upgradePreview.Card != null)
		{
			Add(NCard.Create(_upgradePreview.Card));
			_upgradePreview.Card = null;
		}
		if (source != null)
		{
			source.ExecutionFinished -= OnSelectModeSourceFinished;
		}
	}

	public void AnimIn()
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		Tween? animOutTween = _animOutTween;
		if (animOutTween != null)
		{
			animOutTween.Kill();
		}
		Tween? animEnableTween = _animEnableTween;
		if (animEnableTween != null)
		{
			animEnableTween.Kill();
		}
		_animInTween = ((Node)this).CreateTween();
		_animInTween.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("position"), Variant.op_Implicit(_showPosition), 0.800000011920929).SetEase((EaseType)1).SetTrans((TransitionType)5);
	}

	public void AnimOut()
	{
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		CancelHandSelectionIfNecessary();
		Tween? animInTween = _animInTween;
		if (animInTween != null)
		{
			animInTween.Kill();
		}
		Tween? animEnableTween = _animEnableTween;
		if (animEnableTween != null)
		{
			animEnableTween.Kill();
		}
		_animOutTween = ((Node)this).CreateTween();
		_animOutTween.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("position"), Variant.op_Implicit(_hidePosition), 0.800000011920929).SetEase((EaseType)0).SetTrans((TransitionType)10);
	}

	private void AnimDisable()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		if (!_isDisabled)
		{
			DisableControllerNavigation();
			_animEnableTween = ((Node)this).CreateTween().SetParallel(true);
			_animEnableTween.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("position"), Variant.op_Implicit(_disablePosition), 0.2).SetEase((EaseType)1).SetTrans((TransitionType)7);
			_animEnableTween.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("modulate"), Variant.op_Implicit(_disableModulate), 0.2).SetEase((EaseType)1).SetTrans((TransitionType)7);
			_isDisabled = true;
		}
	}

	private void AnimEnable()
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		if (_isDisabled)
		{
			EnableControllerNavigation();
			DefaultFocusedControl.TryGrabFocus();
			_animEnableTween = ((Node)this).CreateTween().SetParallel(true);
			_animEnableTween.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("position"), Variant.op_Implicit(_showPosition), 0.2).SetEase((EaseType)1).SetTrans((TransitionType)7);
			_animEnableTween.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("modulate"), Variant.op_Implicit(Colors.White), 0.2).SetEase((EaseType)1).SetTrans((TransitionType)7);
			_isDisabled = false;
		}
	}

	public void FlashPlayableHolders()
	{
		foreach (NHandCardHolder holder in Holders)
		{
			if (holder.CardNode != null && holder.CardNode.Model.CanPlay())
			{
				holder.Flash();
			}
		}
	}

	private void OnCardSelected(Node _)
	{
		UpdateSelectedCardContainer(((Node)_selectedHandCardContainer).GetChildCount(false));
	}

	private void OnCardDeselected(Node _)
	{
		UpdateSelectedCardContainer(((Node)_selectedHandCardContainer).GetChildCount(false) - 1);
	}

	private void UpdateSelectedCardContainer(int count)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		float num = 1f;
		float num2 = ((Control)this).Size.Y * 0.5f;
		if (count > 6)
		{
			num = 0.55f;
			num2 -= 150f;
		}
		else if (count > 3)
		{
			num = 0.8f;
			num2 -= 75f;
		}
		Tween? selectedCardScaleTween = _selectedCardScaleTween;
		if (selectedCardScaleTween != null)
		{
			selectedCardScaleTween.Kill();
		}
		_selectedCardScaleTween = ((Node)this).CreateTween().SetParallel(true);
		_selectedCardScaleTween.TweenProperty((GodotObject)(object)_selectedHandCardContainer, NodePath.op_Implicit("position:y"), Variant.op_Implicit(num2), 0.5).SetTrans((TransitionType)4).SetEase((EaseType)2);
		_selectedCardScaleTween.TweenProperty((GodotObject)(object)_selectedHandCardContainer, NodePath.op_Implicit("scale"), Variant.op_Implicit(Vector2.One * num), 0.5).SetTrans((TransitionType)4).SetEase((EaseType)2);
	}

	public void EnableControllerNavigation()
	{
		foreach (NHandCardHolder holder in Holders)
		{
			((Control)holder).FocusMode = (FocusModeEnum)2;
		}
		if (InCardPlay)
		{
			((Control)_currentCardPlay.Holder).FocusMode = (FocusModeEnum)2;
		}
	}

	public void DisableControllerNavigation()
	{
		foreach (NHandCardHolder holder in Holders)
		{
			((Control)holder).FocusMode = (FocusModeEnum)0;
		}
		if (InCardPlay)
		{
			((Control)_currentCardPlay.Holder).FocusMode = (FocusModeEnum)0;
		}
	}

	public override void _UnhandledInput(InputEvent input)
	{
		if (NControllerManager.Instance.IsUsingController || !ActiveScreenContext.Instance.IsCurrent(NCombatRoom.Instance) || CombatManager.Instance.IsOverOrEnding)
		{
			return;
		}
		List<NHandCardHolder> list = new List<NHandCardHolder>();
		list.AddRange(ActiveHolders);
		if (HasDraggedHolder)
		{
			list.Insert(_draggedHolderIndex, null);
		}
		for (int i = 0; i < _selectCardShortcuts.Length; i++)
		{
			StringName val = _selectCardShortcuts[i];
			if (!input.IsActionPressed(val, false, false) || list.Count <= i)
			{
				continue;
			}
			NHandCardHolder nHandCardHolder = list[i];
			if (nHandCardHolder == null)
			{
				continue;
			}
			if (NTargetManager.Instance.IsInSelection)
			{
				NTargetManager.Instance.CancelTargeting();
			}
			switch (CurrentMode)
			{
			case Mode.Play:
				if (AreCardActionsAllowed())
				{
					if (InCardPlay)
					{
						_currentCardPlay.CancelPlayCard();
					}
					StartCardPlay(nHandCardHolder, startedViaShortcut: true);
				}
				break;
			case Mode.SimpleSelect:
				if (!PeekButton.IsPeeking)
				{
					SelectCardInSimpleMode(nHandCardHolder);
				}
				break;
			case Mode.UpgradeSelect:
				if (!PeekButton.IsPeeking)
				{
					SelectCardInUpgradeMode(nHandCardHolder);
				}
				break;
			}
			Viewport viewport = ((Node)this).GetViewport();
			if (viewport != null)
			{
				viewport.SetInputAsHandled();
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
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Expected O, but got Unknown
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Expected O, but got Unknown
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Expected O, but got Unknown
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Expected O, but got Unknown
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Expected O, but got Unknown
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ad: Expected O, but got Unknown
		//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0301: Unknown result type (might be due to invalid IL or missing references)
		//IL_030c: Expected O, but got Unknown
		//IL_0307: Unknown result type (might be due to invalid IL or missing references)
		//IL_0312: Unknown result type (might be due to invalid IL or missing references)
		//IL_0338: Unknown result type (might be due to invalid IL or missing references)
		//IL_0341: Unknown result type (might be due to invalid IL or missing references)
		//IL_0367: Unknown result type (might be due to invalid IL or missing references)
		//IL_038f: Unknown result type (might be due to invalid IL or missing references)
		//IL_039a: Expected O, but got Unknown
		//IL_0395: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0424: Unknown result type (might be due to invalid IL or missing references)
		//IL_044c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0457: Expected O, but got Unknown
		//IL_0452: Unknown result type (might be due to invalid IL or missing references)
		//IL_045d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0483: Unknown result type (might be due to invalid IL or missing references)
		//IL_048c: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0509: Unknown result type (might be due to invalid IL or missing references)
		//IL_0514: Expected O, but got Unknown
		//IL_050f: Unknown result type (might be due to invalid IL or missing references)
		//IL_051a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0540: Unknown result type (might be due to invalid IL or missing references)
		//IL_0549: Unknown result type (might be due to invalid IL or missing references)
		//IL_056f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0578: Unknown result type (might be due to invalid IL or missing references)
		//IL_059e: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d1: Expected O, but got Unknown
		//IL_05cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_061e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0646: Unknown result type (might be due to invalid IL or missing references)
		//IL_0651: Expected O, but got Unknown
		//IL_064c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0657: Unknown result type (might be due to invalid IL or missing references)
		//IL_067d: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b0: Expected O, but got Unknown
		//IL_06ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_06dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0704: Unknown result type (might be due to invalid IL or missing references)
		//IL_070f: Expected O, but got Unknown
		//IL_070a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0715: Unknown result type (might be due to invalid IL or missing references)
		//IL_073b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0763: Unknown result type (might be due to invalid IL or missing references)
		//IL_076e: Expected O, but got Unknown
		//IL_0769: Unknown result type (might be due to invalid IL or missing references)
		//IL_0774: Unknown result type (might be due to invalid IL or missing references)
		//IL_079a: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0801: Unknown result type (might be due to invalid IL or missing references)
		//IL_0827: Unknown result type (might be due to invalid IL or missing references)
		//IL_0830: Unknown result type (might be due to invalid IL or missing references)
		//IL_0856: Unknown result type (might be due to invalid IL or missing references)
		//IL_085f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0885: Unknown result type (might be due to invalid IL or missing references)
		//IL_088e: Unknown result type (might be due to invalid IL or missing references)
		//IL_08b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_08bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_08e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_090b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0916: Expected O, but got Unknown
		//IL_0911: Unknown result type (might be due to invalid IL or missing references)
		//IL_091c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0942: Unknown result type (might be due to invalid IL or missing references)
		//IL_096a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0975: Expected O, but got Unknown
		//IL_0970: Unknown result type (might be due to invalid IL or missing references)
		//IL_097b: Unknown result type (might be due to invalid IL or missing references)
		//IL_09a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_09c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_09cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_09f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_09fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a24: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a2d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a53: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a7b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a86: Expected O, but got Unknown
		//IL_0a81: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a8c: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(37);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._EnterTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._ExitTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.IsAwaitingPlay, new PropertyInfo((Type)1, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("holder"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.Add, new PropertyInfo((Type)24, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("card"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false),
			new PropertyInfo((Type)2, StringName.op_Implicit("index"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AddCardHolder, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("holder"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false),
			new PropertyInfo((Type)2, StringName.op_Implicit("index"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.RemoveCardHolder, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("holder"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnHolderFocused, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("holder"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnHolderUnfocused, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("holder"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.CancelAllCardPlay, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ReturnHolderToHand, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("holder"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ForceRefreshCardIndices, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.RefreshLayout, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnPeekButtonToggled, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("button"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UpdateSelectModeCardVisibility, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.CancelHandSelectionIfNecessary, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnHolderPressed, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("holder"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.CanPlayCards, new PropertyInfo((Type)1, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AreCardActionsAllowed, new PropertyInfo((Type)1, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.StartCardPlay, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("holder"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false),
			new PropertyInfo((Type)1, StringName.op_Implicit("startedViaShortcut"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SelectCardInSimpleMode, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("holder"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SelectCardInUpgradeMode, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("holder"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.DeselectCard, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("card"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnSelectModeConfirmButtonPressed, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.CheckIfSelectionComplete, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.RefreshSelectModeConfirmButton, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AnimIn, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AnimOut, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AnimDisable, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AnimEnable, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.FlashPlayableHolders, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnCardSelected, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Node"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnCardDeselected, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Node"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UpdateSelectedCardContainer, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("count"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.EnableControllerNavigation, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.DisableControllerNavigation, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._UnhandledInput, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("input"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("InputEvent"), false)
		}, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_0296: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0313: Unknown result type (might be due to invalid IL or missing references)
		//IL_033b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0340: Unknown result type (might be due to invalid IL or missing references)
		//IL_0367: Unknown result type (might be due to invalid IL or missing references)
		//IL_036c: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_03de: Unknown result type (might be due to invalid IL or missing references)
		//IL_0411: Unknown result type (might be due to invalid IL or missing references)
		//IL_0444: Unknown result type (might be due to invalid IL or missing references)
		//IL_0477: Unknown result type (might be due to invalid IL or missing references)
		//IL_049c: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_050b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0530: Unknown result type (might be due to invalid IL or missing references)
		//IL_0555: Unknown result type (might be due to invalid IL or missing references)
		//IL_057a: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0613: Unknown result type (might be due to invalid IL or missing references)
		//IL_0638: Unknown result type (might be due to invalid IL or missing references)
		//IL_069a: Unknown result type (might be due to invalid IL or missing references)
		//IL_065d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0690: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
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
		if ((ref method) == MethodName.IsAwaitingPlay && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			bool flag = IsAwaitingPlay(VariantUtils.ConvertTo<NHandCardHolder>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<bool>(ref flag);
			return true;
		}
		if ((ref method) == MethodName.Add && ((NativeVariantPtrArgs)(ref args)).Count == 2)
		{
			NHandCardHolder nHandCardHolder = Add(VariantUtils.ConvertTo<NCard>(ref ((NativeVariantPtrArgs)(ref args))[0]), VariantUtils.ConvertTo<int>(ref ((NativeVariantPtrArgs)(ref args))[1]));
			ret = VariantUtils.CreateFrom<NHandCardHolder>(ref nHandCardHolder);
			return true;
		}
		if ((ref method) == MethodName.AddCardHolder && ((NativeVariantPtrArgs)(ref args)).Count == 2)
		{
			AddCardHolder(VariantUtils.ConvertTo<NHandCardHolder>(ref ((NativeVariantPtrArgs)(ref args))[0]), VariantUtils.ConvertTo<int>(ref ((NativeVariantPtrArgs)(ref args))[1]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.RemoveCardHolder && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			RemoveCardHolder(VariantUtils.ConvertTo<NCardHolder>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnHolderFocused && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OnHolderFocused(VariantUtils.ConvertTo<NHandCardHolder>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnHolderUnfocused && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OnHolderUnfocused(VariantUtils.ConvertTo<NHandCardHolder>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.CancelAllCardPlay && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			CancelAllCardPlay();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.ReturnHolderToHand && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			ReturnHolderToHand(VariantUtils.ConvertTo<NHandCardHolder>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.ForceRefreshCardIndices && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			ForceRefreshCardIndices();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.RefreshLayout && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			RefreshLayout();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnPeekButtonToggled && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OnPeekButtonToggled(VariantUtils.ConvertTo<NPeekButton>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.UpdateSelectModeCardVisibility && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			UpdateSelectModeCardVisibility();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.CancelHandSelectionIfNecessary && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			CancelHandSelectionIfNecessary();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnHolderPressed && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OnHolderPressed(VariantUtils.ConvertTo<NCardHolder>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.CanPlayCards && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			bool flag2 = CanPlayCards();
			ret = VariantUtils.CreateFrom<bool>(ref flag2);
			return true;
		}
		if ((ref method) == MethodName.AreCardActionsAllowed && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			bool flag3 = AreCardActionsAllowed();
			ret = VariantUtils.CreateFrom<bool>(ref flag3);
			return true;
		}
		if ((ref method) == MethodName.StartCardPlay && ((NativeVariantPtrArgs)(ref args)).Count == 2)
		{
			StartCardPlay(VariantUtils.ConvertTo<NHandCardHolder>(ref ((NativeVariantPtrArgs)(ref args))[0]), VariantUtils.ConvertTo<bool>(ref ((NativeVariantPtrArgs)(ref args))[1]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SelectCardInSimpleMode && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			SelectCardInSimpleMode(VariantUtils.ConvertTo<NHandCardHolder>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SelectCardInUpgradeMode && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			SelectCardInUpgradeMode(VariantUtils.ConvertTo<NHandCardHolder>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.DeselectCard && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			DeselectCard(VariantUtils.ConvertTo<NCard>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnSelectModeConfirmButtonPressed && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OnSelectModeConfirmButtonPressed(VariantUtils.ConvertTo<NButton>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.CheckIfSelectionComplete && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			CheckIfSelectionComplete();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.RefreshSelectModeConfirmButton && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			RefreshSelectModeConfirmButton();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.AnimIn && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			AnimIn();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.AnimOut && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			AnimOut();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.AnimDisable && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			AnimDisable();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.AnimEnable && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			AnimEnable();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.FlashPlayableHolders && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			FlashPlayableHolders();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnCardSelected && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OnCardSelected(VariantUtils.ConvertTo<Node>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnCardDeselected && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OnCardDeselected(VariantUtils.ConvertTo<Node>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.UpdateSelectedCardContainer && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			UpdateSelectedCardContainer(VariantUtils.ConvertTo<int>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.EnableControllerNavigation && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			EnableControllerNavigation();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.DisableControllerNavigation && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			DisableControllerNavigation();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._UnhandledInput && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			((Node)this)._UnhandledInput(VariantUtils.ConvertTo<InputEvent>(ref ((NativeVariantPtrArgs)(ref args))[0]));
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
		if ((ref method) == MethodName._EnterTree)
		{
			return true;
		}
		if ((ref method) == MethodName._ExitTree)
		{
			return true;
		}
		if ((ref method) == MethodName.IsAwaitingPlay)
		{
			return true;
		}
		if ((ref method) == MethodName.Add)
		{
			return true;
		}
		if ((ref method) == MethodName.AddCardHolder)
		{
			return true;
		}
		if ((ref method) == MethodName.RemoveCardHolder)
		{
			return true;
		}
		if ((ref method) == MethodName.OnHolderFocused)
		{
			return true;
		}
		if ((ref method) == MethodName.OnHolderUnfocused)
		{
			return true;
		}
		if ((ref method) == MethodName.CancelAllCardPlay)
		{
			return true;
		}
		if ((ref method) == MethodName.ReturnHolderToHand)
		{
			return true;
		}
		if ((ref method) == MethodName.ForceRefreshCardIndices)
		{
			return true;
		}
		if ((ref method) == MethodName.RefreshLayout)
		{
			return true;
		}
		if ((ref method) == MethodName.OnPeekButtonToggled)
		{
			return true;
		}
		if ((ref method) == MethodName.UpdateSelectModeCardVisibility)
		{
			return true;
		}
		if ((ref method) == MethodName.CancelHandSelectionIfNecessary)
		{
			return true;
		}
		if ((ref method) == MethodName.OnHolderPressed)
		{
			return true;
		}
		if ((ref method) == MethodName.CanPlayCards)
		{
			return true;
		}
		if ((ref method) == MethodName.AreCardActionsAllowed)
		{
			return true;
		}
		if ((ref method) == MethodName.StartCardPlay)
		{
			return true;
		}
		if ((ref method) == MethodName.SelectCardInSimpleMode)
		{
			return true;
		}
		if ((ref method) == MethodName.SelectCardInUpgradeMode)
		{
			return true;
		}
		if ((ref method) == MethodName.DeselectCard)
		{
			return true;
		}
		if ((ref method) == MethodName.OnSelectModeConfirmButtonPressed)
		{
			return true;
		}
		if ((ref method) == MethodName.CheckIfSelectionComplete)
		{
			return true;
		}
		if ((ref method) == MethodName.RefreshSelectModeConfirmButton)
		{
			return true;
		}
		if ((ref method) == MethodName.AnimIn)
		{
			return true;
		}
		if ((ref method) == MethodName.AnimOut)
		{
			return true;
		}
		if ((ref method) == MethodName.AnimDisable)
		{
			return true;
		}
		if ((ref method) == MethodName.AnimEnable)
		{
			return true;
		}
		if ((ref method) == MethodName.FlashPlayableHolders)
		{
			return true;
		}
		if ((ref method) == MethodName.OnCardSelected)
		{
			return true;
		}
		if ((ref method) == MethodName.OnCardDeselected)
		{
			return true;
		}
		if ((ref method) == MethodName.UpdateSelectedCardContainer)
		{
			return true;
		}
		if ((ref method) == MethodName.EnableControllerNavigation)
		{
			return true;
		}
		if ((ref method) == MethodName.DisableControllerNavigation)
		{
			return true;
		}
		if ((ref method) == MethodName._UnhandledInput)
		{
			return true;
		}
		return ((Control)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName.CardHolderContainer)
		{
			CardHolderContainer = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName.PeekButton)
		{
			PeekButton = VariantUtils.ConvertTo<NPeekButton>(ref value);
			return true;
		}
		if ((ref name) == PropertyName.CurrentMode)
		{
			CurrentMode = VariantUtils.ConvertTo<Mode>(ref value);
			return true;
		}
		if ((ref name) == PropertyName.FocusedHolder)
		{
			FocusedHolder = VariantUtils.ConvertTo<NHandCardHolder>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._selectCardShortcuts)
		{
			_selectCardShortcuts = VariantUtils.ConvertTo<StringName[]>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._selectModeBackstop)
		{
			_selectModeBackstop = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._upgradePreviewContainer)
		{
			_upgradePreviewContainer = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._selectedHandCardContainer)
		{
			_selectedHandCardContainer = VariantUtils.ConvertTo<NSelectedHandCardContainer>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._upgradePreview)
		{
			_upgradePreview = VariantUtils.ConvertTo<NUpgradePreview>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._selectModeConfirmButton)
		{
			_selectModeConfirmButton = VariantUtils.ConvertTo<NConfirmButton>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._selectionHeader)
		{
			_selectionHeader = VariantUtils.ConvertTo<MegaRichTextLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._currentCardPlay)
		{
			_currentCardPlay = VariantUtils.ConvertTo<NCardPlay>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._currentMode)
		{
			_currentMode = VariantUtils.ConvertTo<Mode>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._draggedHolderIndex)
		{
			_draggedHolderIndex = VariantUtils.ConvertTo<int>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._lastFocusedHolderIdx)
		{
			_lastFocusedHolderIdx = VariantUtils.ConvertTo<int>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._animEnableTween)
		{
			_animEnableTween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._isDisabled)
		{
			_isDisabled = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._animInTween)
		{
			_animInTween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._animOutTween)
		{
			_animOutTween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._selectedCardScaleTween)
		{
			_selectedCardScaleTween = VariantUtils.ConvertTo<Tween>(ref value);
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
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		//IL_026d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0272: Unknown result type (might be due to invalid IL or missing references)
		//IL_028d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0292: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_030d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0312: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName.CardHolderContainer)
		{
			Control cardHolderContainer = CardHolderContainer;
			value = VariantUtils.CreateFrom<Control>(ref cardHolderContainer);
			return true;
		}
		if ((ref name) == PropertyName.PeekButton)
		{
			NPeekButton peekButton = PeekButton;
			value = VariantUtils.CreateFrom<NPeekButton>(ref peekButton);
			return true;
		}
		if ((ref name) == PropertyName.InCardPlay)
		{
			bool inCardPlay = InCardPlay;
			value = VariantUtils.CreateFrom<bool>(ref inCardPlay);
			return true;
		}
		if ((ref name) == PropertyName.IsInCardSelection)
		{
			bool inCardPlay = IsInCardSelection;
			value = VariantUtils.CreateFrom<bool>(ref inCardPlay);
			return true;
		}
		if ((ref name) == PropertyName.CurrentMode)
		{
			Mode currentMode = CurrentMode;
			value = VariantUtils.CreateFrom<Mode>(ref currentMode);
			return true;
		}
		if ((ref name) == PropertyName.HasDraggedHolder)
		{
			bool inCardPlay = HasDraggedHolder;
			value = VariantUtils.CreateFrom<bool>(ref inCardPlay);
			return true;
		}
		if ((ref name) == PropertyName.FocusedHolder)
		{
			NHandCardHolder focusedHolder = FocusedHolder;
			value = VariantUtils.CreateFrom<NHandCardHolder>(ref focusedHolder);
			return true;
		}
		if ((ref name) == PropertyName.DefaultFocusedControl)
		{
			Control cardHolderContainer = DefaultFocusedControl;
			value = VariantUtils.CreateFrom<Control>(ref cardHolderContainer);
			return true;
		}
		if ((ref name) == PropertyName._selectCardShortcuts)
		{
			value = VariantUtils.CreateFrom<StringName[]>(ref _selectCardShortcuts);
			return true;
		}
		if ((ref name) == PropertyName._selectModeBackstop)
		{
			value = VariantUtils.CreateFrom<Control>(ref _selectModeBackstop);
			return true;
		}
		if ((ref name) == PropertyName._upgradePreviewContainer)
		{
			value = VariantUtils.CreateFrom<Control>(ref _upgradePreviewContainer);
			return true;
		}
		if ((ref name) == PropertyName._selectedHandCardContainer)
		{
			value = VariantUtils.CreateFrom<NSelectedHandCardContainer>(ref _selectedHandCardContainer);
			return true;
		}
		if ((ref name) == PropertyName._upgradePreview)
		{
			value = VariantUtils.CreateFrom<NUpgradePreview>(ref _upgradePreview);
			return true;
		}
		if ((ref name) == PropertyName._selectModeConfirmButton)
		{
			value = VariantUtils.CreateFrom<NConfirmButton>(ref _selectModeConfirmButton);
			return true;
		}
		if ((ref name) == PropertyName._selectionHeader)
		{
			value = VariantUtils.CreateFrom<MegaRichTextLabel>(ref _selectionHeader);
			return true;
		}
		if ((ref name) == PropertyName._currentCardPlay)
		{
			value = VariantUtils.CreateFrom<NCardPlay>(ref _currentCardPlay);
			return true;
		}
		if ((ref name) == PropertyName._currentMode)
		{
			value = VariantUtils.CreateFrom<Mode>(ref _currentMode);
			return true;
		}
		if ((ref name) == PropertyName._draggedHolderIndex)
		{
			value = VariantUtils.CreateFrom<int>(ref _draggedHolderIndex);
			return true;
		}
		if ((ref name) == PropertyName._lastFocusedHolderIdx)
		{
			value = VariantUtils.CreateFrom<int>(ref _lastFocusedHolderIdx);
			return true;
		}
		if ((ref name) == PropertyName._animEnableTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _animEnableTween);
			return true;
		}
		if ((ref name) == PropertyName._isDisabled)
		{
			value = VariantUtils.CreateFrom<bool>(ref _isDisabled);
			return true;
		}
		if ((ref name) == PropertyName._animInTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _animInTween);
			return true;
		}
		if ((ref name) == PropertyName._animOutTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _animOutTween);
			return true;
		}
		if ((ref name) == PropertyName._selectedCardScaleTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _selectedCardScaleTween);
			return true;
		}
		return ((GodotObject)this).GetGodotClassPropertyValue(ref name, ref value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
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
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_0288: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_030c: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)28, PropertyName._selectCardShortcuts, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.CardHolderContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.PeekButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._selectModeBackstop, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._upgradePreviewContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._selectedHandCardContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._upgradePreview, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._selectModeConfirmButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._selectionHeader, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._currentCardPlay, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName.InCardPlay, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName.IsInCardSelection, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)2, PropertyName._currentMode, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)2, PropertyName.CurrentMode, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)2, PropertyName._draggedHolderIndex, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName.HasDraggedHolder, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)2, PropertyName._lastFocusedHolderIdx, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._animEnableTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._isDisabled, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._animInTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._animOutTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._selectedCardScaleTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.FocusedHolder, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.DefaultFocusedControl, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		StringName cardHolderContainer = PropertyName.CardHolderContainer;
		Control cardHolderContainer2 = CardHolderContainer;
		info.AddProperty(cardHolderContainer, Variant.From<Control>(ref cardHolderContainer2));
		StringName peekButton = PropertyName.PeekButton;
		NPeekButton peekButton2 = PeekButton;
		info.AddProperty(peekButton, Variant.From<NPeekButton>(ref peekButton2));
		StringName currentMode = PropertyName.CurrentMode;
		Mode currentMode2 = CurrentMode;
		info.AddProperty(currentMode, Variant.From<Mode>(ref currentMode2));
		StringName focusedHolder = PropertyName.FocusedHolder;
		NHandCardHolder focusedHolder2 = FocusedHolder;
		info.AddProperty(focusedHolder, Variant.From<NHandCardHolder>(ref focusedHolder2));
		info.AddProperty(PropertyName._selectCardShortcuts, Variant.From<StringName[]>(ref _selectCardShortcuts));
		info.AddProperty(PropertyName._selectModeBackstop, Variant.From<Control>(ref _selectModeBackstop));
		info.AddProperty(PropertyName._upgradePreviewContainer, Variant.From<Control>(ref _upgradePreviewContainer));
		info.AddProperty(PropertyName._selectedHandCardContainer, Variant.From<NSelectedHandCardContainer>(ref _selectedHandCardContainer));
		info.AddProperty(PropertyName._upgradePreview, Variant.From<NUpgradePreview>(ref _upgradePreview));
		info.AddProperty(PropertyName._selectModeConfirmButton, Variant.From<NConfirmButton>(ref _selectModeConfirmButton));
		info.AddProperty(PropertyName._selectionHeader, Variant.From<MegaRichTextLabel>(ref _selectionHeader));
		info.AddProperty(PropertyName._currentCardPlay, Variant.From<NCardPlay>(ref _currentCardPlay));
		info.AddProperty(PropertyName._currentMode, Variant.From<Mode>(ref _currentMode));
		info.AddProperty(PropertyName._draggedHolderIndex, Variant.From<int>(ref _draggedHolderIndex));
		info.AddProperty(PropertyName._lastFocusedHolderIdx, Variant.From<int>(ref _lastFocusedHolderIdx));
		info.AddProperty(PropertyName._animEnableTween, Variant.From<Tween>(ref _animEnableTween));
		info.AddProperty(PropertyName._isDisabled, Variant.From<bool>(ref _isDisabled));
		info.AddProperty(PropertyName._animInTween, Variant.From<Tween>(ref _animInTween));
		info.AddProperty(PropertyName._animOutTween, Variant.From<Tween>(ref _animOutTween));
		info.AddProperty(PropertyName._selectedCardScaleTween, Variant.From<Tween>(ref _selectedCardScaleTween));
		info.AddSignalEventDelegate(SignalName.ModeChanged, (Delegate)backing_ModeChanged);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName.CardHolderContainer, ref val))
		{
			CardHolderContainer = ((Variant)(ref val)).As<Control>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName.PeekButton, ref val2))
		{
			PeekButton = ((Variant)(ref val2)).As<NPeekButton>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName.CurrentMode, ref val3))
		{
			CurrentMode = ((Variant)(ref val3)).As<Mode>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName.FocusedHolder, ref val4))
		{
			FocusedHolder = ((Variant)(ref val4)).As<NHandCardHolder>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._selectCardShortcuts, ref val5))
		{
			_selectCardShortcuts = ((Variant)(ref val5)).As<StringName[]>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._selectModeBackstop, ref val6))
		{
			_selectModeBackstop = ((Variant)(ref val6)).As<Control>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName._upgradePreviewContainer, ref val7))
		{
			_upgradePreviewContainer = ((Variant)(ref val7)).As<Control>();
		}
		Variant val8 = default(Variant);
		if (info.TryGetProperty(PropertyName._selectedHandCardContainer, ref val8))
		{
			_selectedHandCardContainer = ((Variant)(ref val8)).As<NSelectedHandCardContainer>();
		}
		Variant val9 = default(Variant);
		if (info.TryGetProperty(PropertyName._upgradePreview, ref val9))
		{
			_upgradePreview = ((Variant)(ref val9)).As<NUpgradePreview>();
		}
		Variant val10 = default(Variant);
		if (info.TryGetProperty(PropertyName._selectModeConfirmButton, ref val10))
		{
			_selectModeConfirmButton = ((Variant)(ref val10)).As<NConfirmButton>();
		}
		Variant val11 = default(Variant);
		if (info.TryGetProperty(PropertyName._selectionHeader, ref val11))
		{
			_selectionHeader = ((Variant)(ref val11)).As<MegaRichTextLabel>();
		}
		Variant val12 = default(Variant);
		if (info.TryGetProperty(PropertyName._currentCardPlay, ref val12))
		{
			_currentCardPlay = ((Variant)(ref val12)).As<NCardPlay>();
		}
		Variant val13 = default(Variant);
		if (info.TryGetProperty(PropertyName._currentMode, ref val13))
		{
			_currentMode = ((Variant)(ref val13)).As<Mode>();
		}
		Variant val14 = default(Variant);
		if (info.TryGetProperty(PropertyName._draggedHolderIndex, ref val14))
		{
			_draggedHolderIndex = ((Variant)(ref val14)).As<int>();
		}
		Variant val15 = default(Variant);
		if (info.TryGetProperty(PropertyName._lastFocusedHolderIdx, ref val15))
		{
			_lastFocusedHolderIdx = ((Variant)(ref val15)).As<int>();
		}
		Variant val16 = default(Variant);
		if (info.TryGetProperty(PropertyName._animEnableTween, ref val16))
		{
			_animEnableTween = ((Variant)(ref val16)).As<Tween>();
		}
		Variant val17 = default(Variant);
		if (info.TryGetProperty(PropertyName._isDisabled, ref val17))
		{
			_isDisabled = ((Variant)(ref val17)).As<bool>();
		}
		Variant val18 = default(Variant);
		if (info.TryGetProperty(PropertyName._animInTween, ref val18))
		{
			_animInTween = ((Variant)(ref val18)).As<Tween>();
		}
		Variant val19 = default(Variant);
		if (info.TryGetProperty(PropertyName._animOutTween, ref val19))
		{
			_animOutTween = ((Variant)(ref val19)).As<Tween>();
		}
		Variant val20 = default(Variant);
		if (info.TryGetProperty(PropertyName._selectedCardScaleTween, ref val20))
		{
			_selectedCardScaleTween = ((Variant)(ref val20)).As<Tween>();
		}
		ModeChangedEventHandler modeChangedEventHandler = default(ModeChangedEventHandler);
		if (info.TryGetSignalEventDelegate<ModeChangedEventHandler>(SignalName.ModeChanged, ref modeChangedEventHandler))
		{
			backing_ModeChanged = modeChangedEventHandler;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotSignalList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(1);
		list.Add(new MethodInfo(SignalName.ModeChanged, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	protected void EmitSignalModeChanged()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).EmitSignal(SignalName.ModeChanged, Array.Empty<Variant>());
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RaiseGodotClassSignalCallbacks(in godot_string_name signal, NativeVariantPtrArgs args)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		if ((ref signal) == SignalName.ModeChanged && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			backing_ModeChanged?.Invoke();
		}
		else
		{
			((GodotObject)this).RaiseGodotClassSignalCallbacks(ref signal, args);
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassSignal(in godot_string_name signal)
	{
		if ((ref signal) == SignalName.ModeChanged)
		{
			return true;
		}
		return ((Control)this).HasGodotClassSignal(ref signal);
	}
}
