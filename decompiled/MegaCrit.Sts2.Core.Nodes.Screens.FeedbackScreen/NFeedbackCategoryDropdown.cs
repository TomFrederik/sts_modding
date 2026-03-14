using System;
using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.Nodes.Screens.FeedbackScreen;

[ScriptPath("res://src/Core/Nodes/Screens/FeedbackScreen/NFeedbackCategoryDropdown.cs")]
public class NFeedbackCategoryDropdown : NDropdown
{
	public new class MethodName : NDropdown.MethodName
	{
		public new static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public new static readonly StringName OnFocus = StringName.op_Implicit("OnFocus");

		public new static readonly StringName OnUnfocus = StringName.op_Implicit("OnUnfocus");

		public static readonly StringName PopulateOptions = StringName.op_Implicit("PopulateOptions");

		public static readonly StringName OnDropdownItemSelected = StringName.op_Implicit("OnDropdownItemSelected");
	}

	public new class PropertyName : NDropdown.PropertyName
	{
		public static readonly StringName CurrentCategory = StringName.op_Implicit("CurrentCategory");

		public static readonly StringName _dropdownItemScene = StringName.op_Implicit("_dropdownItemScene");

		public static readonly StringName _selectionReticle = StringName.op_Implicit("_selectionReticle");

		public static readonly StringName _currentCategoryIndex = StringName.op_Implicit("_currentCategoryIndex");
	}

	public new class SignalName : NDropdown.SignalName
	{
	}

	[Export(/*Could not decode attribute arguments.*/)]
	private PackedScene _dropdownItemScene;

	private NSelectionReticle _selectionReticle;

	private static readonly string[] _categories = new string[3] { "bug", "balance", "feedback" };

	private static readonly LocString[] _categoryLoc = new LocString[3]
	{
		new LocString("settings_ui", "FEEDBACK_CATEGORY.bug"),
		new LocString("settings_ui", "FEEDBACK_CATEGORY.balance"),
		new LocString("settings_ui", "FEEDBACK_CATEGORY.feedback")
	};

	private int _currentCategoryIndex = ListExtensions.IndexOf(_categories, "feedback");

	public string CurrentCategory => _categories[_currentCategoryIndex];

	public override void _Ready()
	{
		ConnectSignals();
		_currentOptionHighlight = (Control)(object)((Node)this).GetNode<Panel>(NodePath.op_Implicit("%Highlight"));
		_currentOptionLabel = ((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%Label"));
		PopulateOptions();
		_currentOptionLabel.SetTextAutoSize(_categoryLoc[_currentCategoryIndex].GetFormattedText());
		_selectionReticle = ((Node)this).GetNode<NSelectionReticle>(NodePath.op_Implicit("SelectionReticle"));
	}

	protected override void OnFocus()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		((CanvasItem)_currentOptionHighlight).Modulate = new Color("afcdde");
		if (NControllerManager.Instance.IsUsingController)
		{
			_selectionReticle.OnSelect();
		}
	}

	protected override void OnUnfocus()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		_selectionReticle.OnDeselect();
		((CanvasItem)_currentOptionHighlight).Modulate = Colors.White;
	}

	private void PopulateOptions()
	{
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		Control node = ((Node)this).GetNode<Control>(NodePath.op_Implicit("DropdownContainer/VBoxContainer"));
		foreach (Node child in ((Node)node).GetChildren(false))
		{
			((Node)(object)node).RemoveChildSafely(child);
			child.QueueFreeSafely();
		}
		for (int i = 0; i < _categories.Length; i++)
		{
			NFeedbackCategoryDropdownItem nFeedbackCategoryDropdownItem = _dropdownItemScene.Instantiate<NFeedbackCategoryDropdownItem>((GenEditState)0);
			((Node)(object)node).AddChildSafely((Node?)(object)nFeedbackCategoryDropdownItem);
			((GodotObject)nFeedbackCategoryDropdownItem).Connect(NDropdownItem.SignalName.Selected, Callable.From<NDropdownItem>((Action<NDropdownItem>)OnDropdownItemSelected), 0u);
			nFeedbackCategoryDropdownItem.Init(i, _categoryLoc[i].GetFormattedText());
		}
		((Node)node).GetParent<NDropdownContainer>().RefreshLayout();
	}

	private void OnDropdownItemSelected(NDropdownItem item)
	{
		NFeedbackCategoryDropdownItem nFeedbackCategoryDropdownItem = (NFeedbackCategoryDropdownItem)item;
		if (nFeedbackCategoryDropdownItem.CategoryIndex != _currentCategoryIndex)
		{
			CloseDropdown();
			_currentCategoryIndex = nFeedbackCategoryDropdownItem.CategoryIndex;
			_currentOptionLabel.SetTextAutoSize(_categoryLoc[_currentCategoryIndex].GetFormattedText());
		}
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
		List<MethodInfo> list = new List<MethodInfo>(5);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnFocus, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnUnfocus, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.PopulateOptions, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnDropdownItemSelected, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("item"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
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
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._dropdownItemScene)
		{
			_dropdownItemScene = VariantUtils.ConvertTo<PackedScene>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._selectionReticle)
		{
			_selectionReticle = VariantUtils.ConvertTo<NSelectionReticle>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._currentCategoryIndex)
		{
			_currentCategoryIndex = VariantUtils.ConvertTo<int>(ref value);
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
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName.CurrentCategory)
		{
			string currentCategory = CurrentCategory;
			value = VariantUtils.CreateFrom<string>(ref currentCategory);
			return true;
		}
		if ((ref name) == PropertyName._dropdownItemScene)
		{
			value = VariantUtils.CreateFrom<PackedScene>(ref _dropdownItemScene);
			return true;
		}
		if ((ref name) == PropertyName._selectionReticle)
		{
			value = VariantUtils.CreateFrom<NSelectionReticle>(ref _selectionReticle);
			return true;
		}
		if ((ref name) == PropertyName._currentCategoryIndex)
		{
			value = VariantUtils.CreateFrom<int>(ref _currentCategoryIndex);
			return true;
		}
		return base.GetGodotClassPropertyValue(in name, out value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._dropdownItemScene, (PropertyHint)17, "PackedScene", (PropertyUsageFlags)4102, true));
		list.Add(new PropertyInfo((Type)24, PropertyName._selectionReticle, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)2, PropertyName._currentCategoryIndex, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)4, PropertyName.CurrentCategory, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName._dropdownItemScene, Variant.From<PackedScene>(ref _dropdownItemScene));
		info.AddProperty(PropertyName._selectionReticle, Variant.From<NSelectionReticle>(ref _selectionReticle));
		info.AddProperty(PropertyName._currentCategoryIndex, Variant.From<int>(ref _currentCategoryIndex));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._dropdownItemScene, ref val))
		{
			_dropdownItemScene = ((Variant)(ref val)).As<PackedScene>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._selectionReticle, ref val2))
		{
			_selectionReticle = ((Variant)(ref val2)).As<NSelectionReticle>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._currentCategoryIndex, ref val3))
		{
			_currentCategoryIndex = ((Variant)(ref val3)).As<int>();
		}
	}
}
