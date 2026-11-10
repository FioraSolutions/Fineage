using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

public partial class CompositePrefabEffect : TimedCompositeEffectBase
{
    protected virtual void PlayV2()
    {
    }

    public override void SetProgress(float normalizedTime)
    {
    }

    public void SetImpactHit(Vector3 hitPoint, Vector3 hitDirection)
    {
        if (_context == null)
        {
            _context = CompositeEffectUtilities.BuildContext(_owner, _castData);
        }

        CompositeEffectUtilities.ApplyImpactHit(_context, hitPoint, hitDirection);
    }

}
