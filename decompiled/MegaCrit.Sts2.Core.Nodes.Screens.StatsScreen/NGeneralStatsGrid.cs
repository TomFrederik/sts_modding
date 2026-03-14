using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Characters;
using MegaCrit.Sts2.Core.Platform;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.Timeline;

namespace MegaCrit.Sts2.Core.Nodes.Screens.StatsScreen;

[ScriptPath("res://src/Core/Nodes/Screens/StatsScreen/NGeneralStatsGrid.cs")]
public class NGeneralStatsGrid : Control
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName CreateSection = StringName.op_Implicit("CreateSection");

		public static readonly StringName LoadStats = StringName.op_Implicit("LoadStats");

		public static readonly StringName SetupHoverTips = StringName.op_Implicit("SetupHoverTips");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName DefaultFocusedControl = StringName.op_Implicit("DefaultFocusedControl");

		public static readonly StringName _gridContainer = StringName.op_Implicit("_gridContainer");

		public static readonly StringName _achievementsEntry = StringName.op_Implicit("_achievementsEntry");

		public static readonly StringName _playtimeEntry = StringName.op_Implicit("_playtimeEntry");

		public static readonly StringName _cardsEntry = StringName.op_Implicit("_cardsEntry");

		public static readonly StringName _winLossEntry = StringName.op_Implicit("_winLossEntry");

		public static readonly StringName _monsterEntry = StringName.op_Implicit("_monsterEntry");

		public static readonly StringName _relicEntry = StringName.op_Implicit("_relicEntry");

		public static readonly StringName _potionEntry = StringName.op_Implicit("_potionEntry");

		public static readonly StringName _eventsEntry = StringName.op_Implicit("_eventsEntry");

		public static readonly StringName _streakEntry = StringName.op_Implicit("_streakEntry");

		public static readonly StringName _characterStatContainer = StringName.op_Implicit("_characterStatContainer");

		public static readonly StringName _screenTween = StringName.op_Implicit("_screenTween");
	}

	public class SignalName : SignalName
	{
	}

	private static readonly string _achievementsIconPath = ImageHelper.GetImagePath("atlases/stats_screen_atlas.sprites/stats_achievements.tres");

	private static readonly string _playtimeIconPath = ImageHelper.GetImagePath("atlases/stats_screen_atlas.sprites/stats_clock.tres");

	private static readonly string _cardsIconPath = ImageHelper.GetImagePath("atlases/stats_screen_atlas.sprites/stats_cards.tres");

	private static readonly string _winLossIconPath = ImageHelper.GetImagePath("atlases/stats_screen_atlas.sprites/stats_swords.tres");

	private static readonly string _monsterIconPath = ImageHelper.GetImagePath("atlases/stats_screen_atlas.sprites/stats_monsters.tres");

	private static readonly string _ancientsIconPath = ImageHelper.GetImagePath("atlases/stats_screen_atlas.sprites/stats_ancients.tres");

	private static readonly string _relicIconPath = ImageHelper.GetImagePath("atlases/stats_screen_atlas.sprites/stats_chest.tres");

	private static readonly string _potionIconPath = ImageHelper.GetImagePath("atlases/stats_screen_atlas.sprites/stats_potions_seen.tres");

	private static readonly string _eventsIconPath = ImageHelper.GetImagePath("atlases/stats_screen_atlas.sprites/stats_questionmark.tres");

	private static readonly string _streakIconPath = ImageHelper.GetImagePath("atlases/stats_screen_atlas.sprites/stats_chain.tres");

	private Node _gridContainer;

	private NStatEntry _achievementsEntry;

	private NStatEntry _playtimeEntry;

	private NStatEntry _cardsEntry;

	private NStatEntry _winLossEntry;

	private NStatEntry _monsterEntry;

	private NStatEntry _relicEntry;

	private NStatEntry _potionEntry;

	private NStatEntry _eventsEntry;

	private NStatEntry _streakEntry;

	private Control _characterStatContainer;

	private Tween? _screenTween;

	public static string[] AssetPaths
	{
		get
		{
			List<string> list = new List<string>();
			list.Add(_achievementsIconPath);
			list.Add(_playtimeIconPath);
			list.Add(_cardsIconPath);
			list.Add(_winLossIconPath);
			list.Add(_monsterIconPath);
			list.Add(_ancientsIconPath);
			list.Add(_relicIconPath);
			list.Add(_potionIconPath);
			list.Add(_eventsIconPath);
			list.Add(_streakIconPath);
			list.AddRange(NCharacterStats.AssetPaths);
			return list.ToArray();
		}
	}

	private static HoverTip PlaytimeTip => new HoverTip(new LocString("stats_screen", "TIP_PLAYTIME.header"), new LocString("stats_screen", "TIP_PLAYTIME.description"));

	private static HoverTip WinsLossesTip => new HoverTip(new LocString("stats_screen", "TIP_WIN_LOSS.header"), new LocString("stats_screen", "TIP_WIN_LOSS.description"));

	public Control DefaultFocusedControl => (Control)(object)_achievementsEntry;

	public override void _Ready()
	{
		_gridContainer = (Node)(object)((Node)this).GetNode<Control>(NodePath.op_Implicit("%GridContainer"));
		_characterStatContainer = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%CharacterStatsContainer"));
		_achievementsEntry = CreateSection(_achievementsIconPath);
		_playtimeEntry = CreateSection(_playtimeIconPath);
		_cardsEntry = CreateSection(_cardsIconPath);
		_winLossEntry = CreateSection(_winLossIconPath);
		_monsterEntry = CreateSection(_monsterIconPath);
		_relicEntry = CreateSection(_relicIconPath);
		_potionEntry = CreateSection(_potionIconPath);
		_eventsEntry = CreateSection(_eventsIconPath);
		_streakEntry = CreateSection(_streakIconPath);
		SetupHoverTips();
	}

	private NStatEntry CreateSection(string imgUrl)
	{
		NStatEntry nStatEntry = NStatEntry.Create(imgUrl);
		_gridContainer.AddChildSafely((Node?)(object)nStatEntry);
		return nStatEntry;
	}

	public void LoadStats()
	{
		ProgressState progressSave = SaveManager.Instance.Progress;
		SaveManager instance = SaveManager.Instance;
		LocString locString = new LocString("stats_screen", "ENTRY_ACHIEVEMENTS.top");
		int denominator = AchievementsUtil.TotalAchievementCount();
		int numerator = AchievementsUtil.UnlockedAchievementCount();
		locString.Add("Amount", StringHelper.RatioFormat(numerator, denominator));
		_achievementsEntry.SetTopText(locString.GetFormattedText() ?? "");
		locString = new LocString("stats_screen", "ENTRY_ACHIEVEMENTS.bottom");
		int numerator2 = progressSave.Epochs.Count((SerializableEpoch epoch) => epoch.State >= EpochState.Revealed);
		if (EpochModel.AllEpochIds.All((string id) => progressSave.Epochs.Any((SerializableEpoch epoch) => epoch.Id == id)))
		{
			int count = progressSave.Epochs.Count;
			locString.Add("Amount", StringHelper.RatioFormat(numerator2, count));
		}
		else
		{
			locString.Add("Amount", StringHelper.RatioFormat(numerator2.ToString(), "??"));
		}
		_achievementsEntry.SetBottomText(locString.GetFormattedText() ?? "");
		locString = new LocString("stats_screen", "ENTRY_PLAYTIME.top");
		locString.Add("Playtime", TimeFormatting.Format(progressSave.TotalPlaytime));
		_playtimeEntry.SetTopText(locString.GetFormattedText());
		if (progressSave.Wins > 0)
		{
			locString = new LocString("stats_screen", "ENTRY_PLAYTIME.bottom");
			locString.Add("FastestWin", TimeFormatting.Format(progressSave.FastestVictory));
			_playtimeEntry.SetBottomText(locString.GetFormattedText());
		}
		locString = new LocString("stats_screen", "ENTRY_CARDS.top");
		locString.Add("Amount", StringHelper.RatioFormat(instance.GetTotalUnlockedCards(), SaveManager.GetUnlockableCardCount()));
		_cardsEntry.SetTopText(locString.GetFormattedText() ?? "");
		locString = new LocString("stats_screen", "ENTRY_CARDS.bottom");
		locString.Add("Amount", StringHelper.RatioFormat(progressSave.DiscoveredCards.Count, ModelDb.AllCards.Count()));
		_cardsEntry.SetBottomText(locString.GetFormattedText());
		int aggregateAscensionProgress = instance.GetAggregateAscensionProgress();
		if (aggregateAscensionProgress > 0)
		{
			locString = new LocString("stats_screen", "ENTRY_WIN_LOSS.top");
			locString.Add("Amount", StringHelper.RatioFormat(aggregateAscensionProgress, SaveManager.GetAggregateAscensionCount()));
			_winLossEntry.SetTopText(locString.GetFormattedText() ?? "");
		}
		locString = new LocString("stats_screen", "ENTRY_WIN_LOSS.bottom");
		locString.Add("Wins", progressSave.Wins);
		locString.Add("Losses", progressSave.Losses);
		_winLossEntry.SetBottomText(locString.GetFormattedText());
		locString = new LocString("stats_screen", "ENTRY_MONSTER.top");
		locString.Add("Amount", StringHelper.Radix(instance.GetTotalKills()));
		_monsterEntry.SetTopText(locString.GetFormattedText() ?? "");
		locString = new LocString("stats_screen", "ENTRY_MONSTER.bottom");
		locString.Add("Amount", StringHelper.RatioFormat(instance.Progress.EnemyStats.Count, ModelDb.Monsters.Count()));
		_monsterEntry.SetBottomText(locString.GetFormattedText() ?? "");
		locString = new LocString("stats_screen", "ENTRY_RELIC.top");
		locString.Add("Amount", StringHelper.RatioFormat(instance.GetTotalUnlockedRelics(), SaveManager.GetUnlockableRelicCount()));
		_relicEntry.SetTopText(locString.GetFormattedText() ?? "");
		locString = new LocString("stats_screen", "ENTRY_RELIC.bottom");
		locString.Add("Amount", StringHelper.RatioFormat(progressSave.DiscoveredRelics.Count, ModelDb.AllRelics.Count()));
		_relicEntry.SetBottomText(locString.GetFormattedText());
		locString = new LocString("stats_screen", "ENTRY_POTION.top");
		locString.Add("Amount", StringHelper.RatioFormat(instance.GetTotalUnlockedPotions(), SaveManager.GetUnlockablePotionCount()));
		_potionEntry.SetTopText(locString.GetFormattedText() ?? "");
		locString = new LocString("stats_screen", "ENTRY_POTION.bottom");
		locString.Add("Amount", ModelDb.AllPotions.Count());
		_potionEntry.SetBottomText(locString.GetFormattedText());
		locString = new LocString("stats_screen", "ENTRY_EVENTS.top");
		locString.Add("Amount", "N/A");
		_eventsEntry.SetTopText(locString.GetFormattedText());
		locString = new LocString("stats_screen", "ENTRY_EVENTS.bottom");
		locString.Add("Amount", StringHelper.RatioFormat(progressSave.DiscoveredEvents.Count, ModelDb.AllEvents.Count()));
		_eventsEntry.SetBottomText(locString.GetFormattedText());
		locString = new LocString("stats_screen", "ENTRY_STREAK.top");
		locString.Add("Amount", progressSave.BestWinStreak);
		_streakEntry.SetTopText(locString.GetFormattedText());
		if (aggregateAscensionProgress > 99999999)
		{
			locString = new LocString("stats_screen", "ENTRY_STREAK.bottom");
			locString.Add("Amount", 5m);
			_streakEntry.SetBottomText("[red]" + locString.GetFormattedText() + "[/red]");
		}
		((Node)(object)_characterStatContainer).FreeChildren();
		CreateCharacterSection(progressSave, ModelDb.Character<Ironclad>().Id);
		CreateCharacterSection(progressSave, ModelDb.Character<Silent>().Id);
		CreateCharacterSection(progressSave, ModelDb.Character<Regent>().Id);
		CreateCharacterSection(progressSave, ModelDb.Character<Necrobinder>().Id);
		CreateCharacterSection(progressSave, ModelDb.Character<Defect>().Id);
	}

	private void CreateCharacterSection(ProgressState progressSave, ModelId id)
	{
		CharacterStats statsForCharacter = progressSave.GetStatsForCharacter(id);
		if (statsForCharacter != null)
		{
			NCharacterStats child = NCharacterStats.Create(statsForCharacter);
			((Node)(object)_characterStatContainer).AddChildSafely((Node?)(object)child);
		}
	}

	private void SetupHoverTips()
	{
		_playtimeEntry.SetHoverTip(PlaytimeTip);
		int aggregateAscensionProgress = SaveManager.Instance.GetAggregateAscensionProgress();
		if (aggregateAscensionProgress > 0)
		{
			_winLossEntry.SetHoverTip(WinsLossesTip);
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Expected O, but got Unknown
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(4);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.CreateSection, new PropertyInfo((Type)24, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)4, StringName.op_Implicit("imgUrl"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.LoadStats, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetupHoverTips, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.CreateSection && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			NStatEntry nStatEntry = CreateSection(VariantUtils.ConvertTo<string>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<NStatEntry>(ref nStatEntry);
			return true;
		}
		if ((ref method) == MethodName.LoadStats && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			LoadStats();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetupHoverTips && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			SetupHoverTips();
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
		if ((ref method) == MethodName.CreateSection)
		{
			return true;
		}
		if ((ref method) == MethodName.LoadStats)
		{
			return true;
		}
		if ((ref method) == MethodName.SetupHoverTips)
		{
			return true;
		}
		return ((Control)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._gridContainer)
		{
			_gridContainer = VariantUtils.ConvertTo<Node>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._achievementsEntry)
		{
			_achievementsEntry = VariantUtils.ConvertTo<NStatEntry>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._playtimeEntry)
		{
			_playtimeEntry = VariantUtils.ConvertTo<NStatEntry>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._cardsEntry)
		{
			_cardsEntry = VariantUtils.ConvertTo<NStatEntry>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._winLossEntry)
		{
			_winLossEntry = VariantUtils.ConvertTo<NStatEntry>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._monsterEntry)
		{
			_monsterEntry = VariantUtils.ConvertTo<NStatEntry>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._relicEntry)
		{
			_relicEntry = VariantUtils.ConvertTo<NStatEntry>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._potionEntry)
		{
			_potionEntry = VariantUtils.ConvertTo<NStatEntry>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._eventsEntry)
		{
			_eventsEntry = VariantUtils.ConvertTo<NStatEntry>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._streakEntry)
		{
			_streakEntry = VariantUtils.ConvertTo<NStatEntry>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._characterStatContainer)
		{
			_characterStatContainer = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._screenTween)
		{
			_screenTween = VariantUtils.ConvertTo<Tween>(ref value);
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
		if ((ref name) == PropertyName.DefaultFocusedControl)
		{
			Control defaultFocusedControl = DefaultFocusedControl;
			value = VariantUtils.CreateFrom<Control>(ref defaultFocusedControl);
			return true;
		}
		if ((ref name) == PropertyName._gridContainer)
		{
			value = VariantUtils.CreateFrom<Node>(ref _gridContainer);
			return true;
		}
		if ((ref name) == PropertyName._achievementsEntry)
		{
			value = VariantUtils.CreateFrom<NStatEntry>(ref _achievementsEntry);
			return true;
		}
		if ((ref name) == PropertyName._playtimeEntry)
		{
			value = VariantUtils.CreateFrom<NStatEntry>(ref _playtimeEntry);
			return true;
		}
		if ((ref name) == PropertyName._cardsEntry)
		{
			value = VariantUtils.CreateFrom<NStatEntry>(ref _cardsEntry);
			return true;
		}
		if ((ref name) == PropertyName._winLossEntry)
		{
			value = VariantUtils.CreateFrom<NStatEntry>(ref _winLossEntry);
			return true;
		}
		if ((ref name) == PropertyName._monsterEntry)
		{
			value = VariantUtils.CreateFrom<NStatEntry>(ref _monsterEntry);
			return true;
		}
		if ((ref name) == PropertyName._relicEntry)
		{
			value = VariantUtils.CreateFrom<NStatEntry>(ref _relicEntry);
			return true;
		}
		if ((ref name) == PropertyName._potionEntry)
		{
			value = VariantUtils.CreateFrom<NStatEntry>(ref _potionEntry);
			return true;
		}
		if ((ref name) == PropertyName._eventsEntry)
		{
			value = VariantUtils.CreateFrom<NStatEntry>(ref _eventsEntry);
			return true;
		}
		if ((ref name) == PropertyName._streakEntry)
		{
			value = VariantUtils.CreateFrom<NStatEntry>(ref _streakEntry);
			return true;
		}
		if ((ref name) == PropertyName._characterStatContainer)
		{
			value = VariantUtils.CreateFrom<Control>(ref _characterStatContainer);
			return true;
		}
		if ((ref name) == PropertyName._screenTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _screenTween);
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
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._gridContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._achievementsEntry, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._playtimeEntry, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._cardsEntry, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._winLossEntry, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._monsterEntry, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._relicEntry, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._potionEntry, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._eventsEntry, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._streakEntry, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._characterStatContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._screenTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		((GodotObject)this).SaveGodotObjectData(info);
		info.AddProperty(PropertyName._gridContainer, Variant.From<Node>(ref _gridContainer));
		info.AddProperty(PropertyName._achievementsEntry, Variant.From<NStatEntry>(ref _achievementsEntry));
		info.AddProperty(PropertyName._playtimeEntry, Variant.From<NStatEntry>(ref _playtimeEntry));
		info.AddProperty(PropertyName._cardsEntry, Variant.From<NStatEntry>(ref _cardsEntry));
		info.AddProperty(PropertyName._winLossEntry, Variant.From<NStatEntry>(ref _winLossEntry));
		info.AddProperty(PropertyName._monsterEntry, Variant.From<NStatEntry>(ref _monsterEntry));
		info.AddProperty(PropertyName._relicEntry, Variant.From<NStatEntry>(ref _relicEntry));
		info.AddProperty(PropertyName._potionEntry, Variant.From<NStatEntry>(ref _potionEntry));
		info.AddProperty(PropertyName._eventsEntry, Variant.From<NStatEntry>(ref _eventsEntry));
		info.AddProperty(PropertyName._streakEntry, Variant.From<NStatEntry>(ref _streakEntry));
		info.AddProperty(PropertyName._characterStatContainer, Variant.From<Control>(ref _characterStatContainer));
		info.AddProperty(PropertyName._screenTween, Variant.From<Tween>(ref _screenTween));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._gridContainer, ref val))
		{
			_gridContainer = ((Variant)(ref val)).As<Node>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._achievementsEntry, ref val2))
		{
			_achievementsEntry = ((Variant)(ref val2)).As<NStatEntry>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._playtimeEntry, ref val3))
		{
			_playtimeEntry = ((Variant)(ref val3)).As<NStatEntry>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._cardsEntry, ref val4))
		{
			_cardsEntry = ((Variant)(ref val4)).As<NStatEntry>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._winLossEntry, ref val5))
		{
			_winLossEntry = ((Variant)(ref val5)).As<NStatEntry>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._monsterEntry, ref val6))
		{
			_monsterEntry = ((Variant)(ref val6)).As<NStatEntry>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName._relicEntry, ref val7))
		{
			_relicEntry = ((Variant)(ref val7)).As<NStatEntry>();
		}
		Variant val8 = default(Variant);
		if (info.TryGetProperty(PropertyName._potionEntry, ref val8))
		{
			_potionEntry = ((Variant)(ref val8)).As<NStatEntry>();
		}
		Variant val9 = default(Variant);
		if (info.TryGetProperty(PropertyName._eventsEntry, ref val9))
		{
			_eventsEntry = ((Variant)(ref val9)).As<NStatEntry>();
		}
		Variant val10 = default(Variant);
		if (info.TryGetProperty(PropertyName._streakEntry, ref val10))
		{
			_streakEntry = ((Variant)(ref val10)).As<NStatEntry>();
		}
		Variant val11 = default(Variant);
		if (info.TryGetProperty(PropertyName._characterStatContainer, ref val11))
		{
			_characterStatContainer = ((Variant)(ref val11)).As<Control>();
		}
		Variant val12 = default(Variant);
		if (info.TryGetProperty(PropertyName._screenTween, ref val12))
		{
			_screenTween = ((Variant)(ref val12)).As<Tween>();
		}
	}
}
