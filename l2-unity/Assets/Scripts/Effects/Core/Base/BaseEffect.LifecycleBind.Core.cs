using UnityEngine;
using System;
using System.Text;

public abstract partial class BaseEffect
{
    public void BindLifetimeToHost()
    {
        _lifetimeOwnedByHost = true;
        CancelInvoke(nameof(BeginFadeOut));
    }

    public void BindLifetimeToProjectile()
    {
        _lifetimeOwnedByProjectile = true;
        CancelInvoke(nameof(BeginFadeOut));
    }

    public void BindLifetimeToAuthoredStreams()
    {
        _lifetimeOwnedByAuthoredStreams = true;
        CancelInvoke(nameof(BeginFadeOut));
    }

    public void PrepareDestroyOnHomeArrival()
    {
        _deferDestroyUntilHomeArrival = true;
        CancelInvoke(nameof(BeginFadeOut));
    }
}
