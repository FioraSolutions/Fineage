using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public partial class QuestNameTable
{
    public void ReadQuestNameInterlude(string dataPath)
    {

        using (StreamReader reader = new StreamReader(dataPath))
        {
            string line;
            int index = 0;
            while ((line = reader.ReadLine()) != null)
            {
                if (index != 0)
                {
                    string[] ids = line.Split('\t');
                    int id = Int32.Parse(ids[indexQuestId]);
                    QuestName quest =  CreateModel(ids , id);

                    if (!_questNames.ContainsKey(id))
                    {
                        var  list = new List<QuestName>{quest};
                        _questNames.Add(id , list);
                    }
                    else
                    {
                        _questNames[id].Add(quest);
                    }
                }
                index++;
            }

        }

    }
}
