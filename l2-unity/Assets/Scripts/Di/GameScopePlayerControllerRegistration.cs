using UnityEngine;
using VContainer;
using VContainer.Unity;

/// <summary>
/// Ensures <see cref="PlayerController"/> is always registered in GameScope before component injection.
/// VContainer does not create a registration for bare <c>Register&lt;MonoBehaviour&gt;(factory)</c>;
/// scene components with [Inject] PlayerController otherwise fail with "No such registration".
/// </summary>
internal static class GameScopePlayerControllerRegistration
{
    internal static void Register(IContainerBuilder builder)
    {
        PlayerController inScene = Object.FindFirstObjectByType<PlayerController>(FindObjectsInactive.Include);
        if (inScene != null)
        {
            builder.RegisterComponent(inScene);
            return;
        }

        builder.Register<LivePlayerControllerAccessor>(Lifetime.Singleton);
        builder.Register<PlayerController>(
            container => container.Resolve<LivePlayerControllerAccessor>().Current,
            Lifetime.Singleton);
    }

    sealed class LivePlayerControllerAccessor
    {
        public PlayerController Current =>
            Object.FindFirstObjectByType<PlayerController>(FindObjectsInactive.Include)
            ?? PlayerController.Instance;
    }
}
