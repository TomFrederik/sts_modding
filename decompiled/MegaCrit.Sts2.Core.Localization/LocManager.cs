using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.RegularExpressions;
using Godot;
using MegaCrit.Sts2.Core.Localization.Formatters;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Platform;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.TestSupport;
using Sentry;
using SmartFormat;
using SmartFormat.Core.Extensions;
using SmartFormat.Core.Formatting;
using SmartFormat.Core.Parsing;
using SmartFormat.Core.Settings;
using SmartFormat.Extensions;

namespace MegaCrit.Sts2.Core.Localization;

public class LocManager
{
	public delegate void LocaleChangeCallback();

	private Dictionary<string, LocTable> _tables = new Dictionary<string, LocTable>();

	private static SmartFormatter _smartFormatter = null;

	private const string _weblateProjectSlug = "slaythespire2";

	private static readonly Dictionary<string, string> _weblateToGameLanguage = new Dictionary<string, string>
	{
		{ "de", "deu" },
		{ "es", "spa" },
		{ "es_LATAM", "esp" },
		{ "fr", "fra" },
		{ "it", "ita" },
		{ "ja", "jpn" },
		{ "ko", "kor" },
		{ "pl", "pol" },
		{ "pt_BR", "ptb" },
		{ "ru", "rus" },
		{ "th", "tha" },
		{ "tr", "tur" },
		{ "zh_Hans", "zhs" }
	};

	private static readonly Dictionary<string, string> _gameToWeblateLanguage = _weblateToGameLanguage.ToDictionary<KeyValuePair<string, string>, string, string>((KeyValuePair<string, string> kvp) => kvp.Value, (KeyValuePair<string, string> kvp) => kvp.Key);

	private Dictionary<string, int> _languageKeyCount = new Dictionary<string, int>();

	public const string locOverrideDir = "user://localization_override";

	private readonly List<LocaleChangeCallback> _localeChangeCallbacks = new List<LocaleChangeCallback>();

	private static readonly CultureInfo EnglishCultureInfo;

	public static LocManager Instance { get; private set; } = null;

	private static string LocalizationAssetDir => "res://localization";

	public bool OverridesActive { get; private set; }

	public IReadOnlyList<LocValidationError> ValidationErrors { get; private set; } = Array.Empty<LocValidationError>();

	public string Language { get; private set; }

	public static List<string> Languages { get; }

	public CultureInfo CultureInfo { get; private set; }

