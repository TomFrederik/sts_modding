using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Map;
using MegaCrit.Sts2.Core.Nodes.HoverTips;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Runs.History;

namespace MegaCrit.Sts2.Core.Nodes.Screens.Map;

[ScriptPath("res://src/Core/Nodes/Screens/Map/NNormalMapPoint.cs")]
public class NNormalMapPoint : NMapPoint
{
	public new class MethodName : NMapPoint.MethodName
	{
		public static readonly StringName IconName = StringName.op_Implicit("IconName");

		public static readonly StringName IconPath = StringName.op_Implicit("IconPath");

		public static readonly StringName OutlinePath = StringName.op_Implicit("OutlinePath");

		public static readonly StringName UnknownIconPath = StringName.op_Implicit("UnknownIconPath");

		public static readonly StringName UnknownOutlinePath = StringName.op_Implicit("UnknownOutlinePath");

		public new static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public new static readonly StringName _EnterTree = StringName.op_Implicit("_EnterTree");

		public static readonly StringName RefreshMarkedIconVisibility = StringName.op_Implicit("RefreshMarkedIconVisibility");

		public new static readonly StringName _ExitTree = StringName.op_Implicit("_ExitTree");

		public static readonly StringName _Process = StringName.op_Implicit("_Process");

		public new static readonly StringName OnSelected = StringName.op_Implicit("OnSelected");

		public new static readonly StringName OnFocus = StringName.op_Implicit("OnFocus");

		public new static readonly StringName OnUnfocus = StringName.op_Implicit("OnUnfocus");

		public new static readonly StringName OnPress = StringName.op_Implicit("OnPress");

		public static readonly StringName SetAngle = StringName.op_Implicit("SetAngle");

		public new static readonly StringName RefreshColorInstantly = StringName.op_Implicit("RefreshColorInstantly");

		public new static readonly StringName RefreshState = StringName.op_Implicit("RefreshState");

		public static readonly StringName UpdateIcon = StringName.op_Implicit("UpdateIcon");

		public static readonly StringName ShowCircleVfx = StringName.op_Implicit("ShowCircleVfx");

		public static readonly StringName AnimHover = StringName.op_Implicit("AnimHover");

		public static readonly StringName AnimUnhover = StringName.op_Implicit("AnimUnhover");

		public static readonly StringName AnimPressDown = StringName.op_Implicit("AnimPressDown");

		public static readonly StringName OnHighlightPointType = StringName.op_Implicit("OnHighlightPointType");
	}

	public new class PropertyName : NMapPoint.PropertyName
	{
		public new static readonly StringName TraveledColor = StringName.op_Implicit("TraveledColor");

		public new static readonly StringName UntravelableColor = StringName.op_Implicit("UntravelableColor");

		public new static readonly StringName HoveredColor = StringName.op_Implicit("HoveredColor");

		public new static readonly StringName HoverScale = StringName.op_Implicit("HoverScale");

		public new static readonly StringName DownScale = StringName.op_Implicit("DownScale");

		public static readonly StringName _iconContainer = StringName.op_Implicit("_iconContainer");

		public static readonly StringName _icon = StringName.op_Implicit("_icon");

		public static readonly StringName _questIcon = StringName.op_Implicit("_questIcon");

		public static readonly StringName _outline = StringName.op_Implicit("_outline");

		public static readonly StringName _circleVfx = StringName.op_Implicit("_circleVfx");

		public static readonly StringName _tween = StringName.op_Implicit("_tween");

		public static readonly StringName _pulseTween = StringName.op_Implicit("_pulseTween");

		public static readonly StringName _elapsedTime = StringName.op_Implicit("_elapsedTime");
	}

	public new class SignalName : NMapPoint.SignalName
	{
	}

	private static readonly StringName _mapColor = new StringName("map_color");

	private Control _iconContainer;

	private TextureRect _icon;

	private TextureRect _questIcon;

	private TextureRect _outline;

	private NMapCircleVfx? _circleVfx;

	private Tween? _tween;

	private Tween? _pulseTween;

	private const float _pulseSpeed = 4f;

	private const float _scaleAmount = 0.25f;

	private const float _scaleBase = 1.2f;

	private float _elapsedTime = Rng.Chaotic.NextFloat(3140f);

	protected override Color TraveledColor => Colors.White;

	protected override Color UntravelableColor => StsColors.halfTransparentWhite;

	protected override Color HoveredColor => Colors.White;

	protected override Vector2 HoverScale => Vector2.One * 1.45f;

