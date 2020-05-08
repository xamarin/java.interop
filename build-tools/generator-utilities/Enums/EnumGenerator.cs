using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xamarin.Android.Tools.ObjectModel;

namespace Xamarin.Android.Tools.Enums
{
	public static class EnumGenerator
	{

		private static List<string> skipPackages = new List<string> { "Java.Time", "Java.Time.Chrono" };

		private static List<string> skipTypes = new List<string> { "Android.Test.ITestSuiteProvider" };

		private static HashSet<string> skipEnumeration = new HashSet<string> ();

		private static List<Tuple<AndroidType, string, string>> retryList = new List<Tuple<AndroidType, string, string>> ();

		private static List<EnumMap> enumMap = new List<EnumMap> ();

		private static List<MethodMap> methodMap = new List<MethodMap> ();

		private static string enumMapFile;

		private static string methodMapFile;

		public static Command CliCommandConfiguration ()
		{
			Command cmd = new Command ("--generate--enums") {
				Description = "Validate that all fields where generated as expected.",
			};

			cmd.AddOption (
				new Option ("--verbose", "Prints verbose messages.") {
					Required = false,
					Argument = new Argument<bool> (),
				}
			);

			cmd.AddOption (
				new Option ("--skip-cache", "Skip to use cache file.") {
					Required = false,
					Argument = new Argument<bool> (),
				}
			);

			cmd.AddOption (
				new Option ("--xa-repo", "The Xamarin Android repo/folder location.") {
					Required = true,
					Argument = new Argument<string> (),
				}
			);

			cmd.AddOption (
				new Option ("--api-level", "The api level to \"enumificate\".") {
					Required = true,
					Argument = new Argument<string> (),
				}
			);

			cmd.AddOption (
				new Option ("--skip-enumeration-file", "The skip enumeration file that contains the methods/arguments that should skip enumeration.") {
					Required = false,
					Argument = new Argument<string> (),
				}
			);

			cmd.AddOption (
				new Option ("--exclude-packages", "The Android packages to skip validation, if this option is not provided no packages are excluded from validation.") {
					Required = false,
					Argument = new Argument<List<string>> (),
				}
			);

			cmd.AddOption (
				new Option ("--include-packages", "The Android packages to validate, if this option is not provided all packages are validated.") {
					Required = false,
					Argument = new Argument<List<string>> (),
				}
			);

			cmd.Handler = CommandHandler.Create (async (bool verbose, bool skipCache, string xaRepo, string apiLevel, string skipEnumerationFile, List<string> excludePackages, List<string> includePackages) => {

				Options.Verbose = verbose;
				Options.ApiLevel = apiLevel;
				ObjectModelBuilder.SkipCache = skipCache;

				var metadaFile = Path.Combine (xaRepo, "src/Mono.Android/metadata");
				if (!File.Exists (metadaFile)) {
					Console.WriteLine ($"metadata file does not exist on: {metadaFile}");
					return 1;
				}

				enumMapFile = Path.Combine (xaRepo, "src/Mono.Android/map.csv");
				if (!File.Exists (enumMapFile)) {
					Console.WriteLine ($"Xamarin Android repo not valid on: {xaRepo}");
					return 1;
				}

				methodMapFile = Path.Combine (xaRepo, "src/Mono.Android/methodmap.csv");
				if (!File.Exists (enumMapFile)) {
					Console.WriteLine ($"Xamarin Android repo not valid on: {xaRepo}");
					return 1;
				}

				if (!string.IsNullOrWhiteSpace (skipEnumerationFile) && !File.Exists (skipEnumerationFile)) {
					Console.WriteLine ($"Unable to find skip enumeration file: {skipEnumerationFile}");
					return 1;
				}

				ObjectModelBuilder.Metadata.Load (metadaFile);

				ObjectModelBuilder.ExcludePackages = excludePackages;
				ObjectModelBuilder.IncludePackages = includePackages;

				await LoadSkipItems (skipEnumerationFile);
				await ParseEnumMapCsv ();
				await ParseMethodMapCsv ();

				await ObjectModelBuilder.BuildObjectModel ();

				retryList.Clear ();

				// Generate Enums
				foreach (var package in ObjectModelBuilder.AndroidPackages) {
					if (skipPackages.Contains (package.PackageName)) {
						continue;
					}

					await GenerateEnums (package);
				}

				await RetryGenerateEnums ();

				return 0;
			});

			return cmd;
		}

