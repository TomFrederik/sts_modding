using System;
using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.ControllerInput;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Screens.ScreenContext;

namespace MegaCrit.Sts2.Core.Nodes.Screens.Map;

[ScriptPath("res://src/Core/Nodes/Screens/Map/NControllerMapDrawingInput.cs")]
public class NControllerMapDrawingInput : NMapDrawingInput
{
	public new class MethodName : NMapDrawingInput.MethodName
	{
		public new static readonly StringName Create = StringName.op_Implicit("Create");

		public new static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName _Process = StringName.op_Implicit("_Process");

		public static readonly StringName _Input = StringName.op_Implicit("_Input");
	}

	public new class PropertyName : NMapDrawingInput.PropertyName
	{
		public static readonly StringName _eraserIconPos = StringName.op_Implicit("_eraserIconPos");

		public static readonly StringName _drawingIconPos = StringName.op_Implicit("_drawingIconPos");

		public static readonly StringName _isPressed = StringName.op_Implicit("_isPressed");

		public static readonly StringName _cursorTex = StringName.op_Implicit("_cursorTex");

		public static readonly StringName _cursorTiltedTex = StringName.op_Implicit("_cursorTiltedTex");

		public static readonly StringName _cursor = StringName.op_Implicit("_cursor");

		public static readonly StringName _direction = StringName.op_Implicit("_direction");
	}

	public new class SignalName : NMapDrawingInput.SignalName
	{
	}

	private const string _scenePath = "res://scenes/screens/map/controller_map_drawing_input.tscn";

	private Vector2 _eraserIconPos = new Vector2(-34f, -76f);

	private Vector2 _drawingIconPos = new Vector2(-10f, -76f);

	private bool _isPressed;

	private Texture2D _cursorTex;

	private Texture2D _cursorTiltedTex;

	private Control _cursor;

	private Vector2 _direction;

	public static NMapDrawingInput Create()
	{
		return PreloadManager.Cache.GetScene("res://scenes/screens/map/controller_map_drawing_input.tscn").Instantiate<NControllerMapDrawingInput>((GenEditState)0);
	}

