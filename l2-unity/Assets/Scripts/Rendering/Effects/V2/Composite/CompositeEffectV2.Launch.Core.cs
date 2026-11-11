using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public partial class CompositeEffectV2 : CompositePrefabEffect
{
    void TryLaunchImmediate(CompositePart part, BaseEffect instance)
    {
        if (part == null || instance == null || _launchedProjectiles.Contains(part))
        {
            return;
        }

        HomeFlightPart home = part as HomeFlightPart;
        if (home != null)
        {
            if (home.ShouldLaunchImmediately && home.TryLaunch(instance, _context, HomeProjectiles))
            {
                _launchedProjectiles.Add(part);
            }

            return;
        }

        ShotProjectilePart shot = part as ShotProjectilePart;
        if (shot == null || !shot.ShouldLaunchImmediately)
        {
            return;
        }

        if (shot.TryLaunch(instance, _context))
        {
            _launchedProjectiles.Add(part);
        }
    }
}
