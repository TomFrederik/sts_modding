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
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Multiplayer;
using MegaCrit.Sts2.Core.Nodes.Screens.ScreenContext;
using MegaCrit.Sts2.Core.Runs;

namespace MegaCrit.Sts2.Core.Nodes.Relics;

[ScriptPath("res://src/Core/Nodes/Relics/NRelicInventory.cs")]
public class NRelicInventory : FlowContainer
{
	[Signal]
	public delegate void RelicsChangedEventHandler();

	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName _EnterTree = StringName.op_Implicit("_EnterTree");

		public static readonly StringName _ExitTree = StringName.op_Implicit("_ExitTree");

		public static readonly StringName ConnectPlayerEvents = StringName.op_Implicit("ConnectPlayerEvents");

		public static readonly StringName DisconnectPlayerEvents = StringName.op_Implicit("DisconnectPlayerEvents");

		public static readonly StringName OnRelicUnfocused = StringName.op_Implicit("OnRelicUnfocused");

		public static readonly StringName AnimShow = StringName.op_Implicit("AnimShow");

		public static readonly StringName AnimHide = StringName.op_Implicit("AnimHide");

		public static readonly StringName ShowImmediately = StringName.op_Implicit("ShowImmediately");

		public static readonly StringName HideImmediately = StringName.op_Implicit("HideImmediately");

		public static readonly StringName _Input = StringName.op_Implicit("_Input");

		public static readonly StringName DebugHideTopBar = StringName.op_Implicit("DebugHideTopBar");

		public static readonly StringName UpdateNavigation = StringName.op_Implicit("UpdateNavigation");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName _originalPos = StringName.op_Implicit("_originalPos");

		public static readonly StringName _curTween = StringName.op_Implicit("_curTween");

		public static readonly StringName _debugHideTween = StringName.op_Implicit("_debugHideTween");

