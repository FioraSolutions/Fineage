using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Minimal party roster from Zone V2 <see cref="ZoneV2MsgType.PartyMemberList"/> (1029).
/// Click a member to set target + attack target (assist).
/// </summary>
public class PartyMemberListWindow : L2Window
{
    private static PartyMemberListWindow _instance;
    public static PartyMemberListWindow Instance => _instance;

    private VisualElement _memberRows;
    private Label _titleLabel;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            WirePartyPacketServices();
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        WirePartyPacketServices();
    }

    private void OnDestroy()
    {
        if (_instance == this)
            _instance = null;
    }

    private void WirePartyPacketServices()
    {
        PartyPacketServices.Bind(memberList: this);
    }

    protected override void LoadAssets()
    {
        _windowTemplate = LoadAsset("Data/UI/_Elements/Game/PartyMemberListPanel");
    }

    protected override IEnumerator BuildWindow(VisualElement root)
    {
        InitWindow(root);
        _titleLabel = (Label)GetElementById("PartyTitle");
        _memberRows = GetElementById("MemberRows");
        RefreshFromList(PartyPacketServices.LastMemberList);
        yield return new WaitForEndOfFrame();
    }

    public void RefreshFromList(in ZoneV2PartyMemberList list)
    {
        if (_memberRows == null)
            return;

        _memberRows.Clear();

        ZoneV2PartyMemberEntry[] members = list.Members;
        int count = members != null ? members.Length : 0;
        if (_windowEle != null)
            _windowEle.style.display = count > 0 ? DisplayStyle.Flex : DisplayStyle.None;

        if (_titleLabel != null)
            _titleLabel.text = count > 0 ? $"Party ({count})" : "Party";

        if (members == null || count == 0)
            return;

        int leaderId = list.LeaderObjectId;
        for (int i = 0; i < members.Length; i++)
        {
            ZoneV2PartyMemberEntry entry = members[i];
            if (entry.ObjectId <= 0)
                continue;

            bool isLeader = entry.ObjectId == leaderId;
            string rowText = isLeader
                ? $"* {entry.Name}  Lv{entry.Level}"
                : $"{entry.Name}  Lv{entry.Level}";

            var row = new Button { text = rowText };
            row.AddToClassList("party-member-row");
            row.style.unityTextAlign = TextAnchor.MiddleLeft;
            row.style.marginBottom = 2;
            row.style.backgroundColor = new Color(0f, 0f, 0f, 0.35f);
            row.style.color = new Color(0.82f, 0.9f, 0.75f);
            row.style.fontSize = 11;
            row.style.paddingLeft = 4;
            row.style.paddingRight = 4;
            row.style.paddingTop = 2;
            row.style.paddingBottom = 2;

            int objectId = entry.ObjectId;
            row.RegisterCallback<ClickEvent>(_ => OnMemberClicked(objectId));
            _memberRows.Add(row);
        }
    }

    private void OnMemberClicked(int objectId)
    {
        if (!PartyTargetAssistServices.TrySelectPartyMember(objectId))
            Debug.LogWarning("[Party] Could not target party member objectId=" + objectId);
    }
}
