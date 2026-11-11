using System.Collections;
using UnityEngine;

public sealed partial class PetSummonRise : MonoBehaviour
{
    public static bool IsRising(GameObject go)
    {
        if (go == null)
            return false;
        PetSummonRise rise = go.GetComponent<PetSummonRise>();
        return rise != null && rise._active;
    }

    public static void TryBegin(Entity entity)
    {
        if (entity == null || entity.Identity == null || entity.Identity.SummonState != 2)
            return;
        if (!TablePacketServices.EnterEvents.TryGet(entity.Identity.NpcId, out EnterEventTable.Row row))
            return;
        if (row.SpawnType == 0)
            return;

        PetSummonRise rise = entity.GetComponent<PetSummonRise>();
        if (rise == null)
            rise = entity.gameObject.AddComponent<PetSummonRise>();
        if (rise._active)
            return;

        rise.Begin(entity, row);
    }
}
