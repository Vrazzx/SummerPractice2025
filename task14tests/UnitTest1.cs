namespace task14tests;

using Xunit;
public class DefiniteIntegralTests
{



    [Fact]
    public void LinearFunction_SymmetricInterval_ReturnsZero()
    {
        var X = (double x) => x;
        var result = DefiniteIntegral.Solve(-1, 1, X, 1e-4, 2);
        Assert.Equal(0, result, 1e-4);
    }
    [Fact]
    public void SinFunction_SymmetricInterval_ReturnsZero()
    {
        var SIN = (double x) => Math.Sin(x);
        var result = DefiniteIntegral.Solve(-1, 1, SIN, 1e-5, 8);
        Assert.Equal(0, result, 1e-4);
    }
    [Fact]
    public void LinearFunction_ZeroToFive_ReturnsTen()
    {
        var X = (double x) => x;
        var result = DefiniteIntegral.Solve(0, 5, X, 1e-6, 8);
        Assert.Equal(12.5, result, 1e-5);
    }
}






