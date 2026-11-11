using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

public partial class PositionValidationController
{




    //This function is a test function.It solves the problem with position synchronization when the server sends data that the client needs to move.
    //But the client did not load at this moment.Therefore, we collect these packages and after loading, we find the oldest one in time and move the NPC immediately to the end of this movement vector.
    //Ideally, you need to calculate the path traveled and move the npc there, but for the sake of 2 npc it makes no sense

}
