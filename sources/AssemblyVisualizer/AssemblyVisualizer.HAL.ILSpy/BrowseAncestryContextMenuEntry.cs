// Copyright 2011 Denis Markelov
// This code is distributed under Microsoft Public License 
// (for details please see \docs\Ms-PL)



using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using ICSharpCode.ILSpy;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.TreeView;
using AssemblyVisualizer.Properties;
using AssemblyVisualizer.Model;
using AssemblyVisualizer.AncestryBrowser;
using Mono.Cecil;

namespace AssemblyVisualizer.HAL.ILSpy
{
	[ExportContextMenuEntry(Header = "Browse Ancestry")]
	sealed class BrowseAncestryContextMenuEntry : IContextMenuEntry
	{
		public bool IsVisible(TextViewContext context)
		{
			return (context.SelectedTreeNodes.Count() == 1) 
				   && (context.SelectedTreeNodes.Single() is TypeTreeNode);
		}

		public bool IsEnabled(TextViewContext context)
		{
			return true;
		}

		public void Execute(TextViewContext context)
		{
			var typeDefinition = context.SelectedTreeNodes
				.OfType<TypeTreeNode>()
				.Single().TypeDefinition;

			var window = new AncestryBrowserWindow(HAL.Converter.Type(typeDefinition))
			             	{
			             		Owner = MainWindow.Instance
			             	};
			window.Show();
		}
	}
}
