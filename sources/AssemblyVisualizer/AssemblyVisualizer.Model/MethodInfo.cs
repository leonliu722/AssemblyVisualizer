namespace AssemblyVisualizer.Model;

internal class MethodInfo : MemberInfo
{
	public bool IsVirtual { get; set; }

	public bool IsOverride { get; set; }

	public bool IsSpecialName { get; set; }

	public bool IsFinal { get; set; }
}
