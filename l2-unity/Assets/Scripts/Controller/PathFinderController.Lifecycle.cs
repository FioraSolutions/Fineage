using System;
using System.Collections.Generic;
using UnityEngine;

public partial class PathFinderController : MonoBehaviour
{
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(this);
        }
    }
}
