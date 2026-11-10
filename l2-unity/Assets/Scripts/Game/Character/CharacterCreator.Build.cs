using System;
using Object = UnityEngine.Object;

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
            CharacterDefaultEquipment.EquipStarterGear(gear, appearance);
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

    public void UpdatePawnPosAndRot(GameObject pawnObject, Logongrp pawnData) {
        Vector3 pawnPosition = new Vector3(pawnData.X, pawnData.Y, pawnData.Z);
        pawnPosition = VectorUtils.ConvertPosToUnity(pawnPosition);
        pawnObject.transform.position = pawnPosition;
        pawnObject.transform.eulerAngles = new Vector3(0, 360.00f * pawnData.Yaw / 65536, 0);
        // Pin the slot as destination so a pawn never slides back to its Instantiate origin.
        SelectableCharacterEntity selectable = pawnObject.GetComponent<SelectableCharacterEntity>();
        if (selectable != null)
            selectable.SetDestination(pawnData);
    }

    public void RotatePawn(bool right) {
        _pawnRotating = true;
        _pawnRotatingRight = right;
    }

    public void StopRotatingPawn() {
        _pawnRotating = false;
    }

    public GameObject FallbackPawn() {
        return CreatePawn(CharacterRaceAnimation.FFighter, new PlayerAppearance());
    }

}
