using System;
using SmartFormat.Core.Parsing;
using SmartFormat.Core.Settings;

namespace MegaCrit.Sts2.Core.Localization;

public static class LocValidator
{
	public static bool ValidateFormatString(string text, out string? errorMessage)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		//IL_0017: Expected O, but got Unknown
		try
		{
			Parser val = new Parser((SmartSettings)null);
			val.ParseFormat(text);
			errorMessage = null;
			return true;
		}
		catch (ParsingErrors val2)
		{
			ParsingErrors val3 = val2;
			errorMessage = ((Exception)(object)val3).Message;
			return false;
		}
	}
}