	public override void _Ready()
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		base._Ready();
		_cursor = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%Cursor"));
		((Control)(object)this).TryGrabFocus();
		((GodotObject)((Node)this).GetViewport()).Connect(SignalName.GuiFocusChanged, Callable.From<Control>((Action<Control>)delegate
		{
			StopDrawing();
		}), 0u);
		if (base.DrawingMode == DrawingMode.Drawing)
		{
			_cursorTex = (Texture2D)(object)ImageTexture.CreateFromImage(PreloadManager.Cache.GetAsset<Image>("res://images/packed/common_ui/cursor_quill.png"));
			_cursorTiltedTex = (Texture2D)(object)ImageTexture.CreateFromImage(PreloadManager.Cache.GetAsset<Image>("res://images/packed/common_ui/cursor_quill_tilted.png"));
		}
		else
		{
			_cursorTex = (Texture2D)(object)ImageTexture.CreateFromImage(PreloadManager.Cache.GetAsset<Image>("res://images/packed/common_ui/cursor_eraser.png"));
			_cursorTiltedTex = (Texture2D)(object)ImageTexture.CreateFromImage(PreloadManager.Cache.GetAsset<Image>("res://images/packed/common_ui/cursor_eraser_tilted.png"));
		}
		((Node)_cursor).GetNode<TextureRect>(NodePath.op_Implicit("TextureRect")).Texture = _cursorTex;
		((Control)((Node)_cursor).GetNode<TextureRect>(NodePath.op_Implicit("TextureRect"))).Position = ((base.DrawingMode == DrawingMode.Drawing) ? _drawingIconPos : _eraserIconPos);
	}

	public override void _Process(double delta)
	{
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		Transform2D globalTransform;
		if (Input.IsActionPressed(MegaInput.select, false))
		{
			if (!_isPressed)
			{
				NMapDrawings drawings = _drawings;
				globalTransform = ((CanvasItem)_drawings).GetGlobalTransform();
				drawings.BeginLineLocal(((Transform2D)(ref globalTransform)).Inverse() * _cursor.GlobalPosition, null);
				((Node)_cursor).GetNode<TextureRect>(NodePath.op_Implicit("TextureRect")).Texture = _cursorTiltedTex;
				_isPressed = true;
			}
		}
		else if (_isPressed)
		{
			_drawings.StopLineLocal();
			((Node)_cursor).GetNode<TextureRect>(NodePath.op_Implicit("TextureRect")).Texture = _cursorTex;
			_isPressed = false;
		}
		_direction = Input.GetVector(Controller.joystickLeft, Controller.joystickRight, Controller.joystickUp, Controller.joystickDown, -1f);
		if (((Vector2)(ref _direction)).Length() < 0.1f)
		{
			_direction += Input.GetVector(Controller.dPadWest, Controller.dPadEast, Controller.dPadNorth, Controller.dPadSouth, -1f);
		}
		if (((Vector2)(ref _direction)).Length() > 0f)
		{
			Control cursor = _cursor;
			cursor.GlobalPosition += _direction * 700f * (float)delta;
			Control cursor2 = _cursor;
			Vector2 globalPosition = _cursor.GlobalPosition;
			cursor2.GlobalPosition = ((Vector2)(ref globalPosition)).Clamp(((Control)NGame.Instance).GlobalPosition, ((Control)NGame.Instance).GlobalPosition + ((Control)NGame.Instance).Size);
			if (_drawings.IsLocalDrawing())
			{
				NMapDrawings drawings2 = _drawings;
				globalTransform = ((CanvasItem)_drawings).GetGlobalTransform();
				drawings2.UpdateCurrentLinePositionLocal(((Transform2D)(ref globalTransform)).Inverse() * _cursor.GlobalPosition);
			}
		}
	}

	public override void _Input(InputEvent input)
	{
		if (((CanvasItem)this).IsVisibleInTree() && input.IsActionPressed(MegaInput.cancel, false, false))
		{
			StopDrawing();
			ActiveScreenContext.Instance.FocusOnDefaultControl();
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Expected O, but got Unknown
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Expected O, but got Unknown
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(4);
		list.Add(new MethodInfo(MethodName.Create, new PropertyInfo((Type)24, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false), (MethodFlags)33, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Process, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)3, StringName.op_Implicit("delta"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Input, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("input"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("InputEvent"), false)
		}, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.Create && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			NMapDrawingInput nMapDrawingInput = Create();
			ret = VariantUtils.CreateFrom<NMapDrawingInput>(ref nMapDrawingInput);
			return true;
		}
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._Process && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			((Node)this)._Process(VariantUtils.ConvertTo<double>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._Input && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			((Node)this)._Input(VariantUtils.ConvertTo<InputEvent>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		return base.InvokeGodotClassMethod(in method, args, out ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static bool InvokeGodotClassStaticMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.Create && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			NMapDrawingInput nMapDrawingInput = Create();
			ret = VariantUtils.CreateFrom<NMapDrawingInput>(ref nMapDrawingInput);
			return true;
		}
		ret = default(godot_variant);
		return false;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if ((ref method) == MethodName.Create)
		{
			return true;
		}
		if ((ref method) == MethodName._Ready)
		{
			return true;
		}
		if ((ref method) == MethodName._Process)
		{
			return true;
		}
		if ((ref method) == MethodName._Input)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName._eraserIconPos)
		{
			_eraserIconPos = VariantUtils.ConvertTo<Vector2>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._drawingIconPos)
		{
			_drawingIconPos = VariantUtils.ConvertTo<Vector2>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._isPressed)
		{
			_isPressed = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._cursorTex)
		{
			_cursorTex = VariantUtils.ConvertTo<Texture2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._cursorTiltedTex)
		{
			_cursorTiltedTex = VariantUtils.ConvertTo<Texture2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._cursor)
		{
			_cursor = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._direction)
		{
			_direction = VariantUtils.ConvertTo<Vector2>(ref value);
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
		if ((ref name) == PropertyName._eraserIconPos)
		{
			value = VariantUtils.CreateFrom<Vector2>(ref _eraserIconPos);
			return true;
		}
		if ((ref name) == PropertyName._drawingIconPos)
		{
			value = VariantUtils.CreateFrom<Vector2>(ref _drawingIconPos);
			return true;
		}
		if ((ref name) == PropertyName._isPressed)
		{
			value = VariantUtils.CreateFrom<bool>(ref _isPressed);
			return true;
		}
		if ((ref name) == PropertyName._cursorTex)
		{
			value = VariantUtils.CreateFrom<Texture2D>(ref _cursorTex);
			return true;
		}
		if ((ref name) == PropertyName._cursorTiltedTex)
		{
			value = VariantUtils.CreateFrom<Texture2D>(ref _cursorTiltedTex);
			return true;
		}
		if ((ref name) == PropertyName._cursor)
		{
			value = VariantUtils.CreateFrom<Control>(ref _cursor);
			return true;
		}
		if ((ref name) == PropertyName._direction)
		{
			value = VariantUtils.CreateFrom<Vector2>(ref _direction);
			return true;
		}
		return base.GetGodotClassPropertyValue(in name, out value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)5, PropertyName._eraserIconPos, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)5, PropertyName._drawingIconPos, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._isPressed, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._cursorTex, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._cursorTiltedTex, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._cursor, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)5, PropertyName._direction, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		info.AddProperty(PropertyName._eraserIconPos, Variant.From<Vector2>(ref _eraserIconPos));
		info.AddProperty(PropertyName._drawingIconPos, Variant.From<Vector2>(ref _drawingIconPos));
		info.AddProperty(PropertyName._isPressed, Variant.From<bool>(ref _isPressed));
		info.AddProperty(PropertyName._cursorTex, Variant.From<Texture2D>(ref _cursorTex));
		info.AddProperty(PropertyName._cursorTiltedTex, Variant.From<Texture2D>(ref _cursorTiltedTex));
		info.AddProperty(PropertyName._cursor, Variant.From<Control>(ref _cursor));
		info.AddProperty(PropertyName._direction, Variant.From<Vector2>(ref _direction));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		base.RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._eraserIconPos, ref val))
		{
			_eraserIconPos = ((Variant)(ref val)).As<Vector2>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._drawingIconPos, ref val2))
		{
			_drawingIconPos = ((Variant)(ref val2)).As<Vector2>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._isPressed, ref val3))
		{
			_isPressed = ((Variant)(ref val3)).As<bool>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._cursorTex, ref val4))
		{
			_cursorTex = ((Variant)(ref val4)).As<Texture2D>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._cursorTiltedTex, ref val5))
		{
			_cursorTiltedTex = ((Variant)(ref val5)).As<Texture2D>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._cursor, ref val6))
		{
			_cursor = ((Variant)(ref val6)).As<Control>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName._direction, ref val7))
		{
			_direction = ((Variant)(ref val7)).As<Vector2>();
		}
	}
}
