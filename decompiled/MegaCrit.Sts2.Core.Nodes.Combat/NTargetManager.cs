using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.ControllerInput;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.HoverTips;
using MegaCrit.Sts2.Core.Nodes.Multiplayer;
using MegaCrit.Sts2.Core.Nodes.RestSite;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;

namespace MegaCrit.Sts2.Core.Nodes.Combat;

[ScriptPath("res://src/Core/Nodes/Combat/NTargetManager.cs")]
public class NTargetManager : Node2D
{
	[Signal]
	public delegate void CreatureHoveredEventHandler(NCreature creature);

	[Signal]
	public delegate void CreatureUnhoveredEventHandler(NCreature creature);

	[Signal]
	public delegate void NodeHoveredEventHandler(Node node);

	[Signal]
	public delegate void NodeUnhoveredEventHandler(Node node);

	[Signal]
	public delegate void TargetingBeganEventHandler();

	[Signal]
	public delegate void TargetingEndedEventHandler();

	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName _EnterTree = StringName.op_Implicit("_EnterTree");

		public static readonly StringName _ExitTree = StringName.op_Implicit("_ExitTree");

		public static readonly StringName _Input = StringName.op_Implicit("_Input");

		public static readonly StringName _Process = StringName.op_Implicit("_Process");

		public static readonly StringName CancelTargeting = StringName.op_Implicit("CancelTargeting");

		public static readonly StringName FinishTargeting = StringName.op_Implicit("FinishTargeting");

		public static readonly StringName AllowedToTargetNode = StringName.op_Implicit("AllowedToTargetNode");

		public static readonly StringName OnNodeHovered = StringName.op_Implicit("OnNodeHovered");

		public static readonly StringName OnNodeUnhovered = StringName.op_Implicit("OnNodeUnhovered");

		public static readonly StringName OnCreatureHovered = StringName.op_Implicit("OnCreatureHovered");

		public static readonly StringName OnCreatureUnhovered = StringName.op_Implicit("OnCreatureUnhovered");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName IsInSelection = StringName.op_Implicit("IsInSelection");

		public static readonly StringName HoveredNode = StringName.op_Implicit("HoveredNode");

		public static readonly StringName LastTargetingFinishedFrame = StringName.op_Implicit("LastTargetingFinishedFrame");

		public static readonly StringName _targetingArrow = StringName.op_Implicit("_targetingArrow");

		public static readonly StringName _targetMode = StringName.op_Implicit("_targetMode");

