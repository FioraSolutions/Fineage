using System;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public sealed partial class HomeProjectileTrailVelocityProvider : MonoBehaviour
{
    public void ResetRuntimeState()
    {
        OnEnable();
    }
}
