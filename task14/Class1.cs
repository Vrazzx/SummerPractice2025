using System;
using System.Threading;

public class DefiniteIntegral
{
public static double Solve(double a, double b, Func<double, double> function, double step, int threadsNumber)
{
    if (threadsNumber <= 0) throw new ArgumentException("Число потоков должно быть положительным");
    if (step <= 0) throw new ArgumentException("Шаг должен быть положительным");
    if (b < a) throw new ArgumentException("b Должно быть больше a");

    double totalLength = b - a;
    double segmentLength = totalLength / threadsNumber;
    double result = 0.0;

    
    Parallel.For(0, threadsNumber, () => 0.0, (i, state, localResult) =>
    {
        double start = a + i * segmentLength;
        double end = (i == threadsNumber - 1) ? b : start + segmentLength;
        return localResult + CalculatePartialIntegral(start, end, function, step);
    },
    localResult => 
    {
        
        Interlocked.Exchange(ref result, result + localResult);
    });

    return result;
}


    private static double CalculatePartialIntegral(double a, double b, Func<double, double> function, double step)
    {
        double integral = 0.0;
        double x = a;
        double stepSize = step;

        while (x < b)
        {
            double nextX = Math.Min(x + stepSize, b);
            double y1 = function(x);
            double y2 = function(nextX);
            integral += (y1 + y2) * (nextX - x) * 0.5;
            x = nextX;
        }

        return integral;
    }

    public static double SingleThreadSolve(double a, double b, Func<double, double> function, double step)
    {
        if (step <= 0) throw new ArgumentException("Шаг должен быть положительным");
        if (b < a) throw new ArgumentException("b Должно быть больше a");

        return CalculatePartialIntegral(a, b, function, step);
    }
}
