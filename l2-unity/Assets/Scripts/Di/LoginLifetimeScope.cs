using UnityEngine;
using VContainer;
using VContainer.Unity;

public sealed class LoginLifetimeScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        LifetimeScopeMonoRegistration.RegisterIfFound<L2LoginUI>(builder);
        LifetimeScopeMonoRegistration.RegisterIfFound<LoginWindow>(builder);
        LifetimeScopeMonoRegistration.RegisterIfFound<LicenseWindow>(builder);
        LifetimeScopeMonoRegistration.RegisterIfFound<ServerSelectWindow>(builder);
        LifetimeScopeMonoRegistration.RegisterIfFound<CharSelectWindow>(builder);
        LifetimeScopeMonoRegistration.RegisterIfFound<CharCreationWindow>(builder);
        LifetimeScopeMonoRegistration.RegisterIfFound<LoginCameraManager>(builder);
        LifetimeScopeMonoRegistration.RegisterIfFound<CharacterSelector>(builder);
        LifetimeScopeMonoRegistration.RegisterIfFound<CharacterBuilder>(builder);
        LifetimeScopeMonoRegistration.RegisterIfFound<CharacterCreator>(builder);

        builder.Register<LoginRuntime>(Lifetime.Singleton);

        builder.RegisterBuildCallback(container =>
        {
            LoginRuntime loginRuntime = container.Resolve<LoginRuntime>();
            AppLoginSessionApplyIncomingPacketServices.BindLogin(loginRuntime);
            if (App.Container != null)
            {
                try
                {
                    LobbyFlowController lobby = App.Container.Resolve<LobbyFlowController>();
                    lobby.WireLogin(loginRuntime);
                    GameManager manager = App.Container.Resolve<GameManager>();
                    if (loginRuntime.Ui != null)
                        manager.WireLoginUi(loginRuntime.Ui);
                }
                catch
                {
                }
            }
        });
    }

}
