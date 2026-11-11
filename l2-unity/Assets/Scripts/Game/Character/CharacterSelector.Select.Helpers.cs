using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using Object = UnityEngine.Object;

public partial class CharacterSelector
{
    public void ConfirmSelection()
     {
        if (SelectedSlot == -1) {
            // Debug.LogWarning("Please select a character");
            return;
        }
        IZoneTransport game = ZoneCommands.Transport;
        if (game != null)
            ZoneCommands.Send(new CharacterSelectCommand(SelectedSlot));
       // GameClient.Instance.ClientPacketHandler.SendRequestSelectCharacter(SelectedSlot);
    }

    public void TryToDeleteCharacter()
    {
          if (SelectedSlot == -1)
          {
                // Debug.LogWarning("Please select a character");
                return;
            }
            IZoneTransport game = ZoneCommands.Transport;
            if (game != null)
                ZoneCommands.Send(new CharacterDeleteCommand(SelectedSlot));
            // Debug.LogWarning("Requesting delete character , slot: " + SelectedSlot);
    }

    void Update()
    {
        if (!PollInput.LeftMouseDown || _charSelectCamera == null)
            return;

        // CharSelectWindow marks layout spacers Ignore; real chrome stays pickable.
        L2LoginUI loginUi = LoginPacketServices.Ui;
        if (loginUi != null && loginUi.IsPointerOverPanel())
            return;

        Ray ray = _charSelectCamera.ScreenPointToRay(PollInput.MousePosition);
        // EntityClick only: pawn CharacterControllers / stage colliders must not swallow the hit.
        if (!Physics.Raycast(ray, out RaycastHit hit, 1000f, _characterMask, QueryTriggerInteraction.Collide))
            return;

        SelectableCharacterEntity entity = hit.collider.GetComponentInParent<SelectableCharacterEntity>();
        if (entity == null || entity.CharacterInfoInterlude == null)
            return;

        if (entity.CharacterInfoInterlude.Slot == _selectedCharacterSlot)
        {
            entity.TryPlayVictory();
            return;
        }

        if (_lobbyMode)
        {
            int index = IndexOfLobbySlot(entity.CharacterInfoInterlude.Slot);
            if (index >= 0)
                SelectLobbyCharacter(index);
        }
        else
        {
            SelectInterludeCharacter(entity.CharacterInfoInterlude.Slot);
        }
    }
}
