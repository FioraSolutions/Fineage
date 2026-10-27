/// <summary>
/// Object pool runtime access with [Inject] wiring and singleton fallback (#142 sub-slice 44).
/// </summary>
public static class ObjectPoolPacketServices
{
    private static ModelTable _models;

    public static void Bind(ModelTable models = null)
    {
        if (models != null)
            _models = models;
    }

    public static ModelTable Models =>
        _models != null ? _models : ModelTable.Instance;
}
