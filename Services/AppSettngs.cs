using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace mreader.Services
{
	public class AppSettings
	{
		public string WorkingPath { get; set; } = FileSystem.AppDataDirectory;
		public string ReadDir { get; set; } = string.Empty;
		public string WorkDir { get; set; } = string.Empty;
	}
	static class SettingsService
	{
		private static readonly string SettingsFile = Path.Combine(FileSystem.AppDataDirectory, "settings.json");
		public static AppSettings Load()
		{
			Console.WriteLine("setting path: ", SettingsFile);
			if (!File.Exists(SettingsFile))
				return new AppSettings();

			string json = File.ReadAllText(SettingsFile);
			return JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
			//return System.Text.Json.JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
		}

		public static void Save(AppSettings settings)
		{
			string json = System.Text.Json.JsonSerializer.Serialize(settings);
			File.WriteAllText(SettingsFile, json);
		}
	}
}
