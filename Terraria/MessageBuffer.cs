using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Achievements;
using Terraria.GameContent.Tile_Entities;
using Terraria.GameContent.UI;
using Terraria.GameContent.UI.Chat;
using Terraria.ID;
using Terraria.Net;
using Terraria.UI;
namespace Terraria
{
	public class MessageBuffer
	{
		public const int readBufferMax = 131070;
		public const int writeBufferMax = 131070;
		public bool broadcast;
		public byte[] readBuffer = new byte[131070];
		public byte[] writeBuffer = new byte[131070];
		public bool writeLocked;
		public int messageLength;
		public int totalData;
		public int whoAmI;
		public int spamCount;
		public int maxSpam;
		public bool checkBytes;
		public MemoryStream readerStream;
		public MemoryStream writerStream;
		public BinaryReader reader;
		public BinaryWriter writer;
		public void Reset()
		{
			Array.Clear(this.readBuffer, 0, this.readBuffer.Length);
			Array.Clear(this.writeBuffer, 0, this.writeBuffer.Length);
			this.writeLocked = false;
			this.messageLength = 0;
			this.totalData = 0;
			this.spamCount = 0;
			this.broadcast = false;
			this.checkBytes = false;
			this.ResetReader();
			this.ResetWriter();
		}
		public void ResetReader()
		{
			if (this.readerStream != null)
			{
				this.readerStream.Close();
			}
			this.readerStream = new MemoryStream(this.readBuffer);
			this.reader = new BinaryReader(this.readerStream);
		}
		public void ResetWriter()
		{
			if (this.writerStream != null)
			{
				this.writerStream.Close();
			}
			this.writerStream = new MemoryStream(this.writeBuffer);
			this.writer = new BinaryWriter(this.writerStream);
		}
		public void GetData(int start, int length, out int messageType)
		{
			if (this.whoAmI < 256)
			{
				Netplay.Clients[this.whoAmI].TimeOutTimer = 0;
			}
			else
			{
				Netplay.Connection.TimeOutTimer = 0;
			}
			int num = start + 1;
			byte b = this.readBuffer[start];
			messageType = (int)b;
			if (b >= 105)
			{
				return;
			}
			Main.rxMsg++;
			Main.rxData += length;
			Main.rxMsgType[(int)b]++;
			Main.rxDataType[(int)b] += length;
			if (Main.netMode == 1 && Netplay.Connection.StatusMax > 0)
			{
				Netplay.Connection.StatusCount++;
			}
			if (Main.verboseNetplay)
			{
				for (int i = start; i < start + length; i++)
				{
				}
				for (int j = start; j < start + length; j++)
				{
					byte arg_D6_0 = this.readBuffer[j];
				}
			}
			if (Main.netMode == 2 && b != 38 && Netplay.Clients[this.whoAmI].State == -1)
			{
				NetMessage.SendData(2, this.whoAmI, -1, Lang.mp[1], 0, 0f, 0f, 0f, 0, 0, 0);
				return;
			}
			if (Main.netMode == 2 && Netplay.Clients[this.whoAmI].State < 10 && b > 12 && b != 93 && b != 16 && b != 42 && b != 50 && b != 38 && b != 68)
			{
				NetMessage.BootPlayer(this.whoAmI, Lang.mp[2]);
			}
			if (this.reader == null)
			{
				this.ResetReader();
			}
			this.reader.BaseStream.Position = (long)num;
			switch (b)
			{
			case 1:
			{
				if (Main.netMode != 2)
				{
					return;
				}
				if (Main.dedServ && Netplay.IsBanned(Netplay.Clients[this.whoAmI].Socket.GetRemoteAddress()))
				{
					NetMessage.SendData(2, this.whoAmI, -1, Lang.mp[3], 0, 0f, 0f, 0f, 0, 0, 0);
					return;
				}
				if (Netplay.Clients[this.whoAmI].State != 0)
				{
					return;
				}
				string a = this.reader.ReadString();
				if (!(a == "Terraria" + Main.curRelease))
				{
					NetMessage.SendData(2, this.whoAmI, -1, Lang.mp[4], 0, 0f, 0f, 0f, 0, 0, 0);
					return;
				}
				if (string.IsNullOrEmpty(Netplay.ServerPassword))
				{
					Netplay.Clients[this.whoAmI].State = 1;
					NetMessage.SendData(3, this.whoAmI, -1, "", 0, 0f, 0f, 0f, 0, 0, 0);
					return;
				}
				Netplay.Clients[this.whoAmI].State = -1;
				NetMessage.SendData(37, this.whoAmI, -1, "", 0, 0f, 0f, 0f, 0, 0, 0);
				return;
			}
			case 2:
				if (Main.netMode != 1)
				{
					return;
				}
				Netplay.disconnect = true;
				Main.statusText = this.reader.ReadString();
				return;
			case 3:
			{
				if (Main.netMode != 1)
				{
					return;
				}
				if (Netplay.Connection.State == 1)
				{
					Netplay.Connection.State = 2;
				}
				int num2 = (int)this.reader.ReadByte();
				if (num2 != Main.myPlayer)
				{
					Main.player[num2] = Main.ActivePlayerFileData.Player;
					Main.player[Main.myPlayer] = new Player();
				}
				Main.player[num2].whoAmI = num2;
				Main.myPlayer = num2;
				Player player = Main.player[num2];
				NetMessage.SendData(4, -1, -1, player.name, num2, 0f, 0f, 0f, 0, 0, 0);
				NetMessage.SendData(68, -1, -1, "", num2, 0f, 0f, 0f, 0, 0, 0);
				NetMessage.SendData(16, -1, -1, "", num2, 0f, 0f, 0f, 0, 0, 0);
				NetMessage.SendData(42, -1, -1, "", num2, 0f, 0f, 0f, 0, 0, 0);
				NetMessage.SendData(50, -1, -1, "", num2, 0f, 0f, 0f, 0, 0, 0);
				for (int k = 0; k < 59; k++)
				{
					NetMessage.SendData(5, -1, -1, player.inventory[k].name, num2, (float)k, (float)player.inventory[k].prefix, 0f, 0, 0, 0);
				}
				for (int l = 0; l < player.armor.Length; l++)
				{
					NetMessage.SendData(5, -1, -1, player.armor[l].name, num2, (float)(59 + l), (float)player.armor[l].prefix, 0f, 0, 0, 0);
				}
				for (int m = 0; m < player.dye.Length; m++)
				{
					NetMessage.SendData(5, -1, -1, player.dye[m].name, num2, (float)(58 + player.armor.Length + 1 + m), (float)player.dye[m].prefix, 0f, 0, 0, 0);
				}
				for (int n = 0; n < player.miscEquips.Length; n++)
				{
					NetMessage.SendData(5, -1, -1, "", num2, (float)(58 + player.armor.Length + player.dye.Length + 1 + n), (float)player.miscEquips[n].prefix, 0f, 0, 0, 0);
				}
				for (int num3 = 0; num3 < player.miscDyes.Length; num3++)
				{
					NetMessage.SendData(5, -1, -1, "", num2, (float)(58 + player.armor.Length + player.dye.Length + player.miscEquips.Length + 1 + num3), (float)player.miscDyes[num3].prefix, 0f, 0, 0, 0);
				}
				for (int num4 = 0; num4 < player.bank.item.Length; num4++)
				{
					NetMessage.SendData(5, -1, -1, "", num2, (float)(58 + player.armor.Length + player.dye.Length + player.miscEquips.Length + player.miscDyes.Length + 1 + num4), (float)player.bank.item[num4].prefix, 0f, 0, 0, 0);
				}
				for (int num5 = 0; num5 < player.bank2.item.Length; num5++)
				{
					NetMessage.SendData(5, -1, -1, "", num2, (float)(58 + player.armor.Length + player.dye.Length + player.miscEquips.Length + player.miscDyes.Length + player.bank.item.Length + 1 + num5), (float)player.bank2.item[num5].prefix, 0f, 0, 0, 0);
				}
				NetMessage.SendData(5, -1, -1, "", num2, (float)(58 + player.armor.Length + player.dye.Length + player.miscEquips.Length + player.miscDyes.Length + player.bank.item.Length + player.bank2.item.Length + 1), (float)player.trashItem.prefix, 0f, 0, 0, 0);
				NetMessage.SendData(6, -1, -1, "", 0, 0f, 0f, 0f, 0, 0, 0);
				if (Netplay.Connection.State == 2)
				{
					Netplay.Connection.State = 3;
					return;
				}
				return;
			}
			case 4:
			{
				int num6 = (int)this.reader.ReadByte();
				if (Main.netMode == 2)
				{
					num6 = this.whoAmI;
				}
				if (num6 == Main.myPlayer && !Main.ServerSideCharacter)
				{
					return;
				}
				Player player2 = Main.player[num6];
				player2.whoAmI = num6;
				player2.skinVariant = (int)this.reader.ReadByte();
				player2.skinVariant = (int)MathHelper.Clamp((float)player2.skinVariant, 0f, 7f);
				player2.hair = (int)this.reader.ReadByte();
				if (player2.hair >= 134)
				{
					player2.hair = 0;
				}
				player2.name = this.reader.ReadString().Trim().Trim();
				player2.hairDye = this.reader.ReadByte();
				BitsByte bitsByte = this.reader.ReadByte();
				for (int num7 = 0; num7 < 8; num7++)
				{
					player2.hideVisual[num7] = bitsByte[num7];
				}
				bitsByte = this.reader.ReadByte();
				for (int num8 = 0; num8 < 2; num8++)
				{
					player2.hideVisual[num8 + 8] = bitsByte[num8];
				}
				player2.hideMisc = this.reader.ReadByte();
				player2.hairColor = this.reader.ReadRGB();
				player2.skinColor = this.reader.ReadRGB();
				player2.eyeColor = this.reader.ReadRGB();
				player2.shirtColor = this.reader.ReadRGB();
				player2.underShirtColor = this.reader.ReadRGB();
				player2.pantsColor = this.reader.ReadRGB();
				player2.shoeColor = this.reader.ReadRGB();
				BitsByte bitsByte2 = this.reader.ReadByte();
				player2.difficulty = 0;
				if (bitsByte2[0])
				{
					Player expr_B18 = player2;
					expr_B18.difficulty += 1;
				}
				if (bitsByte2[1])
				{
					Player expr_B32 = player2;
					expr_B32.difficulty += 2;
				}
				if (player2.difficulty > 2)
				{
					player2.difficulty = 2;
				}
				player2.extraAccessory = bitsByte2[2];
				if (Main.netMode != 2)
				{
					return;
				}
				bool flag = false;
				if (Netplay.Clients[this.whoAmI].State < 10)
				{
					for (int num9 = 0; num9 < 255; num9++)
					{
						if (num9 != num6 && player2.name == Main.player[num9].name && Netplay.Clients[num9].IsActive)
						{
							flag = true;
						}
					}
				}
				if (flag)
				{
					NetMessage.SendData(2, this.whoAmI, -1, player2.name + " " + Lang.mp[5], 0, 0f, 0f, 0f, 0, 0, 0);
					return;
				}
				if (player2.name.Length > Player.nameLen)
				{
					NetMessage.SendData(2, this.whoAmI, -1, "Name is too long.", 0, 0f, 0f, 0f, 0, 0, 0);
					return;
				}
				if (player2.name == "")
				{
					NetMessage.SendData(2, this.whoAmI, -1, "Empty name.", 0, 0f, 0f, 0f, 0, 0, 0);
					return;
				}
				Netplay.Clients[this.whoAmI].Name = player2.name;
				Netplay.Clients[this.whoAmI].Name = player2.name;
				NetMessage.SendData(4, -1, this.whoAmI, player2.name, num6, 0f, 0f, 0f, 0, 0, 0);
				return;
			}
			case 5:
			{
				int num10 = (int)this.reader.ReadByte();
				if (Main.netMode == 2)
				{
					num10 = this.whoAmI;
				}
				if (num10 == Main.myPlayer && !Main.ServerSideCharacter && !Main.player[num10].IsStackingItems())
				{
					return;
				}
				Player player3 = Main.player[num10];
				lock (player3)
				{
					int num11 = (int)this.reader.ReadByte();
					int stack = (int)this.reader.ReadInt16();
					int num12 = (int)this.reader.ReadByte();
					int type = (int)this.reader.ReadInt16();
					Item[] array = null;
					int num13 = 0;
					bool flag3 = false;
					if (num11 > 58 + player3.armor.Length + player3.dye.Length + player3.miscEquips.Length + player3.miscDyes.Length + player3.bank.item.Length + player3.bank2.item.Length)
					{
						flag3 = true;
					}
					else if (num11 > 58 + player3.armor.Length + player3.dye.Length + player3.miscEquips.Length + player3.miscDyes.Length + player3.bank.item.Length)
					{
						num13 = num11 - 58 - (player3.armor.Length + player3.dye.Length + player3.miscEquips.Length + player3.miscDyes.Length + player3.bank.item.Length) - 1;
						array = player3.bank2.item;
					}
					else if (num11 > 58 + player3.armor.Length + player3.dye.Length + player3.miscEquips.Length + player3.miscDyes.Length)
					{
						num13 = num11 - 58 - (player3.armor.Length + player3.dye.Length + player3.miscEquips.Length + player3.miscDyes.Length) - 1;
						array = player3.bank.item;
					}
					else if (num11 > 58 + player3.armor.Length + player3.dye.Length + player3.miscEquips.Length)
					{
						num13 = num11 - 58 - (player3.armor.Length + player3.dye.Length + player3.miscEquips.Length) - 1;
						array = player3.miscDyes;
					}
					else if (num11 > 58 + player3.armor.Length + player3.dye.Length)
					{
						num13 = num11 - 58 - (player3.armor.Length + player3.dye.Length) - 1;
						array = player3.miscEquips;
					}
					else if (num11 > 58 + player3.armor.Length)
					{
						num13 = num11 - 58 - player3.armor.Length - 1;
						array = player3.dye;
					}
					else if (num11 > 58)
					{
						num13 = num11 - 58 - 1;
						array = player3.armor;
					}
					else
					{
						num13 = num11;
						array = player3.inventory;
					}
					if (flag3)
					{
						player3.trashItem = new Item();
						player3.trashItem.netDefaults(type);
						player3.trashItem.stack = stack;
						player3.trashItem.Prefix(num12);
					}
					else if (num11 <= 58)
					{
						int type2 = array[num13].type;
						int stack2 = array[num13].stack;
						array[num13] = new Item();
						array[num13].netDefaults(type);
						array[num13].stack = stack;
						array[num13].Prefix(num12);
						if (num10 == Main.myPlayer && num13 == 58)
						{
							Main.mouseItem = array[num13].Clone();
						}
						if (num10 == Main.myPlayer && Main.netMode == 1)
						{
							Main.player[num10].inventoryChestStack[num11] = false;
							if (array[num13].stack != stack2 || array[num13].type != type2)
							{
								Recipe.FindRecipes();
								Main.PlaySound(7, -1, -1, 1);
							}
						}
					}
					else
					{
						array[num13] = new Item();
						array[num13].netDefaults(type);
						array[num13].stack = stack;
						array[num13].Prefix(num12);
					}
					if (Main.netMode == 2 && num10 == this.whoAmI && num11 <= 58 + player3.armor.Length + player3.dye.Length + player3.miscEquips.Length + player3.miscDyes.Length)
					{
						NetMessage.SendData(5, -1, this.whoAmI, "", num10, (float)num11, (float)num12, 0f, 0, 0, 0);
					}
					return;
				}
				break;
			}
			case 6:
				break;
			case 7:
			{
				if (Main.netMode != 1)
				{
					return;
				}
				Main.time = (double)this.reader.ReadInt32();
				BitsByte bitsByte3 = this.reader.ReadByte();
				Main.dayTime = bitsByte3[0];
				Main.bloodMoon = bitsByte3[1];
				Main.eclipse = bitsByte3[2];
				Main.moonPhase = (int)this.reader.ReadByte();
				Main.maxTilesX = (int)this.reader.ReadInt16();
				Main.maxTilesY = (int)this.reader.ReadInt16();
				Main.spawnTileX = (int)this.reader.ReadInt16();
				Main.spawnTileY = (int)this.reader.ReadInt16();
				Main.worldSurface = (double)this.reader.ReadInt16();
				Main.rockLayer = (double)this.reader.ReadInt16();
				Main.worldID = this.reader.ReadInt32();
				Main.worldName = this.reader.ReadString();
				Main.moonType = (int)this.reader.ReadByte();
				WorldGen.setBG(0, (int)this.reader.ReadByte());
				WorldGen.setBG(1, (int)this.reader.ReadByte());
				WorldGen.setBG(2, (int)this.reader.ReadByte());
				WorldGen.setBG(3, (int)this.reader.ReadByte());
				WorldGen.setBG(4, (int)this.reader.ReadByte());
				WorldGen.setBG(5, (int)this.reader.ReadByte());
				WorldGen.setBG(6, (int)this.reader.ReadByte());
				WorldGen.setBG(7, (int)this.reader.ReadByte());
				Main.iceBackStyle = (int)this.reader.ReadByte();
				Main.jungleBackStyle = (int)this.reader.ReadByte();
				Main.hellBackStyle = (int)this.reader.ReadByte();
				Main.windSpeedSet = this.reader.ReadSingle();
				Main.numClouds = (int)this.reader.ReadByte();
				for (int num14 = 0; num14 < 3; num14++)
				{
					Main.treeX[num14] = this.reader.ReadInt32();
				}
				for (int num15 = 0; num15 < 4; num15++)
				{
					Main.treeStyle[num15] = (int)this.reader.ReadByte();
				}
				for (int num16 = 0; num16 < 3; num16++)
				{
					Main.caveBackX[num16] = this.reader.ReadInt32();
				}
				for (int num17 = 0; num17 < 4; num17++)
				{
					Main.caveBackStyle[num17] = (int)this.reader.ReadByte();
				}
				Main.maxRaining = this.reader.ReadSingle();
				Main.raining = (Main.maxRaining > 0f);
				BitsByte bitsByte4 = this.reader.ReadByte();
				WorldGen.shadowOrbSmashed = bitsByte4[0];
				NPC.downedBoss1 = bitsByte4[1];
				NPC.downedBoss2 = bitsByte4[2];
				NPC.downedBoss3 = bitsByte4[3];
				Main.hardMode = bitsByte4[4];
				NPC.downedClown = bitsByte4[5];
				Main.ServerSideCharacter = bitsByte4[6];
				NPC.downedPlantBoss = bitsByte4[7];
				BitsByte bitsByte5 = this.reader.ReadByte();
				NPC.downedMechBoss1 = bitsByte5[0];
				NPC.downedMechBoss2 = bitsByte5[1];
				NPC.downedMechBoss3 = bitsByte5[2];
				NPC.downedMechBossAny = bitsByte5[3];
				Main.cloudBGActive = (float)(bitsByte5[4] ? 1 : 0);
				WorldGen.crimson = bitsByte5[5];
				Main.pumpkinMoon = bitsByte5[6];
				Main.snowMoon = bitsByte5[7];
				BitsByte bitsByte6 = this.reader.ReadByte();
				Main.expertMode = bitsByte6[0];
				Main.fastForwardTime = bitsByte6[1];
				Main.UpdateSundial();
				bool flag4 = bitsByte6[2];
				NPC.downedSlimeKing = bitsByte6[3];
				NPC.downedQueenBee = bitsByte6[4];
				NPC.downedFishron = bitsByte6[5];
				NPC.downedMartians = bitsByte6[6];
				NPC.downedAncientCultist = bitsByte6[7];
				BitsByte bitsByte7 = this.reader.ReadByte();
				NPC.downedMoonlord = bitsByte7[0];
				NPC.downedHalloweenKing = bitsByte7[1];
				NPC.downedHalloweenTree = bitsByte7[2];
				NPC.downedChristmasIceQueen = bitsByte7[3];
				NPC.downedChristmasSantank = bitsByte7[4];
				NPC.downedChristmasTree = bitsByte7[5];
				NPC.downedGolemBoss = bitsByte7[6];
				if (flag4)
				{
					Main.StartSlimeRain(true);
				}
				else
				{
					Main.StopSlimeRain(true);
				}
				Main.invasionType = (int)this.reader.ReadSByte();
				Main.LobbyId = this.reader.ReadUInt64();
				if (Netplay.Connection.State == 3)
				{
					Netplay.Connection.State = 4;
					return;
				}
				return;
			}
			case 8:
			{
				if (Main.netMode != 2)
				{
					return;
				}
				int num18 = this.reader.ReadInt32();
				int num19 = this.reader.ReadInt32();
				bool flag5 = true;
				if (num18 == -1 || num19 == -1)
				{
					flag5 = false;
				}
				else if (num18 < 10 || num18 > Main.maxTilesX - 10)
				{
					flag5 = false;
				}
				else if (num19 < 10 || num19 > Main.maxTilesY - 10)
				{
					flag5 = false;
				}
				int num20 = Netplay.GetSectionX(Main.spawnTileX) - 2;
				int num21 = Netplay.GetSectionY(Main.spawnTileY) - 1;
				int num22 = num20 + 5;
				int num23 = num21 + 3;
				if (num20 < 0)
				{
					num20 = 0;
				}
				if (num22 >= Main.maxSectionsX)
				{
					num22 = Main.maxSectionsX - 1;
				}
				if (num21 < 0)
				{
					num21 = 0;
				}
				if (num23 >= Main.maxSectionsY)
				{
					num23 = Main.maxSectionsY - 1;
				}
				int num24 = (num22 - num20) * (num23 - num21);
				List<Point> list = new List<Point>();
				for (int num25 = num20; num25 < num22; num25++)
				{
					for (int num26 = num21; num26 < num23; num26++)
					{
						list.Add(new Point(num25, num26));
					}
				}
				int num27 = -1;
				int num28 = -1;
				if (flag5)
				{
					num18 = Netplay.GetSectionX(num18) - 2;
					num19 = Netplay.GetSectionY(num19) - 1;
					num27 = num18 + 5;
					num28 = num19 + 3;
					if (num18 < 0)
					{
						num18 = 0;
					}
					if (num27 >= Main.maxSectionsX)
					{
						num27 = Main.maxSectionsX - 1;
					}
					if (num19 < 0)
					{
						num19 = 0;
					}
					if (num28 >= Main.maxSectionsY)
					{
						num28 = Main.maxSectionsY - 1;
					}
					for (int num29 = num18; num29 < num27; num29++)
					{
						for (int num30 = num19; num30 < num28; num30++)
						{
							if (num29 < num20 || num29 >= num22 || num30 < num21 || num30 >= num23)
							{
								list.Add(new Point(num29, num30));
								num24++;
							}
						}
					}
				}
				int num31 = 1;
				List<Point> list2;
				List<Point> list3;
				PortalHelper.SyncPortalsOnPlayerJoin(this.whoAmI, 1, list, out list2, out list3);
				num24 += list2.Count;
				if (Netplay.Clients[this.whoAmI].State == 2)
				{
					Netplay.Clients[this.whoAmI].State = 3;
				}
				NetMessage.SendData(9, this.whoAmI, -1, Lang.inter[44], num24, 0f, 0f, 0f, 0, 0, 0);
				Netplay.Clients[this.whoAmI].StatusText2 = "is receiving tile data";
				Netplay.Clients[this.whoAmI].StatusMax += num24;
				for (int num32 = num20; num32 < num22; num32++)
				{
					for (int num33 = num21; num33 < num23; num33++)
					{
						NetMessage.SendSection(this.whoAmI, num32, num33, false);
					}
				}
				NetMessage.SendData(11, this.whoAmI, -1, "", num20, (float)num21, (float)(num22 - 1), (float)(num23 - 1), 0, 0, 0);
				if (flag5)
				{
					for (int num34 = num18; num34 < num27; num34++)
					{
						for (int num35 = num19; num35 < num28; num35++)
						{
							NetMessage.SendSection(this.whoAmI, num34, num35, true);
						}
					}
					NetMessage.SendData(11, this.whoAmI, -1, "", num18, (float)num19, (float)(num27 - 1), (float)(num28 - 1), 0, 0, 0);
				}
				for (int num36 = 0; num36 < list2.Count; num36++)
				{
					NetMessage.SendSection(this.whoAmI, list2[num36].X, list2[num36].Y, true);
				}
				for (int num37 = 0; num37 < list3.Count; num37++)
				{
					NetMessage.SendData(11, this.whoAmI, -1, "", list3[num37].X - num31, (float)(list3[num37].Y - num31), (float)(list3[num37].X + num31 + 1), (float)(list3[num37].Y + num31 + 1), 0, 0, 0);
				}
				for (int num38 = 0; num38 < 400; num38++)
				{
					if (Main.item[num38].active)
					{
						NetMessage.SendData(21, this.whoAmI, -1, "", num38, 0f, 0f, 0f, 0, 0, 0);
						NetMessage.SendData(22, this.whoAmI, -1, "", num38, 0f, 0f, 0f, 0, 0, 0);
					}
				}
				for (int num39 = 0; num39 < 200; num39++)
				{
					if (Main.npc[num39].active)
					{
						NetMessage.SendData(23, this.whoAmI, -1, "", num39, 0f, 0f, 0f, 0, 0, 0);
					}
				}
				for (int num40 = 0; num40 < 1000; num40++)
				{
					if (Main.projectile[num40].active && (Main.projPet[Main.projectile[num40].type] || Main.projectile[num40].netImportant))
					{
						NetMessage.SendData(27, this.whoAmI, -1, "", num40, 0f, 0f, 0f, 0, 0, 0);
					}
				}
				for (int num41 = 0; num41 < 251; num41++)
				{
					NetMessage.SendData(83, this.whoAmI, -1, "", num41, 0f, 0f, 0f, 0, 0, 0);
				}
				NetMessage.SendData(49, this.whoAmI, -1, "", 0, 0f, 0f, 0f, 0, 0, 0);
				NetMessage.SendData(57, this.whoAmI, -1, "", 0, 0f, 0f, 0f, 0, 0, 0);
				NetMessage.SendData(7, this.whoAmI, -1, "", 0, 0f, 0f, 0f, 0, 0, 0);
				NetMessage.SendData(103, -1, -1, "", NPC.MoonLordCountdown, 0f, 0f, 0f, 0, 0, 0);
				NetMessage.SendData(101, this.whoAmI, -1, "", 0, 0f, 0f, 0f, 0, 0, 0);
				return;
			}
			case 9:
				if (Main.netMode != 1)
				{
					return;
				}
				Netplay.Connection.StatusMax += this.reader.ReadInt32();
				Netplay.Connection.StatusText = this.reader.ReadString();
				return;
			case 10:
				if (Main.netMode != 1)
				{
					return;
				}
				NetMessage.DecompressTileBlock(this.readBuffer, num, length);
				return;
			case 11:
				if (Main.netMode != 1)
				{
					return;
				}
				WorldGen.SectionTileFrame((int)this.reader.ReadInt16(), (int)this.reader.ReadInt16(), (int)this.reader.ReadInt16(), (int)this.reader.ReadInt16());
				return;
			case 12:
			{
				int num42 = (int)this.reader.ReadByte();
				if (Main.netMode == 2)
				{
					num42 = this.whoAmI;
				}
				Player player4 = Main.player[num42];
				player4.SpawnX = (int)this.reader.ReadInt16();
				player4.SpawnY = (int)this.reader.ReadInt16();
				player4.Spawn();
				if (num42 == Main.myPlayer && Main.netMode != 2)
				{
					Main.ActivePlayerFileData.StartPlayTimer();
					Player.EnterWorld(Main.player[Main.myPlayer]);
				}
				if (Main.netMode != 2 || Netplay.Clients[this.whoAmI].State < 3)
				{
					return;
				}
				if (Netplay.Clients[this.whoAmI].State == 3)
				{
					Netplay.Clients[this.whoAmI].State = 10;
					NetMessage.greetPlayer(this.whoAmI);
					NetMessage.buffer[this.whoAmI].broadcast = true;
					NetMessage.syncPlayers();
					NetMessage.SendData(12, -1, this.whoAmI, "", this.whoAmI, 0f, 0f, 0f, 0, 0, 0);
					NetMessage.SendData(74, this.whoAmI, -1, Main.player[this.whoAmI].name, Main.anglerQuest, 0f, 0f, 0f, 0, 0, 0);
					return;
				}
				NetMessage.SendData(12, -1, this.whoAmI, "", this.whoAmI, 0f, 0f, 0f, 0, 0, 0);
				return;
			}
			case 13:
			{
				int num43 = (int)this.reader.ReadByte();
				if (num43 == Main.myPlayer && !Main.ServerSideCharacter)
				{
					return;
				}
				if (Main.netMode == 2)
				{
					num43 = this.whoAmI;
				}
				Player player5 = Main.player[num43];
				BitsByte bitsByte8 = this.reader.ReadByte();
				player5.controlUp = bitsByte8[0];
				player5.controlDown = bitsByte8[1];
				player5.controlLeft = bitsByte8[2];
				player5.controlRight = bitsByte8[3];
				player5.controlJump = bitsByte8[4];
				player5.controlUseItem = bitsByte8[5];
				player5.direction = (bitsByte8[6] ? 1 : -1);
				BitsByte bitsByte9 = this.reader.ReadByte();
				if (bitsByte9[0])
				{
					player5.pulley = true;
					player5.pulleyDir = (byte)(bitsByte9[1] ? 2 : 1);
				}
				else
				{
					player5.pulley = false;
				}
				player5.selectedItem = (int)this.reader.ReadByte();
				player5.position = this.reader.ReadVector2();
				if (bitsByte9[2])
				{
					player5.velocity = this.reader.ReadVector2();
				}
				else
				{
					player5.velocity = Vector2.Zero;
				}
				player5.vortexStealthActive = bitsByte9[3];
				player5.gravDir = (float)(bitsByte9[4] ? 1 : -1);
				if (Main.netMode == 2 && Netplay.Clients[this.whoAmI].State == 10)
				{
					NetMessage.SendData(13, -1, this.whoAmI, "", num43, 0f, 0f, 0f, 0, 0, 0);
					return;
				}
				return;
			}
			case 14:
			{
				if (Main.netMode != 1)
				{
					return;
				}
				int num44 = (int)this.reader.ReadByte();
				int num45 = (int)this.reader.ReadByte();
				if (num45 == 1)
				{
					if (!Main.player[num44].active)
					{
						Main.player[num44] = new Player();
					}
					Main.player[num44].active = true;
					return;
				}
				Main.player[num44].active = false;
				return;
			}
			case 15:
			case 67:
			case 93:
			case 94:
				return;
			case 16:
			{
				int num46 = (int)this.reader.ReadByte();
				if (num46 == Main.myPlayer && !Main.ServerSideCharacter)
				{
					return;
				}
				if (Main.netMode == 2)
				{
					num46 = this.whoAmI;
				}
				Player player6 = Main.player[num46];
				player6.statLife = (int)this.reader.ReadInt16();
				player6.statLifeMax = (int)this.reader.ReadInt16();
				if (player6.statLifeMax < 100)
				{
					player6.statLifeMax = 100;
				}
				player6.dead = (player6.statLife <= 0);
				if (Main.netMode == 2)
				{
					NetMessage.SendData(16, -1, this.whoAmI, "", num46, 0f, 0f, 0f, 0, 0, 0);
					return;
				}
				return;
			}
			case 17:
			{
				byte b2 = this.reader.ReadByte();
				int num47 = (int)this.reader.ReadInt16();
				int num48 = (int)this.reader.ReadInt16();
				short num49 = this.reader.ReadInt16();
				int num50 = (int)this.reader.ReadByte();
				bool flag6 = num49 == 1;
				if (!WorldGen.InWorld(num47, num48, 3))
				{
					return;
				}
				if (Main.tile[num47, num48] == null)
				{
					Main.tile[num47, num48] = new Tile();
				}
				if (Main.netMode == 2)
				{
					if (!flag6)
					{
						if (b2 == 0 || b2 == 2 || b2 == 4)
						{
							Netplay.Clients[this.whoAmI].SpamDeleteBlock += 1f;
						}
						if (b2 == 1 || b2 == 3)
						{
							Netplay.Clients[this.whoAmI].SpamAddBlock += 1f;
						}
					}
					if (!Netplay.Clients[this.whoAmI].TileSections[Netplay.GetSectionX(num47), Netplay.GetSectionY(num48)])
					{
						flag6 = true;
					}
				}
				if (b2 == 0)
				{
					WorldGen.KillTile(num47, num48, flag6, false, false);
				}
				if (b2 == 1)
				{
					WorldGen.PlaceTile(num47, num48, (int)num49, false, true, -1, num50);
				}
				if (b2 == 2)
				{
					WorldGen.KillWall(num47, num48, flag6);
				}
				if (b2 == 3)
				{
					WorldGen.PlaceWall(num47, num48, (int)num49, false);
				}
				if (b2 == 4)
				{
					WorldGen.KillTile(num47, num48, flag6, false, true);
				}
				if (b2 == 5)
				{
					WorldGen.PlaceWire(num47, num48);
				}
				if (b2 == 6)
				{
					WorldGen.KillWire(num47, num48);
				}
				if (b2 == 7)
				{
					WorldGen.PoundTile(num47, num48);
				}
				if (b2 == 8)
				{
					WorldGen.PlaceActuator(num47, num48);
				}
				if (b2 == 9)
				{
					WorldGen.KillActuator(num47, num48);
				}
				if (b2 == 10)
				{
					WorldGen.PlaceWire2(num47, num48);
				}
				if (b2 == 11)
				{
					WorldGen.KillWire2(num47, num48);
				}
				if (b2 == 12)
				{
					WorldGen.PlaceWire3(num47, num48);
				}
				if (b2 == 13)
				{
					WorldGen.KillWire3(num47, num48);
				}
				if (b2 == 14)
				{
					WorldGen.SlopeTile(num47, num48, (int)num49);
				}
				if (b2 == 15)
				{
					Minecart.FrameTrack(num47, num48, true, false);
				}
				if (Main.netMode != 2)
				{
					return;
				}
				NetMessage.SendData(17, -1, this.whoAmI, "", (int)b2, (float)num47, (float)num48, (float)num49, num50, 0, 0);
				if (b2 == 1 && num49 == 53)
				{
					NetMessage.SendTileSquare(-1, num47, num48, 1);
					return;
				}
				return;
			}
			case 18:
				if (Main.netMode != 1)
				{
					return;
				}
				Main.dayTime = (this.reader.ReadByte() == 1);
				Main.time = (double)this.reader.ReadInt32();
				Main.sunModY = this.reader.ReadInt16();
				Main.moonModY = this.reader.ReadInt16();
				return;
			case 19:
			{
				byte b3 = this.reader.ReadByte();
				int num51 = (int)this.reader.ReadInt16();
				int num52 = (int)this.reader.ReadInt16();
				if (!WorldGen.InWorld(num51, num52, 3))
				{
					return;
				}
				int num53 = (this.reader.ReadByte() == 0) ? -1 : 1;
				if (b3 == 0)
				{
					WorldGen.OpenDoor(num51, num52, num53);
				}
				else if (b3 == 1)
				{
					WorldGen.CloseDoor(num51, num52, true);
				}
				else if (b3 == 2)
				{
					WorldGen.ShiftTrapdoor(num51, num52, num53 == 1, 1);
				}
				else if (b3 == 3)
				{
					WorldGen.ShiftTrapdoor(num51, num52, num53 == 1, 0);
				}
				else if (b3 == 4)
				{
					WorldGen.ShiftTallGate(num51, num52, false);
				}
				else if (b3 == 5)
				{
					WorldGen.ShiftTallGate(num51, num52, true);
				}
				if (Main.netMode == 2)
				{
					NetMessage.SendData(19, -1, this.whoAmI, "", (int)b3, (float)num51, (float)num52, (float)((num53 == 1) ? 1 : 0), 0, 0, 0);
					return;
				}
				return;
			}
			case 20:
			{
				short num54 = this.reader.ReadInt16();
				int num55 = (int)this.reader.ReadInt16();
				int num56 = (int)this.reader.ReadInt16();
				if (!WorldGen.InWorld(num55, num56, 3))
				{
					return;
				}
				BitsByte bitsByte10 = 0;
				BitsByte bitsByte11 = 0;
				for (int num57 = num55; num57 < num55 + (int)num54; num57++)
				{
					for (int num58 = num56; num58 < num56 + (int)num54; num58++)
					{
						if (Main.tile[num57, num58] == null)
						{
							Main.tile[num57, num58] = new Tile();
						}
						Tile tile = Main.tile[num57, num58];
						bool flag7 = tile.active();
						bitsByte10 = this.reader.ReadByte();
						bitsByte11 = this.reader.ReadByte();
						tile.active(bitsByte10[0]);
						tile.wall = (byte)(bitsByte10[2] ? 1 : 0);
						bool flag8 = bitsByte10[3];
						if (Main.netMode != 2)
						{
							tile.liquid = (byte)(flag8 ? 1 : 0);
						}
						tile.wire(bitsByte10[4]);
						tile.halfBrick(bitsByte10[5]);
						tile.actuator(bitsByte10[6]);
						tile.inActive(bitsByte10[7]);
						tile.wire2(bitsByte11[0]);
						tile.wire3(bitsByte11[1]);
						if (bitsByte11[2])
						{
							tile.color(this.reader.ReadByte());
						}
						if (bitsByte11[3])
						{
							tile.wallColor(this.reader.ReadByte());
						}
						if (tile.active())
						{
							int type3 = (int)tile.type;
							tile.type = this.reader.ReadUInt16();
							if (Main.tileFrameImportant[(int)tile.type])
							{
								tile.frameX = this.reader.ReadInt16();
								tile.frameY = this.reader.ReadInt16();
							}
							else if (!flag7 || (int)tile.type != type3)
							{
								tile.frameX = -1;
								tile.frameY = -1;
							}
							byte b4 = 0;
							if (bitsByte11[4])
							{
								b4 += 1;
							}
							if (bitsByte11[5])
							{
								b4 += 2;
							}
							if (bitsByte11[6])
							{
								b4 += 4;
							}
							tile.slope(b4);
						}
						if (tile.wall > 0)
						{
							tile.wall = this.reader.ReadByte();
						}
						if (flag8)
						{
							tile.liquid = this.reader.ReadByte();
							tile.liquidType((int)this.reader.ReadByte());
						}
					}
				}
				WorldGen.RangeFrame(num55, num56, num55 + (int)num54, num56 + (int)num54);
				if (Main.netMode == 2)
				{
					NetMessage.SendData((int)b, -1, this.whoAmI, "", (int)num54, (float)num55, (float)num56, 0f, 0, 0, 0);
					return;
				}
				return;
			}
			case 21:
			case 90:
			{
				int num59 = (int)this.reader.ReadInt16();
				Vector2 position = this.reader.ReadVector2();
				Vector2 velocity = this.reader.ReadVector2();
				int stack3 = (int)this.reader.ReadInt16();
				int pre = (int)this.reader.ReadByte();
				int num60 = (int)this.reader.ReadByte();
				int num61 = (int)this.reader.ReadInt16();
				if (Main.netMode == 1)
				{
					if (num61 == 0)
					{
						Main.item[num59].active = false;
						return;
					}
					int num62 = num59;
					Item item = Main.item[num62];
					bool newAndShiny = (item.newAndShiny || item.netID != num61) && ItemSlot.Options.HighlightNewItems && (num61 < 0 || num61 >= 3602 || !ItemID.Sets.NeverShiny[num61]);
					item.netDefaults(num61);
					item.newAndShiny = newAndShiny;
					item.Prefix(pre);
					item.stack = stack3;
					item.position = position;
					item.velocity = velocity;
					item.active = true;
					if (b == 90)
					{
						item.instanced = true;
						item.owner = Main.myPlayer;
						item.keepTime = 600;
					}
					item.wet = Collision.WetCollision(item.position, item.width, item.height);
					return;
				}
				else
				{
					if (Main.itemLockoutTime[num59] > 0)
					{
						return;
					}
					if (num61 == 0)
					{
						if (num59 < 400)
						{
							Main.item[num59].active = false;
							NetMessage.SendData(21, -1, -1, "", num59, 0f, 0f, 0f, 0, 0, 0);
							return;
						}
						return;
					}
					else
					{
						bool flag9 = false;
						if (num59 == 400)
						{
							flag9 = true;
						}
						if (flag9)
						{
							Item item2 = new Item();
							item2.netDefaults(num61);
							num59 = Item.NewItem((int)position.X, (int)position.Y, item2.width, item2.height, item2.type, stack3, true, 0, false, false);
						}
						Item item3 = Main.item[num59];
						item3.netDefaults(num61);
						item3.Prefix(pre);
						item3.stack = stack3;
						item3.position = position;
						item3.velocity = velocity;
						item3.active = true;
						item3.owner = Main.myPlayer;
						if (flag9)
						{
							NetMessage.SendData(21, -1, -1, "", num59, 0f, 0f, 0f, 0, 0, 0);
							if (num60 == 0)
							{
								Main.item[num59].ownIgnore = this.whoAmI;
								Main.item[num59].ownTime = 100;
							}
							Main.item[num59].FindOwner(num59);
							return;
						}
						NetMessage.SendData(21, -1, this.whoAmI, "", num59, 0f, 0f, 0f, 0, 0, 0);
						return;
					}
				}
				break;
			}
			case 22:
			{
				int num63 = (int)this.reader.ReadInt16();
				int num64 = (int)this.reader.ReadByte();
				if (Main.netMode == 2 && Main.item[num63].owner != this.whoAmI)
				{
					return;
				}
				Main.item[num63].owner = num64;
				if (num64 == Main.myPlayer)
				{
					Main.item[num63].keepTime = 15;
				}
				else
				{
					Main.item[num63].keepTime = 0;
				}
				if (Main.netMode == 2)
				{
					Main.item[num63].owner = 255;
					Main.item[num63].keepTime = 15;
					NetMessage.SendData(22, -1, -1, "", num63, 0f, 0f, 0f, 0, 0, 0);
					return;
				}
				return;
			}
			case 23:
			{
				if (Main.netMode != 1)
				{
					return;
				}
				int num65 = (int)this.reader.ReadInt16();
				Vector2 position2 = this.reader.ReadVector2();
				Vector2 velocity2 = this.reader.ReadVector2();
				int target = (int)this.reader.ReadByte();
				BitsByte bitsByte12 = this.reader.ReadByte();
				float[] array2 = new float[NPC.maxAI];
				for (int num66 = 0; num66 < NPC.maxAI; num66++)
				{
					if (bitsByte12[num66 + 2])
					{
						array2[num66] = this.reader.ReadSingle();
					}
					else
					{
						array2[num66] = 0f;
					}
				}
				int num67 = (int)this.reader.ReadInt16();
				int num68 = 0;
				if (!bitsByte12[7])
				{
					byte b5 = this.reader.ReadByte();
					if (b5 == 2)
					{
						num68 = (int)this.reader.ReadInt16();
					}
					else if (b5 == 4)
					{
						num68 = this.reader.ReadInt32();
					}
					else
					{
						num68 = (int)this.reader.ReadSByte();
					}
				}
				int num69 = -1;
				NPC nPC = Main.npc[num65];
				if (!nPC.active || nPC.netID != num67)
				{
					if (nPC.active)
					{
						num69 = nPC.type;
					}
					nPC.active = true;
					nPC.netDefaults(num67);
				}
				nPC.position = position2;
				nPC.velocity = velocity2;
				nPC.target = target;
				nPC.direction = (bitsByte12[0] ? 1 : -1);
				nPC.directionY = (bitsByte12[1] ? 1 : -1);
				nPC.spriteDirection = (bitsByte12[6] ? 1 : -1);
				if (bitsByte12[7])
				{
					num68 = (nPC.life = nPC.lifeMax);
				}
				else
				{
					nPC.life = num68;
				}
				if (num68 <= 0)
				{
					nPC.active = false;
				}
				for (int num70 = 0; num70 < NPC.maxAI; num70++)
				{
					nPC.ai[num70] = array2[num70];
				}
				if (num69 > -1 && num69 != nPC.type)
				{
					nPC.TransformVisuals(num69, nPC.type);
				}
				if (num67 == 262)
				{
					NPC.plantBoss = num65;
				}
				if (num67 == 245)
				{
					NPC.golemBoss = num65;
				}
				if (Main.npcCatchable[nPC.type])
				{
					nPC.releaseOwner = (short)this.reader.ReadByte();
					return;
				}
				return;
			}
			case 24:
			{
				int num71 = (int)this.reader.ReadInt16();
				int num72 = (int)this.reader.ReadByte();
				if (Main.netMode == 2)
				{
					num72 = this.whoAmI;
				}
				Player player7 = Main.player[num72];
				Main.npc[num71].StrikeNPC(player7.inventory[player7.selectedItem].damage, player7.inventory[player7.selectedItem].knockBack, player7.direction, false, false, false);
				if (Main.netMode == 2)
				{
					NetMessage.SendData(24, -1, this.whoAmI, "", num71, (float)num72, 0f, 0f, 0, 0, 0);
					NetMessage.SendData(23, -1, -1, "", num71, 0f, 0f, 0f, 0, 0, 0);
					return;
				}
				return;
			}
			case 25:
			{
				int num73 = (int)this.reader.ReadByte();
				if (Main.netMode == 2)
				{
					num73 = this.whoAmI;
				}
				Color color = this.reader.ReadRGB();
				if (Main.netMode == 2)
				{
					color = new Color(255, 255, 255);
				}
				string text = this.reader.ReadString();
				if (Main.netMode == 1)
				{
					string newText = text;
					if (num73 < 255)
					{
						newText = NameTagHandler.GenerateTag(Main.player[num73].name) + " " + text;
						Main.player[num73].chatOverhead.NewMessage(text, Main.chatLength / 2);
					}
					Main.NewText(newText, color.R, color.G, color.B, false);
					return;
				}
				if (Main.netMode != 2)
				{
					return;
				}
				string text2 = text.ToLower();
				if (text2 == Lang.mp[6] || text2 == Lang.mp[21])
				{
					string text3 = "";
					for (int num74 = 0; num74 < 255; num74++)
					{
						if (Main.player[num74].active)
						{
							if (text3 == "")
							{
								text3 = Main.player[num74].name;
							}
							else
							{
								text3 = text3 + ", " + Main.player[num74].name;
							}
						}
					}
					NetMessage.SendData(25, this.whoAmI, -1, Lang.mp[7] + " " + text3 + ".", 255, 255f, 240f, 20f, 0, 0, 0);
					return;
				}
				if (text2.StartsWith("/me "))
				{
					NetMessage.SendData(25, -1, -1, "*" + Main.player[this.whoAmI].name + " " + text.Substring(4), 255, 200f, 100f, 0f, 0, 0, 0);
					return;
				}
				if (text2 == Lang.mp[8])
				{
					NetMessage.SendData(25, -1, -1, string.Concat(new object[]
					{
						"*",
						Main.player[this.whoAmI].name,
						" ",
						Lang.mp[9],
						" ",
						Main.rand.Next(1, 101)
					}), 255, 255f, 240f, 20f, 0, 0, 0);
					return;
				}
				if (text2.StartsWith("/p "))
				{
					int team = Main.player[this.whoAmI].team;
					color = Main.teamColor[team];
					if (team != 0)
					{
						for (int num75 = 0; num75 < 255; num75++)
						{
							if (Main.player[num75].team == team)
							{
								NetMessage.SendData(25, num75, -1, text.Substring(3), num73, (float)color.R, (float)color.G, (float)color.B, 0, 0, 0);
							}
						}
						return;
					}
					NetMessage.SendData(25, this.whoAmI, -1, Lang.mp[10], 255, 255f, 240f, 20f, 0, 0, 0);
					return;
				}
				else
				{
					if (Main.player[this.whoAmI].difficulty == 2)
					{
						color = Main.hcColor;
					}
					else if (Main.player[this.whoAmI].difficulty == 1)
					{
						color = Main.mcColor;
					}
					NetMessage.SendData(25, -1, -1, text, num73, (float)color.R, (float)color.G, (float)color.B, 0, 0, 0);
					if (Main.dedServ)
					{
						Console.WriteLine("<" + Main.player[this.whoAmI].name + "> " + text);
						return;
					}
					return;
				}
				break;
			}
			case 26:
			{
				int num76 = (int)this.reader.ReadByte();
				if (Main.netMode == 2 && this.whoAmI != num76 && (!Main.player[num76].hostile || !Main.player[this.whoAmI].hostile))
				{
					return;
				}
				int num77 = (int)(this.reader.ReadByte() - 1);
				int num78 = (int)this.reader.ReadInt16();
				string text4 = this.reader.ReadString();
				BitsByte bitsByte13 = this.reader.ReadByte();
				bool flag10 = bitsByte13[0];
				bool flag11 = bitsByte13[1];
				int num79 = bitsByte13[2] ? 0 : -1;
				if (bitsByte13[3])
				{
					num79 = 1;
				}
				Main.player[num76].Hurt(num78, num77, flag10, true, text4, flag11, num79);
				if (Main.netMode == 2)
				{
					NetMessage.SendData(26, -1, this.whoAmI, text4, num76, (float)num77, (float)num78, (float)(flag10 ? 1 : 0), flag11 ? 1 : 0, num79, 0);
					return;
				}
				return;
			}
			case 27:
			{
				int num80 = (int)this.reader.ReadInt16();
				Vector2 position3 = this.reader.ReadVector2();
				Vector2 velocity3 = this.reader.ReadVector2();
				float knockBack = this.reader.ReadSingle();
				int damage = (int)this.reader.ReadInt16();
				int num81 = (int)this.reader.ReadByte();
				int num82 = (int)this.reader.ReadInt16();
				BitsByte bitsByte14 = this.reader.ReadByte();
				float[] array3 = new float[Projectile.maxAI];
				for (int num83 = 0; num83 < Projectile.maxAI; num83++)
				{
					if (bitsByte14[num83])
					{
						array3[num83] = this.reader.ReadSingle();
					}
					else
					{
						array3[num83] = 0f;
					}
				}
				int num84 = (int)(bitsByte14[Projectile.maxAI] ? this.reader.ReadInt16() : -1);
				if (num84 >= 1000)
				{
					num84 = -1;
				}
				if (Main.netMode == 2)
				{
					num81 = this.whoAmI;
					if (Main.projHostile[num82])
					{
						return;
					}
				}
				int num85 = 1000;
				for (int num86 = 0; num86 < 1000; num86++)
				{
					if (Main.projectile[num86].owner == num81 && Main.projectile[num86].identity == num80 && Main.projectile[num86].active)
					{
						num85 = num86;
						break;
					}
				}
				if (num85 == 1000)
				{
					for (int num87 = 0; num87 < 1000; num87++)
					{
						if (!Main.projectile[num87].active)
						{
							num85 = num87;
							break;
						}
					}
				}
				Projectile projectile = Main.projectile[num85];
				if (!projectile.active || projectile.type != num82)
				{
					projectile.SetDefaults(num82);
					if (Main.netMode == 2)
					{
						Netplay.Clients[this.whoAmI].SpamProjectile += 1f;
					}
				}
				projectile.identity = num80;
				projectile.position = position3;
				projectile.velocity = velocity3;
				projectile.type = num82;
				projectile.damage = damage;
				projectile.knockBack = knockBack;
				projectile.owner = num81;
				for (int num88 = 0; num88 < Projectile.maxAI; num88++)
				{
					projectile.ai[num88] = array3[num88];
				}
				if (num84 >= 0)
				{
					projectile.projUUID = num84;
					Main.projectileIdentity[num81, num84] = num85;
				}
				projectile.ProjectileFixDesperation();
				if (Main.netMode == 2)
				{
					NetMessage.SendData(27, -1, this.whoAmI, "", num85, 0f, 0f, 0f, 0, 0, 0);
					return;
				}
				return;
			}
			case 28:
			{
				int num89 = (int)this.reader.ReadInt16();
				int num90 = (int)this.reader.ReadInt16();
				float num91 = this.reader.ReadSingle();
				int num92 = (int)(this.reader.ReadByte() - 1);
				byte b6 = this.reader.ReadByte();
				if (Main.netMode == 2)
				{
					if (num90 < 0)
					{
						num90 = 0;
					}
					Main.npc[num89].PlayerInteraction(this.whoAmI);
				}
				if (num90 >= 0)
				{
					Main.npc[num89].StrikeNPC(num90, num91, num92, b6 == 1, false, true);
				}
				else
				{
					Main.npc[num89].life = 0;
					Main.npc[num89].HitEffect(0, 10.0);
					Main.npc[num89].active = false;
				}
				if (Main.netMode != 2)
				{
					return;
				}
				NetMessage.SendData(28, -1, this.whoAmI, "", num89, (float)num90, num91, (float)num92, (int)b6, 0, 0);
				if (Main.npc[num89].life <= 0)
				{
					NetMessage.SendData(23, -1, -1, "", num89, 0f, 0f, 0f, 0, 0, 0);
				}
				else
				{
					Main.npc[num89].netUpdate = true;
				}
				if (Main.npc[num89].realLife < 0)
				{
					return;
				}
				if (Main.npc[Main.npc[num89].realLife].life <= 0)
				{
					NetMessage.SendData(23, -1, -1, "", Main.npc[num89].realLife, 0f, 0f, 0f, 0, 0, 0);
					return;
				}
				Main.npc[Main.npc[num89].realLife].netUpdate = true;
				return;
			}
			case 29:
			{
				int num93 = (int)this.reader.ReadInt16();
				int num94 = (int)this.reader.ReadByte();
				if (Main.netMode == 2)
				{
					num94 = this.whoAmI;
				}
				for (int num95 = 0; num95 < 1000; num95++)
				{
					if (Main.projectile[num95].owner == num94 && Main.projectile[num95].identity == num93 && Main.projectile[num95].active)
					{
						Main.projectile[num95].Kill();
						break;
					}
				}
				if (Main.netMode == 2)
				{
					NetMessage.SendData(29, -1, this.whoAmI, "", num93, (float)num94, 0f, 0f, 0, 0, 0);
					return;
				}
				return;
			}
			case 30:
			{
				int num96 = (int)this.reader.ReadByte();
				if (Main.netMode == 2)
				{
					num96 = this.whoAmI;
				}
				bool flag12 = this.reader.ReadBoolean();
				Main.player[num96].hostile = flag12;
				if (Main.netMode == 2)
				{
					NetMessage.SendData(30, -1, this.whoAmI, "", num96, 0f, 0f, 0f, 0, 0, 0);
					string str = " " + Lang.mp[flag12 ? 11 : 12];
					Color color2 = Main.teamColor[Main.player[num96].team];
					NetMessage.SendData(25, -1, -1, Main.player[num96].name + str, 255, (float)color2.R, (float)color2.G, (float)color2.B, 0, 0, 0);
					return;
				}
				return;
			}
			case 31:
			{
				if (Main.netMode != 2)
				{
					return;
				}
				int x = (int)this.reader.ReadInt16();
				int y = (int)this.reader.ReadInt16();
				int num97 = Chest.FindChest(x, y);
				if (num97 > -1 && Chest.UsingChest(num97) == -1)
				{
					for (int num98 = 0; num98 < 40; num98++)
					{
						NetMessage.SendData(32, this.whoAmI, -1, "", num97, (float)num98, 0f, 0f, 0, 0, 0);
					}
					NetMessage.SendData(33, this.whoAmI, -1, "", num97, 0f, 0f, 0f, 0, 0, 0);
					Main.player[this.whoAmI].chest = num97;
					if (Main.myPlayer == this.whoAmI)
					{
						Main.recBigList = false;
					}
					NetMessage.SendData(80, -1, this.whoAmI, "", this.whoAmI, (float)num97, 0f, 0f, 0, 0, 0);
					return;
				}
				return;
			}
			case 32:
			{
				int num99 = (int)this.reader.ReadInt16();
				int num100 = (int)this.reader.ReadByte();
				int stack4 = (int)this.reader.ReadInt16();
				int pre2 = (int)this.reader.ReadByte();
				int type4 = (int)this.reader.ReadInt16();
				if (Main.chest[num99] == null)
				{
					Main.chest[num99] = new Chest(false);
				}
				if (Main.chest[num99].item[num100] == null)
				{
					Main.chest[num99].item[num100] = new Item();
				}
				Main.chest[num99].item[num100].netDefaults(type4);
				Main.chest[num99].item[num100].Prefix(pre2);
				Main.chest[num99].item[num100].stack = stack4;
				Recipe.FindRecipes();
				return;
			}
			case 33:
			{
				int num101 = (int)this.reader.ReadInt16();
				int num102 = (int)this.reader.ReadInt16();
				int num103 = (int)this.reader.ReadInt16();
				int num104 = (int)this.reader.ReadByte();
				string text5 = string.Empty;
				if (num104 != 0)
				{
					if (num104 <= 20)
					{
						text5 = this.reader.ReadString();
					}
					else if (num104 != 255)
					{
						num104 = 0;
					}
				}
				if (Main.netMode != 1)
				{
					if (num104 != 0)
					{
						int chest = Main.player[this.whoAmI].chest;
						Chest chest2 = Main.chest[chest];
						chest2.name = text5;
						NetMessage.SendData(69, -1, this.whoAmI, text5, chest, (float)chest2.x, (float)chest2.y, 0f, 0, 0, 0);
					}
					Main.player[this.whoAmI].chest = num101;
					Recipe.FindRecipes();
					NetMessage.SendData(80, -1, this.whoAmI, "", this.whoAmI, (float)num101, 0f, 0f, 0, 0, 0);
					return;
				}
				Player player8 = Main.player[Main.myPlayer];
				if (player8.chest == -1)
				{
					Main.playerInventory = true;
					Main.PlaySound(10, -1, -1, 1);
				}
				else if (player8.chest != num101 && num101 != -1)
				{
					Main.playerInventory = true;
					Main.PlaySound(12, -1, -1, 1);
					Main.recBigList = false;
				}
				else if (player8.chest != -1 && num101 == -1)
				{
					Main.PlaySound(11, -1, -1, 1);
					Main.recBigList = false;
				}
				player8.chest = num101;
				player8.chestX = num102;
				player8.chestY = num103;
				Recipe.FindRecipes();
				if (Main.tile[num102, num103].frameX >= 36 && Main.tile[num102, num103].frameX < 72)
				{
					AchievementsHelper.HandleSpecialEvent(Main.player[Main.myPlayer], 16);
					return;
				}
				return;
			}
			case 34:
			{
				byte b7 = this.reader.ReadByte();
				int num105 = (int)this.reader.ReadInt16();
				int num106 = (int)this.reader.ReadInt16();
				int num107 = (int)this.reader.ReadInt16();
				if (Main.netMode == 2)
				{
					if (b7 == 0)
					{
						int num108 = WorldGen.PlaceChest(num105, num106, 21, false, num107);
						if (num108 == -1)
						{
							NetMessage.SendData(34, this.whoAmI, -1, "", (int)b7, (float)num105, (float)num106, (float)num107, num108, 0, 0);
							Item.NewItem(num105 * 16, num106 * 16, 32, 32, Chest.chestItemSpawn[num107], 1, true, 0, false, false);
							return;
						}
						NetMessage.SendData(34, -1, -1, "", (int)b7, (float)num105, (float)num106, (float)num107, num108, 0, 0);
						return;
					}
					else if (b7 == 2)
					{
						int num109 = WorldGen.PlaceChest(num105, num106, 88, false, num107);
						if (num109 == -1)
						{
							NetMessage.SendData(34, this.whoAmI, -1, "", (int)b7, (float)num105, (float)num106, (float)num107, num109, 0, 0);
							Item.NewItem(num105 * 16, num106 * 16, 32, 32, Chest.dresserItemSpawn[num107], 1, true, 0, false, false);
							return;
						}
						NetMessage.SendData(34, -1, -1, "", (int)b7, (float)num105, (float)num106, (float)num107, num109, 0, 0);
						return;
					}
					else
					{
						Tile tile2 = Main.tile[num105, num106];
						if (tile2.type == 21 && b7 == 1)
						{
							if (tile2.frameX % 36 != 0)
							{
								num105--;
							}
							if (tile2.frameY % 36 != 0)
							{
								num106--;
							}
							int number = Chest.FindChest(num105, num106);
							WorldGen.KillTile(num105, num106, false, false, false);
							if (!tile2.active())
							{
								NetMessage.SendData(34, -1, -1, "", (int)b7, (float)num105, (float)num106, 0f, number, 0, 0);
								return;
							}
							return;
						}
						else
						{
							if (tile2.type != 88 || b7 != 3)
							{
								return;
							}
							num105 -= (int)(tile2.frameX % 54 / 18);
							if (tile2.frameY % 36 != 0)
							{
								num106--;
							}
							int number2 = Chest.FindChest(num105, num106);
							WorldGen.KillTile(num105, num106, false, false, false);
							if (!tile2.active())
							{
								NetMessage.SendData(34, -1, -1, "", (int)b7, (float)num105, (float)num106, 0f, number2, 0, 0);
								return;
							}
							return;
						}
					}
				}
				else
				{
					int num110 = (int)this.reader.ReadInt16();
					if (b7 == 0)
					{
						if (num110 == -1)
						{
							WorldGen.KillTile(num105, num106, false, false, false);
							return;
						}
						WorldGen.PlaceChestDirect(num105, num106, 21, num107, num110);
						return;
					}
					else
					{
						if (b7 != 2)
						{
							Chest.DestroyChestDirect(num105, num106, num110);
							WorldGen.KillTile(num105, num106, false, false, false);
							return;
						}
						if (num110 == -1)
						{
							WorldGen.KillTile(num105, num106, false, false, false);
							return;
						}
						WorldGen.PlaceDresserDirect(num105, num106, 88, num107, num110);
						return;
					}
				}
				break;
			}
			case 35:
			{
				int num111 = (int)this.reader.ReadByte();
				if (Main.netMode == 2)
				{
					num111 = this.whoAmI;
				}
				int num112 = (int)this.reader.ReadInt16();
				if (num111 != Main.myPlayer || Main.ServerSideCharacter)
				{
					Main.player[num111].HealEffect(num112, true);
				}
				if (Main.netMode == 2)
				{
					NetMessage.SendData(35, -1, this.whoAmI, "", num111, (float)num112, 0f, 0f, 0, 0, 0);
					return;
				}
				return;
			}
			case 36:
			{
				int num113 = (int)this.reader.ReadByte();
				if (Main.netMode == 2)
				{
					num113 = this.whoAmI;
				}
				Player player9 = Main.player[num113];
				player9.zone1 = this.reader.ReadByte();
				player9.zone2 = this.reader.ReadByte();
				if (Main.netMode == 2)
				{
					NetMessage.SendData(36, -1, this.whoAmI, "", num113, 0f, 0f, 0f, 0, 0, 0);
					return;
				}
				return;
			}
			case 37:
				if (Main.netMode != 1)
				{
					return;
				}
				if (Main.autoPass)
				{
					NetMessage.SendData(38, -1, -1, Netplay.ServerPassword, 0, 0f, 0f, 0f, 0, 0, 0);
					Main.autoPass = false;
					return;
				}
				Netplay.ServerPassword = "";
				Main.menuMode = 31;
				return;
			case 38:
			{
				if (Main.netMode != 2)
				{
					return;
				}
				string a2 = this.reader.ReadString();
				if (a2 == Netplay.ServerPassword)
				{
					Netplay.Clients[this.whoAmI].State = 1;
					NetMessage.SendData(3, this.whoAmI, -1, "", 0, 0f, 0f, 0f, 0, 0, 0);
					return;
				}
				NetMessage.SendData(2, this.whoAmI, -1, Lang.mp[1], 0, 0f, 0f, 0f, 0, 0, 0);
				return;
			}
			case 39:
			{
				if (Main.netMode != 1)
				{
					return;
				}
				int num114 = (int)this.reader.ReadInt16();
				Main.item[num114].owner = 255;
				NetMessage.SendData(22, -1, -1, "", num114, 0f, 0f, 0f, 0, 0, 0);
				return;
			}
			case 40:
			{
				int num115 = (int)this.reader.ReadByte();
				if (Main.netMode == 2)
				{
					num115 = this.whoAmI;
				}
				int talkNPC = (int)this.reader.ReadInt16();
				Main.player[num115].talkNPC = talkNPC;
				if (Main.netMode == 2)
				{
					NetMessage.SendData(40, -1, this.whoAmI, "", num115, 0f, 0f, 0f, 0, 0, 0);
					return;
				}
				return;
			}
			case 41:
			{
				int num116 = (int)this.reader.ReadByte();
				if (Main.netMode == 2)
				{
					num116 = this.whoAmI;
				}
				Player player10 = Main.player[num116];
				float itemRotation = this.reader.ReadSingle();
				int itemAnimation = (int)this.reader.ReadInt16();
				player10.itemRotation = itemRotation;
				player10.itemAnimation = itemAnimation;
				player10.channel = player10.inventory[player10.selectedItem].channel;
				if (Main.netMode == 2)
				{
					NetMessage.SendData(41, -1, this.whoAmI, "", num116, 0f, 0f, 0f, 0, 0, 0);
					return;
				}
				return;
			}
			case 42:
			{
				int num117 = (int)this.reader.ReadByte();
				if (Main.netMode == 2)
				{
					num117 = this.whoAmI;
				}
				else if (Main.myPlayer == num117 && !Main.ServerSideCharacter)
				{
					return;
				}
				int statMana = (int)this.reader.ReadInt16();
				int statManaMax = (int)this.reader.ReadInt16();
				Main.player[num117].statMana = statMana;
				Main.player[num117].statManaMax = statManaMax;
				return;
			}
			case 43:
			{
				int num118 = (int)this.reader.ReadByte();
				if (Main.netMode == 2)
				{
					num118 = this.whoAmI;
				}
				int num119 = (int)this.reader.ReadInt16();
				if (num118 != Main.myPlayer)
				{
					Main.player[num118].ManaEffect(num119);
				}
				if (Main.netMode == 2)
				{
					NetMessage.SendData(43, -1, this.whoAmI, "", num118, (float)num119, 0f, 0f, 0, 0, 0);
					return;
				}
				return;
			}
			case 44:
			{
				int num120 = (int)this.reader.ReadByte();
				if (Main.netMode == 2)
				{
					num120 = this.whoAmI;
				}
				int num121 = (int)(this.reader.ReadByte() - 1);
				int num122 = (int)this.reader.ReadInt16();
				byte b8 = this.reader.ReadByte();
				string text6 = this.reader.ReadString();
				Main.player[num120].KillMe((double)num122, num121, b8 == 1, text6);
				if (Main.netMode == 2)
				{
					NetMessage.SendData(44, -1, this.whoAmI, text6, num120, (float)num121, (float)num122, (float)b8, 0, 0, 0);
					return;
				}
				return;
			}
			case 45:
			{
				int num123 = (int)this.reader.ReadByte();
				if (Main.netMode == 2)
				{
					num123 = this.whoAmI;
				}
				int num124 = (int)this.reader.ReadByte();
				Player player11 = Main.player[num123];
				int team2 = player11.team;
				player11.team = num124;
				Color color3 = Main.teamColor[num124];
				if (Main.netMode == 2)
				{
					NetMessage.SendData(45, -1, this.whoAmI, "", num123, 0f, 0f, 0f, 0, 0, 0);
					string str2 = " " + Lang.mp[13 + num124];
					if (num124 == 5)
					{
						str2 = " " + Lang.mp[22];
					}
					for (int num125 = 0; num125 < 255; num125++)
					{
						if (num125 == this.whoAmI || (team2 > 0 && Main.player[num125].team == team2) || (num124 > 0 && Main.player[num125].team == num124))
						{
							NetMessage.SendData(25, num125, -1, player11.name + str2, 255, (float)color3.R, (float)color3.G, (float)color3.B, 0, 0, 0);
						}
					}
					return;
				}
				return;
			}
			case 46:
			{
				if (Main.netMode != 2)
				{
					return;
				}
				int i2 = (int)this.reader.ReadInt16();
				int j2 = (int)this.reader.ReadInt16();
				int num126 = Sign.ReadSign(i2, j2, true);
				if (num126 >= 0)
				{
					NetMessage.SendData(47, this.whoAmI, -1, "", num126, (float)this.whoAmI, 0f, 0f, 0, 0, 0);
					return;
				}
				return;
			}
			case 47:
			{
				int num127 = (int)this.reader.ReadInt16();
				int x2 = (int)this.reader.ReadInt16();
				int y2 = (int)this.reader.ReadInt16();
				string text7 = this.reader.ReadString();
				string a3 = null;
				if (Main.sign[num127] != null)
				{
					a3 = Main.sign[num127].text;
				}
				Main.sign[num127] = new Sign();
				Main.sign[num127].x = x2;
				Main.sign[num127].y = y2;
				Sign.TextSign(num127, text7);
				int num128 = (int)this.reader.ReadByte();
				if (Main.netMode == 2 && a3 != text7)
				{
					num128 = this.whoAmI;
					NetMessage.SendData(47, -1, this.whoAmI, "", num127, (float)num128, 0f, 0f, 0, 0, 0);
				}
				if (Main.netMode == 1 && num128 == Main.myPlayer && Main.sign[num127] != null)
				{
					Main.playerInventory = false;
					Main.player[Main.myPlayer].talkNPC = -1;
					Main.npcChatCornerItem = 0;
					Main.editSign = false;
					Main.PlaySound(10, -1, -1, 1);
					Main.player[Main.myPlayer].sign = num127;
					Main.npcChatText = Main.sign[num127].text;
					return;
				}
				return;
			}
			case 48:
			{
				int num129 = (int)this.reader.ReadInt16();
				int num130 = (int)this.reader.ReadInt16();
				byte liquid = this.reader.ReadByte();
				byte liquidType = this.reader.ReadByte();
				if (Main.netMode == 2 && Netplay.spamCheck)
				{
					int num131 = this.whoAmI;
					int num132 = (int)(Main.player[num131].position.X + (float)(Main.player[num131].width / 2));
					int num133 = (int)(Main.player[num131].position.Y + (float)(Main.player[num131].height / 2));
					int num134 = 10;
					int num135 = num132 - num134;
					int num136 = num132 + num134;
					int num137 = num133 - num134;
					int num138 = num133 + num134;
					if (num129 < num135 || num129 > num136 || num130 < num137 || num130 > num138)
					{
						NetMessage.BootPlayer(this.whoAmI, "Cheating attempt detected: Liquid spam");
						return;
					}
				}
				if (Main.tile[num129, num130] == null)
				{
					Main.tile[num129, num130] = new Tile();
				}
				lock (Main.tile[num129, num130])
				{
					Main.tile[num129, num130].liquid = liquid;
					Main.tile[num129, num130].liquidType((int)liquidType);
					if (Main.netMode == 2)
					{
						WorldGen.SquareTileFrame(num129, num130, true);
					}
					return;
				}
				goto IL_4824;
			}
			case 49:
				goto IL_4824;
			case 50:
			{
				int num139 = (int)this.reader.ReadByte();
				if (Main.netMode == 2)
				{
					num139 = this.whoAmI;
				}
				else if (num139 == Main.myPlayer && !Main.ServerSideCharacter)
				{
					return;
				}
				Player player12 = Main.player[num139];
				for (int num140 = 0; num140 < 22; num140++)
				{
					player12.buffType[num140] = (int)this.reader.ReadByte();
					if (player12.buffType[num140] > 0)
					{
						player12.buffTime[num140] = 60;
					}
					else
					{
						player12.buffTime[num140] = 0;
					}
				}
				if (Main.netMode == 2)
				{
					NetMessage.SendData(50, -1, this.whoAmI, "", num139, 0f, 0f, 0f, 0, 0, 0);
					return;
				}
				return;
			}
			case 51:
			{
				byte b9 = this.reader.ReadByte();
				byte b10 = this.reader.ReadByte();
				if (b10 == 1)
				{
					NPC.SpawnSkeletron();
					return;
				}
				if (b10 == 2)
				{
					if (Main.netMode == 2)
					{
						NetMessage.SendData(51, -1, this.whoAmI, "", (int)b9, (float)b10, 0f, 0f, 0, 0, 0);
						return;
					}
					Main.PlaySound(2, (int)Main.player[(int)b9].position.X, (int)Main.player[(int)b9].position.Y, 1);
					return;
				}
				else if (b10 == 3)
				{
					if (Main.netMode == 2)
					{
						Main.Sundialing();
						return;
					}
					return;
				}
				else
				{
					if (b10 == 4)
					{
						Main.npc[(int)b9].BigMimicSpawnSmoke();
						return;
					}
					return;
				}
				break;
			}
			case 52:
			{
				int num141 = (int)this.reader.ReadByte();
				int num142 = (int)this.reader.ReadInt16();
				int num143 = (int)this.reader.ReadInt16();
				if (num141 == 1)
				{
					Chest.Unlock(num142, num143);
					if (Main.netMode == 2)
					{
						NetMessage.SendData(52, -1, this.whoAmI, "", 0, (float)num141, (float)num142, (float)num143, 0, 0, 0);
						NetMessage.SendTileSquare(-1, num142, num143, 2);
					}
				}
				if (num141 != 2)
				{
					return;
				}
				WorldGen.UnlockDoor(num142, num143);
				if (Main.netMode == 2)
				{
					NetMessage.SendData(52, -1, this.whoAmI, "", 0, (float)num141, (float)num142, (float)num143, 0, 0, 0);
					NetMessage.SendTileSquare(-1, num142, num143, 2);
					return;
				}
				return;
			}
			case 53:
			{
				int num144 = (int)this.reader.ReadInt16();
				int type5 = (int)this.reader.ReadByte();
				int time = (int)this.reader.ReadInt16();
				Main.npc[num144].AddBuff(type5, time, true);
				if (Main.netMode == 2)
				{
					NetMessage.SendData(54, -1, -1, "", num144, 0f, 0f, 0f, 0, 0, 0);
					return;
				}
				return;
			}
			case 54:
			{
				if (Main.netMode != 1)
				{
					return;
				}
				int num145 = (int)this.reader.ReadInt16();
				NPC nPC2 = Main.npc[num145];
				for (int num146 = 0; num146 < 5; num146++)
				{
					nPC2.buffType[num146] = (int)this.reader.ReadByte();
					nPC2.buffTime[num146] = (int)this.reader.ReadInt16();
				}
				return;
			}
			case 55:
			{
				int num147 = (int)this.reader.ReadByte();
				int num148 = (int)this.reader.ReadByte();
				int num149 = (int)this.reader.ReadInt16();
				if (Main.netMode == 2 && num147 != this.whoAmI && !Main.pvpBuff[num148])
				{
					return;
				}
				if (Main.netMode == 1 && num147 == Main.myPlayer)
				{
					Main.player[num147].AddBuff(num148, num149, true);
					return;
				}
				if (Main.netMode == 2)
				{
					NetMessage.SendData(55, num147, -1, "", num147, (float)num148, (float)num149, 0f, 0, 0, 0);
					return;
				}
				return;
			}
			case 56:
			{
				int num150 = (int)this.reader.ReadInt16();
				if (num150 < 0 || num150 >= 200)
				{
					return;
				}
				string displayName = this.reader.ReadString();
				if (Main.netMode == 1)
				{
					Main.npc[num150].displayName = displayName;
					return;
				}
				if (Main.netMode == 2)
				{
					NetMessage.SendData(56, this.whoAmI, -1, Main.npc[num150].displayName, num150, 0f, 0f, 0f, 0, 0, 0);
					return;
				}
				return;
			}
			case 57:
				if (Main.netMode != 1)
				{
					return;
				}
				WorldGen.tGood = this.reader.ReadByte();
				WorldGen.tEvil = this.reader.ReadByte();
				WorldGen.tBlood = this.reader.ReadByte();
				return;
			case 58:
			{
				int num151 = (int)this.reader.ReadByte();
				if (Main.netMode == 2)
				{
					num151 = this.whoAmI;
				}
				float num152 = this.reader.ReadSingle();
				if (Main.netMode == 2)
				{
					NetMessage.SendData(58, -1, this.whoAmI, "", this.whoAmI, num152, 0f, 0f, 0, 0, 0);
					return;
				}
				Player player13 = Main.player[num151];
				Main.harpNote = num152;
				int style = 26;
				if (player13.inventory[player13.selectedItem].type == 507)
				{
					style = 35;
				}
				Main.PlaySound(2, (int)player13.position.X, (int)player13.position.Y, style);
				return;
			}
			case 59:
			{
				int num153 = (int)this.reader.ReadInt16();
				int num154 = (int)this.reader.ReadInt16();
				Wiring.HitSwitch(num153, num154);
				if (Main.netMode == 2)
				{
					NetMessage.SendData(59, -1, this.whoAmI, "", num153, (float)num154, 0f, 0f, 0, 0, 0);
					return;
				}
				return;
			}
			case 60:
			{
				int num155 = (int)this.reader.ReadInt16();
				int num156 = (int)this.reader.ReadInt16();
				int num157 = (int)this.reader.ReadInt16();
				byte b11 = this.reader.ReadByte();
				if (num155 >= 200)
				{
					NetMessage.BootPlayer(this.whoAmI, "cheating attempt detected: Invalid kick-out");
					return;
				}
				if (Main.netMode == 1)
				{
					Main.npc[num155].homeless = (b11 == 1);
					Main.npc[num155].homeTileX = num156;
					Main.npc[num155].homeTileY = num157;
					return;
				}
				if (b11 == 0)
				{
					WorldGen.kickOut(num155);
					return;
				}
				WorldGen.moveRoom(num156, num157, num155);
				return;
			}
			case 61:
			{
				int plr = (int)this.reader.ReadInt16();
				int num158 = (int)this.reader.ReadInt16();
				if (Main.netMode != 2)
				{
					return;
				}
				if (num158 >= 0 && num158 < 540 && NPCID.Sets.MPAllowedEnemies[num158])
				{
					bool flag14 = !NPC.AnyNPCs(num158);
					if (flag14)
					{
						NPC.SpawnOnPlayer(plr, num158);
						return;
					}
					return;
				}
				else if (num158 == -4)
				{
					if (!Main.dayTime)
					{
						NetMessage.SendData(25, -1, -1, Lang.misc[31], 255, 50f, 255f, 130f, 0, 0, 0);
						Main.startPumpkinMoon();
						NetMessage.SendData(7, -1, -1, "", 0, 0f, 0f, 0f, 0, 0, 0);
						NetMessage.SendData(78, -1, -1, "", 0, 1f, 2f, 1f, 0, 0, 0);
						return;
					}
					return;
				}
				else if (num158 == -5)
				{
					if (!Main.dayTime)
					{
						NetMessage.SendData(25, -1, -1, Lang.misc[34], 255, 50f, 255f, 130f, 0, 0, 0);
						Main.startSnowMoon();
						NetMessage.SendData(7, -1, -1, "", 0, 0f, 0f, 0f, 0, 0, 0);
						NetMessage.SendData(78, -1, -1, "", 0, 1f, 1f, 1f, 0, 0, 0);
						return;
					}
					return;
				}
				else if (num158 == -6)
				{
					if (Main.dayTime && !Main.eclipse)
					{
						NetMessage.SendData(25, -1, -1, Lang.misc[20], 255, 50f, 255f, 130f, 0, 0, 0);
						Main.eclipse = true;
						NetMessage.SendData(7, -1, -1, "", 0, 0f, 0f, 0f, 0, 0, 0);
						return;
					}
					return;
				}
				else
				{
					if (num158 == -7)
					{
						NetMessage.SendData(25, -1, -1, "martian moon toggled", 255, 50f, 255f, 130f, 0, 0, 0);
						Main.invasionDelay = 0;
						Main.StartInvasion(4);
						NetMessage.SendData(7, -1, -1, "", 0, 0f, 0f, 0f, 0, 0, 0);
						NetMessage.SendData(78, -1, -1, "", 0, 1f, (float)(Main.invasionType + 2), 0f, 0, 0, 0);
						return;
					}
					if (num158 == -8)
					{
						if (NPC.downedGolemBoss && Main.hardMode && !NPC.AnyDanger() && !NPC.AnyoneNearCultists())
						{
							WorldGen.StartImpendingDoom();
							NetMessage.SendData(7, -1, -1, "", 0, 0f, 0f, 0f, 0, 0, 0);
							return;
						}
						return;
					}
					else
					{
						if (num158 < 0)
						{
							int num159 = 1;
							if (num158 > -5)
							{
								num159 = -num158;
							}
							if (num159 > 0 && Main.invasionType == 0)
							{
								Main.invasionDelay = 0;
								Main.StartInvasion(num159);
							}
							NetMessage.SendData(78, -1, -1, "", 0, 1f, (float)(Main.invasionType + 2), 0f, 0, 0, 0);
							return;
						}
						return;
					}
				}
				break;
			}
			case 62:
			{
				int num160 = (int)this.reader.ReadByte();
				int num161 = (int)this.reader.ReadByte();
				if (Main.netMode == 2)
				{
					num160 = this.whoAmI;
				}
				if (num161 == 1)
				{
					Main.player[num160].NinjaDodge();
				}
				if (num161 == 2)
				{
					Main.player[num160].ShadowDodge();
				}
				if (Main.netMode == 2)
				{
					NetMessage.SendData(62, -1, this.whoAmI, "", num160, (float)num161, 0f, 0f, 0, 0, 0);
					return;
				}
				return;
			}
			case 63:
			{
				int num162 = (int)this.reader.ReadInt16();
				int num163 = (int)this.reader.ReadInt16();
				byte b12 = this.reader.ReadByte();
				WorldGen.paintTile(num162, num163, b12, false);
				if (Main.netMode == 2)
				{
					NetMessage.SendData(63, -1, this.whoAmI, "", num162, (float)num163, (float)b12, 0f, 0, 0, 0);
					return;
				}
				return;
			}
			case 64:
			{
				int num164 = (int)this.reader.ReadInt16();
				int num165 = (int)this.reader.ReadInt16();
				byte b13 = this.reader.ReadByte();
				WorldGen.paintWall(num164, num165, b13, false);
				if (Main.netMode == 2)
				{
					NetMessage.SendData(64, -1, this.whoAmI, "", num164, (float)num165, (float)b13, 0f, 0, 0, 0);
					return;
				}
				return;
			}
			case 65:
			{
				BitsByte bitsByte15 = this.reader.ReadByte();
				int num166 = (int)this.reader.ReadInt16();
				if (Main.netMode == 2)
				{
					num166 = this.whoAmI;
				}
				Vector2 vector = this.reader.ReadVector2();
				int num167 = 0;
				int num168 = 0;
				if (bitsByte15[0])
				{
					num167++;
				}
				if (bitsByte15[1])
				{
					num167 += 2;
				}
				if (bitsByte15[2])
				{
					num168++;
				}
				if (bitsByte15[3])
				{
					num168 += 2;
				}
				if (num167 == 0)
				{
					Main.player[num166].Teleport(vector, num168, 0);
				}
				else if (num167 == 1)
				{
					Main.npc[num166].Teleport(vector, num168, 0);
				}
				else if (num167 == 2)
				{
					Main.player[num166].Teleport(vector, num168, 0);
					if (Main.netMode == 2)
					{
						RemoteClient.CheckSection(this.whoAmI, vector, 1);
						NetMessage.SendData(65, -1, -1, "", 0, (float)num166, vector.X, vector.Y, num168, 0, 0);
						int num169 = -1;
						float num170 = 9999f;
						for (int num171 = 0; num171 < 255; num171++)
						{
							if (Main.player[num171].active && num171 != this.whoAmI)
							{
								Vector2 vector2 = Main.player[num171].position - Main.player[this.whoAmI].position;
								if (vector2.Length() < num170)
								{
									num170 = vector2.Length();
									num169 = num171;
								}
							}
						}
						if (num169 >= 0)
						{
							NetMessage.SendData(25, -1, -1, Main.player[this.whoAmI].name + " has teleported to " + Main.player[num169].name, 255, 250f, 250f, 0f, 0, 0, 0);
						}
					}
				}
				if (Main.netMode == 2 && num167 == 0)
				{
					NetMessage.SendData(65, -1, this.whoAmI, "", 0, (float)num166, vector.X, vector.Y, num168, 0, 0);
					return;
				}
				return;
			}
			case 66:
			{
				int num172 = (int)this.reader.ReadByte();
				int num173 = (int)this.reader.ReadInt16();
				if (num173 <= 0)
				{
					return;
				}
				Player player14 = Main.player[num172];
				player14.statLife += num173;
				if (player14.statLife > player14.statLifeMax2)
				{
					player14.statLife = player14.statLifeMax2;
				}
				player14.HealEffect(num173, false);
				if (Main.netMode == 2)
				{
					NetMessage.SendData(66, -1, this.whoAmI, "", num172, (float)num173, 0f, 0f, 0, 0, 0);
					return;
				}
				return;
			}
			case 68:
				this.reader.ReadString();
				return;
			case 69:
			{
				int num174 = (int)this.reader.ReadInt16();
				int num175 = (int)this.reader.ReadInt16();
				int num176 = (int)this.reader.ReadInt16();
				if (Main.netMode == 1)
				{
					if (num174 < 0 || num174 >= 1000)
					{
						return;
					}
					Chest chest3 = Main.chest[num174];
					if (chest3 == null)
					{
						chest3 = new Chest(false);
						chest3.x = num175;
						chest3.y = num176;
						Main.chest[num174] = chest3;
					}
					else if (chest3.x != num175 || chest3.y != num176)
					{
						return;
					}
					chest3.name = this.reader.ReadString();
					return;
				}
				else
				{
					if (num174 < -1 || num174 >= 1000)
					{
						return;
					}
					if (num174 == -1)
					{
						num174 = Chest.FindChest(num175, num176);
						if (num174 == -1)
						{
							return;
						}
					}
					Chest chest4 = Main.chest[num174];
					if (chest4.x != num175 || chest4.y != num176)
					{
						return;
					}
					NetMessage.SendData(69, this.whoAmI, -1, chest4.name, num174, (float)num175, (float)num176, 0f, 0, 0, 0);
					return;
				}
				break;
			}
			case 70:
			{
				if (Main.netMode != 2)
				{
					return;
				}
				int num177 = (int)this.reader.ReadInt16();
				int who = (int)this.reader.ReadByte();
				if (Main.netMode == 2)
				{
					who = this.whoAmI;
				}
				if (num177 < 200 && num177 >= 0)
				{
					NPC.CatchNPC(num177, who);
					return;
				}
				return;
			}
			case 71:
			{
				if (Main.netMode != 2)
				{
					return;
				}
				int x3 = this.reader.ReadInt32();
				int y3 = this.reader.ReadInt32();
				int type6 = (int)this.reader.ReadInt16();
				byte style2 = this.reader.ReadByte();
				NPC.ReleaseNPC(x3, y3, type6, (int)style2, this.whoAmI);
				return;
			}
			case 72:
				if (Main.netMode != 1)
				{
					return;
				}
				for (int num178 = 0; num178 < 40; num178++)
				{
					Main.travelShop[num178] = (int)this.reader.ReadInt16();
				}
				return;
			case 73:
				Main.player[this.whoAmI].TeleportationPotion();
				return;
			case 74:
				if (Main.netMode != 1)
				{
					return;
				}
				Main.anglerQuest = (int)this.reader.ReadByte();
				Main.anglerQuestFinished = this.reader.ReadBoolean();
				return;
			case 75:
			{
				if (Main.netMode != 2)
				{
					return;
				}
				string name = Main.player[this.whoAmI].name;
				if (!Main.anglerWhoFinishedToday.Contains(name))
				{
					Main.anglerWhoFinishedToday.Add(name);
					return;
				}
				return;
			}
			case 76:
			{
				int num179 = (int)this.reader.ReadByte();
				if (num179 == Main.myPlayer && !Main.ServerSideCharacter)
				{
					return;
				}
				if (Main.netMode == 2)
				{
					num179 = this.whoAmI;
				}
				Player player15 = Main.player[num179];
				player15.anglerQuestsFinished = this.reader.ReadInt32();
				if (Main.netMode == 2)
				{
					NetMessage.SendData(76, -1, this.whoAmI, "", num179, 0f, 0f, 0f, 0, 0, 0);
					return;
				}
				return;
			}
			case 77:
			{
				short type7 = this.reader.ReadInt16();
				ushort tileType = this.reader.ReadUInt16();
				short x4 = this.reader.ReadInt16();
				short y4 = this.reader.ReadInt16();
				Animation.NewTemporaryAnimation((int)type7, tileType, (int)x4, (int)y4);
				return;
			}
			case 78:
				if (Main.netMode != 1)
				{
					return;
				}
				Main.ReportInvasionProgress(this.reader.ReadInt32(), this.reader.ReadInt32(), (int)this.reader.ReadSByte(), (int)this.reader.ReadSByte());
				return;
			case 79:
			{
				int x5 = (int)this.reader.ReadInt16();
				int y5 = (int)this.reader.ReadInt16();
				short type8 = this.reader.ReadInt16();
				int style3 = (int)this.reader.ReadInt16();
				int num180 = (int)this.reader.ReadByte();
				int random = (int)this.reader.ReadSByte();
				int direction;
				if (this.reader.ReadBoolean())
				{
					direction = 1;
				}
				else
				{
					direction = -1;
				}
				if (Main.netMode == 2)
				{
					Netplay.Clients[this.whoAmI].SpamAddBlock += 1f;
					if (!WorldGen.InWorld(x5, y5, 10) || !Netplay.Clients[this.whoAmI].TileSections[Netplay.GetSectionX(x5), Netplay.GetSectionY(y5)])
					{
						return;
					}
				}
				WorldGen.PlaceObject(x5, y5, (int)type8, false, style3, num180, random, direction);
				if (Main.netMode == 2)
				{
					NetMessage.SendObjectPlacment(this.whoAmI, x5, y5, (int)type8, style3, num180, random, direction);
					return;
				}
				return;
			}
			case 80:
			{
				if (Main.netMode != 1)
				{
					return;
				}
				int num181 = (int)this.reader.ReadByte();
				int num182 = (int)this.reader.ReadInt16();
				if (num182 >= -3 && num182 < 1000)
				{
					Main.player[num181].chest = num182;
					Recipe.FindRecipes();
					return;
				}
				return;
			}
			case 81:
			{
				if (Main.netMode != 1)
				{
					return;
				}
				int x6 = (int)this.reader.ReadSingle();
				int y6 = (int)this.reader.ReadSingle();
				Color color4 = this.reader.ReadRGB();
				string text8 = this.reader.ReadString();
				CombatText.NewText(new Rectangle(x6, y6, 0, 0), color4, text8, false, false);
				return;
			}
			case 82:
				NetManager.Instance.Read(this.reader, this.whoAmI);
				return;
			case 83:
			{
				if (Main.netMode != 1)
				{
					return;
				}
				int num183 = (int)this.reader.ReadInt16();
				int num184 = this.reader.ReadInt32();
				if (num183 >= 0 && num183 < 251)
				{
					NPC.killCount[num183] = num184;
					return;
				}
				return;
			}
			case 84:
			{
				byte b14 = this.reader.ReadByte();
				float stealth = this.reader.ReadSingle();
				Main.player[(int)b14].stealth = stealth;
				if (Main.netMode == 2)
				{
					NetMessage.SendData(84, -1, this.whoAmI, "", (int)b14, 0f, 0f, 0f, 0, 0, 0);
					return;
				}
				return;
			}
			case 85:
			{
				int num185 = this.whoAmI;
				byte b15 = this.reader.ReadByte();
				if (Main.netMode == 2 && num185 < 255 && b15 < 58)
				{
					Chest.ServerPlaceItem(this.whoAmI, (int)b15);
					return;
				}
				return;
			}
			case 86:
			{
				if (Main.netMode != 1)
				{
					return;
				}
				int key = this.reader.ReadInt32();
				bool flag15 = !this.reader.ReadBoolean();
				if (!flag15)
				{
					TileEntity tileEntity = TileEntity.Read(this.reader);
					TileEntity.ByID[tileEntity.ID] = tileEntity;
					TileEntity.ByPosition[tileEntity.Position] = tileEntity;
					return;
				}
				TileEntity tileEntity2;
				if (TileEntity.ByID.TryGetValue(key, out tileEntity2) && (tileEntity2 is TETrainingDummy || tileEntity2 is TEItemFrame))
				{
					TileEntity.ByID.Remove(key);
					TileEntity.ByPosition.Remove(tileEntity2.Position);
					return;
				}
				return;
			}
			case 87:
			{
				if (Main.netMode != 2)
				{
					return;
				}
				int num186 = (int)this.reader.ReadInt16();
				int num187 = (int)this.reader.ReadInt16();
				int num188 = (int)this.reader.ReadByte();
				if (num186 < 0 || num186 >= Main.maxTilesX)
				{
					return;
				}
				if (num187 < 0 || num187 >= Main.maxTilesY)
				{
					return;
				}
				if (TileEntity.ByPosition.ContainsKey(new Point16(num186, num187)))
				{
					return;
				}
				switch (num188)
				{
				case 0:
					if (!TETrainingDummy.ValidTile(num186, num187))
					{
						return;
					}
					TETrainingDummy.Place(num186, num187);
					return;
				case 1:
				{
					if (!TEItemFrame.ValidTile(num186, num187))
					{
						return;
					}
					int number3 = TEItemFrame.Place(num186, num187);
					NetMessage.SendData(86, -1, -1, "", number3, (float)num186, (float)num187, 0f, 0, 0, 0);
					return;
				}
				default:
					return;
				}
				break;
			}
			case 88:
			{
				if (Main.netMode != 1)
				{
					return;
				}
				int num189 = (int)this.reader.ReadInt16();
				if (num189 < 0 || num189 > 400)
				{
					return;
				}
				Item item4 = Main.item[num189];
				BitsByte bitsByte16 = this.reader.ReadByte();
				if (bitsByte16[0])
				{
					item4.color.PackedValue = this.reader.ReadUInt32();
				}
				if (bitsByte16[1])
				{
					item4.damage = (int)this.reader.ReadUInt16();
				}
				if (bitsByte16[2])
				{
					item4.knockBack = this.reader.ReadSingle();
				}
				if (bitsByte16[3])
				{
					item4.useAnimation = (int)this.reader.ReadUInt16();
				}
				if (bitsByte16[4])
				{
					item4.useTime = (int)this.reader.ReadUInt16();
				}
				if (bitsByte16[5])
				{
					item4.shoot = (int)this.reader.ReadInt16();
				}
				if (bitsByte16[6])
				{
					item4.shootSpeed = this.reader.ReadSingle();
				}
				if (!bitsByte16[7])
				{
					return;
				}
				bitsByte16 = this.reader.ReadByte();
				if (bitsByte16[0])
				{
					item4.width = (int)this.reader.ReadInt16();
				}
				if (bitsByte16[1])
				{
					item4.height = (int)this.reader.ReadInt16();
				}
				if (bitsByte16[2])
				{
					item4.scale = this.reader.ReadSingle();
				}
				if (bitsByte16[3])
				{
					item4.ammo = (int)this.reader.ReadInt16();
				}
				if (bitsByte16[4])
				{
					item4.useAmmo = (int)this.reader.ReadInt16();
				}
				if (bitsByte16[5])
				{
					item4.notAmmo = this.reader.ReadBoolean();
					return;
				}
				return;
			}
			case 89:
			{
				if (Main.netMode != 2)
				{
					return;
				}
				int x7 = (int)this.reader.ReadInt16();
				int y7 = (int)this.reader.ReadInt16();
				int netid = (int)this.reader.ReadInt16();
				int prefix = (int)this.reader.ReadByte();
				int stack5 = (int)this.reader.ReadInt16();
				TEItemFrame.TryPlacing(x7, y7, netid, prefix, stack5);
				return;
			}
			case 91:
			{
				if (Main.netMode != 1)
				{
					return;
				}
				int num190 = this.reader.ReadInt32();
				int num191 = (int)this.reader.ReadByte();
				if (num191 != 255)
				{
					int meta = (int)this.reader.ReadUInt16();
					int num192 = (int)this.reader.ReadByte();
					int num193 = (int)this.reader.ReadByte();
					int metadata = 0;
					if (num193 < 0)
					{
						metadata = (int)this.reader.ReadInt16();
					}
					WorldUIAnchor worldUIAnchor = EmoteBubble.DeserializeNetAnchor(num191, meta);
					lock (EmoteBubble.byID)
					{
						if (!EmoteBubble.byID.ContainsKey(num190))
						{
							EmoteBubble.byID[num190] = new EmoteBubble(num193, worldUIAnchor, num192);
						}
						else
						{
							EmoteBubble.byID[num190].lifeTime = num192;
							EmoteBubble.byID[num190].lifeTimeStart = num192;
							EmoteBubble.byID[num190].emote = num193;
							EmoteBubble.byID[num190].anchor = worldUIAnchor;
						}
						EmoteBubble.byID[num190].ID = num190;
						EmoteBubble.byID[num190].metadata = metadata;
						return;
					}
					goto IL_63B1;
				}
				if (EmoteBubble.byID.ContainsKey(num190))
				{
					EmoteBubble.byID.Remove(num190);
					return;
				}
				return;
			}
			case 92:
				goto IL_63B1;
			case 95:
			{
				if (Main.netMode != 2)
				{
					return;
				}
				ushort num194 = this.reader.ReadUInt16();
				if (num194 < 0 || num194 >= 1000)
				{
					return;
				}
				Projectile projectile2 = Main.projectile[(int)num194];
				if (projectile2.type != 602)
				{
					return;
				}
				projectile2.Kill();
				if (Main.netMode != 0)
				{
					NetMessage.SendData(29, -1, -1, "", projectile2.whoAmI, (float)projectile2.owner, 0f, 0f, 0, 0, 0);
					return;
				}
				return;
			}
			case 96:
			{
				int num195 = (int)this.reader.ReadByte();
				Player player16 = Main.player[num195];
				int num196 = (int)this.reader.ReadInt16();
				Vector2 newPos = this.reader.ReadVector2();
				Vector2 velocity4 = this.reader.ReadVector2();
				int lastPortalColorIndex = num196 + ((num196 % 2 == 0) ? 1 : -1);
				player16.lastPortalColorIndex = lastPortalColorIndex;
				player16.Teleport(newPos, 4, num196);
				player16.velocity = velocity4;
				return;
			}
			case 97:
				if (Main.netMode != 1)
				{
					return;
				}
				AchievementsHelper.NotifyNPCKilledDirect(Main.player[Main.myPlayer], (int)this.reader.ReadInt16());
				return;
			case 98:
				if (Main.netMode != 1)
				{
					return;
				}
				AchievementsHelper.NotifyProgressionEvent((int)this.reader.ReadInt16());
				return;
			case 99:
			{
				int num197 = (int)this.reader.ReadByte();
				if (Main.netMode == 2)
				{
					num197 = this.whoAmI;
				}
				Player player17 = Main.player[num197];
				player17.MinionTargetPoint = this.reader.ReadVector2();
				if (Main.netMode == 2)
				{
					NetMessage.SendData(99, -1, this.whoAmI, "", num197, 0f, 0f, 0f, 0, 0, 0);
					return;
				}
				return;
			}
			case 100:
			{
				int num198 = (int)this.reader.ReadUInt16();
				NPC nPC3 = Main.npc[num198];
				int num199 = (int)this.reader.ReadInt16();
				Vector2 newPos2 = this.reader.ReadVector2();
				Vector2 velocity5 = this.reader.ReadVector2();
				int lastPortalColorIndex2 = num199 + ((num199 % 2 == 0) ? 1 : -1);
				nPC3.lastPortalColorIndex = lastPortalColorIndex2;
				nPC3.Teleport(newPos2, 4, num199);
				nPC3.velocity = velocity5;
				return;
			}
			case 101:
				if (Main.netMode == 2)
				{
					return;
				}
				NPC.ShieldStrengthTowerSolar = (int)this.reader.ReadUInt16();
				NPC.ShieldStrengthTowerVortex = (int)this.reader.ReadUInt16();
				NPC.ShieldStrengthTowerNebula = (int)this.reader.ReadUInt16();
				NPC.ShieldStrengthTowerStardust = (int)this.reader.ReadUInt16();
				if (NPC.ShieldStrengthTowerSolar < 0)
				{
					NPC.ShieldStrengthTowerSolar = 0;
				}
				if (NPC.ShieldStrengthTowerVortex < 0)
				{
					NPC.ShieldStrengthTowerVortex = 0;
				}
				if (NPC.ShieldStrengthTowerNebula < 0)
				{
					NPC.ShieldStrengthTowerNebula = 0;
				}
				if (NPC.ShieldStrengthTowerStardust < 0)
				{
					NPC.ShieldStrengthTowerStardust = 0;
				}
				if (NPC.ShieldStrengthTowerSolar > NPC.LunarShieldPowerExpert)
				{
					NPC.ShieldStrengthTowerSolar = NPC.LunarShieldPowerExpert;
				}
				if (NPC.ShieldStrengthTowerVortex > NPC.LunarShieldPowerExpert)
				{
					NPC.ShieldStrengthTowerVortex = NPC.LunarShieldPowerExpert;
				}
				if (NPC.ShieldStrengthTowerNebula > NPC.LunarShieldPowerExpert)
				{
					NPC.ShieldStrengthTowerNebula = NPC.LunarShieldPowerExpert;
				}
				if (NPC.ShieldStrengthTowerStardust > NPC.LunarShieldPowerExpert)
				{
					NPC.ShieldStrengthTowerStardust = NPC.LunarShieldPowerExpert;
					return;
				}
				return;
			case 102:
			{
				int num200 = (int)this.reader.ReadByte();
				byte b16 = this.reader.ReadByte();
				Vector2 other = this.reader.ReadVector2();
				if (Main.netMode == 2)
				{
					num200 = this.whoAmI;
					NetMessage.SendData(102, -1, -1, "", num200, (float)b16, other.X, other.Y, 0, 0, 0);
					return;
				}
				Player player18 = Main.player[num200];
				for (int num201 = 0; num201 < 255; num201++)
				{
					Player player19 = Main.player[num201];
					if (player19.active && !player19.dead && (player18.team == 0 || player18.team == player19.team) && player19.Distance(other) < 700f)
					{
						Vector2 value = player18.Center - player19.Center;
						Vector2 vector3 = Vector2.Normalize(value);
						if (!vector3.HasNaNs())
						{
							int type9 = 90;
							float num202 = 0f;
							float num203 = 0.209439516f;
							Vector2 spinningpoint = new Vector2(0f, -8f);
							Vector2 value2 = new Vector2(-3f);
							float num204 = 0f;
							float num205 = 0.005f;
							byte b17 = b16;
							if (b17 != 173)
							{
								if (b17 != 176)
								{
									if (b17 == 179)
									{
										type9 = 86;
									}
								}
								else
								{
									type9 = 88;
								}
							}
							else
							{
								type9 = 90;
							}
							int num206 = 0;
							while ((float)num206 < value.Length() / 6f)
							{
								Vector2 position4 = player19.Center + 6f * (float)num206 * vector3 + spinningpoint.RotatedBy((double)num202, default(Vector2)) + value2;
								num202 += num203;
								int num207 = Dust.NewDust(position4, 6, 6, type9, 0f, 0f, 100, default(Color), 1.5f);
								Main.dust[num207].noGravity = true;
								Main.dust[num207].velocity = Vector2.Zero;
								num204 = (Main.dust[num207].fadeIn = num204 + num205);
								Main.dust[num207].velocity += vector3 * 1.5f;
								num206++;
							}
						}
						player19.NebulaLevelup((int)b16);
					}
				}
				return;
			}
			case 103:
				if (Main.netMode == 1)
				{
					NPC.MoonLordCountdown = this.reader.ReadInt32();
					return;
				}
				return;
			case 104:
			{
				if (Main.netMode != 1 || Main.npcShop <= 0)
				{
					return;
				}
				Item[] item5 = Main.instance.shop[Main.npcShop].item;
				int num208 = (int)this.reader.ReadByte();
				int type10 = (int)this.reader.ReadInt16();
				int stack6 = (int)this.reader.ReadInt16();
				int pre3 = (int)this.reader.ReadByte();
				int value3 = this.reader.ReadInt32();
				BitsByte bitsByte17 = this.reader.ReadByte();
				if (num208 < item5.Length)
				{
					item5[num208] = new Item();
					item5[num208].netDefaults(type10);
					item5[num208].stack = stack6;
					item5[num208].Prefix(pre3);
					item5[num208].value = value3;
					item5[num208].buyOnce = bitsByte17[0];
					return;
				}
				return;
			}
			default:
				return;
			}
			if (Main.netMode != 2)
			{
				return;
			}
			if (Netplay.Clients[this.whoAmI].State == 1)
			{
				Netplay.Clients[this.whoAmI].State = 2;
				Netplay.Clients[this.whoAmI].ResetSections();
			}
			NetMessage.SendData(7, this.whoAmI, -1, "", 0, 0f, 0f, 0f, 0, 0, 0);
			Main.SyncAnInvasion(this.whoAmI);
			return;
			IL_4824:
			if (Netplay.Connection.State == 6)
			{
				Netplay.Connection.State = 10;
				Main.ActivePlayerFileData.StartPlayTimer();
				Player.EnterWorld(Main.player[Main.myPlayer]);
				Main.player[Main.myPlayer].Spawn();
				return;
			}
			return;
			IL_63B1:
			int num209 = (int)this.reader.ReadInt16();
			float num210 = this.reader.ReadSingle();
			float num211 = this.reader.ReadSingle();
			float num212 = this.reader.ReadSingle();
			if (num209 < 0 || num209 > 200)
			{
				return;
			}
			if (Main.netMode == 1)
			{
				Main.npc[num209].moneyPing(new Vector2(num211, num212));
				Main.npc[num209].extraValue = num210;
				return;
			}
			Main.npc[num209].extraValue += num210;
			NetMessage.SendData(92, -1, -1, "", num209, Main.npc[num209].extraValue, num211, num212, 0, 0, 0);
			return;
		}
	}
}
