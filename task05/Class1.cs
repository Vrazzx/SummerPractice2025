namespace task05;

using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

public class ClassAnalyzer
{
    private Type _type;

    public ClassAnalyzer(Type type)
    {
        _type = type;
    }

    public IEnumerable<string> GetPublicMethods()
    {
        return _type.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public).Select(m => m.Name);
    }

    public IEnumerable<string> GetMethodsParams(string methodName)
    {
        var method = _type.GetMethods().Where(m => m.Name == methodName);

        return method.First().GetParameters().Select(c => c.Name ?? string.Empty);
    }

    public IEnumerable<string> GetAllFields()
    {
        return _type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic).Select(f => f.Name);
    }

    public IEnumerable<string> GetProperties()
    {
        return _type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static).Select(p => p.Name);
    }

    public bool HasAttribute<T>() where T : Attribute
    {
        return _type.GetCustomAttributes(typeof(T), false).Any();
    }
}
