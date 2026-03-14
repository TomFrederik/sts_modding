using System;
using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.Screens.Map;

namespace MegaCrit.Sts2.Core.Nodes.Multiplayer;

[ScriptPath("res://src/Core/Nodes/Multiplayer/NRemoteMouseCursor.cs")]
public class NRemoteMouseCursor : Control
{
	public class MethodName : MethodName
	{
		public static readonly StringName Create = StringName.op_Implicit("Create");

		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName SetNextPosition = StringName.op_Implicit("SetNextPosition");

		public static readonly StringName _Process = StringName.op_Implicit("_Process");

		public static readonly StringName UpdateImage = StringName.op_Implicit("UpdateImage");

		public static readonly StringName GetHotspot = StringName.op_Implicit("GetHotspot");

		public static readonly StringName GetTexture = StringName.op_Implicit("GetTexture");

		public static readonly StringName RefreshSize = StringName.op_Implicit("RefreshSize");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName PlayerId = StringName.op_Implicit("PlayerId");

		public static readonly StringName _textureRect = StringName.op_Implicit("_textureRect");

		public static readonly StringName _lastPositionUpdateMsec = StringName.op_Implicit("_lastPositionUpdateMsec");

		public static readonly StringName _defaultHotspot = StringName.op_Implicit("_defaultHotspot");

		public static readonly StringName _drawingHotspot = StringName.op_Implicit("_drawingHotspot");

		public static readonly StringName _erasingHotspot = StringName.op_Implicit("_erasingHotspot");

		public static readonly StringName _defaultCursorImage = StringName.op_Implicit("_defaultCursorImage");

		public static readonly StringName _tiltedCursorImage = StringName.op_Implicit("_tiltedCursorImage");

		public static readonly StringName _defaultDrawingImage = StringName.op_Implicit("_defaultDrawingImage");

		public static readonly StringName _tiltedDrawingImage = StringName.op_Implicit("_tiltedDrawingImage");

		public static readonly StringName _defaultErasingImage = StringName.op_Implicit("_defaultErasingImage");

		public static readonly StringName _tiltedErasingImage = StringName.op_Implicit("_tiltedErasingImage");

		public static readonly StringName _defaultCursorTexture = StringName.op_Implicit("_defaultCursorTexture");

		public static readonly StringName _tiltedCursorTexture = StringName.op_Implicit("_tiltedCursorTexture");

		public static readonly StringName _defaultDrawingTexture = StringName.op_Implicit("_defaultDrawingTexture");

		public static readonly StringName _tiltedDrawingTexture = StringName.op_Implicit("_tiltedDrawingTexture");

		public static readonly StringName _defaultErasingTexture = StringName.op_Implicit("_defaultErasingTexture");

		public static readonly StringName _tiltedErasingTexture = StringName.op_Implicit("_tiltedErasingTexture");

		public static readonly StringName _drawingMode = StringName.op_Implicit("_drawingMode");
	}

	public class SignalName : SignalName
	{
	}

	private const string _scenePath = "ui/multiplayer/remote_mouse_cursor";

	private TextureRect _textureRect;

	private Vector2? _previousPosition;

	private Vector2? _nextPosition;

	private ulong _lastPositionUpdateMsec;

	private Vector2 _defaultHotspot;

	private Vector2 _drawingHotspot;

	private Vector2 _erasingHotspot;

	[Export(/*Could not decode attribute arguments.*/)]
	private Image _defaultCursorImage;

	[Export(/*Could not decode attribute arguments.*/)]
	private Image _tiltedCursorImage;

	[Export(/*Could not decode attribute arguments.*/)]
	private Image _defaultDrawingImage;

	[Export(/*Could not decode attribute arguments.*/)]
	private Image _tiltedDrawingImage;

	[Export(/*Could not decode attribute arguments.*/)]
	private Image _defaultErasingImage;

	[Export(/*Could not decode attribute arguments.*/)]
	private Image _tiltedErasingImage;

	private ImageTexture _defaultCursorTexture;

	private ImageTexture _tiltedCursorTexture;

	private ImageTexture _defaultDrawingTexture;

	private ImageTexture _tiltedDrawingTexture;

	private ImageTexture _defaultErasingTexture;

	private ImageTexture _tiltedErasingTexture;

	private DrawingMode _drawingMode;

	public ulong PlayerId { get; private set; }

