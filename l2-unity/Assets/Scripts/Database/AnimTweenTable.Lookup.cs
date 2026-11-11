using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;

public sealed partial class AnimTweenTable
{
    public float TryGetRate(string sequence, float fallback = 30f)
    {
        if (!string.IsNullOrEmpty(sequence) && _rateBySequence.TryGetValue(sequence, out float rate))
            return rate;
        return fallback;
    }

    public float TryGetRate(string mesh, string sequence, float fallback = 30f)
    {
        if (!string.IsNullOrEmpty(mesh) && !string.IsNullOrEmpty(sequence)
            && _rateByMeshSequence.TryGetValue(mesh + "\0" + sequence, out float rate))
            return rate;
        return TryGetRate(sequence, fallback);
    }

    public static string FamilyOf(string sequence)
    {
        if (string.IsNullOrEmpty(sequence))
            return null;

        string n = sequence.ToLowerInvariant();
        if (n.StartsWith("atkwait"))
            return "atkwait";
        if (n.StartsWith("spwait"))
            return "spwait";
        if (n.StartsWith("wait"))
            return "wait";
        if (n.StartsWith("walk"))
            return "walk";
        if (n.StartsWith("run"))
            return "run";
        return null;
    }
}
