using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.ControllerInput;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.Debug;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.HoverTips;
using MegaCrit.Sts2.Core.Nodes.Screens.ScreenContext;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.TestSupport;
using MegaCrit.Sts2.Core.Unlocks;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.Nodes.Screens.InspectScreens;

[ScriptPath("res://src/Core/Nodes/Screens/InspectScreens/NInspectRelicScreen.cs")]
public class NInspectRelicScreen : Control, IScreenContext
{
	public class MethodName : MethodName
	{
		public static readonly StringName Create = StringName.op_Implicit("Create");

		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName _Input = StringName.op_Implicit("_Input");

		public static readonly StringName OnRightButtonPressed = StringName.op_Implicit("OnRightButtonPressed");

		public static readonly StringName OnLeftButtonPressed = StringName.op_Implicit("OnLeftButtonPressed");

		public static readonly StringName SetRelic = StringName.op_Implicit("SetRelic");

		public static readonly StringName UpdateRelicDisplay = StringName.op_Implicit("UpdateRelicDisplay");

		public static readonly StringName SetRarityVisuals = StringName.op_Implicit("SetRarityVisuals");

		public static readonly StringName Close = StringName.op_Implicit("Close");

		public static readonly StringName OnBackstopPressed = StringName.op_Implicit("OnBackstopPressed");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName DefaultFocusedControl = StringName.op_Implicit("DefaultFocusedControl");

		public static readonly StringName _popup = StringName.op_Implicit("_popup");

		public static readonly StringName _backstop = StringName.op_Implicit("_backstop");

		public static readonly StringName _nameLabel = StringName.op_Implicit("_nameLabel");

		public static readonly StringName _rarityLabel = StringName.op_Implicit("_rarityLabel");

		public static readonly StringName _description = StringName.op_Implicit("_description");

		public static readonly StringName _flavor = StringName.op_Implicit("_flavor");

		public static readonly StringName _relicImage = StringName.op_Implicit("_relicImage");

		public static readonly StringName _frameHsv = StringName.op_Implicit("_frameHsv");

		public static readonly StringName _leftButton = StringName.op_Implicit("_leftButton");

		public static readonly StringName _rightButton = StringName.op_Implicit("_rightButton");

		public static readonly StringName _hoverTipRect = StringName.op_Implicit("_hoverTipRect");

		public static readonly StringName _screenTween = StringName.op_Implicit("_screenTween");

		public static readonly StringName _popupTween = StringName.op_Implicit("_popupTween");

		public static readonly StringName _popupPosition = StringName.op_Implicit("_popupPosition");

		public static readonly StringName _leftButtonX = StringName.op_Implicit("_leftButtonX");

		public static readonly StringName _rightButtonX = StringName.op_Implicit("_rightButtonX");

		public static readonly StringName _index = StringName.op_Implicit("_index");
	}

	public class SignalName : SignalName
	{
	}

	private static readonly StringName _v = new StringName("v");

	private static readonly StringName _s = new StringName("s");

	private static readonly StringName _h = new StringName("h");

	private static readonly string _scenePath = SceneHelper.GetScenePath("screens/inspect_relic_screen/inspect_relic_screen");

	private Control _popup;

	private Control _backstop;

	private MegaLabel _nameLabel;

	private MegaLabel _rarityLabel;

	private MegaRichTextLabel _description;

	private MegaRichTextLabel _flavor;

	private TextureRect _relicImage;

	private ShaderMaterial _frameHsv;

	private NGoldArrowButton _leftButton;

	private NGoldArrowButton _rightButton;

	private Control _hoverTipRect;

	private Tween? _screenTween;

	private Tween? _popupTween;

	private Vector2 _popupPosition;

	private float _leftButtonX;

	private float _rightButtonX;

	private const double _arrowButtonDelay = 0.1;

	private IReadOnlyList<RelicModel> _relics;

	private int _index;

	private HashSet<RelicModel> _allUnlockedRelics = new HashSet<RelicModel>();

	public static string[] AssetPaths => new string[1] { _scenePath };

	public Control? DefaultFocusedControl => null;

	public static NInspectRelicScreen? Create()
	{
		if (TestMode.IsOn)
		{
			return null;
		}
		return PreloadManager.Cache.GetScene(_scenePath).Instantiate<NInspectRelicScreen>((GenEditState)0);
	}

