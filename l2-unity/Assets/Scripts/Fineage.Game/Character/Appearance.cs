using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class Appearance {
    public float CollisionHeight;
    public float CollisionRadius;
    public int LHand;
    public int RHand;
    
    public float PhisicalAttackRange { get { return CollisionRadius; } }
    
}
