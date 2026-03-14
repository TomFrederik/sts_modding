using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Screens.MainMenu;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.TestSupport;
using MegaCrit.Sts2.Core.Unlocks;

namespace MegaCrit.Sts2.Core.Nodes.Screens.RelicCollection;

[ScriptPath("res://src/Core/Nodes/Screens/RelicCollection/NRelicCollection.cs")]
public class NRelicCollection : NSubmenu
{
	public new class MethodName : NSubmenu.MethodName
	{
		public static readonly StringName Create = StringName.op_Implicit("Create");

		public new static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public new static readonly StringName OnSubmenuOpened = StringName.op_Implicit("OnSubmenuOpened");

		public new static readonly StringName OnSubmenuClosed = StringName.op_Implicit("OnSubmenuClosed");

		public new static readonly StringName OnSubmenuShown = StringName.op_Implicit("OnSubmenuShown");

		public static readonly StringName ClearRelics = StringName.op_Implicit("ClearRelics");

		public static readonly StringName SetLastFocusedRelic = StringName.op_Implicit("SetLastFocusedRelic");
	}

	public new class PropertyName : NSubmenu.PropertyName
	{
		public new static readonly StringName InitialFocusedControl = StringName.op_Implicit("InitialFocusedControl");

		public static readonly StringName _screenContents = StringName.op_Implicit("_screenContents");

		public static readonly StringName _starter = StringName.op_Implicit("_starter");

		public static readonly StringName _common = StringName.op_Implicit("_common");

		public static readonly StringName _uncommon = StringName.op_Implicit("_uncommon");

		public static readonly StringName _rare = StringName.op_Implicit("_rare");

		public static readonly StringName _shop = StringName.op_Implicit("_shop");

		public static readonly StringName _ancient = StringName.op_Implicit("_ancient");

		public static readonly StringName _event = StringName.op_Implicit("_event");

		public static readonly StringName _screenTween = StringName.op_Implicit("_screenTween");
	}

	public new class SignalName : NSubmenu.SignalName
	{
	}

	private static readonly string _scenePath = SceneHelper.GetScenePath("screens/relic_collection/relic_collection");

	private NScrollableContainer _screenContents;

	private NRelicCollectionCategory _starter;

	private NRelicCollectionCategory _common;

	private NRelicCollectionCategory _uncommon;

	private NRelicCollectionCategory _rare;

	private NRelicCollectionCategory _shop;

	private NRelicCollectionCategory _ancient;

	private NRelicCollectionCategory _event;

	private Tween? _screenTween;

	private Task? _loadTask;

	private readonly List<RelicModel> _relics = new List<RelicModel>();

	public IReadOnlyList<RelicModel> Relics => _relics;

	public static string[] AssetPaths => new string[4]
	{
		_scenePath,
		NRelicCollectionEntry.scenePath,
		NRelicCollectionCategory.scenePath,
		NRelicCollectionEntry.lockedIconPath
	};

	protected override Control? InitialFocusedControl => _starter.DefaultFocusedControl;

	public static NRelicCollection? Create()
	{
		if (TestMode.IsOn)
		{
			return null;
		}
		return PreloadManager.Cache.GetScene(_scenePath).Instantiate<NRelicCollection>((GenEditState)0);
	}

	public override void _Ready()
	{
		ConnectSignals();
		_screenContents = ((Node)this).GetNode<NScrollableContainer>(NodePath.op_Implicit("%ScreenContents"));
		_starter = ((Node)this).GetNode<NRelicCollectionCategory>(NodePath.op_Implicit("%Starter"));
		_common = ((Node)this).GetNode<NRelicCollectionCategory>(NodePath.op_Implicit("%Common"));
		_uncommon = ((Node)this).GetNode<NRelicCollectionCategory>(NodePath.op_Implicit("%Uncommon"));
		_rare = ((Node)this).GetNode<NRelicCollectionCategory>(NodePath.op_Implicit("%Rare"));
		_shop = ((Node)this).GetNode<NRelicCollectionCategory>(NodePath.op_Implicit("%Shop"));
		_ancient = ((Node)this).GetNode<NRelicCollectionCategory>(NodePath.op_Implicit("%Ancient"));
		_event = ((Node)this).GetNode<NRelicCollectionCategory>(NodePath.op_Implicit("%Event"));
	}

	public override void OnSubmenuOpened()
	{
		base.OnSubmenuOpened();
		_relics.Clear();
		_loadTask = TaskHelper.RunSafely(LoadRelics());
	}

