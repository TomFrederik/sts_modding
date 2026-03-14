using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.UI;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards.Holders;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.Settings;

namespace MegaCrit.Sts2.Core.Nodes.Cards;

[ScriptPath("res://src/Core/Nodes/Cards/NTransformPreview.cs")]
public class NTransformPreview : Control
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName _ExitTree = StringName.op_Implicit("_ExitTree");

		public static readonly StringName Uninitialize = StringName.op_Implicit("Uninitialize");

		public static readonly StringName RemoveExistingCards = StringName.op_Implicit("RemoveExistingCards");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName SelectedCardPosition = StringName.op_Implicit("SelectedCardPosition");

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

	private CancellationTokenSource? _cancelTokenSource;

	public Vector2 SelectedCardPosition => _before.GlobalPosition;

	public override void _Ready()
	{
		_before = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%Before"));
		_after = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%After"));
		_arrows = ((Node)this).GetNode<Control>(NodePath.op_Implicit("Arrows"));
	}

	public override void _ExitTree()
	{
		_cancelTokenSource?.Cancel();
	}

	public void Initialize(IEnumerable<CardTransformation> cardTransformations)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		RemoveExistingCards();
		_cancelTokenSource?.Cancel();
		List<CardTransformation> list = cardTransformations.ToList();
		float num = _before.GlobalPosition.X - 100f;
		float num2 = Math.Min(num / ((float)list.Count * 300f + (float)(list.Count - 1) * 30f), 1f);
		for (int i = 0; i < list.Count; i++)
		{
			CardTransformation cardTransformation = list[i];
			NPlayerHand nPlayerHand = NCombatRoom.Instance?.Ui.Hand;
			NPreviewCardHolder nPreviewCardHolder = NPreviewCardHolder.Create(NCard.Create(cardTransformation.Original), nPlayerHand == null, nPlayerHand != null);
			((Node)(object)_before).AddChildSafely((Node?)(object)nPreviewCardHolder);
			((Control)nPreviewCardHolder).FocusMode = (FocusModeEnum)2;
			nPreviewCardHolder.CardNode.UpdateVisuals(cardTransformation.Original.Pile.Type, CardPreviewMode.Normal);
			nPreviewCardHolder.SetCardScale(Vector2.One * num2);
			int num3 = list.Count - i;
			((Control)nPreviewCardHolder).Position = new Vector2((0f - ((float)num3 - 0.5f)) * 300f * num2 - (float)(num3 - 1) * 30f, 0f);
			NCard card = ((cardTransformation.Replacement == null) ? NCard.Create(cardTransformation.Original) : NCard.Create(cardTransformation.Replacement));
			NPreviewCardHolder nPreviewCardHolder2 = NPreviewCardHolder.Create(card, showHoverTips: true, scaleOnHover: false);
			((Control)nPreviewCardHolder2).FocusMode = (FocusModeEnum)0;
			((Node)(object)_after).AddChildSafely((Node?)(object)nPreviewCardHolder2);
			nPreviewCardHolder2.CardNode.UpdateVisuals(cardTransformation.Original.Pile.Type, CardPreviewMode.Normal);
			((Control)nPreviewCardHolder2).Scale = Vector2.One * num2;
			((Control)nPreviewCardHolder2).Position = new Vector2(((float)i + 0.5f) * 300f * num2 + (float)i * 30f, 0f);
			if (cardTransformation.Replacement == null)
			{
				((Control)nPreviewCardHolder2.Hitbox).MouseFilter = (MouseFilterEnum)2;
				TaskHelper.RunSafely(CycleThroughCards(possibleTransformations: (cardTransformation.ReplacementOptions == null) ? CardFactory.GetDefaultTransformationOptions(cardTransformation.Original, cardTransformation.IsInCombat) : cardTransformation.ReplacementOptions, holder: nPreviewCardHolder2, cardPile: cardTransformation.Original.Pile));
			}
		}
	}

	public void Uninitialize()
	{
		_cancelTokenSource?.Cancel();
	}

	private async Task CycleThroughCards(NPreviewCardHolder holder, CardPile cardPile, IEnumerable<CardModel> possibleTransformations)
	{
		_cancelTokenSource = new CancellationTokenSource();
		List<CardModel> cards = possibleTransformations.ToList();
		cards.UnstableShuffle(Rng.Chaotic);
		int cardIndex = 0;
		while (!_cancelTokenSource.IsCancellationRequested)
		{
			holder.ReassignToCard(cards[cardIndex], cardPile.Type, null, ModelVisibility.Visible);
			cardIndex++;
			if (cardIndex >= cards.Count)
			{
				cards.UnstableShuffle(Rng.Chaotic);
				cardIndex = 0;
			}
			if (SaveManager.Instance.PrefsSave.FastMode == FastModeType.Instant)
			{
				await Task.Delay(200);
			}
			else
			{
				await Cmd.Wait(0.2f, ignoreCombatEnd: true);
			}
		}
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
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(4);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._ExitTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.Uninitialize, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.RemoveExistingCards, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
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
		if ((ref method) == MethodName._ExitTree && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._ExitTree();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.Uninitialize && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			Uninitialize();
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
		if ((ref method) == MethodName._ExitTree)
		{
			return true;
		}
		if ((ref method) == MethodName.Uninitialize)
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
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName.SelectedCardPosition)
		{
			Vector2 selectedCardPosition = SelectedCardPosition;
			value = VariantUtils.CreateFrom<Vector2>(ref selectedCardPosition);
			return true;
		}
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
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._before, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._after, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._arrows, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)5, PropertyName.SelectedCardPosition, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
