using System;
using System.Windows.Input;
using AssemblyVisualizer.About;
using ICSharpCode.ILSpy;

namespace AssemblyVisualizer.HAL.ILSpy;

[ExportMainMenuCommand(Menu = "_Visualizer", Header = "_About", MenuOrder = 1.5)]
public class ShowAboutCommand : ICommand
{
	public event EventHandler CanExecuteChanged;

	public bool CanExecute(object parameter)
	{
		return true;
	}

	public void Execute(object parameter)
	{
		AboutWindow aboutWindow = new AboutWindow();
		aboutWindow.Show();
	}
}
