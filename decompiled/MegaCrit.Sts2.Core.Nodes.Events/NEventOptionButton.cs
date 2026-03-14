using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.HoverTips;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Nodes.Vfx.Utilities;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.Settings;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.Nodes.Events;

[ScriptPath("res://src/Core/Nodes/Events/NEventOptionButton.cs")]
public class NEventOptionButton : NButton
{
	public new class MethodName : NButton.MethodName
	{
		public new static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName SetVisuallyLocked = StringName.op_Implicit("SetVisuallyLocked");

		public static readonly StringName AnimateIn = StringName.op_Implicit("AnimateIn");

		public static readonly StringName EnableButton = StringName.op_Implicit("EnableButton");

		public new static readonly StringName OnRelease = StringName.op_Implicit("OnRelease");

		public new static readonly StringName OnPress = StringName.op_Implicit("OnPress");

		public new static readonly StringName OnFocus = StringName.op_Implicit("OnFocus");

		public new static readonly StringName OnUnfocus = StringName.op_Implicit("OnUnfocus");

		public static readonly StringName GrayOut = StringName.op_Implicit("GrayOut");

		public static readonly StringName UpdateShaderParam = StringName.op_Implicit("UpdateShaderParam");

		public static readonly StringName RefreshVotes = StringName.op_Implicit("RefreshVotes");

		public new static readonly StringName _ExitTree = StringName.op_Implicit("_ExitTree");

		public static readonly StringName WillKillPlayer = StringName.op_Implicit("WillKillPlayer");

		public static readonly StringName ShowPersistentKillGlow = StringName.op_Implicit("ShowPersistentKillGlow");

		public static readonly StringName PulseKillGlow = StringName.op_Implicit("PulseKillGlow");
	}

	public new class PropertyName : NButton.PropertyName
	{
		public static readonly StringName Index = StringName.op_Implicit("Index");

		public static readonly StringName VoteContainer = StringName.op_Implicit("VoteContainer");

		public static readonly StringName _label = StringName.op_Implicit("_label");

		public static readonly StringName _image = StringName.op_Implicit("_image");

		public static readonly StringName _outline = StringName.op_Implicit("_outline");

		public static readonly StringName _killGlow = StringName.op_Implicit("_killGlow");

		public static readonly StringName _confirmFlash = StringName.op_Implicit("_confirmFlash");

		public static readonly StringName _hsv = StringName.op_Implicit("_hsv");

		public static readonly StringName _playerVoteContainer = StringName.op_Implicit("_playerVoteContainer");

		public static readonly StringName _animInTween = StringName.op_Implicit("_animInTween");

		public static readonly StringName _flashTween = StringName.op_Implicit("_flashTween");

		public static readonly StringName _killGlowTween = StringName.op_Implicit("_killGlowTween");

		public static readonly StringName _tween = StringName.op_Implicit("_tween");

		public static readonly StringName _buttonColor = StringName.op_Implicit("_buttonColor");

		public static readonly StringName _deathPreventionVfx = StringName.op_Implicit("_deathPreventionVfx");

		public static readonly StringName _deathPreventionVfxPosition = StringName.op_Implicit("_deathPreventionVfxPosition");
	}

	public new class SignalName : NButton.SignalName
	{
	}

	private static readonly StringName _v = new StringName("v");

	private static readonly StringName _s = new StringName("s");

	private static readonly StringName _h = new StringName("h");

	private static readonly string _voteIconPath = SceneHelper.GetScenePath("ui/multiplayer_vote_icon");

	private MegaRichTextLabel _label;

	private NinePatchRect _image;

	private NinePatchRect _outline;

	private NinePatchRect _killGlow;

	private NinePatchRect _confirmFlash;

	private ShaderMaterial? _hsv;

	private NMultiplayerVoteContainer _playerVoteContainer;

	private Tween? _animInTween;

	private Tween? _flashTween;

	private Tween? _killGlowTween;

	private Tween? _tween;

	private static readonly Vector2 _hoverScale = Vector2.One * 1.01f;

	private static readonly Vector2 _pressScale = Vector2.One * 0.99f;

	private const float _defaultV = 0.9f;

	private const float _hoverV = 1.2f;

