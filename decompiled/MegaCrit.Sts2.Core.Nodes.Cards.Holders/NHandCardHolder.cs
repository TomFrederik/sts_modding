using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.Nodes.Cards.Holders;

[ScriptPath("res://src/Core/Nodes/Cards/Holders/NHandCardHolder.cs")]
public class NHandCardHolder : NCardHolder
{
	[Signal]
	public delegate void HolderFocusedEventHandler(NHandCardHolder cardHolder);

	[Signal]
	public delegate void HolderUnfocusedEventHandler(NHandCardHolder cardHolder);

	[Signal]
	public delegate void HolderMouseClickedEventHandler(NCardHolder cardHolder);

	public new class MethodName : NCardHolder.MethodName
	{
		public static readonly StringName Create = StringName.op_Implicit("Create");

		public new static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName _ExitTree = StringName.op_Implicit("_ExitTree");

		public new static readonly StringName Clear = StringName.op_Implicit("Clear");

		public new static readonly StringName OnFocus = StringName.op_Implicit("OnFocus");

		public new static readonly StringName OnUnfocus = StringName.op_Implicit("OnUnfocus");

		public new static readonly StringName OnMousePressed = StringName.op_Implicit("OnMousePressed");

		public new static readonly StringName OnMouseReleased = StringName.op_Implicit("OnMouseReleased");

		public new static readonly StringName DoCardHoverEffects = StringName.op_Implicit("DoCardHoverEffects");

		public static readonly StringName SetIndexLabel = StringName.op_Implicit("SetIndexLabel");

		public static readonly StringName SetTargetAngle = StringName.op_Implicit("SetTargetAngle");

		public static readonly StringName SetTargetPosition = StringName.op_Implicit("SetTargetPosition");

		public static readonly StringName SetTargetScale = StringName.op_Implicit("SetTargetScale");

		public static readonly StringName SetAngleInstantly = StringName.op_Implicit("SetAngleInstantly");

		public static readonly StringName SetScaleInstantly = StringName.op_Implicit("SetScaleInstantly");

		public static readonly StringName StopAnimations = StringName.op_Implicit("StopAnimations");

		public new static readonly StringName SetCard = StringName.op_Implicit("SetCard");

		public static readonly StringName UpdateCard = StringName.op_Implicit("UpdateCard");

		public static readonly StringName BeginDrag = StringName.op_Implicit("BeginDrag");

		public static readonly StringName CancelDrag = StringName.op_Implicit("CancelDrag");

		public static readonly StringName SetDefaultTargets = StringName.op_Implicit("SetDefaultTargets");

		public static readonly StringName Flash = StringName.op_Implicit("Flash");
	}

	public new class PropertyName : NCardHolder.PropertyName
	{
		public static readonly StringName InSelectMode = StringName.op_Implicit("InSelectMode");

		public static readonly StringName TargetPosition = StringName.op_Implicit("TargetPosition");

		public static readonly StringName TargetAngle = StringName.op_Implicit("TargetAngle");

		public static readonly StringName ShouldGlowGold = StringName.op_Implicit("ShouldGlowGold");

		public static readonly StringName ShouldGlowRed = StringName.op_Implicit("ShouldGlowRed");

		public static readonly StringName _flash = StringName.op_Implicit("_flash");

		public static readonly StringName _flashTween = StringName.op_Implicit("_flashTween");

		public static readonly StringName _handIndexLabel = StringName.op_Implicit("_handIndexLabel");

		public static readonly StringName _targetPosition = StringName.op_Implicit("_targetPosition");

		public static readonly StringName _targetAngle = StringName.op_Implicit("_targetAngle");

		public static readonly StringName _targetScale = StringName.op_Implicit("_targetScale");

		public static readonly StringName _hand = StringName.op_Implicit("_hand");
	}

	public new class SignalName : NCardHolder.SignalName
	{
		public static readonly StringName HolderFocused = StringName.op_Implicit("HolderFocused");

		public static readonly StringName HolderUnfocused = StringName.op_Implicit("HolderUnfocused");

		public static readonly StringName HolderMouseClicked = StringName.op_Implicit("HolderMouseClicked");
	}

	private Control _flash;

	private Tween? _flashTween;

	private MegaLabel _handIndexLabel;

	private const float _rotateSpeed = 10f;

	private const float _angleSnapThreshold = 0.1f;

