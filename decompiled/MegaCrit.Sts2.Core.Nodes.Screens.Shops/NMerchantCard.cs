using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Merchant;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.HoverTips;
using MegaCrit.Sts2.Core.Nodes.Vfx;

namespace MegaCrit.Sts2.Core.Nodes.Screens.Shops;

[ScriptPath("res://src/Core/Nodes/Screens/Shops/NMerchantCard.cs")]
public class NMerchantCard : NMerchantSlot
{
	public new class MethodName : NMerchantSlot.MethodName
	{
		public new static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public new static readonly StringName UpdateVisual = StringName.op_Implicit("UpdateVisual");

		public static readonly StringName OnInventoryOpened = StringName.op_Implicit("OnInventoryOpened");

		public new static readonly StringName OnPreview = StringName.op_Implicit("OnPreview");

		public new static readonly StringName CreateHoverTip = StringName.op_Implicit("CreateHoverTip");

		public new static readonly StringName _ExitTree = StringName.op_Implicit("_ExitTree");
	}

	public new class PropertyName : NMerchantSlot.PropertyName
	{
		public static readonly StringName IsShowingUpgradedCard = StringName.op_Implicit("IsShowingUpgradedCard");

		public new static readonly StringName Visual = StringName.op_Implicit("Visual");

		public static readonly StringName _saleVisual = StringName.op_Implicit("_saleVisual");

		public static readonly StringName _cardHolder = StringName.op_Implicit("_cardHolder");

		public static readonly StringName _cardNode = StringName.op_Implicit("_cardNode");

		public new static readonly StringName _hoverTween = StringName.op_Implicit("_hoverTween");
	}

	public new class SignalName : NMerchantSlot.SignalName
	{
	}

	private Node2D _saleVisual;

	private Control _cardHolder;

	private NCard? _cardNode;

	private Tween? _hoverTween;

	private MerchantCardEntry _cardEntry;

	public bool IsShowingUpgradedCard => _cardNode?.Model?.IsUpgraded == true;

	public override MerchantEntry Entry => _cardEntry;

	protected override CanvasItem Visual => (CanvasItem)(object)_cardHolder;

	public override void _Ready()
	{
		ConnectSignals();
		_cardHolder = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%CardHolder"));
		_saleVisual = ((Node)this).GetNode<Node2D>(NodePath.op_Implicit("%SaleVisual"));
	}

	public void FillSlot(MerchantCardEntry cardEntry)
	{
		_cardEntry = cardEntry;
		cardEntry.EntryUpdated += UpdateVisual;
		cardEntry.PurchaseFailed += base.OnPurchaseFailed;
		cardEntry.PurchaseCompleted += OnSuccessfulPurchase;
		UpdateVisual();
	}

	protected override void UpdateVisual()
	{
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		base.UpdateVisual();
		if (_cardEntry.CreationResult == null)
		{
			((CanvasItem)this).Visible = false;
			((Control)this).MouseFilter = (MouseFilterEnum)2;
			ClearHoverTip();
			return;
		}
		if (_cardNode != null && _cardNode.Model != _cardEntry.CreationResult.Card)
		{
			((Node)(object)_cardNode).QueueFreeSafely();
			_cardNode = null;
		}
		if (_cardNode == null)
		{
			_cardNode = NCard.Create(_cardEntry.CreationResult.Card);
			((Node)(object)_cardHolder).AddChildSafely((Node?)(object)_cardNode);
			_cardNode.UpdateVisuals(PileType.None, CardPreviewMode.Normal);
		}
		_costLabel.SetTextAutoSize(_cardEntry.Cost.ToString());
		((CanvasItem)_saleVisual).Visible = _cardEntry.IsOnSale;
		if (!_cardEntry.EnoughGold)
		{
			((CanvasItem)_costLabel).Modulate = StsColors.red;
		}
		else
		{
			((CanvasItem)_costLabel).Modulate = (_cardEntry.IsOnSale ? StsColors.green : StsColors.cream);
		}
	}

	public void OnInventoryOpened()
	{
		CardCreationResult? creationResult = _cardEntry.CreationResult;
		if (creationResult != null && creationResult.HasBeenModified)
		{
			TaskHelper.RunSafely(DoRelicFlash());
		}
	}

	private async Task DoRelicFlash()
	{
		SceneTreeTimer val = ((Node)this).GetTree().CreateTimer(0.4, true, false, false);
		await ((GodotObject)val).ToSignal((GodotObject)(object)val, SignalName.Timeout);
		foreach (RelicModel modifyingRelic in _cardEntry.CreationResult.ModifyingRelics)
		{
			modifyingRelic.Flash();
			_cardNode?.FlashRelicOnCard(modifyingRelic);
		}
	}

	protected override async Task OnTryPurchase(MerchantInventory? inventory)
	{
		await _cardEntry.OnTryPurchaseWrapper(inventory);
	}