	private readonly CancellationTokenSource _cancelToken = new CancellationTokenSource();

	private Color _buttonColor;

	private NThoughtBubbleVfx? _deathPreventionVfx;

	private CancellationTokenSource? _deathPreventionCancellation;

	private Vector2 _deathPreventionVfxPosition;

	public EventModel Event { get; private set; }

	public EventOption Option { get; private set; }

	private int Index { get; set; }

	public NMultiplayerVoteContainer VoteContainer => _playerVoteContainer;

	private static string ScenePath => SceneHelper.GetScenePath("events/event_option_button");

	private static string AncientScenePath => SceneHelper.GetScenePath("events/ancient_event_option_button");

	public static IEnumerable<string> AssetPaths => new global::_003C_003Ez__ReadOnlyArray<string>(new string[3] { ScenePath, AncientScenePath, _voteIconPath });

	public static NEventOptionButton Create(EventModel eventModel, EventOption option, int index)
	{
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		NEventOptionButton nEventOptionButton = ((eventModel is AncientEventModel) ? PreloadManager.Cache.GetScene(AncientScenePath).Instantiate<NEventOptionButton>((GenEditState)0) : PreloadManager.Cache.GetScene(ScenePath).Instantiate<NEventOptionButton>((GenEditState)0));
		nEventOptionButton.Event = eventModel;
		nEventOptionButton.Option = option;
		nEventOptionButton.Index = index;
		nEventOptionButton._buttonColor = eventModel.ButtonColor;
		return nEventOptionButton;
	}

