using System;
using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Screens.MainMenu;
using MegaCrit.Sts2.Core.Nodes.Screens.ScreenContext;
using MegaCrit.Sts2.Core.Nodes.Screens.Settings;
using MegaCrit.Sts2.Core.TestSupport;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.Nodes.Screens.StatsScreen;

[ScriptPath("res://src/Core/Nodes/Screens/StatsScreen/NStatsScreen.cs")]
public class NStatsScreen : NSubmenu
{
	public new class MethodName : NSubmenu.MethodName
	{
		public static readonly StringName Create = StringName.op_Implicit("Create");

		public new static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public new static readonly StringName OnSubmenuOpened = StringName.op_Implicit("OnSubmenuOpened");

		public static readonly StringName OpenStatsMenu = StringName.op_Implicit("OpenStatsMenu");

		public static readonly StringName OpenAchievementsMenu = StringName.op_Implicit("OpenAchievementsMenu");
	}

	public new class PropertyName : NSubmenu.PropertyName
	{
		public new static readonly StringName InitialFocusedControl = StringName.op_Implicit("InitialFocusedControl");

		public static readonly StringName _statsTabManager = StringName.op_Implicit("_statsTabManager");

		public static readonly StringName _statsTab = StringName.op_Implicit("_statsTab");

		public static readonly StringName _achievementsTab = StringName.op_Implicit("_achievementsTab");

		public static readonly StringName _statsGrid = StringName.op_Implicit("_statsGrid");

		public static readonly StringName _screenTween = StringName.op_Implicit("_screenTween");
	}

	public new class SignalName : NSubmenu.SignalName
	{
	}

	private static readonly string _scenePath = SceneHelper.GetScenePath("screens/stats_screen/stats_screen");

	private NStatsTabManager _statsTabManager;

	private NSettingsTab _statsTab;

	private NSettingsTab _achievementsTab;

	private NGeneralStatsGrid _statsGrid;

	private Tween? _screenTween;

	public static string[] AssetPaths
	{
		get
		{
			string scenePath = _scenePath;
			string[] assetPaths = NGeneralStatsGrid.AssetPaths;
			int num = 0;
			string[] array = new string[1 + assetPaths.Length];
			array[num] = scenePath;
			num++;
			ReadOnlySpan<string> readOnlySpan = new ReadOnlySpan<string>(assetPaths);
			readOnlySpan.CopyTo(new Span<string>(array).Slice(num, readOnlySpan.Length));
			num += readOnlySpan.Length;
			return array;
		}
	}

	protected override Control InitialFocusedControl => _statsGrid.DefaultFocusedControl;

	public static NStatsScreen? Create()
	{
		if (TestMode.IsOn)
		{
			return null;
		}
		return PreloadManager.Cache.GetScene(_scenePath).Instantiate<NStatsScreen>((GenEditState)0);
	}

