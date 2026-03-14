using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Nodes.Vfx.Ui;

namespace MegaCrit.Sts2.Core.Debug;

[ScriptPath("res://src/Core/Debug/NCombatVfxSpawner.cs")]
public class NCombatVfxSpawner : Control
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName _Process = StringName.op_Implicit("_Process");

		public static readonly StringName _Input = StringName.op_Implicit("_Input");

		public static readonly StringName GetRandomColor = StringName.op_Implicit("GetRandomColor");

		public static readonly StringName TestFunctionA = StringName.op_Implicit("TestFunctionA");

		public static readonly StringName TestFunctionB = StringName.op_Implicit("TestFunctionB");

		public static readonly StringName TestFunctionC = StringName.op_Implicit("TestFunctionC");

		public static readonly StringName SpawnVfx = StringName.op_Implicit("SpawnVfx");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName _backCombatVfxContainer = StringName.op_Implicit("_backCombatVfxContainer");

		public static readonly StringName _combatVfxContainer = StringName.op_Implicit("_combatVfxContainer");

		public static readonly StringName _env = StringName.op_Implicit("_env");

		public static readonly StringName _playerPosition = StringName.op_Implicit("_playerPosition");

		public static readonly StringName _playerGroundPosition = StringName.op_Implicit("_playerGroundPosition");

		public static readonly StringName _enemyPosition = StringName.op_Implicit("_enemyPosition");

		public static readonly StringName _enemyGroundPosition = StringName.op_Implicit("_enemyGroundPosition");

		public static readonly StringName _defectEyePosition = StringName.op_Implicit("_defectEyePosition");

		public static readonly StringName _lowHpBorderVfx = StringName.op_Implicit("_lowHpBorderVfx");

		public static readonly StringName _gaseousScreenVfx = StringName.op_Implicit("_gaseousScreenVfx");

		public static readonly StringName _shiftPressed = StringName.op_Implicit("_shiftPressed");
	}

	public class SignalName : SignalName
	{
	}

	[Export(/*Could not decode attribute arguments.*/)]
	private Node2D _backCombatVfxContainer;

	[Export(/*Could not decode attribute arguments.*/)]
	private Control _combatVfxContainer;

	private WorldEnvironment _env;

	[Export(/*Could not decode attribute arguments.*/)]
	private Node2D _playerPosition;

	[Export(/*Could not decode attribute arguments.*/)]
	private Node2D _playerGroundPosition;

	[Export(/*Could not decode attribute arguments.*/)]
	private Node2D _enemyPosition;

	[Export(/*Could not decode attribute arguments.*/)]
	private Node2D _enemyGroundPosition;

	[Export(/*Could not decode attribute arguments.*/)]
	private Node2D _defectEyePosition;

	[Export(/*Could not decode attribute arguments.*/)]
	private NLowHpBorderVfx _lowHpBorderVfx;

	[Export(/*Could not decode attribute arguments.*/)]
	private NGaseousScreenVfx _gaseousScreenVfx;

	private decimal _damage = 10m;

	private bool _shiftPressed;

	public override void _Ready()
	{
	}

	public override void _Process(double delta)
	{
		((Node)this)._Process(delta);
		_shiftPressed = Input.IsKeyPressed((Key)4194325);
	}

	public override void _Input(InputEvent inputEvent)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Invalid comparison between Unknown and I8
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Invalid comparison between Unknown and I8
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Invalid comparison between Unknown and I8
		InputEventKey val = (InputEventKey)(object)((inputEvent is InputEventKey) ? inputEvent : null);
		if (val != null && (long)val.Keycode == 81 && val.Pressed)
		{
			TestFunctionA(_shiftPressed);
		}
		val = (InputEventKey)(object)((inputEvent is InputEventKey) ? inputEvent : null);
		if (val != null && (long)val.Keycode == 87 && val.Pressed)
		{
			TestFunctionB(_shiftPressed);
		}
		val = (InputEventKey)(object)((inputEvent is InputEventKey) ? inputEvent : null);
		if (val != null && (long)val.Keycode == 69 && val.Pressed)
		{
			TestFunctionC(_shiftPressed);
		}
	}

	private static Color GetRandomColor()
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		return new Color(Mathf.Lerp(0.35f, 1f, GD.Randf()), Mathf.Lerp(0.35f, 1f, GD.Randf()), Mathf.Lerp(0.35f, 1f, GD.Randf()), 1f);
	}

	private void TestFunctionA(bool shiftPressed)
	{
		_lowHpBorderVfx.Play();
	}

	private void TestFunctionB(bool shiftPressed)
	{
		_gaseousScreenVfx.Play();
	}

	private void TestFunctionC(bool shiftPressed)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		NWormyImpactVfx child = NWormyImpactVfx.Create(_playerGroundPosition.GlobalPosition, _playerPosition.GlobalPosition);
		((Node)(object)_combatVfxContainer).AddChildSafely((Node?)(object)child);
	}

	private void SpawnVfx()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		NGaseousImpactVfx child = NGaseousImpactVfx.Create(_enemyPosition.GlobalPosition, GetRandomColor());
		((Node)(object)_combatVfxContainer).AddChildSafely((Node?)(object)child);
	}

	private async Task SpawningVfx()
	{
		NItemThrowVfx child = NItemThrowVfx.Create(_playerPosition.GlobalPosition + Vector2.Up * 150f, _enemyPosition.GlobalPosition, null);
		((Node)(object)_combatVfxContainer).AddChildSafely((Node?)(object)child);
		await Cmd.Wait(0.55f);
		NSplashVfx child2 = NSplashVfx.Create(_enemyPosition.GlobalPosition, new Color(0.25f, 1f, 0.4f, 1f));
		((Node)(object)_combatVfxContainer).AddChildSafely((Node?)(object)child2);
	}

	private async Task Hyperbeaming()
	{
		NHyperbeamVfx child = NHyperbeamVfx.Create(_defectEyePosition.GlobalPosition, _enemyPosition.GlobalPosition);
		((Node)(object)_combatVfxContainer).AddChildSafely((Node?)(object)child);
		await Cmd.Wait(NHyperbeamVfx.hyperbeamAnticipationDuration);
		NHyperbeamImpactVfx child2 = NHyperbeamImpactVfx.Create(_defectEyePosition.GlobalPosition, _enemyPosition.GlobalPosition);
		((Node)(object)_combatVfxContainer).AddChildSafely((Node?)(object)child2);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Expected O, but got Unknown
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(8);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Process, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)3, StringName.op_Implicit("delta"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Input, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("inputEvent"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("InputEvent"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.GetRandomColor, new PropertyInfo((Type)20, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)33, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.TestFunctionA, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)1, StringName.op_Implicit("shiftPressed"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.TestFunctionB, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)1, StringName.op_Implicit("shiftPressed"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.TestFunctionC, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)1, StringName.op_Implicit("shiftPressed"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SpawnVfx, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._Process && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			((Node)this)._Process(VariantUtils.ConvertTo<double>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._Input && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			((Node)this)._Input(VariantUtils.ConvertTo<InputEvent>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.GetRandomColor && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			Color randomColor = GetRandomColor();
			ret = VariantUtils.CreateFrom<Color>(ref randomColor);
			return true;
		}
		if ((ref method) == MethodName.TestFunctionA && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			TestFunctionA(VariantUtils.ConvertTo<bool>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.TestFunctionB && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			TestFunctionB(VariantUtils.ConvertTo<bool>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.TestFunctionC && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			TestFunctionC(VariantUtils.ConvertTo<bool>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SpawnVfx && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			SpawnVfx();
			ret = default(godot_variant);
			return true;
		}
		return ((Control)this).InvokeGodotClassMethod(ref method, args, ref ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static bool InvokeGodotClassStaticMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.GetRandomColor && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			Color randomColor = GetRandomColor();
			ret = VariantUtils.CreateFrom<Color>(ref randomColor);
			return true;
		}
		ret = default(godot_variant);
		return false;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if ((ref method) == MethodName._Ready)
		{
			return true;
		}
		if ((ref method) == MethodName._Process)
		{
			return true;
		}
		if ((ref method) == MethodName._Input)
		{
			return true;
		}
		if ((ref method) == MethodName.GetRandomColor)
		{
			return true;
		}
		if ((ref method) == MethodName.TestFunctionA)
		{
			return true;
		}
		if ((ref method) == MethodName.TestFunctionB)
		{
			return true;
		}
		if ((ref method) == MethodName.TestFunctionC)
		{
			return true;
		}
		if ((ref method) == MethodName.SpawnVfx)
		{
			return true;
		}
		return ((Control)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._backCombatVfxContainer)
		{
			_backCombatVfxContainer = VariantUtils.ConvertTo<Node2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._combatVfxContainer)
		{
			_combatVfxContainer = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._env)
		{
			_env = VariantUtils.ConvertTo<WorldEnvironment>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._playerPosition)
		{
			_playerPosition = VariantUtils.ConvertTo<Node2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._playerGroundPosition)
		{
			_playerGroundPosition = VariantUtils.ConvertTo<Node2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._enemyPosition)
		{
			_enemyPosition = VariantUtils.ConvertTo<Node2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._enemyGroundPosition)
		{
			_enemyGroundPosition = VariantUtils.ConvertTo<Node2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._defectEyePosition)
		{
			_defectEyePosition = VariantUtils.ConvertTo<Node2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._lowHpBorderVfx)
		{
			_lowHpBorderVfx = VariantUtils.ConvertTo<NLowHpBorderVfx>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._gaseousScreenVfx)
		{
			_gaseousScreenVfx = VariantUtils.ConvertTo<NGaseousScreenVfx>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._shiftPressed)
		{
			_shiftPressed = VariantUtils.ConvertTo<bool>(ref value);
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
		if ((ref name) == PropertyName._backCombatVfxContainer)
		{
			value = VariantUtils.CreateFrom<Node2D>(ref _backCombatVfxContainer);
			return true;
		}
		if ((ref name) == PropertyName._combatVfxContainer)
		{
			value = VariantUtils.CreateFrom<Control>(ref _combatVfxContainer);
			return true;
		}
		if ((ref name) == PropertyName._env)
		{
			value = VariantUtils.CreateFrom<WorldEnvironment>(ref _env);
			return true;
		}
		if ((ref name) == PropertyName._playerPosition)
		{
			value = VariantUtils.CreateFrom<Node2D>(ref _playerPosition);
			return true;
		}
		if ((ref name) == PropertyName._playerGroundPosition)
		{
			value = VariantUtils.CreateFrom<Node2D>(ref _playerGroundPosition);
			return true;
		}
		if ((ref name) == PropertyName._enemyPosition)
		{
			value = VariantUtils.CreateFrom<Node2D>(ref _enemyPosition);
			return true;
		}
		if ((ref name) == PropertyName._enemyGroundPosition)
		{
			value = VariantUtils.CreateFrom<Node2D>(ref _enemyGroundPosition);
			return true;
		}
		if ((ref name) == PropertyName._defectEyePosition)
		{
			value = VariantUtils.CreateFrom<Node2D>(ref _defectEyePosition);
			return true;
		}
		if ((ref name) == PropertyName._lowHpBorderVfx)
		{
			value = VariantUtils.CreateFrom<NLowHpBorderVfx>(ref _lowHpBorderVfx);
			return true;
		}
		if ((ref name) == PropertyName._gaseousScreenVfx)
		{
			value = VariantUtils.CreateFrom<NGaseousScreenVfx>(ref _gaseousScreenVfx);
			return true;
		}
		if ((ref name) == PropertyName._shiftPressed)
		{
			value = VariantUtils.CreateFrom<bool>(ref _shiftPressed);
			return true;
		}
		return ((GodotObject)this).GetGodotClassPropertyValue(ref name, ref value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._backCombatVfxContainer, (PropertyHint)34, "Node2D", (PropertyUsageFlags)4102, true));
		list.Add(new PropertyInfo((Type)24, PropertyName._combatVfxContainer, (PropertyHint)34, "Control", (PropertyUsageFlags)4102, true));
		list.Add(new PropertyInfo((Type)24, PropertyName._env, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._playerPosition, (PropertyHint)34, "Node2D", (PropertyUsageFlags)4102, true));
		list.Add(new PropertyInfo((Type)24, PropertyName._playerGroundPosition, (PropertyHint)34, "Node2D", (PropertyUsageFlags)4102, true));
		list.Add(new PropertyInfo((Type)24, PropertyName._enemyPosition, (PropertyHint)34, "Node2D", (PropertyUsageFlags)4102, true));
		list.Add(new PropertyInfo((Type)24, PropertyName._enemyGroundPosition, (PropertyHint)34, "Node2D", (PropertyUsageFlags)4102, true));
		list.Add(new PropertyInfo((Type)24, PropertyName._defectEyePosition, (PropertyHint)34, "Node2D", (PropertyUsageFlags)4102, true));
		list.Add(new PropertyInfo((Type)24, PropertyName._lowHpBorderVfx, (PropertyHint)34, "ColorRect", (PropertyUsageFlags)4102, true));
		list.Add(new PropertyInfo((Type)24, PropertyName._gaseousScreenVfx, (PropertyHint)34, "AspectRatioContainer", (PropertyUsageFlags)4102, true));
		list.Add(new PropertyInfo((Type)1, PropertyName._shiftPressed, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		((GodotObject)this).SaveGodotObjectData(info);
		info.AddProperty(PropertyName._backCombatVfxContainer, Variant.From<Node2D>(ref _backCombatVfxContainer));
		info.AddProperty(PropertyName._combatVfxContainer, Variant.From<Control>(ref _combatVfxContainer));
		info.AddProperty(PropertyName._env, Variant.From<WorldEnvironment>(ref _env));
		info.AddProperty(PropertyName._playerPosition, Variant.From<Node2D>(ref _playerPosition));
		info.AddProperty(PropertyName._playerGroundPosition, Variant.From<Node2D>(ref _playerGroundPosition));
		info.AddProperty(PropertyName._enemyPosition, Variant.From<Node2D>(ref _enemyPosition));
		info.AddProperty(PropertyName._enemyGroundPosition, Variant.From<Node2D>(ref _enemyGroundPosition));
		info.AddProperty(PropertyName._defectEyePosition, Variant.From<Node2D>(ref _defectEyePosition));
		info.AddProperty(PropertyName._lowHpBorderVfx, Variant.From<NLowHpBorderVfx>(ref _lowHpBorderVfx));
		info.AddProperty(PropertyName._gaseousScreenVfx, Variant.From<NGaseousScreenVfx>(ref _gaseousScreenVfx));
		info.AddProperty(PropertyName._shiftPressed, Variant.From<bool>(ref _shiftPressed));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._backCombatVfxContainer, ref val))
		{
			_backCombatVfxContainer = ((Variant)(ref val)).As<Node2D>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._combatVfxContainer, ref val2))
		{
			_combatVfxContainer = ((Variant)(ref val2)).As<Control>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._env, ref val3))
		{
			_env = ((Variant)(ref val3)).As<WorldEnvironment>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._playerPosition, ref val4))
		{
			_playerPosition = ((Variant)(ref val4)).As<Node2D>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._playerGroundPosition, ref val5))
		{
			_playerGroundPosition = ((Variant)(ref val5)).As<Node2D>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._enemyPosition, ref val6))
		{
			_enemyPosition = ((Variant)(ref val6)).As<Node2D>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName._enemyGroundPosition, ref val7))
		{
			_enemyGroundPosition = ((Variant)(ref val7)).As<Node2D>();
		}
		Variant val8 = default(Variant);
		if (info.TryGetProperty(PropertyName._defectEyePosition, ref val8))
		{
			_defectEyePosition = ((Variant)(ref val8)).As<Node2D>();
		}
		Variant val9 = default(Variant);
		if (info.TryGetProperty(PropertyName._lowHpBorderVfx, ref val9))
		{
			_lowHpBorderVfx = ((Variant)(ref val9)).As<NLowHpBorderVfx>();
		}
		Variant val10 = default(Variant);
		if (info.TryGetProperty(PropertyName._gaseousScreenVfx, ref val10))
		{
			_gaseousScreenVfx = ((Variant)(ref val10)).As<NGaseousScreenVfx>();
		}
		Variant val11 = default(Variant);
		if (info.TryGetProperty(PropertyName._shiftPressed, ref val11))
		{
			_shiftPressed = ((Variant)(ref val11)).As<bool>();
		}
	}
}
