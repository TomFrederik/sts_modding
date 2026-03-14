using System;
using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Nodes.Screens.ScreenContext;

namespace MegaCrit.Sts2.Core.Nodes.Screens.MainMenu;

[ScriptPath("res://src/Core/Nodes/Screens/MainMenu/NSubmenuStack.cs")]
public abstract class NSubmenuStack : Control
{
	[Signal]
	public delegate void StackModifiedEventHandler();

	public class MethodName : MethodName
	{
		public static readonly StringName InitializeForMainMenu = StringName.op_Implicit("InitializeForMainMenu");

		public static readonly StringName Push = StringName.op_Implicit("Push");

		public static readonly StringName Pop = StringName.op_Implicit("Pop");

		public static readonly StringName ShowBackstop = StringName.op_Implicit("ShowBackstop");

		public static readonly StringName HideBackstop = StringName.op_Implicit("HideBackstop");

		public static readonly StringName Peek = StringName.op_Implicit("Peek");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName SubmenusOpen = StringName.op_Implicit("SubmenusOpen");

		public static readonly StringName _mainMenu = StringName.op_Implicit("_mainMenu");
	}

	public class SignalName : SignalName
	{
		public static readonly StringName StackModified = StringName.op_Implicit("StackModified");
	}

	private readonly Stack<NSubmenu> _submenus = new Stack<NSubmenu>();

	private NMainMenu? _mainMenu;

	private StackModifiedEventHandler backing_StackModified;

	public bool SubmenusOpen => _submenus.Count > 0;

	public event StackModifiedEventHandler StackModified
	{
		add
		{
			backing_StackModified = (StackModifiedEventHandler)Delegate.Combine(backing_StackModified, value);
		}
		remove
		{
			backing_StackModified = (StackModifiedEventHandler)Delegate.Remove(backing_StackModified, value);
		}
	}

	public void InitializeForMainMenu(NMainMenu mainMenu)
	{
		_mainMenu = mainMenu;
	}

	public abstract T PushSubmenuType<T>() where T : NSubmenu;

	public abstract T GetSubmenuType<T>() where T : NSubmenu;

	public abstract NSubmenu PushSubmenuType(Type type);

	public abstract NSubmenu GetSubmenuType(Type type);

	public void Push(NSubmenu screen)
	{
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		if (_submenus.Count > 0)
		{
			NSubmenu nSubmenu = _submenus.Peek();
			((CanvasItem)nSubmenu).Visible = false;
			((Control)nSubmenu).MouseFilter = (MouseFilterEnum)2;
		}
		screen.SetStack(this);
		_submenus.Push(screen);
		screen.OnSubmenuOpened();
		((CanvasItem)screen).Visible = true;
		((Control)screen).MouseFilter = (MouseFilterEnum)0;
		_mainMenu?.EnableBackstop();
		ActiveScreenContext.Instance.Update();
		((GodotObject)this).EmitSignal(SignalName.StackModified, Array.Empty<Variant>());
	}

	public void Pop()
	{
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		NSubmenu nSubmenu = _submenus.Pop();
		((CanvasItem)nSubmenu).Visible = false;
		((Control)nSubmenu).MouseFilter = (MouseFilterEnum)2;
		nSubmenu.OnSubmenuClosed();
		if (_submenus.Count > 0)
		{
			NSubmenu nSubmenu2 = _submenus.Peek();
			((CanvasItem)nSubmenu2).Visible = true;
			((Control)nSubmenu2).MouseFilter = (MouseFilterEnum)0;
		}
		else
		{
			HideBackstop();
		}
		ActiveScreenContext.Instance.Update();
		((GodotObject)this).EmitSignal(SignalName.StackModified, Array.Empty<Variant>());
	}

	private void ShowBackstop()
	{
		_mainMenu?.EnableBackstop();
	}

	private void HideBackstop()
	{
		_mainMenu?.DisableBackstop();
	}

	public NSubmenu? Peek()
	{
		if (!_submenus.TryPeek(out NSubmenu result))
		{
			return null;
		}
		return result;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Expected O, but got Unknown
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Expected O, but got Unknown
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Expected O, but got Unknown
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(6);
		list.Add(new MethodInfo(MethodName.InitializeForMainMenu, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("mainMenu"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.Push, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("screen"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.Pop, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ShowBackstop, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.HideBackstop, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.Peek, new PropertyInfo((Type)24, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.InitializeForMainMenu && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			InitializeForMainMenu(VariantUtils.ConvertTo<NMainMenu>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.Push && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			Push(VariantUtils.ConvertTo<NSubmenu>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.Pop && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			Pop();
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
		if ((ref method) == MethodName.Peek && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			NSubmenu nSubmenu = Peek();
			ret = VariantUtils.CreateFrom<NSubmenu>(ref nSubmenu);
			return true;
		}
		return ((Control)this).InvokeGodotClassMethod(ref method, args, ref ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if ((ref method) == MethodName.InitializeForMainMenu)
		{
			return true;
		}
		if ((ref method) == MethodName.Push)
		{
			return true;
		}
		if ((ref method) == MethodName.Pop)
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
		if ((ref method) == MethodName.Peek)
		{
			return true;
		}
		return ((Control)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._mainMenu)
		{
			_mainMenu = VariantUtils.ConvertTo<NMainMenu>(ref value);
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
		if ((ref name) == PropertyName.SubmenusOpen)
		{
			bool submenusOpen = SubmenusOpen;
			value = VariantUtils.CreateFrom<bool>(ref submenusOpen);
			return true;
		}
		if ((ref name) == PropertyName._mainMenu)
		{
			value = VariantUtils.CreateFrom<NMainMenu>(ref _mainMenu);
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
		list.Add(new PropertyInfo((Type)24, PropertyName._mainMenu, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName.SubmenusOpen, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		info.AddProperty(PropertyName._mainMenu, Variant.From<NMainMenu>(ref _mainMenu));
		info.AddSignalEventDelegate(SignalName.StackModified, (Delegate)backing_StackModified);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._mainMenu, ref val))
		{
			_mainMenu = ((Variant)(ref val)).As<NMainMenu>();
		}
		StackModifiedEventHandler stackModifiedEventHandler = default(StackModifiedEventHandler);
		if (info.TryGetSignalEventDelegate<StackModifiedEventHandler>(SignalName.StackModified, ref stackModifiedEventHandler))
		{
			backing_StackModified = stackModifiedEventHandler;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotSignalList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(1);
		list.Add(new MethodInfo(SignalName.StackModified, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	protected void EmitSignalStackModified()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).EmitSignal(SignalName.StackModified, Array.Empty<Variant>());
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RaiseGodotClassSignalCallbacks(in godot_string_name signal, NativeVariantPtrArgs args)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		if ((ref signal) == SignalName.StackModified && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			backing_StackModified?.Invoke();
		}
		else
		{
			((GodotObject)this).RaiseGodotClassSignalCallbacks(ref signal, args);
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassSignal(in godot_string_name signal)
	{
		if ((ref signal) == SignalName.StackModified)
		{
			return true;
		}
		return ((Control)this).HasGodotClassSignal(ref signal);
	}
}
