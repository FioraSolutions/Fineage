/// <summary>
/// Animation roster <c>World</c> injects and drops when an actor leaves (#131 S38f).
/// <c>AppLifetimeScope</c> registers <c>AnimationManager</c> as this interface.
/// The rest of <c>IAnimationManager</c> stays Assembly-CSharp.
/// </summary>
public interface IAnimationRoster
{
    void UnregisterController(int objectId);
}
