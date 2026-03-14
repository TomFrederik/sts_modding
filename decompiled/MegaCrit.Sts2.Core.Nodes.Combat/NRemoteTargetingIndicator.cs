using System;
using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace MegaCrit.Sts2.Core.Nodes.Combat;

[ScriptPath("res://src/Core/Nodes/Combat/NRemoteTargetingIndicator.cs")]
public class NRemoteTargetingIndicator : Node2D
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName _Process = StringName.op_Implicit("_Process");

		public static readonly StringName StartDrawingFrom = StringName.op_Implicit("StartDrawingFrom");

		public static readonly StringName StopDrawing = StringName.op_Implicit("StopDrawing");

		public static readonly StringName UpdateDrawingTo = StringName.op_Implicit("UpdateDrawingTo");

		public static readonly StringName DoTargetingCreatureTween = StringName.op_Implicit("DoTargetingCreatureTween");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName _fromPosition = StringName.op_Implicit("_fromPosition");

		public static readonly StringName _toPosition = StringName.op_Implicit("_toPosition");

		public static readonly StringName _line = StringName.op_Implicit("_line");

		public static readonly StringName _lineBack = StringName.op_Implicit("_lineBack");

		public static readonly StringName _tween = StringName.op_Implicit("_tween");

		public static readonly StringName _isTargetingCreature = StringName.op_Implicit("_isTargetingCreature");
	}

	public class SignalName : SignalName
	{
	}

	private const int _segmentCount = 100;

	private const float _defaultAlpha = 0.5f;

	private const float _targetingAlpha = 1f;

	private Player _player;

	private Vector2 _fromPosition;

	private Vector2 _toPosition;

	private Line2D _line;

	private Line2D _lineBack;

	private Tween? _tween;

	private bool _isTargetingCreature;

	public override void _Ready()
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		_line = ((Node)this).GetNode<Line2D>(NodePath.op_Implicit("Line"));
		_lineBack = ((Node)this).GetNode<Line2D>(NodePath.op_Implicit("LineBack"));
		for (int i = 0; i < 101; i++)
		{
			_line.AddPoint(Vector2.Zero, -1);
			_lineBack.AddPoint(Vector2.Zero, -1);
		}
		StopDrawing();
	}

	public void Initialize(Player player)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		_player = player;
		CharacterModel character = player.Character;
		_line.DefaultColor = character.RemoteTargetingLineColor;
		_lineBack.DefaultColor = character.RemoteTargetingLineOutline;
		Gradient gradient = _line.GetGradient();
		if (gradient != null)
		{
			for (int i = 0; i < gradient.GetPointCount(); i++)
			{
				gradient.SetColor(i, gradient.GetColor(i) * character.RemoteTargetingLineColor);
			}
			_line.SetGradient(gradient);
		}
		Gradient gradient2 = _lineBack.GetGradient();
		if (gradient2 != null)
		{
			for (int j = 0; j < gradient2.GetPointCount(); j++)
			{
				gradient2.SetColor(j, gradient2.GetColor(j) * character.RemoteTargetingLineOutline);
			}
			_lineBack.SetGradient(gradient);
		}
	}

	public override void _Process(double delta)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		Vector2 zero = Vector2.Zero;
		zero.X = _fromPosition.X + (_toPosition.X - _fromPosition.X) * 0.5f;
		zero.Y = _fromPosition.Y - (_toPosition.Y - _fromPosition.Y) * 0.5f;
		for (int i = 0; i < 100; i++)
		{
			Vector2 val = MathHelper.BezierCurve(_fromPosition, _toPosition, zero, (float)i / 101f);
			_line.SetPointPosition(i, val);
			_lineBack.SetPointPosition(i, val);
		}
		_line.SetPointPosition(100, _toPosition);
		_lineBack.SetPointPosition(100, _toPosition);
		bool isTargetingCreature = false;
		foreach (Creature item in _player.Creature.CombatState?.Enemies ?? Array.Empty<Creature>())
		{
			NCreature nCreature = NCombatRoom.Instance?.GetCreatureNode(item);
			if (nCreature != null)
			{
				Rect2 globalRect = nCreature.Hitbox.GetGlobalRect();
				if (((Rect2)(ref globalRect)).HasPoint(((Node2D)this).GlobalPosition + _toPosition))
				{
					isTargetingCreature = true;
					break;
				}
			}
		}
		DoTargetingCreatureTween(isTargetingCreature);
	}

	public void StartDrawingFrom(Vector2 from)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		if (!NCombatUi.IsDebugHideMpTargetingUi)
		{
			_fromPosition = from;
			((CanvasItem)this).Visible = true;
			((Node)this).ProcessMode = (ProcessModeEnum)(((CanvasItem)this).Visible ? 0 : 4);
		}
	}

	public void StopDrawing()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		((CanvasItem)this).Visible = false;
		((Node)this).ProcessMode = (ProcessModeEnum)4;
		Color modulate = ((CanvasItem)this).Modulate;
		modulate.A = 0.5f;
		((CanvasItem)this).Modulate = modulate;
	}

	public void UpdateDrawingTo(Vector2 position)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		_toPosition = position;
	}

	private void DoTargetingCreatureTween(bool isTargetingCreature)
	{
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		if (isTargetingCreature != _isTargetingCreature)
		{
			Tween? tween = _tween;
			if (tween != null)
			{
				tween.Kill();
			}
			_tween = ((Node)this).CreateTween();
			if (isTargetingCreature)
			{
				_tween.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(1f), 0.10000000149011612);
			}
			else
			{
				_tween.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0.5f), 0.25);
			}
			_isTargetingCreature = isTargetingCreature;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(6);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Process, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)3, StringName.op_Implicit("delta"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.StartDrawingFrom, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)5, StringName.op_Implicit("from"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.StopDrawing, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UpdateDrawingTo, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)5, StringName.op_Implicit("position"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.DoTargetingCreatureTween, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)1, StringName.op_Implicit("isTargetingCreature"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
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
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
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
		if ((ref method) == MethodName.StartDrawingFrom && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			StartDrawingFrom(VariantUtils.ConvertTo<Vector2>(ref ((NativeVariantPtrArgs)(ref args))[0]));
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
		if ((ref method) == MethodName.DoTargetingCreatureTween && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			DoTargetingCreatureTween(VariantUtils.ConvertTo<bool>(ref ((NativeVariantPtrArgs)(ref args))[0]));
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
		if ((ref method) == MethodName.DoTargetingCreatureTween)
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
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName._fromPosition)
		{
			_fromPosition = VariantUtils.ConvertTo<Vector2>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._toPosition)
		{
			_toPosition = VariantUtils.ConvertTo<Vector2>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._line)
		{
			_line = VariantUtils.ConvertTo<Line2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._lineBack)
		{
			_lineBack = VariantUtils.ConvertTo<Line2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._tween)
		{
			_tween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._isTargetingCreature)
		{
			_isTargetingCreature = VariantUtils.ConvertTo<bool>(ref value);
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
		if ((ref name) == PropertyName._fromPosition)
		{
			value = VariantUtils.CreateFrom<Vector2>(ref _fromPosition);
			return true;
		}
		if ((ref name) == PropertyName._toPosition)
		{
			value = VariantUtils.CreateFrom<Vector2>(ref _toPosition);
			return true;
		}
		if ((ref name) == PropertyName._line)
		{
			value = VariantUtils.CreateFrom<Line2D>(ref _line);
			return true;
		}
		if ((ref name) == PropertyName._lineBack)
		{
			value = VariantUtils.CreateFrom<Line2D>(ref _lineBack);
			return true;
		}
		if ((ref name) == PropertyName._tween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _tween);
			return true;
		}
		if ((ref name) == PropertyName._isTargetingCreature)
		{
			value = VariantUtils.CreateFrom<bool>(ref _isTargetingCreature);
			return true;
		}
		return ((GodotObject)this).GetGodotClassPropertyValue(ref name, ref value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)5, PropertyName._fromPosition, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)5, PropertyName._toPosition, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._line, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._lineBack, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._tween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._isTargetingCreature, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		((GodotObject)this).SaveGodotObjectData(info);
		info.AddProperty(PropertyName._fromPosition, Variant.From<Vector2>(ref _fromPosition));
		info.AddProperty(PropertyName._toPosition, Variant.From<Vector2>(ref _toPosition));
		info.AddProperty(PropertyName._line, Variant.From<Line2D>(ref _line));
		info.AddProperty(PropertyName._lineBack, Variant.From<Line2D>(ref _lineBack));
		info.AddProperty(PropertyName._tween, Variant.From<Tween>(ref _tween));
		info.AddProperty(PropertyName._isTargetingCreature, Variant.From<bool>(ref _isTargetingCreature));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._fromPosition, ref val))
		{
			_fromPosition = ((Variant)(ref val)).As<Vector2>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._toPosition, ref val2))
		{
			_toPosition = ((Variant)(ref val2)).As<Vector2>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._line, ref val3))
		{
			_line = ((Variant)(ref val3)).As<Line2D>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._lineBack, ref val4))
		{
			_lineBack = ((Variant)(ref val4)).As<Line2D>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._tween, ref val5))
		{
			_tween = ((Variant)(ref val5)).As<Tween>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._isTargetingCreature, ref val6))
		{
			_isTargetingCreature = ((Variant)(ref val6)).As<bool>();
		}
	}
}
