using System;
using Object = UnityEngine.Object;
using UnityEngine;
using UnityEngine.Rendering;
using System.Globalization;

public partial class L2CelestialDiscs
{
    public void DrawMoonBloomInclude(CommandBuffer cmd)
    {
        if (cmd == null || _moonMr == null || !_moonMr.enabled || _moonMat == null)
            return;

        int pass = _moonMat.FindPass("BloomInclude");
        if (pass < 0)
            return;

        cmd.DrawRenderer(_moonMr, _moonMat, 0, pass);
    }

    void LateUpdateApplyLayer()
    {
        if (_sunTf != null)
            ApplyCelestialLayer(_sunTf.gameObject);
        if (_moonTf != null)
            ApplyCelestialLayer(_moonTf.gameObject);
    }

    Camera ResolveCamera()
    {
        if (CameraPacketServices.Controller != null)
        {
            Camera fromController = CameraPacketServices.Controller.GetComponent<Camera>();
            if (fromController == null)
            {
                fromController = CameraPacketServices.Controller.GetComponentInChildren<Camera>();
            }

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

    static void DestroyDisc(ref Transform tf, ref Material mat)
    {
        if (mat != null)
        {
            if (Application.isPlaying)
            {
                Destroy(mat);
            }
            else
            {
                DestroyImmediate(mat);
            }

            mat = null;
        }

        if (tf != null)
        {
            if (Application.isPlaying)
            {
                Destroy(tf.gameObject);
            }
            else
            {
                DestroyImmediate(tf.gameObject);
            }

            tf = null;
        }
    }
}
