using System;
using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;

namespace MegaCrit.Sts2.Core.Nodes.Screens;

[ScriptPath("res://src/Core/Nodes/Screens/NAncientBgContainer.cs")]
public class NAncientBgContainer : Control
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName OnWindowChange = StringName.op_Implicit("OnWindowChange");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName _window = StringName.op_Implicit("_window");

		public static readonly StringName pos4_3 = StringName.op_Implicit("pos4_3");

		public static readonly StringName scale4_3 = StringName.op_Implicit("scale4_3");

		public static readonly StringName pos16_9 = StringName.op_Implicit("pos16_9");

		public static readonly StringName scale16_9 = StringName.op_Implicit("scale16_9");

		public static readonly StringName pos21_9 = StringName.op_Implicit("pos21_9");

		public static readonly StringName scale21_9 = StringName.op_Implicit("scale21_9");
	}

	public class SignalName : SignalName
	{
	}

	private Window _window;

	private const float _ratioMin = 1.3333f;

	private const float _ratioNormal = 1.7777f;

	private const float _ratioMax = 2.3333f;

	private Vector2 pos4_3 = new Vector2(-140f, 110f);

	private Vector2 scale4_3 = new Vector2(1f, 1f);

	private Vector2 pos16_9 = new Vector2(0f, 40f);

	private Vector2 scale16_9 = new Vector2(0.89f, 0.89f);

	private Vector2 pos21_9 = new Vector2(330f, 40f);

	private Vector2 scale21_9 = new Vector2(1f, 1f);

	public override void _Ready()
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		_window = ((Node)this).GetTree().Root;
		((GodotObject)_window).Connect(SignalName.SizeChanged, Callable.From((Action)OnWindowChange), 0u);
		OnWindowChange();
	}

	private void OnWindowChange()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		float num = Mathf.Clamp(((Control)this).Size.X / ((Control)this).Size.Y, 1.3333f, 2.3333f);
		((Control)this).PivotOffset = ((Control)this).Size * 0.5f;
		if (num < 1.7777f)
		{
			float num2 = Mathf.InverseLerp(1.3333f, 1.7777f, num);
			((Control)this).Position = ((Vector2)(ref pos4_3)).Lerp(pos16_9, num2);
			((Control)this).Scale = ((Vector2)(ref scale4_3)).Lerp(scale16_9, num2);
		}
		else
		{
			float num3 = Mathf.InverseLerp(1.7777f, 2.3333f, num);
			((Control)this).Position = ((Vector2)(ref pos16_9)).Lerp(pos21_9, num3);
			((Control)this).Scale = ((Vector2)(ref scale16_9)).Lerp(scale21_9, num3);
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(2);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnWindowChange, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnWindowChange && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnWindowChange();
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
		if ((ref method) == MethodName.OnWindowChange)
		{
			return true;
		}
		return ((Control)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName._window)
		{
			_window = VariantUtils.ConvertTo<Window>(ref value);
			return true;
		}
		if ((ref name) == PropertyName.pos4_3)
		{
			pos4_3 = VariantUtils.ConvertTo<Vector2>(ref value);
			return true;
		}
		if ((ref name) == PropertyName.scale4_3)
		{
			scale4_3 = VariantUtils.ConvertTo<Vector2>(ref value);
			return true;
		}
		if ((ref name) == PropertyName.pos16_9)
		{
			pos16_9 = VariantUtils.ConvertTo<Vector2>(ref value);
			return true;
		}
		if ((ref name) == PropertyName.scale16_9)
		{
			scale16_9 = VariantUtils.ConvertTo<Vector2>(ref value);
			return true;
		}
		if ((ref name) == PropertyName.pos21_9)
		{
			pos21_9 = VariantUtils.ConvertTo<Vector2>(ref value);
			return true;
		}
		if ((ref name) == PropertyName.scale21_9)
		{
			scale21_9 = VariantUtils.ConvertTo<Vector2>(ref value);
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
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName._window)
		{
			value = VariantUtils.CreateFrom<Window>(ref _window);
			return true;
		}
		if ((ref name) == PropertyName.pos4_3)
		{
			value = VariantUtils.CreateFrom<Vector2>(ref pos4_3);
			return true;
		}
		if ((ref name) == PropertyName.scale4_3)
		{
			value = VariantUtils.CreateFrom<Vector2>(ref scale4_3);
			return true;
		}
		if ((ref name) == PropertyName.pos16_9)
		{
			value = VariantUtils.CreateFrom<Vector2>(ref pos16_9);
			return true;
		}
		if ((ref name) == PropertyName.scale16_9)
		{
			value = VariantUtils.CreateFrom<Vector2>(ref scale16_9);
			return true;
		}
		if ((ref name) == PropertyName.pos21_9)
		{
			value = VariantUtils.CreateFrom<Vector2>(ref pos21_9);
			return true;
		}
		if ((ref name) == PropertyName.scale21_9)
		{
			value = VariantUtils.CreateFrom<Vector2>(ref scale21_9);
			return true;
		}
		return ((GodotObject)this).GetGodotClassPropertyValue(ref name, ref value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._window, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)5, PropertyName.pos4_3, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)5, PropertyName.scale4_3, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)5, PropertyName.pos16_9, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)5, PropertyName.scale16_9, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)5, PropertyName.pos21_9, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)5, PropertyName.scale21_9, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		info.AddProperty(PropertyName._window, Variant.From<Window>(ref _window));
		info.AddProperty(PropertyName.pos4_3, Variant.From<Vector2>(ref pos4_3));
		info.AddProperty(PropertyName.scale4_3, Variant.From<Vector2>(ref scale4_3));
		info.AddProperty(PropertyName.pos16_9, Variant.From<Vector2>(ref pos16_9));
		info.AddProperty(PropertyName.scale16_9, Variant.From<Vector2>(ref scale16_9));
		info.AddProperty(PropertyName.pos21_9, Variant.From<Vector2>(ref pos21_9));
		info.AddProperty(PropertyName.scale21_9, Variant.From<Vector2>(ref scale21_9));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._window, ref val))
		{
			_window = ((Variant)(ref val)).As<Window>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName.pos4_3, ref val2))
		{
			pos4_3 = ((Variant)(ref val2)).As<Vector2>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName.scale4_3, ref val3))
		{
			scale4_3 = ((Variant)(ref val3)).As<Vector2>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName.pos16_9, ref val4))
		{
			pos16_9 = ((Variant)(ref val4)).As<Vector2>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName.scale16_9, ref val5))
		{
			scale16_9 = ((Variant)(ref val5)).As<Vector2>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName.pos21_9, ref val6))
		{
			pos21_9 = ((Variant)(ref val6)).As<Vector2>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName.scale21_9, ref val7))
		{
			scale21_9 = ((Variant)(ref val7)).As<Vector2>();
		}
	}
}
