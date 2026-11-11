using UnityEngine;

public sealed partial class HomeProjectileMover
{
    void LateUpdate()
    {
        if (!_isLaunched)
        {
            return;
        }

        DetachFromFollow();

        if (_context == null || _context.CasterTransform == null)
        {
            Finish();
            return;
        }

        if (_maxLifetime > 0f && Time.time - _startedAt >= _maxLifetime)
        {
            Finish();
            return;
        }

        Vector3 target = ResolveHomePosition();
        Vector3 current = transform.position;
        float distanceToHome = Vector3.Distance(current, target);
        float distanceToCasterRoot = Vector3.Distance(current, _context.CasterTransform.position);
        float distance = Mathf.Min(distanceToHome, distanceToCasterRoot);

        if (_arrivalFadeStarted)
        {
            SyncOwnerWorldPosOverride();
            if (Time.time >= _fadeCompleteAt || distance <= _arriveDistance)
            {
                Finish();
            }

            return;
        }

        if (distance <= _fadeStartDistance)
        {
            BeginArrivalFade();
            return;
        }

        Vector3 next = ComputeNextPosition(current, target);
        Vector3 velocity = next - current;
        transform.position = next;
        SyncOwnerWorldPosOverride();

        if (_pathCompleted)
        {
            BeginArrivalFade();
            return;
        }

        if (_rotateToVelocity && velocity.sqrMagnitude > 0.000001f)
        {
            transform.rotation = Quaternion.LookRotation(velocity.normalized, Vector3.up);
        }
    }

    void BeginArrivalFade()
    {
        _arrivalFadeStarted = true;
        _fadeCompleteAt = Time.time + _fadeOutSeconds;
        SyncOwnerWorldPosOverride();

        if (_coordinator != null)
        {
            HideAndDestroyFlightObject();
            _coordinator.NotifyMoverArrived();
        }
        else if (_effect != null)
        {
            _effect.DestroyHomeArrivalImmediate();
        }
        else
        {
            Destroy(gameObject);
        }

        _isLaunched = false;
        enabled = false;
    }

    Vector3 ResolveHomePosition()
    {
        if (_context == null || _context.CasterTransform == null)
        {
            return _startPosition;
        }

        if (_attachmentResolver != null &&
            _attachmentResolver.Resolve(_homeAttachmentPoint, _context, out _, out Vector3 worldPosition))
        {
            if (_homeOffset.sqrMagnitude > 0.000001f)
            {
                worldPosition += _context.CasterTransform.TransformDirection(_homeOffset);
            }

            return worldPosition;
        }

        return _context.CasterTransform.position + _homeOffset;
    }
}
