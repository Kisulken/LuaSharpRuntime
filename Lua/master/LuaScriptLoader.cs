using MoonSharp.Interpreter.Loaders;
using MoonSharp.Interpreter;

namespace Lua {
	public class LuaScriptLoader: ScriptLoaderBase {
		public override object LoadFile(string file, Table globalContext) {
			if (file.StartsWith("static:", System.StringComparison.CurrentCulture)) {
				var module = file.Substring(7);
				switch (module) {
				case "std":
					globalContext["Std"] = typeof(LuaClassStd);
					break;
				}
			}
			else return (LuaShared.sharedEntries.ContainsKey("bin/" + file + ".bin")) ? LuaShared.sharedEntries["bin/" + file + ".bin"] : null;

			return "";
		}

		public override bool ScriptFileExists(string name) {
			return true;
		}
	}
}