using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Audio.Debug;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.ControllerInput;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Screens.Capstones;
using MegaCrit.Sts2.Core.Nodes.Screens.Overlays;

namespace MegaCrit.Sts2.Core.Nodes.Combat;

[ScriptPath("res://src/Core/Nodes/Combat/NPeekButton.cs")]
public class NPeekButton : NButton
{
	[Signal]
	public delegate void ToggledEventHandler(NPeekButton peekButton);

	public new class MethodName : NButton.MethodName
	{
		public new static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public new static readonly StringName OnEnable = StringName.op_Implicit("OnEnable");

		public new static readonly StringName OnDisable = StringName.op_Implicit("OnDisable");

		public static readonly StringName OnOverlayStackChanged = StringName.op_Implicit("OnOverlayStackChanged");

		public static readonly StringName Wiggle = StringName.op_Implicit("Wiggle");

		public static readonly StringName AddTargets = StringName.op_Implicit("AddTargets");

		public static readonly StringName SetPeeking = StringName.op_Implicit("SetPeeking");

		public new static readonly StringName OnRelease = StringName.op_Implicit("OnRelease");

		public new static readonly StringName OnPress = StringName.op_Implicit("OnPress");

		public new static readonly StringName OnFocus = StringName.op_Implicit("OnFocus");

		public new static readonly StringName OnUnfocus = StringName.op_Implicit("OnUnfocus");

		public static readonly StringName OnCombatRoomReady = StringName.op_Implicit("OnCombatRoomReady");
	}

	public new class PropertyName : NButton.PropertyName
	{
		public new static readonly StringName Hotkeys = StringName.op_Implicit("Hotkeys");

		public static readonly StringName IsPeeking = StringName.op_Implicit("IsPeeking");

		public static readonly StringName CurrentCardMarker = StringName.op_Implicit("CurrentCardMarker");

		public static readonly StringName _flash = StringName.op_Implicit("_flash");

		public static readonly StringName _visuals = StringName.op_Implicit("_visuals");

		public static readonly StringName _hoverTween = StringName.op_Implicit("_hoverTween");

		public static readonly StringName _wiggleTween = StringName.op_Implicit("_wiggleTween");
	}

	public new class SignalName : NButton.SignalName
	{
		public static readonly StringName Toggled = StringName.op_Implicit("Toggled");
	}

	private static readonly StringName _pulseStrength = new StringName("pulse_strength");

	private readonly List<Control> _targets = new List<Control>();

	private readonly List<Control> _hiddenTargets = new List<Control>();

	private TextureRect _flash;

	private Control _visuals;

	private IOverlayScreen? _overlayScreenParent;

	private Tween? _hoverTween;

	private Tween? _wiggleTween;

	private ToggledEventHandler backing_Toggled;

	protected override string[] Hotkeys => new string[1] { StringName.op_Implicit(MegaInput.peek) };

	public bool IsPeeking { get; private set; }

	public Marker2D CurrentCardMarker { get; private set; }

	public event ToggledEventHandler Toggled
	{
		add
		{
			backing_Toggled = (ToggledEventHandler)Delegate.Combine(backing_Toggled, value);
		}
		remove
		{
			backing_Toggled = (ToggledEventHandler)Delegate.Remove(backing_Toggled, value);
		}
	}

