using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using MegaCrit.Sts2.Core.AutoSlay;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves;
using Sentry;

namespace MegaCrit.Sts2.Core.Debug;

public static class SentryService
{
	private const string _dsnSettingPath = "sentry/config/dsn";

	private static readonly StringName _sentrySdkSingleton = new StringName("SentrySDK");

	private static readonly StringName _sentryUserClass = new StringName("SentryUser");

	private static readonly StringName _idProperty = new StringName("id");

	private static readonly StringName _setUserMethod = new StringName("set_user");

	private static IDisposable? _sentryInstance;

	public static bool IsEnabled { get; private set; }

	public static void Initialize()
	{
		if (OS.HasFeature("editor") && !DisplayServer.GetName().Equals("headless", StringComparison.OrdinalIgnoreCase))
		{
			Log.Info("[Sentry.NET] Disabled in editor");
			return;
		}
		string dsn = GetDsn();
		if (string.IsNullOrEmpty(dsn))
		{
			Log.Info("[Sentry.NET] Disabled: no DSN configured in project settings");
			return;
		}
		ReleaseInfo releaseInfo = ReleaseInfoManager.Instance.ReleaseInfo;
		string environment = ((releaseInfo != null) ? "playtesters" : "development");
		string release = releaseInfo?.Version ?? "dev";
		_sentryInstance = SentrySdk.Init((Action<SentryOptions>)delegate(SentryOptions options)
		{
			options.Dsn = dsn;
			options.Environment = environment;
			options.Release = release;
			options.Debug = false;
			options.AutoSessionTracking = true;
			options.IsGlobalModeEnabled = true;
			options.SendDefaultPii = false;
			options.SetBeforeSend((Func<SentryEvent, SentryHint, SentryEvent>)delegate(SentryEvent sentryEvent, SentryHint hint)
			{
				if (sentryEvent.Exception is AutoSlayTimeoutException)
				{
					return (SentryEvent?)null;
				}
				if (!HasUserConsent())
				{
					return (SentryEvent?)null;
				}
				return (ModManager.LoadedMods.Count > 0) ? null : sentryEvent;
			});
		});
		IsEnabled = SentrySdk.IsEnabled;
		if (!IsEnabled)
		{
			Log.Warn("[Sentry.NET] SDK initialization failed");
			return;
		}
		SentrySdk.ConfigureScope((Action<Scope>)delegate(Scope scope)
		{
			scope.SetTag("sdk", "dotnet");
			if (releaseInfo != null)
			{
				scope.SetTag("branch", releaseInfo.Branch);
				scope.SetExtra("commit", (object)releaseInfo.Commit);
				scope.SetExtra("build_date", (object)releaseInfo.Date.ToString("o"));
			}
		});
		Log.LogCallback += OnLogCallback;
		Log.Info("[Sentry.NET] Initialized: env=" + environment + ", release=" + release);
	}

	private static void OnLogCallback(LogLevel level, string message, int skipFrames)
	{
		if (IsEnabled)
		{
			switch (level)
			{
			case LogLevel.Error:
				SentrySdk.AddBreadcrumb(message, "log", (string)null, (IDictionary<string, string>)null, (BreadcrumbLevel)2);
				break;
			case LogLevel.Warn:
				SentrySdk.AddBreadcrumb(message, "log", (string)null, (IDictionary<string, string>)null, (BreadcrumbLevel)1);
				break;
			}
		}
	}

