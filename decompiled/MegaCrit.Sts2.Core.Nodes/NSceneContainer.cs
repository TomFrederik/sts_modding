using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Helpers;

namespace MegaCrit.Sts2.Core.Nodes;

[ScriptPath("res://src/Core/Nodes/NSceneContainer.cs")]
public class NSceneContainer : Control
{
	public class MethodName : MethodName
	{
		public static readonly StringName SetCurrentScene = StringName.op_Implicit("SetCurrentScene");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName CurrentScene = StringName.op_Implicit("CurrentScene");

		public static readonly StringName _currentScene = StringName.op_Implicit("_currentScene");
	}

	public class SignalName : SignalName
	{
	}

	private Control? _currentScene;

	public Control? CurrentScene
	{
		get
		{
			if (_currentScene == null)
			{
				return null;
			}
			if (!GodotObject.IsInstanceValid((GodotObject)(object)_currentScene))
			{
				return null;
			}
			if (((GodotObject)_currentScene).IsQueuedForDeletion())
			{
				return null;
			}
			return _currentScene;
		}
		private set
		{
			_currentScene = value;
		}
	}

	public void SetCurrentScene(Control node)
	{
		foreach (Node child in ((Node)this).GetChildren(false))
		{
			((Node)(object)this).RemoveChildSafely(child);
			child.QueueFreeSafely();
		}
		CurrentScene = node;
		if (((Node)node).GetParent() == null)
		{
			((Node)(object)this).AddChildSafely((Node?)(object)node);
		}
		else
		{
			((Node)node).Reparent((Node)(object)this, true);
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Expected O, but got Unknown
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(1);
		list.Add(new MethodInfo(MethodName.SetCurrentScene, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("node"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false)
		}, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.SetCurrentScene && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			SetCurrentScene(VariantUtils.ConvertTo<Control>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		return ((Control)this).InvokeGodotClassMethod(ref method, args, ref ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if ((ref method) == MethodName.SetCurrentScene)
		{
			return true;
		}
		return ((Control)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName.CurrentScene)
		{
			CurrentScene = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._currentScene)
		{
			_currentScene = VariantUtils.ConvertTo<Control>(ref value);
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
		if ((ref name) == PropertyName.CurrentScene)
		{
			Control currentScene = CurrentScene;
			value = VariantUtils.CreateFrom<Control>(ref currentScene);
			return true;
		}
		if ((ref name) == PropertyName._currentScene)
		{
			value = VariantUtils.CreateFrom<Control>(ref _currentScene);
			return true;
		}
		return ((GodotObject)this).GetGodotClassPropertyValue(ref name, ref value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._currentScene, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.CurrentScene, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		StringName currentScene = PropertyName.CurrentScene;
		Control currentScene2 = CurrentScene;
		info.AddProperty(currentScene, Variant.From<Control>(ref currentScene2));
		info.AddProperty(PropertyName._currentScene, Variant.From<Control>(ref _currentScene));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName.CurrentScene, ref val))
		{
			CurrentScene = ((Variant)(ref val)).As<Control>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._currentScene, ref val2))
		{
			_currentScene = ((Variant)(ref val2)).As<Control>();
		}
	}
}
