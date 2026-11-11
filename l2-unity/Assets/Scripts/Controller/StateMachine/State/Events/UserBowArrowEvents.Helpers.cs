public sealed partial class UserBowArrowEvents
{
    void ResolveEvents()
    {
        if (_events != null || _objectId <= 0)
            return;
        if (_animations == null)
            return;
        _events = _animations.GetAnimationEvents(_objectId);
    }

    void OnLoadArrow(string animName)
    {
        if (_bowArrow != null)
            _bowArrow.TryLoadArrow(_user, animName);
    }

    void OnShoot(string animName)
    {
        if (_bowArrow != null)
            _bowArrow.TryShoot(_user, animName);
    }
}
