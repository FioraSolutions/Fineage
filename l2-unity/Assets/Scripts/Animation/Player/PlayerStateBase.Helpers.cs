using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public partial class PlayerStateBase : StateMachineBehaviour
{
    public void SetBool(string name, bool isWeaponAnim, bool value)
    {
        //Debug.Log("Set bool2: " + name);
        SetBool(name, isWeaponAnim, value, true);
    }

    public void SetBool(string name, bool isWeaponAnim, bool value, bool share)
    {
        if (isWeaponAnim)
        {
            name += "_" + _gear.WeaponAnim;
        }

        //Debug.Log("player Set bool: " + name);

        AnimationPacketServices.PlayerAnimation?.SetBool(name, value, share);
    }

    public bool GetBool(string name)
    {
        PlayerAnimationController playerAnimation = AnimationPacketServices.PlayerAnimation;
        return playerAnimation != null && playerAnimation.GetBool(name);
    }
}
