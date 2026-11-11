using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public partial class ArmorgrpTable
{
    public  Armorgrp GetArmor(int armor)
    {
        if(_armorgrps.ContainsKey(armor)) return _armorgrps[armor];
        return null;
    }

    private void ReadArmorGrpDat() {
        _armorgrps = new Dictionary<int, Armorgrp>();
        string dataPath = Path.Combine(Application.streamingAssetsPath, "Data/Meta/Armorgrp_Classic.txt");
        if (!File.Exists(dataPath)) {
            // Debug.LogWarning("File not found: " + dataPath);
            return;
        }

        using (StreamReader reader = new StreamReader(dataPath)) {
            string line;
            while ((line = reader.ReadLine()) != null) {
                Armorgrp armorgrp = new Armorgrp();
                armorgrp.FirstModel = new string[ModelTable.RACE_COUNT];
                armorgrp.FirstTexture = new string[ModelTable.RACE_COUNT];

                armorgrp.AllModels = new List<List<string>>(ModelTable.RACE_COUNT);
                InitializeList(armorgrp.AllModels);

                armorgrp.AllTextures = new List<List<string>>(ModelTable.RACE_COUNT);
                InitializeList(armorgrp.AllTextures);

                string[] keyvals = line.Split('\t');
                for (int i = 0; i < keyvals.Length; i++) {
                    if(!keyvals[i].Contains("=")) {
                        continue;
                    }

                    string[] keyval = keyvals[i].Split("=");
                    string key = keyval[0];
                    string value = keyval[1];
                    
                    if(AbstractgrpDatParser.ParseBaseAbstractItemGrpDat(armorgrp, key, value)) {
                        continue;
                    }


                    switch (key) {                
                        case "body_part": //artifact_a1 = chest, artifact_a2 = legs, artifact_a3 = boots, head = head, artifactbook = gloves, rfinger, lfinger, rear, lear, onepiece,
                            armorgrp.BodyPart = ItemSlotParser.ParseBodyPart(value); //TODO for fullbody store 2 models and textures for one item
                            break;
                        case "armor_type":
                            armorgrp.ArmorType = ArmorClassifier.GetArmorType(DatUtils.CleanupString(value));
                            break;
                        // Race mesh/tex: {{[models...]};{[textures...]}}. Never take FirstTexture from
                        // flat ParseArray — onepiece robes put [None] legs in slot [1], so modTex[1]
                        // becomes "None" and EquipFullArmor fails (Dark Crystal / Major Arcana).
                        case "m_HumnFigh":
                            ApplyRaceMeshTextures(armorgrp, CharacterRaceAnimation.MFighter, value);
                            break;
                        case "f_HumnFigh":
                            ApplyRaceMeshTextures(armorgrp, CharacterRaceAnimation.FFighter, value);
                            break;
                        case "m_DarkElf":
                            ApplyRaceMeshTextures(armorgrp, CharacterRaceAnimation.MDarkElf, value);
                            break;
                        case "f_DarkElf":
                            ApplyRaceMeshTextures(armorgrp, CharacterRaceAnimation.FDarkElf, value);
                            break;
                        case "m_Dorf":
                            ApplyRaceMeshTextures(armorgrp, CharacterRaceAnimation.MDwarf, value);
                            break;
                        case "f_Dorf":
                            ApplyRaceMeshTextures(armorgrp, CharacterRaceAnimation.FDwarf, value);
                            break;
                        case "m_Elf":
                            ApplyRaceMeshTextures(armorgrp, CharacterRaceAnimation.MElf, value);
                            break;
                        case "f_Elf":
                            ApplyRaceMeshTextures(armorgrp, CharacterRaceAnimation.FElf, value);
                            break;
                        case "m_HumnMyst":
                            ApplyRaceMeshTextures(armorgrp, CharacterRaceAnimation.MMagic, value);
                            break;
                        case "f_HumnMyst":
                            ApplyRaceMeshTextures(armorgrp, CharacterRaceAnimation.FMagic, value);
                            break;
                        case "m_OrcFigh":
                            ApplyRaceMeshTextures(armorgrp, CharacterRaceAnimation.MOrc, value);
                            break;
                        case "f_OrcFigh":
                            ApplyRaceMeshTextures(armorgrp, CharacterRaceAnimation.FOrc, value);
                            break;
                        case "m_OrcMage":
                            ApplyRaceMeshTextures(armorgrp, CharacterRaceAnimation.MShaman, value);
                            break;
                        case "f_OrcMage":
                            ApplyRaceMeshTextures(armorgrp, CharacterRaceAnimation.FShaman, value);
                            break;
                        case "mp_bonus": //mp_bonus=0
                            armorgrp.MpBonus = int.Parse(value);
                            break;
                        case "weight": //weight
                            armorgrp.Weight = int.Parse(value);
                            break;
                    }
                }

               // ArmorType armorTyme = ArmorClassifier.GetArmorType(armor_type);


                _armorgrps.TryAdd(armorgrp.ObjectId, armorgrp);
            }

            // Debug.Log($"Successfully imported {_armorgrps.Count} armorgrp(s)");
        }
    }
}
