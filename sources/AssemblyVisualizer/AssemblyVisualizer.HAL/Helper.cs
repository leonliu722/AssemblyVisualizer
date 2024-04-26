using System.Collections.Generic;
using AssemblyVisualizer.HAL.ILSpy;
using AssemblyVisualizer.Model;

namespace AssemblyVisualizer.HAL;

internal static class Helper
{
	private static readonly ModelHelper _helper = new ModelHelper();

	public static EventInfo GetEventForBackingField(object field)
	{
		return _helper.GetEventForBackingField(field);
	}

	public static PropertyInfo GetAccessorProperty(object method)
	{
		return _helper.GetAccessorProperty(method);
	}

	public static EventInfo GetAccessorEvent(object method)
	{
		return _helper.GetAccessorEvent(method);
	}

	public static IEnumerable<MethodInfo> GetUsedMethods(object method)
	{
		return _helper.GetUsedMethods(method);
	}

	public static IEnumerable<FieldInfo> GetUsedFields(object method)
	{
		return _helper.GetUsedFields(method);
	}

	public static TypeInfo GetDeclaringType(object member)
	{
		return _helper.LoadDeclaringType(member);
	}
}
