using System;
using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.ControllerInput;
using MegaCrit.Sts2.Core.Localization.Fonts;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Platform;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.Nodes.GodotExtensions;

[ScriptPath("res://src/Core/Nodes/GodotExtensions/NMegaTextEdit.cs")]
public class NMegaTextEdit : TextEdit
{
	public class MethodName : MethodName
	{
		public static readonly StringName IsEditing = StringName.op_Implicit("IsEditing");

		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName RefreshFont = StringName.op_Implicit("RefreshFont");

		public static readonly StringName OnFocus = StringName.op_Implicit("OnFocus");

		public static readonly StringName OnUnfocus = StringName.op_Implicit("OnUnfocus");

		public static readonly StringName _GuiInput = StringName.op_Implicit("_GuiInput");

		public static readonly StringName OpenKeyboard = StringName.op_Implicit("OpenKeyboard");

		public static readonly StringName StopEditing = StringName.op_Implicit("StopEditing");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName _selectionReticle = StringName.op_Implicit("_selectionReticle");

		public static readonly StringName _isEditing = StringName.op_Implicit("_isEditing");
	}

	public class SignalName : SignalName
	{
	}

	private NSelectionReticle? _selectionReticle;

	private bool _isEditing;

	public bool IsEditing()
	{
		return _isEditing;
	}

	public override void _Ready()
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		RefreshFont();
		((GodotObject)this).Connect(SignalName.FocusExited, Callable.From((Action)StopEditing), 0u);
		if (((Node)this).HasNode(NodePath.op_Implicit("SelectionReticle")))
		{
			_selectionReticle = ((Node)this).GetNode<NSelectionReticle>(NodePath.op_Implicit("SelectionReticle"));
		}
		((GodotObject)this).Connect(SignalName.FocusEntered, Callable.From((Action)OnFocus), 0u);
		((GodotObject)this).Connect(SignalName.FocusExited, Callable.From((Action)OnUnfocus), 0u);
		((GodotObject)this).Connect(SignalName.MouseEntered, Callable.From((Action)OnFocus), 0u);
		((GodotObject)this).Connect(SignalName.MouseExited, Callable.From((Action)OnUnfocus), 0u);
	}

	public void RefreshFont()
	{
		((Control)(object)this).ApplyLocaleFontSubstitution(FontType.Regular, ThemeConstants.TextEdit.font);
	}

	private void OnFocus()
	{
		if (NControllerManager.Instance.IsUsingController)
		{
			_selectionReticle?.OnSelect();
		}
	}

	private void OnUnfocus()
	{
		_selectionReticle?.OnDeselect();
	}

	public override void _GuiInput(InputEvent inputEvent)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Invalid comparison between Unknown and I8
		((Control)this)._GuiInput(inputEvent);
		InputEventMouseButton val = (InputEventMouseButton)(object)((inputEvent is InputEventMouseButton) ? inputEvent : null);
		if (val != null && (long)val.ButtonIndex == 1 && ((InputEvent)val).IsPressed())
		{
			OpenKeyboard();
		}
		if (inputEvent.IsActionPressed(MegaInput.select, false, false))
		{
			OpenKeyboard();
		}
		if (inputEvent.IsActionPressed(MegaInput.cancel, false, false) && IsEditing())
		{
			StopEditing();
			Viewport viewport = ((Node)this).GetViewport();
			if (viewport != null)
			{
				viewport.SetInputAsHandled();
			}
		}
	}

	private void OpenKeyboard()
	{
		((Control)(object)this).TryGrabFocus();
		_isEditing = true;
		PlatformUtil.OpenVirtualKeyboard();
	}

	private void StopEditing()
	{
		_isEditing = false;
		PlatformUtil.CloseVirtualKeyboard();
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
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Expected O, but got Unknown
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(8);
		list.Add(new MethodInfo(MethodName.IsEditing, new PropertyInfo((Type)1, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.RefreshFont, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnFocus, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnUnfocus, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._GuiInput, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("inputEvent"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("InputEvent"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OpenKeyboard, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.StopEditing, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.IsEditing && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			bool flag = IsEditing();
			ret = VariantUtils.CreateFrom<bool>(ref flag);
			return true;
		}
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.RefreshFont && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			RefreshFont();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnFocus && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnFocus();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnUnfocus && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnUnfocus();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._GuiInput && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			((Control)this)._GuiInput(VariantUtils.ConvertTo<InputEvent>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OpenKeyboard && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OpenKeyboard();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.StopEditing && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			StopEditing();
			ret = default(godot_variant);
			return true;
		}
		return ((TextEdit)this).InvokeGodotClassMethod(ref method, args, ref ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if ((ref method) == MethodName.IsEditing)
		{
			return true;
		}
		if ((ref method) == MethodName._Ready)
		{
			return true;
		}
		if ((ref method) == MethodName.RefreshFont)
		{
			return true;
		}
		if ((ref method) == MethodName.OnFocus)
		{
			return true;
		}
		if ((ref method) == MethodName.OnUnfocus)
		{
			return true;
		}
		if ((ref method) == MethodName._GuiInput)
		{
			return true;
		}
		if ((ref method) == MethodName.OpenKeyboard)
		{
			return true;
		}
		if ((ref method) == MethodName.StopEditing)
		{
			return true;
		}
		return ((TextEdit)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._selectionReticle)
		{
			_selectionReticle = VariantUtils.ConvertTo<NSelectionReticle>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._isEditing)
		{
			_isEditing = VariantUtils.ConvertTo<bool>(ref value);
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
		if ((ref name) == PropertyName._selectionReticle)
		{
			value = VariantUtils.CreateFrom<NSelectionReticle>(ref _selectionReticle);
			return true;
		}
		if ((ref name) == PropertyName._isEditing)
		{
			value = VariantUtils.CreateFrom<bool>(ref _isEditing);
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
		list.Add(new PropertyInfo((Type)24, PropertyName._selectionReticle, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._isEditing, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		info.AddProperty(PropertyName._selectionReticle, Variant.From<NSelectionReticle>(ref _selectionReticle));
		info.AddProperty(PropertyName._isEditing, Variant.From<bool>(ref _isEditing));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._selectionReticle, ref val))
		{
			_selectionReticle = ((Variant)(ref val)).As<NSelectionReticle>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._isEditing, ref val2))
		{
			_isEditing = ((Variant)(ref val2)).As<bool>();
		}
	}
}
