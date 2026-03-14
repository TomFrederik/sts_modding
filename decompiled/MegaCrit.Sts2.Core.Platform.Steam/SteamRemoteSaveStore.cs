using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Multiplayer.Transport.Steam;
using MegaCrit.Sts2.Core.Saves;
using Steamworks;

namespace MegaCrit.Sts2.Core.Platform.Steam;

public class SteamRemoteSaveStore : ICloudSaveStore, ISaveStore
{
	public string ReadFile(string path)
	{
		path = CanonicalizePath(path);
		int fileSize = SteamRemoteStorage.GetFileSize(path);
		if (fileSize == 0)
		{
			if (!SteamRemoteStorage.FileExists(path))
			{
				throw new FileNotFoundException("Steam remote storage file not found: " + path);
			}
			return string.Empty;
		}
		byte[] array = new byte[fileSize];
		int num = SteamRemoteStorage.FileRead(path, array, fileSize);
		if (num == 0)
		{
			throw new InvalidOperationException($"Steam remote storage read failed for {path}. Expected {fileSize} bytes, got 0. Steam storage may be corrupted or unavailable.");
		}
		if (num != fileSize)
		{
			Log.Warn($"Steam read returned {num} bytes but expected {fileSize} for {path}. Using partial data.");
		}
		return Encoding.UTF8.GetString(array);
	}

	public async Task<string?> ReadFileAsync(string path)
	{
		path = CanonicalizePath(path);
		int byteCount = SteamRemoteStorage.GetFileSize(path);
		if (byteCount == 0)
		{
			return string.Empty;
		}
		SteamAPICall_t val = SteamRemoteStorage.FileReadAsync(path, 0u, (uint)byteCount);
		if (val == SteamAPICall_t.Invalid)
		{
			throw new InvalidOperationException($"Steam remote storage async read request returned invalid API call for path {path} (bytes {byteCount})");
		}
		using SteamCallResult<RemoteStorageFileReadAsyncComplete_t> callResult = new SteamCallResult<RemoteStorageFileReadAsyncComplete_t>(val);
		RemoteStorageFileReadAsyncComplete_t val2 = await callResult.Task;
		if ((int)val2.m_eResult != 1)
		{
			throw new InvalidOperationException($"Steam remote storage async read request returned error {val2.m_eResult} for path {path}");
		}
		byte[] array = new byte[byteCount];
		SteamRemoteStorage.FileReadAsyncComplete(val2.m_hFileReadAsync, array, val2.m_cubRead);
		return Encoding.UTF8.GetString(array);
	}

	public void WriteFile(string path, string content)
	{
		WriteFile(path, Encoding.UTF8.GetBytes(content));
	}

	public void WriteFile(string path, byte[] bytes)
	{
		path = CanonicalizePath(path);
		if (!SteamRemoteStorage.FileWrite(path, bytes, bytes.Length))
		{
			throw new InvalidOperationException("Steam Cloud write failed. See ISteamRemoteStorage documentation for possible reasons.");
		}
		Log.Info($"Wrote {bytes.Length} bytes to {path} in steam remote store");
	}

	public Task WriteFileAsync(string path, string content)
	{
		return WriteFileAsync(path, Encoding.UTF8.GetBytes(content));
	}

	public async Task WriteFileAsync(string path, byte[] bytes)
	{
		path = CanonicalizePath(path);
		SteamAPICall_t val = SteamRemoteStorage.FileWriteAsync(path, bytes, (uint)bytes.Length);
		if (val == SteamAPICall_t.Invalid)
		{
			throw new InvalidOperationException($"Steam remote storage async write request returned invalid API call for path {path} ({bytes.Length} bytes)");
		}
		using SteamCallResult<RemoteStorageFileWriteAsyncComplete_t> callResult = new SteamCallResult<RemoteStorageFileWriteAsyncComplete_t>(val);
		RemoteStorageFileWriteAsyncComplete_t val2 = await callResult.Task;
		if ((int)val2.m_eResult != 1)
		{
			throw new InvalidOperationException($"Steam remote storage async write request returned error {val2.m_eResult}");
		}
		Log.Info($"Wrote {bytes.Length} bytes to {path} in steam remote store");
	}

