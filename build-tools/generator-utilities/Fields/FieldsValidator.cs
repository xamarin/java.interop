using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xamarin.Android.Tools.ObjectModel;

namespace Xamarin.Android.Tools.Fields
{

	public static class FieldsValidator
	{
		private static int missingTypes;

		private static int missingFields;

		private static List<string> skipPackages = new List<string> { "Java.Time", "Java.Time.Chrono" };

		private static List<string> skipTypes = new List<string> { "Android.Test.ITestSuiteProvider" };

		private static List<string> skipApis = new List<string> { "R" };

		public static Command CliCommandConfiguration ()
		{
			Command cmd = new Command ("--fields-validator") {
				Description = "Validate that all fields where generated as expected.",
			};

			cmd.AddOption (
				new Option ("--verbose", "Prints verbose messages.") {
					Required = false,
					Argument = new Argument<bool> (),
				}
			);

			cmd.AddOption (
				new Option ("--use-cache", "True is default. If False, skips cache usage if cache file exists.") {
					Required = false,
					Argument = new Argument<bool> (() => true),
				}
			);

			cmd.AddOption (
				new Option ("--assembly-location", "Mono.Android.dll assembly folder location.") {
					Required = true,
					Argument = new Argument<string> (),
				}
			);

			cmd.AddOption (
				new Option ("--metadata-location", "The enum map.csv folder location.") {
					Required = true,
					Argument = new Argument<string> (),
				}
			);

			cmd.AddOption (
				new Option ("--enum-map", "The enum map.csv folder location.") {
					Required = true,
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

			cmd.Handler = CommandHandler.Create (async (bool verbose, bool useCache, string assemblyLocation, string metadataLocation, string enumMap, List<string> excludePackages, List<string> includePackages) => {
/*
				Options.Verbose = verbose;
				ObjectModelBuilder.UseCache = useCache;

				var monoAndroidDllFile = Path.Combine (assemblyLocation, "Mono.Android.dll");
				if (!File.Exists (monoAndroidDllFile)) {
					Console.WriteLine ($"Mono.Android.dll does not exist on: {assemblyLocation}");
					return 1;
				}

				var metadaFile = Path.Combine (metadataLocation, "metadata");
				if (!File.Exists (metadaFile)) {
					Console.WriteLine ($"metadata file does not exist on: {metadataLocation}");
					return 1;
				}

				var mapFile = Path.Combine (enumMap, "map.csv");
				if (!File.Exists (mapFile)) {
					Console.WriteLine ($"map.csv file does not exist on: {enumMap}");
					return 1;
				}

				ObjectModelBuilder.MonoAndroidAssembly = Assembly.LoadFrom (monoAndroidDllFile);
				ObjectModelBuilder.Metadata.Load (metadaFile);

				ObjectModelBuilder.ExcludePackages = excludePackages;
				ObjectModelBuilder.IncludePackages = includePackages;

				await ObjectModelBuilder.BuildObjectModel ();

				var map = await ParseMapCsv (mapFile);

				missingTypes = 0;
				missingFields = 0;
				foreach (var package in ObjectModelBuilder.AndroidPackages) {
					if (skipPackages.Contains (package.PackageName)) {
						continue;
					}

					await ValidateTypesAndConstantFields (package, map);
				}

				Console.WriteLine ("Done validating types and fields.");
				Console.WriteLine ($"Total missing types:{missingTypes}");
				Console.WriteLine ($"Total missing fields:{missingFields}");
				*/

				return 0;
			});

			return cmd;
		}

		static async Task<HashSet<string>> ParseMapCsv (string mapFile)
		{
			var enums = new HashSet<string> ();

			var lines = await File.ReadAllLinesAsync (mapFile);
			foreach (var line in lines) {
				if (line.StartsWith ("//", StringComparison.OrdinalIgnoreCase) || string.IsNullOrWhiteSpace (line)) {
					continue;
				}

				var tokens = line.Split (',');
				if (tokens.Length < 5) {
					continue;
				}

				if (string.IsNullOrWhiteSpace (tokens [3])) {
					continue;
				}

				enums.Add ($"{NamingConverter.ConvertNamespaceToCSharp (tokens [3].Replace ("/", "."))}".ToLowerInvariant ());
			}

			return enums;
		}

		static async Task ValidateTypesAndConstantFields (AndroidType androidType, HashSet<string> map)
		{
			if (androidType.AndroidObjectType == AndroidObjectType.Class || androidType.AndroidObjectType == AndroidObjectType.Interface) {
				VerifyConstantFieldsExistOnAssembly (androidType, map);
			}

			foreach (var item in androidType.Types) {
				await ValidateTypesAndConstantFields (item, map);
			}
		}

		static void VerifyConstantFieldsExistOnAssembly (AndroidType androidType, HashSet<string> map)
		{
			var typeName = androidType.PackageName + ".";
			if (androidType.AndroidObjectType == AndroidObjectType.Class) {
				typeName += androidType.FullName;
			} else if (androidType.AndroidObjectType == AndroidObjectType.Interface) {
				if (androidType.ConstantFields.Any ()) {
					// Interface has constants, but if all constats are part of enum we should still keep it as interface
					bool allEnums = true;
					foreach (var field in androidType.ConstantFields) {
						var name = $"i:{androidType.JavaPackageName}.{androidType.OriginalJavaName}.{field.JavaName}".ToLowerInvariant ();
						if (map.Contains (name)) {
							continue;
						}

						name = $"{androidType.JavaPackageName}.{androidType.OriginalJavaName.Replace (".", "$")}.{field.JavaName}".ToLowerInvariant ();
						if (map.Contains (name)) {
							continue;
						}

						allEnums = false;
						break;
					}

					if (allEnums) {
						typeName += androidType.FullName;
					} else {
						if (androidType.FullName.IndexOf ("+I", StringComparison.OrdinalIgnoreCase) != -1) {
							typeName += androidType.FullName.Replace ("+I", "+");
						} else if (androidType.FullName.IndexOf ("+") == -1) {
							typeName += androidType.FullName.Substring (1);
						} else {
							System.Diagnostics.Debug.Assert (false, $"BUGBUG should never get here. FullName:{androidType.FullName}");
						}
					}
				} else {
					typeName += androidType.FullName;
				}
			} else {
				System.Diagnostics.Debug.Assert (false, "BUGBUG should never get here. VerifyConstantFieldsExistOnAssembly");
			}

			// We skip Iterator classes
			if (typeName.EndsWith ("Iterator", StringComparison.OrdinalIgnoreCase)) {
				return;
			}

			var objType = ObjectModelBuilder.MonoAndroidAssembly.GetType (typeName);

			if (objType == null) {
				if (skipApis.Contains (androidType.ApiAdded)) {
					return;
				}

				if (skipTypes.Contains (typeName)) {
					return;
				}

				missingTypes++;
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine ($"Missing Type, {androidType.PackageName}, {androidType.FullName}, , {androidType.Url}, {androidType.ApiAdded}");
				Console.ForegroundColor = ConsoleColor.White;
				return;
			}

			foreach (var field in androidType.ConstantFields) {
				var name = $"{androidType.JavaPackageName}.{androidType.OriginalJavaName}.{field.JavaName}".ToLowerInvariant ();
				if (androidType.AndroidObjectType == AndroidObjectType.Interface) {
					name = $"i:{name}";
				}

				if (map.Contains (name)) {
					continue;
				}

				name = $"{androidType.JavaPackageName}.{androidType.OriginalJavaName.Replace (".", "$")}.{field.JavaName}".ToLowerInvariant ();
				if (map.Contains (name)) {
					continue;
				}

				BindingFlags bindingFlags = BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
				var myField = objType.GetField (field.Name, bindingFlags);

				if (myField == null) {
					if (skipApis.Contains (field.ApiAdded)) {
						continue;
					}

					missingFields++;
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine ($"Missing Field, {androidType.PackageName}, {androidType.FullName}, {field.Name}, {androidType.Url}, {field.ApiAdded}");
					Console.ForegroundColor = ConsoleColor.White;
				}
			}
		}
	}
}
