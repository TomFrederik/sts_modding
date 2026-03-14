using System;
using System.Runtime.InteropServices;
using MegaCrit.Sts2.Core.Logging;
using Steamworks;

namespace MegaCrit.Sts2.Core.Multiplayer.Transport.Steam;

public static class SteamUtil
{
	public const uint handshakeMagicBytes = 2204332656u;

	private static readonly nint[] _messageBuffer = new nint[64];

	public static NetTransferMode ModeFromFlags(int flags)
	{
		if ((flags & 8) > 0)
		{
			return NetTransferMode.Reliable;
		}
		if (flags == 0)
		{
			return NetTransferMode.Unreliable;
		}
		throw new ArgumentOutOfRangeException();
	}

	public static int FlagsFromMode(NetTransferMode mode)
	{
		return mode switch
		{
			NetTransferMode.Unreliable => 0, 
			NetTransferMode.Reliable => 8, 
			_ => throw new ArgumentOutOfRangeException("mode", mode, null), 
		};
	}

	public static SteamNetworkingIdentity ToNetId(this CSteamID id)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		SteamNetworkingIdentity result = default(SteamNetworkingIdentity);
		((SteamNetworkingIdentity)(ref result)).SetSteamID(id);
		return result;
	}

	public static SteamNetworkingIdentity ToNetId64(this ulong id)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		SteamNetworkingIdentity result = default(SteamNetworkingIdentity);
		((SteamNetworkingIdentity)(ref result)).SetSteamID64(id);
		return result;
	}

	public static void ProcessMessages(HSteamNetConnection conn, INetHandler handler, Logger logger)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		int num;
		do
		{
			num = SteamNetworkingSockets.ReceiveMessagesOnConnection(conn, (IntPtr[])_messageBuffer, _messageBuffer.Length);
			if (num > 0)
			{
				logger.VeryDebug($"Processing {num} packets");
			}
			for (int i = 0; i < num; i++)
			{
				nint num2 = _messageBuffer[i];
				SteamNetworkingMessage_t val = Marshal.PtrToStructure<SteamNetworkingMessage_t>(num2);
				byte[] array = new byte[val.m_cbSize];
				Marshal.Copy(val.m_pData, array, 0, val.m_cbSize);
				NetTransferMode netTransferMode = ModeFromFlags(val.m_nFlags);
				logger.VeryDebug($"Received packet of size {array.Length} from sender {((SteamNetworkingIdentity)(ref val.m_identityPeer)).GetSteamID64()} ({netTransferMode}, {val.m_nChannel})");
				handler.OnPacketReceived(((SteamNetworkingIdentity)(ref val.m_identityPeer)).GetSteamID().m_SteamID, array, netTransferMode, val.m_nChannel);
				SteamNetworkingMessage_t.Release((IntPtr)num2);
			}
		}
		while (num == _messageBuffer.Length);
	}
}
