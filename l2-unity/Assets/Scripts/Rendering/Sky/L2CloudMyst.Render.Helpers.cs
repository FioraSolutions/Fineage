using System;
using UnityEngine;
using UnityEngine.Rendering;

public partial class L2CloudMyst
{
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

    void DestroyMesh()
    {
        DestroyObj(_mat);
        _mat = null;
        DestroyObj(_mesh);
        _mesh = null;
        if (_tf != null)
            DestroyObj(_tf.gameObject);
        _tf = null;
        _mf = null;
        _mr = null;
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
