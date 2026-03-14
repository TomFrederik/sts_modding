using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.HoverTips;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.Timeline;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.Nodes.Screens.Timeline;

[ScriptPath("res://src/Core/Nodes/Screens/Timeline/NEpochSlot.cs")]
public class NEpochSlot : NButton
{
	public new class MethodName : NButton.MethodName
	{
		public new static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public new static readonly StringName OnRelease = StringName.op_Implicit("OnRelease");

		public static readonly StringName RevealEpoch = StringName.op_Implicit("RevealEpoch");

		public new static readonly StringName OnFocus = StringName.op_Implicit("OnFocus");

		public new static readonly StringName OnUnfocus = StringName.op_Implicit("OnUnfocus");

		public new static readonly StringName OnPress = StringName.op_Implicit("OnPress");

		public static readonly StringName _Process = StringName.op_Implicit("_Process");

		public static readonly StringName DisableHighlight = StringName.op_Implicit("DisableHighlight");

		public static readonly StringName EnableHighlight = StringName.op_Implicit("EnableHighlight");

		public static readonly StringName SetState = StringName.op_Implicit("SetState");

		public static readonly StringName UpdateShaderS = StringName.op_Implicit("UpdateShaderS");

		public static readonly StringName UpdateShaderV = StringName.op_Implicit("UpdateShaderV");

		public static readonly StringName UpdateBlurLod = StringName.op_Implicit("UpdateBlurLod");
	}

	public new class PropertyName : NButton.PropertyName
	{
		public new static readonly StringName ClickedSfx = StringName.op_Implicit("ClickedSfx");

		public new static readonly StringName HoveredSfx = StringName.op_Implicit("HoveredSfx");

		public static readonly StringName State = StringName.op_Implicit("State");

		public static readonly StringName HasSpawned = StringName.op_Implicit("HasSpawned");

		public static readonly StringName _slotImage = StringName.op_Implicit("_slotImage");

		public static readonly StringName _portrait = StringName.op_Implicit("_portrait");

		public static readonly StringName _chains = StringName.op_Implicit("_chains");

		public static readonly StringName _hsv = StringName.op_Implicit("_hsv");

		public static readonly StringName _blurPortrait = StringName.op_Implicit("_blurPortrait");

		public static readonly StringName _outline = StringName.op_Implicit("_outline");

		public static readonly StringName _blur = StringName.op_Implicit("_blur");

		public static readonly StringName _blurShader = StringName.op_Implicit("_blurShader");

		public static readonly StringName _selectionReticle = StringName.op_Implicit("_selectionReticle");

		public static readonly StringName _offscreenVfx = StringName.op_Implicit("_offscreenVfx");

		public static readonly StringName _highlightVfx = StringName.op_Implicit("_highlightVfx");

		public static readonly StringName _subViewportContainer = StringName.op_Implicit("_subViewportContainer");

		public static readonly StringName _subViewport = StringName.op_Implicit("_subViewport");

		public static readonly StringName _isGlowPulsing = StringName.op_Implicit("_isGlowPulsing");

		public static readonly StringName _isComplete = StringName.op_Implicit("_isComplete");

		public new static readonly StringName _isHovered = StringName.op_Implicit("_isHovered");

		public static readonly StringName _era = StringName.op_Implicit("_era");

		public static readonly StringName eraPosition = StringName.op_Implicit("eraPosition");

		public static readonly StringName _glowTween = StringName.op_Implicit("_glowTween");

		public static readonly StringName _spawnTween = StringName.op_Implicit("_spawnTween");

		public static readonly StringName _hoverTween = StringName.op_Implicit("_hoverTween");
	}

	public new class SignalName : NButton.SignalName
	{
	}

	private static readonly StringName _lod = new StringName("lod");

	private static readonly StringName _v = new StringName("v");

	private static readonly StringName _s = new StringName("s");

	private const string _unlockIconPath = "res://images/packed/unlock_icon.png";

	private const string _scenePath = "res://scenes/timeline_screen/epoch_slot.tscn";

	public static readonly IEnumerable<string> assetPaths = new global::_003C_003Ez__ReadOnlyArray<string>(new string[2] { "res://scenes/timeline_screen/epoch_slot.tscn", "res://images/packed/unlock_icon.png" });

	private TextureRect _slotImage;

	private TextureRect _portrait;

	private TextureRect _chains;

	private ShaderMaterial _hsv;

	private TextureRect _blurPortrait;

	private TextureRect _outline;

	private Control _blur;

	private ShaderMaterial _blurShader;

	private NSelectionReticle _selectionReticle;

	private NEpochOffscreenVfx? _offscreenVfx;

	private Control? _highlightVfx;

	private SubViewportContainer _subViewportContainer;

	private SubViewport _subViewport;

	private bool _isGlowPulsing;

	public EpochModel model;

	private bool _isComplete;

