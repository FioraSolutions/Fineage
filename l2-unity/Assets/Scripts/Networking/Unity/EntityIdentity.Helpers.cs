using UnityEngine;

public partial class EntityIdentity
{
    public void SetL2jPos(float x, float y, float z)
    {
        Vector3 l2jpos = new Vector3(x, y, z);
        _position = VectorUtils.ConvertPosToUnity(l2jpos);
        SetServerPosition(_position);
    }

    public void SetHideHp(int npcId)
    {
        _isHideHpBar = ParceHideHpbar(npcId);
    }

    private bool ParceHideHpbar(int npcId)
    {
        Npcgrp npc = TablePacketServices.NpcGrps != null
            ? TablePacketServices.NpcGrps.GetNpcgrp(npcId)
            : null;
        if (npc == null)
        {
            Debug.LogError("[EntityIdentity] ParceHideHpbar: missing npcgrp for npcId=" + npcId);
            return true;
        }

        // Interlude datapack: hpshowable=0 → overhead HP off; attackable mobs still show in target window.
        if (!npc.HpVisible)
            return true;
        if (npc.Type.Equals("monster_normal", System.StringComparison.OrdinalIgnoreCase)
            || npc.Type.Equals("hide", System.StringComparison.OrdinalIgnoreCase))
            return false;
        return true;
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
