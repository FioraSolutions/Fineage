using System.Collections;
using UnityEngine;

public sealed partial class PetSummonRise : MonoBehaviour
{
    void Begin(Entity entity, EnterEventTable.Row row)
    {
        float height = L2NameplateAnchor.CollisionHeightToUnityMeters(
            entity.Appearance != null ? entity.Appearance.CollisionHeight : 0f);
        Vector3 end = entity.transform.position;
        bool moves = row.IsRise == 1 || row.IsRise == 2;
        Vector3 start = end;
        if (row.IsRise == 1)
            start = end - Vector3.up * (2f * height);
        else if (row.IsRise == 2)
            start = end + Vector3.up * (2f * height);

        Debug.Log(
            "[ENTER_EVENT] table npc=" + entity.Identity.NpcId +
            " class=" + row.EffectClass +
            " rise=" + row.IsRise +
            " anim=" + row.AnimName +
            " rows=" + TablePacketServices.EnterEvents.Count);
        SpawnGroundEffect(row.EffectClass, end, entity.transform.rotation, entity);
        PlayEnterAnim(entity, row.AnimName);
        EntitySpawnShared.LogStriderAppear("enter", entity, "asked=" + row.AnimName);

        if (!moves)
        {
            entity.Identity.SummonState = 0;
            return;
        }

        _active = true;
        SetControllersEnabled(entity.gameObject, false);
        entity.transform.position = start;
        entity.Identity.Position = start;
        StartCoroutine(Rise(entity, start, end));
    }

    IEnumerator Rise(Entity entity, Vector3 start, Vector3 end)
    {
        float elapsed = 0f;
        while (elapsed < RiseSeconds)
        {
            if (entity == null)
                yield break;

            elapsed += Time.deltaTime;
            float u = Mathf.Clamp01(elapsed / RiseSeconds);
            Vector3 pos = Vector3.Lerp(start, end, u);
            entity.transform.position = pos;
            if (entity.Identity != null)
                entity.Identity.Position = pos;
            yield return null;
        }

        if (entity != null)
        {
            entity.transform.position = end;
            if (entity.Identity != null)
            {
                entity.Identity.Position = end;
                entity.Identity.SummonState = 0;
            }
            SetControllersEnabled(entity.gameObject, true);
            EntitySpawnShared.LogStriderAppear("risen", entity);
        }

        _active = false;
    }
}
