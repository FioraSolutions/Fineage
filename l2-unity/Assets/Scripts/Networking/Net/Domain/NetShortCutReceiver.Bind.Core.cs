using System;
using System.Collections.Generic;
using Fineage.Net.Contract;
using UnityEngine;

public static partial class NetShortCutReceiver
{
    public static void Reset()
    {
        _pending.Clear();
        _warnedNoSkillbar = false;
    }

    public static void FlushPending()
    {
        if (_pending.Count == 0)
            return;

        PlayerShortcuts shortcuts = Shortcuts();
        if (shortcuts == null)
            return;

        // Skillbar BuildWindow is a coroutine — applying ShortCutInit before slots exist paints
        // nothing, then clearing the queue permanently loses the EnterWorld hotbar.
        SkillbarWindow skillbar = SkillbarPacketServices.Skillbar;
        if (skillbar == null || !skillbar.IsReady)
            return;

        for (int i = 0; i < _pending.Count; i++)
        {
            try
            {
                _pending[i]();
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning("[Net] hotbar replay failed: " + ex.Message);
                // Keep the rest of the queue; retry next frame instead of dropping slots.
                if (i > 0)
                    _pending.RemoveRange(0, i);
                return;
            }
        }

        _pending.Clear();
        Debug.Log("[Net] hotbar queue flushed");
    }
}
