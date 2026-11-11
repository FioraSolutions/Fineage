using UnityEngine;

public sealed partial class WorldDomainSink : IWorldDomainSink
{
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
        PlayerEntity local = DomainPacketServices.LocalPlayer;
        if (local != null)
            _world.UpdateUserInfo(local, userInfo);
    }

    public void FlushKnownlist()
    {
        if (_worldApply != null)
            _worldApply.FlushKnownlist();
    }
}
