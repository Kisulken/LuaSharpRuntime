using System;
using System.Collections.Generic;

namespace Lua {
	public class LuaShared {
		public const int RUNTIME_VERSION = 0;
		public static Dictionary<string, byte[]> sharedEntries;
		public static LuaPackage.MetaInfo TempMetaInfo;
	}
}
