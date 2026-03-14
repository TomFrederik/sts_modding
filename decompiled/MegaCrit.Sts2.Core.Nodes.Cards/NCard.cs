using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Enchantments;
using MegaCrit.Sts2.Core.Entities.UI;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Helpers.Models;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Pooling;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Nodes.Vfx.Cards;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.Settings;
using MegaCrit.Sts2.Core.TestSupport;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.Nodes.Cards;

[ScriptPath("res://src/Core/Nodes/Cards/NCard.cs")]
public class NCard : Control, IPoolable
{
	public class MethodName : MethodName
	{
		public static readonly StringName OnInstantiated = StringName.op_Implicit("OnInstantiated");

		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName _EnterTree = StringName.op_Implicit("_EnterTree");

		public static readonly StringName _ExitTree = StringName.op_Implicit("_ExitTree");

		public static readonly StringName InitPool = StringName.op_Implicit("InitPool");

		public static readonly StringName GetCurrentSize = StringName.op_Implicit("GetCurrentSize");

		public static readonly StringName UpdateVisuals = StringName.op_Implicit("UpdateVisuals");

		public static readonly StringName ShowUpgradePreview = StringName.op_Implicit("ShowUpgradePreview");

		public static readonly StringName UpdateEnchantmentVisuals = StringName.op_Implicit("UpdateEnchantmentVisuals");

		public static readonly StringName OnEnchantmentStatusChanged = StringName.op_Implicit("OnEnchantmentStatusChanged");

		public static readonly StringName SetEnchantmentStatus = StringName.op_Implicit("SetEnchantmentStatus");

		public static readonly StringName UpdateEnergyCostVisuals = StringName.op_Implicit("UpdateEnergyCostVisuals");

		public static readonly StringName SetPretendCardCanBePlayed = StringName.op_Implicit("SetPretendCardCanBePlayed");

		public static readonly StringName SetForceUnpoweredPreview = StringName.op_Implicit("SetForceUnpoweredPreview");

		public static readonly StringName UpdateEnergyCostColor = StringName.op_Implicit("UpdateEnergyCostColor");

		public static readonly StringName UpdateStarCostVisuals = StringName.op_Implicit("UpdateStarCostVisuals");

		public static readonly StringName UpdateStarCostText = StringName.op_Implicit("UpdateStarCostText");

		public static readonly StringName UpdateStarCostColor = StringName.op_Implicit("UpdateStarCostColor");

		public static readonly StringName GetCostTextColorInHand = StringName.op_Implicit("GetCostTextColorInHand");

		public static readonly StringName GetCostOutlineColorInHand = StringName.op_Implicit("GetCostOutlineColorInHand");

		public static readonly StringName PlayRandomizeCostAnim = StringName.op_Implicit("PlayRandomizeCostAnim");

		public static readonly StringName Reload = StringName.op_Implicit("Reload");

		public static readonly StringName UpdateTypePlaque = StringName.op_Implicit("UpdateTypePlaque");

		public static readonly StringName UpdateTypePlaqueSizeAndPosition = StringName.op_Implicit("UpdateTypePlaqueSizeAndPosition");

		public static readonly StringName UpdateTitleLabel = StringName.op_Implicit("UpdateTitleLabel");

		public static readonly StringName GetTitleLabelOutlineColor = StringName.op_Implicit("GetTitleLabelOutlineColor");

		public static readonly StringName ReloadOverlay = StringName.op_Implicit("ReloadOverlay");

		public static readonly StringName OnAfflictionChanged = StringName.op_Implicit("OnAfflictionChanged");

		public static readonly StringName OnEnchantmentChanged = StringName.op_Implicit("OnEnchantmentChanged");

		public static readonly StringName GetTitleText = StringName.op_Implicit("GetTitleText");

		public static readonly StringName ActivateRewardScreenGlow = StringName.op_Implicit("ActivateRewardScreenGlow");

		public static readonly StringName KillRarityGlow = StringName.op_Implicit("KillRarityGlow");

		public static readonly StringName AnimCardToPlayPile = StringName.op_Implicit("AnimCardToPlayPile");

		public static readonly StringName OnReturnedFromPool = StringName.op_Implicit("OnReturnedFromPool");

		public static readonly StringName OnFreedToPool = StringName.op_Implicit("OnFreedToPool");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName CardHighlight = StringName.op_Implicit("CardHighlight");

		public static readonly StringName Body = StringName.op_Implicit("Body");

		public static readonly StringName Visibility = StringName.op_Implicit("Visibility");

		public static readonly StringName PlayPileTween = StringName.op_Implicit("PlayPileTween");

		public static readonly StringName RandomizeCostTween = StringName.op_Implicit("RandomizeCostTween");

		public static readonly StringName DisplayingPile = StringName.op_Implicit("DisplayingPile");

		public static readonly StringName EnchantmentTab = StringName.op_Implicit("EnchantmentTab");

		public static readonly StringName EnchantmentVfxOverride = StringName.op_Implicit("EnchantmentVfxOverride");

		public static readonly StringName _titleLabel = StringName.op_Implicit("_titleLabel");

		public static readonly StringName _descriptionLabel = StringName.op_Implicit("_descriptionLabel");

		public static readonly StringName _ancientPortrait = StringName.op_Implicit("_ancientPortrait");

		public static readonly StringName _portrait = StringName.op_Implicit("_portrait");

		public static readonly StringName _frame = StringName.op_Implicit("_frame");

		public static readonly StringName _ancientBorder = StringName.op_Implicit("_ancientBorder");

		public static readonly StringName _ancientBanner = StringName.op_Implicit("_ancientBanner");

		public static readonly StringName _ancientTextBg = StringName.op_Implicit("_ancientTextBg");

		public static readonly StringName _ancientHighlight = StringName.op_Implicit("_ancientHighlight");

		public static readonly StringName _portraitBorder = StringName.op_Implicit("_portraitBorder");

		public static readonly StringName _banner = StringName.op_Implicit("_banner");

		public static readonly StringName _lock = StringName.op_Implicit("_lock");

		public static readonly StringName _typePlaque = StringName.op_Implicit("_typePlaque");

		public static readonly StringName _typeLabel = StringName.op_Implicit("_typeLabel");

		public static readonly StringName _portraitCanvasGroup = StringName.op_Implicit("_portraitCanvasGroup");

		public static readonly StringName _rareGlow = StringName.op_Implicit("_rareGlow");

		public static readonly StringName _uncommonGlow = StringName.op_Implicit("_uncommonGlow");

		public static readonly StringName _sparkles = StringName.op_Implicit("_sparkles");

		public static readonly StringName _energyIcon = StringName.op_Implicit("_energyIcon");

		public static readonly StringName _energyLabel = StringName.op_Implicit("_energyLabel");

		public static readonly StringName _unplayableEnergyIcon = StringName.op_Implicit("_unplayableEnergyIcon");

		public static readonly StringName _starIcon = StringName.op_Implicit("_starIcon");

		public static readonly StringName _starLabel = StringName.op_Implicit("_starLabel");

		public static readonly StringName _unplayableStarIcon = StringName.op_Implicit("_unplayableStarIcon");

		public static readonly StringName _overlayContainer = StringName.op_Implicit("_overlayContainer");

		public static readonly StringName _cardOverlay = StringName.op_Implicit("_cardOverlay");

		public static readonly StringName _enchantmentTab = StringName.op_Implicit("_enchantmentTab");

		public static readonly StringName _enchantmentVfxOverride = StringName.op_Implicit("_enchantmentVfxOverride");

		public static readonly StringName _enchantmentIcon = StringName.op_Implicit("_enchantmentIcon");

		public static readonly StringName _enchantmentLabel = StringName.op_Implicit("_enchantmentLabel");

		public static readonly StringName _defaultEnchantmentPosition = StringName.op_Implicit("_defaultEnchantmentPosition");

		public static readonly StringName _pretendCardCanBePlayed = StringName.op_Implicit("_pretendCardCanBePlayed");

		public static readonly StringName _forceUnpoweredPreview = StringName.op_Implicit("_forceUnpoweredPreview");

		public static readonly StringName _portraitBlurMaterial = StringName.op_Implicit("_portraitBlurMaterial");

		public static readonly StringName _canvasGroupMaskBlurMaterial = StringName.op_Implicit("_canvasGroupMaskBlurMaterial");

		public static readonly StringName _canvasGroupBlurMaterial = StringName.op_Implicit("_canvasGroupBlurMaterial");

		public static readonly StringName _canvasGroupMaskMaterial = StringName.op_Implicit("_canvasGroupMaskMaterial");

		public static readonly StringName _visibility = StringName.op_Implicit("_visibility");
	}

	public class SignalName : SignalName
	{
	}

	private const string _scenePath = "res://scenes/cards/card.tscn";

	private static readonly string _portraitBlurMaterialPath = "res://scenes/cards/card_portrait_blur_material.tres";

	private static readonly string _canvasGroupMaskMaterialPath = "res://scenes/cards/card_canvas_group_mask_material.tres";

	private static readonly string _canvasGroupBlurMaterialPath = "res://scenes/cards/card_canvas_group_blur_material.tres";

	private static readonly string _canvasGroupMaskBlurMaterialPath = "res://scenes/cards/card_canvas_group_mask_blur_material.tres";

	private static readonly float _typePlaqueXMargin = 17f;

	private static readonly float _typePlaqueMinXSize = 61f;

	private static readonly StringName _v = new StringName("v");

	private static readonly StringName _s = new StringName("s");

	private static readonly StringName _h = new StringName("h");

	public static readonly Vector2 defaultSize = new Vector2(300f, 422f);

	private CardModel? _model;

	private MegaLabel _titleLabel;

	private MegaRichTextLabel _descriptionLabel;

	private TextureRect _ancientPortrait;

	private TextureRect _portrait;

	private TextureRect _frame;

	private TextureRect _ancientBorder;

	private Control _ancientBanner;

	private TextureRect _ancientTextBg;

	private TextureRect _ancientHighlight;

	private TextureRect _portraitBorder;

	private TextureRect _banner;

	private TextureRect _lock;

	private NinePatchRect _typePlaque;

	private MegaLabel _typeLabel;

	private CanvasGroup _portraitCanvasGroup;

	private NCardRareGlow? _rareGlow;

	private NCardUncommonGlow? _uncommonGlow;

	private GpuParticles2D _sparkles;

	private readonly List<NRelicFlashVfx> _flashVfx = new List<NRelicFlashVfx>();

	private TextureRect _energyIcon;

	private MegaLabel _energyLabel;

	private TextureRect _unplayableEnergyIcon;

	private TextureRect _starIcon;

	private MegaLabel _starLabel;

	private TextureRect _unplayableStarIcon;

	private Node _overlayContainer;

	private Control? _cardOverlay;

	private Creature? _previewTarget;

	private Control _enchantmentTab;

	private TextureRect _enchantmentVfxOverride;

	private TextureRect _enchantmentIcon;

	private MegaLabel _enchantmentLabel;

	private Vector2 _defaultEnchantmentPosition;

	private const int _enchantmentTabStarLabelOffset = 45;

	private EnchantmentModel? _subscribedEnchantment;

	private bool _pretendCardCanBePlayed;

	private bool _forceUnpoweredPreview;

	private Material? _portraitBlurMaterial;

	private Material? _canvasGroupMaskBlurMaterial;

	private Material? _canvasGroupBlurMaterial;

