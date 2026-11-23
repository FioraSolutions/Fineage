#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

// One classification for a brush/BSP face, shared by the extract
// (tools/interlude-extract/brush_face_class.py), the bake pre-verify, and
// material binding. Class comes from docs/interlude-ref/packages/exports;
// Texture (and subclasses) are pixels, every other Material subclass is a
// props chain. The hierarchy is packages/classes.tsv.
public static class L2BrushFaceClass
{
    public enum Kind
    {
        Texture,
        Material,
        Absent,
        Other,
        Ambiguous,
    }

    public readonly struct Face
    {
        public readonly string Package;
        public readonly string Group;
        public readonly string Name;
        public readonly string UeClass;
        public readonly Kind KindOf;

        public Face(string package, string group, string name, string ueClass, Kind kind)
        {
            Package = package;
            Group = group ?? "";
            Name = name;
            UeClass = ueClass ?? "";
            KindOf = kind;
        }

        public string Format()
        {
            string cls = string.IsNullOrEmpty(UeClass) ? "absent" : UeClass;
            return "package=" + Package + " group=" + Group + " name=" + Name + " class=" + cls;
        }
    }

    static Dictionary<string, string> _superOf;
    static Dictionary<string, string> _classByExport;
    static bool _loaded;

    public static Face Classify(string reference)
    {
        EnsureLoaded();
        if (!TryParse(reference, out string package, out string group, out string name))
        {
            return new Face(reference ?? "", "", "", "", Kind.Absent);
        }

        string key = (package + ".utx").ToLowerInvariant() + "\n" + name.ToLowerInvariant();
        if (!_classByExport.TryGetValue(key, out string ueClass))
        {
            return new Face(package, group, name, "", Kind.Absent);
        }
        if (ueClass.IndexOf('\n') >= 0)
        {
            return new Face(package, group, name, ueClass.Replace('\n', ','), Kind.Ambiguous);
        }
        return new Face(package, group, name, ueClass, KindOf(ueClass));
    }

