using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx.Utilities;
using MegaCrit.Sts2.Core.TestSupport;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.Nodes.Vfx;

[ScriptPath("res://src/Core/Nodes/Vfx/NThoughtBubbleVfx.cs")]
public class NThoughtBubbleVfx : Control
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName _ExitTree = StringName.op_Implicit("_ExitTree");

		public static readonly StringName SetTexture = StringName.op_Implicit("SetTexture");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName _container = StringName.op_Implicit("_container");

		public static readonly StringName _label = StringName.op_Implicit("_label");

		public static readonly StringName _textureRect = StringName.op_Implicit("_textureRect");

		public static readonly StringName _contents = StringName.op_Implicit("_contents");

		public static readonly StringName _tail = StringName.op_Implicit("_tail");

		public static readonly StringName _style = StringName.op_Implicit("_style");

		public static readonly StringName _side = StringName.op_Implicit("_side");

		public static readonly StringName _text = StringName.op_Implicit("_text");

		public static readonly StringName _texture = StringName.op_Implicit("_texture");

		public static readonly StringName _tween = StringName.op_Implicit("_tween");
	}

	public class SignalName : SignalName
	{
	}

	private Control _container;

	private MegaRichTextLabel _label;

	private TextureRect _textureRect;

	private Node2D _contents;

	private Node2D _tail;

	private const string _path = "res://scenes/vfx/vfx_thought_bubble.tscn";

	private const float _spawnProportionToTopOfHitbox = 0.75f;

	private const float _spawnProportionToEdgeOfHitbox = 0.75f;

	private Vector2? _startPos;

	private DialogueStyle _style;

	private DialogueSide _side;

	private string? _text;

	private Texture2D? _texture;

	private double? _secondsToDisplay;

	private Tween? _tween;

	private CancellationTokenSource? _cts;

	public static IEnumerable<string> AssetPaths => new _003C_003Ez__ReadOnlySingleElementList<string>("res://scenes/vfx/vfx_thought_bubble.tscn");

	public static NThoughtBubbleVfx? Create(string text, Creature speaker, double? secondsToDisplay)
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		if (TestMode.IsOn)
		{
			return null;
		}
		NThoughtBubbleVfx nThoughtBubbleVfx = CreateInternal(text, null, speaker.Side switch
		{
			CombatSide.Player => DialogueSide.Left, 
			CombatSide.Enemy => DialogueSide.Right, 
			_ => throw new ArgumentOutOfRangeException(), 
		}, secondsToDisplay);
		nThoughtBubbleVfx._startPos = GetCreatureSpeechPosition(speaker);
		return nThoughtBubbleVfx;
	}

	public static NThoughtBubbleVfx? Create(string text, DialogueSide side, double? secondsToDisplay)
	{
		if (TestMode.IsOn)
		{
			return null;
		}
		return CreateInternal(text, null, side, secondsToDisplay);
	}

	public static NThoughtBubbleVfx? Create(Texture2D texture, DialogueSide side, double? secondsToDisplay)
	{
		if (TestMode.IsOn)
		{
			return null;
		}
		return CreateInternal(null, texture, side, secondsToDisplay);
	}

	public override void _Ready()
	{
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		_contents = ((Node)this).GetNode<Node2D>(NodePath.op_Implicit("%Contents"));
		_container = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%Container"));
		_label = ((Node)this).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("%Text"));
		_textureRect = ((Node)this).GetNode<TextureRect>(NodePath.op_Implicit("%Image"));
		_tail = ((Node)this).GetNode<Node2D>(NodePath.op_Implicit("%Tail"));
		if (_startPos.HasValue)
		{
			((Control)this).GlobalPosition = _startPos.Value;
		}
		if (_side == DialogueSide.Right)
		{
			_container.Position = new Vector2(0f - _container.Size.X - _container.Position.X, _container.Position.Y);
			_tail.Scale = new Vector2(-1f, 1f);
		}
		TaskHelper.RunSafely(AnimateThoughtBubble());
	}

	private async Task AnimateThoughtBubble()
	{
		_tween = ((Node)this).CreateTween().SetParallel(true);
		float value = 30f * ((_side == DialogueSide.Left) ? (-1f) : 1f);
		if (_text != null)
		{
			_label.Text = $"[center][fly_in offset_x={value} offset_y=40]{_text}[/fly_in][/center]";
		}
		((CanvasItem)_label).Visible = _text != null;
		if (_texture != null)
		{
			_textureRect.Texture = _texture;
		}
		((CanvasItem)_textureRect).Visible = _texture != null;
		((Control)this).Scale = Vector2.One * 0.75f;
		((CanvasItem)this).Modulate = StsColors.transparentWhite;
		_tween.TweenProperty((GodotObject)(object)_label, NodePath.op_Implicit("visible_ratio"), Variant.op_Implicit(1f), 0.4).From(Variant.op_Implicit(0f));
		_tween.TweenProperty((GodotObject)(object)_textureRect, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(1f), 0.4).From(Variant.op_Implicit(0f));
		_tween.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(1f), 0.25).SetEase((EaseType)1).SetTrans((TransitionType)7);
		_tween.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("scale"), Variant.op_Implicit(Vector2.One), 0.25).SetEase((EaseType)1).SetTrans((TransitionType)10);
		if (_secondsToDisplay.HasValue)
		{
			double num = Math.Max(_secondsToDisplay.Value, 1.0);
			_cts = new CancellationTokenSource();
			await Cmd.Wait((float)num, _cts.Token);
			await GoAway();
		}
	}

	public override void _ExitTree()
	{
		Tween? tween = _tween;
		if (tween != null)
		{
			tween.Kill();
		}
		_cts?.Cancel();
		_cts?.Dispose();
	}

	public async Task GoAway()
	{
		if (GodotObject.IsInstanceValid((GodotObject)(object)this))
		{
			Tween? tween = _tween;
			if (tween != null)
			{
				tween.Kill();
			}
			_tween = ((Node)this).CreateTween();
			_tween.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0f), 0.4).SetEase((EaseType)1).SetTrans((TransitionType)1);
			await ((GodotObject)this).ToSignal((GodotObject)(object)_tween, SignalName.Finished);
			((Node)(object)this).QueueFreeSafely();
		}
	}

	private static NThoughtBubbleVfx CreateInternal(string? text, Texture2D? texture, DialogueSide side, double? secondsToDisplay)
	{
		NThoughtBubbleVfx nThoughtBubbleVfx = PreloadManager.Cache.GetScene("res://scenes/vfx/vfx_thought_bubble.tscn").Instantiate<NThoughtBubbleVfx>((GenEditState)0);
		nThoughtBubbleVfx._side = side;
		nThoughtBubbleVfx._text = text;
		nThoughtBubbleVfx._texture = texture;
		nThoughtBubbleVfx._secondsToDisplay = secondsToDisplay;
		return nThoughtBubbleVfx;
	}

	public static Vector2 GetCreatureSpeechPosition(Creature speaker)
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

	public void SetTexture(Texture2D texture)
	{
		if (_texture == null)
		{
			throw new NotImplementedException("Can't set texture unless thought bubble was initialized with a texture");
		}
		_texture = texture;
		_textureRect.Texture = texture;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Expected O, but got Unknown
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(3);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._ExitTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetTexture, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("texture"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Texture2D"), false)
		}, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
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
		if ((ref method) == MethodName.SetTexture && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			SetTexture(VariantUtils.ConvertTo<Texture2D>(ref ((NativeVariantPtrArgs)(ref args))[0]));
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
		if ((ref method) == MethodName.SetTexture)
		{
			return true;
		}
		return ((Control)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
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
		if ((ref name) == PropertyName._textureRect)
		{
			_textureRect = VariantUtils.ConvertTo<TextureRect>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._contents)
		{
			_contents = VariantUtils.ConvertTo<Node2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._tail)
		{
			_tail = VariantUtils.ConvertTo<Node2D>(ref value);
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
		if ((ref name) == PropertyName._texture)
		{
			_texture = VariantUtils.ConvertTo<Texture2D>(ref value);
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
		if ((ref name) == PropertyName._textureRect)
		{
			value = VariantUtils.CreateFrom<TextureRect>(ref _textureRect);
			return true;
		}
		if ((ref name) == PropertyName._contents)
		{
			value = VariantUtils.CreateFrom<Node2D>(ref _contents);
			return true;
		}
		if ((ref name) == PropertyName._tail)
		{
			value = VariantUtils.CreateFrom<Node2D>(ref _tail);
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
		if ((ref name) == PropertyName._texture)
		{
			value = VariantUtils.CreateFrom<Texture2D>(ref _texture);
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
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._container, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._label, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._textureRect, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._contents, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._tail, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)2, PropertyName._style, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)2, PropertyName._side, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)4, PropertyName._text, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._texture, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._tween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		((GodotObject)this).SaveGodotObjectData(info);
		info.AddProperty(PropertyName._container, Variant.From<Control>(ref _container));
		info.AddProperty(PropertyName._label, Variant.From<MegaRichTextLabel>(ref _label));
		info.AddProperty(PropertyName._textureRect, Variant.From<TextureRect>(ref _textureRect));
		info.AddProperty(PropertyName._contents, Variant.From<Node2D>(ref _contents));
		info.AddProperty(PropertyName._tail, Variant.From<Node2D>(ref _tail));
		info.AddProperty(PropertyName._style, Variant.From<DialogueStyle>(ref _style));
		info.AddProperty(PropertyName._side, Variant.From<DialogueSide>(ref _side));
		info.AddProperty(PropertyName._text, Variant.From<string>(ref _text));
		info.AddProperty(PropertyName._texture, Variant.From<Texture2D>(ref _texture));
		info.AddProperty(PropertyName._tween, Variant.From<Tween>(ref _tween));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._container, ref val))
		{
			_container = ((Variant)(ref val)).As<Control>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._label, ref val2))
		{
			_label = ((Variant)(ref val2)).As<MegaRichTextLabel>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._textureRect, ref val3))
		{
			_textureRect = ((Variant)(ref val3)).As<TextureRect>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._contents, ref val4))
		{
			_contents = ((Variant)(ref val4)).As<Node2D>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._tail, ref val5))
		{
			_tail = ((Variant)(ref val5)).As<Node2D>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._style, ref val6))
		{
			_style = ((Variant)(ref val6)).As<DialogueStyle>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName._side, ref val7))
		{
			_side = ((Variant)(ref val7)).As<DialogueSide>();
		}
		Variant val8 = default(Variant);
		if (info.TryGetProperty(PropertyName._text, ref val8))
		{
			_text = ((Variant)(ref val8)).As<string>();
		}
		Variant val9 = default(Variant);
		if (info.TryGetProperty(PropertyName._texture, ref val9))
		{
			_texture = ((Variant)(ref val9)).As<Texture2D>();
		}
		Variant val10 = default(Variant);
		if (info.TryGetProperty(PropertyName._tween, ref val10))
		{
			_tween = ((Variant)(ref val10)).As<Tween>();
		}
	}
}
