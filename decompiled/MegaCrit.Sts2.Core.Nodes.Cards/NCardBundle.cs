using System;
using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.TestSupport;

namespace MegaCrit.Sts2.Core.Nodes.Cards;

[ScriptPath("res://src/Core/Nodes/Cards/NCardBundle.cs")]
public class NCardBundle : Control
{
	[Signal]
	public delegate void ClickedEventHandler(NCardBundle cardHolder);

	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName ReAddCardNodes = StringName.op_Implicit("ReAddCardNodes");

		public static readonly StringName OnClicked = StringName.op_Implicit("OnClicked");

		public static readonly StringName OnFocused = StringName.op_Implicit("OnFocused");

		public static readonly StringName OnUnfocused = StringName.op_Implicit("OnUnfocused");

		public static readonly StringName _ExitTree = StringName.op_Implicit("_ExitTree");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName Hitbox = StringName.op_Implicit("Hitbox");

		public static readonly StringName _hoverScale = StringName.op_Implicit("_hoverScale");

		public static readonly StringName smallScale = StringName.op_Implicit("smallScale");

		public static readonly StringName _cardHolder = StringName.op_Implicit("_cardHolder");

		public static readonly StringName _hoverTween = StringName.op_Implicit("_hoverTween");

		public static readonly StringName _cardTween = StringName.op_Implicit("_cardTween");
	}

	public class SignalName : SignalName
	{
		public static readonly StringName Clicked = StringName.op_Implicit("Clicked");
	}

	private const float _cardSeparation = 45f;

	private readonly Vector2 _hoverScale = Vector2.One * 0.85f;

	public readonly Vector2 smallScale = Vector2.One * 0.8f;

	private Control _cardHolder;

	private readonly List<NCard> _cardNodes = new List<NCard>();

	private Tween? _hoverTween;

	private Tween? _cardTween;

	private ClickedEventHandler backing_Clicked;

	public NClickableControl Hitbox { get; private set; }

	public IReadOnlyList<CardModel> Bundle { get; private set; }

	public IReadOnlyList<NCard> CardNodes => _cardNodes;

	private static string ScenePath => SceneHelper.GetScenePath("/cards/card_bundle");

	public static IEnumerable<string> AssetPaths => new _003C_003Ez__ReadOnlySingleElementList<string>(ScenePath);

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

	public static NCardBundle? Create(IReadOnlyList<CardModel> bundle)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		if (TestMode.IsOn)
		{
			return null;
		}
		NCardBundle nCardBundle = PreloadManager.Cache.GetScene(ScenePath).Instantiate<NCardBundle>((GenEditState)0);
		((Node)nCardBundle).Name = StringName.op_Implicit("NCardBundle");
		((Control)nCardBundle).Scale = nCardBundle.smallScale;
		nCardBundle.Bundle = bundle;
		return nCardBundle;
	}

	public override void _Ready()
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		Hitbox = ((Node)this).GetNode<NButton>(NodePath.op_Implicit("%Hitbox"));
		_cardHolder = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%Cards"));
		((GodotObject)Hitbox).Connect(NClickableControl.SignalName.Focused, Callable.From<NClickableControl>((Action<NClickableControl>)OnFocused), 0u);
		((GodotObject)Hitbox).Connect(NClickableControl.SignalName.Unfocused, Callable.From<NClickableControl>((Action<NClickableControl>)OnUnfocused), 0u);
		((GodotObject)Hitbox).Connect(NClickableControl.SignalName.Released, Callable.From<NClickableControl>((Action<NClickableControl>)OnClicked), 0u);
		for (int i = 0; i < Bundle.Count; i++)
		{
			NCard nCard = NCard.Create(Bundle[i]);
			_cardHolder = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%Cards"));
			((Node)(object)_cardHolder).AddChildSafely((Node?)(object)nCard);
			nCard.UpdateVisuals(PileType.None, CardPreviewMode.Normal);
			((Control)nCard).Position = ((Control)nCard).Position + new Vector2(-1f, 1f) * 45f * ((float)i - (float)Bundle.Count / 2f);
			float num = 0.5f + (float)i / (float)(Bundle.Count - 1) * 0.5f;
			((CanvasItem)nCard).Modulate = new Color(num, num, num, 1f);
			_cardNodes.Add(nCard);
		}
	}

	public IReadOnlyList<NCard> RemoveCardNodes()
	{
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		Tween? cardTween = _cardTween;
		if (cardTween != null)
		{
			cardTween.Kill();
		}
		_cardTween = ((Node)this).CreateTween().SetParallel(true);
		foreach (NCard cardNode in _cardNodes)
		{
			_cardTween.TweenProperty((GodotObject)(object)cardNode, NodePath.op_Implicit("modulate"), Variant.op_Implicit(Colors.White), 0.15000000596046448);
		}
		return CardNodes;
	}

	public void ReAddCardNodes()
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		Tween? cardTween = _cardTween;
		if (cardTween != null)
		{
			cardTween.Kill();
		}
		_cardTween = ((Node)this).CreateTween().SetParallel(true);
		for (int i = 0; i < _cardNodes.Count; i++)
		{
			NCard nCard = _cardNodes[i];
			Vector2 globalPosition = ((Control)nCard).GlobalPosition;
			((Node)nCard).GetParent()?.RemoveChildSafely((Node?)(object)nCard);
			_cardHolder = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%Cards"));
			((Node)(object)_cardHolder).AddChildSafely((Node?)(object)nCard);
			((Control)nCard).GlobalPosition = globalPosition;
			nCard.UpdateVisuals(PileType.None, CardPreviewMode.Normal);
			_cardTween.TweenProperty((GodotObject)(object)nCard, NodePath.op_Implicit("position"), Variant.op_Implicit(new Vector2(-1f, 1f) * 45f * ((float)i - (float)_cardNodes.Count / 2f)), 0.4000000059604645).SetEase((EaseType)1).SetTrans((TransitionType)5);
			float num = 0.5f + (float)i / (float)(_cardNodes.Count - 1) * 0.5f;
			_cardTween.TweenProperty((GodotObject)(object)nCard, NodePath.op_Implicit("modulate"), Variant.op_Implicit(new Color(num, num, num, 1f)), 0.4000000059604645);
		}
	}

	private void OnClicked(NClickableControl _)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).EmitSignal(SignalName.Clicked, (Variant[])(object)new Variant[1] { Variant.op_Implicit((GodotObject)(object)this) });
	}

	private void OnFocused(NClickableControl _)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		Tween? hoverTween = _hoverTween;
		if (hoverTween != null)
		{
			hoverTween.Kill();
		}
		((Control)this).Scale = _hoverScale;
	}

	private void OnUnfocused(NClickableControl _)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		Tween? hoverTween = _hoverTween;
		if (hoverTween != null)
		{
			hoverTween.Kill();
		}
		_hoverTween = ((Node)this).CreateTween();
		_hoverTween.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("scale"), Variant.op_Implicit(smallScale), 0.5).SetEase((EaseType)1).SetTrans((TransitionType)5);
	}

	public override void _ExitTree()
	{
		foreach (NCard cardNode in _cardNodes)
		{
			if (((Node)this).IsAncestorOf((Node)(object)cardNode))
			{
				((Node)(object)cardNode).QueueFreeSafely();
			}
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Expected O, but got Unknown
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Expected O, but got Unknown
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Expected O, but got Unknown
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(6);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ReAddCardNodes, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnClicked, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnFocused, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnUnfocused, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._ExitTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.ReAddCardNodes && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			ReAddCardNodes();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnClicked && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OnClicked(VariantUtils.ConvertTo<NClickableControl>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnFocused && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OnFocused(VariantUtils.ConvertTo<NClickableControl>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnUnfocused && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OnUnfocused(VariantUtils.ConvertTo<NClickableControl>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._ExitTree && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._ExitTree();
			ret = default(godot_variant);
			return true;
		}
		return ((Control)this).InvokeGodotClassMethod(ref method, args, ref ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if ((ref method) == MethodName._Ready)
		{
			return true;
		}
		if ((ref method) == MethodName.ReAddCardNodes)
		{
			return true;
		}
		if ((ref method) == MethodName.OnClicked)
		{
			return true;
		}
		if ((ref method) == MethodName.OnFocused)
		{
			return true;
		}
		if ((ref method) == MethodName.OnUnfocused)
		{
			return true;
		}
		if ((ref method) == MethodName._ExitTree)
		{
			return true;
		}
		return ((Control)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName.Hitbox)
		{
			Hitbox = VariantUtils.ConvertTo<NClickableControl>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._cardHolder)
		{
			_cardHolder = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._hoverTween)
		{
			_hoverTween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._cardTween)
		{
			_cardTween = VariantUtils.ConvertTo<Tween>(ref value);
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
		if ((ref name) == PropertyName.Hitbox)
		{
			NClickableControl hitbox = Hitbox;
			value = VariantUtils.CreateFrom<NClickableControl>(ref hitbox);
			return true;
		}
		if ((ref name) == PropertyName._hoverScale)
		{
			value = VariantUtils.CreateFrom<Vector2>(ref _hoverScale);
			return true;
		}
		if ((ref name) == PropertyName.smallScale)
		{
			value = VariantUtils.CreateFrom<Vector2>(ref smallScale);
			return true;
		}
		if ((ref name) == PropertyName._cardHolder)
		{
			value = VariantUtils.CreateFrom<Control>(ref _cardHolder);
			return true;
		}
		if ((ref name) == PropertyName._hoverTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _hoverTween);
			return true;
		}
		if ((ref name) == PropertyName._cardTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _cardTween);
			return true;
		}
		return ((GodotObject)this).GetGodotClassPropertyValue(ref name, ref value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)5, PropertyName._hoverScale, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)5, PropertyName.smallScale, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.Hitbox, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._cardHolder, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._hoverTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._cardTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		StringName hitbox = PropertyName.Hitbox;
		NClickableControl hitbox2 = Hitbox;
		info.AddProperty(hitbox, Variant.From<NClickableControl>(ref hitbox2));
		info.AddProperty(PropertyName._cardHolder, Variant.From<Control>(ref _cardHolder));
		info.AddProperty(PropertyName._hoverTween, Variant.From<Tween>(ref _hoverTween));
		info.AddProperty(PropertyName._cardTween, Variant.From<Tween>(ref _cardTween));
		info.AddSignalEventDelegate(SignalName.Clicked, (Delegate)backing_Clicked);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName.Hitbox, ref val))
		{
			Hitbox = ((Variant)(ref val)).As<NClickableControl>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._cardHolder, ref val2))
		{
			_cardHolder = ((Variant)(ref val2)).As<Control>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._hoverTween, ref val3))
		{
			_hoverTween = ((Variant)(ref val3)).As<Tween>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._cardTween, ref val4))
		{
			_cardTween = ((Variant)(ref val4)).As<Tween>();
		}
		ClickedEventHandler clickedEventHandler = default(ClickedEventHandler);
		if (info.TryGetSignalEventDelegate<ClickedEventHandler>(SignalName.Clicked, ref clickedEventHandler))
		{
			backing_Clicked = clickedEventHandler;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotSignalList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Expected O, but got Unknown
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(1);
		list.Add(new MethodInfo(SignalName.Clicked, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("cardHolder"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		return list;
	}

	protected void EmitSignalClicked(NCardBundle cardHolder)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).EmitSignal(SignalName.Clicked, (Variant[])(object)new Variant[1] { Variant.op_Implicit((GodotObject)(object)cardHolder) });
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RaiseGodotClassSignalCallbacks(in godot_string_name signal, NativeVariantPtrArgs args)
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		if ((ref signal) == SignalName.Clicked && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			backing_Clicked?.Invoke(VariantUtils.ConvertTo<NCardBundle>(ref ((NativeVariantPtrArgs)(ref args))[0]));
		}
		else
		{
			((GodotObject)this).RaiseGodotClassSignalCallbacks(ref signal, args);
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassSignal(in godot_string_name signal)
	{
		if ((ref signal) == SignalName.Clicked)
		{
			return true;
		}
		return ((Control)this).HasGodotClassSignal(ref signal);
	}
}
