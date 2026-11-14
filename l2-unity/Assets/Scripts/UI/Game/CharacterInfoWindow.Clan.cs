using System;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Interlude <c>MainWnd.ClanWnd</c>: clan header, member list, and the drawer buttons
/// (member info, authority, notice, clan info, penalty, leave, war, join, title).
/// </summary>
public partial class CharacterInfoWindow
{
    const int ClanPageList = 0;
    const int ClanPageMember = 1;
    const int ClanPageInfo = 2;
    const int ClanPagePenalty = 3;
    const int ClanPageAuth = 4;
    const int ClanPageNotice = 5;
    const int ClanPageHeroes = 6;

    VisualElement _clanPages;
    ScrollView _clanRows;
    Label _clanNameLine;
    Label _clanLeaderLine;
    Label _clanHallLine;
    Label _clanStatusLine;
    Label _clanLevelLine;
    Label _clanCountLine;
    Label _clanPageBody;
    int _clanPage;
    int _clanSelectedId;
    bool _clanBuilt;

    static readonly Color ClanDim = new Color(149f / 255f, 149f / 255f, 149f / 255f);
    static readonly Color ClanText = new Color(223f / 255f, 222f / 255f, 222f / 255f);
    static readonly Color ClanGold = new Color(214f / 255f, 204f / 255f, 177f / 255f);

    public void RefreshClanTab()
    {
        if (_clanHost == null)
            return;
        EnsureClanChrome();
        PaintClanHeader();
        if (_clanPage == ClanPageList)
            PaintClanRows();
        else
            PaintClanPage();
    }

    void EnsureClanChrome()
    {
        if (_clanBuilt)
            return;
        _clanBuilt = true;
        _clanHost.Clear();

        _clanNameLine = ClanLine(6, 4, 160, ClanText);
        _clanLevelLine = ClanLine(168, 4, 80, ClanGold);
        _clanLevelLine.style.unityTextAlign = TextAnchor.MiddleRight;
        _clanLeaderLine = ClanLine(6, 18, 244, ClanText);
        _clanHallLine = ClanLine(6, 32, 244, ClanText);
        _clanStatusLine = ClanLine(6, 46, 120, ClanText);
        _clanCountLine = ClanLine(130, 46, 118, ClanGold);
        _clanCountLine.style.unityTextAlign = TextAnchor.MiddleRight;
        _clanHost.Add(_clanNameLine);
        _clanHost.Add(_clanLevelLine);
        _clanHost.Add(_clanLeaderLine);
        _clanHost.Add(_clanHallLine);
        _clanHost.Add(_clanStatusLine);
        _clanHost.Add(_clanCountLine);

        var head = new Label("Name            Lv   Class");
        head.style.position = Position.Absolute;
        head.style.left = 6;
        head.style.top = 64;
        head.style.width = 244;
        head.style.height = 14;
        head.style.fontSize = 10;
        head.style.color = ClanDim;
        ApplyClanFont(head);
        _clanHost.Add(head);

        _clanPages = new VisualElement();
        _clanPages.style.position = Position.Absolute;
        _clanPages.style.left = 4;
        _clanPages.style.top = 78;
        _clanPages.style.width = 248;
        _clanPages.style.height = 160;
        _clanHost.Add(_clanPages);

        _clanRows = new ScrollView(ScrollViewMode.Vertical);
        _clanRows.style.position = Position.Absolute;
        _clanRows.style.left = 0;
        _clanRows.style.top = 0;
        _clanRows.style.width = 248;
        _clanRows.style.height = 160;
        _clanPages.Add(_clanRows);

        _clanPageBody = new Label();
        _clanPageBody.style.position = Position.Absolute;
        _clanPageBody.style.left = 4;
        _clanPageBody.style.top = 0;
        _clanPageBody.style.width = 240;
        _clanPageBody.style.height = 130;
        _clanPageBody.style.fontSize = 11;
        _clanPageBody.style.color = ClanText;
        _clanPageBody.style.whiteSpace = WhiteSpace.Normal;
        _clanPageBody.style.display = DisplayStyle.None;
        ApplyClanFont(_clanPageBody);
        _clanPages.Add(_clanPageBody);

        var back = ClanButton("Back", 4, 132, 70);
        back.style.display = DisplayStyle.None;
        back.clicked += () => ShowClanPage(ClanPageList);
        back.name = "ClanPageBack";
        _clanPages.Add(back);

        var dismiss = ClanButton("Dismiss", 78, 132, 70);
        dismiss.style.display = DisplayStyle.None;
        dismiss.name = "ClanPageDismiss";
        dismiss.clicked += DismissSelectedMember;
        _clanPages.Add(dismiss);

        var party = ClanButton("Party", 152, 132, 70);
        party.style.display = DisplayStyle.None;
        party.name = "ClanPageParty";
        party.clicked += InviteSelectedToParty;
        _clanPages.Add(party);

        // interface.xdat ClanWnd footer, in file order, three columns.
        string[] labels =
        {
            "Member Info", "Heroes", "Authority",
            "Notice", "Clan Info", "Penalty",
            "Leave", "War Info", "Declare War",
            "Cancel War", "Join", "Edit Auth",
            "Title"
        };
        Action[] clicks =
        {
            () => ShowClanPage(ClanPageMember),
            () => ShowClanPage(ClanPageHeroes),
            () => ShowClanPage(ClanPageAuth),
            () => ShowClanPage(ClanPageNotice),
            () => ShowClanPage(ClanPageInfo),
            () => ShowClanPage(ClanPagePenalty),
            LeaveClan,
            () => NetChatFeed.NoteSystem("Clan war is not available."),
            () => NetChatFeed.NoteSystem("Clan war is not available."),
            () => NetChatFeed.NoteSystem("Clan war is not available."),
            InviteToClan,
            () => ShowClanPage(ClanPageAuth),
            () => NetChatFeed.NoteSystem("Clan titles are not available.")
        };
        for (int i = 0; i < labels.Length; i++)
        {
            int col = i % 3;
            int row = i / 3;
            var button = ClanButton(labels[i], 4 + col * 82, 242 + row * 18, 78);
            int index = i;
            button.clicked += clicks[index];
            _clanHost.Add(button);
        }
    }

