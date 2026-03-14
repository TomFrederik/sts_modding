using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.TestSupport;

namespace MegaCrit.Sts2.Core.Nodes.Vfx;

[ScriptPath("res://src/Core/Nodes/Vfx/NCardFlyShuffleVfx.cs")]
public class NCardFlyShuffleVfx : Control
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName _ExitTree = StringName.op_Implicit("_ExitTree");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName _vfx = StringName.op_Implicit("_vfx");

		public static readonly StringName _fadeOutTween = StringName.op_Implicit("_fadeOutTween");

		public static readonly StringName _vfxFading = StringName.op_Implicit("_vfxFading");

		public static readonly StringName _startPos = StringName.op_Implicit("_startPos");

		public static readonly StringName _endPos = StringName.op_Implicit("_endPos");

		public static readonly StringName _controlPointOffset = StringName.op_Implicit("_controlPointOffset");

		public static readonly StringName _duration = StringName.op_Implicit("_duration");

		public static readonly StringName _speed = StringName.op_Implicit("_speed");

		public static readonly StringName _accel = StringName.op_Implicit("_accel");

		public static readonly StringName _arcDir = StringName.op_Implicit("_arcDir");

		public static readonly StringName _trailPath = StringName.op_Implicit("_trailPath");
	}

	public class SignalName : SignalName
	{
	}

	private NCardTrailVfx? _vfx;

	private Tween? _fadeOutTween;

	private bool _vfxFading;

	private Vector2 _startPos;

	private Vector2 _endPos;

	private float _controlPointOffset;

	private float _duration;

	private float _speed;

	private float _accel;

	private float _arcDir;

	private string _trailPath;

	private CardPile _targetPile;

	private readonly CancellationTokenSource _cancelToken = new CancellationTokenSource();

	private static readonly string _scenePath = SceneHelper.GetScenePath("vfx/vfx_card_shuffle_fly");

	public static IEnumerable<string> AssetPaths => new _003C_003Ez__ReadOnlySingleElementList<string>(_scenePath);

	public static NCardFlyShuffleVfx? Create(CardPile startPile, CardPile targetPile, string trailPath)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		if (TestMode.IsOn)
		{
			return null;
		}
		NCardFlyShuffleVfx nCardFlyShuffleVfx = PreloadManager.Cache.GetScene(_scenePath).Instantiate<NCardFlyShuffleVfx>((GenEditState)0);
		nCardFlyShuffleVfx._startPos = startPile.Type.GetTargetPosition(null);
		nCardFlyShuffleVfx._endPos = targetPile.Type.GetTargetPosition(null);
		nCardFlyShuffleVfx._trailPath = trailPath;
		nCardFlyShuffleVfx._targetPile = targetPile;
		return nCardFlyShuffleVfx;
	}

	public override void _Ready()
	{
		_controlPointOffset = Rng.Chaotic.NextFloat(-300f, 400f);
		_speed = Rng.Chaotic.NextFloat(1.1f, 1.25f);
		_accel = Rng.Chaotic.NextFloat(2f, 2.5f);
		_arcDir = ((_endPos.Y < 540f) ? (-500f) : (500f + _controlPointOffset));
		_duration = Rng.Chaotic.NextFloat(1f, 1.75f);
		_vfx = NCardTrailVfx.Create((Control)(object)this, _trailPath);
		if (_vfx != null)
		{
			((Node)(object)NCombatRoom.Instance.CombatVfxContainer).AddChildSafely((Node?)(object)_vfx);
		}
		Node parent = ((Node)this).GetParent();
		parent.MoveChild((Node)(object)this, parent.GetChildCount(false) - 1);
		TaskHelper.RunSafely(PlayAnim());
	}

	private async Task PlayAnim()
	{
		float time = 0f;
		while (time / _duration <= 1f)
		{
			await ((GodotObject)this).ToSignal((GodotObject)(object)((Node)this).GetTree(), SignalName.ProcessFrame);
			if (_cancelToken.IsCancellationRequested)
			{
				return;
			}
			float num = (float)((Node)this).GetProcessDeltaTime();
			time += _speed * num;
			_speed += _accel * num;
			Vector2 c = _startPos + (_endPos - _startPos) * 0.5f;
			c.Y -= _arcDir;
			((Control)this).GlobalPosition = MathHelper.BezierCurve(_startPos, _endPos, c, time / _duration);
			Vector2 val = MathHelper.BezierCurve(_startPos, _endPos, c, (time + 0.05f) / _duration);
			NCardFlyShuffleVfx nCardFlyShuffleVfx = this;
			Vector2 val2 = val - ((Control)this).GlobalPosition;
			((Control)nCardFlyShuffleVfx).Rotation = ((Vector2)(ref val2)).Angle() + (float)Math.PI / 2f;
		}
		((Control)this).GlobalPosition = _endPos;
		_targetPile.InvokeCardAddFinished();
		time = 0f;
		while (time / _duration <= 1f)
		{
			await ((GodotObject)this).ToSignal((GodotObject)(object)((Node)this).GetTree(), SignalName.ProcessFrame);
			if (_cancelToken.IsCancellationRequested)
			{
				return;
			}
			float num2 = (float)((Node)this).GetProcessDeltaTime();
			time += _speed * num2;
			if (time / _duration > 0.25f && !_vfxFading)
			{
				if (_vfx != null)
				{
					TaskHelper.RunSafely(_vfx.FadeOut());
				}
				_vfxFading = true;
			}
			((Control)this).Scale = Vector2.One * Mathf.Max(Mathf.Lerp(0.1f, -0.1f, time / _duration), 0f);
		}
		_fadeOutTween = ((Node)this).CreateTween();
		_fadeOutTween.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0f), 0.800000011920929);
		await Task.Delay(800);
		((Node)(object)this).QueueFreeSafely();
	}

	public override void _ExitTree()
	{
		Tween? fadeOutTween = _fadeOutTween;
		if (fadeOutTween != null)
		{
			fadeOutTween.Kill();
		}
		_cancelToken.Cancel();
		_cancelToken.Dispose();
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(2);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._ExitTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._ExitTree && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._ExitTree();
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
		if ((ref method) == MethodName._ExitTree)
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
		if ((ref name) == PropertyName._vfx)
		{
			_vfx = VariantUtils.ConvertTo<NCardTrailVfx>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._fadeOutTween)
		{
			_fadeOutTween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._vfxFading)
		{
			_vfxFading = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._startPos)
		{
			_startPos = VariantUtils.ConvertTo<Vector2>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._endPos)
		{
			_endPos = VariantUtils.ConvertTo<Vector2>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._controlPointOffset)
		{
			_controlPointOffset = VariantUtils.ConvertTo<float>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._duration)
		{
			_duration = VariantUtils.ConvertTo<float>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._speed)
		{
			_speed = VariantUtils.ConvertTo<float>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._accel)
		{
			_accel = VariantUtils.ConvertTo<float>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._arcDir)
		{
			_arcDir = VariantUtils.ConvertTo<float>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._trailPath)
		{
			_trailPath = VariantUtils.ConvertTo<string>(ref value);
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
		if ((ref name) == PropertyName._vfx)
		{
			value = VariantUtils.CreateFrom<NCardTrailVfx>(ref _vfx);
			return true;
		}
		if ((ref name) == PropertyName._fadeOutTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _fadeOutTween);
			return true;
		}
		if ((ref name) == PropertyName._vfxFading)
		{
			value = VariantUtils.CreateFrom<bool>(ref _vfxFading);
			return true;
		}
		if ((ref name) == PropertyName._startPos)
		{
			value = VariantUtils.CreateFrom<Vector2>(ref _startPos);
			return true;
		}
		if ((ref name) == PropertyName._endPos)
		{
			value = VariantUtils.CreateFrom<Vector2>(ref _endPos);
			return true;
		}
		if ((ref name) == PropertyName._controlPointOffset)
		{
			value = VariantUtils.CreateFrom<float>(ref _controlPointOffset);
			return true;
		}
		if ((ref name) == PropertyName._duration)
		{
			value = VariantUtils.CreateFrom<float>(ref _duration);
			return true;
		}
		if ((ref name) == PropertyName._speed)
		{
			value = VariantUtils.CreateFrom<float>(ref _speed);
			return true;
		}
		if ((ref name) == PropertyName._accel)
		{
			value = VariantUtils.CreateFrom<float>(ref _accel);
			return true;
		}
		if ((ref name) == PropertyName._arcDir)
		{
			value = VariantUtils.CreateFrom<float>(ref _arcDir);
			return true;
		}
		if ((ref name) == PropertyName._trailPath)
		{
			value = VariantUtils.CreateFrom<string>(ref _trailPath);
			return true;
		}
		return ((GodotObject)this).GetGodotClassPropertyValue(ref name, ref value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._vfx, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._fadeOutTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._vfxFading, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)5, PropertyName._startPos, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)5, PropertyName._endPos, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)3, PropertyName._controlPointOffset, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)3, PropertyName._duration, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)3, PropertyName._speed, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)3, PropertyName._accel, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)3, PropertyName._arcDir, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)4, PropertyName._trailPath, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		((GodotObject)this).SaveGodotObjectData(info);
		info.AddProperty(PropertyName._vfx, Variant.From<NCardTrailVfx>(ref _vfx));
		info.AddProperty(PropertyName._fadeOutTween, Variant.From<Tween>(ref _fadeOutTween));
		info.AddProperty(PropertyName._vfxFading, Variant.From<bool>(ref _vfxFading));
		info.AddProperty(PropertyName._startPos, Variant.From<Vector2>(ref _startPos));
		info.AddProperty(PropertyName._endPos, Variant.From<Vector2>(ref _endPos));
		info.AddProperty(PropertyName._controlPointOffset, Variant.From<float>(ref _controlPointOffset));
		info.AddProperty(PropertyName._duration, Variant.From<float>(ref _duration));
		info.AddProperty(PropertyName._speed, Variant.From<float>(ref _speed));
		info.AddProperty(PropertyName._accel, Variant.From<float>(ref _accel));
		info.AddProperty(PropertyName._arcDir, Variant.From<float>(ref _arcDir));
		info.AddProperty(PropertyName._trailPath, Variant.From<string>(ref _trailPath));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._vfx, ref val))
		{
			_vfx = ((Variant)(ref val)).As<NCardTrailVfx>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._fadeOutTween, ref val2))
		{
			_fadeOutTween = ((Variant)(ref val2)).As<Tween>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._vfxFading, ref val3))
		{
			_vfxFading = ((Variant)(ref val3)).As<bool>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._startPos, ref val4))
		{
			_startPos = ((Variant)(ref val4)).As<Vector2>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._endPos, ref val5))
		{
			_endPos = ((Variant)(ref val5)).As<Vector2>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._controlPointOffset, ref val6))
		{
			_controlPointOffset = ((Variant)(ref val6)).As<float>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName._duration, ref val7))
		{
			_duration = ((Variant)(ref val7)).As<float>();
		}
		Variant val8 = default(Variant);
		if (info.TryGetProperty(PropertyName._speed, ref val8))
		{
			_speed = ((Variant)(ref val8)).As<float>();
		}
		Variant val9 = default(Variant);
		if (info.TryGetProperty(PropertyName._accel, ref val9))
		{
			_accel = ((Variant)(ref val9)).As<float>();
		}
		Variant val10 = default(Variant);
		if (info.TryGetProperty(PropertyName._arcDir, ref val10))
		{
			_arcDir = ((Variant)(ref val10)).As<float>();
		}
		Variant val11 = default(Variant);
		if (info.TryGetProperty(PropertyName._trailPath, ref val11))
		{
			_trailPath = ((Variant)(ref val11)).As<string>();
		}
	}
}