    public static string FindProps(Face face)
    {
        string textures = Path.Combine(Application.dataPath, "Resources", "Data", "Textures");
        string pkg = FindChild(textures, face.Package, directory: true);
        if (pkg == null)
        {
            return null;
        }
        var hits = new List<string>();
        foreach (string path in Directory.GetFiles(pkg, "*", SearchOption.AllDirectories))
        {
            if (string.Equals(Path.GetFileName(path), face.Name + ".props.txt", StringComparison.OrdinalIgnoreCase))
            {
                hits.Add(path);
            }
        }
        if (!string.IsNullOrEmpty(face.Group))
        {
            var grouped = new List<string>();
            foreach (string path in hits)
            {
                foreach (string part in path.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar))
                {
                    if (string.Equals(part, face.Group, StringComparison.OrdinalIgnoreCase))
                    {
                        grouped.Add(path);
                        break;
                    }
                }
            }
            string chosen = SameBytes(grouped);
            if (chosen != null)
            {
                return chosen;
            }
        }
        return SameBytes(hits);
    }

    static string SameBytes(List<string> paths)
    {
        if (paths.Count == 0)
        {
            return null;
        }
        if (paths.Count == 1)
        {
            return paths[0];
        }
        byte[] first = File.ReadAllBytes(paths[0]);
        for (int i = 1; i < paths.Count; i++)
        {
            byte[] other = File.ReadAllBytes(paths[i]);
            if (first.Length != other.Length)
            {
                return null;
            }
            for (int b = 0; b < first.Length; b++)
            {
                if (first[b] != other[b])
                {
                    return null;
                }
            }
        }
        string shallow = paths[0];
        for (int i = 1; i < paths.Count; i++)
        {
            if (paths[i].Length < shallow.Length)
            {
                shallow = paths[i];
            }
        }
        return shallow;
    }

    public static Face ResolveReference(string token, string currentPackage)
    {
        Face here = Classify(currentPackage + "." + token);
        if (here.KindOf != Kind.Absent)
        {
            return here;
        }
        Face full = Classify(token);
        if (full.KindOf != Kind.Absent)
        {
            return full;
        }
        return here;
    }

    public static void CollectMissingChain(Face face, string exportRoot, List<string> missing)
    {
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        WalkChain(face, exportRoot, seen, missing);
    }

    static void WalkChain(Face face, string exportRoot, HashSet<string> seen, List<string> missing)
    {
        string key = face.Package + "\n" + face.Group + "\n" + face.Name;
        if (!seen.Add(key))
        {
            return;
        }
        if (face.KindOf == Kind.Texture)
        {
            if (!TexturePresent(face, exportRoot))
            {
                missing.Add(face.Format() + " expectedPng=" + face.Name + ".png");
            }
            return;
        }
        if (face.KindOf != Kind.Material)
        {
            missing.Add(face.Format());
            return;
        }
        string props = FindProps(face);
        if (props == null)
        {
            missing.Add(face.Format() + " expectedProps=" + face.Name + ".props.txt");
            return;
        }
        string text = File.ReadAllText(props);
        foreach (Match match in Regex.Matches(text, @"\b([A-Za-z_][A-Za-z0-9_]*)'([^']+)'"))
        {
            WalkChain(ResolveReference(match.Groups[2].Value, face.Package), exportRoot, seen, missing);
        }
    }

    static bool TexturePresent(Face face, string exportRoot)
    {
        if (FindUnderPackage(face, face.Name + ".png") != null)
        {
            return true;
        }
        if (string.IsNullOrEmpty(exportRoot))
        {
            return false;
        }
        string pkg = FindChild(exportRoot, face.Package, directory: true);
        return pkg != null && FindNamedFile(pkg, face.Name + ".png") != null;
    }

    public static Material LoadMaterial(Face face)
    {
        if (face.KindOf == Kind.Texture)
        {
            string mat = FindUnderPackage(face, "Materials/" + face.Name + ".mat");
            return mat == null ? null : AssetDatabase.LoadAssetAtPath<Material>(mat);
        }
        if (face.KindOf == Kind.Material)
        {
            string props = FindProps(face);
            if (props == null)
            {
                return null;
            }
            string dir = Path.GetDirectoryName(props);
            string matPath = FindNamedFile(dir, face.Name + ".mat");
            return matPath == null ? null : AssetDatabase.LoadAssetAtPath<Material>(ToAssetPath(matPath));
        }
        return null;
    }

    static void EnsureLoaded()
    {
        if (_loaded)
        {
            return;
        }
        _loaded = true;
        string root = Path.GetFullPath(Path.Combine(
            Application.dataPath, "..", "docs", "interlude-ref", "packages"));
        _superOf = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (string line in File.ReadAllLines(Path.Combine(root, "classes.tsv")))
        {
            string[] cols = line.Split('\t');
            if (cols.Length < 4 || cols[0] == "folder")
            {
                continue;
            }
            if (!_superOf.ContainsKey(cols[2]))
            {
                _superOf[cols[2]] = cols[3];
            }
        }
        _classByExport = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (string part in Directory.GetFiles(root, "exports.part*.tsv"))
        {
            foreach (string line in File.ReadAllLines(part))
            {
                string[] cols = line.Split('\t');
                if (cols.Length < 7 || cols[2] != ".utx")
                {
                    continue;
                }
                string key = cols[1].ToLowerInvariant() + "\n" + cols[5].ToLowerInvariant();
                if (_classByExport.TryGetValue(key, out string prior))
                {
                    if (!string.Equals(prior, cols[6], StringComparison.Ordinal) && prior.IndexOf('\n') < 0)
                    {
                        _classByExport[key] = prior + "\n" + cols[6];
                    }
                    else if (prior.IndexOf('\n') >= 0 && prior.IndexOf(cols[6], StringComparison.Ordinal) < 0)
                    {
                        _classByExport[key] = prior + "\n" + cols[6];
                    }
                }
                else
                {
                    _classByExport[key] = cols[6];
                }
            }
        }
    }

    static Kind KindOf(string ueClass)
    {
        var seen = new HashSet<string>(StringComparer.Ordinal);
        string current = ueClass;
        bool material = false;
        while (!string.IsNullOrEmpty(current) && seen.Add(current) && current != "Class")
        {
            if (current == "Texture")
            {
                return Kind.Texture;
            }
            if (current == "Material")
            {
                material = true;
            }
            if (!_superOf.TryGetValue(current, out current))
            {
                break;
            }
        }
        return material ? Kind.Material : Kind.Other;
    }

    static bool TryParse(string reference, out string package, out string group, out string name)
    {
        package = "";
        group = "";
        name = "";
        if (string.IsNullOrEmpty(reference))
        {
            return false;
        }
        string full = reference.Trim();
        int tick = full.IndexOf('\'');
        if (tick >= 0)
        {
            full = full.Substring(tick + 1).TrimEnd('\'');
        }
        string[] parts = full.Split('.');
        if (parts.Length < 2)
        {
            return false;
        }
        package = parts[0];
        name = parts[parts.Length - 1];
        if (parts.Length > 2)
        {
            group = string.Join(".", parts, 1, parts.Length - 2);
        }
        return !string.IsNullOrEmpty(package) && !string.IsNullOrEmpty(name);
    }

    static string FindUnderPackage(Face face, string relative)
    {
        string textures = Path.Combine(Application.dataPath, "Resources", "Data", "Textures");
        string pkg = FindChild(textures, face.Package, directory: true);
        if (pkg == null)
        {
            return null;
        }
        string[] relParts = relative.Split('/');
        string cur = pkg;
        for (int i = 0; i < relParts.Length; i++)
        {
            bool last = i == relParts.Length - 1;
            // Materials/{name}.mat is a direct child of Materials. A bare
            // {name}.props.txt may sit in a class folder, so the last segment
            // of a props search walks the package tree.
            if (!last || relative.IndexOf('/') >= 0)
            {
                cur = FindChild(cur, relParts[i], directory: !last);
                if (cur == null)
                {
                    return null;
                }
            }
            else
            {
                return FindNamedFile(cur, relParts[i]);
            }
        }
        return ToAssetPath(cur);
    }

    static string FindNamedFile(string directory, string fileName)
    {
        if (string.IsNullOrEmpty(directory) || !Directory.Exists(directory))
        {
            return null;
        }
        foreach (string path in Directory.GetFiles(directory, "*", SearchOption.AllDirectories))
        {
            if (string.Equals(Path.GetFileName(path), fileName, StringComparison.OrdinalIgnoreCase))
            {
                return path;
            }
        }
        return null;
    }

    static string FindChild(string directory, string name, bool directoryChild)
    {
        if (string.IsNullOrEmpty(directory) || !Directory.Exists(directory))
        {
            return null;
        }
        string[] entries = directoryChild ? Directory.GetDirectories(directory) : Directory.GetFiles(directory);
        foreach (string entry in entries)
        {
            if (string.Equals(Path.GetFileName(entry), name, StringComparison.OrdinalIgnoreCase))
            {
                return entry;
            }
        }
        return null;
    }

    static string ToAssetPath(string fullPath)
    {
        string data = Path.GetFullPath(Application.dataPath);
        string full = Path.GetFullPath(fullPath);
        if (!full.StartsWith(data, StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }
        return ("Assets" + full.Substring(data.Length)).Replace('\\', '/');
    }
}
#endif
