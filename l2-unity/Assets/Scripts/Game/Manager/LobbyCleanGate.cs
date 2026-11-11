#if UNITY_EDITOR || DEVELOPMENT_BUILD
using UnityEngine;
using UnityEngine.SceneManagement;

public static partial class LobbyCleanGate
{
    public struct Report
    {
        public bool Ok;
        public bool MenuLoaded;
        public bool LobbyLoaded;
        public int FaceLeftovers;
        public int CharSelectLayerObjs;
        public string Detail;
    }




}
#endif
