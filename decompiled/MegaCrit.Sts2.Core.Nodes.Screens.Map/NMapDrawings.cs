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
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Multiplayer.Game;
using MegaCrit.Sts2.Core.Multiplayer.Game.PeerInput;
using MegaCrit.Sts2.Core.Multiplayer.Messages.Game.Flavor;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves.MapDrawing;

namespace MegaCrit.Sts2.Core.Nodes.Screens.Map;

[ScriptPath("res://src/Core/Nodes/Screens/Map/NMapDrawings.cs")]
public class NMapDrawings : Control
{
	private class DrawingState
	{
		public DrawingMode? overrideDrawingMode;

		public DrawingMode drawingMode;

		public ulong playerId;

		public Line2D? currentlyDrawingLine;

		public required SubViewport drawViewport;

		public bool IsDrawing => currentlyDrawingLine != null;

		public DrawingMode CurrentDrawingMode => overrideDrawingMode ?? drawingMode;
	}

	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName _ExitTree = StringName.op_Implicit("_ExitTree");

		public static readonly StringName UpdateCurrentLinePositionLocal = StringName.op_Implicit("UpdateCurrentLinePositionLocal");

		public static readonly StringName StopLineLocal = StringName.op_Implicit("StopLineLocal");

		public static readonly StringName SetDrawingModeLocal = StringName.op_Implicit("SetDrawingModeLocal");

		public static readonly StringName ClearDrawnLinesLocal = StringName.op_Implicit("ClearDrawnLinesLocal");

		public static readonly StringName IsDrawing = StringName.op_Implicit("IsDrawing");

		public static readonly StringName IsLocalDrawing = StringName.op_Implicit("IsLocalDrawing");

		public static readonly StringName GetDrawingMode = StringName.op_Implicit("GetDrawingMode");

		public static readonly StringName GetLocalDrawingMode = StringName.op_Implicit("GetLocalDrawingMode");

		public static readonly StringName ToNetPosition = StringName.op_Implicit("ToNetPosition");

		public static readonly StringName FromNetPosition = StringName.op_Implicit("FromNetPosition");

		public static readonly StringName ClearAllLines = StringName.op_Implicit("ClearAllLines");

		public static readonly StringName OnPlayerScreenChanged = StringName.op_Implicit("OnPlayerScreenChanged");

		public static readonly StringName TrySendSyncMessage = StringName.op_Implicit("TrySendSyncMessage");

		public static readonly StringName SendSyncMessage = StringName.op_Implicit("SendSyncMessage");

		public static readonly StringName UpdateLocalCursor = StringName.op_Implicit("UpdateLocalCursor");

		public static readonly StringName RepositionBasedOnBackground = StringName.op_Implicit("RepositionBasedOnBackground");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName _lineDrawScene = StringName.op_Implicit("_lineDrawScene");

		public static readonly StringName _lineEraseScene = StringName.op_Implicit("_lineEraseScene");

		public static readonly StringName _cursorManager = StringName.op_Implicit("_cursorManager");

		public static readonly StringName _eraserMaterial = StringName.op_Implicit("_eraserMaterial");

		public static readonly StringName _defaultSize = StringName.op_Implicit("_defaultSize");

