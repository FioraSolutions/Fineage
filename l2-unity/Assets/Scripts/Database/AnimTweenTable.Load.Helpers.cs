using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;

public sealed partial class AnimTweenTable
{
    public bool TryGetTween(string sequence, out float seconds)
    {
        seconds = 0f;
        if (string.IsNullOrEmpty(sequence))
            return false;
        if (_bySequence.TryGetValue(sequence, out seconds))
            return true;

        string family = FamilyOf(sequence);
        return family != null && _byFamily.TryGetValue(family, out seconds);
    }

    public bool TryGetTween(string mesh, string sequence, out float seconds)
    {
        if (!string.IsNullOrEmpty(mesh) && !string.IsNullOrEmpty(sequence)
            && _byMeshSequence.TryGetValue(mesh + "\0" + sequence, out seconds))
        {
            return true;
        }

        return TryGetTween(sequence, out seconds);
    }
}
