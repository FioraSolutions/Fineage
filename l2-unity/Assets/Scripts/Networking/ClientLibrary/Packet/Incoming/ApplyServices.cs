/// <summary>
/// Apply services for callers that live outside the VContainer scope (#142 slice 3).
/// <see cref="NetSession"/> is injected directly; <see cref="NetGearSession"/> and
/// <see cref="NetShortCutReceiver"/> are static, so the composition root publishes the
/// same singleton instances here instead.
/// Every member is null until <see cref="Bind"/> runs, and each caller keeps the legacy
/// static facade as a fallback for that window.
/// </summary>
public static class ApplyServices
{
    public static InventoryApplyService Inventory { get; private set; }

    public static StatusApplyService Status { get; private set; }

    public static void Bind(
        InventoryApplyService inventory = null,
        StatusApplyService status = null)
    {
        if (inventory != null)
            Inventory = inventory;
        if (status != null)
            Status = status;
    }

    /// <summary>Drops the published instances on teardown so a stale session cannot apply.</summary>
    public static void Reset()
    {
        Inventory = null;
        Status = null;
    }
}
