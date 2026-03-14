using System;
using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.Nodes.Screens.RunHistoryScreen;

[ScriptPath("res://src/Core/Nodes/Screens/RunHistoryScreen/NDeckHistoryEntry.cs")]
public class NDeckHistoryEntry : NButton
{
	[Signal]
	public delegate void ClickedEventHandler(NDeckHistoryEntry entry);

	public new class MethodName : NButton.MethodName
	{
		public new static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName Reload = StringName.op_Implicit("Reload");

		public new static readonly StringName OnFocus = StringName.op_Implicit("OnFocus");

		public new static readonly StringName OnUnfocus = StringName.op_Implicit("OnUnfocus");

		public new static readonly StringName OnRelease = StringName.op_Implicit("OnRelease");
	}

	public new class PropertyName : NButton.PropertyName
	{
		public static readonly StringName _titleLabel = StringName.op_Implicit("_titleLabel");

		public static readonly StringName _cardImage = StringName.op_Implicit("_cardImage");

		public static readonly StringName _enchantmentImage = StringName.op_Implicit("_enchantmentImage");

		public static readonly StringName _labelContainer = StringName.op_Implicit("_labelContainer");

		public static readonly StringName _scaleTween = StringName.op_Implicit("_scaleTween");

		public static readonly StringName _amount = StringName.op_Implicit("_amount");
	}

	public new class SignalName : NButton.SignalName
	{
		public static readonly StringName Clicked = StringName.op_Implicit("Clicked");
	}

	private MegaLabel _titleLabel;

	private NTinyCard _cardImage;

	private TextureRect _enchantmentImage;

	private MarginContainer _labelContainer;

	private Tween? _scaleTween;

	private int _amount;

	private ClickedEventHandler backing_Clicked;

	private static string ScenePath => SceneHelper.GetScenePath("screens/run_history_screen/deck_history_entry");

	public static IEnumerable<string> AssetPaths => new _003C_003Ez__ReadOnlySingleElementList<string>(ScenePath);

	public IEnumerable<int> FloorsAddedToDeck { get; private set; }

	public CardModel Card { get; private set; }

	public event ClickedEventHandler Clicked
	{
		add
		{
			backing_Clicked = (ClickedEventHandler)Delegate.Combine(backing_Clicked, value);
		}
		remove
		{
			backing_Clicked = (ClickedEventHandler)Delegate.Remove(backing_Clicked, value);
		}
	}

	public override void _Ready()
	{
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		ConnectSignals();
		_titleLabel = ((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%Label"));
		_labelContainer = ((Node)_titleLabel).GetParent<MarginContainer>();
		_cardImage = ((Node)this).GetNode<NTinyCard>(NodePath.op_Implicit("%Card"));
		_enchantmentImage = ((Node)this).GetNode<TextureRect>(NodePath.op_Implicit("%Enchantment"));
		((Control)_cardImage).PivotOffset = ((Control)_cardImage).Size * 0.5f;
		Reload();
	}

	public static NDeckHistoryEntry Create(CardModel card, int amount)
	{
		return Create(card, amount, Array.Empty<int>());
	}

	public static NDeckHistoryEntry Create(CardModel card, int amount, IEnumerable<int> floorsAdded)
	{
		NDeckHistoryEntry nDeckHistoryEntry = PreloadManager.Cache.GetScene(ScenePath).Instantiate<NDeckHistoryEntry>((GenEditState)0);
		nDeckHistoryEntry.Card = card;
		nDeckHistoryEntry._amount = amount;
		nDeckHistoryEntry.FloorsAddedToDeck = floorsAdded;
		return nDeckHistoryEntry;
	}

	private void Reload()
	{
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		_titleLabel.SetTextAutoSize(Card.Title);
		bool flag = Card.CurrentUpgradeLevel >= 1;
		bool flag2 = Card.Enchantment != null;
		string text = Card.Title;
		if (_amount > 1)
		{
			text = $"{_amount}x {text}";
		}
		_titleLabel.SetTextAutoSize(text);
		if (flag2)
		{
			((Control)_titleLabel).AddThemeColorOverride(ThemeConstants.Label.fontColor, StsColors.purple);
		}
		else if (flag)
		{
			((Control)_titleLabel).AddThemeColorOverride(ThemeConstants.Label.fontColor, StsColors.green);
		}
		_cardImage.SetCard(Card);
		if (Card.Enchantment != null)
		{
			_enchantmentImage.Texture = (Texture2D)(object)Card.Enchantment.Icon;
		}
		((CanvasItem)_enchantmentImage).Visible = Card.Enchantment != null;
		((Control)this).Size = new Vector2(((Control)_cardImage).Size.X + ((Control)_titleLabel).Size.X + 10f, ((Control)this).Size.Y);
	}

	protected override void OnFocus()
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		_scaleTween?.FastForwardToCompletion();
		_scaleTween = ((Node)this).CreateTween().SetParallel(true);
		_scaleTween.TweenProperty((GodotObject)(object)_cardImage, NodePath.op_Implicit("scale"), Variant.op_Implicit(Vector2.One * 1.5f), 0.05);
		_scaleTween.TweenProperty((GodotObject)(object)_labelContainer, NodePath.op_Implicit("position:x"), Variant.op_Implicit(((Control)_labelContainer).Position.X + 8f), 0.05);
	}

	protected override void OnUnfocus()
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		_scaleTween?.FastForwardToCompletion();
		_scaleTween = ((Node)this).CreateTween().SetParallel(true);
		_scaleTween.TweenProperty((GodotObject)(object)_cardImage, NodePath.op_Implicit("scale"), Variant.op_Implicit(Vector2.One), 0.5).SetTrans((TransitionType)5).SetEase((EaseType)1);
		_scaleTween.TweenProperty((GodotObject)(object)_labelContainer, NodePath.op_Implicit("position:x"), Variant.op_Implicit(((Control)_labelContainer).Position.X - 8f), 0.5).SetTrans((TransitionType)5).SetEase((EaseType)1);
	}

