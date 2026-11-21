using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;

/// <summary>Mobius door table (<c>doors.json</c>) for binding Net <see cref="DoorStatusNetMsg.DoorId"/> to scene mesh names.</summary>
public static class DoorCatalog
{
    sealed class Row
    {
        public int Id;
        public string Name;
    }

    static readonly Dictionary<int, Row> ById = new Dictionary<int, Row>();
    static bool _loaded;

    public static bool TryGetName(int doorId, out string name)
    {
        EnsureLoaded();
        if (ById.TryGetValue(doorId, out Row row))
        {
            name = row.Name;
            return !string.IsNullOrEmpty(name);
        }

        name = "";
        return false;
    }

    static void EnsureLoaded()
    {
        if (_loaded)
            return;
        _loaded = true;
        string path = Path.Combine(
            UnityEngine.Application.streamingAssetsPath,
            "Data/Meta/doors.json");
        if (!File.Exists(path))
        {
            FineageLog.Error(FineageLogCategory.Net, $"DoorCatalog: missing datapack at {path}");
            return;
        }

        try
        {
            string json = File.ReadAllText(path);
            var rows = JArray.Parse(json);
            for (int i = 0; i < rows.Count; i++)
            {
                JToken token = rows[i];
                if (token == null)
                    continue;
                int id = token.Value<int?>("id") ?? 0;
                string rowName = token.Value<string>("name") ?? "";
                if (id <= 0 || string.IsNullOrEmpty(rowName))
                    continue;
                ById[id] = new Row { Id = id, Name = rowName };
            }
        }
        catch (Exception ex)
        {
            FineageLog.Error(FineageLogCategory.Net, $"DoorCatalog: failed to parse doors.json — {ex.Message}");
        }
    }
}