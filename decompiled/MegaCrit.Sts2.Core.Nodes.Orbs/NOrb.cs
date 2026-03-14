using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Orbs;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.HoverTips;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.Nodes.Orbs;

[ScriptPath("res://src/Core/Nodes/Orbs/NOrb.cs")]
public class NOrb : NClickableControl
{
	public new class MethodName : NClickableControl.MethodName
	{
		public static readonly StringName Create = StringName.op_Implicit("Create");

		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName _EnterTree = StringName.op_Implicit("_EnterTree");

		public static readonly StringName _ExitTree = StringName.op_Implicit("_ExitTree");

		public static readonly StringName UpdateVisuals = StringName.op_Implicit("UpdateVisuals");

		public static readonly StringName Flash = StringName.op_Implicit("Flash");

		public new static readonly StringName OnFocus = StringName.op_Implicit("OnFocus");

		public new static readonly StringName OnUnfocus = StringName.op_Implicit("OnUnfocus");
	}

	public new class PropertyName : NClickableControl.PropertyName
	{
		public static readonly StringName _outline = StringName.op_Implicit("_outline");

		public static readonly StringName _visualContainer = StringName.op_Implicit("_visualContainer");

		public static readonly StringName _labelContainer = StringName.op_Implicit("_labelContainer");

		public static readonly StringName _passiveLabel = StringName.op_Implicit("_passiveLabel");

		public static readonly StringName _evokeLabel = StringName.op_Implicit("_evokeLabel");

		public static readonly StringName _bounds = StringName.op_Implicit("_bounds");

		public static readonly StringName _flashParticle = StringName.op_Implicit("_flashParticle");

		public static readonly StringName _selectionReticle = StringName.op_Implicit("_selectionReticle");

		public static readonly StringName _isLocal = StringName.op_Implicit("_isLocal");

		public static readonly StringName _sprite = StringName.op_Implicit("_sprite");

		public static readonly StringName _curTween = StringName.op_Implicit("_curTween");
	}

	public new class SignalName : NClickableControl.SignalName
	{
	}

	private TextureRect _outline;

	private Control _visualContainer;

	private Control _labelContainer;

	private MegaLabel _passiveLabel;

	private MegaLabel _evokeLabel;

	private Control _bounds;

	private CpuParticles2D _flashParticle;

	private NSelectionReticle _selectionReticle;

	private bool _isLocal;

	private Node2D? _sprite;

	private Tween? _curTween;

	public OrbModel? Model { get; private set; }

	private static string ScenePath => SceneHelper.GetScenePath("/orbs/orb");

	public static IEnumerable<string> AssetPaths => new _003C_003Ez__ReadOnlySingleElementList<string>(ScenePath);

	public static NOrb Create(bool isLocal)
	{
		NOrb nOrb = PreloadManager.Cache.GetScene(ScenePath).Instantiate<NOrb>((GenEditState)0);
		nOrb._isLocal = isLocal;
		return nOrb;
	}

	public static NOrb Create(bool isLocal, OrbModel? model)
	{
		NOrb nOrb = Create(isLocal);
		nOrb.Model = model;
		return nOrb;
	}