	private Material? _canvasGroupMaskMaterial;

	private ModelVisibility _visibility = ModelVisibility.Visible;

	private readonly LocString _unknownDescription = new LocString("card_library", "UNKNOWN.description");

	private readonly LocString _unknownTitle = new LocString("card_library", "UNKNOWN.title");

	private readonly LocString _lockedDescription = new LocString("card_library", "LOCKED.description");

	private readonly LocString _lockedTitle = new LocString("card_library", "LOCKED.title");

	public NCardHighlight CardHighlight { get; private set; }

	public Control Body { get; private set; }

	public ModelVisibility Visibility
	{
		get
		{
			return _visibility;
		}
		set
		{
			if (_visibility != value)
			{
				_visibility = value;
				Reload();
			}
		}
	}

	public Tween? PlayPileTween { get; set; }

	private Tween? RandomizeCostTween { get; set; }

	public PileType DisplayingPile { get; private set; }

	public Control EnchantmentTab => _enchantmentTab;

	public TextureRect EnchantmentVfxOverride => _enchantmentVfxOverride;

	public CardModel? Model
	{
		get
		{
			return _model;
		}
		set
		{
			if (_model != value)
			{
				CardModel model = _model;
				UnsubscribeFromModel(model);
				_model = value;
				Reload();
				SubscribeToModel(_model);
				this.ModelChanged?.Invoke(model);
				if (_model != null && (_model.RunState != null || _model.CombatState != null) && LocalContext.IsMine(_model))
				{
					SaveManager.Instance.MarkCardAsSeen(_model);
				}
			}
		}
	}

	public static IEnumerable<string> AssetPaths => new global::_003C_003Ez__ReadOnlyArray<string>(new string[5] { "res://scenes/cards/card.tscn", _portraitBlurMaterialPath, _canvasGroupBlurMaterialPath, _canvasGroupMaskBlurMaterialPath, _canvasGroupMaskMaterialPath });

	public event Action<CardModel?>? ModelChanged;

	public void OnInstantiated()
	{
	}

