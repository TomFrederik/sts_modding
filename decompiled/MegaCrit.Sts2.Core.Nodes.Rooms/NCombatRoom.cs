using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Characters;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Screens.ScreenContext;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.TestSupport;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.Nodes.Rooms;

[ScriptPath("res://src/Core/Nodes/Rooms/NCombatRoom.cs")]
public class NCombatRoom : Control, IScreenContext, IRoomWithProceedButton
{
	private struct PlayerAndPets
	{
		public NCreature player;

		public List<NCreature> pets;
	}

	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName _EnterTree = StringName.op_Implicit("_EnterTree");

		public static readonly StringName _ExitTree = StringName.op_Implicit("_ExitTree");

		public static readonly StringName SubscribeToCombatEvents = StringName.op_Implicit("SubscribeToCombatEvents");

		public static readonly StringName AdjustCreatureScaleForAspectRatio = StringName.op_Implicit("AdjustCreatureScaleForAspectRatio");

		public static readonly StringName CreateAllyNodes = StringName.op_Implicit("CreateAllyNodes");

		public static readonly StringName CreateEnemyNodes = StringName.op_Implicit("CreateEnemyNodes");

		public static readonly StringName RemoveCreatureNode = StringName.op_Implicit("RemoveCreatureNode");

		public static readonly StringName UpdateCreatureNavigation = StringName.op_Implicit("UpdateCreatureNavigation");

		public static readonly StringName OnActiveScreenUpdated = StringName.op_Implicit("OnActiveScreenUpdated");

		public static readonly StringName EnableControllerNavigation = StringName.op_Implicit("EnableControllerNavigation");

		public static readonly StringName RandomizeEnemyScalesAndHues = StringName.op_Implicit("RandomizeEnemyScalesAndHues");

		public static readonly StringName RadialBlur = StringName.op_Implicit("RadialBlur");

		public static readonly StringName SetWaitingForOtherPlayersOverlayVisible = StringName.op_Implicit("SetWaitingForOtherPlayersOverlayVisible");

		public static readonly StringName OnProceedButtonPressed = StringName.op_Implicit("OnProceedButtonPressed");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName Ui = StringName.op_Implicit("Ui");

		public static readonly StringName SceneContainer = StringName.op_Implicit("SceneContainer");

		public static readonly StringName BgContainer = StringName.op_Implicit("BgContainer");

		public static readonly StringName Background = StringName.op_Implicit("Background");

		public static readonly StringName ProceedButton = StringName.op_Implicit("ProceedButton");

		public static readonly StringName BackCombatVfxContainer = StringName.op_Implicit("BackCombatVfxContainer");

		public static readonly StringName CombatVfxContainer = StringName.op_Implicit("CombatVfxContainer");

		public static readonly StringName CreatedMsec = StringName.op_Implicit("CreatedMsec");

		public static readonly StringName Mode = StringName.op_Implicit("Mode");

		public static readonly StringName EncounterSlots = StringName.op_Implicit("EncounterSlots");

		public static readonly StringName DefaultFocusedControl = StringName.op_Implicit("DefaultFocusedControl");

		public static readonly StringName FocusedControlFromTopBar = StringName.op_Implicit("FocusedControlFromTopBar");

		public static readonly StringName _allyContainer = StringName.op_Implicit("_allyContainer");

		public static readonly StringName _enemyContainer = StringName.op_Implicit("_enemyContainer");

		public static readonly StringName _radialBlur = StringName.op_Implicit("_radialBlur");

		public static readonly StringName _proceedButton = StringName.op_Implicit("_proceedButton");

		public static readonly StringName _waitingForOtherPlayersOverlay = StringName.op_Implicit("_waitingForOtherPlayersOverlay");

