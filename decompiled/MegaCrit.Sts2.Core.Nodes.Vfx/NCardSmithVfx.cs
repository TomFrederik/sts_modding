using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Audio.Debug;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.Vfx.Utilities;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.TestSupport;

namespace MegaCrit.Sts2.Core.Nodes.Vfx;

[ScriptPath("res://src/Core/Nodes/Vfx/NCardSmithVfx.cs")]
public class NCardSmithVfx : Node2D
{
	public class MethodName : MethodName
	{
		public static readonly StringName Create = StringName.op_Implicit("Create");

		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName _ExitTree = StringName.op_Implicit("_ExitTree");

		public static readonly StringName PlaySubParticles = StringName.op_Implicit("PlaySubParticles");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName SfxVolume = StringName.op_Implicit("SfxVolume");

		public static readonly StringName _tween = StringName.op_Implicit("_tween");

		public static readonly StringName _willPlaySfx = StringName.op_Implicit("_willPlaySfx");

		public static readonly StringName _cardNode = StringName.op_Implicit("_cardNode");

		public static readonly StringName _cardContainer = StringName.op_Implicit("_cardContainer");
	}

	public class SignalName : SignalName
	{
	}

	private Tween? _tween;

	public const string smithSfx = "card_smith.mp3";

	private bool _willPlaySfx = true;

	private readonly List<CardModel> _cards = new List<CardModel>();

	private NCard? _cardNode;

	private Control _cardContainer;

	private static string ScenePath => SceneHelper.GetScenePath("vfx/vfx_card_smith");

	public static IEnumerable<string> AssetPaths => new global::_003C_003Ez__ReadOnlyArray<string>(new string[2]
	{
		ScenePath,
		TmpSfx.GetPath("card_smith.mp3")
	});

	public float SfxVolume { get; set; } = 1f;

	public static NCardSmithVfx? Create(IEnumerable<CardModel> cards, bool playSfx = true)
	{
		NCardSmithVfx nCardSmithVfx = Create();
		if (nCardSmithVfx == null)
		{
			return null;
		}
		nCardSmithVfx._cards.AddRange(cards);
		nCardSmithVfx._willPlaySfx = playSfx;
		return nCardSmithVfx;
	}

	public static NCardSmithVfx? Create(NCard card, bool playSfx = true)
	{
		NCardSmithVfx nCardSmithVfx = Create();
		if (nCardSmithVfx == null)
		{
			return null;
		}
		nCardSmithVfx._willPlaySfx = playSfx;
		nCardSmithVfx._cardNode = card;
		return nCardSmithVfx;
	}

	public static NCardSmithVfx? Create()
	{
		if (TestMode.IsOn)
		{
			return null;
		}
		return PreloadManager.Cache.GetScene(ScenePath).Instantiate<NCardSmithVfx>((GenEditState)0);
	}

