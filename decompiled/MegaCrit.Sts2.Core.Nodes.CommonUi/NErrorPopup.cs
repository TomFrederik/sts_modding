using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Screens.FeedbackScreen;
using MegaCrit.Sts2.Core.Nodes.Screens.ScreenContext;
using MegaCrit.Sts2.Core.TestSupport;

namespace MegaCrit.Sts2.Core.Nodes.CommonUi;

[ScriptPath("res://src/Core/Nodes/CommonUi/NErrorPopup.cs")]
public class NErrorPopup : NVerticalPopup, IScreenContext
{
	public new class MethodName : NVerticalPopup.MethodName
	{
		public new static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName Create = StringName.op_Implicit("Create");

		public static readonly StringName OnOkButtonPressed = StringName.op_Implicit("OnOkButtonPressed");

		public static readonly StringName OnCancelButtonPressed = StringName.op_Implicit("OnCancelButtonPressed");

		public static readonly StringName OnReportBugButtonPressed = StringName.op_Implicit("OnReportBugButtonPressed");
	}

	public new class PropertyName : NVerticalPopup.PropertyName
	{
		public static readonly StringName DefaultFocusedControl = StringName.op_Implicit("DefaultFocusedControl");

		public static readonly StringName _verticalPopup = StringName.op_Implicit("_verticalPopup");

		public static readonly StringName _title = StringName.op_Implicit("_title");

		public static readonly StringName _body = StringName.op_Implicit("_body");

		public static readonly StringName _showReportBugButton = StringName.op_Implicit("_showReportBugButton");
	}

	public new class SignalName : NVerticalPopup.SignalName
	{
	}

	private NVerticalPopup _verticalPopup;

	private string _title;

	private string _body;

	private LocString? _cancel;

	private bool _showReportBugButton;

	private static readonly string _scenePath = SceneHelper.GetScenePath("ui/error_popup");

	public Control? DefaultFocusedControl => null;

	public new static IEnumerable<string> AssetPaths => new _003C_003Ez__ReadOnlySingleElementList<string>(_scenePath);

	public override void _Ready()
	{
		_verticalPopup = ((Node)this).GetNode<NVerticalPopup>(NodePath.op_Implicit("VerticalPopup"));
		_verticalPopup.SetText(_title, _body);
		if (_showReportBugButton)
		{
			_verticalPopup.InitYesButton(new LocString("main_menu_ui", "NETWORK_ERROR.report_bug"), OnReportBugButtonPressed);
		}
		else
		{
			_verticalPopup.InitYesButton(new LocString("main_menu_ui", "GENERIC_POPUP.ok"), OnOkButtonPressed);
		}
		if (_cancel != null)
		{
			_verticalPopup.InitNoButton(_cancel, OnCancelButtonPressed);
		}
		else
		{
			_verticalPopup.HideNoButton();
		}
	}

	public static NErrorPopup? Create(NetErrorInfo info)
	{
		if (TestMode.IsOn)
		{
			return null;
		}
		if (info.SelfInitiated && info.GetReason() == NetError.Quit)
		{
			return null;
		}
		bool showReportBugButton;
		return Create(new LocString("main_menu_ui", "NETWORK_ERROR.header"), LocStringFromNetError(info, out showReportBugButton), null, showReportBugButton);
	}

	public static NErrorPopup? Create(LocString title, LocString body, LocString? cancel, bool showReportBugButton)
	{
		if (TestMode.IsOn)
		{
			return null;
		}
		NErrorPopup nErrorPopup = PreloadManager.Cache.GetScene(_scenePath).Instantiate<NErrorPopup>((GenEditState)0);
		nErrorPopup._title = title.GetFormattedText();
		nErrorPopup._body = body.GetFormattedText();
		nErrorPopup._showReportBugButton = showReportBugButton;
		nErrorPopup._cancel = cancel;
		return nErrorPopup;
	}

	public static NErrorPopup? Create(string title, string body, bool showReportBugButton)
	{
		if (TestMode.IsOn)
		{
			return null;
		}
		NErrorPopup nErrorPopup = PreloadManager.Cache.GetScene(_scenePath).Instantiate<NErrorPopup>((GenEditState)0);
		nErrorPopup._title = title;
		nErrorPopup._body = body;
		nErrorPopup._showReportBugButton = showReportBugButton;
		return nErrorPopup;
	}

