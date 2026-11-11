using NUnit.Framework.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public partial class ItemNameTable
{
    private ItemSets CreateItems(int[] sets1 , string desctiprion)
    {
        if(sets1 != null && sets1.Length > 0)
        {
            return new ItemSets(sets1, desctiprion);
        }
        return null;
    }

    private void  SetDescriptionText(ItemName itemData , string parts , string[] ids)
    {
        if (string.IsNullOrEmpty(parts))
        {
            var oldDescript = DatUtils.CleanupStringOldData(ids[3]);
            itemData.Description = oldDescript;
        }
        else
        {
            string descript_parts = DatUtils.CleanupStringOldData(ids[6]);
            itemData.Description = descript_parts;
        }
    }

    private int[] GetSetsIds(string text)
    {
        if (!string.IsNullOrEmpty(text))
        {

            string[] stringArray = text.Split(',');

            return stringArray
                .Select(s => {
                    int number;
                    return (int.TryParse(s, out number) ? number : 0);
                }).ToArray();
        }
        else
        {
            return null;
        }

    }
}
