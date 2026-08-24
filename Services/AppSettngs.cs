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
		public string MainDirectory { get; set; } = string.Empty;
		// Retained only to migrate the prototype's settings file.
		public string WorkDir { get; set; } = string.Empty;
	}

	public static class SettingsService
	{
		private static readonly string SettingsFile = Path.Combine(FileSystem.AppDataDirectory, "settings.json");
		public static AppSettings Load()
		{
			if (!File.Exists(SettingsFile))
				return new AppSettings();

			string json = File.ReadAllText(SettingsFile);
			var settings = JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
			if (string.IsNullOrWhiteSpace(settings.MainDirectory))
				settings.MainDirectory = settings.WorkDir;
			return settings;
		}

		public static void Save(AppSettings settings)
		{
			Directory.CreateDirectory(FileSystem.AppDataDirectory);
			string json = JsonSerializer.Serialize(settings);
			File.WriteAllText(SettingsFile, json);
		}
	}
}
