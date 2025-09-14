using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace mreader.Helpers
{
	internal class Generic
	{
		public static int ExtractNumber(string fileName)
		{
			// Extract the first number from the filename
			var match = Regex.Match(fileName, @"\d+");
			return match.Success ? int.Parse(match.Value) : 0;
		}
		public static bool IsImageFile(string fileName)
		{
			string ext = Path.GetExtension(fileName).ToLower();
			return ext == ".jpg" || ext == ".jpeg" || ext == ".png" || ext == ".bmp" || ext == ".gif" || ext == ".webp";
		}
		public static bool IsArchive(string filePath)
		{
			string ext = Path.GetExtension(filePath).ToLower();
			return ext == ".zip" || ext == ".rar";
		}
	}
}
