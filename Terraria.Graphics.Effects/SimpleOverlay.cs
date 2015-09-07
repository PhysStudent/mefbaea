using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.Graphics.Shaders;
namespace Terraria.Graphics.Effects
{
	internal class SimpleOverlay : Overlay
	{
		private Ref<Texture2D> _texture;
		private ScreenShaderData _shader;
		public Vector2 TargetPosition = Vector2.Zero;
		public SimpleOverlay(string textureName, ScreenShaderData shader, EffectPriority priority = EffectPriority.VeryLow) : base(priority)
		{
			this._texture = TextureManager.Retrieve((textureName == null) ? "" : textureName);
			this._shader = shader;
		}
		public SimpleOverlay(string textureName, string shaderName = "Default", EffectPriority priority = EffectPriority.VeryLow) : base(priority)
		{
			this._texture = TextureManager.Retrieve((textureName == null) ? "" : textureName);
			this._shader = new ScreenShaderData(Main.screenShader, shaderName);
		}
		public override void Draw(SpriteBatch spriteBatch)
		{
			this._shader.UseGlobalOpacity(this.Opacity);
			this._shader.UseTargetPosition(this.TargetPosition);
			this._shader.Apply();
			spriteBatch.Draw(this._texture.Value, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White);
		}
		internal override void Activate(Vector2 position, params object[] args)
		{
			this.TargetPosition = position;
			this.Mode = OverlayMode.FadeIn;
		}
		internal override void Deactivate(params object[] args)
		{
			this.Mode = OverlayMode.FadeOut;
		}
	}
}
