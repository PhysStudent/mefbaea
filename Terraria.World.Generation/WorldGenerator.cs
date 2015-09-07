using System;
using System.Collections.Generic;
using System.Diagnostics;
using Terraria.GameContent.UI.States;
namespace Terraria.World.Generation
{
	internal class WorldGenerator
	{
		private List<GenPass> _passes = new List<GenPass>();
		private float _totalLoadWeight;
		public void Append(GenPass pass)
		{
			this._passes.Add(pass);
			this._totalLoadWeight += pass.Weight;
		}
		public void GenerateWorld(GenerationProgress progress = null)
		{
			Stopwatch stopwatch = new Stopwatch();
			float num = 0f;
			foreach (GenPass current in this._passes)
			{
				num += current.Weight;
			}
			if (progress == null)
			{
				progress = new GenerationProgress();
			}
			progress.TotalWeight = num;
			string text = "";
			Main.MenuUI.SetState(new UIWorldLoad(progress));
			Main.menuMode = 888;
			foreach (GenPass current2 in this._passes)
			{
				stopwatch.Start();
				progress.Start(current2.Weight);
				current2.Apply(progress);
				progress.End();
				string text2 = text;
				text = string.Concat(new string[]
				{
					text2,
					"Pass - ",
					current2.Name,
					" : ",
					stopwatch.Elapsed.TotalMilliseconds.ToString(),
					",\n"
				});
				stopwatch.Reset();
			}
		}
	}
}
