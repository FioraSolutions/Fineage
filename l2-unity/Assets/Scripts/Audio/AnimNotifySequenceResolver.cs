using System;
using System.Collections.Generic;

/// <summary>
/// Maps a Unity clip name to one MeshAnimation's sequence.
/// A sequence name that exists on more than one MeshAnimation is not a match
/// unless the owner's animation is known. Other packages are not played.
/// </summary>
public static class AnimNotifySequenceResolver
{
    public static bool TryResolve(string clipName, string animationHint, out string sequence, out string error) =>
        TryResolve(clipName, animationHint, out _, out sequence, out error);

    public static bool TryResolve(
        string clipName,
        string animationHint,
        out string animation,
        out string sequence,
        out string error)
    {
        animation = null;
        sequence = null;
        error = null;
        AnimNotifyTable.Load();
        if (string.IsNullOrEmpty(clipName))
        {
            error = "AnimNotifyPlayback: empty clip name, cannot bind a sequence.";
            return false;
        }

        string owner = string.IsNullOrEmpty(animationHint) ? null : animationHint;
        string stripped = StripCloneSuffix(clipName);
        if (TryTakeExact(stripped, owner, clipName, out animation, out sequence, out error))
            return sequence != null;
        if (error != null)
            return false;

        string best = null;
        int bestScore = -1;
        int ties = 0;
        foreach (string name in AnimNotifyTable.SequenceKeys)
        {
            if (owner != null && !AnimNotifyTable.AnimationHasSequence(owner, name))
                continue;
            int score = Score(stripped, name, animationHint);
            if (score < 0)
                continue;
            if (score > bestScore)
            {
                best = name;
                bestScore = score;
                ties = 1;
            }
            else if (score == bestScore)
            {
                ties++;
            }
        }

        if (best != null && ties == 1)
        {
            if (owner == null
                && AnimNotifyTable.TryGetAnimations(best, out IReadOnlyList<string> owners)
                && owners.Count > 1)
            {
                error = FormatShared(best, clipName, owners);
                return false;
            }

            animation = owner;
            if (animation == null
                && AnimNotifyTable.TryGetAnimations(best, out IReadOnlyList<string> single)
                && single.Count == 1)
                animation = single[0];
            sequence = best;
            return true;
        }

        if (best != null && ties > 1)
        {
            error = "AnimNotifyPlayback: ambiguous sequence for clip=\"" + clipName
                + "\"" + (string.IsNullOrEmpty(animationHint) ? "" : " animation=\"" + animationHint + "\"")
                + " matches=" + ties + " score=" + bestScore + ".";
            return false;
        }

        error = "AnimNotifyPlayback: no official sequence for clip=\"" + clipName + "\""
            + (string.IsNullOrEmpty(animationHint) ? "" : " animation=\"" + animationHint + "\"")
            + ".";
        return false;
    }

    static bool TryTakeExact(
        string stripped,
        string owner,
        string clipName,
        out string animation,
        out string sequence,
        out string error)
    {
        animation = null;
        sequence = null;
        error = null;
        if (!AnimNotifyTable.TryGetAnimations(stripped, out IReadOnlyList<string> owners) || owners.Count == 0)
            return false;

        if (owner != null)
        {
            if (!AnimNotifyTable.AnimationHasSequence(owner, stripped))
                return false;
            animation = owner;
            sequence = stripped;
            return true;
        }

        if (owners.Count == 1)
        {
            animation = owners[0];
            sequence = stripped;
            return true;
        }

        error = FormatShared(stripped, clipName, owners);
        return true;
    }

    static string FormatShared(string sequence, string clipName, IReadOnlyList<string> owners)
    {
        string sample = owners.Count > 0 ? owners[0] : "";
        if (owners.Count > 1)
            sample = sample + ", " + owners[1];
        return "AnimNotifyPlayback: sequence=\"" + sequence + "\" is shared by "
            + owners.Count + " MeshAnimations (for example " + sample
            + ") and the owner MeshAnimation is missing. clip=\"" + clipName
            + "\". Other packages are not played.";
    }

    static string StripCloneSuffix(string name)
    {
        if (name.EndsWith("(Clone)", StringComparison.Ordinal))
            return name.Substring(0, name.Length - 7).Trim();
        return name;
    }

    static int Score(string clip, string sequence, string animationHint)
    {
        if (string.Equals(clip, sequence, StringComparison.OrdinalIgnoreCase))
            return 100;
        if (!string.IsNullOrEmpty(animationHint)
            && sequence.IndexOf(animationHint, StringComparison.OrdinalIgnoreCase) < 0
            && animationHint.IndexOf(sequence, StringComparison.OrdinalIgnoreCase) < 0)
        {
            // Don't reject yet — animation hint is the MeshAnimation name (FDarkElf_anim),
            // sequence is Walk_1HS_FDarkElf. Filter by token later.
        }

        if (sequence.EndsWith("_" + clip, StringComparison.OrdinalIgnoreCase)
            || clip.EndsWith("_" + sequence, StringComparison.OrdinalIgnoreCase))
            return PreferHint(80, sequence, animationHint);
        if (sequence.StartsWith(clip + "_", StringComparison.OrdinalIgnoreCase)
            || clip.StartsWith(sequence + "_", StringComparison.OrdinalIgnoreCase))
            return PreferHint(70, sequence, animationHint);
        if (ContainsToken(sequence, clip) || ContainsToken(clip, sequence))
            return PreferHint(40, sequence, animationHint);
        return -1;
    }

    static int PreferHint(int score, string sequence, string animationHint)
    {
        if (string.IsNullOrEmpty(animationHint))
            return score;
        string token = animationHint;
        if (token.EndsWith("_anim", StringComparison.OrdinalIgnoreCase))
            token = token.Substring(0, token.Length - 5);
        if (sequence.IndexOf(token, StringComparison.OrdinalIgnoreCase) >= 0)
            return score + 15;
        return score;
    }

    static bool ContainsToken(string haystack, string needle)
    {
        if (string.IsNullOrEmpty(needle) || needle.Length < 4)
            return false;
        return haystack.IndexOf(needle, StringComparison.OrdinalIgnoreCase) >= 0;
    }
}
