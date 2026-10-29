using System.Collections.Generic;

/// <summary>
/// Fallback lines when a <c>SystemMessage</c> arrives with an empty <c>text</c>.
/// The server normally sends the line already written. Ids 30–38 are quests,
/// 180–189 are shops, and 190–209 are gatekeeper, warehouse, and newbie magic.
/// </summary>
public static class ZoneV2SystemLines
{
    static readonly Dictionary<int, string> Lines = new Dictionary<int, string>
    {
        { 30, "You have acquired the quest {0}." },
        { 31, "You cannot take on any more quests." },
        { 32, "{0} is complete." },
        { 33, "{0} has been cancelled." },
        { 34, "You have earned {0} ({1})." },
        { 35, "You have obtained {0} Adena." },
        { 36, "You have earned {0} experience and {1} SP." },
        { 37, "{0} is already finished." },
        { 38, "{0} cannot be aborted." },
        { 180, "You do not have enough adena." },
        { 181, "You have exceeded the weight limit." },
        { 182, "Your inventory is full." },
        { 183, "The transaction is complete." },
        { 184, "You have exceeded the quantity that can be inputted." },
        { 185, "You have exceeded your out-of-pocket adena limit." },
        { 186, "{0} unit(s) of the item {1} is/are required." },
        { 187, "That item cannot be traded." },
        { 188, "That item is out of stock." },
        { 189, "Wear is not available." },
        { 190, "You do not have enough adena." },
        { 191, "That destination is not on a loaded map." },
        { 192, "You cannot teleport to a village that is in a siege." },
        { 193, "You have been teleported." },
        { 194, "Your warehouse is full." },
        { 195, "Your inventory is full." },
        { 196, "You have exceeded the weight limit." },
        { 197, "You do not have enough adena." },
        { 198, "The transaction is complete." },
        { 199, "You are too low level for supplemental magic." },
        { 200, "You are too high level for supplemental magic." },
        { 201, "Only new characters may receive supplemental magic." },
        { 202, "Only adventurers who have not completed their 3rd class transfer may receive these buffs." },
        { 203, "The Blessing of Protection is only for characters under level 40 who have not completed their second class transfer." },
        { 204, "That teleport is not available." },
        { 205, "Noblesse teleport is not available yet." },
        { 206, "That item cannot be stored." },
        { 208, "There is no supplemental magic for your level." },
        { 209, "Finish enchanting before using the warehouse." }
    };

    public static string Resolve(int id, string text, string[] parms)
    {
        if (!string.IsNullOrEmpty(text))
            return text;
        string template;
        if (!Lines.TryGetValue(id, out template))
            return "";
        if (parms == null || parms.Length == 0)
            return template;
        string line = template;
        for (int i = 0; i < parms.Length; i++)
            line = line.Replace("{" + i + "}", parms[i] ?? "");
        return line;
    }

    public static string BuffRefusal(string code)
    {
        if (string.IsNullOrEmpty(code))
            return "";
        switch (code)
        {
            case "low": return Lines[199];
            case "high": return Lines[200];
            case "newbie": return Lines[201];
            case "class": return Lines[202];
            case "none": return Lines[208];
            default: return "";
        }
    }
}
