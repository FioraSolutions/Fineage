using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

/// <summary>
/// Official item / monster / character wave → Resources path, from the lossless uax catalog.
/// Lookups never invent a stem: missing or ambiguous names log package+name (and candidates).
/// </summary>
public static class OfficialWaveTable
{
    public const string RelativePath = "Data/Meta/Sounds/official-waves.tsv";

    public static Func<string> TextOverride;
    public static Action<string> ErrorLogged;

    public sealed class Entry
    {
        public readonly string Package;
        public readonly string Group;
        public readonly string Name;
        public readonly string ResourcesPath;

        public Entry(string package, string group, string name, string resourcesPath)
        {
            Package = package ?? string.Empty;
            Group = group ?? string.Empty;
            Name = name ?? string.Empty;
            ResourcesPath = resourcesPath ?? string.Empty;
        }
    }

    static readonly List<Entry> All = new List<Entry>();
    static readonly Dictionary<string, List<Entry>> ByName =
        new Dictionary<string, List<Entry>>(StringComparer.OrdinalIgnoreCase);
    static readonly Dictionary<string, List<Entry>> ByPackageName =
        new Dictionary<string, List<Entry>>(StringComparer.OrdinalIgnoreCase);
    static readonly Dictionary<string, List<Entry>> ByGroupName =
        new Dictionary<string, List<Entry>>(StringComparer.OrdinalIgnoreCase);
    static readonly Dictionary<string, List<Entry>> ByChrBankEvent =
        new Dictionary<string, List<Entry>>(StringComparer.OrdinalIgnoreCase);

