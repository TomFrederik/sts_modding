using System;
using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.CommonUi;

namespace MegaCrit.Sts2.Core.Nodes.Combat;

[ScriptPath("res://src/Core/Nodes/Combat/NTargetingArrow.cs")]
public class NTargetingArrow : Node2D
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName _Process = StringName.op_Implicit("_Process");

		public static readonly StringName UpdateArrowPosition = StringName.op_Implicit("UpdateArrowPosition");

		public static readonly StringName SetHighlightingOn = StringName.op_Implicit("SetHighlightingOn");

		public static readonly StringName SetHighlightingOff = StringName.op_Implicit("SetHighlightingOff");

		public static readonly StringName UpdateSegments = StringName.op_Implicit("UpdateSegments");

		public static readonly StringName StartDrawingFrom = StringName.op_Implicit("StartDrawingFrom");

		public static readonly StringName StopDrawing = StringName.op_Implicit("StopDrawing");

		public static readonly StringName UpdateDrawingTo = StringName.op_Implicit("UpdateDrawingTo");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName From = StringName.op_Implicit("From");

		public static readonly StringName _fromPos = StringName.op_Implicit("_fromPos");

		public static readonly StringName _fromControl = StringName.op_Implicit("_fromControl");

		public static readonly StringName _toPosition = StringName.op_Implicit("_toPosition");

		public static readonly StringName _segments = StringName.op_Implicit("_segments");

		public static readonly StringName _arrowHead = StringName.op_Implicit("_arrowHead");

		public static readonly StringName _arrowHeadTween = StringName.op_Implicit("_arrowHeadTween");

		public static readonly StringName _initialized = StringName.op_Implicit("_initialized");

		public static readonly StringName _followMouse = StringName.op_Implicit("_followMouse");
	}

	public class SignalName : SignalName
	{
	}

	private const int _segmentCount = 19;

	private Vector2 _fromPos;

	private Control? _fromControl;

	private Vector2 _toPosition;

	private Vector2? _currentArrowPos;

	private static readonly string _segmentHeadPath = ImageHelper.GetImagePath("ui/combat/targeting_arrow_head.png");

	private static readonly string _segmentBlockPath = ImageHelper.GetImagePath("ui/combat/targeting_arrow_segment.png");

	private Sprite2D[] _segments = (Sprite2D[])(object)new Sprite2D[19];

	private Sprite2D _arrowHead;

	private Tween? _arrowHeadTween;

	private bool _initialized;

	private bool _followMouse;

	private const float _segmentScaleStart = 0.28f;

	private const float _segmentScaleEnd = 0.42f;

	private static readonly Vector2 _arrowHeadDefaultScale = Vector2.One * 0.95f;

	private static readonly Vector2 _arrowHeadHoverScale = Vector2.One * 1.05f;

	private Vector2 From
	{
		get
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			Control? fromControl = _fromControl;
			if (fromControl == null)
			{
				return _fromPos;
			}
			return fromControl.GlobalPosition;
		}
	}

	private static Texture2D SegmentHead => PreloadManager.Cache.GetTexture2D(_segmentHeadPath);

	private static Texture2D SegmentBlock => PreloadManager.Cache.GetTexture2D(_segmentBlockPath);

	public static IEnumerable<string> AssetPaths => new global::_003C_003Ez__ReadOnlyArray<string>(new string[2] { _segmentHeadPath, _segmentBlockPath });

	public override void _Ready()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Expected O, but got Unknown
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Expected O, but got Unknown
		if (!_initialized)
		{
			_initialized = true;
			for (int i = 0; i < 19; i++)
			{
				_segments[i] = new Sprite2D();
				_segments[i].Texture = SegmentBlock;
				((Node)(object)this).AddChildSafely((Node?)(object)_segments[i]);
			}
			_arrowHead = new Sprite2D();
			_arrowHead.Texture = SegmentHead;
			((Node)(object)this).AddChildSafely((Node?)(object)_arrowHead);
			StopDrawing();
		}
	}

	public override void _Process(double delta)
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (((CanvasItem)this).Visible)
		{
			if (_followMouse)
			{
				UpdateDrawingTo(((Node)this).GetViewport().GetMousePosition());
				UpdateArrowPosition(_toPosition);
			}
			else
			{
				Vector2 value = _currentArrowPos.Value;
				_currentArrowPos = ((Vector2)(ref value)).Lerp(_toPosition, (float)delta * 14f);
				UpdateArrowPosition(_currentArrowPos.Value);
			}
		}
	}

	private void UpdateArrowPosition(Vector2 targetPos)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		Sprite2D arrowHead = _arrowHead;
		Vector2 val = new Vector2(0f, 88f);
		((Node2D)arrowHead).Position = targetPos + ((Vector2)(ref val)).Rotated(((Node2D)_arrowHead).Rotation);
		val = new Vector2(0f, 40f);
		Vector2 finalPos = targetPos + ((Vector2)(ref val)).Rotated(((Node2D)_arrowHead).Rotation);
		Vector2 zero = Vector2.Zero;
		zero.X = From.X - (((Node2D)_arrowHead).Position.X - From.X) * 0.25f;
		if (From.Y > 540f)
		{
			zero.Y = ((Node2D)_arrowHead).Position.Y + (((Node2D)_arrowHead).Position.Y - From.Y) * 0.5f;
		}
		else
		{
			zero.Y = ((Node2D)_arrowHead).Position.Y * 0.75f + From.Y * 0.25f;
		}
		Sprite2D arrowHead2 = _arrowHead;
		val = targetPos - zero;
		((Node2D)arrowHead2).Rotation = ((Vector2)(ref val)).Angle() + (float)Math.PI / 2f;
		UpdateSegments(From, finalPos, zero);
	}

	public void SetHighlightingOn(bool isEnemy)
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		Tween? arrowHeadTween = _arrowHeadTween;
		if (arrowHeadTween != null)
		{
			arrowHeadTween.Kill();
		}
		_arrowHeadTween = ((Node)this).CreateTween();
		_arrowHeadTween.TweenProperty((GodotObject)(object)_arrowHead, NodePath.op_Implicit("scale"), Variant.op_Implicit(_arrowHeadHoverScale), 1.0).SetEase((EaseType)1).SetTrans((TransitionType)6);
		((CanvasItem)this).Modulate = (isEnemy ? StsColors.targetingArrowEnemy : StsColors.targetingArrowAlly);
	}

	public void SetHighlightingOff()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		Tween? arrowHeadTween = _arrowHeadTween;
		if (arrowHeadTween != null)
		{
			arrowHeadTween.Kill();
		}
		((Node2D)_arrowHead).Scale = _arrowHeadDefaultScale;
		((CanvasItem)this).Modulate = Colors.White;
	}

	private void UpdateSegments(Vector2 initialPos, Vector2 finalPos, Vector2 controlPoint)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		Vector2 val;
		for (int i = 0; i < 19; i++)
		{
			((Node2D)_segments[i]).Scale = Vector2.One * Mathf.Lerp(0.28f, 0.42f, (float)i * 2f / 19f);
			((Node2D)_segments[i]).Position = MathHelper.BezierCurve(initialPos, finalPos, controlPoint, (float)i / 20f);
			if (i == 0)
			{
				((Node2D)_segments[i]).Rotation = ((((Node2D)_segments[i]).GlobalPosition.Y > 540f) ? 0f : ((float)Math.PI));
				continue;
			}
			Sprite2D obj = _segments[i];
			val = ((Node2D)_segments[i]).Position - ((Node2D)_segments[i - 1]).Position;
			((Node2D)obj).Rotation = ((Vector2)(ref val)).Angle() + (float)Math.PI / 2f;
		}
		Sprite2D obj2 = _segments[0];
		val = ((Node2D)_segments[0]).Position - ((Node2D)_segments[1]).Position;
		((Node2D)obj2).Rotation = ((Vector2)(ref val)).Angle() - (float)Math.PI / 2f;
	}

	public void StartDrawingFrom(Vector2 from, bool usingController)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		_followMouse = !usingController;
		if (!NControllerManager.Instance.IsUsingController)
		{
			Input.MouseMode = (MouseModeEnum)1;
		}
		_fromPos = from;
		((CanvasItem)this).Visible = !NCombatUi.IsDebugHideTargetingUi;
	}

	public void StartDrawingFrom(Control control, bool usingController)
	{
		_followMouse = !usingController;
		((CanvasItem)this).ZIndex = ((CanvasItem)control).ZIndex + 1;
		Input.MouseMode = (MouseModeEnum)1;
		_fromControl = control;
		((CanvasItem)this).Visible = !NCombatUi.IsDebugHideTargetingUi;
	}

	public void StopDrawing()
	{
		if (!NControllerManager.Instance.IsUsingController)
		{
			Input.MouseMode = (MouseModeEnum)0;
		}
		_fromControl = null;
		_currentArrowPos = null;
		((CanvasItem)this).Visible = false;
		SetHighlightingOff();
	}

	public void UpdateDrawingTo(Vector2 position)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		_toPosition = position;
		if (!_followMouse && !_currentArrowPos.HasValue)
		{
			_currentArrowPos = _toPosition;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_0289: Unknown result type (might be due to invalid IL or missing references)
		//IL_0292: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02db: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(9);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Process, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)3, StringName.op_Implicit("delta"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UpdateArrowPosition, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)5, StringName.op_Implicit("targetPos"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetHighlightingOn, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)1, StringName.op_Implicit("isEnemy"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetHighlightingOff, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UpdateSegments, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)5, StringName.op_Implicit("initialPos"), (PropertyHint)0, "", (PropertyUsageFlags)6, false),
			new PropertyInfo((Type)5, StringName.op_Implicit("finalPos"), (PropertyHint)0, "", (PropertyUsageFlags)6, false),
			new PropertyInfo((Type)5, StringName.op_Implicit("controlPoint"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.StartDrawingFrom, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)5, StringName.op_Implicit("from"), (PropertyHint)0, "", (PropertyUsageFlags)6, false),
			new PropertyInfo((Type)1, StringName.op_Implicit("usingController"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.StopDrawing, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UpdateDrawingTo, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)5, StringName.op_Implicit("position"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
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
		if ((ref method) == MethodName.UpdateArrowPosition && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			UpdateArrowPosition(VariantUtils.ConvertTo<Vector2>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetHighlightingOn && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			SetHighlightingOn(VariantUtils.ConvertTo<bool>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetHighlightingOff && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			SetHighlightingOff();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.UpdateSegments && ((NativeVariantPtrArgs)(ref args)).Count == 3)
		{
			UpdateSegments(VariantUtils.ConvertTo<Vector2>(ref ((NativeVariantPtrArgs)(ref args))[0]), VariantUtils.ConvertTo<Vector2>(ref ((NativeVariantPtrArgs)(ref args))[1]), VariantUtils.ConvertTo<Vector2>(ref ((NativeVariantPtrArgs)(ref args))[2]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.StartDrawingFrom && ((NativeVariantPtrArgs)(ref args)).Count == 2)
		{
			StartDrawingFrom(VariantUtils.ConvertTo<Vector2>(ref ((NativeVariantPtrArgs)(ref args))[0]), VariantUtils.ConvertTo<bool>(ref ((NativeVariantPtrArgs)(ref args))[1]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.StopDrawing && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			StopDrawing();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.UpdateDrawingTo && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			UpdateDrawingTo(VariantUtils.ConvertTo<Vector2>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		return ((Node2D)this).InvokeGodotClassMethod(ref method, args, ref ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if ((ref method) == MethodName._Ready)
		{
			return true;
		}
		if ((ref method) == MethodName._Process)
		{
			return true;
		}
		if ((ref method) == MethodName.UpdateArrowPosition)
		{
			return true;
		}
		if ((ref method) == MethodName.SetHighlightingOn)
		{
			return true;
		}
		if ((ref method) == MethodName.SetHighlightingOff)
		{
			return true;
		}
		if ((ref method) == MethodName.UpdateSegments)
		{
			return true;
		}
		if ((ref method) == MethodName.StartDrawingFrom)
		{
			return true;
		}
		if ((ref method) == MethodName.StopDrawing)
		{
			return true;
		}
		if ((ref method) == MethodName.UpdateDrawingTo)
		{
			return true;
		}
		return ((Node2D)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName._fromPos)
		{
			_fromPos = VariantUtils.ConvertTo<Vector2>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._fromControl)
		{
			_fromControl = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._toPosition)
		{
			_toPosition = VariantUtils.ConvertTo<Vector2>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._segments)
		{
			_segments = VariantUtils.ConvertToSystemArrayOfGodotObject<Sprite2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._arrowHead)
		{
			_arrowHead = VariantUtils.ConvertTo<Sprite2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._arrowHeadTween)
		{
			_arrowHeadTween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._initialized)
		{
			_initialized = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._followMouse)
		{
			_followMouse = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		return ((GodotObject)this).SetGodotClassPropertyValue(ref name, ref value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName.From)
		{
			Vector2 val = From;
			value = VariantUtils.CreateFrom<Vector2>(ref val);
			return true;
		}
		if ((ref name) == PropertyName._fromPos)
		{
			value = VariantUtils.CreateFrom<Vector2>(ref _fromPos);
			return true;
		}
		if ((ref name) == PropertyName._fromControl)
		{
			value = VariantUtils.CreateFrom<Control>(ref _fromControl);
			return true;
		}
		if ((ref name) == PropertyName._toPosition)
		{
			value = VariantUtils.CreateFrom<Vector2>(ref _toPosition);
			return true;
		}
		if ((ref name) == PropertyName._segments)
		{
			GodotObject[] segments = (GodotObject[])(object)_segments;
			value = VariantUtils.CreateFromSystemArrayOfGodotObject(segments);
			return true;
		}
		if ((ref name) == PropertyName._arrowHead)
		{
			value = VariantUtils.CreateFrom<Sprite2D>(ref _arrowHead);
			return true;
		}
		if ((ref name) == PropertyName._arrowHeadTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _arrowHeadTween);
			return true;
		}
		if ((ref name) == PropertyName._initialized)
		{
			value = VariantUtils.CreateFrom<bool>(ref _initialized);
			return true;
		}
		if ((ref name) == PropertyName._followMouse)
		{
			value = VariantUtils.CreateFrom<bool>(ref _followMouse);
			return true;
		}
		return ((GodotObject)this).GetGodotClassPropertyValue(ref name, ref value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)5, PropertyName._fromPos, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._fromControl, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)5, PropertyName._toPosition, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)5, PropertyName.From, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)28, PropertyName._segments, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._arrowHead, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._arrowHeadTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._initialized, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._followMouse, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		info.AddProperty(PropertyName._fromPos, Variant.From<Vector2>(ref _fromPos));
		info.AddProperty(PropertyName._fromControl, Variant.From<Control>(ref _fromControl));
		info.AddProperty(PropertyName._toPosition, Variant.From<Vector2>(ref _toPosition));
		StringName segments = PropertyName._segments;
		GodotObject[] segments2 = (GodotObject[])(object)_segments;
		info.AddProperty(segments, Variant.CreateFrom(segments2));
		info.AddProperty(PropertyName._arrowHead, Variant.From<Sprite2D>(ref _arrowHead));
		info.AddProperty(PropertyName._arrowHeadTween, Variant.From<Tween>(ref _arrowHeadTween));
		info.AddProperty(PropertyName._initialized, Variant.From<bool>(ref _initialized));
		info.AddProperty(PropertyName._followMouse, Variant.From<bool>(ref _followMouse));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._fromPos, ref val))
		{
			_fromPos = ((Variant)(ref val)).As<Vector2>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._fromControl, ref val2))
		{
			_fromControl = ((Variant)(ref val2)).As<Control>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._toPosition, ref val3))
		{
			_toPosition = ((Variant)(ref val3)).As<Vector2>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._segments, ref val4))
		{
			_segments = ((Variant)(ref val4)).AsGodotObjectArray<Sprite2D>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._arrowHead, ref val5))
		{
			_arrowHead = ((Variant)(ref val5)).As<Sprite2D>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._arrowHeadTween, ref val6))
		{
			_arrowHeadTween = ((Variant)(ref val6)).As<Tween>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName._initialized, ref val7))
		{
			_initialized = ((Variant)(ref val7)).As<bool>();
		}
		Variant val8 = default(Variant);
		if (info.TryGetProperty(PropertyName._followMouse, ref val8))
		{
			_followMouse = ((Variant)(ref val8)).As<bool>();
		}
	}
}
