using mreader.Services;

namespace mreader.Views.Components;

public partial class FileBrowser : ContentView
{
	public List<string> ImagePaths { get; set; }
	public FileBrowser()
	{
		InitializeComponent();
		var settings = SettingsService.Load();
		string mangaPath = settings.MainDirectory;
		FileInfo[] imageList = Directory.Exists(mangaPath)
			? ImageLoader.ExtractImageFromDir(mangaPath)
			: [];

		ImagePaths = imageList.Select(f => f.FullName).ToList();

		//foreach (var path in ImagePaths)
		//	Console.WriteLine(path);

		BindingContext = this;
	}
}