	protected override Vector2 DownScale => Vector2.One * 0.9f;

	private static string ScenePath => SceneHelper.GetScenePath("/ui/normal_map_point");

	public static IEnumerable<string> AssetPaths => new _003C_003Ez__ReadOnlySingleElementList<string>(ScenePath);

	private static string IconName(MapPointType pointType)
	{
		return pointType switch
		{
			MapPointType.Unassigned => "map_unknown", 
			MapPointType.Monster => "map_monster", 
			MapPointType.Elite => "map_elite", 
			MapPointType.Boss => string.Empty, 
			MapPointType.Ancient => "map_unknown", 
			MapPointType.Treasure => "map_chest", 
			MapPointType.Shop => "map_shop", 
			MapPointType.RestSite => "map_rest", 
			MapPointType.Unknown => "map_unknown", 
			_ => throw new ArgumentOutOfRangeException(pointType.ToString()), 
		};
	}

	private static string IconPath(string filename)
	{
		return ImageHelper.GetImagePath("atlases/ui_atlas.sprites/map/icons/" + filename + ".tres");
	}

	private static string OutlinePath(string filename)
	{
		return ImageHelper.GetImagePath("atlases/compressed.sprites/map/" + filename + "_outline.tres");
	}

	private static string UnknownIconPath(RoomType pointType)
	{
		return ImageHelper.GetImagePath("atlases/ui_atlas.sprites/map/icons/map_" + pointType switch
		{
			RoomType.Treasure => "unknown_chest", 
			RoomType.Monster => "unknown_monster", 
			RoomType.Shop => "unknown_shop", 
			RoomType.Elite => "unknown_elite", 
			_ => "unknown", 
		} + ".tres");
	}

	private static string UnknownOutlinePath(RoomType pointType)
	{
		return OutlinePath(pointType switch
		{
			RoomType.Treasure => "map_chest", 
			RoomType.Monster => "map_monster", 
			RoomType.Shop => "map_shop", 
			RoomType.Elite => "map_elite", 
			_ => "map_unknown", 
		});
	}

	public static NNormalMapPoint Create(MapPoint point, NMapScreen screen, IRunState runState)
	{
		NNormalMapPoint nNormalMapPoint = PreloadManager.Cache.GetScene(ScenePath).Instantiate<NNormalMapPoint>((GenEditState)0);
		nNormalMapPoint.Point = point;
		nNormalMapPoint._screen = screen;
		nNormalMapPoint._runState = runState;
		return nNormalMapPoint;
	}

	public override void _Ready()
	{
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Expected O, but got Unknown
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		ConnectSignals();
		_iconContainer = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%IconContainer"));
		_icon = ((Node)this).GetNode<TextureRect>(NodePath.op_Implicit("%Icon"));
		_outline = ((Node)this).GetNode<TextureRect>(NodePath.op_Implicit("%Outline"));
		_questIcon = ((Node)this).GetNode<TextureRect>(NodePath.op_Implicit("%QuestIcon"));
		UpdateIcon();
		Color mapBgColor = _runState.Act.MapBgColor;
		((CanvasItem)_outline).Modulate = mapBgColor;
		ShaderMaterial val = (ShaderMaterial)((CanvasItem)_icon).Material;
		val.SetShaderParameter(_mapColor, Variant.op_Implicit(((Color)(ref mapBgColor)).Lerp(Colors.Gray, 0.5f)));
		RefreshMarkedIconVisibility();
		RefreshColorInstantly();
		Disable();
	}

	public override void _EnterTree()
	{
		base.Point.NodeMarkedChanged += RefreshMarkedIconVisibility;
		NMapScreen.Instance.PointTypeHighlighted += OnHighlightPointType;
	}

	private void RefreshMarkedIconVisibility()
	{
		((CanvasItem)_questIcon).Visible = base.Point.Quests.Count > 0;
	}

	public override void _ExitTree()
	{
		base.Point.NodeMarkedChanged -= RefreshMarkedIconVisibility;
		NMapScreen.Instance.PointTypeHighlighted -= OnHighlightPointType;
	}

	public override void _Process(double delta)
	{
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		if (_isEnabled)
		{
			if (!base.IsFocused && IsInputAllowed())
			{
				_elapsedTime += (float)delta * 4f;
				_iconContainer.Scale = Vector2.One * (Mathf.Sin(_elapsedTime) * 0.25f + 1.2f);
			}
			else
			{
				Control iconContainer = _iconContainer;
				Vector2 scale = _iconContainer.Scale;
				iconContainer.Scale = ((Vector2)(ref scale)).Lerp(Vector2.One, 0.5f);
			}
		}
	}

