using System;
using VContainer;

/// <summary>
/// Shared motion services. App scope registers them once; lobby and game scopes inherit them.
/// </summary>
public static class MotionPipelineRegistration
{
    public static void Register(IContainerBuilder builder)
    {
        builder.Register<AnimationManager>(Lifetime.Singleton)
            .As<IAnimationManager>()
            .As<IAnimationRoster>()
            .AsSelf();
        builder.Register<SharedMoveRoster>(Lifetime.Singleton)
            .As<IMoveRoster>()
            .AsSelf();
    }

    public static bool IsMotionService(Type type)
    {
        return type == typeof(IAnimationManager)
            || type == typeof(IAnimationRoster)
            || type == typeof(IMoveRoster);
    }
}
