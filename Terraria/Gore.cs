using Microsoft.Xna.Framework;
using System;
using Terraria.GameContent;
namespace Terraria
{
	public class Gore
	{
		public static int goreTime = 600;
		public Vector2 position;
		public Vector2 velocity;
		public float rotation;
		public float scale;
		public int alpha;
		public int type;
		public float light;
		public bool active;
		public bool sticky = true;
		public int timeLeft = Gore.goreTime;
		public bool behindTiles;
		public byte frame;
		public byte frameCounter;
		public byte numFrames = 1;
		public void Update()
		{
			if (Main.netMode == 2)
			{
				return;
			}
			if (this.active)
			{
				if (this.type >= 276 && this.type <= 282)
				{
					this.velocity.X = this.velocity.X * 0.98f;
					this.velocity.Y = this.velocity.Y * 0.98f;
					if (this.velocity.Y < this.scale)
					{
						this.velocity.Y = this.velocity.Y + 0.05f;
					}
					if ((double)this.velocity.Y > 0.1)
					{
						if (this.velocity.X > 0f)
						{
							this.rotation += 0.01f;
						}
						else
						{
							this.rotation -= 0.01f;
						}
					}
				}
				if (this.type >= 570 && this.type <= 572)
				{
					this.scale -= 0.001f;
					if ((double)this.scale <= 0.01)
					{
						this.scale = 0.01f;
						Gore.goreTime = 0;
					}
					this.sticky = false;
					this.rotation = this.velocity.X * 0.1f;
				}
				else if (this.type >= 706 && this.type <= 717)
				{
					if ((double)this.position.Y < Main.worldSurface * 16.0 + 8.0)
					{
						this.alpha = 0;
					}
					else
					{
						this.alpha = 100;
					}
					int num = 4;
					this.frameCounter += 1;
					if (this.frame <= 4)
					{
						int num2 = (int)(this.position.X / 16f);
						int num3 = (int)(this.position.Y / 16f) - 1;
						if (WorldGen.InWorld(num2, num3, 0) && !Main.tile[num2, num3].active())
						{
							this.active = false;
						}
						if (this.frame == 0)
						{
							num = 24 + Main.rand.Next(256);
						}
						if (this.frame == 1)
						{
							num = 24 + Main.rand.Next(256);
						}
						if (this.frame == 2)
						{
							num = 24 + Main.rand.Next(256);
						}
						if (this.frame == 3)
						{
							num = 24 + Main.rand.Next(96);
						}
						if (this.frame == 5)
						{
							num = 16 + Main.rand.Next(64);
						}
						if (this.type == 716)
						{
							num *= 2;
						}
						if (this.type == 717)
						{
							num *= 4;
						}
						if ((int)this.frameCounter >= num)
						{
							this.frameCounter = 0;
							this.frame += 1;
							if (this.frame == 5)
							{
								int num4 = Gore.NewGore(this.position, this.velocity, this.type, 1f);
								Main.gore[num4].frame = 9;
								Main.gore[num4].velocity *= 0f;
							}
						}
					}
					else if (this.frame <= 6)
					{
						num = 8;
						if (this.type == 716)
						{
							num *= 2;
						}
						if (this.type == 717)
						{
							num *= 3;
						}
						if ((int)this.frameCounter >= num)
						{
							this.frameCounter = 0;
							this.frame += 1;
							if (this.frame == 7)
							{
								this.active = false;
							}
						}
					}
					else if (this.frame <= 9)
					{
						num = 6;
						if (this.type == 716)
						{
							num = (int)((double)num * 1.5);
							this.velocity.Y = this.velocity.Y + 0.175f;
						}
						else if (this.type == 717)
						{
							num *= 2;
							this.velocity.Y = this.velocity.Y + 0.15f;
						}
						else
						{
							this.velocity.Y = this.velocity.Y + 0.2f;
						}
						if ((double)this.velocity.Y < 0.5)
						{
							this.velocity.Y = 0.5f;
						}
						if (this.velocity.Y > 12f)
						{
							this.velocity.Y = 12f;
						}
						if ((int)this.frameCounter >= num)
						{
							this.frameCounter = 0;
							this.frame += 1;
						}
						if (this.frame > 9)
						{
							this.frame = 7;
						}
					}
					else
					{
						if (this.type == 716)
						{
							num *= 2;
						}
						else if (this.type == 717)
						{
							num *= 6;
						}
						this.velocity.Y = this.velocity.Y + 0.1f;
						if ((int)this.frameCounter >= num)
						{
							this.frameCounter = 0;
							this.frame += 1;
						}
						this.velocity *= 0f;
						if (this.frame > 14)
						{
							this.active = false;
						}
					}
				}
				else if (this.type == 11 || this.type == 12 || this.type == 13 || this.type == 61 || this.type == 62 || this.type == 63 || this.type == 99 || this.type == 220 || this.type == 221 || this.type == 222 || (this.type >= 375 && this.type <= 377) || (this.type >= 435 && this.type <= 437) || (this.type >= 861 && this.type <= 862))
				{
					this.velocity.Y = this.velocity.Y * 0.98f;
					this.velocity.X = this.velocity.X * 0.98f;
					this.scale -= 0.007f;
					if ((double)this.scale < 0.1)
					{
						this.scale = 0.1f;
						this.alpha = 255;
					}
				}
				else if (this.type == 16 || this.type == 17)
				{
					this.velocity.Y = this.velocity.Y * 0.98f;
					this.velocity.X = this.velocity.X * 0.98f;
					this.scale -= 0.01f;
					if ((double)this.scale < 0.1)
					{
						this.scale = 0.1f;
						this.alpha = 255;
					}
				}
				else if (this.type == 331)
				{
					this.alpha += 5;
					this.velocity.Y = this.velocity.Y * 0.95f;
					this.velocity.X = this.velocity.X * 0.95f;
					this.rotation = this.velocity.X * 0.1f;
				}
				else if (this.type != 860 && this.type != 892 && this.type != 893 && (this.type < 825 || this.type > 827) && (this.type < 411 || this.type > 430))
				{
					this.velocity.Y = this.velocity.Y + 0.2f;
				}
				this.rotation += this.velocity.X * 0.1f;
				if (this.type >= 580 && this.type <= 582)
				{
					this.rotation = 0f;
					this.velocity.X = this.velocity.X * 0.95f;
				}
				if (this.type >= 825 && this.type <= 827)
				{
					if (this.timeLeft < 60)
					{
						this.alpha += Main.rand.Next(1, 7);
					}
					else if (this.alpha > 100)
					{
						this.alpha -= Main.rand.Next(1, 4);
					}
					if (this.alpha < 0)
					{
						this.alpha = 0;
					}
					if (this.alpha > 255)
					{
						this.timeLeft = 0;
					}
					this.velocity.X = (this.velocity.X * 50f + Main.windSpeed * 2f + (float)Main.rand.Next(-10, 11) * 0.1f) / 51f;
					float num5 = 0f;
					if (this.velocity.X < 0f)
					{
						num5 = this.velocity.X * 0.2f;
					}
					this.velocity.Y = (this.velocity.Y * 50f + -0.35f + num5 + (float)Main.rand.Next(-10, 11) * 0.2f) / 51f;
					this.rotation = this.velocity.X * 0.6f;
					float num6 = -1f;
					if (Main.goreLoaded[this.type])
					{
						Rectangle rectangle = new Rectangle((int)this.position.X, (int)this.position.Y, (int)((float)Main.goreTexture[this.type].Width * this.scale), (int)((float)Main.goreTexture[this.type].Height * this.scale));
						for (int i = 0; i < 255; i++)
						{
							if (Main.player[i].active && !Main.player[i].dead)
							{
								Rectangle value = new Rectangle((int)Main.player[i].position.X, (int)Main.player[i].position.Y, Main.player[i].width, Main.player[i].height);
								if (rectangle.Intersects(value))
								{
									this.timeLeft = 0;
									num6 = Main.player[i].velocity.Length();
									break;
								}
							}
						}
					}
					if (this.timeLeft > 0)
					{
						if (Main.rand.Next(2) == 0)
						{
							this.timeLeft--;
						}
						if (Main.rand.Next(50) == 0)
						{
							this.timeLeft -= 5;
						}
						if (Main.rand.Next(100) == 0)
						{
							this.timeLeft -= 10;
						}
					}
					else
					{
						this.alpha = 255;
						if (Main.goreLoaded[this.type] && num6 != -1f)
						{
							float num7 = (float)Main.goreTexture[this.type].Width * this.scale * 0.8f;
							float x = this.position.X;
							float y = this.position.Y;
							float num8 = (float)Main.goreTexture[this.type].Width * this.scale;
							float num9 = (float)Main.goreTexture[this.type].Height * this.scale;
							int num10 = 31;
							int num11 = 0;
							while ((float)num11 < num7)
							{
								int num12 = Dust.NewDust(new Vector2(x, y), (int)num8, (int)num9, num10, 0f, 0f, 0, default(Color), 1f);
								Main.dust[num12].velocity *= (1f + num6) / 3f;
								Main.dust[num12].noGravity = true;
								Main.dust[num12].alpha = 100;
								Main.dust[num12].scale = this.scale;
								num11++;
							}
						}
					}
				}
				if (this.type >= 411 && this.type <= 430)
				{
					this.alpha = 50;
					this.velocity.X = (this.velocity.X * 50f + Main.windSpeed * 2f + (float)Main.rand.Next(-10, 11) * 0.1f) / 51f;
					this.velocity.Y = (this.velocity.Y * 50f + -0.25f + (float)Main.rand.Next(-10, 11) * 0.2f) / 51f;
					this.rotation = this.velocity.X * 0.3f;
					if (Main.goreLoaded[this.type])
					{
						Rectangle rectangle2 = new Rectangle((int)this.position.X, (int)this.position.Y, (int)((float)Main.goreTexture[this.type].Width * this.scale), (int)((float)Main.goreTexture[this.type].Height * this.scale));
						for (int j = 0; j < 255; j++)
						{
							if (Main.player[j].active && !Main.player[j].dead)
							{
								Rectangle value2 = new Rectangle((int)Main.player[j].position.X, (int)Main.player[j].position.Y, Main.player[j].width, Main.player[j].height);
								if (rectangle2.Intersects(value2))
								{
									this.timeLeft = 0;
								}
							}
						}
						if (Collision.SolidCollision(this.position, (int)((float)Main.goreTexture[this.type].Width * this.scale), (int)((float)Main.goreTexture[this.type].Height * this.scale)))
						{
							this.timeLeft = 0;
						}
					}
					if (this.timeLeft > 0)
					{
						if (Main.rand.Next(2) == 0)
						{
							this.timeLeft--;
						}
						if (Main.rand.Next(50) == 0)
						{
							this.timeLeft -= 5;
						}
						if (Main.rand.Next(100) == 0)
						{
							this.timeLeft -= 10;
						}
					}
					else
					{
						this.alpha = 255;
						if (Main.goreLoaded[this.type])
						{
							float num13 = (float)Main.goreTexture[this.type].Width * this.scale * 0.8f;
							float x2 = this.position.X;
							float y2 = this.position.Y;
							float num14 = (float)Main.goreTexture[this.type].Width * this.scale;
							float num15 = (float)Main.goreTexture[this.type].Height * this.scale;
							int num16 = 176;
							if (this.type >= 416 && this.type <= 420)
							{
								num16 = 177;
							}
							if (this.type >= 421 && this.type <= 425)
							{
								num16 = 178;
							}
							if (this.type >= 426 && this.type <= 430)
							{
								num16 = 179;
							}
							int num17 = 0;
							while ((float)num17 < num13)
							{
								int num18 = Dust.NewDust(new Vector2(x2, y2), (int)num14, (int)num15, num16, 0f, 0f, 0, default(Color), 1f);
								Main.dust[num18].noGravity = true;
								Main.dust[num18].alpha = 100;
								Main.dust[num18].scale = this.scale;
								num17++;
							}
						}
					}
				}
				else if (this.type != 860 && this.type != 892 && this.type != 893)
				{
					if (this.type >= 706 && this.type <= 717)
					{
						if (this.type == 716)
						{
							float num19 = 0.6f;
							if (this.frame == 0)
							{
								num19 *= 0.1f;
							}
							else if (this.frame == 1)
							{
								num19 *= 0.2f;
							}
							else if (this.frame == 2)
							{
								num19 *= 0.3f;
							}
							else if (this.frame == 3)
							{
								num19 *= 0.4f;
							}
							else if (this.frame == 4)
							{
								num19 *= 0.5f;
							}
							else if (this.frame == 5)
							{
								num19 *= 0.4f;
							}
							else if (this.frame == 6)
							{
								num19 *= 0.2f;
							}
							else if (this.frame <= 9)
							{
								num19 *= 0.5f;
							}
							else if (this.frame == 10)
							{
								num19 *= 0.5f;
							}
							else if (this.frame == 11)
							{
								num19 *= 0.4f;
							}
							else if (this.frame == 12)
							{
								num19 *= 0.3f;
							}
							else if (this.frame == 13)
							{
								num19 *= 0.2f;
							}
							else if (this.frame == 14)
							{
								num19 *= 0.1f;
							}
							else
							{
								num19 = 0f;
							}
							float r = 1f * num19;
							float g = 0.5f * num19;
							float b = 0.1f * num19;
							Lighting.AddLight(this.position + new Vector2(8f, 8f), r, g, b);
						}
						Vector2 value3 = this.velocity;
						this.velocity = Collision.TileCollision(this.position, this.velocity, 16, 14, false, false, 1);
						if (this.velocity != value3)
						{
							if (this.frame < 10)
							{
								this.frame = 10;
								this.frameCounter = 0;
								if (this.type != 716 && this.type != 717)
								{
									Main.PlaySound(39, (int)this.position.X + 8, (int)this.position.Y + 8, Main.rand.Next(2));
								}
							}
						}
						else if (Collision.WetCollision(this.position + this.velocity, 16, 14))
						{
							if (this.frame < 10)
							{
								this.frame = 10;
								this.frameCounter = 0;
								if (this.type != 716 && this.type != 717)
								{
									Main.PlaySound(39, (int)this.position.X + 8, (int)this.position.Y + 8, 2);
								}
							}
							int num20 = (int)(this.position.X + 8f) / 16;
							int num21 = (int)(this.position.Y + 14f) / 16;
							if (Main.tile[num20, num21] != null && Main.tile[num20, num21].liquid > 0)
							{
								this.velocity *= 0f;
								this.position.Y = (float)(num21 * 16 - (int)(Main.tile[num20, num21].liquid / 16));
							}
						}
					}
					else if (this.sticky)
					{
						int num22 = 32;
						if (Main.goreLoaded[this.type])
						{
							num22 = Main.goreTexture[this.type].Width;
							if (Main.goreTexture[this.type].Height < num22)
							{
								num22 = Main.goreTexture[this.type].Height;
							}
						}
						num22 = (int)((float)num22 * 0.9f);
						this.velocity = Collision.TileCollision(this.position, this.velocity, (int)((float)num22 * this.scale), (int)((float)num22 * this.scale), false, false, 1);
						if (this.velocity.Y == 0f)
						{
							this.velocity.X = this.velocity.X * 0.97f;
							if ((double)this.velocity.X > -0.01 && (double)this.velocity.X < 0.01)
							{
								this.velocity.X = 0f;
							}
						}
						if (this.timeLeft > 0)
						{
							this.timeLeft--;
						}
						else
						{
							this.alpha++;
						}
					}
					else
					{
						this.alpha += 2;
					}
				}
				if (this.type == 860 || this.type == 892 || this.type == 893)
				{
					if (this.velocity.Y < 0f)
					{
						Vector2 vector = new Vector2(this.velocity.X, 0.6f);
						int num23 = 32;
						if (Main.goreLoaded[this.type])
						{
							num23 = Main.goreTexture[this.type].Width;
							if (Main.goreTexture[this.type].Height < num23)
							{
								num23 = Main.goreTexture[this.type].Height;
							}
						}
						num23 = (int)((float)num23 * 0.9f);
						vector = Collision.TileCollision(this.position, vector, (int)((float)num23 * this.scale), (int)((float)num23 * this.scale), false, false, 1);
						vector.X *= 0.97f;
						if ((double)vector.X > -0.01 && (double)vector.X < 0.01)
						{
							vector.X = 0f;
						}
						if (this.timeLeft > 0)
						{
							this.timeLeft--;
						}
						else
						{
							this.alpha++;
						}
						this.velocity.X = vector.X;
					}
					else
					{
						this.velocity.Y = this.velocity.Y + 0.05235988f;
						Vector2 vector2 = new Vector2(Vector2.UnitY.RotatedBy((double)this.velocity.Y, default(Vector2)).X * 2f, Math.Abs(Vector2.UnitY.RotatedBy((double)this.velocity.Y, default(Vector2)).Y) * 3f);
						vector2 *= 2f;
						int num24 = 32;
						if (Main.goreLoaded[this.type])
						{
							num24 = Main.goreTexture[this.type].Width;
							if (Main.goreTexture[this.type].Height < num24)
							{
								num24 = Main.goreTexture[this.type].Height;
							}
						}
						Vector2 value4 = vector2;
						vector2 = Collision.TileCollision(this.position, vector2, (int)((float)num24 * this.scale), (int)((float)num24 * this.scale), false, false, 1);
						if (vector2 != value4)
						{
							this.velocity.Y = -1f;
						}
						this.position += vector2;
						this.rotation = vector2.ToRotation() + 3.14159274f;
						if (this.timeLeft > 0)
						{
							this.timeLeft--;
						}
						else
						{
							this.alpha++;
						}
					}
				}
				else
				{
					this.position += this.velocity;
				}
				if (this.alpha >= 255)
				{
					this.active = false;
				}
				if (this.light > 0f)
				{
					float num25 = this.light * this.scale;
					float num26 = this.light * this.scale;
					float num27 = this.light * this.scale;
					if (this.type == 16)
					{
						num27 *= 0.3f;
						num26 *= 0.8f;
					}
					else if (this.type == 17)
					{
						num26 *= 0.6f;
						num25 *= 0.3f;
					}
					if (Main.goreLoaded[this.type])
					{
						Lighting.AddLight((int)((this.position.X + (float)Main.goreTexture[this.type].Width * this.scale / 2f) / 16f), (int)((this.position.Y + (float)Main.goreTexture[this.type].Height * this.scale / 2f) / 16f), num25, num26, num27);
						return;
					}
					Lighting.AddLight((int)((this.position.X + 32f * this.scale / 2f) / 16f), (int)((this.position.Y + 32f * this.scale / 2f) / 16f), num25, num26, num27);
				}
			}
		}
		public static int NewGore(Vector2 Position, Vector2 Velocity, int Type, float Scale = 1f)
		{
			if (Main.netMode == 2)
			{
				return 500;
			}
			if (Main.gamePaused)
			{
				return 500;
			}
			if (Main.rand == null)
			{
				Main.rand = new Random();
			}
			int num = 500;
			for (int i = 0; i < 500; i++)
			{
				if (!Main.gore[i].active)
				{
					num = i;
					break;
				}
			}
			if (num == 500)
			{
				return num;
			}
			Main.gore[num].numFrames = 1;
			Main.gore[num].frame = 0;
			Main.gore[num].frameCounter = 0;
			Main.gore[num].behindTiles = false;
			Main.gore[num].light = 0f;
			Main.gore[num].position = Position;
			Main.gore[num].velocity = Velocity;
			Gore expr_C9_cp_0 = Main.gore[num];
			expr_C9_cp_0.velocity.Y = expr_C9_cp_0.velocity.Y - (float)Main.rand.Next(10, 31) * 0.1f;
			Gore expr_F6_cp_0 = Main.gore[num];
			expr_F6_cp_0.velocity.X = expr_F6_cp_0.velocity.X + (float)Main.rand.Next(-20, 21) * 0.1f;
			Main.gore[num].type = Type;
			Main.gore[num].active = true;
			Main.gore[num].alpha = 0;
			Main.gore[num].rotation = 0f;
			Main.gore[num].scale = Scale;
			if (!ChildSafety.Disabled && ChildSafety.DangerousGore(Type))
			{
				Main.gore[num].type = Main.rand.Next(11, 14);
				Main.gore[num].scale = Main.rand.NextFloat() * 0.5f + 0.5f;
				Main.gore[num].velocity /= 2f;
			}
			if (Gore.goreTime == 0 || Type == 11 || Type == 12 || Type == 13 || Type == 16 || Type == 17 || Type == 61 || Type == 62 || Type == 63 || Type == 99 || Type == 220 || Type == 221 || Type == 222 || Type == 435 || Type == 436 || Type == 437 || (Type >= 861 && Type <= 862))
			{
				Main.gore[num].sticky = false;
			}
			else if (Type >= 375 && Type <= 377)
			{
				Main.gore[num].sticky = false;
				Main.gore[num].alpha = 100;
			}
			else
			{
				Main.gore[num].sticky = true;
				Main.gore[num].timeLeft = Gore.goreTime;
			}
			if (Type >= 706 && Type <= 717)
			{
				Main.gore[num].numFrames = 15;
				Main.gore[num].behindTiles = true;
				Main.gore[num].timeLeft = Gore.goreTime * 3;
			}
			if (Type == 16 || Type == 17)
			{
				Main.gore[num].alpha = 100;
				Main.gore[num].scale = 0.7f;
				Main.gore[num].light = 1f;
			}
			if (Type >= 570 && Type <= 572)
			{
				Main.gore[num].velocity = Velocity;
			}
			if (Type == 860 || Type == 892 || Type == 893)
			{
				Main.gore[num].velocity = new Vector2((Main.rand.NextFloat() - 0.5f) * 3f, Main.rand.NextFloat() * 6.28318548f);
			}
			if (Type >= 411 && Type <= 430 && Main.goreLoaded[Type])
			{
				Main.gore[num].position.X = Position.X - (float)(Main.goreTexture[Type].Width / 2) * Scale;
				Main.gore[num].position.Y = Position.Y - (float)Main.goreTexture[Type].Height * Scale;
				Gore expr_3F5_cp_0 = Main.gore[num];
				expr_3F5_cp_0.velocity.Y = expr_3F5_cp_0.velocity.Y * ((float)Main.rand.Next(90, 150) * 0.01f);
				Gore expr_425_cp_0 = Main.gore[num];
				expr_425_cp_0.velocity.X = expr_425_cp_0.velocity.X * ((float)Main.rand.Next(40, 90) * 0.01f);
				int num2 = Main.rand.Next(4) * 5;
				Main.gore[num].type += num2;
				Main.gore[num].timeLeft = Main.rand.Next(Gore.goreTime / 2, Gore.goreTime * 2);
			}
			if (Type >= 825 && Type <= 827)
			{
				Main.gore[num].sticky = false;
				if (Main.goreLoaded[Type])
				{
					Main.gore[num].alpha = 150;
					Main.gore[num].velocity = Velocity;
					Main.gore[num].position.X = Position.X - (float)(Main.goreTexture[Type].Width / 2) * Scale;
					Main.gore[num].position.Y = Position.Y - (float)Main.goreTexture[Type].Height * Scale / 2f;
					Main.gore[num].timeLeft = Main.rand.Next(Gore.goreTime / 2, Gore.goreTime + 1);
				}
			}
			return num;
		}
		public Color GetAlpha(Color newColor)
		{
			float num = (float)(255 - this.alpha) / 255f;
			int r;
			int g;
			int b;
			if (this.type == 16 || this.type == 17)
			{
				r = (int)newColor.R;
				g = (int)newColor.G;
				b = (int)newColor.B;
			}
			else
			{
				if (this.type == 716)
				{
					return new Color(255, 255, 255, 200);
				}
				if (this.type >= 570 && this.type <= 572)
				{
					byte b2 = (byte)(255 - this.alpha);
					return new Color((int)b2, (int)b2, (int)b2, (int)(b2 / 2));
				}
				if (this.type == 331)
				{
					return new Color(255, 255, 255, 50);
				}
				r = (int)((float)newColor.R * num);
				g = (int)((float)newColor.G * num);
				b = (int)((float)newColor.B * num);
			}
			int num2 = (int)newColor.A - this.alpha;
			if (num2 < 0)
			{
				num2 = 0;
			}
			if (num2 > 255)
			{
				num2 = 255;
			}
			return new Color(r, g, b, num2);
		}
	}
}
