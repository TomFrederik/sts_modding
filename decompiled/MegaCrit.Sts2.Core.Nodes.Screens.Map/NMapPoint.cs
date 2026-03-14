using System;
using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Map;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.HoverTips;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Runs.History;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.TestSupport;

namespace MegaCrit.Sts2.Core.Nodes.Screens.Map;

[ScriptPath("res://src/Core/Nodes/Screens/Map/NMapPoint.cs")]
public abstract class NMapPoint : NButton
{
	public new class MethodName : NButton.MethodName
	{
		public new static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public new static readonly StringName ConnectSignals = StringName.op_Implicit("ConnectSignals");

		public static readonly StringName IsInputAllowed = StringName.op_Implicit("IsInputAllowed");

		public static readonly StringName RefreshVisualsInstantly = StringName.op_Implicit("RefreshVisualsInstantly");

		public static readonly StringName OnSelected = StringName.op_Implicit("OnSelected");

		public new static readonly StringName OnRelease = StringName.op_Implicit("OnRelease");

		public static readonly StringName RefreshColorInstantly = StringName.op_Implicit("RefreshColorInstantly");

		public static readonly StringName RefreshState = StringName.op_Implicit("RefreshState");

		public new static readonly StringName OnFocus = StringName.op_Implicit("OnFocus");

		public new static readonly StringName OnUnfocus = StringName.op_Implicit("OnUnfocus");
	}

	public new class PropertyName : NButton.PropertyName
	{
		public static readonly StringName TraveledColor = StringName.op_Implicit("TraveledColor");

		public static readonly StringName UntravelableColor = StringName.op_Implicit("UntravelableColor");

		public static readonly StringName HoveredColor = StringName.op_Implicit("HoveredColor");

		public static readonly StringName HoverScale = StringName.op_Implicit("HoverScale");

		public static readonly StringName DownScale = StringName.op_Implicit("DownScale");

		public new static readonly StringName AllowFocusWhileDisabled = StringName.op_Implicit("AllowFocusWhileDisabled");

		public static readonly StringName VoteContainer = StringName.op_Implicit("VoteContainer");

		public static readonly StringName IsTravelable = StringName.op_Implicit("IsTravelable");

		public static readonly StringName State = StringName.op_Implicit("State");

		public static readonly StringName TargetColor = StringName.op_Implicit("TargetColor");

		public static readonly StringName _state = StringName.op_Implicit("_state");

		public static readonly StringName _outlineColor = StringName.op_Implicit("_outlineColor");

		public static readonly StringName _controllerSelectionReticle = StringName.op_Implicit("_controllerSelectionReticle");

		public static readonly StringName _screen = StringName.op_Implicit("_screen");
	}

	public new class SignalName : NButton.SignalName
	{
	}

	private MapPointState _state = MapPointState.Untravelable;

	protected IRunState _runState;

	protected Color _outlineColor = new Color(1f, 1f, 1f, 0.75f);

	protected const double _pressDownDur = 0.3;

	protected const double _unhoverAnimDur = 0.5;

	protected NSelectionReticle _controllerSelectionReticle;

	protected NMapScreen _screen;

	protected abstract Color TraveledColor { get; }

	protected abstract Color UntravelableColor { get; }

	protected abstract Color HoveredColor { get; }

	protected abstract Vector2 HoverScale { get; }

	protected abstract Vector2 DownScale { get; }

	protected override bool AllowFocusWhileDisabled => true;

	public NMultiplayerVoteContainer VoteContainer { get; set; }

	protected bool IsTravelable
	{
		get
		{
			NMapScreen screen = _screen;
			if (screen != null && screen.IsDebugTravelEnabled && !screen.IsTraveling)
			{
				return true;
			}
			if (_screen.IsTravelEnabled)
			{
				return State == MapPointState.Travelable;
			}
			return false;
		}
	}

	public MapPoint Point { get; protected set; }

	public MapPointState State
	{
		get
		{
			return _state;
		}
		set
		{
			if (value != _state)
			{
				_state = value;
				RefreshVisualsInstantly();
			}
		}
	}

	protected Color TargetColor
	{
		get
		{
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			MapPointState state = State;
			if ((uint)(state - 1) <= 1u)
			{
				return TraveledColor;
			}
			return UntravelableColor;
		}
	}

	public override void _Ready()
	{
		if (((object)this).GetType() != typeof(NMapPoint))
		{
			Log.Error($"{((object)this).GetType()}");
			throw new InvalidOperationException("Don't call base._Ready()! Call ConnectSignals() instead.");
		}
		ConnectSignals();
	}

	protected override void ConnectSignals()
	{
		base.ConnectSignals();
		VoteContainer = ((Node)this).GetNode<NMultiplayerVoteContainer>(NodePath.op_Implicit("%MapPointVoteContainer"));
		_controllerSelectionReticle = ((Node)this).GetNode<NSelectionReticle>(NodePath.op_Implicit("%SelectionReticle"));
		VoteContainer.Initialize(ShouldDisplayPlayerVote, _runState.Players);
	}