		static async Task LoadSkipItems (string fileName)
		{
			skipEnumeration.Clear ();
			var lines = await File.ReadAllLinesAsync (fileName);
			foreach (var line in lines) {
				if (line.StartsWith ("//", StringComparison.OrdinalIgnoreCase) || string.IsNullOrWhiteSpace (line)) {
					continue;
				}

				skipEnumeration.Add (line);
			}
		}

		static async Task ParseMethodMapCsv ()
		{
			methodMap.Clear ();

			var lines = await File.ReadAllLinesAsync (methodMapFile);
			foreach (var line in lines) {
				if (line.StartsWith ("//", StringComparison.OrdinalIgnoreCase) || string.IsNullOrWhiteSpace (line)) {
					continue;
				}

				var tokens = line.Split (',');
				if (tokens.Length < 6) {
					continue;
				}

				methodMap.Add (new MethodMap { Api = tokens [0].Trim (), JavaPackage = tokens [1].Trim (), JavaType = tokens [2].Trim (), MethodName = tokens [3].Trim (), ParamName = tokens [4].Trim (), EnumType = tokens [5].Trim () });
			}
		}

		static async Task ParseEnumMapCsv ()
		{
			enumMap.Clear ();

			var lines = await File.ReadAllLinesAsync (enumMapFile);
			foreach (var line in lines) {
				if (line.StartsWith ("//", StringComparison.OrdinalIgnoreCase) || string.IsNullOrWhiteSpace (line)) {
					continue;
				}

				var tokens = line.Split (',');
				if (tokens.Length < 5) {
					continue;
				}

				if (string.IsNullOrWhiteSpace (tokens [1]) || string.IsNullOrWhiteSpace (tokens [2]) || string.IsNullOrWhiteSpace (tokens [4])) {
					continue;
				}

				enumMap.Add (new EnumMap { Api = tokens [0].Trim (), CSharpNamespace = tokens [1].Trim (), CSharpType = tokens [2].Trim (), JavaName = tokens [3].Trim (), Value = tokens [4].Trim () });
			}
		}

		static async Task RetryGenerateEnums ()
		{
			foreach (var item in retryList) {
				var tokens = item.Item3.Split ("#");
				if (tokens.Length < 2) {
					return;
				}

				var name = tokens [1];
				var names = tokens [0].Replace ("/reference/", string.Empty).Split ("/");

				// Get the package name part
				var javaPackageName = $"{names [0]}.{names [1]}";
				var classNameIndex = 2;
				for (; classNameIndex < names.Length; classNameIndex++) {
					if (char.IsUpper (names [classNameIndex] [0])) {
						break;
					}

					javaPackageName += $".{names [classNameIndex]}";
				}

				AndroidType package;
				try {
					package = ObjectModelBuilder.AndroidPackages.Single (ap => ap.JavaPackageName == javaPackageName);
				} catch (Exception e) {
					return;
				}

				var classNames = names [classNameIndex].Split (".");
				var androidType = package.Types.SingleOrDefault (obj => obj.OriginalJavaName.Equals (classNames [0], StringComparison.OrdinalIgnoreCase));
				if (androidType == null) {
					// BUGBUG
					System.Diagnostics.Debugger.Break ();
				}

				for (int i = 1; i < classNames.Length || androidType == null; i++) {
					androidType = androidType.Types.SingleOrDefault (obj => obj.JavaName.Equals (classNames [i], StringComparison.OrdinalIgnoreCase));
				}

				if (androidType == null) {
					// BUGBUG
					System.Diagnostics.Debugger.Break ();
				}

				// Check if it is a method
				if (item.Item2.StartsWith ("get") && name.IndexOf ("(") != -1) {
					var methodName = name.Substring (0, name.IndexOf ("("));
					var enumMethodItem = methodMap.SingleOrDefault (i => i.JavaPackage == androidType.JavaPackageName && i.JavaType == androidType.OriginalJavaName && i.MethodName == methodName);
					if (enumMethodItem == null) {
						continue;
					}

					var methodItem = item.Item1.Methods.Single (i => i.JavaName == item.Item2);

					// Update methodMap
					await UpdateMethodMapFile (methodItem.ApiAdded, item.Item1.JavaPackageName, item.Item1.OriginalJavaName, item.Item2, "return", enumMethodItem.EnumType);
					continue;
				}
			}
		}


