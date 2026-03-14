using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.Cards.Holders;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.TestSupport;

namespace MegaCrit.Sts2.Core.Nodes.Vfx;

[ScriptPath("res://src/Core/Nodes/Vfx/NCardTransformVfx.cs")]
public class NCardTransformVfx : Node2D
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName _ExitTree = StringName.op_Implicit("_ExitTree");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName _tween = StringName.op_Implicit("_tween");
	}

	public class SignalName : SignalName
	{
	}

	private Tween? _tween;

	private CardModel _startCard;

	private CardModel _endCard;

	private IEnumerable<RelicModel>? _relicsToFlash;

	private static string ScenePath => SceneHelper.GetScenePath("vfx/vfx_card_transform");

	public static IEnumerable<string> AssetPaths => new _003C_003Ez__ReadOnlySingleElementList<string>(ScenePath);

	public static NCardTransformVfx? Create(CardModel startCard, CardModel endCard, IEnumerable<RelicModel>? relicsToFlash)
	{
		if (TestMode.IsOn)
		{
			return null;
		}
		NCardTransformVfx nCardTransformVfx = PreloadManager.Cache.GetScene(ScenePath).Instantiate<NCardTransformVfx>((GenEditState)0);
		nCardTransformVfx._startCard = startCard;
		nCardTransformVfx._endCard = endCard;
		nCardTransformVfx._relicsToFlash = relicsToFlash;
		return nCardTransformVfx;
	}

	public override void _Ready()
	{
		TaskHelper.RunSafely(PlayAnimation());
	}

	private async Task<bool> WaitAndInterruptIfNecessary(float seconds, NCard cardNode)
	{
		for (float timer = 0f; timer <= seconds; timer += (float)((Node)this).GetProcessDeltaTime())
		{
			if (!((Node)cardNode).IsInsideTree() || _endCard.Pile == null)
			{
				return false;
			}
			await ((GodotObject)this).ToSignal((GodotObject)(object)((Node)this).GetTree(), SignalName.ProcessFrame);
		}
		return true;
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
		SfxCmd.Play("event:/sfx/ui/cards/card_transform");
		Material textureMat = ((CanvasItem)((Node)this).GetNode<Sprite2D>(NodePath.op_Implicit("%RenderTexture"))).Material;
		NCard cardNode = NCard.Create(_startCard);
		SubViewport node = ((Node)this).GetNode<SubViewport>(NodePath.op_Implicit("SubViewport"));
		((Node)(object)node).AddChildSafely((Node?)(object)cardNode);
		cardNode.UpdateVisuals(PileType.None, CardPreviewMode.Normal);
		((Control)cardNode).Position = new Vector2((float)node.Size.X * 0.5f, (float)node.Size.Y * 0.5f);
		_tween = ((Node)this).CreateTween();
		_tween.TweenProperty((GodotObject)(object)cardNode, NodePath.op_Implicit("scale"), Variant.op_Implicit(Vector2.One * 1f), 0.25).From(Variant.op_Implicit(Vector2.Zero)).SetEase((EaseType)1)
			.SetTrans((TransitionType)7);
		if (!(await WaitAndInterruptIfNecessary(0.75f, cardNode)))
		{
			((Node)(object)this).QueueFreeSafely();
			return;
		}
		_tween = ((Node)this).CreateTween().SetParallel(true);
		_tween.TweenProperty((GodotObject)(object)textureMat, NodePath.op_Implicit("shader_parameter/brightness"), Variant.op_Implicit(1f), 0.5);
		_tween.TweenProperty((GodotObject)(object)textureMat, NodePath.op_Implicit("shader_parameter/boing:x"), Variant.op_Implicit(2f), 0.4000000059604645);
		if (!(await WaitAndInterruptIfNecessary(0.5f, cardNode)))
		{
			((Node)(object)this).QueueFreeSafely();
			return;
		}
		cardNode.Model = _endCard;
		cardNode.UpdateVisuals(PileType.None, CardPreviewMode.Normal);
		((Node)this).GetNode<CpuParticles2D>(NodePath.op_Implicit("%Particle")).Emitting = true;
		_tween = ((Node)this).CreateTween().SetParallel(true);
		_tween.TweenProperty((GodotObject)(object)textureMat, NodePath.op_Implicit("shader_parameter/brightness"), Variant.op_Implicit(0f), 0.20000000298023224);
		_tween.TweenProperty((GodotObject)(object)textureMat, NodePath.op_Implicit("shader_parameter/boing:x"), Variant.op_Implicit(-0.75f), 0.15000000596046448).SetEase((EaseType)1).SetTrans((TransitionType)4);
		await ((GodotObject)this).ToSignal((GodotObject)(object)_tween, SignalName.Finished);
		_tween = ((Node)this).CreateTween().SetParallel(true);
		_tween.TweenProperty((GodotObject)(object)textureMat, NodePath.op_Implicit("shader_parameter/boing:x"), Variant.op_Implicit(0.3f), 0.20000000298023224).SetEase((EaseType)2).SetTrans((TransitionType)4);
		await ((GodotObject)this).ToSignal((GodotObject)(object)_tween, SignalName.Finished);
		_tween = ((Node)this).CreateTween().SetParallel(true);
		_tween.TweenProperty((GodotObject)(object)textureMat, NodePath.op_Implicit("shader_parameter/boing:x"), Variant.op_Implicit(-0.2f), 0.25).SetEase((EaseType)2).SetTrans((TransitionType)4);
		await ((GodotObject)this).ToSignal((GodotObject)(object)_tween, SignalName.Finished);
		_tween = ((Node)this).CreateTween().SetParallel(true);
		_tween.TweenProperty((GodotObject)(object)textureMat, NodePath.op_Implicit("shader_parameter/boing:x"), Variant.op_Implicit(0), 0.30000001192092896).SetEase((EaseType)2).SetTrans((TransitionType)10);
		if (!(await WaitAndInterruptIfNecessary(0.3f, cardNode)))
		{
			((Node)(object)this).QueueFreeSafely();
			return;
		}
		if (_relicsToFlash != null)
		{
			foreach (RelicModel item in _relicsToFlash)
			{
				item.Flash();
				cardNode.FlashRelicOnCard(item);
			}
		}
		if (!(await WaitAndInterruptIfNecessary(0.5f, cardNode)))
		{
			((Node)(object)this).QueueFreeSafely();
			return;
		}
		Vector2 targetPosition = _endCard.Pile.Type.GetTargetPosition(cardNode);
		((Node)cardNode).Reparent((Node)(object)this, true);
		((Control)cardNode).Position = Vector2.Zero;
		NCardFlyVfx nCardFlyVfx = NCardFlyVfx.Create(cardNode, targetPosition, isAddingToPile: false, _endCard.Owner.Character.TrailPath);
		CardPile? pile = _endCard.Pile;
		((Node)((pile == null || pile.Type != PileType.Deck) ? ((object)NCombatRoom.Instance?.CombatVfxContainer) : ((object)NRun.Instance?.GlobalUi.TopBar.TrailContainer)))?.AddChildSafely((Node?)(object)nCardFlyVfx);
		if (nCardFlyVfx?.SwooshAwayCompletion != null)
		{
			await nCardFlyVfx.SwooshAwayCompletion.Task;
		}
		((Node)(object)this).QueueFreeSafely();
	}

	public static async Task PlayAnimOnCardInHand(NCard cardNode, CardModel endCard)
	{
		if (!TestMode.IsOn)
		{
			SfxCmd.Play("event:/sfx/ui/cards/card_transform");
			Tween val = ((Node)cardNode).CreateTween();
			val.TweenProperty((GodotObject)(object)cardNode, NodePath.op_Implicit("scale"), Variant.op_Implicit(Vector2.One * 1.5f), 0.25).From(Variant.op_Implicit(Vector2.One)).SetEase((EaseType)1)
				.SetTrans((TransitionType)7);
			await ((GodotObject)cardNode).ToSignal((GodotObject)(object)val, SignalName.Finished);
			NPlayerHand.Instance?.TryCancelCardPlay(cardNode.Model);
			cardNode.Model = endCard;
			cardNode.UpdateVisuals(endCard.Pile.Type, CardPreviewMode.Normal);
			Tween val2 = ((Node)cardNode).CreateTween();
			val2.TweenProperty((GodotObject)(object)cardNode, NodePath.op_Implicit("scale"), Variant.op_Implicit(Vector2.One), 0.25).From(Variant.op_Implicit(Vector2.One * 1.5f)).SetEase((EaseType)0)
				.SetTrans((TransitionType)7);
			if (NCombatRoom.Instance?.Ui.Hand.GetCardHolder(endCard) is NHandCardHolder nHandCardHolder)
			{
				nHandCardHolder.UpdateCard();
			}
			await ((GodotObject)cardNode).ToSignal((GodotObject)(object)val2, SignalName.Finished);
		}
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
		return ((Node2D)this).InvokeGodotClassMethod(ref method, args, ref ret);
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
		return ((Node2D)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
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
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._tween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		info.AddProperty(PropertyName._tween, Variant.From<Tween>(ref _tween));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._tween, ref val))
		{
			_tween = ((Variant)(ref val)).As<Tween>();
		}
	}
}
