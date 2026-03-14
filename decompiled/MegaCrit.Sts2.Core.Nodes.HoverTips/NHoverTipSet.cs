using System;
using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards.Holders;
using MegaCrit.Sts2.Core.Nodes.Relics;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.Nodes.HoverTips;

[ScriptPath("res://src/Core/Nodes/HoverTips/NHoverTipSet.cs")]
public class NHoverTipSet : Control
{
	public class MethodName : MethodName
	{
		public static readonly StringName SetFollowOwner = StringName.op_Implicit("SetFollowOwner");

		public static readonly StringName CreateAndShowMapPointHistory = StringName.op_Implicit("CreateAndShowMapPointHistory");

		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName _Process = StringName.op_Implicit("_Process");

		public static readonly StringName SetAlignment = StringName.op_Implicit("SetAlignment");

		public static readonly StringName SetAlignmentForRelic = StringName.op_Implicit("SetAlignmentForRelic");

		public static readonly StringName SetAlignmentForCardHolder = StringName.op_Implicit("SetAlignmentForCardHolder");

		public static readonly StringName CorrectVerticalOverflow = StringName.op_Implicit("CorrectVerticalOverflow");

		public static readonly StringName CorrectHorizontalOverflow = StringName.op_Implicit("CorrectHorizontalOverflow");

		public static readonly StringName Clear = StringName.op_Implicit("Clear");

		public static readonly StringName Remove = StringName.op_Implicit("Remove");

		public static readonly StringName SetExtraFollowOffset = StringName.op_Implicit("SetExtraFollowOffset");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName TextHoverTipDimensions = StringName.op_Implicit("TextHoverTipDimensions");

		public static readonly StringName CardHoverTipDimensions = StringName.op_Implicit("CardHoverTipDimensions");

		public static readonly StringName _textHoverTipContainer = StringName.op_Implicit("_textHoverTipContainer");

		public static readonly StringName _cardHoverTipContainer = StringName.op_Implicit("_cardHoverTipContainer");

		public static readonly StringName _owner = StringName.op_Implicit("_owner");

		public static readonly StringName _followOwner = StringName.op_Implicit("_followOwner");

		public static readonly StringName _followOffset = StringName.op_Implicit("_followOffset");

		public static readonly StringName _extraOffset = StringName.op_Implicit("_extraOffset");
	}

	public class SignalName : SignalName
	{
	}

	public static bool shouldBlockHoverTips = false;

	private static readonly StringName _cardHoverTipContainerStr = new StringName("cardHoverTipContainer");

	private static readonly StringName _textHoverTipContainerStr = new StringName("textHoverTipContainer");

	private const float _hoverTipSpacing = 5f;

	private const float _hoverTipWidth = 360f;

	private const string _tipScenePath = "res://scenes/ui/hover_tip.tscn";

	private const string _tipSetScenePath = "res://scenes/ui/hover_tip_set.tscn";

	private const string _debuffMatPath = "res://materials/ui/hover_tip_debuff.tres";

	private static readonly Dictionary<Control, NHoverTipSet> _activeHoverTips = new Dictionary<Control, NHoverTipSet>();

	private VFlowContainer _textHoverTipContainer;

	private NHoverTipCardContainer _cardHoverTipContainer;

	private Control _owner;

	private bool _followOwner;

	private Vector2 _followOffset;

	private Vector2 _extraOffset = Vector2.Zero;

	private static Node HoverTipsContainer => NGame.Instance.HoverTipsContainer;

	private Vector2 TextHoverTipDimensions => ((Control)_textHoverTipContainer).Size;

	private Vector2 CardHoverTipDimensions => ((Control)_cardHoverTipContainer).Size;

	public static IEnumerable<string> AssetPaths => new global::_003C_003Ez__ReadOnlyArray<string>(new string[3] { "res://scenes/ui/hover_tip.tscn", "res://scenes/ui/hover_tip_set.tscn", "res://materials/ui/hover_tip_debuff.tres" });

