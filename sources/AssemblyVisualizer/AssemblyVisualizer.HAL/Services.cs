using System.Collections.Generic;
using AssemblyVisualizer.AncestryBrowser;
using AssemblyVisualizer.AssemblyBrowser;
using AssemblyVisualizer.InteractionBrowser;
using AssemblyVisualizer.Model;
using ICSharpCode.ILSpy;
using Mono.Cecil;

namespace AssemblyVisualizer.HAL;

internal class Services
{
	public static MainWindow MainWindow => MainWindow.Instance;

	public static void BrowseAssemblies(IEnumerable<AssemblyInfo> assemblies)
	{
		AssemblyBrowserWindow assemblyBrowserWindow = new AssemblyBrowserWindow(assemblies);
		assemblyBrowserWindow.Owner = MainWindow;
		assemblyBrowserWindow.Show();
	}

	public static void BrowseInteractions(IEnumerable<TypeInfo> types, bool drawGraph)
	{
		BrowseInteractions(types, drawGraph, passSelection: false);
	}

	public static void BrowseInteractions(IEnumerable<TypeInfo> types, bool drawGraph, bool passSelection)
	{
		if (WindowManager.InteractionBrowsers.Count > 0 && !passSelection)
		{
			SelectionWindow selectionWindow = new SelectionWindow(types, drawGraph);
			selectionWindow.Owner = MainWindow;
			selectionWindow.Show();
		}
		else
		{
			InteractionBrowserWindow interactionBrowserWindow = new InteractionBrowserWindow(types, drawGraph);
			interactionBrowserWindow.Owner = MainWindow;
			interactionBrowserWindow.Show();
		}
	}

	public static void BrowseAncestry(TypeInfo type)
	{
		AncestryBrowserWindow ancestryBrowserWindow = new AncestryBrowserWindow(type);
		ancestryBrowserWindow.Owner = MainWindow;
		ancestryBrowserWindow.Show();
	}

	public static void JumpTo(object memberReference)
	{
		MainWindow.JumpToReference(memberReference);
	}

	public static bool MethodsMatch(MethodInfo method1, MethodInfo method2)
	{
		MethodDefinition methodDefinition = method1.MemberReference as MethodDefinition;
		MethodDefinition methodDefinition2 = method2.MemberReference as MethodDefinition;
		return methodDefinition.Name == methodDefinition2.Name && ParametersMatch(methodDefinition, methodDefinition2);
	}

	private static bool ParametersMatch(MethodDefinition method1, MethodDefinition method2)
	{
		if (method1.Parameters.Count != method2.Parameters.Count)
		{
			return false;
		}
		for (int i = 0; i < method1.Parameters.Count; i++)
		{
			if (method1.Parameters[i].ParameterType.FullName != method2.Parameters[i].ParameterType.FullName)
			{
				return false;
			}
		}
		return true;
	}
}
