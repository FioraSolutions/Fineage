using System;

public partial class PlayerShortcuts : MonoBehaviour
{
    private void RemoveShotcutNoVisible(int slot)
    {
        //SkillbarPacketServices.Skillbar.RemoveShortcut(slot);
        _shortcuts.Remove(slot);
    }

    public void UpdatePageMapping(int skillbarIndex, int page)
    {
        _pageMap[skillbarIndex] = page;
    }

    public void AddShortcut(int slot, int id, int type , int level)
    {
        Shortcut oldShortcut = GetShortcutBySlot(slot);
        if(oldShortcut != null) {
            RemoveShotcutNoVisible(slot);
        }
        
        //GameClient.Instance.ClientPacketHandler.RequestAddShortcut(type, id, slot);
        IZoneTransport game = ZoneCommands.Transport;
        if (game != null)
            ZoneCommands.Send(new RequestShortCutRegCommand(type, slot, id, level));
    }

    public void MoveShortcut(int oldSlot, int newSlot)
    {
        Shortcut oldShortcut = GetShortcutBySlot(oldSlot);
        Shortcut newShortcut = GetShortcutBySlot(newSlot);
        RemoveShotcutNoVisible(newSlot);
        RemoveShotcutNoVisible(oldSlot);

        if (oldShortcut == null)
        {
            // Debug.LogError($"MoveShortcut. Old slot is null at {oldSlot}.");
            return;
        }


        // Debug.Log("Event Reuqets Add ShrtCut 2 ");
        IZoneTransport game = ZoneCommands.Transport;
        if (game != null)
            ZoneCommands.Send(new RequestShortCutRegCommand(oldShortcut.Type, newSlot, oldShortcut.Id, oldShortcut.Level));

        // Swap slots
        if (newShortcut != null)
        {
            if (game != null)
                ZoneCommands.Send(new RequestShortCutRegCommand(newShortcut.Type, oldSlot, newShortcut.Id, newShortcut.Level));
        }
        else
        {
            DeleteShortcut(oldSlot, true);
        }
    }

    public void DeleteShortcut(int oldSlot , bool locRemove)
    {
        if(locRemove) RemoveShotcutLocally(oldSlot);

        // Debug.Log("Нужно реализовать удаление shortcut");
        //GameClient.Instance.ClientPacketHandler.RequestRemoveShortcut(oldSlot);
        IZoneTransport game = ZoneCommands.Transport;
        if (game != null)
            ZoneCommands.Send(new RequestShortCutDelCommand(oldSlot));
    }
}
