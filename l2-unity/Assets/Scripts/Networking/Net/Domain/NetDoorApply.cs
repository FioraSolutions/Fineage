using Fineage.Net.Contract;

/// <summary>Applies <see cref="DoorStatusNetMsg"/> to door entities and static meshes.</summary>
public static class NetDoorApply
{
    public static void Apply(DoorStatusNetMsg msg)
    {
        if (msg == null)
            return;

        NetDoorSession.NoteStatus(msg);
        NetNpcSession.RememberNpc(msg.ObjectId, "", msg.Attackable);

        string scope = msg.CastleId > 0
            ? $"castleId={msg.CastleId}"
            : msg.HallId > 0
                ? $"hallId={msg.HallId}"
                : "no castle/hall";

        if (msg.CastleId <= 0 && msg.HallId <= 0)
            FineageLog.Warn(FineageLogCategory.Net, $"DoorStatus: objectId={msg.ObjectId} missing castleId/hallId");

        QueueApplyIncomingPacketServices.Queue(() =>
        {
            World world = WorldApplyIncomingPacketServices.GameWorld;
            if (world == null)
            {
                FineageLog.Error(FineageLogCategory.Net, "DoorStatus: GameWorld is null.");
                return;
            }

            var entity = world.GetEntityNoLockSync(msg.ObjectId);
            if (entity == null)
                return;

            var visual = entity.GetComponent<DoorNetVisual>();
            if (visual == null)
            {
                FineageLog.Error(
                    FineageLogCategory.Net,
                    $"DoorStatus: entity objectId={msg.ObjectId} ({scope}) has no DoorNetVisual.");
                return;
            }

            if (visual.DoorId != msg.DoorId)
                visual.BindByDoorId(msg.DoorId);
            visual.ApplyStatus(msg.Open, msg.CurHp, msg.MaxHp, msg.Attackable);

            if (entity.Status != null)
                entity.Status.SetHp(msg.CurHp);
            if (entity.Stats != null && msg.MaxHp > 0)
                entity.Stats.MaxHp = msg.MaxHp;
        });
    }
}