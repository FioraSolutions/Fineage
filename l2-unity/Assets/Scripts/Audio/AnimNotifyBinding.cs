using System.Collections.Generic;

/// <summary>One official notify firing on a MeshAnimation sequence (normalized time 0..1).</summary>
public sealed class AnimNotifyBinding
{
    public readonly string Package;
    public readonly string Animation;
    public readonly string Sequence;
    public readonly float Time;
    public readonly string ClassName;
    public readonly string NotifyName;
    public readonly string Function;
    public readonly float Volume;
    public readonly float Radius;
    public readonly string Sound;
    public readonly string SoundRef;
    public readonly bool SurfaceStep;
    public readonly IReadOnlyDictionary<string, string[]> SurfaceArrays;
    public readonly int FrameCount;
    public readonly float Rate;

    public AnimNotifyBinding(
        string package, string animation, string sequence, float time,
        string className, string notifyName, string function,
        float volume, float radius, string sound, string soundRef,
        bool surfaceStep, IReadOnlyDictionary<string, string[]> surfaceArrays,
        int frameCount, float rate)
    {
        Package = package ?? string.Empty;
        Animation = animation ?? string.Empty;
        Sequence = sequence ?? string.Empty;
        Time = time;
        ClassName = className ?? string.Empty;
        NotifyName = notifyName ?? string.Empty;
        Function = function ?? string.Empty;
        Volume = volume;
        Radius = radius;
        Sound = sound ?? string.Empty;
        SoundRef = soundRef ?? string.Empty;
        SurfaceStep = surfaceStep;
        SurfaceArrays = surfaceArrays;
        FrameCount = frameCount;
        Rate = rate;
    }

    public string Id =>
        Package + "." + Animation + "." + Sequence + "." + NotifyName + "@" + Time.ToString("G9", System.Globalization.CultureInfo.InvariantCulture);
}
