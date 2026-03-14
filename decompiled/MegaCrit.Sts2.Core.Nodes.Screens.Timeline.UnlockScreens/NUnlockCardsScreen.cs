using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.Cards.Holders;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.Nodes.Screens.Timeline.UnlockScreens;

[ScriptPath("res://src/Core/Nodes/Screens/Timeline/UnlockScreens/NUnlockCardsScreen.cs")]
public class NUnlockCardsScreen : NUnlockScreen
{
	public new class MethodName : NUnlockScreen.MethodName
	{
		public static readonly StringName Create = StringName.op_Implicit("Create");

		public new static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public new static readonly StringName Open = StringName.op_Implicit("Open");

		public new static readonly StringName OnScreenPreClose = StringName.op_Implicit("OnScreenPreClose");

		public new static readonly StringName OnScreenClose = StringName.op_Implicit("OnScreenClose");

		public static readonly StringName _ExitTree = StringName.op_Implicit("_ExitTree");
	}

	public new class PropertyName : NUnlockScreen.PropertyName
	{
		public static readonly StringName _cardRow = StringName.op_Implicit("_cardRow");

		public static readonly StringName _banner = StringName.op_Implicit("_banner");

		public static readonly StringName _cardTween = StringName.op_Implicit("_cardTween");
	}

	public new class SignalName : NUnlockScreen.SignalName
	{
	}

	private static readonly string _scenePath = SceneHelper.GetScenePath("timeline_screen/unlock_cards_screen");

	private Control _cardRow;

	private NCommonBanner _banner;

	private readonly List<NCard> _nodes = new List<NCard>();

	private IReadOnlyList<CardModel> _cards;

	private Tween? _cardTween;

	private const float _cardXOffset = 350f;

	public static IEnumerable<string> AssetPaths => new _003C_003Ez__ReadOnlySingleElementList<string>(_scenePath);

	public static NUnlockCardsScreen Create()
	{
		return PreloadManager.Cache.GetScene(_scenePath).Instantiate<NUnlockCardsScreen>((GenEditState)0);
	}

	public override void _Ready()
	{
		ConnectSignals();
		_banner = ((Node)this).GetNode<NCommonBanner>(NodePath.op_Implicit("%Banner"));
		_banner.label.SetTextAutoSize(new LocString("timeline", "UNLOCK_CARDS_BANNER").GetRawText());
		_banner.AnimateIn();
		_cardRow = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%CardRow"));
		LocString locString = new LocString("timeline", "UNLOCK_CARDS");
		((Node)this).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("%ExplanationText")).Text = "[center]" + locString.GetFormattedText() + "[/center]";
	}

	public override void Open()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		base.Open();
		SfxCmd.Play("event:/sfx/ui/timeline/ui_timeline_unlock");
		Vector2 val = Vector2.Left * (float)(_cards.Count - 1) * 350f * 0.5f;
		_cardTween = ((Node)this).CreateTween().SetParallel(true);
		int num = 0;
		foreach (CardModel card in _cards)
		{
			NCard nCard = NCard.Create(card);
			NGridCardHolder nGridCardHolder = NGridCardHolder.Create(nCard);
			((Node)(object)_cardRow).AddChildSafely((Node?)(object)nGridCardHolder);
			nCard.UpdateVisuals(PileType.None, CardPreviewMode.Normal);
			((Control)nGridCardHolder).Scale = nGridCardHolder.SmallScale;
			_cardTween.TweenProperty((GodotObject)(object)nGridCardHolder, NodePath.op_Implicit("position"), Variant.op_Implicit(((Control)nGridCardHolder).Position + val + Vector2.Right * 350f * (float)num), 0.5).SetEase((EaseType)1).SetTrans((TransitionType)5);
			_cardTween.TweenProperty((GodotObject)(object)nGridCardHolder, NodePath.op_Implicit("modulate"), Variant.op_Implicit(Colors.White), 1.0).SetEase((EaseType)1).SetTrans((TransitionType)7)
				.From(Variant.op_Implicit(Colors.Black));
			nCard.ActivateRewardScreenGlow();
			_nodes.Add(nCard);
			num++;
		}
	}

	public void SetCards(IReadOnlyList<CardModel> cards)
	{
		_cards = cards;
	}

	protected override void OnScreenPreClose()
	{
		foreach (NCard node in _nodes)
		{
			node.KillRarityGlow();
		}
	}

	protected override void OnScreenClose()
	{
		NTimelineScreen.Instance.EnableInput();
	}

	public override void _ExitTree()
	{
		foreach (NGridCardHolder item in ((IEnumerable)((Node)_cardRow).GetChildren(false)).OfType<NGridCardHolder>())
		{
			((Node)(object)item).QueueFreeSafely();
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Expected O, but got Unknown
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(6);
		list.Add(new MethodInfo(MethodName.Create, new PropertyInfo((Type)24, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false), (MethodFlags)33, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.Open, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnScreenPreClose, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnScreenClose, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._ExitTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.Create && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			NUnlockCardsScreen nUnlockCardsScreen = Create();
			ret = VariantUtils.CreateFrom<NUnlockCardsScreen>(ref nUnlockCardsScreen);
			return true;
		}
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
		if ((ref method) == MethodName._ExitTree && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._ExitTree();
			ret = default(godot_variant);
			return true;
		}
		return base.InvokeGodotClassMethod(in method, args, out ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static bool InvokeGodotClassStaticMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.Create && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			NUnlockCardsScreen nUnlockCardsScreen = Create();
			ret = VariantUtils.CreateFrom<NUnlockCardsScreen>(ref nUnlockCardsScreen);
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
		if ((ref method) == MethodName._ExitTree)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._cardRow)
		{
			_cardRow = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._banner)
		{
			_banner = VariantUtils.ConvertTo<NCommonBanner>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._cardTween)
		{
			_cardTween = VariantUtils.ConvertTo<Tween>(ref value);
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
		if ((ref name) == PropertyName._cardRow)
		{
			value = VariantUtils.CreateFrom<Control>(ref _cardRow);
			return true;
		}
		if ((ref name) == PropertyName._banner)
		{
			value = VariantUtils.CreateFrom<NCommonBanner>(ref _banner);
			return true;
		}
		if ((ref name) == PropertyName._cardTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _cardTween);
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
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._cardRow, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._banner, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._cardTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName._cardRow, Variant.From<Control>(ref _cardRow));
		info.AddProperty(PropertyName._banner, Variant.From<NCommonBanner>(ref _banner));
		info.AddProperty(PropertyName._cardTween, Variant.From<Tween>(ref _cardTween));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._cardRow, ref val))
		{
			_cardRow = ((Variant)(ref val)).As<Control>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._banner, ref val2))
		{
			_banner = ((Variant)(ref val2)).As<NCommonBanner>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._cardTween, ref val3))
		{
			_cardTween = ((Variant)(ref val3)).As<Tween>();
		}
	}
}
