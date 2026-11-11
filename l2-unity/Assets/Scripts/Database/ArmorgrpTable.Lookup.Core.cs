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
}
