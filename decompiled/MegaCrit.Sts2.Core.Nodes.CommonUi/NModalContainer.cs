using System;
using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Nodes.Screens.ScreenContext;

namespace MegaCrit.Sts2.Core.Nodes.CommonUi;

[ScriptPath("res://src/Core/Nodes/CommonUi/NModalContainer.cs")]
public class NModalContainer : Control
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName Add = StringName.op_Implicit("Add");

		public static readonly StringName Clear = StringName.op_Implicit("Clear");

		public static readonly StringName ShowBackstop = StringName.op_Implicit("ShowBackstop");

		public static readonly StringName HideBackstop = StringName.op_Implicit("HideBackstop");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName _backstop = StringName.op_Implicit("_backstop");

		public static readonly StringName _backstopTween = StringName.op_Implicit("_backstopTween");
	}

	public class SignalName : SignalName
	{
	}

	private ColorRect _backstop;

	private Tween? _backstopTween;

	public static NModalContainer? Instance { get; private set; }

	public IScreenContext? OpenModal { get; private set; }

	public override void _Ready()
	{
		if (Instance != null)
		{
			Log.Error("NModalContainer already exists.");
			((Node)(object)this).QueueFreeSafely();
		}
		else
		{
			Instance = this;
			_backstop = ((Node)this).GetNode<ColorRect>(NodePath.op_Implicit("Backstop"));
		}
	}

	public void Add(Node modalToCreate, bool showBackstop = true)
	{
		if (OpenModal != null)
		{
			Log.Warn("There's another modal already open.");
			return;
		}
		OpenModal = (IScreenContext)modalToCreate;
		((Node)(object)this).AddChildSafely(modalToCreate);
		ActiveScreenContext.Instance.Update();
		if (showBackstop)
		{
			ShowBackstop();
		}
	}

	public void Clear()
	{
		foreach (Node child in ((Node)this).GetChildren(false))
		{
			if ((object)child != _backstop)
			{
				child.QueueFreeSafely();
			}
		}
		OpenModal = null;
		ActiveScreenContext.Instance.Update();
		HideBackstop();
	}

	public void ShowBackstop()
	{
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		((Control)this).MouseFilter = (MouseFilterEnum)0;
		((CanvasItem)_backstop).Visible = true;
		Tween? backstopTween = _backstopTween;
		if (backstopTween != null)
		{
			backstopTween.Kill();
		}
		_backstopTween = ((Node)this).CreateTween();
		_backstopTween.TweenProperty((GodotObject)(object)_backstop, NodePath.op_Implicit("color:a"), Variant.op_Implicit(0.85f), 0.3);
	}

	public void HideBackstop()
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		((Control)this).MouseFilter = (MouseFilterEnum)2;
		Tween? backstopTween = _backstopTween;
		if (backstopTween != null)
		{
			backstopTween.Kill();
		}
		_backstopTween = ((Node)this).CreateTween();
		_backstopTween.TweenProperty((GodotObject)(object)_backstop, NodePath.op_Implicit("color:a"), Variant.op_Implicit(0f), 0.3);
		_backstopTween.TweenCallback(Callable.From<bool>((Func<bool>)(() => ((CanvasItem)_backstop).Visible = false)));
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
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(5);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.Add, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("modalToCreate"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Node"), false),
			new PropertyInfo((Type)1, StringName.op_Implicit("showBackstop"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.Clear, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ShowBackstop, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.HideBackstop, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.Add && ((NativeVariantPtrArgs)(ref args)).Count == 2)
		{
			Add(VariantUtils.ConvertTo<Node>(ref ((NativeVariantPtrArgs)(ref args))[0]), VariantUtils.ConvertTo<bool>(ref ((NativeVariantPtrArgs)(ref args))[1]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.Clear && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			Clear();
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
		return ((Control)this).InvokeGodotClassMethod(ref method, args, ref ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if ((ref method) == MethodName._Ready)
		{
			return true;
		}
		if ((ref method) == MethodName.Add)
		{
			return true;
		}
		if ((ref method) == MethodName.Clear)
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
		return ((Control)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._backstop)
		{
			_backstop = VariantUtils.ConvertTo<ColorRect>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._backstopTween)
		{
			_backstopTween = VariantUtils.ConvertTo<Tween>(ref value);
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
		if ((ref name) == PropertyName._backstop)
		{
			value = VariantUtils.CreateFrom<ColorRect>(ref _backstop);
			return true;
		}
		if ((ref name) == PropertyName._backstopTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _backstopTween);
			return true;
		}
		return ((GodotObject)this).GetGodotClassPropertyValue(ref name, ref value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._backstop, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._backstopTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		info.AddProperty(PropertyName._backstop, Variant.From<ColorRect>(ref _backstop));
		info.AddProperty(PropertyName._backstopTween, Variant.From<Tween>(ref _backstopTween));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._backstop, ref val))
		{
			_backstop = ((Variant)(ref val)).As<ColorRect>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._backstopTween, ref val2))
		{
			_backstopTween = ((Variant)(ref val2)).As<Tween>();
		}
	}
}
