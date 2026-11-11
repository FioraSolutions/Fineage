using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;

public partial class SceneLoader : MonoBehaviour
{
    private IEnumerator WorldEnterDiagDelayed()
    {
        yield return null;
        yield return null;
        SafeWorldSnapshot("LoadSWMap +2 frames");
        yield return new WaitForSecondsRealtime(1f);
        SafeWorldSnapshot("LoadSWMap +1s");
        try
        {
            LobbyCleanGate.Report report = LobbyCleanGate.Evaluate();
            FineageLog.Info(
                FineageLogCategory.World,
                (report.Ok ? "LobbyClean PASS " : "LobbyClean FAIL ") + report.Detail);
        }
        catch (System.Exception ex)
        {
            FineageLog.Exception(FineageLogCategory.World, "LobbyCleanGate", ex);
        }
    }

    static void SafeWorldSnapshot(string phase)
    {
        try
        {
            WorldEnterDiag.Snapshot(phase);
        }
        catch (System.Exception ex)
        {
            FineageLog.Exception(FineageLogCategory.World, "WorldEnterDiag." + phase, ex);
        }
    }

    private void FinishLoadSWMap()
    {
        if (GameWorld != null)
        {
            Manager.OnWorldSceneLoaded();
        }
    }
}
