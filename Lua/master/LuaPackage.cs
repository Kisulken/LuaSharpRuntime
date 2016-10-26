using System.IO;
using System.Collections.Generic;

using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Core;

namespace Lua {

	public class LuaPackage {

		public struct MetaInfo {
			public string Name;
			public string Version;
			public int RuntimeVersion;
		}

		public const string META_ENTRY = "meta";

		private MemoryStream outputMemStream;
		private ZipOutputStream zipStream;

		public LuaPackage() {
			outputMemStream = new MemoryStream();
			zipStream = new ZipOutputStream(outputMemStream);
			zipStream.SetLevel(3);
		}

		public void AddData(string key, byte[] data) {
			if (zipStream.IsFinished) {
				throw new System.Exception("Package is already built but attempt to add a new data was requested!");
			}

			ZipEntry newEntry = new ZipEntry(ZipEntry.CleanName(key));
			newEntry.DateTime = System.DateTime.Now;
			newEntry.Size = data.Length;

			zipStream.PutNextEntry(newEntry);

			StreamUtils.Copy(new MemoryStream(data), zipStream, new byte[4096]);
			zipStream.CloseEntry();
		}

		public byte[] Finish(string name = "Unknown", string version = "1.0") {
			if (zipStream.IsFinished) {
				throw new System.Exception("Package is already built but another attempt to build was requested!");
			}

			string meta = name.Replace(System.Environment.NewLine, " ") + System.Environment.NewLine +
							  version + System.Environment.NewLine +
							  LuaShared.RUNTIME_VERSION.ToString();

			AddData(META_ENTRY, LuaUtils.GetBytes(meta));

			zipStream.IsStreamOwner = true;
			zipStream.Close();

			return outputMemStream.ToArray();
		}

		public static Dictionary<string, byte[]> Unpack(byte[] data) {
			var entries = new Dictionary<string, byte[]>();
			ZipFile zf = null;
			try {
				MemoryStream fs = new MemoryStream(data);
				zf = new ZipFile(fs);
				foreach (ZipEntry zipEntry in zf) {
					if (!zipEntry.IsFile)
						continue;

					byte[] buffer = new byte[4096];
					Stream zipStream = zf.GetInputStream(zipEntry);

					using (MemoryStream streamWriter = new MemoryStream()) {
						StreamUtils.Copy(zipStream, streamWriter, buffer);
						entries.Add(zipEntry.Name, streamWriter.ToArray());
					}
				}
			}
			catch (System.Exception e) {
				throw new System.InvalidOperationException("error while extracting the package due " + e.Message);
			}
			finally {
				if (zf != null) {
					zf.IsStreamOwner = true;
					zf.Close();
				}
			}

			return entries;
		}

		public static MetaInfo GetMetaInfo(byte[] meta) {
			string metaStr = LuaUtils.GetString(meta);
			var lines = metaStr.Split('\n');
			var metaStruct = new MetaInfo();
			metaStruct.Name = lines[0];
			metaStruct.Version = lines[1];
			metaStruct.RuntimeVersion = System.Convert.ToInt32(lines[2]);

			return metaStruct;
		}

		public static Dictionary<string, byte[]> UnpackFile(string filename) {
			return Unpack(File.ReadAllBytes(filename));
		}
	}
}