using UnityEngine;
using System;
using System.Collections.Generic;
using VContainer;

public partial class HitManager
{

    /// <summary>
    /// L2 AssociateAttackedNotify: spawn at victim Location, then pull back along
    /// attacker→victim by ~CollisionRadius (not sword hit point, not bone attach).
    /// Prefer this over composite positionOffset — offset must scale per target.
    /// </summary>
    // Slightly less than full CollisionRadius — full radius sat a bit too close to the attacker.
    private const float SoulshotSpawnRadiusScale = 0.3f;








}
