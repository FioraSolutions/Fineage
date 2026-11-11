using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class CompositeEffectV2 : CompositePrefabEffect
{





    sealed class PendingV2Part
    {
        public CompositePart Part;
        public float SpawnAtTime;
    }
}
