public class Animation
{
    public string Value { get; set; }
    public TypesAnimation Type { get; set; }
    public MagicPhase Phase { get; set; }

    /// <summary>
    /// Fixed CrossFade duration when leaving this Animator state (e.g. into wait_/atkwait_).
    /// null = <see cref="LocomotionCrossFadeDefaults.FixedDuration"/>.
    /// </summary>
    public float? ExitCrossFadeDuration { get; }

    public Animation(
        string value,
        TypesAnimation type,
        MagicPhase phase = MagicPhase.None,
        float? exitCrossFadeDuration = null)
    {
        Value = value;
        Type = type;
        Phase = phase;
        ExitCrossFadeDuration = exitCrossFadeDuration;
    }

    public string Concat(string name)
    {
        return Value + name;
    }

    public void MergeStrings(string name)
    {
        Value = Value + name;
    }

    public override string ToString()
    {
        return Value;
    }

    public bool AreAnimationsEqual(Animation first, Animation second)
    {
        if (ReferenceEquals(first, second))
            return true;

        if (first is null || second is null)
            return false;

        return first.Value == second.Value;
    }

}
