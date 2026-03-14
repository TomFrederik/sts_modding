using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Entities.Merchant;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.HoverTips;

namespace MegaCrit.Sts2.Core.Nodes.Screens.Shops;

[ScriptPath("res://src/Core/Nodes/Screens/Shops/NMerchantCardRemoval.cs")]
public class NMerchantCardRemoval : NMerchantSlot
{
	public new class MethodName : NMerchantSlot.MethodName
	{
		public new static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public new static readonly StringName UpdateVisual = StringName.op_Implicit("UpdateVisual");

		public static readonly StringName OnCardRemovalUsed = StringName.op_Implicit("OnCardRemovalUsed");

		public new static readonly StringName CreateHoverTip = StringName.op_Implicit("CreateHoverTip");

		public new static readonly StringName _ExitTree = StringName.op_Implicit("_ExitTree");
	}

	public new class PropertyName : NMerchantSlot.PropertyName
	{
		public new static readonly StringName Visual = StringName.op_Implicit("Visual");

		public static readonly StringName _removalVisual = StringName.op_Implicit("_removalVisual");

		public static readonly StringName _animator = StringName.op_Implicit("_animator");

		public static readonly StringName _costContainer = StringName.op_Implicit("_costContainer");

		public static readonly StringName _isUnavailable = StringName.op_Implicit("_isUnavailable");
	}

	public new class SignalName : NMerchantSlot.SignalName
	{
	}

	private const string _locTable = "merchant_room";

	private Sprite2D _removalVisual;

	private AnimationPlayer _animator;

	private Control _costContainer;

	private bool _isUnavailable;

	private MerchantCardRemovalEntry _removalEntry;

	private LocString Title => new LocString("merchant_room", "MERCHANT.cardRemovalService.title");

	private LocString Description => new LocString("merchant_room", "MERCHANT.cardRemovalService.description");

	public override MerchantEntry Entry => _removalEntry;

	protected override CanvasItem Visual => (CanvasItem)(object)_removalVisual;

	public override void _Ready()
	{
		ConnectSignals();
		_removalVisual = ((Node)this).GetNode<Sprite2D>(NodePath.op_Implicit("%Visual"));
		_animator = ((Node)this).GetNode<AnimationPlayer>(NodePath.op_Implicit("%Animation"));
		_costContainer = ((Node)this).GetNode<Control>(NodePath.op_Implicit("Cost"));
	}

	public void FillSlot(MerchantCardRemovalEntry removalEntry)
	{
		_removalEntry = removalEntry;
		_removalEntry.EntryUpdated += UpdateVisual;
		_removalEntry.PurchaseFailed += base.OnPurchaseFailed;
		_removalEntry.PurchaseCompleted += OnSuccessfulPurchase;
		if (!Hook.ShouldAllowMerchantCardRemoval(base.Player.RunState, base.Player))
		{
			_removalEntry.SetUsed();
		}
		UpdateVisual();
	}

	protected override void UpdateVisual()
	{
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		base.UpdateVisual();
		if (!_isUnavailable)
		{
			if (_removalEntry.Used)
			{
				((Control)_hitbox).MouseFilter = (MouseFilterEnum)2;
				_animator.CurrentAnimation = "Used";
				_isUnavailable = true;
				_animator.Play((StringName)null, -1.0, 1f, false);
				((CanvasItem)_costLabel).Visible = false;
				((CanvasItem)_costContainer).Visible = false;
				((Control)this).FocusMode = (FocusModeEnum)0;
			}
			else
			{
				((Control)this).MouseFilter = (MouseFilterEnum)0;
				((CanvasItem)_costLabel).Visible = true;
				_costLabel.SetTextAutoSize(_removalEntry.Cost.ToString());
				((CanvasItem)_costLabel).Modulate = (_removalEntry.EnoughGold ? StsColors.cream : StsColors.red);
				((CanvasItem)_costContainer).Visible = true;
				((Control)this).FocusMode = (FocusModeEnum)2;
			}
			ClearHoverTip();
		}
	}

	protected override async Task OnTryPurchase(MerchantInventory? inventory)
	{
		await _removalEntry.OnTryPurchaseWrapper(inventory);
	}

	protected void OnSuccessfulPurchase(PurchaseStatus _, MerchantEntry __)
	{
		TriggerMerchantHandToPointHere();
		UpdateVisual();
	}

	public void OnCardRemovalUsed()
	{
		_removalEntry.SetUsed();
		UpdateVisual();
	}

	protected override void CreateHoverTip()
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		LocString title = Title;
		LocString description = Description;
		description.Add("Amount", _removalEntry.CalcPriceIncrease());
		NHoverTipSet nHoverTipSet = NHoverTipSet.CreateAndShow((Control)(object)this, new HoverTip(title, description));
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
		_removalEntry.EntryUpdated -= UpdateVisual;
		_removalEntry.PurchaseFailed -= base.OnPurchaseFailed;
		_removalEntry.PurchaseCompleted -= OnSuccessfulPurchase;
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
		list.Add(new MethodInfo(MethodName.OnCardRemovalUsed, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
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
		if ((ref method) == MethodName.OnCardRemovalUsed && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnCardRemovalUsed();
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
		if ((ref method) == MethodName.OnCardRemovalUsed)
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
		if ((ref name) == PropertyName._removalVisual)
		{
			_removalVisual = VariantUtils.ConvertTo<Sprite2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._animator)
		{
			_animator = VariantUtils.ConvertTo<AnimationPlayer>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._costContainer)
		{
			_costContainer = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._isUnavailable)
		{
			_isUnavailable = VariantUtils.ConvertTo<bool>(ref value);
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
		if ((ref name) == PropertyName._removalVisual)
		{
			value = VariantUtils.CreateFrom<Sprite2D>(ref _removalVisual);
			return true;
		}
		if ((ref name) == PropertyName._animator)
		{
			value = VariantUtils.CreateFrom<AnimationPlayer>(ref _animator);
			return true;
		}
		if ((ref name) == PropertyName._costContainer)
		{
			value = VariantUtils.CreateFrom<Control>(ref _costContainer);
			return true;
		}
		if ((ref name) == PropertyName._isUnavailable)
		{
			value = VariantUtils.CreateFrom<bool>(ref _isUnavailable);
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
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._removalVisual, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._animator, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._costContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._isUnavailable, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		info.AddProperty(PropertyName._removalVisual, Variant.From<Sprite2D>(ref _removalVisual));
		info.AddProperty(PropertyName._animator, Variant.From<AnimationPlayer>(ref _animator));
		info.AddProperty(PropertyName._costContainer, Variant.From<Control>(ref _costContainer));
		info.AddProperty(PropertyName._isUnavailable, Variant.From<bool>(ref _isUnavailable));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._removalVisual, ref val))
		{
			_removalVisual = ((Variant)(ref val)).As<Sprite2D>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._animator, ref val2))
		{
			_animator = ((Variant)(ref val2)).As<AnimationPlayer>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._costContainer, ref val3))
		{
			_costContainer = ((Variant)(ref val3)).As<Control>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._isUnavailable, ref val4))
		{
			_isUnavailable = ((Variant)(ref val4)).As<bool>();
		}
	}
}