	private static LocString LocStringFromNetError(NetErrorInfo info, out bool showReportBugButton)
	{
		NetError reason = info.GetReason();
		string text = default(string);
		switch (reason)
		{
		case NetError.None:
			text = null;
			break;
		case NetError.QuitGameOver:
			text = null;
			break;
		case NetError.CancelledJoin:
			text = null;
			break;
		case NetError.LobbyFull:
			text = "NETWORK_ERROR.LOBBY_FULL.body";
			break;
		case NetError.Quit:
			text = "NETWORK_ERROR.QUIT.body";
			break;
		case NetError.HostAbandoned:
			text = "NETWORK_ERROR.HOST_ABANDONED.body";
			break;
		case NetError.Kicked:
			text = "NETWORK_ERROR.KICKED.body";
			break;
		case NetError.InvalidJoin:
			text = "NETWORK_ERROR.INVALID_JOIN.body";
			break;
		case NetError.RunInProgress:
			text = "NETWORK_ERROR.RUN_IN_PROGRESS.body";
			break;
		case NetError.StateDivergence:
			text = "NETWORK_ERROR.STATE_DIVERGENCE.body";
			break;
		case NetError.ModMismatch:
			text = "NETWORK_ERROR.MOD_MISMATCH.body";
			break;
		case NetError.JoinBlockedByUser:
			text = "NETWORK_ERROR.JOIN_BLOCKED_BY_USER.body";
			break;
		case NetError.NoInternet:
			text = "NETWORK_ERROR.NO_INTERNET.body";
			break;
		case NetError.Timeout:
			text = "NETWORK_ERROR.TIMEOUT.body";
			break;
		case NetError.HandshakeTimeout:
			text = "NETWORK_ERROR.TIMEOUT.body";
			break;
		case NetError.InternalError:
			text = "NETWORK_ERROR.INTERNAL_ERROR.body";
			break;
		case NetError.UnknownNetworkError:
			text = "NETWORK_ERROR.UNKNOWN_ERROR.body";
			break;
		case NetError.TryAgainLater:
			text = "NETWORK_ERROR.TRY_AGAIN_LATER.body";
			break;
		case NetError.FailedToHost:
			text = "NETWORK_ERROR.FAILED_TO_HOST.body";
			break;
		case NetError.NotInSaveGame:
			text = "NETWORK_ERROR.NOT_IN_SAVE_GAME.body";
			break;
		case NetError.VersionMismatch:
			text = "NETWORK_ERROR.VERSION_MISMATCH.body";
			break;
		default:
			global::_003CPrivateImplementationDetails_003E.ThrowSwitchExpressionException(reason);
			break;
		}
		string text2 = text;
		bool flag = ((reason == NetError.None || reason == NetError.StateDivergence || (uint)(reason - 17) <= 1u) ? true : false);
		showReportBugButton = flag;
		if (text2 == null)
		{
			Log.Error($"Invalid net error passed to NNetworkErrorPopup: {info}!");
			text2 = "NETWORK_ERROR.INTERNAL_ERROR.body";
			showReportBugButton = true;
		}
		LocString locString = new LocString("main_menu_ui", text2);
		locString.Add("info", info.GetErrorString());
		return locString;
	}

	private void OnOkButtonPressed(NButton _)
	{
		((Node)(object)this).QueueFreeSafely();
	}

	private void OnCancelButtonPressed(NButton _)
	{
		((Node)(object)this).QueueFreeSafely();
	}

	private void OnReportBugButtonPressed(NButton _)
	{
		TaskHelper.RunSafely(OpenFeedbackScreen());
	}

