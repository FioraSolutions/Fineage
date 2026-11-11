using System;
using System.Collections.Generic;

public sealed partial class NetClanBook
{
    public bool IsLeader(int objectId)
    {
        return objectId > 0 && HasClan && objectId == LeaderObjectId;
    }

    public string LeaderName()
    {
        NetPledgeMember leader;
        if (TryMember(LeaderObjectId, out leader) && !string.IsNullOrEmpty(leader.Name))
            return leader.Name;
        return "";
    }

    public bool TryMember(int objectId, out NetPledgeMember member)
    {
        for (int i = 0; i < _members.Length; i++)
        {
            if (_members[i].ObjectId == objectId)
            {
                member = _members[i];
                return true;
            }
        }

        member = default;
        return false;
    }

    public bool TryMemberByName(string name, out NetPledgeMember member)
    {
        if (!string.IsNullOrEmpty(name))
        {
            for (int i = 0; i < _members.Length; i++)
            {
                if (string.Equals(_members[i].Name, name, StringComparison.Ordinal))
                {
                    member = _members[i];
                    return true;
                }
            }
        }

        member = default;
        return false;
    }

    public int OnlineCount()
    {
        int count = 0;
        for (int i = 0; i < _members.Length; i++)
        {
            if (_members[i].Online)
                count++;
        }

        return count;
    }
}
