using System;

public partial class NpcDecoEffectsTable
{
    static NpcDecoEffectsTable _instance;

    [Obsolete("Use [Inject] / PacketServices instead")]
    public static NpcDecoEffectsTable Instance
    {
        get
        {
            if (_instance == null)
                _instance = new NpcDecoEffectsTable();
            return _instance;
        }
    }

    Dictionary<int, NpcDecoEffect> _byNpcId = new Dictionary<int, NpcDecoEffect>();
    bool _loaded;



}
