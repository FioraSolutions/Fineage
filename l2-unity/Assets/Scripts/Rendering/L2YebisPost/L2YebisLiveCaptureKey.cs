using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Text;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;


/// <summary>
/// G writes actor light constants. F8 opens the skill bar.
/// The frame capture and the F7 grade stage stay off.
/// The Game view must have focus.
/// </summary>
sealed class L2YebisLiveCaptureKey : MonoBehaviour
{
    public static L2YebisLiveCaptureKey Host { get; private set; }

    void Awake()
    {
        Host = this;
    }

    void OnDestroy()
    {
        if (Host == this)
        {
            Host = null;
        }
    }

    void LateUpdate()
    {
        if (!PollInput.KeyDown(UnityEngine.InputSystem.Key.G))
        {
            return;
        }

        L2ActorLightCapture.Write();
    }
}

