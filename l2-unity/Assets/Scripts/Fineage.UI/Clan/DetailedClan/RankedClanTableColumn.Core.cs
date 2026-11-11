using System.Collections.Generic;
using UnityEngine;

public partial class RankedClanTableColumn : MonoBehaviour
{
    // Powers only — GradeList lives in Fineage.Networking; Fineage.UI must stay cycle-safe.
    public static void ForEachClan(IReadOnlyList<int> powers, ICreatorTables _creatorTableWindows)
    {
        var rankList = new List<string>();
        var quantityList = new List<string>();

        for (int i = 0; i < 15; i++)
        {
            if (i < powers.Count)
            {
                rankList.Add(GetTextRank(i));
                quantityList.Add(powers[i].ToString());
            }
            else
            {
                rankList.Add("");
                quantityList.Add("");
            }
        }

        List<TableColumn> listTableColumn = GetColumns(rankList, quantityList);
        _creatorTableWindows.CreateTable(listTableColumn);
    }
}