	protected bool IsInputAllowed()
	{
		if (!_screen.IsTraveling)
		{
			return _screen.Drawings.GetLocalDrawingMode() == DrawingMode.None;
		}
		return false;
	}

	private bool ShouldDisplayPlayerVote(Player player)
	{
		if (_screen.PlayerVoteDictionary.TryGetValue(player, out var value) && value.HasValue)
		{
			return value == Point.coord;
		}
		return _runState.CurrentLocation.coord == Point.coord;
	}

	public void RefreshVisualsInstantly()
	{
		_controllerSelectionReticle.OnDeselect();
		RefreshColorInstantly();
		RefreshState();
	}

	public virtual void OnSelected()
	{
	}

	protected sealed override void OnRelease()
	{
		if (IsTravelable && (Point.coord.row != 0 || !TestMode.IsOff || SaveManager.Instance.SeenFtue("map_select_ftue")) && _screen.Drawings.GetLocalDrawingMode() == DrawingMode.None && (_screen.IsNodeOnScreen(this) || !NControllerManager.Instance.IsUsingController))
		{
			_screen.OnMapPointSelectedLocally(this);
		}
	}

	protected virtual void RefreshColorInstantly()
	{
	}

	protected virtual void RefreshState()
	{
		if (IsTravelable)
		{
			Enable();
		}
		else
		{
			Disable();
		}
	}

	protected override void OnFocus()
	{
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		if (!IsInputAllowed())
		{
			return;
		}
		if (_isEnabled && NControllerManager.Instance.IsUsingController)
		{
			_controllerSelectionReticle.OnSelect();
		}
		if (_state != MapPointState.Traveled || !(_runState.CurrentLocation.coord != Point.coord) || NControllerManager.Instance.IsUsingController)
		{
			return;
		}
		MapPointHistoryEntry historyEntryFor = _runState.GetHistoryEntryFor(new RunLocation(Point.coord, _runState.CurrentActIndex));
		if (historyEntryFor != null)
		{
			int num = Point.coord.row + 1;
			for (int i = 0; i < _runState.MapPointHistory.Count - 1; i++)
			{
				num += _runState.MapPointHistory[i].Count;
			}
			NHoverTipSet tip = NHoverTipSet.CreateAndShowMapPointHistory((Control)(object)this, NMapPointHistoryHoverTip.Create(num, LocalContext.NetId.Value, historyEntryFor));
			Callable val = Callable.From((Action)delegate
			{
				tip.SetAlignment((Control)(object)this, HoverTip.GetHoverTipAlignment((Control)(object)this));
			});
			((Callable)(ref val)).CallDeferred(Array.Empty<Variant>());
		}
	}

