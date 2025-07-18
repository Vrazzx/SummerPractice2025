using System;
using System.Threading;

public class DefiniteIntegral
{
public static double Solve(double a, double b, Func<double, double> function, double step, int threadsNumber)
{
    if (threadsNumber <= 0) throw new ArgumentException("Число потоков должно быть положительным");
    if (step <= 0) throw new ArgumentException("Шаг должен быть положительным");
    if (b < a) throw new ArgumentException("b должно быть больше a");

    // Оптимизация для однопоточного случая
    if (threadsNumber == 1)
    {
        return CalculatePartialIntegral(a, b, function, step);
    }

    double totalLength = b - a;
    double segmentLength = totalLength / threadsNumber;
    double result = 0.0;
    var threads = new Thread[threadsNumber];
    var partialResults = new double[threadsNumber];

    using (var countdown = new CountdownEvent(threadsNumber))
    {
        for (int i = 0; i < threadsNumber; i++)
        {
            int threadIndex = i; // Захватываем локальную переменную для потока
            double start = a + threadIndex * segmentLength;
            double end = (threadIndex == threadsNumber - 1) ? b : start + segmentLength;

            threads[threadIndex] = new Thread(() =>
            {
                partialResults[threadIndex] = CalculatePartialIntegral(start, end, function, step);
                countdown.Signal();
            });

            threads[threadIndex].Start();
        }

        countdown.Wait(); // Ждем завершения всех потоков
        result = partialResults.Sum();
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
    public static double SingleThreadSolve(double a, double b, Func<double, double> function, double step)
    {
        if (step <= 0) throw new ArgumentException("Шаг должен быть положительным");
        if (b < a) throw new ArgumentException("b Должно быть больше a");
        
        return CalculatePartialIntegral(a, b, function, step);
    }
}
