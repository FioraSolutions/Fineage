using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class SkinnedMeshSync : MonoBehaviour
{
    [SerializeField] private SkinnedMeshRenderer _rootSkinnedRenderer;
    [SerializeField] private SkinnedMeshRenderer[] _destSkinnedRenderer;

    readonly Queue<GameObject> _waitingObjects = new();
    readonly List<Renderer> _temporarilyHiddenRenderers = new();
    bool _isProcessing;






    /// <summary>
    /// Blender PSK exports add <c>_end</c> leaf bones that the anim FBX omits.
    /// </summary>

}
