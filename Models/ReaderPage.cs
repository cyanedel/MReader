namespace mreader.Models;

public sealed class ReaderPage : System.ComponentModel.INotifyPropertyChanged
{
	private double zoomScale = 1;
	private double pageHeight = -1;
	private double pageWidth = -1;
	private Aspect aspect = Aspect.AspectFit;
	private ScrollOrientation panOrientation = ScrollOrientation.Horizontal;
    public string Name { get; init; } = string.Empty;
    public ImageSource Source { get; init; } = null!;
	public double ZoomScale
	{
		get => zoomScale;
		set { if (Math.Abs(zoomScale - value) > 0.001) { zoomScale = value; OnPropertyChanged(nameof(ZoomScale)); } }
	}
	public double PageHeight
	{
		get => pageHeight;
		set { if (Math.Abs(pageHeight - value) > 0.1) { pageHeight = value; OnPropertyChanged(nameof(PageHeight)); } }
	}
	public double PageWidth
	{
		get => pageWidth;
		set { if (Math.Abs(pageWidth - value) > 0.1) { pageWidth = value; OnPropertyChanged(nameof(PageWidth)); } }
	}
	public Aspect Aspect
	{
		get => aspect;
		set { if (aspect != value) { aspect = value; OnPropertyChanged(nameof(Aspect)); } }
	}
	public ScrollOrientation PanOrientation
	{
		get => panOrientation;
		set { if (panOrientation != value) { panOrientation = value; OnPropertyChanged(nameof(PanOrientation)); } }
	}

	public event System.ComponentModel.PropertyChangedEventHandler? PropertyChanged;
	private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new(propertyName));
}
