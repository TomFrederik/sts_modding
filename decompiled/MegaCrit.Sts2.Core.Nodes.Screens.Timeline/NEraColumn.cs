using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Timeline;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.Nodes.Screens.Timeline;

[ScriptPath("res://src/Core/Nodes/Screens/Timeline/NEraColumn.cs")]
public class NEraColumn : Control
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName SpawnIcon = StringName.op_Implicit("SpawnIcon");

		public static readonly StringName SetPredictedPosition = StringName.op_Implicit("SetPredictedPosition");

		public static readonly StringName RectChange = StringName.op_Implicit("RectChange");

		public static readonly StringName _ExitTree = StringName.op_Implicit("_ExitTree");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName _icon = StringName.op_Implicit("_icon");

		public static readonly StringName _name = StringName.op_Implicit("_name");

		public static readonly StringName _year = StringName.op_Implicit("_year");

		public static readonly StringName _iconTween = StringName.op_Implicit("_iconTween");

		public static readonly StringName _labelTween = StringName.op_Implicit("_labelTween");

		public static readonly StringName _labelSpawned = StringName.op_Implicit("_labelSpawned");

		public static readonly StringName era = StringName.op_Implicit("era");

		public static readonly StringName _prevLocalPos = StringName.op_Implicit("_prevLocalPos");

		public static readonly StringName _prevGlobalPos = StringName.op_Implicit("_prevGlobalPos");

		public static readonly StringName _predictedPosition = StringName.op_Implicit("_predictedPosition");

		public static readonly StringName _targetPosition = StringName.op_Implicit("_targetPosition");

		public static readonly StringName _isAnimated = StringName.op_Implicit("_isAnimated");
	}

	public class SignalName : SignalName
	{
	}

	private const string _scenePath = "res://scenes/timeline_screen/era_column.tscn";

	public static readonly IEnumerable<string> assetPaths;

	private TextureRect _icon;

	private MegaLabel _name;

	private MegaLabel _year;

	private Tween _iconTween;

	private Tween _labelTween;

	private bool _labelSpawned;

	public EpochEra era;

	private Vector2 _prevLocalPos;

	private Vector2 _prevGlobalPos;

	private Vector2 _predictedPosition;

	private Vector2 _targetPosition;

	private bool _isAnimated;

	private EpochSlotData _data;

	public static NEraColumn Create(EpochSlotData data)
	{
		NEraColumn nEraColumn = PreloadManager.Cache.GetScene("res://scenes/timeline_screen/era_column.tscn").Instantiate<NEraColumn>((GenEditState)0);
		nEraColumn._data = data;
		return nEraColumn;
	}

	public override void _Ready()
	{
		_icon = ((Node)this).GetNode<TextureRect>(NodePath.op_Implicit("%Icon"));
		_name = ((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%Name"));
		_year = ((Node)this).GetNode<MegaLabel>(NodePath.op_Implicit("%Year"));
		era = _data.Era;
		((Node)this).SetName(_data.Era.ToString());
		Init(_data);
		((CanvasItem)this).ItemRectChanged += RectChange;
	}

	public void Init(EpochSlotData epochSlot)
	{
		(Texture2D, string) eraIcon = NTimelineScreen.GetEraIcon(epochSlot.Era);
		_icon.Texture = eraIcon.Item1;
		_name.SetTextAutoSize(new LocString("eras", eraIcon.Item2 + ".name").GetFormattedText());
		_year.SetTextAutoSize(new LocString("eras", eraIcon.Item2 + ".year").GetFormattedText());
		AddSlot(epochSlot);
	}

	public void AddSlot(EpochSlotData epochSlotData)
	{
		NEpochSlot nEpochSlot = NEpochSlot.Create(epochSlotData);
		((Node)(object)this).AddChildSafely((Node?)(object)nEpochSlot);
		((Node)nEpochSlot).Name = StringName.op_Implicit($"Slot{epochSlotData.EraPosition}");
		((Node)this).MoveChild((Node)(object)nEpochSlot, 0);
	}

	public void SpawnIcon()
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		_iconTween = ((Node)this).CreateTween().SetParallel(true);
		_iconTween.TweenProperty((GodotObject)(object)_icon, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(1f), 0.5);
		_iconTween.TweenProperty((GodotObject)(object)_icon, NodePath.op_Implicit("scale"), Variant.op_Implicit(Vector2.One), 0.5).From(Variant.op_Implicit(Vector2.One * 0.1f)).SetEase((EaseType)1)
			.SetTrans((TransitionType)10);
	}

	public async Task SpawnSlots(bool isAnimated)
	{
		foreach (Node child in ((Node)this).GetChildren(false))
		{
			if (child is NEpochSlot { HasSpawned: false } nEpochSlot)
			{
				if (isAnimated)
				{
					await nEpochSlot.SpawnSlot();
				}
				else
				{
					TaskHelper.RunSafely(nEpochSlot.SpawnSlot());
				}
			}
		}
	}

	public async Task SpawnNameAndYear()
	{
		if (!_labelSpawned)
		{
			_labelSpawned = true;
			_labelTween = ((Node)this).CreateTween().SetParallel(true);
			((CanvasItem)_name).SelfModulate = new Color(((CanvasItem)_name).SelfModulate.R, ((CanvasItem)_name).SelfModulate.G, ((CanvasItem)_name).SelfModulate.B, 0f);
			((CanvasItem)_year).Modulate = new Color(((CanvasItem)_year).Modulate.R, ((CanvasItem)_year).Modulate.G, ((CanvasItem)_year).Modulate.B, 0f);
			_labelTween.TweenProperty((GodotObject)(object)_name, NodePath.op_Implicit("self_modulate:a"), Variant.op_Implicit(1f), 1.0);
			_labelTween.TweenProperty((GodotObject)(object)_name, NodePath.op_Implicit("position:y"), Variant.op_Implicit(28f), 1.0).From(Variant.op_Implicit(-36f)).SetEase((EaseType)1)
				.SetTrans((TransitionType)7);
			_labelTween.TweenProperty((GodotObject)(object)_year, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(1f), 1.0).SetDelay(0.5);
			_labelTween.TweenProperty((GodotObject)(object)_year, NodePath.op_Implicit("position:y"), Variant.op_Implicit(20f), 1.0).SetDelay(0.5).From(Variant.op_Implicit(0f))
				.SetEase((EaseType)1)
				.SetTrans((TransitionType)7);
			await ((GodotObject)this).ToSignal((GodotObject)(object)_labelTween, SignalName.Finished);
			await Task.Delay(500);
		}
	}

	public async Task SaveBeforeAnimationPosition()
	{
		_isAnimated = true;
		_prevLocalPos = ((Control)this).Position;
		_prevGlobalPos = ((Control)this).GlobalPosition;
		await ((GodotObject)((Node)this).GetTree()).ToSignal((GodotObject)(object)((Node)this).GetTree(), SignalName.ProcessFrame);
		_isAnimated = false;
		_targetPosition = _predictedPosition;
		((Control)this).GlobalPosition = _prevGlobalPos;
		Tween val = ((Node)this).CreateTween().SetParallel(true);
		val.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("position"), Variant.op_Implicit(_targetPosition), 2.0).SetEase((EaseType)2).SetTrans((TransitionType)7);
	}

	public void SetPredictedPosition(Vector2 setPredictedPosition)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		if (_isAnimated)
		{
			_predictedPosition = setPredictedPosition;
		}
	}

	private void RectChange()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		if (_isAnimated)
		{
			((Control)this).GlobalPosition = _prevGlobalPos;
		}
	}

	public override void _ExitTree()
	{
		((CanvasItem)this).ItemRectChanged -= RectChange;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(5);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SpawnIcon, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetPredictedPosition, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)5, StringName.op_Implicit("setPredictedPosition"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.RectChange, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._ExitTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SpawnIcon && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			SpawnIcon();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetPredictedPosition && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			SetPredictedPosition(VariantUtils.ConvertTo<Vector2>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.RectChange && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			RectChange();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._ExitTree && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._ExitTree();
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
		if ((ref method) == MethodName.SpawnIcon)
		{
			return true;
		}
		if ((ref method) == MethodName.SetPredictedPosition)
		{
			return true;
		}
		if ((ref method) == MethodName.RectChange)
		{
			return true;
		}
		if ((ref method) == MethodName._ExitTree)
		{
			return true;
		}
		return ((Control)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName._icon)
		{
			_icon = VariantUtils.ConvertTo<TextureRect>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._name)
		{
			_name = VariantUtils.ConvertTo<MegaLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._year)
		{
			_year = VariantUtils.ConvertTo<MegaLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._iconTween)
		{
			_iconTween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._labelTween)
		{
			_labelTween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._labelSpawned)
		{
			_labelSpawned = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName.era)
		{
			era = VariantUtils.ConvertTo<EpochEra>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._prevLocalPos)
		{
			_prevLocalPos = VariantUtils.ConvertTo<Vector2>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._prevGlobalPos)
		{
			_prevGlobalPos = VariantUtils.ConvertTo<Vector2>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._predictedPosition)
		{
			_predictedPosition = VariantUtils.ConvertTo<Vector2>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._targetPosition)
		{
			_targetPosition = VariantUtils.ConvertTo<Vector2>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._isAnimated)
		{
			_isAnimated = VariantUtils.ConvertTo<bool>(ref value);
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
		if ((ref name) == PropertyName._icon)
		{
			value = VariantUtils.CreateFrom<TextureRect>(ref _icon);
			return true;
		}
		if ((ref name) == PropertyName._name)
		{
			value = VariantUtils.CreateFrom<MegaLabel>(ref _name);
			return true;
		}
		if ((ref name) == PropertyName._year)
		{
			value = VariantUtils.CreateFrom<MegaLabel>(ref _year);
			return true;
		}
		if ((ref name) == PropertyName._iconTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _iconTween);
			return true;
		}
		if ((ref name) == PropertyName._labelTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _labelTween);
			return true;
		}
		if ((ref name) == PropertyName._labelSpawned)
		{
			value = VariantUtils.CreateFrom<bool>(ref _labelSpawned);
			return true;
		}
		if ((ref name) == PropertyName.era)
		{
			value = VariantUtils.CreateFrom<EpochEra>(ref era);
			return true;
		}
		if ((ref name) == PropertyName._prevLocalPos)
		{
			value = VariantUtils.CreateFrom<Vector2>(ref _prevLocalPos);
			return true;
		}
		if ((ref name) == PropertyName._prevGlobalPos)
		{
			value = VariantUtils.CreateFrom<Vector2>(ref _prevGlobalPos);
			return true;
		}
		if ((ref name) == PropertyName._predictedPosition)
		{
			value = VariantUtils.CreateFrom<Vector2>(ref _predictedPosition);
			return true;
		}
		if ((ref name) == PropertyName._targetPosition)
		{
			value = VariantUtils.CreateFrom<Vector2>(ref _targetPosition);
			return true;
		}
		if ((ref name) == PropertyName._isAnimated)
		{
			value = VariantUtils.CreateFrom<bool>(ref _isAnimated);
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
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._icon, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._name, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._year, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._iconTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._labelTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._labelSpawned, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)2, PropertyName.era, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)5, PropertyName._prevLocalPos, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)5, PropertyName._prevGlobalPos, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)5, PropertyName._predictedPosition, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)5, PropertyName._targetPosition, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._isAnimated, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		((GodotObject)this).SaveGodotObjectData(info);
		info.AddProperty(PropertyName._icon, Variant.From<TextureRect>(ref _icon));
		info.AddProperty(PropertyName._name, Variant.From<MegaLabel>(ref _name));
		info.AddProperty(PropertyName._year, Variant.From<MegaLabel>(ref _year));
		info.AddProperty(PropertyName._iconTween, Variant.From<Tween>(ref _iconTween));
		info.AddProperty(PropertyName._labelTween, Variant.From<Tween>(ref _labelTween));
		info.AddProperty(PropertyName._labelSpawned, Variant.From<bool>(ref _labelSpawned));
		info.AddProperty(PropertyName.era, Variant.From<EpochEra>(ref era));
		info.AddProperty(PropertyName._prevLocalPos, Variant.From<Vector2>(ref _prevLocalPos));
		info.AddProperty(PropertyName._prevGlobalPos, Variant.From<Vector2>(ref _prevGlobalPos));
		info.AddProperty(PropertyName._predictedPosition, Variant.From<Vector2>(ref _predictedPosition));
		info.AddProperty(PropertyName._targetPosition, Variant.From<Vector2>(ref _targetPosition));
		info.AddProperty(PropertyName._isAnimated, Variant.From<bool>(ref _isAnimated));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._icon, ref val))
		{
			_icon = ((Variant)(ref val)).As<TextureRect>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._name, ref val2))
		{
			_name = ((Variant)(ref val2)).As<MegaLabel>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._year, ref val3))
		{
			_year = ((Variant)(ref val3)).As<MegaLabel>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._iconTween, ref val4))
		{
			_iconTween = ((Variant)(ref val4)).As<Tween>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._labelTween, ref val5))
		{
			_labelTween = ((Variant)(ref val5)).As<Tween>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._labelSpawned, ref val6))
		{
			_labelSpawned = ((Variant)(ref val6)).As<bool>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName.era, ref val7))
		{
			era = ((Variant)(ref val7)).As<EpochEra>();
		}
		Variant val8 = default(Variant);
		if (info.TryGetProperty(PropertyName._prevLocalPos, ref val8))
		{
			_prevLocalPos = ((Variant)(ref val8)).As<Vector2>();
		}
		Variant val9 = default(Variant);
		if (info.TryGetProperty(PropertyName._prevGlobalPos, ref val9))
		{
			_prevGlobalPos = ((Variant)(ref val9)).As<Vector2>();
		}
		Variant val10 = default(Variant);
		if (info.TryGetProperty(PropertyName._predictedPosition, ref val10))
		{
			_predictedPosition = ((Variant)(ref val10)).As<Vector2>();
		}
		Variant val11 = default(Variant);
		if (info.TryGetProperty(PropertyName._targetPosition, ref val11))
		{
			_targetPosition = ((Variant)(ref val11)).As<Vector2>();
		}
		Variant val12 = default(Variant);
		if (info.TryGetProperty(PropertyName._isAnimated, ref val12))
		{
			_isAnimated = ((Variant)(ref val12)).As<bool>();
		}
	}

	static NEraColumn()
	{
		List<string> list = new List<string>();
		list.Add("res://scenes/timeline_screen/era_column.tscn");
		list.AddRange(NEpochSlot.assetPaths);
		assetPaths = new _003C_003Ez__ReadOnlyList<string>(list);
	}
}
