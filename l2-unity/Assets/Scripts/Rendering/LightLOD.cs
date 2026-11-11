#if UNITY_EDITOR
public partial class LightLOD : MonoBehaviour
{
    Light _light;
    bool _builtInRp;

    [SerializeField]
    [Range(0, 15)]
    float _updateDelay = 1f;
    [SerializeField] float _squareDistanceFromCamera;
    [SerializeField] float _lastUpdate;

    [SerializeField]
    List<LODAdjustment> LODLevels = new();

    bool _ready;






}
