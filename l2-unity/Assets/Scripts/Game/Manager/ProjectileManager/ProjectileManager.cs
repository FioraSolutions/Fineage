public partial class ProjectileManager : AbstractProjectile, IProjectileManager
{
    [Inject] HitManager _hits;
    [Inject] World _world;

    private World GameWorld
    {
        get { return _world != null ? _world : GamePacketServices.GameWorld; }
    }
    private const string PROJECTILE_TIMER_LOG = "[PROJECTILE_TIMER]";
    private const string MagicProjectileSyncTag = "[MAGIC_PROJECTILE_SYNC]";
    private const float ArrowAnimatorLaunchCompensationSeconds = 0.08f;

    [SerializeField] public ProjectileData defaultSettings;
    public event Action<GameObject, Transform, Vector3, Vector3, int> OnHitMonster;
    public event Action<GameObject, Transform, Vector3, Vector3, int> OnHitEffectProjectile;

    private Dictionary<int, ProjectileData> activeProjectiles = new Dictionary<int, ProjectileData>();
    private int nextId = 0;

    #region Singleton
    private static IProjectileManager _instance;

    [System.Obsolete("Use [Inject] or ProjectilePacketServices.Projectiles instead")]
    public static IProjectileManager Instance => _instance;

}
