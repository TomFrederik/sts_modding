using System.Collections.Generic;
using Godot;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Rooms;

namespace MegaCrit.Sts2.Core.Models.Encounters;

public sealed class FabricatorNormal : EncounterModel
{
	private const string _fabricatorSlot = "fabricator";

	private const string _botSlotPrefix = "bot";

	public override RoomType RoomType => RoomType.Monster;

	public override bool HasScene => true;

	public override IReadOnlyList<string> Slots => new global::_003C_003Ez__ReadOnlyArray<string>(new string[5] { "bot1", "bot2", "fabricator", "bot3", "bot4" });

	public override IEnumerable<MonsterModel> AllPossibleMonsters
	{
		get
		{
			MonsterModel monsterModel = ModelDb.Monster<Fabricator>();
			HashSet<MonsterModel> defenseSpawns = Fabricator.defenseSpawns;
			HashSet<MonsterModel> aggroSpawns = Fabricator.aggroSpawns;
			int num = 0;
			MonsterModel[] array = new MonsterModel[1 + (defenseSpawns.Count + aggroSpawns.Count)];
			array[num] = monsterModel;
			num++;
			foreach (MonsterModel item in defenseSpawns)
			{
				array[num] = item;
				num++;
			}
			foreach (MonsterModel item2 in aggroSpawns)
			{
				array[num] = item2;
				num++;
			}
			return new global::_003C_003Ez__ReadOnlyArray<MonsterModel>(array);
		}
	}

	protected override IReadOnlyList<(MonsterModel, string?)> GenerateMonsters()
	{
		return new _003C_003Ez__ReadOnlySingleElementList<(MonsterModel, string)>((ModelDb.Monster<Fabricator>().ToMutable(), "fabricator"));
	}

	public override float GetCameraScaling()
	{
		return 0.85f;
	}

	public override Vector2 GetCameraOffset()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		return Vector2.Down * 60f;
	}

	public static void SetBotFallPosition(NCreature creatureNode)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		Node2D specialNode = creatureNode.GetSpecialNode<Node2D>("Visuals/FallControl");
		if (specialNode != null)
		{
			float num = 125f;
			specialNode.Position = Vector2.Down * ((num - ((Control)creatureNode).Position.Y) / creatureNode.Visuals.Body.Scale.Y);
		}
	}
}
