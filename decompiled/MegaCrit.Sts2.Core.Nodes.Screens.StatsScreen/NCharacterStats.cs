using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.Nodes.Screens.StatsScreen;

[ScriptPath("res://src/Core/Nodes/Screens/StatsScreen/NCharacterStats.cs")]
public class NCharacterStats : Node
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName LoadStats = StringName.op_Implicit("LoadStats");

		public static readonly StringName CreateSection = StringName.op_Implicit("CreateSection");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName _characterIcon = StringName.op_Implicit("_characterIcon");

		public static readonly StringName _statsContainer = StringName.op_Implicit("_statsContainer");

		public static readonly StringName _nameLabel = StringName.op_Implicit("_nameLabel");

		public static readonly StringName _unlocksLabel = StringName.op_Implicit("_unlocksLabel");

		public static readonly StringName _playtimeEntry = StringName.op_Implicit("_playtimeEntry");

		public static readonly StringName _winLossEntry = StringName.op_Implicit("_winLossEntry");

		public static readonly StringName _streakEntry = StringName.op_Implicit("_streakEntry");
	}

	public class SignalName : SignalName
	{
	}

	private static readonly string _playtimeIconPath = ImageHelper.GetImagePath("atlases/stats_screen_atlas.sprites/stats_clock.tres");

	private static readonly string _winLossIconPath = ImageHelper.GetImagePath("atlases/stats_screen_atlas.sprites/stats_swords.tres");

	private static readonly string _chainIconPath = ImageHelper.GetImagePath("atlases/stats_screen_atlas.sprites/stats_chain.tres");

	private CharacterStats _characterStats;

	private Control _characterIcon;

	private Node _statsContainer;

	private MegaLabel _nameLabel;

	private MegaLabel _unlocksLabel;

	private NStatEntry _playtimeEntry;

	private NStatEntry _winLossEntry;

	private NStatEntry _streakEntry;

	public static string[] AssetPaths => new string[4] { ScenePath, _playtimeIconPath, _winLossIconPath, _chainIconPath };

	private static string ScenePath => SceneHelper.GetScenePath("screens/stats_screen/character_stats");

	public static NCharacterStats Create(CharacterStats characterStats)
	{
		NCharacterStats nCharacterStats = PreloadManager.Cache.GetScene(ScenePath).Instantiate<NCharacterStats>((GenEditState)0);
		nCharacterStats._characterStats = characterStats;
		return nCharacterStats;
	}

	public override void _Ready()
	{
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		CharacterModel byId = ModelDb.GetById<CharacterModel>(_characterStats.Id);
		_characterIcon = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%CharacterIcon"));
		((Node)(object)_characterIcon).AddChildSafely((Node?)(object)byId.Icon);
		_statsContainer = ((Node)this).GetNode<Node>(NodePath.op_Implicit("%StatsContainer"));
		_playtimeEntry = CreateSection(_playtimeIconPath);
		_winLossEntry = CreateSection(_winLossIconPath);
		_streakEntry = CreateSection(_chainIconPath);
		_nameLabel = ((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%NameLabel"));
		_unlocksLabel = ((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%UnlocksLabel"));
		_nameLabel.SetTextAutoSize(byId.Title.GetRawText());
		((Control)_nameLabel).AddThemeColorOverride(ThemeConstants.Label.fontColor, byId.NameColor);
		LoadStats();
	}

	private void LoadStats()
	{
		((CanvasItem)_unlocksLabel).Visible = false;
		LocString locString = new LocString("stats_screen", "ENTRY_CHAR_PLAYTIME.top");
		locString.Add("Playtime", TimeFormatting.Format(_characterStats.Playtime));
		_playtimeEntry.SetTopText(locString.GetFormattedText());
		if (_characterStats.FastestWinTime >= 0)
		{
			locString = new LocString("stats_screen", "ENTRY_CHAR_PLAYTIME.bottom");
			locString.Add("FastestWin", TimeFormatting.Format(_characterStats.FastestWinTime));
			_playtimeEntry.SetBottomText(locString.GetFormattedText());
		}
		locString = new LocString("stats_screen", "ENTRY_CHAR_WIN_LOSS.top");
		if (_characterStats.MaxAscension > 0)
		{
			locString.Add("Amount", _characterStats.MaxAscension);
			_winLossEntry.SetTopText("[red]" + locString.GetFormattedText() + "[/red]");
		}
		locString = new LocString("stats_screen", "ENTRY_CHAR_WIN_LOSS.bottom");
		locString.Add("Wins", _characterStats.TotalWins);
		locString.Add("Losses", _characterStats.TotalLosses);
		_winLossEntry.SetBottomText(locString.GetFormattedText());
		locString = new LocString("stats_screen", "ENTRY_CHAR_STREAK.top");
		locString.Add("Amount", _characterStats.CurrentWinStreak);
		_streakEntry.SetTopText(locString.GetFormattedText());
		locString = new LocString("stats_screen", "ENTRY_CHAR_STREAK.bottom");
		locString.Add("Amount", _characterStats.BestWinStreak);
		_streakEntry.SetBottomText(locString.GetFormattedText());
	}

	private NStatEntry CreateSection(string imgUrl)
	{
		NStatEntry nStatEntry = NStatEntry.Create(imgUrl);
		_statsContainer.AddChildSafely((Node?)(object)nStatEntry);
		return nStatEntry;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Expected O, but got Unknown
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(3);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.LoadStats, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.CreateSection, new PropertyInfo((Type)24, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)4, StringName.op_Implicit("imgUrl"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.LoadStats && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			LoadStats();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.CreateSection && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			NStatEntry nStatEntry = CreateSection(VariantUtils.ConvertTo<string>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<NStatEntry>(ref nStatEntry);
			return true;
		}
		return ((Node)this).InvokeGodotClassMethod(ref method, args, ref ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if ((ref method) == MethodName._Ready)
		{
			return true;
		}
		if ((ref method) == MethodName.LoadStats)
		{
			return true;
		}
		if ((ref method) == MethodName.CreateSection)
		{
			return true;
		}
		return ((Node)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._characterIcon)
		{
			_characterIcon = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._statsContainer)
		{
			_statsContainer = VariantUtils.ConvertTo<Node>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._nameLabel)
		{
			_nameLabel = VariantUtils.ConvertTo<MegaLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._unlocksLabel)
		{
			_unlocksLabel = VariantUtils.ConvertTo<MegaLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._playtimeEntry)
		{
			_playtimeEntry = VariantUtils.ConvertTo<NStatEntry>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._winLossEntry)
		{
			_winLossEntry = VariantUtils.ConvertTo<NStatEntry>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._streakEntry)
		{
			_streakEntry = VariantUtils.ConvertTo<NStatEntry>(ref value);
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
		if ((ref name) == PropertyName._characterIcon)
		{
			value = VariantUtils.CreateFrom<Control>(ref _characterIcon);
			return true;
		}
		if ((ref name) == PropertyName._statsContainer)
		{
			value = VariantUtils.CreateFrom<Node>(ref _statsContainer);
			return true;
		}
		if ((ref name) == PropertyName._nameLabel)
		{
			value = VariantUtils.CreateFrom<MegaLabel>(ref _nameLabel);
			return true;
		}
		if ((ref name) == PropertyName._unlocksLabel)
		{
			value = VariantUtils.CreateFrom<MegaLabel>(ref _unlocksLabel);
			return true;
		}
		if ((ref name) == PropertyName._playtimeEntry)
		{
			value = VariantUtils.CreateFrom<NStatEntry>(ref _playtimeEntry);
			return true;
		}
		if ((ref name) == PropertyName._winLossEntry)
		{
			value = VariantUtils.CreateFrom<NStatEntry>(ref _winLossEntry);
			return true;
		}
		if ((ref name) == PropertyName._streakEntry)
		{
			value = VariantUtils.CreateFrom<NStatEntry>(ref _streakEntry);
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
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._characterIcon, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._statsContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._nameLabel, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._unlocksLabel, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._playtimeEntry, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._winLossEntry, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._streakEntry, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		((GodotObject)this).SaveGodotObjectData(info);
		info.AddProperty(PropertyName._characterIcon, Variant.From<Control>(ref _characterIcon));
		info.AddProperty(PropertyName._statsContainer, Variant.From<Node>(ref _statsContainer));
		info.AddProperty(PropertyName._nameLabel, Variant.From<MegaLabel>(ref _nameLabel));
		info.AddProperty(PropertyName._unlocksLabel, Variant.From<MegaLabel>(ref _unlocksLabel));
		info.AddProperty(PropertyName._playtimeEntry, Variant.From<NStatEntry>(ref _playtimeEntry));
		info.AddProperty(PropertyName._winLossEntry, Variant.From<NStatEntry>(ref _winLossEntry));
		info.AddProperty(PropertyName._streakEntry, Variant.From<NStatEntry>(ref _streakEntry));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._characterIcon, ref val))
		{
			_characterIcon = ((Variant)(ref val)).As<Control>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._statsContainer, ref val2))
		{
			_statsContainer = ((Variant)(ref val2)).As<Node>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._nameLabel, ref val3))
		{
			_nameLabel = ((Variant)(ref val3)).As<MegaLabel>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._unlocksLabel, ref val4))
		{
			_unlocksLabel = ((Variant)(ref val4)).As<MegaLabel>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._playtimeEntry, ref val5))
		{
			_playtimeEntry = ((Variant)(ref val5)).As<NStatEntry>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._winLossEntry, ref val6))
		{
			_winLossEntry = ((Variant)(ref val6)).As<NStatEntry>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName._streakEntry, ref val7))
		{
			_streakEntry = ((Variant)(ref val7)).As<NStatEntry>();
		}
	}
}