	public bool FileExists(string path)
	{
		return SteamRemoteStorage.FileExists(CanonicalizePath(path));
	}

	public bool DirectoryExists(string path)
	{
		return true;
	}

	public void DeleteFile(string path)
	{
		path = CanonicalizePath(path);
		bool flag = SteamRemoteStorage.FileExists(path);
		bool flag2 = SteamRemoteStorage.FileDelete(path);
		if (!flag2 && flag)
		{
			Log.Error("Steam remote storage delete FAILED for " + path + ". File existed but could not be deleted.");
		}
		else if (!flag2 && !flag)
		{
			Log.Debug("Steam delete called on non-existent file " + path + " (no-op)");
		}
		else
		{
			Log.Debug("Deleted " + path + " from Steam remote storage");
		}
	}

	public void RenameFile(string sourcePath, string destinationPath)
	{
		sourcePath = CanonicalizePath(sourcePath);
		destinationPath = CanonicalizePath(destinationPath);
		string content = ReadFile(sourcePath);
		WriteFile(destinationPath, content);
		DeleteFile(sourcePath);
	}

	public string[] GetFilesInDirectory(string directoryPath)
	{
		directoryPath = CanonicalizePath(directoryPath);
		int fileCount = SteamRemoteStorage.GetFileCount();
		List<string> list = new List<string>();
		int num = default(int);
		for (int i = 0; i < fileCount; i++)
		{
			string fileNameAndSize = SteamRemoteStorage.GetFileNameAndSize(i, ref num);
			if (fileNameAndSize.StartsWith(directoryPath))
			{
				string text = fileNameAndSize.Substring(directoryPath.Length + 1);
				if (!text.Contains('/') && !text.Contains('\\'))
				{
					list.Add(text);
				}
			}
		}
		return list.ToArray();
	}

	public string[] GetDirectoriesInDirectory(string directoryPath)
	{
		throw new NotImplementedException();
	}

	public void CreateDirectory(string directoryPath)
	{
	}

	public void DeleteDirectory(string directoryPath)
	{
	}

	public void DeleteTemporaryFiles(string directoryPath)
	{
	}

	public string GetFullPath(string filename)
	{
		throw new NotImplementedException();
	}

	public DateTimeOffset GetLastModifiedTime(string path)
	{
		path = CanonicalizePath(path);
		long fileTimestamp = SteamRemoteStorage.GetFileTimestamp(path);
		return DateTimeOffset.FromUnixTimeSeconds(fileTimestamp);
	}

	public int GetFileSize(string path)
	{
		path = CanonicalizePath(path);
		return SteamRemoteStorage.GetFileSize(path);
	}

	public void SetLastModifiedTime(string path, DateTimeOffset time)
	{
		throw new NotImplementedException();
	}

	public string CanonicalizePath(string path)
	{
		return path.Replace("user://", "").Replace("\\", "/");
	}

	public bool HasCloudFiles()
	{
		return SteamRemoteStorage.GetFileCount() > 0;
	}

	public void ForgetFile(string path)
	{
		path = CanonicalizePath(path);
		if (!SteamRemoteStorage.FileForget(path))
		{
			throw new InvalidOperationException($"Tried to forget file at path {path} from steam storage, but false was returned from {"FileForget"}!");
		}
	}

	public bool IsFilePersisted(string path)
	{
		path = CanonicalizePath(path);
		return SteamRemoteStorage.FilePersisted(path);
	}

	public void BeginSaveBatch()
	{
		if (!SteamRemoteStorage.BeginFileWriteBatch())
		{
			Log.Warn("SteamRemoteStorage.BeginFileWriteBatch returned false (a batch may already be in progress)");
		}
	}

	public void EndSaveBatch()
	{
		if (!SteamRemoteStorage.EndFileWriteBatch())
		{
			Log.Warn("SteamRemoteStorage.EndFileWriteBatch returned false (no batch was in progress)");
		}
	}
}
