using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
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
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.TestSupport;

namespace MegaCrit.Sts2.Core.Nodes.Vfx;

[ScriptPath("res://src/Core/Nodes/Vfx/NCardUpgradeVfx.cs")]
public class NCardUpgradeVfx : Node2D
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName _ExitTree = StringName.op_Implicit("_ExitTree");
	}

	public class PropertyName : PropertyName
	{
	}

	public class SignalName : SignalName
	{
	}

	private CardModel _card;

	private CancellationTokenSource? _cts;

	private static string ScenePath => SceneHelper.GetScenePath("vfx/vfx_card_upgrade");

	public static IEnumerable<string> AssetPaths => new _003C_003Ez__ReadOnlySingleElementList<string>(ScenePath);

	public static NCardUpgradeVfx? Create(CardModel card)
	{
		if (TestMode.IsOn)
		{
			return null;
		}
		NCardUpgradeVfx nCardUpgradeVfx = PreloadManager.Cache.GetScene(ScenePath).Instantiate<NCardUpgradeVfx>((GenEditState)0);
		nCardUpgradeVfx._card = card;
		return nCardUpgradeVfx;
	}

	public override void _Ready()
	{
		TaskHelper.RunSafely(PlayAnimation());
	}

	public override void _ExitTree()
	{
		_cts?.Cancel();
		_cts?.Dispose();
	}

	private async Task PlayAnimation()
	{
		_cts = new CancellationTokenSource();
		NCard cardNode = NCard.Create(_card);
		((Node)(object)this).AddChildSafely((Node?)(object)cardNode);
		((Node)this).MoveChild((Node)(object)cardNode, 0);
		cardNode.UpdateVisuals(PileType.None, CardPreviewMode.Normal);
		((Node)this).GetNode<CpuParticles2D>(NodePath.op_Implicit("%Particle")).Emitting = true;
		Tween val = ((Node)this).CreateTween();
		val.TweenProperty((GodotObject)(object)cardNode, NodePath.op_Implicit("scale"), Variant.op_Implicit(Vector2.One * 1f), 0.25).From(Variant.op_Implicit(Vector2.Zero)).SetEase((EaseType)1)
			.SetTrans((TransitionType)7);
		await Cmd.Wait(1.75f, _cts.Token);
		Vector2 targetPosition = _card.Pile.Type.GetTargetPosition(cardNode);
		NCardFlyVfx nCardFlyVfx = NCardFlyVfx.Create(cardNode, targetPosition, isAddingToPile: false, _card.Owner.Character.TrailPath);
		((Node)((_card.Pile.Type != PileType.Deck) ? ((object)NCombatRoom.Instance?.CombatVfxContainer) : ((object)NRun.Instance?.GlobalUi.TopBar.TrailContainer)))?.AddChildSafely((Node?)(object)nCardFlyVfx);
		if (nCardFlyVfx?.SwooshAwayCompletion != null)
		{
			await nCardFlyVfx.SwooshAwayCompletion.Task;
		}
		if (!_cts.IsCancellationRequested)
		{
			((Node)(object)this).QueueFreeSafely();
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
