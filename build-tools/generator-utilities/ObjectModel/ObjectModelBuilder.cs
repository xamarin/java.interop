using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace Xamarin.Android.Tools.ObjectModel
{
	public static class ObjectModelBuilder
	{
		public const string CacheFile = "om.bin";

		public static bool SkipCache { get; set; }

		public static Assembly MonoAndroidAssembly { get; set; }

		public static XmlDocument Metadata { get; set; } = new XmlDocument ();

		public static List<string> ExcludePackages { get; set; }

		public static List<string> IncludePackages { get; set; }

		public static List<AndroidType> AndroidPackages { get; set; } = new List<AndroidType> ();

		public static readonly Uri DeveloperPackageDocumentationUrl = new Uri (Options.BaseDocumentURl, "reference/packages");

		static object SyncObject = new Object ();

		static ObjectModelBuilder ()
		{
			AppDomain.CurrentDomain.AssemblyResolve += CurrentDomainAssemblyResolve;
		}

		public static async Task BuildObjectModel ()
		{
			if (!SkipCache) {
				var fileInfo = new FileInfo (CacheFile);
				if (fileInfo.Exists) {
					if (fileInfo.TryDeserializeBinaryFile (out List<AndroidType> androidPackages)) {
						AndroidPackages = androidPackages;
						return;
					}
				}
			}

			List<Task> tasks = new List<Task> ();

			var stopWatch = new Stopwatch ();
			stopWatch.Start ();

			var content = await DeveloperPackageDocumentationUrl.GetContent ();
			var pattern = "<td class=\"jd-linkcol\">[^<]*<a href=\"([^\"]*)\">([^<]*)<";
			var regex = new Regex (pattern, RegexOptions.Multiline);
			foreach (Match package in regex.Matches (content)) {

				var javaPackageName = package.Groups [2].Value;

				// Skip all packages that we are explicit excluded.
				if (ExcludePackages != null && ExcludePackages.Any () && ExcludePackages.Any (name => name.Equals (javaPackageName, StringComparison.OrdinalIgnoreCase))) {
					continue;
				}

				// If a list of Included packages was provided, then Skip all packages that we were not provided on the list.
				if (IncludePackages != null && IncludePackages.Any () && !IncludePackages.Any (name => name.Equals (javaPackageName, StringComparison.OrdinalIgnoreCase))) {
					continue;
				}

				var packageName = NamingConverter.ConvertNamespaceToCSharp (javaPackageName);
				var packageNode = Metadata.SelectSingleNode ($"//attr[@path=\"/api/package[@name='{javaPackageName}']\" and @name=\"managedName\"]");
				if (packageNode != null) {
					packageName = packageNode.InnerText;
				}

				tasks.Add (Task.Run (async () => {
					var androidType = new AndroidType {
						AndroidObjectType = AndroidObjectType.Package,
						PackageName = packageName,
						JavaPackageName = javaPackageName,
						Url = new Uri (DeveloperPackageDocumentationUrl, package.Groups [1].Value),
					};

					lock (SyncObject) {
						AndroidPackages.Add (androidType);
					}

					await GetAndroidType (androidType);
				}));
			}

			await Task.WhenAll (tasks);

			// Fix all objects that needs to be fixed
			bool allFixed = false;
			for (var i = 0; i < 5; i++) {
				allFixed = true;
				var packages = AndroidPackages.ToArray ();
				foreach (var package in packages) {
					var types = package.Types.ToArray ();
					foreach (var androidType in types) {
						allFixed &= FixNeededItems (package, androidType);
					}
				}

				if (allFixed == true) {
					break;
				}
			}

			// Unable to fix all object after 5 tries.
			if (!allFixed) {
				foreach (var package in AndroidPackages) {
					PrintUnFixedItems (package);
				}
			}

			stopWatch.Stop ();
			Options.PrintVerbose ($"Building object model took: {stopWatch.ElapsedMilliseconds} milliseconds");

			AndroidPackages.SerializeBinaryToFile (CacheFile);
		}

		static void PrintUnFixedItems (AndroidType androidType)
		{
			if (!androidType.ParentObjectCorrectlyIdentified) {
				Console.WriteLine ($"PackageName:{androidType.PackageName} - FullName:{androidType.FullName}");
			}

			foreach (var type in androidType.Types) {
				PrintUnFixedItems (type);
			}
		}

		static bool FixNeededItems (AndroidType currentParentType, AndroidType androidType)
		{
			bool success = true;
			if (!androidType.ParentObjectCorrectlyIdentified) {
				success &= FixType (currentParentType, androidType);
			}

			var items = androidType.Types.ToArray ();
			foreach (var item in items) {
				success &= FixNeededItems (androidType, item);
			}

			return success;
		}

		static bool FixType (AndroidType currentParentType, AndroidType androidType)
		{
			AndroidType newParentType = null;
			lock (SyncObject) {
				newParentType = FindAndroidType (currentParentType, androidType.OriginalJavaName);
			}

			if (newParentType != null) {
				lock (SyncObject) {
					newParentType.AddType (androidType);
				}

				currentParentType.Types.Remove (androidType);

				if (!string.IsNullOrWhiteSpace (newParentType.FullName)) {
					if (newParentType.AndroidObjectType == AndroidObjectType.Class) {
						androidType.FullName = newParentType.FullName + "+" + androidType.Name;
					} else {
						if (androidType.AndroidObjectType == AndroidObjectType.Class) {
							androidType.FullName = newParentType.JavaName + androidType.Name;
						} else {
							androidType.FullName = newParentType.FullName + androidType.Name.Substring (1);
						}
					}
				}

				androidType.ParentObjectCorrectlyIdentified = true;
				return true;
			}

			return false;
		}

		static async Task GetAndroidType (AndroidType baseType)
		{
			var content = await baseType.Url.GetContent ();
			var pattern = "<div id=\"api-info-block\">(.|\n)*?<a href=[^>]*>([^<]*)</a>";
			var regex = new Regex (pattern, RegexOptions.Multiline);
			var match = regex.Match (content);
			baseType.ApiAdded = match.Groups [2].Value;

			var items = new []{
				new Tuple<AndroidObjectType, string>(AndroidObjectType.Interface, "<li><h2 class=\"hide-from-toc\">Interfaces</h2>(.|\\n)*?<\\/ul>"),
				new Tuple<AndroidObjectType, string>(AndroidObjectType.Class, "<li><h2 class=\"hide-from-toc\">Classes</h2>(.|\\n)*?<\\/ul>"),
				new Tuple<AndroidObjectType, string>(AndroidObjectType.Class, "<li><h2 class=\"hide-from-toc\">Exceptions</h2>(.|\\n)*?<\\/ul>"),
			};

			foreach (var item in items) {
				pattern = item.Item2;
				var androidObjectType = item.Item1;
				regex = new Regex (pattern, RegexOptions.Multiline);
				match = regex.Match (content);
				if (match.Success) {
					var itemContent = match.Groups [0].Value.Trim ();
					pattern = "<li><a href=\"([^\"]*)\">([^<]*)</a></li>";
					regex = new Regex (pattern, RegexOptions.Multiline);
					var matches = regex.Matches (itemContent);
					foreach (Match type in matches) {
						var androidType = await CreateType (new Uri (baseType.Url, type.Groups [1].Value), androidObjectType, baseType);
						if (androidType != null) {
							lock (SyncObject) {
								baseType.AddType (androidType);
							}
						}
					}
				}
			}
		}

		static async Task<AndroidType> CreateType (Uri url, AndroidObjectType androidObjectType, AndroidType baseType)
		{
			var content = await url.GetContent ();
			var originalContent = content;
			var pageContent = content;
			var pattern = "<h1 class=\"api-title\">([^<]*)<";
			var regex = new Regex (pattern, RegexOptions.Multiline);
			var match = regex.Match (content);
			var typeName = match.Groups [1].Value;
			var javaTypeName = typeName;
			var originalJavaName = typeName;

			var lastDotPosition = typeName.LastIndexOf (".");
			if (lastDotPosition != -1) {
				typeName = typeName.Substring (lastDotPosition + 1);
				javaTypeName = typeName;
			}

			XmlNode typeNode;
			if (androidObjectType == AndroidObjectType.Interface) {
				typeNode = Metadata.SelectSingleNode ($"//attr[@path=\"/api/package[@name='{baseType.JavaPackageName}']/interface[@name='{javaTypeName}']\" and @name=\"managedName\"]");
				typeName = NamingConverter.ConvertInterfaceToCSharp (typeName);
			} else {
				typeNode = Metadata.SelectSingleNode ($"//attr[@path=\"/api/package[@name='{baseType.JavaPackageName}']/class[@name='{javaTypeName}']\" and @name=\"managedName\"]");
				typeName = NamingConverter.ConvertClassToCSharp (typeName);
			}

			if (typeNode != null) {
				typeName = typeNode.InnerText;
			}

			AndroidType parentType = null;
			lock (SyncObject) {
				parentType = FindAndroidType (baseType, javaTypeName);
			}

			pattern = "<div id=\"api-info-block\">(.|\n)*?<a [^>]*>(<b>)?([^<]*)(</b>)?</a>";
			regex = new Regex (pattern, RegexOptions.Multiline);
			match = regex.Match (content);
			var apiAdded = match.Groups [3].Value;
			apiAdded = apiAdded.Replace ("Added in ", string.Empty);
			apiAdded = apiAdded.Replace ("API level ", string.Empty);
			apiAdded = apiAdded.Replace ("Android ", string.Empty);

			var androidType = new AndroidType {
				Name = typeName,
				FullName = typeName,
				JavaName = javaTypeName,
				OriginalJavaName = originalJavaName,
				PackageName = baseType.PackageName,
				JavaPackageName = baseType.JavaPackageName,
				Url = url,
				ApiAdded = apiAdded,
			};

			androidType.ParentObjectCorrectlyIdentified = !(parentType == null && originalJavaName.IndexOf (".") != -1);

			if (parentType != null) {
				if (!string.IsNullOrWhiteSpace (parentType.FullName)) {
					if (parentType.AndroidObjectType == AndroidObjectType.Class) {
						androidType.FullName = parentType.FullName + "+" + androidType.Name;
					} else {
						if (androidObjectType == AndroidObjectType.Class) {
							androidType.FullName = parentType.JavaName + androidType.Name;
						} else {
							androidType.FullName = parentType.FullName + androidType.Name.Substring (1);
						}
					}
				}
			} else {
				if (!string.IsNullOrWhiteSpace (baseType.FullName)) {
					androidType.FullName = baseType.FullName + "." + androidType.FullName;
				}
			}

			// Try to identify if it is a class of interface and assert they are the same as method has been informed on androidObjectType argument.
			pattern = "<p>([^<]*)<code class=\"api-signature\" translate=\"no\" dir=\"ltr\">([^<]*)</code>";
			regex = new Regex (pattern, RegexOptions.Multiline);
			match = regex.Match (content);
			content = match.Groups [2].Value.Replace (Environment.NewLine, string.Empty);

			pattern = "(default|private|public|protected)(.)*(class|interface)";
			regex = new Regex (pattern, RegexOptions.Multiline);
			match = regex.Match (content);

			androidType.AndroidObjectType = match.Groups [3].Value == "class" ? AndroidObjectType.Class : AndroidObjectType.Interface;
			System.Diagnostics.Debug.Assert (androidType.AndroidObjectType == androidObjectType);

			if (parentType != null) {
				lock (SyncObject) {
					parentType.AddType (androidType);
				}

				return null;
			}

			AddConstantFieldsToObject (androidType, pageContent);

			// Add Public Ctors
			AddCtors (androidType, pageContent);

			// Add methods
			AddMethods (androidType, pageContent);

			return androidType;
		}

		public static AndroidType FindAndroidType (AndroidType baseType, string originalJavaName)
		{
			if (originalJavaName.IndexOf (".") == -1) {
				return null;
			}
			var packageObject = AndroidPackages.SingleOrDefault (obj => obj.PackageName == baseType.PackageName);
			if (packageObject == null) {
				return null;
			}

			var items = originalJavaName.Split ('.');
			var androidType = packageObject.Types.SingleOrDefault (obj => obj.OriginalJavaName.Equals (items [0], StringComparison.OrdinalIgnoreCase));

			if (androidType == null) {
				if (baseType.AndroidObjectType == AndroidObjectType.Interface) {
					androidType = packageObject.Types.SingleOrDefault (obj => obj.JavaName.Equals ("I" + items [0], StringComparison.OrdinalIgnoreCase));
				}

				if (androidType == null) {
					return null;
				}
			}

			for (var i = 1; i < items.Length - 1; i++) {
				var tempAndroidType = androidType.Types.SingleOrDefault (obj => obj.JavaName.Equals (items [i], StringComparison.OrdinalIgnoreCase));
				if (tempAndroidType == null) {
					if (baseType.AndroidObjectType == AndroidObjectType.Interface) {
						tempAndroidType = androidType.Types.SingleOrDefault (obj => obj.JavaName.Equals ("I" + items [i], StringComparison.OrdinalIgnoreCase));
					}

					if (tempAndroidType == null) {
						return null;
					}
				}

				androidType = tempAndroidType;
			}

			return androidType;
		}

		static void AddConstantFieldsToObject (AndroidType androidType, string content)
		{
			var pattern = "<!-- ========= ENUM CONSTANTS DETAIL ======== -->((.|\\s)*?)<!--";
			var regex = new Regex (pattern, RegexOptions.Multiline);
			var match = regex.Match (content);
			if (!match.Success) {
				return;
			}

			content = match.Groups [1].Value;
			pattern = "(<div data-version-added=\"([^\"]*)\"(.|\n)*?)?<h3 class=\"api-name\" id=\"([^\"]*)\"((.|\n)*?)?Constant Value:((.|\n)*?)?[\\(|<]";
			regex = new Regex (pattern, RegexOptions.Multiline);
			var matches = regex.Matches (content);
			if (matches.Count == 0) {
				return;
			}

			foreach (Match field in matches) {

				if (field.Groups [5].Value.IndexOf ("public static final int") == -1) {
					continue;
				}

				var classes = androidType.JavaName.Split (".");
				var javaNamespace = androidType.AndroidObjectType == AndroidObjectType.Interface ? $"I:{androidType.JavaPackageName.Replace (".", "/")}/{classes [0]}" : $"{androidType.JavaPackageName.Replace (".", "/")}/{classes [0]}";
				for (var i = 1; i < classes.Length; i++) {
					javaNamespace += $"${classes [i]}";
				}

				var apiAdded = field.Groups [2].Value;
				var javaFieldName = field.Groups [4].Value;
				var fieldName = NamingConverter.ConvertFieldToCSharp (javaFieldName);
				var value = field.Groups [7].Value.Replace (Environment.NewLine, string.Empty).Trim ();
				if (!int.TryParse (value, out _)) {
					System.Diagnostics.Debugger.Break ();
				}

				androidType.ConstantFields.Add (new ConstantField (javaNamespace, fieldName, javaFieldName, apiAdded, value));
			}
		}

		static void AddCtors (AndroidType androidType, string content)
		{
			var originalContent = content;
			var pattern = "<h2 class=\"api-section\">Public constructors</h2>((.|\\s)*?<!-- ========= METHOD DETAIL ======== -->)";
			var regex = new Regex (pattern, RegexOptions.Multiline);
			var match = regex.Match (content);
			if (!match.Success) {
				return;
			}

			content = match.Groups [1].Value;
			var methodsContent = new List<string> ();
			var initialPosition = 0;
			while (true) {
				const string DivDataVersion = "<div data-version-added=";
				initialPosition = content.IndexOf (DivDataVersion, initialPosition);
				if (initialPosition == -1) {
					break;
				}

				var finalPosition = content.IndexOf (DivDataVersion, initialPosition + DivDataVersion.Length);
				if (finalPosition == -1) {
					finalPosition = content.IndexOf ("<!-- ========= METHOD DETAIL ======== -->", initialPosition);
				}

				if (finalPosition == -1) {
					break;
				}

				var length = finalPosition - initialPosition;
				methodsContent.Add (content.Substring (initialPosition, length));
				initialPosition = finalPosition;
			}

			foreach (var methodContent in methodsContent) {
				pattern = "(<div data-version-added=\"([^\"]*)\"(.|\n)*?)?<h3 class=\"api-name\" id=\"([^\"]*)\">([^<]*)</h3>";
				regex = new Regex (pattern, RegexOptions.Multiline);
				match = regex.Match (methodContent);
				if (!match.Success) {
					System.Diagnostics.Debugger.Break ();
				}

				var apiAdded = match.Groups [2].Value;
				if (apiAdded != Options.ApiLevel) {
					continue;
				}

				var javaFieldName = "ctor";
				var fieldName = "ctor";
				var paramValue = string.Empty;
				var arguments = new List<Tuple<string, List<string>>> ();
				var returnType = string.Empty;
				var returnValues = new List<string> ();

				const string ParametersConst = ">Parameters<";
				const string EndTag = "</table>";

				// Check Parameters
				var seeAlsoPosition = methodContent.IndexOf ("See also:", 0);
				if (seeAlsoPosition == -1) {
					seeAlsoPosition = methodContent.Length;
				}

				initialPosition = 0;
				initialPosition = methodContent.IndexOf (ParametersConst, initialPosition);
				if (initialPosition != -1 && seeAlsoPosition > initialPosition) {
					var finalPosition = methodContent.IndexOf (EndTag, initialPosition + ParametersConst.Length);
					if (finalPosition == -1) {
						// BUGBUG
						System.Diagnostics.Debugger.Break ();
					}

					if (finalPosition != -1) {
						var length = finalPosition - initialPosition;
						paramValue = methodContent.Substring (initialPosition, length);
						pattern = "<tr>((.|\\s)*?)</tr>";
						regex = new Regex (pattern, RegexOptions.Multiline);
						var matches = regex.Matches (paramValue);
						foreach (Match returnMatch in matches) {
							var argumentValue = returnMatch.Groups [1].Value;

							pattern = "<code translate=\"no\" dir=\"ltr\">((.|\\s)*?)</code>";
							regex = new Regex (pattern, RegexOptions.Multiline);
							var argDefs = regex.Matches (argumentValue);

							if (argDefs.Count < 2) {
								// BUGBUG
								System.Diagnostics.Debugger.Break ();
							}

							var argName = argDefs [0].Groups [1].Value;
							var argType = argDefs [1].Groups [1].Value;
							if (argType != "int") {
								continue;
							}

							var index = 2;

							finalPosition = argumentValue.IndexOf ("Value is", StringComparison.OrdinalIgnoreCase);
							if (finalPosition != -1) {
								argumentValue = argumentValue.Substring (finalPosition);
								regex = new Regex (pattern, RegexOptions.Multiline);
								argDefs = regex.Matches (argumentValue);
								index = 0;
							}

							var values = new List<string> ();
							for (var i = index; i < argDefs.Count; i++) {
								var argValue = argDefs [i].Groups [1].Value;
								if (argValue.IndexOf ("0") != -1) {
									values.Add ("none");
									continue;
								}

								pattern = "<a href=\"([^\"]*)\">([^<]*)</a>";
								regex = new Regex (pattern, RegexOptions.Multiline);
								var matchValue = regex.Match (argValue);
								argValue = matchValue.Groups [1].Value;

								if (!values.Contains (argValue)) {
									values.Add (argValue);
								}
							}

							if (values.Count > 0) {
								arguments.Add (new Tuple<string, List<string>> (argName, values));
							}
						}
					}
				}

				androidType.Methods.Add (new Method (fieldName, javaFieldName, apiAdded, arguments, returnType, returnValues));
			}
		}

		static void AddMethods (AndroidType androidType, string content)
		{
			var originalContent = content;
			var pattern = "<h2 class=\"api-section\">Public methods</h2>((.|\\s)*?<!-- ========= METHOD DETAIL ======== -->)";
			var regex = new Regex (pattern, RegexOptions.Multiline);
			var match = regex.Match (content);
			if (!match.Success) {
				return;
			}

			content = match.Groups [1].Value;
			var methodsContent = new List<string> ();
			var initialPosition = 0;
			while (true) {
				const string DivDataVersion = "<div data-version-added=";
				initialPosition = content.IndexOf (DivDataVersion, initialPosition);
				if (initialPosition == -1) {
					break;
				}

				var finalPosition = content.IndexOf (DivDataVersion, initialPosition + DivDataVersion.Length);
				if (finalPosition == -1) {
					finalPosition = content.IndexOf ("<!-- ========= METHOD DETAIL ======== -->", initialPosition);
				}

				if (finalPosition == -1) {
					break;
				}

				var length = finalPosition - initialPosition;
				methodsContent.Add (content.Substring (initialPosition, length));
				initialPosition = finalPosition;
			}

			foreach (var methodContent in methodsContent) {
				pattern = "(<div data-version-added=\"([^\"]*)\"(.|\n)*?)?<h3 class=\"api-name\" id=\"([^\"]*)\">([^<]*)</h3>";
				regex = new Regex (pattern, RegexOptions.Multiline);
				match = regex.Match (methodContent);
				if (!match.Success) {
					System.Diagnostics.Debugger.Break ();
				}

				var apiAdded = match.Groups [2].Value;
				if (apiAdded != Options.ApiLevel) {
					continue;
				}

				var javaFieldName = match.Groups [5].Value;
				var fieldName = NamingConverter.ConvertFieldToCSharp (javaFieldName);
				var paramValue = string.Empty;
				var arguments = new List<Tuple<string, List<string>>> ();
				var returnType = string.Empty;
				var returnValues = new List<string> ();

				const string ParametersConst = ">Parameters<";
				const string ReturnConst = ">Returns<";
				const string EndTag = "</table>";

				// Check Parameters
				var seeAlsoPosition = methodContent.IndexOf ("See also:", 0);
				if (seeAlsoPosition == -1) {
					seeAlsoPosition = methodContent.Length;
				}

				initialPosition = 0;
				initialPosition = methodContent.IndexOf (ParametersConst, initialPosition);
				if (initialPosition != -1 && seeAlsoPosition > initialPosition) {
					var finalPosition = methodContent.IndexOf (EndTag, initialPosition + ParametersConst.Length);
					if (finalPosition == -1) {
						// BUGBUG
						System.Diagnostics.Debugger.Break ();
					}

					if (finalPosition != -1) {
						var length = finalPosition - initialPosition;
						paramValue = methodContent.Substring (initialPosition, length);
						pattern = "<tr>((.|\\s)*?)</tr>";
						regex = new Regex (pattern, RegexOptions.Multiline);
						var matches = regex.Matches (paramValue);
						foreach (Match returnMatch in matches) {
							var argumentValue = returnMatch.Groups [1].Value;

							pattern = "<code translate=\"no\" dir=\"ltr\">((.|\\s)*?)</code>";
							regex = new Regex (pattern, RegexOptions.Multiline);
							var argDefs = regex.Matches (argumentValue);

							if (argDefs.Count < 2) {
								// BUGBUG
								System.Diagnostics.Debugger.Break ();
							}

							var argName = argDefs [0].Groups [1].Value;
							var argType = argDefs [1].Groups [1].Value;
							if (argType != "int") {
								continue;
							}

							var index = 2;

							finalPosition = argumentValue.IndexOf ("Value is", StringComparison.OrdinalIgnoreCase);
							if (finalPosition != -1) {
								argumentValue = argumentValue.Substring (finalPosition);
								regex = new Regex (pattern, RegexOptions.Multiline);
								argDefs = regex.Matches (argumentValue);
								index = 0;
							}

							var values = new List<string> ();
							for (var i = index; i < argDefs.Count; i++) {
								var argValue = argDefs [i].Groups [1].Value;
								if (argValue.IndexOf ("0") != -1) {
									values.Add ("none");
									continue;
								}

								pattern = "<a href=\"([^\"]*)\">([^<]*)</a>";
								regex = new Regex (pattern, RegexOptions.Multiline);
								var matchValue = regex.Match (argValue);
								argValue = matchValue.Groups [1].Value;

								if (!values.Contains (argValue)) {
									values.Add (argValue);
								}
							}

							if (values.Count > 0) {
								arguments.Add (new Tuple<string, List<string>> (argName, values));
							}
						}
					}
				}

				// Check Return
				initialPosition = 0;
				initialPosition = methodContent.IndexOf (ReturnConst, initialPosition);
				if (initialPosition != -1) {
					var finalPosition = methodContent.IndexOf (EndTag, initialPosition + ReturnConst.Length);
					if (finalPosition == -1) {
						// BUGBUG
						System.Diagnostics.Debugger.Break ();
					}

					if (finalPosition != -1) {
						var length = finalPosition - initialPosition;
						var returnValue = methodContent.Substring (initialPosition, length);

						pattern = "<code translate=\"no\" dir=\"ltr\">((.|\\s)*?)</code>";
						regex = new Regex (pattern, RegexOptions.Multiline);
						var matches = regex.Matches (returnValue);
						returnType = matches [0].Groups [1].Value;

						if (returnType == "int") {

							finalPosition = returnValue.IndexOf ("Value is", StringComparison.OrdinalIgnoreCase);
							if (finalPosition != -1) {
								returnValue = returnValue.Substring (finalPosition);
							}

							if (returnValue.IndexOf ("0</code>") != -1) {
								returnValues.Add ("none");
							}

							pattern = "><a href=\"([^\"]*)\">([^<]*)</a>";
							regex = new Regex (pattern, RegexOptions.Multiline);
							matches = regex.Matches (returnValue);
							foreach (Match returnMatch in matches) {
								returnValues.Add (returnMatch.Groups [1].Value);
							}
						}
					}
				}

				androidType.Methods.Add (new Method (fieldName, javaFieldName, apiAdded, arguments, returnType, returnValues));
			}
		}

		static Assembly CurrentDomainAssemblyResolve (object sender, ResolveEventArgs args)
		{
			// Ignore missing resources
			if (args.Name.Contains (".resources")) {
				return null;
			}

			// check for assemblies already loaded
			Assembly assembly = AppDomain.CurrentDomain.GetAssemblies ().FirstOrDefault (a => a.FullName == args.Name);
			if (assembly != null) {
				return assembly;
			}

			// Try to load by filename - split out the filename of the full assembly name
			// and append the base path of the original assembly (ie. look in the same dir)
			string filename = args.Name.Split (',') [0] + ".dll".ToLower ();
			string asmFile = Path.Combine (new FileInfo (MonoAndroidAssembly.Location).Directory.FullName, "../v1.0", filename);
			return System.Reflection.Assembly.LoadFrom (asmFile);
		}
	}
}
