using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;

public partial class SceneLoader : MonoBehaviour, ISceneLoader
{
    [SerializeField] private string _menuScene = "Menu";
    [SerializeField] private string _lobbyScene = "l2_lobby";
    [SerializeField] private string _gameScene = "Game";
    [SerializeField] private float _tilePreloadDistance = 80f;

    private readonly HashSet<string> _loadedTiles = new HashSet<string>();
    private readonly Dictionary<string, GameObject> _prefabTiles = new Dictionary<string, GameObject>();
    private bool _worldStreaming;
    private bool _streamingBusy;

    GameManager _gameManager;

    private GameManager Manager
    {
        get
        {
            if (_gameManager != null)
                return _gameManager;
            return GamePacketServices.Manager ?? WorldPacketServices.Manager;
        }
    }

    private World GameWorld
    {
        get
        {
            if (App.GameContainer != null)
            {
                try
                {
                    return App.GameContainer.Resolve<World>();
                }
                catch
                {
                }
            }
            return WorldApplyPacketServices.GameWorld ?? GamePacketServices.GameWorld;
        }
    }

    public string GameScene { get { return _gameScene; } }

    public static SceneLoader _instance;
    [Obsolete("Use [Inject] / PacketServices instead")]
    public static SceneLoader Instance { get { return _instance; } }


}
