using System;
using System.Collections.Generic;

public sealed partial class AnimTweenTable
{
    readonly Dictionary<string, List<string>> _sequencesByMesh =
        new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);

    static string NormalizeKey(string value)
    {
        return string.IsNullOrEmpty(value) ? string.Empty : value.Trim().ToLowerInvariant();
    }

    void IndexSequenceRow(string mesh, string sequence)
    {
        if (string.IsNullOrEmpty(mesh) || string.IsNullOrEmpty(sequence))
            return;
        if (!_sequencesByMesh.TryGetValue(mesh, out List<string> list))
        {
            list = new List<string>();
            _sequencesByMesh[mesh] = list;
        }

        for (int i = 0; i < list.Count; i++)
        {
            if (string.Equals(list[i], sequence, StringComparison.Ordinal))
                return;
        }

        list.Add(sequence);
    }

    /// <summary>Exact Interlude AnimTween sequence name (case preserved) for mesh + normalized query.</summary>
    public bool TryResolveExactSequence(string meshAnim, string normalizedQuery, out string exactSequence)
    {
        exactSequence = null;
        if (string.IsNullOrEmpty(meshAnim) || string.IsNullOrEmpty(normalizedQuery))
            return false;

        string want = NormalizeKey(normalizedQuery);
        if (!_sequencesByMesh.TryGetValue(meshAnim, out List<string> list))
            return false;

        for (int i = 0; i < list.Count; i++)
        {
            string seq = list[i];
            if (NormalizeKey(seq) == want)
            {
                exactSequence = seq;
                return true;
            }
        }

        return false;
    }

    /// <summary>First locomotion row for mesh whose AnimTween family matches (wait/walk/run/atkwait).</summary>
    public bool TryResolveLocomotionSequence(
        string meshAnim,
        string slotFamily,
        string weaponSuffix,
        out string exactSequence)
    {
        exactSequence = null;
        if (string.IsNullOrEmpty(meshAnim) || string.IsNullOrEmpty(slotFamily))
            return false;
        if (!_sequencesByMesh.TryGetValue(meshAnim, out List<string> list))
            return false;

        string slot = NormalizeKey(slotFamily);
        string weapon = string.IsNullOrEmpty(weaponSuffix) ? null : NormalizeKey(weaponSuffix);

        string best = null;
        for (int i = 0; i < list.Count; i++)
        {
            string seq = list[i];
            string family = FamilyOf(seq);
            if (family == null || NormalizeKey(family) != slot)
                continue;

            if (weapon == null)
            {
                bool locomotion = slot == "wait" || slot == "walk" || slot == "run" || slot == "atkwait";
                if (locomotion && seq.IndexOf('_', StringComparison.Ordinal) >= 0)
                    continue;
                if (slot == "atk01" && !NormalizeKey(seq).StartsWith("atk01", StringComparison.Ordinal))
                    continue;
                best = seq;
                break;
            }

            string seqLower = NormalizeKey(seq);
            if (seqLower.Contains("_" + weapon) || seqLower.EndsWith(weapon, StringComparison.Ordinal))
            {
                best = seq;
                break;
            }
        }

        if (best == null)
            return false;
        exactSequence = best;
        return true;
    }
}