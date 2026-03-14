using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.HoverTips;
using MegaCrit.Sts2.Core.TestSupport;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.Nodes.Combat;

[ScriptPath("res://src/Core/Nodes/Combat/NEnergyCounter.cs")]
public class NEnergyCounter : Control
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName _EnterTree = StringName.op_Implicit("_EnterTree");

		public static readonly StringName _ExitTree = StringName.op_Implicit("_ExitTree");

		public static readonly StringName OnHovered = StringName.op_Implicit("OnHovered");

		public static readonly StringName OnUnhovered = StringName.op_Implicit("OnUnhovered");

		public static readonly StringName RefreshLabel = StringName.op_Implicit("RefreshLabel");

		public static readonly StringName OnEnergyChanged = StringName.op_Implicit("OnEnergyChanged");

		public static readonly StringName _Process = StringName.op_Implicit("_Process");

		public static readonly StringName AnimIn = StringName.op_Implicit("AnimIn");

		public static readonly StringName AnimOut = StringName.op_Implicit("AnimOut");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName OutlineColor = StringName.op_Implicit("OutlineColor");

		public static readonly StringName _label = StringName.op_Implicit("_label");

		public static readonly StringName _layers = StringName.op_Implicit("_layers");

		public static readonly StringName _rotationLayers = StringName.op_Implicit("_rotationLayers");

		public static readonly StringName _backParticles = StringName.op_Implicit("_backParticles");

		public static readonly StringName _frontParticles = StringName.op_Implicit("_frontParticles");

		public static readonly StringName _animInTween = StringName.op_Implicit("_animInTween");

		public static readonly StringName _animOutTween = StringName.op_Implicit("_animOutTween");
	}

	public class SignalName : SignalName
	{
	}

	private const string _darkenedMatPath = "res://materials/ui/energy_orb_dark.tres";

	private Player _player;

	private MegaLabel _label;

	private Control _layers;

	private Control _rotationLayers;

	private CpuParticles2D _backParticles;

	private CpuParticles2D _frontParticles;

	private HoverTip _hoverTip;

	private Tween? _animInTween;

	private Tween? _animOutTween;

	private const float _animDuration = 0.6f;

	private static readonly Vector2 _showPosition = Vector2.Zero;

	private static readonly Vector2 _hidePosition = new Vector2(-480f, 128f);

	public static IEnumerable<string> AssetPaths => new _003C_003Ez__ReadOnlySingleElementList<string>("res://materials/ui/energy_orb_dark.tres");

	private Color OutlineColor => _player.Character.EnergyLabelOutlineColor;

	public static NEnergyCounter? Create(Player player)
	{
		if (TestMode.IsOn)
		{
			return null;
		}
		NEnergyCounter nEnergyCounter = PreloadManager.Cache.GetScene(player.Character.EnergyCounterPath).Instantiate<NEnergyCounter>((GenEditState)0);
		nEnergyCounter._player = player;
		return nEnergyCounter;
	}

	public override void _Ready()
	{
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		_label = ((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("Label"));
		_layers = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%Layers"));
		_rotationLayers = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%RotationLayers"));
		_backParticles = ((Node)this).GetNode<CpuParticles2D>(NodePath.op_Implicit("%BurstBack"));
		_frontParticles = ((Node)this).GetNode<CpuParticles2D>(NodePath.op_Implicit("%BurstFront"));
		LocString locString = new LocString("static_hover_tips", "ENERGY_COUNT.description");
		locString.Add("energyPrefix", EnergyIconHelper.GetPrefix(_player.Character.CardPool));
		_hoverTip = new HoverTip(new LocString("static_hover_tips", "ENERGY_COUNT.title"), locString);
		((GodotObject)this).Connect(SignalName.MouseEntered, Callable.From((Action)OnHovered), 0u);
		((GodotObject)this).Connect(SignalName.MouseExited, Callable.From((Action)OnUnhovered), 0u);
		RefreshLabel();
	}

	public override void _EnterTree()
	{
		((Node)this)._EnterTree();
		CombatManager.Instance.StateTracker.CombatStateChanged += OnCombatStateChanged;
		_player.PlayerCombatState.EnergyChanged += OnEnergyChanged;
	}

	public override void _ExitTree()
	{
		((Node)this)._ExitTree();
		CombatManager.Instance.StateTracker.CombatStateChanged -= OnCombatStateChanged;
		_player.PlayerCombatState.EnergyChanged -= OnEnergyChanged;
	}

	private void OnHovered()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		NHoverTipSet nHoverTipSet = NHoverTipSet.CreateAndShow((Control)(object)this, _hoverTip);
		((Control)nHoverTipSet).GlobalPosition = ((Control)this).GlobalPosition + new Vector2(-70f, -200f);
	}

	private void OnUnhovered()
	{
		NHoverTipSet.Remove((Control)(object)this);
	}

	private void OnCombatStateChanged(CombatState combatState)
	{
		RefreshLabel();
	}

	private void RefreshLabel()
	{
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		PlayerCombatState playerCombatState = _player.PlayerCombatState;
		_label.SetTextAutoSize($"{playerCombatState.Energy}/{playerCombatState.MaxEnergy}");
		((Control)_label).AddThemeColorOverride(ThemeConstants.Label.fontColor, (playerCombatState.Energy == 0) ? StsColors.red : StsColors.cream);
		((Control)_label).AddThemeColorOverride(ThemeConstants.Label.fontOutlineColor, (playerCombatState.Energy == 0) ? StsColors.unplayableEnergyCostOutline : OutlineColor);
		Material material = ((playerCombatState.Energy == 0) ? PreloadManager.Cache.GetMaterial("res://materials/ui/energy_orb_dark.tres") : null);
		foreach (Control item in ((IEnumerable)((Node)_layers).GetChildren(false)).OfType<Control>())
		{
			((CanvasItem)item).Material = material;
		}
		foreach (Control item2 in ((IEnumerable)((Node)_rotationLayers).GetChildren(false)).OfType<Control>())
		{
			((CanvasItem)item2).Material = material;
		}
		((CanvasItem)_layers).Modulate = ((playerCombatState.Energy == 0) ? Colors.DarkGray : Colors.White);
	}

	private void OnEnergyChanged(int oldEnergy, int newEnergy)
	{
		if (oldEnergy < newEnergy)
		{
			_frontParticles.Emitting = true;
			_backParticles.Emitting = true;
		}
	}

	public override void _Process(double delta)
	{
		float num = ((_player.PlayerCombatState.Energy == 0) ? 5f : 30f);
		for (int i = 0; i < ((Node)_rotationLayers).GetChildCount(false); i++)
		{
			Control child = ((Node)_rotationLayers).GetChild<Control>(i, false);
			child.RotationDegrees += (float)delta * num * (float)(i + 1);
		}
	}

	public void AnimIn()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		Tween? animOutTween = _animOutTween;
		if (animOutTween != null)
		{
			animOutTween.Kill();
		}
		_animInTween = ((Node)this).CreateTween();
		((Control)this).Position = _hidePosition;
		_animInTween.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("position"), Variant.op_Implicit(_showPosition), 0.6000000238418579).SetEase((EaseType)1).SetTrans((TransitionType)5);
	}

	public void AnimOut()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		Tween? animInTween = _animInTween;
		if (animInTween != null)
		{
			animInTween.Kill();
		}
		_animOutTween = ((Node)this).CreateTween();
		((Control)this).Position = _showPosition;
		_animOutTween.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("position"), Variant.op_Implicit(_hidePosition), 0.6000000238418579).SetEase((EaseType)0).SetTrans((TransitionType)10);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(10);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._EnterTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._ExitTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnHovered, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnUnhovered, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.RefreshLabel, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnEnergyChanged, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("oldEnergy"), (PropertyHint)0, "", (PropertyUsageFlags)6, false),
			new PropertyInfo((Type)2, StringName.op_Implicit("newEnergy"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Process, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)3, StringName.op_Implicit("delta"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AnimIn, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AnimOut, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
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
		if ((ref method) == MethodName.OnHovered && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnHovered();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnUnhovered && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnUnhovered();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.RefreshLabel && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			RefreshLabel();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnEnergyChanged && ((NativeVariantPtrArgs)(ref args)).Count == 2)
		{
			OnEnergyChanged(VariantUtils.ConvertTo<int>(ref ((NativeVariantPtrArgs)(ref args))[0]), VariantUtils.ConvertTo<int>(ref ((NativeVariantPtrArgs)(ref args))[1]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._Process && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			((Node)this)._Process(VariantUtils.ConvertTo<double>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.AnimIn && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			AnimIn();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.AnimOut && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			AnimOut();
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
		if ((ref method) == MethodName._EnterTree)
		{
			return true;
		}
		if ((ref method) == MethodName._ExitTree)
		{
			return true;
		}
		if ((ref method) == MethodName.OnHovered)
		{
			return true;
		}
		if ((ref method) == MethodName.OnUnhovered)
		{
			return true;
		}
		if ((ref method) == MethodName.RefreshLabel)
		{
			return true;
		}
		if ((ref method) == MethodName.OnEnergyChanged)
		{
			return true;
		}
		if ((ref method) == MethodName._Process)
		{
			return true;
		}
		if ((ref method) == MethodName.AnimIn)
		{
			return true;
		}
		if ((ref method) == MethodName.AnimOut)
		{
			return true;
		}
		return ((Control)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._label)
		{
			_label = VariantUtils.ConvertTo<MegaLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._layers)
		{
			_layers = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._rotationLayers)
		{
			_rotationLayers = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._backParticles)
		{
			_backParticles = VariantUtils.ConvertTo<CpuParticles2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._frontParticles)
		{
			_frontParticles = VariantUtils.ConvertTo<CpuParticles2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._animInTween)
		{
			_animInTween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._animOutTween)
		{
			_animOutTween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		return ((GodotObject)this).SetGodotClassPropertyValue(ref name, ref value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName.OutlineColor)
		{
			Color outlineColor = OutlineColor;
			value = VariantUtils.CreateFrom<Color>(ref outlineColor);
			return true;
		}
		if ((ref name) == PropertyName._label)
		{
			value = VariantUtils.CreateFrom<MegaLabel>(ref _label);
			return true;
		}
		if ((ref name) == PropertyName._layers)
		{
			value = VariantUtils.CreateFrom<Control>(ref _layers);
			return true;
		}
		if ((ref name) == PropertyName._rotationLayers)
		{
			value = VariantUtils.CreateFrom<Control>(ref _rotationLayers);
			return true;
		}
		if ((ref name) == PropertyName._backParticles)
		{
			value = VariantUtils.CreateFrom<CpuParticles2D>(ref _backParticles);
			return true;
		}
		if ((ref name) == PropertyName._frontParticles)
		{
			value = VariantUtils.CreateFrom<CpuParticles2D>(ref _frontParticles);
			return true;
		}
		if ((ref name) == PropertyName._animInTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _animInTween);
			return true;
		}
		if ((ref name) == PropertyName._animOutTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _animOutTween);
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
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._label, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._layers, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._rotationLayers, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._backParticles, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._frontParticles, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._animInTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._animOutTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)20, PropertyName.OutlineColor, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		((GodotObject)this).SaveGodotObjectData(info);
		info.AddProperty(PropertyName._label, Variant.From<MegaLabel>(ref _label));
		info.AddProperty(PropertyName._layers, Variant.From<Control>(ref _layers));
		info.AddProperty(PropertyName._rotationLayers, Variant.From<Control>(ref _rotationLayers));
		info.AddProperty(PropertyName._backParticles, Variant.From<CpuParticles2D>(ref _backParticles));
		info.AddProperty(PropertyName._frontParticles, Variant.From<CpuParticles2D>(ref _frontParticles));
		info.AddProperty(PropertyName._animInTween, Variant.From<Tween>(ref _animInTween));
		info.AddProperty(PropertyName._animOutTween, Variant.From<Tween>(ref _animOutTween));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._label, ref val))
		{
			_label = ((Variant)(ref val)).As<MegaLabel>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._layers, ref val2))
		{
			_layers = ((Variant)(ref val2)).As<Control>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._rotationLayers, ref val3))
		{
			_rotationLayers = ((Variant)(ref val3)).As<Control>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._backParticles, ref val4))
		{
			_backParticles = ((Variant)(ref val4)).As<CpuParticles2D>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._frontParticles, ref val5))
		{
			_frontParticles = ((Variant)(ref val5)).As<CpuParticles2D>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._animInTween, ref val6))
		{
			_animInTween = ((Variant)(ref val6)).As<Tween>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName._animOutTween, ref val7))
		{
			_animOutTween = ((Variant)(ref val7)).As<Tween>();
		}
	}
}
