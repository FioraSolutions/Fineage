using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;

/// <summary>
/// Loads Interlude <c>ActionName-Interlude-eu.txt</c> (converted from <c>actionname-e.dat</c>).
/// </summary>
public static class InterludeActionNameFile
{
    public const string RelativePath = "Data/Meta/ActionName-Interlude-eu.txt";

    public sealed class Row
    {
        public int Tag;
        public int Id;
        public int Type;
        public int Category;
        public int[] Category2 = Array.Empty<int>();
        public string Cmd = string.Empty;
        public string Icon = string.Empty;
        public string IconStem = string.Empty;
        public string Name = string.Empty;
        public string Desc = string.Empty;
    }

    public static bool TryLoad(out Row[] rows)
    {
        rows = Array.Empty<Row>();
        string path = Path.Combine(Application.streamingAssetsPath, RelativePath);
        if (!File.Exists(path))
        {
            Debug.LogError(
                "[ActionName] Missing Interlude table at StreamingAssets/" + RelativePath);
            return false;
        }

        try
        {
            var list = new List<Row>();
            foreach (string line in File.ReadAllLines(path))
            {
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("//"))
                    continue;
                if (!TryParseLine(line, out Row row))
                    continue;
                list.Add(row);
            }

            if (list.Count == 0)
            {
                Debug.LogError("[ActionName] Interlude table parsed 0 rows: " + path);
                return false;
            }

            rows = list.ToArray();
            return true;
        }
        catch (Exception ex)
        {
            Debug.LogError("[ActionName] Failed to read " + RelativePath + ": " + ex.Message);
            rows = Array.Empty<Row>();
            return false;
        }
    }

    public static bool TryParseLine(string line, out Row row)
    {
        row = null;
        if (string.IsNullOrEmpty(line) || !line.Contains("="))
            return false;

        row = new Row();
        bool haveId = false;
        string[] keyvals = line.Split('	');
        for (int i = 0; i < keyvals.Length; i++)
        {
            int eq = keyvals[i].IndexOf('=');
            if (eq <= 0)
                continue;

            string key = keyvals[i].Substring(0, eq);
            string value = keyvals[i].Substring(eq + 1);
            switch (key)
            {
                case "tag":
                    row.Tag = ParseInt(value);
                    break;
                case "id":
                    row.Id = ParseInt(value);
                    haveId = true;
                    break;
                case "type":
                    row.Type = ParseInt(value);
                    break;
                case "category":
                    row.Category = ParseInt(value);
                    break;
                case "category2":
                    row.Category2 = ParseIntList(value);
                    break;
                case "cmd":
                    row.Cmd = DatUtils.CleanupString(value);
                    break;
                case "icon":
                    row.Icon = DatUtils.CleanupString(value);
                    row.IconStem = StemFromIcon(row.Icon);
                    break;
                case "name":
                    row.Name = DatUtils.CleanupString(value);
                    break;
                case "desc":
                    row.Desc = DatUtils.CleanupString(value);
                    break;
            }
        }

        return haveId;
    }

    public static string StemFromIcon(string icon)
    {
        if (string.IsNullOrEmpty(icon))
            return string.Empty;
        string[] parts = icon.Split('.');
        return parts.Length > 1 ? parts[parts.Length - 1] : parts[0];
    }

    static int ParseInt(string value)
    {
        return int.Parse(value, CultureInfo.InvariantCulture);
    }

    static int[] ParseIntList(string value)
    {
        string trimmed = DatUtils.CleanupStringNew(value);
        if (string.IsNullOrWhiteSpace(trimmed))
            return Array.Empty<int>();

        string[] parts = trimmed.Split(',');
        var nums = new int[parts.Length];
        int n = 0;
        for (int i = 0; i < parts.Length; i++)
        {
            string p = parts[i].Trim();
            if (p.Length == 0)
                continue;
            nums[n++] = int.Parse(p, CultureInfo.InvariantCulture);
        }

        if (n == nums.Length)
            return nums;
        var exact = new int[n];
        Array.Copy(nums, exact, n);
        return exact;
    }
}
