using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;

public class NpcgrpTable {
    private static NpcgrpTable _instance;

    [Obsolete("Use [Inject] NpcgrpTable or TablePacketServices.NpcGrps instead")]
    public static NpcgrpTable Instance {
        get {
            if (_instance == null && App.HasContainer) {
                try {
                    _instance = App.Resolve<NpcgrpTable>();
                } catch {
                }
            }

            if (_instance == null) {
                _instance = new NpcgrpTable();
            }

            return _instance;
        }
    }

    public static void Bind(NpcgrpTable table) {
        if (table != null)
            _instance = table;
    }

    private Dictionary<int, Npcgrp> _npcgrps;
    private NpcDecoEffectsTable _decoEffects;

    public Dictionary<int, Npcgrp> Npcgrps { get { return _npcgrps; } }

    public void Initialize() {
        Initialize(null);
    }

    public void Initialize(NpcDecoEffectsTable decoEffects) {
        _decoEffects = decoEffects;
        if (_decoEffects != null)
            _decoEffects.Initialize();
        ReadNpcgrps();
    }

    private void ReadNpcgrps() {
        _npcgrps = new Dictionary<int, Npcgrp>();
        string dataPath = Path.Combine(Application.streamingAssetsPath, "Data/Meta/Npcgrp_Classic.txt");
        if (!File.Exists(dataPath)) {
            // Debug.LogWarning("File not found: " + dataPath);
            return;
        }

        using (StreamReader reader = new StreamReader(dataPath)) {
            string line;
            while ((line = reader.ReadLine()) != null) {
                Npcgrp npcgrp = new Npcgrp();

                string[] keyvals = line.Split('\t');

                for (int i = 0; i < keyvals.Length; i++) {
                    if (!keyvals[i].Contains("=")) {
                        continue;
                    }

                    string[] keyval = keyvals[i].Split("=");
                    string key = keyval[0];
                    string value = keyval[1];

                    switch (key) {
                        case "npc_id": 
                            npcgrp.NpcId = int.Parse(value); 
                            break;
                        case "class_name": //[LineageNPC.e_warehouse_keeper_FDwarf]	
                            npcgrp.ClassName = DatUtils.CleanupString(value);
                            break;
                        case "mesh_name": //[LineageNPCs.e_warehouse_keeper_FDwarf_m00]	
                            npcgrp.Mesh = DatUtils.CleanupString(value);
                            break;
                        case "texture_name": //{[LineageNPCsTex.e_warehouse_keeper_FDwarf_m00_t00_b00];[LineageNPCsTex.e_warehouse_keeper_FDwarf_m00_t00_b01];...}
                            var allMaterials = DatUtils.ParseMultiArray(value);
                            npcgrp.Materials = allMaterials[0].ToArray();
                            break;
                        case "attack_sound1": //{[ItemSound.fist_1];[ItemSound.fist_2];[ItemSound.fist_3]}	
                            npcgrp.AttackSounds = DatUtils.ParseArray(value);
                            break;
                        case "defense_sound1": //{[ItemSound.armor_underwear_1];[ItemSound.armor_underwear_2];[ItemSound.armor_underwear_3];[ItemSound.armor_underwear_4];[ItemSound.armor_leather_7]}
                            npcgrp.DefenseSounds = DatUtils.ParseArray(value);
                            break;
                        case "damage_sound": //{[ChrSound.FNpc_Young_Dmg_1];[ChrSound.FNpc_Young_Dmg_2];[ChrSound.FNpc_Young_Dmg_3]}
                            npcgrp.DamageSounds = DatUtils.ParseArray(value);
                            break;
                        case "attack_effect": //[LineageEffect.p_u002_a]
                            npcgrp.AttackEffect = DatUtils.CleanupString(value);
                            break;
                        case "npc_speed": //{[LineageNPCsTex.e_warehouse_keeper_FDwarf_m00_t00_b00];[LineageNPCsTex.e_warehouse_keeper_FDwarf_m00_t00_b01];...}
                            npcgrp.Speed = float.Parse(value, CultureInfo.InvariantCulture.NumberFormat) / 52.5f;
                            break;
                        case "collision_height":
                            npcgrp.CollisionHeight = float.Parse(value, CultureInfo.InvariantCulture.NumberFormat) / 52.5f;
                            break;
                        case "collision_radius":
                            npcgrp.CollisionRadius = float.Parse(value, CultureInfo.InvariantCulture.NumberFormat) / 52.5f;
                            break;
                        case "social":
                            npcgrp.Friendly = (value == "1");
                            break;
                        case "hpshowable":
                            npcgrp.HpVisible = (value == "1");
                            break;
                        case "slot_rhand":
                            npcgrp.Rhand = int.Parse(value);
                            break;
                        case "slot_lhand":
                            npcgrp.Lhand = int.Parse(value);
                            break;
                        case "org_hp":
                           // npcgrp.MaxHp = float.Parse(value, CultureInfo.InvariantCulture.NumberFormat);
                           npcgrp.MaxHp = float.Parse(value, CultureInfo.InvariantCulture.NumberFormat);
                            break;
                        case "org_mp":
                            npcgrp.MaxMp = float.Parse(value, CultureInfo.InvariantCulture.NumberFormat);
                            break;
                        case "npc_type":
                            string type = DatUtils.CleanupString(value);
                            npcgrp.Type = type;
                            break;
                        case "property_list":
                            ParsePropertyList(npcgrp, value);
                            break;
                    }
                }

                if (_decoEffects != null && _decoEffects.TryGet(npcgrp.NpcId, out NpcDecoEffect deco))
                    npcgrp.Deco = deco;

                _npcgrps.TryAdd(npcgrp.NpcId, npcgrp);
            }

            // Debug.Log($"Successfully imported {_npcgrps.Count} npcgrp(s)");
        }
    }

    public Npcgrp GetNpcgrp(int id) {
        Npcgrp npcgrp = null;
        _npcgrps.TryGetValue(id, out npcgrp);

        if(npcgrp == null) {
            // Debug.LogWarning($"Npcgrp not found for id [{id}]");
        }

        return npcgrp;
    }

    /// <summary>
    /// property_list={4416;13;47399;1;…} — alternating skillId;level (Interlude TargetStatus icons).
    /// </summary>
    static void ParsePropertyList(Npcgrp npcgrp, string value)
    {
        if (npcgrp == null || string.IsNullOrEmpty(value))
            return;
        string[] parts = DatUtils.ParseArray(value);
        if (parts == null || parts.Length < 2)
            return;

        int pairCount = parts.Length / 2;
        int[] ids = new int[pairCount];
        int[] levels = new int[pairCount];
        int written = 0;
        for (int i = 0; i + 1 < parts.Length; i += 2)
        {
            if (!int.TryParse(parts[i], out int skillId) || skillId <= 0)
                continue;
            if (!int.TryParse(parts[i + 1], out int level) || level <= 0)
                level = 1;
            ids[written] = skillId;
            levels[written] = level;
            written++;
        }

        if (written == 0)
            return;
        if (written != pairCount)
        {
            System.Array.Resize(ref ids, written);
            System.Array.Resize(ref levels, written);
        }

        npcgrp.PropertySkillIds = ids;
        npcgrp.PropertySkillLevels = levels;
    }
}