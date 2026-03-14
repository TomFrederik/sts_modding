using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Debug;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Multiplayer.Game.PeerInput;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.Screens.Map;
using MegaCrit.Sts2.Core.Nodes.Vfx;

namespace MegaCrit.Sts2.Core.Nodes.Multiplayer;

[ScriptPath("res://src/Core/Nodes/Multiplayer/NRemoteMouseCursorContainer.cs")]
public class NRemoteMouseCursorContainer : Control
{
	public class MethodName : MethodName
	{
		public static readonly StringName Deinitialize = StringName.op_Implicit("Deinitialize");

		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName _ExitTree = StringName.op_Implicit("_ExitTree");

		public static readonly StringName ForceUpdateAllCursors = StringName.op_Implicit("ForceUpdateAllCursors");

		public static readonly StringName GetCursorPosition = StringName.op_Implicit("GetCursorPosition");

		public static readonly StringName OnInputStateAdded = StringName.op_Implicit("OnInputStateAdded");

		public static readonly StringName OnInputStateRemoved = StringName.op_Implicit("OnInputStateRemoved");

		public static readonly StringName AddCursor = StringName.op_Implicit("AddCursor");

		public static readonly StringName OnInputStateChanged = StringName.op_Implicit("OnInputStateChanged");

		public static readonly StringName DrawingCursorStateChanged = StringName.op_Implicit("DrawingCursorStateChanged");

		public static readonly StringName GetDrawingMode = StringName.op_Implicit("GetDrawingMode");

		public static readonly StringName GetCursor = StringName.op_Implicit("GetCursor");

		public static readonly StringName RemoveCursor = StringName.op_Implicit("RemoveCursor");

		public static readonly StringName UpdateCursorVisibility = StringName.op_Implicit("UpdateCursorVisibility");

		public static readonly StringName _Input = StringName.op_Implicit("_Input");

		public static readonly StringName OnGuiFocusChanged = StringName.op_Implicit("OnGuiFocusChanged");

		public static readonly StringName ApplyDebugUiVisibility = StringName.op_Implicit("ApplyDebugUiVisibility");
	}

	public class PropertyName : PropertyName
	{
	}

	public class SignalName : SignalName
	{
	}

	private static bool _isDebugUiVisible = true;

	private PeerInputSynchronizer? _synchronizer;

	private readonly List<NRemoteMouseCursor> _cursors = new List<NRemoteMouseCursor>();

	public void Initialize(PeerInputSynchronizer synchronizer, IEnumerable<ulong> connectedPlayerIds)
	{
		if (_synchronizer != null)
		{
			Deinitialize();
		}
		_synchronizer = synchronizer;
		_synchronizer.StateAdded += OnInputStateAdded;
		_synchronizer.StateChanged += OnInputStateChanged;
		_synchronizer.StateRemoved += OnInputStateRemoved;
		_synchronizer.NetService.Disconnected += NetServiceDisconnected;
	}

	private void NetServiceDisconnected(NetErrorInfo _)
	{
		Deinitialize();
	}

	public void Deinitialize()
	{
		if (_synchronizer != null)
		{
			_synchronizer.StateAdded -= OnInputStateAdded;
			_synchronizer.StateChanged -= OnInputStateChanged;
			_synchronizer.StateRemoved -= OnInputStateRemoved;
			_synchronizer.NetService.Disconnected -= NetServiceDisconnected;
			_synchronizer.Dispose();
			_synchronizer = null;
		}
		foreach (NRemoteMouseCursor cursor in _cursors)
		{
			((Node)(object)cursor).QueueFreeSafely();
		}
		_cursors.Clear();
	}

