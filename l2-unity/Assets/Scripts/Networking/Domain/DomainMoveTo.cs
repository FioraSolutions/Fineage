using UnityEngine;

/// <summary>
/// Wire-agnostic move command. Interlude still attaches <see cref="InterludeDto"/>
/// until Intention/PlayerController accept domain payloads directly.
/// </summary>
public sealed class DomainMoveTo
{
    public int ObjectId;
    public Vector3 Destination;
    public Vector3 Origin;

    /// <summary>Interlude adapter bridge; null for future v2 wire.</summary>
    public CharMoveToLocationDto InterludeDto;
}
