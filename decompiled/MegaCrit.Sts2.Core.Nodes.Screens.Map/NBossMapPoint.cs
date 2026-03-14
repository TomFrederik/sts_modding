using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Map;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;

namespace MegaCrit.Sts2.Core.Nodes.Screens.Map;

[ScriptPath("res://src/Core/Nodes/Screens/Map/NBossMapPoint.cs")]
public class NBossMapPoint : NMapPoint
{
	public new class MethodName : NMapPoint.MethodName
	{
		public new static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public new static readonly StringName OnFocus = StringName.op_Implicit("OnFocus");

		public new static readonly StringName OnUnfocus = StringName.op_Implicit("OnUnfocus");

		public new static readonly StringName OnPress = StringName.op_Implicit("OnPress");

		public new static readonly StringName OnSelected = StringName.op_Implicit("OnSelected");

		public new static readonly StringName RefreshColorInstantly = StringName.op_Implicit("RefreshColorInstantly");
	}

	public new class PropertyName : NMapPoint.PropertyName
	{
		public new static readonly StringName TraveledColor = StringName.op_Implicit("TraveledColor");

		public new static readonly StringName UntravelableColor = StringName.op_Implicit("UntravelableColor");

		public new static readonly StringName HoveredColor = StringName.op_Implicit("HoveredColor");

		public new static readonly StringName HoverScale = StringName.op_Implicit("HoverScale");

		public new static readonly StringName DownScale = StringName.op_Implicit("DownScale");

		public static readonly StringName _hoverTween = StringName.op_Implicit("_hoverTween");

		public static readonly StringName _usesSpine = StringName.op_Implicit("_usesSpine");

		public static readonly StringName _spriteContainer = StringName.op_Implicit("_spriteContainer");

		public static readonly StringName _spineSprite = StringName.op_Implicit("_spineSprite");

		public static readonly StringName _material = StringName.op_Implicit("_material");

		public static readonly StringName _placeholderImage = StringName.op_Implicit("_placeholderImage");

		public static readonly StringName _placeholderOutline = StringName.op_Implicit("_placeholderOutline");
	}

	public new class SignalName : NMapPoint.SignalName
	{
	}

	private static readonly StringName _mapColor = new StringName("map_color");

	private static readonly StringName _blackLayerColor = new StringName("black_layer_color");

	private Tween? _hoverTween;

	private ActModel _act;

	private bool _usesSpine;

	private Node2D _spriteContainer;

	private Node2D _spineSprite;

	private MegaSprite _animController;

	private ShaderMaterial _material;

	private TextureRect _placeholderImage;

	private TextureRect _placeholderOutline;

	protected override Color TraveledColor => StsColors.pathDotTraveled;

	protected override Color UntravelableColor => StsColors.red;

	protected override Color HoveredColor => StsColors.pathDotTraveled;

	protected override Vector2 HoverScale => Vector2.One * 1.05f;

	protected override Vector2 DownScale => Vector2.One * 1.02f;

	private static string BossMapPointPath => SceneHelper.GetScenePath("ui/boss_map_point");

	public static IEnumerable<string> AssetPaths => new _003C_003Ez__ReadOnlySingleElementList<string>(BossMapPointPath);

	public static NBossMapPoint Create(MapPoint point, NMapScreen screen, IRunState runState)
	{
		NBossMapPoint nBossMapPoint = PreloadManager.Cache.GetScene(BossMapPointPath).Instantiate<NBossMapPoint>((GenEditState)0);
		nBossMapPoint.Point = point;
		nBossMapPoint._screen = screen;
		nBossMapPoint._runState = runState;
		nBossMapPoint._act = runState.Act;
		return nBossMapPoint;
	}

