/// <summary>
/// Lobby character layer access with [Inject] wiring and Game fallback (#142).
/// </summary>
public static class CharacterPacketServices
{
    private static IAnimationManager _animations;
    private static CharacterBuilder _builder;

    public static void Bind(
        IAnimationManager animations = null,
        CharacterBuilder builder = null)
    {
        if (animations != null)
            _animations = animations;
        if (builder != null)
            _builder = builder;
    }

    public static IAnimationManager Animations =>
        _animations != null ? _animations : GamePacketServices.Animations;

    public static CharacterBuilder Builder =>
        _builder != null ? _builder : ResolveBuilderFallback();

    static CharacterBuilder ResolveBuilderFallback()
    {
        if (App.GameContainer != null)
        {
            try
            {
                return App.GameContainer.Resolve<CharacterBuilder>();
            }
            catch
            {
            }
        }

#pragma warning disable CS0618 // Locator fallback until drain completes
        return CharacterBuilder.Instance;
#pragma warning restore CS0618
    }
}
