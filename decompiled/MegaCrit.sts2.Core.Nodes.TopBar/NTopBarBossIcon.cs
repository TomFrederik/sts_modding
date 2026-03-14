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

[ScriptPath("res://src/Core/Nodes/TopBar/NTopBarBossIcon.cs")]
public class NTopBarBossIcon : NClickableControl
{
	public new class MethodName : NClickableControl.MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName _EnterTree = StringName.op_Implicit("_EnterTree");

		public static readonly StringName _ExitTree = StringName.op_Implicit("_ExitTree");

		public static readonly StringName OnRoomEntered = StringName.op_Implicit("OnRoomEntered");

		public static readonly StringName OnActEntered = StringName.op_Implicit("OnActEntered");

		public static readonly StringName RefreshBossIcon = StringName.op_Implicit("RefreshBossIcon");

		public static readonly StringName RefreshSecondBossIconColor = StringName.op_Implicit("RefreshSecondBossIconColor");

		public new static readonly StringName OnFocus = StringName.op_Implicit("OnFocus");

		public new static readonly StringName OnUnfocus = StringName.op_Implicit("OnUnfocus");
	}

	public new class PropertyName : NClickableControl.PropertyName
	{
		public static readonly StringName ShouldOnlyShowSecondBossIcon = StringName.op_Implicit("ShouldOnlyShowSecondBossIcon");

		public static readonly StringName _bossIcon = StringName.op_Implicit("_bossIcon");

		public static readonly StringName _bossIconOutline = StringName.op_Implicit("_bossIconOutline");

		public static readonly StringName _secondBossIcon = StringName.op_Implicit("_secondBossIcon");

		public static readonly StringName _secondBossIconOutline = StringName.op_Implicit("_secondBossIconOutline");
	}

	public new class SignalName : NClickableControl.SignalName
	{
	}

	private static readonly LocString _bossHoverTipTitle = new LocString("static_hover_tips", "BOSS.title");

	private static readonly LocString _bossHoverTipDescription = new LocString("static_hover_tips", "BOSS.description");

	private static readonly LocString _doubleBossHoverTipTitle = new LocString("static_hover_tips", "DOUBLE_BOSS.title");

	private static readonly LocString _doubleBossHoverTipDescription = new LocString("static_hover_tips", "DOUBLE_BOSS.description");

	private TextureRect _bossIcon;

	private TextureRect _bossIconOutline;

	private TextureRect? _secondBossIcon;

	private TextureRect? _secondBossIconOutline;

	private static readonly StringName _tintColor = new StringName("tint_color");

	private const string _secondBossIconScenePath = "res://scenes/ui/top_bar/second_boss_icon.tscn";

	private IRunState _runState;

	private bool ShouldOnlyShowSecondBossIcon
	{
		get
		{
			if (_runState.Map.SecondBossMapPoint != null)
			{
				return _runState.CurrentMapPoint == _runState.Map.BossMapPoint;
			}
			return false;
		}
	}

	public override void _Ready()
	{
		_bossIcon = ((Node)this).GetNode<TextureRect>(NodePath.op_Implicit("Icon"));
		_bossIconOutline = ((Node)this).GetNode<TextureRect>(NodePath.op_Implicit("Icon/Outline"));
		ConnectSignals();
	}

	public void Initialize(IRunState runState)
	{
		_runState = runState;
		OnActEntered();
	}

	public override void _EnterTree()
	{
		RunManager.Instance.ActEntered += OnActEntered;
		RunManager.Instance.RoomEntered += OnRoomEntered;
	}

	public override void _ExitTree()
	{
		RunManager.Instance.ActEntered -= OnActEntered;
		RunManager.Instance.RoomEntered -= OnRoomEntered;
	}

	private void OnRoomEntered()
	{
		if (_runState.CurrentRoom == null)
		{
			return;
		}
		AbstractRoom baseRoom = _runState.BaseRoom;
		((CanvasItem)this).Visible = baseRoom.RoomType != RoomType.Boss || ShouldOnlyShowSecondBossIcon;
		((Control)this).FocusMode = (FocusModeEnum)((baseRoom.RoomType != RoomType.Boss || ShouldOnlyShowSecondBossIcon) ? 2 : 0);
		if (ShouldOnlyShowSecondBossIcon)
		{
			RefreshBossIcon();
		}
		RefreshSecondBossIconColor();
		if (_runState.CurrentRoom.IsVictoryRoom)
		{
			((CanvasItem)_bossIcon).SetVisible(false);
			((CanvasItem)_bossIconOutline).SetVisible(false);
			TextureRect? secondBossIcon = _secondBossIcon;
			if (secondBossIcon != null)
			{
				((CanvasItem)secondBossIcon).SetVisible(false);
			}
			TextureRect? secondBossIconOutline = _secondBossIconOutline;
			if (secondBossIconOutline != null)
			{
				((CanvasItem)secondBossIconOutline).SetVisible(false);
			}
			((Control)this).FocusMode = (FocusModeEnum)0;
			((Control)this).MouseFilter = (MouseFilterEnum)2;
		}
	}

	private void OnActEntered()
	{
		RefreshBossIcon();
	}

	public void RefreshBossIcon()
	{
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		EncounterModel encounterModel = (ShouldOnlyShowSecondBossIcon ? _runState.Act.SecondBossEncounter : _runState.Act.BossEncounter);
		string roomIconPath = ImageHelper.GetRoomIconPath(MapPointType.Boss, RoomType.Boss, encounterModel.Id);
		string roomIconOutlinePath = ImageHelper.GetRoomIconOutlinePath(MapPointType.Boss, RoomType.Boss, encounterModel.Id);
		_bossIcon.Texture = PreloadManager.Cache.GetTexture2D(roomIconPath);
		_bossIconOutline.Texture = PreloadManager.Cache.GetTexture2D(roomIconOutlinePath);
		EncounterModel secondBossEncounter = _runState.Act.SecondBossEncounter;
		if (secondBossEncounter != null && !ShouldOnlyShowSecondBossIcon)
		{
			if (_secondBossIcon == null)
			{
				PackedScene val = GD.Load<PackedScene>("res://scenes/ui/top_bar/second_boss_icon.tscn");
				_secondBossIcon = val.Instantiate<TextureRect>((GenEditState)0);
				_secondBossIconOutline = ((Node)_secondBossIcon).GetNode<TextureRect>(NodePath.op_Implicit("%Outline"));
				((Control)_secondBossIcon).MouseFilter = (MouseFilterEnum)1;
				((Control)_secondBossIconOutline).MouseFilter = (MouseFilterEnum)1;
				((Node)(object)_bossIcon).AddChildSafely((Node?)(object)_secondBossIcon);
				((Control)_secondBossIcon).Position = new Vector2(30f, 22f);
			}
			string roomIconPath2 = ImageHelper.GetRoomIconPath(MapPointType.Boss, RoomType.Boss, secondBossEncounter.Id);
			string roomIconOutlinePath2 = ImageHelper.GetRoomIconOutlinePath(MapPointType.Boss, RoomType.Boss, secondBossEncounter.Id);
			_secondBossIcon.Texture = PreloadManager.Cache.GetTexture2D(roomIconPath2);
			_secondBossIconOutline.Texture = PreloadManager.Cache.GetTexture2D(roomIconOutlinePath2);
			((CanvasItem)_secondBossIcon).Visible = true;
			RefreshSecondBossIconColor();
		}
		else
		{
			TextureRect? secondBossIcon = _secondBossIcon;
			if (secondBossIcon != null)
			{
				((CanvasItem)secondBossIcon).SetVisible(false);
			}
			TextureRect? secondBossIconOutline = _secondBossIconOutline;
			if (secondBossIconOutline != null)
			{
				((CanvasItem)secondBossIconOutline).SetVisible(false);
			}
		}
	}

	private void RefreshSecondBossIconColor()
	{
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		TextureRect? secondBossIcon = _secondBossIcon;
		Material obj = ((secondBossIcon != null) ? ((CanvasItem)secondBossIcon).Material : null);
		ShaderMaterial val = (ShaderMaterial)(object)((obj is ShaderMaterial) ? obj : null);
		if (val != null)
		{
			TextureRect? secondBossIconOutline = _secondBossIconOutline;
			Material obj2 = ((secondBossIconOutline != null) ? ((CanvasItem)secondBossIconOutline).Material : null);
			ShaderMaterial val2 = (ShaderMaterial)(object)((obj2 is ShaderMaterial) ? obj2 : null);
			if (val2 != null)
			{
				ActModel act = _runState.Act;
				MapPoint currentMapPoint = _runState.CurrentMapPoint;
				MapPoint bossMapPoint = _runState.Map.BossMapPoint;
				MapPoint secondBossMapPoint = _runState.Map.SecondBossMapPoint;
				Color val3 = ((currentMapPoint == bossMapPoint || currentMapPoint == secondBossMapPoint) ? act.MapTraveledColor : act.MapUntraveledColor);
				val.SetShaderParameter(_tintColor, Variant.op_Implicit(new Vector3(val3.R, val3.G, val3.B)));
				val2.SetShaderParameter(_tintColor, Variant.op_Implicit(new Vector3(val3.R, val3.G, val3.B)));
			}
		}
	}

	protected override void OnFocus()
	{
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		EncounterModel bossEncounter = _runState.Act.BossEncounter;
		EncounterModel secondBossEncounter = _runState.Act.SecondBossEncounter;
		HoverTip hoverTip;
		if (secondBossEncounter != null && !ShouldOnlyShowSecondBossIcon)
		{
			_doubleBossHoverTipTitle.Add("BossName1", bossEncounter.Title);
			_doubleBossHoverTipTitle.Add("BossName2", secondBossEncounter.Title);
			_doubleBossHoverTipDescription.Add("BossName1", bossEncounter.Title);
			_doubleBossHoverTipDescription.Add("BossName2", secondBossEncounter.Title);
			hoverTip = new HoverTip(_doubleBossHoverTipTitle, _doubleBossHoverTipDescription);
		}
		else
		{
			_bossHoverTipTitle.Add("BossName", ShouldOnlyShowSecondBossIcon ? secondBossEncounter.Title : bossEncounter.Title);
			_bossHoverTipDescription.Add("BossName", ShouldOnlyShowSecondBossIcon ? secondBossEncounter.Title : bossEncounter.Title);
			hoverTip = new HoverTip(_bossHoverTipTitle, _bossHoverTipDescription);
		}
		NHoverTipSet nHoverTipSet = NHoverTipSet.CreateAndShow((Control)(object)this, hoverTip);
		((Control)nHoverTipSet).GlobalPosition = ((Control)_bossIcon).GlobalPosition + new Vector2(0f, ((Control)this).Size.Y + 20f);
	}

	protected override void OnUnfocus()
	{
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
		List<MethodInfo> list = new List<MethodInfo>(9);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._EnterTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._ExitTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnRoomEntered, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnActEntered, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.RefreshBossIcon, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.RefreshSecondBossIconColor, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnFocus, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnUnfocus, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
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
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
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
		if ((ref method) == MethodName.OnRoomEntered && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnRoomEntered();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnActEntered && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnActEntered();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.RefreshBossIcon && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			RefreshBossIcon();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.RefreshSecondBossIconColor && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			RefreshSecondBossIconColor();
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
		if ((ref method) == MethodName._EnterTree)
		{
			return true;
		}
		if ((ref method) == MethodName._ExitTree)
		{
			return true;
		}
		if ((ref method) == MethodName.OnRoomEntered)
		{
			return true;
		}
		if ((ref method) == MethodName.OnActEntered)
		{
			return true;
		}
		if ((ref method) == MethodName.RefreshBossIcon)
		{
			return true;
		}
		if ((ref method) == MethodName.RefreshSecondBossIconColor)
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
		if ((ref name) == PropertyName._bossIcon)
		{
			_bossIcon = VariantUtils.ConvertTo<TextureRect>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._bossIconOutline)
		{
			_bossIconOutline = VariantUtils.ConvertTo<TextureRect>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._secondBossIcon)
		{
			_secondBossIcon = VariantUtils.ConvertTo<TextureRect>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._secondBossIconOutline)
		{
			_secondBossIconOutline = VariantUtils.ConvertTo<TextureRect>(ref value);
			return true;
		}
		return base.SetGodotClassPropertyValue(in name, in value);
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
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName.ShouldOnlyShowSecondBossIcon)
		{
			bool shouldOnlyShowSecondBossIcon = ShouldOnlyShowSecondBossIcon;
			value = VariantUtils.CreateFrom<bool>(ref shouldOnlyShowSecondBossIcon);
			return true;
		}
		if ((ref name) == PropertyName._bossIcon)
		{
			value = VariantUtils.CreateFrom<TextureRect>(ref _bossIcon);
			return true;
		}
		if ((ref name) == PropertyName._bossIconOutline)
		{
			value = VariantUtils.CreateFrom<TextureRect>(ref _bossIconOutline);
			return true;
		}
		if ((ref name) == PropertyName._secondBossIcon)
		{
			value = VariantUtils.CreateFrom<TextureRect>(ref _secondBossIcon);
			return true;
		}
		if ((ref name) == PropertyName._secondBossIconOutline)
		{
			value = VariantUtils.CreateFrom<TextureRect>(ref _secondBossIconOutline);
			return true;
		}
		return base.GetGodotClassPropertyValue(in name, out value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._bossIcon, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._bossIconOutline, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._secondBossIcon, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._secondBossIconOutline, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName.ShouldOnlyShowSecondBossIcon, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName._bossIcon, Variant.From<TextureRect>(ref _bossIcon));
		info.AddProperty(PropertyName._bossIconOutline, Variant.From<TextureRect>(ref _bossIconOutline));
		info.AddProperty(PropertyName._secondBossIcon, Variant.From<TextureRect>(ref _secondBossIcon));
		info.AddProperty(PropertyName._secondBossIconOutline, Variant.From<TextureRect>(ref _secondBossIconOutline));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._bossIcon, ref val))
		{
			_bossIcon = ((Variant)(ref val)).As<TextureRect>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._bossIconOutline, ref val2))
		{
			_bossIconOutline = ((Variant)(ref val2)).As<TextureRect>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._secondBossIcon, ref val3))
		{
			_secondBossIcon = ((Variant)(ref val3)).As<TextureRect>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._secondBossIconOutline, ref val4))
		{
			_secondBossIconOutline = ((Variant)(ref val4)).As<TextureRect>();
		}
	}
}
