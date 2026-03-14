using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.HoverTips;
using MegaCrit.Sts2.Core.Nodes.Relics;
using MegaCrit.Sts2.Core.Runs;

namespace MegaCrit.Sts2.Core.Nodes.Screens.TreasureRoomRelic;

[ScriptPath("res://src/Core/Nodes/Screens/TreasureRoomRelic/NTreasureRoomRelicHolder.cs")]
public class NTreasureRoomRelicHolder : NButton
{
	public new class MethodName : NButton.MethodName
	{
		public new static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName AnimateAwayVotes = StringName.op_Implicit("AnimateAwayVotes");

		public new static readonly StringName OnFocus = StringName.op_Implicit("OnFocus");

		public new static readonly StringName OnUnfocus = StringName.op_Implicit("OnUnfocus");

		public new static readonly StringName OnPress = StringName.op_Implicit("OnPress");

		public new static readonly StringName OnRelease = StringName.op_Implicit("OnRelease");
	}

	public new class PropertyName : NButton.PropertyName
	{
		public static readonly StringName Index = StringName.op_Implicit("Index");

		public static readonly StringName VoteContainer = StringName.op_Implicit("VoteContainer");

		public static readonly StringName Relic = StringName.op_Implicit("Relic");

		public static readonly StringName _uncommonGlow = StringName.op_Implicit("_uncommonGlow");

		public static readonly StringName _rareGlow = StringName.op_Implicit("_rareGlow");

		public static readonly StringName _animatedIn = StringName.op_Implicit("_animatedIn");

		public static readonly StringName _tween = StringName.op_Implicit("_tween");

		public static readonly StringName _initTween = StringName.op_Implicit("_initTween");
	}

	public new class SignalName : NButton.SignalName
	{
	}

	private GpuParticles2D _uncommonGlow;

	private GpuParticles2D _rareGlow;

	private bool _animatedIn;

	private Tween? _tween;

	private Tween? _initTween;

	public int Index { get; set; }

	public NMultiplayerVoteContainer VoteContainer { get; private set; }

	public NRelic Relic { get; private set; }

	public override void _Ready()
	{
		ConnectSignals();
		VoteContainer = ((Node)this).GetNode<NMultiplayerVoteContainer>(NodePath.op_Implicit("%MultiplayerVoteContainer"));
		Relic = ((Node)this).GetNode<NRelic>(NodePath.op_Implicit("%Relic"));
		_uncommonGlow = ((Node)this).GetNode<GpuParticles2D>(NodePath.op_Implicit("%UncommonGlow"));
		_rareGlow = ((Node)this).GetNode<GpuParticles2D>(NodePath.op_Implicit("%RareGlow"));
	}

