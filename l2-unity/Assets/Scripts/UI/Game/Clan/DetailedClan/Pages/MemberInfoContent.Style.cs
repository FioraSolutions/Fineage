public partial class MemberInfoContent
{
    private void OnEnabledButton(string selectName, bool isLeader, UnityEngine.UIElements.Label rankLabel,
UnityEngine.UIElements.Label titleLabel, UnityEngine.UIElements.Label dismisslabel)
    {
        ApplyLeaderStyles(isLeader, rankLabel, titleLabel, dismisslabel);
        ApplySelfRestrictions(selectName, rankLabel, dismisslabel);
    }

    private void ApplyLeaderStyles(bool isLeader, UnityEngine.UIElements.Label rankLabel,
    UnityEngine.UIElements.Label titleLabel, UnityEngine.UIElements.Label dismisslabel)
    {
        var labels = new[] { rankLabel, titleLabel, dismisslabel };
        string activeStyle = isLeader ? USS_STYLE_YELLOW : USS_STYLE_DISABLED;
        string inactiveStyle = isLeader ? USS_STYLE_DISABLED : USS_STYLE_YELLOW;

        foreach (var label in labels)
        {
            UpdateLabelStyle(label, activeStyle, inactiveStyle);
        }
    }

    private void ApplySelfRestrictions(string selectName, UnityEngine.UIElements.Label rankLabel,
    UnityEngine.UIElements.Label dismisslabel)
    {
        string selfName = StorageNpc.getInstance().GetFirstUser().PlayerInfoInterlude.Identity.Name;

        if (selectName == selfName)
        {
            UpdateLabelStyle(rankLabel, USS_STYLE_DISABLED, USS_STYLE_YELLOW);
            UpdateLabelStyle(dismisslabel, USS_STYLE_DISABLED, USS_STYLE_YELLOW);
        }
    }

    private void UpdateLabelStyle(UnityEngine.UIElements.Label label, string addStyle, string removeStyle)
    {
        label.RemoveFromClassList(removeStyle);
        label.AddToClassList(addStyle);
    }
}
