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
                    Armorgrp grp = new Armorgrp();
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
            grp.BodyPart = BodyPartFromInterlude(body);

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
    /// Official Interlude armorgrp body_part numbers, matched against Classic
    /// body_part strings on the overlapping rows (earring=1, neck=3, ring=4,
    /// underwear=0, head=6, gloves=9, chest=10, legs=11, feet=12, cloak=13,
    /// full armor=15, formal wear=16, circlet=17, hair accessory=18, hair=19).
    /// </summary>
    public static ItemSlot BodyPartFromInterlude(int bodyPartCode)
    {
        switch (bodyPartCode)
        {
            case 0:
                return ItemSlot.underwear;
            case 1:
                return ItemSlot.rear;
            case 3:
                return ItemSlot.neck;
            case 4:
                return ItemSlot.rfinger;
            case 6:
                return ItemSlot.head;
            case 9:
                return ItemSlot.gloves;
            case 10:
                return ItemSlot.chest;
            case 11:
                return ItemSlot.legs;
            case 12:
                return ItemSlot.feet;
            case 15:
                return ItemSlot.fullarmor;
            case 16:
                return ItemSlot.alldress;
            case 17:
                return ItemSlot.hat1;
            case 18:
                return ItemSlot.hat2;
            case 19:
                return ItemSlot.hat1;
            default:
                return ItemSlot.none;
        }
    }

    private void InitializeList(List<List<string>> multilist)
    {
        for (int i = 0; i < ModelTable.RACE_COUNT; i++)
        {
            multilist.Add(null);
        }
    }
}
