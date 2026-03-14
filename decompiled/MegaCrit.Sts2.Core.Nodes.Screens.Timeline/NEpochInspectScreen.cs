using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Achievements;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.ControllerInput;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Screens.ScreenContext;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.Timeline;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.Nodes.Screens.Timeline;

[ScriptPath("res://src/Core/Nodes/Screens/Timeline/NEpochInspectScreen.cs")]
public class NEpochInspectScreen : NClickableControl, IScreenContext
{
	public new class MethodName : NClickableControl.MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName HidePaginators = StringName.op_Implicit("HidePaginators");

		public static readonly StringName Close = StringName.op_Implicit("Close");

		public static readonly StringName UpdateShaderS = StringName.op_Implicit("UpdateShaderS");

		public static readonly StringName UpdateShaderV = StringName.op_Implicit("UpdateShaderV");

		public static readonly StringName _Input = StringName.op_Implicit("_Input");

		public static readonly StringName OnMouseReleased = StringName.op_Implicit("OnMouseReleased");

		public static readonly StringName SpeedUpTextAnimation = StringName.op_Implicit("SpeedUpTextAnimation");

		public static readonly StringName NextChapter = StringName.op_Implicit("NextChapter");

		public static readonly StringName PrevChapter = StringName.op_Implicit("PrevChapter");

