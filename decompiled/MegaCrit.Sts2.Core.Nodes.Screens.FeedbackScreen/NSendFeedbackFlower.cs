using System;
using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Random;

namespace MegaCrit.Sts2.Core.Nodes.Screens.FeedbackScreen;

[ScriptPath("res://src/Core/Nodes/Screens/FeedbackScreen/NSendFeedbackFlower.cs")]
public class NSendFeedbackFlower : Control
{
	public enum State
	{
		None,
		Nodding,
		Anticipation,
		NoddingFast
	}

	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName SetState = StringName.op_Implicit("SetState");

		public static readonly StringName SetRandomPosition = StringName.op_Implicit("SetRandomPosition");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName Cartoon = StringName.op_Implicit("Cartoon");

		public static readonly StringName MyState = StringName.op_Implicit("MyState");

		public static readonly StringName _tween = StringName.op_Implicit("_tween");

		public static readonly StringName _originalPosition = StringName.op_Implicit("_originalPosition");
	}

	public class SignalName : SignalName
	{
	}

	private const string _normalImage = "res://images/atlases/compressed.sprites/feedback/flower.tres";

	private const string _noddingImage = "res://images/atlases/compressed.sprites/feedback/flower_happy.tres";

	private const string _anticipationImage = "res://images/atlases/compressed.sprites/feedback/flower_anticipation.tres";

	private Tween? _tween;

	private Vector2 _originalPosition;

	public NSendFeedbackCartoon Cartoon { get; private set; }

	public State MyState { get; private set; }

	public override void _Ready()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_originalPosition = ((Control)this).Position;
		Cartoon = ((Node)this).GetNode<NSendFeedbackCartoon>(NodePath.op_Implicit("Flower"));
	}

	public void SetState(State state)
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		switch (state)
		{
		case State.Nodding:
		{
			Tween? tween3 = _tween;
			if (tween3 != null)
			{
				tween3.Kill();
			}
			_tween = ((Node)this).CreateTween();
			_tween.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("rotation"), Variant.op_Implicit(Mathf.DegToRad(8f)), 0.5);
			_tween.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("rotation"), Variant.op_Implicit(Mathf.DegToRad(-8f)), 0.5);
			_tween.SetLoops(0);
			((TextureRect)Cartoon).Texture = PreloadManager.Cache.GetTexture2D("res://images/atlases/compressed.sprites/feedback/flower_happy.tres");
			break;
		}
		case State.NoddingFast:
		{
			Tween? tween2 = _tween;
			if (tween2 != null)
			{
				tween2.Kill();
			}
			_tween = ((Node)this).CreateTween();
			_tween.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("rotation"), Variant.op_Implicit(Mathf.DegToRad(8f)), 0.2);
			_tween.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("rotation"), Variant.op_Implicit(Mathf.DegToRad(-8f)), 0.2);
			_tween.SetLoops(0);
			((TextureRect)Cartoon).Texture = PreloadManager.Cache.GetTexture2D("res://images/atlases/compressed.sprites/feedback/flower_happy.tres");
			break;
		}
		case State.Anticipation:
		{
			Tween? tween4 = _tween;
			if (tween4 != null)
			{
				tween4.Kill();
			}
			_tween = null;
			((TextureRect)Cartoon).Texture = PreloadManager.Cache.GetTexture2D("res://images/atlases/compressed.sprites/feedback/flower_anticipation.tres");
			_tween = ((Node)this).CreateTween();
			_tween.TweenInterval(0.05000000074505806);
			_tween.TweenCallback(Callable.From((Action)SetRandomPosition));
			_tween.SetLoops(0);
			break;
		}
		default:
		{
			((Control)this).Rotation = 0f;
			Tween? tween = _tween;
			if (tween != null)
			{
				tween.Kill();
			}
			_tween = null;
			((TextureRect)Cartoon).Texture = PreloadManager.Cache.GetTexture2D("res://images/atlases/compressed.sprites/feedback/flower.tres");
			break;
		}
		}
		MyState = state;
	}

	private void SetRandomPosition()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		((Control)this).Position = _originalPosition + new Vector2(Rng.Chaotic.NextFloat(-3f, 3f), Rng.Chaotic.NextFloat(-3f, 3f));
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
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(3);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetState, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("state"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetRandomPosition, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetState && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			SetState(VariantUtils.ConvertTo<State>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetRandomPosition && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			SetRandomPosition();
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
		if ((ref method) == MethodName.SetState)
		{
			return true;
		}
		if ((ref method) == MethodName.SetRandomPosition)
		{
			return true;
		}
		return ((Control)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName.Cartoon)
		{
			Cartoon = VariantUtils.ConvertTo<NSendFeedbackCartoon>(ref value);
			return true;
		}
		if ((ref name) == PropertyName.MyState)
		{
			MyState = VariantUtils.ConvertTo<State>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._tween)
		{
			_tween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._originalPosition)
		{
			_originalPosition = VariantUtils.ConvertTo<Vector2>(ref value);
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
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName.Cartoon)
		{
			NSendFeedbackCartoon cartoon = Cartoon;
			value = VariantUtils.CreateFrom<NSendFeedbackCartoon>(ref cartoon);
			return true;
		}
		if ((ref name) == PropertyName.MyState)
		{
			State myState = MyState;
			value = VariantUtils.CreateFrom<State>(ref myState);
			return true;
		}
		if ((ref name) == PropertyName._tween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _tween);
			return true;
		}
		if ((ref name) == PropertyName._originalPosition)
		{
			value = VariantUtils.CreateFrom<Vector2>(ref _originalPosition);
			return true;
		}
		return ((GodotObject)this).GetGodotClassPropertyValue(ref name, ref value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._tween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)5, PropertyName._originalPosition, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.Cartoon, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)2, PropertyName.MyState, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		StringName cartoon = PropertyName.Cartoon;
		NSendFeedbackCartoon cartoon2 = Cartoon;
		info.AddProperty(cartoon, Variant.From<NSendFeedbackCartoon>(ref cartoon2));
		StringName myState = PropertyName.MyState;
		State myState2 = MyState;
		info.AddProperty(myState, Variant.From<State>(ref myState2));
		info.AddProperty(PropertyName._tween, Variant.From<Tween>(ref _tween));
		info.AddProperty(PropertyName._originalPosition, Variant.From<Vector2>(ref _originalPosition));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName.Cartoon, ref val))
		{
			Cartoon = ((Variant)(ref val)).As<NSendFeedbackCartoon>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName.MyState, ref val2))
		{
			MyState = ((Variant)(ref val2)).As<State>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._tween, ref val3))
		{
			_tween = ((Variant)(ref val3)).As<Tween>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._originalPosition, ref val4))
		{
			_originalPosition = ((Variant)(ref val4)).As<Vector2>();
		}
	}
}
