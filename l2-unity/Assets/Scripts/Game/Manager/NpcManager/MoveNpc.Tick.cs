using UnityEngine;

public partial class MoveNpc : MonoBehaviour
{
    private void MoveToPoint(Vector3 target, bool ignoreDist)
    {

        if (target != null)
        {
            var gravityOffTransform = new Vector3(transform.position.x, 0, transform.position.z);
            var gravityOffTarget = new Vector3(target.x, 0, target.z);
            float monsterSpeed = GetMonsterSpeed();
            // пїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅ пїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅ пїЅпїЅ пїЅпїЅпїЅпїЅ
            float distance = Vector2.Distance(gravityOffTarget, gravityOffTransform);


            if (distance >= stoppingDistance | ignoreDist == true)
            {

                Vector3 point = gravityOffTarget - gravityOffTransform;
                Vector3 lookDir = point.normalized;
                float vy = ApplyGravity(Time.deltaTime);

                // Clamp the final step to land exactly on target (Interlude
                // calcVelocity: Dest/dt when remaining distance < one frame step),
                // so NPCs stop without overshooting / a cut-off jerk.
                float stepLength = monsterSpeed * Time.deltaTime;
                if (stepLength >= distance)
                    monsterSpeed = distance / Time.deltaTime;

                if (_isTeleport == false)
                {
                    _detectedIsMove = true;
                    // Planar velocity scaled by run speed, gravity integrated
                    // separately (matches PlayerController) so vertical speed is
                    // not masked/"lightened" by monsterSpeed.
                    Vector3 step = lookDir * monsterSpeed * Time.deltaTime;
                    step.y = vy * Time.deltaTime;
                    _controller.Move(step);

                    if (lookDir.sqrMagnitude > 0.0001f)
                    {
                        Quaternion lookRotation = Quaternion.LookRotation(lookDir);
                        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5.0f);
                    }
                }

                if (_lastPos == transform.position)
                {
                    OnFinish(target);
                }
            }
            else
            {
                OnFinish(target);
            }

            _lastPos = transform.position;
        }
    }

    private void OnFinish(Vector3 target)
    {
        // Debug.Log("MoveMonster: пїЅпїЅ пїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅ пїЅпїЅпїЅпїЅпїЅ пїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅ пїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅ пїЅ пїЅпїЅпїЅпїЅ!!!! " + this.name);
        _isMove = false;

        Vector3 dest = new Vector3(target.x, transform.position.y, target.z);
        transform.position = GroundSnapHelper.SnapToGroundOrKeep(dest);
        _detectedIsMove = false;
        if (EntityActionPacketServices.Actions != null)
            EntityActionPacketServices.Actions.NotifyArrived(_entity);
    }

    public bool IsMoving()
    {
        return _isMove;
    }

    public void SetAccessTeleport(bool isTeleport)
    {
        //this._isTeleport = isTeleport;
    }
}
