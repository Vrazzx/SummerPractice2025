namespace task07;

using System.Reflection;
using System.Collections.Generic;
using System.Linq;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property)]
public class DisplayNameAttribute : Attribute
{
    public string DisplayName { get; }
    public DisplayNameAttribute(string displayName)
    {
        DisplayName = displayName;
    }
}

[AttributeUsage(AttributeTargets.Class)]
public class VersionAttribute : Attribute
{
    public int Major { get; }
    public int Minor { get; }

    public VersionAttribute(int major, int minor)
    {
        Major = major;
        Minor = minor;
    }
}


[DisplayName("Пример класса")]
[Version(1, 0)]
public class SampleClass
{

    [DisplayName("Числовое свойство")]
    public int Number { get; set; }

    [DisplayName("Тестовый метод")]
    public void TestMethod()
    {

    }
}

public static class ReflectionHelper
{
    public static void PrintClassInfo(Type type)
    {
        var displayNameAttribute = type.GetCustomAttribute<DisplayNameAttribute>();
        if (displayNameAttribute != null)
        {
            Console.WriteLine($"Имя класса: {displayNameAttribute.DisplayName}");
        }

        var versionAttribute = type.GetCustomAttribute<VersionAttribute>();
        if (versionAttribute != null)
        {
            Console.WriteLine($"Версия класса: {versionAttribute.Major}.{versionAttribute.Minor}");
        }

        Console.WriteLine("Методы: ");
        foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
        {
            var methodDisplayName = method.GetCustomAttribute<DisplayNameAttribute>();
            Console.WriteLine($"- {method.Name}: {(methodDisplayName != null ? methodDisplayName.DisplayName : "Нет имени")}");
        }
        System.Console.WriteLine("Свойства");
        foreach (var property in type.GetProperties())
        {
            var propertyDisplayName = property.GetCustomAttribute<DisplayNameAttribute>();
            System.Console.WriteLine($"- {property.Name}: {(propertyDisplayName != null ? propertyDisplayName.DisplayName : "Нет имени")}");
        }
    }
    
}
