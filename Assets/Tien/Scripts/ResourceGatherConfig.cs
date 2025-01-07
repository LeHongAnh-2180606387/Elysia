using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName ="New Resource Pool Config", menuName = "Configs/ResourcePool")]
public class ResourceGatherConfig : ScriptableObject
{
    [SerializeField] private List<ResourceOdds> resourceOdds;
    
    public List<ResourceOdds> GetDecrescentOrderedResourcesOdds()
    {
        return resourceOdds.OrderByDescending(ro => ro.MinGoldRequirement).ToList();
    }
}

[Serializable]
public class ResourceOdds
{
    public int MinGoldRequirement = 0;
    public List<EntryProbabilityWeight> resourcePool;

    public EntryProbabilityWeight GetRandomEntryBasedOnWeight()
    {
        if (resourcePool == null || resourcePool.Count == 0)
        {
            return null;
        }

        float totalWeight = 0f;
        foreach (var entry in resourcePool)
        {
            totalWeight += entry.Weight;
        }

        float randomWeight = UnityEngine.Random.Range(0, totalWeight);

        float cumulativeWeight = 0f;
        foreach (var entry in resourcePool)
        {
            cumulativeWeight += entry.Weight;
            if (randomWeight <= cumulativeWeight)
            {
                return entry;
            }
        }

        return resourcePool[resourcePool.Count - 1];
    }
}

[Serializable]
public class EntryProbabilityWeight
{
    public ItemSO Item;
    public float Weight;
}