	public static void SetUserContext(string uniqueId)
	{
		if (IsEnabled)
		{
			SentrySdk.ConfigureScope((Action<Scope>)delegate(Scope scope)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_0017: Expected O, but got Unknown
				scope.User = new SentryUser
				{
					Id = uniqueId
				};
			});
			SetGdExtensionUser(uniqueId);
			Log.Debug("[Sentry.NET] User context set");
		}
	}

	private static void SetGdExtensionUser(string uniqueId)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			if (Engine.HasSingleton(_sentrySdkSingleton))
			{
				GodotObject singleton = Engine.GetSingleton(_sentrySdkSingleton);
				Variant val = ClassDB.Instantiate(_sentryUserClass);
				GodotObject val2 = ((Variant)(ref val)).AsGodotObject();
				val2.Set(_idProperty, Variant.op_Implicit(uniqueId));
				singleton.Call(_setUserMethod, (Variant[])(object)new Variant[1] { Variant.op_Implicit(val2) });
			}
		}
		catch (Exception ex)
		{
			Log.Warn("[Sentry] Failed to set GDExtension user: " + ex.Message);
		}
	}

	public static void AddBreadcrumb(string message, string category = "app", BreadcrumbLevel level = (BreadcrumbLevel)0)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		if (IsEnabled)
		{
			SentrySdk.AddBreadcrumb(message, category, (string)null, (IDictionary<string, string>)null, level);
		}
	}

	public static void CaptureException(Exception ex)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		if (IsEnabled)
		{
			SentrySdk.CaptureException(ex, (Action<Scope>)delegate(Scope scope)
			{
				AttachGameState(scope);
			});
		}
	}

	public static void CaptureMessage(string message, SentryLevel level = (SentryLevel)1)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		if (IsEnabled)
		{
			SentrySdk.CaptureMessage(message, level);
		}
	}

	public static void SetTag(string key, string value)
	{
		if (IsEnabled)
		{
			SentrySdk.ConfigureScope((Action<Scope>)delegate(Scope scope)
			{
				scope.SetTag(key, value);
			});
		}
	}

	public static void SetExtra(string key, object value)
	{
		if (IsEnabled)
		{
			SentrySdk.ConfigureScope((Action<Scope>)delegate(Scope scope)
			{
				scope.SetExtra(key, value);
			});
		}
	}

	public static void InitializeForTesting()
	{
		if (IsEnabled)
		{
			return;
		}
		string dsn = GetDsn();
		if (string.IsNullOrEmpty(dsn))
		{
			Log.Warn("[Sentry.NET] Cannot initialize for testing: no DSN configured");
			return;
		}
		_sentryInstance = SentrySdk.Init((Action<SentryOptions>)delegate(SentryOptions options)
		{
			options.Dsn = dsn;
			options.Environment = "development";
			options.Release = ReleaseInfoManager.Instance.ReleaseInfo?.Version ?? "dev-console-test";
			options.Debug = false;
			options.AutoSessionTracking = false;
			options.IsGlobalModeEnabled = true;
			options.SendDefaultPii = false;
		});
		IsEnabled = SentrySdk.IsEnabled;
		if (!IsEnabled)
		{
			Log.Warn("[Sentry.NET] SDK initialization failed for testing");
			return;
		}
		SentrySdk.ConfigureScope((Action<Scope>)delegate(Scope scope)
		{
			scope.SetTag("sdk", "dotnet");
			scope.SetTag("source", "dev-console-test");
		});
		Log.Info("[Sentry.NET] Initialized for testing via dev console");
	}

	public static void Shutdown()
	{
		if (IsEnabled)
		{
			Log.LogCallback -= OnLogCallback;
			Log.Info("[Sentry.NET] Shutting down");
			_sentryInstance?.Dispose();
			_sentryInstance = null;
			IsEnabled = false;
		}
	}

	private static string GetDsn()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		Variant setting = ProjectSettings.GetSetting("sentry/config/dsn", Variant.op_Implicit(""));
		return ((Variant)(ref setting)).AsString();
	}

	private static void AttachGameState(Scope scope)
	{
		try
		{
			string currentSceneName = GetCurrentSceneName();
			if (currentSceneName != null)
			{
				scope.SetExtra("game.scene", (object)currentSceneName);
			}
			RunState runState = RunManager.Instance.DebugOnlyGetState();
			if (RunManager.Instance.IsInProgress && runState != null)
			{
				scope.SetExtra("game.in_run", (object)true);
				scope.SetExtra("game.seed", (object)runState.Rng.StringSeed);
				scope.SetExtra("game.ascension", (object)runState.AscensionLevel);
				scope.SetExtra("game.act", (object)(runState.CurrentActIndex + 1));
				scope.SetExtra("game.act_name", (object)runState.Act.Id.ToString());
				scope.SetExtra("game.floor", (object)runState.TotalFloor);
				scope.SetExtra("game.room_type", (object)runState.CurrentRoom?.GetType().Name);
				IReadOnlyList<Player> players = runState.Players;
				if (players.Count > 0)
				{
					scope.SetExtra("game.characters", (object)string.Join(", ", players.Select((Player p) => p.Character.Id)));
					scope.SetExtra("game.player_count", (object)players.Count);
				}
			}
			else
			{
				scope.SetExtra("game.in_run", (object)false);
			}
			CombatState combatState = CombatManager.Instance.DebugOnlyGetState();
			if (combatState != null)
			{
				scope.SetExtra("combat.round", (object)combatState.RoundNumber);
				scope.SetExtra("combat.enemy_count", (object)combatState.Enemies.Count);
				scope.SetExtra("combat.enemies", (object)string.Join(", ", combatState.Enemies.Select((Creature e) => e.Monster?.Id.ToString() ?? "unknown")));
				List<string> list = combatState.Players.Select((Player p) => $"{p.Creature.CurrentHp}/{p.Creature.MaxHp}").ToList();
				if (list.Count > 0)
				{
					scope.SetExtra("combat.player_hp", (object)string.Join(", ", list));
				}
			}
		}
		catch
		{
		}
	}

	private static string? GetCurrentSceneName()
	{
		try
		{
			NGame instance = NGame.Instance;
			if (instance == null)
			{
				return null;
			}
			if (instance.MainMenu != null)
			{
				return "MainMenu";
			}
			if (instance.CurrentRunNode != null)
			{
				NRun currentRunNode = instance.CurrentRunNode;
				if (currentRunNode.CombatRoom != null)
				{
					return "CombatRoom";
				}
				if (currentRunNode.MapRoom != null)
				{
					return "MapRoom";
				}
				if (currentRunNode.EventRoom != null)
				{
					return "EventRoom";
				}
				if (currentRunNode.RestSiteRoom != null)
				{
					return "RestSiteRoom";
				}
				if (currentRunNode.MerchantRoom != null)
				{
					return "MerchantRoom";
				}
				if (currentRunNode.TreasureRoom != null)
				{
					return "TreasureRoom";
				}
				return "Run";
			}
			if (instance.LogoAnimation != null)
			{
				return "LogoAnimation";
			}
			return null;
		}
		catch
		{
			return null;
		}
	}

	private static bool HasUserConsent()
	{
		try
		{
			return SaveManager.Instance.PrefsSave.UploadData;
		}
		catch
		{
			return false;
		}
	}
}
