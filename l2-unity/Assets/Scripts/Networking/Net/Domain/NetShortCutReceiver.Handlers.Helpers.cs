using System;
using System.Collections.Generic;
using Fineage.Net.Contract;
using UnityEngine;

public static partial class NetShortCutReceiver
{
    static Shortcut ToShortcut(ShortCutNetMsg m)
    {
        if (m == null)
            return null;

        int worldSlot = m.Slot;
        if (worldSlot < 0)
            return null;

        int slot = worldSlot % SlotsPerPage;
        int page = worldSlot / SlotsPerPage;
        return new Shortcut(slot, page, m.Type, m.Id, m.Level);
    }

    static PlayerShortcuts Shortcuts()
    {
        try
        {
            return LocatorApplyIncomingPacketServices.Shortcuts;
        }
        catch (System.Exception)
        {
            // The locator throws until the skillbar is registered on the DI scope.
            return null;
        }
    }

    static void WarnNoSkillbar()
    {
        if (_warnedNoSkillbar)
            return;
        _warnedNoSkillbar = true;
        Debug.LogWarning("[Net] hotbar packet arrived before the skillbar was ready; queuing until slots exist");
    }
}
