using System.Collections.ObjectModel;
using mreader.Models;
using mreader.Services;
using mreader.Views.Pages;

namespace mreader;

public partial class MainPage : ContentPage
{
    private const double MinimumCardWidth = 120;
    private const double CardSpacing = 8;
    private readonly ObservableCollection<LibraryItem> items = [];
    private string mainDirectory = string.Empty;
    private string currentDirectory = string.Empty;

    public MainPage()
    {
        InitializeComponent();
        LibraryItems.ItemsSource = items;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (string.IsNullOrWhiteSpace(mainDirectory)) await InitializeLibraryAsync();
        else LoadDirectory(currentDirectory);
    }

    protected override void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);
        UpdateLibraryLayout(width);
    }

    private async Task InitializeLibraryAsync()
    {
        var settings = SettingsService.Load();
        if (!string.IsNullOrWhiteSpace(settings.MainDirectory) && Directory.Exists(settings.MainDirectory))
        {
            mainDirectory = settings.MainDirectory;
            currentDirectory = mainDirectory;
            LoadDirectory(currentDirectory);
            return;
        }
        if (await DisplayAlert("Choose manga folder", "Select the main folder that contains your local manga.", "Choose folder", "Later")) await ChooseMainDirectoryAsync();
    }

    private async Task ChooseMainDirectoryAsync()
    {
        var selectedPath = await MainDirectoryPicker.PickAsync(Window);
        if (string.IsNullOrWhiteSpace(selectedPath)) return;
        mainDirectory = selectedPath;
        currentDirectory = selectedPath;
        SettingsService.Save(new AppSettings { MainDirectory = mainDirectory });
        LoadDirectory(currentDirectory);
    }

    private void LoadDirectory(string directoryPath)
    {
        if (!Directory.Exists(directoryPath))
        {
            items.Clear();
            CurrentPathLabel.Text = "The selected manga directory is unavailable.";
            BackButton.IsVisible = false;
            return;
        }
        currentDirectory = directoryPath;
        items.Clear();
        try
        {
            foreach (var item in MangaLibraryService.GetItems(directoryPath)) items.Add(item);
        }
        catch (UnauthorizedAccessException)
        {
            CurrentPathLabel.Text = "This folder cannot be read.";
            return;
        }
        CurrentPathLabel.Text = directoryPath;
        BackButton.IsVisible = !Path.GetFullPath(directoryPath).TrimEnd(Path.DirectorySeparatorChar)
            .Equals(Path.GetFullPath(mainDirectory).TrimEnd(Path.DirectorySeparatorChar), StringComparison.OrdinalIgnoreCase);
        UpdateLibraryLayout(Width);
    }

    private void OnLibrarySizeChanged(object? sender, EventArgs e) => UpdateLibraryLayout(Width);

    private void UpdateLibraryLayout(double pageWidth)
    {
        if (pageWidth <= 0) return;
        // Match the CollectionView's horizontal margins; cards themselves fill each grid cell.
        var availableWidth = Math.Max(0, pageWidth - 28);
        var columns = Math.Max(1, (int)Math.Floor((availableWidth + CardSpacing) / (MinimumCardWidth + CardSpacing)));
        var cardWidth = (availableWidth - ((columns - 1) * CardSpacing)) / columns;
        LibraryLayout.Span = columns;
        foreach (var item in items) item.SetCardWidth(cardWidth);
    }

    private async void OnChangeFolderClicked(object sender, EventArgs e) => await ChooseMainDirectoryAsync();
    private void OnRefreshClicked(object sender, EventArgs e) => LoadDirectory(currentDirectory);
    private void OnBackClicked(object sender, EventArgs e)
    {
        var parent = Directory.GetParent(currentDirectory)?.FullName;
        if (!string.IsNullOrWhiteSpace(parent) && parent.StartsWith(mainDirectory, StringComparison.OrdinalIgnoreCase)) LoadDirectory(parent);
    }
    private void OnOpenFolderClicked(object sender, EventArgs e)
    {
        if ((sender as BindableObject)?.BindingContext is LibraryItem { IsFolder: true } folder) LoadDirectory(folder.FullPath);
    }
    private async void OnReadClicked(object sender, EventArgs e)
    {
        if ((sender as BindableObject)?.BindingContext is LibraryItem { IsManga: true } manga) await Navigation.PushAsync(new ImageViewPage(manga));
    }
    private async void OnItemDoubleTapped(object sender, TappedEventArgs e)
    {
        if ((sender as BindableObject)?.BindingContext is not LibraryItem item) return;
        if (item.IsFolder) LoadDirectory(item.FullPath);
        else await Navigation.PushAsync(new ImageViewPage(item));
    }
}