	public void SetFollowOwner()
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		_followOwner = true;
		_followOffset = _owner.GlobalPosition - ((Control)this).GlobalPosition;
	}

	public static NHoverTipSet CreateAndShow(Control owner, IHoverTip hoverTip, HoverTipAlignment alignment = HoverTipAlignment.None)
	{
		return CreateAndShow(owner, new _003C_003Ez__ReadOnlySingleElementList<IHoverTip>(hoverTip), alignment);
	}

	public static NHoverTipSet CreateAndShow(Control owner, IEnumerable<IHoverTip> hoverTips, HoverTipAlignment alignment = HoverTipAlignment.None)
	{
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		NHoverTipSet nHoverTipSet = PreloadManager.Cache.GetScene("res://scenes/ui/hover_tip_set.tscn").Instantiate<NHoverTipSet>((GenEditState)0);
		HoverTipsContainer.AddChildSafely((Node?)(object)nHoverTipSet);
		if (shouldBlockHoverTips)
		{
			return nHoverTipSet;
		}
		_activeHoverTips.Add(owner, nHoverTipSet);
		nHoverTipSet.Init(owner, hoverTips);
		if (NGame.IsDebugHidingHoverTips)
		{
			((CanvasItem)nHoverTipSet).Visible = false;
		}
		((GodotObject)owner).Connect(SignalName.TreeExiting, Callable.From((Action)delegate
		{
			Remove(owner);
		}), 0u);
		nHoverTipSet.SetAlignment(owner, alignment);
		return nHoverTipSet;
	}

	public static NHoverTipSet CreateAndShowMapPointHistory(Control owner, NMapPointHistoryHoverTip historyHoverTip)
	{
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		NHoverTipSet nHoverTipSet = PreloadManager.Cache.GetScene("res://scenes/ui/hover_tip_set.tscn").Instantiate<NHoverTipSet>((GenEditState)0);
		nHoverTipSet._owner = owner;
		HoverTipsContainer.AddChildSafely((Node?)(object)nHoverTipSet);
		_activeHoverTips.Add(owner, nHoverTipSet);
		((Node)(object)nHoverTipSet._textHoverTipContainer).AddChildSafely((Node?)(object)historyHoverTip);
		if (NGame.IsDebugHidingHoverTips)
		{
			((CanvasItem)nHoverTipSet).Visible = false;
		}
		((GodotObject)owner).Connect(SignalName.TreeExiting, Callable.From((Action)delegate
		{
			Remove(owner);
		}), 0u);
		return nHoverTipSet;
	}

	public override void _Ready()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown
		_textHoverTipContainer = new VFlowContainer();
		((Node)_textHoverTipContainer).Name = _textHoverTipContainerStr;
		((Control)_textHoverTipContainer).MouseFilter = (MouseFilterEnum)2;
		((Node)(object)this).AddChildSafely((Node?)(object)_textHoverTipContainer);
		_cardHoverTipContainer = new NHoverTipCardContainer();
		((Node)_cardHoverTipContainer).Name = _cardHoverTipContainerStr;
		((Control)_cardHoverTipContainer).MouseFilter = (MouseFilterEnum)2;
		((Node)(object)this).AddChildSafely((Node?)(object)_cardHoverTipContainer);
	}

	public override void _Process(double delta)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		if (_followOwner && _owner != null)
		{
			((Control)this).GlobalPosition = _owner.GlobalPosition - _followOffset + _extraOffset;
		}
	}

	private void Init(Control owner, IEnumerable<IHoverTip> hoverTips)
	{
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		_owner = owner;
		foreach (IHoverTip item in IHoverTip.RemoveDupes(hoverTips))
		{
			if (item is HoverTip hoverTip)
			{
				Control val = PreloadManager.Cache.GetScene("res://scenes/ui/hover_tip.tscn").Instantiate<Control>((GenEditState)0);
				((Node)(object)_textHoverTipContainer).AddChildSafely((Node?)(object)val);
				MegaLabel node = ((Node)val).GetNode<MegaLabel>(NodePath.op_Implicit("%Title"));
				if (hoverTip.Title == null)
				{
					((CanvasItem)node).Visible = false;
				}
				else
				{
					node.SetTextAutoSize(hoverTip.Title);
				}
				((Node)val).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("%Description")).Text = hoverTip.Description;
				((RichTextLabel)((Node)val).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("%Description"))).AutowrapMode = (AutowrapMode)(hoverTip.ShouldOverrideTextOverflow ? 0 : 3);
				((Node)val).GetNode<TextureRect>(NodePath.op_Implicit("%Icon")).Texture = hoverTip.Icon;
				if (hoverTip.IsDebuff)
				{
					((Node)val).GetNode<CanvasItem>(NodePath.op_Implicit("%Bg")).Material = PreloadManager.Cache.GetMaterial("res://materials/ui/hover_tip_debuff.tres");
				}
				val.ResetSize();
				float num = ((Control)_textHoverTipContainer).Size.Y + val.Size.Y + 5f;
				Rect2 viewportRect = ((CanvasItem)NGame.Instance).GetViewportRect();
				if (num < ((Rect2)(ref viewportRect)).Size.Y - 50f)
				{
					((Control)_textHoverTipContainer).Size = new Vector2(360f, ((Control)_textHoverTipContainer).Size.Y + val.Size.Y + 5f);
				}
				else
				{
					((FlowContainer)_textHoverTipContainer).Alignment = (AlignmentMode)1;
				}
			}
			else
			{
				_cardHoverTipContainer.Add((CardHoverTip)item);
			}
			AbstractModel canonicalModel = item.CanonicalModel;
			if (!(canonicalModel is CardModel card))
			{
				if (!(canonicalModel is RelicModel relic))
				{
					if (canonicalModel is PotionModel potion)
					{
						SaveManager.Instance.MarkPotionAsSeen(potion);
					}
				}
				else
				{
					SaveManager.Instance.MarkRelicAsSeen(relic);
				}
			}
			else
			{
				SaveManager.Instance.MarkCardAsSeen(card);
			}
		}
	}

	public void SetAlignment(Control node, HoverTipAlignment alignment)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		if (alignment != HoverTipAlignment.None)
		{
			((Control)_textHoverTipContainer).Position = Vector2.Zero;
			switch (alignment)
			{
			case HoverTipAlignment.Left:
			{
				((Control)_textHoverTipContainer).GlobalPosition = node.GlobalPosition;
				VFlowContainer textHoverTipContainer = _textHoverTipContainer;
				((Control)textHoverTipContainer).Position = ((Control)textHoverTipContainer).Position + Vector2.Left * ((Control)_textHoverTipContainer).Size.X;
				((FlowContainer)_textHoverTipContainer).ReverseFill = true;
				_cardHoverTipContainer.LayoutResizeAndReposition(node.GlobalPosition + new Vector2(node.Size.X, 0f) * node.Scale, HoverTipAlignment.Right);
				break;
			}
			case HoverTipAlignment.Right:
				_cardHoverTipContainer.LayoutResizeAndReposition(node.GlobalPosition, HoverTipAlignment.Left);
				((Control)_textHoverTipContainer).GlobalPosition = node.GlobalPosition + new Vector2(node.Size.X, 0f) * node.Scale;
				break;
			case HoverTipAlignment.Center:
			{
				((Control)this).GlobalPosition = node.GlobalPosition + Vector2.Down * node.Size.Y * 1.5f;
				NHoverTipCardContainer cardHoverTipContainer = _cardHoverTipContainer;
				((Control)cardHoverTipContainer).GlobalPosition = ((Control)cardHoverTipContainer).GlobalPosition + Vector2.Down * ((Control)_textHoverTipContainer).Size.Y;
				_cardHoverTipContainer.LayoutResizeAndReposition(((Control)_cardHoverTipContainer).GlobalPosition, alignment);
				break;
			}
			}
		}
		CorrectVerticalOverflow();
		CorrectHorizontalOverflow();
	}

	public void SetAlignmentForRelic(NRelic relic)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		HoverTipAlignment hoverTipAlignment = HoverTip.GetHoverTipAlignment((Control)(object)relic);
		Vector2 size = ((Control)relic.Icon).Size;
		Transform2D globalTransform = ((CanvasItem)relic).GetGlobalTransform();
		Vector2 val = size * ((Transform2D)(ref globalTransform)).Scale;
		((Control)_textHoverTipContainer).GlobalPosition = ((Control)relic).GlobalPosition + Vector2.Down * (val.Y + 10f);
		if (hoverTipAlignment == HoverTipAlignment.Left)
		{
			VFlowContainer textHoverTipContainer = _textHoverTipContainer;
			((Control)textHoverTipContainer).Position = ((Control)textHoverTipContainer).Position + Vector2.Left * (((Control)_textHoverTipContainer).Size.X - val.X);
		}
		_cardHoverTipContainer.LayoutResizeAndReposition(((Control)_textHoverTipContainer).GlobalPosition + Vector2.Down * ((Control)_textHoverTipContainer).Size.Y, hoverTipAlignment);
		if (hoverTipAlignment == HoverTipAlignment.Left)
		{
			((Control)_cardHoverTipContainer).GlobalPosition = new Vector2(((Control)_textHoverTipContainer).GlobalPosition.X, ((Control)_cardHoverTipContainer).GlobalPosition.Y);
		}
		Rect2 val2 = ((CanvasItem)NGame.Instance).GetViewportRect();
		float y = ((Rect2)(ref val2)).Size.Y;
		if (((Control)relic).GlobalPosition.Y > y * 0.75f)
		{
			((Control)_textHoverTipContainer).GlobalPosition = ((Control)relic).GlobalPosition + Vector2.Up * ((Control)_textHoverTipContainer).Size.Y;
		}
		CorrectVerticalOverflow();
		CorrectHorizontalOverflow();
		val2 = ((Control)_textHoverTipContainer).GetRect();
		if (((Rect2)(ref val2)).Intersects(((Control)_cardHoverTipContainer).GetRect(), false))
		{
			if (hoverTipAlignment == HoverTipAlignment.Left)
			{
				((Control)_cardHoverTipContainer).GlobalPosition = ((Control)_textHoverTipContainer).GlobalPosition + ((Control)_textHoverTipContainer).Size.X * Vector2.Right;
			}
			else
			{
				((Control)_cardHoverTipContainer).GlobalPosition = ((Control)_textHoverTipContainer).GlobalPosition + ((Control)_cardHoverTipContainer).Size.X * Vector2.Left;
			}
			CorrectVerticalOverflow();
		}
	}

	public void SetAlignmentForCardHolder(NCardHolder holder)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		HoverTipAlignment hoverTipAlignment = HoverTip.GetHoverTipAlignment((Control)(object)holder);
		((Control)_textHoverTipContainer).Position = Vector2.Zero;
		Control hitbox = (Control)(object)holder.Hitbox;
		if (hoverTipAlignment == HoverTipAlignment.Left)
		{
			((Control)_textHoverTipContainer).GlobalPosition = hitbox.GlobalPosition;
			VFlowContainer textHoverTipContainer = _textHoverTipContainer;
			((Control)textHoverTipContainer).Position = ((Control)textHoverTipContainer).Position + (Vector2.Left * ((Control)_textHoverTipContainer).Size.X - new Vector2(10f, 0f));
			((FlowContainer)_textHoverTipContainer).ReverseFill = true;
			_cardHoverTipContainer.LayoutResizeAndReposition(hitbox.GlobalPosition + new Vector2(hitbox.Size.X, 0f) * hitbox.Scale, HoverTipAlignment.Right);
		}
		else
		{
			Vector2 val = hitbox.GlobalPosition;
			if (holder.CardModel != null && (holder.CardModel.CurrentStarCost > 0 || holder.CardModel.HasStarCostX))
			{
				val += Vector2.Left * 15f;
			}
			_cardHoverTipContainer.LayoutResizeAndReposition(val, HoverTipAlignment.Left);
			((Control)_textHoverTipContainer).GlobalPosition = hitbox.GlobalPosition + new Vector2(hitbox.Size.X + 10f, 0f) * hitbox.Scale * ((Control)holder).Scale;
		}
		CorrectVerticalOverflow();
		CorrectHorizontalOverflow();
		SetFollowOwner();
	}

	private void CorrectVerticalOverflow()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		Rect2 viewportRect = ((CanvasItem)NGame.Instance).GetViewportRect();
		float y = ((Rect2)(ref viewportRect)).Size.Y;
		if (((Control)_textHoverTipContainer).GlobalPosition.Y + ((Control)_textHoverTipContainer).Size.Y > y)
		{
			((Control)_textHoverTipContainer).GlobalPosition = new Vector2(((Control)_textHoverTipContainer).GlobalPosition.X, y - ((Control)_textHoverTipContainer).Size.Y);
		}
		if (((Control)_cardHoverTipContainer).GlobalPosition.Y + ((Control)_cardHoverTipContainer).Size.Y > y)
		{
			((Control)_cardHoverTipContainer).GlobalPosition = new Vector2(((Control)_cardHoverTipContainer).GlobalPosition.X, y - ((Control)_cardHoverTipContainer).Size.Y);
		}
	}

	private void CorrectHorizontalOverflow()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		Rect2 viewportRect = ((CanvasItem)NGame.Instance).GetViewportRect();
		float x = ((Rect2)(ref viewportRect)).Size.X;
		Vector2 globalPosition = ((Control)_cardHoverTipContainer).GlobalPosition;
		float x2 = ((Control)_cardHoverTipContainer).Size.X;
		Vector2 globalPosition2 = ((Control)_textHoverTipContainer).GlobalPosition;
		float x3 = ((Control)_textHoverTipContainer).Size.X;
		if (globalPosition.X + x2 <= x && globalPosition2.X + x3 > x)
		{
			float num = globalPosition.X - x3;
			((Control)_textHoverTipContainer).GlobalPosition = new Vector2(num, globalPosition.Y);
		}
		else if (globalPosition.X + x2 > x || globalPosition2.X + x3 > x)
		{
			float num2 = globalPosition2.X + x3 - x2;
			((Control)_cardHoverTipContainer).GlobalPosition = new Vector2(num2, globalPosition.Y);
			VFlowContainer textHoverTipContainer = _textHoverTipContainer;
			((Control)textHoverTipContainer).GlobalPosition = ((Control)textHoverTipContainer).GlobalPosition + Vector2.Left * x2;
		}
		else if (globalPosition.X < 0f || globalPosition2.X < 0f)
		{
			float x4 = globalPosition2.X;
			((Control)_cardHoverTipContainer).GlobalPosition = new Vector2(x4, globalPosition.Y);
			VFlowContainer textHoverTipContainer2 = _textHoverTipContainer;
			((Control)textHoverTipContainer2).GlobalPosition = ((Control)textHoverTipContainer2).GlobalPosition + Vector2.Right * x2;
		}
	}

	public static void Clear()
	{
		foreach (Control key in _activeHoverTips.Keys)
		{
			Remove(key);
		}
	}

	public static void Remove(Control owner)
	{
		if (_activeHoverTips.TryGetValue(owner, out NHoverTipSet value))
		{
			((Node)(object)value).QueueFreeSafely();
			_activeHoverTips.Remove(owner);
		}
	}

	public void SetExtraFollowOffset(Vector2 offset)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		_extraOffset = offset;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Expected O, but got Unknown
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Expected O, but got Unknown
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Expected O, but got Unknown
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Expected O, but got Unknown
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Expected O, but got Unknown
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Expected O, but got Unknown
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02da: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0309: Unknown result type (might be due to invalid IL or missing references)
		//IL_0313: Unknown result type (might be due to invalid IL or missing references)
		//IL_0339: Unknown result type (might be due to invalid IL or missing references)
		//IL_0362: Unknown result type (might be due to invalid IL or missing references)
		//IL_036d: Expected O, but got Unknown
		//IL_0368: Unknown result type (might be due to invalid IL or missing references)
		//IL_0373: Unknown result type (might be due to invalid IL or missing references)
		//IL_0399: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c7: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(12);
		list.Add(new MethodInfo(MethodName.SetFollowOwner, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.CreateAndShowMapPointHistory, new PropertyInfo((Type)24, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false), (MethodFlags)33, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("owner"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false),
			new PropertyInfo((Type)24, StringName.op_Implicit("historyHoverTip"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("MarginContainer"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Process, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)3, StringName.op_Implicit("delta"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetAlignment, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("node"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false),
			new PropertyInfo((Type)2, StringName.op_Implicit("alignment"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetAlignmentForRelic, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("relic"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetAlignmentForCardHolder, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("holder"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.CorrectVerticalOverflow, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.CorrectHorizontalOverflow, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.Clear, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)33, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.Remove, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)33, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("owner"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetExtraFollowOffset, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)5, StringName.op_Implicit("offset"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.SetFollowOwner && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			SetFollowOwner();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.CreateAndShowMapPointHistory && ((NativeVariantPtrArgs)(ref args)).Count == 2)
		{
			NHoverTipSet nHoverTipSet = CreateAndShowMapPointHistory(VariantUtils.ConvertTo<Control>(ref ((NativeVariantPtrArgs)(ref args))[0]), VariantUtils.ConvertTo<NMapPointHistoryHoverTip>(ref ((NativeVariantPtrArgs)(ref args))[1]));
			ret = VariantUtils.CreateFrom<NHoverTipSet>(ref nHoverTipSet);
			return true;
		}
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._Process && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			((Node)this)._Process(VariantUtils.ConvertTo<double>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetAlignment && ((NativeVariantPtrArgs)(ref args)).Count == 2)
		{
			SetAlignment(VariantUtils.ConvertTo<Control>(ref ((NativeVariantPtrArgs)(ref args))[0]), VariantUtils.ConvertTo<HoverTipAlignment>(ref ((NativeVariantPtrArgs)(ref args))[1]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetAlignmentForRelic && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			SetAlignmentForRelic(VariantUtils.ConvertTo<NRelic>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetAlignmentForCardHolder && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			SetAlignmentForCardHolder(VariantUtils.ConvertTo<NCardHolder>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.CorrectVerticalOverflow && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			CorrectVerticalOverflow();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.CorrectHorizontalOverflow && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			CorrectHorizontalOverflow();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.Clear && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			Clear();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.Remove && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			Remove(VariantUtils.ConvertTo<Control>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetExtraFollowOffset && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			SetExtraFollowOffset(VariantUtils.ConvertTo<Vector2>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		return ((Control)this).InvokeGodotClassMethod(ref method, args, ref ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static bool InvokeGodotClassStaticMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.CreateAndShowMapPointHistory && ((NativeVariantPtrArgs)(ref args)).Count == 2)
		{
			NHoverTipSet nHoverTipSet = CreateAndShowMapPointHistory(VariantUtils.ConvertTo<Control>(ref ((NativeVariantPtrArgs)(ref args))[0]), VariantUtils.ConvertTo<NMapPointHistoryHoverTip>(ref ((NativeVariantPtrArgs)(ref args))[1]));
			ret = VariantUtils.CreateFrom<NHoverTipSet>(ref nHoverTipSet);
			return true;
		}
		if ((ref method) == MethodName.Clear && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			Clear();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.Remove && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			Remove(VariantUtils.ConvertTo<Control>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		ret = default(godot_variant);
		return false;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if ((ref method) == MethodName.SetFollowOwner)
		{
			return true;
		}
		if ((ref method) == MethodName.CreateAndShowMapPointHistory)
		{
			return true;
		}
		if ((ref method) == MethodName._Ready)
		{
			return true;
		}
		if ((ref method) == MethodName._Process)
		{
			return true;
		}
		if ((ref method) == MethodName.SetAlignment)
		{
			return true;
		}
		if ((ref method) == MethodName.SetAlignmentForRelic)
		{
			return true;
		}
		if ((ref method) == MethodName.SetAlignmentForCardHolder)
		{
			return true;
		}
		if ((ref method) == MethodName.CorrectVerticalOverflow)
		{
			return true;
		}
		if ((ref method) == MethodName.CorrectHorizontalOverflow)
		{
			return true;
		}
		if ((ref method) == MethodName.Clear)
		{
			return true;
		}
		if ((ref method) == MethodName.Remove)
		{
			return true;
		}
		if ((ref method) == MethodName.SetExtraFollowOffset)
		{
			return true;
		}
		return ((Control)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName._textHoverTipContainer)
		{
			_textHoverTipContainer = VariantUtils.ConvertTo<VFlowContainer>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._cardHoverTipContainer)
		{
			_cardHoverTipContainer = VariantUtils.ConvertTo<NHoverTipCardContainer>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._owner)
		{
			_owner = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._followOwner)
		{
			_followOwner = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._followOffset)
		{
			_followOffset = VariantUtils.ConvertTo<Vector2>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._extraOffset)
		{
			_extraOffset = VariantUtils.ConvertTo<Vector2>(ref value);
			return true;
		}
		return ((GodotObject)this).SetGodotClassPropertyValue(ref name, ref value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName.TextHoverTipDimensions)
		{
			Vector2 textHoverTipDimensions = TextHoverTipDimensions;
			value = VariantUtils.CreateFrom<Vector2>(ref textHoverTipDimensions);
			return true;
		}
		if ((ref name) == PropertyName.CardHoverTipDimensions)
		{
			Vector2 textHoverTipDimensions = CardHoverTipDimensions;
			value = VariantUtils.CreateFrom<Vector2>(ref textHoverTipDimensions);
			return true;
		}
		if ((ref name) == PropertyName._textHoverTipContainer)
		{
			value = VariantUtils.CreateFrom<VFlowContainer>(ref _textHoverTipContainer);
			return true;
		}
		if ((ref name) == PropertyName._cardHoverTipContainer)
		{
			value = VariantUtils.CreateFrom<NHoverTipCardContainer>(ref _cardHoverTipContainer);
			return true;
		}
		if ((ref name) == PropertyName._owner)
		{
			value = VariantUtils.CreateFrom<Control>(ref _owner);
			return true;
		}
		if ((ref name) == PropertyName._followOwner)
		{
			value = VariantUtils.CreateFrom<bool>(ref _followOwner);
			return true;
		}
		if ((ref name) == PropertyName._followOffset)
		{
			value = VariantUtils.CreateFrom<Vector2>(ref _followOffset);
			return true;
		}
		if ((ref name) == PropertyName._extraOffset)
		{
			value = VariantUtils.CreateFrom<Vector2>(ref _extraOffset);
			return true;
		}
		return ((GodotObject)this).GetGodotClassPropertyValue(ref name, ref value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._textHoverTipContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._cardHoverTipContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)5, PropertyName.TextHoverTipDimensions, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)5, PropertyName.CardHoverTipDimensions, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._owner, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._followOwner, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)5, PropertyName._followOffset, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)5, PropertyName._extraOffset, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		((GodotObject)this).SaveGodotObjectData(info);
		info.AddProperty(PropertyName._textHoverTipContainer, Variant.From<VFlowContainer>(ref _textHoverTipContainer));
		info.AddProperty(PropertyName._cardHoverTipContainer, Variant.From<NHoverTipCardContainer>(ref _cardHoverTipContainer));
		info.AddProperty(PropertyName._owner, Variant.From<Control>(ref _owner));
		info.AddProperty(PropertyName._followOwner, Variant.From<bool>(ref _followOwner));
		info.AddProperty(PropertyName._followOffset, Variant.From<Vector2>(ref _followOffset));
		info.AddProperty(PropertyName._extraOffset, Variant.From<Vector2>(ref _extraOffset));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._textHoverTipContainer, ref val))
		{
			_textHoverTipContainer = ((Variant)(ref val)).As<VFlowContainer>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._cardHoverTipContainer, ref val2))
		{
			_cardHoverTipContainer = ((Variant)(ref val2)).As<NHoverTipCardContainer>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._owner, ref val3))
		{
			_owner = ((Variant)(ref val3)).As<Control>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._followOwner, ref val4))
		{
			_followOwner = ((Variant)(ref val4)).As<bool>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._followOffset, ref val5))
		{
			_followOffset = ((Variant)(ref val5)).As<Vector2>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._extraOffset, ref val6))
		{
			_extraOffset = ((Variant)(ref val6)).As<Vector2>();
		}
	}
}
