using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.TestSupport;

namespace MegaCrit.Sts2.Core.Nodes.Potions;

[ScriptPath("res://src/Core/Nodes/Potions/NPotion.cs")]
public class NPotion : Control
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName Reload = StringName.op_Implicit("Reload");

		public static readonly StringName DoFlash = StringName.op_Implicit("DoFlash");

		public static readonly StringName DoBounce = StringName.op_Implicit("DoBounce");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName Image = StringName.op_Implicit("Image");

		public static readonly StringName Outline = StringName.op_Implicit("Outline");

		public static readonly StringName _container = StringName.op_Implicit("_container");

		public static readonly StringName _bounceTween = StringName.op_Implicit("_bounceTween");

		public static readonly StringName _obtainedTween = StringName.op_Implicit("_obtainedTween");
	}

	public class SignalName : SignalName
	{
	}

	private const float _newlyAcquiredPopDuration = 0.35f;

	private const float _newlyAcquiredFadeInDuration = 0.1f;

	private const float _newlyAcquiredPopDistance = 40f;

	private PotionModel? _model;

	private Control _container;

	private Tween? _bounceTween;

	private Tween? _obtainedTween;

	private CancellationTokenSource? _cancellationTokenSource;

	public TextureRect Image { get; private set; }

	public TextureRect Outline { get; private set; }

	private static string ScenePath => SceneHelper.GetScenePath("/potions/potion");

	public static IEnumerable<string> AssetPaths => new global::_003C_003Ez__ReadOnlyArray<string>(new string[2]
	{
		ScenePath,
		NPotionFlashVfx.ScenePath
	});

	public PotionModel Model
	{
		get
		{
			return _model ?? throw new InvalidOperationException("Model was accessed before it was set.");
		}
		set
		{
			value.AssertMutable();
			_model = value;
			Reload();
		}
	}

	public static NPotion? Create(PotionModel potion)
	{
		if (TestMode.IsOn)
		{
			return null;
		}
		NPotion nPotion = PreloadManager.Cache.GetScene(ScenePath).Instantiate<NPotion>((GenEditState)0);
		nPotion.Model = potion;
		return nPotion;
	}

	public override void _Ready()
	{
		Image = ((Node)this).GetNode<TextureRect>(NodePath.op_Implicit("%Image"));
		Outline = ((Node)this).GetNode<TextureRect>(NodePath.op_Implicit("%Outline"));
		_container = ((Node)this).GetNode<Control>(NodePath.op_Implicit("Container"));
		Reload();
	}

	private void Reload()
	{
		if (((Node)this).IsNodeReady() && _model != null)
		{
			Image.Texture = _model.Image;
			Outline.Texture = _model.Outline;
		}
	}

	public async Task PlayNewlyAcquiredAnimation(Vector2? startLocation)
	{
		if (_cancellationTokenSource != null)
		{
			await _cancellationTokenSource.CancelAsync();
		}
		CancellationTokenSource cancelTokenSource = (_cancellationTokenSource = new CancellationTokenSource());
		await ((GodotObject)this).ToSignal((GodotObject)(object)((Node)this).GetTree(), SignalName.ProcessFrame);
		if (!cancelTokenSource.IsCancellationRequested)
		{
			Tween? obtainedTween = _obtainedTween;
			if (obtainedTween != null)
			{
				obtainedTween.Kill();
			}
			if (!startLocation.HasValue)
			{
				Control container = _container;
				Vector2 position = _container.Position;
				position.Y = 40f;
				container.Position = position;
				Control container2 = _container;
				Color modulate = ((CanvasItem)_container).Modulate;
				modulate.A = 0f;
				((CanvasItem)container2).Modulate = modulate;
				_obtainedTween = ((Node)this).GetTree().CreateTween();
				_obtainedTween.TweenProperty((GodotObject)(object)_container, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(1f), 0.10000000149011612);
				_obtainedTween.Parallel();
				_obtainedTween.SetEase((EaseType)1);
				_obtainedTween.SetTrans((TransitionType)10);
				_obtainedTween.TweenProperty((GodotObject)(object)_container, NodePath.op_Implicit("position:y"), Variant.op_Implicit(0f), 0.3499999940395355);
				_obtainedTween.TweenCallback(Callable.From((Action)DoFlash));
			}
			else
			{
				_container.GlobalPosition = startLocation.Value;
				Control container3 = _container;
				Color modulate = ((CanvasItem)_container).Modulate;
				modulate.A = 1f;
				((CanvasItem)container3).Modulate = modulate;
				_obtainedTween = ((Node)this).GetTree().CreateTween();
				_obtainedTween.SetEase((EaseType)1);
				_obtainedTween.SetTrans((TransitionType)4);
				_obtainedTween.TweenProperty((GodotObject)(object)_container, NodePath.op_Implicit("position"), Variant.op_Implicit(Vector2.Zero), 0.3499999940395355);
				_obtainedTween.TweenCallback(Callable.From((Action)DoFlash));
			}
		}
	}

	private void DoFlash()
	{
		((Node)(object)this).AddChildSafely((Node?)(object)NPotionFlashVfx.Create(this));
	}

	public void DoBounce()
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		Tween? bounceTween = _bounceTween;
		if (bounceTween != null)
		{
			bounceTween.Kill();
		}
		_bounceTween = ((Node)this).CreateTween();
		_bounceTween.TweenProperty((GodotObject)(object)_container, NodePath.op_Implicit("position:y"), Variant.op_Implicit(((Control)this).Position.Y - 12f), 0.125).SetEase((EaseType)1).SetTrans((TransitionType)1);
		_bounceTween.TweenProperty((GodotObject)(object)_container, NodePath.op_Implicit("position:y"), Variant.op_Implicit(0f), 0.125).SetEase((EaseType)0).SetTrans((TransitionType)1);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(4);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.Reload, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.DoFlash, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.DoBounce, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.Reload && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			Reload();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.DoFlash && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			DoFlash();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.DoBounce && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			DoBounce();
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
		if ((ref method) == MethodName.Reload)
		{
			return true;
		}
		if ((ref method) == MethodName.DoFlash)
		{
			return true;
		}
		if ((ref method) == MethodName.DoBounce)
		{
			return true;
		}
		return ((Control)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName.Image)
		{
			Image = VariantUtils.ConvertTo<TextureRect>(ref value);
			return true;
		}
		if ((ref name) == PropertyName.Outline)
		{
			Outline = VariantUtils.ConvertTo<TextureRect>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._container)
		{
			_container = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._bounceTween)
		{
			_bounceTween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._obtainedTween)
		{
			_obtainedTween = VariantUtils.ConvertTo<Tween>(ref value);
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
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName.Image)
		{
			TextureRect image = Image;
			value = VariantUtils.CreateFrom<TextureRect>(ref image);
			return true;
		}
		if ((ref name) == PropertyName.Outline)
		{
			TextureRect image = Outline;
			value = VariantUtils.CreateFrom<TextureRect>(ref image);
			return true;
		}
		if ((ref name) == PropertyName._container)
		{
			value = VariantUtils.CreateFrom<Control>(ref _container);
			return true;
		}
		if ((ref name) == PropertyName._bounceTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _bounceTween);
			return true;
		}
		if ((ref name) == PropertyName._obtainedTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _obtainedTween);
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
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._container, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.Image, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.Outline, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._bounceTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._obtainedTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		((GodotObject)this).SaveGodotObjectData(info);
		StringName image = PropertyName.Image;
		TextureRect image2 = Image;
		info.AddProperty(image, Variant.From<TextureRect>(ref image2));
		StringName outline = PropertyName.Outline;
		image2 = Outline;
		info.AddProperty(outline, Variant.From<TextureRect>(ref image2));
		info.AddProperty(PropertyName._container, Variant.From<Control>(ref _container));
		info.AddProperty(PropertyName._bounceTween, Variant.From<Tween>(ref _bounceTween));
		info.AddProperty(PropertyName._obtainedTween, Variant.From<Tween>(ref _obtainedTween));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName.Image, ref val))
		{
			Image = ((Variant)(ref val)).As<TextureRect>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName.Outline, ref val2))
		{
			Outline = ((Variant)(ref val2)).As<TextureRect>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._container, ref val3))
		{
			_container = ((Variant)(ref val3)).As<Control>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._bounceTween, ref val4))
		{
			_bounceTween = ((Variant)(ref val4)).As<Tween>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._obtainedTween, ref val5))
		{
			_obtainedTween = ((Variant)(ref val5)).As<Tween>();
		}
	}
}
