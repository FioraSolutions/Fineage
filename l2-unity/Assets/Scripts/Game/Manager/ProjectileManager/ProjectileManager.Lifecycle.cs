public partial class ProjectileManager : AbstractProjectile, IProjectileManager
{
    private void Awake()
    {
        if ((_instance as UnityEngine.Object) != null && !ReferenceEquals(_instance, this))
        {
            Destroy(gameObject);
            return;
        }
        _entityMask = LayerMask.GetMask("EntityClick");
        _instance = this;
        WireProjectilePacketServices();
        WireGamePacketServices();
    }

    void OnDestroy()
    {
        if (ReferenceEquals(_instance, this))
            _instance = null;
    }

    private void Start()
    {
        WireProjectilePacketServices();
        WireGamePacketServices();
        HitManager hits = _hits != null ? _hits : HitPacketServices.Hits;
        if (hits != null)
            hits.BindProjectileHits();
    }

    private void WireGamePacketServices()
    {
        GamePacketServices.Bind(gameWorld: _world);
    }

    private void WireProjectilePacketServices()
    {
        ProjectilePacketServices.Bind(projectiles: this);
    }
}
