using VContainer;

/// <summary>
/// NPC/monster entity action machine access with [Inject] wiring and container/live fallback (#131).
/// </summary>
public static class EntityActionPacketServices
{
    private static EntityActionMachine _actions;

    public static void Bind(EntityActionMachine actions = null)
    {
        if (actions != null)
            _actions = actions;
    }

    /// <summary>The roster <see cref="Bind"/> stored. No container or <c>Instance</c> fallback.</summary>
    public static IEntityActionRoster BoundActions => _actions;

    public static EntityActionMachine Actions =>
        _actions != null
            ? _actions
            : ResolveFromGame<EntityActionMachine>() ?? LegacyEntityActionMachineInstance();

    static T ResolveFromGame<T>() where T : class
    {
        if (App.GameContainer == null)
            return null;

        try
        {
            return App.GameContainer.Resolve<T>();
        }
        catch
        {
            return null;
        }
    }

#pragma warning disable CS0618 // Locator fallback until all call sites are [Inject]
    static EntityActionMachine LegacyEntityActionMachineInstance() => EntityActionMachine.Instance;
#pragma warning restore CS0618
}
