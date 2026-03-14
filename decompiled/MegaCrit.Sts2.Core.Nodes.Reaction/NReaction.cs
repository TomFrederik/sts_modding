using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Random;

namespace MegaCrit.Sts2.Core.Nodes.Reaction;

[ScriptPath("res://src/Core/Nodes/Reaction/NReaction.cs")]
public class NReaction : TextureRect
{
	public class MethodName : MethodName
	{
		public static readonly StringName Create = StringName.op_Implicit("Create");

		public static readonly StringName BeginAnim = StringName.op_Implicit("BeginAnim");

		public static readonly StringName TypeToTexture = StringName.op_Implicit("TypeToTexture");

		public static readonly StringName TextureToType = StringName.op_Implicit("TextureToType");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName Type = StringName.op_Implicit("Type");
	}

	public class SignalName : SignalName
	{
	}

	private static readonly string _scenePath = SceneHelper.GetScenePath("ui/reaction");

	private const string _exclamationPath = "res://images/ui/emote/exclaim.png";

	private const string _skullPath = "res://images/ui/emote/skull.png";

	private const string _thumbDownPath = "res://images/ui/emote/thumb_down.png";

	private const string _sadSlimePath = "res://images/ui/emote/slime_sad.png";

	private const string _questionMarkPath = "res://images/ui/emote/question.png";

	private const string _heartPath = "res://images/ui/emote/heart.png";

	private const string _thumbUpPath = "res://images/ui/emote/thumb_up.png";

	private const string _happyCultistPath = "res://images/ui/emote/happy_cultist.png";

	public ReactionType Type => TextureToType(((TextureRect)this).Texture);

	public static NReaction Create(Texture2D reactionTexture)
	{
		NReaction nReaction = PreloadManager.Cache.GetScene(_scenePath).Instantiate<NReaction>((GenEditState)0);
		((TextureRect)nReaction).Texture = reactionTexture;
		return nReaction;
	}

	public static NReaction Create(ReactionType type)
	{
		return Create(TypeToTexture(type));
	}

	public void BeginAnim()
	{
		TaskHelper.RunSafely(DoAnim());
	}

	private async Task DoAnim()
	{
		NReaction nReaction = this;
		Color modulate = ((CanvasItem)this).Modulate;
		modulate.A = 0f;
		((CanvasItem)nReaction).Modulate = modulate;
		float num = Rng.Chaotic.NextFloat(40f, 60f);
		float num2 = Rng.Chaotic.NextFloat(-30f, 30f);
		Vector2 position = ((Control)this).Position;
		Vector2 up = Vector2.Up;
		Vector2 val = position + ((Vector2)(ref up)).Rotated(Mathf.DegToRad(num2)) * num;
		Tween val2 = ((Node)this).CreateTween();
		val2.SetParallel(true);
		val2.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("position"), Variant.op_Implicit(val), 0.30000001192092896).SetEase((EaseType)1).SetTrans((TransitionType)5);
		val2.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(1f), 0.30000001192092896).SetEase((EaseType)1).SetTrans((TransitionType)5);
		val2.SetParallel(false);
		val2.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0f), 0.20000000298023224).SetDelay(0.6000000238418579).SetEase((EaseType)0)
			.SetTrans((TransitionType)5);
		await ((GodotObject)this).ToSignal((GodotObject)(object)val2, SignalName.Finished);
		((Node)(object)this).QueueFreeSafely();
	}

	private static Texture2D TypeToTexture(ReactionType type)
	{
		AssetCache cache = PreloadManager.Cache;
		return cache.GetTexture2D(type switch
		{
			ReactionType.Exclamation => "res://images/ui/emote/exclaim.png", 
			ReactionType.Skull => "res://images/ui/emote/skull.png", 
			ReactionType.ThumbDown => "res://images/ui/emote/thumb_down.png", 
			ReactionType.SadSlime => "res://images/ui/emote/slime_sad.png", 
			ReactionType.QuestionMark => "res://images/ui/emote/question.png", 
			ReactionType.Heart => "res://images/ui/emote/heart.png", 
			ReactionType.ThumbUp => "res://images/ui/emote/thumb_up.png", 
			ReactionType.HappyCultist => "res://images/ui/emote/happy_cultist.png", 
			_ => throw new ArgumentOutOfRangeException("type", type, null), 
		});
	}

	private static ReactionType TextureToType(Texture2D texture)
	{
		return ((Resource)texture).ResourcePath switch
		{
			"res://images/ui/emote/exclaim.png" => ReactionType.Exclamation, 
			"res://images/ui/emote/skull.png" => ReactionType.Skull, 
			"res://images/ui/emote/thumb_down.png" => ReactionType.ThumbDown, 
			"res://images/ui/emote/slime_sad.png" => ReactionType.SadSlime, 
			"res://images/ui/emote/question.png" => ReactionType.QuestionMark, 
			"res://images/ui/emote/heart.png" => ReactionType.Heart, 
			"res://images/ui/emote/thumb_up.png" => ReactionType.ThumbUp, 
			"res://images/ui/emote/happy_cultist.png" => ReactionType.HappyCultist, 
			_ => throw new ArgumentOutOfRangeException("texture", texture, null), 
		};
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
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Expected O, but got Unknown
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Expected O, but got Unknown
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(4);
		list.Add(new MethodInfo(MethodName.Create, new PropertyInfo((Type)24, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("TextureRect"), false), (MethodFlags)33, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("reactionTexture"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Texture2D"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.BeginAnim, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.TypeToTexture, new PropertyInfo((Type)24, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Texture2D"), false), (MethodFlags)33, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("type"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.TextureToType, new PropertyInfo((Type)2, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)33, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("texture"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Texture2D"), false)
		}, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.Create && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			NReaction nReaction = Create(VariantUtils.ConvertTo<Texture2D>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<NReaction>(ref nReaction);
			return true;
		}
		if ((ref method) == MethodName.BeginAnim && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			BeginAnim();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.TypeToTexture && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			Texture2D val = TypeToTexture(VariantUtils.ConvertTo<ReactionType>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<Texture2D>(ref val);
			return true;
		}
		if ((ref method) == MethodName.TextureToType && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			ReactionType reactionType = TextureToType(VariantUtils.ConvertTo<Texture2D>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<ReactionType>(ref reactionType);
			return true;
		}
		return ((TextureRect)this).InvokeGodotClassMethod(ref method, args, ref ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static bool InvokeGodotClassStaticMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.Create && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			NReaction nReaction = Create(VariantUtils.ConvertTo<Texture2D>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<NReaction>(ref nReaction);
			return true;
		}
		if ((ref method) == MethodName.TypeToTexture && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			Texture2D val = TypeToTexture(VariantUtils.ConvertTo<ReactionType>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<Texture2D>(ref val);
			return true;
		}
		if ((ref method) == MethodName.TextureToType && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			ReactionType reactionType = TextureToType(VariantUtils.ConvertTo<Texture2D>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<ReactionType>(ref reactionType);
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
		if ((ref method) == MethodName.BeginAnim)
		{
			return true;
		}
		if ((ref method) == MethodName.TypeToTexture)
		{
			return true;
		}
		if ((ref method) == MethodName.TextureToType)
		{
			return true;
		}
		return ((TextureRect)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName.Type)
		{
			ReactionType type = Type;
			value = VariantUtils.CreateFrom<ReactionType>(ref type);
			return true;
		}
		return ((GodotObject)this).GetGodotClassPropertyValue(ref name, ref value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)2, PropertyName.Type, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).SaveGodotObjectData(info);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
	}
}