	public override void OnSubmenuClosed()
	{
		base.OnSubmenuClosed();
		Tween? screenTween = _screenTween;
		if (screenTween != null)
		{
			screenTween.Kill();
		}
		ClearRelics();
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
		_screenTween.TweenProperty((GodotObject)(object)_screenContents, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(1f), 0.4).From(Variant.op_Implicit(0f));
	}

	private async Task LoadRelics()
	{
		((CanvasItem)_starter).Modulate = Colors.Transparent;
		((CanvasItem)_common).Modulate = Colors.Transparent;
		((CanvasItem)_uncommon).Modulate = Colors.Transparent;
		((CanvasItem)_rare).Modulate = Colors.Transparent;
		((CanvasItem)_shop).Modulate = Colors.Transparent;
		((CanvasItem)_ancient).Modulate = Colors.Transparent;
		((CanvasItem)_event).Modulate = Colors.Transparent;
		HashSet<RelicModel> seenRelics = SaveManager.Instance.Progress.DiscoveredRelics.Select(ModelDb.GetByIdOrNull<RelicModel>).OfType<RelicModel>().ToHashSet();
		UnlockState unlockState = SaveManager.Instance.GenerateUnlockStateFromProgress();
		HashSet<RelicModel> allUnlockedRelics = unlockState.Relics.ToHashSet();
		_starter.LoadRelics(RelicRarity.Starter, this, new LocString("relic_collection", "STARTER"), seenRelics, unlockState, allUnlockedRelics);
		_common.LoadRelics(RelicRarity.Common, this, new LocString("relic_collection", "COMMON"), seenRelics, unlockState, allUnlockedRelics);
		_uncommon.LoadRelics(RelicRarity.Uncommon, this, new LocString("relic_collection", "UNCOMMON"), seenRelics, unlockState, allUnlockedRelics);
		_rare.LoadRelics(RelicRarity.Rare, this, new LocString("relic_collection", "RARE"), seenRelics, unlockState, allUnlockedRelics);
		_shop.LoadRelics(RelicRarity.Shop, this, new LocString("relic_collection", "SHOP"), seenRelics, unlockState, allUnlockedRelics);
		_ancient.LoadRelics(RelicRarity.Ancient, this, new LocString("relic_collection", "ANCIENT"), seenRelics, unlockState, allUnlockedRelics);
		_event.LoadRelics(RelicRarity.Event, this, new LocString("relic_collection", "EVENT"), seenRelics, unlockState, allUnlockedRelics);
		List<IReadOnlyList<Control>> list = new List<IReadOnlyList<Control>>();
		list.AddRange(_starter.GetGridItems());
		list.AddRange(_common.GetGridItems());
		list.AddRange(_uncommon.GetGridItems());
		list.AddRange(_rare.GetGridItems());
		list.AddRange(_shop.GetGridItems());
		list.AddRange(_ancient.GetGridItems());
		list.AddRange(_event.GetGridItems());
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
		((CanvasItem)_starter).Modulate = Colors.White;
		((CanvasItem)_common).Modulate = Colors.White;
		((CanvasItem)_uncommon).Modulate = Colors.White;
		((CanvasItem)_rare).Modulate = Colors.White;
		((CanvasItem)_shop).Modulate = Colors.White;
		((CanvasItem)_ancient).Modulate = Colors.White;
		((CanvasItem)_event).Modulate = Colors.White;
		_screenContents.InstantlyScrollToTop();
		InitialFocusedControl?.TryGrabFocus();
	}

	public void AddRelics(IEnumerable<RelicModel> relics)
	{
		_relics.AddRange(relics);
	}

	private void ClearRelics()
	{
		_starter.ClearRelics();
		_common.ClearRelics();
		_uncommon.ClearRelics();
		_rare.ClearRelics();
		_shop.ClearRelics();
		_ancient.ClearRelics();
		_event.ClearRelics();
		_relics.Clear();
	}

