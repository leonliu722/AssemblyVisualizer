using System;
using System.Collections.Generic;
using AssemblyVisualizer.AncestryBrowser;
using AssemblyVisualizer.AssemblyBrowser;
using AssemblyVisualizer.DependencyBrowser;
using AssemblyVisualizer.HAL;
using AssemblyVisualizer.InteractionBrowser;

namespace AssemblyVisualizer;

internal static class WindowManager
{
	private static readonly IList<AssemblyBrowserWindow> _assemblyBrowsers = new List<AssemblyBrowserWindow>();

	private static readonly IList<AncestryBrowserWindow> _ancestryBrowsers = new List<AncestryBrowserWindow>();

	private static readonly IList<DependencyBrowserWindow> _dependencyBrowsers = new List<DependencyBrowserWindow>();

	private static readonly IList<InteractionBrowserWindow> _interactionBrowsers = new List<InteractionBrowserWindow>();

	public static IList<AssemblyBrowserWindow> AssemblyBrowsers => _assemblyBrowsers;

	public static IList<AncestryBrowserWindow> AncestryBrowsers => _ancestryBrowsers;

	public static IList<DependencyBrowserWindow> DependencyBrowsers => _dependencyBrowsers;

	public static IList<InteractionBrowserWindow> InteractionBrowsers => _interactionBrowsers;

	public static event Action InteractionBrowsersChanged;

	public static void AddAssemblyBrowser(AssemblyBrowserWindow window)
	{
		_assemblyBrowsers.Add(window);
	}

	public static void RemoveAssemblyBrowser(AssemblyBrowserWindow window)
	{
		_assemblyBrowsers.Remove(window);
		ClearCacheIfPossible();
		GC.Collect();
	}

	public static void AddAncestryBrowser(AncestryBrowserWindow window)
	{
		_ancestryBrowsers.Add(window);
	}

	public static void RemoveAncestryBrowser(AncestryBrowserWindow window)
	{
		_ancestryBrowsers.Remove(window);
		ClearCacheIfPossible();
		GC.Collect();
	}

	public static void AddDependencyBrowser(DependencyBrowserWindow window)
	{
		_dependencyBrowsers.Add(window);
	}

	public static void RemoveDependencyBrowser(DependencyBrowserWindow window)
	{
		_dependencyBrowsers.Remove(window);
		ClearCacheIfPossible();
		GC.Collect();
	}

	public static void AddInteractionBrowser(InteractionBrowserWindow window)
	{
		_interactionBrowsers.Add(window);
		OnInteractionBrowsersChanged();
	}

	public static void RemoveInteractionBrowser(InteractionBrowserWindow window)
	{
		_interactionBrowsers.Remove(window);
		ClearCacheIfPossible();
		GC.Collect();
		OnInteractionBrowsersChanged();
	}

	private static void ClearCacheIfPossible()
	{
		if (AssemblyBrowsers.Count == 0 && AncestryBrowsers.Count == 0 && DependencyBrowsers.Count == 0 && InteractionBrowsers.Count == 0)
		{
			Converter.ClearCache();
		}
	}

	private static void OnInteractionBrowsersChanged()
	{
		WindowManager.InteractionBrowsersChanged?.Invoke();
	}
}
