using System;
using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.HoverTips;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.TestSupport;

namespace MegaCrit.Sts2.Core.Nodes.Vfx;

[ScriptPath("res://src/Core/Nodes/Vfx/NSovereignBladeVfx.cs")]
public class NSovereignBladeVfx : Node2D
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName _ExitTree = StringName.op_Implicit("_ExitTree");

		public static readonly StringName _Process = StringName.op_Implicit("_Process");

		public static readonly StringName Forge = StringName.op_Implicit("Forge");

		public static readonly StringName Attack = StringName.op_Implicit("Attack");

		public static readonly StringName OnTargetingBegan = StringName.op_Implicit("OnTargetingBegan");

		public static readonly StringName OnTargetingEnded = StringName.op_Implicit("OnTargetingEnded");

		public static readonly StringName OnFocused = StringName.op_Implicit("OnFocused");

		public static readonly StringName OnUnfocused = StringName.op_Implicit("OnUnfocused");

		public static readonly StringName UpdateHoverTip = StringName.op_Implicit("UpdateHoverTip");

		public static readonly StringName FireSparks = StringName.op_Implicit("FireSparks");

		public static readonly StringName FireFlames = StringName.op_Implicit("FireFlames");

		public static readonly StringName EndSlash = StringName.op_Implicit("EndSlash");

		public static readonly StringName CleanupForge = StringName.op_Implicit("CleanupForge");

		public static readonly StringName CleanupAttack = StringName.op_Implicit("CleanupAttack");

		public static readonly StringName RemoveSovereignBlade = StringName.op_Implicit("RemoveSovereignBlade");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName OrbitProgress = StringName.op_Implicit("OrbitProgress");

		public static readonly StringName _spineNode = StringName.op_Implicit("_spineNode");

		public static readonly StringName _bladeGlow = StringName.op_Implicit("_bladeGlow");

		public static readonly StringName _forgeSparks = StringName.op_Implicit("_forgeSparks");

		public static readonly StringName _spawnFlames = StringName.op_Implicit("_spawnFlames");

		public static readonly StringName _spawnFlamesBack = StringName.op_Implicit("_spawnFlamesBack");

		public static readonly StringName _slashParticles = StringName.op_Implicit("_slashParticles");

		public static readonly StringName _chargeParticles = StringName.op_Implicit("_chargeParticles");

		public static readonly StringName _spikeParticles = StringName.op_Implicit("_spikeParticles");

		public static readonly StringName _spikeParticles2 = StringName.op_Implicit("_spikeParticles2");

		public static readonly StringName _spikeCircle = StringName.op_Implicit("_spikeCircle");

		public static readonly StringName _spikeCircle2 = StringName.op_Implicit("_spikeCircle2");

		public static readonly StringName _hilt = StringName.op_Implicit("_hilt");

		public static readonly StringName _hilt2 = StringName.op_Implicit("_hilt2");

		public static readonly StringName _detail = StringName.op_Implicit("_detail");

		public static readonly StringName _trail = StringName.op_Implicit("_trail");

		public static readonly StringName _orbitPath = StringName.op_Implicit("_orbitPath");

		public static readonly StringName _hitbox = StringName.op_Implicit("_hitbox");

		public static readonly StringName _selectionReticle = StringName.op_Implicit("_selectionReticle");

		public static readonly StringName _attackTween = StringName.op_Implicit("_attackTween");

		public static readonly StringName _scaleTween = StringName.op_Implicit("_scaleTween");

		public static readonly StringName _sparkDelay = StringName.op_Implicit("_sparkDelay");

		public static readonly StringName _glowTween = StringName.op_Implicit("_glowTween");

		public static readonly StringName _trailStart = StringName.op_Implicit("_trailStart");

		public static readonly StringName _bladeSize = StringName.op_Implicit("_bladeSize");

		public static readonly StringName _targetOrbitPosition = StringName.op_Implicit("_targetOrbitPosition");

		public static readonly StringName _isBehindCharacter = StringName.op_Implicit("_isBehindCharacter");

		public static readonly StringName _isFocused = StringName.op_Implicit("_isFocused");

		public static readonly StringName _hoverTip = StringName.op_Implicit("_hoverTip");

		public static readonly StringName _isForging = StringName.op_Implicit("_isForging");

		public static readonly StringName _isAttacking = StringName.op_Implicit("_isAttacking");

		public static readonly StringName _isKeyPressed = StringName.op_Implicit("_isKeyPressed");

		public static readonly StringName _testCharge = StringName.op_Implicit("_testCharge");
	}

	public class SignalName : SignalName
	{
	}

	private static readonly string _scenePath = SceneHelper.GetScenePath("vfx/sovereign_blade");

	private Player _owner;

	private Node2D _spineNode;

	private MegaSprite _animController;

	private Node2D _bladeGlow;

	private GpuParticles2D _forgeSparks;

	private GpuParticles2D _spawnFlames;

	private GpuParticles2D _spawnFlamesBack;

	private GpuParticles2D _slashParticles;

	private GpuParticles2D _chargeParticles;

	private GpuParticles2D _spikeParticles;

	private GpuParticles2D _spikeParticles2;

	private GpuParticles2D _spikeCircle;

	private GpuParticles2D _spikeCircle2;

	private TextureRect _hilt;

	private TextureRect _hilt2;

	private TextureRect _detail;

	private Line2D _trail;

	private Path2D _orbitPath;

	private Control _hitbox;

	private NSelectionReticle _selectionReticle;

	private Tween? _attackTween;

	private Tween? _scaleTween;

	private Tween? _sparkDelay;

	private Tween? _glowTween;

	private Vector2 _trailStart;

	private float _bladeSize;

	private const float _orbitSpeed = 60f;

	private Vector2 _targetOrbitPosition;

	private bool _isBehindCharacter;

	private const float _hiltThreshold = 0.3f;

	private const float _detailThreshold = 0.66f;

	private bool _isFocused;

	private NHoverTipSet? _hoverTip;

	private bool _isForging;

	private bool _isAttacking;

	private bool _isKeyPressed;

	private float _testCharge;

	public static IEnumerable<string> AssetPaths => new _003C_003Ez__ReadOnlySingleElementList<string>(_scenePath);

	public CardModel Card { get; private set; }

	public double OrbitProgress { get; set; }

	public override void _Ready()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0307: Unknown result type (might be due to invalid IL or missing references)
		//IL_0333: Unknown result type (might be due to invalid IL or missing references)
		//IL_0339: Unknown result type (might be due to invalid IL or missing references)
		//IL_0355: Unknown result type (might be due to invalid IL or missing references)
		//IL_035b: Unknown result type (might be due to invalid IL or missing references)
		_spineNode = ((Node)this).GetNode<Node2D>(NodePath.op_Implicit("SpineSword"));
		_animController = new MegaSprite(Variant.op_Implicit((GodotObject)(object)_spineNode));
		_bladeGlow = ((Node)this).GetNode<Node2D>(NodePath.op_Implicit("SpineSword/SwordBone/ScaleContainer/BladeGlow"));
		_forgeSparks = ((Node)this).GetNode<GpuParticles2D>(NodePath.op_Implicit("SpineSword/SwordBone/ScaleContainer/ForgeSparks"));
		_spawnFlames = ((Node)this).GetNode<GpuParticles2D>(NodePath.op_Implicit("SpineSword/SwordBone/ScaleContainer/SpawnFlames"));
		_spawnFlamesBack = ((Node)this).GetNode<GpuParticles2D>(NodePath.op_Implicit("SpineSword/SwordBone/ScaleContainer/SpawnFlamesBack"));
		_slashParticles = ((Node)this).GetNode<GpuParticles2D>(NodePath.op_Implicit("SpineSword/SlashParticles"));
		_chargeParticles = ((Node)this).GetNode<GpuParticles2D>(NodePath.op_Implicit("SpineSword/SwordBone/ScaleContainer/ChargeParticles"));
		_spikeParticles = ((Node)this).GetNode<GpuParticles2D>(NodePath.op_Implicit("SpineSword/SwordBone/ScaleContainer/Spikes"));
		_spikeParticles2 = ((Node)this).GetNode<GpuParticles2D>(NodePath.op_Implicit("SpineSword/SwordBone/ScaleContainer/Spikes2"));
		_spikeCircle = ((Node)this).GetNode<GpuParticles2D>(NodePath.op_Implicit("SpineSword/SwordBone/ScaleContainer/SpikeCircle"));
		_spikeCircle2 = ((Node)this).GetNode<GpuParticles2D>(NodePath.op_Implicit("SpineSword/SwordBone/ScaleContainer/SpikeCircle2"));
		_hilt = ((Node)this).GetNode<TextureRect>(NodePath.op_Implicit("SpineSword/SwordBone/ScaleContainer/Hilt"));
		_hilt2 = ((Node)this).GetNode<TextureRect>(NodePath.op_Implicit("SpineSword/SwordBone/ScaleContainer/Hilt2"));
		_detail = ((Node)this).GetNode<TextureRect>(NodePath.op_Implicit("SpineSword/SwordBone/ScaleContainer/Detail"));
		_trail = ((Node)this).GetNode<Line2D>(NodePath.op_Implicit("Trail"));
		_orbitPath = ((Node)this).GetNode<Path2D>(NodePath.op_Implicit("%Path"));
		_hitbox = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%Hitbox"));
		_selectionReticle = ((Node)this).GetNode<NSelectionReticle>(NodePath.op_Implicit("%SelectionReticle"));
		((GodotObject)_hitbox).Connect(SignalName.MouseEntered, Callable.From((Action)OnFocused), 0u);
		((GodotObject)_hitbox).Connect(SignalName.MouseExited, Callable.From((Action)OnUnfocused), 0u);
		((GodotObject)_hitbox).Connect(SignalName.FocusEntered, Callable.From((Action)OnFocused), 0u);
		((GodotObject)_hitbox).Connect(SignalName.FocusExited, Callable.From((Action)OnUnfocused), 0u);
		_forgeSparks.Emitting = false;
		_forgeSparks.OneShot = true;
		_spawnFlames.Emitting = false;
		_spawnFlames.OneShot = true;
		_spawnFlamesBack.Emitting = false;
		_spawnFlamesBack.OneShot = true;
		_slashParticles.Emitting = false;
		_slashParticles.OneShot = true;
		_chargeParticles.Emitting = false;
		_spikeParticles2.Emitting = false;
		_spikeCircle2.Emitting = false;
		((CanvasItem)_bladeGlow).Modulate = Colors.Transparent;
		((CanvasItem)_bladeGlow).Visible = false;
		((Node2D)_trail).GlobalPosition = Vector2.Zero;
		_trail.ClearPoints();
		_animController.GetAnimationState().SetAnimation("idle_loop");
		_spineNode.Scale = Vector2.Zero;
		((CanvasItem)_spineNode).Visible = true;
		((GodotObject)NTargetManager.Instance).Connect(NTargetManager.SignalName.TargetingBegan, Callable.From((Action)OnTargetingBegan), 0u);
		((GodotObject)NTargetManager.Instance).Connect(NTargetManager.SignalName.TargetingEnded, Callable.From((Action)OnTargetingEnded), 0u);
		_owner = Card.Owner;
		_owner.Creature.Died += OnOwnerDied;
	}

	public override void _ExitTree()
	{
		Tween? attackTween = _attackTween;
		if (attackTween != null)
		{
			attackTween.Kill();
		}
		Tween? scaleTween = _scaleTween;
		if (scaleTween != null)
		{
			scaleTween.Kill();
		}
		Tween? sparkDelay = _sparkDelay;
		if (sparkDelay != null)
		{
			sparkDelay.Kill();
		}
		Tween? glowTween = _glowTween;
		if (glowTween != null)
		{
			glowTween.Kill();
		}
		_owner.Creature.Died -= OnOwnerDied;
	}

	public static NSovereignBladeVfx? Create(CardModel card)
	{
		if (TestMode.IsOn)
		{
			return null;
		}
		NSovereignBladeVfx nSovereignBladeVfx = PreloadManager.Cache.GetScene(_scenePath).Instantiate<NSovereignBladeVfx>((GenEditState)0);
		nSovereignBladeVfx.Card = card;
		return nSovereignBladeVfx;
	}

	public override void _Process(double delta)
	{
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		float bakedLength = _orbitPath.Curve.GetBakedLength();
		if (_hoverTip == null)
		{
			OrbitProgress += 60.0 * delta / (double)bakedLength;
		}
		double num = OrbitProgress % 1.0;
		bool flag = num > 0.25 && num < 0.7799999713897705;
		if (flag != _isBehindCharacter && _bladeSize < 0.6f)
		{
			_isBehindCharacter = !_isBehindCharacter;
			((Node)this).GetParent().MoveChild((Node)(object)this, (!flag) ? (((Node)this).GetParent().GetChildCount(false) - 1) : 0);
		}
		Transform2D val = _orbitPath.Curve.SampleBakedWithRotation((float)(OrbitProgress % 1.0) * bakedLength, false);
		Vector2 val2 = ((Node2D)_orbitPath).GlobalTransform * val.Origin;
		val2.X = Mathf.Lerp(val2.X, ((Node2D)this).GlobalPosition.X + 200f, Mathf.Clamp(_bladeSize / 1.25f, 0f, 1f));
		_targetOrbitPosition = val2 + Vector2.Up * (_spineNode.Scale.Y - 1f) * 100f;
		if (!_isAttacking)
		{
			Node2D spineNode = _spineNode;
			Vector2 globalPosition = _spineNode.GlobalPosition;
			spineNode.GlobalPosition = ((Vector2)(ref globalPosition)).Lerp(_targetOrbitPosition, (float)delta * 7f);
		}
	}

	public void Forge(float bladeDamage = 0f, bool showFlames = false)
	{
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0270: Unknown result type (might be due to invalid IL or missing references)
		//IL_0276: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02da: Unknown result type (might be due to invalid IL or missing references)
		//IL_02db: Unknown result type (might be due to invalid IL or missing references)
		if (_isForging)
		{
			CleanupForge();
		}
		_bladeSize = Mathf.Clamp(Mathf.Lerp(0f, 1f, bladeDamage / 200f), 0f, 1f);
		_isForging = true;
		int num = (int)(_bladeSize * 30f);
		if (num > 0)
		{
			_chargeParticles.Amount = num;
			_chargeParticles.Emitting = true;
		}
		else
		{
			_chargeParticles.Emitting = false;
		}
		((CanvasItem)_hilt).Visible = _bladeSize < 0.3f;
		((CanvasItem)_hilt2).Visible = !((CanvasItem)_hilt).Visible;
		GpuParticles2D spikeParticles = _spikeParticles;
		bool emitting = (((CanvasItem)_spikeParticles).Visible = ((CanvasItem)_hilt).Visible);
		spikeParticles.Emitting = emitting;
		GpuParticles2D spikeParticles2 = _spikeParticles2;
		emitting = (((CanvasItem)_spikeParticles2).Visible = !((CanvasItem)_hilt).Visible);
		spikeParticles2.Emitting = emitting;
		GpuParticles2D spikeCircle = _spikeCircle;
		emitting = (((CanvasItem)_spikeCircle).Visible = ((CanvasItem)_hilt).Visible);
		spikeCircle.Emitting = emitting;
		GpuParticles2D spikeCircle2 = _spikeCircle2;
		emitting = (((CanvasItem)_spikeCircle2).Visible = !((CanvasItem)_hilt).Visible);
		spikeCircle2.Emitting = emitting;
		((CanvasItem)_detail).Visible = bladeDamage >= 0.66f;
		((CanvasItem)_bladeGlow).Visible = true;
		Color val = Color.FromHtml((ReadOnlySpan<char>)"#ff7300");
		Color val2 = val;
		val2.A = 0f;
		_glowTween = ((Node)this).CreateTween();
		if (showFlames)
		{
			FireFlames();
		}
		_glowTween.TweenProperty((GodotObject)(object)_bladeGlow, NodePath.op_Implicit("modulate"), Variant.op_Implicit(val), 0.05).SetEase((EaseType)1);
		_glowTween.Chain().TweenProperty((GodotObject)(object)_bladeGlow, NodePath.op_Implicit("modulate"), Variant.op_Implicit(val2), 0.5).SetEase((EaseType)0)
			.SetTrans((TransitionType)7);
		_glowTween.Chain().TweenCallback(Callable.From((Action)CleanupForge));
		Vector2 val3 = Vector2.One * Mathf.Lerp(0.9f, 2f, _bladeSize);
		_scaleTween = ((Node)this).CreateTween();
		_scaleTween.TweenProperty((GodotObject)(object)_spineNode, NodePath.op_Implicit("scale"), Variant.op_Implicit(val3 * 1.2f), 0.05000000074505806).SetEase((EaseType)1).SetTrans((TransitionType)7);
		_scaleTween.Chain().TweenCallback(Callable.From((Action)FireSparks));
		_scaleTween.Chain().TweenProperty((GodotObject)(object)_spineNode, NodePath.op_Implicit("scale"), Variant.op_Implicit(val3), 0.30000001192092896).SetEase((EaseType)2)
			.SetTrans((TransitionType)7);
	}

	public void Attack(Vector2 targetPos)
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		if (_isAttacking)
		{
			CleanupAttack();
		}
		_isAttacking = true;
		_animController.GetAnimationState().SetAnimation("attack", loop: false);
		_attackTween = ((Node)this).CreateTween();
		Vector2 val = default(Vector2);
		((Vector2)(ref val))._002Ector(_spineNode.GlobalPosition.X - 50f, _spineNode.GlobalPosition.Y);
		((CanvasItem)_trail).Visible = true;
		_trailStart = val;
		_attackTween.TweenProperty((GodotObject)(object)_spineNode, NodePath.op_Implicit("rotation"), Variant.op_Implicit(_spineNode.GetAngleTo(targetPos)), 0.05000000074505806);
		_attackTween.Parallel().TweenProperty((GodotObject)(object)_spineNode, NodePath.op_Implicit("global_position"), Variant.op_Implicit(val), 0.07999999821186066).SetEase((EaseType)1)
			.SetTrans((TransitionType)5);
		_attackTween.Chain().TweenProperty((GodotObject)(object)_spineNode, NodePath.op_Implicit("rotation"), Variant.op_Implicit(_spineNode.GetAngleTo(targetPos)), 0.0);
		_attackTween.Parallel().TweenProperty((GodotObject)(object)_spineNode, NodePath.op_Implicit("global_position"), Variant.op_Implicit(targetPos), 0.05000000074505806).SetEase((EaseType)0)
			.SetTrans((TransitionType)5);
		_attackTween.Chain().TweenCallback(Callable.From((Action)EndSlash));
		_attackTween.TweenInterval(0.25);
		_attackTween.Chain().TweenCallback(Callable.From((Action)FireSparks)).SetDelay(0.30000001192092896);
		_attackTween.Chain().TweenCallback(Callable.From((Action)CleanupAttack));
		UpdateHoverTip();
	}

	private void OnTargetingBegan()
	{
		_hitbox.MouseFilter = (MouseFilterEnum)2;
		UpdateHoverTip();
	}

	private void OnTargetingEnded()
	{
		_hitbox.MouseFilter = (MouseFilterEnum)0;
		UpdateHoverTip();
	}

	private void OnFocused()
	{
		_isFocused = true;
		if (!NCombatRoom.Instance.Ui.Hand.InCardPlay)
		{
			UpdateHoverTip();
		}
	}

	private void OnUnfocused()
	{
		_isFocused = false;
		UpdateHoverTip();
	}

	private void UpdateHoverTip()
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Invalid comparison between Unknown and I8
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		bool flag = _isFocused && !_isAttacking && !NTargetManager.Instance.IsInSelection && (long)_hitbox.MouseFilter != 2;
		if (flag && _hoverTip == null)
		{
			_hoverTip = NHoverTipSet.CreateAndShow(_hitbox, HoverTipFactory.FromCard(Card));
			((Control)_hoverTip).GlobalPosition = _hitbox.GlobalPosition + Vector2.Right * _hitbox.Size.X;
			_selectionReticle.OnSelect();
		}
		else if (!flag && _hoverTip != null)
		{
			NHoverTipSet.Remove(_hitbox);
			_selectionReticle.OnDeselect();
			_hoverTip = null;
		}
	}

	private void FireSparks()
	{
		_forgeSparks.Restart();
	}

	private void FireFlames()
	{
		_spawnFlames.Restart();
		_spawnFlamesBack.Restart();
	}

	private void EndSlash()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		_chargeParticles.Emitting = false;
		_chargeParticles.Restart();
		((Node2D)_slashParticles).Rotation = _spineNode.GetAngleTo(_trailStart) - 1.5708f;
		_slashParticles.Restart();
		_trail.AddPoint(_trailStart, -1);
		_trail.AddPoint(((Node)this).GetNode<Node2D>(NodePath.op_Implicit("SpineSword/SwordBone/ScaleContainer/SpikeCircle")).GlobalPosition, -1);
		((CanvasItem)_trail).Modulate = Colors.White;
		((Node)this).CreateTween().TweenProperty((GodotObject)(object)_trail, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0f), 0.20000000298023224);
	}

	private void CleanupForge()
	{
		_isForging = false;
		Tween? scaleTween = _scaleTween;
		if (scaleTween != null)
		{
			scaleTween.Kill();
		}
		Tween? glowTween = _glowTween;
		if (glowTween != null)
		{
			glowTween.Kill();
		}
	}

	private void CleanupAttack()
	{
		_isAttacking = false;
		Tween? attackTween = _attackTween;
		if (attackTween != null)
		{
			attackTween.Kill();
		}
		_animController.GetAnimationState().SetAnimation("idle_loop");
		_spineNode.Rotation = 0f;
		_trail.ClearPoints();
	}

	public void RemoveSovereignBlade()
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		Tween? scaleTween = _scaleTween;
		if (scaleTween != null)
		{
			scaleTween.Kill();
		}
		_scaleTween = ((Node)this).CreateTween();
		_scaleTween.TweenProperty((GodotObject)(object)_spineNode, NodePath.op_Implicit("scale"), Variant.op_Implicit(Vector2.Zero), 0.20000000298023224).SetEase((EaseType)1).SetTrans((TransitionType)7);
		_scaleTween.Chain().TweenCallback(Callable.From((Action)this.QueueFreeSafely));
	}

	private void OnOwnerDied(Creature creature)
	{
		_hitbox.MouseFilter = (MouseFilterEnum)2;
		UpdateHoverTip();
		RemoveSovereignBlade();
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_028a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0293: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0317: Unknown result type (might be due to invalid IL or missing references)
		//IL_0320: Unknown result type (might be due to invalid IL or missing references)
		//IL_0346: Unknown result type (might be due to invalid IL or missing references)
		//IL_034f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0375: Unknown result type (might be due to invalid IL or missing references)
		//IL_037e: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(16);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._ExitTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Process, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)3, StringName.op_Implicit("delta"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.Forge, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)3, StringName.op_Implicit("bladeDamage"), (PropertyHint)0, "", (PropertyUsageFlags)6, false),
			new PropertyInfo((Type)1, StringName.op_Implicit("showFlames"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.Attack, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)5, StringName.op_Implicit("targetPos"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnTargetingBegan, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnTargetingEnded, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnFocused, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnUnfocused, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UpdateHoverTip, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.FireSparks, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.FireFlames, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.EndSlash, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.CleanupForge, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.CleanupAttack, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.RemoveSovereignBlade, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_0289: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
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
		if ((ref method) == MethodName._Process && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			((Node)this)._Process(VariantUtils.ConvertTo<double>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.Forge && ((NativeVariantPtrArgs)(ref args)).Count == 2)
		{
			Forge(VariantUtils.ConvertTo<float>(ref ((NativeVariantPtrArgs)(ref args))[0]), VariantUtils.ConvertTo<bool>(ref ((NativeVariantPtrArgs)(ref args))[1]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.Attack && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			Attack(VariantUtils.ConvertTo<Vector2>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnTargetingBegan && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnTargetingBegan();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnTargetingEnded && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnTargetingEnded();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnFocused && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnFocused();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnUnfocused && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnUnfocused();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.UpdateHoverTip && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			UpdateHoverTip();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.FireSparks && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			FireSparks();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.FireFlames && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			FireFlames();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.EndSlash && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			EndSlash();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.CleanupForge && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			CleanupForge();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.CleanupAttack && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			CleanupAttack();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.RemoveSovereignBlade && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			RemoveSovereignBlade();
			ret = default(godot_variant);
			return true;
		}
		return ((Node2D)this).InvokeGodotClassMethod(ref method, args, ref ret);
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
		if ((ref method) == MethodName._Process)
		{
			return true;
		}
		if ((ref method) == MethodName.Forge)
		{
			return true;
		}
		if ((ref method) == MethodName.Attack)
		{
			return true;
		}
		if ((ref method) == MethodName.OnTargetingBegan)
		{
			return true;
		}
		if ((ref method) == MethodName.OnTargetingEnded)
		{
			return true;
		}
		if ((ref method) == MethodName.OnFocused)
		{
			return true;
		}
		if ((ref method) == MethodName.OnUnfocused)
		{
			return true;
		}
		if ((ref method) == MethodName.UpdateHoverTip)
		{
			return true;
		}
		if ((ref method) == MethodName.FireSparks)
		{
			return true;
		}
		if ((ref method) == MethodName.FireFlames)
		{
			return true;
		}
		if ((ref method) == MethodName.EndSlash)
		{
			return true;
		}
		if ((ref method) == MethodName.CleanupForge)
		{
			return true;
		}
		if ((ref method) == MethodName.CleanupAttack)
		{
			return true;
		}
		if ((ref method) == MethodName.RemoveSovereignBlade)
		{
			return true;
		}
		return ((Node2D)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0281: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName.OrbitProgress)
		{
			OrbitProgress = VariantUtils.ConvertTo<double>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._spineNode)
		{
			_spineNode = VariantUtils.ConvertTo<Node2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._bladeGlow)
		{
			_bladeGlow = VariantUtils.ConvertTo<Node2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._forgeSparks)
		{
			_forgeSparks = VariantUtils.ConvertTo<GpuParticles2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._spawnFlames)
		{
			_spawnFlames = VariantUtils.ConvertTo<GpuParticles2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._spawnFlamesBack)
		{
			_spawnFlamesBack = VariantUtils.ConvertTo<GpuParticles2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._slashParticles)
		{
			_slashParticles = VariantUtils.ConvertTo<GpuParticles2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._chargeParticles)
		{
			_chargeParticles = VariantUtils.ConvertTo<GpuParticles2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._spikeParticles)
		{
			_spikeParticles = VariantUtils.ConvertTo<GpuParticles2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._spikeParticles2)
		{
			_spikeParticles2 = VariantUtils.ConvertTo<GpuParticles2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._spikeCircle)
		{
			_spikeCircle = VariantUtils.ConvertTo<GpuParticles2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._spikeCircle2)
		{
			_spikeCircle2 = VariantUtils.ConvertTo<GpuParticles2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._hilt)
		{
			_hilt = VariantUtils.ConvertTo<TextureRect>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._hilt2)
		{
			_hilt2 = VariantUtils.ConvertTo<TextureRect>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._detail)
		{
			_detail = VariantUtils.ConvertTo<TextureRect>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._trail)
		{
			_trail = VariantUtils.ConvertTo<Line2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._orbitPath)
		{
			_orbitPath = VariantUtils.ConvertTo<Path2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._hitbox)
		{
			_hitbox = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._selectionReticle)
		{
			_selectionReticle = VariantUtils.ConvertTo<NSelectionReticle>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._attackTween)
		{
			_attackTween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._scaleTween)
		{
			_scaleTween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._sparkDelay)
		{
			_sparkDelay = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._glowTween)
		{
			_glowTween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._trailStart)
		{
			_trailStart = VariantUtils.ConvertTo<Vector2>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._bladeSize)
		{
			_bladeSize = VariantUtils.ConvertTo<float>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._targetOrbitPosition)
		{
			_targetOrbitPosition = VariantUtils.ConvertTo<Vector2>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._isBehindCharacter)
		{
			_isBehindCharacter = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._isFocused)
		{
			_isFocused = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._hoverTip)
		{
			_hoverTip = VariantUtils.ConvertTo<NHoverTipSet>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._isForging)
		{
			_isForging = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._isAttacking)
		{
			_isAttacking = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._isKeyPressed)
		{
			_isKeyPressed = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._testCharge)
		{
			_testCharge = VariantUtils.ConvertTo<float>(ref value);
			return true;
		}
		return ((GodotObject)this).SetGodotClassPropertyValue(ref name, ref value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0277: Unknown result type (might be due to invalid IL or missing references)
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0297: Unknown result type (might be due to invalid IL or missing references)
		//IL_029c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0317: Unknown result type (might be due to invalid IL or missing references)
		//IL_031c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0337: Unknown result type (might be due to invalid IL or missing references)
		//IL_033c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0357: Unknown result type (might be due to invalid IL or missing references)
		//IL_035c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0377: Unknown result type (might be due to invalid IL or missing references)
		//IL_037c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0397: Unknown result type (might be due to invalid IL or missing references)
		//IL_039c: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0417: Unknown result type (might be due to invalid IL or missing references)
		//IL_041c: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName.OrbitProgress)
		{
			double orbitProgress = OrbitProgress;
			value = VariantUtils.CreateFrom<double>(ref orbitProgress);
			return true;
		}
		if ((ref name) == PropertyName._spineNode)
		{
			value = VariantUtils.CreateFrom<Node2D>(ref _spineNode);
			return true;
		}
		if ((ref name) == PropertyName._bladeGlow)
		{
			value = VariantUtils.CreateFrom<Node2D>(ref _bladeGlow);
			return true;
		}
		if ((ref name) == PropertyName._forgeSparks)
		{
			value = VariantUtils.CreateFrom<GpuParticles2D>(ref _forgeSparks);
			return true;
		}
		if ((ref name) == PropertyName._spawnFlames)
		{
			value = VariantUtils.CreateFrom<GpuParticles2D>(ref _spawnFlames);
			return true;
		}
		if ((ref name) == PropertyName._spawnFlamesBack)
		{
			value = VariantUtils.CreateFrom<GpuParticles2D>(ref _spawnFlamesBack);
			return true;
		}
		if ((ref name) == PropertyName._slashParticles)
		{
			value = VariantUtils.CreateFrom<GpuParticles2D>(ref _slashParticles);
			return true;
		}
		if ((ref name) == PropertyName._chargeParticles)
		{
			value = VariantUtils.CreateFrom<GpuParticles2D>(ref _chargeParticles);
			return true;
		}
		if ((ref name) == PropertyName._spikeParticles)
		{
			value = VariantUtils.CreateFrom<GpuParticles2D>(ref _spikeParticles);
			return true;
		}
		if ((ref name) == PropertyName._spikeParticles2)
		{
			value = VariantUtils.CreateFrom<GpuParticles2D>(ref _spikeParticles2);
			return true;
		}
		if ((ref name) == PropertyName._spikeCircle)
		{
			value = VariantUtils.CreateFrom<GpuParticles2D>(ref _spikeCircle);
			return true;
		}
		if ((ref name) == PropertyName._spikeCircle2)
		{
			value = VariantUtils.CreateFrom<GpuParticles2D>(ref _spikeCircle2);
			return true;
		}
		if ((ref name) == PropertyName._hilt)
		{
			value = VariantUtils.CreateFrom<TextureRect>(ref _hilt);
			return true;
		}
		if ((ref name) == PropertyName._hilt2)
		{
			value = VariantUtils.CreateFrom<TextureRect>(ref _hilt2);
			return true;
		}
		if ((ref name) == PropertyName._detail)
		{
			value = VariantUtils.CreateFrom<TextureRect>(ref _detail);
			return true;
		}
		if ((ref name) == PropertyName._trail)
		{
			value = VariantUtils.CreateFrom<Line2D>(ref _trail);
			return true;
		}
		if ((ref name) == PropertyName._orbitPath)
		{
			value = VariantUtils.CreateFrom<Path2D>(ref _orbitPath);
			return true;
		}
		if ((ref name) == PropertyName._hitbox)
		{
			value = VariantUtils.CreateFrom<Control>(ref _hitbox);
			return true;
		}
		if ((ref name) == PropertyName._selectionReticle)
		{
			value = VariantUtils.CreateFrom<NSelectionReticle>(ref _selectionReticle);
			return true;
		}
		if ((ref name) == PropertyName._attackTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _attackTween);
			return true;
		}
		if ((ref name) == PropertyName._scaleTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _scaleTween);
			return true;
		}
		if ((ref name) == PropertyName._sparkDelay)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _sparkDelay);
			return true;
		}
		if ((ref name) == PropertyName._glowTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _glowTween);
			return true;
		}
		if ((ref name) == PropertyName._trailStart)
		{
			value = VariantUtils.CreateFrom<Vector2>(ref _trailStart);
			return true;
		}
		if ((ref name) == PropertyName._bladeSize)
		{
			value = VariantUtils.CreateFrom<float>(ref _bladeSize);
			return true;
		}
		if ((ref name) == PropertyName._targetOrbitPosition)
		{
			value = VariantUtils.CreateFrom<Vector2>(ref _targetOrbitPosition);
			return true;
		}
		if ((ref name) == PropertyName._isBehindCharacter)
		{
			value = VariantUtils.CreateFrom<bool>(ref _isBehindCharacter);
			return true;
		}
		if ((ref name) == PropertyName._isFocused)
		{
			value = VariantUtils.CreateFrom<bool>(ref _isFocused);
			return true;
		}
		if ((ref name) == PropertyName._hoverTip)
		{
			value = VariantUtils.CreateFrom<NHoverTipSet>(ref _hoverTip);
			return true;
		}
		if ((ref name) == PropertyName._isForging)
		{
			value = VariantUtils.CreateFrom<bool>(ref _isForging);
			return true;
		}
		if ((ref name) == PropertyName._isAttacking)
		{
			value = VariantUtils.CreateFrom<bool>(ref _isAttacking);
			return true;
		}
		if ((ref name) == PropertyName._isKeyPressed)
		{
			value = VariantUtils.CreateFrom<bool>(ref _isKeyPressed);
			return true;
		}
		if ((ref name) == PropertyName._testCharge)
		{
			value = VariantUtils.CreateFrom<float>(ref _testCharge);
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
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0290: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0312: Unknown result type (might be due to invalid IL or missing references)
		//IL_0332: Unknown result type (might be due to invalid IL or missing references)
		//IL_0352: Unknown result type (might be due to invalid IL or missing references)
		//IL_0372: Unknown result type (might be due to invalid IL or missing references)
		//IL_0392: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0413: Unknown result type (might be due to invalid IL or missing references)
		//IL_0433: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._spineNode, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._bladeGlow, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._forgeSparks, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._spawnFlames, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._spawnFlamesBack, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._slashParticles, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._chargeParticles, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._spikeParticles, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._spikeParticles2, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._spikeCircle, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._spikeCircle2, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._hilt, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._hilt2, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._detail, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._trail, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._orbitPath, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._hitbox, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._selectionReticle, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._attackTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._scaleTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._sparkDelay, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._glowTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)5, PropertyName._trailStart, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)3, PropertyName._bladeSize, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)3, PropertyName.OrbitProgress, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)5, PropertyName._targetOrbitPosition, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._isBehindCharacter, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._isFocused, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._hoverTip, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._isForging, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._isAttacking, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._isKeyPressed, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)3, PropertyName._testCharge, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0294: Unknown result type (might be due to invalid IL or missing references)
		//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d6: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		StringName orbitProgress = PropertyName.OrbitProgress;
		double orbitProgress2 = OrbitProgress;
		info.AddProperty(orbitProgress, Variant.From<double>(ref orbitProgress2));
		info.AddProperty(PropertyName._spineNode, Variant.From<Node2D>(ref _spineNode));
		info.AddProperty(PropertyName._bladeGlow, Variant.From<Node2D>(ref _bladeGlow));
		info.AddProperty(PropertyName._forgeSparks, Variant.From<GpuParticles2D>(ref _forgeSparks));
		info.AddProperty(PropertyName._spawnFlames, Variant.From<GpuParticles2D>(ref _spawnFlames));
		info.AddProperty(PropertyName._spawnFlamesBack, Variant.From<GpuParticles2D>(ref _spawnFlamesBack));
		info.AddProperty(PropertyName._slashParticles, Variant.From<GpuParticles2D>(ref _slashParticles));
		info.AddProperty(PropertyName._chargeParticles, Variant.From<GpuParticles2D>(ref _chargeParticles));
		info.AddProperty(PropertyName._spikeParticles, Variant.From<GpuParticles2D>(ref _spikeParticles));
		info.AddProperty(PropertyName._spikeParticles2, Variant.From<GpuParticles2D>(ref _spikeParticles2));
		info.AddProperty(PropertyName._spikeCircle, Variant.From<GpuParticles2D>(ref _spikeCircle));
		info.AddProperty(PropertyName._spikeCircle2, Variant.From<GpuParticles2D>(ref _spikeCircle2));
		info.AddProperty(PropertyName._hilt, Variant.From<TextureRect>(ref _hilt));
		info.AddProperty(PropertyName._hilt2, Variant.From<TextureRect>(ref _hilt2));
		info.AddProperty(PropertyName._detail, Variant.From<TextureRect>(ref _detail));
		info.AddProperty(PropertyName._trail, Variant.From<Line2D>(ref _trail));
		info.AddProperty(PropertyName._orbitPath, Variant.From<Path2D>(ref _orbitPath));
		info.AddProperty(PropertyName._hitbox, Variant.From<Control>(ref _hitbox));
		info.AddProperty(PropertyName._selectionReticle, Variant.From<NSelectionReticle>(ref _selectionReticle));
		info.AddProperty(PropertyName._attackTween, Variant.From<Tween>(ref _attackTween));
		info.AddProperty(PropertyName._scaleTween, Variant.From<Tween>(ref _scaleTween));
		info.AddProperty(PropertyName._sparkDelay, Variant.From<Tween>(ref _sparkDelay));
		info.AddProperty(PropertyName._glowTween, Variant.From<Tween>(ref _glowTween));
		info.AddProperty(PropertyName._trailStart, Variant.From<Vector2>(ref _trailStart));
		info.AddProperty(PropertyName._bladeSize, Variant.From<float>(ref _bladeSize));
		info.AddProperty(PropertyName._targetOrbitPosition, Variant.From<Vector2>(ref _targetOrbitPosition));
		info.AddProperty(PropertyName._isBehindCharacter, Variant.From<bool>(ref _isBehindCharacter));
		info.AddProperty(PropertyName._isFocused, Variant.From<bool>(ref _isFocused));
		info.AddProperty(PropertyName._hoverTip, Variant.From<NHoverTipSet>(ref _hoverTip));
		info.AddProperty(PropertyName._isForging, Variant.From<bool>(ref _isForging));
		info.AddProperty(PropertyName._isAttacking, Variant.From<bool>(ref _isAttacking));
		info.AddProperty(PropertyName._isKeyPressed, Variant.From<bool>(ref _isKeyPressed));
		info.AddProperty(PropertyName._testCharge, Variant.From<float>(ref _testCharge));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		//IL_029d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02da: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName.OrbitProgress, ref val))
		{
			OrbitProgress = ((Variant)(ref val)).As<double>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._spineNode, ref val2))
		{
			_spineNode = ((Variant)(ref val2)).As<Node2D>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._bladeGlow, ref val3))
		{
			_bladeGlow = ((Variant)(ref val3)).As<Node2D>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._forgeSparks, ref val4))
		{
			_forgeSparks = ((Variant)(ref val4)).As<GpuParticles2D>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._spawnFlames, ref val5))
		{
			_spawnFlames = ((Variant)(ref val5)).As<GpuParticles2D>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._spawnFlamesBack, ref val6))
		{
			_spawnFlamesBack = ((Variant)(ref val6)).As<GpuParticles2D>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName._slashParticles, ref val7))
		{
			_slashParticles = ((Variant)(ref val7)).As<GpuParticles2D>();
		}
		Variant val8 = default(Variant);
		if (info.TryGetProperty(PropertyName._chargeParticles, ref val8))
		{
			_chargeParticles = ((Variant)(ref val8)).As<GpuParticles2D>();
		}
		Variant val9 = default(Variant);
		if (info.TryGetProperty(PropertyName._spikeParticles, ref val9))
		{
			_spikeParticles = ((Variant)(ref val9)).As<GpuParticles2D>();
		}
		Variant val10 = default(Variant);
		if (info.TryGetProperty(PropertyName._spikeParticles2, ref val10))
		{
			_spikeParticles2 = ((Variant)(ref val10)).As<GpuParticles2D>();
		}
		Variant val11 = default(Variant);
		if (info.TryGetProperty(PropertyName._spikeCircle, ref val11))
		{
			_spikeCircle = ((Variant)(ref val11)).As<GpuParticles2D>();
		}
		Variant val12 = default(Variant);
		if (info.TryGetProperty(PropertyName._spikeCircle2, ref val12))
		{
			_spikeCircle2 = ((Variant)(ref val12)).As<GpuParticles2D>();
		}
		Variant val13 = default(Variant);
		if (info.TryGetProperty(PropertyName._hilt, ref val13))
		{
			_hilt = ((Variant)(ref val13)).As<TextureRect>();
		}
		Variant val14 = default(Variant);
		if (info.TryGetProperty(PropertyName._hilt2, ref val14))
		{
			_hilt2 = ((Variant)(ref val14)).As<TextureRect>();
		}
		Variant val15 = default(Variant);
		if (info.TryGetProperty(PropertyName._detail, ref val15))
		{
			_detail = ((Variant)(ref val15)).As<TextureRect>();
		}
		Variant val16 = default(Variant);
		if (info.TryGetProperty(PropertyName._trail, ref val16))
		{
			_trail = ((Variant)(ref val16)).As<Line2D>();
		}
		Variant val17 = default(Variant);
		if (info.TryGetProperty(PropertyName._orbitPath, ref val17))
		{
			_orbitPath = ((Variant)(ref val17)).As<Path2D>();
		}
		Variant val18 = default(Variant);
		if (info.TryGetProperty(PropertyName._hitbox, ref val18))
		{
			_hitbox = ((Variant)(ref val18)).As<Control>();
		}
		Variant val19 = default(Variant);
		if (info.TryGetProperty(PropertyName._selectionReticle, ref val19))
		{
			_selectionReticle = ((Variant)(ref val19)).As<NSelectionReticle>();
		}
		Variant val20 = default(Variant);
		if (info.TryGetProperty(PropertyName._attackTween, ref val20))
		{
			_attackTween = ((Variant)(ref val20)).As<Tween>();
		}
		Variant val21 = default(Variant);
		if (info.TryGetProperty(PropertyName._scaleTween, ref val21))
		{
			_scaleTween = ((Variant)(ref val21)).As<Tween>();
		}
		Variant val22 = default(Variant);
		if (info.TryGetProperty(PropertyName._sparkDelay, ref val22))
		{
			_sparkDelay = ((Variant)(ref val22)).As<Tween>();
		}
		Variant val23 = default(Variant);
		if (info.TryGetProperty(PropertyName._glowTween, ref val23))
		{
			_glowTween = ((Variant)(ref val23)).As<Tween>();
		}
		Variant val24 = default(Variant);
		if (info.TryGetProperty(PropertyName._trailStart, ref val24))
		{
			_trailStart = ((Variant)(ref val24)).As<Vector2>();
		}
		Variant val25 = default(Variant);
		if (info.TryGetProperty(PropertyName._bladeSize, ref val25))
		{
			_bladeSize = ((Variant)(ref val25)).As<float>();
		}
		Variant val26 = default(Variant);
		if (info.TryGetProperty(PropertyName._targetOrbitPosition, ref val26))
		{
			_targetOrbitPosition = ((Variant)(ref val26)).As<Vector2>();
		}
		Variant val27 = default(Variant);
		if (info.TryGetProperty(PropertyName._isBehindCharacter, ref val27))
		{
			_isBehindCharacter = ((Variant)(ref val27)).As<bool>();
		}
		Variant val28 = default(Variant);
		if (info.TryGetProperty(PropertyName._isFocused, ref val28))
		{
			_isFocused = ((Variant)(ref val28)).As<bool>();
		}
		Variant val29 = default(Variant);
		if (info.TryGetProperty(PropertyName._hoverTip, ref val29))
		{
			_hoverTip = ((Variant)(ref val29)).As<NHoverTipSet>();
		}
		Variant val30 = default(Variant);
		if (info.TryGetProperty(PropertyName._isForging, ref val30))
		{
			_isForging = ((Variant)(ref val30)).As<bool>();
		}
		Variant val31 = default(Variant);
		if (info.TryGetProperty(PropertyName._isAttacking, ref val31))
		{
			_isAttacking = ((Variant)(ref val31)).As<bool>();
		}
		Variant val32 = default(Variant);
		if (info.TryGetProperty(PropertyName._isKeyPressed, ref val32))
		{
			_isKeyPressed = ((Variant)(ref val32)).As<bool>();
		}
		Variant val33 = default(Variant);
		if (info.TryGetProperty(PropertyName._testCharge, ref val33))
		{
			_testCharge = ((Variant)(ref val33)).As<float>();
		}
	}
}
