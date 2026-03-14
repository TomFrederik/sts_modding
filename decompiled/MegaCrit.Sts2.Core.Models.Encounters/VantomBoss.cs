using System.Collections.Generic;
using Godot;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Rooms;

namespace MegaCrit.Sts2.Core.Models.Encounters;

public sealed class VantomBoss : EncounterModel
{
	public override RoomType RoomType => RoomType.Boss;

	public override string BossNodePath => "res://images/map/placeholder/" + base.Id.Entry.ToLowerInvariant() + "_icon";

	public override MegaSkeletonDataResource? BossNodeSpineResource => null;

	protected override bool HasCustomBackground => true;

	public override string CustomBgm => "event:/music/act1_boss_vantom";

	public override IEnumerable<MonsterModel> AllPossibleMonsters => new _003C_003Ez__ReadOnlySingleElementList<MonsterModel>(ModelDb.Monster<Vantom>());

	public override float GetCameraScaling()
	{
		return 0.9f;
	}

	public override Vector2 GetCameraOffset()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		return Vector2.Down * 50f;
	}

	protected override IReadOnlyList<(MonsterModel, string?)> GenerateMonsters()
	{
		return new _003C_003Ez__ReadOnlySingleElementList<(MonsterModel, string)>((ModelDb.Monster<Vantom>().ToMutable(), null));
	}
}
