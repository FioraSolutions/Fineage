/// <summary>
/// First consumer of domain events: maps into the existing Interlude-shaped apply path.
/// </summary>
public sealed class WorldDomainSink : IWorldDomainSink
{
    private readonly WorldPacketApply _worldApply;
    private readonly World _world;

    public WorldDomainSink(WorldPacketApply worldApply, World world)
    {
        _worldApply = worldApply;
        _world = world;
    }

    public void ApplyMoveTo(DomainMoveTo move)
    {
        if (move == null || _worldApply == null)
            return;

        // Intention/Player still consume CharMoveToLocationDto; v2 must supply InterludeDto
        // or a domain-native Intention payload later.
        if (move.InterludeDto == null)
            return;

        _worldApply.MoveTo(move.ObjectId, move.Destination, move.Origin, move.InterludeDto);
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
