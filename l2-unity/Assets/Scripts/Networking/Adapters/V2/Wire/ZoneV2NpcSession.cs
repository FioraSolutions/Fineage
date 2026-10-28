using System;
using System.Collections.Generic;

/// <summary>
/// Talk range and the NPC the open dialog, shop, or warehouse belongs to.
/// Attackable is remembered from <c>NpcInfo</c> so a click can talk instead of swing.
/// </summary>
public static class ZoneV2NpcSession
{
    public const int InteractionDistance = 150;

    static readonly object Gate = new object();
    static readonly Dictionary<int, bool> Attackable = new Dictionary<int, bool>();
    static readonly Dictionary<int, string> Names = new Dictionary<int, string>();

    public static int ActiveNpcObjectId { get; private set; }
    public static int ShopNpcObjectId { get; private set; }
    public static int ShopListId { get; private set; }
    public static bool WindowsOpen { get; private set; }

    public static void Reset()
    {
        lock (Gate)
        {
            Attackable.Clear();
            Names.Clear();
        }

        ActiveNpcObjectId = 0;
        ShopNpcObjectId = 0;
        ShopListId = 0;
        WindowsOpen = false;
    }

    public static void RememberNpc(int objectId, string name, bool attackable)
    {
        if (objectId <= 0)
            return;
        lock (Gate)
        {
            Attackable[objectId] = attackable;
            if (!string.IsNullOrEmpty(name))
                Names[objectId] = name;
        }
    }

    public static void Forget(int objectId)
    {
        lock (Gate)
        {
            Attackable.Remove(objectId);
            Names.Remove(objectId);
        }
    }

    /// <summary>
    /// Known flag wins. When NpcInfo has not arrived, monsters stay attackable and other NPCs do not.
    /// </summary>
    public static bool IsAttackable(int objectId, bool spawnedAsMonster)
    {
        lock (Gate)
        {
            bool flag;
            if (Attackable.TryGetValue(objectId, out flag))
                return flag;
        }

        return spawnedAsMonster;
    }

    public static string NameOf(int objectId)
    {
        lock (Gate)
        {
            string name;
            return Names.TryGetValue(objectId, out name) ? name : "";
        }
    }

    public static void OpenDialog(int npcObjectId)
    {
        if (npcObjectId > 0)
            ActiveNpcObjectId = npcObjectId;
        WindowsOpen = true;
    }

    public static void OpenShop(int npcObjectId, int listId)
    {
        if (npcObjectId > 0)
        {
            ActiveNpcObjectId = npcObjectId;
            ShopNpcObjectId = npcObjectId;
        }

        ShopListId = listId;
        WindowsOpen = true;
    }

    public static void CloseWindows()
    {
        WindowsOpen = false;
    }
}

/// <summary>HTML bypass strings the renderer already stripped of a leading <c>bypass -h</c>.</summary>
public static class ZoneV2Bypass
{
    public const int Talk = 0;
    public const int Teleport = 1;
    public const int SupportMagic = 2;
    public const int GiveBlessing = 3;

    public static string Normalize(string command)
    {
        if (string.IsNullOrWhiteSpace(command))
            return "";
        string s = command.Trim();
        bool trimmed = true;
        while (trimmed && s.Length > 0)
        {
            trimmed = false;
            if (Starts(s, "bypass -h "))
            {
                s = s.Substring("bypass -h ".Length).Trim();
                trimmed = true;
            }
            else if (Starts(s, "bypass "))
            {
                s = s.Substring("bypass ".Length).Trim();
                trimmed = true;
            }
            else if (Starts(s, "-h "))
            {
                s = s.Substring(3).Trim();
                trimmed = true;
            }
        }

        if (Starts(s, "npc_"))
        {
            int second = s.IndexOf('_', 4);
            int id;
            if (second > 4 && int.TryParse(s.Substring(4, second - 4), out id))
                s = s.Substring(second + 1).Trim();
        }

        return s;
    }

    public static int Classify(string command, out string listName, out int locationId)
    {
        listName = "NORMAL";
        locationId = 0;
        string s = Normalize(command);
        if (s.Length == 0)
            return Talk;
        if (EqualsWord(s, "SupportMagic"))
            return SupportMagic;
        if (EqualsWord(s, "GiveBlessing"))
            return GiveBlessing;
        if (Starts(s, "teleport "))
        {
            string rest = s.Substring("teleport ".Length).Trim();
            return ParseTeleport(rest, out listName, out locationId) ? Teleport : Talk;
        }

        if (Starts(s, "goto "))
        {
            string rest = s.Substring("goto ".Length).Trim();
            int only;
            if (int.TryParse(rest, out only))
            {
                listName = "NORMAL";
                locationId = only;
                return Teleport;
            }

            return ParseTeleport(rest, out listName, out locationId) ? Teleport : Talk;
        }

        return Talk;
    }

    static bool ParseTeleport(string rest, out string listName, out int locationId)
    {
        listName = "NORMAL";
        locationId = 0;
        if (string.IsNullOrEmpty(rest))
            return false;
        int space = rest.IndexOf(' ');
        if (space < 0)
            return int.TryParse(rest, out locationId);
        listName = rest.Substring(0, space).Trim();
        return int.TryParse(rest.Substring(space + 1).Trim(), out locationId) && listName.Length > 0;
    }

    static bool Starts(string s, string prefix)
    {
        return s.StartsWith(prefix, StringComparison.OrdinalIgnoreCase);
    }

    static bool EqualsWord(string s, string word)
    {
        return string.Equals(s, word, StringComparison.OrdinalIgnoreCase);
    }
}