	protected override void OnUnfocus()
	{
		_controllerSelectionReticle.OnDeselect();
		NHoverTipSet.Remove((Control)(object)this);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<MethodInfo> GetGodotMethodList()
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
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(10);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ConnectSignals, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.IsInputAllowed, new PropertyInfo((Type)1, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.RefreshVisualsInstantly, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnSelected, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnRelease, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.RefreshColorInstantly, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.RefreshState, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnFocus, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnUnfocus, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.ConnectSignals && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			ConnectSignals();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.IsInputAllowed && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			bool flag = IsInputAllowed();
			ret = VariantUtils.CreateFrom<bool>(ref flag);
			return true;
		}
		if ((ref method) == MethodName.RefreshVisualsInstantly && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			RefreshVisualsInstantly();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnSelected && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnSelected();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnRelease && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnRelease();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.RefreshColorInstantly && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			RefreshColorInstantly();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.RefreshState && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			RefreshState();
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
		return base.InvokeGodotClassMethod(in method, args, out ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if ((ref method) == MethodName._Ready)
		{
			return true;
		}
		if ((ref method) == MethodName.ConnectSignals)
		{
			return true;
		}
		if ((ref method) == MethodName.IsInputAllowed)
		{
			return true;
		}
		if ((ref method) == MethodName.RefreshVisualsInstantly)
		{
			return true;
		}
		if ((ref method) == MethodName.OnSelected)
		{
			return true;
		}
		if ((ref method) == MethodName.OnRelease)
		{
			return true;
		}
		if ((ref method) == MethodName.RefreshColorInstantly)
		{
			return true;
		}
		if ((ref method) == MethodName.RefreshState)
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
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName.VoteContainer)
		{
			VoteContainer = VariantUtils.ConvertTo<NMultiplayerVoteContainer>(ref value);
			return true;
		}
		if ((ref name) == PropertyName.State)
		{
			State = VariantUtils.ConvertTo<MapPointState>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._state)
		{
			_state = VariantUtils.ConvertTo<MapPointState>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._outlineColor)
		{
			_outlineColor = VariantUtils.ConvertTo<Color>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._controllerSelectionReticle)
		{
			_controllerSelectionReticle = VariantUtils.ConvertTo<NSelectionReticle>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._screen)
		{
			_screen = VariantUtils.ConvertTo<NMapScreen>(ref value);
			return true;
		}
		return base.SetGodotClassPropertyValue(in name, in value);
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
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName.TraveledColor)
		{
			Color traveledColor = TraveledColor;
			value = VariantUtils.CreateFrom<Color>(ref traveledColor);
			return true;
		}
		if ((ref name) == PropertyName.UntravelableColor)
		{
			Color traveledColor = UntravelableColor;
			value = VariantUtils.CreateFrom<Color>(ref traveledColor);
			return true;
		}
		if ((ref name) == PropertyName.HoveredColor)
		{
			Color traveledColor = HoveredColor;
			value = VariantUtils.CreateFrom<Color>(ref traveledColor);
			return true;
		}
		if ((ref name) == PropertyName.HoverScale)
		{
			Vector2 hoverScale = HoverScale;
			value = VariantUtils.CreateFrom<Vector2>(ref hoverScale);
			return true;
		}
		if ((ref name) == PropertyName.DownScale)
		{
			Vector2 hoverScale = DownScale;
			value = VariantUtils.CreateFrom<Vector2>(ref hoverScale);
			return true;
		}
		if ((ref name) == PropertyName.AllowFocusWhileDisabled)
		{
			bool allowFocusWhileDisabled = AllowFocusWhileDisabled;
			value = VariantUtils.CreateFrom<bool>(ref allowFocusWhileDisabled);
			return true;
		}
		if ((ref name) == PropertyName.VoteContainer)
		{
			NMultiplayerVoteContainer voteContainer = VoteContainer;
			value = VariantUtils.CreateFrom<NMultiplayerVoteContainer>(ref voteContainer);
			return true;
		}
		if ((ref name) == PropertyName.IsTravelable)
		{
			bool allowFocusWhileDisabled = IsTravelable;
			value = VariantUtils.CreateFrom<bool>(ref allowFocusWhileDisabled);
			return true;
		}
		if ((ref name) == PropertyName.State)
		{
			MapPointState state = State;
			value = VariantUtils.CreateFrom<MapPointState>(ref state);
			return true;
		}
		if ((ref name) == PropertyName.TargetColor)
		{
			Color traveledColor = TargetColor;
			value = VariantUtils.CreateFrom<Color>(ref traveledColor);
			return true;
		}
		if ((ref name) == PropertyName._state)
		{
			value = VariantUtils.CreateFrom<MapPointState>(ref _state);
			return true;
		}
		if ((ref name) == PropertyName._outlineColor)
		{
			value = VariantUtils.CreateFrom<Color>(ref _outlineColor);
			return true;
		}
		if ((ref name) == PropertyName._controllerSelectionReticle)
		{
			value = VariantUtils.CreateFrom<NSelectionReticle>(ref _controllerSelectionReticle);
			return true;
		}
		if ((ref name) == PropertyName._screen)
		{
			value = VariantUtils.CreateFrom<NMapScreen>(ref _screen);
			return true;
		}
		return base.GetGodotClassPropertyValue(in name, out value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)2, PropertyName._state, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)20, PropertyName.TraveledColor, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)20, PropertyName.UntravelableColor, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)20, PropertyName.HoveredColor, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)20, PropertyName._outlineColor, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)5, PropertyName.HoverScale, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)5, PropertyName.DownScale, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._controllerSelectionReticle, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName.AllowFocusWhileDisabled, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._screen, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.VoteContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName.IsTravelable, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)2, PropertyName.State, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)20, PropertyName.TargetColor, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		base.SaveGodotObjectData(info);
		StringName voteContainer = PropertyName.VoteContainer;
		NMultiplayerVoteContainer voteContainer2 = VoteContainer;
		info.AddProperty(voteContainer, Variant.From<NMultiplayerVoteContainer>(ref voteContainer2));
		StringName state = PropertyName.State;
		MapPointState state2 = State;
		info.AddProperty(state, Variant.From<MapPointState>(ref state2));
		info.AddProperty(PropertyName._state, Variant.From<MapPointState>(ref _state));
		info.AddProperty(PropertyName._outlineColor, Variant.From<Color>(ref _outlineColor));
		info.AddProperty(PropertyName._controllerSelectionReticle, Variant.From<NSelectionReticle>(ref _controllerSelectionReticle));
		info.AddProperty(PropertyName._screen, Variant.From<NMapScreen>(ref _screen));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		base.RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName.VoteContainer, ref val))
		{
			VoteContainer = ((Variant)(ref val)).As<NMultiplayerVoteContainer>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName.State, ref val2))
		{
			State = ((Variant)(ref val2)).As<MapPointState>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._state, ref val3))
		{
			_state = ((Variant)(ref val3)).As<MapPointState>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._outlineColor, ref val4))
		{
			_outlineColor = ((Variant)(ref val4)).As<Color>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._controllerSelectionReticle, ref val5))
		{
			_controllerSelectionReticle = ((Variant)(ref val5)).As<NSelectionReticle>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._screen, ref val6))
		{
			_screen = ((Variant)(ref val6)).As<NMapScreen>();
		}
	}
}
