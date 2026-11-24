#if (UNITY_EDITOR) 
using UnityEngine;

public class L2StaticMesh {
    public string actorName { get; set; }
    public string staticMesh { get; set; }
    // A StaticMesh= line that is neither StaticMesh'...' nor None.
    public string unparsedStaticMesh { get; set; }
    public string actorClass { get; set; }
    public float y { get; set; }
    public float x { get; set; }
    public float z { get; set; }
    //public int yaw { get; set; }
    //public int roll { get; set; }
    //public int pitch { get; set; }
    public Vector3 eulerAngles;
    public float scale { get; set; }
    public float scaleX { get; set; }
    public float scaleY { get; set; }
    public float scaleZ { get; set; }
    // UE DrawScale3D (actor-local X,Y,Z); may be negative = mirrored
    public Vector3 drawScale3D = Vector3.one;
    public bool hasDrawScale3D;
    public Vector3 prePivot;
}
#endif
