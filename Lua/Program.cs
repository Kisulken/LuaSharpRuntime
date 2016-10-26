using System;
using System.IO;
using MoonSharp.Interpreter;

namespace Lua {

	class MainClass {

		private const int EXIT_CODE_INVALID_ARGUMENTS = 1;
		private const int EXIT_CODE_INVALID_FILE = 2;
		private const int EXIT_CODE_GENERIC_ERROR = 3;
		private const int EXIT_CODE_ENGINE_ERROR = 4;

		public static void Main(string[] args) {
			if (args.Length < 2) {
				if (args[0] == "version") {
					Console.WriteLine(LuaClassStd.GetCredentials());
					return;
				}
				Console.WriteLine("Invalid arguments!");
				Environment.Exit(EXIT_CODE_INVALID_ARGUMENTS);
				return;
			}

			// Script engine initialization
			try {
				Script.DefaultOptions.UseLuaErrorLocations = true;
				Script.DefaultOptions.ScriptLoader = new LuaScriptLoader() {
					ModulePaths = new string[] { "static:?" }
				};

				UserData.RegisterAssembly();

				Script.WarmUp();
			}
			catch (Exception e) {
				Console.WriteLine("Error while initializing the engine due " + e.Message);
				Environment.Exit(EXIT_CODE_ENGINE_ERROR);
			}

			// Command line options
			if (args[0] == "run") {
				if (!File.Exists(args[1])) {
					Console.WriteLine("File " + args[1] + " does not exist!");
					Environment.Exit(EXIT_CODE_INVALID_FILE);
				}
				try {
					new LuaRuntime(args[1]).Run();
				}
				catch (NotSupportedException e) {
					Console.WriteLine("Package is not supported. " + e.Message);
				}
				return;
			}

			try {

				var packageFilename = args[0];
				string[] files = new string[args.Length - 1];

				bool mainEntryFound = false;

				for (int i = 1; i < args.Length; i++) {
					if (Path.GetFileName(args[i]) == "main.lua")
						mainEntryFound = true;

					files[i - 1] = args[i];
				}

				if (!mainEntryFound)
					Console.WriteLine("WARNING! Main entry (main.lua) was not found.");

				var package = new LuaPackage();

				foreach (var filename in files) {
					if (!File.Exists(filename)) {
						Console.WriteLine("File " + filename + " does not exist!");
						Environment.Exit(EXIT_CODE_INVALID_FILE);
						return;
					}

					var content = File.ReadAllText(filename);
					var bytecode = LuaUtils.Compile(content).ToArray();
					package.AddData("bin/" + Path.GetFileNameWithoutExtension(filename) + ".bin", bytecode);
				}

				File.WriteAllBytes(packageFilename, package.Finish());

				Console.WriteLine("Packaging completed.");
			}
			catch (Exception e) {
				Console.WriteLine("Error while packing the files due " + e.Message);
				Environment.Exit(EXIT_CODE_GENERIC_ERROR);
			}
		}
	}
}
