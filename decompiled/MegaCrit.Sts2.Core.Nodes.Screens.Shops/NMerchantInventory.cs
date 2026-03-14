using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.Collections;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Merchant;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Screens.ScreenContext;
using MegaCrit.Sts2.Core.Saves;

namespace MegaCrit.Sts2.Core.Nodes.Screens.Shops;

[ScriptPath("res://src/Core/Nodes/Screens/Shops/NMerchantInventory.cs")]
public class NMerchantInventory : Control, IScreenContext
{
	[Signal]
	public delegate void InventoryClosedEventHandler();

	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName _EnterTree = StringName.op_Implicit("_EnterTree");

		public static readonly StringName _ExitTree = StringName.op_Implicit("_ExitTree");

		public static readonly StringName Open = StringName.op_Implicit("Open");

		public static readonly StringName SubscribeToEntries = StringName.op_Implicit("SubscribeToEntries");

		public static readonly StringName Close = StringName.op_Implicit("Close");

		public static readonly StringName OnCardRemovalUsed = StringName.op_Implicit("OnCardRemovalUsed");

		public static readonly StringName UpdateNavigation = StringName.op_Implicit("UpdateNavigation");

		public static readonly StringName UpdateHorizontalNavigation = StringName.op_Implicit("UpdateHorizontalNavigation");

		public static readonly StringName UpdateVerticalNavigation = StringName.op_Implicit("UpdateVerticalNavigation");

		public static readonly StringName OnActiveScreenUpdated = StringName.op_Implicit("OnActiveScreenUpdated");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName IsOpen = StringName.op_Implicit("IsOpen");

		public static readonly StringName MerchantHand = StringName.op_Implicit("MerchantHand");

		public static readonly StringName DefaultFocusedControl = StringName.op_Implicit("DefaultFocusedControl");

		public static readonly StringName _characterCardContainer = StringName.op_Implicit("_characterCardContainer");

		public static readonly StringName _colorlessCardContainer = StringName.op_Implicit("_colorlessCardContainer");

		public static readonly StringName _relicContainer = StringName.op_Implicit("_relicContainer");

		public static readonly StringName _potionContainer = StringName.op_Implicit("_potionContainer");

		public static readonly StringName _cardRemovalNode = StringName.op_Implicit("_cardRemovalNode");

		public static readonly StringName _backButton = StringName.op_Implicit("_backButton");

		public static readonly StringName _merchantDialogue = StringName.op_Implicit("_merchantDialogue");

		public static readonly StringName _inventoryTween = StringName.op_Implicit("_inventoryTween");

		public static readonly StringName _slotsContainer = StringName.op_Implicit("_slotsContainer");

		public static readonly StringName _backstop = StringName.op_Implicit("_backstop");

