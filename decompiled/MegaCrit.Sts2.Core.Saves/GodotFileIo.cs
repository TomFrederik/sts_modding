using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Godot;
using MegaCrit.Sts2.Core.Exceptions;
using MegaCrit.Sts2.Core.Logging;

namespace MegaCrit.Sts2.Core.Saves;

public class GodotFileIo : ISaveStore
{
	public string SaveDir { get; set; }

	public GodotFileIo(string saveDir)
	{
		SaveDir = saveDir;
		CreateDirectory(SaveDir);
	}

	public string GetFullPath(string filename)
	{
		if (filename.StartsWith(SaveDir))
		{
			return filename;
		}
		return SaveDir + "/" + filename;
	}

	public string? ReadFile(string path)
	{
		path = GetFullPath(path);
		FileAccess val = FileAccess.Open(path, (ModeFlags)1);
		try
		{
			if (val == null)
			{
				return null;
			}
			string asText = val.GetAsText(false);
			val.Close();
			return asText;
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public async Task<string?> ReadFileAsync(string path)
	{
		path = GetFullPath(path);
		ValidateGodotFilePath(path);
		string result;
		await using (FileAccessStream stream = new FileAccessStream(path, (ModeFlags)1))
		{
			using MemoryStream memoryStream = new MemoryStream();
			await stream.CopyToAsync(memoryStream);
			result = Encoding.UTF8.GetString(memoryStream.ToArray());
		}
		return result;
	}

	public DateTimeOffset GetLastModifiedTime(string path)
	{
		path = GetFullPath(path);
		return DateTimeOffset.FromUnixTimeSeconds((long)FileAccess.GetModifiedTime(path));
	}

	public int GetFileSize(string path)
	{
		path = GetFullPath(path);
		FileAccess val = FileAccess.Open(path, (ModeFlags)1);
		try
		{
			if (val == null)
			{
				return 0;
			}
			return (int)val.GetLength();
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public void SetLastModifiedTime(string path, DateTimeOffset time)
	{
		path = GetFullPath(path);
		File.SetLastWriteTimeUtc(ProjectSettings.GlobalizePath(path), time.UtcDateTime);
	}

	public void WriteFile(string path, string content)
	{
		if (string.IsNullOrWhiteSpace(content))
		{
			Log.Error("The content is empty for path='" + path + "'");
		}
		else
		{
			WriteFile(path, Encoding.UTF8.GetBytes(content));
		}
	}

	public void WriteFile(string path, byte[] bytes)
	{
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		path = GetFullPath(path);
		ValidateGodotFilePath(path);
		RotateBackup(path);
		string text = path + ".tmp";
		FileAccess val = FileAccess.Open(text, (ModeFlags)2);
		try
		{
			if (val == null)
			{
				throw new SaveException($"Failed to open file for writing. path='{text}' error={FileAccess.GetOpenError()}");
			}
			val.StoreBuffer(bytes);
			val.Close();
			RenameFile(text, path);
			Log.Info($"Wrote {bytes.Length} bytes to path={path} save_dir={SaveDir}");
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public Task WriteFileAsync(string path, string content)
	{
		return WriteFileAsync(path, Encoding.UTF8.GetBytes(content));
	}

	public async Task WriteFileAsync(string path, byte[] bytes)
	{
		path = GetFullPath(path);
		ValidateGodotFilePath(path);
		RotateBackup(path);
		string tempPath = path + ".tmp";
		await using FileAccessStream stream = new FileAccessStream(tempPath, (ModeFlags)2);
		await stream.WriteAsync(bytes);
		long position = stream.Position;
		stream.Close();
		RenameFile(tempPath, path);
		Log.Info($"Wrote {position} bytes to path={path} save_dir={path}");
	}

	public bool FileExists(string path)
	{
		return FileAccess.FileExists(GetFullPath(path));
	}

	public bool DirectoryExists(string path)
	{
		return DirAccess.DirExistsAbsolute(GetFullPath(path));
	}

	public void DeleteFile(string path)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		DirAccess.RemoveAbsolute(GetFullPath(path));
	}

	public void RenameFile(string sourcePath, string destinationPath)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		if (!FileExists(sourcePath))
		{
			throw new SaveException("Cannot rename file: source does not exist. source=" + GetFullPath(sourcePath));
		}
		sourcePath = GetFullPath(sourcePath);
		destinationPath = GetFullPath(destinationPath);
		Error val = DirAccess.RenameAbsolute(sourcePath, destinationPath);
		if ((int)val != 0)
		{
			throw new SaveException($"Failed to rename file. error={val} source={sourcePath} destination={destinationPath}");
		}
	}

	public string[] GetFilesInDirectory(string directoryPath)
	{
		directoryPath = GetFullPath(directoryPath);
		return DirAccess.GetFilesAt(directoryPath);
	}

	public string[] GetDirectoriesInDirectory(string directoryPath)
	{
		directoryPath = GetFullPath(directoryPath);
		return DirAccess.GetDirectoriesAt(directoryPath);
	}

	public void CreateDirectory(string directoryPath)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		directoryPath = GetFullPath(directoryPath);
		if (!DirAccess.DirExistsAbsolute(directoryPath))
		{
			DirAccess.MakeDirRecursiveAbsolute(directoryPath);
		}
	}

	public void DeleteDirectory(string directoryPath)
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		directoryPath = GetFullPath(directoryPath);
		if (!DirAccess.DirExistsAbsolute(directoryPath))
		{
			return;
		}
		DirAccess val = DirAccess.Open(directoryPath);
		try
		{
			val.IncludeHidden = true;
			string[] files = val.GetFiles();
			foreach (string text in files)
			{
				val.Remove(text);
			}
			string[] directories = val.GetDirectories();
			foreach (string text2 in directories)
			{
				DeleteDirectory(directoryPath + "/" + text2);
			}
			Error val2 = val.Remove("");
			if ((int)val2 != 0)
			{
				throw new InvalidOperationException($"Got error {val2} trying to delete directory {directoryPath}");
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public void DeleteTemporaryFiles(string directoryPath)
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		directoryPath = GetFullPath(directoryPath);
		DirAccess val = DirAccess.Open(directoryPath);
		try
		{
			if (val == null)
			{
				return;
			}
			string[] files = val.GetFiles();
			foreach (string text in files)
			{
				if (text.EndsWith(".tmp"))
				{
					Log.Info("Cleaned up orphaned " + text + " in " + directoryPath);
					val.Remove(text);
				}
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private static void RotateBackup(string fullPath)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		string text = fullPath + ".backup";
		if (FileAccess.FileExists(text))
		{
			DirAccess.RemoveAbsolute(text);
		}
		if (FileAccess.FileExists(fullPath))
		{
			Error val = DirAccess.RenameAbsolute(fullPath, text);
			if ((int)val != 0)
			{
				Log.Warn($"Failed to rotate backup. error={val} source={fullPath} backup={text}");
			}
		}
	}

	private static void ValidateGodotFilePath(string godotFilePath)
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		if (!godotFilePath.Contains("://"))
		{
			throw new SaveException("The path='" + godotFilePath + "' is not a godot file path");
		}
		string baseDir = StringExtensions.GetBaseDir(godotFilePath);
		if (!DirAccess.DirExistsAbsolute(baseDir))
		{
			DirAccess.MakeDirRecursiveAbsolute(baseDir);
		}
	}
}
