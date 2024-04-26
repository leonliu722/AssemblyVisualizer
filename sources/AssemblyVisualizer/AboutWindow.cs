using System.Reflection;
using System.Windows;
using System.Windows.Markup;
using AssemblyVisualizer.Infrastructure;

namespace AssemblyVisualizer.About;

internal partial class AboutWindow : Window, IComponentConnector
{
	public AboutWindow()
	{
		InitializeComponent();
		string text = Assembly.GetExecutingAssembly().GetName().Version.ToString();
		txtVersion.Text = text;
		escapeBinding.Command = new DelegateCommand(base.Close);
	}

	private void SourcesClickHandler(object sender, RoutedEventArgs e)
	{
		GlobalServices.NavigateToSources();
	}
}
