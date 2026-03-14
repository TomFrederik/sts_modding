using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.Collections;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx.Utilities;
using MegaCrit.Sts2.Core.TestSupport;

namespace MegaCrit.Sts2.Core.Nodes.Vfx;

[ScriptPath("res://src/Core/Nodes/Vfx/NMinionDiveBombVfx.cs")]
public class NMinionDiveBombVfx : Node2D
{
	public class MethodName : MethodName
	{
		public static readonly StringName Create = StringName.op_Implicit("Create");

		public static readonly StringName Initialize = StringName.op_Implicit("Initialize");

		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName SetMinionVisible = StringName.op_Implicit("SetMinionVisible");

		public static readonly StringName UpdateMinionSprite = StringName.op_Implicit("UpdateMinionSprite");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName SourceFinalPosition = StringName.op_Implicit("SourceFinalPosition");

		public static readonly StringName DestinationFinalPosition = StringName.op_Implicit("DestinationFinalPosition");

		public static readonly StringName _minionSprite = StringName.op_Implicit("_minionSprite");

		public static readonly StringName _minionTextures = StringName.op_Implicit("_minionTextures");

		public static readonly StringName _minionAnimator = StringName.op_Implicit("_minionAnimator");

		public static readonly StringName _minionAnimations = StringName.op_Implicit("_minionAnimations");

		public static readonly StringName _minionVfx = StringName.op_Implicit("_minionVfx");

		public static readonly StringName _fallingTrail = StringName.op_Implicit("_fallingTrail");

		public static readonly StringName _fallingVfx = StringName.op_Implicit("_fallingVfx");

		public static readonly StringName _impactVfx = StringName.op_Implicit("_impactVfx");

		public static readonly StringName _flightTime = StringName.op_Implicit("_flightTime");

		public static readonly StringName _fallingVfxEntryTime = StringName.op_Implicit("_fallingVfxEntryTime");

		public static readonly StringName _horizontalCurve = StringName.op_Implicit("_horizontalCurve");

		public static readonly StringName _verticalCurve = StringName.op_Implicit("_verticalCurve");

		public static readonly StringName _textureCurve = StringName.op_Implicit("_textureCurve");

		public static readonly StringName _maxHeight = StringName.op_Implicit("_maxHeight");

		public static readonly StringName _sourceOffset = StringName.op_Implicit("_sourceOffset");

		public static readonly StringName _destinationOffset = StringName.op_Implicit("_destinationOffset");

		public static readonly StringName _previousIndex = StringName.op_Implicit("_previousIndex");

		public static readonly StringName _sourcePosition = StringName.op_Implicit("_sourcePosition");

		public static readonly StringName _destinationPosition = StringName.op_Implicit("_destinationPosition");
	}

	public class SignalName : SignalName
	{
	}

	public static readonly string scenePath = SceneHelper.GetScenePath("vfx/vfx_minion_dive_bomb");

	[Export(/*Could not decode attribute arguments.*/)]
	private Sprite2D? _minionSprite;

	[Export(/*Could not decode attribute arguments.*/)]
	private Array<Texture2D>? _minionTextures;

	[Export(/*Could not decode attribute arguments.*/)]
	private AnimationPlayer? _minionAnimator;

	[Export(/*Could not decode attribute arguments.*/)]
	private Array<string>? _minionAnimations;

	[Export(/*Could not decode attribute arguments.*/)]
	private Array<NParticlesContainer>? _minionVfx;

	[Export(/*Could not decode attribute arguments.*/)]
	private Node2D? _fallingTrail;

	[Export(/*Could not decode attribute arguments.*/)]
	private NParticlesContainer? _fallingVfx;

	[Export(/*Could not decode attribute arguments.*/)]
	private NParticlesContainer? _impactVfx;

	[Export(/*Could not decode attribute arguments.*/)]
	private float _flightTime;

	[Export(/*Could not decode attribute arguments.*/)]
	private float _fallingVfxEntryTime;

	[Export(/*Could not decode attribute arguments.*/)]
	private Curve? _horizontalCurve;

	[Export(/*Could not decode attribute arguments.*/)]
	private Curve? _verticalCurve;

	[Export(/*Could not decode attribute arguments.*/)]
	private Curve? _textureCurve;

	[Export(/*Could not decode attribute arguments.*/)]
	private float _maxHeight;

	[Export(/*Could not decode attribute arguments.*/)]
	private Vector2 _sourceOffset;

