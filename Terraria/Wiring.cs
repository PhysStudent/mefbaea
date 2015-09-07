using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria.DataStructures;
using Terraria.ID;
namespace Terraria
{
	public static class Wiring
	{
		private const int MaxPump = 20;
		private const int MaxMech = 1000;
		public static bool running;
		private static Dictionary<Point16, bool> _wireSkip;
		private static DoubleStack<Point16> _wireList;
		private static Dictionary<Point16, byte> _toProcess;
		private static Vector2[] _teleport;
		private static int[] _inPumpX;
		private static int[] _inPumpY;
		private static int _numInPump;
		private static int[] _outPumpX;
		private static int[] _outPumpY;
		private static int _numOutPump;
		private static int[] _mechX;
		private static int[] _mechY;
		private static int _numMechs;
		private static int[] _mechTime;
		public static void Initialize()
		{
			Wiring._wireSkip = new Dictionary<Point16, bool>();
			Wiring._wireList = new DoubleStack<Point16>(1024, 0);
			Wiring._toProcess = new Dictionary<Point16, byte>();
			Wiring._inPumpX = new int[20];
			Wiring._inPumpY = new int[20];
			Wiring._outPumpX = new int[20];
			Wiring._outPumpY = new int[20];
			Wiring._teleport = new Vector2[2];
			Wiring._mechX = new int[1000];
			Wiring._mechY = new int[1000];
			Wiring._mechTime = new int[1000];
		}
		public static void SkipWire(int x, int y)
		{
			Wiring._wireSkip[new Point16(x, y)] = true;
		}
		public static void SkipWire(Point16 point)
		{
			Wiring._wireSkip[point] = true;
		}
		public static void UpdateMech()
		{
			for (int i = Wiring._numMechs - 1; i >= 0; i--)
			{
				Wiring._mechTime[i]--;
				if (Main.tile[Wiring._mechX[i], Wiring._mechY[i]].active() && Main.tile[Wiring._mechX[i], Wiring._mechY[i]].type == 144)
				{
					if (Main.tile[Wiring._mechX[i], Wiring._mechY[i]].frameY == 0)
					{
						Wiring._mechTime[i] = 0;
					}
					else
					{
						int num = (int)(Main.tile[Wiring._mechX[i], Wiring._mechY[i]].frameX / 18);
						if (num == 0)
						{
							num = 60;
						}
						else if (num == 1)
						{
							num = 180;
						}
						else if (num == 2)
						{
							num = 300;
						}
						if (Math.IEEERemainder((double)Wiring._mechTime[i], (double)num) == 0.0)
						{
							Wiring._mechTime[i] = 18000;
							Wiring.TripWire(Wiring._mechX[i], Wiring._mechY[i], 1, 1);
						}
					}
				}
				if (Wiring._mechTime[i] <= 0)
				{
					if (Main.tile[Wiring._mechX[i], Wiring._mechY[i]].active() && Main.tile[Wiring._mechX[i], Wiring._mechY[i]].type == 144)
					{
						Main.tile[Wiring._mechX[i], Wiring._mechY[i]].frameY = 0;
						NetMessage.SendTileSquare(-1, Wiring._mechX[i], Wiring._mechY[i], 1);
					}
					if (Main.tile[Wiring._mechX[i], Wiring._mechY[i]].active() && Main.tile[Wiring._mechX[i], Wiring._mechY[i]].type == 411)
					{
						Tile tile = Main.tile[Wiring._mechX[i], Wiring._mechY[i]];
						int num2 = (int)(tile.frameX % 36 / 18);
						int num3 = (int)(tile.frameY % 36 / 18);
						int num4 = Wiring._mechX[i] - num2;
						int num5 = Wiring._mechY[i] - num3;
						int num6 = 36;
						if (Main.tile[num4, num5].frameX >= 36)
						{
							num6 = -36;
						}
						for (int j = num4; j < num4 + 2; j++)
						{
							for (int k = num5; k < num5 + 2; k++)
							{
								Main.tile[j, k].frameX = (short)((int)Main.tile[j, k].frameX + num6);
							}
						}
						NetMessage.SendTileSquare(-1, num4, num5, 2);
					}
					for (int l = i; l < Wiring._numMechs; l++)
					{
						Wiring._mechX[l] = Wiring._mechX[l + 1];
						Wiring._mechY[l] = Wiring._mechY[l + 1];
						Wiring._mechTime[l] = Wiring._mechTime[l + 1];
					}
					Wiring._numMechs--;
				}
			}
		}
		public static void HitSwitch(int i, int j)
		{
			if (!WorldGen.InWorld(i, j, 0))
			{
				return;
			}
			if (Main.tile[i, j] == null)
			{
				return;
			}
			if (Main.tile[i, j].type == 135 || Main.tile[i, j].type == 314)
			{
				Main.PlaySound(28, i * 16, j * 16, 0);
				Wiring.TripWire(i, j, 1, 1);
				return;
			}
			if (Main.tile[i, j].type == 136)
			{
				if (Main.tile[i, j].frameY == 0)
				{
					Main.tile[i, j].frameY = 18;
				}
				else
				{
					Main.tile[i, j].frameY = 0;
				}
				Main.PlaySound(28, i * 16, j * 16, 0);
				Wiring.TripWire(i, j, 1, 1);
				return;
			}
			if (Main.tile[i, j].type == 144)
			{
				if (Main.tile[i, j].frameY == 0)
				{
					Main.tile[i, j].frameY = 18;
					if (Main.netMode != 1)
					{
						Wiring.CheckMech(i, j, 18000);
					}
				}
				else
				{
					Main.tile[i, j].frameY = 0;
				}
				Main.PlaySound(28, i * 16, j * 16, 0);
				return;
			}
			if (Main.tile[i, j].type == 132 || Main.tile[i, j].type == 411)
			{
				short num = 36;
				int num2 = (int)(Main.tile[i, j].frameX / 18 * -1);
				int num3 = (int)(Main.tile[i, j].frameY / 18 * -1);
				num2 %= 4;
				if (num2 < -1)
				{
					num2 += 2;
					num = -36;
				}
				num2 += i;
				num3 += j;
				if (Main.netMode != 1 && Main.tile[num2, num3].type == 411)
				{
					Wiring.CheckMech(num2, num3, 60);
				}
				for (int k = num2; k < num2 + 2; k++)
				{
					for (int l = num3; l < num3 + 2; l++)
					{
						if (Main.tile[k, l].type == 132 || Main.tile[k, l].type == 411)
						{
							Tile expr_235 = Main.tile[k, l];
							expr_235.frameX += num;
						}
					}
				}
				WorldGen.TileFrame(num2, num3, false, false);
				Main.PlaySound(28, i * 16, j * 16, 0);
				Wiring.TripWire(num2, num3, 2, 2);
			}
		}
		private static bool CheckMech(int i, int j, int time)
		{
			for (int k = 0; k < Wiring._numMechs; k++)
			{
				if (Wiring._mechX[k] == i && Wiring._mechY[k] == j)
				{
					return false;
				}
			}
			if (Wiring._numMechs < 999)
			{
				Wiring._mechX[Wiring._numMechs] = i;
				Wiring._mechY[Wiring._numMechs] = j;
				Wiring._mechTime[Wiring._numMechs] = time;
				Wiring._numMechs++;
				return true;
			}
			return false;
		}
		private static void XferWater()
		{
			for (int i = 0; i < Wiring._numInPump; i++)
			{
				int num = Wiring._inPumpX[i];
				int num2 = Wiring._inPumpY[i];
				int liquid = (int)Main.tile[num, num2].liquid;
				if (liquid > 0)
				{
					bool flag = Main.tile[num, num2].lava();
					bool flag2 = Main.tile[num, num2].honey();
					for (int j = 0; j < Wiring._numOutPump; j++)
					{
						int num3 = Wiring._outPumpX[j];
						int num4 = Wiring._outPumpY[j];
						int liquid2 = (int)Main.tile[num3, num4].liquid;
						if (liquid2 < 255)
						{
							bool flag3 = Main.tile[num3, num4].lava();
							bool flag4 = Main.tile[num3, num4].honey();
							if (liquid2 == 0)
							{
								flag3 = flag;
								flag4 = flag2;
							}
							if (flag == flag3 && flag2 == flag4)
							{
								int num5 = liquid;
								if (num5 + liquid2 > 255)
								{
									num5 = 255 - liquid2;
								}
								Tile expr_102 = Main.tile[num3, num4];
								expr_102.liquid += (byte)num5;
								Tile expr_11E = Main.tile[num, num2];
								expr_11E.liquid -= (byte)num5;
								liquid = (int)Main.tile[num, num2].liquid;
								Main.tile[num3, num4].lava(flag);
								Main.tile[num3, num4].honey(flag2);
								WorldGen.SquareTileFrame(num3, num4, true);
								if (Main.tile[num, num2].liquid == 0)
								{
									Main.tile[num, num2].lava(false);
									WorldGen.SquareTileFrame(num, num2, true);
									break;
								}
							}
						}
					}
					WorldGen.SquareTileFrame(num, num2, true);
				}
			}
		}
		private static void TripWire(int left, int top, int width, int height)
		{
			if (Main.netMode == 1)
			{
				return;
			}
			Wiring.running = true;
			if (Wiring._wireList.Count != 0)
			{
				Wiring._wireList.Clear(true);
			}
			for (int i = left; i < left + width; i++)
			{
				for (int j = top; j < top + height; j++)
				{
					Point16 back = new Point16(i, j);
					Tile tile = Main.tile[i, j];
					if (tile != null && tile.wire())
					{
						Wiring._wireList.PushBack(back);
					}
				}
			}
			Vector2[] array = new Vector2[6];
			Wiring._teleport[0].X = -1f;
			Wiring._teleport[0].Y = -1f;
			Wiring._teleport[1].X = -1f;
			Wiring._teleport[1].Y = -1f;
			if (Wiring._wireList.Count > 0)
			{
				Wiring._numInPump = 0;
				Wiring._numOutPump = 0;
				Wiring.HitWire(Wiring._wireList, 1);
				if (Wiring._numInPump > 0 && Wiring._numOutPump > 0)
				{
					Wiring.XferWater();
				}
			}
			for (int k = left; k < left + width; k++)
			{
				for (int l = top; l < top + height; l++)
				{
					Point16 back = new Point16(k, l);
					Tile tile2 = Main.tile[k, l];
					if (tile2 != null && tile2.wire2())
					{
						Wiring._wireList.PushBack(back);
					}
				}
			}
			array[0] = Wiring._teleport[0];
			array[1] = Wiring._teleport[1];
			Wiring._teleport[0].X = -1f;
			Wiring._teleport[0].Y = -1f;
			Wiring._teleport[1].X = -1f;
			Wiring._teleport[1].Y = -1f;
			if (Wiring._wireList.Count > 0)
			{
				Wiring._numInPump = 0;
				Wiring._numOutPump = 0;
				Wiring.HitWire(Wiring._wireList, 2);
				if (Wiring._numInPump > 0 && Wiring._numOutPump > 0)
				{
					Wiring.XferWater();
				}
			}
			array[2] = Wiring._teleport[0];
			array[3] = Wiring._teleport[1];
			Wiring._teleport[0].X = -1f;
			Wiring._teleport[0].Y = -1f;
			Wiring._teleport[1].X = -1f;
			Wiring._teleport[1].Y = -1f;
			for (int m = left; m < left + width; m++)
			{
				for (int n = top; n < top + height; n++)
				{
					Point16 back = new Point16(m, n);
					Tile tile3 = Main.tile[m, n];
					if (tile3 != null && tile3.wire3())
					{
						Wiring._wireList.PushBack(back);
					}
				}
			}
			if (Wiring._wireList.Count > 0)
			{
				Wiring._numInPump = 0;
				Wiring._numOutPump = 0;
				Wiring.HitWire(Wiring._wireList, 3);
				if (Wiring._numInPump > 0 && Wiring._numOutPump > 0)
				{
					Wiring.XferWater();
				}
			}
			array[4] = Wiring._teleport[0];
			array[5] = Wiring._teleport[1];
			for (int num = 0; num < 5; num += 2)
			{
				Wiring._teleport[0] = array[num];
				Wiring._teleport[1] = array[num + 1];
				if (Wiring._teleport[0].X >= 0f && Wiring._teleport[1].X >= 0f)
				{
					Wiring.Teleport();
				}
			}
		}
		private static void HitWire(DoubleStack<Point16> next, int wireType)
		{
			for (int i = 0; i < next.Count; i++)
			{
				Point16 point = next.PopFront();
				Wiring.SkipWire(point);
				Wiring._toProcess.Add(point, 4);
				next.PushBack(point);
			}
			while (next.Count > 0)
			{
				Point16 key = next.PopFront();
				int x = (int)key.X;
				int y = (int)key.Y;
				if (!Wiring._wireSkip.ContainsKey(key))
				{
					Wiring.HitWireSingle(x, y);
				}
				for (int j = 0; j < 4; j++)
				{
					int num;
					int num2;
					switch (j)
					{
					case 0:
						num = x;
						num2 = y + 1;
						break;
					case 1:
						num = x;
						num2 = y - 1;
						break;
					case 2:
						num = x + 1;
						num2 = y;
						break;
					case 3:
						num = x - 1;
						num2 = y;
						break;
					default:
						num = x;
						num2 = y + 1;
						break;
					}
					if (num >= 2 && num < Main.maxTilesX - 2 && num2 >= 2 && num2 < Main.maxTilesY - 2)
					{
						Tile tile = Main.tile[num, num2];
						if (tile != null)
						{
							bool flag;
							switch (wireType)
							{
							case 1:
								flag = tile.wire();
								break;
							case 2:
								flag = tile.wire2();
								break;
							case 3:
								flag = tile.wire3();
								break;
							default:
								flag = false;
								break;
							}
							if (flag)
							{
								Point16 point2 = new Point16(num, num2);
								byte b;
								if (Wiring._toProcess.TryGetValue(point2, out b))
								{
									b -= 1;
									if (b == 0)
									{
										Wiring._toProcess.Remove(point2);
									}
									else
									{
										Wiring._toProcess[point2] = b;
									}
								}
								else
								{
									next.PushBack(point2);
									Wiring._toProcess.Add(point2, 3);
								}
							}
						}
					}
				}
			}
			Wiring._wireSkip.Clear();
			Wiring._toProcess.Clear();
			Wiring.running = false;
		}
		private static void HitWireSingle(int i, int j)
		{
			Tile tile = Main.tile[i, j];
			int type = (int)tile.type;
			if (tile.active() && type >= 255 && type <= 268)
			{
				if (type >= 262)
				{
					Tile expr_35 = tile;
					expr_35.type -= 7;
				}
				else
				{
					Tile expr_46 = tile;
					expr_46.type += 7;
				}
				NetMessage.SendTileSquare(-1, i, j, 1);
			}
			if (tile.actuator() && (type != 226 || (double)j <= Main.worldSurface || NPC.downedPlantBoss))
			{
				if (tile.inActive())
				{
					Wiring.ReActive(i, j);
				}
				else
				{
					Wiring.DeActive(i, j);
				}
			}
			if (tile.active())
			{
				if (type == 144)
				{
					Wiring.HitSwitch(i, j);
					WorldGen.SquareTileFrame(i, j, true);
					NetMessage.SendTileSquare(-1, i, j, 1);
					return;
				}
				if (type == 406)
				{
					int num = (int)(tile.frameX % 54 / 18);
					int num2 = (int)(tile.frameY % 54 / 18);
					int num3 = i - num;
					int num4 = j - num2;
					int num5 = 54;
					if (Main.tile[num3, num4].frameY >= 108)
					{
						num5 = -108;
					}
					for (int k = num3; k < num3 + 3; k++)
					{
						for (int l = num4; l < num4 + 3; l++)
						{
							Wiring.SkipWire(k, l);
							Main.tile[k, l].frameY = (short)((int)Main.tile[k, l].frameY + num5);
						}
					}
					NetMessage.SendTileSquare(-1, num3 + 1, num4 + 1, 3);
					return;
				}
				if (type == 411)
				{
					int num6 = (int)(tile.frameX % 36 / 18);
					int num7 = (int)(tile.frameY % 36 / 18);
					int num8 = i - num6;
					int num9 = j - num7;
					int num10 = 36;
					if (Main.tile[num8, num9].frameX >= 36)
					{
						num10 = -36;
					}
					for (int m = num8; m < num8 + 2; m++)
					{
						for (int n = num9; n < num9 + 2; n++)
						{
							Wiring.SkipWire(m, n);
							Main.tile[m, n].frameX = (short)((int)Main.tile[m, n].frameX + num10);
						}
					}
					NetMessage.SendTileSquare(-1, num8, num9, 2);
					return;
				}
				if (type == 405)
				{
					int num11 = (int)(tile.frameX % 54 / 18);
					int num12 = (int)(tile.frameY % 36 / 18);
					int num13 = i - num11;
					int num14 = j - num12;
					int num15 = 54;
					if (Main.tile[num13, num14].frameX >= 54)
					{
						num15 = -54;
					}
					for (int num16 = num13; num16 < num13 + 3; num16++)
					{
						for (int num17 = num14; num17 < num14 + 2; num17++)
						{
							Wiring.SkipWire(num16, num17);
							Main.tile[num16, num17].frameX = (short)((int)Main.tile[num16, num17].frameX + num15);
						}
					}
					NetMessage.SendTileSquare(-1, num13 + 1, num14 + 1, 3);
					return;
				}
				if (type == 215)
				{
					int num18 = (int)(tile.frameX % 54 / 18);
					int num19 = (int)(tile.frameY % 36 / 18);
					int num20 = i - num18;
					int num21 = j - num19;
					int num22 = 36;
					if (Main.tile[num20, num21].frameY >= 36)
					{
						num22 = -36;
					}
					for (int num23 = num20; num23 < num20 + 3; num23++)
					{
						for (int num24 = num21; num24 < num21 + 2; num24++)
						{
							Wiring.SkipWire(num23, num24);
							Main.tile[num23, num24].frameY = (short)((int)Main.tile[num23, num24].frameY + num22);
						}
					}
					NetMessage.SendTileSquare(-1, num20 + 1, num21 + 1, 3);
					return;
				}
				if (type == 130)
				{
					if (Main.tile[i, j - 1] != null && Main.tile[i, j - 1].active())
					{
						if (Main.tile[i, j - 1].type == 21)
						{
							return;
						}
						if (Main.tile[i, j - 1].type == 88)
						{
							return;
						}
					}
					tile.type = 131;
					WorldGen.SquareTileFrame(i, j, true);
					NetMessage.SendTileSquare(-1, i, j, 1);
					return;
				}
				if (type == 131)
				{
					tile.type = 130;
					WorldGen.SquareTileFrame(i, j, true);
					NetMessage.SendTileSquare(-1, i, j, 1);
					return;
				}
				if (type == 387 || type == 386)
				{
					bool value = type == 387;
					int num25 = WorldGen.ShiftTrapdoor(i, j, true, -1).ToInt();
					if (num25 == 0)
					{
						num25 = -WorldGen.ShiftTrapdoor(i, j, false, -1).ToInt();
					}
					if (num25 != 0)
					{
						NetMessage.SendData(19, -1, -1, "", 2 + value.ToInt(), (float)i, (float)j, (float)num25, 0, 0, 0);
						return;
					}
				}
				else
				{
					if (type == 389 || type == 388)
					{
						bool flag = type == 389;
						WorldGen.ShiftTallGate(i, j, flag);
						NetMessage.SendData(19, -1, -1, "", 4 + flag.ToInt(), (float)i, (float)j, 0f, 0, 0, 0);
						return;
					}
					if (type == 11)
					{
						if (WorldGen.CloseDoor(i, j, true))
						{
							NetMessage.SendData(19, -1, -1, "", 1, (float)i, (float)j, 0f, 0, 0, 0);
							return;
						}
					}
					else if (type == 10)
					{
						int num26 = 1;
						if (Main.rand.Next(2) == 0)
						{
							num26 = -1;
						}
						if (WorldGen.OpenDoor(i, j, num26))
						{
							NetMessage.SendData(19, -1, -1, "", 0, (float)i, (float)j, (float)num26, 0, 0, 0);
							return;
						}
						if (WorldGen.OpenDoor(i, j, -num26))
						{
							NetMessage.SendData(19, -1, -1, "", 0, (float)i, (float)j, (float)(-(float)num26), 0, 0, 0);
							return;
						}
					}
					else
					{
						if (type == 216)
						{
							WorldGen.LaunchRocket(i, j);
							Wiring.SkipWire(i, j);
							return;
						}
						if (type == 335)
						{
							int num27 = j - (int)(tile.frameY / 18);
							int num28 = i - (int)(tile.frameX / 18);
							Wiring.SkipWire(num28, num27);
							Wiring.SkipWire(num28, num27 + 1);
							Wiring.SkipWire(num28 + 1, num27);
							Wiring.SkipWire(num28 + 1, num27 + 1);
							if (Wiring.CheckMech(num28, num27, 30))
							{
								WorldGen.LaunchRocketSmall(num28, num27);
								return;
							}
						}
						else if (type == 338)
						{
							int num29 = j - (int)(tile.frameY / 18);
							int num30 = i - (int)(tile.frameX / 18);
							Wiring.SkipWire(num30, num29);
							Wiring.SkipWire(num30, num29 + 1);
							if (Wiring.CheckMech(num30, num29, 30))
							{
								bool flag2 = false;
								for (int num31 = 0; num31 < 1000; num31++)
								{
									if (Main.projectile[num31].active && Main.projectile[num31].aiStyle == 73 && Main.projectile[num31].ai[0] == (float)num30 && Main.projectile[num31].ai[1] == (float)num29)
									{
										flag2 = true;
										break;
									}
								}
								if (!flag2)
								{
									Projectile.NewProjectile((float)(num30 * 16 + 8), (float)(num29 * 16 + 2), 0f, 0f, 419 + Main.rand.Next(4), 0, 0f, Main.myPlayer, (float)num30, (float)num29);
									return;
								}
							}
						}
						else if (type == 235)
						{
							int num32 = i - (int)(tile.frameX / 18);
							if (tile.wall == 87 && (double)j > Main.worldSurface && !NPC.downedPlantBoss)
							{
								return;
							}
							if (Wiring._teleport[0].X == -1f)
							{
								Wiring._teleport[0].X = (float)num32;
								Wiring._teleport[0].Y = (float)j;
								if (tile.halfBrick())
								{
									Vector2[] expr_78E_cp_0 = Wiring._teleport;
									int expr_78E_cp_1 = 0;
									expr_78E_cp_0[expr_78E_cp_1].Y = expr_78E_cp_0[expr_78E_cp_1].Y + 0.5f;
									return;
								}
							}
							else if (Wiring._teleport[0].X != (float)num32 || Wiring._teleport[0].Y != (float)j)
							{
								Wiring._teleport[1].X = (float)num32;
								Wiring._teleport[1].Y = (float)j;
								if (tile.halfBrick())
								{
									Vector2[] expr_807_cp_0 = Wiring._teleport;
									int expr_807_cp_1 = 1;
									expr_807_cp_0[expr_807_cp_1].Y = expr_807_cp_0[expr_807_cp_1].Y + 0.5f;
									return;
								}
							}
						}
						else
						{
							if (type == 4)
							{
								if (tile.frameX < 66)
								{
									Tile expr_828 = tile;
									expr_828.frameX += 66;
								}
								else
								{
									Tile expr_83A = tile;
									expr_83A.frameX -= 66;
								}
								NetMessage.SendTileSquare(-1, i, j, 1);
								return;
							}
							if (type == 149)
							{
								if (tile.frameX < 54)
								{
									Tile expr_866 = tile;
									expr_866.frameX += 54;
								}
								else
								{
									Tile expr_878 = tile;
									expr_878.frameX -= 54;
								}
								NetMessage.SendTileSquare(-1, i, j, 1);
								return;
							}
							if (type == 244)
							{
								int num33;
								for (num33 = (int)(tile.frameX / 18); num33 >= 3; num33 -= 3)
								{
								}
								int num34;
								for (num34 = (int)(tile.frameY / 18); num34 >= 3; num34 -= 3)
								{
								}
								int num35 = i - num33;
								int num36 = j - num34;
								int num37 = 54;
								if (Main.tile[num35, num36].frameX >= 54)
								{
									num37 = -54;
								}
								for (int num38 = num35; num38 < num35 + 3; num38++)
								{
									for (int num39 = num36; num39 < num36 + 2; num39++)
									{
										Wiring.SkipWire(num38, num39);
										Main.tile[num38, num39].frameX = (short)((int)Main.tile[num38, num39].frameX + num37);
									}
								}
								NetMessage.SendTileSquare(-1, num35 + 1, num36 + 1, 3);
								return;
							}
							if (type == 42)
							{
								int num40;
								for (num40 = (int)(tile.frameY / 18); num40 >= 2; num40 -= 2)
								{
								}
								int num41 = j - num40;
								short num42 = 18;
								if (tile.frameX > 0)
								{
									num42 = -18;
								}
								Tile expr_9A6 = Main.tile[i, num41];
								expr_9A6.frameX += num42;
								Tile expr_9C4 = Main.tile[i, num41 + 1];
								expr_9C4.frameX += num42;
								Wiring.SkipWire(i, num41);
								Wiring.SkipWire(i, num41 + 1);
								NetMessage.SendTileSquare(-1, i, j, 2);
								return;
							}
							if (type == 93)
							{
								int num43;
								for (num43 = (int)(tile.frameY / 18); num43 >= 3; num43 -= 3)
								{
								}
								num43 = j - num43;
								short num44 = 18;
								if (tile.frameX > 0)
								{
									num44 = -18;
								}
								Tile expr_A33 = Main.tile[i, num43];
								expr_A33.frameX += num44;
								Tile expr_A51 = Main.tile[i, num43 + 1];
								expr_A51.frameX += num44;
								Tile expr_A6F = Main.tile[i, num43 + 2];
								expr_A6F.frameX += num44;
								Wiring.SkipWire(i, num43);
								Wiring.SkipWire(i, num43 + 1);
								Wiring.SkipWire(i, num43 + 2);
								NetMessage.SendTileSquare(-1, i, num43 + 1, 3);
								return;
							}
							if (type == 126 || type == 95 || type == 100 || type == 173)
							{
								int num45;
								for (num45 = (int)(tile.frameY / 18); num45 >= 2; num45 -= 2)
								{
								}
								num45 = j - num45;
								int num46 = (int)(tile.frameX / 18);
								if (num46 > 1)
								{
									num46 -= 2;
								}
								num46 = i - num46;
								short num47 = 36;
								if (Main.tile[num46, num45].frameX > 0)
								{
									num47 = -36;
								}
								Tile expr_B27 = Main.tile[num46, num45];
								expr_B27.frameX += num47;
								Tile expr_B46 = Main.tile[num46, num45 + 1];
								expr_B46.frameX += num47;
								Tile expr_B65 = Main.tile[num46 + 1, num45];
								expr_B65.frameX += num47;
								Tile expr_B86 = Main.tile[num46 + 1, num45 + 1];
								expr_B86.frameX += num47;
								Wiring.SkipWire(num46, num45);
								Wiring.SkipWire(num46 + 1, num45);
								Wiring.SkipWire(num46, num45 + 1);
								Wiring.SkipWire(num46 + 1, num45 + 1);
								NetMessage.SendTileSquare(-1, num46, num45, 3);
								return;
							}
							if (type == 34)
							{
								int num48;
								for (num48 = (int)(tile.frameY / 18); num48 >= 3; num48 -= 3)
								{
								}
								int num49 = j - num48;
								int num50 = (int)(tile.frameX / 18);
								if (num50 > 2)
								{
									num50 -= 3;
								}
								num50 = i - num50;
								short num51 = 54;
								if (Main.tile[num50, num49].frameX > 0)
								{
									num51 = -54;
								}
								for (int num52 = num50; num52 < num50 + 3; num52++)
								{
									for (int num53 = num49; num53 < num49 + 3; num53++)
									{
										Tile expr_C47 = Main.tile[num52, num53];
										expr_C47.frameX += num51;
										Wiring.SkipWire(num52, num53);
									}
								}
								NetMessage.SendTileSquare(-1, num50 + 1, num49 + 1, 3);
								return;
							}
							if (type == 314)
							{
								if (Wiring.CheckMech(i, j, 5))
								{
									Minecart.FlipSwitchTrack(i, j);
									return;
								}
							}
							else
							{
								if (type == 33 || type == 174)
								{
									short num54 = 18;
									if (tile.frameX > 0)
									{
										num54 = -18;
									}
									Tile expr_CC7 = tile;
									expr_CC7.frameX += num54;
									NetMessage.SendTileSquare(-1, i, j, 3);
									return;
								}
								if (type == 92)
								{
									int num55 = j - (int)(tile.frameY / 18);
									short num56 = 18;
									if (tile.frameX > 0)
									{
										num56 = -18;
									}
									for (int num57 = num55; num57 < num55 + 6; num57++)
									{
										Tile expr_D16 = Main.tile[i, num57];
										expr_D16.frameX += num56;
										Wiring.SkipWire(i, num57);
									}
									NetMessage.SendTileSquare(-1, i, num55 + 3, 7);
									return;
								}
								if (type == 137)
								{
									int num58 = (int)(tile.frameY / 18);
									Vector2 zero = Vector2.Zero;
									float speedX = 0f;
									float speedY = 0f;
									int num59 = 0;
									int damage = 0;
									switch (num58)
									{
									case 0:
										if (Wiring.CheckMech(i, j, 200))
										{
											int num60 = -1;
											if (tile.frameX != 0)
											{
												num60 = 1;
											}
											speedX = (float)(12 * num60);
											damage = 20;
											num59 = 98;
											zero = new Vector2((float)(i * 16 + 8), (float)(j * 16 + 7));
											zero.X += (float)(10 * num60);
											zero.Y += 2f;
										}
										break;
									case 1:
										if (Wiring.CheckMech(i, j, 200))
										{
											int num61 = -1;
											if (tile.frameX != 0)
											{
												num61 = 1;
											}
											speedX = (float)(12 * num61);
											damage = 40;
											num59 = 184;
											zero = new Vector2((float)(i * 16 + 8), (float)(j * 16 + 7));
											zero.X += (float)(10 * num61);
											zero.Y += 2f;
										}
										break;
									case 2:
										if (Wiring.CheckMech(i, j, 200))
										{
											int num62 = -1;
											if (tile.frameX != 0)
											{
												num62 = 1;
											}
											speedX = (float)(5 * num62);
											damage = 40;
											num59 = 187;
											zero = new Vector2((float)(i * 16 + 8), (float)(j * 16 + 7));
											zero.X += (float)(10 * num62);
											zero.Y += 2f;
										}
										break;
									case 3:
										if (Wiring.CheckMech(i, j, 300))
										{
											num59 = 185;
											int num63 = 200;
											for (int num64 = 0; num64 < 1000; num64++)
											{
												if (Main.projectile[num64].active && Main.projectile[num64].type == num59)
												{
													float num65 = (new Vector2((float)(i * 16 + 8), (float)(j * 18 + 8)) - Main.projectile[num64].Center).Length();
													if (num65 < 50f)
													{
														num63 -= 50;
													}
													else if (num65 < 100f)
													{
														num63 -= 15;
													}
													else if (num65 < 200f)
													{
														num63 -= 10;
													}
													else if (num65 < 300f)
													{
														num63 -= 8;
													}
													else if (num65 < 400f)
													{
														num63 -= 6;
													}
													else if (num65 < 500f)
													{
														num63 -= 5;
													}
													else if (num65 < 700f)
													{
														num63 -= 4;
													}
													else if (num65 < 900f)
													{
														num63 -= 3;
													}
													else if (num65 < 1200f)
													{
														num63 -= 2;
													}
													else
													{
														num63--;
													}
												}
											}
											if (num63 > 0)
											{
												speedX = (float)Main.rand.Next(-20, 21) * 0.05f;
												speedY = 4f + (float)Main.rand.Next(0, 21) * 0.05f;
												damage = 40;
												zero = new Vector2((float)(i * 16 + 8), (float)(j * 16 + 16));
												zero.Y += 6f;
												Projectile.NewProjectile((float)((int)zero.X), (float)((int)zero.Y), speedX, speedY, num59, damage, 2f, Main.myPlayer, 0f, 0f);
											}
										}
										break;
									case 4:
										if (Wiring.CheckMech(i, j, 90))
										{
											speedX = 0f;
											speedY = 8f;
											damage = 60;
											num59 = 186;
											zero = new Vector2((float)(i * 16 + 8), (float)(j * 16 + 16));
											zero.Y += 10f;
										}
										break;
									}
									if (num59 != 0)
									{
										Projectile.NewProjectile((float)((int)zero.X), (float)((int)zero.Y), speedX, speedY, num59, damage, 2f, Main.myPlayer, 0f, 0f);
										return;
									}
								}
								else
								{
									if (type == 139 || type == 35)
									{
										WorldGen.SwitchMB(i, j);
										return;
									}
									if (type == 207)
									{
										WorldGen.SwitchFountain(i, j);
										return;
									}
									if (type == 410)
									{
										WorldGen.SwitchMonolith(i, j);
										return;
									}
									if (type == 141)
									{
										WorldGen.KillTile(i, j, false, false, true);
										NetMessage.SendTileSquare(-1, i, j, 1);
										Projectile.NewProjectile((float)(i * 16 + 8), (float)(j * 16 + 8), 0f, 0f, 108, 500, 10f, Main.myPlayer, 0f, 0f);
										return;
									}
									if (type == 210)
									{
										WorldGen.ExplodeMine(i, j);
										return;
									}
									if (type == 142 || type == 143)
									{
										int num66 = j - (int)(tile.frameY / 18);
										int num67 = (int)(tile.frameX / 18);
										if (num67 > 1)
										{
											num67 -= 2;
										}
										num67 = i - num67;
										Wiring.SkipWire(num67, num66);
										Wiring.SkipWire(num67, num66 + 1);
										Wiring.SkipWire(num67 + 1, num66);
										Wiring.SkipWire(num67 + 1, num66 + 1);
										if (type == 142)
										{
											for (int num68 = 0; num68 < 4; num68++)
											{
												if (Wiring._numInPump >= 19)
												{
													return;
												}
												int num69;
												int num70;
												if (num68 == 0)
												{
													num69 = num67;
													num70 = num66 + 1;
												}
												else if (num68 == 1)
												{
													num69 = num67 + 1;
													num70 = num66 + 1;
												}
												else if (num68 == 2)
												{
													num69 = num67;
													num70 = num66;
												}
												else
												{
													num69 = num67 + 1;
													num70 = num66;
												}
												Wiring._inPumpX[Wiring._numInPump] = num69;
												Wiring._inPumpY[Wiring._numInPump] = num70;
												Wiring._numInPump++;
											}
											return;
										}
										for (int num71 = 0; num71 < 4; num71++)
										{
											if (Wiring._numOutPump >= 19)
											{
												return;
											}
											int num69;
											int num70;
											if (num71 == 0)
											{
												num69 = num67;
												num70 = num66 + 1;
											}
											else if (num71 == 1)
											{
												num69 = num67 + 1;
												num70 = num66 + 1;
											}
											else if (num71 == 2)
											{
												num69 = num67;
												num70 = num66;
											}
											else
											{
												num69 = num67 + 1;
												num70 = num66;
											}
											Wiring._outPumpX[Wiring._numOutPump] = num69;
											Wiring._outPumpY[Wiring._numOutPump] = num70;
											Wiring._numOutPump++;
										}
										return;
									}
									else if (type == 105)
									{
										int num72 = j - (int)(tile.frameY / 18);
										int num73 = (int)(tile.frameX / 18);
										int num74 = 0;
										while (num73 >= 2)
										{
											num73 -= 2;
											num74++;
										}
										num73 = i - num73;
										Wiring.SkipWire(num73, num72);
										Wiring.SkipWire(num73, num72 + 1);
										Wiring.SkipWire(num73, num72 + 2);
										Wiring.SkipWire(num73 + 1, num72);
										Wiring.SkipWire(num73 + 1, num72 + 1);
										Wiring.SkipWire(num73 + 1, num72 + 2);
										int num75 = num73 * 16 + 16;
										int num76 = (num72 + 3) * 16;
										int num77 = -1;
										if (num74 == 4)
										{
											if (Wiring.CheckMech(num73, num72, 30) && NPC.MechSpawn((float)num75, (float)num76, 1))
											{
												num77 = NPC.NewNPC(num75, num76 - 12, 1, 0, 0f, 0f, 0f, 0f, 255);
											}
										}
										else if (num74 == 7)
										{
											if (Wiring.CheckMech(num73, num72, 30) && NPC.MechSpawn((float)num75, (float)num76, 49))
											{
												num77 = NPC.NewNPC(num75 - 4, num76 - 6, 49, 0, 0f, 0f, 0f, 0f, 255);
											}
										}
										else if (num74 == 8)
										{
											if (Wiring.CheckMech(num73, num72, 30) && NPC.MechSpawn((float)num75, (float)num76, 55))
											{
												num77 = NPC.NewNPC(num75, num76 - 12, 55, 0, 0f, 0f, 0f, 0f, 255);
											}
										}
										else if (num74 == 9)
										{
											if (Wiring.CheckMech(num73, num72, 30) && NPC.MechSpawn((float)num75, (float)num76, 46))
											{
												num77 = NPC.NewNPC(num75, num76 - 12, 46, 0, 0f, 0f, 0f, 0f, 255);
											}
										}
										else if (num74 == 10)
										{
											if (Wiring.CheckMech(num73, num72, 30) && NPC.MechSpawn((float)num75, (float)num76, 21))
											{
												num77 = NPC.NewNPC(num75, num76, 21, 0, 0f, 0f, 0f, 0f, 255);
											}
										}
										else if (num74 == 18)
										{
											if (Wiring.CheckMech(num73, num72, 30) && NPC.MechSpawn((float)num75, (float)num76, 67))
											{
												num77 = NPC.NewNPC(num75, num76 - 12, 67, 0, 0f, 0f, 0f, 0f, 255);
											}
										}
										else if (num74 == 23)
										{
											if (Wiring.CheckMech(num73, num72, 30) && NPC.MechSpawn((float)num75, (float)num76, 63))
											{
												num77 = NPC.NewNPC(num75, num76 - 12, 63, 0, 0f, 0f, 0f, 0f, 255);
											}
										}
										else if (num74 == 27)
										{
											if (Wiring.CheckMech(num73, num72, 30) && NPC.MechSpawn((float)num75, (float)num76, 85))
											{
												num77 = NPC.NewNPC(num75 - 9, num76, 85, 0, 0f, 0f, 0f, 0f, 255);
											}
										}
										else if (num74 == 28)
										{
											if (Wiring.CheckMech(num73, num72, 30) && NPC.MechSpawn((float)num75, (float)num76, 74))
											{
												num77 = NPC.NewNPC(num75, num76 - 12, 74, 0, 0f, 0f, 0f, 0f, 255);
											}
										}
										else if (num74 == 34)
										{
											for (int num78 = 0; num78 < 2; num78++)
											{
												for (int num79 = 0; num79 < 3; num79++)
												{
													Tile tile2 = Main.tile[num73 + num78, num72 + num79];
													tile2.type = 349;
													tile2.frameX = (short)(num78 * 18 + 216);
													tile2.frameY = (short)(num79 * 18);
												}
											}
											Animation.NewTemporaryAnimation(0, 349, num73, num72);
											if (Main.netMode == 2)
											{
												NetMessage.SendTileRange(-1, num73, num72, 2, 3);
											}
										}
										else if (num74 == 42)
										{
											if (Wiring.CheckMech(num73, num72, 30) && NPC.MechSpawn((float)num75, (float)num76, 58))
											{
												num77 = NPC.NewNPC(num75, num76 - 12, 58, 0, 0f, 0f, 0f, 0f, 255);
											}
										}
										else if (num74 == 37)
										{
											if (Wiring.CheckMech(num73, num72, 600) && Item.MechSpawn((float)num75, (float)num76, 58) && Item.MechSpawn((float)num75, (float)num76, 1734) && Item.MechSpawn((float)num75, (float)num76, 1867))
											{
												Item.NewItem(num75, num76 - 16, 0, 0, 58, 1, false, 0, false, false);
											}
										}
										else if (num74 == 50)
										{
											if (Wiring.CheckMech(num73, num72, 30) && NPC.MechSpawn((float)num75, (float)num76, 65) && !Collision.SolidTiles(num73 - 2, num73 + 3, num72, num72 + 2))
											{
												num77 = NPC.NewNPC(num75, num76 - 12, 65, 0, 0f, 0f, 0f, 0f, 255);
											}
										}
										else if (num74 == 2)
										{
											if (Wiring.CheckMech(num73, num72, 600) && Item.MechSpawn((float)num75, (float)num76, 184) && Item.MechSpawn((float)num75, (float)num76, 1735) && Item.MechSpawn((float)num75, (float)num76, 1868))
											{
												Item.NewItem(num75, num76 - 16, 0, 0, 184, 1, false, 0, false, false);
											}
										}
										else if (num74 == 17)
										{
											if (Wiring.CheckMech(num73, num72, 600) && Item.MechSpawn((float)num75, (float)num76, 166))
											{
												Item.NewItem(num75, num76 - 20, 0, 0, 166, 1, false, 0, false, false);
											}
										}
										else if (num74 == 40)
										{
											if (Wiring.CheckMech(num73, num72, 300))
											{
												int[] array = new int[10];
												int num80 = 0;
												for (int num81 = 0; num81 < 200; num81++)
												{
													if (Main.npc[num81].active && (Main.npc[num81].type == 17 || Main.npc[num81].type == 19 || Main.npc[num81].type == 22 || Main.npc[num81].type == 38 || Main.npc[num81].type == 54 || Main.npc[num81].type == 107 || Main.npc[num81].type == 108 || Main.npc[num81].type == 142 || Main.npc[num81].type == 160 || Main.npc[num81].type == 207 || Main.npc[num81].type == 209 || Main.npc[num81].type == 227 || Main.npc[num81].type == 228 || Main.npc[num81].type == 229 || Main.npc[num81].type == 358 || Main.npc[num81].type == 369))
													{
														array[num80] = num81;
														num80++;
														if (num80 >= 9)
														{
															break;
														}
													}
												}
												if (num80 > 0)
												{
													int num82 = array[Main.rand.Next(num80)];
													Main.npc[num82].position.X = (float)(num75 - Main.npc[num82].width / 2);
													Main.npc[num82].position.Y = (float)(num76 - Main.npc[num82].height - 1);
													NetMessage.SendData(23, -1, -1, "", num82, 0f, 0f, 0f, 0, 0, 0);
												}
											}
										}
										else if (num74 == 41 && Wiring.CheckMech(num73, num72, 300))
										{
											int[] array2 = new int[10];
											int num83 = 0;
											for (int num84 = 0; num84 < 200; num84++)
											{
												if (Main.npc[num84].active && (Main.npc[num84].type == 18 || Main.npc[num84].type == 20 || Main.npc[num84].type == 124 || Main.npc[num84].type == 178 || Main.npc[num84].type == 208 || Main.npc[num84].type == 353))
												{
													array2[num83] = num84;
													num83++;
													if (num83 >= 9)
													{
														break;
													}
												}
											}
											if (num83 > 0)
											{
												int num85 = array2[Main.rand.Next(num83)];
												Main.npc[num85].position.X = (float)(num75 - Main.npc[num85].width / 2);
												Main.npc[num85].position.Y = (float)(num76 - Main.npc[num85].height - 1);
												NetMessage.SendData(23, -1, -1, "", num85, 0f, 0f, 0f, 0, 0, 0);
											}
										}
										if (num77 >= 0)
										{
											Main.npc[num77].value = 0f;
											Main.npc[num77].npcSlots = 0f;
											return;
										}
									}
									else if (type == 349)
									{
										int num86 = j - (int)(tile.frameY / 18);
										int num87;
										for (num87 = (int)(tile.frameX / 18); num87 >= 2; num87 -= 2)
										{
										}
										num87 = i - num87;
										Wiring.SkipWire(num87, num86);
										Wiring.SkipWire(num87, num86 + 1);
										Wiring.SkipWire(num87, num86 + 2);
										Wiring.SkipWire(num87 + 1, num86);
										Wiring.SkipWire(num87 + 1, num86 + 1);
										Wiring.SkipWire(num87 + 1, num86 + 2);
										short num88;
										if (Main.tile[num87, num86].frameX == 0)
										{
											num88 = 216;
										}
										else
										{
											num88 = -216;
										}
										for (int num89 = 0; num89 < 2; num89++)
										{
											for (int num90 = 0; num90 < 3; num90++)
											{
												Tile expr_1DF5 = Main.tile[num87 + num89, num86 + num90];
												expr_1DF5.frameX += num88;
											}
										}
										if (Main.netMode == 2)
										{
											NetMessage.SendTileRange(-1, num87, num86, 2, 3);
										}
										Animation.NewTemporaryAnimation((num88 > 0) ? 0 : 1, 349, num87, num86);
									}
								}
							}
						}
					}
				}
			}
		}
		private static void Teleport()
		{
			if (Wiring._teleport[0].X < Wiring._teleport[1].X + 3f && Wiring._teleport[0].X > Wiring._teleport[1].X - 3f && Wiring._teleport[0].Y > Wiring._teleport[1].Y - 3f && Wiring._teleport[0].Y < Wiring._teleport[1].Y)
			{
				return;
			}
			Rectangle[] array = new Rectangle[2];
			array[0].X = (int)(Wiring._teleport[0].X * 16f);
			array[0].Width = 48;
			array[0].Height = 48;
			array[0].Y = (int)(Wiring._teleport[0].Y * 16f - (float)array[0].Height);
			array[1].X = (int)(Wiring._teleport[1].X * 16f);
			array[1].Width = 48;
			array[1].Height = 48;
			array[1].Y = (int)(Wiring._teleport[1].Y * 16f - (float)array[1].Height);
			for (int i = 0; i < 2; i++)
			{
				Vector2 value = new Vector2((float)(array[1].X - array[0].X), (float)(array[1].Y - array[0].Y));
				if (i == 1)
				{
					value = new Vector2((float)(array[0].X - array[1].X), (float)(array[0].Y - array[1].Y));
				}
				for (int j = 0; j < 255; j++)
				{
					if (Main.player[j].active && !Main.player[j].dead && !Main.player[j].teleporting && array[i].Intersects(Main.player[j].getRect()))
					{
						Vector2 vector = Main.player[j].position + value;
						Main.player[j].teleporting = true;
						if (Main.netMode == 2)
						{
							RemoteClient.CheckSection(j, vector, 1);
						}
						Main.player[j].Teleport(vector, 0, 0);
						if (Main.netMode == 2)
						{
							NetMessage.SendData(65, -1, -1, "", 0, (float)j, vector.X, vector.Y, 0, 0, 0);
						}
					}
				}
				for (int k = 0; k < 200; k++)
				{
					if (Main.npc[k].active && !Main.npc[k].teleporting && Main.npc[k].lifeMax > 5 && !Main.npc[k].boss && !Main.npc[k].noTileCollide && array[i].Intersects(Main.npc[k].getRect()))
					{
						Main.npc[k].teleporting = true;
						Main.npc[k].Teleport(Main.npc[k].position + value, 0, 0);
					}
				}
			}
			for (int l = 0; l < 255; l++)
			{
				Main.player[l].teleporting = false;
			}
			for (int m = 0; m < 200; m++)
			{
				Main.npc[m].teleporting = false;
			}
		}
		private static void DeActive(int i, int j)
		{
			if (!Main.tile[i, j].active())
			{
				return;
			}
			bool flag = Main.tileSolid[(int)Main.tile[i, j].type] && !TileID.Sets.NotReallySolid[(int)Main.tile[i, j].type];
			ushort type = Main.tile[i, j].type;
			if (type != 314)
			{
				switch (type)
				{
				case 386:
				case 387:
				case 388:
				case 389:
					break;
				default:
					goto IL_85;
				}
			}
			flag = false;
			IL_85:
			if (!flag)
			{
				return;
			}
			if (Main.tile[i, j - 1].active() && (Main.tile[i, j - 1].type == 5 || Main.tile[i, j - 1].type == 21 || Main.tile[i, j - 1].type == 26 || Main.tile[i, j - 1].type == 77 || Main.tile[i, j - 1].type == 72))
			{
				return;
			}
			Main.tile[i, j].inActive(true);
			WorldGen.SquareTileFrame(i, j, false);
			if (Main.netMode != 1)
			{
				NetMessage.SendTileSquare(-1, i, j, 1);
			}
		}
		private static void ReActive(int i, int j)
		{
			Main.tile[i, j].inActive(false);
			WorldGen.SquareTileFrame(i, j, false);
			if (Main.netMode != 1)
			{
				NetMessage.SendTileSquare(-1, i, j, 1);
			}
		}
	}
}
