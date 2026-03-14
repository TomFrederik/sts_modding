using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Audio.Debug;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Characters;
using MegaCrit.Sts2.Core.Multiplayer.Game;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.HoverTips;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.Unlocks;

namespace MegaCrit.Sts2.Core.Nodes.Screens.CharacterSelect;

[ScriptPath("res://src/Core/Nodes/Screens/CharacterSelect/NCharacterSelectButton.cs")]
public class NCharacterSelectButton : NButton
{
	private enum State
	{
		NotSelected,
		SelectedLocally,
		SelectedRemotely
	}

	public new class MethodName : NButton.MethodName
	{
		public new static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public new static readonly StringName OnFocus = StringName.op_Implicit("OnFocus");

		public new static readonly StringName OnPress = StringName.op_Implicit("OnPress");

		public new static readonly StringName OnUnfocus = StringName.op_Implicit("OnUnfocus");

		public static readonly StringName _Process = StringName.op_Implicit("_Process");

		public static readonly StringName LockForAnimation = StringName.op_Implicit("LockForAnimation");

		public static readonly StringName Reset = StringName.op_Implicit("Reset");

		public static readonly StringName OnRemotePlayerSelected = StringName.op_Implicit("OnRemotePlayerSelected");

		public static readonly StringName OnRemotePlayerDeselected = StringName.op_Implicit("OnRemotePlayerDeselected");

		public static readonly StringName Select = StringName.op_Implicit("Select");

		public static readonly StringName Deselect = StringName.op_Implicit("Deselect");

		public static readonly StringName RefreshState = StringName.op_Implicit("RefreshState");

		public static readonly StringName GetSaturationForCurrentState = StringName.op_Implicit("GetSaturationForCurrentState");

		public static readonly StringName GetValueForCurrentState = StringName.op_Implicit("GetValueForCurrentState");

		public static readonly StringName AnimateSaturationToCurrentState = StringName.op_Implicit("AnimateSaturationToCurrentState");

		public static readonly StringName RefreshOutline = StringName.op_Implicit("RefreshOutline");

		public static readonly StringName RefreshPlayerIcons = StringName.op_Implicit("RefreshPlayerIcons");

		public static readonly StringName DebugUnlock = StringName.op_Implicit("DebugUnlock");

		public static readonly StringName UnlockIfPossible = StringName.op_Implicit("UnlockIfPossible");

		public static readonly StringName UpdateShaderH = StringName.op_Implicit("UpdateShaderH");

		public static readonly StringName UpdateShaderS = StringName.op_Implicit("UpdateShaderS");

		public static readonly StringName UpdateShaderV = StringName.op_Implicit("UpdateShaderV");
	}

	public new class PropertyName : NButton.PropertyName
	{
		public static readonly StringName IsRandom = StringName.op_Implicit("IsRandom");

		public static readonly StringName IsLocked = StringName.op_Implicit("IsLocked");

		public static readonly StringName _icon = StringName.op_Implicit("_icon");

		public static readonly StringName _iconAdd = StringName.op_Implicit("_iconAdd");

		public static readonly StringName _lock = StringName.op_Implicit("_lock");

		public static readonly StringName _outlineLocal = StringName.op_Implicit("_outlineLocal");

		public static readonly StringName _outlineRemote = StringName.op_Implicit("_outlineRemote");

		public static readonly StringName _outlineMixed = StringName.op_Implicit("_outlineMixed");

		public static readonly StringName _shadow = StringName.op_Implicit("_shadow");

		public static readonly StringName _playerIconContainer = StringName.op_Implicit("_playerIconContainer");

		public static readonly StringName _hsv = StringName.op_Implicit("_hsv");

		public static readonly StringName _isLocked = StringName.op_Implicit("_isLocked");

		public static readonly StringName _currentOutline = StringName.op_Implicit("_currentOutline");

		public static readonly StringName _isSelected = StringName.op_Implicit("_isSelected");

		public static readonly StringName _state = StringName.op_Implicit("_state");

		public static readonly StringName _hoverTween = StringName.op_Implicit("_hoverTween");

		public static readonly StringName _hsvTween = StringName.op_Implicit("_hsvTween");
	}

	public new class SignalName : NButton.SignalName
	{
	}

