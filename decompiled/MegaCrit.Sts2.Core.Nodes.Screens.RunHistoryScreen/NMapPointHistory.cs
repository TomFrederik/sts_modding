using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Relics;
using MegaCrit.Sts2.Core.Runs;

namespace MegaCrit.Sts2.Core.Nodes.Screens.RunHistoryScreen;

[ScriptPath("res://src/Core/Nodes/Screens/RunHistoryScreen/NMapPointHistory.cs")]
public class NMapPointHistory : Control
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName SetDeckHistory = StringName.op_Implicit("SetDeckHistory");

		public static readonly StringName SetRelicHistory = StringName.op_Implicit("SetRelicHistory");

		public static readonly StringName HighlightRelevantEntries = StringName.op_Implicit("HighlightRelevantEntries");

		public static readonly StringName UnHighlightEntries = StringName.op_Implicit("UnHighlightEntries");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName DefaultFocusedControl = StringName.op_Implicit("DefaultFocusedControl");

		public static readonly StringName _actContainer = StringName.op_Implicit("_actContainer");
	}

	public class SignalName : SignalName
	{
	}

	private Control _actContainer;

	private readonly List<NActHistoryEntry> _actHistories = new List<NActHistoryEntry>();

	private List<NMapPointHistoryEntry> MapHistories => _actHistories.SelectMany((NActHistoryEntry a) => a.Entries).ToList();

	public Control? DefaultFocusedControl => (Control?)(object)MapHistories.FirstOrDefault();

	public override void _Ready()
	{
		_actContainer = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%Acts"));
	}

	public void LoadHistory(RunHistory history)
	{
		foreach (Node child in ((Node)_actContainer).GetChildren(false))
		{
			child.QueueFreeSafely();
		}
		_actHistories.Clear();
		int num = 1;
		for (int i = 0; i < history.MapPointHistory.Count; i++)
		{
			LocString title = ModelDb.GetById<ActModel>(history.Acts[i]).Title;
			NActHistoryEntry nActHistoryEntry = NActHistoryEntry.Create(title, history, history.MapPointHistory[i], num);
			((Node)(object)_actContainer).AddChildSafely((Node?)(object)nActHistoryEntry);
			_actHistories.Add(nActHistoryEntry);
			num += history.MapPointHistory[i].Count;
		}
	}

	public void SetPlayer(RunHistoryPlayer player)
	{
		foreach (NActHistoryEntry actHistory in _actHistories)
		{
			actHistory.SetPlayer(player);
		}
	}

	public void SetDeckHistory(NDeckHistory deckHistory)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)deckHistory).Connect(NDeckHistory.SignalName.Hovered, Callable.From<NDeckHistoryEntry>((Action<NDeckHistoryEntry>)HighlightRelevantEntries), 0u);
		((GodotObject)deckHistory).Connect(NDeckHistory.SignalName.Unhovered, Callable.From<NDeckHistoryEntry>((Action<NDeckHistoryEntry>)UnHighlightEntries), 0u);
	}

	public void SetRelicHistory(NRelicHistory relicHistory)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)relicHistory).Connect(NRelicHistory.SignalName.Hovered, Callable.From<NRelicBasicHolder>((Action<NRelicBasicHolder>)HighlightRelevantEntries), 0u);
		((GodotObject)relicHistory).Connect(NRelicHistory.SignalName.Unhovered, Callable.From<NRelicBasicHolder>((Action<NRelicBasicHolder>)UnHighlightEntries), 0u);
	}

	private void HighlightRelevantEntries(NDeckHistoryEntry historyEntry)
	{
		foreach (int floorNumber in historyEntry.FloorsAddedToDeck)
		{
			MapHistories.FirstOrDefault((NMapPointHistoryEntry e) => e.FloorNum == floorNumber)?.Highlight();
		}
	}

	private void HighlightRelevantEntries(NRelicBasicHolder holder)
	{
		if (holder.Relic.Model.FloorAddedToDeck > 0)
		{
			MapHistories.FirstOrDefault((NMapPointHistoryEntry e) => e.FloorNum == holder.Relic.Model.FloorAddedToDeck)?.Highlight();
		}
	}

	private void UnHighlightEntries(NRelicBasicHolder _)
	{
		UnHighlightEntries();
	}

	private void UnHighlightEntries(NDeckHistoryEntry _)
	{
		UnHighlightEntries();
	}

	private void UnHighlightEntries()
	{
		foreach (NMapPointHistoryEntry mapHistory in MapHistories)
		{
			mapHistory.Unhighlight();
		}
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
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Expected O, but got Unknown
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Expected O, but got Unknown
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Expected O, but got Unknown
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(6);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetDeckHistory, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("deckHistory"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("VBoxContainer"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetRelicHistory, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("relicHistory"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("VBoxContainer"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.HighlightRelevantEntries, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("historyEntry"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UnHighlightEntries, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UnHighlightEntries, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetDeckHistory && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			SetDeckHistory(VariantUtils.ConvertTo<NDeckHistory>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetRelicHistory && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			SetRelicHistory(VariantUtils.ConvertTo<NRelicHistory>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.HighlightRelevantEntries && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			HighlightRelevantEntries(VariantUtils.ConvertTo<NDeckHistoryEntry>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.UnHighlightEntries && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			UnHighlightEntries(VariantUtils.ConvertTo<NRelicBasicHolder>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.UnHighlightEntries && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			UnHighlightEntries();
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
		if ((ref method) == MethodName.SetDeckHistory)
		{
			return true;
		}
		if ((ref method) == MethodName.SetRelicHistory)
		{
			return true;
		}
		if ((ref method) == MethodName.HighlightRelevantEntries)
		{
			return true;
		}
		if ((ref method) == MethodName.UnHighlightEntries)
		{
			return true;
		}
		return ((Control)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._actContainer)
		{
			_actContainer = VariantUtils.ConvertTo<Control>(ref value);
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
		if ((ref name) == PropertyName.DefaultFocusedControl)
		{
			Control defaultFocusedControl = DefaultFocusedControl;
			value = VariantUtils.CreateFrom<Control>(ref defaultFocusedControl);
			return true;
		}
		if ((ref name) == PropertyName._actContainer)
		{
			value = VariantUtils.CreateFrom<Control>(ref _actContainer);
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
		list.Add(new PropertyInfo((Type)24, PropertyName._actContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.DefaultFocusedControl, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		info.AddProperty(PropertyName._actContainer, Variant.From<Control>(ref _actContainer));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._actContainer, ref val))
		{
			_actContainer = ((Variant)(ref val)).As<Control>();
		}
	}
}
