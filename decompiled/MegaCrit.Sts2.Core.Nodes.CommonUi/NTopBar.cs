using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Debug;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Potions;
using MegaCrit.Sts2.Core.Nodes.Screens.Capstones;
using MegaCrit.Sts2.Core.Nodes.TopBar;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.addons.mega_text;
using MegaCrit.sts2.Core.Nodes.TopBar;

namespace MegaCrit.Sts2.Core.Nodes.CommonUi;

[ScriptPath("res://src/Core/Nodes/CommonUi/NTopBar.cs")]
public class NTopBar : Control
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName _ExitTree = StringName.op_Implicit("_ExitTree");

		public static readonly StringName ToggleAnimState = StringName.op_Implicit("ToggleAnimState");

		public static readonly StringName _Input = StringName.op_Implicit("_Input");

		public static readonly StringName DebugHideTopBar = StringName.op_Implicit("DebugHideTopBar");

		public static readonly StringName AnimHide = StringName.op_Implicit("AnimHide");

		public static readonly StringName AnimShow = StringName.op_Implicit("AnimShow");

		public static readonly StringName MaxPotionsChanged = StringName.op_Implicit("MaxPotionsChanged");

		public static readonly StringName UpdateNavigation = StringName.op_Implicit("UpdateNavigation");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName Map = StringName.op_Implicit("Map");

		public static readonly StringName Deck = StringName.op_Implicit("Deck");

		public static readonly StringName Pause = StringName.op_Implicit("Pause");

		public static readonly StringName PotionContainer = StringName.op_Implicit("PotionContainer");

		public static readonly StringName RoomIcon = StringName.op_Implicit("RoomIcon");

		public static readonly StringName FloorIcon = StringName.op_Implicit("FloorIcon");

		public static readonly StringName BossIcon = StringName.op_Implicit("BossIcon");

		public static readonly StringName Gold = StringName.op_Implicit("Gold");

		public static readonly StringName Hp = StringName.op_Implicit("Hp");

		public static readonly StringName Portrait = StringName.op_Implicit("Portrait");

		public static readonly StringName PortraitTip = StringName.op_Implicit("PortraitTip");

		public static readonly StringName Timer = StringName.op_Implicit("Timer");

		public static readonly StringName TrailContainer = StringName.op_Implicit("TrailContainer");

		public static readonly StringName _capstoneContainer = StringName.op_Implicit("_capstoneContainer");

		public static readonly StringName _modifiersContainer = StringName.op_Implicit("_modifiersContainer");

		public static readonly StringName _ascensionIcon = StringName.op_Implicit("_ascensionIcon");

		public static readonly StringName _ascensionLabel = StringName.op_Implicit("_ascensionLabel");

		public static readonly StringName _ascensionHsv = StringName.op_Implicit("_ascensionHsv");

		public static readonly StringName _hideTween = StringName.op_Implicit("_hideTween");

		public static readonly StringName _isDebugHidden = StringName.op_Implicit("_isDebugHidden");
	}

	public class SignalName : SignalName
	{
	}

	private NCapstoneContainer _capstoneContainer;

	private static readonly StringName _fontOutlineTheme = StringName.op_Implicit("font_outline_color");

	private static readonly StringName _h = new StringName("h");

	private static readonly StringName _v = new StringName("v");

	private static readonly Color _redLabelOutline = new Color("593400");

	private static readonly Color _blueLabelOutline = new Color("004759");

	private Control _modifiersContainer;

	private Control _ascensionIcon;

	private MegaLabel _ascensionLabel;

	private ShaderMaterial _ascensionHsv;

	private Tween? _hideTween;

	private bool _isDebugHidden;

	private Player? _player;

	public NTopBarMapButton Map { get; private set; }

	public NTopBarDeckButton Deck { get; private set; }

	public NTopBarPauseButton Pause { get; private set; }

	public NPotionContainer PotionContainer { get; private set; }

	public NTopBarRoomIcon RoomIcon { get; private set; }

	public NTopBarFloorIcon FloorIcon { get; private set; }

	public NTopBarBossIcon BossIcon { get; private set; }

	public NTopBarGold Gold { get; private set; }

	public NTopBarHp Hp { get; private set; }

	public NTopBarPortrait Portrait { get; private set; }

	public NTopBarPortraitTip PortraitTip { get; private set; }

	public NRunTimer Timer { get; private set; }

	public Node TrailContainer { get; private set; }

	public override void _Ready()
	{
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Expected O, but got Unknown
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		TrailContainer = ((Node)this).GetNode<Node>(NodePath.op_Implicit("%TrailContainer"));
		Map = ((Node)this).GetNode<NTopBarMapButton>(NodePath.op_Implicit("%Map"));
		Deck = ((Node)this).GetNode<NTopBarDeckButton>(NodePath.op_Implicit("%Deck"));
		Pause = ((Node)this).GetNode<NTopBarPauseButton>(NodePath.op_Implicit("%PauseButton"));
		PotionContainer = ((Node)this).GetNode<NPotionContainer>(NodePath.op_Implicit("%PotionContainer"));
		RoomIcon = ((Node)this).GetNode<NTopBarRoomIcon>(NodePath.op_Implicit("%RoomIcon"));
		FloorIcon = ((Node)this).GetNode<NTopBarFloorIcon>(NodePath.op_Implicit("%FloorIcon"));
		BossIcon = ((Node)this).GetNode<NTopBarBossIcon>(NodePath.op_Implicit("%BossIcon"));
		Gold = ((Node)this).GetNode<NTopBarGold>(NodePath.op_Implicit("%TopBarGold"));
		Hp = ((Node)this).GetNode<NTopBarHp>(NodePath.op_Implicit("%TopBarHp"));
		Portrait = ((Node)this).GetNode<NTopBarPortrait>(NodePath.op_Implicit("%TopBarPortrait"));
		PortraitTip = ((Node)this).GetNode<NTopBarPortraitTip>(NodePath.op_Implicit("%TopBarPortraitTip"));
		Timer = ((Node)this).GetNode<NRunTimer>(NodePath.op_Implicit("%TimerContainer"));
		_ascensionIcon = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%AscensionIcon"));
		_ascensionLabel = ((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%AscensionLabel"));
		_ascensionHsv = (ShaderMaterial)((CanvasItem)_ascensionIcon).Material;
		_modifiersContainer = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%Modifiers"));
		_capstoneContainer = ((Node)this).GetParent().GetNode<NCapstoneContainer>(NodePath.op_Implicit("%CapstoneScreenContainer"));
		((GodotObject)_capstoneContainer).Connect(SignalName.ChildEnteredTree, Callable.From<Node>((Action<Node>)ToggleAnimState), 0u);
		((GodotObject)_capstoneContainer).Connect(SignalName.ChildExitingTree, Callable.From<Node>((Action<Node>)ToggleAnimState), 0u);
	}

	public void Initialize(IRunState runState)
	{
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		if (runState.AscensionLevel > 0)
		{
			if (runState.Players.Count > 1)
			{
				_ascensionHsv.SetShaderParameter(_h, Variant.op_Implicit(0.52f));
				_ascensionHsv.SetShaderParameter(_v, Variant.op_Implicit(1.2f));
				((Control)_ascensionLabel).AddThemeColorOverride(_fontOutlineTheme, _blueLabelOutline);
			}
			else
			{
				_ascensionHsv.SetShaderParameter(_h, Variant.op_Implicit(1f));
				_ascensionHsv.SetShaderParameter(_v, Variant.op_Implicit(1f));
				((Control)_ascensionLabel).AddThemeColorOverride(_fontOutlineTheme, _redLabelOutline);
			}
			((CanvasItem)_ascensionIcon).Visible = true;
			_ascensionLabel.SetTextAutoSize(runState.AscensionLevel.ToString());
		}
		((CanvasItem)_modifiersContainer).Visible = runState.Modifiers.Count > 0;
		foreach (ModifierModel modifier in runState.Modifiers)
		{
			NTopBarModifier child = NTopBarModifier.Create(modifier);
			((Node)(object)_modifiersContainer).AddChildSafely((Node?)(object)child);
		}
		_player = LocalContext.GetMe(runState);
		Deck.Initialize(_player);
		RoomIcon.Initialize(runState);
		FloorIcon.Initialize(runState);
		BossIcon.Initialize(runState);
		Gold.Initialize(_player);
		Hp.Initialize(_player);
		Pause.Initialize(runState);
		Portrait.Initialize(_player);
		PortraitTip.Initialize(runState);
		PotionContainer.Initialize(runState);
		_player.RelicObtained += OnRelicsUpdated;
		_player.RelicRemoved += OnRelicsUpdated;
		_player.MaxPotionCountChanged += MaxPotionsChanged;
		Callable val = Callable.From((Action)UpdateNavigation);
		((Callable)(ref val)).CallDeferred(Array.Empty<Variant>());
	}

	public override void _ExitTree()
	{
		if (_player != null)
		{
			_player.RelicObtained -= OnRelicsUpdated;
			_player.RelicRemoved -= OnRelicsUpdated;
			_player.MaxPotionCountChanged -= MaxPotionsChanged;
		}
	}

	private void ToggleAnimState(Node _)
	{
		Pause.ToggleAnimState();
		Deck.ToggleAnimState();
	}

	public override void _Input(InputEvent inputEvent)
	{
		if (inputEvent.IsActionReleased(DebugHotkey.hideTopBar, false))
		{
			DebugHideTopBar();
		}
	}

	private void DebugHideTopBar()
	{
		if (!_isDebugHidden)
		{
			((Node)(object)NGame.Instance).AddChildSafely((Node?)(object)NFullscreenTextVfx.Create("Hide Top Bar"));
			AnimHide();
		}
		else
		{
			((Node)(object)NGame.Instance).AddChildSafely((Node?)(object)NFullscreenTextVfx.Create("Show Top Bar"));
			AnimShow();
		}
		_isDebugHidden = !_isDebugHidden;
	}

	public void AnimHide()
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		((Control)this).FocusBehaviorRecursive = (FocusBehaviorRecursiveEnum)1;
		Tween? hideTween = _hideTween;
		if (hideTween != null)
		{
			hideTween.Kill();
		}
		_hideTween = ((Node)this).CreateTween();
		_hideTween.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("position:y"), Variant.op_Implicit(-100f), 0.25).SetEase((EaseType)1).SetTrans((TransitionType)7);
	}

	public void AnimShow()
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		((Control)this).FocusBehaviorRecursive = (FocusBehaviorRecursiveEnum)2;
		Tween? hideTween = _hideTween;
		if (hideTween != null)
		{
			hideTween.Kill();
		}
		_hideTween = ((Node)this).CreateTween();
		_hideTween.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("position:y"), Variant.op_Implicit(0f), 0.25).SetEase((EaseType)1).SetTrans((TransitionType)7);
	}

	private void OnRelicsUpdated(RelicModel _)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		Callable val = Callable.From((Action)UpdateNavigation);
		((Callable)(ref val)).CallDeferred(Array.Empty<Variant>());
	}

	private void MaxPotionsChanged(int _)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		Callable val = Callable.From((Action)UpdateNavigation);
		((Callable)(ref val)).CallDeferred(Array.Empty<Variant>());
	}

	private void UpdateNavigation()
	{
		Control val = (Control)(object)NRun.Instance.GlobalUi.RelicInventory.RelicNodes.FirstOrDefault();
		if (val != null)
		{
			((Control)Gold).FocusNeighborBottom = ((Node)val).GetPath();
			((Control)Hp).FocusNeighborBottom = ((Node)val).GetPath();
			((Control)FloorIcon).FocusNeighborBottom = ((Node)val).GetPath();
			((Control)RoomIcon).FocusNeighborBottom = ((Node)val).GetPath();
			((Control)BossIcon).FocusNeighborBottom = ((Node)val).GetPath();
			((Control)Gold).FocusNeighborTop = ((Node)Gold).GetPath();
			((Control)Hp).FocusNeighborTop = ((Node)Hp).GetPath();
			((Control)FloorIcon).FocusNeighborTop = ((Node)FloorIcon).GetPath();
			((Control)RoomIcon).FocusNeighborTop = ((Node)RoomIcon).GetPath();
			((Control)BossIcon).FocusNeighborTop = ((Node)BossIcon).GetPath();
			((Control)Hp).FocusNeighborLeft = ((Node)Hp).GetPath();
			((Control)Hp).FocusNeighborRight = ((Node)Gold).GetPath();
			((Control)Gold).FocusNeighborLeft = ((Node)Hp).GetPath();
			NTopBarGold gold = Gold;
			Control? firstPotionControl = PotionContainer.FirstPotionControl;
			((Control)gold).FocusNeighborRight = ((firstPotionControl != null) ? ((Node)firstPotionControl).GetPath() : null);
			NTopBarRoomIcon roomIcon = RoomIcon;
			Control? lastPotionControl = PotionContainer.LastPotionControl;
			((Control)roomIcon).FocusNeighborLeft = ((lastPotionControl != null) ? ((Node)lastPotionControl).GetPath() : null);
			((Control)RoomIcon).FocusNeighborRight = ((Node)FloorIcon).GetPath();
			((Control)FloorIcon).FocusNeighborLeft = ((Node)RoomIcon).GetPath();
			((Control)FloorIcon).FocusNeighborRight = ((Node)BossIcon).GetPath();
			((Control)BossIcon).FocusNeighborLeft = ((Node)FloorIcon).GetPath();
			((Control)BossIcon).FocusNeighborRight = ((Node)BossIcon).GetPath();
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Expected O, but got Unknown
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Expected O, but got Unknown
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(9);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._ExitTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ToggleAnimState, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Node"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Input, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("inputEvent"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("InputEvent"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.DebugHideTopBar, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AnimHide, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AnimShow, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.MaxPotionsChanged, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UpdateNavigation, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
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
		if ((ref method) == MethodName.ToggleAnimState && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			ToggleAnimState(VariantUtils.ConvertTo<Node>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._Input && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			((Node)this)._Input(VariantUtils.ConvertTo<InputEvent>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.DebugHideTopBar && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			DebugHideTopBar();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.AnimHide && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			AnimHide();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.AnimShow && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			AnimShow();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.MaxPotionsChanged && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			MaxPotionsChanged(VariantUtils.ConvertTo<int>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.UpdateNavigation && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			UpdateNavigation();
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
		if ((ref method) == MethodName._ExitTree)
		{
			return true;
		}
		if ((ref method) == MethodName.ToggleAnimState)
		{
			return true;
		}
		if ((ref method) == MethodName._Input)
		{
			return true;
		}
		if ((ref method) == MethodName.DebugHideTopBar)
		{
			return true;
		}
		if ((ref method) == MethodName.AnimHide)
		{
			return true;
		}
		if ((ref method) == MethodName.AnimShow)
		{
			return true;
		}
		if ((ref method) == MethodName.MaxPotionsChanged)
		{
			return true;
		}
		if ((ref method) == MethodName.UpdateNavigation)
		{
			return true;
		}
		return ((Control)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName.Map)
		{
			Map = VariantUtils.ConvertTo<NTopBarMapButton>(ref value);
			return true;
		}
		if ((ref name) == PropertyName.Deck)
		{
			Deck = VariantUtils.ConvertTo<NTopBarDeckButton>(ref value);
			return true;
		}
		if ((ref name) == PropertyName.Pause)
		{
			Pause = VariantUtils.ConvertTo<NTopBarPauseButton>(ref value);
			return true;
		}
		if ((ref name) == PropertyName.PotionContainer)
		{
			PotionContainer = VariantUtils.ConvertTo<NPotionContainer>(ref value);
			return true;
		}
		if ((ref name) == PropertyName.RoomIcon)
		{
			RoomIcon = VariantUtils.ConvertTo<NTopBarRoomIcon>(ref value);
			return true;
		}
		if ((ref name) == PropertyName.FloorIcon)
		{
			FloorIcon = VariantUtils.ConvertTo<NTopBarFloorIcon>(ref value);
			return true;
		}
		if ((ref name) == PropertyName.BossIcon)
		{
			BossIcon = VariantUtils.ConvertTo<NTopBarBossIcon>(ref value);
			return true;
		}
		if ((ref name) == PropertyName.Gold)
		{
			Gold = VariantUtils.ConvertTo<NTopBarGold>(ref value);
			return true;
		}
		if ((ref name) == PropertyName.Hp)
		{
			Hp = VariantUtils.ConvertTo<NTopBarHp>(ref value);
			return true;
		}
		if ((ref name) == PropertyName.Portrait)
		{
			Portrait = VariantUtils.ConvertTo<NTopBarPortrait>(ref value);
			return true;
		}
		if ((ref name) == PropertyName.PortraitTip)
		{
			PortraitTip = VariantUtils.ConvertTo<NTopBarPortraitTip>(ref value);
			return true;
		}
		if ((ref name) == PropertyName.Timer)
		{
			Timer = VariantUtils.ConvertTo<NRunTimer>(ref value);
			return true;
		}
		if ((ref name) == PropertyName.TrailContainer)
		{
			TrailContainer = VariantUtils.ConvertTo<Node>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._capstoneContainer)
		{
			_capstoneContainer = VariantUtils.ConvertTo<NCapstoneContainer>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._modifiersContainer)
		{
			_modifiersContainer = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._ascensionIcon)
		{
			_ascensionIcon = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._ascensionLabel)
		{
			_ascensionLabel = VariantUtils.ConvertTo<MegaLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._ascensionHsv)
		{
			_ascensionHsv = VariantUtils.ConvertTo<ShaderMaterial>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._hideTween)
		{
			_hideTween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._isDebugHidden)
		{
			_isDebugHidden = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		return ((GodotObject)this).SetGodotClassPropertyValue(ref name, ref value);
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
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_0269: Unknown result type (might be due to invalid IL or missing references)
		//IL_0284: Unknown result type (might be due to invalid IL or missing references)
		//IL_0289: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName.Map)
		{
			NTopBarMapButton map = Map;
			value = VariantUtils.CreateFrom<NTopBarMapButton>(ref map);
			return true;
		}
		if ((ref name) == PropertyName.Deck)
		{
			NTopBarDeckButton deck = Deck;
			value = VariantUtils.CreateFrom<NTopBarDeckButton>(ref deck);
			return true;
		}
		if ((ref name) == PropertyName.Pause)
		{
			NTopBarPauseButton pause = Pause;
			value = VariantUtils.CreateFrom<NTopBarPauseButton>(ref pause);
			return true;
		}
		if ((ref name) == PropertyName.PotionContainer)
		{
			NPotionContainer potionContainer = PotionContainer;
			value = VariantUtils.CreateFrom<NPotionContainer>(ref potionContainer);
			return true;
		}
		if ((ref name) == PropertyName.RoomIcon)
		{
			NTopBarRoomIcon roomIcon = RoomIcon;
			value = VariantUtils.CreateFrom<NTopBarRoomIcon>(ref roomIcon);
			return true;
		}
		if ((ref name) == PropertyName.FloorIcon)
		{
			NTopBarFloorIcon floorIcon = FloorIcon;
			value = VariantUtils.CreateFrom<NTopBarFloorIcon>(ref floorIcon);
			return true;
		}
		if ((ref name) == PropertyName.BossIcon)
		{
			NTopBarBossIcon bossIcon = BossIcon;
			value = VariantUtils.CreateFrom<NTopBarBossIcon>(ref bossIcon);
			return true;
		}
		if ((ref name) == PropertyName.Gold)
		{
			NTopBarGold gold = Gold;
			value = VariantUtils.CreateFrom<NTopBarGold>(ref gold);
			return true;
		}
		if ((ref name) == PropertyName.Hp)
		{
			NTopBarHp hp = Hp;
			value = VariantUtils.CreateFrom<NTopBarHp>(ref hp);
			return true;
		}
		if ((ref name) == PropertyName.Portrait)
		{
			NTopBarPortrait portrait = Portrait;
			value = VariantUtils.CreateFrom<NTopBarPortrait>(ref portrait);
			return true;
		}
		if ((ref name) == PropertyName.PortraitTip)
		{
			NTopBarPortraitTip portraitTip = PortraitTip;
			value = VariantUtils.CreateFrom<NTopBarPortraitTip>(ref portraitTip);
			return true;
		}
		if ((ref name) == PropertyName.Timer)
		{
			NRunTimer timer = Timer;
			value = VariantUtils.CreateFrom<NRunTimer>(ref timer);
			return true;
		}
		if ((ref name) == PropertyName.TrailContainer)
		{
			Node trailContainer = TrailContainer;
			value = VariantUtils.CreateFrom<Node>(ref trailContainer);
			return true;
		}
		if ((ref name) == PropertyName._capstoneContainer)
		{
			value = VariantUtils.CreateFrom<NCapstoneContainer>(ref _capstoneContainer);
			return true;
		}
		if ((ref name) == PropertyName._modifiersContainer)
		{
			value = VariantUtils.CreateFrom<Control>(ref _modifiersContainer);
			return true;
		}
		if ((ref name) == PropertyName._ascensionIcon)
		{
			value = VariantUtils.CreateFrom<Control>(ref _ascensionIcon);
			return true;
		}
		if ((ref name) == PropertyName._ascensionLabel)
		{
			value = VariantUtils.CreateFrom<MegaLabel>(ref _ascensionLabel);
			return true;
		}
		if ((ref name) == PropertyName._ascensionHsv)
		{
			value = VariantUtils.CreateFrom<ShaderMaterial>(ref _ascensionHsv);
			return true;
		}
		if ((ref name) == PropertyName._hideTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _hideTween);
			return true;
		}
		if ((ref name) == PropertyName._isDebugHidden)
		{
			value = VariantUtils.CreateFrom<bool>(ref _isDebugHidden);
			return true;
		}
		return ((GodotObject)this).GetGodotClassPropertyValue(ref name, ref value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_028f: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._capstoneContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.Map, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.Deck, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.Pause, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.PotionContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.RoomIcon, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.FloorIcon, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.BossIcon, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.Gold, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.Hp, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.Portrait, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.PortraitTip, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.Timer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.TrailContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._modifiersContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._ascensionIcon, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._ascensionLabel, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._ascensionHsv, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._hideTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._isDebugHidden, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		StringName map = PropertyName.Map;
		NTopBarMapButton map2 = Map;
		info.AddProperty(map, Variant.From<NTopBarMapButton>(ref map2));
		StringName deck = PropertyName.Deck;
		NTopBarDeckButton deck2 = Deck;
		info.AddProperty(deck, Variant.From<NTopBarDeckButton>(ref deck2));
		StringName pause = PropertyName.Pause;
		NTopBarPauseButton pause2 = Pause;
		info.AddProperty(pause, Variant.From<NTopBarPauseButton>(ref pause2));
		StringName potionContainer = PropertyName.PotionContainer;
		NPotionContainer potionContainer2 = PotionContainer;
		info.AddProperty(potionContainer, Variant.From<NPotionContainer>(ref potionContainer2));
		StringName roomIcon = PropertyName.RoomIcon;
		NTopBarRoomIcon roomIcon2 = RoomIcon;
		info.AddProperty(roomIcon, Variant.From<NTopBarRoomIcon>(ref roomIcon2));
		StringName floorIcon = PropertyName.FloorIcon;
		NTopBarFloorIcon floorIcon2 = FloorIcon;
		info.AddProperty(floorIcon, Variant.From<NTopBarFloorIcon>(ref floorIcon2));
		StringName bossIcon = PropertyName.BossIcon;
		NTopBarBossIcon bossIcon2 = BossIcon;
		info.AddProperty(bossIcon, Variant.From<NTopBarBossIcon>(ref bossIcon2));
		StringName gold = PropertyName.Gold;
		NTopBarGold gold2 = Gold;
		info.AddProperty(gold, Variant.From<NTopBarGold>(ref gold2));
		StringName hp = PropertyName.Hp;
		NTopBarHp hp2 = Hp;
		info.AddProperty(hp, Variant.From<NTopBarHp>(ref hp2));
		StringName portrait = PropertyName.Portrait;
		NTopBarPortrait portrait2 = Portrait;
		info.AddProperty(portrait, Variant.From<NTopBarPortrait>(ref portrait2));
		StringName portraitTip = PropertyName.PortraitTip;
		NTopBarPortraitTip portraitTip2 = PortraitTip;
		info.AddProperty(portraitTip, Variant.From<NTopBarPortraitTip>(ref portraitTip2));
		StringName timer = PropertyName.Timer;
		NRunTimer timer2 = Timer;
		info.AddProperty(timer, Variant.From<NRunTimer>(ref timer2));
		StringName trailContainer = PropertyName.TrailContainer;
		Node trailContainer2 = TrailContainer;
		info.AddProperty(trailContainer, Variant.From<Node>(ref trailContainer2));
		info.AddProperty(PropertyName._capstoneContainer, Variant.From<NCapstoneContainer>(ref _capstoneContainer));
		info.AddProperty(PropertyName._modifiersContainer, Variant.From<Control>(ref _modifiersContainer));
		info.AddProperty(PropertyName._ascensionIcon, Variant.From<Control>(ref _ascensionIcon));
		info.AddProperty(PropertyName._ascensionLabel, Variant.From<MegaLabel>(ref _ascensionLabel));
		info.AddProperty(PropertyName._ascensionHsv, Variant.From<ShaderMaterial>(ref _ascensionHsv));
		info.AddProperty(PropertyName._hideTween, Variant.From<Tween>(ref _hideTween));
		info.AddProperty(PropertyName._isDebugHidden, Variant.From<bool>(ref _isDebugHidden));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName.Map, ref val))
		{
			Map = ((Variant)(ref val)).As<NTopBarMapButton>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName.Deck, ref val2))
		{
			Deck = ((Variant)(ref val2)).As<NTopBarDeckButton>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName.Pause, ref val3))
		{
			Pause = ((Variant)(ref val3)).As<NTopBarPauseButton>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName.PotionContainer, ref val4))
		{
			PotionContainer = ((Variant)(ref val4)).As<NPotionContainer>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName.RoomIcon, ref val5))
		{
			RoomIcon = ((Variant)(ref val5)).As<NTopBarRoomIcon>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName.FloorIcon, ref val6))
		{
			FloorIcon = ((Variant)(ref val6)).As<NTopBarFloorIcon>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName.BossIcon, ref val7))
		{
			BossIcon = ((Variant)(ref val7)).As<NTopBarBossIcon>();
		}
		Variant val8 = default(Variant);
		if (info.TryGetProperty(PropertyName.Gold, ref val8))
		{
			Gold = ((Variant)(ref val8)).As<NTopBarGold>();
		}
		Variant val9 = default(Variant);
		if (info.TryGetProperty(PropertyName.Hp, ref val9))
		{
			Hp = ((Variant)(ref val9)).As<NTopBarHp>();
		}
		Variant val10 = default(Variant);
		if (info.TryGetProperty(PropertyName.Portrait, ref val10))
		{
			Portrait = ((Variant)(ref val10)).As<NTopBarPortrait>();
		}
		Variant val11 = default(Variant);
		if (info.TryGetProperty(PropertyName.PortraitTip, ref val11))
		{
			PortraitTip = ((Variant)(ref val11)).As<NTopBarPortraitTip>();
		}
		Variant val12 = default(Variant);
		if (info.TryGetProperty(PropertyName.Timer, ref val12))
		{
			Timer = ((Variant)(ref val12)).As<NRunTimer>();
		}
		Variant val13 = default(Variant);
		if (info.TryGetProperty(PropertyName.TrailContainer, ref val13))
		{
			TrailContainer = ((Variant)(ref val13)).As<Node>();
		}
		Variant val14 = default(Variant);
		if (info.TryGetProperty(PropertyName._capstoneContainer, ref val14))
		{
			_capstoneContainer = ((Variant)(ref val14)).As<NCapstoneContainer>();
		}
		Variant val15 = default(Variant);
		if (info.TryGetProperty(PropertyName._modifiersContainer, ref val15))
		{
			_modifiersContainer = ((Variant)(ref val15)).As<Control>();
		}
		Variant val16 = default(Variant);
		if (info.TryGetProperty(PropertyName._ascensionIcon, ref val16))
		{
			_ascensionIcon = ((Variant)(ref val16)).As<Control>();
		}
		Variant val17 = default(Variant);
		if (info.TryGetProperty(PropertyName._ascensionLabel, ref val17))
		{
			_ascensionLabel = ((Variant)(ref val17)).As<MegaLabel>();
		}
		Variant val18 = default(Variant);
		if (info.TryGetProperty(PropertyName._ascensionHsv, ref val18))
		{
			_ascensionHsv = ((Variant)(ref val18)).As<ShaderMaterial>();
		}
		Variant val19 = default(Variant);
		if (info.TryGetProperty(PropertyName._hideTween, ref val19))
		{
			_hideTween = ((Variant)(ref val19)).As<Tween>();
		}
		Variant val20 = default(Variant);
		if (info.TryGetProperty(PropertyName._isDebugHidden, ref val20))
		{
			_isDebugHidden = ((Variant)(ref val20)).As<bool>();
		}
	}
}
