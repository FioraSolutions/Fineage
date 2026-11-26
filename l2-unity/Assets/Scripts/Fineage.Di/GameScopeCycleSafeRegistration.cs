using VContainer;

/// <summary>
/// GameScope registrations whose types already live in Fineage.Domain, Fineage.Game, or Fineage.Controller.
/// Assembly-CSharp MonoBehaviours stay on <c>GameLifetimeScope</c>. Lifetimes match the previous lines.
/// </summary>
public static class GameScopeCycleSafeRegistration
{
    public static void Register(IContainerBuilder builder)
    {
        builder.Register<GameEventBus>(Lifetime.Singleton).As<IGameEventBus>();
        builder.Register<StatsConverter>(Lifetime.Singleton);
        builder.Register<ItemDropLayerService>(Lifetime.Singleton);
        builder.Register<L2PawnYaw>(Lifetime.Singleton);
        builder.Register<InterludeActionAvailabilityService>(Lifetime.Singleton);
    }
}
