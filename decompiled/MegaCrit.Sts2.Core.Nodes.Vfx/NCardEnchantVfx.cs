using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.TestSupport;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.Nodes.Vfx;

[ScriptPath("res://src/Core/Nodes/Vfx/NCardEnchantVfx.cs")]
public class NCardEnchantVfx : Node2D
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName _ExitTree = StringName.op_Implicit("_ExitTree");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName EmbossCurve = StringName.op_Implicit("EmbossCurve");

		public static readonly StringName _tween = StringName.op_Implicit("_tween");

		public static readonly StringName _cardNode = StringName.op_Implicit("_cardNode");

		public static readonly StringName _enchantmentSparkles = StringName.op_Implicit("_enchantmentSparkles");

		public static readonly StringName _enchantmentIcon = StringName.op_Implicit("_enchantmentIcon");

		public static readonly StringName _enchantmentLabel = StringName.op_Implicit("_enchantmentLabel");
	}

	public class SignalName : SignalName
	{
	}

	private static readonly StringName _progress = new StringName("progress");

	private Tween? _tween;

	private CancellationTokenSource? _cts;

	private CardModel _cardModel;

	private NCard _cardNode;

	private GpuParticles2D _enchantmentSparkles;

	private TextureRect _enchantmentIcon;

	private MegaLabel _enchantmentLabel;

	private static string ScenePath => SceneHelper.GetScenePath("vfx/vfx_card_enchant");

	public static IEnumerable<string> AssetPaths => new _003C_003Ez__ReadOnlySingleElementList<string>(ScenePath);

	[Export(/*Could not decode attribute arguments.*/)]
	public Curve? EmbossCurve { get; set; }

	public static NCardEnchantVfx? Create(CardModel card)
	{
		if (TestMode.IsOn)
		{
			return null;
		}
		if (!LocalContext.IsMine(card))
		{
			return null;
		}
		NCardEnchantVfx nCardEnchantVfx = PreloadManager.Cache.GetScene(ScenePath).Instantiate<NCardEnchantVfx>((GenEditState)0);
		nCardEnchantVfx._cardModel = card;
		return nCardEnchantVfx;
	}

	public override void _Ready()
	{
		_enchantmentSparkles = ((Node)this).GetNode<GpuParticles2D>(NodePath.op_Implicit("%EnchantmentAppearSparkles"));
		_enchantmentIcon = ((Node)this).GetNode<TextureRect>(NodePath.op_Implicit("%EnchantmentInViewport/Icon"));
		_enchantmentLabel = ((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%EnchantmentInViewport/Label"));
		_enchantmentIcon.Texture = (Texture2D)(object)_cardModel.Enchantment.Icon;
		_enchantmentLabel.SetTextAutoSize(_cardModel.Enchantment.DisplayAmount.ToString());
		((CanvasItem)_enchantmentLabel).Visible = _cardModel.Enchantment.ShowAmount;
		_cardNode = NCard.Create(_cardModel);
		((Node)(object)this).AddChildSafely((Node?)(object)_cardNode);
		((Node)this).MoveChild((Node)(object)_cardNode, 0);
		_cardNode.UpdateVisuals(PileType.None, CardPreviewMode.Normal);
		((CanvasItem)_cardNode.EnchantmentTab).Visible = false;
		((CanvasItem)_cardNode.EnchantmentVfxOverride).Visible = true;
		Viewport node = ((Node)this).GetNode<Viewport>(NodePath.op_Implicit("%EnchantmentViewport"));
		_cardNode.EnchantmentVfxOverride.Texture = (Texture2D)(object)node.GetTexture();
		TaskHelper.RunSafely(PlayAnimation());
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
		if (((Node?)(object)_cardNode).IsValid() && ((Node)this).IsAncestorOf((Node)(object)_cardNode))
		{
			((Node)(object)_cardNode).QueueFreeSafely();
		}
	}

	private async Task PlayAnimation()
	{
		_cts = new CancellationTokenSource();
		((ShaderMaterial)((CanvasItem)_cardNode.EnchantmentVfxOverride).Material).SetShaderParameter(_progress, Variant.op_Implicit(0f));
		_tween = ((Node)this).CreateTween();
		SfxCmd.Play("event:/sfx/ui/enchant_shimmer");
		_tween.TweenProperty((GodotObject)(object)_cardNode.EnchantmentVfxOverride, NodePath.op_Implicit("material:shader_parameter/progress"), Variant.op_Implicit(1f), 1.0).SetEase((EaseType)2).SetTrans((TransitionType)4);
		_tween.Parallel().TweenCallback(Callable.From<bool>((Func<bool>)(() => _enchantmentSparkles.Emitting = true))).SetDelay(0.20000000298023224);
		_tween.Parallel().TweenProperty((GodotObject)(object)_enchantmentSparkles, NodePath.op_Implicit("position:x"), Variant.op_Implicit(((Node2D)_enchantmentSparkles).Position.X + 72f), 0.4000000059604645).SetDelay(0.20000000298023224);
		await ((GodotObject)this).ToSignal((GodotObject)(object)_tween, SignalName.Finished);
		await Cmd.Wait(1f, _cts.Token);
		CardModel model = _cardNode.Model;
		if (((Node)_cardNode).IsInsideTree() && model.Pile == null)
		{
			_tween = ((Node)this).CreateTween();
			_tween.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("scale"), Variant.op_Implicit(Vector2.Zero), 0.15000000596046448);
			await ((GodotObject)this).ToSignal((GodotObject)(object)_tween, SignalName.Finished);
		}
		else if (((Node)_cardNode).IsInsideTree())
		{
			Vector2 targetPosition = model.Pile.Type.GetTargetPosition(_cardNode);
			NCardFlyVfx nCardFlyVfx = NCardFlyVfx.Create(_cardNode, targetPosition, isAddingToPile: false, model.Owner.Character.TrailPath);
			NRun.Instance?.GlobalUi.TopBar.TrailContainer.AddChildSafely((Node?)(object)nCardFlyVfx);
			if (nCardFlyVfx.SwooshAwayCompletion != null)
			{
				await nCardFlyVfx.SwooshAwayCompletion.Task;
			}
		}
		((Node)(object)this).QueueFreeSafely();
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
		if ((ref name) == PropertyName.EmbossCurve)
		{
			EmbossCurve = VariantUtils.ConvertTo<Curve>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._tween)
		{
			_tween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._cardNode)
		{
			_cardNode = VariantUtils.ConvertTo<NCard>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._enchantmentSparkles)
		{
			_enchantmentSparkles = VariantUtils.ConvertTo<GpuParticles2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._enchantmentIcon)
		{
			_enchantmentIcon = VariantUtils.ConvertTo<TextureRect>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._enchantmentLabel)
		{
			_enchantmentLabel = VariantUtils.ConvertTo<MegaLabel>(ref value);
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
		if ((ref name) == PropertyName.EmbossCurve)
		{
			Curve embossCurve = EmbossCurve;
			value = VariantUtils.CreateFrom<Curve>(ref embossCurve);
			return true;
		}
		if ((ref name) == PropertyName._tween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _tween);
			return true;
		}
		if ((ref name) == PropertyName._cardNode)
		{
			value = VariantUtils.CreateFrom<NCard>(ref _cardNode);
			return true;
		}
		if ((ref name) == PropertyName._enchantmentSparkles)
		{
			value = VariantUtils.CreateFrom<GpuParticles2D>(ref _enchantmentSparkles);
			return true;
		}
		if ((ref name) == PropertyName._enchantmentIcon)
		{
			value = VariantUtils.CreateFrom<TextureRect>(ref _enchantmentIcon);
			return true;
		}
		if ((ref name) == PropertyName._enchantmentLabel)
		{
			value = VariantUtils.CreateFrom<MegaLabel>(ref _enchantmentLabel);
			return true;
		}
		return ((GodotObject)this).GetGodotClassPropertyValue(ref name, ref value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._tween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.EmbossCurve, (PropertyHint)17, "Curve", (PropertyUsageFlags)4102, true));
		list.Add(new PropertyInfo((Type)24, PropertyName._cardNode, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._enchantmentSparkles, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._enchantmentIcon, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._enchantmentLabel, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		((GodotObject)this).SaveGodotObjectData(info);
		StringName embossCurve = PropertyName.EmbossCurve;
		Curve embossCurve2 = EmbossCurve;
		info.AddProperty(embossCurve, Variant.From<Curve>(ref embossCurve2));
		info.AddProperty(PropertyName._tween, Variant.From<Tween>(ref _tween));
		info.AddProperty(PropertyName._cardNode, Variant.From<NCard>(ref _cardNode));
		info.AddProperty(PropertyName._enchantmentSparkles, Variant.From<GpuParticles2D>(ref _enchantmentSparkles));
		info.AddProperty(PropertyName._enchantmentIcon, Variant.From<TextureRect>(ref _enchantmentIcon));
		info.AddProperty(PropertyName._enchantmentLabel, Variant.From<MegaLabel>(ref _enchantmentLabel));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName.EmbossCurve, ref val))
		{
			EmbossCurve = ((Variant)(ref val)).As<Curve>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._tween, ref val2))
		{
			_tween = ((Variant)(ref val2)).As<Tween>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._cardNode, ref val3))
		{
			_cardNode = ((Variant)(ref val3)).As<NCard>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._enchantmentSparkles, ref val4))
		{
			_enchantmentSparkles = ((Variant)(ref val4)).As<GpuParticles2D>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._enchantmentIcon, ref val5))
		{
			_enchantmentIcon = ((Variant)(ref val5)).As<TextureRect>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._enchantmentLabel, ref val6))
		{
			_enchantmentLabel = ((Variant)(ref val6)).As<MegaLabel>();
		}
	}
}
