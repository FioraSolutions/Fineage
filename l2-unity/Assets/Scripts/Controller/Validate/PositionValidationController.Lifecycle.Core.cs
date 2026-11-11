using System;

using System.Collections.Generic;
using UnityEngine;
using VContainer;

public partial class PositionValidationController : MonoBehaviour
{
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            _validateList = new List<ValidateLocationDto>();
            _validateRemove = new List<ValidateLocationDto>();
            _validateInitPosition = new List<CharMoveToLocationDto>();
            WireControllerPacketServices();
        }
        else
        {
            Destroy(this);
        }
    }
}
