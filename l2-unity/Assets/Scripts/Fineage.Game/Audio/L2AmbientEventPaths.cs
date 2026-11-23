using System;
using System.Collections.Generic;

/// <summary>
/// FMOD event paths for a UE2 <c>Package.Group.Name</c> ambient sound.
/// The bank folder is <c>AmbSound</c>. The package and the group are both
/// path segments; a lookup that drops the group misses <c>AmbSound2.jura.jura_01</c>
/// and collides bare names such as <c>rain_01</c>.
/// </summary>
public static class L2AmbientEventPaths
{
    public static string[] Candidates(string package, string group, string name)
    {
        var list = new List<string>(3);
        if (string.IsNullOrEmpty(package) || string.IsNullOrEmpty(name))
            return list.ToArray();
        if (!string.IsNullOrEmpty(group))
            Add(list, "event:/AmbSound/" + package + "/" + group + "/" + name);
        Add(list, "event:/AmbSound/" + package + "/" + name);
        if (!string.Equals(package, "AmbSound", StringComparison.OrdinalIgnoreCase))
            Add(list, "event:/AmbSound/AmbSound/" + name);
        return list.ToArray();
    }

    /// <summary>Resources path of the official wave, without extension.</summary>
    public static string WaveResource(string package, string group, string name)
    {
        if (string.IsNullOrEmpty(group))
            return "Data/Sounds/" + package + "/" + name;
        return "Data/Sounds/" + package + "/" + group + "/" + name;
    }

    static void Add(List<string> list, string path)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (string.Equals(list[i], path, StringComparison.OrdinalIgnoreCase))
                return;
        }
        list.Add(path);
    }
}
