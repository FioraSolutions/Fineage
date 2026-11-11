using UnityEngine;

public sealed partial class EntityActionStop : IEntityActionProcess
{
    static readonly int BlockMask = LayerMask.GetMask(
        "Default", "Terrain", "StaticMesh", "Brush", "AllowWalk", "Deco", "Obstacle");

    public L2PawnRange Range { get; }






}
