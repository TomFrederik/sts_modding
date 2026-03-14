using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Runs.History;
using MegaCrit.Sts2.Core.TestSupport;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.Nodes.Screens.RunHistoryScreen;

[ScriptPath("res://src/Core/Nodes/Screens/RunHistoryScreen/NActHistoryEntry.cs")]
public class NActHistoryEntry : HBoxContainer
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName _actLabel = StringName.op_Implicit("_actLabel");

		public static readonly StringName _baseFloorNum = StringName.op_Implicit("_baseFloorNum");
	}

	public class SignalName : SignalName
	{
	}

	private MegaLabel _actLabel;

	private LocString _actName;

	private RunHistory _runHistory;

	private IReadOnlyList<MapPointHistoryEntry> _entries;

	private int _baseFloorNum;

	private static string ScenePath => SceneHelper.GetScenePath("screens/run_history_screen/act_history_entry");

	public static IEnumerable<string> AssetPaths => new _003C_003Ez__ReadOnlySingleElementList<string>(ScenePath);

	public List<NMapPointHistoryEntry> Entries { get; private set; } = new List<NMapPointHistoryEntry>();

	public override void _Ready()
	{
		_actLabel = ((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%Title"));
		_actLabel.SetTextAutoSize(_actName.GetFormattedText());
		for (int i = 0; i < _entries.Count; i++)
		{
			NMapPointHistoryEntry nMapPointHistoryEntry = NMapPointHistoryEntry.Create(_runHistory, _entries[i], i + _baseFloorNum);
			((Node)(object)this).AddChildSafely((Node?)(object)nMapPointHistoryEntry);
			Entries.Add(nMapPointHistoryEntry);
		}
	}

	public void SetPlayer(RunHistoryPlayer player)
	{
		foreach (NMapPointHistoryEntry entry in Entries)
		{
			entry.SetPlayer(player);
		}
	}

	public static NActHistoryEntry? Create(LocString actName, RunHistory runHistory, IReadOnlyList<MapPointHistoryEntry> logs, int baseFloorNum)
	{
		if (TestMode.IsOn)
		{
			return null;
		}
		NActHistoryEntry nActHistoryEntry = PreloadManager.Cache.GetScene(ScenePath).Instantiate<NActHistoryEntry>((GenEditState)0);
		nActHistoryEntry._actName = actName;
		nActHistoryEntry._runHistory = runHistory;
		nActHistoryEntry._entries = logs;
		nActHistoryEntry._baseFloorNum = baseFloorNum;
		return nActHistoryEntry;
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
		return ((HBoxContainer)this).InvokeGodotClassMethod(ref method, args, ref ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if ((ref method) == MethodName._Ready)
		{
			return true;
		}
		return ((HBoxContainer)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._actLabel)
		{
			_actLabel = VariantUtils.ConvertTo<MegaLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._baseFloorNum)
		{
			_baseFloorNum = VariantUtils.ConvertTo<int>(ref value);
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
		if ((ref name) == PropertyName._actLabel)
		{
			value = VariantUtils.CreateFrom<MegaLabel>(ref _actLabel);
			return true;
		}
		if ((ref name) == PropertyName._baseFloorNum)
		{
			value = VariantUtils.CreateFrom<int>(ref _baseFloorNum);
			return true;
		}
		return ((GodotObject)this).GetGodotClassPropertyValue(ref name, ref value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._actLabel, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)2, PropertyName._baseFloorNum, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		info.AddProperty(PropertyName._actLabel, Variant.From<MegaLabel>(ref _actLabel));
		info.AddProperty(PropertyName._baseFloorNum, Variant.From<int>(ref _baseFloorNum));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._actLabel, ref val))
		{
			_actLabel = ((Variant)(ref val)).As<MegaLabel>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._baseFloorNum, ref val2))
		{
			_baseFloorNum = ((Variant)(ref val2)).As<int>();
		}
	}
}
