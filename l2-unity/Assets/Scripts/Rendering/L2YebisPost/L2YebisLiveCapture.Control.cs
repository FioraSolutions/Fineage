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

    public static bool Consume()
    {
        if (_request == 0)
        {
            return false;
        }

        _request = 0;
        return true;
    }

    public static string CreateFolder(float worldHours, string cameraFormat, string colorSpace)
    {
        string folder = Path.Combine(Root, DateTime.Now.ToString("yyyyMMdd_HHmmss"));
        Directory.CreateDirectory(folder);
        var state = new StringBuilder();
        state.AppendLine("role=unity-live");
        state.AppendLine("worldHours=" + worldHours.ToString("0.###", CultureInfo.InvariantCulture));
        state.AppendLine("colorSpace=" + colorSpace);
        state.AppendLine("cameraFormat=" + cameraFormat);
        state.AppendLine("bloom=0");
        state.AppendLine("captureFormula=1");
        state.AppendLine("glare=black");
        state.AppendLine("gradeStage=" + _gradeStage + " " + GradeStageName(_gradeStage));
        state.AppendLine("where=camera-color-before-combine");
        File.WriteAllText(Path.Combine(folder, "state.txt"), state.ToString(), Encoding.UTF8);
        File.WriteAllText(Path.Combine(Root, "latest.txt"), folder, Encoding.UTF8);
        // Debug.Log("[Yebis] live capture " + folder);
        return folder;
    }

    public static void ReadAfterFrame(string folder, RenderTexture scene, RenderTexture graded, RenderTexture control)
    {
        if (L2YebisLiveCaptureKey.Host == null)
        {
            // Debug.LogWarning("[Yebis] нет объекта клавиши, кадр не прочитан");
            return;
        }

        L2YebisLiveCaptureKey.Host.StartCoroutine(ReadWhenPresented(folder, scene, graded, control));
    }

    static IEnumerator ReadWhenPresented(string folder, RenderTexture scene, RenderTexture graded, RenderTexture control)
    {
        yield return new WaitForEndOfFrame();
        SaveControl(folder, control);
        SavePresented(folder, scene, "input", "unity-s0-before-combine", "Вход combine, 8-битный, как текстура камеры");
        SavePresented(folder, graded, "output", "unity-after-combine", "Выход combine, 8-битный");
        WriteProbe(folder, scene, graded);
        WriteTerrainColor0(folder, scene);
        // Debug.Log("[Yebis] live capture written " + folder);
    }
}
