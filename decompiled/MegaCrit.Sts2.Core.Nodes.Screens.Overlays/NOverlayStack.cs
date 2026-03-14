using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.Screens.Map;
using MegaCrit.Sts2.Core.Nodes.Screens.ScreenContext;

namespace MegaCrit.Sts2.Core.Nodes.Screens.Overlays;

[ScriptPath("res://src/Core/Nodes/Screens/Overlays/NOverlayStack.cs")]
public class NOverlayStack : Control
{
	[Signal]
	public delegate void ChangedEventHandler();

	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName _EnterTree = StringName.op_Implicit("_EnterTree");

		public static readonly StringName _ExitTree = StringName.op_Implicit("_ExitTree");

		public static readonly StringName Clear = StringName.op_Implicit("Clear");

		public static readonly StringName HideOverlays = StringName.op_Implicit("HideOverlays");

		public static readonly StringName ShowOverlays = StringName.op_Implicit("ShowOverlays");

		public static readonly StringName ShowBackstop = StringName.op_Implicit("ShowBackstop");

		public static readonly StringName HideBackstop = StringName.op_Implicit("HideBackstop");

		public static readonly StringName OnActiveScreenChanged = StringName.op_Implicit("OnActiveScreenChanged");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName ScreenCount = StringName.op_Implicit("ScreenCount");

		public static readonly StringName _backstop = StringName.op_Implicit("_backstop");

