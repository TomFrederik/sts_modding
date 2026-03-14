using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Timeline;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.Nodes.Screens.Timeline.UnlockScreens;

[ScriptPath("res://src/Core/Nodes/Screens/Timeline/UnlockScreens/NUnlockCharacterScreen.cs")]
public class NUnlockCharacterScreen : NUnlockScreen
{
	public new class MethodName : NUnlockScreen.MethodName
	{
		public new static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public new static readonly StringName Open = StringName.op_Implicit("Open");

		public new static readonly StringName OnScreenPreClose = StringName.op_Implicit("OnScreenPreClose");

		public new static readonly StringName OnScreenClose = StringName.op_Implicit("OnScreenClose");
	}

	public new class PropertyName : NUnlockScreen.PropertyName
	{
		public static readonly StringName _topLabel = StringName.op_Implicit("_topLabel");

		public static readonly StringName _bottomLabel = StringName.op_Implicit("_bottomLabel");

		public static readonly StringName _spineAnchor = StringName.op_Implicit("_spineAnchor");

		public static readonly StringName _creatureVisuals = StringName.op_Implicit("_creatureVisuals");

		public static readonly StringName _rareGlow = StringName.op_Implicit("_rareGlow");

		public new static readonly StringName _tween = StringName.op_Implicit("_tween");
	}

	public new class SignalName : NUnlockScreen.SignalName
	{
	}

	private static readonly string _scenePath = SceneHelper.GetScenePath("timeline_screen/unlock_character_screen");

	private MegaRichTextLabel _topLabel;

	private MegaRichTextLabel _bottomLabel;

	private Control _spineAnchor;

	private NCreatureVisuals _creatureVisuals;

	private GpuParticles2D _rareGlow;

	private EpochModel _epoch;

	private CharacterModel _character;

	private Tween? _tween;

	public static IEnumerable<string> AssetPaths => new _003C_003Ez__ReadOnlySingleElementList<string>(_scenePath);

	public static NUnlockCharacterScreen Create(EpochModel epoch, CharacterModel character)
	{
		NUnlockCharacterScreen nUnlockCharacterScreen = PreloadManager.Cache.GetScene(_scenePath).Instantiate<NUnlockCharacterScreen>((GenEditState)0);
		nUnlockCharacterScreen._character = character;
		nUnlockCharacterScreen._epoch = epoch;
		return nUnlockCharacterScreen;
	}

