using System;
using System.IO;
using Godot;

namespace MegaCrit.Sts2.Core.Saves;

public class FileAccessStream : Stream
{
	private readonly FileAccess _file;

	private readonly ModeFlags _flags;

	public override bool CanRead
	{
		get
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			if (_file.IsOpen())
			{
				return ((Enum)_flags).HasFlag((Enum)(object)(ModeFlags)1);
			}
			return false;
		}
	}

	public override bool CanSeek => _file.IsOpen();

	public override bool CanWrite
	{
		get
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			if (_file.IsOpen())
			{
				return ((Enum)_flags).HasFlag((Enum)(object)(ModeFlags)2);
			}
			return false;
		}
	}

	public override long Length => (long)_file.GetLength();

	public override long Position
	{
		get
		{
			return (long)_file.GetPosition();
		}
		set
		{
			_file.Seek((ulong)value);
		}
	}

	public FileAccessStream(string filePath, ModeFlags flags)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		_flags = flags;
		FileAccess val = FileAccess.Open(filePath, flags);
		_file = val ?? throw new IOException($"Opening file {filePath} with flags {flags} failed: {FileAccess.GetOpenError()}");
	}

	public override void Flush()
	{
		_file.Flush();
	}

	public override int Read(byte[] buffer, int offset, int count)
	{
		byte[] buffer2 = _file.GetBuffer((long)count);
		int num = Math.Min(count, buffer2.Length);
		Array.Copy(buffer2, 0, buffer, offset, num);
		return num;
	}

	public override void Write(byte[] buffer, int offset, int count)
	{
		int num = Math.Min(buffer.Length - offset, count);
		if (offset == 0 && buffer.Length <= count)
		{
			_file.StoreBuffer(buffer);
			return;
		}
		byte[] array = new byte[num];
		Array.Copy(buffer, offset, array, 0, num);
		_file.StoreBuffer(array);
	}

	public override long Seek(long offset, SeekOrigin origin)
	{
		switch (origin)
		{
		case SeekOrigin.Begin:
			_file.Seek((ulong)offset);
			break;
		case SeekOrigin.Current:
			_file.Seek(_file.GetPosition() + (ulong)offset);
			break;
		case SeekOrigin.End:
			_file.Seek(_file.GetLength() - (ulong)offset);
			break;
		}
		return (long)_file.GetPosition();
	}

	public override void SetLength(long value)
	{
		throw new NotImplementedException();
	}

	protected override void Dispose(bool disposing)
	{
		_file.Close();
	}
}