		public static readonly StringName _isDebugHidden = StringName.op_Implicit("_isDebugHidden");
	}

	public class SignalName : SignalName
	{
		public static readonly StringName RelicsChanged = StringName.op_Implicit("RelicsChanged");
	}

	private Player? _player;

	private readonly List<NRelicInventoryHolder> _relicNodes = new List<NRelicInventoryHolder>();

	private Vector2 _originalPos;

	private Tween? _curTween;

	private Tween? _debugHideTween;

	private bool _isDebugHidden;

	private RelicsChangedEventHandler backing_RelicsChanged;

	public IReadOnlyList<NRelicInventoryHolder> RelicNodes => _relicNodes;

	public event RelicsChangedEventHandler RelicsChanged
	{
		add
		{
			backing_RelicsChanged = (RelicsChangedEventHandler)Delegate.Combine(backing_RelicsChanged, value);
		}
		remove
		{
			backing_RelicsChanged = (RelicsChangedEventHandler)Delegate.Remove(backing_RelicsChanged, value);
		}
	}

	public override void _Ready()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		_originalPos = ((Control)this).Position;
		((GodotObject)this).Connect(SignalName.FocusEntered, Callable.From((Action)delegate
		{
			((Control)(object)_relicNodes[0]).TryGrabFocus();
		}), 0u);
	}

	public override void _EnterTree()
	{
		((Node)this)._EnterTree();
		ActiveScreenContext.Instance.Updated += UpdateNavigation;
		ConnectPlayerEvents();
	}

	public override void _ExitTree()
	{
		((Node)this)._ExitTree();
		ActiveScreenContext.Instance.Updated -= UpdateNavigation;
		DisconnectPlayerEvents();
	}

	public void Initialize(RunState runState)
	{
		DisconnectPlayerEvents();
		_player = LocalContext.GetMe(runState);
		ConnectPlayerEvents();
		foreach (RelicModel relic in _player.Relics)
		{
			Add(relic, startsShown: true);
		}
	}

	private void ConnectPlayerEvents()
	{
		if (_player != null)
		{
			_player.RelicObtained += OnRelicObtained;
			_player.RelicRemoved += OnRelicRemoved;
		}
	}

	private void DisconnectPlayerEvents()
	{
		if (_player != null)
		{
			_player.RelicObtained -= OnRelicObtained;
			_player.RelicRemoved -= OnRelicRemoved;
		}
	}

	private void Add(RelicModel relic, bool startsShown, int index = -1)
	{
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		NRelicInventoryHolder nRelicInventoryHolder = NRelicInventoryHolder.Create(relic);
		nRelicInventoryHolder.Inventory = this;
		if (index < 0)
		{
			_relicNodes.Add(nRelicInventoryHolder);
		}
		else
		{
			_relicNodes.Insert(index, nRelicInventoryHolder);
		}
		((Node)(object)this).AddChildSafely((Node?)(object)nRelicInventoryHolder);
		((Node)this).MoveChild((Node)(object)nRelicInventoryHolder, index);
		if (!startsShown)
		{
			TextureRect icon = nRelicInventoryHolder.Relic.Icon;
			Color modulate = ((CanvasItem)nRelicInventoryHolder.Relic.Icon).Modulate;
			modulate.A = 0f;
			((CanvasItem)icon).Modulate = modulate;
			UpdateNavigation();
		}
		((GodotObject)nRelicInventoryHolder).Connect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)delegate
		{
			OnRelicClicked(relic);
		}), 0u);
		((GodotObject)nRelicInventoryHolder).Connect(NClickableControl.SignalName.Focused, Callable.From<NClickableControl>((Action<NClickableControl>)delegate
		{
			OnRelicFocused(relic);
		}), 0u);
		((GodotObject)nRelicInventoryHolder).Connect(NClickableControl.SignalName.Unfocused, Callable.From<NClickableControl>((Action<NClickableControl>)delegate
		{
			OnRelicUnfocused();
		}), 0u);
		((GodotObject)this).EmitSignal(SignalName.RelicsChanged, Array.Empty<Variant>());
	}

	private void Remove(RelicModel relic)
	{
		if (LocalContext.IsMine(relic))
		{
			NRelicInventoryHolder nRelicInventoryHolder = _relicNodes.First((NRelicInventoryHolder n) => n.Relic.Model == relic);
			_relicNodes.Remove(nRelicInventoryHolder);
			((Node)(object)this).RemoveChildSafely((Node?)(object)nRelicInventoryHolder);
			EmitSignalRelicsChanged();
			UpdateNavigation();
		}
	}

	private void OnRelicClicked(RelicModel model)
	{
		List<RelicModel> list = new List<RelicModel>();
		foreach (NRelicInventoryHolder relicNode in _relicNodes)
		{
			list.Add(relicNode.Relic.Model);
		}
		NGame.Instance.GetInspectRelicScreen().Open(list, model);
	}

	private void OnRelicFocused(RelicModel model)
	{
		RunManager.Instance.HoveredModelTracker.OnLocalRelicHovered(model);
	}

	private static void OnRelicUnfocused()
	{
		RunManager.Instance.HoveredModelTracker.OnLocalRelicUnhovered();
	}

	public void AnimateRelic(RelicModel relic, Vector2? startPosition = null, Vector2? startScale = null)
	{
		if (LocalContext.IsMine(relic))
		{
			NRelicInventoryHolder nRelicInventoryHolder = _relicNodes.First((NRelicInventoryHolder n) => n.Relic.Model == relic);
			TaskHelper.RunSafely(nRelicInventoryHolder.PlayNewlyAcquiredAnimation(startPosition, startScale));
		}
	}

	private void OnRelicObtained(RelicModel relic)
	{
		Add(relic, startsShown: false, _player.Relics.IndexOf(relic));
	}

	private void OnRelicRemoved(RelicModel relic)
	{
		Remove(relic);
	}

	public void AnimShow()
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		((Control)this).FocusBehaviorRecursive = (FocusBehaviorRecursiveEnum)2;
		Tween? curTween = _curTween;
		if (curTween != null)
		{
			curTween.Kill();
		}
		_curTween = ((Node)this).CreateTween();
		_curTween.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("global_position:y"), Variant.op_Implicit(_originalPos.Y), 0.25).SetTrans((TransitionType)7).SetEase((EaseType)1);
	}

	public void AnimHide()
	{
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		((Control)this).FocusBehaviorRecursive = (FocusBehaviorRecursiveEnum)1;
		Tween? curTween = _curTween;
		if (curTween != null)
		{
			curTween.Kill();
		}
		_curTween = ((Node)this).CreateTween();
		_curTween.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("global_position:y"), Variant.op_Implicit(_originalPos.Y - 68f * (float)((FlowContainer)this).GetLineCount() - 90f), 0.25).SetEase((EaseType)1).SetTrans((TransitionType)7);
	}

	public void ShowImmediately()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		Tween? curTween = _curTween;
		if (curTween != null)
		{
			curTween.Kill();
		}
		Vector2 position = ((Control)this).Position;
		position.Y = _originalPos.Y;
		((Control)this).Position = position;
	}

	public void HideImmediately()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		Tween? curTween = _curTween;
		if (curTween != null)
		{
			curTween.Kill();
		}
		Vector2 position = ((Control)this).Position;
		position.Y = _originalPos.Y - 68f * (float)((FlowContainer)this).GetLineCount() - 90f;
		((Control)this).Position = position;
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
		if (_isDebugHidden)
		{
			AnimShow();
		}
		else
		{
			AnimHide();
		}
		_isDebugHidden = !_isDebugHidden;
	}

	private void UpdateNavigation()
	{
		for (int i = 0; i < RelicNodes.Count; i++)
		{
			NRelicInventoryHolder nRelicInventoryHolder = RelicNodes[i];
			((Control)nRelicInventoryHolder).FocusNeighborLeft = ((i > 0) ? ((Node)RelicNodes[i - 1]).GetPath() : ((Node)RelicNodes[i]).GetPath());
			((Control)nRelicInventoryHolder).FocusNeighborRight = ((i < RelicNodes.Count - 1) ? ((Node)RelicNodes[i + 1]).GetPath() : ((Node)RelicNodes[i]).GetPath());
			Control firstPotionControl = NRun.Instance.GlobalUi.TopBar.PotionContainer.FirstPotionControl;
			((Control)nRelicInventoryHolder).FocusNeighborTop = ((firstPotionControl != null && GodotObject.IsInstanceValid((GodotObject)(object)firstPotionControl)) ? ((Node)firstPotionControl).GetPath() : null);
			NMultiplayerPlayerStateContainer multiplayerPlayerContainer = NRun.Instance.GlobalUi.MultiplayerPlayerContainer;
			if (((Node)multiplayerPlayerContainer).GetChildCount(false) > 0)
			{
				Control val = (Control)(object)multiplayerPlayerContainer.FirstPlayerState?.Hitbox;
				((Control)nRelicInventoryHolder).FocusNeighborBottom = ((val != null && GodotObject.IsInstanceValid((GodotObject)(object)val)) ? ((Node)val).GetPath() : null);
			}
			else
			{
				Control val2 = ActiveScreenContext.Instance.GetCurrentScreen()?.FocusedControlFromTopBar;
				((Control)nRelicInventoryHolder).FocusNeighborBottom = ((val2 != null && GodotObject.IsInstanceValid((GodotObject)(object)val2)) ? ((Node)val2).GetPath() : null);
			}
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
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Expected O, but got Unknown
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_0289: Unknown result type (might be due to invalid IL or missing references)
		//IL_0292: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(13);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._EnterTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._ExitTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ConnectPlayerEvents, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.DisconnectPlayerEvents, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnRelicUnfocused, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)33, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AnimShow, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AnimHide, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ShowImmediately, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.HideImmediately, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Input, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("inputEvent"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("InputEvent"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.DebugHideTopBar, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UpdateNavigation, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
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
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
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
		if ((ref method) == MethodName.ConnectPlayerEvents && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			ConnectPlayerEvents();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.DisconnectPlayerEvents && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			DisconnectPlayerEvents();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnRelicUnfocused && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnRelicUnfocused();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.AnimShow && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			AnimShow();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.AnimHide && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			AnimHide();
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
		if ((ref method) == MethodName.UpdateNavigation && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			UpdateNavigation();
			ret = default(godot_variant);
			return true;
		}
		return ((FlowContainer)this).InvokeGodotClassMethod(ref method, args, ref ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static bool InvokeGodotClassStaticMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.OnRelicUnfocused && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnRelicUnfocused();
			ret = default(godot_variant);
			return true;
		}
		ret = default(godot_variant);
		return false;
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
		if ((ref method) == MethodName.ConnectPlayerEvents)
		{
			return true;
		}
		if ((ref method) == MethodName.DisconnectPlayerEvents)
		{
			return true;
		}
		if ((ref method) == MethodName.OnRelicUnfocused)
		{
			return true;
		}
		if ((ref method) == MethodName.AnimShow)
		{
			return true;
		}
		if ((ref method) == MethodName.AnimHide)
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
		if ((ref method) == MethodName._Input)
		{
			return true;
		}
		if ((ref method) == MethodName.DebugHideTopBar)
		{
			return true;
		}
		if ((ref method) == MethodName.UpdateNavigation)
		{
			return true;
		}
		return ((FlowContainer)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName._originalPos)
		{
			_originalPos = VariantUtils.ConvertTo<Vector2>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._curTween)
		{
			_curTween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._debugHideTween)
		{
			_debugHideTween = VariantUtils.ConvertTo<Tween>(ref value);
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
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName._originalPos)
		{
			value = VariantUtils.CreateFrom<Vector2>(ref _originalPos);
			return true;
		}
		if ((ref name) == PropertyName._curTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _curTween);
			return true;
		}
		if ((ref name) == PropertyName._debugHideTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _debugHideTween);
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
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)5, PropertyName._originalPos, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._curTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._debugHideTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._isDebugHidden, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		info.AddProperty(PropertyName._originalPos, Variant.From<Vector2>(ref _originalPos));
		info.AddProperty(PropertyName._curTween, Variant.From<Tween>(ref _curTween));
		info.AddProperty(PropertyName._debugHideTween, Variant.From<Tween>(ref _debugHideTween));
		info.AddProperty(PropertyName._isDebugHidden, Variant.From<bool>(ref _isDebugHidden));
		info.AddSignalEventDelegate(SignalName.RelicsChanged, (Delegate)backing_RelicsChanged);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._originalPos, ref val))
		{
			_originalPos = ((Variant)(ref val)).As<Vector2>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._curTween, ref val2))
		{
			_curTween = ((Variant)(ref val2)).As<Tween>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._debugHideTween, ref val3))
		{
			_debugHideTween = ((Variant)(ref val3)).As<Tween>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._isDebugHidden, ref val4))
		{
			_isDebugHidden = ((Variant)(ref val4)).As<bool>();
		}
		RelicsChangedEventHandler relicsChangedEventHandler = default(RelicsChangedEventHandler);
		if (info.TryGetSignalEventDelegate<RelicsChangedEventHandler>(SignalName.RelicsChanged, ref relicsChangedEventHandler))
		{
			backing_RelicsChanged = relicsChangedEventHandler;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotSignalList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(1);
		list.Add(new MethodInfo(SignalName.RelicsChanged, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	protected void EmitSignalRelicsChanged()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).EmitSignal(SignalName.RelicsChanged, Array.Empty<Variant>());
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RaiseGodotClassSignalCallbacks(in godot_string_name signal, NativeVariantPtrArgs args)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		if ((ref signal) == SignalName.RelicsChanged && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			backing_RelicsChanged?.Invoke();
		}
		else
		{
			((GodotObject)this).RaiseGodotClassSignalCallbacks(ref signal, args);
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassSignal(in godot_string_name signal)
	{
		if ((ref signal) == SignalName.RelicsChanged)
		{
			return true;
		}
		return ((FlowContainer)this).HasGodotClassSignal(ref signal);
	}
}
