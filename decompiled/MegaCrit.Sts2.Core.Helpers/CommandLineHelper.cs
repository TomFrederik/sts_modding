using Godot;
using Godot.Collections;

namespace MegaCrit.Sts2.Core.Helpers;

public static class CommandLineHelper
{
	private static readonly Dictionary<string, string?> _args;

	static CommandLineHelper()
	{
		_args = new Dictionary<string, string>();
		string[] cmdlineArgs = OS.GetCmdlineArgs();
		for (int i = 0; i < cmdlineArgs.Length; i++)
		{
			string text = cmdlineArgs[i].TrimStart('-');
			string text2 = text;
			string text3 = null;
			int num = text.IndexOf('=');
			if (num > 0)
			{
				text2 = text.Substring(0, num);
				text3 = text.Substring(num + 1);
			}
			else if (i + 1 < cmdlineArgs.Length && !cmdlineArgs[i + 1].StartsWith('-') && !cmdlineArgs[i + 1].StartsWith('+'))
			{
				text3 = cmdlineArgs[i + 1];
				i++;
			}
			_args[text2] = text3;
		}
	}

	public static bool HasArg(string key)
	{
		return _args.ContainsKey(key);
	}

	public static bool TryGetValue(string key, out string? value)
	{
		return _args.TryGetValue(key, ref value);
	}

	public static string? GetValue(string key)
	{
		if (!TryGetValue(key, out string value))
		{
			return null;
		}
		return value;
	}
}
