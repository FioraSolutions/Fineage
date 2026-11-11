using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public partial class WeapongrpTable
{
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
                            if (AbstractgrpDatParser.ParseBaseAbstractItemGrpDat(weaponGrp, key, value))
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
}
