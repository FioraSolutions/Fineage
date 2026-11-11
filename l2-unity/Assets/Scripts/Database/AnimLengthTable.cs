using System;
using System.Collections.Generic;
using VContainer;
using UnityEngine;
using UnityEngine.InputSystem;

public partial class AnimLeghtTable
{
    private static AnimLeghtTable _instance;

    [Obsolete("Use [Inject] / PacketServices instead")]
    public static AnimLeghtTable Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new AnimLeghtTable();
            }

            return _instance;
        }
    }

    private Dictionary<string, float> _leght;




   

}
