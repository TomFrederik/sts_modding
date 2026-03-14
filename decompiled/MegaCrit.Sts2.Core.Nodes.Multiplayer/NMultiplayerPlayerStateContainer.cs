using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Debug;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.Relics;
using MegaCrit.Sts2.Core.Nodes.Screens.ScreenContext;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.Nodes.Multiplayer;

[ScriptPath("res://src/Core/Nodes/Multiplayer/NMultiplayerPlayerStateContainer.cs")]
public class NMultiplayerPlayerStateContainer : Control
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName _EnterTree = StringName.op_Implicit("_EnterTree");

		public static readonly StringName _ExitTree = StringName.op_Implicit("_ExitTree");

		public static readonly StringName _Input = StringName.op_Implicit("_Input");

		public static readonly StringName UpdateNavigation = StringName.op_Implicit("UpdateNavigation");

		public static readonly StringName LockNavigation = StringName.op_Implicit("LockNavigation");

		public static readonly StringName UnlockNavigation = StringName.op_Implicit("UnlockNavigation");

		public static readonly StringName UpdatePositionAfterOneFrame = StringName.op_Implicit("UpdatePositionAfterOneFrame");

		public static readonly StringName UpdatePosition = StringName.op_Implicit("UpdatePosition");

		public static readonly StringName AnimHide = StringName.op_Implicit("AnimHide");

		public static readonly StringName AnimShow = StringName.op_Implicit("AnimShow");

		public static readonly StringName ShowImmediately = StringName.op_Implicit("ShowImmediately");

		public static readonly StringName HideImmediately = StringName.op_Implicit("HideImmediately");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName FirstPlayerState = StringName.op_Implicit("FirstPlayerState");

		public static readonly StringName _tween = StringName.op_Implicit("_tween");

		public static readonly StringName _originalPosition = StringName.op_Implicit("_originalPosition");
	}

	public class SignalName : SignalName
	{
	}

	private IRunState _runState;

	private readonly List<NMultiplayerPlayerState> _nodes = new List<NMultiplayerPlayerState>();

	private Tween? _tween;

	private Vector2 _originalPosition;

	public NMultiplayerPlayerState? FirstPlayerState => ((Node)this).GetChild<NMultiplayerPlayerState>(0, false);

	public override void _Ready()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_originalPosition = ((Control)this).Position;
	}

	public override void _EnterTree()
	{
		ActiveScreenContext.Instance.Updated += UpdateNavigation;
	}

	public override void _ExitTree()
	{
		ActiveScreenContext.Instance.Updated -= UpdateNavigation;
	}

	public override void _Input(InputEvent inputEvent)
	{
		if (inputEvent.IsActionReleased(DebugHotkey.hideMpHealthBars, false))
		{
			((CanvasItem)this).Visible = !((CanvasItem)this).Visible;
			((Node)(object)NGame.Instance).AddChildSafely((Node?)(object)NFullscreenTextVfx.Create((!((CanvasItem)this).Visible) ? "Hide MP Health bars" : "Show MP Health bars"));
		}
	}

	public void Initialize(RunState runState)
	{
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		_runState = runState;
		if (_runState.Players.Count <= 1)
		{
			return;
		}
		Player me = LocalContext.GetMe(_runState);
		NMultiplayerPlayerState nMultiplayerPlayerState = NMultiplayerPlayerState.Create(me);
		((Node)(object)this).AddChildSafely((Node?)(object)nMultiplayerPlayerState);
		_nodes.Add(nMultiplayerPlayerState);
		foreach (Player item in _runState.Players.Except(new _003C_003Ez__ReadOnlySingleElementList<Player>(me)))
		{
			NMultiplayerPlayerState nMultiplayerPlayerState2 = NMultiplayerPlayerState.Create(item);
			((Node)(object)this).AddChildSafely((Node?)(object)nMultiplayerPlayerState2);
			_nodes.Add(nMultiplayerPlayerState2);
		}
		UpdatePosition();
		((GodotObject)NRun.Instance.GlobalUi.RelicInventory).Connect(NRelicInventory.SignalName.RelicsChanged, Callable.From((Action)UpdatePositionAfterOneFrame), 0u);
		((GodotObject)((Node)this).GetViewport()).Connect(SignalName.SizeChanged, Callable.From((Action)UpdatePositionAfterOneFrame), 0u);
		for (int i = 0; i < ((Node)this).GetChildCount(false); i++)
		{
			Control hitbox = (Control)(object)((Node)this).GetChild<NMultiplayerPlayerState>(i, false).Hitbox;
			hitbox.FocusNeighborLeft = ((Node)hitbox).GetPath();
			hitbox.FocusNeighborTop = ((i > 0) ? ((Node)((Node)this).GetChild<NMultiplayerPlayerState>(i - 1, false).Hitbox).GetPath() : null);
			hitbox.FocusNeighborBottom = ((i < ((Node)this).GetChildCount(false) - 1) ? ((Node)((Node)this).GetChild<NMultiplayerPlayerState>(i + 1, false).Hitbox).GetPath() : null);
		}
	}

	private void UpdateNavigation()
	{
		for (int i = 0; i < ((Node)this).GetChildCount(false); i++)
		{
			Control hitbox = (Control)(object)((Node)this).GetChild<NMultiplayerPlayerState>(i, false).Hitbox;
			hitbox.FocusNeighborTop = ((i > 0) ? ((Node)((Node)this).GetChild<NMultiplayerPlayerState>(i - 1, false).Hitbox).GetPath() : null);
			object focusNeighborBottom;
			if (i >= ((Node)this).GetChildCount(false) - 1)
			{
				IScreenContext? currentScreen = ActiveScreenContext.Instance.GetCurrentScreen();
				if (currentScreen == null)
				{
					focusNeighborBottom = null;
				}
				else
				{
					Control? focusedControlFromTopBar = currentScreen.FocusedControlFromTopBar;
					focusNeighborBottom = ((focusedControlFromTopBar != null) ? ((Node)focusedControlFromTopBar).GetPath() : null);
				}
			}
			else
			{
				focusNeighborBottom = ((Node)((Node)this).GetChild<NMultiplayerPlayerState>(i + 1, false).Hitbox).GetPath();
			}
			hitbox.FocusNeighborBottom = (NodePath)focusNeighborBottom;
		}
	}

	public void LockNavigation()
	{
		for (int i = 0; i < ((Node)this).GetChildCount(false); i++)
		{
			Control hitbox = (Control)(object)((Node)this).GetChild<NMultiplayerPlayerState>(i, false).Hitbox;
			hitbox.FocusNeighborTop = ((i > 0) ? ((Node)((Node)this).GetChild<NMultiplayerPlayerState>(i - 1, false).Hitbox).GetPath() : ((Node)hitbox).GetPath());
			hitbox.FocusNeighborBottom = ((i < ((Node)this).GetChildCount(false) - 1) ? ((Node)((Node)this).GetChild<NMultiplayerPlayerState>(i + 1, false).Hitbox).GetPath() : ((Node)hitbox).GetPath());
			hitbox.FocusNeighborLeft = ((Node)hitbox).GetPath();
			hitbox.FocusNeighborRight = ((Node)hitbox).GetPath();
		}
	}

	public void UnlockNavigation()
	{
		UpdateNavigation();
	}

	private void UpdatePositionAfterOneFrame()
	{
		TaskHelper.RunSafely(UpdatePositionAfterOneFrameAsync());
	}

	private async Task UpdatePositionAfterOneFrameAsync()
	{
		await ((GodotObject)this).ToSignal((GodotObject)(object)((Node)this).GetTree(), SignalName.ProcessFrame);
		UpdatePosition();
	}

	private void UpdatePosition()
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		NRelicInventory relicInventory = NRun.Instance.GlobalUi.RelicInventory;
		int lineCount = ((FlowContainer)relicInventory).GetLineCount();
		if (lineCount == 0 || ((Node)relicInventory).GetChildCount(false) == 0)
		{
			((Control)this).Position = ((Control)relicInventory).Position;
			return;
		}
		float y = ((Node)relicInventory).GetChild<Control>(0, false).Size.Y;
		float num = ((Control)relicInventory).GetThemeConstant(ThemeConstants.FlowContainer.vSeparation, StringName.op_Implicit("FlowContainer"));
		((Control)this).Position = ((Control)relicInventory).Position + (float)lineCount * (y + num) * Vector2.Down;
	}

	public void HighlightPlayer(Player player)
	{
		_nodes.FirstOrDefault((NMultiplayerPlayerState n) => n.Player == player)?.OnCreatureHovered();
	}

	public void UnhighlightPlayer(Player player)
	{
		_nodes.FirstOrDefault((NMultiplayerPlayerState n) => n.Player == player)?.OnCreatureUnhovered();
	}

	public void FlashPlayerReady(Player player)
	{
		_nodes.FirstOrDefault((NMultiplayerPlayerState n) => n.Player == player)?.FlashPlayerReady();
	}

	public void AnimHide()
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		Tween? tween = _tween;
		if (tween != null)
		{
			tween.Kill();
		}
		_tween = ((Node)this).CreateTween();
		_tween.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("position:x"), Variant.op_Implicit(0f - ((Control)this).Size.X), 0.20000000298023224).SetTrans((TransitionType)4).SetEase((EaseType)2);
		_tween.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0f), 0.20000000298023224).SetTrans((TransitionType)4).SetEase((EaseType)2);
	}

	public void AnimShow()
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		Tween? tween = _tween;
		if (tween != null)
		{
			tween.Kill();
		}
		_tween = ((Node)this).CreateTween();
		_tween.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("position:x"), Variant.op_Implicit(_originalPosition.X), 0.25).SetTrans((TransitionType)4).SetEase((EaseType)1);
		_tween.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(1f), 0.15000000596046448).SetTrans((TransitionType)4).SetEase((EaseType)0);
	}

	public void ShowImmediately()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		Tween? tween = _tween;
		if (tween != null)
		{
			tween.Kill();
		}
		Vector2 position = ((Control)this).Position;
		position.X = _originalPosition.X;
		((Control)this).Position = position;
		Color modulate = ((CanvasItem)this).Modulate;
		modulate.A = 1f;
		((CanvasItem)this).Modulate = modulate;
	}

	public void HideImmediately()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		Tween? tween = _tween;
		if (tween != null)
		{
			tween.Kill();
		}
		Vector2 position = ((Control)this).Position;
		position.X = 0f - ((Control)this).Size.X;
		((Control)this).Position = position;
		Color modulate = ((CanvasItem)this).Modulate;
		modulate.A = 0f;
		((CanvasItem)this).Modulate = modulate;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Expected O, but got Unknown
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		//IL_0288: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(13);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._EnterTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._ExitTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Input, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("inputEvent"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("InputEvent"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UpdateNavigation, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.LockNavigation, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UnlockNavigation, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UpdatePositionAfterOneFrame, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UpdatePosition, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AnimHide, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AnimShow, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ShowImmediately, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.HideImmediately, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._EnterTree && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._EnterTree();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._ExitTree && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._ExitTree();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._Input && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			((Node)this)._Input(VariantUtils.ConvertTo<InputEvent>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.UpdateNavigation && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			UpdateNavigation();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.LockNavigation && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			LockNavigation();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.UnlockNavigation && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			UnlockNavigation();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.UpdatePositionAfterOneFrame && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			UpdatePositionAfterOneFrame();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.UpdatePosition && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			UpdatePosition();
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
		if ((ref method) == MethodName.ShowImmediately && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			ShowImmediately();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.HideImmediately && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			HideImmediately();
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
		if ((ref method) == MethodName._EnterTree)
		{
			return true;
		}
		if ((ref method) == MethodName._ExitTree)
		{
			return true;
		}
		if ((ref method) == MethodName._Input)
		{
			return true;
		}
		if ((ref method) == MethodName.UpdateNavigation)
		{
			return true;
		}
		if ((ref method) == MethodName.LockNavigation)
		{
			return true;
		}
		if ((ref method) == MethodName.UnlockNavigation)
		{
			return true;
		}
		if ((ref method) == MethodName.UpdatePositionAfterOneFrame)
		{
			return true;
		}
		if ((ref method) == MethodName.UpdatePosition)
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
		if ((ref method) == MethodName.ShowImmediately)
		{
			return true;
		}
		if ((ref method) == MethodName.HideImmediately)
		{
			return true;
		}
		return ((Control)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName._tween)
		{
			_tween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._originalPosition)
		{
			_originalPosition = VariantUtils.ConvertTo<Vector2>(ref value);
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
		if ((ref name) == PropertyName.FirstPlayerState)
		{
			NMultiplayerPlayerState firstPlayerState = FirstPlayerState;
			value = VariantUtils.CreateFrom<NMultiplayerPlayerState>(ref firstPlayerState);
			return true;
		}
		if ((ref name) == PropertyName._tween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _tween);
			return true;
		}
		if ((ref name) == PropertyName._originalPosition)
		{
			value = VariantUtils.CreateFrom<Vector2>(ref _originalPosition);
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
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName.FirstPlayerState, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._tween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)5, PropertyName._originalPosition, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		info.AddProperty(PropertyName._tween, Variant.From<Tween>(ref _tween));
		info.AddProperty(PropertyName._originalPosition, Variant.From<Vector2>(ref _originalPosition));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._tween, ref val))
		{
			_tween = ((Variant)(ref val)).As<Tween>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._originalPosition, ref val2))
		{
			_originalPosition = ((Variant)(ref val2)).As<Vector2>();
		}
	}
}
