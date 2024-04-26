using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using AssemblyVisualizer.HAL;
using AssemblyVisualizer.Infrastructure;
using AssemblyVisualizer.Model;

namespace AssemblyVisualizer.InteractionBrowser;

internal class SelectionWindowViewModel : ViewModelBase
{
	private const int HeightOverhead = 62;

	private const int WidthOverhead = 44;

	private const int OverheadPerPiece = 10;

	private const int WindowsInRow = 3;

	private readonly bool _drawGraph;

	private int _height;

	private int _pieceHeight;

	private int _pieceWidth;

	private IEnumerable<ThumbnailViewModel> _thumbnails;

	private readonly IEnumerable<TypeInfo> _types;

	private int _width;

	private readonly SelectionWindow _window;

	public ICommand CancelCommand { get; private set; }

	public IEnumerable<ThumbnailViewModel> Thumbnails
	{
		get
		{
			return _thumbnails;
		}
		private set
		{
			_thumbnails = value;
			OnPropertyChanged("Thumbnails");
		}
	}

	public int Height
	{
		get
		{
			return _height;
		}
		set
		{
			_height = value;
			OnPropertyChanged("Height");
		}
	}

	public int Width
	{
		get
		{
			return _width;
		}
		set
		{
			_width = value;
			OnPropertyChanged("Width");
		}
	}

	public int PieceHeight
	{
		get
		{
			return _pieceHeight;
		}
		set
		{
			_pieceHeight = value;
			OnPropertyChanged("PieceHeight");
		}
	}

	public int PieceWidth
	{
		get
		{
			return _pieceWidth;
		}
		set
		{
			_pieceWidth = value;
			OnPropertyChanged("PieceWidth");
		}
	}

	public SelectionWindowViewModel(IEnumerable<TypeInfo> types, bool drawGraph, SelectionWindow window)
	{
		_window = window;
		_types = types;
		_drawGraph = drawGraph;
		_pieceHeight = 250;
		_pieceWidth = 250;
		Refresh();
		CancelCommand = new DelegateCommand(CancelCommandHandler);
	}

	public void Refresh()
	{
		List<ThumbnailViewModel> list = WindowManager.InteractionBrowsers.Select((InteractionBrowserWindow ib) => new ThumbnailViewModel(ib.Thumbnail, ib.ThumbnailTooltip, delegate
		{
			_window.Close();
			ib.AddTypes(_types, _drawGraph);
		})).ToList();
		ThumbnailViewModel item = new ThumbnailViewModel(null, null, delegate
		{
			_window.Close();
			Services.BrowseInteractions(_types, _drawGraph, passSelection: true);
		});
		list.Insert(0, item);
		Thumbnails = list;
		_window.Width = Math.Min(3, list.Count) * (PieceWidth + 10) + 44;
		int num = list.Count / 3;
		if (list.Count % 3 != 0)
		{
			num++;
		}
		if (num > 3)
		{
			num = 3;
		}
		_window.Height = num * (PieceHeight + 10) + 62;
	}

	private void CancelCommandHandler()
	{
		_window.Close();
	}
}
