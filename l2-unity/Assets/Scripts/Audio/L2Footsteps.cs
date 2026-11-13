using UnityEngine;

/// <summary>
/// Local pawn steps. Remote bodies already fire steps from the run state.
/// Walk uses the original <c>default_walk</c> bed; run uses the surface bed
/// (dirt, stone, wood, grass) and falls back to <c>default_run</c>.
/// </summary>
public sealed class L2Footsteps : MonoBehaviour
{
    const float WalkInterval = 0.48f;
    const float RunInterval = 0.32f;
    const float MinSpeed = 0.35f;

    Vector3 _last;
    float _carry;
    bool _hasLast;

    void LateUpdate()
    {
        PlayerEntity player = DomainPacketServices.LocalPlayer;
        if (player == null)
        {
            _hasLast = false;
            return;
        }

        Vector3 pos = player.transform.position;
        if (!_hasLast)
        {
            _last = pos;
            _hasLast = true;
            return;
        }

        float dt = Time.deltaTime;
        if (dt <= 0.0001f)
            return;

        Vector3 flat = pos - _last;
        flat.y = 0f;
        float speed = flat.magnitude / dt;
        _last = pos;
        if (speed < MinSpeed)
        {
            _carry = 0f;
            return;
        }

        bool running = player.Running && speed > 1.7f;
        _carry += dt;
        float interval = running ? RunInterval : WalkInterval;
        if (_carry < interval)
            return;
        _carry = 0f;

        AudioManager audio = AudioPacketServices.Audio;
        if (audio == null)
            return;
        audio.PlayStepSound(Surface(pos), pos, running);
    }

    static string Surface(Vector3 pos)
    {
        World world = AudioPacketServices.GameWorld;
        if (world == null)
            world = ControllerPacketServices.GameWorld;
        if (world == null)
            return null;
        if (!Physics.Raycast(pos + Vector3.up, Vector3.down, out RaycastHit hit, 3f, world.GroundMask, QueryTriggerInteraction.Ignore))
            return null;
        return hit.collider != null ? hit.collider.gameObject.tag : null;
    }
}
