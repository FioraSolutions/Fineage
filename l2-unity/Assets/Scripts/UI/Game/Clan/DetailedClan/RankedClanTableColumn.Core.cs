using System.Collections.Generic;
using UnityEngine;

public partial class RankedClanTableColumn : MonoBehaviour
{
    public static void ForEachClan(List<GradeList> gradeList , ICreatorTables _creatorTableWindows)
    {


        var rankList = new List<string>();
        var quantityList = new List<string>();

        for (int i = 0; i < 15; i++)
        {
            if (i < gradeList.Count)
            {
                rankList.Add(GetTextRank(i));
                quantityList.Add(gradeList[i].GetPower().ToString());
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
