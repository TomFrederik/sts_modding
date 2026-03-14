using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;

namespace MegaCrit.Sts2.Core.Nodes.Screens.CharacterSelect;

[ScriptPath("res://src/Core/Nodes/Screens/CharacterSelect/NActDropdown.cs")]
public class NActDropdown : NDropdown
{
	public new class MethodName : NDropdown.MethodName
	{
		public new static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public new static readonly StringName OnFocus = StringName.op_Implicit("OnFocus");

		public new static readonly StringName OnUnfocus = StringName.op_Implicit("OnUnfocus");

		public static readonly StringName PopulateOptions = StringName.op_Implicit("PopulateOptions");

		public static readonly StringName OnDropdownItemSelected = StringName.op_Implicit("OnDropdownItemSelected");

		public static readonly StringName GetDropdownContainer = StringName.op_Implicit("GetDropdownContainer");
	}

	public new class PropertyName : NDropdown.PropertyName
	{
		public static readonly StringName CurrentOption = StringName.op_Implicit("CurrentOption");

		public static readonly StringName _currentOptionIndex = StringName.op_Implicit("_currentOptionIndex");
	}

	public new class SignalName : NDropdown.SignalName
	{
	}

	private static readonly string[] _options = new string[3] { "random", "overgrowth", "underdocks" };

	private int _currentOptionIndex = ListExtensions.IndexOf(_options, "random");

	public string CurrentOption => _options[_currentOptionIndex];

	public override void _Ready()
	{
		ConnectSignals();
		PopulateOptions();
	}

	protected override void OnFocus()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		((CanvasItem)_currentOptionHighlight).Modulate = new Color("afcdde");
	}

	protected override void OnUnfocus()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		((CanvasItem)_currentOptionHighlight).Modulate = Colors.White;
	}

	private void PopulateOptions()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		List<NDropdownItem> list = GetDropdownItems().ToList();
		for (int i = 0; i < _options.Length; i++)
		{
			NDropdownItem nDropdownItem = list[i];
			string text = _options[i];
			((GodotObject)nDropdownItem).Connect(NDropdownItem.SignalName.Selected, Callable.From<NDropdownItem>((Action<NDropdownItem>)OnDropdownItemSelected), 0u);
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(0, 2);
			defaultInterpolatedStringHandler.AppendFormatted(char.ToUpperInvariant(text[0]));
			string text2 = text;
			defaultInterpolatedStringHandler.AppendFormatted(text2.Substring(1, text2.Length - 1));
			nDropdownItem.Text = defaultInterpolatedStringHandler.ToStringAndClear();
		}
		((Node)GetDropdownContainer()).GetParent<NDropdownContainer>().RefreshLayout();
	}

	private void OnDropdownItemSelected(NDropdownItem item)
	{
		CloseDropdown();
		_currentOptionIndex = GetDropdownItems().ToList().IndexOf(item);
		_currentOptionLabel.SetTextAutoSize(item.Text);
	}

	private Control GetDropdownContainer()
	{
		return ((Node)this).GetNode<Control>(NodePath.op_Implicit("DropdownContainer/VBoxContainer"));
	}

	private IEnumerable<NDropdownItem> GetDropdownItems()
	{
		return ((IEnumerable)((Node)GetDropdownContainer()).GetChildren(false)).OfType<NDropdownItem>();
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
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Expected O, but got Unknown
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Expected O, but got Unknown
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(6);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnFocus, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnUnfocus, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.PopulateOptions, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnDropdownItemSelected, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("item"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.GetDropdownContainer, new PropertyInfo((Type)24, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
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
		if ((ref method) == MethodName.PopulateOptions && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			PopulateOptions();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnDropdownItemSelected && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OnDropdownItemSelected(VariantUtils.ConvertTo<NDropdownItem>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.GetDropdownContainer && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			Control dropdownContainer = GetDropdownContainer();
			ret = VariantUtils.CreateFrom<Control>(ref dropdownContainer);
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
		if ((ref method) == MethodName.OnFocus)
		{
			return true;
		}
		if ((ref method) == MethodName.OnUnfocus)
		{
			return true;
		}
		if ((ref method) == MethodName.PopulateOptions)
		{
			return true;
		}
		if ((ref method) == MethodName.OnDropdownItemSelected)
		{
			return true;
		}
		if ((ref method) == MethodName.GetDropdownContainer)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._currentOptionIndex)
		{
			_currentOptionIndex = VariantUtils.ConvertTo<int>(ref value);
			return true;
		}
		return base.SetGodotClassPropertyValue(in name, in value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName.CurrentOption)
		{
			string currentOption = CurrentOption;
			value = VariantUtils.CreateFrom<string>(ref currentOption);
			return true;
		}
		if ((ref name) == PropertyName._currentOptionIndex)
		{
			value = VariantUtils.CreateFrom<int>(ref _currentOptionIndex);
			return true;
		}
		return base.GetGodotClassPropertyValue(in name, out value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)2, PropertyName._currentOptionIndex, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)4, PropertyName.CurrentOption, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName._currentOptionIndex, Variant.From<int>(ref _currentOptionIndex));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._currentOptionIndex, ref val))
		{
			_currentOptionIndex = ((Variant)(ref val)).As<int>();
		}
	}
}
