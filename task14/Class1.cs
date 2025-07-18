using System;
using System.Threading;

public class DefiniteIntegral
{
    public static double Solve(double a, double b, Func<double, double> function, double step, int threadsNumber)
    {
        if (threadsNumber <= 0) throw new ArgumentException("Число потоков должно быть положительным");
        if (step <= 0) throw new ArgumentException("Шаг должен быть положительным");
        if (b < a) throw new ArgumentException("b Должно быть больше a");


        int actualThreads = Math.Min(threadsNumber, Environment.ProcessorCount);
        double totalLength = b - a;
        double result = 0.0;


        Parallel.For(0, actualThreads, new ParallelOptions { MaxDegreeOfParallelism = actualThreads }, i =>
        {

            double start = a + i * totalLength / actualThreads;
            double end = a + (i + 1) * totalLength / actualThreads;

            double partialResult = CalculatePartialIntegral(start, end, function, step);


            Interlocked.Exchange(ref result, result + partialResult);
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
