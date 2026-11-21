using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using VContainer;

public partial class QuestNameTable
{
    private static QuestNameTable _instance;
    private static Dictionary<int, List<QuestName>> _questNames;

    private const int indexQuestId = 1;
    private const int indexQuestProg = 2;
    private const int indexMainName = 3;
    private const int indexProgname = 4;
    private const int indexDescription = 5;
    private const int indexDescriptionToolTips = 53;
    private const int indexQuestType = 41;
    private const int indexClanPetQuest = 128;

    private const int indexLvlMin = 39;
    private const int indexLvlMax = 40;
    private const int indexRequirements = 50;

    private const int indexEntity_name = 42;
    //maybe not true
    private const int indexRepaet= 41;
    private const int indexAreaId = 131;
    private const int indexQuestWorldX = 36;
    private const int indexQuestWorldY = 37;
    private const int indexQuestWorldZ = 38;
    private const int indexContactNpcId = 46;
    private const int indexContactNpcX = 47;
    private const int indexContactNpcY = 48;
    private const int indexContactNpcZ = 49;
    private const int indexItem0 = 7;
    private const int indexNumItem0 = 22;
    [Obsolete("Use [Inject] / PacketServices instead")]
    public static QuestNameTable Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new QuestNameTable();
                _questNames = new Dictionary<int, List<QuestName>>();
            }

            return _instance;
        }
    }



  






}