		public static readonly StringName _validTargetsType = StringName.op_Implicit("_validTargetsType");
	}

	public class SignalName : SignalName
	{
		public static readonly StringName CreatureHovered = StringName.op_Implicit("CreatureHovered");

		public static readonly StringName CreatureUnhovered = StringName.op_Implicit("CreatureUnhovered");

		public static readonly StringName NodeHovered = StringName.op_Implicit("NodeHovered");

		public static readonly StringName NodeUnhovered = StringName.op_Implicit("NodeUnhovered");

		public static readonly StringName TargetingBegan = StringName.op_Implicit("TargetingBegan");

		public static readonly StringName TargetingEnded = StringName.op_Implicit("TargetingEnded");
	}

	private NTargetingArrow _targetingArrow;

	private TaskCompletionSource<Node?>? _completionSource;

	private Func<bool>? _exitEarlyCondition;

	private Func<Node, bool>? _nodeFilter;

	private TargetMode _targetMode;

	private TargetType _validTargetsType;

	private CreatureHoveredEventHandler backing_CreatureHovered;

	private CreatureUnhoveredEventHandler backing_CreatureUnhovered;

	private NodeHoveredEventHandler backing_NodeHovered;

	private NodeUnhoveredEventHandler backing_NodeUnhovered;

	private TargetingBeganEventHandler backing_TargetingBegan;

	private TargetingEndedEventHandler backing_TargetingEnded;

	public static NTargetManager Instance => NRun.Instance.GlobalUi.TargetManager;

	public bool IsInSelection => _targetMode != TargetMode.None;

	private Node? HoveredNode { get; set; }

	public long LastTargetingFinishedFrame { get; set; }

	public event CreatureHoveredEventHandler CreatureHovered
	{
		add
		{
			backing_CreatureHovered = (CreatureHoveredEventHandler)Delegate.Combine(backing_CreatureHovered, value);
		}
		remove
		{
			backing_CreatureHovered = (CreatureHoveredEventHandler)Delegate.Remove(backing_CreatureHovered, value);
		}
	}

	public event CreatureUnhoveredEventHandler CreatureUnhovered
	{
		add
		{
			backing_CreatureUnhovered = (CreatureUnhoveredEventHandler)Delegate.Combine(backing_CreatureUnhovered, value);
		}
		remove
		{
			backing_CreatureUnhovered = (CreatureUnhoveredEventHandler)Delegate.Remove(backing_CreatureUnhovered, value);
		}
	}

	public event NodeHoveredEventHandler NodeHovered
	{
		add
		{
			backing_NodeHovered = (NodeHoveredEventHandler)Delegate.Combine(backing_NodeHovered, value);
		}
		remove
		{
			backing_NodeHovered = (NodeHoveredEventHandler)Delegate.Remove(backing_NodeHovered, value);
		}
	}

	public event NodeUnhoveredEventHandler NodeUnhovered
	{
		add
		{
			backing_NodeUnhovered = (NodeUnhoveredEventHandler)Delegate.Combine(backing_NodeUnhovered, value);
		}
		remove
		{
			backing_NodeUnhovered = (NodeUnhoveredEventHandler)Delegate.Remove(backing_NodeUnhovered, value);
		}
	}

	public event TargetingBeganEventHandler TargetingBegan
	{
		add
		{
			backing_TargetingBegan = (TargetingBeganEventHandler)Delegate.Combine(backing_TargetingBegan, value);
		}
		remove
		{
			backing_TargetingBegan = (TargetingBeganEventHandler)Delegate.Remove(backing_TargetingBegan, value);
		}
	}

	public event TargetingEndedEventHandler TargetingEnded
	{
		add
		{
			backing_TargetingEnded = (TargetingEndedEventHandler)Delegate.Combine(backing_TargetingEnded, value);
		}
		remove
		{
			backing_TargetingEnded = (TargetingEndedEventHandler)Delegate.Remove(backing_TargetingEnded, value);
		}
	}

	public override void _Ready()
	{
		_targetingArrow = ((Node)this).GetNode<NTargetingArrow>(NodePath.op_Implicit("TargetingArrow"));
	}

	public override void _EnterTree()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		((Node)this)._EnterTree();
		if (NControllerManager.Instance != null)
		{
			((GodotObject)NControllerManager.Instance).Connect(NControllerManager.SignalName.MouseDetected, Callable.From((Action)CancelTargeting), 0u);
			((GodotObject)NControllerManager.Instance).Connect(NControllerManager.SignalName.ControllerDetected, Callable.From((Action)CancelTargeting), 0u);
		}
		CombatManager.Instance.CombatEnded += OnCombatEnded;
	}

	public override void _ExitTree()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		((Node)this)._ExitTree();
		if (NControllerManager.Instance != null)
		{
			((GodotObject)NControllerManager.Instance).Disconnect(NControllerManager.SignalName.MouseDetected, Callable.From((Action)CancelTargeting));
			((GodotObject)NControllerManager.Instance).Disconnect(NControllerManager.SignalName.ControllerDetected, Callable.From((Action)CancelTargeting));
		}
		CombatManager.Instance.CombatEnded -= OnCombatEnded;
	}

	public override void _Input(InputEvent inputEvent)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Invalid comparison between Unknown and I8
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Invalid comparison between Unknown and I8
		if (!IsInSelection)
		{
			return;
		}
		bool flag = false;
		bool cancel = false;
		InputEventMouseButton val = (InputEventMouseButton)(object)((inputEvent is InputEventMouseButton) ? inputEvent : null);
		if (val != null)
		{
			MouseButton buttonIndex = val.ButtonIndex;
			if ((long)buttonIndex != 1)
			{
				if ((long)buttonIndex == 2)
				{
					flag = ((InputEvent)val).IsPressed();
					cancel = true;
				}
			}
			else if (((InputEvent)val).IsReleased())
			{
				switch (_targetMode)
				{
				case TargetMode.ReleaseMouseToTarget:
					if (HoveredNode != null)
					{
						flag = true;
					}
					else
					{
						_targetMode = TargetMode.ClickMouseToTarget;
					}
					break;
				case TargetMode.ClickMouseToTarget:
					flag = true;
					break;
				}
			}
		}
		else if (inputEvent.IsActionPressed(MegaInput.select, false, false) && HoveredNode != null)
		{
			flag = true;
			cancel = false;
			((Node)this).GetViewport().SetInputAsHandled();
		}
		else if (inputEvent.IsActionPressed(MegaInput.cancel, false, false) || inputEvent.IsActionPressed(MegaInput.topPanel, false, false))
		{
			flag = true;
			cancel = true;
			((Node)this).GetViewport().SetInputAsHandled();
		}
		if (_exitEarlyCondition != null && _exitEarlyCondition())
		{
			flag = true;
			cancel = true;
		}
		if (flag)
		{
			FinishTargeting(cancel);
		}
	}

	public override void _Process(double delta)
	{
		if (_exitEarlyCondition != null && _exitEarlyCondition())
		{
			FinishTargeting(cancel: true);
		}
		if (HoveredNode is NCreature nCreature)
		{
			Creature entity = nCreature.Entity;
			if (entity != null && !entity.IsHittable && _targetMode == TargetMode.Controller)
			{
				FinishTargeting(cancel: true);
			}
		}
	}

	private void OnCombatEnded(CombatRoom _)
	{
		if (_exitEarlyCondition != null)
		{
			FinishTargeting(cancel: true);
		}
	}

	public void CancelTargeting()
	{
		if (_targetMode != TargetMode.None)
		{
			FinishTargeting(cancel: true);
		}
	}

	private void FinishTargeting(bool cancel)
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		NHoverTipSet.shouldBlockHoverTips = false;
		_exitEarlyCondition = null;
		_completionSource.SetResult(cancel ? null : HoveredNode);
		LastTargetingFinishedFrame = ((Node)this).GetTree().GetFrame();
		((GodotObject)this).EmitSignal(SignalName.TargetingEnded, Array.Empty<Variant>());
		_targetMode = TargetMode.None;
		_targetingArrow.StopDrawing();
		if (HoveredNode is NCreature nCreature)
		{
			nCreature.HideMultiselectReticle();
		}
		else if (HoveredNode is NRestSiteCharacter nRestSiteCharacter)
		{
			nRestSiteCharacter.Deselect();
		}
		HoveredNode = null;
		RunManager.Instance.InputSynchronizer.SyncLocalIsTargeting(isTargeting: false);
	}

	public async Task<Node?> SelectionFinished()
	{
		return await _completionSource.Task;
	}

	public void StartTargeting(TargetType validTargetsType, Vector2 startPosition, TargetMode startingMode, Func<bool>? exitEarlyCondition, Func<Node, bool>? nodeFilter)
	{
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		if (!validTargetsType.IsSingleTarget())
		{
			throw new InvalidOperationException($"Tried to begin targeting with invalid ActionTarget {validTargetsType}!");
		}
		_validTargetsType = validTargetsType;
		_targetingArrow.StartDrawingFrom(startPosition, startingMode == TargetMode.Controller);
		_completionSource = new TaskCompletionSource<Node>();
		_exitEarlyCondition = exitEarlyCondition;
		_nodeFilter = nodeFilter;
		_targetMode = startingMode;
		NHoverTipSet.shouldBlockHoverTips = true;
		((GodotObject)this).EmitSignal(SignalName.TargetingBegan, Array.Empty<Variant>());
		RunManager.Instance.InputSynchronizer.SyncLocalIsTargeting(isTargeting: true);
		foreach (NCreature item in NCombatRoom.Instance?.CreatureNodes ?? Array.Empty<NCreature>())
		{
			item.OnTargetingStarted();
		}
	}

	public void StartTargeting(TargetType validTargetsType, Control control, TargetMode startingMode, Func<bool>? exitEarlyCondition, Func<Node, bool>? nodeFilter)
	{
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		if (!validTargetsType.IsSingleTarget())
		{
			throw new InvalidOperationException($"Tried to begin targeting with invalid ActionTarget {validTargetsType}!");
		}
		_validTargetsType = validTargetsType;
		_targetingArrow.StartDrawingFrom(control, startingMode == TargetMode.Controller);
		_completionSource = new TaskCompletionSource<Node>();
		_exitEarlyCondition = exitEarlyCondition;
		_nodeFilter = nodeFilter;
		_targetMode = startingMode;
		NHoverTipSet.shouldBlockHoverTips = true;
		((GodotObject)this).EmitSignal(SignalName.TargetingBegan, Array.Empty<Variant>());
		RunManager.Instance.InputSynchronizer.SyncLocalIsTargeting(isTargeting: true);
		foreach (NCreature item in NCombatRoom.Instance?.CreatureNodes ?? Array.Empty<NCreature>())
		{
			item.OnTargetingStarted();
		}
	}

	public bool AllowedToTargetNode(Node node)
	{
		if (_nodeFilter != null && !_nodeFilter(node))
		{
			return false;
		}
		if (node is NCreature nCreature)
		{
			return AllowedToTargetCreature(nCreature.Entity);
		}
		if (node is NMultiplayerPlayerState nMultiplayerPlayerState)
		{
			return AllowedToTargetCreature(nMultiplayerPlayerState.Player.Creature);
		}
		return true;
	}

	private bool AllowedToTargetCreature(Creature creature)
	{
		switch (_validTargetsType)
		{
		case TargetType.AnyEnemy:
			if (creature.Side != CombatSide.Enemy)
			{
				return false;
			}
			break;
		case TargetType.AnyPlayer:
			if (!creature.IsPlayer || creature.IsDead)
			{
				return false;
			}
			break;
		case TargetType.AnyAlly:
			if (!creature.IsPlayer || creature.IsDead)
			{
				return false;
			}
			if (LocalContext.IsMe(creature.Player))
			{
				return false;
			}
			break;
		default:
			throw new ArgumentOutOfRangeException("_validTargetsType", _validTargetsType, null);
		}
		return true;
	}

	public void OnNodeHovered(Node node)
	{
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		if (!IsInSelection || !AllowedToTargetNode(node))
		{
			return;
		}
		if (node is NCreature creature)
		{
			OnCreatureHovered(creature);
			return;
		}
		HoveredNode = node;
		_targetingArrow.SetHighlightingOn(isEnemy: false);
		if (_targetMode == TargetMode.Controller)
		{
			if (!(node is NMultiplayerPlayerState nMultiplayerPlayerState))
			{
				Control val = (Control)(object)((node is Control) ? node : null);
				if (val == null)
				{
					Node2D val2 = (Node2D)(object)((node is Node2D) ? node : null);
					if (val2 != null)
					{
						_targetingArrow.UpdateDrawingTo(val2.GlobalPosition);
					}
				}
				else
				{
					_targetingArrow.UpdateDrawingTo(val.GlobalPosition + val.PivotOffset);
				}
			}
			else
			{
				_targetingArrow.UpdateDrawingTo(((Control)nMultiplayerPlayerState).GlobalPosition + Vector2.Right * ((Control)nMultiplayerPlayerState.Hitbox).Size.X + Vector2.Down * ((Control)nMultiplayerPlayerState.Hitbox).Size.Y / 2f);
			}
		}
		((GodotObject)this).EmitSignal(SignalName.NodeHovered, (Variant[])(object)new Variant[1] { Variant.op_Implicit((GodotObject)(object)node) });
	}

	public void OnNodeUnhovered(Node node)
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		if (IsInSelection && AllowedToTargetNode(node))
		{
			if (node is NCreature creature)
			{
				OnCreatureUnhovered(creature);
				return;
			}
			HoveredNode = null;
			_targetingArrow.SetHighlightingOff();
			((GodotObject)this).EmitSignal(SignalName.NodeUnhovered, (Variant[])(object)new Variant[1] { Variant.op_Implicit((GodotObject)(object)node) });
		}
	}

	private void OnCreatureHovered(NCreature creature)
	{
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		if (Hook.ShouldAllowTargeting(creature.Entity.CombatState, creature.Entity, out AbstractModel preventer))
		{
			HoveredNode = (Node?)(object)creature;
			_targetingArrow.SetHighlightingOn(creature.Entity.IsEnemy);
			creature.ShowSingleSelectReticle();
			((GodotObject)this).EmitSignal(SignalName.CreatureHovered, (Variant[])(object)new Variant[1] { Variant.op_Implicit((GodotObject)(object)creature) });
			if (_targetMode == TargetMode.Controller)
			{
				_targetingArrow.UpdateDrawingTo(creature.VfxSpawnPosition);
			}
		}
		else
		{
			TaskHelper.RunSafely(preventer.AfterTargetingBlockedVfx(creature.Entity));
		}
	}

	private void OnCreatureUnhovered(NCreature creature)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).EmitSignal(SignalName.CreatureUnhovered, (Variant[])(object)new Variant[1] { Variant.op_Implicit((GodotObject)(object)creature) });
		if ((object)HoveredNode == creature)
		{
			HoveredNode = null;
		}
		_targetingArrow.SetHighlightingOff();
		if (_targetMode != TargetMode.None)
		{
			creature.HideSingleSelectReticle();
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
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Expected O, but got Unknown
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Expected O, but got Unknown
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0279: Expected O, but got Unknown
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d8: Expected O, but got Unknown
		//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02de: Unknown result type (might be due to invalid IL or missing references)
		//IL_0304: Unknown result type (might be due to invalid IL or missing references)
		//IL_032c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0337: Expected O, but got Unknown
		//IL_0332: Unknown result type (might be due to invalid IL or missing references)
		//IL_033d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0363: Unknown result type (might be due to invalid IL or missing references)
		//IL_038b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0396: Expected O, but got Unknown
		//IL_0391: Unknown result type (might be due to invalid IL or missing references)
		//IL_039c: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(12);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._EnterTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._ExitTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Input, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("inputEvent"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("InputEvent"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Process, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)3, StringName.op_Implicit("delta"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.CancelTargeting, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.FinishTargeting, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)1, StringName.op_Implicit("cancel"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AllowedToTargetNode, new PropertyInfo((Type)1, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("node"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Node"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnNodeHovered, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("node"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Node"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnNodeUnhovered, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("node"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Node"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnCreatureHovered, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("creature"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnCreatureUnhovered, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("creature"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
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
		if ((ref method) == MethodName._Process && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			((Node)this)._Process(VariantUtils.ConvertTo<double>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.CancelTargeting && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			CancelTargeting();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.FinishTargeting && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			FinishTargeting(VariantUtils.ConvertTo<bool>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.AllowedToTargetNode && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			bool flag = AllowedToTargetNode(VariantUtils.ConvertTo<Node>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<bool>(ref flag);
			return true;
		}
		if ((ref method) == MethodName.OnNodeHovered && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OnNodeHovered(VariantUtils.ConvertTo<Node>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnNodeUnhovered && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OnNodeUnhovered(VariantUtils.ConvertTo<Node>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnCreatureHovered && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OnCreatureHovered(VariantUtils.ConvertTo<NCreature>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnCreatureUnhovered && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OnCreatureUnhovered(VariantUtils.ConvertTo<NCreature>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		return ((Node2D)this).InvokeGodotClassMethod(ref method, args, ref ret);
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
		if ((ref method) == MethodName._Process)
		{
			return true;
		}
		if ((ref method) == MethodName.CancelTargeting)
		{
			return true;
		}
		if ((ref method) == MethodName.FinishTargeting)
		{
			return true;
		}
		if ((ref method) == MethodName.AllowedToTargetNode)
		{
			return true;
		}
		if ((ref method) == MethodName.OnNodeHovered)
		{
			return true;
		}
		if ((ref method) == MethodName.OnNodeUnhovered)
		{
			return true;
		}
		if ((ref method) == MethodName.OnCreatureHovered)
		{
			return true;
		}
		if ((ref method) == MethodName.OnCreatureUnhovered)
		{
			return true;
		}
		return ((Node2D)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName.HoveredNode)
		{
			HoveredNode = VariantUtils.ConvertTo<Node>(ref value);
			return true;
		}
		if ((ref name) == PropertyName.LastTargetingFinishedFrame)
		{
			LastTargetingFinishedFrame = VariantUtils.ConvertTo<long>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._targetingArrow)
		{
			_targetingArrow = VariantUtils.ConvertTo<NTargetingArrow>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._targetMode)
		{
			_targetMode = VariantUtils.ConvertTo<TargetMode>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._validTargetsType)
		{
			_validTargetsType = VariantUtils.ConvertTo<TargetType>(ref value);
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
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName.IsInSelection)
		{
			bool isInSelection = IsInSelection;
			value = VariantUtils.CreateFrom<bool>(ref isInSelection);
			return true;
		}
		if ((ref name) == PropertyName.HoveredNode)
		{
			Node hoveredNode = HoveredNode;
			value = VariantUtils.CreateFrom<Node>(ref hoveredNode);
			return true;
		}
		if ((ref name) == PropertyName.LastTargetingFinishedFrame)
		{
			long lastTargetingFinishedFrame = LastTargetingFinishedFrame;
			value = VariantUtils.CreateFrom<long>(ref lastTargetingFinishedFrame);
			return true;
		}
		if ((ref name) == PropertyName._targetingArrow)
		{
			value = VariantUtils.CreateFrom<NTargetingArrow>(ref _targetingArrow);
			return true;
		}
		if ((ref name) == PropertyName._targetMode)
		{
			value = VariantUtils.CreateFrom<TargetMode>(ref _targetMode);
			return true;
		}
		if ((ref name) == PropertyName._validTargetsType)
		{
			value = VariantUtils.CreateFrom<TargetType>(ref _validTargetsType);
			return true;
		}
		return ((GodotObject)this).GetGodotClassPropertyValue(ref name, ref value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._targetingArrow, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName.IsInSelection, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)2, PropertyName._targetMode, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)2, PropertyName._validTargetsType, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.HoveredNode, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)2, PropertyName.LastTargetingFinishedFrame, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		StringName hoveredNode = PropertyName.HoveredNode;
		Node hoveredNode2 = HoveredNode;
		info.AddProperty(hoveredNode, Variant.From<Node>(ref hoveredNode2));
		StringName lastTargetingFinishedFrame = PropertyName.LastTargetingFinishedFrame;
		long lastTargetingFinishedFrame2 = LastTargetingFinishedFrame;
		info.AddProperty(lastTargetingFinishedFrame, Variant.From<long>(ref lastTargetingFinishedFrame2));
		info.AddProperty(PropertyName._targetingArrow, Variant.From<NTargetingArrow>(ref _targetingArrow));
		info.AddProperty(PropertyName._targetMode, Variant.From<TargetMode>(ref _targetMode));
		info.AddProperty(PropertyName._validTargetsType, Variant.From<TargetType>(ref _validTargetsType));
		info.AddSignalEventDelegate(SignalName.CreatureHovered, (Delegate)backing_CreatureHovered);
		info.AddSignalEventDelegate(SignalName.CreatureUnhovered, (Delegate)backing_CreatureUnhovered);
		info.AddSignalEventDelegate(SignalName.NodeHovered, (Delegate)backing_NodeHovered);
		info.AddSignalEventDelegate(SignalName.NodeUnhovered, (Delegate)backing_NodeUnhovered);
		info.AddSignalEventDelegate(SignalName.TargetingBegan, (Delegate)backing_TargetingBegan);
		info.AddSignalEventDelegate(SignalName.TargetingEnded, (Delegate)backing_TargetingEnded);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName.HoveredNode, ref val))
		{
			HoveredNode = ((Variant)(ref val)).As<Node>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName.LastTargetingFinishedFrame, ref val2))
		{
			LastTargetingFinishedFrame = ((Variant)(ref val2)).As<long>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._targetingArrow, ref val3))
		{
			_targetingArrow = ((Variant)(ref val3)).As<NTargetingArrow>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._targetMode, ref val4))
		{
			_targetMode = ((Variant)(ref val4)).As<TargetMode>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._validTargetsType, ref val5))
		{
			_validTargetsType = ((Variant)(ref val5)).As<TargetType>();
		}
		CreatureHoveredEventHandler creatureHoveredEventHandler = default(CreatureHoveredEventHandler);
		if (info.TryGetSignalEventDelegate<CreatureHoveredEventHandler>(SignalName.CreatureHovered, ref creatureHoveredEventHandler))
		{
			backing_CreatureHovered = creatureHoveredEventHandler;
		}
		CreatureUnhoveredEventHandler creatureUnhoveredEventHandler = default(CreatureUnhoveredEventHandler);
		if (info.TryGetSignalEventDelegate<CreatureUnhoveredEventHandler>(SignalName.CreatureUnhovered, ref creatureUnhoveredEventHandler))
		{
			backing_CreatureUnhovered = creatureUnhoveredEventHandler;
		}
		NodeHoveredEventHandler nodeHoveredEventHandler = default(NodeHoveredEventHandler);
		if (info.TryGetSignalEventDelegate<NodeHoveredEventHandler>(SignalName.NodeHovered, ref nodeHoveredEventHandler))
		{
			backing_NodeHovered = nodeHoveredEventHandler;
		}
		NodeUnhoveredEventHandler nodeUnhoveredEventHandler = default(NodeUnhoveredEventHandler);
		if (info.TryGetSignalEventDelegate<NodeUnhoveredEventHandler>(SignalName.NodeUnhovered, ref nodeUnhoveredEventHandler))
		{
			backing_NodeUnhovered = nodeUnhoveredEventHandler;
		}
		TargetingBeganEventHandler targetingBeganEventHandler = default(TargetingBeganEventHandler);
		if (info.TryGetSignalEventDelegate<TargetingBeganEventHandler>(SignalName.TargetingBegan, ref targetingBeganEventHandler))
		{
			backing_TargetingBegan = targetingBeganEventHandler;
		}
		TargetingEndedEventHandler targetingEndedEventHandler = default(TargetingEndedEventHandler);
		if (info.TryGetSignalEventDelegate<TargetingEndedEventHandler>(SignalName.TargetingEnded, ref targetingEndedEventHandler))
		{
			backing_TargetingEnded = targetingEndedEventHandler;
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
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Expected O, but got Unknown
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(6);
		list.Add(new MethodInfo(SignalName.CreatureHovered, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("creature"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(SignalName.CreatureUnhovered, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("creature"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(SignalName.NodeHovered, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("node"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Node"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(SignalName.NodeUnhovered, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("node"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Node"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(SignalName.TargetingBegan, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(SignalName.TargetingEnded, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	protected void EmitSignalCreatureHovered(NCreature creature)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).EmitSignal(SignalName.CreatureHovered, (Variant[])(object)new Variant[1] { Variant.op_Implicit((GodotObject)(object)creature) });
	}

	protected void EmitSignalCreatureUnhovered(NCreature creature)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).EmitSignal(SignalName.CreatureUnhovered, (Variant[])(object)new Variant[1] { Variant.op_Implicit((GodotObject)(object)creature) });
	}

	protected void EmitSignalNodeHovered(Node node)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).EmitSignal(SignalName.NodeHovered, (Variant[])(object)new Variant[1] { Variant.op_Implicit((GodotObject)(object)node) });
	}

	protected void EmitSignalNodeUnhovered(Node node)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).EmitSignal(SignalName.NodeUnhovered, (Variant[])(object)new Variant[1] { Variant.op_Implicit((GodotObject)(object)node) });
	}

	protected void EmitSignalTargetingBegan()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).EmitSignal(SignalName.TargetingBegan, Array.Empty<Variant>());
	}

	protected void EmitSignalTargetingEnded()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).EmitSignal(SignalName.TargetingEnded, Array.Empty<Variant>());
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RaiseGodotClassSignalCallbacks(in godot_string_name signal, NativeVariantPtrArgs args)
	{
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		if ((ref signal) == SignalName.CreatureHovered && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			backing_CreatureHovered?.Invoke(VariantUtils.ConvertTo<NCreature>(ref ((NativeVariantPtrArgs)(ref args))[0]));
		}
		else if ((ref signal) == SignalName.CreatureUnhovered && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			backing_CreatureUnhovered?.Invoke(VariantUtils.ConvertTo<NCreature>(ref ((NativeVariantPtrArgs)(ref args))[0]));
		}
		else if ((ref signal) == SignalName.NodeHovered && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			backing_NodeHovered?.Invoke(VariantUtils.ConvertTo<Node>(ref ((NativeVariantPtrArgs)(ref args))[0]));
		}
		else if ((ref signal) == SignalName.NodeUnhovered && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			backing_NodeUnhovered?.Invoke(VariantUtils.ConvertTo<Node>(ref ((NativeVariantPtrArgs)(ref args))[0]));
		}
		else if ((ref signal) == SignalName.TargetingBegan && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			backing_TargetingBegan?.Invoke();
		}
		else if ((ref signal) == SignalName.TargetingEnded && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			backing_TargetingEnded?.Invoke();
		}
		else
		{
			((GodotObject)this).RaiseGodotClassSignalCallbacks(ref signal, args);
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassSignal(in godot_string_name signal)
	{
		if ((ref signal) == SignalName.CreatureHovered)
		{
			return true;
		}
		if ((ref signal) == SignalName.CreatureUnhovered)
		{
			return true;
		}
		if ((ref signal) == SignalName.NodeHovered)
		{
			return true;
		}
		if ((ref signal) == SignalName.NodeUnhovered)
		{
			return true;
		}
		if ((ref signal) == SignalName.TargetingBegan)
		{
			return true;
		}
		if ((ref signal) == SignalName.TargetingEnded)
		{
			return true;
		}
		return ((Node2D)this).HasGodotClassSignal(ref signal);
	}
}
