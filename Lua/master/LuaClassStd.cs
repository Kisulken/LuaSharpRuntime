using System;
using MoonSharp.Interpreter;

namespace Lua {

	[MoonSharpUserData]
	public class LuaClassStd {

		public static string GetMetaInfo() {
			return LuaShared.TempMetaInfo.Name + ", " + LuaShared.TempMetaInfo.Version;
		}

		public static string GetCredentials() {
			return "LuaSharpRuntime 1.0 (" + LuaShared.RUNTIME_VERSION.ToString() + ")" + Environment.NewLine +
										"Copyright (C) 2016 Daniil Furmanov" + Environment.NewLine +
										"https://github.com/Oxygend/LuaSharpRuntime" + Environment.NewLine +
										Environment.NewLine + Script.GetBanner() + Environment.NewLine +
										"ICSharpCode SharpZipLib, v0.86.0, David Pierson";
		}

		public static string GetLuaVersion() {
			return Script.LUA_VERSION;
		}

	}
}
