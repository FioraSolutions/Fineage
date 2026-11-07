using System;
using System.Collections.Generic;
using Fineage.Net.Contract;
using UnityEngine;

/// <summary>
/// Zone messages that arrive during <c>EnterWorld</c> and were silently dropped
/// by <see cref="NetSession"/> before the contract was resynced with the zone
/// server (<c>UserInfo</c>, the hotbar trio and <c>EtcStatusUpdate</c>).
///
/// <para>
/// They are grouped here rather than inlined into the <c>NetSession</c> switch
/// because they share one trait: they arrive while the world scene and the
/// skillbar may not exist yet, and they must not be lost when they do.
/// </para>
///
/// <para>
/// The legacy <c>*Dto</c> types are <b>not</b> used as an intermediate hop. Their
/// <c>ReadFrom</c> understands the old Interlude layout (<c>ShortCutInit</c> reads
/// five extra ints and two shorts per item row), so replaying them from
/// Net-shaped data would misparse. Building <see cref="Shortcut"/> directly from
/// the Net message is the honest mapping.
/// </para>
/// </summary>
public static class NetShortCutReceiver
{
    /// <summary>
    /// The wire carries a <b>world slot</b> (page * 12 + slot), matching Mobius.
    /// <see cref="Shortcut"/> wants slot and page split apart.
    /// </summary>
    const int SlotsPerPage = 12;

    /// <summary>Highest force charge Mobius reports; guards against a malformed count.</summary>
    const int MaxCharges = 7;

    /// <summary>Announced once, so a missing skillbar is a single log line rather than per-slot spam.</summary>
    static bool _warnedNoSkillbar;

    /// <summary>
    /// <c>ShortCutInit</c> / <c>ShortCutRegister</c> / <c>ShortCutDelete</c> can land before the
    /// skillbar is registered on the DI scope. Queue them so the player's hotbar is not
    /// silently empty on those loads; <see cref="FlushPending"/> is driven from the same
    /// per-frame pump as <see cref="NetGearSession.FlushPending"/>.
    /// </summary>
    static readonly List<Action> _pending = new List<Action>();

    /// <summary>Drop queued slots. Called when a new session starts.</summary>
    public static void Reset()
    {
        _pending.Clear();
        _warnedNoSkillbar = false;
    }

    /// <summary>
    /// Replays whatever was queued while the skillbar did not exist. Safe to call every
    /// frame: it is a no-op once the queue is empty.
    /// </summary>
    public static void FlushPending()
    {
        if (_pending.Count == 0)
            return;

        PlayerShortcuts shortcuts = Shortcuts();
        if (shortcuts == null)
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
            }
        }

        _pending.Clear();
        Debug.Log("[Net] hotbar queue flushed");
    }

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

    static void Dispatch(Action<PlayerShortcuts> apply)
    {
        PlayerShortcuts shortcuts = Shortcuts();
        if (shortcuts != null)
        {
            apply(shortcuts);
            return;
        }

        WarnNoSkillbar();
        _pending.Add(() =>
        {
            PlayerShortcuts ready = Shortcuts();
            if (ready != null)
                apply(ready);
        });
    }

    public static void ApplyEtcStatus(EtcStatusUpdateNetMsg msg)
    {
        if (msg == null)
            return;

        int charges = msg.Charges;
        if (charges < 0)
            charges = 0;
        else if (charges > MaxCharges)
            charges = MaxCharges;

        if (charges > 0)
            Debug.Log("[Net] EtcStatusUpdate charges=" + charges);

        if (msg.WeightPenalty <= 0)
            return;

        // Mobius skill 4270 tracks the load grade: 1 >=50%, 2 >=66.6%, 3 >=80%, 4 >=100%.
        // Map it onto the existing penalty DTO so the status bar draws the same icon it
        // already knows, rather than adding a parallel path for one integer.
        // WeightPenalty[0] is the skill id, [1] is the level the buffer reads.
        var dto = new EtcStatusUpdateDto();
        dto.WeightPenalty[1] = msg.WeightPenalty;
        if (msg.ChatBlocked)
            dto.ChatBanned = 1;

        StatusApplyService status = ApplyServices.Status;
        if (status != null) status.ApplyEtcStatusUpdate(dto);
        else EtcStatusUpdateApplyIncomingPacketServices.ApplyEtcStatusUpdate(dto);
    }

    public static void ApplyChangeMoveType(ChangeMoveTypeNetMsg msg)
    {
        if (msg == null)
            return;

        PlayerEntity player = PlayerEntity.Instance;
        if (player == null || player.Identity == null)
            return;
        if ((ulong)player.Identity.Id != msg.EntityValue)
            return;

        if (msg.RunSpeed > 0.001f)
            player.UpdateRunSpeed(msg.RunSpeed);
        if (msg.WalkSpeed > 0.001f)
            player.UpdateWalkSpeed(msg.WalkSpeed);
    }

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
        Debug.LogWarning("[Net] hotbar packet arrived before the skillbar existed; slot(s) dropped");
    }
}
