using FMOD.Studio;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public partial class RankingPrivelege : AbstractClanContent
{

    private ICreatorTables _creatorTableWindows;
    private Func<string, VisualTreeAsset> _loaderFunc;
    private const string master_table_content = "Content";
    private const string button_edit_auth = "EditAuthButton";
    private PledgePowerGradeListDto _rankingList;
    private GradeList _selectRank;
    private ClanDetailedInfo _detailedClan;
    public Action<int> OnSwitchRank;
    private const int EDIT_MODE_ALL_NOT_CHECKED = -1;





}
