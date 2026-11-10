using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public partial class WeapongrpTable
{
    public void ReadWeaponInterlude()
    {
        //_actionsInterlude = new Dictionary<int, ActionData>();
        string dataPath = Path.Combine(Application.streamingAssetsPath, "Data/Meta/Weapongrp_interlude.txt");

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
                    int id =  Int32.Parse(ids[1]);

                    // Col 24 = handness (0 shield, 1 one-hand, 4 pole/2H). Col 25 is mesh
                    // count — never use it as handness. BodyPartFromInterlude maps handness.
                    int handness = Int32.Parse(ids[24]);
                    int crystalType = Int32.Parse(ids[44]);

                    if (_weaponGrps.ContainsKey(id))
                    {
                        Weapongrp grp = _weaponGrps[id];
                        ApplyInterludeIdentity(grp, ids, handness, crystalType);
                        grp.PAtk = Int32.Parse(ids[41]);
                        grp.Matk = Int32.Parse(ids[42]);
                        grp.CriticalRate = Int32.Parse(ids[45]);

                        grp.Dex = Int32.Parse(ids[47]);
                        grp.ShieldPdef = Int32.Parse(ids[48]);
                        grp.ShieldRate = Int32.Parse(ids[49]);

                        grp.PAtkSpeed = Int32.Parse(ids[50]);
                        grp.MpConsume = Int32.Parse(ids[51]);

                        grp.Soulshot =(byte) Int32.Parse(ids[52]);
                        grp.Spiritshot = (byte)Int32.Parse(ids[53]);
                        grp.DropType = Int32.Parse(ids[2]);
                        grp.DropAnimType = Int32.Parse(ids[3]);
                    }
                    else
                    {
                        Weapongrp grp = new Weapongrp();
                        ApplyInterludeIdentity(grp, ids, handness, crystalType);

                        grp.PAtk = Int32.Parse(ids[41]);
                        grp.Matk = Int32.Parse(ids[42]);
                        grp.CriticalRate = Int32.Parse(ids[45]);

                        grp.Dex = Int32.Parse(ids[47]);
                        grp.ShieldPdef = Int32.Parse(ids[48]);
                        grp.ShieldRate = Int32.Parse(ids[49]);

                        grp.PAtkSpeed = Int32.Parse(ids[50]);
                        grp.MpConsume = Int32.Parse(ids[51]);

                        grp.Soulshot = (byte)Int32.Parse(ids[52]);
                        grp.Spiritshot = (byte)Int32.Parse(ids[53]);
                        grp.DropType = Int32.Parse(ids[2]);
                        grp.DropAnimType = Int32.Parse(ids[3]);

                        _weaponGrps.Add(id , grp);
                    }
                }

                index++;
            }

        }
    }

    /// <summary>
    /// Model / texture / body / grade / weapon type from an Interlude weapongrp row.
    /// Handness col 24: 0 shield, 1 one-hand, 4 pole/two-hand.
    /// </summary>
    static void ApplyInterludeIdentity(Weapongrp grp, string[] ids, int handness, int crystalType)
    {
        var model = DatUtils.ParseArray(ids[7]);
        if (model != null && model.Length > 0 && !string.IsNullOrEmpty(model[0]))
            grp.Model = model[0];

        var modTex = DatUtils.ParseArray(ids[10]);
        if (modTex != null && modTex.Length > 0 && !string.IsNullOrEmpty(modTex[0]))
            grp.Texture = modTex[0];

        var icon = DatUtils.ParseArray(ids[13]);
        if (icon != null && icon.Length > 0)
            grp.Icon = icon[0];

        if (int.TryParse(ids[19], out int weight))
            grp.Weight = weight;
        grp.Material = ItemMaterialParser.Parse(ids[20]);
        grp.BodyPart = BodyPartFromInterlude(handness);
        grp.Grade = GradeFromInterlude(crystalType);

        WeaponType fromHand = WeaponTypeParser.ParseByHandness(handness);
        grp.WeaponType = WeaponClassifier.GetType(fromHand, grp, handness);
        grp.ObjectId = int.Parse(ids[1]);
    }

    /// <summary>
    /// Interlude packed handness used as body: 0 shield, 1 right hand, else two-hand.
    /// </summary>
    static ItemSlot BodyPartFromInterlude(int bodyPartCode)
    {
        switch (bodyPartCode)
        {
            case 0:
                return ItemSlot.lhand_shield;
            case 1:
                return ItemSlot.rhand;
            default:
                return ItemSlot.lrhand;
        }
    }

    /// <summary>Interlude packed crystal_type: 0 none, 1 D … 5 S.</summary>
    static ItemGrade GradeFromInterlude(int crystalType)
    {
        switch (crystalType)
        {
            case 1:
                return ItemGrade.d;
            case 2:
                return ItemGrade.c;
            case 3:
                return ItemGrade.b;
            case 4:
                return ItemGrade.a;
            case 5:
                return ItemGrade.s;
            default:
                return ItemGrade.none;
        }
    }
}