    void ShowClanPage(int page)
    {
        _clanPage = page;
        bool list = page == ClanPageList;
        _clanRows.style.display = list ? DisplayStyle.Flex : DisplayStyle.None;
        _clanPageBody.style.display = list ? DisplayStyle.None : DisplayStyle.Flex;
        bool member = page == ClanPageMember;
        SetClanExtra("ClanPageBack", !list);
        SetClanExtra("ClanPageDismiss", member);
        SetClanExtra("ClanPageParty", member);
        if (list)
            PaintClanRows();
        else
            PaintClanPage();
    }

    void SetClanExtra(string name, bool show)
    {
        VisualElement element = _clanPages.Q(name);
        if (element != null)
            element.style.display = show ? DisplayStyle.Flex : DisplayStyle.None;
    }

    void PaintClanHeader()
    {
        NetClanBook book = NetClanBook.Shared;
        if (!book.HasClan)
        {
            _clanNameLine.text = "No Clan";
            _clanLevelLine.text = "";
            _clanLeaderLine.text = "Leader";
            _clanHallLine.text = "Clan Hall  None";
            _clanStatusLine.text = "";
            _clanCountLine.text = "";
            return;
        }

        _clanNameLine.text = book.Name;
        _clanLevelLine.text = "Lv. " + book.Level;
        string leader = book.LeaderName();
        _clanLeaderLine.text = "Leader  " + (string.IsNullOrEmpty(leader) ? "-" : leader);
        _clanHallLine.text = "Clan Hall  None";
        _clanStatusLine.text = book.Dissolving ? "Dissolving" : "At Peace";
        int total = book.Members != null ? book.Members.Length : book.MemberCount;
        _clanCountLine.text = book.OnlineCount() + " / " + total;
    }

