using System;
using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;

namespace MegaCrit.Sts2.Core.Nodes.Screens.Map;

[ScriptPath("res://src/Core/Nodes/Screens/Map/NMapBg.cs")]
public class NMapBg : VBoxContainer
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName OnVisibilityChanged = StringName.op_Implicit("OnVisibilityChanged");

		public static readonly StringName OnWindowChange = StringName.op_Implicit("OnWindowChange");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName _mapTop = StringName.op_Implicit("_mapTop");

		public static readonly StringName _mapMid = StringName.op_Implicit("_mapMid");

		public static readonly StringName _mapBot = StringName.op_Implicit("_mapBot");

		public static readonly StringName _drawings = StringName.op_Implicit("_drawings");

		public static readonly StringName _window = StringName.op_Implicit("_window");

		public static readonly StringName _offsetX = StringName.op_Implicit("_offsetX");
	}

	public class SignalName : SignalName
	{
	}

	private IRunState _runState;

	private TextureRect _mapTop;

	private TextureRect _mapMid;

	private TextureRect _mapBot;

	private NMapDrawings _drawings;

	private Window _window;

	private const float _sixteenByNine = 1.7777778f;

	private const float _fourByThree = 1.3333334f;

	private const float _defaultY = -1620f;

	private const float _adjustY = -1540f;

	private float _offsetX;

	public override void _Ready()
	{
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		_mapTop = ((Node)this).GetNode<TextureRect>(NodePath.op_Implicit("MapTop"));
		_mapMid = ((Node)this).GetNode<TextureRect>(NodePath.op_Implicit("MapMid"));
		_mapBot = ((Node)this).GetNode<TextureRect>(NodePath.op_Implicit("MapBot"));
		_drawings = ((Node)this).GetNode<NMapDrawings>(NodePath.op_Implicit("%Drawings"));
		_window = ((Node)this).GetTree().Root;
		((GodotObject)_window).Connect(SignalName.SizeChanged, Callable.From((Action)OnWindowChange), 0u);
		OnWindowChange();
		_offsetX = ((Control)this).Position.X;
		((GodotObject)this).Connect(SignalName.VisibilityChanged, Callable.From((Action)OnVisibilityChanged), 0u);
	}

	public void Initialize(IRunState runState)
	{
		_runState = runState;
	}

	private void OnVisibilityChanged()
	{
		ActModel act = _runState.Act;
		_mapTop.Texture = act.MapTopBg;
		_mapMid.Texture = act.MapMidBg;
		_mapBot.Texture = act.MapBotBg;
	}

	private void OnWindowChange()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		float num = Math.Max(1.3333334f, (float)_window.Size.X / (float)_window.Size.Y);
		if (num < 1.7777778f)
		{
			float p = (num - 1.3333334f) / 0.44444442f;
			((Control)this).Position = new Vector2(_offsetX, Mathf.Remap(Ease.CubicOut(p), 0f, 1f, -1540f, -1620f));
		}
		else
		{
			((Control)this).Position = new Vector2(_offsetX, -1620f);
		}
		_drawings.RepositionBasedOnBackground((Control)(object)this);
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
		List<MethodInfo> list = new List<MethodInfo>(3);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnVisibilityChanged, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnWindowChange, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnVisibilityChanged && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnVisibilityChanged();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnWindowChange && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnWindowChange();
			ret = default(godot_variant);
			return true;
		}
		return ((VBoxContainer)this).InvokeGodotClassMethod(ref method, args, ref ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if ((ref method) == MethodName._Ready)
		{
			return true;
		}
		if ((ref method) == MethodName.OnVisibilityChanged)
		{
			return true;
		}
		if ((ref method) == MethodName.OnWindowChange)
		{
			return true;
		}
		return ((VBoxContainer)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._mapTop)
		{
			_mapTop = VariantUtils.ConvertTo<TextureRect>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._mapMid)
		{
			_mapMid = VariantUtils.ConvertTo<TextureRect>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._mapBot)
		{
			_mapBot = VariantUtils.ConvertTo<TextureRect>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._drawings)
		{
			_drawings = VariantUtils.ConvertTo<NMapDrawings>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._window)
		{
			_window = VariantUtils.ConvertTo<Window>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._offsetX)
		{
			_offsetX = VariantUtils.ConvertTo<float>(ref value);
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
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName._mapTop)
		{
			value = VariantUtils.CreateFrom<TextureRect>(ref _mapTop);
			return true;
		}
		if ((ref name) == PropertyName._mapMid)
		{
			value = VariantUtils.CreateFrom<TextureRect>(ref _mapMid);
			return true;
		}
		if ((ref name) == PropertyName._mapBot)
		{
			value = VariantUtils.CreateFrom<TextureRect>(ref _mapBot);
			return true;
		}
		if ((ref name) == PropertyName._drawings)
		{
			value = VariantUtils.CreateFrom<NMapDrawings>(ref _drawings);
			return true;
		}
		if ((ref name) == PropertyName._window)
		{
			value = VariantUtils.CreateFrom<Window>(ref _window);
			return true;
		}
		if ((ref name) == PropertyName._offsetX)
		{
			value = VariantUtils.CreateFrom<float>(ref _offsetX);
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
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._mapTop, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._mapMid, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._mapBot, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._drawings, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._window, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)3, PropertyName._offsetX, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		info.AddProperty(PropertyName._mapTop, Variant.From<TextureRect>(ref _mapTop));
		info.AddProperty(PropertyName._mapMid, Variant.From<TextureRect>(ref _mapMid));
		info.AddProperty(PropertyName._mapBot, Variant.From<TextureRect>(ref _mapBot));
		info.AddProperty(PropertyName._drawings, Variant.From<NMapDrawings>(ref _drawings));
		info.AddProperty(PropertyName._window, Variant.From<Window>(ref _window));
		info.AddProperty(PropertyName._offsetX, Variant.From<float>(ref _offsetX));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._mapTop, ref val))
		{
			_mapTop = ((Variant)(ref val)).As<TextureRect>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._mapMid, ref val2))
		{
			_mapMid = ((Variant)(ref val2)).As<TextureRect>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._mapBot, ref val3))
		{
			_mapBot = ((Variant)(ref val3)).As<TextureRect>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._drawings, ref val4))
		{
			_drawings = ((Variant)(ref val4)).As<NMapDrawings>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._window, ref val5))
		{
			_window = ((Variant)(ref val5)).As<Window>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._offsetX, ref val6))
		{
			_offsetX = ((Variant)(ref val6)).As<float>();
		}
	}
}
