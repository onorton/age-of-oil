using System.Collections.Generic;
using System;
using System.Linq;

public class OilLevels
{
    private IDictionary<int, double> _barrelsPerLevel;

    private static Random random = new Random();

    public OilLevels()
    {
        _barrelsPerLevel = new Dictionary<int, double>();

        var numberOfLevels = random.Next(3, 15);

        for (var i = 1; i <= numberOfLevels; i++)
        {
            var mean = Fib(i) * 1000;
            var sample = normalSample(mean, mean / 10);
            _barrelsPerLevel[i * 100] = sample;
        }
    }

    private double normalSample(double mean, double stdDev)
    {
        double u1 = 1.0 - random.NextDouble();
        double u2 = 1.0 - random.NextDouble();
        double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) *
                     Math.Sin(2.0 * Math.PI * u2);
        return mean + stdDev * randStdNormal;

    }

    private static int Fib(int n)
    {
        if (n < 2)
        {
            return n;
        }
        else
        {
            return Fib(n - 1) + Fib(n - 2);
        }
    }

    public double BarrelsUpToGivenDepth(int maxDepth)
    {
        var barrels = 0.0;
        List<int> depthsInOrder = _barrelsPerLevel.Keys.OrderBy(a => a).ToList();
        foreach (var depth in depthsInOrder)
        {
            if (depth <= maxDepth)
            {
                barrels += _barrelsPerLevel[depth];
            }
            else
            {
                break;
            }
        }
        return barrels;
    }

    // Will try to remove from maximum depth first
    public void RemoveBarrelsUpToGivenDepth(double barrels, int maxDepth)
    {
        double barrelsRemaining = barrels;

        List<int> depthsInReverseOrder = _barrelsPerLevel.Keys.OrderByDescending(a => a).ToList();
        foreach (var depth in depthsInReverseOrder)
        {
            if (depth <= maxDepth)
            {
                var barrelsToRemove = Math.Min(barrelsRemaining, _barrelsPerLevel[depth]);
                _barrelsPerLevel[depth] -= barrelsToRemove;
                barrelsRemaining -= barrelsToRemove;
            }
            else if (barrelsRemaining <= 0)
            {
                break;
            }
        }

    }

    public string Prospect(double accuracy)
    {
        var maximumProspectingDepth = 1000 * accuracy;

        var seenTotalSoFar = 0.0;
        var results = "Estimated:";
        List<int> depthsInOrder = _barrelsPerLevel.Keys.OrderBy(a => a).ToList();
        foreach (var depth in depthsInOrder)
        {
            if (depth > maximumProspectingDepth)
            {
                break;
            }

            var actualAmount = _barrelsPerLevel[depth];
            var min = actualAmount - (1 - accuracy) * actualAmount;
            var max = actualAmount + (1 - accuracy) * actualAmount;
            var seenAmount = min + (max - min) * random.NextDouble();

            seenTotalSoFar += seenAmount;
            var formattedTotal = string.Format("{0:0.} barrels", seenTotalSoFar);
            results += $"\nUp to {depth}ft: {formattedTotal}";
        }

        return results;
    }
}
