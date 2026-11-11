using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

public sealed partial class CastClipPlayable
{
    public void Dispose()
    {
        _active = false;
        _finished = true;
        _ready = false;
        if (_graph.IsValid())
        {
            _graph.Destroy();
        }

        _clipPlayable = default;
        _controllerPlayable = default;
        _mixer = default;
    }
}
