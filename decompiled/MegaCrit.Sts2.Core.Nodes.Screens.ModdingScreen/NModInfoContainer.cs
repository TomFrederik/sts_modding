using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.Nodes.Screens.ModdingScreen;

[ScriptPath("res://src/Core/Nodes/Screens/ModdingScreen/NModInfoContainer.cs")]
public class NModInfoContainer : Control
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName _title = StringName.op_Implicit("_title");

		public static readonly StringName _image = StringName.op_Implicit("_image");

		public static readonly StringName _description = StringName.op_Implicit("_description");
	}

	public class SignalName : SignalName
	{
	}

	private MegaRichTextLabel _title;

	private TextureRect _image;

	private MegaRichTextLabel _description;

	public override void _Ready()
	{
		_title = ((Node)this).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("ModTitle"));
		_image = ((Node)this).GetNode<TextureRect>(NodePath.op_Implicit("ModImage"));
		_description = ((Node)this).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("ModDescription"));
		_title.Text = "";
		_image.Texture = null;
		_description.Text = "";
	}

	public void Fill(Mod mod)
	{
		if (mod.wasLoaded)
		{
			_title.Text = mod.manifest.name;
			_image.Texture = PreloadManager.Cache.GetAsset<Texture2D>("res://" + mod.pckName + "/mod_image.png");
			StringBuilder stringBuilder = new StringBuilder();
			StringBuilder stringBuilder2 = stringBuilder;
			StringBuilder stringBuilder3 = stringBuilder2;
			StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(21, 1, stringBuilder2);
			handler.AppendLiteral("[gold]Author[/gold]: ");
			handler.AppendFormatted(mod.manifest.author ?? "unknown");
			stringBuilder3.AppendLine(ref handler);
			stringBuilder2 = stringBuilder;
			StringBuilder stringBuilder4 = stringBuilder2;
			handler = new StringBuilder.AppendInterpolatedStringHandler(22, 1, stringBuilder2);
			handler.AppendLiteral("[gold]Version[/gold]: ");
			handler.AppendFormatted(mod.manifest.version ?? "unknown");
			stringBuilder4.AppendLine(ref handler);
			stringBuilder.AppendLine();
			stringBuilder2 = stringBuilder;
			StringBuilder stringBuilder5 = stringBuilder2;
			handler = new StringBuilder.AppendInterpolatedStringHandler(0, 1, stringBuilder2);
			handler.AppendFormatted(mod.manifest.description ?? "No description");
			stringBuilder5.AppendLine(ref handler);
			_description.Text = stringBuilder.ToString();
		}
		else
		{
			_title.Text = mod.pckName;
			_image.Texture = NModMenuRow.GetPlatformIcon(mod.modSource);
			_description.Text = new LocString("settings_ui", "MODDING_SCREEN.MOD_UNLOADED_DESCRIPTION").GetFormattedText();
		}
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
		return ((Control)this).InvokeGodotClassMethod(ref method, args, ref ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if ((ref method) == MethodName._Ready)
		{
			return true;
		}
		return ((Control)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._title)
		{
			_title = VariantUtils.ConvertTo<MegaRichTextLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._image)
		{
			_image = VariantUtils.ConvertTo<TextureRect>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._description)
		{
			_description = VariantUtils.ConvertTo<MegaRichTextLabel>(ref value);
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
		if ((ref name) == PropertyName._title)
		{
			value = VariantUtils.CreateFrom<MegaRichTextLabel>(ref _title);
			return true;
		}
		if ((ref name) == PropertyName._image)
		{
			value = VariantUtils.CreateFrom<TextureRect>(ref _image);
			return true;
		}
		if ((ref name) == PropertyName._description)
		{
			value = VariantUtils.CreateFrom<MegaRichTextLabel>(ref _description);
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
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._title, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._image, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._description, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		info.AddProperty(PropertyName._title, Variant.From<MegaRichTextLabel>(ref _title));
		info.AddProperty(PropertyName._image, Variant.From<TextureRect>(ref _image));
		info.AddProperty(PropertyName._description, Variant.From<MegaRichTextLabel>(ref _description));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._title, ref val))
		{
			_title = ((Variant)(ref val)).As<MegaRichTextLabel>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._image, ref val2))
		{
			_image = ((Variant)(ref val2)).As<TextureRect>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._description, ref val3))
		{
			_description = ((Variant)(ref val3)).As<MegaRichTextLabel>();
		}
	}
}
