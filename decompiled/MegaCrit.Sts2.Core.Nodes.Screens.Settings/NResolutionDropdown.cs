using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Platform;
using MegaCrit.Sts2.Core.Saves;

namespace MegaCrit.Sts2.Core.Nodes.Screens.Settings;

[ScriptPath("res://src/Core/Nodes/Screens/Settings/NResolutionDropdown.cs")]
public class NResolutionDropdown : NSettingsDropdown
{
	public new class MethodName : NSettingsDropdown.MethodName
	{
		public static readonly StringName _EnterTree = StringName.op_Implicit("_EnterTree");

		public new static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName RefreshCurrentlySelectedResolution = StringName.op_Implicit("RefreshCurrentlySelectedResolution");

		public static readonly StringName PopulateDropdownItems = StringName.op_Implicit("PopulateDropdownItems");

		public static readonly StringName OnWindowChange = StringName.op_Implicit("OnWindowChange");

		public static readonly StringName RefreshEnabled = StringName.op_Implicit("RefreshEnabled");

		public new static readonly StringName OnEnable = StringName.op_Implicit("OnEnable");

		public new static readonly StringName OnDisable = StringName.op_Implicit("OnDisable");

		public static readonly StringName OnDropdownItemSelected = StringName.op_Implicit("OnDropdownItemSelected");

		public static readonly StringName DoesResolutionFit = StringName.op_Implicit("DoesResolutionFit");
	}

	public new class PropertyName : NSettingsDropdown.PropertyName
	{
		public static readonly StringName _dropdownItemScene = StringName.op_Implicit("_dropdownItemScene");

		public static readonly StringName _arrow = StringName.op_Implicit("_arrow");
	}

	public new class SignalName : NSettingsDropdown.SignalName
	{
	}

	[Export(/*Could not decode attribute arguments.*/)]
	private PackedScene _dropdownItemScene;

	private Control _arrow;

	private static Vector2I _currentResolution;

	public static NResolutionDropdown? Instance { get; private set; }

	public override void _EnterTree()
	{
		Instance = this;
	}