	public override void _Ready()
	{
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		//IL_0296: Unknown result type (might be due to invalid IL or missing references)
		_titleLabel = ((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%TitleLabel"));
		_descriptionLabel = ((Node)this).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("%DescriptionLabel"));
		_frame = ((Node)this).GetNode<TextureRect>(NodePath.op_Implicit("%Frame"));
		_ancientBorder = ((Node)this).GetNode<TextureRect>(NodePath.op_Implicit("%AncientBorder"));
		_ancientTextBg = ((Node)this).GetNode<TextureRect>(NodePath.op_Implicit("%AncientTextBg"));
		_ancientBanner = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%AncientBanner"));
		_ancientHighlight = ((Node)this).GetNode<TextureRect>(NodePath.op_Implicit("%AncientHighlight"));
		_portrait = ((Node)this).GetNode<TextureRect>(NodePath.op_Implicit("%Portrait"));
		_ancientPortrait = ((Node)this).GetNode<TextureRect>(NodePath.op_Implicit("%AncientPortrait"));
		_typeLabel = ((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%TypeLabel"));
		_portraitBorder = ((Node)this).GetNode<TextureRect>(NodePath.op_Implicit("%PortraitBorder"));
		_portraitCanvasGroup = ((Node)this).GetNode<CanvasGroup>(NodePath.op_Implicit("%PortraitCanvasGroup"));
		_energyLabel = ((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%EnergyLabel"));
		_energyIcon = ((Node)this).GetNode<TextureRect>(NodePath.op_Implicit("%EnergyIcon"));
		_starLabel = ((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%StarLabel"));
		_starIcon = ((Node)this).GetNode<TextureRect>(NodePath.op_Implicit("%StarIcon"));
		_banner = ((Node)this).GetNode<TextureRect>(NodePath.op_Implicit("%TitleBanner"));
		_lock = ((Node)this).GetNode<TextureRect>(NodePath.op_Implicit("%Lock"));
		_typePlaque = ((Node)this).GetNode<NinePatchRect>(NodePath.op_Implicit("%TypePlaque"));
		_unplayableEnergyIcon = ((Node)this).GetNode<TextureRect>(NodePath.op_Implicit("%UnplayableEnergyIcon"));
		_unplayableStarIcon = ((Node)this).GetNode<TextureRect>(NodePath.op_Implicit("%UnplayableStarIcon"));
		_enchantmentIcon = ((Node)this).GetNode<TextureRect>(NodePath.op_Implicit("%Enchantment/Icon"));
		_enchantmentLabel = ((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%Enchantment/Label"));
		_sparkles = ((Node)this).GetNode<GpuParticles2D>(NodePath.op_Implicit("CardContainer/CardSparkles"));
		_enchantmentTab = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%Enchantment"));
		((CanvasItem)_enchantmentTab).Visible = false;
		_enchantmentVfxOverride = ((Node)this).GetNode<TextureRect>(NodePath.op_Implicit("%EnchantmentVfxOverride"));
		CardHighlight = ((Node)this).GetNode<NCardHighlight>(NodePath.op_Implicit("%Highlight"));
		Body = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%CardContainer"));
		_overlayContainer = ((Node)this).GetNode(NodePath.op_Implicit("%OverlayContainer"));
		_defaultEnchantmentPosition = _enchantmentTab.Position;
		Reload();
	}

	public override void _EnterTree()
	{
		SubscribeToModel(Model);
	}

	public override void _ExitTree()
	{
		UnsubscribeFromModel(Model);
	}

	private void SubscribeToModel(CardModel? model)
	{
		if (model != null && ((Node)this).IsInsideTree())
		{
			model.AfflictionChanged += OnAfflictionChanged;
			model.EnchantmentChanged += OnEnchantmentChanged;
			SubscribeToEnchantment(model.Enchantment);
		}
	}

	private void UnsubscribeFromModel(CardModel? model)
	{
		if (model != null)
		{
			model.AfflictionChanged -= OnAfflictionChanged;
			model.EnchantmentChanged -= OnEnchantmentChanged;
			UnsubscribeFromEnchantment(model.Enchantment);
		}
	}

	private void SubscribeToEnchantment(EnchantmentModel? model)
	{
		if (model != null && ((Node)this).IsInsideTree())
		{
			if (_subscribedEnchantment != null)
			{
				throw new InvalidOperationException($"Attempted to subscribe to enchantment {model}, but {this} is already subscribed to {_subscribedEnchantment}!");
			}
			_subscribedEnchantment = model;
			_subscribedEnchantment.StatusChanged += OnEnchantmentStatusChanged;
		}
	}

	private void UnsubscribeFromEnchantment(EnchantmentModel? model)
	{
		if (model != null && model == _subscribedEnchantment)
		{
			_subscribedEnchantment.StatusChanged -= OnEnchantmentStatusChanged;
			_subscribedEnchantment = null;
		}
	}

	public static void InitPool()
	{
		NodePool.Init<NCard>("res://scenes/cards/card.tscn", 30);
	}

	public static NCard? Create(CardModel card, ModelVisibility visibility = ModelVisibility.Visible)
	{
		if (TestMode.IsOn)
		{
			return null;
		}
		NCard nCard = NodePool.Get<NCard>();
		nCard.Model = card;
		nCard.Visibility = visibility;
		return nCard;
	}

	public static NCard? FindOnTable(CardModel card, PileType? overridePile = null)
	{
		if (TestMode.IsOn)
		{
			return null;
		}
		if (!CombatManager.Instance.IsInProgress)
		{
			return null;
		}
		NCombatUi nCombatUi = NCombatRoom.Instance?.Ui;
		if (nCombatUi == null)
		{
			return null;
		}
		CardPile? pile = card.Pile;
		return ((pile != null) ? new PileType?(pile.Type) : overridePile) switch
		{
			PileType.None => null, 
			PileType.Draw => null, 
			PileType.Hand => nCombatUi.Hand.GetCard(card) ?? nCombatUi.PlayQueue.GetCardNode(card) ?? nCombatUi.GetCardFromPlayContainer(card), 
			PileType.Discard => null, 
			PileType.Exhaust => null, 
			PileType.Play => nCombatUi.GetCardFromPlayContainer(card), 
			PileType.Deck => null, 
			null => null, 
			_ => throw new ArgumentOutOfRangeException(), 
		};
	}

	public Vector2 GetCurrentSize()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		return defaultSize * ((Control)this).Scale;
	}

	public void SetPreviewTarget(Creature? creature)
	{
		if (_previewTarget != creature)
		{
			_previewTarget = creature;
			UpdateVisuals(DisplayingPile, CardPreviewMode.Normal);
		}
	}

	public void UpdateVisuals(PileType pileType, CardPreviewMode previewMode)
	{
		if (!((Node)this).IsNodeReady())
		{
			return;
		}
		if (Model == null)
		{
			throw new InvalidOperationException("Cannot update text with no model.");
		}
		DisplayingPile = pileType;
		Creature target = _previewTarget ?? Model.CurrentTarget;
		UpdateTitleLabel();
		UpdateEnergyCostVisuals(pileType);
		UpdateStarCostVisuals(pileType);
		UpdateEnchantmentVisuals();
		Model.DynamicVars.ClearPreview();
		if (!_forceUnpoweredPreview)
		{
			Model.UpdateDynamicVarPreview(previewMode, target, Model.DynamicVars);
			if (Model.Enchantment != null)
			{
				Model.Enchantment.DynamicVars.ClearPreview();
				Model.UpdateDynamicVarPreview(previewMode, target, Model.Enchantment.DynamicVars);
			}
		}
		string text = ((previewMode != CardPreviewMode.Upgrade) ? Model.GetDescriptionForPile(pileType, target) : Model.GetDescriptionForUpgradePreview());
		switch (Visibility)
		{
		case ModelVisibility.Visible:
			_descriptionLabel.SetTextAutoSize("[center]" + text + "[/center]");
			break;
		case ModelVisibility.NotSeen:
			_descriptionLabel.SetTextAutoSize("[center][font_size=40]" + _unknownDescription.GetFormattedText() + "[/font_size][/center]");
			break;
		case ModelVisibility.Locked:
			_descriptionLabel.SetTextAutoSize("[center][font_size=40]" + _lockedDescription.GetFormattedText() + "[/font_size][/center]");
			break;
		default:
			throw new InvalidOperationException();
		}
	}

	public void ShowUpgradePreview()
	{
		UpdateVisuals(DisplayingPile, CardPreviewMode.Upgrade);
	}

	private void UpdateEnchantmentVisuals()
	{
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		if (Model == null)
		{
			throw new InvalidOperationException("Cannot show enchantment with no model.");
		}
		EnchantmentModel enchantment = Model.Enchantment;
		if (enchantment != null)
		{
			((CanvasItem)_enchantmentTab).Visible = true;
			_enchantmentIcon.Texture = (Texture2D)(object)enchantment.Icon;
			_enchantmentLabel.SetTextAutoSize(enchantment.DisplayAmount.ToString());
			((CanvasItem)_enchantmentLabel).Visible = enchantment.ShowAmount;
			SetEnchantmentStatus(enchantment.Status);
		}
		else
		{
			((CanvasItem)_enchantmentTab).Visible = false;
		}
		if (Model.HasStarCostX || Model.CurrentStarCost >= 0)
		{
			_enchantmentTab.Position = _defaultEnchantmentPosition;
		}
		else
		{
			_enchantmentTab.Position = _defaultEnchantmentPosition + Vector2.Up * 45f;
		}
	}

	private void OnEnchantmentStatusChanged()
	{
		SetEnchantmentStatus(Model?.Enchantment?.Status ?? EnchantmentStatus.Disabled);
	}

	private void SetEnchantmentStatus(EnchantmentStatus status)
	{
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Expected O, but got Unknown
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Expected O, but got Unknown
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		if (status == EnchantmentStatus.Disabled)
		{
			((CanvasItem)_enchantmentTab).Modulate = new Color(1f, 1f, 1f, 0.9f);
			ShaderMaterial val = (ShaderMaterial)((CanvasItem)_enchantmentTab).Material;
			val.SetShaderParameter(_h, Variant.op_Implicit(0.25));
			val.SetShaderParameter(_s, Variant.op_Implicit(0.1));
			val.SetShaderParameter(_v, Variant.op_Implicit(0.6));
			((CanvasItem)_enchantmentIcon).UseParentMaterial = true;
			((CanvasItem)_enchantmentLabel).SelfModulate = StsColors.gray;
		}
		else
		{
			((CanvasItem)_enchantmentTab).Modulate = Colors.White;
			ShaderMaterial val2 = (ShaderMaterial)((CanvasItem)_enchantmentTab).Material;
			val2.SetShaderParameter(_h, Variant.op_Implicit(0.25));
			val2.SetShaderParameter(_s, Variant.op_Implicit(0.4));
			val2.SetShaderParameter(_v, Variant.op_Implicit(0.6));
			((CanvasItem)_enchantmentIcon).UseParentMaterial = false;
			((CanvasItem)_enchantmentLabel).SelfModulate = Colors.White;
		}
	}

	private void UpdateEnergyCostVisuals(PileType pileType)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		if (Visibility != ModelVisibility.Visible)
		{
			_energyLabel.SetTextAutoSize("?");
			((CanvasItem)_energyIcon).Visible = true;
			((Control)_energyLabel).AddThemeColorOverride(ThemeConstants.Label.fontColor, StsColors.cream);
			((Control)_energyLabel).AddThemeColorOverride(ThemeConstants.Label.fontOutlineColor, Model.Pool.EnergyOutlineColor);
			return;
		}
		if (Model.EnergyCost.CostsX)
		{
			_energyLabel.SetTextAutoSize("X");
			((CanvasItem)_energyIcon).Visible = true;
		}
		else
		{
			int withModifiers = Model.EnergyCost.GetWithModifiers(CostModifiers.All);
			_energyLabel.SetTextAutoSize(withModifiers.ToString());
			((CanvasItem)_energyIcon).Visible = withModifiers >= 0;
		}
		UpdateEnergyCostColor(pileType);
		if (pileType == PileType.Hand && !Model.CanPlay(out UnplayableReason reason, out AbstractModel _))
		{
			((CanvasItem)_unplayableEnergyIcon).Visible = !reason.HasResourceCostReason();
		}
		else
		{
			((CanvasItem)_unplayableEnergyIcon).Visible = false;
		}
	}

	public void SetPretendCardCanBePlayed(bool pretendCardCanBePlayed)
	{
		_pretendCardCanBePlayed = pretendCardCanBePlayed;
		UpdateEnergyCostVisuals(DisplayingPile);
		UpdateStarCostVisuals(DisplayingPile);
	}

	public void SetForceUnpoweredPreview(bool forceUnpoweredPreview)
	{
		_forceUnpoweredPreview = forceUnpoweredPreview;
	}

	private void UpdateEnergyCostColor(PileType pileType)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		Color val = StsColors.cream;
		Color val2 = Model.Pool.EnergyOutlineColor;
		CardEnergyCost energyCost = Model.EnergyCost;
		if (energyCost != null && !energyCost.CostsX && energyCost.WasJustUpgraded)
		{
			val = StsColors.green;
			val2 = StsColors.energyGreenOutline;
		}
		else if (pileType == PileType.Hand)
		{
			CardCostColor energyCostColor = CardCostHelper.GetEnergyCostColor(Model, Model.CombatState);
			val = GetCostTextColorInHand(energyCostColor, _pretendCardCanBePlayed, val);
			val2 = GetCostOutlineColorInHand(energyCostColor, _pretendCardCanBePlayed, val2);
		}
		((Control)_energyLabel).AddThemeColorOverride(ThemeConstants.Label.fontColor, val);
		((Control)_energyLabel).AddThemeColorOverride(ThemeConstants.Label.fontOutlineColor, val2);
	}

	private void UpdateStarCostVisuals(PileType pileType)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		if (Visibility != ModelVisibility.Visible)
		{
			_starLabel.SetTextAutoSize(string.Empty);
			((CanvasItem)_starIcon).Visible = false;
			((Control)_starLabel).AddThemeColorOverride(ThemeConstants.Label.fontColor, StsColors.cream);
			((Control)_starLabel).AddThemeColorOverride(ThemeConstants.Label.fontOutlineColor, Model.Pool.EnergyOutlineColor);
			return;
		}
		if (Model.HasStarCostX)
		{
			_starLabel.SetTextAutoSize("X");
			((CanvasItem)_starIcon).Visible = true;
		}
		else
		{
			_starLabel.SetTextAutoSize(Model.GetStarCostWithModifiers().ToString());
			((CanvasItem)_starIcon).Visible = Model.GetStarCostWithModifiers() >= 0;
		}
		UpdateStarCostColor(pileType);
		if (pileType == PileType.Hand && !Model.CanPlay(out UnplayableReason reason, out AbstractModel _))
		{
			((CanvasItem)_unplayableStarIcon).Visible = !reason.HasResourceCostReason();
		}
		else
		{
			((CanvasItem)_unplayableStarIcon).Visible = false;
		}
	}

	private void UpdateStarCostText(int cost)
	{
		if (Model.HasStarCostX)
		{
			_starLabel.SetTextAutoSize("X");
			((CanvasItem)_starIcon).Visible = true;
		}
		else if (cost >= 0)
		{
			_starLabel.SetTextAutoSize(cost.ToString());
			((CanvasItem)_starIcon).Visible = true;
		}
		else
		{
			((CanvasItem)_starIcon).Visible = false;
		}
	}

	private void UpdateStarCostColor(PileType pileType)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		Color val = StsColors.cream;
		Color val2 = StsColors.defaultStarCostOutline;
		if (!Model.HasStarCostX && Model.WasStarCostJustUpgraded)
		{
			val = StsColors.green;
			val2 = StsColors.energyGreenOutline;
		}
		else if (pileType == PileType.Hand)
		{
			CardCostColor starCostColor = CardCostHelper.GetStarCostColor(Model, Model.CombatState);
			val = GetCostTextColorInHand(starCostColor, _pretendCardCanBePlayed, val);
			val2 = GetCostOutlineColorInHand(starCostColor, _pretendCardCanBePlayed, val2);
		}
		((Control)_starLabel).AddThemeColorOverride(ThemeConstants.Label.fontColor, val);
		((Control)_starLabel).AddThemeColorOverride(ThemeConstants.Label.fontOutlineColor, val2);
	}

	private static Color GetCostTextColorInHand(CardCostColor costColor, bool pretendCardCanBePlayed, Color defaultColor)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		return (Color)(costColor switch
		{
			CardCostColor.Unmodified => defaultColor, 
			CardCostColor.Increased => StsColors.energyBlue, 
			CardCostColor.Decreased => StsColors.green, 
			CardCostColor.InsufficientResources => pretendCardCanBePlayed ? defaultColor : StsColors.red, 
			_ => throw new ArgumentOutOfRangeException("costColor", costColor, null), 
		});
	}

	private static Color GetCostOutlineColorInHand(CardCostColor costColor, bool pretendCardCanBePlayed, Color defaultColor)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		return (Color)(costColor switch
		{
			CardCostColor.Unmodified => defaultColor, 
			CardCostColor.Increased => StsColors.energyBlueOutline, 
			CardCostColor.Decreased => StsColors.energyGreenOutline, 
			CardCostColor.InsufficientResources => pretendCardCanBePlayed ? defaultColor : StsColors.unplayableEnergyCostOutline, 
			_ => throw new ArgumentOutOfRangeException("costColor", costColor, null), 
		});
	}

	public void PlayRandomizeCostAnim()
	{
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		Tween? randomizeCostTween = RandomizeCostTween;
		if (randomizeCostTween != null)
		{
			randomizeCostTween.Kill();
		}
		RandomizeCostTween = ((Node)this).CreateTween();
		float offset = Rng.Chaotic.NextFloat(10f);
		RandomizeCostTween.TweenMethod(Callable.From<float>((Action<float>)delegate(float t)
		{
			int num = (int)(offset + t) % 8;
			if (num > 3)
			{
				_energyLabel.SetTextAutoSize("?");
			}
			else
			{
				_energyLabel.SetTextAutoSize((t % 4f).ToString());
			}
		}), Variant.op_Implicit(0), Variant.op_Implicit(50), (double)Rng.Chaotic.NextFloat(0.4f, 0.6f)).SetEase((EaseType)1).SetTrans((TransitionType)1);
		((GodotObject)RandomizeCostTween).Connect(SignalName.Finished, Callable.From((Action)delegate
		{
			UpdateEnergyCostVisuals(Model.Pile.Type);
		}), 4u);
	}

	private void Reload()
	{
		if (!((Node)this).IsNodeReady() || Model == null)
		{
			return;
		}
		if (OS.HasFeature("editor"))
		{
			((Node)this).Name = StringName.op_Implicit($"{typeof(NCard)}-{Model.Id}");
		}
		_energyIcon.Texture = Model.EnergyIcon;
		UpdateTypePlaque();
		bool flag = Model.Rarity == CardRarity.Ancient;
		((CanvasItem)_portraitBorder).Visible = !flag;
		((CanvasItem)_portrait).Visible = !flag;
		((CanvasItem)_frame).Visible = !flag;
		((CanvasItem)_ancientPortrait).Visible = flag;
		((CanvasItem)_ancientBorder).Visible = flag;
		((CanvasItem)_ancientTextBg).Visible = flag;
		((CanvasItem)_ancientBanner).Visible = flag;
		((CanvasItem)_banner).Visible = !flag;
		((CanvasItem)_lock).Visible = Visibility == ModelVisibility.Locked;
		Texture2D portrait = Model.Portrait;
		if (Visibility != ModelVisibility.Visible)
		{
			if (_portraitBlurMaterial == null)
			{
				_portraitBlurMaterial = PreloadManager.Cache.GetMaterial(_portraitBlurMaterialPath);
			}
			if (flag)
			{
				if (_canvasGroupMaskBlurMaterial == null)
				{
					_canvasGroupMaskBlurMaterial = PreloadManager.Cache.GetMaterial(_canvasGroupMaskBlurMaterialPath);
				}
				((CanvasItem)_portraitCanvasGroup).Material = _canvasGroupMaskBlurMaterial;
			}
			else
			{
				if (_canvasGroupBlurMaterial == null)
				{
					_canvasGroupBlurMaterial = PreloadManager.Cache.GetMaterial(_canvasGroupBlurMaterialPath);
				}
				((CanvasItem)_portraitCanvasGroup).Material = _canvasGroupBlurMaterial;
			}
			((CanvasItem)_portrait).Material = _portraitBlurMaterial;
			((CanvasItem)_ancientPortrait).Material = _portraitBlurMaterial;
		}
		else
		{
			if (flag)
			{
				if (_canvasGroupMaskMaterial == null)
				{
					_canvasGroupMaskMaterial = PreloadManager.Cache.GetMaterial(_canvasGroupMaskMaterialPath);
				}
				((CanvasItem)_portraitCanvasGroup).Material = _canvasGroupMaskMaterial;
			}
			else
			{
				((CanvasItem)_portraitCanvasGroup).Material = null;
			}
			((CanvasItem)_portrait).Material = null;
			((CanvasItem)_ancientPortrait).Material = null;
		}
		if (Model.Rarity != CardRarity.Ancient)
		{
			_portrait.Texture = portrait;
			_portraitBorder.Texture = Model.PortraitBorder;
			((CanvasItem)_portraitBorder).Material = Model.BannerMaterial;
			_frame.Texture = Model.Frame;
			((CanvasItem)_banner).Material = Model.BannerMaterial;
			_banner.Texture = Model.BannerTexture;
		}
		else
		{
			_ancientTextBg.Texture = Model.AncientTextBg;
			_ancientPortrait.Texture = portrait;
			((CanvasItem)_banner).Material = null;
		}
		((CanvasItem)_frame).Material = Model.FrameMaterial;
		ReloadOverlay();
	}

	private void UpdateTypePlaque()
	{
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		_typeLabel.SetTextAutoSize(Model.Type.ToLocString().GetFormattedText());
		Material bannerMaterial = Model.BannerMaterial;
		if (((CanvasItem)_typePlaque).Material != bannerMaterial)
		{
			((CanvasItem)_typePlaque).Material = Model.BannerMaterial;
		}
		Callable val = Callable.From((Action)UpdateTypePlaqueSizeAndPosition);
		((Callable)(ref val)).CallDeferred(Array.Empty<Variant>());
	}

	private void UpdateTypePlaqueSizeAndPosition()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		float num = ((Control)_typePlaque).Position.X + ((Control)_typePlaque).Size.X * 0.5f;
		NinePatchRect typePlaque = _typePlaque;
		Vector2 size = ((Control)_typePlaque).Size;
		size.X = Mathf.Max(((Control)_typeLabel).Size.X + _typePlaqueXMargin, _typePlaqueMinXSize);
		((Control)typePlaque).Size = size;
		NinePatchRect typePlaque2 = _typePlaque;
		size = ((Control)_typePlaque).Position;
		size.X = num - ((Control)_typePlaque).Size.X * 0.5f;
		((Control)typePlaque2).Position = size;
	}

	private void UpdateTitleLabel()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		string textAutoSize;
		Color val;
		Color val2;
		if (Visibility == ModelVisibility.NotSeen)
		{
			textAutoSize = _unknownTitle.GetFormattedText();
			val = StsColors.cream;
			val2 = GetTitleLabelOutlineColor();
		}
		else if (Visibility == ModelVisibility.Locked)
		{
			textAutoSize = _lockedTitle.GetFormattedText();
			val = StsColors.cream;
			val2 = GetTitleLabelOutlineColor();
		}
		else if (Model.CurrentUpgradeLevel == 0)
		{
			textAutoSize = Model.Title;
			val = StsColors.cream;
			val2 = GetTitleLabelOutlineColor();
		}
		else
		{
			textAutoSize = Model.Title;
			val = StsColors.green;
			val2 = StsColors.cardTitleOutlineSpecial;
		}
		_titleLabel.SetTextAutoSize(textAutoSize);
		((Control)_titleLabel).AddThemeColorOverride(ThemeConstants.Label.fontColor, val);
		((Control)_titleLabel).AddThemeColorOverride(ThemeConstants.Label.fontOutlineColor, val2);
	}

