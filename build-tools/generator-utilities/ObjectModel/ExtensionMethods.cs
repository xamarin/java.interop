using System;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Xamarin.Android.Tools.ObjectModel
{
	public static class ExtensionMethods
	{
		public static async Task<string> GetContent (this Uri uri)
		{
			Options.PrintVerbose ($"GetContent : {uri}");

			for (var i = 0; i < 10; i++) {
				try {
					using (var wc = new HttpClient ()) {
						wc.DefaultRequestHeaders.Add ("User-Agent", "curl/7.64.1");
						wc.DefaultRequestHeaders.Add ("Accept", "*/*");

						var content = await wc.GetStringAsync (uri);
						return content;
					}
				} catch (Exception) {
					Console.WriteLine ($"Error downloading content: {uri}");

					if (i == 2) {
						throw;
					}

					await Task.Delay ((1000 * (i + 1)) * (i + 1));
				}
			}

			System.Environment.FailFast ("BUGBUG should never get here ExtensionMethods.GetContent");
			return null;
		}

		public static bool TryDeserialize<T> (this string value, out T outputObject)
		{
			outputObject = default;

			if (string.IsNullOrWhiteSpace (value)) {
				return false;
			}

			try {
				outputObject = JsonConvert.DeserializeObject<T> (value);
				return true;
			} catch (JsonSerializationException e) {
				Console.WriteLine ($"Error: {e}");
				return false;
			} catch (JsonReaderException e) {
				Console.WriteLine ($"Error: {e}");
				return false;
			} catch (FormatException e) {
				Console.WriteLine ($"Error: {e}");
				return false;
			} catch (TargetInvocationException e) {
				Console.WriteLine ($"Error: {e}");
				return false;
			} catch (ArgumentException e) {
				Console.WriteLine ($"Error: {e}");
				return false;
			}
		}

		public static string Serialize (this object value)
		{
			return JsonConvert.SerializeObject (value);
		}

		public static bool TryDeserializeBinaryFile<T> (this FileInfo value, out T outputObject)
		{
			outputObject = default;

			IFormatter formatter = new BinaryFormatter ();
			using var stream = File.Open (value.FullName, FileMode.Open);
			var obj = formatter.Deserialize (stream);
			outputObject = (T) Convert.ChangeType (obj, typeof (T));
			stream.Close ();

			return true;
		}

		public static void SerializeBinaryToFile (this object value, string fileName)
		{
			IFormatter formatter = new BinaryFormatter ();
			using var stream = new FileStream (fileName, FileMode.Create, FileAccess.Write, FileShare.None);
			formatter.Serialize (stream, value);
			stream.Close ();
		}
	}
}
