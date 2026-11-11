using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public partial class ArmorgrpTable
{
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
