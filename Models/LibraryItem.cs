namespace mreader.Models;

public enum LibraryItemKind
{
    Folder,
    MangaDirectory,
    MangaArchive
}

public sealed class LibraryItem : System.ComponentModel.INotifyPropertyChanged
{
    private double cardWidth = 140;
    public string Name { get; init; } = string.Empty;
    public string FullPath { get; init; } = string.Empty;
    public string? CoverPath { get; init; }
    public LibraryItemKind Kind { get; init; }

    public bool IsFolder => Kind == LibraryItemKind.Folder;
    public bool IsManga => !IsFolder;
    public string KindLabel => IsFolder ? "Folder" : Kind == LibraryItemKind.MangaArchive ? "ZIP archive" : "Manga folder";
    public double CardWidth
    {
        get => cardWidth;
        private set
        {
            if (Math.Abs(cardWidth - value) < 0.1) return;
            cardWidth = value;
            PropertyChanged?.Invoke(this, new(nameof(CardWidth)));
            PropertyChanged?.Invoke(this, new(nameof(CardHeight)));
        }
    }
    public double CardHeight => Math.Round(CardWidth * 1.4);

    public event System.ComponentModel.PropertyChangedEventHandler? PropertyChanged;
    public void SetCardWidth(double width) => CardWidth = width;
}
