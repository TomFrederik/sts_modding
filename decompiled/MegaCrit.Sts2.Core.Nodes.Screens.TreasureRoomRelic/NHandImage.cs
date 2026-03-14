using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.TreasureRelicPicking;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.Vfx.Utilities;
using MegaCrit.Sts2.Core.Random;

namespace MegaCrit.Sts2.Core.Nodes.Screens.TreasureRoomRelic;

[ScriptPath("res://src/Core/Nodes/Screens/TreasureRoomRelic/NHandImage.cs")]
public class NHandImage : Control
{
	private enum State
	{
		None,
		Frozen,
		GrabbingRelic
	}

	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName SetIsInFight = StringName.op_Implicit("SetIsInFight");

		public static readonly StringName SetFrozenForRelicAwards = StringName.op_Implicit("SetFrozenForRelicAwards");

		public static readonly StringName DoFightMove = StringName.op_Implicit("DoFightMove");

		public static readonly StringName SetTextureToFightMove = StringName.op_Implicit("SetTextureToFightMove");

		public static readonly StringName SetPointingPosition = StringName.op_Implicit("SetPointingPosition");

		public static readonly StringName AnimateAway = StringName.op_Implicit("AnimateAway");

		public static readonly StringName AnimateIn = StringName.op_Implicit("AnimateIn");

		public static readonly StringName SetIsDown = StringName.op_Implicit("SetIsDown");

		public static readonly StringName SetAnimateInProgress = StringName.op_Implicit("SetAnimateInProgress");

		public static readonly StringName _Process = StringName.op_Implicit("_Process");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName Index = StringName.op_Implicit("Index");

		public static readonly StringName IsDown = StringName.op_Implicit("IsDown");

		public static readonly StringName _grabMarker = StringName.op_Implicit("_grabMarker");

		public static readonly StringName _textureRect = StringName.op_Implicit("_textureRect");

		public static readonly StringName _currentVelocity = StringName.op_Implicit("_currentVelocity");

		public static readonly StringName _desiredPosition = StringName.op_Implicit("_desiredPosition");

		public static readonly StringName _downTween = StringName.op_Implicit("_downTween");

		public static readonly StringName _state = StringName.op_Implicit("_state");

		public static readonly StringName _isInFight = StringName.op_Implicit("_isInFight");

		public static readonly StringName _originalPosition = StringName.op_Implicit("_originalPosition");