	public static NRemoteMouseCursor Create(ulong playerId)
	{
		NRemoteMouseCursor nRemoteMouseCursor = PreloadManager.Cache.GetAsset<PackedScene>(SceneHelper.GetScenePath("ui/multiplayer/remote_mouse_cursor")).Instantiate<NRemoteMouseCursor>((GenEditState)0);
		nRemoteMouseCursor.PlayerId = playerId;
		return nRemoteMouseCursor;
	}

	public override void _Ready()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		_textureRect = ((Node)this).GetNode<TextureRect>(NodePath.op_Implicit("TextureRect"));
		_defaultHotspot = -((Control)_textureRect).Position;
		_drawingHotspot = NMapDrawings.drawingCursorHotspot;
		_erasingHotspot = NMapDrawings.erasingCursorHotspot;
		((Node)this).ProcessMode = (ProcessModeEnum)4;
		_defaultCursorTexture = ImageTexture.CreateFromImage(_defaultCursorImage);
		_tiltedCursorTexture = ImageTexture.CreateFromImage(_tiltedCursorImage);
		_defaultDrawingTexture = ImageTexture.CreateFromImage(_defaultDrawingImage);
		_tiltedDrawingTexture = ImageTexture.CreateFromImage(_tiltedDrawingImage);
		_defaultErasingTexture = ImageTexture.CreateFromImage(_defaultErasingImage);
		_tiltedErasingTexture = ImageTexture.CreateFromImage(_tiltedErasingImage);
		((GodotObject)((Node)this).GetViewport()).Connect(SignalName.SizeChanged, Callable.From((Action)RefreshSize), 0u);
	}

	public void SetNextPosition(Vector2 position)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		if (!_nextPosition.HasValue)
		{
			_nextPosition = position;
		}
		_previousPosition = _nextPosition;
		_nextPosition = position;
		_lastPositionUpdateMsec = Time.GetTicksMsec();
		((Node)this).ProcessMode = (ProcessModeEnum)0;
	}

	public override void _Process(double delta)
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		if (_previousPosition.HasValue && _nextPosition.HasValue)
		{
			float num = (float)(Time.GetTicksMsec() - _lastPositionUpdateMsec) / 50f;
			Vector2 value = _previousPosition.Value;
			((Control)this).Position = ((Vector2)(ref value)).Lerp(_nextPosition.Value, Mathf.Clamp(num, 0f, 1f));
			if (num >= 1f)
			{
				((Node)this).ProcessMode = (ProcessModeEnum)4;
			}
		}
	}

	public void UpdateImage(bool isDown, DrawingMode drawingMode)
	{
		_textureRect.Texture = GetTexture(isDown, drawingMode);
		_drawingMode = drawingMode;
		RefreshSize();
	}

	private Vector2 GetHotspot(DrawingMode drawingMode)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		switch (drawingMode)
		{
		case DrawingMode.None:
			return -_defaultHotspot;
		case DrawingMode.Drawing:
			return -_drawingHotspot;
		case DrawingMode.Erasing:
			return -_erasingHotspot;
		default:
		{
			global::_003CPrivateImplementationDetails_003E.ThrowSwitchExpressionException(drawingMode);
			Vector2 result = default(Vector2);
			return result;
		}
		}
	}

	private Texture2D GetTexture(bool isDown, DrawingMode drawingMode)
	{
		switch (drawingMode)
		{
		case DrawingMode.None:
			return (Texture2D)(object)(isDown ? _tiltedCursorTexture : _defaultCursorTexture);
		case DrawingMode.Drawing:
			return (Texture2D)(object)(isDown ? _defaultDrawingTexture : _tiltedDrawingTexture);
		case DrawingMode.Erasing:
			return (Texture2D)(object)(isDown ? _defaultErasingTexture : _tiltedErasingTexture);
		default:
		{
			global::_003CPrivateImplementationDetails_003E.ThrowSwitchExpressionException(drawingMode);
			ImageTexture result = default(ImageTexture);
			return (Texture2D)(object)result;
		}
		}
	}

	public void RefreshSize()
	{
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		if (OS.GetName() == "Windows")
		{
			int num = DisplayServer.ScreenGetDpi(-1);
			float num2 = (float)num / 96f;
			Transform2D stretchTransform = ((Node)this).GetViewport().GetStretchTransform();
			Vector2 scale = ((Transform2D)(ref stretchTransform)).Scale;
			Vector2 val = ((Vector2)(ref scale)).Inverse();
			Vector2 size = _textureRect.Texture.GetSize();
			Vector2 size2 = size * val * num2;
			((Control)_textureRect).Size = size2;
			((Control)_textureRect).Position = GetHotspot(_drawingMode) * val * num2;
		}
		else
		{
			((Control)_textureRect).Size = _textureRect.Texture.GetSize();
			((Control)_textureRect).Position = GetHotspot(_drawingMode);
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Expected O, but got Unknown
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Expected O, but got Unknown
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_0272: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(8);
		list.Add(new MethodInfo(MethodName.Create, new PropertyInfo((Type)24, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false), (MethodFlags)33, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("playerId"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetNextPosition, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)5, StringName.op_Implicit("position"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Process, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)3, StringName.op_Implicit("delta"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UpdateImage, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)1, StringName.op_Implicit("isDown"), (PropertyHint)0, "", (PropertyUsageFlags)6, false),
			new PropertyInfo((Type)2, StringName.op_Implicit("drawingMode"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.GetHotspot, new PropertyInfo((Type)5, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("drawingMode"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.GetTexture, new PropertyInfo((Type)24, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Texture2D"), false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)1, StringName.op_Implicit("isDown"), (PropertyHint)0, "", (PropertyUsageFlags)6, false),
			new PropertyInfo((Type)2, StringName.op_Implicit("drawingMode"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.RefreshSize, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.Create && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			NRemoteMouseCursor nRemoteMouseCursor = Create(VariantUtils.ConvertTo<ulong>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<NRemoteMouseCursor>(ref nRemoteMouseCursor);
			return true;
		}
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetNextPosition && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			SetNextPosition(VariantUtils.ConvertTo<Vector2>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._Process && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			((Node)this)._Process(VariantUtils.ConvertTo<double>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.UpdateImage && ((NativeVariantPtrArgs)(ref args)).Count == 2)
		{
			UpdateImage(VariantUtils.ConvertTo<bool>(ref ((NativeVariantPtrArgs)(ref args))[0]), VariantUtils.ConvertTo<DrawingMode>(ref ((NativeVariantPtrArgs)(ref args))[1]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.GetHotspot && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			Vector2 hotspot = GetHotspot(VariantUtils.ConvertTo<DrawingMode>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<Vector2>(ref hotspot);
			return true;
		}
		if ((ref method) == MethodName.GetTexture && ((NativeVariantPtrArgs)(ref args)).Count == 2)
		{
			Texture2D texture = GetTexture(VariantUtils.ConvertTo<bool>(ref ((NativeVariantPtrArgs)(ref args))[0]), VariantUtils.ConvertTo<DrawingMode>(ref ((NativeVariantPtrArgs)(ref args))[1]));
			ret = VariantUtils.CreateFrom<Texture2D>(ref texture);
			return true;
		}
		if ((ref method) == MethodName.RefreshSize && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			RefreshSize();
			ret = default(godot_variant);
			return true;
		}
		return ((Control)this).InvokeGodotClassMethod(ref method, args, ref ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static bool InvokeGodotClassStaticMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.Create && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			NRemoteMouseCursor nRemoteMouseCursor = Create(VariantUtils.ConvertTo<ulong>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<NRemoteMouseCursor>(ref nRemoteMouseCursor);
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
		if ((ref method) == MethodName.SetNextPosition)
		{
			return true;
		}
		if ((ref method) == MethodName._Process)
		{
			return true;
		}
		if ((ref method) == MethodName.UpdateImage)
		{
			return true;
		}
		if ((ref method) == MethodName.GetHotspot)
		{
			return true;
		}
		if ((ref method) == MethodName.GetTexture)
		{
			return true;
		}
		if ((ref method) == MethodName.RefreshSize)
		{
			return true;
		}
		return ((Control)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName.PlayerId)
		{
			PlayerId = VariantUtils.ConvertTo<ulong>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._textureRect)
		{
			_textureRect = VariantUtils.ConvertTo<TextureRect>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._lastPositionUpdateMsec)
		{
			_lastPositionUpdateMsec = VariantUtils.ConvertTo<ulong>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._defaultHotspot)
		{
			_defaultHotspot = VariantUtils.ConvertTo<Vector2>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._drawingHotspot)
		{
			_drawingHotspot = VariantUtils.ConvertTo<Vector2>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._erasingHotspot)
		{
			_erasingHotspot = VariantUtils.ConvertTo<Vector2>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._defaultCursorImage)
		{
			_defaultCursorImage = VariantUtils.ConvertTo<Image>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._tiltedCursorImage)
		{
			_tiltedCursorImage = VariantUtils.ConvertTo<Image>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._defaultDrawingImage)
		{
			_defaultDrawingImage = VariantUtils.ConvertTo<Image>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._tiltedDrawingImage)
		{
			_tiltedDrawingImage = VariantUtils.ConvertTo<Image>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._defaultErasingImage)
		{
			_defaultErasingImage = VariantUtils.ConvertTo<Image>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._tiltedErasingImage)
		{
			_tiltedErasingImage = VariantUtils.ConvertTo<Image>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._defaultCursorTexture)
		{
			_defaultCursorTexture = VariantUtils.ConvertTo<ImageTexture>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._tiltedCursorTexture)
		{
			_tiltedCursorTexture = VariantUtils.ConvertTo<ImageTexture>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._defaultDrawingTexture)
		{
			_defaultDrawingTexture = VariantUtils.ConvertTo<ImageTexture>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._tiltedDrawingTexture)
		{
			_tiltedDrawingTexture = VariantUtils.ConvertTo<ImageTexture>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._defaultErasingTexture)
		{
			_defaultErasingTexture = VariantUtils.ConvertTo<ImageTexture>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._tiltedErasingTexture)
		{
			_tiltedErasingTexture = VariantUtils.ConvertTo<ImageTexture>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._drawingMode)
		{
			_drawingMode = VariantUtils.ConvertTo<DrawingMode>(ref value);
			return true;
		}
		return ((GodotObject)this).SetGodotClassPropertyValue(ref name, ref value);
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
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName.PlayerId)
		{
			ulong playerId = PlayerId;
			value = VariantUtils.CreateFrom<ulong>(ref playerId);
			return true;
		}
		if ((ref name) == PropertyName._textureRect)
		{
			value = VariantUtils.CreateFrom<TextureRect>(ref _textureRect);
			return true;
		}
		if ((ref name) == PropertyName._lastPositionUpdateMsec)
		{
			value = VariantUtils.CreateFrom<ulong>(ref _lastPositionUpdateMsec);
			return true;
		}
		if ((ref name) == PropertyName._defaultHotspot)
		{
			value = VariantUtils.CreateFrom<Vector2>(ref _defaultHotspot);
			return true;
		}
		if ((ref name) == PropertyName._drawingHotspot)
		{
			value = VariantUtils.CreateFrom<Vector2>(ref _drawingHotspot);
			return true;
		}
		if ((ref name) == PropertyName._erasingHotspot)
		{
			value = VariantUtils.CreateFrom<Vector2>(ref _erasingHotspot);
			return true;
		}
		if ((ref name) == PropertyName._defaultCursorImage)
		{
			value = VariantUtils.CreateFrom<Image>(ref _defaultCursorImage);
			return true;
		}
		if ((ref name) == PropertyName._tiltedCursorImage)
		{
			value = VariantUtils.CreateFrom<Image>(ref _tiltedCursorImage);
			return true;
		}
		if ((ref name) == PropertyName._defaultDrawingImage)
		{
			value = VariantUtils.CreateFrom<Image>(ref _defaultDrawingImage);
			return true;
		}
		if ((ref name) == PropertyName._tiltedDrawingImage)
		{
			value = VariantUtils.CreateFrom<Image>(ref _tiltedDrawingImage);
			return true;
		}
		if ((ref name) == PropertyName._defaultErasingImage)
		{
			value = VariantUtils.CreateFrom<Image>(ref _defaultErasingImage);
			return true;
		}
		if ((ref name) == PropertyName._tiltedErasingImage)
		{
			value = VariantUtils.CreateFrom<Image>(ref _tiltedErasingImage);
			return true;
		}
		if ((ref name) == PropertyName._defaultCursorTexture)
		{
			value = VariantUtils.CreateFrom<ImageTexture>(ref _defaultCursorTexture);
			return true;
		}
		if ((ref name) == PropertyName._tiltedCursorTexture)
		{
			value = VariantUtils.CreateFrom<ImageTexture>(ref _tiltedCursorTexture);
			return true;
		}
		if ((ref name) == PropertyName._defaultDrawingTexture)
		{
			value = VariantUtils.CreateFrom<ImageTexture>(ref _defaultDrawingTexture);
			return true;
		}
		if ((ref name) == PropertyName._tiltedDrawingTexture)
		{
			value = VariantUtils.CreateFrom<ImageTexture>(ref _tiltedDrawingTexture);
			return true;
		}
		if ((ref name) == PropertyName._defaultErasingTexture)
		{
			value = VariantUtils.CreateFrom<ImageTexture>(ref _defaultErasingTexture);
			return true;
		}
		if ((ref name) == PropertyName._tiltedErasingTexture)
		{
			value = VariantUtils.CreateFrom<ImageTexture>(ref _tiltedErasingTexture);
			return true;
		}
		if ((ref name) == PropertyName._drawingMode)
		{
			value = VariantUtils.CreateFrom<DrawingMode>(ref _drawingMode);
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
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._textureRect, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)2, PropertyName.PlayerId, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)2, PropertyName._lastPositionUpdateMsec, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)5, PropertyName._defaultHotspot, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)5, PropertyName._drawingHotspot, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)5, PropertyName._erasingHotspot, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._defaultCursorImage, (PropertyHint)17, "Image", (PropertyUsageFlags)4102, true));
		list.Add(new PropertyInfo((Type)24, PropertyName._tiltedCursorImage, (PropertyHint)17, "Image", (PropertyUsageFlags)4102, true));
		list.Add(new PropertyInfo((Type)24, PropertyName._defaultDrawingImage, (PropertyHint)17, "Image", (PropertyUsageFlags)4102, true));
		list.Add(new PropertyInfo((Type)24, PropertyName._tiltedDrawingImage, (PropertyHint)17, "Image", (PropertyUsageFlags)4102, true));
		list.Add(new PropertyInfo((Type)24, PropertyName._defaultErasingImage, (PropertyHint)17, "Image", (PropertyUsageFlags)4102, true));
		list.Add(new PropertyInfo((Type)24, PropertyName._tiltedErasingImage, (PropertyHint)17, "Image", (PropertyUsageFlags)4102, true));
		list.Add(new PropertyInfo((Type)24, PropertyName._defaultCursorTexture, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._tiltedCursorTexture, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._defaultDrawingTexture, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._tiltedDrawingTexture, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._defaultErasingTexture, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._tiltedErasingTexture, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)2, PropertyName._drawingMode, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		StringName playerId = PropertyName.PlayerId;
		ulong playerId2 = PlayerId;
		info.AddProperty(playerId, Variant.From<ulong>(ref playerId2));
		info.AddProperty(PropertyName._textureRect, Variant.From<TextureRect>(ref _textureRect));
		info.AddProperty(PropertyName._lastPositionUpdateMsec, Variant.From<ulong>(ref _lastPositionUpdateMsec));
		info.AddProperty(PropertyName._defaultHotspot, Variant.From<Vector2>(ref _defaultHotspot));
		info.AddProperty(PropertyName._drawingHotspot, Variant.From<Vector2>(ref _drawingHotspot));
		info.AddProperty(PropertyName._erasingHotspot, Variant.From<Vector2>(ref _erasingHotspot));
		info.AddProperty(PropertyName._defaultCursorImage, Variant.From<Image>(ref _defaultCursorImage));
		info.AddProperty(PropertyName._tiltedCursorImage, Variant.From<Image>(ref _tiltedCursorImage));
		info.AddProperty(PropertyName._defaultDrawingImage, Variant.From<Image>(ref _defaultDrawingImage));
		info.AddProperty(PropertyName._tiltedDrawingImage, Variant.From<Image>(ref _tiltedDrawingImage));
		info.AddProperty(PropertyName._defaultErasingImage, Variant.From<Image>(ref _defaultErasingImage));
		info.AddProperty(PropertyName._tiltedErasingImage, Variant.From<Image>(ref _tiltedErasingImage));
		info.AddProperty(PropertyName._defaultCursorTexture, Variant.From<ImageTexture>(ref _defaultCursorTexture));
		info.AddProperty(PropertyName._tiltedCursorTexture, Variant.From<ImageTexture>(ref _tiltedCursorTexture));
		info.AddProperty(PropertyName._defaultDrawingTexture, Variant.From<ImageTexture>(ref _defaultDrawingTexture));
		info.AddProperty(PropertyName._tiltedDrawingTexture, Variant.From<ImageTexture>(ref _tiltedDrawingTexture));
		info.AddProperty(PropertyName._defaultErasingTexture, Variant.From<ImageTexture>(ref _defaultErasingTexture));
		info.AddProperty(PropertyName._tiltedErasingTexture, Variant.From<ImageTexture>(ref _tiltedErasingTexture));
		info.AddProperty(PropertyName._drawingMode, Variant.From<DrawingMode>(ref _drawingMode));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName.PlayerId, ref val))
		{
			PlayerId = ((Variant)(ref val)).As<ulong>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._textureRect, ref val2))
		{
			_textureRect = ((Variant)(ref val2)).As<TextureRect>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._lastPositionUpdateMsec, ref val3))
		{
			_lastPositionUpdateMsec = ((Variant)(ref val3)).As<ulong>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._defaultHotspot, ref val4))
		{
			_defaultHotspot = ((Variant)(ref val4)).As<Vector2>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._drawingHotspot, ref val5))
		{
			_drawingHotspot = ((Variant)(ref val5)).As<Vector2>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._erasingHotspot, ref val6))
		{
			_erasingHotspot = ((Variant)(ref val6)).As<Vector2>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName._defaultCursorImage, ref val7))
		{
			_defaultCursorImage = ((Variant)(ref val7)).As<Image>();
		}
		Variant val8 = default(Variant);
		if (info.TryGetProperty(PropertyName._tiltedCursorImage, ref val8))
		{
			_tiltedCursorImage = ((Variant)(ref val8)).As<Image>();
		}
		Variant val9 = default(Variant);
		if (info.TryGetProperty(PropertyName._defaultDrawingImage, ref val9))
		{
			_defaultDrawingImage = ((Variant)(ref val9)).As<Image>();
		}
		Variant val10 = default(Variant);
		if (info.TryGetProperty(PropertyName._tiltedDrawingImage, ref val10))
		{
			_tiltedDrawingImage = ((Variant)(ref val10)).As<Image>();
		}
		Variant val11 = default(Variant);
		if (info.TryGetProperty(PropertyName._defaultErasingImage, ref val11))
		{
			_defaultErasingImage = ((Variant)(ref val11)).As<Image>();
		}
		Variant val12 = default(Variant);
		if (info.TryGetProperty(PropertyName._tiltedErasingImage, ref val12))
		{
			_tiltedErasingImage = ((Variant)(ref val12)).As<Image>();
		}
		Variant val13 = default(Variant);
		if (info.TryGetProperty(PropertyName._defaultCursorTexture, ref val13))
		{
			_defaultCursorTexture = ((Variant)(ref val13)).As<ImageTexture>();
		}
		Variant val14 = default(Variant);
		if (info.TryGetProperty(PropertyName._tiltedCursorTexture, ref val14))
		{
			_tiltedCursorTexture = ((Variant)(ref val14)).As<ImageTexture>();
		}
		Variant val15 = default(Variant);
		if (info.TryGetProperty(PropertyName._defaultDrawingTexture, ref val15))
		{
			_defaultDrawingTexture = ((Variant)(ref val15)).As<ImageTexture>();
		}
		Variant val16 = default(Variant);
		if (info.TryGetProperty(PropertyName._tiltedDrawingTexture, ref val16))
		{
			_tiltedDrawingTexture = ((Variant)(ref val16)).As<ImageTexture>();
		}
		Variant val17 = default(Variant);
		if (info.TryGetProperty(PropertyName._defaultErasingTexture, ref val17))
		{
			_defaultErasingTexture = ((Variant)(ref val17)).As<ImageTexture>();
		}
		Variant val18 = default(Variant);
		if (info.TryGetProperty(PropertyName._tiltedErasingTexture, ref val18))
		{
			_tiltedErasingTexture = ((Variant)(ref val18)).As<ImageTexture>();
		}
		Variant val19 = default(Variant);
		if (info.TryGetProperty(PropertyName._drawingMode, ref val19))
		{
			_drawingMode = ((Variant)(ref val19)).As<DrawingMode>();
		}
	}
}
