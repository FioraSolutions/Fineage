using UnityEngine;
using System.Collections.Generic;

public partial class L2RibbonEmitter
{
    private void ClearMesh()
    {
        if (_mesh == null)
        {
            return;
        }

        _mesh.Clear();
    }
}
