using UnityEngine;

/// <summary>
/// Wire-agnostic move command: object id, destination, and origin.
/// The Interlude packet is mapped onto these fields and rebuilt at apply time.
/// </summary>
public sealed class DomainMoveTo
{
    public int ObjectId;
    public Vector3 Destination;
    public Vector3 Origin;
}
