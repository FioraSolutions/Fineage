public partial class MemberInfoContent
{
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
