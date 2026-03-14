using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.RestSite;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models.Characters;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Nodes.Vfx.Utilities;
using MegaCrit.Sts2.Core.Random;

namespace MegaCrit.Sts2.Core.Nodes.RestSite;

[ScriptPath("res://src/Core/Nodes/RestSite/NRestSiteCharacter.cs")]
public class NRestSiteCharacter : Node2D
{
	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName RandomizeFire = StringName.op_Implicit("RandomizeFire");

		public static readonly StringName OnFocus = StringName.op_Implicit("OnFocus");

		public static readonly StringName OnUnfocus = StringName.op_Implicit("OnUnfocus");

		public static readonly StringName Deselect = StringName.op_Implicit("Deselect");

		public static readonly StringName FlipX = StringName.op_Implicit("FlipX");

		public static readonly StringName HideFlameGlow = StringName.op_Implicit("HideFlameGlow");

		public static readonly StringName RefreshThoughtBubbleVfx = StringName.op_Implicit("RefreshThoughtBubbleVfx");

		public static readonly StringName Shake = StringName.op_Implicit("Shake");

		public static readonly StringName GetRestSiteOptionAnchor = StringName.op_Implicit("GetRestSiteOptionAnchor");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName Hitbox = StringName.op_Implicit("Hitbox");

		public static readonly StringName _controlRoot = StringName.op_Implicit("_controlRoot");

		public static readonly StringName _selectionReticle = StringName.op_Implicit("_selectionReticle");

		public static readonly StringName _leftThoughtAnchor = StringName.op_Implicit("_leftThoughtAnchor");

		public static readonly StringName _rightThoughtAnchor = StringName.op_Implicit("_rightThoughtAnchor");

		public static readonly StringName _characterIndex = StringName.op_Implicit("_characterIndex");

		public static readonly StringName _thoughtBubbleVfx = StringName.op_Implicit("_thoughtBubbleVfx");

