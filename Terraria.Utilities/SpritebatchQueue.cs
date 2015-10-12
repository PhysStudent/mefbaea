using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Terraria.Utilities
{
	class SpritebatchQueue
	{
		public static IList<SpritebatchQueue> queue = new List<SpritebatchQueue>();
		public Texture2D t;
		public SpriteFont s;
		public string x;
		public Vector2 v;
		public Color c;
		public string type;
		public SpritebatchQueue (/*string type, object w, object x, object y, object z =null*/)
		{
			/*if (type == "texture")
			{ t = w; b = x; c = y; this.type = type; }*/
		}
		public void DrawString (SpriteFont spriteFont, string text, Vector2 vector, Color color)
		{
			s = spriteFont; x = text; v = vector; c = color; type = "string";
			queue.Add(this);
		}
		public void Draw(Texture2D texture, Vector2 vector, Color color)
		{
			t = texture; v = vector; c = color; type = "texture";
			queue.Add(this);
		}

	}
}
