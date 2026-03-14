using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.HoverTips;
using MegaCrit.Sts2.Core.Nodes.Screens;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.TestSupport;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.Nodes.Rewards;

[ScriptPath("res://src/Core/Nodes/Rewards/NRewardButton.cs")]
public class NRewardButton : NButton
{
	[Signal]
	public delegate void RewardClaimedEventHandler(NRewardButton button);

	[Signal]
	public delegate void RewardSkippedEventHandler(NRewardButton button);

	public new class MethodName : NButton.MethodName
	{
		public new static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName Reload = StringName.op_Implicit("Reload");

		public new static readonly StringName OnRelease = StringName.op_Implicit("OnRelease");

		public new static readonly StringName OnPress = StringName.op_Implicit("OnPress");

		public new static readonly StringName OnFocus = StringName.op_Implicit("OnFocus");

		public new static readonly StringName OnUnfocus = StringName.op_Implicit("OnUnfocus");

		public static readonly StringName UpdateShaderParam = StringName.op_Implicit("UpdateShaderParam");
	}

	public new class PropertyName : NButton.PropertyName
	{
		public static readonly StringName _background = StringName.op_Implicit("_background");

		public static readonly StringName _iconContainer = StringName.op_Implicit("_iconContainer");

		public static readonly StringName _label = StringName.op_Implicit("_label");

		public static readonly StringName _reticle = StringName.op_Implicit("_reticle");

		public static readonly StringName _hsv = StringName.op_Implicit("_hsv");

		public static readonly StringName _currentTween = StringName.op_Implicit("_currentTween");

		public static readonly StringName _hsvDefault = StringName.op_Implicit("_hsvDefault");

		public static readonly StringName _hsvHover = StringName.op_Implicit("_hsvHover");

		public static readonly StringName _hsvDown = StringName.op_Implicit("_hsvDown");
	}

	public new class SignalName : NButton.SignalName
	{
		public static readonly StringName RewardClaimed = StringName.op_Implicit("RewardClaimed");

		public static readonly StringName RewardSkipped = StringName.op_Implicit("RewardSkipped");
	}

	private static readonly StringName _v = new StringName("v");

	private static readonly StringName _fontOutlineColor = new StringName("theme_override_colors/font_outline_color");

	private static readonly StringName _defaultColor = new StringName("theme_override_colors/default_color");

	private TextureRect _background;

	private Control _iconContainer;

	private MegaRichTextLabel _label;

	private NSelectionReticle _reticle;

	private ShaderMaterial _hsv;

	private Tween? _currentTween;

	private Variant _hsvDefault = Variant.op_Implicit(0.9);

	private Variant _hsvHover = Variant.op_Implicit(1.1);

	private Variant _hsvDown = Variant.op_Implicit(0.7);

	private RewardClaimedEventHandler backing_RewardClaimed;

	private RewardSkippedEventHandler backing_RewardSkipped;

	public Reward? Reward { get; private set; }

	private static string ScenePath => "res://scenes/rewards/reward_button.tscn";

	public static IEnumerable<string> AssetPaths => new _003C_003Ez__ReadOnlySingleElementList<string>(ScenePath);

	public event RewardClaimedEventHandler RewardClaimed
	{
		add
		{
			backing_RewardClaimed = (RewardClaimedEventHandler)Delegate.Combine(backing_RewardClaimed, value);
		}
		remove
		{
			backing_RewardClaimed = (RewardClaimedEventHandler)Delegate.Remove(backing_RewardClaimed, value);
		}
	}

	public event RewardSkippedEventHandler RewardSkipped
	{
		add
		{
			backing_RewardSkipped = (RewardSkippedEventHandler)Delegate.Combine(backing_RewardSkipped, value);
		}
		remove
		{
			backing_RewardSkipped = (RewardSkippedEventHandler)Delegate.Remove(backing_RewardSkipped, value);
		}
	}

	public static NRewardButton Create(Reward reward, NRewardsScreen screen)
	{
		NRewardButton nRewardButton = PreloadManager.Cache.GetScene(ScenePath).Instantiate<NRewardButton>((GenEditState)0);
		nRewardButton.SetReward(reward);
		return nRewardButton;
	}

