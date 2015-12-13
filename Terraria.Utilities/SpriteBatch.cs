using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpriteBatchX = Microsoft.Xna.Framework.Graphics.SpriteBatch;
namespace Terraria.Utilities
{
	public class SpriteBatch : SpriteBatchX
	{
		public bool useIntegerCoords;
		public SpriteBatch(GraphicsDevice gd) : base(gd)
		{
			useIntegerCoords = false;
		}
		public new void Draw(Texture2D texture, Vector2 pos, Rectangle? rect, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects fx, float layer)
		{
			pos.X = (float)Math.Round(pos.X);
			pos.Y = (float)Math.Round(pos.Y);
			base.Draw(texture, pos, rect, color, rotation, origin, scale, fx, layer);
		}
		public new void Draw(Texture2D texture, Vector2 pos, Rectangle? rect, Color color, float rotation, Vector2 origin, float scale, SpriteEffects fx, float layer)
		{
			pos.X = (float)Math.Round(pos.X);
			pos.Y = (float)Math.Round(pos.Y);
			base.Draw(texture, pos, rect, color, rotation, origin, scale, fx, layer);
		}

		public new void Draw(Texture2D texture, Vector2 pos, Color color)
		{
			pos.X = (float)Math.Round(pos.X);
			pos.Y = (float)Math.Round(pos.Y);
			base.Draw(texture, pos, color);
		}
		public new void Draw(Texture2D texture, Rectangle pos, Rectangle? rect, Color color)
		{
			this.Draw(texture, pos, rect, color, 0, Vector2.Zero, SpriteEffects.None, 0);
		}
		public new void Draw(Texture2D texture, Rectangle pos, Color color)
		{
			this.Draw(texture, pos, null, color);
		}
	}
}