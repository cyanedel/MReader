using mreader.Models;
using mreader.Services;

namespace mreader.Views.Pages;

public partial class ImageViewPage : ContentPage
{
    public ImageViewPage(LibraryItem manga)
    {
        InitializeComponent();
        Title = manga.Name;
        TitleLabel.Text = manga.Name;
        try { Pages.ItemsSource = MangaLibraryService.GetPages(manga); }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or InvalidDataException)
        {
            EmptyLabel.Text = "This manga could not be opened. Check that the files are still available.";
        }
    }
}
