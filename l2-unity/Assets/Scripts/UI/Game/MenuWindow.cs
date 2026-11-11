using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

public partial class MenuWindow : L2Window
{
    private static MenuWindow _instance;
    [Obsolete("Use [Inject] / PacketServices instead")]
    public static MenuWindow Instance { get { return _instance; } }







    /// <summary>
    /// USS right/bottom alone was still inset in play mode — pin like Skillbar.
    /// </summary>


    /// <summary>
    /// UITK resource() often misses Interlude sprites — stamp Texture2D after build.
    /// </summary>



}