		public static readonly StringName _backstopFade = StringName.op_Implicit("_backstopFade");
	}

	public class SignalName : SignalName
	{
		public static readonly StringName Changed = StringName.op_Implicit("Changed");
	}

	private readonly List<IOverlayScreen> _overlays = new List<IOverlayScreen>();

	private Control _backstop;

	private Tween? _backstopFade;

	private ChangedEventHandler backing_Changed;

	public static NOverlayStack? Instance => NRun.Instance?.GlobalUi.Overlays;

	public int ScreenCount => _overlays.Count;

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

	public override void _Ready()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		_backstop = ((Node)this).GetNode<Control>(NodePath.op_Implicit("OverlayBackstop"));
		((CanvasItem)_backstop).Modulate = Colors.Transparent;
		_backstop.MouseFilter = (MouseFilterEnum)2;
		Callable val = Callable.From<Error>((Func<Error>)(() => ((GodotObject)NMapScreen.Instance).Connect(NMapScreen.SignalName.Opened, Callable.From((Action)HideOverlays), 0u)));
		((Callable)(ref val)).CallDeferred(Array.Empty<Variant>());
		val = Callable.From<Error>((Func<Error>)(() => ((GodotObject)NMapScreen.Instance).Connect(NMapScreen.SignalName.Closed, Callable.From((Action)ShowOverlays), 0u)));
		((Callable)(ref val)).CallDeferred(Array.Empty<Variant>());
	}

	public override void _EnterTree()
	{
		ActiveScreenContext.Instance.Updated += OnActiveScreenChanged;
	}

	public override void _ExitTree()
	{
		ActiveScreenContext.Instance.Updated -= OnActiveScreenChanged;
		Clear();
	}

	public void Push(IOverlayScreen screen)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Expected O, but got Unknown
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		Peek()?.AfterOverlayHidden();
		((Node)(object)this).AddChildSafely((Node)screen);
		_overlays.Add(screen);
		screen.AfterOverlayOpened();
		screen.AfterOverlayShown();
		_backstop.MouseFilter = (MouseFilterEnum)0;
		Tween? backstopFade = _backstopFade;
		if (backstopFade != null)
		{
			backstopFade.Kill();
		}
		((Node)this).MoveChild((Node)(object)_backstop, _overlays.IndexOf(screen));
		if (!screen.UseSharedBackstop)
		{
			((CanvasItem)_backstop).Modulate = Colors.Transparent;
		}
		else if (ScreenCount == 1)
		{
			ShowBackstop();
		}
		else
		{
			((CanvasItem)_backstop).Modulate = Colors.White;
		}
		ActiveScreenContext.Instance.Update();
		((GodotObject)this).EmitSignal(SignalName.Changed, Array.Empty<Variant>());
	}

	public void Remove(IOverlayScreen screen)
	{
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		bool flag = screen == Peek();
		if (flag)
		{
			HideBackstop();
			screen.AfterOverlayHidden();
		}
		screen.AfterOverlayClosed();
		_overlays.Remove(screen);
		if (flag)
		{
			IOverlayScreen overlayScreen = Peek();
			if (overlayScreen != null)
			{
				_backstop.MouseFilter = (MouseFilterEnum)0;
				((Node)this).MoveChild((Node)(object)_backstop, _overlays.IndexOf(overlayScreen));
				if (overlayScreen.UseSharedBackstop)
				{
					((CanvasItem)_backstop).Modulate = Colors.White;
				}
				else
				{
					HideBackstop();
				}
				overlayScreen.AfterOverlayShown();
			}
			else
			{
				HideBackstop();
			}
		}
		ActiveScreenContext.Instance.Update();
		((GodotObject)this).EmitSignal(SignalName.Changed, Array.Empty<Variant>());
	}

	public void Clear()
	{
		for (IOverlayScreen overlayScreen = Peek(); overlayScreen != null; overlayScreen = Peek())
		{
			Remove(overlayScreen);
		}
	}

	public void HideOverlays()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		((CanvasItem)_backstop).Modulate = Colors.Transparent;
		Peek()?.AfterOverlayHidden();
	}

	public void ShowOverlays()
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		IOverlayScreen overlayScreen = Peek();
		if (overlayScreen != null && !NMapScreen.Instance.IsOpen)
		{
			((CanvasItem)_backstop).Modulate = (overlayScreen.UseSharedBackstop ? Colors.White : Colors.Transparent);
			overlayScreen.AfterOverlayShown();
		}
	}

	public void ShowBackstop()
	{
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		IOverlayScreen? overlayScreen = Peek();
		if (overlayScreen == null || overlayScreen.UseSharedBackstop)
		{
			_backstop.MouseFilter = (MouseFilterEnum)0;
			Tween? backstopFade = _backstopFade;
			if (backstopFade != null)
			{
				backstopFade.Kill();
			}
			_backstopFade = ((Node)this).CreateTween();
			_backstopFade.TweenProperty((GodotObject)(object)_backstop, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(1f), 0.5).SetEase((EaseType)1).SetTrans((TransitionType)7);
		}
	}

	public void HideBackstop()
	{
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		IOverlayScreen? overlayScreen = Peek();
		if (overlayScreen == null || overlayScreen.UseSharedBackstop)
		{
			_backstop.MouseFilter = (MouseFilterEnum)2;
			Tween? backstopFade = _backstopFade;
			if (backstopFade != null)
			{
				backstopFade.Kill();
			}
			if (ScreenCount <= 1)
			{
				_backstopFade = ((Node)this).CreateTween();
				_backstopFade.TweenProperty((GodotObject)(object)_backstop, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0f), 0.5).SetEase((EaseType)1).SetTrans((TransitionType)7);
			}
			else
			{
				((CanvasItem)_backstop).Modulate = Colors.Transparent;
			}
		}
	}

	public IOverlayScreen? Peek()
	{
		return _overlays.LastOrDefault();
	}

	private void OnActiveScreenChanged()
	{
		IOverlayScreen overlayScreen = Peek();
		if (overlayScreen != null)
		{
			if (ActiveScreenContext.Instance.IsCurrent(overlayScreen))
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
		list.Add(new MethodInfo(MethodName.Clear, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.HideOverlays, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ShowOverlays, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ShowBackstop, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.HideBackstop, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
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
		if ((ref method) == MethodName.Clear && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			Clear();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.HideOverlays && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			HideOverlays();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.ShowOverlays && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			ShowOverlays();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.ShowBackstop && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			ShowBackstop();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.HideBackstop && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			HideBackstop();
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
		if ((ref method) == MethodName.Clear)
		{
			return true;
		}
		if ((ref method) == MethodName.HideOverlays)
		{
			return true;
		}
		if ((ref method) == MethodName.ShowOverlays)
		{
			return true;
		}
		if ((ref method) == MethodName.ShowBackstop)
		{
			return true;
		}
		if ((ref method) == MethodName.HideBackstop)
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
		if ((ref name) == PropertyName.ScreenCount)
		{
			int screenCount = ScreenCount;
			value = VariantUtils.CreateFrom<int>(ref screenCount);
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
		list.Add(new PropertyInfo((Type)2, PropertyName.ScreenCount, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotSignalList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(1);
		list.Add(new MethodInfo(SignalName.Changed, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	protected void EmitSignalChanged()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).EmitSignal(SignalName.Changed, Array.Empty<Variant>());
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RaiseGodotClassSignalCallbacks(in godot_string_name signal, NativeVariantPtrArgs args)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		if ((ref signal) == SignalName.Changed && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			backing_Changed?.Invoke();
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
		return ((Control)this).HasGodotClassSignal(ref signal);
	}
}