	public static void Initialize()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		Instance = new LocManager();
		string text = ProjectSettings.GlobalizePath("user://localization_override");
		if (!DirAccess.DirExistsAbsolute(text))
		{
			DirAccess.MakeDirAbsolute(text);
		}
	}

	public LocManager()
	{
		string text = SaveManager.Instance.SettingsSave.Language;
		if (string.IsNullOrEmpty(text))
		{
			string rawLanguage = PlatformUtil.GetRawLanguage();
			text = PlatformUtil.GetThreeLetterLanguageCode();
			if (text == null || !Languages.Contains(text))
			{
				Log.Warn($"Could not initialize language from platform language: {rawLanguage} (resolved: {text}). Defaulting to english");
				text = "eng";
			}
			else
			{
				Log.Info("Initializing language for the first time from platform locale: " + rawLanguage + " -> " + text);
			}
			SaveManager.Instance.SettingsSave.Language = text;
		}
		else if (!Languages.Contains(text))
		{
			Log.Warn("Saved language '" + text + "' is not supported. Defaulting to english");
			text = "eng";
			SaveManager.Instance.SettingsSave.Language = text;
		}
		SetLanguage(text);
		LoadLocFormatters();
		LoadLocCompletionFile();
	}

	private CultureInfo CultureInfoFromThreeLetterCode(string language)
	{
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			CultureInfo cultureInfo = System.Globalization.CultureInfo.GetCultures(CultureTypes.NeutralCultures).FirstOrDefault((CultureInfo c) => c.ThreeLetterISOLanguageName == language);
			if (cultureInfo != null)
			{
				return cultureInfo;
			}
		}
		catch (CultureNotFoundException value)
		{
			Log.Error($"Couldn't enumerate cultures: {value}");
		}
		if (language == "zhs")
		{
			return System.Globalization.CultureInfo.GetCultureInfo("zh-hans");
		}
		if (language == "zht")
		{
			return System.Globalization.CultureInfo.GetCultureInfo("zh-hant");
		}
		if (language == "ptb")
		{
			return System.Globalization.CultureInfo.GetCultureInfo("pt-br");
		}
		if (language == "esp")
		{
			return System.Globalization.CultureInfo.GetCultureInfo("es-419");
		}
		if (language == "spa")
		{
			return System.Globalization.CultureInfo.GetCultureInfo("es-ES");
		}
		if (language == "deu")
		{
			return System.Globalization.CultureInfo.GetCultureInfo("de");
		}
		if (language == "fra")
		{
			return System.Globalization.CultureInfo.GetCultureInfo("fr");
		}
		if (language == "ita")
		{
			return System.Globalization.CultureInfo.GetCultureInfo("it");
		}
		if (language == "jpn")
		{
			return System.Globalization.CultureInfo.GetCultureInfo("ja");
		}
		if (language == "kor")
		{
			return System.Globalization.CultureInfo.GetCultureInfo("ko");
		}
		if (language == "pol")
		{
			return System.Globalization.CultureInfo.GetCultureInfo("pl");
		}
		if (language == "rus")
		{
			return System.Globalization.CultureInfo.GetCultureInfo("ru");
		}
		if (language == "tha")
		{
			return System.Globalization.CultureInfo.GetCultureInfo("th");
		}
		if (language == "tur")
		{
			return System.Globalization.CultureInfo.GetCultureInfo("tr");
		}
		string text = "Language code " + language + " could not be mapped to CultureInfo! Add a new manual mapping";
		Log.Error(text);
		SentrySdk.CaptureMessage(text, (SentryLevel)1);
		return System.Globalization.CultureInfo.GetCultureInfo("en-us");
	}

	private void LoadLocFormatters()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Expected O, but got Unknown
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Expected O, but got Unknown
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Expected O, but got Unknown
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Expected O, but got Unknown
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Expected O, but got Unknown
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Expected O, but got Unknown
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Expected O, but got Unknown
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Expected O, but got Unknown
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Expected O, but got Unknown
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Expected O, but got Unknown
		_smartFormatter = new SmartFormatter((SmartSettings)null);
		ListFormatter val = new ListFormatter();
		_smartFormatter.AddExtensions((ISource[])(object)new ISource[5]
		{
			(ISource)val,
			(ISource)new DictionarySource(),
			(ISource)new ValueTupleSource(),
			(ISource)new ReflectionSource(),
			(ISource)new DefaultSource()
		});
		_smartFormatter.AddExtensions((IFormatter[])(object)new IFormatter[15]
		{
			(IFormatter)val,
			(IFormatter)new PluralLocalizationFormatter(),
			(IFormatter)new ConditionalFormatter(),
			(IFormatter)new ChooseFormatter(),
			(IFormatter)new SubStringFormatter(),
			(IFormatter)new IsMatchFormatter(),
			(IFormatter)new DefaultFormatter(),
			new AbsoluteValueFormatter(),
			new EnergyIconsFormatter(),
			new StarIconsFormatter(),
			new HighlightDifferencesFormatter(),
			new HighlightDifferencesInverseFormatter(),
			new PercentMoreFormatter(),
			new PercentLessFormatter(),
			new ShowIfUpgradedFormatter()
		});
		Smart.Default = _smartFormatter;
	}

	private void LoadLocCompletionFile()
	{
		FileAccess val = FileAccess.Open("localization/completion.json", (ModeFlags)1);
		try
		{
			if (val == null)
			{
				throw new LocException("Cannot find language completion file: localization/completion.json");
			}
			string asText = val.GetAsText(false);
			_languageKeyCount = JsonSerializer.Deserialize(asText, LocManagerSerializerContext.Default.DictionaryStringInt32);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public float GetLanguageCompletion(string language)
	{
		if (!_languageKeyCount.TryGetValue(language, out var value))
		{
			value = 0;
		}
		return (float)value / (float)_languageKeyCount["eng"];
	}

	public string SmartFormat(LocString locString, Dictionary<string, object> variables)
	{
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		string rawText = locString.GetRawText();
		LocTable table = GetTable(locString.LocTable);
		CultureInfo cultureInfo = (table.IsLocalKey(locString.LocEntryKey) ? CultureInfo : EnglishCultureInfo);
		try
		{
			return _smartFormatter.Format((IFormatProvider)cultureInfo, rawText, new object[1] { variables });
		}
		catch (Exception ex) when (((ex is FormattingException || ex is ParsingErrors) ? 1 : 0) != 0)
		{
			string text = $"message={ex.Message}\ntable={locString.LocTable} key={locString.LocEntryKey} variables={ToString(variables)}";
			Log.Error("Localization formatting error! " + text);
			string errorPattern = Regex.Replace(ex.Message.Split('\n')[0], " at \\d+$", "");
			SentrySdk.CaptureException((Exception)new LocException(text), (Action<Scope>)delegate(Scope scope)
			{
				EventLikeExtensions.SetFingerprint((IEventLike)(object)scope, new string[2] { "LocException", errorPattern });
			});
			return rawText;
		}
	}

	private static string ConvertToW(string input)
	{
		int num = 0;
		int num2 = 0;
		char[] array = new char[input.Length];
		for (int i = 0; i < input.Length; i++)
		{
			char c = input[i];
			switch (c)
			{
			case '{':
				num++;
				break;
			case '[':
				num2++;
				break;
			case '}':
				num--;
				break;
			case ']':
				num2--;
				break;
			}
			if (char.IsLetter(c) && num == 0 && num2 == 0)
			{
				array[i] = (char.IsUpper(c) ? 'W' : 'w');
			}
			else
			{
				array[i] = c;
			}
		}
		return new string(array);
	}

	private static string ToString(Dictionary<string, object> variables)
	{
		return "{" + string.Join(",", variables.Select<KeyValuePair<string, object>, string>((KeyValuePair<string, object> kp) => $"{kp.Key}:{kp.Value}")) + "}";
	}

	[MemberNotNull(new string[] { "CultureInfo", "Language" })]
	public void SetLanguage(string language)
	{
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		(Dictionary<string, LocTable> tables, bool overridesActive, List<LocValidationError> validationErrors) tuple = LoadTablesFromPath(language);
		Dictionary<string, LocTable> item = tuple.tables;
		bool item2 = tuple.overridesActive;
		List<LocValidationError> item3 = tuple.validationErrors;
		_tables = item;
		OverridesActive = item2;
		ValidationErrors = item3.AsReadOnly();
		Language = language;
		if (OverridesActive)
		{
			Log.Info("Localization overrides are active for language '" + language + "'");
		}
		CultureInfo = CultureInfoFromThreeLetterCode(Language);
		if (TestMode.IsOn)
		{
			Callable val = Callable.From((Action)TriggerLocaleChange);
			((Callable)(ref val)).CallDeferred(Array.Empty<Variant>());
		}
		else
		{
			TriggerLocaleChange();
		}
	}

	private static (Dictionary<string, LocTable> tables, bool overridesActive, List<LocValidationError> validationErrors) LoadTablesFromPath(string language)
	{
		Dictionary<string, LocTable> dictionary = null;
		if (language != "eng")
		{
			dictionary = LoadTablesFromPath("eng").tables;
		}
		Dictionary<string, LocTable> dictionary2 = new Dictionary<string, LocTable>();
		List<LocValidationError> list = new List<LocValidationError>();
		string text = LocalizationAssetDir + "/" + language;
		bool flag = false;
		bool item = false;
		if (!DirAccess.DirExistsAbsolute(text))
		{
			Log.Warn($"Dir path {text} for language {language} does not exist, falling back to eng");
			text = LocalizationAssetDir + "/eng";
			flag = OS.IsDebugBuild();
		}
		string text2 = ProjectSettings.GlobalizePath("user://localization_override");
		string text3 = Path.Combine(text2, language);
		bool flag2 = DirAccess.DirExistsAbsolute(text3);
		string text4 = Path.Combine(text2, "slaythespire2");
		bool flag3 = DirAccess.DirExistsAbsolute(text4);
		if (flag2)
		{
			Log.Info("Found flat localization override directory: " + text3);
		}
		if (flag3)
		{
			Log.Info("Found Weblate nested override directory: " + text4);
		}
		Log.Info("Loading locale path=" + text);
		IEnumerable<string> enumerable = ListLocalizationFiles(text);
		foreach (string item2 in enumerable)
		{
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(item2);
			string path = text + "/" + item2;
			Dictionary<string, string> dictionary3 = LoadTable(path);
			if (flag)
			{
				dictionary3 = dictionary3.ToDictionary((KeyValuePair<string, string> kvp) => kvp.Key, (KeyValuePair<string, string> kvp) => ConvertToW(kvp.Value));
			}
			LocTable fallback = dictionary?.GetValueOrDefault(fileNameWithoutExtension);
			LocTable locTable = new LocTable(fileNameWithoutExtension, dictionary3, fallback);
			if (!flag)
			{
				if (flag3 && TryLoadWeblateNestedOverrides(text2, language, item2, locTable, list))
				{
					item = true;
				}
				if (flag2)
				{
					string overrideFilePath = Path.Combine(text3, item2);
					if (TryLoadOverrideFile(overrideFilePath, locTable, list))
					{
						item = true;
					}
				}
			}
			foreach (string moddedLocTable in ModManager.GetModdedLocTables(language, item2))
			{
				Log.Info($"Found loc table from mod: {language} {item2}. Merging with base loc table");
				Dictionary<string, string> dictionary4 = LoadTable(moddedLocTable);
				if (flag)
				{
					dictionary4 = dictionary4.ToDictionary((KeyValuePair<string, string> kvp) => kvp.Key, (KeyValuePair<string, string> kvp) => ConvertToW(kvp.Value));
				}
				locTable.MergeWith(dictionary4);
			}
			dictionary2[fileNameWithoutExtension] = locTable;
		}
		return (tables: dictionary2, overridesActive: item, validationErrors: list);
	}

	public LocTable GetTable(string name)
	{
		if (_tables.TryGetValue(name, out LocTable value))
		{
			return value;
		}
		throw new LocException("The loc table='" + name + "' does not exist!");
	}

	private static Dictionary<string, string> LoadTable(string path)
	{
		FileAccess val = FileAccess.Open(path, (ModeFlags)1);
		try
		{
			if (val == null)
			{
				throw new LocException("Cannot find language file: " + path);
			}
			string asText = val.GetAsText(false);
			try
			{
				return JsonSerializer.Deserialize(asText, LocManagerSerializerContext.Default.DictionaryStringString);
			}
			catch (Exception e)
			{
				throw new LocException("Failed to parse language file: " + path, e);
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private static IEnumerable<string> ListLocalizationFiles(string path)
	{
		DirAccess val = DirAccess.Open(path);
		try
		{
			if (val == null)
			{
				throw new LocException("Path does not exist: " + path);
			}
			return (from s in val.GetFiles()
				where s.EndsWith(".json")
				select s).ToArray();
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private static bool TryLoadOverrideFile(string overrideFilePath, LocTable locTable, List<LocValidationError> validationErrors)
	{
		if (!FileAccess.FileExists(overrideFilePath))
		{
			return false;
		}
		Log.Info("Loading localization override: " + overrideFilePath);
		try
		{
			Dictionary<string, string> dictionary = LoadTable(overrideFilePath);
			Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
			foreach (KeyValuePair<string, string> item in dictionary)
			{
				if (item.Key.StartsWith("EXTENSION.") || LocValidator.ValidateFormatString(item.Value, out string errorMessage))
				{
					dictionary2[item.Key] = item.Value;
					continue;
				}
				validationErrors.Add(new LocValidationError(overrideFilePath, item.Key, errorMessage));
				Log.Warn("[LocValidation] Invalid format in override file: " + overrideFilePath);
				Log.Warn("  Key: " + item.Key);
				Log.Warn("  Error: " + errorMessage);
			}
			locTable.MergeWith(dictionary2);
			return true;
		}
		catch (LocException ex)
		{
			validationErrors.Add(new LocValidationError(overrideFilePath, "(JSON parsing error)", ex.InnerException?.Message ?? ex.Message));
			Log.Warn("[LocValidation] Failed to parse override file: " + overrideFilePath);
			Log.Warn("  Error: " + (ex.InnerException?.Message ?? ex.Message));
			return false;
		}
	}

	private static bool TryLoadWeblateNestedOverrides(string globalizedOverrideDir, string language, string filename, LocTable locTable, List<LocValidationError> validationErrors)
	{
		if (!_gameToWeblateLanguage.TryGetValue(language, out string value))
		{
			return false;
		}
		string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filename);
		global::_003C_003Ey__InlineArray5<string> buffer = default(global::_003C_003Ey__InlineArray5<string>);
		buffer[0] = globalizedOverrideDir;
		buffer[1] = "slaythespire2";
		buffer[2] = fileNameWithoutExtension;
		buffer[3] = value;
		buffer[4] = filename;
		string text = Path.Combine(buffer);
		if (TryLoadOverrideFile(text, locTable, validationErrors))
		{
			Log.Info("Found Weblate nested override structure: " + text);
			return true;
		}
		return false;
	}

	public void SubscribeToLocaleChange(LocaleChangeCallback callback)
	{
		_localeChangeCallbacks.Add(callback);
	}

	public void UnsubscribeToLocaleChange(LocaleChangeCallback callback)
	{
		_localeChangeCallbacks.Remove(callback);
	}

	private void TriggerLocaleChange()
	{
		TranslationServer.SetLocale(CultureInfo.Name);
		foreach (LocaleChangeCallback localeChangeCallback in _localeChangeCallbacks)
		{
			localeChangeCallback();
		}
		GC.Collect();
	}

	static LocManager()
	{
		int num = 14;
		List<string> list = new List<string>(num);
		CollectionsMarshal.SetCount(list, num);
		Span<string> span = CollectionsMarshal.AsSpan(list);
		int num2 = 0;
		span[num2] = "eng";
		num2++;
		span[num2] = "zhs";
		num2++;
		span[num2] = "deu";
		num2++;
		span[num2] = "esp";
		num2++;
		span[num2] = "fra";
		num2++;
		span[num2] = "ita";
		num2++;
		span[num2] = "jpn";
		num2++;
		span[num2] = "kor";
		num2++;
		span[num2] = "pol";
		num2++;
		span[num2] = "ptb";
		num2++;
		span[num2] = "rus";
		num2++;
		span[num2] = "spa";
		num2++;
		span[num2] = "tha";
		num2++;
		span[num2] = "tur";
		Languages = list;
		EnglishCultureInfo = System.Globalization.CultureInfo.GetCultureInfo("en");
	}
}
