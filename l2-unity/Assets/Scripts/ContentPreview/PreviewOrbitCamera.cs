using UnityEngine;

/// <summary>
/// Simple orbit camera for the Content Preview scene.
/// LMB drag = orbit, scroll = zoom, MMB drag = pan.
/// </summary>
public class PreviewOrbitCamera : MonoBehaviour
{
    private Vector3 _pivot = Vector3.up;
    private float _distance = 5f;
    private float _yaw = 0f;
    private float _pitch = 20f;

    [Header("Settings")]
    public float orbitSpeed = 0.3f;
    public float zoomSpeed = 0.5f;
    public float panSpeed = 0.005f;
    public float minDistance = 0.5f;
    public float maxDistance = 30f;

    public void SetTarget(Vector3 center, float distance)
    {
        _pivot = center;
        _distance = Mathf.Clamp(distance, minDistance, maxDistance);
        ApplyTransform();
    }

    private void LateUpdate()
    {
        // Orbit (LMB)
        if (Input.GetMouseButton(0))
        {
            _yaw += Input.GetAxis("Mouse X") * orbitSpeed * 5f;
            _pitch -= Input.GetAxis("Mouse Y") * orbitSpeed * 5f;
            _pitch = Mathf.Clamp(_pitch, -89f, 89f);
        }

        // Pan (MMB)
        if (Input.GetMouseButton(2))
        {
            Vector3 right = transform.right;
            Vector3 up = transform.up;
            _pivot -= (right * Input.GetAxis("Mouse X") + up * Input.GetAxis("Mouse Y"))
                      * panSpeed * _distance;
        }

        // Zoom (scroll)
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.001f)
            _distance = Mathf.Clamp(_distance - scroll * zoomSpeed * _distance, minDistance, maxDistance);

        ApplyTransform();
    }

    private void ApplyTransform()
    {
        Quaternion rotation = Quaternion.Euler(_pitch, _yaw, 0f);
        transform.position = _pivot + rotation * (Vector3.back * _distance);
        transform.LookAt(_pivot);
    }
}
