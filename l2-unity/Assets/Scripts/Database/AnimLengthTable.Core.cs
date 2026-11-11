using System;

public partial class AnimLeghtTable
{
    public void Initialize()
    {
        if(_leght != null) _leght.Clear();
        Read();

    }

    public float GetLeghtMs(string key)
    {
        if (_leght.ContainsKey(key))
        {
            return _leght[key] * 1000;
        }
        return 0f;
    }
}
