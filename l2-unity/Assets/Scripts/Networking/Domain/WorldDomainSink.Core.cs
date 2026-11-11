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

        // Intention/Player still consume CharMoveToLocationDto; synthesize when v2 has none.
        CharMoveToLocationDto dto = move.InterludeDto;
        if (dto == null)
            dto = CharMoveToLocationDto.FromUnity(move.ObjectId, move.Destination, move.Origin);

        _worldApply.MoveTo(move.ObjectId, move.Destination, move.Origin, dto);
        _gameEvents?.Publish(new PlayerMovedEvent(move.ObjectId, move.Destination, move.Origin));
    }

    public void ApplyPositionCorrection(int objectId, Vector3 position)
    {
        if (_worldApply == null)
            return;
        _worldApply.ApplyPositionCorrection(objectId, position);
    }

    public void ApplyStopMove(DomainStopMove stop)
    {
        if (stop == null || _worldApply == null)
            return;
        if (stop.InterludeDto != null)
            _worldApply.StopMove(stop.InterludeDto);
    }
}
