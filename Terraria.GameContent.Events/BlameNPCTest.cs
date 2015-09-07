using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria.UI.Chat;
namespace Terraria.GameContent.Events
{
	internal class BlameNPCTest
	{
		public static Dictionary<int, int> npcTypes = new Dictionary<int, int>();
		public static List<KeyValuePair<int, int>> mostSeen = new List<KeyValuePair<int, int>>();
		public static void Update(int newEntry)
		{
			if (BlameNPCTest.npcTypes.ContainsKey(newEntry))
			{
				Dictionary<int, int> dictionary;
				(dictionary = BlameNPCTest.npcTypes)[newEntry] = dictionary[newEntry] + 1;
			}
			else
			{
				BlameNPCTest.npcTypes[newEntry] = 1;
			}
			BlameNPCTest.mostSeen = BlameNPCTest.npcTypes.ToList<KeyValuePair<int, int>>();
			BlameNPCTest.mostSeen.Sort((KeyValuePair<int, int> x, KeyValuePair<int, int> y) => x.Value.CompareTo(y.Value));
		}
		public static void Draw(SpriteBatch sb)
		{
			if (Main.netDiag || Main.showFrameRate)
			{
				return;
			}
			for (int i = 0; i < BlameNPCTest.mostSeen.Count; i++)
			{
				int num = 200 + i % 13 * 100;
				int num2 = 200 + i / 13 * 30;
				ChatManager.DrawColorCodedString(sb, Main.fontItemStack, string.Concat(new object[]
				{
					BlameNPCTest.mostSeen[i].Key,
					" (",
					BlameNPCTest.mostSeen[i].Value,
					")"
				}), new Vector2((float)num, (float)num2), Color.White, 0f, Vector2.Zero, Vector2.One, -1f, false);
			}
		}
	}
}
