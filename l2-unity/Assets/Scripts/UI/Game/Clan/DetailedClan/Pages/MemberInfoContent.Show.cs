public partial class MemberInfoContent
{
    public MemberInfoContent(DataProviderClanInfo dataProvider)
    {
        _dataProvider = dataProvider;
    }

    public void LoadAsset(Func<string, VisualTreeAsset> loaderFunc)
    {
        _tamplateChangeRank = loaderFunc(_templateNameElementChangeRank);
        _tamplateChangeTitle = loaderFunc(_templateNameElementChangeTitle);
    }

    public void PreShow(PledgeReceiveMemberInfoDto memberInfo, VisualElement detailedInfoElement, PledgeShowMemberListAllDto packetAll)
    {
        content = LoadContent(content, detailedInfoElement);
        ClearContent(content);
        SetLeader(memberInfo, packetAll);
        UnregisterCallBackAllButtons();

        Show(memberInfo, detailedInfoElement, packetAll);
    }

    public void Show(PledgeReceiveMemberInfoDto memberInfo, VisualElement detailedInfoElement, PledgeShowMemberListAllDto packetAll)
    {
        VisualElement page = ToolTipsUtils.CloneOne(template);

        _dataProvider.SetMemberInfo(page, memberInfo, packetAll);

        cancelButton = page.Q<Button>(_cancelButtonName);
        var rankButton = page.Q<Button>(_buttonRankName);
        var titleButton = page.Q<Button>(_buttonTitleName);
        var dismissButton = page.Q<Button>(_buttonDismissName);
        _centerBox = page.Q<VisualElement>(_centerBoxContentName);

        var rankEvent = GetSubscribeOnRank(memberInfo.Name, rankButton, _centerBox, detailedInfoElement);
        var titleEvent = GetSubscribeOnTitle(titleButton, _centerBox, detailedInfoElement);
        var dismissEvent = GetSubscribeOnDismiss(memberInfo.Name, dismissButton, _centerBox, detailedInfoElement);



        _selectMemeberName = memberInfo.Name;


        Label dismissLabel = page.Q<Label>(_dismissTitleName);
        Label rankLabel = page.Q<Label>(_labelRankName);
        Label titleLabel= page.Q<Label>(_labelTitleName);

        OnEnabledButton(memberInfo.Name , _isLeader, rankLabel, titleLabel , dismissLabel);




        RegisterCallBackAllButtons(new Button[3] { rankButton, titleButton, dismissButton },
            new EventCallback<ClickEvent>[3] { rankEvent, titleEvent, dismissEvent });

        SubscribeCloseButton(cancelButton, detailedInfoElement);

        if (content != null & page != null)
        {
            content.Add(page);
        }
    }

    private void SetLeader(PledgeReceiveMemberInfoDto memberInfo, PledgeShowMemberListAllDto packetAll)
    {
        _isLeader = StorageNpc.getInstance().GetFirstUser().PlayerInfoInterlude.Identity.Name == packetAll.SubPledgeLeaderName;
    }

    private EventCallback<ClickEvent> GetSubscribeOnRank(string selectName, Button rankButton, VisualElement centerBox, VisualElement detailedInfoElement)
    {
        string selfName = StorageNpc.getInstance().GetFirstUser().PlayerInfoInterlude.Identity.Name;

        return _isLeader && selectName != selfName && rankButton != null && !GetStatusCallBack(0)
            ? new EventCallback<ClickEvent>(evt => OnRankButtonClick(centerBox))
            : null;
    }

    private EventCallback<ClickEvent> GetSubscribeOnTitle(Button titleButton, VisualElement centerBox, VisualElement detailedInfoElement)
    {
        return _isLeader && titleButton != null && !GetStatusCallBack(1)
            ? new EventCallback<ClickEvent>(evt => OnTitleButtonClick(centerBox))
            : null;
    }

    private EventCallback<ClickEvent> GetSubscribeOnDismiss(string selectName, Button rankButton, VisualElement centerBox, VisualElement detailedInfoElement)
    {
        string selfName = StorageNpc.getInstance().GetFirstUser().PlayerInfoInterlude.Identity.Name;

        return _isLeader && selectName != selfName && rankButton != null && !GetStatusCallBack(2)
            ? new EventCallback<ClickEvent>(evt => OnDismissButtonClick(centerBox))
            : null;
    }
}
