using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using AssemblyVisualizer.Common;
using AssemblyVisualizer.Controls.Graph.QuickGraph;
using AssemblyVisualizer.HAL;
using AssemblyVisualizer.Infrastructure;
using AssemblyVisualizer.Model;
using AssemblyVisualizer.Properties;

namespace AssemblyVisualizer.InteractionBrowser;

internal class InteractionBrowserWindowViewModel : ViewModelBase
{
	private static readonly string[] WpfInternalTypes = new string[4] { "DependencyObject", "Visual", "UIElement", "FrameworkElement" };

	private static readonly string[] WpfAssemblies = new string[3] { "WindowsBase", "PresentationFramework", "PresentationCore" };

	private MemberGraph _graph;

	private IEnumerable<HierarchyViewModel> _hierarchies;

	private bool _isSearchVisible;

	private bool _isTypeListVisible = true;

	private bool _isTypeSelectionVisible;

	private string _searchTerm;

	private bool _showStaticConstructors;

	private bool _showUnconnectedVertices;

	private IEnumerable<TypeInfo> _types;

	private readonly IDictionary<TypeInfo, TypeViewModel> _viewModelCorrespondence = new Dictionary<TypeInfo, TypeViewModel>();

	private readonly Dictionary<MemberInfo, MemberViewModel> _viewModelsDictionary = new Dictionary<MemberInfo, MemberViewModel>();

	public IEnumerable<UserCommand> Commands { get; private set; }

	public ICommand ApplySelectionCommand { get; private set; }

	public ICommand ShowSelectionViewCommand { get; private set; }

	public ICommand HideSelectionViewCommand { get; private set; }

	public ICommand ToggleSelectionViewCommand { get; private set; }

	public ICommand ToggleTypeListVisibilityCommand { get; private set; }

	public ICommand ShowSearchCommand { get; private set; }

	public ICommand HideSearchCommand { get; private set; }

	public IEnumerable<HierarchyViewModel> Hierarchies
	{
		get
		{
			return _hierarchies;
		}
		set
		{
			_hierarchies = value;
			OnPropertyChanged("Hierarchies");
		}
	}

	public IEnumerable<TypeViewModel> DisplayedTypes => _hierarchies.SelectMany((HierarchyViewModel h) => h.Types.Where((TypeViewModel t) => t.IsSelected)).Distinct().ToArray();

	public MemberGraph Graph
	{
		get
		{
			return _graph;
		}
		set
		{
			_graph = value;
			OnPropertyChanged("Graph");
		}
	}

	public bool ContainsWpfInternals => _hierarchies.Any((HierarchyViewModel h) => h.Types.Any(IsWpfInternalType));

	public bool ShowStaticConstructors
	{
		get
		{
			return _showStaticConstructors;
		}
		set
		{
			_showStaticConstructors = value;
			OnPropertyChanged("ShowStaticConstructors");
			ReportSelectionChanged();
		}
	}

	public string SearchTerm
	{
		get
		{
			return _searchTerm;
		}
		set
		{
			_searchTerm = value;
			OnPropertyChanged("SearchTerm");
			PerformSearch();
		}
	}

	public bool IsSearchVisible
	{
		get
		{
			return _isSearchVisible;
		}
		set
		{
			_isSearchVisible = value;
			OnPropertyChanged("IsSearchVisible");
		}
	}

	public bool IsTypeListVisible
	{
		get
		{
			return _isTypeListVisible;
		}
		set
		{
			_isTypeListVisible = value;
			OnPropertyChanged("IsTypeListVisible");
		}
	}

	public int MembersCount
	{
		get
		{
			MemberGraph memberGraph = CreateGraph(DisplayedTypes);
			return memberGraph.Vertices.Count();
		}
	}

	public bool IsTypeSelectionVisible
	{
		get
		{
			return _isTypeSelectionVisible;
		}
		set
		{
			_isTypeSelectionVisible = value;
			OnPropertyChanged("IsTypeSelectionVisible");
		}
	}

	public bool ShowUnconnectedVertices
	{
		get
		{
			return _showUnconnectedVertices;
		}
		set
		{
			_showUnconnectedVertices = value;
			OnPropertyChanged("ShowUnconnectedVertices");
			ReportSelectionChanged();
		}
	}

