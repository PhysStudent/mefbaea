using Microsoft.Xna.Framework;
using System;
using Terraria.Graphics.Shaders;
namespace Terraria.Graphics.Effects
{
	internal class Filter : GameEffect
	{
		public bool Active;
		private ScreenShaderData _shader;
		public Vector2 TargetPosition = Vector2.Zero;
		public Filter(ScreenShaderData shader, EffectPriority priority = EffectPriority.VeryLow)
		{
			this._shader = shader;
			this._priority = priority;
		}
		public void Apply()
		{
			this._shader.UseGlobalOpacity(this.Opacity);
			this._shader.UseTargetPosition(this.TargetPosition);
			this._shader.Apply();
		}
		public ScreenShaderData GetShader()
		{
			return this._shader;
		}
		internal override void Activate(Vector2 position, params object[] args)
		{
			this.TargetPosition = position;
			this.Active = true;
		}
		internal override void Deactivate(params object[] args)
		{
			this.Active = false;
		}
		public bool IsActive()
		{
			return this.Active;
		}
	}
}
