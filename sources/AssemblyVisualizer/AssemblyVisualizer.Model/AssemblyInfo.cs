using System;
using System.Collections.Generic;

namespace AssemblyVisualizer.Model;

internal class AssemblyInfo
{
	public string Name { get; set; }

	public string FullName { get; set; }

	public IEnumerable<ModuleInfo> Modules { get; set; }

	public IEnumerable<AssemblyInfo> ReferencedAssemblies { get; set; }

	public int ExportedTypesCount { get; set; }

	public int InternalTypesCount { get; set; }

	public Version Version { get; set; }
}