	public override void _Ready()
	{
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		ConnectSignals();
		_outline = ((Node)this).GetNode<TextureRect>(NodePath.op_Implicit("%Outline"));
		_visualContainer = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%VisualContainer"));
		_passiveLabel = ((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%PassiveAmount"));
		_evokeLabel = ((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%EvokeAmount"));
		_flashParticle = ((Node)this).GetNode<CpuParticles2D>(NodePath.op_Implicit("%Flash"));
		_bounds = ((Node)this).GetNode<Control>(NodePath.op_Implicit("Bounds"));
		_labelContainer = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%LabelContainer"));
		_selectionReticle = ((Node)this).GetNode<NSelectionReticle>(NodePath.op_Implicit("%SelectionReticle"));
		if (Model != null)
		{
			((Node)this).CreateTween().TweenProperty((GodotObject)(object)_outline, NodePath.op_Implicit("scale"), Variant.op_Implicit(Vector2.One), 0.25).From(Variant.op_Implicit(Vector2.Zero));
		}
		if (_isLocal)
		{
			((Control)this).Scale = ((Control)this).Scale * 0.85f;
		}
		UpdateVisuals(isEvoking: false);
	}

	public override void _EnterTree()
	{
		((Node)this)._EnterTree();
		if (Model != null)
		{
			Model.Triggered += Flash;
		}
	}

	public override void _ExitTree()
	{
		((Node)this)._ExitTree();
		if (Model != null)
		{
			Model.Triggered -= Flash;
		}
	}

	public void ReplaceOrb(OrbModel model)
	{
		((Node)(object)_sprite)?.QueueFreeSafely();
		_sprite = null;
		Model = model;
		UpdateVisuals(isEvoking: false);
	}

	public void UpdateVisuals(bool isEvoking)
	{
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		if (!((Node)this).IsNodeReady() || !CombatManager.Instance.IsInProgress)
		{
			return;
		}
		if (Model == null)
		{
			((Node)(object)_sprite)?.QueueFreeSafely();
			((CanvasItem)_passiveLabel).Visible = false;
			((CanvasItem)_evokeLabel).Visible = false;
			((CanvasItem)_outline).Visible = _isLocal;
			((CanvasItem)_flashParticle).Visible = false;
			return;
		}
		if (_sprite == null)
		{
			_sprite = Model.CreateSprite();
			((Node)(object)_visualContainer).AddChildSafely((Node?)(object)_sprite);
			_sprite.Position = Vector2.Zero;
			Tween? curTween = _curTween;
			if (curTween != null)
			{
				curTween.Kill();
			}
			_curTween = ((Node)this).CreateTween();
			_curTween.TweenProperty((GodotObject)(object)_sprite, NodePath.op_Implicit("scale"), Variant.op_Implicit(Vector2.One), 0.5).From(Variant.op_Implicit(Vector2.Zero)).SetTrans((TransitionType)10)
				.SetEase((EaseType)1);
		}
		((CanvasItem)_outline).Visible = false;
		((CanvasItem)_flashParticle).Visible = true;
		_flashParticle.Texture = (Texture2D)(object)Model.Icon;
		((CanvasItem)_labelContainer).Visible = _isLocal;
		if (!_isLocal)
		{
			((CanvasItem)this).Modulate = Model.DarkenedColor;
		}
		OrbModel model = Model;
		if (!(model is PlasmaOrb))
		{
			if (!(model is DarkOrb))
			{
				if (model is GlassOrb)
				{
					((CanvasItem)_passiveLabel).Visible = !isEvoking;
					((CanvasItem)_evokeLabel).Visible = isEvoking;
					((CanvasItem)_sprite).Modulate = ((Model.PassiveVal == 0m) ? Model.DarkenedColor : Colors.White);
					_passiveLabel.SetTextAutoSize(Model.PassiveVal.ToString("0"));
					_evokeLabel.SetTextAutoSize(Model.EvokeVal.ToString("0"));
				}
				else
				{
					((CanvasItem)_passiveLabel).Visible = !isEvoking;
					((CanvasItem)_evokeLabel).Visible = isEvoking;
					_passiveLabel.SetTextAutoSize(Model.PassiveVal.ToString("0"));
					_evokeLabel.SetTextAutoSize(Model.EvokeVal.ToString("0"));
				}
			}
			else
			{
				((CanvasItem)_passiveLabel).Visible = true;
				((CanvasItem)_evokeLabel).Visible = true;
				_passiveLabel.SetTextAutoSize(Model.PassiveVal.ToString("0"));
				_evokeLabel.SetTextAutoSize(Model.EvokeVal.ToString("0"));
			}
		}
		else
		{
			((CanvasItem)_passiveLabel).Visible = false;
			((CanvasItem)_evokeLabel).Visible = false;
		}
	}

	private void Flash()
	{
		_flashParticle.Emitting = true;
	}

	protected override void OnFocus()
	{
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		if (Model != null || _isLocal)
		{
			IEnumerable<IHoverTip> enumerable;
			if (Model != null)
			{
				enumerable = Model.HoverTips;
			}
			else
			{
				IEnumerable<IHoverTip> enumerable2 = new List<IHoverTip> { OrbModel.EmptySlotHoverTipHoverTip };
				enumerable = enumerable2;
			}
			IEnumerable<IHoverTip> hoverTips = enumerable;
			NHoverTipSet nHoverTipSet = NHoverTipSet.CreateAndShow(_bounds, hoverTips, HoverTip.GetHoverTipAlignment(_bounds));
			nHoverTipSet.SetFollowOwner();
			((CanvasItem)_labelContainer).Visible = true;
			((CanvasItem)this).Modulate = Colors.White;
			if (NControllerManager.Instance.IsUsingController)
			{
				_selectionReticle.OnSelect();
			}
		}
	}

	protected override void OnUnfocus()
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		((CanvasItem)_labelContainer).Visible = _isLocal;
		if (Model != null)
		{
			((CanvasItem)this).Modulate = (_isLocal ? Colors.White : Model.DarkenedColor);
		}
		NHoverTipSet.Remove(_bounds);
		_selectionReticle.OnDeselect();
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Expected O, but got Unknown
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(8);
		list.Add(new MethodInfo(MethodName.Create, new PropertyInfo((Type)24, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false), (MethodFlags)33, new List<PropertyInfo>
		{
			new PropertyInfo((Type)1, StringName.op_Implicit("isLocal"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._EnterTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._ExitTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UpdateVisuals, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)1, StringName.op_Implicit("isEvoking"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.Flash, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnFocus, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnUnfocus, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.Create && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			NOrb nOrb = Create(VariantUtils.ConvertTo<bool>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<NOrb>(ref nOrb);
			return true;
		}
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
		if ((ref method) == MethodName.UpdateVisuals && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			UpdateVisuals(VariantUtils.ConvertTo<bool>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.Flash && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			Flash();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnFocus && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnFocus();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnUnfocus && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnUnfocus();
			ret = default(godot_variant);
			return true;
		}
		return base.InvokeGodotClassMethod(in method, args, out ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static bool InvokeGodotClassStaticMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.Create && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			NOrb nOrb = Create(VariantUtils.ConvertTo<bool>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<NOrb>(ref nOrb);
			return true;
		}
		ret = default(godot_variant);
		return false;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if ((ref method) == MethodName.Create)
		{
			return true;
		}
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
		if ((ref method) == MethodName.UpdateVisuals)
		{
			return true;
		}
		if ((ref method) == MethodName.Flash)
		{
			return true;
		}
		if ((ref method) == MethodName.OnFocus)
		{
			return true;
		}
		if ((ref method) == MethodName.OnUnfocus)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._outline)
		{
			_outline = VariantUtils.ConvertTo<TextureRect>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._visualContainer)
		{
			_visualContainer = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._labelContainer)
		{
			_labelContainer = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._passiveLabel)
		{
			_passiveLabel = VariantUtils.ConvertTo<MegaLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._evokeLabel)
		{
			_evokeLabel = VariantUtils.ConvertTo<MegaLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._bounds)
		{
			_bounds = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._flashParticle)
		{
			_flashParticle = VariantUtils.ConvertTo<CpuParticles2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._selectionReticle)
		{
			_selectionReticle = VariantUtils.ConvertTo<NSelectionReticle>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._isLocal)
		{
			_isLocal = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._sprite)
		{
			_sprite = VariantUtils.ConvertTo<Node2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._curTween)
		{
			_curTween = VariantUtils.ConvertTo<Tween>(ref value);
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
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName._outline)
		{
			value = VariantUtils.CreateFrom<TextureRect>(ref _outline);
			return true;
		}
		if ((ref name) == PropertyName._visualContainer)
		{
			value = VariantUtils.CreateFrom<Control>(ref _visualContainer);
			return true;
		}
		if ((ref name) == PropertyName._labelContainer)
		{
			value = VariantUtils.CreateFrom<Control>(ref _labelContainer);
			return true;
		}
		if ((ref name) == PropertyName._passiveLabel)
		{
			value = VariantUtils.CreateFrom<MegaLabel>(ref _passiveLabel);
			return true;
		}
		if ((ref name) == PropertyName._evokeLabel)
		{
			value = VariantUtils.CreateFrom<MegaLabel>(ref _evokeLabel);
			return true;
		}
		if ((ref name) == PropertyName._bounds)
		{
			value = VariantUtils.CreateFrom<Control>(ref _bounds);
			return true;
		}
		if ((ref name) == PropertyName._flashParticle)
		{
			value = VariantUtils.CreateFrom<CpuParticles2D>(ref _flashParticle);
			return true;
		}
		if ((ref name) == PropertyName._selectionReticle)
		{
			value = VariantUtils.CreateFrom<NSelectionReticle>(ref _selectionReticle);
			return true;
		}
		if ((ref name) == PropertyName._isLocal)
		{
			value = VariantUtils.CreateFrom<bool>(ref _isLocal);
			return true;
		}
		if ((ref name) == PropertyName._sprite)
		{
			value = VariantUtils.CreateFrom<Node2D>(ref _sprite);
			return true;
		}
		if ((ref name) == PropertyName._curTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _curTween);
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
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._outline, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._visualContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._labelContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._passiveLabel, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._evokeLabel, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._bounds, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._flashParticle, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._selectionReticle, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._isLocal, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._sprite, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._curTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName._outline, Variant.From<TextureRect>(ref _outline));
		info.AddProperty(PropertyName._visualContainer, Variant.From<Control>(ref _visualContainer));
		info.AddProperty(PropertyName._labelContainer, Variant.From<Control>(ref _labelContainer));
		info.AddProperty(PropertyName._passiveLabel, Variant.From<MegaLabel>(ref _passiveLabel));
		info.AddProperty(PropertyName._evokeLabel, Variant.From<MegaLabel>(ref _evokeLabel));
		info.AddProperty(PropertyName._bounds, Variant.From<Control>(ref _bounds));
		info.AddProperty(PropertyName._flashParticle, Variant.From<CpuParticles2D>(ref _flashParticle));
		info.AddProperty(PropertyName._selectionReticle, Variant.From<NSelectionReticle>(ref _selectionReticle));
		info.AddProperty(PropertyName._isLocal, Variant.From<bool>(ref _isLocal));
		info.AddProperty(PropertyName._sprite, Variant.From<Node2D>(ref _sprite));
		info.AddProperty(PropertyName._curTween, Variant.From<Tween>(ref _curTween));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._outline, ref val))
		{
			_outline = ((Variant)(ref val)).As<TextureRect>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._visualContainer, ref val2))
		{
			_visualContainer = ((Variant)(ref val2)).As<Control>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._labelContainer, ref val3))
		{
			_labelContainer = ((Variant)(ref val3)).As<Control>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._passiveLabel, ref val4))
		{
			_passiveLabel = ((Variant)(ref val4)).As<MegaLabel>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._evokeLabel, ref val5))
		{
			_evokeLabel = ((Variant)(ref val5)).As<MegaLabel>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._bounds, ref val6))
		{
			_bounds = ((Variant)(ref val6)).As<Control>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName._flashParticle, ref val7))
		{
			_flashParticle = ((Variant)(ref val7)).As<CpuParticles2D>();
		}
		Variant val8 = default(Variant);
		if (info.TryGetProperty(PropertyName._selectionReticle, ref val8))
		{
			_selectionReticle = ((Variant)(ref val8)).As<NSelectionReticle>();
		}
		Variant val9 = default(Variant);
		if (info.TryGetProperty(PropertyName._isLocal, ref val9))
		{
			_isLocal = ((Variant)(ref val9)).As<bool>();
		}
		Variant val10 = default(Variant);
		if (info.TryGetProperty(PropertyName._sprite, ref val10))
		{
			_sprite = ((Variant)(ref val10)).As<Node2D>();
		}
		Variant val11 = default(Variant);
		if (info.TryGetProperty(PropertyName._curTween, ref val11))
		{
			_curTween = ((Variant)(ref val11)).As<Tween>();
		}
	}
}
