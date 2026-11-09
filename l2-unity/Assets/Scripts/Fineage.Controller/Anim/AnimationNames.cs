using System;

public static class AnimationNames
{
    public static readonly Animation ATK_WAIT = new Animation("atkwait_" , TypesAnimation.OtherType);
    public static readonly Animation WAIT = new Animation("wait_", TypesAnimation.OtherType);
    public static readonly Animation WALK = new Animation("walk_", TypesAnimation.OtherType);
    public static readonly Animation RUN = new Animation("run_", TypesAnimation.OtherType);
    public static readonly Animation STRIDER_WAIT = new Animation("strider_wait", TypesAnimation.OtherType);
    public static readonly Animation STRIDER_WALK = new Animation("strider_walk", TypesAnimation.OtherType);
    public static readonly Animation STRIDER_RUN = new Animation("strider_run", TypesAnimation.OtherType);
    public static readonly Animation DEAD = new Animation("death", TypesAnimation.OtherType);
    public static readonly Animation REBIRTH = new Animation("rebirth", TypesAnimation.OtherType);
    public static readonly Animation ATK01 = new Animation("jatk01_", TypesAnimation.MeleeAttack);
    public static readonly Animation ATK02 = new Animation("jatk02_", TypesAnimation.MeleeAttack);
    public static readonly Animation ATK03 = new Animation("jatk03_", TypesAnimation.MeleeAttack);

    public static readonly Animation MONSTER_WAIT = new Animation("wait", TypesAnimation.OtherType);
    public static readonly Animation MONSTER_WALK = new Animation("walk", TypesAnimation.OtherType);
    public static readonly Animation MONSTER_RUN = new Animation("run", TypesAnimation.OtherType);
    public static readonly Animation MONSTER_ATK01 = new Animation("atk01", TypesAnimation.OtherType);
    public static readonly Animation MONSTER_DamageAction = new Animation("damageaction", TypesAnimation.OtherType);
    public static readonly Animation MONSTER_DamageAction1 = new Animation("damageaction1", TypesAnimation.OtherType);
}

public static class SpecialAnimationNames
{
    public static readonly Animation ATK_BOW_ATK1 = new Animation("jatk01_bow", TypesAnimation.BowAttack);
    public static readonly Animation ATK_BOW_ATK2 = new Animation("jatk02_bow", TypesAnimation.BowAttack);
    public static readonly Animation ATK_BOW_ATK3 = new Animation("jatk03_bow", TypesAnimation.BowAttack);

    public static readonly Animation ATK01_1HS = new Animation("jatk01_1HS", TypesAnimation.MeleeAttack);
    public static readonly Animation ATK02_1HS = new Animation("jatk02_1HS", TypesAnimation.MeleeAttack);
    public static readonly Animation ATK03_1HS = new Animation("jatk03_1HS", TypesAnimation.MeleeAttack);

    public static readonly Animation ATK01_2HS = new Animation("jatk01_2HS", TypesAnimation.MeleeAttack);
    public static readonly Animation ATK02_2HS = new Animation("jatk02_2HS", TypesAnimation.MeleeAttack);
    public static readonly Animation ATK03_2HS = new Animation("jatk03_2HS", TypesAnimation.MeleeAttack);

    // Dual swings often finish after wall cycle — longer blend into wait/atkwait.
    public static readonly Animation ATK01_DUAL = new Animation(
        "jatk01_dual", TypesAnimation.MeleeAttack, exitCrossFadeDuration: 0.25f);
    public static readonly Animation ATK02_DUAL = new Animation(
        "jatk02_dual", TypesAnimation.MeleeAttack, exitCrossFadeDuration: 0.25f);
    public static readonly Animation ATK03_DUAL = new Animation(
        "jatk03_dual", TypesAnimation.MeleeAttack, exitCrossFadeDuration: 0.25f);

    public static readonly Animation ATK01_POLE = new Animation("jatk01_pole", TypesAnimation.MeleeAttack);
    public static readonly Animation ATK02_POLE = new Animation("jatk02_pole", TypesAnimation.MeleeAttack);
    public static readonly Animation ATK03_POLE = new Animation("jatk03_pole", TypesAnimation.MeleeAttack);

    public static readonly Animation SpAtk_01 = new Animation("SpAtk01_1HS", TypesAnimation.MeleeAttack);
    public static readonly Animation SpAtk_01_bow = new Animation("SpAtk01_bow", TypesAnimation.BowAttack);

    public static readonly Animation MagicShot = new Animation("MagicShot", TypesAnimation.MagicAttack , MagicPhase.End);
    public static readonly Animation CastMid = new Animation("CastMid", TypesAnimation.MagicAttack , MagicPhase.Start);
    public static readonly Animation CastEnd = new Animation("CastEnd", TypesAnimation.MagicAttack, MagicPhase.Shot);

    public static readonly Animation[]  arrayAtkSpecials = new Animation[]
    {
        ATK_BOW_ATK1, ATK_BOW_ATK2, ATK_BOW_ATK3,
        ATK01_1HS, ATK02_1HS, ATK03_1HS,
        ATK01_2HS, ATK02_2HS, ATK03_2HS,
        ATK01_DUAL, ATK02_DUAL, ATK03_DUAL,
        ATK01_POLE, ATK02_POLE, ATK03_POLE
    };

    public static readonly Animation[] arrayPhisicalAtkSpecials = new Animation[] { SpAtk_01 , SpAtk_01_bow };

    public static readonly Animation[] arrayMagicAtkSpecials = new Animation[] { CastMid , CastEnd , MagicShot };
    public static Animation[] GetSpecialsAttackAnimations()
    {
            return arrayAtkSpecials;
    }

    public static Animation[] GetPhisicalSkillsAnimations()
    {
        return arrayPhisicalAtkSpecials;
    }

    public static Animation[] GetMagicSkillsAnimations()
    {
        return arrayMagicAtkSpecials;
    }

}