    void PaintClanRows()
    {
        _clanRows.Clear();
        NetClanBook book = NetClanBook.Shared;
        if (!book.HasClan || book.Members == null)
            return;
        for (int i = 0; i < book.Members.Length; i++)
        {
            NetPledgeMember member = book.Members[i];
            string klass = LobbyRaceCatalog.ClassLabel(member.ClassId);
            if (string.IsNullOrEmpty(klass))
                klass = member.ClassId.ToString();
            var row = new Button();
            row.text = Fit(member.Name, 14) + "  " + member.Level.ToString().PadLeft(2) + "  " + klass;
            row.style.height = 16;
            row.style.marginTop = 0;
            row.style.marginBottom = 0;
            row.style.marginLeft = 0;
            row.style.marginRight = 0;
            row.style.paddingLeft = 2;
            row.style.paddingRight = 0;
            row.style.paddingTop = 0;
            row.style.paddingBottom = 0;
            row.style.fontSize = 10;
            row.style.unityTextAlign = TextAnchor.MiddleLeft;
            row.style.backgroundColor = member.ObjectId == _clanSelectedId
                ? new Color(0.15f, 0.18f, 0.28f, 1f)
                : new Color(0f, 0f, 0f, 0f);
            row.style.color = member.Online ? ClanText : ClanDim;
            row.style.borderTopWidth = 0;
            row.style.borderBottomWidth = 0;
            row.style.borderLeftWidth = 0;
            row.style.borderRightWidth = 0;
            ApplyClanFont(row);
            int id = member.ObjectId;
            row.clicked += () =>
            {
                _clanSelectedId = id;
                PaintClanRows();
            };
            _clanRows.Add(row);
        }
    }

    void PaintClanPage()
    {
        NetClanBook book = NetClanBook.Shared;
        NetPledgeMember member;
        bool hasMember = book.TryMember(_clanSelectedId, out member);
        switch (_clanPage)
        {
            case ClanPageMember:
                if (!hasMember)
                {
                    _clanPageBody.text = "Select a clan member.";
                    break;
                }
                string klass = LobbyRaceCatalog.ClassLabel(member.ClassId) ?? member.ClassId.ToString();
                _clanPageBody.text =
                    member.Name + "\n" +
                    "Level " + member.Level + "   " + klass + "\n" +
                    (member.Online ? "Online" : "Offline") + "\n" +
                    "Order  " + PledgeName(member.PledgeType);
                break;
            case ClanPageInfo:
                _clanPageBody.text = !book.HasClan
                    ? "You are not in a clan."
                    : book.Name + "\nLevel " + book.Level + "\nLeader  " + book.LeaderName() +
                      "\nMembers  " + book.OnlineCount() + " / " + (book.Members != null ? book.Members.Length : 0) +
                      "\n" + (book.Dissolving ? "Dissolving" : "At Peace");
                break;
            case ClanPagePenalty:
                _clanPageBody.text =
                    "Withdrawal  " + Remain(book.CharPenaltyExpiryUnixMs) + "\n" +
                    "Dissolution  " + Remain(book.DissolvingExpiryUnixMs);
                break;
            case ClanPageAuth:
                _clanPageBody.text = "Member privileges are not sent by the server.";
                break;
            case ClanPageNotice:
                _clanPageBody.text = "No notice.";
                break;
            case ClanPageHeroes:
                _clanPageBody.text = "No clan heroes.";
                break;
        }
    }

    void LeaveClan()
    {
        if (!NetClanBook.Shared.HasClan)
            return;
        ConfirmClan("Leave the clan?", () => ZoneCommands.Send(new ClanLeaveRequestCommand()));
    }

    void InviteToClan()
    {
        if (!NetClanBook.Shared.HasClan)
        {
            NetChatFeed.NoteSystem("You are not in a clan.");
            return;
        }

        int target = SelectedWorldTarget();
        if (target <= 0)
        {
            NetChatFeed.NoteSystem("Select a character to invite.");
            return;
        }

        ZoneCommands.Send(new ClanInviteRequestCommand(target, 0));
    }

    void DismissSelectedMember()
    {
        NetClanBook book = NetClanBook.Shared;
        if (!book.HasClan || _clanSelectedId <= 0)
        {
            NetChatFeed.NoteSystem("Select a clan member to dismiss.");
            return;
        }

        int target = _clanSelectedId;
        ConfirmClan("Dismiss this member from the clan?", () => ZoneCommands.Send(new ClanKickRequestCommand(target)));
    }

    void InviteSelectedToParty()
    {
        if (_clanSelectedId <= 0)
            return;
        NetSession session = NetSession.TryGetBound();
        if (session != null)
            _ = session.SendPartyInviteRequest(_clanSelectedId, 0);
    }