	public override void _Ready()
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Expected O, but got Unknown
		ConnectSignals();
		Disable();
		_spriteContainer = ((Node)this).GetNode<Node2D>(NodePath.op_Implicit("%SpriteContainer"));
		_spineSprite = ((Node)this).GetNode<Node2D>(NodePath.op_Implicit("%SpineSprite"));
		_animController = new MegaSprite(Variant.op_Implicit((GodotObject)(object)_spineSprite));
		EncounterModel encounterModel = ((base.Point == _runState.Map.SecondBossMapPoint) ? _runState.Act.SecondBossEncounter : _runState.Act.BossEncounter);
		if (encounterModel.BossNodeSpineResource != null)
		{
			_usesSpine = true;
			((CanvasItem)_spineSprite).Visible = true;
			_animController.SetSkeletonDataRes(encounterModel.BossNodeSpineResource);
			_animController.GetAnimationState().AddAnimation("animation");
			_material = (ShaderMaterial)_animController.GetNormalMaterial();
		}
		else
		{
			_usesSpine = false;
			((CanvasItem)_spineSprite).Visible = false;
			_placeholderImage = ((Node)this).GetNode<TextureRect>(NodePath.op_Implicit("%PlaceholderImage"));
			_placeholderOutline = ((Node)this).GetNode<TextureRect>(NodePath.op_Implicit("%PlaceholderOutline"));
			((CanvasItem)_placeholderImage).Visible = true;
			_placeholderImage.Texture = PreloadManager.Cache.GetAsset<Texture2D>(encounterModel.BossNodePath + ".png");
			_placeholderOutline.Texture = PreloadManager.Cache.GetAsset<Texture2D>(encounterModel.BossNodePath + "_outline.png");
			((CanvasItem)_placeholderImage).SelfModulate = _act.MapTraveledColor;
			((CanvasItem)_placeholderOutline).SelfModulate = _act.MapBgColor;
		}
		RefreshColorInstantly();
	}

	protected override void OnFocus()
	{
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		base.OnFocus();
		if (IsInputAllowed() && base.IsTravelable)
		{
			Tween? hoverTween = _hoverTween;
			if (hoverTween != null)
			{
				hoverTween.Kill();
			}
			_hoverTween = ((Node)this).CreateTween().SetParallel(true);
			_hoverTween.TweenProperty((GodotObject)(object)_spriteContainer, NodePath.op_Implicit("scale"), Variant.op_Implicit(HoverScale), 0.05);
			_ = _usesSpine;
		}
	}

	protected override void OnUnfocus()
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		base.OnUnfocus();
		if (IsInputAllowed())
		{
			Tween? hoverTween = _hoverTween;
			if (hoverTween != null)
			{
				hoverTween.Kill();
			}
			_hoverTween = ((Node)this).CreateTween().SetParallel(true);
			_hoverTween.TweenProperty((GodotObject)(object)_spriteContainer, NodePath.op_Implicit("scale"), Variant.op_Implicit(Vector2.One), 0.5).SetEase((EaseType)1).SetTrans((TransitionType)7);
			_ = _usesSpine;
		}
	}

	protected override void OnPress()
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		if (base.IsTravelable)
		{
			Tween? hoverTween = _hoverTween;
			if (hoverTween != null)
			{
				hoverTween.Kill();
			}
			_hoverTween = ((Node)this).CreateTween().SetParallel(true);
			_hoverTween.TweenProperty((GodotObject)(object)_spriteContainer, NodePath.op_Implicit("scale"), Variant.op_Implicit(DownScale), 0.3).SetEase((EaseType)1).SetTrans((TransitionType)5);
		}
	}

	public override void OnSelected()
	{
		base.State = MapPointState.Traveled;
	}

	protected override void RefreshColorInstantly()
	{
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		if (_usesSpine)
		{
			MapPointState state = base.State;
			if ((uint)(state - 1) <= 1u)
			{
				_material.SetShaderParameter(_blackLayerColor, Variant.op_Implicit(_act.MapTraveledColor));
			}
			else
			{
				_material.SetShaderParameter(_blackLayerColor, Variant.op_Implicit(_act.MapUntraveledColor));
			}
			_material.SetShaderParameter(_mapColor, Variant.op_Implicit(_act.MapBgColor));
		}
		else
		{
			MapPointState state = base.State;
			bool flag = (uint)(state - 1) <= 1u;
			Color selfModulate = (flag ? _act.MapTraveledColor : _act.MapUntraveledColor);
			((CanvasItem)_placeholderImage).SelfModulate = selfModulate;
			((CanvasItem)_placeholderOutline).SelfModulate = _act.MapBgColor;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(6);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnFocus, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnUnfocus, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnPress, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnSelected, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.RefreshColorInstantly, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
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
		if ((ref method) == MethodName.OnSelected && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnSelected();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.RefreshColorInstantly && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			RefreshColorInstantly();
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
		if ((ref method) == MethodName.OnSelected)
		{
			return true;
		}
		if ((ref method) == MethodName.RefreshColorInstantly)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._hoverTween)
		{
			_hoverTween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._usesSpine)
		{
			_usesSpine = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._spriteContainer)
		{
			_spriteContainer = VariantUtils.ConvertTo<Node2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._spineSprite)
		{
			_spineSprite = VariantUtils.ConvertTo<Node2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._material)
		{
			_material = VariantUtils.ConvertTo<ShaderMaterial>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._placeholderImage)
		{
			_placeholderImage = VariantUtils.ConvertTo<TextureRect>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._placeholderOutline)
		{
			_placeholderOutline = VariantUtils.ConvertTo<TextureRect>(ref value);
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
		if ((ref name) == PropertyName._hoverTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _hoverTween);
			return true;
		}
		if ((ref name) == PropertyName._usesSpine)
		{
			value = VariantUtils.CreateFrom<bool>(ref _usesSpine);
			return true;
		}
		if ((ref name) == PropertyName._spriteContainer)
		{
			value = VariantUtils.CreateFrom<Node2D>(ref _spriteContainer);
			return true;
		}
		if ((ref name) == PropertyName._spineSprite)
		{
			value = VariantUtils.CreateFrom<Node2D>(ref _spineSprite);
			return true;
		}
		if ((ref name) == PropertyName._material)
		{
			value = VariantUtils.CreateFrom<ShaderMaterial>(ref _material);
			return true;
		}
		if ((ref name) == PropertyName._placeholderImage)
		{
			value = VariantUtils.CreateFrom<TextureRect>(ref _placeholderImage);
			return true;
		}
		if ((ref name) == PropertyName._placeholderOutline)
		{
			value = VariantUtils.CreateFrom<TextureRect>(ref _placeholderOutline);
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
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)20, PropertyName.TraveledColor, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)20, PropertyName.UntravelableColor, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)20, PropertyName.HoveredColor, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)5, PropertyName.HoverScale, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)5, PropertyName.DownScale, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._hoverTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._usesSpine, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._spriteContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._spineSprite, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._material, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._placeholderImage, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._placeholderOutline, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		info.AddProperty(PropertyName._hoverTween, Variant.From<Tween>(ref _hoverTween));
		info.AddProperty(PropertyName._usesSpine, Variant.From<bool>(ref _usesSpine));
		info.AddProperty(PropertyName._spriteContainer, Variant.From<Node2D>(ref _spriteContainer));
		info.AddProperty(PropertyName._spineSprite, Variant.From<Node2D>(ref _spineSprite));
		info.AddProperty(PropertyName._material, Variant.From<ShaderMaterial>(ref _material));
		info.AddProperty(PropertyName._placeholderImage, Variant.From<TextureRect>(ref _placeholderImage));
		info.AddProperty(PropertyName._placeholderOutline, Variant.From<TextureRect>(ref _placeholderOutline));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._hoverTween, ref val))
		{
			_hoverTween = ((Variant)(ref val)).As<Tween>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._usesSpine, ref val2))
		{
			_usesSpine = ((Variant)(ref val2)).As<bool>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._spriteContainer, ref val3))
		{
			_spriteContainer = ((Variant)(ref val3)).As<Node2D>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._spineSprite, ref val4))
		{
			_spineSprite = ((Variant)(ref val4)).As<Node2D>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._material, ref val5))
		{
			_material = ((Variant)(ref val5)).As<ShaderMaterial>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._placeholderImage, ref val6))
		{
			_placeholderImage = ((Variant)(ref val6)).As<TextureRect>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName._placeholderOutline, ref val7))
		{
			_placeholderOutline = ((Variant)(ref val7)).As<TextureRect>();
		}
	}
}
