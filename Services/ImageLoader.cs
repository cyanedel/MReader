using System.IO.Compression;
using mreader.Helpers;

namespace mreader.Services
{
	static class ImageLoader
	{
		public static FileInfo[] ExtractImageFromDir(string filePath)
		{
			DirectoryInfo d = new(filePath);
			var imageFiles = d.GetFiles("*.*", SearchOption.TopDirectoryOnly)
			.Where(f => Generic.IsImageFile(f.FullName))
			.OrderBy(f => Generic.ExtractNumber(f.Name))
			.ToArray();

			return imageFiles;
		}
		public static List<(string name, byte[] imageData)> ExtractImageFromZip(string filePath)
		{
			var images = new List<(string, byte[])>();

			using var zip = ZipFile.OpenRead(filePath);
			foreach (var entry in zip.Entries
			.Where(e => !string.IsNullOrEmpty(e.Name) && Generic.IsImageFile(e.FullName))
			.OrderBy(e => Generic.ExtractNumber(e.FullName)))
			{
				byte[] imageData;
				using (var entryStream = entry.Open())
				using (var ms = new MemoryStream())
				{
					entryStream.CopyTo(ms);
					imageData = ms.ToArray();
				}
				images.Add((entry.FullName, imageData));
			}
			return images;
		}
	}
}
