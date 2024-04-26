﻿// Copyright 2011 Denis Markelov
// This code is distributed under Microsoft Public License 
// (for details please see \docs\Ms-PL)



using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using ICSharpCode.ILSpy;
using ICSharpCode.ILSpy.TreeNodes;
using AssemblyVisualizer.InteractionBrowser;
using System.Windows;
using Mono.Cecil;
using ICSharpCode.Decompiler.TypeSystem;

namespace AssemblyVisualizer.HAL.ILSpy
{
    [ExportToolbarCommand(ToolTip = "Opens Interaction Browser in type selection mode", ToolbarIcon = "images/ib2.png", ToolbarCategory = "AssemblyVisualizer", ToolbarOrder = 1)]
    public class BrowseInteractionsToolbarCommand : ICommand
    {
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            var selectedNodes = MainWindow.Instance.SelectedNodes;

            var types = selectedNodes
                .OfType<TypeTreeNode>()
                .Select(n => HAL.Converter.Type(new TypeDefinition(n.TypeDefinition.Namespace, n.TypeDefinition.Name, (TypeAttributes)n.TypeDefinition.Attributes)))
                .ToArray();
			if (types.Length == 0)
            {
                MessageBox.Show("Please select one or more types in the tree.", "Assembly Visualizer");
                return;
            }

            var window = new InteractionBrowserWindow(types, false)
            {
                Owner = MainWindow.Instance
            };
            window.Show();
        }
		public static TypeAttributes ConvertAttributesByString(string typeDefinitionString)
		{
			var temp = uint.Parse(typeDefinitionString.Split(' ')[0]);
			return (TypeAttributes)temp;
		}
	}
}

