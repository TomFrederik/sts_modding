using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.ControllerInput;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.Debug;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Screens.Settings;

namespace MegaCrit.Sts2.Core.Nodes.Screens.StatsScreen;

[ScriptPath("res://src/Core/Nodes/Screens/StatsScreen/NStatsTabManager.cs")]
public class NStatsTabManager : Control
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName ResetTabs = StringName.op_Implicit("ResetTabs");

		public static readonly StringName _Input = StringName.op_Implicit("_Input");

		public static readonly StringName SwitchToTab = StringName.op_Implicit("SwitchToTab");

		public static readonly StringName UpdateControllerButton = StringName.op_Implicit("UpdateControllerButton");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName _leftTriggerIcon = StringName.op_Implicit("_leftTriggerIcon");

		public static readonly StringName _rightTriggerIcon = StringName.op_Implicit("_rightTriggerIcon");

		public static readonly StringName _tabContainer = StringName.op_Implicit("_tabContainer");

		public static readonly StringName _currentTab = StringName.op_Implicit("_currentTab");
	}

	public class SignalName : SignalName
	{
	}

	private static readonly StringName _tabLeftHotkey = MegaInput.viewDeckAndTabLeft;

	private static readonly StringName _tabRightHotkey = MegaInput.viewExhaustPileAndTabRight;

	private Control _leftTriggerIcon;

	private Control _rightTriggerIcon;

	private Control _tabContainer;

	private List<NSettingsTab> _tabs;

	private NSettingsTab? _currentTab;

	public override void _Ready()
	{
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		_leftTriggerIcon = ((Node)this).GetNode<Control>(NodePath.op_Implicit("LeftTriggerIcon"));
		_rightTriggerIcon = ((Node)this).GetNode<Control>(NodePath.op_Implicit("RightTriggerIcon"));
		_tabContainer = ((Node)this).GetNode<Control>(NodePath.op_Implicit("TabContainer"));
		_tabs = ((IEnumerable)((Node)_tabContainer).GetChildren(false)).OfType<NSettingsTab>().ToList();
		((GodotObject)NControllerManager.Instance).Connect(NControllerManager.SignalName.MouseDetected, Callable.From((Action)UpdateControllerButton), 0u);
		((GodotObject)NControllerManager.Instance).Connect(NControllerManager.SignalName.ControllerDetected, Callable.From((Action)UpdateControllerButton), 0u);
		((GodotObject)NInputManager.Instance).Connect(NInputManager.SignalName.InputRebound, Callable.From((Action)UpdateControllerButton), 0u);
		foreach (NSettingsTab nSettingsTab in _tabs)
		{
			((GodotObject)nSettingsTab).Connect(NClickableControl.SignalName.Released, Callable.From<NClickableControl>((Action<NClickableControl>)delegate
			{
				SwitchToTab(nSettingsTab);
			}), 0u);
		}
		UpdateControllerButton();
	}

	public void ResetTabs()
	{
		SwitchToTab(((Node)_tabContainer).GetChild<NSettingsTab>(0, false));
	}

	public override void _Input(InputEvent inputEvent)
	{
		if (!((CanvasItem)this).IsVisibleInTree() || ((CanvasItem)NDevConsole.Instance).Visible)
		{
			return;
		}
		Control val = ((Node)this).GetViewport().GuiGetFocusOwner();
		if ((val is TextEdit || val is LineEdit) ? true : false)
		{
			return;
		}
		if (inputEvent.IsActionPressed(_tabLeftHotkey, false, false))
		{
			int num = _tabs.IndexOf(_currentTab) - 1;
			if (num >= 0)
			{
				_tabs[num].ForceTabPressed();
				SwitchToTab(_tabs[num]);
			}
		}
		if (inputEvent.IsActionPressed(_tabRightHotkey, false, false))
		{
			int num2 = Math.Min(_tabs.Count - 1, _tabs.IndexOf(_currentTab) + 1);
			if (num2 < _tabs.Count)
			{
				_tabs[num2].ForceTabPressed();
			}
		}
	}

	private void SwitchToTab(NSettingsTab tab)
	{
		_currentTab = tab;
		foreach (NSettingsTab tab2 in _tabs)
		{
			if (tab2 != _currentTab)
			{
				tab2.Deselect();
			}
			else
			{
				tab2.Select();
			}
		}
	}

	private void UpdateControllerButton()
	{
		((CanvasItem)_leftTriggerIcon).Visible = NControllerManager.Instance.IsUsingController;
		((CanvasItem)_rightTriggerIcon).Visible = NControllerManager.Instance.IsUsingController;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Expected O, but got Unknown
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Expected O, but got Unknown
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(5);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ResetTabs, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Input, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("inputEvent"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("InputEvent"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SwitchToTab, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("tab"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UpdateControllerButton, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.ResetTabs && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			ResetTabs();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._Input && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			((Node)this)._Input(VariantUtils.ConvertTo<InputEvent>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SwitchToTab && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			SwitchToTab(VariantUtils.ConvertTo<NSettingsTab>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.UpdateControllerButton && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			UpdateControllerButton();
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
		if ((ref method) == MethodName.ResetTabs)
		{
			return true;
		}
		if ((ref method) == MethodName._Input)
		{
			return true;
		}
		if ((ref method) == MethodName.SwitchToTab)
		{
			return true;
		}
		if ((ref method) == MethodName.UpdateControllerButton)
		{
			return true;
		}
		return ((Control)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._leftTriggerIcon)
		{
			_leftTriggerIcon = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._rightTriggerIcon)
		{
			_rightTriggerIcon = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._tabContainer)
		{
			_tabContainer = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._currentTab)
		{
			_currentTab = VariantUtils.ConvertTo<NSettingsTab>(ref value);
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
		if ((ref name) == PropertyName._leftTriggerIcon)
		{
			value = VariantUtils.CreateFrom<Control>(ref _leftTriggerIcon);
			return true;
		}
		if ((ref name) == PropertyName._rightTriggerIcon)
		{
			value = VariantUtils.CreateFrom<Control>(ref _rightTriggerIcon);
			return true;
		}
		if ((ref name) == PropertyName._tabContainer)
		{
			value = VariantUtils.CreateFrom<Control>(ref _tabContainer);
			return true;
		}
		if ((ref name) == PropertyName._currentTab)
		{
			value = VariantUtils.CreateFrom<NSettingsTab>(ref _currentTab);
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
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._leftTriggerIcon, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._rightTriggerIcon, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._tabContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._currentTab, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		info.AddProperty(PropertyName._leftTriggerIcon, Variant.From<Control>(ref _leftTriggerIcon));
		info.AddProperty(PropertyName._rightTriggerIcon, Variant.From<Control>(ref _rightTriggerIcon));
		info.AddProperty(PropertyName._tabContainer, Variant.From<Control>(ref _tabContainer));
		info.AddProperty(PropertyName._currentTab, Variant.From<NSettingsTab>(ref _currentTab));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._leftTriggerIcon, ref val))
		{
			_leftTriggerIcon = ((Variant)(ref val)).As<Control>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._rightTriggerIcon, ref val2))
		{
			_rightTriggerIcon = ((Variant)(ref val2)).As<Control>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._tabContainer, ref val3))
		{
			_tabContainer = ((Variant)(ref val3)).As<Control>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._currentTab, ref val4))
		{
			_currentTab = ((Variant)(ref val4)).As<NSettingsTab>();
		}
	}
}
