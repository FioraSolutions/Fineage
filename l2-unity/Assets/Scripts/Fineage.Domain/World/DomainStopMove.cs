using UnityEngine;

/// <summary>
/// Wire-agnostic stop command: object id, stop position, and heading.
/// </summary>
public sealed class DomainStopMove
{
    public int ObjectId;
    public Vector3 StopPos;
    public int Heading;
}