	public override void _Ready()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		ConnectSignals();
		_arrow = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%Arrow"));
		((GodotObject)NGame.Instance).Connect(NGame.SignalName.WindowChange, Callable.From<bool>((Action<bool>)OnWindowChange), 0u);
		RefreshEnabled();
		RefreshCurrentlySelectedResolution();
		PopulateDropdownItems();
	}

	public void RefreshCurrentlySelectedResolution()
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		if (base.IsEnabled)
		{
			_currentResolution = DisplayServer.WindowGetSize(0);
			_currentOptionLabel.SetTextAutoSize($"{_currentResolution.X} x {_currentResolution.Y}");
		}
	}

	public void PopulateDropdownItems()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		ClearDropdownItems();
		Vector2I boundaryResolution = DisplayServer.ScreenGetSize(SaveManager.Instance.SettingsSave.TargetDisplay);
		foreach (Vector2I resolutionWhite in GetResolutionWhiteList())
		{
			if (DoesResolutionFit(resolutionWhite, boundaryResolution))
			{
				NResolutionDropdownItem nResolutionDropdownItem = _dropdownItemScene.Instantiate<NResolutionDropdownItem>((GenEditState)0);
				((Node)(object)_dropdownItems).AddChildSafely((Node?)(object)nResolutionDropdownItem);
				((GodotObject)nResolutionDropdownItem).Connect(NDropdownItem.SignalName.Selected, Callable.From<NDropdownItem>((Action<NDropdownItem>)OnDropdownItemSelected), 0u);
				nResolutionDropdownItem.Init(resolutionWhite);
			}
		}
		((Node)_dropdownItems).GetParent<NDropdownContainer>().RefreshLayout();
	}

	private void OnWindowChange(bool isAutoAspectRatio)
	{
		RefreshEnabled();
		RefreshCurrentlySelectedResolution();
	}

	private void RefreshEnabled()
	{
		if (SaveManager.Instance.SettingsSave.Fullscreen || PlatformUtil.GetSupportedWindowMode().ShouldForceFullscreen())
		{
			Disable();
		}
		else
		{
			Enable();
		}
	}

	protected override void OnEnable()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		((CanvasItem)_currentOptionLabel).Modulate = StsColors.gold;
		((CanvasItem)_arrow).Visible = true;
		RefreshCurrentlySelectedResolution();
	}

	protected override void OnDisable()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		_currentOptionLabel.SetTextAutoSize("N/A");
		((CanvasItem)_currentOptionLabel).Modulate = StsColors.gray;
		((CanvasItem)_arrow).Visible = false;
	}

	private void OnDropdownItemSelected(NDropdownItem nDropdownItem)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		NResolutionDropdownItem nResolutionDropdownItem = (NResolutionDropdownItem)nDropdownItem;
		if (!(nResolutionDropdownItem.resolution == _currentResolution))
		{
			CloseDropdown();
			SaveManager.Instance.SettingsSave.WindowPosition = DisplayServer.WindowGetPosition(0) - DisplayServer.ScreenGetPosition(SaveManager.Instance.SettingsSave.TargetDisplay);
			SaveManager.Instance.SettingsSave.WindowSize = nResolutionDropdownItem.resolution;
			Log.Info($"Setting window size to {nResolutionDropdownItem.resolution} from dropdown");
			NGame.Instance.ApplyDisplaySettings();
		}
	}

	private static bool DoesResolutionFit(Vector2I resolution, Vector2I boundaryResolution)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		if (resolution.X <= boundaryResolution.X)
		{
			return resolution.Y <= boundaryResolution.Y;
		}
		return false;
	}

	private static List<Vector2I> GetResolutionWhiteList()
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_026b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0270: Unknown result type (might be due to invalid IL or missing references)
		//IL_028b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0290: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_030b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0310: Unknown result type (might be due to invalid IL or missing references)
		//IL_032b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0330: Unknown result type (might be due to invalid IL or missing references)
		//IL_034b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0350: Unknown result type (might be due to invalid IL or missing references)
		int num = 26;
		List<Vector2I> list = new List<Vector2I>(num);
		CollectionsMarshal.SetCount(list, num);
		Span<Vector2I> span = CollectionsMarshal.AsSpan(list);
		int num2 = 0;
		span[num2] = new Vector2I(1024, 768);
		num2++;
		span[num2] = new Vector2I(1152, 864);
		num2++;
		span[num2] = new Vector2I(1280, 720);
		num2++;
		span[num2] = new Vector2I(1280, 800);
		num2++;
		span[num2] = new Vector2I(1280, 960);
		num2++;
		span[num2] = new Vector2I(1366, 768);
		num2++;
		span[num2] = new Vector2I(1400, 1050);
		num2++;
		span[num2] = new Vector2I(1440, 900);
		num2++;
		span[num2] = new Vector2I(1440, 1080);
		num2++;
		span[num2] = new Vector2I(1600, 900);
		num2++;
		span[num2] = new Vector2I(1600, 1200);
		num2++;
		span[num2] = new Vector2I(1680, 1050);
		num2++;
		span[num2] = new Vector2I(1856, 1392);
		num2++;
		span[num2] = new Vector2I(1920, 1080);
		num2++;
		span[num2] = new Vector2I(1920, 1200);
		num2++;
		span[num2] = new Vector2I(1920, 1440);
		num2++;
		span[num2] = new Vector2I(2048, 1536);
		num2++;
		span[num2] = new Vector2I(2560, 1080);
		num2++;
		span[num2] = new Vector2I(2560, 1440);
		num2++;
		span[num2] = new Vector2I(2560, 1600);
		num2++;
		span[num2] = new Vector2I(3200, 1800);
		num2++;
		span[num2] = new Vector2I(3440, 1440);
		num2++;
		span[num2] = new Vector2I(3840, 1600);
		num2++;
		span[num2] = new Vector2I(3840, 2160);
		num2++;
		span[num2] = new Vector2I(3840, 2400);
		num2++;
		span[num2] = new Vector2I(7680, 4320);
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Expected O, but got Unknown
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_0270: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(10);
		list.Add(new MethodInfo(MethodName._EnterTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.RefreshCurrentlySelectedResolution, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.PopulateDropdownItems, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnWindowChange, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)1, StringName.op_Implicit("isAutoAspectRatio"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.RefreshEnabled, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnEnable, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnDisable, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnDropdownItemSelected, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("nDropdownItem"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.DoesResolutionFit, new PropertyInfo((Type)1, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)33, new List<PropertyInfo>
		{
			new PropertyInfo((Type)6, StringName.op_Implicit("resolution"), (PropertyHint)0, "", (PropertyUsageFlags)6, false),
			new PropertyInfo((Type)6, StringName.op_Implicit("boundaryResolution"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._EnterTree && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._EnterTree();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.RefreshCurrentlySelectedResolution && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			RefreshCurrentlySelectedResolution();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.PopulateDropdownItems && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			PopulateDropdownItems();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnWindowChange && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OnWindowChange(VariantUtils.ConvertTo<bool>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.RefreshEnabled && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			RefreshEnabled();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnEnable && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnEnable();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnDisable && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnDisable();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnDropdownItemSelected && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OnDropdownItemSelected(VariantUtils.ConvertTo<NDropdownItem>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.DoesResolutionFit && ((NativeVariantPtrArgs)(ref args)).Count == 2)
		{
			bool flag = DoesResolutionFit(VariantUtils.ConvertTo<Vector2I>(ref ((NativeVariantPtrArgs)(ref args))[0]), VariantUtils.ConvertTo<Vector2I>(ref ((NativeVariantPtrArgs)(ref args))[1]));
			ret = VariantUtils.CreateFrom<bool>(ref flag);
			return true;
		}
		return base.InvokeGodotClassMethod(in method, args, out ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static bool InvokeGodotClassStaticMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.DoesResolutionFit && ((NativeVariantPtrArgs)(ref args)).Count == 2)
		{
			bool flag = DoesResolutionFit(VariantUtils.ConvertTo<Vector2I>(ref ((NativeVariantPtrArgs)(ref args))[0]), VariantUtils.ConvertTo<Vector2I>(ref ((NativeVariantPtrArgs)(ref args))[1]));
			ret = VariantUtils.CreateFrom<bool>(ref flag);
			return true;
		}
		ret = default(godot_variant);
		return false;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if ((ref method) == MethodName._EnterTree)
		{
			return true;
		}
		if ((ref method) == MethodName._Ready)
		{
			return true;
		}
		if ((ref method) == MethodName.RefreshCurrentlySelectedResolution)
		{
			return true;
		}
		if ((ref method) == MethodName.PopulateDropdownItems)
		{
			return true;
		}
		if ((ref method) == MethodName.OnWindowChange)
		{
			return true;
		}
		if ((ref method) == MethodName.RefreshEnabled)
		{
			return true;
		}
		if ((ref method) == MethodName.OnEnable)
		{
			return true;
		}
		if ((ref method) == MethodName.OnDisable)
		{
			return true;
		}
		if ((ref method) == MethodName.OnDropdownItemSelected)
		{
			return true;
		}
		if ((ref method) == MethodName.DoesResolutionFit)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._dropdownItemScene)
		{
			_dropdownItemScene = VariantUtils.ConvertTo<PackedScene>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._arrow)
		{
			_arrow = VariantUtils.ConvertTo<Control>(ref value);
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
		if ((ref name) == PropertyName._dropdownItemScene)
		{
			value = VariantUtils.CreateFrom<PackedScene>(ref _dropdownItemScene);
			return true;
		}
		if ((ref name) == PropertyName._arrow)
		{
			value = VariantUtils.CreateFrom<Control>(ref _arrow);
			return true;
		}
		return base.GetGodotClassPropertyValue(in name, out value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._dropdownItemScene, (PropertyHint)17, "PackedScene", (PropertyUsageFlags)4102, true));
		list.Add(new PropertyInfo((Type)24, PropertyName._arrow, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName._dropdownItemScene, Variant.From<PackedScene>(ref _dropdownItemScene));
		info.AddProperty(PropertyName._arrow, Variant.From<Control>(ref _arrow));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._dropdownItemScene, ref val))
		{
			_dropdownItemScene = ((Variant)(ref val)).As<PackedScene>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._arrow, ref val2))
		{
			_arrow = ((Variant)(ref val2)).As<Control>();
		}
	}
}
