using UnityEngine;

public abstract partial class CompositePart : ISerializationCallbackReceiver
{
    public virtual void ConfigurePlayback(
        BaseEffect instance,
        EffectSettings settings,
        MagicCastData castData,
        EffectResolveContext context)
    {
    }

    public virtual string Describe()
    {
        string place = spawnAttachmentPoint.ToString();
        return GetType().Name + " " + place + " / " + spawnTiming +
               " follow=" + follow + " scale=" + scale.ToString("0.###");
    }

    public virtual void OnAfterSpawn(BaseEffect instance, EffectResolveContext context)
    {
    }
}