	public override void _Ready()
	{
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		ConnectSignals();
		_flash = ((Node)this).GetNode<TextureRect>(NodePath.op_Implicit("%Flash"));
		_visuals = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%Visuals"));
		CurrentCardMarker = ((Node)this).GetNode<Marker2D>(NodePath.op_Implicit("%CurrentCardMarker"));
		if (NCombatRoom.Instance != null)
		{
			if (((Node)NCombatRoom.Instance).IsNodeReady())
			{
				OnCombatRoomReady();
			}
			else
			{
				((GodotObject)NCombatRoom.Instance).Connect(SignalName.Ready, Callable.From((Action)OnCombatRoomReady), 0u);
			}
		}
		if (!CombatManager.Instance.IsInProgress)
		{
			((CanvasItem)this).Visible = false;
		}
		for (Node parent = ((Node)this).GetParent(); parent != null; parent = parent.GetParent())
		{
			if (parent is IOverlayScreen overlayScreenParent)
			{
				_overlayScreenParent = overlayScreenParent;
				NOverlayStack? instance = NOverlayStack.Instance;
				if (instance != null)
				{
					((GodotObject)instance).Connect(NOverlayStack.SignalName.Changed, Callable.From((Action)OnOverlayStackChanged), 0u);
				}
				NCapstoneContainer? instance2 = NCapstoneContainer.Instance;
				if (instance2 != null)
				{
					((GodotObject)instance2).Connect(NCapstoneContainer.SignalName.Changed, Callable.From((Action)OnOverlayStackChanged), 0u);
				}
				break;
			}
		}
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		((CanvasItem)this).Visible = true;
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		((CanvasItem)this).Visible = false;
	}

	private void OnOverlayStackChanged()
	{
		if (IsPeeking && _overlayScreenParent != null && NCapstoneContainer.Instance?.CurrentCapstoneScreen == null && _overlayScreenParent == NOverlayStack.Instance?.Peek())
		{
			NOverlayStack.Instance.HideBackstop();
		}
	}

