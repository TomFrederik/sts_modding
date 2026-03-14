using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Godot;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Logging;
using Steamworks;

namespace MegaCrit.Sts2.Core.Platform.Steam;

public static class SteamInitializer
{
	public const ulong steamAppId = 2868840uL;

	public static bool Initialized { get; private set; }

	public static ESteamAPIInitResult? InitResult { get; private set; }

	public static string? InitErrorMessage { get; private set; }

	private static nint SteamDebugResolver(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
	{
		if (libraryName != "steam_api" && libraryName != "steam_api64")
		{
			return IntPtr.Zero;
		}
		if (OS.HasFeature("editor"))
		{
			string text;
			switch (OS.GetName())
			{
			case "Windows":
				text = "steam/SteamApi/steam_api64.dll";
				break;
			case "macOS":
				text = "steam/SteamApi/libsteam_api.dylib";
				break;
			case "FreeBSD":
			case "OpenBSD":
			case "Linux":
			case "NetBSD":
			case "BSD":
				text = "steam/SteamApi/libsteam_api.so";
				break;
			default:
				text = null;
				break;
			}
			string text2 = text;
			if (text2 != null)
			{
				return NativeLibrary.Load(text2);
			}
		}
		return IntPtr.Zero;
	}

	public static bool Initialize(Node node)
	{
		NativeLibrary.SetDllImportResolver(Assembly.GetAssembly(typeof(SteamAPI)), SteamDebugResolver);
		Log.Info("Steamworks: attempting initialization...");
		for (int i = 0; i < 3; i++)
		{
			if (i > 0)
			{
				Thread.Sleep(100);
				Log.Info($"Steam initialization retry attempt {i}");
			}
			Initialized = InitializeInternal();
			if (Initialized)
			{
				break;
			}
		}
		if (Initialized)
		{
			SteamNetworkingUtils.InitRelayNetworkAccess();
			TaskHelper.RunSafely(RunCallbacksAsync(node));
		}
		return Initialized;
	}

	private static bool InitializeInternal()
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			Log.Info($"Steam is running: {SteamAPI.IsSteamRunning()}");
			string initErrorMessage = default(string);
			InitResult = SteamAPI.InitEx(ref initErrorMessage);
			InitErrorMessage = initErrorMessage;
			if (InitResult == (ESteamAPIInitResult?)0)
			{
				Log.Info("Steamworks initialization succeeded!");
				return true;
			}
			Log.Error($"Steamworks initialization failed! Result: {InitResult}, message: {InitErrorMessage}");
		}
		catch (Exception ex)
		{
			Log.Error($"Steamworks initialization threw an exception: {ex}");
			InitResult = null;
			InitErrorMessage = ex.Message;
		}
		return false;
	}

	private static async Task RunCallbacksAsync(Node node)
	{
		while (Initialized)
		{
			await ((GodotObject)node).ToSignal((GodotObject)(object)node.GetTree(), SignalName.ProcessFrame);
			if (!SteamAPI.IsSteamRunning())
			{
				Log.Warn("Steam is no longer running. Stopping callbacks.");
				Initialized = false;
				break;
			}
			SteamAPI.RunCallbacks();
		}
	}

	public static void Uninitialize()
	{
		if (!Initialized)
		{
			return;
		}
		Log.Info("Steamworks: shutting down...");
		try
		{
			SteamAPI.Shutdown();
			Log.Info("Steamworks shutdown succeeded!");
		}
		catch (Exception value)
		{
			Log.Error($"Steamworks shutdown threw an exception: {value}");
		}
		finally
		{
			Initialized = false;
		}
	}
}
