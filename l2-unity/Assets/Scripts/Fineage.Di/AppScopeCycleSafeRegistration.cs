using UnityEngine;
using VContainer;

/// <summary>
/// AppScope registrations whose types already live in Fineage.Networking.
/// Assembly-CSharp hosts stay on <c>AppLifetimeScope</c>. Lifetimes match the previous lines.
/// </summary>
public static class AppScopeCycleSafeRegistration
{
    public static void Register(MonoBehaviour host, IContainerBuilder builder)
    {
        LifetimeScopeMonoRegistration.RegisterIfFoundOnHost<GameClient>(host, builder);
        LifetimeScopeMonoRegistration.RegisterIfFoundOnHost<LoginClient>(host, builder);
        builder.Register<PacketApplyQueue>(Lifetime.Singleton).As<IPacketApplyQueue>().AsSelf();
    }
}
