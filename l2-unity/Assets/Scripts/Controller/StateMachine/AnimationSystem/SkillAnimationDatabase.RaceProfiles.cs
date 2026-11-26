using System;

/// <summary>Shared race → Resources clip folder / mesh stem for SpAtk and override slots (#1620).</summary>
public partial class SkillAnimationDatabase
{
    internal sealed class RaceClipProfile
    {
        public readonly string Token;
        public readonly string Folder;
        public readonly string MeshStem;
        public readonly string Race;
        public readonly string AlternateMeshStem;

        public RaceClipProfile(
            string token,
            string folder,
            string meshStem,
            string race,
            string alternateMeshStem = null)
        {
            Token = token;
            Folder = folder;
            MeshStem = meshStem;
            Race = race;
            AlternateMeshStem = alternateMeshStem;
        }
    }

    /// <summary>Longer tokens first (FDarkElf before FElf).</summary>
    internal static readonly RaceClipProfile[] RaceClipTable =
    {
        new RaceClipProfile(FDarkElf, FOLDER_FDarkElf, "FDarkElf_m000_b", FDarkElf, "FDarkElf_m001_b"),
        new RaceClipProfile(MDarkElf, FOLDER_MDarkElf, "MDarkElf_m000_b", MDarkElf),
        new RaceClipProfile(FShaman, FOLDER_FShaman, "FShaman_m000_b", FShaman),
        new RaceClipProfile(MShaman, FOLDER_MShaman, "MShaman_m000_b", MShaman),
        new RaceClipProfile(FFighter, FOLDER_FFighter, "FFighter_m000_b", FFighter),
        new RaceClipProfile(MFighter, FOLDER_MFighter, "MFighter_m000_b", MFighter),
        new RaceClipProfile(FMagic, FOLDER_FMagic, "FMagic_m000_b", FMagic),
        new RaceClipProfile(MMagic, FOLDER_MMagic, "MMagic_M000_b", MMagic),
        new RaceClipProfile(FElf, FOLDER_FElf, "FElf_m000_b", FElf),
        new RaceClipProfile(MElf, FOLDER_MElf, "MElf_m000_b", MElf),
        new RaceClipProfile(FDwarf, FOLDER_FDwarf, "FDwarf_m001_b", FDwarf),
        new RaceClipProfile(MDwarf, FOLDER_MDwarf, "MDwarf_m000_b", MDwarf),
        new RaceClipProfile(FOrc, FOLDER_FOrc, "FOrc_m000_b", FOrc),
        new RaceClipProfile(MOrc, FOLDER_MOrc, "MOrc_m000_b", MOrc),
    };

    internal static readonly string[] AllRaceTokens =
    {
        MFighter, FFighter, MMagic, FMagic, MElf, FElf, MDarkElf, FDarkElf,
        MDwarf, FDwarf, MOrc, FOrc, MShaman, FShaman,
    };

    internal static RaceClipProfile ResolveRaceProfile(string raceName)
    {
        if (string.IsNullOrEmpty(raceName))
            return null;

        for (int i = 0; i < RaceClipTable.Length; i++)
        {
            RaceClipProfile row = RaceClipTable[i];
            if (string.Equals(raceName, row.Race, StringComparison.OrdinalIgnoreCase))
                return row;
        }

        for (int i = 0; i < RaceClipTable.Length; i++)
        {
            RaceClipProfile row = RaceClipTable[i];
            if (ContainsRaceToken(raceName, row.Token))
                return row;
        }

        return null;
    }

    internal static string[] MeshStemsForProfile(RaceClipProfile profile) => MeshStemsForRaceProfile(profile);

    internal static string[] MeshStemsForRaceProfile(RaceClipProfile profile)
    {
        if (profile == null)
            return Array.Empty<string>();
        if (string.IsNullOrEmpty(profile.AlternateMeshStem))
            return new[] { profile.MeshStem };
        return new[] { profile.MeshStem, profile.AlternateMeshStem };
    }
}