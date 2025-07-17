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


        using (var barrier = new Barrier(threadsNumber + 1))
        {
            for (int i = 0; i < threadsNumber; i++)
            {
                double start = a + i * segmentLength;
                double end = (i == threadsNumber - 1) ? b : start + segmentLength;

                Thread thread = new Thread(() =>
                {
                    double partialResult = CalculatePartialIntegral(start, end, function, step);
                    Interlocked.Exchange(ref result, result + partialResult);
                    barrier.SignalAndWait();
                });

                thread.Start();
            }


            barrier.SignalAndWait();
        }

        return result;
    }

    private static double CalculatePartialIntegral(double a, double b, Func<double, double> function, double step)
    {
        double integral = 0.0;
        double x = a;
        double nextX = x + step;

        while (x < b)
        {
            if (nextX > b) nextX = b;
            double y1 = function(x);
            double y2 = function(nextX);
            integral += (y1 + y2) * (nextX - x) / 2;
            x = nextX;
            nextX += step;
        }

        return integral;
    }
}
