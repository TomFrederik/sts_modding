using System;
using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.Collections;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Random;

namespace MegaCrit.Sts2.Core.Nodes.Vfx;

[ScriptPath("res://src/Core/Nodes/Vfx/NDoomSubEmitterVfx.cs")]
public class NDoomSubEmitterVfx : Node2D
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName FireSpear = StringName.op_Implicit("FireSpear");

		public static readonly StringName FireAllSpears = StringName.op_Implicit("FireAllSpears");

		public static readonly StringName ShowOrHide = StringName.op_Implicit("ShowOrHide");

		public static readonly StringName UpdateWidth = StringName.op_Implicit("UpdateWidth");

		public static readonly StringName SetVisibility = StringName.op_Implicit("SetVisibility");

		public static readonly StringName _ExitTree = StringName.op_Implicit("_ExitTree");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName CurScaleX = StringName.op_Implicit("CurScaleX");

		public static readonly StringName _scalableLayers = StringName.op_Implicit("_scalableLayers");

		public static readonly StringName _spears = StringName.op_Implicit("_spears");

		public static readonly StringName _verticalShrinkingLayer = StringName.op_Implicit("_verticalShrinkingLayer");

		public static readonly StringName _particlesToKeepDense = StringName.op_Implicit("_particlesToKeepDense");

		public static readonly StringName _baseScales = StringName.op_Implicit("_baseScales");

		public static readonly StringName _indeces = StringName.op_Implicit("_indeces");

		public static readonly StringName _baseSpearRegionWidth = StringName.op_Implicit("_baseSpearRegionWidth");

		public static readonly StringName _dumbHackBecauseOfHowTexturerectsWork = StringName.op_Implicit("_dumbHackBecauseOfHowTexturerectsWork");

		public static readonly StringName _rotationHackForSameDumbReason = StringName.op_Implicit("_rotationHackForSameDumbReason");

		public static readonly StringName _baseParticleDensity = StringName.op_Implicit("_baseParticleDensity");

		public static readonly StringName _spearFixedHScale = StringName.op_Implicit("_spearFixedHScale");

		public static readonly StringName _spearAngleIntensity = StringName.op_Implicit("_spearAngleIntensity");

		public static readonly StringName _minSpearSize = StringName.op_Implicit("_minSpearSize");

		public static readonly StringName _maxSpearSize = StringName.op_Implicit("_maxSpearSize");

		public static readonly StringName _minSpearTime = StringName.op_Implicit("_minSpearTime");

		public static readonly StringName _maxSpearTime = StringName.op_Implicit("_maxSpearTime");

		public static readonly StringName _outerMargin = StringName.op_Implicit("_outerMargin");

		public static readonly StringName _innerMargin = StringName.op_Implicit("_innerMargin");

		public static readonly StringName _time = StringName.op_Implicit("_time");

		public static readonly StringName _isOn = StringName.op_Implicit("_isOn");

		public static readonly StringName _curScaleX = StringName.op_Implicit("_curScaleX");

		public static readonly StringName _tween = StringName.op_Implicit("_tween");
	}

	public class SignalName : SignalName
	{
	}

	[Export(/*Could not decode attribute arguments.*/)]
	private Array<Node2D> _scalableLayers;

	[Export(/*Could not decode attribute arguments.*/)]
	private Array<TextureRect> _spears;

	[Export(/*Could not decode attribute arguments.*/)]
	private Node2D _verticalShrinkingLayer;

	[Export(/*Could not decode attribute arguments.*/)]
	private GpuParticles2D _particlesToKeepDense;

	private Array<Vector2> _baseScales;

	private Array<int> _indeces;

	private float _baseSpearRegionWidth = 140f;

	private float _dumbHackBecauseOfHowTexturerectsWork = -20f;

	private float _rotationHackForSameDumbReason = 4f;

	private int _baseParticleDensity = 300;

	private float _spearFixedHScale = 0.4f;

	private float _spearAngleIntensity = 0.15f;

	private float _minSpearSize = 1f;

	private float _maxSpearSize = 1.5f;

	private float _minSpearTime = 0.3f;

	private float _maxSpearTime = 0.6f;

	private float _outerMargin = 0.2f;

	private float _innerMargin = -0.2f;

	private double _time = 2.0;

	private bool _isOn;

	private float _curScaleX = 1f;

	private Tween? _tween;

	public float CurScaleX
	{
		get
		{
			return _curScaleX;
		}
		set
		{
			_curScaleX = value;
			UpdateWidth(_curScaleX);
		}
	}

	public override void _Ready()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		_baseScales = new Array<Vector2>();
		_indeces = new Array<int>();
		foreach (Node2D scalableLayer in _scalableLayers)
		{
			_baseScales.Add(scalableLayer.Scale);
			_indeces.Add(((Node)scalableLayer).GetIndex(false));
		}
		_isOn = false;
		SetVisibility(isOn: false);
		UpdateWidth(0f);
		ShowOrHide(0f, 0f);
	}

	private void FireSpear(TextureRect textureRect = null)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		Vector2 position = ((Control)textureRect).Position;
		position.X = Rng.Chaotic.NextFloat(_baseSpearRegionWidth * -0.5f, _baseSpearRegionWidth * 0.5f) + _dumbHackBecauseOfHowTexturerectsWork;
		((Control)textureRect).Position = position;
		((Control)textureRect).RotationDegrees = ((Control)textureRect).Position.X * _spearAngleIntensity + _rotationHackForSameDumbReason;
		((CanvasItem)textureRect).Modulate = Colors.Transparent;
		float num = _spearFixedHScale / _curScaleX;
		((Control)textureRect).Scale = new Vector2(num, Rng.Chaotic.NextFloat(_minSpearSize, _maxSpearSize));
		Vector2 val = default(Vector2);
		((Vector2)(ref val))._002Ector(num, 0f);
		float num2 = Rng.Chaotic.NextFloat(_minSpearTime, _maxSpearTime);
		Tween val2 = ((Node)textureRect).CreateTween();
		val2.TweenProperty((GodotObject)(object)textureRect, NodePath.op_Implicit("scale"), Variant.op_Implicit(val), (double)num2).From(Variant.op_Implicit(new Vector2(num, Rng.Chaotic.NextFloat(_minSpearSize, _maxSpearSize))));
		if (_isOn)
		{
			val2.TweenCallback(Callable.From((Action)delegate
			{
				FireSpear(textureRect);
			}));
		}
		Tween val3 = ((Node)textureRect).CreateTween();
		float num3 = Rng.Chaotic.NextFloat(0.4f, 0.7f);
		val3.TweenProperty((GodotObject)(object)textureRect, NodePath.op_Implicit("modulate"), Variant.op_Implicit(new Color(1f, 1f, 1f, num3)), 0.20000000298023224).SetEase((EaseType)0).SetTrans((TransitionType)4);
	}

	private void FireAllSpears()
	{
		foreach (TextureRect spear in _spears)
		{
			FireSpear(spear);
		}
	}

	public void ShowOrHide(float widthScale, float tweenTime)
	{
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		_isOn = widthScale > 0.1f;
		float num = (_isOn ? 0f : 0.3f);
		float num2 = widthScale;
		EaseType ease;
		if (_isOn)
		{
			SetVisibility(isOn: true);
			ease = (EaseType)1;
			int amount = (((int)widthScale * _baseParticleDensity <= 1) ? 1 : ((int)widthScale * _baseParticleDensity));
			_particlesToKeepDense.Amount = amount;
		}
		else
		{
			ease = (EaseType)0;
			num2 = 0f;
			tweenTime = _curScaleX * 0.15f;
		}
		Tween? tween = _tween;
		if (tween != null)
		{
			tween.Kill();
		}
		_tween = ((Node)this).CreateTween();
		_tween.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("CurScaleX"), Variant.op_Implicit(num2), (double)tweenTime).SetDelay((double)num).SetEase(ease)
			.SetTrans((TransitionType)7);
		if (!_isOn)
		{
			_tween.TweenCallback(Callable.From((Action)delegate
			{
				SetVisibility(isOn: false);
			}));
		}
	}

	private void UpdateWidth(float width)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		_curScaleX = width;
		int num = 0;
		Vector2 scale = default(Vector2);
		foreach (Node2D scalableLayer in _scalableLayers)
		{
			Vector2 val = _baseScales[num];
			if (scalableLayer == _verticalShrinkingLayer && width < 1f)
			{
				((Vector2)(ref scale))._002Ector(val.X * _curScaleX, val.Y * width / 1f);
			}
			else
			{
				((Vector2)(ref scale))._002Ector(val.X * _curScaleX, val.Y);
			}
			scalableLayer.Scale = scale;
			num++;
		}
	}

	private void SetVisibility(bool isOn)
	{
		int num = 0;
		foreach (Node2D scalableLayer in _scalableLayers)
		{
			GpuParticles2D val = (GpuParticles2D)(object)((scalableLayer is GpuParticles2D) ? scalableLayer : null);
			if (val != null)
			{
				if (isOn)
				{
					val.Restart();
				}
				else
				{
					val.Emitting = false;
				}
			}
			else if (isOn)
			{
				((Node)this).MoveChild((Node)(object)scalableLayer, _indeces[num]);
			}
			num++;
		}
		if (isOn)
		{
			FireAllSpears();
		}
	}

	public override void _ExitTree()
	{
		Tween? tween = _tween;
		if (tween != null)
		{
			tween.Kill();
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Expected O, but got Unknown
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(7);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.FireSpear, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("textureRect"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("TextureRect"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.FireAllSpears, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ShowOrHide, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)3, StringName.op_Implicit("widthScale"), (PropertyHint)0, "", (PropertyUsageFlags)6, false),
			new PropertyInfo((Type)3, StringName.op_Implicit("tweenTime"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UpdateWidth, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)3, StringName.op_Implicit("width"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetVisibility, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)1, StringName.op_Implicit("isOn"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._ExitTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.FireSpear && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			FireSpear(VariantUtils.ConvertTo<TextureRect>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.FireAllSpears && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			FireAllSpears();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.ShowOrHide && ((NativeVariantPtrArgs)(ref args)).Count == 2)
		{
			ShowOrHide(VariantUtils.ConvertTo<float>(ref ((NativeVariantPtrArgs)(ref args))[0]), VariantUtils.ConvertTo<float>(ref ((NativeVariantPtrArgs)(ref args))[1]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.UpdateWidth && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			UpdateWidth(VariantUtils.ConvertTo<float>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetVisibility && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			SetVisibility(VariantUtils.ConvertTo<bool>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._ExitTree && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._ExitTree();
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
		if ((ref method) == MethodName.FireSpear)
		{
			return true;
		}
		if ((ref method) == MethodName.FireAllSpears)
		{
			return true;
		}
		if ((ref method) == MethodName.ShowOrHide)
		{
			return true;
		}
		if ((ref method) == MethodName.UpdateWidth)
		{
			return true;
		}
		if ((ref method) == MethodName.SetVisibility)
		{
			return true;
		}
		if ((ref method) == MethodName._ExitTree)
		{
			return true;
		}
		return ((Node2D)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName.CurScaleX)
		{
			CurScaleX = VariantUtils.ConvertTo<float>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._scalableLayers)
		{
			_scalableLayers = VariantUtils.ConvertToArray<Node2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._spears)
		{
			_spears = VariantUtils.ConvertToArray<TextureRect>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._verticalShrinkingLayer)
		{
			_verticalShrinkingLayer = VariantUtils.ConvertTo<Node2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._particlesToKeepDense)
		{
			_particlesToKeepDense = VariantUtils.ConvertTo<GpuParticles2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._baseScales)
		{
			_baseScales = VariantUtils.ConvertToArray<Vector2>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._indeces)
		{
			_indeces = VariantUtils.ConvertToArray<int>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._baseSpearRegionWidth)
		{
			_baseSpearRegionWidth = VariantUtils.ConvertTo<float>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._dumbHackBecauseOfHowTexturerectsWork)
		{
			_dumbHackBecauseOfHowTexturerectsWork = VariantUtils.ConvertTo<float>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._rotationHackForSameDumbReason)
		{
			_rotationHackForSameDumbReason = VariantUtils.ConvertTo<float>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._baseParticleDensity)
		{
			_baseParticleDensity = VariantUtils.ConvertTo<int>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._spearFixedHScale)
		{
			_spearFixedHScale = VariantUtils.ConvertTo<float>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._spearAngleIntensity)
		{
			_spearAngleIntensity = VariantUtils.ConvertTo<float>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._minSpearSize)
		{
			_minSpearSize = VariantUtils.ConvertTo<float>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._maxSpearSize)
		{
			_maxSpearSize = VariantUtils.ConvertTo<float>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._minSpearTime)
		{
			_minSpearTime = VariantUtils.ConvertTo<float>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._maxSpearTime)
		{
			_maxSpearTime = VariantUtils.ConvertTo<float>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._outerMargin)
		{
			_outerMargin = VariantUtils.ConvertTo<float>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._innerMargin)
		{
			_innerMargin = VariantUtils.ConvertTo<float>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._time)
		{
			_time = VariantUtils.ConvertTo<double>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._isOn)
		{
			_isOn = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._curScaleX)
		{
			_curScaleX = VariantUtils.ConvertTo<float>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._tween)
		{
			_tween = VariantUtils.ConvertTo<Tween>(ref value);
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
		//IL_0277: Unknown result type (might be due to invalid IL or missing references)
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0297: Unknown result type (might be due to invalid IL or missing references)
		//IL_029c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName.CurScaleX)
		{
			float curScaleX = CurScaleX;
			value = VariantUtils.CreateFrom<float>(ref curScaleX);
			return true;
		}
		if ((ref name) == PropertyName._scalableLayers)
		{
			value = VariantUtils.CreateFromArray<Node2D>(_scalableLayers);
			return true;
		}
		if ((ref name) == PropertyName._spears)
		{
			value = VariantUtils.CreateFromArray<TextureRect>(_spears);
			return true;
		}
		if ((ref name) == PropertyName._verticalShrinkingLayer)
		{
			value = VariantUtils.CreateFrom<Node2D>(ref _verticalShrinkingLayer);
			return true;
		}
		if ((ref name) == PropertyName._particlesToKeepDense)
		{
			value = VariantUtils.CreateFrom<GpuParticles2D>(ref _particlesToKeepDense);
			return true;
		}
		if ((ref name) == PropertyName._baseScales)
		{
			value = VariantUtils.CreateFromArray<Vector2>(_baseScales);
			return true;
		}
		if ((ref name) == PropertyName._indeces)
		{
			value = VariantUtils.CreateFromArray<int>(_indeces);
			return true;
		}
		if ((ref name) == PropertyName._baseSpearRegionWidth)
		{
			value = VariantUtils.CreateFrom<float>(ref _baseSpearRegionWidth);
			return true;
		}
		if ((ref name) == PropertyName._dumbHackBecauseOfHowTexturerectsWork)
		{
			value = VariantUtils.CreateFrom<float>(ref _dumbHackBecauseOfHowTexturerectsWork);
			return true;
		}
		if ((ref name) == PropertyName._rotationHackForSameDumbReason)
		{
			value = VariantUtils.CreateFrom<float>(ref _rotationHackForSameDumbReason);
			return true;
		}
		if ((ref name) == PropertyName._baseParticleDensity)
		{
			value = VariantUtils.CreateFrom<int>(ref _baseParticleDensity);
			return true;
		}
		if ((ref name) == PropertyName._spearFixedHScale)
		{
			value = VariantUtils.CreateFrom<float>(ref _spearFixedHScale);
			return true;
		}
		if ((ref name) == PropertyName._spearAngleIntensity)
		{
			value = VariantUtils.CreateFrom<float>(ref _spearAngleIntensity);
			return true;
		}
		if ((ref name) == PropertyName._minSpearSize)
		{
			value = VariantUtils.CreateFrom<float>(ref _minSpearSize);
			return true;
		}
		if ((ref name) == PropertyName._maxSpearSize)
		{
			value = VariantUtils.CreateFrom<float>(ref _maxSpearSize);
			return true;
		}
		if ((ref name) == PropertyName._minSpearTime)
		{
			value = VariantUtils.CreateFrom<float>(ref _minSpearTime);
			return true;
		}
		if ((ref name) == PropertyName._maxSpearTime)
		{
			value = VariantUtils.CreateFrom<float>(ref _maxSpearTime);
			return true;
		}
		if ((ref name) == PropertyName._outerMargin)
		{
			value = VariantUtils.CreateFrom<float>(ref _outerMargin);
			return true;
		}
		if ((ref name) == PropertyName._innerMargin)
		{
			value = VariantUtils.CreateFrom<float>(ref _innerMargin);
			return true;
		}
		if ((ref name) == PropertyName._time)
		{
			value = VariantUtils.CreateFrom<double>(ref _time);
			return true;
		}
		if ((ref name) == PropertyName._isOn)
		{
			value = VariantUtils.CreateFrom<bool>(ref _isOn);
			return true;
		}
		if ((ref name) == PropertyName._curScaleX)
		{
			value = VariantUtils.CreateFrom<float>(ref _curScaleX);
			return true;
		}
		if ((ref name) == PropertyName._tween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _tween);
			return true;
		}
		return ((GodotObject)this).GetGodotClassPropertyValue(ref name, ref value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_0286: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)28, PropertyName._scalableLayers, (PropertyHint)23, "24/34:Node2D", (PropertyUsageFlags)4102, true));
		list.Add(new PropertyInfo((Type)28, PropertyName._spears, (PropertyHint)23, "24/34:TextureRect", (PropertyUsageFlags)4102, true));
		list.Add(new PropertyInfo((Type)24, PropertyName._verticalShrinkingLayer, (PropertyHint)34, "Node2D", (PropertyUsageFlags)4102, true));
		list.Add(new PropertyInfo((Type)24, PropertyName._particlesToKeepDense, (PropertyHint)34, "GPUParticles2D", (PropertyUsageFlags)4102, true));
		list.Add(new PropertyInfo((Type)28, PropertyName._baseScales, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)28, PropertyName._indeces, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)3, PropertyName._baseSpearRegionWidth, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)3, PropertyName._dumbHackBecauseOfHowTexturerectsWork, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)3, PropertyName._rotationHackForSameDumbReason, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)2, PropertyName._baseParticleDensity, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)3, PropertyName._spearFixedHScale, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)3, PropertyName._spearAngleIntensity, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)3, PropertyName._minSpearSize, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)3, PropertyName._maxSpearSize, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)3, PropertyName._minSpearTime, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)3, PropertyName._maxSpearTime, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)3, PropertyName._outerMargin, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)3, PropertyName._innerMargin, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)3, PropertyName._time, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._isOn, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)3, PropertyName._curScaleX, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)3, PropertyName.CurScaleX, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._tween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		StringName curScaleX = PropertyName.CurScaleX;
		float curScaleX2 = CurScaleX;
		info.AddProperty(curScaleX, Variant.From<float>(ref curScaleX2));
		info.AddProperty(PropertyName._scalableLayers, Variant.CreateFrom<Node2D>(_scalableLayers));
		info.AddProperty(PropertyName._spears, Variant.CreateFrom<TextureRect>(_spears));
		info.AddProperty(PropertyName._verticalShrinkingLayer, Variant.From<Node2D>(ref _verticalShrinkingLayer));
		info.AddProperty(PropertyName._particlesToKeepDense, Variant.From<GpuParticles2D>(ref _particlesToKeepDense));
		info.AddProperty(PropertyName._baseScales, Variant.CreateFrom<Vector2>(_baseScales));
		info.AddProperty(PropertyName._indeces, Variant.CreateFrom<int>(_indeces));
		info.AddProperty(PropertyName._baseSpearRegionWidth, Variant.From<float>(ref _baseSpearRegionWidth));
		info.AddProperty(PropertyName._dumbHackBecauseOfHowTexturerectsWork, Variant.From<float>(ref _dumbHackBecauseOfHowTexturerectsWork));
		info.AddProperty(PropertyName._rotationHackForSameDumbReason, Variant.From<float>(ref _rotationHackForSameDumbReason));
		info.AddProperty(PropertyName._baseParticleDensity, Variant.From<int>(ref _baseParticleDensity));
		info.AddProperty(PropertyName._spearFixedHScale, Variant.From<float>(ref _spearFixedHScale));
		info.AddProperty(PropertyName._spearAngleIntensity, Variant.From<float>(ref _spearAngleIntensity));
		info.AddProperty(PropertyName._minSpearSize, Variant.From<float>(ref _minSpearSize));
		info.AddProperty(PropertyName._maxSpearSize, Variant.From<float>(ref _maxSpearSize));
		info.AddProperty(PropertyName._minSpearTime, Variant.From<float>(ref _minSpearTime));
		info.AddProperty(PropertyName._maxSpearTime, Variant.From<float>(ref _maxSpearTime));
		info.AddProperty(PropertyName._outerMargin, Variant.From<float>(ref _outerMargin));
		info.AddProperty(PropertyName._innerMargin, Variant.From<float>(ref _innerMargin));
		info.AddProperty(PropertyName._time, Variant.From<double>(ref _time));
		info.AddProperty(PropertyName._isOn, Variant.From<bool>(ref _isOn));
		info.AddProperty(PropertyName._curScaleX, Variant.From<float>(ref _curScaleX));
		info.AddProperty(PropertyName._tween, Variant.From<Tween>(ref _tween));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName.CurScaleX, ref val))
		{
			CurScaleX = ((Variant)(ref val)).As<float>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._scalableLayers, ref val2))
		{
			_scalableLayers = ((Variant)(ref val2)).AsGodotArray<Node2D>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._spears, ref val3))
		{
			_spears = ((Variant)(ref val3)).AsGodotArray<TextureRect>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._verticalShrinkingLayer, ref val4))
		{
			_verticalShrinkingLayer = ((Variant)(ref val4)).As<Node2D>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._particlesToKeepDense, ref val5))
		{
			_particlesToKeepDense = ((Variant)(ref val5)).As<GpuParticles2D>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._baseScales, ref val6))
		{
			_baseScales = ((Variant)(ref val6)).AsGodotArray<Vector2>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName._indeces, ref val7))
		{
			_indeces = ((Variant)(ref val7)).AsGodotArray<int>();
		}
		Variant val8 = default(Variant);
		if (info.TryGetProperty(PropertyName._baseSpearRegionWidth, ref val8))
		{
			_baseSpearRegionWidth = ((Variant)(ref val8)).As<float>();
		}
		Variant val9 = default(Variant);
		if (info.TryGetProperty(PropertyName._dumbHackBecauseOfHowTexturerectsWork, ref val9))
		{
			_dumbHackBecauseOfHowTexturerectsWork = ((Variant)(ref val9)).As<float>();
		}
		Variant val10 = default(Variant);
		if (info.TryGetProperty(PropertyName._rotationHackForSameDumbReason, ref val10))
		{
			_rotationHackForSameDumbReason = ((Variant)(ref val10)).As<float>();
		}
		Variant val11 = default(Variant);
		if (info.TryGetProperty(PropertyName._baseParticleDensity, ref val11))
		{
			_baseParticleDensity = ((Variant)(ref val11)).As<int>();
		}
		Variant val12 = default(Variant);
		if (info.TryGetProperty(PropertyName._spearFixedHScale, ref val12))
		{
			_spearFixedHScale = ((Variant)(ref val12)).As<float>();
		}
		Variant val13 = default(Variant);
		if (info.TryGetProperty(PropertyName._spearAngleIntensity, ref val13))
		{
			_spearAngleIntensity = ((Variant)(ref val13)).As<float>();
		}
		Variant val14 = default(Variant);
		if (info.TryGetProperty(PropertyName._minSpearSize, ref val14))
		{
			_minSpearSize = ((Variant)(ref val14)).As<float>();
		}
		Variant val15 = default(Variant);
		if (info.TryGetProperty(PropertyName._maxSpearSize, ref val15))
		{
			_maxSpearSize = ((Variant)(ref val15)).As<float>();
		}
		Variant val16 = default(Variant);
		if (info.TryGetProperty(PropertyName._minSpearTime, ref val16))
		{
			_minSpearTime = ((Variant)(ref val16)).As<float>();
		}
		Variant val17 = default(Variant);
		if (info.TryGetProperty(PropertyName._maxSpearTime, ref val17))
		{
			_maxSpearTime = ((Variant)(ref val17)).As<float>();
		}
		Variant val18 = default(Variant);
		if (info.TryGetProperty(PropertyName._outerMargin, ref val18))
		{
			_outerMargin = ((Variant)(ref val18)).As<float>();
		}
		Variant val19 = default(Variant);
		if (info.TryGetProperty(PropertyName._innerMargin, ref val19))
		{
			_innerMargin = ((Variant)(ref val19)).As<float>();
		}
		Variant val20 = default(Variant);
		if (info.TryGetProperty(PropertyName._time, ref val20))
		{
			_time = ((Variant)(ref val20)).As<double>();
		}
		Variant val21 = default(Variant);
		if (info.TryGetProperty(PropertyName._isOn, ref val21))
		{
			_isOn = ((Variant)(ref val21)).As<bool>();
		}
		Variant val22 = default(Variant);
		if (info.TryGetProperty(PropertyName._curScaleX, ref val22))
		{
			_curScaleX = ((Variant)(ref val22)).As<float>();
		}
		Variant val23 = default(Variant);
		if (info.TryGetProperty(PropertyName._tween, ref val23))
		{
			_tween = ((Variant)(ref val23)).As<Tween>();
		}
	}
}
