using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History;
using MegaCrit.Sts2.Core.Helpers;

namespace MegaCrit.Sts2.Core.Nodes.Debug;

[ScriptPath("res://src/Core/Nodes/Debug/NCommandHistory.cs")]
public class NCommandHistory : Panel
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName _Input = StringName.op_Implicit("_Input");

		public static readonly StringName SetBackgroundColor = StringName.op_Implicit("SetBackgroundColor");

		public static readonly StringName ShowConsole = StringName.op_Implicit("ShowConsole");

		public static readonly StringName HideConsole = StringName.op_Implicit("HideConsole");

		public static readonly StringName GetHistory = StringName.op_Implicit("GetHistory");

		public static readonly StringName Refresh = StringName.op_Implicit("Refresh");

		public static readonly StringName GetText = StringName.op_Implicit("GetText");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName _outputBuffer = StringName.op_Implicit("_outputBuffer");
	}

	public class SignalName : SignalName
	{
	}

	private static NCommandHistory? _instance;

	private RichTextLabel _outputBuffer;

	public override void _Ready()
	{
		if (_instance != null)
		{
			((Node)(object)this).QueueFreeSafely();
			return;
		}
		_instance = this;
		HideConsole();
		_outputBuffer = ((Node)this).GetNode<RichTextLabel>(NodePath.op_Implicit("OutputContainer/OutputBuffer"));
	}

	public override void _Input(InputEvent inputEvent)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Invalid comparison between Unknown and I8
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Invalid comparison between Unknown and I8
		InputEventKey val = (InputEventKey)(object)((inputEvent is InputEventKey) ? inputEvent : null);
		if (val == null || !val.Pressed)
		{
			return;
		}
		if ((long)val.Keycode == 43)
		{
			if (((CanvasItem)this).Visible)
			{
				HideConsole();
			}
			else
			{
				ShowConsole();
			}
		}
		if (((CanvasItem)this).Visible && (long)val.Keycode == 4194305)
		{
			HideConsole();
		}
	}

	public void SetBackgroundColor(Color color)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		((CanvasItem)this).Modulate = color;
	}

	public void ShowConsole()
	{
		((CanvasItem)this).Visible = true;
		CombatManager.Instance.History.Changed += Refresh;
		Refresh();
	}

	public void HideConsole()
	{
		CombatManager.Instance.History.Changed -= Refresh;
		((CanvasItem)this).Visible = false;
	}

	public static string GetHistory()
	{
		if (_instance != null)
		{
			return GetText();
		}
		return string.Empty;
	}

	private void Refresh()
	{
		_outputBuffer.Text = GetText();
	}

	private static string GetText()
	{
		return string.Join('\n', CombatManager.Instance.History.Entries.Select((CombatHistoryEntry e) => e.HumanReadableString));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Expected O, but got Unknown
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(8);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Input, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("inputEvent"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("InputEvent"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetBackgroundColor, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)20, StringName.op_Implicit("color"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ShowConsole, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.HideConsole, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.GetHistory, new PropertyInfo((Type)4, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)33, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.Refresh, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.GetText, new PropertyInfo((Type)4, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)33, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._Input && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			((Node)this)._Input(VariantUtils.ConvertTo<InputEvent>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetBackgroundColor && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			SetBackgroundColor(VariantUtils.ConvertTo<Color>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.ShowConsole && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			ShowConsole();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.HideConsole && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			HideConsole();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.GetHistory && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			string history = GetHistory();
			ret = VariantUtils.CreateFrom<string>(ref history);
			return true;
		}
		if ((ref method) == MethodName.Refresh && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			Refresh();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.GetText && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			string text = GetText();
			ret = VariantUtils.CreateFrom<string>(ref text);
			return true;
		}
		return ((Panel)this).InvokeGodotClassMethod(ref method, args, ref ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static bool InvokeGodotClassStaticMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.GetHistory && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			string history = GetHistory();
			ret = VariantUtils.CreateFrom<string>(ref history);
			return true;
		}
		if ((ref method) == MethodName.GetText && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			string text = GetText();
			ret = VariantUtils.CreateFrom<string>(ref text);
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
		if ((ref method) == MethodName._Input)
		{
			return true;
		}
		if ((ref method) == MethodName.SetBackgroundColor)
		{
			return true;
		}
		if ((ref method) == MethodName.ShowConsole)
		{
			return true;
		}
		if ((ref method) == MethodName.HideConsole)
		{
			return true;
		}
		if ((ref method) == MethodName.GetHistory)
		{
			return true;
		}
		if ((ref method) == MethodName.Refresh)
		{
			return true;
		}
		if ((ref method) == MethodName.GetText)
		{
			return true;
		}
		return ((Panel)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._outputBuffer)
		{
			_outputBuffer = VariantUtils.ConvertTo<RichTextLabel>(ref value);
			return true;
		}
		return ((GodotObject)this).SetGodotClassPropertyValue(ref name, ref value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName._outputBuffer)
		{
			value = VariantUtils.CreateFrom<RichTextLabel>(ref _outputBuffer);
			return true;
		}
		return ((GodotObject)this).GetGodotClassPropertyValue(ref name, ref value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._outputBuffer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		info.AddProperty(PropertyName._outputBuffer, Variant.From<RichTextLabel>(ref _outputBuffer));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._outputBuffer, ref val))
		{
			_outputBuffer = ((Variant)(ref val)).As<RichTextLabel>();
		}
	}
}
