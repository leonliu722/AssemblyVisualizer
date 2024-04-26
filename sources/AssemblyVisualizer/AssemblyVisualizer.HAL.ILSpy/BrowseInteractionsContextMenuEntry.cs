// Copyright 2011 Denis Markelov
// This code is distributed under Microsoft Public License 
// (for details please see \docs\Ms-PL)



using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.ILSpy;
using ICSharpCode.TreeView;
using ICSharpCode.ILSpy.TreeNodes;
using AssemblyVisualizer.InteractionBrowser;

namespace AssemblyVisualizer.HAL.ILSpy
{
    [ExportContextMenuEntry(Header = "Browse Interactions")]
    sealed class BrowseInteractionsContextMenuEntry : IContextMenuEntry
    {
        public bool IsVisible(TextViewContext context)
        {
            return (context.SelectedTreeNodes.All(n => n is TypeTreeNode));
        }

        public bool IsEnabled(TextViewContext context)
        {
            return true;
        }

        public void Execute(TextViewContext context)
        {
            var types = context.SelectedTreeNodes
				.OfType<TypeTreeNode>()
                .Select(n => HAL.Converter.Type(n.TypeDefinition))
                .ToArray();            

            var window = new InteractionBrowserWindow(types, true)
            {
                Owner = MainWindow.Instance
            };
            window.Show();
        }
    }
}

