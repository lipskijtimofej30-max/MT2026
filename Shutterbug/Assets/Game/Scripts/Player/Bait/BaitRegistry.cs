using System;
using System.Collections;
using System.Collections.Generic;
using Game.Scripts;
using UnityEngine;

public class BaitRegistry
{
    private readonly HashSet<Bait> _activeBaits = new();
    public HashSet<Bait> ActiveBaits => _activeBaits;
    public event Action<HashSet<Bait>> OnBaitsChanged;

    public void Register(Bait bait)
    {
        _activeBaits.Add(bait);
        OnBaitsChanged?.Invoke(_activeBaits);
    }

    public void Unregister(Bait bait)
    {
        _activeBaits.Remove(bait);
        OnBaitsChanged?.Invoke(_activeBaits);
    }

    public Bait GetClosestBait(Vector3 searchPosition, float maxRadius)
    {
        Bait closest = null;
        float minDistanceSqr = maxRadius * maxRadius;

        foreach (var bait in _activeBaits)
        {
            if (bait == null) continue;

            float distSqr = (bait.transform.position - searchPosition).sqrMagnitude;
            if (distSqr < minDistanceSqr)
            {
                minDistanceSqr = distSqr;
                closest = bait;
            }
        }

        return closest;
    }
}
