using System;
using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;

namespace MegaCrit.Sts2.Core.Nodes.Cards;

[ScriptPath("res://src/Core/Nodes/Cards/NCardHighlight.cs")]
public class NCardHighlight : TextureRect
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName AnimShow = StringName.op_Implicit("AnimShow");

		public static readonly StringName AnimHide = StringName.op_Implicit("AnimHide");

		public static readonly StringName AnimHideInstantly = StringName.op_Implicit("AnimHideInstantly");

		public static readonly StringName AnimFlash = StringName.op_Implicit("AnimFlash");

		public static readonly StringName GetShaderParameter = StringName.op_Implicit("GetShaderParameter");

		public static readonly StringName SetShaderParameter = StringName.op_Implicit("SetShaderParameter");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName _curTween = StringName.op_Implicit("_curTween");

		public static readonly StringName _shaderMaterial = StringName.op_Implicit("_shaderMaterial");
	}

	public class SignalName : SignalName
	{
	}

	private static readonly StringName _shaderParameterWidth = new StringName("width");

	public static readonly Color playableColor = new Color(0f, 0.957f, 0.988f, 0.98f);

	public static readonly Color gold = new Color(1f, 0.784f, 0f, 0.98f);

	public static readonly Color red = new Color(0.83f, 0f, 0.33f, 0.98f);

	private Tween? _curTween;

	private ShaderMaterial _shaderMaterial;

	public override void _Ready()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		_shaderMaterial = (ShaderMaterial)((CanvasItem)this).Material;
	}

	public void AnimShow()
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		Tween? curTween = _curTween;
		if (curTween != null)
		{
			curTween.Kill();
		}
		_curTween = ((Node)this).CreateTween();
		_curTween.TweenMethod(Callable.From<float>((Action<float>)SetShaderParameter), Variant.op_Implicit(GetShaderParameter()), Variant.op_Implicit(0.075f), 0.5).SetEase((EaseType)1).SetTrans((TransitionType)7);
	}

	public void AnimHide()
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		Tween? curTween = _curTween;
		if (curTween != null)
		{
			curTween.Kill();
		}
		_curTween = ((Node)this).CreateTween();
		_curTween.TweenMethod(Callable.From<float>((Action<float>)SetShaderParameter), Variant.op_Implicit(GetShaderParameter()), Variant.op_Implicit(0.0), 0.5);
	}

	public void AnimHideInstantly()
	{
		Tween? curTween = _curTween;
		if (curTween != null)
		{
			curTween.Kill();
		}
		SetShaderParameter(0f);
	}

	public void AnimFlash()
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		Tween? curTween = _curTween;
		if (curTween != null)
		{
			curTween.Kill();
		}
		_curTween = ((Node)this).CreateTween();
		_curTween.TweenMethod(Callable.From<float>((Action<float>)SetShaderParameter), Variant.op_Implicit(GetShaderParameter()), Variant.op_Implicit(0.15f), 0.1);
		_curTween.TweenMethod(Callable.From<float>((Action<float>)SetShaderParameter), Variant.op_Implicit(0.15f), Variant.op_Implicit(0.075f), 0.35).SetEase((EaseType)1).SetTrans((TransitionType)7);
	}

	private float GetShaderParameter()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		Variant shaderParameter = _shaderMaterial.GetShaderParameter(_shaderParameterWidth);
		return ((Variant)(ref shaderParameter)).AsSingle();
	}

	private void SetShaderParameter(float val)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		_shaderMaterial.SetShaderParameter(_shaderParameterWidth, Variant.op_Implicit(val));
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
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(7);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AnimShow, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AnimHide, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AnimHideInstantly, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AnimFlash, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.GetShaderParameter, new PropertyInfo((Type)3, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetShaderParameter, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)3, StringName.op_Implicit("val"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
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
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.AnimShow && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			AnimShow();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.AnimHide && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			AnimHide();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.AnimHideInstantly && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			AnimHideInstantly();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.AnimFlash && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			AnimFlash();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.GetShaderParameter && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			float shaderParameter = GetShaderParameter();
			ret = VariantUtils.CreateFrom<float>(ref shaderParameter);
			return true;
		}
		if ((ref method) == MethodName.SetShaderParameter && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			SetShaderParameter(VariantUtils.ConvertTo<float>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		return ((TextureRect)this).InvokeGodotClassMethod(ref method, args, ref ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if ((ref method) == MethodName._Ready)
		{
			return true;
		}
		if ((ref method) == MethodName.AnimShow)
		{
			return true;
		}
		if ((ref method) == MethodName.AnimHide)
		{
			return true;
		}
		if ((ref method) == MethodName.AnimHideInstantly)
		{
			return true;
		}
		if ((ref method) == MethodName.AnimFlash)
		{
			return true;
		}
		if ((ref method) == MethodName.GetShaderParameter)
		{
			return true;
		}
		if ((ref method) == MethodName.SetShaderParameter)
		{
			return true;
		}
		return ((TextureRect)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._curTween)
		{
			_curTween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._shaderMaterial)
		{
			_shaderMaterial = VariantUtils.ConvertTo<ShaderMaterial>(ref value);
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
		if ((ref name) == PropertyName._curTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _curTween);
			return true;
		}
		if ((ref name) == PropertyName._shaderMaterial)
		{
			value = VariantUtils.CreateFrom<ShaderMaterial>(ref _shaderMaterial);
			return true;
		}
		return ((GodotObject)this).GetGodotClassPropertyValue(ref name, ref value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._curTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._shaderMaterial, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		info.AddProperty(PropertyName._curTween, Variant.From<Tween>(ref _curTween));
		info.AddProperty(PropertyName._shaderMaterial, Variant.From<ShaderMaterial>(ref _shaderMaterial));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._curTween, ref val))
		{
			_curTween = ((Variant)(ref val)).As<Tween>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._shaderMaterial, ref val2))
		{
			_shaderMaterial = ((Variant)(ref val2)).As<ShaderMaterial>();
		}
	}
}