	public string Title => Resources.InteractionBrowser;

	public event Action FillGraphRequest;

	public event Action OriginalSizeRequest;

	public event Action FocusSearchRequest;

	public InteractionBrowserWindowViewModel(IEnumerable<TypeInfo> types, bool drawGraph)
	{
		ApplySelectionCommand = new DelegateCommand(ApplySelectionCommandHandler);
		ShowSelectionViewCommand = new DelegateCommand(ShowSelectionViewCommandHandler);
		HideSelectionViewCommand = new DelegateCommand(HideSelectionViewCommandHandler);
		ToggleSelectionViewCommand = new DelegateCommand(ToggleSelectionViewCommandHandler);
		ToggleTypeListVisibilityCommand = new DelegateCommand(ToggleTypeListVisibilityCommandHandler);
		ShowSearchCommand = new DelegateCommand(ShowSearchCommandHandler);
		HideSearchCommand = new DelegateCommand(HideSearchCommandHandler);
		Commands = new ObservableCollection<UserCommand>
		{
			new UserCommand(Resources.FillGraph, OnFillGraphRequest),
			new UserCommand(Resources.OriginalSize, OnOriginalSizeRequest),
			new UserCommand(Resources.SelectTypes, ShowSelectionViewCommand),
			new UserCommand(Resources.SearchInGraph, ShowSearchCommand)
		};
		AddTypes(types, drawGraph);
	}

	public void AddTypes(IEnumerable<TypeInfo> types, bool drawGraph)
	{
		if (_types != null && types.All((TypeInfo t) => _types.Contains(t)))
		{
			return;
		}
		_types = ((_types == null) ? types : _types.Concat(types).Distinct().ToList());
		Hierarchies = _types.Select(GetHierarchy).ToArray();
		foreach (HierarchyViewModel hierarchy in _hierarchies)
		{
			hierarchy.AllSelected = true;
			HideWpfInternals(hierarchy);
		}
		_showStaticConstructors = !ContainsWpfInternals;
		if (drawGraph)
		{
			DrawGraph();
		}
		else
		{
			IsTypeSelectionVisible = true;
		}
		OnPropertyChanged("ContainsWpfInternals");
		OnPropertyChanged("ShowStaticConstructors");
	}

	public void ReportSelectionChanged()
	{
		OnPropertyChanged("MembersCount");
	}

	private void PerformSearch()
	{
		if (string.IsNullOrEmpty(SearchTerm) || string.IsNullOrEmpty(SearchTerm.Trim()))
		{
			ClearSearch();
			return;
		}
		foreach (MemberViewModel value in _viewModelsDictionary.Values)
		{
			value.IsMarked = value.MemberInfo.Name.IndexOf(SearchTerm, StringComparison.InvariantCultureIgnoreCase) >= 0;
		}
	}

	private void ClearSearch()
	{
		foreach (MemberViewModel value in _viewModelsDictionary.Values)
		{
			value.IsMarked = false;
		}
	}

	private void HideWpfInternals(HierarchyViewModel hierarchy)
	{
		TypeViewModel typeViewModel = hierarchy.Types.First();
		foreach (TypeViewModel type in hierarchy.Types)
		{
			if (type != typeViewModel && IsWpfInternalType(type))
			{
				type.IsSelected = false;
			}
		}
	}

	private bool IsWpfInternalType(TypeViewModel type)
	{
		return WpfInternalTypes.Contains(type.Name) && WpfAssemblies.Contains(type.TypeInfo.Module.Assembly.Name);
	}

	private TypeViewModel GetViewModelForType(TypeInfo typeInfo)
	{
		if (_viewModelCorrespondence.ContainsKey(typeInfo))
		{
			return _viewModelCorrespondence[typeInfo];
		}
		TypeViewModel typeViewModel = new TypeViewModel(typeInfo, this);
		_viewModelCorrespondence.Add(typeInfo, typeViewModel);
		return typeViewModel;
	}

