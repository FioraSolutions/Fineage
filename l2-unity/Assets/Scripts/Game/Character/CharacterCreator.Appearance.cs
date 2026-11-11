using System;
using Object = UnityEngine.Object;
using UnityEngine;
using System.Collections.Generic;
using VContainer;

public partial class CharacterCreator
{
    public void ReBuildFace(CharacterRaceAnimation raceId, byte _face)
    {
        GameObject pawnObject = pawns[currentPawnIndex];
        if (pawnObject != null)
        {
            UserGear gear = pawnObject.GetComponent<UserGear>();
            // Parent immediately — unparented Instantiate lands in the active scene
            // (can be Game) and survives Menu unload as a floating mesh.
            GameObject face = Instantiate(Models.GetFace(raceId, _face), pawnObject.transform, false);
            gear.EquipFace(face);
            LobbyPresentation.ApplyLayer(pawnObject);
        }
    }

    public void ReBuildHair(CharacterRaceAnimation raceId, byte hairColor, byte hairStyle)
    {
        GameObject pawnObject = pawns[currentPawnIndex];

        if (pawnObject != null)
        {
            var hair1M = Models.GetHair(raceId, hairStyle, hairColor, false);
            var hair2M = Models.GetHair(raceId, hairStyle, hairColor, true);
            if (hair1M != null || hair2M != null)
            {
                GameObject hair1 = hair1M != null
                    ? Instantiate(hair1M, pawnObject.transform, false)
                    : null;
                GameObject hair2 = hair2M != null
                    ? Instantiate(hair2M, pawnObject.transform, false)
                    : null;
                UserGear gear = pawnObject.GetComponent<UserGear>();
                gear.EquipHair(hair1, hair2);
                LobbyPresentation.ApplyLayer(pawnObject);
            }

        }
    }

}
