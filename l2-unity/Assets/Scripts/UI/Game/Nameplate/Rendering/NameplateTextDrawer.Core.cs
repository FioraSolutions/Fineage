using System.Collections.Generic;
using UnityEngine;

public sealed partial class NameplateTextDrawer
{
    public void ConfigurePaths(string atlasResourcePath, string csvResourcePath)
    {
        _atlasResourcePath = atlasResourcePath;
        _csvResourcePath = csvResourcePath;
    }

    public void EnsureResources()
    {
        if (_font == null)
        {
            _font = L2BitmapFont.LoadFromResources(_atlasResourcePath, _csvResourcePath);
        }

        if (_font != null)
        {
            _batch.EnsureMaterial(_font.Atlas);
        }
    }
}
