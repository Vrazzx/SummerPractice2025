namespace task05tests;

using Xunit;
using Moq;
using task05;

public class TestClass
{
    public int PublicField;
    private string _privateField;
    public int Property { get; set; }

    public void Method() { }
    public void MethodWithSomeParams(string a, int b) { }
}

[Serializable]
public class AttributedClass { }

public class ClassAnalyzerTests
{
    [Fact]
    public void GetPublicMethods_ReturnsCorrectMethods()
    {
        var analyzer = new ClassAnalyzer(typeof(TestClass));
        var methods = analyzer.GetPublicMethods();

        Assert.Contains("Method", methods);
        Assert.Contains("MethodWithSomeParams", methods);
    }

    [Fact]
    public void GetAllFields_IncludesPrivateFields()
    {
        var analyzer = new ClassAnalyzer(typeof(TestClass));
        var fields = analyzer.GetAllFields();

        Assert.Contains("_privateField", fields);
        Assert.Contains("PublicField", fields);
    }

    [Fact]
    public void GetProperties_ReturnsCorrectProperties()
    {
        var analyzer = new ClassAnalyzer(typeof(TestClass));
        var properties = analyzer.GetProperties().ToList();

        Assert.Single(properties);
        Assert.Contains("Property", properties);
    }

    [Fact]
    public void GetMethodsParams_ReturnsCorrectParams()
    {
        var analyzer = new ClassAnalyzer(typeof(TestClass));
        var emptyParameters = analyzer.GetMethodsParams("Method");
        var withParameters = analyzer.GetMethodsParams("MethodWithSomeParams");

        Assert.Empty(emptyParameters);
        Assert.Contains("a", withParameters);
        Assert.Contains("b", withParameters);
    }

    [Fact]
    public void HasAttribute_ReturnsTrueForAttributedClass()
    {
        var analyzerAtribute = new ClassAnalyzer(typeof(AttributedClass));
        bool hasAttribute = analyzerAtribute.HasAttribute<SerializableAttribute>();
        var analyzer = new ClassAnalyzer(typeof(TestClass));
        bool hasntAttribute = analyzer.HasAttribute<SerializableAttribute>();
        Assert.True(hasAttribute);
        Assert.False(hasntAttribute);
    }
}