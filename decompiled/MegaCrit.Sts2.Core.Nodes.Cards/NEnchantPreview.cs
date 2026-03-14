using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards.Holders;

namespace MegaCrit.Sts2.Core.Nodes.Cards;

[ScriptPath("res://src/Core/Nodes/Cards/NEnchantPreview.cs")]
public class NEnchantPreview : Control
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName RemoveExistingCards = StringName.op_Implicit("RemoveExistingCards");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName _before = StringName.op_Implicit("_before");

		public static readonly StringName _after = StringName.op_Implicit("_after");

		public static readonly StringName _arrows = StringName.op_Implicit("_arrows");
	}

	public class SignalName : SignalName
	{
	}

	private Control _before;

	private Control _after;

	private Control _arrows;

	public override void _Ready()
	{
		_before = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%Before"));
		_after = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%After"));
		_arrows = ((Node)this).GetNode<Control>(NodePath.op_Implicit("Arrows"));
	}

	public void Init(CardModel card, EnchantmentModel canonicalEnchantment, int amount)
	{
		canonicalEnchantment.AssertCanonical();
		RemoveExistingCards();
		NPreviewCardHolder nPreviewCardHolder = NPreviewCardHolder.Create(NCard.Create(card), showHoverTips: true, scaleOnHover: false);
		((Node)(object)_before).AddChildSafely((Node?)(object)nPreviewCardHolder);
		nPreviewCardHolder.CardNode.UpdateVisuals(card.Pile.Type, CardPreviewMode.Normal);
		CardModel cardModel = card.CardScope.CloneCard(card);
		EnchantmentModel enchantmentModel = canonicalEnchantment.ToMutable();
		cardModel.EnchantInternal(enchantmentModel, amount);
		cardModel.IsEnchantmentPreview = true;
		enchantmentModel.ModifyCard();
		NPreviewCardHolder nPreviewCardHolder2 = NPreviewCardHolder.Create(NCard.Create(cardModel), showHoverTips: true, scaleOnHover: false);
		((Node)(object)_after).AddChildSafely((Node?)(object)nPreviewCardHolder2);
		nPreviewCardHolder2.CardNode.UpdateVisuals(PileType.None, CardPreviewMode.Normal);
	}

	private void RemoveExistingCards()
	{
		foreach (Node child in ((Node)_before).GetChildren(false))
		{
			child.QueueFreeSafely();
		}
		foreach (Node child2 in ((Node)_after).GetChildren(false))
		{
			child2.QueueFreeSafely();
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(2);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.RemoveExistingCards, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.RemoveExistingCards && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			RemoveExistingCards();
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
		if ((ref method) == MethodName.RemoveExistingCards)
		{
			return true;
		}
		return ((Control)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._before)
		{
			_before = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._after)
		{
			_after = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._arrows)
		{
			_arrows = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		return ((GodotObject)this).SetGodotClassPropertyValue(ref name, ref value);
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
		if ((ref name) == PropertyName._before)
		{
			value = VariantUtils.CreateFrom<Control>(ref _before);
			return true;
		}
		if ((ref name) == PropertyName._after)
		{
			value = VariantUtils.CreateFrom<Control>(ref _after);
			return true;
		}
		if ((ref name) == PropertyName._arrows)
		{
			value = VariantUtils.CreateFrom<Control>(ref _arrows);
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
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._before, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._after, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._arrows, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		info.AddProperty(PropertyName._before, Variant.From<Control>(ref _before));
		info.AddProperty(PropertyName._after, Variant.From<Control>(ref _after));
		info.AddProperty(PropertyName._arrows, Variant.From<Control>(ref _arrows));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._before, ref val))
		{
			_before = ((Variant)(ref val)).As<Control>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._after, ref val2))
		{
			_after = ((Variant)(ref val2)).As<Control>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._arrows, ref val3))
		{
			_arrows = ((Variant)(ref val3)).As<Control>();
		}
	}
}
