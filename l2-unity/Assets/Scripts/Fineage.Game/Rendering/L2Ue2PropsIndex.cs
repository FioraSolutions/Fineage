using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

/// <summary>
/// Props files keyed by the package and group the dump wrote into the path.
/// A short name is registered only when every file with that name has the same
/// text. Two TexPanner0 exports stay Shaders.TexPanner0 and moon.TexPanner0.
/// </summary>
public static class L2Ue2PropsIndex
{
    public const char PackageScope = '\u001f';

    public sealed class Entry
    {
        public string RelativePath;
        public string Package;
        public string ClassFolder;
        public string Group;
        public string Name;
        public string Text;
        public string DiffuseRef;
    }

    static readonly Regex DiffuseRe = new Regex(
        @"^[ \t]*Diffuse[ \t]*=[ \t]*(?:[A-Za-z_][A-Za-z0-9_]*')?([^'\r\n]+)",
        RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.Compiled);

    public static string TexturesRoot()
    {
        return Path.Combine(Application.dataPath, "Resources", "Data", "Textures");
    }

    public static List<Entry> Load(string texturesRoot)
    {
        var list = new List<Entry>();
        if (string.IsNullOrEmpty(texturesRoot) || !Directory.Exists(texturesRoot))
            return list;
        string[] files = Directory.GetFiles(texturesRoot, "*.props.txt", SearchOption.AllDirectories);
        Array.Sort(files, (a, b) =>
        {
            int byLen = a.Length.CompareTo(b.Length);
            return byLen != 0 ? byLen : string.Compare(a, b, StringComparison.OrdinalIgnoreCase);
        });
        string root = texturesRoot.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        for (int i = 0; i < files.Length; i++)
        {
            string relative = files[i].Substring(root.Length).TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            relative = relative.Replace('\\', '/');
            if (!TryDescribe(relative, out string package, out string classFolder, out string group, out string name))
                continue;
            string text = File.ReadAllText(files[i]);
            list.Add(new Entry
            {
                RelativePath = relative,
                Package = package,
                ClassFolder = classFolder,
                Group = group,
                Name = name,
                Text = text,
                DiffuseRef = DiffuseRefOf(text),
            });
        }
        return list;
    }

    public static bool TryDescribe(string relative, out string package, out string classFolder, out string group, out string name)
    {
        package = null;
        classFolder = null;
        group = null;
        name = null;
        if (string.IsNullOrEmpty(relative))
            return false;
        const string suffix = ".props.txt";
        if (!relative.EndsWith(suffix, StringComparison.OrdinalIgnoreCase))
            return false;
        string[] parts = relative.Substring(0, relative.Length - suffix.Length).Split('/');
        if (parts.Length < 2)
            return false;
        package = parts[0];
        int i = 1;
        while (i < parts.Length - 1 && string.Equals(parts[i], package, StringComparison.OrdinalIgnoreCase))
            i++;
        if (i >= parts.Length)
            return false;
        classFolder = parts[i];
        i++;
        name = parts[parts.Length - 1];
        if (i < parts.Length - 1)
            group = string.Join(".", parts, i, parts.Length - 1 - i);
        else
            group = "";
        return !string.IsNullOrEmpty(name);
    }

    /// <summary>
    /// Fills <paramref name="props"/> and <paramref name="sources"/>.
    /// A key whose files disagree is left out and named in <paramref name="conflicts"/>.
    /// </summary>
    public static void AddKeys(
        IReadOnlyList<Entry> files,
        Dictionary<string, string> props,
        Dictionary<string, string> sources,
        List<string> conflicts)
    {
        var rejected = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var byName = new Dictionary<string, List<Entry>>(StringComparer.OrdinalIgnoreCase);
        var byPackageName = new Dictionary<string, List<Entry>>(StringComparer.OrdinalIgnoreCase);
        for (int i = 0; i < files.Count; i++)
        {
            Entry file = files[i];
            if (!byName.TryGetValue(file.Name, out List<Entry> named))
            {
                named = new List<Entry>();
                byName[file.Name] = named;
            }
            named.Add(file);
            string packageName = file.Package + PackageScope + file.Name;
            if (!byPackageName.TryGetValue(packageName, out List<Entry> packaged))
            {
                packaged = new List<Entry>();
                byPackageName[packageName] = packaged;
            }
            packaged.Add(file);

            if (!string.IsNullOrEmpty(file.Group))
            {
                TryAdd(props, sources, rejected, conflicts, file.Group + "." + file.Name, file);
                TryAdd(props, sources, rejected, conflicts, file.Package + "." + file.Group + "." + file.Name, file);
            }
            else
            {
                // package.Name is shared by Shader and Materials copies. The mesh
                // chooser picks; this key stays unregistered when the texts differ.
                TryAdd(props, sources, rejected, null, file.Package + "." + file.Name, file);
            }
        }

        foreach (var kv in byName)
        {
            if (SameText(kv.Value))
                TryAdd(props, sources, rejected, conflicts, kv.Key, kv.Value[0]);
        }

        foreach (var kv in byPackageName)
        {
            if (SameText(kv.Value))
                TryAdd(props, sources, rejected, conflicts, kv.Key, kv.Value[0]);
        }
    }

