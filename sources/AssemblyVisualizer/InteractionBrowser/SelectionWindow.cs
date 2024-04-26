using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media.Animation;
using AssemblyVisualizer.Model;

namespace AssemblyVisualizer.InteractionBrowser;

internal partial class SelectionWindow : Window, IComponentConnector
{
	public SelectionWindowViewModel ViewModel
	{
		get
		{
			return base.DataContext as SelectionWindowViewModel;
		}
		set
		{
			base.DataContext = value;
		}
	}

	public SelectionWindow(IEnumerable<TypeInfo> types, bool drawGraph)
	{
		InitializeComponent();
		base.DataContext = new SelectionWindowViewModel(types, drawGraph, this);
		WindowManager.InteractionBrowsersChanged += InteractionBrowsersChangedHandler;
		DoubleAnimation animation = new DoubleAnimation(1.0, new Duration(TimeSpan.FromSeconds(0.5)));
		brd.BeginAnimation(UIElement.OpacityProperty, animation);
	}

	private void InteractionBrowsersChangedHandler()
	{
		ViewModel.Refresh();
	}

	protected override void OnClosing(CancelEventArgs e)
	{
		base.OnClosing(e);
		WindowManager.InteractionBrowsersChanged -= InteractionBrowsersChangedHandler;
	}
}