	private HierarchyViewModel GetHierarchy(TypeInfo typeInfo)
	{
		List<TypeInfo> list = new List<TypeInfo>();
		TypeInfo typeInfo2 = typeInfo;
		list.Add(typeInfo);
		while (typeInfo2.BaseType != null)
		{
			TypeInfo baseType = typeInfo2.BaseType;
			list.Add(baseType);
			typeInfo2 = baseType;
		}
		return new HierarchyViewModel(list.Select(GetViewModelForType).ToArray());
	}

	private MemberGraph CreateGraph(IEnumerable<TypeViewModel> typeViewModels)
	{
		MemberGraph graph = new MemberGraph(allowParallelEdges: true);
		TypeInfo[] types = typeViewModels.Select((TypeViewModel vm) => vm.TypeInfo).ToArray();
		foreach (TypeViewModel typeViewModel in typeViewModels)
		{
			TypeInfo typeInfo = typeViewModel.TypeInfo;
			IEnumerable<MethodInfo> enumerable = typeInfo.Methods.Where((MethodInfo m) => !m.Name.StartsWith("<")).Concat(typeInfo.Accessors);
			if (!typeViewModel.ShowInternals)
			{
				enumerable = enumerable.Where((MethodInfo m) => m.IsVisibleOutside());
			}
			if (!ShowStaticConstructors)
			{
				enumerable = enumerable.Where((MethodInfo m) => m.Name != ".cctor");
			}
			foreach (MethodInfo item in enumerable)
			{
				MemberViewModel viewModelForMethod = GetViewModelForMethod(item);
				if (!graph.ContainsVertex(viewModelForMethod))
				{
					graph.AddVertex(viewModelForMethod);
				}
				MethodInfo[] methods = Helper.GetUsedMethods(item.MemberReference).ToArray();
				MethodInfo[] array = (from m in FixDeclaringTypes(methods, types)
					where typeViewModels.Any((TypeViewModel t) => m.IsVisibleOutside() || t.ShowInternals) && !m.Name.StartsWith("<")
					select m).ToArray();
				MethodInfo[] array2 = array;
				foreach (MethodInfo methodInfo in array2)
				{
					MemberViewModel viewModelForMethod2 = GetViewModelForMethod(methodInfo);
					if (!graph.ContainsVertex(viewModelForMethod2))
					{
						graph.AddVertex(viewModelForMethod2);
					}
					graph.AddEdge(new Edge<MemberViewModel>(viewModelForMethod, viewModelForMethod2));
				}
				FieldInfo[] fields = Helper.GetUsedFields(item.MemberReference).ToArray();
				FieldInfo[] array3 = (from m in FixDeclaringTypes(fields, types)
					where typeViewModels.Any((TypeViewModel tvm) => m.IsVisibleOutside() || tvm.ShowInternals) && !m.Name.StartsWith("CS$") && !m.Name.EndsWith("k__BackingField")
					select m).ToArray();
				FieldInfo[] array4 = array3;
				foreach (FieldInfo fieldInfo in array4)
				{
					MemberViewModel viewModelForMethod2 = GetViewModelForField(fieldInfo);
					if (!graph.ContainsVertex(viewModelForMethod2))
					{
						graph.AddVertex(viewModelForMethod2);
					}
					graph.AddEdge(new Edge<MemberViewModel>(viewModelForMethod, viewModelForMethod2));
				}
			}
		}
		if (!ShowUnconnectedVertices)
		{
			graph.RemoveVertexIf((MemberViewModel v) => graph.Degree(v) == 0);
		}
		return graph;
	}

	private IEnumerable<MethodInfo> FixDeclaringTypes(MethodInfo[] methods, TypeInfo[] types)
	{
		List<MethodInfo> list = new List<MethodInfo>();
		string[] source = types.Select((TypeInfo t) => t.FullName).ToArray();
		foreach (MethodInfo method in methods)
		{
			if (types.Contains(method.DeclaringType))
			{
				list.Add(method);
			}
			else
			{
				if (!source.Contains(method.DeclaringType.FullName))
				{
					continue;
				}
				TypeInfo typeInfo = types.Single((TypeInfo t) => t.FullName == method.DeclaringType.FullName);
				MethodInfo methodInfo = typeInfo.Methods.SingleOrDefault((MethodInfo m) => m.MemberReference.ToString() == method.MemberReference.ToString());
				if (methodInfo == null)
				{
					methodInfo = typeInfo.Accessors.Single((MethodInfo m) => m.MemberReference.ToString() == method.MemberReference.ToString());
				}
				list.Add(methodInfo);
			}
		}
		return list;
	}

