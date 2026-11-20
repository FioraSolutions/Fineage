using UnityEngine;

/// <summary>Interlude quest radar arrow on the HUD — shared target from <see cref="QuestJournalState.Direction"/>.</summary>
public sealed class QuestHudDirectionMarker : MonoBehaviour
{
    private const float EdgeInsetPx = 36f;
    private const float ArrowSizePx = 28f;
    private static readonly Color ArrowColor = new(0.95f, 0.82f, 0.15f, 0.95f);

    private Camera _camera;

    private void LateUpdate()
    {
        QuestDirectionTarget target = QuestJournalState.Direction;
        if (!target.Show || target.X == 0 && target.Y == 0)
            return;

        if (_camera == null)
            _camera = Camera.main;
        if (_camera == null)
            return;

        if (!QuestPacketServices.IsTrackingEnabled())
            return;

        Vector3 world = VectorUtils.ConvertPosToUnity(new Vector3(target.X, target.Y, target.Z));
        Vector3 screen = _camera.WorldToScreenPoint(world);
        if (screen.z < 0f)
            return;

        float w = Screen.width;
        float h = Screen.height;
        var center = new Vector2(w * 0.5f, h * 0.5f);
        var pos = new Vector2(screen.x, screen.y);
        float dist = Vector2.Distance(center, pos);
        if (dist < 64f)
            return;

        Vector2 dir = (pos - center).normalized;
        float halfW = w * 0.5f - EdgeInsetPx;
        float halfH = h * 0.5f - EdgeInsetPx;
        float scale = Mathf.Min(
            halfW / Mathf.Max(Mathf.Abs(dir.x), 0.001f),
            halfH / Mathf.Max(Mathf.Abs(dir.y), 0.001f));
        Vector2 edge = center + dir * scale;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        DrawArrow(edge, angle - 90f);
    }

    static void DrawArrow(Vector2 center, float angleDeg)
    {
        Matrix4x4 prev = GUI.matrix;
        GUI.color = ArrowColor;
        GUI.matrix = Matrix4x4.TRS(center, Quaternion.Euler(0f, 0f, angleDeg), Vector3.one);
        GUI.DrawTexture(new Rect(-ArrowSizePx * 0.5f, -ArrowSizePx, ArrowSizePx, ArrowSizePx), Texture2D.whiteTexture);
        GUI.matrix = prev;
        GUI.color = Color.white;
    }
}