	public override void _Ready()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		if (_cardNode != null)
		{
			((Node2D)this).GlobalPosition = ((Control)_cardNode).GlobalPosition;
			((Node2D)this).GlobalScale = ((Control)_cardNode).Scale;
			TaskHelper.RunSafely(PlayAnimation());
		}
		else if (_cards.Count > 0)
		{
			TaskHelper.RunSafely(PlayAnimation(_cards));
		}
		else
		{
			TaskHelper.RunSafely(PlayAnimation());
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

	private async Task PlayAnimation()
	{
		if (_willPlaySfx)
		{
			NDebugAudioManager.Instance?.Play("card_smith.mp3", SfxVolume, PitchVariance.Small);
		}
		_tween = ((Node)this).CreateTween();
		_tween.Parallel().TweenCallback(Callable.From((Action)delegate
		{
			PlaySubParticles((Node)(object)((Node)this).GetNode<Control>(NodePath.op_Implicit("Spark1")));
		}));
		_tween.TweenInterval(0.25);
		_tween.Chain().TweenCallback(Callable.From((Action)delegate
		{
			PlaySubParticles((Node)(object)((Node)this).GetNode<Control>(NodePath.op_Implicit("Spark2")));
		}));
		_tween.TweenInterval(0.25);
		_tween.Chain().TweenCallback(Callable.From((Action)delegate
		{
			PlaySubParticles((Node)(object)((Node)this).GetNode<Control>(NodePath.op_Implicit("Spark3")));
		}));
		_tween.TweenInterval(0.4000000059604645);
		await ((GodotObject)this).ToSignal((GodotObject)(object)_tween, SignalName.Finished);
		((Node)(object)this).QueueFreeSafely();
	}

	private async Task PlayAnimation(IEnumerable<CardModel> cards)
	{
		Control node = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%CardContainer"));
		List<NCard> cardNodes = new List<NCard>();
		foreach (CardModel card in cards)
		{
			NCard nCard = NCard.Create(card);
			((Node)(object)node).AddChildSafely((Node?)(object)nCard);
			nCard.UpdateVisuals(PileType.None, CardPreviewMode.Normal);
			cardNodes.Add(nCard);
		}
		_tween = ((Node)this).CreateTween();
		foreach (NCard item in cardNodes)
		{
			_tween.Parallel().TweenProperty((GodotObject)(object)item, NodePath.op_Implicit("scale"), Variant.op_Implicit(Vector2.One * 1f), 0.25).From(Variant.op_Implicit(Vector2.Zero))
				.SetEase((EaseType)1)
				.SetTrans((TransitionType)7);
		}
		if (_willPlaySfx)
		{
			_tween.Chain().TweenCallback(Callable.From((Action)delegate
			{
				NDebugAudioManager.Instance?.Play("card_smith.mp3", SfxVolume, PitchVariance.Small);
			}));
		}
		_tween.Parallel().TweenCallback(Callable.From((Action)delegate
		{
			PlaySubParticles((Node)(object)((Node)this).GetNode<Control>(NodePath.op_Implicit("Spark1")));
		}));
		_tween.Parallel().TweenCallback(Callable.From((Action)delegate
		{
			NGame.Instance?.ScreenShake(ShakeStrength.Weak, ShakeDuration.Short, 180f + Rng.Chaotic.NextFloat(-10f, 10f));
		}));
		foreach (NCard item2 in cardNodes)
		{
			_tween.Parallel().TweenProperty((GodotObject)(object)item2, NodePath.op_Implicit("rotation_degrees"), Variant.op_Implicit(20), 0.05000000074505806).SetTrans((TransitionType)6)
				.SetEase((EaseType)1);
		}
		_tween.TweenInterval(0.25);
		_tween.Chain().TweenCallback(Callable.From((Action)delegate
		{
			PlaySubParticles((Node)(object)((Node)this).GetNode<Control>(NodePath.op_Implicit("Spark2")));
		}));
		foreach (NCard item3 in cardNodes)
		{
			_tween.Parallel().TweenProperty((GodotObject)(object)item3, NodePath.op_Implicit("rotation_degrees"), Variant.op_Implicit(-10), 0.05000000074505806).SetTrans((TransitionType)6)
				.SetEase((EaseType)1);
		}
		_tween.Parallel().TweenCallback(Callable.From((Action)delegate
		{
			NGame.Instance?.ScreenShake(ShakeStrength.Weak, ShakeDuration.Short, 180f + Rng.Chaotic.NextFloat(-10f, 10f));
		}));
		_tween.TweenInterval(0.25);
		_tween.Chain().TweenCallback(Callable.From((Action)delegate
		{
			PlaySubParticles((Node)(object)((Node)this).GetNode<Control>(NodePath.op_Implicit("Spark3")));
		}));
		foreach (NCard item4 in cardNodes)
		{
			_tween.Parallel().TweenProperty((GodotObject)(object)item4, NodePath.op_Implicit("rotation_degrees"), Variant.op_Implicit(5), 0.05000000074505806).SetTrans((TransitionType)6)
				.SetEase((EaseType)1);
		}
		_tween.Parallel().TweenCallback(Callable.From((Action)delegate
		{
			NGame.Instance?.ScreenShake(ShakeStrength.Weak, ShakeDuration.Short, 180f + Rng.Chaotic.NextFloat(-10f, 10f));
		}));
		_tween.TweenInterval(0.4000000059604645);
		await ((GodotObject)this).ToSignal((GodotObject)(object)_tween, SignalName.Finished);
		if (((Node)cardNodes[0]).IsInsideTree() && _cards[0].Pile == null)
		{
			_tween = ((Node)this).CreateTween();
			foreach (NCard item5 in cardNodes)
			{
				_tween.SetParallel(true).TweenProperty((GodotObject)(object)item5, NodePath.op_Implicit("scale"), Variant.op_Implicit(Vector2.Zero), 0.15000000596046448);
			}
			await ((GodotObject)this).ToSignal((GodotObject)(object)_tween, SignalName.Finished);
		}
		else
		{
			if (!((Node)cardNodes[0]).IsInsideTree())
			{
				return;
			}
			for (int i = 0; i < cardNodes.Count; i++)
			{
				Vector2 targetPosition = cardNodes[i].Model.Pile.Type.GetTargetPosition(cardNodes[i]);
				Vector2 globalPosition = ((Control)cardNodes[i]).GlobalPosition;
				((Node)cardNodes[i]).Reparent((Node)(object)this, true);
				((Control)cardNodes[i]).GlobalPosition = globalPosition;
				NCardFlyVfx nCardFlyVfx = NCardFlyVfx.Create(cardNodes[i], targetPosition, isAddingToPile: false, cardNodes[i].Model.Owner.Character.TrailPath);
				NRun.Instance?.GlobalUi.TopBar.TrailContainer.AddChildSafely((Node?)(object)nCardFlyVfx);
				if (nCardFlyVfx.SwooshAwayCompletion != null && i == cardNodes.Count - 1)
				{
					await nCardFlyVfx.SwooshAwayCompletion.Task;
				}
			}
			((Node)(object)this).QueueFreeSafely();
		}
	}

	private void PlaySubParticles(Node node)
	{
		foreach (CpuParticles2D item in ((IEnumerable)node.GetChildren(false)).OfType<CpuParticles2D>())
		{
			item.Emitting = true;
		}
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
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Expected O, but got Unknown
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Expected O, but got Unknown
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(5);
		list.Add(new MethodInfo(MethodName.Create, new PropertyInfo((Type)24, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Node2D"), false), (MethodFlags)33, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("card"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false),
			new PropertyInfo((Type)1, StringName.op_Implicit("playSfx"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.Create, new PropertyInfo((Type)24, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Node2D"), false), (MethodFlags)33, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._ExitTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.PlaySubParticles, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("node"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Node"), false)
		}, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.Create && ((NativeVariantPtrArgs)(ref args)).Count == 2)
		{
			NCardSmithVfx nCardSmithVfx = Create(VariantUtils.ConvertTo<NCard>(ref ((NativeVariantPtrArgs)(ref args))[0]), VariantUtils.ConvertTo<bool>(ref ((NativeVariantPtrArgs)(ref args))[1]));
			ret = VariantUtils.CreateFrom<NCardSmithVfx>(ref nCardSmithVfx);
			return true;
		}
		if ((ref method) == MethodName.Create && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			NCardSmithVfx nCardSmithVfx2 = Create();
			ret = VariantUtils.CreateFrom<NCardSmithVfx>(ref nCardSmithVfx2);
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
		if ((ref method) == MethodName.PlaySubParticles && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			PlaySubParticles(VariantUtils.ConvertTo<Node>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		return ((Node2D)this).InvokeGodotClassMethod(ref method, args, ref ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static bool InvokeGodotClassStaticMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.Create && ((NativeVariantPtrArgs)(ref args)).Count == 2)
		{
			NCardSmithVfx nCardSmithVfx = Create(VariantUtils.ConvertTo<NCard>(ref ((NativeVariantPtrArgs)(ref args))[0]), VariantUtils.ConvertTo<bool>(ref ((NativeVariantPtrArgs)(ref args))[1]));
			ret = VariantUtils.CreateFrom<NCardSmithVfx>(ref nCardSmithVfx);
			return true;
		}
		if ((ref method) == MethodName.Create && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			NCardSmithVfx nCardSmithVfx2 = Create();
			ret = VariantUtils.CreateFrom<NCardSmithVfx>(ref nCardSmithVfx2);
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
		if ((ref method) == MethodName.PlaySubParticles)
		{
			return true;
		}
		return ((Node2D)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName.SfxVolume)
		{
			SfxVolume = VariantUtils.ConvertTo<float>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._tween)
		{
			_tween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._willPlaySfx)
		{
			_willPlaySfx = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._cardNode)
		{
			_cardNode = VariantUtils.ConvertTo<NCard>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._cardContainer)
		{
			_cardContainer = VariantUtils.ConvertTo<Control>(ref value);
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
		if ((ref name) == PropertyName.SfxVolume)
		{
			float sfxVolume = SfxVolume;
			value = VariantUtils.CreateFrom<float>(ref sfxVolume);
			return true;
		}
		if ((ref name) == PropertyName._tween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _tween);
			return true;
		}
		if ((ref name) == PropertyName._willPlaySfx)
		{
			value = VariantUtils.CreateFrom<bool>(ref _willPlaySfx);
			return true;
		}
		if ((ref name) == PropertyName._cardNode)
		{
			value = VariantUtils.CreateFrom<NCard>(ref _cardNode);
			return true;
		}
		if ((ref name) == PropertyName._cardContainer)
		{
			value = VariantUtils.CreateFrom<Control>(ref _cardContainer);
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
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._tween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._willPlaySfx, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._cardNode, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._cardContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)3, PropertyName.SfxVolume, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		((GodotObject)this).SaveGodotObjectData(info);
		StringName sfxVolume = PropertyName.SfxVolume;
		float sfxVolume2 = SfxVolume;
		info.AddProperty(sfxVolume, Variant.From<float>(ref sfxVolume2));
		info.AddProperty(PropertyName._tween, Variant.From<Tween>(ref _tween));
		info.AddProperty(PropertyName._willPlaySfx, Variant.From<bool>(ref _willPlaySfx));
		info.AddProperty(PropertyName._cardNode, Variant.From<NCard>(ref _cardNode));
		info.AddProperty(PropertyName._cardContainer, Variant.From<Control>(ref _cardContainer));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName.SfxVolume, ref val))
		{
			SfxVolume = ((Variant)(ref val)).As<float>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._tween, ref val2))
		{
			_tween = ((Variant)(ref val2)).As<Tween>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._willPlaySfx, ref val3))
		{
			_willPlaySfx = ((Variant)(ref val3)).As<bool>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._cardNode, ref val4))
		{
			_cardNode = ((Variant)(ref val4)).As<NCard>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._cardContainer, ref val5))
		{
			_cardContainer = ((Variant)(ref val5)).As<Control>();
		}
	}
}
