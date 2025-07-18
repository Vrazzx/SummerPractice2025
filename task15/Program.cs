using System;
using System.Diagnostics;
using ScottPlot;
public class Program
{

    public static double a = -100, b = 100;
    public static Func<double, double> sinFunc = Math.Sin;
    public static void Main(string[] args)
    {
        double optimalStep = OptimalStep();
        System.Console.WriteLine("" + optimalStep);
        OptimalThread(optimalStep);
        SingleOptimalThread(optimalStep);
    }

    public static double OptimalStep()
    {

        double exactValue = -Math.Cos(b) - (-Math.Cos(a));
        var errorsAndSteps = new Dictionary<double, double>();
        double[] steps = { 1e-1, 1e-2, 1e-3, 1e-4, 1e-5, 1e-6 };
        foreach (double step in steps)
        {
            double result = DefiniteIntegral.Solve(a, b, sinFunc, step, 1);
            double error = Math.Abs(result - exactValue);
            Console.WriteLine($"Step: {step}, Error: {error}");
            errorsAndSteps[step] = error;
        }
        var minEntry = errorsAndSteps.OrderBy(kv => kv.Value).FirstOrDefault();
        return minEntry.Key;
    }

    public static void SingleOptimalThread(double step)
    {
        var stopwatch = new Stopwatch();


        stopwatch.Restart();
        for (int i = 0; i < 10; i++)
        {
            DefiniteIntegral.SingleThreadSolve(a, b, sinFunc, step);
        }
        stopwatch.Stop();
        var times = stopwatch.Elapsed.TotalMilliseconds / 10;
        Console.WriteLine($"Avg Time: {times} ms");

    }
    public static void OptimalThread(double step)
    {

        int[] threadCounts = { 1, 2, 4, 8, 16, 32 };
        var stopwatch = new Stopwatch();
        var times = new Dictionary<int, double>();


        foreach (int threads in threadCounts)
        {

            stopwatch.Restart();
            for (int i = 0; i < 10; i++)
            {
                DefiniteIntegral.Solve(a, b, sinFunc, step, threads);
            }
            stopwatch.Stop();
            times[threads] = stopwatch.Elapsed.TotalMilliseconds / 10;
            Console.WriteLine($"Threads: {threads}, Avg Time: {times[threads]} ms");
        }


        var successfulTimes = times.Where(t => !double.IsNaN(t.Value)).ToDictionary(t => t.Key, t => t.Value);

        if (successfulTimes.Count > 0)
        {
            var plt = new ScottPlot.Plot();


            double[] xValues = successfulTimes.Keys.Select(x => (double)x).ToArray();
            double[] yValues = successfulTimes.Values.ToArray();

            var scatter = plt.Add.Scatter(xValues, yValues);
            scatter.LineWidth = 2;
            scatter.MarkerSize = 8;
            scatter.Label = "Время вычисления";


            plt.Title("Зависимость времени вычисления интеграла от количества потоков");
            plt.XLabel("Количество потоков");
            plt.YLabel("Время выполнения (мс)");


            plt.Axes.Bottom.TickGenerator = new ScottPlot.TickGenerators.NumericManual(
                positions: xValues,
                labels: xValues.Select(x => x.ToString()).ToArray()
            );


            plt.ShowLegend();


            plt.SavePng("threads_performance.png", 800, 600);
            Console.WriteLine("График сохранен как threads_performance.png");
        }
        else
        {
            Console.WriteLine("Нет данных для построения графика");
        }
    }
}