		public static readonly StringName _window = StringName.op_Implicit("_window");
	}

	public class SignalName : SignalName
	{
	}

	private const float _centerSafeZone = 150f;

	private const float _defaultPadding = 70f;

	private const float _minimumAutoPadding = 5f;

	private const float _minAlternatingYPos = 40f;

	private const float _maxAlternatingYPos = 60f;

	private const float _alternateYPosBeginPadding = 30f;

	private const float _yPos = 200f;

	private static readonly LocString _waitingLoc = new LocString("gameplay_ui", "MULTIPLAYER_WAITING");

	private readonly List<NCreature> _creatureNodes = new List<NCreature>();

	private readonly List<NCreature> _removingCreatureNodes = new List<NCreature>();

	private Control _allyContainer;

	private Control _enemyContainer;

	private const string _scenePath = "res://scenes/rooms/combat_room.tscn";

	private NRadialBlurVfx _radialBlur;

	private NProceedButton _proceedButton;

	private Control _waitingForOtherPlayersOverlay;

	private ICombatRoomVisuals _visuals;

	private Window _window;

	public static NCombatRoom? Instance => NRun.Instance?.CombatRoom;

	public NCombatUi Ui { get; private set; }

	public IEnumerable<NCreature> CreatureNodes => _creatureNodes;

	public IEnumerable<NCreature> RemovingCreatureNodes => ((IEnumerable<NCreature>)_removingCreatureNodes).Where((Func<NCreature, bool>)GodotObject.IsInstanceValid);

	public Control SceneContainer { get; private set; }

	private Control BgContainer { get; set; }

	public NCombatBackground Background { get; private set; }

	public NProceedButton ProceedButton => _proceedButton;

	public Node BackCombatVfxContainer { get; private set; }

	public Control CombatVfxContainer { get; private set; }

	public ulong CreatedMsec { get; private set; }

	public CombatRoomMode Mode { get; private set; }

	private Control? EncounterSlots { get; set; }

	public static IEnumerable<string> AssetPaths => new _003C_003Ez__ReadOnlySingleElementList<string>("res://scenes/rooms/combat_room.tscn");

	public Control DefaultFocusedControl => Ui.Hand.CardHolderContainer;

	public Control? FocusedControlFromTopBar => _creatureNodes.FirstOrDefault(delegate(NCreature c)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Invalid comparison between Unknown and I8
		if (c != null && c.IsInteractable)
		{
			Control hitbox = c.Hitbox;
			if (hitbox != null)
			{
				return (long)hitbox.FocusMode == 2;
			}
		}
		return false;
	})?.Hitbox ?? DefaultFocusedControl;

	public event Action? ProceedButtonPressed;

	public static NCombatRoom? Create(ICombatRoomVisuals visuals, CombatRoomMode mode)
	{
		if (TestMode.IsOn)
		{
			return null;
		}
		NCombatRoom nCombatRoom = PreloadManager.Cache.GetScene("res://scenes/rooms/combat_room.tscn").Instantiate<NCombatRoom>((GenEditState)0);
		nCombatRoom._visuals = visuals;
		nCombatRoom.Mode = mode;
		return nCombatRoom;
	}

	public override void _Ready()
	{
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_0276: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
		Ui = ((Node)this).GetNode<NCombatUi>(NodePath.op_Implicit("%CombatUi"));
		Ui.Deactivate();
		SceneContainer = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%CombatSceneContainer"));
		_allyContainer = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%AllyContainer"));
		_enemyContainer = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%EnemyContainer"));
		BackCombatVfxContainer = (Node)(object)((Node)this).GetNode<Node2D>(NodePath.op_Implicit("%BackCombatVfxContainer"));
		CombatVfxContainer = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%CombatVfxContainer"));
		_radialBlur = ((Node)this).GetNode<NRadialBlurVfx>(NodePath.op_Implicit("RadialBlur"));
		_waitingForOtherPlayersOverlay = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%WaitingForOtherPlayers"));
		((Node)_waitingForOtherPlayersOverlay).GetNode<MegaLabel>(NodePath.op_Implicit("Label")).SetTextAutoSize(_waitingLoc.GetRawText());
		_proceedButton = ((Node)this).GetNode<NProceedButton>(NodePath.op_Implicit("%ProceedButton"));
		_proceedButton.UpdateText(NProceedButton.ProceedLoc);
		if (Mode == CombatRoomMode.VisualOnly && NEventRoom.Instance == null)
		{
			((CanvasItem)_proceedButton).Visible = true;
			((GodotObject)_proceedButton).Connect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)OnProceedButtonPressed), 0u);
			_proceedButton.Enable();
		}
		BgContainer = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%BgContainer"));
		CreatedMsec = Time.GetTicksMsec();
		Log.Info($"Creating NCombatRoom with mode={Mode} encounter={_visuals.Encounter.Id.Entry}.");
		CreateAllyNodes();
		if (Mode != CombatRoomMode.FinishedCombat)
		{
			CreateEnemyNodes();
		}
		if (Mode != CombatRoomMode.ActiveCombat)
		{
			foreach (NCreature creatureNode in _creatureNodes)
			{
				creatureNode.Hitbox.FocusMode = (FocusModeEnum)0;
				SetCreatureIsInteractable(creatureNode.Entity, on: false);
			}
		}
		SceneContainer.Scale = Vector2.One * _visuals.Encounter.GetCameraScaling();
		Control sceneContainer = SceneContainer;
		sceneContainer.Position += _visuals.Encounter.GetCameraOffset();
		((CanvasItem)SceneContainer).ZIndex = -10;
		((CanvasItem)CombatVfxContainer).ZIndex = -9;
		_window = ((Node)this).GetTree().Root;
		((GodotObject)_window).Connect(SignalName.SizeChanged, Callable.From((Action)AdjustCreatureScaleForAspectRatio), 0u);
		AdjustCreatureScaleForAspectRatio();
		NGame.Instance.SetScreenShakeTarget(SceneContainer);
	}

	public static Rng GenerateBackgroundRngForCurrentPoint(IRunState state)
	{
		uint num = 0u;
		if (state.CurrentMapCoord.HasValue)
		{
			num = (uint)(state.CurrentMapCoord.Value.row + state.CurrentMapCoord.Value.col * 747);
		}
		return new Rng(state.Rng.Seed + num);
	}

	public override void _EnterTree()
	{
		ActiveScreenContext.Instance.Updated += OnActiveScreenUpdated;
		if (Mode == CombatRoomMode.ActiveCombat)
		{
			SubscribeToCombatEvents();
		}
	}

	public override void _ExitTree()
	{
		NGame.Instance.ClearScreenShakeTarget();
		ActiveScreenContext.Instance.Updated -= OnActiveScreenUpdated;
		CombatManager.Instance.CombatSetUp -= OnCombatSetUp;
		CombatManager.Instance.CombatEnded -= RestrictControllerNavigation;
		CombatManager.Instance.CombatWon -= RestrictControllerNavigation;
	}

	private void SubscribeToCombatEvents()
	{
		CombatManager.Instance.CombatSetUp += OnCombatSetUp;
		CombatManager.Instance.CombatEnded += RestrictControllerNavigation;
		CombatManager.Instance.CombatWon += RestrictControllerNavigation;
	}

	private void OnCombatSetUp(CombatState state)
	{
		Ui.Activate(((CombatRoom)_visuals).CombatState);
		if (Background == null)
		{
			SetUpBackground(state.RunState);
		}
	}

	public void SetUpBackground(IRunState state)
	{
		if (Background != null)
		{
			Log.Warn("Tried to set up background twice!");
		}
		Background = _visuals.Encounter.CreateBackground(_visuals.Act, GenerateBackgroundRngForCurrentPoint(state));
		((Node)(object)BgContainer).AddChildSafely((Node?)(object)Background);
	}

	private void AdjustCreatureScaleForAspectRatio()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		_allyContainer.Scale = Vector2.One;
		_enemyContainer.Scale = Vector2.One;
		_enemyContainer.Position = SceneContainer.Size * 0.5f;
		float num = 0f;
		foreach (NCreature creatureNode in _creatureNodes)
		{
			num = Math.Max(((Control)creatureNode).GlobalPosition.X + creatureNode.Visuals.Bounds.Size.X * 0.5f * SceneContainer.Scale.X, num);
		}
		num += 15f;
		if (num > ((Control)this).Size.X)
		{
			float num2 = ((Control)this).Size.X / num;
			_allyContainer.Scale = Vector2.One * num2;
			_enemyContainer.Scale = Vector2.One * num2;
			Control enemyContainer = _enemyContainer;
			enemyContainer.Position += Vector2.Left * (num - ((Control)this).Size.X) * num2;
		}
	}

	private void CreateAllyNodes()
	{
		List<Creature> allies = _visuals.Allies.ToList();
		foreach (Creature item in allies)
		{
			AddCreature(item);
		}
		PositionPlayersAndPets(_creatureNodes.Where((NCreature c) => allies.Contains(c.Entity)).ToList(), _visuals.Encounter.GetCameraScaling(), _visuals.Encounter.FullyCenterPlayers);
	}

	private void CreateEnemyNodes()
	{
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		if (_visuals.Encounter.HasScene)
		{
			EncounterSlots = _visuals.Encounter.CreateScene();
		}
		if (EncounterSlots != null)
		{
			((Node)(object)_enemyContainer).AddChildSafely((Node?)(object)EncounterSlots);
			Control? encounterSlots = EncounterSlots;
			encounterSlots.Position -= new Vector2(1920f, 1080f) * 0.5f;
		}
		List<Creature> enemies = _visuals.Enemies.ToList();
		foreach (Creature item in enemies)
		{
			AddCreature(item);
		}
		List<NCreature> creatures = _creatureNodes.Where((NCreature c) => enemies.Contains(c.Entity)).ToList();
		if (EncounterSlots != null)
		{
			PositionCreaturesWithSlots(creatures);
		}
		else
		{
			PositionEnemies(creatures, _visuals.Encounter.GetCameraScaling());
		}
		RandomizeEnemyScalesAndHues();
	}

	private void PositionCreaturesWithSlots(List<NCreature> creatures)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		foreach (NCreature creature in creatures)
		{
			string slotName = creature.Entity.SlotName;
			((Control)creature).GlobalPosition = ((Node2D)((Node)EncounterSlots).GetNode<Marker2D>(NodePath.op_Implicit(slotName))).GlobalPosition;
		}
	}

	private void PositionEnemies(List<NCreature> creatures, float scaling)
	{
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		float num = 960f / scaling;
		float num2 = 70f;
		float num3 = creatures.Sum((NCreature n) => n.Visuals.Bounds.Size.X);
		float num4 = num3 + (float)(creatures.Count - 1) * num2;
		float val = (num - num4) * 0.5f;
		val = Math.Max(val, 150f);
		float num5 = 0f;
		if (val + num4 > num)
		{
			num2 = Math.Max((num - 150f - num3) / (float)(creatures.Count - 1), 5f);
			num4 = num3 + (float)(creatures.Count - 1) * num2;
			val = (num - num4) * 0.5f;
			if (num2 < 30f)
			{
				num5 = float.Lerp(60f, 40f, (num2 - 5f) / 25f);
			}
			if (val + num4 > num)
			{
				Log.Warn("Creatures for current encounter (" + _visuals.Encounter.Title.GetFormattedText() + ") are being displayed off-screen because they are too wide!");
			}
		}
		for (int num6 = 0; num6 < creatures.Count; num6++)
		{
			NCreature nCreature = creatures[num6];
			((Control)nCreature).Position = new Vector2(val + nCreature.Visuals.Bounds.Size.X * 0.5f, 200f - ((num6 % 2 != 0) ? num5 : 0f));
			val += nCreature.Visuals.Bounds.Size.X + num2;
		}
	}

	public static void PositionPlayersAndPets(List<NCreature> creatureNodes, float scaling, bool fullyCenterPlayers)
	{
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0294: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_035b: Unknown result type (might be due to invalid IL or missing references)
		//IL_032b: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0472: Unknown result type (might be due to invalid IL or missing references)
		//IL_0403: Unknown result type (might be due to invalid IL or missing references)
		//IL_043c: Unknown result type (might be due to invalid IL or missing references)
		List<PlayerAndPets> list = new List<PlayerAndPets>();
		foreach (NCreature creatureNode in creatureNodes)
		{
			if (creatureNode.Entity.IsPlayer)
			{
				PlayerAndPets item = new PlayerAndPets
				{
					player = creatureNode,
					pets = new List<NCreature>()
				};
				if (LocalContext.IsMe(creatureNode.Entity))
				{
					list.Insert(0, item);
				}
				else
				{
					list.Add(item);
				}
			}
		}
		foreach (NCreature creature in creatureNodes)
		{
			if (!creature.Entity.IsPlayer)
			{
				list.First((PlayerAndPets p) => p.player.Entity.Player == creature.Entity.PetOwner).pets.Add(creature);
			}
		}
		float num = 960f / scaling;
		float num2 = 70f;
		int num3 = (int)Math.Ceiling(Math.Sqrt(list.Count));
		int num4 = (int)Math.Ceiling((double)list.Count / (double)num3);
		float num5 = creatureNodes.Take(num3).Sum((NCreature n) => n.Visuals.Bounds.Size.X);
		float num6 = num5 + (float)(num3 - 1) * num2;
		float num7 = num5 * 0.33f;
		float num8 = ((num4 > 1) ? (num7 / (float)(num4 - 1)) : 0f);
		float num9 = ((num4 > 1) ? (120f / (float)(num4 - 1)) : 0f);
		float num10;
		if (fullyCenterPlayers)
		{
			num10 = creatureNodes.First((NCreature c) => c.Entity.IsPlayer).Visuals.Bounds.Size.X * -0.5f;
		}
		else
		{
			num10 = (num - num6) * 0.5f;
			num10 = Math.Max(num10, 150f);
			if (list.Count >= num3 * 2)
			{
				num5 += num7;
			}
			if (num10 + num6 > num)
			{
				num2 = (num - 150f - num5) / (float)(num3 - 1);
				num6 = num5 + (float)(num3 - 1) * num2;
				num10 = (num - num6) * 0.5f;
			}
		}
		for (int num11 = 0; num11 < num3; num11++)
		{
			float targetXPos = num10 + num8 * (float)num11;
			for (int num12 = 0; num12 < num3; num12++)
			{
				int num13 = num11 * num3 + num12;
				if (num13 >= list.Count)
				{
					break;
				}
				PlayerAndPets playerAndPets = list[num13];
				NCreature player = playerAndPets.player;
				List<NCreature> pets = playerAndPets.pets;
				((Control)player).Position = new Vector2(0f - targetXPos - player.Visuals.Bounds.Size.X * 0.5f, 200f - num9 * (float)num11);
				if (LocalContext.IsMe(player.Entity) && player.Entity.Player.Character is Necrobinder)
				{
					NCreature osty = null;
					for (int num14 = 0; num14 < pets.Count; num14++)
					{
						NCreature nCreature = pets[num14];
						if (nCreature.Entity.Monster is Osty)
						{
							osty = nCreature;
							pets.RemoveAt(num14);
							break;
						}
					}
					PositionLocalPlayerOsty(ref targetXPos, ((Control)player).Position.Y, player, osty);
				}
				float num15 = ((pets.Count > 1) ? (player.Visuals.Bounds.Size.X / (float)(pets.Count - 1)) : 0f);
				for (int num16 = 0; num16 < pets.Count; num16++)
				{
					NCreature nCreature2 = pets[num16];
					((Control)nCreature2).Position = new Vector2(0f - targetXPos + 20f - (float)num16 * num15 - nCreature2.Visuals.Bounds.Size.X * 0.5f, ((Control)player).Position.Y + 10f);
				}
				if (num11 > 0)
				{
					((CanvasItem)playerAndPets.player.Visuals).Modulate = new Color(0.5f, 0.5f, 0.5f, 1f);
					foreach (NCreature item2 in pets)
					{
						((CanvasItem)item2.Visuals).Modulate = new Color(0.5f, 0.5f, 0.5f, 1f);
					}
				}
				targetXPos += playerAndPets.player.Visuals.Bounds.Size.X + num2;
			}
		}
		foreach (PlayerAndPets item3 in list)
		{
			((Node)item3.player).GetParent().MoveChild((Node)(object)item3.player, 0);
			for (int num17 = 0; num17 < item3.pets.Count; num17++)
			{
				NCreature nCreature3 = item3.pets[num17];
				((Node)nCreature3).GetParent().MoveChild((Node)(object)nCreature3, num17 + 1);
				if (!LocalContext.IsMe(item3.player.Entity))
				{
					((CanvasItem)nCreature3.Visuals.Bounds).Visible = false;
				}
			}
		}
	}

	private static void PositionLocalPlayerOsty(ref float targetXPos, float playerYPosition, NCreature player, NCreature? osty)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		Vector2 position = ((Control)player).Position;
		position.X = ((Control)player).Position.X - 150f;
		((Control)player).Position = position;
		if (osty != null)
		{
			((Control)osty).Position = new Vector2(0f - targetXPos, playerYPosition) + NCreature.GetOstyOffsetFromPlayer(osty.Entity);
		}
		targetXPos += 100f;
	}

	public NCreature? GetCreatureNode(Creature? creature)
	{
		if (creature != null)
		{
			return CreatureNodes.FirstOrDefault((NCreature c) => c.Entity == creature);
		}
		return null;
	}

	public void RemoveCreatureNode(NCreature node)
	{
		_creatureNodes.Remove(node);
		_removingCreatureNodes.Add(node);
		UpdateCreatureNavigation();
		if (((Node)this).GetViewport().GuiGetFocusOwner() == node.Hitbox)
		{
			_creatureNodes[0].Hitbox.TryGrabFocus();
		}
		TaskHelper.RunSafely(RemoveCreatureWhenGone(node));
	}

	private async Task RemoveCreatureWhenGone(NCreature node)
	{
		if (node.DeathAnimationTask != null)
		{
			await node.DeathAnimationTask;
		}
		_removingCreatureNodes.Remove(node);
	}

	public void AddCreature(Creature creature)
	{
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		NCreature nCreature = NCreature.Create(creature);
		_creatureNodes.Add(nCreature);
		if (creature.IsPlayer || creature.PetOwner != null)
		{
			((Node)(object)_allyContainer).AddChildSafely((Node?)(object)nCreature);
		}
		else
		{
			((Node)(object)_enemyContainer).AddChildSafely((Node?)(object)nCreature);
		}
		if (creature.SlotName != null)
		{
			if (EncounterSlots == null)
			{
				throw new InvalidOperationException($"Creature {creature} has slot name '{creature.SlotName}' but NCombatRoom.EncounterSlots is null.");
			}
			((Control)nCreature).GlobalPosition = ((Node2D)((Node)EncounterSlots).GetNode<Marker2D>(NodePath.op_Implicit(creature.SlotName))).GlobalPosition;
		}
		UpdateCreatureNavigation();
		if (creature.PetOwner == null)
		{
			return;
		}
		NCreature creatureNode = GetCreatureNode(creature.PetOwner.Creature);
		Player player = creatureNode.Entity.Player;
		List<NCreature> list = _creatureNodes.Where((NCreature c) => c.Entity.PetOwner == player && (!(c.Entity.Monster is Osty) || !LocalContext.IsMe(player))).ToList();
		((Node)nCreature).GetParent().MoveChild((Node)(object)nCreature, ((Node)creatureNode).GetIndex(false) + 1);
		if (creature.Monster is Osty && LocalContext.IsMe(player))
		{
			nCreature.OstyScaleToSize(creature.MaxHp, 0f);
			((Node)nCreature).GetParent().MoveChild((Node)(object)nCreature, ((Node)creatureNode).GetIndex(false));
			return;
		}
		float num = ((list.Count > 1) ? (creatureNode.Visuals.Bounds.Size.X / (float)(list.Count - 1)) : 0f);
		for (int num2 = 0; num2 < list.Count; num2++)
		{
			NCreature nCreature2 = list[num2];
			((Control)nCreature2).Position = new Vector2(((Control)creatureNode).Position.X - 20f + (float)num2 * num + nCreature2.Visuals.Bounds.Size.X * 0.5f, ((Control)creatureNode).Position.Y + 10f);
			nCreature2.ToggleIsInteractable(on: false);
		}
		if (((Control)creatureNode).Position.Y < 199f)
		{
			((CanvasItem)nCreature.Visuals).Modulate = new Color(0.5f, 0.5f, 0.5f, 1f);
		}
	}

	public void SetCreatureIsInteractable(Creature? creature, bool on)
	{
		GetCreatureNode(creature)?.ToggleIsInteractable(on);
		UpdateCreatureNavigation();
	}

	private void UpdateCreatureNavigation()
	{
		List<NCreature> list = (from n in _creatureNodes
			where n.IsInteractable
			orderby ((Control)n).GlobalPosition.X
			select n).ToList();
		for (int num = 0; num < list.Count; num++)
		{
			Control hitbox = list[num].Hitbox;
			NodePath path;
			if (num <= 0)
			{
				path = ((Node)list[list.Count - 1].Hitbox).GetPath();
			}
			else
			{
				path = ((Node)list[num - 1].Hitbox).GetPath();
			}
			hitbox.FocusNeighborLeft = path;
			list[num].Hitbox.FocusNeighborRight = ((num < list.Count - 1) ? ((Node)list[num + 1].Hitbox).GetPath() : ((Node)list[0].Hitbox).GetPath());
			list[num].Hitbox.FocusNeighborBottom = ((Node)Ui.Hand.CardHolderContainer).GetPath();
			list[num].Hitbox.FocusNeighborTop = ((Node)list[num].Hitbox).GetPath();
			list[num].UpdateNavigation();
		}
		Control cardHolderContainer = Ui.Hand.CardHolderContainer;
		NCreature? nCreature = Instance.CreatureNodes.FirstOrDefault();
		cardHolderContainer.FocusNeighborTop = ((nCreature != null) ? ((Node)nCreature.Hitbox).GetPath() : null);
	}

	private void OnActiveScreenUpdated()
	{
		if (CombatManager.Instance.IsInProgress)
		{
			this.UpdateControllerNavEnabled();
			if (ActiveScreenContext.Instance.IsCurrent(this))
			{
				Ui.Enable();
			}
			else
			{
				Ui.Disable();
			}
		}
	}

	private void RestrictControllerNavigation(CombatRoom _)
	{
		RestrictControllerNavigation(Array.Empty<Control>());
	}

	public void RestrictControllerNavigation(IEnumerable<Control> whitelist)
	{
		foreach (NCreature creatureNode in _creatureNodes)
		{
			Control hitbox = creatureNode.Hitbox;
			creatureNode.Hitbox.FocusMode = (FocusModeEnum)(whitelist.Contains(hitbox) ? 2 : 0);
			creatureNode.Hitbox.FocusNeighborBottom = ((Node)creatureNode.Hitbox).GetPath();
		}
		Ui.Hand.DisableControllerNavigation();
	}

	public void EnableControllerNavigation()
	{
		foreach (NCreature creatureNode in _creatureNodes)
		{
			creatureNode.Hitbox.FocusMode = (FocusModeEnum)2;
			creatureNode.Hitbox.FocusNeighborBottom = ((Node)Ui.Hand.CardHolderContainer).GetPath();
		}
		Ui.Hand.EnableControllerNavigation();
	}

	private void RandomizeEnemyScalesAndHues()
	{
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		Dictionary<Type, List<NCreature>> dictionary = new Dictionary<Type, List<NCreature>>();
		foreach (NCreature creatureNode in _creatureNodes)
		{
			if (creatureNode.Entity.Side != CombatSide.Player && creatureNode.Entity.Monster != null)
			{
				Type type = creatureNode.Entity.Monster.GetType();
				if (!dictionary.TryGetValue(type, out var value))
				{
					value = (dictionary[type] = new List<NCreature>());
				}
				value.Add(creatureNode);
			}
		}
		foreach (KeyValuePair<Type, List<NCreature>> item in dictionary)
		{
			if (item.Value.Count == 1)
			{
				continue;
			}
			foreach (NCreature item2 in item.Value)
			{
				MonsterModel monster = item2.Entity.Monster;
				int value2 = monster.Creature.MonsterMaxHpBeforeModification.Value;
				float value3 = ((monster.MaxInitialHp != monster.MinInitialHp) ? (((float)(value2 - monster.MinInitialHp) / (float)(monster.MaxInitialHp - monster.MinInitialHp) - 0.5f) * 2f) : 0f);
				value3 = Math.Clamp(value3, 0f, 1f);
				float num = Math.Max(item2.Visuals.Bounds.Size.X, item2.Visuals.Bounds.Size.Y);
				float amount = Math.Clamp(Mathf.InverseLerp(250f, 100f, num), 0f, 1f);
				float num2 = float.Lerp(0.1f, 0.15f, amount);
				item2.SetScaleAndHue(1f + value3 * num2, Rng.Chaotic.NextFloat(0.05f));
			}
		}
	}

	public void RadialBlur(VfxPosition vfxPosition = VfxPosition.Center)
	{
		_radialBlur.Activate(vfxPosition);
	}

	public void ShakeOstyIfDead(Player owner)
	{
		_creatureNodes.FirstOrDefault((NCreature c) => c.Entity.Monster is Osty && c.Entity.PetOwner == owner)?.AnimShake();
	}

	public void PlaySplashVfx(Creature target, Color tint)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		NCreature creatureNode = GetCreatureNode(target);
		if (creatureNode != null)
		{
			Control combatVfxContainer = CombatVfxContainer;
			((Node)(object)combatVfxContainer).AddChildSafely((Node?)(object)NSplashVfx.Create(creatureNode.GetBottomOfHitbox(), tint));
			((Node)(object)combatVfxContainer).AddChildSafely((Node?)(object)NLiquidOverlayVfx.Create(target, tint));
		}
	}

	public void SetWaitingForOtherPlayersOverlayVisible(bool visible)
	{
		((CanvasItem)_waitingForOtherPlayersOverlay).Visible = visible;
	}

	private void OnProceedButtonPressed(NButton button)
	{
		_proceedButton.Disable();
		this.ProceedButtonPressed?.Invoke();
	}

	public void TransitionToActiveCombat(CombatRoom combatRoom)
	{
		if (Mode != CombatRoomMode.VisualOnly)
		{
			throw new InvalidOperationException($"Cannot transition to {"ActiveCombat"} from {Mode}.");
		}
		Mode = CombatRoomMode.ActiveCombat;
		_visuals = combatRoom;
		foreach (NCreature creatureNode in _creatureNodes)
		{
			creatureNode.Hitbox.FocusMode = (FocusModeEnum)2;
			SetCreatureIsInteractable(creatureNode.Entity, on: true);
		}
		SubscribeToCombatEvents();
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
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Expected O, but got Unknown
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		//IL_0288: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_030a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0330: Unknown result type (might be due to invalid IL or missing references)
		//IL_0358: Unknown result type (might be due to invalid IL or missing references)
		//IL_0363: Expected O, but got Unknown
		//IL_035e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0369: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(15);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._EnterTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._ExitTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SubscribeToCombatEvents, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AdjustCreatureScaleForAspectRatio, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.CreateAllyNodes, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.CreateEnemyNodes, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.RemoveCreatureNode, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("node"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UpdateCreatureNavigation, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnActiveScreenUpdated, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.EnableControllerNavigation, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.RandomizeEnemyScalesAndHues, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.RadialBlur, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("vfxPosition"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetWaitingForOtherPlayersOverlayVisible, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)1, StringName.op_Implicit("visible"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnProceedButtonPressed, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("button"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
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
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._EnterTree && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._EnterTree();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._ExitTree && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._ExitTree();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SubscribeToCombatEvents && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			SubscribeToCombatEvents();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.AdjustCreatureScaleForAspectRatio && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			AdjustCreatureScaleForAspectRatio();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.CreateAllyNodes && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			CreateAllyNodes();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.CreateEnemyNodes && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			CreateEnemyNodes();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.RemoveCreatureNode && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			RemoveCreatureNode(VariantUtils.ConvertTo<NCreature>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.UpdateCreatureNavigation && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			UpdateCreatureNavigation();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnActiveScreenUpdated && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnActiveScreenUpdated();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.EnableControllerNavigation && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			EnableControllerNavigation();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.RandomizeEnemyScalesAndHues && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			RandomizeEnemyScalesAndHues();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.RadialBlur && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			RadialBlur(VariantUtils.ConvertTo<VfxPosition>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetWaitingForOtherPlayersOverlayVisible && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			SetWaitingForOtherPlayersOverlayVisible(VariantUtils.ConvertTo<bool>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnProceedButtonPressed && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OnProceedButtonPressed(VariantUtils.ConvertTo<NButton>(ref ((NativeVariantPtrArgs)(ref args))[0]));
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
		if ((ref method) == MethodName._EnterTree)
		{
			return true;
		}
		if ((ref method) == MethodName._ExitTree)
		{
			return true;
		}
		if ((ref method) == MethodName.SubscribeToCombatEvents)
		{
			return true;
		}
		if ((ref method) == MethodName.AdjustCreatureScaleForAspectRatio)
		{
			return true;
		}
		if ((ref method) == MethodName.CreateAllyNodes)
		{
			return true;
		}
		if ((ref method) == MethodName.CreateEnemyNodes)
		{
			return true;
		}
		if ((ref method) == MethodName.RemoveCreatureNode)
		{
			return true;
		}
		if ((ref method) == MethodName.UpdateCreatureNavigation)
		{
			return true;
		}
		if ((ref method) == MethodName.OnActiveScreenUpdated)
		{
			return true;
		}
		if ((ref method) == MethodName.EnableControllerNavigation)
		{
			return true;
		}
		if ((ref method) == MethodName.RandomizeEnemyScalesAndHues)
		{
			return true;
		}
		if ((ref method) == MethodName.RadialBlur)
		{
			return true;
		}
		if ((ref method) == MethodName.SetWaitingForOtherPlayersOverlayVisible)
		{
			return true;
		}
		if ((ref method) == MethodName.OnProceedButtonPressed)
		{
			return true;
		}
		return ((Control)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName.Ui)
		{
			Ui = VariantUtils.ConvertTo<NCombatUi>(ref value);
			return true;
		}
		if ((ref name) == PropertyName.SceneContainer)
		{
			SceneContainer = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName.BgContainer)
		{
			BgContainer = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName.Background)
		{
			Background = VariantUtils.ConvertTo<NCombatBackground>(ref value);
			return true;
		}
		if ((ref name) == PropertyName.BackCombatVfxContainer)
		{
			BackCombatVfxContainer = VariantUtils.ConvertTo<Node>(ref value);
			return true;
		}
		if ((ref name) == PropertyName.CombatVfxContainer)
		{
			CombatVfxContainer = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName.CreatedMsec)
		{
			CreatedMsec = VariantUtils.ConvertTo<ulong>(ref value);
			return true;
		}
		if ((ref name) == PropertyName.Mode)
		{
			Mode = VariantUtils.ConvertTo<CombatRoomMode>(ref value);
			return true;
		}
		if ((ref name) == PropertyName.EncounterSlots)
		{
			EncounterSlots = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._allyContainer)
		{
			_allyContainer = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._enemyContainer)
		{
			_enemyContainer = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._radialBlur)
		{
			_radialBlur = VariantUtils.ConvertTo<NRadialBlurVfx>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._proceedButton)
		{
			_proceedButton = VariantUtils.ConvertTo<NProceedButton>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._waitingForOtherPlayersOverlay)
		{
			_waitingForOtherPlayersOverlay = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._window)
		{
			_window = VariantUtils.ConvertTo<Window>(ref value);
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
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName.Ui)
		{
			NCombatUi ui = Ui;
			value = VariantUtils.CreateFrom<NCombatUi>(ref ui);
			return true;
		}
		if ((ref name) == PropertyName.SceneContainer)
		{
			Control sceneContainer = SceneContainer;
			value = VariantUtils.CreateFrom<Control>(ref sceneContainer);
			return true;
		}
		if ((ref name) == PropertyName.BgContainer)
		{
			Control sceneContainer = BgContainer;
			value = VariantUtils.CreateFrom<Control>(ref sceneContainer);
			return true;
		}
		if ((ref name) == PropertyName.Background)
		{
			NCombatBackground background = Background;
			value = VariantUtils.CreateFrom<NCombatBackground>(ref background);
			return true;
		}
		if ((ref name) == PropertyName.ProceedButton)
		{
			NProceedButton proceedButton = ProceedButton;
			value = VariantUtils.CreateFrom<NProceedButton>(ref proceedButton);
			return true;
		}
		if ((ref name) == PropertyName.BackCombatVfxContainer)
		{
			Node backCombatVfxContainer = BackCombatVfxContainer;
			value = VariantUtils.CreateFrom<Node>(ref backCombatVfxContainer);
			return true;
		}
		if ((ref name) == PropertyName.CombatVfxContainer)
		{
			Control sceneContainer = CombatVfxContainer;
			value = VariantUtils.CreateFrom<Control>(ref sceneContainer);
			return true;
		}
		if ((ref name) == PropertyName.CreatedMsec)
		{
			ulong createdMsec = CreatedMsec;
			value = VariantUtils.CreateFrom<ulong>(ref createdMsec);
			return true;
		}
		if ((ref name) == PropertyName.Mode)
		{
			CombatRoomMode mode = Mode;
			value = VariantUtils.CreateFrom<CombatRoomMode>(ref mode);
			return true;
		}
		if ((ref name) == PropertyName.EncounterSlots)
		{
			Control sceneContainer = EncounterSlots;
			value = VariantUtils.CreateFrom<Control>(ref sceneContainer);
			return true;
		}
		if ((ref name) == PropertyName.DefaultFocusedControl)
		{
			Control sceneContainer = DefaultFocusedControl;
			value = VariantUtils.CreateFrom<Control>(ref sceneContainer);
			return true;
		}
		if ((ref name) == PropertyName.FocusedControlFromTopBar)
		{
			Control sceneContainer = FocusedControlFromTopBar;
			value = VariantUtils.CreateFrom<Control>(ref sceneContainer);
			return true;
		}
		if ((ref name) == PropertyName._allyContainer)
		{
			value = VariantUtils.CreateFrom<Control>(ref _allyContainer);
			return true;
		}
		if ((ref name) == PropertyName._enemyContainer)
		{
			value = VariantUtils.CreateFrom<Control>(ref _enemyContainer);
			return true;
		}
		if ((ref name) == PropertyName._radialBlur)
		{
			value = VariantUtils.CreateFrom<NRadialBlurVfx>(ref _radialBlur);
			return true;
		}
		if ((ref name) == PropertyName._proceedButton)
		{
			value = VariantUtils.CreateFrom<NProceedButton>(ref _proceedButton);
			return true;
		}
		if ((ref name) == PropertyName._waitingForOtherPlayersOverlay)
		{
			value = VariantUtils.CreateFrom<Control>(ref _waitingForOtherPlayersOverlay);
			return true;
		}
		if ((ref name) == PropertyName._window)
		{
			value = VariantUtils.CreateFrom<Window>(ref _window);
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
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName.Ui, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._allyContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._enemyContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.SceneContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.BgContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.Background, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._radialBlur, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._proceedButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.ProceedButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._waitingForOtherPlayersOverlay, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.BackCombatVfxContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.CombatVfxContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)2, PropertyName.CreatedMsec, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)2, PropertyName.Mode, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.EncounterSlots, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._window, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.DefaultFocusedControl, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.FocusedControlFromTopBar, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		StringName ui = PropertyName.Ui;
		NCombatUi ui2 = Ui;
		info.AddProperty(ui, Variant.From<NCombatUi>(ref ui2));
		StringName sceneContainer = PropertyName.SceneContainer;
		Control sceneContainer2 = SceneContainer;
		info.AddProperty(sceneContainer, Variant.From<Control>(ref sceneContainer2));
		StringName bgContainer = PropertyName.BgContainer;
		sceneContainer2 = BgContainer;
		info.AddProperty(bgContainer, Variant.From<Control>(ref sceneContainer2));
		StringName background = PropertyName.Background;
		NCombatBackground background2 = Background;
		info.AddProperty(background, Variant.From<NCombatBackground>(ref background2));
		StringName backCombatVfxContainer = PropertyName.BackCombatVfxContainer;
		Node backCombatVfxContainer2 = BackCombatVfxContainer;
		info.AddProperty(backCombatVfxContainer, Variant.From<Node>(ref backCombatVfxContainer2));
		StringName combatVfxContainer = PropertyName.CombatVfxContainer;
		sceneContainer2 = CombatVfxContainer;
		info.AddProperty(combatVfxContainer, Variant.From<Control>(ref sceneContainer2));
		StringName createdMsec = PropertyName.CreatedMsec;
		ulong createdMsec2 = CreatedMsec;
		info.AddProperty(createdMsec, Variant.From<ulong>(ref createdMsec2));
		StringName mode = PropertyName.Mode;
		CombatRoomMode mode2 = Mode;
		info.AddProperty(mode, Variant.From<CombatRoomMode>(ref mode2));
		StringName encounterSlots = PropertyName.EncounterSlots;
		sceneContainer2 = EncounterSlots;
		info.AddProperty(encounterSlots, Variant.From<Control>(ref sceneContainer2));
		info.AddProperty(PropertyName._allyContainer, Variant.From<Control>(ref _allyContainer));
		info.AddProperty(PropertyName._enemyContainer, Variant.From<Control>(ref _enemyContainer));
		info.AddProperty(PropertyName._radialBlur, Variant.From<NRadialBlurVfx>(ref _radialBlur));
		info.AddProperty(PropertyName._proceedButton, Variant.From<NProceedButton>(ref _proceedButton));
		info.AddProperty(PropertyName._waitingForOtherPlayersOverlay, Variant.From<Control>(ref _waitingForOtherPlayersOverlay));
		info.AddProperty(PropertyName._window, Variant.From<Window>(ref _window));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName.Ui, ref val))
		{
			Ui = ((Variant)(ref val)).As<NCombatUi>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName.SceneContainer, ref val2))
		{
			SceneContainer = ((Variant)(ref val2)).As<Control>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName.BgContainer, ref val3))
		{
			BgContainer = ((Variant)(ref val3)).As<Control>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName.Background, ref val4))
		{
			Background = ((Variant)(ref val4)).As<NCombatBackground>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName.BackCombatVfxContainer, ref val5))
		{
			BackCombatVfxContainer = ((Variant)(ref val5)).As<Node>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName.CombatVfxContainer, ref val6))
		{
			CombatVfxContainer = ((Variant)(ref val6)).As<Control>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName.CreatedMsec, ref val7))
		{
			CreatedMsec = ((Variant)(ref val7)).As<ulong>();
		}
		Variant val8 = default(Variant);
		if (info.TryGetProperty(PropertyName.Mode, ref val8))
		{
			Mode = ((Variant)(ref val8)).As<CombatRoomMode>();
		}
		Variant val9 = default(Variant);
		if (info.TryGetProperty(PropertyName.EncounterSlots, ref val9))
		{
			EncounterSlots = ((Variant)(ref val9)).As<Control>();
		}
		Variant val10 = default(Variant);
		if (info.TryGetProperty(PropertyName._allyContainer, ref val10))
		{
			_allyContainer = ((Variant)(ref val10)).As<Control>();
		}
		Variant val11 = default(Variant);
		if (info.TryGetProperty(PropertyName._enemyContainer, ref val11))
		{
			_enemyContainer = ((Variant)(ref val11)).As<Control>();
		}
		Variant val12 = default(Variant);
		if (info.TryGetProperty(PropertyName._radialBlur, ref val12))
		{
			_radialBlur = ((Variant)(ref val12)).As<NRadialBlurVfx>();
		}
		Variant val13 = default(Variant);
		if (info.TryGetProperty(PropertyName._proceedButton, ref val13))
		{
			_proceedButton = ((Variant)(ref val13)).As<NProceedButton>();
		}
		Variant val14 = default(Variant);
		if (info.TryGetProperty(PropertyName._waitingForOtherPlayersOverlay, ref val14))
		{
			_waitingForOtherPlayersOverlay = ((Variant)(ref val14)).As<Control>();
		}
		Variant val15 = default(Variant);
		if (info.TryGetProperty(PropertyName._window, ref val15))
		{
			_window = ((Variant)(ref val15)).As<Window>();
		}
	}
}
