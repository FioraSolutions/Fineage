using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;

public sealed partial class EnterEventTable
{
    public void Initialize()
    {
        _rows.Clear();

        string dataPath = Path.Combine(Application.streamingAssetsPath, RelativePath);
        if (!File.Exists(dataPath))
            return;

        using (StreamReader reader = new StreamReader(dataPath))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("id\t"))
                    continue;

                string[] cols = line.Split('\t');
                if (cols.Length < 9)
                    continue;
                if (!int.TryParse(cols[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out int id))
                    continue;
                if (id <= 0)
                    continue;
                if (!int.TryParse(cols[5], NumberStyles.Integer, CultureInfo.InvariantCulture, out int isRise))
                    continue;
                if (!int.TryParse(cols[6], NumberStyles.Integer, CultureInfo.InvariantCulture, out int spawnType))
                    continue;

                float.TryParse(cols[3], NumberStyles.Float, CultureInfo.InvariantCulture, out float volume);
                float.TryParse(cols[4], NumberStyles.Float, CultureInfo.InvariantCulture, out float radius);

                _rows[id] = new Row
                {
                    IsRise = isRise,
                    SpawnType = spawnType,
                    EffectClass = ClassName(cols[7]),
                    AnimName = cols[8].Trim(),
                    SoundName = CleanSound(cols[2]),
                    SoundVolume = volume,
                    SoundRadius = radius
                };
            }
        }
    }

    public bool TryGet(int npcId, out Row row)
    {
        if (_rows.Count == 0)
            Initialize();
        return _rows.TryGetValue(npcId, out row);
    }
}