		public static readonly StringName _selectedOptionConfirmation = StringName.op_Implicit("_selectedOptionConfirmation");
	}

	public class SignalName : SignalName
	{
	}

	private static readonly StringName _v = new StringName("v");

	private static readonly StringName _s = new StringName("s");

	private static readonly StringName _noise2Panning = new StringName("Noise2Panning");

	private static readonly StringName _noise1Panning = new StringName("Noise1Panning");

	private static readonly StringName _globalOffset = new StringName("GlobalOffset");

	private static readonly Vector2 _multiplayerConfirmationOffset = new Vector2(-25f, -123f);

	private static readonly Vector2 _multiplayerConfirmationFlipOffset = new Vector2(-155f, 0f);

	private static readonly string _multiplayerConfirmationScenePath = SceneHelper.GetScenePath("rest_site/rest_site_multiplayer_confirmation");

	private Control _controlRoot;

	private NSelectionReticle _selectionReticle;

	private Control _leftThoughtAnchor;

	private Control _rightThoughtAnchor;

	private int _characterIndex;

	private NThoughtBubbleVfx? _thoughtBubbleVfx;

	private CancellationTokenSource? _thoughtBubbleGoAwayCancellation;

	private Control? _selectedOptionConfirmation;

	private RestSiteOption? _hoveredRestSiteOption;

	private RestSiteOption? _selectingRestSiteOption;

	private RestSiteOption? _restSiteOptionInThoughtBubble;

	public Control Hitbox { get; private set; }

	public Player Player { get; private set; }

	public static NRestSiteCharacter Create(Player player, int characterIndex)
	{
		NRestSiteCharacter nRestSiteCharacter = PreloadManager.Cache.GetScene(player.Character.RestSiteAnimPath).Instantiate<NRestSiteCharacter>((GenEditState)0);
		nRestSiteCharacter.Player = player;
		nRestSiteCharacter._characterIndex = characterIndex;
		return nRestSiteCharacter;
	}

	public override void _Ready()
	{
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Expected O, but got Unknown
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Expected O, but got Unknown
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		_controlRoot = ((Node)this).GetNode<Control>(NodePath.op_Implicit("ControlRoot"));
		Hitbox = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%Hitbox"));
		_selectionReticle = ((Node)this).GetNode<NSelectionReticle>(NodePath.op_Implicit("%SelectionReticle"));
		_leftThoughtAnchor = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%ThoughtBubbleLeft"));
		_rightThoughtAnchor = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%ThoughtBubbleRight"));
		string animationName = Player.RunState.CurrentActIndex switch
		{
			0 => "overgrowth_loop", 
			1 => "hive_loop", 
			2 => "glory_loop", 
			_ => throw new InvalidOperationException("Unexpected act"), 
		};
		foreach (Node2D childSpineNode in GetChildSpineNodes())
		{
			MegaTrackEntry megaTrackEntry = new MegaSprite(Variant.op_Implicit((GodotObject)(object)childSpineNode)).GetAnimationState().SetAnimation(animationName);
			megaTrackEntry?.SetTrackTime(megaTrackEntry.GetAnimationEnd() * Rng.Chaotic.NextFloat());
		}
		if (Player.Character is Necrobinder)
		{
			Sprite2D node = ((Node)this).GetNode<Sprite2D>(NodePath.op_Implicit("%NecroFire"));
			Sprite2D node2 = ((Node)this).GetNode<Sprite2D>(NodePath.op_Implicit("%OstyFire"));
			RandomizeFire((ShaderMaterial)((CanvasItem)node).Material);
			RandomizeFire((ShaderMaterial)((CanvasItem)node2).Material);
			if (_characterIndex >= 2)
			{
				Node2D node3 = ((Node)this).GetNode<Node2D>(NodePath.op_Implicit("Osty"));
				Node2D node4 = ((Node)this).GetNode<Node2D>(NodePath.op_Implicit("OstyRightAnchor"));
				node3.Position = node4.Position;
				((Node)this).MoveChild((Node)(object)node3, 0);
			}
		}
		((GodotObject)Hitbox).Connect(SignalName.FocusEntered, Callable.From((Action)OnFocus), 0u);
		((GodotObject)Hitbox).Connect(SignalName.FocusExited, Callable.From((Action)OnUnfocus), 0u);
		((GodotObject)Hitbox).Connect(SignalName.MouseEntered, Callable.From((Action)OnFocus), 0u);
		((GodotObject)Hitbox).Connect(SignalName.MouseExited, Callable.From((Action)OnUnfocus), 0u);
	}

	private void RandomizeFire(ShaderMaterial mat)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		mat.SetShaderParameter(_globalOffset, Variant.op_Implicit(new Vector2(Rng.Chaotic.NextFloat(-50f, 50f), Rng.Chaotic.NextFloat(-50f, 50f))));
		StringName noise1Panning = _noise1Panning;
		Variant shaderParameter = mat.GetShaderParameter(_noise1Panning);
		mat.SetShaderParameter(noise1Panning, Variant.op_Implicit(((Variant)(ref shaderParameter)).AsVector2() + new Vector2(Rng.Chaotic.NextFloat(-0.1f, 0.1f), Rng.Chaotic.NextFloat(-0.1f, 0.1f))));
		StringName noise2Panning = _noise2Panning;
		shaderParameter = mat.GetShaderParameter(_noise2Panning);
		mat.SetShaderParameter(noise2Panning, Variant.op_Implicit(((Variant)(ref shaderParameter)).AsVector2() + new Vector2(Rng.Chaotic.NextFloat(-0.1f, 0.1f), Rng.Chaotic.NextFloat(-0.1f, 0.1f))));
	}

	private void OnFocus()
	{
		if (NTargetManager.Instance.IsInSelection && NTargetManager.Instance.AllowedToTargetNode((Node)(object)this))
		{
			NTargetManager.Instance.OnNodeHovered((Node)(object)this);
			_selectionReticle.OnSelect();
			NRun.Instance.GlobalUi.MultiplayerPlayerContainer.HighlightPlayer(Player);
		}
	}

	private void OnUnfocus()
	{
		if (NTargetManager.Instance.IsInSelection && NTargetManager.Instance.AllowedToTargetNode((Node)(object)this))
		{
			NTargetManager.Instance.OnNodeUnhovered((Node)(object)this);
		}
		Deselect();
	}

	public void Deselect()
	{
		if (_selectionReticle.IsSelected)
		{
			_selectionReticle.OnDeselect();
		}
		NRun.Instance.GlobalUi.MultiplayerPlayerContainer.UnhighlightPlayer(Player);
	}

	public void FlipX()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		Vector2 scale;
		foreach (Node2D childSpineNode in GetChildSpineNodes())
		{
			scale = childSpineNode.Scale;
			scale.X = 0f - childSpineNode.Scale.X;
			childSpineNode.Scale = scale;
			scale = childSpineNode.Position;
			scale.X = 0f - childSpineNode.Position.X;
			childSpineNode.Position = scale;
		}
		Control controlRoot = _controlRoot;
		scale = _controlRoot.Scale;
		scale.X = 0f - _controlRoot.Scale.X;
		controlRoot.Scale = scale;
	}

	public void HideFlameGlow()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		foreach (Node2D childSpineNode in GetChildSpineNodes())
		{
			MegaSprite megaSprite = new MegaSprite(Variant.op_Implicit((GodotObject)(object)childSpineNode));
			if (megaSprite.HasAnimation("_tracks/light_off"))
			{
				megaSprite.GetAnimationState().SetAnimation("_tracks/light_off", loop: true, 1);
			}
		}
	}

	public void ShowHoveredRestSiteOption(RestSiteOption? option)
	{
		_hoveredRestSiteOption = option;
		RefreshThoughtBubbleVfx();
	}

	public void SetSelectingRestSiteOption(RestSiteOption? option)
	{
		_selectingRestSiteOption = option;
		RefreshThoughtBubbleVfx();
	}

	private void RefreshThoughtBubbleVfx()
	{
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Expected O, but got Unknown
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		if (_selectedOptionConfirmation != null)
		{
			return;
		}
		RestSiteOption restSiteOption = _selectingRestSiteOption ?? _hoveredRestSiteOption;
		if (_restSiteOptionInThoughtBubble == restSiteOption)
		{
			return;
		}
		_restSiteOptionInThoughtBubble = restSiteOption;
		if (restSiteOption == null)
		{
			TaskHelper.RunSafely(RemoveThoughtBubbleAfterDelay());
			return;
		}
		_thoughtBubbleGoAwayCancellation?.Cancel();
		if (_thoughtBubbleVfx == null)
		{
			int characterIndex = _characterIndex;
			bool flag = ((characterIndex == 0 || characterIndex == 3) ? true : false);
			bool flag2 = flag;
			_thoughtBubbleVfx = NThoughtBubbleVfx.Create(restSiteOption.Icon, (!flag2) ? DialogueSide.Left : DialogueSide.Right, null);
			ShaderMaterial val = (ShaderMaterial)((CanvasItem)((Node)_thoughtBubbleVfx).GetNode<TextureRect>(NodePath.op_Implicit("%Image"))).Material;
			val.SetShaderParameter(_s, Variant.op_Implicit(0.145f));
			val.SetShaderParameter(_v, Variant.op_Implicit(0.85f));
			((Node)(object)this).AddChildSafely((Node?)(object)_thoughtBubbleVfx);
			((Control)_thoughtBubbleVfx).GlobalPosition = GetRestSiteOptionAnchor().GlobalPosition;
		}
		else
		{
			_thoughtBubbleVfx.SetTexture(restSiteOption.Icon);
		}
	}

	public void ShowSelectedRestSiteOption(RestSiteOption option)
	{
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		_thoughtBubbleVfx?.GoAway();
		_thoughtBubbleVfx = null;
		_selectedOptionConfirmation = PreloadManager.Cache.GetScene(_multiplayerConfirmationScenePath).Instantiate<Control>((GenEditState)0);
		((Node)_selectedOptionConfirmation).GetNode<TextureRect>(NodePath.op_Implicit("%Icon")).Texture = option.Icon;
		((Node)(object)this).AddChildSafely((Node?)(object)_selectedOptionConfirmation);
		int characterIndex = _characterIndex;
		bool flag = ((characterIndex == 0 || characterIndex == 3) ? true : false);
		bool flag2 = flag;
		_selectedOptionConfirmation.GlobalPosition = GetRestSiteOptionAnchor().GlobalPosition;
		Control? selectedOptionConfirmation = _selectedOptionConfirmation;
		selectedOptionConfirmation.Position += _multiplayerConfirmationOffset + (flag2 ? _multiplayerConfirmationFlipOffset : Vector2.Zero);
	}

	public void Shake()
	{
		TaskHelper.RunSafely(DoShake());
	}

	private async Task DoShake()
	{
		ScreenPunchInstance shake = new ScreenPunchInstance(15f, 0.4, 0f);
		Vector2 originalPosition = ((Node2D)this).Position;
		while (!shake.IsDone)
		{
			await ((GodotObject)this).ToSignal((GodotObject)(object)((Node)this).GetTree(), SignalName.ProcessFrame);
			Vector2 val = shake.Update(((Node)this).GetProcessDeltaTime());
			((Node2D)this).Position = originalPosition + val;
		}
		((Node2D)this).Position = originalPosition;
	}

	private Control GetRestSiteOptionAnchor()
	{
		if (_characterIndex < 2)
		{
			return _leftThoughtAnchor;
		}
		return _rightThoughtAnchor;
	}

	private async Task RemoveThoughtBubbleAfterDelay()
	{
		_thoughtBubbleGoAwayCancellation = new CancellationTokenSource();
		await Cmd.Wait(0.5f, _thoughtBubbleGoAwayCancellation.Token);
		if (!_thoughtBubbleGoAwayCancellation.IsCancellationRequested)
		{
			_thoughtBubbleVfx?.GoAway();
			_thoughtBubbleVfx = null;
		}
	}

	private IEnumerable<Node2D> GetChildSpineNodes()
	{
		foreach (Node2D item in ((IEnumerable)((Node)this).GetChildren(false)).OfType<Node2D>())
		{
			if (!(((GodotObject)item).GetClass() != "SpineSprite"))
			{
				yield return item;
			}
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
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Expected O, but got Unknown
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(10);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.RandomizeFire, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("mat"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("ShaderMaterial"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnFocus, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnUnfocus, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.Deselect, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.FlipX, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.HideFlameGlow, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.RefreshThoughtBubbleVfx, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.Shake, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.GetRestSiteOptionAnchor, new PropertyInfo((Type)24, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Control"), false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.RandomizeFire && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			RandomizeFire(VariantUtils.ConvertTo<ShaderMaterial>(ref ((NativeVariantPtrArgs)(ref args))[0]));
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
		if ((ref method) == MethodName.Deselect && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			Deselect();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.FlipX && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			FlipX();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.HideFlameGlow && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			HideFlameGlow();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.RefreshThoughtBubbleVfx && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			RefreshThoughtBubbleVfx();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.Shake && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			Shake();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.GetRestSiteOptionAnchor && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			Control restSiteOptionAnchor = GetRestSiteOptionAnchor();
			ret = VariantUtils.CreateFrom<Control>(ref restSiteOptionAnchor);
			return true;
		}
		return ((Node2D)this).InvokeGodotClassMethod(ref method, args, ref ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if ((ref method) == MethodName._Ready)
		{
			return true;
		}
		if ((ref method) == MethodName.RandomizeFire)
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
		if ((ref method) == MethodName.Deselect)
		{
			return true;
		}
		if ((ref method) == MethodName.FlipX)
		{
			return true;
		}
		if ((ref method) == MethodName.HideFlameGlow)
		{
			return true;
		}
		if ((ref method) == MethodName.RefreshThoughtBubbleVfx)
		{
			return true;
		}
		if ((ref method) == MethodName.Shake)
		{
			return true;
		}
		if ((ref method) == MethodName.GetRestSiteOptionAnchor)
		{
			return true;
		}
		return ((Node2D)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName.Hitbox)
		{
			Hitbox = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._controlRoot)
		{
			_controlRoot = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._selectionReticle)
		{
			_selectionReticle = VariantUtils.ConvertTo<NSelectionReticle>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._leftThoughtAnchor)
		{
			_leftThoughtAnchor = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._rightThoughtAnchor)
		{
			_rightThoughtAnchor = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._characterIndex)
		{
			_characterIndex = VariantUtils.ConvertTo<int>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._thoughtBubbleVfx)
		{
			_thoughtBubbleVfx = VariantUtils.ConvertTo<NThoughtBubbleVfx>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._selectedOptionConfirmation)
		{
			_selectedOptionConfirmation = VariantUtils.ConvertTo<Control>(ref value);
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
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName.Hitbox)
		{
			Control hitbox = Hitbox;
			value = VariantUtils.CreateFrom<Control>(ref hitbox);
			return true;
		}
		if ((ref name) == PropertyName._controlRoot)
		{
			value = VariantUtils.CreateFrom<Control>(ref _controlRoot);
			return true;
		}
		if ((ref name) == PropertyName._selectionReticle)
		{
			value = VariantUtils.CreateFrom<NSelectionReticle>(ref _selectionReticle);
			return true;
		}
		if ((ref name) == PropertyName._leftThoughtAnchor)
		{
			value = VariantUtils.CreateFrom<Control>(ref _leftThoughtAnchor);
			return true;
		}
		if ((ref name) == PropertyName._rightThoughtAnchor)
		{
			value = VariantUtils.CreateFrom<Control>(ref _rightThoughtAnchor);
			return true;
		}
		if ((ref name) == PropertyName._characterIndex)
		{
			value = VariantUtils.CreateFrom<int>(ref _characterIndex);
			return true;
		}
		if ((ref name) == PropertyName._thoughtBubbleVfx)
		{
			value = VariantUtils.CreateFrom<NThoughtBubbleVfx>(ref _thoughtBubbleVfx);
			return true;
		}
		if ((ref name) == PropertyName._selectedOptionConfirmation)
		{
			value = VariantUtils.CreateFrom<Control>(ref _selectedOptionConfirmation);
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
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._controlRoot, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName.Hitbox, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._selectionReticle, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._leftThoughtAnchor, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._rightThoughtAnchor, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)2, PropertyName._characterIndex, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._thoughtBubbleVfx, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._selectedOptionConfirmation, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		StringName hitbox = PropertyName.Hitbox;
		Control hitbox2 = Hitbox;
		info.AddProperty(hitbox, Variant.From<Control>(ref hitbox2));
		info.AddProperty(PropertyName._controlRoot, Variant.From<Control>(ref _controlRoot));
		info.AddProperty(PropertyName._selectionReticle, Variant.From<NSelectionReticle>(ref _selectionReticle));
		info.AddProperty(PropertyName._leftThoughtAnchor, Variant.From<Control>(ref _leftThoughtAnchor));
		info.AddProperty(PropertyName._rightThoughtAnchor, Variant.From<Control>(ref _rightThoughtAnchor));
		info.AddProperty(PropertyName._characterIndex, Variant.From<int>(ref _characterIndex));
		info.AddProperty(PropertyName._thoughtBubbleVfx, Variant.From<NThoughtBubbleVfx>(ref _thoughtBubbleVfx));
		info.AddProperty(PropertyName._selectedOptionConfirmation, Variant.From<Control>(ref _selectedOptionConfirmation));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName.Hitbox, ref val))
		{
			Hitbox = ((Variant)(ref val)).As<Control>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._controlRoot, ref val2))
		{
			_controlRoot = ((Variant)(ref val2)).As<Control>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._selectionReticle, ref val3))
		{
			_selectionReticle = ((Variant)(ref val3)).As<NSelectionReticle>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._leftThoughtAnchor, ref val4))
		{
			_leftThoughtAnchor = ((Variant)(ref val4)).As<Control>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._rightThoughtAnchor, ref val5))
		{
			_rightThoughtAnchor = ((Variant)(ref val5)).As<Control>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._characterIndex, ref val6))
		{
			_characterIndex = ((Variant)(ref val6)).As<int>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName._thoughtBubbleVfx, ref val7))
		{
			_thoughtBubbleVfx = ((Variant)(ref val7)).As<NThoughtBubbleVfx>();
		}
		Variant val8 = default(Variant);
		if (info.TryGetProperty(PropertyName._selectedOptionConfirmation, ref val8))
		{
			_selectedOptionConfirmation = ((Variant)(ref val8)).As<Control>();
		}
	}
}
