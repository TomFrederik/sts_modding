using System.Collections.Generic;
using Godot;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Rooms;

namespace MegaCrit.Sts2.Core.Models.Encounters;

public sealed class BygoneEffigyElite : EncounterModel
{
	public override RoomType RoomType => RoomType.Elite;

	public override IEnumerable<MonsterModel> AllPossibleMonsters => new _003C_003Ez__ReadOnlySingleElementList<MonsterModel>(ModelDb.Monster<BygoneEffigy>());

	public override float GetCameraScaling()
	{
		return 0.88f;
	}

	public override Vector2 GetCameraOffset()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		return Vector2.Down * 50f;
	}

	protected override IReadOnlyList<(MonsterModel, string?)> GenerateMonsters()
	{
		return new _003C_003Ez__ReadOnlySingleElementList<(MonsterModel, string)>((ModelDb.Monster<BygoneEffigy>().ToMutable(), null));
	}
}
