using System;

public partial class MoveAllCharacters : MonoBehaviour, IMoveAllCharacters
{
    void StepMoves()
    {
        ReconcileStoppedMoves();
        if (_moveDict.Count == 0)
        {
            MoveCostLog.NoteStep(0, 0, 0);
            MoveCostLog.EndFrame();
            return;
        }

        long start = System.Diagnostics.Stopwatch.GetTimestamp();
        int moving = 0;
        foreach (KeyValuePair<int, MovementData> kvp in _moveDict)
        {
            MovementData data = kvp.Value;
            bool isMove = data.IsMove();
            MovementTarget movementTarget = data.GetMovementTarget();

            if (isMove == true | data.IsEntity())
            {
                if (movementTarget == null || !movementTarget.TryGetTarget(out Vector3 target))
                {
                    _removeListMove.Add(kvp.Key);
                    continue;
                }

                if (isMove)
                    moving++;
                MoveToPoint(kvp.Key, target, data);
            }
            else
            {

                _removeListMove.Add(kvp.Key);
            }

        }

        RemoveFinishKeyMove();
        double ms = (System.Diagnostics.Stopwatch.GetTimestamp() - start) * 1000.0 / System.Diagnostics.Stopwatch.Frequency;
        MoveCostLog.NoteStep(moving, moving, ms);
        MoveCostLog.EndFrame();
    }

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
