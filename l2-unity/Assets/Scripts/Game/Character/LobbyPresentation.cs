using UnityEngine;
using UnityEngine.SceneManagement;

public static partial class LobbyPresentation
{
    public const string LayerName = "CharSelect";
    public const string ClickLayerName = "EntityClick";
    public const string RootName = "LobbyPresentationRoot";
    public const string ClickAreaName = "click_area";
    const string MenuSceneName = "Menu";

    static int _layer = int.MinValue;
    static int _clickLayer = int.MinValue;
    static Transform _root;

    public static int Layer
    {
        get
        {
            if (_layer == int.MinValue)
                _layer = LayerMask.NameToLayer(LayerName);
            return _layer;
        }
    }

    static int ClickLayer
    {
        get
        {
            if (_clickLayer == int.MinValue)
                _clickLayer = LayerMask.NameToLayer(ClickLayerName);
            return _clickLayer;
        }
    }



    /// <summary>Drop cached root after Menu unload (Unity fake-nulls the Transform).</summary>





    /// <summary>
    /// Paint CharSelect on the pawn tree, but keep <c>click_area</c> on EntityClick
    /// (CharacterSelector raycasts only that layer) and ensure its collider is on.
    /// </summary>
}
