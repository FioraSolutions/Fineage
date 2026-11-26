using System;

/// <summary>
/// One <see cref="IMoveRoster"/> for the lobby and the game.
/// <see cref="AppLifetimeScope"/> registers it. <see cref="GameLifetimeScope"/> binds the scene
/// <see cref="MoveAllCharacters"/> into it when the world loads.
/// </summary>
public sealed class SharedMoveRoster : IMoveRoster
{
    MoveAllCharacters _scene;

    public bool HasScene => _scene != null;

    public void BindScene(MoveAllCharacters scene)
    {
        if (scene == null)
        {
            throw new InvalidOperationException(
                "[SharedMoveRoster] IMoveRoster missing — GameLifetimeScope passed a null MoveAllCharacters.");
        }

        _scene = scene;
    }

    public bool IsMoving(int objId) => Scene().IsMoving(objId);

    public void Forget(int objId) => Scene().Forget(objId);

    public void ReplayUserGait(int objId) => Scene().ReplayUserGait(objId);

    MoveAllCharacters Scene()
    {
        if (_scene == null)
        {
            throw new InvalidOperationException(
                "[SharedMoveRoster] IMoveRoster missing — MoveAllCharacters was not bound. GameLifetimeScope did not find it in loaded scenes.");
        }

        return _scene;
    }
}
