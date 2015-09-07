using System;
namespace Terraria
{
	public class HitTile
	{
		public class HitTileObject
		{
			public int X;
			public int Y;
			public int damage;
			public int type;
			public int timeToLive;
			public int crackStyle;
			public HitTileObject()
			{
				this.Clear();
			}
			public void Clear()
			{
				this.X = 0;
				this.Y = 0;
				this.damage = 0;
				this.type = 0;
				this.timeToLive = 0;
				if (HitTile.rand == null)
				{
					HitTile.rand = new Random((int)DateTime.Now.Ticks);
				}
				this.crackStyle = HitTile.rand.Next(4);
				while (this.crackStyle == HitTile.lastCrack)
				{
					this.crackStyle = HitTile.rand.Next(4);
				}
				HitTile.lastCrack = this.crackStyle;
			}
		}
		internal const int UNUSED = 0;
		internal const int TILE = 1;
		internal const int WALL = 2;
		internal const int MAX_HITTILES = 20;
		internal const int TIMETOLIVE = 60;
		private static Random rand;
		private static int lastCrack = -1;
		public HitTile.HitTileObject[] data;
		private int[] order;
		private int bufferLocation;
		public HitTile()
		{
			HitTile.rand = new Random();
			this.data = new HitTile.HitTileObject[21];
			this.order = new int[21];
			for (int i = 0; i <= 20; i++)
			{
				this.data[i] = new HitTile.HitTileObject();
				this.order[i] = i;
			}
			this.bufferLocation = 0;
		}
		public int HitObject(int x, int y, int hitType)
		{
			HitTile.HitTileObject hitTileObject;
			for (int i = 0; i <= 20; i++)
			{
				int num = this.order[i];
				hitTileObject = this.data[num];
				if (hitTileObject.type == hitType)
				{
					if (hitTileObject.X == x && hitTileObject.Y == y)
					{
						return num;
					}
				}
				else if (i != 0 && hitTileObject.type == 0)
				{
					break;
				}
			}
			hitTileObject = this.data[this.bufferLocation];
			hitTileObject.X = x;
			hitTileObject.Y = y;
			hitTileObject.type = hitType;
			return this.bufferLocation;
		}
		public void UpdatePosition(int tileId, int x, int y)
		{
			if (tileId < 0 || tileId > 20)
			{
				return;
			}
			HitTile.HitTileObject hitTileObject = this.data[tileId];
			hitTileObject.X = x;
			hitTileObject.Y = y;
		}
		public int AddDamage(int tileId, int damageAmount, bool updateAmount = true)
		{
			if (tileId < 0 || tileId > 20)
			{
				return 0;
			}
			if (tileId == this.bufferLocation && damageAmount == 0)
			{
				return 0;
			}
			HitTile.HitTileObject hitTileObject = this.data[tileId];
			if (!updateAmount)
			{
				return hitTileObject.damage + damageAmount;
			}
			hitTileObject.damage += damageAmount;
			hitTileObject.timeToLive = 60;
			if (tileId != this.bufferLocation)
			{
				for (int i = 0; i <= 20; i++)
				{
					if (this.order[i] == tileId)
					{
						IL_E1:
						while (i > 1)
						{
							int num = this.order[i - 1];
							this.order[i - 1] = this.order[i];
							this.order[i] = num;
							i--;
						}
						this.order[1] = tileId;
						goto IL_EE;
					}
				}
                //???
				//goto IL_E1;
			}
			this.bufferLocation = this.order[20];
			this.data[this.bufferLocation].Clear();
			for (int i = 20; i > 0; i--)
			{
				this.order[i] = this.order[i - 1];
			}
			this.order[0] = this.bufferLocation;
			IL_EE:
			return hitTileObject.damage;
		}
		public void Clear(int tileId)
		{
			if (tileId < 0 || tileId > 20)
			{
				return;
			}
			this.data[tileId].Clear();
			for (int i = 0; i < 20; i++)
			{
				if (this.order[i] == tileId)
				{
					IL_47:
					while (i < 20)
					{
						this.order[i] = this.order[i + 1];
						i++;
					}
					this.order[20] = tileId;
					return;
				}
			}
			//goto IL_47;
		}
		public void Prune()
		{
			bool flag = false;
			for (int i = 0; i <= 20; i++)
			{
				HitTile.HitTileObject hitTileObject = this.data[i];
				if (hitTileObject.type != 0)
				{
					Tile tile = Main.tile[hitTileObject.X, hitTileObject.Y];
					if (hitTileObject.timeToLive <= 1)
					{
						hitTileObject.Clear();
						flag = true;
					}
					else
					{
						hitTileObject.timeToLive--;
						if ((double)hitTileObject.timeToLive < 12.0)
						{
							hitTileObject.damage -= 10;
						}
						else if ((double)hitTileObject.timeToLive < 24.0)
						{
							hitTileObject.damage -= 7;
						}
						else if ((double)hitTileObject.timeToLive < 36.0)
						{
							hitTileObject.damage -= 5;
						}
						else if ((double)hitTileObject.timeToLive < 48.0)
						{
							hitTileObject.damage -= 2;
						}
						if (hitTileObject.damage < 0)
						{
							hitTileObject.Clear();
							flag = true;
						}
						else if (hitTileObject.type == 1)
						{
							if (!tile.active())
							{
								hitTileObject.Clear();
								flag = true;
							}
						}
						else if (tile.wall == 0)
						{
							hitTileObject.Clear();
							flag = true;
						}
					}
				}
			}
			if (!flag)
			{
				return;
			}
			int num = 1;
			while (flag)
			{
				flag = false;
				for (int j = num; j < 20; j++)
				{
					if (this.data[this.order[j]].type == 0 && this.data[this.order[j + 1]].type != 0)
					{
						int num2 = this.order[j];
						this.order[j] = this.order[j + 1];
						this.order[j + 1] = num2;
						flag = true;
					}
				}
			}
		}
	}
}
