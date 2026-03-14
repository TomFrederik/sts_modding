using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Relics;
using MegaCrit.Sts2.Core.Nodes.Screens.CardSelection;
using MegaCrit.Sts2.Core.Nodes.Screens.Overlays;
using MegaCrit.Sts2.Core.Nodes.Screens.ScreenContext;
using MegaCrit.Sts2.Core.TestSupport;

namespace MegaCrit.Sts2.Core.Nodes.Screens;

[ScriptPath("res://src/Core/Nodes/Screens/NChooseARelicSelection.cs")]
public class NChooseARelicSelection : Control, IOverlayScreen, IScreenContext
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName _ExitTree = StringName.op_Implicit("_ExitTree");

		public static readonly StringName SelectHolder = StringName.op_Implicit("SelectHolder");

		public static readonly StringName OnSkipButtonReleased = StringName.op_Implicit("OnSkipButtonReleased");

		public static readonly StringName AfterOverlayOpened = StringName.op_Implicit("AfterOverlayOpened");

		public static readonly StringName AfterOverlayClosed = StringName.op_Implicit("AfterOverlayClosed");

		public static readonly StringName AfterOverlayShown = StringName.op_Implicit("AfterOverlayShown");

		public static readonly StringName AfterOverlayHidden = StringName.op_Implicit("AfterOverlayHidden");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName ScreenType = StringName.op_Implicit("ScreenType");

		public static readonly StringName UseSharedBackstop = StringName.op_Implicit("UseSharedBackstop");

		public static readonly StringName DefaultFocusedControl = StringName.op_Implicit("DefaultFocusedControl");

		public static readonly StringName _banner = StringName.op_Implicit("_banner");

		public static readonly StringName _relicRow = StringName.op_Implicit("_relicRow");

		public static readonly StringName _skipButton = StringName.op_Implicit("_skipButton");

		public static readonly StringName _screenComplete = StringName.op_Implicit("_screenComplete");

		public static readonly StringName _relicSelected = StringName.op_Implicit("_relicSelected");

		public static readonly StringName _cardTween = StringName.op_Implicit("_cardTween");

		public static readonly StringName _fadeTween = StringName.op_Implicit("_fadeTween");
	}

	public class SignalName : SignalName
	{
	}

	private const float _relicXSpacing = 200f;

	private NCommonBanner _banner;

	private Control _relicRow;

	private NChoiceSelectionSkipButton _skipButton;

	private readonly TaskCompletionSource<IEnumerable<RelicModel>> _completionSource = new TaskCompletionSource<IEnumerable<RelicModel>>();

	private bool _screenComplete;

	private bool _relicSelected;

	private Tween? _cardTween;

	private Tween? _fadeTween;

	private IReadOnlyList<RelicModel> _relics;

	public NetScreenType ScreenType => NetScreenType.Rewards;

	private static string ScenePath => SceneHelper.GetScenePath("screens/choose_a_relic_selection_screen");

	public static IEnumerable<string> AssetPaths => new _003C_003Ez__ReadOnlySingleElementList<string>(ScenePath);

	public bool UseSharedBackstop => true;

	public Control DefaultFocusedControl
	{
		get
		{
			List<NRelicBasicHolder> list = ((IEnumerable)((Node)_relicRow).GetChildren(false)).OfType<NRelicBasicHolder>().ToList();
			return (Control)(object)list[list.Count / 2];
		}
	}

	public static NChooseARelicSelection? ShowScreen(IReadOnlyList<RelicModel> relics)
	{
		if (TestMode.IsOn)
		{
			return null;
		}
		NChooseARelicSelection nChooseARelicSelection = PreloadManager.Cache.GetScene(ScenePath).Instantiate<NChooseARelicSelection>((GenEditState)0);
		((Node)nChooseARelicSelection).Name = StringName.op_Implicit("NChooseACardSelectionScreen");
		nChooseARelicSelection._relics = relics;
		NOverlayStack.Instance.Push(nChooseARelicSelection);
		return nChooseARelicSelection;
	}

	public override void _Ready()
	{
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		_banner = ((Node)this).GetNode<NCommonBanner>(NodePath.op_Implicit("Banner"));
		_banner.label.SetTextAutoSize(new LocString("gameplay_ui", "CHOOSE_RELIC_HEADER").GetRawText());
		_banner.AnimateIn();
		_relicRow = ((Node)this).GetNode<Control>(NodePath.op_Implicit("RelicRow"));
		Vector2 val = Vector2.Left * (float)(_relics.Count - 1) * 200f * 0.5f;
		for (int i = 0; i < _relics.Count; i++)
		{
			RelicModel relic = _relics[i];
			NRelicBasicHolder holder = NRelicBasicHolder.Create(relic);
			((Control)holder).Scale = Vector2.One * 2f;
			((Node)(object)_relicRow).AddChildSafely((Node?)(object)holder);
			((GodotObject)holder).Connect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)delegate
			{
				SelectHolder(holder);
			}), 0u);
			_cardTween = ((Node)this).CreateTween().SetParallel(true);
			_cardTween.TweenProperty((GodotObject)(object)holder, NodePath.op_Implicit("position"), Variant.op_Implicit(((Control)holder).Position + val + Vector2.Right * 200f * (float)i), 0.5).SetEase((EaseType)1).SetTrans((TransitionType)5);
			_cardTween.TweenProperty((GodotObject)(object)holder, NodePath.op_Implicit("modulate"), Variant.op_Implicit(Colors.White), 1.0).SetEase((EaseType)1).SetTrans((TransitionType)7)
				.From(Variant.op_Implicit(Colors.Black));
		}
		_skipButton = ((Node)this).GetNode<NChoiceSelectionSkipButton>(NodePath.op_Implicit("SkipButton"));
		((GodotObject)_skipButton).Connect(NClickableControl.SignalName.Released, Callable.From<NButton>((Action<NButton>)OnSkipButtonReleased), 0u);
		_skipButton.AnimateIn();
		List<NRelicBasicHolder> list = ((IEnumerable)((Node)_relicRow).GetChildren(false)).OfType<NRelicBasicHolder>().ToList();
		NRelicBasicHolder nRelicBasicHolder = ((IEnumerable)((Node)_relicRow).GetChildren(false)).OfType<NRelicBasicHolder>().ToList()[list.Count / 2];
		((Control)_skipButton).FocusNeighborTop = ((Node)nRelicBasicHolder).GetPath();
		((Control)_skipButton).FocusNeighborBottom = ((Node)_skipButton).GetPath();
		((Control)_skipButton).FocusNeighborLeft = ((Node)_skipButton).GetPath();
		((Control)_skipButton).FocusNeighborRight = ((Node)_skipButton).GetPath();
		for (int num = 0; num < ((Node)_relicRow).GetChildCount(false); num++)
		{
			Control child = ((Node)_relicRow).GetChild<Control>(num, false);
			child.FocusNeighborBottom = ((Node)child).GetPath();
			child.FocusNeighborTop = ((Node)child).GetPath();
			child.FocusNeighborLeft = ((num > 0) ? ((Node)_relicRow).GetChild(num - 1, false).GetPath() : ((Node)_relicRow).GetChild(((Node)_relicRow).GetChildCount(false) - 1, false).GetPath());
			child.FocusNeighborRight = ((num < ((Node)_relicRow).GetChildCount(false) - 1) ? ((Node)_relicRow).GetChild(num + 1, false).GetPath() : ((Node)_relicRow).GetChild(0, false).GetPath());
		}
	}

	public override void _ExitTree()
	{
		if (!_completionSource.Task.IsCompleted)
		{
			_completionSource.SetCanceled();
		}
	}

	private void SelectHolder(NRelicBasicHolder relicHolder)
	{
		RelicModel model = relicHolder.Relic.Model;
		_screenComplete = true;
		_relicSelected = true;
		_completionSource.SetResult(new RelicModel[1] { model });
	}

	public async Task<IEnumerable<RelicModel>> RelicsSelected()
	{
		IEnumerable<RelicModel> result = await _completionSource.Task;
		NOverlayStack.Instance.Remove(this);
		return result;
	}

	private void OnSkipButtonReleased(NButton _)
	{
		_screenComplete = true;
		_completionSource.SetResult(Array.Empty<RelicModel>());
	}

	public void AfterOverlayOpened()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		((CanvasItem)this).Modulate = Colors.Transparent;
		Tween? fadeTween = _fadeTween;
		if (fadeTween != null)
		{
			fadeTween.Kill();
		}
		_fadeTween = ((Node)this).CreateTween();
		_fadeTween.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(1f), 0.2);
	}

	public void AfterOverlayClosed()
	{
		Tween? fadeTween = _fadeTween;
		if (fadeTween != null)
		{
			fadeTween.Kill();
		}
		((Node)(object)this).QueueFreeSafely();
	}

	public void AfterOverlayShown()
	{
		((CanvasItem)this).Visible = true;
	}

	public void AfterOverlayHidden()
	{
		((CanvasItem)this).Visible = false;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Expected O, but got Unknown
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Expected O, but got Unknown
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(8);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._ExitTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SelectHolder, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("relicHolder"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnSkipButtonReleased, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AfterOverlayOpened, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AfterOverlayClosed, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AfterOverlayShown, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AfterOverlayHidden, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
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
		if ((ref method) == MethodName.SelectHolder && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			SelectHolder(VariantUtils.ConvertTo<NRelicBasicHolder>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnSkipButtonReleased && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OnSkipButtonReleased(VariantUtils.ConvertTo<NButton>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.AfterOverlayOpened && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			AfterOverlayOpened();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.AfterOverlayClosed && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			AfterOverlayClosed();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.AfterOverlayShown && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			AfterOverlayShown();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.AfterOverlayHidden && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			AfterOverlayHidden();
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
		if ((ref method) == MethodName._ExitTree)
		{
			return true;
		}
		if ((ref method) == MethodName.SelectHolder)
		{
			return true;
		}
		if ((ref method) == MethodName.OnSkipButtonReleased)
		{
			return true;
		}
		if ((ref method) == MethodName.AfterOverlayOpened)
		{
			return true;
		}
		if ((ref method) == MethodName.AfterOverlayClosed)
		{
			return true;
		}
		if ((ref method) == MethodName.AfterOverlayShown)
		{
			return true;
		}
		if ((ref method) == MethodName.AfterOverlayHidden)
		{
			return true;
		}
		return ((Control)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._banner)
		{
			_banner = VariantUtils.ConvertTo<NCommonBanner>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._relicRow)
		{
			_relicRow = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._skipButton)
		{
			_skipButton = VariantUtils.ConvertTo<NChoiceSelectionSkipButton>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._screenComplete)
		{
			_screenComplete = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._relicSelected)
		{
			_relicSelected = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._cardTween)
		{
			_cardTween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._fadeTween)
		{
			_fadeTween = VariantUtils.ConvertTo<Tween>(ref value);
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
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName.ScreenType)
		{
			NetScreenType screenType = ScreenType;
			value = VariantUtils.CreateFrom<NetScreenType>(ref screenType);
			return true;
		}
		if ((ref name) == PropertyName.UseSharedBackstop)
		{
			bool useSharedBackstop = UseSharedBackstop;
			value = VariantUtils.CreateFrom<bool>(ref useSharedBackstop);
			return true;
		}
		if ((ref name) == PropertyName.DefaultFocusedControl)
		{
			Control defaultFocusedControl = DefaultFocusedControl;
			value = VariantUtils.CreateFrom<Control>(ref defaultFocusedControl);
			return true;
		}
		if ((ref name) == PropertyName._banner)
		{
			value = VariantUtils.CreateFrom<NCommonBanner>(ref _banner);
			return true;
		}
		if ((ref name) == PropertyName._relicRow)
		{
			value = VariantUtils.CreateFrom<Control>(ref _relicRow);
			return true;
		}
		if ((ref name) == PropertyName._skipButton)
		{
			value = VariantUtils.CreateFrom<NChoiceSelectionSkipButton>(ref _skipButton);
			return true;
		}
		if ((ref name) == PropertyName._screenComplete)
		{
			value = VariantUtils.CreateFrom<bool>(ref _screenComplete);
			return true;
		}
		if ((ref name) == PropertyName._relicSelected)
		{
			value = VariantUtils.CreateFrom<bool>(ref _relicSelected);
			return true;
		}
		if ((ref name) == PropertyName._cardTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _cardTween);
			return true;
		}
		if ((ref name) == PropertyName._fadeTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _fadeTween);
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
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._banner, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._relicRow, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._skipButton, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._screenComplete, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._relicSelected, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._cardTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._fadeTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)2, PropertyName.ScreenType, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName.UseSharedBackstop, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.DefaultFocusedControl, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		info.AddProperty(PropertyName._banner, Variant.From<NCommonBanner>(ref _banner));
		info.AddProperty(PropertyName._relicRow, Variant.From<Control>(ref _relicRow));
		info.AddProperty(PropertyName._skipButton, Variant.From<NChoiceSelectionSkipButton>(ref _skipButton));
		info.AddProperty(PropertyName._screenComplete, Variant.From<bool>(ref _screenComplete));
		info.AddProperty(PropertyName._relicSelected, Variant.From<bool>(ref _relicSelected));
		info.AddProperty(PropertyName._cardTween, Variant.From<Tween>(ref _cardTween));
		info.AddProperty(PropertyName._fadeTween, Variant.From<Tween>(ref _fadeTween));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._banner, ref val))
		{
			_banner = ((Variant)(ref val)).As<NCommonBanner>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._relicRow, ref val2))
		{
			_relicRow = ((Variant)(ref val2)).As<Control>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._skipButton, ref val3))
		{
			_skipButton = ((Variant)(ref val3)).As<NChoiceSelectionSkipButton>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._screenComplete, ref val4))
		{
			_screenComplete = ((Variant)(ref val4)).As<bool>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._relicSelected, ref val5))
		{
			_relicSelected = ((Variant)(ref val5)).As<bool>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._cardTween, ref val6))
		{
			_cardTween = ((Variant)(ref val6)).As<Tween>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName._fadeTween, ref val7))
		{
			_fadeTween = ((Variant)(ref val7)).As<Tween>();
		}
	}
}
