using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.TestSupport;

namespace MegaCrit.Sts2.Core.Nodes.Vfx.Cards;

[ScriptPath("res://src/Core/Nodes/Vfx/Cards/NHellraiserVfx.cs")]
public class NHellraiserVfx : Control
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName _duration = StringName.op_Implicit("_duration");

		public static readonly StringName _swordAmount = StringName.op_Implicit("_swordAmount");

		public static readonly StringName _spawnPosition = StringName.op_Implicit("_spawnPosition");
	}

	public class SignalName : SignalName
	{
	}

	private float _duration = 1f;

	private int _swordAmount = 10;

	private Vector2 _spawnPosition;

	private static readonly Vector2 _vfxOffset = new Vector2(-100f, -200f);

	private static readonly string _scenePath = SceneHelper.GetScenePath("vfx/cards/vfx_hellraiser/hellraiser_vfx");

	private const string _hellraiserSfxPath = "event:/sfx/characters/ironclad/ironclad_hellraiser";

	public static IEnumerable<string> AssetPaths => new _003C_003Ez__ReadOnlySingleElementList<string>(_scenePath);

	public static NHellraiserVfx? Create(Creature target)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		if (TestMode.IsOn)
		{
			return null;
		}
		NHellraiserVfx nHellraiserVfx = PreloadManager.Cache.GetScene(_scenePath).Instantiate<NHellraiserVfx>((GenEditState)0);
		nHellraiserVfx._spawnPosition = NCombatRoom.Instance.GetCreatureNode(target).GetBottomOfHitbox() + _vfxOffset;
		return nHellraiserVfx;
	}

	public override void _Ready()
	{
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		List<float> list = new List<float>();
		for (int i = 0; i < _swordAmount; i++)
		{
			list.Add(Rng.Chaotic.NextFloat(10f, 50f));
		}
		list.Sort();
		foreach (float item in list)
		{
			NHellraiserSwordVfx nHellraiserSwordVfx = NHellraiserSwordVfx.Create();
			((Control)nHellraiserSwordVfx).GlobalPosition = _spawnPosition;
			nHellraiserSwordVfx.posY = item;
			float num = MathHelper.Remap(item, 10f, 50f, 0.8f, 1f);
			nHellraiserSwordVfx.targetColor = new Color(num, num, num, 1f);
			((Node)(object)NCombatRoom.Instance.CombatVfxContainer).AddChildSafely((Node?)(object)nHellraiserSwordVfx);
		}
		list = new List<float>();
		for (int j = 0; j < _swordAmount; j++)
		{
			list.Add(Rng.Chaotic.NextFloat(-50f, -10f));
		}
		SfxCmd.Play("event:/sfx/characters/ironclad/ironclad_hellraiser");
		list.Sort();
		foreach (float item2 in list)
		{
			NHellraiserSwordVfx nHellraiserSwordVfx2 = NHellraiserSwordVfx.Create();
			((Control)nHellraiserSwordVfx2).GlobalPosition = _spawnPosition;
			nHellraiserSwordVfx2.posY = item2;
			float num2 = MathHelper.Remap(item2, -10f, -50f, 0.7f, 0.4f);
			nHellraiserSwordVfx2.targetColor = new Color(num2, num2, num2, 1f);
			NCombatRoom.Instance.BackCombatVfxContainer.AddChildSafely((Node?)(object)nHellraiserSwordVfx2);
		}
		((Node)(object)NCombatRoom.Instance.CombatVfxContainer).AddChildSafely((Node?)(object)NAdditiveOverlayVfx.Create());
		TaskHelper.RunSafely(SelfDestruct());
	}

	private async Task SelfDestruct()
	{
		await Task.Delay(2000);
		((Node)(object)this).QueueFreeSafely();
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(1);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
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
		return ((Control)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName._duration)
		{
			_duration = VariantUtils.ConvertTo<float>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._swordAmount)
		{
			_swordAmount = VariantUtils.ConvertTo<int>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._spawnPosition)
		{
			_spawnPosition = VariantUtils.ConvertTo<Vector2>(ref value);
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
		if ((ref name) == PropertyName._duration)
		{
			value = VariantUtils.CreateFrom<float>(ref _duration);
			return true;
		}
		if ((ref name) == PropertyName._swordAmount)
		{
			value = VariantUtils.CreateFrom<int>(ref _swordAmount);
			return true;
		}
		if ((ref name) == PropertyName._spawnPosition)
		{
			value = VariantUtils.CreateFrom<Vector2>(ref _spawnPosition);
			return true;
		}
		return ((GodotObject)this).GetGodotClassPropertyValue(ref name, ref value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)3, PropertyName._duration, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)2, PropertyName._swordAmount, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)5, PropertyName._spawnPosition, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		info.AddProperty(PropertyName._duration, Variant.From<float>(ref _duration));
		info.AddProperty(PropertyName._swordAmount, Variant.From<int>(ref _swordAmount));
		info.AddProperty(PropertyName._spawnPosition, Variant.From<Vector2>(ref _spawnPosition));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._duration, ref val))
		{
			_duration = ((Variant)(ref val)).As<float>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._swordAmount, ref val2))
		{
			_swordAmount = ((Variant)(ref val2)).As<int>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._spawnPosition, ref val3))
		{
			_spawnPosition = ((Variant)(ref val3)).As<Vector2>();
		}
	}
}
