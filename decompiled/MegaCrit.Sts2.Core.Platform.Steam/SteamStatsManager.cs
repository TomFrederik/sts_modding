using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Multiplayer.Transport.Steam;
using Steamworks;

namespace MegaCrit.Sts2.Core.Platform.Steam;

public static class SteamStatsManager
{
	private const string ArchitectDamageStat = "architect_damage";

	private static Callback<UserStatsReceived_t>? _userStatsReceivedCallback;

	private static bool _userStatsReady;

	private static bool _globalStatsReady;

	private static long _globalArchitectDamage;

	public static bool IsGlobalStatsReady => _globalStatsReady;

	public static void Initialize()
	{
		if (SteamInitializer.Initialized)
		{
			_userStatsReady = false;
			_globalStatsReady = false;
			_globalArchitectDamage = 0L;
			_userStatsReceivedCallback = Callback<UserStatsReceived_t>.Create((DispatchDelegate<UserStatsReceived_t>)OnUserStatsReceived);
			SteamUserStats.RequestCurrentStats();
			TaskHelper.RunSafely(RefreshGlobalStats());
		}
	}

	public static async Task RefreshGlobalStats()
	{
		if (!SteamInitializer.Initialized)
		{
			return;
		}
		SteamAPICall_t call = SteamUserStats.RequestGlobalStats(0);
		using SteamCallResult<GlobalStatsReceived_t> callResult = new SteamCallResult<GlobalStatsReceived_t>(call);
		try
		{
			OnGlobalStatsReceived(await callResult.Task);
		}
		catch
		{
			Log.Warn("SteamStatsManager: Failed to receive global stats");
		}
	}

	public static void IncrementArchitectDamage(int score)
	{
		if (!_userStatsReady)
		{
			Log.Warn("SteamStatsManager: Cannot increment architect damage, user stats not ready");
			return;
		}
		int num = default(int);
		bool stat = SteamUserStats.GetStat("architect_damage", ref num);
		bool value = SteamUserStats.SetStat("architect_damage", num + score);
		bool value2 = SteamUserStats.StoreStats();
		Log.Info($"SteamStatsManager: IncrementArchitectDamage by {score} (was {num}, now {num + score}) [get={stat}, set={value}, store={value2}]");
	}

	public static long GetGlobalArchitectDamage()
	{
		return _globalArchitectDamage;
	}

	private static void OnUserStatsReceived(UserStatsReceived_t result)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Invalid comparison between Unknown and I4
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		if (result.m_nGameID == 2868840)
		{
			if ((int)result.m_eResult == 1)
			{
				_userStatsReady = true;
				Log.Info("SteamStatsManager: User stats received");
			}
			else
			{
				Log.Warn($"SteamStatsManager: User stats request failed with result {result.m_eResult}");
			}
		}
	}

	private static void OnGlobalStatsReceived(GlobalStatsReceived_t result)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Invalid comparison between Unknown and I4
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		if (result.m_nGameID == 2868840)
		{
			if ((int)result.m_eResult == 1)
			{
				long globalArchitectDamage = default(long);
				bool globalStat = SteamUserStats.GetGlobalStat("architect_damage", ref globalArchitectDamage);
				_globalArchitectDamage = globalArchitectDamage;
				_globalStatsReady = true;
				Log.Info($"SteamStatsManager: Global stats received (found={globalStat}), architect damage = {_globalArchitectDamage}");
			}
			else
			{
				Log.Warn($"SteamStatsManager: Global stats request failed with result {result.m_eResult}");
			}
		}
	}
}
