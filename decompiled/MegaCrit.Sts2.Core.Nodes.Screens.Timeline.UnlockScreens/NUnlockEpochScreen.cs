using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Timeline;

namespace MegaCrit.Sts2.Core.Nodes.Screens.Timeline.UnlockScreens;

[ScriptPath("res://src/Core/Nodes/Screens/Timeline/UnlockScreens/NUnlockEpochScreen.cs")]
public class NUnlockEpochScreen : NUnlockScreen
{
	public new class MethodName : NUnlockScreen.MethodName
	{
		public new static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public new static readonly StringName Open = StringName.op_Implicit("Open");
	}

	public new class PropertyName : NUnlockScreen.PropertyName
	{
		public static readonly StringName _cardFlyTween = StringName.op_Implicit("_cardFlyTween");

		public static readonly StringName _infoLabel = StringName.op_Implicit("_infoLabel");
	}

	public new class SignalName : NUnlockScreen.SignalName
	{
	}

	private IReadOnlyList<EpochModel> _unlockedEpochs;

	private Tween? _cardFlyTween;

	private const double _initDelay = 0.3;

	private RichTextLabel _infoLabel;

	public override void _Ready()
	{
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		ConnectSignals();
		_infoLabel = ((Node)this).GetNode<RichTextLabel>(NodePath.op_Implicit("%InfoLabel"));
		LocString locString = new LocString("timeline", "UNLOCK_EPOCHS");
		_infoLabel.Text = "[center]" + locString.GetFormattedText() + "[/center]";
		((CanvasItem)_infoLabel).Modulate = StsColors.transparentWhite;
	}

	public override void Open()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Unknown result type (might be due to invalid IL or missing references)
		base.Open();
		_cardFlyTween = ((Node)this).CreateTween().SetParallel(true);
		double num = 0.3;
		Vector2 position = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%Center")).Position;
		PackedScene scene = PreloadManager.Cache.GetScene("res://scenes/timeline_screen/epoch.tscn");
		if (_unlockedEpochs.Count == 3)
		{
			for (int i = 0; i < _unlockedEpochs.Count; i++)
			{
				NEpochCard nEpochCard = scene.Instantiate<NEpochCard>((GenEditState)0);
				nEpochCard.Init(_unlockedEpochs[i]);
				Control node = ((Node)this).GetNode<Control>(NodePath.op_Implicit($"Slot{i}"));
				((Node)(object)node).AddChildSafely((Node?)(object)nEpochCard);
				nEpochCard.SetToWigglyUnlockPreviewMode();
				_cardFlyTween.TweenProperty((GodotObject)(object)node, NodePath.op_Implicit("modulate"), Variant.op_Implicit(Colors.White), 1.0).SetDelay(num - 0.3);
				_cardFlyTween.TweenProperty((GodotObject)(object)node, NodePath.op_Implicit("position"), Variant.op_Implicit(node.Position), 1.0).SetEase((EaseType)1).SetTrans((TransitionType)10)
					.SetDelay(num);
				((CanvasItem)node).Modulate = StsColors.transparentBlack;
				node.Position = position;
				num += 0.25;
			}
		}
		else if (_unlockedEpochs.Count == 2)
		{
			for (int j = 0; j < _unlockedEpochs.Count; j++)
			{
				NEpochCard nEpochCard2 = scene.Instantiate<NEpochCard>((GenEditState)0);
				nEpochCard2.Init(_unlockedEpochs[j]);
				Control node2 = ((Node)this).GetNode<Control>(NodePath.op_Implicit($"Slot{3 + j}"));
				((Node)(object)node2).AddChildSafely((Node?)(object)nEpochCard2);
				nEpochCard2.SetToWigglyUnlockPreviewMode();
				_cardFlyTween.TweenProperty((GodotObject)(object)node2, NodePath.op_Implicit("modulate"), Variant.op_Implicit(Colors.White), 1.0).SetDelay(num - 0.3);
				_cardFlyTween.TweenProperty((GodotObject)(object)node2, NodePath.op_Implicit("position"), Variant.op_Implicit(node2.Position), 1.0).SetEase((EaseType)1).SetTrans((TransitionType)10)
					.SetDelay(num);
				((CanvasItem)node2).Modulate = StsColors.transparentBlack;
				node2.Position = position;
				num += 0.33;
			}
		}
		else
		{
			Log.Error("Unlocking exactly 1 OR more than 3 Epochs are not supported.");
		}
		_cardFlyTween.TweenProperty((GodotObject)(object)_infoLabel, NodePath.op_Implicit("modulate"), Variant.op_Implicit(Colors.White), 1.0).SetDelay(0.25);
	}

	public void SetUnlocks(IReadOnlyList<EpochModel> epochs)
	{
		_unlockedEpochs = epochs;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(2);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.Open, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
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
		if ((ref method) == MethodName.Open && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			Open();
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
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._cardFlyTween)
		{
			_cardFlyTween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._infoLabel)
		{
			_infoLabel = VariantUtils.ConvertTo<RichTextLabel>(ref value);
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
		if ((ref name) == PropertyName._cardFlyTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _cardFlyTween);
			return true;
		}
		if ((ref name) == PropertyName._infoLabel)
		{
			value = VariantUtils.CreateFrom<RichTextLabel>(ref _infoLabel);
			return true;
		}
		return base.GetGodotClassPropertyValue(in name, out value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._cardFlyTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._infoLabel, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName._cardFlyTween, Variant.From<Tween>(ref _cardFlyTween));
		info.AddProperty(PropertyName._infoLabel, Variant.From<RichTextLabel>(ref _infoLabel));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._cardFlyTween, ref val))
		{
			_cardFlyTween = ((Variant)(ref val)).As<Tween>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._infoLabel, ref val2))
		{
			_infoLabel = ((Variant)(ref val2)).As<RichTextLabel>();
		}
	}
}