    static readonly Regex ChrEventSuffix = new Regex(
        @"_(1H_S_Atk|2H_S_Atk|Pole_S_Atk|1H_Atk|2H_Atk|Pole_Atk|AtkWait|Preatk|Atk|Dmg|Breath|Death|Wait|Step|Sitdown|Standup|Stand|Agree|Yawn|Admiration|Swish|Jump)(?:_(\d+))?$",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    static readonly string[] GroupBankSuffixes =
    {
        "_Basic_Action",
        "_Normal_Attack",
        "_Special_Attack",
        "_Social_Action",
        "_Spell",
    };

    static bool _loaded;
    static string _loadError;

    public static bool IsLoaded => _loaded;
    public static string LoadError => _loadError;
    public static int EntryCount => All.Count;

    public static string FormatMissingTableError(string path) =>
        "OfficialWaveTable: missing official wave table path=\"" + path + "\"";

    public static string FormatParseError(string path, int line, string detail) =>
        "OfficialWaveTable: parse error path=\"" + path + "\" line=" + line + " " + detail;

    public static string FormatMissingWaveError(string package, string name, string extra) =>
        "OfficialWaveTable: missing wave package=\"" + (package ?? "")
        + "\" name=\"" + (name ?? "") + "\""
        + (string.IsNullOrEmpty(extra) ? "" : " " + extra);

    public static string FormatAmbiguousWaveError(string name, IReadOnlyList<Entry> candidates) =>
        "OfficialWaveTable: ambiguous wave name=\"" + name + "\" candidates="
        + FormatCandidates(candidates);

    public static void ClearForTests()
    {
        All.Clear();
        ByName.Clear();
        ByPackageName.Clear();
        ByGroupName.Clear();
        ByChrBankEvent.Clear();
        _loaded = false;
        _loadError = null;
        TextOverride = null;
        ErrorLogged = null;
    }

    public static void Load()
    {
        if (_loaded)
            return;
        LoadFrom(ResolveDefaultText(), DefaultPath());
    }

    public static void LoadFrom(string text, string pathForErrors)
    {
        All.Clear();
        ByName.Clear();
        ByPackageName.Clear();
        ByGroupName.Clear();
        ByChrBankEvent.Clear();
        _loadError = null;
        _loaded = true;

        if (string.IsNullOrEmpty(text))
        {
            _loadError = FormatMissingTableError(pathForErrors);
            Log(_loadError);
            return;
        }

        int lineNo = 0;
        using (var reader = new StringReader(text))
        {
            string header = reader.ReadLine();
            lineNo++;
            if (header == null)
            {
                _loadError = FormatMissingTableError(pathForErrors);
                Log(_loadError);
                return;
            }

            string line;
            while ((line = reader.ReadLine()) != null)
            {
                lineNo++;
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                    continue;
                string[] cols = line.Split('\t');
                if (cols.Length < 4)
                {
                    Log(FormatParseError(pathForErrors, lineNo, "expected 4 columns, got " + cols.Length));
                    continue;
                }

                var entry = new Entry(cols[0], cols[1], cols[2], cols[3]);
                if (string.IsNullOrEmpty(entry.Name) || string.IsNullOrEmpty(entry.ResourcesPath))
                {
                    Log(FormatParseError(pathForErrors, lineNo,
                        "empty name or resources_path package=\"" + entry.Package + "\""));
                    continue;
                }

                All.Add(entry);
                Add(ByName, entry.Name, entry);
                Add(ByPackageName, entry.Package + "\0" + entry.Name, entry);
                if (!string.IsNullOrEmpty(entry.Group))
                    Add(ByGroupName, entry.Group + "\0" + entry.Name, entry);
                IndexChr(entry);
            }
        }
    }

    public static bool TryResolve(
        string name,
        out Entry entry,
        out string error,
        string packageHint = null,
        string groupHint = null,
        string contextHint = null)
    {
        EnsureLoaded();
        entry = null;
        error = null;
        if (string.IsNullOrEmpty(name))
        {
            error = FormatMissingWaveError(packageHint, name, "empty name");
            return false;
        }

        List<Entry> matches = null;
        if (!string.IsNullOrEmpty(packageHint) && !string.IsNullOrEmpty(groupHint)
            && ByPackageName.TryGetValue(packageHint + "\0" + name, out List<Entry> pkgRows))
        {
            matches = FilterGroup(pkgRows, groupHint);
        }
        else if (!string.IsNullOrEmpty(packageHint)
                 && ByPackageName.TryGetValue(packageHint + "\0" + name, out pkgRows))
        {
            matches = pkgRows;
        }
        else if (!string.IsNullOrEmpty(groupHint)
                 && ByGroupName.TryGetValue(groupHint + "\0" + name, out List<Entry> groupRows))
        {
            matches = groupRows;
        }
        else
        {
            ByName.TryGetValue(name, out matches);
        }

        if (matches == null || matches.Count == 0)
        {
            error = FormatMissingWaveError(packageHint, name,
                string.IsNullOrEmpty(groupHint) ? null : "group=\"" + groupHint + "\"");
            return false;
        }

        Entry unique = UniquePath(matches);
        if (unique != null)
        {
            entry = unique;
            return true;
        }

        if (!string.IsNullOrEmpty(contextHint))
        {
            var hinted = new List<Entry>();
            for (int i = 0; i < matches.Count; i++)
            {
                if (!string.IsNullOrEmpty(matches[i].Group)
                    && contextHint.IndexOf(matches[i].Group, StringComparison.OrdinalIgnoreCase) >= 0)
                    hinted.Add(matches[i]);
            }

            unique = UniquePath(hinted);
            if (unique != null)
            {
                entry = unique;
                return true;
            }
        }

        error = FormatAmbiguousWaveError(name, matches);
        return false;
    }

    public static bool TryResolveDatToken(string token, out string resourcesPath, out string error)
    {
        resourcesPath = null;
        error = null;
        if (!TryParseDatToken(token, out string package, out string group, out string name))
        {
            error = FormatMissingWaveError(null, token, "unrecognized DAT token");
            return false;
        }

        if (!TryResolve(name, out Entry entry, out error, package, group))
            return false;
        resourcesPath = entry.ResourcesPath;
        return true;
    }

    public static bool TryParseDatToken(string token, out string package, out string group, out string name)
    {
        package = null;
        group = null;
        name = null;
        if (string.IsNullOrWhiteSpace(token))
            return false;

        string cleaned = SoundStem(token.Trim());

        int dot = cleaned.IndexOf('.');
        if (dot <= 0 || dot >= cleaned.Length - 1)
            return false;

        package = cleaned.Substring(0, dot);
        string rest = cleaned.Substring(dot + 1);
        int dot2 = rest.IndexOf('.');
        if (dot2 > 0 && dot2 < rest.Length - 1)
        {
            group = rest.Substring(0, dot2);
            name = rest.Substring(dot2 + 1);
        }
        else
        {
            name = rest;
        }

        return !string.IsNullOrEmpty(package) && !string.IsNullOrEmpty(name);
    }

    public static bool TryResolveMonster(string npcFolder, string eventKey, out string resourcesPath, out string error)
    {
        EnsureLoaded();
        resourcesPath = null;
        error = null;
        if (string.IsNullOrEmpty(npcFolder) || string.IsNullOrEmpty(eventKey))
        {
            error = FormatMissingWaveError("monsound", npcFolder + "/" + eventKey, "empty monster folder or event");
            return false;
        }

        string[] stems =
        {
            npcFolder + "_" + eventKey,
            npcFolder + "_" + Capitalize(eventKey),
        };

        Entry found = null;
        for (int i = 0; i < stems.Length; i++)
        {
            if (TryResolve(stems[i], out Entry entry, out string resolveError, packageHint: null, groupHint: npcFolder))
            {
                found = entry;
                break;
            }

            if (resolveError != null && resolveError.StartsWith("OfficialWaveTable: ambiguous", StringComparison.Ordinal))
            {
                error = resolveError;
                return false;
            }
        }

        if (found == null)
        {
            error = FormatMissingWaveError("monsound", stems[0], "group=\"" + npcFolder + "\"");
            return false;
        }

        resourcesPath = found.ResourcesPath;
        return true;
    }

    public static bool TryResolveCharacter(
        string bank,
        CharacterSoundEvent soundEvent,
        out IReadOnlyList<Entry> entries,
        out string error)
    {
        EnsureLoaded();
        entries = Array.Empty<Entry>();
        error = null;
        if (string.IsNullOrEmpty(bank) || soundEvent == CharacterSoundEvent.Other)
        {
            error = FormatMissingWaveError("chrsound", bank + "_" + soundEvent, "empty character bank or event");
            return false;
        }

        string key = bank + "\0" + ChrEventKey(soundEvent);
        if (!ByChrBankEvent.TryGetValue(key, out List<Entry> list) || list.Count == 0)
        {
            error = FormatMissingWaveError("chrsound", bank + "_" + soundEvent, "no official wave for this bank+event");
            return false;
        }

        entries = list;
        return true;
    }

    public static string ChrEventKey(CharacterSoundEvent soundEvent)
    {
        switch (soundEvent)
        {
            case CharacterSoundEvent.Atk_1H_S:
                return "1H_S_Atk";
            case CharacterSoundEvent.Atk_1H:
                return "1H_Atk";
            case CharacterSoundEvent.Standup:
                return "Standup";
            default:
                return soundEvent.ToString();
        }
    }

    static void IndexChr(Entry entry)
    {
        if (entry.Package.IndexOf("chrsound", StringComparison.OrdinalIgnoreCase) < 0)
            return;

        string bank = ChrBankFromGroup(entry.Group);
        if (string.IsNullOrEmpty(bank))
            return;

        Match m = ChrEventSuffix.Match(entry.Name);
        if (!m.Success)
            return;

        string ev = m.Groups[1].Value;
        if (ev.Equals("Stand", StringComparison.OrdinalIgnoreCase))
            ev = "Standup";
        Add(ByChrBankEvent, bank + "\0" + ev, entry);
    }

    public static string ChrBankFromGroup(string group)
    {
        if (string.IsNullOrEmpty(group))
            return null;
        for (int i = 0; i < GroupBankSuffixes.Length; i++)
        {
            if (group.EndsWith(GroupBankSuffixes[i], StringComparison.OrdinalIgnoreCase))
                return group.Substring(0, group.Length - GroupBankSuffixes[i].Length);
        }

        // Official NPC voice groups are already the bank id (FNpc_Young, MNpc_Giant, …).
        if (group.StartsWith("FNpc_", StringComparison.OrdinalIgnoreCase)
            || group.StartsWith("MNpc_", StringComparison.OrdinalIgnoreCase))
            return group;

        return null;
    }

    static List<Entry> FilterGroup(List<Entry> rows, string groupHint)
    {
        var list = new List<Entry>();
        for (int i = 0; i < rows.Count; i++)
        {
            if (rows[i].Group.Equals(groupHint, StringComparison.OrdinalIgnoreCase))
                list.Add(rows[i]);
        }

        return list.Count > 0 ? list : rows;
    }

    static Entry UniquePath(IReadOnlyList<Entry> matches)
    {
        if (matches == null || matches.Count == 0)
            return null;
        string path = matches[0].ResourcesPath;
        for (int i = 1; i < matches.Count; i++)
        {
            if (!matches[i].ResourcesPath.Equals(path, StringComparison.OrdinalIgnoreCase))
                return null;
        }

        return matches[0];
    }

    static string FormatCandidates(IReadOnlyList<Entry> candidates)
    {
        if (candidates == null || candidates.Count == 0)
            return "(none)";
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var parts = new List<string>();
        for (int i = 0; i < candidates.Count; i++)
        {
            string s = candidates[i].Package + ":" + candidates[i].Group + ":" + candidates[i].ResourcesPath;
            if (seen.Add(s))
                parts.Add(s);
        }

        return string.Join("; ", parts);
    }

    public static string SoundStem(string soundRef)
    {
        if (string.IsNullOrEmpty(soundRef))
            return soundRef;
        int colon = soundRef.LastIndexOf(':');
        return colon >= 0 && colon < soundRef.Length - 1 ? soundRef.Substring(colon + 1) : soundRef;
    }

    static string Capitalize(string s)
    {
        if (string.IsNullOrEmpty(s))
            return s;
        if (s.Length == 1)
            return s.ToUpperInvariant();
        return char.ToUpperInvariant(s[0]) + s.Substring(1).ToLowerInvariant();
    }

    static void Add(Dictionary<string, List<Entry>> map, string key, Entry entry)
    {
        if (!map.TryGetValue(key, out List<Entry> list))
        {
            list = new List<Entry>();
            map[key] = list;
        }

        list.Add(entry);
    }

    static void EnsureLoaded()
    {
        if (!_loaded)
            Load();
    }

    static string DefaultPath() =>
        Path.Combine(Application.streamingAssetsPath, RelativePath);

    static string ResolveDefaultText()
    {
        if (TextOverride != null)
            return TextOverride();
        string path = DefaultPath();
        return File.Exists(path) ? File.ReadAllText(path) : null;
    }

    static void Log(string message)
    {
        ErrorLogged?.Invoke(message);
        Debug.LogError(message);
    }
}