	private bool _isHovered;

	private EpochEra _era;

	public int eraPosition;

	private Tween? _glowTween;

	private Tween? _spawnTween;

	private Tween? _hoverTween;

	private static readonly Color _highlightSlotColor = StsColors.purple;

	private static readonly Color _defaultSlotOutlineColor = new Color("70a0ff18");

	private IHoverTip? _hoverTip;

	protected override string ClickedSfx => "event:/sfx/ui/timeline/ui_timeline_click";

	protected override string HoveredSfx
	{
		get
		{
			if (State != EpochSlotState.NotObtained)
			{
				return "event:/sfx/ui/timeline/ui_timeline_hover";
			}
			return "event:/sfx/ui/timeline/ui_timeline_hover_locked";
		}
	}

	public EpochSlotState State { get; private set; }

	public bool HasSpawned { get; private set; }

	public override void _Ready()
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Expected O, but got Unknown
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Expected O, but got Unknown
		ConnectSignals();
		_slotImage = ((Node)this).GetNode<TextureRect>(NodePath.op_Implicit("%SlotImage"));
		_portrait = ((Node)this).GetNode<TextureRect>(NodePath.op_Implicit("%Portrait"));
		_chains = ((Node)this).GetNode<TextureRect>(NodePath.op_Implicit("%Chains"));
		_hsv = (ShaderMaterial)((CanvasItem)_portrait).GetMaterial();
		_blurPortrait = ((Node)this).GetNode<TextureRect>(NodePath.op_Implicit("%BlurPortrait"));
		_outline = ((Node)this).GetNode<TextureRect>(NodePath.op_Implicit("%Outline"));
		_subViewportContainer = ((Node)this).GetNode<SubViewportContainer>(NodePath.op_Implicit("%SubViewportContainer"));
		_subViewport = ((Node)this).GetNode<SubViewport>(NodePath.op_Implicit("%SubViewport"));
		_blurShader = (ShaderMaterial)((CanvasItem)((Node)this).GetNode<Control>(NodePath.op_Implicit("%Blur"))).GetMaterial();
		_selectionReticle = ((Node)this).GetNode<NSelectionReticle>(NodePath.op_Implicit("%SelectionReticle"));
		if (!NGame.IsReleaseGame())
		{
			MegaLabel node = ((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%DebugLabel"));
			((Label)node).Text = model.GetType().Name;
			((CanvasItem)node).Visible = true;
		}
		SetState(State);
	}

	public static NEpochSlot Create(EpochSlotData data)
	{
		NEpochSlot nEpochSlot = PreloadManager.Cache.GetScene("res://scenes/timeline_screen/epoch_slot.tscn").Instantiate<NEpochSlot>((GenEditState)0);
		nEpochSlot._era = data.Era;
		nEpochSlot.State = data.State;
		nEpochSlot.eraPosition = data.EraPosition;
		nEpochSlot.model = data.Model;
		return nEpochSlot;
	}

	protected override void OnRelease()
	{
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		if (!NGame.IsReleaseGame() && State == EpochSlotState.NotObtained && Input.IsKeyPressed((Key)4194326))
		{
			NHoverTipSet.Remove((Control)(object)this);
			State = EpochSlotState.Obtained;
		}
		base.OnRelease();
		if (State == EpochSlotState.Obtained)
		{
			NTimelineScreen.Instance.DisableInput();
			((Node)this).GetViewport().GuiReleaseFocus();
			RevealEpoch();
			SetState(EpochSlotState.Complete);
		}
		else if (State == EpochSlotState.Complete)
		{
			NTimelineScreen.Instance.DisableInput();
			NTimelineScreen.Instance.OpenInspectScreen(this, playAnimation: false);
		}
		Tween? hoverTween = _hoverTween;
		if (hoverTween != null)
		{
			hoverTween.Kill();
		}
		_hoverTween = ((Node)this).CreateTween().SetParallel(true);
		_hoverTween.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("scale"), Variant.op_Implicit(Vector2.One * 1.05f), 0.25).SetEase((EaseType)1).SetTrans((TransitionType)10);
		_hoverTween.TweenMethod(Callable.From<float>((Action<float>)UpdateShaderV), _hsv.GetShaderParameter(_s), Variant.op_Implicit(1f), 0.05);
	}

	private void RevealEpoch()
	{
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		State = EpochSlotState.Complete;
		((CanvasItem)_portrait).Visible = true;
		_portrait.Texture = model.Portrait;
		DisableHighlight();
		SaveManager.Instance.RevealEpoch(model.Id);
		((CanvasItem)_slotImage).Modulate = Colors.White;
		((CanvasItem)_slotImage).ClipChildren = (ClipChildrenMode)2;
		((CanvasItem)_chains).Visible = false;
		NTimelineScreen.Instance.OpenInspectScreen(this, playAnimation: true);
	}

