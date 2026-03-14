using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.HoverTips;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.TestSupport;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.Nodes.Relics;

[ScriptPath("res://src/Core/Nodes/Relics/NRelicInventoryHolder.cs")]
public class NRelicInventoryHolder : NButton
{
	public new class MethodName : NButton.MethodName
	{
		public new static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public new static readonly StringName _ExitTree = StringName.op_Implicit("_ExitTree");

		public static readonly StringName RefreshAmount = StringName.op_Implicit("RefreshAmount");

		public static readonly StringName RefreshStatus = StringName.op_Implicit("RefreshStatus");

		public new static readonly StringName OnFocus = StringName.op_Implicit("OnFocus");

		public new static readonly StringName OnUnfocus = StringName.op_Implicit("OnUnfocus");

		public static readonly StringName DoFlash = StringName.op_Implicit("DoFlash");

		public static readonly StringName OnDisplayAmountChanged = StringName.op_Implicit("OnDisplayAmountChanged");

		public static readonly StringName OnStatusChanged = StringName.op_Implicit("OnStatusChanged");
	}

	public new class PropertyName : NButton.PropertyName
	{
		public static readonly StringName Relic = StringName.op_Implicit("Relic");

		public static readonly StringName Inventory = StringName.op_Implicit("Inventory");

		public static readonly StringName _relic = StringName.op_Implicit("_relic");

		public static readonly StringName _amountLabel = StringName.op_Implicit("_amountLabel");

		public static readonly StringName _hoverTween = StringName.op_Implicit("_hoverTween");

		public static readonly StringName _obtainedTween = StringName.op_Implicit("_obtainedTween");

		public static readonly StringName _originalIconPosition = StringName.op_Implicit("_originalIconPosition");
	}

	public new class SignalName : NButton.SignalName
	{
	}

	private static readonly string _scenePath = SceneHelper.GetScenePath("relics/relic_inventory_holder");

	private static readonly string _flashPath = SceneHelper.GetScenePath("vfx/relic_inventory_flash_vfx");

	private const float _newlyAcquiredPopDuration = 0.35f;

	private const float _newlyAcquiredFadeInDuration = 0.1f;

	private const float _newlyAcquiredPopDistance = 40f;

	private NRelic _relic;

	private RelicModel? _subscribedRelic;

	private MegaLabel _amountLabel;

	private Tween? _hoverTween;

	private Tween? _obtainedTween;

	private CancellationTokenSource? _cancellationTokenSource;

	private Vector2 _originalIconPosition;

	private RelicModel _model;

	public static IEnumerable<string> AssetPaths => new global::_003C_003Ez__ReadOnlyArray<string>(new string[2] { _scenePath, _flashPath });

	public NRelic Relic => _relic;

	public NRelicInventory Inventory { get; set; }

	public static NRelicInventoryHolder? Create(RelicModel relic)
	{
		if (TestMode.IsOn)
		{
			return null;
		}
		NRelicInventoryHolder nRelicInventoryHolder = PreloadManager.Cache.GetScene(_scenePath).Instantiate<NRelicInventoryHolder>((GenEditState)0);
		((Node)nRelicInventoryHolder).Name = StringName.op_Implicit($"NRelicContainerHolder-{relic.Id}");
		nRelicInventoryHolder._model = relic;
		return nRelicInventoryHolder;
	}

