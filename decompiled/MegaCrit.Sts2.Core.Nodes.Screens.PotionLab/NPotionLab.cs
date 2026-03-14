using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Screens.MainMenu;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.TestSupport;
using MegaCrit.Sts2.Core.Unlocks;

namespace MegaCrit.Sts2.Core.Nodes.Screens.PotionLab;

[ScriptPath("res://src/Core/Nodes/Screens/PotionLab/NPotionLab.cs")]
public class NPotionLab : NSubmenu
{
	public new class MethodName : NSubmenu.MethodName
	{
		public static readonly StringName Create = StringName.op_Implicit("Create");

		public new static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public new static readonly StringName OnSubmenuOpened = StringName.op_Implicit("OnSubmenuOpened");

		public new static readonly StringName OnSubmenuClosed = StringName.op_Implicit("OnSubmenuClosed");

		public new static readonly StringName OnSubmenuShown = StringName.op_Implicit("OnSubmenuShown");

		public static readonly StringName ClearPotions = StringName.op_Implicit("ClearPotions");
	}

	public new class PropertyName : NSubmenu.PropertyName
	{
		public new static readonly StringName InitialFocusedControl = StringName.op_Implicit("InitialFocusedControl");

		public static readonly StringName _screenContents = StringName.op_Implicit("_screenContents");

		public static readonly StringName _common = StringName.op_Implicit("_common");

		public static readonly StringName _uncommon = StringName.op_Implicit("_uncommon");

		public static readonly StringName _rare = StringName.op_Implicit("_rare");

		public static readonly StringName _special = StringName.op_Implicit("_special");

		public static readonly StringName _screenTween = StringName.op_Implicit("_screenTween");
	}

	public new class SignalName : NSubmenu.SignalName
	{
	}

	private static readonly string _scenePath = SceneHelper.GetScenePath("screens/potion_lab/potion_lab");

	private NScrollableContainer _screenContents;

	private NPotionLabCategory _common;

	private NPotionLabCategory _uncommon;

	private NPotionLabCategory _rare;

	private NPotionLabCategory _special;

	private Tween? _screenTween;

	private Task? _loadTask;

	public static string[] AssetPaths => new string[3]
	{
		_scenePath,
		NLabPotionHolder.scenePath,
		NLabPotionHolder.lockedIconPath
	};

	protected override Control? InitialFocusedControl => _common.DefaultFocusedControl;

	public static NPotionLab? Create()
	{
		if (TestMode.IsOn)
		{
			return null;
		}
		return PreloadManager.Cache.GetScene(_scenePath).Instantiate<NPotionLab>((GenEditState)0);
	}

	public override void _Ready()
	{
		ConnectSignals();
		_screenContents = ((Node)this).GetNode<NScrollableContainer>(NodePath.op_Implicit("%ScreenContents"));
		_common = ((Node)this).GetNode<NPotionLabCategory>(NodePath.op_Implicit("%Common"));
		_uncommon = ((Node)this).GetNode<NPotionLabCategory>(NodePath.op_Implicit("%Uncommon"));
		_rare = ((Node)this).GetNode<NPotionLabCategory>(NodePath.op_Implicit("%Rare"));
		_special = ((Node)this).GetNode<NPotionLabCategory>(NodePath.op_Implicit("%Special"));
	}

	public override void OnSubmenuOpened()
	{
		base.OnSubmenuOpened();
		_loadTask = TaskHelper.RunSafely(LoadPotions());
	}

	public override void OnSubmenuClosed()
	{
		base.OnSubmenuClosed();
		Tween? screenTween = _screenTween;
		if (screenTween != null)
		{
			screenTween.Kill();
		}
		ClearPotions();
	}

	protected override void OnSubmenuShown()
	{
		base.OnSubmenuShown();
		TaskHelper.RunSafely(TweenAfterLoading());
	}

