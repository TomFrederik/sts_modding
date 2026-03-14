using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.Nodes.Screens.GameOverScreen;

[ScriptPath("res://src/Core/Nodes/Screens/GameOverScreen/NRunSummary.cs")]
public class NRunSummary : Control
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName _badgeContainer = StringName.op_Implicit("_badgeContainer");

		public static readonly StringName _discoveryContainer = StringName.op_Implicit("_discoveryContainer");

		public static readonly StringName _discoveryHeader = StringName.op_Implicit("_discoveryHeader");

		public static readonly StringName _discoveredCards = StringName.op_Implicit("_discoveredCards");

		public static readonly StringName _discoveredRelics = StringName.op_Implicit("_discoveredRelics");

		public static readonly StringName _discoveredPotions = StringName.op_Implicit("_discoveredPotions");

		public static readonly StringName _discoveredEnemies = StringName.op_Implicit("_discoveredEnemies");

		public static readonly StringName _discoveredEpochs = StringName.op_Implicit("_discoveredEpochs");

		public static readonly StringName _cardCount = StringName.op_Implicit("_cardCount");

		public static readonly StringName _relicCount = StringName.op_Implicit("_relicCount");

		public static readonly StringName _potionCount = StringName.op_Implicit("_potionCount");

		public static readonly StringName _enemyCount = StringName.op_Implicit("_enemyCount");

		public static readonly StringName _epochCount = StringName.op_Implicit("_epochCount");

		public static readonly StringName _tween = StringName.op_Implicit("_tween");

		public static readonly StringName _waitTween = StringName.op_Implicit("_waitTween");
	}

	public class SignalName : SignalName
	{
	}

	private Control _badgeContainer;

	private Control _discoveryContainer;

	private Control _discoveryHeader;

	private NDiscoveredItem _discoveredCards;

	private NDiscoveredItem _discoveredRelics;

	private NDiscoveredItem _discoveredPotions;

	private NDiscoveredItem _discoveredEnemies;

	private NDiscoveredItem _discoveredEpochs;

	private MegaLabel _cardCount;

	private MegaLabel _relicCount;

	private MegaLabel _potionCount;

	private MegaLabel _enemyCount;

	private MegaLabel _epochCount;

	private Tween? _tween;

	private Tween? _waitTween;

	private const int _maxItemsToList = 10;

	public override void _Ready()
	{
		_badgeContainer = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%BadgeContainer"));
		_discoveryContainer = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%DiscoveryContainer"));
		_discoveryHeader = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%DiscoveryHeader"));
		_cardCount = ((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%CardCount"));
		_relicCount = ((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%RelicCount"));
		_potionCount = ((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%PotionCount"));
		_enemyCount = ((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%EnemyCount"));
		_epochCount = ((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%EpochCount"));
		_discoveredCards = ((Node)_cardCount).GetParent<NDiscoveredItem>();
		_discoveredRelics = ((Node)_relicCount).GetParent<NDiscoveredItem>();
		_discoveredPotions = ((Node)_potionCount).GetParent<NDiscoveredItem>();
		_discoveredEnemies = ((Node)_enemyCount).GetParent<NDiscoveredItem>();
		_discoveredEpochs = ((Node)_epochCount).GetParent<NDiscoveredItem>();
		((CanvasItem)_discoveredCards).Visible = false;
		((CanvasItem)_discoveredRelics).Visible = false;
		((CanvasItem)_discoveredPotions).Visible = false;
		((CanvasItem)_discoveredEnemies).Visible = false;
		((CanvasItem)_discoveredEpochs).Visible = false;
	}

	public async Task AnimateInDiscoveries(RunState runState)
	{
		Player player = LocalContext.GetMe(runState);
		if (player.DiscoveredCards.Count + player.DiscoveredRelics.Count + player.DiscoveredPotions.Count + player.DiscoveredEnemies.Count + player.DiscoveredEpochs.Count == 0)
		{
			Log.Info("No discoveries this time. Very sad");
			return;
		}
		Tween val = ((Node)this).CreateTween();
		val.TweenProperty((GodotObject)(object)_discoveryHeader, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(1f), 0.25);
		await Task.Delay(100);
		if (player.DiscoveredCards.Count > 0)
		{
			string discoveryBodyText = GetDiscoveryBodyText(player.DiscoveredCards, (ModelId id) => ModelDb.GetById<CardModel>(id).Title, "game_over_screen", "DISCOVERY_BODY_CARD", "CardCount");
			_discoveredCards.SetHoverTip(new HoverTip(new LocString("game_over_screen", "DISCOVERY_HEADER_CARD"), discoveryBodyText));
			((CanvasItem)_discoveredCards).Visible = true;
			((CanvasItem)_discoveredCards).Modulate = StsColors.transparentBlack;
		}
		if (player.DiscoveredRelics.Count > 0)
		{
			string discoveryBodyText2 = GetDiscoveryBodyText(player.DiscoveredRelics, (ModelId id) => ModelDb.GetById<RelicModel>(id).Title.GetFormattedText(), "game_over_screen", "DISCOVERY_BODY_RELIC", "RelicCount");
			_discoveredRelics.SetHoverTip(new HoverTip(new LocString("game_over_screen", "DISCOVERY_HEADER_RELIC"), discoveryBodyText2));
			((CanvasItem)_discoveredRelics).Visible = true;
			((CanvasItem)_discoveredRelics).Modulate = StsColors.transparentBlack;
		}
		if (player.DiscoveredPotions.Count > 0)
		{
			string discoveryBodyText3 = GetDiscoveryBodyText(player.DiscoveredPotions, (ModelId id) => ModelDb.GetById<PotionModel>(id).Title.GetFormattedText(), "game_over_screen", "DISCOVERY_BODY_POTION", "PotionCount");
			_discoveredPotions.SetHoverTip(new HoverTip(new LocString("game_over_screen", "DISCOVERY_HEADER_POTION"), discoveryBodyText3));
			((CanvasItem)_discoveredPotions).Visible = true;
			((CanvasItem)_discoveredPotions).Modulate = StsColors.transparentBlack;
		}
		if (player.DiscoveredEnemies.Count > 0)
		{
			string discoveryBodyText4 = GetDiscoveryBodyText(player.DiscoveredEnemies, (ModelId id) => ModelDb.GetById<MonsterModel>(id).Title.GetFormattedText(), "game_over_screen", "DISCOVERY_BODY_ENEMY", "EnemyCount");
			_discoveredEnemies.SetHoverTip(new HoverTip(new LocString("game_over_screen", "DISCOVERY_HEADER_ENEMY"), discoveryBodyText4));
			((CanvasItem)_discoveredEnemies).Visible = true;
			((CanvasItem)_discoveredEnemies).Modulate = StsColors.transparentBlack;
		}
		if (player.DiscoveredEpochs.Count > 0)
		{
			LocString title = new LocString("game_over_screen", "DISCOVERY_HEADER_EPOCH");
			LocString locString = new LocString("game_over_screen", "DISCOVERY_BODY_EPOCH");
			locString.Add("EpochCount", player.DiscoveredEpochs.Count);
			HoverTip hoverTip = new HoverTip(title, locString);
			_discoveredEpochs.SetHoverTip(hoverTip);
			((CanvasItem)_discoveredEpochs).Visible = true;
			((CanvasItem)_discoveredEpochs).Modulate = StsColors.transparentBlack;
		}
		if (((CanvasItem)_discoveredCards).Visible)
		{
			_cardCount.SetTextAutoSize($"{player.DiscoveredCards.Count}");
			await TaskHelper.RunSafely(DiscoveryAnimHelper((Control)(object)_discoveredCards));
		}
		if (((CanvasItem)_discoveredRelics).Visible)
		{
			_relicCount.SetTextAutoSize($"{player.DiscoveredRelics.Count}");
			await TaskHelper.RunSafely(DiscoveryAnimHelper((Control)(object)_discoveredRelics));
		}
		if (((CanvasItem)_discoveredPotions).Visible)
		{
			_potionCount.SetTextAutoSize($"{player.DiscoveredPotions.Count}");
			await TaskHelper.RunSafely(DiscoveryAnimHelper((Control)(object)_discoveredPotions));
		}
		if (((CanvasItem)_discoveredEnemies).Visible)
		{
			_enemyCount.SetTextAutoSize($"{player.DiscoveredEnemies.Count}");
			await TaskHelper.RunSafely(DiscoveryAnimHelper((Control)(object)_discoveredEnemies));
		}
		if (((CanvasItem)_discoveredEpochs).Visible)
		{
			_epochCount.SetTextAutoSize($"{player.DiscoveredEpochs.Count}");
			await TaskHelper.RunSafely(DiscoveryAnimHelper((Control)(object)_discoveredEpochs));
		}
	}

	private async Task DiscoveryAnimHelper(Control node)
	{
		((CanvasItem)node).Modulate = StsColors.transparentBlack;
		Tween? tween = _tween;
		if (tween != null)
		{
			tween.Kill();
		}
		_tween = ((Node)this).CreateTween().SetParallel(true);
		_tween.TweenProperty((GodotObject)(object)node, NodePath.op_Implicit("modulate"), Variant.op_Implicit(Colors.White), 0.3);
		_tween.TweenProperty((GodotObject)(object)node, NodePath.op_Implicit("position:y"), Variant.op_Implicit(0f), 0.3).SetEase((EaseType)1).SetTrans((TransitionType)10)
			.From(Variant.op_Implicit(100f));
		await ((GodotObject)this).ToSignal((GodotObject)(object)_tween, SignalName.Finished);
	}

	private static string GetDiscoveryBodyText<T>(List<T> discoveredIds, Func<T, string> getTitle, string locTable, string locKey, string countParam)
	{
		LocString locString = new LocString(locTable, locKey);
		locString.Add(countParam, discoveredIds.Count);
		string text = string.Join("\n", discoveredIds.Take(10).Select(getTitle));
		if (discoveredIds.Count > 10)
		{
			text += "\n....";
		}
		return locString.GetFormattedText() + "\n\n" + text;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(1);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
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
		return ((Control)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._badgeContainer)
		{
			_badgeContainer = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._discoveryContainer)
		{
			_discoveryContainer = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._discoveryHeader)
		{
			_discoveryHeader = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._discoveredCards)
		{
			_discoveredCards = VariantUtils.ConvertTo<NDiscoveredItem>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._discoveredRelics)
		{
			_discoveredRelics = VariantUtils.ConvertTo<NDiscoveredItem>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._discoveredPotions)
		{
			_discoveredPotions = VariantUtils.ConvertTo<NDiscoveredItem>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._discoveredEnemies)
		{
			_discoveredEnemies = VariantUtils.ConvertTo<NDiscoveredItem>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._discoveredEpochs)
		{
			_discoveredEpochs = VariantUtils.ConvertTo<NDiscoveredItem>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._cardCount)
		{
			_cardCount = VariantUtils.ConvertTo<MegaLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._relicCount)
		{
			_relicCount = VariantUtils.ConvertTo<MegaLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._potionCount)
		{
			_potionCount = VariantUtils.ConvertTo<MegaLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._enemyCount)
		{
			_enemyCount = VariantUtils.ConvertTo<MegaLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._epochCount)
		{
			_epochCount = VariantUtils.ConvertTo<MegaLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._tween)
		{
			_tween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._waitTween)
		{
			_waitTween = VariantUtils.ConvertTo<Tween>(ref value);
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
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName._badgeContainer)
		{
			value = VariantUtils.CreateFrom<Control>(ref _badgeContainer);
			return true;
		}
		if ((ref name) == PropertyName._discoveryContainer)
		{
			value = VariantUtils.CreateFrom<Control>(ref _discoveryContainer);
			return true;
		}
		if ((ref name) == PropertyName._discoveryHeader)
		{
			value = VariantUtils.CreateFrom<Control>(ref _discoveryHeader);
			return true;
		}
		if ((ref name) == PropertyName._discoveredCards)
		{
			value = VariantUtils.CreateFrom<NDiscoveredItem>(ref _discoveredCards);
			return true;
		}
		if ((ref name) == PropertyName._discoveredRelics)
		{
			value = VariantUtils.CreateFrom<NDiscoveredItem>(ref _discoveredRelics);
			return true;
		}
		if ((ref name) == PropertyName._discoveredPotions)
		{
			value = VariantUtils.CreateFrom<NDiscoveredItem>(ref _discoveredPotions);
			return true;
		}
		if ((ref name) == PropertyName._discoveredEnemies)
		{
			value = VariantUtils.CreateFrom<NDiscoveredItem>(ref _discoveredEnemies);
			return true;
		}
		if ((ref name) == PropertyName._discoveredEpochs)
		{
			value = VariantUtils.CreateFrom<NDiscoveredItem>(ref _discoveredEpochs);
			return true;
		}
		if ((ref name) == PropertyName._cardCount)
		{
			value = VariantUtils.CreateFrom<MegaLabel>(ref _cardCount);
			return true;
		}
		if ((ref name) == PropertyName._relicCount)
		{
			value = VariantUtils.CreateFrom<MegaLabel>(ref _relicCount);
			return true;
		}
		if ((ref name) == PropertyName._potionCount)
		{
			value = VariantUtils.CreateFrom<MegaLabel>(ref _potionCount);
			return true;
		}
		if ((ref name) == PropertyName._enemyCount)
		{
			value = VariantUtils.CreateFrom<MegaLabel>(ref _enemyCount);
			return true;
		}
		if ((ref name) == PropertyName._epochCount)
		{
			value = VariantUtils.CreateFrom<MegaLabel>(ref _epochCount);
			return true;
		}
		if ((ref name) == PropertyName._tween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _tween);
			return true;
		}
		if ((ref name) == PropertyName._waitTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _waitTween);
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
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._badgeContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._discoveryContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._discoveryHeader, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._discoveredCards, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._discoveredRelics, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._discoveredPotions, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._discoveredEnemies, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._discoveredEpochs, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._cardCount, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._relicCount, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._potionCount, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._enemyCount, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._epochCount, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._tween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._waitTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		((GodotObject)this).SaveGodotObjectData(info);
		info.AddProperty(PropertyName._badgeContainer, Variant.From<Control>(ref _badgeContainer));
		info.AddProperty(PropertyName._discoveryContainer, Variant.From<Control>(ref _discoveryContainer));
		info.AddProperty(PropertyName._discoveryHeader, Variant.From<Control>(ref _discoveryHeader));
		info.AddProperty(PropertyName._discoveredCards, Variant.From<NDiscoveredItem>(ref _discoveredCards));
		info.AddProperty(PropertyName._discoveredRelics, Variant.From<NDiscoveredItem>(ref _discoveredRelics));
		info.AddProperty(PropertyName._discoveredPotions, Variant.From<NDiscoveredItem>(ref _discoveredPotions));
		info.AddProperty(PropertyName._discoveredEnemies, Variant.From<NDiscoveredItem>(ref _discoveredEnemies));
		info.AddProperty(PropertyName._discoveredEpochs, Variant.From<NDiscoveredItem>(ref _discoveredEpochs));
		info.AddProperty(PropertyName._cardCount, Variant.From<MegaLabel>(ref _cardCount));
		info.AddProperty(PropertyName._relicCount, Variant.From<MegaLabel>(ref _relicCount));
		info.AddProperty(PropertyName._potionCount, Variant.From<MegaLabel>(ref _potionCount));
		info.AddProperty(PropertyName._enemyCount, Variant.From<MegaLabel>(ref _enemyCount));
		info.AddProperty(PropertyName._epochCount, Variant.From<MegaLabel>(ref _epochCount));
		info.AddProperty(PropertyName._tween, Variant.From<Tween>(ref _tween));
		info.AddProperty(PropertyName._waitTween, Variant.From<Tween>(ref _waitTween));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._badgeContainer, ref val))
		{
			_badgeContainer = ((Variant)(ref val)).As<Control>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._discoveryContainer, ref val2))
		{
			_discoveryContainer = ((Variant)(ref val2)).As<Control>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._discoveryHeader, ref val3))
		{
			_discoveryHeader = ((Variant)(ref val3)).As<Control>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._discoveredCards, ref val4))
		{
			_discoveredCards = ((Variant)(ref val4)).As<NDiscoveredItem>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._discoveredRelics, ref val5))
		{
			_discoveredRelics = ((Variant)(ref val5)).As<NDiscoveredItem>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._discoveredPotions, ref val6))
		{
			_discoveredPotions = ((Variant)(ref val6)).As<NDiscoveredItem>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName._discoveredEnemies, ref val7))
		{
			_discoveredEnemies = ((Variant)(ref val7)).As<NDiscoveredItem>();
		}
		Variant val8 = default(Variant);
		if (info.TryGetProperty(PropertyName._discoveredEpochs, ref val8))
		{
			_discoveredEpochs = ((Variant)(ref val8)).As<NDiscoveredItem>();
		}
		Variant val9 = default(Variant);
		if (info.TryGetProperty(PropertyName._cardCount, ref val9))
		{
			_cardCount = ((Variant)(ref val9)).As<MegaLabel>();
		}
		Variant val10 = default(Variant);
		if (info.TryGetProperty(PropertyName._relicCount, ref val10))
		{
			_relicCount = ((Variant)(ref val10)).As<MegaLabel>();
		}
		Variant val11 = default(Variant);
		if (info.TryGetProperty(PropertyName._potionCount, ref val11))
		{
			_potionCount = ((Variant)(ref val11)).As<MegaLabel>();
		}
		Variant val12 = default(Variant);
		if (info.TryGetProperty(PropertyName._enemyCount, ref val12))
		{
			_enemyCount = ((Variant)(ref val12)).As<MegaLabel>();
		}
		Variant val13 = default(Variant);
		if (info.TryGetProperty(PropertyName._epochCount, ref val13))
		{
			_epochCount = ((Variant)(ref val13)).As<MegaLabel>();
		}
		Variant val14 = default(Variant);
		if (info.TryGetProperty(PropertyName._tween, ref val14))
		{
			_tween = ((Variant)(ref val14)).As<Tween>();
		}
		Variant val15 = default(Variant);
		if (info.TryGetProperty(PropertyName._waitTween, ref val15))
		{
			_waitTween = ((Variant)(ref val15)).As<Tween>();
		}
	}
}
