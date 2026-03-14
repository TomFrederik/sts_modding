using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Entities.UI;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.HoverTips;
using MegaCrit.Sts2.Core.Nodes.Multiplayer;
using MegaCrit.Sts2.Core.Nodes.Orbs;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Screens.ScreenContext;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.Settings;
using MegaCrit.Sts2.Core.TestSupport;

namespace MegaCrit.Sts2.Core.Nodes.Combat;

[ScriptPath("res://src/Core/Nodes/Combat/NCreature.cs")]
public class NCreature : Control
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName _EnterTree = StringName.op_Implicit("_EnterTree");

		public static readonly StringName _ExitTree = StringName.op_Implicit("_ExitTree");

		public static readonly StringName ConnectSpineAnimatorSignals = StringName.op_Implicit("ConnectSpineAnimatorSignals");

		public static readonly StringName UpdateBounds = StringName.op_Implicit("UpdateBounds");

		public static readonly StringName UpdateNavigation = StringName.op_Implicit("UpdateNavigation");

		public static readonly StringName OnFocus = StringName.op_Implicit("OnFocus");

		public static readonly StringName OnUnfocus = StringName.op_Implicit("OnUnfocus");

		public static readonly StringName OnTargetingStarted = StringName.op_Implicit("OnTargetingStarted");

		public static readonly StringName SetRemotePlayerFocused = StringName.op_Implicit("SetRemotePlayerFocused");

		public static readonly StringName HideHoverTips = StringName.op_Implicit("HideHoverTips");

		public static readonly StringName SetAnimationTrigger = StringName.op_Implicit("SetAnimationTrigger");

		public static readonly StringName GetCurrentAnimationLength = StringName.op_Implicit("GetCurrentAnimationLength");

		public static readonly StringName GetCurrentAnimationTimeRemaining = StringName.op_Implicit("GetCurrentAnimationTimeRemaining");

		public static readonly StringName ToggleIsInteractable = StringName.op_Implicit("ToggleIsInteractable");

		public static readonly StringName AnimDisableUi = StringName.op_Implicit("AnimDisableUi");

		public static readonly StringName AnimEnableUi = StringName.op_Implicit("AnimEnableUi");

		public static readonly StringName StartDeathAnim = StringName.op_Implicit("StartDeathAnim");

		public static readonly StringName StartReviveAnim = StringName.op_Implicit("StartReviveAnim");

		public static readonly StringName AnimTempRevive = StringName.op_Implicit("AnimTempRevive");

		public static readonly StringName ImmediatelySetIdle = StringName.op_Implicit("ImmediatelySetIdle");

		public static readonly StringName AnimHideIntent = StringName.op_Implicit("AnimHideIntent");

		public static readonly StringName SetScaleAndHue = StringName.op_Implicit("SetScaleAndHue");

		public static readonly StringName ScaleTo = StringName.op_Implicit("ScaleTo");

		public static readonly StringName SetDefaultScaleTo = StringName.op_Implicit("SetDefaultScaleTo");

		public static readonly StringName OstyScaleToSize = StringName.op_Implicit("OstyScaleToSize");

		public static readonly StringName AnimShake = StringName.op_Implicit("AnimShake");

		public static readonly StringName DoScaleTween = StringName.op_Implicit("DoScaleTween");

		public static readonly StringName SetOrbManagerPosition = StringName.op_Implicit("SetOrbManagerPosition");

		public static readonly StringName GetTopOfHitbox = StringName.op_Implicit("GetTopOfHitbox");

		public static readonly StringName GetBottomOfHitbox = StringName.op_Implicit("GetBottomOfHitbox");

		public static readonly StringName ShowMultiselectReticle = StringName.op_Implicit("ShowMultiselectReticle");

		public static readonly StringName HideMultiselectReticle = StringName.op_Implicit("HideMultiselectReticle");

		public static readonly StringName ShowSingleSelectReticle = StringName.op_Implicit("ShowSingleSelectReticle");

		public static readonly StringName HideSingleSelectReticle = StringName.op_Implicit("HideSingleSelectReticle");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName Hitbox = StringName.op_Implicit("Hitbox");

		public static readonly StringName OrbManager = StringName.op_Implicit("OrbManager");

		public static readonly StringName IsInteractable = StringName.op_Implicit("IsInteractable");

		public static readonly StringName VfxSpawnPosition = StringName.op_Implicit("VfxSpawnPosition");

		public static readonly StringName Visuals = StringName.op_Implicit("Visuals");

		public static readonly StringName Body = StringName.op_Implicit("Body");

		public static readonly StringName IntentContainer = StringName.op_Implicit("IntentContainer");

		public static readonly StringName IsPlayingDeathAnimation = StringName.op_Implicit("IsPlayingDeathAnimation");

		public static readonly StringName HasSpineAnimation = StringName.op_Implicit("HasSpineAnimation");

		public static readonly StringName IsFocused = StringName.op_Implicit("IsFocused");

		public static readonly StringName PlayerIntentHandler = StringName.op_Implicit("PlayerIntentHandler");

		public static readonly StringName _stateDisplay = StringName.op_Implicit("_stateDisplay");

		public static readonly StringName _intentFadeTween = StringName.op_Implicit("_intentFadeTween");

		public static readonly StringName _shakeTween = StringName.op_Implicit("_shakeTween");

		public static readonly StringName _isRemotePlayerOrPet = StringName.op_Implicit("_isRemotePlayerOrPet");

		public static readonly StringName _tempScale = StringName.op_Implicit("_tempScale");

		public static readonly StringName _scaleTween = StringName.op_Implicit("_scaleTween");

		public static readonly StringName _isInMultiselect = StringName.op_Implicit("_isInMultiselect");

		public static readonly StringName _selectionReticle = StringName.op_Implicit("_selectionReticle");
	}

	public class SignalName : SignalName
	{
	}

	private static readonly string _scenePath = SceneHelper.GetScenePath("combat/creature");

	private NCreatureStateDisplay _stateDisplay;

	private Tween? _intentFadeTween;

	private Tween? _shakeTween;

	private CreatureAnimator? _spineAnimator;

	private bool _isRemotePlayerOrPet;

	private float _tempScale = 1f;

	private Tween? _scaleTween;

	private bool _isInMultiselect;

	private NSelectionReticle _selectionReticle;

	public static IEnumerable<string> AssetPaths => new _003C_003Ez__ReadOnlySingleElementList<string>(_scenePath);

	public Task? DeathAnimationTask { get; set; }

	public CancellationTokenSource DeathAnimCancelToken { get; } = new CancellationTokenSource();

	public Control Hitbox { get; private set; }

	public NOrbManager? OrbManager { get; private set; }

	public bool IsInteractable { get; private set; } = true;

	public Creature Entity { get; private set; }

	public Vector2 VfxSpawnPosition => ((Node2D)Visuals.VfxSpawnPosition).GlobalPosition;

	public NCreatureVisuals Visuals { get; private set; }

	public Node2D Body => Visuals.Body;

	public Control IntentContainer { get; private set; }

	public bool IsPlayingDeathAnimation => DeathAnimationTask != null;

	public bool HasSpineAnimation => Visuals.HasSpineAnimation;

	public MegaSprite? SpineController => Visuals.SpineBody;

	public bool IsFocused { get; private set; }

	public NMultiplayerPlayerIntentHandler? PlayerIntentHandler { get; private set; }

	public T? GetSpecialNode<T>(string name) where T : Node
	{
		return ((Node)Visuals).GetNode<T>(NodePath.op_Implicit(name));
	}

	public static NCreature? Create(Creature entity)
	{
		if (TestMode.IsOn)
		{
			return null;
		}
		NCreature nCreature = PreloadManager.Cache.GetScene(_scenePath).Instantiate<NCreature>((GenEditState)0);
		nCreature.Entity = entity;
		nCreature.Visuals = entity.CreateVisuals();
		return nCreature;
	}

	public override void _Ready()
	{
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		_stateDisplay = ((Node)this).GetNode<NCreatureStateDisplay>(NodePath.op_Implicit("%HealthBar"));
		IntentContainer = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%Intents"));
		Hitbox = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%Hitbox"));
		_selectionReticle = ((Node)this).GetNode<NSelectionReticle>(NodePath.op_Implicit("%SelectionReticle"));
		((GodotObject)Hitbox).Connect(SignalName.FocusEntered, Callable.From((Action)OnFocus), 0u);
		((GodotObject)Hitbox).Connect(SignalName.FocusExited, Callable.From((Action)OnUnfocus), 0u);
		((GodotObject)Hitbox).Connect(SignalName.MouseEntered, Callable.From((Action)OnFocus), 0u);
		((GodotObject)Hitbox).Connect(SignalName.MouseExited, Callable.From((Action)OnUnfocus), 0u);
		if (Entity.IsPlayer)
		{
			OrbManager = NOrbManager.Create(this, LocalContext.IsMe(Entity));
			((Node)(object)this).AddChildSafely((Node?)(object)OrbManager);
			UpdateNavigation();
		}
		if (Entity.IsPlayer)
		{
			CombatState? combatState = Entity.CombatState;
			if (combatState != null && combatState.RunState.Players.Count > 1)
			{
				PlayerIntentHandler = NMultiplayerPlayerIntentHandler.Create(Entity.Player);
				if (PlayerIntentHandler != null)
				{
					((Node)(object)IntentContainer).AddChildSafely((Node?)(object)PlayerIntentHandler);
					((CanvasItem)IntentContainer).Modulate = Colors.White;
				}
			}
		}
		((Node)(object)this).AddChildSafely((Node?)(object)Visuals);
		((Node)this).MoveChild((Node)(object)Visuals, 0);
		((Node2D)Visuals).Position = Vector2.Zero;
		_stateDisplay.SetCreature(Entity);
		bool flag = Entity.PetOwner != null && !LocalContext.IsMe(Entity.PetOwner);
		bool flag2 = Entity.IsPlayer && !LocalContext.IsMe(Entity);
		_isRemotePlayerOrPet = flag2 || flag;
		if (_isRemotePlayerOrPet)
		{
			_stateDisplay.HideImmediately();
		}
		else
		{
			bool flag3 = NCombatRoom.Instance != null && Time.GetTicksMsec() - NCombatRoom.Instance.CreatedMsec < 1000;
			_stateDisplay.AnimateIn(flag3 ? HealthBarAnimMode.SpawnedAtCombatStart : HealthBarAnimMode.SpawnedDuringCombat);
		}
		if (HasSpineAnimation)
		{
			if (Entity.Player != null)
			{
				_spineAnimator = Entity.Player.Character.GenerateAnimator(SpineController);
			}
			else
			{
				_spineAnimator = Entity.Monster.GenerateAnimator(SpineController);
				Visuals.SetUpSkin(Entity.Monster);
			}
			ConnectSpineAnimatorSignals();
			if (Entity.IsDead)
			{
				SetAnimationTrigger("Dead");
				MegaTrackEntry current = Visuals.SpineBody.GetAnimationState().GetCurrent(0);
				current.SetTrackTime(current.GetAnimationEnd());
			}
		}
		SetOrbManagerPosition();
		if (Entity.Monster != null)
		{
			ToggleIsInteractable(Entity.Monster.IsHealthBarVisible);
		}
		UpdateBounds((Node)(object)Visuals);
	}

	public override void _EnterTree()
	{
		((Node)this)._EnterTree();
		CombatManager.Instance.CombatEnded += OnCombatEnded;
		Entity.PowerApplied += OnPowerApplied;
		Entity.PowerRemoved += OnPowerRemoved;
		Entity.PowerIncreased += OnPowerIncreased;
		foreach (PowerModel power in Entity.Powers)
		{
			SubscribeToPower(power);
		}
		ConnectSpineAnimatorSignals();
	}

	public override void _ExitTree()
	{
		((Node)this)._ExitTree();
		DeathAnimCancelToken.Cancel();
		CombatManager.Instance.CombatEnded -= OnCombatEnded;
		Entity.PowerApplied -= OnPowerApplied;
		Entity.PowerRemoved -= OnPowerRemoved;
		Entity.PowerIncreased -= OnPowerIncreased;
		foreach (PowerModel power in Entity.Powers)
		{
			UnsubscribeFromPower(power);
		}
		if (_spineAnimator != null)
		{
			_spineAnimator.BoundsUpdated -= UpdateBounds;
		}
		CombatManager.Instance.StateTracker.CombatStateChanged -= ShowCreatureHoverTips;
	}

	private void ConnectSpineAnimatorSignals()
	{
		if (_spineAnimator != null)
		{
			_spineAnimator.BoundsUpdated -= UpdateBounds;
			_spineAnimator.BoundsUpdated += UpdateBounds;
		}
	}

	private void UpdateBounds(string boundsNodeName)
	{
		UpdateBounds((Node)(object)((Node)Visuals).GetNode<Control>(NodePath.op_Implicit(boundsNodeName)));
	}

	private void UpdateBounds(Node boundsContainer)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		Control node = boundsContainer.GetNode<Control>(NodePath.op_Implicit("Bounds"));
		Vector2 size = node.Size * ((Node2D)Visuals).Scale / _tempScale;
		Vector2 val = (node.GlobalPosition - ((Control)this).GlobalPosition) / _tempScale;
		Hitbox.Size = size;
		Hitbox.GlobalPosition = ((Control)this).GlobalPosition + val;
		((Control)_selectionReticle).Size = size;
		((Control)_selectionReticle).GlobalPosition = ((Control)this).GlobalPosition + val;
		((Control)_selectionReticle).PivotOffset = ((Control)_selectionReticle).Size / 2f;
		IntentContainer.Position = ((Node2D)boundsContainer.GetNode<Marker2D>(NodePath.op_Implicit("IntentPos"))).Position - IntentContainer.Size / 2f;
		_stateDisplay.SetCreatureBounds(Hitbox);
	}

	public void UpdateNavigation()
	{
		if (OrbManager != null)
		{
			Hitbox.FocusNeighborTop = ((Node)OrbManager.DefaultFocusOwner).GetPath();
		}
	}

	public Task UpdateIntent(IEnumerable<Creature> targets)
	{
		if (Entity.Monster == null)
		{
			throw new InvalidOperationException("Only valid on monsters.");
		}
		IReadOnlyList<AbstractIntent> intents = Entity.Monster.NextMove.Intents;
		int i;
		for (i = 0; i < intents.Count && i < ((Node)IntentContainer).GetChildCount(false); i++)
		{
			NIntent child = ((Node)IntentContainer).GetChild<NIntent>(i, false);
			child.SetFrozen(isFrozen: false);
			child.UpdateIntent(intents[i], targets, Entity);
		}
		float num = (float)((object)this).GetHashCode() / 100f;
		for (; i < intents.Count; i++)
		{
			NIntent nIntent = NIntent.Create(num + (float)i * 0.3f);
			((Node)(object)IntentContainer).AddChildSafely((Node?)(object)nIntent);
			nIntent.UpdateIntent(intents[i], targets, Entity);
		}
		List<Node> list = ((IEnumerable<Node>)((Node)IntentContainer).GetChildren(false)).TakeLast(((Node)IntentContainer).GetChildCount(false) - i).ToList();
		foreach (Node item in list)
		{
			((Node)(object)IntentContainer).RemoveChildSafely(item);
			item.QueueFreeSafely();
		}
		return Task.CompletedTask;
	}

	public async Task PerformIntent()
	{
		foreach (NIntent item in ((IEnumerable)((Node)IntentContainer).GetChildren(false)).OfType<NIntent>())
		{
			item.PlayPerform();
			item.SetFrozen(isFrozen: true);
		}
		if (SaveManager.Instance.PrefsSave.FastMode == FastModeType.Instant)
		{
			((CanvasItem)IntentContainer).Modulate = new Color(((CanvasItem)IntentContainer).Modulate.R, ((CanvasItem)IntentContainer).Modulate.G, ((CanvasItem)IntentContainer).Modulate.B, 0f);
			return;
		}
		AnimHideIntent(0.4f);
		await Cmd.CustomScaledWait(0.25f, 0.4f);
	}

	public async Task RefreshIntents()
	{
		await UpdateIntent(Entity.CombatState.Players.Select((Player p) => p.Creature));
		await RevealIntents();
	}

	private Task RevealIntents()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		((CanvasItem)IntentContainer).Modulate = Colors.Transparent;
		Tween? intentFadeTween = _intentFadeTween;
		if (intentFadeTween != null)
		{
			intentFadeTween.Kill();
		}
		_intentFadeTween = ((Node)this).CreateTween().SetParallel(true);
		_intentFadeTween.TweenProperty((GodotObject)(object)IntentContainer, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(1f), 1.0).SetDelay((double)Rng.Chaotic.NextFloat(0f, 0.3f));
		return Task.CompletedTask;
	}

	private void OnFocus()
	{
		if (IsFocused)
		{
			return;
		}
		IsFocused = true;
		if (_isRemotePlayerOrPet)
		{
			_stateDisplay.AnimateIn(HealthBarAnimMode.FromHidden);
			((CanvasItem)_stateDisplay).ZIndex = 1;
			Player me = LocalContext.GetMe(Entity.CombatState);
			NCombatRoom.Instance?.GetCreatureNode(me?.Creature)?.SetRemotePlayerFocused(remotePlayerFocused: true);
		}
		else
		{
			_stateDisplay.ShowNameplate();
		}
		NRun.Instance.GlobalUi.MultiplayerPlayerContainer.HighlightPlayer(Entity.Player);
		if (NTargetManager.Instance.IsInSelection)
		{
			NTargetManager.Instance.OnNodeHovered((Node)(object)this);
			return;
		}
		if (NControllerManager.Instance.IsUsingController)
		{
			ShowSingleSelectReticle();
		}
		ShowHoverTips(Entity.HoverTips);
		CombatManager.Instance.StateTracker.CombatStateChanged += ShowCreatureHoverTips;
	}

	private void OnUnfocus()
	{
		IsFocused = false;
		HideSingleSelectReticle();
		if (_isRemotePlayerOrPet)
		{
			_stateDisplay.AnimateOut();
			Player me = LocalContext.GetMe(Entity.CombatState);
			NCombatRoom.Instance?.GetCreatureNode(me?.Creature)?.SetRemotePlayerFocused(remotePlayerFocused: false);
		}
		else
		{
			_stateDisplay.HideNameplate();
		}
		NRun.Instance.GlobalUi.MultiplayerPlayerContainer.UnhighlightPlayer(Entity.Player);
		NTargetManager.Instance.OnNodeUnhovered((Node)(object)this);
		CombatManager.Instance.StateTracker.CombatStateChanged -= ShowCreatureHoverTips;
		HideHoverTips();
	}

	public void OnTargetingStarted()
	{
		if (IsFocused)
		{
			NTargetManager.Instance.OnNodeHovered((Node)(object)this);
			CombatManager.Instance.StateTracker.CombatStateChanged -= ShowCreatureHoverTips;
			HideHoverTips();
		}
	}

	private void ShowCreatureHoverTips(CombatState _)
	{
		if (Entity.CombatState != null)
		{
			ShowHoverTips(Entity.HoverTips);
		}
	}

	public void ShowHoverTips(IEnumerable<IHoverTip> hoverTips)
	{
		if (!NCombatRoom.Instance.Ui.Hand.InCardPlay)
		{
			HideHoverTips();
			NHoverTipSet.CreateAndShow(Hitbox, hoverTips, HoverTip.GetHoverTipAlignment((Control)(object)this, 0.5f));
		}
	}

	public void SetRemotePlayerFocused(bool remotePlayerFocused)
	{
		if (!LocalContext.IsMe(Entity))
		{
			throw new InvalidOperationException("This should only be called on the local player's creature node!");
		}
		if (remotePlayerFocused)
		{
			_stateDisplay.AnimateOut();
		}
		else if (Entity.IsAlive)
		{
			_stateDisplay.AnimateIn(HealthBarAnimMode.FromHidden);
		}
	}

	public void HideHoverTips()
	{
		NHoverTipSet.Remove(Hitbox);
	}

	private void SubscribeToPower(PowerModel power)
	{
		power.Flashed += OnPowerFlashed;
	}

	private void UnsubscribeFromPower(PowerModel power)
	{
		power.Flashed -= OnPowerFlashed;
	}

	private void OnPowerApplied(PowerModel power)
	{
		SubscribeToPower(power);
	}

	private void OnPowerIncreased(PowerModel power, int amount, bool silent)
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		if (silent || !CombatManager.Instance.IsInProgress)
		{
			return;
		}
		NPowerAppliedVfx vfx = NPowerAppliedVfx.Create(power, amount);
		if (vfx != null)
		{
			Callable val = Callable.From((Action)delegate
			{
				((Node)(object)NCombatRoom.Instance?.CombatVfxContainer).AddChildSafely((Node?)(object)vfx);
			});
			((Callable)(ref val)).CallDeferred(Array.Empty<Variant>());
		}
		if (power.ShouldPlayVfx)
		{
			SfxCmd.Play((power.GetTypeForAmount(amount) == PowerType.Buff) ? "event:/sfx/buff" : "event:/sfx/debuff");
		}
		if (power.GetTypeForAmount(power.Amount) == PowerType.Debuff)
		{
			AnimShake();
		}
	}

	private void OnPowerRemoved(PowerModel power)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		NPowerRemovedVfx vfx = NPowerRemovedVfx.Create(power);
		if (vfx != null)
		{
			Callable val = Callable.From((Action)delegate
			{
				((Node)(object)NCombatRoom.Instance?.CombatVfxContainer).AddChildSafely((Node?)(object)vfx);
			});
			((Callable)(ref val)).CallDeferred(Array.Empty<Variant>());
		}
		UnsubscribeFromPower(power);
	}

	private void OnPowerFlashed(PowerModel power)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		NPowerFlashVfx vfx = NPowerFlashVfx.Create(power);
		if (vfx != null)
		{
			Callable val = Callable.From((Action)delegate
			{
				((Node)(object)NCombatRoom.Instance?.CombatVfxContainer).AddChildSafely((Node?)(object)vfx);
			});
			((Callable)(ref val)).CallDeferred(Array.Empty<Variant>());
		}
	}

	private void OnCombatEnded(CombatRoom _)
	{
		AnimHideIntent();
		OrbManager?.ClearOrbs();
	}

	public void SetAnimationTrigger(string trigger)
	{
		_spineAnimator?.SetTrigger(trigger);
	}

	public float GetCurrentAnimationLength()
	{
		return SpineController.GetAnimationState().GetCurrent(0).GetAnimation()
			.GetDuration();
	}

	public float GetCurrentAnimationTimeRemaining()
	{
		MegaTrackEntry current = SpineController.GetAnimationState().GetCurrent(0);
		return current.GetTrackComplete() - current.GetTrackTime();
	}

	public void ToggleIsInteractable(bool on)
	{
		IsInteractable = on;
		((CanvasItem)_stateDisplay).Visible = !NCombatUi.IsDebugHidingHpBar && on;
		Hitbox.MouseFilter = (MouseFilterEnum)(on ? 0 : 2);
	}

	public Tween AnimDisableUi()
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		Tween val = ((Node)this).CreateTween();
		if (!((Node)this).IsNodeReady())
		{
			val.TweenInterval(0.0);
			return val;
		}
		val.TweenProperty((GodotObject)(object)_stateDisplay, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0f), 0.5).SetDelay(0.5).SetEase((EaseType)1)
			.SetTrans((TransitionType)5);
		return val;
	}

	public Tween AnimEnableUi()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		Tween val = ((Node)this).CreateTween();
		val.TweenProperty((GodotObject)(object)_stateDisplay, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(1f), 0.5).SetDelay(0.5).SetEase((EaseType)1)
			.SetTrans((TransitionType)5);
		return val;
	}

	public float StartDeathAnim(bool shouldRemove)
	{
		if (Hitbox.HasFocus())
		{
			ActiveScreenContext.Instance.FocusOnDefaultControl();
		}
		Hitbox.FocusMode = (FocusModeEnum)0;
		foreach (NIntent item in ((IEnumerable)((Node)IntentContainer).GetChildren(false)).OfType<NIntent>())
		{
			item.SetFrozen(isFrozen: true);
		}
		Task deathAnimationTask = DeathAnimationTask;
		if (deathAnimationTask != null && !deathAnimationTask.IsCompleted)
		{
			return 0f;
		}
		float num = 0f;
		if (_spineAnimator != null)
		{
			MonsterModel? monster = Entity.Monster;
			if (monster != null && monster.HasDeathSfx)
			{
				SfxCmd.PlayDeath(Entity.Monster);
			}
			if (Entity.Player != null)
			{
				SfxCmd.PlayDeath(Entity.Player);
			}
			SetAnimationTrigger("Dead");
			num = GetCurrentAnimationLength();
		}
		DeathAnimationTask = AnimDie(shouldRemove, DeathAnimCancelToken.Token);
		TaskHelper.RunSafely(DeathAnimationTask);
		MonsterModel monster2 = Entity.Monster;
		if (monster2 != null && monster2.HasDeathAnimLengthOverride)
		{
			return Entity.Monster.DeathAnimLengthOverride;
		}
		return Mathf.Min(num, 30f);
	}

	public void StartReviveAnim()
	{
		CreatureAnimator? spineAnimator = _spineAnimator;
		if (spineAnimator != null && spineAnimator.HasTrigger("Revive"))
		{
			SetAnimationTrigger("Revive");
		}
		else if (Entity.IsPlayer)
		{
			AnimTempRevive();
		}
		if (!_isRemotePlayerOrPet)
		{
			AnimEnableUi();
		}
		Hitbox.MouseFilter = (MouseFilterEnum)0;
	}

	private void AnimTempRevive()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		Tween val = ((Node)this).CreateTween();
		val.TweenProperty((GodotObject)(object)Visuals, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0f), 0.20000000298023224);
		val.TweenCallback(Callable.From((Action)ImmediatelySetIdle));
		val.TweenProperty((GodotObject)(object)Visuals, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(1f), 0.20000000298023224);
	}

	private void ImmediatelySetIdle()
	{
		_spineAnimator?.SetTrigger("Idle");
		MegaTrackEntry current = Visuals.SpineBody.GetAnimationState().GetCurrent(0);
		current.SetMixDuration(0f);
		current.SetTrackTime(current.GetAnimationEnd());
	}

	private async Task AnimDie(bool shouldRemove, CancellationToken cancelToken)
	{
		Tween disableUiTween = AnimDisableUi();
		Hitbox.MouseFilter = (MouseFilterEnum)2;
		if (!RunManager.Instance.IsSinglePlayerOrFakeMultiplayer)
		{
			OrbManager?.ClearOrbs();
		}
		if (shouldRemove)
		{
			AnimHideIntent();
		}
		if (_spineAnimator != null)
		{
			float seconds = Math.Min(GetCurrentAnimationTimeRemaining() + 0.5f, 20f);
			await Cmd.Wait(seconds, cancelToken, ignoreCombatEnd: true);
			if (cancelToken.IsCancellationRequested)
			{
				return;
			}
		}
		else
		{
			MonsterModel monster = Entity.Monster;
			if (monster != null && monster.HasDeathAnimLengthOverride)
			{
				await Cmd.Wait(Entity.Monster.DeathAnimLengthOverride, cancelToken, ignoreCombatEnd: true);
			}
		}
		if (shouldRemove)
		{
			Task fadeVfx = null;
			MonsterModel monster = Entity.Monster;
			if (monster != null && monster.ShouldFadeAfterDeath && ((CanvasItem)Body).IsVisibleInTree())
			{
				NMonsterDeathVfx nMonsterDeathVfx = NMonsterDeathVfx.Create(this, cancelToken);
				Node parent = ((Node)this).GetParent();
				parent.AddChildSafely((Node?)(object)nMonsterDeathVfx);
				parent.MoveChild((Node)(object)nMonsterDeathVfx, ((Node)this).GetIndex(false));
				fadeVfx = nMonsterDeathVfx?.PlayVfx();
			}
			if (SaveManager.Instance.PrefsSave.FastMode != FastModeType.Instant)
			{
				if (disableUiTween.IsValid() && disableUiTween.IsRunning())
				{
					await ((GodotObject)this).ToSignal((GodotObject)(object)disableUiTween, SignalName.Finished);
				}
				foreach (IDeathDelayer item in ((Node)(object)this).GetChildrenRecursive<IDeathDelayer>())
				{
					await item.GetDelayTask();
				}
			}
			if (fadeVfx != null)
			{
				await fadeVfx;
			}
			((Node)(object)this).QueueFreeSafely();
		}
		if (Entity.Monster is Osty)
		{
			OstyScaleToSize(0f, 0.75f);
		}
	}

	public void AnimHideIntent(float delay = 0f)
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		Tween? intentFadeTween = _intentFadeTween;
		if (intentFadeTween != null)
		{
			intentFadeTween.Kill();
		}
		_intentFadeTween = ((Node)this).CreateTween().SetParallel(true);
		PropertyTweener val = _intentFadeTween.TweenProperty((GodotObject)(object)IntentContainer, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0f), 0.5);
		if (delay > 0f)
		{
			val.SetDelay((double)delay);
		}
	}

	public void SetScaleAndHue(float scale, float hue)
	{
		Visuals.SetScaleAndHue(scale, hue);
		UpdateBounds((Node)(object)Visuals);
	}

	public void ScaleTo(float size, float duration)
	{
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		if (!Entity.IsMonster || Entity.Monster.CanChangeScale)
		{
			_tempScale = size;
			Tween? scaleTween = _scaleTween;
			if (scaleTween != null)
			{
				scaleTween.Kill();
			}
			_scaleTween = ((Node)this).CreateTween();
			_scaleTween.TweenMethod(Callable.From<Vector2>((Action<Vector2>)DoScaleTween), Variant.op_Implicit(((Node2D)Visuals).Scale), Variant.op_Implicit(Vector2.One * _tempScale * Visuals.DefaultScale), (double)duration).SetEase((EaseType)2).SetTrans((TransitionType)1);
		}
	}

	public void SetDefaultScaleTo(float size, float duration)
	{
		if (!Entity.IsMonster || Entity.Monster.CanChangeScale)
		{
			Visuals.DefaultScale = size;
			ScaleTo(_tempScale, duration);
		}
	}

	public void OstyScaleToSize(float ostyHealth, float duration)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		float num = Mathf.Lerp(Osty.ScaleRange.X, Osty.ScaleRange.Y, Mathf.Clamp(ostyHealth / 150f, 0f, 1f));
		NCreature nCreature = NCombatRoom.Instance?.GetCreatureNode(Entity.PetOwner.Creature);
		_scaleTween = ((Node)this).CreateTween();
		_scaleTween.TweenProperty((GodotObject)(object)Visuals, NodePath.op_Implicit("scale"), Variant.op_Implicit(Vector2.One * num * Visuals.DefaultScale), (double)duration).SetEase((EaseType)2).SetTrans((TransitionType)1);
		if (LocalContext.IsMe(Entity.PetOwner))
		{
			_scaleTween.Parallel().TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("position"), Variant.op_Implicit(((Control)nCreature).Position + GetOstyOffsetFromPlayer(Entity)), (double)duration);
		}
		_scaleTween.TweenCallback(Callable.From((Action)delegate
		{
			UpdateBounds((Node)(object)Visuals);
		}));
	}

	public static Vector2 GetOstyOffsetFromPlayer(Creature osty)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		NCreature nCreature = NCombatRoom.Instance?.GetCreatureNode(osty.PetOwner.Creature);
		Vector2 val = Vector2.Right * nCreature.Hitbox.Size.X * 0.5f;
		Vector2 minOffset = Osty.MinOffset;
		return val + ((Vector2)(ref minOffset)).Lerp(Osty.MaxOffset, Mathf.Clamp((float)osty.MaxHp / 150f, 0f, 1f));
	}

	public void AnimShake()
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		if ((_shakeTween == null || !_shakeTween.IsRunning()) && !Visuals.IsPlayingHurtAnimation())
		{
			((Node2D)Visuals).Position = Vector2.Zero;
			_shakeTween = ((Node)this).CreateTween();
			_shakeTween.TweenMethod(Callable.From<float>((Action<float>)delegate(float t)
			{
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_0010: Unknown result type (might be due to invalid IL or missing references)
				//IL_0021: Unknown result type (might be due to invalid IL or missing references)
				//IL_0032: Unknown result type (might be due to invalid IL or missing references)
				((Node2D)Visuals).Position = Vector2.Right * 10f * Mathf.Sin(t * 4f) * Mathf.Sin(t / 2f);
			}), Variant.op_Implicit(0f), Variant.op_Implicit((float)Math.PI * 2f), 1.0).SetEase((EaseType)1).SetTrans((TransitionType)7);
		}
	}

	private void DoScaleTween(Vector2 scale)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		((Node2D)Visuals).Scale = scale;
		SetOrbManagerPosition();
	}

	private void SetOrbManagerPosition()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		if (OrbManager != null)
		{
			NOrbManager? orbManager = OrbManager;
			Vector2 scale2;
			if (!(((Node2D)Visuals).Scale.X > 1f))
			{
				Vector2 scale = ((Node2D)Visuals).Scale;
				scale2 = ((Vector2)(ref scale)).Lerp(Vector2.One, 0.5f);
			}
			else
			{
				scale2 = Vector2.One;
			}
			((Control)orbManager).Scale = scale2;
			((Control)OrbManager).Position = ((Node2D)Visuals.OrbPosition).Position * Mathf.Min(((Node2D)Visuals).Scale.X, 1.25f);
			if (!OrbManager.IsLocal)
			{
				NOrbManager? orbManager2 = OrbManager;
				((Control)orbManager2).Position = ((Control)orbManager2).Position + Vector2.Up * 50f;
			}
		}
	}

	public Vector2 GetTopOfHitbox()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		return Hitbox.GlobalPosition + new Vector2(Hitbox.Size.X * 0.5f, 0f);
	}

	public Vector2 GetBottomOfHitbox()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		return Hitbox.GlobalPosition + new Vector2(Hitbox.Size.X * 0.5f, Hitbox.Size.Y);
	}

	public void TrackBlockStatus(Creature creature)
	{
		_stateDisplay.TrackBlockStatus(creature);
	}

	public void ShowMultiselectReticle()
	{
		_isInMultiselect = true;
		ShowSingleSelectReticle();
	}

	public void HideMultiselectReticle()
	{
		_isInMultiselect = false;
		HideSingleSelectReticle();
	}

	public void ShowSingleSelectReticle()
	{
		_selectionReticle.OnSelect();
	}

	public void HideSingleSelectReticle()
	{
		if (!_isInMultiselect)
		{
			_selectionReticle.OnDeselect();
		}
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
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_0296: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0325: Unknown result type (might be due to invalid IL or missing references)
		//IL_0348: Unknown result type (might be due to invalid IL or missing references)
		//IL_0353: Unknown result type (might be due to invalid IL or missing references)
		//IL_037e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0389: Expected O, but got Unknown
		//IL_0384: Unknown result type (might be due to invalid IL or missing references)
		//IL_038d: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c3: Expected O, but got Unknown
		//IL_03be: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0410: Unknown result type (might be due to invalid IL or missing references)
		//IL_041b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0441: Unknown result type (might be due to invalid IL or missing references)
		//IL_044a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0470: Unknown result type (might be due to invalid IL or missing references)
		//IL_0479: Unknown result type (might be due to invalid IL or missing references)
		//IL_049f: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0522: Unknown result type (might be due to invalid IL or missing references)
		//IL_0545: Unknown result type (might be due to invalid IL or missing references)
		//IL_0566: Unknown result type (might be due to invalid IL or missing references)
		//IL_0571: Unknown result type (might be due to invalid IL or missing references)
		//IL_0597: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_05db: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_060c: Unknown result type (might be due to invalid IL or missing references)
		//IL_062f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0650: Unknown result type (might be due to invalid IL or missing references)
		//IL_065b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0681: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0725: Unknown result type (might be due to invalid IL or missing references)
		//IL_0748: Unknown result type (might be due to invalid IL or missing references)
		//IL_0753: Unknown result type (might be due to invalid IL or missing references)
		//IL_0779: Unknown result type (might be due to invalid IL or missing references)
		//IL_0782: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_07e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0806: Unknown result type (might be due to invalid IL or missing references)
		//IL_080f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0835: Unknown result type (might be due to invalid IL or missing references)
		//IL_083e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0864: Unknown result type (might be due to invalid IL or missing references)
		//IL_086d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0893: Unknown result type (might be due to invalid IL or missing references)
		//IL_089c: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(35);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._EnterTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._ExitTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ConnectSpineAnimatorSignals, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UpdateBounds, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)4, StringName.op_Implicit("boundsNodeName"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UpdateNavigation, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnFocus, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnUnfocus, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnTargetingStarted, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetRemotePlayerFocused, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)1, StringName.op_Implicit("remotePlayerFocused"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.HideHoverTips, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetAnimationTrigger, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)4, StringName.op_Implicit("trigger"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.GetCurrentAnimationLength, new PropertyInfo((Type)3, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.GetCurrentAnimationTimeRemaining, new PropertyInfo((Type)3, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ToggleIsInteractable, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)1, StringName.op_Implicit("on"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AnimDisableUi, new PropertyInfo((Type)24, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Tween"), false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AnimEnableUi, new PropertyInfo((Type)24, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Tween"), false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.StartDeathAnim, new PropertyInfo((Type)3, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)1, StringName.op_Implicit("shouldRemove"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.StartReviveAnim, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AnimTempRevive, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ImmediatelySetIdle, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AnimHideIntent, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)3, StringName.op_Implicit("delay"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetScaleAndHue, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)3, StringName.op_Implicit("scale"), (PropertyHint)0, "", (PropertyUsageFlags)6, false),
			new PropertyInfo((Type)3, StringName.op_Implicit("hue"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ScaleTo, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)3, StringName.op_Implicit("size"), (PropertyHint)0, "", (PropertyUsageFlags)6, false),
			new PropertyInfo((Type)3, StringName.op_Implicit("duration"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetDefaultScaleTo, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)3, StringName.op_Implicit("size"), (PropertyHint)0, "", (PropertyUsageFlags)6, false),
			new PropertyInfo((Type)3, StringName.op_Implicit("duration"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OstyScaleToSize, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)3, StringName.op_Implicit("ostyHealth"), (PropertyHint)0, "", (PropertyUsageFlags)6, false),
			new PropertyInfo((Type)3, StringName.op_Implicit("duration"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AnimShake, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.DoScaleTween, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)5, StringName.op_Implicit("scale"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetOrbManagerPosition, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.GetTopOfHitbox, new PropertyInfo((Type)5, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.GetBottomOfHitbox, new PropertyInfo((Type)5, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ShowMultiselectReticle, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.HideMultiselectReticle, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ShowSingleSelectReticle, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.HideSingleSelectReticle, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_0269: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		//IL_0296: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0321: Unknown result type (might be due to invalid IL or missing references)
		//IL_0346: Unknown result type (might be due to invalid IL or missing references)
		//IL_036b: Unknown result type (might be due to invalid IL or missing references)
		//IL_039e: Unknown result type (might be due to invalid IL or missing references)
		//IL_03de: Unknown result type (might be due to invalid IL or missing references)
		//IL_041e: Unknown result type (might be due to invalid IL or missing references)
		//IL_045e: Unknown result type (might be due to invalid IL or missing references)
		//IL_049e: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_051b: Unknown result type (might be due to invalid IL or missing references)
		//IL_053a: Unknown result type (might be due to invalid IL or missing references)
		//IL_053f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0544: Unknown result type (might be due to invalid IL or missing references)
		//IL_0549: Unknown result type (might be due to invalid IL or missing references)
		//IL_0567: Unknown result type (might be due to invalid IL or missing references)
		//IL_056c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0571: Unknown result type (might be due to invalid IL or missing references)
		//IL_0576: Unknown result type (might be due to invalid IL or missing references)
		//IL_059a: Unknown result type (might be due to invalid IL or missing references)
		//IL_05bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0613: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0609: Unknown result type (might be due to invalid IL or missing references)
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
		if ((ref method) == MethodName.ConnectSpineAnimatorSignals && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			ConnectSpineAnimatorSignals();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.UpdateBounds && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			UpdateBounds(VariantUtils.ConvertTo<string>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.UpdateNavigation && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			UpdateNavigation();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnFocus && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnFocus();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnUnfocus && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnUnfocus();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnTargetingStarted && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnTargetingStarted();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetRemotePlayerFocused && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			SetRemotePlayerFocused(VariantUtils.ConvertTo<bool>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.HideHoverTips && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			HideHoverTips();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetAnimationTrigger && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			SetAnimationTrigger(VariantUtils.ConvertTo<string>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.GetCurrentAnimationLength && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			float currentAnimationLength = GetCurrentAnimationLength();
			ret = VariantUtils.CreateFrom<float>(ref currentAnimationLength);
			return true;
		}
		if ((ref method) == MethodName.GetCurrentAnimationTimeRemaining && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			float currentAnimationTimeRemaining = GetCurrentAnimationTimeRemaining();
			ret = VariantUtils.CreateFrom<float>(ref currentAnimationTimeRemaining);
			return true;
		}
		if ((ref method) == MethodName.ToggleIsInteractable && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			ToggleIsInteractable(VariantUtils.ConvertTo<bool>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.AnimDisableUi && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			Tween val = AnimDisableUi();
			ret = VariantUtils.CreateFrom<Tween>(ref val);
			return true;
		}
		if ((ref method) == MethodName.AnimEnableUi && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			Tween val2 = AnimEnableUi();
			ret = VariantUtils.CreateFrom<Tween>(ref val2);
			return true;
		}
		if ((ref method) == MethodName.StartDeathAnim && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			float num = StartDeathAnim(VariantUtils.ConvertTo<bool>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<float>(ref num);
			return true;
		}
		if ((ref method) == MethodName.StartReviveAnim && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			StartReviveAnim();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.AnimTempRevive && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			AnimTempRevive();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.ImmediatelySetIdle && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			ImmediatelySetIdle();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.AnimHideIntent && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			AnimHideIntent(VariantUtils.ConvertTo<float>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetScaleAndHue && ((NativeVariantPtrArgs)(ref args)).Count == 2)
		{
			SetScaleAndHue(VariantUtils.ConvertTo<float>(ref ((NativeVariantPtrArgs)(ref args))[0]), VariantUtils.ConvertTo<float>(ref ((NativeVariantPtrArgs)(ref args))[1]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.ScaleTo && ((NativeVariantPtrArgs)(ref args)).Count == 2)
		{
			ScaleTo(VariantUtils.ConvertTo<float>(ref ((NativeVariantPtrArgs)(ref args))[0]), VariantUtils.ConvertTo<float>(ref ((NativeVariantPtrArgs)(ref args))[1]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetDefaultScaleTo && ((NativeVariantPtrArgs)(ref args)).Count == 2)
		{
			SetDefaultScaleTo(VariantUtils.ConvertTo<float>(ref ((NativeVariantPtrArgs)(ref args))[0]), VariantUtils.ConvertTo<float>(ref ((NativeVariantPtrArgs)(ref args))[1]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OstyScaleToSize && ((NativeVariantPtrArgs)(ref args)).Count == 2)
		{
			OstyScaleToSize(VariantUtils.ConvertTo<float>(ref ((NativeVariantPtrArgs)(ref args))[0]), VariantUtils.ConvertTo<float>(ref ((NativeVariantPtrArgs)(ref args))[1]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.AnimShake && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			AnimShake();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.DoScaleTween && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			DoScaleTween(VariantUtils.ConvertTo<Vector2>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetOrbManagerPosition && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			SetOrbManagerPosition();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.GetTopOfHitbox && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			Vector2 topOfHitbox = GetTopOfHitbox();
			ret = VariantUtils.CreateFrom<Vector2>(ref topOfHitbox);
			return true;
		}
		if ((ref method) == MethodName.GetBottomOfHitbox && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			Vector2 bottomOfHitbox = GetBottomOfHitbox();
			ret = VariantUtils.CreateFrom<Vector2>(ref bottomOfHitbox);
			return true;
		}
		if ((ref method) == MethodName.ShowMultiselectReticle && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			ShowMultiselectReticle();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.HideMultiselectReticle && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			HideMultiselectReticle();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.ShowSingleSelectReticle && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			ShowSingleSelectReticle();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.HideSingleSelectReticle && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			HideSingleSelectReticle();
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
		if ((ref method) == MethodName.ConnectSpineAnimatorSignals)
		{
			return true;
		}
		if ((ref method) == MethodName.UpdateBounds)
		{
			return true;
		}
		if ((ref method) == MethodName.UpdateNavigation)
		{
			return true;
		}
		if ((ref method) == MethodName.OnFocus)
		{
			return true;
		}
		if ((ref method) == MethodName.OnUnfocus)
		{
			return true;
		}
		if ((ref method) == MethodName.OnTargetingStarted)
		{
			return true;
		}
		if ((ref method) == MethodName.SetRemotePlayerFocused)
		{
			return true;
		}
		if ((ref method) == MethodName.HideHoverTips)
		{
			return true;
		}
		if ((ref method) == MethodName.SetAnimationTrigger)
		{
			return true;
		}
		if ((ref method) == MethodName.GetCurrentAnimationLength)
		{
			return true;
		}
		if ((ref method) == MethodName.GetCurrentAnimationTimeRemaining)
		{
			return true;
		}
		if ((ref method) == MethodName.ToggleIsInteractable)
		{
			return true;
		}
		if ((ref method) == MethodName.AnimDisableUi)
		{
			return true;
		}
		if ((ref method) == MethodName.AnimEnableUi)
		{
			return true;
		}
		if ((ref method) == MethodName.StartDeathAnim)
		{
			return true;
		}
		if ((ref method) == MethodName.StartReviveAnim)
		{
			return true;
		}
		if ((ref method) == MethodName.AnimTempRevive)
		{
			return true;
		}
		if ((ref method) == MethodName.ImmediatelySetIdle)
		{
			return true;
		}
		if ((ref method) == MethodName.AnimHideIntent)
		{
			return true;
		}
		if ((ref method) == MethodName.SetScaleAndHue)
		{
			return true;
		}
		if ((ref method) == MethodName.ScaleTo)
		{
			return true;
		}
		if ((ref method) == MethodName.SetDefaultScaleTo)
		{
			return true;
		}
		if ((ref method) == MethodName.OstyScaleToSize)
		{
			return true;
		}
		if ((ref method) == MethodName.AnimShake)
		{
			return true;
		}
		if ((ref method) == MethodName.DoScaleTween)
		{
			return true;
		}
		if ((ref method) == MethodName.SetOrbManagerPosition)
		{
			return true;
		}
		if ((ref method) == MethodName.GetTopOfHitbox)
		{
			return true;
		}
		if ((ref method) == MethodName.GetBottomOfHitbox)
		{
			return true;
		}
		if ((ref method) == MethodName.ShowMultiselectReticle)
		{
			return true;
		}
		if ((ref method) == MethodName.HideMultiselectReticle)
		{
			return true;
		}
		if ((ref method) == MethodName.ShowSingleSelectReticle)
		{
			return true;
		}
		if ((ref method) == MethodName.HideSingleSelectReticle)
		{
			return true;
		}
		return ((Control)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName.Hitbox)
		{
			Hitbox = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName.OrbManager)
		{
			OrbManager = VariantUtils.ConvertTo<NOrbManager>(ref value);
			return true;
		}
		if ((ref name) == PropertyName.IsInteractable)
		{
			IsInteractable = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName.Visuals)
		{
			Visuals = VariantUtils.ConvertTo<NCreatureVisuals>(ref value);
			return true;
		}
		if ((ref name) == PropertyName.IntentContainer)
		{
			IntentContainer = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName.IsFocused)
		{
			IsFocused = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName.PlayerIntentHandler)
		{
			PlayerIntentHandler = VariantUtils.ConvertTo<NMultiplayerPlayerIntentHandler>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._stateDisplay)
		{
			_stateDisplay = VariantUtils.ConvertTo<NCreatureStateDisplay>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._intentFadeTween)
		{
			_intentFadeTween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._shakeTween)
		{
			_shakeTween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._isRemotePlayerOrPet)
		{
			_isRemotePlayerOrPet = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._tempScale)
		{
			_tempScale = VariantUtils.ConvertTo<float>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._scaleTween)
		{
			_scaleTween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._isInMultiselect)
		{
			_isInMultiselect = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._selectionReticle)
		{
			_selectionReticle = VariantUtils.ConvertTo<NSelectionReticle>(ref value);
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
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName.Hitbox)
		{
			Control hitbox = Hitbox;
			value = VariantUtils.CreateFrom<Control>(ref hitbox);
			return true;
		}
		if ((ref name) == PropertyName.OrbManager)
		{
			NOrbManager orbManager = OrbManager;
			value = VariantUtils.CreateFrom<NOrbManager>(ref orbManager);
			return true;
		}
		if ((ref name) == PropertyName.IsInteractable)
		{
			bool isInteractable = IsInteractable;
			value = VariantUtils.CreateFrom<bool>(ref isInteractable);
			return true;
		}
		if ((ref name) == PropertyName.VfxSpawnPosition)
		{
			Vector2 vfxSpawnPosition = VfxSpawnPosition;
			value = VariantUtils.CreateFrom<Vector2>(ref vfxSpawnPosition);
			return true;
		}
		if ((ref name) == PropertyName.Visuals)
		{
			NCreatureVisuals visuals = Visuals;
			value = VariantUtils.CreateFrom<NCreatureVisuals>(ref visuals);
			return true;
		}
		if ((ref name) == PropertyName.Body)
		{
			Node2D body = Body;
			value = VariantUtils.CreateFrom<Node2D>(ref body);
			return true;
		}
		if ((ref name) == PropertyName.IntentContainer)
		{
			Control hitbox = IntentContainer;
			value = VariantUtils.CreateFrom<Control>(ref hitbox);
			return true;
		}
		if ((ref name) == PropertyName.IsPlayingDeathAnimation)
		{
			bool isInteractable = IsPlayingDeathAnimation;
			value = VariantUtils.CreateFrom<bool>(ref isInteractable);
			return true;
		}
		if ((ref name) == PropertyName.HasSpineAnimation)
		{
			bool isInteractable = HasSpineAnimation;
			value = VariantUtils.CreateFrom<bool>(ref isInteractable);
			return true;
		}
		if ((ref name) == PropertyName.IsFocused)
		{
			bool isInteractable = IsFocused;
			value = VariantUtils.CreateFrom<bool>(ref isInteractable);
			return true;
		}
		if ((ref name) == PropertyName.PlayerIntentHandler)
		{
			NMultiplayerPlayerIntentHandler playerIntentHandler = PlayerIntentHandler;
			value = VariantUtils.CreateFrom<NMultiplayerPlayerIntentHandler>(ref playerIntentHandler);
			return true;
		}
		if ((ref name) == PropertyName._stateDisplay)
		{
			value = VariantUtils.CreateFrom<NCreatureStateDisplay>(ref _stateDisplay);
			return true;
		}
		if ((ref name) == PropertyName._intentFadeTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _intentFadeTween);
			return true;
		}
		if ((ref name) == PropertyName._shakeTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _shakeTween);
			return true;
		}
		if ((ref name) == PropertyName._isRemotePlayerOrPet)
		{
			value = VariantUtils.CreateFrom<bool>(ref _isRemotePlayerOrPet);
			return true;
		}
		if ((ref name) == PropertyName._tempScale)
		{
			value = VariantUtils.CreateFrom<float>(ref _tempScale);
			return true;
		}
		if ((ref name) == PropertyName._scaleTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _scaleTween);
			return true;
		}
		if ((ref name) == PropertyName._isInMultiselect)
		{
			value = VariantUtils.CreateFrom<bool>(ref _isInMultiselect);
			return true;
		}
		if ((ref name) == PropertyName._selectionReticle)
		{
			value = VariantUtils.CreateFrom<NSelectionReticle>(ref _selectionReticle);
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
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._stateDisplay, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._intentFadeTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._shakeTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._isRemotePlayerOrPet, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)3, PropertyName._tempScale, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._scaleTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.Hitbox, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.OrbManager, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._isInMultiselect, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._selectionReticle, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName.IsInteractable, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)5, PropertyName.VfxSpawnPosition, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.Visuals, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.Body, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.IntentContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName.IsPlayingDeathAnimation, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName.HasSpineAnimation, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName.IsFocused, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.PlayerIntentHandler, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		StringName hitbox = PropertyName.Hitbox;
		Control hitbox2 = Hitbox;
		info.AddProperty(hitbox, Variant.From<Control>(ref hitbox2));
		StringName orbManager = PropertyName.OrbManager;
		NOrbManager orbManager2 = OrbManager;
		info.AddProperty(orbManager, Variant.From<NOrbManager>(ref orbManager2));
		StringName isInteractable = PropertyName.IsInteractable;
		bool isInteractable2 = IsInteractable;
		info.AddProperty(isInteractable, Variant.From<bool>(ref isInteractable2));
		StringName visuals = PropertyName.Visuals;
		NCreatureVisuals visuals2 = Visuals;
		info.AddProperty(visuals, Variant.From<NCreatureVisuals>(ref visuals2));
		StringName intentContainer = PropertyName.IntentContainer;
		hitbox2 = IntentContainer;
		info.AddProperty(intentContainer, Variant.From<Control>(ref hitbox2));
		StringName isFocused = PropertyName.IsFocused;
		isInteractable2 = IsFocused;
		info.AddProperty(isFocused, Variant.From<bool>(ref isInteractable2));
		StringName playerIntentHandler = PropertyName.PlayerIntentHandler;
		NMultiplayerPlayerIntentHandler playerIntentHandler2 = PlayerIntentHandler;
		info.AddProperty(playerIntentHandler, Variant.From<NMultiplayerPlayerIntentHandler>(ref playerIntentHandler2));
		info.AddProperty(PropertyName._stateDisplay, Variant.From<NCreatureStateDisplay>(ref _stateDisplay));
		info.AddProperty(PropertyName._intentFadeTween, Variant.From<Tween>(ref _intentFadeTween));
		info.AddProperty(PropertyName._shakeTween, Variant.From<Tween>(ref _shakeTween));
		info.AddProperty(PropertyName._isRemotePlayerOrPet, Variant.From<bool>(ref _isRemotePlayerOrPet));
		info.AddProperty(PropertyName._tempScale, Variant.From<float>(ref _tempScale));
		info.AddProperty(PropertyName._scaleTween, Variant.From<Tween>(ref _scaleTween));
		info.AddProperty(PropertyName._isInMultiselect, Variant.From<bool>(ref _isInMultiselect));
		info.AddProperty(PropertyName._selectionReticle, Variant.From<NSelectionReticle>(ref _selectionReticle));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName.Hitbox, ref val))
		{
			Hitbox = ((Variant)(ref val)).As<Control>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName.OrbManager, ref val2))
		{
			OrbManager = ((Variant)(ref val2)).As<NOrbManager>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName.IsInteractable, ref val3))
		{
			IsInteractable = ((Variant)(ref val3)).As<bool>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName.Visuals, ref val4))
		{
			Visuals = ((Variant)(ref val4)).As<NCreatureVisuals>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName.IntentContainer, ref val5))
		{
			IntentContainer = ((Variant)(ref val5)).As<Control>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName.IsFocused, ref val6))
		{
			IsFocused = ((Variant)(ref val6)).As<bool>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName.PlayerIntentHandler, ref val7))
		{
			PlayerIntentHandler = ((Variant)(ref val7)).As<NMultiplayerPlayerIntentHandler>();
		}
		Variant val8 = default(Variant);
		if (info.TryGetProperty(PropertyName._stateDisplay, ref val8))
		{
			_stateDisplay = ((Variant)(ref val8)).As<NCreatureStateDisplay>();
		}
		Variant val9 = default(Variant);
		if (info.TryGetProperty(PropertyName._intentFadeTween, ref val9))
		{
			_intentFadeTween = ((Variant)(ref val9)).As<Tween>();
		}
		Variant val10 = default(Variant);
		if (info.TryGetProperty(PropertyName._shakeTween, ref val10))
		{
			_shakeTween = ((Variant)(ref val10)).As<Tween>();
		}
		Variant val11 = default(Variant);
		if (info.TryGetProperty(PropertyName._isRemotePlayerOrPet, ref val11))
		{
			_isRemotePlayerOrPet = ((Variant)(ref val11)).As<bool>();
		}
		Variant val12 = default(Variant);
		if (info.TryGetProperty(PropertyName._tempScale, ref val12))
		{
			_tempScale = ((Variant)(ref val12)).As<float>();
		}
		Variant val13 = default(Variant);
		if (info.TryGetProperty(PropertyName._scaleTween, ref val13))
		{
			_scaleTween = ((Variant)(ref val13)).As<Tween>();
		}
		Variant val14 = default(Variant);
		if (info.TryGetProperty(PropertyName._isInMultiselect, ref val14))
		{
			_isInMultiselect = ((Variant)(ref val14)).As<bool>();
		}
		Variant val15 = default(Variant);
		if (info.TryGetProperty(PropertyName._selectionReticle, ref val15))
		{
			_selectionReticle = ((Variant)(ref val15)).As<NSelectionReticle>();
		}
	}
}
