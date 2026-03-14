using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.Timeline;

namespace MegaCrit.Sts2.Core.Nodes.Screens.Timeline;

[ScriptPath("res://src/Core/Nodes/Screens/Timeline/NEpochCard.cs")]
public class NEpochCard : Control
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName _Process = StringName.op_Implicit("_Process");

		public static readonly StringName SetToWigglyUnlockPreviewMode = StringName.op_Implicit("SetToWigglyUnlockPreviewMode");

		public static readonly StringName GlowFlash = StringName.op_Implicit("GlowFlash");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName _glow = StringName.op_Implicit("_glow");

		public static readonly StringName _mask = StringName.op_Implicit("_mask");

		public static readonly StringName _portrait = StringName.op_Implicit("_portrait");

		public static readonly StringName _isHovered = StringName.op_Implicit("_isHovered");

		public static readonly StringName _isHoverable = StringName.op_Implicit("_isHoverable");

		public static readonly StringName _isHeld = StringName.op_Implicit("_isHeld");

		public static readonly StringName _isWigglyUnlockPreviewMode = StringName.op_Implicit("_isWigglyUnlockPreviewMode");

		public static readonly StringName _glowTween = StringName.op_Implicit("_glowTween");

		public static readonly StringName _targetScale = StringName.op_Implicit("_targetScale");

		public static readonly StringName _time = StringName.op_Implicit("_time");

		public static readonly StringName _noiseSpeed = StringName.op_Implicit("_noiseSpeed");

		public static readonly StringName _noise = StringName.op_Implicit("_noise");

		public static readonly StringName _denyTween = StringName.op_Implicit("_denyTween");

		public static readonly StringName _transparencyTween = StringName.op_Implicit("_transparencyTween");

		public static readonly StringName _scaleTween = StringName.op_Implicit("_scaleTween");

		public static readonly StringName _blueGlowColor = StringName.op_Implicit("_blueGlowColor");

		public static readonly StringName _goldGlowColor = StringName.op_Implicit("_goldGlowColor");
	}

	public class SignalName : SignalName
	{
	}

	private TextureRect _glow;

	private TextureRect _mask;

	private TextureRect _portrait;

	private bool _isHovered;

	private bool _isHoverable = true;

	private bool _isHeld;

	private bool _isWigglyUnlockPreviewMode;

	private Tween? _glowTween;

	private Vector2 _targetScale;

	private float _time;

	private float _noiseSpeed = 0.25f;

	private FastNoiseLite _noise;

	private Tween? _denyTween;

	private Tween? _transparencyTween;

	private Tween? _scaleTween;

	private Color _blueGlowColor = new Color("2de5ff80");

	private Color _goldGlowColor = new Color("ffd92e80");

	public void Init(EpochModel epochModel)
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		_glow = ((Node)this).GetNode<TextureRect>(NodePath.op_Implicit("%GlowPlaceholder"));
		_portrait = ((Node)this).GetNode<TextureRect>(NodePath.op_Implicit("%Portrait"));
		_portrait.Texture = epochModel.BigPortrait;
		((Control)this).Scale = Vector2.One;
	}

	public override void _Ready()
	{
		_mask = ((Node)this).GetNode<TextureRect>(NodePath.op_Implicit("%Mask"));
	}

	public override void _Process(double delta)
	{
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		if (_isWigglyUnlockPreviewMode)
		{
			_time += _noiseSpeed * (float)delta;
			float num = 42f * ((Noise)_noise).GetNoise2D(_time, 0f);
			float num2 = 42f * ((Noise)_noise).GetNoise2D(0f, _time);
			((Control)this).Position = new Vector2(num, num2) - ((Control)this).GetPivotOffset();
		}
	}

	public void SetToWigglyUnlockPreviewMode()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		_noise = new FastNoiseLite();
		_noise.Frequency = 0.2f;
		_noise.SetSeed(Rng.Chaotic.NextInt());
		_isWigglyUnlockPreviewMode = true;
		((Control)this).MouseFilter = (MouseFilterEnum)2;
		((Control)this).Scale = Vector2.One * 1.2f;
	}

	private void GlowFlash()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		((CanvasItem)this).Modulate = new Color(1f, 1f, 1f, 0.75f);
		((CanvasItem)_glow).Modulate = Colors.Gold;
		Tween? glowTween = _glowTween;
		if (glowTween != null)
		{
			glowTween.Kill();
		}
		_glowTween = ((Node)this).CreateTween().SetParallel(true);
		_glowTween.TweenProperty((GodotObject)(object)_glow, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0.5f), 1.0).SetEase((EaseType)1).SetTrans((TransitionType)5);
		_glowTween.TweenProperty((GodotObject)(object)_glow, NodePath.op_Implicit("scale"), Variant.op_Implicit(Vector2.One), 1.0).SetEase((EaseType)1).SetTrans((TransitionType)5)
			.From(Variant.op_Implicit(Vector2.One * 1.5f));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(4);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Process, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)3, StringName.op_Implicit("delta"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetToWigglyUnlockPreviewMode, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.GlowFlash, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._Process && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			((Node)this)._Process(VariantUtils.ConvertTo<double>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetToWigglyUnlockPreviewMode && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			SetToWigglyUnlockPreviewMode();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.GlowFlash && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			GlowFlash();
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
		if ((ref method) == MethodName._Process)
		{
			return true;
		}
		if ((ref method) == MethodName.SetToWigglyUnlockPreviewMode)
		{
			return true;
		}
		if ((ref method) == MethodName.GlowFlash)
		{
			return true;
		}
		return ((Control)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName._glow)
		{
			_glow = VariantUtils.ConvertTo<TextureRect>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._mask)
		{
			_mask = VariantUtils.ConvertTo<TextureRect>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._portrait)
		{
			_portrait = VariantUtils.ConvertTo<TextureRect>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._isHovered)
		{
			_isHovered = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._isHoverable)
		{
			_isHoverable = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._isHeld)
		{
			_isHeld = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._isWigglyUnlockPreviewMode)
		{
			_isWigglyUnlockPreviewMode = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._glowTween)
		{
			_glowTween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._targetScale)
		{
			_targetScale = VariantUtils.ConvertTo<Vector2>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._time)
		{
			_time = VariantUtils.ConvertTo<float>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._noiseSpeed)
		{
			_noiseSpeed = VariantUtils.ConvertTo<float>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._noise)
		{
			_noise = VariantUtils.ConvertTo<FastNoiseLite>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._denyTween)
		{
			_denyTween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._transparencyTween)
		{
			_transparencyTween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._scaleTween)
		{
			_scaleTween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._blueGlowColor)
		{
			_blueGlowColor = VariantUtils.ConvertTo<Color>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._goldGlowColor)
		{
			_goldGlowColor = VariantUtils.ConvertTo<Color>(ref value);
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
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName._glow)
		{
			value = VariantUtils.CreateFrom<TextureRect>(ref _glow);
			return true;
		}
		if ((ref name) == PropertyName._mask)
		{
			value = VariantUtils.CreateFrom<TextureRect>(ref _mask);
			return true;
		}
		if ((ref name) == PropertyName._portrait)
		{
			value = VariantUtils.CreateFrom<TextureRect>(ref _portrait);
			return true;
		}
		if ((ref name) == PropertyName._isHovered)
		{
			value = VariantUtils.CreateFrom<bool>(ref _isHovered);
			return true;
		}
		if ((ref name) == PropertyName._isHoverable)
		{
			value = VariantUtils.CreateFrom<bool>(ref _isHoverable);
			return true;
		}
		if ((ref name) == PropertyName._isHeld)
		{
			value = VariantUtils.CreateFrom<bool>(ref _isHeld);
			return true;
		}
		if ((ref name) == PropertyName._isWigglyUnlockPreviewMode)
		{
			value = VariantUtils.CreateFrom<bool>(ref _isWigglyUnlockPreviewMode);
			return true;
		}
		if ((ref name) == PropertyName._glowTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _glowTween);
			return true;
		}
		if ((ref name) == PropertyName._targetScale)
		{
			value = VariantUtils.CreateFrom<Vector2>(ref _targetScale);
			return true;
		}
		if ((ref name) == PropertyName._time)
		{
			value = VariantUtils.CreateFrom<float>(ref _time);
			return true;
		}
		if ((ref name) == PropertyName._noiseSpeed)
		{
			value = VariantUtils.CreateFrom<float>(ref _noiseSpeed);
			return true;
		}
		if ((ref name) == PropertyName._noise)
		{
			value = VariantUtils.CreateFrom<FastNoiseLite>(ref _noise);
			return true;
		}
		if ((ref name) == PropertyName._denyTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _denyTween);
			return true;
		}
		if ((ref name) == PropertyName._transparencyTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _transparencyTween);
			return true;
		}
		if ((ref name) == PropertyName._scaleTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _scaleTween);
			return true;
		}
		if ((ref name) == PropertyName._blueGlowColor)
		{
			value = VariantUtils.CreateFrom<Color>(ref _blueGlowColor);
			return true;
		}
		if ((ref name) == PropertyName._goldGlowColor)
		{
			value = VariantUtils.CreateFrom<Color>(ref _goldGlowColor);
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
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._glow, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._mask, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._portrait, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._isHovered, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._isHoverable, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._isHeld, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._isWigglyUnlockPreviewMode, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._glowTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)5, PropertyName._targetScale, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)3, PropertyName._time, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)3, PropertyName._noiseSpeed, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._noise, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._denyTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._transparencyTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._scaleTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)20, PropertyName._blueGlowColor, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)20, PropertyName._goldGlowColor, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		info.AddProperty(PropertyName._glow, Variant.From<TextureRect>(ref _glow));
		info.AddProperty(PropertyName._mask, Variant.From<TextureRect>(ref _mask));
		info.AddProperty(PropertyName._portrait, Variant.From<TextureRect>(ref _portrait));
		info.AddProperty(PropertyName._isHovered, Variant.From<bool>(ref _isHovered));
		info.AddProperty(PropertyName._isHoverable, Variant.From<bool>(ref _isHoverable));
		info.AddProperty(PropertyName._isHeld, Variant.From<bool>(ref _isHeld));
		info.AddProperty(PropertyName._isWigglyUnlockPreviewMode, Variant.From<bool>(ref _isWigglyUnlockPreviewMode));
		info.AddProperty(PropertyName._glowTween, Variant.From<Tween>(ref _glowTween));
		info.AddProperty(PropertyName._targetScale, Variant.From<Vector2>(ref _targetScale));
		info.AddProperty(PropertyName._time, Variant.From<float>(ref _time));
		info.AddProperty(PropertyName._noiseSpeed, Variant.From<float>(ref _noiseSpeed));
		info.AddProperty(PropertyName._noise, Variant.From<FastNoiseLite>(ref _noise));
		info.AddProperty(PropertyName._denyTween, Variant.From<Tween>(ref _denyTween));
		info.AddProperty(PropertyName._transparencyTween, Variant.From<Tween>(ref _transparencyTween));
		info.AddProperty(PropertyName._scaleTween, Variant.From<Tween>(ref _scaleTween));
		info.AddProperty(PropertyName._blueGlowColor, Variant.From<Color>(ref _blueGlowColor));
		info.AddProperty(PropertyName._goldGlowColor, Variant.From<Color>(ref _goldGlowColor));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._glow, ref val))
		{
			_glow = ((Variant)(ref val)).As<TextureRect>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._mask, ref val2))
		{
			_mask = ((Variant)(ref val2)).As<TextureRect>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._portrait, ref val3))
		{
			_portrait = ((Variant)(ref val3)).As<TextureRect>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._isHovered, ref val4))
		{
			_isHovered = ((Variant)(ref val4)).As<bool>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._isHoverable, ref val5))
		{
			_isHoverable = ((Variant)(ref val5)).As<bool>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._isHeld, ref val6))
		{
			_isHeld = ((Variant)(ref val6)).As<bool>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName._isWigglyUnlockPreviewMode, ref val7))
		{
			_isWigglyUnlockPreviewMode = ((Variant)(ref val7)).As<bool>();
		}
		Variant val8 = default(Variant);
		if (info.TryGetProperty(PropertyName._glowTween, ref val8))
		{
			_glowTween = ((Variant)(ref val8)).As<Tween>();
		}
		Variant val9 = default(Variant);
		if (info.TryGetProperty(PropertyName._targetScale, ref val9))
		{
			_targetScale = ((Variant)(ref val9)).As<Vector2>();
		}
		Variant val10 = default(Variant);
		if (info.TryGetProperty(PropertyName._time, ref val10))
		{
			_time = ((Variant)(ref val10)).As<float>();
		}
		Variant val11 = default(Variant);
		if (info.TryGetProperty(PropertyName._noiseSpeed, ref val11))
		{
			_noiseSpeed = ((Variant)(ref val11)).As<float>();
		}
		Variant val12 = default(Variant);
		if (info.TryGetProperty(PropertyName._noise, ref val12))
		{
			_noise = ((Variant)(ref val12)).As<FastNoiseLite>();
		}
		Variant val13 = default(Variant);
		if (info.TryGetProperty(PropertyName._denyTween, ref val13))
		{
			_denyTween = ((Variant)(ref val13)).As<Tween>();
		}
		Variant val14 = default(Variant);
		if (info.TryGetProperty(PropertyName._transparencyTween, ref val14))
		{
			_transparencyTween = ((Variant)(ref val14)).As<Tween>();
		}
		Variant val15 = default(Variant);
		if (info.TryGetProperty(PropertyName._scaleTween, ref val15))
		{
			_scaleTween = ((Variant)(ref val15)).As<Tween>();
		}
		Variant val16 = default(Variant);
		if (info.TryGetProperty(PropertyName._blueGlowColor, ref val16))
		{
			_blueGlowColor = ((Variant)(ref val16)).As<Color>();
		}
		Variant val17 = default(Variant);
		if (info.TryGetProperty(PropertyName._goldGlowColor, ref val17))
		{
			_goldGlowColor = ((Variant)(ref val17)).As<Color>();
		}
	}
}
