using System;
using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.Screens.Bestiary;
using MegaCrit.Sts2.Core.Nodes.Screens.CardLibrary;
using MegaCrit.Sts2.Core.Nodes.Screens.CharacterSelect;
using MegaCrit.Sts2.Core.Nodes.Screens.CustomRun;
using MegaCrit.Sts2.Core.Nodes.Screens.DailyRun;
using MegaCrit.Sts2.Core.Nodes.Screens.ModdingScreen;
using MegaCrit.Sts2.Core.Nodes.Screens.PotionLab;
using MegaCrit.Sts2.Core.Nodes.Screens.ProfileScreen;
using MegaCrit.Sts2.Core.Nodes.Screens.RelicCollection;
using MegaCrit.Sts2.Core.Nodes.Screens.RunHistoryScreen;
using MegaCrit.Sts2.Core.Nodes.Screens.Settings;
using MegaCrit.Sts2.Core.Nodes.Screens.StatsScreen;
using MegaCrit.Sts2.Core.Nodes.Screens.Timeline;

namespace MegaCrit.Sts2.Core.Nodes.Screens.MainMenu;

[ScriptPath("res://src/Core/Nodes/Screens/MainMenu/NMainMenuSubmenuStack.cs")]
public class NMainMenuSubmenuStack : NSubmenuStack
{
	public new class MethodName : NSubmenuStack.MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");
	}

	public new class PropertyName : NSubmenuStack.PropertyName
	{
		public static readonly StringName _settingsScreenScene = StringName.op_Implicit("_settingsScreenScene");

		public static readonly StringName _characterSelectScreenScene = StringName.op_Implicit("_characterSelectScreenScene");

		public static readonly StringName _singleplayerSubmenu = StringName.op_Implicit("_singleplayerSubmenu");

		public static readonly StringName _multiplayerSubmenu = StringName.op_Implicit("_multiplayerSubmenu");

		public static readonly StringName _multiplayerHostSubmenu = StringName.op_Implicit("_multiplayerHostSubmenu");

		public static readonly StringName _joinFriendSubmenu = StringName.op_Implicit("_joinFriendSubmenu");

		public static readonly StringName _characterSelectSubmenu = StringName.op_Implicit("_characterSelectSubmenu");

		public static readonly StringName _loadMultiplayerSubmenu = StringName.op_Implicit("_loadMultiplayerSubmenu");

		public static readonly StringName _compendiumSubmenu = StringName.op_Implicit("_compendiumSubmenu");

		public static readonly StringName _bestiarySubmenu = StringName.op_Implicit("_bestiarySubmenu");

		public static readonly StringName _relicCollectionSubmenu = StringName.op_Implicit("_relicCollectionSubmenu");

		public static readonly StringName _potionLabSubmenu = StringName.op_Implicit("_potionLabSubmenu");

		public static readonly StringName _cardLibrarySubmenu = StringName.op_Implicit("_cardLibrarySubmenu");

		public static readonly StringName _runHistorySubmenu = StringName.op_Implicit("_runHistorySubmenu");

		public static readonly StringName _statsScreen = StringName.op_Implicit("_statsScreen");

		public static readonly StringName _timelineScreen = StringName.op_Implicit("_timelineScreen");

		public static readonly StringName _settingsScreen = StringName.op_Implicit("_settingsScreen");

		public static readonly StringName _dailyScreen = StringName.op_Implicit("_dailyScreen");

		public static readonly StringName _dailyLoadScreen = StringName.op_Implicit("_dailyLoadScreen");

		public static readonly StringName _customRunScreen = StringName.op_Implicit("_customRunScreen");

		public static readonly StringName _customRunLoadScreen = StringName.op_Implicit("_customRunLoadScreen");

		public static readonly StringName _moddingScreen = StringName.op_Implicit("_moddingScreen");

		public static readonly StringName _profileScreen = StringName.op_Implicit("_profileScreen");
	}

	public new class SignalName : NSubmenuStack.SignalName
	{
	}

	[Export(/*Could not decode attribute arguments.*/)]
	private PackedScene _settingsScreenScene;

	[Export(/*Could not decode attribute arguments.*/)]
	private PackedScene _characterSelectScreenScene;

	private NSingleplayerSubmenu? _singleplayerSubmenu;

	private NMultiplayerSubmenu? _multiplayerSubmenu;

	private NMultiplayerHostSubmenu? _multiplayerHostSubmenu;

	private NJoinFriendScreen? _joinFriendSubmenu;

	private NCharacterSelectScreen? _characterSelectSubmenu;

	private NMultiplayerLoadGameScreen? _loadMultiplayerSubmenu;

	private NCompendiumSubmenu? _compendiumSubmenu;

	private NBestiary? _bestiarySubmenu;

	private NRelicCollection? _relicCollectionSubmenu;

	private NPotionLab? _potionLabSubmenu;

	private NCardLibrary? _cardLibrarySubmenu;

	private NRunHistory? _runHistorySubmenu;

	private NStatsScreen? _statsScreen;

	private NTimelineScreen? _timelineScreen;

	private NSettingsScreen? _settingsScreen;

	private NDailyRunScreen? _dailyScreen;

	private NDailyRunLoadScreen? _dailyLoadScreen;

	private NCustomRunScreen? _customRunScreen;

	private NCustomRunLoadScreen? _customRunLoadScreen;

	private NModdingScreen? _moddingScreen;

	private NProfileScreen? _profileScreen;

	public override void _Ready()
	{
		GetSubmenuType<NSettingsScreen>();
		GetSubmenuType<NCharacterSelectScreen>();
	}

	public override T PushSubmenuType<T>()
	{
		return (T)PushSubmenuType(typeof(T));
	}

	public override T GetSubmenuType<T>()
	{
		return (T)GetSubmenuType(typeof(T));
	}

	public override NSubmenu PushSubmenuType(Type type)
	{
		NSubmenu submenuType = GetSubmenuType(type);
		Push(submenuType);
		return submenuType;
	}

	public override NSubmenu GetSubmenuType(Type type)
	{
		if (type == typeof(NSingleplayerSubmenu))
		{
			if (_singleplayerSubmenu == null)
			{
				_singleplayerSubmenu = NSingleplayerSubmenu.Create();
				((CanvasItem)_singleplayerSubmenu).Visible = false;
				((Node)(object)this).AddChildSafely((Node?)(object)_singleplayerSubmenu);
			}
			return _singleplayerSubmenu;
		}
		if (type == typeof(NMultiplayerSubmenu))
		{
			if (_multiplayerSubmenu == null)
			{
				_multiplayerSubmenu = NMultiplayerSubmenu.Create();
				((CanvasItem)_multiplayerSubmenu).Visible = false;
				((Node)(object)this).AddChildSafely((Node?)(object)_multiplayerSubmenu);
			}
			return _multiplayerSubmenu;
		}
		if (type == typeof(NMultiplayerHostSubmenu))
		{
			if (_multiplayerHostSubmenu == null)
			{
				_multiplayerHostSubmenu = NMultiplayerHostSubmenu.Create();
				((CanvasItem)_multiplayerHostSubmenu).Visible = false;
				((Node)(object)this).AddChildSafely((Node?)(object)_multiplayerHostSubmenu);
			}
			return _multiplayerHostSubmenu;
		}
		if (type == typeof(NJoinFriendScreen))
		{
			if (_joinFriendSubmenu == null)
			{
				_joinFriendSubmenu = NJoinFriendScreen.Create();
				((CanvasItem)_joinFriendSubmenu).Visible = false;
				((Node)(object)this).AddChildSafely((Node?)(object)_joinFriendSubmenu);
			}
			return _joinFriendSubmenu;
		}
		if (type == typeof(NCharacterSelectScreen))
		{
			if (_characterSelectSubmenu == null)
			{
				_characterSelectSubmenu = _characterSelectScreenScene.Instantiate<NCharacterSelectScreen>((GenEditState)0);
				((CanvasItem)_characterSelectSubmenu).Visible = false;
				((Node)(object)this).AddChildSafely((Node?)(object)_characterSelectSubmenu);
			}
			return _characterSelectSubmenu;
		}
		if (type == typeof(NMultiplayerLoadGameScreen))
		{
			if (_loadMultiplayerSubmenu == null)
			{
				_loadMultiplayerSubmenu = NMultiplayerLoadGameScreen.Create();
				((CanvasItem)_loadMultiplayerSubmenu).Visible = false;
				((Node)(object)this).AddChildSafely((Node?)(object)_loadMultiplayerSubmenu);
			}
			return _loadMultiplayerSubmenu;
		}
		if (type == typeof(NCompendiumSubmenu))
		{
			if (_compendiumSubmenu == null)
			{
				_compendiumSubmenu = NCompendiumSubmenu.Create();
				((CanvasItem)_compendiumSubmenu).Visible = false;
				((Node)(object)this).AddChildSafely((Node?)(object)_compendiumSubmenu);
			}
			return _compendiumSubmenu;
		}
		if (type == typeof(NBestiary))
		{
			if (_bestiarySubmenu == null)
			{
				_bestiarySubmenu = NBestiary.Create();
				((CanvasItem)_bestiarySubmenu).Visible = false;
				((Node)(object)this).AddChildSafely((Node?)(object)_bestiarySubmenu);
			}
			return _bestiarySubmenu;
		}
		if (type == typeof(NRelicCollection))
		{
			if (_relicCollectionSubmenu == null)
			{
				_relicCollectionSubmenu = NRelicCollection.Create();
				((CanvasItem)_relicCollectionSubmenu).Visible = false;
				((Node)(object)this).AddChildSafely((Node?)(object)_relicCollectionSubmenu);
			}
			return _relicCollectionSubmenu;
		}
		if (type == typeof(NPotionLab))
		{
			if (_potionLabSubmenu == null)
			{
				_potionLabSubmenu = NPotionLab.Create();
				((CanvasItem)_potionLabSubmenu).Visible = false;
				((Node)(object)this).AddChildSafely((Node?)(object)_potionLabSubmenu);
			}
			return _potionLabSubmenu;
		}
		if (type == typeof(NMultiplayerHostSubmenu))
		{
			if (_multiplayerHostSubmenu == null)
			{
				_multiplayerHostSubmenu = NMultiplayerHostSubmenu.Create();
				((CanvasItem)_multiplayerHostSubmenu).Visible = false;
				((Node)(object)this).AddChildSafely((Node?)(object)_multiplayerHostSubmenu);
			}
			return _multiplayerHostSubmenu;
		}
		if (type == typeof(NCardLibrary))
		{
			if (_cardLibrarySubmenu == null)
			{
				_cardLibrarySubmenu = NCardLibrary.Create();
				((CanvasItem)_cardLibrarySubmenu).Visible = false;
				((Node)(object)this).AddChildSafely((Node?)(object)_cardLibrarySubmenu);
			}
			return _cardLibrarySubmenu;
		}
		if (type == typeof(NRunHistory))
		{
			if (_runHistorySubmenu == null)
			{
				_runHistorySubmenu = NRunHistory.Create();
				((CanvasItem)_runHistorySubmenu).Visible = false;
				((Node)(object)this).AddChildSafely((Node?)(object)_runHistorySubmenu);
			}
			return _runHistorySubmenu;
		}
		if (type == typeof(NStatsScreen))
		{
			if (_statsScreen == null)
			{
				_statsScreen = NStatsScreen.Create();
				((CanvasItem)_statsScreen).Visible = false;
				((Node)(object)this).AddChildSafely((Node?)(object)_statsScreen);
			}
			return _statsScreen;
		}
		if (type == typeof(NTimelineScreen))
		{
			if (_timelineScreen == null)
			{
				_timelineScreen = NTimelineScreen.Create();
				((CanvasItem)_timelineScreen).Visible = false;
				((Node)(object)this).AddChildSafely((Node?)(object)_timelineScreen);
			}
			return _timelineScreen;
		}
		if (type == typeof(NSettingsScreen))
		{
			if (_settingsScreen == null)
			{
				_settingsScreen = _settingsScreenScene.Instantiate<NSettingsScreen>((GenEditState)0);
				_settingsScreen.SetIsInRun(isInRun: false);
				((CanvasItem)_settingsScreen).Visible = false;
				((Node)(object)this).AddChildSafely((Node?)(object)_settingsScreen);
			}
			return _settingsScreen;
		}
		if (type == typeof(NDailyRunScreen))
		{
			if (_dailyScreen == null)
			{
				_dailyScreen = NDailyRunScreen.Create();
				((CanvasItem)_dailyScreen).Visible = false;
				((Node)(object)this).AddChildSafely((Node?)(object)_dailyScreen);
			}
			return _dailyScreen;
		}
		if (type == typeof(NDailyRunLoadScreen))
		{
			if (_dailyLoadScreen == null)
			{
				_dailyLoadScreen = NDailyRunLoadScreen.Create();
				((CanvasItem)_dailyLoadScreen).Visible = false;
				((Node)(object)this).AddChildSafely((Node?)(object)_dailyLoadScreen);
			}
			return _dailyLoadScreen;
		}
		if (type == typeof(NCustomRunScreen))
		{
			if (_customRunScreen == null)
			{
				_customRunScreen = NCustomRunScreen.Create();
				((CanvasItem)_customRunScreen).Visible = false;
				((Node)(object)this).AddChildSafely((Node?)(object)_customRunScreen);
			}
			return _customRunScreen;
		}
		if (type == typeof(NCustomRunLoadScreen))
		{
			if (_customRunLoadScreen == null)
			{
				_customRunLoadScreen = NCustomRunLoadScreen.Create();
				((CanvasItem)_customRunLoadScreen).Visible = false;
				((Node)(object)this).AddChildSafely((Node?)(object)_customRunLoadScreen);
			}
			return _customRunLoadScreen;
		}
		if (type == typeof(NModdingScreen))
		{
			if (_moddingScreen == null)
			{
				_moddingScreen = NModdingScreen.Create();
				((CanvasItem)_moddingScreen).Visible = false;
				((Node)(object)this).AddChildSafely((Node?)(object)_moddingScreen);
			}
			return _moddingScreen;
		}
		if (type == typeof(NProfileScreen))
		{
			if (_profileScreen == null)
			{
				_profileScreen = NProfileScreen.Create();
				((CanvasItem)_profileScreen).Visible = false;
				((Node)(object)this).AddChildSafely((Node?)(object)_profileScreen);
			}
			return _profileScreen;
		}
		throw new ArgumentException($"No such submenu {type} in main menu");
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<MethodInfo> GetGodotMethodList()
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
		return base.InvokeGodotClassMethod(in method, args, out ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if ((ref method) == MethodName._Ready)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._settingsScreenScene)
		{
			_settingsScreenScene = VariantUtils.ConvertTo<PackedScene>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._characterSelectScreenScene)
		{
			_characterSelectScreenScene = VariantUtils.ConvertTo<PackedScene>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._singleplayerSubmenu)
		{
			_singleplayerSubmenu = VariantUtils.ConvertTo<NSingleplayerSubmenu>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._multiplayerSubmenu)
		{
			_multiplayerSubmenu = VariantUtils.ConvertTo<NMultiplayerSubmenu>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._multiplayerHostSubmenu)
		{
			_multiplayerHostSubmenu = VariantUtils.ConvertTo<NMultiplayerHostSubmenu>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._joinFriendSubmenu)
		{
			_joinFriendSubmenu = VariantUtils.ConvertTo<NJoinFriendScreen>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._characterSelectSubmenu)
		{
			_characterSelectSubmenu = VariantUtils.ConvertTo<NCharacterSelectScreen>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._loadMultiplayerSubmenu)
		{
			_loadMultiplayerSubmenu = VariantUtils.ConvertTo<NMultiplayerLoadGameScreen>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._compendiumSubmenu)
		{
			_compendiumSubmenu = VariantUtils.ConvertTo<NCompendiumSubmenu>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._bestiarySubmenu)
		{
			_bestiarySubmenu = VariantUtils.ConvertTo<NBestiary>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._relicCollectionSubmenu)
		{
			_relicCollectionSubmenu = VariantUtils.ConvertTo<NRelicCollection>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._potionLabSubmenu)
		{
			_potionLabSubmenu = VariantUtils.ConvertTo<NPotionLab>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._cardLibrarySubmenu)
		{
			_cardLibrarySubmenu = VariantUtils.ConvertTo<NCardLibrary>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._runHistorySubmenu)
		{
			_runHistorySubmenu = VariantUtils.ConvertTo<NRunHistory>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._statsScreen)
		{
			_statsScreen = VariantUtils.ConvertTo<NStatsScreen>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._timelineScreen)
		{
			_timelineScreen = VariantUtils.ConvertTo<NTimelineScreen>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._settingsScreen)
		{
			_settingsScreen = VariantUtils.ConvertTo<NSettingsScreen>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._dailyScreen)
		{
			_dailyScreen = VariantUtils.ConvertTo<NDailyRunScreen>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._dailyLoadScreen)
		{
			_dailyLoadScreen = VariantUtils.ConvertTo<NDailyRunLoadScreen>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._customRunScreen)
		{
			_customRunScreen = VariantUtils.ConvertTo<NCustomRunScreen>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._customRunLoadScreen)
		{
			_customRunLoadScreen = VariantUtils.ConvertTo<NCustomRunLoadScreen>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._moddingScreen)
		{
			_moddingScreen = VariantUtils.ConvertTo<NModdingScreen>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._profileScreen)
		{
			_profileScreen = VariantUtils.ConvertTo<NProfileScreen>(ref value);
			return true;
		}
		return base.SetGodotClassPropertyValue(in name, in value);
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
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_0279: Unknown result type (might be due to invalid IL or missing references)
		//IL_0294: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName._settingsScreenScene)
		{
			value = VariantUtils.CreateFrom<PackedScene>(ref _settingsScreenScene);
			return true;
		}
		if ((ref name) == PropertyName._characterSelectScreenScene)
		{
			value = VariantUtils.CreateFrom<PackedScene>(ref _characterSelectScreenScene);
			return true;
		}
		if ((ref name) == PropertyName._singleplayerSubmenu)
		{
			value = VariantUtils.CreateFrom<NSingleplayerSubmenu>(ref _singleplayerSubmenu);
			return true;
		}
		if ((ref name) == PropertyName._multiplayerSubmenu)
		{
			value = VariantUtils.CreateFrom<NMultiplayerSubmenu>(ref _multiplayerSubmenu);
			return true;
		}
		if ((ref name) == PropertyName._multiplayerHostSubmenu)
		{
			value = VariantUtils.CreateFrom<NMultiplayerHostSubmenu>(ref _multiplayerHostSubmenu);
			return true;
		}
		if ((ref name) == PropertyName._joinFriendSubmenu)
		{
			value = VariantUtils.CreateFrom<NJoinFriendScreen>(ref _joinFriendSubmenu);
			return true;
		}
		if ((ref name) == PropertyName._characterSelectSubmenu)
		{
			value = VariantUtils.CreateFrom<NCharacterSelectScreen>(ref _characterSelectSubmenu);
			return true;
		}
		if ((ref name) == PropertyName._loadMultiplayerSubmenu)
		{
			value = VariantUtils.CreateFrom<NMultiplayerLoadGameScreen>(ref _loadMultiplayerSubmenu);
			return true;
		}
		if ((ref name) == PropertyName._compendiumSubmenu)
		{
			value = VariantUtils.CreateFrom<NCompendiumSubmenu>(ref _compendiumSubmenu);
			return true;
		}
		if ((ref name) == PropertyName._bestiarySubmenu)
		{
			value = VariantUtils.CreateFrom<NBestiary>(ref _bestiarySubmenu);
			return true;
		}
		if ((ref name) == PropertyName._relicCollectionSubmenu)
		{
			value = VariantUtils.CreateFrom<NRelicCollection>(ref _relicCollectionSubmenu);
			return true;
		}
		if ((ref name) == PropertyName._potionLabSubmenu)
		{
			value = VariantUtils.CreateFrom<NPotionLab>(ref _potionLabSubmenu);
			return true;
		}
		if ((ref name) == PropertyName._cardLibrarySubmenu)
		{
			value = VariantUtils.CreateFrom<NCardLibrary>(ref _cardLibrarySubmenu);
			return true;
		}
		if ((ref name) == PropertyName._runHistorySubmenu)
		{
			value = VariantUtils.CreateFrom<NRunHistory>(ref _runHistorySubmenu);
			return true;
		}
		if ((ref name) == PropertyName._statsScreen)
		{
			value = VariantUtils.CreateFrom<NStatsScreen>(ref _statsScreen);
			return true;
		}
		if ((ref name) == PropertyName._timelineScreen)
		{
			value = VariantUtils.CreateFrom<NTimelineScreen>(ref _timelineScreen);
			return true;
		}
		if ((ref name) == PropertyName._settingsScreen)
		{
			value = VariantUtils.CreateFrom<NSettingsScreen>(ref _settingsScreen);
			return true;
		}
		if ((ref name) == PropertyName._dailyScreen)
		{
			value = VariantUtils.CreateFrom<NDailyRunScreen>(ref _dailyScreen);
			return true;
		}
		if ((ref name) == PropertyName._dailyLoadScreen)
		{
			value = VariantUtils.CreateFrom<NDailyRunLoadScreen>(ref _dailyLoadScreen);
			return true;
		}
		if ((ref name) == PropertyName._customRunScreen)
		{
			value = VariantUtils.CreateFrom<NCustomRunScreen>(ref _customRunScreen);
			return true;
		}
		if ((ref name) == PropertyName._customRunLoadScreen)
		{
			value = VariantUtils.CreateFrom<NCustomRunLoadScreen>(ref _customRunLoadScreen);
			return true;
		}
		if ((ref name) == PropertyName._moddingScreen)
		{
			value = VariantUtils.CreateFrom<NModdingScreen>(ref _moddingScreen);
			return true;
		}
		if ((ref name) == PropertyName._profileScreen)
		{
			value = VariantUtils.CreateFrom<NProfileScreen>(ref _profileScreen);
			return true;
		}
		return base.GetGodotClassPropertyValue(in name, out value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_0271: Unknown result type (might be due to invalid IL or missing references)
		//IL_0292: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._settingsScreenScene, (PropertyHint)17, "PackedScene", (PropertyUsageFlags)4102, true));
		list.Add(new PropertyInfo((Type)24, PropertyName._characterSelectScreenScene, (PropertyHint)17, "PackedScene", (PropertyUsageFlags)4102, true));
		list.Add(new PropertyInfo((Type)24, PropertyName._singleplayerSubmenu, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._multiplayerSubmenu, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._multiplayerHostSubmenu, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._joinFriendSubmenu, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._characterSelectSubmenu, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._loadMultiplayerSubmenu, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._compendiumSubmenu, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._bestiarySubmenu, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._relicCollectionSubmenu, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._potionLabSubmenu, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._cardLibrarySubmenu, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._runHistorySubmenu, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._statsScreen, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._timelineScreen, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._settingsScreen, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._dailyScreen, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._dailyLoadScreen, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._customRunScreen, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._customRunLoadScreen, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._moddingScreen, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._profileScreen, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName._settingsScreenScene, Variant.From<PackedScene>(ref _settingsScreenScene));
		info.AddProperty(PropertyName._characterSelectScreenScene, Variant.From<PackedScene>(ref _characterSelectScreenScene));
		info.AddProperty(PropertyName._singleplayerSubmenu, Variant.From<NSingleplayerSubmenu>(ref _singleplayerSubmenu));
		info.AddProperty(PropertyName._multiplayerSubmenu, Variant.From<NMultiplayerSubmenu>(ref _multiplayerSubmenu));
		info.AddProperty(PropertyName._multiplayerHostSubmenu, Variant.From<NMultiplayerHostSubmenu>(ref _multiplayerHostSubmenu));
		info.AddProperty(PropertyName._joinFriendSubmenu, Variant.From<NJoinFriendScreen>(ref _joinFriendSubmenu));
		info.AddProperty(PropertyName._characterSelectSubmenu, Variant.From<NCharacterSelectScreen>(ref _characterSelectSubmenu));
		info.AddProperty(PropertyName._loadMultiplayerSubmenu, Variant.From<NMultiplayerLoadGameScreen>(ref _loadMultiplayerSubmenu));
		info.AddProperty(PropertyName._compendiumSubmenu, Variant.From<NCompendiumSubmenu>(ref _compendiumSubmenu));
		info.AddProperty(PropertyName._bestiarySubmenu, Variant.From<NBestiary>(ref _bestiarySubmenu));
		info.AddProperty(PropertyName._relicCollectionSubmenu, Variant.From<NRelicCollection>(ref _relicCollectionSubmenu));
		info.AddProperty(PropertyName._potionLabSubmenu, Variant.From<NPotionLab>(ref _potionLabSubmenu));
		info.AddProperty(PropertyName._cardLibrarySubmenu, Variant.From<NCardLibrary>(ref _cardLibrarySubmenu));
		info.AddProperty(PropertyName._runHistorySubmenu, Variant.From<NRunHistory>(ref _runHistorySubmenu));
		info.AddProperty(PropertyName._statsScreen, Variant.From<NStatsScreen>(ref _statsScreen));
		info.AddProperty(PropertyName._timelineScreen, Variant.From<NTimelineScreen>(ref _timelineScreen));
		info.AddProperty(PropertyName._settingsScreen, Variant.From<NSettingsScreen>(ref _settingsScreen));
		info.AddProperty(PropertyName._dailyScreen, Variant.From<NDailyRunScreen>(ref _dailyScreen));
		info.AddProperty(PropertyName._dailyLoadScreen, Variant.From<NDailyRunLoadScreen>(ref _dailyLoadScreen));
		info.AddProperty(PropertyName._customRunScreen, Variant.From<NCustomRunScreen>(ref _customRunScreen));
		info.AddProperty(PropertyName._customRunLoadScreen, Variant.From<NCustomRunLoadScreen>(ref _customRunLoadScreen));
		info.AddProperty(PropertyName._moddingScreen, Variant.From<NModdingScreen>(ref _moddingScreen));
		info.AddProperty(PropertyName._profileScreen, Variant.From<NProfileScreen>(ref _profileScreen));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._settingsScreenScene, ref val))
		{
			_settingsScreenScene = ((Variant)(ref val)).As<PackedScene>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._characterSelectScreenScene, ref val2))
		{
			_characterSelectScreenScene = ((Variant)(ref val2)).As<PackedScene>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._singleplayerSubmenu, ref val3))
		{
			_singleplayerSubmenu = ((Variant)(ref val3)).As<NSingleplayerSubmenu>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._multiplayerSubmenu, ref val4))
		{
			_multiplayerSubmenu = ((Variant)(ref val4)).As<NMultiplayerSubmenu>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._multiplayerHostSubmenu, ref val5))
		{
			_multiplayerHostSubmenu = ((Variant)(ref val5)).As<NMultiplayerHostSubmenu>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._joinFriendSubmenu, ref val6))
		{
			_joinFriendSubmenu = ((Variant)(ref val6)).As<NJoinFriendScreen>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName._characterSelectSubmenu, ref val7))
		{
			_characterSelectSubmenu = ((Variant)(ref val7)).As<NCharacterSelectScreen>();
		}
		Variant val8 = default(Variant);
		if (info.TryGetProperty(PropertyName._loadMultiplayerSubmenu, ref val8))
		{
			_loadMultiplayerSubmenu = ((Variant)(ref val8)).As<NMultiplayerLoadGameScreen>();
		}
		Variant val9 = default(Variant);
		if (info.TryGetProperty(PropertyName._compendiumSubmenu, ref val9))
		{
			_compendiumSubmenu = ((Variant)(ref val9)).As<NCompendiumSubmenu>();
		}
		Variant val10 = default(Variant);
		if (info.TryGetProperty(PropertyName._bestiarySubmenu, ref val10))
		{
			_bestiarySubmenu = ((Variant)(ref val10)).As<NBestiary>();
		}
		Variant val11 = default(Variant);
		if (info.TryGetProperty(PropertyName._relicCollectionSubmenu, ref val11))
		{
			_relicCollectionSubmenu = ((Variant)(ref val11)).As<NRelicCollection>();
		}
		Variant val12 = default(Variant);
		if (info.TryGetProperty(PropertyName._potionLabSubmenu, ref val12))
		{
			_potionLabSubmenu = ((Variant)(ref val12)).As<NPotionLab>();
		}
		Variant val13 = default(Variant);
		if (info.TryGetProperty(PropertyName._cardLibrarySubmenu, ref val13))
		{
			_cardLibrarySubmenu = ((Variant)(ref val13)).As<NCardLibrary>();
		}
		Variant val14 = default(Variant);
		if (info.TryGetProperty(PropertyName._runHistorySubmenu, ref val14))
		{
			_runHistorySubmenu = ((Variant)(ref val14)).As<NRunHistory>();
		}
		Variant val15 = default(Variant);
		if (info.TryGetProperty(PropertyName._statsScreen, ref val15))
		{
			_statsScreen = ((Variant)(ref val15)).As<NStatsScreen>();
		}
		Variant val16 = default(Variant);
		if (info.TryGetProperty(PropertyName._timelineScreen, ref val16))
		{
			_timelineScreen = ((Variant)(ref val16)).As<NTimelineScreen>();
		}
		Variant val17 = default(Variant);
		if (info.TryGetProperty(PropertyName._settingsScreen, ref val17))
		{
			_settingsScreen = ((Variant)(ref val17)).As<NSettingsScreen>();
		}
		Variant val18 = default(Variant);
		if (info.TryGetProperty(PropertyName._dailyScreen, ref val18))
		{
			_dailyScreen = ((Variant)(ref val18)).As<NDailyRunScreen>();
		}
		Variant val19 = default(Variant);
		if (info.TryGetProperty(PropertyName._dailyLoadScreen, ref val19))
		{
			_dailyLoadScreen = ((Variant)(ref val19)).As<NDailyRunLoadScreen>();
		}
		Variant val20 = default(Variant);
		if (info.TryGetProperty(PropertyName._customRunScreen, ref val20))
		{
			_customRunScreen = ((Variant)(ref val20)).As<NCustomRunScreen>();
		}
		Variant val21 = default(Variant);
		if (info.TryGetProperty(PropertyName._customRunLoadScreen, ref val21))
		{
			_customRunLoadScreen = ((Variant)(ref val21)).As<NCustomRunLoadScreen>();
		}
		Variant val22 = default(Variant);
		if (info.TryGetProperty(PropertyName._moddingScreen, ref val22))
		{
			_moddingScreen = ((Variant)(ref val22)).As<NModdingScreen>();
		}
		Variant val23 = default(Variant);
		if (info.TryGetProperty(PropertyName._profileScreen, ref val23))
		{
			_profileScreen = ((Variant)(ref val23)).As<NProfileScreen>();
		}
	}
}
