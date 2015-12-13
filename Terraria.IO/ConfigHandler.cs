using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Terraria.IO;

namespace Terraria.Utilities
{
    static class ConfigHandler //I wanted to use the name "ConfigMagic" but sanity won...
    {
        private static List<string> writeLater = new List<string>();
        /// <summary>
        /// Remember to cast them...
        /// </summary>
        static Dictionary<string, object> configOptions = new Dictionary<string, object>();

        static ConfigHandler()
        {
            configOptions.Add("endlessWire", true);
            configOptions.Add("yellowWire", true); //For later
            configOptions.Add("greenwWire", true);
            configOptions.Add("redwWire", true);
            configOptions.Add("bluewWire", true);
            configOptions.Add("camSpeed", 4);
            readConfig();
            foreach (string entry in writeLater)
            {
                WriteConfig(entry.Split('=')[0], entry.Split('=')[1]);
            }

        }
        public static void readConfig() //Please work!
        {
            using (StreamReader fileReader = new StreamReader(Main.SavePath + "/MechmodConfig.txt", true))
            {
                foreach (KeyValuePair<string, object> entry in configOptions)
                {
                    string[] currentLine = fileReader.ReadLine().Replace(" ", "").Split('=');
                    if (configOptions.ContainsKey(currentLine[0]))
                    {
                        configOptions[currentLine[0]] = currentLine[1];
                    }
                    else
                    {
                        //WriteConfig(entry.Key, entry.Value.ToString());
                        writeLater.Add(string.Join("=", currentLine));
                    }
                }
            }
        }
        public static void WriteConfig(string name, string value)
        {
            //A really good idea, creating and destroying an object evey time the function is calles
            using (StreamWriter fileWriter = new StreamWriter(Main.SavePath + "/MechmodConfig.txt", true))
            {
                fileWriter.WriteLine(name + " = " + value);
            }
        }
    }
}
