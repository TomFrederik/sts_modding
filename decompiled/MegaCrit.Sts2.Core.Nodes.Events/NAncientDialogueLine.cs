using System;
using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Ancients;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.Nodes.Events;

[ScriptPath("res://src/Core/Nodes/Events/NAncientDialogueLine.cs")]
public class NAncientDialogueLine : NButton
{
	public new class MethodName : NButton.MethodName
	{
		public new static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public new static readonly StringName OnUnfocus = StringName.op_Implicit("OnUnfocus");

		public new static readonly StringName OnFocus = StringName.op_Implicit("OnFocus");

		public new static readonly StringName OnPress = StringName.op_Implicit("OnPress");

		public static readonly StringName PlaySfx = StringName.op_Implicit("PlaySfx");

		public static readonly StringName SetAncientAsSpeaker = StringName.op_Implicit("SetAncientAsSpeaker");

		public static readonly StringName SetCharacterAsSpeaker = StringName.op_Implicit("SetCharacterAsSpeaker");

		public static readonly StringName SetSpeakerIconVisible = StringName.op_Implicit("SetSpeakerIconVisible");

		public static readonly StringName SetTransparency = StringName.op_Implicit("SetTransparency");
	}

	public new class PropertyName : NButton.PropertyName
	{
		public static readonly StringName _iconNode = StringName.op_Implicit("_iconNode");

		public static readonly StringName _tween = StringName.op_Implicit("_tween");

		public static readonly StringName _targetAlpha = StringName.op_Implicit("_targetAlpha");
	}

	public new class SignalName : NButton.SignalName
	{
	}

	private const string _scenePath = "res://scenes/events/ancient_dialogue_line.tscn";

	private AncientDialogueLine _line;

	private AncientEventModel _ancient;

	private CharacterModel _character;

	private Control _iconNode;

	private Tween? _tween;

	private float _targetAlpha = 1f;

	public static NAncientDialogueLine Create(AncientDialogueLine line, AncientEventModel ancient, CharacterModel character)
	{
		NAncientDialogueLine nAncientDialogueLine = PreloadManager.Cache.GetScene("res://scenes/events/ancient_dialogue_line.tscn").Instantiate<NAncientDialogueLine>((GenEditState)0);
		nAncientDialogueLine._line = line;
		nAncientDialogueLine._ancient = ancient;
		nAncientDialogueLine._character = character;
		return nAncientDialogueLine;
	}

