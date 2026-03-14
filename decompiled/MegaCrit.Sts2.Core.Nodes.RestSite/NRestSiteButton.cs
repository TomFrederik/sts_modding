using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Entities.RestSite;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.Nodes.RestSite;

[ScriptPath("res://src/Core/Nodes/RestSite/NRestSiteButton.cs")]
public class NRestSiteButton : NButton
{
	public new class MethodName : NButton.MethodName
	{
		public new static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public new static readonly StringName _ExitTree = StringName.op_Implicit("_ExitTree");

		public static readonly StringName Reload = StringName.op_Implicit("Reload");

		public new static readonly StringName OnRelease = StringName.op_Implicit("OnRelease");

		public new static readonly StringName OnPress = StringName.op_Implicit("OnPress");

		public new static readonly StringName OnFocus = StringName.op_Implicit("OnFocus");

		public new static readonly StringName OnUnfocus = StringName.op_Implicit("OnUnfocus");

		public static readonly StringName RefreshTextState = StringName.op_Implicit("RefreshTextState");

		public static readonly StringName UpdateShaderParam = StringName.op_Implicit("UpdateShaderParam");
	}

	public new class PropertyName : NButton.PropertyName
	{
		public static readonly StringName _hsv = StringName.op_Implicit("_hsv");

		public static readonly StringName _visuals = StringName.op_Implicit("_visuals");

		public static readonly StringName _icon = StringName.op_Implicit("_icon");

		public static readonly StringName _outline = StringName.op_Implicit("_outline");

		public static readonly StringName _label = StringName.op_Implicit("_label");

		public static readonly StringName _labelPosition = StringName.op_Implicit("_labelPosition");

		public static readonly StringName _isUnclickable = StringName.op_Implicit("_isUnclickable");

		public static readonly StringName _executingOption = StringName.op_Implicit("_executingOption");

		public static readonly StringName _currentTween = StringName.op_Implicit("_currentTween");
	}

	public new class SignalName : NButton.SignalName
	{
	}

	private static readonly StringName _v = new StringName("v");

	private static readonly StringName _s = new StringName("s");

	private ShaderMaterial _hsv;

	private Control _visuals;

	private TextureRect _icon;

	private Control _outline;

	private MegaLabel _label;

	private Vector2 _labelPosition;

	private bool _isUnclickable;

	private bool _executingOption;

	private const double _unfocusAnimDur = 1.0;

	private readonly CancellationTokenSource _cts = new CancellationTokenSource();

	private Tween? _currentTween;

	private RestSiteOption? _option;

	private static readonly string _scenePath = SceneHelper.GetScenePath("rest_site/rest_site_button");

	public static IEnumerable<string> AssetPaths => new _003C_003Ez__ReadOnlySingleElementList<string>(_scenePath);

	public RestSiteOption Option
	{
		get
		{
			return _option ?? throw new InvalidOperationException("Option accessed before being set");
		}
		set
		{
			_option = value;
			Reload();
		}
	}

	public override void _Ready()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Expected O, but got Unknown
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		ConnectSignals();
		((CanvasItem)this).Modulate = StsColors.transparentBlack;
		_visuals = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%Visuals"));
		_icon = ((Node)this).GetNode<TextureRect>(NodePath.op_Implicit("%Icon"));
		_outline = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%Outline"));
		_label = ((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%Label"));
		_hsv = (ShaderMaterial)((CanvasItem)_icon).Material;
		_labelPosition = ((Control)_label).Position;
		TaskHelper.RunSafely(AnimateIn());
		Reload();
	}

	private async Task AnimateIn()
	{
		Tween val = ((Node)this).CreateTween();
		val.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("modulate"), Variant.op_Implicit(Colors.White), 0.5).SetEase((EaseType)1).SetTrans((TransitionType)7);
		await val.AwaitFinished(_cts.Token);
		if (((Node?)(object)this).IsValid())
		{
			((Control)this).MouseFilter = (MouseFilterEnum)0;
		}
	}

