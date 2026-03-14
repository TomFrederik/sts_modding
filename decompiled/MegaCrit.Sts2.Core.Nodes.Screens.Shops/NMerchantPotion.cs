using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Entities.Merchant;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.HoverTips;
using MegaCrit.Sts2.Core.Nodes.Potions;

namespace MegaCrit.Sts2.Core.Nodes.Screens.Shops;

[ScriptPath("res://src/Core/Nodes/Screens/Shops/NMerchantPotion.cs")]
public class NMerchantPotion : NMerchantSlot
{
	public new class MethodName : NMerchantSlot.MethodName
	{
		public new static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public new static readonly StringName UpdateVisual = StringName.op_Implicit("UpdateVisual");

		public new static readonly StringName CreateHoverTip = StringName.op_Implicit("CreateHoverTip");

		public new static readonly StringName _ExitTree = StringName.op_Implicit("_ExitTree");
	}

	public new class PropertyName : NMerchantSlot.PropertyName
	{
		public new static readonly StringName Visual = StringName.op_Implicit("Visual");

		public static readonly StringName _potionHolder = StringName.op_Implicit("_potionHolder");

		public static readonly StringName _potionNode = StringName.op_Implicit("_potionNode");

		public static readonly StringName _potionNodePosition = StringName.op_Implicit("_potionNodePosition");
	}

	public new class SignalName : NMerchantSlot.SignalName
	{
	}

	private Control _potionHolder;

	private NPotion? _potionNode;

	private MerchantPotionEntry _potionEntry;

	private PotionModel? _potion;

	private Vector2 _potionNodePosition;

	public override MerchantEntry Entry => _potionEntry;

	protected override CanvasItem Visual => (CanvasItem)(object)_potionHolder;

	public override void _Ready()
	{
		ConnectSignals();
		_potionHolder = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%PotionHolder"));
	}

	public void FillSlot(MerchantPotionEntry potionEntry)
	{
		_potionEntry = potionEntry;
		_potion = potionEntry.Model;
		_potionEntry.EntryUpdated += UpdateVisual;
		_potionEntry.PurchaseFailed += base.OnPurchaseFailed;
		_potionEntry.PurchaseCompleted += OnSuccessfulPurchase;
		UpdateVisual();
	}

	protected override void UpdateVisual()
	{
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		base.UpdateVisual();
		if (_potionEntry.Model == null)
		{
			((CanvasItem)this).Visible = false;
			((Control)this).MouseFilter = (MouseFilterEnum)2;
			if (_potionNode != null)
			{
				((Node)(object)_potionNode).QueueFreeSafely();
				_potionNode = null;
			}
			ClearHoverTip();
			return;
		}
		if (_potionNode != null && _potionNode.Model != _potionEntry.Model)
		{
			((Node)(object)_potionNode).QueueFreeSafely();
			_potionNode = null;
		}
		if (_potionNode == null)
		{
			_potionNode = NPotion.Create(_potionEntry.Model);
			((Node)(object)_potionHolder).AddChildSafely((Node?)(object)_potionNode);
			((Control)_potionNode).Position = Vector2.Zero;
		}
		_potionNodePosition = ((Control)_potionNode).GlobalPosition;
		_costLabel.SetTextAutoSize(_potionEntry.Cost.ToString());
		((CanvasItem)_costLabel).Modulate = (_potionEntry.EnoughGold ? StsColors.cream : StsColors.red);
	}

	protected override async Task OnTryPurchase(MerchantInventory? inventory)
	{
		await _potionEntry.OnTryPurchaseWrapper(inventory);
	}

	protected void OnSuccessfulPurchase(PurchaseStatus _, MerchantEntry __)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		TriggerMerchantHandToPointHere();
		NRun.Instance?.GlobalUi.TopBar.PotionContainer.AnimatePotion(_potion, _potionNodePosition);
		UpdateVisual();
		_potion = _potionEntry.Model;
	}

	protected override void CreateHoverTip()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		NHoverTipSet nHoverTipSet = NHoverTipSet.CreateAndShow((Control)(object)this, _potionNode.Model.HoverTips);
		((Control)nHoverTipSet).GlobalPosition = ((Control)this).GlobalPosition;
		float x = ((Control)this).GlobalPosition.X;
		Rect2 visibleRect = ((Node)this).GetViewport().GetVisibleRect();
		if (x > ((Rect2)(ref visibleRect)).Size.X * 0.5f)
		{
			nHoverTipSet.SetAlignment((Control)(object)this, HoverTipAlignment.Left);
			((Control)nHoverTipSet).GlobalPosition = ((Control)nHoverTipSet).GlobalPosition - ((Control)this).Size * 0.5f * ((Control)this).Scale;
		}
		else
		{
			((Control)nHoverTipSet).GlobalPosition = ((Control)nHoverTipSet).GlobalPosition + (Vector2.Right * ((Control)this).Size.X * 0.5f * ((Control)this).Scale + Vector2.Up * ((Control)this).Size.Y * 0.5f * ((Control)this).Scale);
		}
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		_potionEntry.EntryUpdated -= UpdateVisual;
		_potionEntry.PurchaseFailed -= base.OnPurchaseFailed;
		_potionEntry.PurchaseCompleted -= OnSuccessfulPurchase;
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
		list.Add(new MethodInfo(MethodName.UpdateVisual, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.CreateHoverTip, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._ExitTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
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
		if ((ref method) == MethodName.UpdateVisual && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			UpdateVisual();
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
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName._potionHolder)
		{
			_potionHolder = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._potionNode)
		{
			_potionNode = VariantUtils.ConvertTo<NPotion>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._potionNodePosition)
		{
			_potionNodePosition = VariantUtils.ConvertTo<Vector2>(ref value);
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
		if ((ref name) == PropertyName.Visual)
		{
			CanvasItem visual = Visual;
			value = VariantUtils.CreateFrom<CanvasItem>(ref visual);
			return true;
		}
		if ((ref name) == PropertyName._potionHolder)
		{
			value = VariantUtils.CreateFrom<Control>(ref _potionHolder);
			return true;
		}
		if ((ref name) == PropertyName._potionNode)
		{
			value = VariantUtils.CreateFrom<NPotion>(ref _potionNode);
			return true;
		}
		if ((ref name) == PropertyName._potionNodePosition)
		{
			value = VariantUtils.CreateFrom<Vector2>(ref _potionNodePosition);
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
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._potionHolder, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._potionNode, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.Visual, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)5, PropertyName._potionNodePosition, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName._potionHolder, Variant.From<Control>(ref _potionHolder));
		info.AddProperty(PropertyName._potionNode, Variant.From<NPotion>(ref _potionNode));
		info.AddProperty(PropertyName._potionNodePosition, Variant.From<Vector2>(ref _potionNodePosition));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		base.RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._potionHolder, ref val))
		{
			_potionHolder = ((Variant)(ref val)).As<Control>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._potionNode, ref val2))
		{
			_potionNode = ((Variant)(ref val2)).As<NPotion>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._potionNodePosition, ref val3))
		{
			_potionNodePosition = ((Variant)(ref val3)).As<Vector2>();
		}
	}
}
