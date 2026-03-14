using System;
using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Map;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.HoverTips;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;

namespace MegaCrit.sts2.Core.Nodes.TopBar;

[ScriptPath("res://src/Core/Nodes/TopBar/NTopBarRoomIcon.cs")]
public class NTopBarRoomIcon : NClickableControl
{
	public new class MethodName : NClickableControl.MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName _EnterTree = StringName.op_Implicit("_EnterTree");

		public static readonly StringName _ExitTree = StringName.op_Implicit("_ExitTree");

		public static readonly StringName DebugSetMapPointTypeOverride = StringName.op_Implicit("DebugSetMapPointTypeOverride");

		public static readonly StringName DebugClearMapPointTypeOverride = StringName.op_Implicit("DebugClearMapPointTypeOverride");

		public new static readonly StringName OnFocus = StringName.op_Implicit("OnFocus");

		public static readonly StringName GetHoverTipPrefixForRoomType = StringName.op_Implicit("GetHoverTipPrefixForRoomType");

		public static readonly StringName GetHoverTipPrefixForUnknownRoomType = StringName.op_Implicit("GetHoverTipPrefixForUnknownRoomType");

		public new static readonly StringName OnUnfocus = StringName.op_Implicit("OnUnfocus");

		public static readonly StringName UpdateIcon = StringName.op_Implicit("UpdateIcon");

