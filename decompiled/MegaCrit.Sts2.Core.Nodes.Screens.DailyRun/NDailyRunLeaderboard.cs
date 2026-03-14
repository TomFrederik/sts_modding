using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Daily;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Leaderboard;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.Nodes.Screens.DailyRun;

[ScriptPath("res://src/Core/Nodes/Screens/DailyRun/NDailyRunLeaderboard.cs")]
public class NDailyRunLeaderboard : Control
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName SetLocalizedText = StringName.op_Implicit("SetLocalizedText");

		public static readonly StringName Cleanup = StringName.op_Implicit("Cleanup");

		public static readonly StringName ChangePage = StringName.op_Implicit("ChangePage");

		public static readonly StringName SetPage = StringName.op_Implicit("SetPage");

		public static readonly StringName ClearEntries = StringName.op_Implicit("ClearEntries");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName _titleLabel = StringName.op_Implicit("_titleLabel");

		public static readonly StringName _paginator = StringName.op_Implicit("_paginator");

		public static readonly StringName _scoreContainer = StringName.op_Implicit("_scoreContainer");

		public static readonly StringName _leftArrow = StringName.op_Implicit("_leftArrow");

		public static readonly StringName _rightArrow = StringName.op_Implicit("_rightArrow");

		public static readonly StringName _loadingIndicator = StringName.op_Implicit("_loadingIndicator");

		public static readonly StringName _noScoresIndicator = StringName.op_Implicit("_noScoresIndicator");

		public static readonly StringName _noFriendsIndicator = StringName.op_Implicit("_noFriendsIndicator");

		public static readonly StringName _noScoreUploadIndicator = StringName.op_Implicit("_noScoreUploadIndicator");

		public static readonly StringName _currentPage = StringName.op_Implicit("_currentPage");

		public static readonly StringName _hasNegativeScore = StringName.op_Implicit("_hasNegativeScore");
	}

	public class SignalName : SignalName
	{
	}

	private static readonly string _scenePath = SceneHelper.GetScenePath("screens/daily_run/daily_run_leaderboard");

	private const int _maxEntries = 10;

	private MegaLabel _titleLabel;

	private NLeaderboardDayPaginator _paginator;

	private VBoxContainer _scoreContainer;

	private NLeaderboardPageArrow _leftArrow;

	private NLeaderboardPageArrow _rightArrow;

	private MegaRichTextLabel _loadingIndicator;

	private MegaLabel _noScoresIndicator;

	private MegaLabel _noFriendsIndicator;

	private Control? _noScoreUploadIndicator;

	private int _currentPage;

	private DateTimeOffset _todaysDailyTime;

	private DateTimeOffset _leaderboardTime;

	private readonly List<ulong> _playersInRun = new List<ulong>();

	private bool _hasNegativeScore;

	private static readonly LocString _titleLoc = new LocString("main_menu_ui", "DAILY_RUN_MENU.LEADERBOARDS.title");

	private static readonly LocString _scoreLoc = new LocString("main_menu_ui", "DAILY_RUN_MENU.LEADERBOARDS.noScore");

	private static readonly LocString _fetchingScoreLoc = new LocString("main_menu_ui", "DAILY_RUN_MENU.LEADERBOARDS.fetchingScores");

	private static readonly LocString _friendsLoc = new LocString("main_menu_ui", "DAILY_RUN_MENU.LEADERBOARDS.noFriends");

	public static string[] AssetPaths => new string[1] { _scenePath };

	public override void _Ready()
	{
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		_titleLabel = ((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%Title"));
		_paginator = ((Node)this).GetNode<NLeaderboardDayPaginator>(NodePath.op_Implicit("Paginator"));
		_scoreContainer = ((Node)this).GetNodeOrNull<VBoxContainer>(NodePath.op_Implicit("%ScoreContainer")) ?? ((Node)this).GetNodeOrNull<VBoxContainer>(NodePath.op_Implicit("%LeaderboardScoreContainer")) ?? throw new InvalidOperationException("Couldn't find score container");
		_leftArrow = ((Node)this).GetNode<NLeaderboardPageArrow>(NodePath.op_Implicit("%LeftArrow"));
		_rightArrow = ((Node)this).GetNode<NLeaderboardPageArrow>(NodePath.op_Implicit("%RightArrow"));
		_loadingIndicator = ((Node)this).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("%LoadingText"));
		_noScoresIndicator = ((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%NoScoresIndicator"));
		_noFriendsIndicator = ((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%NoFriendsIndicator"));
		_noScoreUploadIndicator = ((Node)this).GetNodeOrNull<Control>(NodePath.op_Implicit("%ScoreWarning"));
		((GodotObject)this).CallDeferred(MethodName.SetLocalizedText, Array.Empty<Variant>());
		_loadingIndicator.SetTextAutoSize(_fetchingScoreLoc.GetFormattedText());
		_rightArrow.Connect(delegate
		{
			ChangePage(1);
		});
		_leftArrow.Connect(delegate
		{
			ChangePage(-1);
		});
	}

	private void SetLocalizedText()
	{
		_titleLabel.SetTextAutoSize(_titleLoc.GetFormattedText());
		_noScoresIndicator.SetTextAutoSize(_scoreLoc.GetFormattedText());
		_noFriendsIndicator.SetTextAutoSize(_friendsLoc.GetFormattedText());
	}

	public void Cleanup()
	{
		((CanvasItem)_leftArrow).Visible = false;
		((CanvasItem)_rightArrow).Visible = false;
		((CanvasItem)_loadingIndicator).Visible = false;
		((CanvasItem)_noScoresIndicator).Visible = false;
		((CanvasItem)_noFriendsIndicator).Visible = false;
		((CanvasItem)_paginator).Visible = false;
		if (_noScoreUploadIndicator != null)
		{
			((CanvasItem)_noScoreUploadIndicator).Visible = false;
		}
		ClearEntries();
	}

	public void Initialize(DateTimeOffset dateTime, IEnumerable<ulong> playersInRun, bool allowPagination)
	{
		_playersInRun.Clear();
		_playersInRun.AddRange(playersInRun);
		_paginator.Initialize(this, dateTime, allowPagination);
		((CanvasItem)_paginator).Visible = true;
		_todaysDailyTime = dateTime;
		SetDay(dateTime);
	}

	public void SetDay(DateTimeOffset dateTime)
	{
		_leaderboardTime = dateTime;
		SetPage(0);
	}

	private void ChangePage(int increment)
	{
		SetPage(_currentPage + increment);
	}

	private void SetPage(int page)
	{
		_currentPage = page;
		TaskHelper.RunSafely(LoadLeaderboard(_leaderboardTime, _currentPage));
	}

	private async Task LoadLeaderboard(DateTimeOffset dateTime, int page)
	{
		ClearEntries();
		_rightArrow.Disable();
		_leftArrow.Disable();
		_paginator.Disable();
		((CanvasItem)_noFriendsIndicator).Visible = false;
		((CanvasItem)_noScoresIndicator).Visible = false;
		((CanvasItem)_loadingIndicator).Visible = true;
		string leaderboardName = DailyRunUtility.GetLeaderboardName(dateTime, _playersInRun.Count);
		DateTimeOffset dateTime2 = dateTime - TimeSpan.FromDays(1);
		DateTimeOffset rightLeaderboardTime = dateTime + TimeSpan.FromDays(1);
		Task<ILeaderboardHandle?> mainTask = LeaderboardManager.GetLeaderboard(leaderboardName);
		Task<ILeaderboardHandle?> leftTask = LeaderboardManager.GetLeaderboard(DailyRunUtility.GetLeaderboardName(dateTime2, _playersInRun.Count));
		Task<ILeaderboardHandle?> rightTask = LeaderboardManager.GetLeaderboard(DailyRunUtility.GetLeaderboardName(rightLeaderboardTime, _playersInRun.Count));
		global::_003C_003Ey__InlineArray3<Task<ILeaderboardHandle>> buffer = default(global::_003C_003Ey__InlineArray3<Task<ILeaderboardHandle>>);
		buffer[0] = mainTask;
		buffer[1] = leftTask;
		buffer[2] = rightTask;
		await Task.WhenAll<ILeaderboardHandle>(buffer);
		ILeaderboardHandle handle = await mainTask;
		if (handle != null)
		{
			List<LeaderboardEntry> list = await LeaderboardManager.QueryLeaderboard(handle, LeaderboardQueryType.Global, page * 10, 10);
			((CanvasItem)_noScoresIndicator).Visible = list.Count <= 0;
			FillEntries(list);
			((CanvasItem)_leftArrow).Visible = true;
			((CanvasItem)_rightArrow).Visible = true;
			if (page > 0)
			{
				_leftArrow.Enable();
			}
			else
			{
				_leftArrow.Disable();
			}
			if (page * 10 + 10 < LeaderboardManager.GetLeaderboardEntryCount(handle) && !_hasNegativeScore)
			{
				_rightArrow.Enable();
			}
			else
			{
				_rightArrow.Disable();
			}
		}
		else
		{
			((CanvasItem)_noScoresIndicator).Visible = true;
			((CanvasItem)_leftArrow).Visible = false;
			((CanvasItem)_rightArrow).Visible = false;
		}
		bool hasLeftLeaderboard = await leftTask != null;
		bool rightArrowEnabled = await rightTask != null || rightLeaderboardTime == _todaysDailyTime;
		_currentPage = page;
		_paginator.Enable(hasLeftLeaderboard, rightArrowEnabled);
		((CanvasItem)_loadingIndicator).Visible = false;
		if (_noScoreUploadIndicator != null && _todaysDailyTime == dateTime)
		{
			bool flag = await DailyRunUtility.ShouldUploadScore(handle, _playersInRun);
			((CanvasItem)_noScoreUploadIndicator).Visible = !flag;
		}
	}

	private void FillEntries(List<LeaderboardEntry> entries)
	{
		if (entries.Count == 0)
		{
			return;
		}
		_hasNegativeScore = false;
		NDailyRunLeaderboardRow child = NDailyRunLeaderboardRow.CreateHeader();
		((Node)(object)_scoreContainer).AddChildSafely((Node?)(object)child);
		NDailyRunLeaderboardSeparator child2 = NDailyRunLeaderboardSeparator.Create();
		((Node)(object)_scoreContainer).AddChildSafely((Node?)(object)child2);
		foreach (LeaderboardEntry entry in entries)
		{
			if (entry.score < 0)
			{
				_hasNegativeScore = true;
				continue;
			}
			((Node)(object)_scoreContainer).AddChildSafely((Node?)(object)NDailyRunLeaderboardRow.Create(entry));
			((Node)(object)_scoreContainer).AddChildSafely((Node?)(object)NDailyRunLeaderboardSeparator.Create());
		}
	}

	private void ClearEntries()
	{
		((CanvasItem)_noScoresIndicator).Visible = false;
		((CanvasItem)_noFriendsIndicator).Visible = false;
		foreach (Node child in ((Node)_scoreContainer).GetChildren(false))
		{
			child.QueueFreeSafely();
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(6);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetLocalizedText, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.Cleanup, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ChangePage, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("increment"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetPage, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("page"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ClearEntries, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetLocalizedText && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			SetLocalizedText();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.Cleanup && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			Cleanup();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.ChangePage && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			ChangePage(VariantUtils.ConvertTo<int>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetPage && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			SetPage(VariantUtils.ConvertTo<int>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.ClearEntries && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			ClearEntries();
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
		if ((ref method) == MethodName.SetLocalizedText)
		{
			return true;
		}
		if ((ref method) == MethodName.Cleanup)
		{
			return true;
		}
		if ((ref method) == MethodName.ChangePage)
		{
			return true;
		}
		if ((ref method) == MethodName.SetPage)
		{
			return true;
		}
		if ((ref method) == MethodName.ClearEntries)
		{
			return true;
		}
		return ((Control)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._titleLabel)
		{
			_titleLabel = VariantUtils.ConvertTo<MegaLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._paginator)
		{
			_paginator = VariantUtils.ConvertTo<NLeaderboardDayPaginator>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._scoreContainer)
		{
			_scoreContainer = VariantUtils.ConvertTo<VBoxContainer>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._leftArrow)
		{
			_leftArrow = VariantUtils.ConvertTo<NLeaderboardPageArrow>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._rightArrow)
		{
			_rightArrow = VariantUtils.ConvertTo<NLeaderboardPageArrow>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._loadingIndicator)
		{
			_loadingIndicator = VariantUtils.ConvertTo<MegaRichTextLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._noScoresIndicator)
		{
			_noScoresIndicator = VariantUtils.ConvertTo<MegaLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._noFriendsIndicator)
		{
			_noFriendsIndicator = VariantUtils.ConvertTo<MegaLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._noScoreUploadIndicator)
		{
			_noScoreUploadIndicator = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._currentPage)
		{
			_currentPage = VariantUtils.ConvertTo<int>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._hasNegativeScore)
		{
			_hasNegativeScore = VariantUtils.ConvertTo<bool>(ref value);
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
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName._titleLabel)
		{
			value = VariantUtils.CreateFrom<MegaLabel>(ref _titleLabel);
			return true;
		}
		if ((ref name) == PropertyName._paginator)
		{
			value = VariantUtils.CreateFrom<NLeaderboardDayPaginator>(ref _paginator);
			return true;
		}
		if ((ref name) == PropertyName._scoreContainer)
		{
			value = VariantUtils.CreateFrom<VBoxContainer>(ref _scoreContainer);
			return true;
		}
		if ((ref name) == PropertyName._leftArrow)
		{
			value = VariantUtils.CreateFrom<NLeaderboardPageArrow>(ref _leftArrow);
			return true;
		}
		if ((ref name) == PropertyName._rightArrow)
		{
			value = VariantUtils.CreateFrom<NLeaderboardPageArrow>(ref _rightArrow);
			return true;
		}
		if ((ref name) == PropertyName._loadingIndicator)
		{
			value = VariantUtils.CreateFrom<MegaRichTextLabel>(ref _loadingIndicator);
			return true;
		}
		if ((ref name) == PropertyName._noScoresIndicator)
		{
			value = VariantUtils.CreateFrom<MegaLabel>(ref _noScoresIndicator);
			return true;
		}
		if ((ref name) == PropertyName._noFriendsIndicator)
		{
			value = VariantUtils.CreateFrom<MegaLabel>(ref _noFriendsIndicator);
			return true;
		}
		if ((ref name) == PropertyName._noScoreUploadIndicator)
		{
			value = VariantUtils.CreateFrom<Control>(ref _noScoreUploadIndicator);
			return true;
		}
		if ((ref name) == PropertyName._currentPage)
		{
			value = VariantUtils.CreateFrom<int>(ref _currentPage);
			return true;
		}
		if ((ref name) == PropertyName._hasNegativeScore)
		{
			value = VariantUtils.CreateFrom<bool>(ref _hasNegativeScore);
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
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._titleLabel, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._paginator, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._scoreContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._leftArrow, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._rightArrow, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._loadingIndicator, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._noScoresIndicator, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._noFriendsIndicator, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._noScoreUploadIndicator, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)2, PropertyName._currentPage, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._hasNegativeScore, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		((GodotObject)this).SaveGodotObjectData(info);
		info.AddProperty(PropertyName._titleLabel, Variant.From<MegaLabel>(ref _titleLabel));
		info.AddProperty(PropertyName._paginator, Variant.From<NLeaderboardDayPaginator>(ref _paginator));
		info.AddProperty(PropertyName._scoreContainer, Variant.From<VBoxContainer>(ref _scoreContainer));
		info.AddProperty(PropertyName._leftArrow, Variant.From<NLeaderboardPageArrow>(ref _leftArrow));
		info.AddProperty(PropertyName._rightArrow, Variant.From<NLeaderboardPageArrow>(ref _rightArrow));
		info.AddProperty(PropertyName._loadingIndicator, Variant.From<MegaRichTextLabel>(ref _loadingIndicator));
		info.AddProperty(PropertyName._noScoresIndicator, Variant.From<MegaLabel>(ref _noScoresIndicator));
		info.AddProperty(PropertyName._noFriendsIndicator, Variant.From<MegaLabel>(ref _noFriendsIndicator));
		info.AddProperty(PropertyName._noScoreUploadIndicator, Variant.From<Control>(ref _noScoreUploadIndicator));
		info.AddProperty(PropertyName._currentPage, Variant.From<int>(ref _currentPage));
		info.AddProperty(PropertyName._hasNegativeScore, Variant.From<bool>(ref _hasNegativeScore));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._titleLabel, ref val))
		{
			_titleLabel = ((Variant)(ref val)).As<MegaLabel>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._paginator, ref val2))
		{
			_paginator = ((Variant)(ref val2)).As<NLeaderboardDayPaginator>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._scoreContainer, ref val3))
		{
			_scoreContainer = ((Variant)(ref val3)).As<VBoxContainer>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._leftArrow, ref val4))
		{
			_leftArrow = ((Variant)(ref val4)).As<NLeaderboardPageArrow>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._rightArrow, ref val5))
		{
			_rightArrow = ((Variant)(ref val5)).As<NLeaderboardPageArrow>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._loadingIndicator, ref val6))
		{
			_loadingIndicator = ((Variant)(ref val6)).As<MegaRichTextLabel>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName._noScoresIndicator, ref val7))
		{
			_noScoresIndicator = ((Variant)(ref val7)).As<MegaLabel>();
		}
		Variant val8 = default(Variant);
		if (info.TryGetProperty(PropertyName._noFriendsIndicator, ref val8))
		{
			_noFriendsIndicator = ((Variant)(ref val8)).As<MegaLabel>();
		}
		Variant val9 = default(Variant);
		if (info.TryGetProperty(PropertyName._noScoreUploadIndicator, ref val9))
		{
			_noScoreUploadIndicator = ((Variant)(ref val9)).As<Control>();
		}
		Variant val10 = default(Variant);
		if (info.TryGetProperty(PropertyName._currentPage, ref val10))
		{
			_currentPage = ((Variant)(ref val10)).As<int>();
		}
		Variant val11 = default(Variant);
		if (info.TryGetProperty(PropertyName._hasNegativeScore, ref val11))
		{
			_hasNegativeScore = ((Variant)(ref val11)).As<bool>();
		}
	}
}
