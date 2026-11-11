using System;
using UnityEngine;

/// <summary>
/// Lobby/world scene orchestration contract for App scope (#131).
/// Concrete <c>SceneLoader</c> stays ACS (streaming + UI packet bridges).
/// </summary>
public interface ISceneLoader
{
    string GameScene { get; }
    void LoadMenu();
    void LoadSWMap();
    void SwitchScene(string sceneName, Action<AsyncOperation> onComplete);
}
