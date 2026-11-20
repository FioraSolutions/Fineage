/// <summary>Resolved playback descriptor for one sound id (EditMode-testable).</summary>
public readonly struct AudioPlaySpec
{
    public AudioPlaySpec(AudioCategory category, AudioSourceKind source, string id)
    {
        Category = category;
        Source = source;
        Id = id;
    }

    public AudioCategory Category { get; }
    public AudioSourceKind Source { get; }
    public string Id { get; }
}