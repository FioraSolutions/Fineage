using System;

using UnityEngine;

public partial class PlayerController
{

   







  

 



 




  





    
 



    
    /// <summary>
    /// Vertical velocity integrator with Interlude physWalking / physFalling semantics.
    /// Grounded: planar velocity — Physics isGrounded holds the pawn on the floor, a
    /// small downward bias keeps the CharacterController touching it over slopes.
    /// Airborne: accumulate gravity, capped at terminal velocity.
    /// Returns the Y velocity (m/s); the caller integrates it with dt exactly once.
    /// </summary>

    /// <summary>
    /// Frame-rate independent lerp factor preserving the legacy per-frame feel
    /// (0.06 / 0.2 fractions) at the game's 60 fps target. Without this,
    /// Quaternion.Lerp(t) sped up or slowed down with the frame rate.
    /// </summary>

   





}
