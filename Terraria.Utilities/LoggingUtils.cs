using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Terraria.Utilities
{
	static class LoggingUtils
	{//%USERPROFILE%\Desktop\Terraria Mod\MEFBEA\Terraria.v1.3.0.8\
		public static string writePath = @".\Logs\";

		public static void PrintToFile(Exception e, string path, bool full = true, bool lines = true)
		{
			
			//writepath += path;
			string pathWeGonnaWriteTo = writePath + path;
			string stringWeGonnaWriteWith = "";
			
			stringWeGonnaWriteWith += ("\r\n\r\n");
			stringWeGonnaWriteWith += ("msg+env: \r\n" + e.Message + e.StackTrace + "\r\n");
			log(stringWeGonnaWriteWith, pathWeGonnaWriteTo);
		}
		public static bool log (string str, string path="")
		{
			if (path == "")
				path = writePath+"unknown.txt";
			string invalidChars = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
			foreach (char c in invalidChars)
			{
				path = path.Replace(c.ToString(), ""); // or with "."
			}
			using (StreamWriter file =
				new StreamWriter(path, true))
			{
				file.WriteLine(str);
			}
			return true;
		}
	}
}
