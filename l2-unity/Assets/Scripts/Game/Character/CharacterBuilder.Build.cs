using System;

public partial class CharacterBuilder : MonoBehaviour
{
    void Awake() {
        if (_instance == null) {
            _instance = this;
            CharacterPacketServices.Bind(builder: this);
        } else if (_instance != this) {
            Destroy(this);
        }
    }

    public GameObject BuildCharacterBase(CharacterRaceAnimation raceId, PlayerAppearance appearance, EntityType entityType) {
        GameObject prototype = Models.GetContainer(raceId, entityType);
        if (prototype == null) {
            return null;
        }

        bool reused = false;
        GameObject entity = AcquireUserContainer(prototype, entityType, out reused);
        if (entity == null)
            return null;

        Transform container = ResolveModelContainer(entity, entityType);

        UserGear gear = entity.GetComponent<UserGear>();
        PooledUserVisual visual = entity.GetComponent<PooledUserVisual>();
        if (visual == null && entityType == EntityType.User)
            visual = entity.AddComponent<PooledUserVisual>();

        if (reused && visual != null && visual.Built && gear != null)
        {
            SpawnFrameBudget.NoteReused();
            ApplyFaceHair(gear, container, raceId, appearance, visual);
            RecaptureMaterials(entity);
            return entity;
        }

        GameObject face = SpawnPiece(Models.GetFace(raceId, appearance.FaceByte), container);
        GameObject hair1 = SpawnPiece(Models.GetHair(raceId, appearance.HairStyleByte, appearance.HairColorByte, false), container);
        GameObject hair2 = SpawnPiece(Models.GetHair(raceId, appearance.HairStyleByte, appearance.HairColorByte, true), container);

        if (gear != null)
            gear.AddUserGearLink(face, hair1, hair2);

        if (visual != null)
            RememberVisual(visual, appearance);

        RecaptureMaterials(entity);
        return entity;
    }

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