	[Export(/*Could not decode attribute arguments.*/)]
	private Vector2 _destinationOffset;

	private int _previousIndex = -1;

	private Vector2 _sourcePosition;

	private Vector2 _destinationPosition;

	private Vector2 SourceFinalPosition => _sourcePosition + _sourceOffset;

	private Vector2 DestinationFinalPosition => _destinationPosition + _destinationOffset;

	public static NMinionDiveBombVfx? Create(Creature owner, Creature target)
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		if (TestMode.IsOn)
		{
			return null;
		}
		NCreature nCreature = NCombatRoom.Instance?.GetCreatureNode(owner);
		NCreature nCreature2 = NCombatRoom.Instance?.GetCreatureNode(target);
		if (nCreature2 != null && nCreature != null)
		{
			return Create(nCreature.VfxSpawnPosition, nCreature2.GetBottomOfHitbox());
		}
		return null;
	}

	public static NMinionDiveBombVfx? Create(Vector2 playerCenterPosition, Vector2 targetFloorPosition)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		if (TestMode.IsOn)
		{
			return null;
		}
		NMinionDiveBombVfx nMinionDiveBombVfx = PreloadManager.Cache.GetScene(scenePath).Instantiate<NMinionDiveBombVfx>((GenEditState)0);
		nMinionDiveBombVfx.Initialize(playerCenterPosition, targetFloorPosition);
		return nMinionDiveBombVfx;
	}

	private void Initialize(Vector2 sourcePosition, Vector2 destinationPosition)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		_sourcePosition = sourcePosition;
		_destinationPosition = destinationPosition;
		((CanvasItem)_fallingTrail).Visible = true;
		((Node2D)this).GlobalPosition = sourcePosition;
		for (int i = 0; i < _minionVfx.Count; i++)
		{
			_minionVfx[i].SetEmitting(emitting: false);
		}
	}

	public override void _Ready()
	{
		TaskHelper.RunSafely(PlaySequence());
	}

	private void SetMinionVisible(bool visible)
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		((CanvasItem)_minionSprite).SelfModulate = (visible ? new Color(1f, 1f, 1f, 1f) : new Color(1f, 1f, 1f, 0f));
	}

	private void UpdateMinionSprite(int index)
	{
		if (_previousIndex == index)
		{
			return;
		}
		_previousIndex = index;
		Texture2D texture = _minionTextures[Mathf.Clamp(index, 0, _minionTextures.Count - 1)];
		_minionSprite.Texture = texture;
		string text = _minionAnimations[Mathf.Clamp(index, 0, _minionAnimations.Count - 1)];
		if (!_minionAnimator.CurrentAnimation.Equals(text))
		{
			_minionAnimator.Play(StringName.op_Implicit(text), -1.0, 1f, false);
		}
		for (int i = 0; i < _minionVfx.Count; i++)
		{
			if (i == index)
			{
				_minionVfx[i].Restart();
			}
		}
	}

	private async Task PlaySequence()
	{
		Vector2 startPos = SourceFinalPosition;
		Vector2 endPos = DestinationFinalPosition;
		UpdateMinionSprite(0);
		((Node2D)_minionSprite).GlobalPosition = startPos;
		SetMinionVisible(visible: true);
		double timer = 0.0;
		bool isPlayingFallingVfx = false;
		while (timer < (double)_flightTime)
		{
			float num = (float)timer / _flightTime;
			float num2 = _horizontalCurve.Sample(num);
			float num3 = _verticalCurve.Sample(num);
			float num4 = _textureCurve.Sample(num);
			UpdateMinionSprite(Mathf.FloorToInt(num4));
			Vector2 val = ((Vector2)(ref startPos)).Lerp(endPos, num2);
			val += Vector2.Up * num3 * _maxHeight;
			((Node2D)_minionSprite).GlobalPosition = val;
			((Node2D)_fallingVfx).GlobalPosition = val;
			if (timer >= (double)_fallingVfxEntryTime && !isPlayingFallingVfx)
			{
				_fallingVfx.Restart();
				((CanvasItem)_fallingTrail).Visible = true;
				isPlayingFallingVfx = true;
			}
			timer += ((Node)this).GetProcessDeltaTime();
			await ((GodotObject)this).ToSignal((GodotObject)(object)((Node)this).GetTree(), SignalName.ProcessFrame);
		}
		SetMinionVisible(visible: false);
		NGame.Instance?.ScreenShake(ShakeStrength.Weak, ShakeDuration.Short);
		((Node2D)_impactVfx).GlobalPosition = _destinationPosition;
		_impactVfx.Restart();
		((CanvasItem)_fallingTrail).Visible = false;
		_fallingVfx.SetEmitting(emitting: false);
		await Cmd.Wait(2f);
		((Node)(object)this).QueueFreeSafely();
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Expected O, but got Unknown
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(5);
		list.Add(new MethodInfo(MethodName.Create, new PropertyInfo((Type)24, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Node2D"), false), (MethodFlags)33, new List<PropertyInfo>
		{
			new PropertyInfo((Type)5, StringName.op_Implicit("playerCenterPosition"), (PropertyHint)0, "", (PropertyUsageFlags)6, false),
			new PropertyInfo((Type)5, StringName.op_Implicit("targetFloorPosition"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.Initialize, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)5, StringName.op_Implicit("sourcePosition"), (PropertyHint)0, "", (PropertyUsageFlags)6, false),
			new PropertyInfo((Type)5, StringName.op_Implicit("destinationPosition"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetMinionVisible, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)1, StringName.op_Implicit("visible"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UpdateMinionSprite, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("index"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.Create && ((NativeVariantPtrArgs)(ref args)).Count == 2)
		{
			NMinionDiveBombVfx nMinionDiveBombVfx = Create(VariantUtils.ConvertTo<Vector2>(ref ((NativeVariantPtrArgs)(ref args))[0]), VariantUtils.ConvertTo<Vector2>(ref ((NativeVariantPtrArgs)(ref args))[1]));
			ret = VariantUtils.CreateFrom<NMinionDiveBombVfx>(ref nMinionDiveBombVfx);
			return true;
		}
		if ((ref method) == MethodName.Initialize && ((NativeVariantPtrArgs)(ref args)).Count == 2)
		{
			Initialize(VariantUtils.ConvertTo<Vector2>(ref ((NativeVariantPtrArgs)(ref args))[0]), VariantUtils.ConvertTo<Vector2>(ref ((NativeVariantPtrArgs)(ref args))[1]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetMinionVisible && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			SetMinionVisible(VariantUtils.ConvertTo<bool>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.UpdateMinionSprite && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			UpdateMinionSprite(VariantUtils.ConvertTo<int>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		return ((Node2D)this).InvokeGodotClassMethod(ref method, args, ref ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static bool InvokeGodotClassStaticMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.Create && ((NativeVariantPtrArgs)(ref args)).Count == 2)
		{
			NMinionDiveBombVfx nMinionDiveBombVfx = Create(VariantUtils.ConvertTo<Vector2>(ref ((NativeVariantPtrArgs)(ref args))[0]), VariantUtils.ConvertTo<Vector2>(ref ((NativeVariantPtrArgs)(ref args))[1]));
			ret = VariantUtils.CreateFrom<NMinionDiveBombVfx>(ref nMinionDiveBombVfx);
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
		if ((ref method) == MethodName.Initialize)
		{
			return true;
		}
		if ((ref method) == MethodName._Ready)
		{
			return true;
		}
		if ((ref method) == MethodName.SetMinionVisible)
		{
			return true;
		}
		if ((ref method) == MethodName.UpdateMinionSprite)
		{
			return true;
		}
		return ((Node2D)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName._minionSprite)
		{
			_minionSprite = VariantUtils.ConvertTo<Sprite2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._minionTextures)
		{
			_minionTextures = VariantUtils.ConvertToArray<Texture2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._minionAnimator)
		{
			_minionAnimator = VariantUtils.ConvertTo<AnimationPlayer>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._minionAnimations)
		{
			_minionAnimations = VariantUtils.ConvertToArray<string>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._minionVfx)
		{
			_minionVfx = VariantUtils.ConvertToArray<NParticlesContainer>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._fallingTrail)
		{
			_fallingTrail = VariantUtils.ConvertTo<Node2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._fallingVfx)
		{
			_fallingVfx = VariantUtils.ConvertTo<NParticlesContainer>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._impactVfx)
		{
			_impactVfx = VariantUtils.ConvertTo<NParticlesContainer>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._flightTime)
		{
			_flightTime = VariantUtils.ConvertTo<float>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._fallingVfxEntryTime)
		{
			_fallingVfxEntryTime = VariantUtils.ConvertTo<float>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._horizontalCurve)
		{
			_horizontalCurve = VariantUtils.ConvertTo<Curve>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._verticalCurve)
		{
			_verticalCurve = VariantUtils.ConvertTo<Curve>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._textureCurve)
		{
			_textureCurve = VariantUtils.ConvertTo<Curve>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._maxHeight)
		{
			_maxHeight = VariantUtils.ConvertTo<float>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._sourceOffset)
		{
			_sourceOffset = VariantUtils.ConvertTo<Vector2>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._destinationOffset)
		{
			_destinationOffset = VariantUtils.ConvertTo<Vector2>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._previousIndex)
		{
			_previousIndex = VariantUtils.ConvertTo<int>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._sourcePosition)
		{
			_sourcePosition = VariantUtils.ConvertTo<Vector2>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._destinationPosition)
		{
			_destinationPosition = VariantUtils.ConvertTo<Vector2>(ref value);
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
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		//IL_029a: Unknown result type (might be due to invalid IL or missing references)
		//IL_029f: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName.SourceFinalPosition)
		{
			Vector2 sourceFinalPosition = SourceFinalPosition;
			value = VariantUtils.CreateFrom<Vector2>(ref sourceFinalPosition);
			return true;
		}
		if ((ref name) == PropertyName.DestinationFinalPosition)
		{
			Vector2 sourceFinalPosition = DestinationFinalPosition;
			value = VariantUtils.CreateFrom<Vector2>(ref sourceFinalPosition);
			return true;
		}
		if ((ref name) == PropertyName._minionSprite)
		{
			value = VariantUtils.CreateFrom<Sprite2D>(ref _minionSprite);
			return true;
		}
		if ((ref name) == PropertyName._minionTextures)
		{
			value = VariantUtils.CreateFromArray<Texture2D>(_minionTextures);
			return true;
		}
		if ((ref name) == PropertyName._minionAnimator)
		{
			value = VariantUtils.CreateFrom<AnimationPlayer>(ref _minionAnimator);
			return true;
		}
		if ((ref name) == PropertyName._minionAnimations)
		{
			value = VariantUtils.CreateFromArray<string>(_minionAnimations);
			return true;
		}
		if ((ref name) == PropertyName._minionVfx)
		{
			value = VariantUtils.CreateFromArray<NParticlesContainer>(_minionVfx);
			return true;
		}
		if ((ref name) == PropertyName._fallingTrail)
		{
			value = VariantUtils.CreateFrom<Node2D>(ref _fallingTrail);
			return true;
		}
		if ((ref name) == PropertyName._fallingVfx)
		{
			value = VariantUtils.CreateFrom<NParticlesContainer>(ref _fallingVfx);
			return true;
		}
		if ((ref name) == PropertyName._impactVfx)
		{
			value = VariantUtils.CreateFrom<NParticlesContainer>(ref _impactVfx);
			return true;
		}
		if ((ref name) == PropertyName._flightTime)
		{
			value = VariantUtils.CreateFrom<float>(ref _flightTime);
			return true;
		}
		if ((ref name) == PropertyName._fallingVfxEntryTime)
		{
			value = VariantUtils.CreateFrom<float>(ref _fallingVfxEntryTime);
			return true;
		}
		if ((ref name) == PropertyName._horizontalCurve)
		{
			value = VariantUtils.CreateFrom<Curve>(ref _horizontalCurve);
			return true;
		}
		if ((ref name) == PropertyName._verticalCurve)
		{
			value = VariantUtils.CreateFrom<Curve>(ref _verticalCurve);
			return true;
		}
		if ((ref name) == PropertyName._textureCurve)
		{
			value = VariantUtils.CreateFrom<Curve>(ref _textureCurve);
			return true;
		}
		if ((ref name) == PropertyName._maxHeight)
		{
			value = VariantUtils.CreateFrom<float>(ref _maxHeight);
			return true;
		}
		if ((ref name) == PropertyName._sourceOffset)
		{
			value = VariantUtils.CreateFrom<Vector2>(ref _sourceOffset);
			return true;
		}
		if ((ref name) == PropertyName._destinationOffset)
		{
			value = VariantUtils.CreateFrom<Vector2>(ref _destinationOffset);
			return true;
		}
		if ((ref name) == PropertyName._previousIndex)
		{
			value = VariantUtils.CreateFrom<int>(ref _previousIndex);
			return true;
		}
		if ((ref name) == PropertyName._sourcePosition)
		{
			value = VariantUtils.CreateFrom<Vector2>(ref _sourcePosition);
			return true;
		}
		if ((ref name) == PropertyName._destinationPosition)
		{
			value = VariantUtils.CreateFrom<Vector2>(ref _destinationPosition);
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
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		//IL_0272: Unknown result type (might be due to invalid IL or missing references)
		//IL_0292: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._minionSprite, (PropertyHint)34, "Sprite2D", (PropertyUsageFlags)4102, true));
		list.Add(new PropertyInfo((Type)28, PropertyName._minionTextures, (PropertyHint)23, "24/17:Texture2D", (PropertyUsageFlags)4102, true));
		list.Add(new PropertyInfo((Type)24, PropertyName._minionAnimator, (PropertyHint)34, "AnimationPlayer", (PropertyUsageFlags)4102, true));
		list.Add(new PropertyInfo((Type)28, PropertyName._minionAnimations, (PropertyHint)23, "4/0:", (PropertyUsageFlags)4102, true));
		list.Add(new PropertyInfo((Type)28, PropertyName._minionVfx, (PropertyHint)23, "24/34:Node2D", (PropertyUsageFlags)4102, true));
		list.Add(new PropertyInfo((Type)24, PropertyName._fallingTrail, (PropertyHint)34, "Node2D", (PropertyUsageFlags)4102, true));
		list.Add(new PropertyInfo((Type)24, PropertyName._fallingVfx, (PropertyHint)34, "Node2D", (PropertyUsageFlags)4102, true));
		list.Add(new PropertyInfo((Type)24, PropertyName._impactVfx, (PropertyHint)34, "Node2D", (PropertyUsageFlags)4102, true));
		list.Add(new PropertyInfo((Type)3, PropertyName._flightTime, (PropertyHint)0, "", (PropertyUsageFlags)4102, true));
		list.Add(new PropertyInfo((Type)3, PropertyName._fallingVfxEntryTime, (PropertyHint)0, "", (PropertyUsageFlags)4102, true));
		list.Add(new PropertyInfo((Type)24, PropertyName._horizontalCurve, (PropertyHint)17, "Curve", (PropertyUsageFlags)4102, true));
		list.Add(new PropertyInfo((Type)24, PropertyName._verticalCurve, (PropertyHint)17, "Curve", (PropertyUsageFlags)4102, true));
		list.Add(new PropertyInfo((Type)24, PropertyName._textureCurve, (PropertyHint)17, "Curve", (PropertyUsageFlags)4102, true));
		list.Add(new PropertyInfo((Type)3, PropertyName._maxHeight, (PropertyHint)0, "", (PropertyUsageFlags)4102, true));
		list.Add(new PropertyInfo((Type)5, PropertyName._sourceOffset, (PropertyHint)0, "", (PropertyUsageFlags)4102, true));
		list.Add(new PropertyInfo((Type)5, PropertyName._destinationOffset, (PropertyHint)0, "", (PropertyUsageFlags)4102, true));
		list.Add(new PropertyInfo((Type)2, PropertyName._previousIndex, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)5, PropertyName._sourcePosition, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)5, PropertyName._destinationPosition, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)5, PropertyName.SourceFinalPosition, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)5, PropertyName.DestinationFinalPosition, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		info.AddProperty(PropertyName._minionSprite, Variant.From<Sprite2D>(ref _minionSprite));
		info.AddProperty(PropertyName._minionTextures, Variant.CreateFrom<Texture2D>(_minionTextures));
		info.AddProperty(PropertyName._minionAnimator, Variant.From<AnimationPlayer>(ref _minionAnimator));
		info.AddProperty(PropertyName._minionAnimations, Variant.CreateFrom<string>(_minionAnimations));
		info.AddProperty(PropertyName._minionVfx, Variant.CreateFrom<NParticlesContainer>(_minionVfx));
		info.AddProperty(PropertyName._fallingTrail, Variant.From<Node2D>(ref _fallingTrail));
		info.AddProperty(PropertyName._fallingVfx, Variant.From<NParticlesContainer>(ref _fallingVfx));
		info.AddProperty(PropertyName._impactVfx, Variant.From<NParticlesContainer>(ref _impactVfx));
		info.AddProperty(PropertyName._flightTime, Variant.From<float>(ref _flightTime));
		info.AddProperty(PropertyName._fallingVfxEntryTime, Variant.From<float>(ref _fallingVfxEntryTime));
		info.AddProperty(PropertyName._horizontalCurve, Variant.From<Curve>(ref _horizontalCurve));
		info.AddProperty(PropertyName._verticalCurve, Variant.From<Curve>(ref _verticalCurve));
		info.AddProperty(PropertyName._textureCurve, Variant.From<Curve>(ref _textureCurve));
		info.AddProperty(PropertyName._maxHeight, Variant.From<float>(ref _maxHeight));
		info.AddProperty(PropertyName._sourceOffset, Variant.From<Vector2>(ref _sourceOffset));
		info.AddProperty(PropertyName._destinationOffset, Variant.From<Vector2>(ref _destinationOffset));
		info.AddProperty(PropertyName._previousIndex, Variant.From<int>(ref _previousIndex));
		info.AddProperty(PropertyName._sourcePosition, Variant.From<Vector2>(ref _sourcePosition));
		info.AddProperty(PropertyName._destinationPosition, Variant.From<Vector2>(ref _destinationPosition));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._minionSprite, ref val))
		{
			_minionSprite = ((Variant)(ref val)).As<Sprite2D>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._minionTextures, ref val2))
		{
			_minionTextures = ((Variant)(ref val2)).AsGodotArray<Texture2D>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._minionAnimator, ref val3))
		{
			_minionAnimator = ((Variant)(ref val3)).As<AnimationPlayer>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._minionAnimations, ref val4))
		{
			_minionAnimations = ((Variant)(ref val4)).AsGodotArray<string>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._minionVfx, ref val5))
		{
			_minionVfx = ((Variant)(ref val5)).AsGodotArray<NParticlesContainer>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._fallingTrail, ref val6))
		{
			_fallingTrail = ((Variant)(ref val6)).As<Node2D>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName._fallingVfx, ref val7))
		{
			_fallingVfx = ((Variant)(ref val7)).As<NParticlesContainer>();
		}
		Variant val8 = default(Variant);
		if (info.TryGetProperty(PropertyName._impactVfx, ref val8))
		{
			_impactVfx = ((Variant)(ref val8)).As<NParticlesContainer>();
		}
		Variant val9 = default(Variant);
		if (info.TryGetProperty(PropertyName._flightTime, ref val9))
		{
			_flightTime = ((Variant)(ref val9)).As<float>();
		}
		Variant val10 = default(Variant);
		if (info.TryGetProperty(PropertyName._fallingVfxEntryTime, ref val10))
		{
			_fallingVfxEntryTime = ((Variant)(ref val10)).As<float>();
		}
		Variant val11 = default(Variant);
		if (info.TryGetProperty(PropertyName._horizontalCurve, ref val11))
		{
			_horizontalCurve = ((Variant)(ref val11)).As<Curve>();
		}
		Variant val12 = default(Variant);
		if (info.TryGetProperty(PropertyName._verticalCurve, ref val12))
		{
			_verticalCurve = ((Variant)(ref val12)).As<Curve>();
		}
		Variant val13 = default(Variant);
		if (info.TryGetProperty(PropertyName._textureCurve, ref val13))
		{
			_textureCurve = ((Variant)(ref val13)).As<Curve>();
		}
		Variant val14 = default(Variant);
		if (info.TryGetProperty(PropertyName._maxHeight, ref val14))
		{
			_maxHeight = ((Variant)(ref val14)).As<float>();
		}
		Variant val15 = default(Variant);
		if (info.TryGetProperty(PropertyName._sourceOffset, ref val15))
		{
			_sourceOffset = ((Variant)(ref val15)).As<Vector2>();
		}
		Variant val16 = default(Variant);
		if (info.TryGetProperty(PropertyName._destinationOffset, ref val16))
		{
			_destinationOffset = ((Variant)(ref val16)).As<Vector2>();
		}
		Variant val17 = default(Variant);
		if (info.TryGetProperty(PropertyName._previousIndex, ref val17))
		{
			_previousIndex = ((Variant)(ref val17)).As<int>();
		}
		Variant val18 = default(Variant);
		if (info.TryGetProperty(PropertyName._sourcePosition, ref val18))
		{
			_sourcePosition = ((Variant)(ref val18)).As<Vector2>();
		}
		Variant val19 = default(Variant);
		if (info.TryGetProperty(PropertyName._destinationPosition, ref val19))
		{
			_destinationPosition = ((Variant)(ref val19)).As<Vector2>();
		}
	}
}
