using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Leaderboard;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Platform;
using MegaCrit.Sts2.Core.TestSupport;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.Nodes.Screens.DailyRun;

[ScriptPath("res://src/Core/Nodes/Screens/DailyRun/NDailyRunLeaderboardRow.cs")]
public class NDailyRunLeaderboardRow : Control
{
	public class MethodName : MethodName
	{
		public static readonly StringName CreateHeader = StringName.op_Implicit("CreateHeader");

		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName _rank = StringName.op_Implicit("_rank");

		public static readonly StringName _name = StringName.op_Implicit("_name");

		public static readonly StringName _score = StringName.op_Implicit("_score");

		public static readonly StringName _isHeader = StringName.op_Implicit("_isHeader");
	}

	public class SignalName : SignalName
	{
	}

	private static readonly string _scenePath = SceneHelper.GetScenePath("screens/daily_run/daily_run_leaderboard_row");

	private MegaLabel _rank;

	private MegaRichTextLabel _name;

	private MegaLabel _score;

	private LeaderboardEntry? _entry;

	private bool _isHeader;

	public static NDailyRunLeaderboardRow? Create(LeaderboardEntry entry)
	{
		if (TestMode.IsOn)
		{
			return null;
		}
		NDailyRunLeaderboardRow nDailyRunLeaderboardRow = PreloadManager.Cache.GetScene(_scenePath).Instantiate<NDailyRunLeaderboardRow>((GenEditState)0);
		nDailyRunLeaderboardRow._entry = entry;
		return nDailyRunLeaderboardRow;
	}

	public static NDailyRunLeaderboardRow? CreateHeader()
	{
		if (TestMode.IsOn)
		{
			return null;
		}
		NDailyRunLeaderboardRow nDailyRunLeaderboardRow = PreloadManager.Cache.GetScene(_scenePath).Instantiate<NDailyRunLeaderboardRow>((GenEditState)0);
		nDailyRunLeaderboardRow._isHeader = true;
		return nDailyRunLeaderboardRow;
	}

	public override void _Ready()
	{
		_rank = ((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("Rank"));
		_name = ((Node)this).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("Name"));
		_score = ((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("Score"));
		if (_isHeader)
		{
			_rank.SetTextAutoSize(" " + new LocString("main_menu_ui", "LEADERBOARDS.rankHeader").GetRawText());
			_name.SetTextAutoSize(new LocString("main_menu_ui", "LEADERBOARDS.nameHeader").GetRawText());
			_score.SetTextAutoSize(new LocString("main_menu_ui", "LEADERBOARDS.scoreHeader").GetRawText() + " ");
		}
		else if (_entry != null)
		{
			IEnumerable<string> values = _entry.userIds.Select((ulong id) => PlatformUtil.GetPlayerName(LeaderboardManager.CurrentPlatform, id));
			_rank.SetTextAutoSize($" {_entry.rank + 1}");
			_name.SetTextAutoSize(string.Join(",", values));
			_score.SetTextAutoSize($"{_entry.score} ");
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Expected O, but got Unknown
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(2);
		list.Add(new MethodInfo(MethodName.CreateHeader, new PropertyInfo((Type)24, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false), (MethodFlags)33, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.CreateHeader && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			NDailyRunLeaderboardRow nDailyRunLeaderboardRow = CreateHeader();
			ret = VariantUtils.CreateFrom<NDailyRunLeaderboardRow>(ref nDailyRunLeaderboardRow);
			return true;
		}
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		return ((Control)this).InvokeGodotClassMethod(ref method, args, ref ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static bool InvokeGodotClassStaticMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.CreateHeader && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			NDailyRunLeaderboardRow nDailyRunLeaderboardRow = CreateHeader();
			ret = VariantUtils.CreateFrom<NDailyRunLeaderboardRow>(ref nDailyRunLeaderboardRow);
			return true;
		}
		ret = default(godot_variant);
		return false;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if ((ref method) == MethodName.CreateHeader)
		{
			return true;
		}
		if ((ref method) == MethodName._Ready)
		{
			return true;
		}
		return ((Control)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._rank)
		{
			_rank = VariantUtils.ConvertTo<MegaLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._name)
		{
			_name = VariantUtils.ConvertTo<MegaRichTextLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._score)
		{
			_score = VariantUtils.ConvertTo<MegaLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._isHeader)
		{
			_isHeader = VariantUtils.ConvertTo<bool>(ref value);
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
		if ((ref name) == PropertyName._rank)
		{
			value = VariantUtils.CreateFrom<MegaLabel>(ref _rank);
			return true;
		}
		if ((ref name) == PropertyName._name)
		{
			value = VariantUtils.CreateFrom<MegaRichTextLabel>(ref _name);
			return true;
		}
		if ((ref name) == PropertyName._score)
		{
			value = VariantUtils.CreateFrom<MegaLabel>(ref _score);
			return true;
		}
		if ((ref name) == PropertyName._isHeader)
		{
			value = VariantUtils.CreateFrom<bool>(ref _isHeader);
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
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._rank, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._name, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._score, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._isHeader, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		info.AddProperty(PropertyName._rank, Variant.From<MegaLabel>(ref _rank));
		info.AddProperty(PropertyName._name, Variant.From<MegaRichTextLabel>(ref _name));
		info.AddProperty(PropertyName._score, Variant.From<MegaLabel>(ref _score));
		info.AddProperty(PropertyName._isHeader, Variant.From<bool>(ref _isHeader));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._rank, ref val))
		{
			_rank = ((Variant)(ref val)).As<MegaLabel>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._name, ref val2))
		{
			_name = ((Variant)(ref val2)).As<MegaRichTextLabel>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._score, ref val3))
		{
			_score = ((Variant)(ref val3)).As<MegaLabel>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._isHeader, ref val4))
		{
			_isHeader = ((Variant)(ref val4)).As<bool>();
		}
	}
}
