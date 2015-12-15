using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace Terraria.Utilities
{
    static class ConfigHandler //I wanted to use the name "ConfigMagic" but sanity won...
    {
        private static List<string> writeLater = new List<string>();
        /// <summary>
        /// Remember to cast them...
        /// </summary>
        public static Dictionary<string, object> configOptions = new Dictionary<string, object>();

        public static void configFileSetup()
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
                string datLineRightNaow;
                while ((datLineRightNaow = fileReader.ReadLine()) != null)
                {
                    string[] currentLine = datLineRightNaow.Replace(" ", "").Split('=');
                    if (configOptions.ContainsKey(currentLine[0]))
                    {
                        configOptions[currentLine[0]] = currentLine[1];
                    }
                    else
                    {
                        //MessageBox.Show(string.Join("=", currentLine), "config");
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
