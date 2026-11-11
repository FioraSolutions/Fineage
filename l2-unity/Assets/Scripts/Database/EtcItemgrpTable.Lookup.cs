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
                    if (string.IsNullOrEmpty(existing.DropModel))
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
        if (!string.IsNullOrEmpty(icon) && string.IsNullOrEmpty(grp.Icon))
            grp.Icon = icon;

        string equip = ReadCell(ids, InterludeEquipMesh);
        if (!string.IsNullOrEmpty(equip) && string.IsNullOrEmpty(grp.Model))
            grp.Model = FirstToken(equip);

        if (TryReadInt(ids, InterludeDropType, out int dropType))
            grp.DropType = dropType;
        if (TryReadInt(ids, InterludeDropAnimType, out int dropAnim))
            grp.DropAnimType = dropAnim;
        if (TryReadInt(ids, InterludeDropRadius, out int dropRadius))
            grp.DropRadius = dropRadius;
        if (TryReadInt(ids, InterludeDropHeight, out int dropHeight))
            grp.DropHeight = dropHeight;
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
}}
