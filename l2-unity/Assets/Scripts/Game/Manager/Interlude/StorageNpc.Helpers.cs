using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class StorageNpc
{
    public void ClearKnownlist()
    {
        lock (_sync)
        {
            npcs?.Clear();
            chars?.Clear();
        }
    }

    public NpcInfoDto[] CopyNpcs()
    {
        lock (_sync)
        {
            if (npcs == null || npcs.Count == 0)
                return new NpcInfoDto[0];
            NpcInfoDto[] copy = new NpcInfoDto[npcs.Count];
            npcs.Values.CopyTo(copy, 0);
            return copy;
        }
    }

    public CharInfoDto[] CopyChars()
    {
        lock (_sync)
        {
            if (chars == null || chars.Count == 0)
                return new CharInfoDto[0];
            CharInfoDto[] copy = new CharInfoDto[chars.Count];
            chars.Values.CopyTo(copy, 0);
            return copy;
        }
    }

    public void AddUserInfo(UserInfoDto user)
    {
        lock (_sync)
        {
            if (!users.ContainsKey(user.PlayerInfoInterlude.Identity.Id))
            {
                users.Add(user.PlayerInfoInterlude.Identity.Id, user);
            }
            else
            {
                users.Remove(user.PlayerInfoInterlude.Identity.Id);
                users.Add(user.PlayerInfoInterlude.Identity.Id, user);
            }

        }
    }

    public UserInfoDto GetFirstUser()
    {
        if(users.Count > 0)
        {
            return users.Values.First();
        }
        return null;
    }

    public NpcInfoDto GetNpcInfo(int objId)
    {
        //lock (_sync)
        //{
            return (npcs.ContainsKey(objId)) ? npcs[objId] : null;
        //}
    }

    public UserInfoDto GetUserInfo(int objId)
    {
        //lock (_sync)
        //{
            return (users.ContainsKey(objId)) ? users[objId] : null;
        ////}
    }
}
