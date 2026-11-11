using System;
using System.Collections.Generic;
using Fineage.Net.Contract;
using UnityEngine;

public static partial class NetShortCutReceiver
{
    public static void ApplyInit(ShortCutInitNetMsg msg)
    {
        var list = new List<Shortcut>();
        if (msg?.Shortcuts != null)
        {
            for (int i = 0; i < msg.Shortcuts.Length; i++)
            {
                Shortcut shortcut = ToShortcut(msg.Shortcuts[i]);
                if (shortcut != null)
                    list.Add(shortcut);
            }
        }

        // Init is idempotent (it replaces the whole bar), so a queued copy is fine.
        Dispatch(shortcuts =>
        {
            shortcuts.SetShortcutList(list);
            Debug.Log("[Net] ShortCutInit slots=" + list.Count);
        });
    }

    public static void ApplyRegister(ShortCutNetMsg msg)
    {
        Shortcut shortcut = ToShortcut(msg);
        if (shortcut == null)
            return;

        // Register is also idempotent: re-registering a slot overwrites it.
        Dispatch(shortcuts => shortcuts.RegisterShortcut(shortcut));
    }

    public static void ApplyDelete(ShortCutDelNetMsg msg)
    {
        if (msg == null)
            return;

        int slot = msg.Slot;

        // Delete is NOT idempotent against replay: dropping a slot twice after the queue
        // is flushed could remove a different shortcut the player has since added. Replaying
        // it in order is correct, but a stale duplicate is not, so deletes are applied
        // immediately when possible and queued (in order) when not.
        Dispatch(shortcuts => shortcuts.RemoveShotcutLocally(slot));
    }
}