	public override void _Ready()
	{
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		ConnectSignals();
		_statsTab = ((Node)this).GetNode<NSettingsTab>(NodePath.op_Implicit("%StatsTab"));
		_statsTab.SetLabel(new LocString("stats_screen", "TAB_STATS.header").GetFormattedText());
		_achievementsTab = ((Node)this).GetNode<NSettingsTab>(NodePath.op_Implicit("%Achievements"));
		_achievementsTab.SetLabel(new LocString("stats_screen", "TAB_ACHIEVEMENT.header").GetFormattedText());
		((GodotObject)_statsTab).Connect(NClickableControl.SignalName.Released, Callable.From<NClickableControl>((Action<NClickableControl>)delegate
		{
			OpenStatsMenu();
		}), 0u);
		_statsTabManager = ((Node)this).GetNode<NStatsTabManager>(NodePath.op_Implicit("%Tabs"));
		_statsGrid = ((Node)this).GetNode<NGeneralStatsGrid>(NodePath.op_Implicit("%StatsGrid"));
		((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%OverallStatsHeader")).SetTextAutoSize(new LocString("main_menu_ui", "STATISTICS.OVERALL.title").GetFormattedText());
		((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%CharacterStatsHeader")).SetTextAutoSize(new LocString("main_menu_ui", "STATISTICS.title").GetFormattedText());
		_achievementsTab.Disable();
	}

	public override void OnSubmenuOpened()
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		Tween? screenTween = _screenTween;
		if (screenTween != null)
		{
			screenTween.Kill();
		}
		_screenTween = ((Node)this).CreateTween();
		_screenTween.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(1f), 0.4).From(Variant.op_Implicit(0f));
		((CanvasItem)this).Visible = true;
		OpenStatsMenu();
		_statsTabManager.ResetTabs();
	}

	private void OpenStatsMenu()
	{
		((CanvasItem)_statsGrid).Visible = true;
		_statsGrid.LoadStats();
		ActiveScreenContext.Instance.Update();
	}

	private void OpenAchievementsMenu()
	{
		((CanvasItem)_statsGrid).Visible = false;
		ActiveScreenContext.Instance.Update();
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Expected O, but got Unknown
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(5);
		list.Add(new MethodInfo(MethodName.Create, new PropertyInfo((Type)24, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false), (MethodFlags)33, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnSubmenuOpened, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OpenStatsMenu, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OpenAchievementsMenu, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.Create && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			NStatsScreen nStatsScreen = Create();
			ret = VariantUtils.CreateFrom<NStatsScreen>(ref nStatsScreen);
			return true;
		}
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnSubmenuOpened && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnSubmenuOpened();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OpenStatsMenu && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OpenStatsMenu();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OpenAchievementsMenu && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OpenAchievementsMenu();
			ret = default(godot_variant);
			return true;
		}
		return base.InvokeGodotClassMethod(in method, args, out ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static bool InvokeGodotClassStaticMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.Create && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			NStatsScreen nStatsScreen = Create();
			ret = VariantUtils.CreateFrom<NStatsScreen>(ref nStatsScreen);
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
		if ((ref method) == MethodName.OnSubmenuOpened)
		{
			return true;
		}
		if ((ref method) == MethodName.OpenStatsMenu)
		{
			return true;
		}
		if ((ref method) == MethodName.OpenAchievementsMenu)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._statsTabManager)
		{
			_statsTabManager = VariantUtils.ConvertTo<NStatsTabManager>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._statsTab)
		{
			_statsTab = VariantUtils.ConvertTo<NSettingsTab>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._achievementsTab)
		{
			_achievementsTab = VariantUtils.ConvertTo<NSettingsTab>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._statsGrid)
		{
			_statsGrid = VariantUtils.ConvertTo<NGeneralStatsGrid>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._screenTween)
		{
			_screenTween = VariantUtils.ConvertTo<Tween>(ref value);
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
		if ((ref name) == PropertyName.InitialFocusedControl)
		{
			Control initialFocusedControl = InitialFocusedControl;
			value = VariantUtils.CreateFrom<Control>(ref initialFocusedControl);
			return true;
		}
		if ((ref name) == PropertyName._statsTabManager)
		{
			value = VariantUtils.CreateFrom<NStatsTabManager>(ref _statsTabManager);
			return true;
		}
		if ((ref name) == PropertyName._statsTab)
		{
			value = VariantUtils.CreateFrom<NSettingsTab>(ref _statsTab);
			return true;
		}
		if ((ref name) == PropertyName._achievementsTab)
		{
			value = VariantUtils.CreateFrom<NSettingsTab>(ref _achievementsTab);
			return true;
		}
		if ((ref name) == PropertyName._statsGrid)
		{
			value = VariantUtils.CreateFrom<NGeneralStatsGrid>(ref _statsGrid);
			return true;
		}
		if ((ref name) == PropertyName._screenTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _screenTween);
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
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._statsTabManager, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._statsTab, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._achievementsTab, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._statsGrid, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._screenTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.InitialFocusedControl, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName._statsTabManager, Variant.From<NStatsTabManager>(ref _statsTabManager));
		info.AddProperty(PropertyName._statsTab, Variant.From<NSettingsTab>(ref _statsTab));
		info.AddProperty(PropertyName._achievementsTab, Variant.From<NSettingsTab>(ref _achievementsTab));
		info.AddProperty(PropertyName._statsGrid, Variant.From<NGeneralStatsGrid>(ref _statsGrid));
		info.AddProperty(PropertyName._screenTween, Variant.From<Tween>(ref _screenTween));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._statsTabManager, ref val))
		{
			_statsTabManager = ((Variant)(ref val)).As<NStatsTabManager>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._statsTab, ref val2))
		{
			_statsTab = ((Variant)(ref val2)).As<NSettingsTab>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._achievementsTab, ref val3))
		{
			_achievementsTab = ((Variant)(ref val3)).As<NSettingsTab>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._statsGrid, ref val4))
		{
			_statsGrid = ((Variant)(ref val4)).As<NGeneralStatsGrid>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._screenTween, ref val5))
		{
			_screenTween = ((Variant)(ref val5)).As<Tween>();
		}
	}
}
