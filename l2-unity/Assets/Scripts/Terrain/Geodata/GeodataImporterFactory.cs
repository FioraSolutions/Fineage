using System;

public partial class GeodataImporterFactory : MonoBehaviour
{
    //Delegates are a variable that points to a function
    public delegate void ImportJobComplete(Node[,,] geodata);

    //The maximum simultaneous threads we allow to open
    [SerializeField] private int _maxJobs = 16;

    private List<GeodataImporter> _currentJobs;
    private List<GeodataImporter> _todoJobs;

    private static GeodataImporterFactory _instance;
    [Obsolete("Use [Inject] / PacketServices instead")]
    public static GeodataImporterFactory Instance { get { return _instance; } }




}
