using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.ControllerInput;
using MegaCrit.Sts2.Core.Nodes.Debug;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;

namespace MegaCrit.Sts2.Core.Nodes.CommonUi;

[ScriptPath("res://src/Core/Nodes/CommonUi/NHotkeyManager.cs")]
public class NHotkeyManager : Node
{
	public class MethodName : MethodName
	{
		public static readonly StringName AddBlockingScreen = StringName.op_Implicit("AddBlockingScreen");

		public static readonly StringName RemoveBlockingScreen = StringName.op_Implicit("RemoveBlockingScreen");

		public static readonly StringName _UnhandledInput = StringName.op_Implicit("_UnhandledInput");
	}

	public class PropertyName : PropertyName
	{
	}

	public class SignalName : SignalName
	{
	}

	private readonly Dictionary<StringName, List<Action>> _hotkeyPressedBindings = new Dictionary<StringName, List<Action>>();

	private readonly Dictionary<StringName, List<Action>> _hotkeyReleasedBindings = new Dictionary<StringName, List<Action>>();

	private Dictionary<Node, Action> _blockingScreens = new Dictionary<Node, Action>();

	public static NHotkeyManager? Instance
	{
		get
		{
			if (NGame.Instance == null)
			{
				return null;
			}
			return NGame.Instance.HotkeyManager;
		}
	}

	public void PushHotkeyPressedBinding(string hotkey, Action action)
	{
		if (!_hotkeyPressedBindings.ContainsKey(StringName.op_Implicit(hotkey)))
		{
			_hotkeyPressedBindings.Add(StringName.op_Implicit(hotkey), new List<Action>());
		}
		if (!_hotkeyPressedBindings[StringName.op_Implicit(hotkey)].Contains(action))
		{
			_hotkeyPressedBindings[StringName.op_Implicit(hotkey)].Add(action);
		}
	}

	public void RemoveHotkeyPressedBinding(string hotkey, Action action)
	{
		if (_hotkeyPressedBindings.TryGetValue(StringName.op_Implicit(hotkey), out List<Action> value))
		{
			value.Remove(action);
			if (_hotkeyPressedBindings[StringName.op_Implicit(hotkey)].Count == 0)
			{
				_hotkeyPressedBindings.Remove(StringName.op_Implicit(hotkey));
			}
		}
	}

	public void PushHotkeyReleasedBinding(string hotkey, Action action)
	{
		if (!_hotkeyReleasedBindings.ContainsKey(StringName.op_Implicit(hotkey)))
		{
			_hotkeyReleasedBindings.Add(StringName.op_Implicit(hotkey), new List<Action>());
		}
		if (!_hotkeyReleasedBindings[StringName.op_Implicit(hotkey)].Contains(action))
		{
			_hotkeyReleasedBindings[StringName.op_Implicit(hotkey)].Add(action);
		}
	}

	public void RemoveHotkeyReleasedBinding(string hotkey, Action action)
	{
		if (_hotkeyReleasedBindings.TryGetValue(StringName.op_Implicit(hotkey), out List<Action> value))
		{
			value.Remove(action);
			if (_hotkeyReleasedBindings[StringName.op_Implicit(hotkey)].Count == 0)
			{
				_hotkeyReleasedBindings.Remove(StringName.op_Implicit(hotkey));
			}
		}
	}

	public void AddBlockingScreen(Node screen)
	{
		Action action = delegate
		{
		};
		string[] allInputs = MegaInput.AllInputs;
		foreach (string hotkey in allInputs)
		{
			PushHotkeyPressedBinding(hotkey, action);
		}
		string[] allInputs2 = MegaInput.AllInputs;
		foreach (string hotkey2 in allInputs2)
		{
			PushHotkeyReleasedBinding(hotkey2, action);
		}
		_blockingScreens.Add(screen, action);
	}

	public void RemoveBlockingScreen(Node screen)
	{
		if (_blockingScreens.TryGetValue(screen, out Action value))
		{
			string[] allInputs = MegaInput.AllInputs;
			foreach (string hotkey in allInputs)
			{
				RemoveHotkeyPressedBinding(hotkey, value);
			}
			string[] allInputs2 = MegaInput.AllInputs;
			foreach (string hotkey2 in allInputs2)
			{
				RemoveHotkeyReleasedBinding(hotkey2, value);
			}
			_blockingScreens.Remove(screen);
		}
	}

	public override void _UnhandledInput(InputEvent inputEvent)
	{
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		if (((CanvasItem)NDevConsole.Instance).Visible)
		{
			return;
		}
		Viewport viewport = ((Node)this).GetViewport();
		Control val = ((viewport != null) ? viewport.GuiGetFocusOwner() : null);
		if (val != null)
		{
			LineEdit val2 = (LineEdit)(object)((val is LineEdit) ? val : null);
			if ((val2 != null && val2.IsEditing()) || (val is NMegaTextEdit nMegaTextEdit && nMegaTextEdit.IsEditing()))
			{
				return;
			}
		}
		Callable val3;
		foreach (KeyValuePair<StringName, List<Action>> hotkeyPressedBinding in _hotkeyPressedBindings)
		{
			if (inputEvent.IsActionPressed(hotkeyPressedBinding.Key, false, false) && !inputEvent.IsEcho())
			{
				Action action = hotkeyPressedBinding.Value.LastOrDefault();
				if (action != null)
				{
					val3 = Callable.From((Action)action.Invoke);
					((Callable)(ref val3)).CallDeferred(Array.Empty<Variant>());
				}
			}
		}
		foreach (KeyValuePair<StringName, List<Action>> hotkeyReleasedBinding in _hotkeyReleasedBindings)
		{
			if (inputEvent.IsActionReleased(hotkeyReleasedBinding.Key, false))
			{
				Action action2 = hotkeyReleasedBinding.Value.LastOrDefault();
				if (action2 != null)
				{
					val3 = Callable.From((Action)action2.Invoke);
					((Callable)(ref val3)).CallDeferred(Array.Empty<Variant>());
				}
			}
		}
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
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Expected O, but got Unknown
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(3);
		list.Add(new MethodInfo(MethodName.AddBlockingScreen, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("screen"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Node"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.RemoveBlockingScreen, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("screen"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Node"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._UnhandledInput, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("inputEvent"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("InputEvent"), false)
		}, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.AddBlockingScreen && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			AddBlockingScreen(VariantUtils.ConvertTo<Node>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.RemoveBlockingScreen && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			RemoveBlockingScreen(VariantUtils.ConvertTo<Node>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._UnhandledInput && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			((Node)this)._UnhandledInput(VariantUtils.ConvertTo<InputEvent>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		return ((Node)this).InvokeGodotClassMethod(ref method, args, ref ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if ((ref method) == MethodName.AddBlockingScreen)
		{
			return true;
		}
		if ((ref method) == MethodName.RemoveBlockingScreen)
		{
			return true;
		}
		if ((ref method) == MethodName._UnhandledInput)
		{
			return true;
		}
		return ((Node)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).SaveGodotObjectData(info);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
	}
}
