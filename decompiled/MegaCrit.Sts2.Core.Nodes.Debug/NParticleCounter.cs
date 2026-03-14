using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Helpers;

namespace MegaCrit.Sts2.Core.Nodes.Debug;

[ScriptPath("res://src/Core/Nodes/Debug/NParticleCounter.cs")]
public class NParticleCounter : Control
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName _Input = StringName.op_Implicit("_Input");

		public static readonly StringName CheckForHotkey = StringName.op_Implicit("CheckForHotkey");

		public static readonly StringName _Process = StringName.op_Implicit("_Process");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName _icon = StringName.op_Implicit("_icon");

		public static readonly StringName _label = StringName.op_Implicit("_label");

		public static readonly StringName _secondsSinceLastUpdate = StringName.op_Implicit("_secondsSinceLastUpdate");

		public static readonly StringName _totalParticles = StringName.op_Implicit("_totalParticles");

		public static readonly StringName _updateCount = StringName.op_Implicit("_updateCount");
	}

	public class SignalName : SignalName
	{
	}

	private static readonly StringName _toggleParticleCounter = new StringName("toggle_particle_counter");

	private const float _secondsPerUpdate = 5f;

	private TextureRect _icon;

	private Label? _label;

	private double _secondsSinceLastUpdate;

	private int _totalParticles;

	private int _updateCount;

	public override void _Ready()
	{
		if (!OS.HasFeature("editor"))
		{
			((Node)(object)this).QueueFreeSafely();
			return;
		}
		_icon = ((Node)this).GetNode<TextureRect>(NodePath.op_Implicit("%Icon"));
		_label = ((Node)this).GetNode<Label>(NodePath.op_Implicit("%Label"));
		((CanvasItem)this).Visible = false;
	}

	public override void _Input(InputEvent inputEvent)
	{
		CheckForHotkey(inputEvent);
	}

	private void CheckForHotkey(InputEvent inputEvent)
	{
		if (inputEvent.IsActionReleased(_toggleParticleCounter, false) && !((CanvasItem)NDevConsole.Instance).Visible)
		{
			((CanvasItem)this).Visible = !((CanvasItem)this).Visible;
		}
	}

	public override void _Process(double delta)
	{
		if (!((CanvasItem)this).Visible || _label == null)
		{
			return;
		}
		_secondsSinceLastUpdate += delta;
		if ((_totalParticles > 1 || _updateCount > 500) && _secondsSinceLastUpdate < 5.0)
		{
			return;
		}
		_updateCount++;
		_secondsSinceLastUpdate = 0.0;
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		foreach (Node item in GetChildrenRecursive((Node)(object)((Node)this).GetTree().Root))
		{
			CpuParticles2D val = (CpuParticles2D)(object)((item is CpuParticles2D) ? item : null);
			if (val == null)
			{
				GpuParticles2D val2 = (GpuParticles2D)(object)((item is GpuParticles2D) ? item : null);
				if (val2 != null)
				{
					num3 += val2.Amount;
					num4++;
				}
			}
			else
			{
				num += val.Amount;
				num2++;
			}
		}
		_totalParticles = num + num3;
		_label.Text = $"All particles: {_totalParticles}\nCPU particles: {num} in {num2} node{((num2 == 1) ? "" : "s")}\nGPU particles: {num3} in {num4} node{((num4 == 1) ? "" : "s")}";
	}

	private static List<Node> GetChildrenRecursive(Node root)
	{
		int num = 1;
		List<Node> list = new List<Node>(num);
		CollectionsMarshal.SetCount(list, num);
		Span<Node> span = CollectionsMarshal.AsSpan(list);
		int index = 0;
		span[index] = root;
		List<Node> list2 = list;
		foreach (Node child in root.GetChildren(false))
		{
			list2.AddRange(GetChildrenRecursive(child));
		}
		return list2;
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
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Expected O, but got Unknown
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(4);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Input, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("inputEvent"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("InputEvent"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.CheckForHotkey, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("inputEvent"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("InputEvent"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Process, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)3, StringName.op_Implicit("delta"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
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
		if ((ref method) == MethodName.CheckForHotkey && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			CheckForHotkey(VariantUtils.ConvertTo<InputEvent>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._Process && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			((Node)this)._Process(VariantUtils.ConvertTo<double>(ref ((NativeVariantPtrArgs)(ref args))[0]));
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
		if ((ref method) == MethodName._Input)
		{
			return true;
		}
		if ((ref method) == MethodName.CheckForHotkey)
		{
			return true;
		}
		if ((ref method) == MethodName._Process)
		{
			return true;
		}
		return ((Control)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._icon)
		{
			_icon = VariantUtils.ConvertTo<TextureRect>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._label)
		{
			_label = VariantUtils.ConvertTo<Label>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._secondsSinceLastUpdate)
		{
			_secondsSinceLastUpdate = VariantUtils.ConvertTo<double>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._totalParticles)
		{
			_totalParticles = VariantUtils.ConvertTo<int>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._updateCount)
		{
			_updateCount = VariantUtils.ConvertTo<int>(ref value);
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
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName._icon)
		{
			value = VariantUtils.CreateFrom<TextureRect>(ref _icon);
			return true;
		}
		if ((ref name) == PropertyName._label)
		{
			value = VariantUtils.CreateFrom<Label>(ref _label);
			return true;
		}
		if ((ref name) == PropertyName._secondsSinceLastUpdate)
		{
			value = VariantUtils.CreateFrom<double>(ref _secondsSinceLastUpdate);
			return true;
		}
		if ((ref name) == PropertyName._totalParticles)
		{
			value = VariantUtils.CreateFrom<int>(ref _totalParticles);
			return true;
		}
		if ((ref name) == PropertyName._updateCount)
		{
			value = VariantUtils.CreateFrom<int>(ref _updateCount);
			return true;
		}
		return ((GodotObject)this).GetGodotClassPropertyValue(ref name, ref value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._icon, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._label, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)3, PropertyName._secondsSinceLastUpdate, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)2, PropertyName._totalParticles, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)2, PropertyName._updateCount, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		info.AddProperty(PropertyName._icon, Variant.From<TextureRect>(ref _icon));
		info.AddProperty(PropertyName._label, Variant.From<Label>(ref _label));
		info.AddProperty(PropertyName._secondsSinceLastUpdate, Variant.From<double>(ref _secondsSinceLastUpdate));
		info.AddProperty(PropertyName._totalParticles, Variant.From<int>(ref _totalParticles));
		info.AddProperty(PropertyName._updateCount, Variant.From<int>(ref _updateCount));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._icon, ref val))
		{
			_icon = ((Variant)(ref val)).As<TextureRect>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._label, ref val2))
		{
			_label = ((Variant)(ref val2)).As<Label>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._secondsSinceLastUpdate, ref val3))
		{
			_secondsSinceLastUpdate = ((Variant)(ref val3)).As<double>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._totalParticles, ref val4))
		{
			_totalParticles = ((Variant)(ref val4)).As<int>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._updateCount, ref val5))
		{
			_updateCount = ((Variant)(ref val5)).As<int>();
		}
	}
}
