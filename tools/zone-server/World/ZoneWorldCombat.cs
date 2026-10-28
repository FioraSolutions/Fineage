using Fineage.ZoneServer.Combat;
using Fineage.ZoneServer.Session;
using Fineage.ZoneServer.Wire;

namespace Fineage.ZoneServer.World;

public sealed partial class ZoneWorld
{
    public const int SwordMobObjectId = 20_003;
    const int SwordMobTemplateId = 30_006;
    public const int SwordMobMaxHp = 96;

    public bool TryGetEntity(int objectId, out ZoneEntity? entity)
    {
        lock (_lock)
            return _entities.TryGetValue(objectId, out entity);
    }

    public async Task<bool> TrySelectTargetAsync(
        PlayerSession session,
        int playerObjectId,
        int targetObjectId,
        CancellationToken ct)
    {
        if (!TryGetEntity(playerObjectId, out ZoneEntity? player) || player == null)
            return false;
        if (!TryGetEntity(targetObjectId, out ZoneEntity? target) || target == null)
            return false;
        if (!target.IsAliveNpc)
            return false;

        if (!IsVisibleTo(playerObjectId, targetObjectId))
            return false;

        session.SelectedTargetId = targetObjectId;
        await session.SendWireAsync(MsgType.MyTargetSelected, new MyTargetSelectedMsg
        {
            ObjectId = targetObjectId,
            ColorHint = InterludeStatus.TargetColorHostile
        }, ct).ConfigureAwait(false);
        return true;
    }

    public async Task<bool> TryMeleeAttackAsync(PlayerSession session, int playerObjectId, CancellationToken ct)
    {
        int targetId = session.SelectedTargetId;
        if (targetId <= 0)
            return false;

        if (!TryGetEntity(playerObjectId, out ZoneEntity? player) || player == null)
            return false;
        if (!TryGetEntity(targetId, out ZoneEntity? target) || target == null || !target.IsAliveNpc)
            return false;

        if (!ZoneCombat.InMeleeRange(player.X, player.Y, target.X, target.Y))
            return false;

        int damage = ZoneCombat.SampleMeleeDamage;
        target.CurHp = Math.Max(0, target.CurHp - damage);

        var attack = new AttackMsg
        {
            AttackerObjectId = playerObjectId,
            TargetObjectId = targetId,
            Damage = damage,
            Flags = 0,
            AttackerX = player.X,
            AttackerY = player.Y,
            AttackerZ = player.Z,
            TargetX = target.X,
            TargetY = target.Y,
            TargetZ = target.Z
        };

        var status = new StatusUpdateMsg
        {
            ObjectId = targetId,
            AttributeIds = new[] { InterludeStatus.CurHp, InterludeStatus.MaxHp },
            Values = new[] { target.CurHp, target.MaxHp }
        };

        await BroadcastToInterestedAsync(playerObjectId, targetId, MsgType.Attack, attack, ct).ConfigureAwait(false);
        await BroadcastToInterestedAsync(playerObjectId, targetId, MsgType.StatusUpdate, status, ct).ConfigureAwait(false);
        return true;
    }

    bool IsVisibleTo(int observerId, int otherId)
    {
        lock (_lock)
            return _aoi.GetVisible(observerId).Contains(otherId);
    }

    async Task BroadcastToInterestedAsync(
        int playerObjectId,
        int targetObjectId,
        MsgType type,
        object msg,
        CancellationToken ct)
    {
        var recipients = new HashSet<int> { playerObjectId };
        lock (_lock)
        {
            foreach (int id in _aoi.GetVisible(playerObjectId))
                recipients.Add(id);
            foreach (int id in _aoi.GetVisible(targetObjectId))
                recipients.Add(id);
        }

        foreach (int id in recipients)
        {
            if (!_playersByObjectId.ContainsKey(id))
                continue;
            await NotifyPlayerAsync(id, type, msg, ct).ConfigureAwait(false);
        }
    }

    void SeedSwordMobNearSpawn()
    {
        AddStaticNpc(SwordMobObjectId, SwordMobTemplateId, "Gremlin",
            PlayerSession.SpawnX + 280, PlayerSession.SpawnY + 280, PlayerSession.SpawnZ,
            SwordMobMaxHp);
    }
}
