#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
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
        return FindUnderPackage(face, face.Name + ".props.txt");
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