	private Color GetTitleLabelOutlineColor()
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		switch (_model.Rarity)
		{
		case CardRarity.None:
		case CardRarity.Basic:
		case CardRarity.Common:
		case CardRarity.Token:
			return StsColors.cardTitleOutlineCommon;
		case CardRarity.Uncommon:
			return StsColors.cardTitleOutlineUncommon;
		case CardRarity.Rare:
			return StsColors.cardTitleOutlineRare;
		case CardRarity.Curse:
			return StsColors.cardTitleOutlineCurse;
		case CardRarity.Quest:
			return StsColors.cardTitleOutlineQuest;
		case CardRarity.Status:
			return StsColors.cardTitleOutlineStatus;
		case CardRarity.Event:
			return StsColors.cardTitleOutlineSpecial;
		case CardRarity.Ancient:
			return StsColors.cardTitleOutlineCommon;
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	private void ReloadOverlay()
	{
		if (_cardOverlay != null)
		{
			_overlayContainer.RemoveChildSafely((Node?)(object)_cardOverlay);
			((Node)(object)_cardOverlay).QueueFreeSafely();
			_cardOverlay = null;
		}
		if (Model != null)
		{
			if (Model.Rarity == CardRarity.Ancient)
			{
				((CanvasItem)_frame).Visible = false;
				((CanvasItem)_ancientBorder).Visible = true;
				((CanvasItem)_ancientHighlight).Visible = true;
			}
			AfflictionModel affliction = Model.Affliction;
			if (affliction != null && affliction.HasOverlay)
			{
				_cardOverlay = Model.Affliction.CreateOverlay();
			}
			else if (Model.HasBuiltInOverlay)
			{
				_cardOverlay = Model.CreateOverlay();
			}
			if (_cardOverlay != null)
			{
				_overlayContainer.AddChildSafely((Node?)(object)_cardOverlay);
			}
		}
	}

	private void OnAfflictionChanged()
	{
		ReloadOverlay();
	}

	private void OnEnchantmentChanged()
	{
		UnsubscribeFromEnchantment(_subscribedEnchantment);
		SubscribeToEnchantment(Model?.Enchantment);
		UpdateEnchantmentVisuals();
	}

	private string GetTitleText()
	{
		return ((Label)_titleLabel).Text;
	}

	public void ActivateRewardScreenGlow()
	{
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		if (_model.Rarity == CardRarity.Rare)
		{
			((CanvasItem)_sparkles).Visible = true;
			_rareGlow = NCardRareGlow.Create();
			if (_rareGlow != null)
			{
				((Node)(object)Body).AddChildSafely((Node?)(object)_rareGlow);
				((Node)Body).MoveChild((Node)(object)_rareGlow, 1);
			}
			((CanvasItem)CardHighlight).Modulate = NCardHighlight.gold;
		}
		else if (_model.Rarity == CardRarity.Uncommon)
		{
			_uncommonGlow = NCardUncommonGlow.Create();
			if (_uncommonGlow != null)
			{
				((Node)(object)Body).AddChildSafely((Node?)(object)_uncommonGlow);
				((Node)Body).MoveChild((Node)(object)_uncommonGlow, 1);
			}
			((CanvasItem)CardHighlight).Modulate = NCardHighlight.playableColor;
		}
	}

	public void FlashRelicOnCard(RelicModel relic)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		NRelicFlashVfx nRelicFlashVfx = NRelicFlashVfx.Create(relic);
		((Node)(object)Body).AddChildSafely((Node?)(object)nRelicFlashVfx);
		((Control)nRelicFlashVfx).Scale = Vector2.One * 2f;
		((Control)nRelicFlashVfx).Position = ((Control)this).Size * 0.5f;
		_flashVfx.Add(nRelicFlashVfx);
	}

	public void KillRarityGlow()
	{
		_rareGlow?.Kill();
		_uncommonGlow?.Kill();
	}

