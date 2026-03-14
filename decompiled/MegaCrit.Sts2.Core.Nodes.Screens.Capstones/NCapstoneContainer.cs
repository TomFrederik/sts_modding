using System;
using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.HoverTips;
using MegaCrit.Sts2.Core.Nodes.Screens.Overlays;
using MegaCrit.Sts2.Core.Nodes.Screens.ScreenContext;
using MegaCrit.Sts2.Core.Runs;

namespace MegaCrit.Sts2.Core.Nodes.Screens.Capstones;

[ScriptPath("res://src/Core/Nodes/Screens/Capstones/NCapstoneContainer.cs")]
public class NCapstoneContainer : Control
{
	[Signal]
	public delegate void ChangedEventHandler();

	[Signal]
	public delegate void CapstoneClosedEventHandler();

	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName _EnterTree = StringName.op_Implicit("_EnterTree");

		public static readonly StringName _ExitTree = StringName.op_Implicit("_ExitTree");

		public static readonly StringName Close = StringName.op_Implicit("Close");

		public static readonly StringName CloseInternal = StringName.op_Implicit("CloseInternal");

		public static readonly StringName DisableBackstopInstantly = StringName.op_Implicit("DisableBackstopInstantly");

		public static readonly StringName EnableBackstopInstantly = StringName.op_Implicit("EnableBackstopInstantly");

		public static readonly StringName CleanUp = StringName.op_Implicit("CleanUp");

		public static readonly StringName OnActiveScreenChanged = StringName.op_Implicit("OnActiveScreenChanged");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName InUse = StringName.op_Implicit("InUse");

		public static readonly StringName _backstop = StringName.op_Implicit("_backstop");

