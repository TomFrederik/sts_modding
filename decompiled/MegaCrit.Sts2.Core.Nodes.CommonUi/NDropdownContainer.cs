using System;
using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Helpers;

namespace MegaCrit.Sts2.Core.Nodes.CommonUi;

[ScriptPath("res://src/Core/Nodes/CommonUi/NDropdownContainer.cs")]
public class NDropdownContainer : Control
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName _EnterTree = StringName.op_Implicit("_EnterTree");

		public static readonly StringName _ExitTree = StringName.op_Implicit("_ExitTree");

		public static readonly StringName OnVisibilityChange = StringName.op_Implicit("OnVisibilityChange");

		public static readonly StringName RefreshLayout = StringName.op_Implicit("RefreshLayout");

		public static readonly StringName IsScrollbarNeeded = StringName.op_Implicit("IsScrollbarNeeded");

		public static readonly StringName _Process = StringName.op_Implicit("_Process");

		public static readonly StringName ProcessGuiFocus = StringName.op_Implicit("ProcessGuiFocus");

		public static readonly StringName UpdateScrollPosition = StringName.op_Implicit("UpdateScrollPosition");

		public static readonly StringName UpdateScrollbar = StringName.op_Implicit("UpdateScrollbar");

		public static readonly StringName UpdatePositionBasedOnTrain = StringName.op_Implicit("UpdatePositionBasedOnTrain");

		public static readonly StringName _GuiInput = StringName.op_Implicit("_GuiInput");

		public static readonly StringName ProcessMouseEvent = StringName.op_Implicit("ProcessMouseEvent");

		public static readonly StringName ProcessScrollEvent = StringName.op_Implicit("ProcessScrollEvent");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName _scrollbar = StringName.op_Implicit("_scrollbar");

		public static readonly StringName _scrollbarTrain = StringName.op_Implicit("_scrollbarTrain");

		public static readonly StringName _dropdownItems = StringName.op_Implicit("_dropdownItems");

		public static readonly StringName _maxHeight = StringName.op_Implicit("_maxHeight");

		public static readonly StringName _contentHeight = StringName.op_Implicit("_contentHeight");

		public static readonly StringName _startDragPos = StringName.op_Implicit("_startDragPos");

		public static readonly StringName _targetDragPos = StringName.op_Implicit("_targetDragPos");

		public static readonly StringName _scrollLimitBottom = StringName.op_Implicit("_scrollLimitBottom");

		public static readonly StringName _isDragging = StringName.op_Implicit("_isDragging");
	}

	public class SignalName : SignalName
	{
	}

	private NDropdownScrollbar _scrollbar;

	private Control _scrollbarTrain;

	private VBoxContainer _dropdownItems;

	private float _maxHeight;

	private float _contentHeight;

	private Vector2 _startDragPos;

	private Vector2 _targetDragPos = Vector2.Zero;

	private const float _scrollLimitTop = 0f;

	private float _scrollLimitBottom;

	private bool _isDragging;

	public override void _Ready()
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		_scrollbar = ((Node)this).GetNode<NDropdownScrollbar>(NodePath.op_Implicit("Scrollbar"));
		_scrollbarTrain = ((Node)this).GetNode<Control>(NodePath.op_Implicit("Scrollbar/Train"));
		_dropdownItems = ((Node)this).GetNode<VBoxContainer>(NodePath.op_Implicit("VBoxContainer"));
		((GodotObject)this).Connect(SignalName.VisibilityChanged, Callable.From((Action)OnVisibilityChange), 0u);
		_maxHeight = ((Control)this).Size.Y;
	}

	public override void _EnterTree()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		((Node)this)._EnterTree();
		((GodotObject)((Node)this).GetViewport()).Connect(SignalName.GuiFocusChanged, Callable.From<Control>((Action<Control>)ProcessGuiFocus), 0u);
	}

	public override void _ExitTree()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		((Node)this)._ExitTree();
		((GodotObject)((Node)this).GetViewport()).Disconnect(SignalName.GuiFocusChanged, Callable.From<Control>((Action<Control>)ProcessGuiFocus));
	}

	private void OnVisibilityChange()
	{
		if (((CanvasItem)this).Visible)
		{
			_isDragging = false;
		}
	}

	public void RefreshLayout()
	{
		((CanvasItem)_scrollbar).Visible = IsScrollbarNeeded();
	}

	private bool IsScrollbarNeeded()
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		_contentHeight = 0f;
		foreach (Node child in ((Node)_dropdownItems).GetChildren(false))
		{
			Control val = (Control)(object)((child is Control) ? child : null);
			if (val != null)
			{
				_contentHeight += val.Size.Y;
			}
		}
		_scrollLimitBottom = 0f - _contentHeight + _maxHeight;
		if (_contentHeight > _maxHeight)
		{
			((Control)this).Size = new Vector2(((Control)this).Size.X, _maxHeight);
			_scrollbar.RefreshTrainBounds();
			return true;
		}
		((Control)this).Size = new Vector2(((Control)this).Size.X, _contentHeight);
		return false;
	}

	public override void _Process(double delta)
	{
		if (((CanvasItem)this).IsVisibleInTree())
		{
			UpdateScrollPosition(delta);
			UpdateScrollbar();
		}
	}

	private void ProcessGuiFocus(Control focusedControl)
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		if (((CanvasItem)this).IsVisibleInTree() && ((CanvasItem)_scrollbar).Visible && NControllerManager.Instance.IsUsingController && ((Node)_dropdownItems).IsAncestorOf((Node)(object)focusedControl))
		{
			float num = ((Control)_dropdownItems).GlobalPosition.Y - focusedControl.GlobalPosition.Y;
			float num2 = num + ((Control)this).Size.Y * 0.5f;
			num2 = Mathf.Clamp(num2, _scrollLimitBottom, 0f);
			_targetDragPos = new Vector2(_targetDragPos.X, num2);
		}
	}

	private void UpdateScrollPosition(double delta)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		if (!((CanvasItem)_scrollbar).Visible)
		{
			return;
		}
		if (((Control)_dropdownItems).Position != _targetDragPos)
		{
			VBoxContainer dropdownItems = _dropdownItems;
			Vector2 position = ((Control)_dropdownItems).Position;
			((Control)dropdownItems).Position = ((Vector2)(ref position)).Lerp(_targetDragPos, (float)delta * 15f);
			position = ((Control)_dropdownItems).Position;
			if (((Vector2)(ref position)).DistanceTo(_targetDragPos) < 0.5f)
			{
				((Control)_dropdownItems).Position = _targetDragPos;
			}
		}
		if (!_isDragging)
		{
			if (_targetDragPos.Y < _scrollLimitBottom)
			{
				_targetDragPos = ((Vector2)(ref _targetDragPos)).Lerp(new Vector2(0f, _scrollLimitBottom), (float)delta * 12f);
			}
			else if (_targetDragPos.Y > 0f)
			{
				_targetDragPos = ((Vector2)(ref _targetDragPos)).Lerp(new Vector2(0f, 0f), (float)delta * 12f);
			}
		}
	}

	private void UpdateScrollbar()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		if (((CanvasItem)_scrollbar).Visible && !_scrollbar.hasControl)
		{
			float num = (((Control)_dropdownItems).Position.Y - _scrollLimitBottom) / (0f - _scrollLimitBottom);
			_scrollbar.SetTrainPositionFromPercentage(Mathf.Clamp(1f - num, 0f, 1f));
		}
	}

	public void UpdatePositionBasedOnTrain(float trainPosition)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		_targetDragPos = new Vector2(_targetDragPos.X, _scrollLimitBottom + trainPosition * (0f - _scrollLimitBottom));
	}

	public override void _GuiInput(InputEvent inputEvent)
	{
		ProcessMouseEvent(inputEvent);
		ProcessScrollEvent(inputEvent);
	}

	private void ProcessMouseEvent(InputEvent inputEvent)
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Invalid comparison between Unknown and I8
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		if (_isDragging)
		{
			InputEventMouseMotion val = (InputEventMouseMotion)(object)((inputEvent is InputEventMouseMotion) ? inputEvent : null);
			if (val != null)
			{
				_targetDragPos += new Vector2(0f, val.Relative.Y);
				return;
			}
		}
		InputEventMouseButton val2 = (InputEventMouseButton)(object)((inputEvent is InputEventMouseButton) ? inputEvent : null);
		if (val2 == null)
		{
			return;
		}
		if ((long)val2.ButtonIndex == 1)
		{
			if (val2.Pressed)
			{
				_isDragging = true;
				_startDragPos = ((Control)_dropdownItems).Position;
				_targetDragPos = _startDragPos;
			}
			else
			{
				_isDragging = false;
			}
		}
		else if (!val2.Pressed)
		{
			_isDragging = false;
		}
	}

	private void ProcessScrollEvent(InputEvent inputEvent)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		_targetDragPos += new Vector2(0f, ScrollHelper.GetDragForScrollEvent(inputEvent));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Expected O, but got Unknown
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_0297: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fb: Expected O, but got Unknown
		//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0301: Unknown result type (might be due to invalid IL or missing references)
		//IL_0327: Unknown result type (might be due to invalid IL or missing references)
		//IL_034f: Unknown result type (might be due to invalid IL or missing references)
		//IL_035a: Expected O, but got Unknown
		//IL_0355: Unknown result type (might be due to invalid IL or missing references)
		//IL_0360: Unknown result type (might be due to invalid IL or missing references)
		//IL_0386: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b9: Expected O, but got Unknown
		//IL_03b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bf: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(14);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._EnterTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._ExitTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnVisibilityChange, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.RefreshLayout, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.IsScrollbarNeeded, new PropertyInfo((Type)1, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Process, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)3, StringName.op_Implicit("delta"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ProcessGuiFocus, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("focusedControl"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UpdateScrollPosition, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)3, StringName.op_Implicit("delta"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UpdateScrollbar, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UpdatePositionBasedOnTrain, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)3, StringName.op_Implicit("trainPosition"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._GuiInput, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("inputEvent"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("InputEvent"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ProcessMouseEvent, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("inputEvent"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("InputEvent"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ProcessScrollEvent, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("inputEvent"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("InputEvent"), false)
		}, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_0271: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._EnterTree && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._EnterTree();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._ExitTree && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._ExitTree();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnVisibilityChange && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnVisibilityChange();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.RefreshLayout && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			RefreshLayout();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.IsScrollbarNeeded && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			bool flag = IsScrollbarNeeded();
			ret = VariantUtils.CreateFrom<bool>(ref flag);
			return true;
		}
		if ((ref method) == MethodName._Process && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			((Node)this)._Process(VariantUtils.ConvertTo<double>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.ProcessGuiFocus && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			ProcessGuiFocus(VariantUtils.ConvertTo<Control>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.UpdateScrollPosition && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			UpdateScrollPosition(VariantUtils.ConvertTo<double>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.UpdateScrollbar && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			UpdateScrollbar();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.UpdatePositionBasedOnTrain && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			UpdatePositionBasedOnTrain(VariantUtils.ConvertTo<float>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._GuiInput && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			((Control)this)._GuiInput(VariantUtils.ConvertTo<InputEvent>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.ProcessMouseEvent && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			ProcessMouseEvent(VariantUtils.ConvertTo<InputEvent>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.ProcessScrollEvent && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			ProcessScrollEvent(VariantUtils.ConvertTo<InputEvent>(ref ((NativeVariantPtrArgs)(ref args))[0]));
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
		if ((ref method) == MethodName._EnterTree)
		{
			return true;
		}
		if ((ref method) == MethodName._ExitTree)
		{
			return true;
		}
		if ((ref method) == MethodName.OnVisibilityChange)
		{
			return true;
		}
		if ((ref method) == MethodName.RefreshLayout)
		{
			return true;
		}
		if ((ref method) == MethodName.IsScrollbarNeeded)
		{
			return true;
		}
		if ((ref method) == MethodName._Process)
		{
			return true;
		}
		if ((ref method) == MethodName.ProcessGuiFocus)
		{
			return true;
		}
		if ((ref method) == MethodName.UpdateScrollPosition)
		{
			return true;
		}
		if ((ref method) == MethodName.UpdateScrollbar)
		{
			return true;
		}
		if ((ref method) == MethodName.UpdatePositionBasedOnTrain)
		{
			return true;
		}
		if ((ref method) == MethodName._GuiInput)
		{
			return true;
		}
		if ((ref method) == MethodName.ProcessMouseEvent)
		{
			return true;
		}
		if ((ref method) == MethodName.ProcessScrollEvent)
		{
			return true;
		}
		return ((Control)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName._scrollbar)
		{
			_scrollbar = VariantUtils.ConvertTo<NDropdownScrollbar>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._scrollbarTrain)
		{
			_scrollbarTrain = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._dropdownItems)
		{
			_dropdownItems = VariantUtils.ConvertTo<VBoxContainer>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._maxHeight)
		{
			_maxHeight = VariantUtils.ConvertTo<float>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._contentHeight)
		{
			_contentHeight = VariantUtils.ConvertTo<float>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._startDragPos)
		{
			_startDragPos = VariantUtils.ConvertTo<Vector2>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._targetDragPos)
		{
			_targetDragPos = VariantUtils.ConvertTo<Vector2>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._scrollLimitBottom)
		{
			_scrollLimitBottom = VariantUtils.ConvertTo<float>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._isDragging)
		{
			_isDragging = VariantUtils.ConvertTo<bool>(ref value);
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
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName._scrollbar)
		{
			value = VariantUtils.CreateFrom<NDropdownScrollbar>(ref _scrollbar);
			return true;
		}
		if ((ref name) == PropertyName._scrollbarTrain)
		{
			value = VariantUtils.CreateFrom<Control>(ref _scrollbarTrain);
			return true;
		}
		if ((ref name) == PropertyName._dropdownItems)
		{
			value = VariantUtils.CreateFrom<VBoxContainer>(ref _dropdownItems);
			return true;
		}
		if ((ref name) == PropertyName._maxHeight)
		{
			value = VariantUtils.CreateFrom<float>(ref _maxHeight);
			return true;
		}
		if ((ref name) == PropertyName._contentHeight)
		{
			value = VariantUtils.CreateFrom<float>(ref _contentHeight);
			return true;
		}
		if ((ref name) == PropertyName._startDragPos)
		{
			value = VariantUtils.CreateFrom<Vector2>(ref _startDragPos);
			return true;
		}
		if ((ref name) == PropertyName._targetDragPos)
		{
			value = VariantUtils.CreateFrom<Vector2>(ref _targetDragPos);
			return true;
		}
		if ((ref name) == PropertyName._scrollLimitBottom)
		{
			value = VariantUtils.CreateFrom<float>(ref _scrollLimitBottom);
			return true;
		}
		if ((ref name) == PropertyName._isDragging)
		{
			value = VariantUtils.CreateFrom<bool>(ref _isDragging);
			return true;
		}
		return ((GodotObject)this).GetGodotClassPropertyValue(ref name, ref value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._scrollbar, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._scrollbarTrain, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._dropdownItems, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)3, PropertyName._maxHeight, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)3, PropertyName._contentHeight, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)5, PropertyName._startDragPos, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)5, PropertyName._targetDragPos, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)3, PropertyName._scrollLimitBottom, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._isDragging, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		info.AddProperty(PropertyName._scrollbar, Variant.From<NDropdownScrollbar>(ref _scrollbar));
		info.AddProperty(PropertyName._scrollbarTrain, Variant.From<Control>(ref _scrollbarTrain));
		info.AddProperty(PropertyName._dropdownItems, Variant.From<VBoxContainer>(ref _dropdownItems));
		info.AddProperty(PropertyName._maxHeight, Variant.From<float>(ref _maxHeight));
		info.AddProperty(PropertyName._contentHeight, Variant.From<float>(ref _contentHeight));
		info.AddProperty(PropertyName._startDragPos, Variant.From<Vector2>(ref _startDragPos));
		info.AddProperty(PropertyName._targetDragPos, Variant.From<Vector2>(ref _targetDragPos));
		info.AddProperty(PropertyName._scrollLimitBottom, Variant.From<float>(ref _scrollLimitBottom));
		info.AddProperty(PropertyName._isDragging, Variant.From<bool>(ref _isDragging));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._scrollbar, ref val))
		{
			_scrollbar = ((Variant)(ref val)).As<NDropdownScrollbar>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._scrollbarTrain, ref val2))
		{
			_scrollbarTrain = ((Variant)(ref val2)).As<Control>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._dropdownItems, ref val3))
		{
			_dropdownItems = ((Variant)(ref val3)).As<VBoxContainer>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._maxHeight, ref val4))
		{
			_maxHeight = ((Variant)(ref val4)).As<float>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._contentHeight, ref val5))
		{
			_contentHeight = ((Variant)(ref val5)).As<float>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._startDragPos, ref val6))
		{
			_startDragPos = ((Variant)(ref val6)).As<Vector2>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName._targetDragPos, ref val7))
		{
			_targetDragPos = ((Variant)(ref val7)).As<Vector2>();
		}
		Variant val8 = default(Variant);
		if (info.TryGetProperty(PropertyName._scrollLimitBottom, ref val8))
		{
			_scrollLimitBottom = ((Variant)(ref val8)).As<float>();
		}
		Variant val9 = default(Variant);
		if (info.TryGetProperty(PropertyName._isDragging, ref val9))
		{
			_isDragging = ((Variant)(ref val9)).As<bool>();
		}
	}
}
