using System;
using System.Collections.Generic;
using UnityEngine;

public partial class PathFinderController : MonoBehaviour
{
    void OnDestroy()
    {
        _instance = null;
    }
}