	private async Task TweenAfterLoading()
	{
		((CanvasItem)_screenContents).Modulate = new Color(1f, 1f, 1f, 0f);
		if (_loadTask != null)
		{
			await _loadTask;
		}
		Tween? screenTween = _screenTween;
		if (screenTween != null)
		{
			screenTween.Kill();
		}
		_screenTween = ((Node)this).CreateTween();
		_screenTween.TweenProperty((GodotObject)(object)_screenContents, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(1f), 0.25).From(Variant.op_Implicit(0f));
	}

	private async Task LoadPotions()
	{
		((CanvasItem)_common).Modulate = Colors.Transparent;
		((CanvasItem)_uncommon).Modulate = Colors.Transparent;
		((CanvasItem)_rare).Modulate = Colors.Transparent;
		((CanvasItem)_special).Modulate = Colors.Transparent;
		UnlockState unlockState = SaveManager.Instance.GenerateUnlockStateFromProgress();
		HashSet<PotionModel> allUnlockedPotions = unlockState.Potions.ToHashSet();
		HashSet<PotionModel> seenPotions = SaveManager.Instance.Progress.DiscoveredPotions.Select(ModelDb.GetByIdOrNull<PotionModel>).OfType<PotionModel>().ToHashSet();
		_common.LoadPotions(PotionRarity.Common, new LocString("potion_lab", "COMMON"), seenPotions, unlockState, allUnlockedPotions);
		_uncommon.LoadPotions(PotionRarity.Uncommon, new LocString("potion_lab", "UNCOMMON"), seenPotions, unlockState, allUnlockedPotions);
		_rare.LoadPotions(PotionRarity.Rare, new LocString("potion_lab", "RARE"), seenPotions, unlockState, allUnlockedPotions);
		_special.LoadPotions(PotionRarity.Event, new LocString("potion_lab", "SPECIAL"), seenPotions, unlockState, allUnlockedPotions, PotionRarity.Token);
		List<IReadOnlyList<Control>> list = new List<IReadOnlyList<Control>>();
		list.AddRange(_common.GetGridItems());
		list.AddRange(_uncommon.GetGridItems());
		list.AddRange(_rare.GetGridItems());
		list.AddRange(_special.GetGridItems());
		for (int i = 0; i < list.Count; i++)
		{
			for (int j = 0; j < list[i].Count; j++)
			{
				Control val = list[i][j];
				NodePath path;
				if (j <= 0)
				{
					IReadOnlyList<Control> readOnlyList = list[i];
					path = ((Node)readOnlyList[readOnlyList.Count - 1]).GetPath();
				}
				else
				{
					path = ((Node)list[i][j - 1]).GetPath();
				}
				val.FocusNeighborLeft = path;
				val.FocusNeighborRight = ((j < list[i].Count - 1) ? ((Node)list[i][j + 1]).GetPath() : ((Node)list[i][0]).GetPath());
				if (i > 0)
				{
					val.FocusNeighborTop = ((j < list[i - 1].Count) ? ((Node)list[i - 1][j]).GetPath() : ((Node)list[i - 1][list[i - 1].Count - 1]).GetPath());
				}
				else
				{
					val.FocusNeighborTop = ((Node)list[i][j]).GetPath();
				}
				if (i < list.Count - 1)
				{
					val.FocusNeighborBottom = ((j < list[i + 1].Count) ? ((Node)list[i + 1][j]).GetPath() : ((Node)list[i + 1][list[i + 1].Count - 1]).GetPath());
				}
				else
				{
					val.FocusNeighborBottom = ((Node)list[i][j]).GetPath();
				}
			}
		}
		await ((GodotObject)this).ToSignal((GodotObject)(object)((Node)this).GetTree(), SignalName.ProcessFrame);
		((CanvasItem)_common).Modulate = Colors.White;
		((CanvasItem)_uncommon).Modulate = Colors.White;
		((CanvasItem)_rare).Modulate = Colors.White;
		((CanvasItem)_special).Modulate = Colors.White;
		_screenContents.InstantlyScrollToTop();
		InitialFocusedControl?.TryGrabFocus();
	}

