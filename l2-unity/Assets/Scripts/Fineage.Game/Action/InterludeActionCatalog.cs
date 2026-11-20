using System;
using System.Collections.Generic;

/// <summary>
/// Canonical Interlude (C6) Actions window layout, icons, and metadata.
/// Sources: <c>ActionName_Classic-eu.txt</c> rows (ids 0–66, categories),
/// <c>ref/l2-interlude-client/interface.xdat</c> ActionWnd slot order (mirrored in
/// <c>Assets/Resources/Data/UI/_Elements/Game/Action/ActionWindow.uxml</c>),
/// Fineage-Backend <c>ActionProtocol.cs</c> / <c>ZoneWorldActions.cs</c> (RequestActionUse + SocialAction 1:1).
/// </summary>
public static class InterludeActionCatalog
{
    public readonly struct Entry
    {
        public readonly int Id;
        public readonly string CmdName;
        public readonly InterludeActionSection Section;
        public readonly string IconStem;
        public readonly InterludeActionWireKind WireKind;
        /// <summary>Order within <see cref="Section"/> in the ActionWnd grid (0-based).</summary>
        public readonly int SlotOrder;

        public Entry(
            int id,
            string cmdName,
            InterludeActionSection section,
            string iconStem,
            InterludeActionWireKind wireKind,
            int slotOrder = -1)
        {
            Id = id;
            CmdName = cmdName ?? string.Empty;
            Section = section;
            IconStem = iconStem ?? string.Empty;
            WireKind = wireKind;
            SlotOrder = slotOrder;
        }
    }

    static readonly Entry[] All =
    {
        // Basic — ActionWnd 6×3, first 12 slots filled (ActionName category=1, ActionWindow.uxml Basic).
        new Entry(0, "Sit/Stand", InterludeActionSection.Basic, "action001", InterludeActionWireKind.PlayerMovementCombat, 0),
        new Entry(1, "Walk/Run", InterludeActionSection.Basic, "action002", InterludeActionWireKind.PlayerMovementCombat, 1),
        new Entry(2, "Attack", InterludeActionSection.Basic, "action003", InterludeActionWireKind.PlayerMovementCombat, 2),
        new Entry(3, "Trade", InterludeActionSection.Basic, "action004", InterludeActionWireKind.TradeOrStore, 3),
        new Entry(4, "Next Target", InterludeActionSection.Basic, "action005", InterludeActionWireKind.PlayerMovementCombat, 4),
        new Entry(5, "Pick Up", InterludeActionSection.Basic, "action006", InterludeActionWireKind.NotImplemented, 5),
        new Entry(6, "Assist", InterludeActionSection.Basic, "action007", InterludeActionWireKind.PartyCommand, 6),
        new Entry(10, "Private Store - Sell", InterludeActionSection.Basic, "action010", InterludeActionWireKind.TradeOrStore, 7),
        new Entry(28, "Private Store - Buy", InterludeActionSection.Basic, "action028", InterludeActionWireKind.TradeOrStore, 8),
        new Entry(37, "Dwarven Workshop", InterludeActionSection.Basic, "action040", InterludeActionWireKind.TradeOrStore, 9),
        new Entry(38, "Mount/Dismount", InterludeActionSection.Basic, "etc_pet_ride_01", InterludeActionWireKind.NotImplemented, 10),
        new Entry(40, "Recommend", InterludeActionSection.Basic, "action041", InterludeActionWireKind.NotImplemented, 11),

        // Party — category=2 (ActionWindow.uxml Party row).
        new Entry(7, "Rm. Invite", InterludeActionSection.Party, "action011", InterludeActionWireKind.PartyCommand, 0),
        new Entry(8, "Leave Party", InterludeActionSection.Party, "action012", InterludeActionWireKind.PartyCommand, 1),
        new Entry(9, "Dismiss Party Member", InterludeActionSection.Party, "action013", InterludeActionWireKind.PartyCommand, 2),
        new Entry(11, "Party Matching", InterludeActionSection.Party, "action014", InterludeActionWireKind.NotImplemented, 3),
        new Entry(50, "Change Party Leader", InterludeActionSection.Party, "action050", InterludeActionWireKind.NotImplemented, 4),

        // Social — category=4; ActionWindow.uxml Social rows (C6: ids 12–66, no couple emotes 71+).
        new Entry(12, "Announcements", InterludeActionSection.Social, "action015", InterludeActionWireKind.Social, 0),
        new Entry(13, "Victory", InterludeActionSection.Social, "action016", InterludeActionWireKind.Social, 1),
        new Entry(14, "Advance", InterludeActionSection.Social, "action017", InterludeActionWireKind.Social, 2),
        new Entry(24, "Yes", InterludeActionSection.Social, "action020", InterludeActionWireKind.Social, 3),
        new Entry(25, "No", InterludeActionSection.Social, "action021", InterludeActionWireKind.Social, 4),
        new Entry(26, "Bow", InterludeActionSection.Social, "action022", InterludeActionWireKind.Social, 5),
        new Entry(29, "Unaware", InterludeActionSection.Social, "action029", InterludeActionWireKind.Social, 6),
        new Entry(30, "Standby", InterludeActionSection.Social, "action030", InterludeActionWireKind.Social, 7),
        new Entry(31, "Laugh", InterludeActionSection.Social, "action031", InterludeActionWireKind.Social, 8),
        new Entry(33, "Applaud", InterludeActionSection.Social, "action032", InterludeActionWireKind.Social, 9),
        new Entry(34, "Dance", InterludeActionSection.Social, "action033", InterludeActionWireKind.Social, 10),
        new Entry(35, "Sorrow", InterludeActionSection.Social, "action034", InterludeActionWireKind.Social, 11),
        new Entry(62, "Charm", InterludeActionSection.Social, "action051", InterludeActionWireKind.Social, 12),
        new Entry(66, "Shyness", InterludeActionSection.Social, "Action055", InterludeActionWireKind.Social, 13),

        // Pet — category=5 (Alt+C Pet tab; ActionName ids 15–19).
        new Entry(15, "Change Movement Mode", InterludeActionSection.Pet, "action046", InterludeActionWireKind.PetSummonCommand, 0),
        new Entry(16, "Attackers", InterludeActionSection.Pet, "action047", InterludeActionWireKind.PetSummonCommand, 1),
        new Entry(17, "Stop", InterludeActionSection.Pet, "action048", InterludeActionWireKind.PetSummonCommand, 2),
        new Entry(18, "Pick Up", InterludeActionSection.Pet, "action052", InterludeActionWireKind.PetSummonCommand, 3),
        new Entry(19, "Unsummon Pet", InterludeActionSection.Pet, "action054", InterludeActionWireKind.PetSummonCommand, 4),

        // Summon — category=6 (Alt+C Summon tab; shared baseline 21–23).
        new Entry(21, "Change Movement Mode", InterludeActionSection.Summon, "action057", InterludeActionWireKind.PetSummonCommand, 0),
        new Entry(22, "Attackers", InterludeActionSection.Summon, "action060", InterludeActionWireKind.PetSummonCommand, 1),
        new Entry(23, "Stop", InterludeActionSection.Summon, "action062", InterludeActionWireKind.PetSummonCommand, 2),
        new Entry(52, "Liberate", InterludeActionSection.Summon, "action103", InterludeActionWireKind.PetSummonCommand, 3),
        new Entry(53, "Move", InterludeActionSection.Summon, "action105", InterludeActionWireKind.PetSummonCommand, 4),
    };

