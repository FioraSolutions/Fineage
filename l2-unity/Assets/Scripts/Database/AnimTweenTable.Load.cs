using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;

public sealed partial class AnimTweenTable
{
    public void Initialize()
    {
        _byMeshSequence.Clear();
        _bySequence.Clear();
        _byFamily.Clear();
        _rateByMeshSequence.Clear();
        _rateBySequence.Clear();

        string dataPath = Path.Combine(Application.streamingAssetsPath, RelativePath);
        if (!File.Exists(dataPath))
        {
            // Debug.LogWarning("File not found: " + dataPath);
            return;
        }

        using (StreamReader reader = new StreamReader(dataPath))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                    continue;
                if (line.StartsWith("package\t"))
                    continue;

                string[] cols = line.Split('\t');
                if (cols.Length < 7)
                    continue;

                string mesh = cols[1];
                string sequence = cols[2];
                string tweenText = cols[6];
                if (string.IsNullOrEmpty(sequence) || string.IsNullOrEmpty(tweenText))
                    continue;
                if (!float.TryParse(tweenText, NumberStyles.Float, CultureInfo.InvariantCulture, out float tween))
                    continue;

                _byMeshSequence[mesh + "\0" + sequence] = tween;
                if (!_bySequence.ContainsKey(sequence))
                    _bySequence.Add(sequence, tween);

                // Store the sequence source rate (30/24/15…) so locomotion can scale
                // the 24 fps Unity clip by rate/24.
                if (float.TryParse(cols[4], NumberStyles.Float, CultureInfo.InvariantCulture, out float rate) && rate > 0f)
                {
                    string key = mesh + "\0" + sequence;
                    if (!_rateByMeshSequence.ContainsKey(key))
                        _rateByMeshSequence.Add(key, rate);
                    if (!_rateBySequence.ContainsKey(sequence))
                        _rateBySequence.Add(sequence, rate);
                }

                string family = FamilyOf(sequence);
                if (family != null && !_byFamily.ContainsKey(family))
                    _byFamily.Add(family, tween);
            }
        }

        // Debug.Log("Successfully imported " + _byMeshSequence.Count + " animation tween(s)");
    }

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
