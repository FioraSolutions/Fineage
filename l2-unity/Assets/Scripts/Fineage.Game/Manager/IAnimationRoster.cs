/// <summary>
/// Animation roster <c>World.UnregisterObject</c> drops when an actor leaves (#131 S38f).
/// The rest of <c>IAnimationManager</c> stays Assembly-CSharp.
/// </summary>
public interface IAnimationRoster
{
    void UnregisterController(int objectId);
}
