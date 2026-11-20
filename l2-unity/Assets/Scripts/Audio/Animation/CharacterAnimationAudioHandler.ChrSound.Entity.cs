using UnityEngine;

public partial class CharacterAnimationAudioHandler
{
    void TryBindChrSoundBankFromEntity()
    {
        if (TryBindChrSoundBank())
            return;

        NpcEntity npc = GetComponent<NpcEntity>();
        Npcgrp grp = npc != null && npc.NpcData != null ? npc.NpcData.Npcgrp : null;
        if (grp == null)
            return;

        if (NpcChrSoundBankResolver.TryResolveFromDamageSounds(grp.DamageSounds, out string bank))
            _chrSoundBank = bank;
    }

    void LogUnresolvedChrSound(CharacterSoundEvent soundEvent)
    {
        Entity entity = GetComponent<Entity>();
        int npcId = entity != null && entity.Identity != null ? entity.Identity.Id : 0;
        if (!LogMissingChrSoundOnce(npcId, "damage_sound"))
            return;

        FineageLog.Error(
            FineageLogCategory.World,
            "AudioService: npc id=" + npcId + " ChrSound bank unresolved for " + soundEvent
            + " (no ChrSound.* in damage_sound)");
    }
}