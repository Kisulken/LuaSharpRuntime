using System.Collections.Generic;
using System.IO;
using MoonSharp.Interpreter;

namespace Lua {
	public class LuaRuntime {

		private Dictionary<string, byte[]> entries;

		private Script script;

		public LuaRuntime(string filename) {
			entries = LuaPackage.UnpackFile(filename);
			script = new Script(CoreModules.Preset_Complete);

			LuaShared.sharedEntries = entries;
			LuaShared.TempMetaInfo = LuaPackage.GetMetaInfo(entries[LuaPackage.META_ENTRY]);

			if (LuaShared.TempMetaInfo.RuntimeVersion > LuaShared.RUNTIME_VERSION)
				throw new System.NotSupportedException("Package was built using higher version (" +
													   LuaShared.TempMetaInfo.RuntimeVersion.ToString()
													   + " > " + LuaShared.RUNTIME_VERSION.ToString() +
										   ") of runtime-platform than you are using right now.");
		}

		public void Run() {
			if (!entries.ContainsKey("bin/main.bin"))
				throw new System.Exception("Main entry (main.lua) was not presented!");

			var mainEntry = entries["bin/main.bin"];
			try {
				script.DoStream(new MemoryStream(mainEntry));
			}
			catch (ScriptRuntimeException e) {
				System.Console.WriteLine("Runtime error: " + e.DecoratedMessage);
			}
			catch (InternalErrorException e) {
				System.Console.WriteLine("Internal error: " + e.DecoratedMessage);
			}
			catch (InterpreterException e) {
				System.Console.WriteLine("Error: " + e.DecoratedMessage);
			}
			catch (System.Exception e) {
				System.Console.WriteLine("Error: " + e.Message);
			}
		}
	}
}
