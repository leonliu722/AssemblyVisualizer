// Copyright 2011 Denis Markelov
// This code is distributed under Microsoft Public License 
// (for details please see \docs\Ms-PL)



using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.ILSpy;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.TreeView;
using AssemblyVisualizer.Properties;
using AssemblyVisualizer.Model;
using Mono.Cecil;

namespace AssemblyVisualizer.HAL.ILSpy
{
	[ExportContextMenuEntry(Header = "Add to Browser")]
	sealed class AddAssemblyContextMenuEntry : IContextMenuEntry
	{
		public bool IsVisible(TextViewContext context)
		{
			if (WindowManager.AssemblyBrowsers.Count != 1)
			{
				return false;
			}

			var window = WindowManager.AssemblyBrowsers.Single();
			if (!window.ViewModel.Screen.AllowAssemblyDrop)
			{
				return false;
			}

			return context.SelectedTreeNodes.All(n => n is AssemblyTreeNode);
		}

		public bool IsEnabled(TextViewContext context)
		{
			return true;
		}

		public void Execute(TextViewContext context)
		{
			var assemblyDefinitions = context.SelectedTreeNodes
				.OfType<AssemblyTreeNode>()
				.Select(n => HAL.Converter.Assembly(AssemblyDefinition.ReadAssembly(n.LoadedAssembly.GetPEFileOrNull().FileName)))
				.ToList();
			var window = WindowManager.AssemblyBrowsers.Single();
			window.ViewModel.AddAssemblies(assemblyDefinitions);
		}
	}
}