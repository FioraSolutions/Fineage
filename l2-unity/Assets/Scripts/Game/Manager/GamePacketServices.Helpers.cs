using VContainer;

public static partial class GamePacketServices
{
    static IAnimationManager ResolveAnimationsFallback() =>
        ResolveFromGame<IAnimationManager>() ?? AnimationPacketServices.LegacyAnimationManagerInstance();
}