	public override void _ExitTree()
	{
		_cts.Cancel();
		_cts.Dispose();
		Tween? currentTween = _currentTween;
		if (currentTween != null)
		{
			currentTween.Kill();
		}
	}

	public static NRestSiteButton Create(RestSiteOption option)
	{
		NRestSiteButton nRestSiteButton = PreloadManager.Cache.GetScene(_scenePath).Instantiate<NRestSiteButton>((GenEditState)0);
		nRestSiteButton.Option = option;
		nRestSiteButton._isUnclickable = !option.IsEnabled;
		return nRestSiteButton;
	}

	private void Reload()
	{
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		if (((Node)this).IsNodeReady() && !(_option == null))
		{
			_icon.Texture = Option.Icon;
			_label.SetTextAutoSize(Option.Title.GetFormattedText());
			if (!_option.IsEnabled)
			{
				_hsv.SetShaderParameter(_s, Variant.op_Implicit(0f));
				_hsv.SetShaderParameter(_v, Variant.op_Implicit(0.6f));
			}
			else
			{
				_hsv.SetShaderParameter(_s, Variant.op_Implicit(1f));
				_hsv.SetShaderParameter(_v, Variant.op_Implicit(1f));
			}
		}
	}

	protected override void OnRelease()
	{
		if (!_isUnclickable)
		{
			base.OnRelease();
			TaskHelper.RunSafely(SelectOption(Option));
		}
	}

	private async Task SelectOption(RestSiteOption option)
	{
		int num = NRestSiteRoom.Instance.Options.IndexOf(option);
		if (num < 0)
		{
			throw new InvalidOperationException($"Rest site option {option} was selected, but it was not in the list of rest site options!");
		}
		_executingOption = true;
		NRestSiteRoom.Instance.DisableOptions();
		RefreshTextState();
		bool success = false;
		try
		{
			success = await RunManager.Instance.RestSiteSynchronizer.ChooseLocalOption(num);
			if (success)
			{
				NRestSiteRoom.Instance.AfterSelectingOption(option);
			}
		}
		finally
		{
			_executingOption = false;
			if (((Node?)(object)this).IsValid())
			{
				RefreshTextState();
			}
			if (!success && ((Node?)(object)this).IsValid())
			{
				await ((GodotObject)this).ToSignal((GodotObject)(object)((Node)this).GetTree(), SignalName.ProcessFrame);
				NRestSiteRoom.Instance?.EnableOptions();
			}
		}
	}

