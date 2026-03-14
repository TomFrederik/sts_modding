using System;
using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Runs;

namespace MegaCrit.Sts2.Core.Nodes.Reaction;

[ScriptPath("res://src/Core/Nodes/Reaction/NReactionWheel.cs")]
public class NReactionWheel : Control
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName _EnterTree = StringName.op_Implicit("_EnterTree");

		public static readonly StringName _ExitTree = StringName.op_Implicit("_ExitTree");

		public static readonly StringName _Notification = StringName.op_Implicit("_Notification");

		public static readonly StringName _Input = StringName.op_Implicit("_Input");

		public static readonly StringName HideWheel = StringName.op_Implicit("HideWheel");

		public static readonly StringName WarpMouseBackToOriginalPosition = StringName.op_Implicit("WarpMouseBackToOriginalPosition");

		public static readonly StringName React = StringName.op_Implicit("React");

		public static readonly StringName MoveMarker = StringName.op_Implicit("MoveMarker");

		public static readonly StringName GetSelectedWedge = StringName.op_Implicit("GetSelectedWedge");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName _rightWedge = StringName.op_Implicit("_rightWedge");

		public static readonly StringName _downRightWedge = StringName.op_Implicit("_downRightWedge");

		public static readonly StringName _downWedge = StringName.op_Implicit("_downWedge");

		public static readonly StringName _downLeftWedge = StringName.op_Implicit("_downLeftWedge");

		public static readonly StringName _leftWedge = StringName.op_Implicit("_leftWedge");

		public static readonly StringName _upLeftWedge = StringName.op_Implicit("_upLeftWedge");

		public static readonly StringName _upWedge = StringName.op_Implicit("_upWedge");

		public static readonly StringName _upRightWedge = StringName.op_Implicit("_upRightWedge");

		public static readonly StringName _marker = StringName.op_Implicit("_marker");

		public static readonly StringName _ignoreNextMouseInput = StringName.op_Implicit("_ignoreNextMouseInput");

		public static readonly StringName _centerPosition = StringName.op_Implicit("_centerPosition");

		public static readonly StringName _selectedWedge = StringName.op_Implicit("_selectedWedge");
	}

	public class SignalName : SignalName
	{
	}

	private static readonly StringName _reactWheel = new StringName("react_wheel");

	private const float _centerRadius = 70f;

	private NReactionWheelWedge _rightWedge;

	private NReactionWheelWedge _downRightWedge;

	private NReactionWheelWedge _downWedge;

	private NReactionWheelWedge _downLeftWedge;

	private NReactionWheelWedge _leftWedge;

	private NReactionWheelWedge _upLeftWedge;

	private NReactionWheelWedge _upWedge;

	private NReactionWheelWedge _upRightWedge;

	private TextureRect _marker;

	private bool _ignoreNextMouseInput;

	private Vector2 _centerPosition;

	private NReactionWheelWedge? _selectedWedge;

	private Player? _localPlayer;

	public override void _Ready()
	{
		_rightWedge = ((Node)this).GetNode<NReactionWheelWedge>(NodePath.op_Implicit("RightWedge"));
		_downRightWedge = ((Node)this).GetNode<NReactionWheelWedge>(NodePath.op_Implicit("DownRightWedge"));
		_downWedge = ((Node)this).GetNode<NReactionWheelWedge>(NodePath.op_Implicit("DownWedge"));
		_downLeftWedge = ((Node)this).GetNode<NReactionWheelWedge>(NodePath.op_Implicit("DownLeftWedge"));
		_leftWedge = ((Node)this).GetNode<NReactionWheelWedge>(NodePath.op_Implicit("LeftWedge"));
		_upLeftWedge = ((Node)this).GetNode<NReactionWheelWedge>(NodePath.op_Implicit("UpLeftWedge"));
		_upWedge = ((Node)this).GetNode<NReactionWheelWedge>(NodePath.op_Implicit("UpWedge"));
		_upRightWedge = ((Node)this).GetNode<NReactionWheelWedge>(NodePath.op_Implicit("UpRightWedge"));
		_marker = ((Node)this).GetNode<TextureRect>(NodePath.op_Implicit("Marker"));
		((CanvasItem)this).Visible = false;
	}

	public override void _EnterTree()
	{
		RunManager.Instance.RunStarted += OnRunStarted;
	}

	public override void _ExitTree()
	{
		RunManager.Instance.RunStarted -= OnRunStarted;
	}

	public override void _Notification(int what)
	{
		if (((CanvasItem)this).Visible && (long)what == 2017)
		{
			((CanvasItem)this).Visible = false;
		}
	}

	public override void _Input(InputEvent inputEvent)
	{
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		Control val = ((Node)this).GetViewport().GuiGetFocusOwner();
		bool flag = ((val is TextEdit || val is LineEdit) ? true : false);
		bool flag2 = flag;
		if (!NGame.Instance.ReactionContainer.InMultiplayer)
		{
			if (((CanvasItem)this).Visible)
			{
				HideWheel();
			}
			return;
		}
		InputEventMouseMotion val2 = (InputEventMouseMotion)(object)((inputEvent is InputEventMouseMotion) ? inputEvent : null);
		if (val2 != null)
		{
			if (_ignoreNextMouseInput)
			{
				_ignoreNextMouseInput = false;
			}
			else if (((CanvasItem)this).Visible)
			{
				MoveMarker(val2.Relative);
				_ignoreNextMouseInput = true;
				WarpMouseBackToOriginalPosition();
			}
		}
		else if (inputEvent.IsActionPressed(_reactWheel, false, false) && !flag2)
		{
			((CanvasItem)this).Visible = true;
			if (_localPlayer != null)
			{
				_marker.Texture = (Texture2D)(object)_localPlayer.Character.MapMarker;
			}
			_centerPosition = ((Node)this).GetViewport().GetMousePosition();
			((Control)_marker).Position = (((Control)this).Size - ((Control)_marker).Size) * 0.5f;
			((Control)this).GlobalPosition = _centerPosition - ((Control)this).Size * ((Control)this).Scale * 0.5f;
			Input.MouseMode = (MouseModeEnum)1;
		}
		else if (inputEvent.IsActionReleased(_reactWheel, false) && ((CanvasItem)this).Visible)
		{
			HideWheel();
			React();
		}
	}

	private void OnRunStarted(RunState runState)
	{
		_localPlayer = LocalContext.GetMe(runState);
	}

	private void HideWheel()
	{
		Input.MouseMode = (MouseModeEnum)0;
		WarpMouseBackToOriginalPosition();
		((CanvasItem)this).Visible = false;
	}

	private void WarpMouseBackToOriginalPosition()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		Transform2D viewportTransform = ((CanvasItem)this).GetViewportTransform();
		Input.WarpMouse(viewportTransform * _centerPosition);
	}

	private void React()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		if (_selectedWedge != null)
		{
			NGame.Instance.ReactionContainer.DoLocalReaction(_selectedWedge.Reaction, _centerPosition);
		}
	}

	private void MoveMarker(Vector2 relative)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		Vector2 val = (((Control)this).Size - ((Control)_marker).Size) * 0.5f;
		Vector2 val2 = ((Control)_marker).Position - val;
		val2 += relative;
		val2 = ((Vector2)(ref val2)).LimitLength(70f);
		((Control)_marker).Position = val + val2;
		float num = Mathf.Atan2(val2.Y, val2.X);
		((Control)_marker).Rotation = num - (float)Math.PI / 2f;
		NReactionWheelWedge selectedWedge = GetSelectedWedge(num);
		if (_selectedWedge != selectedWedge)
		{
			_selectedWedge?.OnDeselected();
			_selectedWedge = selectedWedge;
			_selectedWedge?.OnSelected();
		}
	}

	private NReactionWheelWedge GetSelectedWedge(float angle)
	{
		float num = Mathf.Wrap(angle + (float)Math.PI / 8f, 0f, (float)Math.PI * 2f);
		float num2 = (float)Math.PI / 4f;
		return (int)(num / num2) switch
		{
			0 => _rightWedge, 
			1 => _downRightWedge, 
			2 => _downWedge, 
			3 => _downLeftWedge, 
			4 => _leftWedge, 
			5 => _upLeftWedge, 
			6 => _upWedge, 
			7 => _upRightWedge, 
			_ => throw new InvalidOperationException(), 
		};
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
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Expected O, but got Unknown
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Expected O, but got Unknown
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(10);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._EnterTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._ExitTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Notification, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("what"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Input, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("inputEvent"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("InputEvent"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.HideWheel, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.WarpMouseBackToOriginalPosition, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.React, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.MoveMarker, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)5, StringName.op_Implicit("relative"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.GetSelectedWedge, new PropertyInfo((Type)24, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("TextureRect"), false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)3, StringName.op_Implicit("angle"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
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
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
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
		if ((ref method) == MethodName._Notification && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			((GodotObject)this)._Notification(VariantUtils.ConvertTo<int>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._Input && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			((Node)this)._Input(VariantUtils.ConvertTo<InputEvent>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.HideWheel && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			HideWheel();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.WarpMouseBackToOriginalPosition && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			WarpMouseBackToOriginalPosition();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.React && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			React();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.MoveMarker && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			MoveMarker(VariantUtils.ConvertTo<Vector2>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.GetSelectedWedge && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			NReactionWheelWedge selectedWedge = GetSelectedWedge(VariantUtils.ConvertTo<float>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<NReactionWheelWedge>(ref selectedWedge);
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
		if ((ref method) == MethodName._Notification)
		{
			return true;
		}
		if ((ref method) == MethodName._Input)
		{
			return true;
		}
		if ((ref method) == MethodName.HideWheel)
		{
			return true;
		}
		if ((ref method) == MethodName.WarpMouseBackToOriginalPosition)
		{
			return true;
		}
		if ((ref method) == MethodName.React)
		{
			return true;
		}
		if ((ref method) == MethodName.MoveMarker)
		{
			return true;
		}
		if ((ref method) == MethodName.GetSelectedWedge)
		{
			return true;
		}
		return ((Control)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName._rightWedge)
		{
			_rightWedge = VariantUtils.ConvertTo<NReactionWheelWedge>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._downRightWedge)
		{
			_downRightWedge = VariantUtils.ConvertTo<NReactionWheelWedge>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._downWedge)
		{
			_downWedge = VariantUtils.ConvertTo<NReactionWheelWedge>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._downLeftWedge)
		{
			_downLeftWedge = VariantUtils.ConvertTo<NReactionWheelWedge>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._leftWedge)
		{
			_leftWedge = VariantUtils.ConvertTo<NReactionWheelWedge>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._upLeftWedge)
		{
			_upLeftWedge = VariantUtils.ConvertTo<NReactionWheelWedge>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._upWedge)
		{
			_upWedge = VariantUtils.ConvertTo<NReactionWheelWedge>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._upRightWedge)
		{
			_upRightWedge = VariantUtils.ConvertTo<NReactionWheelWedge>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._marker)
		{
			_marker = VariantUtils.ConvertTo<TextureRect>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._ignoreNextMouseInput)
		{
			_ignoreNextMouseInput = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._centerPosition)
		{
			_centerPosition = VariantUtils.ConvertTo<Vector2>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._selectedWedge)
		{
			_selectedWedge = VariantUtils.ConvertTo<NReactionWheelWedge>(ref value);
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
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName._rightWedge)
		{
			value = VariantUtils.CreateFrom<NReactionWheelWedge>(ref _rightWedge);
			return true;
		}
		if ((ref name) == PropertyName._downRightWedge)
		{
			value = VariantUtils.CreateFrom<NReactionWheelWedge>(ref _downRightWedge);
			return true;
		}
		if ((ref name) == PropertyName._downWedge)
		{
			value = VariantUtils.CreateFrom<NReactionWheelWedge>(ref _downWedge);
			return true;
		}
		if ((ref name) == PropertyName._downLeftWedge)
		{
			value = VariantUtils.CreateFrom<NReactionWheelWedge>(ref _downLeftWedge);
			return true;
		}
		if ((ref name) == PropertyName._leftWedge)
		{
			value = VariantUtils.CreateFrom<NReactionWheelWedge>(ref _leftWedge);
			return true;
		}
		if ((ref name) == PropertyName._upLeftWedge)
		{
			value = VariantUtils.CreateFrom<NReactionWheelWedge>(ref _upLeftWedge);
			return true;
		}
		if ((ref name) == PropertyName._upWedge)
		{
			value = VariantUtils.CreateFrom<NReactionWheelWedge>(ref _upWedge);
			return true;
		}
		if ((ref name) == PropertyName._upRightWedge)
		{
			value = VariantUtils.CreateFrom<NReactionWheelWedge>(ref _upRightWedge);
			return true;
		}
		if ((ref name) == PropertyName._marker)
		{
			value = VariantUtils.CreateFrom<TextureRect>(ref _marker);
			return true;
		}
		if ((ref name) == PropertyName._ignoreNextMouseInput)
		{
			value = VariantUtils.CreateFrom<bool>(ref _ignoreNextMouseInput);
			return true;
		}
		if ((ref name) == PropertyName._centerPosition)
		{
			value = VariantUtils.CreateFrom<Vector2>(ref _centerPosition);
			return true;
		}
		if ((ref name) == PropertyName._selectedWedge)
		{
			value = VariantUtils.CreateFrom<NReactionWheelWedge>(ref _selectedWedge);
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
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._rightWedge, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._downRightWedge, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._downWedge, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._downLeftWedge, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._leftWedge, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._upLeftWedge, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._upWedge, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._upRightWedge, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._marker, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._ignoreNextMouseInput, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)5, PropertyName._centerPosition, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._selectedWedge, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		info.AddProperty(PropertyName._rightWedge, Variant.From<NReactionWheelWedge>(ref _rightWedge));
		info.AddProperty(PropertyName._downRightWedge, Variant.From<NReactionWheelWedge>(ref _downRightWedge));
		info.AddProperty(PropertyName._downWedge, Variant.From<NReactionWheelWedge>(ref _downWedge));
		info.AddProperty(PropertyName._downLeftWedge, Variant.From<NReactionWheelWedge>(ref _downLeftWedge));
		info.AddProperty(PropertyName._leftWedge, Variant.From<NReactionWheelWedge>(ref _leftWedge));
		info.AddProperty(PropertyName._upLeftWedge, Variant.From<NReactionWheelWedge>(ref _upLeftWedge));
		info.AddProperty(PropertyName._upWedge, Variant.From<NReactionWheelWedge>(ref _upWedge));
		info.AddProperty(PropertyName._upRightWedge, Variant.From<NReactionWheelWedge>(ref _upRightWedge));
		info.AddProperty(PropertyName._marker, Variant.From<TextureRect>(ref _marker));
		info.AddProperty(PropertyName._ignoreNextMouseInput, Variant.From<bool>(ref _ignoreNextMouseInput));
		info.AddProperty(PropertyName._centerPosition, Variant.From<Vector2>(ref _centerPosition));
		info.AddProperty(PropertyName._selectedWedge, Variant.From<NReactionWheelWedge>(ref _selectedWedge));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._rightWedge, ref val))
		{
			_rightWedge = ((Variant)(ref val)).As<NReactionWheelWedge>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._downRightWedge, ref val2))
		{
			_downRightWedge = ((Variant)(ref val2)).As<NReactionWheelWedge>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._downWedge, ref val3))
		{
			_downWedge = ((Variant)(ref val3)).As<NReactionWheelWedge>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._downLeftWedge, ref val4))
		{
			_downLeftWedge = ((Variant)(ref val4)).As<NReactionWheelWedge>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._leftWedge, ref val5))
		{
			_leftWedge = ((Variant)(ref val5)).As<NReactionWheelWedge>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._upLeftWedge, ref val6))
		{
			_upLeftWedge = ((Variant)(ref val6)).As<NReactionWheelWedge>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName._upWedge, ref val7))
		{
			_upWedge = ((Variant)(ref val7)).As<NReactionWheelWedge>();
		}
		Variant val8 = default(Variant);
		if (info.TryGetProperty(PropertyName._upRightWedge, ref val8))
		{
			_upRightWedge = ((Variant)(ref val8)).As<NReactionWheelWedge>();
		}
		Variant val9 = default(Variant);
		if (info.TryGetProperty(PropertyName._marker, ref val9))
		{
			_marker = ((Variant)(ref val9)).As<TextureRect>();
		}
		Variant val10 = default(Variant);
		if (info.TryGetProperty(PropertyName._ignoreNextMouseInput, ref val10))
		{
			_ignoreNextMouseInput = ((Variant)(ref val10)).As<bool>();
		}
		Variant val11 = default(Variant);
		if (info.TryGetProperty(PropertyName._centerPosition, ref val11))
		{
			_centerPosition = ((Variant)(ref val11)).As<Vector2>();
		}
		Variant val12 = default(Variant);
		if (info.TryGetProperty(PropertyName._selectedWedge, ref val12))
		{
			_selectedWedge = ((Variant)(ref val12)).As<NReactionWheelWedge>();
		}
	}
}
