using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.TestSupport;
using MegaCrit.Sts2.Core.ValueProps;

namespace MegaCrit.Sts2.Core.Models.Powers;

public sealed class RollingBoulderPower : PowerModel
{
	public override PowerType Type => PowerType.Buff;

	public override PowerStackType StackType => PowerStackType.Counter;

	public override bool IsInstanced => true;

	protected override IEnumerable<DynamicVar> CanonicalVars => new _003C_003Ez__ReadOnlySingleElementList<DynamicVar>(new DamageVar(5m, ValueProp.Unpowered));

	public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
	{
		if (player != base.Owner.Player)
		{
			return;
		}
		Flash();
		if (TestMode.IsOn)
		{
			await DoDamage(choiceContext, base.CombatState.HittableEnemies);
		}
		else
		{
			List<Task> damageTasks = new List<Task>();
			NRollingBoulderVfx vfx = NRollingBoulderVfx.Create(base.CombatState.HittableEnemies, base.Amount);
			((GodotObject)vfx).Connect(NRollingBoulderVfx.SignalName.HitCreature, Callable.From<NCreature>((Action<NCreature>)delegate(NCreature c)
			{
				damageTasks.Add(DoDamage(choiceContext, new _003C_003Ez__ReadOnlySingleElementList<Creature>(c.Entity)));
			}), 0u);
			Callable val = Callable.From((Action)delegate
			{
				((Node)(object)NCombatRoom.Instance?.CombatVfxContainer).AddChildSafely((Node?)(object)vfx);
				if (!((Node)vfx).IsInsideTree())
				{
					throw new InvalidOperationException("VFX is not inside tree after adding it to combat room!");
				}
			});
			((Callable)(ref val)).CallDeferred(Array.Empty<Variant>());
			await ((GodotObject)vfx).ToSignal((GodotObject)(object)vfx, SignalName.TreeExiting);
			await Task.WhenAll(damageTasks);
		}
		base.Amount += base.DynamicVars.Damage.IntValue;
		InvokeDisplayAmountChanged();
	}

	private Task<IEnumerable<DamageResult>> DoDamage(PlayerChoiceContext choiceContext, IEnumerable<Creature> targets)
	{
		return CreatureCmd.Damage(choiceContext, targets, base.Amount, ValueProp.Unpowered, base.Owner);
	}
}
