using System;
using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.Nodes.Combat;

[ScriptPath("res://src/Core/Nodes/Combat/NPower.cs")]
public class NPower : Control
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName _EnterTree = StringName.op_Implicit("_EnterTree");

		public static readonly StringName _ExitTree = StringName.op_Implicit("_ExitTree");

		public static readonly StringName Reload = StringName.op_Implicit("Reload");

		public static readonly StringName OnPulsingStarted = StringName.op_Implicit("OnPulsingStarted");

		public static readonly StringName OnPulsingStopped = StringName.op_Implicit("OnPulsingStopped");

		public static readonly StringName RefreshAmount = StringName.op_Implicit("RefreshAmount");

		public static readonly StringName OnDisplayAmountChanged = StringName.op_Implicit("OnDisplayAmountChanged");

		public static readonly StringName FlashPower = StringName.op_Implicit("FlashPower");

		public static readonly StringName OnHovered = StringName.op_Implicit("OnHovered");

		public static readonly StringName OnUnhovered = StringName.op_Implicit("OnUnhovered");

		public static readonly StringName SubscribeToModelEvents = StringName.op_Implicit("SubscribeToModelEvents");

		public static readonly StringName UnsubscribeFromModelEvents = StringName.op_Implicit("UnsubscribeFromModelEvents");

		public static readonly StringName OnPowerRemoved = StringName.op_Implicit("OnPowerRemoved");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName Container = StringName.op_Implicit("Container");

		public static readonly StringName _icon = StringName.op_Implicit("_icon");

		public static readonly StringName _amountLabel = StringName.op_Implicit("_amountLabel");

		public static readonly StringName _powerFlash = StringName.op_Implicit("_powerFlash");

		public static readonly StringName _animInTween = StringName.op_Implicit("_animInTween");
	}

	public class SignalName : SignalName
	{
	}

	private static readonly StringName _pulse = new StringName("pulse");

	private PowerModel? _model;

	private TextureRect _icon;

	private MegaLabel _amountLabel;

	private CpuParticles2D _powerFlash;

	private Tween? _animInTween;

	public NPowerContainer Container { get; set; }

	private static string ScenePath => SceneHelper.GetScenePath("combat/power");

	public static IEnumerable<string> AssetPaths => new _003C_003Ez__ReadOnlySingleElementList<string>(ScenePath);

	public PowerModel Model
	{
		get
		{
			return _model ?? throw new InvalidOperationException("Model was accessed before it was set.");
		}
		set
		{
			if (_model != null)
			{
				UnsubscribeFromModelEvents();
			}
			value.AssertMutable();
			_model = value;
			if (_model != null && ((Node)this).IsInsideTree())
			{
				SubscribeToModelEvents();
			}
			Reload();
		}
	}

	public static NPower Create(PowerModel power)
	{
		NPower nPower = PreloadManager.Cache.GetScene(ScenePath).Instantiate<NPower>((GenEditState)0);
		nPower.Model = power;
		return nPower;
	}

	public override void _Ready()
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		_icon = ((Node)this).GetNode<TextureRect>(NodePath.op_Implicit("%Icon"));
		_amountLabel = ((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%AmountLabel"));
		_powerFlash = ((Node)this).GetNode<CpuParticles2D>(NodePath.op_Implicit("%PowerFlash"));
		((GodotObject)this).Connect(SignalName.MouseEntered, Callable.From((Action)OnHovered), 0u);
		((GodotObject)this).Connect(SignalName.MouseExited, Callable.From((Action)OnUnhovered), 0u);
		Reload();
		Tween? animInTween = _animInTween;
		if (animInTween != null)
		{
			animInTween.Kill();
		}
		_animInTween = ((Node)this).CreateTween().SetParallel(true);
		_animInTween.TweenProperty((GodotObject)(object)_icon, NodePath.op_Implicit("position:y"), Variant.op_Implicit(0f), 0.5).SetEase((EaseType)1).SetTrans((TransitionType)10)
			.From(Variant.op_Implicit(-24f));
		_animInTween.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(1f), 0.5).SetEase((EaseType)1).SetTrans((TransitionType)5);
	}

	public override void _EnterTree()
	{
		SubscribeToModelEvents();
	}

	public override void _ExitTree()
	{
		UnsubscribeFromModelEvents();
	}

	private void Reload()
	{
		if (((Node)this).IsNodeReady())
		{
			_icon.Texture = _model?.Icon;
			_powerFlash.Texture = _model?.BigIcon;
			RefreshAmount();
		}
	}

	private void OnPulsingStarted()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		ShaderMaterial val = (ShaderMaterial)((CanvasItem)_icon).Material;
		val.SetShaderParameter(_pulse, Variant.op_Implicit(1));
	}

	private void OnPulsingStopped()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		ShaderMaterial val = (ShaderMaterial)((CanvasItem)_icon).Material;
		val.SetShaderParameter(_pulse, Variant.op_Implicit(0));
	}

	private void RefreshAmount()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		if (_model != null)
		{
			((Control)_amountLabel).AddThemeColorOverride(ThemeConstants.Label.fontColor, Model.AmountLabelColor);
			_amountLabel.SetTextAutoSize((Model.StackType == PowerStackType.Counter) ? Model.DisplayAmount.ToString() : string.Empty);
		}
		else
		{
			_amountLabel.SetTextAutoSize(string.Empty);
		}
	}

	private void OnDisplayAmountChanged()
	{
		FlashPower();
		RefreshAmount();
	}

	private void OnPowerFlashed(PowerModel _)
	{
		FlashPower();
	}

	private void FlashPower()
	{
		_powerFlash.Emitting = true;
	}

	private void OnHovered()
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		NCombatRoom.Instance?.GetCreatureNode(Model.Owner)?.ShowHoverTips(Model.HoverTips);
		((Control)_icon).Scale = Vector2.One * 1.1f;
		CombatManager.Instance.StateTracker.CombatStateChanged += ShowPowerHoverTips;
	}

	private void OnUnhovered()
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		NCombatRoom.Instance?.GetCreatureNode(Model.Owner)?.HideHoverTips();
		((Control)_icon).Scale = Vector2.One * 1f;
		CombatManager.Instance.StateTracker.CombatStateChanged -= ShowPowerHoverTips;
	}

	private void ShowPowerHoverTips(CombatState _)
	{
		NCombatRoom.Instance?.GetCreatureNode(Model.Owner)?.ShowHoverTips(Model.HoverTips);
	}

	private void SubscribeToModelEvents()
	{
		if (_model != null)
		{
			Model.DisplayAmountChanged += OnDisplayAmountChanged;
			Model.Flashed += OnPowerFlashed;
			Model.Removed += OnPowerRemoved;
			Model.Owner.Died += OnOwnerDied;
			Model.Owner.Revived += OnOwnerRevived;
			Model.PulsingStarted += OnPulsingStarted;
			Model.PulsingStopped += OnPulsingStopped;
		}
	}

	private void UnsubscribeFromModelEvents()
	{
		if (_model != null)
		{
			Model.DisplayAmountChanged -= OnDisplayAmountChanged;
			Model.Flashed -= OnPowerFlashed;
			Model.Removed -= OnPowerRemoved;
			Model.Owner.Died -= OnOwnerDied;
			Model.Owner.Revived -= OnOwnerRevived;
			Model.PulsingStarted -= OnPulsingStarted;
			Model.PulsingStopped -= OnPulsingStopped;
		}
	}

	private void OnPowerRemoved()
	{
		UnsubscribeFromModelEvents();
	}

	private void OnOwnerDied(Creature _)
	{
		if (GodotObject.IsInstanceValid((GodotObject)(object)this) && Model.ShouldPowerBeRemovedAfterOwnerDeath())
		{
			((Control)this).MouseFilter = (MouseFilterEnum)2;
		}
	}

	private void OnOwnerRevived(Creature _)
	{
		((Control)this).MouseFilter = (MouseFilterEnum)0;
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
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_0287: Unknown result type (might be due to invalid IL or missing references)
		//IL_0290: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(14);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._EnterTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._ExitTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.Reload, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnPulsingStarted, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnPulsingStopped, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.RefreshAmount, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnDisplayAmountChanged, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.FlashPower, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnHovered, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnUnhovered, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SubscribeToModelEvents, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UnsubscribeFromModelEvents, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnPowerRemoved, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
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
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
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
		if ((ref method) == MethodName.Reload && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			Reload();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnPulsingStarted && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnPulsingStarted();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnPulsingStopped && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnPulsingStopped();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.RefreshAmount && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			RefreshAmount();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnDisplayAmountChanged && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnDisplayAmountChanged();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.FlashPower && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			FlashPower();
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
		if ((ref method) == MethodName.SubscribeToModelEvents && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			SubscribeToModelEvents();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.UnsubscribeFromModelEvents && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			UnsubscribeFromModelEvents();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnPowerRemoved && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnPowerRemoved();
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
		if ((ref method) == MethodName.Reload)
		{
			return true;
		}
		if ((ref method) == MethodName.OnPulsingStarted)
		{
			return true;
		}
		if ((ref method) == MethodName.OnPulsingStopped)
		{
			return true;
		}
		if ((ref method) == MethodName.RefreshAmount)
		{
			return true;
		}
		if ((ref method) == MethodName.OnDisplayAmountChanged)
		{
			return true;
		}
		if ((ref method) == MethodName.FlashPower)
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
		if ((ref method) == MethodName.SubscribeToModelEvents)
		{
			return true;
		}
		if ((ref method) == MethodName.UnsubscribeFromModelEvents)
		{
			return true;
		}
		if ((ref method) == MethodName.OnPowerRemoved)
		{
			return true;
		}
		return ((Control)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName.Container)
		{
			Container = VariantUtils.ConvertTo<NPowerContainer>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._icon)
		{
			_icon = VariantUtils.ConvertTo<TextureRect>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._amountLabel)
		{
			_amountLabel = VariantUtils.ConvertTo<MegaLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._powerFlash)
		{
			_powerFlash = VariantUtils.ConvertTo<CpuParticles2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._animInTween)
		{
			_animInTween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		return ((GodotObject)this).SetGodotClassPropertyValue(ref name, ref value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
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
		if ((ref name) == PropertyName.Container)
		{
			NPowerContainer container = Container;
			value = VariantUtils.CreateFrom<NPowerContainer>(ref container);
			return true;
		}
		if ((ref name) == PropertyName._icon)
		{
			value = VariantUtils.CreateFrom<TextureRect>(ref _icon);
			return true;
		}
		if ((ref name) == PropertyName._amountLabel)
		{
			value = VariantUtils.CreateFrom<MegaLabel>(ref _amountLabel);
			return true;
		}
		if ((ref name) == PropertyName._powerFlash)
		{
			value = VariantUtils.CreateFrom<CpuParticles2D>(ref _powerFlash);
			return true;
		}
		if ((ref name) == PropertyName._animInTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _animInTween);
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
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._icon, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._amountLabel, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._powerFlash, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._animInTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.Container, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		StringName container = PropertyName.Container;
		NPowerContainer container2 = Container;
		info.AddProperty(container, Variant.From<NPowerContainer>(ref container2));
		info.AddProperty(PropertyName._icon, Variant.From<TextureRect>(ref _icon));
		info.AddProperty(PropertyName._amountLabel, Variant.From<MegaLabel>(ref _amountLabel));
		info.AddProperty(PropertyName._powerFlash, Variant.From<CpuParticles2D>(ref _powerFlash));
		info.AddProperty(PropertyName._animInTween, Variant.From<Tween>(ref _animInTween));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName.Container, ref val))
		{
			Container = ((Variant)(ref val)).As<NPowerContainer>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._icon, ref val2))
		{
			_icon = ((Variant)(ref val2)).As<TextureRect>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._amountLabel, ref val3))
		{
			_amountLabel = ((Variant)(ref val3)).As<MegaLabel>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._powerFlash, ref val4))
		{
			_powerFlash = ((Variant)(ref val4)).As<CpuParticles2D>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._animInTween, ref val5))
		{
			_animInTween = ((Variant)(ref val5)).As<Tween>();
		}
	}
}
