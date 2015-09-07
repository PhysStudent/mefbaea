using System;
using System.Collections.Generic;
using System.IO;
using Terraria.GameContent.Tile_Entities;
namespace Terraria.DataStructures
{
	public abstract class TileEntity
	{
		public const int MaxEntitiesPerChunk = 1000;
		public static Dictionary<int, TileEntity> ByID = new Dictionary<int, TileEntity>();
		public static Dictionary<Point16, TileEntity> ByPosition = new Dictionary<Point16, TileEntity>();
		public static int TileEntitiesNextID = 0;
		public int ID;
		public Point16 Position;
		public byte type;
		public static event Action _UpdateStart;
		public static event Action _UpdateEnd;
		public static int AssignNewID()
		{
			return TileEntity.TileEntitiesNextID++;
		}
		public static void UpdateStart()
		{
			if (TileEntity._UpdateStart != null)
			{
				TileEntity._UpdateStart();
			}
		}
		public static void UpdateEnd()
		{
			if (TileEntity._UpdateEnd != null)
			{
				TileEntity._UpdateEnd();
			}
		}
		public static void InitializeAll()
		{
			TETrainingDummy.Initialize();
		}
		public virtual void Update()
		{
		}
		public static void Write(BinaryWriter writer, TileEntity ent)
		{
			writer.Write(ent.type);
			ent.WriteInner(writer);
		}
		public static TileEntity Read(BinaryReader reader)
		{
			TileEntity tileEntity = null;
			byte b = reader.ReadByte();
			switch (b)
			{
			case 0:
				tileEntity = new TETrainingDummy();
				break;
			case 1:
				tileEntity = new TEItemFrame();
				break;
			}
			tileEntity.type = b;
			tileEntity.ReadInner(reader);
			return tileEntity;
		}
		private void WriteInner(BinaryWriter writer)
		{
			writer.Write(this.ID);
			writer.Write(this.Position.X);
			writer.Write(this.Position.Y);
			this.WriteExtraData(writer);
		}
		private void ReadInner(BinaryReader reader)
		{
			this.ID = reader.ReadInt32();
			this.Position = new Point16(reader.ReadInt16(), reader.ReadInt16());
			this.ReadExtraData(reader);
		}
		public virtual void WriteExtraData(BinaryWriter writer)
		{
		}
		public virtual void ReadExtraData(BinaryReader reader)
		{
		}
	}
}
