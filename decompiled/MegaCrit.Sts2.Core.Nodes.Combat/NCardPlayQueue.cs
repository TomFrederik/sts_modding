using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Actions;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Exceptions;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.Cards.Holders;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Multiplayer;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Runs;

namespace MegaCrit.Sts2.Core.Nodes.Combat;

[ScriptPath("res://src/Core/Nodes/Combat/NCardPlayQueue.cs")]
public class NCardPlayQueue : Control
{
	private class QueueItem
	{
		public required NCard card;

		public required GameAction action;

		public Tween? currentTween;
	}

	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName _ExitTree = StringName.op_Implicit("_ExitTree");

		public static readonly StringName RemoveCardFromQueueForCancellation = StringName.op_Implicit("RemoveCardFromQueueForCancellation");

		public static readonly StringName RemoveCardFromQueue = StringName.op_Implicit("RemoveCardFromQueue");

		public static readonly StringName TweenAllToQueuePosition = StringName.op_Implicit("TweenAllToQueuePosition");

		public static readonly StringName AnimOut = StringName.op_Implicit("AnimOut");

		public static readonly StringName GetScaleForQueueIndex = StringName.op_Implicit("GetScaleForQueueIndex");

		public static readonly StringName GetPositionForQueueIndex = StringName.op_Implicit("GetPositionForQueueIndex");
	}

	public class PropertyName : PropertyName
	{
	}

	public class SignalName : SignalName
	{
	}

	private List<QueueItem> _playQueue = new List<QueueItem>();

	public static NCardPlayQueue? Instance => NCombatRoom.Instance?.Ui.PlayQueue;

	public override void _Ready()
	{
		RunManager.Instance.ActionQueueSet.ActionEnqueued += OnActionEnqueued;
	}

	public override void _ExitTree()
	{
		RunManager.Instance.ActionQueueSet.ActionEnqueued -= OnActionEnqueued;
		_playQueue.Clear();
	}

	public void OnLocalCardPlayed(PlayCardAction action, NCardHolder? holder, CardModel card)
	{
		NCard nCard = holder?.CardNode ?? NCard.Create(card);
		CardModel? model = nCard.Model;
		if (model != null && model.Pile?.Type == PileType.Hand)
		{
			QueueItem item = new QueueItem
			{
				card = nCard,
				action = action
			};
			if (((Node)nCard).IsInsideTree())
			{
				((Node)nCard).Reparent((Node)(object)this, true);
			}
			else
			{
				((Node)(object)this).AddChildSafely((Node?)(object)nCard);
			}
			((Node)this).MoveChild((Node)(object)nCard, 0);
			if (holder != null && ((Node?)(object)holder).IsValid())
			{
				NPlayerHand.Instance.RemoveCardHolder(holder);
			}
			_playQueue.Add(item);
			TweenCardToQueuePosition(item, _playQueue.Count - 1);
		}
	}

	private void OnActionEnqueued(GameAction action)
	{
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		if (!(action is PlayCardAction { NetCombatCard: var netCombatCard } playCardAction))
		{
			return;
		}
		CardModel cardModel = netCombatCard.ToCardModelOrNull();
		if (cardModel == null)
		{
			try
			{
				cardModel = ModelDb.GetById<CardModel>(playCardAction.CardModelId);
			}
			catch (ModelNotFoundException)
			{
				cardModel = ModelDb.Card<DeprecatedCard>();
			}
		}
		if (LocalContext.IsMe(playCardAction.Player))
		{
			NCardHolder cardHolder = NPlayerHand.Instance.GetCardHolder(cardModel);
			OnLocalCardPlayed(playCardAction, cardHolder, cardModel);
			return;
		}
		NCreature creatureNode = NCombatRoom.Instance.GetCreatureNode(playCardAction.Player.Creature);
		NMultiplayerPlayerIntentHandler playerIntentHandler = creatureNode.PlayerIntentHandler;
		NCard nCard = NCard.Create(cardModel);
		Vector2 globalPosition = ((Control)playerIntentHandler.CardIntent).GlobalPosition + ((Control)playerIntentHandler.CardIntent).Size * 0.5f;
		((Control)nCard).GlobalPosition = globalPosition;
		((Control)nCard).Scale = Vector2.One * 0.25f;
		((Node)(object)this).AddChildSafely((Node?)(object)nCard);
		((Node)this).MoveChild((Node)(object)nCard, 0);
		QueueItem item = new QueueItem
		{
			card = nCard,
			action = playCardAction
		};
		_playQueue.Add(item);
		UpdateCardVisuals(item);
		TweenCardToQueuePosition(item, _playQueue.Count - 1);
	}

	public void ReAddCardAfterPlayerChoice(NCard card, GameAction action)
	{
		if (action.State == GameActionState.Executing)
		{
			((Node)card).Reparent((Node)(object)NCombatRoom.Instance.Ui.PlayContainer, true);
			card.AnimCardToPlayPile();
			return;
		}
		QueueItem item = new QueueItem
		{
			card = card,
			action = action
		};
		((Node)card).Reparent((Node)(object)this, true);
		((Node)this).MoveChild((Node)(object)card, 0);
		_playQueue.Add(item);
		TweenCardToQueuePosition(item, _playQueue.Count - 1);
		action.BeforeResumedAfterPlayerChoice += BeforeRemoteCardPlayResumedAfterPlayerChoice;
	}

	private void BeforeRemoteCardPlayResumedAfterPlayerChoice(GameAction action)
	{
		action.BeforeResumedAfterPlayerChoice -= BeforeRemoteCardPlayResumedAfterPlayerChoice;
		int num = _playQueue.FindIndex((QueueItem i) => i.action == action);
		if (num >= 0)
		{
			QueueItem queueItem = _playQueue[num];
			RemoveCardFromQueue(num);
			((Node)queueItem.card).Reparent((Node)(object)NCombatRoom.Instance.Ui.PlayContainer, true);
			queueItem.card.AnimCardToPlayPile();
		}
	}

	public void RemoveCardFromQueueForCancellation(PlayCardAction action)
	{
		int num = _playQueue.FindIndex((QueueItem i) => i.action == action);
		if (num >= 0)
		{
			RemoveCardFromQueueForCancellation(num);
		}
	}

	public void RemoveCardFromQueueForCancellation(NCard card, bool forceReturnToHand = false)
	{
		int num = _playQueue.FindIndex((QueueItem i) => i.card == card);
		if (num >= 0)
		{
			RemoveCardFromQueueForCancellation(num, forceReturnToHand);
		}
	}

	private void RemoveCardFromQueueForCancellation(int index, bool forceReturnToHand = false)
	{
		QueueItem queueItem = _playQueue[index];
		RemoveCardFromQueue(index);
		if (queueItem.action.OwnerId == LocalContext.NetId)
		{
			CardModel? model = queueItem.card.Model;
			if ((model != null && model.Pile?.Type == PileType.Hand) || forceReturnToHand)
			{
				NPlayerHand.Instance.Add(queueItem.card);
			}
			else
			{
				TweenCardForCancellation(queueItem);
			}
		}
		else
		{
			TweenCardForCancellation(queueItem);
		}
	}

	public void UpdateCardBeforeExecution(PlayCardAction playCardAction)
	{
		int num = _playQueue.FindIndex((QueueItem i) => i.action == playCardAction);
		if (num < 0)
		{
			return;
		}
		QueueItem queueItem = _playQueue[num];
		queueItem.card.Model = playCardAction.NetCombatCard.ToCardModel();
		UpdateCardVisuals(queueItem);
		if (LocalContext.IsMe(queueItem.card.Model.Owner))
		{
			NCardHolder nCardHolder = NPlayerHand.Instance?.GetCardHolder(queueItem.card.Model);
			if (nCardHolder != null)
			{
				NPlayerHand.Instance?.Remove(queueItem.card.Model);
			}
		}
	}

	public void RemoveCardFromQueueForExecution(CardModel card)
	{
		int num = _playQueue.FindIndex((QueueItem i) => i.card.Model == card);
		if (num < 0)
		{
			throw new InvalidOperationException();
		}
		RemoveCardFromQueue(num);
	}

	private void UpdateCardVisuals(QueueItem item)
	{
		if (item.action is PlayCardAction playCardAction)
		{
			item.card.SetPreviewTarget(playCardAction.Target);
		}
		item.card.UpdateVisuals(item.card.Model.Pile?.Type ?? PileType.None, CardPreviewMode.Normal);
	}

	private void RemoveCardFromQueue(NCard card)
	{
		int num = _playQueue.FindIndex((QueueItem i) => i.card == card);
		if (num >= 0)
		{
			RemoveCardFromQueue(num);
		}
	}

	private void RemoveCardFromQueue(int index)
	{
		QueueItem queueItem = _playQueue[index];
		Tween? currentTween = queueItem.currentTween;
		if (currentTween != null)
		{
			currentTween.Kill();
		}
		_playQueue.RemoveAt(index);
		TweenAllToQueuePosition();
	}

	private void TweenAllToQueuePosition()
	{
		for (int i = 0; i < _playQueue.Count; i++)
		{
			TweenCardToQueuePosition(_playQueue[i], i);
		}
	}

	public NCard? GetCardNode(CardModel card)
	{
		return _playQueue.FirstOrDefault((QueueItem i) => i.card.Model == card)?.card;
	}

	public void AnimOut()
	{
		foreach (QueueItem item in _playQueue)
		{
			Tween? currentTween = item.currentTween;
			if (currentTween != null)
			{
				currentTween.Kill();
			}
			if (item.action.OwnerId == LocalContext.NetId)
			{
				CardModel? model = item.card.Model;
				if (model != null && model.Pile?.Type == PileType.Hand)
				{
					NPlayerHand.Instance.Add(item.card);
					continue;
				}
			}
			TweenCardForCancellation(item);
		}
		_playQueue.Clear();
	}

	private void TweenCardForCancellation(QueueItem item)
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		Tween? currentTween = item.currentTween;
		if (currentTween != null)
		{
			currentTween.Kill();
		}
		item.currentTween = ((Node)this).CreateTween().SetParallel(true);
		item.currentTween.TweenProperty((GodotObject)(object)item.card, NodePath.op_Implicit("position:y"), Variant.op_Implicit(30f), 0.5).AsRelative().SetEase((EaseType)1)
			.SetTrans((TransitionType)7);
		item.currentTween.TweenProperty((GodotObject)(object)item.card, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0f), 0.5).SetEase((EaseType)1).SetTrans((TransitionType)7);
		item.currentTween.Chain().TweenCallback(Callable.From((Action)item.card.QueueFreeSafely));
		item.currentTween.Play();
	}

	private void TweenCardToQueuePosition(QueueItem item, int queueIndex)
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		Tween? currentTween = item.currentTween;
		if (currentTween != null)
		{
			currentTween.Kill();
		}
		item.currentTween = ((Node)this).CreateTween().SetParallel(true);
		item.currentTween.TweenProperty((GodotObject)(object)item.card, NodePath.op_Implicit("position"), Variant.op_Implicit(GetPositionForQueueIndex(item.card, queueIndex)), 0.3499999940395355).SetEase((EaseType)1).SetTrans((TransitionType)7);
		item.currentTween.TweenProperty((GodotObject)(object)item.card, NodePath.op_Implicit("scale"), Variant.op_Implicit(GetScaleForQueueIndex(queueIndex)), 0.3499999940395355).SetEase((EaseType)1).SetTrans((TransitionType)7);
		item.currentTween.TweenProperty((GodotObject)(object)item.card, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(1f), 0.3499999940395355).SetEase((EaseType)1).SetTrans((TransitionType)7);
		item.currentTween.Play();
	}

	private Vector2 GetScaleForQueueIndex(int index)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		index++;
		float num = 1f - (float)index / (float)(index + 1);
		return num * Vector2.One * 0.8f;
	}

	private Vector2 GetPositionForQueueIndex(NCard card, int index)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		index++;
		float num = (float)index / (float)(index + 2);
		return PileType.Play.GetTargetPosition(card) + Vector2.Left * 300f * num;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Expected O, but got Unknown
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Expected O, but got Unknown
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Expected O, but got Unknown
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(8);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._ExitTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.RemoveCardFromQueueForCancellation, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("card"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false),
			new PropertyInfo((Type)1, StringName.op_Implicit("forceReturnToHand"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.RemoveCardFromQueue, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("card"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.TweenAllToQueuePosition, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AnimOut, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.GetScaleForQueueIndex, new PropertyInfo((Type)5, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("index"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.GetPositionForQueueIndex, new PropertyInfo((Type)5, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("card"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false),
			new PropertyInfo((Type)2, StringName.op_Implicit("index"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
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
		if ((ref method) == MethodName.RemoveCardFromQueueForCancellation && ((NativeVariantPtrArgs)(ref args)).Count == 2)
		{
			RemoveCardFromQueueForCancellation(VariantUtils.ConvertTo<NCard>(ref ((NativeVariantPtrArgs)(ref args))[0]), VariantUtils.ConvertTo<bool>(ref ((NativeVariantPtrArgs)(ref args))[1]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.RemoveCardFromQueue && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			RemoveCardFromQueue(VariantUtils.ConvertTo<NCard>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.TweenAllToQueuePosition && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			TweenAllToQueuePosition();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.AnimOut && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			AnimOut();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.GetScaleForQueueIndex && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			Vector2 scaleForQueueIndex = GetScaleForQueueIndex(VariantUtils.ConvertTo<int>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<Vector2>(ref scaleForQueueIndex);
			return true;
		}
		if ((ref method) == MethodName.GetPositionForQueueIndex && ((NativeVariantPtrArgs)(ref args)).Count == 2)
		{
			Vector2 positionForQueueIndex = GetPositionForQueueIndex(VariantUtils.ConvertTo<NCard>(ref ((NativeVariantPtrArgs)(ref args))[0]), VariantUtils.ConvertTo<int>(ref ((NativeVariantPtrArgs)(ref args))[1]));
			ret = VariantUtils.CreateFrom<Vector2>(ref positionForQueueIndex);
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
		if ((ref method) == MethodName.RemoveCardFromQueueForCancellation)
		{
			return true;
		}
		if ((ref method) == MethodName.RemoveCardFromQueue)
		{
			return true;
		}
		if ((ref method) == MethodName.TweenAllToQueuePosition)
		{
			return true;
		}
		if ((ref method) == MethodName.AnimOut)
		{
			return true;
		}
		if ((ref method) == MethodName.GetScaleForQueueIndex)
		{
			return true;
		}
		if ((ref method) == MethodName.GetPositionForQueueIndex)
		{
			return true;
		}
		return ((Control)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).SaveGodotObjectData(info);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
	}
}