	public override void _Ready()
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		ConnectSignals();
		_relic = ((Node)this).GetNode<NRelic>(NodePath.op_Implicit("%Relic"));
		_amountLabel = ((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%AmountLabel"));
		_originalIconPosition = ((Control)_relic.Icon).Position;
		_relic.ModelChanged += OnModelChanged;
		_relic.Model = _model;
	}

	public override void _ExitTree()
	{
		Tween? hoverTween = _hoverTween;
		if (hoverTween != null)
		{
			hoverTween.Kill();
		}
		if (_subscribedRelic != null)
		{
			_subscribedRelic.DisplayAmountChanged -= OnDisplayAmountChanged;
			_subscribedRelic.StatusChanged -= OnStatusChanged;
			_subscribedRelic.Flashed -= OnRelicFlashed;
		}
		_subscribedRelic = null;
		_relic.ModelChanged -= OnModelChanged;
	}

	private void OnModelChanged(RelicModel? oldModel, RelicModel? newModel)
	{
		if (oldModel != null)
		{
			oldModel.DisplayAmountChanged -= OnDisplayAmountChanged;
			oldModel.StatusChanged -= OnStatusChanged;
			oldModel.Flashed -= OnRelicFlashed;
		}
		if (newModel != null)
		{
			newModel.DisplayAmountChanged += OnDisplayAmountChanged;
			newModel.StatusChanged += OnStatusChanged;
			newModel.Flashed += OnRelicFlashed;
		}
		RefreshAmount();
		RefreshStatus();
		_subscribedRelic = newModel;
	}

	private void RefreshAmount()
	{
		if (_relic.Model.ShowCounter && RunManager.Instance.IsInProgress)
		{
			((CanvasItem)_amountLabel).Visible = true;
			_amountLabel.SetTextAutoSize(_relic.Model.DisplayAmount.ToString());
		}
		else
		{
			((CanvasItem)_amountLabel).Visible = false;
		}
	}

	private void RefreshStatus()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		if (!RunManager.Instance.IsInProgress)
		{
			((CanvasItem)_relic.Icon).Modulate = Colors.White;
			return;
		}
		_relic.Model.UpdateTexture(_relic.Icon);
		TextureRect icon = _relic.Icon;
		Color modulate;
		switch (_relic.Model.Status)
		{
		case RelicStatus.Normal:
		case RelicStatus.Active:
			modulate = Colors.White;
			break;
		case RelicStatus.Disabled:
			modulate = new Color("#808080");
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
		((CanvasItem)icon).Modulate = modulate;
	}

	public async Task PlayNewlyAcquiredAnimation(Vector2? startLocation, Vector2? startScale)
	{
		if (_cancellationTokenSource != null)
		{
			await _cancellationTokenSource.CancelAsync();
		}
		CancellationTokenSource cancelTokenSource = (_cancellationTokenSource = new CancellationTokenSource());
		await ((GodotObject)this).ToSignal((GodotObject)(object)((Node)this).GetTree(), SignalName.ProcessFrame);
		if (!cancelTokenSource.IsCancellationRequested)
		{
			Tween? obtainedTween = _obtainedTween;
			if (obtainedTween != null)
			{
				obtainedTween.Kill();
			}
			if (!startLocation.HasValue)
			{
				TextureRect icon = _relic.Icon;
				Vector2 position = ((Control)_relic.Icon).Position;
				position.Y = ((Control)_relic.Icon).Position.Y + 40f;
				((Control)icon).Position = position;
				TextureRect icon2 = _relic.Icon;
				Color modulate = ((CanvasItem)_relic.Icon).Modulate;
				modulate.A = 0f;
				((CanvasItem)icon2).Modulate = modulate;
				_obtainedTween = ((Node)this).GetTree().CreateTween();
				_obtainedTween.TweenProperty((GodotObject)(object)_relic.Icon, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(1f), 0.10000000149011612);
				_obtainedTween.Parallel();
				_obtainedTween.SetEase((EaseType)1);
				_obtainedTween.SetTrans((TransitionType)10);
				_obtainedTween.TweenProperty((GodotObject)(object)_relic.Icon, NodePath.op_Implicit("position:y"), Variant.op_Implicit(_originalIconPosition.Y), 0.3499999940395355);
				_obtainedTween.TweenCallback(Callable.From((Action)DoFlash));
			}
			else
			{
				((Control)_relic.Icon).GlobalPosition = startLocation.Value;
				((Control)_relic.Icon).Scale = (Vector2)(((_003F?)startScale) ?? Vector2.One);
				TextureRect icon3 = _relic.Icon;
				Color modulate = ((CanvasItem)_relic.Icon).Modulate;
				modulate.A = 1f;
				((CanvasItem)icon3).Modulate = modulate;
				_obtainedTween = ((Node)this).GetTree().CreateTween();
				_obtainedTween.SetEase((EaseType)1);
				_obtainedTween.SetTrans((TransitionType)1);
				_obtainedTween.TweenProperty((GodotObject)(object)_relic.Icon, NodePath.op_Implicit("position"), Variant.op_Implicit(_originalIconPosition), 0.3499999940395355);
				_obtainedTween.Parallel().TweenProperty((GodotObject)(object)_relic.Icon, NodePath.op_Implicit("scale"), Variant.op_Implicit(Vector2.One), 0.3499999940395355);
				_obtainedTween.TweenCallback(Callable.From((Action)DoFlash));
			}
		}
	}

	protected override void OnFocus()
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		Tween? hoverTween = _hoverTween;
		if (hoverTween != null)
		{
			hoverTween.Kill();
		}
		_hoverTween = ((Node)this).CreateTween();
		_hoverTween.TweenProperty((GodotObject)(object)_relic.Icon, NodePath.op_Implicit("scale"), Variant.op_Implicit(Vector2.One * 1.25f), 0.05);
		NHoverTipSet nHoverTipSet = NHoverTipSet.CreateAndShow((Control)(object)this, _relic.Model.HoverTips);
		nHoverTipSet.SetAlignmentForRelic(_relic);
	}

