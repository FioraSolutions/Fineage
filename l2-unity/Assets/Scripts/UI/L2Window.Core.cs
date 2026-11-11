using NUnit.Framework.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public abstract partial class L2Window : MonoBehaviour
{
    void Start()
    {
        _isWindowHidden = false;
        EnsureAssetsLoaded();
    }
}
