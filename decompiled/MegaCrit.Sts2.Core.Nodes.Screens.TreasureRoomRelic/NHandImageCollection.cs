using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.TreasureRelicPicking;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Multiplayer.Game.PeerInput;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Runs;

namespace MegaCrit.Sts2.Core.Nodes.Screens.TreasureRoomRelic;

[ScriptPath("res://src/Core/Nodes/Screens/TreasureRoomRelic/NHandImageCollection.cs")]
public class NHandImageCollection : Control
{
	public class MethodName : MethodName
	{
		public static readonly StringName _EnterTree = StringName.op_Implicit("_EnterTree");

		public static readonly StringName _ExitTree = StringName.op_Implicit("_ExitTree");

		public static readonly StringName OnInputStateAdded = StringName.op_Implicit("OnInputStateAdded");

		public static readonly StringName OnInputStateRemoved = StringName.op_Implicit("OnInputStateRemoved");

		public static readonly StringName AddHand = StringName.op_Implicit("AddHand");

		public static readonly StringName OnInputStateChanged = StringName.op_Implicit("OnInputStateChanged");

		public static readonly StringName _Input = StringName.op_Implicit("_Input");

		public static readonly StringName ProcessGuiFocus = StringName.op_Implicit("ProcessGuiFocus");

		public static readonly StringName GetHand = StringName.op_Implicit("GetHand");

		public static readonly StringName RemoveHand = StringName.op_Implicit("RemoveHand");

		public static readonly StringName UpdateHandVisibility = StringName.op_Implicit("UpdateHandVisibility");

		public static readonly StringName BeforeRelicsAwarded = StringName.op_Implicit("BeforeRelicsAwarded");

		public static readonly StringName AnimateHandsIn = StringName.op_Implicit("AnimateHandsIn");

		public static readonly StringName AnimateHandsAway = StringName.op_Implicit("AnimateHandsAway");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName _handAnimateInProgress = StringName.op_Implicit("_handAnimateInProgress");
	}

	public class SignalName : SignalName
	{
	}

	private PeerInputSynchronizer? _synchronizer;

	private readonly List<NHandImage> _hands = new List<NHandImage>();

	private IRunState _runState = NullRunState.Instance;

	private float _handAnimateInProgress;

