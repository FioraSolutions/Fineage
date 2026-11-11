using System;

public partial class SystemMessageTable
{
    private static SystemMessageTable _instance;
    [Obsolete("Use [Inject] / PacketServices instead")]
    public static SystemMessageTable Instance {
        get {
            if (_instance == null) {
                _instance = new SystemMessageTable();
            }

            return _instance;
        }
    }

    private Dictionary<int, SystemMessageDat> _systemMessages;
    public Dictionary<int, SystemMessageDat> SystemMessages { get { return _systemMessages; } }



}
