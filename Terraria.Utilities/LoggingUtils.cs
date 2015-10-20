#define LOG

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;


namespace Terraria.Utilities
{
    static class LoggingUtils
	{
		public static String writePath = Environment.ExpandEnvironmentVariables(@"%USERPROFILE%\Desktop\Terraria Mod\MEFBEA\Terraria.v1.3.0.8\Logs\");
        static LoggingUtils()
        {
            System.IO.Directory.CreateDirectory(writePath);
        }
        public static void PrintToFile(Exception e, String path, bool full = true, bool lines = true)
		{
            //writepath += path;
            String pathWeGonnaWriteTo = writePath + path;
			String stringWeGonnaWriteWith = "";
			StackTrace st = new StackTrace(true);
			stringWeGonnaWriteWith += "framecount: " + st.FrameCount.ToString() + "\r\n";
			for (int i = 0; i < st.FrameCount; i++)
			{
				StackFrame sf = st.GetFrame(i);
				stringWeGonnaWriteWith += sf.GetMethod().Name + " -- " + sf.GetFileName() + " line:" +sf.GetFileLineNumber() + " col:" + sf.GetFileColumnNumber() + "\r\n";
			}
			stringWeGonnaWriteWith += ("\r\n\r\n");
			stringWeGonnaWriteWith += ("\tmsg+env: \r\n" + e.Message + Environment.StackTrace + "\r\n");
			log(stringWeGonnaWriteWith, pathWeGonnaWriteTo);
		}
        public static bool log(String str, String path = "")
        {
            if (path == "")
                path = writePath + "unknown.txt";
#if(LOG)
            using (StreamWriter file =
                new StreamWriter(path, true))
            {
                file.WriteLine(str);
           }
#endif
            return true;
		}
	}
}
