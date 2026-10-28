using UnityEngine;
using VContainer;
using VContainer.Unity;

public sealed class LoginLifetimeScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        RegisterIfFound<L2LoginUI>(builder);
        RegisterIfFound<LoginWindow>(builder);
        RegisterIfFound<LicenseWindow>(builder);
        RegisterIfFound<ServerSelectWindow>(builder);
        RegisterIfFound<CharSelectWindow>(builder);
        RegisterIfFound<CharCreationWindow>(builder);
        RegisterIfFound<LoginCameraManager>(builder);
        RegisterIfFound<CharacterSelector>(builder);
        RegisterIfFound<CharacterBuilder>(builder);
        RegisterIfFound<CharacterCreator>(builder);

        builder.Register<LoginRuntime>(Lifetime.Singleton);

        builder.RegisterBuildCallback(container =>
        {
            LoginRuntime loginRuntime = container.Resolve<LoginRuntime>();
            IncomingPacketActions.BindLogin(loginRuntime);
            AppLoginApplyIncomingPacketServices.BindLogin(loginRuntime);
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

    private void RegisterIfFound<T>(IContainerBuilder builder) where T : MonoBehaviour
    {
        T component = FindFirstObjectByType<T>(FindObjectsInactive.Include);
        if (component != null)
            builder.RegisterComponent(component);
    }
}
