using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.ControllerInput;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.Debug;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Potions;
using MegaCrit.Sts2.Core.Nodes.Relics;
using MegaCrit.Sts2.Core.Nodes.Screens.Capstones;
using MegaCrit.Sts2.Core.Nodes.Screens.RunHistoryScreen;
using MegaCrit.Sts2.Core.Nodes.Screens.ScreenContext;
using MegaCrit.Sts2.Core.Platform;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.Nodes.Multiplayer;

[ScriptPath("res://src/Core/Nodes/Multiplayer/NMultiplayerPlayerExpandedState.cs")]
public class NMultiplayerPlayerExpandedState : Control, ICapstoneScreen, IScreenContext
{
	private class CardGroupKey
	{
		private readonly CardModel _card;

		public CardGroupKey(CardModel card)
		{
			_card = card;
		}

		public override bool Equals(object? obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (obj.GetType() != GetType())
			{
				return false;
			}
			CardGroupKey cardGroupKey = (CardGroupKey)obj;
			if (_card.Id.Equals(cardGroupKey._card.Id) && _card.CurrentUpgradeLevel == cardGroupKey._card.CurrentUpgradeLevel && _card.Enchantment?.Id == cardGroupKey._card.Enchantment?.Id)
			{
				return _card.Enchantment?.Amount == cardGroupKey._card.Enchantment?.Amount;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(_card.Id, _card.CurrentUpgradeLevel, _card.Enchantment?.Id, _card.Enchantment?.Amount);
		}
	}

	public class MethodName : MethodName
	{
		public static readonly StringName AfterCapstoneOpened = StringName.op_Implicit("AfterCapstoneOpened");

		public static readonly StringName AfterCapstoneClosed = StringName.op_Implicit("AfterCapstoneClosed");

		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName _Input = StringName.op_Implicit("_Input");

		public static readonly StringName ShowEntry = StringName.op_Implicit("ShowEntry");

		public static readonly StringName BackButtonPressed = StringName.op_Implicit("BackButtonPressed");

		public static readonly StringName OnRelicClicked = StringName.op_Implicit("OnRelicClicked");

		public static readonly StringName UpdateNavigation = StringName.op_Implicit("UpdateNavigation");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName UseSharedBackstop = StringName.op_Implicit("UseSharedBackstop");

		public static readonly StringName ScreenType = StringName.op_Implicit("ScreenType");

		public static readonly StringName DefaultFocusedControl = StringName.op_Implicit("DefaultFocusedControl");

		public static readonly StringName _playerNameLabel = StringName.op_Implicit("_playerNameLabel");

		public static readonly StringName _cardsHeader = StringName.op_Implicit("_cardsHeader");

		public static readonly StringName _cardContainer = StringName.op_Implicit("_cardContainer");

		public static readonly StringName _backButton = StringName.op_Implicit("_backButton");

		public static readonly StringName _potionsHeader = StringName.op_Implicit("_potionsHeader");

		public static readonly StringName _potionContainer = StringName.op_Implicit("_potionContainer");

		public static readonly StringName _relicsHeader = StringName.op_Implicit("_relicsHeader");

		public static readonly StringName _relicContainer = StringName.op_Implicit("_relicContainer");
	}

	public class SignalName : SignalName
	{
	}

	private static readonly string _scenePath = SceneHelper.GetScenePath("ui/multiplayer_player_expanded_state");

	private MegaRichTextLabel _playerNameLabel;

	private MegaRichTextLabel _cardsHeader;

	private Control _cardContainer;

	private NBackButton _backButton;

	private MegaRichTextLabel _potionsHeader;

	private Control _potionContainer;

	private MegaRichTextLabel _relicsHeader;

	private Control _relicContainer;

	private Player _player;

	private List<CardModel> _cards = new List<CardModel>();

	public bool UseSharedBackstop => true;

	public NetScreenType ScreenType => NetScreenType.RemotePlayerExpandedState;

	public Control? DefaultFocusedControl => null;