		public static readonly StringName _lastMessageMsec = StringName.op_Implicit("_lastMessageMsec");
	}

	public class SignalName : SignalName
	{
	}

	private const int _minUpdateMsec = 50;

	private static readonly string _lineDrawScenePath = SceneHelper.GetScenePath("screens/map/map_line_draw");

	private static readonly string _lineEraseScenePath = SceneHelper.GetScenePath("screens/map/map_line_erase");

	private static readonly string _playerDrawingPath = SceneHelper.GetScenePath("screens/map/map_drawing");

	public const string drawingCursorPath = "res://images/packed/common_ui/cursor_quill.png";

	public const string drawingCursorTiltedPath = "res://images/packed/common_ui/cursor_quill_tilted.png";

	public static readonly Vector2 drawingCursorHotspot = new Vector2(2f, 56f);

	public const string erasingCursorPath = "res://images/packed/common_ui/cursor_eraser.png";

	public const string erasingCursorTiltedPath = "res://images/packed/common_ui/cursor_eraser_tilted.png";

	public static readonly Vector2 erasingCursorHotspot = new Vector2(24f, 58f);

	private const float _minimumPointDistance = 2f;

	private INetGameService _netService;

	private IPlayerCollection _playerCollection;

	private PeerInputSynchronizer _inputSynchronizer;

	private PackedScene _lineDrawScene;

	private PackedScene _lineEraseScene;

	private NCursorManager _cursorManager;

	private Material _eraserMaterial;

	private Vector2 _defaultSize;

	private readonly List<DrawingState> _drawingStates = new List<DrawingState>();

	private MapDrawingMessage? _queuedMessage;

	private ulong _lastMessageMsec;

	private Task? _sendMessageTask;

	private static IEnumerable<string> SelfAssetPaths => new global::_003C_003Ez__ReadOnlyArray<string>(new string[7] { _lineDrawScenePath, _lineEraseScenePath, _playerDrawingPath, "res://images/packed/common_ui/cursor_quill.png", "res://images/packed/common_ui/cursor_quill_tilted.png", "res://images/packed/common_ui/cursor_eraser.png", "res://images/packed/common_ui/cursor_eraser_tilted.png" });

	public static IEnumerable<string> AssetPaths => SelfAssetPaths.Concat(NMapDrawButton.AssetPaths);

	public override void _Ready()
	{
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		_lineDrawScene = PreloadManager.Cache.GetScene(_lineDrawScenePath);
		_lineEraseScene = PreloadManager.Cache.GetScene(_lineEraseScenePath);
		_cursorManager = NGame.Instance.CursorManager;
		Line2D val = _lineEraseScene.Instantiate<Line2D>((GenEditState)0);
		_eraserMaterial = ((CanvasItem)val).Material;
		((Node)(object)val).QueueFreeSafely();
		_defaultSize = ((Control)this).Size;
	}

	public void Initialize(INetGameService netService, IPlayerCollection playerCollection, PeerInputSynchronizer inputSynchronizer)
	{
		_netService = netService;
		_playerCollection = playerCollection;
		_inputSynchronizer = inputSynchronizer;
		_netService.RegisterMessageHandler<MapDrawingMessage>(HandleDrawingMessage);
		_netService.RegisterMessageHandler<ClearMapDrawingsMessage>(HandleClearMapDrawingsMessage);
		_netService.RegisterMessageHandler<MapDrawingModeChangedMessage>(HandleMapDrawingModeChangedMessage);
		inputSynchronizer.ScreenChanged += OnPlayerScreenChanged;
	}

	public override void _ExitTree()
	{
		_netService.UnregisterMessageHandler<MapDrawingMessage>(HandleDrawingMessage);
		_netService.UnregisterMessageHandler<ClearMapDrawingsMessage>(HandleClearMapDrawingsMessage);
		_netService.UnregisterMessageHandler<MapDrawingModeChangedMessage>(HandleMapDrawingModeChangedMessage);
		_inputSynchronizer.ScreenChanged -= OnPlayerScreenChanged;
	}

	public void BeginLineLocal(Vector2 position, DrawingMode? overrideDrawingMode)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		BeginLine(GetDrawingStateForPlayer(_netService.NetId), position, overrideDrawingMode);
		NetMapDrawingEvent ev = new NetMapDrawingEvent
		{
			type = MapDrawingEventType.BeginLine,
			position = ToNetPosition(position),
			overrideDrawingMode = overrideDrawingMode
		};
		QueueOrSendEvent(ev);
	}

	public void UpdateCurrentLinePositionLocal(Vector2 position)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		DrawingState drawingStateForPlayer = GetDrawingStateForPlayer(_netService.NetId);
		UpdateCurrentLinePosition(drawingStateForPlayer, position);
		NetMapDrawingEvent ev = new NetMapDrawingEvent
		{
			type = MapDrawingEventType.ContinueLine,
			position = ToNetPosition(position),
			overrideDrawingMode = drawingStateForPlayer.overrideDrawingMode
		};
		QueueOrSendEvent(ev);
	}

	public void StopLineLocal()
	{
		StopDrawingLine(GetDrawingStateForPlayer(_netService.NetId));
		NetMapDrawingEvent ev = new NetMapDrawingEvent
		{
			type = MapDrawingEventType.EndLine
		};
		QueueOrSendEvent(ev);
	}

	public void SetDrawingModeLocal(DrawingMode drawingMode)
	{
		SetDrawingMode(GetDrawingStateForPlayer(_netService.NetId), drawingMode);
		MapDrawingModeChangedMessage message = new MapDrawingModeChangedMessage
		{
			drawingMode = drawingMode
		};
		_netService.SendMessage(message);
		UpdateLocalCursor();
	}

	public void ClearDrawnLinesLocal()
	{
		ClearAllLinesForPlayer(GetDrawingStateForPlayer(_netService.NetId));
		UpdateLocalCursor();
		_netService.SendMessage(default(ClearMapDrawingsMessage));
	}

	public bool IsDrawing(ulong playerId)
	{
		return GetDrawingStateForPlayer(playerId).IsDrawing;
	}

	public bool IsLocalDrawing()
	{
		return GetDrawingStateForPlayer(_netService.NetId).IsDrawing;
	}

	public DrawingMode GetDrawingMode(ulong playerId)
	{
		return GetDrawingStateForPlayer(playerId).CurrentDrawingMode;
	}

	public DrawingMode GetLocalDrawingMode(bool useOverride = true)
	{
		if (!useOverride)
		{
			return GetDrawingStateForPlayer(_netService.NetId).drawingMode;
		}
		return GetDrawingStateForPlayer(_netService.NetId).CurrentDrawingMode;
	}

	private void QueueOrSendEvent(NetMapDrawingEvent ev)
	{
		if (_queuedMessage == null)
		{
			_queuedMessage = new MapDrawingMessage();
		}
		if (!_queuedMessage.TryAddEvent(ev))
		{
			_queuedMessage.drawingMode = GetDrawingStateForPlayer(_netService.NetId).drawingMode;
			_netService.SendMessage(_queuedMessage);
			_queuedMessage = new MapDrawingMessage();
			if (!_queuedMessage.TryAddEvent(ev))
			{
				throw new InvalidOperationException();
			}
		}
		TrySendSyncMessage();
		UpdateLocalCursor();
	}

	private Vector2 ToNetPosition(Vector2 pos)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		pos.X -= ((Control)this).Size.X * 0.5f;
		pos /= new Vector2(960f, ((Control)this).Size.Y);
		return pos;
	}

	private Vector2 FromNetPosition(Vector2 pos)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		pos *= new Vector2(960f, ((Control)this).Size.Y);
		pos.X += ((Control)this).Size.X * 0.5f;
		return pos;
	}

	private void HandleDrawingMessage(MapDrawingMessage message, ulong senderId)
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		DrawingState drawingStateForPlayer = GetDrawingStateForPlayer(senderId);
		foreach (NetMapDrawingEvent @event in message.Events)
		{
			if (@event.type == MapDrawingEventType.BeginLine)
			{
				if (GetDrawingMode(senderId) != DrawingMode.None)
				{
					StopDrawingLine(drawingStateForPlayer);
				}
				BeginLine(drawingStateForPlayer, FromNetPosition(@event.position), @event.overrideDrawingMode);
			}
			else if (@event.type == MapDrawingEventType.ContinueLine)
			{
				if (!drawingStateForPlayer.IsDrawing)
				{
					if (message.drawingMode.HasValue && drawingStateForPlayer.drawingMode != message.drawingMode)
					{
						SetDrawingMode(drawingStateForPlayer, message.drawingMode.Value);
					}
					BeginLine(drawingStateForPlayer, FromNetPosition(@event.position), @event.overrideDrawingMode);
				}
				UpdateCurrentLinePosition(drawingStateForPlayer, FromNetPosition(@event.position));
			}
			else
			{
				StopDrawingLine(drawingStateForPlayer);
			}
		}
	}

	private void HandleClearMapDrawingsMessage(ClearMapDrawingsMessage message, ulong senderId)
	{
		ClearAllLinesForPlayer(GetDrawingStateForPlayer(senderId));
	}

	private void HandleMapDrawingModeChangedMessage(MapDrawingModeChangedMessage message, ulong senderId)
	{
		SetDrawingMode(GetDrawingStateForPlayer(senderId), message.drawingMode);
	}

	private void BeginLine(DrawingState state, Vector2 position, DrawingMode? overrideDrawingMode)
	{
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		Player player = _playerCollection.GetPlayer(state.playerId);
		DrawingMode drawingMode = overrideDrawingMode ?? state.drawingMode;
		if (drawingMode == DrawingMode.None)
		{
			throw new InvalidOperationException($"Player {state.playerId} is not currently in a drawing mode and no override was passed!");
		}
		state.overrideDrawingMode = overrideDrawingMode;
		state.currentlyDrawingLine = CreateLineForPlayer(player, drawingMode == DrawingMode.Erasing);
		state.currentlyDrawingLine.AddPoint(position * 0.5f, -1);
		state.currentlyDrawingLine.AddPoint(position * 0.5f + new Vector2(0f, 0.5f), -1);
		((Node)(object)state.drawViewport).AddChildSafely((Node?)(object)state.currentlyDrawingLine);
		NGame.Instance.RemoteCursorContainer.DrawingCursorStateChanged(state.playerId);
	}

	private Line2D CreateLineForPlayer(Player player, bool isErasing)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		PackedScene val = (isErasing ? _lineEraseScene : _lineDrawScene);
		Line2D val2 = val.Instantiate<Line2D>((GenEditState)0);
		val2.DefaultColor = player.Character.MapDrawingColor;
		val2.ClearPoints();
		((Node2D)val2).Position = Vector2.Zero;
		return val2;
	}

	private void StopDrawingLine(DrawingState state)
	{
		state.overrideDrawingMode = null;
		state.currentlyDrawingLine = null;
		NGame.Instance.RemoteCursorContainer.DrawingCursorStateChanged(state.playerId);
	}

	private void SetDrawingMode(DrawingState state, DrawingMode drawingMode)
	{
		if (state.drawingMode != drawingMode)
		{
			state.drawingMode = drawingMode;
			NGame.Instance.RemoteCursorContainer.DrawingCursorStateChanged(state.playerId);
		}
	}

	private void UpdateCurrentLinePosition(DrawingState state, Vector2 position)
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		if (state.currentlyDrawingLine == null)
		{
			throw new InvalidOperationException($"Tried to update current line position for player {state.playerId}, but they are not currently drawing a line!");
		}
		Vector2 val = state.currentlyDrawingLine.Points[^1];
		if (!(((Vector2)(ref val)).DistanceSquaredTo(position) < 4f))
		{
			state.currentlyDrawingLine.AddPoint(position * 0.5f, -1);
		}
	}

	private DrawingState GetDrawingStateForPlayer(ulong playerId)
	{
		DrawingState drawingState = _drawingStates.FirstOrDefault((DrawingState s) => s.playerId == playerId);
		if (drawingState == null)
		{
			Control val = PreloadManager.Cache.GetScene(_playerDrawingPath).Instantiate<Control>((GenEditState)0);
			((Node)(object)this).AddChildSafely((Node?)(object)val);
			drawingState = new DrawingState
			{
				playerId = playerId,
				drawViewport = ((Node)val).GetNode<SubViewport>(NodePath.op_Implicit("DrawViewport"))
			};
			TaskHelper.RunSafely(SetVisibleLater(val));
			_drawingStates.Add(drawingState);
		}
		return drawingState;
	}

	private async Task SetVisibleLater(Control mapDrawingScene)
	{
		TextureRect drawingTexture = ((Node)mapDrawingScene).GetNode<TextureRect>(NodePath.op_Implicit("DrawViewportTextureRect"));
		SubViewport drawViewport = ((Node)mapDrawingScene).GetNode<SubViewport>(NodePath.op_Implicit("DrawViewport"));
		drawViewport.RenderTargetUpdateMode = (UpdateMode)4;
		((CanvasItem)drawingTexture).Visible = false;
		await ((GodotObject)this).ToSignal((GodotObject)(object)((Node)this).GetTree(), SignalName.ProcessFrame);
		await ((GodotObject)this).ToSignal((GodotObject)(object)((Node)this).GetTree(), SignalName.ProcessFrame);
		await ((GodotObject)this).ToSignal((GodotObject)(object)((Node)this).GetTree(), SignalName.ProcessFrame);
		((CanvasItem)drawingTexture).Visible = true;
		drawViewport.RenderTargetUpdateMode = (UpdateMode)2;
	}

	public void ClearAllLines()
	{
		foreach (DrawingState drawingState in _drawingStates)
		{
			foreach (Line2D item in ((IEnumerable)((Node)drawingState.drawViewport).GetChildren(false)).OfType<Line2D>())
			{
				((Node)(object)item).QueueFreeSafely();
			}
		}
	}

	public SerializableMapDrawings GetSerializableMapDrawings()
	{
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		SerializableMapDrawings serializableMapDrawings = new SerializableMapDrawings();
		foreach (DrawingState drawingState in _drawingStates)
		{
			SerializablePlayerMapDrawings serializablePlayerMapDrawings = new SerializablePlayerMapDrawings
			{
				playerId = drawingState.playerId
			};
			serializableMapDrawings.drawings.Add(serializablePlayerMapDrawings);
			foreach (Line2D item in ((IEnumerable)((Node)drawingState.drawViewport).GetChildren(false)).OfType<Line2D>())
			{
				SerializableMapDrawingLine serializableMapDrawingLine = new SerializableMapDrawingLine
				{
					mapPoints = new List<Vector2>()
				};
				serializableMapDrawingLine.isEraser = ((CanvasItem)item).Material == _eraserMaterial;
				serializablePlayerMapDrawings.lines.Add(serializableMapDrawingLine);
				Vector2[] points = item.Points;
				foreach (Vector2 pos in points)
				{
					serializableMapDrawingLine.mapPoints.Add(ToNetPosition(pos));
				}
			}
		}
		return serializableMapDrawings;
	}

	public void LoadDrawings(SerializableMapDrawings drawings)
	{
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		foreach (SerializablePlayerMapDrawings drawing in drawings.drawings)
		{
			Player player = _playerCollection.GetPlayer(drawing.playerId);
			if (player == null)
			{
				Log.Warn($"Player {drawing.playerId} has map drawings, but doesn't exist in the run!");
				continue;
			}
			DrawingState drawingStateForPlayer = GetDrawingStateForPlayer(drawing.playerId);
			foreach (SerializableMapDrawingLine line in drawing.lines)
			{
				Line2D val = CreateLineForPlayer(player, line.isEraser);
				((Node)(object)drawingStateForPlayer.drawViewport).AddChildSafely((Node?)(object)val);
				foreach (Vector2 mapPoint in line.mapPoints)
				{
					val.AddPoint(FromNetPosition(mapPoint), -1);
				}
			}
		}
	}

	private void ClearAllLinesForPlayer(DrawingState state)
	{
		foreach (Line2D item in ((IEnumerable)((Node)state.drawViewport).GetChildren(false)).OfType<Line2D>())
		{
			((Node)(object)item).QueueFreeSafely();
		}
		SetDrawingMode(state, DrawingMode.None);
	}

	private void OnPlayerScreenChanged(ulong playerId, NetScreenType oldScreenType)
	{
		if (playerId == _netService.NetId)
		{
			return;
		}
		NetScreenType screenType = _inputSynchronizer.GetScreenType(playerId);
		if (oldScreenType == NetScreenType.Map && screenType != NetScreenType.Map)
		{
			DrawingState drawingStateForPlayer = GetDrawingStateForPlayer(playerId);
			if (drawingStateForPlayer.IsDrawing)
			{
				StopDrawingLine(drawingStateForPlayer);
			}
			if (drawingStateForPlayer.drawingMode != DrawingMode.None)
			{
				SetDrawingMode(drawingStateForPlayer, DrawingMode.None);
			}
		}
	}

	private void TrySendSyncMessage()
	{
		if (_sendMessageTask == null && _netService.IsConnected)
		{
			int num = (int)(_lastMessageMsec + 50 - Time.GetTicksMsec());
			if (num <= 0)
			{
				_sendMessageTask = TaskHelper.RunSafely(SendSyncMessageAfterSmallDelay());
			}
			else
			{
				_sendMessageTask = TaskHelper.RunSafely(QueueSyncMessage(num));
			}
		}
	}

	private async Task QueueSyncMessage(int delayMsec)
	{
		await Task.Delay(delayMsec);
		SendSyncMessage();
	}

	private async Task SendSyncMessageAfterSmallDelay()
	{
		await Task.Yield();
		SendSyncMessage();
	}

	private void SendSyncMessage()
	{
		if (_netService.IsConnected)
		{
			_queuedMessage.drawingMode = GetDrawingStateForPlayer(_netService.NetId).drawingMode;
			_netService.SendMessage(_queuedMessage);
			_lastMessageMsec = Time.GetTicksMsec();
			_queuedMessage = null;
			_sendMessageTask = null;
		}
	}

	private void UpdateLocalCursor()
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		DrawingState drawingStateForPlayer = GetDrawingStateForPlayer(_netService.NetId);
		if (drawingStateForPlayer.CurrentDrawingMode == DrawingMode.Drawing)
		{
			Image asset = PreloadManager.Cache.GetAsset<Image>("res://images/packed/common_ui/cursor_quill.png");
			Image asset2 = PreloadManager.Cache.GetAsset<Image>("res://images/packed/common_ui/cursor_quill_tilted.png");
			_cursorManager.OverrideCursor(asset2, asset, drawingCursorHotspot);
		}
		else if (drawingStateForPlayer.CurrentDrawingMode == DrawingMode.Erasing)
		{
			Image asset3 = PreloadManager.Cache.GetAsset<Image>("res://images/packed/common_ui/cursor_eraser.png");
			Image asset4 = PreloadManager.Cache.GetAsset<Image>("res://images/packed/common_ui/cursor_eraser_tilted.png");
			_cursorManager.OverrideCursor(asset4, asset3, erasingCursorHotspot);
		}
		else
		{
			_cursorManager.StopOverridingCursor();
		}
	}

	public void RepositionBasedOnBackground(Control mapBg)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		((Control)this).Position = new Vector2(mapBg.Position.X + (mapBg.Size.X - ((Control)this).Size.X) * 0.5f, mapBg.Position.Y);
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
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		//IL_028d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0307: Unknown result type (might be due to invalid IL or missing references)
		//IL_032a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0335: Unknown result type (might be due to invalid IL or missing references)
		//IL_035b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0364: Unknown result type (might be due to invalid IL or missing references)
		//IL_038a: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0408: Unknown result type (might be due to invalid IL or missing references)
		//IL_042e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0437: Unknown result type (might be due to invalid IL or missing references)
		//IL_045d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0466: Unknown result type (might be due to invalid IL or missing references)
		//IL_048c: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bf: Expected O, but got Unknown
		//IL_04ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c5: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(18);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._ExitTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UpdateCurrentLinePositionLocal, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)5, StringName.op_Implicit("position"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.StopLineLocal, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetDrawingModeLocal, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("drawingMode"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ClearDrawnLinesLocal, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.IsDrawing, new PropertyInfo((Type)1, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("playerId"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.IsLocalDrawing, new PropertyInfo((Type)1, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.GetDrawingMode, new PropertyInfo((Type)2, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("playerId"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.GetLocalDrawingMode, new PropertyInfo((Type)2, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)1, StringName.op_Implicit("useOverride"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ToNetPosition, new PropertyInfo((Type)5, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)5, StringName.op_Implicit("pos"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.FromNetPosition, new PropertyInfo((Type)5, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)5, StringName.op_Implicit("pos"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ClearAllLines, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnPlayerScreenChanged, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("playerId"), (PropertyHint)0, "", (PropertyUsageFlags)6, false),
			new PropertyInfo((Type)2, StringName.op_Implicit("oldScreenType"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.TrySendSyncMessage, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SendSyncMessage, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UpdateLocalCursor, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.RepositionBasedOnBackground, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("mapBg"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0353: Unknown result type (might be due to invalid IL or missing references)
		//IL_0316: Unknown result type (might be due to invalid IL or missing references)
		//IL_0349: Unknown result type (might be due to invalid IL or missing references)
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
		if ((ref method) == MethodName.UpdateCurrentLinePositionLocal && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			UpdateCurrentLinePositionLocal(VariantUtils.ConvertTo<Vector2>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.StopLineLocal && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			StopLineLocal();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetDrawingModeLocal && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			SetDrawingModeLocal(VariantUtils.ConvertTo<DrawingMode>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.ClearDrawnLinesLocal && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			ClearDrawnLinesLocal();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.IsDrawing && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			bool flag = IsDrawing(VariantUtils.ConvertTo<ulong>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<bool>(ref flag);
			return true;
		}
		if ((ref method) == MethodName.IsLocalDrawing && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			bool flag2 = IsLocalDrawing();
			ret = VariantUtils.CreateFrom<bool>(ref flag2);
			return true;
		}
		if ((ref method) == MethodName.GetDrawingMode && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			DrawingMode drawingMode = GetDrawingMode(VariantUtils.ConvertTo<ulong>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<DrawingMode>(ref drawingMode);
			return true;
		}
		if ((ref method) == MethodName.GetLocalDrawingMode && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			DrawingMode localDrawingMode = GetLocalDrawingMode(VariantUtils.ConvertTo<bool>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<DrawingMode>(ref localDrawingMode);
			return true;
		}
		if ((ref method) == MethodName.ToNetPosition && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			Vector2 val = ToNetPosition(VariantUtils.ConvertTo<Vector2>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<Vector2>(ref val);
			return true;
		}
		if ((ref method) == MethodName.FromNetPosition && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			Vector2 val2 = FromNetPosition(VariantUtils.ConvertTo<Vector2>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<Vector2>(ref val2);
			return true;
		}
		if ((ref method) == MethodName.ClearAllLines && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			ClearAllLines();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnPlayerScreenChanged && ((NativeVariantPtrArgs)(ref args)).Count == 2)
		{
			OnPlayerScreenChanged(VariantUtils.ConvertTo<ulong>(ref ((NativeVariantPtrArgs)(ref args))[0]), VariantUtils.ConvertTo<NetScreenType>(ref ((NativeVariantPtrArgs)(ref args))[1]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.TrySendSyncMessage && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			TrySendSyncMessage();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SendSyncMessage && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			SendSyncMessage();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.UpdateLocalCursor && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			UpdateLocalCursor();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.RepositionBasedOnBackground && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			RepositionBasedOnBackground(VariantUtils.ConvertTo<Control>(ref ((NativeVariantPtrArgs)(ref args))[0]));
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
		if ((ref method) == MethodName.UpdateCurrentLinePositionLocal)
		{
			return true;
		}
		if ((ref method) == MethodName.StopLineLocal)
		{
			return true;
		}
		if ((ref method) == MethodName.SetDrawingModeLocal)
		{
			return true;
		}
		if ((ref method) == MethodName.ClearDrawnLinesLocal)
		{
			return true;
		}
		if ((ref method) == MethodName.IsDrawing)
		{
			return true;
		}
		if ((ref method) == MethodName.IsLocalDrawing)
		{
			return true;
		}
		if ((ref method) == MethodName.GetDrawingMode)
		{
			return true;
		}
		if ((ref method) == MethodName.GetLocalDrawingMode)
		{
			return true;
		}
		if ((ref method) == MethodName.ToNetPosition)
		{
			return true;
		}
		if ((ref method) == MethodName.FromNetPosition)
		{
			return true;
		}
		if ((ref method) == MethodName.ClearAllLines)
		{
			return true;
		}
		if ((ref method) == MethodName.OnPlayerScreenChanged)
		{
			return true;
		}
		if ((ref method) == MethodName.TrySendSyncMessage)
		{
			return true;
		}
		if ((ref method) == MethodName.SendSyncMessage)
		{
			return true;
		}
		if ((ref method) == MethodName.UpdateLocalCursor)
		{
			return true;
		}
		if ((ref method) == MethodName.RepositionBasedOnBackground)
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
		if ((ref name) == PropertyName._lineDrawScene)
		{
			_lineDrawScene = VariantUtils.ConvertTo<PackedScene>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._lineEraseScene)
		{
			_lineEraseScene = VariantUtils.ConvertTo<PackedScene>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._cursorManager)
		{
			_cursorManager = VariantUtils.ConvertTo<NCursorManager>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._eraserMaterial)
		{
			_eraserMaterial = VariantUtils.ConvertTo<Material>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._defaultSize)
		{
			_defaultSize = VariantUtils.ConvertTo<Vector2>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._lastMessageMsec)
		{
			_lastMessageMsec = VariantUtils.ConvertTo<ulong>(ref value);
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
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName._lineDrawScene)
		{
			value = VariantUtils.CreateFrom<PackedScene>(ref _lineDrawScene);
			return true;
		}
		if ((ref name) == PropertyName._lineEraseScene)
		{
			value = VariantUtils.CreateFrom<PackedScene>(ref _lineEraseScene);
			return true;
		}
		if ((ref name) == PropertyName._cursorManager)
		{
			value = VariantUtils.CreateFrom<NCursorManager>(ref _cursorManager);
			return true;
		}
		if ((ref name) == PropertyName._eraserMaterial)
		{
			value = VariantUtils.CreateFrom<Material>(ref _eraserMaterial);
			return true;
		}
		if ((ref name) == PropertyName._defaultSize)
		{
			value = VariantUtils.CreateFrom<Vector2>(ref _defaultSize);
			return true;
		}
		if ((ref name) == PropertyName._lastMessageMsec)
		{
			value = VariantUtils.CreateFrom<ulong>(ref _lastMessageMsec);
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
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._lineDrawScene, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._lineEraseScene, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._cursorManager, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._eraserMaterial, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)5, PropertyName._defaultSize, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)2, PropertyName._lastMessageMsec, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		info.AddProperty(PropertyName._lineDrawScene, Variant.From<PackedScene>(ref _lineDrawScene));
		info.AddProperty(PropertyName._lineEraseScene, Variant.From<PackedScene>(ref _lineEraseScene));
		info.AddProperty(PropertyName._cursorManager, Variant.From<NCursorManager>(ref _cursorManager));
		info.AddProperty(PropertyName._eraserMaterial, Variant.From<Material>(ref _eraserMaterial));
		info.AddProperty(PropertyName._defaultSize, Variant.From<Vector2>(ref _defaultSize));
		info.AddProperty(PropertyName._lastMessageMsec, Variant.From<ulong>(ref _lastMessageMsec));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._lineDrawScene, ref val))
		{
			_lineDrawScene = ((Variant)(ref val)).As<PackedScene>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._lineEraseScene, ref val2))
		{
			_lineEraseScene = ((Variant)(ref val2)).As<PackedScene>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._cursorManager, ref val3))
		{
			_cursorManager = ((Variant)(ref val3)).As<NCursorManager>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._eraserMaterial, ref val4))
		{
			_eraserMaterial = ((Variant)(ref val4)).As<Material>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._defaultSize, ref val5))
		{
			_defaultSize = ((Variant)(ref val5)).As<Vector2>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._lastMessageMsec, ref val6))
		{
			_lastMessageMsec = ((Variant)(ref val6)).As<ulong>();
		}
	}
}