	public override void _Ready()
	{
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Expected O, but got Unknown
		ConnectSignals();
		Event.DynamicVars.AddTo(Option.Description);
		Event.DynamicVars.AddTo(Option.Title);
		string formattedText = Option.Title.GetFormattedText();
		string formattedText2 = Option.Description.GetFormattedText();
		_label = ((Node)this).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("%Text"));
		if (string.IsNullOrEmpty(formattedText))
		{
			_label.Text = formattedText2;
		}
		else
		{
			string value = (Option.IsLocked ? "red" : "gold");
			_label.Text = $"[{value}][b]{formattedText}[/b][/{value}]\n{formattedText2}";
		}
		_image = ((Node)this).GetNode<NinePatchRect>(NodePath.op_Implicit("Image"));
		((CanvasItem)_image).Modulate = _buttonColor;
		_hsv = (ShaderMaterial)((CanvasItem)_image).Material;
		_outline = ((Node)this).GetNode<NinePatchRect>(NodePath.op_Implicit("Outline"));
		_killGlow = ((Node)this).GetNode<NinePatchRect>(NodePath.op_Implicit("RedFlash"));
		_confirmFlash = ((Node)this).GetNode<NinePatchRect>(NodePath.op_Implicit("BlueFlash"));
		_playerVoteContainer = ((Node)this).GetNode<NMultiplayerVoteContainer>(NodePath.op_Implicit("PlayerVoteContainer"));
		_playerVoteContainer.Initialize(ShouldDisplayPlayerVote, Event.Owner.RunState.Players);
		if (Event is AncientEventModel && Option.Relic != null)
		{
			TextureRect node = ((Node)this).GetNode<TextureRect>(NodePath.op_Implicit("%RelicIcon"));
			node.SetTexture(Option.Relic.Icon);
			((Node)node).GetNode<TextureRect>(NodePath.op_Implicit("%Outline")).SetTexture(Option.Relic.IconOutline);
			((CanvasItem)node).Visible = true;
		}
		if (Option.IsLocked)
		{
			SetVisuallyLocked();
		}
		else if (WillKillPlayer())
		{
			ShowPersistentKillGlow();
		}
	}

	private void SetVisuallyLocked()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		((CanvasItem)_label).Modulate = new Color(((CanvasItem)_label).Modulate.R, ((CanvasItem)_label).Modulate.G, ((CanvasItem)_label).Modulate.B, 0.7f);
		ShaderMaterial? hsv = _hsv;
		if (hsv != null)
		{
			hsv.SetShaderParameter(_h, Variant.op_Implicit(0.48f));
		}
		ShaderMaterial? hsv2 = _hsv;
		if (hsv2 != null)
		{
			hsv2.SetShaderParameter(_s, Variant.op_Implicit(0.2f));
		}
		ShaderMaterial? hsv3 = _hsv;
		if (hsv3 != null)
		{
			hsv3.SetShaderParameter(_v, Variant.op_Implicit(0.65f));
		}
	}

	public void AnimateIn()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		if (GodotObject.IsInstanceValid((GodotObject)(object)this))
		{
			if (SaveManager.Instance.PrefsSave.FastMode == FastModeType.Instant)
			{
				EnableButton();
				return;
			}
			((CanvasItem)this).Modulate = StsColors.transparentWhite;
			bool flag = SaveManager.Instance.PrefsSave.FastMode == FastModeType.Fast;
			_animInTween = ((Node)this).CreateTween().SetParallel(true);
			_animInTween.TweenInterval(flag ? 0.25 : 0.5);
			_animInTween.Chain();
			_animInTween.TweenInterval((flag ? 0.1 : 0.2) * (double)Index);
			_animInTween.Chain();
			_animInTween.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("position"), Variant.op_Implicit(((Control)this).Position), flag ? 0.25 : 0.5).SetEase((EaseType)1).SetTrans((TransitionType)10)
				.From(Variant.op_Implicit(((Control)this).Position + new Vector2(-60f, 0f)));
			_animInTween.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("modulate"), Variant.op_Implicit(Colors.White), flag ? 0.25 : 0.5);
			_animInTween.Finished += EnableButton;
		}
	}

	public void EnableButton()
	{
		((Control)this).MouseFilter = (MouseFilterEnum)0;
	}

	protected override void OnRelease()
	{
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		if (Option.IsLocked)
		{
			return;
		}
		if (WillKillPlayer())
		{
			if (Event.Owner.RunState.Players.Count > 1)
			{
				if (_deathPreventionVfx == null)
				{
					LocString eventDeathPreventionLine = Event.Owner.Character.EventDeathPreventionLine;
					_deathPreventionVfx = NThoughtBubbleVfx.Create(eventDeathPreventionLine.GetFormattedText(), DialogueSide.Right, null);
					((Node)(object)NEventRoom.Instance?.VfxContainer)?.AddChildSafely((Node?)(object)_deathPreventionVfx);
					_deathPreventionVfxPosition = ((Control)this).GlobalPosition + new Vector2(-50f, -15f);
					((Control)_deathPreventionVfx).GlobalPosition = _deathPreventionVfxPosition;
				}
				else
				{
					_deathPreventionCancellation?.Cancel();
				}
				TaskHelper.RunSafely(RumbleDeathVfx());
				TaskHelper.RunSafely(ExpireDeathPreventionVfx());
				return;
			}
			ShowPersistentKillGlow();
		}
		NEventRoom.Instance.OptionButtonClicked(Option, Index);
	}

	private async Task RumbleDeathVfx()
	{
		ScreenRumbleInstance rumble = new ScreenRumbleInstance(20f, 0.30000001192092896, 10f, RumbleStyle.Rumble);
		while (!rumble.IsDone)
		{
			await ((GodotObject)this).ToSignal((GodotObject)(object)((Node)this).GetTree(), SignalName.ProcessFrame);
			Vector2 val = rumble.Update(((Node)this).GetProcessDeltaTime());
			((Control)_deathPreventionVfx).GlobalPosition = _deathPreventionVfxPosition + val;
		}
	}

	private async Task ExpireDeathPreventionVfx()
	{
		_deathPreventionCancellation = new CancellationTokenSource();
		await Cmd.Wait(2.5f, _deathPreventionCancellation.Token, ignoreCombatEnd: true);
		if (!_deathPreventionCancellation.IsCancellationRequested)
		{
			_deathPreventionVfx?.GoAway();
			_deathPreventionVfx = null;
		}
	}

	protected override void OnPress()
	{
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		if (!Option.IsLocked)
		{
			Tween? tween = _tween;
			if (tween != null)
			{
				tween.Kill();
			}
			_tween = ((Node)this).CreateTween().SetParallel(true);
			_tween.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("scale"), Variant.op_Implicit(_pressScale), 0.5).SetEase((EaseType)1).SetTrans((TransitionType)5);
			_tween.TweenMethod(Callable.From<float>((Action<float>)UpdateShaderParam), Variant.op_Implicit(1.2f), Variant.op_Implicit(0.9f), 0.5).SetEase((EaseType)1).SetTrans((TransitionType)5);
			base.OnPress();
		}
	}

	protected override void OnFocus()
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		if (!Option.IsLocked)
		{
			base.OnFocus();
			Tween? tween = _tween;
			if (tween != null)
			{
				tween.Kill();
			}
			_tween = ((Node)this).CreateTween().SetParallel(true);
			_tween.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("scale"), Variant.op_Implicit(_hoverScale), 0.05);
			_tween.TweenProperty((GodotObject)(object)_outline, NodePath.op_Implicit("modulate"), Variant.op_Implicit(StsColors.blueGlow), 0.05);
			ShaderMaterial? hsv = _hsv;
			if (hsv != null)
			{
				hsv.SetShaderParameter(_v, Variant.op_Implicit(1.2f));
			}
			NHoverTipSet.CreateAndShow((Control)(object)this, Option.HoverTips, (Event.LayoutType != EventLayoutType.Combat) ? HoverTipAlignment.Left : HoverTipAlignment.Right);
			if (WillKillPlayer())
			{
				PulseKillGlow();
			}
		}
	}

	protected override void OnUnfocus()
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		if (!Option.IsLocked)
		{
			base.OnUnfocus();
			Tween? tween = _tween;
			if (tween != null)
			{
				tween.Kill();
			}
			_tween = ((Node)this).CreateTween().SetParallel(true);
			_tween.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("scale"), Variant.op_Implicit(Vector2.One), 0.5).SetEase((EaseType)1).SetTrans((TransitionType)5);
			_tween.TweenMethod(Callable.From<float>((Action<float>)UpdateShaderParam), Variant.op_Implicit(1.2f), Variant.op_Implicit(0.9f), 0.5).SetEase((EaseType)1).SetTrans((TransitionType)5);
			_tween.TweenProperty((GodotObject)(object)_outline, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0f), 0.3);
			NHoverTipSet.Remove((Control)(object)this);
			if (WillKillPlayer())
			{
				ShowPersistentKillGlow();
			}
		}
	}

	public async Task FlashConfirmation()
	{
		Tween? flashTween = _flashTween;
		if (flashTween != null)
		{
			flashTween.Kill();
		}
		NinePatchRect confirmFlash = _confirmFlash;
		Color modulate = ((CanvasItem)_confirmFlash).Modulate;
		modulate.A = 0f;
		((CanvasItem)confirmFlash).Modulate = modulate;
		_flashTween = ((Node)this).CreateTween();
		_flashTween.TweenProperty((GodotObject)(object)_confirmFlash, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0.8f), 0.5).SetEase((EaseType)1).SetTrans((TransitionType)5);
		_flashTween.Parallel().TweenProperty((GodotObject)(object)_confirmFlash, NodePath.op_Implicit("scale"), Variant.op_Implicit(new Vector2(1.03f, 1.1f)), 0.25).SetEase((EaseType)1)
			.SetTrans((TransitionType)5);
		_flashTween.TweenProperty((GodotObject)(object)_confirmFlash, NodePath.op_Implicit("scale"), Variant.op_Implicit(Vector2.One), 0.25).SetEase((EaseType)1).SetTrans((TransitionType)5);
		_flashTween.TweenProperty((GodotObject)(object)_confirmFlash, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0f), 0.3);
		await ((GodotObject)this).ToSignal((GodotObject)(object)_flashTween, SignalName.Finished);
		await Cmd.Wait(0.5f);
	}

	public void GrayOut()
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		Tween? flashTween = _flashTween;
		if (flashTween != null)
		{
			flashTween.Kill();
		}
		_flashTween = ((Node)this).CreateTween();
		Tween? flashTween2 = _flashTween;
		NodePath obj = NodePath.op_Implicit("modulate");
		Color lightGray = StsColors.lightGray;
		lightGray.A = 0.5f;
		flashTween2.TweenProperty((GodotObject)(object)this, obj, Variant.op_Implicit(lightGray), 0.3).SetEase((EaseType)1).SetTrans((TransitionType)5);
	}

	private void UpdateShaderParam(float newV)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		ShaderMaterial? hsv = _hsv;
		if (hsv != null)
		{
			hsv.SetShaderParameter(_v, Variant.op_Implicit(newV));
		}
	}

	private bool ShouldDisplayPlayerVote(Player player)
	{
		return RunManager.Instance.EventSynchronizer.GetPlayerVote(player) == Index;
	}

	public void RefreshVotes()
	{
		_playerVoteContainer.RefreshPlayerVotes();
	}

	public override void _ExitTree()
	{
		_cancelToken.Cancel();
		if (_animInTween != null)
		{
			_animInTween.Finished -= EnableButton;
		}
	}

	private bool WillKillPlayer()
	{
		if (Event.Owner != null)
		{
			return Option.WillKillPlayer?.Invoke(Event.Owner) ?? false;
		}
		return false;
	}

	private void ShowPersistentKillGlow()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		Tween? killGlowTween = _killGlowTween;
		if (killGlowTween != null)
		{
			killGlowTween.Kill();
		}
		NinePatchRect killGlow = _killGlow;
		Color modulate = ((CanvasItem)_killGlow).Modulate;
		modulate.A = 0.5f;
		((CanvasItem)killGlow).Modulate = modulate;
	}

	private void PulseKillGlow()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		Tween? killGlowTween = _killGlowTween;
		if (killGlowTween != null)
		{
			killGlowTween.Kill();
		}
		NinePatchRect killGlow = _killGlow;
		Color modulate = ((CanvasItem)_killGlow).Modulate;
		modulate.A = 0.25f;
		((CanvasItem)killGlow).Modulate = modulate;
		_killGlowTween = ((Node)this).CreateTween();
		_killGlowTween.TweenProperty((GodotObject)(object)_killGlow, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(1f), 0.2).SetEase((EaseType)1).SetTrans((TransitionType)5);
		_killGlowTween.TweenProperty((GodotObject)(object)_killGlow, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0.25f), 0.8);
		_killGlowTween.SetLoops(0);
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
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0286: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02db: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(15);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetVisuallyLocked, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AnimateIn, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.EnableButton, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnRelease, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnPress, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnFocus, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnUnfocus, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.GrayOut, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UpdateShaderParam, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)3, StringName.op_Implicit("newV"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.RefreshVotes, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._ExitTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.WillKillPlayer, new PropertyInfo((Type)1, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ShowPersistentKillGlow, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.PulseKillGlow, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
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
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetVisuallyLocked && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			SetVisuallyLocked();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.AnimateIn && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			AnimateIn();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.EnableButton && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			EnableButton();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnRelease && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnRelease();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnPress && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnPress();
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
		if ((ref method) == MethodName.GrayOut && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			GrayOut();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.UpdateShaderParam && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			UpdateShaderParam(VariantUtils.ConvertTo<float>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.RefreshVotes && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			RefreshVotes();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._ExitTree && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._ExitTree();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.WillKillPlayer && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			bool flag = WillKillPlayer();
			ret = VariantUtils.CreateFrom<bool>(ref flag);
			return true;
		}
		if ((ref method) == MethodName.ShowPersistentKillGlow && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			ShowPersistentKillGlow();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.PulseKillGlow && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			PulseKillGlow();
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
		if ((ref method) == MethodName.SetVisuallyLocked)
		{
			return true;
		}
		if ((ref method) == MethodName.AnimateIn)
		{
			return true;
		}
		if ((ref method) == MethodName.EnableButton)
		{
			return true;
		}
		if ((ref method) == MethodName.OnRelease)
		{
			return true;
		}
		if ((ref method) == MethodName.OnPress)
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
		if ((ref method) == MethodName.GrayOut)
		{
			return true;
		}
		if ((ref method) == MethodName.UpdateShaderParam)
		{
			return true;
		}
		if ((ref method) == MethodName.RefreshVotes)
		{
			return true;
		}
		if ((ref method) == MethodName._ExitTree)
		{
			return true;
		}
		if ((ref method) == MethodName.WillKillPlayer)
		{
			return true;
		}
		if ((ref method) == MethodName.ShowPersistentKillGlow)
		{
			return true;
		}
		if ((ref method) == MethodName.PulseKillGlow)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName.Index)
		{
			Index = VariantUtils.ConvertTo<int>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._label)
		{
			_label = VariantUtils.ConvertTo<MegaRichTextLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._image)
		{
			_image = VariantUtils.ConvertTo<NinePatchRect>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._outline)
		{
			_outline = VariantUtils.ConvertTo<NinePatchRect>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._killGlow)
		{
			_killGlow = VariantUtils.ConvertTo<NinePatchRect>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._confirmFlash)
		{
			_confirmFlash = VariantUtils.ConvertTo<NinePatchRect>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._hsv)
		{
			_hsv = VariantUtils.ConvertTo<ShaderMaterial>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._playerVoteContainer)
		{
			_playerVoteContainer = VariantUtils.ConvertTo<NMultiplayerVoteContainer>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._animInTween)
		{
			_animInTween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._flashTween)
		{
			_flashTween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._killGlowTween)
		{
			_killGlowTween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._tween)
		{
			_tween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._buttonColor)
		{
			_buttonColor = VariantUtils.ConvertTo<Color>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._deathPreventionVfx)
		{
			_deathPreventionVfx = VariantUtils.ConvertTo<NThoughtBubbleVfx>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._deathPreventionVfxPosition)
		{
			_deathPreventionVfxPosition = VariantUtils.ConvertTo<Vector2>(ref value);
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
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName.Index)
		{
			int index = Index;
			value = VariantUtils.CreateFrom<int>(ref index);
			return true;
		}
		if ((ref name) == PropertyName.VoteContainer)
		{
			NMultiplayerVoteContainer voteContainer = VoteContainer;
			value = VariantUtils.CreateFrom<NMultiplayerVoteContainer>(ref voteContainer);
			return true;
		}
		if ((ref name) == PropertyName._label)
		{
			value = VariantUtils.CreateFrom<MegaRichTextLabel>(ref _label);
			return true;
		}
		if ((ref name) == PropertyName._image)
		{
			value = VariantUtils.CreateFrom<NinePatchRect>(ref _image);
			return true;
		}
		if ((ref name) == PropertyName._outline)
		{
			value = VariantUtils.CreateFrom<NinePatchRect>(ref _outline);
			return true;
		}
		if ((ref name) == PropertyName._killGlow)
		{
			value = VariantUtils.CreateFrom<NinePatchRect>(ref _killGlow);
			return true;
		}
		if ((ref name) == PropertyName._confirmFlash)
		{
			value = VariantUtils.CreateFrom<NinePatchRect>(ref _confirmFlash);
			return true;
		}
		if ((ref name) == PropertyName._hsv)
		{
			value = VariantUtils.CreateFrom<ShaderMaterial>(ref _hsv);
			return true;
		}
		if ((ref name) == PropertyName._playerVoteContainer)
		{
			value = VariantUtils.CreateFrom<NMultiplayerVoteContainer>(ref _playerVoteContainer);
			return true;
		}
		if ((ref name) == PropertyName._animInTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _animInTween);
			return true;
		}
		if ((ref name) == PropertyName._flashTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _flashTween);
			return true;
		}
		if ((ref name) == PropertyName._killGlowTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _killGlowTween);
			return true;
		}
		if ((ref name) == PropertyName._tween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _tween);
			return true;
		}
		if ((ref name) == PropertyName._buttonColor)
		{
			value = VariantUtils.CreateFrom<Color>(ref _buttonColor);
			return true;
		}
		if ((ref name) == PropertyName._deathPreventionVfx)
		{
			value = VariantUtils.CreateFrom<NThoughtBubbleVfx>(ref _deathPreventionVfx);
			return true;
		}
		if ((ref name) == PropertyName._deathPreventionVfxPosition)
		{
			value = VariantUtils.CreateFrom<Vector2>(ref _deathPreventionVfxPosition);
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
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._label, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._image, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._outline, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._killGlow, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._confirmFlash, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._hsv, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._playerVoteContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._animInTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._flashTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._killGlowTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._tween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)2, PropertyName.Index, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.VoteContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)20, PropertyName._buttonColor, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._deathPreventionVfx, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)5, PropertyName._deathPreventionVfxPosition, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		base.SaveGodotObjectData(info);
		StringName index = PropertyName.Index;
		int index2 = Index;
		info.AddProperty(index, Variant.From<int>(ref index2));
		info.AddProperty(PropertyName._label, Variant.From<MegaRichTextLabel>(ref _label));
		info.AddProperty(PropertyName._image, Variant.From<NinePatchRect>(ref _image));
		info.AddProperty(PropertyName._outline, Variant.From<NinePatchRect>(ref _outline));
		info.AddProperty(PropertyName._killGlow, Variant.From<NinePatchRect>(ref _killGlow));
		info.AddProperty(PropertyName._confirmFlash, Variant.From<NinePatchRect>(ref _confirmFlash));
		info.AddProperty(PropertyName._hsv, Variant.From<ShaderMaterial>(ref _hsv));
		info.AddProperty(PropertyName._playerVoteContainer, Variant.From<NMultiplayerVoteContainer>(ref _playerVoteContainer));
		info.AddProperty(PropertyName._animInTween, Variant.From<Tween>(ref _animInTween));
		info.AddProperty(PropertyName._flashTween, Variant.From<Tween>(ref _flashTween));
		info.AddProperty(PropertyName._killGlowTween, Variant.From<Tween>(ref _killGlowTween));
		info.AddProperty(PropertyName._tween, Variant.From<Tween>(ref _tween));
		info.AddProperty(PropertyName._buttonColor, Variant.From<Color>(ref _buttonColor));
		info.AddProperty(PropertyName._deathPreventionVfx, Variant.From<NThoughtBubbleVfx>(ref _deathPreventionVfx));
		info.AddProperty(PropertyName._deathPreventionVfxPosition, Variant.From<Vector2>(ref _deathPreventionVfxPosition));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		base.RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName.Index, ref val))
		{
			Index = ((Variant)(ref val)).As<int>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._label, ref val2))
		{
			_label = ((Variant)(ref val2)).As<MegaRichTextLabel>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._image, ref val3))
		{
			_image = ((Variant)(ref val3)).As<NinePatchRect>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._outline, ref val4))
		{
			_outline = ((Variant)(ref val4)).As<NinePatchRect>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._killGlow, ref val5))
		{
			_killGlow = ((Variant)(ref val5)).As<NinePatchRect>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._confirmFlash, ref val6))
		{
			_confirmFlash = ((Variant)(ref val6)).As<NinePatchRect>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName._hsv, ref val7))
		{
			_hsv = ((Variant)(ref val7)).As<ShaderMaterial>();
		}
		Variant val8 = default(Variant);
		if (info.TryGetProperty(PropertyName._playerVoteContainer, ref val8))
		{
			_playerVoteContainer = ((Variant)(ref val8)).As<NMultiplayerVoteContainer>();
		}
		Variant val9 = default(Variant);
		if (info.TryGetProperty(PropertyName._animInTween, ref val9))
		{
			_animInTween = ((Variant)(ref val9)).As<Tween>();
		}
		Variant val10 = default(Variant);
		if (info.TryGetProperty(PropertyName._flashTween, ref val10))
		{
			_flashTween = ((Variant)(ref val10)).As<Tween>();
		}
		Variant val11 = default(Variant);
		if (info.TryGetProperty(PropertyName._killGlowTween, ref val11))
		{
			_killGlowTween = ((Variant)(ref val11)).As<Tween>();
		}
		Variant val12 = default(Variant);
		if (info.TryGetProperty(PropertyName._tween, ref val12))
		{
			_tween = ((Variant)(ref val12)).As<Tween>();
		}
		Variant val13 = default(Variant);
		if (info.TryGetProperty(PropertyName._buttonColor, ref val13))
		{
			_buttonColor = ((Variant)(ref val13)).As<Color>();
		}
		Variant val14 = default(Variant);
		if (info.TryGetProperty(PropertyName._deathPreventionVfx, ref val14))
		{
			_deathPreventionVfx = ((Variant)(ref val14)).As<NThoughtBubbleVfx>();
		}
		Variant val15 = default(Variant);
		if (info.TryGetProperty(PropertyName._deathPreventionVfxPosition, ref val15))
		{
			_deathPreventionVfxPosition = ((Variant)(ref val15)).As<Vector2>();
		}
	}
}
