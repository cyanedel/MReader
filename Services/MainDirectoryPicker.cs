namespace mreader.Services;

public static class MainDirectoryPicker
{
    public static async Task<string?> PickAsync(Window window)
    {
#if WINDOWS
        var picker = new Windows.Storage.Pickers.FolderPicker();
        picker.FileTypeFilter.Add("*");
        var nativeWindow = window.Handler?.PlatformView as Microsoft.UI.Xaml.Window;
        if (nativeWindow is null)
            return null;

        WinRT.Interop.InitializeWithWindow.Initialize(picker, WinRT.Interop.WindowNative.GetWindowHandle(nativeWindow));
        var folder = await picker.PickSingleFolderAsync();
        return folder?.Path;
#else
        await Task.CompletedTask;
        return null;
#endif
    }
}
