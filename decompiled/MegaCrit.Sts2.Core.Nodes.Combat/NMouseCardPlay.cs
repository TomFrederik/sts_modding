using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Audio.Debug;
using MegaCrit.Sts2.Core.ControllerInput;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards.Holders;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.HoverTips;
using MegaCrit.Sts2.Core.Nodes.Multiplayer;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;

namespace MegaCrit.Sts2.Core.Nodes.Combat;

[ScriptPath("res://src/Core/Nodes/Combat/NMouseCardPlay.cs")]
public class NMouseCardPlay : NCardPlay
{
	public new class MethodName : NCardPlay.MethodName
	{
		public static readonly StringName Create = StringName.op_Implicit("Create");

		public static readonly StringName _Input = StringName.op_Implicit("_Input");

		public new static readonly StringName Start = StringName.op_Implicit("Start");

		public static readonly StringName _EnterTree = StringName.op_Implicit("_EnterTree");

		public static readonly StringName _ExitTree = StringName.op_Implicit("_ExitTree");

		public static readonly StringName DisconnectTargetingSignals = StringName.op_Implicit("DisconnectTargetingSignals");

		public new static readonly StringName OnCancelPlayCard = StringName.op_Implicit("OnCancelPlayCard");

		public static readonly StringName IsCardInPlayZone = StringName.op_Implicit("IsCardInPlayZone");

		public static readonly StringName IsCardInCancelZone = StringName.op_Implicit("IsCardInCancelZone");
	}

	public new class PropertyName : NCardPlay.PropertyName
	{
		public static readonly StringName PlayZoneThreshold = StringName.op_Implicit("PlayZoneThreshold");

		public static readonly StringName CancelZoneThreshold = StringName.op_Implicit("CancelZoneThreshold");

		public static readonly StringName _dragStartYPosition = StringName.op_Implicit("_dragStartYPosition");

		public static readonly StringName _isLeftMouseDown = StringName.op_Implicit("_isLeftMouseDown");

		public static readonly StringName _onCreatureHoverCallable = StringName.op_Implicit("_onCreatureHoverCallable");

		public static readonly StringName _onCreatureUnhoverCallable = StringName.op_Implicit("_onCreatureUnhoverCallable");

		public static readonly StringName _signalsConnected = StringName.op_Implicit("_signalsConnected");

		public static readonly StringName _cancelShortcut = StringName.op_Implicit("_cancelShortcut");

		public static readonly StringName _skipStartCardDrag = StringName.op_Implicit("_skipStartCardDrag");
	}

	public new class SignalName : NCardPlay.SignalName
	{
	}

	private const float _fakeLowerEnterPlayZoneDistance = 100f;

	private const float _fakeUpperEnterPlayZoneDistance = 50f;

	private const float _playZoneScreenProportion = 0.75f;

	private const float _cancelZoneScreenProportion = 0.95f;

	private float _dragStartYPosition;

	private Creature? _target;

	private bool _isLeftMouseDown;

	private CancellationTokenSource _cancellationTokenSource;

	private Callable _onCreatureHoverCallable;

	private Callable _onCreatureUnhoverCallable;

	private bool _signalsConnected;

	private StringName _cancelShortcut;

	private bool _skipStartCardDrag;