		public static readonly StringName GetCurrentMapPointType = StringName.op_Implicit("GetCurrentMapPointType");
	}

	public new class PropertyName : NClickableControl.PropertyName
	{
		public static readonly StringName _roomIcon = StringName.op_Implicit("_roomIcon");

		public static readonly StringName _roomIconOutline = StringName.op_Implicit("_roomIconOutline");

		public static readonly StringName _debugMapPointTypeOverride = StringName.op_Implicit("_debugMapPointTypeOverride");
	}

	public new class SignalName : NClickableControl.SignalName
	{
	}

	private IRunState _runState;

	private TextureRect _roomIcon;

	private TextureRect _roomIconOutline;

	private MapPointType _debugMapPointTypeOverride;

	public override void _Ready()
	{
		_roomIcon = ((Node)this).GetNode<TextureRect>(NodePath.op_Implicit("Icon"));
		_roomIconOutline = ((Node)this).GetNode<TextureRect>(NodePath.op_Implicit("Icon/Outline"));
		ConnectSignals();
	}

	public override void _EnterTree()
	{
		RunManager.Instance.RoomEntered += UpdateIcon;
	}

	public override void _ExitTree()
	{
		RunManager.Instance.RoomEntered -= UpdateIcon;
	}

	public void Initialize(IRunState runState)
	{
		_runState = runState;
		UpdateIcon();
	}

	public void DebugSetMapPointTypeOverride(MapPointType mapPointType)
	{
		if (mapPointType != MapPointType.Unassigned)
		{
			_debugMapPointTypeOverride = mapPointType;
		}
	}

	public void DebugClearMapPointTypeOverride()
	{
		_debugMapPointTypeOverride = MapPointType.Unassigned;
	}

	protected override void OnFocus()
	{
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		string hoverTipPrefixForRoomType = GetHoverTipPrefixForRoomType();
		NHoverTipSet nHoverTipSet = NHoverTipSet.CreateAndShow(hoverTip: new HoverTip(new LocString("static_hover_tips", hoverTipPrefixForRoomType + ".title"), new LocString("static_hover_tips", hoverTipPrefixForRoomType + ".description")), owner: (Control)(object)_roomIcon);
		((Control)nHoverTipSet).GlobalPosition = ((Control)_roomIcon).GlobalPosition + new Vector2(0f, ((Control)this).Size.Y + 20f);
	}

	private string GetHoverTipPrefixForRoomType()
	{
		return GetCurrentMapPointType() switch
		{
			MapPointType.Unassigned => "ROOM_MAP", 
			MapPointType.Unknown => GetHoverTipPrefixForUnknownRoomType(), 
			MapPointType.Shop => "ROOM_MERCHANT", 
			MapPointType.Treasure => "ROOM_TREASURE", 
			MapPointType.RestSite => "ROOM_REST", 
			MapPointType.Monster => "ROOM_ENEMY", 
			MapPointType.Elite => "ROOM_ELITE", 
			MapPointType.Boss => "ROOM_BOSS", 
			MapPointType.Ancient => "ROOM_ANCIENT", 
			_ => throw new ArgumentOutOfRangeException(), 
		};
	}

	private string GetHoverTipPrefixForUnknownRoomType()
	{
		AbstractRoom baseRoom = _runState.BaseRoom;
		return baseRoom.RoomType switch
		{
			RoomType.Monster => "ROOM_UNKNOWN_ENEMY", 
			RoomType.Treasure => "ROOM_UNKNOWN_TREASURE", 
			RoomType.Shop => "ROOM_UNKNOWN_MERCHANT", 
			RoomType.Event => "ROOM_UNKNOWN_EVENT", 
			_ => throw new ArgumentOutOfRangeException(), 
		};
	}

	protected override void OnUnfocus()
	{
		NHoverTipSet.Remove((Control)(object)_roomIcon);
	}

	private void UpdateIcon()
	{
		if (_runState.CurrentRoom != null)
		{
			AbstractRoom baseRoom = _runState.BaseRoom;
			ActModel act = _runState.Act;
			MapPointType currentMapPointType = GetCurrentMapPointType();
			ModelId modelId = null;
			switch (currentMapPointType)
			{
			case MapPointType.Boss:
				modelId = ((_runState.CurrentMapPoint == _runState.Map.SecondBossMapPoint) ? act.SecondBossEncounter.Id : act.BossEncounter.Id);
				break;
			case MapPointType.Ancient:
				modelId = act.Ancient.Id;
				break;
			}
			string roomIconPath = ImageHelper.GetRoomIconPath(currentMapPointType, baseRoom.RoomType, modelId);
			if (roomIconPath != null)
			{
				((CanvasItem)_roomIcon).Visible = true;
				_roomIcon.Texture = (Texture2D)(object)PreloadManager.Cache.GetCompressedTexture2D(roomIconPath);
			}
			else
			{
				((CanvasItem)_roomIcon).Visible = false;
			}
			string roomIconOutlinePath = ImageHelper.GetRoomIconOutlinePath(currentMapPointType, baseRoom.RoomType, modelId);
			if (roomIconOutlinePath != null)
			{
				((CanvasItem)_roomIconOutline).Visible = true;
				_roomIconOutline.Texture = (Texture2D)(object)PreloadManager.Cache.GetCompressedTexture2D(roomIconOutlinePath);
			}
			else
			{
				((CanvasItem)_roomIconOutline).Visible = false;
			}
			if (baseRoom.IsVictoryRoom)
			{
				((CanvasItem)_roomIcon).Visible = false;
				((CanvasItem)_roomIconOutline).Visible = false;
				((Control)this).FocusMode = (FocusModeEnum)0;
				((Control)this).MouseFilter = (MouseFilterEnum)2;
			}
		}
	}

	private MapPointType GetCurrentMapPointType()
	{
		if (_debugMapPointTypeOverride == MapPointType.Unassigned)
		{
			return _runState.CurrentMapPoint?.PointType ?? MapPointType.Unassigned;
		}
		return _debugMapPointTypeOverride;
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
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(11);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._EnterTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._ExitTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.DebugSetMapPointTypeOverride, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("mapPointType"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.DebugClearMapPointTypeOverride, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnFocus, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.GetHoverTipPrefixForRoomType, new PropertyInfo((Type)4, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.GetHoverTipPrefixForUnknownRoomType, new PropertyInfo((Type)4, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnUnfocus, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UpdateIcon, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.GetCurrentMapPointType, new PropertyInfo((Type)2, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
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
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
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
		if ((ref method) == MethodName.DebugSetMapPointTypeOverride && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			DebugSetMapPointTypeOverride(VariantUtils.ConvertTo<MapPointType>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.DebugClearMapPointTypeOverride && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			DebugClearMapPointTypeOverride();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnFocus && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnFocus();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.GetHoverTipPrefixForRoomType && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			string hoverTipPrefixForRoomType = GetHoverTipPrefixForRoomType();
			ret = VariantUtils.CreateFrom<string>(ref hoverTipPrefixForRoomType);
			return true;
		}
		if ((ref method) == MethodName.GetHoverTipPrefixForUnknownRoomType && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			string hoverTipPrefixForUnknownRoomType = GetHoverTipPrefixForUnknownRoomType();
			ret = VariantUtils.CreateFrom<string>(ref hoverTipPrefixForUnknownRoomType);
			return true;
		}
		if ((ref method) == MethodName.OnUnfocus && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnUnfocus();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.UpdateIcon && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			UpdateIcon();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.GetCurrentMapPointType && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			MapPointType currentMapPointType = GetCurrentMapPointType();
			ret = VariantUtils.CreateFrom<MapPointType>(ref currentMapPointType);
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
		if ((ref method) == MethodName._EnterTree)
		{
			return true;
		}
		if ((ref method) == MethodName._ExitTree)
		{
			return true;
		}
		if ((ref method) == MethodName.DebugSetMapPointTypeOverride)
		{
			return true;
		}
		if ((ref method) == MethodName.DebugClearMapPointTypeOverride)
		{
			return true;
		}
		if ((ref method) == MethodName.OnFocus)
		{
			return true;
		}
		if ((ref method) == MethodName.GetHoverTipPrefixForRoomType)
		{
			return true;
		}
		if ((ref method) == MethodName.GetHoverTipPrefixForUnknownRoomType)
		{
			return true;
		}
		if ((ref method) == MethodName.OnUnfocus)
		{
			return true;
		}
		if ((ref method) == MethodName.UpdateIcon)
		{
			return true;
		}
		if ((ref method) == MethodName.GetCurrentMapPointType)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._roomIcon)
		{
			_roomIcon = VariantUtils.ConvertTo<TextureRect>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._roomIconOutline)
		{
			_roomIconOutline = VariantUtils.ConvertTo<TextureRect>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._debugMapPointTypeOverride)
		{
			_debugMapPointTypeOverride = VariantUtils.ConvertTo<MapPointType>(ref value);
			return true;
		}
		return base.SetGodotClassPropertyValue(in name, in value);
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
		if ((ref name) == PropertyName._roomIcon)
		{
			value = VariantUtils.CreateFrom<TextureRect>(ref _roomIcon);
			return true;
		}
		if ((ref name) == PropertyName._roomIconOutline)
		{
			value = VariantUtils.CreateFrom<TextureRect>(ref _roomIconOutline);
			return true;
		}
		if ((ref name) == PropertyName._debugMapPointTypeOverride)
		{
			value = VariantUtils.CreateFrom<MapPointType>(ref _debugMapPointTypeOverride);
			return true;
		}
		return base.GetGodotClassPropertyValue(in name, out value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._roomIcon, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._roomIconOutline, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)2, PropertyName._debugMapPointTypeOverride, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName._roomIcon, Variant.From<TextureRect>(ref _roomIcon));
		info.AddProperty(PropertyName._roomIconOutline, Variant.From<TextureRect>(ref _roomIconOutline));
		info.AddProperty(PropertyName._debugMapPointTypeOverride, Variant.From<MapPointType>(ref _debugMapPointTypeOverride));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._roomIcon, ref val))
		{
			_roomIcon = ((Variant)(ref val)).As<TextureRect>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._roomIconOutline, ref val2))
		{
			_roomIconOutline = ((Variant)(ref val2)).As<TextureRect>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._debugMapPointTypeOverride, ref val3))
		{
			_debugMapPointTypeOverride = ((Variant)(ref val3)).As<MapPointType>();
		}
	}
}
