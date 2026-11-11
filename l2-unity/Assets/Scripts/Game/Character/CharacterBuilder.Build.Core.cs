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
}
