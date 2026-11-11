using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public partial class ArmorgrpTable
{
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
}
