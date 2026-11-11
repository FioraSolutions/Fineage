using VContainer;

/// <summary>
/// App/Game VContainer resolvers without ACS gameplay types (#131).
/// <see cref="App"/> keeps <c>InjectGameObject</c> (needs <c>GameLifetimeScope</c>) in ACS.
/// </summary>
public static class AppContainers
{
    public static IObjectResolver Container { get; set; }
    public static IObjectResolver GameContainer { get; set; }

    public static bool HasContainer => Container != null;

    public static T Resolve<T>()
    {
        return Container.Resolve<T>();
    }
}