	public override void _EnterTree()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		((Node)this)._EnterTree();
		((GodotObject)((Node)this).GetViewport()).Connect(SignalName.GuiFocusChanged, Callable.From<Control>((Action<Control>)ProcessGuiFocus), 0u);
	}

	public override void _ExitTree()
	{
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		if (_synchronizer != null)
		{
			_synchronizer.StateAdded -= OnInputStateAdded;
			_synchronizer.StateChanged -= OnInputStateChanged;
			_synchronizer.StateRemoved -= OnInputStateRemoved;
		}
		NGame.Instance.CursorManager.SetCursorShown(show: true);
		((GodotObject)((Node)this).GetViewport()).Disconnect(SignalName.GuiFocusChanged, Callable.From<Control>((Action<Control>)ProcessGuiFocus));
	}

	public void Initialize(IRunState runState)
	{
		_runState = runState;
		if (_runState.Players.Count <= 1)
		{
			return;
		}
		_synchronizer = RunManager.Instance.InputSynchronizer;
		_synchronizer.StateAdded -= OnInputStateAdded;
		_synchronizer.StateChanged += OnInputStateChanged;
		_synchronizer.StateRemoved += OnInputStateRemoved;
		foreach (Player player in _runState.Players)
		{
			AddHand(player.NetId);
		}
		UpdateHandVisibility();
	}

	private void OnInputStateAdded(ulong playerId)
	{
		AddHand(playerId);
	}

	private void OnInputStateRemoved(ulong playerId)
	{
		RemoveHand(playerId);
	}

	private void AddHand(ulong playerId)
	{
		if (_hands.Any((NHandImage c) => c.Player.NetId == playerId))
		{
			Log.Error($"Tried to add hand for player {playerId} twice!");
		}
		else
		{
			Player player = _runState.GetPlayer(playerId);
			NHandImage nHandImage = NHandImage.Create(player, _runState.GetPlayerSlotIndex(player));
			_hands.Add(nHandImage);
			((Node)(object)this).AddChildSafely((Node?)(object)nHandImage);
		}
	}

	private void OnInputStateChanged(ulong playerId)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		Vector2 controlSpaceFocusPosition = _synchronizer.GetControlSpaceFocusPosition(playerId, (Control)(object)this);
		NHandImage hand = GetHand(playerId);
		hand.SetIsDown(_synchronizer.GetMouseDown(playerId));
		hand.SetPointingPosition(controlSpaceFocusPosition);
		UpdateHandVisibility();
	}

	public override void _Input(InputEvent inputEvent)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Invalid comparison between Unknown and I8
		if (_runState.Players.Count == 1)
		{
			return;
		}
		if (inputEvent is InputEventMouseMotion)
		{
			NHandImage hand = GetHand(LocalContext.NetId.Value);
			hand.SetPointingPosition(((CanvasItem)this).GetGlobalMousePosition());
			return;
		}
		InputEventMouseButton val = (InputEventMouseButton)(object)((inputEvent is InputEventMouseButton) ? inputEvent : null);
		if (val != null && (long)val.ButtonIndex == 1)
		{
			NHandImage hand2 = GetHand(LocalContext.NetId.Value);
			if (((InputEvent)val).IsPressed() && !hand2.IsDown)
			{
				hand2.SetIsDown(isDown: true);
			}
			else if (((InputEvent)val).IsReleased() && hand2.IsDown)
			{
				hand2.SetIsDown(isDown: false);
			}
		}
	}

	private void ProcessGuiFocus(Control focusedControl)
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		if (((CanvasItem)this).IsVisibleInTree() && NControllerManager.Instance.IsUsingController && _runState.Players.Count != 1)
		{
			if (focusedControl is NTreasureRoomRelicHolder)
			{
				NHandImage hand = GetHand(LocalContext.NetId.Value);
				Vector2 down = Vector2.Down;
				Vector2 val = ((Vector2)(ref down)).Rotated(((Control)hand).Rotation);
				Vector2 pointingPosition = focusedControl.GlobalPosition + focusedControl.Size * 0.5f + val * 100f;
				hand.SetPointingPosition(pointingPosition);
			}
			else
			{
				NHandImage hand2 = GetHand(LocalContext.NetId.Value);
				hand2.AnimateAway();
			}
		}
	}

	public NHandImage? GetHand(ulong playerId)
	{
		return _hands.FirstOrDefault((NHandImage c) => c.Player.NetId == playerId);
	}

	private void RemoveHand(ulong playerId)
	{
		NHandImage hand = GetHand(playerId);
		if (hand != null)
		{
			((Node)(object)hand).QueueFreeSafely();
			_hands.Remove(hand);
		}
	}

	private void UpdateHandVisibility()
	{
		NetScreenType screenType = _synchronizer.GetScreenType(LocalContext.NetId.Value);
		foreach (NHandImage hand in _hands)
		{
			NetScreenType netScreenType = ((!RunManager.Instance.IsSinglePlayerOrFakeMultiplayer) ? _synchronizer.GetScreenType(hand.Player.NetId) : NetScreenType.SharedRelicPicking);
			bool flag = netScreenType == NetScreenType.SharedRelicPicking && screenType == NetScreenType.SharedRelicPicking;
			if (!((CanvasItem)hand).Visible && flag)
			{
				hand.AnimateIn();
			}
			((CanvasItem)hand).Visible = flag;
		}
		NGame.Instance.CursorManager.SetCursorShown(screenType != NetScreenType.SharedRelicPicking);
	}

	public void BeforeRelicsAwarded()
	{
		foreach (NHandImage hand in _hands)
		{
			hand.SetFrozenForRelicAwards(frozenForRelicAwards: true);
		}
	}

	public void BeforeFightStarted(List<Player> playersInvolved)
	{
		foreach (Player item in playersInvolved)
		{
			NHandImage hand = GetHand(item.NetId);
			hand.SetIsInFight(inFight: true);
		}
	}

	public void AnimateHandsIn()
	{
		foreach (NHandImage hand in _hands)
		{
			if (((CanvasItem)hand).Visible)
			{
				hand.AnimateIn();
			}
		}
	}

	public void AnimateHandsAway()
	{
		foreach (NHandImage hand in _hands)
		{
			hand.AnimateAway();
		}
	}

	public async Task DoFight(RelicPickingResult result, NTreasureRoomRelicHolder holder)
	{
		RelicPickingFight fight = result.fight;
		List<Tween> tweens = new List<Tween>();
		List<Task> tasks = new List<Task>();
		for (int i = 0; i < fight.rounds.Count; i++)
		{
			float durationMultiplier = 1.5f / ((float)i + 1.5f);
			RelicPickingFightRound round = fight.rounds[i];
			tweens.Clear();
			for (int j = 0; j < fight.playersInvolved.Count; j++)
			{
				RelicPickingFightMove? relicPickingFightMove = round.moves[j];
				if (relicPickingFightMove.HasValue)
				{
					Player player = fight.playersInvolved[j];
					NHandImage hand = GetHand(player.NetId);
					tweens.Add(hand.DoFightMove(relicPickingFightMove.Value, 1.5f * durationMultiplier));
				}
			}
			await Task.WhenAll(tweens.Select((Tween t) => ((GodotObject)this).ToSignal((GodotObject)(object)t, SignalName.Finished).ToTask()));
			List<Player> list = new List<Player>();
			for (int num = 0; num < fight.playersInvolved.Count; num++)
			{
				Player player2 = fight.playersInvolved[num];
				if (i < fight.rounds.Count - 1)
				{
					if (round.moves[num].HasValue && !fight.rounds[i + 1].moves[num].HasValue)
					{
						list.Add(player2);
					}
				}
				else if (round.moves[num].HasValue && result.player != player2)
				{
					list.Add(player2);
				}
			}
			tasks.Clear();
			foreach (Player item in list)
			{
				tasks.Add(DoLoseShake(item, Mathf.Max(1f * durationMultiplier, 0.5f)));
			}
			if (i == fight.rounds.Count - 1)
			{
				await Cmd.Wait(0.5f);
				NHandImage hand2 = GetHand(result.player.NetId);
				tasks.Add(hand2.GrabRelic(holder));
			}
			if (tasks.Count == 0)
			{
				await Cmd.Wait(1f * durationMultiplier);
			}
			else
			{
				await Task.WhenAll(tasks);
			}
		}
		foreach (Player item2 in fight.playersInvolved)
		{
			NHandImage hand3 = GetHand(item2.NetId);
			hand3.SetIsInFight(inFight: false);
		}
	}

	private async Task DoLoseShake(Player player, float duration)
	{
		NHandImage hand = GetHand(player.NetId);
		await hand.DoLoseShake(duration);
		hand.SetIsInFight(inFight: false);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Expected O, but got Unknown
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Expected O, but got Unknown
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0295: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a0: Expected O, but got Unknown
		//IL_029b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02be: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0312: Unknown result type (might be due to invalid IL or missing references)
		//IL_031d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0343: Unknown result type (might be due to invalid IL or missing references)
		//IL_034c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0372: Unknown result type (might be due to invalid IL or missing references)
		//IL_037b: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d9: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(14);
		list.Add(new MethodInfo(MethodName._EnterTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._ExitTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnInputStateAdded, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("playerId"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnInputStateRemoved, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("playerId"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AddHand, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("playerId"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnInputStateChanged, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("playerId"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Input, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("inputEvent"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("InputEvent"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ProcessGuiFocus, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("focusedControl"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.GetHand, new PropertyInfo((Type)24, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("playerId"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.RemoveHand, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("playerId"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UpdateHandVisibility, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.BeforeRelicsAwarded, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AnimateHandsIn, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AnimateHandsAway, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
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
		if ((ref method) == MethodName.OnInputStateAdded && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OnInputStateAdded(VariantUtils.ConvertTo<ulong>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnInputStateRemoved && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OnInputStateRemoved(VariantUtils.ConvertTo<ulong>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.AddHand && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			AddHand(VariantUtils.ConvertTo<ulong>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnInputStateChanged && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OnInputStateChanged(VariantUtils.ConvertTo<ulong>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._Input && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			((Node)this)._Input(VariantUtils.ConvertTo<InputEvent>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.ProcessGuiFocus && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			ProcessGuiFocus(VariantUtils.ConvertTo<Control>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.GetHand && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			NHandImage hand = GetHand(VariantUtils.ConvertTo<ulong>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<NHandImage>(ref hand);
			return true;
		}
		if ((ref method) == MethodName.RemoveHand && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			RemoveHand(VariantUtils.ConvertTo<ulong>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.UpdateHandVisibility && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			UpdateHandVisibility();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.BeforeRelicsAwarded && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			BeforeRelicsAwarded();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.AnimateHandsIn && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			AnimateHandsIn();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.AnimateHandsAway && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			AnimateHandsAway();
			ret = default(godot_variant);
			return true;
		}
		return ((Control)this).InvokeGodotClassMethod(ref method, args, ref ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if ((ref method) == MethodName._EnterTree)
		{
			return true;
		}
		if ((ref method) == MethodName._ExitTree)
		{
			return true;
		}
		if ((ref method) == MethodName.OnInputStateAdded)
		{
			return true;
		}
		if ((ref method) == MethodName.OnInputStateRemoved)
		{
			return true;
		}
		if ((ref method) == MethodName.AddHand)
		{
			return true;
		}
		if ((ref method) == MethodName.OnInputStateChanged)
		{
			return true;
		}
		if ((ref method) == MethodName._Input)
		{
			return true;
		}
		if ((ref method) == MethodName.ProcessGuiFocus)
		{
			return true;
		}
		if ((ref method) == MethodName.GetHand)
		{
			return true;
		}
		if ((ref method) == MethodName.RemoveHand)
		{
			return true;
		}
		if ((ref method) == MethodName.UpdateHandVisibility)
		{
			return true;
		}
		if ((ref method) == MethodName.BeforeRelicsAwarded)
		{
			return true;
		}
		if ((ref method) == MethodName.AnimateHandsIn)
		{
			return true;
		}
		if ((ref method) == MethodName.AnimateHandsAway)
		{
			return true;
		}
		return ((Control)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._handAnimateInProgress)
		{
			_handAnimateInProgress = VariantUtils.ConvertTo<float>(ref value);
			return true;
		}
		return ((GodotObject)this).SetGodotClassPropertyValue(ref name, ref value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName._handAnimateInProgress)
		{
			value = VariantUtils.CreateFrom<float>(ref _handAnimateInProgress);
			return true;
		}
		return ((GodotObject)this).GetGodotClassPropertyValue(ref name, ref value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)3, PropertyName._handAnimateInProgress, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		info.AddProperty(PropertyName._handAnimateInProgress, Variant.From<float>(ref _handAnimateInProgress));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._handAnimateInProgress, ref val))
		{
			_handAnimateInProgress = ((Variant)(ref val)).As<float>();
		}
	}
}