		static async Task GenerateEnums (AndroidType androidType)
		{
			if (androidType.AndroidObjectType == AndroidObjectType.Class || androidType.AndroidObjectType == AndroidObjectType.Interface) {
				await VerifyIfShouldGenerateEnum (androidType);
			}

			foreach (var item in androidType.Types) {
				await GenerateEnums (item);
			}
		}

		static async Task VerifyIfShouldGenerateEnum (AndroidType androidType)
		{
			// Methods
			foreach (var method in androidType.Methods) {
				if (method.ApiAdded == Options.ApiLevel) {

					// Enum for Return values
					if (method.ReturnType == "int" && method.ReturnValues.Count > 0) {
						await GenerateEnumIfNeeded (androidType, method.ApiAdded, method.JavaName, "return", method.ReturnValues);
					}

					// Enum for methods
					foreach (var argument in method.Arguments) {
						await GenerateEnumIfNeeded (androidType, method.ApiAdded, method.JavaName, argument.Item1, argument.Item2);
					}
				}
			}
		}

		static async Task GenerateEnumIfNeeded (AndroidType androidType, string apiAdded, string methodJavaName, string argumentName, List<string> values)
		{
			if (skipEnumeration.Contains ($"{androidType.JavaPackageName}.{androidType.JavaName}.{methodJavaName}.{argumentName}")) {
				return;
			}

			if ($"{methodJavaName}.{argumentName}" == "describeContents.return") {
				Console.WriteLine ($"{androidType.JavaPackageName}.{androidType.JavaName}.{methodJavaName}.{argumentName}");
				return;
			}

			var enumsFound = new List<EnumMap> ();
			var enumsNotFound = new List<EnumMap> ();

			bool hasNone = false;
			foreach (var value in values) {
				if (value == "none") {
					hasNone = true;
					continue;
				}

				if (value.IndexOf ("#") == -1) {
					continue;
				}

				var field = GetConstantField (value);
				if (field == null) {
					// Unable to understand the enum yet, we will add it to the retry and try again later.
					retryList.Add (new Tuple<AndroidType, string, string> (androidType, methodJavaName, value));
					continue;
				}

				// check to see if Constant is already part of a Enum
				try {
					var mapItem = enumMap.SingleOrDefault (i => i.JavaName == $"{field.JavaNamespace}.{field.JavaName}");
					if (mapItem != null) {
						if (!enumsFound.Any (i => i.JavaName == mapItem.JavaName)) {
							enumsFound.Add (mapItem);
						}

						continue;
					}
				} catch (Exception e) {
					Console.WriteLine (e);
					System.Diagnostics.Debugger.Break ();
					throw;
				}

				var enumMapItem = new EnumMap { Api = field.ApiAdded, CSharpType = NamingConverter.ConvertFieldToCSharp (field.JavaName), JavaName = $"{field.JavaNamespace}.{field.JavaName}", FieldName = field.JavaName, Value = field.Value };
				if (!enumsNotFound.Any (i => i.JavaName == enumMapItem.JavaName)) {
					enumsNotFound.Add (enumMapItem);
				}
			}

			// If no enum was found and no not found enum was added, it means we only have None, in this case we are unable to create anew enum.
			if (!enumsFound.Any () && !enumsNotFound.Any ()) {
				// Nothing to enumify
				return;
			}

			if (enumsFound.Any () && enumsFound.GroupBy (i => i.CSharpNamespace).Count () > 1) {
				// In case there is a colision betwen 2 different enums, before report error, see if we can fix it.
				// by fixing it, it means, some enums, use only a value 0 form a different enum,
				// items meaning: unknown or none, and for those cases we can simple ignore the value zero and stick with the other enum.
				var reportError = true;
				var groups = enumsFound.GroupBy (i => i.CSharpNamespace).ToList ();
				if (groups.Count () == 2) {
					if (groups [0].Count () == 1) {
						if (groups [0].ToList () [0].Value == "0") {
							enumsFound.Remove (groups [0].ToList () [0]);
							reportError = false;
						}
					} else if (groups [1].Count () == 1) {
						if (groups [1].ToList () [0].Value == "0") {
							enumsFound.Remove (groups [1].ToList () [0]);
							reportError = false;
						}
					}
				}

				if (reportError) {
					Console.WriteLine ($"Unable to enumify method:'{androidType.JavaPackageName}.{androidType.OriginalJavaName}.{methodJavaName}' because there is already multiple enums types using same const field.");
					return;
				}
			}

			var enumTypeName = string.Empty;
			if (enumsFound.Any ()) {
				enumTypeName = enumsFound [0].CSharpNamespace;
				System.Diagnostics.Debug.Assert (!string.IsNullOrWhiteSpace (enumTypeName), "BUGBUG - This should never happen.");
			} else {
				if (!hasNone && enumsNotFound.Count < 2) {
					return;
				}

				enumTypeName = GetEnumNameSuggestion (androidType.PackageName, enumsNotFound);
				if (string.IsNullOrWhiteSpace (enumTypeName)) {
					System.Diagnostics.Debug.Assert (false, "BUGBUG - Investigate why!");
				}
			}

			// Update enum
			enumsNotFound.ForEach (i => { i.CSharpNamespace = enumTypeName; });

			if (hasNone) {
				// Check to see if None already exists.
				if (!enumMap.Any (i => i.CSharpNamespace == enumTypeName && i.CSharpType == "None" && i.Value == "0")) {
					enumsNotFound.Add (new EnumMap {
						Api = "0",
						CSharpNamespace = enumTypeName,
						CSharpType = "None",
						JavaName = string.Empty,
						FieldName = string.Empty,
						Value = "0"
					});
				}
			}

			if (enumsNotFound.Any () && !skipEnumeration.Contains (enumTypeName)) {
				if (enumsFound.Any ()) {
					// Update map.csv
					await UpdateMapFile (enumsNotFound.SortByEnumValue ());
				} else {
					// Append on map.csv
					await AddOnMapFile (enumsNotFound.SortByEnumValue ());
				}
			}

			// Update methodMap
			await UpdateMethodMapFile (apiAdded, androidType.JavaPackageName, androidType.OriginalJavaName, methodJavaName, argumentName, enumTypeName);
		}