	private static readonly StringName _v = new StringName("v");

	private static readonly StringName _s = new StringName("s");

	private static readonly StringName _h = new StringName("h");

	private static readonly string _playerIconScenePath = SceneHelper.GetScenePath("screens/char_select/char_select_player_icon");

	private static readonly string _unlockedIconPath = ImageHelper.GetImagePath("packed/character_select/char_select_lock3_unlocked.png");

	private TextureRect _icon;

	private TextureRect _iconAdd;

	private TextureRect _lock;

	private Control _outlineLocal;

	private Control _outlineRemote;

	private Control _outlineMixed;

	private Control _shadow;

	private Control _playerIconContainer;

	private CharacterModel _character;

	private ShaderMaterial _hsv;

	private bool _isLocked;

	private static readonly Vector2 _hoverTipOffset = new Vector2(-90f, -180f);

	private ICharacterSelectButtonDelegate? _delegate;

	private Control? _currentOutline;

	private bool _isSelected;

	private readonly HashSet<ulong> _remoteSelectedPlayers = new HashSet<ulong>();

	private State _state;

	private static readonly Vector2 _hoverScale = Vector2.One * 1.1f;

	private Tween? _hoverTween;

	private Tween? _hsvTween;

	private const float _unhoverDuration = 0.5f;

	private const float _glowSpeed = 1.6f;

	private const float _selectedSaturation = 1f;

	private const float _selectedValue = 1.1f;

	private const float _remotelySelectedSaturation = 0.8f;

	private const float _remotelySelectedValue = 0.4f;

	private const float _notSelectedSaturation = 0.2f;

	private const float _notSelectedValue = 0.4f;

	public static IEnumerable<string> AssetPaths => new global::_003C_003Ez__ReadOnlyArray<string>(new string[2] { _playerIconScenePath, _unlockedIconPath });

	public bool IsRandom { get; private set; }

	public IReadOnlyCollection<ulong> RemoteSelectedPlayers => _remoteSelectedPlayers;

	public CharacterModel Character => _character;

	public bool IsLocked => _isLocked;

	public override void _Ready()
	{
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Expected O, but got Unknown
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		ConnectSignals();
		_icon = ((Node)this).GetNode<TextureRect>(NodePath.op_Implicit("%Icon"));
		_iconAdd = ((Node)this).GetNode<TextureRect>(NodePath.op_Implicit("%IconAdd"));
		_lock = ((Node)this).GetNode<TextureRect>(NodePath.op_Implicit("%Lock"));
		_outlineLocal = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%OutlineLocal"));
		_outlineRemote = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%OutlineRemote"));
		_outlineMixed = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%OutlineMixed"));
		_shadow = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%Shadow"));
		_playerIconContainer = ((Node)this).GetNode<Control>(NodePath.op_Implicit("%PlayerIconContainer"));
		_hsv = (ShaderMaterial)((CanvasItem)_icon).Material;
		_hsv.SetShaderParameter(_s, Variant.op_Implicit(0.2f));
		_hsv.SetShaderParameter(_v, Variant.op_Implicit(0.4f));
		((GodotObject)this).Connect(SignalName.FocusEntered, Callable.From((Action)Select), 0u);
	}

	public void Init(CharacterModel character, ICharacterSelectButtonDelegate del)
	{
		_delegate = del;
		_character = character;
		UnlockState unlockState = SaveManager.Instance.GenerateUnlockStateFromProgress();
		if (character is RandomCharacter)
		{
			IsRandom = true;
			_isLocked = ModelDb.AllCharacters.Any((CharacterModel c) => !unlockState.Characters.Contains(c));
		}
		else
		{
			_isLocked = !unlockState.Characters.Contains(_character);
		}
		if (_isLocked)
		{
			_icon.Texture = (Texture2D)(object)character.CharacterSelectLockedIcon;
			((CanvasItem)_lock).Visible = true;
		}
		else
		{
			_icon.Texture = (Texture2D)(object)character.CharacterSelectIcon;
		}
	}

