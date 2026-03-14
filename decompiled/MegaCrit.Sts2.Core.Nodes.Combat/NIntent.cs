using System;
using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Intents;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.addons.mega_text;

namespace MegaCrit.Sts2.Core.Nodes.Combat;

[ScriptPath("res://src/Core/Nodes/Combat/NIntent.cs")]
public class NIntent : Control
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName _EnterTree = StringName.op_Implicit("_EnterTree");

		public static readonly StringName DebugToggleVisibility = StringName.op_Implicit("DebugToggleVisibility");

		public static readonly StringName _ExitTree = StringName.op_Implicit("_ExitTree");

		public static readonly StringName UpdateVisuals = StringName.op_Implicit("UpdateVisuals");

		public static readonly StringName _Process = StringName.op_Implicit("_Process");

		public static readonly StringName Create = StringName.op_Implicit("Create");

		public static readonly StringName PlayPerform = StringName.op_Implicit("PlayPerform");

		public static readonly StringName SetFrozen = StringName.op_Implicit("SetFrozen");

		public static readonly StringName OnHovered = StringName.op_Implicit("OnHovered");

		public static readonly StringName OnUnhovered = StringName.op_Implicit("OnUnhovered");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName _intentHolder = StringName.op_Implicit("_intentHolder");

		public static readonly StringName _intentSprite = StringName.op_Implicit("_intentSprite");

		public static readonly StringName _valueLabel = StringName.op_Implicit("_valueLabel");

		public static readonly StringName _intentParticle = StringName.op_Implicit("_intentParticle");

		public static readonly StringName _timeOffset = StringName.op_Implicit("_timeOffset");

		public static readonly StringName _timeAccumulator = StringName.op_Implicit("_timeAccumulator");

		public static readonly StringName _isFrozen = StringName.op_Implicit("_isFrozen");

		public static readonly StringName _animationName = StringName.op_Implicit("_animationName");
	}

	public class SignalName : SignalName
	{
	}

	private const string _scenePath = "res://scenes/combat/intent.tscn";

	private const float _bobSpeed = (float)Math.PI;

	private const float _bobDistance = 10f;

	private const float _bobOffset = 8f;

	private const int _animationFps = 15;

	private Control _intentHolder;

	private Sprite2D _intentSprite;

	private MegaRichTextLabel _valueLabel;

	private CpuParticles2D _intentParticle;

	private Creature _owner;

	private IEnumerable<Creature> _targets;

	private AbstractIntent _intent;

	private float _timeOffset;

	private float _timeAccumulator;

	private bool _isFrozen;

	private string? _animationName;

	private int? _animationFrame;

	public static IEnumerable<string> AssetPaths
	{
		get
		{
			List<string> list = new List<string>();
			list.Add("res://scenes/combat/intent.tscn");
			list.AddRange(IntentAnimData.AssetPaths);
			return new _003C_003Ez__ReadOnlyList<string>(list);
		}
	}

	public override void _Ready()
	{
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		_intentHolder = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%IntentHolder"));
		_intentSprite = ((Node)this).GetNode<Sprite2D>(NodePath.op_Implicit("%Intent"));
		_valueLabel = ((Node)this).GetNode<MegaRichTextLabel>(NodePath.op_Implicit("%Value"));
		_intentParticle = ((Node)this).GetNode<CpuParticles2D>(NodePath.op_Implicit("%IntentParticle"));
		((GodotObject)this).Connect(SignalName.MouseEntered, Callable.From((Action)OnHovered), 0u);
		((GodotObject)this).Connect(SignalName.MouseExited, Callable.From((Action)OnUnhovered), 0u);
		((CanvasItem)_intentHolder).Modulate = (NCombatUi.IsDebugHidingIntent ? Colors.Transparent : Colors.White);
	}

	public override void _EnterTree()
	{
		CombatManager.Instance.StateTracker.CombatStateChanged += OnCombatStateChanged;
		NCombatRoom.Instance.Ui.DebugToggleIntent += DebugToggleVisibility;
	}

	private void DebugToggleVisibility()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		((CanvasItem)_intentHolder).Modulate = (NCombatUi.IsDebugHidingIntent ? Colors.Transparent : Colors.White);
	}

	public override void _ExitTree()
	{
		CombatManager.Instance.StateTracker.CombatStateChanged -= OnCombatStateChanged;
		NCombatRoom.Instance.Ui.DebugToggleIntent -= DebugToggleVisibility;
	}

	public void UpdateIntent(AbstractIntent intent, IEnumerable<Creature> targets, Creature owner)
	{
		_owner = owner;
		_targets = targets;
		_intent = intent;
		UpdateVisuals();
	}

	private void OnCombatStateChanged(CombatState _)
	{
		if (!_isFrozen)
		{
			UpdateVisuals();
		}
	}

	private void UpdateVisuals()
	{
		string animation = _intent.GetAnimation(_targets, _owner);
		if (_animationName != animation)
		{
			_animationName = animation;
			_animationFrame = null;
			_timeAccumulator = 0f;
		}
		_intentParticle.Texture = _intent.GetTexture(_targets, _owner);
		MegaRichTextLabel valueLabel = _valueLabel;
		AbstractIntent intent = _intent;
		string text = ((intent is AttackIntent attackIntent) ? (attackIntent.GetIntentLabel(_targets, _owner).GetFormattedText() ?? "") : ((!(intent is StatusIntent)) ? string.Empty : (_intent.GetIntentLabel(_targets, _owner).GetFormattedText() ?? "")));
		valueLabel.Text = text;
	}

	public override void _Process(double delta)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		_intentHolder.Position = Vector2.Up * (Mathf.Sin((float)Time.GetTicksMsec() * 0.001f * (float)Math.PI + _timeOffset) * 10f + 8f);
		if (_animationName != null)
		{
			int num = (int)(_timeAccumulator * 15f) % IntentAnimData.GetAnimationFrameCount(_animationName);
			if (_animationFrame != num)
			{
				string animationFrame = IntentAnimData.GetAnimationFrame(_animationName, num);
				_animationFrame = num;
				_intentSprite.Texture = PreloadManager.Cache.GetTexture2D(animationFrame);
			}
			_timeAccumulator += (float)delta;
		}
	}

	public static NIntent Create(float startTime)
	{
		NIntent nIntent = PreloadManager.Cache.GetScene("res://scenes/combat/intent.tscn").Instantiate<NIntent>((GenEditState)0);
		nIntent._timeOffset = startTime;
		return nIntent;
	}

	public void PlayPerform()
	{
		_intentParticle.Emitting = true;
	}

	public void SetFrozen(bool isFrozen)
	{
		_isFrozen = isFrozen;
	}

	private void OnHovered()
	{
		if (_intent.HasIntentTip)
		{
			NCombatRoom.Instance?.GetCreatureNode(_owner)?.ShowHoverTips(new _003C_003Ez__ReadOnlySingleElementList<IHoverTip>(_intent.GetHoverTip(_targets, _owner)));
		}
	}

	private void OnUnhovered()
	{
		NCombatRoom.Instance?.GetCreatureNode(_owner)?.HideHoverTips();
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
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
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Expected O, but got Unknown
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(11);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._EnterTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.DebugToggleVisibility, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._ExitTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UpdateVisuals, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Process, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)3, StringName.op_Implicit("delta"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.Create, new PropertyInfo((Type)24, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false), (MethodFlags)33, new List<PropertyInfo>
		{
			new PropertyInfo((Type)3, StringName.op_Implicit("startTime"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.PlayPerform, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetFrozen, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)1, StringName.op_Implicit("isFrozen"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnHovered, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnUnhovered, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
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
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._EnterTree && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._EnterTree();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.DebugToggleVisibility && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			DebugToggleVisibility();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._ExitTree && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._ExitTree();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.UpdateVisuals && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			UpdateVisuals();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._Process && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			((Node)this)._Process(VariantUtils.ConvertTo<double>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.Create && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			NIntent nIntent = Create(VariantUtils.ConvertTo<float>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<NIntent>(ref nIntent);
			return true;
		}
		if ((ref method) == MethodName.PlayPerform && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			PlayPerform();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetFrozen && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			SetFrozen(VariantUtils.ConvertTo<bool>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnHovered && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnHovered();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnUnhovered && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnUnhovered();
			ret = default(godot_variant);
			return true;
		}
		return ((Control)this).InvokeGodotClassMethod(ref method, args, ref ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static bool InvokeGodotClassStaticMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName.Create && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			NIntent nIntent = Create(VariantUtils.ConvertTo<float>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<NIntent>(ref nIntent);
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
		if ((ref method) == MethodName._EnterTree)
		{
			return true;
		}
		if ((ref method) == MethodName.DebugToggleVisibility)
		{
			return true;
		}
		if ((ref method) == MethodName._ExitTree)
		{
			return true;
		}
		if ((ref method) == MethodName.UpdateVisuals)
		{
			return true;
		}
		if ((ref method) == MethodName._Process)
		{
			return true;
		}
		if ((ref method) == MethodName.Create)
		{
			return true;
		}
		if ((ref method) == MethodName.PlayPerform)
		{
			return true;
		}
		if ((ref method) == MethodName.SetFrozen)
		{
			return true;
		}
		if ((ref method) == MethodName.OnHovered)
		{
			return true;
		}
		if ((ref method) == MethodName.OnUnhovered)
		{
			return true;
		}
		return ((Control)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._intentHolder)
		{
			_intentHolder = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._intentSprite)
		{
			_intentSprite = VariantUtils.ConvertTo<Sprite2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._valueLabel)
		{
			_valueLabel = VariantUtils.ConvertTo<MegaRichTextLabel>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._intentParticle)
		{
			_intentParticle = VariantUtils.ConvertTo<CpuParticles2D>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._timeOffset)
		{
			_timeOffset = VariantUtils.ConvertTo<float>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._timeAccumulator)
		{
			_timeAccumulator = VariantUtils.ConvertTo<float>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._isFrozen)
		{
			_isFrozen = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._animationName)
		{
			_animationName = VariantUtils.ConvertTo<string>(ref value);
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
		if ((ref name) == PropertyName._intentHolder)
		{
			value = VariantUtils.CreateFrom<Control>(ref _intentHolder);
			return true;
		}
		if ((ref name) == PropertyName._intentSprite)
		{
			value = VariantUtils.CreateFrom<Sprite2D>(ref _intentSprite);
			return true;
		}
		if ((ref name) == PropertyName._valueLabel)
		{
			value = VariantUtils.CreateFrom<MegaRichTextLabel>(ref _valueLabel);
			return true;
		}
		if ((ref name) == PropertyName._intentParticle)
		{
			value = VariantUtils.CreateFrom<CpuParticles2D>(ref _intentParticle);
			return true;
		}
		if ((ref name) == PropertyName._timeOffset)
		{
			value = VariantUtils.CreateFrom<float>(ref _timeOffset);
			return true;
		}
		if ((ref name) == PropertyName._timeAccumulator)
		{
			value = VariantUtils.CreateFrom<float>(ref _timeAccumulator);
			return true;
		}
		if ((ref name) == PropertyName._isFrozen)
		{
			value = VariantUtils.CreateFrom<bool>(ref _isFrozen);
			return true;
		}
		if ((ref name) == PropertyName._animationName)
		{
			value = VariantUtils.CreateFrom<string>(ref _animationName);
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
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._intentHolder, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._intentSprite, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._valueLabel, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._intentParticle, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)3, PropertyName._timeOffset, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)3, PropertyName._timeAccumulator, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._isFrozen, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)4, PropertyName._animationName, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		((GodotObject)this).SaveGodotObjectData(info);
		info.AddProperty(PropertyName._intentHolder, Variant.From<Control>(ref _intentHolder));
		info.AddProperty(PropertyName._intentSprite, Variant.From<Sprite2D>(ref _intentSprite));
		info.AddProperty(PropertyName._valueLabel, Variant.From<MegaRichTextLabel>(ref _valueLabel));
		info.AddProperty(PropertyName._intentParticle, Variant.From<CpuParticles2D>(ref _intentParticle));
		info.AddProperty(PropertyName._timeOffset, Variant.From<float>(ref _timeOffset));
		info.AddProperty(PropertyName._timeAccumulator, Variant.From<float>(ref _timeAccumulator));
		info.AddProperty(PropertyName._isFrozen, Variant.From<bool>(ref _isFrozen));
		info.AddProperty(PropertyName._animationName, Variant.From<string>(ref _animationName));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._intentHolder, ref val))
		{
			_intentHolder = ((Variant)(ref val)).As<Control>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._intentSprite, ref val2))
		{
			_intentSprite = ((Variant)(ref val2)).As<Sprite2D>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._valueLabel, ref val3))
		{
			_valueLabel = ((Variant)(ref val3)).As<MegaRichTextLabel>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._intentParticle, ref val4))
		{
			_intentParticle = ((Variant)(ref val4)).As<CpuParticles2D>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._timeOffset, ref val5))
		{
			_timeOffset = ((Variant)(ref val5)).As<float>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._timeAccumulator, ref val6))
		{
			_timeAccumulator = ((Variant)(ref val6)).As<float>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName._isFrozen, ref val7))
		{
			_isFrozen = ((Variant)(ref val7)).As<bool>();
		}
		Variant val8 = default(Variant);
		if (info.TryGetProperty(PropertyName._animationName, ref val8))
		{
			_animationName = ((Variant)(ref val8)).As<string>();
		}
	}
}
