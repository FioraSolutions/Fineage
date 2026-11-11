public sealed partial class UserBowArrowEvents
{
    public UserBowArrowEvents(UserEntity user, BowArrowVisual bowArrow, IAnimationManager animations)
    {
        _user = user;
        _bowArrow = bowArrow;
        _animations = animations;
        _objectId = user != null && user.Identity != null ? user.Identity.Id : 0;
        ResolveEvents();
    }

    public void Enter()
    {
        if (_isSubscribed)
            return;

        ResolveEvents();
        if (_events == null)
            return;

        _isSubscribed = true;
        _events.OnAnimationStartLoadArrow += OnLoadArrow;
        _events.OnAnimationStartShoot += OnShoot;
    }

    public void Exit()
    {
        if (!_isSubscribed)
            return;

        _isSubscribed = false;
        if (_events == null)
            return;

        _events.OnAnimationStartLoadArrow -= OnLoadArrow;
        _events.OnAnimationStartShoot -= OnShoot;
        _events = null;
    }
}
