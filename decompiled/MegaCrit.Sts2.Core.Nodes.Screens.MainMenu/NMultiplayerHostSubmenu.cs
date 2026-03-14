using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Multiplayer;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Screens.CharacterSelect;
using MegaCrit.Sts2.Core.Nodes.Screens.CustomRun;
using MegaCrit.Sts2.Core.Nodes.Screens.DailyRun;
using MegaCrit.Sts2.Core.Platform;
using MegaCrit.Sts2.Core.Platform.Steam;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.TestSupport;
using MegaCrit.Sts2.Core.Timeline.Epochs;

namespace MegaCrit.Sts2.Core.Nodes.Screens.MainMenu;

[ScriptPath("res://src/Core/Nodes/Screens/MainMenu/NMultiplayerHostSubmenu.cs")]
public class NMultiplayerHostSubmenu : NSubmenu
{
	public new class MethodName : NSubmenu.MethodName
	{
		public static readonly StringName Create = StringName.op_Implicit("Create");

		public new static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName RefreshButtons = StringName.op_Implicit("RefreshButtons");

		public new static readonly StringName OnSubmenuOpened = StringName.op_Implicit("OnSubmenuOpened");

		public static readonly StringName OnStandardPressed = StringName.op_Implicit("OnStandardPressed");

		public static readonly StringName OnDailyPressed = StringName.op_Implicit("OnDailyPressed");

		public static readonly StringName OnCustomPressed = StringName.op_Implicit("OnCustomPressed");

		public static readonly StringName StartHost = StringName.op_Implicit("StartHost");
	}

	public new class PropertyName : NSubmenu.PropertyName
	{
		public new static readonly StringName InitialFocusedControl = StringName.op_Implicit("InitialFocusedControl");

		public static readonly StringName _standardButton = StringName.op_Implicit("_standardButton");

		public static readonly StringName _dailyButton = StringName.op_Implicit("_dailyButton");

		public static readonly StringName _customButton = StringName.op_Implicit("_customButton");

		public static readonly StringName _loadingOverlay = StringName.op_Implicit("_loadingOverlay");
	}

	public new class SignalName : NSubmenu.SignalName
	{
	}

	private static readonly string _scenePath = SceneHelper.GetScenePath("screens/multiplayer_host_submenu");

	private NSubmenuButton _standardButton;

	private NSubmenuButton _dailyButton;

	private NSubmenuButton _customButton;

	private const string _keyStandard = "STANDARD_MP";

	private const string _keyDaily = "DAILY_MP";

	private const string _keyCustom = "CUSTOM_MP";

	private Control _loadingOverlay;

	protected override Control InitialFocusedControl => (Control)(object)_standardButton;

	public static NMultiplayerHostSubmenu? Create()
	{
		if (TestMode.IsOn)
		{
			return null;
		}
		return PreloadManager.Cache.GetScene(_scenePath).Instantiate<NMultiplayerHostSubmenu>((GenEditState)0);
	}

