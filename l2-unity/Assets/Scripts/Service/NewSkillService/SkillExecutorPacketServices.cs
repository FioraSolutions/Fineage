using VContainer;

/// <summary>
/// Skill execution access with [Inject] wiring and singleton fallback (#131 / #142).
/// </summary>
public static class SkillExecutorPacketServices
{
    private static SkillExecutor _executor;

    public static void Bind(SkillExecutor executor = null)
    {
        if (executor != null)
            _executor = executor;
    }

    public static SkillExecutor Executor =>
        _executor != null
            ? _executor
            : ResolveFromGame<SkillExecutor>() ?? LegacySkillExecutorInstance();

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
    static SkillExecutor LegacySkillExecutorInstance() => SkillExecutor.Instance;
#pragma warning restore CS0618
}
