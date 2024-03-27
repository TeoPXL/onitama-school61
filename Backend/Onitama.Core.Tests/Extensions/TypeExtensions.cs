using System.Reflection;

namespace Onitama.Core.Tests.Extensions;

public static class TypeExtensions
{
    public static void AssertInterfaceProperty(this Type interfaceType, string propertyName, bool shouldHaveGetter, bool shouldHaveSetter)
    {
        PropertyInfo? property = interfaceType.GetProperty(propertyName);
        Assert.That(property, Is.Not.Null, $"{propertyName} property is missing in the interface");
        if (shouldHaveGetter)
        {
            Assert.That(property!.GetMethod, Is.Not.Null, $"{propertyName} property of the interface does not have a getter");
        }
        else
        {
            Assert.That(property!.GetMethod, Is.Null, $"{propertyName} property of the interface should NOT have a getter");
        }

        if (shouldHaveSetter)
        {
            Assert.That(property.SetMethod, Is.Not.Null, $"{propertyName} property of the interface does not have a setter");
        }
        else
        {
            Assert.That(property.SetMethod, Is.Null, $"{propertyName} property of the interface should NOT have a setter");
        }
    }

    public static void AssertInterfaceMethod(this Type interfaceType, string methodName, Type returnType, params Type[] parameterTypes)
    {
        MethodInfo? method = interfaceType.GetMethod(methodName);
        Assert.That(method, Is.Not.Null, $"{methodName} method is missing in the interface");

        Assert.That(method!.ReturnType, Is.EqualTo(returnType), $"{methodName} method does not have the correct return type in the interface");
        ParameterInfo[] parameters = method.GetParameters();
        Assert.That(parameters.Length, Is.EqualTo(parameterTypes.Length), $"{methodName} method does not have the correct number of parameters in the interface");

        for (int i = 0; i < parameters.Length; i++)
        {
            Assert.That(parameters[i].ParameterType.FullName, Does.StartWith(parameterTypes[i].FullName), $"{methodName} method's parameter at position {i} does not have the correct type");
        }
    }
}