	public override void _Ready()
	{
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		ConnectSignals();
		_loadingOverlay = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%LoadingOverlay"));
		_standardButton = ((Node)this).GetNode<NSubmenuButton>(NodePath.op_Implicit("StandardButton"));
		((GodotObject)_standardButton).Connect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)OnStandardPressed), 0u);
		_standardButton.SetIconAndLocalization("STANDARD_MP");
		_dailyButton = ((Node)this).GetNode<NSubmenuButton>(NodePath.op_Implicit("DailyButton"));
		((GodotObject)_dailyButton).Connect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)OnDailyPressed), 0u);
		_dailyButton.SetIconAndLocalization("DAILY_MP");
		_customButton = ((Node)this).GetNode<NSubmenuButton>(NodePath.op_Implicit("CustomRunButton"));
		((GodotObject)_customButton).Connect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)OnCustomPressed), 0u);
		_customButton.SetIconAndLocalization("CUSTOM_MP");
	}

	private void RefreshButtons()
	{
		_dailyButton.SetEnabled(SaveManager.Instance.IsEpochRevealed<DailyRunEpoch>());
		_customButton.SetEnabled(SaveManager.Instance.IsEpochRevealed<CustomAndSeedsEpoch>());
	}

	public override void OnSubmenuOpened()
	{
		RefreshButtons();
	}

	private void OnStandardPressed(NButton _)
	{
		StartHost(GameMode.Standard);
	}

	private void OnDailyPressed(NButton _)
	{
		StartHost(GameMode.Daily);
	}

	private void OnCustomPressed(NButton _)
	{
		StartHost(GameMode.Custom);
	}

	public void StartHost(GameMode gameMode)
	{
		TaskHelper.RunSafely(StartHostAsync(gameMode, _loadingOverlay, _stack));
	}

	public static async Task StartHostAsync(GameMode gameMode, Control loadingOverlay, NSubmenuStack stack)
	{
		PlatformType platformType = ((SteamInitializer.Initialized && !CommandLineHelper.HasArg("fastmp")) ? PlatformType.Steam : PlatformType.None);
		((CanvasItem)loadingOverlay).Visible = true;
		try
		{
			NetHostGameService netService = new NetHostGameService();
			NetErrorInfo? netErrorInfo = null;
			if (platformType == PlatformType.Steam)
			{
				netErrorInfo = await netService.StartSteamHost(4);
			}
			else
			{
				netService.StartENetHost(33771, 4);
			}
			if (!netErrorInfo.HasValue)
			{
				switch (gameMode)
				{
				case GameMode.Standard:
				{
					NCharacterSelectScreen submenuType3 = stack.GetSubmenuType<NCharacterSelectScreen>();
					submenuType3.InitializeMultiplayerAsHost(netService, 4);
					stack.Push(submenuType3);
					break;
				}
				case GameMode.Daily:
				{
					NDailyRunScreen submenuType2 = stack.GetSubmenuType<NDailyRunScreen>();
					submenuType2.InitializeMultiplayerAsHost(netService);
					stack.Push(submenuType2);
					break;
				}
				default:
				{
					NCustomRunScreen submenuType = stack.GetSubmenuType<NCustomRunScreen>();
					submenuType.InitializeMultiplayerAsHost(netService, 4);
					stack.Push(submenuType);
					break;
				}
				}
			}
			else
			{
				NErrorPopup nErrorPopup = NErrorPopup.Create(netErrorInfo.Value);
				if (nErrorPopup != null)
				{
					NModalContainer.Instance.Add((Node)(object)nErrorPopup);
				}
			}
		}
		catch
		{
			NErrorPopup nErrorPopup2 = NErrorPopup.Create(new NetErrorInfo(NetError.InternalError, selfInitiated: false));
			if (nErrorPopup2 != null)
			{
				NModalContainer.Instance.Add((Node)(object)nErrorPopup2);
			}
			throw;
		}
		finally
		{
			((CanvasItem)loadingOverlay).Visible = false;
		}
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
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Expected O, but got Unknown
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Expected O, but got Unknown
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Expected O, but got Unknown
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(8);
		list.Add(new MethodInfo(MethodName.Create, new PropertyInfo((Type)24, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false), (MethodFlags)33, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.RefreshButtons, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnSubmenuOpened, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnStandardPressed, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnDailyPressed, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnCustomPressed, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.StartHost, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("gameMode"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
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
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.Create && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			NMultiplayerHostSubmenu nMultiplayerHostSubmenu = Create();
			ret = VariantUtils.CreateFrom<NMultiplayerHostSubmenu>(ref nMultiplayerHostSubmenu);
			return true;
		}
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.RefreshButtons && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			RefreshButtons();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnSubmenuOpened && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnSubmenuOpened();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnStandardPressed && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OnStandardPressed(VariantUtils.ConvertTo<NButton>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnDailyPressed && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OnDailyPressed(VariantUtils.ConvertTo<NButton>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnCustomPressed && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OnCustomPressed(VariantUtils.ConvertTo<NButton>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.StartHost && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			StartHost(VariantUtils.ConvertTo<GameMode>(ref ((NativeVariantPtrArgs)(ref args))[0]));
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
			NMultiplayerHostSubmenu nMultiplayerHostSubmenu = Create();
			ret = VariantUtils.CreateFrom<NMultiplayerHostSubmenu>(ref nMultiplayerHostSubmenu);
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
		if ((ref method) == MethodName.RefreshButtons)
		{
			return true;
		}
		if ((ref method) == MethodName.OnSubmenuOpened)
		{
			return true;
		}
		if ((ref method) == MethodName.OnStandardPressed)
		{
			return true;
		}
		if ((ref method) == MethodName.OnDailyPressed)
		{
			return true;
		}
		if ((ref method) == MethodName.OnCustomPressed)
		{
			return true;
		}
		if ((ref method) == MethodName.StartHost)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._standardButton)
		{
			_standardButton = VariantUtils.ConvertTo<NSubmenuButton>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._dailyButton)
		{
			_dailyButton = VariantUtils.ConvertTo<NSubmenuButton>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._customButton)
		{
			_customButton = VariantUtils.ConvertTo<NSubmenuButton>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._loadingOverlay)
		{
			_loadingOverlay = VariantUtils.ConvertTo<Control>(ref value);
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
		if ((ref name) == PropertyName.InitialFocusedControl)
		{
			Control initialFocusedControl = InitialFocusedControl;
			value = VariantUtils.CreateFrom<Control>(ref initialFocusedControl);
			return true;
		}
		if ((ref name) == PropertyName._standardButton)
		{
			value = VariantUtils.CreateFrom<NSubmenuButton>(ref _standardButton);
			return true;
		}
		if ((ref name) == PropertyName._dailyButton)
		{
			value = VariantUtils.CreateFrom<NSubmenuButton>(ref _dailyButton);
			return true;
		}
		if ((ref name) == PropertyName._customButton)
		{
			value = VariantUtils.CreateFrom<NSubmenuButton>(ref _customButton);
			return true;
		}
		if ((ref name) == PropertyName._loadingOverlay)
		{
			value = VariantUtils.CreateFrom<Control>(ref _loadingOverlay);
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
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._standardButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._dailyButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._customButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._loadingOverlay, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName._standardButton, Variant.From<NSubmenuButton>(ref _standardButton));
		info.AddProperty(PropertyName._dailyButton, Variant.From<NSubmenuButton>(ref _dailyButton));
		info.AddProperty(PropertyName._customButton, Variant.From<NSubmenuButton>(ref _customButton));
		info.AddProperty(PropertyName._loadingOverlay, Variant.From<Control>(ref _loadingOverlay));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._standardButton, ref val))
		{
			_standardButton = ((Variant)(ref val)).As<NSubmenuButton>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._dailyButton, ref val2))
		{
			_dailyButton = ((Variant)(ref val2)).As<NSubmenuButton>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._customButton, ref val3))
		{
			_customButton = ((Variant)(ref val3)).As<NSubmenuButton>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._loadingOverlay, ref val4))
		{
			_loadingOverlay = ((Variant)(ref val4)).As<Control>();
		}
	}
}
