using System.Collections.Generic;
using UnityEngine;

public partial class CharacterAnimationAudioHandler
{
    [SerializeField] protected string _chrSoundBank;

    static readonly HashSet<string> LoggedMissingChrSound = new HashSet<string>();

    protected virtual bool TryBindChrSoundBank()
    {
        return !string.IsNullOrEmpty(_chrSoundBank);
    }

    protected bool LogMissingChrSoundOnce(int npcId, string field)
    {
        string key = npcId + "@" + field;
        if (!LoggedMissingChrSound.Add(key))
            return false;

        FineageLog.Error(
            FineageLogCategory.World,
            "AudioService: npc id=" + npcId + " missing npcgrp field \"" + field + "\" for ChrSound idle/voice");
        return true;
    }
}