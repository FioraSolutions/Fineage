using UnityEngine;

public sealed partial class NpcSpawner
{
    public NpcSpawner(ObjectPoolManager pool, AppearFadeService appearFade, NpcDecoService deco)
    {
        _pool = pool;
        _appearFade = appearFade;
        _deco = deco;
    }
}
