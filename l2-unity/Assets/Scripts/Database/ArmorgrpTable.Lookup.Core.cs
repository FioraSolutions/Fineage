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
        string dataPath = Path.Combine(Application.streamingAssetsPath, "Data/Meta/Armorgrp_interlude.txt");
        if (!File.Exists(dataPath))
            return;

        using (StreamReader reader = new StreamReader(dataPath))
        {
            string line;
            int index = 0;
            while ((line = reader.ReadLine()) != null)
            {
                if (index == 0)
                {
                    index++;
                    continue;
                }

                string[] ids = line.Split('\t');
                if (!IsIndexValid(ids, 1) || !int.TryParse(ids[1], out int id))
                {
                    index++;
                    continue;
                }

                if (_armorgrps.TryGetValue(id, out Armorgrp existing))
                {
                    ApplyInterludeIdentity(existing, ids, id);
                }
                else
                {
                    Armorgrp grp = CreateArmorgrp();
                    ApplyInterludeIdentity(grp, ids, id);
                    _armorgrps.Add(id, grp);
                }

                index++;
            }
        }
    }

    /// <summary>
    /// Icon, body, grade, and defense from an Interlude armorgrp row.
    /// Interlude is the source of truth for those identity fields (Classic may
    /// omit the item entirely, or carry a later-chronicle icon).
    /// </summary>
    void ApplyInterludeIdentity(Armorgrp grp, string[] ids, int id)
    {
        if (grp == null || ids == null)
            return;

        grp.ObjectId = id;

        if (IsIndexValid(ids, indexIcon))
        {
            string icon = FirstArmorToken(ids[indexIcon]);
            if (!string.IsNullOrEmpty(icon))
                grp.Icon = icon;
        }

        if (IsIndexValid(ids, indexWeight) && int.TryParse(ids[indexWeight], out int weight))
            grp.Weight = weight;

        if (IsIndexValid(ids, indexBody) && int.TryParse(ids[indexBody], out int body))
            grp.BodyPart = ItemSlotParser.FromInterludeArmor(body);

        if (IsIndexValid(ids, indexCrystal) && int.TryParse(ids[indexCrystal], out int crystal) &&
            crystal >= 0 && crystal <= 5)
            grp.Grade = (ItemGrade)crystal;

        if (IsIndexValid(ids, indexPdef) && int.TryParse(ids[indexPdef], out int pdef))
            grp.PDef = pdef;
        if (IsIndexValid(ids, indexMdef) && int.TryParse(ids[indexMdef], out int mdef))
            grp.MDef = mdef;
        if (IsIndexValid(ids, indexMpBouns) && int.TryParse(ids[indexMpBouns], out int mp))
            grp.MpBonus = mp;
    }

    static string FirstArmorToken(string value)
    {
        if (string.IsNullOrEmpty(value))
            return value;
        if (value.IndexOf('[') < 0 && value.IndexOf('{') < 0)
            return value;
        string[] parts = DatUtils.ParseArray(value);
        return parts != null && parts.Length > 0 ? parts[0] : value;
    }

    /// <summary>
    /// Every armorgrp row gets per-race model/texture slots, including Interlude-only
    /// rows that have no Classic mesh entry (ModelTable iterates them all).
    /// </summary>
    private Armorgrp CreateArmorgrp()
    {
        Armorgrp grp = new Armorgrp();
        grp.FirstModel = new string[ModelTable.RACE_COUNT];
        grp.FirstTexture = new string[ModelTable.RACE_COUNT];
        grp.AllModels = new List<List<string>>(ModelTable.RACE_COUNT);
        InitializeList(grp.AllModels);
        grp.AllTextures = new List<List<string>>(ModelTable.RACE_COUNT);
        InitializeList(grp.AllTextures);
        return grp;
    }

    private void InitializeList(List<List<string>> multilist)
    {
        for (int i = 0; i < ModelTable.RACE_COUNT; i++)
        {
            multilist.Add(null);
        }
    }
}
