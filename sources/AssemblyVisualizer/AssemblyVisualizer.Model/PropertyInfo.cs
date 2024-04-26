namespace AssemblyVisualizer.Model;

internal class PropertyInfo : MemberInfo
{
	public bool IsVirtual { get; set; }

	public bool IsOverride { get; set; }

	public bool IsFinal { get; set; }
}