	public static NMultiplayerPlayerExpandedState Create(Player player)
	{
		NMultiplayerPlayerExpandedState nMultiplayerPlayerExpandedState = PreloadManager.Cache.GetScene(_scenePath).Instantiate<NMultiplayerPlayerExpandedState>((GenEditState)0);
		nMultiplayerPlayerExpandedState._player = player;
		return nMultiplayerPlayerExpandedState;
	}

	public void AfterCapstoneOpened()
	{
		_backButton.Enable();
		NGlobalUi globalUi = NRun.Instance.GlobalUi;
		globalUi.TopBar.AnimHide();
		globalUi.RelicInventory.AnimHide();
		globalUi.MultiplayerPlayerContainer.AnimHide();
		((Node)globalUi).MoveChild((Node)(object)globalUi.AboveTopBarVfxContainer, ((Node)globalUi.CapstoneContainer).GetIndex(false));
		((Node)globalUi).MoveChild((Node)(object)globalUi.CardPreviewContainer, ((Node)globalUi.CapstoneContainer).GetIndex(false));
		((Node)globalUi).MoveChild((Node)(object)globalUi.MessyCardPreviewContainer, ((Node)globalUi.CapstoneContainer).GetIndex(false));
	}

	public void AfterCapstoneClosed()
	{
		NGlobalUi globalUi = NRun.Instance.GlobalUi;
		globalUi.TopBar.AnimShow();
		globalUi.RelicInventory.AnimShow();
		globalUi.MultiplayerPlayerContainer.AnimShow();
		((Node)globalUi).MoveChild((Node)(object)globalUi.AboveTopBarVfxContainer, ((Node)globalUi.TopBar).GetIndex(false) + 1);
		((Node)globalUi).MoveChild((Node)(object)globalUi.CardPreviewContainer, ((Node)globalUi.TopBar).GetIndex(false) + 1);
		((Node)globalUi).MoveChild((Node)(object)globalUi.MessyCardPreviewContainer, ((Node)globalUi.TopBar).GetIndex(false) + 1);
		((Node)(object)this).QueueFreeSafely();
		_backButton.Disable();
	}

