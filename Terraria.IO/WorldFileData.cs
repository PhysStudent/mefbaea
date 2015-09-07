using System;
using System.IO;
using Terraria.Utilities;
namespace Terraria.IO
{
	public class WorldFileData : FileData
	{
		public DateTime CreationTime;
		public int WorldSizeX;
		public int WorldSizeY;
		public bool IsValid = true;
		public string _worldSizeName;
		public bool IsExpertMode;
		public bool HasCorruption = true;
		public bool IsHardMode;
		public string WorldSizeName
		{
			get
			{
				return this._worldSizeName;
			}
		}
		public bool HasCrimson
		{
			get
			{
				return !this.HasCorruption;
			}
			set
			{
				this.HasCorruption = !value;
			}
		}
		public WorldFileData() : base("World")
		{
		}
		public WorldFileData(string path, bool cloudSave) : base("World", path, cloudSave)
		{
		}
		public override void SetAsActive()
		{
			Main.ActiveWorldFileData = this;
		}
		public void SetWorldSize(int x, int y)
		{
			this.WorldSizeX = x;
			this.WorldSizeY = y;
			if (x == 4200)
			{
				this._worldSizeName = "Small";
				return;
			}
			if (x == 6400)
			{
				this._worldSizeName = "Medium";
				return;
			}
			if (x != 8400)
			{
				this._worldSizeName = "Unknown";
				return;
			}
			this._worldSizeName = "Large";
		}
		public static WorldFileData FromInvalidWorld(string path, bool cloudSave)
		{
			WorldFileData worldFileData = new WorldFileData(path, cloudSave);
			worldFileData.IsExpertMode = false;
			worldFileData.Metadata = FileMetadata.FromCurrentSettings(FileType.World);
			worldFileData.SetWorldSize(1, 1);
			worldFileData.HasCorruption = true;
			worldFileData.IsHardMode = false;
			worldFileData.IsValid = false;
			worldFileData.Name = FileUtilities.GetFileName(path, false);
			if (!cloudSave)
			{
				worldFileData.CreationTime = File.GetCreationTime(path);
			}
			else
			{
				worldFileData.CreationTime = DateTime.Now;
			}
			return worldFileData;
		}
		public override void MoveToCloud()
		{
			if (base.IsCloudSave)
			{
				return;
			}
			string worldPathFromName = Main.GetWorldPathFromName(this.Name, true);
			if (FileUtilities.MoveToCloud(base.Path, worldPathFromName))
			{
				Main.LocalFavoriteData.ClearEntry(this);
				this._isCloudSave = true;
				this._path = worldPathFromName;
				Main.CloudFavoritesData.SaveFavorite(this);
			}
		}
		public override void MoveToLocal()
		{
			if (!base.IsCloudSave)
			{
				return;
			}
			string worldPathFromName = Main.GetWorldPathFromName(this.Name, false);
			if (FileUtilities.MoveToLocal(base.Path, worldPathFromName))
			{
				Main.CloudFavoritesData.ClearEntry(this);
				this._isCloudSave = false;
				this._path = worldPathFromName;
				Main.LocalFavoriteData.SaveFavorite(this);
			}
		}
	}
}
