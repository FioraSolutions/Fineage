using System;

public partial class MoveAllCharacters : MonoBehaviour, IMoveAllCharacters
{
    private static IMoveAllCharacters _instance;

    [System.Obsolete("Use MoveAllCharactersPacketServices.Moves or [Inject] IMoveAllCharacters instead")]
    public static IMoveAllCharacters Instance { get { return (_instance as UnityEngine.Object) != null ? _instance : null; } }

    private Dictionary<int, MovementData> _moveDict;
    private Dictionary<int, RotateData> _rotateDict;
    private List<int> _removeListMove;
    private List<int> _removeListRotate;
    private Dictionary<int, MovementData> _stoppedMoveData;
    private List<int> _reconciledStoppedMoves;


















}
