using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx.Utilities;
using MegaCrit.Sts2.Core.TestSupport;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.Nodes.Vfx;

[ScriptPath("res://src/Core/Nodes/Vfx/NSpeechBubbleVfx.cs")]
public class NSpeechBubbleVfx : Control
{
	public class MethodName : MethodName
	{
		public static readonly StringName Create = StringName.op_Implicit("Create");

		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName SetSpeechBubbleColor = StringName.op_Implicit("SetSpeechBubbleColor");

		public static readonly StringName _ExitTree = StringName.op_Implicit("_ExitTree");

		public static readonly StringName CreateInternal = StringName.op_Implicit("CreateInternal");

		public static readonly StringName _Process = StringName.op_Implicit("_Process");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName SecondsToDisplay = StringName.op_Implicit("SecondsToDisplay");

		public static readonly StringName _container = StringName.op_Implicit("_container");

		public static readonly StringName _label = StringName.op_Implicit("_label");

		public static readonly StringName _contents = StringName.op_Implicit("_contents");

		public static readonly StringName _bubble = StringName.op_Implicit("_bubble");

		public static readonly StringName _shadow = StringName.op_Implicit("_shadow");

		public static readonly StringName _hsv = StringName.op_Implicit("_hsv");

		public static readonly StringName _tween = StringName.op_Implicit("_tween");

		public static readonly StringName _startPos = StringName.op_Implicit("_startPos");

		public static readonly StringName _vfxColor = StringName.op_Implicit("_vfxColor");

		public static readonly StringName _style = StringName.op_Implicit("_style");

		public static readonly StringName _side = StringName.op_Implicit("_side");

		public static readonly StringName _text = StringName.op_Implicit("_text");

		public static readonly StringName _elapsedTime = StringName.op_Implicit("_elapsedTime");
	}

	public class SignalName : SignalName
	{
	}

	private static readonly StringName _v = new StringName("v");

	private static readonly StringName _s = new StringName("s");

	private static readonly StringName _h = new StringName("h");

	private Control _container;

	private MegaRichTextLabel _label;

	private Node2D _contents;

	private Sprite2D _bubble;

	private Sprite2D _shadow;

	private ShaderMaterial _hsv;

	private const string _path = "res://scenes/vfx/vfx_speech_bubble.tscn";

	private Tween? _tween;

	private const float _spawnProportionToTopOfHitbox = 0.75f;

	private const float _spawnProportionToEdgeOfHitbox = 0.75f;

	private Vector2 _startPos;

	private VfxColor _vfxColor;

	private DialogueStyle _style;

	private DialogueSide _side;

	private string _text;

	private float _elapsedTime = 3.14f;

	private const float _waveFrequency = 4.5f;

	private const float _waveAmplitude = 2f;

	public static IEnumerable<string> AssetPaths => new _003C_003Ez__ReadOnlySingleElementList<string>("res://scenes/vfx/vfx_speech_bubble.tscn");

	public double SecondsToDisplay { get; private set; }

