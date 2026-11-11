using UnityEngine;

public partial class MagicSkillUseDto : IWireDto
{
    public void SetAttacker(Entity entity)
    {
        attacker = entity;
    }
}
