using System.Collections.Generic;
using UnityEngine;
using System;
using VContainer;

public partial class GravityNpc : MonoBehaviour, IGravity
{

    private Dictionary<int, GravityData> _dict;
    public List<int> _remove;


    private Vector3 _direction;
    private float _speed;
    private float _gravity = 28f;
    private float _moveSpeedMultiplier = 1f;


    private static IGravity _instance;
    [System.Obsolete("Use [Inject] or GravityNpcPacketServices.Gravity instead")]
    public static IGravity Instance { get { return (_instance as UnityEngine.Object) != null ? _instance : null; } }












    //public void Sync()
    //{
    //    _isSync = true;
    //}


}