    /// <summary>
    /// Picks one file when a mesh material's short name is shared.
    /// The current texture's package wins. A Shader wins over a Materials copy
    /// of the same diffuse. Never the first file.
    /// </summary>
    public static Entry Choose(
        IReadOnlyList<Entry> sameName,
        string currentTexture,
        IReadOnlyList<string> textureNames,
        IReadOnlyList<string> texturePackages,
        out string reason)
    {
        reason = null;
        if (sameName == null || sameName.Count == 0)
            return null;
        if (SameText(sameName))
        {
            reason = "identical copies";
            return Shortest(sameName);
        }

        List<Entry> pool = new List<Entry>(sameName);
        if (!string.IsNullOrEmpty(currentTexture))
        {
            string currentShort = L2Ue2MaterialChain.ShortName(currentTexture);
            var byDiffuse = new List<Entry>();
            for (int i = 0; i < pool.Count; i++)
            {
                if (string.Equals(DiffuseShort(pool[i].DiffuseRef), currentShort, StringComparison.OrdinalIgnoreCase))
                    byDiffuse.Add(pool[i]);
            }
            if (byDiffuse.Count == 1)
            {
                reason = "diffuse " + currentShort;
                return byDiffuse[0];
            }
            if (byDiffuse.Count > 1)
                pool = byDiffuse;

            string currentPackage = UniqueTexturePackage(textureNames, texturePackages, currentShort);
            if (!string.IsNullOrEmpty(currentPackage))
            {
                var byPackage = new List<Entry>();
                for (int i = 0; i < pool.Count; i++)
                {
                    string diffusePackage = L2Ue2MaterialChain.PackageName(pool[i].DiffuseRef);
                    if (string.Equals(diffusePackage, currentPackage, StringComparison.OrdinalIgnoreCase))
                        byPackage.Add(pool[i]);
                }
                if (byPackage.Count == 1)
                {
                    reason = "texture package " + currentPackage;
                    return byPackage[0];
                }
                if (byPackage.Count > 1)
                    pool = byPackage;
            }
        }

        Entry shader = null;
        int shaderCount = 0;
        int materialsCount = 0;
        for (int i = 0; i < pool.Count; i++)
        {
            if (string.Equals(pool[i].ClassFolder, "Shader", StringComparison.OrdinalIgnoreCase))
            {
                shaderCount++;
                shader = pool[i];
            }
            else if (string.Equals(pool[i].ClassFolder, "Materials", StringComparison.OrdinalIgnoreCase))
                materialsCount++;
        }
        if (shaderCount == 1 && shaderCount + materialsCount == pool.Count)
        {
            reason = "Shader over Materials";
            return shader;
        }

        reason = "ambiguous";
        return null;
    }

    public static string DescribePaths(IReadOnlyList<Entry> files)
    {
        if (files == null || files.Count == 0)
            return "";
        var parts = new string[files.Count];
        for (int i = 0; i < files.Count; i++)
            parts[i] = files[i].RelativePath + " (" + DiffuseLine(files[i].Text) + ")";
        return string.Join("; ", parts);
    }

    static void TryAdd(
        Dictionary<string, string> props,
        Dictionary<string, string> sources,
        HashSet<string> rejected,
        List<string> conflicts,
        string key,
        Entry file)
    {
        if (rejected.Contains(key))
            return;
        if (props.TryGetValue(key, out string existing))
        {
            if (!string.Equals(existing, file.Text, StringComparison.Ordinal))
            {
                conflicts?.Add(key + " " + sources[key] + " vs " + file.RelativePath);
                props.Remove(key);
                sources.Remove(key);
                rejected.Add(key);
            }
            return;
        }
        props.Add(key, file.Text);
        sources[key] = file.RelativePath;
    }

    static bool SameText(IReadOnlyList<Entry> files)
    {
        if (files.Count == 0)
            return false;
        for (int i = 1; i < files.Count; i++)
        {
            if (!string.Equals(files[i].Text, files[0].Text, StringComparison.Ordinal))
                return false;
        }
        return true;
    }

    static Entry Shortest(IReadOnlyList<Entry> files)
    {
        Entry best = files[0];
        for (int i = 1; i < files.Count; i++)
        {
            if (files[i].RelativePath.Length < best.RelativePath.Length)
                best = files[i];
        }
        return best;
    }

    static string UniqueTexturePackage(IReadOnlyList<string> names, IReadOnlyList<string> packages, string textureShort)
    {
        if (names == null || packages == null || string.IsNullOrEmpty(textureShort))
            return null;
        string found = null;
        int count = Math.Min(names.Count, packages.Count);
        for (int i = 0; i < count; i++)
        {
            if (!string.Equals(names[i], textureShort, StringComparison.OrdinalIgnoreCase))
                continue;
            if (found == null)
                found = packages[i];
            else if (!string.Equals(found, packages[i], StringComparison.OrdinalIgnoreCase))
                return null;
        }
        return found;
    }

    static string DiffuseRefOf(string text)
    {
        if (string.IsNullOrEmpty(text))
            return null;
        Match match = DiffuseRe.Match(text);
        if (!match.Success)
            return null;
        string raw = match.Groups[1].Value.Trim();
        if (raw.Equals("None", StringComparison.OrdinalIgnoreCase))
            return null;
        return raw;
    }

    static string DiffuseShort(string diffuseRef)
    {
        return string.IsNullOrEmpty(diffuseRef) ? null : L2Ue2MaterialChain.ShortName(diffuseRef);
    }

    static string DiffuseLine(string body)
    {
        if (string.IsNullOrEmpty(body))
            return "Diffuse missing";
        int i = body.IndexOf("Diffuse", StringComparison.OrdinalIgnoreCase);
        if (i < 0)
            return "Diffuse missing";
        int end = body.IndexOf('\n', i);
        string line = end > i ? body.Substring(i, end - i) : body.Substring(i);
        return line.Trim();
    }
}
