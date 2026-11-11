using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;
using VContainer;

public sealed partial class AnimTweenTable
{
    private static AnimTweenTable _instance;

    [Obsolete("Use [Inject] / PacketServices instead")]
    public static AnimTweenTable Instance
    {
        get
        {
            if (_instance == null)
                _instance = new AnimTweenTable();
            return _instance;
        }
    }

    const string RelativePath = "Data/Meta/AnimTween.txt";

    readonly Dictionary<string, float> _byMeshSequence =
        new Dictionary<string, float>(StringComparer.OrdinalIgnoreCase);
    readonly Dictionary<string, float> _bySequence =
        new Dictionary<string, float>(StringComparer.OrdinalIgnoreCase);
    readonly Dictionary<string, float> _byFamily =
        new Dictionary<string, float>(StringComparer.OrdinalIgnoreCase);

    // Sequence source rate (AnimTween col "rate", e.g. 30/24/15 fps). Imported
    // Unity clips are keyed on a 24 fps timeline, so playback speed must be
    // rate/24 to reproduce the L2 wall-clock cycle length.
    readonly Dictionary<string, float> _rateByMeshSequence =
        new Dictionary<string, float>(StringComparer.OrdinalIgnoreCase);
    readonly Dictionary<string, float> _rateBySequence =
        new Dictionary<string, float>(StringComparer.OrdinalIgnoreCase);

    public int Count => _byMeshSequence.Count;




    /// <summary>
    /// Source sequence rate (fps) from AnimTween col "rate". Falls back to the
    /// sequence family when the exact mesh+sequence is not listed.
    /// </summary>


    /// <summary>
    /// wait / atkwait / walk / run / spwait. atkwait before wait.
    /// </summary>
}
