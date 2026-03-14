using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.Cards.Holders;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.Ftue;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Saves;

namespace MegaCrit.Sts2.Core.Nodes.Combat;

[ScriptPath("res://src/Core/Nodes/Combat/NCardPlay.cs")]
public abstract class NCardPlay : Node
{
	[Signal]
	public delegate void FinishedEventHandler(bool success);

	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName Start = StringName.op_Implicit("Start");

		public static readonly StringName CancelPlayCard = StringName.op_Implicit("CancelPlayCard");

		public static readonly StringName OnCancelPlayCard = StringName.op_Implicit("OnCancelPlayCard");

		public static readonly StringName Cleanup = StringName.op_Implicit("Cleanup");

		public static readonly StringName OnCreatureHover = StringName.op_Implicit("OnCreatureHover");

		public static readonly StringName OnCreatureUnhover = StringName.op_Implicit("OnCreatureUnhover");

		public static readonly StringName CenterCard = StringName.op_Implicit("CenterCard");

		public static readonly StringName HideTargetingVisuals = StringName.op_Implicit("HideTargetingVisuals");

		public static readonly StringName ShowMultiCreatureTargetingVisuals = StringName.op_Implicit("ShowMultiCreatureTargetingVisuals");

		public static readonly StringName AutoDisableCannotPlayCardFtueCheck = StringName.op_Implicit("AutoDisableCannotPlayCardFtueCheck");

		public static readonly StringName TryShowEvokingOrbs = StringName.op_Implicit("TryShowEvokingOrbs");

		public static readonly StringName HideEvokingOrbs = StringName.op_Implicit("HideEvokingOrbs");

		public static readonly StringName ClearTarget = StringName.op_Implicit("ClearTarget");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName Holder = StringName.op_Implicit("Holder");

		public static readonly StringName CardNode = StringName.op_Implicit("CardNode");

		public static readonly StringName CardOwnerNode = StringName.op_Implicit("CardOwnerNode");

		public static readonly StringName _viewport = StringName.op_Implicit("_viewport");

