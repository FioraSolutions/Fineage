using System;
using System.Collections.Generic;

/// <summary>
/// Interlude Actions window and ActionName lookup, driven by
/// <c>ActionName-Interlude-eu.txt</c> (converted from <c>system/actionname-e.dat</c>).
/// Icons, names, categories, and section order come from that table.
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
        public readonly InterludeActionAvailabilityKind Availability;
        public readonly int[] TemplateNpcIds;
        /// <summary>Order within <see cref="Section"/> in the ActionWnd grid (0-based).</summary>
        public readonly int SlotOrder;

        public Entry(
            int id,
            string cmdName,
            InterludeActionSection section,
            string iconStem,
            InterludeActionWireKind wireKind,
            InterludeActionAvailabilityKind availability,
            int[] templateNpcIds,
            int slotOrder = -1)
        {
            Id = id;
            CmdName = cmdName ?? string.Empty;
            Section = section;
            IconStem = iconStem ?? string.Empty;
            WireKind = wireKind;
            Availability = availability;
            TemplateNpcIds = templateNpcIds ?? Array.Empty<int>();
            SlotOrder = slotOrder;
        }
    }

    static Entry[] _all = Array.Empty<Entry>();
    static Dictionary<int, Entry> _byId = new();
    static Dictionary<InterludeActionSection, Entry[]> _bySection = new();
    static bool _loaded;

    public static IReadOnlyList<Entry> Entries
    {
        get
        {
            EnsureLoaded();
            return _all;
        }
    }

    public static void EnsureLoaded()
    {
        if (_loaded)
            return;

        _loaded = true;
        if (!InterludeActionNameFile.TryLoad(out InterludeActionNameFile.Row[] rows))
        {
            _all = Array.Empty<Entry>();
            _byId = new Dictionary<int, Entry>();
            _bySection = new Dictionary<InterludeActionSection, Entry[]>();
            return;
        }

        var entries = new List<Entry>(rows.Length);
        var sectionCounts = new Dictionary<InterludeActionSection, int>();
        for (int i = 0; i < rows.Length; i++)
        {
            InterludeActionNameFile.Row row = rows[i];
            if (!TrySection(row.Category, out InterludeActionSection section))
                continue;

            int slot = -1;
            if (AppearsInSectionGrid(row, section))
            {
                if (!sectionCounts.TryGetValue(section, out slot))
                    slot = 0;
                sectionCounts[section] = slot + 1;
            }

            if (!InterludeActionAvailabilityRules.TryResolve(
                    row,
                    out InterludeActionAvailabilityKind availability,
                    out int[] templateNpcIds))
            {
                availability = InterludeActionAvailabilityKind.Unknown;
                templateNpcIds = Array.Empty<int>();
            }

            entries.Add(new Entry(
                row.Id,
                row.Cmd,
                section,
                row.IconStem,
                WireKindFor(row.Id, section),
                availability,
                templateNpcIds,
                slot));
        }

        _all = entries.ToArray();
        _byId = new Dictionary<int, Entry>(_all.Length);
        for (int i = 0; i < _all.Length; i++)
        {
            if (!_byId.ContainsKey(_all[i].Id))
                _byId.Add(_all[i].Id, _all[i]);
        }

        var lists = new Dictionary<InterludeActionSection, List<Entry>>();
        for (int i = 0; i < _all.Length; i++)
        {
            Entry e = _all[i];
            if (e.SlotOrder < 0)
                continue;
            if (!lists.TryGetValue(e.Section, out List<Entry> list))
            {
                list = new List<Entry>();
                lists[e.Section] = list;
            }
            list.Add(e);
        }

        _bySection = new Dictionary<InterludeActionSection, Entry[]>();
        foreach (KeyValuePair<InterludeActionSection, List<Entry>> kv in lists)
        {
            kv.Value.Sort((a, b) => a.SlotOrder.CompareTo(b.SlotOrder));
            _bySection[kv.Key] = kv.Value.ToArray();
        }
    }

    /// <summary>
    /// Basic / Party / Social: every Interlude row of that category, in dat order.
    /// Pet / Summon: standing commands only (template-bound skill rows stay in
    /// <see cref="Entries"/> for icon lookup, not the default grid).
    /// </summary>
    static bool AppearsInSectionGrid(InterludeActionNameFile.Row row, InterludeActionSection section)
    {
        switch (section)
        {
            case InterludeActionSection.Basic:
            case InterludeActionSection.Party:
            case InterludeActionSection.Social:
                return true;
            case InterludeActionSection.Pet:
            case InterludeActionSection.Summon:
                return row.Id < 1000 && IsStandaloneCommand(row);
            default:
                return false;
        }
    }

    static bool IsStandaloneCommand(InterludeActionNameFile.Row row)
    {
        if (row.Category2 == null || row.Category2.Length == 0)
            return true;
        if (row.Category2.Length == 1 && row.Category2[0] == -2)
            return true;

        // e.g. Summon Attack (id 22) lists many NPC templates but is still a command button.
        switch (row.Cmd)
        {
            case "Change Movement Mode":
            case "Attack":
            case "Stop":
            case "Pickup":
            case "Pick Up":
            case "Unsummon Pet":
            case "Unsummon":
            case "Move":
                return true;
            default:
                return false;
        }
    }

    static bool TrySection(int category, out InterludeActionSection section)
    {
        switch (category)
        {
            case 0:
                section = InterludeActionSection.Special;
                return true;
            case 1:
                section = InterludeActionSection.Basic;
                return true;
            case 2:
                section = InterludeActionSection.Party;
                return true;
            case 3:
                section = InterludeActionSection.Social;
                return true;
            case 4:
                section = InterludeActionSection.Pet;
                return true;
            case 5:
                section = InterludeActionSection.Summon;
                return true;
            default:
                section = InterludeActionSection.Special;
                return false;
        }
    }

    static InterludeActionWireKind WireKindFor(int id, InterludeActionSection section)
    {
        switch (id)
        {
            case 0:
            case 1:
            case 2:
            case 4:
                return InterludeActionWireKind.PlayerMovementCombat;
            case 3:
            case 10:
            case 28:
            case 37:
            case 51:
                return InterludeActionWireKind.TradeOrStore;
            case 6:
            case 7:
            case 8:
            case 9:
            case 50:
                return InterludeActionWireKind.PartyCommand;
        }

        if (section == InterludeActionSection.Social)
            return InterludeActionWireKind.Social;
        if (section == InterludeActionSection.Pet || section == InterludeActionSection.Summon)
            return InterludeActionWireKind.PetSummonCommand;
        return InterludeActionWireKind.NotImplemented;
    }

    public static bool TryGet(int actionId, out Entry entry)
    {
        EnsureLoaded();
        return _byId.TryGetValue(actionId, out entry);
    }

    public static Entry[] SectionSlots(InterludeActionSection section)
    {
        EnsureLoaded();
        return _bySection.TryGetValue(section, out Entry[] rows) ? rows : Array.Empty<Entry>();
    }

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
