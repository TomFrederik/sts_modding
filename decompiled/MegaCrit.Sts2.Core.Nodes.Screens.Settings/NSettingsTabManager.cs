using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.ControllerInput;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Screens.ScreenContext;

namespace MegaCrit.Sts2.Core.Nodes.Screens.Settings;

[ScriptPath("res://src/Core/Nodes/Screens/Settings/NSettingsTabManager.cs")]
public class NSettingsTabManager : Control
{
	[Signal]
	public delegate void TabChangedEventHandler();

	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName ResetTabs = StringName.op_Implicit("ResetTabs");

		public static readonly StringName Enable = StringName.op_Implicit("Enable");

		public static readonly StringName Disable = StringName.op_Implicit("Disable");

		public static readonly StringName TabLeft = StringName.op_Implicit("TabLeft");

		public static readonly StringName TabRight = StringName.op_Implicit("TabRight");

		public static readonly StringName SwitchTabTo = StringName.op_Implicit("SwitchTabTo");

		public static readonly StringName UpdateControllerButton = StringName.op_Implicit("UpdateControllerButton");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName CurrentlyDisplayedPanel = StringName.op_Implicit("CurrentlyDisplayedPanel");

		public static readonly StringName DefaultFocusedControl = StringName.op_Implicit("DefaultFocusedControl");

		public static readonly StringName _currentTab = StringName.op_Implicit("_currentTab");

		public static readonly StringName _scrollContainer = StringName.op_Implicit("_scrollContainer");

		public static readonly StringName _leftTriggerIcon = StringName.op_Implicit("_leftTriggerIcon");

		public static readonly StringName _rightTriggerIcon = StringName.op_Implicit("_rightTriggerIcon");

