using System.Collections.Generic;
using FMODUnity;

public partial class AudioService
{
    static readonly HashSet<string> LoggedMissingBanks = new HashSet<string>();

    static readonly string[] StartupBanks =
    {
        "Master",
        "Master.strings",
        "ChrSound",
        "MonSound",
        "ItemSound",
        "SkillSound",
        "StepSound",
        "AmbSound",
        "InterfaceSound",
        "Music",
    };

    void LogStartupFmodBanks()
    {
        for (int i = 0; i < StartupBanks.Length; i++)
        {
            string bank = StartupBanks[i];
            if (RuntimeManager.HasBankLoaded(bank))
                continue;

            if (LoggedMissingBanks.Add(bank))
            {
                LogAudioError(
                    "AudioService: FMOD bank not loaded at startup bank=\"" + bank + "\"");
            }
        }
    }
}