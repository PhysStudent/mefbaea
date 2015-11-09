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
		public SpriteBatch(GraphicsDevice gd) : base(gd)
		{
		} 
		public new void Draw(Texture2D texture, Vector2 pos, Rectangle? rect, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects fx, float layer)
		{
			Vector2 newScale = scale * Main.zoomLevel;
			base.Draw(texture, pos, rect, color, rotation, origin, newScale, fx, layer);
		}
		public new void Draw(Texture2D texture, Vector2 pos, Rectangle? rect, Color color, float rotation, Vector2 origin, float scale, SpriteEffects fx, float layer)
		{
			float newScale = scale * Main.zoomLevel;
			base.Draw(texture, pos, rect, color, rotation, origin, newScale, fx, layer);
		}

		public new void Draw(Texture2D texture, Vector2 pos, Color color)
		{
			float newScale = Main.zoomLevel;
			base.Draw(texture, pos, null, color, 0, Vector2.Zero, newScale, SpriteEffects.None, 0);
		}
		public new void Draw(Texture2D texture, Rectangle pos, Rectangle? rect, Color color, float rotation, Vector2 origin, SpriteEffects fx, float layer)
		{
			float newScale = Main.zoomLevel;
			base.Draw(texture, new Vector2(pos.X, pos.Y), rect, color, 0, Vector2.Zero, newScale, SpriteEffects.None, 0);
		}
		public new void Draw(Texture2D texture, Rectangle pos, Rectangle? rect, Color color)
		{
			//float newScale = Main.zoomLevel;
			this.Draw(texture, pos, rect, color, 0, Vector2.Zero, SpriteEffects.None, 0);
		}
		public new void Draw(Texture2D texture, Rectangle pos, Color color)
		{
			//float newScale = Main.zoomLevel;
			this.Draw(texture, pos, null, color);
		}

	}
}
