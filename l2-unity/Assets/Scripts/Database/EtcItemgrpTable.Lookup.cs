using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public partial class EtcItemgrpTable
{
    public void ReadEtcItemInterlude()
    {
        string dataPath = Path.Combine(Application.streamingAssetsPath, "Data/Meta/EtcItemgrp_interlude.txt");
        if (!File.Exists(dataPath))
        {
            // Debug.LogWarning("File not found: " + dataPath);
            return;
        }

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
                if (!IsIndexValid(ids, InterludeId) || !int.TryParse(ids[InterludeId], out int id))
                {
                    index++;
                    continue;
                }

                if (_etcItemGrps.TryGetValue(id, out EtcItemgrp existing))
                {
                    ApplyInterludeDropFields(existing, ids);
                }
                else
                {
                    EtcItemgrp etcItemgrp = new EtcItemgrp();
                    etcItemgrp.ObjectId = id;
                    ApplyInterludeDropFields(etcItemgrp, ids);
                    _etcItemGrps.Add(id, etcItemgrp);
                }

                index++;
            }
        }
    }

    static void ApplyInterludeDropFields(EtcItemgrp grp, string[] ids)
    {
        if (grp == null || ids == null)
            return;

        string dropMesh = ReadCell(ids, InterludeDropMesh1);
        if (!string.IsNullOrEmpty(dropMesh) && string.IsNullOrEmpty(grp.DropModel))
            grp.DropModel = dropMesh;

        string dropTex = ReadCell(ids, InterludeDropTex1);
        if (!string.IsNullOrEmpty(dropTex) && string.IsNullOrEmpty(grp.DropTexture))
            grp.DropTexture = dropTex;

        string icon = ReadCell(ids, InterludeIcon);
        if (!string.IsNullOrEmpty(icon))
            grp.Icon = icon;

        string equip = ReadCell(ids, InterludeEquipMesh);
        if (!string.IsNullOrEmpty(equip) && string.IsNullOrEmpty(grp.Model))
            grp.Model = FirstToken(equip);

        if (TryReadInt(ids, InterludeWeight, out int weight))
            grp.Weight = weight;
        if (TryReadInt(ids, InterludeGrade, out int grade) && grade >= 0 && grade <= 5)
            grp.Grade = (ItemGrade)grade;

        if (TryReadInt(ids, InterludeDropType, out int dropType))
            grp.DropType = dropType;
        if (TryReadInt(ids, InterludeDropAnimType, out int dropAnim))
            grp.DropAnimType = dropAnim;
        if (TryReadInt(ids, InterludeDropRadius, out int dropRadius))
            grp.DropRadius = dropRadius;
        if (TryReadInt(ids, InterludeDropHeight, out int dropHeight))
            grp.DropHeight = dropHeight;

        // Classic key=value rows already have consume_type / etcitem_type. Interlude-only
        // items (Mana Potion 728, …) used to be inserted with defaults: ConsumeType=Normal
        // and EtcItemType=null, so the GM shop treated them as unique trade goods and
        // UseItem NRE'd on EtcItemType.ToString().
        if (TryReadInt(ids, InterludeStackable, out int stackable)
            && grp.ConsumeType == ConsumeCategory.Normal)
        {
            // Do not overwrite a Classic Stackable/Asset value. Enchant scrolls are
            // stackable=0 in Interlude but consume_type_stackable in Classic.
            grp.ConsumeType = ConsumeType.FromInterludeStackable(stackable);
        }

        if (string.IsNullOrEmpty(grp.EtcItemType))
        {
            if (TryReadInt(ids, InterludeFamily, out int family))
                grp.EtcItemType = ConsumeType.EtcItemTypeFromInterludeFamily(family);
            else
                grp.EtcItemType = "none";
        }
    }

    static string ReadCell(string[] ids, int index)
    {
        if (ids == null || index < 0 || index >= ids.Length)
            return null;
        string value = ids[index];
        if (string.IsNullOrWhiteSpace(value))
            return null;
        return value.Trim();
    }

    static bool TryReadInt(string[] ids, int index, out int value)
    {
        value = 0;
        string cell = ReadCell(ids, index);
        return !string.IsNullOrEmpty(cell) && int.TryParse(cell, out value);
    }

    static string FirstToken(string value)
    {
        if (string.IsNullOrEmpty(value))
            return value;
        if (value.IndexOf('[') < 0 && value.IndexOf('{') < 0)
            return value;
        string[] parts = DatUtils.ParseArray(value);
        return parts != null && parts.Length > 0 ? parts[0] : value;
    }

    bool IsIndexValid<T>(T[] array, int index)
    {
        return index >= 0 && index < array.Length;
    }

    /// <summary>
    /// Official client tables mark some Fineage-stackable items as Normal
    /// (stackable column 0 / consume_type_normal). Promote those ids after
    /// Classic+Interlude load; leave Asset and already-Stackable rows alone.
    /// </summary>
    void ApplyServerStackableOverrides()
    {
        if (_etcItemGrps == null)
            return;
        string dataPath = Path.Combine(Application.streamingAssetsPath, "Data/Meta/ItemStackable_override.txt");
        if (!File.Exists(dataPath))
            return;
        foreach (int id in ConsumeType.ReadStackableOverrideIds(File.ReadAllLines(dataPath)))
        {
            if (_etcItemGrps.TryGetValue(id, out EtcItemgrp grp))
                grp.ConsumeType = ConsumeType.PromoteServerStackable(grp.ConsumeType);
        }
    }
}
