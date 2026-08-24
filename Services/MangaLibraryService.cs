using System.IO.Compression;
using mreader.Helpers;
using mreader.Models;

namespace mreader.Services;

public static class MangaLibraryService
{
    public static IReadOnlyList<LibraryItem> GetItems(string directoryPath)
    {
        if (!Directory.Exists(directoryPath))
            return [];

        var items = new List<LibraryItem>();
        foreach (var directory in Directory.EnumerateDirectories(directoryPath).OrderBy(Path.GetFileName, StringComparer.OrdinalIgnoreCase))
        {
            var cover = FindFirstImage(directory);
            items.Add(new LibraryItem
            {
                Name = Path.GetFileName(directory),
                FullPath = directory,
                CoverPath = cover,
                Kind = cover is null ? LibraryItemKind.Folder : LibraryItemKind.MangaDirectory
            });
        }

        foreach (var archive in Directory.EnumerateFiles(directoryPath, "*.zip").OrderBy(Path.GetFileName, StringComparer.OrdinalIgnoreCase))
        {
            if (ContainsImage(archive))
            {
                items.Add(new LibraryItem
                {
                    Name = Path.GetFileNameWithoutExtension(archive),
                    FullPath = archive,
                    Kind = LibraryItemKind.MangaArchive
                });
            }
        }

        return items;
    }

    public static IReadOnlyList<ReaderPage> GetPages(LibraryItem manga)
    {
        if (manga.Kind == LibraryItemKind.MangaDirectory)
        {
            return ImageLoader.ExtractImageFromDir(manga.FullPath)
                .Select(file => new ReaderPage { Name = file.Name, Source = ImageSource.FromFile(file.FullName) })
                .ToList();
        }

        if (manga.Kind == LibraryItemKind.MangaArchive)
        {
            return ImageLoader.ExtractImageFromZip(manga.FullPath)
                .Select(image => new ReaderPage
                {
                    Name = Path.GetFileName(image.name),
                    Source = ImageSource.FromStream(() => new MemoryStream(image.imageData, writable: false))
                })
                .ToList();
        }

        return [];
    }

    private static string? FindFirstImage(string directoryPath) => Directory.EnumerateFiles(directoryPath)
        .Where(Generic.IsImageFile)
        .OrderBy(path => Generic.ExtractNumber(Path.GetFileName(path)))
        .FirstOrDefault();

    private static bool ContainsImage(string archivePath)
    {
        try
        {
            using var archive = ZipFile.OpenRead(archivePath);
            return archive.Entries.Any(entry => !string.IsNullOrEmpty(entry.Name) && Generic.IsImageFile(entry.Name));
        }
        catch (InvalidDataException)
        {
            return false;
        }
        catch (UnauthorizedAccessException)
        {
            return false;
        }
    }
}
