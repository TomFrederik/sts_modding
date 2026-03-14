using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Audio.Debug;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.ControllerInput;
using MegaCrit.Sts2.Core.Debug;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Map;
using MegaCrit.Sts2.Core.Models.Modifiers;
using MegaCrit.Sts2.Core.Multiplayer.Game;
using MegaCrit.Sts2.Core.Multiplayer.Game.PeerInput;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.Ftue;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Screens.Capstones;
using MegaCrit.Sts2.Core.Nodes.Screens.ScreenContext;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.Settings;
using MegaCrit.Sts2.Core.TestSupport;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.Nodes.Screens.Map;

[ScriptPath("res://src/Core/Nodes/Screens/Map/NMapScreen.cs")]
public class NMapScreen : Control, IScreenContext, INetCursorPositionTranslator
{
	[Signal]
	public delegate void OpenedEventHandler();

	[Signal]
	public delegate void ClosedEventHandler();

	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName _ExitTree = StringName.op_Implicit("_ExitTree");

		public static readonly StringName GetLineEndpoint = StringName.op_Implicit("GetLineEndpoint");

		public static readonly StringName RecalculateTravelability = StringName.op_Implicit("RecalculateTravelability");

		public static readonly StringName InitMapVotes = StringName.op_Implicit("InitMapVotes");

		public static readonly StringName OnMapPointSelectedLocally = StringName.op_Implicit("OnMapPointSelectedLocally");

		public static readonly StringName RefreshAllMapPointVotes = StringName.op_Implicit("RefreshAllMapPointVotes");

		public static readonly StringName RemoveAllMapPointsAndPaths = StringName.op_Implicit("RemoveAllMapPointsAndPaths");

		public static readonly StringName _Process = StringName.op_Implicit("_Process");

		public static readonly StringName UpdateScrollPosition = StringName.op_Implicit("UpdateScrollPosition");

		public static readonly StringName _GuiInput = StringName.op_Implicit("_GuiInput");

		public static readonly StringName ProcessMouseEvent = StringName.op_Implicit("ProcessMouseEvent");

		public static readonly StringName ProcessMouseDrawingEvent = StringName.op_Implicit("ProcessMouseDrawingEvent");

		public static readonly StringName ProcessScrollEvent = StringName.op_Implicit("ProcessScrollEvent");

		public static readonly StringName ProcessControllerEvent = StringName.op_Implicit("ProcessControllerEvent");

		public static readonly StringName SetTravelEnabled = StringName.op_Implicit("SetTravelEnabled");

		public static readonly StringName SetDebugTravelEnabled = StringName.op_Implicit("SetDebugTravelEnabled");

		public static readonly StringName RefreshAllPointVisuals = StringName.op_Implicit("RefreshAllPointVisuals");

		public static readonly StringName PlayStartOfActAnimation = StringName.op_Implicit("PlayStartOfActAnimation");

		public static readonly StringName InitMapPrompt = StringName.op_Implicit("InitMapPrompt");

		public static readonly StringName SetInterruptable = StringName.op_Implicit("SetInterruptable");

		public static readonly StringName CanScroll = StringName.op_Implicit("CanScroll");

		public static readonly StringName TryCancelStartOfActAnim = StringName.op_Implicit("TryCancelStartOfActAnim");

		public static readonly StringName OnVisibilityChanged = StringName.op_Implicit("OnVisibilityChanged");

		public static readonly StringName OnCapstoneChanged = StringName.op_Implicit("OnCapstoneChanged");

		public static readonly StringName Close = StringName.op_Implicit("Close");

		public static readonly StringName Open = StringName.op_Implicit("Open");

		public static readonly StringName OnBackButtonPressed = StringName.op_Implicit("OnBackButtonPressed");

		public static readonly StringName _Input = StringName.op_Implicit("_Input");

		public static readonly StringName OnMapDrawingButtonPressed = StringName.op_Implicit("OnMapDrawingButtonPressed");

		public static readonly StringName OnMapErasingButtonPressed = StringName.op_Implicit("OnMapErasingButtonPressed");

		public static readonly StringName UpdateDrawingButtonStates = StringName.op_Implicit("UpdateDrawingButtonStates");

		public static readonly StringName OnClearMapDrawingButtonPressed = StringName.op_Implicit("OnClearMapDrawingButtonPressed");

		public static readonly StringName HighlightPointType = StringName.op_Implicit("HighlightPointType");

		public static readonly StringName OnLegendHotkeyPressed = StringName.op_Implicit("OnLegendHotkeyPressed");

		public static readonly StringName OnDrawingToolsHotkeyPressed = StringName.op_Implicit("OnDrawingToolsHotkeyPressed");

		public static readonly StringName GetNetPositionFromScreenPosition = StringName.op_Implicit("GetNetPositionFromScreenPosition");

		public static readonly StringName GetMapPositionFromNetPosition = StringName.op_Implicit("GetMapPositionFromNetPosition");

		public static readonly StringName GetScreenPositionFromNetPosition = StringName.op_Implicit("GetScreenPositionFromNetPosition");

		public static readonly StringName IsNodeOnScreen = StringName.op_Implicit("IsNodeOnScreen");

		public static readonly StringName CleanUp = StringName.op_Implicit("CleanUp");

		public static readonly StringName UpdateHotkeyDisplay = StringName.op_Implicit("UpdateHotkeyDisplay");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName IsOpen = StringName.op_Implicit("IsOpen");

		public static readonly StringName IsTravelEnabled = StringName.op_Implicit("IsTravelEnabled");

		public static readonly StringName IsDebugTravelEnabled = StringName.op_Implicit("IsDebugTravelEnabled");

		public static readonly StringName MapLegendX = StringName.op_Implicit("MapLegendX");

		public static readonly StringName IsTraveling = StringName.op_Implicit("IsTraveling");

		public static readonly StringName Drawings = StringName.op_Implicit("Drawings");

		public static readonly StringName DefaultFocusedControl = StringName.op_Implicit("DefaultFocusedControl");

		public static readonly StringName _mapContainer = StringName.op_Implicit("_mapContainer");

		public static readonly StringName _pathsContainer = StringName.op_Implicit("_pathsContainer");

		public static readonly StringName _points = StringName.op_Implicit("_points");

		public static readonly StringName _bossPointNode = StringName.op_Implicit("_bossPointNode");

		public static readonly StringName _secondBossPointNode = StringName.op_Implicit("_secondBossPointNode");

		public static readonly StringName _startingPointNode = StringName.op_Implicit("_startingPointNode");

		public static readonly StringName _mapBgContainer = StringName.op_Implicit("_mapBgContainer");

		public static readonly StringName _marker = StringName.op_Implicit("_marker");

		public static readonly StringName _backButton = StringName.op_Implicit("_backButton");

		public static readonly StringName _drawingToolsHotkeyIcon = StringName.op_Implicit("_drawingToolsHotkeyIcon");

		public static readonly StringName _drawingTools = StringName.op_Implicit("_drawingTools");

		public static readonly StringName _mapDrawingButton = StringName.op_Implicit("_mapDrawingButton");

		public static readonly StringName _mapErasingButton = StringName.op_Implicit("_mapErasingButton");

		public static readonly StringName _mapClearButton = StringName.op_Implicit("_mapClearButton");

		public static readonly StringName _mapLegend = StringName.op_Implicit("_mapLegend");

		public static readonly StringName _legendItems = StringName.op_Implicit("_legendItems");

		public static readonly StringName _legendHotkeyIcon = StringName.op_Implicit("_legendHotkeyIcon");

		public static readonly StringName _backstop = StringName.op_Implicit("_backstop");

		public static readonly StringName _tween = StringName.op_Implicit("_tween");

		public static readonly StringName _startDragPos = StringName.op_Implicit("_startDragPos");

		public static readonly StringName _targetDragPos = StringName.op_Implicit("_targetDragPos");

		public static readonly StringName _isDragging = StringName.op_Implicit("_isDragging");

		public static readonly StringName _hasPlayedAnimation = StringName.op_Implicit("_hasPlayedAnimation");

		public static readonly StringName _controllerScrollAmount = StringName.op_Implicit("_controllerScrollAmount");

		public static readonly StringName _distX = StringName.op_Implicit("_distX");

		public static readonly StringName _distY = StringName.op_Implicit("_distY");

		public static readonly StringName _actAnimTween = StringName.op_Implicit("_actAnimTween");

		public static readonly StringName _mapScrollAnimTimer = StringName.op_Implicit("_mapScrollAnimTimer");

		public static readonly StringName _mapAnimStartDelay = StringName.op_Implicit("_mapAnimStartDelay");

		public static readonly StringName _mapAnimDuration = StringName.op_Implicit("_mapAnimDuration");

		public static readonly StringName _canInterruptAnim = StringName.op_Implicit("_canInterruptAnim");

		public static readonly StringName _isInputDisabled = StringName.op_Implicit("_isInputDisabled");

		public static readonly StringName _promptTween = StringName.op_Implicit("_promptTween");

