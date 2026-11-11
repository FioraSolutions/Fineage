using UnityEngine;
using VContainer;

public partial class AbstractMeshManager : MonoBehaviour
{
    [Inject] private ModelTable _models;

    protected ModelTable Models =>
        _models != null ? _models : TablePacketServices.Models;

    protected GameObject _go;












}
