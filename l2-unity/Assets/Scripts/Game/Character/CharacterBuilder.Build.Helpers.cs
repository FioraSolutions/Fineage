using System;

public partial class CharacterBuilder : MonoBehaviour
{
    GameObject AcquireUserContainer(GameObject prototype, EntityType entityType, out bool reused)
    {
        reused = false;
        if (entityType != EntityType.User)
            return Instantiate(prototype);

        IPoolManager pool = ObjectPoolPacketServices.Pool;
        if (pool == null)
            return Instantiate(prototype);

        // SpawnFromPool grows by one when the idle queue is empty.
        // Do not AddPrefab first: that Instantiates a spare on every take and cancels prewarm.
        GameObject pooled = pool.SpawnFromPool(ObjectType.User, prototype);
        if (pooled == null)
        {
            pool.AddPrefabToPool(ObjectType.User, prototype, 1);
            pooled = pool.SpawnFromPool(ObjectType.User, prototype);
        }
        if (pooled == null)
            return Instantiate(prototype);

        PooledUserVisual visual = pooled.GetComponent<PooledUserVisual>();
        reused = visual != null && visual.Built;
        return pooled;
    }

    static void ApplyFaceHair(
        UserGear gear,
        Transform container,
        CharacterRaceAnimation raceId,
        PlayerAppearance appearance,
        PooledUserVisual visual)
    {
        ModelTable models = TablePacketServices.Models;
        bool sameFace = visual.Face == appearance.FaceByte;
        bool sameHair = visual.HairStyle == appearance.HairStyleByte &&
            visual.HairColor == appearance.HairColorByte;
        if (sameFace && sameHair)
            return;

        if (!sameFace)
        {
            GameObject face = SpawnPiece(models.GetFace(raceId, appearance.FaceByte), container);
            if (face != null)
                gear.EquipFace(face);
        }

        if (!sameHair)
        {
            GameObject hair1 = SpawnPiece(
                models.GetHair(raceId, appearance.HairStyleByte, appearance.HairColorByte, false),
                container);
            GameObject hair2 = SpawnPiece(
                models.GetHair(raceId, appearance.HairStyleByte, appearance.HairColorByte, true),
                container);
            gear.EquipHair(hair1, hair2);
        }

        RememberVisual(visual, appearance);
    }

    static void RecaptureMaterials(GameObject entity)
    {
        if (entity == null)
            return;

        PooledInstance pooled = entity.GetComponent<PooledInstance>();
        if (pooled != null)
            pooled.RecaptureSharedMaterials();
    }
}