	public override void _Ready()
	{
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_030b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0311: Unknown result type (might be due to invalid IL or missing references)
		//IL_0352: Unknown result type (might be due to invalid IL or missing references)
		//IL_0358: Unknown result type (might be due to invalid IL or missing references)
		_playerNameLabel = ((Node)this).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("%PlayerNameLabel"));
		_cardsHeader = ((Node)this).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("%CardsHeader"));
		_cardContainer = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%CardContainer"));
		_relicsHeader = ((Node)this).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("%RelicsHeader"));
		_relicContainer = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%RelicContainer"));
		_potionsHeader = ((Node)this).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("%PotionsHeader"));
		_potionContainer = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%PotionContainer"));
		_backButton = ((Node)this).GetNode<NBackButton>(NodePath.op_Implicit("%BackButton"));
		LocString locString = new LocString("gameplay_ui", "MULTIPLAYER_EXPANDED_STATE.title");
		locString.Add("PlayerName", PlatformUtil.GetPlayerName(RunManager.Instance.NetService.Platform, _player.NetId));
		locString.Add("Character", _player.Character.Title);
		_playerNameLabel.Text = locString.GetFormattedText();
		LocString locString2 = new LocString("gameplay_ui", "MULTIPLAYER_EXPANDED_STATE.relicHeader");
		_relicsHeader.Text = locString2.GetFormattedText();
		LocString locString3 = new LocString("gameplay_ui", "MULTIPLAYER_EXPANDED_STATE.cardHeader");
		_cardsHeader.Text = locString3.GetFormattedText();
		LocString locString4 = new LocString("gameplay_ui", "MULTIPLAYER_EXPANDED_STATE.potionHeader");
		_potionsHeader.Text = locString4.GetFormattedText();
		foreach (RelicModel relic in _player.Relics)
		{
			NRelicBasicHolder holder = NRelicBasicHolder.Create(relic);
			((Node)(object)_relicContainer).AddChildSafely((Node?)(object)holder);
			((GodotObject)holder).Connect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)delegate
			{
				OnRelicClicked(holder.Relic);
			}), 0u);
			((Control)holder).MouseDefaultCursorShape = (CursorShape)16;
		}
		foreach (PotionModel potion in _player.Potions)
		{
			NPotionHolder nPotionHolder = NPotionHolder.Create(isUsable: false);
			NPotion nPotion = NPotion.Create(potion);
			((Node)(object)_potionContainer).AddChildSafely((Node?)(object)nPotionHolder);
			nPotionHolder.AddPotion(nPotion);
			((Control)nPotion).Position = Vector2.Zero;
		}
		_cards.Clear();
		_cards.AddRange(_player.Deck.Cards);
		foreach (IGrouping<CardGroupKey, CardModel> item in from x in _player.Deck.Cards
			group x by new CardGroupKey(x))
		{
			NDeckHistoryEntry nDeckHistoryEntry = NDeckHistoryEntry.Create(item.First(), item.Count());
			((GodotObject)nDeckHistoryEntry).Connect(NDeckHistoryEntry.SignalName.Clicked, Callable.From<NDeckHistoryEntry>((Action<NDeckHistoryEntry>)ShowEntry), 0u);
			((Node)(object)_cardContainer).AddChildSafely((Node?)(object)nDeckHistoryEntry);
		}
		((GodotObject)_backButton).Connect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)BackButtonPressed), 0u);
		UpdateNavigation();
	}

	public override void _Input(InputEvent inputEvent)
	{
		if (!((CanvasItem)this).IsVisibleInTree() || ((CanvasItem)NDevConsole.Instance).Visible || !NControllerManager.Instance.IsUsingController)
		{
			return;
		}
		Control val = ((Node)this).GetViewport().GuiGetFocusOwner();
		bool flag = ((val is TextEdit || val is LineEdit) ? true : false);
		if (flag || !ActiveScreenContext.Instance.IsCurrent(this))
		{
			return;
		}
		Control val2 = ((Node)this).GetViewport().GuiGetFocusOwner();
		if ((val2 == null || !((Node)this).IsAncestorOf((Node)(object)val2)) && (inputEvent.IsActionPressed(MegaInput.left, false, false) || inputEvent.IsActionPressed(MegaInput.right, false, false) || inputEvent.IsActionPressed(MegaInput.up, false, false) || inputEvent.IsActionPressed(MegaInput.down, false, false) || inputEvent.IsActionPressed(MegaInput.select, false, false)))
		{
			((Control)(object)((Node)_relicContainer).GetChild<NRelicBasicHolder>(0, false)).TryGrabFocus();
			Viewport viewport = ((Node)this).GetViewport();
			if (viewport != null)
			{
				viewport.SetInputAsHandled();
			}
		}
	}

	private void ShowEntry(NDeckHistoryEntry entry)
	{
		NGame.Instance.GetInspectCardScreen().Open(_cards, _cards.IndexOf(entry.Card));
	}

	private void BackButtonPressed(NButton _)
	{
		NCapstoneContainer.Instance.Close();
	}

	private void OnRelicClicked(NRelic node)
	{
		List<RelicModel> list = new List<RelicModel>();
		foreach (NRelicBasicHolder item in ((IEnumerable)((Node)_relicContainer).GetChildren(false)).OfType<NRelicBasicHolder>())
		{
			list.Add(item.Relic.Model);
		}
		NGame.Instance.GetInspectRelicScreen().Open(list, node.Model);
	}

	private void UpdateNavigation()
	{
		for (int i = 0; i < ((Node)_relicContainer).GetChildCount(false); i++)
		{
			NRelicBasicHolder child = ((Node)_relicContainer).GetChild<NRelicBasicHolder>(i, false);
			((Control)child).FocusNeighborLeft = ((i > 0) ? ((Node)((Node)_relicContainer).GetChild<NRelicBasicHolder>(i - 1, false)).GetPath() : ((Node)((Node)_relicContainer).GetChild<NRelicBasicHolder>(i, false)).GetPath());
			((Control)child).FocusNeighborRight = ((i < ((Node)_relicContainer).GetChildCount(false) - 1) ? ((Node)((Node)_relicContainer).GetChild<NRelicBasicHolder>(i + 1, false)).GetPath() : ((Node)((Node)_relicContainer).GetChild<NRelicBasicHolder>(i, false)).GetPath());
			((Control)child).FocusNeighborTop = ((Node)child).GetPath();
			if (((Node)_potionContainer).GetChildCount(false) > 0)
			{
				Control child2 = ((Node)_potionContainer).GetChild<Control>(Mathf.Min(i, ((Node)_potionContainer).GetChildCount(false) - 1), false);
				((Control)child).FocusNeighborBottom = ((child2 != null) ? ((Node)child2).GetPath() : null);
			}
			else if (((Node)_cardContainer).GetChildCount(false) > 0)
			{
				Control child3 = ((Node)_cardContainer).GetChild<Control>(Mathf.Min(i, ((Node)_cardContainer).GetChildCount(false) - 1), false);
				((Control)child).FocusNeighborBottom = ((child3 != null) ? ((Node)child3).GetPath() : null);
			}
			else
			{
				((Control)child).FocusNeighborBottom = ((Node)child).GetPath();
			}
		}
		for (int j = 0; j < ((Node)_potionContainer).GetChildCount(false); j++)
		{
			NPotionHolder child4 = ((Node)_potionContainer).GetChild<NPotionHolder>(j, false);
			((Control)child4).FocusNeighborLeft = ((j > 0) ? ((Node)((Node)_potionContainer).GetChild<NPotionHolder>(j - 1, false)).GetPath() : ((Node)((Node)_potionContainer).GetChild<NPotionHolder>(j, false)).GetPath());
			((Control)child4).FocusNeighborRight = ((j < ((Node)_potionContainer).GetChildCount(false) - 1) ? ((Node)((Node)_potionContainer).GetChild<NPotionHolder>(j + 1, false)).GetPath() : ((Node)((Node)_potionContainer).GetChild<NPotionHolder>(j, false)).GetPath());
			if (((Node)_relicContainer).GetChildCount(false) > 0)
			{
				Control child5 = ((Node)_relicContainer).GetChild<Control>(Mathf.Min(j, ((Node)_relicContainer).GetChildCount(false) - 1), false);
				((Control)child4).FocusNeighborTop = ((child5 != null) ? ((Node)child5).GetPath() : null);
			}
			else
			{
				((Control)child4).FocusNeighborTop = ((Node)child4).GetPath();
			}
			if (((Node)_cardContainer).GetChildCount(false) > 0)
			{
				Control child6 = ((Node)_cardContainer).GetChild<Control>(Mathf.Min(j, ((Node)_cardContainer).GetChildCount(false) - 1), false);
				((Control)child4).FocusNeighborBottom = ((child6 != null) ? ((Node)child6).GetPath() : null);
			}
			else
			{
				((Control)child4).FocusNeighborBottom = ((Node)child4).GetPath();
			}
		}
		for (int k = 0; k < ((Node)_cardContainer).GetChildCount(false); k++)
		{
			NDeckHistoryEntry child7 = ((Node)_cardContainer).GetChild<NDeckHistoryEntry>(k, false);
			((Control)child7).FocusNeighborLeft = ((k > 0) ? ((Node)((Node)_cardContainer).GetChild<NDeckHistoryEntry>(k - 1, false)).GetPath() : ((Node)((Node)_cardContainer).GetChild<NDeckHistoryEntry>(k, false)).GetPath());
			((Control)child7).FocusNeighborRight = ((k < ((Node)_cardContainer).GetChildCount(false) - 1) ? ((Node)((Node)_cardContainer).GetChild<NDeckHistoryEntry>(k + 1, false)).GetPath() : ((Node)((Node)_cardContainer).GetChild<NDeckHistoryEntry>(k, false)).GetPath());
			if (((Node)_potionContainer).GetChildCount(false) > 0)
			{
				Control child8 = ((Node)_potionContainer).GetChild<Control>(Mathf.Min(k, ((Node)_potionContainer).GetChildCount(false) - 1), false);
				((Control)child7).FocusNeighborTop = ((child8 != null) ? ((Node)child8).GetPath() : null);
			}
			else if (((Node)_relicContainer).GetChildCount(false) > 0)
			{
				Control child9 = ((Node)_relicContainer).GetChild<Control>(Mathf.Min(k, ((Node)_relicContainer).GetChildCount(false) - 1), false);
				((Control)child7).FocusNeighborTop = ((child9 != null) ? ((Node)child9).GetPath() : null);
			}
			else
			{
				((Control)child7).FocusNeighborTop = ((Node)child7).GetPath();
			}
			((Control)child7).FocusNeighborBottom = ((Node)child7).GetPath();
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Expected O, but got Unknown
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Expected O, but got Unknown
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Expected O, but got Unknown
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Expected O, but got Unknown
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(8);
		list.Add(new MethodInfo(MethodName.AfterCapstoneOpened, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AfterCapstoneClosed, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Input, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("inputEvent"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("InputEvent"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ShowEntry, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("entry"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.BackButtonPressed, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnRelicClicked, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("node"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UpdateNavigation, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
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
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.AfterCapstoneOpened && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			AfterCapstoneOpened();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.AfterCapstoneClosed && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			AfterCapstoneClosed();
			ret = default(godot_variant);
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
		if ((ref method) == MethodName.ShowEntry && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			ShowEntry(VariantUtils.ConvertTo<NDeckHistoryEntry>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.BackButtonPressed && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			BackButtonPressed(VariantUtils.ConvertTo<NButton>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnRelicClicked && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OnRelicClicked(VariantUtils.ConvertTo<NRelic>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.UpdateNavigation && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			UpdateNavigation();
			ret = default(godot_variant);
			return true;
		}
		return ((Control)this).InvokeGodotClassMethod(ref method, args, ref ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if ((ref method) == MethodName.AfterCapstoneOpened)
		{
			return true;
		}
		if ((ref method) == MethodName.AfterCapstoneClosed)
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
		if ((ref method) == MethodName.ShowEntry)
		{
			return true;
		}
		if ((ref method) == MethodName.BackButtonPressed)
		{
			return true;
		}
		if ((ref method) == MethodName.OnRelicClicked)
		{
			return true;
		}
		if ((ref method) == MethodName.UpdateNavigation)
		{
			return true;
		}
		return ((Control)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._playerNameLabel)
		{
			_playerNameLabel = VariantUtils.ConvertTo<MegaRichTextLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._cardsHeader)
		{
			_cardsHeader = VariantUtils.ConvertTo<MegaRichTextLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._cardContainer)
		{
			_cardContainer = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._backButton)
		{
			_backButton = VariantUtils.ConvertTo<NBackButton>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._potionsHeader)
		{
			_potionsHeader = VariantUtils.ConvertTo<MegaRichTextLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._potionContainer)
		{
			_potionContainer = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._relicsHeader)
		{
			_relicsHeader = VariantUtils.ConvertTo<MegaRichTextLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._relicContainer)
		{
			_relicContainer = VariantUtils.ConvertTo<Control>(ref value);
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
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName.UseSharedBackstop)
		{
			bool useSharedBackstop = UseSharedBackstop;
			value = VariantUtils.CreateFrom<bool>(ref useSharedBackstop);
			return true;
		}
		if ((ref name) == PropertyName.ScreenType)
		{
			NetScreenType screenType = ScreenType;
			value = VariantUtils.CreateFrom<NetScreenType>(ref screenType);
			return true;
		}
		if ((ref name) == PropertyName.DefaultFocusedControl)
		{
			Control defaultFocusedControl = DefaultFocusedControl;
			value = VariantUtils.CreateFrom<Control>(ref defaultFocusedControl);
			return true;
		}
		if ((ref name) == PropertyName._playerNameLabel)
		{
			value = VariantUtils.CreateFrom<MegaRichTextLabel>(ref _playerNameLabel);
			return true;
		}
		if ((ref name) == PropertyName._cardsHeader)
		{
			value = VariantUtils.CreateFrom<MegaRichTextLabel>(ref _cardsHeader);
			return true;
		}
		if ((ref name) == PropertyName._cardContainer)
		{
			value = VariantUtils.CreateFrom<Control>(ref _cardContainer);
			return true;
		}
		if ((ref name) == PropertyName._backButton)
		{
			value = VariantUtils.CreateFrom<NBackButton>(ref _backButton);
			return true;
		}
		if ((ref name) == PropertyName._potionsHeader)
		{
			value = VariantUtils.CreateFrom<MegaRichTextLabel>(ref _potionsHeader);
			return true;
		}
		if ((ref name) == PropertyName._potionContainer)
		{
			value = VariantUtils.CreateFrom<Control>(ref _potionContainer);
			return true;
		}
		if ((ref name) == PropertyName._relicsHeader)
		{
			value = VariantUtils.CreateFrom<MegaRichTextLabel>(ref _relicsHeader);
			return true;
		}
		if ((ref name) == PropertyName._relicContainer)
		{
			value = VariantUtils.CreateFrom<Control>(ref _relicContainer);
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
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._playerNameLabel, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._cardsHeader, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._cardContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._backButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._potionsHeader, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._potionContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._relicsHeader, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._relicContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName.UseSharedBackstop, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)2, PropertyName.ScreenType, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		((GodotObject)this).SaveGodotObjectData(info);
		info.AddProperty(PropertyName._playerNameLabel, Variant.From<MegaRichTextLabel>(ref _playerNameLabel));
		info.AddProperty(PropertyName._cardsHeader, Variant.From<MegaRichTextLabel>(ref _cardsHeader));
		info.AddProperty(PropertyName._cardContainer, Variant.From<Control>(ref _cardContainer));
		info.AddProperty(PropertyName._backButton, Variant.From<NBackButton>(ref _backButton));
		info.AddProperty(PropertyName._potionsHeader, Variant.From<MegaRichTextLabel>(ref _potionsHeader));
		info.AddProperty(PropertyName._potionContainer, Variant.From<Control>(ref _potionContainer));
		info.AddProperty(PropertyName._relicsHeader, Variant.From<MegaRichTextLabel>(ref _relicsHeader));
		info.AddProperty(PropertyName._relicContainer, Variant.From<Control>(ref _relicContainer));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._playerNameLabel, ref val))
		{
			_playerNameLabel = ((Variant)(ref val)).As<MegaRichTextLabel>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._cardsHeader, ref val2))
		{
			_cardsHeader = ((Variant)(ref val2)).As<MegaRichTextLabel>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._cardContainer, ref val3))
		{
			_cardContainer = ((Variant)(ref val3)).As<Control>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._backButton, ref val4))
		{
			_backButton = ((Variant)(ref val4)).As<NBackButton>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._potionsHeader, ref val5))
		{
			_potionsHeader = ((Variant)(ref val5)).As<MegaRichTextLabel>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._potionContainer, ref val6))
		{
			_potionContainer = ((Variant)(ref val6)).As<Control>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName._relicsHeader, ref val7))
		{
			_relicsHeader = ((Variant)(ref val7)).As<MegaRichTextLabel>();
		}
		Variant val8 = default(Variant);
		if (info.TryGetProperty(PropertyName._relicContainer, ref val8))
		{
			_relicContainer = ((Variant)(ref val8)).As<Control>();
		}
	}
}