		static async Task UpdateMethodMapFile (string apiLevel, string javaPackage, string javaType, string methodName, string argName, string enumTypeName)
		{
			if (methodMap.Any (i => i.JavaPackage == javaPackage && i.JavaType == javaType && i.MethodName == methodName && i.ParamName == argName)) {
				return;
			}

			// Before enum a method make sure the enum options are bigger than 1, otherwise we should not enum
			if (enumMap.Count (i => i.CSharpNamespace == enumTypeName) < 2) {
				return;
			}

			if (apiLevel == "R") {
				apiLevel = "30";
			}

			await File.AppendAllTextAsync (methodMapFile, $"{apiLevel}, {javaPackage}, {javaType}, {methodName}, {argName}, {enumTypeName}{Environment.NewLine}");
		}

		static async Task UpdateMapFile (List<EnumMap> enums)
		{
			var mapFileContent = new List<string> ();
			var lines = await File.ReadAllLinesAsync (enumMapFile);

			bool foundType = false;
			bool enumAdded = false;
			foreach (var line in lines) {
				try {
					if (enumAdded) {
						mapFileContent.Add (line);
						continue;
					}

					if (line.StartsWith ("//", StringComparison.OrdinalIgnoreCase) || string.IsNullOrWhiteSpace (line)) {
						continue;
					}

					var tokens = line.Split (',');
					if (tokens.Length < 5) {
						continue;
					}

					if (string.IsNullOrWhiteSpace (tokens [1]) || string.IsNullOrWhiteSpace (tokens [2]) || string.IsNullOrWhiteSpace (tokens [4])) {
						continue;
					}

					if (!foundType && tokens [1] == $"{enums [0].CSharpNamespace}.{enums [0].CSharpType}") {
						foundType = true;
					} else if (foundType) {

						foreach (var item in enums) {
							if (item.Api == "R") {
								item.Api = "30";
							}

							mapFileContent.Add ($"{item.Api},{item.CSharpNamespace},{item.CSharpType},{item.JavaName},{item.Value}");

							// Also update our lookup
							enumMap.Add (item);
						}

						enumAdded = true;
					}

				} finally {
					mapFileContent.Add (line);
				}
			}

			await File.WriteAllLinesAsync (enumMapFile, mapFileContent);
		}

