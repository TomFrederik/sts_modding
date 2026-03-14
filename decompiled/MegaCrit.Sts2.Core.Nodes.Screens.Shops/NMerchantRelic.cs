using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Entities.Merchant;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.HoverTips;
using MegaCrit.Sts2.Core.Nodes.Relics;

namespace MegaCrit.Sts2.Core.Nodes.Screens.Shops;

[ScriptPath("res://src/Core/Nodes/Screens/Shops/NMerchantRelic.cs")]
public class NMerchantRelic : NMerchantSlot
{
	public new class MethodName : NMerchantSlot.MethodName
	{
		public new static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public new static readonly StringName UpdateVisual = StringName.op_Implicit("UpdateVisual");

		public new static readonly StringName CreateHoverTip = StringName.op_Implicit("CreateHoverTip");

		public new static readonly StringName OnPreview = StringName.op_Implicit("OnPreview");

		public new static readonly StringName _ExitTree = StringName.op_Implicit("_ExitTree");
	}

	public new class PropertyName : NMerchantSlot.PropertyName
	{
		public new static readonly StringName Visual = StringName.op_Implicit("Visual");

		public static readonly StringName _iconSize = StringName.op_Implicit("_iconSize");

		public static readonly StringName _relicHolder = StringName.op_Implicit("_relicHolder");

		public static readonly StringName _relicNode = StringName.op_Implicit("_relicNode");

		public static readonly StringName _relicNodePosition = StringName.op_Implicit("_relicNodePosition");
	}

	public new class SignalName : NMerchantSlot.SignalName
	{
	}

	[Export(/*Could not decode attribute arguments.*/)]
	private NRelic.IconSize _iconSize;

	private Control _relicHolder;

	private NRelic? _relicNode;

	private MerchantRelicEntry _relicEntry;

	private RelicModel? _relic;

	private Vector2 _relicNodePosition;

	public override MerchantEntry Entry => _relicEntry;

	protected override CanvasItem Visual => (CanvasItem)(object)_relicHolder;

	public override void _Ready()
	{
		ConnectSignals();
		_relicHolder = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%RelicHolder"));
	}

	public void FillSlot(MerchantRelicEntry relicEntry)
	{
		_relicEntry = relicEntry;
		_relic = relicEntry.Model;
		relicEntry.EntryUpdated += UpdateVisual;
		relicEntry.PurchaseFailed += base.OnPurchaseFailed;
		relicEntry.PurchaseCompleted += OnSuccessfulPurchase;
		UpdateVisual();
	}

	protected override void UpdateVisual()
	{
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		base.UpdateVisual();
		if (_relicEntry.Model == null)
		{
			((CanvasItem)this).Visible = false;
			((Control)this).MouseFilter = (MouseFilterEnum)2;
			if (_relicNode != null)
			{
				((Node)(object)_relicNode).QueueFreeSafely();
				_relicNode = null;
			}
			ClearHoverTip();
			return;
		}
		if (_relicNode != null && _relicNode.Model != _relicEntry.Model)
		{
			((Node)(object)_relicNode).QueueFreeSafely();
			_relicNode = null;
		}
		if (_relicNode == null)
		{
			_relicNode = NRelic.Create(_relicEntry.Model, _iconSize);
			((Node)(object)_relicHolder).AddChildSafely((Node?)(object)_relicNode);
			if (_iconSize == NRelic.IconSize.Large)
			{
				((Control)_relicNode).Size = new Vector2(128f, 128f);
				TextureRect icon = _relicNode.Icon;
				((Control)icon).Position = ((Control)icon).Position - new Vector2(0f, ((Control)_costLabel).Size.Y);
			}
			((Control)base.Hitbox).Size = ((Control)_relicNode.Icon).Size;
			((Control)base.Hitbox).Scale = _relicHolder.Scale;
			((Control)base.Hitbox).GlobalPosition = ((Control)_relicNode.Icon).GlobalPosition;
		}
		_relicNodePosition = ((Control)_relicNode.Icon).GlobalPosition;
		_costLabel.SetTextAutoSize(_relicEntry.Cost.ToString());
		((CanvasItem)_costLabel).Modulate = (_relicEntry.EnoughGold ? StsColors.cream : StsColors.red);
	}

	protected override async Task OnTryPurchase(MerchantInventory? inventory)
	{
		await _relicEntry.OnTryPurchaseWrapper(inventory);
	}

