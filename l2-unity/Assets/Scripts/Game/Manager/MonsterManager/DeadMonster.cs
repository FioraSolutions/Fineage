using UnityEngine;

public partial class DeadMonster : MonoBehaviour
{
    //fly setting
    private bool _useAntiGravity = false;
    public float speed = 0.000001f; // Скорость движения вверх
    private float zeroPos = 0;
    private bool _isRefresh = false;


    //opacity setting
    private Renderer[] _monsterRenderer;
    private Color originalColor;
    private float currentPos = 0;

    //remove setting
    private World _world;
    private int _monster_id;




  



}
