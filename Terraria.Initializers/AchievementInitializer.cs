using System;
using Terraria.Achievements;
using Terraria.GameContent.Achievements;
using Terraria.GameContent.UI.Chat;
namespace Terraria.Initializers
{
	internal class AchievementInitializer
	{
		public static void Load()
		{
			if (Main.netMode == 2)
			{
				return;
			}
			Achievement achievement = new Achievement("TIMBER", "Timber!!", "Chop down your first tree.");
			achievement.AddCondition(ItemPickupCondition.Create(new short[]
			{
				9,
				619,
				2504,
				620,
				2503,
				2260,
				621,
				911,
				1729
			}));
			Main.Achievements.Register(achievement);
			achievement = new Achievement("NO_HOBO", "No Hobo", "Build a house suitable enough for your first town NPC, such as the guide, to move into.");
			achievement.AddCondition(ProgressionEventCondition.Create(8));
			Main.Achievements.Register(achievement);
			achievement = new Achievement("OBTAIN_HAMMER", "Stop! Hammer Time!", "Obtain your first hammer via crafting or otherwise.");
			achievement.AddCondition(ItemPickupCondition.Create(new short[]
			{
				2775,
				2746,
				3505,
				654,
				3517,
				7,
				3493,
				2780,
				1513,
				2516,
				660,
				3481,
				657,
				922,
				3511,
				2785,
				3499,
				3487,
				196,
				367,
				104,
				797,
				2320,
				787,
				1234,
				1262,
				3465,
				204,
				217,
				1507,
				3524,
				3522,
				3525,
				3523,
				1305
			}));
			Main.Achievements.Register(achievement);
			achievement = new Achievement("OOO_SHINY", "Ooo! Shiny!", "Mine your first nugget of ore with a pickaxe.");
			achievement.AddCondition(TileDestroyedCondition.Create(new ushort[]
			{
				7,
				6,
				9,
				8,
				166,
				167,
				168,
				169,
				22,
				204,
				58,
				107,
				108,
				111,
				221,
				222,
				223,
				211
			}));
			Main.Achievements.Register(achievement);
			achievement = new Achievement("HEART_BREAKER", "Heart Breaker", "Discover and smash your first heart crystal underground.");
			achievement.AddCondition(TileDestroyedCondition.Create(new ushort[]
			{
				12
			}));
			Main.Achievements.Register(achievement);
			achievement = new Achievement("HEAVY_METAL", "Heavy Metal", "Obtain an anvil made from iron or lead.");
			achievement.AddCondition(ItemPickupCondition.Create(new short[]
			{
				35,
				716
			}));
			Main.Achievements.Register(achievement);
			achievement = new Achievement("I_AM_LOOT", "I Am Loot!", "Discover a golden chest underground and take a peek at its contents.");
			achievement.AddCondition(CustomFlagCondition.Create("Peek"));
			Main.Achievements.Register(achievement);
			achievement = new Achievement("STAR_POWER", "Star Power", "Craft a mana crystal out of fallen stars, and consume it.");
			achievement.AddCondition(CustomFlagCondition.Create("Use"));
			Main.Achievements.Register(achievement);
			achievement = new Achievement("HOLD_ON_TIGHT", "Hold on Tight!", "Equip your first grappling hook.");
			achievement.AddCondition(CustomFlagCondition.Create("Equip"));
			Main.Achievements.Register(achievement);
			achievement = new Achievement("EYE_ON_YOU", "Eye on You", "Defeat the Eye of Cthulhu, an ocular menace who only appears at night.");
			achievement.AddCondition(NPCKilledCondition.Create(4));
			Main.Achievements.Register(achievement);
			achievement = new Achievement("SMASHING_POPPET", "Smashing, Poppet!", "Using explosives or your trusty hammer, smash a Shadow Orb or Crimson Heart in the evil parts of your world.");
			achievement.AddCondition(ProgressionEventCondition.Create(7));
			Main.Achievements.Register(achievement);
			achievement = new Achievement("WORM_FODDER", "Worm Fodder", "Defeat the Eater of Worlds, a massive worm whom dwells in the corruption.");
			achievement.AddCondition(NPCKilledCondition.Create(new short[]
			{
				13,
				14,
				15
			}));
			Main.Achievements.Register(achievement);
			achievement = new Achievement("MASTERMIND", "Mastermind", "Defeat the Brain of Cthuhlu, an enourmous demon brain which haunts the creeping crimson.");
			achievement.AddCondition(NPCKilledCondition.Create(266));
			Main.Achievements.Register(achievement);
			achievement = new Achievement("WHERES_MY_HONEY", "Where's My Honey?", "Discover a large bee's hive deep in the jungle.");
			achievement.AddCondition(CustomFlagCondition.Create("Reach"));
			Main.Achievements.Register(achievement);
			achievement = new Achievement("STING_OPERATION", "Sting Operation", "Defeat the Queen Bee, the matriarch of the jungle hives.");
			achievement.AddCondition(NPCKilledCondition.Create(222));
			Main.Achievements.Register(achievement);
			achievement = new Achievement("BONED", "Boned", "Defeat Skeletron, the cursed guardian of the dungeon.");
			achievement.AddCondition(NPCKilledCondition.Create(35));
			Main.Achievements.Register(achievement);
			achievement = new Achievement("DUNGEON_HEIST", "Dungeon Heist", "Steal a key from dungeon's undead denizens, and unlock one of their precious golden chests.");
			achievement.AddCondition(ItemPickupCondition.Create(327));
			achievement.AddCondition(ProgressionEventCondition.Create(19));
			Main.Achievements.Register(achievement);
			achievement = new Achievement("ITS_GETTING_HOT_IN_HERE", "It's Getting Hot in Here", "Spelunk deep enough to reach the molten underworld.");
			achievement.AddCondition(CustomFlagCondition.Create("Reach"));
			Main.Achievements.Register(achievement);
			achievement = new Achievement("MINER_FOR_FIRE", "Miner for Fire", "Craft a molten pickaxe using the hottest of materials.");
			achievement.AddCondition(ItemCraftCondition.Create(122));
			Main.Achievements.Register(achievement);
			achievement = new Achievement("STILL_HUNGRY", "Still Hungry", "Defeat the Wall of Flesh, the master and core of the world who arises after a great, burning sacrifice.");
			achievement.AddCondition(NPCKilledCondition.Create(new short[]
			{
				113,
				114
			}));
			Main.Achievements.Register(achievement);
			achievement = new Achievement("ITS_HARD", "It's Hard!", "Unleash the ancient spirits of light and darkness across your world, enabling much stronger foes and showering the world with dazzling treasures (and rainbows!).");
			achievement.AddCondition(ProgressionEventCondition.Create(9));
			Main.Achievements.Register(achievement);
			achievement = new Achievement("BEGONE_EVIL", "Begone, Evil!", "Smash a demon or crimson altar with a powerful, holy hammer.");
			achievement.AddCondition(ProgressionEventCondition.Create(6));
			Main.Achievements.Register(achievement);
			achievement = new Achievement("EXTRA_SHINY", "Extra Shiny!", "Mine a powerful ore that has been newly blessed upon your world.");
			achievement.AddCondition(TileDestroyedCondition.Create(new ushort[]
			{
				107,
				108,
				111,
				221,
				222,
				223
			}));
			Main.Achievements.Register(achievement);
			achievement = new Achievement("HEAD_IN_THE_CLOUDS", "Head in the Clouds", "Equip a pair of wings.");
			achievement.AddCondition(CustomFlagCondition.Create("Equip"));
			Main.Achievements.Register(achievement);
			achievement = new Achievement("LIKE_A_BOSS", "Like a Boss", "Obtain a boss-summoning item.");
			achievement.AddCondition(ItemPickupCondition.Create(new short[]
			{
				1133,
				1331,
				1307,
				267,
				1293,
				557,
				544,
				556,
				560,
				43,
				70
			}));
			Main.Achievements.Register(achievement);
			achievement = new Achievement("BUCKETS_OF_BOLTS", "Buckets of Bolts", "Defeat the three nocturnal mechanical menaces: the Twins, the Destroyer, and Skeletron Prime.");
			achievement.AddCondition(NPCKilledCondition.Create(new short[]
			{
				125,
				126
			}));
			achievement.AddConditions(NPCKilledCondition.CreateMany(new short[]
			{
				127,
				134
			}));
			achievement.UseConditionsCompletedTracker();
			Main.Achievements.Register(achievement);
			achievement = new Achievement("DRAX_ATTAX", "Drax Attax", "Craft a drax or pickaxe axe using hallowed bars, and the souls of the three mechanical bosses.");
			achievement.AddCondition(ItemCraftCondition.Create(new short[]
			{
				579,
				990
			}));
			Main.Achievements.Register(achievement);
			achievement = new Achievement("PHOTOSYNTHESIS", "Photosynthesis", "Mine chlorophyte, an organic ore found deep among the thickest of flora.");
			achievement.AddCondition(TileDestroyedCondition.Create(new ushort[]
			{
				211
			}));
			Main.Achievements.Register(achievement);
			achievement = new Achievement("GET_A_LIFE", "Get a Life", "Consume a life fruit, which grows in the thick of subterranean jungle grass.");
			achievement.AddCondition(CustomFlagCondition.Create("Use"));
			Main.Achievements.Register(achievement);
			achievement = new Achievement("THE_GREAT_SOUTHERN_PLANTKILL", "The Great Southern Plantkill", "Defeat Plantera, the overgrown monstrosity of the jungle's depths.");
			achievement.AddCondition(NPCKilledCondition.Create(262));
			Main.Achievements.Register(achievement);
			achievement = new Achievement("TEMPLE_RAIDER", "Temple Raider", "Breach the impenetrable walls of the jungle temple.");
			achievement.AddCondition(TileDestroyedCondition.Create(new ushort[]
			{
				226
			}));
			Main.Achievements.Register(achievement);
			achievement = new Achievement("LIHZAHRDIAN_IDOL", "Lihzahrdian Idol", "Defeat Golem, the stone-faced ritualistic idol of the lihzahrd tribe.");
			achievement.AddCondition(NPCKilledCondition.Create(245));
			Main.Achievements.Register(achievement);
			achievement = new Achievement("ROBBING_THE_GRAVE", "Robbing the Grave", "Obtain a rare treasure from a difficult monster in the dungeon.");
			achievement.AddCondition(ItemPickupCondition.Create(new short[]
			{
				1513,
				938,
				963,
				977,
				1300,
				1254,
				1514,
				679,
				759,
				1446,
				1445,
				1444,
				1183,
				1266,
				671
			}));
			Main.Achievements.Register(achievement);
			achievement = new Achievement("BIG_BOOTY", "Big Booty", "Unlock one of the dungeon's large, mysterious chests with a special key.");
			achievement.AddCondition(ProgressionEventCondition.Create(20));
			Main.Achievements.Register(achievement);
			achievement = new Achievement("FISH_OUT_OF_WATER", "Fish Out of Water", "Defeat Duke Fishron, mutant terror of the sea.");
			achievement.AddCondition(NPCKilledCondition.Create(370));
			Main.Achievements.Register(achievement);
			achievement = new Achievement("OBSESSIVE_DEVOTION", "Obsessive Devotion", "Defeat the Ancient Cultist, fanatical leader of the dungeon coven.");
			achievement.AddCondition(NPCKilledCondition.Create(439));
			Main.Achievements.Register(achievement);
			achievement = new Achievement("STAR_DESTROYER", "Star Destroyer", "Defeat the four celestial towers of the moon.");
			achievement.AddConditions(NPCKilledCondition.CreateMany(new short[]
			{
				517,
				422,
				507,
				493
			}));
			Main.Achievements.Register(achievement);
			achievement = new Achievement("CHAMPION_OF_TERRARIA", "Champion of Terraria", "Defeat the Moon Lord.");
			achievement.AddCondition(NPCKilledCondition.Create(398));
			Main.Achievements.Register(achievement);
			achievement = new Achievement("BLOODBATH", "Bloodbath", "Survive a blood moon, a nocturnal event where the rivers run red and monsters swarm aplenty.");
			achievement.AddCondition(ProgressionEventCondition.Create(5));
			Main.Achievements.Register(achievement);
			achievement = new Achievement("SLIPPERY_SHINOBI", "Slippery Shinobi", "Defeat King Slime, the lord of all things slimy.");
			achievement.AddCondition(NPCKilledCondition.Create(50));
			Main.Achievements.Register(achievement);
			achievement = new Achievement("GOBLIN_PUNTER", "Goblin Punter", "Triumph over a goblin invasion, a ragtag regiment of crude, barbaric, pointy-eared warriors and their shadowflame sorcerers.");
			achievement.AddCondition(ProgressionEventCondition.Create(10));
			Main.Achievements.Register(achievement);
			achievement = new Achievement("WALK_THE_PLANK", "Walk the Plank", "Triumph over a pirate invasion, a group of pillagers from the sea out for your booty... and your life!");
			achievement.AddCondition(ProgressionEventCondition.Create(11));
			Main.Achievements.Register(achievement);
			achievement = new Achievement("KILL_THE_SUN", "Kill the Sun", "Survive a solar eclipse, a day darker than night filled with creatures of horror.");
			achievement.AddCondition(ProgressionEventCondition.Create(3));
			Main.Achievements.Register(achievement);
			achievement = new Achievement("DO_YOU_WANT_TO_SLAY_A_SNOWMAN", "Do You Want to Slay a Snowman?", "Triumph over the frost legion, a festive family of maniacal snowman mobsters.");
			achievement.AddCondition(ProgressionEventCondition.Create(12));
			Main.Achievements.Register(achievement);
			achievement = new Achievement("TIN_FOIL_HATTER", "Tin-Foil Hatter", "Triumph over a martian invasion, when beings from out of this world come to scramble your brains and probe you in uncomfortable places.");
			achievement.AddCondition(ProgressionEventCondition.Create(13));
			Main.Achievements.Register(achievement);
			achievement = new Achievement("BALEFUL_HARVEST", "Baleful Harvest", "Reach the 15th wave of a pumpkin moon, where evil lurks among the autumn harvest.");
			achievement.AddCondition(ProgressionEventCondition.Create(15));
			Main.Achievements.Register(achievement);
			achievement = new Achievement("ICE_SCREAM", "Ice Scream", "Reach the 15th wave of a frost moon, where the festive season quickly degrades into madness.");
			achievement.AddCondition(ProgressionEventCondition.Create(14));
			Main.Achievements.Register(achievement);
			achievement = new Achievement("STICKY_SITUATION", "Sticky Situation", "Survive the slime rain, where gelatinous organisms fall from the sky in droves.");
			achievement.AddCondition(ProgressionEventCondition.Create(16));
			Main.Achievements.Register(achievement);
			achievement = new Achievement("REAL_ESTATE_AGENT", "Real Estate Agent", "Have all possible town NPCs living in your world.");
			achievement.AddCondition(ProgressionEventCondition.Create(17));
			Main.Achievements.Register(achievement);
			achievement = new Achievement("NOT_THE_BEES", "Not the Bees!", "Fire a Bee Gun while wearing a full set of Bee Armor.");
			achievement.AddCondition(CustomFlagCondition.Create("Use"));
			Main.Achievements.Register(achievement);
			achievement = new Achievement("JEEPERS_CREEPERS", "Jeepers Creepers", "Stumble into a spider cavern in the underground.");
			achievement.AddCondition(CustomFlagCondition.Create("Reach"));
			Main.Achievements.Register(achievement);
			achievement = new Achievement("FUNKYTOWN", "Funkytown", "Build or encounter a glowing mushroom field above the surface.");
			achievement.AddCondition(CustomFlagCondition.Create("Reach"));
			Main.Achievements.Register(achievement);
			achievement = new Achievement("INTO_ORBIT", "Into Orbit", "You can only go down from here!");
			achievement.AddCondition(CustomFlagCondition.Create("Reach"));
			Main.Achievements.Register(achievement);
			achievement = new Achievement("ROCK_BOTTOM", "Rock Bottom", "The only way is up!");
			achievement.AddCondition(CustomFlagCondition.Create("Reach"));
			Main.Achievements.Register(achievement);
			achievement = new Achievement("MECHA_MAYHEM", "Mecha Mayhem", "Do battle against the Twins, the Destroyer, and Skeletron Prime simultaneously and emerge victorious.");
			achievement.AddCondition(ProgressionEventCondition.Create(21));
			Main.Achievements.Register(achievement);
			achievement = new Achievement("GELATIN_WORLD_TOUR", "Gelatin World Tour", "Defeat every type of slime there is!");
			achievement.AddConditions(NPCKilledCondition.CreateMany(new short[]
			{
				-5,
				-6,
				1,
				81,
				71,
				-3,
				147,
				138,
				-10,
				50,
				59,
				16,
				-7,
				244,
				-8,
				-1,
				-2,
				184,
				204,
				225,
				-9,
				141,
				183,
				-4
			}));
			achievement.UseConditionsCompletedTracker();
			Main.Achievements.Register(achievement);
			achievement = new Achievement("FASHION_STATEMENT", "Fashion Statement", "Equip armor or vanity clothing in all three social slots.");
			achievement.AddCondition(CustomFlagCondition.Create("Equip"));
			Main.Achievements.Register(achievement);
			achievement = new Achievement("VEHICULAR_MANSLAUGHTER", "Vehicular Manslaughter", "Defeat an enemy by running it over with a minecart.");
			achievement.AddCondition(CustomFlagCondition.Create("Hit"));
			Main.Achievements.Register(achievement);
			achievement = new Achievement("BULLDOZER", "Bulldozer", "Destroy a total of 10,000 tiles.");
			achievement.AddCondition(CustomIntCondition.Create("Pick", 10000));
			achievement.UseTrackerFromCondition("Pick");
			Main.Achievements.Register(achievement);
			achievement = new Achievement("THERE_ARE_SOME_WHO_CALL_HIM", "There are Some Who Call Him...", "Kill Tim.");
			achievement.AddCondition(NPCKilledCondition.Create(45));
			Main.Achievements.Register(achievement);
			achievement = new Achievement("DECEIVER_OF_FOOLS", "Deceiver of Fools", "Kill a nymph.");
			achievement.AddCondition(NPCKilledCondition.Create(196));
			Main.Achievements.Register(achievement);
			achievement = new Achievement("SWORD_OF_THE_HERO", "Sword of the Hero", "Obtain a Terra Blade, forged from the finest blades of light and darkness.");
			achievement.AddCondition(ItemPickupCondition.Create(757));
			Main.Achievements.Register(achievement);
			achievement = new Achievement("LUCKY_BREAK", "Lucky Break", "Survive a long fall with just a sliver of health remaining.");
			achievement.AddCondition(CustomFlagCondition.Create("Hit"));
			Main.Achievements.Register(achievement);
			achievement = new Achievement("THROWING_LINES", "Throwing Lines", "Throw a yoyo.");
			achievement.AddCondition(CustomFlagCondition.Create("Use"));
			Main.Achievements.Register(achievement);
			achievement = new Achievement("DYE_HARD", "Dye Hard", "Equip a dye in every possible dye slot.");
			achievement.AddCondition(CustomFlagCondition.Create("Equip"));
			Main.Achievements.Register(achievement);
			achievement = new Achievement("SICK_THROW", "Sick Throw", "Obtain the Terrarian.");
			achievement.AddCondition(ItemPickupCondition.Create(3389));
			Main.Achievements.Register(achievement);
			achievement = new Achievement("FREQUENT_FLYER", "The Frequent Flyer", "Spend over 1 gold being treated by the nurse.");
			achievement.AddCondition(CustomFloatCondition.Create("Pay", 10000f));
			achievement.UseTrackerFromCondition("Pay");
			Main.Achievements.Register(achievement);
			achievement = new Achievement("THE_CAVALRY", "The Cavalry", "Equip a mount.");
			achievement.AddCondition(CustomFlagCondition.Create("Equip"));
			Main.Achievements.Register(achievement);
			achievement = new Achievement("COMPLETELY_AWESOME", "Completely Awesome", "Obtain a minishark.");
			achievement.AddCondition(ItemPickupCondition.Create(98));
			Main.Achievements.Register(achievement);
			achievement = new Achievement("TIL_DEATH", "Til Death...", "Kill the groom.");
			achievement.AddCondition(NPCKilledCondition.Create(53));
			Main.Achievements.Register(achievement);
			achievement = new Achievement("ARCHAEOLOGIST", "Archaeologist", "Kill Doctor Bones.");
			achievement.AddCondition(NPCKilledCondition.Create(52));
			Main.Achievements.Register(achievement);
			achievement = new Achievement("PRETTY_IN_PINK", "Pretty in Pink", "Kill pinky.");
			achievement.AddCondition(NPCKilledCondition.Create(-4));
			Main.Achievements.Register(achievement);
			achievement = new Achievement("RAINBOWS_AND_UNICORNS", "Rainbows and Unicorns", "Fire a rainbow gun while riding on a unicorn.");
			achievement.AddCondition(CustomFlagCondition.Create("Use"));
			Main.Achievements.Register(achievement);
			achievement = new Achievement("YOU_AND_WHAT_ARMY", "You and What Army?", "Command nine summoned minions simultaneously.");
			achievement.AddCondition(CustomFlagCondition.Create("Spawn"));
			Main.Achievements.Register(achievement);
			achievement = new Achievement("PRISMANCER", "Prismancer", "Obtain a rainbow rod.");
			achievement.AddCondition(ItemPickupCondition.Create(495));
			Main.Achievements.Register(achievement);
			achievement = new Achievement("IT_CAN_TALK", "It Can Talk?!", "Build a house in a mushroom biome and have Truffle move in.");
			achievement.AddCondition(ProgressionEventCondition.Create(18));
			Main.Achievements.Register(achievement);
			achievement = new Achievement("WATCH_YOUR_STEP", "Watch Your Step!", "Become a victim to a nasty underground trap.");
			achievement.AddCondition(CustomFlagCondition.Create("Hit"));
			Main.Achievements.Register(achievement);
			achievement = new Achievement("MARATHON_MEDALIST", "Marathon Medalist", "Travel a total of 26.2 miles on foot.");
			achievement.AddCondition(CustomFloatCondition.Create("Move", 1106688f));
			achievement.UseTrackerFromCondition("Move");
			Main.Achievements.Register(achievement);
			achievement = new Achievement("GLORIOUS_GOLDEN_POLE", "Glorious Golden Pole", "Obtain a golden fishing rod.");
			achievement.AddCondition(ItemPickupCondition.Create(2294));
			Main.Achievements.Register(achievement);
			achievement = new Achievement("SERVANT_IN_TRAINING", "Servant-in-Training", "Complete your 1st quest for the angler.");
			achievement.AddCondition(CustomFlagCondition.Create("Finish"));
			Main.Achievements.Register(achievement);
			achievement = new Achievement("GOOD_LITTLE_SLAVE", "Good Little Slave", "Complete your 10th quest for the angler.");
			achievement.AddCondition(CustomIntCondition.Create("Finish", 10));
			achievement.UseTrackerFromCondition("Finish");
			Main.Achievements.Register(achievement);
			achievement = new Achievement("TROUT_MONKEY", "Trout Monkey", "Complete your 25th quest for the angler.");
			achievement.AddCondition(CustomIntCondition.Create("Finish", 25));
			achievement.UseTrackerFromCondition("Finish");
			Main.Achievements.Register(achievement);
			achievement = new Achievement("FAST_AND_FISHIOUS", "Fast and Fishious", "Complete your 50th quest for the angler.");
			achievement.AddCondition(CustomIntCondition.Create("Finish", 50));
			achievement.UseTrackerFromCondition("Finish");
			Main.Achievements.Register(achievement);
			achievement = new Achievement("SUPREME_HELPER_MINION", "Supreme Helper Minion!", "Complete a grand total of 200 quests for the angler.");
			achievement.AddCondition(CustomIntCondition.Create("Finish", 200));
			achievement.UseTrackerFromCondition("Finish");
			Main.Achievements.Register(achievement);
			achievement = new Achievement("TOPPED_OFF", "Topped Off", "Attain maximum health and mana possible without accessories or buffs.");
			achievement.AddCondition(CustomFlagCondition.Create("Use"));
			Main.Achievements.Register(achievement);
			achievement = new Achievement("SLAYER_OF_WORLDS", "Slayer of Worlds", "Defeat every boss in Terraria.");
			achievement.AddCondition(NPCKilledCondition.Create(new short[]
			{
				13,
				14,
				15
			}));
			achievement.AddCondition(NPCKilledCondition.Create(new short[]
			{
				113,
				114
			}));
			achievement.AddCondition(NPCKilledCondition.Create(new short[]
			{
				125,
				126
			}));
			achievement.AddConditions(NPCKilledCondition.CreateMany(new short[]
			{
				4,
				35,
				50,
				222,
				113,
				134,
				127,
				262,
				245,
				439,
				398,
				370
			}));
			achievement.UseConditionsCompletedTracker();
			Main.Achievements.Register(achievement);
			achievement = new Achievement("YOU_CAN_DO_IT", "You Can Do It!", "Survive your character's first full night.");
			achievement.AddCondition(ProgressionEventCondition.Create(1));
			Main.Achievements.Register(achievement);
			achievement = new Achievement("MATCHING_ATTIRE", "Matching Attire", "Equip armor in all three armor slots: head, chest, and feet.");
			achievement.AddCondition(CustomFlagCondition.Create("Equip"));
			Main.Achievements.Register(achievement);
			int num = 0;
			Main.Achievements.RegisterIconIndex("TIMBER", num++);
			Main.Achievements.RegisterIconIndex("NO_HOBO", num++);
			Main.Achievements.RegisterIconIndex("OBTAIN_HAMMER", num++);
			Main.Achievements.RegisterIconIndex("HEART_BREAKER", num++);
			Main.Achievements.RegisterIconIndex("OOO_SHINY", num++);
			Main.Achievements.RegisterIconIndex("HEAVY_METAL", num++);
			Main.Achievements.RegisterIconIndex("I_AM_LOOT", num++);
			Main.Achievements.RegisterIconIndex("STAR_POWER", num++);
			Main.Achievements.RegisterIconIndex("HOLD_ON_TIGHT", num++);
			Main.Achievements.RegisterIconIndex("EYE_ON_YOU", num++);
			Main.Achievements.RegisterIconIndex("SMASHING_POPPET", num++);
			Main.Achievements.RegisterIconIndex("WORM_FODDER", num++);
			Main.Achievements.RegisterIconIndex("MASTERMIND", num++);
			Main.Achievements.RegisterIconIndex("WHERES_MY_HONEY", num++);
			Main.Achievements.RegisterIconIndex("STING_OPERATION", num++);
			Main.Achievements.RegisterIconIndex("BONED", num++);
			Main.Achievements.RegisterIconIndex("DUNGEON_HEIST", num++);
			Main.Achievements.RegisterIconIndex("ITS_GETTING_HOT_IN_HERE", num++);
			Main.Achievements.RegisterIconIndex("MINER_FOR_FIRE", num++);
			Main.Achievements.RegisterIconIndex("STILL_HUNGRY", num++);
			Main.Achievements.RegisterIconIndex("ITS_HARD", num++);
			Main.Achievements.RegisterIconIndex("BEGONE_EVIL", num++);
			Main.Achievements.RegisterIconIndex("EXTRA_SHINY", num++);
			Main.Achievements.RegisterIconIndex("HEAD_IN_THE_CLOUDS", num++);
			Main.Achievements.RegisterIconIndex("LIKE_A_BOSS", num++);
			Main.Achievements.RegisterIconIndex("BUCKETS_OF_BOLTS", num++);
			Main.Achievements.RegisterIconIndex("DRAX_ATTAX", num++);
			Main.Achievements.RegisterIconIndex("PHOTOSYNTHESIS", num++);
			Main.Achievements.RegisterIconIndex("GET_A_LIFE", num++);
			Main.Achievements.RegisterIconIndex("THE_GREAT_SOUTHERN_PLANTKILL", num++);
			Main.Achievements.RegisterIconIndex("TEMPLE_RAIDER", num++);
			Main.Achievements.RegisterIconIndex("LIHZAHRDIAN_IDOL", num++);
			Main.Achievements.RegisterIconIndex("ROBBING_THE_GRAVE", num++);
			Main.Achievements.RegisterIconIndex("BIG_BOOTY", num++);
			Main.Achievements.RegisterIconIndex("FISH_OUT_OF_WATER", num++);
			Main.Achievements.RegisterIconIndex("OBSESSIVE_DEVOTION", num++);
			Main.Achievements.RegisterIconIndex("STAR_DESTROYER", num++);
			Main.Achievements.RegisterIconIndex("CHAMPION_OF_TERRARIA", num++);
			Main.Achievements.RegisterIconIndex("BLOODBATH", num++);
			Main.Achievements.RegisterIconIndex("GOBLIN_PUNTER", num++);
			Main.Achievements.RegisterIconIndex("KILL_THE_SUN", num++);
			Main.Achievements.RegisterIconIndex("WALK_THE_PLANK", num++);
			Main.Achievements.RegisterIconIndex("DO_YOU_WANT_TO_SLAY_A_SNOWMAN", num++);
			Main.Achievements.RegisterIconIndex("TIN_FOIL_HATTER", num++);
			Main.Achievements.RegisterIconIndex("BALEFUL_HARVEST", num++);
			Main.Achievements.RegisterIconIndex("ICE_SCREAM", num++);
			Main.Achievements.RegisterIconIndex("SLIPPERY_SHINOBI", num++);
			Main.Achievements.RegisterIconIndex("STICKY_SITUATION", num++);
			Main.Achievements.RegisterIconIndex("REAL_ESTATE_AGENT", num++);
			Main.Achievements.RegisterIconIndex("NOT_THE_BEES", num++);
			Main.Achievements.RegisterIconIndex("JEEPERS_CREEPERS", num++);
			Main.Achievements.RegisterIconIndex("FUNKYTOWN", num++);
			Main.Achievements.RegisterIconIndex("INTO_ORBIT", num++);
			Main.Achievements.RegisterIconIndex("ROCK_BOTTOM", num++);
			Main.Achievements.RegisterIconIndex("MECHA_MAYHEM", num++);
			Main.Achievements.RegisterIconIndex("GELATIN_WORLD_TOUR", num++);
			Main.Achievements.RegisterIconIndex("FASHION_STATEMENT", num++);
			Main.Achievements.RegisterIconIndex("VEHICULAR_MANSLAUGHTER", num++);
			Main.Achievements.RegisterIconIndex("BULLDOZER", num++);
			Main.Achievements.RegisterIconIndex("THERE_ARE_SOME_WHO_CALL_HIM", num++);
			Main.Achievements.RegisterIconIndex("DECEIVER_OF_FOOLS", num++);
			Main.Achievements.RegisterIconIndex("SWORD_OF_THE_HERO", num++);
			Main.Achievements.RegisterIconIndex("LUCKY_BREAK", num++);
			Main.Achievements.RegisterIconIndex("THROWING_LINES", num++);
			Main.Achievements.RegisterIconIndex("DYE_HARD", num++);
			Main.Achievements.RegisterIconIndex("FREQUENT_FLYER", num++);
			Main.Achievements.RegisterIconIndex("THE_CAVALRY", num++);
			Main.Achievements.RegisterIconIndex("COMPLETELY_AWESOME", num++);
			Main.Achievements.RegisterIconIndex("TIL_DEATH", num++);
			Main.Achievements.RegisterIconIndex("ARCHAEOLOGIST", num++);
			Main.Achievements.RegisterIconIndex("PRETTY_IN_PINK", num++);
			Main.Achievements.RegisterIconIndex("RAINBOWS_AND_UNICORNS", num++);
			Main.Achievements.RegisterIconIndex("YOU_AND_WHAT_ARMY", num++);
			Main.Achievements.RegisterIconIndex("PRISMANCER", num++);
			Main.Achievements.RegisterIconIndex("IT_CAN_TALK", num++);
			Main.Achievements.RegisterIconIndex("WATCH_YOUR_STEP", num++);
			Main.Achievements.RegisterIconIndex("MARATHON_MEDALIST", num++);
			Main.Achievements.RegisterIconIndex("GLORIOUS_GOLDEN_POLE", num++);
			Main.Achievements.RegisterIconIndex("SERVANT_IN_TRAINING", num++);
			Main.Achievements.RegisterIconIndex("GOOD_LITTLE_SLAVE", num++);
			Main.Achievements.RegisterIconIndex("TROUT_MONKEY", num++);
			Main.Achievements.RegisterIconIndex("FAST_AND_FISHIOUS", num++);
			Main.Achievements.RegisterIconIndex("SUPREME_HELPER_MINION", num++);
			Main.Achievements.RegisterIconIndex("TOPPED_OFF", num++);
			Main.Achievements.RegisterIconIndex("SLAYER_OF_WORLDS", num++);
			Main.Achievements.RegisterIconIndex("YOU_CAN_DO_IT", num++);
			Main.Achievements.RegisterIconIndex("SICK_THROW", num++);
			Main.Achievements.RegisterIconIndex("MATCHING_ATTIRE", num++);
			AchievementCategory category = AchievementCategory.Slayer;
			Main.Achievements.RegisterAchievementCategory("EYE_ON_YOU", category);
			Main.Achievements.RegisterAchievementCategory("SLIPPERY_SHINOBI", category);
			Main.Achievements.RegisterAchievementCategory("WORM_FODDER", category);
			Main.Achievements.RegisterAchievementCategory("MASTERMIND", category);
			Main.Achievements.RegisterAchievementCategory("STING_OPERATION", category);
			Main.Achievements.RegisterAchievementCategory("BONED", category);
			Main.Achievements.RegisterAchievementCategory("STILL_HUNGRY", category);
			Main.Achievements.RegisterAchievementCategory("BUCKETS_OF_BOLTS", category);
			Main.Achievements.RegisterAchievementCategory("THE_GREAT_SOUTHERN_PLANTKILL", category);
			Main.Achievements.RegisterAchievementCategory("LIHZAHRDIAN_IDOL", category);
			Main.Achievements.RegisterAchievementCategory("FISH_OUT_OF_WATER", category);
			Main.Achievements.RegisterAchievementCategory("OBSESSIVE_DEVOTION", category);
			Main.Achievements.RegisterAchievementCategory("STAR_DESTROYER", category);
			Main.Achievements.RegisterAchievementCategory("CHAMPION_OF_TERRARIA", category);
			Main.Achievements.RegisterAchievementCategory("GOBLIN_PUNTER", category);
			Main.Achievements.RegisterAchievementCategory("DO_YOU_WANT_TO_SLAY_A_SNOWMAN", category);
			Main.Achievements.RegisterAchievementCategory("WALK_THE_PLANK", category);
			Main.Achievements.RegisterAchievementCategory("BALEFUL_HARVEST", category);
			Main.Achievements.RegisterAchievementCategory("ICE_SCREAM", category);
			Main.Achievements.RegisterAchievementCategory("TIN_FOIL_HATTER", category);
			Main.Achievements.RegisterAchievementCategory("TIL_DEATH", category);
			Main.Achievements.RegisterAchievementCategory("THERE_ARE_SOME_WHO_CALL_HIM", category);
			Main.Achievements.RegisterAchievementCategory("ARCHAEOLOGIST", category);
			Main.Achievements.RegisterAchievementCategory("PRETTY_IN_PINK", category);
			Main.Achievements.RegisterAchievementCategory("DECEIVER_OF_FOOLS", category);
			Main.Achievements.RegisterAchievementCategory("VEHICULAR_MANSLAUGHTER", category);
			category = AchievementCategory.Explorer;
			Main.Achievements.RegisterAchievementCategory("SMASHING_POPPET", category);
			Main.Achievements.RegisterAchievementCategory("BEGONE_EVIL", category);
			Main.Achievements.RegisterAchievementCategory("ITS_HARD", category);
			Main.Achievements.RegisterAchievementCategory("FUNKYTOWN", category);
			Main.Achievements.RegisterAchievementCategory("WATCH_YOUR_STEP", category);
			Main.Achievements.RegisterAchievementCategory("YOU_CAN_DO_IT", category);
			Main.Achievements.RegisterAchievementCategory("BLOODBATH", category);
			Main.Achievements.RegisterAchievementCategory("KILL_THE_SUN", category);
			Main.Achievements.RegisterAchievementCategory("STICKY_SITUATION", category);
			Main.Achievements.RegisterAchievementCategory("NO_HOBO", category);
			Main.Achievements.RegisterAchievementCategory("IT_CAN_TALK", category);
			Main.Achievements.RegisterAchievementCategory("HEART_BREAKER", category);
			Main.Achievements.RegisterAchievementCategory("I_AM_LOOT", category);
			Main.Achievements.RegisterAchievementCategory("ROBBING_THE_GRAVE", category);
			Main.Achievements.RegisterAchievementCategory("GET_A_LIFE", category);
			Main.Achievements.RegisterAchievementCategory("JEEPERS_CREEPERS", category);
			Main.Achievements.RegisterAchievementCategory("WHERES_MY_HONEY", category);
			Main.Achievements.RegisterAchievementCategory("DUNGEON_HEIST", category);
			Main.Achievements.RegisterAchievementCategory("BIG_BOOTY", category);
			Main.Achievements.RegisterAchievementCategory("ITS_GETTING_HOT_IN_HERE", category);
			Main.Achievements.RegisterAchievementCategory("INTO_ORBIT", category);
			Main.Achievements.RegisterAchievementCategory("ROCK_BOTTOM", category);
			Main.Achievements.RegisterAchievementCategory("OOO_SHINY", category);
			Main.Achievements.RegisterAchievementCategory("EXTRA_SHINY", category);
			Main.Achievements.RegisterAchievementCategory("PHOTOSYNTHESIS", category);
			category = AchievementCategory.Challenger;
			Main.Achievements.RegisterAchievementCategory("GELATIN_WORLD_TOUR", category);
			Main.Achievements.RegisterAchievementCategory("SLAYER_OF_WORLDS", category);
			Main.Achievements.RegisterAchievementCategory("REAL_ESTATE_AGENT", category);
			Main.Achievements.RegisterAchievementCategory("YOU_AND_WHAT_ARMY", category);
			Main.Achievements.RegisterAchievementCategory("TOPPED_OFF", category);
			Main.Achievements.RegisterAchievementCategory("MECHA_MAYHEM", category);
			Main.Achievements.RegisterAchievementCategory("BULLDOZER", category);
			Main.Achievements.RegisterAchievementCategory("NOT_THE_BEES", category);
			Main.Achievements.RegisterAchievementCategory("RAINBOWS_AND_UNICORNS", category);
			Main.Achievements.RegisterAchievementCategory("THROWING_LINES", category);
			Main.Achievements.RegisterAchievementCategory("FREQUENT_FLYER", category);
			Main.Achievements.RegisterAchievementCategory("LUCKY_BREAK", category);
			Main.Achievements.RegisterAchievementCategory("MARATHON_MEDALIST", category);
			Main.Achievements.RegisterAchievementCategory("SERVANT_IN_TRAINING", category);
			Main.Achievements.RegisterAchievementCategory("GOOD_LITTLE_SLAVE", category);
			Main.Achievements.RegisterAchievementCategory("TROUT_MONKEY", category);
			Main.Achievements.RegisterAchievementCategory("FAST_AND_FISHIOUS", category);
			Main.Achievements.RegisterAchievementCategory("SUPREME_HELPER_MINION", category);
			category = AchievementCategory.Collector;
			Main.Achievements.RegisterAchievementCategory("OBTAIN_HAMMER", category);
			Main.Achievements.RegisterAchievementCategory("HEAVY_METAL", category);
			Main.Achievements.RegisterAchievementCategory("STAR_POWER", category);
			Main.Achievements.RegisterAchievementCategory("MINER_FOR_FIRE", category);
			Main.Achievements.RegisterAchievementCategory("HEAD_IN_THE_CLOUDS", category);
			Main.Achievements.RegisterAchievementCategory("DRAX_ATTAX", category);
			Main.Achievements.RegisterAchievementCategory("PRISMANCER", category);
			Main.Achievements.RegisterAchievementCategory("SWORD_OF_THE_HERO", category);
			Main.Achievements.RegisterAchievementCategory("HOLD_ON_TIGHT", category);
			Main.Achievements.RegisterAchievementCategory("THE_CAVALRY", category);
			Main.Achievements.RegisterAchievementCategory("DYE_HARD", category);
			Main.Achievements.RegisterAchievementCategory("MATCHING_ATTIRE", category);
			Main.Achievements.RegisterAchievementCategory("FASHION_STATEMENT", category);
			Main.Achievements.RegisterAchievementCategory("COMPLETELY_AWESOME", category);
			Main.Achievements.RegisterAchievementCategory("TIMBER", category);
			Main.Achievements.RegisterAchievementCategory("SICK_THROW", category);
			Main.Achievements.RegisterAchievementCategory("GLORIOUS_GOLDEN_POLE", category);
			Main.Achievements.RegisterAchievementCategory("TEMPLE_RAIDER", category);
			Main.Achievements.RegisterAchievementCategory("LIKE_A_BOSS", category);
			Main.Achievements.Load();
			Main.Achievements.OnAchievementCompleted += new Achievement.AchievementCompleted(AchievementInitializer.OnAchievementCompleted);
			AchievementsHelper.Initialize();
		}
		private static void OnAchievementCompleted(Achievement achievement)
		{
			Main.NewText("Achievement complete! " + AchievementTagHandler.GenerateTag(achievement), 255, 255, 255, false);
		}
	}
}
