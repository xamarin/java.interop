using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using Xamarin.Android.Tools.Enums;
using Xamarin.Android.Tools.Fields;
using Xamarin.Android.Tools.ObjectModel;

namespace Xamarin.Android.Tools
{
	class Program
	{
		static int Main (string [] args)
		{
			var rootCommand = new RootCommand();

			rootCommand.AddCommand(FieldsValidator.CliCommandConfiguration());
            		rootCommand.AddCommand(EnumGenerator.CliCommandConfiguration());

			rootCommand.Name = "generator-tools";
			rootCommand.Description = "Provide checks and helpers to the code generation process.";

			try {
				return rootCommand.InvokeAsync (args).Result;
			} catch (Exception e) {
				Console.ForegroundColor = ConsoleColor.Red;
				Console.Write (e.Message);
				Console.ForegroundColor = ConsoleColor.White;
				return 1;
			}
		}
	}
}
