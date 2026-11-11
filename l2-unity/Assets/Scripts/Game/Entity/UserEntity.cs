using VContainer;
using UnityEngine;

public partial class UserEntity : NetworkEntity
{
    private CharacterAnimationAudioHandler _characterAnimationAudioHandler;
    private CharacterController _characterController;
    private float _attackVisualUntil;
    private UserBowArrowEvents _bowArrowEvents;

    public string Nick
    {
        get
        {
            if (Identity != null && !string.IsNullOrEmpty(Identity.Name))
                return Identity.Name;
            return name;
        }
    }

    public string LogTag
    {
        get { return "[EntityAction:User] nick=" + Nick; }
    }

    public string WeaponAnim
    {
        get
        {
            if (_gear != null && !string.IsNullOrEmpty(_gear.WeaponAnim))
                return _gear.WeaponAnim;
            return "hand";
        }
    }
}
