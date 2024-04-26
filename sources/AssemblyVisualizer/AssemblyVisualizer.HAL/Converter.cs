using AssemblyVisualizer.HAL.ILSpy;
using AssemblyVisualizer.Model;

namespace AssemblyVisualizer.HAL;

internal class Converter
{
	private static readonly IConverter _converter;

	static Converter()
	{
		_converter = new AssemblyVisualizer.HAL.ILSpy.Converter();
	}

	public static AssemblyInfo Assembly(object assembly)
	{
		return _converter.Assembly(assembly);
	}

	public static TypeInfo Type(object type)
	{
		return _converter.Type(type);
	}

	public static MethodInfo Method(object method)
	{
		return _converter.Method(method);
	}

	public static FieldInfo Field(object field)
	{
		return _converter.Field(field);
	}

	public static PropertyInfo Property(object property)
	{
		return _converter.Property(property);
	}

	public static EventInfo Event(object ev)
	{
		return _converter.Event(ev);
	}

	public static void ClearCache()
	{
		_converter.ClearCache();
	}
}
