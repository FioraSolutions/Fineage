using System;

public partial class CharacterBuilder : MonoBehaviour
{
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
        Transform container = ResolveModelContainer(entity, entityType);

        SpawnPiece(Models.GetFace(raceId, appearance.FaceByte), container);
        SpawnPiece(Models.GetHair(raceId, appearance.HairStyleByte, appearance.HairColorByte, false), container);
        SpawnPiece(Models.GetHair(raceId, appearance.HairStyleByte, appearance.HairColorByte, true), container);

        return entity;
    }

    static Transform ResolveModelContainer(GameObject entity, EntityType entityType)
    {
        if (entity == null)
            return null;
        Transform model = entity.transform.Find("model");
        if (model != null)
            return model;
        // Player_MOrc (and Human clones) have no model child — gear/face live on the root
        // so SkinnedMeshSync can see them as siblings of the anim hierarchy.
        if (entityType == EntityType.Player)
            return entity.transform;
        return entity.transform.childCount > 0 ? entity.transform.GetChild(0) : entity.transform;
    }

    static GameObject SpawnPiece(GameObject source, Transform parent)
    {
        if (source == null || parent == null)
            return null;
        return Instantiate(source, parent, false);
    }
}
