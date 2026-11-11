using System.Collections.Generic;
using UnityEngine;

public sealed partial class NameplateCastBarDrawer
{
    public void EnsureResources()
    {
        if (_texBg == null)
            _texBg = L2NameplateBubbles.LoadTexture(BgResourcePath);
        if (_texFill == null)
            _texFill = L2NameplateBubbles.LoadTexture(FillResourcePath);

        if (_texBg != null)
            _batchBg.EnsureMaterial(_texBg);
        if (_texFill != null)
            _batchFill.EnsureMaterial(_texFill);
    }
}