		public static readonly StringName _isTryingToPlayCard = StringName.op_Implicit("_isTryingToPlayCard");
	}

	public class SignalName : SignalName
	{
		public static readonly StringName Finished = StringName.op_Implicit("Finished");
	}

	private static int _totalCardsPlayedForFtue;

	private const int _numCardPlayedUntilDisableFtue = 8;

	protected Viewport _viewport;

	private bool _isTryingToPlayCard;

	private FinishedEventHandler backing_Finished;

	public NHandCardHolder Holder { get; protected set; }

	protected NCard? CardNode => Holder.CardNode;

	protected CardModel? Card => CardNode?.Model;

	protected NCreature? CardOwnerNode => NCombatRoom.Instance.GetCreatureNode(Card?.Owner.Creature);

	public Player Player { get; protected set; }

	public event FinishedEventHandler Finished
	{
		add
		{
			backing_Finished = (FinishedEventHandler)Delegate.Combine(backing_Finished, value);
		}
		remove
		{
			backing_Finished = (FinishedEventHandler)Delegate.Remove(backing_Finished, value);
		}
	}

	public override void _Ready()
	{
		_viewport = ((Node)this).GetViewport();
	}

	public abstract void Start();

	protected void TryPlayCard(Creature? target)
	{
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		if (Card == null)
		{
			return;
		}
		if (Card.TargetType == TargetType.AnyEnemy && target == null)
		{
			CancelPlayCard();
			return;
		}
		if (Card.TargetType == TargetType.AnyAlly && target == null)
		{
			CancelPlayCard();
			return;
		}
		if (!Holder.CardModel.CanPlayTargeting(target))
		{
			CannotPlayThisCardFtueCheck(Holder.CardModel);
			CancelPlayCard();
			return;
		}
		CardModel card = Card;
		_isTryingToPlayCard = true;
		TargetType targetType = card.TargetType;
		bool flag = ((targetType == TargetType.AnyEnemy || targetType == TargetType.AnyAlly) ? true : false);
		bool flag2 = ((!flag) ? card.TryManualPlay(null) : card.TryManualPlay(target));
		_isTryingToPlayCard = false;
		if (flag2)
		{
			AutoDisableCannotPlayCardFtueCheck();
			targetType = card.TargetType;
			flag = ((targetType == TargetType.AnyEnemy || targetType == TargetType.AnyAlly) ? true : false);
			if (flag && ((Node)Holder).IsInsideTree())
			{
				Rect2 visibleRect = ((Node)this).GetViewport().GetVisibleRect();
				Vector2 size = ((Rect2)(ref visibleRect)).Size;
				Holder.SetTargetPosition(new Vector2(size.X / 2f, size.Y - ((Control)Holder).Size.Y));
			}
			Cleanup();
			((GodotObject)this).EmitSignal(SignalName.Finished, (Variant[])(object)new Variant[1] { Variant.op_Implicit(true) });
			((Control)(object)NCombatRoom.Instance?.Ui.Hand).TryGrabFocus();
		}
		else
		{
			CancelPlayCard();
		}
	}

	public void CancelPlayCard()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		if (!_isTryingToPlayCard)
		{
			ClearTarget();
			Cleanup();
			((GodotObject)this).EmitSignal(SignalName.Finished, (Variant[])(object)new Variant[1] { Variant.op_Implicit(false) });
			OnCancelPlayCard();
		}
	}

	protected virtual void OnCancelPlayCard()
	{
	}

	protected virtual void Cleanup()
	{
		HideTargetingVisuals();
		HideEvokingOrbs();
		((Node)(object)this).QueueFreeSafely();
	}

	protected void OnCreatureHover(NCreature creature)
	{
		CardNode?.SetPreviewTarget(creature.Entity);
	}

	protected void OnCreatureUnhover(NCreature _)
	{
		ClearTarget();
	}

	protected void CenterCard()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		Rect2 visibleRect = _viewport.GetVisibleRect();
		Vector2 size = ((Rect2)(ref visibleRect)).Size;
		Holder.SetTargetPosition(new Vector2(size.X / 2f, size.Y - ((Control)Holder.Hitbox).Size.Y * 0.75f / 2f));
		Holder.SetTargetScale(Vector2.One * 0.75f);
	}

	protected void CannotPlayThisCardFtueCheck(CardModel card)
	{
		if (!SaveManager.Instance.SeenFtue("cannot_play_card_ftue") && !card.CanPlay(out UnplayableReason reason, out AbstractModel _) && reason == UnplayableReason.EnergyCostTooHigh)
		{
			NModalContainer.Instance.Add((Node)(object)NCannotPlayCardFtue.Create());
			SaveManager.Instance.MarkFtueAsComplete("cannot_play_card_ftue");
		}
	}

	protected void HideTargetingVisuals()
	{
		foreach (NCreature creatureNode in NCombatRoom.Instance.CreatureNodes)
		{
			creatureNode.HideMultiselectReticle();
		}
		CardNode?.SetPreviewTarget(null);
		CardNode?.UpdateVisuals((Card?.Pile?.Type).GetValueOrDefault(), CardPreviewMode.Normal);
	}

	protected void ShowMultiCreatureTargetingVisuals()
	{
		if (CardNode == null || Card?.CombatState == null)
		{
			return;
		}
		TargetType targetType = Card.TargetType;
		if ((uint)(targetType - 3) <= 1u)
		{
			IReadOnlyList<Creature> hittableEnemies = Card.CombatState.HittableEnemies;
			if (hittableEnemies.Count == 1)
			{
				CardNode.SetPreviewTarget(hittableEnemies[0]);
			}
			CardNode.UpdateVisuals((CardNode.Model?.Pile?.Type).GetValueOrDefault(), CardPreviewMode.MultiCreatureTargeting);
			foreach (Creature item in hittableEnemies)
			{
				NCombatRoom.Instance.GetCreatureNode(item)?.ShowMultiselectReticle();
			}
		}
		if (Card.TargetType == TargetType.AllAllies)
		{
			IEnumerable<Creature> enumerable = Card.CombatState.PlayerCreatures.Where((Creature c) => c.IsAlive);
			{
				foreach (Creature item2 in enumerable)
				{
					NCombatRoom.Instance.GetCreatureNode(item2)?.ShowMultiselectReticle();
				}
				return;
			}
		}
		if (Card.TargetType == TargetType.Self)
		{
			NCombatRoom.Instance.GetCreatureNode(Card.Owner.Creature)?.ShowMultiselectReticle();
		}
		else if (Card.TargetType == TargetType.Osty)
		{
			NCombatRoom.Instance.GetCreatureNode(Card.Owner.Osty)?.ShowMultiselectReticle();
		}
	}

	private void AutoDisableCannotPlayCardFtueCheck()
	{
		_totalCardsPlayedForFtue++;
		if (_totalCardsPlayedForFtue == 8 && !SaveManager.Instance.SeenFtue("cannot_play_card_ftue"))
		{
			Log.Info("Cannot play cards FTUE was disabled, the player never saw it!!");
			SaveManager.Instance.MarkFtueAsComplete("cannot_play_card_ftue");
		}
	}

	protected void TryShowEvokingOrbs()
	{
		if (Card != null)
		{
			CardOwnerNode?.OrbManager?.UpdateVisuals(Card.OrbEvokeType);
		}
	}

	private void HideEvokingOrbs()
	{
		CardOwnerNode?.OrbManager?.UpdateVisuals(OrbEvokeType.None);
	}

	private void ClearTarget()
	{
		CardNode?.SetPreviewTarget(null);
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
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Expected O, but got Unknown
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_0289: Unknown result type (might be due to invalid IL or missing references)
		//IL_0292: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(14);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.Start, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.CancelPlayCard, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnCancelPlayCard, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.Cleanup, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnCreatureHover, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("creature"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnCreatureUnhover, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.CenterCard, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.HideTargetingVisuals, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ShowMultiCreatureTargetingVisuals, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AutoDisableCannotPlayCardFtueCheck, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.TryShowEvokingOrbs, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.HideEvokingOrbs, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ClearTarget, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
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
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.Start && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			Start();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.CancelPlayCard && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			CancelPlayCard();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnCancelPlayCard && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnCancelPlayCard();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.Cleanup && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			Cleanup();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnCreatureHover && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OnCreatureHover(VariantUtils.ConvertTo<NCreature>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnCreatureUnhover && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OnCreatureUnhover(VariantUtils.ConvertTo<NCreature>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.CenterCard && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			CenterCard();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.HideTargetingVisuals && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			HideTargetingVisuals();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.ShowMultiCreatureTargetingVisuals && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			ShowMultiCreatureTargetingVisuals();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.AutoDisableCannotPlayCardFtueCheck && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			AutoDisableCannotPlayCardFtueCheck();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.TryShowEvokingOrbs && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			TryShowEvokingOrbs();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.HideEvokingOrbs && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			HideEvokingOrbs();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.ClearTarget && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			ClearTarget();
			ret = default(godot_variant);
			return true;
		}
		return ((Node)this).InvokeGodotClassMethod(ref method, args, ref ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if ((ref method) == MethodName._Ready)
		{
			return true;
		}
		if ((ref method) == MethodName.Start)
		{
			return true;
		}
		if ((ref method) == MethodName.CancelPlayCard)
		{
			return true;
		}
		if ((ref method) == MethodName.OnCancelPlayCard)
		{
			return true;
		}
		if ((ref method) == MethodName.Cleanup)
		{
			return true;
		}
		if ((ref method) == MethodName.OnCreatureHover)
		{
			return true;
		}
		if ((ref method) == MethodName.OnCreatureUnhover)
		{
			return true;
		}
		if ((ref method) == MethodName.CenterCard)
		{
			return true;
		}
		if ((ref method) == MethodName.HideTargetingVisuals)
		{
			return true;
		}
		if ((ref method) == MethodName.ShowMultiCreatureTargetingVisuals)
		{
			return true;
		}
		if ((ref method) == MethodName.AutoDisableCannotPlayCardFtueCheck)
		{
			return true;
		}
		if ((ref method) == MethodName.TryShowEvokingOrbs)
		{
			return true;
		}
		if ((ref method) == MethodName.HideEvokingOrbs)
		{
			return true;
		}
		if ((ref method) == MethodName.ClearTarget)
		{
			return true;
		}
		return ((Node)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName.Holder)
		{
			Holder = VariantUtils.ConvertTo<NHandCardHolder>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._viewport)
		{
			_viewport = VariantUtils.ConvertTo<Viewport>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._isTryingToPlayCard)
		{
			_isTryingToPlayCard = VariantUtils.ConvertTo<bool>(ref value);
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
		if ((ref name) == PropertyName.Holder)
		{
			NHandCardHolder holder = Holder;
			value = VariantUtils.CreateFrom<NHandCardHolder>(ref holder);
			return true;
		}
		if ((ref name) == PropertyName.CardNode)
		{
			NCard cardNode = CardNode;
			value = VariantUtils.CreateFrom<NCard>(ref cardNode);
			return true;
		}
		if ((ref name) == PropertyName.CardOwnerNode)
		{
			NCreature cardOwnerNode = CardOwnerNode;
			value = VariantUtils.CreateFrom<NCreature>(ref cardOwnerNode);
			return true;
		}
		if ((ref name) == PropertyName._viewport)
		{
			value = VariantUtils.CreateFrom<Viewport>(ref _viewport);
			return true;
		}
		if ((ref name) == PropertyName._isTryingToPlayCard)
		{
			value = VariantUtils.CreateFrom<bool>(ref _isTryingToPlayCard);
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
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName.Holder, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.CardNode, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.CardOwnerNode, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._viewport, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._isTryingToPlayCard, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		StringName holder = PropertyName.Holder;
		NHandCardHolder holder2 = Holder;
		info.AddProperty(holder, Variant.From<NHandCardHolder>(ref holder2));
		info.AddProperty(PropertyName._viewport, Variant.From<Viewport>(ref _viewport));
		info.AddProperty(PropertyName._isTryingToPlayCard, Variant.From<bool>(ref _isTryingToPlayCard));
		info.AddSignalEventDelegate(SignalName.Finished, (Delegate)backing_Finished);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName.Holder, ref val))
		{
			Holder = ((Variant)(ref val)).As<NHandCardHolder>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._viewport, ref val2))
		{
			_viewport = ((Variant)(ref val2)).As<Viewport>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._isTryingToPlayCard, ref val3))
		{
			_isTryingToPlayCard = ((Variant)(ref val3)).As<bool>();
		}
		FinishedEventHandler finishedEventHandler = default(FinishedEventHandler);
		if (info.TryGetSignalEventDelegate<FinishedEventHandler>(SignalName.Finished, ref finishedEventHandler))
		{
			backing_Finished = finishedEventHandler;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotSignalList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(1);
		list.Add(new MethodInfo(SignalName.Finished, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)1, StringName.op_Implicit("success"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		return list;
	}

	protected void EmitSignalFinished(bool success)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).EmitSignal(SignalName.Finished, (Variant[])(object)new Variant[1] { Variant.op_Implicit(success) });
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RaiseGodotClassSignalCallbacks(in godot_string_name signal, NativeVariantPtrArgs args)
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		if ((ref signal) == SignalName.Finished && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			backing_Finished?.Invoke(VariantUtils.ConvertTo<bool>(ref ((NativeVariantPtrArgs)(ref args))[0]));
		}
		else
		{
			((GodotObject)this).RaiseGodotClassSignalCallbacks(ref signal, args);
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassSignal(in godot_string_name signal)
	{
		if ((ref signal) == SignalName.Finished)
		{
			return true;
		}
		return ((Node)this).HasGodotClassSignal(ref signal);
	}
}
