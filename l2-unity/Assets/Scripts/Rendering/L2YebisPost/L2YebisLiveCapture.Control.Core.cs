using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Text;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;

public static partial class L2YebisLiveCapture
{
    public static void CycleGradeStage()
    {
        _gradeStage = _gradeStage >= 4 ? 0 : _gradeStage + 1;
        // Debug.Log("[Yebis] F7 стадия " + _gradeStage + " " + GradeStageName(_gradeStage));
    }

    public static string GradeStageName(int stage)
    {
        switch (stage)
        {
            case 1: return "A выборка s0";
            case 2: return "B матрица, в RT значения выше 1 станут 255";
            case 3: return "C перед степенью 0.625";
            case 4: return "D после степени 0.625";
            default: return "полный выход, то же что D";
        }
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void InstallKey()
    {
        var go = new GameObject("L2YebisLiveCaptureKey")
        {
            hideFlags = HideFlags.HideAndDontSave
        };
        UnityEngine.Object.DontDestroyOnLoad(go);
        go.AddComponent<L2YebisLiveCaptureKey>();
    }

    public static void Request()
    {
        _request = 1;
    }
}