	public override void _Ready()
	{
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		ConnectSignals();
		_topLabel = ((Node)this).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("%TopLabel"));
		_bottomLabel = ((Node)this).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("%BottomLabel"));
		_spineAnchor = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%SpineAnchor"));
		_rareGlow = ((Node)this).GetNode<GpuParticles2D>(NodePath.op_Implicit("%RareGlow"));
		_topLabel.Text = new LocString("epochs", _epoch.Id + ".unlock").GetFormattedText();
		_bottomLabel.Text = new LocString("epochs", _epoch.Id + ".unlockText").GetFormattedText();
		((CanvasItem)_topLabel).Modulate = StsColors.transparentBlack;
		((CanvasItem)_bottomLabel).Modulate = StsColors.transparentBlack;
		((CanvasItem)_spineAnchor).Modulate = StsColors.transparentBlack;
		_creatureVisuals = _character.CreateVisuals();
		((Node)(object)_spineAnchor).AddChildSafely((Node?)(object)_creatureVisuals);
	}

	public override void Open()
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		base.Open();
		SfxCmd.Play("event:/sfx/ui/timeline/ui_timeline_unlock");
		_tween = ((Node)this).CreateTween().SetParallel(true);
		_tween.TweenProperty((GodotObject)(object)_spineAnchor, NodePath.op_Implicit("modulate"), Variant.op_Implicit(Colors.White), 0.25).SetDelay(0.25);
		_tween.TweenProperty((GodotObject)(object)_topLabel, NodePath.op_Implicit("modulate"), Variant.op_Implicit(Colors.White), 1.0).SetDelay(1.0);
		_tween.TweenProperty((GodotObject)(object)_bottomLabel, NodePath.op_Implicit("modulate"), Variant.op_Implicit(Colors.White), 1.0).SetDelay(1.5);
		_tween.TweenProperty((GodotObject)(object)_rareGlow, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(1f), 0.5).SetDelay(1.0);
		_creatureVisuals.SpineBody.GetAnimationState().AddAnimation("idle_loop");
		_creatureVisuals.SpineBody.GetAnimationState().AddAnimation("attack", 0.5f, loop: false);
		_creatureVisuals.SpineBody.GetAnimationState().AddAnimation("idle_loop");
	}

	protected override void OnScreenPreClose()
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		Tween? tween = _tween;
		if (tween != null)
		{
			tween.Kill();
		}
		_tween = ((Node)this).CreateTween();
		_tween.TweenProperty((GodotObject)(object)_rareGlow, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0f), 0.5);
	}

	protected override void OnScreenClose()
	{
		NTimelineScreen.Instance.EnableInput();
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
		List<MethodInfo> list = new List<MethodInfo>(4);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.Open, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnScreenPreClose, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnScreenClose, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.Open && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			Open();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnScreenPreClose && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnScreenPreClose();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnScreenClose && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnScreenClose();
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
		if ((ref method) == MethodName.Open)
		{
			return true;
		}
		if ((ref method) == MethodName.OnScreenPreClose)
		{
			return true;
		}
		if ((ref method) == MethodName.OnScreenClose)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._topLabel)
		{
			_topLabel = VariantUtils.ConvertTo<MegaRichTextLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._bottomLabel)
		{
			_bottomLabel = VariantUtils.ConvertTo<MegaRichTextLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._spineAnchor)
		{
			_spineAnchor = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._creatureVisuals)
		{
			_creatureVisuals = VariantUtils.ConvertTo<NCreatureVisuals>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._rareGlow)
		{
			_rareGlow = VariantUtils.ConvertTo<GpuParticles2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._tween)
		{
			_tween = VariantUtils.ConvertTo<Tween>(ref value);
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
		if ((ref name) == PropertyName._topLabel)
		{
			value = VariantUtils.CreateFrom<MegaRichTextLabel>(ref _topLabel);
			return true;
		}
		if ((ref name) == PropertyName._bottomLabel)
		{
			value = VariantUtils.CreateFrom<MegaRichTextLabel>(ref _bottomLabel);
			return true;
		}
		if ((ref name) == PropertyName._spineAnchor)
		{
			value = VariantUtils.CreateFrom<Control>(ref _spineAnchor);
			return true;
		}
		if ((ref name) == PropertyName._creatureVisuals)
		{
			value = VariantUtils.CreateFrom<NCreatureVisuals>(ref _creatureVisuals);
			return true;
		}
		if ((ref name) == PropertyName._rareGlow)
		{
			value = VariantUtils.CreateFrom<GpuParticles2D>(ref _rareGlow);
			return true;
		}
		if ((ref name) == PropertyName._tween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _tween);
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
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._topLabel, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._bottomLabel, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._spineAnchor, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._creatureVisuals, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._rareGlow, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName._topLabel, Variant.From<MegaRichTextLabel>(ref _topLabel));
		info.AddProperty(PropertyName._bottomLabel, Variant.From<MegaRichTextLabel>(ref _bottomLabel));
		info.AddProperty(PropertyName._spineAnchor, Variant.From<Control>(ref _spineAnchor));
		info.AddProperty(PropertyName._creatureVisuals, Variant.From<NCreatureVisuals>(ref _creatureVisuals));
		info.AddProperty(PropertyName._rareGlow, Variant.From<GpuParticles2D>(ref _rareGlow));
		info.AddProperty(PropertyName._tween, Variant.From<Tween>(ref _tween));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._topLabel, ref val))
		{
			_topLabel = ((Variant)(ref val)).As<MegaRichTextLabel>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._bottomLabel, ref val2))
		{
			_bottomLabel = ((Variant)(ref val2)).As<MegaRichTextLabel>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._spineAnchor, ref val3))
		{
			_spineAnchor = ((Variant)(ref val3)).As<Control>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._creatureVisuals, ref val4))
		{
			_creatureVisuals = ((Variant)(ref val4)).As<NCreatureVisuals>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._rareGlow, ref val5))
		{
			_rareGlow = ((Variant)(ref val5)).As<GpuParticles2D>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._tween, ref val6))
		{
			_tween = ((Variant)(ref val6)).As<Tween>();
		}
	}
}
