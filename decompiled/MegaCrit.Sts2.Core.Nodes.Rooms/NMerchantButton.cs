using System;
using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.ControllerInput;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Nodes.Vfx.Utilities;
using MegaCrit.Sts2.Core.Random;

namespace MegaCrit.Sts2.Core.Nodes.Rooms;

[ScriptPath("res://src/Core/Nodes/Rooms/NMerchantButton.cs")]
public class NMerchantButton : NButton
{
	[Signal]
	public delegate void MerchantOpenedEventHandler(NMerchantButton merchantButton);

	public new class MethodName : NButton.MethodName
	{
		public new static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public new static readonly StringName OnFocus = StringName.op_Implicit("OnFocus");

		public new static readonly StringName OnUnfocus = StringName.op_Implicit("OnUnfocus");

		public new static readonly StringName OnRelease = StringName.op_Implicit("OnRelease");

		public new static readonly StringName RefreshFocus = StringName.op_Implicit("RefreshFocus");
	}

	public new class PropertyName : NButton.PropertyName
	{
		public new static readonly StringName Hotkeys = StringName.op_Implicit("Hotkeys");

		public static readonly StringName IsLocalPlayerDead = StringName.op_Implicit("IsLocalPlayerDead");

		public static readonly StringName _merchantSelectionReticle = StringName.op_Implicit("_merchantSelectionReticle");

		public static readonly StringName _focusedWhileTargeting = StringName.op_Implicit("_focusedWhileTargeting");
	}

	public new class SignalName : NButton.SignalName
	{
		public static readonly StringName MerchantOpened = StringName.op_Implicit("MerchantOpened");
	}

	private MegaSkeleton _merchantSkeleton;

	private NSelectionReticle _merchantSelectionReticle;

	private bool _focusedWhileTargeting;

	private MerchantOpenedEventHandler backing_MerchantOpened;

	protected override string[] Hotkeys => new string[1] { StringName.op_Implicit(MegaInput.select) };

	public bool IsLocalPlayerDead { get; set; }

	public IReadOnlyList<LocString> PlayerDeadLines { get; set; } = Array.Empty<LocString>();

	public event MerchantOpenedEventHandler MerchantOpened
	{
		add
		{
			backing_MerchantOpened = (MerchantOpenedEventHandler)Delegate.Combine(backing_MerchantOpened, value);
		}
		remove
		{
			backing_MerchantOpened = (MerchantOpenedEventHandler)Delegate.Remove(backing_MerchantOpened, value);
		}
	}

