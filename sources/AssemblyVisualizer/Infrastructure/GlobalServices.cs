using System.Diagnostics;

namespace AssemblyVisualizer.Infrastructure;

internal static class GlobalServices
{
	private const string SourcesUrl = "https://github.com/denismarkelov/AssemblyVisualizer";

	public static void NavigateToSources()
	{
		Process.Start("https://github.com/denismarkelov/AssemblyVisualizer");
	}
}