	private float PlayZoneThreshold
	{
		get
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			Rect2 visibleRect = _viewport.GetVisibleRect();
			float num = ((Rect2)(ref visibleRect)).Size.Y * 0.75f;
			if (_skipStartCardDrag)
			{
				return num + 100f;
			}
			if (_dragStartYPosition > num)
			{
				return Mathf.Max(num, _dragStartYPosition - 100f);
			}
			return Mathf.Min(num, _dragStartYPosition - 50f);
		}
	}

	private float CancelZoneThreshold
	{
		get
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			Rect2 visibleRect = _viewport.GetVisibleRect();
			return ((Rect2)(ref visibleRect)).Size.Y * 0.95f;
		}
	}

	public static NMouseCardPlay Create(NHandCardHolder holder, StringName cancelShortcut, bool wasStartedWithShortcut)
	{
		NMouseCardPlay nMouseCardPlay = new NMouseCardPlay();
		nMouseCardPlay.Holder = holder;
		nMouseCardPlay.Player = holder.CardModel.Owner;
		nMouseCardPlay._cancelShortcut = cancelShortcut;
		nMouseCardPlay._skipStartCardDrag = wasStartedWithShortcut;
		return nMouseCardPlay;
	}

	public override void _Input(InputEvent inputEvent)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Invalid comparison between Unknown and I8
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Invalid comparison between Unknown and I8
		InputEventMouseButton val = (InputEventMouseButton)(object)((inputEvent is InputEventMouseButton) ? inputEvent : null);
		if (val != null)
		{
			MouseButton buttonIndex = val.ButtonIndex;
			if ((long)buttonIndex != 1)
			{
				if ((long)buttonIndex == 2 && ((InputEvent)val).IsPressed())
				{
					CancelPlayCard();
				}
			}
			else if (((InputEvent)val).IsPressed())
			{
				_isLeftMouseDown = true;
			}
			else if (((InputEvent)val).IsReleased())
			{
				_isLeftMouseDown = false;
			}
		}
		if (inputEvent.IsActionPressed(_cancelShortcut, false, false) || inputEvent.IsActionPressed(MegaInput.releaseCard, false, false))
		{
			CancelPlayCard();
			Viewport viewport = ((Node)this).GetViewport();
			if (viewport != null)
			{
				viewport.SetInputAsHandled();
			}
		}
	}

	public override void Start()
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		_isLeftMouseDown = !_skipStartCardDrag;
		((Control)base.Holder.Hitbox).MouseFilter = (MouseFilterEnum)2;
		_cancellationTokenSource = new CancellationTokenSource();
		_onCreatureHoverCallable = Callable.From<NCreature>((Action<NCreature>)base.OnCreatureHover);
		_onCreatureUnhoverCallable = Callable.From<NCreature>((Action<NCreature>)base.OnCreatureUnhover);
		TaskHelper.RunSafely(StartAsync());
	}

	public override void _EnterTree()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		if (NControllerManager.Instance != null)
		{
			((GodotObject)NControllerManager.Instance).Connect(NControllerManager.SignalName.MouseDetected, Callable.From((Action)base.CancelPlayCard), 0u);
			((GodotObject)NControllerManager.Instance).Connect(NControllerManager.SignalName.ControllerDetected, Callable.From((Action)base.CancelPlayCard), 0u);
		}
	}

	public override void _ExitTree()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		if (NControllerManager.Instance != null)
		{
			((GodotObject)NControllerManager.Instance).Disconnect(NControllerManager.SignalName.MouseDetected, Callable.From((Action)base.CancelPlayCard));
			((GodotObject)NControllerManager.Instance).Disconnect(NControllerManager.SignalName.ControllerDetected, Callable.From((Action)base.CancelPlayCard));
		}
		_cancellationTokenSource.Cancel();
		_cancellationTokenSource.Dispose();
		DisconnectTargetingSignals();
	}

	private async Task StartAsync()
	{
		if (base.Card == null || base.CardNode == null)
		{
			return;
		}
		await StartCardDrag();
		if (_cancellationTokenSource.IsCancellationRequested)
		{
			return;
		}
		if (!base.Card.CanPlay(out UnplayableReason reason, out AbstractModel preventer))
		{
			CannotPlayThisCardFtueCheck(base.Card);
			CancelPlayCard();
			LocString playerDialogueLine = reason.GetPlayerDialogueLine(preventer);
			if (playerDialogueLine != null)
			{
				((Node)(object)NCombatRoom.Instance.CombatVfxContainer).AddChildSafely((Node?)(object)NThoughtBubbleVfx.Create(playerDialogueLine.GetFormattedText(), base.Card.Owner.Creature, 1.0));
			}
			return;
		}
		base.CardNode.CardHighlight.AnimFlash();
		TargetMode targetMode = ((!_skipStartCardDrag) ? (_isLeftMouseDown ? TargetMode.ReleaseMouseToTarget : TargetMode.ClickMouseToTarget) : TargetMode.ClickMouseToTarget);
		await TargetSelection(targetMode);
		if (!_cancellationTokenSource.IsCancellationRequested)
		{
			if (!IsCardInPlayZone())
			{
				CancelPlayCard();
			}
			if (!_cancellationTokenSource.IsCancellationRequested)
			{
				TryPlayCard(_target);
			}
		}
	}

	private async Task StartCardDrag()
	{
		NDebugAudioManager.Instance?.Play("card_select.mp3", 0.5f);
		NHoverTipSet.Remove((Control)(object)base.Holder);
		_dragStartYPosition = _viewport.GetMousePosition().Y;
		if (!_skipStartCardDrag)
		{
			do
			{
				await LerpToMouse(base.Holder);
			}
			while (!IsCardInPlayZone() && !_cancellationTokenSource.IsCancellationRequested);
		}
	}

	private async Task TargetSelection(TargetMode targetMode)
	{
		if (base.Card != null)
		{
			TryShowEvokingOrbs();
			base.CardNode?.CardHighlight.AnimFlash();
			TargetType targetType = base.Card.TargetType;
			if ((targetType == TargetType.AnyEnemy || targetType == TargetType.AnyAlly) ? true : false)
			{
				await SingleCreatureTargeting(targetMode, base.Card.TargetType);
			}
			else
			{
				await MultiCreatureTargeting(targetMode);
			}
		}
	}

	private async Task SingleCreatureTargeting(TargetMode targetMode, TargetType targetType)
	{
		if (_cancellationTokenSource.IsCancellationRequested)
		{
			return;
		}
		CenterCard();
		NTargetManager instance = NTargetManager.Instance;
		((GodotObject)instance).Connect(NTargetManager.SignalName.CreatureHovered, _onCreatureHoverCallable, 0u);
		((GodotObject)instance).Connect(NTargetManager.SignalName.CreatureUnhovered, _onCreatureUnhoverCallable, 0u);
		_signalsConnected = true;
		instance.StartTargeting(targetType, (Control)(object)base.CardNode, targetMode, () => IsCardInCancelZone() || _cancellationTokenSource.IsCancellationRequested, null);
		Node val = await instance.SelectionFinished();
		if (val != null)
		{
			Creature target;
			if (!(val is NCreature nCreature))
			{
				if (!(val is NMultiplayerPlayerState nMultiplayerPlayerState))
				{
					throw new ArgumentOutOfRangeException("target", val, null);
				}
				target = nMultiplayerPlayerState.Player.Creature;
			}
			else
			{
				target = nCreature.Entity;
			}
			_target = target;
		}
		DisconnectTargetingSignals();
	}

	private void DisconnectTargetingSignals()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		if (_signalsConnected)
		{
			_signalsConnected = false;
			NTargetManager instance = NTargetManager.Instance;
			((GodotObject)instance).Disconnect(NTargetManager.SignalName.CreatureHovered, _onCreatureHoverCallable);
			((GodotObject)instance).Disconnect(NTargetManager.SignalName.CreatureUnhovered, _onCreatureUnhoverCallable);
		}
	}

	private async Task MultiCreatureTargeting(TargetMode targetMode)
	{
		bool isShowingTargetingVisuals = false;
		Func<bool> shouldFinishTargeting = ((targetMode == TargetMode.ReleaseMouseToTarget) ? ((Func<bool>)(() => !_isLeftMouseDown)) : ((Func<bool>)(() => _isLeftMouseDown)));
		do
		{
			if (isShowingTargetingVisuals)
			{
				if (!IsCardInPlayZone())
				{
					HideTargetingVisuals();
					isShowingTargetingVisuals = false;
				}
			}
			else if (IsCardInPlayZone())
			{
				ShowMultiCreatureTargetingVisuals();
				isShowingTargetingVisuals = true;
			}
			await LerpToMouse(base.Holder);
		}
		while (!shouldFinishTargeting() && !_cancellationTokenSource.IsCancellationRequested && !IsCardInCancelZone());
		if (!_cancellationTokenSource.IsCancellationRequested && IsCardInCancelZone())
		{
			CancelPlayCard();
		}
	}

	protected override void OnCancelPlayCard()
	{
		if (GodotObject.IsInstanceValid((GodotObject)(object)this) && ((Node)this).IsInsideTree())
		{
			((Control)base.Holder.Hitbox).MouseFilter = (MouseFilterEnum)0;
			_cancellationTokenSource.Cancel();
		}
	}

	private async Task LerpToMouse(NHandCardHolder cardHolder)
	{
		cardHolder.SetTargetPosition(_viewport.GetMousePosition());
		await ((GodotObject)this).ToSignal((GodotObject)(object)((Node)this).GetTree(), SignalName.ProcessFrame);
	}

	private bool IsCardInPlayZone()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return _viewport.GetMousePosition().Y < PlayZoneThreshold;
	}

	private bool IsCardInCancelZone()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return _viewport.GetMousePosition().Y > CancelZoneThreshold;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Expected O, but got Unknown
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Expected O, but got Unknown
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Expected O, but got Unknown
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(9);
		list.Add(new MethodInfo(MethodName.Create, new PropertyInfo((Type)24, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Node"), false), (MethodFlags)33, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("holder"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false),
			new PropertyInfo((Type)21, StringName.op_Implicit("cancelShortcut"), (PropertyHint)0, "", (PropertyUsageFlags)6, false),
			new PropertyInfo((Type)1, StringName.op_Implicit("wasStartedWithShortcut"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Input, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("inputEvent"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("InputEvent"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.Start, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._EnterTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._ExitTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.DisconnectTargetingSignals, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnCancelPlayCard, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.IsCardInPlayZone, new PropertyInfo((Type)1, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.IsCardInCancelZone, new PropertyInfo((Type)1, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.Create && ((NativeVariantPtrArgs)(ref args)).Count == 3)
		{
			NMouseCardPlay nMouseCardPlay = Create(VariantUtils.ConvertTo<NHandCardHolder>(ref ((NativeVariantPtrArgs)(ref args))[0]), VariantUtils.ConvertTo<StringName>(ref ((NativeVariantPtrArgs)(ref args))[1]), VariantUtils.ConvertTo<bool>(ref ((NativeVariantPtrArgs)(ref args))[2]));
			ret = VariantUtils.CreateFrom<NMouseCardPlay>(ref nMouseCardPlay);
			return true;
		}
		if ((ref method) == MethodName._Input && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			((Node)this)._Input(VariantUtils.ConvertTo<InputEvent>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.Start && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			Start();
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
		if ((ref method) == MethodName.DisconnectTargetingSignals && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			DisconnectTargetingSignals();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnCancelPlayCard && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnCancelPlayCard();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.IsCardInPlayZone && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			bool flag = IsCardInPlayZone();
			ret = VariantUtils.CreateFrom<bool>(ref flag);
			return true;
		}
		if ((ref method) == MethodName.IsCardInCancelZone && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			bool flag2 = IsCardInCancelZone();
			ret = VariantUtils.CreateFrom<bool>(ref flag2);
			return true;
		}
		return base.InvokeGodotClassMethod(in method, args, out ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static bool InvokeGodotClassStaticMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.Create && ((NativeVariantPtrArgs)(ref args)).Count == 3)
		{
			NMouseCardPlay nMouseCardPlay = Create(VariantUtils.ConvertTo<NHandCardHolder>(ref ((NativeVariantPtrArgs)(ref args))[0]), VariantUtils.ConvertTo<StringName>(ref ((NativeVariantPtrArgs)(ref args))[1]), VariantUtils.ConvertTo<bool>(ref ((NativeVariantPtrArgs)(ref args))[2]));
			ret = VariantUtils.CreateFrom<NMouseCardPlay>(ref nMouseCardPlay);
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
		if ((ref method) == MethodName._Input)
		{
			return true;
		}
		if ((ref method) == MethodName.Start)
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
		if ((ref method) == MethodName.DisconnectTargetingSignals)
		{
			return true;
		}
		if ((ref method) == MethodName.OnCancelPlayCard)
		{
			return true;
		}
		if ((ref method) == MethodName.IsCardInPlayZone)
		{
			return true;
		}
		if ((ref method) == MethodName.IsCardInCancelZone)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName._dragStartYPosition)
		{
			_dragStartYPosition = VariantUtils.ConvertTo<float>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._isLeftMouseDown)
		{
			_isLeftMouseDown = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._onCreatureHoverCallable)
		{
			_onCreatureHoverCallable = VariantUtils.ConvertTo<Callable>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._onCreatureUnhoverCallable)
		{
			_onCreatureUnhoverCallable = VariantUtils.ConvertTo<Callable>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._signalsConnected)
		{
			_signalsConnected = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._cancelShortcut)
		{
			_cancelShortcut = VariantUtils.ConvertTo<StringName>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._skipStartCardDrag)
		{
			_skipStartCardDrag = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		return base.SetGodotClassPropertyValue(in name, in value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName.PlayZoneThreshold)
		{
			float playZoneThreshold = PlayZoneThreshold;
			value = VariantUtils.CreateFrom<float>(ref playZoneThreshold);
			return true;
		}
		if ((ref name) == PropertyName.CancelZoneThreshold)
		{
			float playZoneThreshold = CancelZoneThreshold;
			value = VariantUtils.CreateFrom<float>(ref playZoneThreshold);
			return true;
		}
		if ((ref name) == PropertyName._dragStartYPosition)
		{
			value = VariantUtils.CreateFrom<float>(ref _dragStartYPosition);
			return true;
		}
		if ((ref name) == PropertyName._isLeftMouseDown)
		{
			value = VariantUtils.CreateFrom<bool>(ref _isLeftMouseDown);
			return true;
		}
		if ((ref name) == PropertyName._onCreatureHoverCallable)
		{
			value = VariantUtils.CreateFrom<Callable>(ref _onCreatureHoverCallable);
			return true;
		}
		if ((ref name) == PropertyName._onCreatureUnhoverCallable)
		{
			value = VariantUtils.CreateFrom<Callable>(ref _onCreatureUnhoverCallable);
			return true;
		}
		if ((ref name) == PropertyName._signalsConnected)
		{
			value = VariantUtils.CreateFrom<bool>(ref _signalsConnected);
			return true;
		}
		if ((ref name) == PropertyName._cancelShortcut)
		{
			value = VariantUtils.CreateFrom<StringName>(ref _cancelShortcut);
			return true;
		}
		if ((ref name) == PropertyName._skipStartCardDrag)
		{
			value = VariantUtils.CreateFrom<bool>(ref _skipStartCardDrag);
			return true;
		}
		return base.GetGodotClassPropertyValue(in name, out value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)3, PropertyName.PlayZoneThreshold, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)3, PropertyName.CancelZoneThreshold, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)3, PropertyName._dragStartYPosition, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._isLeftMouseDown, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)25, PropertyName._onCreatureHoverCallable, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)25, PropertyName._onCreatureUnhoverCallable, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._signalsConnected, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)21, PropertyName._cancelShortcut, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._skipStartCardDrag, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName._dragStartYPosition, Variant.From<float>(ref _dragStartYPosition));
		info.AddProperty(PropertyName._isLeftMouseDown, Variant.From<bool>(ref _isLeftMouseDown));
		info.AddProperty(PropertyName._onCreatureHoverCallable, Variant.From<Callable>(ref _onCreatureHoverCallable));
		info.AddProperty(PropertyName._onCreatureUnhoverCallable, Variant.From<Callable>(ref _onCreatureUnhoverCallable));
		info.AddProperty(PropertyName._signalsConnected, Variant.From<bool>(ref _signalsConnected));
		info.AddProperty(PropertyName._cancelShortcut, Variant.From<StringName>(ref _cancelShortcut));
		info.AddProperty(PropertyName._skipStartCardDrag, Variant.From<bool>(ref _skipStartCardDrag));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		base.RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._dragStartYPosition, ref val))
		{
			_dragStartYPosition = ((Variant)(ref val)).As<float>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._isLeftMouseDown, ref val2))
		{
			_isLeftMouseDown = ((Variant)(ref val2)).As<bool>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._onCreatureHoverCallable, ref val3))
		{
			_onCreatureHoverCallable = ((Variant)(ref val3)).As<Callable>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._onCreatureUnhoverCallable, ref val4))
		{
			_onCreatureUnhoverCallable = ((Variant)(ref val4)).As<Callable>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._signalsConnected, ref val5))
		{
			_signalsConnected = ((Variant)(ref val5)).As<bool>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._cancelShortcut, ref val6))
		{
			_cancelShortcut = ((Variant)(ref val6)).As<StringName>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName._skipStartCardDrag, ref val7))
		{
			_skipStartCardDrag = ((Variant)(ref val7)).As<bool>();
		}
	}
}