    static int SelectedWorldTarget()
    {
        TargetManager targets = ControllerPacketServices.Targets;
        if (targets == null)
            return 0;
        TargetData target = targets.Target != null ? targets.Target : targets.AttackTarget;
        if (target != null && target.Identity != null)
            return target.Identity.Id;
        return 0;
    }

    static void ConfirmClan(string text, Action ok)
    {
        if (SystemMessagePacketServices.Window == null || ok == null)
            return;
        Action closed = null;
        Action accept = null;
        accept = () =>
        {
            SystemMessagePacketServices.Window.OnButtonOk -= accept;
            SystemMessagePacketServices.Window.OnButtonClosed -= closed;
            ok();
        };
        closed = () =>
        {
            SystemMessagePacketServices.Window.OnButtonOk -= accept;
            SystemMessagePacketServices.Window.OnButtonClosed -= closed;
        };
        SystemMessagePacketServices.Window.OnButtonOk += accept;
        SystemMessagePacketServices.Window.OnButtonClosed += closed;
        SystemMessagePacketServices.Window.ShowWindowDialogYesOrNot(text);
    }

    static string PledgeName(int pledgeType)
    {
        if (pledgeType == 0)
            return "Main";
        if (pledgeType == 100)
            return "Academy";
        if (pledgeType >= 1001)
            return "Order";
        if (pledgeType >= 200)
            return "Royal Guard";
        return pledgeType.ToString();
    }

    static string Remain(long unixMs)
    {
        if (unixMs <= 0)
            return "None";
        long left = unixMs - DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        if (left <= 0)
            return "None";
        TimeSpan span = TimeSpan.FromMilliseconds(left);
        return span.Days + "d " + span.Hours + "h";
    }

    static string Fit(string value, int width)
    {
        if (string.IsNullOrEmpty(value))
            return new string(' ', width);
        if (value.Length >= width)
            return value.Substring(0, width);
        return value.PadRight(width);
    }

    Label ClanLine(float left, float top, float width, Color color)
    {
        var label = new Label();
        label.style.position = Position.Absolute;
        label.style.left = left;
        label.style.top = top;
        label.style.width = width;
        label.style.height = 13;
        label.style.marginLeft = 0;
        label.style.marginRight = 0;
        label.style.marginTop = 0;
        label.style.marginBottom = 0;
        label.style.paddingLeft = 0;
        label.style.paddingRight = 0;
        label.style.paddingTop = 0;
        label.style.paddingBottom = 0;
        label.style.fontSize = 11;
        label.style.color = color;
        ApplyClanFont(label);
        return label;
    }

    Button ClanButton(string text, float left, float top, float width)
    {
        var button = new Button { text = text };
        button.style.position = Position.Absolute;
        button.style.left = left;
        button.style.top = top;
        button.style.width = width;
        button.style.height = 16;
        button.style.marginLeft = 0;
        button.style.marginRight = 0;
        button.style.marginTop = 0;
        button.style.marginBottom = 0;
        button.style.paddingLeft = 0;
        button.style.paddingRight = 0;
        button.style.paddingTop = 0;
        button.style.paddingBottom = 0;
        button.style.fontSize = 10;
        button.style.color = ClanGold;
        button.style.backgroundColor = new Color(0.08f, 0.09f, 0.12f, 1f);
        button.style.borderTopWidth = 1;
        button.style.borderBottomWidth = 1;
        button.style.borderLeftWidth = 1;
        button.style.borderRightWidth = 1;
        button.style.borderTopColor = new Color(0.30f, 0.28f, 0.18f, 1f);
        button.style.borderBottomColor = new Color(0.30f, 0.28f, 0.18f, 1f);
        button.style.borderLeftColor = new Color(0.30f, 0.28f, 0.18f, 1f);
        button.style.borderRightColor = new Color(0.30f, 0.28f, 0.18f, 1f);
        ApplyClanFont(button);
        return button;
    }

    void ApplyClanFont(VisualElement element)
    {
        if (_nameLabel != null)
            element.style.unityFontDefinition = _nameLabel.style.unityFontDefinition;
    }
}
