namespace AssemblyVisualizer.Model;

internal class FieldInfo : MemberInfo
{
	public bool IsInitOnly { get; set; }

	public bool IsSpecialName { get; set; }

	public bool IsLiteral { get; set; }
}
