using System;
using UnityEngine;
using UnityEngine.Rendering;

public partial class L2CloudSimple : MonoBehaviour
{
    static void ApplyTint(Material[] mats, int i, Color tint, float fade)
    {
        if (mats == null || i < 0 || i >= mats.Length || mats[i] == null)
            return;
        mats[i].SetVector(TextureFactorId, new Vector4(tint.r * fade, tint.g * fade, tint.b * fade, tint.a));
    }

    void Place(ref CloudChunk chunk, Vector3 pos, float skyZ, float meshRadius, float liftY)
    {
        if (chunk.tf == null)
            return;
        ApplyCloudLayer(chunk.tf.gameObject);
        chunk.tf.position = pos;
        chunk.tf.rotation = Quaternion.identity;
        chunk.tf.localScale = Vector3.one;
        if (chunk.mats == null)
            return;
        for (int i = 0; i < chunk.mats.Length; i++)
        {
            if (chunk.mats[i] != null)
                chunk.mats[i].SetVector(SkyParamsId, new Vector4(skyZ, meshRadius, liftY, 0f));
        }
    }
}
