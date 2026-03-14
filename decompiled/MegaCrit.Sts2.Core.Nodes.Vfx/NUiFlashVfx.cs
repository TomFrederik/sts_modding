using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.TestSupport;

namespace MegaCrit.Sts2.Core.Nodes.Vfx;

[ScriptPath("res://src/Core/Nodes/Vfx/NUiFlashVfx.cs")]
public class NUiFlashVfx : Control
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName Create = StringName.op_Implicit("Create");

		public static readonly StringName _ExitTree = StringName.op_Implicit("_ExitTree");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName _textureRect = StringName.op_Implicit("_textureRect");

		public static readonly StringName _texture = StringName.op_Implicit("_texture");

		public static readonly StringName _modulate = StringName.op_Implicit("_modulate");

		public static readonly StringName _spriteTween = StringName.op_Implicit("_spriteTween");
	}

	public class SignalName : SignalName
	{
	}

	private const string _scenePath = "res://scenes/vfx/ui_flash_vfx.tscn";

	private TextureRect _textureRect;

	private Texture2D _texture;

	private Color _modulate;

	private Tween? _spriteTween;

	public static IEnumerable<string> AssetPaths => new _003C_003Ez__ReadOnlySingleElementList<string>("res://scenes/vfx/ui_flash_vfx.tscn");

	public override void _Ready()
	{
		_textureRect = ((Node)this).GetNode<TextureRect>(NodePath.op_Implicit("TextureRect"));
		_textureRect.Texture = _texture;
	}

	public async Task StartVfx()
	{
		TextureRect textureRect = _textureRect;
		Color modulate = _modulate;
		modulate.A = 0f;
		((CanvasItem)textureRect).Modulate = modulate;
		((Control)_textureRect).PivotOffset = ((Control)_textureRect).Size * 0.5f;
		_spriteTween = ((Node)this).CreateTween();
		_spriteTween.SetParallel(true);
		_spriteTween.TweenProperty((GodotObject)(object)_textureRect, NodePath.op_Implicit("scale"), Variant.op_Implicit(Vector2.One * 1.3f), 0.5);
		_spriteTween.TweenProperty((GodotObject)(object)_textureRect, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(1f), 0.25).SetEase((EaseType)1).SetTrans((TransitionType)1);
		_spriteTween.TweenProperty((GodotObject)(object)_textureRect, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0f), 0.25).SetEase((EaseType)0).SetTrans((TransitionType)1)
			.SetDelay(0.3499999940395355);
		await ((GodotObject)this).ToSignal((GodotObject)(object)_spriteTween, SignalName.Finished);
		((Node)(object)this).QueueFreeSafely();
	}

	public static NUiFlashVfx? Create(Texture2D tex, Color modulate)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		if (TestMode.IsOn)
		{
			return null;
		}
		NUiFlashVfx nUiFlashVfx = (NUiFlashVfx)(object)PreloadManager.Cache.GetScene("res://scenes/vfx/ui_flash_vfx.tscn").Instantiate((GenEditState)0);
		nUiFlashVfx._texture = tex;
		nUiFlashVfx._modulate = modulate;
		return nUiFlashVfx;
	}

	public override void _ExitTree()
	{
		Tween? spriteTween = _spriteTween;
		if (spriteTween != null)
		{
			spriteTween.Kill();
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Expected O, but got Unknown
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Expected O, but got Unknown
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(3);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.Create, new PropertyInfo((Type)24, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false), (MethodFlags)33, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("tex"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Texture2D"), false),
			new PropertyInfo((Type)20, StringName.op_Implicit("modulate"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._ExitTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.Create && ((NativeVariantPtrArgs)(ref args)).Count == 2)
		{
			NUiFlashVfx nUiFlashVfx = Create(VariantUtils.ConvertTo<Texture2D>(ref ((NativeVariantPtrArgs)(ref args))[0]), VariantUtils.ConvertTo<Color>(ref ((NativeVariantPtrArgs)(ref args))[1]));
			ret = VariantUtils.CreateFrom<NUiFlashVfx>(ref nUiFlashVfx);
			return true;
		}
		if ((ref method) == MethodName._ExitTree && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._ExitTree();
			ret = default(godot_variant);
			return true;
		}
		return ((Control)this).InvokeGodotClassMethod(ref method, args, ref ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static bool InvokeGodotClassStaticMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.Create && ((NativeVariantPtrArgs)(ref args)).Count == 2)
		{
			NUiFlashVfx nUiFlashVfx = Create(VariantUtils.ConvertTo<Texture2D>(ref ((NativeVariantPtrArgs)(ref args))[0]), VariantUtils.ConvertTo<Color>(ref ((NativeVariantPtrArgs)(ref args))[1]));
			ret = VariantUtils.CreateFrom<NUiFlashVfx>(ref nUiFlashVfx);
			return true;
		}
		ret = default(godot_variant);
		return false;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if ((ref method) == MethodName._Ready)
		{
			return true;
		}
		if ((ref method) == MethodName.Create)
		{
			return true;
		}
		if ((ref method) == MethodName._ExitTree)
		{
			return true;
		}
		return ((Control)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName._textureRect)
		{
			_textureRect = VariantUtils.ConvertTo<TextureRect>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._texture)
		{
			_texture = VariantUtils.ConvertTo<Texture2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._modulate)
		{
			_modulate = VariantUtils.ConvertTo<Color>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._spriteTween)
		{
			_spriteTween = VariantUtils.ConvertTo<Tween>(ref value);
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
		if ((ref name) == PropertyName._textureRect)
		{
			value = VariantUtils.CreateFrom<TextureRect>(ref _textureRect);
			return true;
		}
		if ((ref name) == PropertyName._texture)
		{
			value = VariantUtils.CreateFrom<Texture2D>(ref _texture);
			return true;
		}
		if ((ref name) == PropertyName._modulate)
		{
			value = VariantUtils.CreateFrom<Color>(ref _modulate);
			return true;
		}
		if ((ref name) == PropertyName._spriteTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _spriteTween);
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
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._textureRect, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._texture, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)20, PropertyName._modulate, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._spriteTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		info.AddProperty(PropertyName._textureRect, Variant.From<TextureRect>(ref _textureRect));
		info.AddProperty(PropertyName._texture, Variant.From<Texture2D>(ref _texture));
		info.AddProperty(PropertyName._modulate, Variant.From<Color>(ref _modulate));
		info.AddProperty(PropertyName._spriteTween, Variant.From<Tween>(ref _spriteTween));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._textureRect, ref val))
		{
			_textureRect = ((Variant)(ref val)).As<TextureRect>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._texture, ref val2))
		{
			_texture = ((Variant)(ref val2)).As<Texture2D>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._modulate, ref val3))
		{
			_modulate = ((Variant)(ref val3)).As<Color>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._spriteTween, ref val4))
		{
			_spriteTween = ((Variant)(ref val4)).As<Tween>();
		}
	}
}
