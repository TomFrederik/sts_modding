using System;
using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;

namespace MegaCrit.Sts2.Core.Nodes.Screens.Settings;

[ScriptPath("res://src/Core/Nodes/Screens/Settings/NSettingsGradientMask.cs")]
public class NSettingsGradientMask : TextureRect
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName OnResized = StringName.op_Implicit("OnResized");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName _tabContainer = StringName.op_Implicit("_tabContainer");

		public static readonly StringName _texture = StringName.op_Implicit("_texture");
	}

	public class SignalName : SignalName
	{
	}

	private const float _fadeOffset = -8f;

	private const float _fadeSize = 16f;

	private NSettingsTabManager _tabContainer;

	private GradientTexture2D _texture;

	public override void _Ready()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Expected O, but got Unknown
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		NSettingsScreen ancestorOfType = ((Node)(object)this).GetAncestorOfType<NSettingsScreen>();
		_tabContainer = ((Node)ancestorOfType).GetNode<NSettingsTabManager>(NodePath.op_Implicit("%SettingsTabManager"));
		_texture = (GradientTexture2D)((TextureRect)this).Texture;
		((GodotObject)this).Connect(SignalName.Resized, Callable.From((Action)OnResized), 0u);
		OnResized();
	}

	private void OnResized()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		float num = 1f - (((Control)_tabContainer).Position.Y + ((Control)_tabContainer).Size.Y + -8f + 16f) / ((Control)this).Size.Y;
		float num2 = num + 16f / ((Control)this).Size.Y;
		_texture.Gradient.SetOffset(2, num);
		_texture.Gradient.SetOffset(3, num2);
		_texture.Gradient.SetColor(2, Colors.White);
		_texture.Gradient.SetColor(3, StsColors.transparentWhite);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(2);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnResized, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnResized && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnResized();
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
		if ((ref method) == MethodName.OnResized)
		{
			return true;
		}
		return ((TextureRect)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._tabContainer)
		{
			_tabContainer = VariantUtils.ConvertTo<NSettingsTabManager>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._texture)
		{
			_texture = VariantUtils.ConvertTo<GradientTexture2D>(ref value);
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
		if ((ref name) == PropertyName._tabContainer)
		{
			value = VariantUtils.CreateFrom<NSettingsTabManager>(ref _tabContainer);
			return true;
		}
		if ((ref name) == PropertyName._texture)
		{
			value = VariantUtils.CreateFrom<GradientTexture2D>(ref _texture);
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
		list.Add(new PropertyInfo((Type)24, PropertyName._tabContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._texture, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		info.AddProperty(PropertyName._tabContainer, Variant.From<NSettingsTabManager>(ref _tabContainer));
		info.AddProperty(PropertyName._texture, Variant.From<GradientTexture2D>(ref _texture));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._tabContainer, ref val))
		{
			_tabContainer = ((Variant)(ref val)).As<NSettingsTabManager>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._texture, ref val2))
		{
			_texture = ((Variant)(ref val2)).As<GradientTexture2D>();
		}
	}
}
