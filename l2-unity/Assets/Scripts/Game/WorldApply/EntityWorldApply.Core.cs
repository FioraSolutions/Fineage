using UnityEngine;

public abstract partial class EntityWorldApply
{
    public virtual void OnMoveTo(Entity entity, Vector3 destination, Vector3 current, CharMoveToLocationDto dto)
    {
    }

    /// <summary>
    /// Mobius <c>StopRotation</c> rides on PositionCorrection: when <paramref name="heading"/> is a real L2 heading
    /// (0..65535) the pawn must face it, not only move. <c>-1</c> means "position only" (ValidatePosition drift).
    /// </summary>
    public virtual void OnPositionCorrection(Entity entity, Vector3 position, int heading = -1)
    {
        SnapTransform(entity, position, heading);
    }

    public static void SnapTransform(Entity entity, Vector3 position, int heading = -1)
    {
        if (entity == null)
            return;
        // A live yaw drive would step the pawn back toward its old target next LateUpdate.
        if (heading >= 0)
        {
            L2PawnYawDrive drive = entity.GetComponent<L2PawnYawDrive>();
            if (drive != null)
                drive.Stop();
        }

        CharacterController cc = entity.GetComponent<CharacterController>();
        if (cc != null && cc.enabled)
            cc.enabled = false;
        entity.transform.position = position;
        if (heading >= 0)
        {
            float yaw = VectorUtils.HeadingToUnityQuaternionForNpc(heading);
            entity.transform.rotation = Quaternion.Euler(0f, yaw, 0f);
            if (entity.Identity != null)
            {
                entity.Identity.OrigHeading = yaw;
                entity.Identity.Heading = Quaternion.Euler(0f, yaw, 0f);
            }
        }
        if (cc != null)
            cc.enabled = true;
    }

    public virtual void OnStopMove(Entity entity, StopMoveDto dto)
    {
    }

    public virtual void OnMoveToPawn(Entity entity, MoveToPawnDto dto)
    {
    }

    public virtual void OnDie(Entity entity, DieDto dto)
    {
    }

    public virtual void OnRevive(Entity entity, ReviveDto dto)
    {
    }
}
