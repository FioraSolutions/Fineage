using System;
using System.Collections.Generic;

public static class NetClanNotices
{
    public static string LineOrEquip(string code, string message)
    {
        string clan;
        if (TryLine(code, message, out clan))
            return clan;
        return NetEquipNotices.Line(code, message);
    }

    public static bool TryLine(string code, string message, out string line)
    {
        line = null;
        if (string.IsNullOrEmpty(code) || !code.StartsWith("clan_", StringComparison.Ordinal))
            return false;
        if (IsSentence(message))
        {
            line = message.Trim();
            return true;
        }

        line = Map(code);
        return true;
    }

    public static string Map(string code)
    {
        switch (code)
        {
            case "clan_level":
                return "You must be level 10 to create a clan.";
            case "clan_member":
                return "You are already in a clan.";
            case "clan_create_penalty":
                return "You must wait before creating another clan.";
            case "clan_name":
                return "Clan names may use letters and digits only.";
            case "clan_name_length":
                return "A clan name must be 2 to 16 characters.";
            case "clan_name_taken":
                return "That clan name is already taken.";
            case "clan_none":
                return "You are not in a clan.";
            case "clan_leader":
                return "Only the clan leader can do that.";
            case "clan_leader_leave":
                return "The clan leader cannot leave. Dissolve the clan instead.";
            case "clan_pledge":
                return "Only the main clan can be invited to.";
            case "clan_self":
                return "You cannot choose yourself.";
            case "clan_target":
                return "Select a nearby player.";
            case "clan_target_clan":
                return "That player is already in a clan.";
            case "clan_busy":
                return "That player is busy.";
            case "clan_waiting":
                return "That invitation is already pending.";
            case "clan_recruit_penalty":
                return "The clan must wait before recruiting.";
            case "clan_join_penalty":
                return "You must wait before joining a clan.";
            case "clan_full":
                return "The clan is full.";
            case "clan_invite":
                return "That clan invitation has expired.";
            case "clan_combat":
                return "You cannot do that while in combat.";
            case "clan_dismiss_self":
                return "You cannot dismiss yourself.";
            case "clan_dissolving":
                return "The clan is already dissolving.";
            case "clan_not_dissolving":
                return "The clan is not dissolving.";
            case "clan_store":
                return "The clan could not be saved.";
            default:
                return "The clan request was refused.";
        }
    }

    static bool IsSentence(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
            return false;
        string text = message.Trim();
        if (text.StartsWith("clan_", StringComparison.Ordinal))
            return false;
        return text.IndexOf(' ') >= 0;
    }
}
