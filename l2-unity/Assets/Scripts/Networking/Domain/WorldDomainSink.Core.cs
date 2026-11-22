using UnityEngine;

public sealed partial class WorldDomainSink : IWorldDomainSink
{
    public WorldDomainSink(WorldPacketApply worldApply, World world, IGameEventBus gameEvents)
    {
        _worldApply = worldApply;
        _world = world;
        _gameEvents = gameEvents;
    }

    public void ApplyMoveTo(DomainMoveTo move)
    {
        if (move == null || _worldApply == null)
            return;

        // NewMoveToIntention casts this payload and returns when it is null.
        CharMoveToLocationDto dto = CharMoveToLocationDto.FromUnity(
            move.ObjectId, move.Destination, move.Origin);

        _worldApply.MoveTo(move.ObjectId, move.Destination, move.Origin, dto);
        _gameEvents?.Publish(new PlayerMovedEvent(move.ObjectId, move.Destination, move.Origin));
    }

    public void ApplyPositionCorrection(int objectId, Vector3 position, int heading = -1)
    {
        if (_worldApply == null)
            return;
        _worldApply.ApplyPositionCorrection(objectId, position, heading);
    }

    public void ApplyStopMove(DomainStopMove stop)
    {
        if (stop == null || _worldApply == null)
            return;
        _worldApply.StopMove(StopMoveDto.FromUnity(stop.ObjectId, stop.StopPos, stop.Heading));
    }
}
