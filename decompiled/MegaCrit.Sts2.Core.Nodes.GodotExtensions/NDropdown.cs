using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.ControllerInput;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.Debug;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.Nodes.GodotExtensions;

[ScriptPath("res://src/Core/Nodes/GodotExtensions/NDropdown.cs")]
public class NDropdown : NClickableControl
{
	public new class MethodName : NClickableControl.MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public new static readonly StringName ConnectSignals = StringName.op_Implicit("ConnectSignals");

		public static readonly StringName ClearDropdownItems = StringName.op_Implicit("ClearDropdownItems");

		public static readonly StringName _Input = StringName.op_Implicit("_Input");

		public static readonly StringName OnDismisserClicked = StringName.op_Implicit("OnDismisserClicked");

		public new static readonly StringName OnRelease = StringName.op_Implicit("OnRelease");

		public static readonly StringName OpenDropdown = StringName.op_Implicit("OpenDropdown");

		public static readonly StringName CloseDropdown = StringName.op_Implicit("CloseDropdown");
	}

	public new class PropertyName : NClickableControl.PropertyName
	{
		public static readonly StringName _dropdownContainer = StringName.op_Implicit("_dropdownContainer");

		public static readonly StringName _dropdownItems = StringName.op_Implicit("_dropdownItems");

		public static readonly StringName _dismisser = StringName.op_Implicit("_dismisser");

		public static readonly StringName _currentOptionLabel = StringName.op_Implicit("_currentOptionLabel");

		public static readonly StringName _currentOptionHighlight = StringName.op_Implicit("_currentOptionHighlight");

		public new static readonly StringName _isHovered = StringName.op_Implicit("_isHovered");

		public static readonly StringName _isOpen = StringName.op_Implicit("_isOpen");
	}

	public new class SignalName : NClickableControl.SignalName
	{
	}

	private Control _dropdownContainer;

	protected Control _dropdownItems;

	private NButton _dismisser;

	protected MegaLabel _currentOptionLabel;

	protected Control _currentOptionHighlight;

	private bool _isHovered;

	private bool _isOpen;

	public override void _Ready()
	{
		if (((object)this).GetType() != typeof(NDropdown))
		{
			throw new InvalidOperationException("Don't call base._Ready(). Use ConnectSignals() instead");
		}
		ConnectSignals();
	}

	protected override void ConnectSignals()
	{
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		base.ConnectSignals();
		_currentOptionHighlight = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%Highlight"));
		_currentOptionLabel = ((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%Label"));
		_dropdownContainer = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%DropdownContainer"));
		_dropdownItems = ((Node)_dropdownContainer).GetNode<Control>(NodePath.op_Implicit("VBoxContainer"));
		_dismisser = ((Node)this).GetNode<NButton>(NodePath.op_Implicit("%Dismisser"));
		((GodotObject)_dismisser).Connect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)OnDismisserClicked), 0u);
	}

	protected void ClearDropdownItems()
	{
		foreach (Node child in ((Node)_dropdownItems).GetChildren(false))
		{
			((Node)(object)_dropdownItems).RemoveChildSafely(child);
			child.QueueFreeSafely();
		}
	}

	public override void _Input(InputEvent inputEvent)
	{
		if (((CanvasItem)this).IsVisibleInTree() && _isEnabled && !((CanvasItem)NDevConsole.Instance).Visible)
		{
			Control val = ((Node)this).GetViewport().GuiGetFocusOwner();
			bool flag = ((val is TextEdit || val is LineEdit) ? true : false);
			if (!flag && inputEvent.IsActionPressed(MegaInput.cancel, false, false))
			{
				CloseDropdown();
			}
		}
	}

	private void OnDismisserClicked(NButton obj)
	{
		CloseDropdown();
	}

	protected override void OnRelease()
	{
		base.OnRelease();
		if (_isOpen)
		{
			Log.Info("Closing dropdown because you clicked on the main dropdown button.");
			CloseDropdown();
		}
		else
		{
			OpenDropdown();
		}
	}

	private void OpenDropdown()
	{
		((CanvasItem)_dropdownContainer).Visible = true;
		((CanvasItem)_dismisser).Visible = true;
		_isOpen = true;
		((Node)this).GetParent().MoveChild((Node)(object)this, ((Node)this).GetParent().GetChildCount(false));
		List<NDropdownItem> list = ((IEnumerable)((Node)_dropdownItems).GetChildren(false)).OfType<NDropdownItem>().ToList();
		for (int i = 0; i < list.Count; i++)
		{
			list[i].UnhoverSelection();
			((Control)list[i]).FocusNeighborLeft = ((Node)list[i]).GetPath();
			((Control)list[i]).FocusNeighborRight = ((Node)list[i]).GetPath();
			((Control)list[i]).FocusNeighborTop = ((i > 0) ? ((Node)list[i - 1]).GetPath() : ((Node)list[i]).GetPath());
			((Control)list[i]).FocusNeighborBottom = ((i < list.Count - 1) ? ((Node)list[i + 1]).GetPath() : ((Node)list[i]).GetPath());
			((Control)list[i]).FocusMode = (FocusModeEnum)2;
		}
		((Control)(object)list.FirstOrDefault())?.TryGrabFocus();
	}

	protected void CloseDropdown()
	{
		((CanvasItem)_dismisser).Visible = false;
		((CanvasItem)_dropdownContainer).Visible = false;
		_isOpen = false;
		((Control)(object)this).TryGrabFocus();
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Expected O, but got Unknown
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Expected O, but got Unknown
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(8);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ConnectSignals, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ClearDropdownItems, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Input, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("inputEvent"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("InputEvent"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnDismisserClicked, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("obj"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnRelease, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OpenDropdown, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.CloseDropdown, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.ConnectSignals && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			ConnectSignals();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.ClearDropdownItems && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			ClearDropdownItems();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._Input && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			((Node)this)._Input(VariantUtils.ConvertTo<InputEvent>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnDismisserClicked && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OnDismisserClicked(VariantUtils.ConvertTo<NButton>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnRelease && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnRelease();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OpenDropdown && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OpenDropdown();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.CloseDropdown && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			CloseDropdown();
			ret = default(godot_variant);
			return true;
		}
		return base.InvokeGodotClassMethod(in method, args, out ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if ((ref method) == MethodName._Ready)
		{
			return true;
		}
		if ((ref method) == MethodName.ConnectSignals)
		{
			return true;
		}
		if ((ref method) == MethodName.ClearDropdownItems)
		{
			return true;
		}
		if ((ref method) == MethodName._Input)
		{
			return true;
		}
		if ((ref method) == MethodName.OnDismisserClicked)
		{
			return true;
		}
		if ((ref method) == MethodName.OnRelease)
		{
			return true;
		}
		if ((ref method) == MethodName.OpenDropdown)
		{
			return true;
		}
		if ((ref method) == MethodName.CloseDropdown)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._dropdownContainer)
		{
			_dropdownContainer = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._dropdownItems)
		{
			_dropdownItems = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._dismisser)
		{
			_dismisser = VariantUtils.ConvertTo<NButton>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._currentOptionLabel)
		{
			_currentOptionLabel = VariantUtils.ConvertTo<MegaLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._currentOptionHighlight)
		{
			_currentOptionHighlight = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._isHovered)
		{
			_isHovered = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._isOpen)
		{
			_isOpen = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		return base.SetGodotClassPropertyValue(in name, in value);
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
		if ((ref name) == PropertyName._dropdownContainer)
		{
			value = VariantUtils.CreateFrom<Control>(ref _dropdownContainer);
			return true;
		}
		if ((ref name) == PropertyName._dropdownItems)
		{
			value = VariantUtils.CreateFrom<Control>(ref _dropdownItems);
			return true;
		}
		if ((ref name) == PropertyName._dismisser)
		{
			value = VariantUtils.CreateFrom<NButton>(ref _dismisser);
			return true;
		}
		if ((ref name) == PropertyName._currentOptionLabel)
		{
			value = VariantUtils.CreateFrom<MegaLabel>(ref _currentOptionLabel);
			return true;
		}
		if ((ref name) == PropertyName._currentOptionHighlight)
		{
			value = VariantUtils.CreateFrom<Control>(ref _currentOptionHighlight);
			return true;
		}
		if ((ref name) == PropertyName._isHovered)
		{
			value = VariantUtils.CreateFrom<bool>(ref _isHovered);
			return true;
		}
		if ((ref name) == PropertyName._isOpen)
		{
			value = VariantUtils.CreateFrom<bool>(ref _isOpen);
			return true;
		}
		return base.GetGodotClassPropertyValue(in name, out value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._dropdownContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._dropdownItems, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._dismisser, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._currentOptionLabel, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._currentOptionHighlight, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._isHovered, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._isOpen, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName._dropdownContainer, Variant.From<Control>(ref _dropdownContainer));
		info.AddProperty(PropertyName._dropdownItems, Variant.From<Control>(ref _dropdownItems));
		info.AddProperty(PropertyName._dismisser, Variant.From<NButton>(ref _dismisser));
		info.AddProperty(PropertyName._currentOptionLabel, Variant.From<MegaLabel>(ref _currentOptionLabel));
		info.AddProperty(PropertyName._currentOptionHighlight, Variant.From<Control>(ref _currentOptionHighlight));
		info.AddProperty(PropertyName._isHovered, Variant.From<bool>(ref _isHovered));
		info.AddProperty(PropertyName._isOpen, Variant.From<bool>(ref _isOpen));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._dropdownContainer, ref val))
		{
			_dropdownContainer = ((Variant)(ref val)).As<Control>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._dropdownItems, ref val2))
		{
			_dropdownItems = ((Variant)(ref val2)).As<Control>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._dismisser, ref val3))
		{
			_dismisser = ((Variant)(ref val3)).As<NButton>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._currentOptionLabel, ref val4))
		{
			_currentOptionLabel = ((Variant)(ref val4)).As<MegaLabel>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._currentOptionHighlight, ref val5))
		{
			_currentOptionHighlight = ((Variant)(ref val5)).As<Control>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._isHovered, ref val6))
		{
			_isHovered = ((Variant)(ref val6)).As<bool>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName._isOpen, ref val7))
		{
			_isOpen = ((Variant)(ref val7)).As<bool>();
		}
	}
}
