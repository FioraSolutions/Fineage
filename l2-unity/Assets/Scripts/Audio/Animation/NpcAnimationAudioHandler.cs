using UnityEngine;

public class NpcAnimationAudioHandler : CharacterAnimationAudioHandler
{
    protected override void Initialize()
    {
        base.Initialize();
        TryBindChrSoundBankFromEntity();

        NpcEntity npc = GetComponent<NpcEntity>();
        if (npc == null || npc.NpcData?.Npcgrp == null)
            return;

        if (!TryBindChrSoundBank())
        {
            int npcId = npc.Identity != null ? npc.Identity.Id : npc.NpcData.Npcgrp.NpcId;
            LogMissingChrSoundOnce(npcId, "damage_sound");
        }
    }
}