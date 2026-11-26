using UnityEngine;

public partial class EntityIdentity
{
    public void SetL2jPos(float x, float y, float z)
    {
        Vector3 l2jpos = new Vector3(x, y, z);
        _position = VectorUtils.ConvertPosToUnity(l2jpos);
        SetServerPosition(_position);
    }

    public void SetHideHp(bool hideHpBar)
    {
        _isHideHpBar = hideHpBar;
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
