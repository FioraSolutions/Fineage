using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;

/// <summary>
/// SpAtk slot → MeshAnimation sequence from LineageWarrior.int.
/// The pawn plays SpAtkNNAnimName[weapon], not a sequence literally named spatkNN.
/// Rows exist only where the body's ukx has no literal spatkNN_weapon_body take
/// and the substituted int sequence is in that mesh. See LineageWarriorAnimAlias.tsv.
/// </summary>
public static class LineageWarriorAnimAlias
{
    const string RelativePath = "Data/Meta/LineageWarriorAnimAlias.tsv";

    static readonly Dictionary<string, string> BySlot =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    static bool _loaded;

    public static void EnsureLoaded()
    {
        if (_loaded)
            return;
        _loaded = true;
        BySlot.Clear();

        string path = Path.Combine(Application.streamingAssetsPath, RelativePath);
        if (!File.Exists(path))
            return;

        using var reader = new StreamReader(path);
        string line;
        while ((line = reader.ReadLine()) != null)
        {
            if (string.IsNullOrWhiteSpace(line) || line[0] == '#')
                continue;
            string[] cols = line.Split('\t');
            if (cols.Length < 4)
                continue;
            if (!int.TryParse(cols[1].Trim(), NumberStyles.None, CultureInfo.InvariantCulture, out int spatk))
                continue;
            string race = cols[0].Trim();
            string weapon = NormalizeWeapon(cols[2]);
            string sequence = cols[3].Trim();
            if (race.Length == 0 || weapon.Length == 0 || sequence.Length == 0)
                continue;
            BySlot[Key(race, spatk, weapon)] = sequence;
        }
    }

    /// <summary>
    /// Sequence the int names for this slot. Weapon token may be "dual", "_dual", or "1HS".
    /// </summary>
    public static bool TryGet(string race, int spatkNumber, string weaponToken, out string sequence)
    {
        sequence = null;
        EnsureLoaded();
        if (string.IsNullOrEmpty(race) || spatkNumber <= 0)
            return false;
        return BySlot.TryGetValue(Key(race, spatkNumber, NormalizeWeapon(weaponToken)), out sequence)
            && !string.IsNullOrEmpty(sequence);
    }

    public static string NormalizeWeapon(string weaponToken)
    {
        if (string.IsNullOrEmpty(weaponToken))
            return "";
        return weaponToken.Trim().TrimStart('_').ToLowerInvariant();
    }

    static string Key(string race, int spatkNumber, string weapon)
    {
        return race + "\0" + spatkNumber.ToString(CultureInfo.InvariantCulture) + "\0" + weapon;
    }
}
