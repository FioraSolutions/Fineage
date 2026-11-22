using System;
using System.Collections.Generic;

/// <summary>
/// Maps a Unity clip name to an official MeshAnimation sequence name.
/// Does not invent a sequence when the clip cannot be bound.
/// </summary>
public static class AnimNotifySequenceResolver
{
    public static bool TryResolve(string clipName, string animationHint, out string sequence, out string error)
    {
        sequence = null;
        error = null;
        AnimNotifyTable.Load();
        if (string.IsNullOrEmpty(clipName))
        {
            error = "AnimNotifyPlayback: empty clip name, cannot bind a sequence.";
            return false;
        }

        if (AnimNotifyTable.HasSequence(clipName))
        {
            sequence = clipName;
            return true;
        }

        string stripped = StripCloneSuffix(clipName);
        if (!string.Equals(stripped, clipName, StringComparison.Ordinal)
            && AnimNotifyTable.HasSequence(stripped))
        {
            sequence = stripped;
            return true;
        }

        string best = null;
        int bestScore = -1;
        int ties = 0;
        foreach (string name in AnimNotifyTable.SequenceKeys)
        {
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
