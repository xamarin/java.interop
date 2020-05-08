
using System;

namespace Xamarin.Android.Tools
{
	public static class Options
	{
		public static readonly Uri BaseDocumentURl = new Uri ("https://developer.android.com");

		public static bool Verbose { get; set; } = false;

		public static string ApiLevel = string.Empty;

		public static void PrintVerbose (string content)
		{
			if (Verbose) {
				Console.WriteLine (content);
			}
		}
	}
}
