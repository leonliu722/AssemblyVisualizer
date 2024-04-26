using System.Windows.Media;
using AssemblyVisualizer.HAL;

namespace AssemblyVisualizer.Model;

internal class MemberInfo
{
	private TypeInfo _declaringType;

	public string Name { get; set; }

	public string FullName { get; set; }

	public string Text { get; set; }

	public bool IsPublic { get; set; }

	public bool IsInternal { get; set; }

	public bool IsProtected { get; set; }

	public bool IsPrivate { get; set; }

	public bool IsProtectedOrInternal { get; set; }

	public bool IsProtectedAndInternal { get; set; }

	public bool IsStatic { get; set; }

	public ImageSource Icon { get; set; }

	public object MemberReference { get; set; }

	public TypeInfo DeclaringType
	{
		get
		{
			if (_declaringType == null)
			{
				_declaringType = Helper.GetDeclaringType(MemberReference);
			}
			return _declaringType;
		}
		set
		{
			_declaringType = value;
		}
	}
}