	private void ClearPotions()
	{
		_common.ClearPotions();
		_uncommon.ClearPotions();
		_rare.ClearPotions();
		_special.ClearPotions();
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
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(6);
		list.Add(new MethodInfo(MethodName.Create, new PropertyInfo((Type)24, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false), (MethodFlags)33, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnSubmenuOpened, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnSubmenuClosed, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnSubmenuShown, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ClearPotions, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.Create && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			NPotionLab nPotionLab = Create();
			ret = VariantUtils.CreateFrom<NPotionLab>(ref nPotionLab);
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
		if ((ref method) == MethodName.OnSubmenuClosed && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnSubmenuClosed();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnSubmenuShown && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnSubmenuShown();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.ClearPotions && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			ClearPotions();
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
			NPotionLab nPotionLab = Create();
			ret = VariantUtils.CreateFrom<NPotionLab>(ref nPotionLab);
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
		if ((ref method) == MethodName.OnSubmenuClosed)
		{
			return true;
		}
		if ((ref method) == MethodName.OnSubmenuShown)
		{
			return true;
		}
		if ((ref method) == MethodName.ClearPotions)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._screenContents)
		{
			_screenContents = VariantUtils.ConvertTo<NScrollableContainer>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._common)
		{
			_common = VariantUtils.ConvertTo<NPotionLabCategory>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._uncommon)
		{
			_uncommon = VariantUtils.ConvertTo<NPotionLabCategory>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._rare)
		{
			_rare = VariantUtils.ConvertTo<NPotionLabCategory>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._special)
		{
			_special = VariantUtils.ConvertTo<NPotionLabCategory>(ref value);
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
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName.InitialFocusedControl)
		{
			Control initialFocusedControl = InitialFocusedControl;
			value = VariantUtils.CreateFrom<Control>(ref initialFocusedControl);
			return true;
		}
		if ((ref name) == PropertyName._screenContents)
		{
			value = VariantUtils.CreateFrom<NScrollableContainer>(ref _screenContents);
			return true;
		}
		if ((ref name) == PropertyName._common)
		{
			value = VariantUtils.CreateFrom<NPotionLabCategory>(ref _common);
			return true;
		}
		if ((ref name) == PropertyName._uncommon)
		{
			value = VariantUtils.CreateFrom<NPotionLabCategory>(ref _uncommon);
			return true;
		}
		if ((ref name) == PropertyName._rare)
		{
			value = VariantUtils.CreateFrom<NPotionLabCategory>(ref _rare);
			return true;
		}
		if ((ref name) == PropertyName._special)
		{
			value = VariantUtils.CreateFrom<NPotionLabCategory>(ref _special);
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
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._screenContents, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._common, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._uncommon, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._rare, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._special, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName._screenContents, Variant.From<NScrollableContainer>(ref _screenContents));
		info.AddProperty(PropertyName._common, Variant.From<NPotionLabCategory>(ref _common));
		info.AddProperty(PropertyName._uncommon, Variant.From<NPotionLabCategory>(ref _uncommon));
		info.AddProperty(PropertyName._rare, Variant.From<NPotionLabCategory>(ref _rare));
		info.AddProperty(PropertyName._special, Variant.From<NPotionLabCategory>(ref _special));
		info.AddProperty(PropertyName._screenTween, Variant.From<Tween>(ref _screenTween));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._screenContents, ref val))
		{
			_screenContents = ((Variant)(ref val)).As<NScrollableContainer>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._common, ref val2))
		{
			_common = ((Variant)(ref val2)).As<NPotionLabCategory>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._uncommon, ref val3))
		{
			_uncommon = ((Variant)(ref val3)).As<NPotionLabCategory>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._rare, ref val4))
		{
			_rare = ((Variant)(ref val4)).As<NPotionLabCategory>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._special, ref val5))
		{
			_special = ((Variant)(ref val5)).As<NPotionLabCategory>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._screenTween, ref val6))
		{
			_screenTween = ((Variant)(ref val6)).As<Tween>();
		}
	}
}