	public void SetLastFocusedRelic(NRelicCollectionEntry relic)
	{
		_lastFocusedControl = (Control?)(object)relic;
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
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Expected O, but got Unknown
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(7);
		list.Add(new MethodInfo(MethodName.Create, new PropertyInfo((Type)24, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false), (MethodFlags)33, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnSubmenuOpened, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnSubmenuClosed, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnSubmenuShown, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ClearRelics, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetLastFocusedRelic, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("relic"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
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
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.Create && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			NRelicCollection nRelicCollection = Create();
			ret = VariantUtils.CreateFrom<NRelicCollection>(ref nRelicCollection);
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
		if ((ref method) == MethodName.ClearRelics && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			ClearRelics();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetLastFocusedRelic && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			SetLastFocusedRelic(VariantUtils.ConvertTo<NRelicCollectionEntry>(ref ((NativeVariantPtrArgs)(ref args))[0]));
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
			NRelicCollection nRelicCollection = Create();
			ret = VariantUtils.CreateFrom<NRelicCollection>(ref nRelicCollection);
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
		if ((ref method) == MethodName.ClearRelics)
		{
			return true;
		}
		if ((ref method) == MethodName.SetLastFocusedRelic)
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
		if ((ref name) == PropertyName._starter)
		{
			_starter = VariantUtils.ConvertTo<NRelicCollectionCategory>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._common)
		{
			_common = VariantUtils.ConvertTo<NRelicCollectionCategory>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._uncommon)
		{
			_uncommon = VariantUtils.ConvertTo<NRelicCollectionCategory>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._rare)
		{
			_rare = VariantUtils.ConvertTo<NRelicCollectionCategory>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._shop)
		{
			_shop = VariantUtils.ConvertTo<NRelicCollectionCategory>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._ancient)
		{
			_ancient = VariantUtils.ConvertTo<NRelicCollectionCategory>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._event)
		{
			_event = VariantUtils.ConvertTo<NRelicCollectionCategory>(ref value);
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
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
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
		if ((ref name) == PropertyName._starter)
		{
			value = VariantUtils.CreateFrom<NRelicCollectionCategory>(ref _starter);
			return true;
		}
		if ((ref name) == PropertyName._common)
		{
			value = VariantUtils.CreateFrom<NRelicCollectionCategory>(ref _common);
			return true;
		}
		if ((ref name) == PropertyName._uncommon)
		{
			value = VariantUtils.CreateFrom<NRelicCollectionCategory>(ref _uncommon);
			return true;
		}
		if ((ref name) == PropertyName._rare)
		{
			value = VariantUtils.CreateFrom<NRelicCollectionCategory>(ref _rare);
			return true;
		}
		if ((ref name) == PropertyName._shop)
		{
			value = VariantUtils.CreateFrom<NRelicCollectionCategory>(ref _shop);
			return true;
		}
		if ((ref name) == PropertyName._ancient)
		{
			value = VariantUtils.CreateFrom<NRelicCollectionCategory>(ref _ancient);
			return true;
		}
		if ((ref name) == PropertyName._event)
		{
			value = VariantUtils.CreateFrom<NRelicCollectionCategory>(ref _event);
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
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._screenContents, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._starter, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._common, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._uncommon, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._rare, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._shop, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._ancient, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._event, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName._screenContents, Variant.From<NScrollableContainer>(ref _screenContents));
		info.AddProperty(PropertyName._starter, Variant.From<NRelicCollectionCategory>(ref _starter));
		info.AddProperty(PropertyName._common, Variant.From<NRelicCollectionCategory>(ref _common));
		info.AddProperty(PropertyName._uncommon, Variant.From<NRelicCollectionCategory>(ref _uncommon));
		info.AddProperty(PropertyName._rare, Variant.From<NRelicCollectionCategory>(ref _rare));
		info.AddProperty(PropertyName._shop, Variant.From<NRelicCollectionCategory>(ref _shop));
		info.AddProperty(PropertyName._ancient, Variant.From<NRelicCollectionCategory>(ref _ancient));
		info.AddProperty(PropertyName._event, Variant.From<NRelicCollectionCategory>(ref _event));
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
		if (info.TryGetProperty(PropertyName._starter, ref val2))
		{
			_starter = ((Variant)(ref val2)).As<NRelicCollectionCategory>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._common, ref val3))
		{
			_common = ((Variant)(ref val3)).As<NRelicCollectionCategory>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._uncommon, ref val4))
		{
			_uncommon = ((Variant)(ref val4)).As<NRelicCollectionCategory>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._rare, ref val5))
		{
			_rare = ((Variant)(ref val5)).As<NRelicCollectionCategory>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._shop, ref val6))
		{
			_shop = ((Variant)(ref val6)).As<NRelicCollectionCategory>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName._ancient, ref val7))
		{
			_ancient = ((Variant)(ref val7)).As<NRelicCollectionCategory>();
		}
		Variant val8 = default(Variant);
		if (info.TryGetProperty(PropertyName._event, ref val8))
		{
			_event = ((Variant)(ref val8)).As<NRelicCollectionCategory>();
		}
		Variant val9 = default(Variant);
		if (info.TryGetProperty(PropertyName._screenTween, ref val9))
		{
			_screenTween = ((Variant)(ref val9)).As<Tween>();
		}
	}
}
