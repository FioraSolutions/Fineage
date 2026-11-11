using System;

public partial class StatsConverter
{
    private static StatsConverter _instance;

    [System.Obsolete("Use [Inject] StatsConverter or StatsConverterPacketServices.Converter instead")]
    public static StatsConverter Instance {
        get {
            if (_instance == null) {
                _instance = new StatsConverter();
            }
            return _instance;
        }
    }









}
