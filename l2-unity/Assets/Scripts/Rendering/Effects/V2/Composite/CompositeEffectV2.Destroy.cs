using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public partial class CompositeEffectV2 : CompositePrefabEffect
{





    sealed class PendingV2Part
    {
        public CompositePart Part;
        public float SpawnAtTime;
    }
}
