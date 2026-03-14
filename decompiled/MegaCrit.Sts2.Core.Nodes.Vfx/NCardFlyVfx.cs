using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.TestSupport;

namespace MegaCrit.Sts2.Core.Nodes.Vfx;

[ScriptPath("res://src/Core/Nodes/Vfx/NCardFlyVfx.cs")]
public class NCardFlyVfx : Node2D
{
	public class MethodName : MethodName
	{
		public static readonly StringName Create = StringName.op_Implicit("Create");

		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName _ExitTree = StringName.op_Implicit("_ExitTree");

		public static readonly StringName OnCardExitedTree = StringName.op_Implicit("OnCardExitedTree");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName _card = StringName.op_Implicit("_card");

		public static readonly StringName _trailPath = StringName.op_Implicit("_trailPath");

		public static readonly StringName _vfx = StringName.op_Implicit("_vfx");

		public static readonly StringName _fadeOutTween = StringName.op_Implicit("_fadeOutTween");

		public static readonly StringName _vfxFading = StringName.op_Implicit("_vfxFading");

		public static readonly StringName _isAddingToPile = StringName.op_Implicit("_isAddingToPile");

		public static readonly StringName _startPos = StringName.op_Implicit("_startPos");

		public static readonly StringName _endPos = StringName.op_Implicit("_endPos");

		public static readonly StringName _controlPointOffset = StringName.op_Implicit("_controlPointOffset");

		public static readonly StringName _duration = StringName.op_Implicit("_duration");

		public static readonly StringName _speed = StringName.op_Implicit("_speed");

		public static readonly StringName _accel = StringName.op_Implicit("_accel");

		public static readonly StringName _arcDir = StringName.op_Implicit("_arcDir");
	}

	public class SignalName : SignalName
	{
	}

	private NCard _card;

	private string _trailPath;

	private NCardTrailVfx? _vfx;

	private Tween? _fadeOutTween;

	private bool _vfxFading;

	private bool _isAddingToPile;

	private Vector2 _startPos;

	private Vector2 _endPos;

	private float _controlPointOffset;

	private float _duration;

	private float _speed;

	private float _accel;

	private float _arcDir;

	private readonly CancellationTokenSource _cancelToken = new CancellationTokenSource();

	private static readonly string _scenePath = SceneHelper.GetScenePath("vfx/vfx_card_fly");

	public static IEnumerable<string> AssetPaths => new _003C_003Ez__ReadOnlySingleElementList<string>(_scenePath);

	public TaskCompletionSource? SwooshAwayCompletion { get; private set; }