	private const float _scaleSpeed = 8f;

	private const float _scaleSnapThreshold = 0.002f;

	private const float _moveSpeed = 7f;

	private const float _positionSnapThreshold = 1f;

	private const float _reenableHitboxThreshold = 200f;

	private Vector2 _targetPosition;

	private float _targetAngle;

	private Vector2 _targetScale;

	private CancellationTokenSource? _angleCancelToken;

	private CancellationTokenSource? _positionCancelToken;

	private CancellationTokenSource? _scaleCancelToken;

	private NPlayerHand _hand;

	private HolderFocusedEventHandler backing_HolderFocused;

	private HolderUnfocusedEventHandler backing_HolderUnfocused;

	private HolderMouseClickedEventHandler backing_HolderMouseClicked;

	public bool InSelectMode { get; set; }

	public Vector2 TargetPosition => _targetPosition;

	public float TargetAngle => _targetAngle;

	private static string ScenePath => SceneHelper.GetScenePath("cards/holders/hand_card_holder");

	public static IEnumerable<string> AssetPaths => new _003C_003Ez__ReadOnlySingleElementList<string>(ScenePath);

	private bool ShouldGlowGold
	{
		get
		{
			CardModel cardModel = base.CardNode?.Model;
			if (cardModel == null)
			{
				return false;
			}
			if (_hand.SelectModeGoldGlowOverride != null)
			{
				return _hand.SelectModeGoldGlowOverride(cardModel);
			}
			if (CombatManager.Instance.IsPlayPhase && cardModel.CanPlay())
			{
				return cardModel.ShouldGlowGold;
			}
			return false;
		}
	}

	private bool ShouldGlowRed
	{
		get
		{
			CardModel cardModel = base.CardNode?.Model;
			if (cardModel == null)
			{
				return false;
			}
			if (CombatManager.Instance.IsPlayPhase)
			{
				return cardModel.ShouldGlowRed;
			}
			return false;
		}
	}

	public event HolderFocusedEventHandler HolderFocused
	{
		add
		{
			backing_HolderFocused = (HolderFocusedEventHandler)Delegate.Combine(backing_HolderFocused, value);
		}
		remove
		{
			backing_HolderFocused = (HolderFocusedEventHandler)Delegate.Remove(backing_HolderFocused, value);
		}
	}

	public event HolderUnfocusedEventHandler HolderUnfocused
	{
		add
		{
			backing_HolderUnfocused = (HolderUnfocusedEventHandler)Delegate.Combine(backing_HolderUnfocused, value);
		}
		remove
		{
			backing_HolderUnfocused = (HolderUnfocusedEventHandler)Delegate.Remove(backing_HolderUnfocused, value);
		}
	}

	public event HolderMouseClickedEventHandler HolderMouseClicked
	{
		add
		{
			backing_HolderMouseClicked = (HolderMouseClickedEventHandler)Delegate.Combine(backing_HolderMouseClicked, value);
		}
		remove
		{
			backing_HolderMouseClicked = (HolderMouseClickedEventHandler)Delegate.Remove(backing_HolderMouseClicked, value);
		}
	}

	public static NHandCardHolder Create(NCard card, NPlayerHand hand)
	{
		NHandCardHolder nHandCardHolder = PreloadManager.Cache.GetScene(ScenePath).Instantiate<NHandCardHolder>((GenEditState)0);
		((Node)nHandCardHolder).Name = StringName.op_Implicit($"{((object)nHandCardHolder).GetType().Name}-{card.Model.Id}");
		nHandCardHolder.SetCard(card);
		nHandCardHolder._hand = hand;
		return nHandCardHolder;
	}