		public static readonly StringName RefreshChapterPaginators = StringName.op_Implicit("RefreshChapterPaginators");
	}

	public new class PropertyName : NClickableControl.PropertyName
	{
		public static readonly StringName DefaultFocusedControl = StringName.op_Implicit("DefaultFocusedControl");

		public static readonly StringName _closeButton = StringName.op_Implicit("_closeButton");

		public static readonly StringName _portrait = StringName.op_Implicit("_portrait");

		public static readonly StringName _portraitFlash = StringName.op_Implicit("_portraitFlash");

		public static readonly StringName _mask = StringName.op_Implicit("_mask");

		public static readonly StringName _chains = StringName.op_Implicit("_chains");

		public static readonly StringName _portraitHsv = StringName.op_Implicit("_portraitHsv");

		public static readonly StringName _fancyText = StringName.op_Implicit("_fancyText");

		public static readonly StringName _storyLabel = StringName.op_Implicit("_storyLabel");

		public static readonly StringName _chapterLabel = StringName.op_Implicit("_chapterLabel");

		public static readonly StringName _closeLabel = StringName.op_Implicit("_closeLabel");

		public static readonly StringName _placeholderLabel = StringName.op_Implicit("_placeholderLabel");

		public static readonly StringName _nextChapterButton = StringName.op_Implicit("_nextChapterButton");

		public static readonly StringName _prevChapterButton = StringName.op_Implicit("_prevChapterButton");

		public static readonly StringName _unlockInfo = StringName.op_Implicit("_unlockInfo");

		public static readonly StringName _hasStory = StringName.op_Implicit("_hasStory");

		public static readonly StringName _wasRevealed = StringName.op_Implicit("_wasRevealed");

		public static readonly StringName _prevChapterButtonOffsetX = StringName.op_Implicit("_prevChapterButtonOffsetX");

		public static readonly StringName _nextChapterButtonOffsetX = StringName.op_Implicit("_nextChapterButtonOffsetX");

		public static readonly StringName _maskOffsetX = StringName.op_Implicit("_maskOffsetX");

		public static readonly StringName _maskOffsetY = StringName.op_Implicit("_maskOffsetY");

		public static readonly StringName _closeButtonY = StringName.op_Implicit("_closeButtonY");

		public static readonly StringName _unlockTween = StringName.op_Implicit("_unlockTween");

		public static readonly StringName _buttonTween = StringName.op_Implicit("_buttonTween");

		public static readonly StringName _tween = StringName.op_Implicit("_tween");

		public static readonly StringName _textTween = StringName.op_Implicit("_textTween");
	}

	public new class SignalName : NClickableControl.SignalName
	{
	}

	private static readonly LocString placeholderLoc = new LocString("timeline", "PLACEHOLDER_PORTRAIT");

	public static readonly string lockedImagePath = ImageHelper.GetImagePath("packed/timeline/epoch_slot_locked.png");

	private NButton _closeButton;

	private TextureRect _portrait;

	private TextureRect _portraitFlash;

	private TextureRect _mask;

	private NEpochChains _chains;

	private ShaderMaterial _portraitHsv;

	private MegaRichTextLabel _fancyText;

	private MegaLabel _storyLabel;

	private MegaLabel _chapterLabel;

	private MegaLabel _closeLabel;

	private MegaLabel _placeholderLabel;

	private NEpochPaginateButton _nextChapterButton;

	private NEpochPaginateButton _prevChapterButton;

	private NUnlockInfo _unlockInfo;

	private List<SerializableEpoch> _allEpochs;

	private EpochModel _epoch;

	private EpochModel? _prevChapterEpoch;

	private EpochModel? _nextChapterEpoch;

	private LocString _chapterLoc;

	private bool _hasStory;

	private bool _wasRevealed;

	private float _prevChapterButtonOffsetX;

	private float _nextChapterButtonOffsetX;

	private float _maskOffsetX;

	private float _maskOffsetY;

	private float _closeButtonY;

	private Tween? _unlockTween;

	private Tween? _buttonTween;

	private Tween? _tween;

	private Tween? _textTween;

	private static readonly StringName _v = new StringName("v");

	private static readonly StringName _s = new StringName("s");

	public Control? DefaultFocusedControl => null;

	public override void _Ready()
	{
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Expected O, but got Unknown
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		_storyLabel = ((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%StoryLabel"));
		_chapterLabel = ((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%ChapterLabel"));
		_placeholderLabel = ((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%PlaceholderLabel"));
		_portrait = ((Node)this).GetNode<TextureRect>(NodePath.op_Implicit("%Portrait"));
		_portraitFlash = ((Node)this).GetNode<TextureRect>(NodePath.op_Implicit("%PortraitFlash"));
		_mask = ((Node)this).GetNode<TextureRect>(NodePath.op_Implicit("%Mask"));
		_maskOffsetY = ((Control)_mask).OffsetTop;
		_maskOffsetX = ((Control)_mask).OffsetLeft;
		_chains = ((Node)this).GetNode<NEpochChains>(NodePath.op_Implicit("%Chains"));
		_portraitHsv = (ShaderMaterial)((CanvasItem)_portrait).Material;
		_closeButton = ((Node)this).GetNode<NButton>(NodePath.op_Implicit("%CloseButton"));
		_fancyText = ((Node)this).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("%FancyText"));
		_closeLabel = ((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%CloseLabel"));
		_chapterLoc = new LocString("timeline", "EPOCH_INSPECT.chapterFormat");
		_unlockInfo = ((Node)this).GetNode<NUnlockInfo>(NodePath.op_Implicit("%UnlockInfo"));
		_nextChapterButton = ((Node)this).GetNode<NEpochPaginateButton>(NodePath.op_Implicit("%NextChapterButton"));
		_prevChapterButton = ((Node)this).GetNode<NEpochPaginateButton>(NodePath.op_Implicit("%PrevChapterButton"));
		_prevChapterButtonOffsetX = ((Control)_prevChapterButton).OffsetLeft;
		_nextChapterButtonOffsetX = ((Control)_nextChapterButton).OffsetLeft;
		((GodotObject)_nextChapterButton).Connect(NClickableControl.SignalName.MouseReleased, Callable.From<InputEvent>((Action<InputEvent>)delegate
		{
			NextChapter();
		}), 0u);
		((GodotObject)_prevChapterButton).Connect(NClickableControl.SignalName.MouseReleased, Callable.From<InputEvent>((Action<InputEvent>)delegate
		{
			PrevChapter();
		}), 0u);
		((GodotObject)this).Connect(NClickableControl.SignalName.MouseReleased, Callable.From<InputEvent>((Action<InputEvent>)OnMouseReleased), 0u);
		_closeButton.Disable();
	}

	public async Task Open(NEpochSlot slot, EpochModel epoch, bool wasRevealed)
	{
		_buttonTween?.FastForwardToCompletion();
		_wasRevealed = wasRevealed;
		_epoch = epoch;
		if (_epoch.IsArtPlaceholder)
		{
			((CanvasItem)_placeholderLabel).Visible = true;
			((Label)_placeholderLabel).Text = placeholderLoc.GetRawText();
		}
		else
		{
			((CanvasItem)_placeholderLabel).Visible = false;
		}
		((CanvasItem)this).Modulate = Colors.White;
		_portrait.Texture = epoch.BigPortrait;
		((CanvasItem)_fancyText).Modulate = StsColors.transparentWhite;
		_fancyText.Text = epoch.Description;
		_hasStory = epoch.StoryTitle != null;
		SfxCmd.Play("event:/sfx/ui/timeline/ui_timeline_open_epoch");
		if (_hasStory)
		{
			_storyLabel.SetTextAutoSize(epoch.StoryTitle ?? string.Empty);
			_chapterLoc.Add("ChapterIndex", epoch.ChapterIndex);
			_chapterLoc.Add("ChapterName", epoch.Title);
			_chapterLabel.SetTextAutoSize(_chapterLoc.GetFormattedText());
			((Label)_chapterLabel).VerticalAlignment = (VerticalAlignment)1;
		}
		else
		{
			_storyLabel.SetTextAutoSize(string.Empty);
			_chapterLabel.SetTextAutoSize(epoch.Title.GetFormattedText());
			((Label)_chapterLabel).VerticalAlignment = (VerticalAlignment)2;
			_nextChapterButton.Disable();
			_prevChapterButton.Disable();
		}
		((Control)_closeButton).MouseFilter = (MouseFilterEnum)0;
		((Control)_closeButton).Scale = Vector2.One;
		_closeButtonY = ((Control)_closeButton).Position.Y + 180f;
		((Control)_closeButton).Position = new Vector2(((Control)_closeButton).Position.X, _closeButtonY);
		((CanvasItem)_closeButton).Modulate = StsColors.transparentWhite;
		_closeButton.Disable();
		NTimelineScreen.Instance.ShowBackstopAndHideUi();
		((CanvasItem)this).Visible = true;
		Vector2 size = ((Control)_mask).Size;
		((Control)_mask).GlobalPosition = ((Control)slot).GlobalPosition;
		TextureRect mask = _mask;
		StringName size2 = PropertyName.Size;
		Vector2 size3 = ((Control)slot).Size;
		Transform2D globalTransform = ((CanvasItem)slot).GetGlobalTransform();
		((GodotObject)mask).SetDeferred(size2, Variant.op_Implicit(size3 * ((Transform2D)(ref globalTransform)).Scale));
		Tween? tween = _tween;
		if (tween != null)
		{
			tween.Kill();
		}
		_tween = ((Node)this).CreateTween().SetParallel(true);
		_tween.TweenProperty((GodotObject)(object)_mask, NodePath.op_Implicit("offset_left"), Variant.op_Implicit(_maskOffsetX), 0.4).SetTrans((TransitionType)7).SetEase((EaseType)1);
		_tween.TweenProperty((GodotObject)(object)_mask, NodePath.op_Implicit("offset_top"), Variant.op_Implicit(_maskOffsetY), 0.4).SetEase((EaseType)1).SetTrans((TransitionType)5);
		_tween.TweenProperty((GodotObject)(object)_mask, NodePath.op_Implicit("size"), Variant.op_Implicit(size), 0.4).SetTrans((TransitionType)7).SetEase((EaseType)1);
		((CanvasItem)_storyLabel).Modulate = StsColors.transparentWhite;
		((CanvasItem)_chapterLabel).Modulate = StsColors.transparentWhite;
		((CanvasItem)_nextChapterButton).Modulate = StsColors.transparentWhite;
		((CanvasItem)_prevChapterButton).Modulate = StsColors.transparentWhite;
		_tween.TweenProperty((GodotObject)(object)_storyLabel, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(1f), 0.5).SetDelay(0.4);
		_tween.TweenProperty((GodotObject)(object)_chapterLabel, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(1f), 0.5).SetDelay(0.2);
		_tween.TweenProperty((GodotObject)(object)_prevChapterButton, NodePath.op_Implicit("offset_left"), Variant.op_Implicit(_prevChapterButtonOffsetX), 0.25).SetEase((EaseType)1).SetTrans((TransitionType)10)
			.From(Variant.op_Implicit(_prevChapterButtonOffsetX + 100f))
			.SetDelay(0.25);
		_tween.TweenProperty((GodotObject)(object)_prevChapterButton, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(1f), 0.25).SetDelay(0.25);
		_tween.TweenProperty((GodotObject)(object)_nextChapterButton, NodePath.op_Implicit("offset_left"), Variant.op_Implicit(_nextChapterButtonOffsetX), 0.25).SetEase((EaseType)1).SetTrans((TransitionType)10)
			.From(Variant.op_Implicit(_nextChapterButtonOffsetX - 100f))
			.SetDelay(0.25);
		_tween.TweenProperty((GodotObject)(object)_nextChapterButton, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(1f), 0.25).SetDelay(0.25);
		if (wasRevealed)
		{
			await TaskHelper.RunSafely(UnlockAnimation(epoch));
			return;
		}
		_closeLabel.SetTextAutoSize(new LocString("timeline", "EPOCH_INSPECT.closeButton").GetRawText());
		_fancyText.Text = epoch.Description;
		Tween? textTween = _textTween;
		if (textTween != null)
		{
			textTween.Kill();
		}
		_textTween = ((Node)this).CreateTween().SetParallel(true);
		_textTween.TweenProperty((GodotObject)(object)_fancyText, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(1f), 0.5).SetDelay(0.1);
		((RichTextLabel)_fancyText).VisibleRatio = 1f;
		Tween? buttonTween = _buttonTween;
		if (buttonTween != null)
		{
			buttonTween.Kill();
		}
		_buttonTween = ((Node)this).CreateTween().SetParallel(true);
		_buttonTween.TweenProperty((GodotObject)(object)_closeButton, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(1f), 0.3).SetEase((EaseType)1).SetTrans((TransitionType)7)
			.SetDelay(0.1);
		_buttonTween.TweenProperty((GodotObject)(object)_closeButton, NodePath.op_Implicit("position:y"), Variant.op_Implicit(_closeButtonY - 180f), 0.3).SetEase((EaseType)1).SetTrans((TransitionType)10)
			.SetDelay(0.1);
		_buttonTween.TweenCallback(Callable.From((Action)_closeButton.Enable));
		RefreshChapterPaginators();
		_unlockInfo.AnimIn(epoch.UnlockText);
	}

	private void HidePaginators()
	{
		_hasStory = false;
		((CanvasItem)_nextChapterButton).Visible = false;
		((CanvasItem)_prevChapterButton).Visible = false;
	}

	private void OpenViaPaginator(EpochModel epoch)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		_epoch = epoch;
		((CanvasItem)this).Modulate = Colors.White;
		_fancyText.Text = epoch.Description;
		_portrait.Texture = epoch.BigPortrait;
		_hasStory = epoch.StoryTitle != null;
		((CanvasItem)_storyLabel).Modulate = Colors.White;
		((CanvasItem)_chapterLabel).Modulate = Colors.White;
		if (_hasStory)
		{
			_storyLabel.SetTextAutoSize(epoch.StoryTitle ?? string.Empty);
			_chapterLoc.Add("ChapterIndex", epoch.ChapterIndex);
			_chapterLoc.Add("ChapterName", epoch.Title);
			_chapterLabel.SetTextAutoSize(_chapterLoc.GetFormattedText());
			((Label)_chapterLabel).VerticalAlignment = (VerticalAlignment)1;
			((CanvasItem)_nextChapterButton).Modulate = Colors.White;
			((CanvasItem)_prevChapterButton).Modulate = Colors.White;
		}
		else
		{
			_storyLabel.SetTextAutoSize(string.Empty);
			_chapterLabel.SetTextAutoSize(epoch.Title.GetFormattedText());
			((Label)_chapterLabel).VerticalAlignment = (VerticalAlignment)2;
			((CanvasItem)_nextChapterButton).Visible = false;
			((CanvasItem)_prevChapterButton).Visible = false;
		}
		((CanvasItem)_fancyText).Modulate = StsColors.transparentWhite;
		NTimelineScreen.Instance.ShowBackstopAndHideUi();
		((CanvasItem)this).Visible = true;
		Tween? tween = _tween;
		if (tween != null)
		{
			tween.Kill();
		}
		_tween = ((Node)this).CreateTween().SetParallel(true);
		Tween? textTween = _textTween;
		if (textTween != null)
		{
			textTween.Kill();
		}
		_textTween = ((Node)this).CreateTween().SetParallel(true);
		_tween.TweenProperty((GodotObject)(object)_mask, NodePath.op_Implicit("offset_top"), Variant.op_Implicit(_maskOffsetY), 0.25).SetEase((EaseType)1).SetTrans((TransitionType)5);
		_textTween.TweenProperty((GodotObject)(object)_fancyText, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(1f), 0.5).SetDelay(0.1);
		((RichTextLabel)_fancyText).VisibleRatio = 1f;
		TaskHelper.RunSafely(_unlockInfo.AnimInViaPaginator(epoch.UnlockText));
	}

	public void Close()
	{
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		if (NTimelineScreen.Instance.IsScreenQueued())
		{
			NTimelineScreen.Instance.OpenQueuedScreen();
		}
		else
		{
			NTimelineScreen.Instance.EnableInput();
			NTimelineScreen.Instance.HideBackstopAndShowUi(showBackButton: true);
		}
		((Control)this).FocusMode = (FocusModeEnum)0;
		_buttonTween?.FastForwardToCompletion();
		_unlockTween?.FastForwardToCompletion();
		_tween?.FastForwardToCompletion();
		_textTween?.FastForwardToCompletion();
		_buttonTween = ((Node)this).CreateTween().SetParallel(true);
		_buttonTween.TweenProperty((GodotObject)(object)_closeButton, NodePath.op_Implicit("scale"), Variant.op_Implicit(new Vector2(3f, 0.1f)), 0.3).SetEase((EaseType)1).SetTrans((TransitionType)5);
		_buttonTween.TweenProperty((GodotObject)(object)_closeButton, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0f), 0.25).SetEase((EaseType)1).SetTrans((TransitionType)7);
		_buttonTween.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("modulate"), Variant.op_Implicit(new Color(0f, 0f, 0f, 0f)), 0.5);
		_buttonTween.TweenCallback(Callable.From((Action)delegate
		{
			((CanvasItem)this).Visible = false;
			_closeButton.Disable();
			if (_wasRevealed)
			{
				AchievementsHelper.CheckTimelineComplete();
			}
		}));
		NHotkeyManager.Instance.RemoveHotkeyPressedBinding(StringName.op_Implicit(MegaInput.right), NextChapter);
		NHotkeyManager.Instance.RemoveHotkeyPressedBinding(StringName.op_Implicit(MegaInput.left), PrevChapter);
	}

	public async Task UnlockAnimation(EpochModel epoch)
	{
		HidePaginators();
		epoch.QueueUnlocks();
		SaveManager.Instance.SaveProgressFile();
		_unlockInfo.HideImmediately();
		_closeLabel.SetTextAutoSize(new LocString("timeline", "EPOCH_INSPECT.continueButton").GetRawText());
		((RichTextLabel)_fancyText).VisibleRatio = 0f;
		((CanvasItem)_fancyText).Modulate = StsColors.transparentWhite;
		_portraitHsv.SetShaderParameter(_s, Variant.op_Implicit(0f));
		_portraitHsv.SetShaderParameter(_v, Variant.op_Implicit(0.75f));
		((TextureRect)_chains).Texture = PreloadManager.Cache.GetTexture2D(lockedImagePath);
		((CanvasItem)_chains).Visible = true;
		((CanvasItem)_chains).Modulate = Colors.White;
		((CanvasItem)_chains).SelfModulate = Colors.White;
		((CanvasItem)_portraitFlash).Modulate = new Color(1f, 1f, 1f, 0f);
		Tween? unlockTween = _unlockTween;
		if (unlockTween != null)
		{
			unlockTween.Kill();
		}
		_unlockTween = ((Node)this).CreateTween().SetParallel(true);
		_unlockTween.TweenProperty((GodotObject)(object)_chains, NodePath.op_Implicit("scale"), Variant.op_Implicit(Vector2.One * 0.98f), 0.5).SetEase((EaseType)1).SetTrans((TransitionType)5)
			.SetDelay(0.5);
		await ((GodotObject)this).ToSignal((GodotObject)(object)_unlockTween, SignalName.Finished);
		_chains.Unlock();
		await ((GodotObject)this).ToSignal((GodotObject)(object)_chains, NEpochChains.SignalName.OnAnimationFinished);
		((CanvasItem)_portraitFlash).Modulate = Colors.White;
		_unlockTween = ((Node)this).CreateTween().SetParallel(true);
		_unlockTween.TweenProperty((GodotObject)(object)_portraitFlash, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0f), 0.5);
		_unlockTween.TweenMethod(Callable.From<float>((Action<float>)UpdateShaderS), _portraitHsv.GetShaderParameter(_s), Variant.op_Implicit(1f), 1.0).SetEase((EaseType)1).SetTrans((TransitionType)5);
		_unlockTween.TweenMethod(Callable.From<float>((Action<float>)UpdateShaderV), _portraitHsv.GetShaderParameter(_v), Variant.op_Implicit(1f), 1.0).SetEase((EaseType)1).SetTrans((TransitionType)5);
		Tween? textTween = _textTween;
		if (textTween != null)
		{
			textTween.Kill();
		}
		_textTween = ((Node)this).CreateTween().SetParallel(true);
		_textTween.TweenProperty((GodotObject)(object)_fancyText, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(1f), 2.0).SetDelay(0.25);
		_textTween.TweenProperty((GodotObject)(object)_fancyText, NodePath.op_Implicit("visible_ratio"), Variant.op_Implicit(1f), (double)((RichTextLabel)_fancyText).GetTotalCharacterCount() * 0.015).SetDelay(0.5);
		Tween? buttonTween = _buttonTween;
		if (buttonTween != null)
		{
			buttonTween.Kill();
		}
		_buttonTween = ((Node)this).CreateTween().SetParallel(true);
		_buttonTween.TweenProperty((GodotObject)(object)_closeButton, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(1f), 0.3).SetEase((EaseType)1).SetTrans((TransitionType)7)
			.SetDelay(1.0);
		_buttonTween.TweenProperty((GodotObject)(object)_closeButton, NodePath.op_Implicit("position:y"), Variant.op_Implicit(_closeButtonY - 180f), 0.3).SetEase((EaseType)1).SetTrans((TransitionType)10)
			.SetDelay(1.0);
		_buttonTween.TweenCallback(Callable.From((Action)_closeButton.Enable));
		await ((GodotObject)this).ToSignal((GodotObject)(object)_unlockTween, SignalName.Finished);
	}

	private void UpdateShaderS(float value)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		_portraitHsv.SetShaderParameter(_s, Variant.op_Implicit(value));
	}

	private void UpdateShaderV(float value)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		_portraitHsv.SetShaderParameter(_v, Variant.op_Implicit(value));
	}

	public override void _Input(InputEvent inputEvent)
	{
		if (inputEvent.IsActionPressed(MegaInput.select, false, false) || inputEvent.IsActionPressed(MegaInput.accept, false, false))
		{
			SpeedUpTextAnimation();
		}
	}

	private void OnMouseReleased(InputEvent obj)
	{
		SpeedUpTextAnimation();
	}

	private void SpeedUpTextAnimation()
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		if (_textTween != null && _textTween.IsRunning())
		{
			_textTween.Kill();
			((CanvasItem)_fancyText).Modulate = Colors.White;
			((RichTextLabel)_fancyText).VisibleRatio = 1f;
		}
	}

	private void NextChapter()
	{
		OpenViaPaginator(_nextChapterEpoch);
		RefreshChapterPaginators();
	}

	private void PrevChapter()
	{
		OpenViaPaginator(_prevChapterEpoch);
		RefreshChapterPaginators();
	}

	private void RefreshChapterPaginators()
	{
		if (_hasStory)
		{
			_nextChapterEpoch = StoryModel.NextChapter(_epoch);
			_prevChapterEpoch = StoryModel.PrevChapter(_epoch);
			if (_nextChapterEpoch != null)
			{
				NHotkeyManager.Instance.PushHotkeyPressedBinding(StringName.op_Implicit(MegaInput.right), NextChapter);
				((CanvasItem)_nextChapterButton).Visible = true;
			}
			else
			{
				((CanvasItem)_nextChapterButton).Visible = false;
			}
			if (_prevChapterEpoch != null)
			{
				NHotkeyManager.Instance.PushHotkeyPressedBinding(StringName.op_Implicit(MegaInput.left), PrevChapter);
				((CanvasItem)_prevChapterButton).Visible = true;
			}
			else
			{
				((CanvasItem)_prevChapterButton).Visible = false;
			}
		}
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
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Expected O, but got Unknown
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Expected O, but got Unknown
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(11);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.HidePaginators, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.Close, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UpdateShaderS, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)3, StringName.op_Implicit("value"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UpdateShaderV, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)3, StringName.op_Implicit("value"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Input, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("inputEvent"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("InputEvent"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnMouseReleased, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("obj"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("InputEvent"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SpeedUpTextAnimation, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.NextChapter, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.PrevChapter, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.RefreshChapterPaginators, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.HidePaginators && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			HidePaginators();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.Close && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			Close();
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
		if ((ref method) == MethodName._Input && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			((Node)this)._Input(VariantUtils.ConvertTo<InputEvent>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnMouseReleased && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OnMouseReleased(VariantUtils.ConvertTo<InputEvent>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SpeedUpTextAnimation && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			SpeedUpTextAnimation();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.NextChapter && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			NextChapter();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.PrevChapter && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			PrevChapter();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.RefreshChapterPaginators && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			RefreshChapterPaginators();
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
		if ((ref method) == MethodName.HidePaginators)
		{
			return true;
		}
		if ((ref method) == MethodName.Close)
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
		if ((ref method) == MethodName._Input)
		{
			return true;
		}
		if ((ref method) == MethodName.OnMouseReleased)
		{
			return true;
		}
		if ((ref method) == MethodName.SpeedUpTextAnimation)
		{
			return true;
		}
		if ((ref method) == MethodName.NextChapter)
		{
			return true;
		}
		if ((ref method) == MethodName.PrevChapter)
		{
			return true;
		}
		if ((ref method) == MethodName.RefreshChapterPaginators)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._closeButton)
		{
			_closeButton = VariantUtils.ConvertTo<NButton>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._portrait)
		{
			_portrait = VariantUtils.ConvertTo<TextureRect>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._portraitFlash)
		{
			_portraitFlash = VariantUtils.ConvertTo<TextureRect>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._mask)
		{
			_mask = VariantUtils.ConvertTo<TextureRect>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._chains)
		{
			_chains = VariantUtils.ConvertTo<NEpochChains>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._portraitHsv)
		{
			_portraitHsv = VariantUtils.ConvertTo<ShaderMaterial>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._fancyText)
		{
			_fancyText = VariantUtils.ConvertTo<MegaRichTextLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._storyLabel)
		{
			_storyLabel = VariantUtils.ConvertTo<MegaLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._chapterLabel)
		{
			_chapterLabel = VariantUtils.ConvertTo<MegaLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._closeLabel)
		{
			_closeLabel = VariantUtils.ConvertTo<MegaLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._placeholderLabel)
		{
			_placeholderLabel = VariantUtils.ConvertTo<MegaLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._nextChapterButton)
		{
			_nextChapterButton = VariantUtils.ConvertTo<NEpochPaginateButton>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._prevChapterButton)
		{
			_prevChapterButton = VariantUtils.ConvertTo<NEpochPaginateButton>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._unlockInfo)
		{
			_unlockInfo = VariantUtils.ConvertTo<NUnlockInfo>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._hasStory)
		{
			_hasStory = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._wasRevealed)
		{
			_wasRevealed = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._prevChapterButtonOffsetX)
		{
			_prevChapterButtonOffsetX = VariantUtils.ConvertTo<float>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._nextChapterButtonOffsetX)
		{
			_nextChapterButtonOffsetX = VariantUtils.ConvertTo<float>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._maskOffsetX)
		{
			_maskOffsetX = VariantUtils.ConvertTo<float>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._maskOffsetY)
		{
			_maskOffsetY = VariantUtils.ConvertTo<float>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._closeButtonY)
		{
			_closeButtonY = VariantUtils.ConvertTo<float>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._unlockTween)
		{
			_unlockTween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._buttonTween)
		{
			_buttonTween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._tween)
		{
			_tween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._textTween)
		{
			_textTween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		return base.SetGodotClassPropertyValue(in name, in value);
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
		if ((ref name) == PropertyName.DefaultFocusedControl)
		{
			Control defaultFocusedControl = DefaultFocusedControl;
			value = VariantUtils.CreateFrom<Control>(ref defaultFocusedControl);
			return true;
		}
		if ((ref name) == PropertyName._closeButton)
		{
			value = VariantUtils.CreateFrom<NButton>(ref _closeButton);
			return true;
		}
		if ((ref name) == PropertyName._portrait)
		{
			value = VariantUtils.CreateFrom<TextureRect>(ref _portrait);
			return true;
		}
		if ((ref name) == PropertyName._portraitFlash)
		{
			value = VariantUtils.CreateFrom<TextureRect>(ref _portraitFlash);
			return true;
		}
		if ((ref name) == PropertyName._mask)
		{
			value = VariantUtils.CreateFrom<TextureRect>(ref _mask);
			return true;
		}
		if ((ref name) == PropertyName._chains)
		{
			value = VariantUtils.CreateFrom<NEpochChains>(ref _chains);
			return true;
		}
		if ((ref name) == PropertyName._portraitHsv)
		{
			value = VariantUtils.CreateFrom<ShaderMaterial>(ref _portraitHsv);
			return true;
		}
		if ((ref name) == PropertyName._fancyText)
		{
			value = VariantUtils.CreateFrom<MegaRichTextLabel>(ref _fancyText);
			return true;
		}
		if ((ref name) == PropertyName._storyLabel)
		{
			value = VariantUtils.CreateFrom<MegaLabel>(ref _storyLabel);
			return true;
		}
		if ((ref name) == PropertyName._chapterLabel)
		{
			value = VariantUtils.CreateFrom<MegaLabel>(ref _chapterLabel);
			return true;
		}
		if ((ref name) == PropertyName._closeLabel)
		{
			value = VariantUtils.CreateFrom<MegaLabel>(ref _closeLabel);
			return true;
		}
		if ((ref name) == PropertyName._placeholderLabel)
		{
			value = VariantUtils.CreateFrom<MegaLabel>(ref _placeholderLabel);
			return true;
		}
		if ((ref name) == PropertyName._nextChapterButton)
		{
			value = VariantUtils.CreateFrom<NEpochPaginateButton>(ref _nextChapterButton);
			return true;
		}
		if ((ref name) == PropertyName._prevChapterButton)
		{
			value = VariantUtils.CreateFrom<NEpochPaginateButton>(ref _prevChapterButton);
			return true;
		}
		if ((ref name) == PropertyName._unlockInfo)
		{
			value = VariantUtils.CreateFrom<NUnlockInfo>(ref _unlockInfo);
			return true;
		}
		if ((ref name) == PropertyName._hasStory)
		{
			value = VariantUtils.CreateFrom<bool>(ref _hasStory);
			return true;
		}
		if ((ref name) == PropertyName._wasRevealed)
		{
			value = VariantUtils.CreateFrom<bool>(ref _wasRevealed);
			return true;
		}
		if ((ref name) == PropertyName._prevChapterButtonOffsetX)
		{
			value = VariantUtils.CreateFrom<float>(ref _prevChapterButtonOffsetX);
			return true;
		}
		if ((ref name) == PropertyName._nextChapterButtonOffsetX)
		{
			value = VariantUtils.CreateFrom<float>(ref _nextChapterButtonOffsetX);
			return true;
		}
		if ((ref name) == PropertyName._maskOffsetX)
		{
			value = VariantUtils.CreateFrom<float>(ref _maskOffsetX);
			return true;
		}
		if ((ref name) == PropertyName._maskOffsetY)
		{
			value = VariantUtils.CreateFrom<float>(ref _maskOffsetY);
			return true;
		}
		if ((ref name) == PropertyName._closeButtonY)
		{
			value = VariantUtils.CreateFrom<float>(ref _closeButtonY);
			return true;
		}
		if ((ref name) == PropertyName._unlockTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _unlockTween);
			return true;
		}
		if ((ref name) == PropertyName._buttonTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _buttonTween);
			return true;
		}
		if ((ref name) == PropertyName._tween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _tween);
			return true;
		}
		if ((ref name) == PropertyName._textTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _textTween);
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
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_028a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_030d: Unknown result type (might be due to invalid IL or missing references)
		//IL_032e: Unknown result type (might be due to invalid IL or missing references)
		//IL_034f: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._closeButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._portrait, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._portraitFlash, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._mask, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._chains, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._portraitHsv, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._fancyText, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._storyLabel, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._chapterLabel, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._closeLabel, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._placeholderLabel, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._nextChapterButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._prevChapterButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._unlockInfo, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._hasStory, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._wasRevealed, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)3, PropertyName._prevChapterButtonOffsetX, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)3, PropertyName._nextChapterButtonOffsetX, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)3, PropertyName._maskOffsetX, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)3, PropertyName._maskOffsetY, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)3, PropertyName._closeButtonY, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._unlockTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._buttonTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._tween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._textTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.DefaultFocusedControl, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName._closeButton, Variant.From<NButton>(ref _closeButton));
		info.AddProperty(PropertyName._portrait, Variant.From<TextureRect>(ref _portrait));
		info.AddProperty(PropertyName._portraitFlash, Variant.From<TextureRect>(ref _portraitFlash));
		info.AddProperty(PropertyName._mask, Variant.From<TextureRect>(ref _mask));
		info.AddProperty(PropertyName._chains, Variant.From<NEpochChains>(ref _chains));
		info.AddProperty(PropertyName._portraitHsv, Variant.From<ShaderMaterial>(ref _portraitHsv));
		info.AddProperty(PropertyName._fancyText, Variant.From<MegaRichTextLabel>(ref _fancyText));
		info.AddProperty(PropertyName._storyLabel, Variant.From<MegaLabel>(ref _storyLabel));
		info.AddProperty(PropertyName._chapterLabel, Variant.From<MegaLabel>(ref _chapterLabel));
		info.AddProperty(PropertyName._closeLabel, Variant.From<MegaLabel>(ref _closeLabel));
		info.AddProperty(PropertyName._placeholderLabel, Variant.From<MegaLabel>(ref _placeholderLabel));
		info.AddProperty(PropertyName._nextChapterButton, Variant.From<NEpochPaginateButton>(ref _nextChapterButton));
		info.AddProperty(PropertyName._prevChapterButton, Variant.From<NEpochPaginateButton>(ref _prevChapterButton));
		info.AddProperty(PropertyName._unlockInfo, Variant.From<NUnlockInfo>(ref _unlockInfo));
		info.AddProperty(PropertyName._hasStory, Variant.From<bool>(ref _hasStory));
		info.AddProperty(PropertyName._wasRevealed, Variant.From<bool>(ref _wasRevealed));
		info.AddProperty(PropertyName._prevChapterButtonOffsetX, Variant.From<float>(ref _prevChapterButtonOffsetX));
		info.AddProperty(PropertyName._nextChapterButtonOffsetX, Variant.From<float>(ref _nextChapterButtonOffsetX));
		info.AddProperty(PropertyName._maskOffsetX, Variant.From<float>(ref _maskOffsetX));
		info.AddProperty(PropertyName._maskOffsetY, Variant.From<float>(ref _maskOffsetY));
		info.AddProperty(PropertyName._closeButtonY, Variant.From<float>(ref _closeButtonY));
		info.AddProperty(PropertyName._unlockTween, Variant.From<Tween>(ref _unlockTween));
		info.AddProperty(PropertyName._buttonTween, Variant.From<Tween>(ref _buttonTween));
		info.AddProperty(PropertyName._tween, Variant.From<Tween>(ref _tween));
		info.AddProperty(PropertyName._textTween, Variant.From<Tween>(ref _textTween));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._closeButton, ref val))
		{
			_closeButton = ((Variant)(ref val)).As<NButton>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._portrait, ref val2))
		{
			_portrait = ((Variant)(ref val2)).As<TextureRect>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._portraitFlash, ref val3))
		{
			_portraitFlash = ((Variant)(ref val3)).As<TextureRect>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._mask, ref val4))
		{
			_mask = ((Variant)(ref val4)).As<TextureRect>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._chains, ref val5))
		{
			_chains = ((Variant)(ref val5)).As<NEpochChains>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._portraitHsv, ref val6))
		{
			_portraitHsv = ((Variant)(ref val6)).As<ShaderMaterial>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName._fancyText, ref val7))
		{
			_fancyText = ((Variant)(ref val7)).As<MegaRichTextLabel>();
		}
		Variant val8 = default(Variant);
		if (info.TryGetProperty(PropertyName._storyLabel, ref val8))
		{
			_storyLabel = ((Variant)(ref val8)).As<MegaLabel>();
		}
		Variant val9 = default(Variant);
		if (info.TryGetProperty(PropertyName._chapterLabel, ref val9))
		{
			_chapterLabel = ((Variant)(ref val9)).As<MegaLabel>();
		}
		Variant val10 = default(Variant);
		if (info.TryGetProperty(PropertyName._closeLabel, ref val10))
		{
			_closeLabel = ((Variant)(ref val10)).As<MegaLabel>();
		}
		Variant val11 = default(Variant);
		if (info.TryGetProperty(PropertyName._placeholderLabel, ref val11))
		{
			_placeholderLabel = ((Variant)(ref val11)).As<MegaLabel>();
		}
		Variant val12 = default(Variant);
		if (info.TryGetProperty(PropertyName._nextChapterButton, ref val12))
		{
			_nextChapterButton = ((Variant)(ref val12)).As<NEpochPaginateButton>();
		}
		Variant val13 = default(Variant);
		if (info.TryGetProperty(PropertyName._prevChapterButton, ref val13))
		{
			_prevChapterButton = ((Variant)(ref val13)).As<NEpochPaginateButton>();
		}
		Variant val14 = default(Variant);
		if (info.TryGetProperty(PropertyName._unlockInfo, ref val14))
		{
			_unlockInfo = ((Variant)(ref val14)).As<NUnlockInfo>();
		}
		Variant val15 = default(Variant);
		if (info.TryGetProperty(PropertyName._hasStory, ref val15))
		{
			_hasStory = ((Variant)(ref val15)).As<bool>();
		}
		Variant val16 = default(Variant);
		if (info.TryGetProperty(PropertyName._wasRevealed, ref val16))
		{
			_wasRevealed = ((Variant)(ref val16)).As<bool>();
		}
		Variant val17 = default(Variant);
		if (info.TryGetProperty(PropertyName._prevChapterButtonOffsetX, ref val17))
		{
			_prevChapterButtonOffsetX = ((Variant)(ref val17)).As<float>();
		}
		Variant val18 = default(Variant);
		if (info.TryGetProperty(PropertyName._nextChapterButtonOffsetX, ref val18))
		{
			_nextChapterButtonOffsetX = ((Variant)(ref val18)).As<float>();
		}
		Variant val19 = default(Variant);
		if (info.TryGetProperty(PropertyName._maskOffsetX, ref val19))
		{
			_maskOffsetX = ((Variant)(ref val19)).As<float>();
		}
		Variant val20 = default(Variant);
		if (info.TryGetProperty(PropertyName._maskOffsetY, ref val20))
		{
			_maskOffsetY = ((Variant)(ref val20)).As<float>();
		}
		Variant val21 = default(Variant);
		if (info.TryGetProperty(PropertyName._closeButtonY, ref val21))
		{
			_closeButtonY = ((Variant)(ref val21)).As<float>();
		}
		Variant val22 = default(Variant);
		if (info.TryGetProperty(PropertyName._unlockTween, ref val22))
		{
			_unlockTween = ((Variant)(ref val22)).As<Tween>();
		}
		Variant val23 = default(Variant);
		if (info.TryGetProperty(PropertyName._buttonTween, ref val23))
		{
			_buttonTween = ((Variant)(ref val23)).As<Tween>();
		}
		Variant val24 = default(Variant);
		if (info.TryGetProperty(PropertyName._tween, ref val24))
		{
			_tween = ((Variant)(ref val24)).As<Tween>();
		}
		Variant val25 = default(Variant);
		if (info.TryGetProperty(PropertyName._textTween, ref val25))
		{
			_textTween = ((Variant)(ref val25)).As<Tween>();
		}
	}
}
