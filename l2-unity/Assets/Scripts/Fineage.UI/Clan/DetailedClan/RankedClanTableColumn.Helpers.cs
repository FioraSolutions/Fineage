using System.Collections.Generic;
using UnityEngine;

public partial class RankedClanTableColumn : MonoBehaviour
{
    private static List<TableColumn> GetColumns(List<string> rankList, List<string> quantityList)
    {
        var name = new TableColumn(false, "Rank", 140, rankList, 13);
        var lvl = new TableColumn(false, "Quantity", 10, quantityList, 0);

        return new List<TableColumn> { name, lvl };
    }

    private static string GetTextRank(int i)
    {
        switch (i)
        {
            case 0:
                return firstRang;
            case 1:
                return twoRang;
            case 2:
                return treeRang;
            case 3:
                return forRang;
            case 4:
                return fiveRang;
            case 5:
                return mainClanRang;
            case 6:
                return kingClanRang;
            case 7:
                return knightClanRang;
            case 8:
                return academyClanRang;
            default:
                return "";
        }
    }
}