	public void Initialize(RelicModel relic, IRunState runState)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		Relic.Model = relic;
		VoteContainer.Initialize(PlayerVotedForRelic, runState.Players);
		((CanvasItem)Relic).Modulate = StsColors.transparentBlack;
		Tween? initTween = _initTween;
		if (initTween != null)
		{
			initTween.Kill();
		}
		_initTween = ((Node)this).CreateTween().SetParallel(true);
		_initTween.TweenProperty((GodotObject)(object)Relic, NodePath.op_Implicit("modulate"), Variant.op_Implicit(Colors.White), 0.25);
		if (Relic.Model.Rarity == RelicRarity.Uncommon)
		{
			Tween val = ((Node)this).CreateTween().SetParallel(true);
			((CanvasItem)_uncommonGlow).Visible = true;
			((CanvasItem)_uncommonGlow).Modulate = StsColors.transparentWhite;
			((Node2D)_uncommonGlow).GlobalPosition = ((Control)Relic).GlobalPosition + Vector2.One * 68f;
			val.TweenProperty((GodotObject)(object)_uncommonGlow, NodePath.op_Implicit("modulate"), Variant.op_Implicit(Colors.White), 0.25);
		}
		else if (Relic.Model.Rarity == RelicRarity.Rare)
		{
			Tween val2 = ((Node)this).CreateTween().SetParallel(true);
			((CanvasItem)_rareGlow).Visible = true;
			((CanvasItem)_rareGlow).Modulate = StsColors.transparentWhite;
			((Node2D)_rareGlow).GlobalPosition = ((Control)Relic).GlobalPosition + Vector2.One * 68f;
			val2.TweenProperty((GodotObject)(object)_rareGlow, NodePath.op_Implicit("modulate"), Variant.op_Implicit(Colors.White), 0.25);
		}
	}

	private bool PlayerVotedForRelic(Player player)
	{
		return RunManager.Instance.TreasureRoomRelicSynchronizer.GetPlayerVote(player) == Index;
	}

	public void AnimateAwayVotes()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		((Node)this).CreateTween().TweenProperty((GodotObject)(object)VoteContainer, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0f), 0.25);
	}

	protected override void OnFocus()
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		base.OnFocus();
		Tween? tween = _tween;
		if (tween != null)
		{
			tween.Kill();
		}
		_tween = ((Node)this).CreateTween().SetParallel(true);
		_tween.TweenProperty((GodotObject)(object)Relic, NodePath.op_Implicit("scale"), Variant.op_Implicit(Vector2.One * 2.1f), 0.05);
		NHoverTipSet nHoverTipSet = NHoverTipSet.CreateAndShow((Control)(object)this, Relic.Model.HoverTips);
		nHoverTipSet.SetAlignmentForRelic(Relic);
	}

	protected override void OnUnfocus()
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		base.OnUnfocus();
		Tween? tween = _tween;
		if (tween != null)
		{
			tween.Kill();
		}
		_tween = ((Node)this).CreateTween().SetParallel(true);
		_tween.TweenProperty((GodotObject)(object)Relic, NodePath.op_Implicit("scale"), Variant.op_Implicit(Vector2.One * 2f), 0.4).SetEase((EaseType)1).SetTrans((TransitionType)5);
		NHoverTipSet.Remove((Control)(object)this);
	}

	protected override void OnPress()
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		base.OnPress();
		Tween? tween = _tween;
		if (tween != null)
		{
			tween.Kill();
		}
		_tween = ((Node)this).CreateTween().SetParallel(true);
		_tween.TweenProperty((GodotObject)(object)Relic, NodePath.op_Implicit("scale"), Variant.op_Implicit(Vector2.One * 1.9f), 0.4).SetEase((EaseType)1).SetTrans((TransitionType)5);
	}

	protected override void OnRelease()
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		base.OnRelease();
		Tween? tween = _tween;
		if (tween != null)
		{
			tween.Kill();
		}
		_tween = ((Node)this).CreateTween().SetParallel(true);
		_tween.TweenProperty((GodotObject)(object)Relic, NodePath.op_Implicit("scale"), Variant.op_Implicit(Vector2.One * 2f), 0.05).SetEase((EaseType)1).SetTrans((TransitionType)5);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(6);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AnimateAwayVotes, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnFocus, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnUnfocus, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnPress, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnRelease, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.AnimateAwayVotes && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			AnimateAwayVotes();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnFocus && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnFocus();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnUnfocus && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnUnfocus();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnPress && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnPress();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnRelease && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnRelease();
			ret = default(godot_variant);
			return true;
		}
		return base.InvokeGodotClassMethod(in method, args, out ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if ((ref method) == MethodName._Ready)
		{
			return true;
		}
		if ((ref method) == MethodName.AnimateAwayVotes)
		{
			return true;
		}
		if ((ref method) == MethodName.OnFocus)
		{
			return true;
		}
		if ((ref method) == MethodName.OnUnfocus)
		{
			return true;
		}
		if ((ref method) == MethodName.OnPress)
		{
			return true;
		}
		if ((ref method) == MethodName.OnRelease)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName.Index)
		{
			Index = VariantUtils.ConvertTo<int>(ref value);
			return true;
		}
		if ((ref name) == PropertyName.VoteContainer)
		{
			VoteContainer = VariantUtils.ConvertTo<NMultiplayerVoteContainer>(ref value);
			return true;
		}
		if ((ref name) == PropertyName.Relic)
		{
			Relic = VariantUtils.ConvertTo<NRelic>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._uncommonGlow)
		{
			_uncommonGlow = VariantUtils.ConvertTo<GpuParticles2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._rareGlow)
		{
			_rareGlow = VariantUtils.ConvertTo<GpuParticles2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._animatedIn)
		{
			_animatedIn = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._tween)
		{
			_tween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._initTween)
		{
			_initTween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		return base.SetGodotClassPropertyValue(in name, in value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName.Index)
		{
			int index = Index;
			value = VariantUtils.CreateFrom<int>(ref index);
			return true;
		}
		if ((ref name) == PropertyName.VoteContainer)
		{
			NMultiplayerVoteContainer voteContainer = VoteContainer;
			value = VariantUtils.CreateFrom<NMultiplayerVoteContainer>(ref voteContainer);
			return true;
		}
		if ((ref name) == PropertyName.Relic)
		{
			NRelic relic = Relic;
			value = VariantUtils.CreateFrom<NRelic>(ref relic);
			return true;
		}
		if ((ref name) == PropertyName._uncommonGlow)
		{
			value = VariantUtils.CreateFrom<GpuParticles2D>(ref _uncommonGlow);
			return true;
		}
		if ((ref name) == PropertyName._rareGlow)
		{
			value = VariantUtils.CreateFrom<GpuParticles2D>(ref _rareGlow);
			return true;
		}
		if ((ref name) == PropertyName._animatedIn)
		{
			value = VariantUtils.CreateFrom<bool>(ref _animatedIn);
			return true;
		}
		if ((ref name) == PropertyName._tween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _tween);
			return true;
		}
		if ((ref name) == PropertyName._initTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _initTween);
			return true;
		}
		return base.GetGodotClassPropertyValue(in name, out value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)2, PropertyName.Index, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.VoteContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.Relic, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._uncommonGlow, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._rareGlow, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._animatedIn, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._tween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._initTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		base.SaveGodotObjectData(info);
		StringName index = PropertyName.Index;
		int index2 = Index;
		info.AddProperty(index, Variant.From<int>(ref index2));
		StringName voteContainer = PropertyName.VoteContainer;
		NMultiplayerVoteContainer voteContainer2 = VoteContainer;
		info.AddProperty(voteContainer, Variant.From<NMultiplayerVoteContainer>(ref voteContainer2));
		StringName relic = PropertyName.Relic;
		NRelic relic2 = Relic;
		info.AddProperty(relic, Variant.From<NRelic>(ref relic2));
		info.AddProperty(PropertyName._uncommonGlow, Variant.From<GpuParticles2D>(ref _uncommonGlow));
		info.AddProperty(PropertyName._rareGlow, Variant.From<GpuParticles2D>(ref _rareGlow));
		info.AddProperty(PropertyName._animatedIn, Variant.From<bool>(ref _animatedIn));
		info.AddProperty(PropertyName._tween, Variant.From<Tween>(ref _tween));
		info.AddProperty(PropertyName._initTween, Variant.From<Tween>(ref _initTween));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName.Index, ref val))
		{
			Index = ((Variant)(ref val)).As<int>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName.VoteContainer, ref val2))
		{
			VoteContainer = ((Variant)(ref val2)).As<NMultiplayerVoteContainer>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName.Relic, ref val3))
		{
			Relic = ((Variant)(ref val3)).As<NRelic>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._uncommonGlow, ref val4))
		{
			_uncommonGlow = ((Variant)(ref val4)).As<GpuParticles2D>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._rareGlow, ref val5))
		{
			_rareGlow = ((Variant)(ref val5)).As<GpuParticles2D>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._animatedIn, ref val6))
		{
			_animatedIn = ((Variant)(ref val6)).As<bool>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName._tween, ref val7))
		{
			_tween = ((Variant)(ref val7)).As<Tween>();
		}
		Variant val8 = default(Variant);
		if (info.TryGetProperty(PropertyName._initTween, ref val8))
		{
			_initTween = ((Variant)(ref val8)).As<Tween>();
		}
	}
}
