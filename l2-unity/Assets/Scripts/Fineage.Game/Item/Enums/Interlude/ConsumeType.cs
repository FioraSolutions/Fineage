using System.Collections.Generic;

public class ConsumeType
{
    public static ConsumeCategory ParceCategory(string type2)
    {
        if (type2.Equals("consume_type_stackable"))
        {
            return ConsumeCategory.Stackable;
        }
        else if (type2.Equals("consume_type_normal"))
        {
            return ConsumeCategory.Normal;
        }
        else if (type2.Equals("consume_type_asset"))
        {
            return ConsumeCategory.Asset;
        }

        return ConsumeCategory.Normal;
    }

    /// <summary>
    /// Interlude EtcItemgrp <c>stackable</c> column: 2 = stackable, 3 = asset (adena), 0 = normal.
    /// Classic rows already carry consume_type; this is for Interlude-only items
    /// (e.g. Mana Potion 728) whose drop-only loader used to leave ConsumeType at Normal.
    /// </summary>
    public static ConsumeCategory FromInterludeStackable(int stackable)
    {
        if (stackable == 2)
            return ConsumeCategory.Stackable;
        if (stackable == 3)
            return ConsumeCategory.Asset;
        return ConsumeCategory.Normal;
    }

    /// <summary>
    /// Interlude <c>family</c> column → etcitem_type string. Unambiguous families only;
    /// family 0 (none / shots / enchant scrolls) maps to "none". Never returns null,
    /// so UseItem can call IsScrollType without an NRE when Classic has no row.
    /// </summary>
    public static string EtcItemTypeFromInterludeFamily(int family)
    {
        switch (family)
        {
            case 1: return "scroll";
            case 2: return "arrow";
            case 3: return "potion";
            case 5: return "recipe";
            case 6: return "material";
            case 8: return "castle_guard";
            case 9: return "dye";
            case 15: return "ticket_of_lord";
            default: return "none";
        }
    }

    /// <summary>
    /// Official Classic/Interlude tables leave some server-stackable items
    /// (life stones, saga books, later spellbooks) at ConsumeType.Normal.
    /// Promote only Normal so Classic Asset/Stackable values stay intact.
    /// </summary>
    public static ConsumeCategory PromoteServerStackable(ConsumeCategory current)
    {
        return current == ConsumeCategory.Normal ? ConsumeCategory.Stackable : current;
    }

    /// <summary>
    /// Parse <c>ItemStackable_override.txt</c>: one id per line, <c>#</c> comments.
    /// </summary>
    public static IEnumerable<int> ReadStackableOverrideIds(IEnumerable<string> lines)
    {
        if (lines == null)
            yield break;
        foreach (string raw in lines)
        {
            if (string.IsNullOrWhiteSpace(raw))
                continue;
            string line = raw.Trim();
            if (line[0] == '#')
                continue;
            string token = line.Split((char[])null, 2, System.StringSplitOptions.RemoveEmptyEntries)[0];
            if (int.TryParse(token, out int id) && id > 0)
                yield return id;
        }
    }
}