		public static readonly StringName _backstopFade = StringName.op_Implicit("_backstopFade");
	}

	public class SignalName : SignalName
	{
		public static readonly StringName Changed = StringName.op_Implicit("Changed");

		public static readonly StringName CapstoneClosed = StringName.op_Implicit("CapstoneClosed");
	}

	private Control _backstop;

	private Tween? _backstopFade;

	private ChangedEventHandler backing_Changed;

	private CapstoneClosedEventHandler backing_CapstoneClosed;

	public ICapstoneScreen? CurrentCapstoneScreen { get; private set; }

	public bool InUse => CurrentCapstoneScreen != null;

	public static NCapstoneContainer? Instance => NRun.Instance?.GlobalUi.CapstoneContainer;

	public event ChangedEventHandler Changed
	{
		add
		{
			backing_Changed = (ChangedEventHandler)Delegate.Combine(backing_Changed, value);
		}
		remove
		{
			backing_Changed = (ChangedEventHandler)Delegate.Remove(backing_Changed, value);
		}
	}

	public event CapstoneClosedEventHandler CapstoneClosed
	{
		add
		{
			backing_CapstoneClosed = (CapstoneClosedEventHandler)Delegate.Combine(backing_CapstoneClosed, value);
		}
		remove
		{
			backing_CapstoneClosed = (CapstoneClosedEventHandler)Delegate.Remove(backing_CapstoneClosed, value);
		}
	}

	public override void _Ready()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		_backstop = ((Node)this).GetNode<Control>(NodePath.op_Implicit("CapstoneBackstop"));
		((CanvasItem)_backstop).Modulate = Colors.Transparent;
	}

	public override void _EnterTree()
	{
		ActiveScreenContext.Instance.Updated += OnActiveScreenChanged;
	}

	public override void _ExitTree()
	{
		ActiveScreenContext.Instance.Updated -= OnActiveScreenChanged;
	}

	public void Open(ICapstoneScreen screen)
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Expected O, but got Unknown
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Expected O, but got Unknown
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		NHoverTipSet.Clear();
		bool flag = CurrentCapstoneScreen != null;
		if (flag)
		{
			CloseInternal();
		}
		Tween? backstopFade = _backstopFade;
		if (backstopFade != null)
		{
			backstopFade.Kill();
		}
		NOverlayStack.Instance.HideOverlays();
		if (!screen.UseSharedBackstop)
		{
			((CanvasItem)_backstop).Modulate = Colors.Transparent;
		}
		else if (flag || NOverlayStack.Instance.ScreenCount > 0)
		{
			((CanvasItem)_backstop).Modulate = Colors.White;
		}
		else
		{
			_backstopFade = ((Node)this).CreateTween();
			_backstopFade.TweenProperty((GodotObject)(object)_backstop, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(1f), 0.5).SetEase((EaseType)1).SetTrans((TransitionType)7);
		}
		CurrentCapstoneScreen = screen;
		if (!((Node)this).GetChildren(false).Contains((Node)screen))
		{
			((Node)(object)this).AddChildSafely((Node)screen);
		}
		((Node)screen).ProcessMode = (ProcessModeEnum)0;
		screen.AfterCapstoneOpened();
		if (RunManager.Instance.IsSinglePlayerOrFakeMultiplayer)
		{
			CombatManager.Instance.Pause();
		}
		ActiveScreenContext.Instance.Update();
		((GodotObject)this).EmitSignal(SignalName.Changed, Array.Empty<Variant>());
	}

	public void Close()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		if (CurrentCapstoneScreen != null)
		{
			CloseInternal();
			ActiveScreenContext.Instance.Update();
			((GodotObject)this).EmitSignal(SignalName.CapstoneClosed, Array.Empty<Variant>());
			((GodotObject)this).EmitSignal(SignalName.Changed, Array.Empty<Variant>());
		}
	}

	private void CloseInternal()
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		if (RunManager.Instance.IsSinglePlayerOrFakeMultiplayer)
		{
			CombatManager.Instance.Unpause();
		}
		NOverlayStack.Instance.ShowOverlays();
		if (NOverlayStack.Instance.ScreenCount > 0)
		{
			((CanvasItem)_backstop).Modulate = Colors.Transparent;
		}
		else
		{
			Tween? backstopFade = _backstopFade;
			if (backstopFade != null)
			{
				backstopFade.Kill();
			}
			_backstopFade = ((Node)this).CreateTween();
			_backstopFade.TweenProperty((GodotObject)(object)_backstop, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0f), 0.5).SetEase((EaseType)1).SetTrans((TransitionType)7);
		}
		ICapstoneScreen currentCapstoneScreen = CurrentCapstoneScreen;
		CurrentCapstoneScreen = null;
		Node val = (Node)((currentCapstoneScreen is Node) ? currentCapstoneScreen : null);
		if (val != null)
		{
			val.ProcessMode = (ProcessModeEnum)4;
		}
		currentCapstoneScreen?.AfterCapstoneClosed();
		NHoverTipSet.Clear();
	}

	public void DisableBackstopInstantly()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		Tween? backstopFade = _backstopFade;
		if (backstopFade != null)
		{
			backstopFade.Kill();
		}
		((CanvasItem)_backstop).Modulate = Colors.Transparent;
	}

	public void EnableBackstopInstantly()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		Tween? backstopFade = _backstopFade;
		if (backstopFade != null)
		{
			backstopFade.Kill();
		}
		((CanvasItem)_backstop).Modulate = Colors.White;
	}

	public void CleanUp()
	{
		if (RunManager.Instance.IsSinglePlayerOrFakeMultiplayer)
		{
			CombatManager.Instance.Unpause();
		}
	}

	private void OnActiveScreenChanged()
	{
		if (InUse)
		{
			if (ActiveScreenContext.Instance.IsCurrent(CurrentCapstoneScreen))
			{
				((Control)this).FocusBehaviorRecursive = (FocusBehaviorRecursiveEnum)2;
			}
			else
			{
				((Control)this).FocusBehaviorRecursive = (FocusBehaviorRecursiveEnum)1;
			}
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
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
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(9);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._EnterTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._ExitTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.Close, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.CloseInternal, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.DisableBackstopInstantly, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.EnableBackstopInstantly, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.CleanUp, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnActiveScreenChanged, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
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
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._EnterTree && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._EnterTree();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._ExitTree && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._ExitTree();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.Close && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			Close();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.CloseInternal && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			CloseInternal();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.DisableBackstopInstantly && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			DisableBackstopInstantly();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.EnableBackstopInstantly && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			EnableBackstopInstantly();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.CleanUp && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			CleanUp();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnActiveScreenChanged && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnActiveScreenChanged();
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
		if ((ref method) == MethodName._EnterTree)
		{
			return true;
		}
		if ((ref method) == MethodName._ExitTree)
		{
			return true;
		}
		if ((ref method) == MethodName.Close)
		{
			return true;
		}
		if ((ref method) == MethodName.CloseInternal)
		{
			return true;
		}
		if ((ref method) == MethodName.DisableBackstopInstantly)
		{
			return true;
		}
		if ((ref method) == MethodName.EnableBackstopInstantly)
		{
			return true;
		}
		if ((ref method) == MethodName.CleanUp)
		{
			return true;
		}
		if ((ref method) == MethodName.OnActiveScreenChanged)
		{
			return true;
		}
		return ((Control)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._backstop)
		{
			_backstop = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._backstopFade)
		{
			_backstopFade = VariantUtils.ConvertTo<Tween>(ref value);
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
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName.InUse)
		{
			bool inUse = InUse;
			value = VariantUtils.CreateFrom<bool>(ref inUse);
			return true;
		}
		if ((ref name) == PropertyName._backstop)
		{
			value = VariantUtils.CreateFrom<Control>(ref _backstop);
			return true;
		}
		if ((ref name) == PropertyName._backstopFade)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _backstopFade);
			return true;
		}
		return ((GodotObject)this).GetGodotClassPropertyValue(ref name, ref value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)1, PropertyName.InUse, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._backstop, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._backstopFade, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		info.AddProperty(PropertyName._backstop, Variant.From<Control>(ref _backstop));
		info.AddProperty(PropertyName._backstopFade, Variant.From<Tween>(ref _backstopFade));
		info.AddSignalEventDelegate(SignalName.Changed, (Delegate)backing_Changed);
		info.AddSignalEventDelegate(SignalName.CapstoneClosed, (Delegate)backing_CapstoneClosed);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._backstop, ref val))
		{
			_backstop = ((Variant)(ref val)).As<Control>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._backstopFade, ref val2))
		{
			_backstopFade = ((Variant)(ref val2)).As<Tween>();
		}
		ChangedEventHandler changedEventHandler = default(ChangedEventHandler);
		if (info.TryGetSignalEventDelegate<ChangedEventHandler>(SignalName.Changed, ref changedEventHandler))
		{
			backing_Changed = changedEventHandler;
		}
		CapstoneClosedEventHandler capstoneClosedEventHandler = default(CapstoneClosedEventHandler);
		if (info.TryGetSignalEventDelegate<CapstoneClosedEventHandler>(SignalName.CapstoneClosed, ref capstoneClosedEventHandler))
		{
			backing_CapstoneClosed = capstoneClosedEventHandler;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotSignalList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(2);
		list.Add(new MethodInfo(SignalName.Changed, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(SignalName.CapstoneClosed, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	protected void EmitSignalChanged()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).EmitSignal(SignalName.Changed, Array.Empty<Variant>());
	}

	protected void EmitSignalCapstoneClosed()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).EmitSignal(SignalName.CapstoneClosed, Array.Empty<Variant>());
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RaiseGodotClassSignalCallbacks(in godot_string_name signal, NativeVariantPtrArgs args)
	{
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		if ((ref signal) == SignalName.Changed && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			backing_Changed?.Invoke();
		}
		else if ((ref signal) == SignalName.CapstoneClosed && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			backing_CapstoneClosed?.Invoke();
		}
		else
		{
			((GodotObject)this).RaiseGodotClassSignalCallbacks(ref signal, args);
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassSignal(in godot_string_name signal)
	{
		if ((ref signal) == SignalName.Changed)
		{
			return true;
		}
		if ((ref signal) == SignalName.CapstoneClosed)
		{
			return true;
		}
		return ((Control)this).HasGodotClassSignal(ref signal);
	}
}