	protected override void OnFocus()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		if (!_isSelected)
		{
			Tween? hoverTween = _hoverTween;
			if (hoverTween != null)
			{
				hoverTween.Kill();
			}
			((Control)this).Scale = _hoverScale;
			_hsv.SetShaderParameter(_s, Variant.op_Implicit(1f));
			_hsv.SetShaderParameter(_v, Variant.op_Implicit(1.1f));
			if (_isLocked)
			{
				HoverTip hoverTip = new HoverTip(new LocString("main_menu_ui", "CHARACTER_SELECT.locked.title"), _character.GetUnlockText());
				((Control)NHoverTipSet.CreateAndShow((Control)(object)this, hoverTip)).GlobalPosition = ((Control)this).GlobalPosition + _hoverTipOffset;
			}
			SfxCmd.Play("event:/sfx/ui/clicks/ui_hover");
		}
	}

	protected override void OnPress()
	{
	}

	protected override void OnUnfocus()
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		Tween? hoverTween = _hoverTween;
		if (hoverTween != null)
		{
			hoverTween.Kill();
		}
		_hoverTween = ((Node)this).CreateTween().SetParallel(true);
		_hoverTween.TweenProperty((GodotObject)(object)this, NodePath.op_Implicit("scale"), Variant.op_Implicit(Vector2.One), 0.5).SetEase((EaseType)1).SetTrans((TransitionType)5);
		NHoverTipSet.Remove((Control)(object)this);
		AnimateSaturationToCurrentState(_hoverTween);
	}

	public override void _Process(double delta)
	{
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		if (_currentOutline != null)
		{
			if (_isSelected)
			{
				float a = Mathf.Lerp(0.35f, 1f, (Mathf.Cos((float)Time.GetTicksMsec() * 0.001f * 1.6f * (float)Math.PI) + 1f) * 0.5f);
				Control? currentOutline = _currentOutline;
				Color modulate = ((CanvasItem)_currentOutline).Modulate;
				modulate.A = a;
				((CanvasItem)currentOutline).Modulate = modulate;
			}
			else
			{
				Control? currentOutline2 = _currentOutline;
				Color modulate = ((CanvasItem)_currentOutline).Modulate;
				modulate.A = 0.5f;
				((CanvasItem)currentOutline2).Modulate = modulate;
			}
		}
	}

	public void LockForAnimation()
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		_icon.Texture = (Texture2D)(object)_character.CharacterSelectLockedIcon;
		((CanvasItem)_lock).Visible = true;
		((CanvasItem)this).ZIndex = 1;
		((CanvasItem)_lock).Modulate = Colors.White;
		Disable();
	}

	public async Task AnimateUnlock()
	{
		GpuParticles2D chargeParticles = ((Node)this).GetNode<GpuParticles2D>(NodePath.op_Implicit("%UnlockChargeParticles"));
		chargeParticles.Emitting = true;
		float num = 1f;
		Vector2 originalLockPosition = ((Control)_lock).Position;
		float timer = 0f;
		NDebugAudioManager.Instance.Play("character_unlock_charge.mp3");
		while (timer < 1f)
		{
			Vector2 right = Vector2.Right;
			Vector2 val = ((Vector2)(ref right)).Rotated(Rng.Chaotic.NextFloat((float)Math.PI * 2f)) * num;
			((Control)_lock).Position = originalLockPosition + val;
			await ((GodotObject)this).ToSignal((GodotObject)(object)((Node)this).GetTree(), SignalName.ProcessFrame);
			timer += (float)((Node)this).GetProcessDeltaTime();
			num = Mathf.Lerp(1f, 5f, Ease.QuadOut(timer));
		}
		NDebugAudioManager.Instance.Play("character_unlock.mp3");
		((Control)_lock).Position = originalLockPosition;
		_lock.Texture = PreloadManager.Cache.GetTexture2D(_unlockedIconPath);
		_icon.Texture = (Texture2D)(object)_character.CharacterSelectIcon;
		_iconAdd.Texture = _icon.Texture;
		((CanvasItem)_iconAdd).Visible = true;
		GpuParticles2D node = ((Node)this).GetNode<GpuParticles2D>(NodePath.op_Implicit("%UnlockParticles"));
		node.Emitting = true;
		chargeParticles.Emitting = false;
		Tween val2 = ((Node)this).CreateTween();
		val2.SetParallel(true);
		val2.TweenProperty((GodotObject)(object)_iconAdd, NodePath.op_Implicit("scale"), Variant.op_Implicit(Vector2.One * 1.5f), 1.0);
		val2.TweenProperty((GodotObject)(object)_iconAdd, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0f), 1.0).SetEase((EaseType)0).SetTrans((TransitionType)5);
		val2.TweenProperty((GodotObject)(object)_lock, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0f), 0.5).SetDelay(0.5);
		((CanvasItem)this).ZIndex = 0;
		Enable();
	}

	public void Reset()
	{
		foreach (Node child in ((Node)_playerIconContainer).GetChildren(false))
		{
			child.QueueFreeSafely();
		}
		_remoteSelectedPlayers.Clear();
		Deselect();
	}

	public void OnRemotePlayerSelected(ulong playerId)
	{
		_remoteSelectedPlayers.Add(playerId);
		RefreshState();
	}

	public void OnRemotePlayerDeselected(ulong playerId)
	{
		_remoteSelectedPlayers.Remove(playerId);
		RefreshState();
	}

	public void Select()
	{
		if (!_isSelected)
		{
			Tween? hoverTween = _hoverTween;
			if (hoverTween != null)
			{
				hoverTween.Kill();
			}
			_isSelected = true;
			_delegate.SelectCharacter(this, _character);
			RefreshState();
		}
	}

	public void Deselect()
	{
		_isSelected = false;
		RefreshState();
	}

	private void RefreshState()
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		State state = (_isSelected ? State.SelectedLocally : ((_remoteSelectedPlayers.Count > 0) ? State.SelectedRemotely : State.NotSelected));
		State state2 = _state;
		if (state2 != state)
		{
			_state = state;
			if (state2 == State.NotSelected)
			{
				_hsv.SetShaderParameter(_s, Variant.op_Implicit(GetSaturationForCurrentState()));
				_hsv.SetShaderParameter(_v, Variant.op_Implicit(GetValueForCurrentState()));
			}
			else
			{
				Tween? hoverTween = _hoverTween;
				if (hoverTween != null)
				{
					hoverTween.Kill();
				}
				_hoverTween = ((Node)this).CreateTween().SetParallel(true);
				AnimateSaturationToCurrentState(_hoverTween);
			}
		}
		RefreshOutline();
		RefreshPlayerIcons();
	}

	private float GetSaturationForCurrentState()
	{
		return _state switch
		{
			State.SelectedLocally => 1f, 
			State.SelectedRemotely => 0.8f, 
			State.NotSelected => 0.2f, 
			_ => throw new ArgumentOutOfRangeException(), 
		};
	}

	private float GetValueForCurrentState()
	{
		return _state switch
		{
			State.SelectedLocally => 1.1f, 
			State.SelectedRemotely => 0.4f, 
			State.NotSelected => 0.8f, 
			_ => throw new ArgumentOutOfRangeException(), 
		};
	}

	private void AnimateSaturationToCurrentState(Tween tween)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		tween.TweenMethod(Callable.From<float>((Action<float>)UpdateShaderS), _hsv.GetShaderParameter(_s), Variant.op_Implicit(GetSaturationForCurrentState()), 0.5).SetEase((EaseType)1).SetTrans((TransitionType)5);
		tween.TweenMethod(Callable.From<float>((Action<float>)UpdateShaderV), _hsv.GetShaderParameter(_v), Variant.op_Implicit(GetValueForCurrentState()), 0.5).SetEase((EaseType)1).SetTrans((TransitionType)5);
	}

	private void RefreshOutline()
	{
		if (_currentOutline != null)
		{
			((CanvasItem)_currentOutline).Visible = false;
		}
		if (_isSelected && _remoteSelectedPlayers.Count > 0)
		{
			_currentOutline = _outlineMixed;
		}
		else if (_isSelected)
		{
			_currentOutline = _outlineLocal;
		}
		else if (_remoteSelectedPlayers.Count > 0)
		{
			_currentOutline = _outlineRemote;
		}
		else
		{
			_currentOutline = null;
		}
		if (_currentOutline != null)
		{
			((CanvasItem)_currentOutline).Visible = true;
		}
	}

	private void RefreshPlayerIcons()
	{
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		if (_delegate != null && _delegate.Lobby.NetService.Type != NetGameType.Singleplayer)
		{
			int num = _remoteSelectedPlayers.Count + (_isSelected ? 1 : 0);
			for (int i = ((Node)_playerIconContainer).GetChildCount(false); i < num; i++)
			{
				TextureRect child = PreloadManager.Cache.GetScene(_playerIconScenePath).Instantiate<TextureRect>((GenEditState)0);
				((Node)(object)_playerIconContainer).AddChildSafely((Node?)(object)child);
			}
			while (((Node)_playerIconContainer).GetChildCount(false) > num)
			{
				Control child2 = ((Node)_playerIconContainer).GetChild<Control>(0, false);
				((Node)(object)_playerIconContainer).RemoveChildSafely((Node?)(object)child2);
				((Node)(object)child2).QueueFreeSafely();
			}
			for (int j = 0; j < ((Node)_playerIconContainer).GetChildCount(false); j++)
			{
				Control child3 = ((Node)_playerIconContainer).GetChild<Control>(j, false);
				((CanvasItem)child3).Modulate = ((_isSelected && j == 0) ? StsColors.gold : StsColors.blue);
			}
		}
	}

	public void DebugUnlock()
	{
		_icon.Texture = (Texture2D)(object)_character.CharacterSelectIcon;
		_isLocked = false;
		((CanvasItem)_lock).Visible = false;
		Enable();
	}

	public void UnlockIfPossible()
	{
		UnlockState unlockState = SaveManager.Instance.GenerateUnlockStateFromProgress();
		if (unlockState.Characters.Contains(_character))
		{
			_icon.Texture = (Texture2D)(object)_character.CharacterSelectIcon;
			_isLocked = false;
			((CanvasItem)_lock).Visible = false;
			Enable();
		}
	}

	private void UpdateShaderH(float value)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		_hsv.SetShaderParameter(_h, Variant.op_Implicit(value));
	}

	private void UpdateShaderS(float value)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		_hsv.SetShaderParameter(_s, Variant.op_Implicit(value));
	}

	private void UpdateShaderV(float value)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		_hsv.SetShaderParameter(_v, Variant.op_Implicit(value));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<MethodInfo> GetGodotMethodList()
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
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_0269: Unknown result type (might be due to invalid IL or missing references)
		//IL_0272: Unknown result type (might be due to invalid IL or missing references)
		//IL_0298: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0325: Unknown result type (might be due to invalid IL or missing references)
		//IL_034d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0358: Expected O, but got Unknown
		//IL_0353: Unknown result type (might be due to invalid IL or missing references)
		//IL_035e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0384: Unknown result type (might be due to invalid IL or missing references)
		//IL_038d: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0411: Unknown result type (might be due to invalid IL or missing references)
		//IL_041a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0440: Unknown result type (might be due to invalid IL or missing references)
		//IL_0463: Unknown result type (might be due to invalid IL or missing references)
		//IL_046e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0494: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_050b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0516: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(22);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnFocus, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnPress, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnUnfocus, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName._Process, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)3, StringName.op_Implicit("delta"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.LockForAnimation, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.Reset, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnRemotePlayerSelected, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("playerId"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.OnRemotePlayerDeselected, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("playerId"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.Select, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.Deselect, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.RefreshState, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.GetSaturationForCurrentState, new PropertyInfo((Type)3, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.GetValueForCurrentState, new PropertyInfo((Type)3, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.AnimateSaturationToCurrentState, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("tween"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("Tween"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.RefreshOutline, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.RefreshPlayerIcons, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.DebugUnlock, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UnlockIfPossible, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UpdateShaderH, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)3, StringName.op_Implicit("value"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UpdateShaderS, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)3, StringName.op_Implicit("value"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UpdateShaderV, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)3, StringName.op_Implicit("value"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_0269: Unknown result type (might be due to invalid IL or missing references)
		//IL_028e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0330: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0363: Unknown result type (might be due to invalid IL or missing references)
		//IL_0396: Unknown result type (might be due to invalid IL or missing references)
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
		if ((ref method) == MethodName.OnPress && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnPress();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnUnfocus && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			OnUnfocus();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName._Process && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			((Node)this)._Process(VariantUtils.ConvertTo<double>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.LockForAnimation && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			LockForAnimation();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.Reset && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			Reset();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnRemotePlayerSelected && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OnRemotePlayerSelected(VariantUtils.ConvertTo<ulong>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.OnRemotePlayerDeselected && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			OnRemotePlayerDeselected(VariantUtils.ConvertTo<ulong>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.Select && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			Select();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.Deselect && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			Deselect();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.RefreshState && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			RefreshState();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.GetSaturationForCurrentState && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			float saturationForCurrentState = GetSaturationForCurrentState();
			ret = VariantUtils.CreateFrom<float>(ref saturationForCurrentState);
			return true;
		}
		if ((ref method) == MethodName.GetValueForCurrentState && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			float valueForCurrentState = GetValueForCurrentState();
			ret = VariantUtils.CreateFrom<float>(ref valueForCurrentState);
			return true;
		}
		if ((ref method) == MethodName.AnimateSaturationToCurrentState && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			AnimateSaturationToCurrentState(VariantUtils.ConvertTo<Tween>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.RefreshOutline && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			RefreshOutline();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.RefreshPlayerIcons && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			RefreshPlayerIcons();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.DebugUnlock && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			DebugUnlock();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.UnlockIfPossible && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			UnlockIfPossible();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.UpdateShaderH && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			UpdateShaderH(VariantUtils.ConvertTo<float>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.UpdateShaderS && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			UpdateShaderS(VariantUtils.ConvertTo<float>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.UpdateShaderV && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			UpdateShaderV(VariantUtils.ConvertTo<float>(ref ((NativeVariantPtrArgs)(ref args))[0]));
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
		if ((ref method) == MethodName.OnPress)
		{
			return true;
		}
		if ((ref method) == MethodName.OnUnfocus)
		{
			return true;
		}
		if ((ref method) == MethodName._Process)
		{
			return true;
		}
		if ((ref method) == MethodName.LockForAnimation)
		{
			return true;
		}
		if ((ref method) == MethodName.Reset)
		{
			return true;
		}
		if ((ref method) == MethodName.OnRemotePlayerSelected)
		{
			return true;
		}
		if ((ref method) == MethodName.OnRemotePlayerDeselected)
		{
			return true;
		}
		if ((ref method) == MethodName.Select)
		{
			return true;
		}
		if ((ref method) == MethodName.Deselect)
		{
			return true;
		}
		if ((ref method) == MethodName.RefreshState)
		{
			return true;
		}
		if ((ref method) == MethodName.GetSaturationForCurrentState)
		{
			return true;
		}
		if ((ref method) == MethodName.GetValueForCurrentState)
		{
			return true;
		}
		if ((ref method) == MethodName.AnimateSaturationToCurrentState)
		{
			return true;
		}
		if ((ref method) == MethodName.RefreshOutline)
		{
			return true;
		}
		if ((ref method) == MethodName.RefreshPlayerIcons)
		{
			return true;
		}
		if ((ref method) == MethodName.DebugUnlock)
		{
			return true;
		}
		if ((ref method) == MethodName.UnlockIfPossible)
		{
			return true;
		}
		if ((ref method) == MethodName.UpdateShaderH)
		{
			return true;
		}
		if ((ref method) == MethodName.UpdateShaderS)
		{
			return true;
		}
		if ((ref method) == MethodName.UpdateShaderV)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName.IsRandom)
		{
			IsRandom = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._icon)
		{
			_icon = VariantUtils.ConvertTo<TextureRect>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._iconAdd)
		{
			_iconAdd = VariantUtils.ConvertTo<TextureRect>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._lock)
		{
			_lock = VariantUtils.ConvertTo<TextureRect>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._outlineLocal)
		{
			_outlineLocal = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._outlineRemote)
		{
			_outlineRemote = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._outlineMixed)
		{
			_outlineMixed = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._shadow)
		{
			_shadow = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._playerIconContainer)
		{
			_playerIconContainer = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._hsv)
		{
			_hsv = VariantUtils.ConvertTo<ShaderMaterial>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._isLocked)
		{
			_isLocked = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._currentOutline)
		{
			_currentOutline = VariantUtils.ConvertTo<Control>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._isSelected)
		{
			_isSelected = VariantUtils.ConvertTo<bool>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._state)
		{
			_state = VariantUtils.ConvertTo<State>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._hoverTween)
		{
			_hoverTween = VariantUtils.ConvertTo<Tween>(ref value);
			return true;
		}
		if ((ref name) == PropertyName._hsvTween)
		{
			_hsvTween = VariantUtils.ConvertTo<Tween>(ref value);
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
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName.IsRandom)
		{
			bool isRandom = IsRandom;
			value = VariantUtils.CreateFrom<bool>(ref isRandom);
			return true;
		}
		if ((ref name) == PropertyName.IsLocked)
		{
			bool isRandom = IsLocked;
			value = VariantUtils.CreateFrom<bool>(ref isRandom);
			return true;
		}
		if ((ref name) == PropertyName._icon)
		{
			value = VariantUtils.CreateFrom<TextureRect>(ref _icon);
			return true;
		}
		if ((ref name) == PropertyName._iconAdd)
		{
			value = VariantUtils.CreateFrom<TextureRect>(ref _iconAdd);
			return true;
		}
		if ((ref name) == PropertyName._lock)
		{
			value = VariantUtils.CreateFrom<TextureRect>(ref _lock);
			return true;
		}
		if ((ref name) == PropertyName._outlineLocal)
		{
			value = VariantUtils.CreateFrom<Control>(ref _outlineLocal);
			return true;
		}
		if ((ref name) == PropertyName._outlineRemote)
		{
			value = VariantUtils.CreateFrom<Control>(ref _outlineRemote);
			return true;
		}
		if ((ref name) == PropertyName._outlineMixed)
		{
			value = VariantUtils.CreateFrom<Control>(ref _outlineMixed);
			return true;
		}
		if ((ref name) == PropertyName._shadow)
		{
			value = VariantUtils.CreateFrom<Control>(ref _shadow);
			return true;
		}
		if ((ref name) == PropertyName._playerIconContainer)
		{
			value = VariantUtils.CreateFrom<Control>(ref _playerIconContainer);
			return true;
		}
		if ((ref name) == PropertyName._hsv)
		{
			value = VariantUtils.CreateFrom<ShaderMaterial>(ref _hsv);
			return true;
		}
		if ((ref name) == PropertyName._isLocked)
		{
			value = VariantUtils.CreateFrom<bool>(ref _isLocked);
			return true;
		}
		if ((ref name) == PropertyName._currentOutline)
		{
			value = VariantUtils.CreateFrom<Control>(ref _currentOutline);
			return true;
		}
		if ((ref name) == PropertyName._isSelected)
		{
			value = VariantUtils.CreateFrom<bool>(ref _isSelected);
			return true;
		}
		if ((ref name) == PropertyName._state)
		{
			value = VariantUtils.CreateFrom<State>(ref _state);
			return true;
		}
		if ((ref name) == PropertyName._hoverTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _hoverTween);
			return true;
		}
		if ((ref name) == PropertyName._hsvTween)
		{
			value = VariantUtils.CreateFrom<Tween>(ref _hsvTween);
			return true;
		}
		return base.GetGodotClassPropertyValue(in name, out value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<PropertyInfo> GetGodotPropertyList()
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
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._icon, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._iconAdd, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._lock, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._outlineLocal, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._outlineRemote, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._outlineMixed, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._shadow, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._playerIconContainer, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._hsv, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._isLocked, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName.IsRandom, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._currentOutline, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName._isSelected, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)2, PropertyName._state, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._hoverTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)24, PropertyName._hsvTween, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		list.Add(new PropertyInfo((Type)1, PropertyName.IsLocked, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
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
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		base.SaveGodotObjectData(info);
		StringName isRandom = PropertyName.IsRandom;
		bool isRandom2 = IsRandom;
		info.AddProperty(isRandom, Variant.From<bool>(ref isRandom2));
		info.AddProperty(PropertyName._icon, Variant.From<TextureRect>(ref _icon));
		info.AddProperty(PropertyName._iconAdd, Variant.From<TextureRect>(ref _iconAdd));
		info.AddProperty(PropertyName._lock, Variant.From<TextureRect>(ref _lock));
		info.AddProperty(PropertyName._outlineLocal, Variant.From<Control>(ref _outlineLocal));
		info.AddProperty(PropertyName._outlineRemote, Variant.From<Control>(ref _outlineRemote));
		info.AddProperty(PropertyName._outlineMixed, Variant.From<Control>(ref _outlineMixed));
		info.AddProperty(PropertyName._shadow, Variant.From<Control>(ref _shadow));
		info.AddProperty(PropertyName._playerIconContainer, Variant.From<Control>(ref _playerIconContainer));
		info.AddProperty(PropertyName._hsv, Variant.From<ShaderMaterial>(ref _hsv));
		info.AddProperty(PropertyName._isLocked, Variant.From<bool>(ref _isLocked));
		info.AddProperty(PropertyName._currentOutline, Variant.From<Control>(ref _currentOutline));
		info.AddProperty(PropertyName._isSelected, Variant.From<bool>(ref _isSelected));
		info.AddProperty(PropertyName._state, Variant.From<State>(ref _state));
		info.AddProperty(PropertyName._hoverTween, Variant.From<Tween>(ref _hoverTween));
		info.AddProperty(PropertyName._hsvTween, Variant.From<Tween>(ref _hsvTween));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName.IsRandom, ref val))
		{
			IsRandom = ((Variant)(ref val)).As<bool>();
		}
		Variant val2 = default(Variant);
		if (info.TryGetProperty(PropertyName._icon, ref val2))
		{
			_icon = ((Variant)(ref val2)).As<TextureRect>();
		}
		Variant val3 = default(Variant);
		if (info.TryGetProperty(PropertyName._iconAdd, ref val3))
		{
			_iconAdd = ((Variant)(ref val3)).As<TextureRect>();
		}
		Variant val4 = default(Variant);
		if (info.TryGetProperty(PropertyName._lock, ref val4))
		{
			_lock = ((Variant)(ref val4)).As<TextureRect>();
		}
		Variant val5 = default(Variant);
		if (info.TryGetProperty(PropertyName._outlineLocal, ref val5))
		{
			_outlineLocal = ((Variant)(ref val5)).As<Control>();
		}
		Variant val6 = default(Variant);
		if (info.TryGetProperty(PropertyName._outlineRemote, ref val6))
		{
			_outlineRemote = ((Variant)(ref val6)).As<Control>();
		}
		Variant val7 = default(Variant);
		if (info.TryGetProperty(PropertyName._outlineMixed, ref val7))
		{
			_outlineMixed = ((Variant)(ref val7)).As<Control>();
		}
		Variant val8 = default(Variant);
		if (info.TryGetProperty(PropertyName._shadow, ref val8))
		{
			_shadow = ((Variant)(ref val8)).As<Control>();
		}
		Variant val9 = default(Variant);
		if (info.TryGetProperty(PropertyName._playerIconContainer, ref val9))
		{
			_playerIconContainer = ((Variant)(ref val9)).As<Control>();
		}
		Variant val10 = default(Variant);
		if (info.TryGetProperty(PropertyName._hsv, ref val10))
		{
			_hsv = ((Variant)(ref val10)).As<ShaderMaterial>();
		}
		Variant val11 = default(Variant);
		if (info.TryGetProperty(PropertyName._isLocked, ref val11))
		{
			_isLocked = ((Variant)(ref val11)).As<bool>();
		}
		Variant val12 = default(Variant);
		if (info.TryGetProperty(PropertyName._currentOutline, ref val12))
		{
			_currentOutline = ((Variant)(ref val12)).As<Control>();
		}
		Variant val13 = default(Variant);
		if (info.TryGetProperty(PropertyName._isSelected, ref val13))
		{
			_isSelected = ((Variant)(ref val13)).As<bool>();
		}
		Variant val14 = default(Variant);
		if (info.TryGetProperty(PropertyName._state, ref val14))
		{
			_state = ((Variant)(ref val14)).As<State>();
		}
		Variant val15 = default(Variant);
		if (info.TryGetProperty(PropertyName._hoverTween, ref val15))
		{
			_hoverTween = ((Variant)(ref val15)).As<Tween>();
		}
		Variant val16 = default(Variant);
		if (info.TryGetProperty(PropertyName._hsvTween, ref val16))
		{
			_hsvTween = ((Variant)(ref val16)).As<Tween>();
		}
	}
}