	public override void _Ready()
	{
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		ConnectSignals();
		LocString lineText = _line.LineText;
		_character.AddDetailsTo(lineText);
		((Node)this).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("%Text")).Text = lineText.GetFormattedText();
		switch (_line.Speaker)
		{
		case AncientDialogueSpeaker.Ancient:
			SetAncientAsSpeaker();
			break;
		case AncientDialogueSpeaker.Character:
			SetCharacterAsSpeaker();
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
		((CanvasItem)this).Modulate = StsColors.transparentWhite;
		Tween? tween = _tween;
		if (tween != null)
		{
			tween.Kill();
		}
		_tween = ((Node)this).CreateTween();
		_tween.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(1f), 0.1);
	}

	protected override void OnUnfocus()
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		Tween? tween = _tween;
		if (tween != null)
		{
			tween.Kill();
		}
		_tween = ((Node)this).CreateTween();
		_tween.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(_targetAlpha), 0.1);
	}

	protected override void OnFocus()
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		Tween? tween = _tween;
		if (tween != null)
		{
			tween.Kill();
		}
		_tween = ((Node)this).CreateTween();
		_tween.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(1f), 0.1);
	}

	protected override void OnPress()
	{
	}

	public void PlaySfx()
	{
		SfxCmd.Play(_line.GetSfxOrFallbackPath());
	}

	private void SetAncientAsSpeaker()
	{
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		Control node = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%AncientIcon"));
		((Node)node).GetNode<TextureRect>(NodePath.op_Implicit("Icon")).Texture = _ancient.RunHistoryIcon;
		((Node)node).GetNode<TextureRect>(NodePath.op_Implicit("Icon/Outline")).Texture = _ancient.RunHistoryIconOutline;
		_iconNode = node;
		Control node2 = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%DialogueTailLeft"));
		((CanvasItem)node2).Visible = true;
		MarginContainer node3 = ((Node)this).GetNode<MarginContainer>(NodePath.op_Implicit("%TextContainer"));
		((Control)node3).AddThemeConstantOverride(ThemeConstants.MarginContainer.marginLeft, 48);
		((CanvasItem)((Node)this).GetNode<Control>(NodePath.op_Implicit("%Bubble"))).SelfModulate = _ancient.DialogueColor;
		((CanvasItem)node2).SelfModulate = _ancient.DialogueColor;
	}

	private void SetCharacterAsSpeaker()
	{
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		Control node = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%CharacterIcon"));
		((Node)node).GetNode<TextureRect>(NodePath.op_Implicit("Icon")).Texture = _character.IconTexture;
		((Node)node).GetNode<TextureRect>(NodePath.op_Implicit("Icon/Outline")).Texture = _character.IconOutlineTexture;
		_iconNode = node;
		Control node2 = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%DialogueTailRight"));
		((CanvasItem)node2).Visible = true;
		MarginContainer node3 = ((Node)this).GetNode<MarginContainer>(NodePath.op_Implicit("%TextContainer"));
		((Control)node3).AddThemeConstantOverride(ThemeConstants.MarginContainer.marginRight, 46);
		((CanvasItem)((Node)this).GetNode<Control>(NodePath.op_Implicit("%Bubble"))).SelfModulate = _character.DialogueColor;
		((CanvasItem)node2).SelfModulate = _character.DialogueColor;
	}

	public void SetSpeakerIconVisible()
	{
		((CanvasItem)_iconNode).Visible = true;
	}

	public void SetTransparency(float alpha)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		_targetAlpha = alpha;
		((CanvasItem)this).Modulate = new Color(1f, 1f, 1f, alpha);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(9);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnUnfocus, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnFocus, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnPress, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.PlaySfx, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetAncientAsSpeaker, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetCharacterAsSpeaker, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetSpeakerIconVisible, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetTransparency, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)3, StringName.op_Implicit("alpha"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
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
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnUnfocus && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnUnfocus();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnFocus && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnFocus();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnPress && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnPress();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.PlaySfx && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			PlaySfx();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetAncientAsSpeaker && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			SetAncientAsSpeaker();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetCharacterAsSpeaker && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			SetCharacterAsSpeaker();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetSpeakerIconVisible && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			SetSpeakerIconVisible();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetTransparency && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			SetTransparency(VariantUtils.ConvertTo<float>(ref ((NativeVariantPtrArgs)(ref args))[0]));
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
		if ((ref method) == MethodName.OnUnfocus)
		{
			return true;
		}
		if ((ref method) == MethodName.OnFocus)
		{
			return true;
		}
		if ((ref method) == MethodName.OnPress)
		{
			return true;
		}
		if ((ref method) == MethodName.PlaySfx)
		{
			return true;
		}
		if ((ref method) == MethodName.SetAncientAsSpeaker)
		{
			return true;
		}
		if ((ref method) == MethodName.SetCharacterAsSpeaker)
		{
			return true;
		}
		if ((ref method) == MethodName.SetSpeakerIconVisible)
		{
			return true;
		}
		if ((ref method) == MethodName.SetTransparency)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._iconNode)
		{
			_iconNode = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._tween)
		{
			_tween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._targetAlpha)
		{
			_targetAlpha = VariantUtils.ConvertTo<float>(ref value);
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
		if ((ref name) == PropertyName._iconNode)
		{
			value = VariantUtils.CreateFrom<Control>(ref _iconNode);
			return true;
		}
		if ((ref name) == PropertyName._tween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _tween);
			return true;
		}
		if ((ref name) == PropertyName._targetAlpha)
		{
			value = VariantUtils.CreateFrom<float>(ref _targetAlpha);
			return true;
		}
		return base.GetGodotClassPropertyValue(in name, out value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._iconNode, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._tween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)3, PropertyName._targetAlpha, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName._iconNode, Variant.From<Control>(ref _iconNode));
		info.AddProperty(PropertyName._tween, Variant.From<Tween>(ref _tween));
		info.AddProperty(PropertyName._targetAlpha, Variant.From<float>(ref _targetAlpha));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._iconNode, ref val))
		{
			_iconNode = ((Variant)(ref val)).As<Control>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._tween, ref val2))
		{
			_tween = ((Variant)(ref val2)).As<Tween>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._targetAlpha, ref val3))
		{
			_targetAlpha = ((Variant)(ref val3)).As<float>();
		}
	}
}