	protected override void OnFocus()
	{
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02de: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0300: Unknown result type (might be due to invalid IL or missing references)
		//IL_0305: Unknown result type (might be due to invalid IL or missing references)
		//IL_0309: Unknown result type (might be due to invalid IL or missing references)
		//IL_0318: Unknown result type (might be due to invalid IL or missing references)
		//IL_0327: Unknown result type (might be due to invalid IL or missing references)
		//IL_032c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0330: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_036c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0376: Unknown result type (might be due to invalid IL or missing references)
		//IL_034f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0359: Unknown result type (might be due to invalid IL or missing references)
		base.OnFocus();
		_isGlowPulsing = false;
		Tween? hoverTween = _hoverTween;
		if (hoverTween != null)
		{
			hoverTween.Kill();
		}
		_hoverTween = ((Node)this).CreateTween().SetParallel(true);
		if (NControllerManager.Instance.IsUsingController)
		{
			_selectionReticle.OnSelect();
		}
		if (State != EpochSlotState.NotObtained)
		{
			_hoverTween.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("scale"), Variant.op_Implicit(Vector2.One * 1.05f), 0.05);
			if (State == EpochSlotState.Complete)
			{
				_hoverTween.TweenMethod(Callable.From<float>((Action<float>)UpdateShaderS), _hsv.GetShaderParameter(_s), Variant.op_Implicit(1.1f), 0.05);
				_hoverTween.TweenMethod(Callable.From<float>((Action<float>)UpdateShaderV), _hsv.GetShaderParameter(_v), Variant.op_Implicit(1.1f), 0.05);
				LocString unlockInfo = model.UnlockInfo;
				unlockInfo.Add("IsRevealed", variable: true);
				_hoverTip = new HoverTip(model.Title, unlockInfo, PreloadManager.Cache.GetTexture2D("res://images/packed/unlock_icon.png"));
			}
			else if (State == EpochSlotState.Obtained)
			{
				_hoverTween.TweenMethod(Callable.From<float>((Action<float>)UpdateShaderV), _hsv.GetShaderParameter(_v), Variant.op_Implicit(0.65f), 0.05);
				LocString unlockInfo2 = model.UnlockInfo;
				unlockInfo2.Add("IsRevealed", variable: false);
				_hoverTip = new HoverTip(model.Title, unlockInfo2);
			}
		}
		else
		{
			_hoverTween.TweenMethod(Callable.From<float>((Action<float>)UpdateShaderS), _hsv.GetShaderParameter(_s), Variant.op_Implicit(0.25f), 0.05);
			_hoverTween.TweenMethod(Callable.From<float>((Action<float>)UpdateShaderV), _hsv.GetShaderParameter(_v), Variant.op_Implicit(1.2f), 0.05);
			LocString unlockInfo3 = model.UnlockInfo;
			unlockInfo3.Add("IsRevealed", variable: false);
			_hoverTip = new HoverTip(model.Title, unlockInfo3);
		}
		if (_hoverTip != null)
		{
			NHoverTipSet nHoverTipSet = NHoverTipSet.CreateAndShow((Control)(object)this, _hoverTip);
			float num = ((Control)this).Size.X * 0.5f + 6f;
			Transform2D globalTransform = ((CanvasItem)this).GetGlobalTransform();
			float num2 = num * ((Transform2D)(ref globalTransform)).Scale.X;
			float x = ((Control)this).GlobalPosition.X;
			float num3 = ((Control)this).Size.X * 0.5f + 6f;
			globalTransform = ((CanvasItem)this).GetGlobalTransform();
			float num4 = x + num3 * ((Transform2D)(ref globalTransform)).Scale.X;
			float x2 = ((Control)this).GlobalPosition.X;
			Rect2 viewportRect = ((CanvasItem)NGame.Instance).GetViewportRect();
			if (x2 > ((Rect2)(ref viewportRect)).Size.X * 0.7f)
			{
				((Control)nHoverTipSet).GlobalPosition = new Vector2(num4 - num2 - 360f, ((Control)this).GlobalPosition.Y);
			}
			else
			{
				((Control)nHoverTipSet).GlobalPosition = new Vector2(num4 + num2, ((Control)this).GlobalPosition.Y);
			}
			nHoverTipSet.SetFollowOwner();
		}
	}

	protected override void OnUnfocus()
	{
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		base.OnUnfocus();
		_selectionReticle.OnDeselect();
		Tween? hoverTween = _hoverTween;
		if (hoverTween != null)
		{
			hoverTween.Kill();
		}
		_hoverTween = ((Node)this).CreateTween().SetParallel(true);
		_hoverTween.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("scale"), Variant.op_Implicit(Vector2.One), 0.5).SetEase((EaseType)1).SetTrans((TransitionType)7);
		if (State != EpochSlotState.NotObtained)
		{
			if (State == EpochSlotState.Obtained)
			{
				_isGlowPulsing = true;
				_hoverTween.TweenMethod(Callable.From<float>((Action<float>)UpdateShaderV), _hsv.GetShaderParameter(_v), Variant.op_Implicit(0.5f), 0.5).SetEase((EaseType)1).SetTrans((TransitionType)5);
			}
			else if (State == EpochSlotState.Complete)
			{
				_hoverTween.TweenMethod(Callable.From<float>((Action<float>)UpdateShaderS), _hsv.GetShaderParameter(_s), Variant.op_Implicit(1f), 0.5).SetEase((EaseType)1).SetTrans((TransitionType)5);
				_hoverTween.TweenMethod(Callable.From<float>((Action<float>)UpdateShaderV), _hsv.GetShaderParameter(_v), Variant.op_Implicit(1f), 0.5).SetEase((EaseType)1).SetTrans((TransitionType)5);
			}
		}
		else
		{
			_hoverTween.TweenMethod(Callable.From<float>((Action<float>)UpdateShaderS), _hsv.GetShaderParameter(_s), Variant.op_Implicit(0.25f), 0.5).SetEase((EaseType)1).SetTrans((TransitionType)5);
			_hoverTween.TweenMethod(Callable.From<float>((Action<float>)UpdateShaderV), _hsv.GetShaderParameter(_v), Variant.op_Implicit(1.1f), 0.5).SetEase((EaseType)1).SetTrans((TransitionType)5);
		}
		NHoverTipSet.Remove((Control)(object)this);
	}

	protected override void OnPress()
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		if (State != EpochSlotState.NotObtained)
		{
			base.OnPress();
			Tween? hoverTween = _hoverTween;
			if (hoverTween != null)
			{
				hoverTween.Kill();
			}
			_hoverTween = ((Node)this).CreateTween().SetParallel(true);
			_hoverTween.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("scale"), Variant.op_Implicit(Vector2.One * 0.95f), 0.25).SetEase((EaseType)1).SetTrans((TransitionType)5);
			if (State == EpochSlotState.Complete)
			{
				_hoverTween.TweenMethod(Callable.From<float>((Action<float>)UpdateShaderS), _hsv.GetShaderParameter(_s), Variant.op_Implicit(1f), 0.25).SetEase((EaseType)1).SetTrans((TransitionType)5);
				_hoverTween.TweenMethod(Callable.From<float>((Action<float>)UpdateShaderV), _hsv.GetShaderParameter(_v), Variant.op_Implicit(1f), 0.25).SetEase((EaseType)1).SetTrans((TransitionType)5);
			}
			else if (State == EpochSlotState.Obtained)
			{
				_hoverTween.TweenMethod(Callable.From<float>((Action<float>)UpdateShaderV), _hsv.GetShaderParameter(_v), Variant.op_Implicit(0.5f), 0.25).SetEase((EaseType)1).SetTrans((TransitionType)5);
			}
		}
	}

	public async Task SpawnSlot()
	{
		_spawnTween = ((Node)this).CreateTween().SetParallel(true);
		_spawnTween.Chain();
		_spawnTween.TweenInterval(Rng.Chaotic.NextDouble(0.0, 0.3));
		_spawnTween.Chain();
		_spawnTween.TweenProperty((GodotObject)(object)_slotImage, NodePath.op_Implicit("position"), Variant.op_Implicit(Vector2.Zero), 0.5).SetEase((EaseType)1).SetTrans((TransitionType)10)
			.From(Variant.op_Implicit(new Vector2(0f, 64f)));
		_spawnTween.TweenProperty((GodotObject)(object)_outline, NodePath.op_Implicit("position"), Variant.op_Implicit(Vector2.Zero), 0.5).SetEase((EaseType)1).SetTrans((TransitionType)10)
			.From(Variant.op_Implicit(new Vector2(0f, 64f)));
		_spawnTween.TweenProperty((GodotObject)(object)_slotImage, NodePath.op_Implicit("self_modulate:a"), Variant.op_Implicit(1f), 0.5).SetEase((EaseType)1).SetTrans((TransitionType)7);
		await ((GodotObject)this).ToSignal((GodotObject)(object)_spawnTween, SignalName.Finished);
		HasSpawned = true;
		if (State == EpochSlotState.Obtained)
		{
			EnableHighlight();
		}
		else
		{
			((CanvasItem)_outline).Modulate = _defaultSlotOutlineColor;
		}
	}

	public override void _Process(double delta)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		if (_isGlowPulsing)
		{
			((CanvasItem)_outline).Modulate = new Color(((CanvasItem)_outline).Modulate.R, ((CanvasItem)_outline).Modulate.G, ((CanvasItem)_outline).Modulate.B, (Mathf.Sin((float)Time.GetTicksMsec() * 0.005f) + 2f) * 0.25f);
		}
	}

	private void DisableHighlight()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		_isGlowPulsing = false;
		((CanvasItem)_outline).Modulate = Colors.Transparent;
		((Node)(object)_highlightVfx)?.QueueFreeSafely();
		((Node)(object)_offscreenVfx)?.QueueFreeSafely();
	}

	private void EnableHighlight()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		_isGlowPulsing = true;
		((CanvasItem)_outline).Modulate = _highlightSlotColor;
		((CanvasItem)_outline).SelfModulate = StsColors.transparentWhite;
		Tween? glowTween = _glowTween;
		if (glowTween != null)
		{
			glowTween.Kill();
		}
		_glowTween = ((Node)this).CreateTween();
		_glowTween.TweenProperty((GodotObject)(object)_outline, NodePath.op_Implicit("self_modulate:a"), Variant.op_Implicit(1f), 1.0);
		_offscreenVfx = NEpochOffscreenVfx.Create(this);
		_highlightVfx = (Control?)(object)NEpochHighlightVfx.Create();
		((Node)(object)this).AddChildSafely((Node?)(object)_highlightVfx);
		((Node)(object)NTimelineScreen.Instance.GetReminderVfxHolder()).AddChildSafely((Node?)(object)_offscreenVfx);
		((Node)this).MoveChild((Node)(object)_highlightVfx, 0);
	}

	public void SetState(EpochSlotState setState)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		State = setState;
		if (State == EpochSlotState.None)
		{
			Log.Error("Slot State is invalid.");
			return;
		}
		((CanvasItem)_slotImage).Modulate = Colors.White;
		((CanvasItem)_slotImage).ClipChildren = (ClipChildrenMode)2;
		((CanvasItem)_portrait).Visible = true;
		if (State == EpochSlotState.Complete)
		{
			DisableHighlight();
			_portrait.Texture = model.Portrait;
			UpdateShaderS(1f);
			UpdateShaderV(1f);
		}
		else if (State == EpochSlotState.Obtained)
		{
			_portrait.Texture = model.Portrait;
			UpdateShaderS(0f);
			UpdateShaderV(0.5f);
			((CanvasItem)_chains).Visible = true;
		}
		else if (State == EpochSlotState.NotObtained)
		{
			_blurShader.SetShaderParameter(_lod, Variant.op_Implicit(2f));
			UpdateShaderS(0.25f);
			UpdateShaderV(1f);
			_blurPortrait.Texture = model.Portrait;
			((CanvasItem)_subViewportContainer).Visible = true;
			_portrait.Texture = (Texture2D)(object)((Viewport)_subViewport).GetTexture();
		}
		((Control)this).MouseDefaultCursorShape = (CursorShape)((State == EpochSlotState.Complete) ? 16 : 0);
	}

	private void UpdateShaderS(float value)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		_hsv.SetShaderParameter(_s, Variant.op_Implicit(value));
	}

	private void UpdateShaderV(float value)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		_hsv.SetShaderParameter(_v, Variant.op_Implicit(value));
	}

	private void UpdateBlurLod(float value)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		_blurShader.SetShaderParameter(_lod, Variant.op_Implicit(value));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<MethodInfo> GetGodotMethodList()
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
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_0272: Unknown result type (might be due to invalid IL or missing references)
		//IL_0298: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_030f: Unknown result type (might be due to invalid IL or missing references)
		//IL_031a: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(13);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnRelease, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.RevealEpoch, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnFocus, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnUnfocus, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnPress, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Process, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)3, StringName.op_Implicit("delta"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.DisableHighlight, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.EnableHighlight, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetState, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("setState"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UpdateShaderS, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)3, StringName.op_Implicit("value"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UpdateShaderV, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)3, StringName.op_Implicit("value"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UpdateBlurLod, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)3, StringName.op_Implicit("value"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
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
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnRelease && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnRelease();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.RevealEpoch && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			RevealEpoch();
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
		if ((ref method) == MethodName.OnPress && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnPress();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._Process && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			((Node)this)._Process(VariantUtils.ConvertTo<double>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.DisableHighlight && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			DisableHighlight();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.EnableHighlight && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			EnableHighlight();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetState && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			SetState(VariantUtils.ConvertTo<EpochSlotState>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.UpdateShaderS && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			UpdateShaderS(VariantUtils.ConvertTo<float>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.UpdateShaderV && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			UpdateShaderV(VariantUtils.ConvertTo<float>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.UpdateBlurLod && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			UpdateBlurLod(VariantUtils.ConvertTo<float>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		return base.InvokeGodotClassMethod(in method, args, out ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if ((ref method) == MethodName._Ready)
		{
			return true;
		}
		if ((ref method) == MethodName.OnRelease)
		{
			return true;
		}
		if ((ref method) == MethodName.RevealEpoch)
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
		if ((ref method) == MethodName.OnPress)
		{
			return true;
		}
		if ((ref method) == MethodName._Process)
		{
			return true;
		}
		if ((ref method) == MethodName.DisableHighlight)
		{
			return true;
		}
		if ((ref method) == MethodName.EnableHighlight)
		{
			return true;
		}
		if ((ref method) == MethodName.SetState)
		{
			return true;
		}
		if ((ref method) == MethodName.UpdateShaderS)
		{
			return true;
		}
		if ((ref method) == MethodName.UpdateShaderV)
		{
			return true;
		}
		if ((ref method) == MethodName.UpdateBlurLod)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName.State)
		{
			State = VariantUtils.ConvertTo<EpochSlotState>(ref value);
			return true;
		}
		if ((ref name) == PropertyName.HasSpawned)
		{
			HasSpawned = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._slotImage)
		{
			_slotImage = VariantUtils.ConvertTo<TextureRect>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._portrait)
		{
			_portrait = VariantUtils.ConvertTo<TextureRect>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._chains)
		{
			_chains = VariantUtils.ConvertTo<TextureRect>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._hsv)
		{
			_hsv = VariantUtils.ConvertTo<ShaderMaterial>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._blurPortrait)
		{
			_blurPortrait = VariantUtils.ConvertTo<TextureRect>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._outline)
		{
			_outline = VariantUtils.ConvertTo<TextureRect>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._blur)
		{
			_blur = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._blurShader)
		{
			_blurShader = VariantUtils.ConvertTo<ShaderMaterial>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._selectionReticle)
		{
			_selectionReticle = VariantUtils.ConvertTo<NSelectionReticle>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._offscreenVfx)
		{
			_offscreenVfx = VariantUtils.ConvertTo<NEpochOffscreenVfx>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._highlightVfx)
		{
			_highlightVfx = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._subViewportContainer)
		{
			_subViewportContainer = VariantUtils.ConvertTo<SubViewportContainer>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._subViewport)
		{
			_subViewport = VariantUtils.ConvertTo<SubViewport>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._isGlowPulsing)
		{
			_isGlowPulsing = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._isComplete)
		{
			_isComplete = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._isHovered)
		{
			_isHovered = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._era)
		{
			_era = VariantUtils.ConvertTo<EpochEra>(ref value);
			return true;
		}
		if ((ref name) == PropertyName.eraPosition)
		{
			eraPosition = VariantUtils.ConvertTo<int>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._glowTween)
		{
			_glowTween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._spawnTween)
		{
			_spawnTween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._hoverTween)
		{
			_hoverTween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		return base.SetGodotClassPropertyValue(in name, in value);
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
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_0280: Unknown result type (might be due to invalid IL or missing references)
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0300: Unknown result type (might be due to invalid IL or missing references)
		//IL_0305: Unknown result type (might be due to invalid IL or missing references)
		//IL_0320: Unknown result type (might be due to invalid IL or missing references)
		//IL_0325: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName.ClickedSfx)
		{
			string clickedSfx = ClickedSfx;
			value = VariantUtils.CreateFrom<string>(ref clickedSfx);
			return true;
		}
		if ((ref name) == PropertyName.HoveredSfx)
		{
			string clickedSfx = HoveredSfx;
			value = VariantUtils.CreateFrom<string>(ref clickedSfx);
			return true;
		}
		if ((ref name) == PropertyName.State)
		{
			EpochSlotState state = State;
			value = VariantUtils.CreateFrom<EpochSlotState>(ref state);
			return true;
		}
		if ((ref name) == PropertyName.HasSpawned)
		{
			bool hasSpawned = HasSpawned;
			value = VariantUtils.CreateFrom<bool>(ref hasSpawned);
			return true;
		}
		if ((ref name) == PropertyName._slotImage)
		{
			value = VariantUtils.CreateFrom<TextureRect>(ref _slotImage);
			return true;
		}
		if ((ref name) == PropertyName._portrait)
		{
			value = VariantUtils.CreateFrom<TextureRect>(ref _portrait);
			return true;
		}
		if ((ref name) == PropertyName._chains)
		{
			value = VariantUtils.CreateFrom<TextureRect>(ref _chains);
			return true;
		}
		if ((ref name) == PropertyName._hsv)
		{
			value = VariantUtils.CreateFrom<ShaderMaterial>(ref _hsv);
			return true;
		}
		if ((ref name) == PropertyName._blurPortrait)
		{
			value = VariantUtils.CreateFrom<TextureRect>(ref _blurPortrait);
			return true;
		}
		if ((ref name) == PropertyName._outline)
		{
			value = VariantUtils.CreateFrom<TextureRect>(ref _outline);
			return true;
		}
		if ((ref name) == PropertyName._blur)
		{
			value = VariantUtils.CreateFrom<Control>(ref _blur);
			return true;
		}
		if ((ref name) == PropertyName._blurShader)
		{
			value = VariantUtils.CreateFrom<ShaderMaterial>(ref _blurShader);
			return true;
		}
		if ((ref name) == PropertyName._selectionReticle)
		{
			value = VariantUtils.CreateFrom<NSelectionReticle>(ref _selectionReticle);
			return true;
		}
		if ((ref name) == PropertyName._offscreenVfx)
		{
			value = VariantUtils.CreateFrom<NEpochOffscreenVfx>(ref _offscreenVfx);
			return true;
		}
		if ((ref name) == PropertyName._highlightVfx)
		{
			value = VariantUtils.CreateFrom<Control>(ref _highlightVfx);
			return true;
		}
		if ((ref name) == PropertyName._subViewportContainer)
		{
			value = VariantUtils.CreateFrom<SubViewportContainer>(ref _subViewportContainer);
			return true;
		}
		if ((ref name) == PropertyName._subViewport)
		{
			value = VariantUtils.CreateFrom<SubViewport>(ref _subViewport);
			return true;
		}
		if ((ref name) == PropertyName._isGlowPulsing)
		{
			value = VariantUtils.CreateFrom<bool>(ref _isGlowPulsing);
			return true;
		}
		if ((ref name) == PropertyName._isComplete)
		{
			value = VariantUtils.CreateFrom<bool>(ref _isComplete);
			return true;
		}
		if ((ref name) == PropertyName._isHovered)
		{
			value = VariantUtils.CreateFrom<bool>(ref _isHovered);
			return true;
		}
		if ((ref name) == PropertyName._era)
		{
			value = VariantUtils.CreateFrom<EpochEra>(ref _era);
			return true;
		}
		if ((ref name) == PropertyName.eraPosition)
		{
			value = VariantUtils.CreateFrom<int>(ref eraPosition);
			return true;
		}
		if ((ref name) == PropertyName._glowTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _glowTween);
			return true;
		}
		if ((ref name) == PropertyName._spawnTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _spawnTween);
			return true;
		}
		if ((ref name) == PropertyName._hoverTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _hoverTween);
			return true;
		}
		return base.GetGodotClassPropertyValue(in name, out value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_0269: Unknown result type (might be due to invalid IL or missing references)
		//IL_0289: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_030c: Unknown result type (might be due to invalid IL or missing references)
		//IL_032c: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)4, PropertyName.ClickedSfx, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)4, PropertyName.HoveredSfx, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._slotImage, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._portrait, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._chains, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._hsv, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._blurPortrait, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._outline, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._blur, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._blurShader, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._selectionReticle, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._offscreenVfx, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._highlightVfx, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._subViewportContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._subViewport, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._isGlowPulsing, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._isComplete, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._isHovered, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)2, PropertyName.State, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)2, PropertyName._era, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)2, PropertyName.eraPosition, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._glowTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._spawnTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._hoverTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName.HasSpawned, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		base.SaveGodotObjectData(info);
		StringName state = PropertyName.State;
		EpochSlotState state2 = State;
		info.AddProperty(state, Variant.From<EpochSlotState>(ref state2));
		StringName hasSpawned = PropertyName.HasSpawned;
		bool hasSpawned2 = HasSpawned;
		info.AddProperty(hasSpawned, Variant.From<bool>(ref hasSpawned2));
		info.AddProperty(PropertyName._slotImage, Variant.From<TextureRect>(ref _slotImage));
		info.AddProperty(PropertyName._portrait, Variant.From<TextureRect>(ref _portrait));
		info.AddProperty(PropertyName._chains, Variant.From<TextureRect>(ref _chains));
		info.AddProperty(PropertyName._hsv, Variant.From<ShaderMaterial>(ref _hsv));
		info.AddProperty(PropertyName._blurPortrait, Variant.From<TextureRect>(ref _blurPortrait));
		info.AddProperty(PropertyName._outline, Variant.From<TextureRect>(ref _outline));
		info.AddProperty(PropertyName._blur, Variant.From<Control>(ref _blur));
		info.AddProperty(PropertyName._blurShader, Variant.From<ShaderMaterial>(ref _blurShader));
		info.AddProperty(PropertyName._selectionReticle, Variant.From<NSelectionReticle>(ref _selectionReticle));
		info.AddProperty(PropertyName._offscreenVfx, Variant.From<NEpochOffscreenVfx>(ref _offscreenVfx));
		info.AddProperty(PropertyName._highlightVfx, Variant.From<Control>(ref _highlightVfx));
		info.AddProperty(PropertyName._subViewportContainer, Variant.From<SubViewportContainer>(ref _subViewportContainer));
		info.AddProperty(PropertyName._subViewport, Variant.From<SubViewport>(ref _subViewport));
		info.AddProperty(PropertyName._isGlowPulsing, Variant.From<bool>(ref _isGlowPulsing));
		info.AddProperty(PropertyName._isComplete, Variant.From<bool>(ref _isComplete));
		info.AddProperty(PropertyName._isHovered, Variant.From<bool>(ref _isHovered));
		info.AddProperty(PropertyName._era, Variant.From<EpochEra>(ref _era));
		info.AddProperty(PropertyName.eraPosition, Variant.From<int>(ref eraPosition));
		info.AddProperty(PropertyName._glowTween, Variant.From<Tween>(ref _glowTween));
		info.AddProperty(PropertyName._spawnTween, Variant.From<Tween>(ref _spawnTween));
		info.AddProperty(PropertyName._hoverTween, Variant.From<Tween>(ref _hoverTween));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName.State, ref val))
		{
			State = ((Variant)(ref val)).As<EpochSlotState>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName.HasSpawned, ref val2))
		{
			HasSpawned = ((Variant)(ref val2)).As<bool>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._slotImage, ref val3))
		{
			_slotImage = ((Variant)(ref val3)).As<TextureRect>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._portrait, ref val4))
		{
			_portrait = ((Variant)(ref val4)).As<TextureRect>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._chains, ref val5))
		{
			_chains = ((Variant)(ref val5)).As<TextureRect>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._hsv, ref val6))
		{
			_hsv = ((Variant)(ref val6)).As<ShaderMaterial>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName._blurPortrait, ref val7))
		{
			_blurPortrait = ((Variant)(ref val7)).As<TextureRect>();
		}
		Variant val8 = default(Variant);
		if (info.TryGetProperty(PropertyName._outline, ref val8))
		{
			_outline = ((Variant)(ref val8)).As<TextureRect>();
		}
		Variant val9 = default(Variant);
		if (info.TryGetProperty(PropertyName._blur, ref val9))
		{
			_blur = ((Variant)(ref val9)).As<Control>();
		}
		Variant val10 = default(Variant);
		if (info.TryGetProperty(PropertyName._blurShader, ref val10))
		{
			_blurShader = ((Variant)(ref val10)).As<ShaderMaterial>();
		}
		Variant val11 = default(Variant);
		if (info.TryGetProperty(PropertyName._selectionReticle, ref val11))
		{
			_selectionReticle = ((Variant)(ref val11)).As<NSelectionReticle>();
		}
		Variant val12 = default(Variant);
		if (info.TryGetProperty(PropertyName._offscreenVfx, ref val12))
		{
			_offscreenVfx = ((Variant)(ref val12)).As<NEpochOffscreenVfx>();
		}
		Variant val13 = default(Variant);
		if (info.TryGetProperty(PropertyName._highlightVfx, ref val13))
		{
			_highlightVfx = ((Variant)(ref val13)).As<Control>();
		}
		Variant val14 = default(Variant);
		if (info.TryGetProperty(PropertyName._subViewportContainer, ref val14))
		{
			_subViewportContainer = ((Variant)(ref val14)).As<SubViewportContainer>();
		}
		Variant val15 = default(Variant);
		if (info.TryGetProperty(PropertyName._subViewport, ref val15))
		{
			_subViewport = ((Variant)(ref val15)).As<SubViewport>();
		}
		Variant val16 = default(Variant);
		if (info.TryGetProperty(PropertyName._isGlowPulsing, ref val16))
		{
			_isGlowPulsing = ((Variant)(ref val16)).As<bool>();
		}
		Variant val17 = default(Variant);
		if (info.TryGetProperty(PropertyName._isComplete, ref val17))
		{
			_isComplete = ((Variant)(ref val17)).As<bool>();
		}
		Variant val18 = default(Variant);
		if (info.TryGetProperty(PropertyName._isHovered, ref val18))
		{
			_isHovered = ((Variant)(ref val18)).As<bool>();
		}
		Variant val19 = default(Variant);
		if (info.TryGetProperty(PropertyName._era, ref val19))
		{
			_era = ((Variant)(ref val19)).As<EpochEra>();
		}
		Variant val20 = default(Variant);
		if (info.TryGetProperty(PropertyName.eraPosition, ref val20))
		{
			eraPosition = ((Variant)(ref val20)).As<int>();
		}
		Variant val21 = default(Variant);
		if (info.TryGetProperty(PropertyName._glowTween, ref val21))
		{
			_glowTween = ((Variant)(ref val21)).As<Tween>();
		}
		Variant val22 = default(Variant);
		if (info.TryGetProperty(PropertyName._spawnTween, ref val22))
		{
			_spawnTween = ((Variant)(ref val22)).As<Tween>();
		}
		Variant val23 = default(Variant);
		if (info.TryGetProperty(PropertyName._hoverTween, ref val23))
		{
			_hoverTween = ((Variant)(ref val23)).As<Tween>();
		}
	}
}
