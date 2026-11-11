using UnityEngine;
using System;
using System.Text;
using System.Collections.Generic;

public abstract partial class BaseEffect : MonoBehaviour
{
    protected MaterialPropertyBlock propBlock;
    protected Renderer targetRenderer;
    [SerializeField] protected Transform _owner;
    private float _scheduledDestroyAt = -1f;
    private float _scheduledLifeTime = -1f;
    private float _castStartTimeSnapshot = -1f;
    private float _castHitTimeSnapshot = -1f;
    private float _castFlightTimeSnapshot = -1f;
    private bool _deferDestroyUntilHomeArrival;
    private bool _lifetimeOwnedByProjectile;
    private bool _lifetimeOwnedByAuthoredStreams;
    private bool _lifetimeOwnedByHost;
    public virtual void Initialize()
    {
        propBlock = new MaterialPropertyBlock();
        targetRenderer = GetComponent<Renderer>();
    }

    public abstract void SetProgress(float normalizedTime); 
    public abstract void Play();

    /// <summary>
    /// Nested prefab assets this effect needs at runtime. Collected into
    /// EffectManager's session retain list at BuildRegistry.
    /// </summary>
    public virtual void CollectReferencedPrefabs(System.Collections.Generic.List<UnityEngine.Object> sink)
    {
    }

    public virtual void Setup(EffectSettings settings, MagicCastData castData, Transform owner)
    {
        _owner = owner;
        if (castData != null)
        {
            _castStartTimeSnapshot = castData.StartTime;
            _castHitTimeSnapshot = castData.HitTime;
            _castFlightTimeSnapshot = castData.FlightTime;
        }
        else
        {
            _castStartTimeSnapshot = -1f;
            _castHitTimeSnapshot = -1f;
            _castFlightTimeSnapshot = -1f;
        }
    }

    public bool IsDestroyDeferredUntilHomeArrival => _deferDestroyUntilHomeArrival;

    public bool IsLifetimeOwnedByProjectile => _lifetimeOwnedByProjectile;

    public bool IsLifetimeOwnedByAuthoredStreams => _lifetimeOwnedByAuthoredStreams;

    public bool IsLifetimeOwnedByHost => _lifetimeOwnedByHost;

    /// <summary>
    /// NPC deco: do not DestoryEffect on Play. Owner (NpcDecoService) destroys the GO.
    /// </summary>
}