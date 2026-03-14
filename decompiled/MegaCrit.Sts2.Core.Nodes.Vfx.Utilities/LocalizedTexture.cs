using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.Collections;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Saves;

namespace MegaCrit.Sts2.Core.Nodes.Vfx.Utilities;

[GlobalClass]
[ScriptPath("res://src/Core/Nodes/Vfx/Utilities/LocalizedTexture.cs")]
public class LocalizedTexture : Resource
{
	public class MethodName : MethodName
	{
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName _textures = StringName.op_Implicit("_textures");
	}

	public class SignalName : SignalName
	{
	}

	[Export(/*Could not decode attribute arguments.*/)]
	private Dictionary<string, Texture2D> _textures = new Dictionary<string, Texture2D>();

	public bool TryGetTexture(out Texture2D? texture)
	{
		texture = null;
		if (SaveManager.Instance.SettingsSave == null)
		{
			return false;
		}
		string language = SaveManager.Instance.SettingsSave.Language;
		if (string.IsNullOrEmpty(language))
		{
			return false;
		}
		Texture2D val = default(Texture2D);
		if (!_textures.TryGetValue(language, ref val))
		{
			return false;
		}
		texture = val;
		return true;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._textures)
		{
			_textures = VariantUtils.ConvertToDictionary<string, Texture2D>(ref value);
			return true;
		}
		return ((GodotObject)this).SetGodotClassPropertyValue(ref name, ref value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName._textures)
		{
			value = VariantUtils.CreateFromDictionary<string, Texture2D>(_textures);
			return true;
		}
		return ((GodotObject)this).GetGodotClassPropertyValue(ref name, ref value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)27, PropertyName._textures, (PropertyHint)23, "4/0:;24/17:Texture2D", (PropertyUsageFlags)4102, true));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		info.AddProperty(PropertyName._textures, Variant.CreateFrom<string, Texture2D>(_textures));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._textures, ref val))
		{
			_textures = ((Variant)(ref val)).AsGodotDictionary<string, Texture2D>();
		}
	}
}