	protected override void OnUnfocus()
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		Tween? hoverTween = _hoverTween;
		if (hoverTween != null)
		{
			hoverTween.Kill();
		}
		_hoverTween = ((Node)this).CreateTween();
		_hoverTween.TweenProperty((GodotObject)(object)_relic.Icon, NodePath.op_Implicit("scale"), Variant.op_Implicit(Vector2.One), 1.0).SetEase((EaseType)1).SetTrans((TransitionType)5);
		NHoverTipSet.Remove((Control)(object)this);
	}

	private void OnRelicFlashed(RelicModel _, IEnumerable<Creature> __)
	{
		DoFlash();
	}

	private void DoFlash()
	{
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		Node2D val = PreloadManager.Cache.GetScene(_flashPath).Instantiate<Node2D>((GenEditState)0);
		Node aboveTopBarVfxContainer = (Node)(object)NRun.Instance.GlobalUi.AboveTopBarVfxContainer;
		((Node)val).GetNode<GpuParticles2D>(NodePath.op_Implicit("Particles")).Texture = _relic.Model.Icon;
		val.GlobalPosition = ((Control)this).GlobalPosition + ((Control)this).Size * 0.5f;
		aboveTopBarVfxContainer.AddChildSafely((Node?)(object)val);
	}

	private void OnDisplayAmountChanged()
	{
		RefreshAmount();
	}

	private void OnStatusChanged()
	{
		RefreshStatus();
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<MethodInfo> GetGodotMethodList()
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
		List<MethodInfo> list = new List<MethodInfo>(9);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._ExitTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.RefreshAmount, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.RefreshStatus, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnFocus, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnUnfocus, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.DoFlash, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnDisplayAmountChanged, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnStatusChanged, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
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
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._ExitTree && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._ExitTree();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.RefreshAmount && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			RefreshAmount();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.RefreshStatus && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			RefreshStatus();
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
		if ((ref method) == MethodName.DoFlash && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			DoFlash();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnDisplayAmountChanged && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnDisplayAmountChanged();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnStatusChanged && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnStatusChanged();
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
		if ((ref method) == MethodName._ExitTree)
		{
			return true;
		}
		if ((ref method) == MethodName.RefreshAmount)
		{
			return true;
		}
		if ((ref method) == MethodName.RefreshStatus)
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
		if ((ref method) == MethodName.DoFlash)
		{
			return true;
		}
		if ((ref method) == MethodName.OnDisplayAmountChanged)
		{
			return true;
		}
		if ((ref method) == MethodName.OnStatusChanged)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName.Inventory)
		{
			Inventory = VariantUtils.ConvertTo<NRelicInventory>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._relic)
		{
			_relic = VariantUtils.ConvertTo<NRelic>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._amountLabel)
		{
			_amountLabel = VariantUtils.ConvertTo<MegaLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._hoverTween)
		{
			_hoverTween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._obtainedTween)
		{
			_obtainedTween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._originalIconPosition)
		{
			_originalIconPosition = VariantUtils.ConvertTo<Vector2>(ref value);
			return true;
		}
		return base.SetGodotClassPropertyValue(in name, in value);
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
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName.Relic)
		{
			NRelic relic = Relic;
			value = VariantUtils.CreateFrom<NRelic>(ref relic);
			return true;
		}
		if ((ref name) == PropertyName.Inventory)
		{
			NRelicInventory inventory = Inventory;
			value = VariantUtils.CreateFrom<NRelicInventory>(ref inventory);
			return true;
		}
		if ((ref name) == PropertyName._relic)
		{
			value = VariantUtils.CreateFrom<NRelic>(ref _relic);
			return true;
		}
		if ((ref name) == PropertyName._amountLabel)
		{
			value = VariantUtils.CreateFrom<MegaLabel>(ref _amountLabel);
			return true;
		}
		if ((ref name) == PropertyName._hoverTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _hoverTween);
			return true;
		}
		if ((ref name) == PropertyName._obtainedTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _obtainedTween);
			return true;
		}
		if ((ref name) == PropertyName._originalIconPosition)
		{
			value = VariantUtils.CreateFrom<Vector2>(ref _originalIconPosition);
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
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._relic, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._amountLabel, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._hoverTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._obtainedTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)5, PropertyName._originalIconPosition, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.Relic, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.Inventory, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		base.SaveGodotObjectData(info);
		StringName inventory = PropertyName.Inventory;
		NRelicInventory inventory2 = Inventory;
		info.AddProperty(inventory, Variant.From<NRelicInventory>(ref inventory2));
		info.AddProperty(PropertyName._relic, Variant.From<NRelic>(ref _relic));
		info.AddProperty(PropertyName._amountLabel, Variant.From<MegaLabel>(ref _amountLabel));
		info.AddProperty(PropertyName._hoverTween, Variant.From<Tween>(ref _hoverTween));
		info.AddProperty(PropertyName._obtainedTween, Variant.From<Tween>(ref _obtainedTween));
		info.AddProperty(PropertyName._originalIconPosition, Variant.From<Vector2>(ref _originalIconPosition));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		base.RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName.Inventory, ref val))
		{
			Inventory = ((Variant)(ref val)).As<NRelicInventory>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._relic, ref val2))
		{
			_relic = ((Variant)(ref val2)).As<NRelic>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._amountLabel, ref val3))
		{
			_amountLabel = ((Variant)(ref val3)).As<MegaLabel>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._hoverTween, ref val4))
		{
			_hoverTween = ((Variant)(ref val4)).As<Tween>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._obtainedTween, ref val5))
		{
			_obtainedTween = ((Variant)(ref val5)).As<Tween>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._originalIconPosition, ref val6))
		{
			_originalIconPosition = ((Variant)(ref val6)).As<Vector2>();
		}
	}
}
