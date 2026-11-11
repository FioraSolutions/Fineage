using UnityEngine;

public partial class MonsterEntity : NetworkEntity
{
    private MonsterAnimationAudioHandler _monsterAnimationAudioHandler;
    private LayerMask _areaMask = 8192;
    private NpcData _npcData;
    private float _defaultRunSpeed;
    private float _defaultWalkSpeed;
    private CharacterController _characterController;
    private readonly string _walk = "walk";
    private readonly string _run = "run";
    private readonly string _wait = "wait";

    public NpcData NpcData { get { return _npcData; } set { _npcData = value; } }

    public float DefaultRunSpeed { get { return _defaultRunSpeed; } set { _defaultRunSpeed = value; } }
    public float DefaultWalkSpeed { get { return _defaultWalkSpeed; } set { _defaultWalkSpeed = value; } }











  




  
}
