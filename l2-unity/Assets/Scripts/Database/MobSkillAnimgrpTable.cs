using System;

public sealed partial class MobSkillAnimgrpTable
{
    private static MobSkillAnimgrpTable _instance;

    [Obsolete("Use [Inject] / PacketServices instead")]
    public static MobSkillAnimgrpTable Instance
    {
        get
        {
            if (_instance == null)
                _instance = new MobSkillAnimgrpTable();
            return _instance;
        }
    }

    private readonly Dictionary<long, string> _sequences = new Dictionary<long, string>();



}