		static async Task AddOnMapFile (List<EnumMap> enums)
		{
			// Don't enum type with less than 2 items
			if (enums.Count < 2) {
				return;
			}

			var mapFileContent = new List<string> ();
			foreach (var item in enums) {
				if (item.Api == "R") {
					item.Api = "30";
				}

				mapFileContent.Add ($"{item.Api},{item.CSharpNamespace},{item.CSharpType},{item.JavaName},{item.Value}");

				// Also update our lookup
				enumMap.Add (item);
			}

			await File.AppendAllLinesAsync (enumMapFile, mapFileContent);
		}

		static string GetEnumNameSuggestion (string csharpNamespace, List<EnumMap> enums)
		{
			var enumName = string.Empty;
			if (enums.Count == 1) {
				var position = enums [0].FieldName.IndexOf ("_", StringComparison.OrdinalIgnoreCase);
				if (position == -1) {
					return string.Empty;
				}

				enumName = $"{csharpNamespace}.{NamingConverter.ConvertFieldToCSharp (enums [0].FieldName.Substring (0, position))}";
			} else {
				static string GetPrefix (string first, string second)
				{
					int prefixLength = 0;

					for (int i = 0; i < Math.Min (first.Length, second.Length); i++) {
						if (first [i] != second [i])
							break;

						prefixLength++;
					}

					var prefix = first.Substring (0, prefixLength);
					if (string.IsNullOrWhiteSpace (prefix)) {
					}
					return prefix;
				};

				enumName = enums.Select (i => i.JavaName).Aggregate (GetPrefix);
				var index = enumName.LastIndexOf ("/");
				if (index != -1) {
					enumName = enumName.Substring (index + 1);
				}

				if (string.IsNullOrWhiteSpace (enumName)) {
					return string.Empty;
				}

				var names = enumName.Split (".");

				if (names [1].IndexOf ("_") != -1) {
					names [1] = NamingConverter.ConvertFieldToCSharp (names [1]);
				}

				enumName = $"{csharpNamespace}.{names [0]}{names [1]}";
			}

			enumName = enumName.EndsWith ("Type", StringComparison.OrdinalIgnoreCase) ? enumName : $"{enumName}Type";

			// Validate type does not exists.
			if (enumMap.Any (i => i.CSharpNamespace == enumName)) {
				// Hope this will never happen, if it happens we need to think best way to give a better name to avoid colission.
				System.Diagnostics.Debugger.Break ();
			}

			return enumName;
		}

		static ConstantField GetConstantField (string path)
		{
			if (string.IsNullOrWhiteSpace (path)) {
				return null;
			}

			try {
				var tokens = path.Split ("#");
				if (tokens.Length < 2) {
					return null;
				}

				var name = tokens [1];
				var names = tokens [0].Replace ("/reference/", string.Empty).Split ("/");

				// Get the package name part
				var javaPackageName = $"{names [0]}.{names [1]}";
				var classNameIndex = 2;
				for (; classNameIndex < names.Length; classNameIndex++) {
					if (char.IsUpper (names [classNameIndex] [0])) {
						break;
					}

					javaPackageName += $".{names [classNameIndex]}";
				}

				AndroidType package;
				try {
					package = ObjectModelBuilder.AndroidPackages.Single (ap => ap.JavaPackageName == javaPackageName);
				} catch (Exception e) {
					return null;
				}

				var classNames = names [classNameIndex].Split (".");
				var androidType = package.Types.SingleOrDefault (obj => obj.OriginalJavaName.Equals (classNames [0], StringComparison.OrdinalIgnoreCase));
				if (androidType == null) {
					// BUGBUG
					System.Diagnostics.Debugger.Break ();
				}

				for (int i = 1; i < classNames.Length || androidType == null; i++) {
					androidType = androidType.Types.SingleOrDefault (obj => obj.JavaName.Equals (classNames [i], StringComparison.OrdinalIgnoreCase));
				}

				if (androidType == null) {
					// BUGBUG
					System.Diagnostics.Debugger.Break ();
				}

				var constantField = androidType.ConstantFields.SingleOrDefault (c => c.JavaName == name);
				return constantField;
			} catch (Exception e) {
				Console.WriteLine ($"Error: {path}\n{e.Message}");
				throw;
			}
		}

		static List<EnumMap> SortByEnumValue (this List<EnumMap> enums)
		{
			return enums.OrderBy (i => int.Parse (i.Value)).ToList ();
		}
	}
}

