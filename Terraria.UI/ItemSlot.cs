using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using Terraria.GameContent.Achievements;
using Terraria.GameContent.UI.Chat;
using Terraria.ID;
using Terraria.UI.Chat;
namespace Terraria.UI
{
	public class ItemSlot
	{
		public class Options
		{
			public static bool DisableLeftShiftTrashCan = false;
			public static bool HighlightNewItems = true;
		}
		public class Context
		{
			public const int InventoryItem = 0;
			public const int InventoryCoin = 1;
			public const int InventoryAmmo = 2;
			public const int ChestItem = 3;
			public const int BankItem = 4;
			public const int PrefixItem = 5;
			public const int TrashItem = 6;
			public const int GuideItem = 7;
			public const int EquipArmor = 8;
			public const int EquipArmorVanity = 9;
			public const int EquipAccessory = 10;
			public const int EquipAccessoryVanity = 11;
			public const int EquipDye = 12;
			public const int HotbarItem = 13;
			public const int ChatItem = 14;
			public const int ShopItem = 15;
			public const int EquipGrapple = 16;
			public const int EquipMount = 17;
			public const int EquipMinecart = 18;
			public const int EquipPet = 19;
			public const int EquipLight = 20;
			public const int MouseItem = 21;
			public const int CraftingMaterial = 22;
			public const int Count = 23;
		}
		private static Item[] singleSlotArray;
		private static bool[] canFavoriteAt;
		private static int dyeSlotCount;
		private static int accSlotCount;
		static ItemSlot()
		{
			ItemSlot.singleSlotArray = new Item[1];
			ItemSlot.canFavoriteAt = new bool[23];
			ItemSlot.dyeSlotCount = 0;
			ItemSlot.accSlotCount = 0;
			ItemSlot.canFavoriteAt[0] = true;
			ItemSlot.canFavoriteAt[1] = true;
			ItemSlot.canFavoriteAt[2] = true;
		}
		public static void Handle(ref Item inv, int context = 0)
		{
			ItemSlot.singleSlotArray[0] = inv;
			ItemSlot.Handle(ItemSlot.singleSlotArray, context, 0);
			inv = ItemSlot.singleSlotArray[0];
			Recipe.FindRecipes();
		}
		public static void Handle(Item[] inv, int context = 0, int slot = 0)
		{
			ItemSlot.OverrideHover(inv, context, slot);
			if (Main.mouseLeftRelease && Main.mouseLeft)
			{
				ItemSlot.LeftClick(inv, context, slot);
				Recipe.FindRecipes();
			}
			else
			{
				ItemSlot.RightClick(inv, context, slot);
			}
			ItemSlot.MouseHover(inv, context, slot);
		}
		public static void OverrideHover(Item[] inv, int context = 0, int slot = 0)
		{
			Item item = inv[slot];
			if (Main.keyState.IsKeyDown(Keys.LeftShift) && item.type > 0 && item.stack > 0 && !inv[slot].favorited)
			{
				switch (context)
				{
				case 0:
				case 1:
				case 2:
					if (Main.npcShop > 0 && !item.favorited)
					{
						Main.cursorOverride = 10;
					}
					else if (Main.player[Main.myPlayer].chest != -1)
					{
						if (ChestUI.TryPlacingInChest(item, true))
						{
							Main.cursorOverride = 9;
						}
					}
					else
					{
						Main.cursorOverride = 6;
					}
					break;
				case 3:
				case 4:
					if (Main.player[Main.myPlayer].ItemSpace(item))
					{
						Main.cursorOverride = 8;
					}
					break;
				case 5:
				case 8:
				case 9:
				case 10:
				case 11:
				case 12:
				case 16:
				case 17:
				case 18:
				case 19:
				case 20:
					if (Main.player[Main.myPlayer].ItemSpace(inv[slot]))
					{
						Main.cursorOverride = 7;
					}
					break;
				}
			}
			if (Main.keyState.IsKeyDown(Main.FavoriteKey) && ItemSlot.canFavoriteAt[context])
			{
				if (item.type > 0 && item.stack > 0 && Main.chatMode)
				{
					Main.cursorOverride = 2;
					return;
				}
				if (item.type > 0 && item.stack > 0)
				{
					Main.cursorOverride = 3;
				}
			}
		}
		private static bool OverrideLeftClick(Item[] inv, int context = 0, int slot = 0)
		{
			Item item = inv[slot];
			if (Main.cursorOverride == 2)
			{
				if (ChatManager.AddChatText(Main.fontMouseText, ItemTagHandler.GenerateTag(item), Vector2.One))
				{
					Main.PlaySound(12, -1, -1, 1);
				}
				return true;
			}
			if (Main.cursorOverride == 3)
			{
				if (!ItemSlot.canFavoriteAt[context])
				{
					return false;
				}
				item.favorited = !item.favorited;
				Main.PlaySound(12, -1, -1, 1);
				return true;
			}
			else
			{
				if (Main.cursorOverride == 7)
				{
					inv[slot] = Main.player[Main.myPlayer].GetItem(Main.myPlayer, inv[slot], false, true);
					Main.PlaySound(12, -1, -1, 1);
					return true;
				}
				if (Main.cursorOverride == 8)
				{
					inv[slot] = Main.player[Main.myPlayer].GetItem(Main.myPlayer, inv[slot], false, true);
					if (Main.player[Main.myPlayer].chest > -1)
					{
						NetMessage.SendData(32, -1, -1, "", Main.player[Main.myPlayer].chest, (float)slot, 0f, 0f, 0, 0, 0);
					}
					return true;
				}
				if (Main.cursorOverride == 9)
				{
					ChestUI.TryPlacingInChest(inv[slot], false);
					return true;
				}
				return false;
			}
		}
		public static void LeftClick(ref Item inv, int context = 0)
		{
			ItemSlot.singleSlotArray[0] = inv;
			ItemSlot.LeftClick(ItemSlot.singleSlotArray, context, 0);
			inv = ItemSlot.singleSlotArray[0];
		}
		public static void LeftClick(Item[] inv, int context = 0, int slot = 0)
		{
			if (ItemSlot.OverrideLeftClick(inv, context, slot))
			{
				return;
			}
			inv[slot].newAndShiny = false;
			Player player = Main.player[Main.myPlayer];
			bool flag = false;
			switch (context)
			{
			case 0:
			case 1:
			case 2:
			case 3:
			case 4:
				flag = (player.chest == -1);
				break;
			}
			if (Main.keyState.IsKeyDown(Keys.LeftShift) && flag)
			{
				if (inv[slot].type > 0)
				{
					if (Main.npcShop > 0 && !inv[slot].favorited)
					{
						Chest chest = Main.instance.shop[Main.npcShop];
						if (inv[slot].type >= 71 && inv[slot].type <= 74)
						{
							return;
						}
						if (player.SellItem(inv[slot].value, inv[slot].stack))
						{
							chest.AddShop(inv[slot]);
							inv[slot].SetDefaults(0, false);
							Main.PlaySound(18, -1, -1, 1);
							Recipe.FindRecipes();
							return;
						}
						if (inv[slot].value == 0)
						{
							chest.AddShop(inv[slot]);
							inv[slot].SetDefaults(0, false);
							Main.PlaySound(7, -1, -1, 1);
							Recipe.FindRecipes();
							return;
						}
					}
					else if (!inv[slot].favorited && !ItemSlot.Options.DisableLeftShiftTrashCan)
					{
						Main.PlaySound(7, -1, -1, 1);
						player.trashItem = inv[slot].Clone();
						inv[slot].SetDefaults(0, false);
						if (context == 3 && Main.netMode == 1)
						{
							NetMessage.SendData(32, -1, -1, "", player.chest, (float)slot, 0f, 0f, 0, 0, 0);
						}
						Recipe.FindRecipes();
						return;
					}
				}
			}
			else if ((player.selectedItem != slot || player.itemAnimation <= 0) && player.itemTime == 0)
			{
				int num = ItemSlot.PickItemMovementAction(inv, context, slot, Main.mouseItem);
				if (num == 0)
				{
					if (context == 6 && Main.mouseItem.type != 0)
					{
						inv[slot].SetDefaults(0, false);
					}
					Utils.Swap<Item>(ref inv[slot], ref Main.mouseItem);
					if (inv[slot].stack > 0)
					{
						if (context != 0)
						{
							switch (context)
							{
							case 8:
							case 9:
							case 10:
							case 11:
							case 12:
							case 16:
							case 17:
								AchievementsHelper.HandleOnEquip(player, inv[slot], context);
								break;
							}
						}
						else
						{
							AchievementsHelper.NotifyItemPickup(player, inv[slot]);
						}
					}
					if (inv[slot].type == 0 || inv[slot].stack < 1)
					{
						inv[slot] = new Item();
					}
					if (Main.mouseItem.IsTheSameAs(inv[slot]))
					{
						Utils.Swap<bool>(ref inv[slot].favorited, ref Main.mouseItem.favorited);
						if (inv[slot].stack != inv[slot].maxStack && Main.mouseItem.stack != Main.mouseItem.maxStack)
						{
							if (Main.mouseItem.stack + inv[slot].stack <= Main.mouseItem.maxStack)
							{
								inv[slot].stack += Main.mouseItem.stack;
								Main.mouseItem.stack = 0;
							}
							else
							{
								int num2 = Main.mouseItem.maxStack - inv[slot].stack;
								inv[slot].stack += num2;
								Main.mouseItem.stack -= num2;
							}
						}
					}
					if (Main.mouseItem.type == 0 || Main.mouseItem.stack < 1)
					{
						Main.mouseItem = new Item();
					}
					if (Main.mouseItem.type > 0 || inv[slot].type > 0)
					{
						Recipe.FindRecipes();
						Main.PlaySound(7, -1, -1, 1);
					}
					if (context == 3 && Main.netMode == 1)
					{
						NetMessage.SendData(32, -1, -1, "", player.chest, (float)slot, 0f, 0f, 0, 0, 0);
					}
				}
				else if (num == 1)
				{
					if (Main.mouseItem.stack == 1 && Main.mouseItem.type > 0 && inv[slot].type > 0 && inv[slot].IsNotTheSameAs(Main.mouseItem))
					{
						Utils.Swap<Item>(ref inv[slot], ref Main.mouseItem);
						Main.PlaySound(7, -1, -1, 1);
						if (inv[slot].stack > 0)
						{
							if (context != 0)
							{
								switch (context)
								{
								case 8:
								case 9:
								case 10:
								case 11:
								case 12:
								case 16:
								case 17:
									AchievementsHelper.HandleOnEquip(player, inv[slot], context);
									break;
								}
							}
							else
							{
								AchievementsHelper.NotifyItemPickup(player, inv[slot]);
							}
						}
					}
					else if (Main.mouseItem.type == 0 && inv[slot].type > 0)
					{
						Utils.Swap<Item>(ref inv[slot], ref Main.mouseItem);
						if (inv[slot].type == 0 || inv[slot].stack < 1)
						{
							inv[slot] = new Item();
						}
						if (Main.mouseItem.type == 0 || Main.mouseItem.stack < 1)
						{
							Main.mouseItem = new Item();
						}
						if (Main.mouseItem.type > 0 || inv[slot].type > 0)
						{
							Recipe.FindRecipes();
							Main.PlaySound(7, -1, -1, 1);
						}
					}
					else if (Main.mouseItem.type > 0 && inv[slot].type == 0)
					{
						if (Main.mouseItem.stack == 1)
						{
							Utils.Swap<Item>(ref inv[slot], ref Main.mouseItem);
							if (inv[slot].type == 0 || inv[slot].stack < 1)
							{
								inv[slot] = new Item();
							}
							if (Main.mouseItem.type == 0 || Main.mouseItem.stack < 1)
							{
								Main.mouseItem = new Item();
							}
							if (Main.mouseItem.type > 0 || inv[slot].type > 0)
							{
								Recipe.FindRecipes();
								Main.PlaySound(7, -1, -1, 1);
							}
						}
						else
						{
							Main.mouseItem.stack--;
							inv[slot].SetDefaults(Main.mouseItem.type, false);
							Recipe.FindRecipes();
							Main.PlaySound(7, -1, -1, 1);
						}
						if (inv[slot].stack > 0)
						{
							if (context != 0)
							{
								switch (context)
								{
								case 8:
								case 9:
								case 10:
								case 11:
								case 12:
								case 16:
								case 17:
									AchievementsHelper.HandleOnEquip(player, inv[slot], context);
									break;
								}
							}
							else
							{
								AchievementsHelper.NotifyItemPickup(player, inv[slot]);
							}
						}
					}
				}
				else if (num == 2)
				{
					if (Main.mouseItem.stack == 1 && Main.mouseItem.dye > 0 && inv[slot].type > 0 && inv[slot].type != Main.mouseItem.type)
					{
						Utils.Swap<Item>(ref inv[slot], ref Main.mouseItem);
						Main.PlaySound(7, -1, -1, 1);
						if (inv[slot].stack > 0)
						{
							if (context != 0)
							{
								switch (context)
								{
								case 8:
								case 9:
								case 10:
								case 11:
								case 12:
								case 16:
								case 17:
									AchievementsHelper.HandleOnEquip(player, inv[slot], context);
									break;
								}
							}
							else
							{
								AchievementsHelper.NotifyItemPickup(player, inv[slot]);
							}
						}
					}
					else if (Main.mouseItem.type == 0 && inv[slot].type > 0)
					{
						Utils.Swap<Item>(ref inv[slot], ref Main.mouseItem);
						if (inv[slot].type == 0 || inv[slot].stack < 1)
						{
							inv[slot] = new Item();
						}
						if (Main.mouseItem.type == 0 || Main.mouseItem.stack < 1)
						{
							Main.mouseItem = new Item();
						}
						if (Main.mouseItem.type > 0 || inv[slot].type > 0)
						{
							Recipe.FindRecipes();
							Main.PlaySound(7, -1, -1, 1);
						}
					}
					else if (Main.mouseItem.dye > 0 && inv[slot].type == 0)
					{
						if (Main.mouseItem.stack == 1)
						{
							Utils.Swap<Item>(ref inv[slot], ref Main.mouseItem);
							if (inv[slot].type == 0 || inv[slot].stack < 1)
							{
								inv[slot] = new Item();
							}
							if (Main.mouseItem.type == 0 || Main.mouseItem.stack < 1)
							{
								Main.mouseItem = new Item();
							}
							if (Main.mouseItem.type > 0 || inv[slot].type > 0)
							{
								Recipe.FindRecipes();
								Main.PlaySound(7, -1, -1, 1);
							}
						}
						else
						{
							Main.mouseItem.stack--;
							inv[slot].SetDefaults(Main.mouseItem.type, false);
							Recipe.FindRecipes();
							Main.PlaySound(7, -1, -1, 1);
						}
						if (inv[slot].stack > 0)
						{
							if (context != 0)
							{
								switch (context)
								{
								case 8:
								case 9:
								case 10:
								case 11:
								case 12:
								case 16:
								case 17:
									AchievementsHelper.HandleOnEquip(player, inv[slot], context);
									break;
								}
							}
							else
							{
								AchievementsHelper.NotifyItemPickup(player, inv[slot]);
							}
						}
					}
				}
				else if (num == 3)
				{
					Main.mouseItem.netDefaults(inv[slot].netID);
					if (inv[slot].buyOnce)
					{
						Main.mouseItem.Prefix((int)inv[slot].prefix);
					}
					else
					{
						Main.mouseItem.Prefix(-1);
					}
					Main.mouseItem.position = player.Center - new Vector2((float)Main.mouseItem.width, (float)Main.mouseItem.headSlot) / 2f;
					ItemText.NewText(Main.mouseItem, Main.mouseItem.stack, false, false);
					if (inv[slot].buyOnce && --inv[slot].stack <= 0)
					{
						inv[slot].SetDefaults(0, false);
					}
					if (inv[slot].value > 0)
					{
						Main.PlaySound(18, -1, -1, 1);
					}
					else
					{
						Main.PlaySound(7, -1, -1, 1);
					}
				}
				else if (num == 4)
				{
					Chest chest2 = Main.instance.shop[Main.npcShop];
					if (player.SellItem(Main.mouseItem.value, Main.mouseItem.stack))
					{
						chest2.AddShop(Main.mouseItem);
						Main.mouseItem.SetDefaults(0, false);
						Main.PlaySound(18, -1, -1, 1);
					}
					else if (Main.mouseItem.value == 0)
					{
						chest2.AddShop(Main.mouseItem);
						Main.mouseItem.SetDefaults(0, false);
						Main.PlaySound(7, -1, -1, 1);
					}
					Recipe.FindRecipes();
				}
				switch (context)
				{
				case 0:
				case 1:
				case 2:
				case 5:
					return;
				case 3:
				case 4:
					IL_A97:
					inv[slot].favorited = false;
					return;
				}
				//goto IL_A97;
                inv[slot].favorited = false;
			}
		}
		public static int PickItemMovementAction(Item[] inv, int context, int slot, Item checkItem)
		{
			Player player = Main.player[Main.myPlayer];
			int result = -1;
			if (context == 0)
			{
				result = 0;
			}
			else if (context == 1)
			{
				if (checkItem.type == 0 || checkItem.type == 71 || checkItem.type == 72 || checkItem.type == 73 || checkItem.type == 74)
				{
					result = 0;
				}
			}
			else if (context == 2)
			{
				if (((checkItem.type == 0 || checkItem.ammo > 0 || checkItem.bait > 0) && !checkItem.notAmmo) || checkItem.type == 530)
				{
					result = 0;
				}
			}
			else if (context == 3)
			{
				result = 0;
			}
			else if (context == 4)
			{
				result = 0;
			}
			else if (context == 5)
			{
				if (checkItem.Prefix(-3) || checkItem.type == 0)
				{
					result = 0;
				}
			}
			else if (context == 6)
			{
				result = 0;
			}
			else if (context == 7)
			{
				if (checkItem.material || checkItem.type == 0)
				{
					result = 0;
				}
			}
			else if (context == 8)
			{
				if (checkItem.type == 0 || (checkItem.headSlot > -1 && slot == 0) || (checkItem.bodySlot > -1 && slot == 1) || (checkItem.legSlot > -1 && slot == 2))
				{
					result = 1;
				}
			}
			else if (context == 9)
			{
				if (checkItem.type == 0 || (checkItem.headSlot > -1 && slot == 10) || (checkItem.bodySlot > -1 && slot == 11) || (checkItem.legSlot > -1 && slot == 12))
				{
					result = 1;
				}
			}
			else if (context == 10)
			{
				if (checkItem.type == 0 || (checkItem.accessory && !ItemSlot.AccCheck(checkItem, slot)))
				{
					result = 1;
				}
			}
			else if (context == 11)
			{
				if (checkItem.type == 0 || (checkItem.accessory && !ItemSlot.AccCheck(checkItem, slot)))
				{
					result = 1;
				}
			}
			else if (context == 12)
			{
				result = 2;
			}
			else if (context == 15)
			{
				if (checkItem.type == 0 && inv[slot].type > 0)
				{
					if (player.BuyItem(inv[slot].value))
					{
						result = 3;
					}
				}
				else if (inv[slot].type == 0 && checkItem.type > 0 && (checkItem.type < 71 || checkItem.type > 74))
				{
					result = 4;
				}
			}
			else if (context == 16)
			{
				if (checkItem.type == 0 || Main.projHook[checkItem.shoot])
				{
					result = 1;
				}
			}
			else if (context == 17)
			{
				if (checkItem.type == 0 || (checkItem.mountType != -1 && !MountID.Sets.Cart[checkItem.mountType]))
				{
					result = 1;
				}
			}
			else if (context == 19)
			{
				if (checkItem.type == 0 || (checkItem.buffType > 0 && Main.vanityPet[checkItem.buffType] && !Main.lightPet[checkItem.buffType]))
				{
					result = 1;
				}
			}
			else if (context == 18)
			{
				if (checkItem.type == 0 || (checkItem.mountType != -1 && MountID.Sets.Cart[checkItem.mountType]))
				{
					result = 1;
				}
			}
			else if (context == 20 && (checkItem.type == 0 || (checkItem.buffType > 0 && Main.lightPet[checkItem.buffType])))
			{
				result = 1;
			}
			return result;
		}
		public static void RightClick(ref Item inv, int context = 0)
		{
			ItemSlot.singleSlotArray[0] = inv;
			ItemSlot.RightClick(ItemSlot.singleSlotArray, context, 0);
			inv = ItemSlot.singleSlotArray[0];
		}
		public static void RightClick(Item[] inv, int context = 0, int slot = 0)
		{
			Player player = Main.player[Main.myPlayer];
			inv[slot].newAndShiny = false;
			if (player.itemAnimation > 0)
			{
				return;
			}
			bool flag = false;
			if (context == 0)
			{
				flag = true;
				if (Main.mouseRight && inv[slot].type >= 3318 && inv[slot].type <= 3332)
				{
					if (Main.mouseRightRelease)
					{
						player.OpenBossBag(inv[slot].type);
						inv[slot].stack--;
						if (inv[slot].stack == 0)
						{
							inv[slot].SetDefaults(0, false);
						}
						Main.PlaySound(7, -1, -1, 1);
						Main.stackSplit = 30;
						Main.mouseRightRelease = false;
						Recipe.FindRecipes();
					}
				}
				else if (Main.mouseRight && ((inv[slot].type >= 2334 && inv[slot].type <= 2336) || (inv[slot].type >= 3203 && inv[slot].type <= 3208)))
				{
					if (Main.mouseRightRelease)
					{
						player.openCrate(inv[slot].type);
						inv[slot].stack--;
						if (inv[slot].stack == 0)
						{
							inv[slot].SetDefaults(0, false);
						}
						Main.PlaySound(7, -1, -1, 1);
						Main.stackSplit = 30;
						Main.mouseRightRelease = false;
						Recipe.FindRecipes();
					}
				}
				else if (Main.mouseRight && inv[slot].type == 3093)
				{
					if (Main.mouseRightRelease)
					{
						player.openHerbBag();
						inv[slot].stack--;
						if (inv[slot].stack == 0)
						{
							inv[slot].SetDefaults(0, false);
						}
						Main.PlaySound(7, -1, -1, 1);
						Main.stackSplit = 30;
						Main.mouseRightRelease = false;
						Recipe.FindRecipes();
					}
				}
				else if (Main.mouseRight && inv[slot].type == 1774)
				{
					if (Main.mouseRightRelease)
					{
						inv[slot].stack--;
						if (inv[slot].stack == 0)
						{
							inv[slot].SetDefaults(0, false);
						}
						Main.PlaySound(7, -1, -1, 1);
						Main.stackSplit = 30;
						Main.mouseRightRelease = false;
						player.openGoodieBag();
						Recipe.FindRecipes();
					}
				}
				else if (Main.mouseRight && inv[slot].type == 3085)
				{
					if (Main.mouseRightRelease && player.consumeItem(327))
					{
						inv[slot].stack--;
						if (inv[slot].stack == 0)
						{
							inv[slot].SetDefaults(0, false);
						}
						Main.PlaySound(7, -1, -1, 1);
						Main.stackSplit = 30;
						Main.mouseRightRelease = false;
						player.openLockBox();
						Recipe.FindRecipes();
					}
				}
				else if (Main.mouseRight && inv[slot].type == 1869)
				{
					if (Main.mouseRightRelease)
					{
						inv[slot].stack--;
						if (inv[slot].stack == 0)
						{
							inv[slot].SetDefaults(0, false);
						}
						Main.PlaySound(7, -1, -1, 1);
						Main.stackSplit = 30;
						Main.mouseRightRelease = false;
						player.openPresent();
						Recipe.FindRecipes();
					}
				}
				else if (Main.mouseRight && Main.mouseRightRelease && (inv[slot].type == 599 || inv[slot].type == 600 || inv[slot].type == 601))
				{
					Main.PlaySound(7, -1, -1, 1);
					Main.stackSplit = 30;
					Main.mouseRightRelease = false;
					int num = Main.rand.Next(14);
					if (num == 0 && Main.hardMode)
					{
						inv[slot].SetDefaults(602, false);
					}
					else if (num <= 7)
					{
						inv[slot].SetDefaults(586, false);
						inv[slot].stack = Main.rand.Next(20, 50);
					}
					else
					{
						inv[slot].SetDefaults(591, false);
						inv[slot].stack = Main.rand.Next(20, 50);
					}
					Recipe.FindRecipes();
				}
				else
				{
					flag = false;
				}
			}
			else if (context == 9 || context == 11)
			{
				flag = true;
				if (Main.mouseRight && Main.mouseRightRelease && ((inv[slot].type > 0 && inv[slot].stack > 0) || (inv[slot - 10].type > 0 && inv[slot - 10].stack > 0)))
				{
					bool flag2 = true;
					if (flag2 && context == 11 && inv[slot].wingSlot > 0)
					{
						for (int i = 3; i < 10; i++)
						{
							if (inv[i].wingSlot > 0 && i != slot - 10)
							{
								flag2 = false;
							}
						}
					}
					if (flag2)
					{
						Utils.Swap<Item>(ref inv[slot], ref inv[slot - 10]);
						Main.PlaySound(7, -1, -1, 1);
						Recipe.FindRecipes();
						if (inv[slot].stack > 0)
						{
							if (context != 0)
							{
								switch (context)
								{
								case 8:
								case 9:
								case 10:
								case 11:
								case 12:
								case 16:
								case 17:
									AchievementsHelper.HandleOnEquip(player, inv[slot], context);
									break;
								}
							}
							else
							{
								AchievementsHelper.NotifyItemPickup(player, inv[slot]);
							}
						}
					}
				}
			}
			else if (context == 12)
			{
				flag = true;
				if (Main.mouseRight && Main.mouseRightRelease && Main.mouseItem.stack < Main.mouseItem.maxStack && Main.mouseItem.type > 0 && inv[slot].type > 0 && Main.mouseItem.type == inv[slot].type)
				{
					Main.mouseItem.stack++;
					inv[slot].SetDefaults(0, false);
					Main.PlaySound(7, -1, -1, 1);
				}
			}
			else if (context == 15)
			{
				flag = true;
				Chest arg_5BE_0 = Main.instance.shop[Main.npcShop];
				if (Main.stackSplit <= 1 && Main.mouseRight && inv[slot].type > 0 && (Main.mouseItem.IsTheSameAs(inv[slot]) || Main.mouseItem.type == 0))
				{
					int num2 = Main.superFastStack + 1;
					for (int j = 0; j < num2; j++)
					{
						if ((Main.mouseItem.stack < Main.mouseItem.maxStack || Main.mouseItem.type == 0) && player.BuyItem(inv[slot].value) && inv[slot].stack > 0)
						{
							if (j == 0)
							{
								Main.PlaySound(18, -1, -1, 1);
							}
							if (Main.mouseItem.type == 0)
							{
								Main.mouseItem.netDefaults(inv[slot].netID);
								if (inv[slot].prefix != 0)
								{
									Main.mouseItem.Prefix((int)inv[slot].prefix);
								}
								Main.mouseItem.stack = 0;
							}
							Main.mouseItem.stack++;
							if (Main.stackSplit == 0)
							{
								Main.stackSplit = 15;
							}
							else
							{
								Main.stackSplit = Main.stackDelay;
							}
							if (inv[slot].buyOnce && --inv[slot].stack <= 0)
							{
								inv[slot].SetDefaults(0, false);
							}
						}
					}
				}
			}
			if (flag)
			{
				return;
			}
			if ((context == 0 || context == 4 || context == 3) && Main.mouseRight && Main.mouseRightRelease && inv[slot].maxStack == 1)
			{
				if (inv[slot].dye > 0)
				{
					bool flag3;
					inv[slot] = ItemSlot.DyeSwap(inv[slot], out flag3);
					if (flag3)
					{
						Main.EquipPageSelected = 0;
						AchievementsHelper.HandleOnEquip(player, inv[slot], 12);
					}
				}
				else if (Main.projHook[inv[slot].shoot])
				{
					bool flag3;
					inv[slot] = ItemSlot.EquipSwap(inv[slot], player.miscEquips, 4, out flag3);
					if (flag3)
					{
						Main.EquipPageSelected = 2;
						AchievementsHelper.HandleOnEquip(player, inv[slot], 16);
					}
				}
				else if (inv[slot].mountType != -1 && !MountID.Sets.Cart[inv[slot].mountType])
				{
					bool flag3;
					inv[slot] = ItemSlot.EquipSwap(inv[slot], player.miscEquips, 3, out flag3);
					if (flag3)
					{
						Main.EquipPageSelected = 2;
						AchievementsHelper.HandleOnEquip(player, inv[slot], 17);
					}
				}
				else if (inv[slot].mountType != -1 && MountID.Sets.Cart[inv[slot].mountType])
				{
					bool flag3;
					inv[slot] = ItemSlot.EquipSwap(inv[slot], player.miscEquips, 2, out flag3);
					if (flag3)
					{
						Main.EquipPageSelected = 2;
					}
				}
				else if (inv[slot].buffType > 0 && Main.lightPet[inv[slot].buffType])
				{
					bool flag3;
					inv[slot] = ItemSlot.EquipSwap(inv[slot], player.miscEquips, 1, out flag3);
					if (flag3)
					{
						Main.EquipPageSelected = 2;
					}
				}
				else if (inv[slot].buffType > 0 && Main.vanityPet[inv[slot].buffType])
				{
					bool flag3;
					inv[slot] = ItemSlot.EquipSwap(inv[slot], player.miscEquips, 0, out flag3);
					if (flag3)
					{
						Main.EquipPageSelected = 2;
					}
				}
				else
				{
					bool flag3;
					inv[slot] = ItemSlot.ArmorSwap(inv[slot], out flag3);
					if (flag3)
					{
						Main.EquipPageSelected = 0;
						AchievementsHelper.HandleOnEquip(player, inv[slot], 8);
					}
				}
				Recipe.FindRecipes();
				if (context == 3 && Main.netMode == 1)
				{
					NetMessage.SendData(32, -1, -1, "", player.chest, (float)slot, 0f, 0f, 0, 0, 0);
					return;
				}
			}
			else if (Main.stackSplit <= 1 && Main.mouseRight)
			{
				bool flag4 = true;
				if (context == 0 && inv[slot].maxStack <= 1)
				{
					flag4 = false;
				}
				if (context == 3 && inv[slot].maxStack <= 1)
				{
					flag4 = false;
				}
				if (context == 4 && inv[slot].maxStack <= 1)
				{
					flag4 = false;
				}
				if (flag4 && (Main.mouseItem.IsTheSameAs(inv[slot]) || Main.mouseItem.type == 0) && (Main.mouseItem.stack < Main.mouseItem.maxStack || Main.mouseItem.type == 0))
				{
					if (Main.mouseItem.type == 0)
					{
						Main.mouseItem = inv[slot].Clone();
						Main.mouseItem.stack = 0;
						if (inv[slot].favorited && inv[slot].maxStack == 1)
						{
							Main.mouseItem.favorited = true;
						}
						else
						{
							Main.mouseItem.favorited = false;
						}
					}
					Main.mouseItem.stack++;
					inv[slot].stack--;
					if (inv[slot].stack <= 0)
					{
						inv[slot] = new Item();
					}
					Recipe.FindRecipes();
					Main.soundInstanceMenuTick.Stop();
					Main.soundInstanceMenuTick = Main.soundMenuTick.CreateInstance();
					Main.PlaySound(12, -1, -1, 1);
					if (Main.stackSplit == 0)
					{
						Main.stackSplit = 15;
					}
					else
					{
						Main.stackSplit = Main.stackDelay;
					}
					if (context == 3 && Main.netMode == 1)
					{
						NetMessage.SendData(32, -1, -1, "", player.chest, (float)slot, 0f, 0f, 0, 0, 0);
					}
				}
			}
		}
		public static void Draw(SpriteBatch spriteBatch, ref Item inv, int context, Vector2 position, Color lightColor = default(Color))
		{
			ItemSlot.singleSlotArray[0] = inv;
			ItemSlot.Draw(spriteBatch, ItemSlot.singleSlotArray, context, 0, position, lightColor);
			inv = ItemSlot.singleSlotArray[0];
		}
		public static void Draw(SpriteBatch spriteBatch, Item[] inv, int context, int slot, Vector2 position, Color lightColor = default(Color))
		{
			Player player = Main.player[Main.myPlayer];
			Item item = inv[slot];
			float inventoryScale = Main.inventoryScale;
			Color color = Color.White;
			if (lightColor != Color.Transparent)
			{
				color = lightColor;
			}
			Texture2D texture2D = Main.inventoryBackTexture;
			Color color2 = Main.inventoryBack;
			bool flag = false;
			if (item.type > 0 && item.stack > 0 && item.favorited && context != 13 && context != 21 && context != 22)
			{
				texture2D = Main.inventoryBack10Texture;
			}
			else if (item.type > 0 && item.stack > 0 && ItemSlot.Options.HighlightNewItems && item.newAndShiny && context != 13 && context != 21 && context != 22)
			{
				texture2D = Main.inventoryBack15Texture;
				float num = (float)Main.mouseTextColor / 255f;
				num = num * 0.2f + 0.8f;
				color2 = color2.MultiplyRGBA(new Color(num, num, num));
			}
			else if (context == 0 && slot < 10)
			{
				texture2D = Main.inventoryBack9Texture;
			}
			else if (context == 10 || context == 8 || context == 16 || context == 17 || context == 19 || context == 18 || context == 20)
			{
				texture2D = Main.inventoryBack3Texture;
			}
			else if (context == 11 || context == 9)
			{
				texture2D = Main.inventoryBack8Texture;
			}
			else if (context == 12)
			{
				texture2D = Main.inventoryBack12Texture;
			}
			else if (context == 3)
			{
				texture2D = Main.inventoryBack5Texture;
			}
			else if (context == 4)
			{
				texture2D = Main.inventoryBack2Texture;
			}
			else if (context == 7 || context == 5)
			{
				texture2D = Main.inventoryBack4Texture;
			}
			else if (context == 6)
			{
				texture2D = Main.inventoryBack7Texture;
			}
			else if (context == 13)
			{
				byte b = 200;
				if (slot == Main.player[Main.myPlayer].selectedItem)
				{
					texture2D = Main.inventoryBack14Texture;
					b = 255;
				}
				color2 = new Color((int)b, (int)b, (int)b, (int)b);
			}
			else if (context == 14 || context == 21)
			{
				flag = true;
			}
			else if (context == 15)
			{
				texture2D = Main.inventoryBack6Texture;
			}
			else if (context == 22)
			{
				texture2D = Main.inventoryBack4Texture;
			}
			if (!flag)
			{
				spriteBatch.Draw(texture2D, position, null, color2, 0f, default(Vector2), inventoryScale, SpriteEffects.None, 0f);
			}
			int num2 = -1;
			switch (context)
			{
			case 8:
				if (slot == 0)
				{
					num2 = 0;
				}
				if (slot == 1)
				{
					num2 = 6;
				}
				if (slot == 2)
				{
					num2 = 12;
				}
				break;
			case 9:
				if (slot == 10)
				{
					num2 = 3;
				}
				if (slot == 11)
				{
					num2 = 9;
				}
				if (slot == 12)
				{
					num2 = 15;
				}
				break;
			case 10:
				num2 = 11;
				break;
			case 11:
				num2 = 2;
				break;
			case 12:
				num2 = 1;
				break;
			case 16:
				num2 = 4;
				break;
			case 17:
				num2 = 13;
				break;
			case 18:
				num2 = 7;
				break;
			case 19:
				num2 = 10;
				break;
			case 20:
				num2 = 17;
				break;
			}
			if ((item.type <= 0 || item.stack <= 0) && num2 != -1)
			{
				Texture2D texture2D2 = Main.extraTexture[54];
				Rectangle rectangle = texture2D2.Frame(3, 6, num2 % 3, num2 / 3);
				rectangle.Width -= 2;
				rectangle.Height -= 2;
				spriteBatch.Draw(texture2D2, position + texture2D.Size() / 2f * inventoryScale, new Rectangle?(rectangle), Color.White * 0.35f, 0f, rectangle.Size() / 2f, inventoryScale, SpriteEffects.None, 0f);
			}
			if (item.type > 0 && item.stack > 0)
			{
				Texture2D texture2D3 = Main.itemTexture[item.type];
				Rectangle rectangle2;
				if (Main.itemAnimations[item.type] != null)
				{
					rectangle2 = Main.itemAnimations[item.type].GetFrame(texture2D3);
				}
				else
				{
					rectangle2 = texture2D3.Frame(1, 1, 0, 0);
				}
				Color newColor = color;
				float num3 = 1f;
				ItemSlot.GetItemLight(ref newColor, ref num3, item, false);
				float num4 = 1f;
				if (rectangle2.Width > 32 || rectangle2.Height > 32)
				{
					if (rectangle2.Width > rectangle2.Height)
					{
						num4 = 32f / (float)rectangle2.Width;
					}
					else
					{
						num4 = 32f / (float)rectangle2.Height;
					}
				}
				num4 *= inventoryScale;
				Vector2 position2 = position + texture2D.Size() * inventoryScale / 2f - rectangle2.Size() * num4 / 2f;
				Vector2 origin = rectangle2.Size() * (num3 / 2f - 0.5f);
				spriteBatch.Draw(texture2D3, position2, new Rectangle?(rectangle2), item.GetAlpha(newColor), 0f, origin, num4 * num3, SpriteEffects.None, 0f);
				if (item.color != Color.Transparent)
				{
					spriteBatch.Draw(texture2D3, position2, new Rectangle?(rectangle2), item.GetColor(color), 0f, origin, num4 * num3, SpriteEffects.None, 0f);
				}
				if (item.stack > 1)
				{
					ChatManager.DrawColorCodedStringWithShadow(spriteBatch, Main.fontItemStack, item.stack.ToString(), position + new Vector2(10f, 26f) * inventoryScale, color, 0f, Vector2.Zero, new Vector2(inventoryScale), -1f, inventoryScale);
				}
				int num5 = -1;
				if (context == 13)
				{
					if (item.useAmmo > 0)
					{
						int useAmmo = item.useAmmo;
						num5 = 0;
						for (int i = 0; i < 58; i++)
						{
							if (inv[i].ammo == useAmmo)
							{
								num5 += inv[i].stack;
							}
						}
					}
					if (item.fishingPole > 0)
					{
						num5 = 0;
						for (int j = 0; j < 58; j++)
						{
							if (inv[j].bait > 0)
							{
								num5 += inv[j].stack;
							}
						}
					}
					if (item.tileWand > 0)
					{
						int tileWand = item.tileWand;
						num5 = 0;
						for (int k = 0; k < 58; k++)
						{
							if (inv[k].type == tileWand)
							{
								num5 += inv[k].stack;
							}
						}
					}
					if (item.type == 509 || item.type == 851 || item.type == 850)
					{
						num5 = 0;
						for (int l = 0; l < 58; l++)
						{
							if (inv[l].type == 530)
							{
								num5 += inv[l].stack;
							}
						}
					}
				}
				if (num5 != -1)
				{
					ChatManager.DrawColorCodedStringWithShadow(spriteBatch, Main.fontItemStack, num5.ToString(), position + new Vector2(8f, 30f) * inventoryScale, color, 0f, Vector2.Zero, new Vector2(inventoryScale * 0.8f), -1f, inventoryScale);
				}
				if (context == 13)
				{
					string text = string.Concat(slot + 1);
					if (text == "10")
					{
						text = "0";
					}
					ChatManager.DrawColorCodedStringWithShadow(spriteBatch, Main.fontItemStack, text, position + new Vector2(8f, 4f) * inventoryScale, color, 0f, Vector2.Zero, new Vector2(inventoryScale), -1f, inventoryScale);
				}
				if (context == 13 && item.potion)
				{
					Vector2 position3 = position + texture2D.Size() * inventoryScale / 2f - Main.cdTexture.Size() * inventoryScale / 2f;
					Color color3 = item.GetAlpha(color) * ((float)player.potionDelay / (float)player.potionDelayTime);
					spriteBatch.Draw(Main.cdTexture, position3, null, color3, 0f, default(Vector2), num4, SpriteEffects.None, 0f);
				}
				if ((context == 10 || context == 18) && item.expertOnly && !Main.expertMode)
				{
					Vector2 position4 = position + texture2D.Size() * inventoryScale / 2f - Main.cdTexture.Size() * inventoryScale / 2f;
					Color white = Color.White;
					spriteBatch.Draw(Main.cdTexture, position4, null, white, 0f, default(Vector2), num4, SpriteEffects.None, 0f);
				}
			}
			else if (context == 6)
			{
				Texture2D trashTexture = Main.trashTexture;
				Vector2 position5 = position + texture2D.Size() * inventoryScale / 2f - trashTexture.Size() * inventoryScale / 2f;
				spriteBatch.Draw(trashTexture, position5, null, new Color(100, 100, 100, 100), 0f, default(Vector2), inventoryScale, SpriteEffects.None, 0f);
			}
			if (context == 0 && slot < 10)
			{
				float num6 = inventoryScale;
				string text2 = string.Concat(slot + 1);
				if (text2 == "10")
				{
					text2 = "0";
				}
				Color inventoryBack = Main.inventoryBack;
				int num7 = 0;
				if (Main.player[Main.myPlayer].selectedItem == slot)
				{
					num7 -= 3;
					inventoryBack.R = 255;
					inventoryBack.B = 0;
					inventoryBack.G = 210;
					inventoryBack.A = 100;
					num6 *= 1.4f;
				}
				ChatManager.DrawColorCodedStringWithShadow(spriteBatch, Main.fontItemStack, text2, position + new Vector2(6f, (float)(4 + num7)) * inventoryScale, inventoryBack, 0f, Vector2.Zero, new Vector2(inventoryScale), -1f, inventoryScale);
			}
		}
		public static void MouseHover(ref Item inv, int context = 0)
		{
			ItemSlot.singleSlotArray[0] = inv;
			ItemSlot.MouseHover(ItemSlot.singleSlotArray, context, 0);
			inv = ItemSlot.singleSlotArray[0];
		}
		public static void MouseHover(Item[] inv, int context = 0, int slot = 0)
		{
			if (context == 6 && Main.hoverItemName == null)
			{
				Main.hoverItemName = Lang.inter[3];
			}
			if (inv[slot].type > 0 && inv[slot].stack > 0)
			{
				Main.hoverItemName = inv[slot].name;
				if (inv[slot].stack > 1)
				{
					object hoverItemName = Main.hoverItemName;
					Main.hoverItemName = string.Concat(new object[]
					{
						hoverItemName,
						" (",
						inv[slot].stack,
						")"
					});
				}
				Main.toolTip = inv[slot].Clone();
				if (context == 8 && slot <= 2)
				{
					Main.toolTip.wornArmor = true;
				}
				if (context == 11 || context == 9)
				{
					Main.toolTip.social = true;
				}
				if (context == 15)
				{
					Main.toolTip.buy = true;
					return;
				}
			}
			else
			{
				if (context == 10 || context == 11)
				{
					Main.hoverItemName = Lang.inter[9];
				}
				if (context == 11)
				{
					Main.hoverItemName = Lang.inter[11] + " " + Main.hoverItemName;
				}
				if (context == 8 || context == 9)
				{
					if (slot == 0 || slot == 10)
					{
						Main.hoverItemName = Lang.inter[12];
					}
					if (slot == 1 || slot == 11)
					{
						Main.hoverItemName = Lang.inter[13];
					}
					if (slot == 2 || slot == 12)
					{
						Main.hoverItemName = Lang.inter[14];
					}
					if (slot >= 10)
					{
						Main.hoverItemName = Lang.inter[11] + " " + Main.hoverItemName;
					}
				}
				if (context == 12)
				{
					Main.hoverItemName = Lang.inter[57];
				}
				if (context == 16)
				{
					Main.hoverItemName = Lang.inter[90];
				}
				if (context == 17)
				{
					Main.hoverItemName = Lang.inter[91];
				}
				if (context == 19)
				{
					Main.hoverItemName = Lang.inter[92];
				}
				if (context == 18)
				{
					Main.hoverItemName = Lang.inter[93];
				}
				if (context == 20)
				{
					Main.hoverItemName = Lang.inter[94];
				}
			}
		}
		private static bool AccCheck(Item item, int slot)
		{
			Player player = Main.player[Main.myPlayer];
			if (slot != -1)
			{
				if (player.armor[slot].IsTheSameAs(item))
				{
					return false;
				}
				if (player.armor[slot].wingSlot > 0 && item.wingSlot > 0)
				{
					return false;
				}
			}
			for (int i = 0; i < player.armor.Length; i++)
			{
				if (slot < 10 && i < 10)
				{
					if (item.wingSlot > 0 && player.armor[i].wingSlot > 0)
					{
						return true;
					}
					if (slot >= 10 && i >= 10 && item.wingSlot > 0 && player.armor[i].wingSlot > 0)
					{
						return true;
					}
				}
				if (item.IsTheSameAs(player.armor[i]))
				{
					return true;
				}
			}
			return false;
		}
		private static Item DyeSwap(Item item, out bool success)
		{
			success = false;
			if (item.dye <= 0)
			{
				return item;
			}
			Player player = Main.player[Main.myPlayer];
			for (int i = 0; i < 10; i++)
			{
				if (player.dye[i].type == 0)
				{
					ItemSlot.dyeSlotCount = i;
					break;
				}
			}
			if (ItemSlot.dyeSlotCount >= 10)
			{
				ItemSlot.dyeSlotCount = 0;
			}
			if (ItemSlot.dyeSlotCount < 0)
			{
				ItemSlot.dyeSlotCount = 9;
			}
			Item result = player.dye[ItemSlot.dyeSlotCount].Clone();
			player.dye[ItemSlot.dyeSlotCount] = item.Clone();
			ItemSlot.dyeSlotCount++;
			if (ItemSlot.dyeSlotCount >= 10)
			{
				ItemSlot.accSlotCount = 0;
			}
			Main.PlaySound(7, -1, -1, 1);
			Recipe.FindRecipes();
			success = true;
			return result;
		}
		private static Item ArmorSwap(Item item, out bool success)
		{
			success = false;
			if (item.headSlot == -1 && item.bodySlot == -1 && item.legSlot == -1 && !item.accessory)
			{
				return item;
			}
			Player player = Main.player[Main.myPlayer];
			int num = (item.vanity && !item.accessory) ? 10 : 0;
			item.favorited = false;
			Item result = item;
			if (item.headSlot != -1)
			{
				result = player.armor[num].Clone();
				player.armor[num] = item.Clone();
			}
			else if (item.bodySlot != -1)
			{
				result = player.armor[num + 1].Clone();
				player.armor[num + 1] = item.Clone();
			}
			else if (item.legSlot != -1)
			{
				result = player.armor[num + 2].Clone();
				player.armor[num + 2] = item.Clone();
			}
			else if (item.accessory)
			{
				int num2 = 5 + Main.player[Main.myPlayer].extraAccessorySlots;
				for (int i = 3; i < 3 + num2; i++)
				{
					if (player.armor[i].type == 0)
					{
						ItemSlot.accSlotCount = i - 3;
						break;
					}
				}
				for (int j = 0; j < player.armor.Length; j++)
				{
					if (item.IsTheSameAs(player.armor[j]))
					{
						ItemSlot.accSlotCount = j - 3;
					}
					if (j < 10 && item.wingSlot > 0 && player.armor[j].wingSlot > 0)
					{
						ItemSlot.accSlotCount = j - 3;
					}
				}
				if (ItemSlot.accSlotCount >= num2)
				{
					ItemSlot.accSlotCount = 0;
				}
				if (ItemSlot.accSlotCount < 0)
				{
					ItemSlot.accSlotCount = num2 - 1;
				}
				int num3 = 3 + ItemSlot.accSlotCount;
				for (int k = 0; k < player.armor.Length; k++)
				{
					if (item.IsTheSameAs(player.armor[k]))
					{
						num3 = k;
					}
				}
				result = player.armor[num3].Clone();
				player.armor[num3] = item.Clone();
				ItemSlot.accSlotCount++;
				if (ItemSlot.accSlotCount >= num2)
				{
					ItemSlot.accSlotCount = 0;
				}
			}
			Main.PlaySound(7, -1, -1, 1);
			Recipe.FindRecipes();
			success = true;
			return result;
		}
		private static Item EquipSwap(Item item, Item[] inv, int slot, out bool success)
		{
			success = false;
			Player arg_0E_0 = Main.player[Main.myPlayer];
			item.favorited = false;
			Item result = inv[slot].Clone();
			inv[slot] = item.Clone();
			Main.PlaySound(7, -1, -1, 1);
			Recipe.FindRecipes();
			success = true;
			return result;
		}
		public static void EquipPage(Item item)
		{
			Main.EquipPage = -1;
			if (Main.projHook[item.shoot])
			{
				Main.EquipPage = 2;
				return;
			}
			if (item.mountType != -1)
			{
				Main.EquipPage = 2;
				return;
			}
			if (item.buffType > 0 && Main.vanityPet[item.buffType])
			{
				Main.EquipPage = 2;
				return;
			}
			if (item.buffType > 0 && Main.lightPet[item.buffType])
			{
				Main.EquipPage = 2;
				return;
			}
			if (item.dye > 0 && Main.EquipPageSelected == 1)
			{
				Main.EquipPage = 0;
				return;
			}
			if (item.legSlot != -1 || item.headSlot != -1 || item.bodySlot != -1 || item.accessory)
			{
				Main.EquipPage = 0;
			}
		}
		public static void DrawMoney(SpriteBatch sb, string text, float shopx, float shopy, int[] coinsArray, bool horizontal = false)
		{
			Utils.DrawBorderStringFourWay(sb, Main.fontMouseText, text, shopx, shopy + 40f, Color.White * ((float)Main.mouseTextColor / 255f), Color.Black, Vector2.Zero, 1f);
			if (horizontal)
			{
				for (int i = 0; i < 4; i++)
				{
					if (i == 0)
					{
						int arg_50_0 = coinsArray[3 - i];
					}
					Vector2 position = new Vector2(shopx + ChatManager.GetStringSize(Main.fontMouseText, text, Vector2.One, -1f).X + (float)(24 * i) + 45f, shopy + 50f);
					sb.Draw(Main.itemTexture[74 - i], position, null, Color.White, 0f, Main.itemTexture[74 - i].Size() / 2f, 1f, SpriteEffects.None, 0f);
					Utils.DrawBorderStringFourWay(sb, Main.fontItemStack, coinsArray[3 - i].ToString(), position.X - 11f, position.Y, Color.White, Color.Black, new Vector2(0.3f), 0.75f);
				}
				return;
			}
			for (int j = 0; j < 4; j++)
			{
				int num = (j == 0 && coinsArray[3 - j] > 99) ? -6 : 0;
				sb.Draw(Main.itemTexture[74 - j], new Vector2(shopx + 11f + (float)(24 * j), shopy + 75f), null, Color.White, 0f, Main.itemTexture[74 - j].Size() / 2f, 1f, SpriteEffects.None, 0f);
				Utils.DrawBorderStringFourWay(sb, Main.fontItemStack, coinsArray[3 - j].ToString(), shopx + (float)(24 * j) + (float)num, shopy + 75f, Color.White, Color.Black, new Vector2(0.3f), 0.75f);
			}
		}
		public static void DrawSavings(SpriteBatch sb, float shopx, float shopy, bool horizontal = false)
		{
			Player player = Main.player[Main.myPlayer];
			bool flag;
			long num = Utils.CoinsCount(out flag, player.bank.item, new int[0]);
			long num2 = Utils.CoinsCount(out flag, player.bank2.item, new int[0]);
			long num3 = Utils.CoinsCombineStacks(out flag, new long[]
			{
				num,
				num2
			});
			if (num3 > 0L)
			{
				if (num2 > 0L)
				{
					sb.Draw(Main.itemTexture[346], Utils.CenteredRectangle(new Vector2(shopx + 80f, shopy + 50f), Main.itemTexture[346].Size() * 0.65f), null, Color.White);
				}
				if (num > 0L)
				{
					sb.Draw(Main.itemTexture[87], Utils.CenteredRectangle(new Vector2(shopx + 70f, shopy + 60f), Main.itemTexture[87].Size() * 0.65f), null, Color.White);
				}
				ItemSlot.DrawMoney(sb, Lang.inter[66], shopx, shopy, Utils.CoinsSplit(num3), horizontal);
			}
		}
		public static void GetItemLight(ref Color currentColor, Item item, bool outInTheWorld = false)
		{
			float num = 1f;
			ItemSlot.GetItemLight(ref currentColor, ref num, item, outInTheWorld);
		}
		public static void GetItemLight(ref Color currentColor, int type, bool outInTheWorld = false)
		{
			float num = 1f;
			ItemSlot.GetItemLight(ref currentColor, ref num, type, outInTheWorld);
		}
		public static void GetItemLight(ref Color currentColor, ref float scale, Item item, bool outInTheWorld = false)
		{
			ItemSlot.GetItemLight(ref currentColor, ref scale, item.type, outInTheWorld);
		}
		public static Color GetItemLight(ref Color currentColor, ref float scale, int type, bool outInTheWorld = false)
		{
			if (type < 0 || type > 3602)
			{
				return currentColor;
			}
			if (type == 662 || type == 663)
			{
				currentColor.R = (byte)Main.DiscoR;
				currentColor.G = (byte)Main.DiscoG;
				currentColor.B = (byte)Main.DiscoB;
				currentColor.A = 255;
			}
			else if (ItemID.Sets.ItemIconPulse[type])
			{
				scale = Main.essScale;
				currentColor.R = (byte)((float)currentColor.R * scale);
				currentColor.G = (byte)((float)currentColor.G * scale);
				currentColor.B = (byte)((float)currentColor.B * scale);
				currentColor.A = (byte)((float)currentColor.A * scale);
			}
			else if (type == 58 || type == 184)
			{
				scale = Main.essScale * 0.25f + 0.75f;
				currentColor.R = (byte)((float)currentColor.R * scale);
				currentColor.G = (byte)((float)currentColor.G * scale);
				currentColor.B = (byte)((float)currentColor.B * scale);
				currentColor.A = (byte)((float)currentColor.A * scale);
			}
			return currentColor;
		}
	}
}
