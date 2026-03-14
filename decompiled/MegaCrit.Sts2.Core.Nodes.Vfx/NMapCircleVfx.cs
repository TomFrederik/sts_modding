using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.TestSupport;

namespace MegaCrit.Sts2.Core.Nodes.Vfx;

[ScriptPath("res://src/Core/Nodes/Vfx/NMapCircleVfx.cs")]
public class NMapCircleVfx : Control
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName Create = StringName.op_Implicit("Create");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName _image = StringName.op_Implicit("_image");

		public static readonly StringName _playAnim = StringName.op_Implicit("_playAnim");
	}

	public class SignalName : SignalName
	{
	}

	private TextureRect _image;

	private const string _path = "res://scenes/vfx/map_circle_vfx.tscn";

	private static readonly string[] _textures = new string[5] { "res://images/atlases/compressed.sprites/map/map_circle_0.tres", "res://images/atlases/compressed.sprites/map/map_circle_1.tres", "res://images/atlases/compressed.sprites/map/map_circle_2.tres", "res://images/atlases/compressed.sprites/map/map_circle_3.tres", "res://images/atlases/compressed.sprites/map/map_circle_4.tres" };

	private const double _animInterval = 1.0 / 24.0;

	private bool _playAnim;

	public static IEnumerable<string> AssetPaths => _textures.Append("res://scenes/vfx/map_circle_vfx.tscn");

	public override void _Ready()
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		_image = ((Node)this).GetNode<TextureRect>(NodePath.op_Implicit("TextureRect"));
		_image.Texture = PreloadManager.Cache.GetTexture2D(_textures[0]);
		((Control)this).RotationDegrees = Rng.Chaotic.NextFloat(360f);
		Vector2 val = Vector2.One * Rng.Chaotic.NextFloat(0.85f, 0.9f);
		if (_playAnim)
		{
			Tween val2 = ((Node)this).CreateTween().SetParallel(true);
			val2.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("scale"), Variant.op_Implicit(val), 1.0).SetEase((EaseType)1).SetTrans((TransitionType)5)
				.From(Variant.op_Implicit(val * 2f));
			val2.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0.95f), 0.5).SetEase((EaseType)1).SetTrans((TransitionType)5)
				.From(Variant.op_Implicit(0f));
			TaskHelper.RunSafely(AnimateSprite());
		}
		else
		{
			((Control)this).Scale = val;
			((CanvasItem)this).Modulate = new Color(((CanvasItem)this).Modulate, 0.95f);
			string path = _textures.Last();
			_image.Texture = PreloadManager.Cache.GetTexture2D(path);
		}
	}

	private async Task AnimateSprite()
	{
		string[] textures = _textures;
		foreach (string path in textures)
		{
			_image.Texture = PreloadManager.Cache.GetTexture2D(path);
			SceneTreeTimer val = ((Node)this).GetTree().CreateTimer(1.0 / 24.0, true, false, false);
			await ((GodotObject)this).ToSignal((GodotObject)(object)val, SignalName.Timeout);
		}
	}

	public static NMapCircleVfx? Create(bool playAnim)
	{
		if (TestMode.IsOn)
		{
			return null;
		}
		NMapCircleVfx nMapCircleVfx = PreloadManager.Cache.GetScene("res://scenes/vfx/map_circle_vfx.tscn").Instantiate<NMapCircleVfx>((GenEditState)0);
		nMapCircleVfx._playAnim = playAnim;
		return nMapCircleVfx;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Expected O, but got Unknown
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(2);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.Create, new PropertyInfo((Type)24, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false), (MethodFlags)33, new List<PropertyInfo>
		{
			new PropertyInfo((Type)1, StringName.op_Implicit("playAnim"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.Create && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			NMapCircleVfx nMapCircleVfx = Create(VariantUtils.ConvertTo<bool>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<NMapCircleVfx>(ref nMapCircleVfx);
			return true;
		}
		return ((Control)this).InvokeGodotClassMethod(ref method, args, ref ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static bool InvokeGodotClassStaticMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.Create && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			NMapCircleVfx nMapCircleVfx = Create(VariantUtils.ConvertTo<bool>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<NMapCircleVfx>(ref nMapCircleVfx);
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
		return ((Control)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._image)
		{
			_image = VariantUtils.ConvertTo<TextureRect>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._playAnim)
		{
			_playAnim = VariantUtils.ConvertTo<bool>(ref value);
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
		if ((ref name) == PropertyName._image)
		{
			value = VariantUtils.CreateFrom<TextureRect>(ref _image);
			return true;
		}
		if ((ref name) == PropertyName._playAnim)
		{
			value = VariantUtils.CreateFrom<bool>(ref _playAnim);
			return true;
		}
		return ((GodotObject)this).GetGodotClassPropertyValue(ref name, ref value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._image, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._playAnim, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		info.AddProperty(PropertyName._image, Variant.From<TextureRect>(ref _image));
		info.AddProperty(PropertyName._playAnim, Variant.From<bool>(ref _playAnim));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._image, ref val))
		{
			_image = ((Variant)(ref val)).As<TextureRect>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._playAnim, ref val2))
		{
			_playAnim = ((Variant)(ref val2)).As<bool>();
		}
	}
}
