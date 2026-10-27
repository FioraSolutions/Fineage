using UnityEngine;

public sealed class DomainStopMove
{
    public int ObjectId;
    public Vector3 StopPos;
    public int Heading;

    /// <summary>Interlude adapter bridge; null for future v2 wire.</summary>
    public StopMoveDto InterludeDto;
}
