using UnityEngine;

public partial class MagicSkillUseDto : IWireDto
{
    public void ReadFrom(PacketReader reader)
    {
        _attackerObjId = reader.ReadI();
        _targetObjId = reader.ReadI();

        _skillId = reader.ReadI();
        // Debug.Log("MagicSkillUse->skillID " + _skillId);
        _skilllvl = reader.ReadI();
        _hittime = reader.ReadI();
        // Debug.Log("MagicSkillUse->hittime " + _hittime);
        _reusedelay = reader.ReadI();

        _aX = reader.ReadI();
        _aY = reader.ReadI();
        _aZ = reader.ReadI();
        _attackerPos = VectorUtils.ConvertPosToUnity(new Vector3(_aX, _aY, _aZ));
        
        _critical = reader.ReadI();

        if(_critical == 1)
        {
            reader.ReadSh();
        }

        _tX = reader.ReadI();
        _tY = reader.ReadI();
        _tZ = reader.ReadI();
        _targetPos = VectorUtils.ConvertPosToUnity(new Vector3(_tX, _tY, _tZ));
        _skillGrp = TablePacketServices.SkillGrps.GetSkill(_skillId, _skilllvl);
    }

    public void InitFromNet(
        int casterObjectId,
        int targetObjectId,
        int skillId,
        int skillLevel,
        int hitTimeMs,
        int reuseDelayMs)
    {
        _attackerObjId = casterObjectId;
        _targetObjId = targetObjectId;
        _skillId = skillId;
        _skilllvl = skillLevel;
        _hittime = hitTimeMs;
        _reusedelay = reuseDelayMs;
        _skillGrp = TablePacketServices.SkillGrps != null
            ? TablePacketServices.SkillGrps.GetSkill(_skillId, _skilllvl)
            : null;
    }
}
