namespace task11tests;

using Xunit;
using task11;

public class DynamicCalculatorTests
{
    [Fact]
    public void TestAdd()
    {
        Type calculatorType = DynamicClassGenerator.GenerateCalculatorType();
        dynamic calculator = Activator.CreateInstance(calculatorType);
        
        Assert.Equal(8, calculator.Add(5, 3));
    }

    [Fact]
    public void TestMinus()
    {
        Type calculatorType = DynamicClassGenerator.GenerateCalculatorType();
        dynamic calculator = Activator.CreateInstance(calculatorType);
        
        Assert.Equal(2, calculator.Minus(5, 3));
    }

    [Fact]
    public void TestMul()
    {
        Type calculatorType = DynamicClassGenerator.GenerateCalculatorType();
        dynamic calculator = Activator.CreateInstance(calculatorType);
        
        Assert.Equal(15, calculator.Mul(5, 3));
    }

    [Fact]
    public void TestDiv()
    {
        Type calculatorType = DynamicClassGenerator.GenerateCalculatorType();
        dynamic calculator = Activator.CreateInstance(calculatorType);
        
        Assert.Equal(2, calculator.Div(6, 3));
    }
}