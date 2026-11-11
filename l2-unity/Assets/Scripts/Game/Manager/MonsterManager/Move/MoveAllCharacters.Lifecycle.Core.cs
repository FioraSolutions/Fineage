using System;

public partial class MoveAllCharacters : MonoBehaviour, IMoveAllCharacters
{
    private void Awake()
    {
        if ((_instance as UnityEngine.Object) == null)
        {
            _instance = this;
            _moveDict = new Dictionary<int, MovementData>();
            _rotateDict = new Dictionary<int, RotateData>();
            _removeListMove = new List<int>();
            _removeListRotate = new List<int>();
            _stoppedMoveData = new Dictionary<int, MovementData>();
            _reconciledStoppedMoves = new List<int>();
            CrowdAnimTest.EnsureTimeCap();
            MoveAllCharactersPacketServices.Bind(moves: this);
        }
        else
        {
            Destroy(this);
        }
    }
}