	public override void _Ready()
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		ConnectSignals();
		_merchantSelectionReticle = ((Node)this).GetNode<NSelectionReticle>(NodePath.op_Implicit("%MerchantSelectionReticle"));
		MegaSprite megaSprite = new MegaSprite(Variant.op_Implicit((GodotObject)(object)((Node)this).GetNode(NodePath.op_Implicit("%MerchantVisual"))));
		_merchantSkeleton = megaSprite.GetSkeleton();
		megaSprite.GetAnimationState().SetAnimation("idle_loop");
	}

	protected override void OnFocus()
	{
		base.OnFocus();
		RefreshFocus();
	}

	protected override void OnUnfocus()
	{
		base.OnUnfocus();
		_merchantSelectionReticle.OnDeselect();
		if (_focusedWhileTargeting)
		{
			NTargetManager.Instance.OnNodeUnhovered((Node)(object)this);
		}
		else
		{
			_merchantSkeleton.SetSkinByName("default");
			_merchantSkeleton.SetSlotsToSetupPose();
		}
		_focusedWhileTargeting = false;
	}

	protected override void OnRelease()
	{
		if (_focusedWhileTargeting)
		{
			_merchantSelectionReticle.OnDeselect();
			_focusedWhileTargeting = false;
			RefreshFocus();
		}
		else if (IsLocalPlayerDead)
		{
			LocString locString = Rng.Chaotic.NextItem(PlayerDeadLines);
			if (locString != null)
			{
				PlayDialogue(locString);
			}
		}
		else
		{
			EmitSignalMerchantOpened(this);
		}
	}

	public NSpeechBubbleVfx? PlayDialogue(LocString line, double duration = 2.0)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		NSpeechBubbleVfx nSpeechBubbleVfx = NSpeechBubbleVfx.Create(line.GetFormattedText(), DialogueSide.Right, ((Control)this).GlobalPosition + ((Control)this).Size.X * Vector2.Left, duration, VfxColor.Blue);
		if (nSpeechBubbleVfx != null)
		{
			((Node)this).GetParent().AddChildSafely((Node?)(object)nSpeechBubbleVfx);
		}
		return nSpeechBubbleVfx;
	}

	private void RefreshFocus()
	{
		if (NTargetManager.Instance.IsInSelection && NTargetManager.Instance.AllowedToTargetNode((Node)(object)this))
		{
			NTargetManager.Instance.OnNodeHovered((Node)(object)this);
			_merchantSelectionReticle.OnSelect();
			_focusedWhileTargeting = true;
		}
		else
		{
			_merchantSkeleton.SetSkinByName("outline");
			_merchantSkeleton.SetSlotsToSetupPose();
			_focusedWhileTargeting = false;
		}
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
		list.Add(new MethodInfo(MethodName.OnFocus, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnUnfocus, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnRelease, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.RefreshFocus, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
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
		if ((ref method) == MethodName.OnRelease && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnRelease();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.RefreshFocus && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			RefreshFocus();
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
		if ((ref method) == MethodName.OnFocus)
		{
			return true;
		}
		if ((ref method) == MethodName.OnUnfocus)
		{
			return true;
		}
		if ((ref method) == MethodName.OnRelease)
		{
			return true;
		}
		if ((ref method) == MethodName.RefreshFocus)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName.IsLocalPlayerDead)
		{
			IsLocalPlayerDead = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._merchantSelectionReticle)
		{
			_merchantSelectionReticle = VariantUtils.ConvertTo<NSelectionReticle>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._focusedWhileTargeting)
		{
			_focusedWhileTargeting = VariantUtils.ConvertTo<bool>(ref value);
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
		if ((ref name) == PropertyName.Hotkeys)
		{
			string[] hotkeys = Hotkeys;
			value = VariantUtils.CreateFrom<string[]>(ref hotkeys);
			return true;
		}
		if ((ref name) == PropertyName.IsLocalPlayerDead)
		{
			bool isLocalPlayerDead = IsLocalPlayerDead;
			value = VariantUtils.CreateFrom<bool>(ref isLocalPlayerDead);
			return true;
		}
		if ((ref name) == PropertyName._merchantSelectionReticle)
		{
			value = VariantUtils.CreateFrom<NSelectionReticle>(ref _merchantSelectionReticle);
			return true;
		}
		if ((ref name) == PropertyName._focusedWhileTargeting)
		{
			value = VariantUtils.CreateFrom<bool>(ref _focusedWhileTargeting);
			return true;
		}
		return base.GetGodotClassPropertyValue(in name, out value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)34, PropertyName.Hotkeys, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._merchantSelectionReticle, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._focusedWhileTargeting, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName.IsLocalPlayerDead, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		base.SaveGodotObjectData(info);
		StringName isLocalPlayerDead = PropertyName.IsLocalPlayerDead;
		bool isLocalPlayerDead2 = IsLocalPlayerDead;
		info.AddProperty(isLocalPlayerDead, Variant.From<bool>(ref isLocalPlayerDead2));
		info.AddProperty(PropertyName._merchantSelectionReticle, Variant.From<NSelectionReticle>(ref _merchantSelectionReticle));
		info.AddProperty(PropertyName._focusedWhileTargeting, Variant.From<bool>(ref _focusedWhileTargeting));
		info.AddSignalEventDelegate(SignalName.MerchantOpened, (Delegate)backing_MerchantOpened);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName.IsLocalPlayerDead, ref val))
		{
			IsLocalPlayerDead = ((Variant)(ref val)).As<bool>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._merchantSelectionReticle, ref val2))
		{
			_merchantSelectionReticle = ((Variant)(ref val2)).As<NSelectionReticle>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._focusedWhileTargeting, ref val3))
		{
			_focusedWhileTargeting = ((Variant)(ref val3)).As<bool>();
		}
		MerchantOpenedEventHandler merchantOpenedEventHandler = default(MerchantOpenedEventHandler);
		if (info.TryGetSignalEventDelegate<MerchantOpenedEventHandler>(SignalName.MerchantOpened, ref merchantOpenedEventHandler))
		{
			backing_MerchantOpened = merchantOpenedEventHandler;
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
		list.Add(new MethodInfo(SignalName.MerchantOpened, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("merchantButton"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		return list;
	}

	protected void EmitSignalMerchantOpened(NMerchantButton merchantButton)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).EmitSignal(SignalName.MerchantOpened, (Variant[])(object)new Variant[1] { Variant.op_Implicit((GodotObject)(object)merchantButton) });
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RaiseGodotClassSignalCallbacks(in godot_string_name signal, NativeVariantPtrArgs args)
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		if ((ref signal) == SignalName.MerchantOpened && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			backing_MerchantOpened?.Invoke(VariantUtils.ConvertTo<NMerchantButton>(ref ((NativeVariantPtrArgs)(ref args))[0]));
		}
		else
		{
			base.RaiseGodotClassSignalCallbacks(in signal, args);
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassSignal(in godot_string_name signal)
	{
		if ((ref signal) == SignalName.MerchantOpened)
		{
			return true;
		}
		return base.HasGodotClassSignal(in signal);
	}
}
