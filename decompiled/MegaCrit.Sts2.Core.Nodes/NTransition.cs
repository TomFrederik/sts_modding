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
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.Settings;
using MegaCrit.Sts2.Core.TestSupport;

namespace MegaCrit.Sts2.Core.Nodes;

[ScriptPath("res://src/Core/Nodes/NTransition.cs")]
public class NTransition : ColorRect
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName InTransition = StringName.op_Implicit("InTransition");

		public static readonly StringName _initialGradientYPosition = StringName.op_Implicit("_initialGradientYPosition");

		public static readonly StringName _targetGradientYPosition = StringName.op_Implicit("_targetGradientYPosition");

		public static readonly StringName _gradientTransition = StringName.op_Implicit("_gradientTransition");

		public static readonly StringName _simpleTransition = StringName.op_Implicit("_simpleTransition");

		public static readonly StringName _tween = StringName.op_Implicit("_tween");
	}

	public class SignalName : SignalName
	{
	}

	private static readonly StringName _threshold = new StringName("threshold");

	private const string _fightTransitionPath = "res://materials/transitions/fight_transition_mat.tres";

	private const string _fadeTransitionPath = "res://materials/transitions/fade_transition_mat.tres";

	private float _initialGradientYPosition;

	private float _targetGradientYPosition;

	private Control _gradientTransition;

	private Control _simpleTransition;

	private Tween? _tween;

	public bool InTransition { get; private set; }

	public static IEnumerable<string> AssetPaths => new global::_003C_003Ez__ReadOnlyArray<string>(new string[2] { "res://materials/transitions/fight_transition_mat.tres", "res://materials/transitions/fade_transition_mat.tres" });

	public override void _Ready()
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		_gradientTransition = ((Node)this).GetNode<Control>(NodePath.op_Implicit("GradientTransition"));
		_simpleTransition = ((Node)this).GetNode<Control>(NodePath.op_Implicit("SimpleTransition"));
		_initialGradientYPosition = _gradientTransition.Position.Y;
		_targetGradientYPosition = 0f;
	}

	public async Task FadeOut(float time = 0.8f, string transitionPath = "res://materials/transitions/fade_transition_mat.tres", CancellationToken? cancelToken = null)
	{
		if (SaveManager.Instance.PrefsSave.FastMode == FastModeType.Instant)
		{
			InTransition = true;
			((CanvasItem)this).Visible = false;
			return;
		}
		InTransition = true;
		Control simpleTransition = _simpleTransition;
		Color modulate = ((CanvasItem)_simpleTransition).Modulate;
		modulate.A = 0f;
		((CanvasItem)simpleTransition).Modulate = modulate;
		Tween? tween = _tween;
		if (tween != null)
		{
			tween.Kill();
		}
		_tween = ((Node)this).CreateTween().SetParallel(true);
		_tween.TweenProperty((GodotObject)(object)_simpleTransition, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(1f), (double)time).SetEase((EaseType)0).SetTrans((TransitionType)4);
		Material material = ((CanvasItem)this).Material;
		ShaderMaterial val = (ShaderMaterial)(object)((material is ShaderMaterial) ? material : null);
		if (val == null)
		{
			Log.Warn("NTransition.Material is null or not a ShaderMaterial (actual: " + (((object)((CanvasItem)this).Material)?.GetType().Name ?? "null") + "). Skipping transition.");
			return;
		}
		Variant shaderParameter = val.GetShaderParameter(_threshold);
		if (((Variant)(ref shaderParameter)).AsInt32() == 1)
		{
			return;
		}
		((CanvasItem)this).Material = PreloadManager.Cache.GetMaterial(transitionPath);
		Material material2 = ((CanvasItem)this).Material;
		ShaderMaterial transitionMaterial = (ShaderMaterial)(object)((material2 is ShaderMaterial) ? material2 : null);
		if (transitionMaterial == null)
		{
			Log.Warn("NTransition.Material failed to load from cache (path: " + transitionPath + "). Skipping transition.");
			return;
		}
		transitionMaterial.SetShaderParameter(_threshold, Variant.op_Implicit(0));
		double t = 0.0;
		while (t < (double)time)
		{
			if (cancelToken.HasValue && cancelToken.GetValueOrDefault().IsCancellationRequested)
			{
				_tween?.FastForwardToCompletion();
				break;
			}
			transitionMaterial.SetShaderParameter(_threshold, Variant.op_Implicit(1.0 - ((double)time - t)));
			t += ((Node)this).GetProcessDeltaTime();
			await ((GodotObject)this).ToSignal((GodotObject)(object)((Node)this).GetTree(), SignalName.ProcessFrame);
		}
		((Control)this).MouseFilter = (MouseFilterEnum)0;
		transitionMaterial.SetShaderParameter(_threshold, Variant.op_Implicit(1));
	}

	public async Task FadeIn(float time = 0.8f, string transitionPath = "res://materials/transitions/fade_transition_mat.tres", CancellationToken? cancelToken = null)
	{
		if (SaveManager.Instance.PrefsSave.FastMode == FastModeType.Instant)
		{
			((CanvasItem)this).Visible = false;
			InTransition = false;
			((Control)this).MouseFilter = (MouseFilterEnum)2;
			return;
		}
		Tween? tween = _tween;
		if (tween != null)
		{
			tween.Kill();
		}
		Control simpleTransition = _simpleTransition;
		Color modulate = ((CanvasItem)_simpleTransition).Modulate;
		modulate.A = 0f;
		((CanvasItem)simpleTransition).Modulate = modulate;
		Material material = ((CanvasItem)this).Material;
		ShaderMaterial val = (ShaderMaterial)(object)((material is ShaderMaterial) ? material : null);
		if (val == null)
		{
			Log.Warn("NTransition.Material is null or not a ShaderMaterial (actual: " + (((object)((CanvasItem)this).Material)?.GetType().Name ?? "null") + "). Skipping transition.");
			InTransition = false;
			return;
		}
		Variant shaderParameter = val.GetShaderParameter(_threshold);
		if (((Variant)(ref shaderParameter)).AsInt32() == 0)
		{
			InTransition = false;
			return;
		}
		((CanvasItem)this).Material = PreloadManager.Cache.GetMaterial(transitionPath);
		Material material2 = ((CanvasItem)this).Material;
		ShaderMaterial transitionMaterial = (ShaderMaterial)(object)((material2 is ShaderMaterial) ? material2 : null);
		if (transitionMaterial == null)
		{
			Log.Warn("NTransition.Material failed to load from cache (path: " + transitionPath + "). Skipping transition.");
			InTransition = false;
			return;
		}
		transitionMaterial.SetShaderParameter(_threshold, Variant.op_Implicit(1));
		((Control)this).MouseFilter = (MouseFilterEnum)2;
		double t = 0.0;
		while (t < (double)time)
		{
			if (cancelToken.HasValue && cancelToken.GetValueOrDefault().IsCancellationRequested)
			{
				_tween?.FastForwardToCompletion();
				break;
			}
			transitionMaterial.SetShaderParameter(_threshold, Variant.op_Implicit(Ease.CubicIn((float)((double)time - t))));
			t += ((Node)this).GetProcessDeltaTime();
			await ((GodotObject)this).ToSignal((GodotObject)(object)((Node)this).GetTree(), SignalName.ProcessFrame);
			if (t / (double)time > 0.75)
			{
				InTransition = false;
			}
		}
		InTransition = false;
		transitionMaterial.SetShaderParameter(_threshold, Variant.op_Implicit(0));
		((Control)this).MouseFilter = (MouseFilterEnum)2;
	}

	public async Task RoomFadeOut()
	{
		InTransition = true;
		if (!TestMode.IsOn && SaveManager.Instance.PrefsSave.FastMode != FastModeType.Instant)
		{
			Control simpleTransition = _simpleTransition;
			Color modulate = ((CanvasItem)_simpleTransition).Modulate;
			modulate.A = 0f;
			((CanvasItem)simpleTransition).Modulate = modulate;
			Control gradientTransition = _gradientTransition;
			modulate = ((CanvasItem)_gradientTransition).Modulate;
			modulate.A = 1f;
			((CanvasItem)gradientTransition).Modulate = modulate;
			Control gradientTransition2 = _gradientTransition;
			Vector2 position = _gradientTransition.Position;
			position.Y = _initialGradientYPosition;
			gradientTransition2.Position = position;
			Tween? tween = _tween;
			if (tween != null)
			{
				tween.Kill();
			}
			_tween = ((Node)this).CreateTween().SetParallel(true);
			if (SaveManager.Instance.PrefsSave.FastMode == FastModeType.Normal)
			{
				_tween.TweenProperty((GodotObject)(object)_gradientTransition, NodePath.op_Implicit("position:y"), Variant.op_Implicit(_targetGradientYPosition), 0.6).SetDelay(0.5);
				_tween.TweenProperty((GodotObject)(object)_simpleTransition, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(1f), 0.6).SetDelay(0.5);
			}
			else if (SaveManager.Instance.PrefsSave.FastMode == FastModeType.Fast)
			{
				_tween.TweenProperty((GodotObject)(object)_simpleTransition, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(1f), 0.3).SetEase((EaseType)1).SetTrans((TransitionType)4)
					.SetDelay(0.3);
			}
			await ((GodotObject)this).ToSignal((GodotObject)(object)_tween, SignalName.Finished);
		}
	}

	public async Task RoomFadeIn(bool showTransition = true)
	{
		if (TestMode.IsOn)
		{
			return;
		}
		Color modulate;
		if (!showTransition || SaveManager.Instance.PrefsSave.FastMode == FastModeType.Instant)
		{
			Control simpleTransition = _simpleTransition;
			modulate = ((CanvasItem)_simpleTransition).Modulate;
			modulate.A = 0f;
			((CanvasItem)simpleTransition).Modulate = modulate;
		}
		Material material = ((CanvasItem)this).Material;
		ShaderMaterial val = (ShaderMaterial)(object)((material is ShaderMaterial) ? material : null);
		if (val == null)
		{
			Log.Warn("NTransition.Material is null or not a ShaderMaterial (actual: " + (((object)((CanvasItem)this).Material)?.GetType().Name ?? "null") + "). Skipping transition.");
			InTransition = false;
			return;
		}
		val.SetShaderParameter(_threshold, Variant.op_Implicit(0));
		Control gradientTransition = _gradientTransition;
		modulate = ((CanvasItem)_gradientTransition).Modulate;
		modulate.A = 0f;
		((CanvasItem)gradientTransition).Modulate = modulate;
		Control simpleTransition2 = _simpleTransition;
		modulate = ((CanvasItem)_simpleTransition).Modulate;
		modulate.A = 1f;
		((CanvasItem)simpleTransition2).Modulate = modulate;
		Tween? tween = _tween;
		if (tween != null)
		{
			tween.Kill();
		}
		_tween = ((Node)this).CreateTween().SetParallel(true);
		if (SaveManager.Instance.PrefsSave.FastMode == FastModeType.Fast)
		{
			_tween.TweenProperty((GodotObject)(object)_simpleTransition, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0f), 0.3);
			((Control)this).MouseFilter = (MouseFilterEnum)2;
		}
		else
		{
			_tween.TweenProperty((GodotObject)(object)_simpleTransition, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0f), 0.8);
			_tween.TweenCallback(Callable.From((Action)delegate
			{
				((Control)this).MouseFilter = (MouseFilterEnum)2;
				InTransition = false;
			})).SetDelay(0.2);
		}
		await ((GodotObject)this).ToSignal((GodotObject)(object)_tween, SignalName.Finished);
		InTransition = false;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(1);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		return ((ColorRect)this).InvokeGodotClassMethod(ref method, args, ref ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if ((ref method) == MethodName._Ready)
		{
			return true;
		}
		return ((ColorRect)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName.InTransition)
		{
			InTransition = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._initialGradientYPosition)
		{
			_initialGradientYPosition = VariantUtils.ConvertTo<float>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._targetGradientYPosition)
		{
			_targetGradientYPosition = VariantUtils.ConvertTo<float>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._gradientTransition)
		{
			_gradientTransition = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._simpleTransition)
		{
			_simpleTransition = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._tween)
		{
			_tween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		return ((GodotObject)this).SetGodotClassPropertyValue(ref name, ref value);
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
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName.InTransition)
		{
			bool inTransition = InTransition;
			value = VariantUtils.CreateFrom<bool>(ref inTransition);
			return true;
		}
		if ((ref name) == PropertyName._initialGradientYPosition)
		{
			value = VariantUtils.CreateFrom<float>(ref _initialGradientYPosition);
			return true;
		}
		if ((ref name) == PropertyName._targetGradientYPosition)
		{
			value = VariantUtils.CreateFrom<float>(ref _targetGradientYPosition);
			return true;
		}
		if ((ref name) == PropertyName._gradientTransition)
		{
			value = VariantUtils.CreateFrom<Control>(ref _gradientTransition);
			return true;
		}
		if ((ref name) == PropertyName._simpleTransition)
		{
			value = VariantUtils.CreateFrom<Control>(ref _simpleTransition);
			return true;
		}
		if ((ref name) == PropertyName._tween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _tween);
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
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)1, PropertyName.InTransition, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)3, PropertyName._initialGradientYPosition, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)3, PropertyName._targetGradientYPosition, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._gradientTransition, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._simpleTransition, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._tween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		StringName inTransition = PropertyName.InTransition;
		bool inTransition2 = InTransition;
		info.AddProperty(inTransition, Variant.From<bool>(ref inTransition2));
		info.AddProperty(PropertyName._initialGradientYPosition, Variant.From<float>(ref _initialGradientYPosition));
		info.AddProperty(PropertyName._targetGradientYPosition, Variant.From<float>(ref _targetGradientYPosition));
		info.AddProperty(PropertyName._gradientTransition, Variant.From<Control>(ref _gradientTransition));
		info.AddProperty(PropertyName._simpleTransition, Variant.From<Control>(ref _simpleTransition));
		info.AddProperty(PropertyName._tween, Variant.From<Tween>(ref _tween));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName.InTransition, ref val))
		{
			InTransition = ((Variant)(ref val)).As<bool>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._initialGradientYPosition, ref val2))
		{
			_initialGradientYPosition = ((Variant)(ref val2)).As<float>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._targetGradientYPosition, ref val3))
		{
			_targetGradientYPosition = ((Variant)(ref val3)).As<float>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._gradientTransition, ref val4))
		{
			_gradientTransition = ((Variant)(ref val4)).As<Control>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._simpleTransition, ref val5))
		{
			_simpleTransition = ((Variant)(ref val5)).As<Control>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._tween, ref val6))
		{
			_tween = ((Variant)(ref val6)).As<Tween>();
		}
	}
}
