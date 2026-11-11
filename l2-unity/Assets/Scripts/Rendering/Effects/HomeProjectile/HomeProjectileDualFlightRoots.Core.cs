using System.Collections.Generic;
using UnityEngine;

public sealed partial class HomeProjectileDualFlightRoots
{
    public List<HomeProjectileFlightAnchor> CollectTaggedAnchors(BaseEffect effect)
    {
        List<HomeProjectileFlightAnchor> anchors = new List<HomeProjectileFlightAnchor>();
        if (effect == null)
        {
            return anchors;
        }

        HomeProjectileFlightAnchor[] tagged =
            effect.GetComponentsInChildren<HomeProjectileFlightAnchor>(true);
        for (int i = 0; i < tagged.Length; i++)
        {
            if (tagged[i] != null)
            {
                anchors.Add(tagged[i]);
            }
        }

        return anchors;
    }
}
