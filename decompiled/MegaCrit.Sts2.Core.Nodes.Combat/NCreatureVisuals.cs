using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;

namespace MegaCrit.Sts2.Core.Nodes.Combat;

[ScriptPath("res://src/Core/Nodes/Combat/NCreatureVisuals.cs")]
public class NCreatureVisuals : Node2D
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName SetScaleAndHue = StringName.op_Implicit("SetScaleAndHue");

		public static readonly StringName IsPlayingHurtAnimation = StringName.op_Implicit("IsPlayingHurtAnimation");

		public static readonly StringName TryApplyLiquidOverlay = StringName.op_Implicit("TryApplyLiquidOverlay");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName Body = StringName.op_Implicit("Body");

		public static readonly StringName Bounds = StringName.op_Implicit("Bounds");

		public static readonly StringName IntentPosition = StringName.op_Implicit("IntentPosition");

		public static readonly StringName OrbPosition = StringName.op_Implicit("OrbPosition");

		public static readonly StringName TalkPosition = StringName.op_Implicit("TalkPosition");

		public static readonly StringName HasSpineAnimation = StringName.op_Implicit("HasSpineAnimation");

		public static readonly StringName VfxSpawnPosition = StringName.op_Implicit("VfxSpawnPosition");

		public static readonly StringName DefaultScale = StringName.op_Implicit("DefaultScale");

		public static readonly StringName _hue = StringName.op_Implicit("_hue");

		public static readonly StringName _liquidOverlayTimer = StringName.op_Implicit("_liquidOverlayTimer");

		public static readonly StringName _savedNormalMaterial = StringName.op_Implicit("_savedNormalMaterial");

		public static readonly StringName _currentLiquidOverlayMaterial = StringName.op_Implicit("_currentLiquidOverlayMaterial");
	}

	public class SignalName : SignalName
	{
	}

	private static readonly StringName _overlayInfluence = new StringName("overlay_influence");

	private static readonly StringName _h = new StringName("h");

	private static readonly StringName _tint = new StringName("tint");

	private const double _baseLiquidOverlayDuration = 1.0;

	private float _hue = 1f;

	private double _liquidOverlayTimer;

	private Material? _savedNormalMaterial;

	private ShaderMaterial? _currentLiquidOverlayMaterial;

	public Node2D Body { get; private set; }

	public Control Bounds { get; private set; }

	public Marker2D IntentPosition { get; private set; }

	public Marker2D OrbPosition { get; private set; }

	public Marker2D? TalkPosition { get; private set; }

	public bool HasSpineAnimation
	{
		get
		{
			if (GodotObject.IsInstanceValid((GodotObject)(object)Body))
			{
				return ((GodotObject)Body).GetClass() == "SpineSprite";
			}
			return false;
		}
	}

	public MegaSprite? SpineBody { get; private set; }

	public Marker2D VfxSpawnPosition { get; private set; }

	public float DefaultScale { get; set; } = 1f;

	public override void _Ready()
	{
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		Body = ((Node)this).GetNode<Node2D>(NodePath.op_Implicit("%Visuals"));
		Bounds = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%Bounds"));
		IntentPosition = ((Node)this).GetNode<Marker2D>(NodePath.op_Implicit("%IntentPos"));
		VfxSpawnPosition = ((Node)this).GetNode<Marker2D>(NodePath.op_Implicit("%CenterPos"));
		OrbPosition = (Marker2D)(((Node)this).HasNode(NodePath.op_Implicit("%OrbPos")) ? ((object)((Node)this).GetNode<Marker2D>(NodePath.op_Implicit("%OrbPos"))) : ((object)IntentPosition));
		TalkPosition = (((Node)this).HasNode(NodePath.op_Implicit("%TalkPos")) ? ((Node)this).GetNode<Marker2D>(NodePath.op_Implicit("%TalkPos")) : null);
		if (HasSpineAnimation)
		{
			SpineBody = new MegaSprite(Variant.op_Implicit((GodotObject)(object)Body));
		}
		_savedNormalMaterial = null;
		_currentLiquidOverlayMaterial = null;
	}

	public void SetUpSkin(MonsterModel model)
	{
		if (SpineBody?.GetSkeleton() != null)
		{
			model.SetupSkins(this);
		}
	}

	public void SetScaleAndHue(float scale, float hue)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Expected O, but got Unknown
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Expected O, but got Unknown
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Expected O, but got Unknown
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		DefaultScale = scale;
		((Node2D)this).Scale = Vector2.One * scale;
		_hue = hue;
		if (!Mathf.IsEqualApprox(hue, 0f) && SpineBody != null)
		{
			Material normalMaterial = SpineBody.GetNormalMaterial();
			ShaderMaterial val2;
			if (normalMaterial == null)
			{
				Material val = (Material)(ShaderMaterial)PreloadManager.Cache.GetMaterial("res://materials/vfx/hsv.tres");
				val2 = (ShaderMaterial)((Resource)val).Duplicate(false);
				SpineBody.SetNormalMaterial((Material)(object)val2);
			}
			else
			{
				val2 = (ShaderMaterial)normalMaterial;
			}
			val2.SetShaderParameter(_h, Variant.op_Implicit(hue));
		}
	}

	public bool IsPlayingHurtAnimation()
	{
		if (SpineBody?.GetSkeleton() != null)
		{
			return SpineBody.GetAnimationState().GetCurrent(0).GetAnimation()
				.GetName()
				.Equals("hurt");
		}
		return false;
	}

	public void TryApplyLiquidOverlay(Color tint)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		if (_currentLiquidOverlayMaterial != null)
		{
			_currentLiquidOverlayMaterial.SetShaderParameter(_tint, Variant.op_Implicit(tint));
			_liquidOverlayTimer = 1.0;
		}
		else
		{
			TaskHelper.RunSafely(ApplyLiquidOverlayInternal(tint));
		}
	}

	private async Task ApplyLiquidOverlayInternal(Color tint)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		if (SpineBody != null)
		{
			_savedNormalMaterial = SpineBody.GetNormalMaterial();
			Material val = (Material)(ShaderMaterial)PreloadManager.Cache.GetMaterial("res://materials/vfx/potion/potion_liquid_overlay.tres");
			_currentLiquidOverlayMaterial = (ShaderMaterial)((Resource)val).Duplicate(false);
			_currentLiquidOverlayMaterial.SetShaderParameter(_tint, Variant.op_Implicit(tint));
			_currentLiquidOverlayMaterial.SetShaderParameter(_h, Variant.op_Implicit(_hue));
			_currentLiquidOverlayMaterial.SetShaderParameter(_overlayInfluence, Variant.op_Implicit(1f));
			SpineBody.SetNormalMaterial((Material)(object)_currentLiquidOverlayMaterial);
			_liquidOverlayTimer = 1.0;
			while (_liquidOverlayTimer > 0.0)
			{
				double num = (1.0 - _liquidOverlayTimer) / 1.0;
				_currentLiquidOverlayMaterial.SetShaderParameter(_overlayInfluence, Variant.op_Implicit(1.0 - num));
				_liquidOverlayTimer -= ((Node)this).GetProcessDeltaTime();
				await ((GodotObject)this).ToSignal((GodotObject)(object)((Node)this).GetTree(), SignalName.ProcessFrame);
			}
			SpineBody.SetNormalMaterial(_savedNormalMaterial);
			_currentLiquidOverlayMaterial = null;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(4);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetScaleAndHue, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)3, StringName.op_Implicit("scale"), (PropertyHint)0, "", (PropertyUsageFlags)6, false),
			new PropertyInfo((Type)3, StringName.op_Implicit("hue"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.IsPlayingHurtAnimation, new PropertyInfo((Type)1, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.TryApplyLiquidOverlay, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)20, StringName.op_Implicit("tint"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetScaleAndHue && ((NativeVariantPtrArgs)(ref args)).Count == 2)
		{
			SetScaleAndHue(VariantUtils.ConvertTo<float>(ref ((NativeVariantPtrArgs)(ref args))[0]), VariantUtils.ConvertTo<float>(ref ((NativeVariantPtrArgs)(ref args))[1]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.IsPlayingHurtAnimation && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			bool flag = IsPlayingHurtAnimation();
			ret = VariantUtils.CreateFrom<bool>(ref flag);
			return true;
		}
		if ((ref method) == MethodName.TryApplyLiquidOverlay && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			TryApplyLiquidOverlay(VariantUtils.ConvertTo<Color>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		return ((Node2D)this).InvokeGodotClassMethod(ref method, args, ref ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if ((ref method) == MethodName._Ready)
		{
			return true;
		}
		if ((ref method) == MethodName.SetScaleAndHue)
		{
			return true;
		}
		if ((ref method) == MethodName.IsPlayingHurtAnimation)
		{
			return true;
		}
		if ((ref method) == MethodName.TryApplyLiquidOverlay)
		{
			return true;
		}
		return ((Node2D)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName.Body)
		{
			Body = VariantUtils.ConvertTo<Node2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName.Bounds)
		{
			Bounds = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName.IntentPosition)
		{
			IntentPosition = VariantUtils.ConvertTo<Marker2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName.OrbPosition)
		{
			OrbPosition = VariantUtils.ConvertTo<Marker2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName.TalkPosition)
		{
			TalkPosition = VariantUtils.ConvertTo<Marker2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName.VfxSpawnPosition)
		{
			VfxSpawnPosition = VariantUtils.ConvertTo<Marker2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName.DefaultScale)
		{
			DefaultScale = VariantUtils.ConvertTo<float>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._hue)
		{
			_hue = VariantUtils.ConvertTo<float>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._liquidOverlayTimer)
		{
			_liquidOverlayTimer = VariantUtils.ConvertTo<double>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._savedNormalMaterial)
		{
			_savedNormalMaterial = VariantUtils.ConvertTo<Material>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._currentLiquidOverlayMaterial)
		{
			_currentLiquidOverlayMaterial = VariantUtils.ConvertTo<ShaderMaterial>(ref value);
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
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName.Body)
		{
			Node2D body = Body;
			value = VariantUtils.CreateFrom<Node2D>(ref body);
			return true;
		}
		if ((ref name) == PropertyName.Bounds)
		{
			Control bounds = Bounds;
			value = VariantUtils.CreateFrom<Control>(ref bounds);
			return true;
		}
		if ((ref name) == PropertyName.IntentPosition)
		{
			Marker2D intentPosition = IntentPosition;
			value = VariantUtils.CreateFrom<Marker2D>(ref intentPosition);
			return true;
		}
		if ((ref name) == PropertyName.OrbPosition)
		{
			Marker2D intentPosition = OrbPosition;
			value = VariantUtils.CreateFrom<Marker2D>(ref intentPosition);
			return true;
		}
		if ((ref name) == PropertyName.TalkPosition)
		{
			Marker2D intentPosition = TalkPosition;
			value = VariantUtils.CreateFrom<Marker2D>(ref intentPosition);
			return true;
		}
		if ((ref name) == PropertyName.HasSpineAnimation)
		{
			bool hasSpineAnimation = HasSpineAnimation;
			value = VariantUtils.CreateFrom<bool>(ref hasSpineAnimation);
			return true;
		}
		if ((ref name) == PropertyName.VfxSpawnPosition)
		{
			Marker2D intentPosition = VfxSpawnPosition;
			value = VariantUtils.CreateFrom<Marker2D>(ref intentPosition);
			return true;
		}
		if ((ref name) == PropertyName.DefaultScale)
		{
			float defaultScale = DefaultScale;
			value = VariantUtils.CreateFrom<float>(ref defaultScale);
			return true;
		}
		if ((ref name) == PropertyName._hue)
		{
			value = VariantUtils.CreateFrom<float>(ref _hue);
			return true;
		}
		if ((ref name) == PropertyName._liquidOverlayTimer)
		{
			value = VariantUtils.CreateFrom<double>(ref _liquidOverlayTimer);
			return true;
		}
		if ((ref name) == PropertyName._savedNormalMaterial)
		{
			value = VariantUtils.CreateFrom<Material>(ref _savedNormalMaterial);
			return true;
		}
		if ((ref name) == PropertyName._currentLiquidOverlayMaterial)
		{
			value = VariantUtils.CreateFrom<ShaderMaterial>(ref _currentLiquidOverlayMaterial);
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
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName.Body, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.Bounds, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.IntentPosition, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.OrbPosition, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.TalkPosition, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName.HasSpineAnimation, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.VfxSpawnPosition, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)3, PropertyName.DefaultScale, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)3, PropertyName._hue, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)3, PropertyName._liquidOverlayTimer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._savedNormalMaterial, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._currentLiquidOverlayMaterial, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		StringName body = PropertyName.Body;
		Node2D body2 = Body;
		info.AddProperty(body, Variant.From<Node2D>(ref body2));
		StringName bounds = PropertyName.Bounds;
		Control bounds2 = Bounds;
		info.AddProperty(bounds, Variant.From<Control>(ref bounds2));
		StringName intentPosition = PropertyName.IntentPosition;
		Marker2D intentPosition2 = IntentPosition;
		info.AddProperty(intentPosition, Variant.From<Marker2D>(ref intentPosition2));
		StringName orbPosition = PropertyName.OrbPosition;
		intentPosition2 = OrbPosition;
		info.AddProperty(orbPosition, Variant.From<Marker2D>(ref intentPosition2));
		StringName talkPosition = PropertyName.TalkPosition;
		intentPosition2 = TalkPosition;
		info.AddProperty(talkPosition, Variant.From<Marker2D>(ref intentPosition2));
		StringName vfxSpawnPosition = PropertyName.VfxSpawnPosition;
		intentPosition2 = VfxSpawnPosition;
		info.AddProperty(vfxSpawnPosition, Variant.From<Marker2D>(ref intentPosition2));
		StringName defaultScale = PropertyName.DefaultScale;
		float defaultScale2 = DefaultScale;
		info.AddProperty(defaultScale, Variant.From<float>(ref defaultScale2));
		info.AddProperty(PropertyName._hue, Variant.From<float>(ref _hue));
		info.AddProperty(PropertyName._liquidOverlayTimer, Variant.From<double>(ref _liquidOverlayTimer));
		info.AddProperty(PropertyName._savedNormalMaterial, Variant.From<Material>(ref _savedNormalMaterial));
		info.AddProperty(PropertyName._currentLiquidOverlayMaterial, Variant.From<ShaderMaterial>(ref _currentLiquidOverlayMaterial));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName.Body, ref val))
		{
			Body = ((Variant)(ref val)).As<Node2D>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName.Bounds, ref val2))
		{
			Bounds = ((Variant)(ref val2)).As<Control>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName.IntentPosition, ref val3))
		{
			IntentPosition = ((Variant)(ref val3)).As<Marker2D>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName.OrbPosition, ref val4))
		{
			OrbPosition = ((Variant)(ref val4)).As<Marker2D>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName.TalkPosition, ref val5))
		{
			TalkPosition = ((Variant)(ref val5)).As<Marker2D>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName.VfxSpawnPosition, ref val6))
		{
			VfxSpawnPosition = ((Variant)(ref val6)).As<Marker2D>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName.DefaultScale, ref val7))
		{
			DefaultScale = ((Variant)(ref val7)).As<float>();
		}
		Variant val8 = default(Variant);
		if (info.TryGetProperty(PropertyName._hue, ref val8))
		{
			_hue = ((Variant)(ref val8)).As<float>();
		}
		Variant val9 = default(Variant);
		if (info.TryGetProperty(PropertyName._liquidOverlayTimer, ref val9))
		{
			_liquidOverlayTimer = ((Variant)(ref val9)).As<double>();
		}
		Variant val10 = default(Variant);
		if (info.TryGetProperty(PropertyName._savedNormalMaterial, ref val10))
		{
			_savedNormalMaterial = ((Variant)(ref val10)).As<Material>();
		}
		Variant val11 = default(Variant);
		if (info.TryGetProperty(PropertyName._currentLiquidOverlayMaterial, ref val11))
		{
			_currentLiquidOverlayMaterial = ((Variant)(ref val11)).As<ShaderMaterial>();
		}
	}
}
