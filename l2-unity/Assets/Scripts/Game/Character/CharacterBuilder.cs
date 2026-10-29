using UnityEngine;
using VContainer;

public class CharacterBuilder : MonoBehaviour
{
    private static CharacterBuilder _instance;
    public static CharacterBuilder Instance { get { return _instance; } }

    [Inject] private ModelTable _models;

    private ModelTable Models
    {
        get { return _models != null ? _models : ModelTable.Instance; }
    }

    void Awake() {
        if (_instance == null) {
            _instance = this;
        } else if (_instance != this) {
            Destroy(this);
        }
    }

    // Load player animations, face and hair
    public GameObject BuildCharacterBase(CharacterRaceAnimation raceId, PlayerAppearance appearance, EntityType entityType) {
        GameObject prototype = Models.GetContainer(raceId, entityType);
        if (prototype == null) {
            return null;
        }

        bool reused = false;
        GameObject entity = AcquireUserContainer(prototype, entityType, out reused);
        if (entity == null)
            return null;

        Transform container = entityType != EntityType.Player && entity.transform.childCount > 0 ?
            entity.transform.GetChild(0) :
            entity.transform;

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

        IPoolManager pool = ObjectPoolManager.Instance;
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
        ModelTable models = ModelTable.Instance;
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

    static void RememberVisual(PooledUserVisual visual, PlayerAppearance appearance)
    {
        visual.Built = true;
        visual.Face = appearance.FaceByte;
        visual.HairStyle = appearance.HairStyleByte;
        visual.HairColor = appearance.HairColorByte;
    }

    public GameObject ReplaceNewPawnFace(CharacterRaceAnimation raceId , byte _face , byte hairColor , byte hairStyle) {
        GameObject entity = Instantiate(Models.GetContainer(raceId, EntityType.Pawn));

        Transform container = entity.transform;
        SpawnPiece(Models.GetFace(raceId, _face), container);
        SpawnPiece(Models.GetHair(raceId, hairStyle, hairColor, false), container);
        SpawnPiece(Models.GetHair(raceId, hairStyle, hairColor, true), container);

        return entity;
    }

    public GameObject BuildCharacterBaseInterlude(CharacterRaceAnimation raceId, PlayerAppearance appearance, EntityType entityType) {
        GameObject prototype = Models.GetContainer(raceId, entityType);
        if (prototype == null) {
            return null;
        }

        GameObject entity = Instantiate(prototype);
        Transform container = entityType != EntityType.Player ?
            entity.transform.GetChild(0) :
            entity.transform;

        SpawnPiece(Models.GetFace(raceId, appearance.FaceByte), container);
        SpawnPiece(Models.GetHair(raceId, appearance.HairStyleByte, appearance.HairColorByte, false), container);
        SpawnPiece(Models.GetHair(raceId, appearance.HairStyleByte, appearance.HairColorByte, true), container);

        return entity;
    }

    static GameObject SpawnPiece(GameObject source, Transform parent)
    {
        if (source == null || parent == null)
            return null;
        return Instantiate(source, parent, false);
    }


}
