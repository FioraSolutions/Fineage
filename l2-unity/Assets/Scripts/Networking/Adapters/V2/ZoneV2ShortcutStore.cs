using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

/// <summary>
/// Zone V2 has no ShortCutReg/Init wire yet (<see cref="FineageZoneTransport"/> swallows those commands).
/// Persist the bar locally per stable lobby character id so icons survive re-login on this client.
/// </summary>
public static class ZoneV2ShortcutStore
{
    const string PrefPrefix = "Fineage.Shortcuts.";

    public static void Save(int characterId, IEnumerable<Shortcut> shortcuts)
    {
        if (characterId <= 0)
            return;
        var sb = new StringBuilder(128);
        bool first = true;
        if (shortcuts != null)
        {
            foreach (Shortcut s in shortcuts)
            {
                if (s == null)
                    continue;
                if (!first)
                    sb.Append(';');
                first = false;
                // worldSlot = slot + page * 12
                int worldSlot = s.Slot + s.Page * PlayerShortcuts.MAXIMUM_SHORTCUTS_PER_BAR;
                sb.Append(worldSlot).Append(',')
                    .Append(s.Type).Append(',')
                    .Append(s.Id).Append(',')
                    .Append(s.Level);
            }
        }

        PlayerPrefs.SetString(PrefPrefix + characterId, sb.ToString());
        PlayerPrefs.Save();
        Debug.Log("[ZoneV2] shortcuts saved char=" + characterId + " payload=" + sb);
    }

    public static List<Shortcut> Load(int characterId)
    {
        var list = new List<Shortcut>();
        if (characterId <= 0)
            return list;
        string raw = PlayerPrefs.GetString(PrefPrefix + characterId, "");
        if (string.IsNullOrEmpty(raw))
            return list;

        string[] rows = raw.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < rows.Length; i++)
        {
            string[] parts = rows[i].Split(',');
            if (parts.Length < 4)
                continue;
            int worldSlot;
            int type;
            int id;
            int level;
            if (!int.TryParse(parts[0], out worldSlot) ||
                !int.TryParse(parts[1], out type) ||
                !int.TryParse(parts[2], out id) ||
                !int.TryParse(parts[3], out level))
                continue;
            int slot = worldSlot % PlayerShortcuts.MAXIMUM_SHORTCUTS_PER_BAR;
            int page = worldSlot / PlayerShortcuts.MAXIMUM_SHORTCUTS_PER_BAR;
            list.Add(new Shortcut(slot, page, type, id, level));
        }

        Debug.Log("[ZoneV2] shortcuts loaded char=" + characterId + " count=" + list.Count);
        return list;
    }
}
