using System;
using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.Collections;
using Godot.NativeInterop;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.TestSupport;

namespace MegaCrit.Sts2.Core.Nodes.Audio;

[ScriptPath("res://src/Core/Nodes/Audio/NAudioManager.cs")]
public class NAudioManager : Node
{
	public class MethodName : MethodName
	{
		public static readonly StringName _EnterTree = StringName.op_Implicit("_EnterTree");

		public static readonly StringName PlayLoop = StringName.op_Implicit("PlayLoop");

		public static readonly StringName StopLoop = StringName.op_Implicit("StopLoop");

		public static readonly StringName SetParam = StringName.op_Implicit("SetParam");

		public static readonly StringName StopAllLoops = StringName.op_Implicit("StopAllLoops");

		public static readonly StringName PlayOneShot = StringName.op_Implicit("PlayOneShot");

		public static readonly StringName PlayMusic = StringName.op_Implicit("PlayMusic");

		public static readonly StringName UpdateMusicParameter = StringName.op_Implicit("UpdateMusicParameter");

		public static readonly StringName StopMusic = StringName.op_Implicit("StopMusic");

		public static readonly StringName SetMasterVol = StringName.op_Implicit("SetMasterVol");

		public static readonly StringName SetSfxVol = StringName.op_Implicit("SetSfxVol");

		public static readonly StringName SetAmbienceVol = StringName.op_Implicit("SetAmbienceVol");

		public static readonly StringName SetBgmVol = StringName.op_Implicit("SetBgmVol");
	}

	public class PropertyName : PropertyName
	{
		public static readonly StringName _audioNode = StringName.op_Implicit("_audioNode");
	}

	public class SignalName : SignalName
	{
	}

	private static readonly StringName _setBgmVolume = new StringName("set_bgm_volume");

	private static readonly StringName _setAmbienceVolume = new StringName("set_ambience_volume");

	private static readonly StringName _setSfxVolume = new StringName("set_sfx_volume");

	private static readonly StringName _setMasterVolume = new StringName("set_master_volume");

	private static readonly StringName _stopMusic = new StringName("stop_music");

	private static readonly StringName _playMusic = new StringName("play_music");

	private static readonly StringName _playOneShot = new StringName("play_one_shot");

	private static readonly StringName _stopAllLoops = new StringName("stop_all_loops");

	private static readonly StringName _setParam = new StringName("set_param");

	private static readonly StringName _stopLoop = new StringName("stop_loop");

	private static readonly StringName _playLoop = new StringName("play_loop");

	private static readonly StringName _updateMusicParameterCallback = new StringName("update_music_parameter");

	private Node _audioNode;

	public static NAudioManager? Instance => NGame.Instance?.AudioManager;

	public override void _EnterTree()
	{
		_audioNode = ((Node)this).GetNode<Node>(NodePath.op_Implicit("Proxy"));
	}

