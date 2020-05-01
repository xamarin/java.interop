using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Java.Interop.BootstrapTasks
{
	public class FindJdk : Task
	{
		[Output]
		public string JdkPath { get; set; }

		public override bool Execute ()
		{
			Log.LogMessage ("Begin looking for JDK.");

			JdkPath = FindBestJdk ();

			if (string.IsNullOrWhiteSpace (JdkPath))
				Log.LogError ("Could not find Jdk");
			else {
				JdkPath = JdkPath.TrimEnd ('/', '\\') + '\\';
				Log.LogMessage ($"- JDK: {JdkPath}");
			}			

			return !Log.HasLoggedErrors;
		}

		string FindBestJdk ()
		{
			IEnumerable<string> potential_sdks = Enumerable.Empty<string> ();

			if (Path.DirectorySeparatorChar == '\\') {
				// Running on Windows
				potential_sdks = potential_sdks.Concat (GetPreferredJdkPaths ())
				.Concat (GetOpenJdkPaths ())
				.Concat (GetKnownOpenJdkPaths ())
				.Concat (GetOracleJdkPaths ())
				.Concat (GetEnvironmentJdkPaths ());
			}

			potential_sdks = potential_sdks.Concat (GetMacOSMicrosoftJdkPaths ())
				.Concat (GetJavaHomeEnvironmentJdks ())
				.Concat (GetPathJdks ());

			return potential_sdks
				.Select (p => JdkInfo.Create (p))
				.Where (p => p != null)
				.Select (p => p.JdkPath)
				.FirstOrDefault ();
		}

		static IEnumerable<string> GetPreferredJdkPaths ()
		{
			return GetPathsFromRegistry (Environment.GetEnvironmentVariable ("XAMARIN_ANDROID_REGKEY"), "JavaSdkDirectory");
		}

		static IEnumerable<string> GetOpenJdkPaths ()
		{
			return GetPathsFromRegistry (@"SOFTWARE\Microsoft\VisualStudio\Android", "JavaHome");
		}

		static IEnumerable<string> GetPathsFromRegistry (string key, string value)
		{
			var roots = new [] { RegistryEx.CurrentUser, RegistryEx.LocalMachine };
			var wows = new [] { RegistryEx.Wow64.Key32, RegistryEx.Wow64.Key64 };

			foreach (var root in roots)
			foreach (var wow in wows)
				yield return RegistryEx.GetValueString (root, key, value, wow);
		}

		static IEnumerable<string> GetKnownOpenJdkPaths ()
		{
			string JdkFolderNamePattern = "microsoft_dist_openjdk_";

			var rootPaths = new List<string> {
				Path.Combine (Environment.ExpandEnvironmentVariables ("%ProgramW6432%"), "Android", "jdk"),
				Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.ProgramFilesX86), "Android", "jdk"),
			};

			foreach (var rootPath in rootPaths) {
				if (Directory.Exists (rootPath)) {
					foreach (var directoryName in Directory.EnumerateDirectories (rootPath, $"{JdkFolderNamePattern}*").ToList ()) {
						var versionString = directoryName.Replace ($"{rootPath}\\{JdkFolderNamePattern}", string.Empty);
						if (Version.TryParse (versionString, out Version ver)) {
							yield return directoryName;
						}
					}
				}
			}
		}

		static IEnumerable<string> GetOracleJdkPaths ()
		{
			string subkey = @"SOFTWARE\JavaSoft\Java Development Kit";

			foreach (var wow64 in new [] { RegistryEx.Wow64.Key32, RegistryEx.Wow64.Key64 }) {
				string key_name = string.Format (@"{0}\{1}\{2}", "HKLM", subkey, "CurrentVersion");
				var currentVersion = RegistryEx.GetValueString (RegistryEx.LocalMachine, subkey, "CurrentVersion", wow64);

				if (!string.IsNullOrEmpty (currentVersion)) {

					// No matter what the CurrentVersion is, look for 1.6 or 1.7 or 1.8
					yield return RegistryEx.GetValueString (RegistryEx.LocalMachine, subkey + "\\" + "1.8", "JavaHome", wow64);
					yield return RegistryEx.GetValueString (RegistryEx.LocalMachine, subkey + "\\" + "1.7", "JavaHome", wow64);
					yield return RegistryEx.GetValueString (RegistryEx.LocalMachine, subkey + "\\" + "1.6", "JavaHome", wow64);
				}
			}
		}

		static IEnumerable<string> GetEnvironmentJdkPaths ()
		{
			yield return Environment.GetEnvironmentVariable ("JAVA_HOME");
		}

		static IEnumerable<string> GetMacOSMicrosoftJdkPaths ()
		{
			var jdks = AppDomain.CurrentDomain.GetData ($"GetMacOSMicrosoftJdkPaths jdks override! {typeof (JdkInfo).AssemblyQualifiedName}")
				?.ToString ();
			if (jdks == null) {
				var home = Environment.GetFolderPath (Environment.SpecialFolder.Personal);
				jdks = Path.Combine (home, "Library", "Developer", "Xamarin", "jdk");
			}
			if (!Directory.Exists (jdks))
				return Enumerable.Empty<string> ();

			return Directory.EnumerateDirectories (jdks);
		}

		static IEnumerable<string> GetJavaHomeEnvironmentJdks ()
		{
			yield return Environment.GetEnvironmentVariable ("JAVA_HOME");
		}

		 static IEnumerable<string> GetPathJdks ()
		{
			var path = Environment.GetEnvironmentVariable ("PATH");
			var pathDirs = path.Split (new char [] { Path.PathSeparator }, StringSplitOptions.RemoveEmptyEntries);

			foreach (var dir in pathDirs) {
				if (Directory.EnumerateFiles (dir, "java*").Any ())
					yield return Path.GetDirectoryName (dir);
			}
		}

		class JdkInfo
		{
			static string [] ExecutableFileExtensions;

			public string JdkPath { get; private set; }

			static JdkInfo ()
			{
				var pathExt = Environment.GetEnvironmentVariable ("PATHEXT");
				var pathExts = pathExt?.Split (new char [] { Path.PathSeparator }, StringSplitOptions.RemoveEmptyEntries) ?? new string [0];

				ExecutableFileExtensions = pathExts;
			}

			// Returns null if a valid Java isn't found at 'path'.
			public static JdkInfo Create (string path)
			{
				if (path is null || !Directory.Exists (path))
					return null;

				if (!FindExecutablesInDirectory (Path.Combine (path, "bin"), "javac").Any ())
					return null;

				return new JdkInfo { JdkPath = path };
			}

			static IEnumerable<string> FindExecutablesInDirectory (string dir, string executable)
			{
				foreach (var exe in ExecutableFiles (executable)) {
					var exePath = Path.Combine (dir, exe);
					if (File.Exists (exePath))
						yield return exePath;
				}
			}

			static IEnumerable<string> ExecutableFiles (string executable)
			{
				if (ExecutableFileExtensions == null || ExecutableFileExtensions.Length == 0) {
					yield return executable;
					yield break;
				}

				foreach (var ext in ExecutableFileExtensions)
					yield return Path.ChangeExtension (executable, ext);
				yield return executable;
			}
		}

		static class RegistryEx
		{
			const string ADVAPI = "advapi32.dll";

			public static UIntPtr CurrentUser = (UIntPtr) 0x80000001;
			public static UIntPtr LocalMachine = (UIntPtr) 0x80000002;

			[DllImport (ADVAPI, CharSet = CharSet.Unicode, SetLastError = true)]
			static extern int RegOpenKeyEx (UIntPtr hKey, string subKey, uint reserved, uint sam, out UIntPtr phkResult);

			[DllImport (ADVAPI, CharSet = CharSet.Unicode, SetLastError = true)]
			static extern int RegQueryValueExW (UIntPtr hKey, string lpValueName, int lpReserved, out uint lpType,
				StringBuilder lpData, ref uint lpcbData);

			[DllImport (ADVAPI, CharSet = CharSet.Unicode, SetLastError = true)]
			static extern int RegSetValueExW (UIntPtr hKey, string lpValueName, int lpReserved,
				uint dwType, string data, uint cbData);

			[DllImport (ADVAPI, CharSet = CharSet.Unicode, SetLastError = true)]
			static extern int RegSetValueExW (UIntPtr hKey, string lpValueName, int lpReserved,
				uint dwType, IntPtr data, uint cbData);

			[DllImport (ADVAPI, CharSet = CharSet.Unicode, SetLastError = true)]
			static extern int RegCreateKeyEx (UIntPtr hKey, string subKey, uint reserved, string @class, uint options,
				uint samDesired, IntPtr lpSecurityAttributes, out UIntPtr phkResult, out Disposition lpdwDisposition);

			[DllImport ("advapi32.dll", SetLastError = true)]
			static extern int RegCloseKey (UIntPtr hKey);

			public static string GetValueString (UIntPtr key, string subkey, string valueName, Wow64 wow64)
			{
				UIntPtr regKeyHandle;
				uint sam = (uint) Rights.QueryValue + (uint) wow64;
				if (RegOpenKeyEx (key, subkey, 0, sam, out regKeyHandle) != 0)
					return null;

				try {
					uint type;
					var sb = new StringBuilder (2048);
					uint cbData = (uint) sb.Capacity;
					if (RegQueryValueExW (regKeyHandle, valueName, 0, out type, sb, ref cbData) == 0) {
						return sb.ToString ();
					}
					return null;
				} finally {
					RegCloseKey (regKeyHandle);
				}
			}

			public static void SetValueString (UIntPtr key, string subkey, string valueName, string value, Wow64 wow64)
			{
				UIntPtr regKeyHandle;
				uint sam = (uint) (Rights.CreateSubKey | Rights.SetValue) + (uint) wow64;
				uint options = (uint) Options.NonVolatile;
				Disposition disposition;
				if (RegCreateKeyEx (key, subkey, 0, null, options, sam, IntPtr.Zero, out regKeyHandle, out disposition) != 0) {
					throw new Exception ("Could not open or craete key");
				}

				try {
					uint type = (uint) ValueType.String;
					uint lenBytesPlusNull = ((uint) value.Length + 1) * 2;
					var result = RegSetValueExW (regKeyHandle, valueName, 0, type, value, lenBytesPlusNull);
					if (result != 0)
						throw new Exception (string.Format ("Error {0} setting registry key '{1}{2}@{3}'='{4}'",
							result, key, subkey, valueName, value));
				} finally {
					RegCloseKey (regKeyHandle);
				}
			}

			[Flags]
			enum Rights : uint
			{
				None = 0,
				QueryValue = 0x0001,
				SetValue = 0x0002,
				CreateSubKey = 0x0004,
				EnumerateSubKey = 0x0008,
			}

			enum Options
			{
				BackupRestore = 0x00000004,
				CreateLink = 0x00000002,
				NonVolatile = 0x00000000,
				Volatile = 0x00000001,
			}

			public enum Wow64 : uint
			{
				Key64 = 0x0100,
				Key32 = 0x0200,
			}

			enum ValueType : uint
			{
				None = 0, //REG_NONE
				String = 1, //REG_SZ
				UnexpandedString = 2, //REG_EXPAND_SZ
				Binary = 3, //REG_BINARY
				DWord = 4, //REG_DWORD
				DWordLittleEndian = 4, //REG_DWORD_LITTLE_ENDIAN
				DWordBigEndian = 5, //REG_DWORD_BIG_ENDIAN
				Link = 6, //REG_LINK
				MultiString = 7, //REG_MULTI_SZ
				ResourceList = 8, //REG_RESOURCE_LIST
				FullResourceDescriptor = 9, //REG_FULL_RESOURCE_DESCRIPTOR
				ResourceRequirementsList = 10, //REG_RESOURCE_REQUIREMENTS_LIST
				QWord = 11, //REG_QWORD
				QWordLittleEndian = 11, //REG_QWORD_LITTLE_ENDIAN
			}

			enum Disposition : uint
			{
				CreatedNewKey = 0x00000001,
				OpenedExistingKey = 0x00000002,
			}
		}
	}
}