	private IEnumerable<FieldInfo> FixDeclaringTypes(FieldInfo[] fields, TypeInfo[] types)
	{
		List<FieldInfo> list = new List<FieldInfo>();
		string[] source = types.Select((TypeInfo t) => t.FullName).ToArray();
		foreach (FieldInfo field in fields)
		{
			if (types.Contains(field.DeclaringType))
			{
				list.Add(field);
			}
			else if (source.Contains(field.DeclaringType.FullName))
			{
				TypeInfo typeInfo = types.Single((TypeInfo t) => t.FullName == field.DeclaringType.FullName);
				FieldInfo item = typeInfo.Fields.Single((FieldInfo m) => m.Name == field.Name);
				list.Add(item);
			}
		}
		return list;
	}

	private MemberViewModel GetViewModelForField(FieldInfo fieldInfo)
	{
		if (_viewModelsDictionary.ContainsKey(fieldInfo))
		{
			TypeViewModel viewModelForType = GetViewModelForType(fieldInfo.DeclaringType);
			MemberViewModel memberViewModel = _viewModelsDictionary[fieldInfo];
			memberViewModel.Background = viewModelForType.Background;
			memberViewModel.Foreground = viewModelForType.Foreground;
			return memberViewModel;
		}
		EventInfo eventForBackingField = Helper.GetEventForBackingField(fieldInfo.MemberReference);
		if (eventForBackingField != null)
		{
			TypeViewModel viewModelForType = GetViewModelForType(eventForBackingField.DeclaringType);
			if (_viewModelsDictionary.ContainsKey(eventForBackingField))
			{
				MemberViewModel memberViewModel = _viewModelsDictionary[eventForBackingField];
				memberViewModel.Background = viewModelForType.Background;
				memberViewModel.Foreground = viewModelForType.Foreground;
				return memberViewModel;
			}
			EventViewModel eventViewModel = new EventViewModel(eventForBackingField);
			eventViewModel.Background = viewModelForType.Background;
			eventViewModel.Foreground = viewModelForType.Foreground;
			eventViewModel.ToolTip = viewModelForType.Name;
			EventViewModel eventViewModel2 = eventViewModel;
			_viewModelsDictionary.Add(eventForBackingField, eventViewModel2);
			return eventViewModel2;
		}
		TypeViewModel viewModelForType2 = GetViewModelForType(fieldInfo.DeclaringType);
		FieldViewModel fieldViewModel = new FieldViewModel(fieldInfo);
		fieldViewModel.Background = viewModelForType2.Background;
		fieldViewModel.Foreground = viewModelForType2.Foreground;
		fieldViewModel.ToolTip = viewModelForType2.Name;
		FieldViewModel fieldViewModel2 = fieldViewModel;
		_viewModelsDictionary.Add(fieldInfo, fieldViewModel2);
		return fieldViewModel2;
	}

