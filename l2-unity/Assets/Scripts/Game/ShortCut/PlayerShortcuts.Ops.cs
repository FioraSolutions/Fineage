using System;

public partial class PlayerShortcuts : MonoBehaviour
{
    public void SetShortcutList(List<Shortcut> shortcuts)
    {
        if (_shortcuts == null)
        {
            _shortcuts = new Dictionary<int, Shortcut>();
        }
        else
        {
            _shortcuts.Clear();
        }

        for (int i = 0; i < shortcuts.Count; i++)
        {
            Shortcut shortcut = shortcuts[i];
            _shortcuts[shortcut.Slot + shortcut.Page * MAXIMUM_SHORTCUTS_PER_BAR] = shortcut;
        }

        // Always keep the dict; paint only when the skillbar slots exist (EnterWorld races BuildWindow).
        SkillbarWindow skillbar = SkillbarPacketServices.Skillbar;
        if (skillbar != null && skillbar.IsReady)
            skillbar.UpdateAllShortcuts(shortcuts);
    }

    public void RegisterShortcut(Shortcut shortcut)
    {
        if (_shortcuts == null)
        {
            _shortcuts = new Dictionary<int, Shortcut>();
        }

        int slot = shortcut.Slot + shortcut.Page * MAXIMUM_SHORTCUTS_PER_BAR;
        // Server ShortCutRegister is authoritative: overwrite a stale local row instead of
        // silently dropping the echo (TryAdd failed left soulshot / item icons blank).
        _shortcuts[slot] = shortcut;
        SkillbarWindow skillbar = SkillbarPacketServices.Skillbar;
        if (skillbar != null && skillbar.IsReady)
            skillbar.AddShortcut(shortcut);
    }

    public Shortcut GetShortcutBySlot(int slot)
    {
        if (_shortcuts.TryGetValue(slot, out Shortcut shortcut))
        {
            return shortcut;
        }

        return null;
    }

    public void RemoveShotcutLocally(int slot)
    {
        SkillbarWindow skillbar = SkillbarPacketServices.Skillbar;
        if (skillbar != null && skillbar.IsReady)
            skillbar.RemoveShortcut(slot);
        _shortcuts.Remove(slot);
    }

    private void RemoveShotcutNoVisible(int slot)
    {
        //SkillbarPacketServices.Skillbar.RemoveShortcut(slot);
        _shortcuts.Remove(slot);
    }


    public void UpdatePageMapping(int skillbarIndex, int page)
    {
        _pageMap[skillbarIndex] = page;
    }

    #region ShortcutClientRequests
    // Shortcut dragged onto skillbar
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

    // Shortcut dragged within bar
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

    // Shortcut dragged out of bar
    public void DeleteShortcut(int oldSlot , bool locRemove)
    {
        if(locRemove) RemoveShotcutLocally(oldSlot);

        // Debug.Log("Нужно реализовать удаление shortcut");
        //GameClient.Instance.ClientPacketHandler.RequestRemoveShortcut(oldSlot);
        IZoneTransport game = ZoneCommands.Transport;
        if (game != null)
            ZoneCommands.Send(new RequestShortCutDelCommand(oldSlot));
    }

    #endregion
}
