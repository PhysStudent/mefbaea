using System;
namespace Terraria.DataStructures
{
	public class TileObjectPreviewData
	{
		public const int None = 0;
		public const int ValidSpot = 1;
		public const int InvalidSpot = 2;
		private ushort _type;
		private short _style;
		private int _alternate;
		private int _random;
		private bool _active;
		private Point16 _size;
		private Point16 _coordinates;
		private Point16 _objectStart;
		private int[,] _data;
		private Point16 _dataSize;
		private float _percentValid;
		public static TileObjectPreviewData placementCache;
		public static TileObjectPreviewData randomCache;
		public bool Active
		{
			get
			{
				return this._active;
			}
			set
			{
				this._active = value;
			}
		}
		public ushort Type
		{
			get
			{
				return this._type;
			}
			set
			{
				this._type = value;
			}
		}
		public short Style
		{
			get
			{
				return this._style;
			}
			set
			{
				this._style = value;
			}
		}
		public int Alternate
		{
			get
			{
				return this._alternate;
			}
			set
			{
				this._alternate = value;
			}
		}
		public int Random
		{
			get
			{
				return this._random;
			}
			set
			{
				this._random = value;
			}
		}
		public Point16 Size
		{
			get
			{
				return this._size;
			}
			set
			{
				if (value.X <= 0 || value.Y <= 0)
				{
					throw new FormatException("PlacementData.Size was set to a negative value.");
				}
				if (value.X > this._dataSize.X || value.Y > this._dataSize.Y)
				{
					int num = (int)((value.X > this._dataSize.X) ? value.X : this._dataSize.X);
					int num2 = (int)((value.Y > this._dataSize.Y) ? value.Y : this._dataSize.Y);
					int[,] array = new int[num, num2];
					if (this._data != null)
					{
						for (int i = 0; i < (int)this._dataSize.X; i++)
						{
							for (int j = 0; j < (int)this._dataSize.Y; j++)
							{
								array[i, j] = this._data[i, j];
							}
						}
					}
					this._data = array;
					this._dataSize = new Point16(num, num2);
				}
				this._size = value;
			}
		}
		public Point16 Coordinates
		{
			get
			{
				return this._coordinates;
			}
			set
			{
				this._coordinates = value;
			}
		}
		public Point16 ObjectStart
		{
			get
			{
				return this._objectStart;
			}
			set
			{
				this._objectStart = value;
			}
		}
		public int this[int x, int y]
		{
			get
			{
				if (x < 0 || y < 0 || x >= (int)this._size.X || y >= (int)this._size.Y)
				{
					throw new IndexOutOfRangeException();
				}
				return this._data[x, y];
			}
			set
			{
				if (x < 0 || y < 0 || x >= (int)this._size.X || y >= (int)this._size.Y)
				{
					throw new IndexOutOfRangeException();
				}
				this._data[x, y] = value;
			}
		}
		public void Reset()
		{
			this._active = false;
			this._size = Point16.Zero;
			this._coordinates = Point16.Zero;
			this._objectStart = Point16.Zero;
			this._percentValid = 0f;
			this._type = 0;
			this._style = 0;
			this._alternate = -1;
			this._random = -1;
			if (this._data != null)
			{
				Array.Clear(this._data, 0, (int)(this._dataSize.X * this._dataSize.Y));
			}
		}
		public void CopyFrom(TileObjectPreviewData copy)
		{
			this._type = copy._type;
			this._style = copy._style;
			this._alternate = copy._alternate;
			this._random = copy._random;
			this._active = copy._active;
			this._size = copy._size;
			this._coordinates = copy._coordinates;
			this._objectStart = copy._objectStart;
			this._percentValid = copy._percentValid;
			if (this._data == null)
			{
				this._data = new int[(int)copy._dataSize.X, (int)copy._dataSize.Y];
				this._dataSize = copy._dataSize;
			}
			else
			{
				Array.Clear(this._data, 0, this._data.Length);
			}
			if (this._dataSize.X < copy._dataSize.X || this._dataSize.Y < copy._dataSize.Y)
			{
				int num = (int)((copy._dataSize.X > this._dataSize.X) ? copy._dataSize.X : this._dataSize.X);
				int num2 = (int)((copy._dataSize.Y > this._dataSize.Y) ? copy._dataSize.Y : this._dataSize.Y);
				this._data = new int[num, num2];
				this._dataSize = new Point16(num, num2);
			}
			for (int i = 0; i < (int)copy._dataSize.X; i++)
			{
				for (int j = 0; j < (int)copy._dataSize.Y; j++)
				{
					this._data[i, j] = copy._data[i, j];
				}
			}
		}
		public void AllInvalid()
		{
			for (int i = 0; i < (int)this._size.X; i++)
			{
				for (int j = 0; j < (int)this._size.Y; j++)
				{
					if (this._data[i, j] != 0)
					{
						this._data[i, j] = 2;
					}
				}
			}
		}
	}
}
