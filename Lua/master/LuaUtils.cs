using System.IO;
using MoonSharp.Interpreter;

public class LuaUtils {

	public static MemoryStream Compile(string code) {
		Script S = new Script(CoreModules.Preset_Complete);

		DynValue chunk = S.LoadString(code);

		var stream = new MemoryStream();
		S.Dump(chunk, stream);

		return stream;
	}

	public static Stream LoadBytecode(string path) {
		MemoryStream ms = new MemoryStream();
		using (ms) {
			using (FileStream file = new FileStream(path, FileMode.Open, FileAccess.Read)) {
				byte[] bytes = new byte[file.Length];
				file.Read(bytes, 0, (int)file.Length);
				ms.Write(bytes, 0, (int)file.Length);
			}
		}
		return ms;
	}

	public static byte[] GetBytes(string str) {
		byte[] bytes = new byte[str.Length * sizeof(char)];
		System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
		return bytes;
	}

	public static string GetString(byte[] bytes) {
		char[] chars = new char[bytes.Length / sizeof(char)];
		System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
		return new string(chars);
	}
}
