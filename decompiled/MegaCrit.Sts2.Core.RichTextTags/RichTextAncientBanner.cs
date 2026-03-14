using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;

namespace MegaCrit.Sts2.Core.RichTextTags;

[GlobalClass]
[Tool]
[ScriptPath("res://src/Core/RichTextTags/RichTextAncientBanner.cs")]
public class RichTextAncientBanner : AbstractMegaRichTextEffect
{
	public new class MethodName : AbstractMegaRichTextEffect.MethodName
	{
		public static readonly StringName _ProcessCustomFX = StringName.op_Implicit("_ProcessCustomFX");
	}

	public new class PropertyName : AbstractMegaRichTextEffect.PropertyName
	{
		public static readonly StringName Rotation = StringName.op_Implicit("Rotation");

		public static readonly StringName Spacing = StringName.op_Implicit("Spacing");

		public static readonly StringName CenterCharacter = StringName.op_Implicit("CenterCharacter");

		public new static readonly StringName Bbcode = StringName.op_Implicit("Bbcode");

		public new static readonly StringName bbcode = StringName.op_Implicit("bbcode");
	}

	public new class SignalName : AbstractMegaRichTextEffect.SignalName
	{
	}

	public new string bbcode = "ancient_banner";

	public float Rotation { get; set; }

	public float Spacing { get; set; }

	public float CenterCharacter { get; set; }

	protected override string Bbcode => bbcode;

	public override bool _ProcessCustomFX(CharFXTransform charFx)
	{
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		if (ShouldTransformText())
		{
			float num = (float)charFx.RelativeIndex + 0.5f - CenterCharacter;
			Transform2D transform = charFx.Transform;
			Vector2 x = charFx.Transform.X;
			x.X = Rotation;
			transform.X = x;
			charFx.Transform = transform;
			transform = charFx.Transform;
			x = charFx.Transform.Origin;
			x.X = charFx.Transform.Origin.X + num * Spacing;
			transform.Origin = x;
			charFx.Transform = transform;
		}
		else
		{
			double num2 = charFx.ElapsedTime * 3.0 - (double)((float)charFx.RelativeIndex * 0.015f);
			Color color = charFx.Color;
			color.A = Mathf.Clamp((float)num2, 0f, 1f);
			charFx.Color = color;
		}
		return true;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Expected O, but got Unknown
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(1);
		list.Add(new MethodInfo(MethodName._ProcessCustomFX, new PropertyInfo((Type)1, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("charFx"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("CharFXTransform"), false)
		}, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._ProcessCustomFX && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			bool flag = ((RichTextEffect)this)._ProcessCustomFX(VariantUtils.ConvertTo<CharFXTransform>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<bool>(ref flag);
			return true;
		}
		return base.InvokeGodotClassMethod(in method, args, out ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if ((ref method) == MethodName._ProcessCustomFX)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName.Rotation)
		{
			Rotation = VariantUtils.ConvertTo<float>(ref value);
			return true;
		}
		if ((ref name) == PropertyName.Spacing)
		{
			Spacing = VariantUtils.ConvertTo<float>(ref value);
			return true;
		}
		if ((ref name) == PropertyName.CenterCharacter)
		{
			CenterCharacter = VariantUtils.ConvertTo<float>(ref value);
			return true;
		}
		if ((ref name) == PropertyName.bbcode)
		{
			bbcode = VariantUtils.ConvertTo<string>(ref value);
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
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName.Rotation)
		{
			float rotation = Rotation;
			value = VariantUtils.CreateFrom<float>(ref rotation);
			return true;
		}
		if ((ref name) == PropertyName.Spacing)
		{
			float rotation = Spacing;
			value = VariantUtils.CreateFrom<float>(ref rotation);
			return true;
		}
		if ((ref name) == PropertyName.CenterCharacter)
		{
			float rotation = CenterCharacter;
			value = VariantUtils.CreateFrom<float>(ref rotation);
			return true;
		}
		if ((ref name) == PropertyName.Bbcode)
		{
			string text = Bbcode;
			value = VariantUtils.CreateFrom<string>(ref text);
			return true;
		}
		if ((ref name) == PropertyName.bbcode)
		{
			value = VariantUtils.CreateFrom<string>(ref bbcode);
			return true;
		}
		return base.GetGodotClassPropertyValue(in name, out value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)3, PropertyName.Rotation, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)3, PropertyName.Spacing, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)3, PropertyName.CenterCharacter, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)4, PropertyName.bbcode, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)4, PropertyName.Bbcode, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		base.SaveGodotObjectData(info);
		StringName rotation = PropertyName.Rotation;
		float rotation2 = Rotation;
		info.AddProperty(rotation, Variant.From<float>(ref rotation2));
		StringName spacing = PropertyName.Spacing;
		rotation2 = Spacing;
		info.AddProperty(spacing, Variant.From<float>(ref rotation2));
		StringName centerCharacter = PropertyName.CenterCharacter;
		rotation2 = CenterCharacter;
		info.AddProperty(centerCharacter, Variant.From<float>(ref rotation2));
		info.AddProperty(PropertyName.bbcode, Variant.From<string>(ref bbcode));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName.Rotation, ref val))
		{
			Rotation = ((Variant)(ref val)).As<float>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName.Spacing, ref val2))
		{
			Spacing = ((Variant)(ref val2)).As<float>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName.CenterCharacter, ref val3))
		{
			CenterCharacter = ((Variant)(ref val3)).As<float>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName.bbcode, ref val4))
		{
			bbcode = ((Variant)(ref val4)).As<string>();
		}
	}
}