		public static readonly StringName _scrollbarTween = StringName.op_Implicit("_scrollbarTween");
	}

	public class SignalName : SignalName
	{
		public static readonly StringName TabChanged = StringName.op_Implicit("TabChanged");
	}

	private const float _scrollPaddingTop = 20f;

	private const float _scrollPaddingBottom = 30f;

	private static readonly StringName _tabLeftHotkey = MegaInput.viewDeckAndTabLeft;

	private static readonly StringName _tabRightHotkey = MegaInput.viewExhaustPileAndTabRight;

	private NSettingsTab? _currentTab;

	private NScrollableContainer _scrollContainer;

	private readonly Dictionary<NSettingsTab, NSettingsPanel> _tabs = new Dictionary<NSettingsTab, NSettingsPanel>();

	private TextureRect _leftTriggerIcon;

	private TextureRect _rightTriggerIcon;

	private Tween? _scrollbarTween;

	private TabChangedEventHandler backing_TabChanged;

	private NSettingsPanel CurrentlyDisplayedPanel => _tabs[_currentTab];

	public Control? DefaultFocusedControl
	{
		get
		{
			if (_currentTab == null)
			{
				return null;
			}
			return _tabs[_currentTab].DefaultFocusedControl;
		}
	}

	public event TabChangedEventHandler TabChanged
	{
		add
		{
			backing_TabChanged = (TabChangedEventHandler)Delegate.Combine(backing_TabChanged, value);
		}
		remove
		{
			backing_TabChanged = (TabChangedEventHandler)Delegate.Remove(backing_TabChanged, value);
		}
	}

	public override void _Ready()
	{
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		_leftTriggerIcon = ((Node)this).GetNode<TextureRect>(NodePath.op_Implicit("LeftTriggerIcon"));
		_rightTriggerIcon = ((Node)this).GetNode<TextureRect>(NodePath.op_Implicit("RightTriggerIcon"));
		_scrollContainer = ((Node)this).GetNode<NScrollableContainer>(NodePath.op_Implicit("%ScrollContainer"));
		_scrollContainer.DisableScrollingIfContentFits();
		NSettingsTab node = ((Node)this).GetNode<NSettingsTab>(NodePath.op_Implicit("General"));
		node.SetLabel(new LocString("settings_ui", "TAB_GENERAL").GetFormattedText());
		_tabs.Add(node, ((Node)this).GetNode<NSettingsPanel>(NodePath.op_Implicit("%GeneralSettings")));
		node = ((Node)this).GetNode<NSettingsTab>(NodePath.op_Implicit("Graphics"));
		node.SetLabel(new LocString("settings_ui", "TAB_GRAPHICS").GetFormattedText());
		_tabs.Add(node, ((Node)this).GetNode<NSettingsPanel>(NodePath.op_Implicit("%GraphicsSettings")));
		node = ((Node)this).GetNode<NSettingsTab>(NodePath.op_Implicit("Sound"));
		node.SetLabel(new LocString("settings_ui", "TAB_SOUND").GetFormattedText());
		_tabs.Add(node, ((Node)this).GetNode<NSettingsPanel>(NodePath.op_Implicit("%SoundSettings")));
		node = ((Node)this).GetNode<NSettingsTab>(NodePath.op_Implicit("Input"));
		node.SetLabel(new LocString("settings_ui", "TAB_INPUT").GetFormattedText());
		_tabs.Add(node, ((Node)this).GetNode<NSettingsPanel>(NodePath.op_Implicit("%InputSettings")));
		foreach (NSettingsTab tab in _tabs.Keys)
		{
			((GodotObject)tab).Connect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)delegate
			{
				SwitchTabTo(tab);
			}), 0u);
		}
		((GodotObject)NControllerManager.Instance).Connect(NControllerManager.SignalName.MouseDetected, Callable.From((Action)UpdateControllerButton), 0u);
		((GodotObject)NControllerManager.Instance).Connect(NControllerManager.SignalName.ControllerDetected, Callable.From((Action)UpdateControllerButton), 0u);
		((GodotObject)NInputManager.Instance).Connect(NInputManager.SignalName.InputRebound, Callable.From((Action)UpdateControllerButton), 0u);
		UpdateControllerButton();
	}

	public void ResetTabs()
	{
		_tabs.First().Key.Select();
		SwitchTabTo(_tabs.First().Key);
	}

	public void Enable()
	{
		NHotkeyManager.Instance.PushHotkeyPressedBinding(StringName.op_Implicit(_tabLeftHotkey), TabLeft);
		NHotkeyManager.Instance.PushHotkeyPressedBinding(StringName.op_Implicit(_tabRightHotkey), TabRight);
	}

	public void Disable()
	{
		NHotkeyManager.Instance.RemoveHotkeyPressedBinding(StringName.op_Implicit(_tabLeftHotkey), TabLeft);
		NHotkeyManager.Instance.RemoveHotkeyPressedBinding(StringName.op_Implicit(_tabRightHotkey), TabRight);
	}

	private void TabLeft()
	{
		List<NSettingsTab> list = _tabs.Keys.ToList();
		int num = list.IndexOf(_currentTab) - 1;
		if (num >= 0)
		{
			SwitchTabTo(list[num]);
		}
	}

	private void TabRight()
	{
		List<NSettingsTab> list = _tabs.Keys.ToList();
		int num = Math.Min(list.Count - 1, list.IndexOf(_currentTab) + 1);
		if (num < list.Count)
		{
			SwitchTabTo(list[num]);
		}
	}

	private void SwitchTabTo(NSettingsTab selectedTab)
	{
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		if (selectedTab != _currentTab)
		{
			foreach (NSettingsTab key in _tabs.Keys)
			{
				key.Deselect();
				((CanvasItem)_tabs[key]).Visible = false;
			}
			selectedTab.Select();
			((CanvasItem)_tabs[selectedTab]).Visible = true;
			_currentTab = selectedTab;
			_scrollContainer.SetContent((Control?)(object)CurrentlyDisplayedPanel, 20f, 30f);
			_scrollContainer.InstantlyScrollToTop();
			Tween? scrollbarTween = _scrollbarTween;
			if (scrollbarTween != null)
			{
				scrollbarTween.Kill();
			}
			_scrollbarTween = ((Node)this).CreateTween().SetParallel(true);
			_scrollbarTween.TweenProperty((GodotObject)(object)_scrollContainer.Scrollbar, NodePath.op_Implicit("modulate"), Variant.op_Implicit(Colors.White), 0.5).From(Variant.op_Implicit(StsColors.transparentBlack)).SetEase((EaseType)1)
				.SetTrans((TransitionType)7);
		}
		ActiveScreenContext.Instance.Update();
	}

	private void UpdateControllerButton()
	{
		((CanvasItem)_leftTriggerIcon).Visible = NControllerManager.Instance.IsUsingController;
		((CanvasItem)_rightTriggerIcon).Visible = NControllerManager.Instance.IsUsingController;
		_leftTriggerIcon.Texture = NInputManager.Instance.GetHotkeyIcon(StringName.op_Implicit(MegaInput.viewDeckAndTabLeft));
		_rightTriggerIcon.Texture = NInputManager.Instance.GetHotkeyIcon(StringName.op_Implicit(MegaInput.viewExhaustPileAndTabRight));
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
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Expected O, but got Unknown
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(8);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ResetTabs, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.Enable, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.Disable, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.TabLeft, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.TabRight, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SwitchTabTo, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("selectedTab"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UpdateControllerButton, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
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
		if ((ref method) == MethodName.Enable && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			Enable();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.Disable && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			Disable();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.TabLeft && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			TabLeft();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.TabRight && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			TabRight();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SwitchTabTo && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			SwitchTabTo(VariantUtils.ConvertTo<NSettingsTab>(ref ((NativeVariantPtrArgs)(ref args))[0]));
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
		if ((ref method) == MethodName.Enable)
		{
			return true;
		}
		if ((ref method) == MethodName.Disable)
		{
			return true;
		}
		if ((ref method) == MethodName.TabLeft)
		{
			return true;
		}
		if ((ref method) == MethodName.TabRight)
		{
			return true;
		}
		if ((ref method) == MethodName.SwitchTabTo)
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
		if ((ref name) == PropertyName._currentTab)
		{
			_currentTab = VariantUtils.ConvertTo<NSettingsTab>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._scrollContainer)
		{
			_scrollContainer = VariantUtils.ConvertTo<NScrollableContainer>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._leftTriggerIcon)
		{
			_leftTriggerIcon = VariantUtils.ConvertTo<TextureRect>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._rightTriggerIcon)
		{
			_rightTriggerIcon = VariantUtils.ConvertTo<TextureRect>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._scrollbarTween)
		{
			_scrollbarTween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		return ((GodotObject)this).SetGodotClassPropertyValue(ref name, ref value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName.CurrentlyDisplayedPanel)
		{
			NSettingsPanel currentlyDisplayedPanel = CurrentlyDisplayedPanel;
			value = VariantUtils.CreateFrom<NSettingsPanel>(ref currentlyDisplayedPanel);
			return true;
		}
		if ((ref name) == PropertyName.DefaultFocusedControl)
		{
			Control defaultFocusedControl = DefaultFocusedControl;
			value = VariantUtils.CreateFrom<Control>(ref defaultFocusedControl);
			return true;
		}
		if ((ref name) == PropertyName._currentTab)
		{
			value = VariantUtils.CreateFrom<NSettingsTab>(ref _currentTab);
			return true;
		}
		if ((ref name) == PropertyName._scrollContainer)
		{
			value = VariantUtils.CreateFrom<NScrollableContainer>(ref _scrollContainer);
			return true;
		}
		if ((ref name) == PropertyName._leftTriggerIcon)
		{
			value = VariantUtils.CreateFrom<TextureRect>(ref _leftTriggerIcon);
			return true;
		}
		if ((ref name) == PropertyName._rightTriggerIcon)
		{
			value = VariantUtils.CreateFrom<TextureRect>(ref _rightTriggerIcon);
			return true;
		}
		if ((ref name) == PropertyName._scrollbarTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _scrollbarTween);
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
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._currentTab, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._scrollContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.CurrentlyDisplayedPanel, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._leftTriggerIcon, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._rightTriggerIcon, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._scrollbarTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		info.AddProperty(PropertyName._currentTab, Variant.From<NSettingsTab>(ref _currentTab));
		info.AddProperty(PropertyName._scrollContainer, Variant.From<NScrollableContainer>(ref _scrollContainer));
		info.AddProperty(PropertyName._leftTriggerIcon, Variant.From<TextureRect>(ref _leftTriggerIcon));
		info.AddProperty(PropertyName._rightTriggerIcon, Variant.From<TextureRect>(ref _rightTriggerIcon));
		info.AddProperty(PropertyName._scrollbarTween, Variant.From<Tween>(ref _scrollbarTween));
		info.AddSignalEventDelegate(SignalName.TabChanged, (Delegate)backing_TabChanged);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._currentTab, ref val))
		{
			_currentTab = ((Variant)(ref val)).As<NSettingsTab>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._scrollContainer, ref val2))
		{
			_scrollContainer = ((Variant)(ref val2)).As<NScrollableContainer>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._leftTriggerIcon, ref val3))
		{
			_leftTriggerIcon = ((Variant)(ref val3)).As<TextureRect>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._rightTriggerIcon, ref val4))
		{
			_rightTriggerIcon = ((Variant)(ref val4)).As<TextureRect>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._scrollbarTween, ref val5))
		{
			_scrollbarTween = ((Variant)(ref val5)).As<Tween>();
		}
		TabChangedEventHandler tabChangedEventHandler = default(TabChangedEventHandler);
		if (info.TryGetSignalEventDelegate<TabChangedEventHandler>(SignalName.TabChanged, ref tabChangedEventHandler))
		{
			backing_TabChanged = tabChangedEventHandler;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotSignalList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(1);
		list.Add(new MethodInfo(SignalName.TabChanged, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	protected void EmitSignalTabChanged()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).EmitSignal(SignalName.TabChanged, Array.Empty<Variant>());
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RaiseGodotClassSignalCallbacks(in godot_string_name signal, NativeVariantPtrArgs args)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		if ((ref signal) == SignalName.TabChanged && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			backing_TabChanged?.Invoke();
		}
		else
		{
			((GodotObject)this).RaiseGodotClassSignalCallbacks(ref signal, args);
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassSignal(in godot_string_name signal)
	{
		if ((ref signal) == SignalName.TabChanged)
		{
			return true;
		}
		return ((Control)this).HasGodotClassSignal(ref signal);
	}
}