	public void Wiggle()
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		((CanvasItem)_flash).Visible = true;
		TextureRect flash = _flash;
		Color modulate = ((CanvasItem)_flash).Modulate;
		modulate.A = 0f;
		((CanvasItem)flash).Modulate = modulate;
		Tween? wiggleTween = _wiggleTween;
		if (wiggleTween != null)
		{
			wiggleTween.Kill();
		}
		_wiggleTween = ((Node)this).CreateTween();
		_visuals.RotationDegrees = 0f;
		_wiggleTween.TweenMethod(Callable.From<float>((Action<float>)delegate(float t)
		{
			_visuals.RotationDegrees = 10f * Mathf.Sin(t * 3f) * Mathf.Sin(t * 0.5f);
		}), Variant.op_Implicit(0f), Variant.op_Implicit((float)Math.PI * 2f), 0.5);
		_wiggleTween.Parallel().TweenMethod(Callable.From<float>((Action<float>)delegate(float t)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			_visuals.Scale = Vector2.One + Vector2.One * 0.15f * Mathf.Sin(t) * Mathf.Sin(t * 0.5f);
		}), Variant.op_Implicit(0f), Variant.op_Implicit((float)Math.PI), 0.25);
		_wiggleTween.Parallel().TweenProperty((GodotObject)(object)_flash, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(1f), 0.1);
		_wiggleTween.Chain().TweenProperty((GodotObject)(object)_flash, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0f), 0.3).SetTrans((TransitionType)4)
			.SetEase((EaseType)1);
		NDebugAudioManager.Instance.Play("deny.mp3", 0.5f, PitchVariance.Medium);
	}

	public void AddTargets(params Control[] targets)
	{
		_targets.AddRange(targets);
	}

	public void SetPeeking(bool isPeeking)
	{
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		if (IsPeeking == isPeeking)
		{
			return;
		}
		IsPeeking = isPeeking;
		if (NOverlayStack.Instance.ScreenCount > 0)
		{
			if (IsPeeking)
			{
				NOverlayStack.Instance.HideBackstop();
			}
			else
			{
				NOverlayStack.Instance.ShowBackstop();
			}
		}
		if (IsPeeking)
		{
			foreach (Control item in _targets.Where((Control t) => ((CanvasItem)t).Visible))
			{
				_hiddenTargets.Add(item);
				((CanvasItem)item).Visible = false;
			}
		}
		else
		{
			foreach (Control hiddenTarget in _hiddenTargets)
			{
				((CanvasItem)hiddenTarget).Visible = true;
			}
			_hiddenTargets.Clear();
		}
		((ShaderMaterial)((CanvasItem)_visuals).Material).SetShaderParameter(_pulseStrength, Variant.op_Implicit(IsPeeking ? 1 : 0));
		((GodotObject)this).EmitSignal(SignalName.Toggled, (Variant[])(object)new Variant[1] { Variant.op_Implicit((GodotObject)(object)this) });
	}

	protected override void OnRelease()
	{
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		SetPeeking(!IsPeeking);
		Tween? hoverTween = _hoverTween;
		if (hoverTween != null)
		{
			hoverTween.Kill();
		}
		_hoverTween = ((Node)this).CreateTween();
		_hoverTween.TweenProperty((GodotObject)(object)_visuals, NodePath.op_Implicit("scale"), Variant.op_Implicit(Vector2.One), 0.15);
	}

	protected override void OnPress()
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		base.OnPress();
		Tween? hoverTween = _hoverTween;
		if (hoverTween != null)
		{
			hoverTween.Kill();
		}
		_hoverTween = ((Node)this).CreateTween();
		_hoverTween.TweenProperty((GodotObject)(object)_visuals, NodePath.op_Implicit("scale"), Variant.op_Implicit(Vector2.One * 0.95f), 0.05);
	}

	protected override void OnFocus()
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		base.OnFocus();
		Tween? hoverTween = _hoverTween;
		if (hoverTween != null)
		{
			hoverTween.Kill();
		}
		_hoverTween = ((Node)this).CreateTween();
		_hoverTween.TweenProperty((GodotObject)(object)_visuals, NodePath.op_Implicit("scale"), Variant.op_Implicit(Vector2.One * 1.1f), 0.05);
	}

	protected override void OnUnfocus()
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		base.OnUnfocus();
		Tween? hoverTween = _hoverTween;
		if (hoverTween != null)
		{
			hoverTween.Kill();
		}
		_hoverTween = ((Node)this).CreateTween();
		_hoverTween.TweenProperty((GodotObject)(object)_visuals, NodePath.op_Implicit("scale"), Variant.op_Implicit(Vector2.One), 0.15);
	}

	private void OnCombatRoomReady()
	{
		NCombatRoom.Instance.Ui.OnPeekButtonReady(this);
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
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(12);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnEnable, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnDisable, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnOverlayStackChanged, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.Wiggle, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AddTargets, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)28, StringName.op_Implicit("targets"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetPeeking, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)1, StringName.op_Implicit("isPeeking"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnRelease, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnPress, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnFocus, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnUnfocus, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnCombatRoomReady, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
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
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnEnable && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnEnable();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnDisable && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnDisable();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnOverlayStackChanged && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnOverlayStackChanged();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.Wiggle && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			Wiggle();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.AddTargets && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			AddTargets(VariantUtils.ConvertToSystemArrayOfGodotObject<Control>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetPeeking && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			SetPeeking(VariantUtils.ConvertTo<bool>(ref ((NativeVariantPtrArgs)(ref args))[0]));
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
		if ((ref method) == MethodName.OnCombatRoomReady && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnCombatRoomReady();
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
		if ((ref method) == MethodName.OnEnable)
		{
			return true;
		}
		if ((ref method) == MethodName.OnDisable)
		{
			return true;
		}
		if ((ref method) == MethodName.OnOverlayStackChanged)
		{
			return true;
		}
		if ((ref method) == MethodName.Wiggle)
		{
			return true;
		}
		if ((ref method) == MethodName.AddTargets)
		{
			return true;
		}
		if ((ref method) == MethodName.SetPeeking)
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
		if ((ref method) == MethodName.OnCombatRoomReady)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName.IsPeeking)
		{
			IsPeeking = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName.CurrentCardMarker)
		{
			CurrentCardMarker = VariantUtils.ConvertTo<Marker2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._flash)
		{
			_flash = VariantUtils.ConvertTo<TextureRect>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._visuals)
		{
			_visuals = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._hoverTween)
		{
			_hoverTween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._wiggleTween)
		{
			_wiggleTween = VariantUtils.ConvertTo<Tween>(ref value);
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
		if ((ref name) == PropertyName.Hotkeys)
		{
			string[] hotkeys = Hotkeys;
			value = VariantUtils.CreateFrom<string[]>(ref hotkeys);
			return true;
		}
		if ((ref name) == PropertyName.IsPeeking)
		{
			bool isPeeking = IsPeeking;
			value = VariantUtils.CreateFrom<bool>(ref isPeeking);
			return true;
		}
		if ((ref name) == PropertyName.CurrentCardMarker)
		{
			Marker2D currentCardMarker = CurrentCardMarker;
			value = VariantUtils.CreateFrom<Marker2D>(ref currentCardMarker);
			return true;
		}
		if ((ref name) == PropertyName._flash)
		{
			value = VariantUtils.CreateFrom<TextureRect>(ref _flash);
			return true;
		}
		if ((ref name) == PropertyName._visuals)
		{
			value = VariantUtils.CreateFrom<Control>(ref _visuals);
			return true;
		}
		if ((ref name) == PropertyName._hoverTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _hoverTween);
			return true;
		}
		if ((ref name) == PropertyName._wiggleTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _wiggleTween);
			return true;
		}
		return base.GetGodotClassPropertyValue(in name, out value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)34, PropertyName.Hotkeys, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName.IsPeeking, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._flash, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._visuals, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._hoverTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._wiggleTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.CurrentCardMarker, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		base.SaveGodotObjectData(info);
		StringName isPeeking = PropertyName.IsPeeking;
		bool isPeeking2 = IsPeeking;
		info.AddProperty(isPeeking, Variant.From<bool>(ref isPeeking2));
		StringName currentCardMarker = PropertyName.CurrentCardMarker;
		Marker2D currentCardMarker2 = CurrentCardMarker;
		info.AddProperty(currentCardMarker, Variant.From<Marker2D>(ref currentCardMarker2));
		info.AddProperty(PropertyName._flash, Variant.From<TextureRect>(ref _flash));
		info.AddProperty(PropertyName._visuals, Variant.From<Control>(ref _visuals));
		info.AddProperty(PropertyName._hoverTween, Variant.From<Tween>(ref _hoverTween));
		info.AddProperty(PropertyName._wiggleTween, Variant.From<Tween>(ref _wiggleTween));
		info.AddSignalEventDelegate(SignalName.Toggled, (Delegate)backing_Toggled);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName.IsPeeking, ref val))
		{
			IsPeeking = ((Variant)(ref val)).As<bool>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName.CurrentCardMarker, ref val2))
		{
			CurrentCardMarker = ((Variant)(ref val2)).As<Marker2D>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._flash, ref val3))
		{
			_flash = ((Variant)(ref val3)).As<TextureRect>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._visuals, ref val4))
		{
			_visuals = ((Variant)(ref val4)).As<Control>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._hoverTween, ref val5))
		{
			_hoverTween = ((Variant)(ref val5)).As<Tween>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._wiggleTween, ref val6))
		{
			_wiggleTween = ((Variant)(ref val6)).As<Tween>();
		}
		ToggledEventHandler toggledEventHandler = default(ToggledEventHandler);
		if (info.TryGetSignalEventDelegate<ToggledEventHandler>(SignalName.Toggled, ref toggledEventHandler))
		{
			backing_Toggled = toggledEventHandler;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<MethodInfo> GetGodotSignalList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Expected O, but got Unknown
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(1);
		list.Add(new MethodInfo(SignalName.Toggled, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("peekButton"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		return list;
	}

	protected void EmitSignalToggled(NPeekButton peekButton)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).EmitSignal(SignalName.Toggled, (Variant[])(object)new Variant[1] { Variant.op_Implicit((GodotObject)(object)peekButton) });
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RaiseGodotClassSignalCallbacks(in godot_string_name signal, NativeVariantPtrArgs args)
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		if ((ref signal) == SignalName.Toggled && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			backing_Toggled?.Invoke(VariantUtils.ConvertTo<NPeekButton>(ref ((NativeVariantPtrArgs)(ref args))[0]));
		}
		else
		{
			base.RaiseGodotClassSignalCallbacks(in signal, args);
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassSignal(in godot_string_name signal)
	{
		if ((ref signal) == SignalName.Toggled)
		{
			return true;
		}
		return base.HasGodotClassSignal(in signal);
	}
}