	public static NSpeechBubbleVfx? Create(string text, Creature speaker, double secondsToDisplay, VfxColor vfxColor = VfxColor.White)
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		if (TestMode.IsOn)
		{
			return null;
		}
		NSpeechBubbleVfx nSpeechBubbleVfx = CreateInternal(text, speaker.Side switch
		{
			CombatSide.Player => DialogueSide.Left, 
			CombatSide.Enemy => DialogueSide.Right, 
			_ => throw new ArgumentOutOfRangeException(), 
		}, secondsToDisplay);
		nSpeechBubbleVfx._startPos = GetCreatureSpeechPosition(speaker);
		nSpeechBubbleVfx._vfxColor = vfxColor;
		return nSpeechBubbleVfx;
	}

	public static NSpeechBubbleVfx? Create(string text, DialogueSide side, Vector2 globalPosition, double secondsToDisplay, VfxColor vfxColor = VfxColor.White)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		if (TestMode.IsOn)
		{
			return null;
		}
		NSpeechBubbleVfx nSpeechBubbleVfx = CreateInternal(text, side, secondsToDisplay);
		nSpeechBubbleVfx._startPos = globalPosition;
		nSpeechBubbleVfx._vfxColor = vfxColor;
		return nSpeechBubbleVfx;
	}

	public override void _Ready()
	{
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Expected O, but got Unknown
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		_contents = ((Node)this).GetNode<Node2D>(NodePath.op_Implicit("%Contents"));
		_bubble = ((Node)this).GetNode<Sprite2D>(NodePath.op_Implicit("%Bubble"));
		_shadow = ((Node)this).GetNode<Sprite2D>(NodePath.op_Implicit("%Shadow"));
		_container = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%Container"));
		_label = ((Node)this).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("%Text"));
		_hsv = (ShaderMaterial)((CanvasItem)_bubble).Material;
		SetSpeechBubbleColor();
		((Control)this).GlobalPosition = _startPos;
		if (_side == DialogueSide.Right)
		{
			_container.Position = new Vector2(0f - _container.Size.X - _container.Position.X, _container.Position.Y);
			_bubble.FlipH = true;
			_shadow.FlipH = true;
		}
		TaskHelper.RunSafely(AnimateSpeechBubble());
	}

	private void SetSpeechBubbleColor()
	{
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		switch (_vfxColor)
		{
		case VfxColor.Blue:
			_hsv.SetShaderParameter(_h, Variant.op_Implicit(0.05f));
			_hsv.SetShaderParameter(_s, Variant.op_Implicit(1.3f));
			_hsv.SetShaderParameter(_v, Variant.op_Implicit(0.55f));
			break;
		case VfxColor.Green:
			_hsv.SetShaderParameter(_h, Variant.op_Implicit(0.8f));
			_hsv.SetShaderParameter(_s, Variant.op_Implicit(1.5f));
			_hsv.SetShaderParameter(_v, Variant.op_Implicit(0.6f));
			break;
		case VfxColor.Purple:
			_hsv.SetShaderParameter(_h, Variant.op_Implicit(0.3f));
			_hsv.SetShaderParameter(_s, Variant.op_Implicit(0.6f));
			_hsv.SetShaderParameter(_v, Variant.op_Implicit(0.5f));
			break;
		case VfxColor.Red:
			_hsv.SetShaderParameter(_h, Variant.op_Implicit(0.48f));
			_hsv.SetShaderParameter(_s, Variant.op_Implicit(2f));
			_hsv.SetShaderParameter(_v, Variant.op_Implicit(0.5f));
			break;
		case VfxColor.Black:
			_hsv.SetShaderParameter(_h, Variant.op_Implicit(1f));
			_hsv.SetShaderParameter(_s, Variant.op_Implicit(0.25f));
			_hsv.SetShaderParameter(_v, Variant.op_Implicit(0.3f));
			break;
		default:
			_hsv.SetShaderParameter(_h, Variant.op_Implicit(1f));
			_hsv.SetShaderParameter(_s, Variant.op_Implicit(0.9f));
			_hsv.SetShaderParameter(_v, Variant.op_Implicit(0.5f));
			break;
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

	private async Task AnimateSpeechBubble()
	{
		_tween = ((Node)this).CreateTween().SetParallel(true);
		float value = 60f * ((_side == DialogueSide.Left) ? (-1f) : 1f);
		_label.Text = $"[center][fly_in offset_x={value} offset_y=40]{_text}[/fly_in][/center]";
		_tween.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(1f), 0.4).From(Variant.op_Implicit(0f));
		_tween.TweenProperty((GodotObject)(object)_label, NodePath.op_Implicit("visible_ratio"), Variant.op_Implicit(1f), 0.4).From(Variant.op_Implicit(0f));
		_tween.TweenProperty((GodotObject)(object)_bubble, NodePath.op_Implicit("scale"), Variant.op_Implicit(new Vector2(0.75f, 0.75f)), 0.5).From(Variant.op_Implicit(new Vector2(0.25f, 0.25f))).SetEase((EaseType)1)
			.SetTrans((TransitionType)5);
		_tween.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("rotation_degrees"), Variant.op_Implicit(0f), 0.3).From(Variant.op_Implicit(7f)).SetEase((EaseType)1)
			.SetTrans((TransitionType)1);
		_tween.Chain();
		double num = Math.Max(SecondsToDisplay - 1.0, 1.0);
		_tween.TweenInterval(num);
		_tween.Chain();
		await AnimOutInternal();
	}

	public async Task AnimOut()
	{
		Tween? tween = _tween;
		if (tween != null)
		{
			tween.Kill();
		}
		_tween = ((Node)this).CreateTween().SetParallel(true);
		await AnimOutInternal();
	}

	private async Task AnimOutInternal()
	{
		_tween.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("modulate"), Variant.op_Implicit(StsColors.transparentBlack), 0.4).SetEase((EaseType)1).SetTrans((TransitionType)1);
		await ((GodotObject)this).ToSignal((GodotObject)(object)_tween, SignalName.Finished);
		((Node)(object)this).QueueFreeSafely();
	}

	private static NSpeechBubbleVfx CreateInternal(string text, DialogueSide side, double secondsToDisplay)
	{
		NSpeechBubbleVfx nSpeechBubbleVfx = PreloadManager.Cache.GetScene("res://scenes/vfx/vfx_speech_bubble.tscn").Instantiate<NSpeechBubbleVfx>((GenEditState)0);
		nSpeechBubbleVfx._side = side;
		nSpeechBubbleVfx._text = text;
		nSpeechBubbleVfx.SecondsToDisplay = secondsToDisplay;
		return nSpeechBubbleVfx;
	}

	public override void _Process(double delta)
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		_elapsedTime += (float)delta * 4.5f;
		_contents.Position = new Vector2(0f, Mathf.Sin(_elapsedTime) * 2f);
	}

	private static Vector2 GetCreatureSpeechPosition(Creature speaker)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		NCreature creatureNode = NCombatRoom.Instance.GetCreatureNode(speaker);
		if (creatureNode.Visuals.TalkPosition != null)
		{
			return ((Node2D)creatureNode.Visuals.TalkPosition).GlobalPosition;
		}
		Vector2 result = creatureNode.VfxSpawnPosition + new Vector2(0f, (0f - creatureNode.Hitbox.Size.Y) * 0.5f * 0.75f);
		if (speaker.Side == CombatSide.Player)
		{
			result.X += creatureNode.Hitbox.Size.X * 0.75f;
		}
		else
		{
			result.X -= creatureNode.Hitbox.Size.X * 0.75f;
		}
		return result;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Expected O, but got Unknown
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Expected O, but got Unknown
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(6);
		list.Add(new MethodInfo(MethodName.Create, new PropertyInfo((Type)24, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false), (MethodFlags)33, new List<PropertyInfo>
		{
			new PropertyInfo((Type)4, StringName.op_Implicit("text"), (PropertyHint)0, "", (PropertyUsageFlags)6, false),
			new PropertyInfo((Type)2, StringName.op_Implicit("side"), (PropertyHint)0, "", (PropertyUsageFlags)6, false),
			new PropertyInfo((Type)5, StringName.op_Implicit("globalPosition"), (PropertyHint)0, "", (PropertyUsageFlags)6, false),
			new PropertyInfo((Type)3, StringName.op_Implicit("secondsToDisplay"), (PropertyHint)0, "", (PropertyUsageFlags)6, false),
			new PropertyInfo((Type)2, StringName.op_Implicit("vfxColor"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetSpeechBubbleColor, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._ExitTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.CreateInternal, new PropertyInfo((Type)24, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false), (MethodFlags)33, new List<PropertyInfo>
		{
			new PropertyInfo((Type)4, StringName.op_Implicit("text"), (PropertyHint)0, "", (PropertyUsageFlags)6, false),
			new PropertyInfo((Type)2, StringName.op_Implicit("side"), (PropertyHint)0, "", (PropertyUsageFlags)6, false),
			new PropertyInfo((Type)3, StringName.op_Implicit("secondsToDisplay"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
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
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.Create && ((NativeVariantPtrArgs)(ref args)).Count == 5)
		{
			NSpeechBubbleVfx nSpeechBubbleVfx = Create(VariantUtils.ConvertTo<string>(ref ((NativeVariantPtrArgs)(ref args))[0]), VariantUtils.ConvertTo<DialogueSide>(ref ((NativeVariantPtrArgs)(ref args))[1]), VariantUtils.ConvertTo<Vector2>(ref ((NativeVariantPtrArgs)(ref args))[2]), VariantUtils.ConvertTo<double>(ref ((NativeVariantPtrArgs)(ref args))[3]), VariantUtils.ConvertTo<VfxColor>(ref ((NativeVariantPtrArgs)(ref args))[4]));
			ret = VariantUtils.CreateFrom<NSpeechBubbleVfx>(ref nSpeechBubbleVfx);
			return true;
		}
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetSpeechBubbleColor && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			SetSpeechBubbleColor();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._ExitTree && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._ExitTree();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.CreateInternal && ((NativeVariantPtrArgs)(ref args)).Count == 3)
		{
			NSpeechBubbleVfx nSpeechBubbleVfx2 = CreateInternal(VariantUtils.ConvertTo<string>(ref ((NativeVariantPtrArgs)(ref args))[0]), VariantUtils.ConvertTo<DialogueSide>(ref ((NativeVariantPtrArgs)(ref args))[1]), VariantUtils.ConvertTo<double>(ref ((NativeVariantPtrArgs)(ref args))[2]));
			ret = VariantUtils.CreateFrom<NSpeechBubbleVfx>(ref nSpeechBubbleVfx2);
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
	internal static bool InvokeGodotClassStaticMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.Create && ((NativeVariantPtrArgs)(ref args)).Count == 5)
		{
			NSpeechBubbleVfx nSpeechBubbleVfx = Create(VariantUtils.ConvertTo<string>(ref ((NativeVariantPtrArgs)(ref args))[0]), VariantUtils.ConvertTo<DialogueSide>(ref ((NativeVariantPtrArgs)(ref args))[1]), VariantUtils.ConvertTo<Vector2>(ref ((NativeVariantPtrArgs)(ref args))[2]), VariantUtils.ConvertTo<double>(ref ((NativeVariantPtrArgs)(ref args))[3]), VariantUtils.ConvertTo<VfxColor>(ref ((NativeVariantPtrArgs)(ref args))[4]));
			ret = VariantUtils.CreateFrom<NSpeechBubbleVfx>(ref nSpeechBubbleVfx);
			return true;
		}
		if ((ref method) == MethodName.CreateInternal && ((NativeVariantPtrArgs)(ref args)).Count == 3)
		{
			NSpeechBubbleVfx nSpeechBubbleVfx2 = CreateInternal(VariantUtils.ConvertTo<string>(ref ((NativeVariantPtrArgs)(ref args))[0]), VariantUtils.ConvertTo<DialogueSide>(ref ((NativeVariantPtrArgs)(ref args))[1]), VariantUtils.ConvertTo<double>(ref ((NativeVariantPtrArgs)(ref args))[2]));
			ret = VariantUtils.CreateFrom<NSpeechBubbleVfx>(ref nSpeechBubbleVfx2);
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
		if ((ref method) == MethodName.SetSpeechBubbleColor)
		{
			return true;
		}
		if ((ref method) == MethodName._ExitTree)
		{
			return true;
		}
		if ((ref method) == MethodName.CreateInternal)
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
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName.SecondsToDisplay)
		{
			SecondsToDisplay = VariantUtils.ConvertTo<double>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._container)
		{
			_container = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._label)
		{
			_label = VariantUtils.ConvertTo<MegaRichTextLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._contents)
		{
			_contents = VariantUtils.ConvertTo<Node2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._bubble)
		{
			_bubble = VariantUtils.ConvertTo<Sprite2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._shadow)
		{
			_shadow = VariantUtils.ConvertTo<Sprite2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._hsv)
		{
			_hsv = VariantUtils.ConvertTo<ShaderMaterial>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._tween)
		{
			_tween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._startPos)
		{
			_startPos = VariantUtils.ConvertTo<Vector2>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._vfxColor)
		{
			_vfxColor = VariantUtils.ConvertTo<VfxColor>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._style)
		{
			_style = VariantUtils.ConvertTo<DialogueStyle>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._side)
		{
			_side = VariantUtils.ConvertTo<DialogueSide>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._text)
		{
			_text = VariantUtils.ConvertTo<string>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._elapsedTime)
		{
			_elapsedTime = VariantUtils.ConvertTo<float>(ref value);
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
		if ((ref name) == PropertyName.SecondsToDisplay)
		{
			double secondsToDisplay = SecondsToDisplay;
			value = VariantUtils.CreateFrom<double>(ref secondsToDisplay);
			return true;
		}
		if ((ref name) == PropertyName._container)
		{
			value = VariantUtils.CreateFrom<Control>(ref _container);
			return true;
		}
		if ((ref name) == PropertyName._label)
		{
			value = VariantUtils.CreateFrom<MegaRichTextLabel>(ref _label);
			return true;
		}
		if ((ref name) == PropertyName._contents)
		{
			value = VariantUtils.CreateFrom<Node2D>(ref _contents);
			return true;
		}
		if ((ref name) == PropertyName._bubble)
		{
			value = VariantUtils.CreateFrom<Sprite2D>(ref _bubble);
			return true;
		}
		if ((ref name) == PropertyName._shadow)
		{
			value = VariantUtils.CreateFrom<Sprite2D>(ref _shadow);
			return true;
		}
		if ((ref name) == PropertyName._hsv)
		{
			value = VariantUtils.CreateFrom<ShaderMaterial>(ref _hsv);
			return true;
		}
		if ((ref name) == PropertyName._tween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _tween);
			return true;
		}
		if ((ref name) == PropertyName._startPos)
		{
			value = VariantUtils.CreateFrom<Vector2>(ref _startPos);
			return true;
		}
		if ((ref name) == PropertyName._vfxColor)
		{
			value = VariantUtils.CreateFrom<VfxColor>(ref _vfxColor);
			return true;
		}
		if ((ref name) == PropertyName._style)
		{
			value = VariantUtils.CreateFrom<DialogueStyle>(ref _style);
			return true;
		}
		if ((ref name) == PropertyName._side)
		{
			value = VariantUtils.CreateFrom<DialogueSide>(ref _side);
			return true;
		}
		if ((ref name) == PropertyName._text)
		{
			value = VariantUtils.CreateFrom<string>(ref _text);
			return true;
		}
		if ((ref name) == PropertyName._elapsedTime)
		{
			value = VariantUtils.CreateFrom<float>(ref _elapsedTime);
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
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._container, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._label, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._contents, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._bubble, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._shadow, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._hsv, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._tween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)5, PropertyName._startPos, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)2, PropertyName._vfxColor, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)2, PropertyName._style, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)2, PropertyName._side, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)4, PropertyName._text, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)3, PropertyName.SecondsToDisplay, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)3, PropertyName._elapsedTime, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		((GodotObject)this).SaveGodotObjectData(info);
		StringName secondsToDisplay = PropertyName.SecondsToDisplay;
		double secondsToDisplay2 = SecondsToDisplay;
		info.AddProperty(secondsToDisplay, Variant.From<double>(ref secondsToDisplay2));
		info.AddProperty(PropertyName._container, Variant.From<Control>(ref _container));
		info.AddProperty(PropertyName._label, Variant.From<MegaRichTextLabel>(ref _label));
		info.AddProperty(PropertyName._contents, Variant.From<Node2D>(ref _contents));
		info.AddProperty(PropertyName._bubble, Variant.From<Sprite2D>(ref _bubble));
		info.AddProperty(PropertyName._shadow, Variant.From<Sprite2D>(ref _shadow));
		info.AddProperty(PropertyName._hsv, Variant.From<ShaderMaterial>(ref _hsv));
		info.AddProperty(PropertyName._tween, Variant.From<Tween>(ref _tween));
		info.AddProperty(PropertyName._startPos, Variant.From<Vector2>(ref _startPos));
		info.AddProperty(PropertyName._vfxColor, Variant.From<VfxColor>(ref _vfxColor));
		info.AddProperty(PropertyName._style, Variant.From<DialogueStyle>(ref _style));
		info.AddProperty(PropertyName._side, Variant.From<DialogueSide>(ref _side));
		info.AddProperty(PropertyName._text, Variant.From<string>(ref _text));
		info.AddProperty(PropertyName._elapsedTime, Variant.From<float>(ref _elapsedTime));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName.SecondsToDisplay, ref val))
		{
			SecondsToDisplay = ((Variant)(ref val)).As<double>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._container, ref val2))
		{
			_container = ((Variant)(ref val2)).As<Control>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._label, ref val3))
		{
			_label = ((Variant)(ref val3)).As<MegaRichTextLabel>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._contents, ref val4))
		{
			_contents = ((Variant)(ref val4)).As<Node2D>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._bubble, ref val5))
		{
			_bubble = ((Variant)(ref val5)).As<Sprite2D>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._shadow, ref val6))
		{
			_shadow = ((Variant)(ref val6)).As<Sprite2D>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName._hsv, ref val7))
		{
			_hsv = ((Variant)(ref val7)).As<ShaderMaterial>();
		}
		Variant val8 = default(Variant);
		if (info.TryGetProperty(PropertyName._tween, ref val8))
		{
			_tween = ((Variant)(ref val8)).As<Tween>();
		}
		Variant val9 = default(Variant);
		if (info.TryGetProperty(PropertyName._startPos, ref val9))
		{
			_startPos = ((Variant)(ref val9)).As<Vector2>();
		}
		Variant val10 = default(Variant);
		if (info.TryGetProperty(PropertyName._vfxColor, ref val10))
		{
			_vfxColor = ((Variant)(ref val10)).As<VfxColor>();
		}
		Variant val11 = default(Variant);
		if (info.TryGetProperty(PropertyName._style, ref val11))
		{
			_style = ((Variant)(ref val11)).As<DialogueStyle>();
		}
		Variant val12 = default(Variant);
		if (info.TryGetProperty(PropertyName._side, ref val12))
		{
			_side = ((Variant)(ref val12)).As<DialogueSide>();
		}
		Variant val13 = default(Variant);
		if (info.TryGetProperty(PropertyName._text, ref val13))
		{
			_text = ((Variant)(ref val13)).As<string>();
		}
		Variant val14 = default(Variant);
		if (info.TryGetProperty(PropertyName._elapsedTime, ref val14))
		{
			_elapsedTime = ((Variant)(ref val14)).As<float>();
		}
	}
}