		public static readonly StringName _handAnimateInProgress = StringName.op_Implicit("_handAnimateInProgress");
	}

	public class SignalName : SignalName
	{
	}

	private static readonly string _scenePath = SceneHelper.GetScenePath("ui/hand_image");

	private static readonly Vector2 _pointingPivot = new Vector2(163f, 10f);

	private static readonly Vector2 _fightingPivot = new Vector2(197f, 600f);

	private Marker2D _grabMarker;

	private TextureRect _textureRect;

	private Vector2 _currentVelocity;

	private Vector2 _desiredPosition;

	private Tween? _downTween;

	private State _state;

	private bool _isInFight;

	private Vector2 _originalPosition;

	private float _handAnimateInProgress;

	public Player Player { get; private set; }

	public int Index { get; private set; }

	public bool IsDown { get; private set; }

	public static NHandImage Create(Player player, int slotIndex)
	{
		NHandImage nHandImage = PreloadManager.Cache.GetScene(_scenePath).Instantiate<NHandImage>((GenEditState)0);
		nHandImage.Player = player;
		nHandImage.Index = slotIndex;
		return nHandImage;
	}

	public override void _Ready()
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		_textureRect = ((Node)this).GetNode<TextureRect>(NodePath.op_Implicit("TextureRect"));
		_grabMarker = ((Node)this).GetNode<Marker2D>(NodePath.op_Implicit("GrabMarker"));
		_originalPosition = ((Control)_textureRect).Position;
		((Control)this).Rotation = (Index % 4) switch
		{
			0 => 0f, 
			1 => (float)Math.PI / 2f, 
			2 => -(float)Math.PI / 2f, 
			3 => (float)Math.PI, 
			_ => throw new ArgumentOutOfRangeException(), 
		};
		if (!LocalContext.IsMe(Player))
		{
			((CanvasItem)this).Modulate = new Color(0.5f, 0.5f, 0.5f, 0.5f);
		}
		_textureRect.Texture = Player.Character.ArmPointingTexture;
	}

	public void SetIsInFight(bool inFight)
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		_isInFight = inFight;
		if (_isInFight)
		{
			((Control)_textureRect).PivotOffset = _fightingPivot;
			((CanvasItem)this).Modulate = Colors.White;
			((CanvasItem)this).ZIndex = 1;
			return;
		}
		((Control)_textureRect).PivotOffset = _pointingPivot;
		if (!LocalContext.IsMe(Player))
		{
			((CanvasItem)this).Modulate = new Color(0.5f, 0.5f, 0.5f, 0.5f);
		}
		((CanvasItem)this).ZIndex = 0;
	}

	public void SetFrozenForRelicAwards(bool frozenForRelicAwards)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		if (frozenForRelicAwards)
		{
			_state = State.Frozen;
			Rect2 viewportRect = ((CanvasItem)this).GetViewportRect();
			Vector2 down = Vector2.Down;
			Vector2 val = ((Vector2)(ref down)).Rotated(((Control)this).Rotation);
			_desiredPosition = ((Rect2)(ref viewportRect)).Size / 2f + ((Rect2)(ref viewportRect)).Size * val * 0.1667f;
		}
		else
		{
			_state = State.None;
		}
	}

	public Tween DoFightMove(RelicPickingFightMove move, float duration)
	{
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_0296: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0311: Unknown result type (might be due to invalid IL or missing references)
		float num = 0.666f * duration / 3f;
		float num2 = 0.333f * duration / 3f;
		int num3 = 6;
		List<float> list = new List<float>(num3);
		CollectionsMarshal.SetCount(list, num3);
		Span<float> span = CollectionsMarshal.AsSpan(list);
		int num4 = 0;
		span[num4] = num;
		num4++;
		span[num4] = num2;
		num4++;
		span[num4] = num;
		num4++;
		span[num4] = num2;
		num4++;
		span[num4] = num;
		num4++;
		span[num4] = num2;
		List<float> list2 = list;
		for (int i = 0; i < list2.Count - 1; i++)
		{
			float num5 = Rng.Chaotic.NextFloat((0f - duration) / 25f, duration / 25f);
			list2[i] += num5;
			list2[i + 1] -= num5;
		}
		SetTextureToFightMove(RelicPickingFightMove.Rock);
		Tween val = ((Node)this).CreateTween();
		val.Chain().TweenProperty((GodotObject)(object)_textureRect, NodePath.op_Implicit("rotation"), Variant.op_Implicit(-(float)Math.PI / 10f + Rng.Chaotic.NextFloat(-0.05f, 0.05f)), (double)list2[0]).SetTrans((TransitionType)0)
			.SetEase((EaseType)0);
		val.Chain().TweenProperty((GodotObject)(object)_textureRect, NodePath.op_Implicit("rotation"), Variant.op_Implicit(Rng.Chaotic.NextFloat(-0.02f, 0.02f)), (double)list2[1]).SetTrans((TransitionType)5)
			.SetEase((EaseType)0);
		val.Chain().TweenProperty((GodotObject)(object)_textureRect, NodePath.op_Implicit("rotation"), Variant.op_Implicit(-(float)Math.PI / 10f + Rng.Chaotic.NextFloat(-0.05f, 0.05f)), (double)list2[2]).SetTrans((TransitionType)0)
			.SetEase((EaseType)0);
		val.Chain().TweenProperty((GodotObject)(object)_textureRect, NodePath.op_Implicit("rotation"), Variant.op_Implicit(Rng.Chaotic.NextFloat(-0.02f, 0.02f)), (double)list2[3]).SetTrans((TransitionType)5)
			.SetEase((EaseType)0);
		val.Chain().TweenProperty((GodotObject)(object)_textureRect, NodePath.op_Implicit("rotation"), Variant.op_Implicit(-(float)Math.PI / 10f + Rng.Chaotic.NextFloat(-0.05f, 0.05f)), (double)list2[4]).SetTrans((TransitionType)0)
			.SetEase((EaseType)0);
		val.Chain().TweenProperty((GodotObject)(object)_textureRect, NodePath.op_Implicit("rotation"), Variant.op_Implicit(Rng.Chaotic.NextFloat(-0.02f, 0.02f)), (double)list2[5]).SetTrans((TransitionType)5)
			.SetEase((EaseType)0);
		val.TweenCallback(Callable.From((Action)delegate
		{
			SetTextureToFightMove(move);
		}));
		return val;
	}

	private void SetTextureToFightMove(RelicPickingFightMove move)
	{
		TextureRect textureRect = _textureRect;
		textureRect.Texture = (Texture2D)(move switch
		{
			RelicPickingFightMove.Rock => Player.Character.ArmRockTexture, 
			RelicPickingFightMove.Paper => Player.Character.ArmPaperTexture, 
			RelicPickingFightMove.Scissors => Player.Character.ArmScissorsTexture, 
			_ => throw new ArgumentOutOfRangeException("move", move, null), 
		});
	}

	public void SetPointingPosition(Vector2 position)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		if (_state == State.None)
		{
			_desiredPosition = position;
		}
	}

	public void AnimateAway()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		Rect2 viewportRect = ((CanvasItem)this).GetViewportRect();
		Vector2 down = Vector2.Down;
		Vector2 val = ((Vector2)(ref down)).Rotated(((Control)this).Rotation);
		_desiredPosition = ((Rect2)(ref viewportRect)).Size / 2f + ((Rect2)(ref viewportRect)).Size * val * 0.8f;
	}

	public void AnimateIn()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		Tween val = ((Node)this).CreateTween();
		_handAnimateInProgress = 0f;
		val.TweenMethod(Callable.From<float>((Action<float>)delegate(float v)
		{
			_handAnimateInProgress = v;
		}), Variant.op_Implicit(0f), Variant.op_Implicit(1f), 0.6000000238418579).SetTrans((TransitionType)5).SetEase((EaseType)1);
	}

	public void SetIsDown(bool isDown)
	{
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		if (IsDown != isDown)
		{
			IsDown = isDown;
			Tween? downTween = _downTween;
			if (downTween != null)
			{
				downTween.Kill();
			}
			if (isDown)
			{
				((Control)_textureRect).Scale = Vector2.One * 0.98f;
				return;
			}
			_downTween = ((Node)this).CreateTween();
			_downTween.TweenProperty((GodotObject)(object)_textureRect, NodePath.op_Implicit("scale"), Variant.op_Implicit(Vector2.One), 0.20000000298023224).SetTrans((TransitionType)5).SetEase((EaseType)1);
		}
	}

	public async Task DoLoseShake(float duration)
	{
		((Node)this).CreateTween().TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("modulate"), Variant.op_Implicit(new Color(0.5f, 0.5f, 0.5f, 0.5f)), (double)(duration * 0.333f)).SetDelay((double)(duration * 0.667f));
		ScreenRumbleInstance rumble = new ScreenRumbleInstance(100f, duration, 5f, RumbleStyle.Rumble);
		while (!rumble.IsDone)
		{
			Vector2 val = rumble.Update(((Node)this).GetProcessDeltaTime());
			((Control)_textureRect).Position = _originalPosition + val;
			await ((GodotObject)this).ToSignal((GodotObject)(object)((Node)this).GetTree(), SignalName.ProcessFrame);
		}
		((Control)_textureRect).Position = _originalPosition;
	}

	public async Task GrabRelic(NTreasureRoomRelicHolder holder)
	{
		State oldState = _state;
		_state = State.GrabbingRelic;
		SetTextureToFightMove(RelicPickingFightMove.Paper);
		Tween val = ((Node)this).CreateTween();
		Tween obj = val;
		NHandImage nHandImage = this;
		NodePath obj2 = NodePath.op_Implicit("global_position");
		Vector2 globalPosition = ((Control)holder).GlobalPosition;
		Vector2 position = ((Node2D)_grabMarker).Position;
		obj.TweenProperty((GodotObject)(object)nHandImage, obj2, Variant.op_Implicit(globalPosition - ((Vector2)(ref position)).Rotated(((Control)this).Rotation) + ((Control)holder).Size * 0.5f), 0.5).SetTrans((TransitionType)1).SetEase((EaseType)2);
		await ((GodotObject)this).ToSignal((GodotObject)(object)val, SignalName.Finished);
		SetTextureToFightMove(RelicPickingFightMove.Rock);
		((Node)holder).Reparent((Node)(object)this, true);
		((Control)holder).Rotation = 0f - ((Control)this).Rotation;
		((Control)holder).Position = ((Node2D)_grabMarker).Position - ((Control)holder).Size * 0.5f;
		val = ((Node)this).CreateTween();
		val.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("global_position"), Variant.op_Implicit(_desiredPosition), 0.5).SetTrans((TransitionType)1).SetEase((EaseType)2);
		await ((GodotObject)this).ToSignal((GodotObject)(object)val, SignalName.Finished);
		_state = oldState;
	}

	public void SetAnimateInProgress(float animateInProgress)
	{
		_handAnimateInProgress = animateInProgress;
	}

	public override void _Process(double delta)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		Rect2 viewportRect = ((CanvasItem)this).GetViewportRect();
		Vector2 size = ((Rect2)(ref viewportRect)).Size;
		int num = Index % 4;
		if (_state != State.GrabbingRelic)
		{
			Vector2 val;
			if ((num == 0 || num == 3) ? true : false)
			{
				float num2 = ((num == 0) ? 1 : (-1));
				val = num2 * Vector2.Down;
			}
			else
			{
				float num3 = ((num == 1) ? 1 : (-1));
				val = num3 * Vector2.Left;
			}
			Rect2 viewportRect2 = ((CanvasItem)this).GetViewportRect();
			Vector2 val2 = ((Rect2)(ref viewportRect2)).Size / 2f + ((Rect2)(ref viewportRect2)).Size * val;
			float smoothTime = ((_state == State.Frozen) ? 0.25f : ((!LocalContext.IsMe(Player)) ? 0.07f : 0.01f));
			Vector2 target = ((Vector2)(ref val2)).Lerp(_desiredPosition, _handAnimateInProgress);
			((Control)this).GlobalPosition = MathHelper.SmoothDamp(((Control)this).GlobalPosition, target, ref _currentVelocity, smoothTime, (float)delta);
		}
		if (_state == State.None)
		{
			if ((num == 0 || num == 3) ? true : false)
			{
				float num4 = ((num == 0) ? 1 : (-1));
				((Control)_textureRect).Rotation = num4 * (((Control)this).GlobalPosition.X - size.X / 2f) / 2000f;
			}
			else
			{
				float num5 = ((num == 1) ? 1 : (-1));
				((Control)_textureRect).Rotation = num5 * (((Control)this).GlobalPosition.Y - size.Y / 2f) / 1000f;
			}
		}
		else
		{
			((Control)_textureRect).Rotation = 0f;
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
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Expected O, but got Unknown
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0281: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02af: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0303: Unknown result type (might be due to invalid IL or missing references)
		//IL_0329: Unknown result type (might be due to invalid IL or missing references)
		//IL_034c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0357: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(11);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetIsInFight, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)1, StringName.op_Implicit("inFight"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetFrozenForRelicAwards, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)1, StringName.op_Implicit("frozenForRelicAwards"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.DoFightMove, new PropertyInfo((Type)24, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Tween"), false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("move"), (PropertyHint)0, "", (PropertyUsageFlags)6, false),
			new PropertyInfo((Type)3, StringName.op_Implicit("duration"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetTextureToFightMove, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("move"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetPointingPosition, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)5, StringName.op_Implicit("position"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AnimateAway, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AnimateIn, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetIsDown, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)1, StringName.op_Implicit("isDown"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetAnimateInProgress, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)3, StringName.op_Implicit("animateInProgress"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Process, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)3, StringName.op_Implicit("delta"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetIsInFight && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			SetIsInFight(VariantUtils.ConvertTo<bool>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetFrozenForRelicAwards && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			SetFrozenForRelicAwards(VariantUtils.ConvertTo<bool>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.DoFightMove && ((NativeVariantPtrArgs)(ref args)).Count == 2)
		{
			Tween val = DoFightMove(VariantUtils.ConvertTo<RelicPickingFightMove>(ref ((NativeVariantPtrArgs)(ref args))[0]), VariantUtils.ConvertTo<float>(ref ((NativeVariantPtrArgs)(ref args))[1]));
			ret = VariantUtils.CreateFrom<Tween>(ref val);
			return true;
		}
		if ((ref method) == MethodName.SetTextureToFightMove && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			SetTextureToFightMove(VariantUtils.ConvertTo<RelicPickingFightMove>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetPointingPosition && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			SetPointingPosition(VariantUtils.ConvertTo<Vector2>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.AnimateAway && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			AnimateAway();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.AnimateIn && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			AnimateIn();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetIsDown && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			SetIsDown(VariantUtils.ConvertTo<bool>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetAnimateInProgress && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			SetAnimateInProgress(VariantUtils.ConvertTo<float>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._Process && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			((Node)this)._Process(VariantUtils.ConvertTo<double>(ref ((NativeVariantPtrArgs)(ref args))[0]));
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
		if ((ref method) == MethodName.SetIsInFight)
		{
			return true;
		}
		if ((ref method) == MethodName.SetFrozenForRelicAwards)
		{
			return true;
		}
		if ((ref method) == MethodName.DoFightMove)
		{
			return true;
		}
		if ((ref method) == MethodName.SetTextureToFightMove)
		{
			return true;
		}
		if ((ref method) == MethodName.SetPointingPosition)
		{
			return true;
		}
		if ((ref method) == MethodName.AnimateAway)
		{
			return true;
		}
		if ((ref method) == MethodName.AnimateIn)
		{
			return true;
		}
		if ((ref method) == MethodName.SetIsDown)
		{
			return true;
		}
		if ((ref method) == MethodName.SetAnimateInProgress)
		{
			return true;
		}
		if ((ref method) == MethodName._Process)
		{
			return true;
		}
		return ((Control)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName.Index)
		{
			Index = VariantUtils.ConvertTo<int>(ref value);
			return true;
		}
		if ((ref name) == PropertyName.IsDown)
		{
			IsDown = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._grabMarker)
		{
			_grabMarker = VariantUtils.ConvertTo<Marker2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._textureRect)
		{
			_textureRect = VariantUtils.ConvertTo<TextureRect>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._currentVelocity)
		{
			_currentVelocity = VariantUtils.ConvertTo<Vector2>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._desiredPosition)
		{
			_desiredPosition = VariantUtils.ConvertTo<Vector2>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._downTween)
		{
			_downTween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._state)
		{
			_state = VariantUtils.ConvertTo<State>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._isInFight)
		{
			_isInFight = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._originalPosition)
		{
			_originalPosition = VariantUtils.ConvertTo<Vector2>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._handAnimateInProgress)
		{
			_handAnimateInProgress = VariantUtils.ConvertTo<float>(ref value);
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
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName.Index)
		{
			int index = Index;
			value = VariantUtils.CreateFrom<int>(ref index);
			return true;
		}
		if ((ref name) == PropertyName.IsDown)
		{
			bool isDown = IsDown;
			value = VariantUtils.CreateFrom<bool>(ref isDown);
			return true;
		}
		if ((ref name) == PropertyName._grabMarker)
		{
			value = VariantUtils.CreateFrom<Marker2D>(ref _grabMarker);
			return true;
		}
		if ((ref name) == PropertyName._textureRect)
		{
			value = VariantUtils.CreateFrom<TextureRect>(ref _textureRect);
			return true;
		}
		if ((ref name) == PropertyName._currentVelocity)
		{
			value = VariantUtils.CreateFrom<Vector2>(ref _currentVelocity);
			return true;
		}
		if ((ref name) == PropertyName._desiredPosition)
		{
			value = VariantUtils.CreateFrom<Vector2>(ref _desiredPosition);
			return true;
		}
		if ((ref name) == PropertyName._downTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _downTween);
			return true;
		}
		if ((ref name) == PropertyName._state)
		{
			value = VariantUtils.CreateFrom<State>(ref _state);
			return true;
		}
		if ((ref name) == PropertyName._isInFight)
		{
			value = VariantUtils.CreateFrom<bool>(ref _isInFight);
			return true;
		}
		if ((ref name) == PropertyName._originalPosition)
		{
			value = VariantUtils.CreateFrom<Vector2>(ref _originalPosition);
			return true;
		}
		if ((ref name) == PropertyName._handAnimateInProgress)
		{
			value = VariantUtils.CreateFrom<float>(ref _handAnimateInProgress);
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
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._grabMarker, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._textureRect, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)5, PropertyName._currentVelocity, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)5, PropertyName._desiredPosition, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._downTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)2, PropertyName._state, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._isInFight, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)5, PropertyName._originalPosition, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)3, PropertyName._handAnimateInProgress, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)2, PropertyName.Index, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName.IsDown, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		StringName index = PropertyName.Index;
		int index2 = Index;
		info.AddProperty(index, Variant.From<int>(ref index2));
		StringName isDown = PropertyName.IsDown;
		bool isDown2 = IsDown;
		info.AddProperty(isDown, Variant.From<bool>(ref isDown2));
		info.AddProperty(PropertyName._grabMarker, Variant.From<Marker2D>(ref _grabMarker));
		info.AddProperty(PropertyName._textureRect, Variant.From<TextureRect>(ref _textureRect));
		info.AddProperty(PropertyName._currentVelocity, Variant.From<Vector2>(ref _currentVelocity));
		info.AddProperty(PropertyName._desiredPosition, Variant.From<Vector2>(ref _desiredPosition));
		info.AddProperty(PropertyName._downTween, Variant.From<Tween>(ref _downTween));
		info.AddProperty(PropertyName._state, Variant.From<State>(ref _state));
		info.AddProperty(PropertyName._isInFight, Variant.From<bool>(ref _isInFight));
		info.AddProperty(PropertyName._originalPosition, Variant.From<Vector2>(ref _originalPosition));
		info.AddProperty(PropertyName._handAnimateInProgress, Variant.From<float>(ref _handAnimateInProgress));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName.Index, ref val))
		{
			Index = ((Variant)(ref val)).As<int>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName.IsDown, ref val2))
		{
			IsDown = ((Variant)(ref val2)).As<bool>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._grabMarker, ref val3))
		{
			_grabMarker = ((Variant)(ref val3)).As<Marker2D>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._textureRect, ref val4))
		{
			_textureRect = ((Variant)(ref val4)).As<TextureRect>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._currentVelocity, ref val5))
		{
			_currentVelocity = ((Variant)(ref val5)).As<Vector2>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._desiredPosition, ref val6))
		{
			_desiredPosition = ((Variant)(ref val6)).As<Vector2>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName._downTween, ref val7))
		{
			_downTween = ((Variant)(ref val7)).As<Tween>();
		}
		Variant val8 = default(Variant);
		if (info.TryGetProperty(PropertyName._state, ref val8))
		{
			_state = ((Variant)(ref val8)).As<State>();
		}
		Variant val9 = default(Variant);
		if (info.TryGetProperty(PropertyName._isInFight, ref val9))
		{
			_isInFight = ((Variant)(ref val9)).As<bool>();
		}
		Variant val10 = default(Variant);
		if (info.TryGetProperty(PropertyName._originalPosition, ref val10))
		{
			_originalPosition = ((Variant)(ref val10)).As<Vector2>();
		}
		Variant val11 = default(Variant);
		if (info.TryGetProperty(PropertyName._handAnimateInProgress, ref val11))
		{
			_handAnimateInProgress = ((Variant)(ref val11)).As<float>();
		}
	}
}