	public override void _Ready()
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		ConnectSignals();
		_flash = ((Node)this).GetNode<Control>(NodePath.op_Implicit("Flash"));
		((CanvasItem)_flash).Modulate = new Color(((CanvasItem)_flash).Modulate.R, ((CanvasItem)_flash).Modulate.G, ((CanvasItem)_flash).Modulate.B, 0f);
		_handIndexLabel = ((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%HandIndex"));
		UpdateCard();
		base.Hitbox.SetEnabled(enabled: false);
	}

	public override void _ExitTree()
	{
		((Node)this)._ExitTree();
		UnsubscribeFromEvents(base.CardNode?.Model);
		StopAnimations();
	}

	public override void Clear()
	{
		UnsubscribeFromEvents(base.CardNode?.Model);
		base.Clear();
		StopAnimations();
	}

	protected override void OnFocus()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).EmitSignal(SignalName.HolderFocused, (Variant[])(object)new Variant[1] { Variant.op_Implicit((GodotObject)(object)this) });
		base.OnFocus();
	}

	protected override void OnUnfocus()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).EmitSignal(SignalName.HolderUnfocused, (Variant[])(object)new Variant[1] { Variant.op_Implicit((GodotObject)(object)this) });
		base.OnUnfocus();
	}

	protected override void OnMousePressed(InputEvent inputEvent)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Invalid comparison between Unknown and I8
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		InputEventMouseButton val = (InputEventMouseButton)(object)((inputEvent is InputEventMouseButton) ? inputEvent : null);
		if (val != null && (long)val.ButtonIndex == 1 && _isClickable)
		{
			SfxCmd.Play("event:/sfx/ui/clicks/ui_click");
			((GodotObject)this).EmitSignal(SignalName.HolderMouseClicked, (Variant[])(object)new Variant[1] { Variant.op_Implicit((GodotObject)(object)this) });
		}
	}

	protected override void OnMouseReleased(InputEvent inputEvent)
	{
	}

	protected override void DoCardHoverEffects(bool isHovered)
	{
		((CanvasItem)this).ZIndex = (isHovered ? 1 : 0);
		if (isHovered)
		{
			CreateHoverTips();
		}
		else
		{
			ClearHoverTips();
		}
	}

	public void SetIndexLabel(int i)
	{
		_handIndexLabel.SetTextAutoSize(i.ToString());
		((CanvasItem)_handIndexLabel).Visible = i > 0 && SaveManager.Instance.PrefsSave.ShowCardIndices;
	}

	public void SetTargetAngle(float angle)
	{
		_targetAngle = angle;
		_angleCancelToken?.Cancel();
		_angleCancelToken = new CancellationTokenSource();
		TaskHelper.RunSafely(AnimAngle(_angleCancelToken));
	}

	public void SetTargetPosition(Vector2 position)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		_targetPosition = position;
		_positionCancelToken?.Cancel();
		_positionCancelToken = new CancellationTokenSource();
		TaskHelper.RunSafely(AnimPosition(_positionCancelToken));
	}

	public void SetTargetScale(Vector2 scale)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		_targetScale = scale;
		_scaleCancelToken?.Cancel();
		_scaleCancelToken = new CancellationTokenSource();
		TaskHelper.RunSafely(AnimScale(_scaleCancelToken));
	}

	public void SetAngleInstantly(float setAngle)
	{
		_angleCancelToken?.Cancel();
		((Control)this).RotationDegrees = setAngle;
	}

	public void SetScaleInstantly(Vector2 setScale)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		_scaleCancelToken?.Cancel();
		((Control)this).Scale = setScale;
	}

	private void StopAnimations()
	{
		_angleCancelToken?.Cancel();
		_positionCancelToken?.Cancel();
		_scaleCancelToken?.Cancel();
	}

	private async Task AnimAngle(CancellationTokenSource cancelToken)
	{
		while (!cancelToken.IsCancellationRequested)
		{
			((Control)this).RotationDegrees = Mathf.Lerp(((Control)this).RotationDegrees, _targetAngle, (float)((Node)this).GetProcessDeltaTime() * 10f);
			if (Mathf.Abs(((Control)this).RotationDegrees - _targetAngle) < 0.1f)
			{
				((Control)this).RotationDegrees = _targetAngle;
				break;
			}
			await ((GodotObject)this).ToSignal((GodotObject)(object)((Node)this).GetTree(), SignalName.ProcessFrame);
		}
	}

	private async Task AnimScale(CancellationTokenSource cancelToken)
	{
		while (!cancelToken.IsCancellationRequested)
		{
			NHandCardHolder nHandCardHolder = this;
			Vector2 scale = ((Control)this).Scale;
			((Control)nHandCardHolder).Scale = ((Vector2)(ref scale)).Lerp(_targetScale, (float)((Node)this).GetProcessDeltaTime() * 8f);
			if (Mathf.Abs(_targetScale.X - ((Control)this).Scale.X) < 0.002f)
			{
				((Control)this).Scale = _targetScale;
				break;
			}
			await ((GodotObject)this).ToSignal((GodotObject)(object)((Node)this).GetTree(), SignalName.ProcessFrame);
		}
	}

	private async Task AnimPosition(CancellationTokenSource cancelToken)
	{
		Vector2 position;
		while (!cancelToken.IsCancellationRequested)
		{
			NHandCardHolder nHandCardHolder = this;
			position = ((Control)this).Position;
			((Control)nHandCardHolder).Position = ((Vector2)(ref position)).Lerp(_targetPosition, (float)((Node)this).GetProcessDeltaTime() * 7f);
			float num = Mathf.Abs(((Control)this).Position.X - _targetPosition.X);
			if (!base.Hitbox.IsEnabled && num < 200f)
			{
				base.Hitbox.SetEnabled(enabled: true);
			}
			position = ((Control)this).Position;
			if (((Vector2)(ref position)).DistanceSquaredTo(_targetPosition) < 1f)
			{
				((Control)this).Position = _targetPosition;
				return;
			}
			await ((GodotObject)this).ToSignal((GodotObject)(object)((Node)this).GetTree(), SignalName.ProcessFrame);
		}
		if (!base.Hitbox.IsEnabled)
		{
			position = ((Control)this).Position;
			if (((Vector2)(ref position)).DistanceSquaredTo(_targetPosition) < 200f)
			{
				base.Hitbox.SetEnabled(enabled: true);
			}
		}
	}

	protected override void SetCard(NCard node)
	{
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		if (base.CardNode != null)
		{
			base.CardNode.ModelChanged -= OnModelChanged;
		}
		UnsubscribeFromEvents(base.CardNode?.Model);
		base.SetCard(node);
		UpdateCard();
		SubscribeToEvents(base.CardNode?.Model);
		if (base.CardNode != null)
		{
			base.CardNode.ModelChanged += OnModelChanged;
		}
		if (((Control)node).Scale != Vector2.One)
		{
			((Node)node).CreateTween().TweenProperty((GodotObject)(object)node, NodePath.op_Implicit("scale"), Variant.op_Implicit(Vector2.One), 0.25);
		}
	}

	public void UpdateCard()
	{
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		if (!((Node)this).IsNodeReady() || base.CardNode == null)
		{
			return;
		}
		base.CardNode.UpdateVisuals(PileType.Hand, CardPreviewMode.Normal);
		if (!CombatManager.Instance.IsInProgress)
		{
			return;
		}
		if (base.CardNode.Model.CanPlay() || ShouldGlowRed || ShouldGlowGold)
		{
			base.CardNode.CardHighlight.AnimShow();
			((CanvasItem)base.CardNode.CardHighlight).Modulate = NCardHighlight.playableColor;
			if (ShouldGlowRed)
			{
				((CanvasItem)base.CardNode.CardHighlight).Modulate = NCardHighlight.red;
			}
			else if (ShouldGlowGold)
			{
				((CanvasItem)base.CardNode.CardHighlight).Modulate = NCardHighlight.gold;
			}
		}
		else
		{
			base.CardNode.CardHighlight.AnimHide();
		}
	}

	public void BeginDrag()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		SetAngleInstantly(0f);
		SetScaleInstantly(HoverScale);
	}

	public void CancelDrag()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		((CanvasItem)this).ZIndex = 0;
		SetAngleInstantly(0f);
		SetScaleInstantly(Vector2.One);
	}

	public void SetDefaultTargets()
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		((CanvasItem)this).ZIndex = 0;
		IReadOnlyList<NHandCardHolder> activeHolders = _hand.ActiveHolders;
		int num = activeHolders.IndexOf(this);
		int count = activeHolders.Count;
		if (num >= 0)
		{
			SetTargetPosition(HandPosHelper.GetPosition(count, num));
			SetTargetAngle(HandPosHelper.GetAngle(count, num));
			SetTargetScale(HandPosHelper.GetScale(count));
		}
	}

	public void Flash()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		if (GodotObject.IsInstanceValid((GodotObject)(object)_flash))
		{
			_flash.Scale = Vector2.One;
			((CanvasItem)_flash).Modulate = NCardHighlight.playableColor;
			if (ShouldGlowGold)
			{
				((CanvasItem)_flash).Modulate = NCardHighlight.gold;
			}
			else if (ShouldGlowRed)
			{
				((CanvasItem)_flash).Modulate = NCardHighlight.red;
			}
			Tween? flashTween = _flashTween;
			if (flashTween != null)
			{
				flashTween.Kill();
			}
			_flashTween = ((Node)this).CreateTween();
			_flashTween.TweenProperty((GodotObject)(object)_flash, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0.6), 0.15);
			_flashTween.TweenProperty((GodotObject)(object)_flash, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0), 0.3);
		}
	}

	private void SubscribeToEvents(CardModel? card)
	{
		if (card != null && ((Node)this).IsInsideTree())
		{
			card.Upgraded += Flash;
			card.KeywordsChanged += Flash;
			card.ReplayCountChanged += Flash;
			card.AfflictionChanged += Flash;
			card.EnergyCostChanged += Flash;
			card.StarCostChanged += Flash;
		}
	}

	private void UnsubscribeFromEvents(CardModel? card)
	{
		if (card != null)
		{
			card.Upgraded -= Flash;
			card.KeywordsChanged -= Flash;
			card.ReplayCountChanged -= Flash;
			card.AfflictionChanged -= Flash;
			card.EnergyCostChanged -= Flash;
			card.StarCostChanged -= Flash;
		}
	}

	private void OnModelChanged(CardModel? oldModel)
	{
		UnsubscribeFromEvents(oldModel);
		SubscribeToEvents(base.CardNode?.Model);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Expected O, but got Unknown
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Expected O, but got Unknown
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Expected O, but got Unknown
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Expected O, but got Unknown
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Expected O, but got Unknown
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_0287: Unknown result type (might be due to invalid IL or missing references)
		//IL_0292: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02db: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_030c: Unknown result type (might be due to invalid IL or missing references)
		//IL_032f: Unknown result type (might be due to invalid IL or missing references)
		//IL_033a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0360: Unknown result type (might be due to invalid IL or missing references)
		//IL_0383: Unknown result type (might be due to invalid IL or missing references)
		//IL_038e: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0408: Unknown result type (might be due to invalid IL or missing references)
		//IL_042b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0436: Unknown result type (might be due to invalid IL or missing references)
		//IL_045c: Unknown result type (might be due to invalid IL or missing references)
		//IL_047f: Unknown result type (might be due to invalid IL or missing references)
		//IL_048a: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04df: Unknown result type (might be due to invalid IL or missing references)
		//IL_0507: Unknown result type (might be due to invalid IL or missing references)
		//IL_0512: Expected O, but got Unknown
		//IL_050d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0518: Unknown result type (might be due to invalid IL or missing references)
		//IL_053e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0547: Unknown result type (might be due to invalid IL or missing references)
		//IL_056d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0576: Unknown result type (might be due to invalid IL or missing references)
		//IL_059c: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_05cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_05fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0603: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(22);
		list.Add(new MethodInfo(MethodName.Create, new PropertyInfo((Type)24, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false), (MethodFlags)33, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("card"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false),
			new PropertyInfo((Type)24, StringName.op_Implicit("hand"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._ExitTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.Clear, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnFocus, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnUnfocus, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnMousePressed, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("inputEvent"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("InputEvent"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnMouseReleased, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("inputEvent"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("InputEvent"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.DoCardHoverEffects, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)1, StringName.op_Implicit("isHovered"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetIndexLabel, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("i"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetTargetAngle, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)3, StringName.op_Implicit("angle"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetTargetPosition, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)5, StringName.op_Implicit("position"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetTargetScale, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)5, StringName.op_Implicit("scale"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetAngleInstantly, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)3, StringName.op_Implicit("setAngle"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetScaleInstantly, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)5, StringName.op_Implicit("setScale"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.StopAnimations, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetCard, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("node"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UpdateCard, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.BeginDrag, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.CancelDrag, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetDefaultTargets, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.Flash, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_028f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_031a: Unknown result type (might be due to invalid IL or missing references)
		//IL_033f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0364: Unknown result type (might be due to invalid IL or missing references)
		//IL_0389: Unknown result type (might be due to invalid IL or missing references)
		//IL_03dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d3: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.Create && ((NativeVariantPtrArgs)(ref args)).Count == 2)
		{
			NHandCardHolder nHandCardHolder = Create(VariantUtils.ConvertTo<NCard>(ref ((NativeVariantPtrArgs)(ref args))[0]), VariantUtils.ConvertTo<NPlayerHand>(ref ((NativeVariantPtrArgs)(ref args))[1]));
			ret = VariantUtils.CreateFrom<NHandCardHolder>(ref nHandCardHolder);
			return true;
		}
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
		if ((ref method) == MethodName.Clear && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			Clear();
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
		if ((ref method) == MethodName.OnMousePressed && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OnMousePressed(VariantUtils.ConvertTo<InputEvent>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnMouseReleased && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OnMouseReleased(VariantUtils.ConvertTo<InputEvent>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.DoCardHoverEffects && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			DoCardHoverEffects(VariantUtils.ConvertTo<bool>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetIndexLabel && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			SetIndexLabel(VariantUtils.ConvertTo<int>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetTargetAngle && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			SetTargetAngle(VariantUtils.ConvertTo<float>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetTargetPosition && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			SetTargetPosition(VariantUtils.ConvertTo<Vector2>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetTargetScale && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			SetTargetScale(VariantUtils.ConvertTo<Vector2>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetAngleInstantly && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			SetAngleInstantly(VariantUtils.ConvertTo<float>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetScaleInstantly && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			SetScaleInstantly(VariantUtils.ConvertTo<Vector2>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.StopAnimations && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			StopAnimations();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetCard && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			SetCard(VariantUtils.ConvertTo<NCard>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.UpdateCard && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			UpdateCard();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.BeginDrag && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			BeginDrag();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.CancelDrag && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			CancelDrag();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetDefaultTargets && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			SetDefaultTargets();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.Flash && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			Flash();
			ret = default(godot_variant);
			return true;
		}
		return base.InvokeGodotClassMethod(in method, args, out ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static bool InvokeGodotClassStaticMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.Create && ((NativeVariantPtrArgs)(ref args)).Count == 2)
		{
			NHandCardHolder nHandCardHolder = Create(VariantUtils.ConvertTo<NCard>(ref ((NativeVariantPtrArgs)(ref args))[0]), VariantUtils.ConvertTo<NPlayerHand>(ref ((NativeVariantPtrArgs)(ref args))[1]));
			ret = VariantUtils.CreateFrom<NHandCardHolder>(ref nHandCardHolder);
			return true;
		}
		ret = default(godot_variant);
		return false;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if ((ref method) == MethodName.Create)
		{
			return true;
		}
		if ((ref method) == MethodName._Ready)
		{
			return true;
		}
		if ((ref method) == MethodName._ExitTree)
		{
			return true;
		}
		if ((ref method) == MethodName.Clear)
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
		if ((ref method) == MethodName.OnMousePressed)
		{
			return true;
		}
		if ((ref method) == MethodName.OnMouseReleased)
		{
			return true;
		}
		if ((ref method) == MethodName.DoCardHoverEffects)
		{
			return true;
		}
		if ((ref method) == MethodName.SetIndexLabel)
		{
			return true;
		}
		if ((ref method) == MethodName.SetTargetAngle)
		{
			return true;
		}
		if ((ref method) == MethodName.SetTargetPosition)
		{
			return true;
		}
		if ((ref method) == MethodName.SetTargetScale)
		{
			return true;
		}
		if ((ref method) == MethodName.SetAngleInstantly)
		{
			return true;
		}
		if ((ref method) == MethodName.SetScaleInstantly)
		{
			return true;
		}
		if ((ref method) == MethodName.StopAnimations)
		{
			return true;
		}
		if ((ref method) == MethodName.SetCard)
		{
			return true;
		}
		if ((ref method) == MethodName.UpdateCard)
		{
			return true;
		}
		if ((ref method) == MethodName.BeginDrag)
		{
			return true;
		}
		if ((ref method) == MethodName.CancelDrag)
		{
			return true;
		}
		if ((ref method) == MethodName.SetDefaultTargets)
		{
			return true;
		}
		if ((ref method) == MethodName.Flash)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName.InSelectMode)
		{
			InSelectMode = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._flash)
		{
			_flash = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._flashTween)
		{
			_flashTween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._handIndexLabel)
		{
			_handIndexLabel = VariantUtils.ConvertTo<MegaLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._targetPosition)
		{
			_targetPosition = VariantUtils.ConvertTo<Vector2>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._targetAngle)
		{
			_targetAngle = VariantUtils.ConvertTo<float>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._targetScale)
		{
			_targetScale = VariantUtils.ConvertTo<Vector2>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._hand)
		{
			_hand = VariantUtils.ConvertTo<NPlayerHand>(ref value);
			return true;
		}
		return base.SetGodotClassPropertyValue(in name, in value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName.InSelectMode)
		{
			bool inSelectMode = InSelectMode;
			value = VariantUtils.CreateFrom<bool>(ref inSelectMode);
			return true;
		}
		if ((ref name) == PropertyName.TargetPosition)
		{
			Vector2 targetPosition = TargetPosition;
			value = VariantUtils.CreateFrom<Vector2>(ref targetPosition);
			return true;
		}
		if ((ref name) == PropertyName.TargetAngle)
		{
			float targetAngle = TargetAngle;
			value = VariantUtils.CreateFrom<float>(ref targetAngle);
			return true;
		}
		if ((ref name) == PropertyName.ShouldGlowGold)
		{
			bool inSelectMode = ShouldGlowGold;
			value = VariantUtils.CreateFrom<bool>(ref inSelectMode);
			return true;
		}
		if ((ref name) == PropertyName.ShouldGlowRed)
		{
			bool inSelectMode = ShouldGlowRed;
			value = VariantUtils.CreateFrom<bool>(ref inSelectMode);
			return true;
		}
		if ((ref name) == PropertyName._flash)
		{
			value = VariantUtils.CreateFrom<Control>(ref _flash);
			return true;
		}
		if ((ref name) == PropertyName._flashTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _flashTween);
			return true;
		}
		if ((ref name) == PropertyName._handIndexLabel)
		{
			value = VariantUtils.CreateFrom<MegaLabel>(ref _handIndexLabel);
			return true;
		}
		if ((ref name) == PropertyName._targetPosition)
		{
			value = VariantUtils.CreateFrom<Vector2>(ref _targetPosition);
			return true;
		}
		if ((ref name) == PropertyName._targetAngle)
		{
			value = VariantUtils.CreateFrom<float>(ref _targetAngle);
			return true;
		}
		if ((ref name) == PropertyName._targetScale)
		{
			value = VariantUtils.CreateFrom<Vector2>(ref _targetScale);
			return true;
		}
		if ((ref name) == PropertyName._hand)
		{
			value = VariantUtils.CreateFrom<NPlayerHand>(ref _hand);
			return true;
		}
		return base.GetGodotClassPropertyValue(in name, out value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._flash, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._flashTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._handIndexLabel, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)5, PropertyName._targetPosition, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)3, PropertyName._targetAngle, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)5, PropertyName._targetScale, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._hand, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName.InSelectMode, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)5, PropertyName.TargetPosition, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)3, PropertyName.TargetAngle, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName.ShouldGlowGold, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName.ShouldGlowRed, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		base.SaveGodotObjectData(info);
		StringName inSelectMode = PropertyName.InSelectMode;
		bool inSelectMode2 = InSelectMode;
		info.AddProperty(inSelectMode, Variant.From<bool>(ref inSelectMode2));
		info.AddProperty(PropertyName._flash, Variant.From<Control>(ref _flash));
		info.AddProperty(PropertyName._flashTween, Variant.From<Tween>(ref _flashTween));
		info.AddProperty(PropertyName._handIndexLabel, Variant.From<MegaLabel>(ref _handIndexLabel));
		info.AddProperty(PropertyName._targetPosition, Variant.From<Vector2>(ref _targetPosition));
		info.AddProperty(PropertyName._targetAngle, Variant.From<float>(ref _targetAngle));
		info.AddProperty(PropertyName._targetScale, Variant.From<Vector2>(ref _targetScale));
		info.AddProperty(PropertyName._hand, Variant.From<NPlayerHand>(ref _hand));
		info.AddSignalEventDelegate(SignalName.HolderFocused, (Delegate)backing_HolderFocused);
		info.AddSignalEventDelegate(SignalName.HolderUnfocused, (Delegate)backing_HolderUnfocused);
		info.AddSignalEventDelegate(SignalName.HolderMouseClicked, (Delegate)backing_HolderMouseClicked);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		base.RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName.InSelectMode, ref val))
		{
			InSelectMode = ((Variant)(ref val)).As<bool>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._flash, ref val2))
		{
			_flash = ((Variant)(ref val2)).As<Control>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._flashTween, ref val3))
		{
			_flashTween = ((Variant)(ref val3)).As<Tween>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._handIndexLabel, ref val4))
		{
			_handIndexLabel = ((Variant)(ref val4)).As<MegaLabel>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._targetPosition, ref val5))
		{
			_targetPosition = ((Variant)(ref val5)).As<Vector2>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._targetAngle, ref val6))
		{
			_targetAngle = ((Variant)(ref val6)).As<float>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName._targetScale, ref val7))
		{
			_targetScale = ((Variant)(ref val7)).As<Vector2>();
		}
		Variant val8 = default(Variant);
		if (info.TryGetProperty(PropertyName._hand, ref val8))
		{
			_hand = ((Variant)(ref val8)).As<NPlayerHand>();
		}
		HolderFocusedEventHandler holderFocusedEventHandler = default(HolderFocusedEventHandler);
		if (info.TryGetSignalEventDelegate<HolderFocusedEventHandler>(SignalName.HolderFocused, ref holderFocusedEventHandler))
		{
			backing_HolderFocused = holderFocusedEventHandler;
		}
		HolderUnfocusedEventHandler holderUnfocusedEventHandler = default(HolderUnfocusedEventHandler);
		if (info.TryGetSignalEventDelegate<HolderUnfocusedEventHandler>(SignalName.HolderUnfocused, ref holderUnfocusedEventHandler))
		{
			backing_HolderUnfocused = holderUnfocusedEventHandler;
		}
		HolderMouseClickedEventHandler holderMouseClickedEventHandler = default(HolderMouseClickedEventHandler);
		if (info.TryGetSignalEventDelegate<HolderMouseClickedEventHandler>(SignalName.HolderMouseClicked, ref holderMouseClickedEventHandler))
		{
			backing_HolderMouseClicked = holderMouseClickedEventHandler;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<MethodInfo> GetGodotSignalList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Expected O, but got Unknown
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Expected O, but got Unknown
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Expected O, but got Unknown
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(3);
		list.Add(new MethodInfo(SignalName.HolderFocused, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("cardHolder"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(SignalName.HolderUnfocused, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("cardHolder"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(SignalName.HolderMouseClicked, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("cardHolder"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		return list;
	}

	protected void EmitSignalHolderFocused(NHandCardHolder cardHolder)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).EmitSignal(SignalName.HolderFocused, (Variant[])(object)new Variant[1] { Variant.op_Implicit((GodotObject)(object)cardHolder) });
	}

	protected void EmitSignalHolderUnfocused(NHandCardHolder cardHolder)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).EmitSignal(SignalName.HolderUnfocused, (Variant[])(object)new Variant[1] { Variant.op_Implicit((GodotObject)(object)cardHolder) });
	}

	protected void EmitSignalHolderMouseClicked(NCardHolder cardHolder)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).EmitSignal(SignalName.HolderMouseClicked, (Variant[])(object)new Variant[1] { Variant.op_Implicit((GodotObject)(object)cardHolder) });
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RaiseGodotClassSignalCallbacks(in godot_string_name signal, NativeVariantPtrArgs args)
	{
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		if ((ref signal) == SignalName.HolderFocused && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			backing_HolderFocused?.Invoke(VariantUtils.ConvertTo<NHandCardHolder>(ref ((NativeVariantPtrArgs)(ref args))[0]));
		}
		else if ((ref signal) == SignalName.HolderUnfocused && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			backing_HolderUnfocused?.Invoke(VariantUtils.ConvertTo<NHandCardHolder>(ref ((NativeVariantPtrArgs)(ref args))[0]));
		}
		else if ((ref signal) == SignalName.HolderMouseClicked && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			backing_HolderMouseClicked?.Invoke(VariantUtils.ConvertTo<NCardHolder>(ref ((NativeVariantPtrArgs)(ref args))[0]));
		}
		else
		{
			base.RaiseGodotClassSignalCallbacks(in signal, args);
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassSignal(in godot_string_name signal)
	{
		if ((ref signal) == SignalName.HolderFocused)
		{
			return true;
		}
		if ((ref signal) == SignalName.HolderUnfocused)
		{
			return true;
		}
		if ((ref signal) == SignalName.HolderMouseClicked)
		{
			return true;
		}
		return base.HasGodotClassSignal(in signal);
	}
}
