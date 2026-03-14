using System.CodeDom.Compiler;

namespace System.Text.RegularExpressions.Generated;

[GeneratedCode("System.Text.RegularExpressions.Generator", "9.0.12.31616")]
internal sealed class _003CRegexGenerator_g_003EFDF0533EE0711AEDB383B6CD4472EB7858DDC4C8D92DA17B11D90839AAF25C6DD__NonSpaceWhitespaceCharacters_5 : Regex
{
	private sealed class RunnerFactory : RegexRunnerFactory
	{
		private sealed class Runner : RegexRunner
		{
			protected override void Scan(ReadOnlySpan<char> inputSpan)
			{
				if (TryFindNextPossibleStartingPosition(inputSpan))
				{
					int num = runtextpos;
					Capture(0, num, runtextpos = num + 1);
				}
			}

			private bool TryFindNextPossibleStartingPosition(ReadOnlySpan<char> inputSpan)
			{
				int num = runtextpos;
				if ((uint)num < (uint)inputSpan.Length)
				{
					int num2 = inputSpan.Slice(num).IndexOfAny('\t', '\n', '\r');
					if (num2 >= 0)
					{
						runtextpos = num + num2;
						return true;
					}
				}
				runtextpos = inputSpan.Length;
				return false;
			}
		}

		protected override RegexRunner CreateInstance()
		{
			return new Runner();
		}
	}

	internal static readonly _003CRegexGenerator_g_003EFDF0533EE0711AEDB383B6CD4472EB7858DDC4C8D92DA17B11D90839AAF25C6DD__NonSpaceWhitespaceCharacters_5 Instance = new _003CRegexGenerator_g_003EFDF0533EE0711AEDB383B6CD4472EB7858DDC4C8D92DA17B11D90839AAF25C6DD__NonSpaceWhitespaceCharacters_5();

	private _003CRegexGenerator_g_003EFDF0533EE0711AEDB383B6CD4472EB7858DDC4C8D92DA17B11D90839AAF25C6DD__NonSpaceWhitespaceCharacters_5()
	{
		pattern = "[\\t\\r\\n]";
		roptions = RegexOptions.None;
		Regex.ValidateMatchTimeout(_003CRegexGenerator_g_003EFDF0533EE0711AEDB383B6CD4472EB7858DDC4C8D92DA17B11D90839AAF25C6DD__Utilities.s_defaultTimeout);
		internalMatchTimeout = _003CRegexGenerator_g_003EFDF0533EE0711AEDB383B6CD4472EB7858DDC4C8D92DA17B11D90839AAF25C6DD__Utilities.s_defaultTimeout;
		factory = new RunnerFactory();
		capsize = 1;
	}
}
