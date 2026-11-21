using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;

/// <summary>Npc template ids that can be ridden (strider, wyvern). Used for mount action gating.</summary>
public static class InterludeMountablePetCatalog
{
  public const string RelativePath = "Data/Meta/InterludeMountablePetNpcIds.txt";

  static int[] _ids = Array.Empty<int>();
  static HashSet<int> _set = new HashSet<int>();
  static bool _loaded;

  public static void EnsureLoaded()
  {
    if (_loaded)
      return;
    _loaded = true;
    string path = Path.Combine(Application.streamingAssetsPath, RelativePath);
    if (!File.Exists(path))
    {
      Debug.LogError("[MountablePet] Missing table at StreamingAssets/" + RelativePath);
      _ids = Array.Empty<int>();
      _set = new HashSet<int>();
      return;
    }

    var list = new List<int>();
    foreach (string line in File.ReadAllLines(path))
    {
      if (string.IsNullOrWhiteSpace(line) || line.StartsWith("//"))
        continue;
      if (int.TryParse(line.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out int id))
        list.Add(id);
    }

    list.Sort();
    _ids = list.ToArray();
    _set = new HashSet<int>(_ids);
  }

  public static bool IsMountableNpc(int npcTemplateId)
  {
    EnsureLoaded();
    return _set.Contains(npcTemplateId);
  }

  public static int[] Ids
  {
    get
    {
      EnsureLoaded();
      return _ids;
    }
  }

  /// <summary>Tests may reset catalog without streaming assets.</summary>
  internal static void ResetForTests(int[] ids)
  {
    _loaded = true;
    _ids = ids ?? Array.Empty<int>();
    _set = new HashSet<int>(_ids);
  }
}