		public static readonly StringName _lastSlot = StringName.op_Implicit("_lastSlot");
	}

	public class SignalName : SignalName
	{
		public static readonly StringName InventoryClosed = StringName.op_Implicit("InventoryClosed");
	}

	private const float _openPosition = 80f;

	private const float _closedPosition = -1000f;

	private Control? _characterCardContainer;

	private Control? _colorlessCardContainer;

	protected Control? _relicContainer;

	private Control? _potionContainer;

	private NMerchantCardRemoval? _cardRemovalNode;

	private NBackButton _backButton;

	private NMerchantDialogue _merchantDialogue;

	private Tween? _inventoryTween;

	private Control _slotsContainer;

	private ColorRect _backstop;

	private NMerchantSlot? _lastSlot;

	private InventoryClosedEventHandler backing_InventoryClosed;

	public MerchantInventory? Inventory { get; private set; }

	public bool IsOpen { get; private set; }

	public NMerchantHand MerchantHand { get; private set; }

	public Control? DefaultFocusedControl
	{
		get
		{
			NMerchantSlot lastSlot = _lastSlot;
			if (lastSlot != null)
			{
				MerchantEntry entry = lastSlot.Entry;
				if (entry != null && entry.IsStocked)
				{
					return (Control?)(object)_lastSlot;
				}
			}
			return (Control?)(object)GetAllSlots().FirstOrDefault((NMerchantSlot s) => s.Entry.IsStocked);
		}
	}

	public event InventoryClosedEventHandler InventoryClosed
	{
		add
		{
			backing_InventoryClosed = (InventoryClosedEventHandler)Delegate.Combine(backing_InventoryClosed, value);
		}
		remove
		{
			backing_InventoryClosed = (InventoryClosedEventHandler)Delegate.Remove(backing_InventoryClosed, value);
		}
	}

	public override void _Ready()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		_merchantDialogue = ((Node)this).GetNode<NMerchantDialogue>(NodePath.op_Implicit("%Dialogue"));
		((CanvasItem)_merchantDialogue).Modulate = Colors.Transparent;
		_slotsContainer = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%SlotsContainer"));
		_slotsContainer.Position = new Vector2(_slotsContainer.Position.X, -1000f);
		_backstop = ((Node)this).GetNode<ColorRect>(NodePath.op_Implicit("Backstop"));
		MerchantHand = ((Node)this).GetNode<NMerchantHand>(NodePath.op_Implicit("%MerchantHand"));
		_characterCardContainer = ((Node)this).GetNodeOrNull<Control>(NodePath.op_Implicit("%CharacterCards"));
		_colorlessCardContainer = ((Node)this).GetNodeOrNull<Control>(NodePath.op_Implicit("%ColorlessCards"));
		_relicContainer = ((Node)this).GetNodeOrNull<Control>(NodePath.op_Implicit("%Relics"));
		_potionContainer = ((Node)this).GetNodeOrNull<Control>(NodePath.op_Implicit("%Potions"));
		_cardRemovalNode = ((Node)this).GetNodeOrNull<NMerchantCardRemoval>(NodePath.op_Implicit("%MerchantCardRemoval"));
		_backButton = ((Node)this).GetNode<NBackButton>(NodePath.op_Implicit("%BackButton"));
		((GodotObject)_backButton).Connect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)delegate
		{
			Close();
		}), 0u);
		_backButton.Disable();
		NGame.Instance.SetScreenShakeTarget((Control)(object)this);
	}

	public override void _EnterTree()
	{
		ActiveScreenContext.Instance.Updated += OnActiveScreenUpdated;
		SubscribeToEntries();
	}

	public override void _ExitTree()
	{
		ActiveScreenContext.Instance.Updated -= OnActiveScreenUpdated;
		if (Inventory == null)
		{
			return;
		}
		foreach (MerchantEntry allEntry in Inventory.AllEntries)
		{
			allEntry.PurchaseCompleted -= OnPurchaseCompleted;
			allEntry.PurchaseFailed -= _merchantDialogue.ShowForPurchaseAttempt;
		}
	}

	public void Initialize(MerchantInventory inventory, MerchantDialogueSet dialogue)
	{
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		if (Inventory != null)
		{
			throw new InvalidOperationException("Merchant inventory already populated.");
		}
		Inventory = inventory;
		for (int i = 0; i < Inventory.CharacterCardEntries.Count; i++)
		{
			NMerchantCard child = ((Node)_characterCardContainer).GetChild<NMerchantCard>(i, false);
			child.Initialize(this);
			child.FillSlot(Inventory.CharacterCardEntries[i]);
		}
		for (int j = 0; j < Inventory.ColorlessCardEntries.Count; j++)
		{
			NMerchantCard child2 = ((Node)_colorlessCardContainer).GetChild<NMerchantCard>(j, false);
			child2.Initialize(this);
			child2.FillSlot(Inventory.ColorlessCardEntries[j]);
		}
		for (int k = 0; k < Inventory.RelicEntries.Count; k++)
		{
			NMerchantRelic child3 = ((Node)_relicContainer).GetChild<NMerchantRelic>(k, false);
			child3.Initialize(this);
			child3.FillSlot(Inventory.RelicEntries[k]);
		}
		for (int l = 0; l < Inventory.PotionEntries.Count; l++)
		{
			NMerchantPotion child4 = ((Node)_potionContainer).GetChild<NMerchantPotion>(l, false);
			child4.Initialize(this);
			child4.FillSlot(Inventory.PotionEntries[l]);
		}
		if (Inventory.CardRemovalEntry != null)
		{
			_cardRemovalNode.Initialize(this);
			_cardRemovalNode.FillSlot(Inventory.CardRemovalEntry);
		}
		SubscribeToEntries();
		UpdateNavigation();
		foreach (NMerchantSlot slot in GetAllSlots())
		{
			((GodotObject)slot).Connect(SignalName.FocusEntered, Callable.From((Action)delegate
			{
				_lastSlot = slot;
			}), 0u);
		}
		_merchantDialogue.Initialize(dialogue);
	}

	public void Open()
	{
		if (!SaveManager.Instance.SeenFtue("merchant_ftue"))
		{
			SaveManager.Instance.MarkFtueAsComplete("merchant_ftue");
		}
		TaskHelper.RunSafely(DoOpenAnimation());
		((Control)this).MouseFilter = (MouseFilterEnum)0;
		_backButton.Enable();
		foreach (NMerchantCard cardSlot in GetCardSlots())
		{
			cardSlot.OnInventoryOpened();
		}
		SfxCmd.Play("event:/sfx/npcs/merchant/merchant_welcome");
		IsOpen = true;
		ActiveScreenContext.Instance.Update();
		_merchantDialogue.ShowOnInventoryOpen();
	}

	private void SubscribeToEntries()
	{
		if (!((Node)this).IsNodeReady() || Inventory == null)
		{
			return;
		}
		foreach (MerchantEntry allEntry in Inventory.AllEntries)
		{
			allEntry.PurchaseCompleted += OnPurchaseCompleted;
			allEntry.PurchaseFailed += _merchantDialogue.ShowForPurchaseAttempt;
		}
	}

	private async Task DoOpenAnimation()
	{
		Tween? inventoryTween = _inventoryTween;
		if (inventoryTween != null)
		{
			inventoryTween.Kill();
		}
		_inventoryTween = ((Node)this).CreateTween().SetParallel(true);
		_inventoryTween.TweenProperty((GodotObject)(object)_backstop, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0.8f), 1.0).SetEase((EaseType)2).SetTrans((TransitionType)1)
			.FromCurrent();
		_inventoryTween.TweenProperty((GodotObject)(object)_slotsContainer, NodePath.op_Implicit("position:y"), Variant.op_Implicit(80f), 0.699999988079071).SetEase((EaseType)1).SetTrans((TransitionType)2)
			.FromCurrent();
		await ((GodotObject)this).ToSignal((GodotObject)(object)_inventoryTween, SignalName.Finished);
	}

	private void Close()
	{
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		MerchantHand.StopPointing(0f);
		Tween? inventoryTween = _inventoryTween;
		if (inventoryTween != null)
		{
			inventoryTween.Kill();
		}
		_inventoryTween = ((Node)this).CreateTween().SetParallel(true);
		_inventoryTween.TweenProperty((GodotObject)(object)_backstop, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0f), 0.800000011920929).SetEase((EaseType)2).SetTrans((TransitionType)1)
			.FromCurrent();
		_inventoryTween.TweenProperty((GodotObject)(object)_slotsContainer, NodePath.op_Implicit("position:y"), Variant.op_Implicit(-1000f), 0.5).SetEase((EaseType)1).SetTrans((TransitionType)7)
			.FromCurrent();
		((Control)this).MouseFilter = (MouseFilterEnum)2;
		_backButton.Disable();
		_lastSlot = null;
		IsOpen = false;
		ActiveScreenContext.Instance.Update();
		EmitSignalInventoryClosed();
	}

	private void OnPurchaseCompleted(PurchaseStatus status, MerchantEntry entry)
	{
		UpdateNavigation();
		NMerchantSlot lastSlot = GetAllSlots().FirstOrDefault((NMerchantSlot s) => s.Entry == entry);
		if (lastSlot != null)
		{
			((Control)(object)(from s in GetAllSlots()
				where ((CanvasItem)s).Visible && s != lastSlot
				select s).MinBy(delegate(NMerchantSlot s)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_000c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0011: Unknown result type (might be due to invalid IL or missing references)
				//IL_0016: Unknown result type (might be due to invalid IL or missing references)
				Vector2 val = ((Control)s).GlobalPosition - ((Control)lastSlot).GlobalPosition;
				return ((Vector2)(ref val)).Length();
			}))?.TryGrabFocus();
		}
		SfxCmd.Play("event:/sfx/npcs/merchant/merchant_thank_yous");
		_merchantDialogue.ShowForPurchaseAttempt(status);
	}

	public void OnCardRemovalUsed()
	{
		_cardRemovalNode.OnCardRemovalUsed();
	}

	public IEnumerable<NMerchantSlot> GetAllSlots()
	{
		List<NMerchantSlot> list = new List<NMerchantSlot>();
		list.AddRange(GetCardSlots());
		if (_relicContainer != null)
		{
			list.AddRange(((IEnumerable)((Node)_relicContainer).GetChildren(false)).OfType<NMerchantRelic>());
		}
		if (_potionContainer != null)
		{
			list.AddRange(((IEnumerable)((Node)_potionContainer).GetChildren(false)).OfType<NMerchantPotion>());
		}
		if (_cardRemovalNode != null)
		{
			list.Add(_cardRemovalNode);
		}
		return list;
	}

	private IEnumerable<NMerchantCard> GetCardSlots()
	{
		IEnumerable<Node>[] array = new IEnumerable<Node>[2];
		Control? characterCardContainer = _characterCardContainer;
		array[0] = (IEnumerable<Node>)(((characterCardContainer != null) ? ((Node)characterCardContainer).GetChildren(false) : null) ?? new Array<Node>());
		Control? colorlessCardContainer = _colorlessCardContainer;
		array[1] = (IEnumerable<Node>)(((colorlessCardContainer != null) ? ((Node)colorlessCardContainer).GetChildren(false) : null) ?? new Array<Node>());
		return array.SelectMany((IEnumerable<Node> n) => n).OfType<NMerchantCard>();
	}

	protected virtual void UpdateNavigation()
	{
		UpdateHorizontalNavigation();
		UpdateVerticalNavigation();
	}

	private void UpdateHorizontalNavigation()
	{
		Control? characterCardContainer = _characterCardContainer;
		List<NMerchantSlot> source = ((characterCardContainer != null) ? (from c in ((IEnumerable)((Node)characterCardContainer).GetChildren(false)).OfType<NMerchantSlot>()
			where ((CanvasItem)c).Visible
			select c).ToList() : null) ?? new List<NMerchantSlot>();
		Control? colorlessCardContainer = _colorlessCardContainer;
		List<NMerchantSlot> list = ((colorlessCardContainer != null) ? (from c in ((IEnumerable)((Node)colorlessCardContainer).GetChildren(false)).OfType<NMerchantSlot>()
			where ((CanvasItem)c).Visible
			select c).ToList() : null) ?? new List<NMerchantSlot>();
		Control? relicContainer = _relicContainer;
		List<NMerchantSlot> list2 = ((relicContainer != null) ? (from c in ((IEnumerable)((Node)relicContainer).GetChildren(false)).OfType<NMerchantSlot>()
			where ((CanvasItem)c).Visible
			select c).ToList() : null) ?? new List<NMerchantSlot>();
		Control? potionContainer = _potionContainer;
		List<NMerchantSlot> list3 = ((potionContainer != null) ? (from c in ((IEnumerable)((Node)potionContainer).GetChildren(false)).OfType<NMerchantSlot>()
			where ((CanvasItem)c).Visible
			select c).ToList() : null) ?? new List<NMerchantSlot>();
		List<NMerchantSlot> list4 = source.ToList();
		IEnumerable<NMerchantSlot> first = list.Concat(list2);
		IEnumerable<NMerchantSlot> second;
		if (_cardRemovalNode == null)
		{
			IEnumerable<NMerchantSlot> enumerable = Array.Empty<NMerchantSlot>();
			second = enumerable;
		}
		else
		{
			IEnumerable<NMerchantSlot> enumerable = new _003C_003Ez__ReadOnlySingleElementList<NMerchantSlot>(_cardRemovalNode);
			second = enumerable;
		}
		List<NMerchantSlot> list5 = first.Concat(second).ToList();
		IEnumerable<NMerchantSlot> first2 = list.Concat(list3);
		IEnumerable<NMerchantSlot> second2;
		if (_cardRemovalNode == null)
		{
			IEnumerable<NMerchantSlot> enumerable = Array.Empty<NMerchantSlot>();
			second2 = enumerable;
		}
		else
		{
			IEnumerable<NMerchantSlot> enumerable = new _003C_003Ez__ReadOnlySingleElementList<NMerchantSlot>(_cardRemovalNode);
			second2 = enumerable;
		}
		List<NMerchantSlot> list6 = first2.Concat(second2).ToList();
		for (int num = 0; num < list4.Count; num++)
		{
			((Control)list4[num]).FocusNeighborLeft = ((num > 0) ? ((Node)list4[num - 1]).GetPath() : ((Node)list4[num]).GetPath());
			((Control)list4[num]).FocusNeighborRight = ((num < list4.Count - 1) ? ((Node)list4[num + 1]).GetPath() : ((Node)list4[num]).GetPath());
		}
		for (int num2 = 0; num2 < list6.Count; num2++)
		{
			((Control)list6[num2]).FocusNeighborLeft = ((num2 > 0) ? ((Node)list6[num2 - 1]).GetPath() : ((Node)list6[num2]).GetPath());
			((Control)list6[num2]).FocusNeighborRight = ((num2 < list6.Count - 1) ? ((Node)list6[num2 + 1]).GetPath() : ((Node)list6[num2]).GetPath());
		}
		for (int num3 = 0; num3 < list5.Count; num3++)
		{
			((Control)list5[num3]).FocusNeighborLeft = ((num3 > 0) ? ((Node)list5[num3 - 1]).GetPath() : ((Node)list5[num3]).GetPath());
			((Control)list5[num3]).FocusNeighborRight = ((num3 < list5.Count - 1) ? ((Node)list5[num3 + 1]).GetPath() : ((Node)list5[num3]).GetPath());
		}
		if (list2.Count == 0 && list3.Count > 0)
		{
			if (_cardRemovalNode != null)
			{
				((Control)_cardRemovalNode).FocusNeighborLeft = ((Node)list3.Last()).GetPath();
			}
			if (list.Count > 0)
			{
				((Control)list.Last()).FocusNeighborRight = ((Node)list3.First()).GetPath();
			}
		}
	}

	private void UpdateVerticalNavigation()
	{
		Control? characterCardContainer = _characterCardContainer;
		List<NMerchantSlot> source = ((characterCardContainer != null) ? ((IEnumerable)((Node)characterCardContainer).GetChildren(false)).OfType<NMerchantSlot>().ToList() : null) ?? new List<NMerchantSlot>();
		Control? colorlessCardContainer = _colorlessCardContainer;
		List<NMerchantSlot> first = ((colorlessCardContainer != null) ? ((IEnumerable)((Node)colorlessCardContainer).GetChildren(false)).OfType<NMerchantSlot>().ToList() : null) ?? new List<NMerchantSlot>();
		Control? relicContainer = _relicContainer;
		List<NMerchantSlot> second = ((relicContainer != null) ? ((IEnumerable)((Node)relicContainer).GetChildren(false)).OfType<NMerchantSlot>().ToList() : null) ?? new List<NMerchantSlot>();
		Control? potionContainer = _potionContainer;
		List<NMerchantSlot> second2 = ((potionContainer != null) ? ((IEnumerable)((Node)potionContainer).GetChildren(false)).OfType<NMerchantSlot>().ToList() : null) ?? new List<NMerchantSlot>();
		List<NMerchantSlot> list = source.ToList();
		IEnumerable<NMerchantSlot> first2 = first.Concat(second);
		IEnumerable<NMerchantSlot> second3;
		if (_cardRemovalNode == null)
		{
			IEnumerable<NMerchantSlot> enumerable = Array.Empty<NMerchantSlot>();
			second3 = enumerable;
		}
		else
		{
			IEnumerable<NMerchantSlot> enumerable = new _003C_003Ez__ReadOnlySingleElementList<NMerchantSlot>(_cardRemovalNode);
			second3 = enumerable;
		}
		List<NMerchantSlot> list2 = first2.Concat(second3).ToList();
		IEnumerable<NMerchantSlot> first3 = first.Concat(second2);
		IEnumerable<NMerchantSlot> second4;
		if (_cardRemovalNode == null)
		{
			IEnumerable<NMerchantSlot> enumerable = Array.Empty<NMerchantSlot>();
			second4 = enumerable;
		}
		else
		{
			IEnumerable<NMerchantSlot> enumerable = new _003C_003Ez__ReadOnlySingleElementList<NMerchantSlot>(_cardRemovalNode);
			second4 = enumerable;
		}
		List<NMerchantSlot> list3 = first3.Concat(second4).ToList();
		for (int i = 0; i < list.Count; i++)
		{
			((Control)list[i]).FocusNeighborTop = ((Node)list[i]).GetPath();
			if (list2.Count > 0)
			{
				Control closestVisible = GetClosestVisible(i, list2);
				if (closestVisible != null)
				{
					((Control)list[i]).FocusNeighborBottom = ((Node)closestVisible).GetPath();
					continue;
				}
			}
			Control closestVisible2 = GetClosestVisible(i, list3);
			if (closestVisible2 != null)
			{
				((Control)list[i]).FocusNeighborBottom = ((Node)closestVisible2).GetPath();
			}
			else
			{
				((Control)list[i]).FocusNeighborBottom = ((Node)list[i]).GetPath();
			}
		}
		for (int j = 0; j < list3.Count; ((Control)list3[j]).FocusNeighborBottom = ((Node)list3[j]).GetPath(), j++)
		{
			if (list2.Count > 0)
			{
				Control closestVisible3 = GetClosestVisible(j, list2);
				if (closestVisible3 != null)
				{
					((Control)list3[j]).FocusNeighborTop = ((Node)closestVisible3).GetPath();
					continue;
				}
			}
			Control closestVisible4 = GetClosestVisible(j, list);
			if (closestVisible4 != null)
			{
				((Control)list3[j]).FocusNeighborTop = ((Node)closestVisible4).GetPath();
			}
			else
			{
				((Control)list3[j]).FocusNeighborTop = ((Node)list3[j]).GetPath();
			}
		}
		for (int k = 0; k < list2.Count; k++)
		{
			if (list.Count > 0)
			{
				Control closestVisible5 = GetClosestVisible(k, list);
				if (closestVisible5 != null)
				{
					((Control)list2[k]).FocusNeighborTop = ((Node)closestVisible5).GetPath();
					goto IL_02bb;
				}
			}
			((Control)list2[k]).FocusNeighborTop = ((Node)list2[k]).GetPath();
			goto IL_02bb;
			IL_02bb:
			if (list3.Count > 0)
			{
				Control closestVisible6 = GetClosestVisible(k, list3);
				if (closestVisible6 != null)
				{
					((Control)list2[k]).FocusNeighborBottom = ((Node)closestVisible6).GetPath();
					continue;
				}
			}
			((Control)list2[k]).FocusNeighborBottom = ((Node)list2[k]).GetPath();
		}
	}

	private Control? GetClosestVisible(int idx, List<NMerchantSlot> row)
	{
		NMerchantSlot nMerchantSlot = row[Math.Min(idx, row.Count - 1)];
		if (((CanvasItem)nMerchantSlot).Visible)
		{
			return (Control?)(object)nMerchantSlot;
		}
		int num = row.IndexOf(nMerchantSlot);
		int num2 = num - 1;
		int num3 = num + 1;
		while (num2 >= 0 || num3 < row.Count)
		{
			if (num3 < row.Count)
			{
				if (((CanvasItem)row[num3]).Visible)
				{
					return (Control?)(object)row[num3];
				}
				num3++;
			}
			if (num2 >= 0)
			{
				if (((CanvasItem)row[num2]).Visible)
				{
					return (Control?)(object)row[num2];
				}
				num2--;
			}
		}
		return null;
	}

	private void OnActiveScreenUpdated()
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		this.UpdateControllerNavEnabled();
		if (ActiveScreenContext.Instance.IsCurrent(this))
		{
			if (_characterCardContainer != null && NControllerManager.Instance.IsUsingController && _inventoryTween != null && _inventoryTween.IsRunning())
			{
				float num = 80f - _slotsContainer.Position.Y;
				MerchantHand.PointAtTarget(((Control)((Node)_characterCardContainer).GetChild<NMerchantCard>(0, false)).GlobalPosition + Vector2.Down * num);
			}
			_backButton.Enable();
		}
		else
		{
			_backButton.Disable();
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
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(11);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._EnterTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._ExitTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.Open, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SubscribeToEntries, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.Close, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnCardRemovalUsed, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UpdateNavigation, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UpdateHorizontalNavigation, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UpdateVerticalNavigation, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnActiveScreenUpdated, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
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
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
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
		if ((ref method) == MethodName.Open && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			Open();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SubscribeToEntries && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			SubscribeToEntries();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.Close && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			Close();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnCardRemovalUsed && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnCardRemovalUsed();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.UpdateNavigation && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			UpdateNavigation();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.UpdateHorizontalNavigation && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			UpdateHorizontalNavigation();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.UpdateVerticalNavigation && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			UpdateVerticalNavigation();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnActiveScreenUpdated && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnActiveScreenUpdated();
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
		if ((ref method) == MethodName.Open)
		{
			return true;
		}
		if ((ref method) == MethodName.SubscribeToEntries)
		{
			return true;
		}
		if ((ref method) == MethodName.Close)
		{
			return true;
		}
		if ((ref method) == MethodName.OnCardRemovalUsed)
		{
			return true;
		}
		if ((ref method) == MethodName.UpdateNavigation)
		{
			return true;
		}
		if ((ref method) == MethodName.UpdateHorizontalNavigation)
		{
			return true;
		}
		if ((ref method) == MethodName.UpdateVerticalNavigation)
		{
			return true;
		}
		if ((ref method) == MethodName.OnActiveScreenUpdated)
		{
			return true;
		}
		return ((Control)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName.IsOpen)
		{
			IsOpen = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName.MerchantHand)
		{
			MerchantHand = VariantUtils.ConvertTo<NMerchantHand>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._characterCardContainer)
		{
			_characterCardContainer = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._colorlessCardContainer)
		{
			_colorlessCardContainer = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._relicContainer)
		{
			_relicContainer = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._potionContainer)
		{
			_potionContainer = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._cardRemovalNode)
		{
			_cardRemovalNode = VariantUtils.ConvertTo<NMerchantCardRemoval>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._backButton)
		{
			_backButton = VariantUtils.ConvertTo<NBackButton>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._merchantDialogue)
		{
			_merchantDialogue = VariantUtils.ConvertTo<NMerchantDialogue>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._inventoryTween)
		{
			_inventoryTween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._slotsContainer)
		{
			_slotsContainer = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._backstop)
		{
			_backstop = VariantUtils.ConvertTo<ColorRect>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._lastSlot)
		{
			_lastSlot = VariantUtils.ConvertTo<NMerchantSlot>(ref value);
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
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName.IsOpen)
		{
			bool isOpen = IsOpen;
			value = VariantUtils.CreateFrom<bool>(ref isOpen);
			return true;
		}
		if ((ref name) == PropertyName.MerchantHand)
		{
			NMerchantHand merchantHand = MerchantHand;
			value = VariantUtils.CreateFrom<NMerchantHand>(ref merchantHand);
			return true;
		}
		if ((ref name) == PropertyName.DefaultFocusedControl)
		{
			Control defaultFocusedControl = DefaultFocusedControl;
			value = VariantUtils.CreateFrom<Control>(ref defaultFocusedControl);
			return true;
		}
		if ((ref name) == PropertyName._characterCardContainer)
		{
			value = VariantUtils.CreateFrom<Control>(ref _characterCardContainer);
			return true;
		}
		if ((ref name) == PropertyName._colorlessCardContainer)
		{
			value = VariantUtils.CreateFrom<Control>(ref _colorlessCardContainer);
			return true;
		}
		if ((ref name) == PropertyName._relicContainer)
		{
			value = VariantUtils.CreateFrom<Control>(ref _relicContainer);
			return true;
		}
		if ((ref name) == PropertyName._potionContainer)
		{
			value = VariantUtils.CreateFrom<Control>(ref _potionContainer);
			return true;
		}
		if ((ref name) == PropertyName._cardRemovalNode)
		{
			value = VariantUtils.CreateFrom<NMerchantCardRemoval>(ref _cardRemovalNode);
			return true;
		}
		if ((ref name) == PropertyName._backButton)
		{
			value = VariantUtils.CreateFrom<NBackButton>(ref _backButton);
			return true;
		}
		if ((ref name) == PropertyName._merchantDialogue)
		{
			value = VariantUtils.CreateFrom<NMerchantDialogue>(ref _merchantDialogue);
			return true;
		}
		if ((ref name) == PropertyName._inventoryTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _inventoryTween);
			return true;
		}
		if ((ref name) == PropertyName._slotsContainer)
		{
			value = VariantUtils.CreateFrom<Control>(ref _slotsContainer);
			return true;
		}
		if ((ref name) == PropertyName._backstop)
		{
			value = VariantUtils.CreateFrom<ColorRect>(ref _backstop);
			return true;
		}
		if ((ref name) == PropertyName._lastSlot)
		{
			value = VariantUtils.CreateFrom<NMerchantSlot>(ref _lastSlot);
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
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._characterCardContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._colorlessCardContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._relicContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._potionContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._cardRemovalNode, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._backButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._merchantDialogue, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._inventoryTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._slotsContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._backstop, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._lastSlot, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName.IsOpen, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.MerchantHand, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.DefaultFocusedControl, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		StringName isOpen = PropertyName.IsOpen;
		bool isOpen2 = IsOpen;
		info.AddProperty(isOpen, Variant.From<bool>(ref isOpen2));
		StringName merchantHand = PropertyName.MerchantHand;
		NMerchantHand merchantHand2 = MerchantHand;
		info.AddProperty(merchantHand, Variant.From<NMerchantHand>(ref merchantHand2));
		info.AddProperty(PropertyName._characterCardContainer, Variant.From<Control>(ref _characterCardContainer));
		info.AddProperty(PropertyName._colorlessCardContainer, Variant.From<Control>(ref _colorlessCardContainer));
		info.AddProperty(PropertyName._relicContainer, Variant.From<Control>(ref _relicContainer));
		info.AddProperty(PropertyName._potionContainer, Variant.From<Control>(ref _potionContainer));
		info.AddProperty(PropertyName._cardRemovalNode, Variant.From<NMerchantCardRemoval>(ref _cardRemovalNode));
		info.AddProperty(PropertyName._backButton, Variant.From<NBackButton>(ref _backButton));
		info.AddProperty(PropertyName._merchantDialogue, Variant.From<NMerchantDialogue>(ref _merchantDialogue));
		info.AddProperty(PropertyName._inventoryTween, Variant.From<Tween>(ref _inventoryTween));
		info.AddProperty(PropertyName._slotsContainer, Variant.From<Control>(ref _slotsContainer));
		info.AddProperty(PropertyName._backstop, Variant.From<ColorRect>(ref _backstop));
		info.AddProperty(PropertyName._lastSlot, Variant.From<NMerchantSlot>(ref _lastSlot));
		info.AddSignalEventDelegate(SignalName.InventoryClosed, (Delegate)backing_InventoryClosed);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName.IsOpen, ref val))
		{
			IsOpen = ((Variant)(ref val)).As<bool>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName.MerchantHand, ref val2))
		{
			MerchantHand = ((Variant)(ref val2)).As<NMerchantHand>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._characterCardContainer, ref val3))
		{
			_characterCardContainer = ((Variant)(ref val3)).As<Control>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._colorlessCardContainer, ref val4))
		{
			_colorlessCardContainer = ((Variant)(ref val4)).As<Control>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._relicContainer, ref val5))
		{
			_relicContainer = ((Variant)(ref val5)).As<Control>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._potionContainer, ref val6))
		{
			_potionContainer = ((Variant)(ref val6)).As<Control>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName._cardRemovalNode, ref val7))
		{
			_cardRemovalNode = ((Variant)(ref val7)).As<NMerchantCardRemoval>();
		}
		Variant val8 = default(Variant);
		if (info.TryGetProperty(PropertyName._backButton, ref val8))
		{
			_backButton = ((Variant)(ref val8)).As<NBackButton>();
		}
		Variant val9 = default(Variant);
		if (info.TryGetProperty(PropertyName._merchantDialogue, ref val9))
		{
			_merchantDialogue = ((Variant)(ref val9)).As<NMerchantDialogue>();
		}
		Variant val10 = default(Variant);
		if (info.TryGetProperty(PropertyName._inventoryTween, ref val10))
		{
			_inventoryTween = ((Variant)(ref val10)).As<Tween>();
		}
		Variant val11 = default(Variant);
		if (info.TryGetProperty(PropertyName._slotsContainer, ref val11))
		{
			_slotsContainer = ((Variant)(ref val11)).As<Control>();
		}
		Variant val12 = default(Variant);
		if (info.TryGetProperty(PropertyName._backstop, ref val12))
		{
			_backstop = ((Variant)(ref val12)).As<ColorRect>();
		}
		Variant val13 = default(Variant);
		if (info.TryGetProperty(PropertyName._lastSlot, ref val13))
		{
			_lastSlot = ((Variant)(ref val13)).As<NMerchantSlot>();
		}
		InventoryClosedEventHandler inventoryClosedEventHandler = default(InventoryClosedEventHandler);
		if (info.TryGetSignalEventDelegate<InventoryClosedEventHandler>(SignalName.InventoryClosed, ref inventoryClosedEventHandler))
		{
			backing_InventoryClosed = inventoryClosedEventHandler;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotSignalList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(1);
		list.Add(new MethodInfo(SignalName.InventoryClosed, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	protected void EmitSignalInventoryClosed()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).EmitSignal(SignalName.InventoryClosed, Array.Empty<Variant>());
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RaiseGodotClassSignalCallbacks(in godot_string_name signal, NativeVariantPtrArgs args)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		if ((ref signal) == SignalName.InventoryClosed && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			backing_InventoryClosed?.Invoke();
		}
		else
		{
			((GodotObject)this).RaiseGodotClassSignalCallbacks(ref signal, args);
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassSignal(in godot_string_name signal)
	{
		if ((ref signal) == SignalName.InventoryClosed)
		{
			return true;
		}
		return ((Control)this).HasGodotClassSignal(ref signal);
	}
}
