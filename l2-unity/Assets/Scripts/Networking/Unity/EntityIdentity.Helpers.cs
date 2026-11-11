using UnityEngine;

public partial class EntityIdentity
{
    public void SetL2jPos(float x, float y, float z)
    {
        Vector3 l2jpos = new Vector3(x, y, z);
        _position = VectorUtils.ConvertPosToUnity(l2jpos);
    }

    public void SetHideHp(int npcId)
    {
        _isHideHpBar = ParceHideHpbar(npcId);
    }

    private bool ParceHideHpbar(int npcId)
    {
        Npcgrp npc = TablePacketServices.NpcGrps.GetNpcgrp(npcId);
        if (npc != null)
        {
            if (npc.Type.Equals("monster_normal") | npc.Type.Equals("hide"))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        return false;
    }

    public Vector3 GetL2jPos()
    {
        return VectorUtils.ConvertPosUnityToL2j(_position);
    }

    public Vector3 GetL2jPos(Vector3 originPosition)
    {
        return VectorUtils.ConvertPosUnityToL2j(originPosition);
    }
}
