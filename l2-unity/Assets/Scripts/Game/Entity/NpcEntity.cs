using UnityEngine;

public partial class NpcEntity : NetworkEntity
{
    private CharacterAnimationAudioHandler _npcAnimationAudioHandler;

    private static readonly string _walk = "walk";
    private static readonly string _run = "run";
    private static readonly string _wait = "wait";
    private string[] allAnim = new string[] { _walk , _run , _wait, };
    private CharacterController _characterController;
    [SerializeField] private NpcData _npcData;

    public NpcData NpcData { get { return _npcData; } set { _npcData = value; } }
















}