	protected void OnSuccessfulPurchase(PurchaseStatus _, MerchantEntry __)
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		TriggerMerchantHandToPointHere();
		NRun.Instance?.GlobalUi.ReparentCard(_cardNode);
		NRun.Instance?.GlobalUi.TopBar.TrailContainer.AddChildSafely((Node?)(object)NCardFlyVfx.Create(_cardNode, PileType.Deck.GetTargetPosition(_cardNode), isAddingToPile: true, _cardNode.Model.Owner.Character.TrailPath));
		_cardNode = null;
		UpdateVisual();
	}

	protected override void OnPreview()
	{
		ClearHoverTip();
		NInspectCardScreen inspectCardScreen = NGame.Instance.GetInspectCardScreen();
		int num = 1;
		List<CardModel> list = new List<CardModel>(num);
		CollectionsMarshal.SetCount(list, num);
		Span<CardModel> span = CollectionsMarshal.AsSpan(list);
		int index = 0;
		span[index] = _cardNode.Model;
		inspectCardScreen.Open(list, 0);
	}

	protected override void CreateHoverTip()
	{
		NHoverTipSet nHoverTipSet = NHoverTipSet.CreateAndShow((Control)(object)this, _cardEntry.CreationResult.Card.HoverTips);
		nHoverTipSet.SetAlignment((Control)(object)_hitbox, HoverTip.GetHoverTipAlignment((Control)(object)this));
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		((Node)(object)_cardNode)?.QueueFreeSafely();
		_cardEntry.EntryUpdated -= UpdateVisual;
		_cardEntry.PurchaseFailed -= base.OnPurchaseFailed;
		_cardEntry.PurchaseCompleted -= OnSuccessfulPurchase;
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
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(6);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UpdateVisual, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnInventoryOpened, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnPreview, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.CreateHoverTip, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._ExitTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.UpdateVisual && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			UpdateVisual();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnInventoryOpened && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnInventoryOpened();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnPreview && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnPreview();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.CreateHoverTip && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			CreateHoverTip();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._ExitTree && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._ExitTree();
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
		if ((ref method) == MethodName.UpdateVisual)
		{
			return true;
		}
		if ((ref method) == MethodName.OnInventoryOpened)
		{
			return true;
		}
		if ((ref method) == MethodName.OnPreview)
		{
			return true;
		}
		if ((ref method) == MethodName.CreateHoverTip)
		{
			return true;
		}
		if ((ref method) == MethodName._ExitTree)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._saleVisual)
		{
			_saleVisual = VariantUtils.ConvertTo<Node2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._cardHolder)
		{
			_cardHolder = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._cardNode)
		{
			_cardNode = VariantUtils.ConvertTo<NCard>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._hoverTween)
		{
			_hoverTween = VariantUtils.ConvertTo<Tween>(ref value);
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
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName.IsShowingUpgradedCard)
		{
			bool isShowingUpgradedCard = IsShowingUpgradedCard;
			value = VariantUtils.CreateFrom<bool>(ref isShowingUpgradedCard);
			return true;
		}
		if ((ref name) == PropertyName.Visual)
		{
			CanvasItem visual = Visual;
			value = VariantUtils.CreateFrom<CanvasItem>(ref visual);
			return true;
		}
		if ((ref name) == PropertyName._saleVisual)
		{
			value = VariantUtils.CreateFrom<Node2D>(ref _saleVisual);
			return true;
		}
		if ((ref name) == PropertyName._cardHolder)
		{
			value = VariantUtils.CreateFrom<Control>(ref _cardHolder);
			return true;
		}
		if ((ref name) == PropertyName._cardNode)
		{
			value = VariantUtils.CreateFrom<NCard>(ref _cardNode);
			return true;
		}
		if ((ref name) == PropertyName._hoverTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _hoverTween);
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
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._saleVisual, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._cardHolder, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._cardNode, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._hoverTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName.IsShowingUpgradedCard, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.Visual, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		info.AddProperty(PropertyName._saleVisual, Variant.From<Node2D>(ref _saleVisual));
		info.AddProperty(PropertyName._cardHolder, Variant.From<Control>(ref _cardHolder));
		info.AddProperty(PropertyName._cardNode, Variant.From<NCard>(ref _cardNode));
		info.AddProperty(PropertyName._hoverTween, Variant.From<Tween>(ref _hoverTween));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._saleVisual, ref val))
		{
			_saleVisual = ((Variant)(ref val)).As<Node2D>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._cardHolder, ref val2))
		{
			_cardHolder = ((Variant)(ref val2)).As<Control>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._cardNode, ref val3))
		{
			_cardNode = ((Variant)(ref val3)).As<NCard>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._hoverTween, ref val4))
		{
			_hoverTween = ((Variant)(ref val4)).As<Tween>();
		}
	}
}
