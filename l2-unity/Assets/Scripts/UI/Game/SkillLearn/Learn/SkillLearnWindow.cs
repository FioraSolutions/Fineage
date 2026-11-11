using System;
using System.Collections.Generic;
using VContainer;
using UnityEngine.UIElements;
using System.Collections;
using UnityEngine;

public partial class SkillLearnWindow : L2PopupWindow
{

    private static SkillLearnWindow _instance;
    protected VisualTreeAsset _rowsTemplate;
    private ICreatorVerticalScroll _creatorWindow;
    private Label _spLabel;
    private List<OtherModel> _list;


    [Obsolete("Use [Inject] / PacketServices instead")]
    public static SkillLearnWindow Instance { get { return _instance; } }






}
