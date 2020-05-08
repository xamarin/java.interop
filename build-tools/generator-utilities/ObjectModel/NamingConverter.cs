using System;
using System.Globalization;

namespace Xamarin.Android.Tools.ObjectModel
{
	public static class NamingConverter
	{
		public static string ConvertNamespaceToCSharp (string content)
		{
			content = content.ToLowerInvariant ();
			TextInfo txtInfo = new CultureInfo ("en-us", false).TextInfo;
			content = txtInfo.ToTitleCase (content);
			var tokens = content.Split ('.');
			for (var i = 0; i < tokens.Length; i++) {
				if (tokens [i].Length == 2) {
					tokens [i] = tokens [i].ToUpperInvariant ();
				}
			}

			return string.Join (".", tokens);
		}

		public static string ConvertClassToCSharp (string content)
		{
			return content;
		}

		public static string ConvertInterfaceToCSharp (string content)
		{

			return $"I{content}";
		}

		public static string ConvertFieldToCSharp (string content)
		{
			if (string.IsNullOrWhiteSpace (content)) {
				return string.Empty;
			}

			try {
				if (content.IndexOf ("_") == -1 && char.IsLower (content [0])) {
					return char.ToUpperInvariant (content [0]) + content.Substring (1);
				}

				return ConvertPascalCase (content).Replace ("_", string.Empty);
			} catch (Exception e) {
				Console.WriteLine (e);
				System.Diagnostics.Debugger.Break ();
				throw;
			}
		}

		public static string ConvertPascalCase (string content)
		{
			content = content.ToLowerInvariant ();
			TextInfo txtInfo = new CultureInfo ("en-us", false).TextInfo;
			return txtInfo.ToTitleCase (content).Trim ();
		}
	}
}