	public override void _Ready()
	{
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Expected O, but got Unknown
		ConnectSignals();
		_background = ((Node)this).GetNode<TextureRect>(NodePath.op_Implicit("%Background"));
		_iconContainer = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%Icon"));
		_label = ((Node)this).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("%Label"));
		_reticle = ((Node)this).GetNode<NSelectionReticle>(NodePath.op_Implicit("%SelectionReticle"));
		_hsv = (ShaderMaterial)((CanvasItem)_background).Material;
		Reload();
	}

	private void SetReward(Reward reward)
	{
		if (reward is LinkedRewardSet)
		{
			throw new ArgumentException("You aren't allowed to apply a RewardChainSet to a NRewardButton");
		}
		Reward = reward;
		if (((Node)this).IsNodeReady())
		{
			Reload();
		}
	}

	private void Reload()
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		if (((Node)this).IsNodeReady() && Reward != null)
		{
			Control val = Reward.CreateIcon();
			((Node)(object)_iconContainer).AddChildSafely((Node?)(object)val);
			val.Position = Reward.IconPosition;
			if (Reward is PotionReward)
			{
				val.Scale = 0.8f * Vector2.One;
			}
			_label.Text = Reward.Description.GetFormattedText();
		}
	}

	private async Task GetReward()
	{
		Disable();
		if (await Reward.OnSelectWrapper())
		{
			if (TestMode.IsOff)
			{
				NGlobalUi globalUi = NRun.Instance.GlobalUi;
				Reward reward = Reward;
				if (reward is RelicReward relicReward)
				{
					RelicModel claimedRelic = relicReward.ClaimedRelic;
					if (claimedRelic != null)
					{
						globalUi.RelicInventory.AnimateRelic(relicReward.ClaimedRelic, _iconContainer.GlobalPosition);
					}
				}
				else if (reward is PotionReward potionReward)
				{
					globalUi.TopBar.PotionContainer.AnimatePotion(potionReward.ClaimedPotion, _iconContainer.GlobalPosition);
				}
			}
			_isEnabled = false;
			((GodotObject)this).EmitSignal(SignalName.RewardClaimed, (Variant[])(object)new Variant[1] { Variant.op_Implicit((GodotObject)(object)this) });
		}
		else
		{
			Enable();
			((Control)(object)this).TryGrabFocus();
			((GodotObject)this).EmitSignal(SignalName.RewardSkipped, (Variant[])(object)new Variant[1] { Variant.op_Implicit((GodotObject)(object)this) });
		}
	}

	protected override void OnRelease()
	{
		base.OnRelease();
		OnUnfocus();
		TaskHelper.RunSafely(GetReward());
	}

	protected override void OnPress()
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		base.OnPress();
		Tween? currentTween = _currentTween;
		if (currentTween != null)
		{
			currentTween.Kill();
		}
		_currentTween = ((Node)this).CreateTween().SetParallel(true);
		_currentTween.TweenMethod(Callable.From<float>((Action<float>)UpdateShaderParam), _hsvHover, _hsvDown, 0.5).SetEase((EaseType)1).SetTrans((TransitionType)5);
		_currentTween.TweenProperty((GodotObject)(object)_label, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(1f), 0.5).SetEase((EaseType)1).SetTrans((TransitionType)5);
		NHoverTipSet.Remove((Control)(object)this);
	}

	protected override void OnFocus()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		base.OnFocus();
		((GodotObject)_label).Set(_defaultColor, Variant.op_Implicit(StsColors.gold));
		((GodotObject)_label).Set(_fontOutlineColor, Variant.op_Implicit(StsColors.rewardLabelGoldOutline));
		Tween? currentTween = _currentTween;
		if (currentTween != null)
		{
			currentTween.Kill();
		}
		_hsv.SetShaderParameter(_v, _hsvHover);
		NHoverTipSet nHoverTipSet = NHoverTipSet.CreateAndShow((Control)(object)this, Reward.HoverTips);
		((Control)nHoverTipSet).GlobalPosition = ((Control)this).GlobalPosition + Vector2.Left * 45f;
		nHoverTipSet.SetAlignment((Control)(object)this, HoverTipAlignment.Left);
		_reticle.OnSelect();
	}

	protected override void OnUnfocus()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		base.OnUnfocus();
		((GodotObject)_label).Set(_defaultColor, Variant.op_Implicit(StsColors.cream));
		((GodotObject)_label).Set(_fontOutlineColor, Variant.op_Implicit(StsColors.rewardLabelOutline));
		Tween? currentTween = _currentTween;
		if (currentTween != null)
		{
			currentTween.Kill();
		}
		_currentTween = ((Node)this).CreateTween();
		_currentTween.TweenMethod(Callable.From<float>((Action<float>)UpdateShaderParam), _hsv.GetShaderParameter(_v), _hsvDefault, 0.5).SetEase((EaseType)1).SetTrans((TransitionType)5);
		NHoverTipSet.Remove((Control)(object)this);
		_reticle.OnDeselect();
	}

	private void UpdateShaderParam(float value)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		_hsv.SetShaderParameter(_v, Variant.op_Implicit(value));
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
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(7);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.Reload, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnRelease, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnPress, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnFocus, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnUnfocus, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UpdateShaderParam, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)3, StringName.op_Implicit("value"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.Reload && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			Reload();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnRelease && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnRelease();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnPress && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnPress();
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
		if ((ref method) == MethodName.UpdateShaderParam && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			UpdateShaderParam(VariantUtils.ConvertTo<float>(ref ((NativeVariantPtrArgs)(ref args))[0]));
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
		if ((ref method) == MethodName.Reload)
		{
			return true;
		}
		if ((ref method) == MethodName.OnRelease)
		{
			return true;
		}
		if ((ref method) == MethodName.OnPress)
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
		if ((ref method) == MethodName.UpdateShaderParam)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName._background)
		{
			_background = VariantUtils.ConvertTo<TextureRect>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._iconContainer)
		{
			_iconContainer = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._label)
		{
			_label = VariantUtils.ConvertTo<MegaRichTextLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._reticle)
		{
			_reticle = VariantUtils.ConvertTo<NSelectionReticle>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._hsv)
		{
			_hsv = VariantUtils.ConvertTo<ShaderMaterial>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._currentTween)
		{
			_currentTween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._hsvDefault)
		{
			_hsvDefault = VariantUtils.ConvertTo<Variant>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._hsvHover)
		{
			_hsvHover = VariantUtils.ConvertTo<Variant>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._hsvDown)
		{
			_hsvDown = VariantUtils.ConvertTo<Variant>(ref value);
			return true;
		}
		return base.SetGodotClassPropertyValue(in name, in value);
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
		if ((ref name) == PropertyName._background)
		{
			value = VariantUtils.CreateFrom<TextureRect>(ref _background);
			return true;
		}
		if ((ref name) == PropertyName._iconContainer)
		{
			value = VariantUtils.CreateFrom<Control>(ref _iconContainer);
			return true;
		}
		if ((ref name) == PropertyName._label)
		{
			value = VariantUtils.CreateFrom<MegaRichTextLabel>(ref _label);
			return true;
		}
		if ((ref name) == PropertyName._reticle)
		{
			value = VariantUtils.CreateFrom<NSelectionReticle>(ref _reticle);
			return true;
		}
		if ((ref name) == PropertyName._hsv)
		{
			value = VariantUtils.CreateFrom<ShaderMaterial>(ref _hsv);
			return true;
		}
		if ((ref name) == PropertyName._currentTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _currentTween);
			return true;
		}
		if ((ref name) == PropertyName._hsvDefault)
		{
			value = VariantUtils.CreateFrom<Variant>(ref _hsvDefault);
			return true;
		}
		if ((ref name) == PropertyName._hsvHover)
		{
			value = VariantUtils.CreateFrom<Variant>(ref _hsvHover);
			return true;
		}
		if ((ref name) == PropertyName._hsvDown)
		{
			value = VariantUtils.CreateFrom<Variant>(ref _hsvDown);
			return true;
		}
		return base.GetGodotClassPropertyValue(in name, out value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._background, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._iconContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._label, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._reticle, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._hsv, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._currentTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)0, PropertyName._hsvDefault, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)0, PropertyName._hsvHover, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)0, PropertyName._hsvDown, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName._background, Variant.From<TextureRect>(ref _background));
		info.AddProperty(PropertyName._iconContainer, Variant.From<Control>(ref _iconContainer));
		info.AddProperty(PropertyName._label, Variant.From<MegaRichTextLabel>(ref _label));
		info.AddProperty(PropertyName._reticle, Variant.From<NSelectionReticle>(ref _reticle));
		info.AddProperty(PropertyName._hsv, Variant.From<ShaderMaterial>(ref _hsv));
		info.AddProperty(PropertyName._currentTween, Variant.From<Tween>(ref _currentTween));
		info.AddProperty(PropertyName._hsvDefault, Variant.From<Variant>(ref _hsvDefault));
		info.AddProperty(PropertyName._hsvHover, Variant.From<Variant>(ref _hsvHover));
		info.AddProperty(PropertyName._hsvDown, Variant.From<Variant>(ref _hsvDown));
		info.AddSignalEventDelegate(SignalName.RewardClaimed, (Delegate)backing_RewardClaimed);
		info.AddSignalEventDelegate(SignalName.RewardSkipped, (Delegate)backing_RewardSkipped);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		base.RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._background, ref val))
		{
			_background = ((Variant)(ref val)).As<TextureRect>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._iconContainer, ref val2))
		{
			_iconContainer = ((Variant)(ref val2)).As<Control>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._label, ref val3))
		{
			_label = ((Variant)(ref val3)).As<MegaRichTextLabel>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._reticle, ref val4))
		{
			_reticle = ((Variant)(ref val4)).As<NSelectionReticle>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._hsv, ref val5))
		{
			_hsv = ((Variant)(ref val5)).As<ShaderMaterial>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._currentTween, ref val6))
		{
			_currentTween = ((Variant)(ref val6)).As<Tween>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName._hsvDefault, ref val7))
		{
			_hsvDefault = ((Variant)(ref val7)).As<Variant>();
		}
		Variant val8 = default(Variant);
		if (info.TryGetProperty(PropertyName._hsvHover, ref val8))
		{
			_hsvHover = ((Variant)(ref val8)).As<Variant>();
		}
		Variant val9 = default(Variant);
		if (info.TryGetProperty(PropertyName._hsvDown, ref val9))
		{
			_hsvDown = ((Variant)(ref val9)).As<Variant>();
		}
		RewardClaimedEventHandler rewardClaimedEventHandler = default(RewardClaimedEventHandler);
		if (info.TryGetSignalEventDelegate<RewardClaimedEventHandler>(SignalName.RewardClaimed, ref rewardClaimedEventHandler))
		{
			backing_RewardClaimed = rewardClaimedEventHandler;
		}
		RewardSkippedEventHandler rewardSkippedEventHandler = default(RewardSkippedEventHandler);
		if (info.TryGetSignalEventDelegate<RewardSkippedEventHandler>(SignalName.RewardSkipped, ref rewardSkippedEventHandler))
		{
			backing_RewardSkipped = rewardSkippedEventHandler;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<MethodInfo> GetGodotSignalList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Expected O, but got Unknown
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Expected O, but got Unknown
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(2);
		list.Add(new MethodInfo(SignalName.RewardClaimed, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("button"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(SignalName.RewardSkipped, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("button"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		return list;
	}

	protected void EmitSignalRewardClaimed(NRewardButton button)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).EmitSignal(SignalName.RewardClaimed, (Variant[])(object)new Variant[1] { Variant.op_Implicit((GodotObject)(object)button) });
	}

	protected void EmitSignalRewardSkipped(NRewardButton button)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).EmitSignal(SignalName.RewardSkipped, (Variant[])(object)new Variant[1] { Variant.op_Implicit((GodotObject)(object)button) });
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RaiseGodotClassSignalCallbacks(in godot_string_name signal, NativeVariantPtrArgs args)
	{
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		if ((ref signal) == SignalName.RewardClaimed && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			backing_RewardClaimed?.Invoke(VariantUtils.ConvertTo<NRewardButton>(ref ((NativeVariantPtrArgs)(ref args))[0]));
		}
		else if ((ref signal) == SignalName.RewardSkipped && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			backing_RewardSkipped?.Invoke(VariantUtils.ConvertTo<NRewardButton>(ref ((NativeVariantPtrArgs)(ref args))[0]));
		}
		else
		{
			base.RaiseGodotClassSignalCallbacks(in signal, args);
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassSignal(in godot_string_name signal)
	{
		if ((ref signal) == SignalName.RewardClaimed)
		{
			return true;
		}
		if ((ref signal) == SignalName.RewardSkipped)
		{
			return true;
		}
		return base.HasGodotClassSignal(in signal);
	}
}
