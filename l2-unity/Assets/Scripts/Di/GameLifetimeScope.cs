using System;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public sealed partial class GameLifetimeScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        RegisterWorldServices(builder);
        RegisterUiServices(builder);
    }


    private void RegisterIfFound<T>(IContainerBuilder builder) where T : MonoBehaviour
        => LifetimeScopeMonoRegistration.RegisterIfFound<T>(builder);

    /// <summary>
    /// Registers a type that is created after the scope is built (prefab-instantiated
    /// MonoBehaviours such as <see cref="PlayerInventory"/> / <see cref="TargetManager"/>).
    /// PacketServices fallbacks stay here; generic Find/LateBound lives in Fineage.Di (#131).
    /// </summary>
    private void RegisterLateBound<T>(IContainerBuilder builder) where T : MonoBehaviour
        => LifetimeScopeMonoRegistration.RegisterLateBound<T>(builder, () => ResolvePacketServicesFallback<T>());

    /// <summary>
    /// Registers a scene MonoBehaviour when present; otherwise a live singleton that resolves
    /// FindInScene → PacketServices Instance bridge (spawn-deferred types).
    /// </summary>
    private void RegisterLiveMono<T>(IContainerBuilder builder) where T : MonoBehaviour
        => LifetimeScopeMonoRegistration.RegisterLiveMono<T>(builder, () => ResolvePacketServicesFallback<T>());

}
