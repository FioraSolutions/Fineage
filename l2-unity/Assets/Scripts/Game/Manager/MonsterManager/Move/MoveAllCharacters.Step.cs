using System;

public partial class MoveAllCharacters
{
    private void MoveToPoint(int key, Vector3 target, MovementData data)
    {
        if (!data.IsEntity())
        {
            _removeListMove.Add(key);
            return;
        }

        Transform transform = data.GetTransform();
        if (transform == null)
        {
            _removeListMove.Add(key);
            return;
        }

        if (data.IsOwnerDead())
        {
            data.SetIsMove(false);
            _removeListMove.Add(key);
            data.SetLastPosition(transform.position);
            return;
        }

        Vector3 before = transform.position;
        if (!data.IsMove())
        {
            _removeListMove.Add(key);
            data.SetLastPosition(before);
            return;
        }

        float stopDistance = data.GetDistance();
        float dx = target.x - before.x;
        float dz = target.z - before.z;
        float distSq = dx * dx + dz * dz;
        if (distSq < stopDistance * stopDistance)
        {
            data.OnFinish(target);
            _removeListMove.Add(key);
            data.SetLastPosition(transform.position);
            return;
        }

        if (distSq <= 0.0001f)
        {
            data.SetLastPosition(transform.position);
            return;
        }

        data.AdvanceMoveTimer();
        data.SyncUserGait();
        float speed = data.GetSpeed();
        if (L2PawnWalkEntity.IsCharInfoPawn(data.GetEntity()))
        {
            data.StepCharInfo(target, speed);
        }
        else
        {
            Vector3 direction = new Vector3(dx, 0f, dz);
            float dist = Mathf.Sqrt(distSq);
            direction.x /= dist;
            direction.z /= dist;

            // Clamp the final step so the pawn lands exactly on target instead
            // of overshooting it. Interlude calcVelocity does Dest/dt when the
            // remaining distance is shorter than one frame step; we reproduce that
            // as dist/dt here so monsters/NPCs stop without a cut-off jerk.
            float stepLength = speed * Time.deltaTime;
            if (stepLength >= dist)
                speed = dist / Time.deltaTime;

            data.Move(direction, speed);
        }

        data.SetLastPosition(transform.position);
    }

    private void TurnTowardsAttacker(Vector3 attackerPosition, Transform mTranform)
    {
        Vector3 directionToAttacker = attackerPosition - mTranform.position;
        directionToAttacker.y = 0f;
        if (directionToAttacker.sqrMagnitude < 0.0001f)
            return;
        Quaternion lookRotation = Quaternion.LookRotation(directionToAttacker);
        //default 25
        mTranform.rotation = Quaternion.Slerp(mTranform.rotation, lookRotation, Time.deltaTime * 15f);
    }

    private void RemoveFinishKeyMove()
    {
        foreach (int key in _removeListMove)
        {
            MovementData data;
            if (_moveDict.TryGetValue(key, out data))
            {
                _stoppedMoveData[key] = data;
                _moveDict.Remove(key);
            }
        }

        _removeListMove.Clear();
    }

    private void ReconcileStoppedMoves()
    {
        if (_stoppedMoveData == null || _stoppedMoveData.Count == 0)
            return;

        foreach (KeyValuePair<int, MovementData> pair in _stoppedMoveData)
        {
            MovementData current;
            if (_moveDict.TryGetValue(pair.Key, out current) &&
                current != null &&
                current.IsMove())
            {
                _reconciledStoppedMoves.Add(pair.Key);
                continue;
            }

            if (pair.Value == null || pair.Value.ReconcileStoppedAnimation())
                _reconciledStoppedMoves.Add(pair.Key);
        }

        for (int i = 0; i < _reconciledStoppedMoves.Count; i++)
            _stoppedMoveData.Remove(_reconciledStoppedMoves[i]);
        _reconciledStoppedMoves.Clear();
    }

    private void RemoveFinishKeyRotate()
    {
        foreach (int key in _removeListRotate)
        {
            if (_rotateDict.ContainsKey(key))
            {
                _rotateDict.Remove(key);
            }
        }

        _removeListRotate.Clear();
    }

}
