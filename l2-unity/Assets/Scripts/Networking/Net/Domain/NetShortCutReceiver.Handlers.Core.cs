using System;
using System.Collections.Generic;
using Fineage.Net.Contract;
using UnityEngine;

public static partial class NetShortCutReceiver
{
    static void Dispatch(Action<PlayerShortcuts> apply)
    {
        PlayerShortcuts shortcuts = Shortcuts();
        SkillbarWindow skillbar = SkillbarPacketServices.Skillbar;
        // PlayerShortcuts can exist before SkillbarWindow finishes BuildWindow — applying
        // ShortCutInit then paints nothing and FlushPending never sees a queued copy.
        if (shortcuts != null && skillbar != null && skillbar.IsReady)
        {
            apply(shortcuts);
            return;
        }

        WarnNoSkillbar();
        _pending.Add(() =>
        {
            PlayerShortcuts ready = Shortcuts();
            SkillbarWindow bar = SkillbarPacketServices.Skillbar;
            if (ready != null && bar != null && bar.IsReady)
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

        // Mobius skill 4270 tracks the load grade: 1 >=50%, 2 >=66.6%, 3 >=80%, 4 >=100%.
        // Level 0 must still be applied — BufferPanel.DeleteDataCell only runs on level 0,
        // so an early return here left the overload icon stuck after dumping weight.
        // WeightPenalty[0] is the skill id, [1] is the level the buffer reads.
        var dto = new EtcStatusUpdateDto();
        dto.WeightPenalty[1] = msg.WeightPenalty < 0 ? 0 : msg.WeightPenalty;
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

        PlayerEntity player = DomainPacketServices.LocalPlayer;
        if (player == null || player.Identity == null)
            return;
        if ((ulong)player.Identity.Id != msg.EntityValue)
            return;

        if (msg.RunSpeed > 0.001f)
            player.UpdateRunSpeed(msg.RunSpeed);
        if (msg.WalkSpeed > 0.001f)
            player.UpdateWalkSpeed(msg.WalkSpeed);
    }
}