	protected override void OnRelease()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).EmitSignal(SignalName.Clicked, (Variant[])(object)new Variant[1] { Variant.op_Implicit((GodotObject)(object)this) });
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
		List<MethodInfo> list = new List<MethodInfo>(5);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.Reload, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnFocus, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnUnfocus, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnRelease, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
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
		if ((ref method) == MethodName.Reload)
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
		if ((ref method) == MethodName.OnRelease)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._titleLabel)
		{
			_titleLabel = VariantUtils.ConvertTo<MegaLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._cardImage)
		{
			_cardImage = VariantUtils.ConvertTo<NTinyCard>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._enchantmentImage)
		{
			_enchantmentImage = VariantUtils.ConvertTo<TextureRect>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._labelContainer)
		{
			_labelContainer = VariantUtils.ConvertTo<MarginContainer>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._scaleTween)
		{
			_scaleTween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._amount)
		{
			_amount = VariantUtils.ConvertTo<int>(ref value);
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
		if ((ref name) == PropertyName._titleLabel)
		{
			value = VariantUtils.CreateFrom<MegaLabel>(ref _titleLabel);
			return true;
		}
		if ((ref name) == PropertyName._cardImage)
		{
			value = VariantUtils.CreateFrom<NTinyCard>(ref _cardImage);
			return true;
		}
		if ((ref name) == PropertyName._enchantmentImage)
		{
			value = VariantUtils.CreateFrom<TextureRect>(ref _enchantmentImage);
			return true;
		}
		if ((ref name) == PropertyName._labelContainer)
		{
			value = VariantUtils.CreateFrom<MarginContainer>(ref _labelContainer);
			return true;
		}
		if ((ref name) == PropertyName._scaleTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _scaleTween);
			return true;
		}
		if ((ref name) == PropertyName._amount)
		{
			value = VariantUtils.CreateFrom<int>(ref _amount);
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
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._titleLabel, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._cardImage, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._enchantmentImage, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._labelContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._scaleTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)2, PropertyName._amount, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		info.AddProperty(PropertyName._titleLabel, Variant.From<MegaLabel>(ref _titleLabel));
		info.AddProperty(PropertyName._cardImage, Variant.From<NTinyCard>(ref _cardImage));
		info.AddProperty(PropertyName._enchantmentImage, Variant.From<TextureRect>(ref _enchantmentImage));
		info.AddProperty(PropertyName._labelContainer, Variant.From<MarginContainer>(ref _labelContainer));
		info.AddProperty(PropertyName._scaleTween, Variant.From<Tween>(ref _scaleTween));
		info.AddProperty(PropertyName._amount, Variant.From<int>(ref _amount));
		info.AddSignalEventDelegate(SignalName.Clicked, (Delegate)backing_Clicked);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._titleLabel, ref val))
		{
			_titleLabel = ((Variant)(ref val)).As<MegaLabel>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._cardImage, ref val2))
		{
			_cardImage = ((Variant)(ref val2)).As<NTinyCard>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._enchantmentImage, ref val3))
		{
			_enchantmentImage = ((Variant)(ref val3)).As<TextureRect>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._labelContainer, ref val4))
		{
			_labelContainer = ((Variant)(ref val4)).As<MarginContainer>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._scaleTween, ref val5))
		{
			_scaleTween = ((Variant)(ref val5)).As<Tween>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._amount, ref val6))
		{
			_amount = ((Variant)(ref val6)).As<int>();
		}
		ClickedEventHandler clickedEventHandler = default(ClickedEventHandler);
		if (info.TryGetSignalEventDelegate<ClickedEventHandler>(SignalName.Clicked, ref clickedEventHandler))
		{
			backing_Clicked = clickedEventHandler;
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
		List<MethodInfo> list = new List<MethodInfo>(1);
		list.Add(new MethodInfo(SignalName.Clicked, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("entry"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		return list;
	}

	protected void EmitSignalClicked(NDeckHistoryEntry entry)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).EmitSignal(SignalName.Clicked, (Variant[])(object)new Variant[1] { Variant.op_Implicit((GodotObject)(object)entry) });
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RaiseGodotClassSignalCallbacks(in godot_string_name signal, NativeVariantPtrArgs args)
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		if ((ref signal) == SignalName.Clicked && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			backing_Clicked?.Invoke(VariantUtils.ConvertTo<NDeckHistoryEntry>(ref ((NativeVariantPtrArgs)(ref args))[0]));
		}
		else
		{
			base.RaiseGodotClassSignalCallbacks(in signal, args);
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassSignal(in godot_string_name signal)
	{
		if ((ref signal) == SignalName.Clicked)
		{
			return true;
		}
		return base.HasGodotClassSignal(in signal);
	}
}
