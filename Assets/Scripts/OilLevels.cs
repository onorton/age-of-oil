using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;

public class OilLevels
{
    private IDictionary<int, double> _barrelsPerLevel = new Dictionary<int, double>()
    {
        [100] = 100,
        [200] = 100
    };

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
}