	protected override void OnPress()
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		if (!_isUnclickable)
		{
			base.OnPress();
			Tween? currentTween = _currentTween;
			if (currentTween != null)
			{
				currentTween.Kill();
			}
			_currentTween = ((Node)this).CreateTween().SetParallel(true);
			_currentTween.TweenProperty((GodotObject)(object)_visuals, NodePath.op_Implicit("scale"), Variant.op_Implicit(Vector2.One * 0.9f), 1.0).SetEase((EaseType)1).SetTrans((TransitionType)5);
			_currentTween.TweenProperty((GodotObject)(object)_label, NodePath.op_Implicit("position"), Variant.op_Implicit(_labelPosition), 1.0).SetEase((EaseType)1).SetTrans((TransitionType)5);
			_currentTween.TweenProperty((GodotObject)(object)_outline, NodePath.op_Implicit("scale"), Variant.op_Implicit(Vector2.One * 0.9f), 1.0).SetEase((EaseType)1).SetTrans((TransitionType)5);
			_currentTween.TweenMethod(Callable.From<float>((Action<float>)UpdateShaderParam), Variant.op_Implicit(1.2f), Variant.op_Implicit(1f), 1.0).SetEase((EaseType)1).SetTrans((TransitionType)5);
		}
	}

	protected override void OnFocus()
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		base.OnFocus();
		Tween? currentTween = _currentTween;
		if (currentTween != null)
		{
			currentTween.Kill();
		}
		_currentTween = ((Node)this).CreateTween().SetParallel(true);
		_currentTween.TweenProperty((GodotObject)(object)_visuals, NodePath.op_Implicit("scale"), Variant.op_Implicit(Vector2.One * 1.1f), 0.05);
		_currentTween.TweenProperty((GodotObject)(object)_outline, NodePath.op_Implicit("scale"), Variant.op_Implicit(Vector2.One), 0.05);
		_currentTween.TweenProperty((GodotObject)(object)_label, NodePath.op_Implicit("position"), Variant.op_Implicit(_labelPosition + new Vector2(0f, 6f)), 0.05);
		if (!_isUnclickable)
		{
			_hsv.SetShaderParameter(_v, Variant.op_Implicit(1.2f));
		}
		RefreshTextState();
	}

	protected override void OnUnfocus()
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		Tween? currentTween = _currentTween;
		if (currentTween != null)
		{
			currentTween.Kill();
		}
		_currentTween = ((Node)this).CreateTween().SetParallel(true);
		_currentTween.TweenProperty((GodotObject)(object)_visuals, NodePath.op_Implicit("scale"), Variant.op_Implicit(Vector2.One), 1.0).SetEase((EaseType)1).SetTrans((TransitionType)5);
		_currentTween.TweenProperty((GodotObject)(object)_label, NodePath.op_Implicit("position"), Variant.op_Implicit(_labelPosition), 1.0).SetEase((EaseType)1).SetTrans((TransitionType)5);
		_currentTween.TweenProperty((GodotObject)(object)_outline, NodePath.op_Implicit("scale"), Variant.op_Implicit(Vector2.One * 0.9f), 1.0).SetEase((EaseType)1).SetTrans((TransitionType)5);
		if (!_isUnclickable)
		{
			_currentTween.TweenMethod(Callable.From<float>((Action<float>)UpdateShaderParam), Variant.op_Implicit(1.2f), Variant.op_Implicit(1f), 1.0).SetEase((EaseType)1).SetTrans((TransitionType)5);
		}
		RefreshTextState();
	}

	public void RefreshTextState()
	{
		NRestSiteRoom instance = NRestSiteRoom.Instance;
		if (instance != null)
		{
			if (base.IsFocused || _executingOption)
			{
				instance.SetText(Option.Description.GetFormattedText());
			}
			else
			{
				instance.FadeOutOptionDescription();
			}
		}
	}

	private void UpdateShaderParam(float value)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		_hsv.SetShaderParameter(_v, Variant.op_Implicit(value));
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
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(9);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._ExitTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.Reload, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnRelease, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnPress, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnFocus, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnUnfocus, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.RefreshTextState, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UpdateShaderParam, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)3, StringName.op_Implicit("value"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
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
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
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
		if ((ref method) == MethodName.Reload && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			Reload();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnRelease && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnRelease();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnPress && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnPress();
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
		if ((ref method) == MethodName.RefreshTextState && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			RefreshTextState();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.UpdateShaderParam && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			UpdateShaderParam(VariantUtils.ConvertTo<float>(ref ((NativeVariantPtrArgs)(ref args))[0]));
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
		if ((ref method) == MethodName.Reload)
		{
			return true;
		}
		if ((ref method) == MethodName.OnRelease)
		{
			return true;
		}
		if ((ref method) == MethodName.OnPress)
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
		if ((ref method) == MethodName.RefreshTextState)
		{
			return true;
		}
		if ((ref method) == MethodName.UpdateShaderParam)
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
		if ((ref name) == PropertyName._hsv)
		{
			_hsv = VariantUtils.ConvertTo<ShaderMaterial>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._visuals)
		{
			_visuals = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._icon)
		{
			_icon = VariantUtils.ConvertTo<TextureRect>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._outline)
		{
			_outline = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._label)
		{
			_label = VariantUtils.ConvertTo<MegaLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._labelPosition)
		{
			_labelPosition = VariantUtils.ConvertTo<Vector2>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._isUnclickable)
		{
			_isUnclickable = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._executingOption)
		{
			_executingOption = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._currentTween)
		{
			_currentTween = VariantUtils.ConvertTo<Tween>(ref value);
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
		if ((ref name) == PropertyName._hsv)
		{
			value = VariantUtils.CreateFrom<ShaderMaterial>(ref _hsv);
			return true;
		}
		if ((ref name) == PropertyName._visuals)
		{
			value = VariantUtils.CreateFrom<Control>(ref _visuals);
			return true;
		}
		if ((ref name) == PropertyName._icon)
		{
			value = VariantUtils.CreateFrom<TextureRect>(ref _icon);
			return true;
		}
		if ((ref name) == PropertyName._outline)
		{
			value = VariantUtils.CreateFrom<Control>(ref _outline);
			return true;
		}
		if ((ref name) == PropertyName._label)
		{
			value = VariantUtils.CreateFrom<MegaLabel>(ref _label);
			return true;
		}
		if ((ref name) == PropertyName._labelPosition)
		{
			value = VariantUtils.CreateFrom<Vector2>(ref _labelPosition);
			return true;
		}
		if ((ref name) == PropertyName._isUnclickable)
		{
			value = VariantUtils.CreateFrom<bool>(ref _isUnclickable);
			return true;
		}
		if ((ref name) == PropertyName._executingOption)
		{
			value = VariantUtils.CreateFrom<bool>(ref _executingOption);
			return true;
		}
		if ((ref name) == PropertyName._currentTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _currentTween);
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
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._hsv, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._visuals, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._icon, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._outline, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._label, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)5, PropertyName._labelPosition, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._isUnclickable, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._executingOption, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._currentTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName._hsv, Variant.From<ShaderMaterial>(ref _hsv));
		info.AddProperty(PropertyName._visuals, Variant.From<Control>(ref _visuals));
		info.AddProperty(PropertyName._icon, Variant.From<TextureRect>(ref _icon));
		info.AddProperty(PropertyName._outline, Variant.From<Control>(ref _outline));
		info.AddProperty(PropertyName._label, Variant.From<MegaLabel>(ref _label));
		info.AddProperty(PropertyName._labelPosition, Variant.From<Vector2>(ref _labelPosition));
		info.AddProperty(PropertyName._isUnclickable, Variant.From<bool>(ref _isUnclickable));
		info.AddProperty(PropertyName._executingOption, Variant.From<bool>(ref _executingOption));
		info.AddProperty(PropertyName._currentTween, Variant.From<Tween>(ref _currentTween));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		base.RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._hsv, ref val))
		{
			_hsv = ((Variant)(ref val)).As<ShaderMaterial>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._visuals, ref val2))
		{
			_visuals = ((Variant)(ref val2)).As<Control>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._icon, ref val3))
		{
			_icon = ((Variant)(ref val3)).As<TextureRect>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._outline, ref val4))
		{
			_outline = ((Variant)(ref val4)).As<Control>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._label, ref val5))
		{
			_label = ((Variant)(ref val5)).As<MegaLabel>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._labelPosition, ref val6))
		{
			_labelPosition = ((Variant)(ref val6)).As<Vector2>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName._isUnclickable, ref val7))
		{
			_isUnclickable = ((Variant)(ref val7)).As<bool>();
		}
		Variant val8 = default(Variant);
		if (info.TryGetProperty(PropertyName._executingOption, ref val8))
		{
			_executingOption = ((Variant)(ref val8)).As<bool>();
		}
		Variant val9 = default(Variant);
		if (info.TryGetProperty(PropertyName._currentTween, ref val9))
		{
			_currentTween = ((Variant)(ref val9)).As<Tween>();
		}
	}
}
