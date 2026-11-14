using System;
using Object = UnityEngine.Object;
using UnityEngine;
using System.Collections.Generic;
using VContainer;

public partial class CharacterCreator
{
    public GameObject CreatePawn(CharacterRaceAnimation raceId, PlayerAppearance appearance) {
        GameObject pawnObject = Builder.BuildCharacterBase(raceId, appearance, EntityType.Pawn);
        if (pawnObject == null) {
            // m0nster: временная заглушка, пока не реализованы все персонажи
            return null;
        }

        UserGear gear = pawnObject.GetComponent<UserGear>();

        gear.Initialize(-1, raceId);
        gear.SyncEquippedArmor(appearance);
        gear.SyncEquippedWeapons(appearance.RHand, appearance.LHand);

        return pawnObject;
    }

    public GameObject CreatePawnInterlude(CharacterRaceAnimation raceId, PlayerAppearance appearance)
    {
        GameObject pawnObject = Builder.BuildCharacterBaseInterlude(raceId, appearance, EntityType.Pawn);
        if (pawnObject == null) {
            // m0nster: временная заглушка, пока не реализованы все персонажи
            return null;
        }
        
        UserGear gear = pawnObject.GetComponent<UserGear>();
        if (gear == null)
            return pawnObject;

        // Isolate gear failures per pawn so one bad race cannot abort the lobby roster.
        try
        {
            gear.Initialize(-1, raceId);
        }
        catch (System.Exception ex)
        {
            Debug.LogException(ex);
        }

        // Armor first. A bad hair chain must not skip the body.
        try
        {
            CharacterDefaultEquipment.EquipStarterGear(gear, appearance);
        }
        catch (System.Exception ex)
        {
            Debug.LogException(ex);
        }

        try
        {
            // Face/hair after Initialize — EquipFace parents to gear container + SyncMesh (#933).
            Builder.AttachFaceAndHair(gear, raceId, appearance);
        }
        catch (System.Exception ex)
        {
            Debug.LogException(ex);
        }

        return pawnObject;
    }

    public void PlacePawn(GameObject pawnObject, Logongrp pawnData, string name, GameObject container)
    {
        UpdatePawnPosAndRot(pawnObject, pawnData);
        pawnObject.transform.name = name;
        pawnObject.transform.SetParent(container.transform, true);
        pawnObject.SetActive(true);
        LobbyPresentation.ApplyLayer(pawnObject);
        BindLobbyPawnPresentation(pawnObject);
    }
}
