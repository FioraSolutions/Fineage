using UnityEngine;

public partial class MagicSkillUseDto : IWireDto
{
    private int _attackerObjId;
    private int _targetObjId;
    private Vector3 _attackerPos;
    private Vector3 _targetPos;
    private Skillgrp _skillGrp;
    private int _aX;
    private int _aY;
    private int _aZ;

    private int _tX;
    private int _tY;
    private int _tZ;

    private int _skillId;
    private int _skilllvl;
    private int _hittime;
    private int _reusedelay;
    private int _critical;
    private Entity attacker;

    public Entity EntityAttacker { get => attacker; }
    public int SkillId { get => _skillId; }

    public int SkillLvl { get => _skilllvl; }

    public int HitTime { get => _hittime; }

    public int Reusedelay { get => _reusedelay; }

    public int AttackerObjId { get => _attackerObjId; }

    public int TargetId { get => _targetObjId; }

    public Vector3 AttackerPos { get => _attackerPos; }
    public Vector3 TargetPos { get => _targetPos; }

    public Skillgrp SkillGrp { get => _skillGrp; }

    


    /// <summary>Fineage Net <c>MagicSkillUse</c> (1066) — positions come from entities at apply time.</summary>


}