	public void Open(IReadOnlyList<RelicModel> relics, RelicModel relic)
	{
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_031e: Unknown result type (might be due to invalid IL or missing references)
		//IL_032d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0332: Unknown result type (might be due to invalid IL or missing references)
		//IL_0337: Unknown result type (might be due to invalid IL or missing references)
		Log.Info($"Inspecting Relic: {relic.Title}");
		UnlockState unlockState = SaveManager.Instance.GenerateUnlockStateFromProgress();
		_allUnlockedRelics.Clear();
		_allUnlockedRelics.UnionWith(unlockState.Relics);
		_relics = relics.ToList();
		_index = relics.IndexOf(relic);
		SetRelic(_index);
		((CanvasItem)this).Visible = true;
		((CanvasItem)_popup).Modulate = StsColors.transparentBlack;
		((CanvasItem)_leftButton).Modulate = StsColors.transparentBlack;
		((CanvasItem)_rightButton).Modulate = StsColors.transparentBlack;
		_leftButton.Enable();
		_rightButton.Enable();
		((CanvasItem)_backstop).Visible = true;
		_backstop.MouseFilter = (MouseFilterEnum)0;
		((Control)_leftButton).MouseFilter = (MouseFilterEnum)0;
		((Control)_rightButton).MouseFilter = (MouseFilterEnum)0;
		Tween? screenTween = _screenTween;
		if (screenTween != null)
		{
			screenTween.Kill();
		}
		_screenTween = ((Node)this).CreateTween().SetParallel(true);
		_screenTween.TweenProperty((GodotObject)(object)_backstop, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0.9f), 0.25);
		_screenTween.TweenProperty((GodotObject)(object)_leftButton, NodePath.op_Implicit("position:x"), Variant.op_Implicit(_leftButtonX), 0.25).SetEase((EaseType)1).SetTrans((TransitionType)10)
			.From(Variant.op_Implicit(_leftButtonX + 100f))
			.SetDelay(0.1);
		_screenTween.TweenProperty((GodotObject)(object)_leftButton, NodePath.op_Implicit("modulate"), Variant.op_Implicit(Colors.White), 0.25).SetDelay(0.1);
		_screenTween.TweenProperty((GodotObject)(object)_rightButton, NodePath.op_Implicit("position:x"), Variant.op_Implicit(_rightButtonX), 0.25).SetEase((EaseType)1).SetTrans((TransitionType)10)
			.From(Variant.op_Implicit(_rightButtonX - 100f))
			.SetDelay(0.1);
		_screenTween.TweenProperty((GodotObject)(object)_rightButton, NodePath.op_Implicit("modulate"), Variant.op_Implicit(Colors.White), 0.25).SetDelay(0.1);
		Tween? popupTween = _popupTween;
		if (popupTween != null)
		{
			popupTween.Kill();
		}
		_popupTween = ((Node)this).CreateTween().SetParallel(true);
		_popupTween.TweenProperty((GodotObject)(object)_popup, NodePath.op_Implicit("modulate"), Variant.op_Implicit(Colors.White), 0.25);
		_popupTween.TweenProperty((GodotObject)(object)_popup, NodePath.op_Implicit("position"), Variant.op_Implicit(_popupPosition), 0.25).SetEase((EaseType)1).SetTrans((TransitionType)7)
			.From(Variant.op_Implicit(_popupPosition + new Vector2(0f, 200f)));
		ActiveScreenContext.Instance.Update();
		NHotkeyManager.Instance.AddBlockingScreen((Node)(object)this);
		NHotkeyManager.Instance.PushHotkeyPressedBinding(StringName.op_Implicit(MegaInput.cancel), Close);
		NHotkeyManager.Instance.PushHotkeyPressedBinding(StringName.op_Implicit(MegaInput.pauseAndBack), Close);
	}

	public override void _Ready()
	{
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Expected O, but got Unknown
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		_popup = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%Popup"));
		_backstop = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%Backstop"));
		_nameLabel = ((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%RelicName"));
		_rarityLabel = ((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%Rarity"));
		_description = ((Node)this).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("%RelicDescription"));
		_flavor = ((Node)this).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("%FlavorText"));
		_relicImage = ((Node)this).GetNode<TextureRect>(NodePath.op_Implicit("%RelicImage"));
		_frameHsv = (ShaderMaterial)((CanvasItem)((Node)this).GetNode<Control>(NodePath.op_Implicit("%Frame"))).Material;
		_leftButton = ((Node)this).GetNode<NGoldArrowButton>(NodePath.op_Implicit("%LeftArrow"));
		_rightButton = ((Node)this).GetNode<NGoldArrowButton>(NodePath.op_Implicit("%RightArrow"));
		_popupPosition = _popup.Position;
		_hoverTipRect = ((Node)this).GetNode<Control>(NodePath.op_Implicit("HoverTipRect"));
		_backstop = (Control)(object)((Node)this).GetNode<NButton>(NodePath.op_Implicit("Backstop"));
		((GodotObject)_backstop).Connect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)OnBackstopPressed), 0u);
		_leftButton = ((Node)this).GetNode<NGoldArrowButton>(NodePath.op_Implicit("LeftArrow"));
		((GodotObject)_leftButton).Connect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)OnLeftButtonPressed), 0u);
		_rightButton = ((Node)this).GetNode<NGoldArrowButton>(NodePath.op_Implicit("RightArrow"));
		((GodotObject)_rightButton).Connect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)OnRightButtonPressed), 0u);
		_leftButtonX = ((Control)_leftButton).Position.X;
		_rightButtonX = ((Control)_rightButton).Position.X;
	}

	public override void _Input(InputEvent inputEvent)
	{
		if (!((CanvasItem)this).IsVisibleInTree() || ((CanvasItem)NDevConsole.Instance).Visible)
		{
			return;
		}
		Control val = ((Node)this).GetViewport().GuiGetFocusOwner();
		if ((!(val is TextEdit) && !(val is LineEdit)) || 1 == 0)
		{
			if (inputEvent.IsActionPressed(MegaInput.left, false, false))
			{
				OnLeftButtonPressed(_leftButton);
			}
			if (inputEvent.IsActionPressed(MegaInput.right, false, false))
			{
				OnRightButtonPressed(_rightButton);
			}
		}
	}

	private void OnRightButtonPressed(NButton button)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		SetRelic(_index + 1);
		((CanvasItem)_popup).Modulate = Colors.White;
		Tween? popupTween = _popupTween;
		if (popupTween != null)
		{
			popupTween.Kill();
		}
		_popupTween = ((Node)this).CreateTween().SetParallel(true);
		_popupTween.TweenProperty((GodotObject)(object)_popup, NodePath.op_Implicit("position"), Variant.op_Implicit(_popupPosition), 0.25).SetEase((EaseType)1).SetTrans((TransitionType)5)
			.From(Variant.op_Implicit(_popupPosition + new Vector2(100f, 0f)));
	}

	private void OnLeftButtonPressed(NButton button)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		SetRelic(_index - 1);
		((CanvasItem)_popup).Modulate = Colors.White;
		Tween? popupTween = _popupTween;
		if (popupTween != null)
		{
			popupTween.Kill();
		}
		_popupTween = ((Node)this).CreateTween().SetParallel(true);
		_popupTween.TweenProperty((GodotObject)(object)_popup, NodePath.op_Implicit("position"), Variant.op_Implicit(_popupPosition), 0.25).SetEase((EaseType)1).SetTrans((TransitionType)5)
			.From(Variant.op_Implicit(_popupPosition + new Vector2(-100f, 0f)));
	}

	private void SetRelic(int index)
	{
		_index = Math.Clamp(index, 0, _relics.Count - 1);
		((CanvasItem)_leftButton).Visible = _index > 0;
		((Control)_leftButton).MouseFilter = (MouseFilterEnum)((_index > 0) ? 0 : 2);
		((CanvasItem)_rightButton).Visible = _index < _relics.Count - 1;
		((Control)_rightButton).MouseFilter = (MouseFilterEnum)((_index < _relics.Count - 1) ? 0 : 2);
		UpdateRelicDisplay();
	}

	private void UpdateRelicDisplay()
	{
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		RelicModel relicModel = _relics[_index];
		if (!_allUnlockedRelics.Contains(relicModel.CanonicalInstance))
		{
			_nameLabel.SetTextAutoSize(new LocString("inspect_relic_screen", "LOCKED_TITLE").GetFormattedText());
			_rarityLabel.SetTextAutoSize(string.Empty);
			((CanvasItem)_relicImage).SelfModulate = Colors.White;
			_description.SetTextAutoSize(new LocString("inspect_relic_screen", "LOCKED_DESCRIPTION").GetFormattedText());
			_flavor.Text = string.Empty;
			SetRarityVisuals(RelicRarity.Common);
			_relicImage.Texture = PreloadManager.Cache.GetTexture2D(ImageHelper.GetImagePath("packed/common_ui/locked_model.png"));
		}
		else if (!SaveManager.Instance.IsRelicSeen(relicModel))
		{
			_nameLabel.SetTextAutoSize(new LocString("inspect_relic_screen", "UNDISCOVERED_TITLE").GetFormattedText());
			_rarityLabel.SetTextAutoSize(string.Empty);
			((CanvasItem)_relicImage).SelfModulate = StsColors.ninetyPercentBlack;
			_description.SetTextAutoSize(new LocString("inspect_relic_screen", "UNDISCOVERED_DESCRIPTION").GetFormattedText());
			_flavor.Text = string.Empty;
			SetRarityVisuals(relicModel.Rarity);
			_relicImage.Texture = relicModel.BigIcon;
		}
		else
		{
			_nameLabel.SetTextAutoSize(relicModel.Title.GetFormattedText());
			LocString locString = new LocString("gameplay_ui", "RELIC_RARITY." + relicModel.Rarity.ToString().ToUpperInvariant());
			_rarityLabel.SetTextAutoSize(locString.GetFormattedText());
			((CanvasItem)_relicImage).SelfModulate = Colors.White;
			_description.SetTextAutoSize(relicModel.DynamicDescription.GetFormattedText());
			_flavor.SetTextAutoSize(relicModel.Flavor.GetFormattedText());
			SetRarityVisuals(relicModel.Rarity);
			_relicImage.Texture = relicModel.BigIcon;
		}
		NHoverTipSet.Clear();
		if (SaveManager.Instance.IsRelicSeen(relicModel))
		{
			NHoverTipSet nHoverTipSet = NHoverTipSet.CreateAndShow((Control)(object)this, relicModel.HoverTipsExcludingRelic);
			nHoverTipSet.SetAlignment(_hoverTipRect, HoverTip.GetHoverTipAlignment((Control)(object)this));
		}
	}

	private void SetRarityVisuals(RelicRarity rarity)
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = default(Vector3);
		switch (rarity)
		{
		case RelicRarity.None:
		case RelicRarity.Starter:
		case RelicRarity.Common:
			((CanvasItem)_rarityLabel).Modulate = StsColors.cream;
			((Vector3)(ref val))._002Ector(0.95f, 0.25f, 0.9f);
			break;
		case RelicRarity.Uncommon:
			((CanvasItem)_rarityLabel).Modulate = StsColors.blue;
			((Vector3)(ref val))._002Ector(0.426f, 0.8f, 1.1f);
			break;
		case RelicRarity.Rare:
			((CanvasItem)_rarityLabel).Modulate = StsColors.gold;
			((Vector3)(ref val))._002Ector(1f, 0.8f, 1.15f);
			break;
		case RelicRarity.Shop:
			((CanvasItem)_rarityLabel).Modulate = StsColors.blue;
			((Vector3)(ref val))._002Ector(0.525f, 2.5f, 0.85f);
			break;
		case RelicRarity.Event:
			((CanvasItem)_rarityLabel).Modulate = StsColors.green;
			((Vector3)(ref val))._002Ector(0.23f, 0.75f, 0.9f);
			break;
		case RelicRarity.Ancient:
			((CanvasItem)_rarityLabel).Modulate = StsColors.red;
			((Vector3)(ref val))._002Ector(0.875f, 3f, 0.9f);
			break;
		default:
			Log.Error("Unspecified relic rarity: " + rarity);
			throw new ArgumentOutOfRangeException();
		}
		_frameHsv.SetShaderParameter(_h, Variant.op_Implicit(val.X));
		_frameHsv.SetShaderParameter(_s, Variant.op_Implicit(val.Y));
		_frameHsv.SetShaderParameter(_v, Variant.op_Implicit(val.Z));
	}

	public void Close()
	{
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		if (((CanvasItem)this).Visible)
		{
			NHotkeyManager.Instance.RemoveBlockingScreen((Node)(object)this);
			NHotkeyManager.Instance.RemoveHotkeyPressedBinding(StringName.op_Implicit(MegaInput.cancel), Close);
			NHotkeyManager.Instance.RemoveHotkeyPressedBinding(StringName.op_Implicit(MegaInput.pauseAndBack), Close);
			_backstop.MouseFilter = (MouseFilterEnum)2;
			((Control)_leftButton).MouseFilter = (MouseFilterEnum)2;
			((Control)_rightButton).MouseFilter = (MouseFilterEnum)2;
			_leftButton.Disable();
			_rightButton.Disable();
			Tween? screenTween = _screenTween;
			if (screenTween != null)
			{
				screenTween.Kill();
			}
			_screenTween = ((Node)this).CreateTween().SetParallel(true);
			_screenTween.TweenProperty((GodotObject)(object)_backstop, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0f), 0.25);
			_screenTween.TweenProperty((GodotObject)(object)_leftButton, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0f), 0.1);
			_screenTween.TweenProperty((GodotObject)(object)_rightButton, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0f), 0.1);
			_screenTween.TweenProperty((GodotObject)(object)_popup, NodePath.op_Implicit("modulate"), Variant.op_Implicit(StsColors.transparentWhite), 0.1);
			_screenTween.Chain().TweenCallback(Callable.From((Action)delegate
			{
				((CanvasItem)this).Visible = false;
				ActiveScreenContext.Instance.Update();
			}));
			NHoverTipSet.Clear();
		}
	}

	private void OnBackstopPressed(NButton _)
	{
		Close();
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Expected O, but got Unknown
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Expected O, but got Unknown
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Expected O, but got Unknown
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Expected O, but got Unknown
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		//IL_028b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e4: Expected O, but got Unknown
		//IL_02df: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(10);
		list.Add(new MethodInfo(MethodName.Create, new PropertyInfo((Type)24, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false), (MethodFlags)33, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Input, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("inputEvent"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("InputEvent"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnRightButtonPressed, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("button"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnLeftButtonPressed, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("button"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetRelic, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("index"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UpdateRelicDisplay, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetRarityVisuals, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("rarity"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.Close, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnBackstopPressed, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.Create && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			NInspectRelicScreen nInspectRelicScreen = Create();
			ret = VariantUtils.CreateFrom<NInspectRelicScreen>(ref nInspectRelicScreen);
			return true;
		}
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._Input && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			((Node)this)._Input(VariantUtils.ConvertTo<InputEvent>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnRightButtonPressed && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OnRightButtonPressed(VariantUtils.ConvertTo<NButton>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnLeftButtonPressed && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OnLeftButtonPressed(VariantUtils.ConvertTo<NButton>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetRelic && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			SetRelic(VariantUtils.ConvertTo<int>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.UpdateRelicDisplay && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			UpdateRelicDisplay();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetRarityVisuals && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			SetRarityVisuals(VariantUtils.ConvertTo<RelicRarity>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.Close && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			Close();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnBackstopPressed && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OnBackstopPressed(VariantUtils.ConvertTo<NButton>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		return ((Control)this).InvokeGodotClassMethod(ref method, args, ref ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static bool InvokeGodotClassStaticMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.Create && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			NInspectRelicScreen nInspectRelicScreen = Create();
			ret = VariantUtils.CreateFrom<NInspectRelicScreen>(ref nInspectRelicScreen);
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
		if ((ref method) == MethodName._Input)
		{
			return true;
		}
		if ((ref method) == MethodName.OnRightButtonPressed)
		{
			return true;
		}
		if ((ref method) == MethodName.OnLeftButtonPressed)
		{
			return true;
		}
		if ((ref method) == MethodName.SetRelic)
		{
			return true;
		}
		if ((ref method) == MethodName.UpdateRelicDisplay)
		{
			return true;
		}
		if ((ref method) == MethodName.SetRarityVisuals)
		{
			return true;
		}
		if ((ref method) == MethodName.Close)
		{
			return true;
		}
		if ((ref method) == MethodName.OnBackstopPressed)
		{
			return true;
		}
		return ((Control)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName._popup)
		{
			_popup = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._backstop)
		{
			_backstop = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._nameLabel)
		{
			_nameLabel = VariantUtils.ConvertTo<MegaLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._rarityLabel)
		{
			_rarityLabel = VariantUtils.ConvertTo<MegaLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._description)
		{
			_description = VariantUtils.ConvertTo<MegaRichTextLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._flavor)
		{
			_flavor = VariantUtils.ConvertTo<MegaRichTextLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._relicImage)
		{
			_relicImage = VariantUtils.ConvertTo<TextureRect>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._frameHsv)
		{
			_frameHsv = VariantUtils.ConvertTo<ShaderMaterial>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._leftButton)
		{
			_leftButton = VariantUtils.ConvertTo<NGoldArrowButton>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._rightButton)
		{
			_rightButton = VariantUtils.ConvertTo<NGoldArrowButton>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._hoverTipRect)
		{
			_hoverTipRect = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._screenTween)
		{
			_screenTween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._popupTween)
		{
			_popupTween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._popupPosition)
		{
			_popupPosition = VariantUtils.ConvertTo<Vector2>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._leftButtonX)
		{
			_leftButtonX = VariantUtils.ConvertTo<float>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._rightButtonX)
		{
			_rightButtonX = VariantUtils.ConvertTo<float>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._index)
		{
			_index = VariantUtils.ConvertTo<int>(ref value);
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
		if ((ref name) == PropertyName.DefaultFocusedControl)
		{
			Control defaultFocusedControl = DefaultFocusedControl;
			value = VariantUtils.CreateFrom<Control>(ref defaultFocusedControl);
			return true;
		}
		if ((ref name) == PropertyName._popup)
		{
			value = VariantUtils.CreateFrom<Control>(ref _popup);
			return true;
		}
		if ((ref name) == PropertyName._backstop)
		{
			value = VariantUtils.CreateFrom<Control>(ref _backstop);
			return true;
		}
		if ((ref name) == PropertyName._nameLabel)
		{
			value = VariantUtils.CreateFrom<MegaLabel>(ref _nameLabel);
			return true;
		}
		if ((ref name) == PropertyName._rarityLabel)
		{
			value = VariantUtils.CreateFrom<MegaLabel>(ref _rarityLabel);
			return true;
		}
		if ((ref name) == PropertyName._description)
		{
			value = VariantUtils.CreateFrom<MegaRichTextLabel>(ref _description);
			return true;
		}
		if ((ref name) == PropertyName._flavor)
		{
			value = VariantUtils.CreateFrom<MegaRichTextLabel>(ref _flavor);
			return true;
		}
		if ((ref name) == PropertyName._relicImage)
		{
			value = VariantUtils.CreateFrom<TextureRect>(ref _relicImage);
			return true;
		}
		if ((ref name) == PropertyName._frameHsv)
		{
			value = VariantUtils.CreateFrom<ShaderMaterial>(ref _frameHsv);
			return true;
		}
		if ((ref name) == PropertyName._leftButton)
		{
			value = VariantUtils.CreateFrom<NGoldArrowButton>(ref _leftButton);
			return true;
		}
		if ((ref name) == PropertyName._rightButton)
		{
			value = VariantUtils.CreateFrom<NGoldArrowButton>(ref _rightButton);
			return true;
		}
		if ((ref name) == PropertyName._hoverTipRect)
		{
			value = VariantUtils.CreateFrom<Control>(ref _hoverTipRect);
			return true;
		}
		if ((ref name) == PropertyName._screenTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _screenTween);
			return true;
		}
		if ((ref name) == PropertyName._popupTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _popupTween);
			return true;
		}
		if ((ref name) == PropertyName._popupPosition)
		{
			value = VariantUtils.CreateFrom<Vector2>(ref _popupPosition);
			return true;
		}
		if ((ref name) == PropertyName._leftButtonX)
		{
			value = VariantUtils.CreateFrom<float>(ref _leftButtonX);
			return true;
		}
		if ((ref name) == PropertyName._rightButtonX)
		{
			value = VariantUtils.CreateFrom<float>(ref _rightButtonX);
			return true;
		}
		if ((ref name) == PropertyName._index)
		{
			value = VariantUtils.CreateFrom<int>(ref _index);
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
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._popup, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._backstop, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._nameLabel, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._rarityLabel, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._description, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._flavor, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._relicImage, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._frameHsv, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._leftButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._rightButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._hoverTipRect, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._screenTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._popupTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)5, PropertyName._popupPosition, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)3, PropertyName._leftButtonX, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)3, PropertyName._rightButtonX, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)2, PropertyName._index, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		((GodotObject)this).SaveGodotObjectData(info);
		info.AddProperty(PropertyName._popup, Variant.From<Control>(ref _popup));
		info.AddProperty(PropertyName._backstop, Variant.From<Control>(ref _backstop));
		info.AddProperty(PropertyName._nameLabel, Variant.From<MegaLabel>(ref _nameLabel));
		info.AddProperty(PropertyName._rarityLabel, Variant.From<MegaLabel>(ref _rarityLabel));
		info.AddProperty(PropertyName._description, Variant.From<MegaRichTextLabel>(ref _description));
		info.AddProperty(PropertyName._flavor, Variant.From<MegaRichTextLabel>(ref _flavor));
		info.AddProperty(PropertyName._relicImage, Variant.From<TextureRect>(ref _relicImage));
		info.AddProperty(PropertyName._frameHsv, Variant.From<ShaderMaterial>(ref _frameHsv));
		info.AddProperty(PropertyName._leftButton, Variant.From<NGoldArrowButton>(ref _leftButton));
		info.AddProperty(PropertyName._rightButton, Variant.From<NGoldArrowButton>(ref _rightButton));
		info.AddProperty(PropertyName._hoverTipRect, Variant.From<Control>(ref _hoverTipRect));
		info.AddProperty(PropertyName._screenTween, Variant.From<Tween>(ref _screenTween));
		info.AddProperty(PropertyName._popupTween, Variant.From<Tween>(ref _popupTween));
		info.AddProperty(PropertyName._popupPosition, Variant.From<Vector2>(ref _popupPosition));
		info.AddProperty(PropertyName._leftButtonX, Variant.From<float>(ref _leftButtonX));
		info.AddProperty(PropertyName._rightButtonX, Variant.From<float>(ref _rightButtonX));
		info.AddProperty(PropertyName._index, Variant.From<int>(ref _index));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._popup, ref val))
		{
			_popup = ((Variant)(ref val)).As<Control>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._backstop, ref val2))
		{
			_backstop = ((Variant)(ref val2)).As<Control>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._nameLabel, ref val3))
		{
			_nameLabel = ((Variant)(ref val3)).As<MegaLabel>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._rarityLabel, ref val4))
		{
			_rarityLabel = ((Variant)(ref val4)).As<MegaLabel>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._description, ref val5))
		{
			_description = ((Variant)(ref val5)).As<MegaRichTextLabel>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._flavor, ref val6))
		{
			_flavor = ((Variant)(ref val6)).As<MegaRichTextLabel>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName._relicImage, ref val7))
		{
			_relicImage = ((Variant)(ref val7)).As<TextureRect>();
		}
		Variant val8 = default(Variant);
		if (info.TryGetProperty(PropertyName._frameHsv, ref val8))
		{
			_frameHsv = ((Variant)(ref val8)).As<ShaderMaterial>();
		}
		Variant val9 = default(Variant);
		if (info.TryGetProperty(PropertyName._leftButton, ref val9))
		{
			_leftButton = ((Variant)(ref val9)).As<NGoldArrowButton>();
		}
		Variant val10 = default(Variant);
		if (info.TryGetProperty(PropertyName._rightButton, ref val10))
		{
			_rightButton = ((Variant)(ref val10)).As<NGoldArrowButton>();
		}
		Variant val11 = default(Variant);
		if (info.TryGetProperty(PropertyName._hoverTipRect, ref val11))
		{
			_hoverTipRect = ((Variant)(ref val11)).As<Control>();
		}
		Variant val12 = default(Variant);
		if (info.TryGetProperty(PropertyName._screenTween, ref val12))
		{
			_screenTween = ((Variant)(ref val12)).As<Tween>();
		}
		Variant val13 = default(Variant);
		if (info.TryGetProperty(PropertyName._popupTween, ref val13))
		{
			_popupTween = ((Variant)(ref val13)).As<Tween>();
		}
		Variant val14 = default(Variant);
		if (info.TryGetProperty(PropertyName._popupPosition, ref val14))
		{
			_popupPosition = ((Variant)(ref val14)).As<Vector2>();
		}
		Variant val15 = default(Variant);
		if (info.TryGetProperty(PropertyName._leftButtonX, ref val15))
		{
			_leftButtonX = ((Variant)(ref val15)).As<float>();
		}
		Variant val16 = default(Variant);
		if (info.TryGetProperty(PropertyName._rightButtonX, ref val16))
		{
			_rightButtonX = ((Variant)(ref val16)).As<float>();
		}
		Variant val17 = default(Variant);
		if (info.TryGetProperty(PropertyName._index, ref val17))
		{
			_index = ((Variant)(ref val17)).As<int>();
		}
	}
}
