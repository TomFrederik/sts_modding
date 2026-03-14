using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Events.Custom.CrystalSphereEvent;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.TestSupport;

namespace MegaCrit.Sts2.Core.Nodes.Events.Custom.CrystalSphere;

[ScriptPath("res://src/Core/Nodes/Events/Custom/CrystalSphere/NCrystalSphereCell.cs")]
public class NCrystalSphereCell : NClickableControl
{
	public new class MethodName : NClickableControl.MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName _EnterTree = StringName.op_Implicit("_EnterTree");

		public static readonly StringName _ExitTree = StringName.op_Implicit("_ExitTree");

		public static readonly StringName OnEntityHighlightUpdated = StringName.op_Implicit("OnEntityHighlightUpdated");

		public static readonly StringName EntityClicked = StringName.op_Implicit("EntityClicked");
	}

	public new class PropertyName : NClickableControl.PropertyName
	{
		public static readonly StringName _mask = StringName.op_Implicit("_mask");

		public static readonly StringName _hoveredFg = StringName.op_Implicit("_hoveredFg");

		public static readonly StringName _fadeTween = StringName.op_Implicit("_fadeTween");
	}

	public new class SignalName : NClickableControl.SignalName
	{
	}

	private const string _scenePath = "res://scenes/events/custom/crystal_sphere/crystal_sphere_cell.tscn";

	private NCrystalSphereMask _mask;

	private Control _hoveredFg;

	private Tween? _fadeTween;

	public CrystalSphereCell Entity { get; private set; }

	public static NCrystalSphereCell? Create(CrystalSphereCell cell, NCrystalSphereMask mask)
	{
		if (TestMode.IsOn)
		{
			return null;
		}
		NCrystalSphereCell nCrystalSphereCell = PreloadManager.Cache.GetScene("res://scenes/events/custom/crystal_sphere/crystal_sphere_cell.tscn").Instantiate<NCrystalSphereCell>((GenEditState)0);
		nCrystalSphereCell.Entity = cell;
		nCrystalSphereCell._mask = mask;
		return nCrystalSphereCell;
	}

	public override void _Ready()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		_hoveredFg = ((Node)this).GetNode<Control>(NodePath.op_Implicit("HoveredFg"));
		((CanvasItem)this).Modulate = Colors.Transparent;
		((Control)this).MouseFilter = (MouseFilterEnum)(Entity.IsHidden ? 0 : 2);
		((Control)this).FocusMode = (FocusModeEnum)(Entity.IsHidden ? 2 : 0);
		((CanvasItem)_hoveredFg).Visible = false;
	}

	public override void _EnterTree()
	{
		Entity.HighlightUpdated += OnEntityHighlightUpdated;
		Entity.FogUpdated += EntityClicked;
	}

	public override void _ExitTree()
	{
		Entity.HighlightUpdated -= OnEntityHighlightUpdated;
		Entity.FogUpdated -= EntityClicked;
	}

	private void OnEntityHighlightUpdated()
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		Tween? fadeTween = _fadeTween;
		if (fadeTween != null)
		{
			fadeTween.Kill();
		}
		_fadeTween = ((Node)this).CreateTween();
		Tween? fadeTween2 = _fadeTween;
		NodePath obj = NodePath.op_Implicit("modulate");
		CrystalSphereCell entity = Entity;
		fadeTween2.TweenProperty((GodotObject)(object)this, obj, Variant.op_Implicit((entity != null && entity.IsHighlighted && entity.IsHidden) ? Colors.White : Colors.Transparent), 0.15000000596046448);
		((CanvasItem)_hoveredFg).Visible = Entity.IsHovered;
	}

	private void EntityClicked()
	{
		((Control)this).MouseFilter = (MouseFilterEnum)(Entity.IsHidden ? 0 : 2);
		((Control)this).FocusMode = (FocusModeEnum)(Entity.IsHidden ? 2 : 0);
		_mask.UpdateMat(Entity);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<MethodInfo> GetGodotMethodList()
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
		List<MethodInfo> list = new List<MethodInfo>(5);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._EnterTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._ExitTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnEntityHighlightUpdated, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.EntityClicked, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._EnterTree && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._EnterTree();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._ExitTree && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._ExitTree();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnEntityHighlightUpdated && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnEntityHighlightUpdated();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.EntityClicked && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			EntityClicked();
			ret = default(godot_variant);
			return true;
		}
		return base.InvokeGodotClassMethod(in method, args, out ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if ((ref method) == MethodName._Ready)
		{
			return true;
		}
		if ((ref method) == MethodName._EnterTree)
		{
			return true;
		}
		if ((ref method) == MethodName._ExitTree)
		{
			return true;
		}
		if ((ref method) == MethodName.OnEntityHighlightUpdated)
		{
			return true;
		}
		if ((ref method) == MethodName.EntityClicked)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._mask)
		{
			_mask = VariantUtils.ConvertTo<NCrystalSphereMask>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._hoveredFg)
		{
			_hoveredFg = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._fadeTween)
		{
			_fadeTween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		return base.SetGodotClassPropertyValue(in name, in value);
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
		if ((ref name) == PropertyName._mask)
		{
			value = VariantUtils.CreateFrom<NCrystalSphereMask>(ref _mask);
			return true;
		}
		if ((ref name) == PropertyName._hoveredFg)
		{
			value = VariantUtils.CreateFrom<Control>(ref _hoveredFg);
			return true;
		}
		if ((ref name) == PropertyName._fadeTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _fadeTween);
			return true;
		}
		return base.GetGodotClassPropertyValue(in name, out value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._mask, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._hoveredFg, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._fadeTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName._mask, Variant.From<NCrystalSphereMask>(ref _mask));
		info.AddProperty(PropertyName._hoveredFg, Variant.From<Control>(ref _hoveredFg));
		info.AddProperty(PropertyName._fadeTween, Variant.From<Tween>(ref _fadeTween));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._mask, ref val))
		{
			_mask = ((Variant)(ref val)).As<NCrystalSphereMask>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._hoveredFg, ref val2))
		{
			_hoveredFg = ((Variant)(ref val2)).As<Control>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._fadeTween, ref val3))
		{
			_fadeTween = ((Variant)(ref val3)).As<Tween>();
		}
	}
}
