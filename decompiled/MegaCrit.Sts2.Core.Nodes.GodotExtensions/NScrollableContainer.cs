using System;
using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.ControllerInput;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.CommonUi;

namespace MegaCrit.Sts2.Core.Nodes.GodotExtensions;

[ScriptPath("res://src/Core/Nodes/GodotExtensions/NScrollableContainer.cs")]
public class NScrollableContainer : Control
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName SetContent = StringName.op_Implicit("SetContent");

		public static readonly StringName DisableScrollingIfContentFits = StringName.op_Implicit("DisableScrollingIfContentFits");

		public static readonly StringName _EnterTree = StringName.op_Implicit("_EnterTree");

		public static readonly StringName _ExitTree = StringName.op_Implicit("_ExitTree");

		public static readonly StringName UpdateScrollLimitBottom = StringName.op_Implicit("UpdateScrollLimitBottom");

		public static readonly StringName _GuiInput = StringName.op_Implicit("_GuiInput");

		public static readonly StringName _Input = StringName.op_Implicit("_Input");

		public static readonly StringName ProcessControllerEvent = StringName.op_Implicit("ProcessControllerEvent");

		public static readonly StringName ProcessMouseEvent = StringName.op_Implicit("ProcessMouseEvent");

		public static readonly StringName ProcessScrollEvent = StringName.op_Implicit("ProcessScrollEvent");

		public static readonly StringName _Process = StringName.op_Implicit("_Process");

		public static readonly StringName InstantlyScrollToTop = StringName.op_Implicit("InstantlyScrollToTop");

		public static readonly StringName ProcessGuiFocus = StringName.op_Implicit("ProcessGuiFocus");

		public static readonly StringName UpdateScrollPosition = StringName.op_Implicit("UpdateScrollPosition");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName ScrollViewportTop = StringName.op_Implicit("ScrollViewportTop");

		public static readonly StringName ScrollViewportSize = StringName.op_Implicit("ScrollViewportSize");

		public static readonly StringName ScrollLimitBottom = StringName.op_Implicit("ScrollLimitBottom");

		public static readonly StringName Scrollbar = StringName.op_Implicit("Scrollbar");

		public static readonly StringName _controllerScrollAmount = StringName.op_Implicit("_controllerScrollAmount");

		public static readonly StringName _startDragPosY = StringName.op_Implicit("_startDragPosY");

		public static readonly StringName _targetDragPosY = StringName.op_Implicit("_targetDragPosY");

		public static readonly StringName _isDragging = StringName.op_Implicit("_isDragging");

		public static readonly StringName _paddingTop = StringName.op_Implicit("_paddingTop");

		public static readonly StringName _paddingBottom = StringName.op_Implicit("_paddingBottom");

		public static readonly StringName _content = StringName.op_Implicit("_content");

		public static readonly StringName _scrollbarPressed = StringName.op_Implicit("_scrollbarPressed");

		public static readonly StringName _disableScrollingIfContentFits = StringName.op_Implicit("_disableScrollingIfContentFits");
	}

	public class SignalName : SignalName
	{
	}

	private float _controllerScrollAmount = 400f;

	private float _startDragPosY;

	private float _targetDragPosY;

	private bool _isDragging;

	private float _paddingTop;

	private float _paddingBottom;

	private Control? _content;

	private bool _scrollbarPressed;

	private bool _disableScrollingIfContentFits;

	private float ScrollViewportTop
	{
		get
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			if (_content != null)
			{
				return ((Node)_content).GetParent<Control>().Position.Y;
			}
			return 0f;
		}
	}

	private float ScrollViewportSize
	{
		get
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			if (_content != null)
			{
				return ((Node)_content).GetParent<Control>().Size.Y;
			}
			return 0f;
		}
	}

	private float ScrollLimitBottom
	{
		get
		{
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			if (_content != null)
			{
				return 0f - (_paddingBottom + _paddingTop + _content.Size.Y) + ((Node)_content).GetParent<Control>().Size.Y;
			}
			return 0f;
		}
	}

	public NScrollbar Scrollbar { get; private set; }

	public override void _Ready()
	{
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		_content = ((Node)this).GetNodeOrNull<Control>(NodePath.op_Implicit("Content")) ?? ((Node)this).GetNodeOrNull<Control>(NodePath.op_Implicit("Mask/Content"));
		Scrollbar = ((Node)this).GetNode<NScrollbar>(NodePath.op_Implicit("Scrollbar"));
		SetContent(_content);
		((CanvasItem)Scrollbar).Visible = false;
		((GodotObject)Scrollbar).Connect(NScrollbar.SignalName.MousePressed, Callable.From<InputEvent>((Action<InputEvent>)delegate
		{
			_scrollbarPressed = true;
		}), 0u);
		((GodotObject)Scrollbar).Connect(NScrollbar.SignalName.MouseReleased, Callable.From<InputEvent>((Action<InputEvent>)delegate
		{
			_scrollbarPressed = false;
		}), 0u);
	}

	public void SetContent(Control? content, float paddingTop = 0f, float paddingBottom = 0f)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		Callable val = Callable.From((Action)UpdateScrollLimitBottom);
		if (_content != null && ((GodotObject)_content).IsConnected(SignalName.ItemRectChanged, val))
		{
			((GodotObject)_content).Disconnect(SignalName.ItemRectChanged, val);
		}
		_content = content;
		if (_content != null)
		{
			((GodotObject)_content).Connect(SignalName.ItemRectChanged, Callable.From((Action)UpdateScrollLimitBottom), 0u);
			_paddingTop = paddingTop;
			_paddingBottom = paddingBottom;
			UpdateScrollLimitBottom();
		}
	}

	public void DisableScrollingIfContentFits()
	{
		_disableScrollingIfContentFits = true;
	}

	public override void _EnterTree()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)((Node)this).GetViewport()).Connect(SignalName.GuiFocusChanged, Callable.From<Control>((Action<Control>)ProcessGuiFocus), 0u);
	}

	public override void _ExitTree()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)((Node)this).GetViewport()).Disconnect(SignalName.GuiFocusChanged, Callable.From<Control>((Action<Control>)ProcessGuiFocus));
	}

	private void UpdateScrollLimitBottom()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		if (_content != null)
		{
			((CanvasItem)Scrollbar).Visible = _content.Size.Y + _paddingTop + _paddingBottom > ScrollViewportSize;
			((Control)Scrollbar).MouseFilter = (MouseFilterEnum)(((CanvasItem)Scrollbar).Visible ? 0 : 2);
		}
	}

	public override void _GuiInput(InputEvent inputEvent)
	{
		if (((CanvasItem)this).IsVisibleInTree())
		{
			ProcessMouseEvent(inputEvent);
			ProcessScrollEvent(inputEvent);
		}
	}

	public override void _Input(InputEvent inputEvent)
	{
		if (((CanvasItem)this).IsVisibleInTree())
		{
			Viewport viewport = ((Node)this).GetViewport();
			if (viewport == null || viewport.GuiGetFocusOwner() == null)
			{
				ProcessControllerEvent(inputEvent);
			}
		}
	}

	private void ProcessControllerEvent(InputEvent inputEvent)
	{
		if (inputEvent.IsActionPressed(MegaInput.up, false, false))
		{
			_targetDragPosY += _controllerScrollAmount;
		}
		else if (inputEvent.IsActionPressed(MegaInput.down, false, false))
		{
			_targetDragPosY += 0f - _controllerScrollAmount;
		}
	}

	private void ProcessMouseEvent(InputEvent inputEvent)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Invalid comparison between Unknown and I8
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		if (_content == null)
		{
			return;
		}
		InputEventMouseMotion val = (InputEventMouseMotion)(object)((inputEvent is InputEventMouseMotion) ? inputEvent : null);
		if (val == null)
		{
			InputEventMouseButton val2 = (InputEventMouseButton)(object)((inputEvent is InputEventMouseButton) ? inputEvent : null);
			if (val2 == null)
			{
				return;
			}
			if ((long)val2.ButtonIndex == 1)
			{
				_isDragging = val2.Pressed;
				if (val2.Pressed)
				{
					_startDragPosY = _content.Position.Y - _paddingTop;
					_targetDragPosY = _startDragPosY;
				}
			}
			else if (!val2.Pressed)
			{
				_isDragging = false;
			}
		}
		else if (_isDragging)
		{
			_targetDragPosY += val.Relative.Y;
		}
	}

	private void ProcessScrollEvent(InputEvent inputEvent)
	{
		_targetDragPosY += ScrollHelper.GetDragForScrollEvent(inputEvent);
	}

	public override void _Process(double delta)
	{
		if (((CanvasItem)this).IsVisibleInTree() && (!_disableScrollingIfContentFits || ((CanvasItem)Scrollbar).Visible))
		{
			UpdateScrollPosition(delta);
		}
	}

	public void InstantlyScrollToTop()
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		if (_content == null)
		{
			throw new InvalidOperationException("No content to scroll!");
		}
		_targetDragPosY = 0f;
		Control? content = _content;
		Vector2 position = _content.Position;
		position.Y = _paddingTop;
		content.Position = position;
		Scrollbar.SetValueWithoutAnimation(0.0);
	}

	private void ProcessGuiFocus(Control focusedControl)
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		if (_content != null && ((CanvasItem)this).IsVisibleInTree() && NControllerManager.Instance.IsUsingController && ((Node)_content).IsAncestorOf((Node)(object)focusedControl))
		{
			float num = _content.GlobalPosition.Y - focusedControl.GlobalPosition.Y;
			float num2 = num + ScrollViewportSize * 0.5f;
			float num3 = Mathf.Max(ScrollLimitBottom, 0f);
			float num4 = Mathf.Min(ScrollLimitBottom, 0f);
			num2 = Mathf.Clamp(num2, num4, num3);
			_targetDragPosY = num2;
		}
	}

	private void UpdateScrollPosition(double delta)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		if (_content == null)
		{
			return;
		}
		float num = _paddingTop + _targetDragPosY;
		if (!Mathf.IsEqualApprox(_content.Position.Y, num))
		{
			float y = Mathf.Lerp(_content.Position.Y, num, (float)delta * 15f);
			Control? content = _content;
			Vector2 position = _content.Position;
			position.Y = y;
			content.Position = position;
			if (Mathf.Abs(_content.Position.Y - num) < 0.5f)
			{
				Control? content2 = _content;
				position = _content.Position;
				position.Y = num;
				content2.Position = position;
			}
			if (!_scrollbarPressed && ScrollLimitBottom < 0f)
			{
				Scrollbar.SetValueWithoutAnimation(Mathf.Clamp((_content.Position.Y - _paddingTop) / ScrollLimitBottom, 0f, 1f) * 100f);
			}
		}
		if (_scrollbarPressed)
		{
			_targetDragPosY = Mathf.Lerp(0f, ScrollLimitBottom, (float)((Range)Scrollbar).Value * 0.01f);
		}
		if (!_isDragging)
		{
			if (_targetDragPosY < Mathf.Min(ScrollLimitBottom, 0f))
			{
				_targetDragPosY = Mathf.Lerp(_targetDragPosY, ScrollLimitBottom, (float)delta * 12f);
			}
			else if (_targetDragPosY > Mathf.Max(ScrollLimitBottom, 0f))
			{
				_targetDragPosY = Mathf.Lerp(_targetDragPosY, 0f, (float)delta * 12f);
			}
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Expected O, but got Unknown
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Expected O, but got Unknown
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Expected O, but got Unknown
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0296: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a1: Expected O, but got Unknown
		//IL_029c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0300: Expected O, but got Unknown
		//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0306: Unknown result type (might be due to invalid IL or missing references)
		//IL_032c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0354: Unknown result type (might be due to invalid IL or missing references)
		//IL_035f: Expected O, but got Unknown
		//IL_035a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0365: Unknown result type (might be due to invalid IL or missing references)
		//IL_038b: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03df: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_040e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0436: Unknown result type (might be due to invalid IL or missing references)
		//IL_0441: Expected O, but got Unknown
		//IL_043c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0447: Unknown result type (might be due to invalid IL or missing references)
		//IL_046d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0490: Unknown result type (might be due to invalid IL or missing references)
		//IL_049b: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(15);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetContent, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("content"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false),
			new PropertyInfo((Type)3, StringName.op_Implicit("paddingTop"), (PropertyHint)0, "", (PropertyUsageFlags)6, false),
			new PropertyInfo((Type)3, StringName.op_Implicit("paddingBottom"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.DisableScrollingIfContentFits, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._EnterTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._ExitTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UpdateScrollLimitBottom, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._GuiInput, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("inputEvent"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("InputEvent"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Input, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("inputEvent"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("InputEvent"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ProcessControllerEvent, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
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
		list.Add(new MethodInfo(MethodName._Process, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)3, StringName.op_Implicit("delta"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.InstantlyScrollToTop, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ProcessGuiFocus, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("focusedControl"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UpdateScrollPosition, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)3, StringName.op_Implicit("delta"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0288: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetContent && ((NativeVariantPtrArgs)(ref args)).Count == 3)
		{
			SetContent(VariantUtils.ConvertTo<Control>(ref ((NativeVariantPtrArgs)(ref args))[0]), VariantUtils.ConvertTo<float>(ref ((NativeVariantPtrArgs)(ref args))[1]), VariantUtils.ConvertTo<float>(ref ((NativeVariantPtrArgs)(ref args))[2]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.DisableScrollingIfContentFits && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			DisableScrollingIfContentFits();
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
		if ((ref method) == MethodName.UpdateScrollLimitBottom && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			UpdateScrollLimitBottom();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._GuiInput && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			((Control)this)._GuiInput(VariantUtils.ConvertTo<InputEvent>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._Input && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			((Node)this)._Input(VariantUtils.ConvertTo<InputEvent>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.ProcessControllerEvent && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			ProcessControllerEvent(VariantUtils.ConvertTo<InputEvent>(ref ((NativeVariantPtrArgs)(ref args))[0]));
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
		if ((ref method) == MethodName._Process && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			((Node)this)._Process(VariantUtils.ConvertTo<double>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.InstantlyScrollToTop && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			InstantlyScrollToTop();
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
		return ((Control)this).InvokeGodotClassMethod(ref method, args, ref ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if ((ref method) == MethodName._Ready)
		{
			return true;
		}
		if ((ref method) == MethodName.SetContent)
		{
			return true;
		}
		if ((ref method) == MethodName.DisableScrollingIfContentFits)
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
		if ((ref method) == MethodName.UpdateScrollLimitBottom)
		{
			return true;
		}
		if ((ref method) == MethodName._GuiInput)
		{
			return true;
		}
		if ((ref method) == MethodName._Input)
		{
			return true;
		}
		if ((ref method) == MethodName.ProcessControllerEvent)
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
		if ((ref method) == MethodName._Process)
		{
			return true;
		}
		if ((ref method) == MethodName.InstantlyScrollToTop)
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
		return ((Control)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName.Scrollbar)
		{
			Scrollbar = VariantUtils.ConvertTo<NScrollbar>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._controllerScrollAmount)
		{
			_controllerScrollAmount = VariantUtils.ConvertTo<float>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._startDragPosY)
		{
			_startDragPosY = VariantUtils.ConvertTo<float>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._targetDragPosY)
		{
			_targetDragPosY = VariantUtils.ConvertTo<float>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._isDragging)
		{
			_isDragging = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._paddingTop)
		{
			_paddingTop = VariantUtils.ConvertTo<float>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._paddingBottom)
		{
			_paddingBottom = VariantUtils.ConvertTo<float>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._content)
		{
			_content = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._scrollbarPressed)
		{
			_scrollbarPressed = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._disableScrollingIfContentFits)
		{
			_disableScrollingIfContentFits = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		return ((GodotObject)this).SetGodotClassPropertyValue(ref name, ref value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName.ScrollViewportTop)
		{
			float scrollViewportTop = ScrollViewportTop;
			value = VariantUtils.CreateFrom<float>(ref scrollViewportTop);
			return true;
		}
		if ((ref name) == PropertyName.ScrollViewportSize)
		{
			float scrollViewportTop = ScrollViewportSize;
			value = VariantUtils.CreateFrom<float>(ref scrollViewportTop);
			return true;
		}
		if ((ref name) == PropertyName.ScrollLimitBottom)
		{
			float scrollViewportTop = ScrollLimitBottom;
			value = VariantUtils.CreateFrom<float>(ref scrollViewportTop);
			return true;
		}
		if ((ref name) == PropertyName.Scrollbar)
		{
			NScrollbar scrollbar = Scrollbar;
			value = VariantUtils.CreateFrom<NScrollbar>(ref scrollbar);
			return true;
		}
		if ((ref name) == PropertyName._controllerScrollAmount)
		{
			value = VariantUtils.CreateFrom<float>(ref _controllerScrollAmount);
			return true;
		}
		if ((ref name) == PropertyName._startDragPosY)
		{
			value = VariantUtils.CreateFrom<float>(ref _startDragPosY);
			return true;
		}
		if ((ref name) == PropertyName._targetDragPosY)
		{
			value = VariantUtils.CreateFrom<float>(ref _targetDragPosY);
			return true;
		}
		if ((ref name) == PropertyName._isDragging)
		{
			value = VariantUtils.CreateFrom<bool>(ref _isDragging);
			return true;
		}
		if ((ref name) == PropertyName._paddingTop)
		{
			value = VariantUtils.CreateFrom<float>(ref _paddingTop);
			return true;
		}
		if ((ref name) == PropertyName._paddingBottom)
		{
			value = VariantUtils.CreateFrom<float>(ref _paddingBottom);
			return true;
		}
		if ((ref name) == PropertyName._content)
		{
			value = VariantUtils.CreateFrom<Control>(ref _content);
			return true;
		}
		if ((ref name) == PropertyName._scrollbarPressed)
		{
			value = VariantUtils.CreateFrom<bool>(ref _scrollbarPressed);
			return true;
		}
		if ((ref name) == PropertyName._disableScrollingIfContentFits)
		{
			value = VariantUtils.CreateFrom<bool>(ref _disableScrollingIfContentFits);
			return true;
		}
		return ((GodotObject)this).GetGodotClassPropertyValue(ref name, ref value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)3, PropertyName._controllerScrollAmount, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)3, PropertyName._startDragPosY, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)3, PropertyName._targetDragPosY, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._isDragging, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)3, PropertyName.ScrollViewportTop, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)3, PropertyName.ScrollViewportSize, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)3, PropertyName.ScrollLimitBottom, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)3, PropertyName._paddingTop, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)3, PropertyName._paddingBottom, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._content, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._scrollbarPressed, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._disableScrollingIfContentFits, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.Scrollbar, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		StringName scrollbar = PropertyName.Scrollbar;
		NScrollbar scrollbar2 = Scrollbar;
		info.AddProperty(scrollbar, Variant.From<NScrollbar>(ref scrollbar2));
		info.AddProperty(PropertyName._controllerScrollAmount, Variant.From<float>(ref _controllerScrollAmount));
		info.AddProperty(PropertyName._startDragPosY, Variant.From<float>(ref _startDragPosY));
		info.AddProperty(PropertyName._targetDragPosY, Variant.From<float>(ref _targetDragPosY));
		info.AddProperty(PropertyName._isDragging, Variant.From<bool>(ref _isDragging));
		info.AddProperty(PropertyName._paddingTop, Variant.From<float>(ref _paddingTop));
		info.AddProperty(PropertyName._paddingBottom, Variant.From<float>(ref _paddingBottom));
		info.AddProperty(PropertyName._content, Variant.From<Control>(ref _content));
		info.AddProperty(PropertyName._scrollbarPressed, Variant.From<bool>(ref _scrollbarPressed));
		info.AddProperty(PropertyName._disableScrollingIfContentFits, Variant.From<bool>(ref _disableScrollingIfContentFits));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName.Scrollbar, ref val))
		{
			Scrollbar = ((Variant)(ref val)).As<NScrollbar>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._controllerScrollAmount, ref val2))
		{
			_controllerScrollAmount = ((Variant)(ref val2)).As<float>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._startDragPosY, ref val3))
		{
			_startDragPosY = ((Variant)(ref val3)).As<float>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._targetDragPosY, ref val4))
		{
			_targetDragPosY = ((Variant)(ref val4)).As<float>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._isDragging, ref val5))
		{
			_isDragging = ((Variant)(ref val5)).As<bool>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._paddingTop, ref val6))
		{
			_paddingTop = ((Variant)(ref val6)).As<float>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName._paddingBottom, ref val7))
		{
			_paddingBottom = ((Variant)(ref val7)).As<float>();
		}
		Variant val8 = default(Variant);
		if (info.TryGetProperty(PropertyName._content, ref val8))
		{
			_content = ((Variant)(ref val8)).As<Control>();
		}
		Variant val9 = default(Variant);
		if (info.TryGetProperty(PropertyName._scrollbarPressed, ref val9))
		{
			_scrollbarPressed = ((Variant)(ref val9)).As<bool>();
		}
		Variant val10 = default(Variant);
		if (info.TryGetProperty(PropertyName._disableScrollingIfContentFits, ref val10))
		{
			_disableScrollingIfContentFits = ((Variant)(ref val10)).As<bool>();
		}
	}
}
