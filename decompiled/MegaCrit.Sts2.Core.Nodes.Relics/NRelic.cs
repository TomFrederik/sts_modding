using System;
using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.TestSupport;

namespace MegaCrit.Sts2.Core.Nodes.Relics;

[ScriptPath("res://src/Core/Nodes/Relics/NRelic.cs")]
public class NRelic : Control
{
	public enum IconSize
	{
		Small,
		Large
	}

	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName Reload = StringName.op_Implicit("Reload");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName Icon = StringName.op_Implicit("Icon");

		public static readonly StringName Outline = StringName.op_Implicit("Outline");

		public static readonly StringName _iconSize = StringName.op_Implicit("_iconSize");
	}

	public class SignalName : SignalName
	{
	}

	public const string relicMatPath = "res://materials/ui/relic_mat.tres";

	private static readonly string _scenePath = SceneHelper.GetScenePath("relics/relic");

	private RelicModel? _model;

	private IconSize _iconSize;

	public static IEnumerable<string> AssetPaths => new global::_003C_003Ez__ReadOnlyArray<string>(new string[2] { "res://materials/ui/relic_mat.tres", _scenePath });

	public TextureRect Icon { get; private set; }

	public TextureRect Outline { get; private set; }

	public RelicModel Model
	{
		get
		{
			return _model ?? throw new InvalidOperationException("Model was accessed before it was set.");
		}
		set
		{
			if (_model != value)
			{
				RelicModel model = _model;
				_model = value;
				this.ModelChanged?.Invoke(model, _model);
			}
			Reload();
		}
	}

	public event Action<RelicModel?, RelicModel?>? ModelChanged;

	public static NRelic? Create(RelicModel relic, IconSize iconSize)
	{
		if (TestMode.IsOn)
		{
			return null;
		}
		NRelic nRelic = PreloadManager.Cache.GetScene(_scenePath).Instantiate<NRelic>((GenEditState)0);
		((Node)nRelic).Name = StringName.op_Implicit($"NRelic-{relic.Id}");
		nRelic.Model = relic;
		nRelic._iconSize = iconSize;
		return nRelic;
	}

	public override void _Ready()
	{
		Icon = ((Node)this).GetNode<TextureRect>(NodePath.op_Implicit("%Icon"));
		Outline = ((Node)this).GetNode<TextureRect>(NodePath.op_Implicit("%Outline"));
		Reload();
	}

	private void Reload()
	{
		if (((Node)this).IsNodeReady() && _model != null)
		{
			Model.UpdateTexture(Icon);
			switch (_iconSize)
			{
			case IconSize.Small:
				Icon.Texture = Model.Icon;
				((CanvasItem)Outline).Visible = true;
				Outline.Texture = Model.IconOutline;
				break;
			case IconSize.Large:
				Icon.Texture = Model.BigIcon;
				((CanvasItem)Outline).Visible = false;
				break;
			default:
				throw new ArgumentOutOfRangeException("_iconSize", _iconSize, null);
			}
		}
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
		list.Add(new MethodInfo(MethodName.Reload, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
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
		if ((ref method) == MethodName.Reload && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			Reload();
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
		return ((Control)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName.Icon)
		{
			Icon = VariantUtils.ConvertTo<TextureRect>(ref value);
			return true;
		}
		if ((ref name) == PropertyName.Outline)
		{
			Outline = VariantUtils.ConvertTo<TextureRect>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._iconSize)
		{
			_iconSize = VariantUtils.ConvertTo<IconSize>(ref value);
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
		if ((ref name) == PropertyName.Icon)
		{
			TextureRect icon = Icon;
			value = VariantUtils.CreateFrom<TextureRect>(ref icon);
			return true;
		}
		if ((ref name) == PropertyName.Outline)
		{
			TextureRect icon = Outline;
			value = VariantUtils.CreateFrom<TextureRect>(ref icon);
			return true;
		}
		if ((ref name) == PropertyName._iconSize)
		{
			value = VariantUtils.CreateFrom<IconSize>(ref _iconSize);
			return true;
		}
		return ((GodotObject)this).GetGodotClassPropertyValue(ref name, ref value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName.Icon, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.Outline, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)2, PropertyName._iconSize, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		StringName icon = PropertyName.Icon;
		TextureRect icon2 = Icon;
		info.AddProperty(icon, Variant.From<TextureRect>(ref icon2));
		StringName outline = PropertyName.Outline;
		icon2 = Outline;
		info.AddProperty(outline, Variant.From<TextureRect>(ref icon2));
		info.AddProperty(PropertyName._iconSize, Variant.From<IconSize>(ref _iconSize));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName.Icon, ref val))
		{
			Icon = ((Variant)(ref val)).As<TextureRect>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName.Outline, ref val2))
		{
			Outline = ((Variant)(ref val2)).As<TextureRect>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._iconSize, ref val3))
		{
			_iconSize = ((Variant)(ref val3)).As<IconSize>();
		}
	}
}