	private MemberViewModel GetViewModelForMethod(MethodInfo methodInfo)
	{
		if (_viewModelsDictionary.ContainsKey(methodInfo))
		{
			TypeViewModel viewModelForType = GetViewModelForType(methodInfo.DeclaringType);
			MemberViewModel memberViewModel = _viewModelsDictionary[methodInfo];
			memberViewModel.Background = viewModelForType.Background;
			memberViewModel.Foreground = viewModelForType.Foreground;
			return memberViewModel;
		}
		if (IsPropertyAccessor(methodInfo))
		{
			PropertyInfo accessorProperty = Helper.GetAccessorProperty(methodInfo.MemberReference);
			TypeViewModel viewModelForType = GetViewModelForType(accessorProperty.DeclaringType);
			if (_viewModelsDictionary.ContainsKey(accessorProperty))
			{
				MemberViewModel memberViewModel = _viewModelsDictionary[accessorProperty];
				memberViewModel.Background = viewModelForType.Background;
				memberViewModel.Foreground = viewModelForType.Foreground;
				return memberViewModel;
			}
			PropertyViewModel propertyViewModel = new PropertyViewModel(accessorProperty);
			propertyViewModel.Background = viewModelForType.Background;
			propertyViewModel.Foreground = viewModelForType.Foreground;
			propertyViewModel.ToolTip = viewModelForType.Name;
			PropertyViewModel propertyViewModel2 = propertyViewModel;
			_viewModelsDictionary.Add(accessorProperty, propertyViewModel2);
			return propertyViewModel2;
		}
		if (IsEventAccessor(methodInfo))
		{
			EventInfo accessorEvent = Helper.GetAccessorEvent(methodInfo.MemberReference);
			TypeViewModel viewModelForType = GetViewModelForType(accessorEvent.DeclaringType);
			if (_viewModelsDictionary.ContainsKey(accessorEvent))
			{
				MemberViewModel memberViewModel = _viewModelsDictionary[accessorEvent];
				memberViewModel.Background = viewModelForType.Background;
				memberViewModel.Foreground = viewModelForType.Foreground;
				return memberViewModel;
			}
			EventViewModel eventViewModel = new EventViewModel(accessorEvent);
			eventViewModel.Background = viewModelForType.Background;
			eventViewModel.Foreground = viewModelForType.Foreground;
			eventViewModel.ToolTip = viewModelForType.Name;
			EventViewModel eventViewModel2 = eventViewModel;
			_viewModelsDictionary.Add(accessorEvent, eventViewModel2);
			return eventViewModel2;
		}
		TypeViewModel viewModelForType2 = GetViewModelForType(methodInfo.DeclaringType);
		MethodViewModel methodViewModel = new MethodViewModel(methodInfo);
		methodViewModel.Background = viewModelForType2.Background;
		methodViewModel.Foreground = viewModelForType2.Foreground;
		methodViewModel.ToolTip = viewModelForType2.Name;
		MethodViewModel methodViewModel2 = methodViewModel;
		_viewModelsDictionary.Add(methodInfo, methodViewModel2);
		return methodViewModel2;
	}

	private void ApplySelectionCommandHandler()
	{
		if (IsTypeSelectionVisible)
		{
			IsTypeSelectionVisible = false;
			DrawGraph();
		}
	}

	private void DrawGraph()
	{
		IEnumerable<TypeViewModel> displayedTypes = DisplayedTypes;
		ColorizeTypes(displayedTypes);
		Graph = CreateGraph(displayedTypes);
		OnPropertyChanged("DisplayedTypes");
	}

	private void HideSelectionViewCommandHandler()
	{
		IsTypeSelectionVisible = false;
	}

	private void ShowSelectionViewCommandHandler()
	{
		IsTypeSelectionVisible = true;
	}

	private void ToggleSelectionViewCommandHandler()
	{
		IsTypeSelectionVisible = !IsTypeSelectionVisible;
	}

	private void ToggleTypeListVisibilityCommandHandler()
	{
		IsTypeListVisible = !IsTypeListVisible;
	}

	private void HideSearchCommandHandler()
	{
		IsSearchVisible = false;
		SearchTerm = string.Empty;
	}

	private void ShowSearchCommandHandler()
	{
		IsSearchVisible = true;
		OnFocusSearchRequest();
	}

	private void OnOriginalSizeRequest()
	{
		this.OriginalSizeRequest?.Invoke();
	}

	private void OnFillGraphRequest()
	{
		this.FillGraphRequest?.Invoke();
	}

	private void OnFocusSearchRequest()
	{
		this.FocusSearchRequest?.Invoke();
	}

	private static bool IsPropertyAccessor(MethodInfo method)
	{
		return method.IsSpecialName && (method.Name.IndexOf("get_") != -1 || method.Name.IndexOf("set_") != -1);
	}

	private static bool IsEventAccessor(MethodInfo method)
	{
		return method.IsSpecialName && (method.Name.IndexOf("add_") != -1 || method.Name.IndexOf("remove_") != -1);
	}

	private static void ColorizeTypes(IEnumerable<TypeViewModel> types)
	{
		int num = 0;
		foreach (TypeViewModel type in types)
		{
			type.Background = BrushProvider.SingleBrushes[num];
			num++;
			if (num >= BrushProvider.SingleBrushes.Count)
			{
				num = 0;
			}
		}
	}
}
