using Fineage.Net.Contract;
using UnityEngine;
using System;

public sealed partial class FineageZoneTransport
{
    bool TrySendInteractionCommands(INetworkCommand command)
    {
        RequestSay2Command netSay = command as RequestSay2Command;
        if (netSay != null)
        {
            int chatType = netSay.Data != null ? netSay.Data.Type : NetChat.All;
            Session.SendSay(netSay.Message, chatType);
            return true;
        }

        RequestMagicSkillUseCommand netSkill = command as RequestMagicSkillUseCommand;
        if (netSkill != null)
        {
            // Mobius Player.useMagic: the caster stops the moment the cast is ordered, before
            // the zone answers with MagicSkillUse. Waiting for the reply let the client walk a
            // few metres past the cast origin, then PositionCorrection snapped it back.
            // A skill still on reuse is refused by the zone without touching movement (Mobius: SystemMessage +
            // ActionFailed only), so do not stop the run for it: the server keeps the hero moving and a local halt
            // left it running on the spot until the next correction teleported it forward.
            if (!LocalSkillReuse.IsOnReuse(netSkill.SkillId))
                LocalCastStop.Halt();
            int targetId = ResolveSelectedTargetObjectId();
            bool ctrl = netSkill.CtrlPressed != 0;
            Session.SendUseSkill(netSkill.SkillId, targetId, ctrl);
            return true;
        }

        RequestRestartPointCommand netRevive = command as RequestRestartPointCommand;
        if (netRevive != null)
        {
            Session.SendReviveRequest(netRevive.Action);
            return true;
        }

        // Mobius trainer window: RequestAcquireSkillInfo / RequestAcquireSkill go straight to the zone.
        RequestAcquireSkillInfoCommand netSkillInfo = command as RequestAcquireSkillInfoCommand;
        if (netSkillInfo != null)
        {
            Session.SendRequestAcquireSkillInfo(
                netSkillInfo.SkillId, netSkillInfo.SkillLevel, netSkillInfo.SkillType);
            return true;
        }

        RequestAcquireSkillCommand netSkillLearn = command as RequestAcquireSkillCommand;
        if (netSkillLearn != null)
        {
            Session.SendRequestAcquireSkill(
                netSkillLearn.SkillId, netSkillLearn.SkillLevel, netSkillLearn.SkillType);
            return true;
        }

        RequestBypassToServerCommand netBypass = command as RequestBypassToServerCommand;
        if (netBypass != null)
        {
            string bypass = netBypass.Bypass ?? "";
            // Mobius admin_* (//gmshop, //give, HTML admin_buy): no NPC — zone gates by KuK + FINEAGE_ZONE_ADMIN.
            if (IsAdminBypass(bypass))
            {
                // //give → admin_give; zone also accepts admin_create_item.
                if (NetGiveCommand.TryParse(bypass, out int giveItemId, out int giveCount))
                    bypass = "admin_create_item " + giveItemId + " " + giveCount;
                Session.SendNpcLink(0, bypass);
                return true;
            }

            int npcId = NetNpcSession.ActiveNpcObjectId;
            if (npcId > 0)
                Session.SendNpcLink(npcId, bypass);
            return true;
        }

        return false;
    }
}