	public void PlayLoop(string path, bool usesLoopParam)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		if (!TestMode.IsOn)
		{
			((GodotObject)_audioNode).Call(_playLoop, (Variant[])(object)new Variant[2]
			{
				Variant.op_Implicit(path),
				Variant.op_Implicit(usesLoopParam)
			});
		}
	}

	public void StopLoop(string path)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		if (!TestMode.IsOn)
		{
			((GodotObject)_audioNode).Call(_stopLoop, (Variant[])(object)new Variant[1] { Variant.op_Implicit(path) });
		}
	}

	public void SetParam(string path, string param, float value)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		if (!TestMode.IsOn)
		{
			((GodotObject)_audioNode).Call(_setParam, (Variant[])(object)new Variant[3]
			{
				Variant.op_Implicit(path),
				Variant.op_Implicit(param),
				Variant.op_Implicit(value)
			});
		}
	}

	public void StopAllLoops()
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		if (!TestMode.IsOn)
		{
			((GodotObject)_audioNode).Call(_stopAllLoops, Array.Empty<Variant>());
		}
	}

	public void PlayOneShot(string path, Dictionary<string, float> parameters, float volume = 1f)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Expected O, but got Unknown
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		if (TestMode.IsOn)
		{
			return;
		}
		Dictionary val = new Dictionary();
		foreach (KeyValuePair<string, float> parameter in parameters)
		{
			val.Add(Variant.op_Implicit(parameter.Key), Variant.op_Implicit(parameter.Value));
		}
		((GodotObject)_audioNode).Call(_playOneShot, (Variant[])(object)new Variant[3]
		{
			Variant.op_Implicit(path),
			Variant.op_Implicit(val),
			Variant.op_Implicit(volume)
		});
	}

	public void PlayOneShot(string path, float volume = 1f)
	{
		if (!TestMode.IsOn)
		{
			PlayOneShot(path, new Dictionary<string, float>(), volume);
		}
	}

	public void PlayMusic(string music)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		if (!TestMode.IsOn)
		{
			((GodotObject)_audioNode).Call(_playMusic, (Variant[])(object)new Variant[1] { Variant.op_Implicit(music) });
		}
	}

	public void UpdateMusicParameter(string parameter, string value)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		if (!NonInteractiveMode.IsActive)
		{
			((GodotObject)_audioNode).Call(_updateMusicParameterCallback, (Variant[])(object)new Variant[2]
			{
				Variant.op_Implicit(parameter),
				Variant.op_Implicit(value)
			});
		}
	}

	public void StopMusic()
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		if (!TestMode.IsOn)
		{
			((GodotObject)_audioNode).Call(_stopMusic, Array.Empty<Variant>());
		}
	}

	public void SetMasterVol(float volume)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		if (!TestMode.IsOn)
		{
			((GodotObject)_audioNode).Call(_setMasterVolume, (Variant[])(object)new Variant[1] { Variant.op_Implicit(Mathf.Pow(volume, 2f)) });
		}
	}

	public void SetSfxVol(float volume)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		if (!TestMode.IsOn)
		{
			((GodotObject)_audioNode).Call(_setSfxVolume, (Variant[])(object)new Variant[1] { Variant.op_Implicit(Mathf.Pow(volume, 2f)) });
		}
	}

	public void SetAmbienceVol(float volume)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		if (!TestMode.IsOn)
		{
			((GodotObject)_audioNode).Call(_setAmbienceVolume, (Variant[])(object)new Variant[1] { Variant.op_Implicit(Mathf.Pow(volume, 2f)) });
		}
	}

	public void SetBgmVol(float volume)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		if (!TestMode.IsOn)
		{
			((GodotObject)_audioNode).Call(_setBgmVolume, (Variant[])(object)new Variant[1] { Variant.op_Implicit(Mathf.Pow(volume, 2f)) });
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_0279: Unknown result type (might be due to invalid IL or missing references)
		//IL_0284: Unknown result type (might be due to invalid IL or missing references)
		//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_031f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0328: Unknown result type (might be due to invalid IL or missing references)
		//IL_034e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0371: Unknown result type (might be due to invalid IL or missing references)
		//IL_037c: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0419: Unknown result type (might be due to invalid IL or missing references)
		//IL_0424: Unknown result type (might be due to invalid IL or missing references)
		//IL_044a: Unknown result type (might be due to invalid IL or missing references)
		//IL_046d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0478: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> list = new List<MethodInfo>(13);
		list.Add(new MethodInfo(MethodName._EnterTree, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.PlayLoop, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)4, StringName.op_Implicit("path"), (PropertyHint)0, "", (PropertyUsageFlags)6, false),
			new PropertyInfo((Type)1, StringName.op_Implicit("usesLoopParam"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.StopLoop, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)4, StringName.op_Implicit("path"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetParam, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)4, StringName.op_Implicit("path"), (PropertyHint)0, "", (PropertyUsageFlags)6, false),
			new PropertyInfo((Type)4, StringName.op_Implicit("param"), (PropertyHint)0, "", (PropertyUsageFlags)6, false),
			new PropertyInfo((Type)3, StringName.op_Implicit("value"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.StopAllLoops, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.PlayOneShot, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)4, StringName.op_Implicit("path"), (PropertyHint)0, "", (PropertyUsageFlags)6, false),
			new PropertyInfo((Type)3, StringName.op_Implicit("volume"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.PlayMusic, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)4, StringName.op_Implicit("music"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.UpdateMusicParameter, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)4, StringName.op_Implicit("parameter"), (PropertyHint)0, "", (PropertyUsageFlags)6, false),
			new PropertyInfo((Type)4, StringName.op_Implicit("value"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.StopMusic, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, (List<PropertyInfo>)null, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetMasterVol, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)3, StringName.op_Implicit("volume"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetSfxVol, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)3, StringName.op_Implicit("volume"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetAmbienceVol, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)3, StringName.op_Implicit("volume"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		list.Add(new MethodInfo(MethodName.SetBgmVol, new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false), (MethodFlags)1, new List<PropertyInfo>
		{
			new PropertyInfo((Type)3, StringName.op_Implicit("volume"), (PropertyHint)0, "", (PropertyUsageFlags)6, false)
		}, (List<Variant>)null));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._EnterTree && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			((Node)this)._EnterTree();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.PlayLoop && ((NativeVariantPtrArgs)(ref args)).Count == 2)
		{
			PlayLoop(VariantUtils.ConvertTo<string>(ref ((NativeVariantPtrArgs)(ref args))[0]), VariantUtils.ConvertTo<bool>(ref ((NativeVariantPtrArgs)(ref args))[1]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.StopLoop && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			StopLoop(VariantUtils.ConvertTo<string>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetParam && ((NativeVariantPtrArgs)(ref args)).Count == 3)
		{
			SetParam(VariantUtils.ConvertTo<string>(ref ((NativeVariantPtrArgs)(ref args))[0]), VariantUtils.ConvertTo<string>(ref ((NativeVariantPtrArgs)(ref args))[1]), VariantUtils.ConvertTo<float>(ref ((NativeVariantPtrArgs)(ref args))[2]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.StopAllLoops && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			StopAllLoops();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.PlayOneShot && ((NativeVariantPtrArgs)(ref args)).Count == 2)
		{
			PlayOneShot(VariantUtils.ConvertTo<string>(ref ((NativeVariantPtrArgs)(ref args))[0]), VariantUtils.ConvertTo<float>(ref ((NativeVariantPtrArgs)(ref args))[1]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.PlayMusic && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			PlayMusic(VariantUtils.ConvertTo<string>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.UpdateMusicParameter && ((NativeVariantPtrArgs)(ref args)).Count == 2)
		{
			UpdateMusicParameter(VariantUtils.ConvertTo<string>(ref ((NativeVariantPtrArgs)(ref args))[0]), VariantUtils.ConvertTo<string>(ref ((NativeVariantPtrArgs)(ref args))[1]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.StopMusic && ((NativeVariantPtrArgs)(ref args)).Count == 0)
		{
			StopMusic();
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetMasterVol && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			SetMasterVol(VariantUtils.ConvertTo<float>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetSfxVol && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			SetSfxVol(VariantUtils.ConvertTo<float>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetAmbienceVol && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			SetAmbienceVol(VariantUtils.ConvertTo<float>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		if ((ref method) == MethodName.SetBgmVol && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			SetBgmVol(VariantUtils.ConvertTo<float>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		return ((Node)this).InvokeGodotClassMethod(ref method, args, ref ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if ((ref method) == MethodName._EnterTree)
		{
			return true;
		}
		if ((ref method) == MethodName.PlayLoop)
		{
			return true;
		}
		if ((ref method) == MethodName.StopLoop)
		{
			return true;
		}
		if ((ref method) == MethodName.SetParam)
		{
			return true;
		}
		if ((ref method) == MethodName.StopAllLoops)
		{
			return true;
		}
		if ((ref method) == MethodName.PlayOneShot)
		{
			return true;
		}
		if ((ref method) == MethodName.PlayMusic)
		{
			return true;
		}
		if ((ref method) == MethodName.UpdateMusicParameter)
		{
			return true;
		}
		if ((ref method) == MethodName.StopMusic)
		{
			return true;
		}
		if ((ref method) == MethodName.SetMasterVol)
		{
			return true;
		}
		if ((ref method) == MethodName.SetSfxVol)
		{
			return true;
		}
		if ((ref method) == MethodName.SetAmbienceVol)
		{
			return true;
		}
		if ((ref method) == MethodName.SetBgmVol)
		{
			return true;
		}
		return ((Node)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if ((ref name) == PropertyName._audioNode)
		{
			_audioNode = VariantUtils.ConvertTo<Node>(ref value);
			return true;
		}
		return ((GodotObject)this).SetGodotClassPropertyValue(ref name, ref value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		if ((ref name) == PropertyName._audioNode)
		{
			value = VariantUtils.CreateFrom<Node>(ref _audioNode);
			return true;
		}
		return ((GodotObject)this).GetGodotClassPropertyValue(ref name, ref value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		List<PropertyInfo> list = new List<PropertyInfo>();
		list.Add(new PropertyInfo((Type)24, PropertyName._audioNode, (PropertyHint)0, "", (PropertyUsageFlags)4096, false));
		return list;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		((GodotObject)this).SaveGodotObjectData(info);
		info.AddProperty(PropertyName._audioNode, Variant.From<Node>(ref _audioNode));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
		Variant val = default(Variant);
		if (info.TryGetProperty(PropertyName._audioNode, ref val))
		{
			_audioNode = ((Variant)(ref val)).As<Node>();
		}
	}
}
