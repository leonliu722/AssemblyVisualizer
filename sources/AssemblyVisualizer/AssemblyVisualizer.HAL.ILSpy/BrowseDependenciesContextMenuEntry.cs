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
using AssemblyVisualizer.DependencyBrowser;
using Mono.Cecil;

namespace AssemblyVisualizer.HAL.ILSpy
{
    [ExportContextMenuEntry(Header = "Browse Dependencies")]
    sealed class BrowseDependenciesContextMenuEntry : IContextMenuEntry
    {
        public bool IsVisible(TextViewContext context)
        {
            return context.SelectedTreeNodes.OfType<AssemblyTreeNode>().Count() > 0;                   
        }

        public bool IsEnabled(TextViewContext context)
        {
            return true;
        }

        public void Execute(TextViewContext context)
        {
            var assemblyDefinitions = context.SelectedTreeNodes
				.OfType<AssemblyTreeNode>()
                .Select(n=>AssemblyDefinition.ReadAssembly(n.LoadedAssembly.GetPEFileOrNull().FileName));

            var window = new DependencyBrowserWindow(assemblyDefinitions.Select(HAL.Converter.Assembly))
            {
                Owner = Services.MainWindow
            };
            window.Show();
        }
    }
}
