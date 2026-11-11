using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class StorageNpc
{
    private StorageNpc()
    { }

    public static StorageNpc getInstance()
    {
        if (instance == null)
        {
            instance = new StorageNpc();
            npcs = new Dictionary<int, NpcInfoDto>();
            users = new Dictionary<int, UserInfoDto>();
            chars = new Dictionary<int, CharInfoDto>();
        }
            
        return instance;
    }

    public void AddNpcInfo(NpcInfoDto npc)
    {
        lock (_sync)
        {
            if (!npcs.ContainsKey(npc.Identity.Id))
            {
                npcs.Add(npc.Identity.Id, npc);
            }
            else
            {
                npcs.Remove(npc.Identity.Id);
                npcs.Add(npc.Identity.Id, npc);
            }
           
        }
    }

    public void AddCharInfo(CharInfoDto info)
    {
        if (info == null || info.Identity == null)
            return;
        lock (_sync)
        {
            if (chars == null)
                chars = new Dictionary<int, CharInfoDto>();
            chars[info.Identity.Id] = info;
        }
    }

    public void RemoveNpc(int objectId)
    {
        lock (_sync)
        {
            if (npcs != null)
                npcs.Remove(objectId);
        }
    }

    public void RemoveChar(int objectId)
    {
        lock (_sync)
        {
            if (chars != null)
                chars.Remove(objectId);
        }
    }
}
