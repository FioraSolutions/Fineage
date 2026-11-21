/// <summary>Priority FX owners: local player, current target, party (step 17).</summary>
public static class FxBudgetPriority
{
    public static bool IsHighPriorityCaster(int casterEntityId, int skillTargetObjectId)
    {
        if (casterEntityId <= 0)
            return true;

        PlayerEntity local = DomainPacketServices.LocalPlayer;
        if (local == null || local.Identity == null)
            return true;

        int localId = local.Identity.Id;
        if (casterEntityId == localId)
            return true;

        if (skillTargetObjectId > 0 && skillTargetObjectId == localId)
            return true;

        if (local.TargetId > 0 && casterEntityId == local.TargetId)
            return true;

        if (skillTargetObjectId > 0 && skillTargetObjectId == local.TargetId)
            return true;

        return false;
    }
}