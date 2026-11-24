using UnityEngine;

/// <summary>
/// One Unreal-actor placement used by every world tile and by the login vista.
/// Position is <see cref="VectorUtils.ConvertPosToUnity"/> of Location, minus the
/// same conversion of PrePivot scaled by DrawScale×DrawScale3D and rotated by the
/// actor rotator. Rotation is <see cref="VectorUtils.ConvertRotToUnity"/>
/// (pitch, yaw, roll in the 65536 = 360° circle) times the mesh-local yaw fix
/// times the prefab's own rotation. Scale is DrawScale (0 means the property was
/// absent; Engine.u/Actor.uc defaultproperties DrawScale=1) times DrawScale3D
/// (absent means 1,1,1). Each UE axis is applied to the single prefab-local axis
/// the mesh yaw fix sends it to. A basis that is not a permutation logs an error
/// and does not guess a dominant axis.
/// </summary>
public static class L2Ue2ActorXform
{
    public struct Pose
    {
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 localScale;
    }

    public static Pose Evaluate(
        Vector3 locationUu,
        Vector3 rotationPitchYawRollUu,
        Vector3 prePivotUu,
        float drawScale,
        Vector3 drawScale3D,
        bool hasDrawScale3D,
        float meshLocalYawFixDegrees,
        Quaternion prefabLocalRotation,
        Vector3 prefabLocalScale,
        string actorName)
    {
        float ds = drawScale != 0f ? drawScale : 1f;
        Vector3 s3 = hasDrawScale3D ? drawScale3D : Vector3.one;
        Vector3 ueScale = new Vector3(s3.x * ds, s3.y * ds, s3.z * ds);

        Quaternion ueRot = Quaternion.Euler(VectorUtils.ConvertRotToUnity(rotationPitchYawRollUu));
        Vector3 pivotUu = Vector3.Scale(prePivotUu, ueScale);
        Vector3 position = VectorUtils.ConvertPosToUnity(locationUu)
            - ueRot * VectorUtils.ConvertPosToUnity(pivotUu);

        Quaternion meshLocal = Quaternion.Euler(0f, meshLocalYawFixDegrees, 0f) * prefabLocalRotation;
        Quaternion rotation = ueRot * meshLocal;
        Vector3 localScale = Vector3.Scale(MapScale(ueScale, meshLocal, actorName), prefabLocalScale);
        return new Pose { position = position, rotation = rotation, localScale = localScale };
    }

    /// <summary>
    /// Unreal horizontal FOV (Camera.FovAngle, degrees) to Unity's vertical
    /// <c>Camera.fieldOfView</c> at <paramref name="aspect"/> (width/height).
    /// </summary>
    public static float VerticalFovDegrees(float horizontalDegrees, float aspect)
    {
        if (aspect <= 0f)
        {
            Debug.LogError("[L2Ue2ActorXform] FOV aspect " + aspect + " is not positive; using 1");
            aspect = 1f;
        }

        float half = horizontalDegrees * Mathf.Deg2Rad * 0.5f;
        return Mathf.Atan(Mathf.Tan(half) / aspect) * 2f * Mathf.Rad2Deg;
    }

    static Vector3 MapScale(Vector3 ueScale, Quaternion meshLocal, string actorName)
    {
        Quaternion inv = Quaternion.Inverse(meshLocal);
        Vector3 s = Vector3.one;
        s = Accumulate(s, inv * Vector3.forward, ueScale.x, actorName, "X");
        s = Accumulate(s, inv * Vector3.right, ueScale.y, actorName, "Y");
        s = Accumulate(s, inv * Vector3.up, ueScale.z, actorName, "Z");
        return s;
    }

    static Vector3 Accumulate(Vector3 s, Vector3 localAxis, float v, string actorName, string axis)
    {
        float ax = Mathf.Abs(localAxis.x);
        float ay = Mathf.Abs(localAxis.y);
        float az = Mathf.Abs(localAxis.z);
        float max = ax;
        if (ay > max) max = ay;
        if (az > max) max = az;
        if (max < 0.999f)
        {
            Debug.LogError("[L2Ue2ActorXform] " + actorName + " DrawScale " + axis
                + " does not land on a single local axis (basis " + localAxis.ToString("F3")
                + "); that axis is left unscaled");
            return s;
        }

        if (ax >= 0.999f) s.x *= v;
        else if (ay >= 0.999f) s.y *= v;
        else s.z *= v;
        return s;
    }
}

/// <summary>
/// TerrainInfo height and corner. Height is
/// Location.Z + (sample - 32768) * (TerrainScale.Z / 256). TerrainInfo.DrawScale
/// is not a height multiplier: applying the stored 10 moves the sheet off the
/// static-mesh bases. The Height.bmp is bottom-up and UE V = 0 is the +Y edge,
/// so the file row is (height - 1 - v). The corner is the sheet center minus
/// half the sector extent; there is no extra one-sector seam shift.
/// </summary>
public static class L2Ue2Terrain
{
    public static float HeightUu(int sample, float locationZ, float terrainScaleZ)
    {
        return locationZ + (sample - 32768f) * (terrainScaleZ / 256f);
    }

    public static Vector3 UnityCorner(Vector3 locationUu, Vector3 terrainScaleUu, int sectors)
    {
        float tx = sectors * terrainScaleUu.y;
        float ty = sectors * terrainScaleUu.z;
        float tz = sectors * terrainScaleUu.x;
        return new Vector3(
            locationUu.y - tx * 0.5f,
            locationUu.z - ty * 0.5f,
            locationUu.x - tz * 0.5f) * VectorUtils.L2UuToUnity;
    }

    public static bool TryHeightAt(
        ushort[] fileSamples,
        int width,
        int height,
        Vector3 locationUu,
        Vector3 terrainScaleUu,
        float xUu,
        float yUu,
        out float heightUu,
        out int iu,
        out int iv,
        out int sample)
    {
        heightUu = 0f;
        iu = 0;
        iv = 0;
        sample = 0;
        if (fileSamples == null || width <= 0 || height <= 0 || fileSamples.Length < width * height)
        {
            Debug.LogError("[L2Ue2Terrain] height samples missing (width=" + width + " height=" + height + ")");
            return false;
        }

        float ox = locationUu.x - width * terrainScaleUu.x * 0.5f;
        float oy = locationUu.y - height * terrainScaleUu.y * 0.5f;
        iu = Mathf.FloorToInt((xUu - ox) / terrainScaleUu.x);
        iv = Mathf.FloorToInt((yUu - oy) / terrainScaleUu.y);
        if (iu < 0 || iv < 0 || iu >= width || iv >= height)
        {
            Debug.LogError("[L2Ue2Terrain] point (" + xUu + "," + yUu + ") is outside the sheet texel ("
                + iu + "," + iv + ")");
            return false;
        }

        int fileRow = height - 1 - iv;
        sample = fileSamples[fileRow * width + iu];
        heightUu = HeightUu(sample, locationUu.z, terrainScaleUu.z);
        return true;
    }
}