	public override void OnSelected()
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		ShowCircleVfx(playAnim: true);
		base.State = MapPointState.Traveled;
		Tween? tween = _tween;
		if (tween != null)
		{
			tween.Kill();
		}
		_tween = ((Node)this).CreateTween().SetParallel(true);
		_tween.TweenProperty((GodotObject)(object)_iconContainer, NodePath.op_Implicit("scale"), Variant.op_Implicit(Vector2.One), 0.3).SetEase((EaseType)1).SetTrans((TransitionType)7);
		_tween.TweenProperty((GodotObject)(object)_icon, NodePath.op_Implicit("scale"), Variant.op_Implicit(Vector2.One), 0.3).SetEase((EaseType)1).SetTrans((TransitionType)10);
		_tween.TweenProperty((GodotObject)(object)_questIcon, NodePath.op_Implicit("scale"), Variant.op_Implicit(Vector2.One), 0.3).SetEase((EaseType)1).SetTrans((TransitionType)10);
		_tween.TweenProperty((GodotObject)(object)_icon, NodePath.op_Implicit("self_modulate"), Variant.op_Implicit(base.TargetColor), 0.3).SetEase((EaseType)1).SetTrans((TransitionType)5);
		_tween.TweenProperty((GodotObject)(object)_outline, NodePath.op_Implicit("modulate"), Variant.op_Implicit(_runState.Act.MapBgColor), 0.3).SetEase((EaseType)1).SetTrans((TransitionType)5);
	}

	protected override void OnFocus()
	{
		base.OnFocus();
		if (IsInputAllowed())
		{
			AnimHover();
		}
	}

	protected override void OnUnfocus()
	{
		base.OnUnfocus();
		NHoverTipSet.Remove((Control)(object)this);
		if (_isEnabled)
		{
			_elapsedTime = 3.926991f;
		}
		if (IsInputAllowed())
		{
			AnimUnhover();
		}
	}

	protected override void OnPress()
	{
		if (base.IsTravelable)
		{
			AnimPressDown();
			NHoverTipSet.Remove((Control)(object)this);
		}
	}

	public void SetAngle(float degrees)
	{
		_iconContainer.RotationDegrees = degrees;
	}

	protected override void RefreshColorInstantly()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		((CanvasItem)_icon).SelfModulate = base.TargetColor;
	}

	protected override void RefreshState()
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		base.RefreshState();
		UpdateIcon();
		if (base.State == MapPointState.Traveled)
		{
			ShowCircleVfx(playAnim: false);
		}
		if (!base.IsFocused)
		{
			_iconContainer.Scale = Vector2.One;
		}
	}

	private void UpdateIcon()
	{
		if (base.Point.PointType != MapPointType.Unknown || base.State != MapPointState.Traveled)
		{
			string filename = IconName(base.Point.PointType);
			string text = IconPath(filename);
			_icon.Texture = ResourceLoader.Load<Texture2D>(text, (string)null, (CacheMode)1);
			string text2 = OutlinePath(IconName(base.Point.PointType));
			_outline.Texture = ResourceLoader.Load<Texture2D>(text2, (string)null, (CacheMode)1);
		}
		else if (_runState.MapPointHistory.Count > _runState.CurrentActIndex)
		{
			IReadOnlyList<MapPointHistoryEntry> readOnlyList = _runState.MapPointHistory[_runState.CurrentActIndex];
			if (readOnlyList.Count > base.Point.coord.row)
			{
				RoomType roomType = readOnlyList[base.Point.coord.row].Rooms.First().RoomType;
				_icon.Texture = ResourceLoader.Load<Texture2D>(UnknownIconPath(roomType), (string)null, (CacheMode)1);
				_outline.Texture = ResourceLoader.Load<Texture2D>(UnknownOutlinePath(roomType), (string)null, (CacheMode)1);
			}
		}
	}

	private void ShowCircleVfx(bool playAnim)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		if (_circleVfx == null)
		{
			_circleVfx = NMapCircleVfx.Create(playAnim);
			((Node)(object)this).AddChildSafely((Node?)(object)_circleVfx);
			NMapCircleVfx? circleVfx = _circleVfx;
			((Control)circleVfx).Position = ((Control)circleVfx).Position + ((Control)this).PivotOffset;
		}
	}

	private void AnimHover()
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		Tween? tween = _tween;
		if (tween != null)
		{
			tween.Kill();
		}
		_tween = ((Node)this).CreateTween().SetParallel(true);
		_tween.TweenProperty((GodotObject)(object)_icon, NodePath.op_Implicit("scale"), Variant.op_Implicit(HoverScale), 0.05);
		_tween.TweenProperty((GodotObject)(object)_questIcon, NodePath.op_Implicit("scale"), Variant.op_Implicit(HoverScale), 0.05);
		_tween.TweenProperty((GodotObject)(object)_icon, NodePath.op_Implicit("self_modulate"), Variant.op_Implicit(HoveredColor), 0.05);
		if (base.IsTravelable)
		{
			_tween.TweenProperty((GodotObject)(object)_outline, NodePath.op_Implicit("modulate"), Variant.op_Implicit(_outlineColor), 0.05);
		}
	}

	private void AnimUnhover()
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		Tween? tween = _tween;
		if (tween != null)
		{
			tween.Kill();
		}
		_tween = ((Node)this).CreateTween().SetParallel(true);
		_tween.TweenProperty((GodotObject)(object)_icon, NodePath.op_Implicit("scale"), Variant.op_Implicit(Vector2.One), 0.5).SetEase((EaseType)1).SetTrans((TransitionType)7);
		_tween.TweenProperty((GodotObject)(object)_questIcon, NodePath.op_Implicit("scale"), Variant.op_Implicit(Vector2.One), 0.5).SetEase((EaseType)1).SetTrans((TransitionType)7);
		_tween.TweenProperty((GodotObject)(object)_icon, NodePath.op_Implicit("self_modulate"), Variant.op_Implicit(base.TargetColor), 0.5).SetEase((EaseType)1).SetTrans((TransitionType)7);
		_tween.TweenProperty((GodotObject)(object)_outline, NodePath.op_Implicit("modulate"), Variant.op_Implicit(_runState.Act.MapBgColor), 0.5).SetEase((EaseType)1).SetTrans((TransitionType)7);
	}

	private void AnimPressDown()
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		Tween? tween = _tween;
		if (tween != null)
		{
			tween.Kill();
		}
		_tween = ((Node)this).CreateTween().SetParallel(true);
		_tween.TweenProperty((GodotObject)(object)_icon, NodePath.op_Implicit("scale"), Variant.op_Implicit(DownScale), 0.3).SetEase((EaseType)1).SetTrans((TransitionType)7);
		_tween.TweenProperty((GodotObject)(object)_questIcon, NodePath.op_Implicit("scale"), Variant.op_Implicit(DownScale), 0.3).SetEase((EaseType)1).SetTrans((TransitionType)7);
		_tween.TweenProperty((GodotObject)(object)_outline, NodePath.op_Implicit("modulate"), Variant.op_Implicit(_runState.Act.MapBgColor), 0.3).SetEase((EaseType)1).SetTrans((TransitionType)5);
	}

	private void OnHighlightPointType(MapPointType pointType)
	{
		if (pointType == base.Point.PointType)
		{
			AnimHover();
		}
		else
		{
			AnimUnhover();
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_0289: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_030c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0315: Unknown result type (might be due to invalid IL or missing references)
		//IL_033b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0344: Unknown result type (might be due to invalid IL or missing references)
		//IL_036a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0373: Unknown result type (might be due to invalid IL or missing references)
		//IL_0399: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_041c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0425: Unknown result type (might be due to invalid IL or missing references)
		//IL_044b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0454: Unknown result type (might be due to invalid IL or missing references)
		//IL_047a: Unknown result type (might be due to invalid IL or missing references)
		//IL_049d: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0506: Unknown result type (might be due to invalid IL or missing references)
		//IL_052c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0535: Unknown result type (might be due to invalid IL or missing references)
		//IL_055b: Unknown result type (might be due to invalid IL or missing references)
		//IL_057e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0589: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(23);
		list.Add(new MethodInfo(MethodName.IconName, new PropertyInfo((Type)4, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)33, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("pointType"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.IconPath, new PropertyInfo((Type)4, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)33, new List<PropertyInfo>
		{
			new PropertyInfo((Type)4, StringName.op_Implicit("filename"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OutlinePath, new PropertyInfo((Type)4, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)33, new List<PropertyInfo>
		{
			new PropertyInfo((Type)4, StringName.op_Implicit("filename"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UnknownIconPath, new PropertyInfo((Type)4, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)33, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("pointType"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UnknownOutlinePath, new PropertyInfo((Type)4, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)33, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("pointType"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._EnterTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.RefreshMarkedIconVisibility, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._ExitTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Process, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)3, StringName.op_Implicit("delta"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnSelected, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnFocus, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnUnfocus, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnPress, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetAngle, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)3, StringName.op_Implicit("degrees"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.RefreshColorInstantly, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.RefreshState, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UpdateIcon, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.ShowCircleVfx, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)1, StringName.op_Implicit("playAnim"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AnimHover, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AnimUnhover, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AnimPressDown, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnHighlightPointType, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("pointType"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0271: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0313: Unknown result type (might be due to invalid IL or missing references)
		//IL_0346: Unknown result type (might be due to invalid IL or missing references)
		//IL_036b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0390: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e8: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.IconName && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			string text = IconName(VariantUtils.ConvertTo<MapPointType>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<string>(ref text);
			return true;
		}
		if ((ref method) == MethodName.IconPath && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			string text2 = IconPath(VariantUtils.ConvertTo<string>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<string>(ref text2);
			return true;
		}
		if ((ref method) == MethodName.OutlinePath && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			string text3 = OutlinePath(VariantUtils.ConvertTo<string>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<string>(ref text3);
			return true;
		}
		if ((ref method) == MethodName.UnknownIconPath && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			string text4 = UnknownIconPath(VariantUtils.ConvertTo<RoomType>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<string>(ref text4);
			return true;
		}
		if ((ref method) == MethodName.UnknownOutlinePath && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			string text5 = UnknownOutlinePath(VariantUtils.ConvertTo<RoomType>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<string>(ref text5);
			return true;
		}
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
		if ((ref method) == MethodName.RefreshMarkedIconVisibility && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			RefreshMarkedIconVisibility();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._ExitTree && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._ExitTree();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._Process && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			((Node)this)._Process(VariantUtils.ConvertTo<double>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnSelected && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnSelected();
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
		if ((ref method) == MethodName.OnPress && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnPress();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetAngle && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			SetAngle(VariantUtils.ConvertTo<float>(ref ((NativeVariantPtrArgs)(ref args))[0]));
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
		if ((ref method) == MethodName.UpdateIcon && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			UpdateIcon();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.ShowCircleVfx && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			ShowCircleVfx(VariantUtils.ConvertTo<bool>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.AnimHover && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			AnimHover();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.AnimUnhover && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			AnimUnhover();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.AnimPressDown && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			AnimPressDown();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnHighlightPointType && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OnHighlightPointType(VariantUtils.ConvertTo<MapPointType>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		return base.InvokeGodotClassMethod(in method, args, out ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static bool InvokeGodotClassStaticMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.IconName && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			string text = IconName(VariantUtils.ConvertTo<MapPointType>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<string>(ref text);
			return true;
		}
		if ((ref method) == MethodName.IconPath && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			string text2 = IconPath(VariantUtils.ConvertTo<string>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<string>(ref text2);
			return true;
		}
		if ((ref method) == MethodName.OutlinePath && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			string text3 = OutlinePath(VariantUtils.ConvertTo<string>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<string>(ref text3);
			return true;
		}
		if ((ref method) == MethodName.UnknownIconPath && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			string text4 = UnknownIconPath(VariantUtils.ConvertTo<RoomType>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<string>(ref text4);
			return true;
		}
		if ((ref method) == MethodName.UnknownOutlinePath && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			string text5 = UnknownOutlinePath(VariantUtils.ConvertTo<RoomType>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<string>(ref text5);
			return true;
		}
		ret = default(godot_variant);
		return false;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if ((ref method) == MethodName.IconName)
		{
			return true;
		}
		if ((ref method) == MethodName.IconPath)
		{
			return true;
		}
		if ((ref method) == MethodName.OutlinePath)
		{
			return true;
		}
		if ((ref method) == MethodName.UnknownIconPath)
		{
			return true;
		}
		if ((ref method) == MethodName.UnknownOutlinePath)
		{
			return true;
		}
		if ((ref method) == MethodName._Ready)
		{
			return true;
		}
		if ((ref method) == MethodName._EnterTree)
		{
			return true;
		}
		if ((ref method) == MethodName.RefreshMarkedIconVisibility)
		{
			return true;
		}
		if ((ref method) == MethodName._ExitTree)
		{
			return true;
		}
		if ((ref method) == MethodName._Process)
		{
			return true;
		}
		if ((ref method) == MethodName.OnSelected)
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
		if ((ref method) == MethodName.OnPress)
		{
			return true;
		}
		if ((ref method) == MethodName.SetAngle)
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
		if ((ref method) == MethodName.UpdateIcon)
		{
			return true;
		}
		if ((ref method) == MethodName.ShowCircleVfx)
		{
			return true;
		}
		if ((ref method) == MethodName.AnimHover)
		{
			return true;
		}
		if ((ref method) == MethodName.AnimUnhover)
		{
			return true;
		}
		if ((ref method) == MethodName.AnimPressDown)
		{
			return true;
		}
		if ((ref method) == MethodName.OnHighlightPointType)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._iconContainer)
		{
			_iconContainer = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._icon)
		{
			_icon = VariantUtils.ConvertTo<TextureRect>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._questIcon)
		{
			_questIcon = VariantUtils.ConvertTo<TextureRect>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._outline)
		{
			_outline = VariantUtils.ConvertTo<TextureRect>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._circleVfx)
		{
			_circleVfx = VariantUtils.ConvertTo<NMapCircleVfx>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._tween)
		{
			_tween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._pulseTween)
		{
			_pulseTween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._elapsedTime)
		{
			_elapsedTime = VariantUtils.ConvertTo<float>(ref value);
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
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
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
		if ((ref name) == PropertyName._iconContainer)
		{
			value = VariantUtils.CreateFrom<Control>(ref _iconContainer);
			return true;
		}
		if ((ref name) == PropertyName._icon)
		{
			value = VariantUtils.CreateFrom<TextureRect>(ref _icon);
			return true;
		}
		if ((ref name) == PropertyName._questIcon)
		{
			value = VariantUtils.CreateFrom<TextureRect>(ref _questIcon);
			return true;
		}
		if ((ref name) == PropertyName._outline)
		{
			value = VariantUtils.CreateFrom<TextureRect>(ref _outline);
			return true;
		}
		if ((ref name) == PropertyName._circleVfx)
		{
			value = VariantUtils.CreateFrom<NMapCircleVfx>(ref _circleVfx);
			return true;
		}
		if ((ref name) == PropertyName._tween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _tween);
			return true;
		}
		if ((ref name) == PropertyName._pulseTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _pulseTween);
			return true;
		}
		if ((ref name) == PropertyName._elapsedTime)
		{
			value = VariantUtils.CreateFrom<float>(ref _elapsedTime);
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
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._iconContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._icon, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._questIcon, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._outline, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._circleVfx, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._tween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)20, PropertyName.TraveledColor, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)20, PropertyName.UntravelableColor, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)20, PropertyName.HoveredColor, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)5, PropertyName.HoverScale, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)5, PropertyName.DownScale, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._pulseTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)3, PropertyName._elapsedTime, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName._iconContainer, Variant.From<Control>(ref _iconContainer));
		info.AddProperty(PropertyName._icon, Variant.From<TextureRect>(ref _icon));
		info.AddProperty(PropertyName._questIcon, Variant.From<TextureRect>(ref _questIcon));
		info.AddProperty(PropertyName._outline, Variant.From<TextureRect>(ref _outline));
		info.AddProperty(PropertyName._circleVfx, Variant.From<NMapCircleVfx>(ref _circleVfx));
		info.AddProperty(PropertyName._tween, Variant.From<Tween>(ref _tween));
		info.AddProperty(PropertyName._pulseTween, Variant.From<Tween>(ref _pulseTween));
		info.AddProperty(PropertyName._elapsedTime, Variant.From<float>(ref _elapsedTime));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._iconContainer, ref val))
		{
			_iconContainer = ((Variant)(ref val)).As<Control>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._icon, ref val2))
		{
			_icon = ((Variant)(ref val2)).As<TextureRect>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._questIcon, ref val3))
		{
			_questIcon = ((Variant)(ref val3)).As<TextureRect>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._outline, ref val4))
		{
			_outline = ((Variant)(ref val4)).As<TextureRect>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._circleVfx, ref val5))
		{
			_circleVfx = ((Variant)(ref val5)).As<NMapCircleVfx>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._tween, ref val6))
		{
			_tween = ((Variant)(ref val6)).As<Tween>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName._pulseTween, ref val7))
		{
			_pulseTween = ((Variant)(ref val7)).As<Tween>();
		}
		Variant val8 = default(Variant);
		if (info.TryGetProperty(PropertyName._elapsedTime, ref val8))
		{
			_elapsedTime = ((Variant)(ref val8)).As<float>();
		}
	}
}