		public static readonly StringName _drawingInput = StringName.op_Implicit("_drawingInput");
	}

	public class SignalName : SignalName
	{
		public static readonly StringName Opened = StringName.op_Implicit("Opened");

		public static readonly StringName Closed = StringName.op_Implicit("Closed");
	}

	private ActMap _map = NullActMap.Instance;

	private Control _mapContainer;

	private Control _pathsContainer;

	private Control _points;

	private NBossMapPoint? _bossPointNode;

	private NBossMapPoint? _secondBossPointNode;

	private NMapPoint? _startingPointNode;

	private NMapBg _mapBgContainer;

	private NMapMarker _marker;

	private NBackButton _backButton;

	private TextureRect _drawingToolsHotkeyIcon;

	private Control _drawingTools;

	private NMapDrawButton _mapDrawingButton;

	private NMapEraseButton _mapErasingButton;

	private NMapClearButton _mapClearButton;

	private Control _mapLegend;

	private Control _legendItems;

	private TextureRect _legendHotkeyIcon;

	private Control _backstop;

	private Tween? _tween;

	private Vector2 _startDragPos;

	private Vector2 _targetDragPos;

	private bool _isDragging;

	private bool _hasPlayedAnimation;

	private readonly Dictionary<MapCoord, NMapPoint> _mapPointDictionary = new Dictionary<MapCoord, NMapPoint>();

	private readonly Dictionary<(MapCoord, MapCoord), IReadOnlyList<TextureRect>> _paths = new Dictionary<(MapCoord, MapCoord), IReadOnlyList<TextureRect>>();

	private float _controllerScrollAmount = 400f;

	private const float _scrollLimitTop = 1800f;

	private const float _scrollLimitBottom = -600f;

	private const float _totalHeight = 2325f;

	private const float _totalWidth = 1050f;

	private float _distX;

	private float _distY;

	private const float _pointJitterX = 21f;

	private const float _pointJitterY = 25f;

	private const float _tickDist = 22f;

	private const float _pathPosJitter = 3f;

	private const float _pathAngleJitter = 0.1f;

	private static readonly Vector2 _tickTraveledScale = Vector2.One * 1.2f;

	private Tween? _actAnimTween;

	private float _mapScrollAnimTimer;

	private const string _mapTickScenePath = "res://scenes/ui/map_dot.tscn";

	private readonly double _mapAnimStartDelay = ((SaveManager.Instance.PrefsSave.FastMode == FastModeType.Fast) ? 0.5 : 1.0);

	private readonly double _mapAnimDuration = ((SaveManager.Instance.PrefsSave.FastMode == FastModeType.Fast) ? 1.5 : 3.0);

	private bool _canInterruptAnim;

	private bool _isInputDisabled;

	private RunState _runState;

	private Tween? _promptTween;

	private NMapDrawingInput? _drawingInput;

	private OpenedEventHandler backing_Opened;

	private ClosedEventHandler backing_Closed;

	public static NMapScreen? Instance => NRun.Instance?.GlobalUi.MapScreen;

	public bool IsOpen { get; private set; }

	public bool IsTravelEnabled { get; private set; }

	public bool IsDebugTravelEnabled { get; private set; }

	private float MapLegendX => ((Control)this).Size.X * 0.8f;

	public bool IsTraveling { get; set; }

	public Dictionary<Player, MapCoord?> PlayerVoteDictionary { get; } = new Dictionary<Player, MapCoord?>();

	public NMapDrawings Drawings { get; private set; }

	public static IEnumerable<string> AssetPaths => NMapDrawings.AssetPaths.Append("res://scenes/ui/map_dot.tscn");

	public Control DefaultFocusedControl
	{
		get
		{
			NMapPoint nMapPoint = _mapPointDictionary.Values.FirstOrDefault((NMapPoint n) => n.IsEnabled);
			if (nMapPoint != null)
			{
				return (Control)(object)nMapPoint;
			}
			return (Control)(object)this;
		}
	}

	public event Action<MapPointType>? PointTypeHighlighted;

	public event OpenedEventHandler Opened
	{
		add
		{
			backing_Opened = (OpenedEventHandler)Delegate.Combine(backing_Opened, value);
		}
		remove
		{
			backing_Opened = (OpenedEventHandler)Delegate.Remove(backing_Opened, value);
		}
	}

	public event ClosedEventHandler Closed
	{
		add
		{
			backing_Closed = (ClosedEventHandler)Delegate.Combine(backing_Closed, value);
		}
		remove
		{
			backing_Closed = (ClosedEventHandler)Delegate.Remove(backing_Closed, value);
		}
	}

	public override void _Ready()
	{
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		//IL_0294: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Unknown result type (might be due to invalid IL or missing references)
		//IL_035e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0364: Unknown result type (might be due to invalid IL or missing references)
		//IL_0380: Unknown result type (might be due to invalid IL or missing references)
		//IL_0386: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a8: Unknown result type (might be due to invalid IL or missing references)
		((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("MapLegend/Header")).SetTextAutoSize(new LocString("map", "LEGEND_HEADER").GetFormattedText());
		_mapContainer = ((Node)this).GetNode<Control>(NodePath.op_Implicit("TheMap"));
		_mapBgContainer = ((Node)this).GetNode<NMapBg>(NodePath.op_Implicit("%MapBg"));
		_pathsContainer = ((Node)this).GetNode<Control>(NodePath.op_Implicit("TheMap/Paths"));
		_points = ((Node)this).GetNode<Control>(NodePath.op_Implicit("TheMap/Points"));
		_marker = ((Node)this).GetNode<NMapMarker>(NodePath.op_Implicit("TheMap/MapMarker"));
		Drawings = ((Node)this).GetNode<NMapDrawings>(NodePath.op_Implicit("TheMap/Drawings"));
		_backButton = ((Node)this).GetNode<NBackButton>(NodePath.op_Implicit("Back"));
		((GodotObject)_backButton).Connect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)OnBackButtonPressed), 0u);
		_backButton.Disable();
		_mapLegend = ((Node)this).GetNode<Control>(NodePath.op_Implicit("MapLegend"));
		_legendItems = ((Node)this).GetNode<Control>(NodePath.op_Implicit("MapLegend/LegendItems"));
		_legendHotkeyIcon = ((Node)this).GetNode<TextureRect>(NodePath.op_Implicit("MapLegend/LegendHotkeyIcon"));
		_drawingToolsHotkeyIcon = ((Node)this).GetNode<TextureRect>(NodePath.op_Implicit("DrawingToolsHotkey"));
		_backstop = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%Backstop"));
		_drawingTools = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%DrawingTools"));
		_mapDrawingButton = ((Node)this).GetNode<NMapDrawButton>(NodePath.op_Implicit("%DrawButton"));
		((GodotObject)_mapDrawingButton).Connect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)OnMapDrawingButtonPressed), 0u);
		_mapErasingButton = ((Node)this).GetNode<NMapEraseButton>(NodePath.op_Implicit("%EraseButton"));
		((GodotObject)_mapErasingButton).Connect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)OnMapErasingButtonPressed), 0u);
		_mapClearButton = ((Node)this).GetNode<NMapClearButton>(NodePath.op_Implicit("%ClearButton"));
		((GodotObject)_mapClearButton).Connect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)OnClearMapDrawingButtonPressed), 0u);
		RunManager.Instance.MapSelectionSynchronizer.PlayerVoteChanged += OnPlayerVoteChanged;
		RunManager.Instance.MapSelectionSynchronizer.PlayerVoteCancelled += OnPlayerVoteCancelled;
		((Node)this).ProcessMode = (ProcessModeEnum)(((CanvasItem)this).Visible ? 0 : 4);
		((GodotObject)this).Connect(SignalName.VisibilityChanged, Callable.From((Action)OnVisibilityChanged), 0u);
		Callable val = Callable.From<Error>((Func<Error>)(() => ((GodotObject)NCapstoneContainer.Instance).Connect(NCapstoneContainer.SignalName.Changed, Callable.From((Action)OnCapstoneChanged), 0u)));
		((Callable)(ref val)).CallDeferred(Array.Empty<Variant>());
		List<NMapLegendItem> list = ((IEnumerable)((Node)_legendItems).GetChildren(false)).OfType<NMapLegendItem>().ToList();
		for (int num = 0; num < list.Count; num++)
		{
			((Control)list[num]).FocusNeighborTop = ((num > 0) ? ((Node)list[num - 1]).GetPath() : ((Node)list[num]).GetPath());
			((Control)list[num]).FocusNeighborBottom = ((num < list.Count - 1) ? ((Node)list[num + 1]).GetPath() : ((Node)list[num]).GetPath());
			((Control)list[num]).FocusNeighborRight = ((Node)list[num]).GetPath();
		}
		((GodotObject)NControllerManager.Instance).Connect(NControllerManager.SignalName.MouseDetected, Callable.From((Action)UpdateHotkeyDisplay), 0u);
		((GodotObject)NControllerManager.Instance).Connect(NControllerManager.SignalName.ControllerDetected, Callable.From((Action)UpdateHotkeyDisplay), 0u);
		((GodotObject)NInputManager.Instance).Connect(NInputManager.SignalName.InputRebound, Callable.From((Action)UpdateHotkeyDisplay), 0u);
		UpdateHotkeyDisplay();
	}

	public override void _ExitTree()
	{
		RunManager.Instance.MapSelectionSynchronizer.PlayerVoteChanged -= OnPlayerVoteChanged;
		RunManager.Instance.MapSelectionSynchronizer.PlayerVoteCancelled -= OnPlayerVoteCancelled;
	}

	public void Initialize(RunState runState)
	{
		_runState = runState;
		Drawings.Initialize(RunManager.Instance.NetService, _runState, RunManager.Instance.InputSynchronizer);
		_marker.Initialize(LocalContext.GetMe(_runState));
		_mapBgContainer.Initialize(_runState);
	}

	public void SetMap(ActMap map, uint seed, bool clearDrawings)
	{
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_028f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_037e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0330: Unknown result type (might be due to invalid IL or missing references)
		//IL_047d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0489: Unknown result type (might be due to invalid IL or missing references)
		_map = map;
		_mapPointDictionary.Clear();
		_paths.Clear();
		RemoveAllMapPointsAndPaths();
		_marker.ResetMapPoint();
		if (clearDrawings)
		{
			Drawings.ClearAllLines();
		}
		_hasPlayedAnimation = false;
		int rowCount = map.GetRowCount();
		int columnCount = map.GetColumnCount();
		float num = ((map.SecondBossMapPoint != null) ? 0.9f : 1f);
		_distY = 2325f / (float)(rowCount - 1) * num;
		_distX = 1050f / (float)columnCount;
		Rng rng = new Rng(seed, $"map_jitter_{_runState.CurrentActIndex}");
		Vector2 val = default(Vector2);
		((Vector2)(ref val))._002Ector(-500f, 740f);
		Vector2 val2 = default(Vector2);
		((Vector2)(ref val2))._002Ector(_distX, 0f - _distY);
		foreach (MapPoint allMapPoint in map.GetAllMapPoints())
		{
			NNormalMapPoint nNormalMapPoint = NNormalMapPoint.Create(allMapPoint, this, _runState);
			((Control)nNormalMapPoint).Position = new Vector2((float)allMapPoint.coord.col, (float)allMapPoint.coord.row) * val2 + val;
			float num2 = rng.NextFloat(-21f, 21f);
			float num3 = rng.NextFloat(-25f, 25f);
			((Control)nNormalMapPoint).Position = ((Control)nNormalMapPoint).Position + new Vector2(num2, num3);
			_mapPointDictionary.Add(allMapPoint.coord, nNormalMapPoint);
			((Node)(object)_points).AddChildSafely((Node?)(object)nNormalMapPoint);
			nNormalMapPoint.SetAngle(Rng.Chaotic.NextGaussianFloat(0f, 8f));
		}
		_bossPointNode = NBossMapPoint.Create(map.BossMapPoint, this, _runState);
		((Control)_bossPointNode).Position = new Vector2(-200f, -1980f * num);
		((Node)(object)_points).AddChildSafely((Node?)(object)_bossPointNode);
		_mapPointDictionary[map.BossMapPoint.coord] = _bossPointNode;
		if (map.SecondBossMapPoint != null)
		{
			((Control)_bossPointNode).Scale = new Vector2(0.75f, 0.75f);
			_secondBossPointNode = NBossMapPoint.Create(map.SecondBossMapPoint, this, _runState);
			((Control)_secondBossPointNode).Position = new Vector2(-200f, -2280f * num);
			((Control)_secondBossPointNode).Scale = new Vector2(0.75f, 0.75f);
			((Node)(object)_points).AddChildSafely((Node?)(object)_secondBossPointNode);
			_mapPointDictionary[map.SecondBossMapPoint.coord] = _secondBossPointNode;
		}
		if (map.StartingMapPoint.PointType == MapPointType.Ancient)
		{
			_startingPointNode = NAncientMapPoint.Create(map.StartingMapPoint, this, _runState);
			((Control)_startingPointNode).Position = new Vector2(-80f, (float)map.StartingMapPoint.coord.row * (0f - _distY) + 720f);
		}
		else
		{
			_startingPointNode = NNormalMapPoint.Create(map.StartingMapPoint, this, _runState);
			((Control)_startingPointNode).Position = new Vector2(-80f, (float)map.StartingMapPoint.coord.row * (0f - _distY) + 800f);
		}
		((Node)(object)_points).AddChildSafely((Node?)(object)_startingPointNode);
		_mapPointDictionary[map.StartingMapPoint.coord] = _startingPointNode;
		foreach (MapPoint allMapPoint2 in map.GetAllMapPoints())
		{
			DrawPaths(_mapPointDictionary[allMapPoint2.coord], allMapPoint2);
		}
		DrawPaths(_startingPointNode, map.StartingMapPoint);
		DrawPaths(_bossPointNode, map.BossMapPoint);
		IReadOnlyList<MapCoord> visitedMapCoords = _runState.VisitedMapCoords;
		for (int i = 0; i < visitedMapCoords.Count - 1; i++)
		{
			if (!_paths.TryGetValue((visitedMapCoords[i], visitedMapCoords[i + 1]), out IReadOnlyList<TextureRect> value))
			{
				continue;
			}
			foreach (TextureRect item in value)
			{
				((CanvasItem)item).Modulate = _runState.Act.MapTraveledColor;
				((Control)item).Scale = _tickTraveledScale;
			}
		}
		InitMapVotes();
		RefreshAllMapPointVotes();
		for (int j = 0; j < map.GetRowCount(); j++)
		{
			IEnumerable<MapPoint> pointsInRow = map.GetPointsInRow(j);
			List<NMapPoint> list = pointsInRow.Select((MapPoint p) => _mapPointDictionary[p.coord]).ToList();
			for (int num4 = 0; num4 < list.Count; num4++)
			{
				((Control)list[num4]).FocusNeighborLeft = ((num4 > 0) ? ((Node)list[num4 - 1]).GetPath() : ((Node)list[num4]).GetPath());
				((Control)list[num4]).FocusNeighborRight = ((num4 < list.Count - 1) ? ((Node)list[num4 + 1]).GetPath() : ((Node)list[num4]).GetPath());
				((Control)list[num4]).FocusNeighborTop = ((Node)list[num4]).GetPath();
				((Control)list[num4]).FocusNeighborBottom = ((Node)list[num4]).GetPath();
			}
		}
		((Control)_startingPointNode).FocusNeighborLeft = ((Node)_startingPointNode).GetPath();
		((Control)_startingPointNode).FocusNeighborRight = ((Node)_startingPointNode).GetPath();
		((Control)_startingPointNode).FocusNeighborTop = ((Node)_startingPointNode).GetPath();
		((Control)_startingPointNode).FocusNeighborBottom = ((Node)_startingPointNode).GetPath();
		((Control)_bossPointNode).FocusNeighborLeft = ((Node)_bossPointNode).GetPath();
		((Control)_bossPointNode).FocusNeighborRight = ((Node)_bossPointNode).GetPath();
		((Control)_bossPointNode).FocusNeighborBottom = ((Node)_bossPointNode).GetPath();
		if (_secondBossPointNode != null)
		{
			((Control)_bossPointNode).FocusNeighborTop = ((Node)_secondBossPointNode).GetPath();
			((Control)_secondBossPointNode).FocusNeighborBottom = ((Node)_bossPointNode).GetPath();
			((Control)_secondBossPointNode).FocusNeighborLeft = ((Node)_secondBossPointNode).GetPath();
			((Control)_secondBossPointNode).FocusNeighborRight = ((Node)_secondBossPointNode).GetPath();
			((Control)_secondBossPointNode).FocusNeighborTop = ((Node)_secondBossPointNode).GetPath();
		}
		else
		{
			((Control)_bossPointNode).FocusNeighborTop = ((Node)_bossPointNode).GetPath();
		}
		if (((CanvasItem)this).IsVisible())
		{
			RecalculateTravelability();
			RefreshAllPointVisuals();
		}
	}

	private void DrawPaths(NMapPoint mapPointNode, MapPoint mapPoint)
	{
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		foreach (MapPoint child in mapPoint.Children)
		{
			if (!_mapPointDictionary.TryGetValue(child.coord, out NMapPoint value))
			{
				throw new InvalidOperationException($"Map point child with coord {child.coord} is not in the map point dictionary!");
			}
			Vector2 lineEndpoint = GetLineEndpoint(mapPointNode);
			Vector2 lineEndpoint2 = GetLineEndpoint(value);
			IReadOnlyList<TextureRect> value2 = CreatePath(lineEndpoint, lineEndpoint2);
			_paths.Add((mapPoint.coord, child.coord), value2);
		}
	}

	private Vector2 GetLineEndpoint(NMapPoint point)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		if (point is NNormalMapPoint)
		{
			return ((Control)point).Position;
		}
		return ((Control)point).Position + ((Control)point).Size * 0.5f;
	}

	private void RecalculateTravelability()
	{
		if (_runState.VisitedMapCoords.Any())
		{
			foreach (NMapPoint value in _mapPointDictionary.Values)
			{
				value.State = MapPointState.Untravelable;
			}
			foreach (MapCoord visitedMapCoord in _runState.VisitedMapCoords)
			{
				_mapPointDictionary[visitedMapCoord].State = MapPointState.Traveled;
			}
			IReadOnlyList<MapCoord> visitedMapCoords = _runState.VisitedMapCoords;
			MapCoord mapCoord = visitedMapCoords[visitedMapCoords.Count - 1];
			if (_secondBossPointNode != null && mapCoord == _bossPointNode.Point.coord)
			{
				_secondBossPointNode.State = MapPointState.Travelable;
				return;
			}
			if (mapCoord.row != _map.GetRowCount() - 1)
			{
				IEnumerable<MapPoint> enumerable = (_runState.Modifiers.OfType<Flight>().Any() ? _map.GetPointsInRow(mapCoord.row + 1) : _mapPointDictionary[mapCoord].Point.Children);
				{
					foreach (MapPoint item in enumerable)
					{
						_mapPointDictionary[item.coord].State = MapPointState.Travelable;
					}
					return;
				}
			}
			_bossPointNode.State = MapPointState.Travelable;
		}
		else
		{
			_startingPointNode.State = MapPointState.Travelable;
		}
	}

	private void InitMapVotes()
	{
		foreach (Player player in _runState.Players)
		{
			MapCoord? mapCoord = RunManager.Instance.MapSelectionSynchronizer.GetVote(player)?.coord;
			if (mapCoord.HasValue)
			{
				OnPlayerVoteChangedInternal(player, null, mapCoord.Value);
			}
		}
	}

	public void OnMapPointSelectedLocally(NMapPoint point)
	{
		Player me = LocalContext.GetMe(_runState);
		if (!PlayerVoteDictionary.TryGetValue(me, out var value) || value != point.Point.coord)
		{
			OnPlayerVoteChangedInternal(me, RunManager.Instance.MapSelectionSynchronizer.GetVote(me)?.coord, point.Point.coord);
			RunLocation source = new RunLocation(_runState.CurrentMapCoord, _runState.CurrentActIndex);
			MapVote value2 = new MapVote
			{
				coord = point.Point.coord,
				mapGenerationCount = RunManager.Instance.MapSelectionSynchronizer.MapGenerationCount
			};
			VoteForMapCoordAction action = new VoteForMapCoordAction(LocalContext.GetMe(_runState), source, value2);
			RunManager.Instance.ActionQueueSynchronizer.RequestEnqueue(action);
		}
		else if (_runState.Players.Count > 1)
		{
			RunManager.Instance.FlavorSynchronizer.SendMapPing(point.Point.coord);
		}
	}

	private void OnPlayerVoteChanged(Player player, MapVote? oldLocation, MapVote? newLocation)
	{
		Log.Info($"Player vote changed for {player.NetId}: {oldLocation}->{newLocation}");
		if (!LocalContext.IsMe(player))
		{
			OnPlayerVoteChangedInternal(player, oldLocation?.coord, newLocation?.coord);
		}
	}

	private void OnPlayerVoteCancelled(Player player)
	{
		Log.Info($"Player vote cancelled for {player.NetId}");
		OnPlayerVoteChangedInternal(player, PlayerVoteDictionary[player], null);
	}

	private void OnPlayerVoteChangedInternal(Player player, MapCoord? oldCoord, MapCoord? newCoord)
	{
		if (_runState.Players.Count > 1)
		{
			PlayerVoteDictionary[player] = newCoord;
			if (oldCoord.HasValue)
			{
				NMapPoint nMapPoint = _mapPointDictionary[oldCoord.Value];
				nMapPoint.VoteContainer.RefreshPlayerVotes();
			}
			else if (_runState.CurrentLocation.coord.HasValue)
			{
				NMapPoint nMapPoint2 = _mapPointDictionary[_runState.CurrentLocation.coord.Value];
				nMapPoint2.VoteContainer.RefreshPlayerVotes();
			}
			if (newCoord.HasValue)
			{
				NMapPoint nMapPoint3 = _mapPointDictionary[newCoord.Value];
				nMapPoint3.VoteContainer.RefreshPlayerVotes();
			}
			else if (_runState.CurrentLocation.coord.HasValue)
			{
				NMapPoint nMapPoint4 = _mapPointDictionary[_runState.CurrentLocation.coord.Value];
				nMapPoint4.VoteContainer.RefreshPlayerVotes();
			}
		}
	}

	public void InitMarker(MapCoord coord)
	{
		NMapPoint mapPoint = _mapPointDictionary[coord];
		_marker.SetMapPoint(mapPoint);
	}

	public async Task TravelToMapCoord(MapCoord coord)
	{
		IsTraveling = true;
		RecalculateTravelability();
		if (NCapstoneContainer.Instance.CurrentCapstoneScreen is NDeckViewScreen)
		{
			NCapstoneContainer.Instance.Close();
		}
		_marker.HideMapPoint();
		IsTravelEnabled = false;
		MapSplitVoteAnimation mapSplitVoteAnimation = new MapSplitVoteAnimation(this, _runState, _mapPointDictionary);
		await mapSplitVoteAnimation.TryPlay(coord);
		NMapPoint node = _mapPointDictionary[coord];
		node.OnSelected();
		float scaleMultiplier = 1f;
		if (node is NAncientMapPoint)
		{
			scaleMultiplier = 1.5f;
		}
		else if (node is NBossMapPoint)
		{
			scaleMultiplier = 2f;
		}
		NMapNodeSelectVfx nMapNodeSelectVfx = NMapNodeSelectVfx.Create(scaleMultiplier);
		SfxCmd.Play("event:/sfx/ui/map/map_select");
		((Node)(object)node).AddChildSafely((Node?)(object)nMapNodeSelectVfx);
		((Control)nMapNodeSelectVfx).Position = ((Control)nMapNodeSelectVfx).Position + ((Control)node).PivotOffset;
		IReadOnlyList<MapCoord> visitedMapCoords = _runState.VisitedMapCoords;
		SfxCmd.Play("event:/sfx/ui/wipe_map");
		Task fadeOutTask = TaskHelper.RunSafely(NGame.Instance.Transition.RoomFadeOut());
		if (visitedMapCoords.Any())
		{
			if (_paths.TryGetValue((visitedMapCoords[visitedMapCoords.Count - 1], node.Point.coord), out IReadOnlyList<TextureRect> value))
			{
				float waitPerTick = SaveManager.Instance.PrefsSave.FastMode switch
				{
					FastModeType.Fast => 0.3f, 
					FastModeType.Normal => 0.8f, 
					_ => 0f, 
				} / (float)value.Count;
				foreach (TextureRect tick in value)
				{
					await Cmd.Wait(waitPerTick);
					((CanvasItem)tick).Modulate = StsColors.pathDotTraveled;
					Tween val = ((Node)this).CreateTween();
					val.TweenProperty((GodotObject)(object)tick, NodePath.op_Implicit("scale"), Variant.op_Implicit(_tickTraveledScale), 0.4).From(Variant.op_Implicit(Vector2.One * 1.7f)).SetEase((EaseType)1)
						.SetTrans((TransitionType)7);
				}
			}
		}
		_marker.SetMapPoint(node);
		await fadeOutTask;
		await RunManager.Instance.EnterMapCoord(coord);
		RefreshAllPointVisuals();
		PlayerVoteDictionary.Clear();
		RefreshAllMapPointVotes();
	}

	public void RefreshAllMapPointVotes()
	{
		foreach (NMapPoint value in _mapPointDictionary.Values)
		{
			value.VoteContainer.RefreshPlayerVotes();
		}
	}

	private void RemoveAllMapPointsAndPaths()
	{
		((Node)(object)_points).FreeChildren();
		((Node)(object)_pathsContainer).FreeChildren();
		((Node)(object)_bossPointNode)?.QueueFreeSafely();
		((Node)(object)_secondBossPointNode)?.QueueFreeSafely();
		((Node)(object)_startingPointNode)?.QueueFreeSafely();
	}

	private IReadOnlyList<TextureRect> CreatePath(Vector2 start, Vector2 end)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		List<TextureRect> list = new List<TextureRect>();
		Vector2 val = end - start;
		Vector2 val2 = ((Vector2)(ref val)).Normalized();
		float num = ((Vector2)(ref val2)).Angle() + (float)Math.PI / 2f;
		float num2 = ((Vector2)(ref start)).DistanceTo(end);
		int num3 = (int)(num2 / 22f) + 1;
		for (int i = 1; i < num3; i++)
		{
			float num4 = (float)i * 22f;
			TextureRect val3 = PreloadManager.Cache.GetScene("res://scenes/ui/map_dot.tscn").Instantiate<TextureRect>((GenEditState)0);
			((Control)val3).Position = start + val2 * num4;
			((Control)val3).Position = ((Control)val3).Position - new Vector2(((Control)this).Size.X * 0.5f - 20f, ((Control)this).Size.Y * 0.5f - 20f);
			((Control)val3).Position = ((Control)val3).Position + new Vector2(Rng.Chaotic.NextFloat(-3f, 3f), Rng.Chaotic.NextFloat(-3f, 3f));
			val3.FlipH = Rng.Chaotic.NextBool();
			((Control)val3).Rotation = num + Rng.Chaotic.NextGaussianFloat(0f, 0.1f);
			((CanvasItem)val3).Modulate = _runState.Act.MapUntraveledColor;
			((Node)(object)_pathsContainer).AddChildSafely((Node?)(object)val3);
			list.Add(val3);
		}
		return list;
	}

	public override void _Process(double delta)
	{
		if (((CanvasItem)this).IsVisibleInTree() && (_actAnimTween == null || !_actAnimTween.IsRunning()))
		{
			UpdateScrollPosition(delta);
		}
	}

	private void UpdateScrollPosition(double delta)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		if (_mapContainer.Position != _targetDragPos)
		{
			Control mapContainer = _mapContainer;
			Vector2 position = _mapContainer.Position;
			mapContainer.Position = ((Vector2)(ref position)).Lerp(_targetDragPos, (float)delta * 15f);
			position = _mapContainer.Position;
			if (((Vector2)(ref position)).DistanceTo(_targetDragPos) < 0.5f)
			{
				_mapContainer.Position = _targetDragPos;
			}
		}
		if (!_isDragging)
		{
			if (_targetDragPos.Y < -600f)
			{
				_targetDragPos = ((Vector2)(ref _targetDragPos)).Lerp(new Vector2(0f, -600f), (float)delta * 12f);
			}
			else if (_targetDragPos.Y > 1800f)
			{
				_targetDragPos = ((Vector2)(ref _targetDragPos)).Lerp(new Vector2(0f, 1800f), (float)delta * 12f);
			}
		}
		NGame.Instance.RemoteCursorContainer.ForceUpdateAllCursors();
	}

	public override void _GuiInput(InputEvent inputEvent)
	{
		if (((CanvasItem)this).IsVisibleInTree())
		{
			ProcessMouseEvent(inputEvent);
			ProcessScrollEvent(inputEvent);
		}
	}

	private void ProcessMouseEvent(InputEvent inputEvent)
	{
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Invalid comparison between Unknown and I8
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		ProcessMouseDrawingEvent(inputEvent);
		if (_drawingInput != null)
		{
			return;
		}
		if (_isDragging)
		{
			InputEventMouseMotion val = (InputEventMouseMotion)(object)((inputEvent is InputEventMouseMotion) ? inputEvent : null);
			if (val != null)
			{
				_targetDragPos += new Vector2(0f, val.Relative.Y);
				goto IL_00b4;
			}
		}
		InputEventMouseButton val2 = (InputEventMouseButton)(object)((inputEvent is InputEventMouseButton) ? inputEvent : null);
		if (val2 != null)
		{
			if ((long)val2.ButtonIndex == 1)
			{
				if (val2.Pressed && CanScroll())
				{
					_isDragging = true;
					_startDragPos = _mapContainer.Position;
					_targetDragPos = _startDragPos;
					TryCancelStartOfActAnim();
				}
				else
				{
					_isDragging = false;
				}
			}
			else if (!val2.Pressed)
			{
				_isDragging = false;
			}
		}
		goto IL_00b4;
		IL_00b4:
		InputEventMouseMotion val3 = (InputEventMouseMotion)(object)((inputEvent is InputEventMouseMotion) ? inputEvent : null);
		if (val3 != null && Drawings.IsLocalDrawing())
		{
			NMapDrawings drawings = Drawings;
			Transform2D globalTransform = ((CanvasItem)Drawings).GetGlobalTransform();
			drawings.UpdateCurrentLinePositionLocal(((Transform2D)(ref globalTransform)).Inverse() * ((InputEventMouse)val3).GlobalPosition);
		}
	}

	private void ProcessMouseDrawingEvent(InputEvent inputEvent)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Invalid comparison between Unknown and I8
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Invalid comparison between Unknown and I8
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		if (_isInputDisabled || (_actAnimTween != null && _actAnimTween.IsRunning()) || _drawingInput != null)
		{
			return;
		}
		InputEventMouseButton val = (InputEventMouseButton)(object)((inputEvent is InputEventMouseButton) ? inputEvent : null);
		if (val == null || !val.Pressed)
		{
			return;
		}
		if ((long)val.ButtonIndex == 2)
		{
			_drawingInput = NMapDrawingInput.Create(Drawings, DrawingMode.Drawing, stopOnMouseRelease: true);
		}
		else if ((long)val.ButtonIndex == 3)
		{
			_drawingInput = NMapDrawingInput.Create(Drawings, DrawingMode.Erasing, stopOnMouseRelease: true);
		}
		NMapDrawingInput? drawingInput = _drawingInput;
		if (drawingInput != null)
		{
			((GodotObject)drawingInput).Connect(NMapDrawingInput.SignalName.Finished, Callable.From((Action)delegate
			{
				_drawingInput = null;
				UpdateDrawingButtonStates();
			}), 0u);
		}
		((Node)(object)this).AddChildSafely((Node?)(object)_drawingInput);
	}

	private void ProcessScrollEvent(InputEvent inputEvent)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		if (CanScroll())
		{
			_targetDragPos += new Vector2(0f, ScrollHelper.GetDragForScrollEvent(inputEvent));
			if ((inputEvent is InputEventMouseButton || inputEvent is InputEventPanGesture) ? true : false)
			{
				TryCancelStartOfActAnim();
			}
		}
	}

	private void ProcessControllerEvent(InputEvent inputEvent)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		if (inputEvent.IsActionPressed(MegaInput.up, false, false) && CanScroll())
		{
			_targetDragPos += new Vector2(0f, _controllerScrollAmount);
			TryCancelStartOfActAnim();
		}
		else if (inputEvent.IsActionPressed(MegaInput.down, false, false) && CanScroll())
		{
			_targetDragPos += new Vector2(0f, 0f - _controllerScrollAmount);
			TryCancelStartOfActAnim();
		}
		else if (inputEvent.IsActionPressed(MegaInput.right, false, false) || inputEvent.IsActionPressed(MegaInput.left, false, false) || inputEvent.IsActionPressed(MegaInput.select, false, false))
		{
			if (_runState.ActFloor == 0)
			{
				_targetDragPos = new Vector2(0f, -600f);
				return;
			}
			int num = _runState.CurrentMapCoord?.row ?? 0;
			_targetDragPos = new Vector2(0f, -600f + (float)num * _distY);
		}
	}

	public void SetTravelEnabled(bool enabled)
	{
		IsTravelEnabled = enabled && Hook.ShouldProceedToNextMapPoint(_runState);
		RefreshAllPointVisuals();
	}

	public void SetDebugTravelEnabled(bool enabled)
	{
		IsDebugTravelEnabled = enabled;
		RefreshAllPointVisuals();
	}

	public void RefreshAllPointVisuals()
	{
		foreach (NMapPoint value in _mapPointDictionary.Values)
		{
			value.RefreshVisualsInstantly();
		}
		((Control)(object)_mapPointDictionary.Values.FirstOrDefault((NMapPoint n) => n.IsEnabled))?.TryGrabFocus();
	}

	private void PlayStartOfActAnimation()
	{
		if (_hasPlayedAnimation)
		{
			Log.Warn("Tried to play start of act animation twice! Ignoring second try");
			return;
		}
		_hasPlayedAnimation = true;
		NActBanner child = NActBanner.Create(_runState.Act, _runState.CurrentActIndex);
		((Node)(object)NRun.Instance?.GlobalUi.MapScreen).AddChildSafely((Node?)(object)child);
		TaskHelper.RunSafely(StartOfActAnim());
	}

	private async Task StartOfActAnim()
	{
		_mapContainer.Position = new Vector2(0f, 1800f);
		Tween? actAnimTween = _actAnimTween;
		if (actAnimTween != null)
		{
			actAnimTween.Kill();
		}
		_actAnimTween = ((Node)this).CreateTween().SetParallel(true);
		_actAnimTween.TweenInterval(_mapAnimStartDelay);
		_actAnimTween.Chain();
		Vector2 targetDragPos = default(Vector2);
		((Vector2)(ref targetDragPos))._002Ector(0f, -600f);
		_actAnimTween.TweenProperty((GodotObject)(object)_mapContainer, NodePath.op_Implicit("position:y"), Variant.op_Implicit(-600f), _mapAnimDuration).SetEase((EaseType)2).SetTrans((TransitionType)5);
		_actAnimTween.TweenCallback(Callable.From((Action)SetInterruptable)).SetDelay(_mapAnimDuration * 0.25);
		_targetDragPos = targetDragPos;
		await ((GodotObject)this).ToSignal((GodotObject)(object)_actAnimTween, SignalName.Finished);
		_actAnimTween = null;
		InitMapPrompt();
	}

	private void InitMapPrompt()
	{
		if (!TestMode.IsOn && !SaveManager.Instance.SeenFtue("map_select_ftue"))
		{
			TaskHelper.RunSafely(MapFtueCheck());
		}
	}

	private async Task MapFtueCheck()
	{
		await Task.Delay(100);
		NMapSelectFtue nMapSelectFtue = NMapSelectFtue.Create((Control)(object)_startingPointNode);
		NModalContainer.Instance.Add((Node)(object)nMapSelectFtue);
		SaveManager.Instance.MarkFtueAsComplete("map_select_ftue");
		await nMapSelectFtue.WaitForPlayerToConfirm();
	}

	private void SetInterruptable()
	{
		_canInterruptAnim = true;
	}

	private bool CanScroll()
	{
		if (_actAnimTween == null || _canInterruptAnim)
		{
			return !_isInputDisabled;
		}
		return false;
	}

	private void TryCancelStartOfActAnim()
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		if (_actAnimTween != null && _canInterruptAnim)
		{
			Tween? actAnimTween = _actAnimTween;
			if (actAnimTween != null)
			{
				actAnimTween.Kill();
			}
			_actAnimTween = null;
			_canInterruptAnim = false;
			_isDragging = false;
			_targetDragPos = new Vector2(0f, -600f);
			TaskHelper.RunSafely(DisableInputVeryBriefly());
		}
	}

	private async Task DisableInputVeryBriefly()
	{
		_isInputDisabled = true;
		_drawingInput?.StopDrawing();
		await Task.Delay(200);
		_isInputDisabled = false;
		InitMapPrompt();
	}

	private void OnVisibilityChanged()
	{
		if (((CanvasItem)this).Visible)
		{
			RunManager.Instance.InputSynchronizer.StartOverridingCursorPositioning(this);
			return;
		}
		_isDragging = false;
		RunManager.Instance.InputSynchronizer.StopOverridingCursorPositioning();
		_backButton.Disable();
		Drawings.StopLineLocal();
		Drawings.SetDrawingModeLocal(DrawingMode.None);
		_drawingInput?.StopDrawing();
		UpdateDrawingButtonStates();
	}

	private void OnCapstoneChanged()
	{
		((CanvasItem)_backstop).Visible = !(NCapstoneContainer.Instance?.InUse ?? false);
		if (((CanvasItem)this).Visible)
		{
			if (!((CanvasItem)_backstop).Visible)
			{
				NRun.Instance.GlobalUi.TopBar.Map.StopOscillation();
			}
			else
			{
				NRun.Instance.GlobalUi.TopBar.Map.StartOscillation();
			}
		}
	}

	public void Close(bool animateOut = true)
	{
		if (IsOpen)
		{
			IsOpen = false;
			((Control)this).FocusMode = (FocusModeEnum)0;
			NRun.Instance.GlobalUi.TopBar.Map.StopOscillation();
			NHotkeyManager.Instance.RemoveHotkeyPressedBinding(StringName.op_Implicit(MegaInput.accept), OnLegendHotkeyPressed);
			NHotkeyManager.Instance.RemoveHotkeyPressedBinding(StringName.op_Implicit(MegaInput.viewExhaustPileAndTabRight), OnDrawingToolsHotkeyPressed);
			if (RunManager.Instance.IsSinglePlayerOrFakeMultiplayer)
			{
				CombatManager.Instance.Unpause();
			}
			_backButton.Disable();
			ActiveScreenContext.Instance.Update();
			EmitSignalClosed();
			if (animateOut)
			{
				TaskHelper.RunSafely(AnimClose());
				SfxCmd.Play("event:/sfx/ui/map/map_close");
			}
			else
			{
				((CanvasItem)this).Visible = false;
				((Node)this).ProcessMode = (ProcessModeEnum)4;
			}
		}
	}

	private async Task AnimClose()
	{
		Tween? tween = _tween;
		if (tween != null)
		{
			tween.Kill();
		}
		_tween = ((Node)this).CreateTween().SetParallel(true);
		_tween.TweenProperty((GodotObject)(object)_backstop, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0f), 0.15);
		_tween.TweenProperty((GodotObject)(object)_points, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0f), 0.15);
		_tween.TweenProperty((GodotObject)(object)_mapContainer, NodePath.op_Implicit("modulate"), Variant.op_Implicit(StsColors.transparentBlack), 0.25).SetDelay(0.1);
		_tween.TweenProperty((GodotObject)(object)_mapContainer, NodePath.op_Implicit("position:y"), Variant.op_Implicit(_mapContainer.Position.Y + 200f), 0.25).SetEase((EaseType)1).SetTrans((TransitionType)7);
		_tween.TweenProperty((GodotObject)(object)_mapLegend, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0f), 0.15);
		_tween.TweenProperty((GodotObject)(object)_mapLegend, NodePath.op_Implicit("position:x"), Variant.op_Implicit(MapLegendX + 120f), 0.25).SetEase((EaseType)1).SetTrans((TransitionType)7);
		_tween.TweenProperty((GodotObject)(object)_drawingTools, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0f), 0.15);
		await ((GodotObject)this).ToSignal((GodotObject)(object)_tween, SignalName.Finished);
		((CanvasItem)this).Visible = false;
		((Node)this).ProcessMode = (ProcessModeEnum)4;
	}

	public NMapScreen Open(bool isOpenedFromTopBar = false)
	{
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_0241: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_0284: Unknown result type (might be due to invalid IL or missing references)
		//IL_029a: Unknown result type (might be due to invalid IL or missing references)
		//IL_029f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0318: Unknown result type (might be due to invalid IL or missing references)
		//IL_0342: Unknown result type (might be due to invalid IL or missing references)
		//IL_0347: Unknown result type (might be due to invalid IL or missing references)
		//IL_035a: Unknown result type (might be due to invalid IL or missing references)
		//IL_035f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0380: Unknown result type (might be due to invalid IL or missing references)
		//IL_0385: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0420: Unknown result type (might be due to invalid IL or missing references)
		//IL_0425: Unknown result type (might be due to invalid IL or missing references)
		//IL_0462: Unknown result type (might be due to invalid IL or missing references)
		if (IsOpen)
		{
			return this;
		}
		IsOpen = true;
		((CanvasItem)this).Visible = true;
		_backButton.MoveToHidePosition();
		NHotkeyManager.Instance.PushHotkeyPressedBinding(StringName.op_Implicit(MegaInput.accept), OnLegendHotkeyPressed);
		NHotkeyManager.Instance.PushHotkeyPressedBinding(StringName.op_Implicit(MegaInput.viewExhaustPileAndTabRight), OnDrawingToolsHotkeyPressed);
		if (_runState.ActFloor > 0)
		{
			_backButton.Enable();
		}
		((Node)this).ProcessMode = (ProcessModeEnum)0;
		NRun.Instance.GlobalUi.TopBar.Map.StartOscillation();
		if (RunManager.Instance.IsSinglePlayerOrFakeMultiplayer)
		{
			CombatManager.Instance.Pause();
		}
		Color modulate;
		if (((_runState.CurrentActIndex == 0 && _runState.ExtraFields.StartedWithNeow) ? (_runState.ActFloor == 1) : (_runState.ActFloor == 0)) && !_hasPlayedAnimation)
		{
			if (!isOpenedFromTopBar && (SaveManager.Instance.PrefsSave.FastMode < FastModeType.Fast || !SaveManager.Instance.SeenFtue("map_select_ftue")))
			{
				PlayStartOfActAnimation();
			}
			else
			{
				_hasPlayedAnimation = true;
				Control mapContainer = _mapContainer;
				Vector2 position = _mapContainer.Position;
				position.Y = -600f;
				mapContainer.Position = position;
				_targetDragPos = new Vector2(0f, -600f);
				NActBanner child = NActBanner.Create(_runState.Act, _runState.CurrentActIndex);
				((Node)(object)NRun.Instance.GlobalUi.MapScreen).AddChildSafely((Node?)(object)child);
			}
		}
		else
		{
			int num = _runState.CurrentMapCoord?.row ?? 0;
			_targetDragPos = new Vector2(0f, -600f + (float)num * _distY);
			_mapContainer.Position = new Vector2(0f, -600f + (float)num * _distY);
			Control points = _points;
			modulate = ((CanvasItem)_points).Modulate;
			modulate.A = 0f;
			((CanvasItem)points).Modulate = modulate;
			Control backstop = _backstop;
			modulate = ((CanvasItem)_backstop).Modulate;
			modulate.A = 0f;
			((CanvasItem)backstop).Modulate = modulate;
			((CanvasItem)_mapLegend).Modulate = StsColors.transparentBlack;
			((CanvasItem)_drawingTools).Modulate = StsColors.transparentBlack;
		}
		Control mapLegend = _mapLegend;
		modulate = ((CanvasItem)_mapLegend).Modulate;
		modulate.A = 0f;
		((CanvasItem)mapLegend).Modulate = modulate;
		Control drawingTools = _drawingTools;
		modulate = ((CanvasItem)_drawingTools).Modulate;
		modulate.A = 0f;
		((CanvasItem)drawingTools).Modulate = modulate;
		Tween? tween = _tween;
		if (tween != null)
		{
			tween.Kill();
		}
		_tween = ((Node)this).CreateTween().SetParallel(true);
		_tween.TweenProperty((GodotObject)(object)_backstop, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0.85f), 0.25);
		_tween.TweenProperty((GodotObject)(object)_mapContainer, NodePath.op_Implicit("modulate"), Variant.op_Implicit(Colors.White), 0.25).From(Variant.op_Implicit(StsColors.transparentBlack));
		_tween.TweenProperty((GodotObject)(object)_mapLegend, NodePath.op_Implicit("modulate"), Variant.op_Implicit(Colors.White), 0.25).SetDelay(0.1);
		_tween.TweenProperty((GodotObject)(object)_mapLegend, NodePath.op_Implicit("position:x"), Variant.op_Implicit(MapLegendX), 0.25).From(Variant.op_Implicit(MapLegendX + 120f)).SetEase((EaseType)1)
			.SetTrans((TransitionType)10)
			.SetDelay(0.1);
		_tween.TweenProperty((GodotObject)(object)_drawingTools, NodePath.op_Implicit("modulate"), Variant.op_Implicit(Colors.White), 0.25).SetDelay(0.2);
		_tween.TweenProperty((GodotObject)(object)_points, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(1f), 0.25).SetDelay(0.1);
		RecalculateTravelability();
		if (_runState.VisitedMapCoords.Count != 0)
		{
			IReadOnlyList<MapCoord> visitedMapCoords = _runState.VisitedMapCoords;
			MapCoord key = visitedMapCoords[visitedMapCoords.Count - 1];
			if (_bossPointNode.Point.coord.row != key.row && _startingPointNode.Point.coord.row != key.row)
			{
				NMapPoint mapPoint = _mapPointDictionary[key];
				_marker.SetMapPoint(mapPoint);
			}
		}
		SfxCmd.Play("event:/sfx/ui/map/map_open");
		ActiveScreenContext.Instance.Update();
		EmitSignalOpened();
		NMapPoint nMapPoint = _mapPointDictionary.Values.FirstOrDefault((NMapPoint n) => n.IsEnabled);
		if (nMapPoint == null)
		{
			((Control)this).FocusMode = (FocusModeEnum)2;
		}
		return this;
	}

	private void OnBackButtonPressed(NButton _)
	{
		Close();
	}

	public override void _Input(InputEvent inputEvent)
	{
		if ((((Node)this).GetViewport().GuiGetFocusOwner() is NMapPoint || ((Control)this).HasFocus()) && ActiveScreenContext.Instance.IsCurrent(this))
		{
			if (inputEvent.IsActionReleased(DebugHotkey.unlockCharacters, false))
			{
				((CanvasItem)_mapLegend).Visible = !((CanvasItem)_mapLegend).Visible;
				((Node)(object)NGame.Instance).AddChildSafely((Node?)(object)NFullscreenTextVfx.Create(((CanvasItem)_mapLegend).Visible ? "Show Legend" : "Hide Legend"));
			}
			if (((CanvasItem)this).IsVisibleInTree())
			{
				ProcessControllerEvent(inputEvent);
			}
		}
	}

	private void OnMapDrawingButtonPressed(NButton _)
	{
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		NMapDrawingInput drawingInput = _drawingInput;
		if (drawingInput != null && drawingInput.DrawingMode == DrawingMode.Drawing)
		{
			_drawingInput?.StopDrawing();
		}
		else
		{
			_drawingInput?.StopDrawing();
			_drawingInput = NMapDrawingInput.Create(Drawings, DrawingMode.Drawing);
			((GodotObject)_drawingInput).Connect(NMapDrawingInput.SignalName.Finished, Callable.From((Action)delegate
			{
				_drawingInput = null;
				UpdateDrawingButtonStates();
			}), 0u);
			((Node)(object)this).AddChildSafely((Node?)(object)_drawingInput);
		}
		UpdateDrawingButtonStates();
	}

	private void OnMapErasingButtonPressed(NButton _)
	{
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		NMapDrawingInput drawingInput = _drawingInput;
		if (drawingInput != null && drawingInput.DrawingMode == DrawingMode.Erasing)
		{
			_drawingInput?.StopDrawing();
		}
		else
		{
			_drawingInput?.StopDrawing();
			_drawingInput = NMapDrawingInput.Create(Drawings, DrawingMode.Erasing);
			((GodotObject)_drawingInput).Connect(NMapDrawingInput.SignalName.Finished, Callable.From((Action)delegate
			{
				_drawingInput = null;
				UpdateDrawingButtonStates();
			}), 0u);
			((Node)(object)this).AddChildSafely((Node?)(object)_drawingInput);
		}
		UpdateDrawingButtonStates();
	}

	private void UpdateDrawingButtonStates()
	{
		_mapDrawingButton.SetIsDrawing(Drawings.GetLocalDrawingMode() == DrawingMode.Drawing);
		_mapErasingButton.SetIsErasing(Drawings.GetLocalDrawingMode() == DrawingMode.Erasing);
	}

	private void OnClearMapDrawingButtonPressed(NButton _)
	{
		Drawings.ClearDrawnLinesLocal();
		SfxCmd.Play("event:/sfx/ui/map/map_erase");
		UpdateDrawingButtonStates();
	}

	public void HighlightPointType(MapPointType pointType)
	{
		this.PointTypeHighlighted?.Invoke(pointType);
	}

	public void PingMapCoord(MapCoord coord, Player player)
	{
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		if (!_mapPointDictionary.TryGetValue(coord, out NMapPoint value))
		{
			Log.Error($"Someone tried to ping map coord {coord} that doesn't exist!");
			return;
		}
		NMapPingVfx nMapPingVfx = NMapPingVfx.Create();
		((CanvasItem)nMapPingVfx).Modulate = player.Character.MapDrawingColor;
		((Node)(object)value).AddChildSafely((Node?)(object)nMapPingVfx);
		((Node)value).MoveChild((Node)(object)nMapPingVfx, 0);
		((Control)nMapPingVfx).Position = Vector2.Zero;
		((Control)nMapPingVfx).Size = ((Control)nMapPingVfx).Size * (((Control)value).Size.X * (1f / 64f));
		((Control)nMapPingVfx).PivotOffset = ((Control)nMapPingVfx).Size * 0.5f;
		NRun.Instance.GlobalUi.MultiplayerPlayerContainer.FlashPlayerReady(player);
		NDebugAudioManager.Instance.Play("map_ping.mp3", 1f, PitchVariance.Medium);
	}

	private void OnLegendHotkeyPressed()
	{
		List<NMapLegendItem> list = ((IEnumerable)((Node)_legendItems).GetChildren(false)).OfType<NMapLegendItem>().ToList();
		if (list.Any((NMapLegendItem c) => (object)((Node)this).GetViewport().GuiGetFocusOwner() == c))
		{
			((Control)(object)_mapPointDictionary.Values.FirstOrDefault((NMapPoint n) => n.IsEnabled))?.TryGrabFocus();
			return;
		}
		NMapPoint nMapPoint = _mapPointDictionary.Values.LastOrDefault((NMapPoint n) => n.IsEnabled);
		if (nMapPoint != null)
		{
			foreach (NMapLegendItem item in list)
			{
				if (nMapPoint != null)
				{
					((Control)item).FocusNeighborLeft = ((Node)nMapPoint).GetPath();
				}
				else
				{
					((Control)item).FocusNeighborLeft = ((Node)this).GetPath();
				}
			}
		}
		((Control)(object)list[0]).TryGrabFocus();
	}

	private void OnDrawingToolsHotkeyPressed()
	{
		NMapDrawingInput drawingInput = _drawingInput;
		if (drawingInput != null && drawingInput.DrawingMode == DrawingMode.Erasing)
		{
			((Control)(object)_mapErasingButton).TryGrabFocus();
		}
		else
		{
			((Control)(object)_mapDrawingButton).TryGrabFocus();
		}
	}

	public Vector2 GetNetPositionFromScreenPosition(Vector2 screenPosition)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		Transform2D globalTransformWithCanvas = ((CanvasItem)_mapBgContainer).GetGlobalTransformWithCanvas();
		Vector2 val = ((Transform2D)(ref globalTransformWithCanvas)).Inverse() * screenPosition;
		Vector2 val2 = ((Control)_mapBgContainer).Size * 0.5f;
		Vector2 val3 = default(Vector2);
		((Vector2)(ref val3))._002Ector(960f, val2.Y);
		return (val - val2) / val3;
	}

	private Vector2 GetMapPositionFromNetPosition(Vector2 netPosition)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		Vector2 val = ((Control)_mapBgContainer).Size * 0.5f;
		Vector2 val2 = default(Vector2);
		((Vector2)(ref val2))._002Ector(960f, val.Y);
		return netPosition * val2 + val;
	}

	public Vector2 GetScreenPositionFromNetPosition(Vector2 netPosition)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		Vector2 mapPositionFromNetPosition = GetMapPositionFromNetPosition(netPosition);
		return ((CanvasItem)_mapBgContainer).GetGlobalTransformWithCanvas() * mapPositionFromNetPosition;
	}

	public bool IsNodeOnScreen(NMapPoint mapPoint)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		float y = ((Control)mapPoint).GlobalPosition.Y;
		if (y > 0f)
		{
			return y < ((Control)this).Size.Y;
		}
		return false;
	}

	public void CleanUp()
	{
		if (RunManager.Instance.IsSinglePlayerOrFakeMultiplayer)
		{
			CombatManager.Instance.Unpause();
		}
	}

	private void UpdateHotkeyDisplay()
	{
		((CanvasItem)_legendHotkeyIcon).Visible = NControllerManager.Instance.IsUsingController;
		_legendHotkeyIcon.Texture = NInputManager.Instance.GetHotkeyIcon(StringName.op_Implicit(MegaInput.accept));
		((CanvasItem)_drawingToolsHotkeyIcon).Visible = NControllerManager.Instance.IsUsingController;
		_drawingToolsHotkeyIcon.Texture = NInputManager.Instance.GetHotkeyIcon(StringName.op_Implicit(MegaInput.viewExhaustPileAndTabRight));
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
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Expected O, but got Unknown
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d7: Expected O, but got Unknown
		//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0303: Unknown result type (might be due to invalid IL or missing references)
		//IL_032b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0336: Expected O, but got Unknown
		//IL_0331: Unknown result type (might be due to invalid IL or missing references)
		//IL_033c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0362: Unknown result type (might be due to invalid IL or missing references)
		//IL_038a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0395: Expected O, but got Unknown
		//IL_0390: Unknown result type (might be due to invalid IL or missing references)
		//IL_039b: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f4: Expected O, but got Unknown
		//IL_03ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0420: Unknown result type (might be due to invalid IL or missing references)
		//IL_0448: Unknown result type (might be due to invalid IL or missing references)
		//IL_0453: Expected O, but got Unknown
		//IL_044e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0459: Unknown result type (might be due to invalid IL or missing references)
		//IL_047f: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0501: Unknown result type (might be due to invalid IL or missing references)
		//IL_0527: Unknown result type (might be due to invalid IL or missing references)
		//IL_0530: Unknown result type (might be due to invalid IL or missing references)
		//IL_0556: Unknown result type (might be due to invalid IL or missing references)
		//IL_055f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0585: Unknown result type (might be due to invalid IL or missing references)
		//IL_058e: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_05bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0612: Unknown result type (might be due to invalid IL or missing references)
		//IL_061b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0641: Unknown result type (might be due to invalid IL or missing references)
		//IL_064a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0670: Unknown result type (might be due to invalid IL or missing references)
		//IL_0679: Unknown result type (might be due to invalid IL or missing references)
		//IL_069f: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_06cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0703: Expected O, but got Unknown
		//IL_06fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0721: Unknown result type (might be due to invalid IL or missing references)
		//IL_072c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0752: Unknown result type (might be due to invalid IL or missing references)
		//IL_077a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0785: Expected O, but got Unknown
		//IL_0780: Unknown result type (might be due to invalid IL or missing references)
		//IL_078b: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_07e4: Expected O, but got Unknown
		//IL_07df: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0810: Unknown result type (might be due to invalid IL or missing references)
		//IL_0838: Unknown result type (might be due to invalid IL or missing references)
		//IL_0843: Expected O, but got Unknown
		//IL_083e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0849: Unknown result type (might be due to invalid IL or missing references)
		//IL_086f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0897: Unknown result type (might be due to invalid IL or missing references)
		//IL_08a2: Expected O, but got Unknown
		//IL_089d: Unknown result type (might be due to invalid IL or missing references)
		//IL_08a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_08d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_08fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0925: Unknown result type (might be due to invalid IL or missing references)
		//IL_0930: Expected O, but got Unknown
		//IL_092b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0936: Unknown result type (might be due to invalid IL or missing references)
		//IL_095c: Unknown result type (might be due to invalid IL or missing references)
		//IL_097f: Unknown result type (might be due to invalid IL or missing references)
		//IL_098a: Unknown result type (might be due to invalid IL or missing references)
		//IL_09b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_09b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_09df: Unknown result type (might be due to invalid IL or missing references)
		//IL_09e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a0e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a31: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a3c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a62: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a85: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a90: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ab6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ad9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ae4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b0a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b32: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b3d: Expected O, but got Unknown
		//IL_0b38: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b43: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b69: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b72: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b98: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ba1: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(42);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._ExitTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.GetLineEndpoint, new PropertyInfo((Type)5, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("point"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.RecalculateTravelability, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.InitMapVotes, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnMapPointSelectedLocally, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("point"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.RefreshAllMapPointVotes, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.RemoveAllMapPointsAndPaths, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Process, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)3, StringName.op_Implicit("delta"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UpdateScrollPosition, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)3, StringName.op_Implicit("delta"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._GuiInput, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("inputEvent"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("InputEvent"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ProcessMouseEvent, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("inputEvent"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("InputEvent"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ProcessMouseDrawingEvent, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("inputEvent"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("InputEvent"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ProcessScrollEvent, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("inputEvent"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("InputEvent"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ProcessControllerEvent, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("inputEvent"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("InputEvent"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetTravelEnabled, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)1, StringName.op_Implicit("enabled"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetDebugTravelEnabled, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)1, StringName.op_Implicit("enabled"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.RefreshAllPointVisuals, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.PlayStartOfActAnimation, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.InitMapPrompt, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetInterruptable, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.CanScroll, new PropertyInfo((Type)1, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.TryCancelStartOfActAnim, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnVisibilityChanged, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnCapstoneChanged, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.Close, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)1, StringName.op_Implicit("animateOut"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.Open, new PropertyInfo((Type)24, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)1, StringName.op_Implicit("isOpenedFromTopBar"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnBackButtonPressed, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Input, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("inputEvent"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("InputEvent"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnMapDrawingButtonPressed, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnMapErasingButtonPressed, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UpdateDrawingButtonStates, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnClearMapDrawingButtonPressed, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.HighlightPointType, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("pointType"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnLegendHotkeyPressed, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnDrawingToolsHotkeyPressed, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.GetNetPositionFromScreenPosition, new PropertyInfo((Type)5, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)5, StringName.op_Implicit("screenPosition"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.GetMapPositionFromNetPosition, new PropertyInfo((Type)5, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)5, StringName.op_Implicit("netPosition"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.GetScreenPositionFromNetPosition, new PropertyInfo((Type)5, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)5, StringName.op_Implicit("netPosition"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.IsNodeOnScreen, new PropertyInfo((Type)1, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("mapPoint"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.CleanUp, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UpdateHotkeyDisplay, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02db: Unknown result type (might be due to invalid IL or missing references)
		//IL_030e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0333: Unknown result type (might be due to invalid IL or missing references)
		//IL_0358: Unknown result type (might be due to invalid IL or missing references)
		//IL_037d: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0418: Unknown result type (might be due to invalid IL or missing references)
		//IL_043d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0470: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_04dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0510: Unknown result type (might be due to invalid IL or missing references)
		//IL_0543: Unknown result type (might be due to invalid IL or missing references)
		//IL_0576: Unknown result type (might be due to invalid IL or missing references)
		//IL_059b: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_0601: Unknown result type (might be due to invalid IL or missing references)
		//IL_0626: Unknown result type (might be due to invalid IL or missing references)
		//IL_064b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0673: Unknown result type (might be due to invalid IL or missing references)
		//IL_0678: Unknown result type (might be due to invalid IL or missing references)
		//IL_067d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0681: Unknown result type (might be due to invalid IL or missing references)
		//IL_0686: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_06bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_06fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0732: Unknown result type (might be due to invalid IL or missing references)
		//IL_0737: Unknown result type (might be due to invalid IL or missing references)
		//IL_078a: Unknown result type (might be due to invalid IL or missing references)
		//IL_075b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0780: Unknown result type (might be due to invalid IL or missing references)
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
		if ((ref method) == MethodName.GetLineEndpoint && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			Vector2 lineEndpoint = GetLineEndpoint(VariantUtils.ConvertTo<NMapPoint>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<Vector2>(ref lineEndpoint);
			return true;
		}
		if ((ref method) == MethodName.RecalculateTravelability && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			RecalculateTravelability();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.InitMapVotes && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			InitMapVotes();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnMapPointSelectedLocally && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OnMapPointSelectedLocally(VariantUtils.ConvertTo<NMapPoint>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.RefreshAllMapPointVotes && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			RefreshAllMapPointVotes();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.RemoveAllMapPointsAndPaths && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			RemoveAllMapPointsAndPaths();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._Process && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			((Node)this)._Process(VariantUtils.ConvertTo<double>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.UpdateScrollPosition && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			UpdateScrollPosition(VariantUtils.ConvertTo<double>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._GuiInput && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			((Control)this)._GuiInput(VariantUtils.ConvertTo<InputEvent>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.ProcessMouseEvent && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			ProcessMouseEvent(VariantUtils.ConvertTo<InputEvent>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.ProcessMouseDrawingEvent && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			ProcessMouseDrawingEvent(VariantUtils.ConvertTo<InputEvent>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.ProcessScrollEvent && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			ProcessScrollEvent(VariantUtils.ConvertTo<InputEvent>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.ProcessControllerEvent && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			ProcessControllerEvent(VariantUtils.ConvertTo<InputEvent>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetTravelEnabled && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			SetTravelEnabled(VariantUtils.ConvertTo<bool>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetDebugTravelEnabled && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			SetDebugTravelEnabled(VariantUtils.ConvertTo<bool>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.RefreshAllPointVisuals && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			RefreshAllPointVisuals();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.PlayStartOfActAnimation && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			PlayStartOfActAnimation();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.InitMapPrompt && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			InitMapPrompt();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetInterruptable && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			SetInterruptable();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.CanScroll && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			bool flag = CanScroll();
			ret = VariantUtils.CreateFrom<bool>(ref flag);
			return true;
		}
		if ((ref method) == MethodName.TryCancelStartOfActAnim && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			TryCancelStartOfActAnim();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnVisibilityChanged && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnVisibilityChanged();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnCapstoneChanged && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnCapstoneChanged();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.Close && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			Close(VariantUtils.ConvertTo<bool>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.Open && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			NMapScreen nMapScreen = Open(VariantUtils.ConvertTo<bool>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<NMapScreen>(ref nMapScreen);
			return true;
		}
		if ((ref method) == MethodName.OnBackButtonPressed && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OnBackButtonPressed(VariantUtils.ConvertTo<NButton>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._Input && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			((Node)this)._Input(VariantUtils.ConvertTo<InputEvent>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnMapDrawingButtonPressed && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OnMapDrawingButtonPressed(VariantUtils.ConvertTo<NButton>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnMapErasingButtonPressed && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OnMapErasingButtonPressed(VariantUtils.ConvertTo<NButton>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.UpdateDrawingButtonStates && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			UpdateDrawingButtonStates();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnClearMapDrawingButtonPressed && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OnClearMapDrawingButtonPressed(VariantUtils.ConvertTo<NButton>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.HighlightPointType && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			HighlightPointType(VariantUtils.ConvertTo<MapPointType>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnLegendHotkeyPressed && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnLegendHotkeyPressed();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnDrawingToolsHotkeyPressed && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnDrawingToolsHotkeyPressed();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.GetNetPositionFromScreenPosition && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			Vector2 netPositionFromScreenPosition = GetNetPositionFromScreenPosition(VariantUtils.ConvertTo<Vector2>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<Vector2>(ref netPositionFromScreenPosition);
			return true;
		}
		if ((ref method) == MethodName.GetMapPositionFromNetPosition && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			Vector2 mapPositionFromNetPosition = GetMapPositionFromNetPosition(VariantUtils.ConvertTo<Vector2>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<Vector2>(ref mapPositionFromNetPosition);
			return true;
		}
		if ((ref method) == MethodName.GetScreenPositionFromNetPosition && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			Vector2 screenPositionFromNetPosition = GetScreenPositionFromNetPosition(VariantUtils.ConvertTo<Vector2>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<Vector2>(ref screenPositionFromNetPosition);
			return true;
		}
		if ((ref method) == MethodName.IsNodeOnScreen && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			bool flag2 = IsNodeOnScreen(VariantUtils.ConvertTo<NMapPoint>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<bool>(ref flag2);
			return true;
		}
		if ((ref method) == MethodName.CleanUp && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			CleanUp();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.UpdateHotkeyDisplay && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			UpdateHotkeyDisplay();
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
		if ((ref method) == MethodName.GetLineEndpoint)
		{
			return true;
		}
		if ((ref method) == MethodName.RecalculateTravelability)
		{
			return true;
		}
		if ((ref method) == MethodName.InitMapVotes)
		{
			return true;
		}
		if ((ref method) == MethodName.OnMapPointSelectedLocally)
		{
			return true;
		}
		if ((ref method) == MethodName.RefreshAllMapPointVotes)
		{
			return true;
		}
		if ((ref method) == MethodName.RemoveAllMapPointsAndPaths)
		{
			return true;
		}
		if ((ref method) == MethodName._Process)
		{
			return true;
		}
		if ((ref method) == MethodName.UpdateScrollPosition)
		{
			return true;
		}
		if ((ref method) == MethodName._GuiInput)
		{
			return true;
		}
		if ((ref method) == MethodName.ProcessMouseEvent)
		{
			return true;
		}
		if ((ref method) == MethodName.ProcessMouseDrawingEvent)
		{
			return true;
		}
		if ((ref method) == MethodName.ProcessScrollEvent)
		{
			return true;
		}
		if ((ref method) == MethodName.ProcessControllerEvent)
		{
			return true;
		}
		if ((ref method) == MethodName.SetTravelEnabled)
		{
			return true;
		}
		if ((ref method) == MethodName.SetDebugTravelEnabled)
		{
			return true;
		}
		if ((ref method) == MethodName.RefreshAllPointVisuals)
		{
			return true;
		}
		if ((ref method) == MethodName.PlayStartOfActAnimation)
		{
			return true;
		}
		if ((ref method) == MethodName.InitMapPrompt)
		{
			return true;
		}
		if ((ref method) == MethodName.SetInterruptable)
		{
			return true;
		}
		if ((ref method) == MethodName.CanScroll)
		{
			return true;
		}
		if ((ref method) == MethodName.TryCancelStartOfActAnim)
		{
			return true;
		}
		if ((ref method) == MethodName.OnVisibilityChanged)
		{
			return true;
		}
		if ((ref method) == MethodName.OnCapstoneChanged)
		{
			return true;
		}
		if ((ref method) == MethodName.Close)
		{
			return true;
		}
		if ((ref method) == MethodName.Open)
		{
			return true;
		}
		if ((ref method) == MethodName.OnBackButtonPressed)
		{
			return true;
		}
		if ((ref method) == MethodName._Input)
		{
			return true;
		}
		if ((ref method) == MethodName.OnMapDrawingButtonPressed)
		{
			return true;
		}
		if ((ref method) == MethodName.OnMapErasingButtonPressed)
		{
			return true;
		}
		if ((ref method) == MethodName.UpdateDrawingButtonStates)
		{
			return true;
		}
		if ((ref method) == MethodName.OnClearMapDrawingButtonPressed)
		{
			return true;
		}
		if ((ref method) == MethodName.HighlightPointType)
		{
			return true;
		}
		if ((ref method) == MethodName.OnLegendHotkeyPressed)
		{
			return true;
		}
		if ((ref method) == MethodName.OnDrawingToolsHotkeyPressed)
		{
			return true;
		}
		if ((ref method) == MethodName.GetNetPositionFromScreenPosition)
		{
			return true;
		}
		if ((ref method) == MethodName.GetMapPositionFromNetPosition)
		{
			return true;
		}
		if ((ref method) == MethodName.GetScreenPositionFromNetPosition)
		{
			return true;
		}
		if ((ref method) == MethodName.IsNodeOnScreen)
		{
			return true;
		}
		if ((ref method) == MethodName.CleanUp)
		{
			return true;
		}
		if ((ref method) == MethodName.UpdateHotkeyDisplay)
		{
			return true;
		}
		return ((Control)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		//IL_0297: Unknown result type (might be due to invalid IL or missing references)
		//IL_029c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName.IsOpen)
		{
			IsOpen = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName.IsTravelEnabled)
		{
			IsTravelEnabled = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName.IsDebugTravelEnabled)
		{
			IsDebugTravelEnabled = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName.IsTraveling)
		{
			IsTraveling = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName.Drawings)
		{
			Drawings = VariantUtils.ConvertTo<NMapDrawings>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._mapContainer)
		{
			_mapContainer = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._pathsContainer)
		{
			_pathsContainer = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._points)
		{
			_points = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._bossPointNode)
		{
			_bossPointNode = VariantUtils.ConvertTo<NBossMapPoint>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._secondBossPointNode)
		{
			_secondBossPointNode = VariantUtils.ConvertTo<NBossMapPoint>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._startingPointNode)
		{
			_startingPointNode = VariantUtils.ConvertTo<NMapPoint>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._mapBgContainer)
		{
			_mapBgContainer = VariantUtils.ConvertTo<NMapBg>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._marker)
		{
			_marker = VariantUtils.ConvertTo<NMapMarker>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._backButton)
		{
			_backButton = VariantUtils.ConvertTo<NBackButton>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._drawingToolsHotkeyIcon)
		{
			_drawingToolsHotkeyIcon = VariantUtils.ConvertTo<TextureRect>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._drawingTools)
		{
			_drawingTools = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._mapDrawingButton)
		{
			_mapDrawingButton = VariantUtils.ConvertTo<NMapDrawButton>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._mapErasingButton)
		{
			_mapErasingButton = VariantUtils.ConvertTo<NMapEraseButton>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._mapClearButton)
		{
			_mapClearButton = VariantUtils.ConvertTo<NMapClearButton>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._mapLegend)
		{
			_mapLegend = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._legendItems)
		{
			_legendItems = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._legendHotkeyIcon)
		{
			_legendHotkeyIcon = VariantUtils.ConvertTo<TextureRect>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._backstop)
		{
			_backstop = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._tween)
		{
			_tween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._startDragPos)
		{
			_startDragPos = VariantUtils.ConvertTo<Vector2>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._targetDragPos)
		{
			_targetDragPos = VariantUtils.ConvertTo<Vector2>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._isDragging)
		{
			_isDragging = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._hasPlayedAnimation)
		{
			_hasPlayedAnimation = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._controllerScrollAmount)
		{
			_controllerScrollAmount = VariantUtils.ConvertTo<float>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._distX)
		{
			_distX = VariantUtils.ConvertTo<float>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._distY)
		{
			_distY = VariantUtils.ConvertTo<float>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._actAnimTween)
		{
			_actAnimTween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._mapScrollAnimTimer)
		{
			_mapScrollAnimTimer = VariantUtils.ConvertTo<float>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._canInterruptAnim)
		{
			_canInterruptAnim = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._isInputDisabled)
		{
			_isInputDisabled = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._promptTween)
		{
			_promptTween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._drawingInput)
		{
			_drawingInput = VariantUtils.ConvertTo<NMapDrawingInput>(ref value);
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
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0269: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0289: Unknown result type (might be due to invalid IL or missing references)
		//IL_028e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0309: Unknown result type (might be due to invalid IL or missing references)
		//IL_030e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0329: Unknown result type (might be due to invalid IL or missing references)
		//IL_032e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0349: Unknown result type (might be due to invalid IL or missing references)
		//IL_034e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0369: Unknown result type (might be due to invalid IL or missing references)
		//IL_036e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0389: Unknown result type (might be due to invalid IL or missing references)
		//IL_038e: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0409: Unknown result type (might be due to invalid IL or missing references)
		//IL_040e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0429: Unknown result type (might be due to invalid IL or missing references)
		//IL_042e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0449: Unknown result type (might be due to invalid IL or missing references)
		//IL_044e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0469: Unknown result type (might be due to invalid IL or missing references)
		//IL_046e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0489: Unknown result type (might be due to invalid IL or missing references)
		//IL_048e: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0509: Unknown result type (might be due to invalid IL or missing references)
		//IL_050e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0529: Unknown result type (might be due to invalid IL or missing references)
		//IL_052e: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName.IsOpen)
		{
			bool isOpen = IsOpen;
			value = VariantUtils.CreateFrom<bool>(ref isOpen);
			return true;
		}
		if ((ref name) == PropertyName.IsTravelEnabled)
		{
			bool isOpen = IsTravelEnabled;
			value = VariantUtils.CreateFrom<bool>(ref isOpen);
			return true;
		}
		if ((ref name) == PropertyName.IsDebugTravelEnabled)
		{
			bool isOpen = IsDebugTravelEnabled;
			value = VariantUtils.CreateFrom<bool>(ref isOpen);
			return true;
		}
		if ((ref name) == PropertyName.MapLegendX)
		{
			float mapLegendX = MapLegendX;
			value = VariantUtils.CreateFrom<float>(ref mapLegendX);
			return true;
		}
		if ((ref name) == PropertyName.IsTraveling)
		{
			bool isOpen = IsTraveling;
			value = VariantUtils.CreateFrom<bool>(ref isOpen);
			return true;
		}
		if ((ref name) == PropertyName.Drawings)
		{
			NMapDrawings drawings = Drawings;
			value = VariantUtils.CreateFrom<NMapDrawings>(ref drawings);
			return true;
		}
		if ((ref name) == PropertyName.DefaultFocusedControl)
		{
			Control defaultFocusedControl = DefaultFocusedControl;
			value = VariantUtils.CreateFrom<Control>(ref defaultFocusedControl);
			return true;
		}
		if ((ref name) == PropertyName._mapContainer)
		{
			value = VariantUtils.CreateFrom<Control>(ref _mapContainer);
			return true;
		}
		if ((ref name) == PropertyName._pathsContainer)
		{
			value = VariantUtils.CreateFrom<Control>(ref _pathsContainer);
			return true;
		}
		if ((ref name) == PropertyName._points)
		{
			value = VariantUtils.CreateFrom<Control>(ref _points);
			return true;
		}
		if ((ref name) == PropertyName._bossPointNode)
		{
			value = VariantUtils.CreateFrom<NBossMapPoint>(ref _bossPointNode);
			return true;
		}
		if ((ref name) == PropertyName._secondBossPointNode)
		{
			value = VariantUtils.CreateFrom<NBossMapPoint>(ref _secondBossPointNode);
			return true;
		}
		if ((ref name) == PropertyName._startingPointNode)
		{
			value = VariantUtils.CreateFrom<NMapPoint>(ref _startingPointNode);
			return true;
		}
		if ((ref name) == PropertyName._mapBgContainer)
		{
			value = VariantUtils.CreateFrom<NMapBg>(ref _mapBgContainer);
			return true;
		}
		if ((ref name) == PropertyName._marker)
		{
			value = VariantUtils.CreateFrom<NMapMarker>(ref _marker);
			return true;
		}
		if ((ref name) == PropertyName._backButton)
		{
			value = VariantUtils.CreateFrom<NBackButton>(ref _backButton);
			return true;
		}
		if ((ref name) == PropertyName._drawingToolsHotkeyIcon)
		{
			value = VariantUtils.CreateFrom<TextureRect>(ref _drawingToolsHotkeyIcon);
			return true;
		}
		if ((ref name) == PropertyName._drawingTools)
		{
			value = VariantUtils.CreateFrom<Control>(ref _drawingTools);
			return true;
		}
		if ((ref name) == PropertyName._mapDrawingButton)
		{
			value = VariantUtils.CreateFrom<NMapDrawButton>(ref _mapDrawingButton);
			return true;
		}
		if ((ref name) == PropertyName._mapErasingButton)
		{
			value = VariantUtils.CreateFrom<NMapEraseButton>(ref _mapErasingButton);
			return true;
		}
		if ((ref name) == PropertyName._mapClearButton)
		{
			value = VariantUtils.CreateFrom<NMapClearButton>(ref _mapClearButton);
			return true;
		}
		if ((ref name) == PropertyName._mapLegend)
		{
			value = VariantUtils.CreateFrom<Control>(ref _mapLegend);
			return true;
		}
		if ((ref name) == PropertyName._legendItems)
		{
			value = VariantUtils.CreateFrom<Control>(ref _legendItems);
			return true;
		}
		if ((ref name) == PropertyName._legendHotkeyIcon)
		{
			value = VariantUtils.CreateFrom<TextureRect>(ref _legendHotkeyIcon);
			return true;
		}
		if ((ref name) == PropertyName._backstop)
		{
			value = VariantUtils.CreateFrom<Control>(ref _backstop);
			return true;
		}
		if ((ref name) == PropertyName._tween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _tween);
			return true;
		}
		if ((ref name) == PropertyName._startDragPos)
		{
			value = VariantUtils.CreateFrom<Vector2>(ref _startDragPos);
			return true;
		}
		if ((ref name) == PropertyName._targetDragPos)
		{
			value = VariantUtils.CreateFrom<Vector2>(ref _targetDragPos);
			return true;
		}
		if ((ref name) == PropertyName._isDragging)
		{
			value = VariantUtils.CreateFrom<bool>(ref _isDragging);
			return true;
		}
		if ((ref name) == PropertyName._hasPlayedAnimation)
		{
			value = VariantUtils.CreateFrom<bool>(ref _hasPlayedAnimation);
			return true;
		}
		if ((ref name) == PropertyName._controllerScrollAmount)
		{
			value = VariantUtils.CreateFrom<float>(ref _controllerScrollAmount);
			return true;
		}
		if ((ref name) == PropertyName._distX)
		{
			value = VariantUtils.CreateFrom<float>(ref _distX);
			return true;
		}
		if ((ref name) == PropertyName._distY)
		{
			value = VariantUtils.CreateFrom<float>(ref _distY);
			return true;
		}
		if ((ref name) == PropertyName._actAnimTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _actAnimTween);
			return true;
		}
		if ((ref name) == PropertyName._mapScrollAnimTimer)
		{
			value = VariantUtils.CreateFrom<float>(ref _mapScrollAnimTimer);
			return true;
		}
		if ((ref name) == PropertyName._mapAnimStartDelay)
		{
			value = VariantUtils.CreateFrom<double>(ref _mapAnimStartDelay);
			return true;
		}
		if ((ref name) == PropertyName._mapAnimDuration)
		{
			value = VariantUtils.CreateFrom<double>(ref _mapAnimDuration);
			return true;
		}
		if ((ref name) == PropertyName._canInterruptAnim)
		{
			value = VariantUtils.CreateFrom<bool>(ref _canInterruptAnim);
			return true;
		}
		if ((ref name) == PropertyName._isInputDisabled)
		{
			value = VariantUtils.CreateFrom<bool>(ref _isInputDisabled);
			return true;
		}
		if ((ref name) == PropertyName._promptTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _promptTween);
			return true;
		}
		if ((ref name) == PropertyName._drawingInput)
		{
			value = VariantUtils.CreateFrom<NMapDrawingInput>(ref _drawingInput);
			return true;
		}
		return ((GodotObject)this).GetGodotClassPropertyValue(ref name, ref value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Unknown result type (might be due to invalid IL or missing references)
		//IL_028d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_030f: Unknown result type (might be due to invalid IL or missing references)
		//IL_032f: Unknown result type (might be due to invalid IL or missing references)
		//IL_034f: Unknown result type (might be due to invalid IL or missing references)
		//IL_036f: Unknown result type (might be due to invalid IL or missing references)
		//IL_038f: Unknown result type (might be due to invalid IL or missing references)
		//IL_03af: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0410: Unknown result type (might be due to invalid IL or missing references)
		//IL_0430: Unknown result type (might be due to invalid IL or missing references)
		//IL_0450: Unknown result type (might be due to invalid IL or missing references)
		//IL_0470: Unknown result type (might be due to invalid IL or missing references)
		//IL_0490: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0513: Unknown result type (might be due to invalid IL or missing references)
		//IL_0534: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)1, PropertyName.IsOpen, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName.IsTravelEnabled, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName.IsDebugTravelEnabled, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._mapContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._pathsContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._points, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._bossPointNode, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._secondBossPointNode, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._startingPointNode, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._mapBgContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._marker, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._backButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._drawingToolsHotkeyIcon, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._drawingTools, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._mapDrawingButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._mapErasingButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._mapClearButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._mapLegend, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._legendItems, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._legendHotkeyIcon, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._backstop, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._tween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)5, PropertyName._startDragPos, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)5, PropertyName._targetDragPos, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._isDragging, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._hasPlayedAnimation, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)3, PropertyName.MapLegendX, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName.IsTraveling, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)3, PropertyName._controllerScrollAmount, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)3, PropertyName._distX, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)3, PropertyName._distY, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._actAnimTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)3, PropertyName._mapScrollAnimTimer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)3, PropertyName._mapAnimStartDelay, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)3, PropertyName._mapAnimDuration, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._canInterruptAnim, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._isInputDisabled, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._promptTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.Drawings, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._drawingInput, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.DefaultFocusedControl, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_028a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_030e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0324: Unknown result type (might be due to invalid IL or missing references)
		//IL_033a: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		StringName isOpen = PropertyName.IsOpen;
		bool isOpen2 = IsOpen;
		info.AddProperty(isOpen, Variant.From<bool>(ref isOpen2));
		StringName isTravelEnabled = PropertyName.IsTravelEnabled;
		isOpen2 = IsTravelEnabled;
		info.AddProperty(isTravelEnabled, Variant.From<bool>(ref isOpen2));
		StringName isDebugTravelEnabled = PropertyName.IsDebugTravelEnabled;
		isOpen2 = IsDebugTravelEnabled;
		info.AddProperty(isDebugTravelEnabled, Variant.From<bool>(ref isOpen2));
		StringName isTraveling = PropertyName.IsTraveling;
		isOpen2 = IsTraveling;
		info.AddProperty(isTraveling, Variant.From<bool>(ref isOpen2));
		StringName drawings = PropertyName.Drawings;
		NMapDrawings drawings2 = Drawings;
		info.AddProperty(drawings, Variant.From<NMapDrawings>(ref drawings2));
		info.AddProperty(PropertyName._mapContainer, Variant.From<Control>(ref _mapContainer));
		info.AddProperty(PropertyName._pathsContainer, Variant.From<Control>(ref _pathsContainer));
		info.AddProperty(PropertyName._points, Variant.From<Control>(ref _points));
		info.AddProperty(PropertyName._bossPointNode, Variant.From<NBossMapPoint>(ref _bossPointNode));
		info.AddProperty(PropertyName._secondBossPointNode, Variant.From<NBossMapPoint>(ref _secondBossPointNode));
		info.AddProperty(PropertyName._startingPointNode, Variant.From<NMapPoint>(ref _startingPointNode));
		info.AddProperty(PropertyName._mapBgContainer, Variant.From<NMapBg>(ref _mapBgContainer));
		info.AddProperty(PropertyName._marker, Variant.From<NMapMarker>(ref _marker));
		info.AddProperty(PropertyName._backButton, Variant.From<NBackButton>(ref _backButton));
		info.AddProperty(PropertyName._drawingToolsHotkeyIcon, Variant.From<TextureRect>(ref _drawingToolsHotkeyIcon));
		info.AddProperty(PropertyName._drawingTools, Variant.From<Control>(ref _drawingTools));
		info.AddProperty(PropertyName._mapDrawingButton, Variant.From<NMapDrawButton>(ref _mapDrawingButton));
		info.AddProperty(PropertyName._mapErasingButton, Variant.From<NMapEraseButton>(ref _mapErasingButton));
		info.AddProperty(PropertyName._mapClearButton, Variant.From<NMapClearButton>(ref _mapClearButton));
		info.AddProperty(PropertyName._mapLegend, Variant.From<Control>(ref _mapLegend));
		info.AddProperty(PropertyName._legendItems, Variant.From<Control>(ref _legendItems));
		info.AddProperty(PropertyName._legendHotkeyIcon, Variant.From<TextureRect>(ref _legendHotkeyIcon));
		info.AddProperty(PropertyName._backstop, Variant.From<Control>(ref _backstop));
		info.AddProperty(PropertyName._tween, Variant.From<Tween>(ref _tween));
		info.AddProperty(PropertyName._startDragPos, Variant.From<Vector2>(ref _startDragPos));
		info.AddProperty(PropertyName._targetDragPos, Variant.From<Vector2>(ref _targetDragPos));
		info.AddProperty(PropertyName._isDragging, Variant.From<bool>(ref _isDragging));
		info.AddProperty(PropertyName._hasPlayedAnimation, Variant.From<bool>(ref _hasPlayedAnimation));
		info.AddProperty(PropertyName._controllerScrollAmount, Variant.From<float>(ref _controllerScrollAmount));
		info.AddProperty(PropertyName._distX, Variant.From<float>(ref _distX));
		info.AddProperty(PropertyName._distY, Variant.From<float>(ref _distY));
		info.AddProperty(PropertyName._actAnimTween, Variant.From<Tween>(ref _actAnimTween));
		info.AddProperty(PropertyName._mapScrollAnimTimer, Variant.From<float>(ref _mapScrollAnimTimer));
		info.AddProperty(PropertyName._canInterruptAnim, Variant.From<bool>(ref _canInterruptAnim));
		info.AddProperty(PropertyName._isInputDisabled, Variant.From<bool>(ref _isInputDisabled));
		info.AddProperty(PropertyName._promptTween, Variant.From<Tween>(ref _promptTween));
		info.AddProperty(PropertyName._drawingInput, Variant.From<NMapDrawingInput>(ref _drawingInput));
		info.AddSignalEventDelegate(SignalName.Opened, (Delegate)backing_Opened);
		info.AddSignalEventDelegate(SignalName.Closed, (Delegate)backing_Closed);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02be: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02da: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName.IsOpen, ref val))
		{
			IsOpen = ((Variant)(ref val)).As<bool>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName.IsTravelEnabled, ref val2))
		{
			IsTravelEnabled = ((Variant)(ref val2)).As<bool>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName.IsDebugTravelEnabled, ref val3))
		{
			IsDebugTravelEnabled = ((Variant)(ref val3)).As<bool>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName.IsTraveling, ref val4))
		{
			IsTraveling = ((Variant)(ref val4)).As<bool>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName.Drawings, ref val5))
		{
			Drawings = ((Variant)(ref val5)).As<NMapDrawings>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._mapContainer, ref val6))
		{
			_mapContainer = ((Variant)(ref val6)).As<Control>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName._pathsContainer, ref val7))
		{
			_pathsContainer = ((Variant)(ref val7)).As<Control>();
		}
		Variant val8 = default(Variant);
		if (info.TryGetProperty(PropertyName._points, ref val8))
		{
			_points = ((Variant)(ref val8)).As<Control>();
		}
		Variant val9 = default(Variant);
		if (info.TryGetProperty(PropertyName._bossPointNode, ref val9))
		{
			_bossPointNode = ((Variant)(ref val9)).As<NBossMapPoint>();
		}
		Variant val10 = default(Variant);
		if (info.TryGetProperty(PropertyName._secondBossPointNode, ref val10))
		{
			_secondBossPointNode = ((Variant)(ref val10)).As<NBossMapPoint>();
		}
		Variant val11 = default(Variant);
		if (info.TryGetProperty(PropertyName._startingPointNode, ref val11))
		{
			_startingPointNode = ((Variant)(ref val11)).As<NMapPoint>();
		}
		Variant val12 = default(Variant);
		if (info.TryGetProperty(PropertyName._mapBgContainer, ref val12))
		{
			_mapBgContainer = ((Variant)(ref val12)).As<NMapBg>();
		}
		Variant val13 = default(Variant);
		if (info.TryGetProperty(PropertyName._marker, ref val13))
		{
			_marker = ((Variant)(ref val13)).As<NMapMarker>();
		}
		Variant val14 = default(Variant);
		if (info.TryGetProperty(PropertyName._backButton, ref val14))
		{
			_backButton = ((Variant)(ref val14)).As<NBackButton>();
		}
		Variant val15 = default(Variant);
		if (info.TryGetProperty(PropertyName._drawingToolsHotkeyIcon, ref val15))
		{
			_drawingToolsHotkeyIcon = ((Variant)(ref val15)).As<TextureRect>();
		}
		Variant val16 = default(Variant);
		if (info.TryGetProperty(PropertyName._drawingTools, ref val16))
		{
			_drawingTools = ((Variant)(ref val16)).As<Control>();
		}
		Variant val17 = default(Variant);
		if (info.TryGetProperty(PropertyName._mapDrawingButton, ref val17))
		{
			_mapDrawingButton = ((Variant)(ref val17)).As<NMapDrawButton>();
		}
		Variant val18 = default(Variant);
		if (info.TryGetProperty(PropertyName._mapErasingButton, ref val18))
		{
			_mapErasingButton = ((Variant)(ref val18)).As<NMapEraseButton>();
		}
		Variant val19 = default(Variant);
		if (info.TryGetProperty(PropertyName._mapClearButton, ref val19))
		{
			_mapClearButton = ((Variant)(ref val19)).As<NMapClearButton>();
		}
		Variant val20 = default(Variant);
		if (info.TryGetProperty(PropertyName._mapLegend, ref val20))
		{
			_mapLegend = ((Variant)(ref val20)).As<Control>();
		}
		Variant val21 = default(Variant);
		if (info.TryGetProperty(PropertyName._legendItems, ref val21))
		{
			_legendItems = ((Variant)(ref val21)).As<Control>();
		}
		Variant val22 = default(Variant);
		if (info.TryGetProperty(PropertyName._legendHotkeyIcon, ref val22))
		{
			_legendHotkeyIcon = ((Variant)(ref val22)).As<TextureRect>();
		}
		Variant val23 = default(Variant);
		if (info.TryGetProperty(PropertyName._backstop, ref val23))
		{
			_backstop = ((Variant)(ref val23)).As<Control>();
		}
		Variant val24 = default(Variant);
		if (info.TryGetProperty(PropertyName._tween, ref val24))
		{
			_tween = ((Variant)(ref val24)).As<Tween>();
		}
		Variant val25 = default(Variant);
		if (info.TryGetProperty(PropertyName._startDragPos, ref val25))
		{
			_startDragPos = ((Variant)(ref val25)).As<Vector2>();
		}
		Variant val26 = default(Variant);
		if (info.TryGetProperty(PropertyName._targetDragPos, ref val26))
		{
			_targetDragPos = ((Variant)(ref val26)).As<Vector2>();
		}
		Variant val27 = default(Variant);
		if (info.TryGetProperty(PropertyName._isDragging, ref val27))
		{
			_isDragging = ((Variant)(ref val27)).As<bool>();
		}
		Variant val28 = default(Variant);
		if (info.TryGetProperty(PropertyName._hasPlayedAnimation, ref val28))
		{
			_hasPlayedAnimation = ((Variant)(ref val28)).As<bool>();
		}
		Variant val29 = default(Variant);
		if (info.TryGetProperty(PropertyName._controllerScrollAmount, ref val29))
		{
			_controllerScrollAmount = ((Variant)(ref val29)).As<float>();
		}
		Variant val30 = default(Variant);
		if (info.TryGetProperty(PropertyName._distX, ref val30))
		{
			_distX = ((Variant)(ref val30)).As<float>();
		}
		Variant val31 = default(Variant);
		if (info.TryGetProperty(PropertyName._distY, ref val31))
		{
			_distY = ((Variant)(ref val31)).As<float>();
		}
		Variant val32 = default(Variant);
		if (info.TryGetProperty(PropertyName._actAnimTween, ref val32))
		{
			_actAnimTween = ((Variant)(ref val32)).As<Tween>();
		}
		Variant val33 = default(Variant);
		if (info.TryGetProperty(PropertyName._mapScrollAnimTimer, ref val33))
		{
			_mapScrollAnimTimer = ((Variant)(ref val33)).As<float>();
		}
		Variant val34 = default(Variant);
		if (info.TryGetProperty(PropertyName._canInterruptAnim, ref val34))
		{
			_canInterruptAnim = ((Variant)(ref val34)).As<bool>();
		}
		Variant val35 = default(Variant);
		if (info.TryGetProperty(PropertyName._isInputDisabled, ref val35))
		{
			_isInputDisabled = ((Variant)(ref val35)).As<bool>();
		}
		Variant val36 = default(Variant);
		if (info.TryGetProperty(PropertyName._promptTween, ref val36))
		{
			_promptTween = ((Variant)(ref val36)).As<Tween>();
		}
		Variant val37 = default(Variant);
		if (info.TryGetProperty(PropertyName._drawingInput, ref val37))
		{
			_drawingInput = ((Variant)(ref val37)).As<NMapDrawingInput>();
		}
		OpenedEventHandler openedEventHandler = default(OpenedEventHandler);
		if (info.TryGetSignalEventDelegate<OpenedEventHandler>(SignalName.Opened, ref openedEventHandler))
		{
			backing_Opened = openedEventHandler;
		}
		ClosedEventHandler closedEventHandler = default(ClosedEventHandler);
		if (info.TryGetSignalEventDelegate<ClosedEventHandler>(SignalName.Closed, ref closedEventHandler))
		{
			backing_Closed = closedEventHandler;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotSignalList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(2);
		list.Add(new MethodInfo(SignalName.Opened, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(SignalName.Closed, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	protected void EmitSignalOpened()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).EmitSignal(SignalName.Opened, Array.Empty<Variant>());
	}

	protected void EmitSignalClosed()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).EmitSignal(SignalName.Closed, Array.Empty<Variant>());
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RaiseGodotClassSignalCallbacks(in godot_string_name signal, NativeVariantPtrArgs args)
	{
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		if ((ref signal) == SignalName.Opened && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			backing_Opened?.Invoke();
		}
		else if ((ref signal) == SignalName.Closed && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			backing_Closed?.Invoke();
		}
		else
		{
			((GodotObject)this).RaiseGodotClassSignalCallbacks(ref signal, args);
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassSignal(in godot_string_name signal)
	{
		if ((ref signal) == SignalName.Opened)
		{
			return true;
		}
		if ((ref signal) == SignalName.Closed)
		{
			return true;
		}
		return ((Control)this).HasGodotClassSignal(ref signal);
	}
}
