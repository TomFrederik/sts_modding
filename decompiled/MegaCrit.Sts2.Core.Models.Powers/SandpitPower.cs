using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Nodes.Audio;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.TestSupport;

namespace MegaCrit.Sts2.Core.Models.Powers;

public sealed class SandpitPower : PowerModel
{
	private const float _paddingDistanceFromMonster = 450f;

	private const float _paddingDistanceFromOriginal = 50f;

	private const float _tweenTime = 0.25f;

	private int _initialAmount;

	private float _initialTargetPosition;

	public override PowerType Type => PowerType.Buff;

	public override PowerStackType StackType => PowerStackType.Counter;

	public override bool IsInstanced => true;

	private IReadOnlyList<Creature> AllAffectedCreatures
	{
		get
		{
			Creature creature = base.Target.Player.Creature;
			IReadOnlyList<Creature> pets = base.Target.Pets;
			int num = 0;
			Creature[] array = new Creature[1 + pets.Count];
			array[num] = creature;
			num++;
			foreach (Creature item in pets)
			{
				array[num] = item;
				num++;
			}
			return new global::_003C_003Ez__ReadOnlyArray<Creature>(array);
		}
	}

	public override Task AfterApplied(Creature? applier, CardModel? cardSource)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		if (TestMode.IsOn)
		{
			return Task.CompletedTask;
		}
		NCreature creatureNode = NCombatRoom.Instance.GetCreatureNode(base.Target);
		_initialAmount = base.Amount;
		_initialTargetPosition = ((Control)creatureNode).GlobalPosition.X;
		return Task.CompletedTask;
	}

	public override async Task AfterSideTurnStart(CombatSide side, CombatState combatState)
	{
		if (side == CombatSide.Enemy)
		{
			await PowerCmd.Decrement(this);
		}
	}

	public override async Task AfterPowerAmountChanged(PowerModel power, decimal _, Creature? __, CardModel? cardSource)
	{
		if (!TestMode.IsOn && power == this)
		{
			await UpdateCreaturePositions();
			if (LocalContext.IsMe(base.Target))
			{
				int num = Mathf.Clamp(6 - base.Amount, 0, 5);
				NRunMusicController.Instance?.UpdateMusicParameter(TheInsatiable.TheInsatiableTrackName, num);
			}
		}
	}

	public override async Task AfterRemoved(Creature oldOwner)
	{
		if (oldOwner.IsDead || base.Target.IsDead)
		{
			return;
		}
		if (TestMode.IsOff)
		{
			NCreature creatureNode = NCombatRoom.Instance.GetCreatureNode(base.Owner);
			Tween val = ((Node)NCombatRoom.Instance).CreateTween();
			float num = ((Control)creatureNode).GlobalPosition.X - 450f;
			foreach (Creature allAffectedCreature in AllAffectedCreatures)
			{
				NCreature creatureNode2 = NCombatRoom.Instance.GetCreatureNode(allAffectedCreature);
				val.Parallel().TweenProperty((GodotObject)(object)creatureNode2, NodePath.op_Implicit("global_position:x"), Variant.op_Implicit(num), 0.699999988079071).SetEase((EaseType)2)
					.SetTrans((TransitionType)1);
			}
		}
		SfxCmd.Play("event:/sfx/enemy/enemy_attacks/the_insatiable/the_insatiable_finisher");
		await CreatureCmd.TriggerAnim(base.Owner, "EatPlayerTrigger", 0f);
		await Cmd.Wait(0.5f);
		foreach (Creature allAffectedCreature2 in AllAffectedCreatures)
		{
			if (TestMode.IsOff)
			{
				NCreature creatureNode3 = NCombatRoom.Instance.GetCreatureNode(allAffectedCreature2);
				((CanvasItem)creatureNode3.Visuals).Visible = false;
			}
			if (allAffectedCreature2.IsPlayer || allAffectedCreature2.Monster is Osty)
			{
				await CreatureCmd.Kill(allAffectedCreature2, force: true);
			}
		}
	}

	public override async Task AfterCreatureAddedToCombat(Creature creature)
	{
		if (creature.Side != base.Owner.Side)
		{
			await UpdateCreaturePositions();
		}
	}

	public override async Task AfterOstyRevived(Creature osty)
	{
		await UpdateCreaturePositions();
	}

	public override async Task BeforeTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
	{
		if (side == CombatSide.Enemy)
		{
			await UpdateCreaturePositions();
		}
	}

	private async Task UpdateCreaturePositions()
	{
		if (TestMode.IsOn)
		{
			return;
		}
		NCreature creatureNode = NCombatRoom.Instance.GetCreatureNode(base.Owner);
		NCreature creatureNode2 = NCombatRoom.Instance.GetCreatureNode(base.Target);
		float num = ((Control)creatureNode).GlobalPosition.X - 450f;
		float num2 = _initialTargetPosition + 50f;
		float num3 = (num2 - num) / (float)_initialAmount;
		int num4 = Mathf.Min(base.Amount, _initialAmount);
		int num5 = Mathf.Max(base.Amount - _initialAmount, 0);
		NCreature nCreature = creatureNode2;
		float num6 = 0f;
		Player player = base.Target?.Player;
		if (player != null && player.IsOstyAlive && LocalContext.IsMe(base.Target))
		{
			NCreature creatureNode3 = NCombatRoom.Instance.GetCreatureNode(base.Target.Player.Osty);
			nCreature = creatureNode3;
			float x = NCreature.GetOstyOffsetFromPlayer(creatureNode3.Entity).X;
			num2 += x;
			num3 = (num2 - num) / (float)_initialAmount;
			float num7 = 100f * (1f - (float)num4 / (float)_initialAmount);
			num6 = x - num7;
		}
		float num8 = ((Control)creatureNode).GlobalPosition.X - 400f + num3 * (float)num4 + num3 * ((float)num5 / ((float)num5 + 2f));
		Tween val = null;
		foreach (Creature allAffectedCreature in AllAffectedCreatures)
		{
			float num9 = num8 - ((Control)nCreature).GlobalPosition.X;
			if (allAffectedCreature != nCreature.Entity)
			{
				num9 = num8 - num6 - ((Control)creatureNode2).GlobalPosition.X;
			}
			if (Math.Abs(num9) <= 5f || allAffectedCreature.IsDead)
			{
				continue;
			}
			NCreature creatureNode4 = NCombatRoom.Instance.GetCreatureNode(allAffectedCreature);
			if (creatureNode4 != null)
			{
				if (val == null)
				{
					val = ((Node)NCombatRoom.Instance).CreateTween().SetParallel(true).SetEase((EaseType)1)
						.SetTrans((TransitionType)7);
				}
				val.TweenProperty((GodotObject)(object)creatureNode4, NodePath.op_Implicit("global_position:x"), Variant.op_Implicit(((Control)creatureNode4).GlobalPosition.X + num9), 0.25);
			}
		}
		if (val != null)
		{
			await ((GodotObject)val).ToSignal((GodotObject)(object)val, SignalName.Finished);
		}
	}
}
