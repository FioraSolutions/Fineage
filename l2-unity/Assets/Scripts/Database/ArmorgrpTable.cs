using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class ArmorgrpTable {
    private static ArmorgrpTable _instance;

    [Obsolete("Use [Inject] ArmorgrpTable or TablePacketServices.ArmorGrps instead")]
    public static ArmorgrpTable Instance {
        get {
            if (_instance == null && App.HasContainer) {
                try {
                    _instance = App.Resolve<ArmorgrpTable>();
                } catch {
                }
            }

            if (_instance == null) {
                _instance = new ArmorgrpTable();
            }

            return _instance;
        }
    }

    public static void Bind(ArmorgrpTable table) {
        if (table != null)
            _instance = table;
    }

    private Dictionary<int, Armorgrp> _armorgrps;

    public Dictionary<int, Armorgrp> ArmorGrps { get { return _armorgrps; } }


    public void Initialize() {
        ReadArmorGrpDat();
        ReadArmorInterlude();
    }

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
                    
                    if(DatUtils.ParseBaseAbstractItemGrpDat(armorgrp, key, value)) {
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
    //329 PDef
    //330 Mdef
    //331 MpBonus

    private int indexPdef = 329;
    private int indexMdef = 330;
    private int indexMpBouns = 331;
    public void ReadArmorInterlude()
    {
        //_actionsInterlude = new Dictionary<int, ActionData>();
        string dataPath = Path.Combine(Application.streamingAssetsPath, "Data/Meta/Armorgrp_interlude.txt");

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
                    int id = Int32.Parse(ids[1]);
  
                    if (_armorgrps.ContainsKey(id))
                    {
                        Armorgrp grp = _armorgrps[id];
                        
                        if (IsIndexValid(ids, indexPdef))
                        {
                            grp.PDef  = Int32.Parse(ids[indexPdef]);
                        }
                        if (IsIndexValid(ids, indexMdef))
                        {
                            grp.MDef  = Int32.Parse(ids[indexMdef]);
                        }
                        if (IsIndexValid(ids, indexMpBouns))
                        {
                            grp.MpBonus = Int32.Parse(ids[indexMpBouns]);
                        }

                    }
                }

                index++;
            }

        }
    }


    private void InitializeList(List<List<string>> multilist)
    {
        for (int i =0; i < ModelTable.RACE_COUNT; i++)
        {
            multilist.Add(null);
        }

    }

    /// <summary>
    /// Parses <c>{{[model0];[model1]};{[tex0];[tex1]}}</c> into AllModels/AllTextures and
    /// FirstModel/FirstTexture for one race. First* always come from multi[0][0] / multi[1][0].
    /// </summary>
    static void ApplyRaceMeshTextures(Armorgrp armorgrp, CharacterRaceAnimation race, string value)
    {
        byte raceIndex = (byte)race;
        List<List<string>> multi = DatUtils.ParseMultiArray(value);
        if (multi == null || multi.Count < 2)
            return;

        List<string> models = multi[0];
        List<string> textures = multi[1];
        armorgrp.AllModels[raceIndex] = models;
        armorgrp.AllTextures[raceIndex] = textures;

        armorgrp.FirstModel[raceIndex] = FirstNonNone(models);
        armorgrp.FirstTexture[raceIndex] = FirstNonNone(textures);
    }

    static string FirstNonNone(List<string> values)
    {
        if (values == null)
            return null;
        for (int i = 0; i < values.Count; i++)
        {
            string v = values[i];
            if (!string.IsNullOrEmpty(v) &&
                !string.Equals(v, "None", StringComparison.OrdinalIgnoreCase))
                return v;
        }
        return values.Count > 0 ? values[0] : null;
    }

    bool IsIndexValid<T>(T[] array, int index)
    {
        return index >= 0 && index < array.Length;
    }
}
