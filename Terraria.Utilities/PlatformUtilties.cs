using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;
namespace Terraria.Utilities
{
	public static class PlatformUtilties
	{
		public static string GetClipboard()
		{
			string clipboardText = "";
            Thread thread = new Thread(delegate(object state)
			{
				clipboardText = Clipboard.GetText();
			});
			thread.SetApartmentState(ApartmentState.STA);
			thread.Start();
			thread.Join();
			char[] array = new char[clipboardText.Length];
			int length = 0;
			for (int i = 0; i < clipboardText.Length; i++)
			{
				if (clipboardText[i] >= ' ' && clipboardText[i] != '\u007f')
				{
					array[length++] = clipboardText[i];
				}
			}
			return new string(array, 0, length);
		}
		public static void SetClipboard(string text)
		{
            Thread thread = new Thread(delegate(object state)
			{
				if (text.Length > 0)
				{
					Clipboard.SetText(text);
				}
			});
			thread.SetApartmentState(ApartmentState.STA);
			thread.Start();
			thread.Join();
		}
		public static string GetStoragePath()
		{
			string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "My Games");
			return Path.Combine(path, "Terraria");
		}
	}
}
