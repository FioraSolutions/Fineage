using System;

public partial class SpineProceduralController : MonoBehaviour
{
    private Dictionary<Transform, BoneModification> _activeModifications = new Dictionary<Transform, BoneModification>();
    private List<Transform> _bonesToRemove = new();
    public float fadeSpeed = 10f; // Скорость затухания (чем выше, тем быстрее вернется)

    private static SpineProceduralController _instance;

    [System.Obsolete("Use [Inject] instead")]
    public static SpineProceduralController Instance => _instance;









}