    static readonly Dictionary<int, Entry> ById = BuildById();
    static readonly Dictionary<InterludeActionSection, Entry[]> BySection = BuildBySection();

    static Dictionary<int, Entry> BuildById()
    {
        var map = new Dictionary<int, Entry>();
        for (int i = 0; i < All.Length; i++)
        {
            Entry e = All[i];
            if (!map.ContainsKey(e.Id))
                map.Add(e.Id, e);
        }
        return map;
    }

    static Dictionary<InterludeActionSection, Entry[]> BuildBySection()
    {
        var lists = new Dictionary<InterludeActionSection, List<Entry>>();
        for (int i = 0; i < All.Length; i++)
        {
            Entry e = All[i];
            if (e.SlotOrder < 0)
                continue;
            if (!lists.TryGetValue(e.Section, out List<Entry> list))
            {
                list = new List<Entry>();
                lists[e.Section] = list;
            }
            list.Add(e);
        }

        var result = new Dictionary<InterludeActionSection, Entry[]>();
        foreach (KeyValuePair<InterludeActionSection, List<Entry>> kv in lists)
        {
            kv.Value.Sort((a, b) => a.SlotOrder.CompareTo(b.SlotOrder));
            result[kv.Key] = kv.Value.ToArray();
        }
        return result;
    }

    public static IReadOnlyList<Entry> Entries => All;

    public static bool TryGet(int actionId, out Entry entry) => ById.TryGetValue(actionId, out entry);

    public static Entry[] SectionSlots(InterludeActionSection section) =>
        BySection.TryGetValue(section, out Entry[] rows) ? rows : Array.Empty<Entry>();

    public static int[] SectionSlotActionIds(InterludeActionSection section)
    {
        Entry[] rows = SectionSlots(section);
        var ids = new int[rows.Length];
        for (int i = 0; i < rows.Length; i++)
            ids[i] = rows[i].Id;
        return ids;
    }

    public static bool TryGetIconStem(int actionId, out string iconStem)
    {
        if (TryGet(actionId, out Entry e))
        {
            iconStem = e.IconStem;
            return !string.IsNullOrEmpty(iconStem);
        }

        iconStem = null;
        return false;
    }
}