	private void OnSuccessfulPurchase(PurchaseStatus _, MerchantEntry __)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		TriggerMerchantHandToPointHere();
		NRun.Instance?.GlobalUi.RelicInventory.AnimateRelic(_relic, _relicNodePosition);
		UpdateVisual();
		_relic = _relicEntry.Model;
	}

	protected override void CreateHoverTip()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		NHoverTipSet nHoverTipSet = NHoverTipSet.CreateAndShow((Control)(object)this, _relicNode.Model.HoverTips);
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
			nHoverTipSet.SetAlignment((Control)(object)this, HoverTipAlignment.Right);
			((Control)nHoverTipSet).GlobalPosition = ((Control)nHoverTipSet).GlobalPosition + (Vector2.Right * ((Control)this).Size.X * 0.5f * ((Control)this).Scale + Vector2.Up * ((Control)this).Size.Y * 0.5f * ((Control)this).Scale);
		}
	}

	protected override void OnPreview()
	{
		ClearHoverTip();
		int num = 1;
		List<RelicModel> list = new List<RelicModel>(num);
		CollectionsMarshal.SetCount(list, num);
		Span<RelicModel> span = CollectionsMarshal.AsSpan(list);
		int index = 0;
		span[index] = _relicNode.Model;
		List<RelicModel> relics = list;
		NGame.Instance.GetInspectRelicScreen().Open(relics, _relicNode.Model);
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		_relicEntry.EntryUpdated -= UpdateVisual;
		_relicEntry.PurchaseFailed -= base.OnPurchaseFailed;
		_relicEntry.PurchaseCompleted -= OnSuccessfulPurchase;
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
		list.Add(new MethodInfo(MethodName.UpdateVisual, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.CreateHoverTip, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnPreview, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._ExitTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
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
		if ((ref method) == MethodName.OnPreview && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnPreview();
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
		if ((ref method) == MethodName.OnPreview)
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
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName._iconSize)
		{
			_iconSize = VariantUtils.ConvertTo<NRelic.IconSize>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._relicHolder)
		{
			_relicHolder = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._relicNode)
		{
			_relicNode = VariantUtils.ConvertTo<NRelic>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._relicNodePosition)
		{
			_relicNodePosition = VariantUtils.ConvertTo<Vector2>(ref value);
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
		if ((ref name) == PropertyName.Visual)
		{
			CanvasItem visual = Visual;
			value = VariantUtils.CreateFrom<CanvasItem>(ref visual);
			return true;
		}
		if ((ref name) == PropertyName._iconSize)
		{
			value = VariantUtils.CreateFrom<NRelic.IconSize>(ref _iconSize);
			return true;
		}
		if ((ref name) == PropertyName._relicHolder)
		{
			value = VariantUtils.CreateFrom<Control>(ref _relicHolder);
			return true;
		}
		if ((ref name) == PropertyName._relicNode)
		{
			value = VariantUtils.CreateFrom<NRelic>(ref _relicNode);
			return true;
		}
		if ((ref name) == PropertyName._relicNodePosition)
		{
			value = VariantUtils.CreateFrom<Vector2>(ref _relicNodePosition);
			return true;
		}
		return base.GetGodotClassPropertyValue(in name, out value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)2, PropertyName._iconSize, (PropertyHint)2, "Small,Large", (PropertyUsageFlags)4102, true));
		list.Add(new PropertyInfo((Type)24, PropertyName._relicHolder, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._relicNode, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.Visual, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)5, PropertyName._relicNodePosition, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		info.AddProperty(PropertyName._iconSize, Variant.From<NRelic.IconSize>(ref _iconSize));
		info.AddProperty(PropertyName._relicHolder, Variant.From<Control>(ref _relicHolder));
		info.AddProperty(PropertyName._relicNode, Variant.From<NRelic>(ref _relicNode));
		info.AddProperty(PropertyName._relicNodePosition, Variant.From<Vector2>(ref _relicNodePosition));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		base.RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._iconSize, ref val))
		{
			_iconSize = ((Variant)(ref val)).As<NRelic.IconSize>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._relicHolder, ref val2))
		{
			_relicHolder = ((Variant)(ref val2)).As<Control>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._relicNode, ref val3))
		{
			_relicNode = ((Variant)(ref val3)).As<NRelic>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._relicNodePosition, ref val4))
		{
			_relicNodePosition = ((Variant)(ref val4)).As<Vector2>();
		}
	}
}
