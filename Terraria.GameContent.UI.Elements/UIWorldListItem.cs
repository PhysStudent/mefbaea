using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.Graphics;
using Terraria.IO;
using Terraria.Social;
using Terraria.UI;
namespace Terraria.GameContent.UI.Elements
{
	internal class UIWorldListItem : UIPanel
	{
		private WorldFileData _data;
		private Texture2D _dividerTexture;
		private Texture2D _innerPanelTexture;
		private UIImage _worldIcon;
		private UIText _buttonLabel;
		private UIText _deleteButtonLabel;
		private Texture2D _buttonCloudActiveTexture;
		private Texture2D _buttonCloudInactiveTexture;
		private Texture2D _buttonFavoriteActiveTexture;
		private Texture2D _buttonFavoriteInactiveTexture;
		private Texture2D _buttonPlayTexture;
		private Texture2D _buttonDeleteTexture;
		private UIImageButton _deleteButton;
		public bool IsFavorite
		{
			get
			{
				return this._data.IsFavorite;
			}
		}
		public UIWorldListItem(WorldFileData data)
		{
			this.BorderColor = new Color(89, 116, 213) * 0.7f;
			this._dividerTexture = TextureManager.Load("Images/UI/Divider");
			this._innerPanelTexture = TextureManager.Load("Images/UI/InnerPanelBackground");
			this._buttonCloudActiveTexture = TextureManager.Load("Images/UI/ButtonCloudActive");
			this._buttonCloudInactiveTexture = TextureManager.Load("Images/UI/ButtonCloudInactive");
			this._buttonFavoriteActiveTexture = TextureManager.Load("Images/UI/ButtonFavoriteActive");
			this._buttonFavoriteInactiveTexture = TextureManager.Load("Images/UI/ButtonFavoriteInactive");
			this._buttonPlayTexture = TextureManager.Load("Images/UI/ButtonPlay");
			this._buttonDeleteTexture = TextureManager.Load("Images/UI/ButtonDelete");
			this.Height.Set(96f, 0f);
			this.Width.Set(0f, 1f);
			base.SetPadding(6f);
			this._data = data;
			this._worldIcon = new UIImage(this.GetIcon());
			this._worldIcon.Left.Set(4f, 0f);
			this._worldIcon.OnDoubleClick += new UIElement.MouseEvent(this.PlayGame);
			base.Append(this._worldIcon);
			UIImageButton uIImageButton = new UIImageButton(this._buttonPlayTexture);
			uIImageButton.VAlign = 1f;
			uIImageButton.Left.Set(4f, 0f);
			uIImageButton.OnClick += new UIElement.MouseEvent(this.PlayGame);
			base.OnDoubleClick += new UIElement.MouseEvent(this.PlayGame);
			uIImageButton.OnMouseOver += new UIElement.MouseEvent(this.PlayMouseOver);
			uIImageButton.OnMouseOut += new UIElement.MouseEvent(this.ButtonMouseOut);
			base.Append(uIImageButton);
			UIImageButton uIImageButton2 = new UIImageButton(this._data.IsFavorite ? this._buttonFavoriteActiveTexture : this._buttonFavoriteInactiveTexture);
			uIImageButton2.VAlign = 1f;
			uIImageButton2.Left.Set(28f, 0f);
			uIImageButton2.OnClick += new UIElement.MouseEvent(this.FavoriteButtonClick);
			uIImageButton2.OnMouseOver += new UIElement.MouseEvent(this.FavoriteMouseOver);
			uIImageButton2.OnMouseOut += new UIElement.MouseEvent(this.ButtonMouseOut);
			uIImageButton2.SetVisibility(1f, this._data.IsFavorite ? 0.8f : 0.4f);
			base.Append(uIImageButton2);
			if (SocialAPI.Cloud != null)
			{
				UIImageButton uIImageButton3 = new UIImageButton(this._data.IsCloudSave ? this._buttonCloudActiveTexture : this._buttonCloudInactiveTexture);
				uIImageButton3.VAlign = 1f;
				uIImageButton3.Left.Set(52f, 0f);
				uIImageButton3.OnClick += new UIElement.MouseEvent(this.CloudButtonClick);
				uIImageButton3.OnMouseOver += new UIElement.MouseEvent(this.CloudMouseOver);
				uIImageButton3.OnMouseOut += new UIElement.MouseEvent(this.ButtonMouseOut);
				base.Append(uIImageButton3);
			}
			UIImageButton uIImageButton4 = new UIImageButton(this._buttonDeleteTexture);
			uIImageButton4.VAlign = 1f;
			uIImageButton4.HAlign = 1f;
			uIImageButton4.OnClick += new UIElement.MouseEvent(this.DeleteButtonClick);
			uIImageButton4.OnMouseOver += new UIElement.MouseEvent(this.DeleteMouseOver);
			uIImageButton4.OnMouseOut += new UIElement.MouseEvent(this.DeleteMouseOut);
			this._deleteButton = uIImageButton4;
			if (!this._data.IsFavorite)
			{
				base.Append(uIImageButton4);
			}
			this._buttonLabel = new UIText("", 1f, false);
			this._buttonLabel.VAlign = 1f;
			this._buttonLabel.Left.Set(80f, 0f);
			this._buttonLabel.Top.Set(-3f, 0f);
			base.Append(this._buttonLabel);
			this._deleteButtonLabel = new UIText("", 1f, false);
			this._deleteButtonLabel.VAlign = 1f;
			this._deleteButtonLabel.HAlign = 1f;
			this._deleteButtonLabel.Left.Set(-30f, 0f);
			this._deleteButtonLabel.Top.Set(-3f, 0f);
			base.Append(this._deleteButtonLabel);
		}
		private Texture2D GetIcon()
		{
			return TextureManager.Load("Images/UI/Icon" + (this._data.IsHardMode ? "Hallow" : "") + (this._data.HasCorruption ? "Corruption" : "Crimson"));
		}
		private void FavoriteMouseOver(UIMouseEvent evt, UIElement listeningElement)
		{
			if (this._data.IsFavorite)
			{
				this._buttonLabel.SetText("Unfavorite");
				return;
			}
			this._buttonLabel.SetText("Favorite");
		}
		private void CloudMouseOver(UIMouseEvent evt, UIElement listeningElement)
		{
			if (this._data.IsCloudSave)
			{
				this._buttonLabel.SetText("Move off cloud");
				return;
			}
			this._buttonLabel.SetText("Move to cloud");
		}
		private void PlayMouseOver(UIMouseEvent evt, UIElement listeningElement)
		{
			this._buttonLabel.SetText("Play");
		}
		private void DeleteMouseOver(UIMouseEvent evt, UIElement listeningElement)
		{
			this._deleteButtonLabel.SetText("Delete");
		}
		private void DeleteMouseOut(UIMouseEvent evt, UIElement listeningElement)
		{
			this._deleteButtonLabel.SetText("");
		}
		private void ButtonMouseOut(UIMouseEvent evt, UIElement listeningElement)
		{
			this._buttonLabel.SetText("");
		}
		private void CloudButtonClick(UIMouseEvent evt, UIElement listeningElement)
		{
			if (this._data.IsCloudSave)
			{
				this._data.MoveToLocal();
			}
			else
			{
				this._data.MoveToCloud();
			}
			((UIImageButton)evt.Target).SetImage(this._data.IsCloudSave ? this._buttonCloudActiveTexture : this._buttonCloudInactiveTexture);
			if (this._data.IsCloudSave)
			{
				this._buttonLabel.SetText("Move off cloud");
				return;
			}
			this._buttonLabel.SetText("Move to cloud");
		}
		private void DeleteButtonClick(UIMouseEvent evt, UIElement listeningElement)
		{
			for (int i = 0; i < Main.WorldList.Count; i++)
			{
				if (Main.WorldList[i] == this._data)
				{
					Main.PlaySound(10, -1, -1, 1);
					Main.selectedWorld = i;
					Main.menuMode = 9;
					return;
				}
			}
		}
		private void PlayGame(UIMouseEvent evt, UIElement listeningElement)
		{
			if (listeningElement != evt.Target)
			{
				return;
			}
			this._data.SetAsActive();
			Main.PlaySound(10, -1, -1, 1);
			Main.GetInputText("");
			if (Main.menuMultiplayer && SocialAPI.Network != null)
			{
				Main.menuMode = 889;
			}
			else if (Main.menuMultiplayer)
			{
				Main.menuMode = 30;
			}
			else
			{
				Main.menuMode = 10;
			}
			if (!Main.menuMultiplayer)
			{
				WorldGen.playWorld();
			}
		}
		private void FavoriteButtonClick(UIMouseEvent evt, UIElement listeningElement)
		{
			this._data.ToggleFavorite();
			((UIImageButton)evt.Target).SetImage(this._data.IsFavorite ? this._buttonFavoriteActiveTexture : this._buttonFavoriteInactiveTexture);
			((UIImageButton)evt.Target).SetVisibility(1f, this._data.IsFavorite ? 0.8f : 0.4f);
			if (this._data.IsFavorite)
			{
				this._buttonLabel.SetText("Unfavorite");
				base.RemoveChild(this._deleteButton);
			}
			else
			{
				this._buttonLabel.SetText("Favorite");
				base.Append(this._deleteButton);
			}
			UIList uIList = this.Parent.Parent as UIList;
			if (uIList != null)
			{
				uIList.UpdateOrder();
			}
		}
		public override int CompareTo(object obj)
		{
			UIWorldListItem uIWorldListItem = obj as UIWorldListItem;
			if (uIWorldListItem == null)
			{
				return base.CompareTo(obj);
			}
			if (this.IsFavorite && !uIWorldListItem.IsFavorite)
			{
				return -1;
			}
			if (!this.IsFavorite && uIWorldListItem.IsFavorite)
			{
				return 1;
			}
			return this._data.Name.CompareTo(uIWorldListItem._data.Name);
		}
		public override void MouseOver(UIMouseEvent evt)
		{
			base.MouseOver(evt);
			this.BackgroundColor = new Color(73, 94, 171);
			this.BorderColor = new Color(89, 116, 213);
		}
		public override void MouseOut(UIMouseEvent evt)
		{
			base.MouseOut(evt);
			this.BackgroundColor = new Color(63, 82, 151) * 0.7f;
			this.BorderColor = new Color(89, 116, 213) * 0.7f;
		}
		private void DrawPanel(SpriteBatch spriteBatch, Vector2 position, float width)
		{
			spriteBatch.Draw(this._innerPanelTexture, position, new Rectangle?(new Rectangle(0, 0, 8, this._innerPanelTexture.Height)), Color.White);
			spriteBatch.Draw(this._innerPanelTexture, new Vector2(position.X + 8f, position.Y), new Rectangle?(new Rectangle(8, 0, 8, this._innerPanelTexture.Height)), Color.White, 0f, Vector2.Zero, new Vector2((width - 16f) / 8f, 1f), SpriteEffects.None, 0f);
			spriteBatch.Draw(this._innerPanelTexture, new Vector2(position.X + width - 8f, position.Y), new Rectangle?(new Rectangle(16, 0, 8, this._innerPanelTexture.Height)), Color.White);
		}
		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			base.DrawSelf(spriteBatch);
			CalculatedStyle innerDimensions = base.GetInnerDimensions();
			CalculatedStyle dimensions = this._worldIcon.GetDimensions();
			float num = dimensions.X + dimensions.Width;
			Color color = this._data.IsValid ? Color.White : Color.Red;
			Utils.DrawBorderString(spriteBatch, this._data.Name, new Vector2(num + 6f, dimensions.Y - 2f), color, 1f, 0f, 0f, -1);
			spriteBatch.Draw(this._dividerTexture, new Vector2(num, innerDimensions.Y + 21f), null, Color.White, 0f, Vector2.Zero, new Vector2((base.GetDimensions().X + base.GetDimensions().Width - num) / 8f, 1f), SpriteEffects.None, 0f);
			Vector2 vector = new Vector2(num + 6f, innerDimensions.Y + 29f);
			float num2 = 80f;
			this.DrawPanel(spriteBatch, vector, num2);
			string text = this._data.IsExpertMode ? "Expert" : "Normal";
			float x = Main.fontMouseText.MeasureString(text).X;
			float x2 = num2 * 0.5f - x * 0.5f;
			Utils.DrawBorderString(spriteBatch, text, vector + new Vector2(x2, 3f), this._data.IsExpertMode ? new Color(217, 143, 244) : Color.White, 1f, 0f, 0f, -1);
			vector.X += num2 + 5f;
			float num3 = 140f;
			this.DrawPanel(spriteBatch, vector, num3);
			string text2 = this._data.WorldSizeName + " World";
			float x3 = Main.fontMouseText.MeasureString(text2).X;
			float x4 = num3 * 0.5f - x3 * 0.5f;
			Utils.DrawBorderString(spriteBatch, text2, vector + new Vector2(x4, 3f), Color.White, 1f, 0f, 0f, -1);
			vector.X += num3 + 5f;
			float num4 = innerDimensions.X + innerDimensions.Width - vector.X;
			this.DrawPanel(spriteBatch, vector, num4);
			string text3 = "Created: " + this._data.CreationTime.ToString("d MMMM yyyy");
			float x5 = Main.fontMouseText.MeasureString(text3).X;
			float x6 = num4 * 0.5f - x5 * 0.5f;
			Utils.DrawBorderString(spriteBatch, text3, vector + new Vector2(x6, 3f), Color.White, 1f, 0f, 0f, -1);
			vector.X += num4 + 5f;
		}
	}
}