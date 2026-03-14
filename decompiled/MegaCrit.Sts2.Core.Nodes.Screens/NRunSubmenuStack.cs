using System;
using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.Screens.Bestiary;
using MegaCrit.Sts2.Core.Nodes.Screens.CardLibrary;
using MegaCrit.Sts2.Core.Nodes.Screens.MainMenu;
using MegaCrit.Sts2.Core.Nodes.Screens.PauseMenu;
using MegaCrit.Sts2.Core.Nodes.Screens.PotionLab;
using MegaCrit.Sts2.Core.Nodes.Screens.RelicCollection;
using MegaCrit.Sts2.Core.Nodes.Screens.RunHistoryScreen;
using MegaCrit.Sts2.Core.Nodes.Screens.Settings;
using MegaCrit.Sts2.Core.Nodes.Screens.StatsScreen;

namespace MegaCrit.Sts2.Core.Nodes.Screens;

[ScriptPath("res://src/Core/Nodes/Screens/NRunSubmenuStack.cs")]
public class NRunSubmenuStack : NSubmenuStack
{
	public new class MethodName : NSubmenuStack.MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");
	}

	public new class PropertyName : NSubmenuStack.PropertyName
	{
		public static readonly StringName _settingsScreenScene = StringName.op_Implicit("_settingsScreenScene");

		public static readonly StringName _pauseMenuScene = StringName.op_Implicit("_pauseMenuScene");

		public static readonly StringName _statsScreenScene = StringName.op_Implicit("_statsScreenScene");

		public static readonly StringName _runHistoryScreenScene = StringName.op_Implicit("_runHistoryScreenScene");

		public static readonly StringName _compendiumSubmenu = StringName.op_Implicit("_compendiumSubmenu");

		public static readonly StringName _bestiarySubmenu = StringName.op_Implicit("_bestiarySubmenu");

		public static readonly StringName _relicCollectionSubmenu = StringName.op_Implicit("_relicCollectionSubmenu");

		public static readonly StringName _potionLabSubmenu = StringName.op_Implicit("_potionLabSubmenu");

		public static readonly StringName _cardLibrarySubmenu = StringName.op_Implicit("_cardLibrarySubmenu");

		public static readonly StringName _runHistoryScreen = StringName.op_Implicit("_runHistoryScreen");

		public static readonly StringName _settingsScreen = StringName.op_Implicit("_settingsScreen");

		public static readonly StringName _statsScreen = StringName.op_Implicit("_statsScreen");

		public static readonly StringName _pauseMenu = StringName.op_Implicit("_pauseMenu");
	}

	public new class SignalName : NSubmenuStack.SignalName
	{
	}

	[Export(/*Could not decode attribute arguments.*/)]
	private PackedScene _settingsScreenScene;

	[Export(/*Could not decode attribute arguments.*/)]
	private PackedScene _pauseMenuScene;

	[Export(/*Could not decode attribute arguments.*/)]
	private PackedScene _statsScreenScene;

	[Export(/*Could not decode attribute arguments.*/)]
	private PackedScene _runHistoryScreenScene;

	private NCompendiumSubmenu? _compendiumSubmenu;

	private NBestiary? _bestiarySubmenu;

	private NRelicCollection? _relicCollectionSubmenu;

	private NPotionLab? _potionLabSubmenu;

	private NCardLibrary? _cardLibrarySubmenu;

	private NRunHistory? _runHistoryScreen;

	private NSettingsScreen? _settingsScreen;

	private NStatsScreen? _statsScreen;

	private NPauseMenu? _pauseMenu;

	public override void _Ready()
	{
		GetSubmenuType<NSettingsScreen>();
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
			if (_runHistoryScreen == null)
			{
				_runHistoryScreen = _runHistoryScreenScene.Instantiate<NRunHistory>((GenEditState)0);
				((CanvasItem)_runHistoryScreen).Visible = false;
				((Node)(object)this).AddChildSafely((Node?)(object)_runHistoryScreen);
			}
			return _runHistoryScreen;
		}
		if (type == typeof(NSettingsScreen))
		{
			if (_settingsScreen == null)
			{
				_settingsScreen = _settingsScreenScene.Instantiate<NSettingsScreen>((GenEditState)0);
				_settingsScreen.SetIsInRun(isInRun: true);
				((CanvasItem)_settingsScreen).Visible = false;
				((Node)(object)this).AddChildSafely((Node?)(object)_settingsScreen);
			}
			return _settingsScreen;
		}
		if (type == typeof(NStatsScreen))
		{
			if (_statsScreen == null)
			{
				_statsScreen = _statsScreenScene.Instantiate<NStatsScreen>((GenEditState)0);
				((CanvasItem)_statsScreen).Visible = false;
				((Node)(object)this).AddChildSafely((Node?)(object)_statsScreen);
			}
			return _statsScreen;
		}
		if (type == typeof(NPauseMenu))
		{
			if (_pauseMenu == null)
			{
				_pauseMenu = _pauseMenuScene.Instantiate<NPauseMenu>((GenEditState)0);
				((CanvasItem)_pauseMenu).Visible = false;
				((Node)(object)this).AddChildSafely((Node?)(object)_pauseMenu);
			}
			return _pauseMenu;
		}
		throw new ArgumentException($"No such submenu of type {type} in run");
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
		if ((ref name) == PropertyName._pauseMenuScene)
		{
			_pauseMenuScene = VariantUtils.ConvertTo<PackedScene>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._statsScreenScene)
		{
			_statsScreenScene = VariantUtils.ConvertTo<PackedScene>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._runHistoryScreenScene)
		{
			_runHistoryScreenScene = VariantUtils.ConvertTo<PackedScene>(ref value);
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
		if ((ref name) == PropertyName._runHistoryScreen)
		{
			_runHistoryScreen = VariantUtils.ConvertTo<NRunHistory>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._settingsScreen)
		{
			_settingsScreen = VariantUtils.ConvertTo<NSettingsScreen>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._statsScreen)
		{
			_statsScreen = VariantUtils.ConvertTo<NStatsScreen>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._pauseMenu)
		{
			_pauseMenu = VariantUtils.ConvertTo<NPauseMenu>(ref value);
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
		if ((ref name) == PropertyName._settingsScreenScene)
		{
			value = VariantUtils.CreateFrom<PackedScene>(ref _settingsScreenScene);
			return true;
		}
		if ((ref name) == PropertyName._pauseMenuScene)
		{
			value = VariantUtils.CreateFrom<PackedScene>(ref _pauseMenuScene);
			return true;
		}
		if ((ref name) == PropertyName._statsScreenScene)
		{
			value = VariantUtils.CreateFrom<PackedScene>(ref _statsScreenScene);
			return true;
		}
		if ((ref name) == PropertyName._runHistoryScreenScene)
		{
			value = VariantUtils.CreateFrom<PackedScene>(ref _runHistoryScreenScene);
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
		if ((ref name) == PropertyName._runHistoryScreen)
		{
			value = VariantUtils.CreateFrom<NRunHistory>(ref _runHistoryScreen);
			return true;
		}
		if ((ref name) == PropertyName._settingsScreen)
		{
			value = VariantUtils.CreateFrom<NSettingsScreen>(ref _settingsScreen);
			return true;
		}
		if ((ref name) == PropertyName._statsScreen)
		{
			value = VariantUtils.CreateFrom<NStatsScreen>(ref _statsScreen);
			return true;
		}
		if ((ref name) == PropertyName._pauseMenu)
		{
			value = VariantUtils.CreateFrom<NPauseMenu>(ref _pauseMenu);
			return true;
		}
		return base.GetGodotClassPropertyValue(in name, out value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._settingsScreenScene, (PropertyHint)17, "PackedScene", (PropertyUsageFlags)4102, true));
		list.Add(new PropertyInfo((Type)24, PropertyName._pauseMenuScene, (PropertyHint)17, "PackedScene", (PropertyUsageFlags)4102, true));
		list.Add(new PropertyInfo((Type)24, PropertyName._statsScreenScene, (PropertyHint)17, "PackedScene", (PropertyUsageFlags)4102, true));
		list.Add(new PropertyInfo((Type)24, PropertyName._runHistoryScreenScene, (PropertyHint)17, "PackedScene", (PropertyUsageFlags)4102, true));
		list.Add(new PropertyInfo((Type)24, PropertyName._compendiumSubmenu, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._bestiarySubmenu, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._relicCollectionSubmenu, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._potionLabSubmenu, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._cardLibrarySubmenu, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._runHistoryScreen, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._settingsScreen, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._statsScreen, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._pauseMenu, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName._settingsScreenScene, Variant.From<PackedScene>(ref _settingsScreenScene));
		info.AddProperty(PropertyName._pauseMenuScene, Variant.From<PackedScene>(ref _pauseMenuScene));
		info.AddProperty(PropertyName._statsScreenScene, Variant.From<PackedScene>(ref _statsScreenScene));
		info.AddProperty(PropertyName._runHistoryScreenScene, Variant.From<PackedScene>(ref _runHistoryScreenScene));
		info.AddProperty(PropertyName._compendiumSubmenu, Variant.From<NCompendiumSubmenu>(ref _compendiumSubmenu));
		info.AddProperty(PropertyName._bestiarySubmenu, Variant.From<NBestiary>(ref _bestiarySubmenu));
		info.AddProperty(PropertyName._relicCollectionSubmenu, Variant.From<NRelicCollection>(ref _relicCollectionSubmenu));
		info.AddProperty(PropertyName._potionLabSubmenu, Variant.From<NPotionLab>(ref _potionLabSubmenu));
		info.AddProperty(PropertyName._cardLibrarySubmenu, Variant.From<NCardLibrary>(ref _cardLibrarySubmenu));
		info.AddProperty(PropertyName._runHistoryScreen, Variant.From<NRunHistory>(ref _runHistoryScreen));
		info.AddProperty(PropertyName._settingsScreen, Variant.From<NSettingsScreen>(ref _settingsScreen));
		info.AddProperty(PropertyName._statsScreen, Variant.From<NStatsScreen>(ref _statsScreen));
		info.AddProperty(PropertyName._pauseMenu, Variant.From<NPauseMenu>(ref _pauseMenu));
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
		if (info.TryGetProperty(PropertyName._pauseMenuScene, ref val2))
		{
			_pauseMenuScene = ((Variant)(ref val2)).As<PackedScene>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._statsScreenScene, ref val3))
		{
			_statsScreenScene = ((Variant)(ref val3)).As<PackedScene>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._runHistoryScreenScene, ref val4))
		{
			_runHistoryScreenScene = ((Variant)(ref val4)).As<PackedScene>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._compendiumSubmenu, ref val5))
		{
			_compendiumSubmenu = ((Variant)(ref val5)).As<NCompendiumSubmenu>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._bestiarySubmenu, ref val6))
		{
			_bestiarySubmenu = ((Variant)(ref val6)).As<NBestiary>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName._relicCollectionSubmenu, ref val7))
		{
			_relicCollectionSubmenu = ((Variant)(ref val7)).As<NRelicCollection>();
		}
		Variant val8 = default(Variant);
		if (info.TryGetProperty(PropertyName._potionLabSubmenu, ref val8))
		{
			_potionLabSubmenu = ((Variant)(ref val8)).As<NPotionLab>();
		}
		Variant val9 = default(Variant);
		if (info.TryGetProperty(PropertyName._cardLibrarySubmenu, ref val9))
		{
			_cardLibrarySubmenu = ((Variant)(ref val9)).As<NCardLibrary>();
		}
		Variant val10 = default(Variant);
		if (info.TryGetProperty(PropertyName._runHistoryScreen, ref val10))
		{
			_runHistoryScreen = ((Variant)(ref val10)).As<NRunHistory>();
		}
		Variant val11 = default(Variant);
		if (info.TryGetProperty(PropertyName._settingsScreen, ref val11))
		{
			_settingsScreen = ((Variant)(ref val11)).As<NSettingsScreen>();
		}
		Variant val12 = default(Variant);
		if (info.TryGetProperty(PropertyName._statsScreen, ref val12))
		{
			_statsScreen = ((Variant)(ref val12)).As<NStatsScreen>();
		}
		Variant val13 = default(Variant);
		if (info.TryGetProperty(PropertyName._pauseMenu, ref val13))
		{
			_pauseMenu = ((Variant)(ref val13)).As<NPauseMenu>();
		}
	}
}
