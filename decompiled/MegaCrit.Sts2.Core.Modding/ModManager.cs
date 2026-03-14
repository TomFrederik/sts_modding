using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text.Json;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Platform.Steam;
using MegaCrit.Sts2.Core.Saves;
using Steamworks;

namespace MegaCrit.Sts2.Core.Modding;

public static class ModManager
{
	public delegate void MetricsUploadHook(SerializableRun run, bool isVictory, ulong localPlayerId);

	private static List<Mod> _mods = new List<Mod>();

	private static List<Mod> _loadedMods = new List<Mod>();

	private static bool _initialized;

	private static Callback<ItemInstalled_t>? _steamItemInstalledCallback;

	public static IReadOnlyList<Mod> AllMods => _mods;

	public static IReadOnlyList<Mod> LoadedMods => _loadedMods;

	public static bool PlayerAgreedToModLoading => SaveManager.Instance.SettingsSave.ModSettings?.PlayerAgreedToModLoading ?? false;

	public static event Action<Mod>? OnModDetected;

	public static event MetricsUploadHook? OnMetricsUpload;

	public static void Initialize()
	{
		if (CommandLineHelper.HasArg("nomods"))
		{
			Log.Info("'nomods' passed as executable argument, skipping mod initialization");
			return;
		}
		AppDomain.CurrentDomain.AssemblyResolve += HandleAssemblyResolveFailure;
		string executablePath = OS.GetExecutablePath();
		string directoryName = Path.GetDirectoryName(executablePath);
		DirAccess val = DirAccess.Open(Path.Combine(directoryName, "mods"));
		try
		{
			if (val != null)
			{
				LoadModsInDirRecursive(val, ModSource.ModsDirectory);
			}
			if (SteamInitializer.Initialized)
			{
				InitializeSteamMods();
			}
			_loadedMods = _mods.Where((Mod m) => m.wasLoaded).ToList();
			if (_loadedMods.Count > 0)
			{
				Log.Info($" --- RUNNING MODDED! --- Loaded {_loadedMods.Count} mods ({_mods.Count} total)");
			}
			_initialized = true;
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private static void LoadModsInDirRecursive(DirAccess dirAccess, ModSource source)
	{
		string[] files = dirAccess.GetFiles();
		foreach (string text in files)
		{
			if (text.EndsWith(".pck"))
			{
				Log.Info("Found mod pck file " + dirAccess.GetCurrentDir(true) + "/" + text);
				TryLoadModFromPck(text, dirAccess, source);
			}
		}
		string[] directories = dirAccess.GetDirectories();
		foreach (string path in directories)
		{
			DirAccess val = DirAccess.Open(Path.Combine(dirAccess.GetCurrentDir(true), path));
			try
			{
				if (val != null)
				{
					LoadModsInDirRecursive(val, source);
				}
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
		}
	}

	private static void InitializeSteamMods()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		uint numSubscribedItems = SteamUGC.GetNumSubscribedItems();
		PublishedFileId_t[] array = (PublishedFileId_t[])(object)new PublishedFileId_t[numSubscribedItems];
		numSubscribedItems = SteamUGC.GetSubscribedItems(array, numSubscribedItems);
		for (int i = 0; i < numSubscribedItems; i++)
		{
			PublishedFileId_t workshopItemId = array[i];
			TryLoadModFromSteam(workshopItemId);
		}
		_steamItemInstalledCallback = Callback<ItemInstalled_t>.Create((DispatchDelegate<ItemInstalled_t>)OnSteamWorkshopItemInstalled);
	}

	private static void TryLoadModFromSteam(PublishedFileId_t workshopItemId)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		ulong value = default(ulong);
		string text = default(string);
		uint value2 = default(uint);
		if (!SteamUGC.GetItemInstallInfo(workshopItemId, ref value, ref text, 256u, ref value2))
		{
			Log.Warn($"Could not get Steam Workshop item install info for item {workshopItemId.m_PublishedFileId}");
			return;
		}
		Log.Info($"Looking for mods to load from Steam Workshop mod {workshopItemId.m_PublishedFileId} in {text} (size {value}, last modified {value2})");
		DirAccess val = DirAccess.Open(text);
		try
		{
			if (val == null)
			{
				Log.Warn("Could not open Steam Workshop folder: " + text);
			}
			else
			{
				LoadModsInDirRecursive(val, ModSource.SteamWorkshop);
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private static void OnSteamWorkshopItemInstalled(ItemInstalled_t ev)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		if ((ulong)ev.m_unAppID.m_AppId == 2868840)
		{
			Log.Info($"Detected new Steam Workshop item installation, id: {ev.m_nPublishedFileId.m_PublishedFileId}");
			TryLoadModFromSteam(ev.m_nPublishedFileId);
		}
	}

	private static void TryLoadModFromPck(string pckFilename, DirAccess dirAccess, ModSource source)
	{
		//IL_03bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c6: Expected O, but got Unknown
		Assembly assembly = null;
		string pckName = Path.GetFileNameWithoutExtension(pckFilename);
		bool flag = SaveManager.Instance.SettingsSave.ModSettings?.IsModDisabled(pckName, source) ?? false;
		bool flag2 = _mods.Any((Mod m) => m.manifest?.pckName == pckName);
		if (!PlayerAgreedToModLoading || flag || flag2 || _initialized)
		{
			if (_initialized)
			{
				Log.Info("Skipping loading mod " + pckFilename + ", can't load mods at runtime");
			}
			else if (flag)
			{
				Log.Info("Skipping loading mod " + pckFilename + ", it is set to disabled in settings");
			}
			else if (!PlayerAgreedToModLoading)
			{
				Log.Info("Skipping loading mod " + pckFilename + ", user has not yet seen the mods warning");
			}
			else if (flag2)
			{
				Log.Warn("Tried to load mod with PCK name " + pckName + ", but a mod is already loaded with that name!");
			}
			Mod mod = new Mod
			{
				pckName = pckName,
				modSource = source,
				manifest = null,
				wasLoaded = false
			};
			_mods.Add(mod);
			ModManager.OnModDetected?.Invoke(mod);
			return;
		}
		try
		{
			if (!File.Exists(Path.Combine(dirAccess.GetCurrentDir(true), pckFilename)))
			{
				throw new InvalidOperationException("PCK not found at path " + pckFilename + "!");
			}
			string text = pckName + ".dll";
			if (dirAccess.FileExists(text))
			{
				Log.Info("Loading assembly DLL " + text);
				AssemblyLoadContext loadContext = AssemblyLoadContext.GetLoadContext(Assembly.GetExecutingAssembly());
				if (loadContext != null)
				{
					assembly = loadContext.LoadFromAssemblyPath(Path.Combine(dirAccess.GetCurrentDir(true), text));
				}
			}
			if (!ProjectSettings.LoadResourcePack(Path.Combine(dirAccess.GetCurrentDir(true), pckFilename), true, 0))
			{
				throw new InvalidOperationException("Godot errored while loading PCK file " + pckName + "!");
			}
			if (!ResourceLoader.Exists("res://mod_manifest.json", ""))
			{
				throw new InvalidOperationException(pckFilename + " did not supply a mod manifest!");
			}
			using FileAccessStream utf8Json = new FileAccessStream("res://mod_manifest.json", (ModeFlags)1);
			ModManifest modManifest = JsonSerializer.Deserialize(utf8Json, JsonSerializationUtility.GetTypeInfo<ModManifest>());
			if (modManifest == null)
			{
				throw new InvalidOperationException("JSON deserialization returned null when trying to deserialize mod manifest!");
			}
			if (!string.Equals(modManifest.pckName, pckName, StringComparison.OrdinalIgnoreCase))
			{
				throw new InvalidOperationException($"PCK name in mod manifest {modManifest.pckName} does not match the pck {pckName} we're currently loading!");
			}
			bool? assemblyLoadedSuccessfully = null;
			if (assembly != null)
			{
				assemblyLoadedSuccessfully = true;
				List<Type> list = (from t in assembly.GetTypes()
					where t.GetCustomAttribute<ModInitializerAttribute>() != null
					select t).ToList();
				if (list.Count > 0)
				{
					foreach (Type item in list)
					{
						Log.Info($"Calling initializer method of type {item} for {assembly}");
						bool flag3 = CallModInitializer(item);
						assemblyLoadedSuccessfully = assemblyLoadedSuccessfully.Value && flag3;
					}
				}
				else
				{
					try
					{
						Log.Info($"No ModInitializerAttribute detected. Calling Harmony.PatchAll for {assembly}");
						Harmony val = new Harmony((modManifest.author ?? "unknown") + "." + pckName);
						val.PatchAll(assembly);
					}
					catch (Exception value)
					{
						Log.Error($"Exception caught while trying to run PatchAll on assembly {assembly}:\n{value}");
						assemblyLoadedSuccessfully = false;
					}
				}
			}
			Log.Info($"Finished mod initialization for '{modManifest.name}' ({modManifest.pckName}).");
			Mod mod2 = new Mod
			{
				pckName = pckName,
				modSource = source,
				manifest = modManifest,
				wasLoaded = true,
				assembly = assembly,
				assemblyLoadedSuccessfully = assemblyLoadedSuccessfully
			};
			_mods.Add(mod2);
			ModManager.OnModDetected?.Invoke(mod2);
		}
		catch (Exception value2)
		{
			Log.Error($"Error loading mod {pckFilename}: {value2}");
		}
	}

	private static bool CallModInitializer(Type initializerType)
	{
		ModInitializerAttribute customAttribute = initializerType.GetCustomAttribute<ModInitializerAttribute>();
		MethodInfo method = initializerType.GetMethod(customAttribute.initializerMethod, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
		if (method == null)
		{
			method = initializerType.GetMethod(customAttribute.initializerMethod, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (method != null)
			{
				Log.Error($"Tried to call mod initializer {initializerType.Name}.{customAttribute.initializerMethod} but it's not static! Declare it to be static");
			}
			else
			{
				Log.Error($"Found mod initializer class of type {initializerType}, but it does not contain the method {customAttribute.initializerMethod} declared in the ModInitializerAttribute!");
			}
			return false;
		}
		try
		{
			Log.Info($"Calling initializer method {initializerType.Name}.{customAttribute.initializerMethod}...");
			method.Invoke(null, null);
		}
		catch (Exception value)
		{
			Log.Error($"Exception thrown when calling mod initializer of type {initializerType}: {value}");
			return false;
		}
		return true;
	}

	public static IEnumerable<string> GetModdedLocTables(string language, string file)
	{
		foreach (Mod mod in _mods)
		{
			if (mod.wasLoaded)
			{
				string text = $"res://{mod.manifest.pckName}/localization/{language}/{file}";
				if (ResourceLoader.Exists(text, ""))
				{
					yield return text;
				}
			}
		}
	}

	public static List<string>? GetModNameList()
	{
		if (LoadedMods.Count == 0)
		{
			return null;
		}
		return LoadedMods.Select((Mod m) => m.manifest.pckName + m.manifest.version).ToList();
	}

	private static Assembly HandleAssemblyResolveFailure(object? source, ResolveEventArgs ev)
	{
		if (ev.Name.StartsWith("sts2,"))
		{
			Log.Info($"Failed to resolve assembly '{ev.Name}' but it looks like the STS2 assembly. Resolving using {Assembly.GetExecutingAssembly()}");
			return Assembly.GetExecutingAssembly();
		}
		if (ev.Name.StartsWith("0Harmony,"))
		{
			Log.Info($"Failed to resolve assembly '{ev.Name}' but it looks like the Harmony assembly. Resolving using {typeof(Harmony).Assembly}");
			return typeof(Harmony).Assembly;
		}
		return null;
	}

	public static void CallMetricsHooks(SerializableRun run, bool isVictory, ulong localPlayerId)
	{
		ModManager.OnMetricsUpload?.Invoke(run, isVictory, localPlayerId);
	}

	public static void Dispose()
	{
		_steamItemInstalledCallback?.Dispose();
	}
}
