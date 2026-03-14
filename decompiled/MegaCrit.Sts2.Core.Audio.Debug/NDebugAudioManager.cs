using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Random;

namespace MegaCrit.Sts2.Core.Audio.Debug;

[ScriptPath("res://src/Core/Audio/Debug/NDebugAudioManager.cs")]
public class NDebugAudioManager : Node
{
	private struct PlayingSound
	{
		public int id;

		public AudioStreamPlayer player;

		public Callable callable;
	}

	public class MethodName : MethodName
	{
		public static readonly StringName _Ready = StringName.op_Implicit("_Ready");

		public static readonly StringName Play = StringName.op_Implicit("Play");

		public static readonly StringName Stop = StringName.op_Implicit("Stop");

		public static readonly StringName StopInternalById = StringName.op_Implicit("StopInternalById");

		public static readonly StringName StopInternal = StringName.op_Implicit("StopInternal");

		public static readonly StringName SetMasterAudioVolume = StringName.op_Implicit("SetMasterAudioVolume");

		public static readonly StringName SetSfxAudioVolume = StringName.op_Implicit("SetSfxAudioVolume");

		public static readonly StringName PlayerFinished = StringName.op_Implicit("PlayerFinished");

		public static readonly StringName GetRandomPitchScale = StringName.op_Implicit("GetRandomPitchScale");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName _nextId = StringName.op_Implicit("_nextId");
	}

	public class SignalName : SignalName
	{
	}

	private static readonly StringName _sfx = new StringName("SFX");

	private static readonly StringName _master = new StringName("Master");

	private List<AudioStreamPlayer> _freeAudioPlayers = new List<AudioStreamPlayer>();

	private readonly List<PlayingSound> _playingSounds = new List<PlayingSound>();

	private int _nextId;

	public static NDebugAudioManager? Instance => NGame.Instance?.DebugAudio;

	public override void _Ready()
	{
		_freeAudioPlayers.AddRange(((IEnumerable)((Node)this).GetChildren(false)).OfType<AudioStreamPlayer>());
	}

	public int Play(string streamName, float volume = 1f, PitchVariance variance = PitchVariance.None)
	{
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Expected O, but got Unknown
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		AudioStreamPlayer streamPlayer;
		if (_freeAudioPlayers.Count > 0)
		{
			List<AudioStreamPlayer> freeAudioPlayers = _freeAudioPlayers;
			streamPlayer = freeAudioPlayers[freeAudioPlayers.Count - 1];
			_freeAudioPlayers.RemoveAt(_freeAudioPlayers.Count - 1);
		}
		else
		{
			streamPlayer = new AudioStreamPlayer();
			((Node)(object)this).AddChildSafely((Node?)(object)streamPlayer);
		}
		AudioStream asset = PreloadManager.Cache.GetAsset<AudioStream>(TmpSfx.GetPath(streamName));
		((Node)streamPlayer).Name = StringName.op_Implicit("StreamPlayer-" + streamName);
		streamPlayer.Stream = asset;
		streamPlayer.VolumeLinear = volume;
		streamPlayer.PitchScale = GetRandomPitchScale(variance);
		streamPlayer.Bus = _sfx;
		Callable val = Callable.From((Action)delegate
		{
			PlayerFinished(streamPlayer);
		});
		((GodotObject)streamPlayer).Connect(SignalName.Finished, val, 0u);
		streamPlayer.Play(0f);
		PlayingSound item = new PlayingSound
		{
			id = _nextId,
			player = streamPlayer,
			callable = val
		};
		_playingSounds.Add(item);
		_nextId++;
		return item.id;
	}

	public void Stop(int id, float fadeTime = 0.5f)
	{
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		int i;
		for (i = 0; i < _playingSounds.Count; i++)
		{
			PlayingSound playingSound = _playingSounds[i];
			if (playingSound.id != id)
			{
				continue;
			}
			if (fadeTime > 0f)
			{
				Tween val = ((Node)this).CreateTween();
				val.TweenProperty((GodotObject)(object)playingSound.player, NodePath.op_Implicit("volume_linear"), Variant.op_Implicit(0f), (double)fadeTime);
				val.TweenCallback(Callable.From((Action)delegate
				{
					StopInternalById(i);
				}));
			}
			else
			{
				StopInternal(i);
			}
			return;
		}
		Log.Warn($"Tried to stop sound with ID {id} but no sound with that ID was found!");
	}

	private void StopInternalById(int id)
	{
		for (int i = 0; i < _playingSounds.Count; i++)
		{
			if (_playingSounds[i].id == id)
			{
				StopInternal(i);
				break;
			}
		}
	}

