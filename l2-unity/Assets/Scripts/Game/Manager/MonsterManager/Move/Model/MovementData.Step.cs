using UnityEngine;

public partial class MovementData
{


    /// </summary>
    /// bounce between two different ground conventions.
    /// GroundSnapHelper / spawn / NetworkTransformReceive so the pawn does not
    /// floor. Y is written as the surface height exactly (hit.point.y), matching
    /// probe). Applies gravity to _verticalVelocity, then rests the pawn on the
    /// ULevel::ToFloor / APawn::processLanded (0x086 / 0x297 downward capsule
    /// Port of Interlude APawn::physWalking (planar velocity + gravity) and
    /// <summary>



}
