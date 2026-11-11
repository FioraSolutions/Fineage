using System;

public partial class MoveAllCharacters : MonoBehaviour, IMoveAllCharacters
{
    public void OnDestroy()
    {
        if (ReferenceEquals(_instance, this))
            _instance = null;
        if (_moveDict == null)
            return; // duplicate destroyed in Awake: never initialized
        _moveDict.Clear();
        _rotateDict.Clear();
        _removeListMove.Clear(); 
        _removeListRotate.Clear(); 
        _stoppedMoveData.Clear();
        _reconciledStoppedMoves.Clear();
        _rotateDict.Clear(); 
        _removeListMove.Clear();
        _removeListRotate.Clear();

        _moveDict = null;
        _rotateDict = null;
        _removeListMove = null;
        _removeListRotate = null;
        _stoppedMoveData = null;
        _reconciledStoppedMoves = null;
        _rotateDict = null;
        _removeListMove = null;
        _removeListRotate = null;


        Destroy(gameObject);
    }
}
