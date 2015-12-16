using System.Collections.Generic;
using System.IO;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Windows.Forms;

namespace Terraria.Utilities
{
    static class ConfigHandler //I wanted to use the name "ConfigMagic" but sanity won...
    {
        private static Dictionary<string, object> writeLater;
        /// <summary>
        /// Remember to cast them...
        /// </summary>
        public static Dictionary<string, object> configOptions = new Dictionary<string, object>();

        /// <summary>
        /// Worthless file reader
        /// </summary>
        private static StreamReader fileReader = new StreamReader(Main.SavePath + "/MechmodConfig.txt", true);

        static ConfigHandler()
        {
            //Nothing to see here, move on...
        }
        public static void configFileSetup()
        {
            configOptions.Add("endlessWire", true);
            configOptions.Add("yellowWire", true); //For later
            configOptions.Add("greenWire", true);
            configOptions.Add("redWire", true);
            configOptions.Add("blueWire", true);
            configOptions.Add("camSpeed", 4);

            readConfig();
            fileReader.Close();
            foreach (KeyValuePair<string, object> entry in writeLater)
            {
                WriteConfig(entry.Key, entry.Value.ToString());
            }
        }
        public static void readConfig() //Please work!
        {
            writeLater = new Dictionary<string, object>(configOptions); //Apparently this deep-copies the dictionary even though it has objects..

            string datLineRightNaow;
            while ((datLineRightNaow = fileReader.ReadLine()) != null)
            {
                string[] currentLine = datLineRightNaow.Replace(" ", "").Split('=');
                if (configOptions.ContainsKey(currentLine[0]))
                {
                    writeLater.Remove(currentLine[0]);
                    configOptions[currentLine[0]] = currentLine[1];
                }
            }
        }
        public static void WriteConfig(string name, string value)
        {
            //A really good idea, creating and destroying an object every time the function is called
            using (StreamWriter fileWriter = new StreamWriter(Main.SavePath + "/MechmodConfig.txt", true))
            {
                fileWriter.WriteLine(name + " = " + value);
            }
        }
    }
}
