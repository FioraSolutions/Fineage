using System.Collections.Generic;

/// <summary>
/// A zone Reject carries a machine code (the world's protocol constant, e.g. <c>not_owned</c>,
/// <c>range</c>, <c>not_enough_adena</c>) plus an English message. The code was only logged, so a
/// refused action looked like nothing happened: the click went through and the UI stayed silent.
/// This maps the codes a player can actually hit onto a line the system chat can show.
/// </summary>
public static class NetRejectLines
{
    static readonly Dictionary<string, string> Texts = new Dictionary<string, string>
    {
        // interaction / targeting
        { "range", "Too far away." },
        { "target", "That is not a valid target." },
        { "world", "You are not in the world." },
        { "not_in_world", "Enter the world first." },
        { "bad_request", "That action is not possible." },
        { "not_implemented", "That action is not implemented yet." },
        { "npc", "That is not an NPC." },
        { "attackable", "That NPC cannot be talked to." },
        { "action_dead", "You cannot do that while dead." },
        { "action_blocked", "You cannot do that right now." },
        { "action_sitting", "You cannot do that while sitting." },
        { "action_unknown", "Unknown action." },
        { "action_unsupported", "That action is not supported." },
        { "dead", "You are dead." },
        { "blocked", "You cannot do that here." },

        // inventory / items
        { "not_owned", "You do not have that item." },
        { "not_equippable", "You cannot equip that item." },
        { "bad_slot", "That slot is not valid." },
        { "empty", "That slot is empty." },
        { "forbidden", "You cannot use that." },
        { "class_restricted", "Your class cannot use that." },
        { "weight", "You are carrying too much." },
        { "weight_penalty", "Your weight penalty blocks that." },
        { "slots_full", "Your inventory is full." },

        // money and services
        { "not_enough_adena", "You do not have enough adena." },
        { "not_enough_items", "You do not have the required items." },
        { "not_enough_fee", "You do not have enough to pay the fee." },
        { "unknown_teleport", "That teleport is not available." },
        { "destination_unloaded", "That destination is not available." },
        { "siege_blocked", "You cannot teleport while that castle is under siege." },
        { "nobles_deferred", "Only Noblesse may use this teleport." },

        // combat / skills
        { "unknown_skill", "Unknown skill." },
        { "skill_unknown", "Unknown skill." },
        { "skill_passive", "That skill is passive." },
        { "skill_cooltime", "That skill is not ready yet." },
        { "skill_reuse", "That skill is not ready yet." },
        { "skill_mp", "Not enough MP." },
        { "skill_condition", "You cannot use that skill now." },
        { "skill_weapon", "You need the right weapon for that skill." },
        { "skill_range", "The target is out of range." },
        { "skill_target", "That is not a valid target." },
        { "revive_not_dead", "You are not dead." },
        { "revive_unsupported", "You cannot revive here." },
        { "death_already", "You are already dead." },

        // quests
        { "quest_unknown", "Unknown quest." },
        { "quest_not_started", "You have not started that quest." },
        { "quest_already", "You already have that quest." },
        { "quest_conditions", "You do not meet the quest requirements." },
        { "quest_inventory", "Your quest inventory is full." },
    };

    /// <summary>
    /// Player-facing line for a reject, or empty when the code is one the client should not surface
    /// (protocol handshake faults and the like). Unknown codes fall back to the server message.
    /// </summary>
    public static string Resolve(string code, string message)
    {
        if (!string.IsNullOrEmpty(code) && Texts.TryGetValue(code, out var text))
            return text;
        if (IsTechnical(code))
            return "";
        return string.IsNullOrWhiteSpace(message) ? "" : message;
    }

    static bool IsTechnical(string code) =>
        code == "hello_required"
        || code == "truncated"
        || code == "bad_envelope"
        || code == "protocol";
}