	private async Task OpenFeedbackScreen()
	{
		SceneTree sceneTree = ((Node)this).GetTree();
		((Node)(object)this).QueueFreeSafely();
		await ((GodotObject)sceneTree).ToSignal((GodotObject)(object)sceneTree, SignalName.ProcessFrame);
		await ((GodotObject)sceneTree).ToSignal((GodotObject)(object)sceneTree, SignalName.ProcessFrame);
		await NFeedbackScreenOpener.Instance.OpenFeedbackScreen();
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Expected O, but got Unknown
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Expected O, but got Unknown
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Expected O, but got Unknown
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Expected O, but got Unknown
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(5);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.Create, new PropertyInfo((Type)24, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false), (MethodFlags)33, new List<PropertyInfo>
		{
			new PropertyInfo((Type)4, StringName.op_Implicit("title"), (PropertyHint)0, "", (PropertyUsageFlags)6, false),
			new PropertyInfo((Type)4, StringName.op_Implicit("body"), (PropertyHint)0, "", (PropertyUsageFlags)6, false),
			new PropertyInfo((Type)1, StringName.op_Implicit("showReportBugButton"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnOkButtonPressed, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnCancelButtonPressed, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnReportBugButtonPressed, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.Create && ((NativeVariantPtrArgs)(ref args)).Count == 3)
		{
			NErrorPopup nErrorPopup = Create(VariantUtils.ConvertTo<string>(ref ((NativeVariantPtrArgs)(ref args))[0]), VariantUtils.ConvertTo<string>(ref ((NativeVariantPtrArgs)(ref args))[1]), VariantUtils.ConvertTo<bool>(ref ((NativeVariantPtrArgs)(ref args))[2]));
			ret = VariantUtils.CreateFrom<NErrorPopup>(ref nErrorPopup);
			return true;
		}
		if ((ref method) == MethodName.OnOkButtonPressed && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OnOkButtonPressed(VariantUtils.ConvertTo<NButton>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnCancelButtonPressed && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OnCancelButtonPressed(VariantUtils.ConvertTo<NButton>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnReportBugButtonPressed && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OnReportBugButtonPressed(VariantUtils.ConvertTo<NButton>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		return base.InvokeGodotClassMethod(in method, args, out ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static bool InvokeGodotClassStaticMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.Create && ((NativeVariantPtrArgs)(ref args)).Count == 3)
		{
			NErrorPopup nErrorPopup = Create(VariantUtils.ConvertTo<string>(ref ((NativeVariantPtrArgs)(ref args))[0]), VariantUtils.ConvertTo<string>(ref ((NativeVariantPtrArgs)(ref args))[1]), VariantUtils.ConvertTo<bool>(ref ((NativeVariantPtrArgs)(ref args))[2]));
			ret = VariantUtils.CreateFrom<NErrorPopup>(ref nErrorPopup);
			return true;
		}
		ret = default(godot_variant);
		return false;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if ((ref method) == MethodName._Ready)
		{
			return true;
		}
		if ((ref method) == MethodName.Create)
		{
			return true;
		}
		if ((ref method) == MethodName.OnOkButtonPressed)
		{
			return true;
		}
		if ((ref method) == MethodName.OnCancelButtonPressed)
		{
			return true;
		}
		if ((ref method) == MethodName.OnReportBugButtonPressed)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._verticalPopup)
		{
			_verticalPopup = VariantUtils.ConvertTo<NVerticalPopup>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._title)
		{
			_title = VariantUtils.ConvertTo<string>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._body)
		{
			_body = VariantUtils.ConvertTo<string>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._showReportBugButton)
		{
			_showReportBugButton = VariantUtils.ConvertTo<bool>(ref value);
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
		if ((ref name) == PropertyName.DefaultFocusedControl)
		{
			Control defaultFocusedControl = DefaultFocusedControl;
			value = VariantUtils.CreateFrom<Control>(ref defaultFocusedControl);
			return true;
		}
		if ((ref name) == PropertyName._verticalPopup)
		{
			value = VariantUtils.CreateFrom<NVerticalPopup>(ref _verticalPopup);
			return true;
		}
		if ((ref name) == PropertyName._title)
		{
			value = VariantUtils.CreateFrom<string>(ref _title);
			return true;
		}
		if ((ref name) == PropertyName._body)
		{
			value = VariantUtils.CreateFrom<string>(ref _body);
			return true;
		}
		if ((ref name) == PropertyName._showReportBugButton)
		{
			value = VariantUtils.CreateFrom<bool>(ref _showReportBugButton);
			return true;
		}
		return base.GetGodotClassPropertyValue(in name, out value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._verticalPopup, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)4, PropertyName._title, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)4, PropertyName._body, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._showReportBugButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.DefaultFocusedControl, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName._verticalPopup, Variant.From<NVerticalPopup>(ref _verticalPopup));
		info.AddProperty(PropertyName._title, Variant.From<string>(ref _title));
		info.AddProperty(PropertyName._body, Variant.From<string>(ref _body));
		info.AddProperty(PropertyName._showReportBugButton, Variant.From<bool>(ref _showReportBugButton));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._verticalPopup, ref val))
		{
			_verticalPopup = ((Variant)(ref val)).As<NVerticalPopup>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._title, ref val2))
		{
			_title = ((Variant)(ref val2)).As<string>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._body, ref val3))
		{
			_body = ((Variant)(ref val3)).As<string>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._showReportBugButton, ref val4))
		{
			_showReportBugButton = ((Variant)(ref val4)).As<bool>();
		}
	}
}
