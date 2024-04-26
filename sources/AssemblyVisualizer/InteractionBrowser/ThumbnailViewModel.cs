using System;
using System.Windows.Input;
using System.Windows.Media;
using AssemblyVisualizer.Infrastructure;

namespace AssemblyVisualizer.InteractionBrowser;

internal class ThumbnailViewModel : ViewModelBase
{
	public Visual Thumbnail { get; private set; }

	public string Tooltip { get; private set; }

	public ICommand SelectCommand { get; private set; }

	public bool IsNewWindow => Thumbnail == null;

	public ThumbnailViewModel(Visual thumbnail, string tooltip, Action selectAction)
	{
		Thumbnail = thumbnail;
		Tooltip = tooltip;
		SelectCommand = new DelegateCommand(selectAction);
	}
}