	private void StopInternal(int soundIndex)
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		PlayingSound playingSound = _playingSounds[soundIndex];
		if (playingSound.player.IsPlaying())
		{
			playingSound.player.Stop();
		}
		((GodotObject)playingSound.player).Disconnect(SignalName.Finished, playingSound.callable);
		_playingSounds.RemoveAt(soundIndex);
		_freeAudioPlayers.Add(playingSound.player);
	}

	public void SetMasterAudioVolume(float linearVolume)
	{
		AudioServer.Singleton.SetBusVolumeDb(AudioServer.Singleton.GetBusIndex(_master), Mathf.LinearToDb(Mathf.Pow(linearVolume, 2f)));
	}

	public void SetSfxAudioVolume(float linearVolume)
	{
		AudioServer.Singleton.SetBusVolumeDb(AudioServer.Singleton.GetBusIndex(_sfx), Mathf.LinearToDb(Mathf.Pow(linearVolume, 2f)));
	}

	private void PlayerFinished(AudioStreamPlayer player)
	{
		for (int i = 0; i < _playingSounds.Count; i++)
		{
			if (_playingSounds[i].player == player)
			{
				StopInternal(i);
				break;
			}
		}
	}

	private float GetRandomPitchScale(PitchVariance variance)
	{
		float num = variance switch
		{
			PitchVariance.None => 0f, 
			PitchVariance.Small => 0.02f, 
			PitchVariance.Medium => 0.05f, 
			PitchVariance.Large => 0.1f, 
			PitchVariance.TooMuch => 0.2f, 
			_ => throw new ArgumentOutOfRangeException("variance", variance, null), 
		};
		if (num == 0f)
		{
			return 1f;
		}
		return 1f + Rng.Chaotic.NextFloat(0f - num, num);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0288: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e1: Expected O, but got Unknown
		//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_030d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0330: Unknown result type (might be due to invalid IL or missing references)
		//IL_033b: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(9);
		list.Add(new MethodInfo(MethodName._Ready, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.Play, new PropertyInfo((Type)2, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)4, StringName.op_Implicit("streamName"), (PropertyHint)0, "", (PropertyUsageFlags)6, false),
			new PropertyInfo((Type)3, StringName.op_Implicit("volume"), (PropertyHint)0, "", (PropertyUsageFlags)6, false),
			new PropertyInfo((Type)2, StringName.op_Implicit("variance"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.Stop, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("id"), (PropertyHint)0, "", (PropertyUsageFlags)6, false),
			new PropertyInfo((Type)3, StringName.op_Implicit("fadeTime"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.StopInternalById, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("id"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.StopInternal, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("soundIndex"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetMasterAudioVolume, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)3, StringName.op_Implicit("linearVolume"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetSfxAudioVolume, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)3, StringName.op_Implicit("linearVolume"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.PlayerFinished, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)24, StringName.op_Implicit("player"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("AudioStreamPlayer"), false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.GetRandomPitchScale, new PropertyInfo((Type)3, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)2, StringName.op_Implicit("variance"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._Ready && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._Ready();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.Play && ((NativeVariantPtrArgs)(ref args)).Count == 3)
		{
			int num = Play(VariantUtils.ConvertTo<string>(ref ((NativeVariantPtrArgs)(ref args))[0]), VariantUtils.ConvertTo<float>(ref ((NativeVariantPtrArgs)(ref args))[1]), VariantUtils.ConvertTo<PitchVariance>(ref ((NativeVariantPtrArgs)(ref args))[2]));
			ret = VariantUtils.CreateFrom<int>(ref num);
			return true;
		}
		if ((ref method) == MethodName.Stop && ((NativeVariantPtrArgs)(ref args)).Count == 2)
		{
			Stop(VariantUtils.ConvertTo<int>(ref ((NativeVariantPtrArgs)(ref args))[0]), VariantUtils.ConvertTo<float>(ref ((NativeVariantPtrArgs)(ref args))[1]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.StopInternalById && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			StopInternalById(VariantUtils.ConvertTo<int>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.StopInternal && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			StopInternal(VariantUtils.ConvertTo<int>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetMasterAudioVolume && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			SetMasterAudioVolume(VariantUtils.ConvertTo<float>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetSfxAudioVolume && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			SetSfxAudioVolume(VariantUtils.ConvertTo<float>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.PlayerFinished && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			PlayerFinished(VariantUtils.ConvertTo<AudioStreamPlayer>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.GetRandomPitchScale && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			float randomPitchScale = GetRandomPitchScale(VariantUtils.ConvertTo<PitchVariance>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = VariantUtils.CreateFrom<float>(ref randomPitchScale);
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
		if ((ref method) == MethodName.Play)
		{
			return true;
		}
		if ((ref method) == MethodName.Stop)
		{
			return true;
		}
		if ((ref method) == MethodName.StopInternalById)
		{
			return true;
		}
		if ((ref method) == MethodName.StopInternal)
		{
			return true;
		}
		if ((ref method) == MethodName.SetMasterAudioVolume)
		{
			return true;
		}
		if ((ref method) == MethodName.SetSfxAudioVolume)
		{
			return true;
		}
		if ((ref method) == MethodName.PlayerFinished)
		{
			return true;
		}
		if ((ref method) == MethodName.GetRandomPitchScale)
		{
			return true;
		}
		return ((Node)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._nextId)
		{
			_nextId = VariantUtils.ConvertTo<int>(ref value);
			return true;
		}
		return ((GodotObject)this).SetGodotClassPropertyValue(ref name, ref value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName._nextId)
		{
			value = VariantUtils.CreateFrom<int>(ref _nextId);
			return true;
		}
		return ((GodotObject)this).GetGodotClassPropertyValue(ref name, ref value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)2, PropertyName._nextId, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		info.AddProperty(PropertyName._nextId, Variant.From<int>(ref _nextId));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._nextId, ref val))
		{
			_nextId = ((Variant)(ref val)).As<int>();
		}
	}
}