	public static NCardFlyVfx? Create(NCard card, Vector2 end, bool isAddingToPile, string trailPath)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		if (TestMode.IsOn)
		{
			return null;
		}
		NCardFlyVfx nCardFlyVfx = PreloadManager.Cache.GetScene(_scenePath).Instantiate<NCardFlyVfx>((GenEditState)0);
		nCardFlyVfx._startPos = ((Control)card).GlobalPosition;
		nCardFlyVfx._endPos = end;
		nCardFlyVfx._card = card;
		nCardFlyVfx._isAddingToPile = isAddingToPile;
		nCardFlyVfx._trailPath = trailPath;
		return nCardFlyVfx;
	}

	public override void _Ready()
	{
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		_vfx = NCardTrailVfx.Create((Control)(object)_card, _trailPath);
		if (_vfx != null)
		{
			((Node)this).GetParent().AddChildSafely((Node?)(object)_vfx);
		}
		_controlPointOffset = Rng.Chaotic.NextFloat(100f, 400f);
		_speed = Rng.Chaotic.NextFloat(1.1f, 1.25f);
		_accel = Rng.Chaotic.NextFloat(2f, 2.5f);
		float y = _endPos.Y;
		Rect2 viewportRect = ((CanvasItem)this).GetViewportRect();
		_arcDir = ((y < ((Rect2)(ref viewportRect)).Size.Y * 0.5f) ? (-500f) : (500f + _controlPointOffset));
		_duration = Rng.Chaotic.NextFloat(1f, 1.75f);
		((GodotObject)_card).Connect(SignalName.TreeExited, Callable.From((Action)OnCardExitedTree), 0u);
		if (NCombatUi.IsDebugHidingPlayContainer)
		{
			((CanvasItem)_card).Modulate = Colors.Transparent;
			((CanvasItem)_card).Visible = false;
			((CanvasItem)this).Visible = false;
		}
		TaskHelper.RunSafely(PlayAnim());
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

	private void OnCardExitedTree()
	{
		try
		{
			((Node)(object)_vfx)?.QueueFreeSafely();
		}
		catch (ObjectDisposedException)
		{
		}
		SwooshAwayCompletion?.TrySetResult();
		((Node)(object)this).QueueFreeSafely();
	}

	private async Task PlayAnim()
	{
		SwooshAwayCompletion = new TaskCompletionSource();
		float time = 0f;
		while (time / _duration <= 1f)
		{
			await ((GodotObject)this).ToSignal((GodotObject)(object)((Node)this).GetTree(), SignalName.ProcessFrame);
			if (_cancelToken.IsCancellationRequested)
			{
				SwooshAwayCompletion?.SetResult();
				return;
			}
			float num = (float)((Node)this).GetProcessDeltaTime();
			time += _speed * num;
			_speed += _accel * num;
			Vector2 c = _startPos + (_endPos - _startPos) * 0.5f;
			c.Y -= _arcDir;
			Vector2 val = MathHelper.BezierCurve(_startPos, _endPos, c, (time + 0.05f) / _duration);
			((Control)_card).GlobalPosition = MathHelper.BezierCurve(_startPos, _endPos, c, time / _duration);
			Vector2 val2 = val - ((Control)_card).GlobalPosition;
			float num2 = ((Vector2)(ref val2)).Angle() + (float)Math.PI / 2f;
			Node parent = ((Node)_card).GetParent();
			Control val3 = (Control)(object)((parent is Control) ? parent : null);
			if (val3 != null)
			{
				num2 -= val3.Rotation;
			}
			else
			{
				Node2D val4 = (Node2D)(object)((parent is Node2D) ? parent : null);
				if (val4 != null)
				{
					num2 -= val4.Rotation;
				}
			}
			((Control)_card).Rotation = Mathf.LerpAngle(((Control)_card).Rotation, num2, num * 12f);
			Control body = _card.Body;
			Color white = Colors.White;
			((CanvasItem)body).Modulate = ((Color)(ref white)).Lerp(Colors.Black, Mathf.Clamp(time * 3f / _duration, 0f, 1f));
			_card.Body.Scale = Vector2.One * Mathf.Lerp(1f, 0.1f, Mathf.Clamp(time * 3f / _duration, 0f, 1f));
		}
		((Control)_card).GlobalPosition = _endPos;
		if (_isAddingToPile)
		{
			_card.Model.Pile?.InvokeCardAddFinished();
		}
		time = 0f;
		while (time / _duration <= 1f)
		{
			await ((GodotObject)this).ToSignal((GodotObject)(object)((Node)this).GetTree(), SignalName.ProcessFrame);
			if (_cancelToken.IsCancellationRequested)
			{
				SwooshAwayCompletion?.SetResult();
				return;
			}
			float num3 = (float)((Node)this).GetProcessDeltaTime();
			time += _speed * num3;
			if (time / _duration > 0.25f && !_vfxFading)
			{
				if (_vfx != null)
				{
					TaskHelper.RunSafely(_vfx.FadeOut());
				}
				_vfxFading = true;
			}
			_card.Body.Scale = Vector2.One * Mathf.Max(Mathf.Lerp(0.1f, -0.15f, time / _duration), 0f);
		}
		SwooshAwayCompletion?.SetResult();
		((Node)(object)_card).QueueFreeSafely();
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Expected O, but got Unknown
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Expected O, but got Unknown
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(4);
		list.Add(new MethodInfo(MethodName.Create, new PropertyInfo((Type)24, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Node2D"), false), (MethodFlags)33, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("card"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false),
			new PropertyInfo((Type)5, StringName.op_Implicit("end"), (PropertyHint)0, "", (PropertyUsageFlags)6, false),
			new PropertyInfo((Type)1, StringName.op_Implicit("isAddingToPile"), (PropertyHint)0, "", (PropertyUsageFlags)6, false),
			new PropertyInfo((Type)4, StringName.op_Implicit("trailPath"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._ExitTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnCardExitedTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.Create && ((NativeVariantPtrArgs)(ref args)).Count == 4)
		{
			NCardFlyVfx nCardFlyVfx = Create(VariantUtils.ConvertTo<NCard>(ref ((NativeVariantPtrArgs)(ref args))[0]), VariantUtils.ConvertTo<Vector2>(ref ((NativeVariantPtrArgs)(ref args))[1]), VariantUtils.ConvertTo<bool>(ref ((NativeVariantPtrArgs)(ref args))[2]), VariantUtils.ConvertTo<string>(ref ((NativeVariantPtrArgs)(ref args))[3]));
			ret = VariantUtils.CreateFrom<NCardFlyVfx>(ref nCardFlyVfx);
			return true;
		}
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
		if ((ref method) == MethodName.OnCardExitedTree && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnCardExitedTree();
			ret = default(godot_variant);
			return true;
		}
		return ((Node2D)this).InvokeGodotClassMethod(ref method, args, ref ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static bool InvokeGodotClassStaticMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.Create && ((NativeVariantPtrArgs)(ref args)).Count == 4)
		{
			NCardFlyVfx nCardFlyVfx = Create(VariantUtils.ConvertTo<NCard>(ref ((NativeVariantPtrArgs)(ref args))[0]), VariantUtils.ConvertTo<Vector2>(ref ((NativeVariantPtrArgs)(ref args))[1]), VariantUtils.ConvertTo<bool>(ref ((NativeVariantPtrArgs)(ref args))[2]), VariantUtils.ConvertTo<string>(ref ((NativeVariantPtrArgs)(ref args))[3]));
			ret = VariantUtils.CreateFrom<NCardFlyVfx>(ref nCardFlyVfx);
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
		if ((ref method) == MethodName._ExitTree)
		{
			return true;
		}
		if ((ref method) == MethodName.OnCardExitedTree)
		{
			return true;
		}
		return ((Node2D)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName._card)
		{
			_card = VariantUtils.ConvertTo<NCard>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._trailPath)
		{
			_trailPath = VariantUtils.ConvertTo<string>(ref value);
			return true;
		}
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
		if ((ref name) == PropertyName._isAddingToPile)
		{
			_isAddingToPile = VariantUtils.ConvertTo<bool>(ref value);
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
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName._card)
		{
			value = VariantUtils.CreateFrom<NCard>(ref _card);
			return true;
		}
		if ((ref name) == PropertyName._trailPath)
		{
			value = VariantUtils.CreateFrom<string>(ref _trailPath);
			return true;
		}
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
		if ((ref name) == PropertyName._isAddingToPile)
		{
			value = VariantUtils.CreateFrom<bool>(ref _isAddingToPile);
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
		return ((GodotObject)this).GetGodotClassPropertyValue(ref name, ref value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._card, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)4, PropertyName._trailPath, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._vfx, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._fadeOutTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._vfxFading, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._isAddingToPile, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)5, PropertyName._startPos, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)5, PropertyName._endPos, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)3, PropertyName._controlPointOffset, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)3, PropertyName._duration, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)3, PropertyName._speed, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)3, PropertyName._accel, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)3, PropertyName._arcDir, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		info.AddProperty(PropertyName._card, Variant.From<NCard>(ref _card));
		info.AddProperty(PropertyName._trailPath, Variant.From<string>(ref _trailPath));
		info.AddProperty(PropertyName._vfx, Variant.From<NCardTrailVfx>(ref _vfx));
		info.AddProperty(PropertyName._fadeOutTween, Variant.From<Tween>(ref _fadeOutTween));
		info.AddProperty(PropertyName._vfxFading, Variant.From<bool>(ref _vfxFading));
		info.AddProperty(PropertyName._isAddingToPile, Variant.From<bool>(ref _isAddingToPile));
		info.AddProperty(PropertyName._startPos, Variant.From<Vector2>(ref _startPos));
		info.AddProperty(PropertyName._endPos, Variant.From<Vector2>(ref _endPos));
		info.AddProperty(PropertyName._controlPointOffset, Variant.From<float>(ref _controlPointOffset));
		info.AddProperty(PropertyName._duration, Variant.From<float>(ref _duration));
		info.AddProperty(PropertyName._speed, Variant.From<float>(ref _speed));
		info.AddProperty(PropertyName._accel, Variant.From<float>(ref _accel));
		info.AddProperty(PropertyName._arcDir, Variant.From<float>(ref _arcDir));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._card, ref val))
		{
			_card = ((Variant)(ref val)).As<NCard>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._trailPath, ref val2))
		{
			_trailPath = ((Variant)(ref val2)).As<string>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._vfx, ref val3))
		{
			_vfx = ((Variant)(ref val3)).As<NCardTrailVfx>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._fadeOutTween, ref val4))
		{
			_fadeOutTween = ((Variant)(ref val4)).As<Tween>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._vfxFading, ref val5))
		{
			_vfxFading = ((Variant)(ref val5)).As<bool>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._isAddingToPile, ref val6))
		{
			_isAddingToPile = ((Variant)(ref val6)).As<bool>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName._startPos, ref val7))
		{
			_startPos = ((Variant)(ref val7)).As<Vector2>();
		}
		Variant val8 = default(Variant);
		if (info.TryGetProperty(PropertyName._endPos, ref val8))
		{
			_endPos = ((Variant)(ref val8)).As<Vector2>();
		}
		Variant val9 = default(Variant);
		if (info.TryGetProperty(PropertyName._controlPointOffset, ref val9))
		{
			_controlPointOffset = ((Variant)(ref val9)).As<float>();
		}
		Variant val10 = default(Variant);
		if (info.TryGetProperty(PropertyName._duration, ref val10))
		{
			_duration = ((Variant)(ref val10)).As<float>();
		}
		Variant val11 = default(Variant);
		if (info.TryGetProperty(PropertyName._speed, ref val11))
		{
			_speed = ((Variant)(ref val11)).As<float>();
		}
		Variant val12 = default(Variant);
		if (info.TryGetProperty(PropertyName._accel, ref val12))
		{
			_accel = ((Variant)(ref val12)).As<float>();
		}
		Variant val13 = default(Variant);
		if (info.TryGetProperty(PropertyName._arcDir, ref val13))
		{
			_arcDir = ((Variant)(ref val13)).As<float>();
		}
	}
}
