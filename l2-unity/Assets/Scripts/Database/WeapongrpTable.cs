using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class WeapongrpTable {
    private static WeapongrpTable _instance;

    [Obsolete("Use [Inject] WeapongrpTable or TablePacketServices.WeaponGrps instead")]
    public static WeapongrpTable Instance {
        get {
            if (_instance == null && App.HasContainer) {
                try {
                    _instance = App.Resolve<WeapongrpTable>();
                } catch {
                }
            }

            if (_instance == null) {
                _instance = new WeapongrpTable();
            }

            return _instance;
        }
    }

    public static void Bind(WeapongrpTable table) {
        if (table != null)
            _instance = table;
    }

    private Dictionary<int, Weapongrp> _weaponGrps;
    public Dictionary<int, Weapongrp> WeaponGrps { get { return _weaponGrps; } }

    public void Initialize() {
        ReadWeaponGrpDat();
        ReadWeaponInterlude();
    }

    public Weapongrp GetWeapon(int id)
    {
        if (_weaponGrps.ContainsKey(id))
        {
            return _weaponGrps[id];
        }
        return null;
    }

    private void ReadWeaponGrpDat() {
        _weaponGrps = new Dictionary<int, Weapongrp>();

        string dataPath = Path.Combine(Application.streamingAssetsPath, "Data/Meta/Weapongrp_Classic.txt");
        if (!File.Exists(dataPath)) {
            // Debug.LogWarning("File not found: " + dataPath);
            return;
        }

        using (StreamReader reader = new StreamReader(dataPath)) {
            string line;
            while ((line = reader.ReadLine()) != null) {

                string[] keyvals = line.Split('\t');

                Weapongrp weaponGrp = new Weapongrp();

                WeaponType weaponType = WeaponType.none;
                //WeaponMaterial weaponMaterial = WeaponMaterial.None;
                int handness = 0;
                string[] modTex;

                for (int i = 0; i < keyvals.Length; i++) {
                    if (!keyvals[i].Contains("=")) {
                        continue;
                    }

                    string[] keyval = keyvals[i].Split("=");
                    string key = keyval[0];
                    string value = keyval[1];

                    if (!key.Equals("icon"))
                    {
                        if (!key.Equals("weight"))
                        {
                            if (DatUtils.ParseBaseAbstractItemGrpDat(weaponGrp, key, value))
                            {
                                continue;
                            }
                        }

                    }
  

                    switch (key) {              
                        case "body_part": //artifact_a1 = chest, artifact_a2 = legs, artifact_a3 = boots, head = head, artifactbook = gloves, rfinger, lfinger, rear, lear, onepiece,
                            weaponGrp.BodyPart = ItemSlotParser.ParseBodyPart(value); 
                            break;             
                        case "mp_consume": //mp_consume=0
                            weaponGrp.MpConsume = int.Parse(value);
                            break;                  
                        case "handness":
                            handness = int.Parse(value);
                            break;
                        case "weapon_type":
                            weaponType = WeaponTypeParser.Parse(value);
                            break;
                        case "soulshot_count":
                            weaponGrp.Soulshot = byte.Parse(value);
                            break;
                        case "spiritshot_count":
                            weaponGrp.Spiritshot = byte.Parse(value);
                            break;
                        case "weight":
                            weaponGrp.Weight = int.Parse(value);
                           break;
                        case "icon":
                            weaponGrp.Icon = DatUtils.ParseArray(value)[0];
                            break;
                        case "is_magic_weapon":
                            weaponGrp.IsMagicWeapon = int.Parse(value);
                            break;
                        case "wp_mesh": //{{[LineageWeapons.hell_knife_m00_wp]};{1}}
                            //TODO for dualswords, store 2 models and textures
                            modTex = DatUtils.ParseArray(value);
                            weaponGrp.Model = modTex[0];
                            break;
                        case "texture": //{[LineageWeaponsTex.hell_knife_t00_wp]}	
                            //TODO for dualswords, store 2 models and textures
                            modTex = DatUtils.ParseArray(value);
                            weaponGrp.Texture = modTex[0];
                            break;
                        case "item_sound": // {[ItemSound.sword_small_2];[ItemSound.public_sword_shing_9];[ItemSound.sword_small_7];[ItemSound.dagger_4]}
                            string[] itemsounds = DatUtils.ParseArray(value);
                            weaponGrp.ItemSounds = itemsounds;
                            break;                 
                    }
                }


                weaponType = WeaponClassifier.GetType(weaponType, weaponGrp, handness);
                weaponGrp.WeaponType = weaponType;

                if (!TablePacketServices.Items.ShouldLoadItem(weaponGrp.ObjectId)) {
                    continue;
                }

                if (!_weaponGrps.ContainsKey(weaponGrp.ObjectId))
                {
                    _weaponGrps.Add(weaponGrp.ObjectId, weaponGrp);
                }
                
            }

            // Debug.Log($"Successfully imported {_weaponGrps.Count} weapongrps(s)");
        }
    }

    

    //41 P atk
    //42 M atk
    //45 Crit rate
    //50 P tak speed
    //51 Mana (MpConsume)
    //52 Soulshot
    //53 SpiritShot

    //49 - shield_rate
    //47 - dex
    //48 - shield_pdef
    public void ReadWeaponInterlude()
    {
        //_actionsInterlude = new Dictionary<int, ActionData>();
        string dataPath = Path.Combine(Application.streamingAssetsPath, "Data/Meta/Weapongrp_interlude.txt");

        using (StreamReader reader = new StreamReader(dataPath))
        {
            string line;
            int index = 0;
            while ((line = reader.ReadLine()) != null)
            {
                //string[] test = line.Split('\t');
                if (index != 0)
                {
                    string[] ids = line.Split('\t');
                    int id =  Int32.Parse(ids[1]);

                    // Col 24 = handness (0 shield, 1 one-hand, 4 pole/2H). Col 25 is mesh
                    // count — never use it as handness. BodyPartFromInterlude maps handness.
                    int handness = Int32.Parse(ids[24]);
                    int crystalType = Int32.Parse(ids[44]);

                    if (_weaponGrps.ContainsKey(id))
                    {
                        Weapongrp grp = _weaponGrps[id];
                        ApplyInterludeIdentity(grp, ids, handness, crystalType);
                        grp.PAtk = Int32.Parse(ids[41]);
                        grp.Matk = Int32.Parse(ids[42]);
                        grp.CriticalRate = Int32.Parse(ids[45]);

                        grp.Dex = Int32.Parse(ids[47]);
                        grp.ShieldPdef = Int32.Parse(ids[48]);
                        grp.ShieldRate = Int32.Parse(ids[49]);

                        grp.PAtkSpeed = Int32.Parse(ids[50]);
                        grp.MpConsume = Int32.Parse(ids[51]);

                        grp.Soulshot =(byte) Int32.Parse(ids[52]);
                        grp.Spiritshot = (byte)Int32.Parse(ids[53]);
                        grp.DropType = Int32.Parse(ids[2]);
                        grp.DropAnimType = Int32.Parse(ids[3]);
                    }
                    else
                    {
                        Weapongrp grp = new Weapongrp();
                        ApplyInterludeIdentity(grp, ids, handness, crystalType);

                        grp.PAtk = Int32.Parse(ids[41]);
                        grp.Matk = Int32.Parse(ids[42]);
                        grp.CriticalRate = Int32.Parse(ids[45]);

                        grp.Dex = Int32.Parse(ids[47]);
                        grp.ShieldPdef = Int32.Parse(ids[48]);
                        grp.ShieldRate = Int32.Parse(ids[49]);

                        grp.PAtkSpeed = Int32.Parse(ids[50]);
                        grp.MpConsume = Int32.Parse(ids[51]);

                        grp.Soulshot = (byte)Int32.Parse(ids[52]);
                        grp.Spiritshot = (byte)Int32.Parse(ids[53]);
                        grp.DropType = Int32.Parse(ids[2]);
                        grp.DropAnimType = Int32.Parse(ids[3]);

                        _weaponGrps.Add(id , grp);
                    }
                }

                index++;
            }

        }
    }

    /// <summary>
    /// Model / texture / body / grade / weapon type from an Interlude weapongrp row.
    /// Handness col 24: 0 shield, 1 one-hand, 4 pole/two-hand.
    /// </summary>
    static void ApplyInterludeIdentity(Weapongrp grp, string[] ids, int handness, int crystalType)
    {
        var model = DatUtils.ParseArray(ids[7]);
        if (model != null && model.Length > 0 && !string.IsNullOrEmpty(model[0]))
            grp.Model = model[0];

        var modTex = DatUtils.ParseArray(ids[10]);
        if (modTex != null && modTex.Length > 0 && !string.IsNullOrEmpty(modTex[0]))
            grp.Texture = modTex[0];

        var icon = DatUtils.ParseArray(ids[13]);
        if (icon != null && icon.Length > 0)
            grp.Icon = icon[0];

        if (int.TryParse(ids[19], out int weight))
            grp.Weight = weight;
        grp.Material = ItemMaterialParser.Parse(ids[20]);
        grp.BodyPart = BodyPartFromInterlude(handness);
        grp.Grade = GradeFromInterlude(crystalType);

        WeaponType fromHand = WeaponTypeParser.ParseByHandness(handness);
        grp.WeaponType = WeaponClassifier.GetType(fromHand, grp, handness);
        grp.ObjectId = int.Parse(ids[1]);
    }

    /// <summary>
    /// Interlude packed handness used as body: 0 shield, 1 right hand, else two-hand.
    /// </summary>
    static ItemSlot BodyPartFromInterlude(int bodyPartCode)
    {
        switch (bodyPartCode)
        {
            case 0:
                return ItemSlot.lhand_shield;
            case 1:
                return ItemSlot.rhand;
            default:
                return ItemSlot.lrhand;
        }
    }

    /// <summary>Interlude packed crystal_type: 0 none, 1 D … 5 S.</summary>
    static ItemGrade GradeFromInterlude(int crystalType)
    {
        switch (crystalType)
        {
            case 1:
                return ItemGrade.d;
            case 2:
                return ItemGrade.c;
            case 3:
                return ItemGrade.b;
            case 4:
                return ItemGrade.a;
            case 5:
                return ItemGrade.s;
            default:
                return ItemGrade.none;
        }
    }
}
