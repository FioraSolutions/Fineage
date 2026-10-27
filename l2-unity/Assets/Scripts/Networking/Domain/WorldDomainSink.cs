/// <summary>
/// First consumer of domain events: maps into the existing Interlude-shaped apply path.
/// </summary>
public sealed class WorldDomainSink : IWorldDomainSink
{
    private readonly WorldPacketApply _worldApply;
    private readonly World _world;
    private readonly IGameEventBus _gameEvents;

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

    public void ApplyStopMove(DomainStopMove stop)
    {
        if (stop == null || _worldApply == null)
            return;
        if (stop.InterludeDto != null)
            _worldApply.StopMove(stop.InterludeDto);
    }

    public void ApplyKnownlistAdd(DomainKnownlistAdd add)
    {
        if (add == null || _worldApply == null)
            return;

        if (add.Kind == KnownlistKind.User && add.InterludeChar != null)
            _worldApply.UpdateUser(add.InterludeChar);
        else if (add.Kind == KnownlistKind.Npc && add.InterludeNpc != null)
            _worldApply.UpdateNpc(add.InterludeNpc);
    }

    public void ApplyPlayerSpawned(DomainPlayerSpawned spawned)
    {
        if (spawned == null || _world == null)
            return;
        if (spawned.Identity == null)
            return;
        _world.SpawnPlayer(spawned.Identity, spawned.Status, spawned.Stats, spawned.Appearance);
        _gameEvents?.Publish(new EntitySpawnedEvent(spawned.Identity.Id));
    }

    public void ApplyStatsChanged(DomainStatsChanged changed)
    {
        if (changed == null || changed.InterludeUserInfo == null || _world == null)
            return;

        UserInfoDto userInfo = changed.InterludeUserInfo;
        _world.UserInfoUpdateCharacter(userInfo);
        if (PlayerEntity.Instance != null)
            _world.UpdateUserInfo(PlayerEntity.Instance, userInfo);
    }

    public void FlushKnownlist()
    {
        if (_worldApply != null)
            _worldApply.FlushKnownlist();
    }
}