	public async Task AnimMultiCardPlay()
	{
		if (GodotObject.IsInstanceValid((GodotObject)(object)this))
		{
			Vector2 scale = ((Control)this).Scale;
			float y = ((Control)this).Position.Y;
			PlayPileTween?.FastForwardToCompletion();
			PlayPileTween = ((Node)this).CreateTween().SetParallel(true);
			PlayPileTween.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("modulate"), Variant.op_Implicit(StsColors.transparentBlack), 0.2);
			PlayPileTween.Chain();
			PlayPileTween.TweenInterval((SaveManager.Instance.PrefsSave.FastMode == FastModeType.Fast) ? 0.1 : 0.2);
			PlayPileTween.Chain();
			PlayPileTween.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("modulate"), Variant.op_Implicit(Colors.White), 0.2).SetEase((EaseType)1).SetTrans((TransitionType)7);
			PlayPileTween.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("scale"), Variant.op_Implicit(scale), 0.25).SetEase((EaseType)1).SetTrans((TransitionType)7)
				.From(Variant.op_Implicit(scale * 0.5f));
			PlayPileTween.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("position:y"), Variant.op_Implicit(y), 0.25).SetEase((EaseType)1).SetTrans((TransitionType)10)
				.From(Variant.op_Implicit(y + 250f));
			await Cmd.CustomScaledWait(0.4f, 0.5f);
		}
	}

	public void AnimCardToPlayPile()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		Vector2 targetPosition = PileType.Play.GetTargetPosition(this);
		PlayPileTween?.FastForwardToCompletion();
		PlayPileTween = ((Node)this).CreateTween().SetParallel(true);
		PlayPileTween.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("position"), Variant.op_Implicit(targetPosition), 0.2).SetEase((EaseType)1).SetTrans((TransitionType)7);
		PlayPileTween.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("scale"), Variant.op_Implicit(Vector2.One * 0.8f), 0.2).SetEase((EaseType)1).SetTrans((TransitionType)7);
	}

	public void OnReturnedFromPool()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		if (((Node)this).IsNodeReady())
		{
			((Control)this).Position = Vector2.Zero;
			((Control)this).Rotation = 0f;
			((Control)this).Scale = Vector2.One;
			((CanvasItem)this).Modulate = Colors.White;
			((CanvasItem)this).Visible = true;
			((CanvasItem)Body).Visible = true;
			((CanvasItem)Body).Modulate = Colors.White;
			Body.Scale = Vector2.One;
			_visibility = ModelVisibility.Visible;
			((CanvasItem)CardHighlight).Modulate = NCardHighlight.playableColor;
			CardHighlight.AnimHideInstantly();
			((CanvasItem)_sparkles).Visible = false;
			((CanvasItem)_enchantmentTab).Visible = false;
			((CanvasItem)_enchantmentVfxOverride).Visible = false;
			_model = null;
			_previewTarget = null;
			_pretendCardCanBePlayed = false;
			_forceUnpoweredPreview = false;
			((CanvasItem)_portrait).Material = null;
			((CanvasItem)_ancientPortrait).Material = null;
			((CanvasItem)_portraitCanvasGroup).Material = null;
			DisplayingPile = PileType.None;
			this.ModelChanged = null;
		}
	}

	public void OnFreedToPool()
	{
		((Node)(object)_rareGlow)?.QueueFreeSafely();
		_rareGlow = null;
		((Node)(object)_uncommonGlow)?.QueueFreeSafely();
		_uncommonGlow = null;
		((Node)(object)_cardOverlay)?.QueueFreeSafely();
		_cardOverlay = null;
		_portrait.Texture = null;
		_enchantmentVfxOverride.Texture = null;
		foreach (NRelicFlashVfx item in _flashVfx)
		{
			if (((Node?)(object)item).IsValid())
			{
				((Node)(object)item).QueueFreeSafely();
			}
		}
		_flashVfx.Clear();
		Tween? playPileTween = PlayPileTween;
		if (playPileTween != null)
		{
			playPileTween.Kill();
		}
		PlayPileTween = null;
		Tween? randomizeCostTween = RandomizeCostTween;
		if (randomizeCostTween != null)
		{
			randomizeCostTween.Kill();
		}
		RandomizeCostTween = null;
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
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0241: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0295: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_030c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0317: Unknown result type (might be due to invalid IL or missing references)
		//IL_033d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0360: Unknown result type (might be due to invalid IL or missing references)
		//IL_036b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0391: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0408: Unknown result type (might be due to invalid IL or missing references)
		//IL_0413: Unknown result type (might be due to invalid IL or missing references)
		//IL_0439: Unknown result type (might be due to invalid IL or missing references)
		//IL_045c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0467: Unknown result type (might be due to invalid IL or missing references)
		//IL_048d: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0506: Unknown result type (might be due to invalid IL or missing references)
		//IL_0527: Unknown result type (might be due to invalid IL or missing references)
		//IL_0549: Unknown result type (might be due to invalid IL or missing references)
		//IL_0554: Unknown result type (might be due to invalid IL or missing references)
		//IL_057b: Unknown result type (might be due to invalid IL or missing references)
		//IL_059f: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0613: Unknown result type (might be due to invalid IL or missing references)
		//IL_061c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0642: Unknown result type (might be due to invalid IL or missing references)
		//IL_064b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0671: Unknown result type (might be due to invalid IL or missing references)
		//IL_067a: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_06cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0708: Unknown result type (might be due to invalid IL or missing references)
		//IL_072e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0737: Unknown result type (might be due to invalid IL or missing references)
		//IL_075d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0766: Unknown result type (might be due to invalid IL or missing references)
		//IL_078c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0795: Unknown result type (might be due to invalid IL or missing references)
		//IL_07bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0819: Unknown result type (might be due to invalid IL or missing references)
		//IL_0822: Unknown result type (might be due to invalid IL or missing references)
		//IL_0848: Unknown result type (might be due to invalid IL or missing references)
		//IL_0851: Unknown result type (might be due to invalid IL or missing references)
		//IL_0877: Unknown result type (might be due to invalid IL or missing references)
		//IL_0880: Unknown result type (might be due to invalid IL or missing references)
		//IL_08a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_08af: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(35);
		list.Add(new MethodInfo(MethodName.OnInstantiated, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._EnterTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._ExitTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.InitPool, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)33, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.GetCurrentSize, new PropertyInfo((Type)5, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UpdateVisuals, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("pileType"), (PropertyHint)0, "", (PropertyUsageFlags)6, false),
			new PropertyInfo((Type)2, StringName.op_Implicit("previewMode"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ShowUpgradePreview, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UpdateEnchantmentVisuals, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnEnchantmentStatusChanged, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetEnchantmentStatus, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("status"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UpdateEnergyCostVisuals, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("pileType"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetPretendCardCanBePlayed, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)1, StringName.op_Implicit("pretendCardCanBePlayed"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetForceUnpoweredPreview, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)1, StringName.op_Implicit("forceUnpoweredPreview"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UpdateEnergyCostColor, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("pileType"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UpdateStarCostVisuals, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("pileType"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UpdateStarCostText, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("cost"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UpdateStarCostColor, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("pileType"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.GetCostTextColorInHand, new PropertyInfo((Type)20, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)33, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("costColor"), (PropertyHint)0, "", (PropertyUsageFlags)6, false),
			new PropertyInfo((Type)1, StringName.op_Implicit("pretendCardCanBePlayed"), (PropertyHint)0, "", (PropertyUsageFlags)6, false),
			new PropertyInfo((Type)20, StringName.op_Implicit("defaultColor"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.GetCostOutlineColorInHand, new PropertyInfo((Type)20, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)33, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("costColor"), (PropertyHint)0, "", (PropertyUsageFlags)6, false),
			new PropertyInfo((Type)1, StringName.op_Implicit("pretendCardCanBePlayed"), (PropertyHint)0, "", (PropertyUsageFlags)6, false),
			new PropertyInfo((Type)20, StringName.op_Implicit("defaultColor"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.PlayRandomizeCostAnim, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.Reload, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UpdateTypePlaque, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UpdateTypePlaqueSizeAndPosition, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UpdateTitleLabel, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.GetTitleLabelOutlineColor, new PropertyInfo((Type)20, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ReloadOverlay, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnAfflictionChanged, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnEnchantmentChanged, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.GetTitleText, new PropertyInfo((Type)4, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ActivateRewardScreenGlow, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.KillRarityGlow, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AnimCardToPlayPile, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnReturnedFromPool, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnFreedToPool, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		//IL_028a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0323: Unknown result type (might be due to invalid IL or missing references)
		//IL_0364: Unknown result type (might be due to invalid IL or missing references)
		//IL_0369: Unknown result type (might be due to invalid IL or missing references)
		//IL_036e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0372: Unknown result type (might be due to invalid IL or missing references)
		//IL_0377: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0413: Unknown result type (might be due to invalid IL or missing references)
		//IL_0438: Unknown result type (might be due to invalid IL or missing references)
		//IL_045d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0482: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_04af: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_051d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0546: Unknown result type (might be due to invalid IL or missing references)
		//IL_054b: Unknown result type (might be due to invalid IL or missing references)
		//IL_056f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0594: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_060d: Unknown result type (might be due to invalid IL or missing references)
		//IL_05de: Unknown result type (might be due to invalid IL or missing references)
		//IL_0603: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.OnInstantiated && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnInstantiated();
			ret = default(godot_variant);
			return true;
		}
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
		if ((ref method) == MethodName.InitPool && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			InitPool();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.GetCurrentSize && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			Vector2 currentSize = GetCurrentSize();
			ret = VariantUtils.CreateFrom<Vector2>(ref currentSize);
			return true;
		}
		if ((ref method) == MethodName.UpdateVisuals && ((NativeVariantPtrArgs)(ref args)).Count == 2)
		{
			UpdateVisuals(VariantUtils.ConvertTo<PileType>(ref ((NativeVariantPtrArgs)(ref args))[0]), VariantUtils.ConvertTo<CardPreviewMode>(ref ((NativeVariantPtrArgs)(ref args))[1]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.ShowUpgradePreview && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			ShowUpgradePreview();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.UpdateEnchantmentVisuals && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			UpdateEnchantmentVisuals();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnEnchantmentStatusChanged && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnEnchantmentStatusChanged();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetEnchantmentStatus && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			SetEnchantmentStatus(VariantUtils.ConvertTo<EnchantmentStatus>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.UpdateEnergyCostVisuals && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			UpdateEnergyCostVisuals(VariantUtils.ConvertTo<PileType>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetPretendCardCanBePlayed && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			SetPretendCardCanBePlayed(VariantUtils.ConvertTo<bool>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetForceUnpoweredPreview && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			SetForceUnpoweredPreview(VariantUtils.ConvertTo<bool>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.UpdateEnergyCostColor && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			UpdateEnergyCostColor(VariantUtils.ConvertTo<PileType>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.UpdateStarCostVisuals && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			UpdateStarCostVisuals(VariantUtils.ConvertTo<PileType>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.UpdateStarCostText && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			UpdateStarCostText(VariantUtils.ConvertTo<int>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.UpdateStarCostColor && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			UpdateStarCostColor(VariantUtils.ConvertTo<PileType>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.GetCostTextColorInHand && ((NativeVariantPtrArgs)(ref args)).Count == 3)
		{
			Color costTextColorInHand = GetCostTextColorInHand(VariantUtils.ConvertTo<CardCostColor>(ref ((NativeVariantPtrArgs)(ref args))[0]), VariantUtils.ConvertTo<bool>(ref ((NativeVariantPtrArgs)(ref args))[1]), VariantUtils.ConvertTo<Color>(ref ((NativeVariantPtrArgs)(ref args))[2]));
			ret = VariantUtils.CreateFrom<Color>(ref costTextColorInHand);
			return true;
		}
		if ((ref method) == MethodName.GetCostOutlineColorInHand && ((NativeVariantPtrArgs)(ref args)).Count == 3)
		{
			Color costOutlineColorInHand = GetCostOutlineColorInHand(VariantUtils.ConvertTo<CardCostColor>(ref ((NativeVariantPtrArgs)(ref args))[0]), VariantUtils.ConvertTo<bool>(ref ((NativeVariantPtrArgs)(ref args))[1]), VariantUtils.ConvertTo<Color>(ref ((NativeVariantPtrArgs)(ref args))[2]));
			ret = VariantUtils.CreateFrom<Color>(ref costOutlineColorInHand);
			return true;
		}
		if ((ref method) == MethodName.PlayRandomizeCostAnim && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			PlayRandomizeCostAnim();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.Reload && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			Reload();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.UpdateTypePlaque && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			UpdateTypePlaque();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.UpdateTypePlaqueSizeAndPosition && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			UpdateTypePlaqueSizeAndPosition();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.UpdateTitleLabel && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			UpdateTitleLabel();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.GetTitleLabelOutlineColor && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			Color titleLabelOutlineColor = GetTitleLabelOutlineColor();
			ret = VariantUtils.CreateFrom<Color>(ref titleLabelOutlineColor);
			return true;
		}
		if ((ref method) == MethodName.ReloadOverlay && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			ReloadOverlay();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnAfflictionChanged && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnAfflictionChanged();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnEnchantmentChanged && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnEnchantmentChanged();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.GetTitleText && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			string titleText = GetTitleText();
			ret = VariantUtils.CreateFrom<string>(ref titleText);
			return true;
		}
		if ((ref method) == MethodName.ActivateRewardScreenGlow && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			ActivateRewardScreenGlow();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.KillRarityGlow && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			KillRarityGlow();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.AnimCardToPlayPile && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			AnimCardToPlayPile();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnReturnedFromPool && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnReturnedFromPool();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnFreedToPool && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnFreedToPool();
			ret = default(godot_variant);
			return true;
		}
		return ((Control)this).InvokeGodotClassMethod(ref method, args, ref ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static bool InvokeGodotClassStaticMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.InitPool && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			InitPool();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.GetCostTextColorInHand && ((NativeVariantPtrArgs)(ref args)).Count == 3)
		{
			Color costTextColorInHand = GetCostTextColorInHand(VariantUtils.ConvertTo<CardCostColor>(ref ((NativeVariantPtrArgs)(ref args))[0]), VariantUtils.ConvertTo<bool>(ref ((NativeVariantPtrArgs)(ref args))[1]), VariantUtils.ConvertTo<Color>(ref ((NativeVariantPtrArgs)(ref args))[2]));
			ret = VariantUtils.CreateFrom<Color>(ref costTextColorInHand);
			return true;
		}
		if ((ref method) == MethodName.GetCostOutlineColorInHand && ((NativeVariantPtrArgs)(ref args)).Count == 3)
		{
			Color costOutlineColorInHand = GetCostOutlineColorInHand(VariantUtils.ConvertTo<CardCostColor>(ref ((NativeVariantPtrArgs)(ref args))[0]), VariantUtils.ConvertTo<bool>(ref ((NativeVariantPtrArgs)(ref args))[1]), VariantUtils.ConvertTo<Color>(ref ((NativeVariantPtrArgs)(ref args))[2]));
			ret = VariantUtils.CreateFrom<Color>(ref costOutlineColorInHand);
			return true;
		}
		ret = default(godot_variant);
		return false;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if ((ref method) == MethodName.OnInstantiated)
		{
			return true;
		}
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
		if ((ref method) == MethodName.InitPool)
		{
			return true;
		}
		if ((ref method) == MethodName.GetCurrentSize)
		{
			return true;
		}
		if ((ref method) == MethodName.UpdateVisuals)
		{
			return true;
		}
		if ((ref method) == MethodName.ShowUpgradePreview)
		{
			return true;
		}
		if ((ref method) == MethodName.UpdateEnchantmentVisuals)
		{
			return true;
		}
		if ((ref method) == MethodName.OnEnchantmentStatusChanged)
		{
			return true;
		}
		if ((ref method) == MethodName.SetEnchantmentStatus)
		{
			return true;
		}
		if ((ref method) == MethodName.UpdateEnergyCostVisuals)
		{
			return true;
		}
		if ((ref method) == MethodName.SetPretendCardCanBePlayed)
		{
			return true;
		}
		if ((ref method) == MethodName.SetForceUnpoweredPreview)
		{
			return true;
		}
		if ((ref method) == MethodName.UpdateEnergyCostColor)
		{
			return true;
		}
		if ((ref method) == MethodName.UpdateStarCostVisuals)
		{
			return true;
		}
		if ((ref method) == MethodName.UpdateStarCostText)
		{
			return true;
		}
		if ((ref method) == MethodName.UpdateStarCostColor)
		{
			return true;
		}
		if ((ref method) == MethodName.GetCostTextColorInHand)
		{
			return true;
		}
		if ((ref method) == MethodName.GetCostOutlineColorInHand)
		{
			return true;
		}
		if ((ref method) == MethodName.PlayRandomizeCostAnim)
		{
			return true;
		}
		if ((ref method) == MethodName.Reload)
		{
			return true;
		}
		if ((ref method) == MethodName.UpdateTypePlaque)
		{
			return true;
		}
		if ((ref method) == MethodName.UpdateTypePlaqueSizeAndPosition)
		{
			return true;
		}
		if ((ref method) == MethodName.UpdateTitleLabel)
		{
			return true;
		}
		if ((ref method) == MethodName.GetTitleLabelOutlineColor)
		{
			return true;
		}
		if ((ref method) == MethodName.ReloadOverlay)
		{
			return true;
		}
		if ((ref method) == MethodName.OnAfflictionChanged)
		{
			return true;
		}
		if ((ref method) == MethodName.OnEnchantmentChanged)
		{
			return true;
		}
		if ((ref method) == MethodName.GetTitleText)
		{
			return true;
		}
		if ((ref method) == MethodName.ActivateRewardScreenGlow)
		{
			return true;
		}
		if ((ref method) == MethodName.KillRarityGlow)
		{
			return true;
		}
		if ((ref method) == MethodName.AnimCardToPlayPile)
		{
			return true;
		}
		if ((ref method) == MethodName.OnReturnedFromPool)
		{
			return true;
		}
		if ((ref method) == MethodName.OnFreedToPool)
		{
			return true;
		}
		return ((Control)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		//IL_03db: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e0: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName.CardHighlight)
		{
			CardHighlight = VariantUtils.ConvertTo<NCardHighlight>(ref value);
			return true;
		}
		if ((ref name) == PropertyName.Body)
		{
			Body = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName.Visibility)
		{
			Visibility = VariantUtils.ConvertTo<ModelVisibility>(ref value);
			return true;
		}
		if ((ref name) == PropertyName.PlayPileTween)
		{
			PlayPileTween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		if ((ref name) == PropertyName.RandomizeCostTween)
		{
			RandomizeCostTween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		if ((ref name) == PropertyName.DisplayingPile)
		{
			DisplayingPile = VariantUtils.ConvertTo<PileType>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._titleLabel)
		{
			_titleLabel = VariantUtils.ConvertTo<MegaLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._descriptionLabel)
		{
			_descriptionLabel = VariantUtils.ConvertTo<MegaRichTextLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._ancientPortrait)
		{
			_ancientPortrait = VariantUtils.ConvertTo<TextureRect>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._portrait)
		{
			_portrait = VariantUtils.ConvertTo<TextureRect>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._frame)
		{
			_frame = VariantUtils.ConvertTo<TextureRect>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._ancientBorder)
		{
			_ancientBorder = VariantUtils.ConvertTo<TextureRect>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._ancientBanner)
		{
			_ancientBanner = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._ancientTextBg)
		{
			_ancientTextBg = VariantUtils.ConvertTo<TextureRect>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._ancientHighlight)
		{
			_ancientHighlight = VariantUtils.ConvertTo<TextureRect>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._portraitBorder)
		{
			_portraitBorder = VariantUtils.ConvertTo<TextureRect>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._banner)
		{
			_banner = VariantUtils.ConvertTo<TextureRect>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._lock)
		{
			_lock = VariantUtils.ConvertTo<TextureRect>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._typePlaque)
		{
			_typePlaque = VariantUtils.ConvertTo<NinePatchRect>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._typeLabel)
		{
			_typeLabel = VariantUtils.ConvertTo<MegaLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._portraitCanvasGroup)
		{
			_portraitCanvasGroup = VariantUtils.ConvertTo<CanvasGroup>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._rareGlow)
		{
			_rareGlow = VariantUtils.ConvertTo<NCardRareGlow>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._uncommonGlow)
		{
			_uncommonGlow = VariantUtils.ConvertTo<NCardUncommonGlow>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._sparkles)
		{
			_sparkles = VariantUtils.ConvertTo<GpuParticles2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._energyIcon)
		{
			_energyIcon = VariantUtils.ConvertTo<TextureRect>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._energyLabel)
		{
			_energyLabel = VariantUtils.ConvertTo<MegaLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._unplayableEnergyIcon)
		{
			_unplayableEnergyIcon = VariantUtils.ConvertTo<TextureRect>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._starIcon)
		{
			_starIcon = VariantUtils.ConvertTo<TextureRect>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._starLabel)
		{
			_starLabel = VariantUtils.ConvertTo<MegaLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._unplayableStarIcon)
		{
			_unplayableStarIcon = VariantUtils.ConvertTo<TextureRect>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._overlayContainer)
		{
			_overlayContainer = VariantUtils.ConvertTo<Node>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._cardOverlay)
		{
			_cardOverlay = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._enchantmentTab)
		{
			_enchantmentTab = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._enchantmentVfxOverride)
		{
			_enchantmentVfxOverride = VariantUtils.ConvertTo<TextureRect>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._enchantmentIcon)
		{
			_enchantmentIcon = VariantUtils.ConvertTo<TextureRect>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._enchantmentLabel)
		{
			_enchantmentLabel = VariantUtils.ConvertTo<MegaLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._defaultEnchantmentPosition)
		{
			_defaultEnchantmentPosition = VariantUtils.ConvertTo<Vector2>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._pretendCardCanBePlayed)
		{
			_pretendCardCanBePlayed = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._forceUnpoweredPreview)
		{
			_forceUnpoweredPreview = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._portraitBlurMaterial)
		{
			_portraitBlurMaterial = VariantUtils.ConvertTo<Material>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._canvasGroupMaskBlurMaterial)
		{
			_canvasGroupMaskBlurMaterial = VariantUtils.ConvertTo<Material>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._canvasGroupBlurMaterial)
		{
			_canvasGroupBlurMaterial = VariantUtils.ConvertTo<Material>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._canvasGroupMaskMaterial)
		{
			_canvasGroupMaskMaterial = VariantUtils.ConvertTo<Material>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._visibility)
		{
			_visibility = VariantUtils.ConvertTo<ModelVisibility>(ref value);
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
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_028e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0293: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_030e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0313: Unknown result type (might be due to invalid IL or missing references)
		//IL_032e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0333: Unknown result type (might be due to invalid IL or missing references)
		//IL_034e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0353: Unknown result type (might be due to invalid IL or missing references)
		//IL_036e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0373: Unknown result type (might be due to invalid IL or missing references)
		//IL_038e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0393: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_040e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0413: Unknown result type (might be due to invalid IL or missing references)
		//IL_042e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0433: Unknown result type (might be due to invalid IL or missing references)
		//IL_044e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0453: Unknown result type (might be due to invalid IL or missing references)
		//IL_046e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0473: Unknown result type (might be due to invalid IL or missing references)
		//IL_048e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0493: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_050e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0513: Unknown result type (might be due to invalid IL or missing references)
		//IL_052e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0533: Unknown result type (might be due to invalid IL or missing references)
		//IL_054e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0553: Unknown result type (might be due to invalid IL or missing references)
		//IL_056e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0573: Unknown result type (might be due to invalid IL or missing references)
		//IL_058e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0593: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d3: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName.CardHighlight)
		{
			NCardHighlight cardHighlight = CardHighlight;
			value = VariantUtils.CreateFrom<NCardHighlight>(ref cardHighlight);
			return true;
		}
		if ((ref name) == PropertyName.Body)
		{
			Control body = Body;
			value = VariantUtils.CreateFrom<Control>(ref body);
			return true;
		}
		if ((ref name) == PropertyName.Visibility)
		{
			ModelVisibility visibility = Visibility;
			value = VariantUtils.CreateFrom<ModelVisibility>(ref visibility);
			return true;
		}
		if ((ref name) == PropertyName.PlayPileTween)
		{
			Tween playPileTween = PlayPileTween;
			value = VariantUtils.CreateFrom<Tween>(ref playPileTween);
			return true;
		}
		if ((ref name) == PropertyName.RandomizeCostTween)
		{
			Tween playPileTween = RandomizeCostTween;
			value = VariantUtils.CreateFrom<Tween>(ref playPileTween);
			return true;
		}
		if ((ref name) == PropertyName.DisplayingPile)
		{
			PileType displayingPile = DisplayingPile;
			value = VariantUtils.CreateFrom<PileType>(ref displayingPile);
			return true;
		}
		if ((ref name) == PropertyName.EnchantmentTab)
		{
			Control body = EnchantmentTab;
			value = VariantUtils.CreateFrom<Control>(ref body);
			return true;
		}
		if ((ref name) == PropertyName.EnchantmentVfxOverride)
		{
			TextureRect enchantmentVfxOverride = EnchantmentVfxOverride;
			value = VariantUtils.CreateFrom<TextureRect>(ref enchantmentVfxOverride);
			return true;
		}
		if ((ref name) == PropertyName._titleLabel)
		{
			value = VariantUtils.CreateFrom<MegaLabel>(ref _titleLabel);
			return true;
		}
		if ((ref name) == PropertyName._descriptionLabel)
		{
			value = VariantUtils.CreateFrom<MegaRichTextLabel>(ref _descriptionLabel);
			return true;
		}
		if ((ref name) == PropertyName._ancientPortrait)
		{
			value = VariantUtils.CreateFrom<TextureRect>(ref _ancientPortrait);
			return true;
		}
		if ((ref name) == PropertyName._portrait)
		{
			value = VariantUtils.CreateFrom<TextureRect>(ref _portrait);
			return true;
		}
		if ((ref name) == PropertyName._frame)
		{
			value = VariantUtils.CreateFrom<TextureRect>(ref _frame);
			return true;
		}
		if ((ref name) == PropertyName._ancientBorder)
		{
			value = VariantUtils.CreateFrom<TextureRect>(ref _ancientBorder);
			return true;
		}
		if ((ref name) == PropertyName._ancientBanner)
		{
			value = VariantUtils.CreateFrom<Control>(ref _ancientBanner);
			return true;
		}
		if ((ref name) == PropertyName._ancientTextBg)
		{
			value = VariantUtils.CreateFrom<TextureRect>(ref _ancientTextBg);
			return true;
		}
		if ((ref name) == PropertyName._ancientHighlight)
		{
			value = VariantUtils.CreateFrom<TextureRect>(ref _ancientHighlight);
			return true;
		}
		if ((ref name) == PropertyName._portraitBorder)
		{
			value = VariantUtils.CreateFrom<TextureRect>(ref _portraitBorder);
			return true;
		}
		if ((ref name) == PropertyName._banner)
		{
			value = VariantUtils.CreateFrom<TextureRect>(ref _banner);
			return true;
		}
		if ((ref name) == PropertyName._lock)
		{
			value = VariantUtils.CreateFrom<TextureRect>(ref _lock);
			return true;
		}
		if ((ref name) == PropertyName._typePlaque)
		{
			value = VariantUtils.CreateFrom<NinePatchRect>(ref _typePlaque);
			return true;
		}
		if ((ref name) == PropertyName._typeLabel)
		{
			value = VariantUtils.CreateFrom<MegaLabel>(ref _typeLabel);
			return true;
		}
		if ((ref name) == PropertyName._portraitCanvasGroup)
		{
			value = VariantUtils.CreateFrom<CanvasGroup>(ref _portraitCanvasGroup);
			return true;
		}
		if ((ref name) == PropertyName._rareGlow)
		{
			value = VariantUtils.CreateFrom<NCardRareGlow>(ref _rareGlow);
			return true;
		}
		if ((ref name) == PropertyName._uncommonGlow)
		{
			value = VariantUtils.CreateFrom<NCardUncommonGlow>(ref _uncommonGlow);
			return true;
		}
		if ((ref name) == PropertyName._sparkles)
		{
			value = VariantUtils.CreateFrom<GpuParticles2D>(ref _sparkles);
			return true;
		}
		if ((ref name) == PropertyName._energyIcon)
		{
			value = VariantUtils.CreateFrom<TextureRect>(ref _energyIcon);
			return true;
		}
		if ((ref name) == PropertyName._energyLabel)
		{
			value = VariantUtils.CreateFrom<MegaLabel>(ref _energyLabel);
			return true;
		}
		if ((ref name) == PropertyName._unplayableEnergyIcon)
		{
			value = VariantUtils.CreateFrom<TextureRect>(ref _unplayableEnergyIcon);
			return true;
		}
		if ((ref name) == PropertyName._starIcon)
		{
			value = VariantUtils.CreateFrom<TextureRect>(ref _starIcon);
			return true;
		}
		if ((ref name) == PropertyName._starLabel)
		{
			value = VariantUtils.CreateFrom<MegaLabel>(ref _starLabel);
			return true;
		}
		if ((ref name) == PropertyName._unplayableStarIcon)
		{
			value = VariantUtils.CreateFrom<TextureRect>(ref _unplayableStarIcon);
			return true;
		}
		if ((ref name) == PropertyName._overlayContainer)
		{
			value = VariantUtils.CreateFrom<Node>(ref _overlayContainer);
			return true;
		}
		if ((ref name) == PropertyName._cardOverlay)
		{
			value = VariantUtils.CreateFrom<Control>(ref _cardOverlay);
			return true;
		}
		if ((ref name) == PropertyName._enchantmentTab)
		{
			value = VariantUtils.CreateFrom<Control>(ref _enchantmentTab);
			return true;
		}
		if ((ref name) == PropertyName._enchantmentVfxOverride)
		{
			value = VariantUtils.CreateFrom<TextureRect>(ref _enchantmentVfxOverride);
			return true;
		}
		if ((ref name) == PropertyName._enchantmentIcon)
		{
			value = VariantUtils.CreateFrom<TextureRect>(ref _enchantmentIcon);
			return true;
		}
		if ((ref name) == PropertyName._enchantmentLabel)
		{
			value = VariantUtils.CreateFrom<MegaLabel>(ref _enchantmentLabel);
			return true;
		}
		if ((ref name) == PropertyName._defaultEnchantmentPosition)
		{
			value = VariantUtils.CreateFrom<Vector2>(ref _defaultEnchantmentPosition);
			return true;
		}
		if ((ref name) == PropertyName._pretendCardCanBePlayed)
		{
			value = VariantUtils.CreateFrom<bool>(ref _pretendCardCanBePlayed);
			return true;
		}
		if ((ref name) == PropertyName._forceUnpoweredPreview)
		{
			value = VariantUtils.CreateFrom<bool>(ref _forceUnpoweredPreview);
			return true;
		}
		if ((ref name) == PropertyName._portraitBlurMaterial)
		{
			value = VariantUtils.CreateFrom<Material>(ref _portraitBlurMaterial);
			return true;
		}
		if ((ref name) == PropertyName._canvasGroupMaskBlurMaterial)
		{
			value = VariantUtils.CreateFrom<Material>(ref _canvasGroupMaskBlurMaterial);
			return true;
		}
		if ((ref name) == PropertyName._canvasGroupBlurMaterial)
		{
			value = VariantUtils.CreateFrom<Material>(ref _canvasGroupBlurMaterial);
			return true;
		}
		if ((ref name) == PropertyName._canvasGroupMaskMaterial)
		{
			value = VariantUtils.CreateFrom<Material>(ref _canvasGroupMaskMaterial);
			return true;
		}
		if ((ref name) == PropertyName._visibility)
		{
			value = VariantUtils.CreateFrom<ModelVisibility>(ref _visibility);
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
		//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0314: Unknown result type (might be due to invalid IL or missing references)
		//IL_0335: Unknown result type (might be due to invalid IL or missing references)
		//IL_0356: Unknown result type (might be due to invalid IL or missing references)
		//IL_0377: Unknown result type (might be due to invalid IL or missing references)
		//IL_0398: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03da: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_041b: Unknown result type (might be due to invalid IL or missing references)
		//IL_043c: Unknown result type (might be due to invalid IL or missing references)
		//IL_045c: Unknown result type (might be due to invalid IL or missing references)
		//IL_047c: Unknown result type (might be due to invalid IL or missing references)
		//IL_049d: Unknown result type (might be due to invalid IL or missing references)
		//IL_04be: Unknown result type (might be due to invalid IL or missing references)
		//IL_04df: Unknown result type (might be due to invalid IL or missing references)
		//IL_0500: Unknown result type (might be due to invalid IL or missing references)
		//IL_0520: Unknown result type (might be due to invalid IL or missing references)
		//IL_0540: Unknown result type (might be due to invalid IL or missing references)
		//IL_0561: Unknown result type (might be due to invalid IL or missing references)
		//IL_0582: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e4: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._titleLabel, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._descriptionLabel, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._ancientPortrait, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._portrait, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._frame, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._ancientBorder, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._ancientBanner, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._ancientTextBg, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._ancientHighlight, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._portraitBorder, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._banner, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._lock, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._typePlaque, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._typeLabel, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._portraitCanvasGroup, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._rareGlow, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._uncommonGlow, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._sparkles, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._energyIcon, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._energyLabel, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._unplayableEnergyIcon, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._starIcon, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._starLabel, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._unplayableStarIcon, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._overlayContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._cardOverlay, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._enchantmentTab, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._enchantmentVfxOverride, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._enchantmentIcon, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._enchantmentLabel, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)5, PropertyName._defaultEnchantmentPosition, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.CardHighlight, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.Body, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._pretendCardCanBePlayed, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._forceUnpoweredPreview, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._portraitBlurMaterial, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._canvasGroupMaskBlurMaterial, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._canvasGroupBlurMaterial, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._canvasGroupMaskMaterial, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)2, PropertyName._visibility, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)2, PropertyName.Visibility, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.PlayPileTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.RandomizeCostTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)2, PropertyName.DisplayingPile, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.EnchantmentTab, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.EnchantmentVfxOverride, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		//IL_028e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0312: Unknown result type (might be due to invalid IL or missing references)
		//IL_0328: Unknown result type (might be due to invalid IL or missing references)
		//IL_033e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0354: Unknown result type (might be due to invalid IL or missing references)
		//IL_036a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0380: Unknown result type (might be due to invalid IL or missing references)
		//IL_0396: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d8: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		StringName cardHighlight = PropertyName.CardHighlight;
		NCardHighlight cardHighlight2 = CardHighlight;
		info.AddProperty(cardHighlight, Variant.From<NCardHighlight>(ref cardHighlight2));
		StringName body = PropertyName.Body;
		Control body2 = Body;
		info.AddProperty(body, Variant.From<Control>(ref body2));
		StringName visibility = PropertyName.Visibility;
		ModelVisibility visibility2 = Visibility;
		info.AddProperty(visibility, Variant.From<ModelVisibility>(ref visibility2));
		StringName playPileTween = PropertyName.PlayPileTween;
		Tween playPileTween2 = PlayPileTween;
		info.AddProperty(playPileTween, Variant.From<Tween>(ref playPileTween2));
		StringName randomizeCostTween = PropertyName.RandomizeCostTween;
		playPileTween2 = RandomizeCostTween;
		info.AddProperty(randomizeCostTween, Variant.From<Tween>(ref playPileTween2));
		StringName displayingPile = PropertyName.DisplayingPile;
		PileType displayingPile2 = DisplayingPile;
		info.AddProperty(displayingPile, Variant.From<PileType>(ref displayingPile2));
		info.AddProperty(PropertyName._titleLabel, Variant.From<MegaLabel>(ref _titleLabel));
		info.AddProperty(PropertyName._descriptionLabel, Variant.From<MegaRichTextLabel>(ref _descriptionLabel));
		info.AddProperty(PropertyName._ancientPortrait, Variant.From<TextureRect>(ref _ancientPortrait));
		info.AddProperty(PropertyName._portrait, Variant.From<TextureRect>(ref _portrait));
		info.AddProperty(PropertyName._frame, Variant.From<TextureRect>(ref _frame));
		info.AddProperty(PropertyName._ancientBorder, Variant.From<TextureRect>(ref _ancientBorder));
		info.AddProperty(PropertyName._ancientBanner, Variant.From<Control>(ref _ancientBanner));
		info.AddProperty(PropertyName._ancientTextBg, Variant.From<TextureRect>(ref _ancientTextBg));
		info.AddProperty(PropertyName._ancientHighlight, Variant.From<TextureRect>(ref _ancientHighlight));
		info.AddProperty(PropertyName._portraitBorder, Variant.From<TextureRect>(ref _portraitBorder));
		info.AddProperty(PropertyName._banner, Variant.From<TextureRect>(ref _banner));
		info.AddProperty(PropertyName._lock, Variant.From<TextureRect>(ref _lock));
		info.AddProperty(PropertyName._typePlaque, Variant.From<NinePatchRect>(ref _typePlaque));
		info.AddProperty(PropertyName._typeLabel, Variant.From<MegaLabel>(ref _typeLabel));
		info.AddProperty(PropertyName._portraitCanvasGroup, Variant.From<CanvasGroup>(ref _portraitCanvasGroup));
		info.AddProperty(PropertyName._rareGlow, Variant.From<NCardRareGlow>(ref _rareGlow));
		info.AddProperty(PropertyName._uncommonGlow, Variant.From<NCardUncommonGlow>(ref _uncommonGlow));
		info.AddProperty(PropertyName._sparkles, Variant.From<GpuParticles2D>(ref _sparkles));
		info.AddProperty(PropertyName._energyIcon, Variant.From<TextureRect>(ref _energyIcon));
		info.AddProperty(PropertyName._energyLabel, Variant.From<MegaLabel>(ref _energyLabel));
		info.AddProperty(PropertyName._unplayableEnergyIcon, Variant.From<TextureRect>(ref _unplayableEnergyIcon));
		info.AddProperty(PropertyName._starIcon, Variant.From<TextureRect>(ref _starIcon));
		info.AddProperty(PropertyName._starLabel, Variant.From<MegaLabel>(ref _starLabel));
		info.AddProperty(PropertyName._unplayableStarIcon, Variant.From<TextureRect>(ref _unplayableStarIcon));
		info.AddProperty(PropertyName._overlayContainer, Variant.From<Node>(ref _overlayContainer));
		info.AddProperty(PropertyName._cardOverlay, Variant.From<Control>(ref _cardOverlay));
		info.AddProperty(PropertyName._enchantmentTab, Variant.From<Control>(ref _enchantmentTab));
		info.AddProperty(PropertyName._enchantmentVfxOverride, Variant.From<TextureRect>(ref _enchantmentVfxOverride));
		info.AddProperty(PropertyName._enchantmentIcon, Variant.From<TextureRect>(ref _enchantmentIcon));
		info.AddProperty(PropertyName._enchantmentLabel, Variant.From<MegaLabel>(ref _enchantmentLabel));
		info.AddProperty(PropertyName._defaultEnchantmentPosition, Variant.From<Vector2>(ref _defaultEnchantmentPosition));
		info.AddProperty(PropertyName._pretendCardCanBePlayed, Variant.From<bool>(ref _pretendCardCanBePlayed));
		info.AddProperty(PropertyName._forceUnpoweredPreview, Variant.From<bool>(ref _forceUnpoweredPreview));
		info.AddProperty(PropertyName._portraitBlurMaterial, Variant.From<Material>(ref _portraitBlurMaterial));
		info.AddProperty(PropertyName._canvasGroupMaskBlurMaterial, Variant.From<Material>(ref _canvasGroupMaskBlurMaterial));
		info.AddProperty(PropertyName._canvasGroupBlurMaterial, Variant.From<Material>(ref _canvasGroupBlurMaterial));
		info.AddProperty(PropertyName._canvasGroupMaskMaterial, Variant.From<Material>(ref _canvasGroupMaskMaterial));
		info.AddProperty(PropertyName._visibility, Variant.From<ModelVisibility>(ref _visibility));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0409: Unknown result type (might be due to invalid IL or missing references)
		//IL_040e: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName.CardHighlight, ref val))
		{
			CardHighlight = ((Variant)(ref val)).As<NCardHighlight>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName.Body, ref val2))
		{
			Body = ((Variant)(ref val2)).As<Control>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName.Visibility, ref val3))
		{
			Visibility = ((Variant)(ref val3)).As<ModelVisibility>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName.PlayPileTween, ref val4))
		{
			PlayPileTween = ((Variant)(ref val4)).As<Tween>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName.RandomizeCostTween, ref val5))
		{
			RandomizeCostTween = ((Variant)(ref val5)).As<Tween>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName.DisplayingPile, ref val6))
		{
			DisplayingPile = ((Variant)(ref val6)).As<PileType>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName._titleLabel, ref val7))
		{
			_titleLabel = ((Variant)(ref val7)).As<MegaLabel>();
		}
		Variant val8 = default(Variant);
		if (info.TryGetProperty(PropertyName._descriptionLabel, ref val8))
		{
			_descriptionLabel = ((Variant)(ref val8)).As<MegaRichTextLabel>();
		}
		Variant val9 = default(Variant);
		if (info.TryGetProperty(PropertyName._ancientPortrait, ref val9))
		{
			_ancientPortrait = ((Variant)(ref val9)).As<TextureRect>();
		}
		Variant val10 = default(Variant);
		if (info.TryGetProperty(PropertyName._portrait, ref val10))
		{
			_portrait = ((Variant)(ref val10)).As<TextureRect>();
		}
		Variant val11 = default(Variant);
		if (info.TryGetProperty(PropertyName._frame, ref val11))
		{
			_frame = ((Variant)(ref val11)).As<TextureRect>();
		}
		Variant val12 = default(Variant);
		if (info.TryGetProperty(PropertyName._ancientBorder, ref val12))
		{
			_ancientBorder = ((Variant)(ref val12)).As<TextureRect>();
		}
		Variant val13 = default(Variant);
		if (info.TryGetProperty(PropertyName._ancientBanner, ref val13))
		{
			_ancientBanner = ((Variant)(ref val13)).As<Control>();
		}
		Variant val14 = default(Variant);
		if (info.TryGetProperty(PropertyName._ancientTextBg, ref val14))
		{
			_ancientTextBg = ((Variant)(ref val14)).As<TextureRect>();
		}
		Variant val15 = default(Variant);
		if (info.TryGetProperty(PropertyName._ancientHighlight, ref val15))
		{
			_ancientHighlight = ((Variant)(ref val15)).As<TextureRect>();
		}
		Variant val16 = default(Variant);
		if (info.TryGetProperty(PropertyName._portraitBorder, ref val16))
		{
			_portraitBorder = ((Variant)(ref val16)).As<TextureRect>();
		}
		Variant val17 = default(Variant);
		if (info.TryGetProperty(PropertyName._banner, ref val17))
		{
			_banner = ((Variant)(ref val17)).As<TextureRect>();
		}
		Variant val18 = default(Variant);
		if (info.TryGetProperty(PropertyName._lock, ref val18))
		{
			_lock = ((Variant)(ref val18)).As<TextureRect>();
		}
		Variant val19 = default(Variant);
		if (info.TryGetProperty(PropertyName._typePlaque, ref val19))
		{
			_typePlaque = ((Variant)(ref val19)).As<NinePatchRect>();
		}
		Variant val20 = default(Variant);
		if (info.TryGetProperty(PropertyName._typeLabel, ref val20))
		{
			_typeLabel = ((Variant)(ref val20)).As<MegaLabel>();
		}
		Variant val21 = default(Variant);
		if (info.TryGetProperty(PropertyName._portraitCanvasGroup, ref val21))
		{
			_portraitCanvasGroup = ((Variant)(ref val21)).As<CanvasGroup>();
		}
		Variant val22 = default(Variant);
		if (info.TryGetProperty(PropertyName._rareGlow, ref val22))
		{
			_rareGlow = ((Variant)(ref val22)).As<NCardRareGlow>();
		}
		Variant val23 = default(Variant);
		if (info.TryGetProperty(PropertyName._uncommonGlow, ref val23))
		{
			_uncommonGlow = ((Variant)(ref val23)).As<NCardUncommonGlow>();
		}
		Variant val24 = default(Variant);
		if (info.TryGetProperty(PropertyName._sparkles, ref val24))
		{
			_sparkles = ((Variant)(ref val24)).As<GpuParticles2D>();
		}
		Variant val25 = default(Variant);
		if (info.TryGetProperty(PropertyName._energyIcon, ref val25))
		{
			_energyIcon = ((Variant)(ref val25)).As<TextureRect>();
		}
		Variant val26 = default(Variant);
		if (info.TryGetProperty(PropertyName._energyLabel, ref val26))
		{
			_energyLabel = ((Variant)(ref val26)).As<MegaLabel>();
		}
		Variant val27 = default(Variant);
		if (info.TryGetProperty(PropertyName._unplayableEnergyIcon, ref val27))
		{
			_unplayableEnergyIcon = ((Variant)(ref val27)).As<TextureRect>();
		}
		Variant val28 = default(Variant);
		if (info.TryGetProperty(PropertyName._starIcon, ref val28))
		{
			_starIcon = ((Variant)(ref val28)).As<TextureRect>();
		}
		Variant val29 = default(Variant);
		if (info.TryGetProperty(PropertyName._starLabel, ref val29))
		{
			_starLabel = ((Variant)(ref val29)).As<MegaLabel>();
		}
		Variant val30 = default(Variant);
		if (info.TryGetProperty(PropertyName._unplayableStarIcon, ref val30))
		{
			_unplayableStarIcon = ((Variant)(ref val30)).As<TextureRect>();
		}
		Variant val31 = default(Variant);
		if (info.TryGetProperty(PropertyName._overlayContainer, ref val31))
		{
			_overlayContainer = ((Variant)(ref val31)).As<Node>();
		}
		Variant val32 = default(Variant);
		if (info.TryGetProperty(PropertyName._cardOverlay, ref val32))
		{
			_cardOverlay = ((Variant)(ref val32)).As<Control>();
		}
		Variant val33 = default(Variant);
		if (info.TryGetProperty(PropertyName._enchantmentTab, ref val33))
		{
			_enchantmentTab = ((Variant)(ref val33)).As<Control>();
		}
		Variant val34 = default(Variant);
		if (info.TryGetProperty(PropertyName._enchantmentVfxOverride, ref val34))
		{
			_enchantmentVfxOverride = ((Variant)(ref val34)).As<TextureRect>();
		}
		Variant val35 = default(Variant);
		if (info.TryGetProperty(PropertyName._enchantmentIcon, ref val35))
		{
			_enchantmentIcon = ((Variant)(ref val35)).As<TextureRect>();
		}
		Variant val36 = default(Variant);
		if (info.TryGetProperty(PropertyName._enchantmentLabel, ref val36))
		{
			_enchantmentLabel = ((Variant)(ref val36)).As<MegaLabel>();
		}
		Variant val37 = default(Variant);
		if (info.TryGetProperty(PropertyName._defaultEnchantmentPosition, ref val37))
		{
			_defaultEnchantmentPosition = ((Variant)(ref val37)).As<Vector2>();
		}
		Variant val38 = default(Variant);
		if (info.TryGetProperty(PropertyName._pretendCardCanBePlayed, ref val38))
		{
			_pretendCardCanBePlayed = ((Variant)(ref val38)).As<bool>();
		}
		Variant val39 = default(Variant);
		if (info.TryGetProperty(PropertyName._forceUnpoweredPreview, ref val39))
		{
			_forceUnpoweredPreview = ((Variant)(ref val39)).As<bool>();
		}
		Variant val40 = default(Variant);
		if (info.TryGetProperty(PropertyName._portraitBlurMaterial, ref val40))
		{
			_portraitBlurMaterial = ((Variant)(ref val40)).As<Material>();
		}
		Variant val41 = default(Variant);
		if (info.TryGetProperty(PropertyName._canvasGroupMaskBlurMaterial, ref val41))
		{
			_canvasGroupMaskBlurMaterial = ((Variant)(ref val41)).As<Material>();
		}
		Variant val42 = default(Variant);
		if (info.TryGetProperty(PropertyName._canvasGroupBlurMaterial, ref val42))
		{
			_canvasGroupBlurMaterial = ((Variant)(ref val42)).As<Material>();
		}
		Variant val43 = default(Variant);
		if (info.TryGetProperty(PropertyName._canvasGroupMaskMaterial, ref val43))
		{
			_canvasGroupMaskMaterial = ((Variant)(ref val43)).As<Material>();
		}
		Variant val44 = default(Variant);
		if (info.TryGetProperty(PropertyName._visibility, ref val44))
		{
			_visibility = ((Variant)(ref val44)).As<ModelVisibility>();
		}
	}
}
