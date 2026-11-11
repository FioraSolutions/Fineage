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
    public void ReadItemNameInterlude()
    {
        //_actionsInterlude = new Dictionary<int, ActionData>();
        string dataPath = Path.Combine(Application.streamingAssetsPath, "Data/Meta/Itemname-e_interlude.txt");

        try
        {
            using (StreamReader reader = new StreamReader(dataPath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.IndexOf(".\\0") > -1 | line.IndexOf("a,") > -1)
                    {
                        ItemName itemData = new ItemName();

                        string[] ids = line.Split('\t');
                        //int s1 = line.IndexOf("a,") + 2;
                        //int s2 = line.IndexOf(".\\0");



                        string parts1 = DatUtils.CleanupStringOldData(ids[5]);
                        string desctiptionSet1 = DatUtils.CleanupStringOldData(ids[6]);

                        string parts2 = DatUtils.CleanupStringOldData(ids[7]);
                        string desctiptionSet2 = DatUtils.CleanupStringOldData(ids[8]);


                        itemData.Id = Int32.Parse(ids[0]);
                        itemData.Name = ids[1];
                        // Interlude SoT col 2: SA / special ability name (Focus, Guidance, …).
                        if (ids.Length > 2)
                            itemData.AdditionalName = (ids[2] ?? string.Empty).Trim();
                        // Interlude SoT: Classic-eu often carries later-chronicle text
                        // (e.g. Queen Ant 6660 "P./M. Atk. +4%") that disagrees with Mobius CT0.
                        // Always take the item body text from column 3 — set-bonus text lives in ids[6]/[8].
                        if (ids.Length > 3)
                            itemData.Description = DatUtils.CleanupStringOldData(ids[3]);


                        int[] sets1 = GetSetsIds(parts1);
                        ItemSets itemSets1 = CreateItems(sets1, desctiptionSet1);

                        int[] sets2 = GetSetsIds(parts2);
                        ItemSets itemSet2 = CreateItems(sets2, desctiptionSet2);


                        if (_itemNames.ContainsKey(itemData.Id))
                        {
                            ItemName existing = _itemNames[itemData.Id];
                            if (!string.IsNullOrEmpty(itemData.Name))
                                existing.Name = itemData.Name;
                            existing.AdditionalName = itemData.AdditionalName ?? string.Empty;
                            if (!string.IsNullOrEmpty(itemData.Description))
                                existing.Description = itemData.Description;
                            AddSets(itemData, itemSets1, itemSet2);
                        }
                        else
                        {
                            ItemName itemName = new ItemName();
                            itemName.Id = itemData.Id;
                            itemName.Name = itemData.Name;
                            itemName.AdditionalName = itemData.AdditionalName;
                            itemName.Description = itemData.Description;
                            _itemNames.Add(itemData.Id, itemName);
                        }
                    }

                }

            }
        }
        catch (Exception)
        {
            // Debug.LogError("ItemNametable error parce itemName " + ex.Message);
        }
        

    }

    private void AddSets(ItemName itemData , ItemSets itemSets1, ItemSets itemSet2)
    {
        if (itemSets1 != null & itemSet2 != null)
        {
            _itemNames[itemData.Id].SetSets(new ItemSets[2] { itemSets1, itemSet2 });
        }
        else if (itemSets1 != null)
        {
            _itemNames[itemData.Id].SetSets(new ItemSets[1] { itemSets1 });
        }
    }

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
