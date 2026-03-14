using System;
using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;

namespace MegaCrit.Sts2.Core.Nodes.Screens.CharacterSelect;

[ScriptPath("res://src/Core/Nodes/Screens/CharacterSelect/NRegentCharacterSelectBg.cs")]
public class NRegentCharacterSelectBg : Control
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName SetSkin = StringName.op_Implicit("SetSkin");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName _sphereGuardianHover = StringName.op_Implicit("_sphereGuardianHover");

		public static readonly StringName _decaHover = StringName.op_Implicit("_decaHover");

		public static readonly StringName _sentryHover = StringName.op_Implicit("_sentryHover");

		public static readonly StringName _sneckoHover = StringName.op_Implicit("_sneckoHover");

		public static readonly StringName _cultistHover = StringName.op_Implicit("_cultistHover");

		public static readonly StringName _shapesHover = StringName.op_Implicit("_shapesHover");

		public static readonly StringName _amongusHover = StringName.op_Implicit("_amongusHover");
	}

	public class SignalName : SignalName
	{
	}

	private MegaSprite _spineController;

	private Control _sphereGuardianHover;

	private Control _decaHover;

	private Control _sentryHover;

	private Control _sneckoHover;

	private Control _cultistHover;

	private Control _shapesHover;

	private Control _amongusHover;

	public override void _Ready()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0298: Unknown result type (might be due to invalid IL or missing references)
		//IL_029e: Unknown result type (might be due to invalid IL or missing references)
		_spineController = new MegaSprite(Variant.op_Implicit((GodotObject)(object)((Node)this).GetNode(NodePath.op_Implicit("SpineSprite"))));
		_sphereGuardianHover = ((Node)this).GetNode<Control>(NodePath.op_Implicit("SphereGuardianHover"));
		((GodotObject)_sphereGuardianHover).Connect(SignalName.MouseEntered, Callable.From((Action)delegate
		{
			SetSkin("spheric guardian constellation");
		}), 0u);
		((GodotObject)_sphereGuardianHover).Connect(SignalName.MouseExited, Callable.From((Action)delegate
		{
			SetSkin("normal");
		}), 0u);
		_decaHover = ((Node)this).GetNode<Control>(NodePath.op_Implicit("DecaHover"));
		((GodotObject)_decaHover).Connect(SignalName.MouseEntered, Callable.From((Action)delegate
		{
			SetSkin("deca outline");
		}), 0u);
		((GodotObject)_decaHover).Connect(SignalName.MouseExited, Callable.From((Action)delegate
		{
			SetSkin("normal");
		}), 0u);
		_sentryHover = ((Node)this).GetNode<Control>(NodePath.op_Implicit("SentryHover"));
		((GodotObject)_sentryHover).Connect(SignalName.MouseEntered, Callable.From((Action)delegate
		{
			SetSkin("sentry constellation");
		}), 0u);
		((GodotObject)_sentryHover).Connect(SignalName.MouseExited, Callable.From((Action)delegate
		{
			SetSkin("normal");
		}), 0u);
		_sneckoHover = ((Node)this).GetNode<Control>(NodePath.op_Implicit("SneckoHover"));
		((GodotObject)_sneckoHover).Connect(SignalName.MouseEntered, Callable.From((Action)delegate
		{
			SetSkin("snecko constellation");
		}), 0u);
		((GodotObject)_sneckoHover).Connect(SignalName.MouseExited, Callable.From((Action)delegate
		{
			SetSkin("normal");
		}), 0u);
		_cultistHover = ((Node)this).GetNode<Control>(NodePath.op_Implicit("CultistHover"));
		((GodotObject)_cultistHover).Connect(SignalName.MouseEntered, Callable.From((Action)delegate
		{
			SetSkin("cultist constellation");
		}), 0u);
		((GodotObject)_cultistHover).Connect(SignalName.MouseExited, Callable.From((Action)delegate
		{
			SetSkin("normal");
		}), 0u);
		_shapesHover = ((Node)this).GetNode<Control>(NodePath.op_Implicit("ShapesHover"));
		((GodotObject)_shapesHover).Connect(SignalName.MouseEntered, Callable.From((Action)delegate
		{
			SetSkin("shapes constellation");
		}), 0u);
		((GodotObject)_shapesHover).Connect(SignalName.MouseExited, Callable.From((Action)delegate
		{
			SetSkin("normal");
		}), 0u);
		_amongusHover = ((Node)this).GetNode<Control>(NodePath.op_Implicit("AmongusHover"));
		((GodotObject)_amongusHover).Connect(SignalName.MouseEntered, Callable.From((Action)delegate
		{
			SetSkin("amongus constellation");
		}), 0u);
		((GodotObject)_amongusHover).Connect(SignalName.MouseExited, Callable.From((Action)delegate
		{
			SetSkin("normal");
		}), 0u);
	}

	private void SetSkin(string skinName)
	{
		MegaSkeleton skeleton = _spineController.GetSkeleton();
		skeleton.SetSkin(skeleton.GetData().FindSkin(skinName));
		skeleton.SetSlotsToSetupPose();
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(2);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetSkin, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)4, StringName.op_Implicit("skinName"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetSkin && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			SetSkin(VariantUtils.ConvertTo<string>(ref ((NativeVariantPtrArgs)(ref args))[0]));
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
		if ((ref method) == MethodName.SetSkin)
		{
			return true;
		}
		return ((Control)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._sphereGuardianHover)
		{
			_sphereGuardianHover = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._decaHover)
		{
			_decaHover = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._sentryHover)
		{
			_sentryHover = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._sneckoHover)
		{
			_sneckoHover = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._cultistHover)
		{
			_cultistHover = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._shapesHover)
		{
			_shapesHover = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._amongusHover)
		{
			_amongusHover = VariantUtils.ConvertTo<Control>(ref value);
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
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName._sphereGuardianHover)
		{
			value = VariantUtils.CreateFrom<Control>(ref _sphereGuardianHover);
			return true;
		}
		if ((ref name) == PropertyName._decaHover)
		{
			value = VariantUtils.CreateFrom<Control>(ref _decaHover);
			return true;
		}
		if ((ref name) == PropertyName._sentryHover)
		{
			value = VariantUtils.CreateFrom<Control>(ref _sentryHover);
			return true;
		}
		if ((ref name) == PropertyName._sneckoHover)
		{
			value = VariantUtils.CreateFrom<Control>(ref _sneckoHover);
			return true;
		}
		if ((ref name) == PropertyName._cultistHover)
		{
			value = VariantUtils.CreateFrom<Control>(ref _cultistHover);
			return true;
		}
		if ((ref name) == PropertyName._shapesHover)
		{
			value = VariantUtils.CreateFrom<Control>(ref _shapesHover);
			return true;
		}
		if ((ref name) == PropertyName._amongusHover)
		{
			value = VariantUtils.CreateFrom<Control>(ref _amongusHover);
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
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._sphereGuardianHover, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._decaHover, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._sentryHover, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._sneckoHover, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._cultistHover, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._shapesHover, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._amongusHover, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		info.AddProperty(PropertyName._sphereGuardianHover, Variant.From<Control>(ref _sphereGuardianHover));
		info.AddProperty(PropertyName._decaHover, Variant.From<Control>(ref _decaHover));
		info.AddProperty(PropertyName._sentryHover, Variant.From<Control>(ref _sentryHover));
		info.AddProperty(PropertyName._sneckoHover, Variant.From<Control>(ref _sneckoHover));
		info.AddProperty(PropertyName._cultistHover, Variant.From<Control>(ref _cultistHover));
		info.AddProperty(PropertyName._shapesHover, Variant.From<Control>(ref _shapesHover));
		info.AddProperty(PropertyName._amongusHover, Variant.From<Control>(ref _amongusHover));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._sphereGuardianHover, ref val))
		{
			_sphereGuardianHover = ((Variant)(ref val)).As<Control>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._decaHover, ref val2))
		{
			_decaHover = ((Variant)(ref val2)).As<Control>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._sentryHover, ref val3))
		{
			_sentryHover = ((Variant)(ref val3)).As<Control>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._sneckoHover, ref val4))
		{
			_sneckoHover = ((Variant)(ref val4)).As<Control>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._cultistHover, ref val5))
		{
			_cultistHover = ((Variant)(ref val5)).As<Control>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._shapesHover, ref val6))
		{
			_shapesHover = ((Variant)(ref val6)).As<Control>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName._amongusHover, ref val7))
		{
			_amongusHover = ((Variant)(ref val7)).As<Control>();
		}
	}
}
