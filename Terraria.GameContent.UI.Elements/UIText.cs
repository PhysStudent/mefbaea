using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.UI;
namespace Terraria.GameContent.UI.Elements
{
	internal class UIText : UIElement
	{
		private string _text = "";
		private float _textScale = 1f;
		private Vector2 _textSize = Vector2.Zero;
		private bool _isLarge;
		public UIText(string text, float textScale = 1f, bool large = false)
		{
			this.SetText(text, textScale, large);
		}
		public override void Recalculate()
		{
			this.SetText(this._text, this._textScale, this._isLarge);
			base.Recalculate();
		}
		public void SetText(string text)
		{
			this.SetText(text, this._textScale, this._isLarge);
		}
		public void SetText(string text, float textScale, bool large)
		{
			SpriteFont spriteFont = large ? Main.fontDeathText : Main.fontMouseText;
			Vector2 textSize = new Vector2(spriteFont.MeasureString(text).X, large ? 32f : 16f) * textScale;
			this._text = text;
			this._textScale = textScale;
			this._textSize = textSize;
			this._isLarge = large;
			this.MinWidth.Set(textSize.X + this.PaddingLeft + this.PaddingRight, 0f);
			this.MinHeight.Set(textSize.Y + this.PaddingTop + this.PaddingBottom, 0f);
		}
		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			base.DrawSelf(spriteBatch);
			CalculatedStyle innerDimensions = base.GetInnerDimensions();
			Vector2 pos = innerDimensions.Position();
			if (this._isLarge)
			{
				pos.Y -= 10f * this._textScale;
			}
			else
			{
				pos.Y -= 2f * this._textScale;
			}
			pos.X += (innerDimensions.Width - this._textSize.X) * 0.5f;
			if (this._isLarge)
			{
				Utils.DrawBorderStringBig(spriteBatch, this._text, pos, Color.White, this._textScale, 0f, 0f, -1);
				return;
			}
			Utils.DrawBorderString(spriteBatch, this._text, pos, Color.White, this._textScale, 0f, 0f, -1);
		}
	}
}
