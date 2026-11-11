using System;
using UnityEngine;
using UnityEngine.Rendering;

public partial class L2CloudSimple : MonoBehaviour
{
    public void DrawBloomExclude(CommandBuffer cmd)
    {
        DrawChunkExclude(cmd, ref _domeA);
        DrawChunkExclude(cmd, ref _domeB);
    }

    static void DrawChunkExclude(CommandBuffer cmd, ref CloudChunk chunk)
    {
        if (cmd == null || chunk.mr == null || !chunk.mr.enabled || chunk.mats == null)
            return;

        for (int i = 0; i < chunk.mats.Length; i++)
        {
            Material mat = chunk.mats[i];
            if (mat == null)
                continue;
            int pass = mat.FindPass("BloomExclude");
            if (pass < 0)
                continue;
            cmd.DrawRenderer(chunk.mr, mat, i, pass);
        }
    }

    float ResolveSkyDistance(Camera cam)
    {
        float far = cam != null ? cam.farClipPlane : 500f;
        return Mathf.Clamp(far * 0.88f, 80f, 4000f);
    }

    Camera ResolveCamera()
    {
        if (CameraPacketServices.Controller != null)
        {
            Camera fromController = CameraPacketServices.Controller.GetComponent<Camera>();
            if (fromController == null)
                fromController = CameraPacketServices.Controller.GetComponentInChildren<Camera>();
            if (fromController != null && fromController.enabled && fromController.gameObject.activeInHierarchy)
            {
                _cam = fromController;
                return _cam;
            }
        }

        Camera main = Camera.main;
        if (main != null && main.enabled && main.gameObject.activeInHierarchy)
        {
            _cam = main;
            return _cam;
        }

        Camera[] cams = Camera.allCameras;
        for (int i = 0; i < cams.Length; i++)
        {
            Camera c = cams[i];
            if (c != null && c.enabled && c.gameObject.activeInHierarchy && !c.orthographic)
            {
                _cam = c;
                return _cam;
            }
        }

        return _cam;
    }

    void DestroyChunks()
    {
        DestroyChunk(ref _domeA);
        DestroyChunk(ref _domeB);
    }

    static void DestroyChunk(ref CloudChunk chunk)
    {
        if (chunk.mats != null)
        {
            for (int i = 0; i < chunk.mats.Length; i++)
                DestroyObj(chunk.mats[i]);
            chunk.mats = null;
        }

        DestroyObj(chunk.mesh);
        chunk.mesh = null;
        if (chunk.tf != null)
            DestroyObj(chunk.tf.gameObject);
        chunk.tf = null;
        chunk.mf = null;
        chunk.mr = null;
    }

    static void DestroyObj(UnityEngine.Object obj)
    {
        if (obj == null)
            return;
        if (Application.isPlaying)
            Destroy(obj);
        else
            DestroyImmediate(obj);
    }
}
