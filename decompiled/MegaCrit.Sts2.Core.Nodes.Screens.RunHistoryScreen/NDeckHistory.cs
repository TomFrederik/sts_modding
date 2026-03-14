using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.Nodes.Screens.RunHistoryScreen;

[ScriptPath("res://src/Core/Nodes/Screens/RunHistoryScreen/NDeckHistory.cs")]
public class NDeckHistory : VBoxContainer
{
	[Signal]
	public delegate void HoveredEventHandler(NDeckHistoryEntry deckHistoryEntry);

	[Signal]
	public delegate void UnhoveredEventHandler(NDeckHistoryEntry deckHistoryEntry);

	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName ShowEntry = StringName.op_Implicit("ShowEntry");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName _headerLabel = StringName.op_Implicit("_headerLabel");

		public static readonly StringName _cardContainer = StringName.op_Implicit("_cardContainer");
	}

	public class SignalName : SignalName
	{
		public static readonly StringName Hovered = StringName.op_Implicit("Hovered");

		public static readonly StringName Unhovered = StringName.op_Implicit("Unhovered");
	}

	private readonly LocString _deckHeader = new LocString("run_history", "DECK_HISTORY.header");

	private readonly LocString _cardCategories = new LocString("run_history", "DECK_HISTORY.categories");

	private MegaRichTextLabel _headerLabel;

	private Control _cardContainer;

	private readonly List<CardModel> _allCards = new List<CardModel>();

	private HoveredEventHandler backing_Hovered;

	private UnhoveredEventHandler backing_Unhovered;

	public event HoveredEventHandler Hovered
	{
		add
		{
			backing_Hovered = (HoveredEventHandler)Delegate.Combine(backing_Hovered, value);
		}
		remove
		{
			backing_Hovered = (HoveredEventHandler)Delegate.Remove(backing_Hovered, value);
		}
	}

	public event UnhoveredEventHandler Unhovered
	{
		add
		{
			backing_Unhovered = (UnhoveredEventHandler)Delegate.Combine(backing_Unhovered, value);
		}
		remove
		{
			backing_Unhovered = (UnhoveredEventHandler)Delegate.Remove(backing_Unhovered, value);
		}
	}

	public override void _Ready()
	{
		_headerLabel = ((Node)this).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("Header"));
		_cardContainer = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%CardContainer"));
	}

	public void LoadDeck(Player player, IEnumerable<SerializableCard> cards)
	{
		StringBuilder stringBuilder = new StringBuilder();
		Dictionary<CardRarity, int> dictionary = new Dictionary<CardRarity, int>();
		CardRarity[] values = Enum.GetValues<CardRarity>();
		foreach (CardRarity key in values)
		{
			dictionary.Add(key, 0);
		}
		List<SerializableCard> list = cards.ToList();
		CardRarity key2;
		int value;
		foreach (SerializableCard item in list)
		{
			CardModel cardModel = SaveUtil.CardOrDeprecated(item.Id);
			key2 = cardModel.Rarity;
			value = dictionary[key2]++;
		}
		_deckHeader.Add("totalCards", list.Count);
		foreach (KeyValuePair<CardRarity, int> item2 in dictionary)
		{
			item2.Deconstruct(out key2, out value);
			CardRarity cardRarity = key2;
			int num = value;
			_cardCategories.Add(cardRarity.ToString() + "Cards", num);
		}
		StringBuilder stringBuilder2 = stringBuilder;
		StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(20, 1, stringBuilder2);
		handler.AppendLiteral("[gold][b]");
		handler.AppendFormatted(_deckHeader.GetFormattedText());
		handler.AppendLiteral("[/b][/gold]");
		stringBuilder2.Append(ref handler);
		stringBuilder.Append(_cardCategories.GetFormattedText().Trim(','));
		_headerLabel.Text = stringBuilder.ToString();
		PopulateCards(player, list);
	}

	private void PopulateCards(Player player, IEnumerable<SerializableCard> cards)
	{
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		foreach (Node child in ((Node)_cardContainer).GetChildren(false))
		{
			child.QueueFreeSafely();
		}
		_allCards.Clear();
		foreach (IGrouping<SerializableCard, SerializableCard> item in from x in cards
			group x by x)
		{
			CardModel cardModel = CardModel.FromSerializable(item.Key);
			cardModel.Owner = player;
			_allCards.Add(cardModel);
			NDeckHistoryEntry entry = NDeckHistoryEntry.Create(cardModel, item.Count(), from c in item
				where c.FloorAddedToDeck.HasValue
				select c.FloorAddedToDeck.Value);
			((GodotObject)entry).Connect(NDeckHistoryEntry.SignalName.Clicked, Callable.From<NDeckHistoryEntry>((Action<NDeckHistoryEntry>)ShowEntry), 0u);
			((GodotObject)entry).Connect(NClickableControl.SignalName.Focused, Callable.From<NClickableControl>((Action<NClickableControl>)delegate
			{
				//IL_0019: Unknown result type (might be due to invalid IL or missing references)
				//IL_001e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0023: Unknown result type (might be due to invalid IL or missing references)
				((GodotObject)this).EmitSignal(SignalName.Hovered, (Variant[])(object)new Variant[1] { Variant.op_Implicit((GodotObject)(object)entry) });
			}), 0u);
			((GodotObject)entry).Connect(NClickableControl.SignalName.Unfocused, Callable.From<NClickableControl>((Action<NClickableControl>)delegate
			{
				//IL_0019: Unknown result type (might be due to invalid IL or missing references)
				//IL_001e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0023: Unknown result type (might be due to invalid IL or missing references)
				((GodotObject)this).EmitSignal(SignalName.Unhovered, (Variant[])(object)new Variant[1] { Variant.op_Implicit((GodotObject)(object)entry) });
			}), 0u);
			((Node)(object)_cardContainer).AddChildSafely((Node?)(object)entry);
		}
	}

	private void ShowEntry(NDeckHistoryEntry entry)
	{
		NGame.Instance.GetInspectCardScreen().Open(_allCards, _allCards.IndexOf(entry.Card));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Expected O, but got Unknown
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(2);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ShowEntry, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("entry"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.ShowEntry && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			ShowEntry(VariantUtils.ConvertTo<NDeckHistoryEntry>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		return ((VBoxContainer)this).InvokeGodotClassMethod(ref method, args, ref ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if ((ref method) == MethodName._Ready)
		{
			return true;
		}
		if ((ref method) == MethodName.ShowEntry)
		{
			return true;
		}
		return ((VBoxContainer)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._headerLabel)
		{
			_headerLabel = VariantUtils.ConvertTo<MegaRichTextLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._cardContainer)
		{
			_cardContainer = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		return ((GodotObject)this).SetGodotClassPropertyValue(ref name, ref value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName._headerLabel)
		{
			value = VariantUtils.CreateFrom<MegaRichTextLabel>(ref _headerLabel);
			return true;
		}
		if ((ref name) == PropertyName._cardContainer)
		{
			value = VariantUtils.CreateFrom<Control>(ref _cardContainer);
			return true;
		}
		return ((GodotObject)this).GetGodotClassPropertyValue(ref name, ref value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._headerLabel, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._cardContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		info.AddProperty(PropertyName._headerLabel, Variant.From<MegaRichTextLabel>(ref _headerLabel));
		info.AddProperty(PropertyName._cardContainer, Variant.From<Control>(ref _cardContainer));
		info.AddSignalEventDelegate(SignalName.Hovered, (Delegate)backing_Hovered);
		info.AddSignalEventDelegate(SignalName.Unhovered, (Delegate)backing_Unhovered);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._headerLabel, ref val))
		{
			_headerLabel = ((Variant)(ref val)).As<MegaRichTextLabel>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._cardContainer, ref val2))
		{
			_cardContainer = ((Variant)(ref val2)).As<Control>();
		}
		HoveredEventHandler hoveredEventHandler = default(HoveredEventHandler);
		if (info.TryGetSignalEventDelegate<HoveredEventHandler>(SignalName.Hovered, ref hoveredEventHandler))
		{
			backing_Hovered = hoveredEventHandler;
		}
		UnhoveredEventHandler unhoveredEventHandler = default(UnhoveredEventHandler);
		if (info.TryGetSignalEventDelegate<UnhoveredEventHandler>(SignalName.Unhovered, ref unhoveredEventHandler))
		{
			backing_Unhovered = unhoveredEventHandler;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotSignalList()
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
		List<MethodInfo> list = new List<MethodInfo>(2);
		list.Add(new MethodInfo(SignalName.Hovered, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("deckHistoryEntry"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(SignalName.Unhovered, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("deckHistoryEntry"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		return list;
	}

	protected void EmitSignalHovered(NDeckHistoryEntry deckHistoryEntry)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).EmitSignal(SignalName.Hovered, (Variant[])(object)new Variant[1] { Variant.op_Implicit((GodotObject)(object)deckHistoryEntry) });
	}

	protected void EmitSignalUnhovered(NDeckHistoryEntry deckHistoryEntry)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).EmitSignal(SignalName.Unhovered, (Variant[])(object)new Variant[1] { Variant.op_Implicit((GodotObject)(object)deckHistoryEntry) });
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RaiseGodotClassSignalCallbacks(in godot_string_name signal, NativeVariantPtrArgs args)
	{
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		if ((ref signal) == SignalName.Hovered && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			backing_Hovered?.Invoke(VariantUtils.ConvertTo<NDeckHistoryEntry>(ref ((NativeVariantPtrArgs)(ref args))[0]));
		}
		else if ((ref signal) == SignalName.Unhovered && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			backing_Unhovered?.Invoke(VariantUtils.ConvertTo<NDeckHistoryEntry>(ref ((NativeVariantPtrArgs)(ref args))[0]));
		}
		else
		{
			((GodotObject)this).RaiseGodotClassSignalCallbacks(ref signal, args);
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassSignal(in godot_string_name signal)
	{
		if ((ref signal) == SignalName.Hovered)
		{
			return true;
		}
		if ((ref signal) == SignalName.Unhovered)
		{
			return true;
		}
		return ((VBoxContainer)this).HasGodotClassSignal(ref signal);
	}
}
