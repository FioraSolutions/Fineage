using System;
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
            RequireMotionServices(container);
            LoginRuntime loginRuntime = container.Resolve<LoginRuntime>();
            AppLoginSessionApplyIncomingPacketServices.BindLogin(loginRuntime);
            if (App.Container != null)
            {
                try
                {
                    ILobbySession lobbySession = App.Container.Resolve<ILobbySession>();
                    LobbyFlowController lobby = lobbySession as LobbyFlowController
                        ?? App.Container.Resolve<LobbyFlowController>();
                    lobby.WireLogin(loginRuntime);
                    GameManager manager = App.Container.Resolve<GameManager>();
                    L2LoginUI loginUi = loginRuntime.Ui;
                    if (loginUi != null)
                        manager.WireLoginUi(loginUi);
                    else
                        Debug.LogError("[LoginLifetimeScope] L2LoginUI missing — LoginRuntime.Ui was not registered, so GameManager has no login loading UI.");
                }
                catch (System.Exception ex)
                {
                    Debug.LogError(
                        "[LoginLifetimeScope] Failed to wire login session from App.Container: "
                        + ex.Message);
                }
            }
            else
            {
                Debug.LogWarning(
                    "[LoginLifetimeScope] App.Container missing — lobby wire skipped (AppLifetimeScope not ready)");
            }
        });
    }

    static void RequireMotionServices(IObjectResolver container)
    {
        IAnimationManager animations;
        try
        {
            animations = container.Resolve<IAnimationManager>();
        }
        catch (VContainerException ex)
        {
            if (ex.InvalidType == typeof(IAnimationManager))
            {
                throw new InvalidOperationException(
                    "[LoginLifetimeScope] IAnimationManager missing — AppLifetimeScope did not register AnimationManager.",
                    ex);
            }

            throw;
        }

        if (animations is not AnimationManager)
        {
            throw new InvalidOperationException(
                "[LoginLifetimeScope] IAnimationManager is not an AnimationManager — AppLifetimeScope did not register AnimationManager.");
        }

        IMoveRoster moves;
        try
        {
            moves = container.Resolve<IMoveRoster>();
        }
        catch (VContainerException ex)
        {
            if (ex.InvalidType == typeof(IMoveRoster))
            {
                throw new InvalidOperationException(
                    "[LoginLifetimeScope] IMoveRoster missing — AppLifetimeScope did not register SharedMoveRoster.",
                    ex);
            }

            throw;
        }

        if (moves is not SharedMoveRoster)
        {
            throw new InvalidOperationException(
                "[LoginLifetimeScope] IMoveRoster is not a SharedMoveRoster — AppLifetimeScope did not register SharedMoveRoster.");
        }
    }

}
