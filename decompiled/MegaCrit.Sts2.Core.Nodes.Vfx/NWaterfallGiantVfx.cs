using System;
using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;

namespace MegaCrit.Sts2.Core.Nodes.Vfx;

[GlobalClass]
[ScriptPath("res://src/Core/Nodes/Vfx/NWaterfallGiantVfx.cs")]
public class NWaterfallGiantVfx : Node
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName OnAnimationEvent = StringName.op_Implicit("OnAnimationEvent");

		public static readonly StringName StartSteam1 = StringName.op_Implicit("StartSteam1");

		public static readonly StringName EndSteam1 = StringName.op_Implicit("EndSteam1");

		public static readonly StringName StartSteam2 = StringName.op_Implicit("StartSteam2");

		public static readonly StringName EndSteam2 = StringName.op_Implicit("EndSteam2");

		public static readonly StringName StartSteam3 = StringName.op_Implicit("StartSteam3");

		public static readonly StringName EndSteam3 = StringName.op_Implicit("EndSteam3");

		public static readonly StringName StartSteam5 = StringName.op_Implicit("StartSteam5");

		public static readonly StringName EndSteam5 = StringName.op_Implicit("EndSteam5");

		public static readonly StringName StartWaterfall = StringName.op_Implicit("StartWaterfall");

		public static readonly StringName EndWaterfall = StringName.op_Implicit("EndWaterfall");

		public static readonly StringName Explode = StringName.op_Implicit("Explode");

		public static readonly StringName Buildup1 = StringName.op_Implicit("Buildup1");

		public static readonly StringName Buildup2 = StringName.op_Implicit("Buildup2");

		public static readonly StringName Buildup3 = StringName.op_Implicit("Buildup3");

		public static readonly StringName EmitGracefully = StringName.op_Implicit("EmitGracefully");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName _steam1Particles = StringName.op_Implicit("_steam1Particles");

		public static readonly StringName _steam2Particles = StringName.op_Implicit("_steam2Particles");

		public static readonly StringName _steam3Particles = StringName.op_Implicit("_steam3Particles");

		public static readonly StringName _steam4Particles = StringName.op_Implicit("_steam4Particles");

		public static readonly StringName _steam5Particles = StringName.op_Implicit("_steam5Particles");

		public static readonly StringName _steam6Particles = StringName.op_Implicit("_steam6Particles");

		public static readonly StringName _steamLeakParticles1 = StringName.op_Implicit("_steamLeakParticles1");

		public static readonly StringName _steamLeakParticles2 = StringName.op_Implicit("_steamLeakParticles2");

		public static readonly StringName _steamLeakParticles3 = StringName.op_Implicit("_steamLeakParticles3");

		public static readonly StringName _mistParticles = StringName.op_Implicit("_mistParticles");

		public static readonly StringName _mouthParticles = StringName.op_Implicit("_mouthParticles");

		public static readonly StringName _dropletParticles = StringName.op_Implicit("_dropletParticles");

		public static readonly StringName _isDead = StringName.op_Implicit("_isDead");

		public static readonly StringName _parent = StringName.op_Implicit("_parent");
	}

	public class SignalName : SignalName
	{
	}

	private GpuParticles2D _steam1Particles;

	private GpuParticles2D _steam2Particles;

	private GpuParticles2D _steam3Particles;

	private GpuParticles2D _steam4Particles;

	private GpuParticles2D _steam5Particles;

	private GpuParticles2D _steam6Particles;

	private GpuParticles2D _steamLeakParticles1;

	private GpuParticles2D _steamLeakParticles2;

	private GpuParticles2D _steamLeakParticles3;

	private GpuParticles2D _mistParticles;

	private GpuParticles2D _mouthParticles;

	private GpuParticles2D _dropletParticles;

	private bool _isDead;

	private Node2D _parent;

	private MegaSprite _animController;

	public override void _Ready()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		_parent = ((Node)this).GetParent<Node2D>();
		_animController = new MegaSprite(Variant.op_Implicit((GodotObject)(object)_parent));
		_animController.ConnectAnimationEvent(Callable.From<GodotObject, GodotObject, GodotObject, GodotObject>((Action<GodotObject, GodotObject, GodotObject, GodotObject>)OnAnimationEvent));
		_steam1Particles = ((Node)_parent).GetNode<GpuParticles2D>(NodePath.op_Implicit("SteamSlot1/steamParticles1"));
		_steam2Particles = ((Node)_parent).GetNode<GpuParticles2D>(NodePath.op_Implicit("SteamSlot2/steamParticles2"));
		_steam3Particles = ((Node)_parent).GetNode<GpuParticles2D>(NodePath.op_Implicit("SteamSlot3/steamParticles3"));
		_steam4Particles = ((Node)_parent).GetNode<GpuParticles2D>(NodePath.op_Implicit("SteamSlot4/steamParticles4"));
		_steam5Particles = ((Node)_parent).GetNode<GpuParticles2D>(NodePath.op_Implicit("SteamSlot5/steamParticles5"));
		_steam6Particles = ((Node)_parent).GetNode<GpuParticles2D>(NodePath.op_Implicit("SteamSlot6/steamParticles6"));
		_steamLeakParticles1 = ((Node)_parent).GetNode<GpuParticles2D>(NodePath.op_Implicit("SteamLeakSlot1/steamLeakParticles1"));
		_steamLeakParticles2 = ((Node)_parent).GetNode<GpuParticles2D>(NodePath.op_Implicit("SteamLeakSlot2/steamLeakParticles2"));
		_steamLeakParticles3 = ((Node)_parent).GetNode<GpuParticles2D>(NodePath.op_Implicit("SteamLeakSlot3/steamLeakParticles3"));
		_mistParticles = ((Node)_parent).GetNode<GpuParticles2D>(NodePath.op_Implicit("MistSlot/MistParticles"));
		_dropletParticles = ((Node)_parent).GetNode<GpuParticles2D>(NodePath.op_Implicit("MistSlot/Droplets"));
		_mouthParticles = ((Node)_parent).GetNode<GpuParticles2D>(NodePath.op_Implicit("MouthDropletsSlot/MouthDroplets"));
		_steam1Particles.Emitting = false;
		_steam2Particles.Emitting = false;
		_steam3Particles.Emitting = false;
		_steam4Particles.Emitting = false;
		_steam5Particles.Emitting = false;
		_steam6Particles.Emitting = false;
		_steamLeakParticles1.Emitting = false;
		_steamLeakParticles2.Emitting = false;
		_steamLeakParticles3.Emitting = false;
	}

	private void OnAnimationEvent(GodotObject _, GodotObject __, GodotObject ___, GodotObject spineEvent)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		string eventName = new MegaEvent(Variant.op_Implicit(spineEvent)).GetData().GetEventName();
		if (eventName == null)
		{
			return;
		}
		switch (eventName.Length)
		{
		case 13:
			switch (eventName[6])
			{
			case '1':
				if (eventName == "steam_1_start")
				{
					StartSteam1();
				}
				break;
			case '2':
				if (eventName == "steam_2_start")
				{
					StartSteam2();
				}
				break;
			case '3':
				if (eventName == "steam_3_start")
				{
					StartSteam3();
				}
				break;
			case '5':
				if (eventName == "steam_5_start")
				{
					StartSteam5();
				}
				break;
			case 'a':
				if (eventName == "waterfall_end")
				{
					EndWaterfall();
				}
				break;
			}
			break;
		case 11:
			switch (eventName[6])
			{
			case '1':
				if (eventName == "steam_1_end")
				{
					EndSteam1();
				}
				break;
			case '2':
				if (eventName == "steam_2_end")
				{
					EndSteam2();
				}
				break;
			case '3':
				if (eventName == "steam_3_end")
				{
					EndSteam3();
				}
				break;
			case '5':
				if (eventName == "steam_5_end")
				{
					EndSteam5();
				}
				break;
			case '4':
				break;
			}
			break;
		case 8:
			switch (eventName[7])
			{
			case '1':
				if (eventName == "buildup1")
				{
					Buildup1();
				}
				break;
			case '2':
				if (eventName == "buildup2")
				{
					Buildup2();
				}
				break;
			case '3':
				if (eventName == "buildup3")
				{
					Buildup3();
				}
				break;
			}
			break;
		case 15:
			if (eventName == "waterfall_start")
			{
				StartWaterfall();
			}
			break;
		case 7:
			if (eventName == "explode")
			{
				Explode();
			}
			break;
		case 9:
		case 10:
		case 12:
		case 14:
			break;
		}
	}

	private void StartSteam1()
	{
		EmitGracefully(_steam1Particles);
	}

	private void EndSteam1()
	{
		_steam1Particles.Emitting = false;
	}

	private void StartSteam2()
	{
		EmitGracefully(_steam2Particles);
	}

	private void EndSteam2()
	{
		_steam2Particles.Emitting = false;
	}

	private void StartSteam3()
	{
		EmitGracefully(_steam3Particles);
		EmitGracefully(_steam4Particles);
	}

	private void EndSteam3()
	{
		_steam3Particles.Emitting = false;
		_steam4Particles.Emitting = false;
	}

	private void StartSteam5()
	{
		EmitGracefully(_steam5Particles);
		EmitGracefully(_steam6Particles);
	}

	private void EndSteam5()
	{
		_steam5Particles.Emitting = false;
		_steam6Particles.Emitting = false;
	}

	private void StartWaterfall()
	{
		_mouthParticles.Emitting = true;
		_dropletParticles.Emitting = true;
		_mistParticles.Emitting = true;
		_isDead = false;
	}

	private void EndWaterfall()
	{
		_mouthParticles.Emitting = false;
		_dropletParticles.Emitting = false;
		_mistParticles.Emitting = false;
	}

	private void Explode()
	{
		((CanvasItem)_steam1Particles).Visible = false;
		((CanvasItem)_steam2Particles).Visible = false;
		((CanvasItem)_steam3Particles).Visible = false;
		((CanvasItem)_steam4Particles).Visible = false;
		((CanvasItem)_steam5Particles).Visible = false;
		((CanvasItem)_steam6Particles).Visible = false;
		((CanvasItem)_steamLeakParticles1).Visible = false;
		((CanvasItem)_steamLeakParticles2).Visible = false;
		((CanvasItem)_steamLeakParticles3).Visible = false;
		_isDead = true;
	}

	private void Buildup1()
	{
		if (!_isDead)
		{
			EmitGracefully(_steamLeakParticles1);
			EmitGracefully(_steamLeakParticles2);
			EmitGracefully(_steamLeakParticles3);
			GpuParticles2D steamLeakParticles = _steamLeakParticles1;
			GpuParticles2D steamLeakParticles2 = _steamLeakParticles2;
			int num = (_steamLeakParticles3.Amount = 8);
			int amount = (steamLeakParticles2.Amount = num);
			steamLeakParticles.Amount = amount;
			GpuParticles2D steamLeakParticles3 = _steamLeakParticles1;
			GpuParticles2D steamLeakParticles4 = _steamLeakParticles2;
			double num4 = (_steamLeakParticles3.Lifetime = 0.3700000047683716);
			double lifetime = (steamLeakParticles4.Lifetime = num4);
			steamLeakParticles3.Lifetime = lifetime;
		}
	}

	private void Buildup2()
	{
		if (!_isDead)
		{
			EmitGracefully(_steamLeakParticles1);
			EmitGracefully(_steamLeakParticles2);
			EmitGracefully(_steamLeakParticles3);
			GpuParticles2D steamLeakParticles = _steamLeakParticles1;
			GpuParticles2D steamLeakParticles2 = _steamLeakParticles2;
			int num = (_steamLeakParticles3.Amount = 15);
			int amount = (steamLeakParticles2.Amount = num);
			steamLeakParticles.Amount = amount;
			GpuParticles2D steamLeakParticles3 = _steamLeakParticles1;
			GpuParticles2D steamLeakParticles4 = _steamLeakParticles2;
			double num4 = (_steamLeakParticles3.Lifetime = 0.44999998807907104);
			double lifetime = (steamLeakParticles4.Lifetime = num4);
			steamLeakParticles3.Lifetime = lifetime;
		}
	}

	private void Buildup3()
	{
		if (!_isDead)
		{
			EmitGracefully(_steamLeakParticles1);
			EmitGracefully(_steamLeakParticles2);
			EmitGracefully(_steamLeakParticles3);
			GpuParticles2D steamLeakParticles = _steamLeakParticles1;
			GpuParticles2D steamLeakParticles2 = _steamLeakParticles2;
			int num = (_steamLeakParticles3.Amount = 20);
			int amount = (steamLeakParticles2.Amount = num);
			steamLeakParticles.Amount = amount;
			GpuParticles2D steamLeakParticles3 = _steamLeakParticles1;
			GpuParticles2D steamLeakParticles4 = _steamLeakParticles2;
			double num4 = (_steamLeakParticles3.Lifetime = 0.6000000238418579);
			double lifetime = (steamLeakParticles4.Lifetime = num4);
			steamLeakParticles3.Lifetime = lifetime;
		}
	}

	private void EmitGracefully(GpuParticles2D emitter)
	{
		if (!((CanvasItem)emitter).Visible)
		{
			((CanvasItem)emitter).Visible = true;
			emitter.Restart();
		}
		else
		{
			emitter.Emitting = true;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Expected O, but got Unknown
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Expected O, but got Unknown
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Expected O, but got Unknown
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Expected O, but got Unknown
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0288: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_030c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0315: Unknown result type (might be due to invalid IL or missing references)
		//IL_033b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0344: Unknown result type (might be due to invalid IL or missing references)
		//IL_036a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0373: Unknown result type (might be due to invalid IL or missing references)
		//IL_0399: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fb: Expected O, but got Unknown
		//IL_03f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0401: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(17);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnAnimationEvent, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("_"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Object"), false),
			new PropertyInfo((Type)24, StringName.op_Implicit("__"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Object"), false),
			new PropertyInfo((Type)24, StringName.op_Implicit("___"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Object"), false),
			new PropertyInfo((Type)24, StringName.op_Implicit("spineEvent"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Object"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.StartSteam1, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.EndSteam1, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.StartSteam2, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.EndSteam2, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.StartSteam3, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.EndSteam3, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.StartSteam5, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.EndSteam5, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.StartWaterfall, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.EndWaterfall, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.Explode, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.Buildup1, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.Buildup2, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.Buildup3, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.EmitGracefully, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("emitter"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("GPUParticles2D"), false)
		}, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnAnimationEvent && ((NativeVariantPtrArgs)(ref args)).Count == 4)
		{
			OnAnimationEvent(VariantUtils.ConvertTo<GodotObject>(ref ((NativeVariantPtrArgs)(ref args))[0]), VariantUtils.ConvertTo<GodotObject>(ref ((NativeVariantPtrArgs)(ref args))[1]), VariantUtils.ConvertTo<GodotObject>(ref ((NativeVariantPtrArgs)(ref args))[2]), VariantUtils.ConvertTo<GodotObject>(ref ((NativeVariantPtrArgs)(ref args))[3]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.StartSteam1 && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			StartSteam1();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.EndSteam1 && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			EndSteam1();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.StartSteam2 && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			StartSteam2();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.EndSteam2 && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			EndSteam2();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.StartSteam3 && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			StartSteam3();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.EndSteam3 && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			EndSteam3();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.StartSteam5 && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			StartSteam5();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.EndSteam5 && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			EndSteam5();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.StartWaterfall && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			StartWaterfall();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.EndWaterfall && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			EndWaterfall();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.Explode && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			Explode();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.Buildup1 && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			Buildup1();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.Buildup2 && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			Buildup2();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.Buildup3 && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			Buildup3();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.EmitGracefully && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			EmitGracefully(VariantUtils.ConvertTo<GpuParticles2D>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		return ((Node)this).InvokeGodotClassMethod(ref method, args, ref ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if ((ref method) == MethodName._Ready)
		{
			return true;
		}
		if ((ref method) == MethodName.OnAnimationEvent)
		{
			return true;
		}
		if ((ref method) == MethodName.StartSteam1)
		{
			return true;
		}
		if ((ref method) == MethodName.EndSteam1)
		{
			return true;
		}
		if ((ref method) == MethodName.StartSteam2)
		{
			return true;
		}
		if ((ref method) == MethodName.EndSteam2)
		{
			return true;
		}
		if ((ref method) == MethodName.StartSteam3)
		{
			return true;
		}
		if ((ref method) == MethodName.EndSteam3)
		{
			return true;
		}
		if ((ref method) == MethodName.StartSteam5)
		{
			return true;
		}
		if ((ref method) == MethodName.EndSteam5)
		{
			return true;
		}
		if ((ref method) == MethodName.StartWaterfall)
		{
			return true;
		}
		if ((ref method) == MethodName.EndWaterfall)
		{
			return true;
		}
		if ((ref method) == MethodName.Explode)
		{
			return true;
		}
		if ((ref method) == MethodName.Buildup1)
		{
			return true;
		}
		if ((ref method) == MethodName.Buildup2)
		{
			return true;
		}
		if ((ref method) == MethodName.Buildup3)
		{
			return true;
		}
		if ((ref method) == MethodName.EmitGracefully)
		{
			return true;
		}
		return ((Node)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._steam1Particles)
		{
			_steam1Particles = VariantUtils.ConvertTo<GpuParticles2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._steam2Particles)
		{
			_steam2Particles = VariantUtils.ConvertTo<GpuParticles2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._steam3Particles)
		{
			_steam3Particles = VariantUtils.ConvertTo<GpuParticles2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._steam4Particles)
		{
			_steam4Particles = VariantUtils.ConvertTo<GpuParticles2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._steam5Particles)
		{
			_steam5Particles = VariantUtils.ConvertTo<GpuParticles2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._steam6Particles)
		{
			_steam6Particles = VariantUtils.ConvertTo<GpuParticles2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._steamLeakParticles1)
		{
			_steamLeakParticles1 = VariantUtils.ConvertTo<GpuParticles2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._steamLeakParticles2)
		{
			_steamLeakParticles2 = VariantUtils.ConvertTo<GpuParticles2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._steamLeakParticles3)
		{
			_steamLeakParticles3 = VariantUtils.ConvertTo<GpuParticles2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._mistParticles)
		{
			_mistParticles = VariantUtils.ConvertTo<GpuParticles2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._mouthParticles)
		{
			_mouthParticles = VariantUtils.ConvertTo<GpuParticles2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._dropletParticles)
		{
			_dropletParticles = VariantUtils.ConvertTo<GpuParticles2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._isDead)
		{
			_isDead = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._parent)
		{
			_parent = VariantUtils.ConvertTo<Node2D>(ref value);
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
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName._steam1Particles)
		{
			value = VariantUtils.CreateFrom<GpuParticles2D>(ref _steam1Particles);
			return true;
		}
		if ((ref name) == PropertyName._steam2Particles)
		{
			value = VariantUtils.CreateFrom<GpuParticles2D>(ref _steam2Particles);
			return true;
		}
		if ((ref name) == PropertyName._steam3Particles)
		{
			value = VariantUtils.CreateFrom<GpuParticles2D>(ref _steam3Particles);
			return true;
		}
		if ((ref name) == PropertyName._steam4Particles)
		{
			value = VariantUtils.CreateFrom<GpuParticles2D>(ref _steam4Particles);
			return true;
		}
		if ((ref name) == PropertyName._steam5Particles)
		{
			value = VariantUtils.CreateFrom<GpuParticles2D>(ref _steam5Particles);
			return true;
		}
		if ((ref name) == PropertyName._steam6Particles)
		{
			value = VariantUtils.CreateFrom<GpuParticles2D>(ref _steam6Particles);
			return true;
		}
		if ((ref name) == PropertyName._steamLeakParticles1)
		{
			value = VariantUtils.CreateFrom<GpuParticles2D>(ref _steamLeakParticles1);
			return true;
		}
		if ((ref name) == PropertyName._steamLeakParticles2)
		{
			value = VariantUtils.CreateFrom<GpuParticles2D>(ref _steamLeakParticles2);
			return true;
		}
		if ((ref name) == PropertyName._steamLeakParticles3)
		{
			value = VariantUtils.CreateFrom<GpuParticles2D>(ref _steamLeakParticles3);
			return true;
		}
		if ((ref name) == PropertyName._mistParticles)
		{
			value = VariantUtils.CreateFrom<GpuParticles2D>(ref _mistParticles);
			return true;
		}
		if ((ref name) == PropertyName._mouthParticles)
		{
			value = VariantUtils.CreateFrom<GpuParticles2D>(ref _mouthParticles);
			return true;
		}
		if ((ref name) == PropertyName._dropletParticles)
		{
			value = VariantUtils.CreateFrom<GpuParticles2D>(ref _dropletParticles);
			return true;
		}
		if ((ref name) == PropertyName._isDead)
		{
			value = VariantUtils.CreateFrom<bool>(ref _isDead);
			return true;
		}
		if ((ref name) == PropertyName._parent)
		{
			value = VariantUtils.CreateFrom<Node2D>(ref _parent);
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
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._steam1Particles, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._steam2Particles, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._steam3Particles, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._steam4Particles, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._steam5Particles, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._steam6Particles, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._steamLeakParticles1, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._steamLeakParticles2, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._steamLeakParticles3, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._mistParticles, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._mouthParticles, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._dropletParticles, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._isDead, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._parent, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		info.AddProperty(PropertyName._steam1Particles, Variant.From<GpuParticles2D>(ref _steam1Particles));
		info.AddProperty(PropertyName._steam2Particles, Variant.From<GpuParticles2D>(ref _steam2Particles));
		info.AddProperty(PropertyName._steam3Particles, Variant.From<GpuParticles2D>(ref _steam3Particles));
		info.AddProperty(PropertyName._steam4Particles, Variant.From<GpuParticles2D>(ref _steam4Particles));
		info.AddProperty(PropertyName._steam5Particles, Variant.From<GpuParticles2D>(ref _steam5Particles));
		info.AddProperty(PropertyName._steam6Particles, Variant.From<GpuParticles2D>(ref _steam6Particles));
		info.AddProperty(PropertyName._steamLeakParticles1, Variant.From<GpuParticles2D>(ref _steamLeakParticles1));
		info.AddProperty(PropertyName._steamLeakParticles2, Variant.From<GpuParticles2D>(ref _steamLeakParticles2));
		info.AddProperty(PropertyName._steamLeakParticles3, Variant.From<GpuParticles2D>(ref _steamLeakParticles3));
		info.AddProperty(PropertyName._mistParticles, Variant.From<GpuParticles2D>(ref _mistParticles));
		info.AddProperty(PropertyName._mouthParticles, Variant.From<GpuParticles2D>(ref _mouthParticles));
		info.AddProperty(PropertyName._dropletParticles, Variant.From<GpuParticles2D>(ref _dropletParticles));
		info.AddProperty(PropertyName._isDead, Variant.From<bool>(ref _isDead));
		info.AddProperty(PropertyName._parent, Variant.From<Node2D>(ref _parent));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._steam1Particles, ref val))
		{
			_steam1Particles = ((Variant)(ref val)).As<GpuParticles2D>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._steam2Particles, ref val2))
		{
			_steam2Particles = ((Variant)(ref val2)).As<GpuParticles2D>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._steam3Particles, ref val3))
		{
			_steam3Particles = ((Variant)(ref val3)).As<GpuParticles2D>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._steam4Particles, ref val4))
		{
			_steam4Particles = ((Variant)(ref val4)).As<GpuParticles2D>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._steam5Particles, ref val5))
		{
			_steam5Particles = ((Variant)(ref val5)).As<GpuParticles2D>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._steam6Particles, ref val6))
		{
			_steam6Particles = ((Variant)(ref val6)).As<GpuParticles2D>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName._steamLeakParticles1, ref val7))
		{
			_steamLeakParticles1 = ((Variant)(ref val7)).As<GpuParticles2D>();
		}
		Variant val8 = default(Variant);
		if (info.TryGetProperty(PropertyName._steamLeakParticles2, ref val8))
		{
			_steamLeakParticles2 = ((Variant)(ref val8)).As<GpuParticles2D>();
		}
		Variant val9 = default(Variant);
		if (info.TryGetProperty(PropertyName._steamLeakParticles3, ref val9))
		{
			_steamLeakParticles3 = ((Variant)(ref val9)).As<GpuParticles2D>();
		}
		Variant val10 = default(Variant);
		if (info.TryGetProperty(PropertyName._mistParticles, ref val10))
		{
			_mistParticles = ((Variant)(ref val10)).As<GpuParticles2D>();
		}
		Variant val11 = default(Variant);
		if (info.TryGetProperty(PropertyName._mouthParticles, ref val11))
		{
			_mouthParticles = ((Variant)(ref val11)).As<GpuParticles2D>();
		}
		Variant val12 = default(Variant);
		if (info.TryGetProperty(PropertyName._dropletParticles, ref val12))
		{
			_dropletParticles = ((Variant)(ref val12)).As<GpuParticles2D>();
		}
		Variant val13 = default(Variant);
		if (info.TryGetProperty(PropertyName._isDead, ref val13))
		{
			_isDead = ((Variant)(ref val13)).As<bool>();
		}
		Variant val14 = default(Variant);
		if (info.TryGetProperty(PropertyName._parent, ref val14))
		{
			_parent = ((Variant)(ref val14)).As<Node2D>();
		}
	}
}
