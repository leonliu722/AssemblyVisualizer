using System.Collections.Generic;

namespace AssemblyVisualizer.Model;

internal class ModuleInfo
{
	public AssemblyInfo Assembly { get; set; }

	public IEnumerable<TypeInfo> Types { get; set; }
}
