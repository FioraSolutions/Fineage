using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;

public partial class SceneLoader : MonoBehaviour
{
    public void LoadSWMap()
    {
        IZoneTransport transport = ZoneCommands.Transport;
        if (transport == null || transport.PlayerInfo.Identity == null)
        {
            Debug.LogError("LoadSWMap: no player xyz (PlayerInfo.Identity null) — loading UI will stick");
            return;
        }

        Vector3 l2Pos = transport.PlayerInfo.Identity.GetL2jPos();
        StartCoroutine(LoadSWMapRoutine(l2Pos));
    }

    private IEnumerator LoadSWMapRoutine(Vector3 l2Pos)
    {
        // Textbook: drop lobby island first. No Menu/l2_lobby overlap with world tile load.
        yield return UnloadLobbyScenesRoutine();

        yield return LoadSceneAdditiveIfNeeded(_gameScene);
        SetSceneRootsActive(_gameScene, true, true);
        EnsureGameplayCamera();

        Scene game = SceneManager.GetSceneByName(_gameScene);
        if (game.IsValid() && game.isLoaded)
        {
            SceneManager.SetActiveScene(game);
        }

        Manager.OnWorldLoading();
        SetGameHudVisible(true);

        string tile = VectorUtils.GetSwMapName(l2Pos);
        FineageLog.Info(FineageLogCategory.World, "LoadSWMap tile='" + tile + "' xyz=(" + l2Pos.x + ", " + l2Pos.y + ", " + l2Pos.z + ")");

        Geodata geodata = WorldPacketServices.Geodata;
        if (geodata != null)
        {
            geodata.LoadMaps(new List<string> { tile });
        }

        yield return LoadSceneAdditiveIfNeeded(tile);
        _loadedTiles.Add(tile);

        FinishLoadSWMap();
        EnsureGameplayCamera();
        WorldClock.EnsurePersistent();
        _worldStreaming = true;
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        StartCoroutine(WorldEnterDiagDelayed());
#endif
    }
}
