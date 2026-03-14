using System.Collections.Generic;
using Godot;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Rooms;

namespace MegaCrit.Sts2.Core.Models.Encounters;

public sealed class OvicopterNormal : EncounterModel
{
	private const string _ovicopterSlot = "ovicopter";

	private const string _eggSlotPrefix = "egg";

	public override bool HasScene => true;

	public override IReadOnlyList<string> Slots => new global::_003C_003Ez__ReadOnlyArray<string>(new string[6] { "egg1", "egg2", "egg3", "egg4", "egg5", "ovicopter" });

	public override RoomType RoomType => RoomType.Monster;

	public override IEnumerable<MonsterModel> AllPossibleMonsters => new global::_003C_003Ez__ReadOnlyArray<MonsterModel>(new MonsterModel[2]
	{
		ModelDb.Monster<Ovicopter>(),
		ModelDb.Monster<ToughEgg>()
	});

	public override float GetCameraScaling()
	{
		return 0.8f;
	}

	public override Vector2 GetCameraOffset()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		return Vector2.Down * 50f + Vector2.Left * 100f;
	}

	protected override IReadOnlyList<(MonsterModel, string?)> GenerateMonsters()
	{
		List<(MonsterModel, string)> list = new List<(MonsterModel, string)>();
		list.Add((ModelDb.Monster<Ovicopter>().ToMutable(), "ovicopter"));
		return list;
	}
}
