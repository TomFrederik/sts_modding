using System;
using System.IO;
using System.Text.Json;
using Godot;
using MegaCrit.Sts2.Core.Logging;

namespace MegaCrit.Sts2.Core.Debug;

public class ReleaseInfoManager
{
	private static ReleaseInfoManager? _instance;

	private const string _releaseInfoFileName = "release_info.json";

	public static ReleaseInfoManager Instance => _instance ?? (_instance = new ReleaseInfoManager());

	public ReleaseInfo? ReleaseInfo { get; }

	private ReleaseInfoManager()
	{
		ReleaseInfo = LoadConfig();
	}

	private ReleaseInfo? LoadConfig()
	{
		string[] possibleReleaseInfoPaths = GetPossibleReleaseInfoPaths();
		string[] array = possibleReleaseInfoPaths;
		foreach (string text in array)
		{
			string text2 = ProjectSettings.GlobalizePath(text);
			if (!FileAccess.FileExists(text2))
			{
				continue;
			}
			Log.Info("Found release_info.json at: " + text2);
			FileAccess val = FileAccess.Open(text2, (ModeFlags)1);
			try
			{
				if (val == null)
				{
					Log.Error("Failed to open file: " + text2);
					continue;
				}
				try
				{
					string asText = val.GetAsText(false);
					return JsonSerializer.Deserialize(asText, ReleaseInfoJsonSerializerContext.Default.ReleaseInfo);
				}
				catch (JsonException ex)
				{
					Log.Error("Failed to deserialize release_info.json: " + ex.Message);
				}
				catch (Exception ex2)
				{
					Log.Error("Unexpected error reading release_info.json: " + ex2.Message);
				}
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
		}
		Log.Info("File `release_info.json` not found in any of the expected locations.");
		return null;
	}

	private static string[] GetPossibleReleaseInfoPaths()
	{
		string executablePath = OS.GetExecutablePath();
		string path = Path.GetDirectoryName(executablePath) ?? string.Empty;
		if (OS.GetName() == "macOS")
		{
			string path2 = Path.Combine(path, "..", "Resources");
			return new string[2]
			{
				Path.Combine(path2, "release_info.json"),
				Path.Combine(path, "release_info.json")
			};
		}
		return new string[1] { Path.Combine(path, "release_info.json") };
	}
}
