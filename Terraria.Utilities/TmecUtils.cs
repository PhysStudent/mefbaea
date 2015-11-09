#define LOG

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;


namespace Terraria.Utilities
{
    static class TmecUtils
	{
		public static String writePath = Environment.ExpandEnvironmentVariables(@"%USERPROFILE%\Desktop\Terraria Mod\MEFBEA\Terraria.v1.3.0.8\Logs\");
        static TmecUtils()
        {
            System.IO.Directory.CreateDirectory(writePath);
        }
        public static void PrintToFile(Exception e, String path, bool full = true, bool lines = true)
		{
#if (!LOG)
			return;
#endif
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

		public static bool placeSet(String[] textArray)
		{
			if (textArray.Length < 4)
			{
				Main.NewText("Invalid format. /place <type> <direction> <count> [wire|actuator|force]", 213, 0, 0, true);
				return false;
			}
			int count;
			if (Main.placePosition.X == 0f && Main.placePosition.Y == 0f)
				Main.placePosition = new Vector2((int)(Math.Floor(Main.player[Main.myPlayer].position.X / 16)), (int)(Math.Floor(Main.player[Main.myPlayer].position.Y / 16) + 3)); //get players pos as tile
			if (!int.TryParse(textArray[1], out Main.blockType) || !int.TryParse(textArray[3], out count)) //if type or count aren't a number, fail
			{
				Main.NewText("Invalid format. /place <type> <direction> <count> [wire|actuator|force]", 213, 0, 0);
				return false;
			}

			bool forceIt = false;
			bool wire = false;
			bool actu = false;
			int slope = 0;
			try
			{
				for (int i = 3; i < textArray.Length; i++)
				{
					switch (textArray[i])
					{
						case "force":
						case "f":
							forceIt = true;
							Main.NewText("force", 0, 0, 213);
							break;
						case "wire":
						case "w":
							wire = true;
							break;
						case "actuator":
						case "act":
						case "a":
							actu = true;
							break;
					}

					if (textArray[i][0] == 'h')
					{
						Main.NewText(textArray[i].Substring(1, 2) + textArray[i], 0, 213, 0);
						if (!int.TryParse(textArray[i].Substring(1, 2), out slope) || slope <= 5)
						{
							Main.NewText("Error: Hoik format: h1|h5" + slope.ToString(), 213, 0, 0, false);
						}
					}

				}
			}
			catch { }
			Main.NewText("/place " + Main.blockType.ToString() + " " + count.ToString() + " - " + wire.ToString() + actu.ToString() + forceIt.ToString() + "" + slope.ToString(), 0, 213, 0, true);

			Vector2 coords = new Vector2(0, 0);
			for (int i = 0; i <= count; i++)
			{
				switch (textArray[2])
				{
					case "left":
						coords = new Vector2((int)Main.placePosition.X - i, (int)Main.placePosition.Y);
						break;
					case "right":
						coords = new Vector2((int)Main.placePosition.X + i, (int)Main.placePosition.Y);
						break;
					case "down":
						coords = new Vector2((int)Main.placePosition.X, (int)Main.placePosition.Y + i);
						break;
					case "up":
						coords = new Vector2((int)Main.placePosition.X, (int)Main.placePosition.Y - i);
						break;
				}
				//	Main.NewText("/place " + Main.blockType.ToString() + " " + textArray[3] + " " + count.ToString() + " - " + wire.ToString() + actu.ToString() + forceIt.ToString(), 0, 213, 0);
				//	Main.NewText("coords: " + coords.ToString(), 255, 213, 0);

				WorldGen.PlaceTile((int)coords.X, (int)coords.Y, Main.blockType, false, forceIt);
				if (wire)
					WorldGen.PlaceWire((int)coords.X, (int)coords.Y);
				if (actu)
					WorldGen.PlaceActuator((int)coords.X, (int)coords.Y);
			}

			//doing it again to avoid it murdering everything
			if (slope != 0)
			{
				WorldGen.gen = true;
				for (int i = 0; i <= count; i++)
				{
					switch (textArray[2])
					{
						case "left":
							coords = new Vector2((int)Main.placePosition.X - i, (int)Main.placePosition.Y);
							break;
						case "right":
							coords = new Vector2((int)Main.placePosition.X + i, (int)Main.placePosition.Y);
							break;
						case "down":
							coords = new Vector2((int)Main.placePosition.X, (int)Main.placePosition.Y + i);
							break;
						case "up":
							coords = new Vector2((int)Main.placePosition.X, (int)Main.placePosition.Y - i);
							break;
					}
					//WorldGen.SlopeTile((int)coords.X, (int)coords.Y, slope);
					Main.tile[(int)coords.X, (int)coords.Y].slope((byte)slope);
					Main.NewText(slope.ToString());
				}
				WorldGen.gen = false;
			}
			return true;
		}
	}
}