	public override void _Ready()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)((Node)this).GetViewport()).Connect(SignalName.GuiFocusChanged, Callable.From<Control>((Action<Control>)OnGuiFocusChanged), 0u);
	}

	public override void _ExitTree()
	{
		Deinitialize();
	}

	public void ForceUpdateAllCursors()
	{
		foreach (NRemoteMouseCursor cursor in _cursors)
		{
			OnInputStateChanged(cursor.PlayerId);
		}
	}

	public Vector2 GetCursorPosition(ulong playerId)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return ((Control)GetCursor(playerId)).Position;
	}

	private void OnInputStateAdded(ulong playerId)
	{
		AddCursor(playerId);
	}

	private void OnInputStateRemoved(ulong playerId)
	{
		RemoveCursor(playerId);
	}

	private void AddCursor(ulong playerId)
	{
		if (playerId != _synchronizer?.NetService.NetId)
		{
			if (_cursors.Any((NRemoteMouseCursor c) => c.PlayerId == playerId))
			{
				Log.Error($"Tried to add cursor for player {playerId} twice!");
			}
			else
			{
				NRemoteMouseCursor nRemoteMouseCursor = NRemoteMouseCursor.Create(playerId);
				_cursors.Add(nRemoteMouseCursor);
				((Node)(object)this).AddChildSafely((Node?)(object)nRemoteMouseCursor);
			}
		}
	}

	private void OnInputStateChanged(ulong playerId)
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		if (playerId == _synchronizer?.NetService.NetId)
		{
			UpdateCursorVisibility();
			return;
		}
		Vector2 controlSpaceFocusPosition = _synchronizer.GetControlSpaceFocusPosition(playerId, (Control)(object)this);
		NRemoteMouseCursor cursor = GetCursor(playerId);
		cursor.SetNextPosition(controlSpaceFocusPosition);
		cursor.UpdateImage(_synchronizer.GetMouseDown(playerId), GetDrawingMode(playerId));
		UpdateCursorVisibility();
	}

	public void DrawingCursorStateChanged(ulong playerId)
	{
		GetCursor(playerId)?.UpdateImage(_synchronizer.GetMouseDown(playerId), GetDrawingMode(playerId));
	}

	private static DrawingMode GetDrawingMode(ulong playerId)
	{
		if (NRun.Instance != null)
		{
			return NRun.Instance.GlobalUi.MapScreen.Drawings.GetDrawingMode(playerId);
		}
		return DrawingMode.None;
	}

	private NRemoteMouseCursor? GetCursor(ulong playerId)
	{
		return _cursors.FirstOrDefault((NRemoteMouseCursor c) => c.PlayerId == playerId);
	}

	private void RemoveCursor(ulong playerId)
	{
		NRemoteMouseCursor cursor = GetCursor(playerId);
		if (cursor != null)
		{
			((Node)(object)cursor).QueueFreeSafely();
			_cursors.Remove(cursor);
		}
	}

	private void UpdateCursorVisibility()
	{
		NetScreenType screenType = _synchronizer.GetScreenType(_synchronizer.NetService.NetId);
		foreach (NRemoteMouseCursor cursor in _cursors)
		{
			NetScreenType screenType2 = _synchronizer.GetScreenType(cursor.PlayerId);
			bool flag = screenType == screenType2;
			bool flag2 = (((uint)(screenType2 - 5) <= 3u || screenType2 == NetScreenType.RemotePlayerExpandedState) ? true : false);
			bool flag3 = flag2;
			bool flag4 = screenType2 == NetScreenType.SharedRelicPicking;
			((CanvasItem)cursor).Visible = flag && !flag3 && !flag4;
		}
	}

	public override void _Input(InputEvent inputEvent)
	{
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Invalid comparison between Unknown and I8
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		if (_synchronizer == null)
		{
			return;
		}
		if (inputEvent.IsActionReleased(DebugHotkey.hideMpCursors, false))
		{
			_isDebugUiVisible = !_isDebugUiVisible;
			ApplyDebugUiVisibility();
			((Node)(object)NGame.Instance).AddChildSafely((Node?)(object)NFullscreenTextVfx.Create(_isDebugUiVisible ? "Show MP Cursors" : "Hide MP Cursors"));
		}
		InputEventMouseMotion val = (InputEventMouseMotion)(object)((inputEvent is InputEventMouseMotion) ? inputEvent : null);
		if (val != null)
		{
			_synchronizer.SyncLocalIsUsingController(isUsingController: false);
			if (!((CanvasItem)NGame.Instance.ReactionWheel).Visible)
			{
				_synchronizer.SyncLocalMousePos(((InputEventMouse)val).Position, (Control)(object)this);
			}
			return;
		}
		InputEventMouseButton val2 = (InputEventMouseButton)(object)((inputEvent is InputEventMouseButton) ? inputEvent : null);
		if (val2 != null)
		{
			_synchronizer.SyncLocalIsUsingController(isUsingController: false);
			if ((long)val2.ButtonIndex == 1)
			{
				_synchronizer.SyncLocalMouseDown(val2.Pressed);
			}
		}
	}

	private void OnGuiFocusChanged(Control focused)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		if (_synchronizer != null)
		{
			NControllerManager? instance = NControllerManager.Instance;
			if (instance != null && instance.IsUsingController)
			{
				_synchronizer.SyncLocalIsUsingController(isUsingController: true);
				_synchronizer.SyncLocalControllerFocus(focused.GlobalPosition + focused.Size * 0.5f, (Control)(object)this);
			}
		}
	}

	private void ApplyDebugUiVisibility()
	{
		((CanvasItem)this).Visible = _isDebugUiVisible;
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
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0284: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0307: Unknown result type (might be due to invalid IL or missing references)
		//IL_0332: Unknown result type (might be due to invalid IL or missing references)
		//IL_033d: Expected O, but got Unknown
		//IL_0338: Unknown result type (might be due to invalid IL or missing references)
		//IL_035b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0366: Unknown result type (might be due to invalid IL or missing references)
		//IL_038c: Unknown result type (might be due to invalid IL or missing references)
		//IL_03af: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_040f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0437: Unknown result type (might be due to invalid IL or missing references)
		//IL_0442: Expected O, but got Unknown
		//IL_043d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0448: Unknown result type (might be due to invalid IL or missing references)
		//IL_046e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0496: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a1: Expected O, but got Unknown
		//IL_049c: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d6: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(17);
		list.Add(new MethodInfo(MethodName.Deinitialize, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._ExitTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ForceUpdateAllCursors, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.GetCursorPosition, new PropertyInfo((Type)5, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("playerId"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnInputStateAdded, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("playerId"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnInputStateRemoved, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("playerId"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AddCursor, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("playerId"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnInputStateChanged, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("playerId"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.DrawingCursorStateChanged, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("playerId"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.GetDrawingMode, new PropertyInfo((Type)2, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)33, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("playerId"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.GetCursor, new PropertyInfo((Type)24, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("playerId"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.RemoveCursor, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("playerId"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UpdateCursorVisibility, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Input, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("inputEvent"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("InputEvent"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnGuiFocusChanged, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("focused"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ApplyDebugUiVisibility, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_026b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0290: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0325: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_031b: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.Deinitialize && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			Deinitialize();
			ret = default(godot_variant);
			return true;
		}
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
		if ((ref method) == MethodName.ForceUpdateAllCursors && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			ForceUpdateAllCursors();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.GetCursorPosition && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			Vector2 cursorPosition = GetCursorPosition(VariantUtils.ConvertTo<ulong>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<Vector2>(ref cursorPosition);
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
		if ((ref method) == MethodName.AddCursor && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			AddCursor(VariantUtils.ConvertTo<ulong>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnInputStateChanged && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OnInputStateChanged(VariantUtils.ConvertTo<ulong>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.DrawingCursorStateChanged && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			DrawingCursorStateChanged(VariantUtils.ConvertTo<ulong>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.GetDrawingMode && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			DrawingMode drawingMode = GetDrawingMode(VariantUtils.ConvertTo<ulong>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<DrawingMode>(ref drawingMode);
			return true;
		}
		if ((ref method) == MethodName.GetCursor && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			NRemoteMouseCursor cursor = GetCursor(VariantUtils.ConvertTo<ulong>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<NRemoteMouseCursor>(ref cursor);
			return true;
		}
		if ((ref method) == MethodName.RemoveCursor && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			RemoveCursor(VariantUtils.ConvertTo<ulong>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.UpdateCursorVisibility && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			UpdateCursorVisibility();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._Input && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			((Node)this)._Input(VariantUtils.ConvertTo<InputEvent>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnGuiFocusChanged && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OnGuiFocusChanged(VariantUtils.ConvertTo<Control>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.ApplyDebugUiVisibility && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			ApplyDebugUiVisibility();
			ret = default(godot_variant);
			return true;
		}
		return ((Control)this).InvokeGodotClassMethod(ref method, args, ref ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static bool InvokeGodotClassStaticMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.GetDrawingMode && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			DrawingMode drawingMode = GetDrawingMode(VariantUtils.ConvertTo<ulong>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<DrawingMode>(ref drawingMode);
			return true;
		}
		ret = default(godot_variant);
		return false;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if ((ref method) == MethodName.Deinitialize)
		{
			return true;
		}
		if ((ref method) == MethodName._Ready)
		{
			return true;
		}
		if ((ref method) == MethodName._ExitTree)
		{
			return true;
		}
		if ((ref method) == MethodName.ForceUpdateAllCursors)
		{
			return true;
		}
		if ((ref method) == MethodName.GetCursorPosition)
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
		if ((ref method) == MethodName.AddCursor)
		{
			return true;
		}
		if ((ref method) == MethodName.OnInputStateChanged)
		{
			return true;
		}
		if ((ref method) == MethodName.DrawingCursorStateChanged)
		{
			return true;
		}
		if ((ref method) == MethodName.GetDrawingMode)
		{
			return true;
		}
		if ((ref method) == MethodName.GetCursor)
		{
			return true;
		}
		if ((ref method) == MethodName.RemoveCursor)
		{
			return true;
		}
		if ((ref method) == MethodName.UpdateCursorVisibility)
		{
			return true;
		}
		if ((ref method) == MethodName._Input)
		{
			return true;
		}
		if ((ref method) == MethodName.OnGuiFocusChanged)
		{
			return true;
		}
		if ((ref method) == MethodName.ApplyDebugUiVisibility)
		{
			return true;
		}
		return ((Control)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).SaveGodotObjectData(info);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
	}
}
