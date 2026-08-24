using mreader.Models;
using mreader.Services;

namespace mreader.Views.Pages;

public partial class ImageViewPage : ContentPage
{
    private IReadOnlyList<ReaderPage> pages = [];
    private ReaderSettings settings;
    private double zoomScale = 1;
    private double pinchStartZoom = 1;

    public ImageViewPage(LibraryItem manga)
    {
        InitializeComponent();
        Title = manga.Name;
        TitleLabel.Text = manga.Name;
        settings = SettingsService.Load().Reader;
        FitPicker.SelectedIndex = (int)settings.FitMode;
        DirectionPicker.SelectedIndex = (int)settings.Direction;
        PageGapSwitch.IsToggled = settings.ShowPageGap;

        try
        {
            pages = MangaLibraryService.GetPages(manga);
            Pages.ItemsSource = pages;
            ApplyReaderSettings();
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or InvalidDataException)
        {
            pages = [];
            EmptyLabel.Text = "This manga could not be opened. Check that the files are still available.";
        }
    }

    protected override void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);
        ApplyReaderSettings();
    }

    private void ApplyReaderSettings(bool resetHorizontalPosition = true)
    {
        var isHorizontal = settings.Direction != ReadingDirection.TopToBottom;
        Pages.ItemsLayout = new LinearItemsLayout(isHorizontal ? ItemsLayoutOrientation.Horizontal : ItemsLayoutOrientation.Vertical)
        {
            ItemSpacing = settings.ShowPageGap ? 8 : 0
        };
        Pages.FlowDirection = settings.Direction == ReadingDirection.RightToLeft ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;
        var pageWidth = settings.FitMode == PageFitMode.FillWidth && Pages.Width > 0 ? Pages.Width : -1;
        var pageHeight = settings.FitMode == PageFitMode.FillHeight && Pages.Height > 0 ? Math.Max(1, Pages.Height - 12) : -1;
        foreach (var page in pages)
        {
            page.Aspect = Aspect.AspectFit;
            page.PageHeight = pageHeight > 0 ? pageHeight * zoomScale : -1;
            page.PageWidth = pageWidth > 0 ? pageWidth * zoomScale : -1;
            page.ZoomScale = zoomScale;
            page.PanOrientation = isHorizontal ? ScrollOrientation.Neither : ScrollOrientation.Horizontal;
        }
        if (resetHorizontalPosition && isHorizontal && pages.Count > 0)
            Pages.ScrollTo(0, position: ScrollToPosition.Start, animate: false);
    }

    private void OnPagesSizeChanged(object? sender, EventArgs e) => ApplyReaderSettings();

    private void SaveSettings()
    {
        var appSettings = SettingsService.Load();
        appSettings.Reader = settings;
        SettingsService.Save(appSettings);
    }

    private void OnSettingsClicked(object sender, EventArgs e) => SettingsPanel.IsVisible = !SettingsPanel.IsVisible;
    private void OnFitChanged(object sender, EventArgs e)
    {
        if (FitPicker.SelectedIndex < 0) return;
        settings.FitMode = (PageFitMode)FitPicker.SelectedIndex;
        SaveSettings();
        ApplyReaderSettings();
    }
    private void OnDirectionChanged(object sender, EventArgs e)
    {
        if (DirectionPicker.SelectedIndex < 0) return;
        settings.Direction = (ReadingDirection)DirectionPicker.SelectedIndex;
        SaveSettings();
        ApplyReaderSettings();
    }
    private void OnPageGapToggled(object sender, ToggledEventArgs e)
    {
        settings.ShowPageGap = e.Value;
        SaveSettings();
        ApplyReaderSettings();
    }
    private void OnZoomInClicked(object sender, EventArgs e) => SetZoom(zoomScale + 0.25);
    private void OnZoomOutClicked(object sender, EventArgs e) => SetZoom(zoomScale - 0.25);
    private void OnResetZoomClicked(object sender, EventArgs e) => SetZoom(1);
    private void SetZoom(double value)
    {
        zoomScale = Math.Clamp(value, 0.5, 3);
        ZoomButton.Text = $"{zoomScale:P0}";
        ApplyReaderSettings(resetHorizontalPosition: false);
    }
    private void OnPagePinchUpdated(object sender, PinchGestureUpdatedEventArgs e)
    {
        if (e.Status == GestureStatus.Started) pinchStartZoom = zoomScale;
        if (e.Status == GestureStatus.Running) SetZoom(pinchStartZoom * e.Scale);
    }
}
