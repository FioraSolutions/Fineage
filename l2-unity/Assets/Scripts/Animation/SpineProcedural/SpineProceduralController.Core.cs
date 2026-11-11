using System;

public partial class SpineProceduralController : MonoBehaviour
{
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        WireSpineProceduralPacketServices();
    }

    private void OnDestroy()
    {
        if (_instance == this)
            _instance = null;
    }

    private void Start()
    {
        WireSpineProceduralPacketServices();
    }

    private void WireSpineProceduralPacketServices()
    {
        SpineProceduralPacketServices.Bind(controller: this);
    }
}
